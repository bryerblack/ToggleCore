// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using ToggleCoreLibrary.Contexts;
using ToggleCoreLibrary.Operations;

namespace ToggleCoreTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FeatureToggle.SetFeatureToggleMapper(new FeatureToggleDbMapper());
            var test = new Test();
            test.TestMethod();
            Console.WriteLine("APPLICATION ENDED. . .");
        }
    }
}