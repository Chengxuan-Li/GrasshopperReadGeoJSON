using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ReadGeoJSON
{
    public class Importer
    {
        public string Path;
        public IDictionary<string, object> Result;


        public string Type = "";
        public string Name = "";
        public string CRSName = "";
        public string PropertyNames = "";


        public Importer(string path)
        {
            Path = path;
            var json = File.ReadAllText(Path);
            Result = JsonConvert.DeserializeObject<IDictionary<string, object>>(
                json, new JsonConverter[] { new Converter() });
            object type;
            object name;
            object crs;
            object features;

            if (Result.TryGetValue("type", out type)) Type = type.ToString();
            if (Result.TryGetValue("name", out name)) Name = name.ToString();
            if (Result.TryGetValue("crs", out crs) || crs is IDictionary<string, object>)
            {
                object crsProperties;
                if ((crs as IDictionary<string, object>).TryGetValue("properties", out crsProperties) || crsProperties is IDictionary<string, object>)
                {
                    object crsName;
                    if ((crsProperties as IDictionary<string, object>).TryGetValue("name", out crsName))
                    {
                        CRSName = crsName.ToString();
                    }
                }
            }
        }

        public bool ImportInModelSpace()
        {

            return false;
        }


    }
}
