namespace Sprint.Grid
{
    using System;
    using System.Linq.Expressions;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Collections.Generic;

    public interface IGridColumnConfiguration<TModel>
    {
        /// <summary>
        /// Назвние Колонки.
        /// </summary>        
        IGridColumnConfiguration<TModel> Title(string title);

        /// <summary>
        /// Видимость колонки. 
        /// </summary>                
        IGridColumnConfiguration<TModel> Visible(bool isVisible);

        IGridColumnConfiguration<TModel> SortColumn<TProperty>(Expression<Func<TModel, TProperty>> property);

        IGridColumnConfiguration<TModel> SortColumn<TProperty>(Expression<Func<TModel, TProperty>> property, SortDirection initialDirection);

        IGridColumnConfiguration<TModel> SortColumn<TProperty>(Expression<Func<TModel, TProperty>> property, SortDirection initialDirection, int sortOrder);

        IGridColumnConfiguration<TModel> Encode(bool encode);

        IGridColumnConfiguration<TModel> HeaderAttributes(object attributes);

        IGridColumnConfiguration<TModel> Attributes(object attributes);

        IGridColumnConfiguration<TModel> Header(Func<HtmlHelper, object> headerRender);
        
        IGridColumnConfiguration<TModel> CellCondition(Func<TModel, bool> condition);

        IGridColumnConfiguration<TModel> GroupColumn<TProperty>(Expression<Func<TModel, TProperty>> property, int? groupOrder = null, Func<TProperty, object> groupTitleRender = null);

        IGridColumnConfiguration<TModel> SummaryCell(Func<IEnumerable<TModel>, HtmlHelper, object> summaryCellValueRender);
    }
}
