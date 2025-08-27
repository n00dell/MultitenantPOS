using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects
{
    [DefaultClassOptions]

    public class Branch : BaseClassWithKeys
    { 
        public Branch(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        string address;
        string name;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Address
        {
            get => address;
            set => SetPropertyValue(nameof(Address), ref address, value);
        }
    }
}