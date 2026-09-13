```csharp
using System;

namespace StaticLearning
{
    public static class MathUtils
    {
        public const double Pi = 3.14159;
        public static readonly string CalculatorName = "SuperMath v1.0";

        public static double CalculateCircleArea(double radius)
        {
            return Pi * radius * radius;
        }

        public static string ToPercentageString(double number)
        {
            return $"{number * 100}%";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Calculator: {MathUtils.CalculatorName}");

            double radius = 5;
            double area = MathUtils.CalculateCircleArea(radius);
            Console.WriteLine($"Circle Area (r={radius}): {area}");

            double ratio = 0.75;
            string percentage = MathUtils.ToPercentageString(ratio);
            Console.WriteLine($"Percentage: {percentage}");
        }
    }
}
```
