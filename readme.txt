This git repository actually comprises two Visual Studio projects (one solution)

IBM1410SMS     - this is the code for the SMS capture application
MySQLFramework - this is a very rudimentary encapulsation of the database used by IBM1410SMS

A second git repository, cube1us/IBM1410FPGA holds the genreated VHDL that comes out of
this application along with hand coded VHDL to implement the IBM 1410 Data Processing System
in an FPGA.

Eventually there will be a third git repository holding the PC hosting application that will provide
console, card, print, tape and maybe disk functionality over a high speed USB serial connection to
an FPGA development board.

The file Documentation.pdf is a PDF version of IBM1410SMS/Documentation.docx

The file ibm1410sms.sql.gz is a zipped dump of the current MySQL database.

The file IBM1410SMSModel.mwb is a MySQL Workbench model of the database  (.PDF is a print version)

The Folder IBM1410-ImportedSpreadSheets contaisn the data captured from card location charts in 
spreadsheets, that were then used to do initial population of the database.  

See the project documentation file IBM1410SMS/Documentation.docx for more information.

Added COPYING, database dump and readme to git commit
See COPYING for license information
