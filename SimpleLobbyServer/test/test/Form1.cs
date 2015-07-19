using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        string readData;
        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.BeginUpdate();

      /*
                ListViewItem lvi = new ListViewItem("haha");
                listView1.Items.Add(lvi);


                ListViewItem lvi2 = new ListViewItem("yghan");
                listView1.Items.Add(lvi2);
                Console.WriteLine("haha");
            */
        }

        private void button1_Click(object sender, EventArgs e)
        {
            readData = textBox1.
            Text;
            listV();

            
        }


        private void listV()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(listV));
            else
            {
        //        listView1.View = View.Details;
          //      listView1.BeginUpdate();

                ListViewItem lvi = new ListViewItem(readData);
                listView1.Items.Add(lvi);
                Console.WriteLine(readData);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indexNum = listView1.FocusedItem.Index;
            string dT = listView1.Items[indexNum].SubItems[0].Text;
            textBox2.Text = dT;
        }


    }
    
    
}
