using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;
using System.Threading;
using System.Data;
using System.Net.Mail;
using System.Windows.Forms;


namespace Election
{


    class ElectionProcess
    {

        // Sub class that contains current total results for a particular PARTY
        class PartyResultValues
        {
            private int seatCount;

            public int SeatCount
            {
                get { return seatCount; }
                set { seatCount = value; }
            }
            private int voteCount;

            public int VoteCount
            {
                get { return voteCount; }
                set { voteCount = value; }
            }

            public PartyResultValues(int _seatCount, int _voteCount)
            {
                SeatCount = _seatCount;
                VoteCount = _voteCount;
            }
        }

        // specific file location constants to run this exercise
        const string PROCESS_DIRECTORY = @"C:\election\process";
        const string DONE_DIRECTORY = @"C:\election\done";
        const string FAILED_DIRECTORY = @"C:\election\failed";
        const string ELECTION_XSD = @"c:\election\election.xsd";

        // email specific constants
        const string EMAIL_FROM = "electionprocess@gmail.com";
        const string EMAIL_SUPERVISOR = "dmundy@sintecmedia.com";
        const string EMAIL_HOST = "smtp.googlemail.com";
        const int EMAIL_PORT = 587;
        const string EMAIL_ADDRESS_SUPERVISOR = "dpmundy120@googlemail.com";
        const string EMAIL_ADDRESS_PASSWORD = "xxxxxxx";
        
        // other constants
        const int MAJORITY_SEATS = 326;
        const string ELECTION_XSD_NODES = @"/constituencyResults/constituencyResult/results/result";
        const string PARTY_CODE_NODE = "partyCode";
        const string VOTES_NODE = "votes";
        public const string RESULTS_TABLE = "RESULTS";
        const string SHARE_COLUMN = "share";
        const string PARTY_COLUMN = "party";
        const string SEATS_COLUMN = "seats";
        const string TWO_DECIMAL_PLACES_FORMAT = "n2";
        const string OTHER_CODE = "OTHER";
        const string ERROR_EMAIL_SUBJECT = "Problem with Election ingest of file: ";
        const string PROBLEM_EMAIL = "Problem with email setup, email subject for supervisor would be: \n\n";

        // poll with 5 second sleep
        const int POLL_SLEEP = 5000; 
    
        public  delegate void MajorityFoundDelegate(string winner);
        public delegate void RefreshGUIDelegate(DataSet ds);

        // reference of GUI procedure to run once Majority has found
        MajorityFoundDelegate MajorityFound;
        // reference of GUI procedure to run to refresh grid after an xml loaded
        RefreshGUIDelegate RefreshGUI;
        // Our GUI form. Save reference so that we can safely invoke delegates from a thread
        Form GuiForm;

        // seat count/vote count totals stored for each distinct Party Code
        Dictionary<string, PartyResultValues> partyResults = new Dictionary<string, PartyResultValues>();
        int totalVotes;
        bool majorityFound = false;

        public int TotalVotes
        {
            get { return totalVotes; }
            set { totalVotes = value; }
        }

        System.Data.DataSet electionDataSet;

        // GUI dataset to present top 4 results in order
        public System.Data.DataSet ElectionDataSet
        {
            get { return electionDataSet; }
            set { electionDataSet = value; }
        }

        // constructor record Gui dataset, delegates to update Gui form
        public ElectionProcess(System.Data.DataSet ds, MajorityFoundDelegate majorityFound , RefreshGUIDelegate refreshGUIDelegate, Form guiForm) {
            ElectionDataSet = ds.Clone();
            MajorityFound = majorityFound;
            RefreshGUI = refreshGUIDelegate;
            GuiForm = guiForm;
        }

        // Email the supervisor via GMail. If this is not setup then show the functionality works in concept by showing a simple message box.
        public void EmailSupervisorFailed(string xmlPath)
        {
            try
            {
                MailMessage mail = new MailMessage(EMAIL_FROM, EMAIL_SUPERVISOR);
                SmtpClient client = new SmtpClient();
                client.Port = 587222;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = EMAIL_HOST;
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(EMAIL_ADDRESS_SUPERVISOR, EMAIL_ADDRESS_PASSWORD);
                mail.Subject = ERROR_EMAIL_SUBJECT + xmlPath;
                mail.Body = "";
                client.Send(mail);
            } catch(Exception ex) {
                MessageBox.Show(PROBLEM_EMAIL + ERROR_EMAIL_SUBJECT+xmlPath);
            }
        }

        /* Add a result to Gui Dataset */
        public void AddElectionDataSetRow(string partyCode, int seats, double share)
        {
            DataRow newElectionRow = ElectionDataSet.Tables[RESULTS_TABLE].NewRow();
            newElectionRow[PARTY_COLUMN] = partyCode;
            newElectionRow[SEATS_COLUMN] = seats;
            newElectionRow[SHARE_COLUMN] = share.ToString(TWO_DECIMAL_PLACES_FORMAT);
            ElectionDataSet.Tables[RESULTS_TABLE].Rows.Add(newElectionRow);
        }

        // Recalculate GUI Dataset for top 3 parties and others.
        // Calculate via proxy from our working dictionary of party results
        // Show special label if majority party found

