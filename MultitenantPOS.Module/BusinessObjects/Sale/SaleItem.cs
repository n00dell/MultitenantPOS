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

namespace MultitenantPOS.Module.BusinessObjects.Sale
{
    [DefaultClassOptions]
    
    public class SaleItem : BaseClassWithKeys
    { 
        public SaleItem(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }



        decimal price;
        decimal quantity;
        Sale sale;
        Product product;

        public Product Product
        {
            get => product;
            set => SetPropertyValue(nameof(Product), ref product, value);
        }


        [Association("Sale-Items")]
        public Sale Sale
        {
            get => sale;
            set => SetPropertyValue(nameof(Sale), ref sale, value);
        }


        public decimal Quantity
        {
            get => quantity;
            set => SetPropertyValue(nameof(Quantity), ref quantity, value);
        }
        
        public decimal Price
        {
            get => price;
            set => SetPropertyValue(nameof(Price), ref price, value);
        }

    }
}