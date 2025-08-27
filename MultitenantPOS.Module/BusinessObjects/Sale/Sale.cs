using DevExpress.Data.Filtering;

using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;


namespace MultitenantPOS.Module.BusinessObjects.Sale
{
    [DefaultClassOptions]
    
    public class Sale : BaseClassWithKeys
    { 
        public Sale(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        decimal totalAmount;
        ApplicationUser cashier;
        public Branch branch;

        public Branch Branch
        {
            get => branch;
            set => SetPropertyValue(nameof(Branch), ref branch, value);
        }

        public ApplicationUser Cashier
        {
            get => cashier;
            set => SetPropertyValue(nameof(Cashier), ref cashier, value);
        }

        
        public decimal TotalAmount
        {
            get => totalAmount;
            set => SetPropertyValue(nameof(TotalAmount), ref totalAmount, value);
        }

        [Association("Sale-Items")]
        public XPCollection<SaleItem> Items
        {
            get
            {
                return GetCollection<SaleItem>(nameof(Items));
            }
        }

    }
}