using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RestfulRouting
{
    public class AdditionalAction
    {
        private IDictionary<string, KeyValuePair<string, HttpVerbs[]>> _actions;

        public AdditionalAction(IDictionary<string, KeyValuePair<string, HttpVerbs[]>> actions)
        {
            _actions = actions;
        }

        public void Get(string action)
        {
            Route(action, action, HttpVerbs.Get);
        }

        public void Post(string action)
        {
            Route(action, action, HttpVerbs.Post);
        }

        public void Put(string action)
        {
            Route(action, action, HttpVerbs.Put);
        }

        public void Delete(string action)
        {
            Route(action, action, HttpVerbs.Delete);
        }

        public void Head(string action)
        {
            Route(action, action, HttpVerbs.Head);
        }

        public void Get(string resource, string action)
        {
            Route(resource, action, HttpVerbs.Get);
        }

        public void Post(string resource, string action)
        {
            Route(resource, action, HttpVerbs.Post);
        }

        public void Put(string resource, string action)
        {
            Route(resource, action, HttpVerbs.Put);
        }

        public void Delete(string resource, string action)
        {
            Route(resource, action, HttpVerbs.Delete);
        }

        public void Head(string resource, string action)
        {
            Route(resource, action, HttpVerbs.Head);
        }

        private void Route(string resource, string action, HttpVerbs verb)
        {
            var actionName = RouteSet.LowercaseActions ? action.ToLowerInvariant() : action;
            if (!_actions.ContainsKey(action))
                _actions[actionName] = new KeyValuePair<string,HttpVerbs[]>(resource, new[] { verb });
            else
            {
                var keyValue = _actions[actionName];
                var verbs = keyValue.Value.ToList();
                verbs.Add(verb);
                _actions[actionName] = new KeyValuePair<string,HttpVerbs[]>(keyValue.Key, verbs.ToArray());
            }
        }
    }
}
