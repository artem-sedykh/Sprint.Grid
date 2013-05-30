namespace Sprint.Grid.Impl
{
    using System.Text;
    using System.Web.Mvc;

    public class DefaultPager
    {
        private readonly IPagedList _pagedList;
        private readonly PagedListRenderOptions _pagedListRenderOptions;
        private readonly HtmlHelper _htmlHelper;
        private readonly string _linkClass;

        public DefaultPager(IPagedList pagedList, PagedListRenderOptions pagedListRenderOptions, HtmlHelper htmlHelper, string linkClass)
        {
            _pagedList = pagedList;
            _pagedListRenderOptions = pagedListRenderOptions;
            _htmlHelper = htmlHelper;
            _linkClass = linkClass;
        }

        public void Render()
        {
            var sb = new StringBuilder();
            sb.Append("<div class=\"pagination\">");            
            
            if (_pagedList.PageCount > 1)
            {                             
                if (_pagedList.PageNumber == 1)
                    sb.Append("<a class=\"grid-pager-button-disabled\"><i class=\"grid-pager-first\"></i></a>");
                else
                    sb.AppendFormat("<a class=\"grid-pager-button {0}\" href=\"{1}\"><i class=\"grid-pager-first\"></i></a>",_linkClass,1);                

                if (_pagedList.HasPreviousPage)
                    sb.AppendFormat("<a class=\"grid-pager-button {0}\" href=\"{1}\"><i class=\"grid-pager-prev\"></i></a>",_linkClass,_pagedList.PageNumber-1);
                else
                    sb.Append("<a class=\"grid-pager-button-disabled\"><i class=\"grid-pager-prev\"></i></a>");

                sb.Append("<span class=\"grid-pager-pagenumber\">");
                sb.AppendFormat(_pagedListRenderOptions.PageFormat, _pagedList.PageNumber, _pagedList.PageCount);
                sb.Append("</span>");                
                
                if (_pagedList.HasNextPage)
                    sb.AppendFormat("<a class=\"grid-pager-button {0}\" href=\"{1}\"><i class=\"grid-pager-next\"></i></a>",_linkClass, _pagedList.PageNumber + 1);
                else
                    sb.Append("<a class=\"grid-pager-button-disabled\"><i class=\"grid-pager-next\"></i></a>");                

                var lastPage = _pagedList.PageCount;

                if (_pagedList.PageNumber < lastPage)
                    sb.AppendFormat("<a class=\"grid-pager-button {0}\" href=\"{1}\"><i class=\"grid-pager-last\"></i></a>",_linkClass,lastPage);
                else
                    sb.Append("<a class=\"grid-pager-button-disabled\"><i class=\"grid-pager-last\"></i></a>");
                                
            }

            sb.Append("<span class=\"grid-pager-total\">");

            if (_pagedList.PageCount == 1)
                sb.AppendFormat(_pagedListRenderOptions.TotalSingleFormat, _pagedList.FirstItemOnPage, _pagedList.TotalItemCount);
            else
                sb.AppendFormat(_pagedListRenderOptions.TotalFormat, _pagedList.FirstItemOnPage, _pagedList.LastItemOnPage, _pagedList.TotalItemCount);

            sb.Append("</span>");            
            
            sb.Append("</div>");

            _htmlHelper.ViewContext.Writer.Write(sb.ToString());
        }
    }
}
