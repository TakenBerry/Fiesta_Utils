using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Zepheus.FiestaLib.SHN;

namespace ShineTableParser
{
    public partial class frmMain : Form
    {
        const string Comment = ";";
        const string StructInteger = "<integer>";
        const string StructString = "<string>";

        const string Replace = "#exchange";
        const string Ignore = "#ignore";
        const string StartDefine = "#define";
        const string EndDefine = "#enddefine";
        const string Table = "#table";
        const string ColumnName = "#columnname";
        const string ColumnType = "#columntype";
        const string Record = "#record";
        const string RecordLine = "#recordin"; // Contains tablename as first row.

        public frmMain()
        {
            InitializeComponent();
        }

        private Dictionary<string, ShineTable> ParseShineTable(string file)
        {
            if (!File.Exists(file)) throw new Exception("FFFFFFFFF");

            Dictionary<string, ShineTable> ret = new Dictionary<string, ShineTable>();

            using (var files = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var sr = new StreamReader(files, Encoding.Default))
                {
                    bool definingStruct = false, definingTable = false;
                    int lineNR = 0;
                    ShineTable curTable = null;
                    List<string> ColumnTypes = null;
                    string comment = "";
                    char? ignore = null;
                    KeyValuePair<string, string>? replaceThis = null;
                    while (!sr.EndOfStream)
                    {
                        lineNR++;
                        string line = sr.ReadLine().TrimStart();
                        if (line.Contains(Comment))
                        {
                            // Remove everything after it.
                            int index = line.IndexOf(Comment);
                            comment = line.Substring(index + 1);
                            //Console.WriteLine("Comment @ {0}: {1}", lineNR, comment);

                            line = line.Remove(index);
                        }

                        if (ignore.HasValue)
                        {
                            line = line.Replace(ignore.Value.ToString(), "");
                        }

                        if (line == string.Empty)
                        {
                            continue;
                        }
                        string lineLower = line.ToLower();
                        string[] lineSplit = line.Split('\t');

                        if (lineLower.StartsWith(Replace))
                        {
                            // ...
                            replaceThis = new KeyValuePair<string, string>(ConvertShitToString(lineSplit[1]), ConvertShitToString(lineSplit[2])); // take risks :D
                            //continue;
                        }
                        if (lineLower.StartsWith(Ignore))
                        {
                            ignore = ConvertShitToString(lineSplit[1])[0];
                            //continue;
                        }

                        if (lineLower.StartsWith(StartDefine))
                        {
                            if (definingStruct || definingTable)
                            {
                                throw new Exception("Already defining.");
                            }
                            // Get the name..
                            string name = line.Substring(StartDefine.Length + 1);
                            curTable = new ShineTable(name);

                            definingStruct = true;
                            continue;
                        }
                        else if (lineLower.StartsWith(Table))
                        {
                            if (definingStruct)
                            {
                                throw new Exception("Already defining.");
                            }
                            // Get the name..
                            string name = lineSplit[1].Trim(); // I hope this works D;
                            curTable = new ShineTable(name);
                            ColumnTypes = new List<string>();
                            ret.Add(name, curTable);
                            definingTable = true;
                            continue;
                        }

                        if (lineLower.StartsWith(EndDefine))
                        {
                            if (!definingStruct)
                            {
                                throw new Exception("Not started defining.");
                            }
                            definingStruct = false;
                            ret.Add(curTable.TableName, curTable);
                            continue;
                        }

                        line = line.Trim();
                        lineLower = lineLower.Trim();

                        if (definingStruct)
                        {
                            string columnName = comment.Trim();
                            if (columnName == string.Empty) continue;
                            curTable.AddColumn(columnName, lineLower);
                            Console.WriteLine("Added column {0} to table {1}", columnName, curTable.TableName);
                        }
                        else if (definingTable)
                        {
                            // Lets search for columns..
                            if (lineLower.StartsWith(ColumnType))
                            {
                                for (int i = 1; i < lineSplit.Length; i++)
                                {
                                    string l = lineSplit[i].Trim();
                                    if (l == string.Empty) continue;
                                    ColumnTypes.Add(l);
                                }
                            }
                            else if (lineLower.StartsWith(ColumnName))
                            {
                                int j = 0;
                                for (int i = 1; i < lineSplit.Length; i++)
                                {
                                    string l = lineSplit[i].Trim();
                                    if (l == string.Empty) continue;
                                    var coltype = ColumnTypes[j++];
                                    //curTable.AddColumn(l + "(" + coltype + ")", coltype);
                                    curTable.AddColumn(l, coltype);
                                }
                            }
                            else if (lineLower.StartsWith(RecordLine))
                            {
                                // Next column is tablename
                                string tablename = lineSplit[1].Trim();
                                if (ret.ContainsKey(tablename))
                                {
                                    curTable = ret[tablename];
                                    // Lets start.
                                    object[] data = new object[curTable.Columns.Count];
                                    int j = 0;
                                    for (int i = 2; i < lineSplit.Length; i++)
                                    {
                                        string l = lineSplit[i].Trim();
                                        if (l == string.Empty) continue;
                                        data[j++] = Check(replaceThis, l.TrimEnd(','));
                                    }
                                    curTable.AddRow(data);
                                }
                            }
                            else if (lineLower.StartsWith(Record))
                            {
                                // Right under the table
                                object[] data = new object[curTable.Columns.Count];
                                int j = 0;
                                for (int i = 1; i < lineSplit.Length; i++)
                                {
                                    string l = lineSplit[i].Trim();
                                    if (l == string.Empty) continue;
                                    data[j++] = Check(replaceThis, l.TrimEnd(','));
                                }
                                curTable.AddRow(data);
                            }
                        }
                        else
                        {
                            if (ret.ContainsKey(lineSplit[0].Trim()))
                            {
                                // Should be a struct I guess D:
                                var table = ret[lineSplit[0].Trim()];
                                int columnsInStruct = table.Columns.Count;
                                int readColumns = 0;
                                object[] data = new object[columnsInStruct];
                                for (int i = 1; ; i++)
                                {
                                    if (readColumns == columnsInStruct)
                                    {
                                        break;
                                    }
                                    else if (lineSplit.Length < i)
                                    {
                                        throw new Exception(string.Format("Could not read all columns of line {0}", lineNR));
                                    }
                                    // Cannot count on the tabs ...
                                    string columnText = lineSplit[i].Trim();
                                    if (columnText == string.Empty) continue;
                                    // Well, lets see if we can put it into the list
                                    columnText = columnText.TrimEnd(',').Trim('"');

                                    data[readColumns++] = columnText;
                                }
                                table.AddRow(data);
                            }
                        }

                    }
                }
            }


