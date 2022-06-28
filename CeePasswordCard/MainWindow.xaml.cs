using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace CeePasswordCard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[][] data;
        private static string dataFile;
        static MainWindow()
        {
            dataFile = Path.Combine(Path.GetDirectoryName(typeof(MainWindow).Assembly.Location), "data.txt");
            if (!File.Exists(dataFile))
            {
                dataFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "data.txt");
            }
            if (!File.Exists(dataFile))
            {
                dataFile = null;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            string content = "";
            if (dataFile != null)
            {
                content = File.ReadAllText(dataFile);
            }
            data = content
                .Split(new char[] { '\n', '\r' })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(line => line.Split(new char[] { '\t', ' ', '　', ',', ';', ':', '，', '；', '：' }).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray())
                .ToArray();
            content = string.Join(Environment.NewLine, data.Select(line => string.Join("  ", line)));
            ContentTextBox.Text = content;
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            string posStr = new(PosTextBox.Text.Where(x => !"\r\n\t 　,;:，；：".Contains(x)).ToArray());
            posStr = posStr.ToUpperInvariant();
            if (posStr.Length <= 0 || posStr.Length % 2 != 0)
            {
                _ = MessageBox.Show(this, "您输入的坐标无限哦，先检查一下再试试吧", "查询失败啦", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            for (int i = 0; i < posStr.Length; i += 2)
            {
                int x = posStr[i] - 'A';
                int y = posStr[i + 1] - '1';
                if (x < 0 || y < 0 || x > data.Length || y > data[x].Length)
                {
                    result += "<invaild>";
                }
                else
                    result += data[x][y];
            }
            if (MessageBox.Show(this, $"查询成功啦，结果是 {result}{Environment.NewLine}要帮您复制到剪辑版吗？", "查询成功！", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Clipboard.SetText(result);
            }
        }
    }
}
