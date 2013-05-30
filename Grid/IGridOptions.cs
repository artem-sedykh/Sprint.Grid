namespace Sprint.Grid
{
    using System.Collections.Generic;    
    
    public interface IGridOptions
    {
        Dictionary<string, Dictionary<string, object>> ColOpt { get; set; }

        Dictionary<string, string> PageOpt { get; set; }
    }
}