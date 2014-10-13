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
	public class HashVerifier : Form
	{
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
			SHA1 = 1,
			SHA256 = 2,
			SHA384 = 3,
			SHA512 = 4,
			RIPEMD160 = 5,
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
			SetupUI();
		}
		
		/// <summary>
		/// Use Windows.Forms to setup the user interface
		/// </summary>
		/// <remarks>Should only be called once</remarks>
		private void SetupUI()
		{
			/*
			 * --------------------------
			 * Define form properties
			 * --------------------------
			 */
			this.BackColor = Color.White;
			this.DoubleBuffered = true;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Size = new System.Drawing.Size(645, 258);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Hash Verifier";
			
			/*
			 * --------------------------
			 * Define copy to clipboard button
			 * --------------------------
			 */
			Button btnCopy = new Button();
			btnCopy.FlatStyle = FlatStyle.Flat;
			btnCopy.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnCopy.Location = new Point(498, 152);
        	btnCopy.Size = new Size(128, 22);
        	btnCopy.TabIndex = 3;
			btnCopy.Text = "Copy Hash";
        	btnCopy.UseVisualStyleBackColor = true;
			
			//setup event handlers
			btnCopy.Click += new EventHandler(btnCopy_Click);
			
			//add button to form
			this.Controls.Add(btnCopy);
			
			/*
			 * --------------------------
			 * Define hash selector combo box
			 * --------------------------
			 */
			cboHash.BackColor = Color.White;
        	cboHash.DropDownStyle = ComboBoxStyle.DropDownList;
        	cboHash.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	cboHash.FormattingEnabled = true;
        	cboHash.Location = new Point(393, 153);
        	cboHash.Size = new Size(99, 21);
        	cboHash.TabIndex = 2;
			
			//add combo box options
			cboHash.Items.Add("MD5");
			cboHash.Items.Add("RIPEMD-160");
			cboHash.Items.Add("SHA1");
			cboHash.Items.Add("SHA256");
			cboHash.Items.Add("SHA384");
			cboHash.Items.Add("SHA512");
			cboHash.SelectedIndex = 0;
			
			//setup event handlers
			cboHash.SelectedIndexChanged += new EventHandler(cboHash_SelectedIndexChanged);
			
			//add combo box to form
			this.Controls.Add(cboHash);
			
			/*
			 * --------------------------
			 * Define hash to check against information label
			 * --------------------------
			 */
			Label lblCheckHashInfo = new Label();
			lblCheckHashInfo.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	lblCheckHashInfo.Location = new Point(3, 177);
        	lblCheckHashInfo.Size = new Size(506, 21);
        	lblCheckHashInfo.Text = "Hash to check against:";
			
			//add label to form
			this.Controls.Add(lblCheckHashInfo);
			
			/*
			 * --------------------------
			 * Define top drag information label properties
			 * --------------------------
			 */
			Label lblDragInfo = new Label();
			lblDragInfo.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	lblDragInfo.Location = new Point(3, 9);
        	lblDragInfo.Size = new Size(161, 14);
        	lblDragInfo.Text = "Drag file(s) into here to begin:";
			
			//add label to form
			this.Controls.Add(lblDragInfo);
			
			/*
			 * --------------------------
			 * Define generated hash information label
			 * --------------------------
			 */
			Label lblGenHashInfo = new Label();
			lblGenHashInfo.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	lblGenHashInfo.Location = new Point(3, 137);
        	lblGenHashInfo.Size = new Size(100, 14);
        	lblGenHashInfo.Text = "Hash produced:";
			
			//add label to form
			this.Controls.Add(lblGenHashInfo);
			
			/*
			 * --------------------------
			 * Define result label
			 * --------------------------
			 */
			lblResult.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	lblResult.ForeColor = Color.White;
        	lblResult.Location = new Point(518, 194);
        	lblResult.Size = new Size(108, 21);
        	lblResult.TextAlign = ContentAlignment.MiddleCenter;
			
			//add label to form
			this.Controls.Add(lblResult);
			
			/*
			 * --------------------------
			 * Define file list box
			 * --------------------------
			 */
			lstFileList.AllowDrop = true;
        	lstFileList.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	lstFileList.FormattingEnabled = true;
        	lstFileList.HorizontalScrollbar = true;
        	lstFileList.Location = new Point(3, 26);
			lstFileList.ScrollAlwaysVisible = false;
        	lstFileList.Size = new Size(623, 108);
			
			//setup event handlers
			lstFileList.DragEnter += new DragEventHandler(lstFileList_DragEnter);
			lstFileList.DragDrop += new DragEventHandler(lstFileList_DragDrop);
			lstFileList.SelectedIndexChanged += new EventHandler(lstFileList_SelectedIndexChanged);
			
			//add list box to form
			this.Controls.Add(lstFileList);
			
			/*
			 * --------------------------
			 * Define hash to check against text field
			 * --------------------------
			 */
			txtCheckHash.BackColor = Color.White;
        	txtCheckHash.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	txtCheckHash.Location = new Point(3, 197);
			txtCheckHash.ReadOnly = false;
        	txtCheckHash.Size = new Size(506, 21);
        	txtCheckHash.TabIndex = 0;
			
			//setup event handlers
			txtCheckHash.TextChanged += new EventHandler(txtCheckHash_TextChanged);

			//add button to form
			this.Controls.Add(txtCheckHash);
			
			/*
			 * --------------------------
			 * Define generated hash text field
			 * --------------------------
			 */
			txtGenHash.BackColor = Color.White;
        	txtGenHash.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
        	txtGenHash.Location = new Point(3, 154);
        	txtGenHash.ReadOnly = true;
        	txtGenHash.Size = new Size(381, 21);
        	txtGenHash.TabIndex = 1;
				
			//setup event handlers
			txtGenHash.TextChanged += new EventHandler(txtGenHash_TextChanged);
			
			//add button to form
			this.Controls.Add(txtGenHash);
			
		}
		
		/*
		 * --------------------------
		 * Event Handlers
		 * --------------------------
		*/
		
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
						//add files to list box
						lstFileList.Items.Add(droppedFiles[i]);
					}
					//in the case of a directory we need to search its sub-directories and return all found files
					else if(Directory.Exists(droppedFiles[i]))
					{
						//store all found files from the directory
						string[] foundFiles = Directory.GetFiles(droppedFiles[i],"*",SearchOption.AllDirectories);
						
						//add files to list box
						lstFileList.Items.AddRange(foundFiles);
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
				
				//make sure the file still exists
				if(File.Exists(filePath))
				{
					//create an inputstream for file i/o
					FileStream inputStream = null;
					
					try
					{
						//try and create a file stream around the file
						inputStream = new FileStream(filePath,FileMode.Open,FileAccess.Read);
					}
					catch(Exception)
					{
						//show error message
						MessageBox.Show("Cannot open file for reading. Do you have permission to do so?","Error",MessageBoxButtons.OK);
					}					
					
					//switch on the hash being used
					switch (hash)
					{
						case HASH_NAME.MD5:
							MD5CryptoServiceProvider MD5HashFunction = new MD5CryptoServiceProvider();
							txtGenHash.Text = ConvertToHexString(MD5HashFunction.ComputeHash(inputStream));	
							break;
							
						case HASH_NAME.RIPEMD160:
							RIPEMD160Managed RIPEMD160HashFunction = new RIPEMD160Managed();
							txtGenHash.Text = ConvertToHexString(RIPEMD160HashFunction.ComputeHash(inputStream));
							break;
							
						case HASH_NAME.SHA1:
							SHA1Managed SHA1HashFunction = new SHA1Managed();
							txtGenHash.Text = ConvertToHexString(SHA1HashFunction.ComputeHash(inputStream));	
							break;
							
						case HASH_NAME.SHA256:
							SHA256Managed SHA256HashFunction = new SHA256Managed();
							txtGenHash.Text = ConvertToHexString(SHA256HashFunction.ComputeHash(inputStream));	
							break;
						
						case HASH_NAME.SHA384:
							SHA384Managed SHA384HashFunction = new SHA384Managed();
							txtGenHash.Text = ConvertToHexString(SHA384HashFunction.ComputeHash(inputStream));	
							break;
						
						case HASH_NAME.SHA512:
							SHA512Managed SHA512HashFunction = new SHA512Managed();
							txtGenHash.Text = ConvertToHexString(SHA512HashFunction.ComputeHash(inputStream));	
							break;
							
						default:
							MessageBox.Show("Invalid hash selection","Error!",MessageBoxButtons.OK);
							break;
					}
				}else
					//Show error message
					MessageBox.Show("File not found. Was it moved or deleted?","Error",MessageBoxButtons.OK);
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
		
	}
}

