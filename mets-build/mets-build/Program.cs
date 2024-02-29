

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using System.Xml;
using System.Web;
using System.Reflection.Metadata.Ecma335;
using System.Transactions;
using System.Security.Cryptography;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.StaticFiles;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Tracing;
using System.Reflection.Emit;
//using static System.Net.WebRequestMethods;
//using System.Security.Cryptography;
//using static System.Runtime.InteropServices.JavaScript.JSType;




/*  Build a METS file for a CSIP
 *  See
https://earkcsip.dilcis.eu/#useofmets

*/


/* A E-ARK package can be at any location on the disk but it must lie in a folder called E-ARK
 * This is to be able to identify the start
 * 
 */


public class METSBuild
{
    const string filepath = @"c:\tmp\filepath.txt";

    const string filegroupschemas = "file-grp-schemas";
    const string filegroupdata = "file-grp-data";
    const string filegroupdocumentation = "file-grp-documenatation";

    const bool Debug = false;
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("ERROR: I need a directory to create a mets file from!");
            Environment.Exit(-1);
            //no args passed
        }




        // test the provided path for existance
        foreach (string path in args)
        {
            if (File.Exists(path))
            {
                // This path is a file
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                // This path is a directory
                ProcessDirectory(path);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }
        }


        // start nets file generation
        mets(args[0]);

        Environment.Exit(0);
    }





    /*****************************************************************************************
     * ***************************************************************************************
     */


    public static void mets(string path)
    {

        if (File.Exists(filepath))
        {
            // Delete the file
            File.Delete(filepath);
        }

        // write the mets start
        Console.Write("<mets:mets");

        rootelement(path);
        Console.Write(">");
        Newline();
        
        metsHdr(path);
        Newline();
        
        dmdSec(path);
        Newline();

        
        amdSec(path);
        Newline();

        
        fileSec(path);
        Newline();
        

        
        structMap(path);
        Newline();
        

        Newline();
        Console.WriteLine("</mets:mets>");
     

    }


    
    public static void rootelement(string path)
    {


        string string1 = "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";
        string string2 = "xmlns:mets=\"http://www.loc.gov/METS/\"";
        string string3 = "xmlns:xlink=\"http://www.w3.org/1999/xlink\"";
        string string4 = "xmlns:csip=\"https://DILCIS.eu/XML/METS/CSIPExtensionMETS\"";
        string string5 = "xsi:schemaLocation=";
        string string6 = "http://www.loc.gov/METS/ http://www.loc.gov/standards/mets/mets.xsd";
        string string7 = "http://www.w3.org/1999/xlink http://www.loc.gov/standards/mets/xlink.xsd";
        string string8 = "https://DILCIS.eu/XML/METS/CSIPExtensionMETS https://earkcsip.dilcis.eu/schema/DILCISExtensionMETS.xsd";

        Newline();

        Console.Write(string1); Newline();
        Console.Write(string2); Newline();
        Console.Write(string3); Newline();
        Console.Write(string4); Newline();
        Console.Write(string5 + "\"" + string6 + " " + string7 + " " + string8 + "\"");


        //Console.Write(" " + string1 + " " + string2 + " " + string3 + " " + string4 + " " + string5 + " " + string6 + " " + string7);


        Newline();

        CSIP1(path);
        CSIP2(path);
        CSIP3(path);
        CSIP4(path);
        CSIP5(path);
        CSIP6(path);

    }

    public static void metsHdr(string path)
    {


        CSIP117(path);
        CSIP7(path);
        CSIP8(path);
        CSIP9(path);
        Console.Write(">");
        CSIP10(path);
        CSIP11(path);
        CSIP12(path);
        CSIP13(path);
        CSIP14(path);
        CSIP15(path);
        CSIP16(path);
        

        // mets:agent 
        Newline();
        Console.Write(@"</mets:agent>");
        Newline();

        // write the mets hdr end
        Console.Write(@"</mets:metsHdr>");

    }

    public static void dmdSec(string path)
    {

        /*
         * 
         * The purpose of the METS descriptive metadata section is to embed or refer to files containing 
         * descriptive metadata. CSIP is only using referencing of files containing descriptive metadata.
         * 
         * We assume that descriptive metadata is provided by files of the path metadata/descriptive/*
         * */

        
        DescriptiveMetadata(path);
        Newline();
        

    }
    public static void amdSec(string path)
    {
        CSIP31(path);
        AdministrativeMetadata(path);
        Newline();
        Console.Write(@"</mets:amdSec>");

    }
   
    public static void structMap(string path)
    {

        CSIP80(path);
        CSIP81(path);
        CSIP82(path);
        CSIP83(path);
        Console.Write(">");

        CSIP84(path);
        CSIP85(path);
        CSIP88(path);


        CSIP92(path);
        // read the file looking for these data types

        Console.Write("\"");

        // divid for metadata
        using (StreamReader reader = new StreamReader(filepath))
        {
            string line;

            int i = 0;
            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);
                
                if (parts[1] == "descriptive")
                {
                    
                    i += 1;
                    if (i > 1) Console.Write(" ");
                    Console.Write(GetPath(parts[0]));
                }
            }
            
        }
        Console.Write("\"");


        // administrative
        CSIP91(path);
        Console.Write("\"");
        using (StreamReader reader = new StreamReader(filepath))
        {
            string line;

            int i = 0;
            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);

                if (parts[1] == "administrative")
                {

                    i += 1;
                    if (i > 1) Console.Write(" ");
                    Console.Write(GetPath(parts[0]));
                }
            }

        }
        Console.Write("\"");
        Console.Write(">");
        Newline();
        Console.Write("</mets:div>");



        //divid for schemas
        Newline();
        Console.Write("<mets:div");
        Console.Write(" " + "ID=");
        Console.Write("\"struct-map-schemas-div\"");
        Console.Write(" "  + "LABEL=\"schemas\">");
        Newline();
        Console.Write("<mets:fptr");
        Console.Write(" " + "FILEID=");
        using (StreamReader reader = new StreamReader(filepath))
        {
            string line;

            
            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);

                if (parts[1] == "schemas")
                {
                    Console.Write("\"" + parts[2] + "\"");
                    break;
                }
            }
        }
        Console.Write(">");
        Newline();
        Console.Write("</mets:fptr>");
        Newline();
        Console.Write("</mets:div>");


        // divid for data
        Newline();
        Console.Write("<mets:div");
        Console.Write(" " + "ID=");
        Console.Write("\"struct-map-data-div\"");
        Console.Write(" " + "LABEL=\"data\">");
        Newline();
        Console.Write("<mets:fptr");
        Console.Write(" " + "FILEID=");
        using (StreamReader reader = new StreamReader(filepath))
        {
            string line;

           
            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);

                if (parts[1] == "data")
                {
                    Console.Write("\"" + parts[2] + "\"");
                    break;
                }
            }
        }
        Console.Write(">");
        Newline();
        Console.Write("</mets:fptr>");
        Newline();
        Console.Write("</mets:div>");


        // divid for documenatation
        Newline();
        CSIP93(path);
        CSIP94(path);
        CSIP95(path);

        Newline();
        Console.Write("<mets:fptr");
        Console.Write(" "  + "FILEID=");
        using (StreamReader reader = new StreamReader(filepath))
        {
            string line;

           
            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);

                if (parts[1] == "documenatation")
                {
                    Console.Write("\"" + parts[2] + "\"");
                    break;
                }
            }
        }
        Console.Write(">");
        Newline();
        Console.Write("</mets:fptr>");
        Newline();
        Console.Write("</mets:div>");

        Newline();
        Console.Write("</mets:structMap>");
    }

    

    /*
     * HELPER FUNCTIONS
     * 
     */


    public static void fileSec(string path)
    {

        string val = "fileSec01";

        CSIP58();
        CSIP59(val);

        // classify the files
        FileSecProcessing(path, filepath);

        //Write the mets file after classification
        FileSecProcessFile(filepath);

        Newline();
        Console.Write(@"</mets:fileSec>");


    }

    static string GetDate()
    {

        string str = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.ffffff");
        return (str);

    }


    static void WritetoFile(string filename, string text)
    {

        using (StreamWriter writer = new StreamWriter(filename, true))
        {
            // Write some text to the file
            writer.WriteLine(text);
        }
    }


    static string GetMimeType(string fileName)
    {
        string mimeType = "application/unknown";
        string ext = Path.GetExtension(fileName).ToLower();
        Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
        if (regKey != null && regKey.GetValue("Content Type") != null)
        {
            mimeType = regKey.GetValue("Content Type").ToString();
        }
        else if (ext == ".png")
        {
            mimeType = "image/png";
        }
        return mimeType;
    }


    static void Newline()
    {
        Console.Write('\n');

    }

    static void Space()
    {
        Console.Write(" ");
    }

    public static void FileSecProcessing(string targetDirectory, string filelist)
    {

        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        {
            //Console.WriteLine(fileName);

            // make three types - schemas, data and documenation
            if (fileName.Contains(@"schemas"))
            {   
                WritetoFile(filelist, fileName+ "," + "schemas" + "," + filegroupschemas);
            }
            else if (fileName.Contains(@"representations"))
            {
                WritetoFile(filelist, fileName + "," + "data" + "," + filegroupdata);
            }
            else if (fileName.Contains(@"metadata"))
            {
                // does not get put into filesec
            }
            else
            { 
                WritetoFile(filelist, fileName+ "," + "documenatation" + "," + filegroupdocumentation);
            }
            
        }

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
            FileSecProcessing(subdirectory, filelist);


    }
    public static void FileSecProcessFile(string filePath)
    {
        // now we are ready to process the filelist by type
        int fileid = 1;

        CSIP60(filegroupschemas, "schemas");
        // look for type "schemas"
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;



            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);
                if (parts[1] == "schemas")
                {
                    WriteEntry(parts[0], "schemas", fileid);
                    fileid = fileid + 1;
                }
            
            }
        }
        Newline();
        Console.Write("</mets:fileGrp>");


        CSIP60(filegroupdata, "data");
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;

            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);
                if (parts[1] == "data")
                {
                    WriteEntry(parts[0], "data", fileid);
                    fileid = fileid + 1;
                }
            }
        }
        Newline();
        Console.Write("</mets:fileGrp>");

        // look for type "documenatation"
        CSIP60(filegroupdocumentation, "documenatation");
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;

            // Read each line until the end of the file
            while ((line = reader.ReadLine()) != null)
            {
                //Console.WriteLine(line);
                string[] parts = line.Split(',');

                //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);
                if (parts[1] == "documenatation")
                {
                    WriteEntry(parts[0], "documenatation", fileid);
                    fileid = fileid + 1;
                }
            }
        }
        Newline();
        Console.Write("</mets:fileGrp>");

    }
    
    public static void WriteEntry(string Filename, string type, int fileid)
    { 
        
        
        string path = null;


        //Console.WriteLine("in WriteEntry " + Filename + " " + type);

        // get size of file
        FileInfo fileInfo = new FileInfo(Filename);
        long fileSizeInBytes = fileInfo.Length;

        // calculate checksum
        string md5Checksum = ComputeMD5Checksum(Filename);


        //CSIP113(path);
        //CSIP114(path);
        CSIP61(path);
        CSIP62(path);
        CSIP63(path);
        CSIP64(path);
        CSIP65(path);
        CSIP66(path);
      
        string str = string.Concat("file", fileid);
        CSIP67(str);
        CSIP68(Filename);
        CSIP69(fileSizeInBytes);
        CSIP70(path);
        CSIP71(md5Checksum);
        CSIP72(path);
        CSIP73("rigsarkivet");
        CSIP74(path);
        CSIP75(path);
        CSIP76(path);
        CSIP77(path);
        CSIP78(path);
        CSIP79(Filename);
        Console.Write("</mets:file>");
        
    }


    // Process all files in the directory passed in, recurse on any directories
    // that are found, and process the files they contain.
    public static void ProcessDirectory(string targetDirectory)
    {
        // Process the list of files found in the directory.
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
            ProcessFile(fileName);

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
            ProcessDirectory(subdirectory);
    }
    public static void ProcessFile(string path)
    {
        //Console.WriteLine("Processed file '{0}'.", path);
    }


    public static void DescriptiveMetadata(string targetDirectory)
    {
        // Process the list of files found in the directory.
        // the filepath must be exact

        int number = 0;

        string[] fileEntries = Directory.GetFiles(targetDirectory);
        

        foreach (string fileName in fileEntries)
        {
            //Console.WriteLine("filentries" + fileName);
            if (fileName.Contains(@"metadata\descriptive"))
            {
                number = number + 1;
                ProcessDescriptiveFile(fileName, number);
            }
        }

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
            DescriptiveMetadata(subdirectory);
    }
    public static void ProcessDescriptiveFile(string path, int number)
    {
        //Console.WriteLine("Processed descriptivefile '{0}'.", path);

        string url = null;
        bool started = false;
       

        // get size of file
        FileInfo fileInfo = new FileInfo(path);
        long fileSizeInBytes = fileInfo.Length;

        // add to the clasification file
        WritetoFile(filepath, path + "," +  "descriptive");

        // calculate checksum
        string md5Checksum = ComputeMD5Checksum(path);

        string[] sections = path.Split('\\');

        // Output each section
        for (int i = 0; i < sections.Length; i++)
        {
            //Console.WriteLine("section value " + sections[i] + " " + i);

            if (sections[i].Contains("metadata") == true)
            {
                // start of file
                started = true;
                url = url + sections[i];
            }
            else
            {
                if (started == true)
                { 
                    url = url + '\\' + sections[i];
                }
            } 
        }
        //Console.WriteLine("url is" + url);



        // so now we can add the mets section to this file given by url

       

       CSIP17(path);
       CSIP18(url, number);
       CSIP19(url);
       CSIP20(url);
       CSIP21(url);
       CSIP22(url);
       CSIP23(url);
       CSIP24(url); ;
       CSIP25(url);
       CSIP26(url);
       CSIP27(url, fileSizeInBytes);
       CSIP28(url);
       CSIP29(url, md5Checksum);
       CSIP30(url);
       Console.Write(">");
       Newline();
       Console.Write("</mets:mdRef>");
       Newline();
       Console.Write("</mets:dmdSec>");
       Newline();


    }


    public static void AdministrativeMetadata(string targetDirectory)
    {
        // Process the list of files found in the directory.

        int number = 0;

        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
        if (fileName.Contains(@"metadata\preservation"))
        {
                number = number + 1;
                ProcessAdministrativeFile(fileName, number);
        }

        // Recurse into subdirectories of this directory.
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
            AdministrativeMetadata(subdirectory);
    }
    public static void ProcessAdministrativeFile(string path, int number)
    {
        //Console.WriteLine("Processed descriptivefile '{0}'.", path);

        string url = null;
        bool started = false;

        // add to the clasification file
        WritetoFile(filepath, path + "," + "administrative");

        // get size of file
        FileInfo fileInfo = new FileInfo(path);
        long fileSizeInBytes = fileInfo.Length;

        // calculate checksum
        string md5Checksum = ComputeMD5Checksum(path);

        string[] sections = path.Split('\\');

        // Output each section
        for (int i = 0; i < sections.Length; i++)
        {
            //Console.WriteLine("section value " + sections[i] + " " + i);

            if (sections[i].Contains("metadata") == true)
            {
                // start of file
                started = true;
                url = url + sections[i];
            }
            else
            {
                if (started == true)
                {
                    url = url + '\\' + sections[i];
                }
            }
        }
        //Console.WriteLine("url is" + url);



        // so now we can add the mets section to this file given by url

        //CSIP31(url);
        CSIP32(url);
        CSIP33(url, number);
        CSIP34(url);
        CSIP35(url);
        CSIP36(url);
        CSIP37(url);
        CSIP38(url);
        CSIP39(url);
        CSIP40(url);
        CSIP41(url, fileSizeInBytes);
        CSIP42(url);
        CSIP43(url, md5Checksum);
        CSIP44(url);

        Newline();
        Console.Write("</mets:mdRef>");
        Newline();
        Console.Write("</mets:digiprovMD>");


        // 45 -57 is rightMD if we have any
        /*
        Console.Write("</mets:mdRef>");
        CSIP45(url);
        CSIP46(url, number);
        CSIP47(url);
        CSIP48(url);
        CSIP49(url);
        CSIP50(url);
        CSIP51(url);
        CSIP52(url);
        CSIP53(url);
        CSIP54(url);
        CSIP55(url);
        CSIP56(url);
        CSIP57(url);
        Console.Write("</mets:rightsMD>");

        */

        // end of rights MDF



    }

    static string ComputeMD5Checksum(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }


    static string GetPath(string fullpath)
    {

        // get the real path of the file in the archive after E-ARK
        //Newline();
        //Console.Write("in getpath" + fullpath);

         string[] parts = fullpath.Split("E-ARK\\");

        //Console.WriteLine("READ back " + parts[0] + " " + parts[1]);
        //parts 1 will be the file
        return (parts[1]);

    }

    /* CSIP Definitions
     * 
     * 
     * 
     * 
     */



    public static void CSIP1(string path)
    {

        string string1 = "OBJID=\"csip-mets-example\"";
        Console.Write(string1);

    }
    public static void CSIP2(string path)
    {

        //TYPE = "Datasets"

        string string1 = "TYPE=\"Datasets\"";
        Console.Write(" " + string1);


    }
    public static void CSIP3(string path)
    {

    }
    public static void CSIP4(string path)
    {
        //csip:CONTENTINFORMATIONTYPE="MIXED"

        string string1 = "csip:CONTENTINFORMATIONTYPE=\"MIXED\"";
        Console.Write(" " + string1);
    }
    public static void CSIP5(string path)
    {
        //PROFILE = "https://earkcsip.dilcis.eu/profile/E-ARK-CSIP.xml"

        string string1 = "PROFILE=\"https://earkcsip.dilcis.eu/profile/E-ARK-CSIP.xml\"";
        Console.Write(" " + string1);
    }
    public static void CSIP6(string path) { }
    public static void CSIP7(string path)
    {
        // CREATEDATE="2024-02-14T13:33:50.698+01:00"

        
        Console.Write(" " + "CREATEDATE=" + '\"' + GetDate() + '\"');
    }
    public static void CSIP8(string path)
    {

        // LASTMODDATE="2024-02-14T13:33:50.698+01:00"

        Console.Write(" " + "LASTMODDATE=" + '\"' + GetDate() + '\"');


    }
    public static void CSIP9(string path)
    {
        //csip:OAISPACKAGETYPE="SIP"
        Console.Write(" " + "csip:OAISPACKAGETYPE=\"SIP\"" + ' ');

    }
    public static void CSIP10(string path)
    {

        
        Newline();
        Console.Write("<mets:agent" + " ");
    }
    public static void CSIP11(string path)
    {

        Console.Write(" " + "ROLE=\"CREATOR\"" + " ");

    }
    public static void CSIP12(string path)
    {

        Console.Write(" " + "TYPE=\"OTHER\"" + " ");

    }
    public static void CSIP13(string path)
    {
        Console.Write(" " + "OTHERTYPE=\"SOFTWARE\"" + " ");
        Console.Write(">");
        Newline();
    }
    public static void CSIP14(string path)
    {

        //< mets:name >METS-BUILD</mets:name >
        Console.Write("<mets:name>METS-BUILD</mets:name>");
        Newline();
    }
    public static void CSIP15(string path)
    {
        // <mets:note csip:NOTETYPE="SOFTWARE VERSION">2.1.0-beta.7</mets:note>
        Console.Write("<mets:note");
    }
    public static void CSIP16(string path)
    {
        Console.Write(" " + "csip:NOTETYPE=\"SOFTWARE VERSION\">2.1.8-beta.7</mets:note>");
    }
    public static void CSIP17(string path)
    {
        //Console.Write("in CSIP17");

        Console.Write(@"<mets:dmdSec");
    }
    public static void CSIP18(string path, int number)
    {

        // must be uniqeue
        Console.Write(" " + "ID=\"ead-file{0}\"", number);
    }
    public static void CSIP19(string path)
    {

        Console.Write(" " + "CREATED=" + '\"' + GetDate() + '\"');
    }
    public static void CSIP20(string path)
    {

        Console.Write(" " + "STATUS=\"CURRENT\">");
    }
    public static void CSIP21(string path)
    {

        Newline();
        Console.Write("<mets:mdRef");
    }
    public static void CSIP22(string path)
    {

        Console.Write(" " + "LOCTYPE=\"URL\"");
    }
    public static void CSIP23(string path)
    {

        Console.Write(" " + "xlink:type=\"simple\"");
    }
    public static void CSIP24(string path)
    {
        Console.Write(" " + "xlink:href=\"{0}\"", path);
    }
    public static void CSIP25(string path)
    {

        Console.Write(" " + "MDTYPE=\"EAD\"");

    }
    public static void CSIP26(string path)
    {

        Console.Write(" " + "MIMETYPE=\"ápplication/xml\"");

    }
    public static void CSIP27(string path, long size)
    {

        Console.Write(" " + "SIZE=\"{0}\"", size);

    }
    public static void CSIP28(string path)
    {

        Console.Write(" " + "CREATED=" + '\"' + GetDate() + '\"');
    }
    public static void CSIP29(string path, string md5Checksum)
    {

        Console.Write(" " + "CHECKSUM=\"{0}\"", md5Checksum);
    }
    public static void CSIP30(string path)
    {

        Console.Write(" " + "CHECKSUMTYPE=\"{0}\"", "MD5");

    }


    // 31 to 57 is for PREMIS files only
    public static void CSIP31(string path)
    {

        Console.WriteLine(@"<mets:amdSec>");
    }
    public static void CSIP32(string path)
    {

        Console.Write("<mets:digiprovMD");
    }
    public static void CSIP33(string path, int number)
    {

        Console.Write(" " + "ID=\"premis-file{0}\"", number);
    }
    public static void CSIP34(string path)
    {

        Console.Write(" " + "STATUS=\"CURRENT\">");
        Newline();

    }
    public static void CSIP35(string path)
    {

        Console.Write("<mets:mdRef");

    }
    public static void CSIP36(string path)
    {

        Console.Write(" " + "LOCTYPE=\"URL\"");

    }
    public static void CSIP37(string path)
    {

        Console.Write(" " + "xlink:type=\"simple\"");

    }
    public static void CSIP38(string path)
    {

        Console.Write(" " + "xlink:href=\"{0}\"", path);

    }
    public static void CSIP39(string path)
    {

        Console.Write(" " + "MDTYPE=\"PREMIS\"");

    }
    public static void CSIP40(string path)
    {

        Console.Write(" " + "MIMETYPE=\"text/xml\"");
    }
    public static void CSIP41(string path, long size)
    {

        Console.Write(" " + "SIZE=\"{0}\"", size);

    }
    public static void CSIP42(string path)
    {

        Console.Write(" " + "CREATED=" + '\"' + GetDate() + '\"');
    }
    public static void CSIP43(string path, string md5Checksum)
    {

        Console.Write(" " + "CHECKSUM=\"{0}\"", md5Checksum);
    }
    public static void CSIP44(string path)
    {

        Console.Write(" " + "CHECKSUMTYPE=\"{0}\"" + ">", "MD5");
    }

    // 45-57 is rights MD - not a requirement
    // depends how a rights file is defined - TBD
    public static void CSIP45(string path)
    {

        Newline();
        Console.Write("<mets:rightsMD");
    }
    public static void CSIP46(string path, int number)
    {

        Console.Write(" " + "ID={0}", number);
    }
    public static void CSIP47(string path)
    {

        Console.Write(" " + "STATUS=\"CURRENT\"");

    }
    public static void CSIP48(string path) { }
    public static void CSIP49(string path) { }
    public static void CSIP50(string path) { }
    public static void CSIP51(string path) { }
    public static void CSIP52(string path) { }
    public static void CSIP53(string path) { }
    public static void CSIP54(string path) { }
    public static void CSIP55(string path) { }
    public static void CSIP56(string path) { }
    public static void CSIP57(string path) { }


    public static void CSIP58() {

        Console.Write(@"<mets:fileSec");

    }
    public static void CSIP59(string val) {
   
        Console.Write(" " + "ID=\"{0}\">", val);
    }
    public static void CSIP60(string value, string use) {

        Newline();
        Console.Write("<mets:fileGrp ID=\"{0}\" USE=\"{1}\">", value, use);
    }
    public static void CSIP61(string path) { }
    public static void CSIP62(string path) { }
    public static void CSIP63(string path) { }
    public static void CSIP64(string path) { }
    public static void CSIP65(string path) { }
    public static void CSIP66(string path) {

        Newline();
        Console.Write("<mets:file");

    }
    public static void CSIP67(string value) {

        Console.Write(" " + "ID=\"{0}\"", value);
    }
    public static void CSIP68(string path) {

        //string mt = MimeMapping.GetMimeMapping(path);
        //string mt = MimeKit.MimeTypes.GetMimeType(path);
        string mt = GetMimeType(path);
        Console.Write(" " + "MIMETYPE=\"{0}\"", mt) ;
    }
    public static void CSIP69(long size) {

        Console.Write(" " + "SIZE=\"{0}\"", size);
    }
    public static void CSIP70(string path) { 
    
    Console.Write(" " + "CREATED=\"{0}\"", GetDate());
    }
    public static void CSIP71(string ck) {

        Console.Write(" " + "CHECKSUM=\"{0}\"", ck);
    }
    public static void CSIP72(string path) {

        Console.Write(" " + "CHECKMSUMTYPE=\"MD5\"");
    }
    public static void CSIP73(string ownerid) {

        Console.Write(" " + "OWNERID=\"{0}\">",ownerid );
    }
    public static void CSIP74(string path) { }
    public static void CSIP75(string path) { }
    public static void CSIP76(string path) {

        Newline();
        Console.Write("<mets:FLocat");
    
    }
    public static void CSIP77(string path) {
        Console.Write(" " + "LOCTYPE='URL'");
    }
    public static void CSIP78(string path) {

        Console.Write(" " + "xlink:type=\"simple\"");
    }
    public static void CSIP79(string path) {
        Console.Write(" " + "xlink:href=\"{0}\">", GetPath(path));
        Newline();
        Console.WriteLine("</mets:FLocat>");
    }
    public static void CSIP80(string path) {

        Console.Write("<mets:structMap");
    
    
    }
    public static void CSIP81(string path) {

        Console.Write(" " + "TYPE=\"PHYSICAL\"");
    }
    public static void CSIP82(string path) {
        Console.Write(" " + "LABEL=\"CSIP\"");

    }
    public static void CSIP83(string path) {

        Console.Write(" " + "ID=\"struct-map-1\"");
    }
    public static void CSIP84(string path) {

        Newline();
        Console.Write("<mets:div");
    


    }
    public static void CSIP85(string path)
    {

        Console.Write(" " + "ID=\"struct-map-metadata-div\"");
    }

    // defunt
    /*
    public static void CSIP86(string path) { }

    public static void CSIP87(string path) { }
    */
    public static void CSIP88(string path) {

        Console.Write(" " + "LABEL=\"metadata\"");

    }
    public static void CSIP89(string path) { }
    public static void CSIP90(string path) { }
    public static void CSIP91(string path) {

        Console.Write(" " + "AMID=");

    }
    public static void CSIP92(string path) {

        Console.Write(" " + "DMID=");

    }
    public static void CSIP93(string path) {

       
        Console.Write("<mets:div");

    }
    public static void CSIP94(string path) {
        Console.Write(" " + "ID=\"struct-map-documenation-div\"");

    }
    public static void CSIP95(string path) {

        Console.Write(" " + "LABEL=\"documenatation\"");
        Console.Write(">");
    }
    public static void CSIP96(string path) { }
    public static void CSIP97(string path) { }
    public static void CSIP98(string path) { }
    public static void CSIP99(string path) { }
    public static void CSIP100(string path) { }
    public static void CSIP101(string path) { }
    public static void CSIP102(string path) { }
    public static void CSIP103(string path) { }
    public static void CSIP104(string path) { }
    public static void CSIP105(string path) { }
    public static void CSIP106(string path) { }
    public static void CSIP107(string path) { }
    public static void CSIP108(string path) { }
    public static void CSIP109(string path) { }
    public static void CSIP110(string path) { }
    public static void CSIP111(string path) { }
    public static void CSIP112(string path) { }
    public static void CSIP113(string path) { }
    public static void CSIP114(string path) { }
    public static void CSIP115(string path) { }
    public static void CSIP116(string path) { }
    public static void CSIP117(string path)
    {

        // write the mets hdr start
        Console.Write(@"<mets:metsHdr");


    }






}