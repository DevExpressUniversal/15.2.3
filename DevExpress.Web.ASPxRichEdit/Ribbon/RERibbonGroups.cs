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
using System.Linq;
using System.Text;
using DevExpress.Office.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERFileCommonGroup : RERGroup {
		public RERFileCommonGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupCommon)) { }
		public RERFileCommonGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Common")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Save; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERUndoGroup : RERGroup {
		public RERUndoGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupUndo)) { }
		public RERUndoGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Undo")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Undo; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERClipboardGroup : RERGroup {
		 public RERClipboardGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupClipboard)) { }
		 public RERClipboardGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Clipboard")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Paste; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERFontGroup : RERGroup {
		public RERFontGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupFont)) { }
		public RERFontGroup(string text)
			: base(text, true) { }
		[ DefaultValue(true)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Font")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Font; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowFontForm; } }
	}
	public class RERParagraphGroup : RERGroup {
		public RERParagraphGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupParagraph)) { }
		public RERParagraphGroup(string text)
			: base(text, true) { }
		[ DefaultValue(true)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Paragraph")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Paragraph; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowParagraphForm; } }
	}
	public class RERStylesGroup : RERGroup {
		public RERStylesGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupStyles)) { }
		public RERStylesGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Styles")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.ChangeFontStyle; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class REREditingGroup : RERGroup {
		public REREditingGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupEditing)) { }
		public REREditingGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Editing")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Find; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERPagesGroup : RERGroup {
		public RERPagesGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupPages)) { }
		public RERPagesGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Pages")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.InsertPageBreak; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERTablesGroup : RERGroup {
		public RERTablesGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTables)) { }
		public RERTablesGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Tables")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.InsertTable; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERIllustrationsGroup : RERGroup {
		public RERIllustrationsGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupIllustrations)) { }
		public RERIllustrationsGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Illustrations")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.InsertImage; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERLinksGroup : RERGroup {
		public RERLinksGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupLinks)) { }
		public RERLinksGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Links")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Hyperlink; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERHeaderAndFooterGroup : RERGroup {
		public RERHeaderAndFooterGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupHeaderFooter)) { }
		public RERHeaderAndFooterGroup(string text)
			: base(text) { }
		protected override string DefaultImage { get { return RichEditRibbonImages.Header; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERSymbolsGroup : RERGroup {
		public RERSymbolsGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupSymbols)) { }
		public RERSymbolsGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Symbols")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.Symbol; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERPageSetupGroup : RERGroup {
		public RERPageSetupGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupPageSetup)) { }
		public RERPageSetupGroup(string text)
			: base(text, true) { }
		[ DefaultValue(true)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Page Setup")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.PaperSize; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowPageSetupForm; } }
	}
	public class RERBackgroundGroup : RERGroup {
		public RERBackgroundGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupPageBackground)) { }
		public RERBackgroundGroup(string text)
			: base(text, false) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Background")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.PageColor; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERInsertFieldsGroup : RERGroup {
		public RERInsertFieldsGroup()
			: this("Insert Fields") { } 
		public RERInsertFieldsGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Insert Fields")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.InsertDataField; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERMailMergeViewGroup : RERGroup {
		public RERMailMergeViewGroup()
			: this("View") { } 
		public RERMailMergeViewGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("View")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.ToggleFieldCodes; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERCurrentRecordGroup : RERGroup {
		public RERCurrentRecordGroup()
			: this("Current Record") { } 
		public RERCurrentRecordGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Current Record")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.UpdateField; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERFinishMailMergeGroup : RERGroup {
		public RERFinishMailMergeGroup()
			: this("Finish") { } 
		public RERFinishMailMergeGroup(string text)
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Finish")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.MailMerge; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERShowGroup : RERGroup {
		public RERShowGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupShow)) { }
		public RERShowGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("Show")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.RulerHorizontal; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERViewGroup : RERGroup {
		public RERViewGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupView)) { }
		public RERViewGroup(string text) 
			: base(text) { }
		[ DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[ DefaultValue("View")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.FullScreen; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERTableStyleOptionsGroup : RERGroup {
		public RERTableStyleOptionsGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsDesignTableStyleOptions)) { }
		public RERTableStyleOptionsGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Table Style Options")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.TableProperties; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERTableStylesGroup : RERGroup {
		public RERTableStylesGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsDesignTableStyles)) { }
		public RERTableStylesGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Table Styles")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.ModifyTableStyle; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERBordersAndShadingsGroup : RERGroup {
		public RERBordersAndShadingsGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsDesignBordersAndShadings)) { }
		public RERBordersAndShadingsGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Border & Shadings")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.BordersOutside; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERTableGroup : RERGroup {
		public RERTableGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsLayoutTable)) { }
		public RERTableGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Table")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.TableProperties; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERRowAndColumnsGroup : RERGroup {
		public RERRowAndColumnsGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsLayoutRowsAndColumns)) { }
		public RERRowAndColumnsGroup(string text)
			: base(text) {
			this.ShowDialogBoxLauncher = true;
		}
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Rows & Columns")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.InsertTableRowsBelow; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowInsertTableCellsForm; } }
	}
	public class RERMergeGroup : RERGroup {
		public RERMergeGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsLayoutMerge)) { }
		public RERMergeGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Merge")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.MergeTableCells; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERCellSizeGroup : RERGroup {
		public RERCellSizeGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsLayoutCellSize)) { }
		public RERCellSizeGroup(string text)
			: base(text) {
			this.ShowDialogBoxLauncher = true;
		}
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Cell Size")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.TableAutoFitContents; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowTablePropertiesForm; } }
	}
	public class RERAlignmentGroup : RERGroup {
		public RERAlignmentGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupTableToolsLayoutAlignment)) { }
		public RERAlignmentGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Alignment")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.AlignMiddleCenter; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERHeaderFooterNavigationGroup : RERGroup {
		public RERHeaderFooterNavigationGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupHeaderFooterToolsDesignNavigation)) { }
		public RERHeaderFooterNavigationGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Navigation")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.GoToHeader; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERHeaderFooterOptionsGroup : RERGroup {
		public RERHeaderFooterOptionsGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupHeaderFooterToolsDesignOptions)) { }
		public RERHeaderFooterOptionsGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Options")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.DifferentFirstPage; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
	public class RERHeaderFooterCloseGroup : RERGroup {
		public RERHeaderFooterCloseGroup()
			: this(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.GroupHeaderFooterToolsDesignClose)) { }
		public RERHeaderFooterCloseGroup(string text)
			: base(text) { }
		[DefaultValue(false)]
		public new bool ShowDialogBoxLauncher {
			get { return base.ShowDialogBoxLauncher; }
			set { base.ShowDialogBoxLauncher = value; }
		}
		[DefaultValue("Close")]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultImage { get { return RichEditRibbonImages.CloseHeaderAndFooter; } }
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
	}
}
