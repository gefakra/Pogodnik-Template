using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Models
{
    public class ForecastItem
    {
        public string Date { get; set; }
        public string Icon { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
    }
}
