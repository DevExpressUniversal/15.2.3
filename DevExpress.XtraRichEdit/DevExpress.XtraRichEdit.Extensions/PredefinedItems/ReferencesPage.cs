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
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditTableOfContentsItemBuilder
	public class RichEditTableOfContentsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertTableOfContentsItem());
			items.Add(new UpdateTableOfContentsItem());
			AddParagraphsToTableOfContentItem tocItem = new AddParagraphsToTableOfContentItem();
			items.Add(tocItem);
			IBarSubItem tocSubItem = tocItem;
			for (int i = 0; i <= 9; i++) {
				SetParagraphHeadingLevelItem levelItem = new SetParagraphHeadingLevelItem();
				levelItem.OutlineLevel = i;
				tocSubItem.AddBarItem(levelItem);
			}
		}
	}
	#endregion
	#region AddParagraphsToTableOfContentItem
	public class AddParagraphsToTableOfContentItem : RichEditCommandBarSubItem {
		public AddParagraphsToTableOfContentItem() {
		}
		public AddParagraphsToTableOfContentItem(BarManager manager)
			: base(manager) {
		}
		public AddParagraphsToTableOfContentItem(string caption)
			: base(caption) {
		}
		public AddParagraphsToTableOfContentItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.AddParagraphsToTableOfContents; } }
	}
	#endregion
	#region InsertTableOfContentsItem
	public class InsertTableOfContentsItem: RichEditCommandBarButtonItem {
		public InsertTableOfContentsItem() {
		}
		public InsertTableOfContentsItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableOfContentsItem(string caption)
			: base(caption) {
		}
		public InsertTableOfContentsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableOfContents; } }
	}
	#endregion
	#region UpdateTableOfContentsItem
	public class UpdateTableOfContentsItem: RichEditCommandBarButtonItem {
		public UpdateTableOfContentsItem() {
		}
		public UpdateTableOfContentsItem(BarManager manager)
			: base(manager) {
		}
		public UpdateTableOfContentsItem(string caption)
			: base(caption) {
		}
		public UpdateTableOfContentsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.UpdateTableOfContents; } }
	}
	#endregion
	#region UpdateTableOfFiguresItem
	public class UpdateTableOfFiguresItem : RichEditCommandBarButtonItem {
		public UpdateTableOfFiguresItem() {
		}
		public UpdateTableOfFiguresItem(BarManager manager)
			: base(manager) {
		}
		public UpdateTableOfFiguresItem(string caption)
			: base(caption) {
		}
		public UpdateTableOfFiguresItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.UpdateTableOfFigures; } }
	}
	#endregion
	#region SetParagraphHeadingLevelItem
	public class SetParagraphHeadingLevelItem: RichEditCommandBarButtonItem {
		int outlineLevel;
		public SetParagraphHeadingLevelItem() {
		}
		public SetParagraphHeadingLevelItem(BarManager manager)
			: base(manager) {
		}
		public SetParagraphHeadingLevelItem(string caption)
			: base(caption) {
		}
		public SetParagraphHeadingLevelItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public int OutlineLevel { get { return outlineLevel; } set { outlineLevel = Math.Max(0, Math.Min(9, value)); } }
		protected override RichEditCommandId CommandId {
			get {
				IConvertToInt<RichEditCommandId> value = RichEditCommandId.SetParagraphBodyTextLevel;
				return new RichEditCommandId(value.ToInt() + outlineLevel);
			}
		}
	}
	#endregion
	#region RichEditCaptionsItemBuilder
	public class RichEditCaptionsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			InsertCaptionPlaceholderItem secItem = new InsertCaptionPlaceholderItem();
			items.Add(secItem);
			InsertFiguresCaptionItems secFiguresItem = new InsertFiguresCaptionItems();
			InsertTablesCaptionItems secTablesItem = new InsertTablesCaptionItems();
			InsertEquationsCaptionItems secEquationsItem = new InsertEquationsCaptionItems();
			IBarSubItem secSubItem = secItem;
			secSubItem.AddBarItem(secFiguresItem);
			secSubItem.AddBarItem(secTablesItem);
			secSubItem.AddBarItem(secEquationsItem);
			InsertTableOfFiguresPlaceholderItem tocItem = new InsertTableOfFiguresPlaceholderItem();
			items.Add(tocItem);
			InsertTableOfFiguresItems tocFiguresItem = new InsertTableOfFiguresItems();
			InsertTableOfTablesItems tocTablesItem = new InsertTableOfTablesItems();
			InsertTableOfEquationsItems tocEquationsItem = new InsertTableOfEquationsItems();
			IBarSubItem tocSubItem = tocItem;
			tocSubItem.AddBarItem(tocFiguresItem);
			tocSubItem.AddBarItem(tocTablesItem);
			tocSubItem.AddBarItem(tocEquationsItem);
			items.Add(new UpdateTableOfFiguresItem());
		}
	}
	#endregion
	#region InsertTableOfFiguresPlaceholderItem
	public class InsertTableOfFiguresPlaceholderItem : RichEditCommandBarSubItem {
		public InsertTableOfFiguresPlaceholderItem() {
		}
		public InsertTableOfFiguresPlaceholderItem(BarManager manager)
			: base(manager) {
		}
		public InsertTableOfFiguresPlaceholderItem(string caption)
			: base(caption) {
		}
		public InsertTableOfFiguresPlaceholderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableOfFiguresPlaceholder; } }
	}
	#endregion
	#region InsertTableOfEquationsItems
	public class InsertTableOfEquationsItems: RichEditCommandBarButtonItem {
		public InsertTableOfEquationsItems() {
		}
		public InsertTableOfEquationsItems(BarManager manager)
			: base(manager) {
		}
		public InsertTableOfEquationsItems(string caption)
			: base(caption) {
		}
		public InsertTableOfEquationsItems(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableOfEquations; } }
	}
	#endregion
	#region InsertTableOfFiguresItems
	public class InsertTableOfFiguresItems: RichEditCommandBarButtonItem {
		public InsertTableOfFiguresItems() {
		}
		public InsertTableOfFiguresItems(BarManager manager)
			: base(manager) {
		}
		public InsertTableOfFiguresItems(string caption)
			: base(caption) {
		}
		public InsertTableOfFiguresItems(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableOfFigures; } }
	}
	#endregion
	#region InsertTableOfTablesItems
	public class InsertTableOfTablesItems: RichEditCommandBarButtonItem {
		public InsertTableOfTablesItems() {
		}
		public InsertTableOfTablesItems(BarManager manager)
			: base(manager) {
		}
		public InsertTableOfTablesItems(string caption)
			: base(caption) {
		}
		public InsertTableOfTablesItems(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTableOfTables; } }
	}
	#endregion
	#region InsertCaptionPlaceholderItem
	public class InsertCaptionPlaceholderItem : RichEditCommandBarSubItem {
		public InsertCaptionPlaceholderItem() {
		}
		public InsertCaptionPlaceholderItem(BarManager manager)
			: base(manager) {
		}
		public InsertCaptionPlaceholderItem(string caption)
			: base(caption) {
		}
		public InsertCaptionPlaceholderItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertCaptionPlaceholder; } }
	}
		#endregion
	#region InsertEquationsCaptionItems
	public class InsertEquationsCaptionItems: RichEditCommandBarButtonItem {
		public InsertEquationsCaptionItems() {
		}
		public InsertEquationsCaptionItems(BarManager manager)
			: base(manager) {
		}
		public InsertEquationsCaptionItems(string caption)
			: base(caption) {
		}
		public InsertEquationsCaptionItems(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertEquationsCaption; } }
	}
	#endregion
	#region InsertFiguresCaptionItems
	public class InsertFiguresCaptionItems: RichEditCommandBarButtonItem {
		public InsertFiguresCaptionItems() {
		}
		public InsertFiguresCaptionItems(BarManager manager)
			: base(manager) {
		}
		public InsertFiguresCaptionItems(string caption)
			: base(caption) {
		}
		public InsertFiguresCaptionItems(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertFiguresCaption; } }
	}
	#endregion
	#region InsertTablesCaptionItems
	public class InsertTablesCaptionItems: RichEditCommandBarButtonItem {
		public InsertTablesCaptionItems() {
		}
		public InsertTablesCaptionItems(BarManager manager)
			: base(manager) {
		}
		public InsertTablesCaptionItems(string caption)
			: base(caption) {
		}
		public InsertTablesCaptionItems(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.InsertTablesCaption; } }
	}
	#endregion
	#region RichEditTableOfContentsBarCreator
	public class RichEditTableOfContentsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReferencesRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(TableOfContentsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TableOfContentsBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 6; } }
		public override Bar CreateBar() {
			return new TableOfContentsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditTableOfContentsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReferencesRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new TableOfContentsRibbonPageGroup();
		}
	}
	#endregion
	#region RichEditCaptionsBarCreator
	public class RichEditCaptionsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReferencesRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(CaptionsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CaptionsBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 7; } }
		public override Bar CreateBar() {
			return new CaptionsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditCaptionsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReferencesRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CaptionsRibbonPageGroup();
		}
	}
	#endregion
	#region TableOfContentsBar
	public class TableOfContentsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public TableOfContentsBar()
			: base() {
		}
		public TableOfContentsBar(BarManager manager)
			: base(manager) {
		}
		public TableOfContentsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupTableOfContents); } }
	}
	#endregion
	#region CaptionsBar
	public class CaptionsBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public CaptionsBar()
			: base() {
		}
		public CaptionsBar(BarManager manager)
			: base(manager) {
		}
		public CaptionsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupCaptions); } }
	}
	#endregion
	#region ReferencesRibbonPage
	public class ReferencesRibbonPage : ControlCommandBasedRibbonPage {
		public ReferencesRibbonPage() {
		}
		public ReferencesRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageReferences); } }
		protected override RibbonPage CreatePage() {
			return new ReferencesRibbonPage();
		}
	}
	#endregion
	#region TableOfContentsRibbonPageGroup
	public class TableOfContentsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public TableOfContentsRibbonPageGroup() {
		}
		public TableOfContentsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupTableOfContents); } }
	}
	#endregion
	#region CaptionsRibbonPageGroup
	public class CaptionsRibbonPageGroup : RichEditControlRibbonPageGroup {
		public CaptionsRibbonPageGroup() {
		}
		public CaptionsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupCaptions); } }
	}
	#endregion
}
