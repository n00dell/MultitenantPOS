using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MultitenantPOS.Module.BusinessObjects.Common;
using MultitenantPOS.Module.BusinessObjects.Sale;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MultitenantPOS.Module.BusinessObjects.Shift
{
    [DefaultClassOptions]
    
    public class Shift : BaseClassWithKeys
    {
        public Shift(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        DateTime totalSales;
        DateTime clockOut;
        DateTime clockIn;
        Branch branch;
        ApplicationUser cashier;

        public ApplicationUser Cashier
        {
            get => cashier;
            set => SetPropertyValue(nameof(Cashier), ref cashier, value);
        }


        public Branch Branch
        {
            get => branch;
            set => SetPropertyValue(nameof(Branch), ref branch, value);
        }


        public DateTime ClockIn
        {
            get => clockIn;
            set => SetPropertyValue(nameof(ClockIn), ref clockIn, value);
        }


        public DateTime ClockOut
        {
            get => clockOut;
            set => SetPropertyValue(nameof(ClockOut), ref clockOut, value);
        }

        
        public DateTime TotalSales
        {
            get => totalSales;
            set => SetPropertyValue(nameof(TotalSales), ref totalSales, value);
        }

    }
}