namespace Sprint.Grid
{
    using System;
    using System.Linq;        
    using Impl;
    using System.Web.Mvc;
    using System.Collections.Generic;

    public interface IGridModel
    {
        void Init(IGridOptions options);

        IGridOptions Options { get; }

        object TableAttributes { get; set; }
        GridRenderOptions GridRenderOptions { get; set; }

        string GridKey { get; }

        bool Multisort { get; set; }

        int PageSize { get; set; }

        bool ShowEmptyRows { get; set; }

        bool ShowEmptyRowsInGroup { get; set; }

        int PageSizeInGroup { get; set; }

        int Page { get; set; }

        string SearchPattern { get; set; }

        string Prefix { get; set; }

        PagedListRenderOptions PagedListRenderOptions { get; set; }

        int SeparatorLevelLength { get; set; }
    }

    public interface IGridModel<TModel>:IGridModel where TModel : class
    {

        void Render(HtmlHelper htmlHelper, IQueryable<TModel> query);

        IColumnBuilder<TModel> Columns { get; }        

        IGridRender<TModel> GridRender { get; set; }

        Func<IPagedList, PagedListRenderOptions, HtmlHelper, string,string> PagerRender { get; set; }

        Func<TModel,UrlHelper, string> HierarchyUrl { get; set; }

        Func<TModel,HtmlHelper, object> RowAttributes { get; set; }

        Func<TModel, IEnumerable<TModel>> ChildSelector { get; set; }        
    }
}