using System.Data.SqlClient;
using System.Windows.Controls;

namespace LightningMarks
{
    internal class Manager
    {
        public static Frame MainFrame { get; set; }
        public static SqlConnection connection { get; set; }
        public static string myrole { get; set; }
        public static int my_id { get; set; }
    }
}
