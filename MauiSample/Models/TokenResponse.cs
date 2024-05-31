using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matrix42.mobile.app.ui.Models
{
    public class TokenResponse
    {
        public string ApiToken { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public DateTime ValidUntil { get; set; }
        public string UserId { get; set; }
    }
}
