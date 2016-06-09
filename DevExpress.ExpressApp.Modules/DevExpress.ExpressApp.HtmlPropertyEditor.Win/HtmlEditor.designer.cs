#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

namespace DevExpress.ExpressApp.HtmlPropertyEditor.Win
{
	partial class HtmlEditor
	{
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HtmlEditor));
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.bar1 = new DevExpress.XtraBars.Bar();
			this.Bold = new DevExpress.XtraBars.BarButtonItem();
			this.Italic = new DevExpress.XtraBars.BarButtonItem();
			this.Underline = new DevExpress.XtraBars.BarButtonItem();
			this.JustifyLeft = new DevExpress.XtraBars.BarButtonItem();
			this.JustifyCenter = new DevExpress.XtraBars.BarButtonItem();
			this.JustifyRight = new DevExpress.XtraBars.BarButtonItem();
			this.JustifyFull = new DevExpress.XtraBars.BarButtonItem();
			this.Forecolor = new DevExpress.XtraBars.BarButtonItem();
			this.forecolorPopupControl = new ColorPopupControl();
			this.backColor = new DevExpress.XtraBars.BarButtonItem();
			this.backcolorPopupControl = new ColorPopupControl();
			this.FontSelector = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemFontEdit = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.FontSize = new DevExpress.XtraBars.BarEditItem();
			this.Styles = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemFontSize = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.repositoryItemStyles = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.bar2 = new DevExpress.XtraBars.Bar();
			this.Cut = new DevExpress.XtraBars.BarButtonItem();
			this.Copy = new DevExpress.XtraBars.BarButtonItem();
			this.Paste = new DevExpress.XtraBars.BarButtonItem();
			this.Undo = new DevExpress.XtraBars.BarButtonItem();
			this.Redo = new DevExpress.XtraBars.BarButtonItem();
			this.RemoveFormat = new DevExpress.XtraBars.BarButtonItem();
			this.Subscript = new DevExpress.XtraBars.BarButtonItem();
			this.Superscript = new DevExpress.XtraBars.BarButtonItem();
			this.InsertOrderedList = new DevExpress.XtraBars.BarButtonItem();
			this.InsertUnorderedList = new DevExpress.XtraBars.BarButtonItem();
			this.Indent = new DevExpress.XtraBars.BarButtonItem();
			this.Outdent = new DevExpress.XtraBars.BarButtonItem();
			this.CreateLink = new DevExpress.XtraBars.BarButtonItem();
			this.Unlink = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.StrikeThrough = new DevExpress.XtraBars.BarButtonItem();
			this.InsertImage = new DevExpress.XtraBars.BarButtonItem();
			this.Color = new DevExpress.XtraBars.BarButtonItem();
			this.repositoryItemForecolorEdit = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.repositoryItemBackcolorEdit = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.designPage = new DevExpress.XtraTab.XtraTabPage();
			this.htmlPage = new DevExpress.XtraTab.XtraTabPage();
			this.browserControl = new WebBrowserEx();
			this.memo = new DevExpress.XtraEditors.MemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.forecolorPopupControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.backcolorPopupControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemStyles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemForecolorEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBackcolorEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.designPage.SuspendLayout();
			this.htmlPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.memo.Properties)).BeginInit();
			this.SuspendLayout();
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.bar1,
			this.bar2});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.Bold,
			this.Italic,
			this.Underline,
			this.StrikeThrough,
			this.JustifyLeft,
			this.JustifyCenter,
			this.JustifyRight,
			this.JustifyFull,
			this.FontSelector,
			this.FontSize,
			this.Styles,
			this.Cut,
			this.Copy,
			this.Paste,
			this.Undo,
			this.Redo,
			this.RemoveFormat,
			this.Superscript,
			this.Subscript,
			this.InsertOrderedList,
			this.InsertUnorderedList,
			this.Indent,
			this.Outdent,
			this.CreateLink,
			this.Unlink,
			this.InsertImage,
			this.Color,
			this.Forecolor,
			this.backColor});
			this.barManager.MaxItemId = 30;
			this.barManager.HideBarsWhenMerging = false; 
			this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemForecolorEdit,
			this.repositoryItemFontEdit,
			this.repositoryItemBackcolorEdit,
			this.repositoryItemFontSize,
			this.repositoryItemStyles
			});
			this.bar1.BarName = "Text Formating";
			this.bar1.DockCol = 0;
			this.bar1.DockRow = 0;
			this.bar1.OptionsBar.UseWholeRow = true;
			this.bar1.OptionsBar.AllowQuickCustomization = false;
			this.bar1.OptionsBar.AllowDelete = false;
			this.bar1.OptionsBar.AllowCollapse = false;
			this.bar1.OptionsBar.DrawDragBorder = false;
			this.bar1.OptionsBar.DrawSizeGrip = false;
			this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Bold, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Italic, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Underline, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.StrikeThrough, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.JustifyLeft, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.JustifyCenter, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.JustifyRight, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.JustifyFull, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(((DevExpress.XtraBars.BarLinkUserDefines)((DevExpress.XtraBars.BarLinkUserDefines.PaintStyle | DevExpress.XtraBars.BarLinkUserDefines.Width))), this.Forecolor, "", true, true, true, 20, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.backColor, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(((DevExpress.XtraBars.BarLinkUserDefines)((DevExpress.XtraBars.BarLinkUserDefines.PaintStyle | DevExpress.XtraBars.BarLinkUserDefines.Width))), this.FontSelector, "", true, true, true, 139, null, DevExpress.XtraBars.BarItemPaintStyle.Caption),
			new DevExpress.XtraBars.LinkPersistInfo(((DevExpress.XtraBars.BarLinkUserDefines)((DevExpress.XtraBars.BarLinkUserDefines.PaintStyle | DevExpress.XtraBars.BarLinkUserDefines.Width))), this.FontSize, "", false, true, true, 90, null, DevExpress.XtraBars.BarItemPaintStyle.Caption),
			new DevExpress.XtraBars.LinkPersistInfo(((DevExpress.XtraBars.BarLinkUserDefines)((DevExpress.XtraBars.BarLinkUserDefines.PaintStyle | DevExpress.XtraBars.BarLinkUserDefines.Width))), this.Styles, "", false, true, true, 90, null, DevExpress.XtraBars.BarItemPaintStyle.Caption)});			
			this.Bold.Glyph = ((System.Drawing.Image)(resources.GetObject("Bold.Glyph")));
			this.Bold.Id = 0;
			this.Bold.Name = "Bold";
			this.Bold.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
			this.Bold.Tag = "Bold";
			this.Bold.Appearance.Options.UseFont = true;
			this.Bold.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Italic.Glyph = ((System.Drawing.Image)(resources.GetObject("Italic.Glyph")));
			this.Italic.Id = 1;
			this.Italic.Name = "Italic";
			this.Italic.Tag = "Italic";
			this.Italic.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Underline.Glyph = ((System.Drawing.Image)(resources.GetObject("Underline.Glyph")));
			this.Underline.Id = 2;
			this.Underline.Name = "Underline";
			this.Underline.Appearance.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.World);
			this.Underline.Tag = "Underline";
			this.Underline.Appearance.Options.UseFont = true;
			this.Underline.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.JustifyLeft.Glyph = ((System.Drawing.Image)(resources.GetObject("JustifyLeft.Glyph")));
			this.JustifyLeft.Id = 4;
			this.JustifyLeft.Name = "JustifyLeft";
			this.JustifyLeft.Tag = "JustifyLeft";
			this.JustifyLeft.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.JustifyCenter.Glyph = ((System.Drawing.Image)(resources.GetObject("JustifyCenter.Glyph")));
			this.JustifyCenter.Id = 5;
			this.JustifyCenter.Name = "JustifyCenter";
			this.JustifyCenter.Tag = "JustifyCenter";
			this.JustifyCenter.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.JustifyRight.Glyph = ((System.Drawing.Image)(resources.GetObject("JustifyRight.Glyph")));
			this.JustifyRight.Id = 6;
			this.JustifyRight.Name = "JustifyRight";
			this.JustifyRight.Tag = "JustifyRight";
			this.JustifyRight.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.JustifyFull.Glyph = ((System.Drawing.Image)(resources.GetObject("JustifyFull.Glyph")));
			this.JustifyFull.Hint = "Justify Full";
			this.JustifyFull.Id = 8;
			this.JustifyFull.Name = "JustifyFull";
			this.JustifyFull.Tag = "JustifyFull";
			this.JustifyFull.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Forecolor.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.Forecolor.DropDownControl = this.forecolorPopupControl;
			this.Forecolor.Glyph = ((System.Drawing.Image)(resources.GetObject("Forecolor.Glyph")));
			this.Forecolor.Id = 28;
			this.Forecolor.Name = "Forecolor";
			this.Forecolor.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Forecolor_ItemClick);
			this.forecolorPopupControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.forecolorPopupControl.Location = new System.Drawing.Point(603, 22);
			this.forecolorPopupControl.Manager = this.barManager;
			this.forecolorPopupControl.Name = "foreColorPopupControl";
			this.forecolorPopupControl.Size = new System.Drawing.Size(192, 227);
			this.forecolorPopupControl.TabIndex = 5;
			this.forecolorPopupControl.Tag = "Forecolor";
			this.forecolorPopupControl.Visible = false;
			this.forecolorPopupControl.ColorChanged += new System.EventHandler(this.ColorPopupControl_ColorChanged);
			this.backColor.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.backColor.DropDownControl = this.backcolorPopupControl;
			this.backColor.Glyph = ((System.Drawing.Image)(resources.GetObject("backColor.Glyph")));
			this.backColor.Id = 29;
			this.backColor.Name = "backColor";
			this.backColor.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.Forecolor_ItemClick);
			this.backcolorPopupControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.backcolorPopupControl.Location = new System.Drawing.Point(558, 59);
			this.backcolorPopupControl.Manager = this.barManager;
			this.backcolorPopupControl.Name = "backColorPopupControl";
			this.backcolorPopupControl.Size = new System.Drawing.Size(192, 227);
			this.backcolorPopupControl.TabIndex = 6;
			this.backcolorPopupControl.Tag = "Backcolor";
			this.backcolorPopupControl.Visible = false;
			this.backcolorPopupControl.ColorChanged += new System.EventHandler(this.ColorPopupControl_ColorChanged);
			this.FontSelector.Edit = this.repositoryItemFontEdit;
			this.FontSelector.Id = 10;
			this.FontSelector.Name = "FontSelector";
			this.FontSelector.Tag = "FontName";
			this.FontSelector.EditValueChanged += new System.EventHandler(this.FontSelector_EditValueChanged);
			this.repositoryItemFontEdit.AutoHeight = false;
			this.repositoryItemFontEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemFontEdit.Name = "repositoryItemFontEdit1";
			this.FontSize.Edit = this.repositoryItemFontSize;
			this.FontSize.Id = 12;
			this.FontSize.Name = "FontSize";
			this.FontSize.Tag = "FontSize";
			this.Styles.Appearance.Options.UseFont = true;
			this.FontSize.EditValueChanged += new System.EventHandler(this.FontSelector_EditValueChanged);
			this.Styles.Edit = this.repositoryItemStyles;
			this.Styles.Id = 30;
			this.Styles.Name = "Styles";
			this.Styles.Tag = "FormatBlock";
			this.Styles.Appearance.Options.UseFont = true;
			this.Styles.EditValueChanged += new System.EventHandler(this.FontSelector_EditValueChanged);
			this.repositoryItemFontSize.AutoHeight = false;
			this.repositoryItemFontSize.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemFontSize.Items.AddRange(new object[] {
			"1",
			"2",
			"3",
			"4",
			"5",
			"6",
			"7"});
			this.repositoryItemFontSize.Name = "repositoryItemComboBox1";
			this.repositoryItemFontSize.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.repositoryItemStyles.AutoHeight = false;
			this.repositoryItemStyles.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemStyles.Items.AddRange(new object[] {
			"Normal",
			"Heading 1",
			"Heading 2",
			"Heading 3",
			"Heading 4",
			"Heading 5",
			"Heading 6",
			"Address"
			});
			this.repositoryItemStyles.Name = "stylesRepositoryItem";
			this.repositoryItemStyles.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.bar2.BarName = "Actions";
			this.bar2.DockCol = 0;
			this.bar2.DockRow = 1;
			this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar2.FloatLocation = new System.Drawing.Point(257, 157);
			this.bar2.OptionsBar.UseWholeRow = true;
			this.bar2.OptionsBar.AllowQuickCustomization = false;
			this.bar2.OptionsBar.AllowDelete = false;
			this.bar2.OptionsBar.AllowCollapse = false;
			this.bar2.OptionsBar.DrawDragBorder = false;
			this.bar2.OptionsBar.DrawSizeGrip = false;
			this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Cut, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Copy, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Paste, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Undo, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Redo, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.RemoveFormat, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Subscript, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Superscript, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.InsertOrderedList, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.InsertUnorderedList, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Indent, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.Standard),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Outdent, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.CreateLink, "", true, true, true, 0, null, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.Unlink, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.PaintStyle, this.InsertImage, DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph)});
			this.bar2.Offset = 1;
			this.Cut.Enabled = false;
			this.Cut.Glyph = ((System.Drawing.Image)(resources.GetObject("Cut.Glyph")));
			this.Cut.GlyphDisabled = ((System.Drawing.Image)(resources.GetObject("Cut.GlyphDisabled")));
			this.Cut.Id = 13;
			this.Cut.Name = "Cut";
			this.Cut.Tag = "Cut";
			this.Cut.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Copy.Enabled = false;
			this.Copy.Glyph = ((System.Drawing.Image)(resources.GetObject("Copy.Glyph")));
			this.Copy.GlyphDisabled = ((System.Drawing.Image)(resources.GetObject("Copy.GlyphDisabled")));
			this.Copy.Id = 14;
			this.Copy.Name = "Copy";
			this.Copy.Tag = "Copy";
			this.Copy.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Paste.Glyph = ((System.Drawing.Image)(resources.GetObject("Paste.Glyph")));
			this.Paste.Id = 15;
			this.Paste.Name = "Paste";
			this.Paste.Tag = "Paste";
			this.Paste.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Undo.Id = 16;
			this.Undo.Name = "Undo";
			this.Undo.Tag = "Undo";
			this.Undo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Redo.Id = 17;
			this.Redo.Name = "Redo";
			this.Redo.Tag = "Redo";
			this.Redo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.RemoveFormat.Glyph = ((System.Drawing.Image)(resources.GetObject("RemoveFormat.Glyph")));
			this.RemoveFormat.Id = 18;
			this.RemoveFormat.Name = "RemoveFormat";
			this.RemoveFormat.Tag = "RemoveFormat";
			this.RemoveFormat.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Subscript.Glyph = ((System.Drawing.Image)(resources.GetObject("Subscript.Glyph")));
			this.Subscript.Id = 20;
			this.Subscript.Name = "Subscript";
			this.Subscript.Tag = "Subscript";
			this.Subscript.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Superscript.Glyph = ((System.Drawing.Image)(resources.GetObject("Superscript.Glyph")));
			this.Superscript.Id = 19;
			this.Superscript.Name = "Superscript";
			this.Superscript.Tag = "Superscript";
			this.Superscript.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.InsertOrderedList.Glyph = ((System.Drawing.Image)(resources.GetObject("InsertOrderedList.Glyph")));
			this.InsertOrderedList.Id = 21;
			this.InsertOrderedList.Name = "InsertOrderedList";
			this.InsertOrderedList.Tag = "InsertOrderedList";
			this.InsertOrderedList.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.InsertUnorderedList.Glyph = ((System.Drawing.Image)(resources.GetObject("InsertUnorderedList.Glyph")));
			this.InsertUnorderedList.Id = 22;
			this.InsertUnorderedList.Name = "InsertUnorderedList";
			this.InsertUnorderedList.Tag = "InsertUnorderedList";
			this.InsertUnorderedList.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Indent.Glyph = ((System.Drawing.Image)(resources.GetObject("Indent.Glyph")));
			this.Indent.Id = 23;
			this.Indent.Name = "Indent";
			this.Indent.Tag = "Indent";
			this.Indent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Outdent.Glyph = ((System.Drawing.Image)(resources.GetObject("Outdent.Glyph")));
			this.Outdent.Id = 24;
			this.Outdent.Name = "Outdent";
			this.Outdent.Tag = "Outdent";
			this.Outdent.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.CreateLink.Glyph = ((System.Drawing.Image)(resources.GetObject("CreateLink.Glyph")));
			this.CreateLink.Id = 25;
			this.CreateLink.Name = "CreateLink";
			this.CreateLink.Tag = "CreateLink";
			this.CreateLink.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.Unlink.Glyph = ((System.Drawing.Image)(resources.GetObject("Unlink.Glyph")));
			this.Unlink.Id = 26;
			this.Unlink.Name = "Unlink";
			this.Unlink.Tag = "Unlink";
			this.Unlink.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.StrikeThrough.Glyph = ((System.Drawing.Image)(resources.GetObject("StrikeThrough.Glyph")));
			this.StrikeThrough.Id = 3;
			this.StrikeThrough.Name = "StrikeThrough";
			this.StrikeThrough.Tag = "StrikeThrough";
			this.StrikeThrough.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
			this.InsertImage.Glyph = ((System.Drawing.Image)(resources.GetObject("InsertImage.Glyph")));
			this.InsertImage.Id = 27;
			this.InsertImage.Name = "InsertImage";
			this.InsertImage.Tag = "InsertImage";
			this.InsertImage.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.InsertImage_ItemClick);
			this.Color.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.Color.Caption = "Color";
			this.Color.Id = 27;
			this.Color.Name = "Color";
			this.repositoryItemForecolorEdit.AutoHeight = false;
			this.repositoryItemForecolorEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
			this.repositoryItemForecolorEdit.Name = "repositoryItemForecolorEdit";
			this.repositoryItemForecolorEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
			this.repositoryItemBackcolorEdit.AutoHeight = false;
			this.repositoryItemBackcolorEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)});
			this.repositoryItemBackcolorEdit.Name = "repositoryItemBackcolorEdit";
			this.repositoryItemBackcolorEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 53);
			this.tabControl.Name = "xtraTabControl1";
			this.tabControl.SelectedTabPage = this.designPage;
			this.tabControl.Size = new System.Drawing.Size(869, 351);
			this.tabControl.TabIndex = 7;
			this.tabControl.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.designPage,
			this.htmlPage});
			this.tabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(XtraTabControl1_SelectedPageChanged);
			this.tabControl.GotFocus += new System.EventHandler(XtraTabControl1_GotFocus);
			this.designPage.Controls.Add(this.browserControl);
			this.designPage.Name = "designPage";
			this.designPage.Size = new System.Drawing.Size(860, 320);
			this.designPage.Text = "Design";
			this.htmlPage.Controls.Add(this.memo);
			this.htmlPage.Name = "htmlPage";
			this.htmlPage.Size = new System.Drawing.Size(854, 311);
			this.htmlPage.Text = "Html";
			this.browserControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.browserControl.IsWebBrowserContextMenuEnabled = false;
			this.browserControl.Location = new System.Drawing.Point(0, 0);
			this.browserControl.MinimumSize = new System.Drawing.Size(20, 20);
			this.browserControl.Name = "webBrowser1";
			this.browserControl.Size = new System.Drawing.Size(860, 320);
			this.browserControl.TabIndex = 5;
			this.browserControl.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(browserControl_PreviewKeyDown);
			this.memo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.memo.Location = new System.Drawing.Point(0, 0);
			this.memo.Name = "memoEdit1";
			this.memo.Size = new System.Drawing.Size(854, 311);
			this.memo.TabIndex = 0;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.backcolorPopupControl);
			this.Controls.Add(this.forecolorPopupControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "HtmlEditor";
			this.Size = new System.Drawing.Size(869, 404);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.forecolorPopupControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.backcolorPopupControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemStyles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemForecolorEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemBackcolorEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.designPage.ResumeLayout(false);
			this.htmlPage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.memo.Properties)).EndInit();
			this.ResumeLayout(false);
		}	
		#endregion
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemFontSize;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox repositoryItemStyles;
		private DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit;
		private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemBackcolorEdit;
		private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemForecolorEdit;
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.Bar bar1;
		private DevExpress.XtraBars.Bar bar2;
		private DevExpress.XtraBars.BarButtonItem Bold;
		private DevExpress.XtraBars.BarButtonItem Italic;
		private DevExpress.XtraBars.BarButtonItem Underline;
		private DevExpress.XtraBars.BarButtonItem StrikeThrough;
		private DevExpress.XtraBars.BarButtonItem JustifyLeft;
		private DevExpress.XtraBars.BarButtonItem JustifyCenter;
		private DevExpress.XtraBars.BarButtonItem JustifyRight;
		private DevExpress.XtraBars.BarButtonItem JustifyFull;
		private DevExpress.XtraBars.BarEditItem FontSelector;
		private DevExpress.XtraBars.BarEditItem FontSize;
		private DevExpress.XtraBars.BarEditItem Styles;
		private DevExpress.XtraBars.BarButtonItem Cut;
		private DevExpress.XtraBars.BarButtonItem Copy;
		private DevExpress.XtraBars.BarButtonItem Paste;
		private DevExpress.XtraBars.BarButtonItem Undo;
		private DevExpress.XtraBars.BarButtonItem Redo;
		private DevExpress.XtraBars.BarButtonItem RemoveFormat;
		private DevExpress.XtraBars.BarButtonItem Superscript;
		private DevExpress.XtraBars.BarButtonItem Subscript;
		private DevExpress.XtraBars.BarButtonItem InsertOrderedList;
		private DevExpress.XtraBars.BarButtonItem InsertUnorderedList;
		private DevExpress.XtraBars.BarButtonItem Outdent;
		private DevExpress.XtraBars.BarButtonItem Indent;
		private DevExpress.XtraBars.BarButtonItem CreateLink;
		private DevExpress.XtraBars.BarButtonItem Unlink;
		private DevExpress.XtraBars.BarButtonItem InsertImage;
		private DevExpress.XtraBars.BarButtonItem Color;
		private DevExpress.XtraBars.BarButtonItem Forecolor;
		private DevExpress.XtraBars.BarButtonItem backColor;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private ColorPopupControl backcolorPopupControl;
		private ColorPopupControl forecolorPopupControl;
		private DevExpress.XtraTab.XtraTabControl tabControl;
		private DevExpress.XtraTab.XtraTabPage htmlPage;
		private DevExpress.XtraTab.XtraTabPage designPage;
		private DevExpress.XtraEditors.MemoEdit memo;
		private WebBrowserEx browserControl;
	}
}
