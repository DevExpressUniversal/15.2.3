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
using System.ComponentModel;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
namespace DevExpress.XtraScheduler.UI {
	#region RibbonViewSelector
	[Obsolete(ObsoleteText.SRRibbonViewSelector, true)]
	public partial class RibbonViewSelector : SchedulerRibbonCommandBarComponent {
		ViewSelectorItemHelperBase helper;
		public RibbonViewSelector()
			: base() {
		}
		public RibbonViewSelector(IContainer container)
			: base(container) {
		}
		protected override Type SupportedRibbonPageGroupType { get { return typeof(ViewSelectorRibbonPageGroup); } }
		protected override Type SupportedRibbonPageType { get { return typeof(ViewSelectorRibbonPage); } }
		protected override Type SupportedBarItemType { get { return typeof(ViewSelectorItem); } }
		internal ViewSelectorItemHelperBase ItemHelper {
			get {
				if (helper == null)
					helper = new RibbonViewSelectorItemHelper(this);
				return helper;
			}
		}
		protected override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewSelectorRibbonPage();
		}
		protected override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ViewSelectorRibbonPageGroup();
		}
		protected override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ViewSelectorItemBuilder();
		}
		protected override void InitializeBarItemBuilder() {
			((ViewSelectorItemBuilder)BarItemBuilder).SchedulerControl = SchedulerControl;
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
	#region ViewSelectorRibbonPageGroup
	public class ViewSelectorRibbonPageGroup : CommandBasedRibbonPageGroup {
		public ViewSelectorRibbonPageGroup()
			: base() {
		}
		public ViewSelectorRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_ViewSelector); }
		}
	}
	#endregion
	#region ViewSelectorRibbonPage
	public class ViewSelectorRibbonPage : CommandBasedRibbonPage {
		public override string DefaultText { get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_ViewSelector); } }
		public ViewSelectorRibbonPage()
			: base() {
		}
		public ViewSelectorRibbonPage(string text)
			: base(text) {
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region RibbonViewSelectorItemHelper
	public class RibbonViewSelectorItemHelper : ViewSelectorItemHelperBase {
		public RibbonViewSelectorItemHelper(SchedulerRibbonCommandBarComponent viewSelector)
			: base(viewSelector) {
		}
		protected new SchedulerRibbonCommandBarComponent CommandBar {
			get { return (SchedulerRibbonCommandBarComponent)base.CommandBar; }
		}
		protected override SchedulerControl SchedulerControl {
			get { return CommandBar.SchedulerControl; }
		}
	}
	#endregion
}
