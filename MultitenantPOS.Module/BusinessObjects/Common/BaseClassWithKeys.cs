using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;


namespace MultitenantPOS.Module.BusinessObjects.Common
{
    [DefaultClassOptions]
    [FriendlyKeyProperty("Id")]
    [NavigationItem(false)]
    public class BaseClassWithKeys : XPLiteObject
    { 
        public BaseClassWithKeys(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        protected override void OnSaving()
        {
            if (Session.IsNewObject(this))
            {
                CreatedOn = DateTime.Now;
                CreatedBy = SecuritySystem.CurrentUser as ApplicationUser;
            }
            else
            {
                AlteredOn = DateTime.Now;
                AlteredBy = SecuritySystem.CurrentUser as ApplicationUser;
            }
        }

        DateTime dola;
        DateTime alteredOn;
        ApplicationUser alteredBy;
        DateTime createdOn;
        ApplicationUser createdBy;
        int id;

        public int Id
        {
            get => id;
            set => SetPropertyValue(nameof(Id), ref id, value);
        }


        public ApplicationUser CreatedBy
        {
            get => createdBy;
            set => SetPropertyValue(nameof(CreatedBy), ref createdBy, value);
        }


        public DateTime CreatedOn
        {
            get => createdOn;
            set => SetPropertyValue(nameof(CreatedOn), ref createdOn, value);
        }


        public ApplicationUser AlteredBy
        {
            get => alteredBy;
            set => SetPropertyValue(nameof(AlteredBy), ref alteredBy, value);
        }


        public DateTime AlteredOn
        {
            get => alteredOn;
            set => SetPropertyValue(nameof(AlteredOn), ref alteredOn, value);
        }

        
        public DateTime Dola
        {
            get => dola;
            set => SetPropertyValue(nameof(Dola), ref dola, value);
        }
    }
}