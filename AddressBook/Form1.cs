using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AddressBook
{
    public partial class Form1 : Form
    {
        static List<Contact> contacts = new List<Contact>();
        private Contact contact;

        private string filePath = @"./Data/Contacts.dat"; //@".\Data\Contacts.dat";
        private string folderPath = @"./Data/";

        // used by sort
        private int sortColumn = -1;



        public Form1()
        {
            InitializeComponent();
            

            bool initLoad = InitContactsList();

            PollAndUpdateListView();

            if (initLoad == false)
            {
                MessageBox.Show("File doesnt exist yet"); // "init load function failed to load properly");

            }


            // enter key will click save
            this.AcceptButton = btnSave;
        }





        private void UpdateStateInfo(Color color, string displayedText)
        {
            // update app state for user
            lblState.BackColor = color;
            lblState.Text = displayedText;
        }





        private void SetNewListValues(int idCount, string[] data)
        {            

            Contact newContact = new Contact
            {
                ID  = idCount,
                FirstName = data[0],
                LastName = data[1],
                Address = data[2]
            };


            contacts.Add(newContact);            
        }





        public bool InitContactsList()
        {
            try
            {
                // update app state for user
                UpdateStateInfo(Color.White, "Loading Data....");



                
                bool LoadFromFile = LoadList(true);


                if (LoadFromFile == false)
                    return false; // could add a logging feature in the future.


                // update app state for user
                UpdateStateInfo(Color.White, "Data loading complete");
                
            }
            catch (Exception)// e)
            {
                // update app state for user
                UpdateStateInfo(Color.Orange, "File not found yet...");

                // log file data
                // MessageBox.Show("error \"InitContactsList\": " + e.Message + "  , Error MSG: " + e.ToString());

                return false;
            }

            return true;
        }





        public bool SaveList()
        {
            try
            {
                // create save folder if it doesnt exsit else it skips it
                string physicalPath = AppDomain.CurrentDomain.BaseDirectory;
                string directory = Path.Combine(physicalPath, folderPath);
                Directory.CreateDirectory(directory); // no need to check if it exists



                // could add a binary saving feature in the future
                FileStream saveFile = new FileStream(filePath, FileMode.Create);
                StreamWriter fileWriter = new StreamWriter(saveFile);


                // update app state for user
                UpdateStateInfo(Color.White, "Saving Data...");


                // iter contacts list
                foreach (Contact person in contacts)
                {
                    fileWriter.Write(person.ID.ToString() + "," + person.FirstName + "," + person.LastName + "," + person.Address + "\n");
                }



                fileWriter.Close();
                saveFile.Close();

                
                // update app state for user
                UpdateStateInfo(Color.White, "Successfully Saved Data...");
            }
            catch (Exception) // e)
            {
                // update app state for user
                UpdateStateInfo(Color.Red, "Error Saving Data...");

                // log file data
                //MessageBox.Show("error Saving list: " + e.Message + "\n\n\nError MSG: " + e.ToString());

                return false;
            }


            PollAndUpdateListView();

            return true;
        }





        public bool LoadList(bool isInit)
        {
            try
            {
                // update app state for user
                UpdateStateInfo(Color.White, "Loading Data...");


                // used only on app start up for loading from file
                if(isInit == false)
                    contacts.Clear();




                FileStream openFile = new FileStream(filePath, FileMode.Open);
                StreamReader fileReader = new StreamReader(openFile);



                while (fileReader.EndOfStream == false)
                {
                    string[] token = fileReader.ReadLine().Split(',');


                    Contact person = new Contact();
                    person.ID = Convert.ToInt32(token[0]);
                    person.FirstName = token[1];
                    person.LastName = token[2];
                    person.Address = token[3];


                    contacts.Add(person);
                }


                fileReader.Close();
                openFile.Close();


                // update app state for user
                UpdateStateInfo(Color.White, "Successfully Loaded Data...");

            }
            catch (Exception) // e)
            {
                // update app state for user
                UpdateStateInfo(Color.Yellow, "Error Loading Data...");

                // log file data
                // MessageBox.Show("Error loading list: " + e.Message + "\n\n\nError MSG: " + e.ToString());

                return false;
            }


            return true;
        }





        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            listView.Enabled = false;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnAddPerson.Enabled = false;

            txtBoxFirstName.Enabled = true;
            txtBoxLastName.Enabled = true;
            txtBoxAddress.Enabled = true;

            // always add new contact to the next Unused ID
            lblID.Text = (listView.Items.Count + 1).ToString();

            //txtBoxFirstName.Select();
            this.txtBoxFirstName.Focus();
        }





        private void UpdateInputFieldsWithSelected(string id, string firstName, string lastName, string address)
        {
            lblID.Text = id;
            txtBoxFirstName.Text = firstName;
            txtBoxLastName.Text = lastName;
            txtBoxAddress.Text = address;
        }




        private void SetUserInputDefaults()
        {
            UpdateInputFieldsWithSelected("-1", "", "", "");


            listView.Enabled = true;
            btnAddPerson.Enabled = true;    
            btnCancel.Enabled = false;
            btnRemove.Enabled = false;
            btnSave.Enabled = false;

            txtBoxFirstName.Enabled = false;
            txtBoxLastName.Enabled = false;
            txtBoxAddress.Enabled = false;
        }





        private void btnCancel_Click(object sender, EventArgs e)
        {
            // clear user input form
            SetUserInputDefaults();

            

            listView.Enabled = true;

            // update app state for user
            UpdateStateInfo(Color.White, "Cleared entered Data");

        }





        private void btnRemove_Click(object sender, EventArgs e)
        {
            // remove from listview selected object.
            listView.SelectedItems[0].Remove();

            // update id's on any object after this one
            // save new list format
            int counter = 1;


            contacts.Clear();


            foreach (ListViewItem item in listView.Items)
            {
                string[] newContact = { item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[3].Text };               

                SetNewListValues(counter, newContact);

                counter++;
            }

            listView.Items.Clear();

            PollAndUpdateListView();


            // clear selected contact information to unselected defaults
            SetUserInputDefaults();

            
            btnRemove.Enabled = false;


            // update app state for user
            UpdateStateInfo(Color.Yellow, "Removed Selected Data");



            // resave data
            bool saveList = SaveList();

            if (saveList == false)
            {
                MessageBox.Show("save file failed");
            }
        }





        private void PollAndUpdateListView()
        {
            listView.Items.Clear();

            foreach (Contact person in contacts)
            {
                AddPersonToListView(person.ID.ToString(), person.FirstName, person.LastName, person.Address);                
            }
        }





        private void AddPersonToListView(string id, string firstName, string lastName, string address)
        {
            string[] personRow = { id, firstName, lastName, address };

            ListViewItem item = new ListViewItem(personRow);

            listView.Items.Add(item);
        }





        private void btnSave_Click(object sender, EventArgs e)
        {
            // if you select a user in the list the information is displayed. ** NOTE ** Multiple selected users is not implamented at this stage and may not due to limited interface setup currently
            if(listView.SelectedItems.Count > 0)
            {
                string[] selectedPerson = { listView.SelectedItems[0].SubItems[0].Text, // ID number
                                            listView.SelectedItems[0].SubItems[1].Text, // first name
                                            listView.SelectedItems[0].SubItems[2].Text, // last name
                                            listView.SelectedItems[0].SubItems[3].Text }; // address

                if (selectedPerson[0].ToString() == lblID.Text)
                {
                    // we are using an exsiting user
                    // check each box with selected person data if they are different save new data over old.
                    string[] checkData = { lblID.Text, txtBoxFirstName.Text, txtBoxLastName.Text, txtBoxAddress.Text };
                    bool testData = true;

                    for (int i = 1; i < selectedPerson.Length; i++)
                    {
                        if (selectedPerson[i] == checkData[i] == false)
                        {
                            // easyer to read then if(!true)
                            testData = false;
                        }
                    }

                    if (testData)
                    {
                        SetUserInputDefaults();

                        return;
                    }                        
                    else
                    {
                        // verify new data isnt null or empty
                        for (int i = 0; i < selectedPerson.Length; i++)
                        {
                            if (checkData[i] == "")
                            {
                                // update app state for user
                                UpdateStateInfo(Color.Orange, "ERROR! Complete the form!");
                                
                                return;
                            }
                                
                        }

                        // write new data to old object
                        for (int i = 0; i < selectedPerson.Length; i++)
                        {
                            listView.SelectedItems[0].SubItems[i].Text = checkData[i];
                        }

                        // update app state for user
                        UpdateStateInfo(Color.White, "Contact Updated...");
                    }

                    SetUserInputDefaults();


                    return;
                }
            }


            


            // check all text boxes for values
            // if any value == "" or NULL
            // return;
            if (txtBoxFirstName.Text == "" || txtBoxLastName.Text == "" || txtBoxAddress.Text == "")
            {
                // update app state for user
                UpdateStateInfo(Color.Yellow, "ERROR! Complete the form!");


                return;
            }

            // update app state for user
            UpdateStateInfo(Color.White, "New Contact Saved");


            
            // else add new contact
            contact = new Contact();

            contact.ID = int.Parse(lblID.Text);
            contact.FirstName = txtBoxFirstName.Text;
            contact.LastName = txtBoxLastName.Text;
            contact.Address = txtBoxAddress.Text;

            contacts.Add(contact);

 

            // save list to dat file
            // save binary file future feature possibly
            bool saveList = SaveList();

            if (saveList == false)
            {
                MessageBox.Show("save file failed");
            }


            // clear object
            SetUserInputDefaults();
        }





        private void ListView_MouseClick(object sender, MouseEventArgs e)
        {
            string id = listView.SelectedItems[0].SubItems[0].Text;
            string firstName = listView.SelectedItems[0].SubItems[1].Text;
            string lastName = listView.SelectedItems[0].SubItems[2].Text;
            string address = listView.SelectedItems[0].SubItems[3].Text;


            UpdateInputFieldsWithSelected(id, firstName, lastName, address);
            
            btnAddPerson.Enabled = false;
            btnCancel.Enabled = true;
            btnRemove.Enabled = true;
            btnSave.Enabled = true;

            txtBoxFirstName.Enabled = true;
            txtBoxLastName.Enabled = true;
            txtBoxAddress.Enabled = true;
        }





        private void ListView_ColumnClickSort(object sender, ColumnClickEventArgs e)
        {
            // is the column the same as before?
            if (e.Column != sortColumn)
            {
                // set the sort column to the new column
                sortColumn = e.Column;

                // set the sort order to ascending by default.
                listView.Sorting = SortOrder.Ascending;
            }
            else
            {
                // determine what last sort order was and update
                if (listView.Sorting == SortOrder.Ascending)
                    listView.Sorting = SortOrder.Descending;
                else
                    listView.Sorting = SortOrder.Ascending;
            }

            // manual sort
            listView.Sort();

            // set the ListViewItemSorter property to the new listviewitemcomparer object
            this.listView.ListViewItemSorter = new ListViewItemComparer(e.Column, listView.Sorting);
        }
    }
}
