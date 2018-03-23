using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Domain.Entities;
using Database = Domain.Entities.Database;
using _Attribute = Domain.Entities.Attribute.Attribute;
using System.Data.SqlClient;

namespace Forms
{
    public partial class InquiryForm : Form
    {
        App.App app;
        Database database;
        string inquiry = "";
        List<Table> allTables = new List<Table>();
        List<Table> tables = new List<Table>();
        List<Table> selectedTables = new List<Table>();
        List<_Attribute> attributes = new List<_Attribute>();
        List<_Attribute> selectedAttributes = new List<_Attribute>();

        public InquiryForm(Database _database, App.App _app)
        {
            InitializeComponent();
            this.database = _database;
            this.app = _app;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection1 = new SqlConnection(database.ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = sqlConnection1;
            try
            {
                sqlConnection1.Open();
                reader = cmd.ExecuteReader();

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                if (reader.HasRows)
                {
                    reader.Read();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dataGridView1.Columns.Add("Column" + i, reader.GetName(i));
                    }

                    int row = 0;
                    do
                    {
                        dataGridView1.Rows.Add();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string type = reader.GetFieldType(i).Name;
                            switch (type)
                            {
                                case ("Int16"):
                                    dataGridView1[i, row].Value = reader.GetInt16(i);
                                    break;
                                case ("Int32"):
                                    dataGridView1[i, row].Value = reader.GetInt32(i);
                                    break;
                                case ("Decimal"):
                                    dataGridView1[i, row].Value = reader.GetDecimal(i);
                                    break;
                                case ("Double"):
                                    dataGridView1[i, row].Value = reader.GetDouble(i);
                                    break;
                                case ("String"):
                                    dataGridView1[i, row].Value = reader.GetString(i);
                                    break;

                            }
                        }
                        row++;
                    } while (reader.Read());
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();

                sqlConnection1.Close();
            }
            catch(InvalidOperationException e1)
            {
                MessageBox.Show("Введён неверный текст запроса.");
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            inquiry = inquiryTextBox.Text;
        }

        private void InquiryForm_Load(object sender, EventArgs e)
        {
            this.tables = app.GetDatabaseTables(database).ToList();
            this.allTables = new List<Table>(this.tables);
            tablesListBox.DataSource = (from t in this.tables select t.Name).ToList();
            tablesListBox.Refresh();
        }

        private void addTableButton_Click(object sender, EventArgs e)
        {
            if (tablesListBox.SelectedIndex >= 0)
            {
                selectedTables.Add(tables[tablesListBox.SelectedIndex]);
                selectedTablesListBoxRefresh();

                attributes.AddRange(app.GetTableAttributes(tables[tablesListBox.SelectedIndex]));
                attributesListBoxRefresh();

                tables.RemoveAt(tablesListBox.SelectedIndex);
                tablesListBoxRefresh();

                attributeColumnsRefresh();
            }
        }

        private void tablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (tablesListBox.SelectedIndex >= 0)
            {
                selectedTables.Add(tables[tablesListBox.SelectedIndex]);
                selectedTablesListBoxRefresh();

                attributes.AddRange(app.GetTableAttributes(tables[tablesListBox.SelectedIndex]));
                attributesListBoxRefresh();

                tables.RemoveAt(tablesListBox.SelectedIndex);
                tablesListBoxRefresh();

                attributeColumnsRefresh();
            }
        }

        private void deleteTableButton_Click(object sender, EventArgs e)
        {
            if (selectedTablesListBox.SelectedIndex >= 0)
            {
                attributes.RemoveAll(a => a.TableId == selectedTables[selectedTablesListBox.SelectedIndex].Id);
                attributesListBoxRefresh();

                selectedAttributes.RemoveAll(a => a.TableId == selectedTables[selectedTablesListBox.SelectedIndex].Id);
                selectedAttributesListBoxRefresh();

                tables.Add(selectedTables[selectedTablesListBox.SelectedIndex]);
                tablesListBoxRefresh();

                selectedTables.RemoveAt(selectedTablesListBox.SelectedIndex);
                selectedTablesListBoxRefresh();

                attributeColumnsRefresh();
            }
        }

