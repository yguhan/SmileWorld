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
        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.BeginUpdate();

                ListViewItem lvi = new ListViewItem("yghan");
                listView1.Items.Add(lvi);


                ListViewItem lvi2 = new ListViewItem("yghan");
                listView1.Items.Add(lvi2);
       

        }
    }
    
    
}
