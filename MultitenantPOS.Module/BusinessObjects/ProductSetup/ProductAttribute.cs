using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;


namespace MultitenantPOS.Module.BusinessObjects.ProductSetup
{
    [DefaultClassOptions]

    public class ProductAttribute : BaseClassWithKeys
    { 
        public ProductAttribute(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://docs.devexpress.com/eXpressAppFramework/112834/getting-started/in-depth-tutorial-winforms-webforms/business-model-design/initialize-a-property-after-creating-an-object-xpo?v=22.1).
        }



        Product product;
        string shortCode;
        string value;
        string attributeName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string AttributeName
        {
            get => attributeName;
            set => SetPropertyValue(nameof(AttributeName), ref attributeName, value);
        }


        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Value
        {
            get => value;
            set => SetPropertyValue(nameof(Value), ref value, value);
        }

        
        [Size(10)]
        public string ShortCode
        {
            get => shortCode;
            set => SetPropertyValue(nameof(ShortCode), ref shortCode, value);
        }

        
        [Association("Product-ProductAttributes")]
        public Product Product
        {
            get => product;
            set => SetPropertyValue(nameof(Product), ref product, value);
        }

    }
}