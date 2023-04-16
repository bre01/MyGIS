using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*MyPorperty myPorperty = new MyPorperty();
            Console.WriteLine(myPorperty.name);
            myPorperty.name= "Test";
            //Console.WriteLine(myPorperty._name);
            Console.WriteLine(myPorperty.name);
            LC myLc = new LC(3);
            BC myBc = new BC(3, 10);*/
            Vehicle myCar = new Car(5, 80);
            myCar.Run(800);
            Truck myTruck = new Truck(100, 80);
            myTruck.Run(800,"test");
            
        }

    }
    public class LC
    {
        public int number;
        public LC(int number)
        {
            this.number = number;
        }
    }
    public class BC : LC
    {
        public int number;

        public BC(int number1, int number2) : base(number2)
        {
            this.number = number1;
            base.number = number2;
        }

    } 
    public class MyPorperty
    {
        private string _name="hello";
        public string name 
        { 
            set { _name = value;} 
            get { return _name;}
        }
    }
}
