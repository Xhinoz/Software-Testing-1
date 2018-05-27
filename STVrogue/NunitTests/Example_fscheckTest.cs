using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FsCheck;

/* This file contains some examples of using the automated testing tool
 * FSCheck (sometimes I also simply call it QuickCheck, because it is where
 * it was derived from).
 */
namespace SomeNameClass
{
    /* An example of a class to test */
    public class Thermometer
    {
        private float temperature;

        public Thermometer(float t)
        {
            // only alow non-neg initial tempereture
            temperature = Math.Abs(t);
        }

        public void increase(float t)
        {
            float t_ = temperature + t;
            if (t_ < 273.15) throw new ArgumentException();
            temperature = t_;
        }

        public float get() { return temperature; }
    }

    [TestFixture(Description = "containing few examples of automated tests using fscheck")]
    class Example_fscheckTest
    {
        /*// QuickCheck writes its report to the Console. To see what it reporteed,
        // click at the "Output" link in your unit test tab.
        [Test(Description = "test the constructor")]
        public void NTest_Thermometer_constructor()
        {
            // QuickCheck will generate random values for t, within the range
            // specified by the When-clause. For every t, the body of the lamda
            // expression specifies your test. The test passes if the body returns
            // true. QuickCheck checks whether this is the case for every t it
            // generates.
            // Do note that a plain random algorithm will have to be very lucky if your
            // When-clause is complicated enough. 
            Prop.ForAll<float>(
                // note that this is just a lambda-expression
                t =>
                {
                    // create a new thermometer(t)
                    Thermometer trm = new Thermometer(t);
                    // check that the thermometer is configure with temperature |t|
                    return (Math.Abs(t) == trm.get())
                    .
                        // Use a When-clause to specify a sensical range (pre-condition) of valid t values
                    When(-1000 <= t && t <= 1000)
                    .
                        // Use classify-clauses to get statistics of the distribution of your tests
                    Classify(t > 0, "positive").
                    Classify(t == 0.0f, "zero").
                    Classify(t < 0, "negative");
                }
             )
             .QuickCheckThrowOnFailure();
        }

        [Test(Description = "another example: a test on decreasing the temperature below 0 kelvin, the thermometer should throw an exception")]
        public void NTest_Thermometer_increase_exception()
        {
            Prop.ForAll<float>(
                t =>
                {
                    Thermometer trm = new Thermometer(0);
                    Boolean ok = false;
                    try { trm.increase(t); }
                    catch (ArgumentException e) { ok = true; }
                    return ok
                    .
                    When(-1000 <= t && t < 273.15);
                }
             )
             .QuickCheckThrowOnFailure();
        }

        [Test(Description = "demonstrating how to deploy custom value generators")]
        public void NTest_Thermometer_demonstrating_customGenerator()
        {
            // Register the class that contains your custom value-generators
            Arb.Register<MyCustomGenerators>();
            Prop.ForAll<float>(
                t =>
                {
                    Thermometer trm = new Thermometer(t);
                    Console.WriteLine("** t = " + t);
                    return (Math.Abs(t) == trm.get());
                }
             )
             .QuickCheckThrowOnFailure();
        }*/

        /* A class containing your value generators, in this case just one. */
        class MyCustomGenerators
        {
            /* A value generator is represented by a static method that returns an instance of Arbitrary. 
             * You can have multiple such generators. */
            public static Arbitrary<float> customTemperatureGen()
            {
                /* The actual generator is an instance of Gen, here is an example: */
                Gen<float> temperatureGenerator = Gen.OneOf<float>(
                           Gen.Constant(-273.15f),
                           Gen.Constant(-1.0f),
                           Gen.Constant(0f),
                           Gen.Constant(1.0f));
                /* Then we lift it to become an Arbitrary, and return it: */
                return temperatureGenerator.ToArbitrary<float>();
            }
        }
    }
}
