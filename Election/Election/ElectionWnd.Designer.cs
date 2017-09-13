namespace Election
{
    partial class ElectionWnd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnElectionStart = new System.Windows.Forms.Button();
            this.gvElection = new System.Windows.Forms.DataGridView();
            this.partyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seatsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shareDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsElection = new System.Windows.Forms.BindingSource(this.components);
            this.dsElection = new System.Data.DataSet();
            this.DtResults = new System.Data.DataTable();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.lblResult = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gvElection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsElection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsElection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtResults)).BeginInit();
            this.SuspendLayout();
            // 
            // btnElectionStart
            // 
            this.btnElectionStart.Location = new System.Drawing.Point(43, 34);
            this.btnElectionStart.Name = "btnElectionStart";
            this.btnElectionStart.Size = new System.Drawing.Size(328, 23);
            this.btnElectionStart.TabIndex = 0;
            this.btnElectionStart.Text = "Press to start for Election process to begin!";
            this.btnElectionStart.UseVisualStyleBackColor = true;
            this.btnElectionStart.Click += new System.EventHandler(this.btnElectionStart_Click);
            // 
            // gvElection
            // 
            this.gvElection.AutoGenerateColumns = false;
            this.gvElection.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvElection.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.partyDataGridViewTextBoxColumn,
            this.seatsDataGridViewTextBoxColumn,
            this.shareDataGridViewTextBoxColumn});
            this.gvElection.DataSource = this.bsElection;
            this.gvElection.Location = new System.Drawing.Point(43, 93);
            this.gvElection.Name = "gvElection";
            this.gvElection.Size = new System.Drawing.Size(344, 143);
            this.gvElection.TabIndex = 1;
            // 
            // partyDataGridViewTextBoxColumn
            // 
            this.partyDataGridViewTextBoxColumn.DataPropertyName = "Party";
            this.partyDataGridViewTextBoxColumn.HeaderText = "Party";
            this.partyDataGridViewTextBoxColumn.Name = "partyDataGridViewTextBoxColumn";
            // 
            // seatsDataGridViewTextBoxColumn
            // 
            this.seatsDataGridViewTextBoxColumn.DataPropertyName = "Seats";
            this.seatsDataGridViewTextBoxColumn.HeaderText = "Seats";
            this.seatsDataGridViewTextBoxColumn.Name = "seatsDataGridViewTextBoxColumn";
            // 
            // shareDataGridViewTextBoxColumn
            // 
            this.shareDataGridViewTextBoxColumn.DataPropertyName = "Share";
            this.shareDataGridViewTextBoxColumn.HeaderText = "Share";
            this.shareDataGridViewTextBoxColumn.Name = "shareDataGridViewTextBoxColumn";
            // 
            // bsElection
            // 
            this.bsElection.DataMember = "RESULTS";
            this.bsElection.DataSource = this.dsElection;
            // 
            // dsElection
            // 
            this.dsElection.DataSetName = "dsElection";
            this.dsElection.Tables.AddRange(new System.Data.DataTable[] {
            this.DtResults});
            // 
            // DtResults
            // 
            this.DtResults.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4});
            this.DtResults.TableName = "RESULTS";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Party";
            this.dataColumn2.ColumnName = "Party";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Seats";
            this.dataColumn3.ColumnName = "Seats";
            this.dataColumn3.DataType = typeof(int);
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Share";
            this.dataColumn4.ColumnName = "Share";
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(40, 270);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 13);
            this.lblResult.TabIndex = 2;
            // 
            // ElectionWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 361);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.gvElection);
            this.Controls.Add(this.btnElectionStart);
            this.Name = "ElectionWnd";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gvElection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsElection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsElection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DtResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnElectionStart;
        private System.Windows.Forms.DataGridView gvElection;
        private System.Data.DataSet dsElection;
        private System.Data.DataTable DtResults;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn partyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn seatsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shareDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource bsElection;
    }
}

