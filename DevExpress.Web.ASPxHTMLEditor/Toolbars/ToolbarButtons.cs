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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.Design;
using System;
using System.Collections;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class CustomToolbarButton : HtmlEditorToolbarItem {
		public CustomToolbarButton()
			: base() {
		}
		public CustomToolbarButton(string text)
			: base(text) {
		}
		public CustomToolbarButton(string text, string commandName)
			: base(text, commandName) {
		}
		public CustomToolbarButton(string text, string commandName, string toolTip)
			: base(text, commandName, toolTip) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("CustomToolbarButtonCommandName"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("CustomToolbarButtonViewStyle"),
#endif
		Category("Behavior"), DefaultValue(ViewStyle.Text), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return (ViewStyle)GetEnumProperty("ViewStyle", ViewStyle.Text); }
			set {
				SetEnumProperty("ViewStyle", ViewStyle.Text, value);
				LayoutChanged();
			}
		}
	}
	public class ToolbarCutButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "cut";				
		public ToolbarCutButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCut), DefaultCommandName) {
		}
		public ToolbarCutButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarCutButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCutButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Cut)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCutButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Cut;
		}
		protected internal override string GetNotAllowedMessage() {
			return "This command cannot be executed. Please use Ctrl+X to Cut to the clipboard.";
		}
	}
	public class ToolbarCopyButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "copy";
		public ToolbarCopyButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCopy), DefaultCommandName) {
		}
		public ToolbarCopyButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarCopyButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCopyButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Copy)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCopyButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Copy;
		}
		protected internal override string GetNotAllowedMessage() {
			return "This command cannot be executed. Please use Ctrl+C to Copy to the clipboard.";
		}
	}
	public class ToolbarPasteButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "paste";
		const string Shortcut = "(Ctrl+V)";
		public ToolbarPasteButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPaste), DefaultCommandName) {
		}
		public ToolbarPasteButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarPasteButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarPasteButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Paste)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarPasteButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Paste;
		}
		protected internal override string GetNotAllowedMessage() {
			return "This command cannot be executed. Please use Ctrl+V to Paste from the clipboard.";
		}
	}
	public class ToolbarPasteFromWordButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "pastefromworddialog";
		public ToolbarPasteFromWordButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteRtf), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPasteRtf)) {
		}
		public ToolbarPasteFromWordButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarPasteFromWordButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarPasteFromWordButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_PasteRtf)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarPasteFromWordButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_PasteRtf)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.PasteFromWord;
		}
	}
	public class ToolbarPrintButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "print";
		public ToolbarPrintButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPrint), DefaultCommandName) {
		}
		public ToolbarPrintButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarPrintButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarPrintButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Print)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarPrintButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Print + " " + ShortcutConsts.Print)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Print;
		}
	}
	public class ToolbarUndoButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "undo";
		public ToolbarUndoButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUndo), DefaultCommandName) {
		}
		public ToolbarUndoButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarUndoButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarUndoButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Undo)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarUndoButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Undo;
		}
	}
	public class ToolbarRedoButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "redo";
		public ToolbarRedoButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRedo), DefaultCommandName) {
		}
		public ToolbarRedoButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarRedoButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarRedoButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Redo)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarRedoButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Redo;
		}
	}
	public class ToolbarCheckSpellingButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "checkspelling";
		public ToolbarCheckSpellingButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCheckSpelling), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCheckSpelling)) {
		}
		public ToolbarCheckSpellingButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarCheckSpellingButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCheckSpellingButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_CheckSpelling)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCheckSpellingButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_CheckSpelling)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.CheckSpelling;
		}
	}
	public class ToolbarFontNameEdit : ToolbarComboBoxBase {		
		protected const string DefaultCommandName = "fontname";
		protected const string DefaultWidth = "160";
		public ToolbarFontNameEdit()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontName), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontName)) {
		}
		public ToolbarFontNameEdit(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarFontNameEdit(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		public override void CreateDefaultItems() {
			Items.AddRange(new ToolbarListEditItem[] {
				new ToolbarListEditItem("Times New Roman"),
				new ToolbarListEditItem("Tahoma"),
				new ToolbarListEditItem("Verdana"),
				new ToolbarListEditItem("Arial"),
				new ToolbarListEditItem("MS Sans Serif"),
				new ToolbarListEditItem("Courier"),
				new ToolbarListEditItem("Segoe UI")
			});
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontNameEditDefaultCaption"),
#endif
		DefaultValue("(" + StringResources.HtmlEditorText_FontName + ")")]
		public override string DefaultCaption {
			get { return base.DefaultCaption; }
			set { base.DefaultCaption = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontNameEditText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_FontName)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontNameEditToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_FontName)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontNameEditWidth"),
#endif
		DefaultValue(typeof(Unit), DefaultWidth), AutoFormatDisable, Themeable(false)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontNameEditItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarListEditItemCollection Items {
			get { return (ToolbarListEditItemCollection)ItemsInternal; }
		}
		protected override Unit GetDefaultWidth() {
			return Unit.Pixel(int.Parse(DefaultWidth));
		}
		protected override string GetDefaultCaption() {
			return string.Format("({0})", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontName));
		}
		protected internal override ToolbarComboBoxControl CreateComboBoxInstance(ASPxWebControl owner, ToolbarComboBoxProperties properties) {
			return new ToolbarFontNameComboBoxControl(owner, properties);
		}
		protected internal override string GetDesignTimeValue(HtmlEditorDocumentStyles docStyle) {
			return docStyle.Font.Name;
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarFontSizeEdit : ToolbarComboBoxBase {		
		protected const string DefaultCommandName = "fontsize";
		protected const string DefaultWidth = "100";
		public ToolbarFontSizeEdit()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontSize), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontSize)) {
		}
		public ToolbarFontSizeEdit(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarFontSizeEdit(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		public override void CreateDefaultItems() {
			Items.AddRange(new ToolbarListEditItem[] {
				new ToolbarListEditItem("1 (8pt)", "1"),
				new ToolbarListEditItem("2 (10pt)", "2"),
				new ToolbarListEditItem("3 (12pt)", "3"),
				new ToolbarListEditItem("4 (14pt)", "4"),
				new ToolbarListEditItem("5 (18pt)", "5"),
				new ToolbarListEditItem("6 (24pt)", "6"),
				new ToolbarListEditItem("7 (36pt)", "7")
			});
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontSizeEditDefaultCaption"),
#endif
		DefaultValue("(" + StringResources.HtmlEditorText_FontSize + ")")]
		public override string DefaultCaption {
			get { return base.DefaultCaption; }
			set { base.DefaultCaption = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontSizeEditText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_FontSize)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontSizeEditToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_FontSize)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontSizeEditWidth"),
#endif
		DefaultValue(typeof(Unit), DefaultWidth), AutoFormatDisable, Themeable(false)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontSizeEditItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarListEditItemCollection Items {
			get { return (ToolbarListEditItemCollection)ItemsInternal; }
		}
		protected override Unit GetDefaultWidth() {
			return Unit.Pixel(int.Parse(DefaultWidth));
		}
		protected override string GetDefaultCaption() {
			return string.Format("({0})", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FontSize));
		}
		protected internal override ToolbarComboBoxControl CreateComboBoxInstance(ASPxWebControl owner, ToolbarComboBoxProperties properties) {
			return new ToolbarFontSizeComboBoxControl(owner, properties);
		}
		protected internal override string GetDesignTimeValue(HtmlEditorDocumentStyles docStyle) {
			return docStyle.Font.Size.ToString();
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarParagraphFormattingEdit : ToolbarComboBoxBase {		
		protected const string DefaultCommandName = "formatblock";
		protected const string DefaultWidth = "120";
		public ToolbarParagraphFormattingEdit()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Paragraph), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Paragraph)) {
		}
		public ToolbarParagraphFormattingEdit(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarParagraphFormattingEdit(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		public override void CreateDefaultItems() {
			Items.AddRange(new ToolbarListEditItem[] {
				new ToolbarListEditItem("Normal", "p"),
				new ToolbarListEditItem("Heading  1", "h1"),
				new ToolbarListEditItem("Heading  2", "h2"),
				new ToolbarListEditItem("Heading  3", "h3"),
				new ToolbarListEditItem("Heading  4", "h4"),
				new ToolbarListEditItem("Heading  5", "h5"),
				new ToolbarListEditItem("Heading  6", "h6"),
				new ToolbarListEditItem("Address", "address"),
				new ToolbarListEditItem("Normal (DIV)", "div")
			});
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarParagraphFormattingEditDefaultCaption"),
#endif
		DefaultValue("(" + StringResources.HtmlEditorText_Paragraph + ")")]
		public override string DefaultCaption {
			get { return base.DefaultCaption; }
			set { base.DefaultCaption = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarParagraphFormattingEditText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Paragraph)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarParagraphFormattingEditToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Paragraph)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarParagraphFormattingEditItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarListEditItemCollection Items {
			get { return (ToolbarListEditItemCollection)ItemsInternal; }
		}
		protected override Unit GetDefaultWidth() {
			return Unit.Pixel(int.Parse(DefaultWidth));
		}
		protected override string GetDefaultCaption() {
			return string.Format("({0})", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.Paragraph));
		}
		protected internal override ToolbarComboBoxControl CreateComboBoxInstance(ASPxWebControl owner, ToolbarComboBoxProperties properties) {
			return new ToolbarParagraphFormattingComboBoxControl(owner, properties);
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarComboBox : ToolbarComboBoxBase {
		protected const string DefaultWidth = "100";
		public ToolbarComboBox()
			: this(string.Empty) {
		}
		public ToolbarComboBox(string commandName)
			: this(commandName, false) {
		}
		public ToolbarComboBox(string commandName, bool beginGroup)
			: this(commandName, beginGroup, true) {
		}
		public ToolbarComboBox(string commandName, bool beginGroup, bool visible)
			: base(commandName) {
			BeginGroup = beginGroup;
			Visible = visible;
		}
		public ToolbarComboBox(string commandName, params ToolbarCustomListEditItem[] items)
			: this(commandName, false, true, items) {
		}
		public ToolbarComboBox(string commandName, bool beginGroup, bool visible, params ToolbarCustomListEditItem[] items)
			: this(commandName, beginGroup, visible) {
			Items.AddRange(items);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxCommandName"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxWidth"),
#endif
		DefaultValue(typeof(Unit), DefaultWidth), AutoFormatDisable, Themeable(false)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarCustomListEditItemCollection Items {
			get { return (ToolbarCustomListEditItemCollection)ItemsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxPropertiesComboBox"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ToolbarCustomComboBoxProperties PropertiesComboBox { get { return (ToolbarCustomComboBoxProperties)Properties; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit DropDownHeight {
			get { return base.DropDownHeight; }
			set { base.DropDownHeight = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit DropDownWidth {
			get { return base.DropDownWidth; }
			set { base.DropDownWidth = value; }
		}
		protected override Unit GetDefaultWidth() {
			return Unit.Pixel(int.Parse(DefaultWidth));
		}
		protected internal override ToolbarComboBoxControl CreateComboBoxInstance(ASPxWebControl owner, ToolbarComboBoxProperties properties) {
			ToolbarCustomComboBoxProperties cProperties = properties as ToolbarCustomComboBoxProperties;
			ToolbarCustomComboBoxControl combobox = new ToolbarCustomComboBoxControl(owner, cProperties);
			cProperties.CommandName = CommandName;
			combobox.DataSource = Properties.DataSource;
			combobox.DataSourceID = Properties.DataSourceID;
			return combobox;
		}
		protected internal override ToolbarComboBoxProperties CreateComboBoxProperties(IPropertiesOwner owner) {
			return new ToolbarCustomComboBoxProperties(owner, GetDefaultCaption());
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarBoldButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "bold";
		public ToolbarBoldButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBold), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarBoldButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarBoldButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarBoldButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Bold)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarBoldButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Bold;
		}
		protected internal override bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return docStyle.Font.Bold;
		}
	}
	public class ToolbarItalicButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "italic";
		public ToolbarItalicButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandItalic), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarItalicButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarItalicButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarItalicButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Italic)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarItalicButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Italic;
		}
		protected internal override bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return docStyle.Font.Italic;
		}
	}
	public class ToolbarUnderlineButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "underline";
		public ToolbarUnderlineButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUnderline), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUnderline)) {
			Checkable = true;
		}
		public ToolbarUnderlineButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarUnderlineButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarUnderlineButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Underline)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarUnderlineButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Underline)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Underline;
		}
	}
	public class ToolbarStrikethroughButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "strikethrough";
		public ToolbarStrikethroughButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandStrikethrough), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandStrikethrough)) {
			Checkable = true;
		}
		public ToolbarStrikethroughButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarStrikethroughButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarStrikethroughButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Strikethrough)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarStrikethroughButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Strikethrough)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Strikethrough;
		}
	}
	public class ToolbarJustifyCenterButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "justifycenter";
		public ToolbarJustifyCenterButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCenter), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarJustifyCenterButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarJustifyCenterButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyCenterButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_AlignCenter)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyCenterButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Center;
		}
		protected internal override bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return docStyle.HorizontalAlign == HorizontalAlign.Center;
		}
	}
	public class ToolbarJustifyLeftButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "justifyleft";
		public ToolbarJustifyLeftButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandLeft), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarJustifyLeftButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarJustifyLeftButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyLeftButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_AlignLeft)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyLeftButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Left;
		}
		protected internal override bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return docStyle.HorizontalAlign == HorizontalAlign.Left;
		}
	}
	public class ToolbarJustifyRightButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "justifyright";
		public ToolbarJustifyRightButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRight), 
			DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarJustifyRightButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarJustifyRightButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyRightButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_AlignRight)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyRightButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Right;
		}
		protected internal override bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return docStyle.HorizontalAlign == HorizontalAlign.Right;
		}
	}
	public class ToolbarJustifyFullButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "justifyfull";
		public ToolbarJustifyFullButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandJustify), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandJustify)) {
			Checkable = true;
		}
		public ToolbarJustifyFullButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarJustifyFullButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyFullButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Justify)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarJustifyFullButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Justify)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Justify;
		}
		protected internal override bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return docStyle.HorizontalAlign == HorizontalAlign.Justify;
		}
	}
	public class ToolbarSuperscriptButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "superscript";
		public ToolbarSuperscriptButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandSuperscript), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandSuperscript)) {
			Checkable = true;
		}
public ToolbarSuperscriptButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarSuperscriptButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSuperscriptButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Superscript)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSuperscriptButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Superscript)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Superscript;
		}
	}
	public class ToolbarSubscriptButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "subscript";
		public ToolbarSubscriptButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandSubscript), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandSubscript)) {
			Checkable = true;
		}
public ToolbarSubscriptButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarSubscriptButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSubscriptButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Subscript)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSubscriptButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Subscript)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Subscript;
		}
	}
	public class ToolbarIndentButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "indent";
		public ToolbarIndentButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandIndent), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandIndent)) {
			Checkable = true;
		}
		public ToolbarIndentButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarIndentButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarIndentButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Indent)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarIndentButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Indent)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return IsRightToLeftInternal() ? HtmlEditorIconImages.Outdent : HtmlEditorIconImages.Indent;
		}
	}
	public class ToolbarOutdentButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "outdent";
		public ToolbarOutdentButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOutdent), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOutdent)) {
			Checkable = true;
		}
		public ToolbarOutdentButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarOutdentButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarOutdentButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Outdent)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarOutdentButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Outdent)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return IsRightToLeftInternal() ? HtmlEditorIconImages.Indent : HtmlEditorIconImages.Outdent;
		}
	}
	public class ToolbarInsertLinkDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertlinkdialog";
		public ToolbarInsertLinkDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertLink), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarInsertLinkDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertLinkDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertLinkDialogButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_InsertLink)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertLinkDialogButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertLinkDialog;
		}
	}
	public class ToolbarInsertOrderedListButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertorderedlist";
		public ToolbarInsertOrderedListButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOrderedList), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOrderedList)) {
			Checkable = true;
		}
		public ToolbarInsertOrderedListButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertOrderedListButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertOrderedListButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_OrderedList)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertOrderedListButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_OrderedList)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.OrderedList;
		}
	}
	public class ToolbarInsertUnorderedListButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertunorderedlist";
		public ToolbarInsertUnorderedListButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBulletList), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBulletList)) {
			Checkable = true;
		}
		public ToolbarInsertUnorderedListButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertUnorderedListButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertUnorderedListButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_BulletList)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertUnorderedListButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_BulletList)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.UnorderedList;
		}
	}
	public class ToolbarUnlinkButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "unlink";
		public ToolbarUnlinkButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUnlink), DefaultCommandName) {
		}
		public ToolbarUnlinkButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarUnlinkButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarUnlinkButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Unlink)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarUnlinkButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.UnLink;
		}
	}
	public class ToolbarFindAndReplaceDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "findandreplacedialog";
		public ToolbarFindAndReplaceDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.FindAndReplace), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarFindAndReplaceDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarFindAndReplaceDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_FindAndReplace)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.FindAndReplaceDialog;
		}
	}
	public class ToolbarInsertImageDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertimagedialog";
		public ToolbarInsertImageDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertImage), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarInsertImageDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertImageDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertImageDialogButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_InsertImage)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertImageDialogButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertImageDialog;
		}
	}
	public class ToolbarInsertAudioDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertaudiodialog";
		public ToolbarInsertAudioDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertAudio),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertAudio)) {
			Checkable = true;
		}
		public ToolbarInsertAudioDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertAudioDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertAudio)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertAudio)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertAudioDialog;
		}
	}
	public class ToolbarInsertVideoDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertvideodialog";
		public ToolbarInsertVideoDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertVideo),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertVideo)) {
			Checkable = true;
		}
		public ToolbarInsertVideoDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertVideoDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertVideo)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertVideo)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertVideoDialog;
		}
	}
	public class ToolbarInsertFlashDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertflashdialog";
		public ToolbarInsertFlashDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertFlash)) {
			Checkable = true;
		}
		public ToolbarInsertFlashDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertFlashDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertFlash)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertFlash)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertFlashDialog;
		}
	}
	public class ToolbarInsertYouTubeVideoDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertyoutubevideodialog";
		public ToolbarInsertYouTubeVideoDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertYouTubeVideo)) {
			Checkable = true;
		}
		public ToolbarInsertYouTubeVideoDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertYouTubeVideoDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertYouTubeVideo)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertYouTubeVideo)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertYouTubeVideoDialog;
		}
	}
	public class ToolbarInsertPlaceholderDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "insertplaceholderdialog";
		public ToolbarInsertPlaceholderDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertPlaceholder),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertPlaceholder)) {
			Checkable = true;
		}
		public ToolbarInsertPlaceholderDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertPlaceholderDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertPlaceholder)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertPlaceholder)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertPlaceholderDialog;
		}
	}
	public class ToolbarTableOperationsDropDownButton : ToolbarDropDownBase {
		protected const string DefaultCommandName = "inserttabledialog";
		public ToolbarTableOperationsDropDownButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTable), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTable)) {
		}
		public ToolbarTableOperationsDropDownButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarTableOperationsDropDownButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		public override void CreateDefaultItems() {
			Items.Add(
				new ToolbarInsertTableDialogButton(true),
				new ToolbarTablePropertiesDialogButton(true),
				new ToolbarTableRowPropertiesDialogButton(),
				new ToolbarTableColumnPropertiesDialogButton(),
				new ToolbarTableCellPropertiesDialogButton(),
				new ToolbarInsertTableRowAboveButton(true),
				new ToolbarInsertTableRowBelowButton(),
				new ToolbarInsertTableColumnToLeftButton(),
				new ToolbarInsertTableColumnToRightButton(),
				new ToolbarSplitTableCellHorizontallyButton(true),
				new ToolbarSplitTableCellVerticallyButton(),
				new ToolbarMergeTableCellRightButton(),
				new ToolbarMergeTableCellDownButton(),
				new ToolbarDeleteTableButton(true),
				new ToolbarDeleteTableRowButton(),
				new ToolbarDeleteTableColumnButton()
			);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableOperationsDropDownButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTable)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableOperationsDropDownButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTable)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableOperationsDropDownButtonItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public HtmlEditorToolbarItemCollection Items {
			get { return (HtmlEditorToolbarItemCollection)ItemsInternal; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertTableDialog;
		}
		protected override DropDownItemClickMode GetDefaultClickModeValue() {
			return DropDownItemClickMode.ExecuteAction;
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarInsertTableDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "inserttabledialog";
		public ToolbarInsertTableDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_InsertTable),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_InsertTable), ViewStyle.ImageAndText) {
		}
		public ToolbarInsertTableDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertTableDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableDialogButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableDialogButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTable)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableDialogButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTable)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertTableDialog;
		}
	}
	public class ToolbarTablePropertiesDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "tablepropertiesdialog";
		public ToolbarTablePropertiesDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableProperties),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableProperties),
			ViewStyle.ImageAndText) {
		}
		public ToolbarTablePropertiesDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarTablePropertiesDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTablePropertiesDialogButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTablePropertiesDialogButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableProperties)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTablePropertiesDialogButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_TableProperties)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.TablePropertiesDialog;
		}
	}
	public class ToolbarTableCellPropertiesDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "tablecellpropertiesdialog";
		public ToolbarTableCellPropertiesDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableCellProperties),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableCellProperties),
			ViewStyle.ImageAndText) {
		}
		public ToolbarTableCellPropertiesDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarTableCellPropertiesDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableCellPropertiesDialogButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableCellPropertiesDialogButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableCellProperties)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableCellPropertiesDialogButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_TableCellProperties)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.TableCellPropertiesDialog;
		}
	}
	public class ToolbarTableColumnPropertiesDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "tablecolumnpropertiesdialog";
		public ToolbarTableColumnPropertiesDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableColumnProperties),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableColumnProperties),
			ViewStyle.ImageAndText) {
		}
		public ToolbarTableColumnPropertiesDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarTableColumnPropertiesDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableColumnPropertiesDialogButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableColumnPropertiesDialogButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableColumnProperties)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableColumnPropertiesDialogButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_TableColumnProperties)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.TableColumnPropertiesDialog;
		}
	}
	public class ToolbarTableRowPropertiesDialogButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "tablerowpropertiesdialog";
		public ToolbarTableRowPropertiesDialogButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableRowProperties),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.TableRowProperties),
			ViewStyle.ImageAndText) {
		}
		public ToolbarTableRowPropertiesDialogButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarTableRowPropertiesDialogButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableRowPropertiesDialogButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableRowPropertiesDialogButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableRowProperties)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarTableRowPropertiesDialogButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_TableRowProperties)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.TableRowPropertiesDialog;
		}
	}
	public class ToolbarInsertTableRowAboveButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "inserttablerowabove";
		public ToolbarInsertTableRowAboveButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowAbove),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowAbove),
			ViewStyle.ImageAndText) {
		}
		public ToolbarInsertTableRowAboveButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertTableRowAboveButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableRowAboveButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableRowAboveButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableRowAbove)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableRowAboveButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableRowAbove)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertTableRowAbove;
		}
	}
	public class ToolbarInsertTableRowBelowButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "inserttablerowbelow";
		public ToolbarInsertTableRowBelowButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowBelow),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowBelow),
			ViewStyle.ImageAndText) {
		}
		public ToolbarInsertTableRowBelowButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertTableRowBelowButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableRowBelowButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableRowBelowButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableRowBelow)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableRowBelowButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableRowBelow)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertTableRowBelow;
		}
	}
	public class ToolbarInsertTableColumnToLeftButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "inserttablecolumntoleft";
		public ToolbarInsertTableColumnToLeftButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToLeft),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToLeft),
			ViewStyle.ImageAndText) {
		}
		public ToolbarInsertTableColumnToLeftButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertTableColumnToLeftButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableColumnToLeftButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableColumnToLeftButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableColumnToLeft)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableColumnToLeftButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableColumnToLeft)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertTableColumnToLeft;
		}
	}
	public class ToolbarInsertTableColumnToRightButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "inserttablecolumntoright";
		public ToolbarInsertTableColumnToRightButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToRight),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToRight),
			ViewStyle.ImageAndText) {
		}
		public ToolbarInsertTableColumnToRightButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarInsertTableColumnToRightButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableColumnToRightButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableColumnToRightButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableColumnToRight)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarInsertTableColumnToRightButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_InsertTableColumnToRight)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.InsertTableColumnToRight;
		}
	}
	public class ToolbarDeleteTableButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "deletetable";
		public ToolbarDeleteTableButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTable),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTable), 
			ViewStyle.ImageAndText) {
		}
		public ToolbarDeleteTableButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarDeleteTableButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_DeleteTable)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_DeleteTable)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.DeleteTable;
		}
	}
	public class ToolbarDeleteTableColumnButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "deletetablecolumn";
		public ToolbarDeleteTableColumnButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableColumn),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableColumn),
			ViewStyle.ImageAndText) {
		}
		public ToolbarDeleteTableColumnButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarDeleteTableColumnButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableColumnButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableColumnButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_DeleteTableColumn)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableColumnButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_DeleteTableColumn)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.DeleteTableColumn;
		}
	}
	public class ToolbarDeleteTableRowButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "deletetablerow";
		public ToolbarDeleteTableRowButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableRow),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableRow),
			ViewStyle.ImageAndText) {
		}
		public ToolbarDeleteTableRowButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarDeleteTableRowButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableRowButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableRowButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_DeleteTableRow)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDeleteTableRowButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_DeleteTableRow)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.DeleteTableRow;
		}
	}
	public class ToolbarSplitTableCellHorizontallyButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "splittablecellhorizontally";
		public ToolbarSplitTableCellHorizontallyButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellHorizontal),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellHorizontal),
			ViewStyle.ImageAndText) {
		}
		public ToolbarSplitTableCellHorizontallyButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarSplitTableCellHorizontallyButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSplitTableCellHorizontallyButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSplitTableCellHorizontallyButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_SplitTableCellHorizontal)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSplitTableCellHorizontallyButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_SplitTableCellHorizontal)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.SplitTableCellHorizontal;
		}
	}
	public class ToolbarSplitTableCellVerticallyButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "splittablecellvertically";
		public ToolbarSplitTableCellVerticallyButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellVertical),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellVertical),
			ViewStyle.ImageAndText) {
		}
		public ToolbarSplitTableCellVerticallyButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarSplitTableCellVerticallyButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSplitTableCellVerticallyButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSplitTableCellVerticallyButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_SplitTableCellVertical)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarSplitTableCellVerticallyButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_SplitTableCellVertical)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.SplitTableCellVertical;
		}
	}
	public class ToolbarMergeTableCellRightButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "mergetablecellright";
		public ToolbarMergeTableCellRightButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellHorizontal),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellHorizontal),
			ViewStyle.ImageAndText) {
		}
		public ToolbarMergeTableCellRightButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarMergeTableCellRightButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMergeTableCellRightButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMergeTableCellRightButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_MergeTableCellRight)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMergeTableCellRightButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_MergeTableCellRight)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.MergeTableCellHorizontal;
		}
	}
	public class ToolbarMergeTableCellDownButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "mergetablecelldown";
		public ToolbarMergeTableCellDownButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellDown),
			DefaultCommandName, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellDown),
			ViewStyle.ImageAndText) {
		}
		public ToolbarMergeTableCellDownButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarMergeTableCellDownButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMergeTableCellDownButtonViewStyle"),
