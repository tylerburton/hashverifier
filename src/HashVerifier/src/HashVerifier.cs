//  
//  Copyright (C) 2009 Tyler Burton
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 2.1 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

// Import required libraries
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms; //use Windows.Forms for consistant look across all platforms


namespace HashVerifier
{
	
	/// <summary>
	/// HashVerifier allows you to verify file hashes and compare them to pre-known values
	/// </summary>
	public partial class HashVerifier : Form
	{
		//Global version number
		const string _VERSION = "0.2.0.0";
		
		
		//Global UI objects
		ComboBox cboHash = new ComboBox();
		Label lblResult = new Label();
		ListBox lstFileList = new ListBox();
		TextBox txtCheckHash = new TextBox();
		TextBox txtGenHash = new TextBox();
		
		/// <summary>
		/// The different one-way compression algorithms supported
		/// </summary>
		private enum HASH_NAME
		{
			MD5 = 0,
			RIPEMD160 = 1,
			SHA1 = 2,
			SHA256 = 3,
			SHA384 = 4,
			SHA512 = 5,
		}
				
		/// <summary>
		/// Static entry point
		/// </summary>
		[STAThreadAttribute]
		public static void Main()
		{
			//Start an instance of the program
			Application.Run(new HashVerifier());
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public HashVerifier()
		{
			//create the user interface
			InitializeComponent();
		}	
		
		/*
		 * --------------------------
		 * Event Handlers
		 * --------------------------
		*/

		/// <summary>
		/// Opens a file open dialog window so the user can add a new file
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default sender object
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void menuStrip_File_SelectFile_Click(object sender, EventArgs e)
		{
			//create an initialize dialog
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;
			ofd.Multiselect = true;
			ofd.Title = "Select file to add to list";
			
			//show the dialog
			ofd.ShowDialog();

			//make sure something was selected
			if(ofd.FileNames.Length > 0)
			{
				//loop through all files
				for(int i=0;i<ofd.FileNames.Length;i++)
				{
					//check if file exists
					if(File.Exists(ofd.FileNames[i]))
					   	//add file to list box
						addToFileList(ofd.FileNames);
				}
			}
		}

		/// <summary>
		/// Opens a directory open dialog window so the user can add a new directory
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default sender object
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void menuStrip_File_SelectDirectory_Click(object sender, EventArgs e)
		{
			//create an initialize dialog
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.ShowNewFolderButton = false;
			
			//show the dialog
			fbd.ShowDialog();

			if(fbd.SelectedPath.Length > 0)
			{
				if(Directory.Exists(fbd.SelectedPath))
				   {
					//store all found files from the directory
					string[] foundFiles = Directory.GetFiles(fbd.SelectedPath,"*",SearchOption.AllDirectories);
					
					//add files to list box
					addToFileList(foundFiles);
				}
			}
		}
		
		/// <summary>
		/// Closes the program down
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default object sender
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void menuStrip_File_Close_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		/// <summary>
		/// Exports a text file with all of the hashes for the selected file
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default sender object
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event arguments
		/// </param>
		private void menuStrip_Export_HashFile_SingleFile_Click(object sender, EventArgs e)
		{
			if((lstFileList.Items.Count > 0) && (lstFileList.SelectedIndex != -1))
			{
				//initialize save file dialog
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.AddExtension = true;
				sfd.CheckPathExists = true;
				sfd.CreatePrompt = false;
				sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
				sfd.SupportMultiDottedExtensions = true;
				sfd.Title = "Where do you want to save file?";
	
				//show the dialog
				sfd.ShowDialog();
	
				//create an array with all hash types
				HASH_NAME[] all = {HASH_NAME.MD5, HASH_NAME.RIPEMD160, HASH_NAME.SHA1, HASH_NAME.SHA256, HASH_NAME.SHA384, HASH_NAME.SHA512};

				//create a single element array out of selected item
				string[] temp = {(string)lstFileList.SelectedItem};
				
				//call function to write all hashes to file
				writeHashesToFile(all,sfd.FileName,temp);
			}else{
				//display an error message to the user
				MessageBox.Show("A file must be selected in order to perform an export","Error",MessageBoxButtons.OK);
			}
		}

		/// <summary>
		/// Exports a text file with all of the hashes for all of the files
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default sender object
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event arguments
		/// </param>
		private void menuStrip_Export_HashFile_AllFiles_Click(object sender, EventArgs e)
		{
			if(lstFileList.Items.Count > 0)
			{
				//initialize save file dialog
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.AddExtension = true;
				sfd.CheckPathExists = true;
				sfd.CreatePrompt = false;
				sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
				sfd.SupportMultiDottedExtensions = true;
				sfd.Title = "Where do you want to save file?";
	
				//show the dialog
				sfd.ShowDialog();
	
				//create an array with all hash types
				HASH_NAME[] all = {HASH_NAME.MD5, HASH_NAME.RIPEMD160, HASH_NAME.SHA1, HASH_NAME.SHA256, HASH_NAME.SHA384, HASH_NAME.SHA512};

				//create an array for all files
				string[] temp = new string[lstFileList.Items.Count];

				//because C# doesn't support Items.Item(index) we need to get crafty
				int i = 0;
				//loop through all object in list box
				foreach(object itm in lstFileList.Items)
				{
					//store string in array
					temp[i] = (string)itm;
					i++;
				}
			
				//call function to write all hashes to file
				writeHashesToFile(all,sfd.FileName,temp);
			}else{
				//display an error message to the user
				MessageBox.Show("There must be files in the list in order to perform an export","Error",MessageBoxButtons.OK);
			}
		}

		/// <summary>
		/// Start the system browser and load the help file
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default system object
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void menuStrip_Help_Help_Click(object sender, EventArgs e)
		{
			//check if file exists
			if(File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + "/help.htm"))
			{
				try
				{
					//start system browser to load help file
					System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) + "/help.htm");
				}
				catch(Exception)
				{
					//display error message
					MessageBox.Show("Could not start default browser","Error",MessageBoxButtons.OK);
				}
			}else
				//display error message
				MessageBox.Show("Could not find help file at: " + Path.GetDirectoryName(Application.ExecutablePath) + "/help.htm","Error",MessageBoxButtons.OK);
		}

		/// <summary>
		/// Create an about dialog describing details of the project
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default sender object
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event arguments
		/// </param>
		private void menuStrip_Help_About_Click(object sender, EventArgs e)
		{
			//create about form and initialize its settings
			Form frmAbout = new Form();
			frmAbout.BackColor = Color.White;
			frmAbout.FormBorderStyle = FormBorderStyle.FixedSingle;
			frmAbout.MaximizeBox = false;
			frmAbout.MinimizeBox = false;
			frmAbout.Size = new Size(450,145);
			frmAbout.StartPosition = FormStartPosition.CenterParent;
			frmAbout.Text = "About Hash Verifier";

			//create its layout engine and apply it to the dialog
			TableLayoutPanel tlp = new TableLayoutPanel();
			tlp.BorderStyle = BorderStyle.FixedSingle;
			tlp.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			tlp.ColumnCount = 2;
			tlp.Dock = DockStyle.Fill;
			tlp.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
			tlp.RowCount = 6;
			
			//Create the first half of the very top label
			Label lblTopLabel1 = new Label();
			lblTopLabel1.Font = new Font("Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblTopLabel1.Text = "Hash ";
			lblTopLabel1.TextAlign = ContentAlignment.MiddleRight;

			//Create the second half of the very top label
			Label lblTopLabel2 = new Label();
			lblTopLabel2.Font = new Font("Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
			lblTopLabel2.Text = "Verifier";
			lblTopLabel2.TextAlign = ContentAlignment.MiddleLeft;

			//Create the version info label
			Label lblVersionText = new Label();
			lblVersionText.Font = new Font("Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblVersionText.Text = "Version: ";
			lblVersionText.Width = 100;

			//Create the version details label
			Label lblVersionNumber = new Label();
			lblVersionNumber.Font = new Font("Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblVersionNumber.Text = _VERSION;
			lblVersionNumber.TextAlign = ContentAlignment.MiddleCenter;
			lblVersionNumber.Width = 300;

			//Create the license info label
			Label lblLicenseText = new Label();
			lblLicenseText.Font = new Font("Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblLicenseText.Text = "License: ";
			lblLicenseText.Width = 100;

			//Create the license details label
			Label lblLicenseInfo = new Label();
			lblLicenseInfo.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblLicenseInfo.Text = "Released under the GNU Lesser General Public License (LGPL)";
			lblLicenseInfo.TextAlign = ContentAlignment.MiddleCenter;
			lblLicenseInfo.Width = 300;
			
			//Create the copyright info label
			Label lblCopyrightText = new Label();
			lblCopyrightText.Font = new Font("Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblCopyrightText.Text = "Copyright: ";
			lblCopyrightText.Width = 100;

			//Create the copyright details label
			Label lblCopyrightInfo = new Label();
			lblCopyrightInfo.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblCopyrightInfo.Text = "2009 Tyler Burton";
			lblCopyrightInfo.TextAlign = ContentAlignment.MiddleCenter;
			lblCopyrightInfo.Width = 300;

			//Create the contact info label
			Label lblContactText = new Label();
			lblContactText.Font = new Font("Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblContactText.Text = "Contact: ";
			lblContactText.Width = 100;

			//Create the contact details label
			Label lblContactInfo = new Label();
			lblContactInfo.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			lblContactInfo.Text = "software@tylerburton.ca";
			lblContactInfo.TextAlign = ContentAlignment.MiddleCenter;
			lblContactInfo.Width = 300;

			//Add the labels to the layout engine control
			tlp.Controls.Add(lblTopLabel1);
			tlp.Controls.Add(lblTopLabel2);
			tlp.Controls.Add(lblVersionText);
			tlp.Controls.Add(lblVersionNumber);
			tlp.Controls.Add(lblLicenseText);
			tlp.Controls.Add(lblLicenseInfo);
			tlp.Controls.Add(lblCopyrightText);
			tlp.Controls.Add(lblCopyrightInfo);
			tlp.Controls.Add(lblContactText);
			tlp.Controls.Add(lblContactInfo);

			//Add the layout engine control to the dialog
			frmAbout.Controls.Add(tlp);

			//show the form as a dialog
			frmAbout.ShowDialog();
		}
		
		/// <summary>
		/// Displays a graphical drag/drop effect
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default object sender
		/// </param>
		/// <param name="e">
		/// A <see cref="DragEventArgs"/>
		/// The arguments for the drag event
		/// </param>
		private void lstFileList_DragEnter(object sender, DragEventArgs e)
		{
			//if the user is dragging a file over the list box
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			   e.Effect = DragDropEffects.All; //show effect
			else
				e.Effect = DragDropEffects.None; //don't show effect
		}
		
		/// <summary>
		/// Add the drag/dropped files to the listbox
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default object sender
		/// </param>
		/// <param name="e">
		/// A <see cref="DragEventArgs"/>
		/// The arguments for the drag event
		/// </param>
		private void lstFileList_DragDrop(object sender, DragEventArgs e)
		{

			//if the user dropped a file onto the list box
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				//receive and store the dropped file paths
				string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop,false);
				
				//loop through file paths, ensure they exist and add them to the list box
				for(int i=0; i<droppedFiles.Length; i++)
				{
					//check to make sure file still exists
					if(File.Exists(droppedFiles[i]))
					{
						//create a single element array to hold the value
						string[] temp = new string[1];
						temp[0] = droppedFiles[i];
						
						//add files to list box
						addToFileList(temp);
					}
					//in the case of a directory we need to search its sub-directories and return all found files
					else if(Directory.Exists(droppedFiles[i]))
					{
						//store all found files from the directory
						string[] foundFiles = Directory.GetFiles(droppedFiles[i],"*",SearchOption.AllDirectories);
						
						//add files to list box
						addToFileList(foundFiles);
					}
				}
			}
		}
		
		/// <summary>
		/// If a file in the list box is selected, display the hash hex string for that file
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// Default object sender
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void lstFileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			DisplayComputedHash(GetSelectedHash());
		}
		
		/// <summary>
		/// If the hash algorithm is changed, display the hash hex string for the currently selected file (if applicable)
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// </param>
		/// Default object sender
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void cboHash_SelectedIndexChanged(object sender, EventArgs e)
		{
			//make sure something is selected before continuing
			if(lstFileList.SelectedIndex != -1)
				DisplayComputedHash(GetSelectedHash());
		}
		
		/// <summary>
		/// Provide feedback for the user if the hashes match
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default object sender
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void txtGenHash_TextChanged(object sender, EventArgs e)
		{
			CompareHashes();
		}
		
		/// <summary>
		/// Provide feedback for the user if the hashes match
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default object sender
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void txtCheckHash_TextChanged(object sender, EventArgs e)
		{
			CompareHashes();
		}
		
		/// <summary>
		/// Copy the generated hash to the clipboard
		/// </summary>
		/// <param name="sender">
		/// A <see cref="System.Object"/>
		/// Default sender
		/// </param>
		/// <param name="e">
		/// A <see cref="EventArgs"/>
		/// Default event args
		/// </param>
		private void btnCopy_Click(object sender, EventArgs e)
		{
			//check to make sure there is something to copy first
			if(txtGenHash.Text.Length > 0){
				//copy hex string to clipboard
				if(txtGenHash.Text.Length > 0)
					Clipboard.SetText(txtGenHash.Text);
			}
			else
			{
				//show error message
				MessageBox.Show("Nothing to copy. Generate a hash by clicking on a file above first!","Error",MessageBoxButtons.OK);
			}
		}
			                    
		/*
		 * --------------------------
		 * Helper functions
		 * --------------------------
		*/
		
		/// <summary>
		/// Adds the file paths to the list box
		/// </summary>
		/// <param name="filePaths">
		/// A <see cref="System.String"/>
		/// The file paths of the list box
		/// </param>
		private void addToFileList(string[] filePaths)
		{
			//add files to list 
			lstFileList.Items.AddRange(filePaths);
		}
		
		/// <summary>
		/// Returns the enum of the currently selected hash algorithm
		/// </summary>
		/// <returns>
		/// A <see cref="HASH_NAME"/>
		/// The currently selected hash algorithm
		/// </returns>
		private HASH_NAME GetSelectedHash()
		{
			switch((string)cboHash.SelectedItem)
			{
				case "MD5":
					return HASH_NAME.MD5;
				
				case "SHA1":
					return HASH_NAME.SHA1;
					
				case "SHA256":
					return HASH_NAME.SHA256;
				
				case "SHA384":
					return HASH_NAME.SHA384;
				
				case "SHA512":
					return HASH_NAME.SHA512;
				
				case "RIPEMD-160":
					return HASH_NAME.RIPEMD160;
				
				default:
					return HASH_NAME.MD5;
				
			}
		}

		/// <summary>
		/// Compute the hash of the given file with the given hash algorithm
		/// </summary>
		/// <param name="hash">
		/// A <see cref="HASH_NAME"/>
		/// The hash to use
		/// </param>
		/// <param name="filePath">
		/// A <see cref="System.String"/>
		/// The path to the file to hash
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// The hex string of the hash
		/// </returns>
		private string computeHash(HASH_NAME hash, string filePath)
		{
			//check to make sure file exists
			if(File.Exists(filePath))
			{
				//create an inputstream for file i/o
				FileStream inputStream = null;
				
				try
				{
					//try and create a file stream around the file
					inputStream = new FileStream(filePath,FileMode.Open,FileAccess.Read);					
				
					//switch on the hash being used
					switch (hash)
					{
						case HASH_NAME.MD5:
							MD5CryptoServiceProvider MD5HashFunction = new MD5CryptoServiceProvider();
							return ConvertToHexString(MD5HashFunction.ComputeHash(inputStream));	
							
						case HASH_NAME.RIPEMD160:
							RIPEMD160Managed RIPEMD160HashFunction = new RIPEMD160Managed();
							return ConvertToHexString(RIPEMD160HashFunction.ComputeHash(inputStream));
							
						case HASH_NAME.SHA1:
							SHA1Managed SHA1HashFunction = new SHA1Managed();
							return ConvertToHexString(SHA1HashFunction.ComputeHash(inputStream));	
							
						case HASH_NAME.SHA256:
							SHA256Managed SHA256HashFunction = new SHA256Managed();
							return ConvertToHexString(SHA256HashFunction.ComputeHash(inputStream));	
						
						case HASH_NAME.SHA384:
							SHA384Managed SHA384HashFunction = new SHA384Managed();
							return ConvertToHexString(SHA384HashFunction.ComputeHash(inputStream));	
						
						case HASH_NAME.SHA512:
							SHA512Managed SHA512HashFunction = new SHA512Managed();
							return ConvertToHexString(SHA512HashFunction.ComputeHash(inputStream));
							
						default:
							//show error message
							MessageBox.Show("Invalid hash selection","Error!",MessageBoxButtons.OK);
							
							//return nothing
							return "";
					}
				}
				catch(IOException)
				{
					//show error message
					MessageBox.Show("Cannot open file for reading. Do you have permission to do so?","Error",MessageBoxButtons.OK);
	
					//return nothing
					return "";
				}
				catch(Exception)
				{
					//general error message
					MessageBox.Show("An error has occured!","Error",MessageBoxButtons.OK);
					
					//return nothing
					return "";
				}
			}else
				return "";
		}
		
		/// <summary>
		/// Computes the hash of the currently selected file and returns its hex string in the txtGenHash text box
		/// </summary>
		/// <param name="hash">
		/// A <see cref="HASH_NAME"/>
		/// The hash function to use
		/// </param>
		private void DisplayComputedHash(HASH_NAME hash)
		{
	
			//make sure something is selected
			if(lstFileList.SelectedIndex != -1)
			{	
				//extract selected file path
				string filePath = (string)lstFileList.SelectedItem;
			
				//compute hash and show result
				txtGenHash.Text = computeHash(hash,filePath);
			}	
				
		}
		
		/// <summary>
		/// Returns the hexadecimal string conversion of the byte array 
		/// </summary>
		/// <param name="input">
		/// A <see cref="System.Byte"/>
		/// The byte array to convert
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// The hexadecimal string
		/// </returns>
		private string ConvertToHexString(byte[] input)
		{
			//if the string is empty we can just return an empty string
			if(input.Length == 0)
				return "";
			else
				//return the hex string
				return BitConverter.ToString(input).Replace("-","");
		}
		
		/// <summary>
		/// Provide visual feedback to the user if the hashes match
		/// </summary>
		private void CompareHashes()
		{	
			//make sure we have something to compare
			if(txtCheckHash.Text.Length  > 0)
			{
				//the hashes match
				if(txtCheckHash.Text.ToUpper() == txtGenHash.Text.ToUpper()) 	
				{
					lblResult.BackColor = Color.Green;
					lblResult.Text = "VERIFIED";
				}
				//the hashes don't match
				else 
				{
					lblResult.BackColor = Color.Red;
					lblResult.Text = "FAILED";
				}
			}
			else
			{
				//reset defaults
				lblResult.Text = "";
				lblResult.BackColor = Color.White;
			}
		}

		/// <summary>
		/// Writes the hash string results of fileToHash of the compression functions to hashFile
		/// </summary>
		/// <param name="hashes">
		/// A <see cref="HASH_NAME"/>
		/// The hash algorithms to use
		/// </param>
		/// <param name="hashFile">
		/// A <see cref="System.String"/>
		/// The file to write the hashes to
		/// </param>
		/// <param name="filesToHash">
		/// A <see cref="System.String[]"/>
		/// The file to hash
		/// </param>
		private void writeHashesToFile(HASH_NAME[] hashes, string hashFile, string[] filesToHash)
		{
			try
			{
				//create file for writing
				TextWriter writer = new StreamWriter(hashFile);
				
				for(int i=0; i<filesToHash.Length;i++)
				{
					//in this case if the file doesn't exist we don't want to even write anything out for it
					if(File.Exists(filesToHash[i]))
					{
						//extract file name
						String fName = "";

						//depending on operating system extract the last part of the file name
						if(filesToHash[i].LastIndexOf("/") != -1)
						{
							fName = filesToHash[i].Substring(filesToHash[i].LastIndexOf("/")+1);
						}else if(filesToHash[i].LastIndexOf("\\") != -1)
						{
							fName = filesToHash[i].Substring(filesToHash[i].LastIndexOf("\\")+1);
						}else
						{
							fName = "";
						}
						
						//write the file name to the file
						writer.WriteLine("File name: " + fName);
	
						//loop through all selected hash algorithms and write out their value
						for(int j=0; j<hashes.Length;j++)
						{
							switch (hashes[j])
							{
								case HASH_NAME.MD5:
									writer.WriteLine("MD5: " + computeHash(hashes[j],filesToHash[i]));
									break;
								case HASH_NAME.RIPEMD160:
									writer.WriteLine("RIPEMD-160: " + computeHash(hashes[j],filesToHash[i]));
									break;
								case HASH_NAME.SHA1:
									writer.WriteLine("SHA1: " + computeHash(hashes[j],filesToHash[i]));
									break;
								case HASH_NAME.SHA256:
									writer.WriteLine("SHA256: " + computeHash(hashes[j],filesToHash[i]));
									break;
								case HASH_NAME.SHA384:
									writer.WriteLine("SHA384: " + computeHash(hashes[j],filesToHash[i]));
									break;
								case HASH_NAME.SHA512:
									writer.WriteLine("SHA512: " + computeHash(hashes[j],filesToHash[i]));
									break;
								default:
									//show error message
									MessageBox.Show("Invalid hash selection","Error!",MessageBoxButtons.OK);
									break;
							}
						}
					}
					//if its not the last file print out a blank newline
					if(i != filesToHash.Length-1)
						writer.WriteLine("");
				}
				
				//close file
				writer.Close();

			}catch(IOException)
			{
				//display error to user
				MessageBox.Show("Cannot write to " + hashFile + ". Do you have permission to do so?","Error",MessageBoxButtons.OK);
			}catch(Exception)
			{
				//display error to user
				MessageBox.Show("An error has occured!","Error",MessageBoxButtons.OK);
			}
		}
	}

}