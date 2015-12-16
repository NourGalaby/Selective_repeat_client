using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client_selective
{
    public partial class status : Form
    {
        public status()
        {
            InitializeComponent();

            Button[] buttons = new Button[500];
               for(int i=0;i<500;i++){
                buttons[i]=   new System.Windows.Forms.Button();
               }

               panel1.Controls.AddRange(buttons);
        }

        static public void init()
        {
    
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
