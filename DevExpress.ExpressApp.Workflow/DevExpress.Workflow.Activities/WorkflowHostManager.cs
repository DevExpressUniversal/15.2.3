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
using System.Activities;
using System.ComponentModel;
using System.ServiceModel;
using DevExpress.ExpressApp.MiddleTier;
namespace DevExpress.Workflow {
	public class HostFaultedException : Exception {
		public HostFaultedException(WorkflowHost host) {
			this.Host = host;
			Data.Add("WorkflowHost.ActivityUniqueId", host.ActivityUniqueId);
		}
		public WorkflowHost Host { get; set; }
	}
	public enum HostManagerState { Closed, Opening, Opened, Closing }
	public class WorkflowHostEventArgs : EventArgs {
		public WorkflowHostEventArgs(WorkflowHost host) {
			WorkflowHost = host;
		}
		public WorkflowHost WorkflowHost { get; private set; }
	}
	public static class TracerHelper {
		public static void TraceMethodOperation(object obj, string methodName, string op) {
			if(DevExpress.Persistent.Base.Tracing.Tracer.VerbositySwitch.TraceVerbose) {
				string typeName = (obj == null) ? "<null>" : obj.GetType().FullName;
				string operationName = string.IsNullOrEmpty(op) ? "IsNullOrEmpty" : op;
				DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText(operationName + typeName + "." + methodName);
			}
		}
		public static void TraceMethodEnter(object obj, string methodName) {
			TraceMethodOperation(obj, methodName, ">");
		}
		public static void TraceMethodExit(object obj, string methodName) {
			TraceMethodOperation(obj, methodName, "<");
		}
		public static void TraceVariableValue(string name, object value) {
			if(DevExpress.Persistent.Base.Tracing.Tracer.VerbositySwitch.TraceVerbose) {
				DevExpress.Persistent.Base.Tracing.Tracer.LogVerboseText(name + " = " + (value == null ? "<null>" : ("'" + value.ToString() + "'")));
			}
		}
	}
	public class WorkflowHostManager : IDisposable {
		public const string ExceptionDataKey = "DxWorkflowHostManager.ExceptionDetails";
		private delegate void Predicate();
		private Dictionary<string, WorkflowHost> hosts = new Dictionary<string, WorkflowHost>();
		private void WrapToCustomHandleException(Predicate func, string details) {
			try {
				func();
			}
			catch(Exception e) {
				e.Data.Add(ExceptionDataKey, details);
				CustomHandleExceptionEventArgs args = new CustomHandleExceptionEventArgs(e);
				if(CustomHandleException != null) {
					CustomHandleException(this, args);
				}
				if(!args.Handled) {
					throw;
				}
			}
		}
		private void Host_Faulted(object sender, EventArgs e) {
			CustomHandleExceptionEventArgs args = new CustomHandleExceptionEventArgs(new HostFaultedException((WorkflowHost)sender));
			if(CustomHandleException != null) {
				CustomHandleException(this, args);
			}
		}
		protected void OnHostCreated(WorkflowHost host) {
			if(HostCreated != null) {
				HostCreated(this, new WorkflowHostEventArgs(host));
			}
		}
		protected void OnHostsOpening() {
			if(HostsOpening != null) {
				HostsOpening(this, EventArgs.Empty);
			}
		}
		protected void OnHostsOpened() {
			if(HostsOpened != null) {
				HostsOpened(this, EventArgs.Empty);
			}
		}
		protected void OnHostsClosing() {
			if(HostsClosing != null) {
				HostsClosing(this, EventArgs.Empty);
			}
		}
		protected HostManagerState State { get; private set; }
		public WorkflowHostManager(string baseUri) {
			this.BaseUri = baseUri;
			State = HostManagerState.Closed;
			CloseHostTimeout = TimeSpan.Zero;
		}
		public virtual void RefreshHosts(IDictionary<string, Activity> activities) {
			TracerHelper.TraceMethodEnter(this, "RefreshHosts");
			if(State != HostManagerState.Closed) {
				throw new InvalidOperationException();
			}
			Hosts.Clear();
			TracerHelper.TraceVariableValue("activities.Count", activities.Count);
			foreach(KeyValuePair<string, Activity> activity in activities) {
				TracerHelper.TraceVariableValue("activity.Key", activity.Key);
				WrapToCustomHandleException(delegate() {
					WorkflowHost host = new WorkflowHost(activity.Value, activity.Key, BaseUri.TrimEnd('/') + "/" + activity.Key);
					OnHostCreated(host);
					Hosts[activity.Key] = host;
					host.Faulted += new EventHandler(Host_Faulted);
				}, activity.Key);
			}
			TracerHelper.TraceMethodExit(this, "RefreshHosts");
		}
		public virtual void OpenHosts() {
			TracerHelper.TraceMethodEnter(this, "OpenHosts");
			OnHostsOpening();
			State = HostManagerState.Opening;
			foreach(string hostKey in Hosts.Keys) {
				WrapToCustomHandleException(delegate() { OpenHost(hostKey); }, hostKey);
			}
			State = HostManagerState.Opened;
			OnHostsOpened();
			TracerHelper.TraceMethodExit(this, "OpenHosts");
		}
		public virtual void CloseHosts() {
			TracerHelper.TraceMethodEnter(this, "CloseHosts");
			OnHostsClosing();
			State = HostManagerState.Closing;
			TracerHelper.TraceVariableValue("Hosts.Count", Hosts.Count);
			foreach(string hostKey in Hosts.Keys) {
				WrapToCustomHandleException(delegate() { CloseHost(hostKey); }, hostKey);
			}
			State = HostManagerState.Closed;
			TracerHelper.TraceMethodExit(this, "CloseHosts");
		}
		public virtual void OpenHost(string workflowUniqueId) {
			Hosts[workflowUniqueId].Host.Open();
		}
		public virtual void CloseHost(string workflowUniqueId) {
			TracerHelper.TraceMethodEnter(this, "CloseHost");
			TracerHelper.TraceVariableValue("workflowUniqueId", workflowUniqueId);
			TracerHelper.TraceVariableValue("Hosts[workflowUniqueId].State", Hosts[workflowUniqueId].State);
			if(Hosts[workflowUniqueId].State == CommunicationState.Faulted) {
				Hosts[workflowUniqueId].Host.Abort();
			}
			else {
				TracerHelper.TraceVariableValue("CloseHostTimeout", CloseHostTimeout);
				if(CloseHostTimeout != TimeSpan.Zero) {
					Hosts[workflowUniqueId].Host.Close(CloseHostTimeout);
				}
				else {
					Hosts[workflowUniqueId].Host.Close();
				}
			}
			TracerHelper.TraceMethodExit(this, "CloseHost");
		}
		public void Dispose() {
			CloseHosts();
		}
		public static Guid StartWorkflow(string baseUri, string workflowUniqueId, IDictionary<string, object> inputs) {
			return CreationEndpoint.GetCreationClient(baseUri + "/" + workflowUniqueId).Create(inputs);
		}
		public string BaseUri { get; private set; }
		public TimeSpan CloseHostTimeout { get; set; }
		public IDictionary<string, WorkflowHost> Hosts { get { return hosts; } }
		public event EventHandler<WorkflowHostEventArgs> HostCreated;
		public event EventHandler HostsOpening;
		public event EventHandler HostsOpened;
		public event EventHandler HostsClosing;
		public event EventHandler<CustomHandleExceptionEventArgs> CustomHandleException;
	}
}
