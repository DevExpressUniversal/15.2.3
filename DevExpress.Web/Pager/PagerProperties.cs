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
using System.Text;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Localization;
	public enum PagerButtonPosition { Left, Right, Inside }
	public enum PagerPageSizePosition { Left, Right }
	[AutoFormatUrlPropertyClass]
	public class PagerButtonProperties: PropertiesBase, IPropertiesOwner {
		protected const string DefaultTextFormatString = "{0}"; 
		ImagePosition fDefaultImagePosition = ImagePosition.Left;
		string fDefaultText = "";
		ImagePropertiesEx fImage = null;
		internal protected bool fDefaultVisible = false;
		[
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ImagePropertiesEx Image {
			get {
				if(fImage == null)
					fImage = new ImagePropertiesEx(this);
				return fImage;
			}
		}
		[
		NotifyParentProperty(true), AutoFormatEnable]
		public virtual ImagePosition ImagePosition {
			get { return (ImagePosition)GetEnumProperty("ImagePosition", fDefaultImagePosition); }
			set {
				SetEnumProperty("ImagePosition", fDefaultImagePosition, value);
				Changed();
			}
		}
		[AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool ShowDefaultText {
			get { return GetBoolProperty("ShowDefaultText", false); }
			set {
				SetBoolProperty("ShowDefaultText", false, value);
				Changed();
			}
		}
		[
		NotifyParentProperty(true), Localizable(true), AutoFormatEnable]
		public virtual string Text {
			get { return GetStringProperty("Text", fDefaultText); }
			set {
				SetStringProperty("Text", fDefaultText, value);
				Changed();
			}
		}
		[AutoFormatEnable, DefaultValue(DefaultTextFormatString), Localizable(true), NotifyParentProperty(true)]
		public virtual string TextFormatString {
			get { return GetStringProperty("TextFormatString", DefaultTextFormatString); }
			set {
				SetStringProperty("TextFormatString", DefaultTextFormatString, value);
				Changed();
			}
		}
		[
		NotifyParentProperty(true), AutoFormatEnable]
		public virtual bool Visible {
			get { return GetBoolProperty("Visible", fDefaultVisible); }
			set {
				if(Visible != value) {
					SetBoolProperty("Visible", fDefaultVisible, value);
					Changed();
				}
			}
		}
		protected internal PagerSettingsEx PagerSettings {
			get { return Owner as PagerSettingsEx; }
		}
		public PagerButtonProperties()
			: base() {
		}
		public PagerButtonProperties(IPropertiesOwner owner, ImagePosition defaultImagePosition, 
			string defaultText, bool defaultVisible)
			: base(owner) {
			fDefaultImagePosition = defaultImagePosition;
			fDefaultText = defaultText;
			fDefaultVisible = defaultVisible;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is PagerButtonProperties) {
					PagerButtonProperties src = source as PagerButtonProperties;
					Image.Assign(src.Image);
					ImagePosition = src.ImagePosition;
					Text = src.Text;
					Visible = src.Visible;
					TextFormatString = src.TextFormatString;
					ShowDefaultText = src.ShowDefaultText;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal string GetText() {
			bool showDefaultText = ShowDefaultText && string.IsNullOrEmpty(Text);
			return string.Format(TextFormatString, showDefaultText ? GetDefaultText() : Text);
		}
		protected internal virtual string GetDefaultText() {
			return fDefaultText;
		}
		protected internal virtual bool IsDisabled(int pageIndex, int pageCount) {
			return false;
		}
		protected internal virtual string GetButtonIDSuffix() {
			return "";
		}
		protected bool ShouldSerializeImagePosition() {
			return ImagePosition != fDefaultImagePosition;
		}
		protected bool ShouldSerializeText() {
			return ShowDefaultText && string.IsNullOrEmpty(Text) && Text != GetDefaultText() 
				|| !ShowDefaultText && Text != fDefaultText;
		}
		protected void ResetText() {
			Text = GetDefaultText();
		}
		protected bool ShouldSerializeVisible() {
			return Visible != fDefaultVisible;
		}
		protected void ResetVisible() {
			Visible = fDefaultVisible;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Image };
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class AllButtonProperties : PagerButtonProperties {
		public AllButtonProperties(IPropertiesOwner owner)
			: base(owner, ImagePosition.Left, "", false) {
		}
		public AllButtonProperties(IPropertiesOwner owner, bool visible)
			: base(owner, ImagePosition.Left, "", visible) {
		}
		public AllButtonProperties(IPropertiesOwner owner, bool visible, string text)
			: base(owner, ImagePosition.Left, text, visible) {
		}
		protected internal override string GetDefaultText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_All);
		}
		protected internal override bool IsDisabled(int pageIndex, int pageCount) {
			return (pageIndex == -1 || pageCount == 1);
		}
		protected internal override string GetButtonIDSuffix() {
			return "A";
		}
	}
	public class PrevButtonProperties : PagerButtonProperties {
		public PrevButtonProperties(IPropertiesOwner owner)
			: base(owner, ImagePosition.Left, "", true) {
		}
		public PrevButtonProperties(IPropertiesOwner owner, bool visible)
			: base(owner, ImagePosition.Left, "", visible) {
		}
		public PrevButtonProperties(IPropertiesOwner owner, bool visible, string text)
			: base(owner, ImagePosition.Left, text, visible) {
		}
		protected internal override string GetDefaultText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_Prev);
		}
		protected internal override bool IsDisabled(int pageIndex, int pageCount) {
			return (pageIndex == 0 || pageIndex == -1);
		}
		protected internal override string GetButtonIDSuffix() {
			return "P";
		}
	}
	public class NextButtonProperties : PagerButtonProperties {
		public NextButtonProperties(IPropertiesOwner owner)
			: base(owner, ImagePosition.Right, "", true) {
		}
		public NextButtonProperties(IPropertiesOwner owner, bool visible)
			: base(owner, ImagePosition.Right, "", visible) {
		}
		public NextButtonProperties(IPropertiesOwner owner, bool visible, string text)
			: base(owner, ImagePosition.Right, text, visible) {
		}
		protected internal override string GetDefaultText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_Next);
		}
		protected internal override bool IsDisabled(int pageIndex, int pageCount) {
			return (pageIndex >= pageCount - 1 || pageIndex == -1);
		}
		protected internal override string GetButtonIDSuffix() {
			return "N";
		}
	}
	public class FirstButtonProperties : PagerButtonProperties {
		public FirstButtonProperties(IPropertiesOwner owner)
			: base(owner, ImagePosition.Left, "", false) {
		}
		public FirstButtonProperties(IPropertiesOwner owner, bool visible)
			: base(owner, ImagePosition.Left, "", visible) {
		}
		public FirstButtonProperties(IPropertiesOwner owner, bool visible, string text)
			: base(owner, ImagePosition.Left, text, visible) {
		}
		protected internal override string GetDefaultText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_First);
		}
		protected internal override bool IsDisabled(int pageIndex, int pageCount) {
			return ((pageIndex == 0) || (pageIndex == -1 && PagerSettings.PageSizeItemSettings.Visible));
		}
		protected internal override string GetButtonIDSuffix() {
			return "F";
		}
	}
	public class LastButtonProperties : PagerButtonProperties {
		public LastButtonProperties(IPropertiesOwner owner)
			: base(owner, ImagePosition.Right, "", false) {
		}
		public LastButtonProperties(IPropertiesOwner owner, bool visible)
			: base(owner, ImagePosition.Right, "", visible) {
		}
		public LastButtonProperties(IPropertiesOwner owner, bool visible, string text)
			: base(owner, ImagePosition.Right, text, visible) {
		}
		protected internal override string GetDefaultText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_Last);
		}
		protected internal override bool IsDisabled(int pageIndex, int pageCount) {
			return ((pageIndex >= pageCount - 1) || (pageIndex == -1 && PagerSettings.PageSizeItemSettings.Visible));
		}
		protected internal override string GetButtonIDSuffix() {
			return "L";
		}
	}
	public class SummaryProperties : PagerButtonProperties {
		string fDefaultAllPagesText = ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_SummaryAllPagesFormat);
		string fDefaultEmptyText = ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_SummaryEmptyText);
		PagerButtonPosition fDefaultPosition = PagerButtonPosition.Left;
		[
#if !SL
	DevExpressWebLocalizedDescription("SummaryPropertiesAllPagesText"),
#endif
		NotifyParentProperty(true), Localizable(true), AutoFormatEnable]
		public virtual string AllPagesText {
			get { return GetStringProperty("AllPagesText", fDefaultAllPagesText); }
			set {
				SetStringProperty("AllPagesText", fDefaultAllPagesText, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SummaryPropertiesPosition"),
#endif
		NotifyParentProperty(true), AutoFormatEnable]
		public virtual PagerButtonPosition Position {
			get { return (PagerButtonPosition)GetEnumProperty("Position", fDefaultPosition); }
			set {
				SetEnumProperty("Position", fDefaultPosition, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SummaryPropertiesEmptyText"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, Localizable(true), Browsable(true)]
		public virtual string EmptyText {
			get {
				return GetStringProperty("EmptyText", fDefaultEmptyText);
			}
			set {
				SetStringProperty("EmptyText", fDefaultEmptyText, value);
				Changed();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagePropertiesEx Image {
			get { return base.Image; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagePosition ImagePosition {
			get { return base.ImagePosition; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowDefaultText {
			get { return base.ShowDefaultText; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TextFormatString {
			get { return base.TextFormatString; }
		}
		public SummaryProperties()
			: base() {
		}
		public SummaryProperties(IPropertiesOwner owner)
			: base(owner, ImagePosition.Left, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_SummaryFormat), true) {
		}
		public SummaryProperties(IPropertiesOwner owner, bool visible)
			: base(owner, ImagePosition.Left, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_SummaryFormat), visible) {
		}
		public SummaryProperties(IPropertiesOwner owner, PagerButtonPosition position, string text, string allPagesText)
			: base(owner, ImagePosition.Left, text, true) {
			fDefaultAllPagesText = allPagesText;
			fDefaultPosition = position;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is SummaryProperties) {
					SummaryProperties src = source as SummaryProperties;
					AllPagesText = src.AllPagesText;
					EmptyText = src.EmptyText;
					Position = src.Position;
				}
			} finally {
				EndUpdate();
			}
		}
		protected bool ShouldSerializeAllPagesText() {
			return AllPagesText != fDefaultAllPagesText;
		}
		protected bool ShouldSerializeEmptyText() {
			return EmptyText != fDefaultEmptyText;
		}
		protected bool ShouldSerializePosition() {
			return Position != fDefaultPosition;
		}
	}
	public class PageSizeItemSettings : PagerButtonProperties {
		private ButtonImageProperties dropDownImage = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsAllItemText"),
#endif
		NotifyParentProperty(true), Localizable(true), AutoFormatEnable]
		public string AllItemText
		{
			get { return GetStringProperty("AllItemText", GetDefaultAllItemText()); }
			set { SetStringProperty("AllItemText", GetDefaultAllItemText(), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsShowAllItem"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, DefaultValue(false)]
		public virtual bool ShowAllItem
		{
			get { return GetBoolProperty("ShowAllItem", false); }
			set
			{
				SetBoolProperty("ShowAllItem", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsItems"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, TypeConverter(typeof(StringListConverter))]
		public string[] Items
		{
			get { return (string[])GetObjectProperty("Items", GetDefaultPageSizeItems()); }
			set
			{
				if (!CommonUtils.AreEqualsArrays(value, Items))
				{
					CommonUtils.CheckNegativeOrZeroItems(value, "Items");
					CommonUtils.CheckDuplicateItems(value, "Items");
					SetObjectProperty("Items", GetDefaultPageSizeItems(), value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsCaption"),
#endif
		NotifyParentProperty(true), Localizable(true), AutoFormatEnable]
		public string Caption
		{
			get { return GetStringProperty("Caption", GetDefaultCaption()); }
			set { SetStringProperty("Caption", GetDefaultCaption(), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsDropDownImage"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonImageProperties DropDownImage
		{
			get
			{
				if (dropDownImage == null)
					dropDownImage = new ButtonImageProperties(this);
				return dropDownImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsPosition"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(PagerPageSizePosition.Right)]
		public PagerPageSizePosition Position
		{
			get { return (PagerPageSizePosition)GetEnumProperty("Position", PagerPageSizePosition.Right); }
			set
			{
				SetEnumProperty("Position", PagerPageSizePosition.Right, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PageSizeItemSettingsShowPopupShadow"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, DefaultValue(true)]
		public bool ShowPopupShadow
		{
			get { return GetBoolProperty("ShowPopupShadow", true); }
			set { SetBoolProperty("ShowPopupShadow", true, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagePropertiesEx Image {
			get { return base.Image; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImagePosition ImagePosition {
			get { return base.ImagePosition; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowDefaultText {
			get { return base.ShowDefaultText; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string TextFormatString {
			get { return base.TextFormatString; }
		}
		public PageSizeItemSettings()
			: base() {
		}
		public PageSizeItemSettings(IPropertiesOwner owner)
			: base(owner, ImagePosition.Left, "", false) {
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is PageSizeItemSettings) {
					PageSizeItemSettings src = source as PageSizeItemSettings;
					Caption = src.Caption;
					DropDownImage.Assign(src.DropDownImage);
					AllItemText = src.AllItemText;
					Position = src.Position;
					ShowAllItem = src.ShowAllItem;
					ShowPopupShadow = src.ShowPopupShadow;
					Items = src.Items;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual string GetDefaultCaption() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_PageSize);
		}
		protected internal virtual string GetDefaultAllItemText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_PageSizeAllItem);
		}
		protected virtual string[] GetDefaultPageSizeItems() {
			return new string[] { "10", "20", "50", "100", "200" };
		}
		protected internal override bool IsDisabled(int pageIndex, int pageCount) {
			return false;
		}
		protected bool ShouldSerializeCaption() {
			return Caption != GetDefaultCaption();
		}
		protected bool ShouldSerializeAllItemText() {
			return AllItemText != GetDefaultAllItemText();
		}
		protected bool ShouldSerializeItems() {
			return !CommonUtils.AreEqualsArrays(Items, GetDefaultPageSizeItems());
		}
		protected void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		protected void ResetAllItemText() {
			AllItemText = GetDefaultAllItemText();
		}
		protected void ResetItems() {
			Items = GetDefaultPageSizeItems();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { DropDownImage };
		}
	}
	[AutoFormatUrlPropertyClass]
	public class PagerSettingsEx : PropertiesBase, IPropertiesOwner {
		protected internal const string DefaultPageNumberFormat = "{0}";
		protected internal const string DefaultCurrentPageNumberFormat = "[{0}]";
		protected AllButtonProperties fAllButtonProperties = null;
		protected FirstButtonProperties fFirstPageButtonProperties = null;
		protected LastButtonProperties fLastPageButtonProperties = null;
		protected NextButtonProperties fNextPageButtonProperties = null;
		protected PrevButtonProperties fPrevPageButtonProperties = null;
		protected SummaryProperties fSummaryProperties = null;
		protected PageSizeItemSettings fPageSizeItemSettings = null;
		protected internal ASPxPagerBase Pager {
			get { return Owner as ASPxPagerBase; }
		}
		public PagerSettingsEx()
			: base() {
		}
		public PagerSettingsEx(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExCurrentPageNumberFormat"),
#endif
		DefaultValue(DefaultCurrentPageNumberFormat), NotifyParentProperty(true), AutoFormatDisable,
		Localizable(true)]
		public virtual string CurrentPageNumberFormat {
			get { return GetStringProperty("CurrentPageNumberFormat", DefaultCurrentPageNumberFormat); }
			set { SetStringProperty("CurrentPageNumberFormat", DefaultCurrentPageNumberFormat, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExEllipsisMode"),
#endif
		DefaultValue(PagerEllipsisMode.InsideNumeric), NotifyParentProperty(true), AutoFormatDisable]
		public virtual PagerEllipsisMode EllipsisMode {
			get { return (PagerEllipsisMode)GetEnumProperty("EllipsisMode", PagerEllipsisMode.InsideNumeric); }
			set {
				SetEnumProperty("EllipsisMode", PagerEllipsisMode.InsideNumeric, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExEnableAdaptivity"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool EnableAdaptivity {
			get { return GetBoolProperty("EnableAdaptivity", false); }
			set { SetBoolProperty("EnableAdaptivity", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExNumericButtonCount"),
#endif
		DefaultValue(10), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int NumericButtonCount {
			get { return GetIntProperty("NumericButtonCount", 10); }
			set {
				if(NumericButtonCount != value) {
					CommonUtils.CheckNegativeOrZeroValue(value, "NumericButtonCount");
					SetIntProperty("NumericButtonCount", 10, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExPageNumberFormat"),
#endif
		DefaultValue(DefaultPageNumberFormat), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public virtual string PageNumberFormat {
			get { return GetStringProperty("PageNumberFormat", DefaultPageNumberFormat); }
			set { SetStringProperty("PageNumberFormat", DefaultPageNumberFormat, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExPosition"),
#endif
		DefaultValue(PagerPosition.TopAndBottom), NotifyParentProperty(true), AutoFormatDisable]
		public virtual PagerPosition Position {
			get { return (PagerPosition)GetEnumProperty("Position", PagerPosition.TopAndBottom); }
			set {
				SetEnumProperty("Position", PagerPosition.TopAndBottom, value);
				Changed();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable,
		DefaultValue(SEOFriendlyMode.Disabled), NotifyParentProperty(true)]
		public virtual SEOFriendlyMode SEOFriendly {
			get { return (SEOFriendlyMode)GetEnumProperty("SEOFriendly", SEOFriendlyMode.Disabled); }
			set {
				SetEnumProperty("SEOFriendly", SEOFriendlyMode.Disabled, value);
				Changed();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable,
		DefaultValue(""), NotifyParentProperty(true)]
		public virtual string SeoNavigateUrlFormatString {
			get { return GetStringProperty("SeoNavigateUrlFormatString", ""); }
			set { SetStringProperty("SeoNavigateUrlFormatString", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExShowNumericButtons"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool ShowNumericButtons {
			get { return GetBoolProperty("ShowNumericButtons", true); }
			set {
				SetBoolProperty("ShowNumericButtons", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExShowDefaultImages"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatEnable]
		public virtual bool ShowDefaultImages {
			get { return GetBoolProperty("ShowDefaultImages", true); }
			set {
				SetBoolProperty("ShowDefaultImages", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExShowDisabledButtons"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public virtual bool ShowDisabledButtons {
			get { return GetBoolProperty("ShowDisabledButtons", true); }
			set {
				SetBoolProperty("ShowDisabledButtons", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExShowSeparators"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public virtual bool ShowSeparators {
			get { return GetBoolProperty("ShowSeparators", false); }
			set {
				SetBoolProperty("ShowSeparators", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExRenderMode"),
#endif
		Obsolete("This property is now obsolete. The Lightweight render mode is used."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(ControlRenderMode.Lightweight), NotifyParentProperty(true), AutoFormatDisable]
		public virtual ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExAllButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AllButtonProperties AllButton {
			get {
				if(fAllButtonProperties == null)
					fAllButtonProperties = CreateAllButtonProperties(this);
				return fAllButtonProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExFirstPageButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FirstButtonProperties FirstPageButton {
			get {
				if(fFirstPageButtonProperties == null)
					fFirstPageButtonProperties = CreateFirstButtonProperties(this);
				return fFirstPageButtonProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExLastPageButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LastButtonProperties LastPageButton {
			get {
				if(fLastPageButtonProperties == null)
					fLastPageButtonProperties = CreateLastButtonProperties(this);
				return fLastPageButtonProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExNextPageButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NextButtonProperties NextPageButton {
			get {
				if(fNextPageButtonProperties == null)
					fNextPageButtonProperties = CreateNextButtonProperties(this);
				return fNextPageButtonProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExPrevPageButton"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrevButtonProperties PrevPageButton {
			get {
				if(fPrevPageButtonProperties == null)
					fPrevPageButtonProperties = CreatePrevButtonProperties(this);
				return fPrevPageButtonProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExSummary"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true)]
		public SummaryProperties Summary {
			get {
				if(fSummaryProperties == null)
					fSummaryProperties = CreateSummaryProperties(this);
				return fSummaryProperties;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PagerSettingsExPageSizeItemSettings"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(true)]
		public PageSizeItemSettings PageSizeItemSettings
		{
			get
			{
				if (fPageSizeItemSettings == null)
					fPageSizeItemSettings = CreatePageSizeItemSettings(this);
				return fPageSizeItemSettings;
			}
		}
		protected void RecreateOwnerStyle() {
			ASPxWebControl control = Owner as ASPxWebControl;
			if (control != null)
				control.RecreateStyles();
		}
		protected internal bool IsSEOEnabled {
			get {
				ASPxWebControl control = Owner as ASPxWebControl;
				if(control != null) {
					if(SEOFriendly == SEOFriendlyMode.CrawlerOnly)
						return !control.DesignMode && (
							control.Request != null && control.Request.Browser.Crawler ||
							!string.IsNullOrEmpty(SeoNavigateUrlFormatString)
						);
					else
						return SEOFriendly == SEOFriendlyMode.Enabled || 
							!string.IsNullOrEmpty(SeoNavigateUrlFormatString);
				}
				return false;
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is PagerSettingsEx) {
					PagerSettingsEx src = source as PagerSettingsEx;
					AllButton.Assign(src.AllButton);
					CurrentPageNumberFormat = src.CurrentPageNumberFormat;
					EllipsisMode = src.EllipsisMode;
					EnableAdaptivity = src.EnableAdaptivity;
					FirstPageButton.Assign(src.FirstPageButton);
					LastPageButton.Assign(src.LastPageButton);
					NextPageButton.Assign(src.NextPageButton);
					NumericButtonCount = src.NumericButtonCount;
					Position = src.Position;
					PrevPageButton.Assign(src.PrevPageButton);
					PageNumberFormat = src.PageNumberFormat;
					PageSizeItemSettings.Assign(src.PageSizeItemSettings);
					SEOFriendly = src.SEOFriendly;
					SeoNavigateUrlFormatString = src.SeoNavigateUrlFormatString;
					ShowNumericButtons = src.ShowNumericButtons;
					ShowDefaultImages = src.ShowDefaultImages;
					ShowDisabledButtons = src.ShowDisabledButtons;
					ShowSeparators = src.ShowSeparators;
					Summary.Assign(src.Summary);
					Visible = src.Visible;
				}
			} finally {
				EndUpdate();
			}
		}
		protected virtual AllButtonProperties CreateAllButtonProperties(IPropertiesOwner owner) {
			return new AllButtonProperties(owner);
		}
		protected virtual FirstButtonProperties CreateFirstButtonProperties(IPropertiesOwner owner) {
			return new FirstButtonProperties(owner);
		}
		protected virtual LastButtonProperties CreateLastButtonProperties(IPropertiesOwner owner) {
			return new LastButtonProperties(owner);
		}
		protected virtual NextButtonProperties CreateNextButtonProperties(IPropertiesOwner owner) {
			return new NextButtonProperties(owner);
		}
		protected virtual PrevButtonProperties CreatePrevButtonProperties(IPropertiesOwner owner) {
			return new PrevButtonProperties(owner);
		}
		protected virtual SummaryProperties CreateSummaryProperties(IPropertiesOwner owner) {
			return new SummaryProperties(owner);
		}
		protected virtual PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new PageSizeItemSettings(owner);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { PageSizeItemSettings, AllButton, FirstPageButton, LastPageButton,
				NextPageButton, PrevPageButton, Summary };
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			if(properties == FirstPageButton)
				LastPageButton.Visible = FirstPageButton.Visible;
			if(properties == LastPageButton)
				FirstPageButton.Visible = LastPageButton.Visible;
			if(properties == NextPageButton)
				PrevPageButton.Visible = NextPageButton.Visible;
			if(properties == PrevPageButton)
				NextPageButton.Visible = PrevPageButton.Visible;
			Changed();
		}
	}
}
