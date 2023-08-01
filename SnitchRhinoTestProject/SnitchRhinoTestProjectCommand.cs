using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using SnitchCommon;
using SnitchIFC;
using System;
using System.Collections.Generic;

namespace SnitchRhinoTestProject
{
    public class SnitchRhinoTestProjectCommand : Command
    {
        public SnitchRhinoTestProjectCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static SnitchRhinoTestProjectCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "SnitchRhinoTestProjectCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string filePath = "C:\\Users\\pima\\OneDrive - Ramboll\\Documents\\Projektit\\Diplomityöt\\Niko Partanen\\ifc_testing_model.ifc";
            Building output = FromIFCtoSnitch.importIFC(filePath);
            foreach (var beam in output.Beams.Values)
            {
                doc.Objects.AddLine(beam.CenterLine);
            }
            output.CalculateBeamLoadBearingWidths();
            doc.Views.Redraw();

            return Result.Success;
        }
    }
}
