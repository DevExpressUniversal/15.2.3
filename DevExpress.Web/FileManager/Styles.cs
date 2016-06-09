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
using System.Linq;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Web {
	public abstract class FileManagerItemStyleBase : AppearanceStyleBase {
		FileManagerItemStateStyle selectionInactiveStyle;
		public FileManagerItemStyleBase() : base() { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerItemStyleBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
#pragma warning disable 197
				list.Add(delegate(object style, bool create) { return ((FileManagerItemStyleBase)style).CreateSelectionInactiveStyle(create); });
#pragma warning restore 197
				getObjects = list.ToArray();
			}
			return getObjects;
		}
		FileManagerItemStateStyle CreateSelectionInactiveStyle(bool create) {
			if(selectionInactiveStyle == null && create)
				TrackViewState(selectionInactiveStyle = CreateSelectionInactiveStyle());
			return selectionInactiveStyle;
		}
		protected internal virtual FileManagerItemStateStyle SelectionActiveStyle { get { return (FileManagerItemStateStyle)base.SelectedStyle; } }
		protected internal virtual FileManagerItemStateStyle SelectionInactiveStyle { get { return CreateSelectionInactiveStyle(true); } }
		protected override AppearanceSelectedStyle CreateSelectedStyle() {
			return CreateSelectionActiveStyle();
		}
		protected abstract FileManagerItemStateStyle CreateSelectionInactiveStyle();
		protected abstract FileManagerItemStateStyle CreateSelectionActiveStyle();
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			FileManagerItemStyleBase itemStyle = style as FileManagerItemStyleBase;
			if(itemStyle != null)
				SelectionInactiveStyle.CopyFrom(itemStyle.SelectionInactiveStyle);
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			FileManagerItemStyleBase itemStyle = style as FileManagerItemStyleBase;
			if(itemStyle != null)
				SelectionInactiveStyle.MergeWith(itemStyle.SelectionInactiveStyle);
		}
	}
	public class FileManagerItemStyle : FileManagerItemStyleBase {
		public FileManagerItemStyle() : base() { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerItemStyleHoverStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerItemStateStyle HoverStyle {
			get { return (FileManagerItemStateStyle)base.HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerItemStyleSelectionActiveStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerItemStateStyle SelectionActiveStyle {
			get { return (FileManagerItemStateStyle)base.SelectionActiveStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerItemStyleSelectionInactiveStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerItemStateStyle SelectionInactiveStyle {
			get { return (FileManagerItemStateStyle)base.SelectionInactiveStyle; }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new FileManagerItemStateStyle();
		}
		protected override FileManagerItemStateStyle CreateSelectionActiveStyle() {
			return new FileManagerItemStateStyle();
		}
		protected override FileManagerItemStateStyle CreateSelectionInactiveStyle() {
			return new FileManagerItemStateStyle();
		}
	}
	public class FileManagerFileStyle : FileManagerItemStyleBase {
		const string DefaultWidth = "100px";
		FileManagerFileStateStyle focusedStyle;
		public FileManagerFileStyle()
			: base() {
			Width = Unit.Parse(DefaultWidth);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleMargins"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Margins Margins {
			get { return base.Margins; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleWidth"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), DefaultWidth),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width { get { return base.Width; } set { base.Width = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleHeight"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleHoverStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerFileStateStyle HoverStyle {
			get { return (FileManagerFileStateStyle)base.HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual FileManagerFileStateStyle FocusedStyle
		{
			get { return CreateFocusedStyle(true); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleSelectionActiveStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerFileStateStyle SelectionActiveStyle {
			get { return (FileManagerFileStateStyle)base.SelectionActiveStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStyleSelectionInactiveStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerFileStateStyle SelectionInactiveStyle {
			get { return (FileManagerFileStateStyle)base.SelectionInactiveStyle; }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new FileManagerFileStateStyle();
		}
		protected override FileManagerItemStateStyle CreateSelectionActiveStyle() {
			return new FileManagerFileStateStyle();
		}
		protected override FileManagerItemStateStyle CreateSelectionInactiveStyle() {
			return new FileManagerFileStateStyle();
		}
		static GetStateManagerObject[] getObjects;
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			if(getObjects == null) {
				List<GetStateManagerObject> list = new List<GetStateManagerObject>();
				list.AddRange(base.GetStateManagedObjectsDelegates());
				list.Add(delegate(object style, bool create) { return ((FileManagerFileStyle)style).CreateFocusedStyle(create); });
				getObjects = list.ToArray();
			}
			return getObjects;
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			FileManagerFileStyle itemStyle = style as FileManagerFileStyle;
			if(itemStyle != null)
				FocusedStyle.CopyFrom(itemStyle.FocusedStyle);
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			FileManagerFileStyle itemStyle = style as FileManagerFileStyle;
			if(itemStyle != null)
				FocusedStyle.MergeWith(itemStyle.FocusedStyle);
		}
		FileManagerFileStateStyle CreateFocusedStyle(bool create) {
			if(focusedStyle == null && create)
				TrackViewState(focusedStyle = new FileManagerFileStateStyle());
			return focusedStyle;
		}
	}
	public class FileManagerFolderStyle : FileManagerItemStyleBase {
		public FileManagerFolderStyle() : base() { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFolderStyleHoverStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerFolderStateStyle HoverStyle {
			get { return (FileManagerFolderStateStyle)base.HoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFolderStyleSelectionActiveStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerFolderStateStyle SelectionActiveStyle {
			get { return (FileManagerFolderStateStyle)base.SelectionActiveStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFolderStyleSelectionInactiveStyle"),
#endif
		Category("Styles"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual FileManagerFolderStateStyle SelectionInactiveStyle {
			get { return (FileManagerFolderStateStyle)base.SelectionInactiveStyle; }
		}
		protected override AppearanceSelectedStyle CreateHoverStyle() {
			return new FileManagerFolderStateStyle();
		}
		protected override FileManagerItemStateStyle CreateSelectionActiveStyle() {
			return new FileManagerFolderStateStyle();
		}
		protected override FileManagerItemStateStyle CreateSelectionInactiveStyle() {
			return new FileManagerFolderStateStyle();
		}
	}
	public class FileManagerItemStateStyle : AppearanceSelectedStyle {}
	public class FileManagerFileStateStyle : FileManagerItemStateStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFileStateStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
	}
	public class FileManagerFolderStateStyle : FileManagerItemStateStyle { }
	public class FileManagerHighlightStyle : AppearanceStyleBase {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
	}
	public class FileManagerPanelStyle : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerPanelStylePaddings"),
#endif
		Category("Layout"), AutoFormatEnable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerPanelStyleHeight"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height { get { return base.Height; } set { base.Height = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
	}
	public abstract class FileManagerContainerStyleBase : SplitterPaneStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ForeColor { get { return base.ForeColor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap { get { return base.Wrap; } }
	}
	public class FileManagerFolderContainerStyle : FileManagerContainerStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerFolderContainerStyleWidth"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable,
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new virtual Unit Width {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "Width", Unit.Empty); }
			set { ViewStateUtils.SetUnitProperty(ViewState, "Width", Unit.Empty, value); }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			FileManagerFolderContainerStyle src = style as FileManagerFolderContainerStyle;
			if(src != null) {
				if(!src.Width.IsEmpty)
					Width = src.Width;
			}
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			FileManagerFolderContainerStyle src = style as FileManagerFolderContainerStyle;
			if(src != null) {
				if(!src.Width.IsEmpty)
					Width = src.Width;
			}
		}
	}
	public class FileManagerFileContainerStyle : FileManagerContainerStyleBase { }
	public class FileManagerToolbarStyle : FileManagerPanelStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerToolbarStylePathTextBoxWidth"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit PathTextBoxWidth {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "PathTextBoxWidth", Unit.Empty); }
			set { ViewStateUtils.SetUnitProperty(ViewState, "PathTextBoxWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerToolbarStyleFilterTextBoxWidth"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public virtual Unit FilterTextBoxWidth {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "FilterTextBoxWidth", Unit.Empty); }
			set { ViewStateUtils.SetUnitProperty(ViewState, "FilterTextBoxWidth", Unit.Empty, value); }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			FileManagerToolbarStyle src = style as FileManagerToolbarStyle;
			if(src != null) {
				if(!src.PathTextBoxWidth.IsEmpty)
					PathTextBoxWidth = src.PathTextBoxWidth;
				if(!src.FilterTextBoxWidth.IsEmpty)
					FilterTextBoxWidth = src.FilterTextBoxWidth;
			}
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			FileManagerToolbarStyle src = style as FileManagerToolbarStyle;
			if(src != null) {
				if(!src.PathTextBoxWidth.IsEmpty)
					PathTextBoxWidth = src.PathTextBoxWidth;
				if(!src.FilterTextBoxWidth.IsEmpty)
					FilterTextBoxWidth = src.FilterTextBoxWidth;
			}
		}
	}
	public class FileManagerToolbarItemStyle : MenuItemStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle CheckedStyle {
			get { return base.CheckedStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit DropDownButtonSpacing {
			get { return base.DropDownButtonSpacing; }
			set { base.DropDownButtonSpacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override MenuItemDropDownButtonStyle DropDownButtonStyle {
			get { return base.DropDownButtonStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit PopOutImageSpacing {
			get { return base.PopOutImageSpacing; }
			set { base.PopOutImageSpacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal override AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle SelectedStyle {
			get { return base.SelectedStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ToolbarDropDownButtonSpacing {
			get { return base.ToolbarDropDownButtonSpacing; }
			set { base.ToolbarDropDownButtonSpacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ToolbarPopOutImageSpacing {
			get { return base.ToolbarPopOutImageSpacing; }
			set { base.ToolbarPopOutImageSpacing = value; }
		}
	}
	public class FileManagerStyles : StylesBase {
		const string CssClassPrefix = "dxfm";
		const string ItemStyleName = "ItemStyle";
		const string FileStyleName = "FileStyle";
		const string FolderStyleName = "FolderStyle";
		const string FileAreaFolderStyleName = "FileAreaFolderStyle";
		const string BreadcrumbsStyleName = "Breadcrumbs";
		const string BreadcrumbsItemStyleName = "BreadcrumbsItemStyle";
		const string FileHighlightStyle = "FileHighlightStyle";
		const string ToolbarStyleName = "ToolbarStyle";
		const string UploadPanelStyleName = "UploadPanelStyle";
		const string FolderContainerStyleName = "FolderContainerStyle";
		const string FileContainerStyleName = "FileContainerStyle";
		const string ToolbarItemStyleName = "ToolbarItemStyle";
		const string ContextMenuItemStyleName = "ContextMenuItemStyle";
		const string PathBoxStyleName = "PathBoxStyle";
		const string FilterBoxStyleName = "FilterBoxStyle";
		protected internal const string
			SpecialCssClass = CssClassPrefix,
			FileCssClass = CssClassPrefix + "-file",
			FileContentCssClass = CssClassPrefix + "-content",
			FileHoverCssClass = CssClassPrefix + "-fileH",
			FileSelectionActiveCssClass = CssClassPrefix + "-fileSA",
			FileSelectionInactiveCssClass = CssClassPrefix + "-fileSI",
			FileFocusCssClass = CssClassPrefix + "-fileF",
			FolderCssClass = CssClassPrefix + "-folder",
			FolderSelectionInactiveCssClass = CssClassPrefix + "-folderSI",
			BreadcrumbsCssClass = CssClassPrefix + "-breadCrumbs",
			BreadcrumbsItemCssClass = CssClassPrefix + "-bcItem",
			BreadcrumbsItemHoverCssClass = CssClassPrefix + "-bcItemH",
			BreadcrumbsPopupCssClass = CssClassPrefix + "-bcPopup",
			HighlightCssClass = CssClassPrefix + "-highlight",
			ToolbarCssClass = CssClassPrefix + "-toolbar",
			ToolbarFilterCssClass = CssClassPrefix + "-filter",
			ToolbarPathCssClass = CssClassPrefix + "-path",
			UploadPanelCssClass = CssClassPrefix + "-uploadPanel",
			UploadPanelTableCssClass = CssClassPrefix + "-uploadPanelTable",
			UploadPanelTableButtonCellCssClass = CssClassPrefix + "-uploadPanelTableBCell",
			UploadPanelDisableButton = CssClassPrefix + "-uploadDisable",
			UploadProgressPopupCssClass = CssClassPrefix + "-upPopup",
			RenameFileInputCssClass = CssClassPrefix + "-rInput",
			FolderBrowserPopupFoldersContainer = CssClassPrefix + "-mpFoldersC",
			FolderBrowserPopupButtonContainer = CssClassPrefix + "-mpButtonC",
			CreateInputCssClass = CssClassPrefix + "-cInput",
			RightToLeftCssClass = CssClassPrefix + "-rtl",
			DesctopCssClass = CssClassPrefix + "-dst",
			TouchCssClass = CssClassPrefix + "-tch",
			FileColumnTitleCellCssClass = CssClassPrefix + "-fileNameCell",
			FileColumnTitleCssClass = CssClassPrefix + "-fileName",
			FileColumnThumbnailCssClass = CssClassPrefix + "-fileThumb",
			FilePaneCssClass = CssClassPrefix + "-filePane";
		public FileManagerStyles(ISkinOwner splitter)
			: base(splitter) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesItem"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerItemStyle Item { get { return (FileManagerItemStyle)GetStyle(ItemStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesFile"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerFileStyle File { get { return (FileManagerFileStyle)GetStyle(FileStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesFolder"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerFolderStyle Folder { get { return (FileManagerFolderStyle)GetStyle(FolderStyleName); } }
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerFileStyle FileAreaFolder { get { return (FileManagerFileStyle)GetStyle(FileAreaFolderStyleName); } }
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase Breadcrumbs { get { return (AppearanceStyleBase)GetStyle(BreadcrumbsStyleName); } }
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle BreadcrumbsItem { get { return (AppearanceStyle)GetStyle(BreadcrumbsItemStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesHighlight"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerHighlightStyle Highlight { get { return (FileManagerHighlightStyle)GetStyle(FileHighlightStyle); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesToolbar"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerToolbarStyle Toolbar { get { return (FileManagerToolbarStyle)GetStyle(ToolbarStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesUploadPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerPanelStyle UploadPanel { get { return (FileManagerPanelStyle)GetStyle(UploadPanelStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesFolderContainer"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerFolderContainerStyle FolderContainer { get { return (FileManagerFolderContainerStyle)GetStyle(FolderContainerStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesFileContainer"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerFileContainerStyle FileContainer { get { return (FileManagerFileContainerStyle)GetStyle(FileContainerStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesToolbarItem"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerToolbarItemStyle ToolbarItem { get { return (FileManagerToolbarItemStyle)GetStyle(ToolbarItemStyleName); } }
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerToolbarItemStyle ContextMenuItem { get { return (FileManagerToolbarItemStyle)GetStyle(ContextMenuItemStyleName); } }
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerToolbarItemStyle PathBox { get { return (FileManagerToolbarItemStyle)GetStyle(PathBoxStyleName); } }
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerToolbarItemStyle FilterBox { get { return (FileManagerToolbarItemStyle)GetStyle(FilterBoxStyleName); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesLoadingPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanel { get { return base.LoadingPanelInternal; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerStylesLoadingDiv"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingDivStyle LoadingDiv { get { return base.LoadingDivInternal; } }
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ItemStyleName, delegate() { return new FileManagerItemStyle(); }));
			list.Add(new StyleInfo(FileStyleName, delegate() { return new FileManagerFileStyle(); }));
			list.Add(new StyleInfo(FileAreaFolderStyleName, delegate() { return new FileManagerFileStyle(); }));
			list.Add(new StyleInfo(FileHighlightStyle, delegate() { return new FileManagerHighlightStyle(); }));
			list.Add(new StyleInfo(ToolbarStyleName, delegate() { return new FileManagerToolbarStyle(); }));
			list.Add(new StyleInfo(UploadPanelStyleName, delegate() { return new FileManagerPanelStyle(); }));
			list.Add(new StyleInfo(FolderStyleName, delegate() { return new FileManagerFolderStyle(); }));
			list.Add(new StyleInfo(FolderStyleName, delegate() { return new FileManagerFolderStyle(); }));
			list.Add(new StyleInfo(FolderContainerStyleName, delegate() { return new FileManagerFolderContainerStyle(); }));
			list.Add(new StyleInfo(FileContainerStyleName, delegate() { return new FileManagerFileContainerStyle(); }));
			list.Add(new StyleInfo(BreadcrumbsStyleName, delegate() { return new AppearanceStyleBase(); }));
			list.Add(new StyleInfo(BreadcrumbsItemStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(ToolbarItemStyleName, delegate() { return new FileManagerToolbarItemStyle(); }));
			list.Add(new StyleInfo(ContextMenuItemStyleName, delegate() { return new FileManagerToolbarItemStyle(); }));
			list.Add(new StyleInfo(PathBoxStyleName, delegate() { return new FileManagerToolbarItemStyle(); }));
			list.Add(new StyleInfo(FilterBoxStyleName, delegate() { return new FileManagerToolbarItemStyle(); }));
		}
		protected T GetStyleWithCssClass<T>(params string[] cssClasses) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CssClass = RenderUtils.CombineCssClasses(cssClasses);
			return style;
		}
		protected internal FileManagerFileStyle GetDefaultFileStyle() {
			return GetStyleWithCssClass<FileManagerFileStyle>(FileCssClass);
		}
		protected internal FileManagerFileStyle GetDefaultFolderStyle() {
			return GetStyleWithCssClass<FileManagerFileStyle>(FolderCssClass);
		}
		protected internal FileManagerFileStyle GetDefaultFileAreaFolderStyle() {
			return GetStyleWithCssClass<FileManagerFileStyle>(FileCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileContentStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileContentCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileHoverStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileHoverCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileSelectionActiveStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileSelectionActiveCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileSelectionInactiveStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileSelectionInactiveCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileAreaFolderContentStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileContentCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileAreaFolderHoverStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileHoverCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileAreaFolderSelectionActiveStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileSelectionActiveCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileAreaFolderSelectionInactiveStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileSelectionInactiveCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFolderSelectionInactiveStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FolderSelectionInactiveCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultHighlightStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(HighlightCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileFocusStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileFocusCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileAreaFolderFocusStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FileFocusCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultThumbnailCheckBoxFocusStyle() {
			return CreateStyleByName(string.Empty, DevExpress.Web.Internal.InternalCheckBox.InternalCheckboxControl.FocusedCheckBoxClassName);
		}
		protected internal AppearanceStyleBase GetDefaultBreadcrumbsStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(BreadcrumbsCssClass);
		}		
		protected internal AppearanceStyle GetDefaultBreadcrumbsItemStyle() {
			return GetStyleWithCssClass<AppearanceStyle>(BreadcrumbsItemCssClass);
		}
		protected internal AppearanceSelectedStyle GetDefaultBreadcrumbsItemHoverStyle() {
			return GetStyleWithCssClass<AppearanceSelectedStyle>(BreadcrumbsItemHoverCssClass);
		}
		protected internal Unit GetDefaultFoldersContainerWidth() {
			return Unit.Pixel(250);
		}
		protected internal Unit GetDefaultToolbarHeight() {
			return Unit.Pixel(40);
		}
		protected internal AppearanceStyleBase GetDefaultToolbarStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(ToolbarCssClass);
		}
		protected internal AppearanceStyleBase GetDefaultFileContainerStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(FilePaneCssClass);
		}
		protected internal Unit GetDefaultUploadPanelHeight() {
			return Unit.Pixel(39);
		}
		protected internal AppearanceStyleBase GetDefaultUploadPanelStyle() {
			return GetStyleWithCssClass<AppearanceStyleBase>(UploadPanelCssClass);
		}
	}
	public class FileManagerContextMenuStyles : MenuStyles {
		public FileManagerContextMenuStyles(ASPxFileManager fileManager)
			: base(fileManager) {
		}
	}
	public class FileManagerDetailsViewStyles : GridViewStyles {
		internal const string GridViewHeaderPostfix = "dxfmGridHeader";
		public FileManagerDetailsViewStyles(ISkinOwner skinOwner)
			:base(skinOwner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int GroupButtonWidth {
			get{ return base.GroupButtonWidth; }
			set{ base.GroupButtonWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewGroupRowStyle GroupRow {
			get { return base.GroupRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewGroupRowStyle FocusedGroupRow {
			get { return base.FocusedGroupRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewDataRowStyle DetailRow {
			get { return base.DetailRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCellStyle DetailCell {
			get { return base.DetailCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewDataRowStyle PreviewRow {
			get { return base.PreviewRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewRowStyle FilterRow {
			get { return base.FilterRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewGroupFooterStyle GroupFooter {
			get { return base.GroupFooter; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewGroupPanelStyle GroupPanel {
			get { return base.GroupPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewHeaderPanelStyle HeaderPanel {
			get { return base.HeaderPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCellStyle PagerTopPanel {
			get { return base.PagerTopPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCellStyle PagerBottomPanel {
			get { return base.PagerBottomPanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCellStyle DetailButton {
			get { return base.DetailButton; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new LoadingDivStyle LoadingDiv {
			get { return base.LoadingDivInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewEditCellStyle InlineEditCell {
			get { return base.InlineEditCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterCellStyle FilterCell {
			get { return base.FilterCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewInlineEditRowStyle InlineEditRow {
			get { return base.InlineEditRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewDataRowStyle EditFormDisplayRow {
			get { return base.EditFormDisplayRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewRowStyle EditingErrorRow {
			get { return base.EditingErrorRow; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewEditFormStyle EditForm {
			get { return base.EditForm; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewEditCellStyle EditFormCell {
			get { return base.EditFormCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewEditFormTableStyle EditFormTable {
			get { return base.EditFormTable; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewEditFormCaptionStyle EditFormColumnCaption {
			get { return base.EditFormColumnCaption; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewTitleStyle TitlePanel {
			get { return base.TitlePanel; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewStatusBarStyle StatusBar {
			get { return base.StatusBar; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterBarStyle FilterBar {
			get { return base.FilterBar; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterBarStyle FilterBarLink {
			get { return base.FilterBarLink; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterBarStyle FilterBarCheckBoxCell {
			get { return base.FilterBarCheckBoxCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterBarStyle FilterBarImageCell {
			get { return base.FilterBarImageCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterBarStyle FilterBarExpressionCell {
			get { return base.FilterBarExpressionCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFilterBarStyle FilterBarClearButtonCell {
			get { return base.FilterBarClearButtonCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCellStyle BatchEditCell {
			get { return base.BatchEditCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCellStyle BatchEditModifiedCell {
			get { return base.BatchEditModifiedCell; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewCommandColumnStyle CommandColumnItem {
			get { return base.CommandColumnItem; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DevExpress.Web.MenuItemStyle FilterRowMenuItem {
			get { return base.FilterRowMenuItem; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DevExpress.Web.MenuStyle FilterRowMenu {
			get { return base.FilterRowMenu; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridViewFooterStyle Footer {
			get { return base.Footer; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Theme {
			get { return base.Theme; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerDetailsViewStylesCommandColumn"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FileManagerGridViewCommandColumnStyle CommandColumn
		{
			get { return (FileManagerGridViewCommandColumnStyle)base.CommandColumn; }
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Remove(list.Find(si => si.Name == CommandColumnStyleName));
			list.Add(new StyleInfo(CommandColumnStyleName, typeof(FileManagerGridViewCommandColumnStyle)));
		}
	}
	public class FileManagerGridViewCommandColumnStyle : GridViewCommandColumnStyle {
		[
#if !SL
	DevExpressWebLocalizedDescription("FileManagerGridViewCommandColumnStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Width
		{
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
}
