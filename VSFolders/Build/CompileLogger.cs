using System;
using System.Collections.Generic;
using System.Windows.Documents;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VSFolders.Build
{
    public class CompileLogger : ILogger
    {
        private IEventSource _eventSource;
        OutputWindowPane _buildPane;
        public void Initialize(IEventSource eventSource)
        {
            _eventSource = eventSource;
            //eventSource.AnyEventRaised += EventSourceOnAnyEventRaised;
            _eventSource.BuildFinished += eventSource_BuildFinished;
            _eventSource.BuildStarted += eventSource_BuildStarted;
            _eventSource.ErrorRaised += eventSource_ErrorRaised;
            _eventSource.MessageRaised += eventSource_MessageRaised;
            _eventSource.ProjectStarted += eventSource_ProjectStarted;
            _eventSource.ProjectFinished += eventSource_ProjectFinished;
            _eventSource.WarningRaised += eventSource_WarningRaised;
            _eventSource.StatusEventRaised += eventSource_StatusEventRaised;
            _buildPane = CreatePane("Custom Build");
            _buildPane.Clear();
            _buildPane.Activate();
        }

        void eventSource_StatusEventRaised(object sender, BuildStatusEventArgs e)
        {
            //WriteLine(e.Message);
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            WriteLine(e.Message);
        }

        void eventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            WriteLine(e.Message);
        }

        void eventSource_ProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            WriteLine(e.Message);
        }

        void eventSource_MessageRaised(object sender, BuildMessageEventArgs e)
        {
            //WriteLine(e.Message);
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            WriteLine(e.Message);
        }

        void eventSource_BuildStarted(object sender, BuildStartedEventArgs e)
        {
            WriteLine(e.Message);
        }

        private void eventSource_BuildFinished(object sender, BuildFinishedEventArgs e)
        {
            WriteLine(e.Message);
        }

        OutputWindowPane CreatePane(string title)
        {
            DTE2 dte = (DTE2)GetService(typeof(DTE));
            OutputWindowPanes panes =
                dte.ToolWindows.OutputWindow.OutputWindowPanes;

            try
            {
                // If the pane exists already, return it.
                return panes.Item(title);
            }
            catch (ArgumentException)
            {
                // Create a new pane.
                return panes.Add(title);
            }
        }

        private void Write(string value)
        {
            _buildPane.OutputString(value);
        }

        private void WriteLine(string value)
        {
            _buildPane.OutputString(value);
            _buildPane.OutputString(Environment.NewLine);
        }


        private void EventSourceOnAnyEventRaised(object sender, BuildEventArgs buildEventArgs)
        {
            //_buildPane.OutputString(buildEventArgs.Message + Environment.NewLine);
        }

        private static object GetService(Type type)
        {
            return ServiceProvider.GlobalProvider.GetService(type);
        }

        private static DTE2 DTE { get { return GetService(typeof(DTE)) as DTE2; } }

        public void Shutdown()
        {
            _eventSource.BuildFinished -= eventSource_BuildFinished;
            _eventSource.BuildStarted -= eventSource_BuildStarted;
            _eventSource.ErrorRaised -= eventSource_ErrorRaised;
            _eventSource.MessageRaised -= eventSource_MessageRaised;
            _eventSource.ProjectStarted -= eventSource_ProjectStarted;
            _eventSource.ProjectFinished -= eventSource_ProjectFinished;
            _eventSource.WarningRaised -= eventSource_WarningRaised;
            _eventSource.StatusEventRaised -= eventSource_StatusEventRaised;
        }

        public LoggerVerbosity Verbosity { get; set; }
        public string Parameters { get; set; }
    }
}
