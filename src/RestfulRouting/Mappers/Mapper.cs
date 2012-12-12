using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using RestfulRouting.Mappers;

namespace RestfulRouting
{
    public interface IMapper
    {
        void Root<TController>(Expression<Func<TController, object>> action);
        void Route(RouteBase routeBase);
        void Resources<TController>(Action<IResourcesMapper<TController>> mapper = null) where TController : Controller;
        void Resource<TController>(Action<IResourceMapper<TController>> mapper = null) where TController : Controller;
        void Area<TController>(string name, Action<IAreaMapper> mapper = null) where TController : Controller;
        void Area<TController>(string name, string pathPrefix, Action<IAreaMapper> mapper) where TController : Controller;
        void Area(string name, Action<IAreaMapper> mapper);
        void Area(string name, string pathPrefix, Action<IAreaMapper> mapper);
        StandardMapper Path(string path);
        void Connect<TRouteSet>(string path = "", string[] namespaces = null) where TRouteSet : RouteSet, new();
        void WithRouteHandler(IRouteHandler routeHandler);
        void DebugRoute(string path);
    }

    public class Mapper : IMapper
    {
        protected List<Mapper> Mappers = new List<Mapper>();
        protected string BasePath;
        protected IRouteHandler RouteHandler = new MvcRouteHandler();
        protected RouteValueDictionary Constraints = new RouteValueDictionary();
        protected List<string> ResourcePaths = new List<string>();
        protected string[] Namespaces;
        
        public Mapper(string[] namespaces = null)
        {
            this.Namespaces = namespaces;
        }

        public void Root<TController>(Expression<Func<TController, object>> action)
        {
            AddMapper(new RootMapper<TController>(action));
        }

        public void Route(RouteBase routeBase)
        {
            AddMapper(new RouteMapper(routeBase));
        }

        public void Resources<TController>(Action<IResourcesMapper<TController>> mapper = null) where TController : Controller
        {
            AddMapper(new ResourcesMapper<TController>(mapper));
        }

        public void Resource<TController>(Action<IResourceMapper<TController>> mapper = null) where TController : Controller
        {
            AddMapper(new ResourceMapper<TController>(mapper));
        }

        public void Area<TController>(string name, Action<IAreaMapper> mapper = null) where TController : Controller
        {
            AddMapper(new AreaMapper(name, typeof(TController).Namespace, mapper));
        }

        public void Area<TController>(string name, string pathPrefix, Action<IAreaMapper> mapper) where TController : Controller
        {
            AddMapper(new AreaMapper(name, typeof(TController).Namespace, mapper));
        }

        public void Area(string name, Action<IAreaMapper> mapper)
        {
            AddMapper(new AreaMapper(name, null, mapper));
        }

        public void Area(string name, string pathPrefix, Action<IAreaMapper> mapper)
        {
            AddMapper(new AreaMapper(name, null, mapper));
        }

        public virtual StandardMapper Path(string path)
        {
            var mapper = new StandardMapper().Path(path);
            AddMapper(mapper);
            return mapper;
        }

        public virtual void Connect<TRouteSet>(string path = "", string[] namespaces = null) where TRouteSet : RouteSet, new()
        {
            ConnectMapper<TRouteSet> set = new ConnectMapper<TRouteSet>(path);
            set.Namespaces = namespaces;
            AddMapper(set);
        }

        public void WithRouteHandler(IRouteHandler routeHandler)
        {
            RouteHandler = routeHandler;
        }

        public void DebugRoute(string path)
        {
            AddMapper(new DebugRouteMapper(path));
        }

        private void AddMapper(Mapper mapper)
        {
            if (mapper.Namespaces == null)
                mapper.Namespaces = this.Namespaces;
            Mappers.Add(mapper);
        }

        protected void AddResourcePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
                ResourcePaths.Add(path);
        }

        protected virtual void SetBasePath(string basePath)
        {
            BasePath = basePath;
        }

        public virtual void RegisterRoutes(RouteCollection routeCollection)
        {
            foreach (var mapper in Mappers)
            {
                mapper.WithRouteHandler(RouteHandler);
                mapper.RegisterRoutes(routeCollection);
            }
        }

        protected string Join(params string[] parts)
        {
            var validParts = parts.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return string.Join("/", validParts);
        }

        protected string GetControllerName<T>()
        {
            var controllerName = typeof(T).Name;

            string name = controllerName.Substring(0, controllerName.Length - "Controller".Length);
            return RouteSet.LowercaseDefaults ? name.ToLowerInvariant() : name;
        }

        protected void RegisterNested(RouteCollection routeCollection, Action<Mapper> action = null)
        {
            foreach (var mapper in Mappers)
            {
                ConfigureNestedMapper(mapper);

                if (action != null)
                {
                    action(mapper);
                }

                mapper.RegisterRoutes(routeCollection);
            }
        }

        private void ConfigureNestedMapper(Mapper mapper)
        {
            mapper.SetBasePath(BasePath);
            mapper.WithRouteHandler(RouteHandler);
            mapper.InheritConstraints(Constraints);
            mapper.InheritNamespaces(Namespaces);
        }

        private void InheritConstraints(RouteValueDictionary constraints)
        {
            foreach (var constraint in constraints)
            {
                if (!Constraints.ContainsKey(constraint.Key))
                {
                    Constraints[constraint.Key] = constraint.Value;
                }
            }
        }

        private void InheritNamespaces(string[] namespaces)
        {
            if (this.Namespaces == null)
                this.Namespaces = namespaces;
            else
                this.Namespaces = ((string[])this.Namespaces).Concat(namespaces).ToArray<string>();
        }

        protected void ConfigureRoute(Route route)
        {
            if (route.Constraints == null)
                route.Constraints = new RouteValueDictionary();
            foreach (var constraint in Constraints)
            {
                route.Constraints[constraint.Key] = constraint.Value;
            }
            
            if (this.Namespaces != null && this.Namespaces.Length > 0)
            {
                if (route.DataTokens == null)
                    route.DataTokens = new RouteValueDictionary();
                
                route.DataTokens["Namespaces"] = this.Namespaces;
            }
        }

        public void SetParentResources(List<string> resources)
        {
            ResourcePaths = resources;
        }

        public string JoinResources(string with)
        {
            var resources = new List<string>();
            resources.AddRange(ResourcePaths);
            resources.Add(with);
            return string.Join("_", resources.Select(r => r.ToLowerInvariant()));
        }
    }
}
