using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MyTC
{
    public partial class Form1 : Form
    {

        List<string> Adresses = new List<string>();
        int k;
        int currIndex = -1;
        string currListViewAdress = "";
        string strCopy;
        ToolStripMenuItem copyMenuItem;
        ToolStripMenuItem pasteMenuItem;
        ToolStripMenuItem RenameMenuItem;
        ToolStripMenuItem DeleteMenuItem;
        ToolStripMenuItem ViewMenuItem;

        ToolStripMenuItem СutMenuItem;
        ToolStripMenuItem Creat;
        ToolStripMenuItem[] items;
        string oldName;
        int pos;
        bool cut;
        public Form1()
        {
            InitializeComponent();
            cut = false;
            listView1.ColumnClick += new ColumnClickEventHandler(ClickOnColumn);
            copyMenuItem = new ToolStripMenuItem("Копировать");
            pasteMenuItem = new ToolStripMenuItem("Вставить");
            RenameMenuItem = new ToolStripMenuItem("Переименовать");
            DeleteMenuItem = new ToolStripMenuItem("Удалить");
            СutMenuItem = new ToolStripMenuItem("Вырезать");


            Creat = new ToolStripMenuItem("Создать");
            ToolStripMenuItem CreateNewFolder=new ToolStripMenuItem("Папку");

            ToolStripMenuItem CreateNewTxtFile = new ToolStripMenuItem("Текстовый документ");
            CreateNewTxtFile.Click += CreateNewTxtFile_Click;
            CreateNewFolder.Click += CreateNewFolder_Click;
            Creat.DropDownItems.AddRange(new[] { CreateNewFolder, CreateNewTxtFile });



            ViewMenuItem = new ToolStripMenuItem("Вид");

            items = new ToolStripMenuItem[] { TableToolStripMenuItem,
            ListToolStripMenuItem, TilesToolStripMenuItem, ListImgToolStripMenuItem, ListIconToolStripMenuItem };

            ViewMenuItem.DropDownItems.AddRange(items);


            copyMenuItem.Enabled = false;
            RenameMenuItem.Enabled = false;
            DeleteMenuItem.Enabled = false;
            СutMenuItem.Enabled = false;


            contextMenuStrip1.Items.AddRange(new[] { ViewMenuItem, copyMenuItem, pasteMenuItem, RenameMenuItem, DeleteMenuItem, СutMenuItem,Creat });
            copyMenuItem.Click += CopyMenuItem_Click;
            pasteMenuItem.Click += PasteMenuItem_Click;
            RenameMenuItem.Click += RenameMenuItem_Click;
            DeleteMenuItem.Click += DeleteMenuItem_Click;
            СutMenuItem.Click += СutMenuItem_Click;
            pasteMenuItem.Enabled = false;
            ColumnHeader c = new ColumnHeader();
            c.Text = "Имя";
            c.Width = c.Width + 80;
            ColumnHeader c2 = new ColumnHeader();
            c2.Text = "Размер";
            c2.Width = c2.Width + 60;
            ColumnHeader c3 = new ColumnHeader();
            c3.Text = "Тип";
            c3.Width = c3.Width + 60;
            ColumnHeader c4 = new ColumnHeader();
            c4.Text = "Изменен";
            c4.Width = c4.Width + 60;
            listView1.Columns.Add(c);
            listView1.Columns.Add(c2);
            listView1.Columns.Add(c3);
            listView1.Columns.Add(c4);

            string[] str = Directory.GetLogicalDrives();
            int n = 1;
            foreach (string s in str)
            {
                try
                {
                    TreeNode tn = new TreeNode();
                    tn.Name = s;
                    tn.Text = "Локальный диск " + s;
                    treeView1.Nodes.Add(tn.Name, tn.Text, 2);
                    FileInfo f = new FileInfo(@s);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@s);
                    foreach (string s2 in str2)
                    {
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        treeView1.Nodes[n - 1].Nodes.Add(s2, t, 0);
                    }
                }
                catch { }
                n++;
            }
            foreach (TreeNode tn in treeView1.Nodes)
            {
                for (int i = 65; i < 91; i++)
                {
                    char sym = Convert.ToChar(i);
                    if (tn.Name == sym + ":\\")
                        tn.SelectedImageIndex = 2;
                }
            }
        }

        private void CreateNewTxtFile_Click(object sender, EventArgs e)
        {
            k = 1;
            while (true)
            {
                try
                {

                    if (!File.Exists(currListViewAdress + "\\" + k.ToString()+".txt"))
                    {
                        File.Create(currListViewAdress + "\\"  + k.ToString() + ".txt");
                        break;
                    }
                    k++;
                }
                catch
                {


                }
            }
            LoadListView();
        }

        private void CreateNewFolder_Click(object sender, EventArgs e)
        {
          k = 1;
            while (true)
            {
                try
                {
                    if (!Directory.Exists(currListViewAdress + "\\" + "Новая папка " + k.ToString()))
                    {
                        Directory.CreateDirectory(currListViewAdress+"\\" + "Новая папка " + k.ToString());
                        break;
                    }
                    k++;
                }
                catch
                {
                  

                }
            }
            LoadListView();
        }

        private void СutMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                strCopy = @listView1.SelectedItems[0].Name;
                pasteMenuItem.Enabled = true;
                cut = true;
            }
        }

        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(@listView1.SelectedItems[0].Name);
                FileAttributes attr = File.GetAttributes(@listView1.SelectedItems[0].Name);
                string path = currListViewAdress + "\\";
                if (DialogResult.Yes == MessageBox.Show("Удалить " + listView1.SelectedItems[0].Text + "?", "Удаление", MessageBoxButtons.YesNo))

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        DeleteDirectory(@listView1.SelectedItems[0].Name);
                    }
                    else
                    {

                        File.Delete(@listView1.SelectedItems[0].Name);
                    }


                LoadListView();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DeleteDirectory(string path)
        {
           
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }

                try
                {
                    Directory.Delete(path, true);
                }
                catch (IOException)
                {
                    Directory.Delete(path, true);
                }
                catch (UnauthorizedAccessException)
                {
                    Directory.Delete(path, true);
                }
        }
         
        


        private void RenameMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    oldName = @listView1.SelectedItems[0].Name;
                    listView1.SelectedItems[0].BeginEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void PasteMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                string name;
                name = strCopy.Substring(strCopy.LastIndexOf('\\') + 1);
                if (cut == false)
                {
                    if (listView1.SelectedItems.Count > 0)
                    {
                        File.Copy(strCopy, @listView1.SelectedItems[0].Name + "\\" + name);

                    }
                    else
                        File.Copy(strCopy, @toolStripTextBox1.Text + "\\" + name);
                }
                else
                {
                    if (listView1.SelectedItems.Count > 0)
                    {
                        File.Move(strCopy, @listView1.SelectedItems[0].Name + "\\" + name);

                    }
                    else
                        File.Move(strCopy, @toolStripTextBox1.Text + "\\" + name);
                }
                    LoadListView();
                pasteMenuItem.Enabled = false;
                listView1_SelectedIndexChanged( sender,  e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count > 0)
            {
                strCopy = @listView1.SelectedItems[0].Name;
                pasteMenuItem.Enabled = true;
                cut = false;
            }

        }

        private void ClickOnColumn(object sender, ColumnClickEventArgs e)
        {
            // clicking on the name column
            if (e.Column == 0)
            {
                if (listView1.Sorting == SortOrder.Descending)
                    listView1.Sorting = SortOrder.Ascending;
                else
                    listView1.Sorting = SortOrder.Descending;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string strtmp = "";
            if (Adresses.Count != 0)
            {
                strtmp = Adresses[Adresses.Count - 1];
                Adresses.Clear();
                Adresses.Add(strtmp);
                currIndex = 0;
            }
            Adresses.Add(e.Node.Name);
            currIndex++;

            // checking the ability to go backward / forward
            if (currIndex + 1 == Adresses.Count)
                toolStripButton2.Enabled = false;
            else
                toolStripButton2.Enabled = true;
            if (currIndex - 1 == -1)
                toolStripButton1.Enabled = false;
            else
                toolStripButton1.Enabled = true;
            listView1.Items.Clear();
            currListViewAdress = e.Node.Name;
            toolStripTextBox1.Text = currListViewAdress;


            //Fill ListView
            try
            {
                if (listView1.View != View.Tile)
                {
                    FileInfo f = new FileInfo(@e.Node.Name);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@e.Node.Name);
                    ListViewItem lw = new ListViewItem();
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        string type = "Папка";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    str2 = Directory.GetFiles(@e.Node.Name);
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        string type = "Файл";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
                else
                {
                    FileInfo f = new FileInfo(@e.Node.Name);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@e.Node.Name);
                    ListViewItem lw = new ListViewItem();
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    str2 = Directory.GetFiles(@e.Node.Name);
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
            }
            catch { }

        }

        private void списокІконокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos = 4;
            Check(items, pos);

            listView1.View = View.SmallIcon;

        }

        private void списокЗображеньToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos = 3;
            Check(items, pos);

            listView1.View = View.LargeIcon;

        }

        private void плиткиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos = 2;
            Check(items, pos);

            listView1.View = View.Tile;
            LoadListView();

            
        }

        private void списокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos = 1;
            Check(items, pos);

            listView1.View = View.List;

        }

        private void таблицяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pos = 0;
            Check(items, pos);
            listView1.View = View.Details;
            listView1.Items.Clear();
            FileInfo f = new FileInfo(@currListViewAdress);
            string t = "";
            string[] str2 = Directory.GetDirectories(@currListViewAdress);
            ListViewItem lw = new ListViewItem();
            foreach (string s2 in str2)
            {
                f = new FileInfo(@s2);
                string type = "Папка";
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                lw.Name = s2;
                listView1.Items.Add(lw);
            }
            str2 = Directory.GetFiles(@currListViewAdress);
            foreach (string s2 in str2)
            {
                f = new FileInfo(@s2);
                string type = "Файл";
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                lw.Name = s2;
                listView1.Items.Add(lw);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                // double click on a folder or file in the listview
                if (listView1.SelectedItems[0].Text.IndexOf('.') == -1)
                {
                    // clicking on the folder
                    Adresses.Add(listView1.SelectedItems[0].Name);
                    currIndex++;
                    currListViewAdress = Adresses[currIndex];
                    if (currIndex + 1 == Adresses.Count)
                        toolStripButton2.Enabled = false;
                    else
                        toolStripButton2.Enabled = true;
                    if (currIndex - 1 == -1)
                        toolStripButton1.Enabled = false;
                    else
                        toolStripButton1.Enabled = true;
                    currListViewAdress = listView1.SelectedItems[0].Name;
                    toolStripTextBox1.Text = currListViewAdress;
                    FileInfo f = new FileInfo(@listView1.SelectedItems[0].Name);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@listView1.SelectedItems[0].Name);
                    string[] str3 = Directory.GetFiles(@listView1.SelectedItems[0].Name);
                    listView1.Items.Clear();
                    ListViewItem lw = new ListViewItem();
                    if (listView1.View == View.Details)
                    {
                        foreach (string s2 in str2)
                        {
                            f = new FileInfo(@s2);
                            string type = "Папка";
                            t = s2.Substring(s2.LastIndexOf('\\') + 1);
                            lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                            lw.Name = s2;
                            listView1.Items.Add(lw);
                        }
                        foreach (string s2 in str3)
                        {
                            f = new FileInfo(@s2);
                            string type = "Файл";
                            t = s2.Substring(s2.LastIndexOf('\\') + 1);
                            lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                            lw.Name = s2;
                            listView1.Items.Add(lw);
                        }
                    }
                    else
                    {
                        foreach (string s2 in str2)
                        {
                            f = new FileInfo(@s2);
                            t = s2.Substring(s2.LastIndexOf('\\') + 1);
                            lw = new ListViewItem(new string[] { t }, 0);
                            lw.Name = s2;
                            listView1.Items.Add(lw);
                        }
                        foreach (string s2 in str3)
                        {
                            f = new FileInfo(@s2);
                            t = s2.Substring(s2.LastIndexOf('\\') + 1);
                            lw = new ListViewItem(new string[] { t }, 1);
                            lw.Name = s2;
                            listView1.Items.Add(lw);
                        }
                    }
                }
                else
                {
                    // click on the file
               

                    System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                    MyProc.StartInfo.FileName = @listView1.SelectedItems[0].Name;
                    MyProc.Start();
                }
            }
            catch(Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

       

        private void обновитиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadListView();

        }

        private void закритьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            int i = 0;
            // filling of the child nodes with the child nodes of the deployed bridge      

            try
            {

                foreach (TreeNode tn in e.Node.Nodes)
                {
                    string[] str2 = Directory.GetDirectories(@tn.Name);
                    foreach (string str in str2)
                    {
                        TreeNode temp = new TreeNode();
                        temp.Name = str;
                        temp.Text = str.Substring(str.LastIndexOf('\\') + 1);
                        e.Node.Nodes[i].Nodes.Add(temp);
                    }
                    i++;
                }
            }
            catch { }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //Back
            try
            {
                if (currIndex - 1 != -1)
                {
                    currIndex--;
                    currListViewAdress = ((string)Adresses[currIndex]);
                    if (currIndex + 1 == Adresses.Count)
                        toolStripButton2.Enabled = false;
                    else
                        toolStripButton2.Enabled = true;
                    if (currIndex - 1 == -1)
                        toolStripButton1.Enabled = false;
                    else
                        toolStripButton1.Enabled = true;
                    LoadListView();

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            //Next
            try
            {
                if (currIndex + 1 != Adresses.Count)
                {
                    currIndex++;
                    currListViewAdress = ((string)Adresses[currIndex]);
                    if (currIndex + 1 == Adresses.Count)
                        toolStripButton2.Enabled = false;
                    else
                        toolStripButton2.Enabled = true;
                    if (currIndex - 1 == -1)
                        toolStripButton1.Enabled = false;
                    else
                        toolStripButton1.Enabled = true;

                    LoadListView();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                try
                {
                    string[] str2 = Directory.GetDirectories(@toolStripTextBox1.Text);
                    string[] str3 = Directory.GetFiles(@toolStripTextBox1.Text);
                    currIndex++;
                    currListViewAdress = toolStripTextBox1.Text;
                    Adresses.Add(toolStripTextBox1.Text);
                    if (currIndex + 1 == Adresses.Count)
                        toolStripButton2.Enabled = false;
                    else
                        toolStripButton2.Enabled = true;
                    if (currIndex - 1 == -1)
                        toolStripButton1.Enabled = false;
                    else
                        toolStripButton1.Enabled = true;
                    LoadListView();
                }
                catch
                {
                    toolStripTextBox1.Text = currListViewAdress;
                }
            }
        }


        public void LoadListView()
        {
            try
            {
                toolStripTextBox1.Text = currListViewAdress;
                FileInfo f = new FileInfo(@currListViewAdress);
                string t = "";
                string[] str2 = Directory.GetDirectories(@currListViewAdress);
                string[] str3 = Directory.GetFiles(@currListViewAdress);
                listView1.Items.Clear();
                ListViewItem lw = new ListViewItem();
                if (listView1.View == View.Details)
                {
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        string type = "Папка";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    foreach (string s2 in str3)
                    {
                        f = new FileInfo(@s2);
                        string type = "Файл";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
                else
                {
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    foreach (string s2 in str3)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);

                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(@oldName);
                string path = currListViewAdress + "\\";

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    Directory.Move(@oldName, @path + e.Label.ToString());
                }
                else
                {
                    string n = oldName.Substring(oldName.LastIndexOf('.') + 1);
                    File.Move(@oldName, @path + e.Label.ToString() + '.' + n);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    RenameMenuItem.Enabled = true;
                    copyMenuItem.Enabled = true;
                    DeleteMenuItem.Enabled = true;
                    СutMenuItem.Enabled = true;
                }
                else
                {
                    copyMenuItem.Enabled = false;
                    RenameMenuItem.Enabled = false;
                    DeleteMenuItem.Enabled = false;
                    СutMenuItem.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void видToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewToolStripMenuItem.DropDownItems.Clear();
            ViewToolStripMenuItem.DropDownItems.AddRange(items);

            ViewToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            ViewToolStripMenuItem.DropDownItems.Add(ReloadToolStripMenuItem);

        }

        private void contextMenuStrip1_BeginDrag(object sender, EventArgs e)
        {
           
            ViewMenuItem.DropDownItems.AddRange(items);

        }

   

     

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                LoadListView();

            }
        }

        void Check(ToolStripMenuItem [] it,int i)
        {
            foreach (var item in it)
            {
                if(item.Checked==true)
                {
                    item.Checked = false;
                }

            }
            it[i].Checked = true;
        }
    
    }
}
