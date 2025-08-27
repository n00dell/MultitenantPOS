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

namespace MultitenantPOS.Module.BusinessObjects.Stock
{
    [DefaultClassOptions]
    
    public class StockAlert : BaseClassWithKeys
    { 
        public StockAlert(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }



        string reason;
        DateTime propertyName;
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


        
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Reason
        {
            get => reason;
            set => SetPropertyValue(nameof(Reason), ref reason, value);
        }

    }
}