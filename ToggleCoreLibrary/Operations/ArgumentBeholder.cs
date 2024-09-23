using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreLibrary.Operations
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ArgumentBeholder : Attribute
    {
        public ArgumentBeholder() { }
    }
}
