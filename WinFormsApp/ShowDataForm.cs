namespace Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;
    using Domain.Entities.Attribute;
    using Domain.Entities.Attribute.Integer;
    using Attribute = Domain.Entities.Attribute.Attribute;
    using String = Domain.Entities.Attribute.String;

    public partial class ShowDataForm : Form
    {
        private readonly IEnumerable<IDictionary<Attribute, object>> _dataEnumerable;

        public ShowDataForm(IEnumerable<IDictionary<Attribute, object>> dataEnumerable)
        {
            _dataEnumerable = dataEnumerable;
            InitializeComponent();

            Show();
        }

        private new void Show()
        {
            int i = 0;
            foreach (KeyValuePair<Attribute, object> keyValuePair in _dataEnumerable.ElementAt(0))
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridView1.Columns[i].Name = keyValuePair.Key.Name;
                i++;
            }

            i = 0;
            foreach (IDictionary<Attribute, object> dictionary in _dataEnumerable)
            {
                dataGridView1.Rows.Add();
                int j = 0;
                foreach (KeyValuePair<Attribute, object> keyValuePair in dictionary)
                {
                    switch (keyValuePair.Key)
                    {
                        case PrimaryKey _:
                            dataGridView1[j, i].Value = (int)keyValuePair.Value;
                            break;
                        case String _:
                            dataGridView1[j, i].Value = (string)keyValuePair.Value;
                            break;
                        case DecimalNumber _:
                            dataGridView1[j, i].Value = ((decimal)keyValuePair.Value).ToString(CultureInfo.CurrentCulture);
                            break;
                        case RealNumber _:
                            dataGridView1[j, i].Value = ((double)keyValuePair.Value).ToString(CultureInfo.CurrentCulture);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    j++;
                }

                i++;
            }
        }
    }
}