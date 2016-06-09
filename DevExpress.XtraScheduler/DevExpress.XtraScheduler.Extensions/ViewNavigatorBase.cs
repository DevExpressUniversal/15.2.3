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
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Commands.Internal;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.UI {
	#region ViewNavigatorItemBuilder
	public class ViewNavigatorItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(CreateBackwardBarItem());
			items.Add(CreateForwardBarItem());
			items.Add(CreateGotoTodayItem());
			items.Add(CreateZoomInBarItem());
			items.Add(CreateZoomOutBarItem());
		}
		protected virtual ViewNavigatorZoomInItem CreateZoomInBarItem() {
			return new ViewNavigatorZoomInItem();
		}
		protected virtual ViewNavigatorZoomOutItem CreateZoomOutBarItem() {
			return new ViewNavigatorZoomOutItem();
		}
		protected virtual ViewNavigatorBackwardItem CreateBackwardBarItem() {
			return new ViewNavigatorBackwardItem();
		}
		protected virtual ViewNavigatorForwardItem CreateForwardBarItem() {
			return new ViewNavigatorForwardItem();
		}
		protected virtual ViewNavigatorTodayItem CreateGotoTodayItem() {
			return new ViewNavigatorTodayItem();
		}
	}
	#endregion
	#region ViewNavigatorBarItemBase (abstract class)
	public abstract class ViewNavigatorItemBase : SchedulerCommandBarButtonItemBase {
		protected ViewNavigatorItemBase()
			: base() {
		}
		protected ViewNavigatorItemBase(string caption)
			: base(caption) {
		}
		protected ViewNavigatorItemBase(BarManager manager)
			: base(manager) {
		}
		protected ViewNavigatorItemBase(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected internal override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
			SchedulerControl.InnerControl.AfterApplyChanges += new AfterApplyChangesEventHandler(OnInnerControlAfterApplyChanges);
			SchedulerControl.InnerControl.ServiceManager.ServiceListChanged += new EventHandler(OnServiceListChanged);
		}
		protected internal override void UnsubscribeControlEvents() {
			SchedulerControl.InnerControl.AfterApplyChanges -= new AfterApplyChangesEventHandler(OnInnerControlAfterApplyChanges);
			SchedulerControl.InnerControl.ServiceManager.ServiceListChanged -= new EventHandler(OnServiceListChanged);
			base.UnsubscribeControlEvents();
		}
		protected internal virtual void OnInnerControlAfterApplyChanges(object sender, AfterApplyChangesEventArgs e) {
			this.UpdateItemVisibility();
		}
		protected internal virtual void OnServiceListChanged(object sender, EventArgs e) {
			this.UpdateItemVisibility();
		}
	}
	#endregion
	#region ViewNavigatorTodayItem
	public class ViewNavigatorTodayItem : ViewNavigatorItemBase {
		public ViewNavigatorTodayItem() {
		}
		public ViewNavigatorTodayItem(BarManager manager)
			: base(manager) {
		}
		public ViewNavigatorTodayItem(string caption)
			: base(caption) {
		}
		public ViewNavigatorTodayItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override string GetDefaultCaption() {
			return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.CaptionViewNavigator_Today);
		}
		protected override Command CreateCommand() {
			if (SchedulerControl == null)
				return null;
			return new ServiceGotoTodayCommand(SchedulerControl);
		}
	}
	#endregion
	#region ViewNavigatorBackwardItem
	public class ViewNavigatorBackwardItem : ViewNavigatorItemBase {
		public ViewNavigatorBackwardItem() {
		}
		public ViewNavigatorBackwardItem(BarManager manager)
			: base(manager) {
		}
		public ViewNavigatorBackwardItem(string caption)
			: base(caption) {
		}
		public ViewNavigatorBackwardItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override Command CreateCommand() {
			if (SchedulerControl == null)
				return null;
			return new ServiceNavigateViewBackwardCommand(SchedulerControl);
		}
	}
	#endregion
	#region ViewNavigatorForwardItem
	public class ViewNavigatorForwardItem : ViewNavigatorItemBase {
		public ViewNavigatorForwardItem() {
		}
		public ViewNavigatorForwardItem(BarManager manager)
			: base(manager) {
		}
		public ViewNavigatorForwardItem(string caption)
			: base(caption) {
		}
		public ViewNavigatorForwardItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override Command CreateCommand() {
			if (SchedulerControl == null)
				return null;
			return new ServiceNavigateViewForwardCommand(SchedulerControl);
		}
	}
	#endregion
	#region ViewNavigatorZoomInItem
	public class ViewNavigatorZoomInItem : ViewNavigatorItemBase {
		public ViewNavigatorZoomInItem() {
		}
		public ViewNavigatorZoomInItem(BarManager manager)
			: base(manager) {
		}
		public ViewNavigatorZoomInItem(string caption)
			: base(caption) {
		}
		public ViewNavigatorZoomInItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override Keys GetDefaultShortcutKeys() {
			return Keys.Add | Keys.Control;
		}
		protected override Command CreateCommand() {
			if (SchedulerControl == null)
				return null;
			return new ViewZoomInCommand(SchedulerControl);
		}
	}
	#endregion
	#region ViewNavigatorZoomOutItem
	public class ViewNavigatorZoomOutItem : ViewNavigatorItemBase {
		public ViewNavigatorZoomOutItem() {
		}
		public ViewNavigatorZoomOutItem(BarManager manager)
			: base(manager) {
		}
		public ViewNavigatorZoomOutItem(string caption)
			: base(caption) {
		}
		public ViewNavigatorZoomOutItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override Keys GetDefaultShortcutKeys() {
			return Keys.Subtract | Keys.Control;
		}
		protected override Command CreateCommand() {
			if (SchedulerControl == null)
				return null;
			return new ViewZoomOutCommand(SchedulerControl);
		}
	}
	#endregion
}
