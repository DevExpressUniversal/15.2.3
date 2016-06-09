#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraScheduler.UI {
	#region ViewSelectorItemBuilder
	public class ViewSelectorItemBuilder : CommandBasedBarItemBuilder {
		SchedulerControl schedulerControl;
		public ViewSelectorItemBuilder() {
		}
		public SchedulerControl SchedulerControl { get { return schedulerControl; } set { schedulerControl = value; } }
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			if (SchedulerControl == null)
				return;
			SchedulerViewRepository views = schedulerControl.Views;
			if (views == null)
				return;
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				ViewSelectorItem btn = CreateItem(views[i]);
				items.Add(btn);
			}
		}
		protected internal virtual ViewSelectorItem CreateItem(SchedulerViewBase view) {
			return new ViewSelectorItem(view.Type);
		}
	}
	#endregion
	#region ViewSelectorItem
	public class ViewSelectorItem : SchedulerCommandBarCheckItem {
		#region Fields
		SchedulerViewType schedulerViewType;
		#endregion
		public ViewSelectorItem()
			: base() {
		}
		public ViewSelectorItem(SchedulerViewType viewType)
			: base() {
			this.schedulerViewType = viewType;
		}
		public ViewSelectorItem(SchedulerViewType viewType, string caption)
			: base(caption) {
			this.schedulerViewType = viewType;
		}
		public ViewSelectorItem(BarManager manager, SchedulerViewType viewType)
			: base(manager) {
			this.schedulerViewType = viewType;
		}
		public ViewSelectorItem(BarManager manager, SchedulerViewType viewType, string caption)
			: base(manager, caption) {
			this.schedulerViewType = viewType;
		}
		[Browsable(false)]
		public SchedulerViewType SchedulerViewType { get { return schedulerViewType; } set { schedulerViewType = value; } }
		protected override Image LoadDefaultImage() {
			SchedulerViewBase view = GetActiveView();
			return view != null ? view.InnerView.LoadSmallImage() : null;
		}
		protected override Image LoadDefaultLargeImage() {
			SchedulerViewBase view = GetActiveView();
			return view != null ? view.InnerView.LoadLargeImage() : null;
		}
		protected override Keys GetDefaultShortcutKeys() {
			SchedulerViewBase view = GetActiveView();
			return view != null ? view.InnerView.Shortcut : base.GetDefaultShortcutKeys();
		}
		protected override string GetDefaultCaption() {
			SchedulerViewBase view = GetActiveView();
			return view != null ? view.ShortDisplayName : string.Empty;
		}
		protected override string GetDefaultSuperTipDescription() {
			SchedulerViewBase view = GetActiveView();
			return view != null ? view.InnerView.Description : string.Empty;
		}
		protected override string GetDefaultSuperTipTitle() {
			if (Caption != GetDefaultCaption())
				return Caption;
			SchedulerViewBase view = GetActiveView();
			return view != null ? view.DisplayName : base.GetDefaultSuperTipTitle();
		}
		protected internal SchedulerViewBase GetActiveView() {
			if (SchedulerControl == null)
				return null;
			if (SchedulerControl.Views == null)
				return null;
			SchedulerViewBase view = SchedulerControl.Views[schedulerViewType];
			return view;
		}
		protected override Command CreateCommand() {
			SchedulerViewBase view = GetActiveView();
			if (view == null)
				return null;
			return new SwitchViewCommand(SchedulerControl, view.InnerView);
		}
		internal void InternalUpdateItemVisibility() {
			this.UpdateItemVisibility();
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	using DevExpress.XtraScheduler.UI;
	using DevExpress.Utils;
	#region ViewSelectorItemHelperBase (abstract class)
	public abstract class ViewSelectorItemHelperBase {
		readonly CommandBasedBarComponentBase commandBar;
		protected ViewSelectorItemHelperBase(CommandBasedBarComponentBase commandBar) {
			Guard.ArgumentNotNull(commandBar, "commandBar");
			this.commandBar = commandBar;
		}
		protected virtual CommandBasedBarComponentBase CommandBar { get { return commandBar; } }
		protected abstract SchedulerControl SchedulerControl { get; }
		public virtual void HandleActiveViewChanged() {
			if (SchedulerControl == null)
				return;
			List<BarItem> items = CommandBasedBarComponentBaseHelper.GetSupportedBarItems(CommandBar);
			foreach (ViewSelectorItem btn in items)
				if (btn.SchedulerViewType == SchedulerControl.ActiveViewType)
					btn.Checked = true;
		}
		public void HandleViewUIChanged(SchedulerViewUIChangedEventArgs e) {
			if (e.PropertyName == "Enabled")
				OnViewsEnabledChanged();
			if (e.PropertyName == "ShortDisplayName")
				OnViewsMenuCaptionChanged(e.ViewType, (string)e.OldValue, (string)e.NewValue);
		}
		public virtual void HandleServiceListChanged() {
			UpdateItemsVisibility();
		}
		protected internal virtual void OnViewsEnabledChanged() {
			UpdateItemsVisibility();
		}
		protected internal virtual void UpdateItemsVisibility() {
			List<BarItem> items = CommandBasedBarComponentBaseHelper.GetSupportedBarItems(CommandBar);
			foreach (ViewSelectorItem btn in items)
				btn.InternalUpdateItemVisibility();
		}
		protected internal virtual void OnViewsMenuCaptionChanged(SchedulerViewType viewType, string oldMenuCaption, string newMenuCaption) {
			if (oldMenuCaption == newMenuCaption)
				return;
			List<BarItem> items = CommandBasedBarComponentBaseHelper.GetSupportedBarItems(CommandBar);
			foreach (ViewSelectorItem btn in items)
				if (btn.SchedulerViewType == viewType) {
					if (btn.Caption == oldMenuCaption) {
						btn.Caption = newMenuCaption;
					}
				}
		}
	}
	#endregion
}
