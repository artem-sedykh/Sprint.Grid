namespace Sprint.Grid.Impl
{
    using System;    

    public class PagedListModel : IPagedList
    {
        private readonly int _pageCount;
        private readonly int _totalItemCount;
        private readonly int _pageNumber;
        private readonly int _pageSize;
        private readonly bool _hasPreviousPage;
        private readonly bool _hasNextPage;
        private readonly bool _isFirstPage;
        private readonly bool _isLastPage;
        private readonly int _firstItemOnPage;
        private readonly int _lastItemOnPage;        

        public PagedListModel(int pageNumber, int pageSize, int totalItemCount)
        {
            _pageNumber = pageNumber;
            _pageSize = pageSize;            
            _totalItemCount = totalItemCount;            
            _pageCount = _totalItemCount > 0
                        ? (int)Math.Ceiling(_totalItemCount / (double)_pageSize)
                        : 0;
            _hasPreviousPage = _pageNumber > 1;
            _hasNextPage = _pageNumber < _pageCount;
            _isFirstPage = _pageNumber == 1;
            _isLastPage = _pageNumber >= _pageCount;
            _firstItemOnPage = (_pageNumber - 1) * _pageSize + 1;
            var numberOfLastItemOnPage = _firstItemOnPage + _pageSize - 1;
            _lastItemOnPage = numberOfLastItemOnPage > _totalItemCount
                            ? _totalItemCount
                            : numberOfLastItemOnPage;
        }

        public int PageCount
        {
            get { return _pageCount; }
        }

        public int TotalItemCount
        {
            get { return _totalItemCount; }
        }        

        public int PageNumber
        {
            get { return _pageNumber; }
        }

        public int PageSize
        {
            get { return _pageSize; }
        }

        public bool HasPreviousPage
        {
            get { return _hasPreviousPage; }
        }

        public bool HasNextPage
        {
            get { return _hasNextPage; }
        }

        public bool IsFirstPage
        {
            get { return _isFirstPage; }
        }

        public bool IsLastPage
        {
            get { return _isLastPage; }
        }

        public int FirstItemOnPage
        {
            get { return _firstItemOnPage; }
        }

        public int LastItemOnPage
        {
            get { return _lastItemOnPage; }
        }
    }
}
