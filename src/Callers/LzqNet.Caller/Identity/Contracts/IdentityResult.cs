using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzqNet.Caller.Identity.Contracts;
public class IdentityResult<T>
{
    public int Code { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}
