FRC-FencingResultConverter generates competition result files in PDF format from fencing competition software
result files in xml format.

Make sure you have installed .NET Framework version 4.5 before you start "FRC-FencingResultConverter2.3", 
otherwise you will get an error message including the title ".NET Framework Initialization Error".
You can download .NET Framework version 4.5 from following link:
http://www.microsoft.com/en-us/download/details.aspx?id=17851

This version of FRC-FencingResultConverter supports conversion of xml files either in FIE format or extended 
format from the "EnGarde"-software for individual competitions. For team competitions the xml file must be 
in extended format. The team competiton can have placement matches from "3rd place"-"16th place".

This version of FRC-FencingResultConverter does not support conversion of team competition xml files in FIE
format, competitions without tableau, poule sizes larger than 11, and repechage.

If the xml file is a team competition in FIE format, or includes some incorrect formats, an error message will 
pop up saying "The format of the xml file is not supported by this program".


Instructions:

1. Start FRC-FencingResultConverter2.3 by clicking on it.
2. Check one of the three options, and select files by clicking on "Open".
3. The selected files are now displayed in the list box. If you want to remove a file from the list, select
   the file and click on "Remove"-button.
4. Click on "Convert"-button to convert the file/files.

Options:

"One bundle and all files separately as output" - You will have to specify the directory and filename for the 
bundle, which you can do in the Save File Dialog which will pop up. The other output-files will be stored in the
same directory as the bundle.
"Only one bundle as output" - You will have to specify the directory and filename for the bundle, which you can
do in the Save File Dialog which will pop up. There are no other output files.
"Only files separately as output" - There is no bundle. The output-files will be store in the same directory as
the input-files.
For conversion of only one xml-file, "Only one bundle as output" or "Only files separately as output" is 
recommended.
