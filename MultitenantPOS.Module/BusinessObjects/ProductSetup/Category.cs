using DevExpress.Charts.Native;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;


namespace MultitenantPOS.Module.BusinessObjects.ProductSetup
{
    [DefaultClassOptions]
    
    public class Category : BaseClassWithKeys
    {
        public Category(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        string name;
        string shortCode;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Size(10)]
        public string ShortCode
        {
            get => shortCode;
            set => SetPropertyValue(nameof(ShortCode), ref shortCode, value?.ToUpper());
        }

    }
}