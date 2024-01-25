using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Rhino;
using Rhino.Geometry;
using Rhino.Collections;
using Rhino.DocObjects;

namespace ReadGeoJSON
{
    public class Importer
    {
        public string Path;
        public bool ImportInModelSpace;
        public IDictionary<string, object> Result;

        public string Msg = "";
        public string Type = "";
        public string Name = "";
        public string CRSName = "";
        public string GeometryType = "";
        public List<GeometryBase> Geometries = new List<GeometryBase>();
        public List<string> PropertyNames = new List<string>();
        public List<string> GuidStrings = new List<string>();

        public RhinoDoc Doc;

        public Importer(string path, bool importInModelSpace)
        {
            Path = path;
            ImportInModelSpace = importInModelSpace;
            Doc = RhinoDoc.ActiveDoc;
            var json = File.ReadAllText(Path);
            Result = JsonConvert.DeserializeObject<IDictionary<string, object>>(
                json, new JsonConverter[] { new Converter() });
            object type;
            object name;
            object crs;
            object features;
            

            if (Result.TryGetValue("type", out type)) Type = type.ToString();
            if (Result.TryGetValue("name", out name)) Name = name.ToString();
            if (Result.TryGetValue("crs", out crs) && crs is IDictionary<string, object>)
            {
                object crsProperties;
                if ((crs as IDictionary<string, object>).TryGetValue("properties", out crsProperties) && crsProperties is IDictionary<string, object>)
                {
                    object crsName;
                    if ((crsProperties as IDictionary<string, object>).TryGetValue("name", out crsName))
                    {
                        CRSName = crsName.ToString();
                    }
                }
            }
            if (Result.TryGetValue("features", out features) && features is JArray)
            {
                JArray featuresList = (features as JArray);
                object featureProperties;
                object featureGeometry;
                IDictionary<string, object> featurePropertiesDictionary;
                IDictionary<string, object> featureGeometryDictionary;
                object geometryType;
                GeometryBase geometry;
                

                for (int i = 0; i < featuresList.Count; i++)
                {
                    IDictionary<string, object> feature = (featuresList[i] as JToken).ToObject<IDictionary<string, object>>();
                    
                    if (feature.TryGetValue("properties", out featureProperties))
                    {
                        featurePropertiesDictionary = (featureProperties as JToken).ToObject<IDictionary<string, object>>();
                        if (i == 0) PropertyNames = featurePropertiesDictionary.Keys.ToList();
                        if (feature.TryGetValue("geometry", out featureGeometry))
                        {
                            featureGeometryDictionary = (featureGeometry as JToken).ToObject<IDictionary<string, object>>();
                            if (i == 0 && featureGeometryDictionary.TryGetValue("type", out geometryType)) GeometryType = geometryType.ToString();
                            GenerateGeometry(featureGeometryDictionary, out geometry);
                            if (!(geometry is null)) Geometries.Add(geometry);
                            if (ImportInModelSpace) GuidStrings.Add(BakeGeometry(geometry, featurePropertiesDictionary).ToString());
                        }
                    }
                }

            }
        }

        void GenerateGeometry(IDictionary<string, object> geometryDictionary, out GeometryBase geometry)
        {
            object geometryType;
            string thisGeometryType = "";
            object coordinates;
            if (geometryDictionary.TryGetValue("type", out geometryType)) thisGeometryType = geometryType.ToString();
            if (geometryDictionary.TryGetValue("coordinates", out coordinates))
            {
                if (thisGeometryType == "MultiPolygon")
                {
                    geometry = GenerateMultiPolygon(coordinates);
                    return;
                }    
            }
            geometry = null; 
        }

        PolylineCurve GenerateMultiPolygon(object coordinates)
        {
            if (coordinates is JArray)
            {
                var coordinatesArray = (coordinates as JArray).ToObject<double[][][][]>();
                var cdnts = coordinatesArray[0][0];
                List<Point3d> points = new List<Point3d>();
                foreach (double[] coordinate in cdnts)
                {
                    points.Add(new Point3d(coordinate[0], coordinate[1], coordinate[2]));
                }
                
                return new Polyline(points).ToPolylineCurve();
            } else
            {
                return null;
            }
        }

        Guid BakeGeometry(GeometryBase geometry, IDictionary<string, object> properties)
        {
            ObjectAttributes objectAttributes = new ObjectAttributes();
            objectAttributes.SetUserString("GeoJSONName", Name);

            object queryObj;
            foreach (string key in properties.Keys)
            {
                if (properties.TryGetValue(key, out queryObj))
                {
                    if (queryObj is null)
                    {
                        objectAttributes.SetUserString(key, "");
                    } else
                    {
                        objectAttributes.SetUserString(key, queryObj.ToString());
                    }
                    
                }
            }

            return Doc.Objects.Add(geometry, objectAttributes);
        }
    }
}
