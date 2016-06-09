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
using DevExpress.ExpressApp.Win.Editors;
using System.Activities.Presentation;
using System.Activities.XamlIntegration;
using System.Xaml;
using System.IO;
using System.Activities;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Workflow.Win {
	public interface IWorkflowDesignerControlContainer {
		WorkflowDesignerControl WorkflowDesignerControl { get; }
		event EventHandler<EventArgs> ControlCreated;
	}
	[PropertyEditor(typeof(String), false)]
	public class WorkflowPropertyEditor : WinPropertyEditor, IComplexViewItem, IWorkflowDesignerControlContainer {
		private XafApplication application = null;
		private void result_XamlChanged(object sender, EventArgs e) {
			OnControlValueChanged();
		}
		protected override object CreateControlCore() {
			ActivitiesInformationEventArgs activitiesInformationEventArgs = new ActivitiesInformationEventArgs(new List<ActivityInformation>());
			if(QueryAvailableActivities != null) {
				QueryAvailableActivities(this, activitiesInformationEventArgs);
			}
			WorkflowDesignerControl result = new WorkflowDesignerControl(activitiesInformationEventArgs.ActivitiesInformation);
			result.ObjectSpaceProvider = application.ObjectSpaceProvider;
			result.XamlChanged += new EventHandler(result_XamlChanged);
			return result;
		}
		public WorkflowPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "Xaml";
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.application = application;
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			base.BreakLinksToControl(unwireEventsOnly);
			if(WorkflowDesignerControl != null && !unwireEventsOnly) {
				WorkflowDesignerControl.XamlChanged -= new EventHandler(result_XamlChanged);
			}
		}
		public WorkflowDesignerControl WorkflowDesignerControl {
			get { return (WorkflowDesignerControl)Control; }
		}
		public event EventHandler<ActivitiesInformationEventArgs> QueryAvailableActivities;
		#region obsoleted since v13.1.8
		[Obsolete("Use 'WorkflowDesignerControl' instead.")]
		public WorkflowDesignerControl DesignerControl {
			get { return WorkflowDesignerControl; }
		}
		#endregion
	}
}
