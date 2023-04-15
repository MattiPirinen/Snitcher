using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnitchCommon
{
    public abstract class BuildingObjects
    {
        //---------------------- CONSTRUCTORS ------------------------

        public BuildingObjects()
        {

        }

        //------------------------- FIELDS ---------------------------

        //----------------------- PROPERTIES -------------------------

        public string ConcreteClass { get; set; }
        public decimal Volume_conc { get; set; }
        public decimal Volume_steel { get; set; }
        public double CO2 { get; set; }

        //------------------------ METHODS ---------------------------

        private double Get_rebarWeight_kg()
        {
            return (double)this.Volume_steel * 7850;
        }

        private double Get_CO2()
        {
            double co2_concrete = Get_CO2_concrete();
            double co2_rebars = Get_CO2_rebars();

            return co2_concrete + co2_rebars;
        }

        public double Get_CO2_concrete()
        {
            decimal volume = this.Volume_conc;
            double specific_weight = 24; // kN/m^3 for unreinforced concrete
            double g = 9.81;
            double rho = specific_weight * 1000 /*to N/m^3*/ / g /*to kg/m^3*/;
            double concreteMass = (double)volume * rho; // kg
            double kgCo2PerKgConcrete = this.Get_CO2_emissionFactor();

            return kgCo2PerKgConcrete * concreteMass;
        }

        public double Get_CO2_rebars()
        {
            double steelMass = Get_rebarWeight_kg();
            double kgCo2PerKgSteel = 0.67;

            return kgCo2PerKgSteel * steelMass;
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
