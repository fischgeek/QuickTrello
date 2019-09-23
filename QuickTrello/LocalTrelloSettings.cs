using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickTrello
{
    public class LocalTrelloSettings
    {
        public string ApiKey { get; set; }
        public string ApiToken { get; set; }
        public string TargetListId { get; set; }
    }
}
