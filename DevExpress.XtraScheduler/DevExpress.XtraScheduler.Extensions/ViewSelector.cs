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
using DevExpress.XtraBars;
using System.ComponentModel;
using DevExpress.XtraScheduler;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.XtraScheduler.UI {
	#region ViewSelector
	[Obsolete(ObsoleteText.SRViewSelector, true)]
	public partial class ViewSelector : SchedulerCommandBarComponent {
		ViewSelectorItemHelperBase helper;
		public ViewSelector()
			: base() {
		}
		public ViewSelector(IContainer container)
			: base(container) {
			container.Add(this);
		}
		protected override Type SupportedBarType { get { return typeof(ViewSelectorBar); } }
		protected override Type SupportedBarItemType { get { return typeof(ViewSelectorItem); } }
		internal ViewSelectorItemHelperBase ItemHelper {
			get {
				if (helper == null)
					helper = new ViewSelectorItemHelper(this);
				return helper;
			}
		}
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ViewSelectorItemBuilder();
		}
		protected override void InitializeBarItemBuilder() {
			((ViewSelectorItemBuilder)BarItemBuilder).SchedulerControl = SchedulerControl;
		}
		protected override CommandBasedBar CreateBarInstance() {
			return new ViewSelectorBar();
		}
		protected internal override void UnsubscribeSchedulerControlEvents() {
			base.UnsubscribeSchedulerControlEvents();
			if (SchedulerControl != null) {
				SchedulerControl.ActiveViewChanged -= new EventHandler(SchedulerControl_ActiveViewChanged);
				SchedulerControl.ViewUIChanged -= new SchedulerViewUIChangedEventHandler(SchedulerControl_ViewUIChanged);
				SchedulerControl.InnerControl.ServiceManager.ServiceListChanged -= new EventHandler(ServiceManager_ServiceListChanged);
			}
		}
		protected internal override void SubscribeSchedulerControlEvents() {
			base.SubscribeSchedulerControlEvents();
			if (SchedulerControl != null) {
				SchedulerControl.ActiveViewChanged += new EventHandler(SchedulerControl_ActiveViewChanged);
				SchedulerControl.ViewUIChanged += new SchedulerViewUIChangedEventHandler(SchedulerControl_ViewUIChanged);
				SchedulerControl.InnerControl.ServiceManager.ServiceListChanged += new EventHandler(ServiceManager_ServiceListChanged);
			}
		}
		void ServiceManager_ServiceListChanged(object sender, EventArgs e) {
			ItemHelper.HandleServiceListChanged();
		}
		protected internal virtual void SchedulerControl_ActiveViewChanged(object sender, EventArgs e) {
			ItemHelper.HandleActiveViewChanged();
		}
		protected internal virtual void SchedulerControl_ViewUIChanged(object sender, SchedulerViewUIChangedEventArgs e) {
			ItemHelper.HandleViewUIChanged(e);
		}
	}
	#endregion
	#region ViewSelectorBar
	public class ViewSelectorBar : CommandBasedBar {
		public ViewSelectorBar()
			: base() {
		}
		public ViewSelectorBar(BarManager manager)
			: base(manager) {
		}
		public ViewSelectorBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_ViewSelector); } }
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	using DevExpress.XtraScheduler.UI;
	#region ViewSelectorItemHelper
	public class ViewSelectorItemHelper : ViewSelectorItemHelperBase {
		public ViewSelectorItemHelper(SchedulerCommandBarComponent viewSelector)
			: base(viewSelector) {
		}
		protected new SchedulerCommandBarComponent CommandBar {
			get { return (SchedulerCommandBarComponent)base.CommandBar; }
		}
		protected override SchedulerControl SchedulerControl {
			get { return CommandBar.SchedulerControl; }
		}
	}
	#endregion
}
