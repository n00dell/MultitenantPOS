using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
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
                var currentUser = SecuritySystem.CurrentUser as ApplicationUser;
                if (currentUser != null && currentUser.Session != Session)
                {
                    currentUser = Session.GetObjectByKey<ApplicationUser>(currentUser.Oid);
                }
                CreatedBy = currentUser;
            }
            else
            {
                AlteredOn = DateTime.Now;
                var currentUser = SecuritySystem.CurrentUser as ApplicationUser;
                if (currentUser != null && currentUser.Session != Session)
                {
                    currentUser = Session.GetObjectByKey<ApplicationUser>(currentUser.Oid);
                }
                AlteredBy = currentUser;
            }
        }

        DateTime dola;
        DateTime alteredOn;
        ApplicationUser alteredBy;
        DateTime createdOn;
        ApplicationUser createdBy;
        int id;

        [Key(AutoGenerate = true)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public int Id
        {
            get => id;
            set => SetPropertyValue(nameof(Id), ref id, value);
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public ApplicationUser CreatedBy
        {
            get => createdBy;
            set => SetPropertyValue(nameof(CreatedBy), ref createdBy, value);
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime CreatedOn
        {
            get => createdOn;
            set => SetPropertyValue(nameof(CreatedOn), ref createdOn, value);
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public ApplicationUser AlteredBy
        {
            get => alteredBy;
            set => SetPropertyValue(nameof(AlteredBy), ref alteredBy, value);
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime AlteredOn
        {
            get => alteredOn;
            set => SetPropertyValue(nameof(AlteredOn), ref alteredOn, value);
        }

        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public DateTime Dola
        {
            get => dola;
            set => SetPropertyValue(nameof(Dola), ref dola, value);
        }
    }
}