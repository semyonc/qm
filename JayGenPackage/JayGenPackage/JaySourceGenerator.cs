extern alias VSShell9;
extern alias VSShell10;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.IO;
using System.Text;
using Microsoft.Win32;
using VSShell9::Microsoft.VisualStudio;
using VSShell9::Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj80;
using System.Diagnostics;
using VSOLE = Microsoft.VisualStudio.OLE.Interop;
using EnvDTE;
using EnvDTE80;
using System.Reflection;

namespace wmhelp.VStudio.Generator
{
    /// <summary>
    /// This is the generator class. 
    /// When setting the 'Custom Tool' property of a C#, VB, or J# project item to "JayClassGenerator", 
    /// the GenerateCode function will get called and will return the contents of the generated file 
    /// to the project system
    /// </summary>
    [ComVisible(true)]
    [Guid("8DA02118-5B10-4efa-9B0D-CB0C779EE41F")]
    [CodeGeneratorRegistration(typeof(JaySourceGenerator), "JaySourceGenerator", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [VSShell9::Microsoft.VisualStudio.Shell.ProvideObject(typeof(JaySourceGenerator))]
    class JaySourceGenerator : BaseCodeGeneratorWithSite
    {
#pragma warning disable 0414
        //The name of this generator (use for 'Custom Tool' property of project item)
        internal static string name = "JaySourceGenerator";
#pragma warning restore 0414        

        private StringBuilder _jayOutput;
        private List<string> _jayErrors;
        private OutputWindowPane _generalPane = null;

        /// <summary>
        /// Function that builds the contents of the generated file based on the contents of the input file
        /// </summary>
        /// <param name="inputFileContent">Content of the input file</param>
        /// <returns>Generated file as a byte array</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            try
            {
                ProjectItem projItem = GetProjectItem();
                if (!projItem.Saved)
                    projItem.Save("");

                DTE2 dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE.10.0");
                if (dte2 == null)
                    GeneratorWarning(0, "Can't obtain DTE2", 0, 0);
                else
                {
                    _generalPane = dte2.ToolWindows.OutputWindow.ActivePane;
                    _generalPane.Activate();
                }
                                                                 
                System.Diagnostics.Process wjay = new System.Diagnostics.Process();

                try
                {
                    wjay.StartInfo.WorkingDirectory = Path.GetDirectoryName(projItem.get_FileNames(1));
                    FileInfo fi = new FileInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\wjay.exe");
                    if (fi.Exists)
                        wjay.StartInfo.FileName = fi.FullName;
                    else
                        wjay.StartInfo.FileName = "c:\\utils\\wjay.exe";
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(projItem.get_FileNames(1)), "yydebug")))
                        wjay.StartInfo.Arguments = "-t -c -v " + Path.GetFileName(projItem.get_FileNames(1));
                    else
                        wjay.StartInfo.Arguments = "-c -v " + Path.GetFileName(projItem.get_FileNames(1));
                    wjay.StartInfo.UseShellExecute = false;
                    wjay.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    wjay.StartInfo.CreateNoWindow = true;

                    wjay.StartInfo.RedirectStandardInput = true;
                    
                    wjay.StartInfo.RedirectStandardOutput = true;
                    _jayOutput = new StringBuilder();
                    wjay.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);

                    wjay.StartInfo.RedirectStandardError = true;
                    _jayErrors = new List<string>();
                    wjay.ErrorDataReceived += new DataReceivedEventHandler(ProcessErrorHandler);

                    wjay.Start();
                    wjay.BeginOutputReadLine();
                    wjay.BeginErrorReadLine();

                    StreamReader Skeleton = File.OpenText(wjay.StartInfo.WorkingDirectory + "\\skeleton.cs");
                    wjay.StandardInput.Write(Skeleton.ReadToEnd());
                    wjay.StandardInput.Flush();
                    wjay.StandardInput.Close();

                    wjay.WaitForExit();

                    if (wjay.ExitCode == 0)
                    {
                        StreamReader youtput = new StreamReader(wjay.StartInfo.WorkingDirectory + "\\y.output");
                        List<String> slist = new List<string>();
                        while (!youtput.EndOfStream)
                        {
                            if (slist.Count > 10)
                                slist.RemoveAt(0);
                            slist.Add(youtput.ReadLine());
                        }
                        youtput.Close();

                        if (_generalPane != null)
                        {
                            _generalPane.OutputString("Running wjay.exe:\n");
                            for (int k = 1; k < 3 && slist.Count - k > 0; k++)
                                if (!String.IsNullOrEmpty(slist[slist.Count - k]))
                                    _generalPane.OutputString(slist[slist.Count - k] + "\n");
                            System.Windows.Forms.Application.DoEvents();
                        }

                        //Get the Encoding used by the writer. We're getting the WindowsCodePage encoding, 
                        //which may not work with all languages
                        Encoding enc = Encoding.ASCII;

                        //Get the preamble (byte-order mark) for our encoding
                        byte[] preamble = enc.GetPreamble();
                        int preambleLength = preamble.Length;

                        //Convert the writer contents to a byte array
                        byte[] body = enc.GetBytes(_jayOutput.ToString());

                        //Prepend the preamble to body (store result in resized preamble array)
                        Array.Resize<byte>(ref preamble, preambleLength + body.Length);
                        Array.Copy(body, 0, preamble, preambleLength, body.Length);

                        //Return the combined byte array
                        return preamble;
                    }
                    else
                    {
                        foreach(string s in _jayErrors)
                            GeneratorError(1, s, 1, 1);
                        
                        //Returning null signifies that generation has failed
                        return null;
                    }
                }
                finally
                {                    
                    wjay.Close();
                }                                
            }
            catch (Exception e)
            {
                GeneratorError(4, e.ToString(), 1, 1);
                //Returning null signifies that generation has failed
                return null;
            }
        }

        private void ProcessOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (outLine.Data != null)
            {
                _jayOutput.Append(Environment.NewLine);
                _jayOutput.Append(outLine.Data);
            }
        }

        private void ProcessErrorHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            // Collect the sort command output.
            if (outLine.Data != null)
            {
                _jayErrors.Add(outLine.Data);
                if (_generalPane != null)
                {
                    _generalPane.OutputString(outLine.Data);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
        }
    }
}
