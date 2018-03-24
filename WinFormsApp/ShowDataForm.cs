using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Forms
{
    using Domain.Entities.Attribute.Integer;

    public partial class ShowDataForm : Form
    {
        private IEnumerable<IDictionary<Domain.Entities.Attribute.Attribute, object>> _dataEnumerable;

        public ShowDataForm(IEnumerable<IDictionary<Domain.Entities.Attribute.Attribute, object>> dataEnumerable)
        {
            _dataEnumerable = dataEnumerable;
            InitializeComponent();

            this.Show();

            //dataGridView1.DataSource = 
        }

        private void Show()
        {
            int i = 0;
            foreach (KeyValuePair<Domain.Entities.Attribute.Attribute, object> keyValuePair in _dataEnumerable.ElementAt(0))
            {
                dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
                dataGridView1.Columns[i].Name = keyValuePair.Key.Name;
                i++;
            }

            i = 0;
            foreach (Dictionary<Domain.Entities.Attribute.Attribute, object> dictionary in _dataEnumerable)
            {
                dataGridView1.Rows.Add();
                int j = 0;
                foreach (KeyValuePair<Domain.Entities.Attribute.Attribute, object> keyValuePair in dictionary)
                {
                    switch (keyValuePair.Key)
                    {
                        case PrimaryKey primaryKey:
                            dataGridView1[j, i].Value = (int)keyValuePair.Value; 
                            break;
                        case Domain.Entities.Attribute.String str:
                            dataGridView1[j, i].Value = (string)keyValuePair.Value;
                            break;
                        case Domain.Entities.Attribute.DecimalNumber str:
                            dataGridView1[j, i].Value = ((decimal)keyValuePair.Value).ToString();
                            break;
                        case Domain.Entities.Attribute.RealNumber str:
                            dataGridView1[j, i].Value = ((double)keyValuePair.Value).ToString();
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
