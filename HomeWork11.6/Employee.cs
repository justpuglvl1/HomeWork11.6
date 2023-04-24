using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork11._6
{
    public abstract class Employee
    {
        static Random rnd = new Random();
        public string Name { get; set; }
        public byte Age { get; set; }
        public byte Projects { get; set; }
        public uint DepId { get; set; }
        public abstract uint Salary { get; set; }
        public uint Id { get; set; }
        public static uint count = 0;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Имя сотрудника</param>
        protected Employee(string name, byte age, byte projects, uint depId)
        {
            Name = name;
            Age = age;
            Projects = projects;
            DepId = depId;
            Id = ++count;
        }
        protected Employee() : this("", 18, 0, 0) { }
        protected Employee(string name) : this(name, (byte)rnd.Next(18, 50), (byte)rnd.Next(1, 10), 0) { }
        protected Employee(string name, uint depId) : this(name, (byte)rnd.Next(18, 50), (byte)rnd.Next(1, 10), depId) { }
    }
    internal class CEO : Employee
    {
        public CEO(string name, byte age, byte projects, uint depId) :
            base(name, age, projects, depId)
        { }
        public CEO() : base("Boss", 50, 0, 0) { }

        public override uint Salary { get; set; } = 50000;      // фиксированная заработная плата
        public string Position { get; } = "CEO";
    }

    internal class Administrator : Employee
    {
        /// <summary>
        /// Создать администратора
        /// </summary>
        /// <param name="name">Имя администратора</param>
        public Administrator(string name, byte age, byte projects, uint depId) :
            base(name, age, projects, depId)
        { }
        public Administrator(uint depId) : base($"Admin_{Guid.NewGuid().ToString().Substring(0, 5)}", depId) { }
        public Administrator() : base($"Admin_{Guid.NewGuid().ToString().Substring(0, 5)}") { }

        public override uint Salary { get; set; } = 7000;     // заработная плата = 15% от заработной платы всех сотрудников во всех подразделениях, но не менее 7000$
        public string Position { get; } = "Administrator";
    }

    internal class Manager : Employee
    {
        /// <summary>
        /// Создать менеджера
        /// </summary>
        /// <param name="name">Имя менеджера</param>
        public Manager(string name, byte age, byte projects, uint depId) :
            base(name, age, projects, depId)
        { }
        public Manager(uint depId) : base($"Admin_{Guid.NewGuid().ToString().Substring(0, 5)}", depId) { }
        public Manager() : base($"Manager_{Guid.NewGuid().ToString().Substring(0, 5)}") { }

        public override uint Salary { get; set; } = 5000;       // фиксированная заработная плата
        public string Position { get; } = "Manager";
    }

    internal class Staff : Employee
    {
        /// <summary>
        /// Создать Персонал
        /// </summary>
        /// <param name="name"></param>
        public Staff(string name, byte age, byte projects, uint depId) :
            base(name, age, projects, depId)
        { }
        public Staff(uint depId) : base($"Admin_{Guid.NewGuid().ToString().Substring(0, 5)}", depId) { }
        public Staff() : base($"Staff_{Guid.NewGuid().ToString().Substring(0, 5)}") { }

        public override uint Salary { get; set; } = 3000;       // фиксированная заработная плата
        public string Position { get; } = "Staff";
    }

    internal class Intern : Employee
    {
        /// <summary>
        /// Создать стажера
        /// </summary>
        /// <param name="name"></param>
        public Intern(string name, byte age, byte projects, uint depId) :
                      base(name, age, projects, depId)
        { }
        public Intern(uint depId) : base($"Admin_{Guid.NewGuid().ToString().Substring(0, 5)}", depId) { }
        public Intern() : base($"Intern_{Guid.NewGuid().ToString().Substring(0, 5)}") { }

        public override uint Salary { get; set; } = 1000;       // фиксированная заработная плата
        public string Position { get; } = "Intern";
    }
}
