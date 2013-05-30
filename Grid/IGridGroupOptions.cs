namespace Sprint.Grid
{
    using System.Collections.Generic;    

    public interface IGridGroupOptions
    {
        IDictionary<string, object> GroupKey { get; set; }

        int GPage { get; set; }        

        IDictionary<string, IDictionary<string, object>> GColOpt { get; set; }

    }
}
