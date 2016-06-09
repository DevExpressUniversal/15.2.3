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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.ASPxHtmlEditor.Rendering;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxHtmlEditor {
	public class HtmlEditorContextMenuItem : CollectionItem {
		MenuItemImageProperties image;
		public HtmlEditorContextMenuItem() {
			Text = DefaultText;
			CommandName = DefaultCommandName;
		}
		public HtmlEditorContextMenuItem(string text) : this(text, string.Empty) { }
		public HtmlEditorContextMenuItem(string text, string commandName) : this() {
			Text = text;
			CommandName = commandName;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorContextMenuItemText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", string.Empty); }
			set { SetStringProperty("Text", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorContextMenuItemCommandName"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string CommandName {
			get { return GetStringProperty("CommandName", string.Empty); }
			set { SetStringProperty("CommandName", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorContextMenuItemBeginGroup"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", false); }
			set { SetBoolProperty("BeginGroup", false, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorContextMenuItemVisible"),
#endif
		DefaultValue(true), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorContextMenuItemVisibleIndex"),
#endif
		AutoFormatDisable, NotifyParentProperty(true)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		[Category("Images"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public MenuItemImageProperties Image {
			get {
				if(image == null)
					image = new MenuItemImageProperties(this);
				return image;
			}
		}
		protected ASPxHtmlEditor HtmlEditor {
			get { return ((HtmlEditorContextMenuItemCollection)Collection).Owner; }
		}
		protected virtual string DefaultCommandName {
			get { return string.Empty; }
		}
		protected virtual string DefaultText {
			get { return string.Empty; }
		}
		protected virtual string ResourceImageName {
			get { return string.Empty; }
		}
		protected internal string GetCommandName() {
			if(!string.IsNullOrEmpty(CommandName))
				return CommandName;
			return DefaultCommandName;
		}
		protected internal string GetText() {
			if(!string.IsNullOrEmpty(Text))
				return Text;
			return DefaultText;
		}
		protected internal ItemImagePropertiesBase GetImage() {
			if(!string.IsNullOrEmpty(ResourceImageName)) {
				ItemImageProperties properties = new ItemImageProperties();
				properties.CopyFrom(HtmlEditor.Images.IconImages.GetImageProperties(HtmlEditor.Page, ResourceImageName));
				properties.CopyFrom(Image);
				return properties;
			}
			return Image;
		}
		public override string ToString() {
			return Text;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HtmlEditorContextMenuItem item = source as HtmlEditorContextMenuItem;
			if(item != null) {
				Text = item.Text;
				CommandName = item.CommandName;
				BeginGroup = item.BeginGroup;
				Visible = item.Visible;
				Image.Assign(item.Image);
			}
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Image });
		}
	}
	public class HECutContextMenuItem : HtmlEditorContextMenuItem {
		public HECutContextMenuItem() { }
		public HECutContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("cut")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Cut)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "cut"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCut); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Cut; }
		}
	}
	public class HECopyContextMenuItem : HtmlEditorContextMenuItem {
		public HECopyContextMenuItem() { }
		public HECopyContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("copy")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Copy)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "copy"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCopy); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Copy; }
		}
	}
	public class HEPasteContextMenuItem : HtmlEditorContextMenuItem {
		public HEPasteContextMenuItem() { }
		public HEPasteContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("paste")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Paste)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "paste"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandPaste); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Paste; }
		}
	}
	public class HESelectAllContextMenuItem : HtmlEditorContextMenuItem {
		public HESelectAllContextMenuItem() { }
		public HESelectAllContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("selectall")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_SelectAll)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "selectall"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SelectAll); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.SelectAll; }
		}
	}
	public class HEUnlinkContextMenuItem : HtmlEditorContextMenuItem {
		public HEUnlinkContextMenuItem()
			: base() { }
		public HEUnlinkContextMenuItem(string text)
			: base(text) { }
		[Browsable(false), DefaultValue("unlink")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Unlink)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "unlink"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUnlink); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.UnLink; }
		}
	}
	public class HEChangeLinkDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangeLinkDialogContextMenuItem() { }
		public HEChangeLinkDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changelinkdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangeLink)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changelinkdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangeLink); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertLinkDialog; }
		}
	}
	public class HEChangeImageDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangeImageDialogContextMenuItem() { }
		public HEChangeImageDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changeimagedialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangeImage)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changeimagedialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangeImage); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertImageDialog; }
		}
	}
	public class HEChangeAudioDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangeAudioDialogContextMenuItem() { }
		public HEChangeAudioDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changeaudiodialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangeAudio)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changeaudiodialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangeAudio); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertAudioDialog; }
		}
	}
	public class HEChangeVideoDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangeVideoDialogContextMenuItem() { }
		public HEChangeVideoDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changevideodialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangeVideo)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changevideodialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangeVideo); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertVideoDialog; }
		}
	}
	public class HEChangeFlashDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangeFlashDialogContextMenuItem() { }
		public HEChangeFlashDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changeflashdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangeFlash)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changeflashdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangeFlash); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertFlashDialog; }
		}
	}
	public class HEChangeYouTubeVideoDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangeYouTubeVideoDialogContextMenuItem() { }
		public HEChangeYouTubeVideoDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changeyoutubevideodialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangeYouTubeVideo)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changeyoutubevideodialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangeYouTubeVideo); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertYouTubeVideoDialog; }
		}
	}
	public class HETablePropertiesDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HETablePropertiesDialogContextMenuItem() { }
		public HETablePropertiesDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("tablepropertiesdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableProperties)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "tablepropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TablePropertiesDialog; }
		}
	}
	public class HETableRowPropertiesDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HETableRowPropertiesDialogContextMenuItem() { }
		public HETableRowPropertiesDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("tablerowpropertiesdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableRowProperties)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "tablerowpropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableRowProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TableRowPropertiesDialog; }
		}
	}
	public class HETableColumnPropertiesDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HETableColumnPropertiesDialogContextMenuItem() { }
		public HETableColumnPropertiesDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("tablecolumnpropertiesdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableColumnProperties)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "tablecolumnpropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableColumnProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TableColumnPropertiesDialog; }
		}
	}
	public class HETableCellPropertiesDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HETableCellPropertiesDialogContextMenuItem() { }
		public HETableCellPropertiesDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("tablecellpropertiesdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_TableCellProperties)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "tablecellpropertiesdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_TableCellProperties); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.TableCellPropertiesDialog; }
		}
	}
	public class HEInsertTableContextMenuItem : HtmlEditorContextMenuItem {
		public HEInsertTableContextMenuItem()
			: base() { }
		public HEInsertTableContextMenuItem(string text)
			: base(text) { }
		[Browsable(false), DefaultValue("inserttabledialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_InsertTable)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "inserttabledialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_InsertTable); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableDialog; }
		}
	}
	public class HEInsertTableRowAboveContextMenuItem : HtmlEditorContextMenuItem {
		public HEInsertTableRowAboveContextMenuItem() { }
		public HEInsertTableRowAboveContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("inserttablerowabove")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertTableRowAbove)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "inserttablerowabove"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowAbove); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableRowAbove; }
		}
	}
	public class HEInsertTableRowBelowContextMenuItem : HtmlEditorContextMenuItem {
		public HEInsertTableRowBelowContextMenuItem() { }
		public HEInsertTableRowBelowContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("inserttablerowbelow")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertTableRowBelow)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "inserttablerowbelow"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableRowBelow); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableRowBelow; }
		}
	}
	public class HEInsertTableColumnToLeftContextMenuItem : HtmlEditorContextMenuItem {
		public HEInsertTableColumnToLeftContextMenuItem() { }
		public HEInsertTableColumnToLeftContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("inserttablecolumntoleft")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertTableColumnToLeft)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "inserttablecolumntoleft"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToLeft); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableColumnToLeft; }
		}
	}
	public class HEInsertTableColumnToRightContextMenuItem : HtmlEditorContextMenuItem {
		public HEInsertTableColumnToRightContextMenuItem() { }
		public HEInsertTableColumnToRightContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("inserttablecolumntoright")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_InsertTableColumnToRight)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "inserttablecolumntoright"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InsertTableColumnToRight); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertTableColumnToRight; }
		}
	}
	public class HESplitTableCellHorizontalContextMenuItem : HtmlEditorContextMenuItem {
		public HESplitTableCellHorizontalContextMenuItem() { }
		public HESplitTableCellHorizontalContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("splittablecellhorizontally")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_SplitTableCellHorizontal)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "splittablecellhorizontally"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellHorizontal); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.SplitTableCellHorizontal; }
		}
	}
	public class HESplitTableCellVerticalContextMenuItem : HtmlEditorContextMenuItem {
		public HESplitTableCellVerticalContextMenuItem() { }
		public HESplitTableCellVerticalContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("splittablecellvertically")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_SplitTableCellVertical)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "splittablecellvertically"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.SplitTableCellVertical); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.SplitTableCellVertical; }
		}
	}
	public class HEMergeTableCellRightContextMenuItem : HtmlEditorContextMenuItem {
		public HEMergeTableCellRightContextMenuItem() { }
		public HEMergeTableCellRightContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("mergetablecellright")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_MergeTableCellRight)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "mergetablecellright"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellHorizontal); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.MergeTableCellHorizontal; }
		}
	}
	public class HEMergeTableCellDownContextMenuItem : HtmlEditorContextMenuItem {
		public HEMergeTableCellDownContextMenuItem() { }
		public HEMergeTableCellDownContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("mergetablecelldown")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_MergeTableCellDown)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "mergetablecelldown"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.MergeTableCellDown); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.MergeTableCellDown; }
		}
	}
	public class HEDeleteTableContextMenuItem : HtmlEditorContextMenuItem {
		public HEDeleteTableContextMenuItem() { }
		public HEDeleteTableContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("deletetable")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_DeleteTable)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "deletetable"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTable); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.DeleteTable; }
		}
	}
	public class HEDeleteTableRowContextMenuItem : HtmlEditorContextMenuItem {
		public HEDeleteTableRowContextMenuItem() { }
		public HEDeleteTableRowContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("deletetablerow")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_DeleteTableRow)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "deletetablerow"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableRow); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.DeleteTableRow; }
		}
	}
	public class HEDeleteTableColumnContextMenuItem : HtmlEditorContextMenuItem {
		public HEDeleteTableColumnContextMenuItem() { }
		public HEDeleteTableColumnContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("deletetablecolumn")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_DeleteTableColumn)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "deletetablecolumn"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteTableColumn); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.DeleteTableColumn; }
		}
	}
	public class HEChangePlaceholderDialogContextMenuItem : HtmlEditorContextMenuItem {
		public HEChangePlaceholderDialogContextMenuItem() { }
		public HEChangePlaceholderDialogContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("changeplaceholderdialog")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ContextMenu_ChangePlaceholder)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "changeplaceholderdialog"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ContextMenu_ChangePlaceholder); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.InsertPlaceholderDialog; }
		}
	}
	public class HEOutdentContextMenuItem : HtmlEditorContextMenuItem {
		public HEOutdentContextMenuItem()
			: base() { }
		public HEOutdentContextMenuItem(string text)
			: base(text) { }
		[Browsable(false), DefaultValue("outdent")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Outdent)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "outdent"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandOutdent); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Outdent; }
		}
	}
	public class HEIndentContextMenuItem : HtmlEditorContextMenuItem {
		public HEIndentContextMenuItem()
			: base() { }
		public HEIndentContextMenuItem(string text)
			: base(text) { }
		[Browsable(false), DefaultValue("indent")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Indent)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "indent"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandIndent); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Indent; }
		}
	}
	public class HECommentContextMenuItem : HtmlEditorContextMenuItem {
		public HECommentContextMenuItem() { }
		public HECommentContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("comment")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Comment)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "comment"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandComment); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Comment; }
		}
	}
	public class HEUncommentContextMenuItem : HtmlEditorContextMenuItem {
		public HEUncommentContextMenuItem() { }
		public HEUncommentContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("uncomment")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_Uncomment)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "uncomment"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandUncomment); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.Uncomment; }
		}
	}
	public class HEFormatDocumentContextMenuItem : HtmlEditorContextMenuItem {
		public HEFormatDocumentContextMenuItem() { }
		public HEFormatDocumentContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("formatdocument")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_FormatDocument)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "formatdocument"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandFormatDocument); }
		}
		protected override string ResourceImageName {
			get { return HtmlEditorIconImages.FormatDocument; }
		}
	}
	public class HECollapseTagContextMenuItem : HtmlEditorContextMenuItem {
		public HECollapseTagContextMenuItem() { }
		public HECollapseTagContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("collapsetag")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_CollapseTag)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "collapsetag"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandCollapseTag); }
		}
		protected override string ResourceImageName {
			get { return string.Empty; }
		}
	}
	public class HEExpandTagContextMenuItem : HtmlEditorContextMenuItem {
		public HEExpandTagContextMenuItem() { }
		public HEExpandTagContextMenuItem(string text)
			: base(text) {
		}
		[Browsable(false), DefaultValue("expandtag")]
		public new string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[DefaultValue(StringResources.HtmlEditorText_ExpandTag)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultCommandName {
			get { return "expandtag"; }
		}
		protected override string DefaultText {
			get { return ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.CommandExpandTag); }
		}
		protected override string ResourceImageName {
			get { return string.Empty; }
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorContextMenuItemCollection : Collection<HtmlEditorContextMenuItem> {
		protected internal HtmlEditorContextMenuItemCollection()
			: base() {
		}
		public HtmlEditorContextMenuItemCollection(ASPxHtmlEditor owner)
			: base(owner) {
		}
		public HtmlEditorContextMenuItem this[string commandName] {
			get {
				return Find(delegate(HtmlEditorContextMenuItem item) {
					return item.CommandName == commandName;
				});
			}
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorContextMenuItemCollectionOwner")]
#endif
		public new ASPxHtmlEditor Owner { get { return (ASPxHtmlEditor)base.Owner; } }
		public HtmlEditorContextMenuItem Add() {
			return AddInternal(new HtmlEditorContextMenuItem());
		}
		public HtmlEditorContextMenuItem Add(string text) {
			return AddInternal(new HtmlEditorContextMenuItem(text));
		}
		public HtmlEditorContextMenuItem Add(string text, string commandName) {
			return AddInternal(new HtmlEditorContextMenuItem(text, commandName));
		}
		protected override void OnChanged() {
			if(Owner != null)
				(Owner as IWebControlObject).LayoutChanged();
		}
		public void CreateDefaultItems() {
			Add(
				PrepareItem(new HECutContextMenuItem(), true),
				new HECopyContextMenuItem(),
				new HEPasteContextMenuItem(),
				PrepareItem(new HEChangePlaceholderDialogContextMenuItem(), true),
				PrepareItem(new HEChangeLinkDialogContextMenuItem(), true),
				PrepareItem(new HEChangeImageDialogContextMenuItem(), true),
				PrepareItem(new HEChangeAudioDialogContextMenuItem(), true),
				PrepareItem(new HEChangeVideoDialogContextMenuItem(), true),
				PrepareItem(new HEChangeFlashDialogContextMenuItem(), true),
				PrepareItem(new HEChangeYouTubeVideoDialogContextMenuItem(), true),
				PrepareItem(new HETablePropertiesDialogContextMenuItem(), true),
				new HETableRowPropertiesDialogContextMenuItem(),
				new HETableColumnPropertiesDialogContextMenuItem(),
				new HETableCellPropertiesDialogContextMenuItem(),
				PrepareItem(new HEInsertTableRowAboveContextMenuItem(), true),
				new HEInsertTableRowBelowContextMenuItem(),
				new HEInsertTableColumnToLeftContextMenuItem(),
				new HEInsertTableColumnToRightContextMenuItem(),
				PrepareItem(new HESplitTableCellHorizontalContextMenuItem(), true),
				new HESplitTableCellVerticalContextMenuItem(),
				new HEMergeTableCellRightContextMenuItem(),
				new HEMergeTableCellDownContextMenuItem(),
				PrepareItem(new HEDeleteTableContextMenuItem(), true),
				new HEDeleteTableRowContextMenuItem(),
				new HEDeleteTableColumnContextMenuItem(),
				PrepareItem(new HECommentContextMenuItem(), true),
				new HEUncommentContextMenuItem(),
				PrepareItem(new HECollapseTagContextMenuItem(), true),
				new HEExpandTagContextMenuItem(),
				PrepareItem(new HEFormatDocumentContextMenuItem(), true),
				PrepareItem(new HESelectAllContextMenuItem(), true)
			);
		}
		protected HtmlEditorContextMenuItem PrepareItem(HtmlEditorContextMenuItem item, bool beginGroup) {
			item.BeginGroup = beginGroup;
			return item;
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	[ToolboxItem(false)]
	public class HtmlEditorContextMenu : ASPxInternalWebControl {
		public HtmlEditorContextMenu(ASPxHtmlEditor htmlEditor, string id) {
			HtmlEditor = htmlEditor;
			PopupMenuID = id;
		}
		protected ASPxHtmlEditor HtmlEditor { get; private set; }
		protected ASPxHtmlEditorScripts Scripts { get { return HtmlEditor.RenderHelper.Scripts; } }
		protected ASPxPopupMenu PopupMenu { get; private set; }
		protected string PopupMenuID { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			PopupMenu = new HtmlEditorPopupMenu(HtmlEditor);
			PopupMenu.ID = PopupMenuID;
			PopupMenu.EnableViewState = false;
			PopupMenu.EncodeHtml = HtmlEditor.EncodeHtml;
			Controls.Add(PopupMenu);
			CreateItems();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupMenu.ParentStyles = HtmlEditor.StylesContextMenu;
			PopupMenu.GutterWidth = System.Web.UI.WebControls.Unit.Pixel(0);
			PopupMenu.ClientSideEvents.ItemClick += Scripts.ContextMenuItemClickHandleName;
			if(Browser.IsIE)
				PopupMenu.ClientSideEvents.CloseUp += Scripts.ContextMenuCloseUpHandleName;
		}
		protected void CreateItems() {
			HtmlEditorContextMenuItemCollection items = new HtmlEditorContextMenuItemCollection(HtmlEditor);
			items.Assign(HtmlEditor.ContextMenuItems);
			if(items.Count == 0)
				items.CreateDefaultItems();
			foreach(HtmlEditorContextMenuItem item in items) {
				if(item.Visible)
					PopupMenu.Items.Add(CreateMenuItem(item));
			}
		}
		protected MenuItem CreateMenuItem(HtmlEditorContextMenuItem contextMenuItem) {
			MenuItem item = new MenuItem(contextMenuItem.GetText(), contextMenuItem.GetCommandName());
			item.Image.CopyFrom(contextMenuItem.GetImage());
			item.BeginGroup = contextMenuItem.BeginGroup;
			return item;
		}
	}
	[ToolboxItem(false)]
	public class HtmlEditorPopupMenu : ASPxPopupMenu {
		public HtmlEditorPopupMenu(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
			ParentSkinOwner = htmlEditor;
		}
	}
}
