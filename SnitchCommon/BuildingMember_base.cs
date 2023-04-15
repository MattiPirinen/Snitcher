using Rhino.Geometry;
using System.Collections.Generic;

namespace SnitchCommon
{
    public class BuildingMember_base : Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public BuildingMember_base() : base()
        {

        }


        //----------------------- PROPERTIES -------------------------
        public int FloorNo { get; set; }
        public string ConcreteClass { get; set; }
        public double G { get { return 9.81; } }
        public Mesh Mesh { get; set; }

        //------------------------ METHODS ---------------------------

        public void CalculateProperties(AverageCo2Values averageCo2Values)
        {
            Set_weight_steel_N();
            Set_weight_concrete_N();

            Set_CO2_concrete();
            Set_CO2_steel();

            CalculateScore(averageCo2Values);
        }

        private void CalculateScore(AverageCo2Values averageCo2Values)
        {
            double ref_val = ChooseReferenceValue(averageCo2Values);

            this.Score = this.CO2_total / ref_val - 1;
        }

        private double ChooseReferenceValue(AverageCo2Values averageCo2Values)
        {
            if(this is Column)
            {
                return averageCo2Values.Column;
            }
            else if(this is Beam)
            {
                return averageCo2Values.Beam;
            }
            else if(this is Slab)
            {
                return averageCo2Values.Slab;
            }
            else if(this is Wall)
            {
                return averageCo2Values.Wall;
            }
            else
            {
                return 2E-16;
            }
        }

        private double Get_mass_steel_kg()
        {
            return this.Mass_steel_m3 * 7850;
        }

        private void Set_weight_steel_N()
        {
            this.Weight_steel_N = Get_mass_steel_kg() * this.G;
        }

        private void Set_weight_concrete_N()
        {
            this.Weight_concrete_N = this.Volume_concrete_m3 * 24000;
        }

        private void Set_CO2_concrete()
        {
            double mass_concrete_kg = this.Weight_concrete_N / this.G;
            double kgCo2PerKgConcrete = this.Get_CO2_emissionFactor();

            this.CO2_concrete = kgCo2PerKgConcrete * mass_concrete_kg;
        }
        
        private double Set_CO2_steel()
        {
            return 0.67 * Get_mass_steel_kg();
        }

        public double Get_CO2_emissionFactor()
        {
            return this.Co2EmissionsOfConcrete[this.ConcreteClass];
        }

        private Dictionary<string, double> Co2EmissionsOfConcrete { get; set; } = new Dictionary<string, double>
        {
            {"C12/15", 0.09},
            {"C16/20", 0.106}, // not in co2data
            {"C20/25", 0.106},
            {"C25/30", 0.115},
            {"C30/37", 0.13},
            {"C35/45", 0.14},
            {"C40/50", 0.16}, // not in co2data
            {"C45/55", 0.16},
            {"C50/60", 0.17},
            {"C55/67", 0.17}, // not in co2data
            {"C60/75", 0.17}, // not in co2data
            {"C70/85", 0.17}, // not in co2data
            {"C80/95", 0.17}, // not in co2data
            {"C90/105", 0.17}, // not in co2data

        };

    }
}
