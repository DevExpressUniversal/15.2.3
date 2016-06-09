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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditDocumentViewsItemBuilder
	public class RichEditDocumentViewsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new SwitchToSimpleViewItem());
			items.Add(new SwitchToDraftViewItem());
			items.Add(new SwitchToPrintLayoutViewItem());
		}
	}
	#endregion
	#region SwitchToDraftViewItem
	public class SwitchToDraftViewItem : RichEditCommandBarCheckItem {
		public SwitchToDraftViewItem() {
		}
		public SwitchToDraftViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToDraftViewItem(string caption)
			: base(caption) {
		}
		public SwitchToDraftViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SwitchToDraftView; } }
		protected override void SetupStatusBarLink(BarItemLink link) {
			base.SetupStatusBarLink(link);
			link.UserAlignment = BarItemLinkAlignment.Right;
			link.UserRibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
	}
	#endregion
	#region SwitchToPrintLayoutViewItem
	public class SwitchToPrintLayoutViewItem : RichEditCommandBarCheckItem {
		public SwitchToPrintLayoutViewItem() {
		}
		public SwitchToPrintLayoutViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToPrintLayoutViewItem(string caption)
			: base(caption) {
		}
		public SwitchToPrintLayoutViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SwitchToPrintLayoutView; } }
		protected override void SetupStatusBarLink(BarItemLink link) {
			base.SetupStatusBarLink(link);
			link.UserAlignment = BarItemLinkAlignment.Right;
			link.UserRibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
	}
	#endregion
	#region SwitchToSimpleViewItem
	public class SwitchToSimpleViewItem : RichEditCommandBarCheckItem {
		public SwitchToSimpleViewItem() {
		}
		public SwitchToSimpleViewItem(BarManager manager)
			: base(manager) {
		}
		public SwitchToSimpleViewItem(string caption)
			: base(caption) {
		}
		public SwitchToSimpleViewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.SwitchToSimpleView; } }
		protected override void SetupStatusBarLink(BarItemLink link) {
			base.SetupStatusBarLink(link);
			link.UserAlignment = BarItemLinkAlignment.Right;
			link.UserRibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
	}
	#endregion
	#region RichEditShowItemBuilder
	public class RichEditShowItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ToggleShowHorizontalRulerItem());
			items.Add(new ToggleShowVerticalRulerItem());
		}
	}
	#endregion
	#region ToggleShowHorizontalRulerItem
	public class ToggleShowHorizontalRulerItem : RichEditCommandBarCheckItem {
		public ToggleShowHorizontalRulerItem() {
		}
		public ToggleShowHorizontalRulerItem(BarManager manager)
			: base(manager) {
		}
		public ToggleShowHorizontalRulerItem(string caption)
			: base(caption) {
		}
		public ToggleShowHorizontalRulerItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleShowHorizontalRuler; } }
	}
	#endregion
	#region ToggleShowVerticalRulerItem
	public class ToggleShowVerticalRulerItem : RichEditCommandBarCheckItem {
		public ToggleShowVerticalRulerItem() {
		}
		public ToggleShowVerticalRulerItem(BarManager manager)
			: base(manager) {
		}
		public ToggleShowVerticalRulerItem(string caption)
			: base(caption) {
		}
		public ToggleShowVerticalRulerItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleShowVerticalRuler; } }
	}
	#endregion
	#region RichEditZoomItemBuilder
	public class RichEditZoomItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ZoomOutItem());
			items.Add(new ZoomInItem());
		}
	}
	#endregion
	#region ZoomInItem
	public class ZoomInItem: RichEditCommandBarButtonItem {
		public ZoomInItem() {
		}
		public ZoomInItem(BarManager manager)
			: base(manager) {
		}
		public ZoomInItem(string caption)
			: base(caption) {
		}
		public ZoomInItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ZoomIn; } }
	}
	#endregion
	#region ZoomOutItem
	public class ZoomOutItem: RichEditCommandBarButtonItem {
		public ZoomOutItem() {
		}
		public ZoomOutItem(BarManager manager)
			: base(manager) {
		}
		public ZoomOutItem(string caption)
			: base(caption) {
		}
		public ZoomOutItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ZoomOut; } }
	}
	#endregion
	#region RichEditDocumentViewsBarCreator
	public class RichEditDocumentViewsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(DocumentViewsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(DocumentViewsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new DocumentViewsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditDocumentViewsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new DocumentViewsRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditShowBarCreator
	public class RichEditShowBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ShowRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ShowBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new ShowBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditShowItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ShowRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditZoomBarCreator
	public class RichEditZoomBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ViewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ZoomRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ZoomBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new ZoomBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditZoomItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ViewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ZoomRibbonPageGroup();
		}
	}
	#endregion
	#region DocumentViewsBar
	public class DocumentViewsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public DocumentViewsBar() {
		}
		public DocumentViewsBar(BarManager manager)
			: base(manager) {
		}
		public DocumentViewsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentViews); } }
	}
	#endregion
	#region ShowBar
	public class ShowBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public ShowBar() {
		}
		public ShowBar(BarManager manager)
			: base(manager) {
		}
		public ShowBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupShow); } }
	}
	#endregion
	#region ZoomBar
	public class ZoomBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public ZoomBar() {
		}
		public ZoomBar(BarManager manager)
			: base(manager) {
		}
		public ZoomBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupZoom); } }
	}
	#endregion
	#region ViewRibbonPage
	public class ViewRibbonPage : ControlCommandBasedRibbonPage {
		public ViewRibbonPage() {
		}
		public ViewRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageView); } }
		protected override RibbonPage CreatePage() {
			return new ViewRibbonPage();
		}
	}
	#endregion
	#region DocumentViewsRibbonPageGroup
	public class DocumentViewsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public DocumentViewsRibbonPageGroup() {
		}
		public DocumentViewsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupDocumentViews); } }
	}
	#endregion
	#region ShowRibbonPageGroup
	public class ShowRibbonPageGroup : RichEditControlRibbonPageGroup {
		public ShowRibbonPageGroup() {
		}
		public ShowRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupShow); } }
	}
	#endregion
	#region ZoomRibbonPageGroup
	public class ZoomRibbonPageGroup : RichEditControlRibbonPageGroup {
		public ZoomRibbonPageGroup() {
		}
		public ZoomRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupZoom); } }
	}
		#endregion
}
