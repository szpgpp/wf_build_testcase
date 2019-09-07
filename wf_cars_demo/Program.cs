using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wf_cars_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //new objects
            var car1 = new CAR("1", true);
            var car2 = new CAR("2");
            var car3 = new CAR("3");
            var car4 = new CAR("4", false, true);
            var car5 = new CAR("5", false, true);
            var car6 = new CAR("6");
            //var car4 = new CAR("2");

            //build relations among them
            car1.AddNextCAR(car2);
            car2.AddNextCAR(car3);
            car2.AddNextCAR(car4);
            car2.AddNextCAR(car5);
            car3.AddNextCAR(car6);
            car6.AddNextCAR(car5);

            //get l_case
            CASE_Builder cb = new CASE_Builder(car1);
            cb.ReBuild();
            var l_case = cb.LCASE;

            //show l_case
            foreach (var c in l_case)
            {
                Console.WriteLine(c.ToPathString());
            }

            //Wait
            Console.ReadKey();
        }
        private static List<CASE> GetResults(CAR car1)
        {
            List<CASE> l_case = new List<CASE>();
            var xcase = new CASE();
            xcase.Add(car1);
            l_case.Add(xcase);

            if (car1.NextCARs != null)
            {
                if (true) //cover all
                {
                    foreach (var car in car1.NextCARs)
                    {
                        if (!xcase.Exists(car))
                        {
                            xcase.Add(car);
                        }
                        else
                        {
                            xcase = new CASE();
                            l_case.Add(xcase);
                            xcase.Add(car1);
                            xcase.Add(car);
                        }
                    }
                }
            }
            return l_case;
        }
    }
    public class CAR
    {
        public string Name;
        public bool IsStart;
        public bool IsEnd;
        public bool IsConCurrent = false;
        public List<CAR> NextCARs = null;
        public CAR(string name, bool isStart = false, bool isEnd = false)
        {
            this.Name = name;
            this.IsStart = isStart;
            this.IsEnd = isEnd;
            this.NextCARs = new List<CAR>();
        }
        public void AddNextCAR(CAR car)
        {
            this.NextCARs.Add(car);
        }
    }
    public class CASE: ICloneable
    {
        public List<CAR> CARS = new List<CAR>();
        public bool IsEnd = false;
        public Guid ID = Guid.NewGuid();
        public CASE() { }
        public CASE(CAR startCAR) { this.Add(startCAR); }
        public void Add(CAR car)
        {
            this.CARS.Add(car);
            if (car.IsEnd) this.IsEnd = true;
        }
        public void Insert(int index, CAR car)
        {
            this.CARS.Insert(index, car);
        }
        public bool Exists(CAR car)
        {
            return this.CARS.Exists(t => t.Name == car.Name);
        }
        public string ToPathString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var car in this.CARS)
            {
                if (sb.Length > 0) sb.Append("/");
                sb.Append(car.Name);
            }
            return sb.ToString();
        }
        public CAR StartCAR
        {
            get
            {
                if (this.CARS.Count == 0) return null;
                return this.CARS[0];
            }
        }
        public CAR EndCAR
        {
            get
            {
                if (this.CARS.Count == 0) return null;
                return this.CARS[this.CARS.Count - 1];
            }
        }
        public bool StartWith(CAR car)
        {
            return this.StartCAR == car;
        }
        public bool EndWith(CAR car)
        {
            return this.EndCAR == car;
        }

        public object Clone()
        {
            var new_case = new CASE();
            new_case.CARS = this.CARS.ToList();
            new_case.IsEnd = this.IsEnd;
            new_case.ID = Guid.NewGuid();
            return new_case;
        }
    }
    public class CASE_Builder
    {
        public CAR StartCAR = null;
        public CASE_Builder(CAR startCAR) { this.StartCAR = startCAR; }
        public int Degree = 0;
        public List<CASE> LCASE = new List<CASE>();
        public bool BuildNext()
        {
            //get end cars
            if (this.LCASE.Count == 0)
            {
                var xcase = new CASE();
                this.LCASE.Add(new CASE(this.StartCAR));
                this.Degree = 1;
                return true;
            }

            var cases = this.LCASE.FindAll(t => !t.IsEnd).ToList();
            if (cases.Count == 0) { cases.Clear(); return false; }
            foreach(var xcase in cases)
            {
                var xcase_clone = xcase.Clone() as CASE;
                var endCase = xcase.EndCAR;
                for (var i = 0; i < endCase.NextCARs.Count; i++)
                {
                    var crn = endCase.NextCARs[i];
                    if (i == 0)
                    {
                        var xcase0 = this.LCASE.Find(t => t.ID == xcase.ID);
                        xcase0.Add(crn);
                    }
                    else
                    {
                        var xcase_clone1 = xcase_clone.Clone() as CASE;
                        xcase_clone1.Add(crn);
                        this.LCASE.Add(xcase_clone1);
                    }
                }

            }
            this.Degree++;
            cases.Clear();
            return true;
        }
        public void ReBuild()
        {
            this.Degree = 0;
            if (this.LCASE != null) this.LCASE.Clear();
            this.LCASE = new List<CASE>();

            while (BuildNext()) ;
        }
    }
}