#endif
Category("Behavior"), DefaultValue(ViewStyle.ImageAndText), AutoFormatDisable]
		public override ViewStyle ViewStyle {
			get { return base.ViewStyle; }
			set { base.ViewStyle = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMergeTableCellDownButtonText"),
#endif
DefaultValue(StringResources.HtmlEditorText_MergeTableCellDown)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarMergeTableCellDownButtonToolTip"),
#endif
DefaultValue(StringResources.HtmlEditorText_MergeTableCellDown)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.MergeTableCellDown;
		}
	}
	public class ToolbarRemoveFormatButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "removeformat";
		public ToolbarRemoveFormatButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRemoveFormat),
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandRemoveFormat)) {
		}
		public ToolbarRemoveFormatButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarRemoveFormatButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarRemoveFormatButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RemoveFormat)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarRemoveFormatButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_RemoveFormat)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.RemoveFormat;
		}
	}
	public class ToolbarDropDownItemPicker : ToolbarCustomDropDownBase {
		HtmlEditorDropDownItemPickerStyle style = new HtmlEditorDropDownItemPickerStyle();
		HtmlEditorDropDownItemPickerItemStyle itemStyle = new HtmlEditorDropDownItemPickerItemStyle();
		public ToolbarDropDownItemPicker()
			: this(string.Empty) {
		}
		public ToolbarDropDownItemPicker(string commandName)
			: this(commandName, false) {
		}
		public ToolbarDropDownItemPicker(string commandName, bool beginGroup)
			: this(commandName, beginGroup, true) {
		}
		public ToolbarDropDownItemPicker(string commandName, bool beginGroup, bool visible)
			: base(commandName) {
			BeginGroup = beginGroup;
			Visible = visible;
			ClickMode = DropDownItemClickMode.ExecuteSelectedItemAction;
		}
		public ToolbarDropDownItemPicker(string commandName, params ToolbarItemPickerItem[] items)
			: this(commandName, false, true, items) {
		}
		public ToolbarDropDownItemPicker(string commandName, bool beginGroup, bool visible, params ToolbarItemPickerItem[] items)
			: this(commandName, beginGroup, visible) {
			Items.AddRange(items);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarItemPickerItemCollection Items {
			get { return (ToolbarItemPickerItemCollection)ItemsInternal; }
		}
		protected override Collection CreateDropDownItemCollection() {
			return new ToolbarItemPickerItemCollection(this);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerCommandName"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerColumnCount"),
#endif
		DefaultValue(3), NotifyParentProperty(true)]
		public int ColumnCount {
			get { return GetIntProperty("ColumnCount", 3); }
			set { SetIntProperty("ColumnCount", 3, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerItemHeight"),
#endif
		DefaultValue(0), AutoFormatDisable, Themeable(false)]
		public int ItemHeight {
			get { return GetIntProperty("ItemHeight", 0); }
			set { SetIntProperty("ItemHeight", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerItemWidth"),
#endif
		DefaultValue(0), AutoFormatDisable, Themeable(false)]
		public int ItemWidth {
			get { return GetIntProperty("ItemWidth", 0); }
			set { SetIntProperty("ItemWidth", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerItemPickerStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDropDownItemPickerStyle ItemPickerStyle { get { return style; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerItemPickerItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorDropDownItemPickerItemStyle ItemPickerItemStyle { get { return itemStyle; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerImagePosition"),
#endif
		DefaultValue(ToolbarItemPickerImagePosition.Left), NotifyParentProperty(true)]
		public ToolbarItemPickerImagePosition ImagePosition {
			get { return (ToolbarItemPickerImagePosition)GetEnumProperty("ImagePosition", ToolbarItemPickerImagePosition.Left); }
			set { SetEnumProperty("ImagePosition", ToolbarItemPickerImagePosition.Left, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownItemPickerClickMode"),
#endif
		DefaultValue(DropDownItemClickMode.ExecuteSelectedItemAction)]
		public override DropDownItemClickMode ClickMode { get { return base.ClickMode; } set { base.ClickMode = value; } }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarDropDownItemPicker src = source as ToolbarDropDownItemPicker;
			if(src != null) {
				ColumnCount = src.ColumnCount;
				ImagePosition = src.ImagePosition;
				ItemWidth = src.ItemWidth;
				ItemHeight = src.ItemHeight;
				ItemPickerStyle.Assign(src.ItemPickerStyle);
				ItemPickerItemStyle.Assign(src.ItemPickerItemStyle);
			}
		}
		protected internal override ToolbarCustomItem GetSelectedItem() {
			if(SelectedItemIndex < Items.Count)
				return Items[SelectedItemIndex];
			return null;
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarDropDownMenu : ToolbarCustomDropDownBase {
		public ToolbarDropDownMenu()
			: this(string.Empty) {
		}
		public ToolbarDropDownMenu(string commandName)
			: this(commandName, false) {
		}
		public ToolbarDropDownMenu(string commandName, bool beginGroup)
			: this(commandName, beginGroup, true) {
		}
		public ToolbarDropDownMenu(string commandName, bool beginGroup, bool visible)
			: base(commandName) {
			BeginGroup = beginGroup;
			Visible = visible;
		}
		public ToolbarDropDownMenu(string commandName, params ToolbarMenuItem[] items)
			: this(commandName, false, true, items) {
		}
		public ToolbarDropDownMenu(string commandName, bool beginGroup, bool visible, params ToolbarMenuItem[] items)
			: this(commandName, beginGroup, visible) {
			Items.AddRange(items);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownMenuItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarMenuItemCollection Items {
			get { return (ToolbarMenuItemCollection)ItemsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownMenuCommandName"),
#endif
		Category("Behavior"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarDropDownMenuBeginGroupField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string BeginGroupField {
			get { return GetStringProperty("BeginGroupField", ""); }
			set { SetStringProperty("BeginGroupField", "", value); }
		}
		protected override Collection CreateDropDownItemCollection() {
			return new ToolbarMenuItemCollection(this);
		}
		protected internal override ToolbarCustomItem GetSelectedItem() {
			if(SelectedItemIndex < Items.Count)
				return Items[SelectedItemIndex];
			return null;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarDropDownMenu src = source as ToolbarDropDownMenu;
			if(src != null)
				BeginGroupField = src.BeginGroupField;
		}
		internal override bool IsMenu { get { return true; } }
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarFontColorButton : ToolbarColorButtonBase {
		protected internal const string DefaultCommandName = "forecolor";
		protected const string DefaultColor = "#000000";
		public ToolbarFontColorButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandForeColor), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandForeColor), 
			DefaultColor) {
		}
		public ToolbarFontColorButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarFontColorButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontColorButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_ForeColor)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFontColorButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_ForeColor)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.ForeColor;
		}
		protected override DropDownItemClickMode GetDefaultClickModeValue() {
			return DropDownItemClickMode.ExecuteSelectedItemAction;
		}
		protected internal override string GetDesignTimeValue(HtmlEditorDocumentStyles docStyle) {
			return System.Drawing.ColorTranslator.ToHtml(docStyle.ForeColor);
		}
	}
	public class ToolbarBackColorButton : ToolbarColorButtonBase {
		protected internal const string DefaultCommandName = "backcolor";
		protected const string DefaultColor = "#FFFFFF";
		public ToolbarBackColorButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBackColor), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandBackColor), 
			DefaultColor) {
		}
		public ToolbarBackColorButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarBackColorButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarBackColorButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_BackColor)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarBackColorButtonToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_BackColor)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.BackColor;
		}
		protected override DropDownItemClickMode GetDefaultClickModeValue() {
			return DropDownItemClickMode.ExecuteSelectedItemAction;
		}
	}
	public class ToolbarCustomCssEdit : ToolbarComboBoxBase {		
		protected const string DefaultCommandName = "applycss";
		protected const string DefaultWidth = "130";
		public ToolbarCustomCssEdit()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandApplyCss), 
			DefaultCommandName,
			ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandApplyCss)) {
		}
		public ToolbarCustomCssEdit(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarCustomCssEdit(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		public override void CreateDefaultItems() {
			Items.Add(new ToolbarCustomCssListEditItem("Clear Style", "", ""));
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssEditDefaultCaption"),
#endif
		DefaultValue("(" + StringResources.HtmlEditorText_ApplyCss + ")")]
		public override string DefaultCaption {
			get { return base.DefaultCaption; }
			set { base.DefaultCaption = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssEditText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_ApplyCss)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssEditToolTip"),
#endif
		DefaultValue(StringResources.HtmlEditorText_ApplyCss)]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomCssEditItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarCustomCssListEditItemCollection Items {
			get { return (ToolbarCustomCssListEditItemCollection)ItemsInternal; }
		}
		protected override Unit GetDefaultWidth() {
			return Unit.Pixel(int.Parse(DefaultWidth));
		}
		protected override string GetDefaultCaption() {
			return string.Format("({0})", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandApplyCss));
		}
		protected internal override ToolbarComboBoxControl CreateComboBoxInstance(ASPxWebControl owner, ToolbarComboBoxProperties properties) {
			return new ToolbarCustomCssComboBoxControl(owner, properties as ToolbarCustomCssComboBoxProperties);
		}
		protected internal override ToolbarComboBoxProperties CreateComboBoxProperties(IPropertiesOwner owner) {
			return new ToolbarCustomCssComboBoxProperties(owner, GetDefaultCaption());
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarFullscreenButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "fullscreen";
		public ToolbarFullscreenButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandFullscreen), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarFullscreenButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarFullscreenButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFullscreenButtonText"),
#endif
		DefaultValue(StringResources.HtmlEditorText_Fullscreen)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarFullscreenButtonToolTip"),
#endif
 DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.Fullscreen;
		}
	}
	public class ToolbarSelectAllButton : HtmlEditorToolbarItem {
		protected const string DefaultCommandName = "selectall";
		public ToolbarSelectAllButton()
			: base(ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SelectAll), DefaultCommandName) {
			Checkable = true;
		}
		public ToolbarSelectAllButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarSelectAllButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[DefaultValue(StringResources.HtmlEditorText_SelectAll)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[DefaultValue("")]
		public override string ToolTip {
			get { return base.ToolTip; }
			set { base.ToolTip = value; }
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.SelectAll;
		}
	}
	public class ToolbarCustomDialogButton : HtmlEditorToolbarItem {
		const string CommandNameFormat = "customdialog;{0}";
		public ToolbarCustomDialogButton()
			: base() {
		}
		public ToolbarCustomDialogButton(string name)
			: this(name, false) {
		}
		public ToolbarCustomDialogButton(string name, string toolTip)
			: this(name, toolTip, false) {
		}
		public ToolbarCustomDialogButton(string name, bool beginGroup)
			: this(name, beginGroup, true) {
		}
		public ToolbarCustomDialogButton(string name, bool beginGroup, bool visible)
			: this(name, string.Empty, beginGroup, visible) {
		}
		public ToolbarCustomDialogButton(string name, string toolTip, bool beginGroup)
			: this(name, toolTip, beginGroup, true) {
		}
		public ToolbarCustomDialogButton(string name, string toolTip, bool beginGroup, bool visible)
			: this() {
			Name = name;
			ToolTip = toolTip;
			BeginGroup = beginGroup;
			Visible = visible;
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDialogButtonCommandName")]
#endif
		public override string CommandName {
			get { return string.Format(CommandNameFormat, Name); }
			set {}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDialogButtonName"),
#endif
		DefaultValue(""), NotifyParentProperty(true)]
		public virtual string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		public override void Assign(CollectionItem source) {
			ToolbarCustomDialogButton src = source as ToolbarCustomDialogButton;
			if(src != null)
				Name = src.Name;
			base.Assign(source);
		}
	}
	public class ToolbarExportDropDownButton : ToolbarDropDownBase {
		const string DefaultCommandName = "export";
		public ToolbarExportDropDownButton()
			: base("", DefaultCommandName) {
			ClickMode = DropDownItemClickMode.ExecuteSelectedItemAction;
		}
		public ToolbarExportDropDownButton(bool beginGroup)
			: this() {
			BeginGroup = beginGroup;
		}
		public ToolbarExportDropDownButton(bool beginGroup, bool visible)
			: this(beginGroup) {
			Visible = visible;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarExportDropDownButtonItems"),
#endif
PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ToolbarMenuItemCollection Items
		{
			get { return (ToolbarMenuItemCollection)ItemsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarExportDropDownButtonSelectedItemIndex"),
#endif
DefaultValue(0), NotifyParentProperty(true)]
		public int SelectedItemIndex { get { return SelectedItemIndexInternal; } set { SelectedItemIndexInternal = value; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarExportDropDownButtonClickMode"),
#endif
Category("Behavior"), DefaultValue(DropDownItemClickMode.ExecuteSelectedItemAction), AutoFormatDisable]
		public virtual DropDownItemClickMode ClickMode { get { return ClickModeInternal; } set { ClickModeInternal = value; } }
		protected override Collection CreateDropDownItemCollection() {
			return new ToolbarMenuItemCollection(this);
		}
		protected internal override ToolbarCustomItem GetSelectedItem() {
			if(SelectedItemIndex < Items.Count)
				return Items[SelectedItemIndex];
			return null;
		}
		public override void CreateDefaultItems() {
			base.CreateDefaultItems();
			Items.Add(
				new ToolbarExportDropDownItem(HtmlEditorExportFormat.Rtf),
				new ToolbarExportDropDownItem(HtmlEditorExportFormat.Pdf),
				new ToolbarExportDropDownItem(HtmlEditorExportFormat.Txt),
				new ToolbarExportDropDownItem(HtmlEditorExportFormat.Docx),
				new ToolbarExportDropDownItem(HtmlEditorExportFormat.Odt),
				new ToolbarExportDropDownItem(HtmlEditorExportFormat.Mht)
			);
		}
		internal override bool IsMenu { get { return true; } }
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public class ToolbarExportDropDownItem : ToolbarMenuItem {
		public ToolbarExportDropDownItem()
			: base() {
		}
		public ToolbarExportDropDownItem(HtmlEditorExportFormat format)
			: this(format, "") {
		}
		public ToolbarExportDropDownItem(HtmlEditorExportFormat format, string text)
			: this(format, text, text) {
		}
		public ToolbarExportDropDownItem(HtmlEditorExportFormat format, string text, string toolTip)
			: this(format, text, toolTip, false) {
		}
		public ToolbarExportDropDownItem(HtmlEditorExportFormat format, string text, bool beginGroup)
			: this(format, text, text, beginGroup) {
		}
		public ToolbarExportDropDownItem(HtmlEditorExportFormat format, string text, string toolTip, bool beginGroup)
			: this() {
			Format = format;
			Text = text;
			ToolTip = toolTip;
			BeginGroup = beginGroup;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarExportDropDownItemFormat"),
#endif
DefaultValue(HtmlEditorExportFormat.Rtf), NotifyParentProperty(true)]
		public virtual HtmlEditorExportFormat Format
		{
			get { return (HtmlEditorExportFormat)GetEnumProperty("Format", HtmlEditorExportFormat.Rtf); }
			set { SetEnumProperty("Format", HtmlEditorExportFormat.Rtf, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Value {
			get { return Format.ToString(); }
			set { }
		}
		public override void Assign(CollectionItem source) {
			ToolbarExportDropDownItem src = source as ToolbarExportDropDownItem;
			if(src != null)
				Format = src.Format;
			base.Assign(source);
		}
		string GetDefaultTextValue() {
			ASPxHtmlEditorStringId localizationStringId;
			switch(Format) {
				case HtmlEditorExportFormat.Rtf:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsRtf;
					break;
				case HtmlEditorExportFormat.Docx:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsDocx;
					break;
				case HtmlEditorExportFormat.Mht:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsMht;
					break;
				case HtmlEditorExportFormat.Odt:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsOdt;
					break;
				case HtmlEditorExportFormat.Pdf:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsPdf;
					break;
				case HtmlEditorExportFormat.Txt:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsTxt;
					break;
				default:
					throw new NotImplementedException();
			}
			return ASPxHtmlEditorLocalizer.GetString(localizationStringId);
		}
		string GetDefaultToolTipValue() {
			ASPxHtmlEditorStringId localizationStringId;
			switch(Format) {
				case HtmlEditorExportFormat.Rtf:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsRtf_ToolTip;
					break;
				case HtmlEditorExportFormat.Docx:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsDocx_ToolTip;
					break;
				case HtmlEditorExportFormat.Mht:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsMht_ToolTip;
					break;
				case HtmlEditorExportFormat.Odt:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsOdt_ToolTip;
					break;
				case HtmlEditorExportFormat.Pdf:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsPdf_ToolTip;
					break;
				case HtmlEditorExportFormat.Txt:
					localizationStringId = ASPxHtmlEditorStringId.SaveAsTxt_ToolTip;
					break;
				default:
					throw new NotImplementedException();
			}
			return ASPxHtmlEditorLocalizer.GetString(localizationStringId);
		}
		protected internal override string GetText() {
			return string.IsNullOrEmpty(Text)
				? GetDefaultTextValue()
				: Text;
		}
		protected internal override string GetToolTip() {
			return string.IsNullOrEmpty(ToolTip)
				? GetDefaultToolTipValue()
				: ToolTip;
		}
		protected override string GetResourceImageName() {
			return HtmlEditorIconImages.GetExportFormatResourceName(Format);
		}
	}
}
