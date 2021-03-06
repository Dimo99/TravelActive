﻿using System.ComponentModel.DataAnnotations;
using Api.Query.CustomAttributes.Direction;

namespace TravelActive.Models.BindingModels
{
    public class CoordinatesBindingModel
    {
        [Location]
        [Required]
        public string StartingPoint { get; set; }
        [Location]
        [Required]
        public string DestinationPoint { get; set; }
    }
}