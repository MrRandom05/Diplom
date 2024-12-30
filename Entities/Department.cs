namespace Diplom
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public static Department Of(string name)
        {
            return new Department() {DepartmentName = name};
        }
    }
}