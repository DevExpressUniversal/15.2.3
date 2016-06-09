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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	public class ImageSpriteProperties: PropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSpritePropertiesCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual string CssClass {
			get { return GetStringProperty("CssClass", string.Empty); }
			set {
				SetStringProperty("CssClass", string.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSpritePropertiesLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit Left {
			get { return GetUnitProperty("Left", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "Left", true);
				SetUnitProperty("Left", Unit.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSpritePropertiesTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit Top {
			get { return GetUnitProperty("Top", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "Top", true);
				SetUnitProperty("Top", Unit.Empty, value);
				Changed();
			}
		}
		protected internal string CheckedCssClass {
			get { return GetStringProperty("CheckedCssClass", string.Empty); }
			set {
				SetStringProperty("CheckedCssClass", string.Empty, value);
				Changed();
			}
		}
		protected internal Unit CheckedLeft {
			get { return GetUnitProperty("CheckedLeft", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "CheckedLeft", true);
				SetUnitProperty("CheckedLeft", Unit.Empty, value);
				Changed();
			}
		}
		protected internal Unit CheckedTop {
			get { return GetUnitProperty("CheckedTop", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "CheckedTop", true);
				SetUnitProperty("CheckedTop", Unit.Empty, value);
				Changed();
			}
		}
		protected internal string DisabledCssClass {
			get { return GetStringProperty("DisabledCssClass", string.Empty); }
			set {
				SetStringProperty("DisabledCssClass", string.Empty, value);
				Changed();
			}
		}
		protected internal Unit DisabledLeft {
			get { return GetUnitProperty("DisabledLeft", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DisabledLeft", true);
				SetUnitProperty("DisabledLeft", Unit.Empty, value);
				Changed();
			}
		}
		protected internal Unit DisabledTop {
			get { return GetUnitProperty("DisabledTop", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DisabledTop", true);
				SetUnitProperty("DisabledTop", Unit.Empty, value);
				Changed();
			}
		}
		protected internal string HottrackedCssClass {
			get { return GetStringProperty("HottrackedCssClass", string.Empty); }
			set {
				SetStringProperty("HottrackedCssClass", string.Empty, value);
				Changed();
			}
		}
		protected internal Unit HottrackedLeft {
			get { return GetUnitProperty("HottrackedLeft", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "HottrackedLeft", true);
				SetUnitProperty("HottrackedLeft", Unit.Empty, value);
				Changed();
			}
		}
		protected internal Unit HottrackedTop {
			get { return GetUnitProperty("HottrackedTop", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "HottrackedTop", true);
				SetUnitProperty("HottrackedTop", Unit.Empty, value);
				Changed();
			}
		}
		protected internal string PressedCssClass {
			get { return GetStringProperty("PressedCssClass", string.Empty); }
			set {
				SetStringProperty("PressedCssClass", string.Empty, value);
				Changed();
			}
		}
		protected internal Unit PressedLeft {
			get { return GetUnitProperty("PressedLeft", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "PressedLeft", true);
				SetUnitProperty("PressedLeft", Unit.Empty, value);
				Changed();
			}
		}
		protected internal Unit PressedTop {
			get { return GetUnitProperty("PressedTop", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "PressedTop", true);
				SetUnitProperty("PressedTop", Unit.Empty, value);
				Changed();
			}
		}
		protected internal string SelectedCssClass {
			get { return GetStringProperty("SelectedCssClass", string.Empty); }
			set {
				SetStringProperty("SelectedCssClass", string.Empty, value);
				Changed();
			}
		}
		protected internal Unit SelectedLeft {
			get { return GetUnitProperty("SelectedLeft", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "SelectedLeft", true);
				SetUnitProperty("SelectedLeft", Unit.Empty, value);
				Changed();
			}
		}
		protected internal Unit SelectedTop {
			get { return GetUnitProperty("SelectedTop", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "SelectedTop", true);
				SetUnitProperty("SelectedTop", Unit.Empty, value);
				Changed();
			}
		}
		public ImageSpriteProperties()
			: base() {
		}
		public ImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		public sealed override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				Reset();
				CopyFrom(source);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void CopyFrom(PropertiesBase properties) {
			if(properties is ImageSpriteProperties) {
				ImageSpriteProperties spriteProperties = properties as ImageSpriteProperties;
				BeginUpdate();
				try {
					if(!string.IsNullOrEmpty(spriteProperties.CssClass))
						CssClass = spriteProperties.CssClass;
					if(!spriteProperties.Left.IsEmpty)
						Left = spriteProperties.Left;
					if(!spriteProperties.Top.IsEmpty)
						Top = spriteProperties.Top;
					if(!string.IsNullOrEmpty(spriteProperties.CheckedCssClass))
						CheckedCssClass = spriteProperties.CheckedCssClass;
					if(!spriteProperties.CheckedLeft.IsEmpty)
						CheckedLeft = spriteProperties.CheckedLeft;
					if(!spriteProperties.CheckedTop.IsEmpty)
						CheckedTop = spriteProperties.CheckedTop;
					if(!string.IsNullOrEmpty(spriteProperties.DisabledCssClass))
						DisabledCssClass = spriteProperties.DisabledCssClass;
					if(!spriteProperties.DisabledLeft.IsEmpty)
						DisabledLeft = spriteProperties.DisabledLeft;
					if(!spriteProperties.DisabledTop.IsEmpty)
						DisabledTop = spriteProperties.DisabledTop;
					if(!string.IsNullOrEmpty(spriteProperties.HottrackedCssClass))
						HottrackedCssClass = spriteProperties.HottrackedCssClass;
					if(!spriteProperties.HottrackedLeft.IsEmpty)
						HottrackedLeft = spriteProperties.HottrackedLeft;
					if(!spriteProperties.HottrackedTop.IsEmpty)
						HottrackedTop = spriteProperties.HottrackedTop;
					if(!string.IsNullOrEmpty(spriteProperties.PressedCssClass))
						PressedCssClass = spriteProperties.PressedCssClass;
					if(!spriteProperties.PressedLeft.IsEmpty)
						PressedLeft = spriteProperties.PressedLeft;
					if(!spriteProperties.PressedTop.IsEmpty)
						PressedTop = spriteProperties.PressedTop;
					if(!string.IsNullOrEmpty(spriteProperties.SelectedCssClass))
						SelectedCssClass = spriteProperties.SelectedCssClass;
					if(!spriteProperties.SelectedLeft.IsEmpty)
						SelectedLeft = spriteProperties.SelectedLeft;
					if(!spriteProperties.SelectedTop.IsEmpty)
						SelectedTop = spriteProperties.SelectedTop;
				}
				finally {
					EndUpdate();
				}
			}
		}
		public virtual void MergeWith(PropertiesBase properties) {
			if(properties is ImageSpriteProperties) {
				ImageSpriteProperties spriteProperties = properties as ImageSpriteProperties;
				BeginUpdate();
				try {
					if(!string.IsNullOrEmpty(spriteProperties.CssClass) && string.IsNullOrEmpty(CssClass))
						CssClass = spriteProperties.CssClass;
					if(!spriteProperties.Left.IsEmpty && Left.IsEmpty)
						Left = spriteProperties.Left;
					if(!spriteProperties.Top.IsEmpty && Top.IsEmpty)
						Top = spriteProperties.Top;
					if(!string.IsNullOrEmpty(spriteProperties.CheckedCssClass) && string.IsNullOrEmpty(CheckedCssClass))
						CheckedCssClass = spriteProperties.CheckedCssClass;
					if(!spriteProperties.CheckedLeft.IsEmpty && CheckedLeft.IsEmpty)
						CheckedLeft = spriteProperties.CheckedLeft;
					if(!spriteProperties.CheckedTop.IsEmpty && CheckedTop.IsEmpty)
						CheckedTop = spriteProperties.CheckedTop;
					if(!string.IsNullOrEmpty(spriteProperties.DisabledCssClass) && string.IsNullOrEmpty(DisabledCssClass))
						DisabledCssClass = spriteProperties.DisabledCssClass;
					if(!spriteProperties.DisabledLeft.IsEmpty && DisabledLeft.IsEmpty)
						DisabledLeft = spriteProperties.DisabledLeft;
					if(!spriteProperties.DisabledTop.IsEmpty && DisabledTop.IsEmpty)
						DisabledTop = spriteProperties.DisabledTop;
					if(!string.IsNullOrEmpty(spriteProperties.HottrackedCssClass) && string.IsNullOrEmpty(HottrackedCssClass))
						HottrackedCssClass = spriteProperties.HottrackedCssClass;
					if(!spriteProperties.HottrackedLeft.IsEmpty && HottrackedLeft.IsEmpty)
						HottrackedLeft = spriteProperties.HottrackedLeft;
					if(!spriteProperties.HottrackedTop.IsEmpty && HottrackedTop.IsEmpty)
						HottrackedTop = spriteProperties.HottrackedTop;
					if(!string.IsNullOrEmpty(spriteProperties.PressedCssClass) && string.IsNullOrEmpty(PressedCssClass))
						PressedCssClass = spriteProperties.PressedCssClass;
					if(!spriteProperties.PressedLeft.IsEmpty && PressedLeft.IsEmpty)
						PressedLeft = spriteProperties.PressedLeft;
					if(!spriteProperties.PressedTop.IsEmpty && PressedTop.IsEmpty)
						PressedTop = spriteProperties.PressedTop;
					if(!string.IsNullOrEmpty(spriteProperties.SelectedCssClass) && string.IsNullOrEmpty(SelectedCssClass))
						SelectedCssClass = spriteProperties.SelectedCssClass;
					if(!spriteProperties.SelectedLeft.IsEmpty && SelectedLeft.IsEmpty)
						SelectedLeft = spriteProperties.SelectedLeft;
					if(!spriteProperties.SelectedTop.IsEmpty && SelectedTop.IsEmpty)
						SelectedTop = spriteProperties.SelectedTop;
				}
				finally {
					EndUpdate();
				}
			}
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				CssClass = string.Empty;
				Left = Unit.Empty;
				Top = Unit.Empty;
				CheckedCssClass = string.Empty;
				CheckedLeft = Unit.Empty;
				CheckedTop = Unit.Empty;
				DisabledCssClass = string.Empty;
				DisabledLeft = Unit.Empty;
				DisabledTop = Unit.Empty;
				HottrackedCssClass = string.Empty;
				HottrackedLeft = Unit.Empty;
				HottrackedTop = Unit.Empty;
				PressedCssClass = string.Empty;
				PressedLeft = Unit.Empty;
				PressedTop = Unit.Empty;
				SelectedCssClass = string.Empty;
				SelectedLeft = Unit.Empty;
				SelectedTop = Unit.Empty;
			}
			finally {
				EndUpdate();
			}
		}
	}
	[AutoFormatUrlPropertyClass]
	public class ImagePropertiesBase : PropertiesBase, IPropertiesOwner {
		class InternalEmptyImageProperties : ImageProperties {
			public override ImageAlign Align { get { return base.Align; } set { base.Align = value; } }
			public override string AlternateText { get { return base.AlternateText; } set { base.AlternateText = value; } }
			public override string DescriptionUrl { get { return base.DescriptionUrl; } set { base.DescriptionUrl = value; } }
			public override Unit Height { get { return base.Height; } set { base.Height = value; } }
			public override string Url { get { return base.Url; } set { base.Url = value; } }
			public override Unit Width { get { return base.Width; } set { base.Width = value; } }
			public override void CopyFrom(PropertiesBase properties) {
			}
			public override void Reset() {
			}
			public override void MergeWith(PropertiesBase properties) {
			}
		}
		private bool isResourcePng = false;
		private string spriteUrl = string.Empty;
		private ImageSpriteProperties spriteProperties;
		static object imageAlignNotSet = ImageAlign.NotSet;
		public static readonly ImageProperties Empty = new InternalEmptyImageProperties(); 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(ImageAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public virtual ImageAlign Align {
			get { return (ImageAlign)GetEnumProperty("ImageAlign", imageAlignNotSet); }
			set {
				SetEnumProperty("ImageAlign", imageAlignNotSet, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesBaseAlternateText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public virtual string AlternateText {
			get { return GetStringProperty("Alt", ""); }
			set {
				SetStringProperty("Alt", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesBaseToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public virtual string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set {
				SetStringProperty("ToolTip", "", value);
				Changed();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatEnable]
		public virtual string DescriptionUrl {
			get { return GetStringProperty("DescriptionUrl", ""); }
			set {
				SetStringProperty("DescriptionUrl", "", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesBaseIconID"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true),
		TypeConverter("DevExpress.Web.Design.IconIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), AutoFormatDisable,
		EditorAttribute("DevExpress.Web.Design.IconUITypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string IconID {
			get { return GetStringProperty("IconID", ""); }
			set {
				if(value != IconID) {
					IconsHelper.UnregisterIconID(IconID);
					SetStringProperty("IconID", "", value);
					IconsHelper.RegisterIconID(IconID);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesBaseHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "Height");
				SetUnitProperty("Height", Unit.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesBaseWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "Width");
				SetUnitProperty("Width", Unit.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesBaseUrl"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		UrlProperty, AutoFormatUrlProperty, AutoFormatEnable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string Url {
			get { return GetStringProperty("Url", ""); }
			set {
				SetStringProperty("Url", "", value);
				Changed();
			}
		}
		protected internal ImageSpriteProperties SpritePropertiesInternal {
			get {
				if(spriteProperties == null)
					spriteProperties = CreateSpriteProperties();
				return spriteProperties;
			}
		}
		protected internal virtual string UrlChecked {
			get { return GetStringProperty("UrlChecked", ""); }
			set {
				SetStringProperty("UrlChecked", "", value);
				Changed();
			}
		}
		protected internal virtual string UrlDisabled {
			get { return GetStringProperty("UrlDisabled", ""); }
			set {
				SetStringProperty("UrlDisabled", "", value);
				Changed();
			}
		}
		protected internal virtual string UrlHottracked {
			get { return GetStringProperty("UrlHottracked", ""); }
			set {
				SetStringProperty("UrlHottracked", "", value);
				Changed();
			}
		}
		protected internal virtual string UrlPressed {
			get { return GetStringProperty("UrlPressed", ""); }
			set {
				SetStringProperty("UrlPressed", "", value);
				Changed();
			}
		}
		protected internal virtual string UrlSelected {
			get { return GetStringProperty("UrlSelected", ""); }
			set {
				SetStringProperty("UrlSelected", "", value);
				Changed();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsResourcePng {
			get { return isResourcePng; }
			set { isResourcePng = value; }
		}
		protected internal virtual string SpriteUrl {
			get { return spriteUrl; }
			set { spriteUrl = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmpty {
			get {
				return string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(IconID) && !UseImageSprite(SpritePropertiesInternal.CssClass, 
					SpritePropertiesInternal.Left, SpritePropertiesInternal.Top);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmptyChecked {
			get {
				return string.IsNullOrEmpty(UrlChecked) && !UseImageSprite(SpritePropertiesInternal.CheckedCssClass, 
					SpritePropertiesInternal.CheckedLeft, SpritePropertiesInternal.CheckedTop);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmptyDisabled {
			get {
				return string.IsNullOrEmpty(UrlDisabled) && !UseImageSprite(SpritePropertiesInternal.DisabledCssClass, 
					SpritePropertiesInternal.DisabledLeft, SpritePropertiesInternal.DisabledTop);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmptyHottracked {
			get {
				return string.IsNullOrEmpty(UrlHottracked) && !UseImageSprite(SpritePropertiesInternal.HottrackedCssClass,
					SpritePropertiesInternal.HottrackedLeft, SpritePropertiesInternal.HottrackedTop);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmptyPressed {
			get {
				return string.IsNullOrEmpty(UrlPressed) && !UseImageSprite(SpritePropertiesInternal.PressedCssClass,
					SpritePropertiesInternal.PressedLeft, SpritePropertiesInternal.PressedTop);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsEmptySelected {
			get {
				return string.IsNullOrEmpty(UrlSelected) && !UseImageSprite(SpritePropertiesInternal.SelectedCssClass,
					SpritePropertiesInternal.SelectedLeft, SpritePropertiesInternal.SelectedTop);
			}
		}
		public ImagePropertiesBase()
			: base() {
		}
		public ImagePropertiesBase(string url)
			: this() {
			Url = url;
		}
		public ImagePropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		public void AssignToControl(Image image, bool designMode) {
			AssingToImage(image, Url, designMode);
		}
		protected void AssingToImage(Image image, string url, bool designMode) {
			AssingToControl(image, url, GetSpriteCssClass(false), GetSpriteLeft(false), GetSpriteTop(false), IconsHelper.GetIconCssClass(IconID, false), designMode);
		}
		public void AssignToControl(WebControl control, bool designMode, bool disabled) {
			AssingToControl(control, GetUrl(disabled), GetSpriteCssClass(disabled), GetSpriteLeft(disabled), GetSpriteTop(disabled), IconsHelper.GetIconCssClass(IconID, disabled), designMode);
		}
		public void AssignToControl(Image image, bool designMode, bool disabled) {
			AssingToControl(image, GetUrl(disabled), GetSpriteCssClass(disabled), GetSpriteLeft(disabled), GetSpriteTop(disabled), IconsHelper.GetIconCssClass(IconID, disabled), designMode);
		}
		protected string GetUrl(bool disabled) {
			return (disabled && !string.IsNullOrEmpty(UrlDisabled)) ? UrlDisabled : Url;
		}
		protected string GetSpriteCssClass(bool disabled) {
			return (disabled && !string.IsNullOrEmpty(SpritePropertiesInternal.DisabledCssClass)) ? SpritePropertiesInternal.DisabledCssClass : SpritePropertiesInternal.CssClass;
		}
		protected Unit GetSpriteLeft(bool disabled) {
			return (disabled && !SpritePropertiesInternal.DisabledLeft.IsEmpty) ? SpritePropertiesInternal.DisabledLeft : SpritePropertiesInternal.Left;
		}
		protected Unit GetSpriteTop(bool disabled) {
			return (disabled && !SpritePropertiesInternal.DisabledTop.IsEmpty) ? SpritePropertiesInternal.DisabledTop : SpritePropertiesInternal.Top;
		}
		protected void SetImageUrl(WebControl control, string value) {
			if(control is Image)
				((Image)control).ImageUrl = value;
			else
				RenderUtils.SetStyleStringAttribute(control, "background-image", value, false);
		}
		protected void AssingToControl(WebControl control, string url, string spriteCssClass, Unit spriteLeft, Unit spriteTop, string iconCssClass, bool designMode) {
			Image image = control as Image;
			if(image != null) {
				image.AlternateText = AlternateText;
				image.DescriptionUrl = DescriptionUrl;
				image.ImageAlign = Align;
			}
			control.ToolTip = ToolTip;
			control.Width = Width;
			control.Height = Height;
			if(!string.IsNullOrEmpty(url))
				SetImageUrl(control, ResourceManager.GetResourceUrl(control.Page, url));
			else if(!string.IsNullOrEmpty(IconID)) {
				if(image != null)
					SetImageUrl(control, EmptyImageProperties.GetGlobalEmptyImage(control.Page).Url);
				control.CssClass = RenderUtils.CombineCssClasses(control.CssClass, iconCssClass);
				if(designMode) {
					string iconSpriteUrl = ResourceManager.GetResourceUrl(control.Page, IconsHelper.GetIconImageResourceName(IconID));
					control.Style.Add(HtmlTextWriterStyle.BackgroundImage, iconSpriteUrl);
				}
			}
			else if(UseImageSprite(spriteCssClass, spriteLeft, spriteTop)) {
				if(image != null)
					SetImageUrl(control, EmptyImageProperties.GetGlobalEmptyImage(control.Page).Url);
				control.CssClass = RenderUtils.CombineCssClasses(control.CssClass, spriteCssClass);
				if(!string.IsNullOrEmpty(SpriteUrl)) {
					if(!designMode || !SpriteUrl.Contains("mvwres:")) {
						string background = GetSpriteRuntimeBackgroundAttribute(control, SpriteUrl, spriteLeft, spriteTop);
						if(!string.IsNullOrEmpty(background))
							control.Style.Add("background", background);
					}
					else
						control.Style.Add(HtmlTextWriterStyle.BackgroundImage, SpriteUrl);
				}
			}
			else if(RenderUtils.IsHtml5Mode(control))
				SetImageUrl(control, EmptyImageProperties.GetGlobalEmptyImage(control.Page).Url);
		}
		public object GetScriptObject(Page page) {
			if(!string.IsNullOrEmpty(IconID))
				return GetScriptObject(page, Url, IconsHelper.GetIconCssClass(IconID, false), Unit.Empty, Unit.Empty);
			else
				return GetScriptObject(page, Url, SpritePropertiesInternal.CssClass, SpritePropertiesInternal.Left, SpritePropertiesInternal.Top);
		}
		public object GetDisabledScriptObject(Page page) {
			if(!string.IsNullOrEmpty(IconID))
				return GetScriptObject(page, UrlDisabled, IconsHelper.GetIconCssClass(IconID, true), Unit.Empty, Unit.Empty);
			else
				return GetScriptObject(page, UrlDisabled, SpritePropertiesInternal.DisabledCssClass, SpritePropertiesInternal.DisabledLeft, SpritePropertiesInternal.DisabledTop);
		}
		public object GetHottrackedScriptObject(Page page) {
			return GetScriptObject(page, UrlHottracked, SpritePropertiesInternal.HottrackedCssClass, SpritePropertiesInternal.HottrackedLeft, SpritePropertiesInternal.HottrackedTop);
		}
		public object GetPressedScriptObject(Page page) {
			return GetScriptObject(page, UrlPressed, SpritePropertiesInternal.PressedCssClass, SpritePropertiesInternal.PressedLeft, SpritePropertiesInternal.PressedTop);
		}
		public object GetCheckedScriptObject(Page page) {
			return GetScriptObject(page, UrlChecked, SpritePropertiesInternal.CheckedCssClass, SpritePropertiesInternal.CheckedLeft, SpritePropertiesInternal.CheckedTop);
		}
		public object GetSelectedScriptObject(Page page) {
			return GetScriptObject(page, UrlSelected, SpritePropertiesInternal.SelectedCssClass, SpritePropertiesInternal.SelectedLeft, SpritePropertiesInternal.SelectedTop);
		}
		protected object GetScriptObject(Page page, string url, string spriteCssClass, Unit spriteLeft, Unit spriteTop) {
			object scriptObject = null;		   
			if(!string.IsNullOrEmpty(url)) {
				if(MvcUtils.RenderMode != MvcRenderMode.None)
					scriptObject = MvcUtils.ResolveClientUrl(url);
				else
					scriptObject = (page != null) ? page.ResolveClientUrl(url) : url;
			} else if(UseImageSprite(spriteCssClass, spriteLeft, spriteTop)) {
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if(!string.IsNullOrEmpty(spriteCssClass))
					dictionary["spriteCssClass"] = spriteCssClass;
				string background = GetSpriteRuntimeBackgroundAttribute(page, SpriteUrl, spriteLeft, spriteTop);
				if(!string.IsNullOrEmpty(background))
					dictionary["spriteBackground"] = background;
				scriptObject = dictionary;
			}
			return scriptObject;
		}
		protected string GetSpriteRuntimeBackgroundAttribute(Control control, string spriteUrl, Unit spriteLeft, Unit spriteTop) {
			if(string.IsNullOrEmpty(spriteUrl)) return string.Empty;
			string result = "transparent ";
			result += "url('" + HttpUtility.HtmlEncode(ResourceManager.ResolveClientUrl(control, spriteUrl)) + "') ";
			result += "no-repeat ";
			if(!spriteLeft.IsEmpty)
				result += string.Format("-{0} ", spriteLeft);
			if(!spriteTop.IsEmpty)
				result += string.Format("-{0} ", spriteTop);
			return result;
		}
		protected bool UseImageSprite(string spriteCssClass, Unit spriteLeft, Unit spriteTop) {
			return !string.IsNullOrEmpty(spriteCssClass) || 
				(!string.IsNullOrEmpty(SpriteUrl) && !spriteLeft.IsEmpty && !spriteTop.IsEmpty && !Width.IsEmpty && !Height.IsEmpty);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { SpritePropertiesInternal };
		}
		protected virtual ImageSpriteProperties CreateSpriteProperties() {
			return new ImageSpriteProperties(this);
		}
		public sealed override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				Reset();
				CopyFrom(source);
			} finally {
				EndUpdate();
			}
		}
		public virtual void CopyFrom(PropertiesBase properties) {
			if(properties is ImagePropertiesBase) {
				ImagePropertiesBase imageProperties = (ImagePropertiesBase)properties;
				BeginUpdate();
				try {
					if(imageProperties.AlternateText != "")
						AlternateText = imageProperties.AlternateText;
					if(imageProperties.ToolTip != "")
						ToolTip = imageProperties.ToolTip;
					if(imageProperties.DescriptionUrl != "")
						DescriptionUrl = imageProperties.DescriptionUrl;
					if(imageProperties.IconID != "")
						IconID = imageProperties.IconID;
					if(imageProperties.Url != "")
						Url = imageProperties.Url;
					if(imageProperties.UrlChecked != "")
						UrlChecked = imageProperties.UrlChecked;
					if(imageProperties.UrlDisabled != "")
						UrlDisabled = imageProperties.UrlDisabled;
					if(imageProperties.UrlHottracked != "")
						UrlHottracked = imageProperties.UrlHottracked;
					if(imageProperties.UrlPressed != "")
						UrlPressed = imageProperties.UrlPressed;
					if(imageProperties.UrlSelected != "")
						UrlSelected = imageProperties.UrlSelected;
					if(!imageProperties.Width.IsEmpty)
						Width = imageProperties.Width;
					if(!imageProperties.Height.IsEmpty)
						Height = imageProperties.Height;
					if(imageProperties.SpriteUrl != "")
						SpriteUrl = imageProperties.SpriteUrl;
					SpritePropertiesInternal.CopyFrom(imageProperties.SpritePropertiesInternal);
					if(imageProperties.Align != ImageAlign.NotSet)
						Align = imageProperties.Align;
				} finally {
					EndUpdate();
				}
			}
		}
		public virtual void MergeWith(PropertiesBase properties) {
			if(properties is ImagePropertiesBase) {
				ImagePropertiesBase imageProperties = (ImagePropertiesBase)properties;
				BeginUpdate();
				try {
					if(imageProperties.AlternateText != "" && AlternateText == "")
						AlternateText = imageProperties.AlternateText;
					if(imageProperties.ToolTip != "" && ToolTip == "")
						ToolTip = imageProperties.ToolTip;
					if(imageProperties.DescriptionUrl != "" && DescriptionUrl == "")
						DescriptionUrl = imageProperties.DescriptionUrl;
					if(imageProperties.IconID != "" && IconID == "")
						IconID = imageProperties.IconID;
					if(imageProperties.Url != "" && Url == "")
						Url = imageProperties.Url;
					if(imageProperties.UrlChecked != "" && UrlChecked == "")
						UrlChecked = imageProperties.UrlChecked;
					if(imageProperties.UrlDisabled != "" && UrlDisabled == "")
						UrlDisabled = imageProperties.UrlDisabled;
					if(imageProperties.UrlHottracked != "" && UrlHottracked == "")
						UrlHottracked = imageProperties.UrlHottracked;
					if(imageProperties.UrlPressed != "" && UrlPressed == "")
						UrlPressed = imageProperties.UrlPressed;
					if(imageProperties.UrlSelected != "" && UrlSelected == "")
						UrlSelected = imageProperties.UrlSelected;
					if(!imageProperties.Width.IsEmpty && Width.IsEmpty)
						Width = imageProperties.Width;
					if(!imageProperties.Height.IsEmpty && Height.IsEmpty)
						Height = imageProperties.Height;
					if(imageProperties.SpriteUrl != "" && SpriteUrl == "")
						SpriteUrl = imageProperties.SpriteUrl;
					SpritePropertiesInternal.MergeWith(imageProperties.SpritePropertiesInternal);
					if(imageProperties.Align != ImageAlign.NotSet && Align == ImageAlign.NotSet)
						Align = imageProperties.Align;
				}
				finally {
					EndUpdate();
				}
			}
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				AlternateText = "";
				ToolTip = "";
				DescriptionUrl = "";
				IconID = "";
				Url = "";
				UrlChecked = "";
				UrlDisabled = "";
				UrlHottracked = "";
				UrlPressed = "";
				UrlSelected = "";
				Width = Unit.Empty;
				Height = Unit.Empty;
				SpriteUrl = "";
				SpritePropertiesInternal.Reset();
				Align = ImageAlign.NotSet;
			} finally {
				EndUpdate();
			}
		}
		public override string ToString() {
			return CommonUtils.GetObjectText(this, true);
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			IconsHelper.RegisterIconID(IconID);
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class ImageProperties: ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public ImageSpriteProperties SpriteProperties {
			get { return SpritePropertiesInternal; }
		}
		public ImageProperties()
			: base() {
		}
		public ImageProperties(string url)
			: base(url) {
		}
		public ImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class FileManagerThumbnailProperties : ImageProperties {
		public FileManagerThumbnailProperties()
			: base() {
		}
		public FileManagerThumbnailProperties(string url)
			: base(url) {
		}
		public FileManagerThumbnailProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
	}
	public class ImageSpritePropertiesEx: ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSpritePropertiesExDisabledCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string DisabledCssClass {
			get { return base.DisabledCssClass; }
			set { base.DisabledCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSpritePropertiesExDisabledLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledLeft {
			get { return base.DisabledLeft; }
			set { base.DisabledLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSpritePropertiesExDisabledTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledTop {
			get { return base.DisabledTop; }
			set { base.DisabledTop = value; }
		}
		public ImageSpritePropertiesEx()
			: base() {
		}
		public ImageSpritePropertiesEx(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ImagePropertiesEx: ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesExSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public ImageSpritePropertiesEx SpriteProperties {
			get { return (ImageSpritePropertiesEx)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagePropertiesExUrlDisabled"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlDisabled {
			get { return base.UrlDisabled; }
			set { base.UrlDisabled = value; }
		}
		public ImagePropertiesEx()
			: base() {
		}
		public ImagePropertiesEx(string url)
			: base(url) {
			UrlDisabled = url;
		}
		public ImagePropertiesEx(string url, string urlDisabled)
			: this(url) {
			UrlDisabled = urlDisabled;
		}
		public ImagePropertiesEx(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new ImageSpritePropertiesEx(this);
		}
	}
	public class InternalCheckBoxImageProperties : ImagePropertiesEx {
		public InternalCheckBoxImageProperties() : base() { }
		public InternalCheckBoxImageProperties(string url): base(url) { }
		public InternalCheckBoxImageProperties(string url, string urlDisabled): base(url, urlDisabled) { }
		public InternalCheckBoxImageProperties(IPropertiesOwner owner): base(owner) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string AlternateText {
			get { return  string.Empty; }
			set { }
		}
	}
	public class ItemImageSpriteProperties: ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesDisabledCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string DisabledCssClass {
			get { return base.DisabledCssClass; }
			set { base.DisabledCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesDisabledLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledLeft {
			get { return base.DisabledLeft; }
			set { base.DisabledLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesDisabledTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledTop {
			get { return base.DisabledTop; }
			set { base.DisabledTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesHottrackedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string HottrackedCssClass {
			get { return base.HottrackedCssClass; }
			set { base.HottrackedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesHottrackedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedLeft {
			get { return base.HottrackedLeft; }
			set { base.HottrackedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesHottrackedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedTop {
			get { return base.HottrackedTop; }
			set { base.HottrackedTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesSelectedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string SelectedCssClass {
			get { return base.SelectedCssClass; }
			set { base.SelectedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesSelectedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit SelectedLeft {
			get { return base.SelectedLeft; }
			set { base.SelectedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImageSpritePropertiesSelectedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit SelectedTop {
			get { return base.SelectedTop; }
			set { base.SelectedTop = value; }
		}
		public ItemImageSpriteProperties()
			: base() {
		}
		public ItemImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ItemImagePropertiesBase: ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImagePropertiesBaseUrlDisabled"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlDisabled {
			get { return base.UrlDisabled; }
			set { base.UrlDisabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImagePropertiesBaseUrlHottracked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImagePropertiesBaseUrlSelected"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlSelected {
			get { return base.UrlSelected; }
			set { base.UrlSelected = value; }
		}
		public ItemImagePropertiesBase()
			: base() {
		}
		public ItemImagePropertiesBase(string url)
			: base(url) {
		}
		public ItemImagePropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new ItemImageSpriteProperties(this);
		}
	}
	public class ItemImageProperties: ItemImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ItemImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public ItemImageSpriteProperties SpriteProperties {
			get { return (ItemImageSpriteProperties)SpritePropertiesInternal; }
		}
		public ItemImageProperties()
			: base() {
		}
		public ItemImageProperties(string url)
			: base(url) {
		}
		public ItemImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ButtonImageSpriteProperties: ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesDisabledCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string DisabledCssClass {
			get { return base.DisabledCssClass; }
			set { base.DisabledCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesDisabledLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledLeft {
			get { return base.DisabledLeft; }
			set { base.DisabledLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesDisabledTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit DisabledTop {
			get { return base.DisabledTop; }
			set { base.DisabledTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesHottrackedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string HottrackedCssClass {
			get { return base.HottrackedCssClass; }
			set { base.HottrackedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesHottrackedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedLeft {
			get { return base.HottrackedLeft; }
			set { base.HottrackedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesHottrackedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedTop {
			get { return base.HottrackedTop; }
			set { base.HottrackedTop = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesPressedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string PressedCssClass {
			get { return base.PressedCssClass; }
			set { base.PressedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesPressedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit PressedLeft {
			get { return base.PressedLeft; }
			set { base.PressedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImageSpritePropertiesPressedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit PressedTop {
			get { return base.PressedTop; }
			set { base.PressedTop = value; }
		}
		public ButtonImageSpriteProperties()
			: base() {
		}
		public ButtonImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ButtonImagePropertiesBase: ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImagePropertiesBaseUrlDisabled"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlDisabled {
			get { return base.UrlDisabled; }
			set { base.UrlDisabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImagePropertiesBaseUrlHottracked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImagePropertiesBaseUrlPressed"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlPressed {
			get { return base.UrlPressed; }
			set { base.UrlPressed = value; }
		}
		public ButtonImagePropertiesBase()
			: base() {
		}
		public ButtonImagePropertiesBase(string url)
			: base(url) {
		}
		public ButtonImagePropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new ButtonImageSpriteProperties(this);
		}
	}
	public class ButtonImageProperties: ButtonImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public ButtonImageSpriteProperties SpriteProperties {
			get { return (ButtonImageSpriteProperties)SpritePropertiesInternal; }
		}
		public ButtonImageProperties()
			: base() {
		}
		public ButtonImageProperties(string url)
			: base(url) {
		}
		public ButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class CheckedButtonImageSpriteProperties: ButtonImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckedButtonImageSpritePropertiesCheckedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string CheckedCssClass {
			get { return base.CheckedCssClass; }
			set { base.CheckedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckedButtonImageSpritePropertiesCheckedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit CheckedLeft {
			get { return base.CheckedLeft; }
			set { base.CheckedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckedButtonImageSpritePropertiesCheckedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit CheckedTop {
			get { return base.CheckedTop; }
			set { base.CheckedTop = value; }
		}
		public CheckedButtonImageSpriteProperties()
			: base() {
		}
		public CheckedButtonImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class CheckedButtonImageProperties: ButtonImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckedButtonImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public CheckedButtonImageSpriteProperties SpriteProperties {
			get { return (CheckedButtonImageSpriteProperties)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckedButtonImagePropertiesUrlChecked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatUrlProperty, AutoFormatEnable]
		public new string UrlChecked {
			get { return base.UrlChecked; }
			set { base.UrlChecked = value; }
		}
		public CheckedButtonImageProperties()
			: base() {
		}
		public CheckedButtonImageProperties(string url)
			: base(url) {
		}
		public CheckedButtonImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new CheckedButtonImageSpriteProperties(this);
		}
	}
	public class HottrackedImageSpriteProperties: ImageSpriteProperties {
		[
#if !SL
	DevExpressWebLocalizedDescription("HottrackedImageSpritePropertiesHottrackedCssClass"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatEnable]
		public new string HottrackedCssClass {
			get { return base.HottrackedCssClass; }
			set { base.HottrackedCssClass = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HottrackedImageSpritePropertiesHottrackedLeft"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedLeft {
			get { return base.HottrackedLeft; }
			set { base.HottrackedLeft = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HottrackedImageSpritePropertiesHottrackedTop"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public new Unit HottrackedTop {
			get { return base.HottrackedTop; }
			set { base.HottrackedTop = value; }
		}
		public HottrackedImageSpriteProperties()
			: base() {
		}
		public HottrackedImageSpriteProperties(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class HottrackedImageProperties: ImagePropertiesBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("HottrackedImagePropertiesSpriteProperties"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatEnable]
		public HottrackedImageSpriteProperties SpriteProperties {
			get { return (HottrackedImageSpriteProperties)SpritePropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("HottrackedImagePropertiesUrlHottracked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		UrlProperty, AutoFormatEnable, AutoFormatUrlProperty]
		public new string UrlHottracked {
			get { return base.UrlHottracked; }
			set { base.UrlHottracked = value; }
		}
		public HottrackedImageProperties()
			: base() {
		}
		public HottrackedImageProperties(string url)
			: base(url) {
		}
		public HottrackedImageProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ImageSpriteProperties CreateSpriteProperties() {
			return new HottrackedImageSpriteProperties(this);
		}
	}
	public class EmptyImageProperties : ImagePropertiesBase {
		protected internal const string EmptyImageResourceName = ASPxWebControl.WebImagesResourcePath + "1x1.gif";
		static EmptyImageProperties globalEmptyImage = null;
#if !SL
	[DevExpressWebLocalizedDescription("EmptyImagePropertiesUrl")]
#endif
		public override string Url { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageAlign Align { get { return base.Align; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string AlternateText { get { return base.AlternateText; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string DescriptionUrl { get { return base.DescriptionUrl; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Height { get { return base.Height; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTip { get { return base.ToolTip; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Width { get { return base.Width; } set { } }
		public EmptyImageProperties()
			: base() {
		}
		public EmptyImageProperties(string url)
			: base(url) {
		}
		public static EmptyImageProperties GetGlobalEmptyImage(Page page) {
			if(globalEmptyImage == null || string.IsNullOrEmpty(globalEmptyImage.Url)) {
				globalEmptyImage = new EmptyImageProperties(GetDefaultEmptyImageUrl(page));
			}
			return globalEmptyImage;
		}
		public static string GetEmptyImageUrl(Page page) {
			string emptyImageUrl = GetGlobalEmptyImage(page).Url;
			return UrlUtils.IsAppRelativePath(emptyImageUrl)
				? page.ResolveClientUrl(emptyImageUrl)
				: emptyImageUrl;
		}
		protected static string GetDefaultEmptyImageUrl(Page page) {
			return ResourceManager.GetResourceUrl(page, typeof(ASPxWebControl), EmptyImageResourceName);
		}
	}
	public class ShadowImageProperties : ImagePropertiesBase {
		static ShadowImageProperties globalShadowImage = null;
#if !SL
	[DevExpressWebLocalizedDescription("ShadowImagePropertiesBottomEdgeUrl")]
#endif
		public string BottomEdgeUrl { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("ShadowImagePropertiesRightEdgeUrl")]
#endif
		public string RightEdgeUrl { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("ShadowImagePropertiesCornerUrl")]
#endif
		public string CornerUrl { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("ShadowImagePropertiesCornerWidth")]
#endif
		public Unit CornerWidth { get; set; }
#if !SL
	[DevExpressWebLocalizedDescription("ShadowImagePropertiesCornerHeight")]
#endif
		public Unit CornerHeight { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageAlign Align { get { return base.Align; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string AlternateText { get { return base.AlternateText; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string DescriptionUrl { get { return base.DescriptionUrl; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Height { get { return CornerHeight; } set { CornerHeight = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTip { get { return base.ToolTip; } set { } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Url { get { return CornerUrl; } set { CornerUrl = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Width { get { return CornerWidth; } set { CornerWidth = value; } }
		protected internal override string UrlDisabled { get { return BottomEdgeUrl; } set { BottomEdgeUrl = value; } }
		protected internal override string UrlChecked { get { return RightEdgeUrl; } set { RightEdgeUrl = value; } }
		public ShadowImageProperties()
			: base() {
		}
		public ShadowImageProperties(string bottomEdgeUrl, string rightEdgeUrl, string cornerUrl, Unit cornerWidth, Unit cornerHeight)
			: base() {
			BottomEdgeUrl = bottomEdgeUrl;
			RightEdgeUrl = rightEdgeUrl;
			CornerUrl = cornerUrl;
			CornerWidth = cornerWidth;
			CornerHeight = cornerHeight;
		}
		public override string ToString() {
			return BottomEdgeUrl + " " + CornerUrl + " " + RightEdgeUrl;
		}
		public static ShadowImageProperties GetGlobalShadowImage(Page page) {
			if(globalShadowImage == null) 
				globalShadowImage = new ShadowImageProperties();
			return globalShadowImage;
		}
	}
	[AutoFormatUrlPropertyClass]
	public class ImagesBase : PropertiesBase, IPropertiesOwner {
		public const string HottrackedPostfixConst = "Hover";
		public const string SelectedPostfixConst = "Selected";
		public const string PressedPostfixConst = "Pressed";
		public const string DisabledPostfixConst = "Disabled";
		public const string CheckedPostfixConst = "Checked";
		public const string SpriteImageName = "sprite";
		public const string LoadingPanelImageName = "Loading";
		public const string WindowResizerImageName = "WindowResizer";
		public const string WindowResizerRtlImageName = "WindowResizerRtl";
		private const string LoadingImageCssClassName = "dxlp-loadingImage";
		private Dictionary<string, ImagePropertiesBase> images = new Dictionary<string, ImagePropertiesBase>();
		private List<ImageInfo> infoList;
		private Dictionary<string, ImageInfo> infoIndex;
		static Dictionary<Type, List<ImageInfo>> typedInfoList = new Dictionary<Type, List<ImageInfo>>();
		static Dictionary<Type, Dictionary<string, ImageInfo>> typedInfoIndex = new Dictionary<Type, Dictionary<string, ImageInfo>>();
		public ImagesBase(ISkinOwner owner)
			: base(owner) {
			CreateImages();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagesBaseLoadingPanel"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImageProperties LoadingPanel {
			get { return GetImage(LoadingPanelImageName); }
		}
		protected internal virtual ImageProperties GetDefaultLoadingImageProperties() {
			ImageProperties result = new ImageProperties();
			result.SpriteProperties.CssClass = LoadingImageCssClassName;
			return result;
		}
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		protected InternalCheckBoxImageProperties CheckedImage { get { return (InternalCheckBoxImageProperties)GetImageBase(InternalCheckboxControl.CheckBoxCheckedImageName); } }
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		protected InternalCheckBoxImageProperties UncheckedImage { get { return (InternalCheckBoxImageProperties)GetImageBase(InternalCheckboxControl.CheckBoxUncheckedImageName); } }
		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		protected InternalCheckBoxImageProperties GrayedImage { get { return (InternalCheckBoxImageProperties)GetImageBase(InternalCheckboxControl.CheckBoxGrayedImageName); } }
		protected virtual ImageProperties WindowResizerInternal {
			get { return GetImage(WindowResizerImageName); }
		}
		protected virtual ImageProperties WindowResizerRtlInternal {
			get { return GetImage(WindowResizerRtlImageName); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagesBaseImageFolder"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder {
			get { return GetStringProperty("ImageFolder", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				UrlUtils.ValidateFolderUrl(ref value);
				if(value == ImageFolder) return;
				SetStringProperty("ImageFolder", string.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagesBaseSpriteImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false), UrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		AutoFormatEnable, AutoFormatUrlProperty]
		public string SpriteImageUrl {
			get { return GetStringProperty("SpriteImageUrl", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(value == SpriteImageUrl) return;
				SetStringProperty("SpriteImageUrl", string.Empty, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImagesBaseSpriteCssFilePath"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false), UrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)),
		AutoFormatEnable, AutoFormatUrlProperty]
		public string SpriteCssFilePath {
			get { return GetStringProperty("SpriteCssFilePath", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(value == SpriteCssFilePath) return;
				SetStringProperty("SpriteCssFilePath", string.Empty, value);
				Changed();
			}
		}
		protected ISkinOwner SkinOwner {
			get { return Owner as ISkinOwner; }
		}
		Dictionary<string, ImagePropertiesBase> Images {
			get { return images; }
		}
		List<ImageInfo> InfoList {
			get { return infoList; }
		}
		protected Dictionary<string, ImageInfo> InfoIndex {
			get { return infoIndex; }
		}
		protected virtual bool KeepDefaultSizes {
			get { return false; }
		}
		#region Postfixes
		protected virtual string HottrackedPostfix {
			get { return HottrackedPostfixConst; }
		}
		protected virtual string SelectedPostfix {
			get { return SelectedPostfixConst; }
		}
		protected virtual string PressedPostfix {
			get { return PressedPostfixConst; }
		}
		protected virtual string DisabledPostfix {
			get { return DisabledPostfixConst; }
		}
		protected virtual string CheckedPostfix {
			get { return CheckedPostfixConst; }
		}
		#endregion
		#region Assign, CopyFrom, Reset
		public sealed override void Assign(PropertiesBase source) {
			base.Assign(source);
			ImagesBase srcImages = source as ImagesBase;
			if(srcImages != null) {
				Reset();
				CopyFrom(srcImages);
			}
		}
		public virtual void CopyFrom(ImagesBase source) {
			if(!string.IsNullOrEmpty(source.ImageFolder))
				ImageFolder = source.ImageFolder;
			if(!string.IsNullOrEmpty(source.SpriteImageUrl))
				SpriteImageUrl = source.SpriteImageUrl;
			if(!string.IsNullOrEmpty(source.SpriteCssFilePath))
				SpriteCssFilePath = source.SpriteCssFilePath;
			foreach(KeyValuePair<string, ImagePropertiesBase> image in source.Images) {
				ImagePropertiesBase prop = GetImageBase(image.Key);
				if(prop != null)
					prop.CopyFrom(image.Value);
			}
		}
		public virtual void Reset() {
			Images.Clear();
		}
		#endregion
		public void UpdateSpriteUrl(ImagePropertiesEx properties, Page page) {
			UpdateSpriteUrl(properties, page, string.Empty, null, string.Empty);
		}
		public void UpdateSpriteUrl(ImagePropertiesEx properties, Page page, string spriteControlName, Type resourceType, string resourcePath) {
			properties.SpriteUrl = GetSpriteImageUrl(page, spriteControlName, resourceType, resourcePath);
		}
		public ImageProperties GetImageProperties(Page page, string imageName) {
			return GetImageProperties(page, imageName, false);
		}
		public virtual ImageProperties GetImageProperties(Page page, string imageName, bool encode) {
			ImageInfo info = InfoIndex[imageName];
			ImageProperties result = new ImageProperties();
			if(info.AltTextEvaluator != null)
				result.AlternateText = info.AltTextEvaluator();
			else if(!string.IsNullOrEmpty(info.AltText))
				result.AlternateText = info.AltText;
			if(info.HasResourceImage) {
				if(HasImageFolder() && KeepDefaultSizes) {
					result.Width = info.Width;
					result.Height = info.Height;
				}
				result.Url = GetUrl(page, info, string.Empty, encode, info.SpriteCssClass);
				if(info.HasDisabledState)
					result.UrlDisabled = GetUrl(page, info, DisabledPostfix, encode, info.SpriteCssClass);
				if(info.HasHottrackState)
					result.UrlHottracked = GetUrl(page, info, HottrackedPostfix, encode, info.SpriteCssClass);
				if(info.HasPressedState)
					result.UrlPressed = GetUrl(page, info, PressedPostfix, encode, info.SpriteCssClass);
				if(info.HasSelectedState)
					result.UrlSelected = GetUrl(page, info, SelectedPostfix, encode, info.SpriteCssClass);
				if(info.HasCheckedState)
					result.UrlChecked = GetUrl(page, info, CheckedPostfix, encode, info.SpriteCssClass);
			}
			string spriteUrl = GetSpriteImageUrl(page);
			if(info.HasSprite || !string.IsNullOrEmpty(spriteUrl)) {
				result.SpriteUrl = spriteUrl;
				result.SpritePropertiesInternal.CssClass = GetSpriteClassName(page, info, string.Empty);
				if(info.HasDisabledState) 
					result.SpritePropertiesInternal.DisabledCssClass = GetSpriteClassName(page, info, DisabledPostfix);
				if(info.HasHottrackState) 
					result.SpritePropertiesInternal.HottrackedCssClass = GetSpriteClassName(page, info, HottrackedPostfix);
				if(info.HasPressedState) 
					result.SpritePropertiesInternal.PressedCssClass = GetSpriteClassName(page, info, PressedPostfix);
				if(info.HasSelectedState) 
					result.SpritePropertiesInternal.SelectedCssClass = GetSpriteClassName(page, info, SelectedPostfix);
				if(info.HasCheckedState) 
					result.SpritePropertiesInternal.CheckedCssClass = GetSpriteClassName(page, info, CheckedPostfix);
			}
			result.CopyFrom(GetImageBase(imageName));
			return result;
		}
		[Obsolete]
		public ImageProperties GetImagePropertiesEx(Page page, string imageName, string disabledImageName, 
			string hoverImageName, string pressedImageName, string selectedImageName) {
			ImageProperties props = GetImageProperties(page, imageName);
			if(!string.IsNullOrEmpty(disabledImageName))
				props.UrlDisabled = disabledImageName;
			if(!string.IsNullOrEmpty(hoverImageName))
				props.UrlHottracked = hoverImageName;
			if(!string.IsNullOrEmpty(pressedImageName))
				props.UrlPressed = pressedImageName;
			if(!string.IsNullOrEmpty(selectedImageName))
				props.UrlSelected = selectedImageName;
			return props;
		}
		public static string GetSpriteLevelImageResource(Page page, ISkinOwner skinOwner, string defaultResourcePath, string imageName) {
			if(string.IsNullOrEmpty(skinOwner.GetCssPostFix()))
				return ResourceManager.GetResourceUrl(page, skinOwner.GetType(), defaultResourcePath + imageName);
			else if(ThemesProvider.IsThemePublic(skinOwner.GetCssPostFix())) {
				string spriteCssFilePath = string.Format(skinOwner.GetSpriteCssFilePath(), skinOwner.GetControlName());
				string[] spriteCssFilePathParts = spriteCssFilePath.Split('/');
				string imagePath = string.Join("/", spriteCssFilePathParts, 0, spriteCssFilePathParts.Length - 1) + "/" + imageName;
				return ResourceManager.GetResourceUrl(page, imagePath);
			}
			return string.Empty;
		}
		protected virtual Type GetResourceType() {
			return typeof(ASPxWebControl);
		}
		protected virtual string GetResourceImagePath() {
			return ASPxWebControl.WebImagesResourcePath;
		}
		protected virtual string GetDesignTimeResourceSpriteImagePath() {
			return ASPxWebControl.WebImagesResourcePath + GetDesignTimeResourceSpriteImageDefaultName();
		}
		protected string GetDesignTimeResourceSpriteImageDefaultName() {
			return SpriteImageName + ".png";
		}
		protected virtual string GetResourceSpriteCssPath() {
			return ASPxWebControl.WebSpriteCssResourceName;
		}
		protected virtual string GetImageCategory() {
			return string.Empty;
		}
		protected virtual string GetSpriteImageFileName() {
			return string.Empty;
		}
		protected bool UseDefaultSpriteCssFile() {
			foreach(ImageInfo info in InfoList) {
				if(info.HasSprite)
					return true;
			}
			return false;
		}
		protected virtual internal void RegisterDefaultSpriteCssFile(Page page) {
			if(UseDefaultSpriteCssFile())
				ResourceManager.RegisterCssResource(page, GetResourceType(), GetResourceSpriteCssPath());
		}
		protected virtual void PopulateImageInfoList(List<ImageInfo> list) {
			list.Add(new ImageInfo(LoadingPanelImageName));
			list.Add(new ImageInfo(InternalCheckboxControl.CheckBoxCheckedImageName, ImageFlags.HasDisabledState, string.Empty, typeof(InternalCheckBoxImageProperties), InternalCheckboxControl.CheckBoxCheckedImageName, InternalCheckboxControl.WebSpriteControlName));
			list.Add(new ImageInfo(InternalCheckboxControl.CheckBoxUncheckedImageName, ImageFlags.HasDisabledState, string.Empty, typeof(InternalCheckBoxImageProperties), InternalCheckboxControl.CheckBoxUncheckedImageName, InternalCheckboxControl.WebSpriteControlName));
			list.Add(new ImageInfo(InternalCheckboxControl.CheckBoxGrayedImageName, ImageFlags.HasDisabledState, string.Empty, typeof(InternalCheckBoxImageProperties), InternalCheckboxControl.CheckBoxGrayedImageName, InternalCheckboxControl.WebSpriteControlName));
		}
		protected ImageProperties GetImage(string imageName) {
			return (ImageProperties)GetImageBase(imageName);
		}
		protected ImageProperties GetImage(string imageName, bool create) {
			return (ImageProperties)GetImageBase(imageName, create);
		}
		protected ImagePropertiesBase GetImageBase(string imageName) {
			return GetImageBase(imageName, true);
		}
		protected ImagePropertiesBase GetImageBase(string imageName, bool create) {
			if(string.IsNullOrEmpty(imageName)) 
				return null;
			ImagePropertiesBase image;
			if(!Images.TryGetValue(imageName, out image) && create) {
				ImageInfo info;
				if(InfoIndex.TryGetValue(imageName, out info)) {
					if(info.PropertiesType == typeof(ImageProperties))
						image = new ImageProperties();
					else
						image = (ImagePropertiesBase)Activator.CreateInstance(info.PropertiesType, this);
					TrackViewState(image);
				}
				Images.Add(imageName, image);
			}
			return image;
		}
		string cachedFolderResult;
		string cachedFolder;
		protected string GetImageFolder() {
			if(SkinOwner == null) 
				return ImageFolder;
			string folder = SkinOwner.GetImageFolder();
			if(cachedFolder != folder) {
				cachedFolder = folder;
				cachedFolderResult = string.Format(folder, SkinOwner.GetControlName());
			}
			return cachedFolderResult;
		}
		string cachedSpriteUrlResult;
		string cachedSpriteUrl;
		protected string GetSpriteImageUrl(Page page, string spriteControlName, Type resourceType, string resurcePath) {
			if(SkinOwner == null)
				return SpriteImageUrl;
			string url = SkinOwner.GetSpriteImageUrl();
			if(cachedSpriteUrl != url || !string.IsNullOrEmpty(spriteControlName) || !string.IsNullOrEmpty(GetSpriteImageFileName())) {
				cachedSpriteUrl = url;
				cachedSpriteUrlResult = string.Format(url, SkinOwner.GetControlName());
				if(string.IsNullOrEmpty(cachedSpriteUrlResult)) {
					bool designMode = page != null && page.Site != null;
					if(designMode && SkinOwner != null) {
						if(string.IsNullOrEmpty(GetCssPostFix())) {
							if(!string.IsNullOrEmpty(GetDesignTimeResourceSpriteImagePath())) {
								string path = string.IsNullOrEmpty(resurcePath) ? GetDesignTimeResourceSpriteImagePath() : resurcePath;
								cachedSpriteUrlResult = ResourceManager.GetResourceUrl(page, resourceType ?? GetResourceType(), path);
							}
						}
						else if(ThemesProvider.IsThemePublic(GetCssPostFix())) {
							string spriteCssFilePath = string.Format(SkinOwner.GetSpriteCssFilePath(), SkinOwner.GetControlName());
							string spriteImageFilePath = string.Format(SkinOwner.GetSpriteCssFilePath(), string.IsNullOrEmpty(spriteControlName) ? SkinOwner.GetControlName() : spriteControlName);
							if(!string.IsNullOrEmpty(GetSpriteImageFileName())) {
								spriteCssFilePath = spriteImageFilePath.Replace(SpriteImageName, GetSpriteImageFileName());
								spriteImageFilePath = spriteImageFilePath.Replace(SpriteImageName, GetSpriteImageFileName());
							}
							spriteImageFilePath = spriteImageFilePath.Replace(".css", ".png");
							cachedSpriteUrlResult = ResourceManager.GetResourceUrl(page, spriteImageFilePath);
						}
					}
				}
			}
			return cachedSpriteUrlResult;
		}
		protected string GetSpriteImageUrl(Page page) {
			return GetSpriteImageUrl(page, string.Empty, null, string.Empty);
		}
		protected void CreateIndex() {
			this.infoIndex = new Dictionary<string, ImageInfo>(InfoList.Count);
			foreach(ImageInfo info in InfoList)
				InfoIndex[info.Prefix] = info;
		}
		internal string GetUrlInternal(string imgFileName, bool isResource) {
			string url = isResource ? GetResourceImagePath() : GetImageFolder();
			string cat = GetImageCategory();
			if(!string.IsNullOrEmpty(cat))
				return url + cat + (isResource ? "." : "/") + imgFileName;
			else
				return url + imgFileName;
		}
		protected string GetUrl(Page page, ImageInfo info, string postfix, bool encode, string spriteCssClass) {
			bool useResImage = !HasImageFolder();
			string fileType = info.IsPng ? "png" : "gif";
			string fileName = info.Prefix + postfix + "." + fileType;
			if(useResImage) {
				if(info.HasResourceImage && string.IsNullOrEmpty(spriteCssClass))
					return ResourceManager.GetResourceUrl(page, GetResourceType(), GetUrlInternal(fileName, true));
				return string.Empty;
			}
			else 
				return GetUrlInternal(fileName, false);
		}
		protected string GetSpriteClassName(Page page, ImageInfo info, string postfix) {
			if(!string.IsNullOrEmpty(info.SpriteCssClass)) {
				string result = "dx";
				if(SkinOwner != null) {
					string controlName = string.IsNullOrEmpty(info.ControlName) ? SkinOwner.GetControlName() : info.ControlName;
					result += controlName + "_";
				}
				if(!string.IsNullOrEmpty(GetImageCategory())) 
					result += GetImageCategory() + "_";
				result += info.SpriteCssClass + postfix;
				if(!string.IsNullOrEmpty(GetCssPostFix())) 
					result += "_" + GetCssPostFix();
				return result;
			}
			return string.Empty;
		}
		protected virtual string GetCssPostFix() {
			if(SkinOwner != null)
				return SkinOwner.GetCssPostFix();
			return string.Empty;
		}
		private void CreateImages() {
			if(!typedInfoList.TryGetValue(GetType(), out infoList)) {
				this.infoList = new List<ImageInfo>();
				PopulateImageInfoList(InfoList);
				CreateIndex();
				lock(typedInfoList) {
					typedInfoIndex[GetType()] = this.infoIndex;
					typedInfoList[GetType()] = this.infoList;
				}
			} else
				infoIndex = typedInfoIndex[GetType()];
		}
		private bool HasImageFolder() {
			return !string.IsNullOrEmpty(GetImageFolder());
		}
		public override string ToString() {
			return string.Empty;
		}
		static Dictionary<Type, GetStateManagerObject[]> stateDelegates = new Dictionary<Type, GetStateManagerObject[]>();
		protected override GetStateManagerObject[] GetStateManagedObjectsDelegates() {
			GetStateManagerObject[] state;
			if(!stateDelegates.TryGetValue(GetType(), out state)) {
				List<GetStateManagerObject> res = new List<GetStateManagerObject>();
				foreach(ImageInfo info in InfoList) {
					string name = info.Prefix.ToString(); 
					res.Add(delegate(object images, bool create) { return ((ImagesBase)images).GetImageBase(name, create); });
				}
				state = res.ToArray();
				lock(stateDelegates)
					stateDelegates[GetType()] =  state;
			}
			return state;
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			if(Owner != null) 
				Owner.Changed(this);
		}
	}
}
namespace DevExpress.Web.Internal {
	[Flags]
	public enum ImageFlags {
		Empty				   = 0x000,
		IsPng				   = 0x001, 
		HasNoResourceImage	  = 0x004,
		HasHottrackState		= 0x008, 
		HasSelectedState		= 0x010, 
		HasPressedState		 = 0x020, 
		HasDisabledState		= 0x040,
		HasCheckedState		= 0x080
	}
	public static class ImageFlasgPresets {
		public static ImageFlags PngButton = ImageFlags.IsPng | ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasPressedState;
		public static ImageFlags PngItem = ImageFlags.IsPng | ImageFlags.HasDisabledState | ImageFlags.HasHottrackState | ImageFlags.HasSelectedState;
	}
	public class ImageInfo {
		private string spriteCssClass = string.Empty;
		private string controlName = string.Empty;
		private string prefix = string.Empty;
		private ImageFlags flags = ImageFlags.Empty;
		private Unit width = Unit.Empty;
		private Unit heigth = Unit.Empty;
		private string altText = string.Empty;
		private Type propertiesType = typeof(ImageProperties);
		public delegate string StringFunction();
		private StringFunction altTextEvaluator;
		public ImageInfo() {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, string altText, Type propertiesType, string spriteCssClass) {
			Prefix = prefix;
			Flags = flags;
			Width = width;
			Height = heigth;
			AltText = altText;
			if(propertiesType != null)
				PropertiesType = propertiesType;
			SpriteCssClass = spriteCssClass;
		}
		public string ControlName {
			get { return controlName; }
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, StringFunction altTextEvaluator, Type propertiesType, string spriteCssClass)
			: this(prefix, flags, width, heigth, String.Empty, propertiesType, spriteCssClass) {
			this.altTextEvaluator = altTextEvaluator;
		}
		public ImageInfo(string prefix, ImageFlags flags, StringFunction altTextEvaluator, Type propertiesType, string spriteCssClass)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altTextEvaluator, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, StringFunction altTextEvaluator, Type propertiesType)
			: this(prefix, flags, width, heigth, altTextEvaluator, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, StringFunction altTextEvaluator)
			: this(prefix, flags, width, heigth, altTextEvaluator, null, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, StringFunction altTextEvaluator, string spriteCssClass)
			: this(prefix, flags, width, heigth, altTextEvaluator, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, StringFunction altTextEvaluator, Type propertiesType, string spriteCssClass)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, altTextEvaluator, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix, StringFunction altTextEvaluator, Type propertiesType)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, altTextEvaluator, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, StringFunction altTextEvaluator)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altTextEvaluator, null, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, StringFunction altTextEvaluator, string spriteCssClass)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altTextEvaluator, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, StringFunction altTextEvaluator, string spriteCssClass)
			: this(prefix, ImageFlags.Empty, altTextEvaluator, spriteCssClass) {
		}
		public ImageInfo(string prefix, StringFunction altTextEvaluator)
			: this(prefix, ImageFlags.Empty, altTextEvaluator, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, string altText, Type propertiesType) 
			: this(prefix, flags, width, heigth, altText, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, string altText, Type propertiesType)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altText, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, string altText, Type propertiesType, string spriteCssClass)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altText, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, string altText, Type propertiesType, string spriteCssClass, string controlName)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altText, propertiesType, spriteCssClass) {
				this.controlName = controlName;
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, string altText, string spriteCssClass)
			: this(prefix, flags, width, heigth, altText, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit side, string altText, string spriteCssClass)
			: this(prefix, flags, side, side, altText, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth)
			: this(prefix, flags, width, heigth, string.Empty, null, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit width, Unit heigth, string spriteCssClass)
			: this(prefix, flags, width, heigth, string.Empty, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit side)
			: this(prefix, flags, side, side, string.Empty, null, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit side, string spriteCssClass)
			: this(prefix, flags, side, side, string.Empty, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags)
			: this(prefix, flags, Unit.Empty, Unit.Empty, string.Empty, null, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, string spriteCssClass)
			: this(prefix, flags, Unit.Empty, Unit.Empty, string.Empty, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit side, Type propertiesType)
			: this(prefix, flags, side, side, string.Empty, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Unit side, Type propertiesType, string spriteCssClass)
			: this(prefix, flags, side, side, string.Empty, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Type propertiesType, string spriteCssClass)
			: this(prefix, flags, Unit.Empty, Unit.Empty, string.Empty, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, string altText, string spriteCssClass)
			: this(prefix, flags, Unit.Empty, Unit.Empty, altText, null, spriteCssClass) {
		}
		public ImageInfo(string prefix, ImageFlags flags, Type propertiesType)
			: this(prefix, flags, Unit.Empty, Unit.Empty, string.Empty, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, Type propertiesType)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, string.Empty, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, Type propertiesType, string spriteCssClass)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, string.Empty, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix, string altText, string spriteCssClass)
			: this(prefix, ImageFlags.Empty, Unit.Empty, altText, spriteCssClass) {
		}
		public ImageInfo(string prefix, string altText, Type propertiesType)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, altText, propertiesType, string.Empty) {
		}
		public ImageInfo(string prefix, string altText, Type propertiesType, string spriteCssClass)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, altText, propertiesType, spriteCssClass) {
		}
		public ImageInfo(string prefix)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, string.Empty, null, string.Empty) {
		}
		public ImageInfo(string prefix, string spriteCssClass)
			: this(prefix, ImageFlags.Empty, Unit.Empty, Unit.Empty, string.Empty, null, spriteCssClass) {
		}
		public string Prefix {
			get { return prefix; }
			set { prefix = value; }
		}
		public string AltText {
			get { return altText; }
			set { altText = value; }
		}
		public StringFunction AltTextEvaluator {
			get { return altTextEvaluator; }
			set { altTextEvaluator = value; }
		}
		public Unit Width {
			get { return width; }
			set { width = value; }
		}
		public Unit Height {
			get { return heigth; }
			set { heigth = value; }
		}
		public Type PropertiesType {
			get { return propertiesType; }
			set { propertiesType = value; }
		}
		public string SpriteCssClass {
			get { return spriteCssClass; }
			set { spriteCssClass = value; }
		}
		#region Flag based properties
		public bool IsPng {
			get { return HasFlags(ImageFlags.IsPng); }			
		}
		public bool HasResourceImage {
			get { return !HasFlags(ImageFlags.HasNoResourceImage); }
		}
		public bool HasHottrackState {
			get { return HasFlags(ImageFlags.HasHottrackState); }
		}
		public bool HasDisabledState {
			get { return HasFlags(ImageFlags.HasDisabledState); }
		}
		public bool HasPressedState {
			get { return HasFlags(ImageFlags.HasPressedState); }
		}
		public bool HasSelectedState {
			get { return HasFlags(ImageFlags.HasSelectedState); }
		}
		public bool HasCheckedState {
			get { return HasFlags(ImageFlags.HasCheckedState); }
		}
		public bool HasSprite {
			get { return !string.IsNullOrEmpty(SpriteCssClass); }
		}
		#endregion
		protected ImageFlags Flags {
			get { return flags; }
			set { flags = value; }
		}
		protected bool HasFlags(ImageFlags flags) {
			return (Flags & flags) == flags;
		}		
	}
}
namespace DevExpress.Web.Internal {
	public static class IconsHelper {
		public const string GrayScalePostfix = "gray";
		public const string Office2013Postfix = "office2013";
		public const string DevAVPostfix = "devav";
		const string IconsResourcePath = "Icons/";
		const string IconSpriteImage16x16ResourceName = IconsResourcePath + "Sprite_16x16.png";
		const string IconSpriteImage16x16GrayResourceName = IconsResourcePath + "Sprite_16x16" + GrayScalePostfix + ".png";
		const string IconSpriteImage16x16Office2013ResourceName = IconsResourcePath + "Sprite_16x16" + Office2013Postfix + ".png";
		const string IconSpriteImage16x16DevAVResourceName = IconsResourcePath + "Sprite_16x16" + DevAVPostfix + ".png";
		const string IconSpriteImage32x32ResourceName = IconsResourcePath + "Sprite_32x32.png";
		const string IconSpriteImage32x32GrayResourceName = IconsResourcePath + "Sprite_32x32" + GrayScalePostfix + ".png";
		const string IconSpriteImage32x32Office2013ResourceName = IconsResourcePath + "Sprite_32x32" + Office2013Postfix + ".png";
		const string IconSpriteImage32x32DevAVResourceName = IconsResourcePath + "Sprite_32x32" + DevAVPostfix + ".png";
		const string IconSpriteCss16x16ResourceName = IconsResourcePath + "Sprite_16x16.css";
		const string IconSpriteCss16x16GrayResourceName = IconsResourcePath + "Sprite_16x16" + GrayScalePostfix + ".css";
		const string IconSpriteCss16x16Office2013ResourceName = IconsResourcePath + "Sprite_16x16" + Office2013Postfix + ".css";
		const string IconSpriteCss16x16DevAVResourceName = IconsResourcePath + "Sprite_16x16" + DevAVPostfix + ".css";
		const string IconSpriteCss32x32ResourceName = IconsResourcePath + "Sprite_32x32.css";
		const string IconSpriteCss32x32GrayResourceName = IconsResourcePath + "Sprite_32x32" + GrayScalePostfix + ".css";
		const string IconSpriteCss32x32Office2013ResourceName = IconsResourcePath + "Sprite_32x32" + Office2013Postfix + ".css";
		const string IconSpriteCss32x32DevAVResourceName = IconsResourcePath + "Sprite_32x32" + DevAVPostfix + ".css";
		static List<string> registeredIconIDs = new List<string>();
		internal static List<string> RegisteredIconIDs {
			get {
				if(HttpContext.Current == null)
					return registeredIconIDs;
				return HttpUtils.GetContextObject<List<string>>("DXRegisteredIconIDs");
			}
		}
		public static void RegisterIconID(string id) {
			if(!string.IsNullOrEmpty(id) && !RegisteredIconIDs.Contains(id))
				RegisteredIconIDs.Add(id);
		}
		public static void UnregisterIconID(string id) {
			if(!string.IsNullOrEmpty(id) && RegisteredIconIDs.Contains(id))
				RegisteredIconIDs.Remove(id);
		}
		public static string GetIconCssClass(string iconID, bool disabled) {
			return "dxIcon_" + iconID + (disabled ? "_disabled" : "");
		}
		public static string GetIconImageResourceName(string iconID) {
			if(iconID.Contains("16x16" + DevAVPostfix))
				return IconSpriteImage16x16DevAVResourceName;
			else if(iconID.Contains("16x16" + Office2013Postfix))
				return IconSpriteImage16x16Office2013ResourceName;
			else if(iconID.Contains("16x16" + GrayScalePostfix))
				return IconSpriteImage16x16GrayResourceName;
			else if(iconID.Contains("16x16"))
				return IconSpriteImage16x16ResourceName;
			else if(iconID.Contains("32x32" + DevAVPostfix))
				return IconSpriteImage32x32DevAVResourceName;
			else if(iconID.Contains("32x32" + Office2013Postfix))
				return IconSpriteImage32x32Office2013ResourceName;
			else if(iconID.Contains("32x32" + GrayScalePostfix))
				return IconSpriteImage32x32GrayResourceName;
			else if(iconID.Contains("32x32"))
				return IconSpriteImage32x32ResourceName;
			return string.Empty;
		}
		public static string GetIconCssResourceName(string iconID) {
			if(iconID.Contains("16x16" + Office2013Postfix))
				return IconSpriteCss16x16Office2013ResourceName;
			else if(iconID.Contains("16x16" + DevAVPostfix))
				return IconSpriteCss16x16DevAVResourceName;
			else if(iconID.Contains("16x16" + GrayScalePostfix))
				return IconSpriteCss16x16GrayResourceName;
			else if(iconID.Contains("16x16"))
				return IconSpriteCss16x16ResourceName;
			else if(iconID.Contains("32x32" + DevAVPostfix))
				return IconSpriteCss32x32DevAVResourceName;
			else if(iconID.Contains("32x32" + Office2013Postfix))
				return IconSpriteCss32x32Office2013ResourceName;
			else if(iconID.Contains("32x32" + GrayScalePostfix))
				return IconSpriteCss32x32GrayResourceName;
			else if(iconID.Contains("32x32"))
				return IconSpriteCss32x32ResourceName;
			return string.Empty;
		}
		public static void RegisterIconSpriteCssFiles(Page page) {
			foreach(string id in RegisteredIconIDs) 
				ResourceManager.RegisterCssResource(page, GetIconCssResourceName(id));
		}
	}
}
