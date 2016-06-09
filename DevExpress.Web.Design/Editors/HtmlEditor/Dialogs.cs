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
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Web.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	[CLSCompliant(false)]
	public class FeatureCheckSpellingPropertiesDialogFrame : MainEmbeddedFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureSpellCheckingPropertiesDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.CheckSpellingDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureTableRowPropertiesDialogFrame : MainEmbeddedFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureTableRowPropertiesDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.TableRowPropertiesDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureTableColumnPropertiesDialogFrame : MainEmbeddedFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureTableColumnPropertiesDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.TableColumnPropertiesDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureTableCellPropertiesDialogFrame : MainEmbeddedFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureTableCellPropertiesDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.TableCellPropertiesDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertPasteFromWordDialogFrame : MainEmbeddedFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertPasteFromWordDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.PasteFromWordDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertTableDialogFrame : MainEmbeddedFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertTableDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.TableDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertLinkDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertLinkDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.LinkDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertYouTubeVideoDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertYouTubeVideoDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.YouTubeVideoDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertImageDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertImageDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.ImageDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertAudioDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertAudioDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.AudioDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertVideoDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertVideoDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.VideoDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureInsertFlashDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureInsertFlashDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.FlashDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public class FeatureChangeElementPropertiesDialogFrame : FeatureHtmlEditorDialogFrame {
		public override Type FeatureBrowserFormBase { get { return typeof(FeatureChangeElementPropertiesDialogForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.HtmlEditor.ChangeElementPropertiesDialog.xml" }; } }
	}
	[CLSCompliant(false)]
	public abstract class FeatureHtmlEditorDialogFrame : FeatureBrowserMainFrameWeb {
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			TreeViewItems.ExpandAll();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertFlashDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override AccessRulesCollection AccessRulesItems { get { return HtmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.PermissionSettings.AccessRules; } }
		protected override FileManagerDetailsColumnCollection DetailViewColumns { get { return HtmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FileListSettings.DetailsViewSettings.Columns; } }
		protected override Collection CssClassItems { get { return HtmlEditor.SettingsDialogs.InsertFlashDialog.CssClassItems; } }
		protected override FileManagerToolbarItemCollection ToolbarItems { get { return HtmlEditor.SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.ToolbarSettings.Items; } }
		protected internal override string GetDependendViewSelectorPropertyName() {
			return "SettingsDialogs.InsertFlashDialog.SettingsFlashSelector.FileListSettings.View";
		}
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertFlashDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureChangeElementPropertiesDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override Collection CssClassItems { get { return HtmlEditor.SettingsDialogs.ChangeElementPropertiesDialog.CssClassItems; } }
		protected override XtraFrame GetGeneralFrame() { return null; }
	}
	[CLSCompliant(false)]
	public class FeatureSpellCheckingPropertiesDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarCheckSpellingButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureTableRowPropertiesDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarTableRowPropertiesDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureTableColumnPropertiesDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarTableColumnPropertiesDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureTableCellPropertiesDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarTableCellPropertiesDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertPasteFromWordDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarPasteFromWordButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertTableDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertTableDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertLinkDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override AccessRulesCollection AccessRulesItems { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.PermissionSettings.AccessRules; } }
		protected override FileManagerDetailsColumnCollection DetailViewColumns { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileListSettings.DetailsViewSettings.Columns; } }
		protected override FileManagerToolbarItemCollection ToolbarItems { get { return HtmlEditor.SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.ToolbarSettings.Items; } }
		protected override Collection CssClassItems { get { return null; } }
		protected internal override string GetDependendViewSelectorPropertyName() {
			return "SettingsDialogs.InsertLinkDialog.SettingsDocumentSelector.FileListSettings.View";
		}
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertLinkDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertYouTubeVideoDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override Collection CssClassItems { get { return HtmlEditor.SettingsDialogs.InsertYouTubeVideoDialog.CssClassItems; } }
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertYouTubeVideoDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertImageDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override AccessRulesCollection AccessRulesItems { get { return HtmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector.PermissionSettings.AccessRules; } }
		protected override FileManagerDetailsColumnCollection DetailViewColumns { get { return HtmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileListSettings.DetailsViewSettings.Columns; } }
		protected override Collection CssClassItems { get { return HtmlEditor.SettingsDialogs.InsertImageDialog.CssClassItems; } }
		protected override FileManagerToolbarItemCollection ToolbarItems { get { return HtmlEditor.SettingsDialogs.InsertImageDialog.SettingsImageSelector.ToolbarSettings.Items; } }
		protected override ItemsEditorOwner CssClassItemsOwner { get { return new FlatCollectionItemsOwner<InsertImageCssClassItem>(HtmlEditor, HtmlEditor.Site, CssClassItems); } }
		protected internal override string GetDependendViewSelectorPropertyName() {
			return "SettingsDialogs.InsertImageDialog.SettingsImageSelector.FileListSettings.View";
		}
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertImageDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertAudioDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override AccessRulesCollection AccessRulesItems { get { return HtmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.PermissionSettings.AccessRules; } }
		protected override FileManagerDetailsColumnCollection DetailViewColumns { get { return HtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FileListSettings.DetailsViewSettings.Columns; } }
		protected override Collection CssClassItems { get { return HtmlEditor.SettingsDialogs.InsertAudioDialog.CssClassItems; } }
		protected override FileManagerToolbarItemCollection ToolbarItems { get { return HtmlEditor.SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.ToolbarSettings.Items; } }
		protected internal override string GetDependendViewSelectorPropertyName() {
			return "SettingsDialogs.InsertAudioDialog.SettingsAudioSelector.FileListSettings.View";
		}
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertVideoDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public class FeatureInsertVideoDialogForm : HtmlEditorToolbarDialogFormBase {
		protected override AccessRulesCollection AccessRulesItems { get { return HtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.PermissionSettings.AccessRules; } }
		protected override FileManagerDetailsColumnCollection DetailViewColumns { get { return HtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FileListSettings.DetailsViewSettings.Columns; } }
		protected override Collection CssClassItems { get { return HtmlEditor.SettingsDialogs.InsertVideoDialog.CssClassItems; } }
		protected override FileManagerToolbarItemCollection ToolbarItems { get { return HtmlEditor.SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.ToolbarSettings.Items; } }
		protected internal override string GetDependendViewSelectorPropertyName() {
			return "SettingsDialogs.InsertVideoDialog.SettingsVideoSelector.FileListSettings.View";
		}
		protected override XtraFrame GetGeneralFrame() {
			return new HtmlEditorToolbarDialogGeneralFrame<ToolbarInsertVideoDialogButton>();
		}
	}
	[CLSCompliant(false)]
	public abstract class HtmlEditorToolbarDialogFormBase : FeatureTabbedViewForm {
		DetailsViewSettingsColumnsEditorFrame columnsEditorEmbeddedFrame;
		ItemsEditorFrame toolbarItemsEditorFrame;
		ItemsEditorFrame permissionSettingsFrame;
		ItemsEditorFrame cssClassItemsFrame;
		XtraFrame generalFrame;
		IServiceProvider Provider { get { return HtmlEditor.Site; } }
		protected ASPxHtmlEditor HtmlEditor { get { return (ASPxHtmlEditor)SourceObject; } }
		protected virtual ItemsEditorOwner AccessRulesOwner {
			get { return new HtmlEditorPermissionSettingsItemsOwner(HtmlEditor, HtmlEditor.Site, AccessRulesItems); }
		}
		protected virtual ItemsEditorOwner DetailsViewColumnsOwner {
			get { return new DetailsViewSettingsColumnsOwner(HtmlEditor, HtmlEditor.Site, DetailViewColumns); }
		}
		protected virtual ItemsEditorOwner CssClassItemsOwner {
			get {
				if(CssClassItems == null) return null;
				return new FlatCollectionItemsOwner<InsertMediaCssClassItem>(HtmlEditor, HtmlEditor.Site, CssClassItems);
			}
		}
		protected virtual FileManagerToolbarItemsOwner ToolbarItemsOwner {
			get {
				if(ToolbarItems == null) return null;
				return new FileManagerToolbarItemsOwner(HtmlEditor, "Toolbar Items", HtmlEditor.Site, ToolbarItems);
			}
		}
		protected virtual AccessRulesCollection AccessRulesItems { get { return null; } }
		protected virtual FileManagerDetailsColumnCollection DetailViewColumns { get { return null; } }
		protected virtual FileManagerToolbarItemCollection ToolbarItems { get { return null; } }
		protected virtual Collection CssClassItems { get { return null; } }
		protected ItemsEditorFrame PermissionSettingsFrame {
			get {
				if(permissionSettingsFrame == null && AccessRulesOwner != null)
					permissionSettingsFrame = new ItemsEditorFrame(AccessRulesOwner);
				return permissionSettingsFrame;
			}
		}
		protected ItemsEditorFrame CssClassItemsFrame {
			get {
				if(cssClassItemsFrame == null && CssClassItemsOwner != null)
					cssClassItemsFrame = new ItemsEditorFrame(CssClassItemsOwner);
				return cssClassItemsFrame;
			}
		}
		protected DetailsViewSettingsColumnsEditorFrame ColumnsEditorEmbeddedFrame {
			get {
				if(columnsEditorEmbeddedFrame == null)
					columnsEditorEmbeddedFrame = new DetailsViewSettingsColumnsEditorFrame((DetailsViewSettingsColumnsOwner)DetailsViewColumnsOwner);
				return columnsEditorEmbeddedFrame;
			}
		}
		protected ItemsEditorFrame ToolbarItemsEditorFrame {
			get {
				if(toolbarItemsEditorFrame == null && ToolbarItemsOwner != null)
					toolbarItemsEditorFrame = new ItemsEditorFrame(ToolbarItemsOwner);
				return toolbarItemsEditorFrame;
			}
		}
		internal XtraFrame GeneralFrame {
			get {
				if(generalFrame == null)
					generalFrame = GetGeneralFrame();
				return generalFrame;
			}
		}
		protected override bool TopPanelVisibility { get { return FeatureId == "FileListSettings_Selector"; } }
		protected abstract XtraFrame GetGeneralFrame();
		protected override PropertyEditorType GetTopLevelPropertyEditorType() {
			return PropertyEditorType.ComboBox;
		}
		protected override void FillPageToFrameAssociator() {
			AddPageToFrameAssociation("General", (IEmbeddedFrame)GeneralFrame);
			AddPageToFrameAssociation("Columns_DetailsView", ColumnsEditorEmbeddedFrame);
			AddPageToFrameAssociation("Toolbar_Items", ColumnsEditorEmbeddedFrame);
			AddPageToFrameAssociation("PermissionSettings_Selector", PermissionSettingsFrame);
			AddPageToFrameAssociation("CssClassItems", CssClassItemsFrame);
		}
		protected override void FillViewInfoSelector() {
			ViewsSelector.AddViewSelectorInfoItem("Details", new string[] { "Main_DetailsView", "Columns_DetailsView" }, FileListView.Details);
			ViewsSelector.AddViewSelectorInfoItem("Thumbnails", new string[] { "Main_ThumbnailsView" }, FileListView.Thumbnails);
		}
	}
	[CLSCompliant(false)]
	public class HtmlEditorToolbarDialogGeneralFrame<T> : FeatureBrowserDefaultPageDescriptions where T : HtmlEditorToolbarItem {
		PanelControl topPanel;
		Font elementsFont;
		Font underlineFont;
		List<Pair<int, List<Control>>> ensureDescriptionElementHeights;
		HtmlEditorDialogItemOwner<T> toolbarItemOwner;
		protected virtual new bool IsPropertyGridVisible { get { return true; } }
		protected virtual bool IsAllowDialog { get { return ToolbarItemOwner.GetAllowItemType(); } }
		CheckEdit CheckBoxAllowProperty { get; set; }
		internal HtmlEditorDialogItemOwner<T> ToolbarItemOwner {
			get {
				var htmlEditor = (ASPxHtmlEditor)((HtmlEditorToolbarDialogFormBase)FrameOwner).SourceObject;
				if(toolbarItemOwner == null)
					toolbarItemOwner = new HtmlEditorDialogItemOwner<T>(htmlEditor);
				return toolbarItemOwner;
			}
		}
		PanelControl TopPanel {
			get {
				if(topPanel == null) {
					topPanel = new PanelControl() { Name = "TopPanel" };
					topPanel.Parent = this;
					topPanel.Dock = DockStyle.Top;
					topPanel.BorderStyle = BorderStyles.NoBorder;
				}
				return topPanel;
			}
		}
		Font ElementsFont {
			get {
				if(elementsFont == null)
					elementsFont = new Font(Font.FontFamily, 9.5f);
				return elementsFont;
			}
		}
		Font UnderlineFont {
			get {
				if(underlineFont == null)
					underlineFont = new Font(Font.FontFamily, 9.5f, FontStyle.Underline);
				return underlineFont;
			}
		}
		List<Pair<int, List<Control>>> EnsureDescriptionElementRows {
			get {
				if(ensureDescriptionElementHeights == null)
					ensureDescriptionElementHeights = new List<Pair<int, List<Control>>>();
				return ensureDescriptionElementHeights;
			}
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			pgMain.Visible = IsPropertyGridVisible;
			Load += (s, e) => {
				GenerateLinkEnsureText();
				TopPanel.SizeChanged += (s0, e0) => { OnTopPanelSizeChanged(); };
				InitializeCheckBoxToolbarItemAllowProperty();
				UpdateTopPanelElementsLocation();
			};
		}
		void GenerateLinkEnsureText(bool regenerate = false) {
			TopPanel.SuspendLayout();
			if(regenerate)
				EnsureDescriptionElementRows.ForEach(k => k.Second.ForEach(c => { c.Visible = false; c.Parent = null; }));
			ToolbarItemOwner.GetItemTypeEnsureTextValues().ForEach(t => AddEnsureDescriptionControlElementRow(CreateEnsureDescriptionTextControl(10, t)));
			UpdateTopPanelElementsLocation();
			TopPanel.ResumeLayout();
		}
		Control CreateEnsureDescriptionTextControl(int left, TextElement textElement) {
			Control textControl = null;
			switch(textElement.ElementType) {
				case TextElementType.Label:
				case TextElementType.ParaLabel:
					textControl = new LabelControl();
					textControl.Font = ElementsFont;
					break;
				case TextElementType.HyperLink:
				case TextElementType.ParaLink:
					textControl = new HyperLinkEdit();
					textControl.Click += (s, e) => { OnLinkEnsureItemClick(); };
					textControl.Font = UnderlineFont;
					break;
				default:
					return textControl;
			}
			var baseControl = (BaseControl)textControl;
			baseControl.BorderStyle = BorderStyles.NoBorder;
			textControl.Parent = TopPanel;
			textControl.Left = left;
			textControl.Text = textElement.Text;
			textControl.Width = DesignTimeFormHelper.GetTextSize(textControl, textControl.Text, textControl.Font).Width;
			textControl.Tag = textElement.NewLineElement;
			return textControl;
		}
		void AddEnsureDescriptionControlElementRow(Control control) {
			Pair<int, List<Control>> row = null;
			var rowIndex = 0;
			if(Convert.ToBoolean(control.Tag)) {
				row = EnsureDescriptionElementRows.FirstOrDefault(r => r.Second.Count == 0);
				if(row == null)
					row = GetEnsureDescriptionElementRow(EnsureDescriptionElementRows.Count);
				rowIndex = EnsureDescriptionElementRows.IndexOf(row);
				AddEnsureDescriptionControlElementRowCore(row, control, rowIndex);
				return;
			}
			rowIndex = (int)(control.Width / TopPanel.Width);
			if(control.Width > TopPanel.Width) {
				var prevRow = GetEnsureDescriptionElementRow(rowIndex - 1);
				if(prevRow.Second.Count == 0) {
					row = prevRow;
					--rowIndex;
				}
			} else {
				row = GetEnsureDescriptionElementRow(rowIndex);
			}
			AddEnsureDescriptionControlElementRowCore(row, control, rowIndex);
		}
		void AddEnsureDescriptionControlElementRowCore(Pair<int, List<Control>> row, Control control, int rowIndex) {
			SetEnsureDescriptionElementLocation(control, row.First, rowIndex * 20);
			row.First += control.Width;
			row.Second.Add(control);
		}
		Pair<int, List<Control>> GetEnsureDescriptionElementRow(int rowIndex) {
			if(rowIndex < 0)
				return null;
			if(EnsureDescriptionElementRows.Count > rowIndex)
				return EnsureDescriptionElementRows[rowIndex];
			return InsertNewEnsureDescriptionRow(rowIndex);
		}
		Pair<int, List<Control>> InsertNewEnsureDescriptionRow(int insertBeforeIndex) {
			var row = new Pair<int, List<Control>>(10, new List<Control>());
			if(EnsureDescriptionElementRows.Count <= insertBeforeIndex) {
				EnsureDescriptionElementRows.Add(row);
			} else {
				EnsureDescriptionElementRows.Insert(insertBeforeIndex, row);
				for(var i = insertBeforeIndex + 1; i < EnsureDescriptionElementRows.Count; ++i)
					EnsureDescriptionElementRows[i].Second.ForEach(e => e.Top += 22);
			}
			return row;
		}
		protected void OnTopPanelSizeChanged() {
			TopPanel.SuspendLayout();
			UpdateEnsureElementRowLocation(0);
			UpdateTopPanelElementsLocation();
			TopPanel.ResumeLayout();
		}
		void UpdateTopPanelElementsLocation() { 
			var height = EnsureDescriptionElementRows.Count(r => r.Second.Exists(c => c.Visible)) * 26;
			if(CheckBoxAllowProperty != null && CheckBoxAllowProperty.Visible) {
				CheckBoxAllowProperty.Top = height;
				height += 26;
			}
			TopPanel.Height = height;
		}
		void UpdateEnsureElementRowLocation(int rowIndex) {
			if(rowIndex >= EnsureDescriptionElementRows.Count)
				return;
			var rowElements = EnsureDescriptionElementRows[rowIndex];
			if(rowElements == null)
				return;
			if(rowElements.Second.Count == 0) {
				UpdateEnsureElementRowLocation(++rowIndex);
				return;
			}
			var sourceControl = rowElements.Second.Last();
			var nextRowIndex = rowIndex + 1;
			if(!Convert.ToBoolean(sourceControl.Tag)) {
				if(rowElements.First > TopPanel.Width && rowElements.Second.Count != 1)
					nextRowIndex = ExtendEnsureDescriptionControl(rowElements, rowIndex, sourceControl);
				else if(rowElements.First < TopPanel.Width)
					nextRowIndex = SqueezeEnsureDescriptionControl(rowElements, rowIndex, sourceControl);
			} else {
				nextRowIndex = ProcessLocationForEnsureDescriptionParaElement(rowIndex, sourceControl);
			}
			UpdateEnsureElementRowLocation(nextRowIndex);
		}
		int ProcessLocationForEnsureDescriptionParaElement(int rowIndex, Control control) {
			var nextRowIndex = rowIndex + 1;
			if(rowIndex == 0) {
				SetEnsureDescriptionElementLocation(control, 10, rowIndex * 22);
				return nextRowIndex;
			}
			var prevRow = EnsureDescriptionElementRows[rowIndex - 1];
			if(prevRow.Second.Count == 0) {
				prevRow.Second.Add(control);
				prevRow.First += control.Width;
				var currentRow = EnsureDescriptionElementRows[rowIndex];
				currentRow.First -= control.Width;
				currentRow.Second.Remove(control);
				SetEnsureDescriptionElementLocation(control, 10, (rowIndex - 1) * 22);
				nextRowIndex = rowIndex;
			}
			return nextRowIndex;
		}
		int ExtendEnsureDescriptionControl(Pair<int, List<Control>> rowElements, int rowIndex, Control sourceControl) {
			var targetRow = GetOrPrepareRowForEnsureDescriptionTextElement(++rowIndex);
			var controlWidth = sourceControl.Width;
			SetEnsureDescriptionElementLocation(sourceControl, 10, rowIndex * 22);
			rowElements.First -= controlWidth;
			rowElements.Second.Remove(sourceControl);
			targetRow.First += controlWidth;
			targetRow.Second.ForEach(e => e.Left += controlWidth);
			targetRow.Second.Insert(0, sourceControl);
			return rowElements.First > TopPanel.Width ? rowIndex - 1 : rowIndex;
		}
		Pair<int, List<Control>> GetOrPrepareRowForEnsureDescriptionTextElement(int rowIndex) { 
			var result = GetEnsureDescriptionElementRow(rowIndex);
			if(result.Second.Count == 0)
				return result;
			var lastControl = result.Second.Last();
			if(lastControl != null && Convert.ToBoolean(lastControl.Tag))
				result = InsertNewEnsureDescriptionRow(rowIndex);
			return result;
		}
		int SqueezeEnsureDescriptionControl(Pair<int, List<Control>> rowElements, int rowIndex, Control sourceControl) {
			var containerWidth = TopPanel.Width;
			var controlWidth = sourceControl.Width;
			sourceControl = rowElements.Second.First();
			controlWidth = sourceControl.Width;
			var prevRowIndex = rowIndex - 1;
			var nextRowIndex = rowIndex + 1;
			var targetRow = GetPreviousEnsureDescriptionRow(prevRowIndex);
			if(targetRow != null && (targetRow.First + controlWidth) < containerWidth) {
				var currentRow = GetEnsureDescriptionElementRow(rowIndex);
				currentRow.Second.Remove(sourceControl);
				currentRow.Second.ForEach(c => c.Left -= controlWidth);
				currentRow.First -= controlWidth;
				SetEnsureDescriptionElementLocation(sourceControl, targetRow.First, prevRowIndex * 22);
				targetRow.First += controlWidth;
				targetRow.Second.Add(sourceControl);
				nextRowIndex = (targetRow.First + controlWidth) < containerWidth ? rowIndex : nextRowIndex;
			}
			return nextRowIndex;
		}
		Pair<int, List<Control>> GetPreviousEnsureDescriptionRow(int prevRowIndex) { 
			var result = GetEnsureDescriptionElementRow(prevRowIndex);
			if(result == null || result.Second.Count == 0)
				return result;
			if(Convert.ToBoolean(result.Second.Last().Tag))
				return null;
			return result;
		}
		void SetEnsureDescriptionElementLocation(Control control, int left, int top) {
			var isHyperLink = control is HyperLinkEdit;
			control.Left = isHyperLink ? left - 2 : left;
			control.Top = isHyperLink ? top - 2 : top;
		}
		void InitializeCheckBoxToolbarItemAllowProperty() {
			if(!ToolbarItemOwner.GetItemTypeHasAllowProperty())
				return;
			TopPanel.Height = TopPanel.Height + 30;
			CheckBoxAllowProperty = new CheckEdit() { Name = "CheckBoxAllowProperty" };
			CheckBoxAllowProperty.Font = ElementsFont;
			CheckBoxAllowProperty.Parent = TopPanel;
			CheckBoxAllowProperty.Text = ToolbarItemOwner.GetToolbarItemAllowPropertyCaption();
			CheckBoxAllowProperty.Width = DesignTimeFormHelper.GetTextWidth(this, CheckBoxAllowProperty.Text, CheckBoxAllowProperty.Font) + 10;
			CheckBoxAllowProperty.Left = 10;
			CheckBoxAllowProperty.Top = (CheckBoxAllowProperty.Parent.Height * 5) / 8 - CheckBoxAllowProperty.Height / 2;
			CheckBoxAllowProperty.Checked = IsAllowDialog;
			CheckBoxAllowProperty.CheckedChanged += (s, e) => { OnAllowDialogsCheckChanged(CheckBoxAllowProperty); };
		}
		protected virtual void OnAllowDialogsCheckChanged(CheckEdit checkBox) {
			ToolbarItemOwner.SetItemTypeAllowPropertyValue(checkBox.Checked);
		}
		protected virtual void OnLinkEnsureItemClick() {
			ToolbarItemOwner.InsertItemToFirstToolbar();
			GenerateLinkEnsureText(true);
		}
		class Pair<D, U> {
			public Pair() { }
			public Pair(D first, U second) {
				this.First = first;
				this.Second = second;
			}
			public D First { get; set; }
			public U Second { get; set; }
		};
	}
	public class HtmlEditorDialogItemOwner<T> where T : HtmlEditorToolbarItem {
		HtmlEditorToolbarItemsOwner toolbarItemsOwner;
		Dictionary<string, ToolbarItemAllowPropertyDescriptor> typesWithAllowDialogSetting;
		Dictionary<Type, Type> toolbarModeItemTypeAssociations;
		Dictionary<Type, KeyValuePair<Type, Type>> commandItemsPaths;
		Type itemType;
		public HtmlEditorDialogItemOwner(ASPxHtmlEditor htmlEditor) {
			HtmlEditor = htmlEditor;
			FillTypesWithAllowDialogSettings();
			FillCommandItemMap();
		}
		public ASPxHtmlEditor HtmlEditor { get; private set; }
		bool IsToolbarRibbonMode { get { return HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Ribbon || HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon; } }
		bool IsToolbarMenuMode { get { return HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Menu || HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.None; } }
		Dictionary<Type, KeyValuePair<Type, Type>> CommandItemsPaths {
			get {
				if(commandItemsPaths == null)
					commandItemsPaths = new Dictionary<Type, KeyValuePair<Type, Type>>();
				return commandItemsPaths;
			}
		}
		Type ItemType {
			get {
				if(itemType == null) {
					if(IsToolbarMenuMode)
						itemType = typeof(T);
					else
						itemType = ToolbarModeItemTypeAssociations[typeof(T)];
				}
				return itemType;
			}
		}
		HtmlEditorToolbarItemsOwner ToolbarItemsOwner {
			get {
				if(toolbarItemsOwner == null)
					toolbarItemsOwner = new HtmlEditorToolbarItemsOwner(HtmlEditor, HtmlEditor.Site);
				return toolbarItemsOwner;
			}
		}
		Dictionary<string, ToolbarItemAllowPropertyDescriptor> TypesWithAllowDialogSetting {
			get {
				if(typesWithAllowDialogSetting == null)
					typesWithAllowDialogSetting = new Dictionary<string, ToolbarItemAllowPropertyDescriptor>();
				return typesWithAllowDialogSetting;
			}
		}
		Dictionary<Type, Type> ToolbarModeItemTypeAssociations {
			get {
				if(toolbarModeItemTypeAssociations == null)
					toolbarModeItemTypeAssociations = new Dictionary<Type, Type>() {
						{typeof(ToolbarInsertFlashDialogButton), typeof(HEInsertFlashDialogRibbonCommand)},
						{typeof(ToolbarCheckSpellingButton), typeof(HECheckSpellingRibbonCommand)},
						{typeof(ToolbarTableRowPropertiesDialogButton), typeof(HETableRowPropertiesRibbonCommand)},
						{typeof(ToolbarTableColumnPropertiesDialogButton), typeof(HETableColumnPropertiesRibbonCommand)},
						{typeof(ToolbarTableCellPropertiesDialogButton), typeof(HETableCellPropertiesRibbonCommand)},
						{typeof(ToolbarPasteFromWordButton), typeof(HEPasteFromWordRibbonCommand)},
						{typeof(ToolbarInsertTableDialogButton), typeof(HEInsertTableRibbonCommand)}, 
						{typeof(ToolbarInsertLinkDialogButton), typeof(HEInsertLinkDialogRibbonCommand)},
						{typeof(ToolbarInsertYouTubeVideoDialogButton), typeof(HEInsertYouTubeVideoDialogRibbonCommand)},
						{typeof(ToolbarInsertImageDialogButton), typeof(HEInsertImageRibbonCommand)},
						{typeof(ToolbarInsertVideoDialogButton), typeof(HEInsertVideoDialogRibbonCommand)},
						{typeof(ToolbarInsertPlaceholderDialogButton), typeof(HEInsertPlaceholderDialogRibbonCommand)}
					};
				return toolbarModeItemTypeAssociations;
			}
		}
		public bool GetItemTypeHasAllowProperty() {
			return !string.IsNullOrEmpty(GetItemTypeAllowProperty());
		}
		public virtual bool GetAllowItemType() {
			var allowPropertyName = GetItemTypeAllowProperty();
			return !string.IsNullOrEmpty(allowPropertyName) ? Convert.ToBoolean(FeatureBrowserHelper.GetPropertyValue(HtmlEditor, allowPropertyName)) : false;
		}
		public List<TextElement> GetItemTypeEnsureTextValues() {
			if(FindElementTypeInToolbar(ItemType))
				return new List<TextElement>();
			var toolbarTypeName = IsToolbarMenuMode ? "toolbar" : "ribbon";
			var itemsOwner = IsToolbarMenuMode ? (ItemsEditorOwner)new HtmlEditorToolbarItemsOwner(HtmlEditor, HtmlEditor.Site) : new HtmlEditorRibbonItemsOwner(HtmlEditor, HtmlEditor.Site, HtmlEditor.RibbonTabs);
			return ExpandLabelToolbarItemDescriptor(string.Format("<Label>These </Label><Label>settings </Label><Label>are </Label><Label>in </Label><Label>effect </Label><Label>for </Label><Label>the </Label><Label>dialog </Label><Label>invoked </Label>by the <Label>'{0}' </Label><Label>{1} item, </Label><Label>however </Label><Label>this </Label><Label>item </Label><Label>is not </Label><Label>yet added </Label><Label>to the toolbar.</Label><ParaLink>Click here to add the corresponding toolbar item</ParaLink>",
				itemsOwner.GetDesignTimeItemText(ItemType), toolbarTypeName));
		}
		protected List<TextElement> ExpandLabelToolbarItemDescriptor(string text) {
			var result = new List<TextElement>();
			var tags = Enum.GetNames(typeof(TextElementType));
			foreach(var tag in tags) {
				var regex = new Regex(string.Format(@"<{0}>(.*?)</{0}>", tag));
				var matches = regex.Matches(text);
				foreach(Match match in matches) {
					if(match.Length > 0) {
						var typeElement = (TextElementType)Enum.Parse(typeof(TextElementType), tag);
						result.Add(new TextElement(typeElement, match.Groups[match.Groups.Count - 1].Value));
					}
				}
			}
			return result;
		}
		public bool FindElementTypeInToolbar(Type element) {
			if(IsToolbarMenuMode) {
				if(!HtmlEditor.Toolbars.IsEmpty)
					return HtmlEditor.Toolbars.FirstOrDefault(t => t.Items.FirstOrDefault(i => i.GetType() == element) != null) != null;
			} else {
				if(!HtmlEditor.RibbonTabs.IsEmpty)
					return HtmlEditor.RibbonTabs.Any(t => t.Groups.Any(g => g.Items.Any(i => i.GetType() == element)));
			}
			return false;
		}
		public void InsertItemToFirstToolbar() {
			if(IsToolbarMenuMode)
				InsertItemToFirstMenuToolbar(ItemType);
			else if(IsToolbarRibbonMode)
				InsertItemToFirstRibbonTab(ItemType);
		}
		public void SetItemTypeAllowPropertyValue(bool allow) {
			var allowPropertyName = GetItemTypeAllowProperty();
			if(!string.IsNullOrEmpty(allowPropertyName))
				FeatureBrowserHelper.SetPropertyValue(HtmlEditor, allowPropertyName, allow);
		}
		public string GetToolbarItemAllowPropertyCaption() {
			var key = GetItemTypeAllowProperty();
			return !string.IsNullOrEmpty(key) ? TypesWithAllowDialogSetting[key].Caption : string.Empty;
		}
		string GetItemTypeAllowProperty() {
			foreach(var key in TypesWithAllowDialogSetting.Keys) {
				var type = TypesWithAllowDialogSetting[key].ToList().FirstOrDefault(t => t == ItemType);
				if(type != null)
					return key;
			}
			return string.Empty;
		}
		protected virtual void FillTypesWithAllowDialogSettings() {
			if(HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Menu) {
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowHTML5MediaElements", "Allow HTML5 Media Elements", typeof(ToolbarInsertVideoDialogButton));
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowHTML5MediaElements", "Allow HTML5 Media Elements", typeof(ToolbarInsertAudioDialogButton));
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowObjectAndEmbedElements", "Allow Object and Embedded elements", typeof(ToolbarInsertFlashDialogButton));
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowYouTubeVideoIFrames", "Allow YouTube video IFrame", typeof(ToolbarInsertYouTubeVideoDialogButton));
			} else if(IsToolbarRibbonMode) {
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowHTML5MediaElements", "Allow HTML5 Media Elements", typeof(HEInsertVideoDialogRibbonCommand));
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowHTML5MediaElements", "Allow HTML5 Media Elements", typeof(HEInsertAudioDialogRibbonCommand));
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowObjectAndEmbedElements", "Allow Object and Embedded elements", typeof(HEInsertFlashDialogRibbonCommand));
				AddToolbarItemTypeAllowProperty("SettingsHtmlEditing.AllowYouTubeVideoIFrames", "Allow YouTube video IFrame", typeof(HEInsertYouTubeVideoDialogRibbonCommand));
			}
		}
		void AddToolbarItemTypeAllowProperty(string propertyName, string caption, Type itemType) {
			ToolbarItemAllowPropertyDescriptor descriptor = null;
			if(!TypesWithAllowDialogSetting.ContainsKey(propertyName)) {
				descriptor = new ToolbarItemAllowPropertyDescriptor(caption);
				TypesWithAllowDialogSetting[propertyName] = descriptor;
			} else {
				descriptor = TypesWithAllowDialogSetting[propertyName];
			}
			descriptor.Add(itemType);
		}
		void FillCommandItemMap() {
			AddCommandItemPath(typeof(HEPasteFromWordRibbonCommand), typeof(HEHomeRibbonTab), typeof(HEClipboardRibbonGroup));
			AddCommandItemPath(typeof(HECheckSpellingRibbonCommand), typeof(HEReviewRibbonTab), typeof(HESpellingRibbonGroup));
			AddCommandItemPath(typeof(HEInsertTableRibbonCommand), typeof(HEInsertRibbonTab), typeof(HETablesRibbonGroup));
			AddCommandItemPath(typeof(HEInsertLinkDialogRibbonCommand), typeof(HEInsertRibbonTab), typeof(HELinksRibbonGroup));
			AddCommandItemPath(typeof(HEInsertImageRibbonCommand), typeof(HEInsertRibbonTab), typeof(HEImagesRibbonGroup));
			AddCommandItemPath(typeof(HEInsertVideoDialogRibbonCommand), typeof(HEInsertRibbonTab), typeof(HEMediaRibbonGroup));
			AddCommandItemPath(typeof(HEInsertAudioDialogRibbonCommand), typeof(HEInsertRibbonTab), typeof(HEMediaRibbonGroup));
			AddCommandItemPath(typeof(HEInsertFlashDialogRibbonCommand), typeof(HEInsertRibbonTab), typeof(HEMediaRibbonGroup));
			AddCommandItemPath(typeof(HETableCellPropertiesRibbonCommand), typeof(HEInsertRibbonTab), typeof(HETablePropertiesRibbonGroup));
			AddCommandItemPath(typeof(HEInsertYouTubeVideoDialogRibbonCommand), typeof(HEInsertRibbonTab), typeof(HEMediaRibbonGroup));
			AddCommandItemPath(typeof(HETableRowPropertiesRibbonCommand), typeof(HETableRibbonTab), typeof(HETablePropertiesRibbonGroup));
			AddCommandItemPath(typeof(HETableColumnPropertiesRibbonCommand), typeof(HETableRibbonTab), typeof(HETablePropertiesRibbonGroup));
			AddCommandItemPath(typeof(HEInsertPlaceholderDialogRibbonCommand), typeof(HETableRibbonTab), typeof(RibbonGroup));
		}
		void AddCommandItemPath(Type itemType, Type tabType, Type groupType) {
			CommandItemsPaths[itemType] = new KeyValuePair<Type, Type>(tabType, groupType);
		}
		void InsertItemToFirstMenuToolbar(Type itemType) {
			if(HtmlEditor.Toolbars.IsEmpty)
				HtmlEditor.CreateDefaultToolbars(false);
			HtmlEditor.Toolbars.First().Items.Add((HtmlEditorToolbarItem)Activator.CreateInstance(itemType));
		}
		void InsertItemToFirstRibbonTab(Type itemType) {
			if(HtmlEditor.RibbonTabs.IsEmpty)
				HtmlEditor.CreateDefaultRibbonTabs(false);
			var item = (HERibbonCommandBase)Activator.CreateInstance(itemType);
			var tab = GetDefaultRibbonTab(itemType);
			var group = GetDefaultRibbonGroup(tab, itemType);
			if(group != null && !group.Items.Any(i => i.GetType() == itemType))
				group.Items.Add(item);
		}
		RibbonTab GetDefaultRibbonTab(Type itemType) {
			if(!CommandItemsPaths.ContainsKey(itemType))
				return null;
			var tabType = CommandItemsPaths[itemType].Key;
			var tab = HtmlEditor.RibbonTabs.FirstOrDefault(t => t.GetType() == tabType);
			if(tab == null) {
				tab = (RibbonTab)Activator.CreateInstance(tabType);
				HtmlEditor.RibbonTabs.Add(tab);
			}
			return tab;
		}
		RibbonGroup GetDefaultRibbonGroup(RibbonTab tab, Type itemType) {
			if(tab == null || !CommandItemsPaths.ContainsKey(itemType))
				return null;
			var groupType = CommandItemsPaths[itemType].Value;
			var group = tab.Groups.FirstOrDefault(g => g.GetType() == groupType);
			if(group == null) {
				group = (RibbonGroup)Activator.CreateInstance(groupType);
				tab.Groups.Add(group);
			}
			return group;
		}
		public class ToolbarItemAllowPropertyDescriptor : List<Type> {
			public ToolbarItemAllowPropertyDescriptor(string caption) {
				Caption = caption;
			}
			public string Caption { get; private set; }
		}
	}
	public enum TextElementType { Label, ParaLabel, HyperLink, ParaLink };
	public class TextElement {
		public TextElement(TextElementType type, string text) {
			ElementType = type;
			Text = text;
		}
		public TextElementType ElementType { get; private set; }
		public string Text { get; private set; }
		public bool NewLineElement { get { return ElementType == TextElementType.ParaLink || ElementType == TextElementType.ParaLabel; } }
	}
}
