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
using System.Text;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class FindLookupNewObjectDialogController : DialogController {
		private FindLookupDialogController owner;
		protected override void Accept(SimpleActionExecuteEventArgs args) {
			Window.View.ObjectSpace.CommitChanges();
			owner.SetSearchTextForObject(Window.View.CurrentObject);
			Window.Close(true);
		}
		public FindLookupNewObjectDialogController() { }
		public void Initialize(FindLookupDialogController owner) {
			this.owner = owner;
		}
	}
	public class FindLookupDialogController : DialogController {
		private LookupEditorHelper helper;
		private void NewObjectAction_Executed(object sender, ActionBaseEventArgs e) {
			e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
			FindLookupNewObjectDialogController controller = Application.CreateController<FindLookupNewObjectDialogController>();
			controller.Initialize(this);
			e.ShowViewParameters.Controllers.Add(controller);
		}
		protected override void OnDeactivated() {
			NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
			if(newObjectViewController != null) {
				newObjectViewController.NewObjectAction.Executed -= new EventHandler<ActionBaseEventArgs>(NewObjectAction_Executed);
			}
			base.OnDeactivated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			NewObjectViewController newObjectViewController = Frame.GetController<NewObjectViewController>();
			if(newObjectViewController != null) {
				newObjectViewController.NewObjectAction.Executed += new EventHandler<ActionBaseEventArgs>(NewObjectAction_Executed);
			}
			AcceptAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
		}
		public FindLookupDialogController()
			: base() {
			SaveOnAccept = false;
		}
		public void Initialize(LookupEditorHelper helper) {
			this.helper = helper;
		}
		public void SetSearchTextForObject(object obj) {
			FilterController filterController = Frame.GetController<FilterController>();
			if(filterController != null) {
				filterController.FullTextFilterAction.DoExecute(helper.GetDisplayText(obj, "", null));
				if(filterController.View.CollectionSource is LookupEditPropertyCollectionSource) {
					filterController.View.CollectionSource.Reload();
				}
			}
		}
	}
}
