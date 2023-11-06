using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace b_term
{
    public partial class MainForm : Form
    {
        [DllImport("ymodem.dll")]
        public static extern int YModem_Start(ref char[] _name, int size);

        [DllImport("ymodem.dll")]
        public static extern int YModem_Request(ref char[] _name, int size);

        const int PACKET_SIZE = 128;
        public unsafe struct YModem_Packet_t
        {
            public byte command;
            public byte counter;
            public byte counterInv;
            public fixed byte data[PACKET_SIZE];
            public UInt16 crc16;
        };

        public MainForm()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateSerialPorts(toolStripComboBox1);
            startToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;

            // Add columns
            listViewLocal.Columns.Add("Name");
            listViewLocal.Columns.Add("Size");
            listViewLocal.Columns.Add("Date");

            // Add columns
            listViewRemote.Columns.Add("Name");
            listViewRemote.Columns.Add("Size");
            listViewRemote.Columns.Add("Date");

            LoadLastPath();
            AutoSizeListViewColumns(listViewLocal);
            AutoSizeListViewColumns(listViewRemote);
        }
        private void AutoSizeListViewColumns(ListView listView)
        {
            foreach (ColumnHeader column in listView.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }
        private void LoadLastPath()
        {
            txtLocalPath.Text = Environment.CurrentDirectory;
        }

        private void PopulateSerialPorts(ToolStripComboBox comboBox)
        {
            comboBox.Items.Clear();
            string[] portNames = SerialPort.GetPortNames();
            comboBox.Items.AddRange(portNames);
            if (comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = true;
                toolStripComboBox1.Enabled = false;
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
                toolStripComboBox1.Enabled = true;
            }
        }
        private void txtLocalPath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtLocalPath.Text))
            {
                AddBackItem(listViewLocal);
                PopulateListView(listViewLocal, txtLocalPath.Text);
            }
        }

        private void AddBackItem(ListView listView)
        {
            listView.Items.Clear(); // Clear the ListView

            ListViewItem item = new ListViewItem("...");
            item.SubItems.Add("<<<"); // Type
            listView.Items.Add(item);
        }

        private void PopulateListView(ListView listView, string directoryPath)
        {
            //listView.Items.Clear();
            var dirInfo = new DirectoryInfo(directoryPath);

            foreach (var subDir in dirInfo.GetDirectories())
            {
                var item = new ListViewItem(subDir.Name);
                item.SubItems.Add("<dir>");
                item.SubItems.Add(subDir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                listView.Items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new ListViewItem(file.Name);
                item.SubItems.Add(file.Length.ToString());
                item.SubItems.Add(file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                listView.Items.Add(item);
            }
        }
        private void ListView_DoubleClick(object sender, EventArgs e, TextBox pathTextBox, ListView listView)
        {
            var selectedListItem = listView.SelectedItems[0];
            var sizeValue = selectedListItem.SubItems[1].Text;

            if (sizeValue == "<dir>")
            {
                pathTextBox.Text = Path.Combine(pathTextBox.Text, selectedListItem.Text);
            }
            else if (sizeValue == "<<<")
            {
                pathTextBox.Text = GetParentPath(pathTextBox.Text);
            }
        }
        private void listViewLocal_DoubleClick(object sender, EventArgs e)
        {
            ListView_DoubleClick(sender, e, txtLocalPath, listViewLocal);
        }

        private void listViewRemote_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(listViewLocal.SelectedItems[0].ToString());
        }
        static string GetParentPath(string currentPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);

            // Get the parent directory (move one level up in the directory structure)
            DirectoryInfo parentDirectory = directoryInfo.Parent;

            if (parentDirectory != null)
            {
                return parentDirectory.FullName;
            }

            return null;
        }

        private void btnRefreshLocal_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtLocalPath.Text))
            {
                AddBackItem(listViewLocal);
                PopulateListView(listViewLocal, txtLocalPath.Text);
            }
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            var filePath = Path.Combine("\\", listViewRemote.SelectedItems[0].SubItems[0].Text);
            var filePathArray = filePath.ToCharArray();
            MessageBox.Show(filePath);
            Console.Write(YModem_Request(ref filePathArray, filePathArray.Length));
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine("\\", listViewLocal.SelectedItems[0].SubItems[0].Text);
            char[] filePathArray = filePath.ToCharArray();
            progressBar1.Maximum = (int.Parse(listViewLocal.SelectedItems[0].SubItems[1].Text) / PACKET_SIZE)+1;
            MessageBox.Show(filePath + progressBar1.Maximum);
            MessageBox.Show(YModem_Request(ref filePathArray, PACKET_SIZE).ToString());
        }
        private void ListView_SelectedIndexChanged(object sender, EventArgs e, Button button, ListView listView)
        {
            button.Enabled = false;

            if (listView.SelectedItems.Count == 1)
            {
                ListViewItem selectedListItem = listView.SelectedItems[0]; // Get the selected item
                string sizeValue = selectedListItem.SubItems[1].Text; // Index 1 corresponds to the "Size" column

                if (sizeValue != "<dir>" && sizeValue != "<<<")
                {
                    button.Enabled = true;
                }
            }
        }
        private void listViewRemote_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView_SelectedIndexChanged(sender, e, btnReceive, listViewRemote);
        }
        private void listViewLocal_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView_SelectedIndexChanged(sender, e, btnSend, listViewLocal);
        }

        private void txtRemotePath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtRemotePath.Text))
            {
                AddBackItem(listViewRemote);
                PopulateListView(listViewRemote, txtRemotePath.Text);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = toolStripComboBox1.Text;
            txtRemotePath.Text = $"\\\\{serialPort1.PortName}";
        }
    }
}
