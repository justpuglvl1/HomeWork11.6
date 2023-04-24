using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork11._6
{
    public class Organisation
    {
        public string Title { get; set; }
        public uint Id { get; set; }
        public uint ParId { get; set; }
        public uint AdministratorId { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public static uint count = 0;

        public bool SalaryFlag { get; set; }        // верно, если зарплата учитывается в текущем отделе
        public uint SalaryAmount { get; set; }      // размер заработной платы всех сотрудников текущего отдела

        public Organisation(string title)
        {
            Title = title;
            Id = ++count;
            Employees = new ObservableCollection<Employee>();
        }

        public Organisation() : this($"dep{count}") { }
    }
}
