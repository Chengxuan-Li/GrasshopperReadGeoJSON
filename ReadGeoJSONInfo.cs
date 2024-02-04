using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace ReadGeoJSON
{
    public class ReadGeoJSONInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "ReadGeoJSON";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("cf26b4c3-bfb9-4e1b-830c-263949aacdf4");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
