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

//import libraries
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HashVerifier
{
	
	
	public partial class HashVerifier
	{
	
		/// <summary>
		/// Use Windows.Forms to setup the user interface
		/// </summary>
		/// <remarks>Should only be called once</remarks>
		private void InitializeComponent()
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
			this.Size = new Size(645, 282);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Hash Verifier";

			/*
			 * --------------------------
			 * Define menu strip
			 * --------------------------
			 */
			MenuStrip menuStrip = new MenuStrip();
			menuStrip.Location = new Point(0,0);
			
			//add menu to form
			this.Controls.Add(menuStrip);

			/*
			 * --------------------------
			 * Define menuStrip file menu
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_File = new ToolStripMenuItem("File");
			menuStrip.Items.Add(menuStrip_File);

			/*
			 * --------------------------
			 * Define file menu select file option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_File_SelectFile = new ToolStripMenuItem("Select File");
			menuStrip_File_SelectFile.ShortcutKeys = Keys.Control | Keys.O;
			menuStrip_File.DropDownItems.Add(menuStrip_File_SelectFile);

			//setup event handler
			menuStrip_File_SelectFile.Click += new EventHandler(menuStrip_File_SelectFile_Click);

			/*
			 * --------------------------
			 * Define file menu select directory option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_File_SelectDirectory = new ToolStripMenuItem("Select Directory");
			menuStrip_File_SelectDirectory.ShortcutKeys = Keys.Control | Keys.D;
			menuStrip_File.DropDownItems.Add(menuStrip_File_SelectDirectory);

			//setup event handler
			menuStrip_File_SelectDirectory.Click += new EventHandler(menuStrip_File_SelectDirectory_Click);
			
			/*
			 * --------------------------
			 * Define file menu close option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_File_Close = new ToolStripMenuItem("Close");
			menuStrip_File.DropDownItems.Add(menuStrip_File_Close);

			//setup event handlers
			menuStrip_File_Close.Click += new EventHandler(menuStrip_File_Close_Click);

			/*
			 * --------------------------
			 * Define menuStrip export menu
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Export = new ToolStripMenuItem("Export");
			menuStrip.Items.Add(menuStrip_Export);

			/*
			 * --------------------------
			 * Define export menu hash file option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Export_HashFile = new ToolStripMenuItem("Export to Hash File");
			menuStrip_Export.DropDownItems.Add(menuStrip_Export_HashFile);

			/*
			 * --------------------------
			 * Define export -> hash file menu single file option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Export_HashFile_SingleFile = new ToolStripMenuItem("Single File");
			menuStrip_Export_HashFile.DropDownItems.Add(menuStrip_Export_HashFile_SingleFile);

			//setup event handlers
			menuStrip_Export_HashFile_SingleFile.Click += new EventHandler(menuStrip_Export_HashFile_SingleFile_Click);

			/*
			 * --------------------------
			 * Define export -> hash file menu all files option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Export_HashFile_AllFiles = new ToolStripMenuItem("All Files");
			menuStrip_Export_HashFile.DropDownItems.Add(menuStrip_Export_HashFile_AllFiles);

			//setup event handlers
			menuStrip_Export_HashFile_AllFiles.Click += new EventHandler(menuStrip_Export_HashFile_AllFiles_Click);

			/*
			 * --------------------------
			 * Define menuStrip help menu
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Help = new ToolStripMenuItem("Help");
			menuStrip.Items.Add(menuStrip_Help);

			/*
			 * --------------------------
			 * Define help -> help option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Help_Help = new ToolStripMenuItem("Help");
			menuStrip_Help.DropDownItems.Add(menuStrip_Help_Help);
			menuStrip_Help_Help.ShortcutKeys = Keys.F1;

			//setup event handlers
			menuStrip_Help_Help.Click += new EventHandler(menuStrip_Help_Help_Click);

			/*
			 * --------------------------
			 * Define help -> about option
			 * --------------------------
			 */
			ToolStripMenuItem menuStrip_Help_About = new ToolStripMenuItem("About");
			menuStrip_Help.DropDownItems.Add(menuStrip_Help_About);

			//setup event handlers
			menuStrip_Help_About.Click += new EventHandler(menuStrip_Help_About_Click);
			
			/*
			 * --------------------------
			 * Define copy to clipboard button
			 * --------------------------
			 */
			Button btnCopy = new Button();
			btnCopy.FlatStyle = FlatStyle.Flat;
			btnCopy.Font = new Font("Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			btnCopy.Location = new Point(498, 176);
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
        	cboHash.Location = new Point(393, 177);
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
        	lblCheckHashInfo.Location = new Point(3, 201);
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
        	lblDragInfo.Location = new Point(3, 33);
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
        	lblGenHashInfo.Location = new Point(3, 161);
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
        	lblResult.Location = new Point(518, 218);
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
        	lstFileList.Location = new Point(3, 50);
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
        	txtCheckHash.Location = new Point(3, 221);
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
        	txtGenHash.Location = new Point(3, 178);
        	txtGenHash.ReadOnly = true;
        	txtGenHash.Size = new Size(381, 21);
        	txtGenHash.TabIndex = 1;
				
			//setup event handlers
			txtGenHash.TextChanged += new EventHandler(txtGenHash_TextChanged);
			
			//add button to form
			this.Controls.Add(txtGenHash);
			
		}
	}
}