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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class RefreshController : ViewController {
		private DevExpress.ExpressApp.Actions.SimpleAction refreshAction;
		private void refreshAction_OnExecute(Object sender, SimpleActionExecuteEventArgs e) {
			Refresh();
		}
		private void ObjectSpace_Reloaded(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void ObjectSpace_Committed(Object sender, EventArgs e) {
			if((View is ListView) && View.IsRoot && (((ListView)View).CollectionSource is CollectionSource)
				&& ((CollectionSource)((ListView)View).CollectionSource).IsServerMode) {
				((ListView)View).CollectionSource.Reload();
			}
			UpdateActionState();
		}
		protected virtual void Refresh() {
			IList refreshedObjectSpaces = new List<IObjectSpace>();
			if(ObjectSpace != null) {
				ObjectSpace.Refresh();
				refreshedObjectSpaces.Add(ObjectSpace);
			}
			if(View is CompositeView) {
				foreach(IFrameContainer frameContainer in ((CompositeView)View).GetItems<IFrameContainer>()) {
					if((frameContainer.Frame != null)
							&& (frameContainer.Frame.View != null)
							&& (frameContainer.Frame.View.ObjectSpace != null)
							&& !refreshedObjectSpaces.Contains(frameContainer.Frame.View.ObjectSpace)) {
						frameContainer.Frame.View.ObjectSpace.Refresh();
						refreshedObjectSpaces.Add(frameContainer.Frame.View.ObjectSpace);
					}
				}
			}
		}
		protected virtual void UpdateActionState() {
			refreshAction.Enabled.SetItemValue("Has context to move",
				(View is ListView) || (View is ISelectionContext) && !ObjectSpace.IsNewObject(((ISelectionContext)View).CurrentObject));
		}
		protected override void OnActivated() {
			base.OnActivated();
			refreshAction.Active.SetItemValue("Root view", View.IsRoot);
			if(ObjectSpace != null) {
				ObjectSpace.Reloaded += new EventHandler(ObjectSpace_Reloaded);
				ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
				UpdateActionState();
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			if(ObjectSpace != null) {
				ObjectSpace.Reloaded -= new EventHandler(ObjectSpace_Reloaded);
				ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			}
		}
		public RefreshController() {
			refreshAction = new SimpleAction();
			refreshAction.Caption = "Refresh";
			refreshAction.Category = "View";
			refreshAction.ConfirmationMessage = "";
			refreshAction.Id = "Refresh";
			refreshAction.ImageName = "MenuBar_Refresh";
			refreshAction.Shortcut = "F5";
			refreshAction.QuickAccess = true;
			refreshAction.Execute += new SimpleActionExecuteEventHandler(refreshAction_OnExecute);
			RegisterActions(refreshAction);
		}
		public SimpleAction RefreshAction {
			get { return refreshAction; }
		}
	}
}
