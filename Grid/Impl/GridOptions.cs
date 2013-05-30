namespace Sprint.Grid.Impl
{
    using System.Collections.Generic;

    public class GridOptions : IGridOptions
    {        
        public Dictionary<string, Dictionary<string,object>> ColOpt { get; set; }

        public Dictionary<string, string> PageOpt { get; set; }
    }                   
}
