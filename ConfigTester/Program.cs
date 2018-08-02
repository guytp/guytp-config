using System;
using Guytp.Config;

namespace ConfigTester
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfig config = AppConfig.ApplicationInstance;
            Console.WriteLine(config.GetAppSetting<string>("Test.Abc"));
            Console.WriteLine(config.GetAppSetting<double>("DoubleSetting"));
            Console.WriteLine(config.GetConnectionString("Default"));
            ComplexSetting complexSetting = config.GetAppSetting<ComplexSetting>("ComplexSetting");
            Console.WriteLine(complexSetting.Setting1);
            Console.WriteLine(complexSetting.Setting2);
            TestObject testObject = config.GetObject<TestObject>("TestObject");
            Console.WriteLine(testObject.SomeValue);
            Console.WriteLine(testObject.SubObject.Int);
            Console.WriteLine(testObject.SubObject.Dec);
            Console.WriteLine(testObject.SubObject.Str);


            TestObject missingObject = config.GetObject<TestObject>("MissingObject");

            Console.ReadKey();

        }
    }
}
