using System;

namespace TravelActive.Common.Extensions
{
    public static class NumericsExtensions
    {
        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
        
    }
}