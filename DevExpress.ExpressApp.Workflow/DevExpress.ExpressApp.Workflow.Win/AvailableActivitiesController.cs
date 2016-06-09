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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Workflow.Win {
	public class WorkflowVisualizationController : ViewController<DetailView> {
		List<IWorkflowDefinition> objs = new List<IWorkflowDefinition>();
		private bool isCompiledActivitiesRefreshed = false;
		private void viewItem_ControlCreating(object sender, EventArgs e) {
			if (AvailableActivitiesProvider != null && !isCompiledActivitiesRefreshed) {
				AvailableActivitiesProvider.RefreshCompiledActivities();
				isCompiledActivitiesRefreshed = true;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(AvailableActivitiesProvider == null) {
				AvailableActivitiesProvider = Application.Modules.FindModule<Workflow.Win.WorkflowWindowsFormsModule>().AvailableActivitiesProvider;
			}
			foreach (WorkflowVisualizationPropertyEditor viewItem in View.GetItems<WorkflowVisualizationPropertyEditor>()) {
				viewItem.ControlCreating += new EventHandler<EventArgs>(viewItem_ControlCreating);
			}
			foreach (WorkflowPropertyEditor workflowPropertyEditor in View.GetItems<WorkflowPropertyEditor>()) {
				workflowPropertyEditor.ControlCreating += new EventHandler<EventArgs>(viewItem_ControlCreating);
			}
		}
		protected override void OnDeactivated() {
			foreach (WorkflowVisualizationPropertyEditor viewItem in View.GetItems<WorkflowVisualizationPropertyEditor>()) {
				viewItem.ControlCreating -= new EventHandler<EventArgs>(viewItem_ControlCreating);
			}
			foreach (WorkflowPropertyEditor workflowPropertyEditor in View.GetItems<WorkflowPropertyEditor>()) {
				workflowPropertyEditor.ControlCreating -= new EventHandler<EventArgs>(viewItem_ControlCreating);
			}
			isCompiledActivitiesRefreshed = false;
			base.OnDeactivated();
		}
		public AvailableActivitiesProvider AvailableActivitiesProvider { get; set; }
	}
	public class AvailableActivitiesController : ViewController<DetailView> {
		private void viewItem_QueryAvailableActivities(object sender, ActivitiesInformationEventArgs e) {
			if(AvailableActivitiesProvider != null) {
				e.ActivitiesInformation.AddRange(AvailableActivitiesProvider.GetAvailableActivities());
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(AvailableActivitiesProvider == null) {
				AvailableActivitiesProvider = Application.Modules.FindModule<Workflow.Win.WorkflowWindowsFormsModule>().AvailableActivitiesProvider;
			}
			foreach(WorkflowPropertyEditor viewItem in View.GetItems<WorkflowPropertyEditor>()) {
				viewItem.QueryAvailableActivities += new EventHandler<ActivitiesInformationEventArgs>(viewItem_QueryAvailableActivities);
			}
		} 
		protected override void OnDeactivated() {
			foreach(WorkflowPropertyEditor viewItem in View.GetItems<WorkflowPropertyEditor>()) {
				viewItem.QueryAvailableActivities -= new EventHandler<ActivitiesInformationEventArgs>(viewItem_QueryAvailableActivities);
			}
			base.OnDeactivated();
		}
		public AvailableActivitiesProvider AvailableActivitiesProvider { get; set; }
	}
}