        private void selectedTablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedTablesListBox.SelectedIndex >= 0)
            {
                attributes.RemoveAll(a => a.TableId == selectedTables[selectedTablesListBox.SelectedIndex].Id);
                attributesListBoxRefresh();

                selectedAttributes.RemoveAll(a => a.TableId == selectedTables[selectedTablesListBox.SelectedIndex].Id);
                selectedAttributesListBoxRefresh();

                tables.Add(selectedTables[selectedTablesListBox.SelectedIndex]);
                tablesListBoxRefresh();

                selectedTables.RemoveAt(selectedTablesListBox.SelectedIndex);
                selectedTablesListBoxRefresh();

                attributeColumnsRefresh();
            }
        }

        private void addAttributeButton_Click(object sender, EventArgs e)
        {
            if (attributesListBox.SelectedIndex >= 0)
            {
                selectedAttributes.Add(attributes[attributesListBox.SelectedIndex]);
                selectedAttributesListBoxRefresh();

                attributes.RemoveAt(attributesListBox.SelectedIndex);
                attributesListBoxRefresh();
            }
        }

        private void attributesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (attributesListBox.SelectedIndex >= 0)
            {
                selectedAttributes.Add(attributes[attributesListBox.SelectedIndex]);
                selectedAttributesListBoxRefresh();

                attributes.RemoveAt(attributesListBox.SelectedIndex);
                attributesListBoxRefresh();
            }
        }

        private void deleteAttributeButton_Click(object sender, EventArgs e)
        {
            if (selectedAttributesListBox.SelectedIndex >= 0)
            {
                attributes.Add(selectedAttributes[selectedAttributesListBox.SelectedIndex]);
                attributesListBoxRefresh();

                selectedAttributes.RemoveAt(selectedAttributesListBox.SelectedIndex);
                selectedAttributesListBoxRefresh();
            }
        }

        private void selectedAttributesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedAttributesListBox.SelectedIndex >= 0)
            {
                attributes.Add(selectedAttributes[selectedAttributesListBox.SelectedIndex]);
                attributesListBoxRefresh();

                selectedAttributes.RemoveAt(selectedAttributesListBox.SelectedIndex);
                selectedAttributesListBoxRefresh();
            }
        }

        private void tablesListBoxRefresh()
        {
            tablesListBox.DataSource = (from t in this.tables select t.Name).ToList();
            tablesListBox.Refresh();
        }

        private void selectedTablesListBoxRefresh()
        {
            selectedTablesListBox.DataSource = (from t in this.selectedTables select t.Name).ToList();
            selectedTablesListBox.Refresh();
        }

        private void attributesListBoxRefresh()
        {
            attributesListBox.DataSource = (from t in this.attributes
                                            select (
                                            (from q in this.allTables
                                             where q.Id == t.TableId
                                             select q).ElementAt(0).Name + ".[" + t.Name + "]")).ToList();
            attributesListBox.Refresh();
        }

        private void selectedAttributesListBoxRefresh()
        {
            selectedAttributesListBox.DataSource = (from t in this.selectedAttributes
                                                    select (
                                                    (from q in this.allTables
                                                     where q.Id == t.TableId
                                                     select q).ElementAt(0).Name + ".[" + t.Name + "]")).ToList();
            selectedAttributesListBox.Refresh();
        }

        private void attributeColumnsRefresh()
        {
            attribute1ComboBox.DataSource = (from t in this.attributes
                                             select (
                                             (from q in this.allTables
                                              where q.Id == t.TableId
                                              select q).ElementAt(0).Name + ".[" + t.Name + "]")).ToList();
            attribute1ComboBox.Refresh();

            attribute2ComboBox.DataSource = (from t in this.attributes
                                             select (
                                             (from q in this.allTables
                                              where q.Id == t.TableId
                                              select q).ElementAt(0).Name + ".[" + t.Name + "]")).ToList();
            attribute2ComboBox.Refresh();
        }

        private void conditionDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                conditionDataGridView.Rows.RemoveAt(e.RowIndex);
                if (conditionDataGridView.RowCount == 0)
                    condition1ComboBox.Enabled = false;
                else
                    conditionDataGridView[1, 0].Value = null;
            }
        }

        private void createConditionButton_Click(object sender, EventArgs e)
        {
            if (conditionDataGridView.RowCount == 0 && attribute1ComboBox.SelectedIndex >= 0 &&
                condition2ComboBox.SelectedIndex >= 0 && attribute2ComboBox.Text.Length > 0)
            {
                conditionDataGridView.Rows.Add();
                int index = conditionDataGridView.Rows.Count - 1;
                conditionDataGridView[2, index].Value = attribute1ComboBox.Text;
                conditionDataGridView[3, index].Value = condition2ComboBox.Text;
                conditionDataGridView[4, index].Value = attribute2ComboBox.Text;
                condition1ComboBox.Enabled = true;
                return;
            }

            if (conditionDataGridView.RowCount > 0 && condition1ComboBox.SelectedIndex >= 0 &&
                attribute1ComboBox.SelectedIndex >= 0 && condition2ComboBox.SelectedIndex >= 0 &&
                attribute2ComboBox.Text.Length > 0)
            {
                conditionDataGridView.Rows.Add();
                int index = conditionDataGridView.Rows.Count - 1;
                conditionDataGridView[1, index].Value = condition1ComboBox.Text;
                conditionDataGridView[2, index].Value = attribute1ComboBox.Text;
                conditionDataGridView[3, index].Value = condition2ComboBox.Text;
                conditionDataGridView[4, index].Value = attribute2ComboBox.Text;
                return;
            }

            MessageBox.Show("Одно или несколько полей не заполнены");
        }

        private void createInquiryButton_Click(object sender, EventArgs e)
        {
            string from = " ";
            string select = " ";
            string where = " ";

            if (selectedTables.Count > 0) from = "FROM ";
            if (selectedAttributes.Count > 0) select = "SELECT ";
            if (conditionDataGridView.RowCount > 0) where = "WHERE ";

            int count = selectedTablesListBox.Items.Count;
            for (int i = 0; i < count - 1; i++)
            {
                from += (selectedTablesListBox.Items[i].ToString() + ", ");
            }
            from += (selectedTablesListBox.Items[count - 1].ToString() + " ");

            count = selectedAttributesListBox.Items.Count;
            for (int i = 0; i < count - 1; i++)
            {
                select += (selectedAttributesListBox.Items[i] + ", ");
            }
            select += (selectedAttributesListBox.Items[count - 1] + " ");

            for (int i = 0; i < conditionDataGridView.RowCount; i++)
            {
                if (conditionDataGridView[1, i].Value != null)
                {
                    if (conditionDataGridView[1, i].Value.ToString() == "И")
                        where += "AND ";
                    if (conditionDataGridView[1, i].Value.ToString() == "ИЛИ")
                        where += "OR ";
                }
                where += conditionDataGridView[2, i].Value.ToString() + " ";
                where += conditionDataGridView[3, i].Value.ToString() + " ";
                where += conditionDataGridView[4, i].Value.ToString() + " ";
            }

            inquiry = select + from;
            if (conditionDataGridView.RowCount > 0)
                inquiry += where;

            inquiryTextBox.Text = inquiry;
        }

        private void createAndCarryInquiryButton_Click(object sender, EventArgs e)
        {
            createInquiryButton_Click(sender, e);
            button1_Click(sender, e);
        }
    }
}
