using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MopUpData.Models
{
    class PostObjects
    {
    }
    public class TaskColor
    {
        public string Name { get; set; }
    }

    public class Object
    {
        [JsonProperty("@objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("@createOrUpdate")]
        public bool CreateOrUpdate { get; set; }
        public string CallID { get; set; }
        public int Number { get; set; }
        public TaskColor TaskColor { get; set; }
    }

    public class Operation
    {
        public string Action { get; set; }
        public Object Object { get; set; }
    }

    public class Root
    {
    
        public List<Operation> Operations { get; set; }
        public bool OneTransaction { get; set; }
        public bool ErrorOnNonExistingDictionaries { get; set; }
        public bool ContinueOnError { get; set; }
    }

}
