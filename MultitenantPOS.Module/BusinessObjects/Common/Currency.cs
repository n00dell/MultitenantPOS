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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects.Common
{
    [DefaultClassOptions]

    public class Currency : BaseClassWithKeys
    { 
        public Currency(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
         }

        string name;
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        string code;
        [Size(10)]
        [ModelDefault("AllowEdit", "false")]
        public string Code
        {
            get => code;
            set => SetPropertyValue(nameof(Code), ref code, value?.ToUpper());
        }

        string symbol;
        [Size(10)]
        [ModelDefault("AllowEdit", "false")]
        public string Symbol
        {
            get => symbol;
            set => SetPropertyValue(nameof(Symbol), ref symbol, value);
        }

        [NotMapped]
        [ModelDefault("Caption", "Display Name")]
        public string DisplayName => $"{Code} - {Name} ({Symbol})";

    }
}