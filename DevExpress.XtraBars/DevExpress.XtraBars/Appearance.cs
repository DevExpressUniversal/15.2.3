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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.XtraBars {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DocumentManagerAppearances : BaseOwnerAppearance {
		AppearanceObject viewCore;
		public DocumentManagerAppearances(IAppearanceOwner owner)
			: base(owner) {
			this.viewCore = CreateAppearance();
		}
		bool ShouldSerializeView() { return View.ShouldSerialize(); }
		void ResetView() { View.Reset(); View.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject View { get { return viewCore; } }
		public override void Dispose() {
			DestroyAppearance(View);
		}
		protected override void OnResetCore() {
			ResetView();
		}
		protected internal virtual void Combine(DocumentManagerAppearances main, DocumentManagerAppearances manager, DocumentManagerAppearances defaultAppearance) {
			BeginUpdate();
			try {
				AppearanceHelper.Combine(View, new AppearanceObject[] { main.View, manager.View, defaultAppearance.View });
			}
			finally { EndUpdate(); }
		}
		protected internal virtual void Assign(DocumentManagerAppearances appearance) {
			BeginUpdate();
			try {
				View.Assign(appearance.View);
			}
			finally { EndUpdate(); }
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonGalleryAppearances : BaseOwnerAppearance {
		AppearanceObject filterPanelCaption,
			groupCaption;
		StateAppearances appearanceItemCaption, appearanceItemDescription;
		public RibbonGalleryAppearances(IAppearanceOwner owner)
			: base(owner) {
			this.filterPanelCaption = CreateAppearance();
			this.groupCaption = CreateAppearance();
			this.appearanceItemCaption = CreateStateAppearance();
			this.appearanceItemDescription = CreateStateAppearance(); 
		}
		protected virtual StateAppearances CreateStateAppearance() {
			StateAppearances res = new StateAppearances(this);
			res.Changed += new EventHandler(OnApperanceChanged);
			return res;
		}
		bool ShouldSerializeItemCaptionAppearance() { return ItemCaptionAppearance.ShouldSerialize(); }
		void ResetItemCaptionAppearance() { ItemCaptionAppearance.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonGalleryAppearancesItemCaptionAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances ItemCaptionAppearance {
			get { return appearanceItemCaption; }
		}
		bool ShouldSerializeItemDescriptionAppearance() { return ItemDescriptionAppearance.ShouldSerialize(); }
		void ResetItemDescriptionAppearance() { ItemDescriptionAppearance.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonGalleryAppearancesItemDescriptionAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public StateAppearances ItemDescriptionAppearance {
			get { return appearanceItemDescription; }
		}
		bool ShouldSerializeFilterPanelCaption() { return FilterPanelCaption.ShouldSerialize(); }
		void ResetFilterPanelCaption() { FilterPanelCaption.Reset(); FilterPanelCaption.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonGalleryAppearancesFilterPanelCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject FilterPanelCaption { get { return filterPanelCaption; } }
		bool ShouldSerializeGroupCaption() { return GroupCaption.ShouldSerialize(); }
		void ResetGroupCaption() { GroupCaption.Reset(); GroupCaption.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonGalleryAppearancesGroupCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject GroupCaption { get { return groupCaption; } }
		[Obsolete("Use ItemCaptionAppearance instead of ItemCaption"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual AppearanceObject ItemCaption { get { return ItemCaptionAppearance.Normal; } }
		[Obsolete("Use ItemDescriptionAppearance instead of ItemDescription"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual AppearanceObject ItemDescription { get { return ItemDescriptionAppearance.Normal; } }
		public override void Dispose() {
			DestroyAppearance(FilterPanelCaption);
			DestroyAppearance(GroupCaption);
		}
		protected override void OnResetCore() {
			ResetFilterPanelCaption();
			ResetItemCaptionAppearance();
			ResetItemDescriptionAppearance();
			ResetGroupCaption();
		}
		protected internal virtual void Combine(RibbonGalleryAppearances main, RibbonGalleryAppearances ribbon, RibbonGalleryAppearances defaultAppearance) {
			BeginUpdate();
			try {
				AppearanceHelper.Combine(FilterPanelCaption, new AppearanceObject[] { main.FilterPanelCaption, ribbon.FilterPanelCaption, defaultAppearance.FilterPanelCaption });
				AppearanceHelper.Combine(GroupCaption, new AppearanceObject[] { main.GroupCaption, ribbon.GroupCaption, defaultAppearance.GroupCaption });
				ItemCaptionAppearance.Combine(new StateAppearances[]{ main.ItemCaptionAppearance, ribbon.ItemCaptionAppearance, defaultAppearance.ItemCaptionAppearance});
				ItemDescriptionAppearance.Combine(new StateAppearances[]{ main.ItemDescriptionAppearance, ribbon.ItemDescriptionAppearance, defaultAppearance.ItemDescriptionAppearance});								
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void Assign(RibbonGalleryAppearances appearance) {
			BeginUpdate();
			try {
				FilterPanelCaption.Assign(appearance.FilterPanelCaption);
				GroupCaption.Assign(appearance.GroupCaption);
				ItemCaptionAppearance.Assign(appearance.ItemCaptionAppearance);
				ItemDescriptionAppearance.Assign(appearance.ItemDescriptionAppearance);
			}
			finally {
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BackstageViewAppearances : BaseOwnerAppearance {
		AppearanceObject button, buttonPressed, buttonHover, buttonDisabled;
		AppearanceObject tab, tabSelected, tabHover, tabDisabled;
		AppearanceObject backstageView;
		AppearanceObject separator;
		public BackstageViewAppearances(IAppearanceOwner owner) : base(owner) {
			this.button = CreateAppearance();
			this.buttonPressed = CreateAppearance();
			this.buttonHover = CreateAppearance();
			this.buttonDisabled = CreateAppearance();
			this.tab = CreateAppearance();
			this.tabSelected = CreateAppearance();
			this.tabHover = CreateAppearance();
			this.tabDisabled = CreateAppearance();
			this.backstageView = CreateAppearance();
			this.separator = CreateAppearance();
		}
		bool ShouldSerializeButton() { return Button.ShouldSerialize(); }
		void ResetButton() { Button.Reset(); Button.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Button { get { return button; } }
		bool ShouldSerializeButtonHover() { return ButtonHover.ShouldSerialize(); }
		void ResetButtonHover() { ButtonHover.Reset(); ButtonHover.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ButtonHover { get { return buttonHover; } }
		bool ShouldSerializeButtonPressed() { return ButtonPressed.ShouldSerialize(); }
		void ResetButtonPressed() { ButtonPressed.Reset(); ButtonPressed.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ButtonPressed { get { return buttonPressed; } }
		bool ShouldSerializeButtonDisabled() { return ButtonDisabled.ShouldSerialize(); }
		void ResetButtonDisabled() { ButtonDisabled.Reset(); ButtonDisabled.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ButtonDisabled { get { return buttonDisabled; } }
		bool ShouldSerializeTab() { return Tab.ShouldSerialize(); }
		void ResetTab() { Tab.Reset(); Tab.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Tab { get { return tab; } }
		bool ShouldSerializeTabHover() { return TabHover.ShouldSerialize(); }
		void ResetTabHover() { TabHover.Reset(); TabHover.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject TabHover { get { return tabHover; } }
		bool ShouldSerializeTabSelected() { return TabSelected.ShouldSerialize(); }
		void ResetTabSelected() { TabSelected.Reset(); TabSelected.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject TabSelected { get { return tabSelected; } }
		bool ShouldSerializeTabDisabled() { return TabDisabled.ShouldSerialize(); }
		void ResetTabDisabled() { TabDisabled.Reset(); TabDisabled.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject TabDisabled { get { return tabDisabled; } }
		bool ShouldSerializeBackstageView() { return BackstageView.ShouldSerialize(); }
		void ResetBackstageView() { BackstageView.Reset(); BackstageView.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject BackstageView { get { return backstageView; } }
		bool ShouldSerializeSeparator() { return Separator.ShouldSerialize(); }
		void ResetSeparator() { Separator.Reset(); Separator.Options.Reset(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Separator { get { return separator; } }
		protected override void OnResetCore() {
			ResetButton();
			ResetButtonHover();
			ResetButtonPressed();
			ResetButtonDisabled();
			ResetTab();
			ResetTabHover();
			ResetTabSelected();
			ResetTabDisabled();
			ResetBackstageView();
			ResetSeparator();
		}
		public override void Dispose() {
			base.Dispose();
			DestroyAppearance(Button);
			DestroyAppearance(ButtonHover);
			DestroyAppearance(ButtonPressed);
			DestroyAppearance(ButtonDisabled);
			DestroyAppearance(Tab);
			DestroyAppearance(TabHover);
			DestroyAppearance(TabSelected);
			DestroyAppearance(TabDisabled);
			DestroyAppearance(BackstageView);
			DestroyAppearance(Separator);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonAppearances : BaseOwnerAppearance {
		AppearanceObject pageGroupCaption, item, pageHeader, itemDisabled, formCaption, itemDescription, itemDescriptionDisabled, pageCategory,
			itemHovered, itemPressed, itemDescriptionHovered, itemDescriptionPressed, editor;
		RibbonGalleryAppearances gallery;
		Color formCaptionForeColor2, formCaptionForeColorInactive;
		public RibbonAppearances(IAppearanceOwner owner) : base(owner) {
			this.gallery = new RibbonGalleryAppearances(owner);
			this.gallery.Changed += new EventHandler(OnGalleryChanged);
			this.formCaptionForeColor2 = formCaptionForeColorInactive = Color.Empty;
			this.formCaption = CreateAppearance();
			this.pageGroupCaption = CreateAppearance();
			this.item = CreateAppearance();
			this.editor = CreateAppearance();
			this.itemHovered = CreateAppearance();
			this.itemPressed = CreateAppearance();
			this.itemDescription = CreateAppearance();
			this.itemDescriptionHovered = CreateAppearance();
			this.itemDescriptionPressed = CreateAppearance();
			this.itemDescriptionDisabled = CreateAppearance();
			this.pageHeader = CreateAppearance();
			this.pageCategory = CreateAppearance();
			this.itemDisabled = CreateAppearance();
		}
		void OnGalleryChanged(object sender, EventArgs e) {
			OnApperanceChanged(this, EventArgs.Empty);
		}
		bool ShouldSerializeGallery() { return Gallery.ShouldSerialize(); }
		void ResetGallery() { Gallery.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesGallery"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RibbonGalleryAppearances Gallery { get { return gallery; } }
		bool ShouldSerializeFormCaption() { return FormCaption.ShouldSerialize(); }
		void ResetFormCaption() { FormCaption.Reset(); FormCaption.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesFormCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject FormCaption { get { return formCaption; } }
		bool ShouldSerializePageGroupCaption() { return PageGroupCaption.ShouldSerialize(); }
		void ResetPageGroupCaption() { PageGroupCaption.Reset(); PageGroupCaption.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesPageGroupCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject PageGroupCaption { get { return pageGroupCaption; } }
		bool ShouldSerializeItem() { return Item.ShouldSerialize(); }
		void ResetItem() { Item.Reset(); Item.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItem"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Item { get { return item; } }
		bool ShouldSerializeEditor() { return Editor.ShouldSerialize(); }
		void ResetEditor() { Editor.Reset(); Editor.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesEditor"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Editor { get { return editor; } }
		bool ShouldSerializeItemHovered() { return ItemHovered.ShouldSerialize(); }
		void ResetItemHovered() { ItemHovered.Reset(); ItemHovered.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemHovered"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemHovered { get { return itemHovered; } }
		bool ShouldSerializeItemPressed() { return ItemPressed.ShouldSerialize(); }
		void ResetItemPressed() { ItemPressed.Reset(); ItemPressed.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemPressed"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemPressed { get { return itemPressed; } }
		bool ShouldSerializeItemDescription() { return ItemDescription.ShouldSerialize(); }
		void ResetItemDescription() { ItemDescription.Reset(); ItemDescription.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemDescription"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemDescription { get { return itemDescription; } }
		bool ShouldSerializeItemDescriptionHovered() { return ItemDescriptionHovered.ShouldSerialize(); }
		void ResetItemDescriptionHovered() { ItemDescriptionHovered.Reset(); ItemDescriptionHovered.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemDescriptionHovered"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemDescriptionHovered { get { return itemDescriptionHovered; } }
		bool ShouldSerializeItemDescriptionPressed() { return ItemDescriptionPressed.ShouldSerialize(); }
		void ResetItemDescriptionPressed() { ItemDescriptionPressed.Reset(); ItemDescriptionPressed.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemDescriptionPressed"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemDescriptionPressed { get { return itemDescriptionPressed; } }
		bool ShouldSerializeItemDescriptionDisabled() { return ItemDescriptionDisabled.ShouldSerialize(); }
		void ResetItemDescriptionDisabled() { ItemDescriptionDisabled.Reset(); ItemDescriptionDisabled.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemDescriptionDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemDescriptionDisabled { get { return itemDescriptionDisabled; } }
		bool ShouldSerializePageHeader() { return PageHeader.ShouldSerialize(); }
		void ResetPageHeader() { PageHeader.Reset(); PageHeader.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesPageHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject PageHeader { get { return pageHeader; } }
		bool ShouldSerializePageCategory() { return PageCategory.ShouldSerialize(); }
		void ResetPageCategory() { PageCategory.Reset(); PageCategory.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesPageCategory"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject PageCategory { get { return pageCategory; } }
		bool ShouldSerializeItemDisabled() { return ItemDisabled.ShouldSerialize(); }
		void ResetItemDisabled() { ItemDisabled.Reset(); ItemDisabled.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonAppearancesItemDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject ItemDisabled { get { return itemDisabled; } }
		bool ShouldSerializeFormCaptionForeColor2() { return !FormCaptionForeColor2.IsEmpty; }
		void ResetFormCaptionForeColor2() { FormCaptionForeColor2 = Color.Empty; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonAppearancesFormCaptionForeColor2")]
#endif
		public Color FormCaptionForeColor2 {
			get { return formCaptionForeColor2; }
			set {
				if(FormCaptionForeColor2 == value) return;
				formCaptionForeColor2 = value;
				OnChanged();
			}
		}
		bool ShouldSerializeFormCaptionForeColorInactive() { return !FormCaptionForeColorInactive.IsEmpty; }
		void ResetFormCaptionForeColorInactive() { FormCaptionForeColorInactive = Color.Empty; }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonAppearancesFormCaptionForeColorInactive")]
#endif
		public Color FormCaptionForeColorInactive {
			get { return formCaptionForeColorInactive; }
			set {
				if(FormCaptionForeColorInactive == value) return;
				formCaptionForeColorInactive = value;
				OnChanged();
			}
		}
		public override void Dispose() {
			Gallery.Changed -= new EventHandler(OnGalleryChanged);
			Gallery.Dispose();
			DestroyAppearance(FormCaption);
			DestroyAppearance(Item);
			DestroyAppearance(Editor);
			DestroyAppearance(ItemHovered);
			DestroyAppearance(ItemPressed);
			DestroyAppearance(ItemDisabled);
			DestroyAppearance(ItemDescription);
			DestroyAppearance(ItemDescriptionHovered);
			DestroyAppearance(ItemDescriptionPressed);
			DestroyAppearance(ItemDescriptionDisabled);
			DestroyAppearance(PageGroupCaption);
			DestroyAppearance(PageHeader);
			DestroyAppearance(PageCategory);
		}
		protected override void OnResetCore() {
			ResetGallery();
			ResetItem();
			ResetEditor();
			ResetFormCaption();
			ResetItemHovered();
			ResetItemPressed();
			ResetItemDisabled();
			ResetItemDescription();
			ResetItemDescriptionHovered();
			ResetItemDescriptionPressed();
			ResetItemDescriptionDisabled();
			ResetPageHeader();
			ResetPageCategory();
			ResetPageGroupCaption();
			ResetFormCaptionForeColor2();
			ResetFormCaptionForeColorInactive();
		}
		public override void BeginUpdate() {
			Gallery.BeginUpdate();
			base.BeginUpdate();
		}
		public override void EndUpdate() {
			Gallery.EndUpdate();
			base.EndUpdate();
		}
		protected internal virtual void Combine(RibbonAppearances main, RibbonAppearances defaultAppearance) {
			BeginUpdate();
			try {
				FormCaptionForeColor2 = main.FormCaptionForeColor2.IsEmpty ? defaultAppearance.FormCaptionForeColor2 : main.FormCaptionForeColor2;
				FormCaptionForeColorInactive = main.FormCaptionForeColorInactive.IsEmpty ? defaultAppearance.FormCaptionForeColorInactive : main.FormCaptionForeColorInactive;
				AppearanceHelper.Combine(FormCaption, new AppearanceObject[] { main.FormCaption, defaultAppearance.FormCaption });
				AppearanceHelper.Combine(Item, new AppearanceObject[] { main.Item, defaultAppearance.Item });
				AppearanceHelper.Combine(Editor, new AppearanceObject[] { main.Editor, defaultAppearance.Editor });
				AppearanceHelper.Combine(ItemHovered, new AppearanceObject[] { main.ItemHovered, defaultAppearance.ItemHovered });
				AppearanceHelper.Combine(ItemPressed, new AppearanceObject[] { main.ItemPressed, defaultAppearance.ItemPressed });
				AppearanceHelper.Combine(ItemDisabled, new AppearanceObject[] { main.ItemDisabled, defaultAppearance.ItemDisabled });
				AppearanceHelper.Combine(ItemDescription, new AppearanceObject[] { main.ItemDescription, defaultAppearance.ItemDescription });
				AppearanceHelper.Combine(ItemDescriptionHovered, new AppearanceObject[] { main.ItemDescriptionHovered, defaultAppearance.ItemDescriptionHovered });
				AppearanceHelper.Combine(ItemDescriptionPressed, new AppearanceObject[] { main.ItemDescriptionPressed, defaultAppearance.ItemDescriptionPressed });
				AppearanceHelper.Combine(ItemDescriptionDisabled, new AppearanceObject[] { main.ItemDescriptionDisabled, defaultAppearance.ItemDescriptionDisabled });
				AppearanceHelper.Combine(PageGroupCaption, new AppearanceObject[] { main.PageGroupCaption, defaultAppearance.PageGroupCaption });
				AppearanceHelper.Combine(PageHeader, new AppearanceObject[] { main.PageHeader, defaultAppearance.PageHeader });
				AppearanceHelper.Combine(PageCategory, new AppearanceObject[] { main.PageCategory, defaultAppearance.PageCategory });
			}
			finally {
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MenuAppearance : BaseOwnerAppearance {
		AppearanceObject menuBar, sideStrip, sideStripNonRecent, menuCaption, headerItemAppearance;
		StateAppearances menuAppearance;
		public MenuAppearance() : this(null) { }
		internal MenuAppearance(IAppearanceOwner owner) : base(owner) {
			this.menuCaption = CreateAppearance();
			this.menuAppearance = CreateStateAppearace();
			this.sideStripNonRecent = CreateAppearance();
			this.sideStrip = CreateAppearance();
			this.menuBar = CreateAppearance();
			this.headerItemAppearance = CreateAppearance();
		}
		protected virtual StateAppearances CreateStateAppearace() {
			StateAppearances res = new StateAppearances(this);
			res.Changed += new EventHandler(OnApperanceChanged);
			return res;
		}
		protected virtual void DestroyStateAppearance(StateAppearances state) {
			state.Changed -= new EventHandler(OnApperanceChanged);
			state.Dispose();
		}
		bool ShouldSerializeMenuCaption() { return MenuCaption.ShouldSerialize(); }
		void ResetMenuCaption() { MenuCaption.Reset(); MenuCaption.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceMenuCaption"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject MenuCaption { get { return menuCaption; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceMenu"),
#endif
 Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual AppearanceObject Menu { get { return AppearanceMenu.Normal; } }
		bool ShouldSerializeAppearanceMenu() { return AppearanceMenu.ShouldSerialize(); }
		void ResetAppearanceMenu() { AppearanceMenu.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceAppearanceMenu"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual StateAppearances AppearanceMenu { get { return menuAppearance; } }
		bool ShouldSerializeMenuBar() { return MenuBar.ShouldSerialize(); }
		void ResetMenuBar() { MenuBar.Reset(); MenuBar.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceMenuBar"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject MenuBar { get { return menuBar; } }
		bool ShouldSerializeSideStrip() { return SideStrip.ShouldSerialize(); }
		void ResetSideStrip() { SideStrip.Reset(); SideStrip.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceSideStrip"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject SideStrip { get { return sideStrip; } }
		bool ShouldSerializeSideStripNonRecent() { return SideStripNonRecent.ShouldSerialize(); }
		void ResetSideStripNonRecent() { SideStripNonRecent.Reset(); SideStripNonRecent.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceSideStripNonRecent"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject SideStripNonRecent { get { return sideStripNonRecent; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("MenuAppearanceHeaderItemAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject HeaderItemAppearance { get { return headerItemAppearance; } }
		void ResetHeaderItemAppearance() { HeaderItemAppearance.Reset(); HeaderItemAppearance.Options.Reset(); }
		public override void Dispose() {
			DestroyStateAppearance(AppearanceMenu);
			DestroyAppearance(MenuBar);
			DestroyAppearance(MenuCaption);
			DestroyAppearance(SideStrip);
			DestroyAppearance(SideStripNonRecent);
			DestroyAppearance(HeaderItemAppearance);
		}
		protected override void OnResetCore() {
			ResetMenuCaption();
			ResetAppearanceMenu();
			ResetMenuBar();
			ResetSideStrip();
			ResetSideStripNonRecent();
			ResetHeaderItemAppearance();
		}
		public virtual void UpdateRightToLeft(bool rightToLeft) {
			AppearanceMenu.UpdateRightToLeft(rightToLeft);
			MenuBar.TextOptions.RightToLeft = rightToLeft;
			MenuCaption.TextOptions.RightToLeft = rightToLeft;
			SideStrip.TextOptions.RightToLeft = rightToLeft;
			SideStripNonRecent.TextOptions.RightToLeft = rightToLeft;
			HeaderItemAppearance.TextOptions.RightToLeft = rightToLeft;
		}
		protected internal virtual void Combine(MenuAppearance main, MenuAppearance defaultAppearance) {
			BeginUpdate();
			try {
				AppearanceMenu.Combine(new StateAppearances[] { main.AppearanceMenu, defaultAppearance.AppearanceMenu });
				AppearanceHelper.Combine(MenuCaption, new AppearanceObject[] { main.menuCaption, defaultAppearance.MenuCaption });
				AppearanceHelper.Combine(MenuBar, new AppearanceObject[] { main.MenuBar, defaultAppearance.MenuBar });
				AppearanceHelper.Combine(SideStrip, new AppearanceObject[] { main.SideStrip, defaultAppearance.SideStrip});
				AppearanceHelper.Combine(SideStripNonRecent, new AppearanceObject[] { main.SideStripNonRecent, defaultAppearance.SideStripNonRecent});
				AppearanceHelper.Combine(HeaderItemAppearance, new AppearanceObject[] { main.HeaderItemAppearance, defaultAppearance.HeaderItemAppearance });
			}
			finally {
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StateAppearances : BaseOwnerAppearance, ICloneable {
		AppearanceObject appearance;
		AppearanceObject appearanceHovered;
		AppearanceObject appearancePressed;
		AppearanceObject appearanceDisabled;
		public StateAppearances(AppearanceDefault defaultApp) : this((IAppearanceOwner)null) {
			Normal.Assign(defaultApp);
			Hovered.Assign(defaultApp);
			Pressed.Assign(defaultApp);
			Disabled.Assign(defaultApp);
		}
		public StateAppearances(AppearanceObject obj)
			: this((IAppearanceOwner)null) {
			Normal.Assign(obj);
			Hovered.Assign(obj);
			Pressed.Assign(obj);
			Disabled.Assign(obj);
		}
		public StateAppearances() : this((IAppearanceOwner)null) { }
		public StateAppearances(IAppearanceOwner owner)
			: base(owner) {
			this.appearance = CreateAppearance();
			this.appearanceHovered = CreateAppearance();
			this.appearancePressed = CreateAppearance();
			this.appearanceDisabled = CreateAppearance();
		}
		public virtual void UpdateRightToLeft(bool rightToLeft) {
			Normal.TextOptions.RightToLeft = rightToLeft;
			Hovered.TextOptions.RightToLeft = rightToLeft;
			Pressed.TextOptions.RightToLeft = rightToLeft;
			Disabled.TextOptions.RightToLeft = rightToLeft;
		}
		protected override AppearanceObject CreateAppearance() {
			AppearanceObject res = CreateAppearanceCore();
			res.Changed += new EventHandler(OnApperanceChanged);
			return res;
		}
		public void Assign(StateAppearances app) {
			Normal.Assign(app.Normal);
			Hovered.Assign(app.Hovered);
			Pressed.Assign(app.Pressed);
			Disabled.Assign(app.Disabled);
		}
		public void Assign(AppearanceDefault app) {
			Normal.Assign(app);
			Hovered.Assign(app);
			Pressed.Assign(app);
			Disabled.Assign(app);
		}
		public void Assign(AppearanceObject app) {
			Normal.Assign(app);
			Hovered.Assign(app);
			Pressed.Assign(app);
			Disabled.Assign(app);
		}
		public Font SetFont(Font font) {
			Normal.Font = font;
			Hovered.Font = font;
			Pressed.Font = font;
			Disabled.Font = font;
			return font;
		}
		public void SetWordWrap(WordWrap wrapMode) {
			Normal.TextOptions.WordWrap = wrapMode;
			Hovered.TextOptions.WordWrap = wrapMode;
			Pressed.TextOptions.WordWrap = wrapMode;
			Disabled.TextOptions.WordWrap = wrapMode;
		}
		public void SetVAlignment(VertAlignment align) {
			Normal.TextOptions.VAlignment = align;
			Hovered.TextOptions.VAlignment = align;
			Pressed.TextOptions.VAlignment = align;
			Disabled.TextOptions.VAlignment = align;
		}
		protected virtual AppearanceObject CreateAppearanceCore() {
			return new AppearanceObject(this, true);
		}
		internal void ResetDisabled() {
			Disabled.Reset();
		}
		internal bool ShouldSerializeDisabled() {
			return this.appearanceDisabled != null && Disabled.ShouldSerialize();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StateAppearancesDisabled"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter("DevExpress.XtraBars.TypeConverters.BarItemAppearanceConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
		public virtual AppearanceObject Disabled {
			get { return appearanceDisabled; }
		}
		internal void ResetNormal() {
			Normal.Reset();
		}
		internal bool ShouldSerializeNormal() {
			return this.appearance != null && Normal.ShouldSerialize();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StateAppearancesNormal"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter("DevExpress.XtraBars.TypeConverters.BarItemAppearanceConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
		public virtual AppearanceObject Normal {
			get { return appearance; }
		}
		internal void ResetHovered() {
			Hovered.Reset();
		}
		internal bool ShouldSerializeHovered() {
			return this.appearanceHovered != null && Hovered.ShouldSerialize();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StateAppearancesHovered"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter("DevExpress.XtraBars.TypeConverters.BarItemAppearanceConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
		public virtual AppearanceObject Hovered {
			get { return appearanceHovered; }
		}
		internal void ResetPressed() {
			Pressed.Reset();
		}
		internal bool ShouldSerializePressed() {
			return this.appearancePressed != null && Pressed.ShouldSerialize();
		}
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter("DevExpress.XtraBars.TypeConverters.BarItemAppearanceConverter, " + AssemblyInfo.SRAssemblyBarsDesign)]
		public virtual AppearanceObject Pressed {
			get { return appearancePressed; }
		}
		protected override void OnResetCore() {
			Normal.Reset();
			Hovered.Reset();
			Pressed.Reset();
			Disabled.Reset();
		}
		public override void Dispose() {
			base.Dispose();
			DestroyAppearance(Normal);
			DestroyAppearance(Hovered);
			DestroyAppearance(Pressed);
			DestroyAppearance(Disabled);
		}
		protected internal virtual void Combine(StateAppearances main, StateAppearances defaultAppearance) {
			BeginUpdate();
			try {
				AppearanceHelper.Combine(Normal, new AppearanceObject[] { main.Normal, defaultAppearance.Normal });
				AppearanceHelper.Combine(Hovered, new AppearanceObject[] { main.Hovered, defaultAppearance.Hovered, Normal });
				AppearanceHelper.Combine(Pressed, new AppearanceObject[] { main.Pressed, defaultAppearance.Pressed, Normal });
				AppearanceHelper.Combine(Disabled, new AppearanceObject[] { main.Disabled, defaultAppearance.Disabled, Normal });
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void Combine(StateAppearances[] sources) {
			BeginUpdate();
			try {
				AppearanceObject[] src = new AppearanceObject[sources.Length];
				for(int i = 0; i < sources.Length; i++) {
					src[i] = sources[i].Normal;
				}
				AppearanceHelper.Combine(Normal, src);
				for(int i = 0; i < sources.Length; i++) {
					src[i] = sources[i].Hovered;
				}
				AppearanceHelper.Combine(Hovered, src);
				for(int i = 0; i < sources.Length; i++) {
					src[i] = sources[i].Pressed;
				}
				AppearanceHelper.Combine(Pressed, src);
				for(int i = 0; i < sources.Length; i++) {
					src[i] = sources[i].Disabled;
				}
				AppearanceHelper.Combine(Disabled, src);
			} finally {
				EndUpdate();
			}
		}
		public virtual AppearanceObject GetAppearance(BarLinkState state) {
			if((state & BarLinkState.Disabled) != 0)
				return Disabled;
			if((state & BarLinkState.Highlighted) != 0 || 
				(state & BarLinkState.Selected) != 0)
				return Hovered;
			if((state & BarLinkState.Pressed) != 0 || 
				(state & BarLinkState.Checked) != 0)
				return Pressed;
			return Normal;
		}
		public virtual AppearanceObject GetAppearance(ObjectState state) {
			switch(state) { 
				case ObjectState.Hot:
					return Hovered;
				case ObjectState.Pressed:
					return Pressed;
				case ObjectState.Disabled:
					return Disabled;
			}
			return Normal;
		}
		#region ICloneable Members
		object ICloneable.Clone() {
			StateAppearances app = new StateAppearances();
			app.Assign(this);
			return app;
		}
		#endregion
	}
}
