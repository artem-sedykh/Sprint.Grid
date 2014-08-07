namespace Sprint.Grid.Impl
{    
    using System.Linq;    
    using System;
    using System.Web.Mvc;
    using System.Collections.Generic;

    public class GridModel<TModel> : IGridModel<TModel> where TModel : class
    {
        private readonly IColumnBuilder<TModel> _columns;
        private IGridOptions _options;        
        private readonly string _gridKey;

        public GridModel(string gridKey)
        {
            _gridKey = gridKey;

            _columns = new ColumnBuilder<TModel>();

            PageSize = 25;

            Page = 1;

            PageSizeInGroup = 10;                        

            PagedListRenderOptions = GridPagedListRenderOptions.Default;

            GridRender = new HtmlTableGridRenderer<TModel>(this);

            GridRenderOptions = GridRenderOptions.Default;

            SeparatorLevelLength = 5;
        }

        public void Init(IGridOptions options)
        {
            _options = options ?? new GridOptions();

            this.MergeGridOptions(_options);
        }        

        public void Render(HtmlHelper htmlHelper, IQueryable<TModel> query)
        {
            GridRender.Render(htmlHelper, query);
        }
        

        public IColumnBuilder<TModel> Columns
        {
            get { return _columns; }
        }

        public IGridOptions Options
        {
            get { return _options; }
        }                

        public object TableAttributes { get; set; }

        public IGridRender<TModel> GridRender { get; set; }

        public Func<IPagedList, PagedListRenderOptions, HtmlHelper, string,string> PagerRender { get; set; }

        public Func<TModel, UrlHelper, string> HierarchyUrl { get; set; }

        public GridRenderOptions GridRenderOptions { get; set; }

        public string GridKey
        {
            get { return _gridKey; }
        }

        public bool Multisort { get; set; }

        public int PageSize { get; set; }

        public int PageSizeInGroup { get; set; }

        public int GroupPageSize { get; set; }

        public int Page { get; set; }        

        public string SearchPattern { get; set; }

        public string Prefix { get; set; }

        public PagedListRenderOptions PagedListRenderOptions { get; set; }        

        public Func<TModel,HtmlHelper, object> RowAttributes { get; set; }

        public Func<TModel, IEnumerable<TModel>> ChildSelector { get; set; }

        public int SeparatorLevelLength { get; set; }
    }
}
