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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.Workflow.Utils;
using System.Activities.Tracking;
using DevExpress.Utils;
using System.Activities;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using System.Activities.XamlIntegration;
using System.IO;
namespace DevExpress.ExpressApp.Workflow.Win {
	public class CustomProvideWorkflowDesignerControlContainerEventArgs : EventArgs {
		public IWorkflowDesignerControlContainer Container { get; set; }
	}
	public class DebugWorkflowController : ViewController<DetailView> {
		public const string HasDesignerControlEnabledKey = "HasDesignerControl";
		public const string IsDebuggingEnabledKey = "IsDebugging";
		private SimpleAction debugWorkflow;
		private SimpleAction toggleBreakpoint;
		private SimpleAction stopDebug;
		private WorkflowDesignerControl designerControl;
		internal WorkflowDesignerControl DesignerControl { 
			get { return designerControl; } 
			set {
				if(designerControl == value) {
					return;
				}
				if(designerControl != null) {
					if(value == null) {
						designerControl.Control.KeyDown += new System.Windows.Input.KeyEventHandler(control_KeyDown);
					}
					else {
						throw new NotImplementedException();
					}
				}
				designerControl = value;
				if(designerControl != null) {
					designerControl.Control.KeyDown += new System.Windows.Input.KeyEventHandler(control_KeyDown);
				}
				UpdateActionsEnabled();
			} 
		}
		private void container_ControlCreated(object sender, EventArgs e) {
			DesignerControl = ((IWorkflowDesignerControlContainer)sender).WorkflowDesignerControl;
		}
		private void debugWorkflow_Execute(object sender, SimpleActionExecuteEventArgs e) {
			stopDebug.Enabled[IsDebuggingEnabledKey] = true;
			DebugWorkflow();
		}
		private void toggleBreakpoint_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ToggleBreakpoint();
		}
		private void stopDebug_Execute(object sender, SimpleActionExecuteEventArgs e) {
			DebugStop();
		}
		private void UpdateActionsEnabled() {
			bool hasControl = (designerControl != null);
			debugWorkflow.Enabled[HasDesignerControlEnabledKey] = hasControl;
			toggleBreakpoint.Enabled[HasDesignerControlEnabledKey] = hasControl;
			stopDebug.Enabled[HasDesignerControlEnabledKey] = hasControl;
			stopDebug.Enabled[IsDebuggingEnabledKey] = false;
		}
		private IWorkflowDesignerControlContainer FindWorkflowDesignerControlContainer() {
			if(CustomProvideWorkflowDesignerControlContainer != null) {
				CustomProvideWorkflowDesignerControlContainerEventArgs args = new CustomProvideWorkflowDesignerControlContainerEventArgs();
				CustomProvideWorkflowDesignerControlContainer(this, args);
				return args.Container;
			}
			else {
				return View.FindItem("Xaml") as WorkflowPropertyEditor;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			UpdateActionsEnabled();
			IWorkflowDesignerControlContainer container = FindWorkflowDesignerControlContainer();
			if(container != null) {
				if(container.WorkflowDesignerControl != null) {
					DesignerControl = container.WorkflowDesignerControl;
				}
				else {
					container.ControlCreated += container_ControlCreated;
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			DesignerControl = null;
		}
		public DebugWorkflowController() {
			TargetObjectType = typeof(IWorkflowDefinition);
			debugWorkflow = new SimpleAction(this, "DebugWorkflow", WorkflowModule.WorkflowActionCategory);
			debugWorkflow.Caption = "Debug";
			debugWorkflow.ToolTip = "Start/continue debugging of the current workflow.";
			debugWorkflow.ImageName = "Action_Debug_Start";
			debugWorkflow.Execute += new SimpleActionExecuteEventHandler(debugWorkflow_Execute);
			toggleBreakpoint = new SimpleAction(this, "ToggleBreakpoint", WorkflowModule.WorkflowActionCategory);
			toggleBreakpoint.Caption = "Toggle Breakpoint";
			toggleBreakpoint.ToolTip = "Toggle breakpoint at the selected activity.";
			toggleBreakpoint.ImageName = "Action_Debug_Breakpoint_Toggle";
			toggleBreakpoint.Execute += new SimpleActionExecuteEventHandler(toggleBreakpoint_Execute);
			stopDebug = new SimpleAction(this, "StopDebug", WorkflowModule.WorkflowActionCategory);
			stopDebug.Caption = "Stop Debug";
			stopDebug.ToolTip = "Stop debugging of the current workflow.";
			stopDebug.ImageName = "Action_Debug_Stop";
			stopDebug.Execute += new SimpleActionExecuteEventHandler(stopDebug_Execute);
		}
		#region Debugger helper methods
		private const String all = "*";
		private AutoResetEvent resumeRuntimeFromHost = null;
		private bool stepByStepMode = false;
		private bool forceStopDebugging = false;
		private TrackingRecordsForm trackingRecordsForm = null;
		protected TrackingParticipantBase CreateTrackingParticipant() {
			TrackingParticipantBase debugTracker = new DebuggerTrackingParticipant() {
				TrackingProfile = new TrackingProfile() {
					Name = "CustomTrackingProfile",
					Queries = {
							new CustomTrackingQuery {
								Name = all,
								ActivityName = all
							},
							new WorkflowInstanceQuery {
								States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
							},
							new ActivityStateQuery {
								ActivityName = all,
								States = { all },
								Variables = { { all } }
							},
							new FaultPropagationQuery {
								FaultHandlerActivityName = all,
								FaultSourceActivityName = all
							}
						}
				}
			};
			TrackingParticipantEventArgs args = new TrackingParticipantEventArgs(debugTracker);
			if(TrackingParticipantCreated != null) {
				TrackingParticipantCreated(this, args);
			}
			Guard.ArgumentNotNull(args.TrackingParticipant, "TrackingParticipant");
			return args.TrackingParticipant;
		}
		protected virtual void OnStopDebugging() {
			HighlightLocation(null);
			if(resumeRuntimeFromHost != null) {
				resumeRuntimeFromHost.Set();
				resumeRuntimeFromHost = null;
			}
			stepByStepMode = false;
			stopDebug.Enabled[IsDebuggingEnabledKey] = false;
			if(DebuggingStopped != null) {
				DebuggingStopped(this, EventArgs.Empty);
			}
		}
		private void debugTracker_TrackReceived(object sender, TrackingEventArgs trackingEventArgs) {
			if(forceStopDebugging) {
				forceStopDebugging = false;
				throw new Exception(TrackingParticipantBase.UserCancelledExceptionMessage);
			}
			ActivityStateRecord activityStateRecord = trackingEventArgs.Record as ActivityStateRecord;
			if((activityStateRecord != null) && (!activityStateRecord.Activity.TypeName.Contains("System.Activities.Expressions"))) {
				HighlightLocation(activityStateRecord.Activity.Id);
				bool isBreakpointHit = DesignerControl.DebuggerHelper.IsBreakpoint(activityStateRecord.Activity.Id);
				if(resumeRuntimeFromHost != null) { 
					if(isBreakpointHit == true || stepByStepMode) {
						resumeRuntimeFromHost.WaitOne();
					}
				}
				DesignerControl.Control.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() => {
					trackingRecordsForm.AppendText(TrackingParticipantHelper.GetInfo(activityStateRecord));
					System.Threading.Thread.Sleep(1000);
				}));
			}
			FaultPropagationRecord faultPropagationRecord = trackingEventArgs.Record as FaultPropagationRecord;
			if(faultPropagationRecord  != null) {
				DesignerControl.Control.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() => {
					trackingRecordsForm.AppendText(TrackingParticipantHelper.GetInfo(faultPropagationRecord));
					System.Threading.Thread.Sleep(1000);
				}));
			}
		}
		private void control_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
			if(e.Key == Key.F9) {
				ToggleBreakpoint();
			}
			if(e.Key == Key.System && e.SystemKey == Key.F10) {
				DebugStep();
			}
		}
		private Activity GetRuntimeExecutionRoot() {
			Activity root = ActivityXamlServices.Load(new StringReader(DesignerControl.Xaml));
			WorkflowInspectionServices.CacheMetadata(root);
			return root;
		}
		private void HighlightLocation(string actionId) {
			DesignerControl.Control.Dispatcher.Invoke(DispatcherPriority.Render, (Action)(() => {
				DesignerControl.DebuggerHelper.UpdateCurrentLocation(actionId);
			}));
		}
		#endregion
		public void ToggleBreakpoint() {
			Guard.ArgumentNotNull(DesignerControl, "DesignerControl");
			Guard.ArgumentNotNull(DesignerControl.DebuggerHelper, "DesignerControl.DebuggerHelper");
			DesignerControl.DebuggerHelper.ToggleBreakpoint();
		}
		public void DebugStep() {
			if(resumeRuntimeFromHost != null) {
				resumeRuntimeFromHost.Set();
				stepByStepMode = true;
			}
		}
		public void DebugWorkflow() {
			if(resumeRuntimeFromHost == null) {
				Guard.ArgumentNotNull(DesignerControl, "DesignerControl");
				DesignerControl.HardUpdateLocationMappings();
				resumeRuntimeFromHost = new AutoResetEvent(false);
				WorkflowInvoker debuggingInstance = new WorkflowInvoker(GetRuntimeExecutionRoot());
				if(trackingRecordsForm == null || trackingRecordsForm.IsDisposed) {
					trackingRecordsForm = new TrackingRecordsForm();
				}
				trackingRecordsForm.Clear();
				TrackingParticipantBase debugTracker = CreateTrackingParticipant();
				debugTracker.TrackReceived += new EventHandler<TrackingEventArgs>(debugTracker_TrackReceived);
				debuggingInstance.Extensions.Add(debugTracker);
				if(DesignerControl.ObjectSpaceProvider != null) {
					debuggingInstance.Extensions.Add(DesignerControl.ObjectSpaceProvider);
				}
				trackingRecordsForm.Show();
				ThreadPool.QueueUserWorkItem(new WaitCallback((context) => {
					try {
						debuggingInstance.Invoke(new Dictionary<string, object>(), new TimeSpan(1, 0, 0));
					}
					catch(Exception e) {
						DesignerControl.Control.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() => {
							trackingRecordsForm.AppendText(e.Message + "\r\n");
							trackingRecordsForm.AppendText(e.StackTrace + "\r\n");
						}));
						debuggingInstance.CancelAsync("Faulted due to the inner exception.");
					}
					finally {
						OnStopDebugging();
					}
				}));
			}
			else {
				resumeRuntimeFromHost.Set();
			}
		}
		public void DebugStop() {
			if(resumeRuntimeFromHost != null) {
				forceStopDebugging = true;
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(trackingRecordsForm != null && !trackingRecordsForm.IsDisposed) {
					trackingRecordsForm.Dispose();
					trackingRecordsForm = null;
				}
			}
		}
		public SimpleAction DebugWorkflowAction { get { return debugWorkflow; } }
		public SimpleAction ToggleBreakpointAction { get { return toggleBreakpoint; } }
		public SimpleAction StopDebugAction { get { return stopDebug; } }
		public event EventHandler<CustomProvideWorkflowDesignerControlContainerEventArgs> CustomProvideWorkflowDesignerControlContainer;
		public event EventHandler<TrackingParticipantEventArgs> TrackingParticipantCreated;
