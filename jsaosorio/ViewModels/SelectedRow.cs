using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jsaosorio.Models
{
    public class SelectedRow
    {
        public string Domain { get; set; }
        public List<string> Emails { get; set; }
        public string DefaultEmail { get; set; }
        public bool IsCustom { get; set; }
    }
}
