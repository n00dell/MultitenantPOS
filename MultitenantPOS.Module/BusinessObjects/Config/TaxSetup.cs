using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;
using MultitenantPOS.Module.BusinessObjects.ProductSetup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects.Config
{
    [DefaultClassOptions]
   
    public class TaxSetup : BaseClassWithKeys
    { 
        public TaxSetup(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://docs.devexpress.com/eXpressAppFramework/112834/getting-started/in-depth-tutorial-winforms-webforms/business-model-design/initialize-a-property-after-creating-an-object-xpo?v=22.1).
        }


        Product product;
        decimal vAT;

        
        public Product Product
        {
            get => product;
            set => SetPropertyValue(nameof(Product), ref product, value);
        }

        public decimal VAT
        {
            get => vAT;
            set => SetPropertyValue(nameof(VAT), ref vAT, value);
        }
    }
}