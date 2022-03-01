using System;
using System.Collections.Specialized;

namespace ToDoListApp.Helpers
{
    public class UriBuilderExt
    {
        private UriBuilder _builder;
        private NameValueCollection _collection;

        public UriBuilderExt(string uri)
        {
            _builder = new UriBuilder(uri);
            _collection = System.Web.HttpUtility.ParseQueryString(string.Empty);
        }

        public Uri Uri
        {
            get
            {
                _builder.Query = _collection.ToString();
                return _builder.Uri;
            }
        }

        public void AddParameter(string key, string value)
        {
            _collection.Add(key, value);
        }
    }
}
