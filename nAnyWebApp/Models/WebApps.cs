using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAnyWebApp.Models
{
    internal class WebApps
    {
        int Id { get; set; }
        string Name { get; set; }
        string Url { get; set; }
        string Description { get; set; }
        string ImageUrl { get; set; }
        string CustomCss { get; set; }
        string CustomJs { get; set; }
    }
}
