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
using System.Reflection;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using System.Drawing;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditIllustrationsItemBuilder
	public class RichEditIllustrationsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertPictureItem());
			items.Add(new InsertFloatingPictureItem());
		}
	}
	#endregion
	#region InsertPictureItem
	public class InsertPictureItem: RichEditCommandBarButtonItem {
		public InsertPictureItem() {
		}
		public InsertPictureItem(BarManager manager)
			: base(manager) {
		}
		public InsertPictureItem(string caption)
			: base(caption) {
		}
		public InsertPictureItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertPicture; } }
	}
	#endregion
	#region InsertFloatingPictureItem
	public class InsertFloatingPictureItem: RichEditCommandBarButtonItem {
		public InsertFloatingPictureItem() {
		}
		public InsertFloatingPictureItem(BarManager manager)
			: base(manager) {
		}
		public InsertFloatingPictureItem(string caption)
			: base(caption) {
		}
		public InsertFloatingPictureItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertFloatingPicture; } }
	}
	#endregion
	#region RichEditTextItemBuilder
	public class RichEditTextItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertTextBoxItem());
		}
	}
	#endregion
	#region InsertTextBoxItem
	public class InsertTextBoxItem: RichEditCommandBarButtonItem {
		public InsertTextBoxItem() {
		}
		public InsertTextBoxItem(BarManager manager)
			: base(manager) {
		}
		public InsertTextBoxItem(string caption)
			: base(caption) {
		}
		public InsertTextBoxItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTextBox; } }
	}
	#endregion
	#region RichEditTablesItemBuilder
	public class RichEditTablesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertTableItem());
		}
	}
	#endregion
	#region InsertTableItem
	public class InsertTableItem: RichEditCommandBarButtonItem {
		class RichEditTableSizePopupControlContainer : SizeChooserPopupControlContainer {
			protected override int DefaultMaxPageRows { get { return 8; } }
			protected override int DefaultMaxPageColumns { get { return 10; } }
			protected override int DefaultPageColumns { get { return 10; } }
			protected override int DefaultPageRows { get { return 8; } }
			protected override int InnerMargin { get { return 3; } }
			protected override System.Drawing.Bitmap LoadItemBitmap() {
				return ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraRichEdit.Images.SizeItem.png", Assembly.GetExecutingAssembly());
			}
		}
		RichEditTableSizePopupControlContainer container;
		public InsertTableItem() {
		}
		public InsertTableItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableItem(string caption)
			: base(caption) {
		}
		public InsertTableItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTable; } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarButtonStyle ButtonStyle { get { return BarButtonStyle.DropDown; } set { } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override PopupControl DropDownControl { get { return container; } set { } }
		#endregion
		protected override void Initialize() {
			base.Initialize();
			InitializePopupControl();
		}
		protected virtual void InitializePopupControl() {
			this.container = new RichEditTableSizePopupControlContainer();
			this.container.CloseUp += new EventHandler(OnContainerCloseUp);
		}
		void OnContainerCloseUp(object sender, EventArgs e) {
			if (container.Commited) {
				IInsertTableCommand command = RichEditControl.CreateCommand(CommandId) as IInsertTableCommand;
				if (command != null && command.CanExecute())
					command.InsertTable(container.SelectedRows, container.SelectedColumns);
			}
		}
	}
	#endregion
	#region RichEditSymbolsItemBuilder
	public class RichEditSymbolsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertSymbolItem());
		}
	}
	#endregion
	#region InsertSymbolItem
	public class InsertSymbolItem: RichEditCommandBarButtonItem {
		public InsertSymbolItem() {
		}
		public InsertSymbolItem(BarManager manager)
			: base(manager) {
		}
		public InsertSymbolItem(string caption)
			: base(caption) {
		}
		public InsertSymbolItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowSymbolForm; } }
	}
	#endregion
	#region RichEditLinksItemBuilder
	public class RichEditLinksItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertBookmarkItem());
			items.Add(new InsertHyperlinkItem());
		}
	}
	#endregion
	#region InsertBookmarkItem
	public class InsertBookmarkItem: RichEditCommandBarButtonItem {
		public InsertBookmarkItem() {
		}
		public InsertBookmarkItem(BarManager manager)
			: base(manager) {
		}
		public InsertBookmarkItem(string caption)
			: base(caption) {
		}
		public InsertBookmarkItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowBookmarkForm; } }
	}
	#endregion
	#region InsertHyperlinkItem
	public class InsertHyperlinkItem: RichEditCommandBarButtonItem {
		public InsertHyperlinkItem() {
		}
		public InsertHyperlinkItem(BarManager manager)
			: base(manager) {
		}
		public InsertHyperlinkItem(string caption)
			: base(caption) {
		}
		public InsertHyperlinkItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.ShowHyperlinkForm; } }
	}
	#endregion
	#region RichEditPagesItemBuilder
	public class RichEditPagesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertPageBreakItem2());
		}
	}
	#endregion
	#region InsertPageBreakItem
	public class InsertPageBreakItem: RichEditCommandBarButtonItem {
		public InsertPageBreakItem() {
		}
		public InsertPageBreakItem(BarManager manager)
			: base(manager) {
		}
		public InsertPageBreakItem(string caption)
			: base(caption) {
		}
		public InsertPageBreakItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertPageBreak; } }
	}
	#endregion
	#region InsertPageBreakItem2
	public class InsertPageBreakItem2 : RichEditCommandBarButtonItem {
		public InsertPageBreakItem2() {
		}
		public InsertPageBreakItem2(BarManager manager)
			: base(manager) {
		}
		public InsertPageBreakItem2(string caption)
			: base(caption) {
		}
		public InsertPageBreakItem2(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertPageBreak2; } }
	}
	#endregion
	#region RichEditHeaderFooterItemBuilder
	public class RichEditHeaderFooterItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new EditPageHeaderItem());
			items.Add(new EditPageFooterItem());
			items.Add(new InsertPageNumberItem());
			items.Add(new InsertPageCountItem());
		}
	}
	#endregion
	#region EditPageHeaderItem
	public class EditPageHeaderItem: RichEditCommandBarButtonItem {
		public EditPageHeaderItem() {
		}
		public EditPageHeaderItem(BarManager manager)
			: base(manager) {
		}
		public EditPageHeaderItem(string caption)
			: base(caption) {
		}
		public EditPageHeaderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.EditPageHeader; } }
	}
	#endregion
	#region EditPageFooterItem
	public class EditPageFooterItem: RichEditCommandBarButtonItem {
		public EditPageFooterItem() {
		}
		public EditPageFooterItem(BarManager manager)
			: base(manager) {
		}
		public EditPageFooterItem(string caption)
			: base(caption) {
		}
		public EditPageFooterItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.EditPageFooter; } }
	}
	#endregion
	#region InsertPageNumberItem
	public class InsertPageNumberItem: RichEditCommandBarButtonItem {
		public InsertPageNumberItem() {
		}
		public InsertPageNumberItem(BarManager manager)
			: base(manager) {
		}
		public InsertPageNumberItem(string caption)
			: base(caption) {
		}
		public InsertPageNumberItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertPageNumberField; } }
	}
	#endregion
	#region InsertPageCountItem
	public class InsertPageCountItem: RichEditCommandBarButtonItem {
		public InsertPageCountItem() {
		}
		public InsertPageCountItem(BarManager manager)
			: base(manager) {
		}
		public InsertPageCountItem(string caption)
			: base(caption) {
		}
		public InsertPageCountItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertPageCountField; } }
	}
	#endregion
	#region RichEditIllustrationsBarCreator
	public class RichEditIllustrationsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(IllustrationsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(IllustrationsBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new IllustrationsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditIllustrationsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new IllustrationsRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditTextBarCreator
	public class RichEditTextBarCreator : ControlCommandBarCreator {
		private static readonly Assembly imageResourceAssembly = typeof(IRichEditControl).Assembly;
		private const string imageResourcePrefix = "DevExpress.XtraRichEdit.Images";
		private const string pageGroupImageName = "InsertTextBox";
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TextRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TextBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new TextBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTextItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			Image glyph = CommandResourceImageLoader.LoadSmallImage(imageResourcePrefix, pageGroupImageName, imageResourceAssembly);
			return new TextRibbonPageGroup() { Glyph = glyph };
		}
	}
	#endregion
	#region RichEditTablesBarCreator
	public class RichEditTablesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TablesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TablesBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new TablesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTablesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TablesRibbonPageGroup() { AllowTextClipping = false };
		}
	}
	#endregion
	#region RichEditSymbolsBarCreator
	public class RichEditSymbolsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(SymbolsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(SymbolsBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 3; } }
		public override Bar CreateBar() {
			return new SymbolsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditSymbolsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new SymbolsRibbonPageGroup() { AllowTextClipping = false };
		}
	}
	#endregion
	#region RichEditLinksBarCreator
	public class RichEditLinksBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(LinksRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(LinksBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new LinksBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditLinksItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new LinksRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditPagesBarCreator
	public class RichEditPagesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PagesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PagesBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 5; } }
		public override Bar CreateBar() {
			return new PagesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditPagesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PagesRibbonPageGroup() { AllowTextClipping = false };
		}
	}
	#endregion
	#region RichEditHeaderFooterBarCreator
	public class RichEditHeaderFooterBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(InsertRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(HeaderFooterRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(HeaderFooterBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new HeaderFooterBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditHeaderFooterItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new InsertRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new HeaderFooterRibbonPageGroup();
		}
	}
	#endregion
	#region IllustrationsBar
	public class IllustrationsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public IllustrationsBar() {
		}
		public IllustrationsBar(BarManager manager)
			: base(manager) {
		}
		public IllustrationsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupIllustrations); } }
	}
	#endregion
	#region TextBar
	public class TextBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TextBar() {
		}
		public TextBar(BarManager manager)
			: base(manager) {
		}
		public TextBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupText); } }
	}
	#endregion
	#region TablesBar
	public class TablesBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TablesBar() {
		}
		public TablesBar(BarManager manager)
			: base(manager) {
		}
		public TablesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupTables); } }
	}
	#endregion
	#region SymbolsBar
	public class SymbolsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public SymbolsBar() {
		}
		public SymbolsBar(BarManager manager)
			: base(manager) {
		}
		public SymbolsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupSymbols); } }
	}
	#endregion
	#region LinksBar
	public class LinksBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public LinksBar() {
		}
		public LinksBar(BarManager manager)
			: base(manager) {
		}
		public LinksBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupLinks); } }
	}
	#endregion
	#region PagesBar
	public class PagesBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public PagesBar() {
		}
		public PagesBar(BarManager manager)
			: base(manager) {
		}
		public PagesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupPages); } }
	}
	#endregion
	#region HeaderFooterBar
	public class HeaderFooterBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public HeaderFooterBar() {
		}
		public HeaderFooterBar(BarManager manager)
			: base(manager) {
		}
		public HeaderFooterBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooter); } }
	}
	#endregion
	#region InsertRibbonPage
	public class InsertRibbonPage : ControlCommandBasedRibbonPage {
		public InsertRibbonPage() {
		}
		public InsertRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageInsert); } }
		protected override RibbonPage CreatePage() {
			return new InsertRibbonPage();
		}
	}
	#endregion
	#region IllustrationsRibbonPageGroup
	public class IllustrationsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public IllustrationsRibbonPageGroup() {
		}
		public IllustrationsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupIllustrations); } }
	}
	#endregion
	#region TextRibbonPageGroup
	public class TextRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TextRibbonPageGroup() {
		}
		public TextRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupText); } }
	}
	#endregion
	#region TablesRibbonPageGroup
	public class TablesRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TablesRibbonPageGroup() {
		}
		public TablesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupTables); } }
	}
	#endregion
	#region SymbolsRibbonPageGroup
	public class SymbolsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public SymbolsRibbonPageGroup() {
		}
		public SymbolsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupSymbols); } }
	}
	#endregion
	#region LinksRibbonPageGroup
	public class LinksRibbonPageGroup : RichEditControlRibbonPageGroup {
		public LinksRibbonPageGroup() {
		}
		public LinksRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupLinks); } }
	}
	#endregion
	#region PagesRibbonPageGroup
	public class PagesRibbonPageGroup : RichEditControlRibbonPageGroup {
		public PagesRibbonPageGroup() {
		}
		public PagesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupPages); } }
	}
	#endregion
	#region HeaderFooterRibbonPageGroup
	public class HeaderFooterRibbonPageGroup : RichEditControlRibbonPageGroup {
		public HeaderFooterRibbonPageGroup() {
		}
		public HeaderFooterRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupHeaderFooter); } }
	}
	#endregion
}
