using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MopUpData.Models
{
    class ProgressBarModel
    {
        public int PercentageComplete { get; set; } = 0;
        public List<int> SitesDownloaded { get; set; } = new List<int>();
    }
}
