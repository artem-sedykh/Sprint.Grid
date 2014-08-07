namespace Sprint.Grid.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web.Helpers;
    using System.Web.Mvc;

    public class GridColumn<TModel> : IGridColumn<TModel>, IGridColumnConfiguration<TModel>
    {
        private string _title;
        private bool _isVisible;
        private LambdaExpression _sortProperty;
        private SortDirection? _sortDirection;
        private int? _sortOrder;
        private bool _encode=true;
        private IDictionary<string, object> _headerAttributes;
        private IDictionary<string, object> _attributes;
        private Func<HtmlHelper, object> _headerRender;
        private Func<TModel, bool> _cellCondition;
        private LambdaExpression _groupKeyProperty;
        private int? _groupOrder;        
        private readonly Func<TModel, HtmlHelper, object> _valueRender;
        private Func<object, object> _groupTitleRender;
        private Func<IEnumerable<TModel>, HtmlHelper, object> _summaryCellValueRender;

        public GridColumn(Func<TModel, HtmlHelper, object> valueRender, int order)
        {
            _valueRender = valueRender;
            Order = order;
            _isVisible = true;            
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.Title(string title)
        {
            _title = title;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.Visible(bool isVisible)
        {
            _isVisible = isVisible;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.SortColumn<TProperty>(Expression<Func<TModel, TProperty>> property)
        {
            _sortProperty = property;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.SortColumn<TProperty>(Expression<Func<TModel, TProperty>> property, SortDirection initialDirection)
        {
            _sortProperty = property;

            _sortDirection = initialDirection;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.SortColumn<TProperty>(Expression<Func<TModel, TProperty>> property, SortDirection initialDirection, int sortOrder)
        {
            _sortProperty = property;

            _sortDirection = initialDirection;

            _sortOrder = sortOrder;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.Encode(bool encode)
        {
            _encode = encode;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.HeaderAttributes(object attributes)
        {
            _headerAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.Attributes(object attributes)
        {
            _attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.Header(Func<HtmlHelper, object> headerRender)
        {
            _headerRender = headerRender;

            return this;
        }

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.CellCondition(Func<TModel, bool> condition)
        {
            _cellCondition = condition;

            return this;
        }        

        IGridColumnConfiguration<TModel> IGridColumnConfiguration<TModel>.GroupColumn<TProperty>(Expression<Func<TModel, TProperty>> property, int? groupOrder, Func<TProperty, object> groupTitleRender)
        {
            _groupKeyProperty = property;
            GroupSortDirection = System.Web.Helpers.SortDirection.Ascending;
            _groupOrder = groupOrder;

            if (groupTitleRender != null)
                _groupTitleRender = x => groupTitleRender.DynamicInvoke(x);            

            return this;
        }

        public IGridColumnConfiguration<TModel> SummaryCell(Func<IEnumerable<TModel>, HtmlHelper, object> summaryCellValueRender)
        {
            _summaryCellValueRender = summaryCellValueRender;

            return this;
        }

        public string Title
        {
            get { return _title; }
        }

        public int Order { get; set; }

        public SortDirection? SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        public SortDirection? GroupSortDirection { get; set; }

        public int? SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public LambdaExpression SortProperty
        {
            get { return _sortProperty; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool Encode
        {
            get { return _encode; }
        }

        public IDictionary<string, object> HeaderAttributes
        {
            get { return _headerAttributes; }
        }

        public IDictionary<string, object> Attributes {
            get
            {
                return _attributes;
            }
        }        

        public Func<HtmlHelper, object> HeaderRender
        {
            get { return _headerRender; }
        }

        int? IGridColumn.GroupOrder
        {
            get { return _groupOrder; }
            set { _groupOrder = value; }
        }

        public LambdaExpression GroupKeyProperty
        {
            get { return _groupKeyProperty; }
        }

        public Func<object, object> GroupTitleRender
        {
            get { return _groupTitleRender; }
        }

        public Func<TModel, bool> CellCondition
        {
            get { return _cellCondition; }
        }

        public Func<TModel, HtmlHelper, object> ColumnValueRender
        {
            get { return _valueRender; }
        }

        public Func<IEnumerable<TModel>, HtmlHelper, object> SummaryCellValueRender
        {
            get { return _summaryCellValueRender; }
        }
    }
}
