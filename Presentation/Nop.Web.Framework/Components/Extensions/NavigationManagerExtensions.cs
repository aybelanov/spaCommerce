using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Nop.Web.Framework.Components.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static string GetRelativePath(this NavigationManager helper, string absoluteUri = null)
        {
            var uri = absoluteUri == null ? new Uri(helper.Uri) : new Uri(absoluteUri);
            return uri.AbsolutePath;
        }

        /// <summary>
        /// Update the current page
        /// </summary>
        /// <param name="helper"></param>
        public static void UpdatePage(this NavigationManager helper)
        {
            UpdatePage(helper, false);
        }

        /// <summary>
        /// Update the current page
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="forceLoad">Force a refresh of the page</param>
        public static void UpdatePage(this NavigationManager helper, bool forceLoad)
        {
            string locationAbsoluteUri = helper.Uri;
            helper.NavigateTo(locationAbsoluteUri, forceLoad);
        }

        // TODO need to test
        /// <summary>
        /// Get values of a parameter inside the query string
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="queryParameter"></param>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetQueryParameterValues<T>(this NavigationManager helper, string queryParameter, string absoluteUri = null)
        {
            var uri = absoluteUri == null ? new Uri(helper.Uri) : new Uri(absoluteUri);
            var queryString = uri.Query;
            var queries = QueryHelpers.ParseQuery(queryString);

            foreach (var query in queries)
                if (query.Key.Equals(queryParameter, StringComparison.InvariantCultureIgnoreCase))
                {
                    // the converter doesn't convert an empty string to null for Nullable types
                    // so we have to assembly a new collection of values
                    var values = query.Value.Select(x => string.IsNullOrWhiteSpace(x) || x == "null" ? null : x);
                    var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { typeof(T) }));
                    var method = list.GetType().GetMethod("Add");
                    var converter = TypeDescriptor.GetConverter(typeof(T));

                    if (converter != null && values.Select(x => converter.IsValid(x)).All(x => x))
                    {
                        foreach (string value in values)
                            method.Invoke(list, new[] { converter.ConvertFromString(value) });

                        return (IEnumerable<T>)list;
                    }
                }

            return null;
        }

        /// <summary>
        /// if the query string has the pointed key
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key"></param>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        public static bool ContainsKey(this NavigationManager helper, string key, string absoluteUri = null)
        {
            if (key != null)
            {
                var uri = absoluteUri == null ? new Uri(helper.Uri) : new Uri(absoluteUri);
                var queryString = uri.Query;
                var queries = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(queryString);

                return queries.ContainsKey(key);
            }

            return false;
        }


        /// <summary>
        /// Remove pointed keys from a query string
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="keys"></param>
        /// <param name="absoluteUri"></param>
        /// <returns></returns>
        public static QueryString RemoveQueryString(this NavigationManager helper, IEnumerable<string> keys, string absoluteUri = null)
        {
            var uri = absoluteUri == null ? new Uri(helper.Uri) : new Uri(absoluteUri);
            var queryString = uri.Query;
            var queries = QueryHelpers.ParseQuery(queryString);
            var queryBuilder = new QueryBuilder();
            
            foreach (var key in keys)
                queries.Remove(key);

            foreach (var query in queries)
                queryBuilder.Add(query.Key, query.Value.ToArray());

            return queryBuilder.ToQueryString();
        }


        /// <summary>
        /// Modifing query parameters with new keys and values
        /// </summary>
        /// <param name="helper">this helper</param>
        /// <param name="data">an object, that is containing pairs (a props name and its value)</param>
        /// <param name="absoluteUri">a full uri fir modification</param>
        /// <returns>updated query string</returns>
        public static QueryString ModifyQueryString(this NavigationManager helper, IEnumerable<(string, object)> data, string absoluteUri = null)
        {
            var pairs = data.ToDictionary(x => x.Item1, x => x.Item2.ToString());
            return ModifyQueryString(helper, pairs, absoluteUri);
        }

     
        /// <summary>x
        /// Modifing query parameters with new keys and values
        /// </summary>
        /// <param name="helper">this helper</param>
        /// <param name="data">an object, that is containing pairs (a props name and its value)</param>
        /// <param name="absoluteUri">a full uri fir modification</param>
        /// <returns>updated query string</returns>
        public static QueryString ModifyQueryString(this NavigationManager helper, object data, string absoluteUri = null)
        {
            var props = data.GetType().GetProperties();
            var activeProps = props.Where(x => x.GetValue(data) != null);
            var pairs = activeProps.ToDictionary(x=>x.Name.ToLower(), x=>x.GetValue(data).ToString());
            return ModifyQueryString(helper, pairs, absoluteUri);
        }

        /// <summary>
        /// Modifing query parameters with new keys and values
        /// </summary>
        /// <param name="helper">this helper</param>
        /// <param name="pairs">new keys and values</param>
        /// <param name="absoluteUri">a full uri fir modification</param>
        /// <returns>updated query string</returns>
        public static QueryString ModifyQueryString(this NavigationManager helper, IDictionary<string, string> pairs, string absoluteUri = null)
        {
            var convertPairs = pairs.ToDictionary(x => x.Key, x => new StringValues(x.Value));
            return ModifyQueryString(helper, convertPairs, absoluteUri);
        }

        /// <summary>
        /// Modifing query parameters with new keys and values
        /// </summary>
        /// <param name="helper">this helper</param>
        /// <param name="pairs">new keys and values</param>
        /// <param name="absoluteUri">a full uri fir modification</param>
        /// <returns>updated query string</returns>
        public static QueryString ModifyQueryString(this NavigationManager helper, IDictionary<string, StringValues> pairs, string absoluteUri = null)
        {
            var uri = absoluteUri == null ? new Uri(helper.Uri) : new Uri(absoluteUri);
            var queryString = uri.Query;
            var queries = QueryHelpers.ParseQuery(queryString);
            var queryBuilder = new QueryBuilder();

            // we ignore and delete page number from a queri if new pairs content changing of pagesize or new search
            // because of we don't know how many pages a new viewmode pagesize contents. We start from the first page.
            if (pairs.ContainsKey("pagesize") || pairs.ContainsKey("q") || pairs.ContainsKey("specs"))
                queries.Remove("pagenumber");

            // we save only different keys
            queries.Where(x=> !pairs.ContainsKey(x.Key)).ToList()
                .ForEach(x => queryBuilder.Add(x.Key, x.Value.ToArray()));           

            // We delete pagenumber from a query if his number is 0 or 1. We don't show pagenumber in the query at the first page.
            foreach (var pair in pairs)
                if (pair.Key != "pagenumber" || (pair.Value.Count == 1 && int.TryParse(pair.Value[0], out int number) && number > 1))
                    queryBuilder.Add(pair.Key, pair.Value.ToArray());

            
            return queryBuilder.ToQueryString();
        }

        /// <summary>
        /// Sets values of the new Model to match the query string 
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="helper"></param>
        /// <param name="absoluteUri">An uri, which queries are going to parse</param>
        public static T SetModelFromQuery<T>(this NavigationManager helper, string absoluteUri = null)
            => SetModelFromQuery<T>(helper, Activator.CreateInstance<T>(), absoluteUri);


        // TODO Need to test
        /// <summary>
        /// Sets values of the Model to match the query string 
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="helper"></param>
        /// <param name="model">Model</param>
        /// <param name="absoluteUri">An uri, which queries are going to parse</param>
        /// <param name="resetPageNuber">Set the page number to null (for example for a new search)</param>
        public static T SetModelFromQuery<T>(this NavigationManager helper, T model, string absoluteUri = null)
        {
            var uri = absoluteUri == null ? new Uri(helper.Uri) : new Uri(absoluteUri);
            var queryString = uri.Query;
            var queries = QueryHelpers.ParseQuery(queryString);
            var props = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var query in queries)
                foreach (var prop in props)
                    if (query.Key.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase))
                        if (query.Value.Count == 1)
                        {
                            var value = query.Value[0];
                            var converter = TypeDescriptor.GetConverter(prop.PropertyType);

                            if (converter != null && converter.IsValid(value))
                                prop.SetValue(model, converter.ConvertFromString(value));
                        }
                        else if (prop.PropertyType.IsGenericType
                            && prop.PropertyType.GetGenericArguments().Length == 1
                            && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                             {
                                // the converter doesn't convert an empty string to null for Nullable types
                                // so we have to assembly a new collection of values
                                var values = query.Value.Select(x => string.IsNullOrWhiteSpace(x) || x == "null" ? null : x);
                                var baseType = prop.PropertyType.GetGenericArguments()[0];
                                var list = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { baseType }));
                                var method = list.GetType().GetMethod("Add");
                                var converter = TypeDescriptor.GetConverter(baseType);

                                if (converter != null && values.Select(x => converter.IsValid(x)).All(x => x))
                                {
                                    foreach (string value in values)
                                        method.Invoke(list, new[] { converter.ConvertFromString(value) });

                                    prop.SetValue(model, list);
                                }
                             }

            return model;
        }
    }
}
