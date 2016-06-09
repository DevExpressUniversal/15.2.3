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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Security.Strategy.PermissionMatrix {
	public class ListViewProcessMatrixTypePermissionObjectController : ViewController<ListView> {
		private void processObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			TypePermissionMatrixItem permissionsObject = e.InnerArgs.CurrentObject as TypePermissionMatrixItem;
			if(permissionsObject != null) {
				IObjectSpace targetObjectSpace = View.ObjectSpace.CreateNestedObjectSpace();
				SecuritySystemTypePermissionObject source = permissionsObject.CreateSourceInSpecificObjectSpace(targetObjectSpace);
				targetObjectSpace.Committed += delegate(object committedOs, EventArgs args) {
					permissionsObject.Source = View.ObjectSpace.GetObject<SecuritySystemTypePermissionObject>(source);
				};
				DetailView currentObjectView = Application.CreateDetailView(targetObjectSpace, source, true);
				ShowViewParameters parameters = new ShowViewParameters(currentObjectView);
				parameters.TargetWindow = TargetWindow.NewModalWindow;
				parameters.Context = TemplateContext.PopupWindow;
				DialogController dialogController = Application.CreateController<DialogController>();
				parameters.Controllers.Add(dialogController);
				Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(Frame, null));
				currentObjectView.ViewEditMode = ViewEditMode.View;
				e.Handled = true;
			}
		}
		private void Application_DetailViewCreating(object sender, DetailViewCreatingEventArgs e) {
			if(e.Obj != null && e.Obj is TypePermissionMatrixItem) {
				TypePermissionMatrixItem permissionsObject = e.Obj as TypePermissionMatrixItem;
				if(permissionsObject != null) {
					SecuritySystemTypePermissionObject source = permissionsObject.CreateSourceInSpecificObjectSpace(e.ObjectSpace);
					e.View = Application.CreateDetailView(e.ObjectSpace, source, true);
					e.ObjectSpace.Committed += delegate(object committedOs, EventArgs args) {
						permissionsObject.Source = e.View.ObjectSpace.GetObject<SecuritySystemTypePermissionObject>(source);
					};
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListViewProcessCurrentObjectController processObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processObjectController != null) {
				processObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processObjectController_CustomProcessSelectedItem);
				Application.DetailViewCreating += new EventHandler<DetailViewCreatingEventArgs>(Application_DetailViewCreating);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ListViewProcessCurrentObjectController processObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processObjectController != null) {
				processObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processObjectController_CustomProcessSelectedItem);
				Application.DetailViewCreating -= new EventHandler<DetailViewCreatingEventArgs>(Application_DetailViewCreating);
			}
		}
		public ListViewProcessMatrixTypePermissionObjectController() {
			TargetObjectType = typeof(TypePermissionMatrixItem);
		}
	}
}
