namespace Sprint.Grid.Impl
{
    public interface IGroupingItem
    {
        object Key { get; set; }

        int Count { get; set; }
    }
    
    public class GroupingItem:IGroupingItem
    {
        public object Key { get; set; }

        public int Count { get; set; }
    }
}
