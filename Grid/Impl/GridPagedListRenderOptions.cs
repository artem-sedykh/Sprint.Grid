namespace Sprint.Grid.Impl
{
    public class PagedListRenderOptions
    {
        public string TotalFormat { get; set; }

        public string TotalSingleFormat { get; set; }

        public string PageFormat { get; set; }
    }

    public static class GridPagedListRenderOptions
    {
        public static PagedListRenderOptions Default
        {
            get
            {
                return new PagedListRenderOptions
                    {
                        PageFormat = "Page {0} of {1}",
                        TotalFormat = "Showing {0} - {1} of {2}",
                        TotalSingleFormat = "Showing {0} of {1}"
                    };
            }
        }
    }
}
