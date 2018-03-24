namespace Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public partial class LinkCreationForm : Form
    {
        private bool _isCascadeDelete,
                     _isCascadeUpdate;

        private List<TreeNode> _listOfConnectiableNodes = new List<TreeNode>();
        private string _masterTable;
        private string _slaveTable;

        public LinkCreationForm()
        {
            InitializeComponent();
        }

        private void LinkCreationForm_Load(object sender, EventArgs e)
        {
            LinksBox.Items.Clear();
            foreach (TreeNode node in _listOfConnectiableNodes)
                LinksBox.Items.Add(node.Text);
            MTable.Text = _masterTable;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            _isCascadeDelete = CascadeDelete.AutoCheck;
            _isCascadeUpdate = CascadeUpdate.AutoCheck;
            _slaveTable = LinksBox.Text;
            Close();
        }

        public void SetListOfNodes(List<TreeNode> list)
        {
            _listOfConnectiableNodes = list;
        }

        public void SetMasterTable(string mTable)
        {
            _masterTable = mTable;
        }

        public string GetSlaveTable()
        {
            return _slaveTable;
        }

        public bool GetCascadeUpdate()
        {
            return _isCascadeUpdate;
        }

        public bool GetCascadeDelete()
        {
            return _isCascadeDelete;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}