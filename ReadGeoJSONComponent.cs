using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace ReadGeoJSON
{
    public class ReadGeoJSONComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ReadGeoJSONComponent()
          : base("ReadGeoJSON", "ReadGeoJSON",
              "ReadGeoJSON",
              "User", "Primitive")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddTextParameter("Path", "P", "Path of File", GH_ParamAccess.item);
            pManager.AddBooleanParameter("ImportInModelSpace", "IIMS", "True if the geometries and properties should be imported directly into the model space of Rhino", GH_ParamAccess.item);

            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.
            pManager.AddTextParameter("Message", "Msg", "Custom message to display", GH_ParamAccess.item);
            pManager.AddTextParameter("Type", "Type", @"Type of GeoJSON, expect ""Feature Collection""", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Name", "Name of the GeoJSON layer", GH_ParamAccess.item);
            pManager.AddTextParameter("CRS", "CRS", "Name of the coordinate reference system", GH_ParamAccess.item);
            pManager.AddTextParameter("GeometryType", "GType", "Geometry type of the first feature detected", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Geometries", "Geo", "Underlying geometries", GH_ParamAccess.list);
            pManager.AddTextParameter("PropertyNames", "PNames", "List of property names", GH_ParamAccess.list);
            pManager.AddTextParameter("Guids", "Guids", "List of Guids", GH_ParamAccess.list);
            //pManager.AddTextParameter("PropertiesDataFlow", "PDF", "List of properties data flow", GH_ParamAccess.list);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.
            string path = "";
            
            bool importInModelSpace = false;


            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetData(0, ref path)) return;
            DA.GetData(1, ref importInModelSpace);

            if (path.Length <= 1)
            {
                return;
            }

            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
            // The actual functionality will be in a different method:
            Importer importer = new Importer(path, importInModelSpace);

            // Finally assign the spiral to the output parameter.
            DA.SetData(0, importer.Msg);
            DA.SetData(1, importer.Type);
            DA.SetData(2, importer.Name);
            DA.SetData(3, importer.CRSName);
            DA.SetData(4, importer.GeometryType);
            DA.SetDataList(5, importer.Geometries);
            DA.SetDataList(6, importer.PropertyNames);
            DA.SetDataList(7, importer.GuidStrings);
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("1345dc18-8efb-4daf-8e65-8b0fdcee5311"); }
        }
    }
}
