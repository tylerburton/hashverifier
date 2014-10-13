//  
//  Copyright (C) 2010 Tyler Burton
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

package ca.tylerburton.hashverifier;

/*
 * Import required libraries
 */
import org.eclipse.swt.SWT;
import org.eclipse.swt.dnd.Clipboard;
import org.eclipse.swt.dnd.DND;
import org.eclipse.swt.dnd.DropTarget;
import org.eclipse.swt.dnd.DropTargetEvent;
import org.eclipse.swt.dnd.DropTargetListener;
import org.eclipse.swt.dnd.FileTransfer;
import org.eclipse.swt.dnd.TextTransfer;
import org.eclipse.swt.dnd.Transfer;
import org.eclipse.swt.events.ModifyEvent;
import org.eclipse.swt.events.ModifyListener;
import org.eclipse.swt.events.SelectionEvent;
import org.eclipse.swt.events.SelectionListener;
import org.eclipse.swt.graphics.Color;
import org.eclipse.swt.graphics.RGB;
import org.eclipse.swt.graphics.Rectangle;
import org.eclipse.swt.layout.GridData;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.program.Program;
import org.eclipse.swt.widgets.Button;
import org.eclipse.swt.widgets.Combo;
import org.eclipse.swt.widgets.DirectoryDialog;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.FileDialog;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.List;
import org.eclipse.swt.widgets.Menu;
import org.eclipse.swt.widgets.MenuItem;
import org.eclipse.swt.widgets.Monitor;
import org.eclipse.swt.widgets.Shell;
import org.eclipse.swt.widgets.Text;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.security.*;
import java.util.ArrayList;

/**
 * The HashVerifier program
 * @author Tyler Burton (software@tylerburton.ca)
 * @version 0.3.0.0 (September 1, 2010)
 *
 */
public final class HashVerifierMain {

	/**
	 * Globals
	 */
	Clipboard _clipboard = null; // A link to the system clipboard
	Combo _cboHash = null; // The hash selection combo box
	List _lstFiles = null; // The file list
	Label _lblVerified = null; // The label that shows if the hash matched or not
	Text _txtProducedHash = null; // The text field showing the generated hash
	Text _txtHashToCheck = null; // The text field containing the hash to check against
	final Shell _shell; // The application's window
	
	// An enumerated type of hashes available for selection
	private enum HASH { MD2, MD5, SHA1, SHA256, SHA384, SHA512 };

	/**
	 * This function starts the program
	 * @param args
	 */
	public static void main(String[] args) {
		@SuppressWarnings("unused")
		HashVerifierMain application = new HashVerifierMain();
	}

