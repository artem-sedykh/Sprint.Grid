namespace Sprint.Grid.Impl
{
    using System.Collections.Generic;

    public class GridGroupOptions : IGridGroupOptions
    {
        public IDictionary<string, object> GroupKey { get; set; }
        public int GPage { get; set; }        
        public IDictionary<string, IDictionary<string, object>> GColOpt { get; set; }
    }
}
