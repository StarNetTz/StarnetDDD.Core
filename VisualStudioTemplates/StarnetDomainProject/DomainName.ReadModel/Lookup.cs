using System.Collections.Generic;

namespace $safeprojectname$
{
    public class Lookup
    {
        public string Id { get; set; }
        public List<LookupItem> Data { get; set; }

        public Lookup()
        {
            Data = new List<LookupItem>();
        }
    }
}
