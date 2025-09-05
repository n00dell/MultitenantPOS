using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Config;
using MultitenantPOS.Module.BusinessObjects.ProductSetup;
using OfficeOpenXml; // Requires EPPlus NuGet package
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MultitenantPOS.Module.Controllers
{
    public partial class ExcelBulkUploadController : ObjectViewController<ListView, Product>
    {
        private PopupWindowShowAction uploadProductsAction;

        public ExcelBulkUploadController()
        {
            InitializeComponent();

            uploadProductsAction = new PopupWindowShowAction(this, "UploadProducts", PredefinedCategory.View)
            {
                Caption = "Upload Products",
                ImageName = "BO_FileAttachment",
                ToolTip = "Upload Products from Excel file",
                TargetObjectType = typeof(Product),
                TargetViewType = ViewType.ListView,
                TypeOfView = typeof(ListView)
            };
            uploadProductsAction.Execute += UploadProductsAction_Execute;
            uploadProductsAction.CustomizePopupWindowParams += UploadProductsAction_CustomizePopupWindowParams;
        }

        private void UploadProductsAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            // Use ObjectSpace specific to UploadFileParameters
            IObjectSpace os = Application.CreateObjectSpace(typeof(UploadFileParameters));
            if (os == null)
            {
                throw new UserFriendlyException("Failed to create ObjectSpace for UploadFileParameters.");
            }

            // Create the UploadFileParameters object
            var uploadParams = os.CreateObject<UploadFileParameters>();
            if (uploadParams == null)
            {
                throw new UserFriendlyException("Failed to create UploadFileParameters object.");
            }

            e.DialogController.SaveOnAccept = false;
            e.View = e.Application.CreateDetailView(os, uploadParams);
            e.IsSizeable = false;
            e.Size = new Size(400, 200);
        }

        private void UploadProductsAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            try
            {
                if (e.PopupWindowViewCurrentObject == null)
                {
                    throw new UserFriendlyException("No file selected. Please upload an Excel file.");
                }

                UploadFileParameters uploadParams = (UploadFileParameters)e.PopupWindowViewCurrentObject;

                if (uploadParams.File == null || uploadParams.File.Content == null || uploadParams.File.Content.Length == 0)
                {
                    throw new UserFriendlyException("No valid file provided. Please upload a .xlsx file.");
                }

                if (string.IsNullOrEmpty(uploadParams.File.FileName) ||
                    !uploadParams.File.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UserFriendlyException("Invalid file format. Please upload a .xlsx file.");
                }

                IObjectSpace objectSpace = Application.CreateObjectSpace();
                if (objectSpace == null)
                {
                    throw new UserFriendlyException("Failed to create ObjectSpace.");
                }

                Session currentSession = ((XPObjectSpace)objectSpace).Session;
                if (currentSession == null)
                {
                    throw new UserFriendlyException("Failed to access database session.");
                }

                UnitOfWork uow = new UnitOfWork(currentSession.DataLayer);
                int totalCreated = 0;

                ExcelPackage.License.SetNonCommercialPersonal("Lee");
                using (var stream = uploadParams.GetFileStream())
                {
                    if (stream.Length == 0)
                    {
                        throw new UserFriendlyException("Uploaded file is empty.");
                    }

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            throw new UserFriendlyException("Excel file contains no worksheets.");
                        }
                        totalCreated = ProcessExcelUpload(uow, worksheet);
                    }
                }

                e.PopupWindowView.ObjectSpace.CommitChanges();
                View.ObjectSpace.Refresh();

                if (totalCreated > 0)
                {
                    Application.ShowViewStrategy.ShowMessage($"Products uploaded successfully! {totalCreated} records created.", InformationType.Success);
                }
                else
                {
                    Application.ShowViewStrategy.ShowMessage("Data was uploaded successfully but no data changed.", InformationType.Success);
                }
            }
            catch (Exception ex)
            {
                Application.ShowViewStrategy.ShowMessage($"Error uploading data: {ex.Message}", InformationType.Error);
            }
        }

        private int ProcessExcelUpload(UnitOfWork uow, ExcelWorksheet worksheet)
        {
            int totalCreated = 0;

            if (worksheet == null)
            {
                throw new UserFriendlyException("No worksheet found in the Excel file.");
            }

            totalCreated += ProcessWorksheet(uow, worksheet);
            uow.CommitChanges();

            return totalCreated;
        }

        private int ProcessWorksheet(UnitOfWork uow, ExcelWorksheet worksheet)
        {
            int createdCount = 0;

            for (int i = 2; i <= worksheet.Dimension?.Rows; i++) // Start from row 2 to skip headers
            {
                if (!IsRowEmpty(worksheet, i))
                {
                    if (ProcessRow(uow, worksheet, i))
                    {
                        createdCount++;
                    }
                }
            }

            return createdCount;
        }

        private bool ProcessRow(UnitOfWork uow, ExcelWorksheet worksheet, int rowIndex)
        {
            try
            {
                string name = GetCellValue(worksheet, rowIndex, 1);
                string sku = GetCellValue(worksheet, rowIndex, 2);
                decimal price = decimal.TryParse(GetCellValue(worksheet, rowIndex, 3), out var p) ? p : 0;
                decimal cost = decimal.TryParse(GetCellValue(worksheet, rowIndex, 4), out var c) ? c : 0;
                decimal vat = decimal.TryParse(GetCellValue(worksheet, rowIndex, 5), out var v) ? v : 0;
                string categoryName = GetCellValue(worksheet, rowIndex, 6);
                string unitName = GetCellValue(worksheet, rowIndex, 7);
                decimal taxVAT = decimal.TryParse(GetCellValue(worksheet, rowIndex, 8), out var tv) ? tv : 0;
                string attrName = GetCellValue(worksheet, rowIndex, 9);
                string attrValue = GetCellValue(worksheet, rowIndex, 10);
                string attrShortCode = GetCellValue(worksheet, rowIndex, 11);

                if (string.IsNullOrEmpty(name))
                {
                    return false; // Skip rows with empty product name
                }

                // Find or create Category
                Category category = null;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    category = uow.FindObject<Category>(
                        CriteriaOperator.Parse("Name == ?", categoryName));
                    if (category == null)
                    {
                        category = new Category(uow)
                        {
                            Name = categoryName,
                            ShortCode = categoryName.Substring(0, Math.Min(10, categoryName.Length)).ToUpper()
                        };
                        category.Save();
                    }
                }

                // Find or create UnitofMeasure
                UnitofMeasure unit = null;
                if (!string.IsNullOrEmpty(unitName))
                {
                    unit = uow.FindObject<UnitofMeasure>(
                        CriteriaOperator.Parse("Name == ?", unitName));
                    if (unit == null)
                    {
                        unit = new UnitofMeasure(uow)
                        {
                            Name = unitName
                        };
                        unit.Save();
                    }
                }

                // Check if product already exists by SKU or Name (if SKU is empty)
                Product existingProduct = null;
                if (!string.IsNullOrEmpty(sku))
                {
                    existingProduct = uow.FindObject<Product>(
                        CriteriaOperator.Parse("SKU == ?", sku));
                }
                else
                {
                    // If no SKU provided, check by name to avoid duplicates
                    existingProduct = uow.FindObject<Product>(
                        CriteriaOperator.Parse("Name == ?", name));
                }

                if (existingProduct != null)
                {
                    return false; // Product already exists
                }

                // Create new Product
                var product = new Product(uow)
                {
                    Name = name,
                    Price = price,
                    Cost = cost,
                    VAT = vat,
                    Category = category,
                    Unit = unit,
                    IsActive = true
                };

                // Set SKU if provided, otherwise let the Product class generate it
                if (!string.IsNullOrEmpty(sku))
                {
                    product.SKU = sku;
                }

                // Save the product first
                product.Save();

                // Handle ProductAttribute after product is saved
                if (!string.IsNullOrEmpty(attrName) && !string.IsNullOrEmpty(attrValue))
                {
                    var attribute = new ProductAttribute(uow)
                    {
                        AttributeName = attrName,
                        Value = attrValue,
                        ShortCode = string.IsNullOrEmpty(attrShortCode) ?
                                   attrValue.Substring(0, Math.Min(5, attrValue.Length)).ToUpper() :
                                   attrShortCode.ToUpper(),
                        Product = product
                    };
                    attribute.Save();
                }

                // Handle TaxSetup after product is saved
                if (taxVAT != 0)
                {
                    // Check if tax setup already exists for this product
                    var existingTax = uow.FindObject<TaxSetup>(
                        CriteriaOperator.Parse("Product == ? AND VAT == ?", product, taxVAT));

                    if (existingTax == null)
                    {
                        var tax = new TaxSetup(uow)
                        {
                            VAT = taxVAT,
                            Product = product
                        };
                        tax.Save();
                        product.Tax = tax;
                        product.Save();
                    }
                    else
                    {
                        product.Tax = existingTax;
                        product.Save();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"Error processing row {rowIndex}: {ex.Message}");
            }
        }

        private string GetCellValue(ExcelWorksheet worksheet, int row, int column)
        {
            try
            {
                var cellValue = worksheet.Cells[row, column]?.Value;
                if (cellValue == null)
                    return string.Empty;

                return cellValue.ToString()?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException($"Error reading cell at row {row}, column {column}: {ex.Message}");
            }
        }

        private bool IsRowEmpty(ExcelWorksheet worksheet, int row)
        {
            try
            {
                if (worksheet.Dimension == null)
                    return true;

                int maxColumn = Math.Min(11, worksheet.Dimension.Columns);
                for (int i = 1; i <= maxColumn; i++)
                {
                    if (!string.IsNullOrEmpty(GetCellValue(worksheet, row, i)))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return true; // If we can't read the row, consider it empty
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }

    [DomainComponent]
    [NonPersistent]
    public class UploadFileParameters
    {
        public UploadFileParameters()
        {
            File = new TempFileData();
        }

        [EditorAlias("FileDataPropertyEditor")]
        public TempFileData File { get; set; }

        // Helper method to get the file as a stream
        public Stream GetFileStream()
        {
            return new MemoryStream(File.Content ?? Array.Empty<byte>());
        }
    }

    [NonPersistent]
    public class TempFileData : IFileData
    {
        public TempFileData()
        {
            Content = Array.Empty<byte>();
            FileName = string.Empty;
        }

        public string FileName { get; set; }

        public byte[] Content { get; set; }

        public int Size => Content?.Length ?? 0;

        public void Clear()
        {
            Content = Array.Empty<byte>();
            FileName = string.Empty;
        }

        public void LoadFromStream(string fileName, Stream stream)
        {
            FileName = fileName;
            Content = new byte[stream.Length];
            stream.Read(Content, 0, (int)stream.Length);
        }

        public void SaveToStream(Stream stream)
        {
            if (Content != null)
            {
                stream.Write(Content, 0, Content.Length);
            }
        }
    }
}