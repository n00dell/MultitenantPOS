using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects.Common
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
        }


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
        public string SKU
        {
            get => sKU;
            set => SetPropertyValue(nameof(SKU), ref sKU, value);
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

        
        [Association("Category-Products")]
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

    }
}