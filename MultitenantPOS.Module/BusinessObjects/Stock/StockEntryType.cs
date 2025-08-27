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

namespace MultitenantPOS.Module.BusinessObjects.Stock
{
    [DefaultClassOptions]
    
    public class StockEntryType : BaseObject
    { 
        public StockEntryType(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
        // Purchase, Return, Adjustment, TransferIn, TransferOut


        bool isTransferOut;
        bool isTransferIn;
        bool isAdjustment;
        bool isReturn;
        bool isPurchase;

        public bool IsPurchase
        {
            get => isPurchase;
            set
            {
                SetPropertyValue(nameof(IsPurchase), ref isPurchase, value);
                if (!IsDeleted && IsLoading)
                {
                    if (IsPurchase)
                    {
                        IsReturn = false;
                        IsAdjustment = false;
                        IsTransferIn = false;
                        IsTransferOut = false;
                    }
                }
            }
        }


        public bool IsReturn
        {
            get => isReturn;
            set
            {
                SetPropertyValue(nameof(IsReturn), ref isReturn, value);
                if (!IsDeleted && !IsLoading)
                {
                    if (IsReturn)
                    {
                        IsPurchase = false;
                        IsAdjustment = false;
                        IsTransferIn = false;
                        IsTransferOut = false;
                    }
                    
                }
            }
        }


        public bool IsAdjustment
        {
            get => isAdjustment;
            set
            {
                SetPropertyValue(nameof(IsAdjustment), ref isAdjustment, value);
                if (!IsDeleted && !IsLoading)
                {
                    if (IsAdjustment)
                    {
                        IsPurchase = false;
                        IsReturn = false;
                        IsTransferIn = false;
                        IsTransferOut = false;
                    }
                }
            }
        }


        public bool IsTransferIn
        {
            get => isTransferIn;
            set
            {
                SetPropertyValue(nameof(IsTransferIn), ref isTransferIn, value);
                if (!IsDeleted && !IsLoading)
                {
                    if (IsTransferIn)
                    {
                        IsPurchase = false;
                        IsReturn = false;
                        IsAdjustment = false;
                        IsTransferOut = false;
                    }
                }
            }
        }

        
        public bool IsTransferOut
        {
            get => isTransferOut;
            set
            {
                SetPropertyValue(nameof(IsTransferOut), ref isTransferOut, value);
                if (!IsDeleted && !IsLoading)
                {
                    if (IsTransferOut)
                    {
                        IsPurchase = false;
                        IsReturn = false;
                        IsAdjustment = false;
                        IsTransferIn = false;
                    }
                }
            }
        }
    }
}