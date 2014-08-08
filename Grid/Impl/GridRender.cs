namespace Sprint.Grid.Impl
{    
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web;
    using System;    

    public abstract class GridRender<TModel>:IGridRender<TModel> where TModel:class
    {
        protected GridRender(IGridModel<TModel> gridModel)
        {
            GridModel = gridModel;
            _gridKey = GridModel.GridKey;

            _groupHeaderClass = String.Format("js-{0}-{1}", GridKey, "group-column");
            _sortClass = String.Format("js-{0}-{1}", GridKey, "sort-link");
            _sortGroupGridClass = String.Format("js-{0}-{1}", GridKey, "sort-group-grid");
            _groupingHeaderClass = String.Format("js-{0}-{1}", GridKey, "grouping-header");
            _groupIndicatorClass = String.Format("js-{0}-{1}", GridKey, "group-indicator");                        
            _expandGroupClass = String.Format("js-{0}-{1}", GridKey, "expand-group");            
            _gridStateClass = String.Format("js-{0}-{1}", GridKey, "options");
            _gridGroupStateClass = String.Format("js-{0}-{1}", GridKey, "group-options");
            _alternateRowClass = "alternate";
            _gridPageLinkClass = String.Format("js-{0}-{1}", GridKey, "page-link");
            _gridGroupPageLinkClass = String.Format("js-{0}-{1}", GridKey, "group-page-link");            
            _gridHierarchyLinkClass=String.Format("js-{0}-{1}", GridKey, "hierarchy");
        }

        private GridRenderOptions _gridRenderOptions;
        private HtmlHelper _htmlHelper;
        private UrlHelper _urlHelper;
        private TextWriter _writer;
        private IEnumerable<KeyValuePair<string, IGridColumn<TModel>>> _orderedVisibleColumns;
        private readonly string _groupHeaderClass;
        private readonly string _sortClass;
        private readonly string _groupingHeaderClass;
        private readonly string _groupIndicatorClass;        
        private readonly string _gridKey;
        private bool _isGroupGrid;        
        private readonly string _gridStateClass;
        private readonly string _alternateRowClass;
        private readonly string _gridPageLinkClass;
        private readonly string _expandGroupClass;        
        private readonly string _gridGroupPageLinkClass;        
        private readonly string _gridGroupStateClass;
        private readonly string _sortGroupGridClass;
        private readonly string _gridHierarchyLinkClass;

        protected HtmlHelper HtmlHelper {
            get { return _htmlHelper; }
        }

        protected UrlHelper UrlHelper
        {
            get { return _urlHelper; }
        }

        protected TextWriter Writer
        {
            get { return _writer; }
        }

        protected IEnumerable<KeyValuePair<string, IGridColumn<TModel>>> OrderedVisibleColumns {
            get { return _orderedVisibleColumns; }
        }

        protected bool IsGroupGrid {
            get { return _isGroupGrid; }
        }

        protected string GridKey 
        {
            get { return _gridKey; }
        }

        protected string GroupHeaderClass
        {
            get { return _groupHeaderClass; }
        }

        protected string SortClass {
            get { return _sortClass; }
        }

        protected string GroupingHeaderClass
        {
            get { return _groupingHeaderClass; }
        }

        protected string GroupIndicatorClass
        {
            get { return _groupIndicatorClass; }
        }                

        protected string GridStateClass
        {
            get { return _gridStateClass; }
        }

        public IGridModel<TModel> GridModel { get; set; }

        protected string AlternateRowClass
        {
            get { return _alternateRowClass; }
        }

        protected string GridPageLinkClass
        {
            get { return _gridPageLinkClass; }
        }

        protected string ExpandGroupClass
        {
            get { return _expandGroupClass; }
        }        

        protected string GridGroupPageLinkClass
        {
            get { return _gridGroupPageLinkClass; }
        }        

        protected GridRenderOptions GridRenderOptions
        {
            get { return _gridRenderOptions; }
        }

        protected string GridGroupStateClass
        {
            get { return _gridGroupStateClass; }
        }

        protected string SortGroupGridClass
        {
            get { return _sortGroupGridClass; }
        }

        protected string GridHierarchyLinkClass
        {
            get { return _gridHierarchyLinkClass; }
        }

        public virtual void Render(HtmlHelper htmlHelper, IQueryable<TModel> query)
        {
            _orderedVisibleColumns = GridModel.Columns.Where(c => c.Value.IsVisible).OrderBy(c => c.Value.Order).ToList();

            _isGroupGrid = _orderedVisibleColumns.Any(x => x.Value.GroupOrder.HasValue && x.Value.GroupKeyProperty != null);

            _htmlHelper = htmlHelper;

            _urlHelper = new UrlHelper(_htmlHelper.ViewContext.RequestContext);

            _writer = htmlHelper.ViewContext.Writer;

            _gridRenderOptions = GridModel.GridRenderOptions ?? new GridRenderOptions();

            var groupOptions = ModelHelper.GetModel<GridGroupOptions>(GridModel.Prefix, null, null,HtmlHelper.ViewContext.Controller.ValueProvider,HtmlHelper.ViewContext.Controller.ControllerContext);            

            if (groupOptions != null && groupOptions.GroupKey != null && groupOptions.GroupKey.Any())
            {
                GridModel.MergeGridGroupOptions(groupOptions);                

                var sortColumns = OrderedVisibleColumns.Where(x => x.Value.SortDirection.HasValue).Select(x => x.Value).OrderBy(x => x.SortOrder).ToArray();
                var groupColumns = GridModel.Columns.Where(x => groupOptions.GroupKey.ContainsKey(x.Key));

                query = groupColumns.Aggregate(query, (current, column) => current.Where(column.Value.GroupKeyProperty, groupOptions.GroupKey[column.Key])).Sort(sortColumns.ToDictionary(x => x.SortProperty, x => x.SortDirection));                

                var queryCount = query.Count();
                var page = groupOptions.GPage == 0 ? 1 : groupOptions.GPage;

                if (page > (int)Math.Ceiling(((double)queryCount) / GridModel.PageSizeInGroup))
                    page = 1;
                var numberToSkip = (page - 1) * GridModel.PageSizeInGroup;
                var items = query.Skip(numberToSkip).Take(GridModel.PageSizeInGroup).ToArray();
              
                var paginationModel = new PagedListModel(page, GridModel.PageSizeInGroup, queryCount);

                RenderGroupTable(items, groupOptions, paginationModel);
            }
            else
            {
                RenderGidContainerStart();

                RenderGroupingHeader();

                RenderTableStart();

                RenderTableHeader();

                var paginateModel = RenderTableBody(query);

                RenderTableEnd();

                RenderGridState();

                RenderPager(paginateModel, GridPageLinkClass,false);                

                RenderGidContainerEnd();   
            }            
        }        

        #region Container

        public virtual void RenderGidContainerStart()
        {
            var url = GridUrl();
            var sanitizedId = TagBuilder.CreateSanitizedId(GridKey, HtmlHelper.IdAttributeDotReplacement);

            Writer.Write("<div id=\"{0}\" name=\"{1}\" class=\"sprint-grid\" data-action=\"{2}\" data-multisort=\"{3}\" data-prefix=\"{4}\">", sanitizedId, GridKey, url, GridModel.Multisort,GridModel.Prefix);
        }

        public virtual void RenderGidContainerEnd()
        {
            Writer.Write("</div>");
        }

        #endregion

        #region Grouping

        public virtual void RenderGroupingHeader()
        {
            if (OrderedVisibleColumns.All(x => x.Value.GroupKeyProperty == null))
                return;

            Writer.Write("<div class=\"grouping-header {0}\">", GroupingHeaderClass);

            if (IsGroupGrid)
            {
                foreach (var column in OrderedVisibleColumns.Where(c => c.Value.GroupOrder.HasValue).OrderBy(x => x.Value.GroupOrder))
                {
                    RenderGroupIndicator(column.Value, column.Key);
                }
            }
            else
            {
                Writer.Write("<p class=\"grouping-header-empty\">");
                Writer.Write(GridRenderOptions.EmptyGroupHeaderText);
                Writer.Write("</p>");
            }

            Writer.Write("</div>");
        }

        public abstract void RenderGroupIndicator(IGridColumn column, string key);

        public abstract void RenderGroupTable(TModel[] items, IGridGroupOptions groupOptions, IPagedList paginationViewModel);

        #endregion

        #region Table

        public virtual void RenderTableStart()
        {
            var table = new TagBuilder("table");
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(GridModel.TableAttributes);
            table.MergeAttributes(attributes);
            Writer.Write(table.ToString(TagRenderMode.StartTag));
        }

        public virtual void RenderTableEnd()
        {
            Writer.Write("</table>");
        }

        public virtual void RenderTableHeader()
        {
            Writer.Write("<thead>");
            Writer.Write("<tr>");
            var nextSortOrder = GridModel.Multisort ? (OrderedVisibleColumns.Any(x => x.Value.SortOrder.HasValue)
                                           ? (OrderedVisibleColumns.Where(x => x.Value.SortOrder.HasValue).Max(x => x.Value.SortOrder) + 1) : 0)
                                           : 0;

            if (IsGroupGrid ||GridModel.HierarchyUrl!=null)            
                Writer.Write("<th class=\"hierarchy-cell\"></th>");                         

            foreach (var column in OrderedVisibleColumns)            
                RenderColumnHeader(column.Value, column.Key, nextSortOrder.GetValueOrDefault(0),false);

            if (IsGroupGrid || GridModel.HierarchyUrl != null)
                Writer.Write("<th class=\"hierarchy-cell-right\"></th>");

            Writer.Write("</tr>");
            Writer.Write("</thead>");
        }

        public virtual IPagedList RenderTableBody(IQueryable<TModel> query)
        {
            Writer.Write("<tbody>");
            IPagedList paginateModel;
            var sortColumns = OrderedVisibleColumns.Where(x => x.Value.SortDirection.HasValue).Select(x => x.Value).OrderBy(x => x.SortOrder).ToArray();

            if (IsGroupGrid)
            {
                var groupColumns = OrderedVisibleColumns.Where(x => x.Value.GroupKeyProperty != null && x.Value.GroupOrder.HasValue).OrderBy(x => x.Value.GroupOrder).ToArray();

                #region Инициализируем опции для группы

                var groupOptions = new GridGroupOptions
                {
                    GPage = 1,                    
                    GroupKey = new Dictionary<string, object>(),
                    GColOpt = new Dictionary<string, IDictionary<string, object>>()
                };
                
                foreach (var column in GridModel.Options.ColOpt.Where(x => x.Value.Any(c => c.Key == "so" || c.Key == "sd")))
                {
                    groupOptions.GColOpt[column.Key] = new Dictionary<string, object>();
                    foreach (var colOptions in column.Value)
                        groupOptions.GColOpt[column.Key][colOptions.Key] = colOptions.Value;
                }

                #endregion                                                              

                var groupQuery = query.GroupBy(groupColumns.ToDictionary(x => x.Value.GroupKeyProperty, x => x.Value.GroupSortDirection));                

                var groupQueryCount = groupQuery.ToArray().Count();
                var page = GridModel.Page == 0 ? 1 : GridModel.Page;
                if (page > (int)Math.Ceiling(((double)groupQueryCount) / GridModel.PageSize))
                    page = 1;
                var numberToSkip = (page - 1) * GridModel.PageSize;
                var items = groupQuery.Skip(numberToSkip).Take(GridModel.PageSize).ToArray();

                paginateModel = new PagedListModel(page, GridModel.PageSize, groupQueryCount);

                var alternate = false;
                var columnCount = OrderedVisibleColumns.Count();

                for (var i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    RenderGroupTableRow(groupColumns, item, alternate, columnCount, groupOptions);
                    alternate = !alternate;
                }

                if(GridModel.ShowEmptyRows)
                {
                    if (GridModel.ShowEmptyRows)
                    {
                        for (var i = items.Length; i < GridModel.PageSize; i++)
                        {
                            RenderGroupTableRow(groupColumns, null, alternate, columnCount, groupOptions,empty:true);
                            alternate = !alternate;
                        }
                    }
                }
                if (items.Length == 0)
                {
                    Writer.Write("<tr>");
                    Writer.Write("<td class=\"empty-grid-row\" colspan=\"{1}\">{0}</td>", GridRenderOptions.EmptyText, OrderedVisibleColumns.Count() + 2);
                    Writer.Write("</tr>");
                }    
            }
            else
            {                
                query = query.Sort(sortColumns.ToDictionary(x=>x.SortProperty,x=>x.SortDirection));
                var queryCount = query.Count();
                var page = GridModel.Page == 0 ? 1 : GridModel.Page;
                if (page > (int)Math.Ceiling(((double)queryCount) / GridModel.PageSize))
                    page = 1;

                var numberToSkip = (page - 1) * GridModel.PageSize;
                var items = query.Skip(numberToSkip).Take(GridModel.PageSize).ToArray();
                
                paginateModel = new PagedListModel(page, GridModel.PageSize, queryCount);
                var alternate = false;

                for (var i = 0; i < items.Length; i++)
                {
                    var item = items[i];                    
                    RenderTableRow(item, OrderedVisibleColumns, alternate);
                    alternate = !alternate;
                }

                if(GridModel.ShowEmptyRows)
                {
                    for (var i = items.Length; i < GridModel.PageSize; i++)
                    {
                        RenderTableRow(null, OrderedVisibleColumns, alternate, empty: true);
                        alternate = !alternate;
                    }
                }

                if (items.Length == 0)
                {
                    Writer.Write("<tr>");
                    Writer.Write("<td class=\"empty-grid-row\" colspan=\"{1}\">{0}</td>", GridRenderOptions.EmptyText, OrderedVisibleColumns.Count() + 2);
                    Writer.Write("</tr>");
                }
                else
                {
                    RenderSummaryRow(items, OrderedVisibleColumns.ToArray());
                }
            }

            Writer.Write("</tbody>");

            return paginateModel;
        }

        public abstract void RenderGroupTableRow(KeyValuePair<string, IGridColumn<TModel>>[] groupColumns, IGroupingItem groupItem, bool alternate, int columnCount, IGridGroupOptions groupOptions,bool empty=false);

        public abstract void RenderTableRow(TModel item, IEnumerable<KeyValuePair<string, IGridColumn<TModel>>> columns, bool alternate,int level=0, bool empty=false);

        public abstract void RenderSummaryRow(TModel[] items, KeyValuePair<string, IGridColumn<TModel>>[] columns);

        public abstract void RenderColumnHeader(IGridColumn<TModel> column, string key, int nextSortOrder, bool isGroupTableHeader);

        public abstract void RenderPager(IPagedList pagedList, string actionClass, bool isGroupPager);

        #endregion

        public abstract void RenderGridState();       

        public virtual string GridUrl()
        {            
            var routeValues = new RouteValueDictionary(HtmlHelper.ViewContext.RouteData.Values);
            var queryString = UrlHelper.RequestContext.HttpContext.Request.QueryString;
            foreach (var key in queryString.AllKeys)
            {
                routeValues[key] = queryString[key];
            }
            var url = UrlHelper.RouteUrl(routeValues);
            return url;
        }

        public object GetCellValue(IGridColumn<TModel> column,TModel item)
        {
            if (column.CellCondition != null && !column.CellCondition(item))
            {
                return null;
            }

            var value = column.ColumnValueRender(item, HtmlHelper);            

            if (column.Encode && value != null && !(value is IHtmlString))
            {
                value = HttpUtility.HtmlEncode(value.ToString());
            }

            return value;
        }        
    }
}
