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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditHeaderFooterToolsDesignCloseItemBuilder
	public class RichEditHeaderFooterToolsDesignCloseItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ClosePageHeaderFooterItem());
		}
	}
	#endregion
	#region ClosePageHeaderFooterItem
	public class ClosePageHeaderFooterItem: RichEditCommandBarButtonItem {
		public ClosePageHeaderFooterItem() {
		}
		public ClosePageHeaderFooterItem(BarManager manager)
			: base(manager) {
		}
		public ClosePageHeaderFooterItem(string caption)
			: base(caption) {
		}
		public ClosePageHeaderFooterItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ClosePageHeaderFooter; } }
	}
	#endregion
	#region RichEditHeaderFooterToolsDesignNavigationItemBuilder
	public class RichEditHeaderFooterToolsDesignNavigationItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GoToPageHeaderItem());
			items.Add(new GoToPageFooterItem());
			items.Add(new GoToNextHeaderFooterItem());
			items.Add(new GoToPreviousHeaderFooterItem());
			items.Add(new ToggleLinkToPreviousItem());
		}
	}
	#endregion
	#region GoToPageHeaderItem
	public class GoToPageHeaderItem: RichEditCommandBarButtonItem {
		public GoToPageHeaderItem() {
		}
		public GoToPageHeaderItem(BarManager manager)
			: base(manager) {
		}
		public GoToPageHeaderItem(string caption)
			: base(caption) {
		}
		public GoToPageHeaderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.GoToPageHeader; } }
	}
	#endregion
	#region GoToPageFooterItem
	public class GoToPageFooterItem: RichEditCommandBarButtonItem {
		public GoToPageFooterItem() {
		}
		public GoToPageFooterItem(BarManager manager)
			: base(manager) {
		}
		public GoToPageFooterItem(string caption)
			: base(caption) {
		}
		public GoToPageFooterItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.GoToPageFooter; } }
	}
	#endregion
	#region GoToPreviousHeaderFooterItem
	public class GoToPreviousHeaderFooterItem: RichEditCommandBarButtonItem {
		public GoToPreviousHeaderFooterItem() {
		}
		public GoToPreviousHeaderFooterItem(BarManager manager)
			: base(manager) {
		}
		public GoToPreviousHeaderFooterItem(string caption)
			: base(caption) {
		}
		public GoToPreviousHeaderFooterItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.GoToPreviousHeaderFooter; } }
	}
	#endregion
	#region GoToNextHeaderFooterItem
	public class GoToNextHeaderFooterItem: RichEditCommandBarButtonItem {
		public GoToNextHeaderFooterItem() {
		}
		public GoToNextHeaderFooterItem(BarManager manager)
			: base(manager) {
		}
		public GoToNextHeaderFooterItem(string caption)
			: base(caption) {
		}
		public GoToNextHeaderFooterItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.GoToNextHeaderFooter; } }
	}
	#endregion
	#region ToggleLinkToPreviousItem
	public class ToggleLinkToPreviousItem : RichEditCommandBarCheckItem {
		public ToggleLinkToPreviousItem() {
		}
		public ToggleLinkToPreviousItem(BarManager manager)
			: base(manager) {
		}
		public ToggleLinkToPreviousItem(string caption)
			: base(caption) {
		}
		public ToggleLinkToPreviousItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleHeaderFooterLinkToPrevious; } }
	}
	#endregion
	#region RichEditHeaderFooterToolsDesignOptionsItemBuilder
	public class RichEditHeaderFooterToolsDesignOptionsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ToggleDifferentFirstPageItem());
			items.Add(new ToggleDifferentOddAndEvenPagesItem());
		}
	}
	#endregion
	#region ToggleDifferentFirstPageItem
	public class ToggleDifferentFirstPageItem : RichEditCommandBarCheckItem {
		public ToggleDifferentFirstPageItem() {
		}
		public ToggleDifferentFirstPageItem(BarManager manager)
			: base(manager) {
		}
		public ToggleDifferentFirstPageItem(string caption)
			: base(caption) {
		}
		public ToggleDifferentFirstPageItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleDifferentFirstPage; } }
	}
	#endregion
	#region ToggleDifferentOddAndEvenPagesItem
	public class ToggleDifferentOddAndEvenPagesItem : RichEditCommandBarCheckItem {
		public ToggleDifferentOddAndEvenPagesItem() {
		}
		public ToggleDifferentOddAndEvenPagesItem(BarManager manager)
			: base(manager) {
		}
		public ToggleDifferentOddAndEvenPagesItem(string caption)
			: base(caption) {
		}
		public ToggleDifferentOddAndEvenPagesItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ToggleDifferentOddAndEvenPages; } }
	}
	#endregion
	#region RichEditHeaderFooterToolsDesignCloseBarCreator
	public class RichEditHeaderFooterToolsDesignCloseBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(HeaderFooterToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageType { get { return typeof(HeaderFooterToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(HeaderFooterToolsDesignCloseRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(HeaderFooterToolsDesignCloseBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new HeaderFooterToolsDesignCloseBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditHeaderFooterToolsDesignCloseItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new HeaderFooterToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HeaderFooterToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new HeaderFooterToolsDesignCloseRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditHeaderFooterToolsDesignNavigationBarCreator
	public class RichEditHeaderFooterToolsDesignNavigationBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(HeaderFooterToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageType { get { return typeof(HeaderFooterToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(HeaderFooterToolsDesignNavigationRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(HeaderFooterToolsDesignNavigationBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new HeaderFooterToolsDesignNavigationBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditHeaderFooterToolsDesignNavigationItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new HeaderFooterToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HeaderFooterToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new HeaderFooterToolsDesignNavigationRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditHeaderFooterToolsDesignOptionsBarCreator
	public class RichEditHeaderFooterToolsDesignOptionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(HeaderFooterToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageType { get { return typeof(HeaderFooterToolsDesignRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(HeaderFooterToolsDesignOptionsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(HeaderFooterToolsDesignOptionsBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new HeaderFooterToolsDesignOptionsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditHeaderFooterToolsDesignOptionsItemBuilder();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new HeaderFooterToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new HeaderFooterToolsDesignRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new HeaderFooterToolsDesignOptionsRibbonPageGroup();
		}
	}
	#endregion
	#region HeaderFooterToolsDesignCloseBar
	public class HeaderFooterToolsDesignCloseBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public HeaderFooterToolsDesignCloseBar()
			: base() {
		}
		public HeaderFooterToolsDesignCloseBar(BarManager manager)
			: base(manager) {
		}
		public HeaderFooterToolsDesignCloseBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignClose); } }
	}
	#endregion
	#region HeaderFooterToolsDesignNavigationBar
	public class HeaderFooterToolsDesignNavigationBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public HeaderFooterToolsDesignNavigationBar()
			: base() {
		}
		public HeaderFooterToolsDesignNavigationBar(BarManager manager)
			: base(manager) {
		}
		public HeaderFooterToolsDesignNavigationBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignNavigation); } }
	}
	#endregion
	#region HeaderFooterToolsDesignOptionsBar
	public class HeaderFooterToolsDesignOptionsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public HeaderFooterToolsDesignOptionsBar()
			: base() {
		}
		public HeaderFooterToolsDesignOptionsBar(BarManager manager)
			: base(manager) {
		}
		public HeaderFooterToolsDesignOptionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignOptions); } }
	}
	#endregion
	#region HeaderFooterToolsDesignRibbonPage
	public class HeaderFooterToolsDesignRibbonPage : ControlCommandBasedRibbonPage {
		public HeaderFooterToolsDesignRibbonPage() {
		}
		public HeaderFooterToolsDesignRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageHeaderFooterToolsDesign); } }
		protected override RibbonPage CreatePage() {
			return new HeaderFooterToolsDesignRibbonPage();
		}
	}
	#endregion
	#region HeaderFooterToolsDesignCloseRibbonPageGroup
	public class HeaderFooterToolsDesignCloseRibbonPageGroup : RichEditControlRibbonPageGroup {
		public HeaderFooterToolsDesignCloseRibbonPageGroup() {
		}
		public HeaderFooterToolsDesignCloseRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignClose); } }
	}
	#endregion
	#region HeaderFooterToolsDesignNavigationRibbonPageGroup
	public class HeaderFooterToolsDesignNavigationRibbonPageGroup : RichEditControlRibbonPageGroup {
		public HeaderFooterToolsDesignNavigationRibbonPageGroup() {
		}
		public HeaderFooterToolsDesignNavigationRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignNavigation); } }
	}
	#endregion
	#region HeaderFooterToolsDesignOptionsRibbonPageGroup
	public class HeaderFooterToolsDesignOptionsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public HeaderFooterToolsDesignOptionsRibbonPageGroup() {
		}
		public HeaderFooterToolsDesignOptionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooterToolsDesignOptions); } }
	}
	#endregion
	#region HeaderFooterToolsRibbonPageCategory
	public class HeaderFooterToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<RichEditControl, RichEditCommandId> {
		public HeaderFooterToolsRibbonPageCategory() {
			this.Color = DXColor.FromArgb(0xff, 0x26, 0xb0, 0x23);
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageCategoryHeaderFooterTools); } }
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
		public override RichEditCommandId CommandId { get { return RichEditCommandId.ToolsHeaderFooterCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new HeaderFooterToolsRibbonPageCategory();
		}
	}
	#endregion
}
