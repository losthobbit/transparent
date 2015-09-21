using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IEmail
    {
        void Send(string subject, string html, string toAddress, string fromAddress = null);
    }
}
