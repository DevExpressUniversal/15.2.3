#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Loggers;
using DevExpress.Persistent.Base;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.ExpressApp.Design.Commands {
	public enum CommandType { None, Run, RunNextStep, RunToCursor, StopExecuting }
	public class EasyTestManager {
		public static bool EnableTrace = false;
		private _DTE dte;
		private System.IServiceProvider serviceProvider;
		private ManualResetEvent manualResetEvent;
		private System.Threading.Thread testExecThread;
		private TestExecutor testExecutor;
		private String currentTestFileName;
		private CommandType prevCommandType = CommandType.None;
		private String cursorPositionFileName;
		private Int32 cursorPosition;
		private Boolean isWaiting;
		private MarkerManager markerManager;
		private PositionInScript previousCommandEndPosition = null;
		private Boolean IsStopped {
			get {
				TraceMethodEnter(this, "IsStopped");
				TraceValue("testExecThread", testExecThread);
				bool result = (testExecThread == null) || !testExecThread.IsAlive;
				TraceValue("result", result);
				TraceMethodExit(this, "IsStopped");
				return result;
			}
		}
		private Boolean IsWaiting {
			get {
				TraceMethodEnter(this, "IsRunAvailable");
				TraceValue("isWaiting", isWaiting);
				TraceValue("testExecThread", testExecThread);
				bool result = (testExecThread != null) && testExecThread.IsAlive && isWaiting;
				TraceValue("result", result);
				TraceMethodExit(this, "IsWaiting");
				return result;
			}
		}
		public Boolean IsRunning {
			get { return (testExecThread != null) && testExecThread.IsAlive && !isWaiting; }
		}
		private SolutionConfiguration FindSolutionConfiguration(String name) {
			foreach(SolutionConfiguration solutionConfiguration in dte.Solution.SolutionBuild.SolutionConfigurations) {
				if(solutionConfiguration.Name == name) {
					return solutionConfiguration;
				}
			}
			return null;
		}
		private void SaveDocuments() {
			foreach(object doc in dte.Documents) {
				if(doc is Document) {
					Document document = (Document)doc;
					if(!document.Saved) {
						document.Save(null);
					}
				}
			}
		}
		private Options GetOptions(String testPath) {
			String optionFileName = Path.Combine(testPath, "Config.xml");
			if(!File.Exists(optionFileName)) {
				throw new Exception(String.Format(
					"It's impossible to run test. " + Environment.NewLine +
					"The easy test 'Config.xml' file not found. Search path: '{0}'", testPath));
			}
			using(FileStream optionsStream = new FileStream(optionFileName, FileMode.Open, FileAccess.Read, FileShare.None)) {
				try {
					return Options.LoadOptions(optionsStream, null, null, testPath);
				}
				catch(Exception e) {
					throw new Exception("Error when loading the '" + optionFileName + "' file. " + e.Message);
				}
			}
		}
		private Boolean BuildSolution() {
			SolutionConfiguration previousSolutionConfiguration = dte.Solution.SolutionBuild.ActiveConfiguration;
			SolutionConfiguration solutionConfiguration = FindSolutionConfiguration("EasyTest");
			if(solutionConfiguration == null) {
				solutionConfiguration = FindSolutionConfiguration("Debug");
			}
			if(solutionConfiguration != null) {
				solutionConfiguration.Activate();
			}
			try {
				Logger.Instance.GetLogger<VSOutputLogger>().AddMessage(String.Format(@"Build the following configuration: ""{0}"".", dte.Solution.SolutionBuild.ActiveConfiguration.Name));
				dte.ExecuteCommand("Build.BuildSolution", "");
				while(dte.Solution.SolutionBuild.BuildState != vsBuildState.vsBuildStateDone) {
					System.Threading.Thread.Sleep(100);
					Application.DoEvents();
				}
			}
			finally {
				if(previousSolutionConfiguration != null) {
					Logger.Instance.GetLogger<VSOutputLogger>().AddMessage(String.Format(@"Restore the previous active configuration: ""{0}"".", previousSolutionConfiguration.Name));
					previousSolutionConfiguration.Activate();
				}
			}
			Logger.Instance.GetLogger<VSOutputLogger>().AddMessage("Get build result.");
			Boolean isBuildSuccess = dte.Solution.SolutionBuild.LastBuildInfo == 0;
			if(!isBuildSuccess) {
				Logger.Instance.GetLogger<VSOutputLogger>().AddMessage("Solution build failed. Unable to run test.");
			}
			else {
				Logger.Instance.GetLogger<VSOutputLogger>().AddMessage("BuildSuccess.");
			}
			return isBuildSuccess;
		}
		private void CollectEtsFileNamesFromProjectItem(ProjectItem projectItem, IList<String> fileNames) {
			ProjectItems subProjectItems = projectItem.ProjectItems;
			if(subProjectItems == null) {
				if(projectItem.SubProject != null) {
					subProjectItems = projectItem.SubProject.ProjectItems;
				}
			}
			if((subProjectItems != null) && (subProjectItems.Count > 0)) {
				foreach(ProjectItem subProjectItem in subProjectItems) {
					CollectEtsFileNamesFromProjectItem(subProjectItem, fileNames);
				}
			}
			else {
				String fileName = projectItem.get_FileNames(1);
				if(IsEtsFile(fileName) && !fileNames.Contains(fileName)) {
					fileNames.Add(fileName);
				}
			}
		}
		private void CollectEtsFileNamesFromProject(Project project, IList<String> fileNames) {
			foreach(ProjectItem projectItem in project.ProjectItems) {
				CollectEtsFileNamesFromProjectItem(projectItem, fileNames);
			}
		}
		private void CollectEtsFileNamesFromSolution(_Solution solution, IList<String> fileNames) {
			foreach(Project project in solution.Projects) {
				CollectEtsFileNamesFromProject(project, fileNames);
			}
		}
		private void CollectEtsFileNamesFromSelectedItems(IList<String> fileNames) {
			TraceMethodEnter(this, "CollectEtsFileNamesFromSelectedItems");
			Object[] items = (Object[])(IEnumerable)((DTE2)dte).ToolWindows.SolutionExplorer.SelectedItems;
			TraceValue("items.Length", items.Length);
			foreach(UIHierarchyItem item in items) {
				if(item.Object is _Solution) {
					CollectEtsFileNamesFromSolution((_Solution)item.Object, fileNames);
				}
				else if(item.Object is Project) {
					CollectEtsFileNamesFromProject((Project)item.Object, fileNames);
				}
				else if(item.Object is ProjectItem) {
					CollectEtsFileNamesFromProjectItem((ProjectItem)item.Object, fileNames);
				}
			}
			TraceValue("fileNames.Count", fileNames.Count);
			TraceMethodExit(this, "CollectEtsFileNamesFromSelectedItems");
		}
		private Boolean IsEtsFile(String fileName) {
			TraceMethodEnter(this, "IsEtsFile");
			TraceValue("fileName", fileName);
			bool result = !String.IsNullOrEmpty(fileName) && (Path.GetExtension(fileName).ToLower() == ".ets");
			TraceValue("result", result);
			TraceMethodExit(this, "IsEtsFile");
			return result;
		}
		private Boolean IsEtsFileEditorActive() {
			TraceMethodEnter(this, "IsEtsFileEditorActive");
			bool result = false;
			TraceValue("dte.ActiveWindow", dte.ActiveWindow);
			if(dte.ActiveWindow != null) {
				TraceValue("dte.ActiveWindow", dte.ActiveWindow.Type);
				if(dte.ActiveWindow.Type == vsWindowType.vsWindowTypeDocument) {
					TraceValue("dte.ActiveWindow.Document", dte.ActiveWindow.Document);
					if(dte.ActiveWindow.Document != null) {
						result = IsEtsFile(dte.ActiveWindow.Document.FullName);
					}
				}
			}
			TraceValue("result", result);
			TraceMethodExit(this, "IsEtsFileEditorActive");
			return result;
		}
		private Boolean IsEtsFilesSelectedInSolutionExplorer() {
			TraceMethodEnter(this, "IsEtsFilesSelectedInSolutionExplorer");
			bool result = false;
			TraceValue("dte.ActiveWindow", dte.ActiveWindow);
			if(dte.ActiveWindow != null) {
				TraceValue("dte.ActiveWindow.Type", dte.ActiveWindow.Type);
				if(dte.ActiveWindow.Type == vsWindowType.vsWindowTypeSolutionExplorer) {
					IList<String> fileNames = new List<String>();
					CollectEtsFileNamesFromSelectedItems(fileNames);
					return (fileNames.Count > 0);
				}
			}
			TraceValue("result", result);
			TraceMethodExit(this, "IsEtsFilesSelectedInSolutionExplorer");
			return result;
		}
		private Boolean IsBreakpointBetweenLines(Int32 startLine, Int32 endLine) {
			foreach(Breakpoint breakpoint in dte.Debugger.Breakpoints) {
				if(breakpoint.Enabled
					&& (breakpoint.File == currentTestFileName)
					&& (breakpoint.FileLine - 1 >= startLine) && (breakpoint.FileLine - 1 <= endLine)) {
					return true;
				}
			}
			return false;
		}
		private void UpdateUI() {
			if(serviceProvider != null) {
				IVsUIShell vsShell = (IVsUIShell)serviceProvider.GetService(typeof(IVsUIShell));
				if(vsShell != null) {
					int hr = vsShell.UpdateCommandUI(0);
					Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
				}
			}
		}
		private void WaitNextCommand(String fileName, Int32 startLine, Int32 endLine) {
			markerManager.DeleteMarker();
			markerManager.DrawMarker(fileName, startLine, endLine);
			isWaiting = true;
			manualResetEvent.Reset();
			UpdateUI();
			manualResetEvent.WaitOne();
			isWaiting = false;
			markerManager.DeleteMarker();
		}
		private void testExecutor_BeforeExecuteCommand(Object sender, ExecuteCommandEventArgs e) {
			EasyTestManager.TraceMethodEnter(this, "testExecutor_BeforeExecuteCommand");
			DevExpress.EasyTest.Framework.Command command = (DevExpress.EasyTest.Framework.Command)sender;
			EasyTestManager.TraceValue("command.Text", command.Text);
			EasyTestManager.TraceValue("prevCommandType", prevCommandType);
			EasyTestManager.TraceValue("previousCommandEndPosition", previousCommandEndPosition);			
			int startPositionLineNumber = (previousCommandEndPosition != null) ? previousCommandEndPosition.LineNumber + 1 : 0;
			if(IsBreakpointBetweenLines(startPositionLineNumber, command.EndPosition.LineNumber)) {
				WaitNextCommand(currentTestFileName, command.StartPosition.LineNumber, command.EndPosition.LineNumber);
			}
			else {
				if(prevCommandType == CommandType.RunNextStep) {
					WaitNextCommand(currentTestFileName, command.StartPosition.LineNumber, command.EndPosition.LineNumber);
				}
				else {
					if(prevCommandType == CommandType.RunToCursor) {
						if((cursorPositionFileName == currentTestFileName) && (cursorPosition <= command.EndPosition.LineNumber + 1)) {
							WaitNextCommand(currentTestFileName, command.StartPosition.LineNumber, command.EndPosition.LineNumber);
						}
					}
				}
			}
			previousCommandEndPosition = command.EndPosition;
			EasyTestManager.TraceMethodExit(this, "testExecutor_BeforeExecuteCommand");
		}
		private void RunTest(String fileName) {
			EasyTestManager.TraceMethodEnter(this, "RunTest");
			EasyTestManager.TraceValue("fileName", fileName);
			currentTestFileName = fileName;
			if(!String.IsNullOrEmpty(fileName)) {
				try {
					String testPath = Path.GetDirectoryName(fileName);
					Options options = GetOptions(Path.GetDirectoryName(fileName));
					if(options != null) {
						SaveDocuments();
						Logger.Instance.AddLogger(new FileLogger(testPath));
						previousCommandEndPosition = null;
						testExecutor = CreateTestExecutor(options);
						testExecutor.IsDebugging = true;
						testExecutor.BeforeExecuteCommand += new EventHandler<ExecuteCommandEventArgs>(testExecutor_BeforeExecuteCommand);
						testExecutor.RunTest(fileName);
					}
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(e);
					Logger.Instance.GetLogger<VSOutputLogger>().AddMessage(e.Message);
				}
			}
			EasyTestManager.TraceMethodExit(this, "RunTest");
		}
		private void RunTests(CommandType commandType, params String[] fileNames) {
			try {
				if(fileNames.Length > 0) {
					Logger.Instance.GetLogger<VSOutputLogger>().ClearOutput();
					if(!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator)) {
						Logger.Instance.GetLogger<VSOutputLogger>().Exception(new Exception("Cannot run test. Restart Visual Studio with administrative privileges and retry."));
					}
					else if(BuildSolution()) {
						prevCommandType = commandType;
						isWaiting = false;
						testExecThread = new System.Threading.Thread(new ThreadStart(delegate() {
							Logger.Instance.RunBatch(fileNames);
							foreach(String fileName in fileNames) {
								if(prevCommandType == CommandType.StopExecuting) {
									break;
								}
								RunTest(fileName);
							}
							Logger.Instance.GetLogger<VSOutputLogger>().WriteCurrentTestsResult();
						}));
						testExecThread.SetApartmentState(ApartmentState.STA);
						testExecThread.Start();
					}
				}
			}
			catch(Exception e) {
				Logger.Instance.GetLogger<VSOutputLogger>().AddMessage("Cannot run test.");
				Logger.Instance.GetLogger<VSOutputLogger>().Exception(e);
				throw e;
			}
		}
		protected virtual TestExecutor CreateTestExecutor(Options options) {
			return new DesignTimeTestExecutor(options);
		}
		public EasyTestManager(_DTE dte, System.IServiceProvider serviceProvider) {
			this.dte = dte;
			this.serviceProvider = serviceProvider;
			Logger.Instance.AddLogger(new VSOutputLogger(dte));
			Logger.Instance.AddLogger(new FileLogger());
			markerManager = new MarkerManager(dte, serviceProvider);
			manualResetEvent = new ManualResetEvent(true);
		}
		public static void TraceMethodEnter(object owner, string methodName) {
			if(EnableTrace) {
				Trace.WriteLine(">" + ((owner is Type) ? owner.ToString() : owner.GetType().ToString()) + "." + methodName + ", " + 
					System.Threading.Thread.CurrentThread.ManagedThreadId);
			}
		}
		public static void TraceValue(string name, object val) {
			if(EnableTrace) {
				Trace.WriteLine("  " + name + " = '" + ((val == null) ? "<null>" : val.ToString()) + "', " + System.Threading.Thread.CurrentThread.ManagedThreadId);
			}
		}
		public static void TraceMethodExit(object owner, string methodName) {
			if(EnableTrace) {
				Trace.WriteLine("<" + ((owner is Type) ? owner.ToString() : owner.GetType().ToString()) + "." + methodName  + ", " + 
					System.Threading.Thread.CurrentThread.ManagedThreadId);
			}
		}
		public void Run() {
			lock(this) {
				if(IsStopped) {
					List<String> fileNames = new List<String>();
					if(IsEtsFileEditorActive()) {
						fileNames.Add(dte.ActiveWindow.Document.FullName);
					}
					else if(IsEtsFilesSelectedInSolutionExplorer()) {
						CollectEtsFileNamesFromSelectedItems(fileNames);
					}
					RunTests(CommandType.Run, fileNames.ToArray());
				}
				else if(IsWaiting) {
					prevCommandType = CommandType.Run;
					manualResetEvent.Set();
				}
			}
		}
		public void RunNextStep() {
			lock(this) {
				TraceMethodEnter(this, "RunNextStep");
				bool isStopped = IsStopped;
				TraceValue("isStopped", isStopped);
				if(IsStopped) {
					List<String> fileNames = new List<String>();
					if(IsEtsFileEditorActive()) {
						fileNames.Add(dte.ActiveWindow.Document.FullName);
					}
					else if(IsEtsFilesSelectedInSolutionExplorer()) {
						CollectEtsFileNamesFromSelectedItems(fileNames);
					}
					RunTests(CommandType.RunNextStep, fileNames.ToArray());
				}
				else if(IsWaiting) {
					prevCommandType = CommandType.RunNextStep;
					TraceValue("manualResetEvent.Set()", "");
					manualResetEvent.Set(); 
				}
				TraceMethodExit(this, "RunNextStep");
			}
		}
		public void RunToCursor() {
			lock(this) {
				if(IsEtsFileEditorActive()) {
					cursorPositionFileName = dte.ActiveWindow.Document.FullName;
					cursorPosition = ((TextSelection)dte.ActiveWindow.Document.Selection).CurrentLine;
					if(IsStopped) {
						RunTests(CommandType.RunToCursor, dte.ActiveWindow.Document.FullName);
					}
					else if(IsWaiting) {
						prevCommandType = CommandType.RunToCursor;
						manualResetEvent.Set();
					}
				}
			}
		}
		public void StopExecuting() {
			lock(this) {
				TraceMethodEnter(this, "StopExecuting");
				prevCommandType = CommandType.StopExecuting;
				markerManager.DeleteMarker();
				TraceValue("testExecutor", testExecutor);
				if(testExecutor != null) {
					testExecutor.AbortTest();
				}
				TraceValue("manualResetEvent.Set()", "");
				manualResetEvent.Set();
				TraceMethodExit(this, "StopExecuting");
			}
		}
		public Boolean IsRunAvailable {
			get {
				TraceMethodEnter(this, "IsRunAvailable");
				bool result =
					IsWaiting
					||
					IsStopped && (IsEtsFileEditorActive() || IsEtsFilesSelectedInSolutionExplorer());
				TraceValue("result", result);
				TraceMethodExit(this, "IsRunAvailable");
				return result;
			}
		}
		public Boolean IsRunNextStepAvailable {
			get {
				TraceMethodEnter(this, "IsRunNextStepAvailable");
				bool result =
					IsWaiting
					||
					IsStopped && (IsEtsFileEditorActive() || IsEtsFilesSelectedInSolutionExplorer());
				TraceValue("result", result);
				TraceMethodExit(this, "IsRunNextStepAvailable");
				return result;
			}
		}
		public Boolean IsRunToCursorAvailable {
			get {
				return (IsStopped || IsWaiting) && IsEtsFileEditorActive();
			}
		}
		public Boolean IsStopRunningAvailable {
			get {
				return (IsRunning || IsWaiting);
			}
		}
	}
	public class VSOutputLogger : ConsoleLogger {
		OutputWindowPane outputWindowCore = null;
		_DTE dte = null;
		public VSOutputLogger(_DTE dte)
			: base() {
			this.dte = dte;
			dte.Events.SolutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(SolutionEvents_AfterClosing);
		}
		public override void AddMessage(string message) {
			OutputWindow.OutputString(message + Environment.NewLine);
		}
		public override void Exception(Exception exception) {
			base.Exception(exception);
			int line = -1;
			if(CurrentTestLog.Errors.Count > 0 && CurrentTestLog.Errors[0].PositionInScript != null) {
				line = CurrentTestLog.Errors[0].PositionInScript.LineNumber + 1;
			}
			string message = exception.Message + Environment.NewLine + exception.StackTrace + Environment.NewLine;
			OutputWindow.OutputTaskItemString(message, vsTaskPriority.vsTaskPriorityHigh,
				"User", vsTaskIcon.vsTaskIconUser,
				CurrentTestLog.TestFileName,
				line, exception.Message, true);
			OutputWindow.OutputString(Environment.NewLine);
		}
		public override void ApplicationStarting(string applicationName) {
			base.ApplicationStarting(applicationName);
			OutputWindow.OutputString("The application " + applicationName + " is starting. This may take several minutes..." + Environment.NewLine);
		}
		public override void ApplicationStarted(string applicationName) {
			base.ApplicationStarted(applicationName);
			OutputWindow.OutputString("The application " + applicationName + " has started." + Environment.NewLine);
		}
		public override void FinishApplication() {
			base.FinishApplication();
			OutputWindow.OutputString("Application: " + CurrentTestLog.ApplicationName + Environment.NewLine);
			OutputWindow.OutputString("Test result: " + CurrentTestLog.Result + Environment.NewLine);
			string logPath = Path.GetDirectoryName(CurrentTestLog.TestFileName);
			string applicationViews = CurrentTestLog.ApplicationViews;
			if(!string.IsNullOrEmpty(applicationViews) && logPath != null) {
				OutputWindow.OutputString("See the screenshot(s):" + Environment.NewLine);
				foreach(string fileName in applicationViews.Split(';')) {
					string viewFileName = Path.Combine(logPath, fileName);
					OutputWindow.OutputTaskItemString("\t" + viewFileName, vsTaskPriority.vsTaskPriorityHigh,
						"User", vsTaskIcon.vsTaskIconUser,
						null,
						-1, CurrentTestLog.Result, true);
					OutputWindow.OutputString(Environment.NewLine);
				}
			}
			OutputWindow.OutputString("Test took " + CurrentTestLog.Elapsed + Environment.NewLine);
		}
		public override void StartLog(string testName, string testFileName) {
			OutputWindow.OutputString(Environment.NewLine);
			base.StartLog(testName, testFileName);
			OutputWindow.Activate();
		}
		public virtual void ClearOutput() {
			OutputWindow.Clear();
		}
		private void SolutionEvents_AfterClosing() {
			ClearOutput();
		}
		private OutputWindowPane GetOutputWindowPane() {
			EnvDTE.Window win = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);
			OutputWindow outputWindow = win.Object as OutputWindow;
			OutputWindowPane pane;
			try {
				pane = outputWindow.OutputWindowPanes.Item("Easy Test");
			}
			catch {
				pane = outputWindow.OutputWindowPanes.Add("Easy Test");
			}
			return pane;
		}
		private OutputWindowPane OutputWindow {
			get {
				if(outputWindowCore == null) {
					outputWindowCore = GetOutputWindowPane();
				}
				return outputWindowCore;
			}
		}
	}
	public class DesignTimeTestExecutor : TestExecutor {
		public DesignTimeTestExecutor(Options options) : base(options) { }
		protected override IApplicationAdapter CreateApplicationAdapterCore(string applicationName) {
			TestApplication testApplication = Options.Applications[applicationName];
			if(File.Exists(testApplication.AdapterFileName)) { 
				string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				Directory.CreateDirectory(tempPath);
				string adapterDirectoryName = Path.GetDirectoryName(testApplication.AdapterFileName);
				foreach(string filePath in Directory.GetFiles(adapterDirectoryName, "*.dll")) {
					File.Copy(filePath, Path.Combine(tempPath, Path.GetFileName(filePath)), true);
				}
				testApplication.AdapterFileName = Path.Combine(tempPath, Path.GetFileName(testApplication.AdapterFileName));
			}
			return base.CreateApplicationAdapterCore(applicationName);
		}
	}
}
