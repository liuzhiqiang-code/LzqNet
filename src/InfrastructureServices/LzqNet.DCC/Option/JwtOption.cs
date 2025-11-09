using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.DCC.Option;
public class JwtOption
{
    public string Audience { get; set; }
    public string Authority { get; set; }
    public bool RequireHttpsMetadata { get; set; }
}
