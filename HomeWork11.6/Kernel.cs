using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HomeWork11._6
{
    class Kernel
    {
        string json;
        uint parId;      // родительский идентификатор для дочерней организации
        private Random rnd = new Random();
        private ObservableCollection<Organisation> org;
        private ObservableCollection<Organisation> subDepartment;

        /// <summary>
        /// Создать основную организацию
        /// </summary>
        /// <param name="amount">количество корневых отделов</param>
        public ObservableCollection<Organisation> CreateOrg(int amount)
        {
            org = new ObservableCollection<Organisation>();
            org.Add(new Organisation("MainOrg"));
            CreateSubOrgs(amount);
            AddEmpToOrg(1000);
            List<int> deptsWithoutChilds = GetSubDeptsWODescendants();
            List<int> deptsWithChilds = GetDeptsWithDescendants(deptsWithoutChilds);
            GetAdminSalary(deptsWithoutChilds);
            GetAllSalary(deptsWithChilds);
            return org;
        }

        /// <summary>
        /// Создание корневых отделов
        /// </summary>
        /// <param name="amount">Количество подотделов</param>
        private void CreateSubOrgs(int amount)
        {
            ++parId;
            for (int i = 0; i < amount; i++)
            {
                org.Add(new Organisation() { ParId = parId });
            }
            CreateSubDepts(amount);
            parId = 0;
        }

        /// <summary>
        /// Создавать подотделы
        /// </summary>
        /// <param name="amount"></param>
        private void CreateSubDepts(int amount)
        {
            for (int i = 0; i < amount * 4; i++)
            {
                org.Add(new Organisation() { ParId = (uint)rnd.Next(2, amount + 2) });
            }
        }

        /// <summary>
        /// Добавьте сотрудников в отдел и рассчитайте их зарплату (без учета зарплаты администратора)
        /// </summary>
        /// <param name="empAmount">Количество сотрудников</param>
        private void AddEmpToOrg(int empAmount)
        {
            org[0].Employees.Add(new CEO());
            int orgNumb;

            for (int i = 0; i < empAmount; i++)
            {
                switch (rnd.Next(4))
                {
                    case 0:     // В одном подразделении может быть только 1 администратор
                        orgNumb = rnd.Next(1, org.Count);
                        if (!CheckAdminInOrg(org[orgNumb].Employees))
                        {
                            org[orgNumb].Employees.Add(new Administrator());
                            org[orgNumb].AdministratorId = (uint)org[orgNumb].Employees.Count - 1;      // получить идентификатор администратора

                        }
                        break;
                    case 1:
                        orgNumb = rnd.Next(1, org.Count);
                        org[orgNumb].Employees.Add(new Manager());

                        foreach (var emp in org[orgNumb].Employees)     // добавить зарплату менеджера в список зарплат подотдела
                        {
                            if (emp is Manager)
                            {
                                org[orgNumb].SalaryAmount += emp.Salary;
                                break;
                            }

                        }
                        break;
                    case 2:
                        orgNumb = rnd.Next(1, org.Count);
                        org[orgNumb].Employees.Add(new Staff());

                        foreach (var emp in org[orgNumb].Employees)     // добавить зарплату сотрудника в список зарплат подотдела
                        {
                            if (emp is Staff)
                            {
                                org[orgNumb].SalaryAmount += emp.Salary;
                                break;
                            }
                        }
                        break;
                    case 3:
                        orgNumb = rnd.Next(1, org.Count);
                        org[orgNumb].Employees.Add(new Intern());

                        foreach (var emp in org[orgNumb].Employees)     // добавить зарплату стажера в список зарплат подотдела
                        {
                            if (emp is Intern)
                            {
                                org[orgNumb].SalaryAmount += emp.Salary;
                                break;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Рассчитайте зарплату администратора и всего отдела в списке отделов без детей
        /// </summary>
        private void GetAdminSalary(List<int> depNumbs)
        {
            int AdminID;
            int estimatedSalary;

            for (int i = 0; i < depNumbs.Count; i++)             // Получить и установите зарплату администратора
            {
                AdminID = (int)org[depNumbs[i]].AdministratorId;
                estimatedSalary = (int)org[depNumbs[i]].SalaryAmount / 100 * 15;
                if (estimatedSalary > org[depNumbs[i]].Employees[AdminID].Salary)
                {
                    org[depNumbs[i]].Employees[AdminID].Salary = (uint)estimatedSalary;
                }
                org[depNumbs[i]].SalaryAmount = org[depNumbs[i]].Employees[AdminID].Salary + org[depNumbs[i]].SalaryAmount;      // Зарплата всего отдела
                org[depNumbs[i]].SalaryFlag = true;              // Рассчитывается заработная плата текущего отдела
            }
        }

        /// <summary>
        /// Рассчитайте зарплату администратора и всего отдела
        /// </summary>
        /// <param name="currDept">Номер текущего отдела</param>
        /// <param name="subDepts">Список подразделений текущего счета</param>
        private void GetAdminSalary(int currDept, List<int> subDepts)
        {
            int AdminID;
            int estimatedSalary;
            int estimatedSalaryAllSubDepts = 0;

            AdminID = (int)org[currDept].AdministratorId;

            for (int i = 0; i < subDepts.Count; i++)
            {
                estimatedSalaryAllSubDepts += (int)org[subDepts[i]].SalaryAmount;
            }

            estimatedSalaryAllSubDepts += (int)org[currDept].SalaryAmount;         // Добавить текущую зарплату отдела
            estimatedSalary = estimatedSalaryAllSubDepts / 100 * 15;


            if (estimatedSalary > org[currDept].Employees[AdminID].Salary)
            {
                org[currDept].Employees[AdminID].Salary = (uint)estimatedSalary;
            }
            org[currDept].SalaryAmount = org[currDept].Employees[AdminID].Salary + org[currDept].SalaryAmount;      // Зарплата всего отдела
            org[currDept].SalaryFlag = true;              // Рассчитывается заработная плата отдела
        }

        /// <summary>
        /// Рассчитайте заработную плату всех подотделов и отделов
        /// </summary>
        private void GetAllSalary(List<int> depNumbs)       // Долги с дочерними подразделениями
        {
            List<int> subDepts;

            try
            {
                do
                {
                    for (int i = 0; i < depNumbs.Count; i++)
                    {
                        subDepts = GetSubDepts(depNumbs[i]);            // Получить список подотделов выбранного отдела

                        for (int j = 0; j < subDepts.Count; j++)
                        {
                            if (org[subDepts[j]].SalaryFlag)
                            {
                                GetAdminSalary(depNumbs[i], subDepts);
                            }
                        }
                    }
                } while (!CheckSalaryFlag(depNumbs));
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        /// <summary>
        /// Рассчитывается ли заработная плата во всех отделах?
        /// </summary>
        /// <returns></returns>
        private bool CheckSalaryFlag(List<int> deptsId)
        {
            foreach (var depts in deptsId)
            {
                if (!org[depts].SalaryFlag)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Получить номера подотделов без потомков
        /// </summary>
        /// <returns></returns>
        private List<int> GetSubDeptsWODescendants()
        {
            bool inDep = false;
            List<int> DeptsWOChilds = new List<int>();

            for (int i = 0; i <= org.Count; i++)        // Номер отдела
            {
                for (int j = 0; j < org.Count; j++)     // Идентификационный номер родителя
                {
                    if (org[j].ParId == i)
                    {
                        inDep = true;
                        j = org.Count;                  // Условие выхода
                    }
                }

                if (!inDep)
                {
                    DeptsWOChilds.Add(i - 1);        // Добавить подотдел без потомков в список
                }
                inDep = false;
            }
            return DeptsWOChilds;
        }

        /// <summary>
        /// Получить номера отделов с потомками 
        /// </summary>
        /// <returns></returns>
        private List<int> GetDeptsWithDescendants(List<int> deptsWODescendants)
        {
            List<int> deptsWithChilds = new List<int>();

            for (int i = 1; i < org.Count; i++)
            {
                deptsWithChilds.Add(i);
            }

            foreach (int dep in deptsWODescendants)
            {
                deptsWithChilds.Remove(dep);
            }
            return deptsWithChilds;
        }

        /// <summary>
        /// Получить список подотделов выбранного отдела
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns></returns>
        private List<int> GetSubDepts(int dep)
        {
            List<int> subDepts = new List<int>();

            for (int i = 0; i < org.Count; i++)
            {
                if (org[i].ParId - 1 == dep)
                {
                    subDepts.Add((int)org[i].Id - 1);
                }
            }
            return subDepts;
        }

        /// <summary>
        /// Получить список отделов, отсортированных по родительскому идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ObservableCollection<Organisation> GetSubDepts(uint id)
        {
            subDepartment = new ObservableCollection<Organisation>();

            for (int i = 0; i < org.Count; i++)
            {
                if (org[i].ParId == id)
                {
                    subDepartment.Add(org[i]);
                }
            }
            return subDepartment;
        }

        /// <summary>
        /// Проверьте администратора в подотделе
        /// </summary>
        /// <param name="emp">Employees list</param>
        /// <returns></returns>
        private bool CheckAdminInOrg(ObservableCollection<Employee> emp)
        {
            for (int i = 0; i < emp.Count; i++)
            {
                if (emp[i] is Administrator)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Очистить все данные
        /// </summary>
        public void ClearData()
        {
            Employee.count = 0;
            Organisation.count = 0;
            org.Clear();
            subDepartment.Clear();
        }

        /// <summary>
        /// Метод сериализации, сохранение данных в файл
        /// </summary>
        public void SaveData()
        {
            json = JsonConvert.SerializeObject(org);
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "data";
            save.DefaultExt = ".json";
            save.Filter = "JSON file (.json)|*.json";

            if (save.ShowDialog() == true)
            {
                string filename = save.FileName;
                using (StreamWriter sw = new StreamWriter(filename))
                {
                    sw.WriteLine(json);
                }
            }
            MessageBox.Show("Data successfully saved", "Save data", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Загружать сотрудников из файла
        /// </summary>
        public ObservableCollection<Organisation> LoadData()
        {
            OpenFileDialog load = new OpenFileDialog();
            load.FileName = "data";
            load.DefaultExt = ".json";
            load.Filter = "JSON file (.json)|*.json";

            if (load.ShowDialog() == true)
            {
                string filename = load.FileName;

                using (StreamReader sr = new StreamReader(filename))
                {
                    json = sr.ReadToEnd();
                }
                JsonConverter[] converters = { new EmployeeConverter() };
                org = JsonConvert.DeserializeObject<ObservableCollection<Organisation>>(json, new JsonSerializerSettings() { Converters = converters });
            }

            MessageBox.Show("Data successsfully loaded", "Load data", MessageBoxButton.OK, MessageBoxImage.Information);
            return org;
        }

        public class EmployeeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Employee));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                if (jo["Position"].Value<string>() == "CEO")
                    return jo.ToObject<CEO>(serializer);

                if (jo["Position"].Value<string>() == "Administrator")
                    return jo.ToObject<Administrator>(serializer);

                if (jo["Position"].Value<string>() == "Manager")
                    return jo.ToObject<Manager>(serializer);

                if (jo["Position"].Value<string>() == "Staff")
                    return jo.ToObject<Staff>(serializer);

                if (jo["Position"].Value<string>() == "Intern")
                    return jo.ToObject<Intern>(serializer);

                return null;
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

    }
}
