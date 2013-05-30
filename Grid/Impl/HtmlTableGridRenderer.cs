using System.Text;

namespace Sprint.Grid.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;    
    using System.Web.Helpers;
    using System.Web.Mvc;        

    public class HtmlTableGridRenderer<TModel>:GridRender<TModel> where TModel:class
    {        
        public HtmlTableGridRenderer(IGridModel<TModel> gridModel) : base(gridModel){}

        public override void RenderGroupIndicator(IGridColumn column, string key)
        {
            var direction = column.GroupSortDirection == SortDirection.Ascending
                                ? SortDirection.Descending
                                : SortDirection.Ascending;

            var sortClass = column.GroupSortDirection == SortDirection.Ascending ? "asc-group-sort" : "desc-group-sort";

            Writer.Write("<div data-key=\"{0}\" data-direction=\"{1}\" class=\"group-indicator {2}\">", key,(int)direction, GroupIndicatorClass);

            Writer.Write("<a class=\"group-sort\"><span class=\"grid-icon {0}\"></span>{1}</a>", sortClass, column.Title);
            
            Writer.Write("<a class=\"group-drop-button\"><span class=\"grid-icon\"></span></a></div>");   
        }

        public override void RenderSummaryRow(TModel[] items, KeyValuePair<string, IGridColumn<TModel>>[] columns)
        {            
            if (!columns.Any(x => x.Value.SummaryCellValueRender != null)) return;

            var isHierarchy = GridModel.HierarchyUrl != null;

            Writer.Write("<tr class=\"grid-summary-row\">");
            if (isHierarchy)
                Writer.Write("<td class=\"hierarchy-cell\"></td>");

            foreach (var column in columns)
            {
                Writer.Write("<td>");
                if (column.Value.SummaryCellValueRender != null)                    
                    Writer.Write(column.Value.SummaryCellValueRender(items, HtmlHelper));                    
                Writer.Write("</td>");
            }

            if (isHierarchy)
                Writer.Write("<td class=\"hierarchy-cell-right\"></td>");

            Writer.Write("</tr>");
        }

        public override void RenderColumnHeader(IGridColumn<TModel> column, string key, int nextSortOrder,bool isGroupTableHeader)
        {
            var th = new TagBuilder("th");
            th.MergeAttributes(column.HeaderAttributes);
            if (column.SortDirection.HasValue)
                th.Attributes["class"] = th.Attributes.ContainsKey("class")? "sorted " + th.Attributes["class"]: "sorted";

            Writer.Write(th.ToString(TagRenderMode.StartTag));

            if (column.HeaderRender != null)
            {
                var value = column.HeaderRender(HtmlHelper);
                Writer.Write(value);
            }
            else
            {
                var isGroupableColumn = (!isGroupTableHeader && column.GroupKeyProperty != null && !column.GroupOrder.HasValue);

                var groupClass = isGroupableColumn ? GroupHeaderClass : string.Empty;

                var groupTag = string.Empty;

                if (isGroupableColumn)
                {              
                    var groupTagBuilder = new TagBuilder("sup");
                    groupTagBuilder.AddCssClass("grid-group-tag");
                    groupTagBuilder.SetInnerText("G");

                    if (!String.IsNullOrEmpty(GridRenderOptions.GroupTagTitle))
                        groupTagBuilder.Attributes["title"] = GridRenderOptions.GroupTagTitle;

                    groupTag = groupTagBuilder.ToString(TagRenderMode.Normal);
                }
                
                Writer.Write("<div class=\"grid-column-wrap {2}\" data-key=\"{0}\" data-title=\"{1}\">", key, column.Title, groupClass);
                Writer.Write(groupTag);

                var isSortableColumn = column.SortProperty != null;
                if (isSortableColumn)
                    RenderSortableColumnHeader(column, key, nextSortOrder, isGroupTableHeader);
                else
                    Writer.Write("<span class=\"grid-column-title\">{0}</span>", column.Title);

                Writer.Write("</div>");
            }

            Writer.Write("</th>");
        }

        public override void RenderPager(IPagedList pagedList, string actionClass, bool isGroupPager)
        {
            var statusLinkClass = isGroupPager ? String.Format("{0}-{1}", GridModel.GridKey, "group-status") : String.Format("{0}-{1}", GridModel.GridKey, "status");
            var options = GridModel.PagedListRenderOptions ?? GridPagedListRenderOptions.Default;
            var cls = isGroupPager ? GridGroupPageLinkClass : GridPageLinkClass;
            Writer.Write("<div class=\"grid-pagination-wrap\">");

            if (GridModel.PagerRender != null)
            {
                Writer.Write(GridModel.PagerRender(pagedList, options, HtmlHelper, actionClass));
            }
            else
            {
                
                Writer.Write("<div class=\"grid-status\">");
                Writer.Write("<a class=\"grid-status-icon {0} {1} refresh\">Refresh</a>", statusLinkClass, cls);
                Writer.Write("</div>");

                var pager = new DefaultPager(pagedList, options, HtmlHelper, actionClass);
                pager.Render();                
            }

            Writer.Write("</div>");
        }

        private void RenderSortableColumnHeader(IGridColumn column, string key, int nextSortOrder, bool isGroupTableHeader)
        {
            var multisort = GridModel.Multisort;            
            var direction = column.SortDirection.HasValue ? (column.SortDirection == SortDirection.Ascending ? (SortDirection?)SortDirection.Descending : null) : SortDirection.Ascending;
            var sortOrder = direction.HasValue && multisort ? (column.SortDirection.HasValue ? column.SortOrder : nextSortOrder) : null;
            var currentSortClass = isGroupTableHeader ? SortGroupGridClass : SortClass;

            if (column.SortDirection.HasValue && multisort)
            {
                var sortOrderTagBuilder = new TagBuilder("sup");
                sortOrderTagBuilder.AddCssClass("sort-order");
                var index = column.SortOrder + 1;
                sortOrderTagBuilder.SetInnerText(index.HasValue ? index.Value.ToString(CultureInfo.InvariantCulture) : null);
                if (!String.IsNullOrEmpty(GridRenderOptions.SortIndexTitle))
                    sortOrderTagBuilder.Attributes["title"] = GridRenderOptions.SortIndexTitle;
                
                Writer.Write(sortOrderTagBuilder.ToString(TagRenderMode.Normal));
            }

            Writer.Write("<a class=\"grid-sort-link {4}\" data-key=\"{0}\" data-direction=\"{1}\" data-sortorder=\"{2}\">{3}", key, (int?)direction, sortOrder, column.Title, currentSortClass);            
            if(column.SortDirection!=null)
                Writer.Write("<span class=\"{0}\"></span>",column.SortDirection==SortDirection.Ascending?"asc-sort":"desc-sort");

            Writer.Write("</a>");
        }        

        public override void RenderGroupTableRow(KeyValuePair<string, IGridColumn<TModel>>[] groupColumns, IGroupingItem groupItem, bool alternate, int columnCount, IGridGroupOptions groupOptions)
        {                        
            var groupKeys = HtmlHelper.AnonymousObjectToHtmlAttributes(groupItem.Key);

            Writer.Write(alternate ? "<tr class=\"alternate\">" : "<tr>");
            Writer.Write("<td class=\"hierarchy-cell\"><a class=\"grid-icon {0} plus lazy\"></a>", ExpandGroupClass);
            Writer.Write("</td>");
            Writer.Write("<td class=\"grid-group-row\" colspan=\"{0}\">", columnCount + 1);

            for (var i = 0; i < groupColumns.Length; i++)
            {
                var column = groupColumns[i];
                var propertyName = String.Format("Key{0}", i);
                var value = groupKeys[propertyName];
                groupOptions.GroupKey[column.Key] = value != null ? value.ToString() : null;
                var columnTitle = column.Value.Title;
                if (column.Value.GroupTitleRender != null && value != null)
                    value = column.Value.GroupTitleRender(value);
                Writer.Write(GridModel.GridRenderOptions.GroupTitleTemplate, columnTitle, value ?? GridRenderOptions.EmptyGroupTitleText);
            }                        
            
            Writer.Write(GridModel.GridRenderOptions.GroupCountTemplate, groupItem.Count);
            Writer.Write("</td>");
            Writer.Write("</tr>");

            Writer.Write("<tr style=\"display:none;\"><td class=\"hierarchy-cell\"></td>");            
            Writer.Write("<td class=\"grid-group-row\" colspan=\"{0}\">", columnCount);
            RenderGroupGridState(groupOptions);
            Writer.Write("</td>");
            Writer.Write("<td class=\"hierarchy-cell-right\"></td></tr>");            
        }

        public override void RenderGroupTable(TModel[] items, IGridGroupOptions groupOptions, IPagedList paginationViewModel)
        {
            var isHierarchy = GridModel.HierarchyUrl != null;
            
            Writer.Write("<div class=\"grid-group-wrap\">");
            Writer.Write("<table class=\"grid-group-table\"><thead>");
            var nextSortOrder = GridModel.Multisort ? (OrderedVisibleColumns.Any(x => x.Value.SortOrder.HasValue)
                                           ? (OrderedVisibleColumns.Where(x => x.Value.SortOrder.HasValue).Max(x => x.Value.SortOrder) + 1) : 0)
                                           : 0;
            if (isHierarchy)
                Writer.Write("<th class=\"hierarchy-cell\"></th>");

            foreach (var column in OrderedVisibleColumns)
            {
                RenderColumnHeader(column.Value, column.Key, nextSortOrder.GetValueOrDefault(0),true);
            }

            if (isHierarchy)
                Writer.Write("<th class=\"hierarchy-cell-right\"></th>");

            Writer.Write("</thead>");
            Writer.Write("<tbody>");
            var alternate = false;
            for (var i = 0; i < items.Length; i++)
            {
                RenderTableRow(items[i], OrderedVisibleColumns, alternate);
                alternate = !alternate;
            }

            if (items.Length == 0)
            {
                Writer.Write("<tr>");
                Writer.Write("<td class=\"empty-grid-row\" colspan=\"{1}\">{0}</td>", GridRenderOptions.EmptyText,
                             OrderedVisibleColumns.Count() + 2);
                Writer.Write("</tr>");
            }
            else
            {
                RenderSummaryRow(items, OrderedVisibleColumns.ToArray());
            }

            Writer.Write("</tbody>");
            Writer.Write("</table>");
            
            RenderGroupGridState(groupOptions);            

            RenderPager(paginationViewModel, GridGroupPageLinkClass,true);            

            Writer.Write("</div>");
        }

        public override void RenderTableRow(TModel item, IEnumerable<KeyValuePair<string, IGridColumn<TModel>>> columns, bool alternate, int level = 0)
        {            
            var tr = new TagBuilder("tr");
            var isHierarchy = GridModel.HierarchyUrl != null;
            
            var rowAttributes = GridModel.RowAttributes!=null?GridModel.RowAttributes(item,HtmlHelper):null;
            tr.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(rowAttributes));
            if (alternate)
                tr.Attributes["class"] = tr.Attributes.ContainsKey("class")? String.Format("{0} {1}", tr.Attributes["class"], AlternateRowClass): AlternateRowClass;

            Writer.Write(tr.ToString(TagRenderMode.StartTag));

            if (isHierarchy)
                Writer.Write("<td class=\"hierarchy-cell\"><a href=\"{0}\" class=\"grid-icon {1} plus\"></a></td>", GridModel.HierarchyUrl(item, UrlHelper), GridHierarchyLinkClass);

            var cols = columns as KeyValuePair<string, IGridColumn<TModel>>[] ?? columns.ToArray();

            for (var i = 0; i < cols.Length; i++)
            {
                var td = new TagBuilder("td");
                td.MergeAttributes(cols[i].Value.Attributes);
                var value = GetCellValue(cols[i].Value, item);

                Writer.Write(td.ToString(TagRenderMode.StartTag));
                if (i == 0)
                {
                    var padding = new StringBuilder();
                    for (var j = 0; j < level * GridModel.SeparatorLevelLength; j++)                    
                        padding.Append("&nbsp;");                    
                    Writer.Write(padding.ToString());
                }
                Writer.Write(value);
                Writer.Write("</td>");                
            }            

            if (isHierarchy)
                Writer.Write("<td class=\"hierarchy-cell-right\"></td>");

            Writer.Write("</tr>");

            if (isHierarchy)
            {
                Writer.Write("<tr style=\"display:none;\"><td class=\"hierarchy-cell\"></td>");
                Writer.Write("<td class=\"grid-hierarchy-row\" colspan=\"{0}\"></td>",OrderedVisibleColumns.Count());
                Writer.Write("<td class=\"hierarchy-cell-right\"></td>");
                Writer.Write("</tr>");
            }
            var alt = alternate;

            if (GridModel.ChildSelector != null)
            {
                var childrens = GridModel.ChildSelector(item);
                if (childrens != null)
                {
                    level = level + 1;                    
                    foreach (var children in childrens)
                    {
                        alt = !alt;
                        RenderTableRow(children, cols, alt, level);
                    }                        
                }                
            }
        }

        private void RenderGroupGridState(IGridGroupOptions groupOptions)
        {            
            Writer.Write("<div style=\"display:none;\">");
            Writer.Write("<input type=\"hidden\" class=\"{0}\" value=\"{1}\" />", GridGroupStateClass, HtmlHelper.Encode(Json.Encode(groupOptions)));
            Writer.Write("</div>");
        }

        public override void RenderGridState()
        {
            var options = GridModel.Options;
            Writer.Write("<div style=\"display:none;\"><input type=\"hidden\" class=\"{0}\" value=\"{1}\" /></div>", 
                GridStateClass, HtmlHelper.Encode(Json.Encode(options)));
        }                       
    }
}
