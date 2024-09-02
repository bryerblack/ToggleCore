using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToggleCoreTest
{
    public class TestContext
    {
        string Country { get; set; }

        public TestContext(string country)
        {
            Country = country; 
        }
    }
}
