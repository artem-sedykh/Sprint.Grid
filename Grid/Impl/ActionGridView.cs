namespace Sprint.Grid.Impl
{
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;

    public class ActionGridView<TModel> : IActionGridView<TModel> where TModel : class
    {
        class ViewDataContainer : IViewDataContainer
        {
            public ViewDataContainer(ViewDataDictionary viewData)
            {
                ViewData = viewData;
            }

            public ViewDataDictionary ViewData { get; set; }
        }

        private readonly IGridModel<TModel> _model;
        private readonly IQueryable<TModel> _query;

        public ActionGridView(IGridModel<TModel> model, IQueryable<TModel> query)
        {
            _model = model;
            _query = query;
        }

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            var html = new HtmlHelper(viewContext, new ViewDataContainer(viewContext.ViewData));
            _model.Render(html, _query);
        }

        public IActionGridView<TModel> Init(IGridOptions options)
        {
            _model.Init(options);

            return this;
        }
    }
}
