namespace Sprint.Grid
{
    using System;
    using System.Linq;        
    using Impl;
    using System.Web.Mvc;
    using System.Collections.Generic;

    public interface IGridModel<TModel> where TModel : class
    {
        void Init(IGridOptions options);

        void Render(HtmlHelper htmlHelper, IQueryable<TModel> query);

        /// <summary>
        /// Колонки
        /// </summary>
        IColumnBuilder<TModel> Columns { get; }

        /// <summary>
        /// Настройки 
        /// </summary>
        IGridOptions Options { get; }        

        object TableAttributes { get; set; }

        IGridRender<TModel> GridRender { get; set; }

        Func<IPagedList, PagedListRenderOptions, HtmlHelper, string,string> PagerRender { get; set; }

        Func<TModel,UrlHelper, string> HierarchyUrl { get; set; }

        GridRenderOptions GridRenderOptions { get; set; }

        string GridKey { get; }        

        bool Multisort { get; set; }

        int PageSize { get; set; }

        int PageSizeInGroup { get; set; }        

        int Page { get; set; }        

        string SearchPattern { get; set; }

        string Prefix { get; set; }

        PagedListRenderOptions PagedListRenderOptions { get; set; }

        Func<TModel,HtmlHelper, object> RowAttributes { get; set; }

        Func<TModel, IEnumerable<TModel>> ChildSelector { get; set; }

        int SeparatorLevelLength { get; set; }
    }
}