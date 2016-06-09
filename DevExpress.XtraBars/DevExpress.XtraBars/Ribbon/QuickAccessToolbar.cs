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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.Accessible;
using DevExpress.XtraBars.Localization;
namespace DevExpress.XtraBars.Ribbon {
	public class RibbonQuickAccessToolbar : IDisposable, IXtraSerializable, BarLinksHolder, ISupportXtraSerializer, IVisualEffectsHolder {
		private static readonly object startDeserializing = new object();
		RibbonQuickAccessToolbarPainter painter;
		RibbonQuickToolbarItemLinkCollection itemLinks;
		Rectangle bounds;
		RibbonControl ribbon;
		RibbonQuickToolbarBarItem dropDownItem;
		RibbonQuickToolbarCustomizeItem customizeItem;
		RibbonQuickToolbarCustomizeItemLink customizeItemLink;
		RibbonQuickToolbarBarItemLink dropDownItemLink;
		RibbonTouchMouseModeItem touchMouseModeItem;
		RibbonTouchMouseModeItemLink touchMouseModeItemLink;
		RibbonApplicationButtonItem applicationButtonItem;
		RibbonApplicationButtonItemLink applicationButtonItemLink;
		AccessibleQuickAccessToolbar accessibleToolbar;
		bool showCustomizeItem = true;
		object tag;
		public RibbonQuickAccessToolbar(RibbonControl ribbon) {
			this.itemLinks = new RibbonQuickToolbarItemLinkCollection(this);
			this.bounds = Rectangle.Empty;
			this.ribbon = ribbon;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonQuickAccessToolbarShowCustomizeItem"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool ShowCustomizeItem {
			get { return showCustomizeItem; }
			set {
				if(ShowCustomizeItem == value) return;
				showCustomizeItem = value;
				CustomizeItemLink.Visible = value;
				OnChanged();
			}
		}
		bool BarLinksHolder.Enabled { get { return true; } }
		protected internal bool Contains(BarItemLink link) { return ItemLinks.Contains(link); }
		protected internal void Remove(BarItem item) {
			for(int n = ItemLinks.Count - 1; n >= 0; n--) {
				if(ItemLinks[n].Item == item) ItemLinks.RemoveAt(n);
			}
		}
		protected internal bool Contains(BarItem item) {
			foreach(BarItemLink link in ItemLinks) {
				if(link.Item == item) return true;
			}
			return false;
		}
		public virtual void Dispose() {
			DestroyItems();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("RibbonQuickAccessToolbarTag"),
#endif
 DefaultValue(null), Category("Data"),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		protected virtual void DestroyItems() {
			ItemLinks.Clear();
			if(customizeItem != null) customizeItem.Dispose();
			if(dropDownItem != null) dropDownItem.Dispose();
			if(customizeItemLink != null) customizeItemLink.Dispose();
			if(dropDownItemLink != null) dropDownItemLink.Dispose();
			customizeItem = null;
			dropDownItem = null;
			customizeItemLink = null;
			dropDownItemLink = null;
		}
		protected internal bool IsDestroying { get { return Ribbon == null || Ribbon.IsDestroying; } }
		public virtual bool IsToolbarCustomizationItem(BarItemLink link) {
			if(link == null) return false;
			if(link == CustomizeItemLink || link == DropDownItemLink) return true;
			return false;
		}
		protected internal RibbonQuickToolbarCustomizeItem CustomizeItem {
			get {
				if(customizeItem == null) {
					customizeItem = new RibbonQuickToolbarCustomizeItem(Ribbon, Manager);
					customizeItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				}
				return customizeItem;
			}
		}
		protected internal RibbonQuickToolbarBarItem DropDownItem {
			get {
				if(dropDownItem == null) {
					dropDownItem = new RibbonQuickToolbarBarItem(Ribbon, Manager);
					dropDownItem.ItemPress += new ItemClickEventHandler(OnDropDownItemItemPress);
					dropDownItem.RibbonStyle = RibbonItemStyles.SmallWithoutText;
				}
				return dropDownItem;
			}
		}
		protected internal RibbonApplicationButtonItem ApplicationButtonItem {
			get {
				if(applicationButtonItem == null)
					applicationButtonItem = new RibbonApplicationButtonItem(Ribbon);
				return applicationButtonItem;
			}
		}
		protected internal RibbonTouchMouseModeItem TouchMouseModeItem {
			get {
				if(touchMouseModeItem == null)
					touchMouseModeItem = new RibbonTouchMouseModeItem(Ribbon, Manager);
				return touchMouseModeItem;
			}
		}
		internal void SetAccessible(AccessibleQuickAccessToolbar value) {
			accessibleToolbar = value;
		}
		protected internal AccessibleQuickAccessToolbar Accessible {
			get { return accessibleToolbar; }
		}
		protected virtual void OnDropDownItemItemPress(object sender, ItemClickEventArgs e) {
			if(Ribbon.IsPopupToolbarOpened) {
				Ribbon.PopupToolbar = null;
				return;
			}
			RibbonQuickToolbarPopupForm form = new RibbonQuickToolbarPopupForm(Ribbon);
			form.ToolbarInfo = Ribbon.ViewInfo.Toolbar;
			Ribbon.PopupToolbar = form;
			Ribbon.PopupToolbar.ShowPopup();
		}
		protected internal RibbonQuickToolbarBarItemLink DropDownItemLink {
			get {
				if(dropDownItemLink == null) {
					dropDownItemLink = (RibbonQuickToolbarBarItemLink)DropDownItem.CreateLink(null, DropDownItem);
					dropDownItemLink.Caption = BarLocalizer.Active.GetLocalizedString(BarString.CustomizeToolbarText);
				}
				return dropDownItemLink;
			}
		}
		protected internal RibbonQuickToolbarCustomizeItemLink CustomizeItemLink {
			get {
				if(customizeItemLink == null) {
					customizeItemLink = (RibbonQuickToolbarCustomizeItemLink)CustomizeItem.CreateLink(null, CustomizeItem);
					customizeItemLink.Caption = BarLocalizer.Active.GetLocalizedString(BarString.CustomizeToolbarText);
				}
				return customizeItemLink;
			}
		}
		protected internal RibbonApplicationButtonItemLink ApplicationButtonItemLink {
			get {
				if(applicationButtonItemLink == null)
					applicationButtonItemLink = (RibbonApplicationButtonItemLink)ApplicationButtonItem.CreateLink(null, ItemLinks);
				return applicationButtonItemLink;
			}
		}
		protected internal RibbonTouchMouseModeItemLink TouchMouseModeItemLink {
			get {
				if(touchMouseModeItemLink == null) {
					touchMouseModeItemLink = (RibbonTouchMouseModeItemLink)TouchMouseModeItem.CreateLink(null, ItemLinks);
				}
				return touchMouseModeItemLink;
			}
		}
		protected internal RibbonBarManager Manager {
			get {
				if(Ribbon != null) return Ribbon.Manager;
				return null;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("RibbonQuickAccessToolbarRibbon")]
#endif
public RibbonControl Ribbon { get { return ribbon; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, false, true)]
		public RibbonQuickToolbarItemLinkCollection ItemLinks { get { return itemLinks; } }
		protected virtual RibbonQuickAccessToolbarPainter CreatePainter() { return new RibbonQuickAccessToolbarPainter(); }
		protected internal RibbonQuickAccessToolbarPainter Painter {
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			}
		}
		protected virtual void OnChanged() {
			Ribbon.Refresh();
		}
		protected internal virtual void OnLinksChanged(CollectionChangeEventArgs e) {
			if(LinksChanged != null) LinksChanged(e);
			OnChanged();
		}
		#region layout
		public virtual void SaveLayoutToXml(string xmlFile) {
			SaveLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		public virtual void RestoreLayoutFromXml(string xmlFile) {
			RestoreLayoutCore(new XmlXtraSerializer(), xmlFile);
		}
		void ISupportXtraSerializer.SaveLayoutToRegistry(string path) { SaveLayoutCore(new RegistryXtraSerializer(), path); }
		public virtual bool SaveLayoutToRegistry(string path) {
			return SaveLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void RestoreLayoutFromRegistry(string path) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path);
		}
		public virtual void SaveLayoutToStream(System.IO.Stream stream) {
			SaveLayoutCore(new XmlXtraSerializer(), stream);
		}
		public virtual void RestoreLayoutFromStream(System.IO.Stream stream) {
			RestoreLayoutCore(new XmlXtraSerializer(), stream);
		}
		protected virtual bool SaveLayoutCore(XtraSerializer serializer, object path) {
			System.IO.Stream stream = path as System.IO.Stream;
			if(stream != null)
				return serializer.SerializeObjects(new XtraObjectInfo[] { new XtraObjectInfo("RibbonQuickAccessToolbar", this) }, stream, this.GetType().Name);
			else
				return serializer.SerializeObjects(new XtraObjectInfo[] { new XtraObjectInfo("RibbonQuickAccessToolbar", this) }, path.ToString(), this.GetType().Name);
		}
		protected virtual Form Form {
			get {
				if(Ribbon == null) return null;
				return Ribbon.FindForm();
			}
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			Ribbon.Manager.ForceLinkCreate();
			System.IO.Stream stream = path as System.IO.Stream;
			if(Form != null) Form.SuspendLayout();
			try {
				if(stream != null)
					serializer.DeserializeObjects(new XtraObjectInfo[] { new XtraObjectInfo("RibbonQuickAccessToolbar", this) }, stream, this.GetType().Name);
				else
					serializer.DeserializeObjects(new XtraObjectInfo[] { new XtraObjectInfo("RibbonQuickAccessToolbar", this) }, path.ToString(), this.GetType().Name);
			}
			finally {
				if(Form != null) Form.ResumeLayout(true);
			}
		}
		public event LinksChangedEventHandler LinksChanged;
		void IXtraSerializable.OnStartSerializing() { }
		void IXtraSerializable.OnEndSerializing() { }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) { 
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) { }
		#endregion
		internal void XtraClearItemLinks(XtraItemEventArgs e) { ItemLinks.Clear(); }
		internal RibbonPageGroup GetGroupByName(string groupName) {
			foreach(RibbonPage page in Ribbon.PageCategories.TotalCategory.Pages) {
				RibbonPageGroup group = page.Groups[groupName];
				if(group != null) return group;
			}
			return null;
		}
		internal object CreatePageGroupToolbarItem(XtraItemEventArgs e) {
			RibbonPageGroup group = GetGroupByName(e.Item.ChildProperties["PageGroupName"].Value as string);
			if(group == null) return null;
			return ItemLinks.Add(group.ToolbarContentButtonLink.Item);
		}
		internal object CreateGalleryToolbarItem(XtraItemEventArgs e) {
			RibbonGalleryBarItem item = Manager.Items[e.Item.ChildProperties["GalleryBarItemName"].Value as string] as RibbonGalleryBarItem;
			if(item == null) return null;
			return ItemLinks.Add(item);
		}
		internal object XtraCreateItemLinksItem(XtraItemEventArgs e) {
			bool isGalleryToolbarLink = bool.Parse((string)e.Item.ChildProperties["IsGalleryToolbarItemLink"].Value);
			if(isGalleryToolbarLink) return CreateGalleryToolbarItem(e);
			bool isPageGroupToolbarLink = bool.Parse((string)e.Item.ChildProperties["IsPageGroupContentToolbarButtonLink"].Value);
			if(isPageGroupToolbarLink) return CreatePageGroupToolbarItem(e);
			bool isMerged = e.Item.ChildProperties["IsMerged"] != null? bool.Parse((string)e.Item.ChildProperties["IsMerged"].Value) : false;
			RibbonBarManager manager = null;
			if(isMerged) {
				manager = Ribbon.MergedRibbon == null ? null : Ribbon.MergedRibbon.Manager;
			}
			else
				manager = Ribbon.Manager;
			if(manager == null)
				return null;
			return BarLinksHolderSerializer.CreateItemLink(manager, e, this); 
		}
		BarManager BarLinksHolder.Manager { get { return Manager; } }
		BarItemLinkCollection BarLinksHolder.ItemLinks { get { return ItemLinks; } }
		MenuDrawMode BarLinksHolder.MenuDrawMode { get { return MenuDrawMode.Default; } }
		BarItemLink BarLinksHolder.AddItem(BarItem item) { return ItemLinks.Add(item); }
		BarItemLink BarLinksHolder.AddItem(BarItem item, LinkPersistInfo info) { return ItemLinks.Add(item, info); }
		void BarLinksHolder.BeginUpdate() { }
		void BarLinksHolder.EndUpdate() { }
		void BarLinksHolder.ClearLinks() { ItemLinks.Clear(); }
		BarItemLink BarLinksHolder.InsertItem(BarItemLink beforeLink, BarItem item) { return ItemLinks.Insert(beforeLink, item); }
		void BarLinksHolder.RemoveLink(BarItemLink link) { ItemLinks.Remove(link); }
		bool IVisualEffectsHolder.VisualEffectsVisible { get { return Ribbon != null && Ribbon.Visible; } }
		DevExpress.Utils.VisualEffects.ISupportAdornerUIManager IVisualEffectsHolder.VisualEffectsOwner { get { return Ribbon; } }
	}
	public delegate void LinksChangedEventHandler(CollectionChangeEventArgs e);
	[System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraBars.Design.Serialization.RibbonItemLinksSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public abstract class BaseRibbonItemLinkCollection : BarItemLinkCollection {
		public abstract RibbonControl Ribbon { get; }
		protected override BarManager Manager { get { return Ribbon == null ? null : Ribbon.Manager; } }
		protected override bool OnInsert(int index, object item) {
			bool res = base.OnInsert(index, item);
			if(!res) return false;
			BarItemLink link = item as BarItemLink;
			if(link != null && link.Item != null) {
				if(link.Item.Manager == null) link.Item.Manager = Manager;
			}
			return true;
		}
		protected virtual bool BaseIsMergedState { get { return base.IsMergedState; } }
		public void Remove(BarItem item) {
			foreach(BarItemLink link in InnerList) {
				if(link.Item == item) {
					Remove(link);
					break;
				}
			}
		}
		public override void Clear() {
			base.ClearItems();
		}
		protected internal override object Owner { get { return this; } }
	}
	public class RibbonQuickToolbarItemLinkCollection : BaseRibbonItemLinkCollection, IHasRibbonKeyTipManager, IList {
		RibbonQuickAccessToolbar toolbar;
		public RibbonQuickToolbarItemLinkCollection(RibbonQuickAccessToolbar toolbar) {
			this.toolbar = toolbar;
		}
		public RibbonQuickAccessToolbar Toolbar { get { return toolbar; } }
		public override RibbonControl Ribbon { get { return Toolbar.Ribbon; } }
		protected override void OnRemoveComplete(int position, object item) {
			BarItemLink link = (BarItemLink)item;
			if(link.IsMerged && Ribbon.MergedRibbon != null)
				Ribbon.MergedRibbon.Toolbar.ItemLinks.Remove(link.Item);
			base.OnRemoveComplete(position, item);
		}
		protected override void OnInsertComplete(int position, object item) {
			base.OnInsertComplete(position, item);
			BarItemLink link = (BarItemLink)item;
			if(!link.IsMerged || Ribbon.MergedRibbon == null)
				return;
			if(!ShouldAddMergedLink(link))
				return;
			Ribbon.MergedRibbon.Toolbar.ItemLinks.Add(link.Item);
		}
		protected virtual bool ShouldAddMergedLink(BarItemLink link) {
			foreach(BarItemLink tlink in Ribbon.MergedRibbon.Toolbar.ItemLinks) {
				if(tlink.Item == link.Item)
					return false;
			}
			return true;
		}
		protected override void OnCollectionChanged(CollectionChangeEventArgs e) {
			base.OnCollectionChanged(e);
			Toolbar.OnLinksChanged(e);
			if(Toolbar.Accessible != null)
				Toolbar.Accessible.UpdateCollection();
			if(Toolbar.Ribbon is RibbonPopupToolbarControl) {
				((RibbonPopupToolbarControl)(Toolbar.Ribbon)).SourceToolbar.ItemLinks.OnCollectionChanged(e);
			}
		}
		RibbonBaseKeyTipManager IHasRibbonKeyTipManager.KeyTipManager { get { return Ribbon.KeyTipManager as RibbonBaseKeyTipManager; } }
		protected override bool GetVisulEffectsVisible() {
			IVisualEffectsHolder holder = Toolbar as IVisualEffectsHolder;
			return holder != null ? holder.VisualEffectsVisible : false;
		}
		protected override DevExpress.Utils.VisualEffects.ISupportAdornerUIManager GetVisualEffectsOwner() {
			IVisualEffectsHolder holder = Toolbar as IVisualEffectsHolder;
			return holder != null ? holder.VisualEffectsOwner : null;
		}
	}
}
