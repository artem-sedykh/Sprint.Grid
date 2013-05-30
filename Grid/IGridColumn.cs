using System.Collections.Generic;

namespace Sprint.Grid
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Helpers;
    using System.Web.Mvc;

    public interface IGridColumn
    {
        string Title { get; }

        int Order { get; set; }

        SortDirection? SortDirection { get; set; }

        SortDirection? GroupSortDirection { get; set; }

        int? SortOrder { get; set; }

        LambdaExpression SortProperty { get; }

        bool IsVisible { get; set; }

        bool Encode { get; }

        IDictionary<string,object> HeaderAttributes { get; }

        IDictionary<string, object> Attributes { get; }

        Func<HtmlHelper, object> HeaderRender { get; }

        int? GroupOrder { get; set; }

        LambdaExpression GroupKeyProperty { get; }

        Func<object, object> GroupTitleRender { get; }        
    }

    public interface IGridColumn<in TModel> : IGridColumn
    {
        Func<TModel, bool> CellCondition { get; }

        Func<TModel, HtmlHelper, object> ColumnValueRender { get; }

        Func<IEnumerable<TModel>, HtmlHelper, object> SummaryCellValueRender { get; }
    }
}
