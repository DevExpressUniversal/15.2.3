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
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public enum DateVerticalPosition { Top, Header, BelowHeader, Bottom }
	public enum DateHorizontalPosition { Left, Right, OutsideLeft, OutsideRight }
	public enum HeadlineImagePosition { Left, Right }
	public enum TailImagePosition { BeforeTailText, AfterTailText }
	public enum TailPosition { Inline, NewLine, KeepWithLastWord }
	[DXWebToolboxItem(true), DefaultProperty("ContentText"),
	DataBindingHandler("DevExpress.Web.Design.HeadlineDataBindingHandler, " + AssemblyInfo.SRAssemblyWebDesignFull),
	Designer("DevExpress.Web.Design.ASPxHeadlineDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxHeadline.bmp")
	]
	public class ASPxHeadline : ASPxWebControl {
		private const string TextEllipsis = "&hellip;"; 
		HeadlineSettings fHeadlineSettings = null;
		private HeadlineControl fHeadlineControl = null;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineContentText"),
#endif
		DefaultValue(""), Bindable(true), AutoFormatDisable, Localizable(true),
		Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(UITypeEditor))]
		public string ContentText {
			get { return GetStringProperty("ContentText", ""); }
			set {
				SetStringProperty("ContentText", "", value);
				LayoutChanged();
			}
		}
		static object minDate = DateTime.MinValue;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineDate"),
