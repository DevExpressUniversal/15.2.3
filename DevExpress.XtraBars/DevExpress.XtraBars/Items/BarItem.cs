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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Editors;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Text.Internal;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraBars {
	[
	DXToolboxItem(false), DesignTimeVisible(false), DefaultEvent("ItemClick"),
	Designer("DevExpress.XtraBars.Design.BarItemDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner)),
	SmartTagSupport(typeof(BarItemDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto), SmartTagFilter(typeof(BarItemSmartTagFilter))
	]
	public class BarItem : Component, IXtraSerializable, ICustomTypeDescriptor, IAppearanceOwner, IBindableComponent, IDXImageUriClient, ISupportAdornerElementBarItem {
 		RibbonItemStyles ribbonStyle;
		BarManager manager, disposedManager;
		Guid categoryGuid;
		BarMenuMerge mergeType;
		int mergeOrder;
		BarItemLinkCollection links;
		BarItemPaintStyle paintStyle;
		int imageIndex, id, imageIndexDisabled, largeImageIndex, largeImageIndexDisabled;
		internal int categoryIndex;
		object tag;
		string name, accessibleName, accessibleDescription;
		Image glyph, glyphDisabled, largeGlyph, largeGlyphDisabled;
		DxImageUri imageUri;
		Shortcut shortCut;
		BarShortcut itemShortcut;
		string caption, description, hint;
		bool enabled;
		BarItemVisibility visibility;
		int lockUpdate;
		protected internal bool fIsPrivateItem;
		private bool showInCustomizationForm = true;
		protected BarItemBorderStyle fBorderStyle;
		BorderStyles border;
		BarItemLinkAlignment alignment;
		SuperToolTip superTip, dropDownSuperTip;
		int largeWidth = 0, smallWithTextWidth = 0, smallWithoutTextWidth = 0;
		BarItemEventFireMode itemClickFireMode = BarItemEventFireMode.Default;
		bool causesValidation = false;
		bool allowRightClickInMenu = true;
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		DefaultBoolean effectiveGlyphSkinning;
		BarItemAppearances appearance, appearanceInMenu;
		private static object itemClick = new object();
		private static object itemPress = new object();
		private static object itemDoubleClick = new object();
		private static object hyperlinkClick = new object();
		internal static bool SkipDisposeLinkOnRemove { get; set; }
		protected BarItem() {
			this.ribbonStyle = RibbonItemStyles.Default;
			this.accessibleDescription = this.accessibleName = null;
			this.border = DefaultBorder;
			this.mergeType = BarMenuMerge.Add;
			this.itemShortcut = BarShortcut.Empty;
			this.name = "";
			this.disposedManager = this.manager = null;
			this.fBorderStyle = BarItemBorderStyle.None;
			this.hint = description = caption = "";
			this.tag = null;
			this.id = -1;
			this.fIsPrivateItem = false;
			this.categoryIndex = 0;
			this.categoryGuid = BarManagerCategory.DefaultCategory.Guid;
			this.glyphDisabled = this.glyph = null;
			this.paintStyle = BarItemPaintStyle.Standard;
			this.links = new BarItemLinkCollection();
			this.links.CollectionChanged += new CollectionChangeEventHandler(OnItemLinkCollectionChanged);
			this.shortCut = Shortcut.None;
			this.enabled = true;
			this.visibility = BarItemVisibility.Always;
			this.alignment = BarItemLinkAlignment.Default;
			this.imageIndexDisabled = this.imageIndex = -1;
			this.largeImageIndex = this.largeImageIndexDisabled = -1;
			this.largeGlyph = this.largeGlyphDisabled = null;
			this.effectiveGlyphSkinning = AllowGlyphSkinning;
			this.imageUri = CreateImageUriInstance();
			this.imageUri.Changed += OnImageUriChanged;
			this.lockUpdate = 0;
			this.mergeOrder = 0;
			this.appearance = new BarItemAppearances(this);
			this.appearance.Changed += new EventHandler(OnOwnAppearanceChanged);
			this.appearanceInMenu = new BarItemAppearances(this);
			this.appearanceInMenu.Changed += new EventHandler(OnOwnAppearanceChanged);
			this.allowHtmlText = DefaultBoolean.Default;
		}
		internal BarItem(BarManager barManager) : this() {
			if(barManager == null) return;
			this.Manager = barManager;
		}
		ControlBindingsCollection dataBindingsCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemDataBindings")]
#endif
		[ParenthesizePropertyName(true), RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
		public ControlBindingsCollection DataBindings {
			get {
				if(dataBindingsCore == null)
					dataBindingsCore = CreateDataBindings();
				return dataBindingsCore;
			}
		}
		BindingContext bindingContextCore;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingContext BindingContext {
			get {
				if(bindingContextCore != null)
					return bindingContextCore;
				IBindableComponent parentComponent = GetParentBindableComponent(Manager);
				return (parentComponent != null) ? parentComponent.BindingContext : null;
			}
			set {
				if(bindingContextCore == value) return;
				this.bindingContextCore = value;
				OnBindingContextChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected internal virtual void OnBindingContextChanged() {
			this.UpdateBindingsCore(() => this.BindingContext);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void UpdateBindings(BindingContext context) {
			UpdateBindingsCore(() => context);
		}
		void UpdateBindingsCore(Func<BindingContext> getContext) {
			if(dataBindingsCore == null) return;
			BindingContext context = null;
			for(int i = 0; i < DataBindings.Count; i++)
				BindingContext.UpdateBinding(context ?? (context = getContext()), this.DataBindings[i]);
		}
		IBindableComponent GetParentBindableComponent(BarManager manager) {
			return (manager != null) ? manager.GetParentBindableComponent() : null;
		}
		protected virtual ControlBindingsCollection CreateDataBindings() {
			return new ControlBindingsCollection(this);
		}
		protected virtual DxImageUri CreateImageUriInstance() {
			return new DxImageUri();
		}
		[Browsable(false)]
		public virtual bool IsImageExist {
			get {
				return Glyph != null || (ImageUri != null && ImageUri.HasImage) || (ImageCollection.IsImageListImageExists(Images, ImageIndex));
			}
		}
		[Browsable(false)]
		public virtual bool IsLargeImageExist {
			get {
				return LargeGlyph != null || (ImageUri != null && ImageUri.HasLargeImage) || (ImageCollection.IsImageListImageExists(LargeImages, LargeImageIndex));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAllowRightClickInMenu"),
#endif
 DefaultValue(true)]
		public bool AllowRightClickInMenu {
			get { return allowRightClickInMenu; }
			set { allowRightClickInMenu = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemClickFireMode"),
#endif
DefaultValue(BarItemEventFireMode.Default)]
		public BarItemEventFireMode ItemClickFireMode {
			get { return itemClickFireMode; }
			set { itemClickFireMode = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemCausesValidation"),
#endif
DefaultValue(false)]
		public virtual bool CausesValidation {
			get { return causesValidation; }
			set { causesValidation = value; }
		}
		protected internal virtual RibbonItemStyles GetRibbonAllowedStyles(RibbonItemViewInfo itemInfo) { 
			if(RibbonStyle == RibbonItemStyles.Default) return GetRibbonDefaultAllowedStyles(itemInfo);
			return RibbonStyle;
		}
		protected virtual RibbonItemStyles GetRibbonDefaultAllowedStyles(RibbonItemViewInfo itemInfo) {
			RibbonItemStyles res = RibbonItemStyles.SmallWithoutText;
			if(!IsImageExist || itemInfo == null || (itemInfo != null && itemInfo.OwnerButtonGroup == null)) res |= RibbonItemStyles.SmallWithText;
			if(IsLargeImageExist) res |= RibbonItemStyles.Large;
			return res;
		}
		protected virtual BorderStyles DefaultBorder { get { return BorderStyles.NoBorder; } }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) { }
		void IXtraSerializable.OnStartSerializing() { }
		void IXtraSerializable.OnEndSerializing() { }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAccessibleName"),
#endif
 DefaultValue(null), Localizable(true), Category("Accessibility")]
		public virtual string AccessibleName {
			get { return accessibleName; }
			set { accessibleName = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAccessibleDescription"),
#endif
 DefaultValue(null), Localizable(true), System.ComponentModel.Category("Accessibility")]
		public virtual string AccessibleDescription {
			get { return accessibleDescription; }
			set { accessibleDescription = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAlignment"),
#endif
 DefaultValue(BarItemLinkAlignment.Default), System.ComponentModel.Category("Appearance")]
		public virtual BarItemLinkAlignment Alignment {
			get { return alignment; }
			set {
				if(Alignment == value) return;
				alignment = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAllowGlyphSkinning"),
#endif
 DefaultValue(DefaultBoolean.Default), Category("Appearance"), SmartTagProperty("Allow Glyph Skinning", "Image", 5)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value)
					return;
				allowGlyphSkinning = value;
				OnAllowGlyphSkinningChanged();
			}
		}
		protected virtual void OnAllowGlyphSkinningChanged() {
			this.effectiveGlyphSkinning = AllowGlyphSkinning;
			OnItemChanged(true);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLargeWidth"),
#endif
 DefaultValue(0), Category("Behavior"), SupportedByRibbon(SupportedByRibbonKind.Supported), SupportedByBarManager(SupportedByBarManagerKind.NonSupported), Localizable(true)]
		public virtual int LargeWidth {
			get { return largeWidth; }
			set {
				if(LargeWidth == value) return;
				largeWidth = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemSmallWithTextWidth"),
#endif
 DefaultValue(0), Category("Behavior"), SupportedByRibbon(SupportedByRibbonKind.Supported), SupportedByBarManager(SupportedByBarManagerKind.NonSupported), Localizable(true)]
		public virtual int SmallWithTextWidth {
			get { return smallWithTextWidth; }
			set {
				if(SmallWithTextWidth == value) return;
				smallWithTextWidth = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemSmallWithoutTextWidth"),
#endif
 DefaultValue(0), Category("Behavior"), SupportedByRibbon(SupportedByRibbonKind.Supported), SupportedByBarManager(SupportedByBarManagerKind.NonSupported), Localizable(true)]
		public virtual int SmallWithoutTextWidth {
			get { return smallWithoutTextWidth; }
			set {
				if(SmallWithoutTextWidth == value) return;
				smallWithoutTextWidth = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemUseOwnFont"),
#endif
 DefaultValue(false), Category("Appearance"), Obsolete("Use the Appearance.Options.UseFont property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual bool UseOwnFont {
			get { return Appearance.Options.UseFont; }
			set { Appearance.Options.UseFont = value; }
		}
		internal void ResetOwnFont() {
			if(this.appearance == null) return;
			Appearance.Font = AppearanceObject.DefaultMenuFont;
		}
		internal bool ShouldSerializeOwnFont() { return false; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemOwnFont"),
#endif
 Category("Appearance"), Obsolete("Use the Appearance.Font property to specify the item's font."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Font OwnFont {
			get { return Appearance.Font; }
			set { Appearance.Font = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAppearanceDisabled"),
#endif
 Category("Appearance"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BarItemAppearance AppearanceDisabled {
			get { return (BarItemAppearance)ItemAppearance.Disabled; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemAppearance"),
#endif
 Category("Appearance"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BarItemAppearance Appearance {
			get { return (BarItemAppearance)ItemAppearance.Normal; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BarItemAppearances ItemAppearance { get { return this.appearance; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemInMenuAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual BarItemAppearances ItemInMenuAppearance { get { return this.appearanceInMenu; } }
		protected virtual void OnOwnAppearanceChanged(object sender, EventArgs e) {
			OnItemChanged();
		}
		DefaultBoolean showItemShortcut = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowItemShortcut {
			get { return showItemShortcut; }
			set {
				if(ShowItemShortcut == value)
					return;
				showItemShortcut = value;
				OnItemChanged();
			}
		}
		[Browsable(false)]
		public string Name {
			get {
				if(Site == null) return name;
				return Site.Name;
			}
			set {
				if(value == null) value = string.Empty;
				if(Manager != null) Manager.Items.UpdateItemHash(this, name, value);
				name = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemRibbonStyle"),
#endif
 System.ComponentModel.Category("Behavior"), System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor)), SupportedByBarManager(SupportedByBarManagerKind.NonSupported), SmartTagProperty("Ribbon Style", "Appearance", 6, SmartTagActionType.RefreshAfterExecute)]
		public RibbonItemStyles RibbonStyle {
			get { return ribbonStyle; }
			set {
				if(RibbonStyle == value) return;
				ribbonStyle = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool ShouldSerializeRibbonStyle() {
			return RibbonStyle != RibbonItemStyles.Default;
		}
		protected internal virtual void ResetRibbonStyle() {
			RibbonStyle = RibbonItemStyles.Default;
		}
		internal bool ShouldSerializeVisibleWhenVertical() { return !VisibleWhenVertical; }
		[Browsable(false)]
		public virtual bool VisibleWhenVertical { 
			get { return true; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), XtraSerializableProperty()]
		public int Id {
			get {
				UpdateId();
				return id;
			}
			set { id = value; 
				if(Manager != null && Manager.MaxItemId < id) {
					Manager.MaxItemId = id;
					Manager.FireManagerChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemMergeType"),
#endif
 Category("Behavior"), DefaultValue(BarMenuMerge.Add)]
		public BarMenuMerge MergeType {
			get { return mergeType; }
			set {
				if(MergeType == value) return;
				this.mergeType = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemMergeOrder"),
#endif
 Category("Behavior"), DefaultValue(0)]
		public int MergeOrder {
			get { return mergeOrder; }
			set {
				if(MergeOrder == value) return;
				this.mergeOrder = value;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemTag"),
#endif
 DefaultValue(null), Category("Data"), Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set {
				tag = value;
			}
		}
		[Obsolete(BarsObsoleteText.SRObsoleteItemCategoryIndex), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CategoryIndex {
			get { 
				if(Manager == null) return -1;
				return Manager.Categories[CategoryGuid].Index;
			}
			set {
				if(Manager == null || Manager.IsLoading) {
					this.categoryIndex = value;
				}
				return;
			}
		}
		bool ShouldSerializeCategoryGuid() { return CategoryGuid != BarManagerCategory.DefaultCategory.Guid; }
		[Browsable(false), Category("Behavior")]
		public Guid CategoryGuid {
			get { return categoryGuid; }
			set {
				if(CategoryGuid == value) return;
				categoryGuid = value;
				FireChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManagerCategory Category { 
			get { 
				if(Manager == null) return null;
				return Manager.Categories[CategoryGuid];
			}
			set {
				if(value == null) value = BarManagerCategory.DefaultCategory;
				if(Category == value) return;
				CategoryGuid = value.Guid;
			}
		}
		internal void SetCategory(Guid guid) {
			this.categoryGuid = guid; 
		}
		bool ShouldSerializeBorder() { return Border != DefaultBorder; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemBorder"),
#endif
 Category("Appearance"), SupportedByRibbon(SupportedByRibbonKind.NonSupported)]
		public virtual BorderStyles Border {
			get { return border; }
			set {
				if(Border == value) return;
				border = value;
				OnItemChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use the Border property")]
		public BarItemBorderStyle BorderStyle {
			get { return Border == BorderStyles.NoBorder ? BarItemBorderStyle.None : BarItemBorderStyle.Single; }
			set {
				if(BorderStyle == value) return;
				if(value == BarItemBorderStyle.None) Border = BorderStyles.NoBorder;
				else Border = BorderStyles.Default;
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemEnabled"),
#endif
 DefaultValue(true), Category("Behavior"), SmartTagProperty("Enabled", "Appearance", 8)]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				OnEnabledChanged();
			}
		}
		DefaultBoolean allowHtmlText;
		[ DefaultValue(DefaultBoolean.Default), Category("Appearance")]
		public virtual DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(allowHtmlText == value) return;
				allowHtmlText = value;
				OnItemChanged();
			}
		}
		[Browsable(false)]
		public bool IsAllowHtmlText {
			get {
				if(Ribbon != null && (Ribbon.AllowHtmlText || Manager.AllowHtmlText || AllowHtmlText == DefaultBoolean.True) && AllowHtmlText != DefaultBoolean.False) return true;
				else if(Manager != null && (Manager.AllowHtmlText || AllowHtmlText == DefaultBoolean.True) && AllowHtmlText != DefaultBoolean.False) return true;
				return false;
			}
		}
		protected virtual bool UpdateLayoutOnEnabledChanged { get { return false; } }
		protected virtual void OnEnabledChanged() {
			UpdateLinkProperty(BarLinkProperty.Enabled);
			OnItemChanged(false, !UpdateLayoutOnEnabledChanged);
			UpdateHolderItemLink();
		}
		protected void UpdateHolderItemLink() {
			foreach(BarItemLink link in GetLinksToNotify()) {
				if(link.LastCommandOwnerItem == null) continue;
				link.LastCommandOwnerItem.OnItemChanged(false, false);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemVisibility"),
#endif
 DefaultValue(BarItemVisibility.Always), Category("Behavior")]
		public virtual BarItemVisibility Visibility {
			get { return visibility; }
			set {
				if(Visibility == value) return;
				visibility = value;
				UpdateLinkProperty(BarLinkProperty.Visibility);
				OnItemChanged(false);
				if(Ribbon != null) {
					Ribbon.Refresh();
					if(Ribbon.StatusBar != null) Ribbon.StatusBar.Refresh();
				}
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemGlyphDisabled"),
#endif
 DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image GlyphDisabled {
			get { return glyphDisabled; }
			set {
				if(GlyphDisabled == value) return;
				Image prev = GlyphDisabled;
				glyphDisabled = UpdateImage(value);
				OnItemChanged(IsSameSize(prev, GlyphDisabled));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemGlyph"),
#endif
 Category("Appearance"), SmartTagProperty("Glyph", "Image", 0, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Glyph {
			get { return glyph; }
			set {
				if(value == Glyph) return;
				Image prev = Glyph;
				glyph = UpdateImage(value);
				OnItemChanged(IsSameSize(prev, Glyph));
			}
		}
		protected internal virtual bool ShouldSerializeGlyph() {
			return Glyph != null;
		}
		protected internal virtual void ResetGlyph() {
			Glyph = null;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemImageUri"),
#endif
 Category("Appearance"), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagProperty("ImageUri", "Image", 4, SmartTagActionType.RefreshAfterExecute)]
		public virtual DxImageUri ImageUri {
			get { return imageUri; }
			set {
				if(value == null || ImageUri.Equals(value)) return;
				DxImageUri prev = ImageUri;
				this.imageUri = value;
				OnImageUriChanged(prev, value);
			}
		}
		protected virtual void OnImageUriChanged(DxImageUri prev, DxImageUri next) {
			if(prev != null) {
				prev.Changed -= OnImageUriChanged;
			}
			if(next != null) {
				next.Changed += OnImageUriChanged;
				next.SetClient(this);
			}
			OnItemChanged(false);
		}
		void OnImageUriChanged(object sender, EventArgs e) {
			OnImageUriChangedCore();
		}
		protected virtual void OnImageUriChangedCore() {
			OnItemChanged(false);
		}
		protected internal virtual bool ShouldSerializeImageUri() {
			return ImageUri.ShouldSerialize();
		}
		protected internal virtual void ResetImageUri() {
			ImageUri.Reset();
		}
		protected bool IsSameSize(Image old, Image newImage) {
			if(old == newImage) return true;
			if(old == null || newImage == null) return false;
			return old.Size == newImage.Size;
		}
		protected Image UpdateImage(Image image) {
			return image;
		}
		[Browsable(false), DefaultValue(Shortcut.None), Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("You should use the ItemShortcut property")]
		public virtual Shortcut ShortCut {
			get { return shortCut; }
			set {
				if(value == ShortCut) return;
				shortCut = value;
				ItemShortcut = new BarShortcut(shortCut);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemShortcut"),
#endif
 Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.XtraBars.Design.BarShortcutEditor, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual BarShortcut ItemShortcut {
			get { return itemShortcut; }
			set {
				if(value == null || !value.IsExist) value = BarShortcut.Empty;
				if(value == ItemShortcut) return;
				itemShortcut = value;
				if(!string.IsNullOrEmpty(ShortcutKeyDisplayString) && ItemShortcut == BarShortcut.Empty) {
					itemShortcut = BarShortcut.Empty.Clone();
				}
				if(!String.IsNullOrEmpty(shortcutKeyDisplayString)) {
					ItemShortcut.DisplayString = shortcutKeyDisplayString;
				}
				OnItemChanged(true);
			}
		}
		protected internal virtual bool ShouldSerializeItemShortcut() { return ItemShortcut != BarShortcut.Empty; }
		protected internal virtual void ResetItemShortcut() {
			ItemShortcut = BarShortcut.Empty;
		}
		string shortcutKeyDisplayString = string.Empty;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemShortcutKeyDisplayString"),
#endif
 Category("Behavior"), DefaultValue(""), RefreshProperties(RefreshProperties.Repaint), Localizable(true)]
		public virtual string ShortcutKeyDisplayString {
			get { return GetShortcutKeyDisplayString(ItemShortcut); }
			set {
				if(ShortcutKeyDisplayString == value) return;
				shortcutKeyDisplayString = value;
				if(ItemShortcut != null && ItemShortcut != BarShortcut.Empty)
					ItemShortcut.DisplayString = shortcutKeyDisplayString;
				OnItemChanged(true);
			}
		}
		protected internal virtual string GetShortcutKeyDisplayString(BarShortcut shortcut) {
			return shortcut != null && shortcut != BarShortcut.Empty ? shortcut.DisplayString : shortcutKeyDisplayString;
		}
		[Browsable(false)]
		public virtual Font Font { 
			get { 
				if((this.appearance != null && Appearance.Options.UseFont) || Manager == null) return Appearance.Font;
				return Manager.DrawParameters.ItemsFont; 
			} 
		}
		[Browsable(false)]
		public object LargeImages { get { return Manager == null ? null : Manager.LargeImages; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLargeImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("LargeImages"), SmartTagProperty("Large Image Index", "Image", 3, SmartTagActionType.RefreshAfterExecute)]
		public virtual int LargeImageIndex {
			get { return largeImageIndex; }
			set {
				if(LargeImageIndex == value) return;
				bool prevExist = IsLargeImageExist;
				largeImageIndex = value;
				OnItemChanged(prevExist == IsLargeImageExist);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLargeImageIndexDisabled"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("LargeImages")]
		public virtual int LargeImageIndexDisabled {
			get { return largeImageIndexDisabled; }
			set {
				if(LargeImageIndexDisabled == value) return;
				largeImageIndexDisabled = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)),
		SmartTagProperty("Image Index", "Image", 2, SmartTagActionType.RefreshAfterExecute), ImageList("Images")]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(value == ImageIndex) return;
				bool imageExist = IsImageExist;
				imageIndex = value;
				UpdateLinkProperty(BarLinkProperty.ImageIndex);
				OnItemChanged(imageExist == IsImageExist);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemImageIndexDisabled"),
#endif
 DefaultValue(-1), Category("Appearance"), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("Images")]
		public virtual int ImageIndexDisabled {
			get { return imageIndexDisabled; }
			set {
				if(ImageIndexDisabled == value) return;
				imageIndexDisabled = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLargeGlyph"),
#endif
 Category("Appearance"), SmartTagProperty("Large Glyph", "Image", 1, SmartTagActionType.RefreshAfterExecute), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image LargeGlyph {
			get { return largeGlyph; }
			set {
				if(LargeGlyph == value) return;
				Image prev = LargeGlyph;
				largeGlyph = UpdateImage(value);
				OnItemChanged(IsSameSize(prev, LargeGlyph));
			}
		}
		protected internal virtual bool ShouldSerializeLargeGlyph() {
			return LargeGlyph != null;
		}
		protected internal virtual void ResetLargeGlyph() {
			LargeGlyph = null;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLargeGlyphDisabled"),
#endif
 DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image LargeGlyphDisabled {
			get { return largeGlyphDisabled; }
			set {
				if(LargeGlyphDisabled == value) return;
				Image prev = LargeGlyphDisabled;
				largeGlyphDisabled = UpdateImage(value);
				OnItemChanged(IsSameSize(prev, LargeGlyphDisabled));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemPaintStyle")
#else
	Description("")
#endif
, Category("Appearance")]
		public virtual BarItemPaintStyle PaintStyle {
			get { return paintStyle; }
			set {
				if(PaintStyle == value) return;
				paintStyle = value;
				OnItemChanged();
			}
		}
		protected internal virtual bool ShouldSerializePaintStyle() {
			return PaintStyle != BarItemPaintStyle.Standard;
		}
		protected internal virtual void ResetPaintStyle() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManager Manager {
			get {
				if(disposedManager != null) return disposedManager;
				return manager;
			}
			set {
				if(manager == value) return;
				BarManager oldManager = manager;
				manager = null;
				if(oldManager != null) oldManager.Items.Remove(this);
				manager = value;
				if(manager != null) manager.Items.Add(this);
				OnManagerChanged(oldManager);
			}
		}
		internal void RibbonSetManager(BarManager manager) { this.manager = manager; } 
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BarItemLinkCollection Links { get { return links; } }
		bool ShouldSerializeSize() { return !Size.IsEmpty; }
		void ResetSize() { Size = Size.Empty; }
		Size size = Size.Empty;
		[Localizable(true), SupportedByBarManager(SupportedByBarManagerKind.Supported), SupportedByRibbon(SupportedByRibbonKind.NonSupported)]
		public virtual Size Size {
			get { return size; }
			set {
				if(Size == value)
					return;
				size = value;
				UpdateLinkProperty(BarLinkProperty.Width);
				OnItemChanged();
			}
		}
		protected virtual int DefaultWidth { get { return 0; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int Width {
			get { return Size.Width; }
			set { Size = new Size(value, Size.Height); }
		}
		protected internal virtual int Height { get { return Size.Height; } }
		BarItemContentAlignment contentHorizontalAlignment = BarItemContentAlignment.Default;
		[DefaultValue(BarItemContentAlignment.Default), SupportedByBarManager(SupportedByBarManagerKind.Supported), SupportedByRibbon(SupportedByRibbonKind.NonSupported)]
		public BarItemContentAlignment ContentHorizontalAlignment {
			get { return contentHorizontalAlignment; }
			set {
				if(ContentHorizontalAlignment == value)
					return;
				contentHorizontalAlignment = value;
				OnItemChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemSuperTip"),
#endif
 Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor)), SmartTagProperty("Super Tip", "Appearance", 7), Category("Appearance"), Localizable(true)]
		public virtual SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		protected virtual bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		public virtual void ResetSuperTip() { SuperTip = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemDropDownSuperTip"),
#endif
Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), Category("Appearance"), SupportedByBarManager(SupportedByBarManagerKind.NonSupported), Localizable(true)]
		public SuperToolTip DropDownSuperTip {
			get { return dropDownSuperTip; }
			set { dropDownSuperTip = value; }
		}
		protected virtual bool ShouldSerializeDropDownSuperTip() { return DropDownSuperTip != null && !DropDownSuperTip.IsEmpty; }
		public virtual void ResetDropDownSuperTip() { DropDownSuperTip = null; }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemHint"),
#endif
 DefaultValue(""), Category("Appearance"), Localizable(true)]
		public virtual string Hint {
			get { return hint; }
			set {
				if(Hint == value) return;
				hint = value;
				OnItemChanged(true);
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemDescription"),
#endif
  Localizable(true), Category("Appearance")]
		public virtual string Description {
			get { return description; }
			set {
				if(Description == value) return;
				description = value;
				OnItemChanged(true);
			}
		}
		protected virtual bool ShouldSerializeDescription() { 
			return !String.IsNullOrEmpty(Description); 
		}
		protected virtual void ResetDescription() {
			Description = String.Empty;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemCaption"),
#endif
  Category("Appearance"), Localizable(true), Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)), SmartTagProperty("Caption", "Appearance", 0, SmartTagActionType.RefreshAfterExecute)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				caption = value;
				UpdateLinkProperty(BarLinkProperty.Caption);
				OnItemChanged(false);
			}
		}
		protected internal virtual bool ShouldSerializeCaption() {
			return !String.IsNullOrEmpty(Caption);
		}
		protected internal virtual void ResetCaption() {
			Caption = String.Empty;
		}
		[Browsable(false)]
		public object Images { get { return Manager == null ? null : Manager.Images; } }
		protected bool IsDisposing { get { return manager == null && disposedManager != null; } }
		public virtual void Reset() {
		}
		public virtual void BeginUpdate() {
			this.lockUpdate++;
		}
		protected internal bool IsLockUpdate { get { return lockUpdate != 0; } }
		public virtual void EndUpdate() {
			if(--this.lockUpdate == 0) 
				OnItemChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void CancelUpdate() {
			--this.lockUpdate;
		}
		public virtual bool ContainsItem(BarItem item) {
			return false;
		}
		public void Refresh() {
			if(IsLoading || Manager == null) return;
			for(int n = 0; n < Links.Count; n++) {
				Links[n].Refresh();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UpdateVisualEffects(UpdateAction.Dispose);
				if(dataBindingsCore != null)
					dataBindingsCore.Clear();
				this.dataBindingsCore = null;
				this.bindingContextCore = null;
				if(this.imageUri != null) {
					this.imageUri.Changed -= OnImageUriChanged;
					this.imageUri.Dispose();
				}
				this.imageUri = null;
				if(Links != null) {
					Links.Clear();
					this.links.CollectionChanged -= new CollectionChangeEventHandler(OnItemLinkCollectionChanged);
				}
				this.disposedManager = Manager;
				Manager = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void OnItemLinkCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Remove) {
				BarItemLink link = e.Element as BarItemLink;
				if(!SkipDisposeLinkOnRemove)
					link.Dispose();
			}
			UpdateVisualEffectCollection(e);
		}
		protected virtual void ClearLinksToItem() {
			for(int n = Links.Count - 1; n >= 0; n--) {
				(Links[n] as BarItemLink).Dispose();
			}
			Links.ClearItems();
		}
		protected internal bool GetRunTimeVisibility() {
			if(Manager == null) return false;
			return Visibility == BarItemVisibility.Always || Visibility == BarItemVisibility.OnlyInRuntime;
		}
		protected internal virtual void OnItemCreated(object creationArguments) { }
		protected internal virtual void AfterLoad() {
			UpdateId();
		}
		protected internal void UpdateId() {
			if(id == -1 && Manager != null && !IsPrivateItem && DesignMode) {
				id = Manager.GetNewItemId();
			}
		}
		protected internal virtual bool IsLoading { 
			get { 
				if(Manager == null || Manager.IsLoading || Manager.IsDestroying) return true;
				return false;
			}
		}
		protected internal virtual bool IsPrivateItem { get { return fIsPrivateItem; } set { fIsPrivateItem = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowInCustomizationForm { get { return showInCustomizationForm; } set { showInCustomizationForm = value; } }
		protected internal void OnItemChanged() { OnItemChanged(false); }
		protected virtual ICollection GetLinksToNotify() { return Links; }
		protected internal virtual void OnItemChanged(bool onlyInvalidate, bool onlyUpdateAppearance) {
			if(IsLockUpdate || IsLoading) return;
			UpdateVisualEffects(UpdateAction.BeginUpdate);
			foreach(BarItemLink link in GetLinksToNotify()) {
				if(link.Manager != null && link.Manager.IsDestroying) continue;
				OnItemChanged(link, onlyInvalidate, onlyUpdateAppearance);
			}
			if(Manager != null && !IsPrivateItem) {
				Manager.Helper.CustomizationManager.UpdateItem(this);
				Manager.Items.OnItemChanged(this);
			}
			UpdateVisualEffects(UpdateAction.EndUpdate);
		}
		protected internal virtual void OnItemChanged(bool onlyInvalidate) {
			OnItemChanged(onlyInvalidate, false);
		}
		protected virtual void OnItemChanged(BarItemLink link, bool onlyInvalidate, bool onlyUpdateAppearance) {
			if(link.ContainerEx != null) onlyInvalidate = false;
			if(onlyInvalidate || onlyUpdateAppearance) {
				link.CheckUpdateLinkState();
				if(onlyUpdateAppearance) {
					link.UpdateOwnerAppearance();
				}
				link.Invalidate();
			} else {
				link.OnLinkChanged();
			}
		}
		protected virtual void UpdateLinkProperty(BarLinkProperty property) {
			for(int n = 0; n < Links.Count; n++) {
				BarItemLink link = Links[n];
				link.OnUpdateLinkProperty(property);
			}
		}
		protected internal virtual bool CanKeyboardSelect { get { return true; } }
		protected internal virtual bool CanPress { get { return Enabled; } }
		protected internal virtual BarItemPaintStyle CalcRealPaintStyle(BarItemLink link) {
			BarItemPaintStyle ps = link == null ? PaintStyle : link.PaintStyle;
			if(ps == BarItemPaintStyle.Standard) {
				ps = BarItemPaintStyle.CaptionGlyph;
			}
			return ps;
		}
		protected internal void ResetUsageData() {
			foreach(BarItemLink link in Links) {
				link.CheckMostRecentlyUsed();
			}
		}
		protected virtual BarItemLink CreateLinkCore(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) {
			return null;
		}
		protected internal BarItemLink CreateLink(BarItemLinkReadOnlyCollection links, object linkedObject) {
			var res = CreateLinkCore(links, this, linkedObject);
			if(res != null) return res;
			if(Manager == null || Manager.Helper == null) {
				BarLinksHolder holder = linkedObject as BarLinksHolder;
				 if(holder == null || holder.Manager == null || holder.Manager.Helper == null) {
					return CreateLink(links, this, linkedObject);
				}
				Manager = holder.Manager;
			}
			return Manager.Helper.CreateLink(links, this, linkedObject); 
		}
		internal BarItemLink CreateLink(BarItemLinkReadOnlyCollection links, BarItem item, object linkedObject) {
			if(Manager != null && Manager.Helper != null) return Manager.Helper.CreateLink(links, this, linkedObject); 
			DevExpress.XtraBars.Styles.BarItemInfo itemInfo = BarAndDockingController.Default.PaintStyle.ItemInfoCollection.GetInfoByItem(item.GetType());
			if(itemInfo == null) throw new Exception(item.GetType() + " itemInfo not found");
			return itemInfo.CreateLink(links, item, linkedObject);
		}
		protected internal virtual bool CanCloseSubOnClick(BarItemLink link) { return false; }
		protected virtual void OnManagerChanged(BarManager oldManager) {
			OnManagerChanged();
			if(!IsDisposing) {
				if(ImageUri != null) ImageUri.SetClient(this);
				if(GetParentBindableComponent(Manager) != GetParentBindableComponent(oldManager))
					OnBindingContextChanged();
			}
		}
		protected virtual void OnManagerChanged() { }
		internal void FireChanged() {
			if(!DesignMode || Manager == null || IsPrivateItem) return;
			Manager.FireManagerChanged();
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				if(AllowFireChanging) {
					srv.OnComponentChanging(this, null);
				}
				srv.OnComponentChanged(this, null, null, null);
			}
		}
		protected virtual bool AllowFireChanging { get { return true; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarItemHyperlinkClick")]
#endif
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { Events.AddHandler(hyperlinkClick, value); }
			remove { Events.RemoveHandler(hyperlinkClick, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemClick"),
#endif
 Category("Events")]
		public event ItemClickEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemDoubleClick"),
#endif
 Category("Events")]
		public event ItemClickEventHandler ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemItemPress"),
#endif
 Category("Events")]
		public event ItemClickEventHandler ItemPress {
			add { Events.AddHandler(itemPress, value); }
			remove { Events.RemoveHandler(itemPress, value); }
		}
		public virtual void PerformClick() {
			PerformClick(null);
		}
		public virtual void PerformClick(BarItemLink link) {
			if(!Enabled)
				return;
			OnClick(link);
		}
		protected internal virtual void OnClick(BarItemLink link) {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[itemClick];
			if(!PerformValidation())
				return;
			if(handler != null) {
				ItemClickEventArgs e = new ItemClickEventArgs(this, link);
				if(ItemClickFireMode != BarItemEventFireMode.Postponed || Manager.Form == null)
					handler(Manager, e);
				else
					Manager.Form.BeginInvoke(handler, new object[] { Manager, e });
			}
			if(Manager == null) return; 
			if(!IsPrivateItem) {
				Manager.RaiseItemClick(new ItemClickEventArgs(this, link));
			}
		}
		protected virtual bool PerformValidation() {
			if(CausesValidation && Manager != null && Manager.GetForm() != null) {
				if(Manager.IsMdiContainerManager && Manager.ActiveMdiChild != null)
					return Manager.ActiveMdiChild.ValidateChildren();
				if(!Manager.IsMdiContainerManager) {
					var documentManager = Docking2010.DocumentManager.FromControl(Manager.Form);
					if(documentManager != null && !documentManager.IsNoDocumentsStrategyInUse) {
						var activeChild = documentManager.GetActiveChild() as Docking2010.DocumentContainer;
						if(activeChild != null && activeChild.CanValidateChild())
							return activeChild.ValidateChild();
					}
				}
				return Manager.GetForm().ValidateChildren();
			}
			return true;
		}
		protected internal virtual void OnHyperLinkClick(BarItemLink link, StringBlock hyperlink) {
			HyperlinkClickEventHandler handler = (HyperlinkClickEventHandler)Events[hyperlinkClick];
			HyperlinkClickEventArgs e = new HyperlinkClickEventArgs() { Link = hyperlink.Link, Text = hyperlink.Text };
			if(handler != null)
				handler(Manager, e);
			if(Manager == null)
				return;
			if(!IsPrivateItem)
				Manager.RaiseHyperlinkClick(e);
		}
		protected internal virtual void OnDoubleClick(BarItemLink link) {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[itemDoubleClick];
			if(handler != null)
				handler(Manager, new ItemClickEventArgs(this, link));
			if(Manager == null) return; 
			if(!IsPrivateItem)
				Manager.RaiseItemDoubleClick(new ItemClickEventArgs(this, link));
		}
		protected internal virtual void OnPress(BarItemLink link) {
			ItemClickEventHandler handler = (ItemClickEventHandler)Events[itemPress];
			if(handler != null)
				handler(Manager, new ItemClickEventArgs(this, link));
			if(Manager == null) return; 
			if(!IsPrivateItem)
				Manager.RaiseItemPress(new ItemClickEventArgs(this, link));
		}
		internal void RaiseLinkChanged() { OnLinkChanged(); }
		protected virtual void OnLinkChanged() { FireChanged();	}
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return FilterProperties(TypeDescriptor.GetProperties(this, attributes, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return FilterProperties(TypeDescriptor.GetProperties(this, true));
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		protected internal virtual RibbonControl Ribbon {
			get {
				RibbonBarManager manager = Manager as RibbonBarManager;
				return manager == null ? null : manager.Ribbon;
			}
		}
		protected internal bool IsRibbonAssigned { get { return Ribbon != null; } }
		PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection collection) {
			if(LinkProvider != null || Ribbon != null) {
				ArrayList list = new ArrayList(collection);
				if(LinkProvider != null) {
					foreach(PropertyDescriptor pd in TypeDescriptor.GetProperties(LinkProvider)) {
						if(!pd.IsBrowsable) continue;
						list.Add(new RedirectedPropertyDescriptor(LinkProvider, pd));
					}
				}
				if(Ribbon != null) FilterNonRibbonProperties(list);
				else FilterNonBarManagerProperties(list);
				collection = new PropertyDescriptorCollection(list.ToArray(typeof(PropertyDescriptor)) as PropertyDescriptor[]);
			}
			return collection;
		}
		void FilterNonBarManagerProperties(ArrayList collection) {
			for(int n = collection.Count - 1; n >= 0; n--) {
				PropertyDescriptor pd = (PropertyDescriptor)collection[n];
				SupportedByBarManager supported = pd.Attributes[typeof(SupportedByBarManager)] as SupportedByBarManager;
				if(supported != null && supported.Support == SupportedByBarManagerKind.NonSupported)
					collection.RemoveAt(n);
			}
		}
		void FilterNonRibbonProperties(ArrayList collection) {
			for(int n = collection.Count - 1; n >= 0; n--) {
				PropertyDescriptor pd = (PropertyDescriptor)collection[n];
				SupportedByRibbon supported = pd.Attributes[typeof(SupportedByRibbon)] as SupportedByRibbon;
				if(supported != null && 
					(supported.Support == SupportedByRibbonKind.NonSupported || 
					(supported.Support == SupportedByRibbonKind.SupportedInMenu && LinkProvider != null)))
						collection.RemoveAt(n);
			}
		}
		protected internal virtual BarLinkInfoProvider CreateLinkInfoProvider(BarItemLink link) {
			return new BarLinkInfoProvider(link);
		}
		BarLinkInfoProvider linkProvider;
		internal BarLinkInfoProvider LinkProvider {
			get { return linkProvider; }
			set { linkProvider = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public BarLinkInfoProvider GetBarLinkInfoProvider() {
			return LinkProvider;
		}
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		#region IDXImageUriClient
		UserLookAndFeel IDXImageUriClient.LookAndFeel {
			get { return Manager != null ? Manager.GetController().LookAndFeel : null; }
		}
		bool IDXImageUriClient.SupportsLookAndFeel {
			get { return true; }
		}
		void IDXImageUriClient.SetGlyphSkinningValue(bool value) {
			this.effectiveGlyphSkinning = value ? DefaultBoolean.True : DefaultBoolean.False;
		}
		bool IDXImageUriClient.SupportsGlyphSkinning {
			get { return AllowGlyphSkinning != DefaultBoolean.False; }
		}
		bool IDXImageUriClient.IsDesignMode { get { return DesignMode; } }
		#endregion
		internal bool GetAllowGlyphSkinning() {
			if(this.effectiveGlyphSkinning != DefaultBoolean.Default) {
				return this.effectiveGlyphSkinning == DefaultBoolean.True ? true : false;
			}
			return Manager != null && Manager.AllowGlyphSkinning;
		}
		#region ISupportAdornerElementBarItem Members
		readonly static object updateVisualEffects = new object();
		readonly static object visualEffectCollectionChanged = new object();
		event UpdateActionEventHandler ISupportAdornerElementBarItem.Changed {
			add { Events.AddHandler(updateVisualEffects, value); }
			remove { Events.RemoveHandler(updateVisualEffects, value); }
		}
		protected void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[updateVisualEffects] as UpdateActionEventHandler;
			if(handler == null) return;
			UpdateActionEvendArgs e = new UpdateActionEvendArgs(action);
			handler(this, e);
		}
		event CollectionChangeEventHandler ISupportAdornerElementBarItem.CollectionChanged {
			add { Events.AddHandler(visualEffectCollectionChanged, value); }
			remove { Events.RemoveHandler(visualEffectCollectionChanged, value); }
		}
		protected void UpdateVisualEffectCollection(CollectionChangeEventArgs e) {
			CollectionChangeEventHandler handler = Events[visualEffectCollectionChanged] as CollectionChangeEventHandler;
			if(handler == null) return;			
			handler(this, e);
		}
		System.Collections.Generic.IEnumerable<ISupportAdornerElementBarItemLink> ISupportAdornerElementBarItem.Elements {
			get {
				System.Collections.Generic.IList<ISupportAdornerElementBarItemLink> element = new System.Collections.Generic.List<ISupportAdornerElementBarItemLink>();
				foreach(BarItemLink link in GetLinksToNotify())
					element.Add(link);
				return element;
			}
		}
		#endregion
	}
	class BarItemSmartTagFilter : ISmartTagFilter {
		BarItem barItem;
		public bool FilterMethod(string MethodName, object actionMethodItem) {
			return true;
		}
		public bool FilterProperty(MemberDescriptor descriptor) {
			if(barItem != null && !barItem.IsRibbonAssigned) {
				SupportedByBarManager attr = descriptor.Attributes[typeof(SupportedByBarManager)] as SupportedByBarManager;
				if(attr != null && attr.Support == SupportedByBarManagerKind.NonSupported) return false;
			}
			if(descriptor.Name == "LargeImageIndex" && barItem.LargeImages == null) return false;
			if(descriptor.Name == "ImageIndex" && barItem.Images == null) return false;
			return true;
		}
		public void SetComponent(IComponent component) { barItem = component as BarItem; }
	}
	[TypeConverter("DevExpress.XtraBars.TypeConverters.BarShortcutTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign),
	 System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.BarShortcutCodeDomSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class BarShortcut {
		Keys key, secondKey;
		public BarShortcut(BarShortcut shortcut) : this(shortcut.Key, shortcut.SecondKey) {
		}
		public BarShortcut(Shortcut shortcut) : this((Keys)shortcut) {
		}
		public BarShortcut() : this(Keys.None) {
		}
		public BarShortcut(Keys key) : this(key, Keys.None) {
		}
		public BarShortcut(Keys key, Keys secondKey) {
			this.key = CheckKey(key, false);
			this.secondKey = CheckKey(secondKey, true);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarShortcutKey")]
#endif
		public Keys Key { get { return key; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarShortcutSecondKey")]
#endif
		public Keys SecondKey { get { return secondKey; } }
		public override string ToString() {
			if(this == Empty) return "(none)";
			if(!IsExist) return "";
			string res = GetKeyDisplayText(Key), s2 = GetKeyDisplayText(SecondKey);
			if(IsValidShortcut(SecondKey) && s2 != "") res +=", " + s2;
			return res;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarShortcutIsExist")]
#endif
		public virtual bool IsExist {
			get {
				if(Key == Keys.None || !IsValidShortcut(Key)) return false;
				return true;
			}
		}
		string displayString = string.Empty;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarShortcutDisplayString"),
#endif
 DefaultValue("")]
		public virtual string DisplayString {
			get { return displayString; }
			set { displayString = value; }
		}
		protected virtual Keys CheckKey(Keys key, bool isSecond) {
			Keys v = IsValidShortcut(key) ? key : Keys.None;
			if(isSecond) {
				if(Key == Keys.None) v = Keys.None;
			}
			return v;
		}
		protected virtual bool IsValidShortcut(Keys key) {
			if(key == Keys.None) return false;
			key = key & (~Keys.Modifiers);
			if(key == Keys.None || key == Keys.ControlKey || key == Keys.ShiftKey || key == Keys.Alt) return false;
			return true;
		}
		protected virtual string GetKeyDisplayText(Keys key) {
			string res = "";
			if (key == DevExpress.XtraBars.Commands.CommandBasedBarButtonItem.BarShortcutAuto.Key)
				return res;
			if(key == Keys.None) return res;
			if((key & Keys.Control) != 0 || key == Keys.ControlKey) res = KeyShortcut.ControlKeyName;
			if((key & Keys.Shift) != 0 || key == Keys.ShiftKey) res += (res.Length > 0 ? "+" : "") + KeyShortcut.ShiftKeyName;
			if((key & Keys.Alt) != 0 || key == Keys.Alt) res += (res.Length > 0 ? "+" : "") + KeyShortcut.AltKeyName;
			key = key & (~Keys.Modifiers);
			if(key != Keys.None) res += (res.Length > 0 ? "+" : "") + GetKeyCodeDisplayText(key);
			return res;
		}
		static Hashtable keyDisplayText;
		static BarShortcut() {
			keyDisplayText = new Hashtable();
			keyDisplayText[Keys.OemBackslash] = "/";
			keyDisplayText[Keys.OemCloseBrackets] = "]";
			keyDisplayText[Keys.Oemcomma] = ",";
			keyDisplayText[Keys.OemMinus] = "-";
			keyDisplayText[Keys.OemOpenBrackets] = "[";
			keyDisplayText[Keys.OemPeriod] = ".";
			keyDisplayText[Keys.OemPipe] = "\\";
			keyDisplayText[Keys.Oemplus] = "+";
			keyDisplayText[Keys.OemQuestion] = "?";
			keyDisplayText[Keys.OemQuotes] = "'";
			keyDisplayText[Keys.OemSemicolon] = ";";
			keyDisplayText[Keys.Oemtilde] = "`";
			keyDisplayText[Keys.D0] = "0";
			keyDisplayText[Keys.D1] = "1";
			keyDisplayText[Keys.D2] = "2";
			keyDisplayText[Keys.D3] = "3";
			keyDisplayText[Keys.D4] = "4";
			keyDisplayText[Keys.D5] = "5";
			keyDisplayText[Keys.D6] = "6";
			keyDisplayText[Keys.D7] = "7";
			keyDisplayText[Keys.D8] = "8";
			keyDisplayText[Keys.D9] = "9";
			keyDisplayText[Keys.PageDown] = "PageDown";
			keyDisplayText[Keys.PageUp] = "PageUp";
		}
		protected virtual string GetKeyCodeDisplayText(Keys key) {
			if(keyDisplayText.ContainsKey(key)) return keyDisplayText[key].ToString();
			return KeyShortcut.GetKeyDisplayText(key);
		}
		public static BarShortcut Empty = new BarShortcut();
		public static bool operator ==(BarShortcut left, BarShortcut right) {
			if(Object.ReferenceEquals(left, right)) return true;
			if(Object.ReferenceEquals(left, null)) return false;
			if(Object.ReferenceEquals(right, null)) return false;
			return (left.Key == right.Key && left.SecondKey == right.SecondKey);
		}
		public static bool operator !=(BarShortcut left, BarShortcut right) {
			if (left == null && right == null) return false;
			if(Object.ReferenceEquals(left, null) || Object.ReferenceEquals(right, null)) return true;
			return (left.Key != right.Key || left.SecondKey != right.SecondKey);
		}
		public override bool Equals(object value) {
			BarShortcut shcut = value as BarShortcut;
			if(shcut == null) return false;
			return this.key == shcut.Key && this.SecondKey == shcut.SecondKey;
		}
		public override int GetHashCode() {
			return ((int)Key) ^ ((int)SecondKey);
		}
		internal BarShortcut Clone() {
			BarShortcut res = new BarShortcut(this);
			res.DisplayString = DisplayString;
			return res;
		}
	}
	public class BarItemAppearance : AppearanceObject {
		public BarItemAppearance() { }
		public BarItemAppearance(IAppearanceOwner owner, bool reserved) : base(owner, reserved) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor2 {
			get { return base.BackColor2; }
			set { base.BackColor2 = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Drawing.Drawing2D.LinearGradientMode GradientMode {
			get { return base.GradientMode; }
			set { base.GradientMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image Image {
			get { return base.Image; }
			set { base.Image = value; }
		}
		[Browsable(false)]
		public new Color BorderColor { 
			get { return base.BorderColor; }
			set { base.BorderColor = value; }
		}
		protected override TextOptions CreateTextOptions() {
			return new BarItemTextOptions(this);
		}
		new bool ShouldSerializeBorderColor() {
			if(!BarUtilites.IsBelongsToRadialMenuManager(this))
				return false;
			return BorderColor != Color.Empty;
		}
		void ResetBorderColor() {
			BorderColor = Color.Empty;
		}
		protected internal IAppearanceOwner OwnerCore { get { return Owner; } }
	}
	public class BarItemTextOptions : TextOptions {
		public BarItemTextOptions(AppearanceObject appearance) : base(appearance) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override HorzAlignment HAlignment {
			get { return base.HAlignment; }
			set { base.HAlignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override VertAlignment VAlignment {
			get { return base.VAlignment; }
			set { base.VAlignment = value; }
		}
	}
	public enum BarItemEventFireMode { Default, Immediate, Postponed }
	public enum BarItemContentAlignment { Default, Stretch, Near, Center, Far }
	public class BarItemAppearances : StateAppearances {
		public BarItemAppearances(IAppearanceOwner owner) : base(owner) { }
		protected override AppearanceObject CreateAppearanceCore() {
			return new BarItemAppearance(this, true);
		}
		protected internal IAppearanceOwner Owner { get { return base.OwnerCore; } }
	}
}
