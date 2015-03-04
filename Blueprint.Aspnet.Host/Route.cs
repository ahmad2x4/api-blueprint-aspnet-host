using System.Collections.Generic;

namespace Blueprint.Aspnet.Host
{
    public class Route : IRoute
    {
        protected bool Equals(Route other)
        {
            return string.Equals(UrlTemplate, other.UrlTemplate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Route) obj);
        }

        public override int GetHashCode()
        {
            return (UrlTemplate != null ? UrlTemplate.GetHashCode() : 0);
        }

        public sealed class UrlTemplateEqualityComparer : IEqualityComparer<IRoute>
        {
            public bool Equals(IRoute x, IRoute y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.UrlTemplate, y.UrlTemplate);
            }

            public int GetHashCode(IRoute obj)
            {
                return (obj.UrlTemplate != null ? obj.UrlTemplate.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<IRoute> UrlTemplateComparerInstance = new UrlTemplateEqualityComparer();

        public static IEqualityComparer<IRoute> UrlTemplateComparer
        {
            get { return UrlTemplateComparerInstance; }
        }

        public Route(string urlTemplate)
        {
            UrlTemplate = urlTemplate;
        }

        public string UrlTemplate { get; private set; }
    }
}