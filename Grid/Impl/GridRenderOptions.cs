namespace Sprint.Grid.Impl
{
    public class GridRenderOptions
    {
        public string EmptyText { get; set; }

        public string EmptyGroupHeaderText { get; set; }

        public string GroupTagTitle { get; set; }        

        public string SortIndexTitle { get; set; }

        public string EmptyGroupTitleText { get; set; }

        public string GroupCountTemplate { get; set; }

        public string GroupTitleTemplate { get; set; }

        public static GridRenderOptions Default
        {
            get
            {
                return new GridRenderOptions
                    {
                        EmptyGroupHeaderText = "Drag a column header and drop it here to group by that column",
                        EmptyText="Empty",
                        GroupTagTitle = "Grouping",                        
                        EmptyGroupTitleText="[Empty]",
                        GroupTitleTemplate = "<b>{0}</b>:{1};",
                        GroupCountTemplate = "<b class=\"grid-group-count\">Count: {0}</b>"
                    };
            }
        }
    }
}
