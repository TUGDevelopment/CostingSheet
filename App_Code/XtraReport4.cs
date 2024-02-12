using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WebApplication1
{
    public partial class XtraReport4 : DevExpress.XtraReports.UI.XtraReport
    {
        private string p;

        public XtraReport4()
        {
            InitializeComponent();
        }

        public XtraReport4(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }

    }
}