        public void RefreshElectionDataSet()
        {
            int partyCount = 0; 
            int otherSeats = 0;
            double totalShare = 0.0;
            double currentShare;

            ElectionDataSet.Tables[RESULTS_TABLE].Clear();
    
            // order our working dictionary and put results into dataset usable by our GUI
            foreach(var partyResult in partyResults.OrderByDescending(p => p.Value.SeatCount)) {
                partyCount++;
                // if we have gone past 3 parties then all others results can be aggregate for other
                if (partyCount > 3)
                {
                    otherSeats += partyResult.Value.SeatCount;
                }
                else
                {
                    // work out top 3 results and put in the gui record 
                    currentShare = (double)partyResult.Value.VoteCount * 100.0 / TotalVotes;
                    AddElectionDataSetRow(partyResult.Key, partyResult.Value.SeatCount, currentShare);
                    totalShare += currentShare;
                    if (partyResult.Value.SeatCount > MAJORITY_SEATS && !majorityFound)
                     {
                        majorityFound = true;
                        // show majority found message in GUI
                        GuiForm.Invoke(new MethodInvoker(() => MajorityFound(partyResult.Key)));
                    }
                }
            }

            // record fourth Other category for remaining parties
            if (partyCount > 3)
            {
                // calculate share of others as remaining from 100.0, avoids slight rounding issues
                AddElectionDataSetRow(OTHER_CODE, otherSeats, 100.0 - totalShare);
            }

            // Thread safe method to call refresh delegate to inform grid that changes have been made 
            // copy of election dataset used as threads should not modify datastructures of another process
            DataSet electionDataSetCopy = ElectionDataSet.Copy();
            GuiForm.BeginInvoke(RefreshGUI, electionDataSetCopy);
        }

        // process and load all xml files currently in the Process Directory
        // no file validation! could also be extended to also check for duplicate records to record as errors/overwrite
        public void ProcessXmlFiles() {
            bool valid;
            string fileTo;
            string fileName;
            string[] filePaths = Directory.GetFiles(PROCESS_DIRECTORY);

            foreach (string filePath in filePaths) {
                fileName = Path.GetFileName(filePath);
                valid = ProcessXmlFile(filePath);
                if (valid) {
                  fileTo = DONE_DIRECTORY+@"\"+fileName;
                } else {
                  fileTo = FAILED_DIRECTORY+@"\"+fileName;
                  EmailSupervisorFailed(filePath);
                }
                // Move file to done/failed directory
         
                try
                {
                    File.Move(filePath, fileTo);
                } 
                catch (IOException)
                {
                    // simple validation -- ideally record errors and more checking
                    // most simple case is that done/failed already contains file from previous run.
                    if (File.Exists(fileTo))
                      File.Delete(filePath);
                 }
            }
        }

        // process one xml file and update GUI
        public bool ProcessXmlFile(string xmlPath)
        {
            string partyCode;
            int votes;
         
            bool isFirstResult = true; // first Party result in file
            bool isWinner = false;     // Is current party the winner

            XmlDocument regionFile = new XmlDocument();
          
            // Make sure xml is valid according to schema
           
            if (!LoadValidateSchema(ref regionFile, xmlPath))
                return false;

            // find each party result node and process votes/seat winner
            // we could record everything for a full production system e.g. constituency info. just for this exercise just record from xml what we need.
            XmlNodeList nodes = regionFile.DocumentElement.SelectNodes(ELECTION_XSD_NODES);
     
            foreach (XmlNode node in nodes)
            {
                isWinner = isFirstResult;

                partyCode = node.SelectSingleNode(PARTY_CODE_NODE).InnerText;
                // loads votes. No entry indicates zero votes. In actual system may consider this an error and raise appropriate failure as required.
                if (node.SelectSingleNode(VOTES_NODE) != null)
                    votes = Int32.Parse(node.SelectSingleNode(VOTES_NODE).InnerText);
                else
                    votes = 0;
    
                /* update our dictionary with current party information found in XML */
                if (partyResults.ContainsKey(partyCode))
                {
                    PartyResultValues currPartyResultValues = partyResults[partyCode];
                    partyResults.Remove(partyCode);
                    partyResults[partyCode] = new PartyResultValues(currPartyResultValues.SeatCount + (isWinner ? 1 : 0), currPartyResultValues.VoteCount + votes);
                } else {
                    partyResults[partyCode] = new PartyResultValues( (isWinner ? 1 : 0), votes);
                }
                TotalVotes += votes;
                isFirstResult = false;
            }
            /* Refresh GUI dataset */
            RefreshElectionDataSet();
       
            return true;
        }

        // Validate xml document against XSD schema using simplistic boolean approach. Could be extended to give actual error location to send to supervisor.
        // The XSD I created allows PartyCode with no votes/share and assumes the votes here is zero. So should allow loading of all supplied XML.

        public bool LoadValidateSchema(ref XmlDocument xml, string xmlPath)
        {
            // a very rough and ready validation of XML Loading
            try
            {
                xml.Load(xmlPath);
                xml.Schemas.Add(null, ELECTION_XSD);
            }
            catch (Exception ex)
            {
                return false;
            }
    
            // file has loaded now do full schema validation
            try
            {
                xml.Validate(null);
            }
            catch (XmlSchemaValidationException)
            {
                return false;
            }
            return true;
        }

        // Main Election ingest routine to poll for xml files and load
        public void ElectionStart()
        {
            while (true)
            {
                ProcessXmlFiles();
                Thread.Sleep(POLL_SLEEP);
            }

       }
    }
}
