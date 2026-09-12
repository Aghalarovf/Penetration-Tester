```csharp
using System;

public class Animal
{
    public virtual void MakeSound()
    {
        Console.WriteLine("Animals make a sound");
    }
}

public class Dog : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Hav Hav");
    }
}

public class Cat : Animal
{
    public override void MakeSound()
    {
        Console.WriteLine("Miau blet");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Animal a1 = new Dog();
        Animal a2 = new Cat();

        a1.MakeSound();
        a2.MakeSound();
    }
}
```
