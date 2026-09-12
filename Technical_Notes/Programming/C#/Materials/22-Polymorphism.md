## Virtual Polymorphism
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

## Abstract Polymorphism
```csharp
using System;

public abstract class Employee
{
    public abstract void GetRole();

}

public class Manager : Employee
{
    public override void GetRole()
    {
        Console.WriteLine("Mən Manageram");
    }
}

public class Developer : Employee
{
    public override void GetRole()
    {
        Console.WriteLine("Mən Developerəm");
    }
}

public class Designer : Employee
{
    public override void GetRole()
    {
        Console.WriteLine("Mən Designerəm");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Employee e1 = new Manager();
        Employee e2 = new Developer();
        Employee e3 = new Designer();

        e1.GetRole();
        e2.GetRole();
        e3.GetRole();
    }
}
```