#if DebugTest
		public TrackingRecordsForm DebugTest_TrackingRecordsForm {
			get { return trackingRecordsForm; }
			set { trackingRecordsForm = value; }
		}
		public WorkflowDesignerControl DebugTest_DesignerControl {
			get { return DesignerControl; }
			set { DesignerControl = value; }
		}
		public event EventHandler DebuggingStopped;		
#else
		internal event EventHandler DebuggingStopped;
#endif
	}
	public class TrackingParticipantEventArgs : EventArgs {
		public TrackingParticipantEventArgs(TrackingParticipantBase trackingParticipant) {
			TrackingParticipant = trackingParticipant;
		}
		public TrackingParticipantBase TrackingParticipant { get; set; }
	}
#if DebugTest
	public class TrackingRecordsForm : XtraForm {
		public string DebugTest_GetText() {
			return GetText();
		}
#else
	internal class TrackingRecordsForm : XtraForm {
#endif
		private TextBox textBox;
		public TrackingRecordsForm() {
			Text = "Tracking records";
			TopMost = true;
			ShowInTaskbar = false;
			textBox = new TextBox();
			textBox.ReadOnly = true;
			textBox.Multiline = true;
			textBox.ScrollBars = ScrollBars.Both;
			textBox.Dock = DockStyle.Fill;
			Controls.Add(textBox);
		}
		public void AppendText(string text) {
			textBox.AppendText(text);
		}
		internal string GetText() {
			return textBox.Text;
		}
		public void Clear() {
			textBox.Clear();
		}
	}
}
