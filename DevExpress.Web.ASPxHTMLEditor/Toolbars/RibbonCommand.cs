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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	using DevExpress.Web.Design;
	using DevExpress.Web.Office.Internal;
	public class HEHomeRibbonTab : HERibbonTabBase {
		public HEHomeRibbonTab() 
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonTab_Home)) { }
		public HEHomeRibbonTab(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEHomeRibbonTabText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonTab_Home)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonTab_Home; }
		}
	}
	public class HEInsertRibbonTab : HERibbonTabBase {
		public HEInsertRibbonTab()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonTab_Insert)) { }
		public HEInsertRibbonTab(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEInsertRibbonTabText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonTab_Insert)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonTab_Insert; }
		}
	}
	public class HEViewRibbonTab : HERibbonTabBase {
		public HEViewRibbonTab()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonTab_View)) { }
		public HEViewRibbonTab(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEViewRibbonTabText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonTab_View)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonTab_View; }
		}
	}
	public class HETableRibbonTab : HERibbonTabBase {
		public HETableRibbonTab()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonTab_Table)) { }
		public HETableRibbonTab(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HETableRibbonTabText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonTab_Table)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonTab_Table; }
		}
	}
	public class HETableLayoutRibbonTab : HERibbonTabBase {
		public HETableLayoutRibbonTab()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonTabCategory_Layout)) { }
		public HETableLayoutRibbonTab(string text) {
			Text = text;
		}
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonTabCategory_Layout; }
		}
	}
	public class HEReviewRibbonTab : HERibbonTabBase {
		public HEReviewRibbonTab()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonTab_Review)) { }
		public HEReviewRibbonTab(string text) {
			Text = text;
		}
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonTab_Review; }
		}
	}
	public class HETableToolsRibbonTabCategory : RibbonContextTabCategory {
		const string defaultName = "hetabletools";
		const string defaultColor = "#d31313"; 
		public HETableToolsRibbonTabCategory()
			: base(defaultName, System.Drawing.Color.FromName(defaultColor)) { }
		public HETableToolsRibbonTabCategory(System.Drawing.Color color) {
			Color = color;
			Name = defaultName;
		}
		[Browsable(false), DefaultValue(defaultName)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
	}
	public class HEUndoRibbonGroup : HERibbonGroupBase {
		public HEUndoRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Undo)) { }
		public HEUndoRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEUndoRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Undo)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Undo; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.UndoLarge; }
		}
	}
	public class HEClipboardRibbonGroup : HERibbonGroupBase {
		public HEClipboardRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Clipboard)) { }
		public HEClipboardRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEClipboardRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Clipboard)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Clipboard; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.PasteLarge; }
		}
	}
	public class HEFontRibbonGroup : HERibbonGroupBase {
		public HEFontRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Font)) { }
		public HEFontRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEFontRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Font)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Font); }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.FontGroup; }
		}
	}
	public class HEParagraphRibbonGroup : HERibbonGroupBase {
		public HEParagraphRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Paragraph)) { }
		public HEParagraphRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEParagraphRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Paragraph)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Paragraph; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.CenterLarge; }
		}
	}
	public class HEImagesRibbonGroup : HERibbonGroupBase {
		public HEImagesRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Images)) { }
		public HEImagesRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEImagesRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Images)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Images; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.InsertImageDialogLarge; }
		}
	}
	public class HELinksRibbonGroup : HERibbonGroupBase {
		public HELinksRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Links)) { }
		public HELinksRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HELinksRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Links)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Links; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.InsertLinkDialogLarge; }
		}
	}
	public class HEViewsRibbonGroup : HERibbonGroupBase {
		public HEViewsRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Views)) { }
		public HEViewsRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEViewsRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Views)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Views; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.ViewsGroup; }
		}
	}
	public class HETablesRibbonGroup : HERibbonGroupBase {
		public HETablesRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Tables)) { }
		public HETablesRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HETablesRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Tables)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Tables; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.InsertTableDialogLarge; }
		}
	}
	public class HEDeleteTableRibbonGroup : HERibbonGroupBase {
		public HEDeleteTableRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_DeleteTable)) { }
		public HEDeleteTableRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEDeleteTableRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_DeleteTable)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_DeleteTable; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.DeleteTableLarge; }
		}
	}
	public class HEInsertTableRibbonGroup : HERibbonGroupBase {
		public HEInsertTableRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_InsertTable)) { }
		public HEInsertTableRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEInsertTableRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_InsertTable)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_InsertTable; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.InsertTableRowAboveLarge; }
		}
	}
	public class HEMergeTableRibbonGroup : HERibbonGroupBase {
		public HEMergeTableRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_MergeTable)) { }
		public HEMergeTableRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HEMergeTableRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_MergeTable)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_MergeTable; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.MergeTableCellDownLarge; }
		}
	}
	public class HETablePropertiesRibbonGroup : HERibbonGroupBase {
		public HETablePropertiesRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_TableProperties)) { }
		public HETablePropertiesRibbonGroup(string text) {
			Text = text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HETablePropertiesRibbonGroupText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RibbonGroup_TableProperties)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_TableProperties; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.TablePropertiesDialogLarge; }
		}
	}
	public class HESpellingRibbonGroup : HERibbonGroupBase {
		public HESpellingRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Spelling)) { }
		public HESpellingRibbonGroup(string text) {
			Text = text;
		}
		[DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Spelling)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Spelling; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.CheckSpellingLarge; }
		}
	}
	public class HEMediaRibbonGroup : HERibbonGroupBase {
		public HEMediaRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Media)) { }
		public HEMediaRibbonGroup(string text) {
			Text = text;
		}
		[DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Media)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Media; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.InsertFlashDialogLarge; }
		}
	}
	public class HEEditingRibbonGroup : HERibbonGroupBase {
		public HEEditingRibbonGroup()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonGroup_Editing)) { }
		public HEEditingRibbonGroup(string text) {
			Text = text;
		}
		[DefaultValue(StringResources.HtmlEditorText_RibbonGroup_Editing)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return StringResources.HtmlEditorText_RibbonGroup_Editing; }
		}
		protected override string ResourceImageName {
			get { return HERibbonImages.FindAndReplaceDialogLarge; }
		}
	}
	public class HEPasteSelectionRibbonCommand : HERibbonCommandBase {
		public HEPasteSelectionRibbonCommand() 
			: base() { }
		public HEPasteSelectionRibbonCommand(string text)
			: base(text) { }
		public HEPasteSelectionRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEPasteSelectionRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "paste"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPaste); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Paste; }
		}
	}
	public class HEPasteFromWordRibbonCommand : HERibbonCommandBase {
		public HEPasteFromWordRibbonCommand() 
			: base() { }
		public HEPasteFromWordRibbonCommand(string text)
			: base(text) { }
		public HEPasteFromWordRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEPasteFromWordRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "pastefromworddialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteRtf); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.PasteFromWord; }
		}
	}
	public class HECopySelectionRibbonCommand : HERibbonCommandBase {
		public HECopySelectionRibbonCommand() 
			: base() { }
		public HECopySelectionRibbonCommand(string text)
			: base(text) { }
		public HECopySelectionRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HECopySelectionRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "copy"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCopy); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Copy; }
		}
	}
	public class HECutSelectionRibbonCommand : HERibbonCommandBase {
		public HECutSelectionRibbonCommand() 
			: base() { }
		public HECutSelectionRibbonCommand(string text)
			: base(text) { }
		public HECutSelectionRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HECutSelectionRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "cut"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCut); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Cut; }
		}
	}
	public class HEBoldRibbonCommand : HERibbonToggleCommandBase {
		public HEBoldRibbonCommand() 
			: base() { }
		public HEBoldRibbonCommand(string text)
			: base(text) { }
		public HEBoldRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEBoldRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "bold"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBold); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Bold; }
		}
		protected override bool ShowText {
			get { return false; }
		}
	}
	public class HEItalicRibbonCommand : HERibbonToggleCommandBase {
		public HEItalicRibbonCommand() 
			: base() { }
		public HEItalicRibbonCommand(string text)
			: base(text) { }
		public HEItalicRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEItalicRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "italic"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandItalic); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Italic; }
		}
		protected override bool ShowText {
			get { return false; }
		}
	}
	public class HEStrikeoutRibbonCommand : HERibbonToggleCommandBase {
		public HEStrikeoutRibbonCommand() 
			: base() { }
		public HEStrikeoutRibbonCommand(string text)
			: base(text) { }
		public HEStrikeoutRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEStrikeoutRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "strikethrough"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandStrikethrough); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Strikethrough; }
		}
		protected override bool ShowText {
			get { return false; }
		}
	}
	public class HEUnderlineRibbonCommand : HERibbonToggleCommandBase {
		public HEUnderlineRibbonCommand() 
			: base() { }
		public HEUnderlineRibbonCommand(string text)
			: base(text) { }
		public HEUnderlineRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEUnderlineRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "underline"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUnderline); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Underline; }
		}
		protected override bool ShowText {
			get { return false; }
		}
	}
	public class HERemoveFormatRibbonCommand : HERibbonCommandBase {
		public HERemoveFormatRibbonCommand() 
			: base() { }
		public HERemoveFormatRibbonCommand(string text)
			: base(text) { }
		public HERemoveFormatRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HERemoveFormatRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "removeformat"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRemoveFormat); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.RemoveFormat; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "fontGroup"; }
		}
	}
	public class HESuperscriptRibbonCommand : HERibbonToggleCommandBase {
		public HESuperscriptRibbonCommand() 
			: base() { }
		public HESuperscriptRibbonCommand(string text)
			: base(text) { }
		public HESuperscriptRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HESuperscriptRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "superscript"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandSuperscript); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Superscript; }
		}
		protected override bool ShowText {
			get { return false; }
		}
	}
	public class HESubscriptRibbonCommand : HERibbonToggleCommandBase {
		public HESubscriptRibbonCommand() 
			: base() { }
		public HESubscriptRibbonCommand(string text)
			: base(text) { }
		public HESubscriptRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HESubscriptRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "subscript"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandSubscript); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Subscript; }
		}
		protected override bool ShowText {
			get { return false; }
		}
	}
	public class HEFontColorRibbonCommand : HERibbonColorCommandBase {
		public HEFontColorRibbonCommand() 
			: base() { }
		public HEFontColorRibbonCommand(string text)
			: base(text) { }
		public HEFontColorRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEFontColorRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "forecolor"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandForeColor); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.ForeColor; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "fontGroup"; }
		}
	}
	public class HEBackColorRibbonCommand : HERibbonColorCommandBase {
		public HEBackColorRibbonCommand() 
			: base() { }
		public HEBackColorRibbonCommand(string text)
			: base(text) { }
		public HEBackColorRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEBackColorRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "backcolor"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBackColor); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.BackColor; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "fontGroup"; }
		}
	}
	public class HEFontNameRibbonCommand : HERibbonComboBoxCommandBase {
		public HEFontNameRibbonCommand() 
			: base() { }
		public HEFontNameRibbonCommand(string text)
			: base(text) { }
		protected override string CommandID {
			get { return "fontname"; }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontName); }
		}
		protected override ListEditItemCollection DefaultItems {
			get { return GetItems(); }
		}
		protected override int DefaultWidth {
			get { return 160; }
		}
		protected override string DefaultGroupName {
			get { return "fontGroup"; }
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection fontCollection = new ListEditItemCollection();
			fontCollection.Add("Times New Roman", "Times New Roman");
			fontCollection.Add("Tahoma", "Tahoma");
			fontCollection.Add("Verdana", "Verdana");
			fontCollection.Add("Arial", "Arial");
			fontCollection.Add("MS Sans Serif", "MS Sans Serif");
			fontCollection.Add("Courier", "Courier");
			fontCollection.Add("Segoe UI", "Segoe UI");
			return fontCollection;
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ListEditItem item in Items)
				result.Add(item.Index, string.Format("<span style=\"font-family: {0};\">{1}</span>", item.Value, string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text));
			return result;
		}
	}
	public class HEFontSizeRibbonCommand : HERibbonComboBoxCommandBase {
		public HEFontSizeRibbonCommand() 
			: base() { }
		public HEFontSizeRibbonCommand(string text)
			: base(text) { }
		protected override string CommandID {
			get { return "fontsize"; }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontSize); }
		}
		protected override ListEditItemCollection DefaultItems {
			get { return GetItems(); }
		}
		protected override int DefaultWidth {
			get { return 100; }
		}
		protected override string DefaultGroupName {
			get { return "fontGroup"; }
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection fontCollection = new ListEditItemCollection();
			fontCollection.Add("1 (8pt)", "1");
			fontCollection.Add("2 (10pt)", "2");
			fontCollection.Add("3 (12pt)", "3");
			fontCollection.Add("4 (14pt)", "4");
			fontCollection.Add("5 (18pt)", "5");
			fontCollection.Add("6 (24pt)", "6");
			fontCollection.Add("7 (36pt)", "7");
			return fontCollection;
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			Dictionary<int, string> sizeRatio = ToolbarFontSizeComboBoxControl.GetSizeRatio();
			foreach(ListEditItem item in Items) {
				var itemValue = Convert.ToInt32(item.Value);
				if(!this.Collection.Owner.IsDesignMode() && !sizeRatio.ContainsKey(itemValue))
					throw new Exception("Wrong font size value. Only numbers 1-7 are allowed.");
				string sizeValue = sizeRatio.ContainsKey(itemValue) ? sizeRatio[itemValue] : string.Empty;
				result.Add(item.Index, string.Format("<span style=\"font-size: {0};\">{1}</span>", sizeValue, string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text));
			}
			return result;
		}
	}
	public class HEAlignmentLeftRibbonCommand : HERibbonToggleCommandBase {
		public HEAlignmentLeftRibbonCommand() 
			: base() { }
		public HEAlignmentLeftRibbonCommand(string text)
			: base(text) { }
		public HEAlignmentLeftRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEAlignmentLeftRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "justifyleft"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandLeft); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Left; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "justifyGroup"; }
		}
	}
	public class HEAlignmentCenterRibbonCommand : HERibbonToggleCommandBase {
		public HEAlignmentCenterRibbonCommand() 
			: base() { }
		public HEAlignmentCenterRibbonCommand(string text)
			: base(text) { }
		public HEAlignmentCenterRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEAlignmentCenterRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "justifycenter"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCenter); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Center; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "justifyGroup"; }
		}
	}
	public class HEAlignmentRightRibbonCommand : HERibbonToggleCommandBase {
		public HEAlignmentRightRibbonCommand() 
			: base() { }
		public HEAlignmentRightRibbonCommand(string text)
			: base(text) { }
		public HEAlignmentRightRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEAlignmentRightRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "justifyright"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRight); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Right; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "justifyGroup"; }
		}
	}
	public class HEAlignmentJustifyRibbonCommand : HERibbonToggleCommandBase {
		public HEAlignmentJustifyRibbonCommand() 
			: base() { }
		public HEAlignmentJustifyRibbonCommand(string text)
			: base(text) { }
		public HEAlignmentJustifyRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEAlignmentJustifyRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "justifyfull"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandJustify); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Justify; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "justifyGroup"; }
		}
	}
	public class HEOutdentRibbonCommand : HERibbonCommandBase {
		public HEOutdentRibbonCommand() 
			: base() { }
		public HEOutdentRibbonCommand(string text)
			: base(text) { }
		public HEOutdentRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEOutdentRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "outdent"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOutdent); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Outdent; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "indentGroup"; }
		}
	}
	public class HEIndentRibbonCommand : HERibbonCommandBase {
		public HEIndentRibbonCommand() 
			: base() { }
		public HEIndentRibbonCommand(string text)
			: base(text) { }
		public HEIndentRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEIndentRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "indent"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandIndent); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Indent; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "indentGroup"; }
		}
	}
	public class HEInsertOrderedListRibbonCommand : HERibbonCommandBase {
		public HEInsertOrderedListRibbonCommand() 
			: base() { }
		public HEInsertOrderedListRibbonCommand(string text)
			: base(text) { }
		public HEInsertOrderedListRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertOrderedListRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertorderedlist"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOrderedList); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.OrderedList; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "listGroup"; }
		}
	}
	public class HEInsertUnorderedListRibbonCommand : HERibbonCommandBase {
		public HEInsertUnorderedListRibbonCommand() 
			: base() { }
		public HEInsertUnorderedListRibbonCommand(string text)
			: base(text) { }
		public HEInsertUnorderedListRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertUnorderedListRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertunorderedlist"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBulletList); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.UnorderedList; }
		}
		protected override bool ShowText {
			get { return false; }
		}
		protected override string DefaultGroupName {
			get { return "listGroup"; }
		}
	}
	public class HEUndoRibbonCommand : HERibbonCommandBase {
		public HEUndoRibbonCommand() 
			: base() { }
		public HEUndoRibbonCommand(string text)
			: base(text) { }
		public HEUndoRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEUndoRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "undo"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUndo); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Undo; }
		}
	}
	public class HERedoRibbonCommand : HERibbonCommandBase {
		 public HERedoRibbonCommand() 
			: base() { }
		public HERedoRibbonCommand(string text)
			: base(text) { }
		public HERedoRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HERedoRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "redo"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRedo); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Redo; }
		}
	}
	public class HEInsertTableDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEInsertTableDropDownRibbonCommand()
			: base() { }
		public HEInsertTableDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		protected override string CommandID {
			get { return "inserttabledialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_Table); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableDialog; }
		}
		protected override void FillItems() {
			Items.Add(new HEInsertTableByGridHighlightingRibbonCommand());
		}
	}
	public class HEInsertTableByGridHighlightingRibbonCommand : HERibbonDropDownCommandBase {
		public HEInsertTableByGridHighlightingRibbonCommand()
			: base() {
			Template = new InsertTableTemplate(this, GetName());
		}
		protected override string CommandID {
			get { return "inserttable"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertTable); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableDialog; }
		}
	}
	public class HEInsertTableRibbonCommand: HERibbonCommandBase {
		public HEInsertTableRibbonCommand() 
			: base() { }
		public HEInsertTableRibbonCommand(string text)
			: base(text) { }
		public HEInsertTableRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertTableRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "inserttabledialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertTable); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableDialog; }
		}
	}
	public class HETablePropertiesRibbonCommand : HERibbonCommandBase {
		public HETablePropertiesRibbonCommand() 
			: base() { }
		public HETablePropertiesRibbonCommand(string text)
			: base(text) { }
		public HETablePropertiesRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HETablePropertiesRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "tablepropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_TableProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TablePropertiesDialog; }
		}
	}
	public class HETableRowPropertiesRibbonCommand : HERibbonCommandBase {
		public HETableRowPropertiesRibbonCommand() 
			: base() { }
		public HETableRowPropertiesRibbonCommand(string text)
			: base(text) { }
		public HETableRowPropertiesRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HETableRowPropertiesRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "tablerowpropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_RowProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TableRowPropertiesDialog; }
		}
	}
	public class HETableColumnPropertiesRibbonCommand : HERibbonCommandBase {
		public HETableColumnPropertiesRibbonCommand() 
			: base() { }
		public HETableColumnPropertiesRibbonCommand(string text)
			: base(text) { }
		public HETableColumnPropertiesRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HETableColumnPropertiesRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "tablecolumnpropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_ColumnProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TableColumnPropertiesDialog; }
		}
	}
	public class HETableCellPropertiesRibbonCommand : HERibbonCommandBase {
		public HETableCellPropertiesRibbonCommand() 
			: base() { }
		public HETableCellPropertiesRibbonCommand(string text)
			: base(text) { }
		public HETableCellPropertiesRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HETableCellPropertiesRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "tablecellpropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_CellProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TableCellPropertiesDialog; }
		}
	}
	public class HEInsertTableRowAboveRibbonCommand : HERibbonCommandBase {
		public HEInsertTableRowAboveRibbonCommand() 
			: base() { }
		public HEInsertTableRowAboveRibbonCommand(string text)
			: base(text) { }
		public HEInsertTableRowAboveRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertTableRowAboveRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "inserttablerowabove"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertTableRowAbove); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowAbove); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableRowAbove; }
		}
	}
	public class HEInsertTableRowBelowRibbonCommand : HERibbonCommandBase {
		public HEInsertTableRowBelowRibbonCommand() 
			: base() { }
		public HEInsertTableRowBelowRibbonCommand(string text)
			: base(text) { }
		public HEInsertTableRowBelowRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertTableRowBelowRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "inserttablerowbelow"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertTableRowBelow); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowBelow); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableRowBelow; }
		}
	}
	public class HEInsertTableColumnToLeftRibbonCommand : HERibbonCommandBase {
		public HEInsertTableColumnToLeftRibbonCommand() 
			: base() { }
		public HEInsertTableColumnToLeftRibbonCommand(string text)
			: base(text) { }
		public HEInsertTableColumnToLeftRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertTableColumnToLeftRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "inserttablecolumntoleft"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertTableColumnToLeft); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToLeft); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableColumnToLeft; }
		}
	}
	public class HEInsertTableColumnToRightRibbonCommand : HERibbonCommandBase {
		public HEInsertTableColumnToRightRibbonCommand() 
			: base() { }
		public HEInsertTableColumnToRightRibbonCommand(string text)
			: base(text) { }
		public HEInsertTableColumnToRightRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertTableColumnToRightRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "inserttablecolumntoright"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertTableColumnToRight); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToRight); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableColumnToRight; }
		}
	}
	public class HESplitTableCellHorizontallyRibbonCommand : HERibbonCommandBase {
		public HESplitTableCellHorizontallyRibbonCommand() 
			: base() { }
		public HESplitTableCellHorizontallyRibbonCommand(string text)
			: base(text) { }
		public HESplitTableCellHorizontallyRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HESplitTableCellHorizontallyRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "splittablecellhorizontally"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellHorizontal); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.SplitTableCellHorizontal; }
		}
	}
	public class HESplitTableCellVerticallyRibbonCommand : HERibbonCommandBase {
		public HESplitTableCellVerticallyRibbonCommand() 
			: base() { }
		public HESplitTableCellVerticallyRibbonCommand(string text)
			: base(text) { }
		public HESplitTableCellVerticallyRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HESplitTableCellVerticallyRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "splittablecellvertically"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellVertical); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.SplitTableCellVertical; }
		}
	}
	public class HEMergeTableCellRightRibbonCommand : HERibbonCommandBase {
		public HEMergeTableCellRightRibbonCommand() 
			: base() { }
		public HEMergeTableCellRightRibbonCommand(string text)
			: base(text) { }
		public HEMergeTableCellRightRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEMergeTableCellRightRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "mergetablecellright"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellHorizontal); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.MergeTableCellHorizontal; }
		}
	}
	public class HEMergeTableCellDownRibbonCommand : HERibbonCommandBase {
		public HEMergeTableCellDownRibbonCommand() 
			: base() { }
		public HEMergeTableCellDownRibbonCommand(string text)
			: base(text) { }
		public HEMergeTableCellDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEMergeTableCellDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "mergetablecelldown"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellDown); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.MergeTableCellDown; }
		}
	}
	public class HEDeleteTableRibbonCommand : HERibbonCommandBase {
		public HEDeleteTableRibbonCommand() 
			: base() { }
		public HEDeleteTableRibbonCommand(string text)
			: base(text) { }
		public HEDeleteTableRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEDeleteTableRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "deletetable"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTable); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.DeleteTable; }
		}
	}
	public class HEDeleteTableRowRibbonCommand : HERibbonCommandBase {
		public HEDeleteTableRowRibbonCommand() 
			: base() { }
		public HEDeleteTableRowRibbonCommand(string text)
			: base(text) { }
		public HEDeleteTableRowRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEDeleteTableRowRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "deletetablerow"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableRow); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.DeleteTableRow; }
		}
	}
	public class HEDeleteTableColumnRibbonCommand : HERibbonCommandBase {
		public HEDeleteTableColumnRibbonCommand() 
			: base() { }
		public HEDeleteTableColumnRibbonCommand(string text)
			: base(text) { }
		public HEDeleteTableColumnRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEDeleteTableColumnRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "deletetablecolumn"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableColumn); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.DeleteTableColumn; }
		}
	}
	public class HEInsertImageRibbonCommand : HERibbonCommandBase {
		public HEInsertImageRibbonCommand() 
			: base() { }
		public HEInsertImageRibbonCommand(string text)
			: base(text) { }
		public HEInsertImageRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertImageRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertimagedialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertImage); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertImageDialog; }
		}
	}
	public class HEInsertLinkDialogRibbonCommand : HERibbonCommandBase {
		public HEInsertLinkDialogRibbonCommand() 
			: base() { }
		public HEInsertLinkDialogRibbonCommand(string text)
			: base(text) { }
		public HEInsertLinkDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertLinkDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertlinkdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertLink); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertLinkDialog; }
		}
	}
	public class HEInsertAudioDialogRibbonCommand : HERibbonCommandBase {
		public HEInsertAudioDialogRibbonCommand()
			: base() { }
		public HEInsertAudioDialogRibbonCommand(string text)
			: base(text) { }
		public HEInsertAudioDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertAudioDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertaudiodialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertAudio); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertAudioDialog; }
		}
	}
	public class HEInsertVideoDialogRibbonCommand : HERibbonCommandBase {
		public HEInsertVideoDialogRibbonCommand()
			: base() { }
		public HEInsertVideoDialogRibbonCommand(string text)
			: base(text) { }
		public HEInsertVideoDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertVideoDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertvideodialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertVideo); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertVideoDialog; }
		}
	}
	public class HEInsertFlashDialogRibbonCommand : HERibbonCommandBase {
		public HEInsertFlashDialogRibbonCommand()
			: base() { }
		public HEInsertFlashDialogRibbonCommand(string text)
			: base(text) { }
		public HEInsertFlashDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertFlashDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertflashdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertFlash); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertFlashDialog; }
		}
	}
	public class HEInsertYouTubeVideoDialogRibbonCommand : HERibbonCommandBase {
		public HEInsertYouTubeVideoDialogRibbonCommand()
			: base() { }
		public HEInsertYouTubeVideoDialogRibbonCommand(string text)
			: base(text) { }
		public HEInsertYouTubeVideoDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertYouTubeVideoDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertyoutubevideodialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertYouTubeVideo); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertYouTubeVideoDialog; }
		}
	}
	public class HEInsertPlaceholderDialogRibbonCommand : HERibbonCommandBase {
		public HEInsertPlaceholderDialogRibbonCommand()
			: base() { }
		public HEInsertPlaceholderDialogRibbonCommand(string text)
			: base(text) { }
		public HEInsertPlaceholderDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEInsertPlaceholderDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "insertplaceholderdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RibbonItem_InsertPlaceholder); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertPlaceholderDialog; }
		}
	}
	public class HEUnlinkRibbonCommand : HERibbonCommandBase {
		public HEUnlinkRibbonCommand() 
			: base() { }
		public HEUnlinkRibbonCommand(string text)
			: base(text) { }
		public HEUnlinkRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEUnlinkRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "unlink"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUnlink); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.UnLink; }
		}
	}
	public class HEFullscreenRibbonCommand : HERibbonToggleCommandBase {
		public HEFullscreenRibbonCommand() 
			: base() { }
		public HEFullscreenRibbonCommand(string text)
			: base(text) { }
		public HEFullscreenRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEFullscreenRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "fullscreen"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandFullscreen); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Fullscreen; }
		}
	}
	public class HESelectAllRibbonCommand : HERibbonToggleCommandBase {
		public HESelectAllRibbonCommand()
			: base() { }
		public HESelectAllRibbonCommand(string text)
			: base(text) { }
		public HESelectAllRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HESelectAllRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "selectall"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SelectAll); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.SelectAll; }
		}
	}
	public class HECheckSpellingRibbonCommand : HERibbonCommandBase {
		public HECheckSpellingRibbonCommand() 
			: base() { }
		public HECheckSpellingRibbonCommand(string text)
			: base(text) { }
		public HECheckSpellingRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HECheckSpellingRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "checkspelling"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCheckSpelling); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.CheckSpelling; }
		}
	}
	public class HEParagraphFormattingRibbonCommand : HERibbonComboBoxCommandBase {
		public HEParagraphFormattingRibbonCommand() 
			: base() { }
		public HEParagraphFormattingRibbonCommand(string text)
			: base(text) { }
		protected override string CommandID {
			get { return "formatblock"; }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Paragraph); }
		}
		protected override int DefaultWidth {
			get { return 120; }
		}
		protected override ListEditItemCollection DefaultItems {
			get { return GetItems(); }
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection fontCollection = new ListEditItemCollection();
			fontCollection.Add("Normal", "p");
			fontCollection.Add("Heading  1", "h1");
			fontCollection.Add("Heading  2", "h2");
			fontCollection.Add("Heading  3", "h3");
			fontCollection.Add("Heading  4", "h4");
			fontCollection.Add("Heading  5", "h5");
			fontCollection.Add("Heading  6", "h6");
			fontCollection.Add("Address", "address");
			fontCollection.Add("Normal (DIV)", "div");
			return fontCollection;
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ListEditItem item in Items)
				result.Add(item.Index, string.Format("<{0} style=\"margin: 0px;\">{1}</{0}>", item.Value, item.Text));
			return result;
		}
	}
	public class HECustomDialogRibbonCommand : HERibbonCommandBase {
		public HECustomDialogRibbonCommand() 
			: base() { }
		public HECustomDialogRibbonCommand(string text)
			: base(text) { }
		public HECustomDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HECustomDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		const string CommandNameFormat = "customdialog;{0}";
		protected override string CommandID {
			get { return string.Format(CommandNameFormat, Name); }
		}
		[Browsable(true)]
		public override string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
	}
	public class HEPrintRibbonCommand : HERibbonCommandBase {
		public HEPrintRibbonCommand() 
			: base() { }
		public HEPrintRibbonCommand(string text)
			: base(text) { }
		public HEPrintRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEPrintRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "print"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPrint); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Print; }
		}
	}
	public class HEFindAndReplaceDialogRibbonCommand : HERibbonCommandBase {
		public HEFindAndReplaceDialogRibbonCommand()
			: base() { }
		public HEFindAndReplaceDialogRibbonCommand(string text)
			: base(text) { }
		public HEFindAndReplaceDialogRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEFindAndReplaceDialogRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		protected override string CommandID {
			get { return "findandreplacedialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FindAndReplace); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.FindAndReplaceDialog; }
		}
	}
	public class HEExportToRtfDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEExportToRtfDropDownRibbonCommand()
			: base() { }
		public HEExportToRtfDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEExportToRtfDropDownRibbonCommand(string text)
			: base(text) { }
		public HEExportToRtfDropDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		public HEExportToRtfDropDownRibbonCommand(string text, params RibbonDropDownButtonItem[] items)
			: base(text, items) { }
		public HEExportToRtfDropDownRibbonCommand(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(text, size, items) { }
		protected override string CommandID {
			get { return string.Format("{0};{1}", "export", HtmlEditorExportFormat.Rtf); }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsRtf); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsRtf_ToolTip); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.GetExportFormatResourceName(HtmlEditorExportFormat.Rtf); }
		}
	}
	public class HEExportToDocxDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEExportToDocxDropDownRibbonCommand()
			: base() { }
		public HEExportToDocxDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEExportToDocxDropDownRibbonCommand(string text)
			: base(text) { }
		public HEExportToDocxDropDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		public HEExportToDocxDropDownRibbonCommand(string text, params RibbonDropDownButtonItem[] items)
			: base(text, items) { }
		public HEExportToDocxDropDownRibbonCommand(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(text, size, items) { }
		protected override string CommandID {
			get { return string.Format("{0};{1}", "export", HtmlEditorExportFormat.Docx); }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsDocx); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsDocx_ToolTip); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.GetExportFormatResourceName(HtmlEditorExportFormat.Docx); }
		}
	}
	public class HEExportToMhtDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEExportToMhtDropDownRibbonCommand()
			: base() { }
		public HEExportToMhtDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEExportToMhtDropDownRibbonCommand(string text)
			: base(text) { }
		public HEExportToMhtDropDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		public HEExportToMhtDropDownRibbonCommand(string text, params RibbonDropDownButtonItem[] items)
			: base(text, items) { }
		public HEExportToMhtDropDownRibbonCommand(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(text, size, items) { }
		protected override string CommandID {
			get { return string.Format("{0};{1}", "export", HtmlEditorExportFormat.Mht); }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsMht); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsMht_ToolTip); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.GetExportFormatResourceName(HtmlEditorExportFormat.Mht); }
		}
	}
	public class HEExportToOdtDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEExportToOdtDropDownRibbonCommand()
			: base() { }
		public HEExportToOdtDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEExportToOdtDropDownRibbonCommand(string text)
			: base(text) { }
		public HEExportToOdtDropDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		public HEExportToOdtDropDownRibbonCommand(string text, params RibbonDropDownButtonItem[] items)
			: base(text, items) { }
		public HEExportToOdtDropDownRibbonCommand(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(text, size, items) { }
		protected override string CommandID {
			get { return string.Format("{0};{1}", "export", HtmlEditorExportFormat.Odt); }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsOdt); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsOdt_ToolTip); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.GetExportFormatResourceName(HtmlEditorExportFormat.Odt); }
		}
	}
	public class HEExportToPdfDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEExportToPdfDropDownRibbonCommand()
			: base() { }
		public HEExportToPdfDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEExportToPdfDropDownRibbonCommand(string text)
			: base(text) { }
		public HEExportToPdfDropDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		public HEExportToPdfDropDownRibbonCommand(string text, params RibbonDropDownButtonItem[] items)
			: base(text, items) { }
		public HEExportToPdfDropDownRibbonCommand(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(text, size, items) { }
		protected override string CommandID {
			get { return string.Format("{0};{1}", "export", HtmlEditorExportFormat.Pdf); }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsPdf); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsPdf_ToolTip); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.GetExportFormatResourceName(HtmlEditorExportFormat.Pdf); }
		}
	}
	public class HEExportToTxtDropDownRibbonCommand : HERibbonDropDownCommandBase {
		public HEExportToTxtDropDownRibbonCommand()
			: base() { }
		public HEExportToTxtDropDownRibbonCommand(RibbonItemSize size)
			: base(size) { }
		public HEExportToTxtDropDownRibbonCommand(string text)
			: base(text) { }
		public HEExportToTxtDropDownRibbonCommand(string text, RibbonItemSize size)
			: base(text, size) { }
		public HEExportToTxtDropDownRibbonCommand(string text, params RibbonDropDownButtonItem[] items)
			: base(text, items) { }
		public HEExportToTxtDropDownRibbonCommand(string text, RibbonItemSize size, params RibbonDropDownButtonItem[] items) 
			: base(text, size, items) { }
		protected override string CommandID {
			get { return string.Format("{0};{1}", "export", HtmlEditorExportFormat.Txt); }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsTxt); }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SaveAsTxt_ToolTip); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.GetExportFormatResourceName(HtmlEditorExportFormat.Txt); }
		}
	}
	public class HECustomCssRibbonCommand : HERibbonComboBoxCommandBase {
		HERibbonCustomCssListBoxProperties properties;
		public HECustomCssRibbonCommand() 
			: this(string.Empty) { }
		public HECustomCssRibbonCommand(string text)
			: base(text) {
			this.PropertiesComboBox.Width = System.Web.UI.WebControls.Unit.Pixel(DefaultWidth);
			this.PropertiesComboBox.NullText = string.Format("({0})", DefaultToolTip);
		}
		protected override string CommandID {
			get { return "applycss"; }
		}
		protected override int DefaultWidth {
			get { return 130; }
		}
		protected override string DefaultToolTip {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandApplyCss); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HECustomCssRibbonCommandItems"),
#endif
		AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Themeable(false)]
		public new ToolbarCustomCssListEditItemCollection Items {
			get { return (ToolbarCustomCssListEditItemCollection)PropertiesComboBox.Items; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HECustomCssRibbonCommandPropertiesComboBox"),
#endif
		AutoFormatDisable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Themeable(false)]
		public new HERibbonCustomCssListBoxProperties PropertiesComboBox {
			get {
				if (properties == null)
					properties = new HERibbonCustomCssListBoxProperties(this);
				return properties;
			}
		}
		protected override RibbonComboBoxProperties GetComboBoxProperties() {
			return PropertiesComboBox;
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ToolbarCustomCssListEditItem item in Items) {
				NameValueCollection attrs = new NameValueCollection();
				attrs.Add("style", "display: block");
				ToolbarCustomListEditItemTemplate templ = new ToolbarCustomListEditItemTemplate(item.GetText(), "span", attrs, item.PreviewStyle);
				Control tampControl = new Control();
				templ.InstantiateIn(tampControl);
				result.Add(item.Index, RenderUtils.GetRenderResult(tampControl));
			}
			return result;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HECustomCssRibbonCommand item = source as HECustomCssRibbonCommand;
			if(item != null)
				PropertiesComboBox.Assign(item.PropertiesComboBox);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PropertiesComboBox });
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Items" });
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return PropertiesComboBox;
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
	}
	public class HERibbonCustomCssListBoxProperties : RibbonComboBoxProperties {
		public HERibbonCustomCssListBoxProperties(IPropertiesOwner owner)
			: base(owner) { }
		protected override ComboBoxListBoxProperties CreateListBoxProperties() {
			return new ToolbarCustomCssListBoxProperties(this);
		}
	}
}
