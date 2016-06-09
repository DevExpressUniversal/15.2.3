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
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class UploadControlBrowseButtonStyle : ButtonStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle PressedStyle {
			get { return base.PressedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlBrowseButtonStyleImageSpacing"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit ImageSpacing
		{
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlBrowseButtonStyleWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true),]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
	}
	public class UploadControlButtonStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlButtonStyleDisabledStyle"),
#endif
NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle
		{
			get { return base.DisabledStyle; }
		}
	}
	public class UploadControlTextBoxStyleBase : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AppearanceSelectedStyle HoverStyle {
			get { return base.HoverStyle; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit ImageSpacing {
			get { return base.ImageSpacing; }
			set { base.ImageSpacing = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Spacing {
			get { return base.Spacing; }
			set { base.Spacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlTextBoxStyleBaseHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class UploadControlTextBoxStyle : UploadControlTextBoxStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlTextBoxStyleDisabledStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new virtual DisabledStyle DisabledStyle {
			get { return base.DisabledStyle; }
		}
	}
	public class UploadControlNullTextStyle : UploadControlTextBoxStyleBase {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { base.VerticalAlign = value; }
		}
	}
	public class UploadControlStyles : StylesBase {
		Nullable<bool> isNativeInternal = null;
		public const string HiddenUIStyleName = "dxucHidden";
		public const string EditAreaSystemStyleName = "dxucEditAreaSys";
		public const string ButtonSystemStyleName = "dxucButtonSys";
		public const string InlineDropZoneSystemStyleName = "dxucInlineDropZoneSys";
		public const string InlineDropZoneBorderWrapperSystemStyleName = "dxucIZBorder";
		public const string InlineDropZoneBackgroundWrapperSystemStyleName = "dxucIZBackground";
		public const string NullTextStyleName = "NullText";
		public const string TextBoxStyleName = "TextBox";
		public const string BrowseButtonStyleName = "BrowseButton";
		public const string ButtonStyleName = "Button";
		public const string ErrorMessageStyleName = "ErrorMessage";
		public const string ProgressBarStyleName = "ProgressBar";
		public const string ProgressBarIndicatorStyleName = "ProgressBarIndicator";
		public const string InlineDropZoneStyleName = "InlineDropZone";
		public const string FileListItemStyleName = "FileListItem";
		public UploadControlStyles(ASPxUploadControl uploadControl)
			: base(uploadControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesNative"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set { base.Native = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesNullText"),
#endif
PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public UploadControlNullTextStyle NullText
		{
			get { return (UploadControlNullTextStyle)GetStyle(NullTextStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesTextBox"),
#endif
PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public UploadControlTextBoxStyle TextBox
		{
			get { return (UploadControlTextBoxStyle)GetStyle(TextBoxStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesBrowseButton"),
#endif
PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public UploadControlBrowseButtonStyle BrowseButton
		{
			get { return (UploadControlBrowseButtonStyle)GetStyle(BrowseButtonStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public UploadControlButtonStyle Button {
			get { return (UploadControlButtonStyle)GetStyle(ButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the ButtonStyle.DisabledStyle property instead."),
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesDisabledButton")
#else
	Description("")
#endif
]
		public AppearanceSelectedStyle DisabledButton {
			get { return Button.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesErrorMessage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public AppearanceStyleBase ErrorMessage {
			get { return (AppearanceStyleBase)GetStyle(ErrorMessageStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesProgressBar"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ProgressStyle ProgressBar {
			get { return (ProgressStyle)GetStyle(ProgressBarStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesProgressBarIndicator"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ProgressBarIndicatorStyle ProgressBarIndicator {
			get { return (ProgressBarIndicatorStyle)GetStyle(ProgressBarIndicatorStyleName); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public UploadControlDropZoneStyle DropZone {
			get { return (UploadControlDropZoneStyle)GetStyle(InlineDropZoneStyleName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlStylesFileListItemStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlFileListItemStyle FileListItemStyle {
			get { return (UploadControlFileListItemStyle)GetStyle(FileListItemStyleName); }
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxuc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(NullTextStyleName, delegate() { return new UploadControlNullTextStyle(); }));
			list.Add(new StyleInfo(TextBoxStyleName, delegate() { return new UploadControlTextBoxStyle(); }));
			list.Add(new StyleInfo(BrowseButtonStyleName, delegate() { return new UploadControlBrowseButtonStyle(); }));
			list.Add(new StyleInfo(ButtonStyleName, delegate() { return new UploadControlButtonStyle(); }));
			list.Add(new StyleInfo(ErrorMessageStyleName, delegate() { return new AppearanceStyleBase(); }));
			list.Add(new StyleInfo(ProgressBarStyleName, delegate() { return new ProgressStyle(); }));
			list.Add(new StyleInfo(ProgressBarIndicatorStyleName, delegate() { return new ProgressBarIndicatorStyle(); }));
			list.Add(new StyleInfo(InlineDropZoneStyleName, delegate() { return new UploadControlDropZoneStyle(); }));
			list.Add(new StyleInfo(FileListItemStyleName, delegate() { return new UploadControlFileListItemStyle(); }));
		}
		private T GetDefaultStyle<T>(string styleName) where T : AppearanceStyleBase, new() {
			T style = new T();
			style.CopyFrom(CreateStyleByName(styleName));
			return style;
		}
		private T GetDefaultStyle<T>(string styleName, bool isNative) where T : AppearanceStyleBase, new() {
			T style = new T();
			this.isNativeInternal = isNative;
			style.CopyFrom(CreateStyleByName(styleName));
			this.isNativeInternal = null;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultNullTextStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("NullTextStyle");
		}
		protected internal AppearanceStyleBase GetDefaultTextBoxStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("TextBoxStyle");
		}
		protected internal AppearanceStyleBase GetDefaultTextBoxDisabledStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("TextBoxDisabledStyle");
		}
		protected internal AppearanceStyleBase GetDefaultBrowseButtonStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("BrowseButtonStyle");
		}
		protected internal AppearanceStyleBase GetDefaultBrowseButtonHoverStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("BrowseButtonHoverStyle");
		}
		protected internal AppearanceStyleBase GetDefaultBrowseButtonPressedStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("BrowseButtonPressedStyle");
		}
		protected internal AppearanceStyleBase GetDefaultBrowseButtonDisabledStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("BrowseButtonDisabledStyle");
		}
		protected internal AppearanceStyleBase GetDefaultButtonStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("ButtonStyle", false);
		}
		protected internal AppearanceStyleBase GetDefaultButtonDisabledStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("ButtonDisabledStyle", false);
		}
		protected internal AppearanceStyleBase GetDefaultErrorCellStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("ErrorCellStyle", false);
		}
		protected internal ProgressStyle GetDefaultProgressStyle() {
			ProgressStyle style = GetDefaultStyle<ProgressStyle>("ProgressBarStyle", false);
			return style;
		}
		protected internal ProgressBarIndicatorStyle GetDefaultProgressBarIndicatorStyle() {
			ProgressBarIndicatorStyle style = GetDefaultStyle<ProgressBarIndicatorStyle>("ProgressBarIndicatorStyle", false);
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultEditAreaStyle() {
			return GetDefaultStyle<AppearanceStyleBase>("EditAreaStyle");
		}
		protected internal AppearanceStyleBase GetDefaultInputsTableStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), "Inputs");
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultPlatformErrorPanelStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CssClass = GetCssClassName(GetCssClassNamePrefix(), "SilverlightPluginLinkPanel");
			return style;
		}
		protected internal UploadControlDropZoneStyle GetDefaultInlineDropZoneStyle() {
			UploadControlDropZoneStyle style = GetDefaultStyle<UploadControlDropZoneStyle>("DropZoneStyle", false);
			return style;
		}
		protected internal UploadControlFileListItemStyle GetDefaultFileListItemStyle() {
			UploadControlFileListItemStyle style = GetDefaultStyle<UploadControlFileListItemStyle>("FileListItemStyle", false);
			return style;
		}
		protected internal Unit GetDefaultInputSpacing() {
			return Unit.Pixel(2);
		}
		protected internal Unit GetDefaultButtonSpacing() {
			return Unit.Pixel(10);
		}
		protected internal Unit GetDefaultBrowseButtonSpacing() {
			return Unit.Pixel(5);
		}
		protected internal Unit GetDefaultRemoveButtonSpacing() {
			return Unit.Pixel(5);
		}
		protected internal Unit GetDefaultAddUploadButtonsSpacing() {
			return Unit.Pixel(13);
		}
		protected internal Unit GetDefaultCancelButtonSpacing() {
			return Unit.Pixel(13);
		}
		public override bool IsNative() {
			if(this.isNativeInternal.HasValue)
				return this.isNativeInternal.Value;
			return base.IsNative();
		}
	}
	public class ProgressStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DefaultBoolean Wrap {
			get { return base.Wrap; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorizontalAlign HorizontalAlign {
			get { return base.HorizontalAlign; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VerticalAlign VerticalAlign {
			get { return base.VerticalAlign; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressStylePaddings"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public new virtual Paddings Paddings {
			get { return base.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressStyleHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class ProgressBarIndicatorStyle : IndicatorStyle {
	}
	public class UploadControlDropZoneStyle : AppearanceStyle {
	}
	public class UploadControlFileListItemStyle : AppearanceStyle {
	}
}
