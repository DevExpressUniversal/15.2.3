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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LayoutItemHelpTextSettings : PropertiesBase {
		const HelpTextPosition DefaultPosition = HelpTextPosition.Bottom;
		LayoutItemBase item = null;
		public LayoutItemHelpTextSettings(LayoutItemBase item)
			: base(item) {
			this.item = item;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemHelpTextSettingsPosition"),
#endif
		DefaultValue(HelpTextPosition.Auto), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextPosition Position {
			get { return (HelpTextPosition)GetEnumProperty("Position", HelpTextPosition.Auto); }
			set { 
				SetEnumProperty("Position", HelpTextPosition.Auto, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemHelpTextSettingsHorizontalAlign"),
#endif
		DefaultValue(HelpTextHorizontalAlign.Auto), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextHorizontalAlign HorizontalAlign {
			get { return (HelpTextHorizontalAlign)GetEnumProperty("HorizontalAlign", HelpTextHorizontalAlign.Auto); }
			set { SetEnumProperty("HorizontalAlign", HelpTextHorizontalAlign.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemHelpTextSettingsVerticalAlign"),
#endif
		DefaultValue(HelpTextVerticalAlign.Auto), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextVerticalAlign VerticalAlign {
			get { return (HelpTextVerticalAlign)GetEnumProperty("VerticalAlign", HelpTextVerticalAlign.Auto); }
			set { SetEnumProperty("VerticalAlign", HelpTextVerticalAlign.Auto, value); }
		}
		internal HelpTextPosition GetPosition() {
			if (Position != HelpTextPosition.Auto)
				return Position;
			if(this.item.ParentGroupInternal != null) {
				HelpTextPosition parentPosition = this.item.ParentGroupInternal.SettingsItemHelpTexts.Position;
				if (parentPosition != HelpTextPosition.Auto)
					return parentPosition;
			}
			if(this.item.Owner != null) {
				HelpTextPosition ownerPosition = this.item.Owner.SettingsItemHelpTexts.Position;
				if (ownerPosition != HelpTextPosition.Auto)
					return ownerPosition;
			}
			return DefaultPosition;
		}
		internal HelpTextHorizontalAlign GetHorizontalAlign() {
			if (HorizontalAlign != HelpTextHorizontalAlign.Auto)
				return HorizontalAlign;
			if(this.item.ParentGroupInternal != null) {
				HelpTextHorizontalAlign parentHorizontalAlign = this.item.ParentGroupInternal.SettingsItemHelpTexts.HorizontalAlign;
				if (parentHorizontalAlign != HelpTextHorizontalAlign.Auto)
					return parentHorizontalAlign;
			}
			if(this.item.Owner != null) {
				HelpTextHorizontalAlign ownerHorizontalAlign = this.item.Owner.SettingsItemHelpTexts.HorizontalAlign;
				if (ownerHorizontalAlign != HelpTextHorizontalAlign.Auto)
					return ownerHorizontalAlign;
			}
			return GetDefaultHorizontalAlign();
		}
		internal HelpTextHorizontalAlign GetDefaultHorizontalAlign() {
			bool rtl = (this.item.Owner as ISkinOwner).IsRightToLeft();
			if (GetPosition() == HelpTextPosition.Left)
				return rtl ? HelpTextHorizontalAlign.Left : HelpTextHorizontalAlign.Right;
			else
				return rtl ? HelpTextHorizontalAlign.Right : HelpTextHorizontalAlign.Left;
		}
		internal HelpTextVerticalAlign GetVerticalAlign() {
			if (VerticalAlign != HelpTextVerticalAlign.Auto)
				return VerticalAlign;
			if(this.item.ParentGroupInternal != null) {
				HelpTextVerticalAlign parentVerticalAlign = this.item.ParentGroupInternal.SettingsItemHelpTexts.VerticalAlign;
				if (parentVerticalAlign != HelpTextVerticalAlign.Auto)
					return parentVerticalAlign;
			}
			if(this.item.Owner != null) {
				HelpTextVerticalAlign ownerVerticalAlign = this.item.Owner.SettingsItemHelpTexts.VerticalAlign;
				if (ownerVerticalAlign != HelpTextVerticalAlign.Auto)
					return ownerVerticalAlign;
			}
			return GetDefaultVerticalAlign();
		}
		internal HelpTextVerticalAlign GetDefaultVerticalAlign() {
			return GetPosition() == HelpTextPosition.Top ? HelpTextVerticalAlign.Bottom : HelpTextVerticalAlign.Top;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				LayoutItemHelpTextSettings src = source as LayoutItemHelpTextSettings;
				if (src != null) {
					HorizontalAlign = src.HorizontalAlign;
					Position = src.Position;
					VerticalAlign = src.VerticalAlign;
				}
			} finally {
				EndUpdate();
			}
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TabbedLayoutGroupTabPageImage : PropertiesBase {
		ASPxPageControl pageControl = null;
		public TabbedLayoutGroupTabPageImage(ASPxPageControl pageControl) {
			this.pageControl = pageControl;
		}
		protected ASPxPageControl PageControl{ get { return this.pageControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageImageActiveTabImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties ActiveTabImage {
			get { return PageControl.ActiveTabImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageImageScrollLeftButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ScrollLeftButtonImage {
			get { return PageControl.ScrollLeftButtonImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageImageScrollRightButtonImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonImageProperties ScrollRightButtonImage {
			get { return PageControl.ScrollRightButtonImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageImageTabImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabImageProperties TabImage
		{
			get { return PageControl.TabImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageImageLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties LoadingPanelImage
		{
			get { return PageControl.LoadingPanelImage; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TabbedLayoutGroupTabPageImage tabbedLayoutGroupTabPageImage = source as TabbedLayoutGroupTabPageImage;
			if(tabbedLayoutGroupTabPageImage != null) {
				ActiveTabImage.Assign(tabbedLayoutGroupTabPageImage.ActiveTabImage);
				LoadingPanelImage.Assign(tabbedLayoutGroupTabPageImage.LoadingPanelImage);
				ScrollLeftButtonImage.Assign(tabbedLayoutGroupTabPageImage.ScrollLeftButtonImage);
				ScrollRightButtonImage.Assign(tabbedLayoutGroupTabPageImage.ScrollRightButtonImage);
				TabImage.Assign(tabbedLayoutGroupTabPageImage.TabImage);
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TabbedLayoutGroupTabPageStyles : PropertiesBase {
		ASPxPageControl pageControl = null;
		public TabbedLayoutGroupTabPageStyles(ASPxPageControl pageControl) {
			this.pageControl = pageControl;
		}
		protected ASPxPageControl PageControl{ get { return this.pageControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesActiveTabStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle ActiveTabStyle {
			get { return PageControl.ActiveTabStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesContentStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContentStyle ContentStyle {
			get { return PageControl.ContentStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesScrollButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonStyle ScrollButtonStyle {
			get { return PageControl.ScrollButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LoadingPanelStyle LoadingPanelStyle {
			get { return PageControl.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesTabStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyle TabStyle {
			get { return PageControl.TabStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesSpaceBeforeTabsTemplateStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpaceTabTemplateStyle SpaceBeforeTabsTemplateStyle {
			get { return PageControl.SpaceBeforeTabsTemplateStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesSpaceAfterTabsTemplateStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpaceTabTemplateStyle SpaceAfterTabsTemplateStyle {
			get { return PageControl.SpaceAfterTabsTemplateStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageStylesDisabledStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DisabledStyle DisabledStyle {
			get { return PageControl.DisabledStyle; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TabbedLayoutGroupTabPageStyles tabbedLayoutGroupTabPageStyles = source as TabbedLayoutGroupTabPageStyles;
			if(tabbedLayoutGroupTabPageStyles != null) {
				ActiveTabStyle.Assign(tabbedLayoutGroupTabPageStyles.ActiveTabStyle);
				ContentStyle.Assign(tabbedLayoutGroupTabPageStyles.ContentStyle);
				DisabledStyle.Assign(tabbedLayoutGroupTabPageStyles.DisabledStyle);
				LoadingPanelStyle.Assign(tabbedLayoutGroupTabPageStyles.LoadingPanelStyle);
				ScrollButtonStyle.Assign(tabbedLayoutGroupTabPageStyles.ScrollButtonStyle);
				SpaceAfterTabsTemplateStyle.Assign(tabbedLayoutGroupTabPageStyles.SpaceAfterTabsTemplateStyle);
				SpaceBeforeTabsTemplateStyle.Assign(tabbedLayoutGroupTabPageStyles.SpaceBeforeTabsTemplateStyle);
				TabStyle.Assign(tabbedLayoutGroupTabPageStyles.TabStyle);
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TabbedLayoutGroupTabPageSettings : PropertiesBase {
		ASPxPageControl pageControl = null;
		public TabbedLayoutGroupTabPageSettings(ASPxPageControl pageControl) {
			this.pageControl = pageControl;
		}
		protected ASPxPageControl PageControl{ get { return this.pageControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsActivateTabPageAction"),
#endif
		Category("Behavior"), DefaultValue(ActivateTabPageAction.Click), AutoFormatDisable]
		public ActivateTabPageAction ActivateTabPageAction {
			get { return PageControl.ActivateTabPageAction; }
			set { PageControl.ActivateTabPageAction = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsAutoPostBack"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoPostBack {
			get { return PageControl.AutoPostBack; }
			set { PageControl.AutoPostBack = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return PageControl.EnableCallbackCompression; }
			set { PageControl.EnableCallbackCompression = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsEnableCallBacks"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return PageControl.EnableCallBacks; }
			set { PageControl.EnableCallBacks = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return PageControl.EnableClientSideAPI; }
			set { PageControl.EnableClientSideAPI = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsEnableHierarchyRecreation"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableHierarchyRecreation {
			get { return PageControl.EnableHierarchyRecreation; }
			set { PageControl.EnableHierarchyRecreation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsEnableHotTrack"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool EnableHotTrack {
			get { return PageControl.EnableHotTrack; }
			set { PageControl.EnableHotTrack = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsEnableTabScrolling"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool EnableTabScrolling {
			get { return PageControl.EnableTabScrolling; }
			set { PageControl.EnableTabScrolling = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return PageControl.JSProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsRenderMode"),
#endif
 Category("Layout"),
		Obsolete("This property is now obsolete. The Lightweight render mode is used."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		AutoFormatDisable, DefaultValue(ControlRenderMode.Lightweight)]
		public ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsScrollButtonsIndent"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ScrollButtonsIndent {
			get { return PageControl.ScrollButtonsIndent; }
			set { PageControl.ScrollButtonsIndent = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TabbedLayoutGroupTabPageSettingsScrollButtonSpacing"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ScrollButtonSpacing {
			get { return PageControl.ScrollButtonSpacing; }
			set { PageControl.ScrollButtonSpacing = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public ITemplate ActiveTabTemplate {
			get { return PageControl.ActiveTabTemplate; }
			set { PageControl.ActiveTabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public ITemplate TabTemplate {
			get { return PageControl.TabTemplate; }
			set { PageControl.TabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate SpaceAfterTabsTemplate {
			get { return PageControl.SpaceAfterTabsTemplate; }
			set { PageControl.SpaceAfterTabsTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(TemplateContainerBase)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate SpaceBeforeTabsTemplate {
			get { return PageControl.SpaceBeforeTabsTemplate; }
			set { PageControl.SpaceBeforeTabsTemplate = value; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			TabbedLayoutGroupTabPageSettings SettingsTabPages = source as TabbedLayoutGroupTabPageSettings;
			if(SettingsTabPages != null) {
				ActivateTabPageAction = SettingsTabPages.ActivateTabPageAction;
				AutoPostBack = SettingsTabPages.AutoPostBack;
				EnableCallbackCompression = SettingsTabPages.EnableCallbackCompression;
				EnableCallBacks = SettingsTabPages.EnableCallBacks;
				EnableClientSideAPI = SettingsTabPages.EnableClientSideAPI;
				EnableHierarchyRecreation = SettingsTabPages.EnableHierarchyRecreation;
				EnableHotTrack = SettingsTabPages.EnableHotTrack;
				EnableTabScrolling = SettingsTabPages.EnableTabScrolling;
				ScrollButtonsIndent = SettingsTabPages.ScrollButtonsIndent;
				ScrollButtonSpacing = SettingsTabPages.ScrollButtonSpacing;
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LayoutItemCaptionSettings : PropertiesBase {
		const LayoutItemCaptionLocation DefaultLocation = LayoutItemCaptionLocation.Left;
		const FormLayoutHorizontalAlign DefaultHorizontalAlign = FormLayoutHorizontalAlign.Left,
			DefaultRTLHorizontalAlign = FormLayoutHorizontalAlign.Right;
		const FormLayoutVerticalAlign DefaultVerticalAlign = FormLayoutVerticalAlign.Top;
		LayoutItemBase item = null;
		public LayoutItemCaptionSettings(LayoutItemBase item)
			: base(item) {
			this.item = item;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaptionSettingsLocation"),
#endif
		DefaultValue(LayoutItemCaptionLocation.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public LayoutItemCaptionLocation Location {
			get { return (LayoutItemCaptionLocation)GetEnumProperty("Location", LayoutItemCaptionLocation.NotSet); }
			set {
				if (Location != value) {
					SetEnumProperty("Location", LayoutItemCaptionLocation.NotSet, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaptionSettingsHorizontalAlign"),
#endif
		DefaultValue(FormLayoutHorizontalAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public FormLayoutHorizontalAlign HorizontalAlign {
			get { return (FormLayoutHorizontalAlign)GetEnumProperty("HorizontalAlign", FormLayoutHorizontalAlign.NotSet); }
			set { SetEnumProperty("HorizontalAlign", FormLayoutHorizontalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutItemCaptionSettingsVerticalAlign"),
#endif
		DefaultValue(FormLayoutVerticalAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public FormLayoutVerticalAlign VerticalAlign {
			get { return (FormLayoutVerticalAlign)GetEnumProperty("VerticalAlign", FormLayoutVerticalAlign.NotSet); }
			set { SetEnumProperty("VerticalAlign", FormLayoutVerticalAlign.NotSet, value); }
		}
		protected FormLayoutHorizontalAlign GetDefaultHorizontalAlign() {
			return item.Owner != null && (this.item.Owner as ISkinOwner).IsRightToLeft() ? DefaultRTLHorizontalAlign : DefaultHorizontalAlign;
		}
		internal FormLayoutHorizontalAlign GetHorizontalAlign() {
#pragma warning disable 618
			if (HorizontalAlign != FormLayoutHorizontalAlign.NoSet && HorizontalAlign != FormLayoutHorizontalAlign.NotSet)
				return HorizontalAlign;
			if(item.ParentGroupInternal != null) {
				FormLayoutHorizontalAlign parentHorizontalAlign = item.ParentGroupInternal.SettingsItemCaptions.HorizontalAlign;
				if (parentHorizontalAlign != FormLayoutHorizontalAlign.NoSet && parentHorizontalAlign != FormLayoutHorizontalAlign.NotSet)
					return parentHorizontalAlign;
			}
			if(item.ParentGroupInternal != null) {
				FormLayoutHorizontalAlign ownerHorizontalAlign = item.Owner.SettingsItemCaptions.HorizontalAlign;
				if (ownerHorizontalAlign != FormLayoutHorizontalAlign.NoSet && ownerHorizontalAlign != FormLayoutHorizontalAlign.NotSet)
					return ownerHorizontalAlign;
			}
			return GetDefaultHorizontalAlign();
#pragma warning restore 618
		}
		internal FormLayoutVerticalAlign GetVerticalAlign() {
#pragma warning disable 618
			if (VerticalAlign != FormLayoutVerticalAlign.NoSet && VerticalAlign != FormLayoutVerticalAlign.NotSet)
				return VerticalAlign;
			if(item.ParentGroupInternal != null) {
				FormLayoutVerticalAlign parentVerticalAlign = item.ParentGroupInternal.SettingsItemCaptions.VerticalAlign;
				if (parentVerticalAlign != FormLayoutVerticalAlign.NoSet && parentVerticalAlign != FormLayoutVerticalAlign.NotSet)
					return parentVerticalAlign;
			}
			if(item.ParentGroupInternal != null) {
				FormLayoutVerticalAlign ownerVerticalAlign = item.Owner.SettingsItemCaptions.VerticalAlign;
				if (ownerVerticalAlign != FormLayoutVerticalAlign.NoSet && ownerVerticalAlign != FormLayoutVerticalAlign.NotSet)
					return ownerVerticalAlign;
			}
			return DefaultVerticalAlign;
#pragma warning restore 618
		}
		internal LayoutItemCaptionLocation GetLocation() {
#pragma warning disable 618
			if (Location != LayoutItemCaptionLocation.NoSet && Location != LayoutItemCaptionLocation.NotSet)
				return Location;
			if(item.ParentGroupInternal != null) {
				LayoutItemCaptionLocation parentLocation = item.ParentGroupInternal.SettingsItemCaptions.Location;
				if (parentLocation != LayoutItemCaptionLocation.NoSet && parentLocation != LayoutItemCaptionLocation.NotSet)
					return parentLocation;
			}
			if(item.ParentGroupInternal != null) {
				LayoutItemCaptionLocation ownerLocation = item.Owner.SettingsItemCaptions.Location;
				if (ownerLocation != LayoutItemCaptionLocation.NoSet && ownerLocation != LayoutItemCaptionLocation.NotSet)
					return ownerLocation;
			}
			return DefaultLocation;
#pragma warning restore 618
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				LayoutItemCaptionSettings src = source as LayoutItemCaptionSettings;
				if (src != null) {
					Location = src.Location;
					VerticalAlign = src.VerticalAlign;
					HorizontalAlign = src.HorizontalAlign;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LayoutGroupItemSettings : PropertiesBase {
		LayoutItemBase item = null;
		public LayoutGroupItemSettings(LayoutItemBase item)
			: base(item) {
			this.item = item;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupItemSettingsWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit Width {
			get { return (Unit)GetObjectProperty("Width", Unit.Empty); }
			set { SetObjectProperty("Width", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupItemSettingsHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit Height {
			get { return (Unit)GetObjectProperty("Height", Unit.Empty); }
			set { SetObjectProperty("Height", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupItemSettingsShowCaption"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatEnable]
		public DefaultBoolean ShowCaption {
			get { return (DefaultBoolean)GetObjectProperty("ShowCaption", DefaultBoolean.Default); }
			set {
				SetObjectProperty("ShowCaption", DefaultBoolean.Default, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupItemSettingsVerticalAlign"),
#endif
		DefaultValue(FormLayoutVerticalAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public FormLayoutVerticalAlign VerticalAlign {
			get { return (FormLayoutVerticalAlign)GetEnumProperty("VerticalAlign", FormLayoutVerticalAlign.NotSet); }
			set { SetEnumProperty("VerticalAlign", FormLayoutVerticalAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("LayoutGroupItemSettingsHorizontalAlign"),
#endif
		DefaultValue(FormLayoutHorizontalAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public FormLayoutHorizontalAlign HorizontalAlign {
			get { return (FormLayoutHorizontalAlign)GetEnumProperty("HorizontalAlign", FormLayoutHorizontalAlign.NotSet); }
			set { SetEnumProperty("HorizontalAlign", FormLayoutHorizontalAlign.NotSet, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				LayoutGroupItemSettings src = source as LayoutGroupItemSettings;
				if (src != null) {
					Width = src.Width;
					Height = src.Height;
					ShowCaption = src.ShowCaption;
					VerticalAlign = src.VerticalAlign;
					HorizontalAlign = src.HorizontalAlign;
				}
			} finally {
				EndUpdate();
			}
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	public enum FormLayoutAdaptivityMode { Off, SingleColumnWindowLimit }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FormLayoutAdaptivitySettings : PropertiesBase {
		public FormLayoutAdaptivitySettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutAdaptivitySettingsAdaptivityMode"),
#endif
		DefaultValue(FormLayoutAdaptivityMode.Off), NotifyParentProperty(true), AutoFormatEnable]
		public FormLayoutAdaptivityMode AdaptivityMode {
			get { return (FormLayoutAdaptivityMode)GetEnumProperty("AdaptivityMode", FormLayoutAdaptivityMode.Off); }
			set {
				SetEnumProperty("AdaptivityMode", FormLayoutAdaptivityMode.Off, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutAdaptivitySettingsSwitchToSingleColumnAtWindowInnerWidth"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatEnable]
		public int SwitchToSingleColumnAtWindowInnerWidth {
			get { return GetIntProperty("SwitchToSingleColumnAtWindowInnerWidth", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "SwitchToSingleColumnAtWindowInnerWidth");
				SetIntProperty("SwitchToSingleColumnAtWindowInnerWidth", 0, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FormLayoutAdaptivitySettings src = source as FormLayoutAdaptivitySettings;
			if(src != null) {
				AdaptivityMode = src.AdaptivityMode;
				SwitchToSingleColumnAtWindowInnerWidth = src.SwitchToSingleColumnAtWindowInnerWidth;
			}
		}
	}
}
