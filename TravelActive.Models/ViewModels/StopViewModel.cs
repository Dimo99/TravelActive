﻿using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class StopViewModel
    {
        public string StopName { get; set; }
        public LatLng LatLng { get; set; }
        public override string ToString()
        {
            return $"{LatLng.Latitude},{LatLng.Longitude}";
        }
    }
}