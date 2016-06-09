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
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Workflow.Win {
	public interface IWorkflowVizualizationHelper : IDisposable {
		void Initialize(System.Activities.Presentation.WorkflowDesigner designer);
		void UpdateSourceLocationMapping();
		void UpdateCurrentLocation(string selectedActionId);
	}
	public interface IWorkflowDebuggerHelper : IWorkflowVizualizationHelper {
		void ToggleBreakpoint();
		void UpdateBreakpoints();
		bool IsBreakpoint(string activityId);
	}
	public class WorkflowDebuggerHelper : IWorkflowDebuggerHelper {
		private const String FileName = "UNRESOLVED:";
		private WorkflowDesigner designer;
		private Dictionary<object, SourceLocation> designerSourceLocationMapping = new Dictionary<object, SourceLocation>();
		private Dictionary<object, SourceLocation> wfElementToSourceLocationMap = new Dictionary<object, SourceLocation>();
		internal List<string> breakpointList = new List<string>();
		private object GetRootInstance() {
			ModelService modelService = designer.Context.Services.GetService<ModelService>();
			if(modelService != null) {
				return modelService.Root.GetCurrentValue();
			}
			else {
				return null;
			}
		}
		private Activity GetRootWorkflowElement(object rootModelObject) {
			Guard.ArgumentNotNull(rootModelObject, "rootModelObject");
			Activity rootWorkflowElement;
			IDebuggableWorkflowTree debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
			if(debuggableWorkflowTree != null) {
				rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
			}
			else {
				rootWorkflowElement = rootModelObject as Activity;
			}
			return rootWorkflowElement;
		}
		private Activity GetRootRuntimeWorkflowElement() {
			designer.Flush();
			Activity root = ActivityXamlServices.Load(new StringReader(designer.Text));
			WorkflowInspectionServices.CacheMetadata(root);
			IEnumerator<Activity> activityEnumerator = WorkflowInspectionServices.GetActivities(root).GetEnumerator();
			activityEnumerator.MoveNext();
			while(activityEnumerator.Current != null && IsVariable(activityEnumerator.Current)) {
				activityEnumerator.MoveNext();
			}
			root = activityEnumerator.Current;
			return root;
		}
		internal bool IsVariable(Activity activity) {
			Type activityType = activity.GetType();
			if(typeof(Argument).IsAssignableFrom(activityType) || typeof(Variable).IsAssignableFrom(activityType) || activityType.FullName.Contains("VisualBasic")) {
				return true;
			}
			return false;
		}
		internal Dictionary<string, Activity> BuildActivityIdToWfElementMap() {
			EnsureSourceLocationMapping();
			Dictionary<string, Activity> map = new Dictionary<string, Activity>();
			Activity wfElement;
			foreach(object instance in wfElementToSourceLocationMap.Keys) {
				wfElement = instance as Activity;
				if(wfElement != null) {
					map.Add(wfElement.Id, wfElement);
				}
			}
			return map;
		}
		private void EnsureSourceLocationMapping() {
			if(wfElementToSourceLocationMap.Count == 0) {
				UpdateSourceLocationMapping();
			}
		}
		private SourceLocation FindLocationInMapByActivityId(string activityId, Dictionary<string, Activity> actualActivityIdToWfElementMap) {
			EnsureSourceLocationMapping();
			if(actualActivityIdToWfElementMap.ContainsKey(activityId)) {
				Activity activity = actualActivityIdToWfElementMap[activityId];
				return wfElementToSourceLocationMap[activity];
			}
			return null;
		}
		internal SourceLocation FindLocationByActivityId(string selectionId) {
			Dictionary<string, Activity> activityIdToWfElementMap = BuildActivityIdToWfElementMap();
			if(!string.IsNullOrEmpty(selectionId)) {
				SourceLocation result = FindLocationInMapByActivityId(selectionId, activityIdToWfElementMap);
				if(result == null) {
					string foundActivityId = "";
					foreach(string activityId in activityIdToWfElementMap.Keys) {
						if(selectionId.StartsWith(activityId) && foundActivityId.Length <= activityId.Length) {
							foundActivityId = activityId;
						}
					}
					result = FindLocationInMapByActivityId(foundActivityId, activityIdToWfElementMap);
				}
				return result;
			}
			return null;
		}
		internal SourceLocation FindLocationByActivity(Activity selectedActivity) {
			EnsureSourceLocationMapping();
			if(designerSourceLocationMapping.ContainsKey(selectedActivity)) {
				return designerSourceLocationMapping[selectedActivity];
			}
			return null;
		}
		public WorkflowDebuggerHelper(WorkflowDesigner designer) {
			Initialize(designer);
		}
		public void Initialize(WorkflowDesigner designer) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
			designerSourceLocationMapping.Clear();
			wfElementToSourceLocationMap.Clear();
		}
		public void UpdateSourceLocationMapping() {
			object rootInstance = GetRootInstance();
			wfElementToSourceLocationMap.Clear();
			designerSourceLocationMapping.Clear();
			if(rootInstance != null) {
				Activity documentRootElement = GetRootWorkflowElement(rootInstance);
				if(documentRootElement != null) {
					SourceLocationProvider.CollectMapping(GetRootRuntimeWorkflowElement(), documentRootElement, wfElementToSourceLocationMap, FileName);
					SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping, FileName);
				}
			}
			if(designer.DebugManagerView is DebuggerService) {
				((DebuggerService)(designer.DebugManagerView)).UpdateSourceLocations(designerSourceLocationMapping);
			}
		}
		public void UpdateCurrentLocation(string selectedActionId) {
			designer.DebugManagerView.CurrentLocation = FindLocationByActivityId(selectedActionId);
		}
		public void ToggleBreakpoint() {
			ModelItem selectedModelItem = designer.Context.Items.GetValue<Selection>().PrimarySelection;
			Activity selectedActivity = selectedModelItem.GetCurrentValue() as Activity;
			if(selectedActivity != null) {
				SourceLocation srcLoc = FindLocationByActivity(selectedActivity);
				if(srcLoc != null) {
					if(!breakpointList.Contains(selectedActivity.Id)) {
						designer.Context.Services.GetService<IDesignerDebugView>().InsertBreakpoint(srcLoc, BreakpointTypes.Bounded | BreakpointTypes.Enabled);
						breakpointList.Add(selectedActivity.Id);
					}
					else {
						designer.Context.Services.GetService<IDesignerDebugView>().DeleteBreakpoint(srcLoc);
						breakpointList.Remove(selectedActivity.Id);
					}
				}
			}
		}
		public void UpdateBreakpoints() {
			List<string> breakpointsToRemove = new List<string>();
			foreach(string activityId in breakpointList) {
				SourceLocation breakpointLocation = FindLocationByActivityId(activityId);
				if(breakpointLocation != null) {
					designer.Context.Services.GetService<IDesignerDebugView>().UpdateBreakpoint(breakpointLocation, BreakpointTypes.Bounded | BreakpointTypes.Enabled);
				}
				else {
					breakpointsToRemove.Add(activityId);
				}
			}
			foreach(string activityId in breakpointsToRemove) {
				breakpointList.Remove(activityId);
			}
		}
		public bool IsBreakpoint(string activityId) {
			return breakpointList.Contains(activityId);
		}
		public void Dispose() {
			designerSourceLocationMapping.Clear();
			wfElementToSourceLocationMap.Clear();
			breakpointList.Clear();
			designer = null;
		}
#if DebugTest
		public SourceLocation DebugTest_FindLocationByActivityId(string selectionId) {
			return FindLocationByActivityId(selectionId);
		}
		public SourceLocation DebugTest_FindLocationByActivity(Activity selectedActivity) {
			return FindLocationByActivity(selectedActivity);
		}
		public Dictionary<string, Activity> DebugTest_BuildActivityIdToWfElementMap() {
			return BuildActivityIdToWfElementMap();
		}
		public List<string> DebugTest_BreakpointList {
			get { return breakpointList; }
			set { breakpointList = value; }
		}
#endif
	}
}
