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
import org.eclipse.swt.graphics.Font;
import org.eclipse.swt.layout.GridLayout;
import org.eclipse.swt.widgets.Dialog;
import org.eclipse.swt.widgets.Display;
import org.eclipse.swt.widgets.Label;
import org.eclipse.swt.widgets.Shell;

/**
 * The about dialog for the HashVerifier program
 * @author Tyler Burton (software@tylerburton.ca)
 * @version 0.3.0.0 (September 1, 2010)
 *
 */
public class HashVerifierAboutDialog extends Dialog{

	/**
	 * The constructor. Takes in a reference to it's parent shell.
	 * @param parent The parent shell.
	 */
	public HashVerifierAboutDialog(Shell parent) {
		super(parent);
	}
	
	/**
	 * Opens the dialog and keeps it open until the user closes it. 
	 * @return True if the user closes it
	 */
	public boolean open() {
		
		// Grab a reference to the parent
	    Shell parent = getParent();
	    
	    // Grab a reference to the parent's display
	    Display display = parent.getDisplay();
	    
	    // Create a new shell for the dialog
	    Shell dialog = new Shell(parent, SWT.DIALOG_TRIM | SWT.APPLICATION_MODAL);
	    
	    // Set the dialog's size
	    dialog.setSize(375, 150);
	    
	    // Set the dialog's window text
	    dialog.setText("About");
	    
	    // Set Dialog's layout engine
		dialog.setLayout(new GridLayout(1, false));
		
		// Create a label for the program's name
		Label lblProgramName = new Label(dialog,SWT.NONE);
		
		// Set it's text
		lblProgramName.setText("HashVerifier");
		
		// Change it's font
		Font f = new Font(display, "SansSerif", 14, SWT.BOLD);
		lblProgramName.setFont(f);
		f.dispose();
		
		// Create a label for the program's version
		Label lblVersion = new Label(dialog, SWT.NONE);
		
		// Set it's text
		lblVersion.setText("Version: 0.3.0.0");
		
		// Create a label for the program's license
		Label lblLicense = new Label(dialog, SWT.NONE);
		
		// Set it's text
		lblLicense.setText("Released under the GNU Lesser Public License (LGPL)");
		
		// Create a label for the program's copyright
		Label lblCopyright = new Label(dialog, SWT.NONE);
		
		// Set it's text
		lblCopyright.setText("Copyright: 2010 Tyler Burton");
		
		// Create a label for the program's contact information
		Label lblContact = new Label(dialog, SWT.NONE);
		
		// Set it's text
		lblContact.setText("Contact: software@tylerburton.ca");
		
		// Display the dialog
	    dialog.open();
	    
	    // Start render and event loop
	    while (!dialog.isDisposed()) {
	      if (!display.readAndDispatch())
	        display.sleep();
	    }
	    
	    // Return back to the calling code
	    return true;
	 }

}
