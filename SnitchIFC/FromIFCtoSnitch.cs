using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc.Extensions;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Rhino.Geometry;
using SnitchCommon;

namespace SnitchIFC
{
    public static class FromIFCtoSnitch
    {
        public static Building importIFC(string filePath)
        {

            Building building = new Building();
            StringBuilder sb = new StringBuilder();
            // List of variables looked for in the ifc model
            // Units unclear!
            var wantedProperties = new List<string>
            {
                "Weight",
                "Height",
                "Length",
                "Width",
                "Volume",
                "Gross footprint area",
                "Area per tons",
                "Net surface area",
                "Bottom elevation",
                "Top elevation"
            };

            using (var model = IfcStore.Open(filePath))
            {
                // Check if project exists
                string projectName = Path.GetFileNameWithoutExtension(filePath);

                // Create list of all elements (Empty for now)
                var allElements = new List<IIfcElement>();

                // Fill the list of allElements with all of the selected elements (Reduces the amount of loops needed)
                allElements.AddRange(model.Instances.OfType<IIfcBeam>().ToList());
                allElements.AddRange(model.Instances.OfType<IIfcColumn>().ToList());
                allElements.AddRange(model.Instances.OfType<IIfcSlab>().ToList());

                // Loop through the elements
                foreach (var element in allElements)
                {
                    // Get the element tye name
                    var elementTypeName = element.GetType().Name;

                    string addElement = $"INSERT INTO {elementTypeName}";

                    // Print the basic information
                    Console.WriteLine($"{elementTypeName} ID: {element.GlobalId}");
                    Console.WriteLine($"{elementTypeName} name: {element.Name}");


                    double length = 0;


                    string columnNames = "(" +
                        "uniqueId, " +
                        "Name, " +
                        "Weight, " +
                        "Volume, " +
                        "GrossFootprintArea, " +
                        "AreaPerTons, " +
                        "NetSurfaceArea, " +
                        "Height, " +
                        "Width, " +
                        "Length, " +
                        "BottomElevation, " +
                        "TopElevation, " +
                        "ProjectName)";

                    string columnValues = $"('{element.GlobalId}', '{element.Name}', ";

                    // Get all single-value properties of the beam
                    var properties = element.IsDefinedBy
                        .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                        .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties)
                        .OfType<IIfcPropertySingleValue>();
                    
                    foreach (var property in properties)
                    {
                        if (wantedProperties.Contains(property.Name))
                        {
                            if (property.Name == "Length")
                            {
                                length = double.Parse(property.NominalValue.ToString());
                            }
                            columnValues += $"'{property.NominalValue}', ";
                            sb.AppendLine($"{elementTypeName} {property.Name}: {property.NominalValue}");
                        }
                    }
                    columnValues += $"'{projectName}')";

                    if (element is IfcBeam || element is IfcColumn ifcColumn)
                    {
                        // Get element location
                        IfcLocalPlacement test1 = (IfcLocalPlacement)element.ObjectPlacement;
                        IfcAxis2Placement3D placement = (IfcAxis2Placement3D)test1.RelativePlacement;
                        Point3d pt1 = CartesianPointToPoint3d(placement.Location);
                        Point3d pt2;
                        if (element is IfcBeam)
                        {
                            pt2 = new Point3d()
                            {
                                X = placement.Location.X + placement.RefDirection.X * length,
                                Y = placement.Location.Y + placement.RefDirection.Y * length,
                                Z = placement.Location.Z + placement.RefDirection.Z * length
                            };
                        }
                        else
                        {
                            pt2 = new Point3d()
                            {
                                X = placement.Location.X + placement.Axis.X * length,
                                Y = placement.Location.Y + placement.Axis.Y * length,
                                Z = placement.Location.Z + placement.Axis.Z * length
                            };
                        }
                        

                        Beam beam = new Beam();
                        beam.CenterLine = new Line(pt1, pt2);
                        building.Beams.Add(beam.Guid, beam);

                        sb.AppendLine($"startPt:{pt1}, entPt:{pt2}");
                    }
                }
            }
            return building;
        }

        private static Point3d CartesianPointToPoint3d(IfcCartesianPoint location)
        {
            return new Point3d()
            {
                X = location.X,
                Y = location.Y,
                Z = location.Z
            };
        }
    }
}
