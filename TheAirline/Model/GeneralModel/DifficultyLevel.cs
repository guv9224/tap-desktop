﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheAirline.Model.GeneralModel
{
    //the class for a difficulty level
    public class DifficultyLevel
    {
        public string Name { get; set; }
        public double MoneyLevel { get; set; }
        public double LoanLevel { get; set; }
        public double PassengersLevel { get; set; }
        public double PriceLevel { get; set; }
        public double AILevel { get; set; }
        public DifficultyLevel(string name, double money, double loan, double passengers, double price, double AI)
        {
            this.Name = name;
            this.MoneyLevel = money;
            this.LoanLevel = loan;
            this.PassengersLevel = passengers;
            this.PriceLevel = price;
            this.AILevel = AI;
        }
    }
    //the list of diffiulty levels
    public class DifficultyLevels
    {
        private static List<DifficultyLevel> levels = new List<DifficultyLevel>();
        //adds a difficulty level to the level
        public static void AddDifficultyLevel(DifficultyLevel level)
        {
            levels.Add(level);
        }
        //returns the list of levels
        public static List<DifficultyLevel> GetDifficultyLevels()
        {
            return levels;
        }
        //returns a difficulty level based on name
        public static DifficultyLevel GetDifficultyLevel(string name) 
        {
            return levels.Find(l => l.Name == name);
        }
        //removes a difficulty level
        public static void RemoveDifficultyLevel(DifficultyLevel level)
        {
            levels.Remove(level);
        }
    }
}