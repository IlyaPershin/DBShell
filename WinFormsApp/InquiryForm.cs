namespace Forms
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using App;
    using Domain.Entities;
    using _Attribute = Domain.Entities.Attribute.Attribute;

    public partial class InquiryForm : Form
    {
        private readonly App _app;
        private readonly List<_Attribute> _attributes = new List<_Attribute>();
        private readonly Database _database;
        private readonly List<_Attribute> _selectedAttributes = new List<_Attribute>();
        private readonly List<Table> _selectedTables = new List<Table>();
        private List<Table> _allTables = new List<Table>();
        private string _inquiry = "";
        private List<Table> _tables = new List<Table>();

        public InquiryForm(Database database, App app)
        {
            InitializeComponent();
            _database = database;
            _app = app;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(_database.ConnectionString);

            using (SqlCommand cmd = new SqlCommand(inquiryTextBox.Text, sqlConnection))
            {
                try
                {
                    sqlConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        for (int i = 0; i < reader.FieldCount; i++) dataGridView1.Columns.Add("Column" + i, reader.GetName(i));

                        int row = 0;
                        do
                        {
                            dataGridView1.Rows.Add();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string type = reader.GetFieldType(i)?.Name;
                                switch (type)
                                {
                                    case "Int16":
                                        dataGridView1[i, row].Value = reader.GetInt16(i);
                                        break;
                                    case "Int32":
                                        dataGridView1[i, row].Value = reader.GetInt32(i);
                                        break;
                                    case "Decimal":
                                        dataGridView1[i, row].Value = reader.GetDecimal(i);
                                        break;
                                    case "Double":
                                        dataGridView1[i, row].Value = reader.GetDouble(i);
                                        break;
                                    case "String":
                                        dataGridView1[i, row].Value = reader.GetString(i);
                                        break;
                                    default:
                                        throw new NotImplementedException();
                                }
                            }

                            row++;
                        } while (reader.Read());
                    }
                    else
                        Console.WriteLine(@"No rows found.");

                    reader.Close();

                    sqlConnection.Close();
                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show($@"Введён неверный текст запроса: {exception.Message}");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _inquiry = inquiryTextBox.Text;
        }

        private void InquiryForm_Load(object sender, EventArgs e)
        {
            _tables = _app.GetDatabaseTables(_database).ToList();
            _allTables = new List<Table>(_tables);
            tablesListBox.DataSource = (from t in _tables select t.Name).ToList();
            tablesListBox.Refresh();
        }

        private void addTableButton_Click(object sender, EventArgs e)
        {
            if (tablesListBox.SelectedIndex < 0) return;

            _selectedTables.Add(_tables[tablesListBox.SelectedIndex]);
            SelectedTablesListBoxRefresh();

            _attributes.AddRange(_app.GetTableAttributes(_tables[tablesListBox.SelectedIndex]));
            AttributesListBoxRefresh();

            _tables.RemoveAt(tablesListBox.SelectedIndex);
            TablesListBoxRefresh();

            AttributeColumnsRefresh();
        }

        private void tablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (tablesListBox.SelectedIndex < 0) return;

            _selectedTables.Add(_tables[tablesListBox.SelectedIndex]);
            SelectedTablesListBoxRefresh();

            _attributes.AddRange(_app.GetTableAttributes(_tables[tablesListBox.SelectedIndex]));
            AttributesListBoxRefresh();

            _tables.RemoveAt(tablesListBox.SelectedIndex);
            TablesListBoxRefresh();

            AttributeColumnsRefresh();
        }

        private void deleteTableButton_Click(object sender, EventArgs e)
        {
            if (selectedTablesListBox.SelectedIndex < 0) return;

            _attributes.RemoveAll(a => a.TableId == _selectedTables[selectedTablesListBox.SelectedIndex].Id);
            AttributesListBoxRefresh();

            _selectedAttributes.RemoveAll(a => a.TableId == _selectedTables[selectedTablesListBox.SelectedIndex].Id);
            SelectedAttributesListBoxRefresh();

            _tables.Add(_selectedTables[selectedTablesListBox.SelectedIndex]);
            TablesListBoxRefresh();

            _selectedTables.RemoveAt(selectedTablesListBox.SelectedIndex);
            SelectedTablesListBoxRefresh();

            AttributeColumnsRefresh();
        }

        private void selectedTablesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedTablesListBox.SelectedIndex < 0) return;

            _attributes.RemoveAll(a => a.TableId == _selectedTables[selectedTablesListBox.SelectedIndex].Id);
            AttributesListBoxRefresh();

            _selectedAttributes.RemoveAll(a => a.TableId == _selectedTables[selectedTablesListBox.SelectedIndex].Id);
            SelectedAttributesListBoxRefresh();

            _tables.Add(_selectedTables[selectedTablesListBox.SelectedIndex]);
            TablesListBoxRefresh();

            _selectedTables.RemoveAt(selectedTablesListBox.SelectedIndex);
            SelectedTablesListBoxRefresh();

            AttributeColumnsRefresh();
        }

        private void addAttributeButton_Click(object sender, EventArgs e)
        {
            if (attributesListBox.SelectedIndex < 0) return;

            _selectedAttributes.Add(_attributes[attributesListBox.SelectedIndex]);
            SelectedAttributesListBoxRefresh();

            _attributes.RemoveAt(attributesListBox.SelectedIndex);
            AttributesListBoxRefresh();
        }

        private void attributesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (attributesListBox.SelectedIndex < 0) return;

            _selectedAttributes.Add(_attributes[attributesListBox.SelectedIndex]);
            SelectedAttributesListBoxRefresh();

            _attributes.RemoveAt(attributesListBox.SelectedIndex);
            AttributesListBoxRefresh();
        }

        private void deleteAttributeButton_Click(object sender, EventArgs e)
        {
            if (selectedAttributesListBox.SelectedIndex < 0) return;

            _attributes.Add(_selectedAttributes[selectedAttributesListBox.SelectedIndex]);
            AttributesListBoxRefresh();

            _selectedAttributes.RemoveAt(selectedAttributesListBox.SelectedIndex);
            SelectedAttributesListBoxRefresh();
        }

        private void selectedAttributesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedAttributesListBox.SelectedIndex < 0) return;

            _attributes.Add(_selectedAttributes[selectedAttributesListBox.SelectedIndex]);
            AttributesListBoxRefresh();

            _selectedAttributes.RemoveAt(selectedAttributesListBox.SelectedIndex);
            SelectedAttributesListBoxRefresh();
        }

        private void TablesListBoxRefresh()
        {
            tablesListBox.DataSource = (from t in _tables select t.Name).ToList();
            tablesListBox.Refresh();
        }

        private void SelectedTablesListBoxRefresh()
        {
            selectedTablesListBox.DataSource = (from t in _selectedTables select t.Name).ToList();
            selectedTablesListBox.Refresh();
        }

        private void AttributesListBoxRefresh()
        {
            List<string> list = new List<string>();
            foreach (_Attribute attribute in _attributes)
                list.Add(
                    _allTables
                        .Where(q => q.Id == attribute.TableId)
                        .ElementAt(0)
                        .Name + ".[" + attribute.Name + "]");

            attributesListBox.DataSource = list;
            attributesListBox.Refresh();
        }

        private void SelectedAttributesListBoxRefresh()
        {
            selectedAttributesListBox.DataSource =
                _selectedAttributes
                    .Select(attribute =>
                                _allTables
                                    .Where(q => q.Id == attribute.TableId)
                                    .ElementAt(0)
                                    .Name + ".[" + attribute.Name + "]")
                    .ToList();

            selectedAttributesListBox.Refresh();
        }

        private void AttributeColumnsRefresh()
        {
            attribute1ComboBox.DataSource =
                _attributes
                    .Select(attribute =>
                                _allTables
                                    .Where(q => q.Id == attribute.TableId)
                                    .ElementAt(0)
                                    .Name + ".[" + attribute.Name + "]")
                    .ToList();

            attribute1ComboBox.Refresh();

            attribute2ComboBox.DataSource =
                _attributes
                    .Select(attribute =>
                                _allTables
                                    .Where(q => q.Id == attribute.TableId)
                                    .ElementAt(0)
                                    .Name + ".[" + attribute.Name + "]")
                    .ToList();

            attribute2ComboBox.Refresh();
        }

        private void conditionDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0) return;

            conditionDataGridView.Rows.RemoveAt(e.RowIndex);
            if (conditionDataGridView.RowCount == 0)
                condition1ComboBox.Enabled = false;
            else
                conditionDataGridView[1, 0].Value = null;
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

            MessageBox.Show(@"Одно или несколько полей не заполнены.");
        }

        private void createInquiryButton_Click(object sender, EventArgs e)
        {
            string from = " ";
            string select = " ";
            string where = " ";

            if (_selectedTables.Count > 0) from = "FROM ";
            if (_selectedAttributes.Count > 0) select = "SELECT ";
            if (conditionDataGridView.RowCount > 0) where = "WHERE ";

            int count = selectedTablesListBox.Items.Count;
            for (int i = 0; i < count - 1; i++) from += selectedTablesListBox.Items[i] + ", ";
            from += selectedTablesListBox.Items[count - 1] + " ";

            count = selectedAttributesListBox.Items.Count;
            for (int i = 0; i < count - 1; i++) select += selectedAttributesListBox.Items[i] + ", ";
            select += selectedAttributesListBox.Items[count - 1] + " ";

            for (int i = 0; i < conditionDataGridView.RowCount; i++)
            {
                if (conditionDataGridView[1, i].Value != null)
                    switch (conditionDataGridView[1, i].Value.ToString())
                    {
                        case "И":
                            where += "AND ";
                            break;
                        case "ИЛИ":
                            where += "OR ";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                where += conditionDataGridView[2, i].Value + " ";
                where += conditionDataGridView[3, i].Value + " ";
                where += conditionDataGridView[4, i].Value + " ";
            }

            _inquiry = select + from;
            if (conditionDataGridView.RowCount > 0)
                _inquiry += where;

            inquiryTextBox.Text = _inquiry;
        }

        private void createAndCarryInquiryButton_Click(object sender, EventArgs e)
        {
            createInquiryButton_Click(sender, e);
            button1_Click(sender, e);
        }
    }
}