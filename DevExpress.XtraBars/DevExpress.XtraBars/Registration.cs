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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Paint;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Customization;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Objects;
namespace DevExpress.XtraBars.Styles {
	public class BarClassInfo {
		string name;
		bool designTimeVisible;
		Type itemType, viewInfoType;
		CustomPainter painter;
		BarClassInfoCollection collection;
		public BarClassInfo(string name, Type itemType, Type viewInfoType, CustomPainter painter, bool designTimeVisible) {
			this.collection = null;
			this.name = name;
			this.itemType = itemType;
			this.viewInfoType = viewInfoType;
			this.painter = painter;
			this.designTimeVisible = designTimeVisible;
			ExtractConstructors();
		}
		public BarClassInfo(string name, Type itemType, Type viewInfoType, CustomPainter painter) : this(name, itemType, viewInfoType, painter, true) {
		}
		protected virtual void ExtractConstructors() {
		}
		public virtual BarClassInfoCollection Collection { 
			get { return collection; } 
			set { if(collection == null) collection = value;
			}
		}
		public string Name { get { return name; } }
		public bool DesignTimeVisible { get { return designTimeVisible; } }
		public Type ItemType { get { return itemType; } }
		public CustomPainter Painter { get { return painter; } }
		public Type ViewInfoType { get { return viewInfoType; } }
		public override string ToString() { return Name; }
	}
	public class BarClassInfoCollection : CollectionBase {
		Hashtable nameHash, itemTypeHash;
		BarManagerPaintStyle paintStyle;
		public BarClassInfoCollection(BarManagerPaintStyle paintStyle) {
			this.paintStyle = paintStyle;
			this.nameHash = new Hashtable();
			this.itemTypeHash = new Hashtable();
		}
		public virtual BarManagerPaintStyle PaintStyle { get { return paintStyle; } }
		protected virtual Hashtable NameHash { get { return nameHash; } }
		protected virtual Hashtable ItemTypeHash { get { return itemTypeHash; } }
		public virtual void Add(BarClassInfo itemInfo) {
			List.Add(itemInfo);
		}
		public virtual void Remove(BarClassInfo itemInfo) {
			List.Remove(itemInfo);
		}
		public virtual bool Contains(string name) { return NameHash[name] != null; }
		protected override void OnInsert(int index, object val) {
			BarClassInfo itemInfo = val as BarClassInfo;
			if(itemInfo == null || itemInfo.Collection != null || Contains(itemInfo.Name)) return;
			OnRealInsert(index, itemInfo);
			base.OnInsert(index, val);
		}
		protected virtual void OnRealInsert(int index, BarClassInfo classInfo) {
			NameHash[classInfo.Name] = classInfo;
			ItemTypeHash[classInfo.ItemType] = classInfo;
			classInfo.Collection = this;
		}
		protected virtual void OnRealRemove(int index, BarClassInfo classInfo) {
			NameHash.Remove(classInfo.Name);
			ItemTypeHash.Remove(classInfo.ItemType);
		}
		protected override void OnRemoveComplete(int index, object val) {
			BarClassInfo classInfo = val as BarClassInfo;
			if(classInfo != null) OnRealRemove(index, classInfo);
			base.OnRemoveComplete(index, val);
		}
		protected override void OnClearComplete() {
			NameHash.Clear();
			ItemTypeHash.Clear();
		}
	}
	public class BarCoreItemInfo : BarItemInfo {
		public BarCoreItemInfo(string name, Type itemType, Type linkType, BarItemInfo baseInfo, bool designTimeVisible, bool supportRibbon)
			: base(name, name, baseInfo.ImageIndex, itemType, linkType, baseInfo.ViewInfoType, baseInfo.LinkPainter, designTimeVisible, supportRibbon) {
		}
		public BarCoreItemInfo(string name, int imageIndex, Type itemType, Type linkType, BarItemInfo baseInfo, bool designTimeVisible, bool supportRibbon)
			: base(name, name, imageIndex, itemType, linkType, baseInfo.ViewInfoType, baseInfo.LinkPainter, designTimeVisible, supportRibbon) {
		}
		public BarCoreItemInfo(string name, string caption, Type itemType, Type linkType, BarItemInfo baseInfo, bool designTimeVisible, bool supportRibbon)
			: base(name, caption, baseInfo.ImageIndex, itemType, linkType, baseInfo.ViewInfoType, baseInfo.LinkPainter, designTimeVisible, supportRibbon) {
		}
		public BarCoreItemInfo(string name, string caption, int imageIndex, Type itemType, Type linkType, BarItemInfo baseInfo, bool designTimeVisible, bool supportRibbon)
			: base(name, caption, imageIndex, itemType, linkType, baseInfo.ViewInfoType, baseInfo.LinkPainter, designTimeVisible, supportRibbon) {
		}
	}
	public class BarInternalItemInfo : BarItemInfo {
		public BarInternalItemInfo(string name, Type itemType, Type linkType, Type viewInfoType, BarLinkPainter linkPainter) :
			base(name, string.Empty, -1, itemType, linkType, viewInfoType, linkPainter, false, false) { }
	}
	public class BarItemInfo : BarClassInfo {
		Type linkType;
		ConstructorInfo linkConstructor;
		ConstructorInfo LinkConstructor {
			get {
				if(linkConstructor == null)
					linkConstructor = LinkType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { typeof(BarItemLinkReadOnlyCollection), typeof(BarItem), typeof(object) }, null);
				return linkConstructor;
			}
		}
		ConstructorInfo viewInfoConstructor;
		ConstructorInfo ViewInfoConstructor {
			get {
				if(viewInfoConstructor == null)
					viewInfoConstructor = ViewInfoType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { typeof(BarDrawParameters), typeof(BarItemLink) }, null);
				return viewInfoConstructor;
			}
		}
		int imageIndex = -1;
		string caption;
		bool supportRibbon;
		public BarItemInfo(string name, string caption, int imageIndex, Type itemType, Type linkType, Type viewInfoType, BarLinkPainter linkPainter, bool designTimeVisible, bool supportRibbon) 
			: base(name, itemType, viewInfoType, linkPainter, designTimeVisible) {
			this.caption = caption;
			this.imageIndex = imageIndex;
			this.supportRibbon = supportRibbon;
			this.viewInfoConstructor = this.linkConstructor = null;
			this.linkType = linkType;
			ExtractConstructors();
		}
		public BarItem CreateItem() {
			return Activator.CreateInstance(ItemType) as BarItem;
		}
		public new virtual BarItemInfoCollection Collection { 
			get { return base.Collection as BarItemInfoCollection; } 
			set { base.Collection = value;
			}
		}
		public string GetCaption() {
			if(Caption == "") return Name;
			return string.Format("{0} ({1})", Caption, Name);
		}
		public string GetShortCaption() {
			if(Caption == "") return Name;
			return Caption;
		}
		public string Caption { get { return caption; } }
		public virtual int ImageIndex { get { return imageIndex; } }
		public Type LinkType { get { return linkType; } }
		public BarLinkPainter LinkPainter { get { return Painter as BarLinkPainter; } }
		public BarItemLink CreateLink(BarItemLinkReadOnlyCollection links, BarItem item, object linkedObject) {
			return LinkConstructor.Invoke(new object[] { links, item, linkedObject}) as BarItemLink;
		}
		public BarLinkViewInfo CreateViewInfo(BarItemLink link) {
			return ViewInfoConstructor.Invoke(new object[] { Collection.PaintStyle.DrawParameters, link}) as BarLinkViewInfo;
		}
		public bool SupportRibbon { get { return supportRibbon; } }
	}
	public class BarItemInfoCollection : BarClassInfoCollection {
		Hashtable linkTypeHash;
		public BarItemInfoCollection(BarManagerPaintStyle paintStyle) : base(paintStyle) {
			this.linkTypeHash = new Hashtable();
		}
		protected virtual Hashtable LinkTypeHash { get { return linkTypeHash; } }
		public virtual BarItemInfo this[int index] {
			get { return List[index] as BarItemInfo; } 
		}
		public virtual BarItemInfo this[string name] {
			get { return NameHash[name] as BarItemInfo; } 
		}
		public virtual BarItemInfo this[Type linkType] {
			get { 
				while(linkType != null) {
					BarItemInfo bi = LinkTypeHash[linkType] as BarItemInfo;
					if(bi != null) return bi;
					linkType = linkType.BaseType;
				}
				return null;
			} 
		}
		public virtual BarItemInfo GetInfoByItem(Type itemType) {
			while(itemType != null) {
				BarItemInfo bi = ItemTypeHash[itemType] as BarItemInfo;
				if(bi != null) return bi;
				itemType = itemType.BaseType;
			}
			return null;
		}
		protected override void OnRealInsert(int index, BarClassInfo classInfo) {
			BarItemInfo itemInfo = classInfo as BarItemInfo;
			if(itemInfo == null) return;
			LinkTypeHash[itemInfo.LinkType] = itemInfo;
			base.OnRealInsert(index, classInfo);
		}		
		protected override void OnRealRemove(int index, BarClassInfo classInfo) {
			BarItemInfo itemInfo = classInfo as BarItemInfo;
			if(itemInfo == null) return;
			LinkTypeHash.Remove(itemInfo.LinkType);
			base.OnRealRemove(index, classInfo);
		}
		protected override void OnClearComplete() {
			LinkTypeHash.Clear();
			base.OnClearComplete();
		}
		public void AddCollection(BarItemInfoCollection coll) {
			foreach(BarItemInfo info in coll)
				Add(info);
		}
	}
	public class BarCoreControlInfo : BarControlInfo {
		public BarCoreControlInfo(string name, Type itemType, Type viewInfoType, BarControlInfo info)
			: base(name, itemType, viewInfoType, info.Painter) {
		}
		public BarCoreControlInfo(string name, Type itemType, BarControlInfo info)
			: base(name, itemType, info.ViewInfoType, info.Painter) {
		}
	}
	public class BarControlInfo : BarClassInfo {
		ConstructorInfo viewInfoConstructor;
		ConstructorInfo ViewInfoConstructor {
			get {
				if(viewInfoConstructor == null) {
					viewInfoConstructor = ViewInfoType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { typeof(BarManager), typeof(BarDrawParameters), typeof(CustomControl) }, null);
					if(viewInfoConstructor == null)
						viewInfoConstructor = ViewInfoType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic, null, new Type[] { typeof(BarManager), typeof(BarDrawParameters), typeof(DevExpress.XtraBars.Forms.ControlForm) }, null);
				}
				return viewInfoConstructor;
			}
		}
		public BarControlInfo(string name, Type itemType, Type viewInfoType, CustomPainter customPainter)
			: base(name, itemType, viewInfoType, customPainter, false) {
			this.viewInfoConstructor = null;
			ExtractConstructors();
		}
		protected override void ExtractConstructors() {
			base.ExtractConstructors();
			if(ViewInfoType == null) return;
		}
		public new virtual BarControlInfoCollection Collection { 
			get { return base.Collection as BarControlInfoCollection; } 
			set { base.Collection = value;
			}
		}
		public CustomViewInfo CreateViewInfo(BarManager manager, Control control) {
			return ViewInfoConstructor.Invoke(new object[] { manager, Collection.PaintStyle.DrawParameters, control}) as CustomViewInfo;
		}
	}
	public class BarControlInfoCollection : BarClassInfoCollection {
		public BarControlInfoCollection(BarManagerPaintStyle paintStyle) : base(paintStyle) {
		}
		public virtual BarControlInfo this[int index] { get { return List[index] as BarControlInfo; } }
		public virtual BarControlInfo this[string name] { get { return NameHash[name] as BarControlInfo; } }
		public virtual BarControlInfo GetInfoByItem(Type itemType) { 
			while(itemType != null) {
				BarControlInfo bi = ItemTypeHash[itemType] as BarControlInfo;
				if(bi != null) return bi;
				itemType = itemType.BaseType;
			}
			return null;
		}
		protected override void OnRealInsert(int index, BarClassInfo classInfo) {
			BarControlInfo controlInfo = classInfo as BarControlInfo;
			if(controlInfo == null) return;
			base.OnRealInsert(index, controlInfo);
		}		
		protected override void OnRealRemove(int index, BarClassInfo classInfo) {
			BarControlInfo controlInfo = classInfo as BarControlInfo;
			if(controlInfo == null) return;
			base.OnRemoveComplete(index, classInfo);
		}
	}
	public class BarEmbeddedLookAndFeel : UserLookAndFeel {
		public BarEmbeddedLookAndFeel() : base(null) { }
		protected override bool AllowParentEvents { get { return false; } }
	}
	delegate BarManagerPaintStyle CreateBarManagerPaintStyle(BarManagerPaintStyleCollection paintStyles);
	class FakeBarManagerPaintStyle : BarManagerPaintStyle {
		CreateBarManagerPaintStyle create;
		public BarManagerPaintStyle Create(BarManagerPaintStyleCollection paintStyles) {
			return create(paintStyles);
		}
		public FakeBarManagerPaintStyle(CreateBarManagerPaintStyle create, string name) : base(null, name) {
			this.create = create;
		}
		public override BarButtonLinkPainter CreateButtonItemLinkPainter() {
			throw new InvalidOperationException();
		}
		protected override void RegisterItemInfo() {
			throw new InvalidOperationException();
		}
		protected override void RegisterBarInfo() {
			throw new InvalidOperationException();
		}
		protected override void CreatePainters() {
			throw new InvalidOperationException();
		}
	}
	public abstract class BarManagerPaintStyle : IDisposable {
		string name;
		BarItemInfoCollection itemInfoCollection;
		BarControlInfoCollection barInfoCollection;
		BarManagerPaintStyleCollection collection;
		UserLookAndFeel linksLookAndFeel, customizationLookAndFeel;
		protected BarDrawParameters fDrawParameters;
		protected PrimitivesPainter fPrimitivesPainter;
		protected DockElementsParameters fElementsParameters;
		protected DockElementsPainter fElementsPainter;
		public BarManagerPaintStyle(BarManagerPaintStyleCollection collection, string name) {
			this.name = name;
			this.collection = collection;
			this.itemInfoCollection = new BarItemInfoCollection(this);
			this.barInfoCollection = new BarControlInfoCollection(this);
		}
		public virtual bool CanAnimate { get { return false; } }
		bool initialized;
		public virtual void Initialize() {
			if(initialized)
				return;
			CreatePainters();
			RegisterItemInfo();
			RegisterBarInfo();
			RegisterCoreItems();
			initialized = true;
		}
		public abstract BarButtonLinkPainter CreateButtonItemLinkPainter();
		protected virtual void RegisterCoreItems() {
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonGalleryBarItem", "Image Gallery", 11, typeof(RibbonGalleryBarItem), typeof(RibbonGalleryBarItemLink), ItemInfoCollection["BarButtonItem"], false, true));
			ItemInfoCollection.Add(new BarCoreItemInfo("BarInListItem", typeof(BarInListItem), typeof(BarInListItemLink), ItemInfoCollection["BarButtonItem"], false, false));
			ItemInfoCollection.Add(new BarCoreItemInfo("BarButtonGroup", "Button Group", 10, typeof(BarButtonGroup), typeof(BarButtonGroupLink), ItemInfoCollection["BarSubItem"], false, true));
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonQuickToolbarBarItem", typeof(RibbonQuickToolbarBarItem), typeof(RibbonQuickToolbarBarItemLink), ItemInfoCollection["BarButtonItem"], false, false));
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonApplicationButtonItem", typeof(RibbonApplicationButtonItem), typeof(RibbonApplicationButtonItemLink), ItemInfoCollection["BarButtonItem"], false, false));
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonQuickToolbarCustomizeItem", typeof(RibbonQuickToolbarCustomizeItem), typeof(RibbonQuickToolbarCustomizeItemLink), ItemInfoCollection["BarListItem"], false, false));
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonGroupItem", typeof(RibbonGroupItem), typeof(RibbonPageGroupItemLink), ItemInfoCollection["BarLargeButtonItem"], false, false));
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonToolbarPopupItem", typeof(RibbonToolbarPopupItem), typeof(RibbonToolbarPopupItemLink), ItemInfoCollection["BarButtonItem"], false, false));
			ItemInfoCollection.Add(new BarCoreItemInfo("RibbonTouchMouseModeItem", typeof(RibbonTouchMouseModeItem), typeof(RibbonTouchMouseModeItemLink), ItemInfoCollection["BarButtonItem"], false, false));
			BarInfoCollection.Add(new BarCoreControlInfo("BarItemLinksControl", typeof(BarItemLinksControl), typeof(BarItemLinksControlViewInfo), BarInfoCollection["SubMenuControl"]));
		}
		protected abstract void RegisterItemInfo();
		protected abstract void RegisterBarInfo();
		protected abstract void CreatePainters();
		public virtual void Dispose() {
			if(this.linksLookAndFeel != null) {
				this.linksLookAndFeel.Dispose();
			}
			if(this.customizationLookAndFeel != null)
				this.customizationLookAndFeel.Dispose();
		}
		public virtual bool AllowLinkLighting { get { return true; } }
		protected virtual UserLookAndFeel CreateLookAndFeel() {
			BarEmbeddedLookAndFeel lf = new BarEmbeddedLookAndFeel();
			lf.SetStyle(LookAndFeelStyle.UltraFlat, false, false);
			return lf;
		}
		protected virtual UserLookAndFeel CreateCustomizationLookAndFeel() {
			BarEmbeddedLookAndFeel lf = new BarEmbeddedLookAndFeel();
			lf.SetStyle(LookAndFeelStyle.Flat, true, false);
			return lf;
		}
		public virtual UserLookAndFeel LinksLookAndFeel {
			get {
				if(linksLookAndFeel == null)
					linksLookAndFeel = CreateLookAndFeel();
				return linksLookAndFeel;
			}
		}
		public virtual UserLookAndFeel CustomizationLookAndFeel {
			get {
				if(customizationLookAndFeel == null)
					customizationLookAndFeel = CreateCustomizationLookAndFeel();
				return customizationLookAndFeel;
			}
		}
		public virtual string AlternatePaintStyleName { get { return ""; } }
		public virtual bool IsAllowUse { get { return true; } }
		public virtual BarItemInfoCollection ItemInfoCollection { get { return itemInfoCollection; } }
		public virtual BarControlInfoCollection BarInfoCollection { get { return barInfoCollection; } }
		public virtual BarManagerPaintStyleCollection Collection { get { return collection; } }
		public virtual string Name { get { return name; } }
		public BarAndDockingController Controller { get { return Collection.Controller; } }
		public virtual BarDrawParameters DrawParameters { get { return fDrawParameters; } }
		public virtual PrimitivesPainter PrimitivesPainter { get { return fPrimitivesPainter; } }
		public virtual DockElementsPainter ElementsPainter { get { return fElementsPainter; } }
		public virtual DockElementsParameters ElementsParameters { get { return fElementsParameters; } }
		protected internal virtual int CalcLinkIndent(BarLinkViewInfo linkInfo, BarIndent linkIndent) {
			if(linkInfo.IsLinkInMenu) {
				switch(linkIndent) {
					case BarIndent.Left: return 7;
					case BarIndent.Right: return DrawParameters.Constants.SubMenuHorzRightIndent;
					case BarIndent.Top: return DrawParameters.Constants.SubMenuItemTopIndent;
					case BarIndent.Bottom: return DrawParameters.Constants.SubMenuItemBottomIndent;
				}
				return 0;
			}
			switch(linkIndent) {
				case BarIndent.Left:
				case BarIndent.Right: return linkInfo.Link.GetLinkHorzIndent();
				case BarIndent.Top: 
				case BarIndent.Bottom: return linkInfo.Link.GetLinkVertIndent();
			}
			return 0;
		}
		protected internal virtual Size CalcMenuNavigationHeaderSize(Graphics graphics, Size contentSize) {
			return contentSize;
		}
		protected internal virtual Rectangle CalcNavigationHeaderContentBounds(Graphics graphics, Rectangle rect) {
			return rect;
		}
		protected internal virtual StateAppearances GetNavigationHeaderAppearanceDefault() {
			AppearanceDefault def = new AppearanceDefault(Color.Black, Color.Transparent);
			def.Font = new Font(AppearanceObject.DefaultFont.FontFamily, 10.0f);
			StateAppearances app = new StateAppearances(def);
			return app;
		}
		protected internal virtual int CalcEditLinkWidth(int editLinkWidth) {
			return editLinkWidth;
		}
		protected internal virtual Size CalcNavigationBackArrowSize(Graphics graphics) {
			return new Size(16, 16);
		}
		protected internal virtual int CalcBarCustomizationLinkMinHeight(Graphics g, object sourceObject, bool isVertical, bool isMainMenu) {
			return 0;
		}
		protected internal virtual Image GetCheckGlyph() {
			return null;
		}
	}
	public class BarManagerPaintStyleCollection : CollectionBase {
		BarAndDockingController controller;
		public BarManagerPaintStyleCollection(BarAndDockingController controller) {
			this.controller = controller;
		}
		public BarAndDockingController Controller { get { return controller; } }
		public virtual BarManagerPaintStyle this[string name] { 
			get { 
				int index = IndexOf(name);
				if(index >= 0) {
					return this[index];
				}
				return null;
			}
		}
		public virtual BarManagerPaintStyle this[int index] {
			get {
				BarManagerPaintStyle style = (BarManagerPaintStyle)List[index];
				FakeBarManagerPaintStyle fake = style as FakeBarManagerPaintStyle;
				if(fake != null) {
					style = fake.Create(this);
					List[index] = style;
				}
				style.Initialize();
				return style;
			}
		}
		public virtual void Add(BarManagerPaintStyle paintStyle) {
			List.Add(paintStyle);
		}
		public virtual void Remove(BarManagerPaintStyle paintStyle) {
			List.Remove(paintStyle);
		}
		protected override void OnInsert(int index, object val) {
			BarManagerPaintStyle paintStyle = val as BarManagerPaintStyle;
			if(Contains(paintStyle.Name)) return;
			base.OnInsert(index, val);
		}	
		public virtual int IndexOf(string name) {
			for(int n = 0; n < Count; n++) {
				BarManagerPaintStyle style = (BarManagerPaintStyle)List[n];
				if(style.Name == name) return n;
			}
			return -1;
		}
		public virtual bool Contains(string name) {
			foreach(BarManagerPaintStyle style in this) {
				if(style.Name == name) return true;
			}
			return false;
		}
	}
	public class BarManagerOfficeXPPaintStyle : BarManagerPaintStyle {
		public BarManagerOfficeXPPaintStyle(BarManagerPaintStyleCollection collection) : base(collection, "OfficeXP") {
		}
		protected override void CreatePainters() {
			this.fDrawParameters = new BarOfficeXPDrawParameters(this);
			this.fPrimitivesPainter = new OfficeXPPrimitivesPainter(this);
			this.fElementsPainter = new DockElementsOfficeXPPainter(this);
			this.fElementsParameters = new DockElementsOfficeXPParameters();
		}
		protected override void RegisterBarInfo() {
			BarInfoCollection.Add(new BarControlInfo("BarDockControl", typeof(BarDockControl), typeof(DockControlViewInfo), new BarDockControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("BarControl", typeof(CustomControl), typeof(BarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("DockedBarControl", typeof(DockedBarControl), typeof(DockedBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControl", typeof(SubMenuBarControl), typeof(SubMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupMenuControl", typeof(PopupMenuBarControl), typeof(PopupMenuBarControlViewInfo), new PopupControlContainerBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControl", typeof(PopupContainerBarControl), typeof(PopupContainerControlViewInfo), new PopupControlContainerBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("QuickCustomizationBarControl", typeof(QuickCustomizationBarControl), typeof(QuickCustomizationBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControl", typeof(FloatingBarControl), typeof(FloatingBarControlViewInfo), new FloatingBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("TitleBarControl", typeof(DevExpress.XtraBars.Objects.TitleBarControl), typeof(DevExpress.XtraBars.Objects.TitleBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("ControlForm", typeof(DevExpress.XtraBars.Forms.ControlForm), typeof(ControlFormViewInfo), new ControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControlForm", typeof(DevExpress.XtraBars.Forms.SubMenuControlForm), typeof(ControlFormViewInfo), new ControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControlForm", typeof(DevExpress.XtraBars.Forms.PopupContainerForm), typeof(PopupContainerFormViewInfo), new ControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControlForm", typeof(DevExpress.XtraBars.Forms.FloatingBarControlForm), typeof(FloatingBarControlFormViewInfo), new FloatingBarControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("GalleryDropDownBarControl", typeof(GalleryDropDownBarControl), typeof(GalleryDropDownControlViewInfo), new NonSkinGalleryDropDownBarControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuBarControl", typeof(AppMenuBarControl), typeof(AppMenuControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuForm", typeof(DevExpress.XtraBars.Forms.AppMenuForm), typeof(AppMenuFormViewInfo), new SkinAppMenuFormPainter(this)));
		}
		public override BarButtonLinkPainter CreateButtonItemLinkPainter() { return new BarButtonLinkPainter(this); }
		protected override void RegisterItemInfo() {
			ItemInfoCollection.Add(new BarItemInfo("BarButtonItem", "Button", 0, typeof(BarButtonItem), typeof(BarButtonItemLink), typeof(BarButtonLinkViewInfo), CreateButtonItemLinkPainter(), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarCheckItem", "Check", 9, typeof(BarCheckItem), typeof(BarCheckItemLink), typeof(BarCheckButtonLinkViewInfo), new BarCheckButtonLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToggleSwitchItem", "Toggle Switch", 9, typeof(BarToggleSwitchItem), typeof(BarToggleSwitchItemLink), typeof(BarToggleSwitchLinkViewInfo), new BarToggleSwitchLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarSubItem", "Menu", 1, typeof(BarSubItem), typeof(BarSubItemLink), typeof(BarSubItemLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarStaticItem", "Static Text", 2, typeof(BarStaticItem), typeof(BarStaticItemLink), typeof(BarStaticLinkViewInfo), new BarStaticLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarHeaderItem", "Header Text", 2, typeof(BarHeaderItem), typeof(BarHeaderItemLink), typeof(BarHeaderLinkViewInfo), new BarHeaderLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarEditItem", "Editor", 3, typeof(BarEditItem), typeof(BarEditItemLink), typeof(BarEditLinkViewInfo), new BarEditLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerItem", "Link Container", 4, typeof(BarLinkContainerItem), typeof(BarLinkContainerItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarLargeButtonItem", "Large Button", 5, typeof(BarLargeButtonItem), typeof(BarLargeButtonItemLink), typeof(BarLargeButtonLinkViewInfo), new BarLargeButtonLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarListItem", "String List", 6, typeof(BarListItem), typeof(BarListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarMdiChildrenListItem", "Mdi Children List", 7, typeof(BarMdiChildrenListItem), typeof(BarMdiChildrenListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarDockingMenuItem", "Docking Menu", 13, typeof(BarDockingMenuItem), typeof(BarDockingMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarWorkspaceMenuItem", "Workspace Menu", 14, typeof(BarWorkspaceMenuItem), typeof(BarWorkspaceMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToolbarsListItem", "Toolbar List", 8, typeof(BarToolbarsListItem), typeof(BarToolbarsListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerExItem", "LinkContainerEx", 4, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), false, false));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuAddRemoveButtonsItem", typeof(BarQMenuAddRemoveButtonsItem), typeof(BarQMenuAddRemoveButtonsItemLink), typeof(BarQMenuAddRemoveButtonsLinkViewInfo), new BarQMenuAddRemoveButtonsLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQBarCustomization", typeof(BarQBarCustomizationItem), typeof(BarQBarCustomizationItemLink), typeof(BarQBarCustomizationLinkViewInfo), new BarQBarCustomizationLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuCustomization", typeof(BarQMenuCustomizationItem), typeof(BarQMenuCustomizationItemLink), typeof(BarQMenuCustomizationLinkViewInfo), new BarQMenuCustomizationLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarRecentExpander", typeof(BarRecentExpanderItem), typeof(BarRecentExpanderItemLink), typeof(BarRecentExpanderItemLinkViewInfo), new BarRecentExpanderLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarEmptyItem", typeof(BarEmptyItem), typeof(BarEmptyItemLink), typeof(BarEmptyLinkViewInfo), new BarEmptyLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarMdiButtonItem", typeof(BarMdiButtonItem), typeof(BarMdiButtonItemLink), typeof(BarMdiButtonLinkViewInfo), new BarMdiButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarSystemMenuItem", typeof(BarSystemMenuItem), typeof(BarSystemMenuItemLink), typeof(BarSystemMenuLinkViewInfo), new BarCustomContainerLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarScrollItem", typeof(BarScrollItem), typeof(BarScrollItemLink), typeof(BarScrollItemLinkViewInfo), new BarScrollLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarDesignTimeItem", typeof(BarDesignTimeItem), typeof(BarDesignTimeItemLink), typeof(BarDesignTimeLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarCloseItem", typeof(BarCloseItem), typeof(BarCloseItemLink), typeof(BarCloseLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarInMdiChildrenListItem", typeof(BarInMdiChildrenListItem), typeof(BarInMdiChildrenListItemLink), typeof(BarInMdiChildrenListButtonLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("RibbonExpandCollapseItem", typeof(RibbonExpandCollapseItem), typeof(RibbonExpandCollapseItemLink), typeof(RibbonExpandCollapseItemLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("AutoHiddenPagesMenuItem", typeof(AutoHiddenPagesMenuItem), typeof(AutoHiddenPagesMenuItemLink), typeof(AutoHiddenPagesMenuItemLinkViewInfo), new BarButtonLinkPainter(this)));
		}
	}
	public class BarManagerOffice2000PaintStyle : BarManagerPaintStyle {
		public BarManagerOffice2000PaintStyle(BarManagerPaintStyleCollection collection) : base(collection, "Office2000") {	}
		protected override UserLookAndFeel CreateLookAndFeel() {
			BarEmbeddedLookAndFeel lf = new BarEmbeddedLookAndFeel();
			lf.UseDefaultLookAndFeel = false;
			lf.UseWindowsXPTheme = false;
			lf.Style = LookAndFeelStyle.Flat;
			return lf;
		}
		protected override void CreatePainters() {
			this.fDrawParameters = new BarOffice2000DrawParameters(this);
			this.fPrimitivesPainter = new PrimitivesPainterOffice2000(this);
			this.fElementsPainter = new DockElementsOffice2000Painter(this);
			this.fElementsParameters = new DockElementsOffice2000Parameters();
		}
		protected override void RegisterBarInfo() {
			BarInfoCollection.Add(new BarControlInfo("BarDockControl", typeof(BarDockControl), typeof(DockControlViewInfo), new BarDockControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("BarControl", typeof(CustomControl), typeof(BarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("DockedBarControl", typeof(DockedBarControl), typeof(DockedBarControlOffice2000ViewInfo), new BarOffice2000Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControl", typeof(SubMenuBarControl), typeof(SubMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupMenuControl", typeof(PopupMenuBarControl), typeof(PopupMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControl", typeof(PopupContainerBarControl), typeof(PopupContainerControlViewInfo), new PopupControlContainerBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("QuickCustomizationBarControl", typeof(QuickCustomizationBarControl), typeof(QuickCustomizationBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControl", typeof(FloatingBarControl), typeof(FloatingBarControlViewInfo), new FloatingBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("TitleBarControl", typeof(DevExpress.XtraBars.Objects.TitleBarControl), typeof(DevExpress.XtraBars.Objects.TitleBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("ControlForm", typeof(DevExpress.XtraBars.Forms.ControlForm), typeof(ControlFormViewInfo), new ControlFormOffice2000Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControlForm", typeof(DevExpress.XtraBars.Forms.SubMenuControlForm), typeof(ControlFormViewInfo), new ControlFormOffice2000Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControlForm", typeof(DevExpress.XtraBars.Forms.PopupContainerForm), typeof(PopupContainerFormViewInfo), new ControlFormOffice2000Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControlForm", typeof(DevExpress.XtraBars.Forms.FloatingBarControlForm), typeof(FloatingBarControlFormOffice2000ViewInfo), new FloatingBarControlFormOffice2000Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("GalleryDropDownBarControl", typeof(GalleryDropDownBarControl), typeof(GalleryDropDownControlViewInfo), new NonSkinGalleryDropDownBarControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuBarControl", typeof(AppMenuBarControl), typeof(AppMenuControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuForm", typeof(DevExpress.XtraBars.Forms.AppMenuForm), typeof(AppMenuFormViewInfo), new SkinAppMenuFormPainter(this)));
		}
		public override BarButtonLinkPainter CreateButtonItemLinkPainter() { return new BarButtonOffice2000LinkPainter(this); }
		protected override void RegisterItemInfo() {
			ItemInfoCollection.Add(new BarItemInfo("BarButtonItem", "Button", 0, typeof(BarButtonItem), typeof(BarButtonItemLink), typeof(BarButtonLinkViewInfo), CreateButtonItemLinkPainter(), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarCheckItem", "Check", 9, typeof(BarCheckItem), typeof(BarCheckItemLink), typeof(BarCheckButtonLinkViewInfo), new BarBaseButtonOffice2000LinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToggleSwitchItem", "Toggle Switch", 9, typeof(BarToggleSwitchItem), typeof(BarToggleSwitchItemLink), typeof(BarToggleSwitchLinkViewInfo), new BarToggleSwitchLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarSubItem", "Menu", 1, typeof(BarSubItem), typeof(BarSubItemLink), typeof(BarSubItemLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarStaticItem", "Static Text", 2, typeof(BarStaticItem), typeof(BarStaticItemLink), typeof(BarStaticLinkViewInfo), new BarStaticLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarHeaderItem", "Header Text", 2, typeof(BarHeaderItem), typeof(BarHeaderItemLink), typeof(BarHeaderLinkViewInfo), new BarHeaderLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarEditItem", "Editor", 3, typeof(BarEditItem), typeof(BarEditItemLink), typeof(BarEditLinkViewInfo), new BarEditOffice2000LinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarLargeButtonItem", "Large Button", 4, typeof(BarLargeButtonItem), typeof(BarLargeButtonItemLink), typeof(BarLargeButtonLinkViewInfo), new BarLargeButtonOffice2000LinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerItem", "Link Container", 5, typeof(BarLinkContainerItem), typeof(BarLinkContainerItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarListItem", "String List", 6, typeof(BarListItem), typeof(BarListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarMdiChildrenListItem", "Mdi Children List", 7, typeof(BarMdiChildrenListItem), typeof(BarMdiChildrenListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarDockingMenuItem", "Docking Menu", 13, typeof(BarDockingMenuItem), typeof(BarDockingMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarWorkspaceMenuItem", "Workspace Menu", 14, typeof(BarWorkspaceMenuItem), typeof(BarWorkspaceMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToolbarsListItem", "Toolbar list", 8, typeof(BarToolbarsListItem), typeof(BarToolbarsListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerExItem", "LinkContainerEx", 5, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), false, false));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuAddRemoveButtonsItem", typeof(BarQMenuAddRemoveButtonsItem), typeof(BarQMenuAddRemoveButtonsItemLink), typeof(BarQMenuAddRemoveButtonsLinkViewInfo), new BarQMenuAddRemoveButtonsOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQBarCustomization", typeof(BarQBarCustomizationItem), typeof(BarQBarCustomizationItemLink), typeof(BarQBarCustomizationLinkViewInfo), new BarQBarCustomizationLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuCustomization", typeof(BarQMenuCustomizationItem), typeof(BarQMenuCustomizationItemLink), typeof(BarQMenuCustomizationLinkViewInfo), new BarQMenuCustomizationOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarRecentExpander", typeof(BarRecentExpanderItem), typeof(BarRecentExpanderItemLink), typeof(BarRecentExpanderItemLinkViewInfo), new BarRecentExpanderOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarEmptyItem", typeof(BarEmptyItem), typeof(BarEmptyItemLink), typeof(BarEmptyLinkViewInfo), new BarEmptyLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarMdiButtonItem", typeof(BarMdiButtonItem), typeof(BarMdiButtonItemLink), typeof(BarMdiButtonLinkViewInfo), new BarMdiButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarSystemMenuItem", typeof(BarSystemMenuItem), typeof(BarSystemMenuItemLink), typeof(BarSystemMenuLinkViewInfo), new BarCustomContainerLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarScrollItem", typeof(BarScrollItem), typeof(BarScrollItemLink), typeof(BarScrollItemLinkViewInfo), new BarScrollLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarDesignTimeItem", typeof(BarDesignTimeItem), typeof(BarDesignTimeItemLink), typeof(BarDesignTimeLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarCloseItem", typeof(BarCloseItem), typeof(BarCloseItemLink), typeof(BarCloseLinkViewInfo), new BarButtonOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarInMdiChildrenListItem", typeof(BarInMdiChildrenListItem), typeof(BarInMdiChildrenListItemLink), typeof(BarInMdiChildrenListButtonLinkViewInfo), new BarButtonOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("RibbonExpandCollapseItem", typeof(RibbonExpandCollapseItem), typeof(RibbonExpandCollapseItemLink), typeof(RibbonExpandCollapseItemLinkViewInfo), new BarButtonOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("AutoHiddenPagesMenuItem", typeof(AutoHiddenPagesMenuItem), typeof(AutoHiddenPagesMenuItemLink), typeof(AutoHiddenPagesMenuItemLinkViewInfo), new BarButtonLinkPainter(this)));
		}
	}
	public class BarManagerWindowsXPPaintStyle : BarManagerPaintStyle {
		public BarManagerWindowsXPPaintStyle(BarManagerPaintStyleCollection collection) : base(collection, "WindowsXP") {	}
		protected override UserLookAndFeel CreateLookAndFeel() {
			BarEmbeddedLookAndFeel lf = new BarEmbeddedLookAndFeel();
			lf.UseDefaultLookAndFeel = false;
			lf.UseWindowsXPTheme = true;
			lf.Style = LookAndFeelStyle.Flat;
			return lf;
		}
		public override string AlternatePaintStyleName { get { return "Office2000"; } }
		public override bool IsAllowUse { 
			get { 
				return DevExpress.Utils.WXPaint.Painter.ThemesEnabled;
			} 
		}
		protected override void CreatePainters() {
			this.fDrawParameters = new BarWindowsXPDrawParameters(this);
			this.fPrimitivesPainter = new PrimitivesPainterWindowsXP(this);
			this.fElementsPainter = new DockElementsWindowsXPPainter(this);
			this.fElementsParameters = new DockElementsWindowsXPParameters();
		}
		protected override void RegisterBarInfo() {
			BarInfoCollection.Add(new BarControlInfo("BarDockControl", typeof(BarDockControl), typeof(DockControlWindowsXPViewInfo), new DockControlWindowsXPPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("BarControl", typeof(CustomControl), typeof(BarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("DockedBarControl", typeof(DockedBarControl), typeof(DockedBarControlWindowsXPViewInfo), new BarWindowsXPPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControl", typeof(SubMenuBarControl), typeof(SubMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupMenuControl", typeof(PopupMenuBarControl), typeof(PopupMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControl", typeof(PopupContainerBarControl), typeof(PopupContainerControlViewInfo), new PopupControlContainerBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("QuickCustomizationBarControl", typeof(QuickCustomizationBarControl), typeof(QuickCustomizationBarControlWindowsXPViewInfo), new QuickCustomizationBarWindowsXPPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControl", typeof(FloatingBarControl), typeof(FloatingBarControlWindowsXPViewInfo), new FloatingWindowsXPBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("TitleBarControl", typeof(DevExpress.XtraBars.Objects.TitleBarControl), typeof(DevExpress.XtraBars.Objects.TitleBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("ControlForm", typeof(DevExpress.XtraBars.Forms.ControlForm), typeof(ControlFormViewInfo), new ControlFormWindowsXPPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControlForm", typeof(DevExpress.XtraBars.Forms.SubMenuControlForm), typeof(ControlFormViewInfo), new ControlFormWindowsXPPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControlForm", typeof(DevExpress.XtraBars.Forms.PopupContainerForm), typeof(PopupContainerFormViewInfo), new ControlFormWindowsXPPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControlForm", typeof(DevExpress.XtraBars.Forms.FloatingBarControlForm), typeof(FloatingBarControlFormOffice2000ViewInfo), new FloatingBarControlFormOffice2000Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("GalleryDropDownBarControl", typeof(GalleryDropDownBarControl), typeof(GalleryDropDownControlViewInfo), new NonSkinGalleryDropDownBarControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuBarControl", typeof(AppMenuBarControl), typeof(AppMenuControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuForm", typeof(DevExpress.XtraBars.Forms.AppMenuForm), typeof(AppMenuFormViewInfo), new SkinAppMenuFormPainter(this)));
		}
		public override BarButtonLinkPainter CreateButtonItemLinkPainter() { return new BarButtonWindowsXPLinkPainter(this); }
		protected override void RegisterItemInfo() {
			ItemInfoCollection.Add(new BarItemInfo("BarButtonItem", "Button", 0, typeof(BarButtonItem), typeof(BarButtonItemLink), typeof(BarButtonLinkViewInfo), CreateButtonItemLinkPainter(), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarCheckItem", "Check", 9, typeof(BarCheckItem), typeof(BarCheckItemLink), typeof(BarCheckButtonLinkViewInfo), new BarBaseButtonWindowsXPLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToggleSwitchItem", "Toggle Switch", 9, typeof(BarToggleSwitchItem), typeof(BarToggleSwitchItemLink), typeof(BarToggleSwitchLinkViewInfo), new BarToggleSwitchLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarSubItem", "Menu", 1, typeof(BarSubItem), typeof(BarSubItemLink), typeof(BarSubItemLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarStaticItem", "Static Text", 2, typeof(BarStaticItem), typeof(BarStaticItemLink), typeof(BarStaticLinkViewInfo), new BarStaticLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarHeaderItem", "Header Text", 2, typeof(BarHeaderItem), typeof(BarHeaderItemLink), typeof(BarHeaderLinkViewInfo), new BarHeaderLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarEditItem", "Editor", 3, typeof(BarEditItem), typeof(BarEditItemLink), typeof(BarEditLinkViewInfo), new BarEditOffice2000LinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarLargeButtonItem", "Large Button", 4, typeof(BarLargeButtonItem), typeof(BarLargeButtonItemLink), typeof(BarLargeButtonLinkViewInfo), new BarLargeButtonWindowsXPLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerItem", "Link Container", 5, typeof(BarLinkContainerItem), typeof(BarLinkContainerItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarListItem", "String List", 6, typeof(BarListItem), typeof(BarListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarMdiChildrenListItem", "Mdi Children List", 7, typeof(BarMdiChildrenListItem), typeof(BarMdiChildrenListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarDockingMenuItem", "Docking Menu", 13, typeof(BarDockingMenuItem), typeof(BarDockingMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarWorkspaceMenuItem", "Workspace Menu", 14, typeof(BarWorkspaceMenuItem), typeof(BarWorkspaceMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToolbarsListItem", "Toolbar List", 8, typeof(BarToolbarsListItem), typeof(BarToolbarsListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerExItem", "LinkContainerEx", 5, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), false, false));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuAddRemoveButtonsItem", typeof(BarQMenuAddRemoveButtonsItem), typeof(BarQMenuAddRemoveButtonsItemLink), typeof(BarQMenuAddRemoveButtonsLinkViewInfo), new BarQMenuAddRemoveButtonsOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQBarCustomization", typeof(BarQBarCustomizationItem), typeof(BarQBarCustomizationItemLink), typeof(BarQBarCustomizationLinkViewInfo), new BarQBarCustomizationLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuCustomization", typeof(BarQMenuCustomizationItem), typeof(BarQMenuCustomizationItemLink), typeof(BarQMenuCustomizationLinkViewInfo), new BarQMenuCustomizationOffice2000LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarRecentExpander", typeof(BarRecentExpanderItem), typeof(BarRecentExpanderItemLink), typeof(BarRecentExpanderItemLinkViewInfo), new BarRecentExpanderLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarEmptyItem", typeof(BarEmptyItem), typeof(BarEmptyItemLink), typeof(BarEmptyLinkViewInfo), new BarEmptyLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarMdiButtonItem", typeof(BarMdiButtonItem), typeof(BarMdiButtonItemLink), typeof(BarMdiButtonLinkWindowsXPViewInfo), new BarMdiButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarSystemMenuItem", typeof(BarSystemMenuItem), typeof(BarSystemMenuItemLink), typeof(BarSystemMenuLinkViewInfo), new BarCustomContainerLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarScrollItem", typeof(BarScrollItem), typeof(BarScrollItemLink), typeof(BarScrollItemLinkViewInfo), new BarScrollLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarDesignTimeItem", typeof(BarDesignTimeItem), typeof(BarDesignTimeItemLink), typeof(BarDesignTimeLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarCloseItem", typeof(BarCloseItem), typeof(BarCloseItemLink), typeof(BarCloseLinkViewInfo), new BarButtonWindowsXPLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarInMdiChildrenListItem", typeof(BarInMdiChildrenListItem), typeof(BarInMdiChildrenListItemLink), typeof(BarInMdiChildrenListButtonLinkViewInfo), new BarButtonWindowsXPLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("RibbonExpandCollapseItem", typeof(RibbonExpandCollapseItem), typeof(RibbonExpandCollapseItemLink), typeof(RibbonExpandCollapseItemLinkViewInfo), new BarButtonWindowsXPLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("AutoHiddenPagesMenuItem", typeof(AutoHiddenPagesMenuItem), typeof(AutoHiddenPagesMenuItemLink), typeof(AutoHiddenPagesMenuItemLinkViewInfo), new BarButtonLinkPainter(this)));
		}
	}
	public class SkinBarEmbeddedLookAndFeel : BarEmbeddedLookAndFeel {
		DevExpress.Skins.ISkinProviderEx skinProvider;
		public SkinBarEmbeddedLookAndFeel(DevExpress.Skins.ISkinProviderEx skinProvider) {
			this.skinProvider = skinProvider;
		}
		public override string SkinName { get { return skinProvider.SkinName; } }
		public override bool GetTouchUI() {
			return skinProvider.GetTouchUI();
		}
		public override float GetTouchScaleFactor() {
			return skinProvider.GetTouchScaleFactor();
		}
		public override Color GetMaskColor() {
			return skinProvider.GetMaskColor();
		}
		public override Color GetMaskColor2() {
			return skinProvider.GetMaskColor2();
		}
	}
	public class SkinBarManagerPaintStyle : BarManagerOffice2003PaintStyle, DevExpress.Skins.ISkinProvider, ISkinProviderEx {
		public SkinBarManagerPaintStyle(BarManagerPaintStyleCollection collection) : base(collection, "Skin") {
		}
		string DevExpress.Skins.ISkinProvider.SkinName { get { return Controller == null ? Skin.Name : ((ISkinProvider)Controller.LookAndFeel).SkinName; } }
		bool ISkinProviderEx.GetTouchUI() {
			if(Controller == null)
				return false;
			return Controller.LookAndFeel.GetTouchUI();
		}
		protected internal override Image GetCheckGlyph() {
			return Skin[BarSkins.SkinPopupMenuCheck].Glyph.GetImages().Images[0];
		}
		protected internal override int CalcBarCustomizationLinkMinHeight(Graphics g, object sourceObject, bool isVertical, bool isMainMenu) {
			string elementName = isMainMenu? BarSkins.SkinMainMenuCustomize: BarSkins.SkinBarCustomize;
			SkinElement elem = BarSkins.GetSkin(this)[elementName];
			Rectangle res = ObjectPainter.CalcObjectMinBounds(g, SkinElementPainter.Default, new SkinElementInfo(elem));
			return isVertical ? res.Width : res.Height;
		}
		protected internal override Size CalcNavigationBackArrowSize(Graphics graphics) {
			SkinElement elem = RibbonSkins.GetSkin(this)[BarSkins.SkinNavigationHeaderArrowBack];
			return ObjectPainter.CalcObjectMinBounds(graphics, SkinElementPainter.Default, new SkinElementInfo(elem, Rectangle.Empty)).Size;
		}
		protected internal override int CalcEditLinkWidth(int editLinkWidth) {
			float scale = 1.4f + ((ISkinProviderEx)this).GetTouchScaleFactor() / 10.0f;
			if(((ISkinProviderEx)this).GetTouchUI())
				return (int)(editLinkWidth * scale);
			return editLinkWidth;
		}
		protected internal override StateAppearances GetNavigationHeaderAppearanceDefault() {
			SkinElement elem = RibbonSkins.GetSkin(this)[RibbonSkins.SkinPopupGalleryGroupCaption];
			AppearanceDefault defApp = new AppearanceDefault(Color.Black, Color.Empty);
			elem.Apply(defApp);
			StateAppearances res = new StateAppearances(defApp);
			res.Hovered.ForeColor = elem.GetForeColor(ObjectState.Hot);
			res.Pressed.ForeColor = elem.GetForeColor(ObjectState.Pressed);
			res.Disabled.ForeColor = elem.GetForeColor(ObjectState.Disabled);
			return res;
		}
		protected internal override Size CalcMenuNavigationHeaderSize(Graphics graphics, Size contentSize) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(this)[RibbonSkins.SkinPopupGalleryGroupCaption], Rectangle.Empty);
			return ObjectPainter.CalcBoundsByClientRectangle(graphics, SkinElementPainter.Default, info, new Rectangle(Point.Empty, contentSize)).Size;
		}
		protected internal override Rectangle CalcNavigationHeaderContentBounds(Graphics graphics, Rectangle rect) {
			SkinElementInfo info = new SkinElementInfo(RibbonSkins.GetSkin(this)[RibbonSkins.SkinPopupGalleryGroupCaption], rect);
			return ObjectPainter.GetObjectClientRectangle(graphics, SkinElementPainter.Default, info);
		}
		float ISkinProviderEx.GetTouchScaleFactor() {
			return Controller.LookAndFeel.GetTouchScaleFactor();
		}
		Color ISkinProviderEx.GetMaskColor() {
			return Controller.LookAndFeel.GetMaskColor();
		}
		Color ISkinProviderEx.GetMaskColor2() {
			return Controller.LookAndFeel.GetMaskColor2();
		}
		public DevExpress.Skins.Skin Skin { 
			get { 
				return DevExpress.Skins.BarSkins.GetSkin(this); 
			} 
		} 
		protected virtual Skin GetSkin(BarLinkViewInfo linkInfo) { 
			if(linkInfo.Link.Ribbon != null)
				return DevExpress.Skins.BarSkins.GetSkin(linkInfo.Link.Ribbon.ViewInfo.Provider);
			return Skin;
		}
		protected virtual Skin GetDockManagerSkin(BarLinkViewInfo linkInfo) {
			if(linkInfo.Link.Ribbon != null)
				return DevExpress.Skins.DockingSkins.GetSkin(linkInfo.Link.Ribbon.ViewInfo.Provider);
			return DevExpress.Skins.DockingSkins.GetSkin(this); ;
		}
		public override bool AllowLinkLighting { get { return false; } }
		public override bool CanAnimate { get { return true; } }
		protected override void RegisterBarInfo() {
			BarInfoCollection.Add(new BarControlInfo("BarDockControl", typeof(BarDockControl), typeof(DockControlOffice2003ViewInfo), new DockControlOffice2003Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("BarControl", typeof(CustomControl), typeof(BarControlViewInfo), new SkinBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("DockedBarControl", typeof(DockedBarControl), typeof(SkinDockedBarControlViewInfo), new SkinBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControl", typeof(SubMenuBarControl), typeof(SubMenuBarControlViewInfo), new SkinBarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupMenuControl", typeof(PopupMenuBarControl), typeof(PopupMenuBarControlViewInfo), new SkinBarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControl", typeof(PopupContainerBarControl), typeof(PopupContainerControlViewInfo), new SkinPopupControlContainerBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("QuickCustomizationBarControl", typeof(QuickCustomizationBarControl), typeof(QuickCustomizationBarControlViewInfo), new SkinBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControl", typeof(FloatingBarControl), typeof(SkinFloatingBarControlViewInfo), new SkinFloatingBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("TitleBarControl", typeof(DevExpress.XtraBars.Objects.TitleBarControl), typeof(DevExpress.XtraBars.Objects.TitleBarControlViewInfo), new SkinTitleBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("TabFormControl", typeof(TabFormControl), typeof(TabFormControlViewInfo), new TabFormControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("ControlForm", typeof(DevExpress.XtraBars.Forms.ControlForm), typeof(ControlFormViewInfo), new SkinControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControlForm", typeof(DevExpress.XtraBars.Forms.SubMenuControlForm), typeof(ControlFormViewInfo), new SkinControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControlForm", typeof(DevExpress.XtraBars.Forms.PopupContainerForm), typeof(PopupContainerFormViewInfo), new SkinControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControlForm", typeof(DevExpress.XtraBars.Forms.FloatingBarControlForm), typeof(SkinFloatingBarControlFormViewInfo), new SkinFloatingBarControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("GalleryDropDownBarControl", typeof(GalleryDropDownBarControl), typeof(GalleryDropDownControlViewInfo), new GalleyDropDownBarControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuBarControl", typeof(AppMenuBarControl), typeof(AppMenuControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuForm", typeof(DevExpress.XtraBars.Forms.AppMenuForm), typeof(AppMenuFormViewInfo), new SkinAppMenuFormPainter(this)));
		}
		protected override void RegisterCoreItems() {
			base.RegisterCoreItems();
			ItemInfoCollection.Add(new BarCoreItemInfo("SkinRibbonGalleryBarItem", "Skin Gallery", 12, typeof(SkinRibbonGalleryBarItem), typeof(SkinRibbonGalleryBarItemLink), ItemInfoCollection["BarButtonItem"], false, true));
		}
		protected override void RegisterItemInfo() {
			base.RegisterItemInfo();
			ItemInfoCollection.Remove(ItemInfoCollection["BarQBarCustomization"]);
			ItemInfoCollection.Remove(ItemInfoCollection["BarRecentExpander"]);
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQBarCustomization", typeof(BarQBarCustomizationItem), typeof(BarQBarCustomizationItemLink), typeof(BarQBarCustomizationLinkViewInfo), new BarQBarCustomizationSkinLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarRecentExpander", typeof(BarRecentExpanderItem), typeof(BarRecentExpanderItemLink), typeof(BarRecentExpanderItemLinkViewInfo), new SkinBarRecentExpanderLinkPainter(this)));
			ItemInfoCollection.Remove(ItemInfoCollection["BarMdiButtonItem"]);
			ItemInfoCollection.Add(new BarInternalItemInfo("BarMdiButtonItem", typeof(BarMdiButtonItem), typeof(BarMdiButtonItemLink), typeof(BarMdiButtonLinkSkinViewInfo), new BarMdiButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarItemInfo("SkinBarSubItem", "Skin Menu", 12, typeof(SkinBarSubItem), typeof(SkinBarSubItemLink), typeof(SkinBarSubItemViewInfo), new BarCustomContainerLinkPainter(this), true, false));
		}
		protected override UserLookAndFeel CreateCustomizationLookAndFeel() {
			SkinBarEmbeddedLookAndFeel lf = new SkinBarEmbeddedLookAndFeel(this);
			lf.SetSkinStyle(lf.SkinName); 
			return lf;
		}
		protected override UserLookAndFeel CreateLookAndFeel() {
			SkinBarEmbeddedLookAndFeel lf = new SkinBarEmbeddedLookAndFeel(this);
			lf.SetSkinStyle(lf.SkinName); 
			return lf;
		}
		protected override void CreatePainters() {
			this.fDrawParameters = new SkinBarDrawParameters(this);
			this.fPrimitivesPainter = new SkinPrimitivesPainter(this);
			this.fElementsPainter = new DockElementsSkinPainter(this);
			this.fElementsParameters = new DockElementsSkinParameters(this);
		}
		protected virtual SkinElement GetLinkElement(BarLinkViewInfo linkInfo, BarIndent linkIndent) {
			if(linkInfo.DrawMode == BarLinkDrawMode.InMenuGallery)
				return GetSkin(linkInfo)[BarSkins.SkinLinkSelected];
			if(linkInfo.IsLinkInMenu)
				return GetSkin(linkInfo)[BarSkins.SkinPopupMenuLinkSelected];
			if(linkInfo.BarControl is TitleBarControl)
				return GetDockManagerSkin(linkInfo)[DockingSkins.SkinDockWindowButton];
			if(linkInfo.BarControl is QuickCustomizationBarControl)
				return GetSkin(linkInfo)[BarSkins.SkinLinkSelected];
			if(linkInfo.Bar == null)
				return null;
			if(linkInfo.Bar.IsMainMenu) {
				if(linkIndent == BarIndent.DragBorder)
					return GetSkin(linkInfo)[BarSkins.SkinMainMenuDrag];
				else
					return GetSkin(linkInfo)[BarSkins.SkinMainMenuLinkSelected];
			}
			else if(linkInfo.Bar.IsStatusBar) {
				SkinElement element = GetSkin(linkInfo)[BarSkins.SkinInStatusBarLinkSelected];
				if(element == null)
					element = GetSkin(linkInfo)[BarSkins.SkinLinkSelected];
				return element;
			}
			else if(linkIndent == BarIndent.DragBorder)
				return GetSkin(linkInfo)[BarSkins.SkinBarDrag];
			return GetSkin(linkInfo)[BarSkins.SkinLinkSelected];
		}
		protected SkinPaddingEdges GetLinkContentMargins(BarLinkViewInfo linkInfo, BarIndent linkIndent) {
			SkinPaddingEdges res = new SkinPaddingEdges();
			SkinElement element = GetLinkElement(linkInfo, linkIndent);
			if(element != null)
				res = (SkinPaddingEdges)element.ContentMargins.Clone();
			if(linkInfo.Bar != null) {
				if(linkInfo.Link.GetLinkHorzIndent() != -1) {
					res.Left = linkInfo.Link.GetLinkHorzIndent();
					res.Right = linkInfo.Link.GetLinkHorzIndent();
				}
				if(linkInfo.Link.GetLinkVertIndent() != -1) {
					res.Top = linkInfo.Link.GetLinkVertIndent();
					res.Bottom = linkInfo.Link.GetLinkVertIndent();
				}
			}
			return res;
		}
		const int DefaultLinkHorzIndent = 6;
		const int DefaultLinkVertIndent = 4;
		protected internal override int CalcLinkIndent(BarLinkViewInfo linkInfo, BarIndent linkIndent) {
			SkinPaddingEdges cm = GetLinkContentMargins(linkInfo, linkIndent);
			if(cm.IsEmpty) {
				int res = base.CalcLinkIndent(linkInfo, linkIndent);
				if(res > -1)
					return res;
				if(linkIndent == BarIndent.Left || linkIndent == BarIndent.Right)
					return DefaultLinkHorzIndent;
				return DefaultLinkVertIndent;
			}
			if(linkInfo.IsDrawVerticalRotated) 
				cm = new SkinPaddingEdges(cm.Top, cm.Right, cm.Bottom, cm.Left);
			switch(linkIndent) { 
				case BarIndent.DragBorder:
				case BarIndent.SizeGrip:
				case BarIndent.Left:
					return cm.Left;
				case BarIndent.Right:
					return cm.Right;
				case BarIndent.Top:
					return cm.Top;
				case BarIndent.Bottom:
					return cm.Bottom;
			}
			return cm.Left;
		}
	}
	public class BarManagerOffice2003PaintStyle : BarManagerPaintStyle {
		protected BarManagerOffice2003PaintStyle(BarManagerPaintStyleCollection collection, string name) : base(collection, name) { }
		public BarManagerOffice2003PaintStyle(BarManagerPaintStyleCollection collection) : this(collection, "Office2003") {	}
		public override bool AllowLinkLighting { get { return false; } }
		protected override UserLookAndFeel CreateLookAndFeel() {
			BarEmbeddedLookAndFeel lf = new BarEmbeddedLookAndFeel();
			lf.UseDefaultLookAndFeel = false;
			lf.UseWindowsXPTheme = false;
			lf.Style = LookAndFeelStyle.Office2003;
			return lf;
		}
		protected override void CreatePainters() {
			this.fDrawParameters = new BarOffice2003DrawParameters(this);
			this.fPrimitivesPainter = new PrimitivesPainterOffice2003(this);
			this.fElementsPainter = new DockElementsOffice2003Painter(this);
			this.fElementsParameters = new DockElementsOffice2003Parameters();
		}
		protected override void RegisterBarInfo() {
			BarInfoCollection.Add(new BarControlInfo("BarDockControl", typeof(BarDockControl), typeof(DockControlOffice2003ViewInfo), new DockControlOffice2003Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("BarControl", typeof(CustomControl), typeof(BarControlViewInfo), new BarOffice2003Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("DockedBarControl", typeof(DockedBarControl), typeof(DockedBarControlOffice2003ViewInfo), new BarOffice2003Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControl", typeof(SubMenuBarControl), typeof(SubMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupMenuControl", typeof(PopupMenuBarControl), typeof(PopupMenuBarControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControl", typeof(PopupContainerBarControl), typeof(PopupContainerControlViewInfo), new PopupControlContainerBarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("QuickCustomizationBarControl", typeof(QuickCustomizationBarControl), typeof(QuickCustomizationBarControlViewInfo), new BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControl", typeof(FloatingBarControl), typeof(FloatingBarControlOffice2003ViewInfo), new FloatingOffice2003BarPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("TitleBarControl", typeof(DevExpress.XtraBars.Objects.TitleBarControl), typeof(DevExpress.XtraBars.Objects.TitleBarControlViewInfo), new BarOffice2003Painter(this)));
			BarInfoCollection.Add(new BarControlInfo("ControlForm", typeof(DevExpress.XtraBars.Forms.ControlForm), typeof(ControlFormViewInfo), new ControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("SubMenuControlForm", typeof(DevExpress.XtraBars.Forms.SubMenuControlForm), typeof(ControlFormViewInfo), new ControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("PopupContainerControlForm", typeof(DevExpress.XtraBars.Forms.PopupContainerForm), typeof(PopupContainerFormViewInfo), new ControlFormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("FloatingBarControlForm", typeof(DevExpress.XtraBars.Forms.FloatingBarControlForm), typeof(FloatingBarControlFormOffice2003ViewInfo), new FloatingBarControlOffice2003FormPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("GalleryDropDownBarControl", typeof(GalleryDropDownBarControl), typeof(GalleryDropDownControlViewInfo), new NonSkinGalleryDropDownBarControlPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuBarControl", typeof(AppMenuBarControl), typeof(AppMenuControlViewInfo), new BarSubMenuPainter(this)));
			BarInfoCollection.Add(new BarControlInfo("AppMenuForm", typeof(DevExpress.XtraBars.Forms.AppMenuForm), typeof(AppMenuFormViewInfo), new SkinAppMenuFormPainter(this)));
		}
		public override BarButtonLinkPainter CreateButtonItemLinkPainter() { return new BarButtonOffice2003LinkPainter(this); }
		protected override void RegisterItemInfo() {
			ItemInfoCollection.Add(new BarItemInfo("BarButtonItem", "Button", 0, typeof(BarButtonItem), typeof(BarButtonItemLink), typeof(BarButtonLinkViewInfo), CreateButtonItemLinkPainter(), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarCheckItem", "Check", 9, typeof(BarCheckItem), typeof(BarCheckItemLink), typeof(BarCheckButtonLinkViewInfo), new BarBaseButtonOffice2003LinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToggleSwitchItem", "Toggle Switch", 9, typeof(BarToggleSwitchItem), typeof(BarToggleSwitchItemLink), typeof(BarToggleSwitchLinkViewInfo), new BarToggleSwitchLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarSubItem", "Menu", 1, typeof(BarSubItem), typeof(BarSubItemLink), typeof(BarSubItemLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarStaticItem", "Static Text", 2, typeof(BarStaticItem), typeof(BarStaticItemLink), typeof(BarStaticLinkViewInfo), new BarStaticLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarHeaderItem", "Header Text", 2, typeof(BarHeaderItem), typeof(BarHeaderItemLink), typeof(BarHeaderLinkViewInfo), new BarHeaderLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarEditItem", "Editor", 3, typeof(BarEditItem), typeof(BarEditItemLink), typeof(BarEditLinkViewInfo), new BarEditLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerItem", "Link Container", 4, typeof(BarLinkContainerItem), typeof(BarLinkContainerItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarLargeButtonItem", "Large Button", 5, typeof(BarLargeButtonItem), typeof(BarLargeButtonItemLink), typeof(BarLargeButtonLinkViewInfo), new BarLargeButtonLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarListItem", "String List", 6, typeof(BarListItem), typeof(BarListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarMdiChildrenListItem", "Mdi Children List", 7, typeof(BarMdiChildrenListItem), typeof(BarMdiChildrenListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarDockingMenuItem", "Docking Menu", 13, typeof(BarDockingMenuItem), typeof(BarDockingMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarWorkspaceMenuItem", "Workspace Menu", 14, typeof(BarWorkspaceMenuItem), typeof(BarWorkspaceMenuItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, true));
			ItemInfoCollection.Add(new BarItemInfo("BarToolbarsListItem", "Toolbar List", 8, typeof(BarToolbarsListItem), typeof(BarToolbarsListItemLink), typeof(BarListLinkViewInfo), new BarCustomContainerLinkPainter(this), true, false));
			ItemInfoCollection.Add(new BarItemInfo("BarLinkContainerExItem", "LinkContainerEx", 4, typeof(BarLinkContainerExItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new BarCustomContainerLinkPainter(this), false, false));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuAddRemoveButtonsItem", typeof(BarQMenuAddRemoveButtonsItem), typeof(BarQMenuAddRemoveButtonsItemLink), typeof(BarQMenuAddRemoveButtonsLinkViewInfo), new BarQMenuAddRemoveButtonsLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQBarCustomization", typeof(BarQBarCustomizationItem), typeof(BarQBarCustomizationItemLink), typeof(BarQBarCustomizationLinkViewInfo), new BarQBarCustomizationOffice2003LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarQMenuCustomization", typeof(BarQMenuCustomizationItem), typeof(BarQMenuCustomizationItemLink), typeof(BarQMenuCustomizationLinkViewInfo), new BarQMenuCustomizationOffice2003LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarRecentExpander", typeof(BarRecentExpanderItem), typeof(BarRecentExpanderItemLink), typeof(BarRecentExpanderItemLinkViewInfo), new BarRecentExpanderOffice2003LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarEmptyItem", typeof(BarEmptyItem), typeof(BarEmptyItemLink), typeof(BarEmptyLinkViewInfo), new BarEmptyLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarMdiButtonItem", typeof(BarMdiButtonItem), typeof(BarMdiButtonItemLink), typeof(BarMdiButtonLinkViewInfo), new BarMdiButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarSystemMenuItem", typeof(BarSystemMenuItem), typeof(BarSystemMenuItemLink), typeof(BarSystemMenuLinkViewInfo), new BarCustomContainerLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarScrollItem", typeof(BarScrollItem), typeof(BarScrollItemLink), typeof(BarScrollItemLinkViewInfo), new BarScrollLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarDesignTimeItem", typeof(BarDesignTimeItem), typeof(BarDesignTimeItemLink), typeof(BarDesignTimeLinkViewInfo), new BarButtonLinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarCloseItem", typeof(BarCloseItem), typeof(BarCloseItemLink), typeof(BarCloseLinkViewInfo), new BarButtonOffice2003LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("BarInMdiChildrenListItem", typeof(BarInMdiChildrenListItem), typeof(BarInMdiChildrenListItemLink), typeof(BarInMdiChildrenListButtonLinkViewInfo), new BarButtonOffice2003LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("RibbonExpandCollapseItem", typeof(RibbonExpandCollapseItem), typeof(RibbonExpandCollapseItemLink), typeof(RibbonExpandCollapseItemLinkViewInfo), new BarButtonOffice2003LinkPainter(this)));
			ItemInfoCollection.Add(new BarInternalItemInfo("AutoHiddenPagesMenuItem", typeof(AutoHiddenPagesMenuItem), typeof(AutoHiddenPagesMenuItemLink), typeof(AutoHiddenPagesMenuItemLinkViewInfo), new BarButtonLinkPainter(this)));
		}
	}
}
