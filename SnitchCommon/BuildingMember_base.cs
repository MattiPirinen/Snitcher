using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public class BuildingMember_base : Building_base
    {
        //---------------------- CONSTRUCTORS ------------------------

        public BuildingMember_base() : base()
        {

        }


        //----------------------- PROPERTIES -------------------------

        public string ConcreteClass { get; set; }
        public decimal G { get { return 9.81m; } }

        //------------------------ METHODS ---------------------------

        public void CalculateProperties()
        {
            Set_weight_steel_N();
            Set_weight_concrete_N();

            Set_CO2_concrete();
            Set_CO2_steel();
        }

        private decimal Get_mass_steel_kg()
        {
            return this.Volume_steel_m3 * 7850;
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
            decimal mass_concrete_kg = this.Weight_concrete_N / this.G;
            decimal kgCo2PerKgConcrete = this.Get_CO2_emissionFactor();

            this.CO2_concrete = kgCo2PerKgConcrete * mass_concrete_kg;
        }
        
        private decimal Set_CO2_steel()
        {
            return 0.67m * Get_mass_steel_kg();
        }

        public decimal Get_CO2_emissionFactor()
        {
            return (decimal)this.Co2EmissionsOfConcrete[this.ConcreteClass];
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
