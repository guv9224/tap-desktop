﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheAirline.Model.AirlineModel.SubsidiaryModel
{
    //the class for a subsidiary airline for an airline
    public class SubsidiaryAirline : Airline
    {
        public Airline Airline { get; set; }
        public SubsidiaryAirline(Airline airline,AirlineProfile profile, AirlineMentality mentality, AirlineFocus market, AirlineLicense license)
            : base(profile, mentality, market,license,budget)
        {
            this.Airline = airline;
            this.Profile.Logos = this.Airline.Profile.Logos;
        }
        public override bool isHuman()
        {
            return this.Airline != null && this.Airline.isHuman();
        }
        public override bool isSubsidiaryAirline()
        {
            return this.Airline != null;
        }
   
    }
}
