namespace Sprint.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public interface IColumnBuilder<TModel> : IEnumerable<KeyValuePair<string, IGridColumn<TModel>>>
    {
        IGridColumnConfiguration<TModel> For(Func<TModel, HtmlHelper, object> property, string key);

        IGridColumnConfiguration<TModel> For(Func<TModel, object> property, string key);
    }
}
