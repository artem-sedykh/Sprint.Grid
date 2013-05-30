namespace Sprint.Grid
{    
    using System.Web.Mvc;
    using System.Linq;

    public interface IGridRender<TModel> where TModel : class
    {
        IGridModel<TModel> GridModel { get; set; }

        void Render(HtmlHelper htmlHelper, IQueryable<TModel> query);
    }
}