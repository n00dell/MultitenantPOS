using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.MultiTenancy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects
{
    [DefaultClassOptions]
   
    public class TenantExtended : Tenant
    { 
        public TenantExtended(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
        private FileData _logo;
        [FileTypeFilter("Image", 1, "*.jpg", "*.png", "*.gif", "*.svg")]
        public FileData Logo
        {
            get => _logo;
            set => SetPropertyValue(nameof(Logo), ref _logo, value);
        }

    }

}