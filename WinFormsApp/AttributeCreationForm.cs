namespace Forms
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    public partial class AttributeCreationForm : Form
    {
        private readonly Dictionary<string, string> _attribute = new Dictionary<string, string>();
        private readonly Regex _attributeName = new Regex(@"^[a-zA-Z0-9_]*[a-zA-Z0-9]+[a-zA-Z0-9_]*$");

        public AttributeCreationForm()
        {
            InitializeComponent();
        }

        private void CreateAttribute_Click(object sender, EventArgs e)
        {
            if (_attributeName.IsMatch(AttributeNameTextbox.Text)) _attribute.Add("Name", AttributeNameTextbox.Text);

            _attribute.Add("SQLType", TypeComboBox.Text);
            _attribute.Add("Description", AttributeDescriptionTExtbox.Text);
            _attribute.Add("IsNullable", IsNullableFlag.AutoCheck.ToString());
            _attribute.Add("IsIndexed", IsIndexedFlag.AutoCheck.ToString());
            _attribute.Add("KeyType", FKcombo.Text);
            Close();
        }

        public void SetNodes(List<string> nodes) { }

        public Dictionary<string, string> GetAttributeDictionary()
        {
            return _attribute;
        }

        private void AttributeCreationForm_Load(object sender, EventArgs e) { }
    }
}