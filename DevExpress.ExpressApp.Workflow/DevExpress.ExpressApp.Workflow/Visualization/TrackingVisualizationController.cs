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
using DevExpress.ExpressApp.Editors;
using DevExpress.Workflow;
namespace DevExpress.ExpressApp.Workflow.Visualization {
	public class TrackingVisualizationController : RunningWorkflowInstanceInfoControllerBase<DetailView> {
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateWorkflowTrackingRecordVisualization("");
		}
		protected virtual void UpdateWorkflowTrackingRecordVisualization(string selectedActivityId) {
			IRunningWorkflowInstanceInfo info = View.CurrentObject as IRunningWorkflowInstanceInfo;
			if(info != null) {
				info.SetTrackingRecordVisualizationInfo(new VisualizationInfo(GetWorkflowXaml(info), selectedActivityId));
				ViewItem vizualizer = View.FindItem("TrackingRecordVisualizationInfo");
				vizualizer.Refresh();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			UpdateWorkflowTrackingRecordVisualization("");
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			ListPropertyEditor trackingRecords = View.FindItem("TrackingRecords") as ListPropertyEditor;
			if(trackingRecords != null) {
				trackingRecords.ControlCreated += new EventHandler<EventArgs>(trackingRecords_ControlCreated);
			}
		}
		private void trackingRecords_ControlCreated(object sender, EventArgs e) {
			ListPropertyEditor trackingRecords = sender as ListPropertyEditor;
			trackingRecords.ControlCreated -= new EventHandler<EventArgs>(trackingRecords_ControlCreated);
			trackingRecords.ListView.Editor.SelectionChanged += new EventHandler(Editor_SelectionChanged);
		}
		void Editor_SelectionChanged(object sender, EventArgs e) {
			ListEditor trackingRecords = sender as ListEditor;
			ITrackingRecord trackingRecord = trackingRecords.FocusedObject as ITrackingRecord;
			UpdateWorkflowTrackingRecordVisualization(trackingRecord != null ? trackingRecord.ActivityId : "");
		}
		protected override void OnDeactivated() {
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			ListPropertyEditor trackingRecords = View.FindItem("TrackingRecords") as ListPropertyEditor;
			if(trackingRecords != null && trackingRecords.ListView.Editor != null) {
				trackingRecords.ListView.Editor.SelectionChanged -= new EventHandler(Editor_SelectionChanged);
			}
			base.OnDeactivated();
		}
	}
}