#endif
		DefaultValue(typeof(DateTime), "1/1/0001"), Bindable(true), AutoFormatDisable]
		public DateTime Date {
			get { return (DateTime)GetObjectProperty("Date", minDate); }
			set {
				SetObjectProperty("Date", minDate, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineDateFormatString"),
#endif
		DefaultValue(HeadlineSettings.DefaultDateFormatString), AutoFormatDisable, Localizable(true)]
		public string DateFormatString {
			get { return HeadlineSettings.DateFormatString; }
			set { HeadlineSettings.DateFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineHeaderText"),
#endif
		DefaultValue(""), Bindable(true), AutoFormatDisable, Localizable(true)]
		public string HeaderText {
			get { return GetStringProperty("HeaderText", ""); }
			set {
				SetStringProperty("HeaderText", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineTailText"),
#endif
		DefaultValue(""), Bindable(true), AutoFormatDisable, Localizable(true)]
		public string TailText {
			get { return HeadlineSettings.TailText; }
			set { HeadlineSettings.TailText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineToolTip"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable]
		public override string ToolTip {
			get { return HeadlineSettings.ToolTip; }
			set { HeadlineSettings.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineMaxLength"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int MaxLength {
			get { return HeadlineSettings.MaxLength; }
			set { HeadlineSettings.MaxLength = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineNavigateUrl"),
#endif
		DefaultValue(""), Bindable(true), AutoFormatDisable, Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(UITypeEditor))]
		public string NavigateUrl {
			get { return GetStringProperty("NavigateUrl", ""); }
			set {
				SetStringProperty("NavigateUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineTarget"),
#endif
		DefaultValue(""), Localizable(false),
		TypeConverter(typeof(TargetConverter)), AutoFormatDisable]
		public string Target {
			get { return HeadlineSettings.Target; }
			set { HeadlineSettings.Target = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineDateHorizontalPosition"),
#endif
		Category("Appearance"), DefaultValue(DateHorizontalPosition.Left), AutoFormatEnable]
		public DateHorizontalPosition DateHorizontalPosition {
			get { return HeadlineSettings.DateHorizontalPosition; }
			set { HeadlineSettings.DateHorizontalPosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineDateVerticalPosition"),
#endif
		Category("Appearance"), DefaultValue(DateVerticalPosition.BelowHeader), AutoFormatEnable]
		public DateVerticalPosition DateVerticalPosition {
			get { return HeadlineSettings.DateVerticalPosition; }
			set { HeadlineSettings.DateVerticalPosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineImagePosition"),
#endif
		Category("Appearance"), DefaultValue(HeadlineImagePosition.Left), AutoFormatEnable]
		public HeadlineImagePosition ImagePosition {
			get { return HeadlineSettings.ImagePosition; }
			set { HeadlineSettings.ImagePosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineTailImagePosition"),
#endif
		Category("Appearance"), DefaultValue(TailImagePosition.AfterTailText), AutoFormatEnable]
		public TailImagePosition TailImagePosition {
			get { return HeadlineSettings.TailImagePosition; }
			set { HeadlineSettings.TailImagePosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineTailPosition"),
#endif
		Category("Appearance"), DefaultValue(TailPosition.Inline), AutoFormatEnable]
		public TailPosition TailPosition {
			get { return HeadlineSettings.TailPosition; }
			set { HeadlineSettings.TailPosition = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return (ControlStyle as AppearanceStyleBase).HorizontalAlign; }
			set {
				(ControlStyle as AppearanceStyleBase).HorizontalAlign = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineShowHeaderAsLink"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowHeaderAsLink {
			get { return HeadlineSettings.ShowHeaderAsLink; }
			set { HeadlineSettings.ShowHeaderAsLink = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineShowContentAsLink"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowContentAsLink {
			get { return HeadlineSettings.ShowContentAsLink; }
			set { HeadlineSettings.ShowContentAsLink = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineShowImageAsLink"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowImageAsLink {
			get { return HeadlineSettings.ShowImageAsLink; }
			set { HeadlineSettings.ShowImageAsLink = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineShowContentInToolTip"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ShowContentInToolTip {
			get { return HeadlineSettings.ShowContentInToolTip; }
			set { HeadlineSettings.ShowContentInToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineSpriteImageUrl"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineImage"),
#endif
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public ImageProperties Image {
			get { return Images.Image; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineTailImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HeadlineTailImageProperties TailImage {
			get { return HeadlineSettings.TailImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HeadlineContentStyle ContentStyle {
			get { return Styles.Content; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineDateStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HeadlineDateStyle DateStyle {
			get { return Styles.Date; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineHeaderStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HeadlineStyle HeaderStyle {
			get { return Styles.Header; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineLeftPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HeadlinePanelStyle LeftPanelStyle {
			get { return Styles.LeftPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineRightPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HeadlinePanelStyle RightPanelStyle {
			get { return Styles.RightPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxHeadlineTailStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HeadlineTailStyle TailStyle {
			get { return Styles.Tail; }
		}
		protected internal HeadlineSettings HeadlineSettings {
			get {
				if (fHeadlineSettings == null)
					fHeadlineSettings = new HeadlineSettings(this);
				return fHeadlineSettings;
			}
		}
		protected HeadlineStyles Styles {
			get { return StylesInternal as HeadlineStyles; }
		}
		protected HeadlineImages Images {
			get { return (HeadlineImages)ImagesInternal; }
		}
		public ASPxHeadline()
			: base() {
		}
		protected override bool HasContent() {
			return GetTruncatedContentText() != "" || HasDate() || HasHeader() ||
				HasTail() || HasImage();
		}
		protected override void ClearControlFields() {
			fHeadlineControl = null;
		}
		protected override void CreateControlHierarchy() {
			fHeadlineControl = new HeadlineControl(this);
			Controls.Add(fHeadlineControl);
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return false;
		}
		protected internal virtual string GetTailOnClick() {
			return "";
		}
		protected internal string GetContentToolTip() {
			if (ShowContentInToolTip && HasContentEllipsis())
				return HtmlEncode(ContentText); 
			else
				return "";
		}
		protected internal string GetToolTip() {
			if (HasMainDiv() && HasContentEllipsis() && ShowContentInToolTip)
				return GetContentToolTip();
			return ToolTip;
		}
		protected internal string GetDate() {
			return string.Format(DateFormatString, Date);
		}
		protected internal string GetHeaderText() {
			return HeaderText;
		}
		protected internal string GetTailText() {
			return TailText;
		}
		protected internal string GetLastWordPart() {
			string text = "";
			if (IsLastWordExists())
				text = GetTruncatedContentText().Substring(GetContentTextSpacePosition() + 1);
			text = HtmlEncode(text);
			if(EncodeHtml)
				text = HtmlConvertor.ToMultilineHtml(text);
			text += HasContentEllipsis() ? TextEllipsis : "";
			return text;
		}
		protected internal string GetTextPart() {
			string text = GetTruncatedContentText();
			if (IsLastWordExists())
				text = text.Substring(0, GetContentTextSpacePosition() + 1);
			text = HtmlEncode(text);
			if(EncodeHtml)
				text = HtmlConvertor.ToMultilineHtml(text);
			text += HasContentEllipsis() && !IsLastWordExists() ? TextEllipsis : "";
			return text;
		}
		protected internal string GetTruncatedContentText() {
			string s = GetContentText();
			if (EncodeHtml) {
				if (MaxLength > 0 && s.Length > MaxLength) {
					int endWordPos = s.IndexOf(' ', MaxLength);
					if (s[MaxLength - 1] != ' ' && endWordPos != -1)
						s = s.Substring(0, endWordPos);
					else
						s = s.Substring(0, MaxLength);
				}
			}
			return s;
		}
		protected internal string GetContentText() {
			if (!EncodeHtml)
				return ContentText;
			return ContentText.Trim();
		}
		protected internal int GetContentTextSpacePosition() {
			return GetTruncatedContentText().TrimEnd().LastIndexOf(" ");
		}
		protected internal bool IsLastWordExists() {
			return HasTailSpan() &&
				(TailPosition == TailPosition.KeepWithLastWord || (TailPosition == TailPosition.NewLine && HasContentEllipsis()));
		}
		protected internal bool IsSimpleRender() {
			return !HasHeader() && (!HasTail() || TailPosition != TailPosition.NewLine) &&
				(!HasDate() || IsDateInPanel());
		}
		protected internal bool HasMainDiv() {
			return Width.IsEmpty && !IsDateInPanel() && !HasImage();
		}
		protected internal bool ContentHasLink() {
			return NavigateUrl != "" && ShowContentAsLink;
		}
		protected internal bool HeaderHasLink() {
			return NavigateUrl != "" && ShowHeaderAsLink;
		}
		protected internal bool TailHasLink() {
			return NavigateUrl != "" && (TailPosition == TailPosition.NewLine || (!ContentHasLink() || (TailImageHasLink() && !IsLastWordExists())));
		}
		protected internal bool ImageHasLink() {
			return NavigateUrl != "" && ShowImageAsLink;
		}
		protected internal bool TailImageHasLink() {
			return NavigateUrl != "" && HasTailImage();
		}
		protected internal bool HasDate() {
			return Date != DateTime.MinValue;
		}
		protected internal bool HasDateSpacing() {
			return (IsDateAndImageInSamePanel() && IsDateInPanelTop()) ||
				(!IsDateInPanel() && !IsDateInBottom());
		}
		protected internal bool IsDateAndImageInSamePanel() {
			return (IsImageInLeftPanel() && IsDateInLeftPanel()) ||
				(IsImageInRightPanel() && IsDateInRightPanel());
		}
		protected internal bool IsDateInBelowHeader() {
			return HasDate() && !IsDateInPanel() &&
				DateVerticalPosition == DateVerticalPosition.BelowHeader;
		}
		protected internal bool IsDateInHeader() {
			return HasDate() && HasHeader() && !IsDateInPanel() &&
				DateVerticalPosition == DateVerticalPosition.Header;
		}
		protected internal bool IsDateInTop() {
			return HasDate() && !IsDateInPanel() &&
				(DateVerticalPosition == DateVerticalPosition.Top ||
				(!HasHeader() && DateVerticalPosition == DateVerticalPosition.Header));
		}
		protected internal bool IsDateInBottom() {
			return HasDate() && !IsDateInPanel() &&
				DateVerticalPosition == DateVerticalPosition.Bottom;
		}
		protected internal bool IsDateInPanel() {
			return DateHorizontalPosition == DateHorizontalPosition.OutsideLeft ||
				DateHorizontalPosition == DateHorizontalPosition.OutsideRight;
		}
		protected internal bool IsDateInPanelTop() {
			return IsDateInPanel() && DateVerticalPosition != DateVerticalPosition.Bottom;
		}
		protected internal bool IsDateInLeftPanel() {
			return HasDate() && DateHorizontalPosition == DateHorizontalPosition.OutsideLeft;
		}
		protected internal bool IsDateInRightPanel() {
			return HasDate() && DateHorizontalPosition == DateHorizontalPosition.OutsideRight;
		}
		protected internal bool HasImage() {
			return !Image.IsEmpty;
		}
		protected internal bool HasImageSpacing() {
			return IsDateAndImageInSamePanel() && !IsDateInPanelTop();
		}
		protected internal bool IsImageInLeftPanel() {
			return HasImage() && ImagePosition == HeadlineImagePosition.Left;
		}
		protected internal bool IsImageInRightPanel() {
			return HasImage() && ImagePosition == HeadlineImagePosition.Right;
		}
		protected internal bool IsLeftPanelNeeded() {
			return IsImageInLeftPanel() || IsDateInLeftPanel();
		}
		protected internal bool IsRightPanelNeeded() {
			return IsImageInRightPanel() || IsDateInRightPanel();
		}
		protected internal bool HasLeftPanelSpacingCell() {
			Unit spacing = GetLeftPanelSpacing();
			return IsLeftPanelNeeded() && !spacing.IsEmpty && (spacing.Value != 0);
		}
		protected internal bool HasRightPanelSpacingCell() {
			Unit spacing = GetRightPanelSpacing();
			return IsRightPanelNeeded() && !spacing.IsEmpty && (spacing.Value != 0);
		}
		protected internal bool HasHeader() {
			return HeaderText != "";
		}
		protected internal bool HasHeaderLineHeight() {
			return IsDateInPanel();
		}
		protected internal bool HasContentEllipsis() {
			return MaxLength > 0 && GetTruncatedContentText().Length < GetContentText().Length;
		}
		protected internal bool HasTail() {
			return TailText != "" || HasTailImage();
		}
		protected internal bool HasTailSpan() {
			return HasTail() && TailContainsWords() && TailPosition != TailPosition.Inline;
		}
		protected internal bool HasTailImage() {
			return !TailImage.IsEmpty;
		}
		protected internal bool IsTailInNewLine() {
			return HasTail() && TailPosition == TailPosition.NewLine;
		}
		protected internal virtual bool IsTailRequired() {
			return HasTail();
		}
		protected bool TailContainsWords() {
			return EncodeHtml && GetContentTextSpacePosition() != -1;
		}
		protected override ImagesBase CreateImages() {
			return new HeadlineImages(this);
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new HeadlineStyles(this);
		}
		protected internal Unit GetContentLineHeight() {
			return GetContentStyle().LineHeight;
		}
		protected internal Unit GetHeaderLineHeight() {
			return GetHeaderStyle().LineHeight;
		}
		protected internal Paddings GetContentPaddings() {
			return GetContentStyle().Paddings;
		}
		protected internal Paddings GetDatePaddings() {
			return GetDateStyle().Paddings;
		}
		protected internal Paddings GetHeaderPaddings() {
			return GetHeaderStyle().Paddings;
		}
		protected internal Paddings GetLeftPanelPaddings() {
			return GetLeftPanelStyle().Paddings;
		}
		protected internal Paddings GetRightPanelPaddings() {
			return GetRightPanelStyle().Paddings;
		}
		protected internal Paddings GetTailPaddings() {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Styles.GetTailPaddings());
			paddings.CopyFrom(TailStyle.Paddings);
			return paddings;
		}
		static object contentStyleKey = new object();
		protected internal HeadlineContentStyle GetContentStyle() {
			return (HeadlineContentStyle)CreateStyle(delegate() {
				HeadlineContentStyle style = new HeadlineContentStyle();
				style.CopyFrom(Styles.GetDefaultContentStyle());
				style.CopyFrom(GetCustomContentStyle());
				MergeDisableStyle(style);
				return style;
			}, contentStyleKey);
		}
		protected internal HeadlineContentStyle GetCustomContentStyle() {
			HeadlineContentStyle style = new HeadlineContentStyle();
			style.CopyFrom(ContentStyle);
			if(IsSimpleRender())
				style.MergeWith(GetControlStyle());
			if(!HasMainDiv())
				style.VerticalAlign = VerticalAlign.Top;
			return style;
		}
		protected internal HeadlineDateStyle GetDateHeaderStyle() {
			HeadlineDateStyle style = Styles.GetDefaultDateHeaderStyle();
			style.CopyFrom(DateStyle);
			style.HorizontalAlign = HorizontalAlign.NotSet;
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineDateStyle GetDateLeftPanelStyle() {
			HeadlineDateStyle style = Styles.GetDefaultDateLeftPanelStyle();
			style.CopyFrom(DateStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineDateStyle GetDateRightPanelStyle() {
			HeadlineDateStyle style = Styles.GetDefaultDateRightPanelStyle();
			style.CopyFrom(DateStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineDateStyle GetDateStyle() {
			HeadlineDateStyle style = new HeadlineDateStyle();
			style.CopyFrom(Styles.GetDefaultDateStyle());
			style.CopyFrom(DateStyle);
			if ((ControlStyle as AppearanceStyleBase).HorizontalAlign == HorizontalAlign.Justify &&
					DateStyle.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign.Justify;
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineStyle GetHeaderStyle() {
			HeadlineStyle style = new HeadlineStyle();
			style.CopyFrom(Styles.GetDefaultHeaderStyle());
			style.CopyFrom(GetCustomHeaderStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineStyle GetCustomHeaderStyle() {
			HeadlineStyle style = new HeadlineStyle();
			style.CopyFrom(HeaderStyle);
			if((ControlStyle as AppearanceStyleBase).HorizontalAlign == HorizontalAlign.Justify &&
					HeaderStyle.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign.Justify;
			return style;
		}
		protected internal HeadlinePanelStyle GetLeftPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(Styles.GetDefaultLeftPanelStyle());
			style.CopyFrom(GetCustomLeftPanelStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlinePanelStyle GetCustomLeftPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(LeftPanelStyle);
			if((ControlStyle as AppearanceStyleBase).HorizontalAlign == HorizontalAlign.Justify &&
					LeftPanelStyle.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign.Justify;
			return style;
		}
		protected internal HeadlineStyle GetRightPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(Styles.GetDefaultRightPanelStyle());			
			style.CopyFrom(GetCustomRightPanelStyle());
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineStyle GetCustomRightPanelStyle() {
			HeadlinePanelStyle style = new HeadlinePanelStyle();
			style.CopyFrom(RightPanelStyle);
			if((ControlStyle as AppearanceStyleBase).HorizontalAlign == HorizontalAlign.Justify &&
					RightPanelStyle.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign.Justify;
			return style;
		}
		protected internal HeadlineStyle GetTailDivStyle() {
			HeadlineStyle style = new HeadlineStyle();
			style.CopyFrom(Styles.GetDefaultTailDivStyle());
			style.CopyFrom(TailStyle);
			if ((ControlStyle as AppearanceStyleBase).HorizontalAlign == HorizontalAlign.Justify &&
					TailStyle.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign.Justify;
			MergeDisableStyle(style);
			return style;
		}
		protected internal HeadlineStyle GetTailStyle() {
			HeadlineStyle style = new HeadlineStyle();
			style.CopyFrom(Styles.GetDefaultTailStyle());
			style.CopyFrom(TailStyle);
			style.Paddings.Reset();
			MergeDisableStyle(style);
			return style;
		}
		protected internal Unit GetDateSpacing() {
			return GetDateStyle().Spacing;
		}
		protected internal Unit GetContentSpacing() {
			return GetContentStyle().Spacing;
		}
		protected internal Unit GetHeaderSpacing() {
			return GetHeaderStyle().Spacing;
		}
		protected internal Unit GetLeftPanelImageSpacing() {
			return GetLeftPanelStyle().ImageSpacing;
		}
		protected internal Unit GetLeftPanelSpacing() {
			return GetLeftPanelStyle().Spacing;
		}
		protected internal Unit GetRightPanelImageSpacing() {
			return GetRightPanelStyle().ImageSpacing;
		}
		protected internal Unit GetRightPanelSpacing() {
			return GetRightPanelStyle().Spacing;
		}
		protected internal Unit GetTailSpacing() {
			return GetTailDivStyle().Spacing;
		}
		protected internal Unit GetTailImageSpacing() {
			return GetTailStyle().ImageSpacing;
		}
		protected internal Unit GetTailLineHeight() {
			return TailPosition != TailPosition.NewLine ? GetCustomContentStyle().LineHeight : Unit.Empty;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { HeadlineSettings });
		}
	}
}
