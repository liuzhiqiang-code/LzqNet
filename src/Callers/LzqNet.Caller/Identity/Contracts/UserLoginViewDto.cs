using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Identity.Contracts;
public class UserLoginViewDto
{
    public string Token { get; set; }
    public string UserName { get; set; }
    public DateTime ExpiresIn { get; set; }
}
