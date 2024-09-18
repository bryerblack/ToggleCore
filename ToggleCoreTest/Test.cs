using ToggleCoreLibrary.Operations;

namespace ToggleCoreTest
{
    public interface ITest
    {
        void TestMethod();
        void TestMethodOn();
        void TestMethodOff();
    }

    public class Test : ITest
    {
        public void TestMethod()
        {
            Console.WriteLine("APPLICATION START. . .");
            TestMethodOn();
            TestMethodOff();
            Console.WriteLine("NOW STARTING SECOND BATCH. . .");
            TestMethodOff();
            TestMethodOn();
        }

        [FeatureToggle("FT0002")]
        public void TestMethodOn()
        {
            Console.WriteLine("enter TesteMethod() - On");
        }

        [FeatureToggle("FT1000")]
        public void TestMethodOff()
        {
            Console.WriteLine("THIS SOULD NOT HAVE HAPPENED");
            throw new Exception();
        }
    }
}