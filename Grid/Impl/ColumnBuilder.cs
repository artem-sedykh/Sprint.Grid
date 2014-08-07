namespace Sprint.Grid.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class ColumnBuilder<TModel> : IColumnBuilder<TModel>
    {
        private readonly IDictionary<string, IGridColumn<TModel>> _columns;
        private int _index;

        public ColumnBuilder()
        {
            _columns = new Dictionary<string, IGridColumn<TModel>>();
            _index = 0;
        }

        public IGridColumnConfiguration<TModel> For(Func<TModel, HtmlHelper, object> property, string key)
        {
            var column = new GridColumn<TModel>(property, _index++);

            _columns.Add(key, column);

            return column;
        }

        public IGridColumnConfiguration<TModel> For(Func<TModel, object> property, string key)
        {
            Func<TModel, HtmlHelper, object> prop = (m, html) => property != null ? property(m) : null;

            var column = new GridColumn<TModel>(prop, _index++);

            _columns.Add(key, column);

            return column;
        }

        public IEnumerator<KeyValuePair<string, IGridColumn<TModel>>> GetEnumerator()
        {
            return _columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
