using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;
using MultitenantPOS.Module.BusinessObjects.ProductSetup;



namespace MultitenantPOS.Module.BusinessObjects.Stock
{
    [DefaultClassOptions]
    
    public class StockEntry : BaseClassWithKeys
    { 
        public StockEntry(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        StockEntryType type;
        DateTime entryD;
        decimal quantity;
        Branch branch;
        Product product;

        public Product Product
        {
            get => product;
            set => SetPropertyValue(nameof(Product), ref product, value);
        }


        public Branch Branch
        {
            get => branch;
            set => SetPropertyValue(nameof(Branch), ref branch, value);
        }


        public decimal Quantity
        {
            get => quantity;
            set => SetPropertyValue(nameof(Quantity), ref quantity, value);
        }


        
        public StockEntryType Type
        {
            get => type;
            set => SetPropertyValue(nameof(Type), ref type, value);
        }

    }
}