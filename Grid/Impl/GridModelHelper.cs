namespace Sprint.Grid.Impl
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Web.Helpers;
    using Helpers;

    public static class GridModelHelper
    {
        internal static void InitOrders<TModel>(this IGridModel<TModel> gridModel) where TModel:class
        {
            var groupIndex = 0;
            var columnIndex = 0;
            var sortIndex = 0;
            var visibleColumns = gridModel.Columns.Where(x => x.Value.IsVisible).ToList();

            foreach (var column in visibleColumns.Where(x => x.Value.GroupOrder.HasValue).OrderBy(x => x.Value.GroupOrder))
                column.Value.GroupOrder = groupIndex++;

            foreach (var column in visibleColumns.Where(x => x.Value.SortDirection.HasValue).OrderBy(x => x.Value.SortOrder))
                column.Value.SortOrder = sortIndex++;

            foreach (var column in gridModel.Columns.OrderBy(x => x.Value.Order))
                column.Value.Order = columnIndex++;                
        }

        public static void MergeGridOptions<TModel>(this IGridModel<TModel> gridModel, IGridOptions options) where TModel : class
        {
            gridModel.InitOrders();
            
            options.PageOpt = options.PageOpt ?? new Dictionary<string, string>();
            options.ColOpt = options.ColOpt ?? new Dictionary<string, Dictionary<string, object>>();

            #region PageOptions

            var page = options.PageOpt.ContainsKey("p") ? Convert.ToInt32(options.PageOpt["p"]) : gridModel.Page;

            if (page == gridModel.Page)
                options.PageOpt.Remove("p");
            else
                gridModel.Page = page;

            var pageSize = options.PageOpt.ContainsKey("ps") ? Convert.ToInt32(options.PageOpt["ps"]) : gridModel.PageSize;

            if (pageSize == gridModel.PageSize)
                options.PageOpt.Remove("ps");
            else
                gridModel.PageSize = pageSize;

            var groupPageSize = options.PageOpt.ContainsKey("gps") ? Convert.ToInt32(options.PageOpt["gps"]) : gridModel.PageSizeInGroup;

            if (groupPageSize == gridModel.PageSizeInGroup)
                options.PageOpt.Remove("gps");
            else
                gridModel.PageSizeInGroup = groupPageSize;

            #endregion

            foreach (var column in options.ColOpt)
            {
                if (column.Value == null || !column.Value.Any()) continue;

                var columnOptions = column.Value;

                var col = gridModel.Columns.FirstOrDefault(x => x.Key == column.Key);

                #region Column

                var order = columnOptions.ContainsKey("co") && columnOptions["co"] != null ? (Convert.ToInt32(columnOptions["co"])) : col.Value.Order;

                if (order == col.Value.Order)
                    columnOptions.Remove("co");
                else
                    col.Value.Order = order;

                var visible = columnOptions.ContainsKey("vc") && columnOptions["vc"] != null ? (Convert.ToBoolean(columnOptions["vc"])) : col.Value.IsVisible;

                if (visible == col.Value.IsVisible)
                    columnOptions.Remove("vc");
                else
                    col.Value.IsVisible = visible;

                #endregion

                #region Sorting

                if (gridModel.Multisort)
                {
                    var sortOrder = columnOptions.ContainsKey("so") ? Helpers.ToNullable<Int32>(columnOptions["so"]) : col.Value.SortOrder;

                    if (sortOrder == col.Value.SortOrder)
                        columnOptions.Remove("so");
                    else
                        col.Value.SortOrder = sortOrder;
                }

                var sortDirection = columnOptions.ContainsKey("sd") ? (SortDirection?)Helpers.ToNullable<Int32>(columnOptions["sd"]) : col.Value.SortDirection;

                if (sortDirection == col.Value.SortDirection)
                    columnOptions.Remove("sd");
                else
                    col.Value.SortDirection = sortDirection;

                #endregion

                #region Grouping

                var groupSortDirection = columnOptions.ContainsKey("gd") ? ((SortDirection?)Helpers.ToNullable<Int32>(columnOptions["gd"])) : col.Value.GroupSortDirection;

                if (groupSortDirection == col.Value.GroupSortDirection)
                    columnOptions.Remove("gd");
                else
                    col.Value.GroupSortDirection = groupSortDirection;

                var groupOrder = columnOptions.ContainsKey("go") ? Helpers.ToNullable<Int32>(columnOptions["go"]) : col.Value.GroupOrder;

                if (groupOrder == col.Value.GroupOrder)
                    columnOptions.Remove("go");
                else
                    col.Value.GroupOrder = groupOrder;

                #endregion
            }

            //Выставляем опции по умолчанию
            foreach (var column in gridModel.Columns.Where(c => c.Value.IsVisible && (c.Value.SortDirection.HasValue || c.Value.GroupOrder.HasValue)))
            {
                var key = column.Key;
                var existKey = options.ColOpt.ContainsKey(key);
                options.ColOpt[key] = existKey ? options.ColOpt[key] : new Dictionary<string, object>();
                var columnOptions = options.ColOpt[key];

                if (column.Value.SortDirection.HasValue)
                {
                    if (!columnOptions.ContainsKey("sd"))
                        columnOptions["sd"] = (int)column.Value.SortDirection.Value;

                    if (!columnOptions.ContainsKey("so") && gridModel.Multisort)
                        columnOptions["so"] = column.Value.SortOrder;
                }

                if (!column.Value.GroupOrder.HasValue) continue;
                if (!columnOptions.ContainsKey("go"))
                    columnOptions["go"] = column.Value.GroupOrder;

                if (!columnOptions.ContainsKey("gd"))
                    columnOptions["gd"] = (int?)column.Value.GroupSortDirection;
            }

            //Чистим пустые опции
            var emptyColumnOptions = options.ColOpt.Where(x => !x.Value.Any()).Select(x => x.Key).ToList();
            foreach (var key in emptyColumnOptions)
                options.ColOpt.Remove(key);

            gridModel.InitOrders();
        }

        public static void MergeGridGroupOptions<TModel>(this IGridModel<TModel> gridModel, IGridGroupOptions options) where TModel : class
        {
            options.GColOpt = options.GColOpt ?? new Dictionary<string, IDictionary<string, object>>();

            gridModel.InitOrders();

            foreach (var column in gridModel.Columns.Where(x=>x.Value.IsVisible))
            {
                column.Value.SortDirection = null;
                column.Value.SortOrder = null;
            }

            foreach (var column in options.GColOpt)
            {                
                if (column.Value == null || !column.Value.Any()) continue;

                var columnOptions = column.Value;

                var col = gridModel.Columns.FirstOrDefault(x => x.Key == column.Key);

                if (gridModel.Multisort)
                {
                    var sortOrder = columnOptions.ContainsKey("so") ? Helpers.ToNullable<Int32>(columnOptions["so"]) : col.Value.SortOrder;

                    if (sortOrder == col.Value.SortOrder)
                        columnOptions.Remove("so");
                    else
                        col.Value.SortOrder = sortOrder;
                }

                var sortDirection = columnOptions.ContainsKey("sd") ? (SortDirection?)Helpers.ToNullable<Int32>(columnOptions["sd"]) : col.Value.SortDirection;

                if (sortDirection == col.Value.SortDirection)
                    columnOptions.Remove("sd");
                else
                    col.Value.SortDirection = sortDirection;
            }

            //Чистим пустые опции
            var emptyColumnOptions = options.GColOpt.Where(x => !x.Value.Any()).Select(x => x.Key).ToList();
            foreach (var key in emptyColumnOptions)
                options.GColOpt.Remove(key);

            gridModel.InitOrders();
        }
    }
}