            return ret;
        }

        private string ConvertShitToString(string input)
        {
            // HACKZ IN HERE
            if (input.StartsWith("\\x"))
            {
                return ((char)Convert.ToByte(input.Substring(2), 16)).ToString();
            }
            else if (input.StartsWith("\\o"))
            {
                return ((char)Convert.ToByte(input.Substring(2), 8)).ToString();
            }

            return input.Length > 0 ? input[0].ToString() : "";
        }

        private string Check(KeyValuePair<string, string>? replacing, string input)
        {
            return replacing.HasValue ? input.Replace(replacing.Value.Key, replacing.Value.Value) : input;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    long i = GC.GetTotalMemory(false);
                    DateTime start = DateTime.Now;
                    var tables = ParseShineTable(file);
                    TimeSpan lon = DateTime.Now.Subtract(start);
                    MessageBox.Show(string.Format("Loaded {1} in {0} msecs ({2} KB in mem)", lon.TotalMilliseconds, file, (GC.GetTotalMemory(false) - i) / 1024));
                    TabControl tc = new TabControl();
                    tc.Dock = DockStyle.Fill;
                    foreach (var kvp in tables)
                    {
                        TabPage tp = new TabPage(kvp.Key);
                        tp.Controls.Add(new DataGridView()
                        {
                            DataSource = kvp.Value,
                            Dock = DockStyle.Fill,
                            EditMode = DataGridViewEditMode.EditProgrammatically,
                            AllowUserToOrderColumns = true,
                            /*AutoSize = true,
                            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells*/
                        });
                        tc.TabPages.Add(tp);
                    }
                    TabPage lawl = new TabPage(Path.GetFileNameWithoutExtension(file));
                    lawl.Controls.Add(tc);
                    tabControl1.TabPages.Add(lawl);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var vals = TablesOnSelectedTab();
            if (vals == null) return;
            foreach (var v in vals)
            {
                var datas = v.DataSource as ShineTable;
                datas.WriteXml(string.Format("Test_{0}_{1}.xml", datas.TableName, DateTime.Now.ToString("d_M_yyyy HH_mm_ss")));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var vals = TablesOnSelectedTab();
            if (vals == null) return;
            foreach (var v in vals)
            {
                var datas = v.DataSource as ShineTable;
                datas.ToFile(string.Format("Class_{0}_{1}.cs", datas.TableName, DateTime.Now.ToString("d_M_yyyy HH_mm_ss")));
            }
        }

        private List<DataGridView> TablesOnSelectedTab()
        {
            if (tabControl1.SelectedTab == null) return null;
            else
            {
                // Tab -> Tab(s) -> datagrid
                List<DataGridView> list = new List<DataGridView>();
                foreach (var control in (tabControl1.SelectedTab.Controls[0] as TabControl).TabPages)
                {
                    list.Add(((control as TabControl).Controls[0]) as DataGridView);
                }
                return list;
            }
        }

        private DataGridView SelectedTableOnSelectedTab()
        {
            if (tabControl1.SelectedTab == null) return null;
            else
            {
                var tc = tabControl1.SelectedTab.Controls[0] as TabControl;
                if (tc.SelectedTab == null) return null;
                return tc.SelectedTab.Controls[0] as DataGridView;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null) return;
            tabControl1.TabPages.Remove(tabControl1.SelectedTab);
        }

        private void ExportAsSHN_Click(object sender, EventArgs e)
        {
            var table = SelectedTableOnSelectedTab();
            if (table != null)
            {
                var shntable = table.DataSource as SHNFile;
                shntable.Save(shntable.TableName + ".shn");
            }
        }
    }
}
