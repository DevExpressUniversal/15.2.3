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
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.SystemModule {
	public class ResetViewSettingsController : Controller {
		private SimpleAction resetViewSettingsAction;
		private void ObjectSpace_ModifiedChanged(Object sender, EventArgs e) {
			UpdateActionState();
		}
		private void Frame_ViewChanging(Object sender, ViewChangingEventArgs e) {
			if((Frame != null) && (Frame.View is ObjectView)) {
				((ObjectView)Frame.View).ObjectSpace.ModifiedChanged -= ObjectSpace_ModifiedChanged;
			}
		}
		private void Frame_ViewChanged(Object sender, ViewChangedEventArgs e) {
			resetViewSettingsAction.RaiseChanged(ActionChangedType.ToolTip);
			resetViewSettingsAction.RaiseChanged(ActionChangedType.ConfirmationMessage);
			if((Frame != null) && (Frame.View is ObjectView)) {
				((ObjectView)Frame.View).ObjectSpace.ModifiedChanged += ObjectSpace_ModifiedChanged;
			}
			UpdateActionState();
		}
		private void ResetViewSettingsAction_CustomGetTotalTooltip(Object sender, CustomGetTotalTooltipEventArgs e) {
			if((Frame != null) && (Frame.View != null)) {
				e.Tooltip = String.Format(e.Tooltip, Frame.View.Caption);
			}
		}
		private void ResetViewSettingsAction_CustomGetFormattedConfirmationMessage(Object sender, CustomGetFormattedConfirmationMessageEventArgs e) {
			if((Frame != null) && (Frame.View != null)) {
				e.ConfirmationMessage = String.Format(e.ConfirmationMessage, Frame.View.Caption);
			}
		}
		private void ResetViewSettingsAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			ResetViewSettings();
		}
		protected virtual void UpdateActionState() {
			String viewIsNotNullKey = "View is not null";
			String viewIsNotModifiedKey = "View is not modified";
			resetViewSettingsAction.Active.SetItemValue(viewIsNotNullKey, (Frame != null) && (Frame.View != null));
			if((Frame != null) && (Frame.View is ObjectView)) {
				resetViewSettingsAction.Enabled.SetItemValue(viewIsNotModifiedKey, !((ObjectView)Frame.View).ObjectSpace.IsModified);
			}
			else {
				resetViewSettingsAction.Enabled.RemoveItem(viewIsNotModifiedKey);
			}
		}
		protected virtual void ResetViewSettings() {
			if((Frame != null) && (Frame.View != null)) {
				IModelView viewModel1 = Frame.View.Model;
				IModelView viewModel2 = null;
				if((Frame.View is ListView) && (((ListView)Frame.View).EditView != null)) {
					viewModel2 = ((ListView)Frame.View).EditView.Model;
				}
				if(Frame.View.IsRoot) {
					ViewShortcut viewShortcut = Frame.View.CreateShortcut();
					if(Frame.SetView(null)) {
						((ModelNode)viewModel1).Undo();
						if(viewModel2 != null) {
							((ModelNode)viewModel2).Undo();
						}
						Frame.SetView(Application.ProcessShortcut(viewShortcut));
					}
				}
				else if((Frame is NestedFrame)
						&& (((NestedFrame)Frame).ViewItem is ListPropertyEditor)
						&& (((ListPropertyEditor)((NestedFrame)Frame).ViewItem).ListView != null)) {
					if(Frame.SetView(null)) {
						((ModelNode)viewModel1).Undo();
						if(viewModel2 != null) {
							((ModelNode)viewModel2).Undo();
						}
						((NestedFrame)Frame).ViewItem.CreateControl();
					}
				}
				else if((Frame is NestedFrame)
						&& (((NestedFrame)Frame).ViewItem is DetailPropertyEditor)
						&& (((DetailPropertyEditor)((NestedFrame)Frame).ViewItem).DetailView != null)) {
					if(Frame.SetView(null)) {
						((ModelNode)viewModel1).Undo();
						((NestedFrame)Frame).ViewItem.CreateControl();
					}
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Frame.ViewChanging += Frame_ViewChanging;
			Frame.ViewChanged += Frame_ViewChanged;
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			Frame.ViewChanging -= Frame_ViewChanging;
			Frame.ViewChanged -= Frame_ViewChanged;
		}
		public ResetViewSettingsController()
			: base() {
			resetViewSettingsAction = new SimpleAction(this, "ResetViewSettings", PredefinedCategory.View);
			resetViewSettingsAction.Caption = "Reset View Settings";
			resetViewSettingsAction.ImageName = "Action_ResetViewSettings";
			resetViewSettingsAction.ToolTip = @"Resets all settings made for the ""{0}"" view.";
			resetViewSettingsAction.ConfirmationMessage = @"You are about to reset all settings made for the ""{0}"" view. Do you want to proceed?";
			resetViewSettingsAction.CustomGetTotalTooltip += ResetViewSettingsAction_CustomGetTotalTooltip;
			resetViewSettingsAction.CustomGetFormattedConfirmationMessage += ResetViewSettingsAction_CustomGetFormattedConfirmationMessage;
			resetViewSettingsAction.Execute += new SimpleActionExecuteEventHandler(ResetViewSettingsAction_Execute);
		}
		public SimpleAction ResetViewSettingsAction {
			get { return resetViewSettingsAction; }
		}
	}
}
