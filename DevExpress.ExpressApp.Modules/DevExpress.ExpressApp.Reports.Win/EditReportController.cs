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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
namespace DevExpress.ExpressApp.Reports.Win {
	public class EditReportController : ObjectViewController {
		private SimpleAction editReportAction;
		private void View_SelectionChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateActionState();
		}
		void editReportAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ListViewProcessCurrentObjectController.ShowObject(e.CurrentObject, e.ShowViewParameters, Application, Frame, View);
			((DetailView)e.ShowViewParameters.CreatedView).ViewEditMode = ViewEditMode.Edit;
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			UpdateActionState();
			ListView listView = View as ListView;
			editReportAction.Active["View is ListView"] = (listView != null);
		}
		private void UpdateActionState() {
			if(View.SelectedObjects.Count == 1) {
				editReportAction.Enabled.SetItemValue("Security", DataManipulationRight.CanEdit(((ObjectView)View).ObjectTypeInfo.Type, null, View.CurrentObject, LinkToListViewController.FindCollectionSource(Frame), View.ObjectSpace));
			}
		}
		public EditReportController()
			: base() {
			editReportAction = new SimpleAction(this, "EditReportController.Edit", PredefinedCategory.Edit);
			editReportAction.Caption = "Edit";
			editReportAction.ImageName = "MenuBar_Edit";
			editReportAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			editReportAction.Execute += new SimpleActionExecuteEventHandler(editReportAction_Execute);
			TypeOfView = typeof(ObjectView);
			TargetObjectType = typeof(IReportData);
		}
		public SimpleAction EditReportAction {
			get { return editReportAction; }
		}
	}
}
