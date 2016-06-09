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
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class WebNewObjectViewController : NewObjectViewController {
		private SingleChoiceAction quickCreateAction;
		private const string PopupWindowContextKey = "PopupWindowContext";
		private ViewEditMode? collectionsEditMode = null;
		private void quickCreateAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			New(args);
		}
		protected override void UpdateActionState() {
			base.UpdateActionState();
			NewObjectAction.BeginUpdate();
			try {
				NewObjectAction.Items.Clear();
				NewObjectAction.Items.AddRange(CollectDescendantItems());
				if(NewObjectAction.Items.Count > 0) {
					NewObjectAction.SelectedItem = NewObjectAction.Items.FirstActiveItem;
				}
			}
			finally {
				NewObjectAction.EndUpdate();
			}
			quickCreateAction.BeginUpdate();
			try {
				quickCreateAction.Items.Clear();
				quickCreateAction.Items.AddRange(CollectRootObjectItems());
			}
			finally {
				quickCreateAction.EndUpdate();
			}
		}
		protected override void UpdateActionActivity(ActionBase action) {
			base.UpdateActionActivity(action);
			action.Active[PopupWindowContextKey] = Frame.Context != TemplateContext.PopupWindow;
		}
		protected override IObjectSpace CreateObjectSpace(Type objectType) {
			PropertyCollectionSource propertyCollectionSource = (GetCurrentCollectionSource() as PropertyCollectionSource);
			if(propertyCollectionSource != null && propertyCollectionSource.MemberInfo.IsAggregated && CollectionsEditMode.HasValue && CollectionsEditMode.Value == ViewEditMode.View) {
				return Application.CreateObjectSpace(objectType);
			}
			else {
				return base.CreateObjectSpace(objectType);
			}
		}
		protected override Boolean NeedToUpdateNewObjectAction() {
			return base.NeedToUpdateNewObjectAction() && !(CollectionsEditMode.HasValue && CollectionsEditMode.Value == ViewEditMode.View);
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View is ListView && CollectionsEditMode.HasValue && CollectionsEditMode.Value == ViewEditMode.View) {
				linkNewObjectToParentImmediately = true;
			}
		}
		public WebNewObjectViewController() {
			quickCreateAction = new SingleChoiceAction(this, "QuickCreateAction", "RootObjectsCreation");
			quickCreateAction.ShowItemsOnClick = true;
			quickCreateAction.Caption = "Quick create";
			quickCreateAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			quickCreateAction.Execute += new SingleChoiceActionExecuteEventHandler(quickCreateAction_OnExecute);
			Actions.Add(quickCreateAction);
		}
		public ViewEditMode? CollectionsEditMode {
			get {
				if(!collectionsEditMode.HasValue) {
					collectionsEditMode = ShowViewStrategy.GetCollectionsEditMode(Frame, Application);
				}
				return collectionsEditMode;
			}
		}
		public SingleChoiceAction QuickCreateAction {
			get { return quickCreateAction; }
		}
	}
}
