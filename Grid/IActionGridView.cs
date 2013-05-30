namespace Sprint.Grid
{
    using System.Web.Mvc;

    public interface IActionGridView<TModel> : IView where TModel : class
    {
        IActionGridView<TModel> Init(IGridOptions options);
    }
}