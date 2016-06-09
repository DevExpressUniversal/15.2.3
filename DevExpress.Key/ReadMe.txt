=================================
REBUILDING DEVELOPER EXPRESS DLLs
=================================

Before building the Developer Express dll's (consult your EULA for a list of all redistributable dlls), you have to create a strong name file. To create this file, you must execute the following command: sn -k StrongKey.snk and copy the newly generated StrongKey.snk file to the \Devexpress.Key\ directory. The Sn.exe utility is located in your FrameworkSDK binary directory.
Please read the 'Strong-Named Assemblies' topic in MSDN for more information.
Once you complete these steps, you can distribute your Developer Express dlls. 

For assistance, feel free to write to our support department at: support@devexpress.com