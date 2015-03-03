using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blueprint.Aspnet.Host
{
    public interface IResponseWrapper
    {
        void End();

        void Clear();

        int StatusCode { get; set; }

        object AppendHeader(string p1, string p2);

        void Write(string p);
    }
}
