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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Actions;
using System.ComponentModel;
using DevExpress.ExpressApp.Templates;
namespace DevExpress.ExpressApp.ConditionalAppearance {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ActionAppearanceItem : IAppearanceEnabled, IAppearanceVisibility {
		private DevExpress.ExpressApp.Actions.ActionBase action;
		public ActionAppearanceItem(DevExpress.ExpressApp.Actions.ActionBase action) {
			this.action = action;
		}
		public string ActionId {
			get { return action.Id; }
		}
		#region IAppearanceEnabled Members
		bool IAppearanceEnabled.Enabled {
			get { return action.Enabled; }
			set { action.Enabled.SetItemValue("ByAppearance", value); }
		}
		public void ResetEnabled() {
			action.Enabled.RemoveItem("ByAppearance");
		}
		#endregion
		#region IAppearanceVisibility Members
		public ViewItemVisibility Visibility {
			get { return action.Active ? ViewItemVisibility.Show : ViewItemVisibility.Hide; }
			set {
				action.Active.SetItemValue("ByAppearance", value == ViewItemVisibility.Show);
				if(VisibilityChanged != null) {
					VisibilityChanged(this, EventArgs.Empty);
				}
			}
		}
		public void ResetVisibility() {
			action.Active.RemoveItem("ByAppearance");
		}
		public event EventHandler VisibilityChanged;
		#endregion
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class InplaceActionAppearanceItem : IAppearanceEnabled {
		private BoundItemCreatingEventArgs args;
		public InplaceActionAppearanceItem(BoundItemCreatingEventArgs args) {
			this.args = args;
		}
		#region IAppearanceEnabled Members
		public bool Enabled { get { return args.Enabled; } set { if(Enabled) { args.Enabled = value; } } }
		void IAppearanceEnabled.ResetEnabled() { }
		#endregion
	}
	public interface ISupportResetCache {
		void ResetCache();
	}
	public class ActionAppearanceController : ObjectViewController, ISupportRefreshItemsAppearance, ISupportResetCache {
		private const bool ForceUpdateOnSelectionChangedDefaultValue = true;
		private List<ActionAppearanceItem> actionAppearanceItems = null;
		private bool forceUpdateOnSelectionChanged = ForceUpdateOnSelectionChangedDefaultValue;
#if DebugTest
		public List<ActionAppearanceItem> DebugTest_ActionAppearanceItems {
			get { return ActionAppearanceItems; }
		}
#endif
		internal List<ActionAppearanceItem> ActionAppearanceItems {
			get {
				if(actionAppearanceItems == null) {
					CollectActions();
				}
				return actionAppearanceItems;
			}
		}
		private void CollectActions() {
			List<string> actionsMentionedInRules = new List<string>();
			if(actionAppearanceItems == null) {
				actionAppearanceItems = new List<ActionAppearanceItem>();
			}
			actionAppearanceItems.Clear();
			AppearanceController appearanceController = Frame.GetController<AppearanceController>();
			actionsMentionedInRules.AddRange(appearanceController.GetTargetedActions(View));
			foreach(Controller controller in Frame.Controllers) {
				foreach(ActionBase action in controller.Actions) {
					if(actionsMentionedInRules.Contains(action.Id)) {
						actionAppearanceItems.Add(new ActionAppearanceItem(action));
					}
				}
			}
		}
		private object[] GetContextObjects() {
			object[] contextObjects = new object[View.SelectedObjects.Count];
			View.SelectedObjects.CopyTo(contextObjects, 0);
			return contextObjects;
		}
		private void SubscribeToContextMenuTemplate() {
			ListView listView = View as ListView;
			if(listView != null) {
				DevExpress.ExpressApp.Editors.ListEditor listEditor = listView.Editor;
				if(listEditor != null) {
					IContextMenuTemplate contextMenuTemplate = listEditor.ContextMenuTemplate;
					if(contextMenuTemplate != null) {
						contextMenuTemplate.BoundItemCreating += new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
					}
				}
			}
		}
		private void UnsubscribeFromContextMenuTemplate() {
			ListView listView = View as ListView;
			if(listView != null) {
				DevExpress.ExpressApp.Editors.ListEditor listEditor = listView.Editor;
				if(listEditor != null) {
					IContextMenuTemplate contextMenuTemplate = listEditor.ContextMenuTemplate;
					if(contextMenuTemplate != null) {
						contextMenuTemplate.BoundItemCreating -= new EventHandler<BoundItemCreatingEventArgs>(contextMenuTemplate_BoundItemCreating);
					}
				}
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			SubscribeToContextMenuTemplate();
		}
		private void View_ControlsCreating(object sender, EventArgs e) {
			UnsubscribeFromContextMenuTemplate();
		}
		private void contextMenuTemplate_BoundItemCreating(object sender, BoundItemCreatingEventArgs args) {
			Frame.GetController<AppearanceController>().RefreshItemAppearance(ViewInfo.FromView(View), AppearanceController.AppearanceActionType, args.Action.Id, new InplaceActionAppearanceItem(args), args.Object, null);
		}
		private void appearanceController_Activated(object sender, EventArgs e) {
			RefreshActionsAppearance();
			((AppearanceController)sender).Activated -= new EventHandler(appearanceController_Activated);
		}
		protected override void OnActivated() {
			base.OnActivated();
			CollectActions();
			AppearanceController appearanceController = Frame.GetController<AppearanceController>();
			appearanceController.RegisterController(this);
			if(!appearanceController.Active) {
				appearanceController.Activated += new EventHandler(appearanceController_Activated);
			}
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			View.ControlsCreating += new EventHandler(View_ControlsCreating);
			View.SelectionChanged += new EventHandler(View_SelectionChanged);
		}
		void View_SelectionChanged(object sender, EventArgs e)
		{
			if (ForceUpdateOnSelectionChanged) {
				RefreshActionsAppearance();
			}
		}
		protected override void OnDeactivated() {
			UnsubscribeFromContextMenuTemplate();
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			View.ControlsCreating -= new EventHandler(View_ControlsCreating);
			View.SelectionChanged -= new EventHandler(View_SelectionChanged);
			foreach(ActionAppearanceItem actionItem in ActionAppearanceItems) { 
				actionItem.ResetEnabled();
				actionItem.ResetVisibility();
			}
			Frame.GetController<AppearanceController>().UnRegisterController(this);
			base.OnDeactivated();
		}
		#region ISupportRefreshItemsAppearance Members
		void ISupportRefreshItemsAppearance.RefreshViewItemsAppearance() {
			RefreshActionsAppearance();
		}
		#endregion
		public void RefreshActionsAppearance()
		{
			AppearanceController appearanceController = Frame.GetController<AppearanceController>();
			if (appearanceController.Active)
			{
				foreach (ActionAppearanceItem actionItem in ActionAppearanceItems)
				{
					appearanceController.RefreshItemAppearance(ViewInfo.FromView(View), AppearanceController.AppearanceActionType, actionItem.ActionId, actionItem, GetContextObjects(), null);
				}
			}
		}
		public ActionAppearanceController() {
			TypeOfView = typeof(ObjectView);
		}
		#region ISupportResetCache Members
		public void ResetCache() {
			if(actionAppearanceItems != null) {
				actionAppearanceItems.Clear();
				actionAppearanceItems = null;
			}
		}
		#endregion
		[DefaultValue(ForceUpdateOnSelectionChangedDefaultValue)]
		public bool ForceUpdateOnSelectionChanged {
			get { return forceUpdateOnSelectionChanged; }
			set { forceUpdateOnSelectionChanged = value; }
		}
	}
}
