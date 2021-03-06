﻿using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.Models.Skud;
using FiresecService.SKUD;

namespace FiresecService.Service
{
    public partial class FiresecService : IFiresecService
    {
        private FiresecServiceSKUD _skud = new FiresecServiceSKUD();

        #region IFiresecService Members
        public IEnumerable<EmployeeCard> GetAllEmployees(EmployeeCardIndexFilter filter)
        {
            return _skud.GetAllEmployees(filter);
        }

        public bool DeleteEmployee(int id)
        {
            return _skud.DeleteEmployee(id);
        }

        public EmployeeCardDetails GetEmployeeCard(int id)
        {
            return _skud.GetEmployeeCard(id);
        }

        public int SaveEmployeeCard(EmployeeCardDetails employeeCard)
        {
            return _skud.SaveEmployeeCard(employeeCard);
        }

        public IEnumerable<EmployeeDepartment> GetEmployeeDepartments()
        {
            return _skud.GetEmployeeDepartments();
        }

        public IEnumerable<EmployeeGroup> GetEmployeeGroups()
        {
            return _skud.GetEmployeeGroups();
        }

        public IEnumerable<EmployeePosition> GetEmployeePositions()
        {
            return _skud.GetEmployeePositions();
        }

        public IEnumerable<Employee> GetEmployees(EmployeeFilter filter)
        {
            return _skud.GetEmployees(filter);
        }
        public IEnumerable<Department> GetDepartments(DepartmentFilter filter)
        {
            return _skud.GetDepartments(filter);
        }
        public IEnumerable<Position> GetPositions(PositionFilter filter)
        {
            return _skud.GetPositions(filter);
        }
        #endregion
    }
}