using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.Model;

public class LogEntry
{
    public string Time { get; set; }
    public DecisionType DecisionType { get; set; }
}
