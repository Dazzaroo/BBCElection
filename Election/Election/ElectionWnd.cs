using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Election
{

    public partial class ElectionWnd : Form
    {

        const string MAJORITY_FOUND_MESSAGE = " has won the General Election!";

        public ElectionWnd()
        {
            InitializeComponent();
        }

        private void btnElectionStart_Click(object sender, EventArgs e)
        {
            btnElectionStart.Enabled = false;
  
            // Start Election Process in separate thread so our UI does not become unresponsive.
            bsElection.SuspendBinding();
            Task.Factory.StartNew(() =>
            {
                // Start election process and pass our gui dataset which is refresh each file load and our MajorityFound procedure
                ElectionProcess electionProcess  = new ElectionProcess(this.dsElection, MajorityFound, RefreshGrid, this);
                electionProcess.ElectionStart();
            });

        }

        // When majority found then this is called via delegate to show a result has been found
        public void MajorityFound(string winner)
        {
            lblResult.Text = winner + MAJORITY_FOUND_MESSAGE;
            lblResult.ForeColor = Color.Red;
        }

        public void RefreshGrid(DataSet ds)
        {
    
            dsElection.Tables[ElectionProcess.RESULTS_TABLE].Clear();
            dsElection.Tables[ElectionProcess.RESULTS_TABLE].Merge(ds.Tables[ElectionProcess.RESULTS_TABLE]);
            bsElection.ResumeBinding();
        }
    }
}
