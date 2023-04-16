using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Class1
    {
    }
    public abstract class Vehicle
    {
        public float V { get; set; }
        public Vehicle(float v)
        {
            V = v;
        }
        public virtual void Run(float d)
        {
            Console.WriteLine("it takes {0} to run {1} kilometers", d / V, d);
        }
    }
    public class Car : Vehicle
    {
        int passengers;
        public Car(int passengers, int speed) : base(speed)
        {
            this.passengers = passengers;
            this.V = speed;
        }

    }
    public class Truck : Vehicle
    {
        int load;
        public Truck(int load, int speed) : base(speed)
        {
            this.load = load;
            base.V = speed;
        }
        public  void Run(float distance,string number)
        {
            Console.WriteLine("it takes {0} to run {1} kilometers{2}", (1 + load / 100) * distance / V, distance,number);

        }
    }
    public class Student
    {
        string id, name;
        bool gender;
        private int grade;
        private int _class;

        public string ID { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public bool Gender { get => gender; set => gender = value; }
        public int Class { get => _class; set => _class = value; }
        public int Grade { get => grade; set => grade = value; }

        public Student(string id,string name,bool gender,int _class=1,int grade = 1)
        {
            this.ID = id;
            this.Name = name;
            this.Gender = gender;   
            this.Class= _class;
            this.Grade = grade;
        }
        public string ToString(string myString) {
            return ID + Name;
            }
    }
    public class Undergraduate:Student
    {
        public string department { get; set; }
         public int Grade 
        {   get
            {
                return base.Grade;
            }
            set
            {
                if (Grade < 0 | Grade > 3)
                {
                    throw new Exception("the Grade is not right");
                }
                else
                {
                    base.Grade = Grade;
                }

            } 
        }
          
        public Undergraduate(string id,string name,bool gender,int _class=1,int grade = 1):base(id,name,gender,_class,grade)
        {
            this.department = department;

        }

    }
    public class Graduate : Student
    {
        public string department;
        public string Tutor { get; set; }
        public int Grade 
        {   get
            {
                return base.Grade;
            }
            set
            {
                if (Grade < 0 | Grade > 4)
                {
                    throw new Exception("the Grade is not right");
                }
                else
                {
                    base.Grade = Grade;
                }

            } 
        }
        public Graduate(string id, string name, bool gender, int _class = 1, int grade = 1) : base(id, name, gender, _class , grade )
        {
            base.ID = id;
            base.Name = name;

        }
          

    }
}
