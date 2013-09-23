///**
/// Copyright 2008 Mikko Halttunen
/// This program is free software; you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation; version 2 of the License.
/// 
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with this program; if not, write to the Free Software Foundation, Inc.,
/// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
/// 
///**

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Thread=EnvDTE.Thread;
using System.Windows.Forms;

[assembly: log4net.Config.XmlConfigurator(ConfigFileExtension = "log4net", Watch = true)]

namespace VisualStudioComparisonTools
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ComparisonConfig config = new ComparisonConfig();

        enum ComparisonType { File, Directory }

        private KeyValuePair<string, ComparisonType> previousFilePath = new KeyValuePair<string, ComparisonType>();

        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Debug("Start connectMode=" + connectMode);

            _applicationObject = (DTE2) application;
            _addInInstance = (AddIn) addInInst;

            //AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            try
            {
                
                    log.Debug("Loading config");
                config.Load();
            }
            catch (Exception ex)
            {
                log.Debug(ex);
                throw;
            }

            if (connectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                log.Debug("Getting menubar");

                object[] contextGUIDS = new object[] {};
                Commands2 commands = (Commands2) _applicationObject.Commands;

                try
                {
                    log.Debug("Getting \"Code Window\" command bar");

                    //Find the "Code Window" command bar:
                    Microsoft.VisualStudio.CommandBars.CommandBar codeWindowCommandBar =
                        ((Microsoft.VisualStudio.CommandBars.CommandBars) _applicationObject.CommandBars)["Code Window"];

                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(_addInInstance, "VisualStudioComparisonTools",
                                                                "Compare with Clipboard",
                                                                "Executes the command for VisualStudioComparisonTools",
                                                                true, 556, ref contextGUIDS,
                                                                (int) vsCommandStatus.vsCommandStatusSupported +
                                                                (int) vsCommandStatus.vsCommandStatusEnabled,
                                                                (int) vsCommandStyle.vsCommandStylePictAndText,
                                                                vsCommandControlType.vsCommandControlTypeButton);

                    log.Debug("Getting command " + command.Name);

                    if ((command != null))
                    {
                        
                            log.Debug("Adding to menu count:" + codeWindowCommandBar.Controls.Count);
                        command.AddControl(codeWindowCommandBar, codeWindowCommandBar.Controls.Count);
                    }
                }
                catch (System.ArgumentException ex)
                {
                    log.Debug(ex);
                }

                try
                {
                    log.Debug("Getting \"Item\" command bar");

                    //Find the "Item" command bar (solution explorer file, not folder and not solution or project):
                    Microsoft.VisualStudio.CommandBars.CommandBar codeWindowCommandBar =
                        ((Microsoft.VisualStudio.CommandBars.CommandBars) _applicationObject.CommandBars)["Item"];

                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(_addInInstance,
                                                                "VisualStudioComparisonToolsSolutionExplorer",
                                                                "Compare with Clipboard",
                                                                "Executes the command for VisualStudioComparisonTools",
                                                                true, 556, ref contextGUIDS,
                                                                (int) vsCommandStatus.vsCommandStatusSupported +
                                                                (int) vsCommandStatus.vsCommandStatusEnabled,
                                                                (int) vsCommandStyle.vsCommandStylePictAndText,
                                                                vsCommandControlType.vsCommandControlTypeButton);

                    log.Debug("Getting command " + command.Name);

                    if ((command != null))
                    {
                        
                            log.Debug("Adding to menu count:" + codeWindowCommandBar.Controls.Count);
                        command.AddControl(codeWindowCommandBar, codeWindowCommandBar.Controls.Count);
                    }
                }
                catch (System.ArgumentException ex)
                {
                    log.Debug(ex);
                }

                try
                {
                    log.Debug("Getting \"Item\" command bar");

                    //Find the "Item" command bar (solution explorer file, not folder and not solution or project):
                    Microsoft.VisualStudio.CommandBars.CommandBar codeWindowCommandBar =
                        ((Microsoft.VisualStudio.CommandBars.CommandBars) _applicationObject.CommandBars)["Item"];

                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(_addInInstance, "CompareFilesSolutionExplorer",
                                                                "Compare Selected Files",
                                                                "Executes the command for VisualStudioComparisonTools",
                                                                true, 585, ref contextGUIDS,
                                                                (int) vsCommandStatus.vsCommandStatusSupported +
                                                                (int) vsCommandStatus.vsCommandStatusEnabled,
                                                                (int) vsCommandStyle.vsCommandStylePictAndText,
                                                                vsCommandControlType.vsCommandControlTypeButton);

                    log.Debug("Getting command " + command.Name);

                    if ((command != null))
                    {
                        
                            log.Debug("Adding to menu count:" + codeWindowCommandBar.Controls.Count);
                        command.AddControl(codeWindowCommandBar, codeWindowCommandBar.Controls.Count);
                    }
                }
                catch (System.ArgumentException ex)
                {
                    log.Debug(ex);
                }

                try
                {
                    log.Debug("Getting \"Folder\" command bar");

                    //Find the "Folder" command bar (solution explorer file, not folder and not solution or project):
                    Microsoft.VisualStudio.CommandBars.CommandBar codeWindowCommandBar =
                        ((Microsoft.VisualStudio.CommandBars.CommandBars) _applicationObject.CommandBars)["Folder"];

                    //Add a command to the Commands collection:
                    Command command = commands.AddNamedCommand2(_addInInstance, "CompareFoldersSolutionExplorer",
                                                                "Compare Selected Folders",
                                                                "Executes the command for VisualStudioComparisonTools",
                                                                true, 357, ref contextGUIDS,
                                                                (int) vsCommandStatus.vsCommandStatusSupported +
                                                                (int) vsCommandStatus.vsCommandStatusEnabled,
                                                                (int) vsCommandStyle.vsCommandStylePictAndText,
                                                                vsCommandControlType.vsCommandControlTypeButton);

                    log.Debug("Getting command " + command.Name);

                    if ((command != null))
                    {
                        
                            log.Debug("Adding to menu count:" + codeWindowCommandBar.Controls.Count);
                        command.AddControl(codeWindowCommandBar, codeWindowCommandBar.Controls.Count);
                    }
                }
                catch (System.ArgumentException ex)
                {
                    log.Debug(ex);
                }
            }
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception exception = (Exception) args.ExceptionObject;
            log.Error(exception);
            ShowThreadExceptionDialog(exception);
        }

        public static void ShowThreadExceptionDialog(Exception exception)
        {
            MessageBox.Show("Sorry. There was an error in Visual Studio Comparison Tools. See log for more details. " + exception.Message);
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param term='commandName'>The name of the command to determine state for.</param>
        /// <param term='neededText'>Text that is needed for the command.</param>
        /// <param term='status'>The state of the command in the user interface.</param>
        /// <param term='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status,
                                ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == "VisualStudioComparisonTools.Connect.VisualStudioComparisonTools")
                {
                    status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported |
                             vsCommandStatus.vsCommandStatusEnabled;
                    return;
                }
                else if (commandName == "VisualStudioComparisonTools.Connect.VisualStudioComparisonToolsSolutionExplorer")
                {
                    if (!_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count == 1)
                    {
                        status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported |
                                 vsCommandStatus.vsCommandStatusEnabled;
                    }
                    else
                    {
                        status = (vsCommandStatus) vsCommandStatus.vsCommandStatusUnsupported |
                                 vsCommandStatus.vsCommandStatusInvisible;
                    }

                    return;
                }
                else if (commandName == "VisualStudioComparisonTools.Connect.CompareFilesSolutionExplorer")
                {
                    if (_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count == 2)
                    {
                        status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported |
                                 vsCommandStatus.vsCommandStatusEnabled;
                    }
                    else
                    {
                        status = (vsCommandStatus) vsCommandStatus.vsCommandStatusUnsupported |
                                 vsCommandStatus.vsCommandStatusInvisible;
                    }

                    return;
                }
                else if (commandName == "VisualStudioComparisonTools.Connect.CompareFoldersSolutionExplorer")
                {
                    if (_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count == 2)
                    {
                        status = (vsCommandStatus) vsCommandStatus.vsCommandStatusSupported |
                                 vsCommandStatus.vsCommandStatusEnabled;
                    }
                    else
                    {
                        status = (vsCommandStatus) vsCommandStatus.vsCommandStatusUnsupported |
                                 vsCommandStatus.vsCommandStatusInvisible;
                    }

                    return;
                }
            }
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param term='commandName'>The name of the command to execute.</param>
        /// <param term='executeOption'>Describes how the command should be run.</param>
        /// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
        /// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
        /// <param term='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut,
                         ref bool handled)
        {
            log.Debug("Start");
            handled = false;
            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                bool isFileOnClipboard;
                string clipboardText = GetClipboard(out isFileOnClipboard);

                if (commandName == "VisualStudioComparisonTools.Connect.VisualStudioComparisonTools")
                {
                    
                    log.Debug("Command found (VisualStudioComparisonTools.Connect.VisualStudioComparisonTools)");

                    log.Debug("Saving all documents");
                    _applicationObject.Documents.SaveAll();
                    log.Debug("Saved all documents");

                    ComparisonWorkerProcess workerProcess = new ComparisonWorkerProcess(_applicationObject, config);
                    workerProcess.TextSelection = GetActiveTextSelection();
                    workerProcess.ComparisonFilePath1 = GetActiveDocumentFilePath();

                    if (!isFileOnClipboard || !File.Exists(clipboardText))
                    {
                        workerProcess.ClipboardText = clipboardText;
                    }
                    else
                    {
                        workerProcess.ComparisonFilePath2 = clipboardText;
                    }
                    

                    log.Debug("Starting comparison process");

                    System.Threading.Thread workerThread =
                        new System.Threading.Thread(new ThreadStart(workerProcess.OpenComparisonProcess));
                    workerThread.Start();

                    handled = true;
                }
                else if (commandName == "VisualStudioComparisonTools.Connect.VisualStudioComparisonToolsSolutionExplorer")
                {
                    
                        log.Debug(
                            "Command found (VisualStudioComparisonTools.Connect.VisualStudioComparisonToolsSolutionExplorer)");

                    if (!_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count == 1)
                    {
                        log.Debug("Saving all documents");
                        _applicationObject.Documents.SaveAll();
                        log.Debug("Saved all documents");

                        String filePath = _applicationObject.SelectedItems.Item(1).ProjectItem.get_FileNames(1);

                        
                            log.Debug("Document found. Comparing " + filePath + " to clipboard");

                        ComparisonWorkerProcess workerProcess = new ComparisonWorkerProcess(_applicationObject, config);
                        workerProcess.ComparisonFilePath1 = filePath;

                        if (!isFileOnClipboard || !File.Exists(clipboardText))
                        {
                            workerProcess.ClipboardText = clipboardText;
                        }
                        else
                        {
                            workerProcess.ComparisonFilePath2 = clipboardText;
                        }

                        log.Debug("Starting comparison process");

                        System.Threading.Thread workerThread =
                            new System.Threading.Thread(new ThreadStart(workerProcess.OpenComparisonProcess));
                        workerThread.Start();

                        handled = true;
                    }
                }
                else if (commandName == "VisualStudioComparisonTools.Connect.CompareFilesSolutionExplorer")
                {
                    
                        log.Debug("Command found (VisualStudioComparisonTools.Connect.CompareFilesSolutionExplorer)");

                    if (_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count == 2)
                    {
                        log.Debug("Saving all documents");
                        _applicationObject.Documents.SaveAll();
                        log.Debug("Saved all documents");

                        String filePath1 = _applicationObject.SelectedItems.Item(1).ProjectItem.get_FileNames(1);
                        String filePath2 = _applicationObject.SelectedItems.Item(2).ProjectItem.get_FileNames(1);
                        
                            log.Debug("Document1 found. Comparing " + filePath1 + " to clipboard");
                        
                            log.Debug("Document2 found. Comparing " + filePath2 + " to clipboard");

                        if (!File.Exists(filePath1) || !File.Exists(filePath2))
                        {
                            throw new Exception("Either one of selected files don't exist for some reason!");
                        }

                        ComparisonWorkerProcess workerProcess = new ComparisonWorkerProcess(_applicationObject, config);
                        workerProcess.ComparisonFilePath1 = filePath1;
                        workerProcess.ComparisonFilePath2 = filePath2;

                        log.Debug("Starting comparison process");

                        System.Threading.Thread workerThread =
                            new System.Threading.Thread(new ThreadStart(workerProcess.OpenComparisonProcess));
                        workerThread.Start();

                        handled = true;
                    }
                }
                else if (commandName == "VisualStudioComparisonTools.Connect.CompareFoldersSolutionExplorer")
                {
                    
                        log.Debug(
                            "Command found (VisualStudioComparisonTools.Connect.VisualStudioComparisonToolsSolutionExplorer)");

                    if (_applicationObject.SelectedItems.MultiSelect && _applicationObject.SelectedItems.Count == 2)
                    {
                        log.Debug("Saving all documents");
                        _applicationObject.Documents.SaveAll();
                        log.Debug("Saved all documents");

                        String filePath1 = _applicationObject.SelectedItems.Item(1).ProjectItem.get_FileNames(1);
                        String filePath2 = _applicationObject.SelectedItems.Item(2).ProjectItem.get_FileNames(1);
                        
                            log.Debug("Folder1 found. Comparing " + filePath1);
                        
                            log.Debug("Folder2 found. Comparing " + filePath2);

                        if (Directory.Exists(filePath1) && Directory.Exists(filePath2))
                        {
                            filePath1 = Path.GetDirectoryName(filePath1);
                            filePath2 = Path.GetDirectoryName(filePath2);
                        }
                        else
                        {
                            throw new Exception("Selected directories don't exist for some reason!");
                        }

                            log.Debug("Folder1 path cleaned. Comparing " + filePath1);
                            log.Debug("Folder2 path cleaned. Comparing " + filePath2);


                        ComparisonWorkerProcess workerProcess = new ComparisonWorkerProcess(_applicationObject, config);
                        workerProcess.ComparisonFilePath1 = filePath1;
                        workerProcess.ComparisonFilePath2 = filePath2;

                        log.Debug("Starting comparison process");

                        System.Threading.Thread workerThread =
                            new System.Threading.Thread(new ThreadStart(workerProcess.OpenComparisonProcess));
                        workerThread.Start();

                        handled = true;
                    }
                }
            }
            log.Debug("End handled=" + handled);
        }

        public TextSelection GetActiveTextSelection()
        {
            return (TextSelection) _applicationObject.ActiveDocument.Selection;
        }

        public string GetActiveDocumentFilePath()
        {
            return _applicationObject.ActiveDocument.FullName;
        }

        public string GetClipboard(out bool isFile)
        {
            log.Debug("Getting Clipboard: " + Clipboard.GetDataObject());
            isFile = false;
            
            if (Clipboard.ContainsFileDropList())
            {
                log.Debug("Found files in clipboard");
                StringCollection fileDropList = Clipboard.GetFileDropList();
                if (fileDropList != null && fileDropList.Count > 0)
                {
                    string fileDrop = fileDropList[0];
                    log.Debug("There are total of " + fileDropList.Count + " files in clipboard. Selecting first: " + fileDrop);
                    isFile = true;
                    return fileDrop;
                }
            }
            if (Clipboard.ContainsText())
            {
                log.Debug("Using text representation of clipboard");
                return Clipboard.GetText();
            }
            else
            {
                log.Debug("Nothing interesting in the clipboard");
                return "";
            }
        }

        private DTE2 _applicationObject;
        private AddIn _addInInstance;

        public void GetCommandBars()
        {
            foreach (Microsoft.VisualStudio.CommandBars.CommandBar bar in (Microsoft.VisualStudio.CommandBars.CommandBars) _applicationObject.CommandBars)
            {
                log.Fatal("CommandBar:" + bar.Name + " id=" + bar.Id + " index=" + bar.Index);
                foreach (CommandBarControl commandBarControl in bar.Controls)
                {
                    log.Fatal("CommandBarControl:" + commandBarControl.Caption + " id=" + commandBarControl.Id +
                              " index=" + commandBarControl.Index);
                }
            }
        }
    }
}