	/**
	 * The constructor. This method sets up the GUI and assigns the action listeners.
	 */
	public HashVerifierMain() {

		// Create a Display to link to underlying operating system
		Display display = new Display();
		
		// Create a clipboard and attach it to the system
		_clipboard = new Clipboard(display);

		// Create a shell (window)
		_shell = new Shell(display);

		// Set the shell's layout engine
		_shell.setLayout(new GridLayout(3, false));

		// Set the shell's size
		_shell.setSize(650, 300);
		
		// Set the shell's title text
		_shell.setText("Hash Verifier");

		/*
		 * Start shell in the centre of the screen
		 */

		// Grab the primary monitor
		Monitor primary = display.getPrimaryMonitor();

		// Scale the dimensions to centre the shell
		Rectangle bounds = primary.getBounds();
		Rectangle rect = _shell.getBounds();
		int x = bounds.x + (bounds.width - rect.width) / 2;
		int y = bounds.y + (bounds.height - rect.height) / 2;
		
		// Set the window location
		_shell.setLocation(x, y);

		// Create the top level menu
		Menu mnuTop = new Menu(_shell, SWT.BAR);
		_shell.setMenuBar(mnuTop);

		// Add "File" menu item
		MenuItem mniFile = new MenuItem(mnuTop, SWT.CASCADE);
		mniFile.setText("&File");

		// Add sub-menu for "File"
		Menu smnuFile = new Menu(_shell, SWT.DROP_DOWN);
		mniFile.setMenu(smnuFile);

		// Add "Select File" menu item
		MenuItem mniSelectFile = new MenuItem(smnuFile, SWT.PUSH);
		mniSelectFile.setText("Select File\tCtrl+O");
		
		// Assign the keyboard short-cut
		mniSelectFile.setAccelerator(SWT.CTRL + 'O');
		
		// Add action listener to "Select File" sub-menu item
		mniSelectFile.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);
			}

			/**
			 * This method is run when the user activates the "Select File" menu option.
			 * It displays the system file selection dialog.
			 */
			public void widgetSelected(SelectionEvent e) {
				// Create a new dialog
				FileDialog fd = new FileDialog(_shell, SWT.OPEN);
				
				// Set the text on the dialog
		        fd.setText("Select File");
		        
		        // Set the starting directory
		        fd.setFilterPath(System.getProperty("user.home"));
		        
		        // Get the selected file's path
		        String selected = fd.open();
		        
		        // Only add the file to the list if the user actually selected something and that it is in fact a file
		        if(selected != null && (new File(selected).isFile()))
		        	_lstFiles.add(selected);
			}
			
		});
		
		// Add "Select Directory" menu item
		MenuItem mniSelectDirectory = new MenuItem(smnuFile, SWT.PUSH);
		mniSelectDirectory.setText("Select Directory\tCtrl+D");
		
		// Assign keyboard short-cut
		mniSelectDirectory.setAccelerator(SWT.CTRL + 'D');
		
		// Add action listener to "Select Directory" sub-menu item
		mniSelectDirectory.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);
			}

			/**
			 * This method is run when the user activates the "Select Directory" menu option.
			 * It displays the system directory selection dialog.
			 */
			public void widgetSelected(SelectionEvent e) {
				// Create a new dialog
				DirectoryDialog dd = new DirectoryDialog(_shell, SWT.OPEN);
				
				// Set the text on the dialog
		        dd.setText("Select Directory");
		        
		        // Set the starting directory
		        dd.setFilterPath(System.getProperty("user.home"));
		        
		        // Get the selected directory's path
		        String selected = dd.open();
		        
		        // Make sure that the user actually selected something and that what was selected was actually a directory
		        if((selected != null) && (new File(selected).isDirectory()))
		        {
		        	// Create an ArrayList of Files scanned from the directory and its sub-directories
		        	ArrayList<File> fileList = getFilesRecursively(selected);
		        	
		        	// A sanity check to make sure we don't get any null objects
		        	if(fileList != null)
		        	{
		        		// Add the discovered files to the file list 
				        for(int i=0; i<fileList.size(); i++)
				        	_lstFiles.add(fileList.get(i).getPath());
		        	}
		        	
		        	// Attempt to free some memory
		        	System.gc();
		        }
			}

			/**
			 * This recursive function scans each sub-directory and builds a list of files to return
			 * @param parentDirectory The starting directory to look in
			 * @return an ArrayList of discovered files
			 */
			private ArrayList<File> getFilesRecursively(String parentDirectory){
				
				// Encapsulate the directory in a File object 
				File startingDirectory = new File(parentDirectory);
				
				// Create an inner list to fill files with
				ArrayList<File> fileList = new ArrayList<File>();
				
				// Do a sanity check that we did actually get a starting directory
				if(startingDirectory.isDirectory())
				{
					// Get the list of all files in the directory
					File[] files = startingDirectory.listFiles();
					
					// Make sure the directory contains files before continuing
					if(files != null)
					{
						// Loop through directory and add the files, or recursively call this function 
						// with newly found directories
						for(int i=0; i<files.length; i++)
						{
							if(files[i].isFile())
								fileList.add(files[i]);
							else if(files[i].isDirectory())
								fileList.addAll(getFilesRecursively(files[i].getPath()));
						}	
					}
					
					// Return the list of discovered files
					return fileList;
				}else
					// Return the list of discovered files
					return fileList;
			}
			
		});
		
		// Add "Close" menu item
		MenuItem mniClose = new MenuItem(smnuFile, SWT.PUSH);
		mniClose.setText("&Close");

		// Add action listener to "Close" sub-menu item
		mniClose.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);
			}

			/**
			 * This method is run when the user activates the "Close" menu option.
			 * It exits the program.
			 */
			public void widgetSelected(SelectionEvent e) {
				// Free the memory
				_shell.getDisplay().dispose();
				
				// Exit successfully 
				System.exit(0);
			}
		});
		
		// Add "Help" menu item
		MenuItem mniHelp = new MenuItem(mnuTop, SWT.CASCADE);
		mniHelp.setText("Help");
		
		// Add sub-menu for "Help"
		Menu smnuHelp = new Menu(_shell, SWT.DROP_DOWN);
		mniHelp.setMenu(smnuHelp);
		
		// Add "Visit Website" menu item
		MenuItem mniWebsite = new MenuItem(smnuHelp, SWT.PUSH);
		mniWebsite.setText("Visit Website\tF1");
		
		// Add keyboard short-cut
		mniWebsite.setAccelerator(SWT.F1);
		
		// Add action listener to "Visit Website" menu item
		mniWebsite.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);
			}

			/**
			 * This method is run when the user activates the "Visit Website" menu option.
			 * It launches the system's default browser to www.tylerburton.ca
			 */
			public void widgetSelected(SelectionEvent e) {
				Program.launch("http://www.tylerburton.ca");
			}
			
		});
		
		// Add "About" menu item
		MenuItem mniAbout = new MenuItem(smnuHelp, SWT.PUSH);
		mniAbout.setText("About");
		
		// Add action listener to "About" menu item
		mniAbout.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);
			}

			/**
			 * This method is run when the user activates the "About" menu option.
			 * It displays a dialog with program information.
			 */
			public void widgetSelected(SelectionEvent e) {
				
				// Create the about dialog 
				HashVerifierAboutDialog aboutDialog = new HashVerifierAboutDialog(_shell);
				
				// Display it and keep it open until the user closes the about dialog
			    if(aboutDialog.open())
			    {
			    	//do nothing - this forces the dialog to stay on top
			    }
			}
		});
		
		/*
		 * *********************************************************
		 * lblInfo
		 * *********************************************************
		 */

		// Add info label
		Label lblInfo = new Label(_shell, SWT.NONE);
		lblInfo.setText("Drag file(s) into here to begin"); // Set the text
		GridData gridData = new GridData(); // Create a layout schema
		gridData.horizontalSpan = 3; // Span over 3 columns
		lblInfo.setLayoutData(gridData);

		/*
		 * *********************************************************
		 * lstFiles
		 * *********************************************************
		 */
		
		// Add file list
		_lstFiles = new List(_shell, SWT.BORDER | SWT.MULTI | SWT.V_SCROLL);
		gridData = new GridData(); // Create a layout schema
		gridData.horizontalSpan = 3; // Span over 3 columns
		gridData.horizontalAlignment = SWT.FILL; // Tell it to fill the horizontal space
		gridData.grabExcessHorizontalSpace = true; // Tell it to get any additional horizontal space it can
		gridData.verticalAlignment = SWT.FILL; // Tell it to fill the vertical space
		gridData.grabExcessVerticalSpace = true; // Tell it to get any additional vertical space it can
		_lstFiles.setLayoutData(gridData);
		
		/*
		 * Setup selection listeners for lstFiles
		 */
		_lstFiles.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);
			}

			/**
			 * This method is run when the user clicks in the file list
			 * It shows the hash in the text field
			 */
			public void widgetSelected(SelectionEvent e) {
				// Compute and display the hash
				computeHash();
				
				// Compare the hashes
				compareHashes();
			}
		});
		
		/*
		 * Setup file list for drag and drop from files only 
		 */

		// Allow data to be copied from the drag source to the target (in this case the lstFiles widget)
		DropTarget target = new DropTarget(_lstFiles, DND.DROP_COPY	| DND.DROP_DEFAULT);

		// We only want to allow files to be dragged and dropped
		final FileTransfer fileTransfer = FileTransfer.getInstance();
		
		// Set the types we will allow for this drag and drop operation
		Transfer[] types = new Transfer[] { fileTransfer };
		target.setTransfer(types);

		/*
		 * Set the action listeners for the drag and drop operation
		 */
		target.addDropListener(new DropTargetListener() {
			/*
			 * Triggered when the drag operation 'enters' the widget. Only allows for copy operations.
			 * @see org.eclipse.swt.dnd.DropTargetListener#dragEnter(org.eclipse.swt.dnd.DropTargetEvent)
			 */
			public void dragEnter(DropTargetEvent event) {
				if (event.detail == DND.DROP_DEFAULT) {
					if ((event.operations & DND.DROP_COPY) != 0) {
						event.detail = DND.DROP_COPY;
					} else {
						event.detail = DND.DROP_NONE;
					}
				}
				
				// We will only accept files to be dragged and dropped
				for (int i = 0; i < event.dataTypes.length; i++) {
					if (fileTransfer.isSupportedType(event.dataTypes[i])) {
						event.currentDataType = event.dataTypes[i];
						// Files should only be copied
						if (event.detail != DND.DROP_COPY) {
							event.detail = DND.DROP_NONE;
						}
						break;
					}
				}
			}

			/*
			 * Triggered when the drag operation moves over the target. Provides feedback to the user.
			 * @see org.eclipse.swt.dnd.DropTargetListener#dragOver(org.eclipse.swt.dnd.DropTargetEvent)
			 */
			public void dragOver(DropTargetEvent event) {
				event.feedback = DND.FEEDBACK_SELECT | DND.FEEDBACK_SCROLL;
			}

			/*
			 * Triggered when the drag operation is changed. This again only allows file copies to be made.
			 * @see org.eclipse.swt.dnd.DropTargetListener#dragOperationChanged(org.eclipse.swt.dnd.DropTargetEvent)
			 */
			public void dragOperationChanged(DropTargetEvent event) {
				if (event.detail == DND.DROP_DEFAULT) {
					if ((event.operations & DND.DROP_COPY) != 0) {
						event.detail = DND.DROP_COPY;
					} else {
						event.detail = DND.DROP_NONE;
					}
				}
				// Copy the file path
				if (fileTransfer.isSupportedType(event.currentDataType)) {
					if (event.detail != DND.DROP_COPY) {
						event.detail = DND.DROP_NONE;
					}
				}
			}

			/*
			 * Triggered when the drop is completed. Adds the file path to the list
			 * @see org.eclipse.swt.dnd.DropTargetListener#drop(org.eclipse.swt.dnd.DropTargetEvent)
			 */
			public void drop(DropTargetEvent event) {
				if (fileTransfer.isSupportedType(event.currentDataType)) {
					String[] files = (String[]) event.data;
					for (int i = 0; i < files.length; i++) {
						_lstFiles.add(files[i]);
					}
				}
			}

			/*
			 * These methods must be implemented but we will leave them empty
			 * @see org.eclipse.swt.dnd.DropTargetListener#dragLeave(org.eclipse.swt.dnd.DropTargetEvent)
			 */
			public void dragLeave(DropTargetEvent event) {
				// Do nothing
			}

			/*
			 * These methods must be implemented but we will leave them empty
			 * @see org.eclipse.swt.dnd.DropTargetListener#dropAccept(org.eclipse.swt.dnd.DropTargetEvent)
			 */
			public void dropAccept(DropTargetEvent event) {
				// Do nothing
			}
		});
		
		/*
		 * *********************************************************
		 * lblHashProduced
		 * *********************************************************
		 */
		
		// Add label
		Label lblHashProduced = new Label(_shell, SWT.NONE);
		lblHashProduced.setText("Hash produced:"); // Set the text
		gridData = new GridData(); // Create a layout schema
		gridData.horizontalSpan = 3; // Span over 3 columns
		lblHashProduced.setLayoutData(gridData);

		/*
		 * *********************************************************
		 * txtProducedHash
		 * *********************************************************
		 */
		
		// Add text field
		_txtProducedHash = new Text(_shell, SWT.BORDER);
		_txtProducedHash.setEditable(false); // Make sure the user can't edit it
		gridData = new GridData(); // Create a layout schema
		gridData.horizontalAlignment = SWT.FILL; // Tell it to fill the horizontal space
		gridData.grabExcessHorizontalSpace = true; // Tell it to get any excess horizontal space it can
		_txtProducedHash.setLayoutData(gridData);
		
		/*
		 * *********************************************************
		 * cboHash
		 * *********************************************************
		 */
		
		// Add combo box
		_cboHash = new Combo(_shell, SWT.READ_ONLY);
		
		// Set the items
		_cboHash.setItems (new String [] {"SHA-1", "SHA-256", "SHA-384", "SHA-512", "MD2", "MD5"});
		
		// Select the first in the list
		_cboHash.select(0);
		
		/*
		 * Add selection listeners
		 */
		_cboHash.addModifyListener(new ModifyListener(){
			
			/**
			 * This event is run whenever the combo box text changes
			 */
			public void modifyText(ModifyEvent e) {
				// Compute and display hash
				computeHash();
				
				// Compare the hashes
				compareHashes();
			}	
		});
		
		/*
		 * *********************************************************
		 * btnCopyHash
		 * *********************************************************
		 */
		
		// Add a button
		Button btnCopyHash = new Button(_shell, SWT.PUSH);
		
		// Set the button text
		btnCopyHash.setText("Copy Hash");
		
		/*
		 * Add action listeners
		 */
		btnCopyHash.addSelectionListener(new SelectionListener(){
			public void widgetDefaultSelected(SelectionEvent e) {
				widgetSelected(e);				
			}

			/**
			 * This method is run when the user activates the "Copy Hash" button.
			 * It copies the current produced hash text (if available) to the clipboard
			 */
			public void widgetSelected(SelectionEvent e) {
				if(_txtProducedHash.getText() != null && _txtProducedHash.getText().length() > 0)
					_clipboard.setContents(new Object[] { _txtProducedHash.getText() }, new Transfer[] { TextTransfer.getInstance() });
			}
			
		});
		
		/*
		 * *********************************************************
		 * lblHashToCheck
		 * *********************************************************
		 */
		
		// Add label
		Label lblHashToCheck = new Label(_shell, SWT.NONE);
		lblHashToCheck.setText("Hash to check against:"); // Set the text
		gridData = new GridData(); // Create a layout schema
		gridData.horizontalSpan = 3; // Span over 3 columns
		lblHashToCheck.setLayoutData(gridData);
		
		/*
		 * *********************************************************
		 * lblHashToCheck
		 * *********************************************************
		 */
		
		// Add text box
		_txtHashToCheck = new Text(_shell, SWT.BORDER);
		gridData = new GridData(); // Create a layout schema
		gridData.grabExcessHorizontalSpace = true; // Tell it to get any excess horizontal space it can
		gridData.horizontalAlignment = SWT.FILL; // Tell it to fill the horizontal space
		gridData.horizontalSpan = 2; // Tell it span 2 columns in the grid
		_txtHashToCheck.setLayoutData(gridData);
		
		/*
		 * Add action listeners
		 */
		_txtHashToCheck.addModifyListener(new ModifyListener(){
			
			/**
			 * This method is run when the user changes the text in the hash to check text field.
			 * It checks the entered text against the produced hash text and displays a verified or failed message.
			 */
			public void modifyText(ModifyEvent e) {
				
				// Compare the produced hash to the user supplied one
				compareHashes();
			}
		});
		
		/*
		 * *********************************************************
		 * lblVerified
		 * *********************************************************
		 */
		
		// Add label
		_lblVerified = new Label(_shell, SWT.NONE);
		_lblVerified.setText(""); // Set the label text
		gridData = new GridData(); // Create a layout schema
		gridData.horizontalAlignment = SWT.FILL; // Tell it to fill the horizontal space
		gridData.horizontalSpan = 1; // Tell it to only span one grid
		_lblVerified.setLayoutData(gridData);
		
		// Move shell to top of drawing order
		_shell.open();

		// Start render and event loop
		while (!_shell.isDisposed()) {
			if (!display.readAndDispatch())
				display.sleep();
		}

		// Cleanup and free memory
		_clipboard.dispose();
		display.dispose();
	}
	
	/**
	 * Returns the message digest produced using the hash function on the file 
	 * @param hash The hash algorithm to use
	 * @param filepath The file path
	 * @return The message digest
	 */
	private byte[] getHash(HASH hash, String filepath)
	{
		// Create a reference to hold the message digest we will be using
		MessageDigest md = null;
		
		// Create a file stream to read the contents of the file we will be hashing
		FileInputStream fileInputStream = null;
		
		// Create a buffered input stream to improve performance for the file input stream
		BufferedInputStream bufferedInputStream = null;
		
		try{
			// Open the file for reading
			fileInputStream = new FileInputStream(new File(filepath));
			bufferedInputStream = new BufferedInputStream(fileInputStream, 4096);
			
			/*
			 * Determine which algorithm to use and instantiate the object
			 */
			if(hash == HASH.MD2)
			{
				md = MessageDigest.getInstance("MD2");
			}else if(hash == HASH.MD5)
			{
				md = MessageDigest.getInstance("MD5");
			}else if(hash == HASH.SHA1)
			{
				md = MessageDigest.getInstance("SHA-1");
			}else if(hash == HASH.SHA256)
			{
				md = MessageDigest.getInstance("SHA-256");
			}else if(hash == HASH.SHA384)
			{
				md = MessageDigest.getInstance("SHA-384");
			}else if(hash == HASH.SHA512)
			{
				md = MessageDigest.getInstance("SHA-512");
			}
			
			// If for some reason this is still null we need to exit the function
			if(md != null)
			{
				// Create a buffer to read data in from the file
				byte[] buff = new byte[1024];
				
				// Create a byte count of how much we have read on each pass
				int count = 0;
				
				// Keep reading while we have more input 
				while((count = bufferedInputStream.read(buff, 0, buff.length)) != -1){
					// Update the hash state with the new data
					md.update(buff, 0, count);
				}
				
				// Close the file stream
				bufferedInputStream.close();
				
				// Finish generating the message digest
				return md.digest();
			}else
				return null;
			
		}catch(NoSuchAlgorithmException e){
			e.printStackTrace();
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		// An error has occurred, return null
		return null;
	}
	
	/**
	 * Compute the hash of the selected file and update the text field with the hash's hex string
	 */
	private void computeHash(){
		// Get the list of selection indecies 
		int [] selection = _lstFiles.getSelectionIndices ();
		
		// This is where we will store the final selected item
		int selected = 0;
		
		// If the user has selected more than one we want to update it back to the first file selected
		if(selection.length > 0)
		{
			selected = selection[0];
			selection = null;
		}else
			return;

		// Set the file list to the first selected item
		_lstFiles.setSelection(selected);
		
		// Create a string to hold the file's hash in hex
		String hash = null;
		
		// Figure out which algorithm we are using and convert the result to hex
		if(_cboHash.getText().equalsIgnoreCase("md2")){
			hash = convertToHexString(getHash(HASH.MD2,_lstFiles.getSelection()[0]));
		}else if(_cboHash.getText().equalsIgnoreCase("md5")){
			hash = convertToHexString(getHash(HASH.MD5,_lstFiles.getSelection()[0]));
		}else if(_cboHash.getText().equalsIgnoreCase("sha-1")){
			hash = convertToHexString(getHash(HASH.SHA1,_lstFiles.getSelection()[0]));
		}else if(_cboHash.getText().equalsIgnoreCase("sha-256")){
			hash = convertToHexString(getHash(HASH.SHA256,_lstFiles.getSelection()[0]));
		}else if(_cboHash.getText().equalsIgnoreCase("sha-384")){
			hash = convertToHexString(getHash(HASH.SHA384,_lstFiles.getSelection()[0]));
		}else if(_cboHash.getText().equalsIgnoreCase("sha-512")){
			hash = convertToHexString(getHash(HASH.SHA512,_lstFiles.getSelection()[0]));
		}
		
		// If there was an error we will simply set it to empty string
		if(hash == null)
			hash = "";
		
		// Update the text field with the hash string
		_txtProducedHash.setText(hash);
	}
	
	/**
	 * Compares the produced hash to the user supplied one and updates the GUI
	 */
	private void compareHashes()
	{
		// Get the produced hash string
		String producedHash = _txtProducedHash.getText();
		
		// Get the hash to check string
		String compareHash = _txtHashToCheck.getText();
		
		// Only run this code if we actually have something to work with
		if(compareHash != null && compareHash.length() > 0)
		{
			// Create a color object
			Color clrBg = null;
			
			// Check the hash strings against one another
			if(compareHash.equalsIgnoreCase(producedHash))
			{
				// If they match change the label color to green
				clrBg = new Color(_shell.getDisplay(), new RGB(0, 255, 0));
				
				// Update the label's text
				_lblVerified.setText("VERIFIED");
			}else{
				// If they match change the label color to red
				clrBg = new Color(_shell.getDisplay(), new RGB(255, 0, 0));
				
				// Update the label's text
				_lblVerified.setText("FAILED");
			}
			
			// Set the label background color
			_lblVerified.setBackground(clrBg);
			
			// Free the memory we used creating the color object
			clrBg.dispose();	
			System.gc();
		}	
	}

	/**
	 * Converts the input byte array to a hex string
	 * @param input The input byte array
	 * @return The hex string
	 */
	private String convertToHexString(byte[] input){
		
		// Create a string containing the hex alphabet
		final String HEX_CHARACTERS = "0123456789ABCDEF";
		
		// Make sure we have good input
		if(input == null || input.length <= 0)
			return "";
		
		// Use a StringBuilder to create our output so we don't create a bunch of useless strings
		StringBuilder output = new StringBuilder(input.length * 2);
		
		// Loop through the input and build out output
		for(int i=0; i<input.length; i++)
			output.append(HEX_CHARACTERS.charAt((input[i] & 0xF0) >> 4)).append(HEX_CHARACTERS.charAt((input[i] & 0x0F)));
		
		// Return the output
		return output.toString();
	}

}
