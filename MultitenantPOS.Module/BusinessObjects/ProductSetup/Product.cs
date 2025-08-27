using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;
using MultitenantPOS.Module.BusinessObjects.Config;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects.ProductSetup
{
    [DefaultClassOptions]
    
    public class Product : BaseClassWithKeys
    {
        public Product(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            isActive = true;
        }
        protected override void OnSaving()
        {
            if (!IsLoading && string.IsNullOrEmpty(SKU))
            {
                GenerateSKU();
            }
            base.OnSaving();
        }
        private void GenerateSKU()
        {
            var sb = new StringBuilder();

            // Append category shortcode
            if (Category != null && !string.IsNullOrEmpty(Category.ShortCode))
            {
                sb.Append(Category.ShortCode.ToUpper());
                sb.Append("-");
            }

            // Append shortened name
            sb.Append(Shorten(Name));

            // Append attribute shortcodes (sorted by name for consistency)
            foreach (var attr in ProductAttributes.OrderBy(a => a.AttributeName))
            {
                string attrShort = !string.IsNullOrEmpty(attr.ShortCode) ? attr.ShortCode : Shorten(attr.Value);
                sb.Append("-").Append(attrShort.ToUpper());
            }

            string proposedSKU = sb.ToString();

            // Ensure uniqueness
            string finalSKU = proposedSKU;
            int suffix = 1;
            while (Session.FindObject<Product>(CriteriaOperator.Parse("SKU == ?", finalSKU)) is Product existing && existing != this)
            {
                finalSKU = $"{proposedSKU}-{suffix}";
                suffix++;
            }

            SKU = finalSKU;
        }

        private string Shorten(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            // Simple shortening: remove spaces/vowels, take up to 5 chars (customize as needed)
            string cleaned = new string(input.Where(c => !char.IsWhiteSpace(c) && "AEIOUaeiou".IndexOf(c) < 0).ToArray()).ToUpper();
            return cleaned.Substring(0, Math.Min(5, cleaned.Length));
        }

        decimal vAT;
        Currency currency;
        ProductAttribute productAttribute;
        TaxSetup tax;
        Category category;
        UnitofMeasure unit;
        bool isActive;
        decimal price;
        string sKU;
        string name;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        [ModelDefault("AllowEdit", "False")]
        public string SKU
        {
            get => sKU;
            set => SetPropertyValue(nameof(SKU), ref sKU, value);
        }


        public Currency Currency
        {
            get => currency;
            set => SetPropertyValue(nameof(Currency), ref currency, value);
        }


        public decimal Price
        {
            get => price;
            set => SetPropertyValue(nameof(Price), ref price, value);
        }


        public UnitofMeasure Unit
        {
            get => unit;
            set => SetPropertyValue(nameof(Unit), ref unit, value);
        }


        public Category Category
        {
            get => category;
            set => SetPropertyValue(nameof(Category), ref category, value);
        }

        public bool IsActive
        {
            get => isActive;
            set => SetPropertyValue(nameof(IsActive), ref isActive, value);
        }


        public TaxSetup Tax
        {
            get => tax;
            set => SetPropertyValue(nameof(Tax), ref tax, value);
        }



        [Association("Product-ProductAttributes")]
        public XPCollection<ProductAttribute> ProductAttributes
        {
            get
            {
                return GetCollection<ProductAttribute>(nameof(ProductAttributes));
            }
        }

        string description;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }

        decimal cost;
        public decimal Cost
        {
            get => cost;
            set => SetPropertyValue(nameof(Cost), ref cost, value);
        }

        
        public decimal VAT
        {
            get => vAT;
            set => SetPropertyValue(nameof(VAT), ref vAT, value);
        }

    }
}