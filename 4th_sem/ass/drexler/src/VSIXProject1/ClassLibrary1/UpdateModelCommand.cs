﻿//-----------------------------------------------------------------------
// <copyright file="UpdateModelCommand.cs" company="MD Development">
//     Copyright (c) MD Development. All rights reserved.
// </copyright>
// <author>Michael Drexler</author>
//-----------------------------------------------------------------------
namespace ClassLibrary1
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml;
    using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Presentation;
    using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Uml;
    using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
    using Microsoft.VisualStudio.Uml.Classes;
    using Microsoft.VisualStudio.Modeling.Diagrams;

    /// <summary>
    /// Custom context menu command extension
    /// See http://msdn.microsoft.com/en-us/library/ee329481(v=vs.120).aspx
    /// </summary>
    [Export(typeof(ICommandExtension))]
    [ClassDesignerExtension] // TODO: Add other diagram types if needed
    class UpdateModelCommand : ICommandExtension
    {
        /// <summary>
        /// Background thread which updates the GUI (class diagram)
        /// </summary>
        private Thread updateThread;

        /// <summary>
        /// Flag whether the background thread should update the GUI or not
        /// </summary>
        private static bool isAutomaticallyUpdateActivated = false;

        /// <summary>
        /// Flag to see if the log file has changed and the GUI has to be updated
        /// </summary>
        private static bool areUpdatesAvailable = false;

        /// <summary>
        /// Handles privileged access to ressources for threads
        /// </summary>
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// Delegate for the UIThreadHolder of the class diagram
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="comments"></param>
        public delegate void UpdateModelDelegate(XmlDocument xmlFile, IEnumerable<IComment> comments);

        /// <summary>
        /// Gets or sets the diagram context
        /// </summary>
        [Import]
        public IDiagramContext context { get; set; }

        /// <summary>
        /// Gets or sets the value for the linked undo context
        /// </summary>
        [Import]
        public ILinkedUndoContext linkedUndoContext { get; set; }

        /// <summary>
        /// Gets or sets the value of the menu command text
        /// </summary>
        public string Text
        {
            get { return "Activate Update Model"; }
        }

        /// <summary>
        /// Method to show error messages to the user
        /// </summary>
        /// <param name="ex"></param>
        private void DisplayErrorMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, "Ein Fehler ist aufgetreten!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Thread functionality to process
        /// </summary>
        /// <param name="obj"></param>
        private void DoWork(object obj)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(@"C:\Temp", "AspectLog.xml");
            watcher.EnableRaisingEvents = isAutomaticallyUpdateActivated;
            watcher.Created += watcher_Changed;
            watcher.Changed += watcher_Changed;
            DiagramView uiThreadHolder = context.CurrentDiagram.GetObject<Diagram>().ActiveDiagramView;

            while (isAutomaticallyUpdateActivated)
            {
                if(areUpdatesAvailable)
                {
                    XmlDocument logFile = this.ReadLogFile();
                    IEnumerable<IComment> comments = this.ReadCommentsFromUMLClassDiagram();
                    uiThreadHolder.Invoke(new UpdateModelDelegate(UpdateClassDiagram), logFile, comments);

                    mutex.WaitOne();
                    areUpdatesAvailable = false;
                    mutex.ReleaseMutex();
                }

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Update all comment shapes with the logged informations for the annotated classes.
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="comments"></param>
        public static void UpdateClassDiagram(XmlDocument logFile, IEnumerable<IComment> comments)
        {
            foreach (var comment in comments)
            {
                XmlNode classNode = logFile.SelectSingleNode(string.Format("descendant::{0}", comment.Description));
                if (classNode != null)
                {
                    XmlNode instanceCounter = classNode.SelectSingleNode("descendant::InstanceCounter");
                    XmlNode methodCounter = classNode.SelectSingleNode("descendant::MethodCallsCounter");
                    comment.Body = comment.Description;
                    if (instanceCounter != null)
                    {
                        comment.Body += string.Concat(Environment.NewLine, "InstanceCounter: ", instanceCounter.InnerText, Environment.NewLine);
                    }

                    if (methodCounter != null)
                    {
                        comment.Body += string.Concat("MethodCounter: ", methodCounter.InnerText);
                    }
                }
            }
        }

        /// <summary>
        /// Get all comment shapes from the model store
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IComment> ReadCommentsFromUMLClassDiagram()
        {
            try
            {
                IClassDiagram diagram = context.CurrentDiagram as IClassDiagram;
                IModelStore store = diagram.ModelStore;
                IPackage rootPackage = store.Root;
                IEnumerable<IComment> comments = store.AllInstances<IComment>();

                return comments;
            }
            catch(Exception ex)
            {
                this.DisplayErrorMessage(ex);
            }

            return null;
        }

        /// <summary>
        /// Read the log file
        /// </summary>
        /// <returns></returns>
        private XmlDocument ReadLogFile()
        {
            mutex.WaitOne();
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Temp\AspectLog.xml");
            mutex.ReleaseMutex();

            return doc;
        }

        /// <summary>
        /// Update UML ClassDiagram if a log file was created or changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Better to add jobs the a queue which will be processed from a thread?
            mutex.WaitOne();
            areUpdatesAvailable = true;
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// Executes the update class diagram logic
        /// </summary>
        /// <param name="command"></param>
        public void Execute(IMenuCommand command)
        {
            // Start a background thread to update the GUI (class diagram)
            try
            {
                if(updateThread == null)
                {
                    updateThread = new Thread(new ParameterizedThreadStart(DoWork));
                    updateThread.IsBackground = true;
                    updateThread.Name = "UpdateUMLClassDiagramThread";
                }

                if (!isAutomaticallyUpdateActivated)
                {
                    isAutomaticallyUpdateActivated = true;
                    MessageBox.Show("Update is activated!", "Info", MessageBoxButtons.OK);

                    if(!updateThread.IsAlive)
                    {
                        updateThread.Start();
                    }
                    else
                    {
                        updateThread.Resume();
                    }
                }
                else
                {
                    if (updateThread.IsAlive)
                    {
                        updateThread.Suspend();
                    }

                    isAutomaticallyUpdateActivated = false;
                    MessageBox.Show("Update is deactivated!", "Info", MessageBoxButtons.OK);
                }
            }
            catch(ThreadStartException ex)
            {
                this.DisplayErrorMessage(ex);
            }
            catch(ThreadAbortException ex)
            {
                this.DisplayErrorMessage(ex);
            }
            catch(ThreadInterruptedException ex)
            {
                this.DisplayErrorMessage(ex);
            }
            catch(ThreadStateException ex)
            {
                this.DisplayErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the visibility of the MenuCommand item
        /// </summary>
        /// <param name="command"></param>
        public void QueryStatus(IMenuCommand command)
        {
            command.Visible = command.Enabled = true;
            command.Text = isAutomaticallyUpdateActivated ? "Deactivate Update Model" : "Activate Update Model";
        }
    }
}
