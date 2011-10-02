namespace RestfulRouting
{
    public class RouteNames
    {
        public RouteNames()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            IndexName = "Index";
            ShowName = "Show";
            NewName = "New";
            CreateName = "Create";
            EditName = "Edit";
            UpdateName = "Update";
            DeleteName = "Delete";
            DestroyName = "Destroy";
        }

        public string IndexName { get; set; }

        public string ShowName { get; set; }

        public string CreateName { get; set; }

        public string UpdateName { get; set; }

        public string DestroyName { get; set; }

        public string NewName { get; set; }

        public string EditName { get; set; }

        public string DeleteName { get; set; }
    }
}
