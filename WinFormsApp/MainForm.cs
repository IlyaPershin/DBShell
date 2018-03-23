

// Check connection string!

namespace Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using App;
    using Domain.Entities;
    using Domain.Entities.Attribute;
    using Domain.Entities.Attribute.Integer;
    using Domain.Entities.Link;
    using Attribute = Domain.Entities.Attribute.Attribute;
    using String = Domain.Entities.Attribute.String;

    public partial class MainForm : Form
    {
        private readonly App _app;
        private readonly List<string> _nodes = new List<string>();

        public MainForm(App app)
        {
            _app = app;
            InitializeComponent();
        }

        #region CreateDatabaseButton

        private void CreateDatabase_Click(object sender, EventArgs e)
        {
            _nodes.Clear();
            DatabaseCreationForm databaseForm = new DatabaseCreationForm();
            if (DatabasesTree.Nodes.Count != 0)
                foreach (TreeNode treeviewNodes in DatabasesTree.Nodes)
                    _nodes.Add(treeviewNodes.Text);

            databaseForm.SetNodes(_nodes);
            databaseForm.Show();
            databaseForm.Text = @"База данных";
            databaseForm.dbNameLabel.Text = @"Введите название базы данных";
            databaseForm.FormClosing += (obj, args) =>
            {
                TreeNode newNode = new TreeNode();

                if (databaseForm.InputText == string.Empty) return;

                try
                {
                    _app.CreateDatabase(databaseForm.InputText);
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show(exception.Message);
                }

                newNode.Text = databaseForm.InputText;
                DatabasesTree.Nodes.Add(newNode);


                DatabasesTree.ExpandAll();
                DatabasesTree.Update();
            };
        }

        #endregion CreateDatabaseButton

        #region CreateTableButton

        private void CreateTable_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null && DatabasesTree.SelectedNode.Level == 0)
            {
                _nodes.Clear();
                DatabaseCreationForm tableForm = new DatabaseCreationForm();
                if (DatabasesTree.Nodes.Count != 0)
                {
                    TreeNode treeviewNodes = DatabasesTree.SelectedNode != null && DatabasesTree.SelectedNode.Level == 0
                        ? DatabasesTree.SelectedNode
                        : DatabasesTree.TopNode;
                    foreach (TreeNode node in treeviewNodes.Nodes) _nodes.Add(node.Text);
                }

                tableForm.SetNodes(_nodes);
                tableForm.Show();
                tableForm.Text = @"Таблица";
                tableForm.dbNameLabel.Text = @"Введите название таблицы";
                tableForm.FormClosing += (obj, args) =>
                {
                    TreeNode node = new TreeNode();

                    if (tableForm.InputText == string.Empty) return;

                    Database database = _app.GetDatabaseByName(DatabasesTree.SelectedNode.Text);
                    try
                    {
                        _app.AddTable(database, tableForm.InputText);
                    }
                    catch (ArgumentException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }

                    ShowDatabases();
                };
            }
            else
                MessageBox.Show(@"Выберите базу данных!");
        }

        #endregion CreateTableButton

        #region CreateAttributeButton

        private void CreateAttribute_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null && DatabasesTree.SelectedNode.Level == 1)
            {
                Dictionary<string, string> attribute;
                AttributeCreationForm attributeForm = new AttributeCreationForm();
                List<TreeNode> nodesList = new List<TreeNode>();
                nodesList.AddRange(
                    DatabasesTree.Nodes
                                 .OfType<TreeNode>()
                                 .Where(x => x.Text == @"PrimaryKey")
                                 .Select(x => x.Parent)
                                 .ToArray()
                );
                foreach (TreeNode node in nodesList) _nodes.Add(node.Text);

                attributeForm.SetNodes(_nodes);
                attributeForm.Show();
                attributeForm.FormClosing += (obj, args) =>
                {
                    TreeNode newNode = new TreeNode();
                    attribute = attributeForm.GetAttributeDictionary();
                    newNode.Text = attribute["Name"];
                    foreach (KeyValuePair<string, string> attr in attribute)
                    {
                        TreeNode childNode = new TreeNode {Text = attr.Key + @"=>" + attr.Value};
                        newNode.Nodes.Add(childNode);
                    }

                    Table table = _app.GetTableByName(_app.GetDatabaseByName(DatabasesTree.SelectedNode.Parent.Text),
                                                      DatabasesTree.SelectedNode.Text);
                    try
                    {
                        switch (attribute["SQLType"])
                        {
                            case "NVARCHAR":
                                _app.AddDecimalAttribute(table, attribute["Name"],
                                                         Convert.ToBoolean(attribute["IsNullable"]));
                                break;
                            case "INT":
                                _app.AddIntegerAttribute(table, attribute["Name"],
                                                         Convert.ToBoolean(attribute["IsNullable"]));
                                break;
                            case "STRING":
                                _app.AddStringAttribute(table, attribute["Name"],
                                                        Convert.ToBoolean(attribute["IsNullable"]));
                                break;
                            case "FLOAT":
                                _app.AddFloatAttribute(table, attribute["Name"],
                                                       Convert.ToBoolean(attribute["IsNullable"]));
                                break;
                        }
                    }
                    catch (ArgumentException exception)
                    {
                        MessageBox.Show(exception.Message);
                    }

                    DatabasesTree.SelectedNode.Nodes.Add(newNode);
                    ShowDatabases();
                };
            }
            else
                MessageBox.Show(@"Выберите таблицу!");
        }

        #endregion CreateAttributeButton

        #region CreateLinkButton

        private void CreateLinkButton_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null &&
                DatabasesTree.SelectedNode.Level == 1)
            {
                TreeNode parentNode = DatabasesTree.SelectedNode.Parent;
                Table seletctedTable = _app.GetTableByName(_app.GetDatabaseByName(parentNode.Text),
                                                           DatabasesTree.SelectedNode.Text);
                List<TreeNode> listOfConnectiableNodes = new List<TreeNode>();
                foreach (TreeNode childNode in parentNode.Nodes)
                    if (childNode.Text != DatabasesTree.SelectedNode.Text)
                        listOfConnectiableNodes.Add(childNode);
                IEnumerable<Link> links =
                    _app.GetDatabaseLinks(_app.GetDatabaseByName(DatabasesTree.SelectedNode.Parent.Text));

                IEnumerable<Link> enumerable = links.ToList();
                foreach (TreeNode connectiableNode in listOfConnectiableNodes)
                {
                    Table connectedTable =
                        _app.GetTableByName(_app.GetDatabaseByName(parentNode.Text), connectiableNode.Text);
                    foreach (Link link in enumerable.Where(link => link.MasterAttributeId == seletctedTable.Id && link.SlaveAttributeId == connectedTable.Id))
                        listOfConnectiableNodes.Remove(connectiableNode);
                }

                LinkCreationForm linkForm = new LinkCreationForm();
                linkForm.setListOfNodes(listOfConnectiableNodes);
                linkForm.setMasterTable(DatabasesTree.SelectedNode.Text);
                linkForm.Show();

                linkForm.FormClosing += (obj, args) =>
                {
                    Table slaveTable = _app.GetTableByName(_app.GetDatabaseByName(parentNode.Text),
                                                           linkForm.getSlaveTable());
                    try
                    {
                        _app.AddLink(seletctedTable, slaveTable, linkForm.getCascadeDelete(),
                                     linkForm.getCascadeUpdate());
                    }
                    catch (ArgumentException ae)
                    {
                        MessageBox.Show(@"Такая ссылка уже существует." + ae.Message);
                    }
                };
            }
            else
                MessageBox.Show(@"Выберите таблицу!");
        }

        #endregion CreateLinkButton

        #region ShowDatabasesButton

        private void showDatabaseButton_Click(object sender, EventArgs e)
        {
            ShowDatabases();
        }

        #endregion ShowDatabasesButton

        #region ShowDatabasesMethod

        public void ShowDatabases()
        {
            DatabasesTree.Nodes.Clear();
            IEnumerable<Database> databases = _app.GetAllDatabases().ToList();
            if (!databases.Any())
                MessageBox.Show(@"Вы не создали ни одной базы данных!");
            else
                foreach (Database database in databases)
                {
                    TreeNode dbasesNode = new TreeNode {Text = database.Name};
                    List<Table> tables = _app.GetDatabaseTables(database).ToList();
                    foreach (Table table in tables)
                    {
                        TreeNode tablesNode = new TreeNode {Text = table.Name};
                        IEnumerable<Attribute> attributes = _app.GetTableAttributes(table);
                        foreach (Attribute attribute in attributes)
                        {
                            TreeNode attributeNode = new TreeNode {Text = attribute.Name};
                            TreeNode attributType = new TreeNode {Text = attribute.SqlType.ToString()};
                            TreeNode attributeDescription = new TreeNode {Text = attribute.Description};
                            TreeNode attributeIsNullable = new TreeNode {Text = attribute.IsNullable.ToString()};

                            TreeNode attributeIsIndexed = new TreeNode {Text = attribute.IsIndexed.ToString()};

                            TreeNode attributeIsPrimaryKey = new TreeNode {Text = attribute.IsPrimaryKey.ToString()};

                            attributeNode.Nodes.Add("Description => " + attributeDescription.Text);
                            attributeNode.Nodes.Add("Type => {attribute.SqlType}");
                            attributeNode.Nodes.Add("Type => " + attributType.Text);
                            attributeNode.Nodes.Add("IsNullable => " + attributeIsNullable.Text);
                            attributeNode.Nodes.Add("IsIndexed => " + attributeIsIndexed.Text);
                            attributeNode.Nodes.Add("IsPrimaryKey => " + attributeIsPrimaryKey.Text);

                            tablesNode.Nodes.Add(attributeNode);
                        }

                        dbasesNode.Nodes.Add(tablesNode);
                        tablesNode.Expand();
                        dbasesNode.Expand();
                    }

                    DatabasesTree.Nodes.Add(dbasesNode);
                }
        }

        #endregion ShowDatabasesMethod

        #region ShowLinksButton

        private void ShowLinks_Click(object sender, EventArgs e)
        {
            LinksView.Rows.Clear();
            if (DatabasesTree.SelectedNode != null &&
                DatabasesTree.SelectedNode.Level == 0)
            {
                Database showingDatabase = _app.GetDatabaseByName(DatabasesTree.SelectedNode.Text);
                IEnumerable<Link> links;
                try
                {
                    links = _app.GetDatabaseLinks(showingDatabase).ToList();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(@"Ошибка при получении ссылок. " + ex.Message);
                    return;
                }

                foreach (Link link in links)
                    try
                    {
                        Table MasterTable = _app.GetTableById(link.MasterAttributeId);
                        Table slaveTable = _app.GetAttributeTable(_app.GetAttributeById(link.SlaveAttributeId));
                        if (MasterTable != null && slaveTable != null)
                            LinksView.Rows.Add(MasterTable.Name, slaveTable.Name, link.MasterAttributeId.ToString(), link.SlaveAttributeId.ToString());
                        else
                            MessageBox.Show(@"Не удалось получить экземпляр таблицы.");
                    }
                    catch (ArgumentNullException except)
                    {
                        MessageBox.Show(@"Нулевая ссылка: " + except.Message);
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(@"Ошибка при вставке: " + ex.Message);
                    }
            }
            else
                MessageBox.Show(@"Выберите базу данных!");
        }

        #endregion ShowLinksButton

        #region MainForm_Load

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowDatabases();
        }

        #endregion MainForm_Load

        #region DeleteDatabaseButton

        private void DeleteDatabaseButton_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null &&
                DatabasesTree.SelectedNode.Level == 0)
                try
                {
                    if (_app.IsDatabaseExist(DatabasesTree.SelectedNode.Text))
                    {
                        _app.RemoveDatabase(_app.GetDatabaseByName(DatabasesTree.SelectedNode.Text));
                        DatabasesTree.SelectedNode.Remove();
                        ShowDatabases();
                    }
                    else
                        MessageBox.Show(@"Базы с таким именем не существует!");
                }
                catch (NullReferenceException exception)
                {
                    MessageBox.Show(@"Нулевая ссылка: " + exception.Message);
                }
            else
                MessageBox.Show(@"Выберите базу данных!");
        }

        #endregion DeleteDatabaseButton

        #region DeleteTableButton

        private void DeleteTableButton_Click(object sender, EventArgs e)
        {
            //Чекнуть существование связи с этой таблицей перед удалением!
            if (DatabasesTree.SelectedNode != null &&
                DatabasesTree.SelectedNode.Level == 1)
                try
                {
                    Database database = _app.GetDatabaseByName(DatabasesTree.SelectedNode.Parent.Text);

                    Table table = _app.GetTableByName(database, DatabasesTree.SelectedNode.Text);

                    _app.RemoveTable(table);

                    DatabasesTree.SelectedNode.Remove();
                    ShowDatabases();
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show(@"У таблицы DatabasesTree.SelectedNode.Text имеются связанные таблицы!" + exception.Message);
                }
            else
                MessageBox.Show(@"Выберите таблицу!");
        }

        #endregion DeleteTableButton

        #region DeleteAttributeButton

        private void DeleteAttributeButton_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null &&
                DatabasesTree.SelectedNode.Level == 2)
                try
                {
                    _app.RemoveAttribute(_app.GetAttributeByName(
                                             _app.GetTableByName(_app.GetDatabaseByName(DatabasesTree.SelectedNode.Parent.Parent.Text),
                                                                 DatabasesTree.SelectedNode.Parent.Text), DatabasesTree.SelectedNode.Text));
                    DatabasesTree.SelectedNode.Remove();
                    ShowDatabases();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(@"Уникальный идентификатор нельзя удалить! " + exception.Message);
                }
            else
                MessageBox.Show(@"Выберите атрибут!");
        }

        #endregion DeleteAttributeButton

        #region DeleteLinkButton

        private void DeleteLinkButton_Click(object sender, EventArgs e)
        {
            if (LinksView.SelectedRows.Count != 0)
            {
                Table masterTable = _app.GetTableById(Convert.ToInt32(LinksView.SelectedRows[0].Cells[2].Value));
                Table slaveTable = _app.GetTableById(Convert.ToInt32(LinksView.SelectedRows[0].Cells[3].Value));
                try
                {
                    Link linkToDelete = _app.GetLink(masterTable, slaveTable);
                    _app.RemoveLink(linkToDelete);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Ошибка при удалении связи. " + ex.Message);
                }
            }
            else
                MessageBox.Show(@"Выберите связь для удаления!");
        }

        #endregion DeleteLinkButton

        private void CreateInquiryButton_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null && DatabasesTree.SelectedNode.Level == 0)
            {
                Database database = _app.GetDatabaseByName(DatabasesTree.SelectedNode.Text);

                try
                {
                    InquiryForm inquiryForm = new InquiryForm(database, _app);
                    inquiryForm.ShowDialog();
                }
                catch (ArgumentException e1)
                {
                    Console.WriteLine("ошбика при проверке возможности развертывания. " + e1.Message);
                }


                //_nodes.Clear();
                //if (DatabasesTree.Nodes.Count != 0)
                //{
                //    TreeNode treeviewNodes = DatabasesTree.SelectedNode != null && DatabasesTree.SelectedNode.Level == 0
                //        ? DatabasesTree.SelectedNode
                //        : DatabasesTree.TopNode;
                //    foreach (TreeNode node in treeviewNodes.Nodes) _nodes.Add(node.Text);
                //}
            }
            else
                MessageBox.Show(@"Выберите базу данных!");
        }

        private void DeployButton_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode != null &&
                DatabasesTree.SelectedNode.Level == 0)
                try
                {
                    if (_app.IsDatabaseExist(DatabasesTree.SelectedNode.Text))
                    {
                        Database database = _app.GetDatabaseByName(DatabasesTree.SelectedNode.Text);

                        if (_app.IsDatabaseDeployed(database))
                        {
                            MessageBox.Show(@"Database already deployed.");
                            return;
                        }

                        if (!_app.IsDatabaseDeployable(database))
                        {
                            MessageBox.Show(@"Database cannot be deployed.");
                            return;
                        }

                        _app.DeployDatabase(database);
                        ShowDatabases();
                    }
                    else
                        MessageBox.Show(@"Базы с таким именем не существует!");
                }
                catch (NullReferenceException exception)
                {
                    MessageBox.Show(@"Нулевая ссылка: " + exception.Message);
                }
            else
                MessageBox.Show(@"Выберите базу данных!");
        }

        private void AddRowButton_Click(object sender, EventArgs e)
        {
            if (DatabasesTree.SelectedNode.Parent != null &&
                DatabasesTree.SelectedNode.Parent.Level == 0)
                if (_app.IsDatabaseExist(DatabasesTree.SelectedNode.Parent.Text))
                {
                    Database database = _app.GetDatabaseByName(DatabasesTree.SelectedNode.Parent.Text);

                    if (!_app.IsDatabaseDeployed(database))
                    {
                        MessageBox.Show(@"Database was not deployed.");
                        return;
                    }

                    Table table = _app.GetTableByName(database, DatabasesTree.SelectedNode.Text);

                    Dictionary<Attribute, string> values = new Dictionary<Attribute, string>();

                    Form form = CreateAddRowForm(_app.GetTableAttributes(table));
                    form.Show();
                    form.FormClosing += (obj, args) =>
                    {
                        IEnumerable<Attribute> attributes = _app.GetTableAttributes(table).ToList();

                        foreach (Attribute attribute in attributes)
                            switch (attribute)
                            {
                                case PrimaryKey _:
                                case ForeignKey _:
                                    continue;

                                case String stringAttribute:
                                    string stringInputValue = form.Controls[$"{attribute.Name}TextBox"].Text;
                                    values[stringAttribute] = stringInputValue;
                                    break;
                                case IntegerNumber integerAttribute:
                                    string intInputValue = form.Controls[$"{attribute.Name}TextBox"].Text;
                                    values[integerAttribute] = intInputValue;
                                    break;
                                case DecimalNumber decimalNumber:
                                    string stringDecimalValue = form.Controls[$"{attribute.Name}TextBox"].Text;
                                    values[decimalNumber] = stringDecimalValue;
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }

                        try
                        {
                            _app.InsertData(table, values);
                        }
                        catch (ArgumentException e2)
                        {
                            Console.WriteLine(@"Ошбика при вставке значений: " + e2.Message);
                        }

                        ShowDatabases();
                    };
                }
                else
                    MessageBox.Show(@"Базы с таким именем не существует!");
            else
                MessageBox.Show(@"Выберите базу данных!");
        }

        private Form CreateAddRowForm(IEnumerable<Attribute> attributes)
        {
            int tabIndex = 1;
            const int verticalOffsetBase = 10;
            const int verticalOffsetSize = 30;
            int verticalOffsset = 0;

            Form addRowForm = new Form
            {
                AutoScaleDimensions = new SizeF(6F, 13F),
                AutoScaleMode = AutoScaleMode.Font,
                ClientSize = new Size(500, verticalOffsetBase + verticalOffsetSize * 10),
                Name = "AddRowForm",
                Text = @"Add Row Form"
            };

            addRowForm.SuspendLayout();

            foreach (Attribute attribute in attributes.Where(attribute => !(attribute is PrimaryKey) && !(attribute is ForeignKey)))
            {
                addRowForm.Controls.Add(new Label
                {
                    AutoSize = true,
                    Location = new Point(10, verticalOffsetBase + verticalOffsset * verticalOffsetSize),
                    Name = $"{attribute.Name}Label",
                    Size = new Size(35, 20),
                    TabIndex = tabIndex++,
                    Text = attribute.Name
                });

                addRowForm.Controls.Add(new TextBox
                {
                    Location = new Point(150, verticalOffsetBase + verticalOffsset * verticalOffsetSize),
                    Name = $"{attribute.Name}TextBox",
                    Size = new Size(300, 20),
                    TabIndex = tabIndex++
                });

                verticalOffsset++;
            }

            addRowForm.Controls.Add(new Button
            {
                Location = new Point(250, verticalOffsetBase + verticalOffsset * verticalOffsetSize),
                Name = "SaveButton",
                Size = new Size(75, 20),
                TabIndex = tabIndex++,
                Text = @"Save",
                UseVisualStyleBackColor = true
            });

            addRowForm.ResumeLayout(false);
            addRowForm.PerformLayout();

            return addRowForm;
        }
    }
}