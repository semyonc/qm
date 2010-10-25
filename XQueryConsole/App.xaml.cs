using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Input;

using DataEngine.CoreServices;
using DataEngine.XQuery;

namespace XQueryConsole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = GetType().Assembly.GetManifestResourceStream("XQueryConsole.XQuery.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("XQuery", new string[] { ".xq" }, customHighlighting);

            // set keyboard shortcuts
            KeyGesture ExecuteCmdKeyGesture = new KeyGesture(Key.F5, ModifierKeys.None);
            KeyGesture CancelExecuteCmdKeyGesture = new KeyGesture(Key.Escape, ModifierKeys.None);
            KeyGesture ShowResultsCmdKeyGesture = new KeyGesture(Key.R, ModifierKeys.Control);
            ((RoutedUICommand)QueryPage.ExecuteCommand).InputGestures.Add(ExecuteCmdKeyGesture);
            ApplicationCommands.Stop.InputGestures.Add(CancelExecuteCmdKeyGesture);
            ((RoutedUICommand)XQueryConsole.MainWindow.ShowResultsCommand).InputGestures.Add(ShowResultsCmdKeyGesture);
            
            // register extension function in wmh namespace
            XQueryFunctionTable.Register(typeof(WmhFuncs));       
        }
    }
}
