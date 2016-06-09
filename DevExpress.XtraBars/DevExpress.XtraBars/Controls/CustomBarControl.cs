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
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils.Text.Internal;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraBars.Controls {
	[ToolboxItem(false)]
	public class CustomControl : Control, IDisposableEx, IBarObject, ICustomBarControl, IEditorBackgroundProvider, IToolTipControlClient, ISupportAdornerUIManager {
		BarManager manager;
		Hashtable sizeHash;
		int lockLayout;
		int lockPaint = 0;
		bool destroying, creating, canDispose, destroyed;
		CustomViewInfo viewInfo;
		protected internal CustomControl(BarManager manager) {
			this.destroyed = false;
			this.sizeHash = new Hashtable();
			this.lockLayout = 0;
			this.manager = manager;
			this.destroying = false;
			this.canDispose = this.creating = true;
			this.viewInfo = null;
			SetStyle(ControlStyles.UserPaint | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint, true);
			OnAddToManager();
		}
		void OnAddToManager() {
			if(Manager != null) {
				Manager.GetToolTipController().AddClientControl(this);
				Manager.BarControls.Add(this);
			}
		}
		Control ICustomBarControl.Control { get { return this; } }
		protected virtual bool ProcessKeyPressCore(KeyPressEventArgs e) { return false; }
		bool ICustomBarControl.ProcessKeyPress(KeyPressEventArgs e) { return ProcessKeyPressCore(e); }
		public void AccessibleNotifyClients(AccessibleEvents events, int childID) {
			AccessibilityNotifyClients(events, childID);
		}
		DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal bool HasAccessible { get { return dxAccessible != null; } }
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.StandardAccessibleEx(this, base.CreateAccessibilityInstance());
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		protected internal bool CanDisposeCore { get { return canDispose; } set { canDispose = value; } }
		public virtual bool CanDispose { get { return canDispose; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(!CanDispose) return;
				UpdateVisualEffects(UpdateAction.Dispose);
			}
			OnRemoveFromManager();
			this.destroyed = true;
			base.Dispose(disposing);
		}
		private void OnRemoveFromManager() {
			if(Manager != null) {
				Manager.GetToolTipController().RemoveClientControl(this);
				Manager.BarControls.Remove(this);
			}
		}
		internal bool Destroyed { get { return destroyed; } }
		protected internal virtual void MakeLinkVisible(BarItemLink link) { }
		protected bool CanInit { get { return creating; } }
		protected internal virtual void Init() {
			if(!CanInit) return;
			try {
				viewInfo = CreateViewInfo();
			}
			finally {
				creating = false;
			}
		}
		protected virtual Hashtable SizeHash { get { return sizeHash; } }
		protected virtual bool IsAllowHash { get { return false; } }
		public virtual void ClearHash() {
			SizeHash.Clear();
		}
		public virtual bool IsInterceptKey(KeyEventArgs e) { return true; }
		public virtual bool IsNeededKey(KeyEventArgs e) { return false; }
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			OnKeyDown(e);
		}
		public virtual void ProcessKeyUp(KeyEventArgs e) {
			OnKeyUp(e);
		}
		public virtual bool IsSubMenu { get { return false; } }
		public virtual bool IsMultiLine { get { return false; } }
		public virtual bool IsVertical { get { return false; } set { } }
		public virtual bool RotateWhenVertical { get { return false; } }
		public virtual bool Destroying { get { return destroying; } }
		public virtual bool IsCanSpringLinks { get { return false; } }
		public virtual BarManager Manager { get { return manager; } protected set { } }
		internal virtual void SetManagerCore(BarManager manager) { this.manager = manager; }
		public virtual CustomControlViewInfo ViewInfo { get { return Destroying ? null : viewInfo as CustomControlViewInfo; } }
		protected internal virtual CustomViewInfo CreateViewInfo() {
			if(Manager == null) return null;
			CustomViewInfo bv = Manager.Helper.CreateControlViewInfo(this);
			bv.SourceObject = this;
			return bv;
		}
		bool IBarObject.IsBarObject { get { return true; } }
		public virtual bool ShouldCloseOnLostFocus(Control newFocus) { return true; }
		BarMenuCloseType IBarObject.ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return ShouldCloseMenuOnClick(e, child) ? BarMenuCloseType.All : BarMenuCloseType.None;
		}
		public virtual bool ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			return true;
		}
		public virtual bool ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) { return true; }
		public virtual void BeginLayout() { LockLayout++; }
		public virtual void EndLayout() {
			if(--LockLayout == 0) LayoutChanged();
		}
		public virtual int LockLayout { get { return lockLayout; } set { lockLayout = value; } }
		public virtual void UpdateScheme() {
			viewInfo = CreateViewInfo();
			OnSchemeChanged();
			LayoutChanged();
			UpdateViewInfo();
		}
		protected virtual void OnSchemeChanged() { }
		public virtual void LayoutChanged() {
			if(LockLayout != 0) return;
			ClearHash();
			UpdateViewInfo();
			Invalidate();
		}
		public virtual void DoSetVisible(bool newVisible) {
			if(newVisible != Visible)
				Visible = newVisible;
		}
		public Size CalcSize(int width) { return CalcSize(width, -1); }
		public virtual Size CalcSize(int width, int maxHeight) {
			if(IsAllowHash) {
				if(SizeHash.Contains(width)) return (Size)SizeHash[width];
			}
			CheckDirty();
			CustomControlViewInfo vInfo = CreateViewInfo() as CustomControlViewInfo;
			if(vInfo == null) return Size.Empty;
			Size size = vInfo.CalcBarSize(null, this, width, maxHeight); 
			if(IsAllowHash) {
				SizeHash[width] = size;
			}
			return size;
		}
		public virtual Size CalcMinSize() {
			int minWidth = 0;
			if(IsAllowHash) {
				if(SizeHash.Contains(minWidth)) return (Size)SizeHash[minWidth];
			}
			CheckDirty();
			CustomControlViewInfo vInfo = CreateViewInfo() as CustomControlViewInfo;
			if(vInfo == null) return Size.Empty;
			Size size = vInfo.CalcBarSize(null, this, minWidth, -1);
			if(IsAllowHash) {
				SizeHash[minWidth] = size;
			}
			return size;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage {
			get { return null; } 
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get {
				return ViewInfo == null || ViewInfo.Appearance.Normal.BackColor == Color.Empty ? SystemColors.Control : ViewInfo.Appearance.Normal.BackColor;
			}
		}
		public virtual void UpdateViewInfo() {
			if(Destroyed) return;
			if(ViewInfo != null) {
				UpdateVisualEffects(UpdateAction.BeginUpdate);
				ClearHash();
				ViewInfo.CalcViewInfo(null, this, ClientRectangle);
				UpdateVisualEffects(UpdateAction.EndUpdate);
			}
		}
		protected override void OnCreateControl() {
			if(ViewInfo != null) ViewInfo.UpdateControlRegion(this);
			base.OnCreateControl();
		}
		protected override void OnResize(EventArgs e) {
			if(Destroying) return;
			UpdateViewInfo();
			base.OnResize(e);
			if(ViewInfo != null)
				ViewInfo.UpdateControlRegion(this);
		}
		protected void DirectDraw(PaintEventArgs e) {
			GraphicsInfoArgs dra = new GraphicsInfoArgs(new GraphicsCache(e), ClientRectangle);
			ViewInfo.Painter.Draw(dra, ViewInfo, null);
			dra.Cache.Dispose();
		}
		protected internal void LockPaint() { lockPaint++; }
		protected internal void UnlockPaint() {
			if(--lockPaint == 0) Invalidate();
		}
		protected internal bool IsPaintLocked { get { return lockPaint != 0; } }
		protected override void OnPaintBackground(PaintEventArgs e) {
			if(IsPaintLocked) return;
			base.OnPaintBackground(e);
		}
		protected virtual bool HasInvalidLinks {
			get { return false; }
		}
		protected internal virtual void CheckDirty() {
			if(!ViewInfo.IsReady || HasInvalidLinks) UpdateViewInfo();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(IsPaintLocked || (Manager != null && Manager.IsPaintLocked)) return;
			DevExpress.Utils.Mdi.ControlState.CheckPaintError(this);
			if(ViewInfo != null && Manager != null && !Manager.IsDestroying && !Destroyed) {
				CheckDirty();
				if(ViewInfo.IsDrawForeground)
					DirectDraw(e);
				else
					PaintBase(e);
			}
			else
				PaintBase(e);
			RaisePaintEvent(this, e);
		}
		protected virtual void PaintBase(PaintEventArgs e) {
			base.OnPaint(e);
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		#region IEditorBackgroundProvider Members
		bool IEditorBackgroundProvider.DrawBackground(Control ownerEdit, GraphicsCache cache) {
			return ViewInfo.Painter.DrawEditorBackground(ownerEdit, ViewInfo, cache);
		}
		#endregion
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			BarItemLink link = null;
			ToolTipControlInfo info = GetToolTipInfo(point, out link);
			if(link == null)
				return info;
			if(!link.ScreenBounds.IsEmpty && Manager.ToolTipAnchor == ToolTipAnchor.BarItemLink) {
				info.ObjectBounds = link.ScreenBounds;
				info.ToolTipLocation = Manager.GetToolTipController().IsDefaultToolTipLocation ? ToolTipLocation.BottomLeft : Manager.GetToolTipController().ToolTipLocation;
			}
			else {
				info.ObjectBounds = Rectangle.Empty;
			}
			if(link.IsLinkInMenu) {
				info.ToolTipLocation = ToolTipLocation.RightCenter;
			}
			return info;
		}
		protected virtual ToolTipControlInfo GetToolTipInfo(Point point, out BarItemLink link) {
			link = null;
			return null;
		}
		bool IToolTipControlClient.ShowToolTips {
			get { return Manager == null || !Manager.IsDesignMode; }
		}
		#region ISupportAdornerUIManager Members
		readonly static object updateVisualEffects = new object();
		event UpdateActionEventHandler ISupportAdornerUIManager.Changed {
			add { Events.AddHandler(updateVisualEffects, value); }
			remove { Events.RemoveHandler(updateVisualEffects, value); }
		}
		void ISupportAdornerUIManager.UpdateVisualEffects(UpdateAction action) { UpdateVisualEffects(action); }
		protected void UpdateVisualEffects(UpdateAction action) {
			UpdateActionEventHandler handler = Events[updateVisualEffects] as UpdateActionEventHandler;
			if(handler == null) return;
			handler(this, new UpdateActionEvendArgs(action));
		}
		#endregion
	}
	public abstract class CustomLinksControl : CustomControl, ISupportXtraAnimation {
		public enum ControlLink { EmptyLink, DesignTimeLink, CloseLink, UpScroll, DownScroll };
		public class InternalControlLinks : IDisposable {
			Hashtable links;
			CustomLinksControl owner;
			public InternalControlLinks(CustomLinksControl owner) {
				this.owner = owner;
				this.links = new Hashtable();
			}
			public CustomLinksControl Owner { get { return owner; } }
			public virtual void Dispose() {
				if(Links != null) {
					DestroyLinks();
					this.links = null;
					this.owner = null;
				}
			}
			public BarItemLink EmptyLink { get { return this[ControlLink.EmptyLink]; } }
			protected virtual void DestroyLinks() {
				if(Links == null) return;
				foreach(DictionaryEntry entry in Links) {
					BarItemLink link = entry.Value as BarItemLink;
					if(link != null) link.Dispose();
				}
				Links.Clear();
			}
			public virtual void UpdateOwner(object ownerControl) {
				if(Links == null) return;
				foreach(BarItemLink link in Links.Values) {
					link.ownerControl = ownerControl;
				}
			}
			public virtual void UpdateLinkedObject(object linkedObject) {
				if(Links == null) return;
				foreach(BarItemLink link in Links.Values) {
					link.linkedObject = linkedObject;
				}
			}
			public BarManager Manager { get { return Owner.Manager; } }
			public virtual void CreateLinks() {
				this[ControlLink.EmptyLink] = Manager.InternalItems.EmptyItem.CreateLink(null, Owner);
				this[ControlLink.CloseLink] = Manager.InternalItems.CloseItem.CreateLink(null, Owner);
				if(Manager.IsDesignMode) {
					this[ControlLink.DesignTimeLink] = Manager.InternalItems.DesignTimeItem.CreateLink(null, Owner);
				}
				UpdateOwner(Owner);
			}
			protected Hashtable Links { get { return links; } }
			public BarItemLink this[ControlLink link] {
				get { return Links[link] as BarItemLink; }
				set { Links[link] = value; }
			}
		}
		InternalControlLinks controlLinks;
		protected IList linksSource;
		Point mouseLastPoint, mouseDownPoint;
		BarItemLinkReadOnlyCollection visibleLinks;
		BarItemLinkReadOnlyCollection links;
		LinksNavigation navigator;
		ContainerKeyTipManager keyTipManager = null;
		internal CustomLinksControl(BarManager manager, IList linksSource)
			: base(manager) {
			this.navigator = CreateLinksNavigator();
			this.mouseDownPoint = this.mouseLastPoint = new Point(-1, -1);
			this.linksSource = linksSource;
			this.visibleLinks = new BarItemLinkReadOnlyCollection();
			this.links = new BarItemLinkReadOnlyCollection();
			this.TabStop = false;
			if(Manager != null && Manager.IsCustomizing) AllowDrop = true;
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point, out BarItemLink link) {
			if(!ViewInfo.IsReady) CheckDirty();
			BarLinkViewInfo linkInfo = ViewInfo.GetLinkViewInfoByPoint(point, false);
			if(linkInfo != null) {
				link = linkInfo.Link;
				return linkInfo.Link.GetToolTipInfo(point);
			}
			link = null;
			return null;
		}
		CustomAnimationInvoker animationInvoker;
		internal CustomAnimationInvoker AnimationInvoker {
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(this.OnAnimation);
				return animationInvoker;
			}
		}
		protected internal virtual void OnAnimation(BaseAnimationInfo info) {
			if(Manager.IsDesignMode) return;
			BarAnimatedItemsHelper.Animate(this, Manager, info, AnimationInvoker);
		}
		protected Point MouseDownPoint { get { return mouseDownPoint; } set { mouseDownPoint = value; } }
		protected Point MouseLastPoint { get { return mouseLastPoint; } set { mouseLastPoint = value; } }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.CustomLinksContainerAccessible(this);
		}
		protected virtual InternalControlLinks CreateInternalControlLinks() {
			return new InternalControlLinks(this);
		}
		public InternalControlLinks ControlLinks {
			get {
				if(controlLinks == null && Manager != null) {
					controlLinks = CreateInternalControlLinks();
					controlLinks.CreateLinks();
				}
				return controlLinks;
			}
		}
		protected abstract LinksNavigation CreateLinksNavigator();
		public virtual LinksNavigation Navigator { get { return navigator; } }
		public override bool IsInterceptKey(KeyEventArgs e) { return Navigator.IsInterceptKey(e, ThisActiveLink); }
		public override bool IsNeededKey(KeyEventArgs e) { return Navigator.IsNeededKey(e, ThisActiveLink); }
		public override void ProcessKeyDown(KeyEventArgs e) {
			Navigator.ProcessKeyDown(e, ThisActiveLink);
		}
		protected override bool ProcessKeyPressCore(KeyPressEventArgs e) {
			if(KeyTipManager.Show) {
				KeyTipManager.AddChar(e.KeyChar);
				return true;
			}
			return false;
		}
		protected virtual bool ShouldCloseOnChildClick { get { return false; } }
		public override bool ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			if(Destroying) return true;
			if(e.MouseUp) return false;
			BarItemLink link = GetLinkByPoint(e.ScreenPoint);
			if(link == null) {
				if(IsHandleCreated && this.ClientRectangle.Contains(PointToClient(e.ScreenPoint))) return false;
				if(child != null && child.ClientRectangle.Contains(child.PointToClient(e.ScreenPoint)))
					return ShouldCloseOnChildClick;
			}
			if(Manager.SelectionInfo.HighlightedLink == null || GetVisibleLinks().Contains(Manager.SelectionInfo.HighlightedLink)) return false;
			if(this is IPopup) {
				Manager.SelectionInfo.ClosePopupChildren(this as IPopup);
				return false;
			}
			return true;
		}
		public override void ProcessKeyUp(KeyEventArgs e) {
			Navigator.ProcessKeyUp(e, ThisActiveLink);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(KeyTipManager != null)
					KeyTipManager.ClearItems();
				if(Manager != null) {
					if(VisibleLinks.Contains(Manager.SelectionInfo.HighlightedLink)) {
						Manager.SelectionInfo.HighlightedLink = null;
					}
				}
				if(CanDispose) {
					SetLinksOwner(null);
				}
				if(controlLinks != null) controlLinks.Dispose();
				this.controlLinks = null;
			}
			base.Dispose(disposing);
		}
		public virtual BarItemLink FindAcceleratedLink(KeyEventArgs e) {
			foreach(BarItemLink link in GetLinks()) {
				if(link.Enabled) {
					Keys linkKey = link.Accelerator;
					if(linkKey != Keys.None && (e.KeyData == linkKey || (e.KeyCode == linkKey && e.Alt))) return link;
				}
			}
			return null;
		}
		public virtual BarItemLinkReadOnlyCollection GetVisibleLinks() { return VisibleLinks; }
		internal virtual BarItemLinkReadOnlyCollection GetLinks() { return TotalVisibleLinks; }
		public virtual BarItemLink ThisActiveLink {
			get {
				if(VisibleLinks.Contains(Manager.SelectionInfo.EditingLink))
					return Manager.SelectionInfo.EditingLink;
				if(VisibleLinks.Contains(Manager.SelectionInfo.HighlightedLink)) return Manager.SelectionInfo.HighlightedLink;
				return null;
			}
		}
		public virtual BarItemLinkReadOnlyCollection VisibleLinks { get { return visibleLinks; } }
		internal virtual BarItemLinkReadOnlyCollection TotalVisibleLinks { get { return links; } }
		protected internal virtual void ExcludeRarelyItemFromVisible(BarItemLinkReadOnlyCollection links) {
			int recentCount = links.RecentLinkCount;
			if(recentCount > 1) {
				links.SortRecentList();
				for(int n = recentCount - 1; n >= 0; n--) {
					BarItemLink link = links.GetRecentLink(n);
					if(link.Item is BarQBarCustomizationItem) continue;
					links.RemoveItem(link);
					break;
				}
			}
		}
		protected virtual void AddBarVisibleLinks(BarItemLinkReadOnlyCollection visibleLinks, IList source) {
			if(Manager == null || Manager.Helper == null || Manager.Helper.RecentHelper == null) return;
			Manager.Helper.RecentHelper.ClearInternalGroups(source);
			AddVisibleLinks(visibleLinks, source);
		}
		protected virtual void AddSimpleVisibleLinks(BarItemLinkReadOnlyCollection visibleLinks, IList source) {
			Manager.Helper.RecentHelper.ClearInternalGroups(source);
			bool shouldBeginGroup = false;
			foreach(BarItemLink link in source) {
				if(!link.CanVisible) {
					if(link.BeginGroup) shouldBeginGroup = true;
					continue;
				}
				visibleLinks.AddItem(link);
				if(shouldBeginGroup)
					link.internalBeginGroup = shouldBeginGroup;
				shouldBeginGroup = false;
			}
		}
		protected virtual bool ContainsListItemLink(BarCustomContainerItemLink listLink, BarItemLink link) {
			if(listLink == null) return false;
			if(listLink.Item.ItemLinks.Contains(link) || listLink.Item.ItemLinks.Contains(link.ClonedFromLink)) return true;
			foreach(BarItemLink lLink in listLink.Item.ItemLinks) {
				if(ContainsListItemLink(lLink as BarCustomContainerItemLink, link)) return true;
			}
			return false;
		}
		protected virtual bool ContainsListItemLink(IList critLinks, BarItemLink link) {
			foreach(object obj in critLinks) {
				if(ContainsListItemLink(obj as BarCustomContainerItemLink, link)) return true;
			}
			return false;
		}
		protected virtual void CopyLinks(BarItemLinkReadOnlyCollection destLinks, BarItemLinkReadOnlyCollection srcLinks, IList critLinks) {
			foreach(BarItemLink link in srcLinks) {
				if(critLinks == null || critLinks.Contains(link)) {
					destLinks.AddItem(link);
					continue;
				}
				if(ContainsListItemLink(critLinks, link)) destLinks.AddItem(link);
			}
		}
		protected virtual void UpdateEditingAndHighlightedLink(BarItemLinkReadOnlyCollection inplaceLinks) {
			foreach(BarItemLink inlink in inplaceLinks) {
				if(Manager.SelectionInfo.EditingLink != null && Manager.SelectionInfo.EditingLink.ClonedFromLink == inlink.ClonedFromLink)
					Manager.SelectionInfo.SetEditingLink(inlink as BarEditItemLink);
				if(Manager.SelectionInfo.HighlightedLink != null && Manager.SelectionInfo.HighlightedLink.ClonedFromLink == inlink.ClonedFromLink)
					Manager.SelectionInfo.SetHighlightedLink(inlink);
			}
		}
		protected virtual bool CanHideLink(BarItemLink link) {
			return false;
		}
		protected virtual bool SuppressPaint { get; set; }
		protected virtual void AddVisibleLinks(BarItemLinkReadOnlyCollection visibleLinks, IList sourceLinks) {
			bool shouldBeginGroup = false;
			for(int n = 0; n < sourceLinks.Count; n++) {
				BarItemLink link = sourceLinks[n] as BarItemLink;
				link.internalBeginGroup = false;
				if(!link.CanVisible) {
					if(link.BeginGroup) shouldBeginGroup = true;
					continue;
				}
				if(CanHideLink(link)) continue;
				SetLinkOwner(link, this);
				if(link is BarCustomContainerItemLink) {
					BarCustomContainerItemLink cInternalLink = link as BarCustomContainerItemLink;
					if(Manager != null) Manager.LockPaint();
					((BarCustomContainerItem)cInternalLink.Item).OnGetItemData();
					if(Manager != null) Manager.UnlockPaint();
					BarItemLinkReadOnlyCollection inplaceLinks = cInternalLink.InplaceLinks;
					if(inplaceLinks != null) {
						if(cInternalLink.Item.ShouldUpdateEditingLink)
							UpdateEditingAndHighlightedLink(inplaceLinks);
						if(inplaceLinks.Count > 0)
							inplaceLinks[0].SetBeginGroup(link.BeginGroup | shouldBeginGroup);
						AddVisibleLinks(visibleLinks, inplaceLinks);
						shouldBeginGroup = false;
						SetLinkOwner(link, null);
						continue;
					} 
					BarSubItem subItem = (BarCustomContainerItem)cInternalLink.Item as BarSubItem;
					if(subItem != null && !subItem.InplaceVisible) continue;
				}
				if(link.Manager == null || link.Item == null)
					continue;
				visibleLinks.AddItem(link);
				if(shouldBeginGroup)
					link.internalBeginGroup = shouldBeginGroup;
				shouldBeginGroup = false;
			}
		}
		protected virtual void SetLinkOwner(BarItemLink link, object owner) {
			if((owner == null && link.ownerControl == this) || owner != null)
				link.ownerControl = owner;
		}
		protected virtual void SetLinksOwner(BarItemLinkReadOnlyCollection links, object owner) {
			foreach(BarItemLink link in links) {
				SetLinkOwner(link, owner);
			}
		}
		protected virtual void SetLinksOwner(object owner) {
			SetLinksOwner(VisibleLinks, owner);
		}
		public virtual BarItemLink GetLinkByPoint(Point screenPoint) { return GetLinkByPoint(screenPoint, false); }
		public virtual BarItemLink GetLinkByPoint(Point screenPoint, bool includeSeparator) {
			if(ViewInfo == null) return null;
			BarLinkViewInfo lvi = ViewInfo.GetLinkViewInfoByPoint(PointToClient(screenPoint), includeSeparator);
			if(lvi != null) {
				if(lvi.Link != null && lvi.Link.Item == null) return null;
				return lvi.Link;
			}
			return null;
		}
		protected internal virtual void LockedUpdateVisibleLinks() {
			if(LockLayout != 0) return;
			LockLayout++;
			try {
				UpdateVisibleLinks();
			}
			finally {
				LockLayout--;
			}
		}
		protected virtual bool HasGhostLinks() {
			foreach(BarItemLink link in VisibleLinks) {
				if(link.Manager == null || link.Item == null)
					return true;
			}
			return false;
		}
		public override void UpdateViewInfo() {
			if(HasGhostLinks()) {
				RemoveGhostLinks();
			}
			base.UpdateViewInfo();
		}
		protected virtual void RemoveGhostLinks() {
			UpdateVisibleLinks();
		}
		protected internal virtual void UpdateVisibleLinks() {
			SetLinksOwner(null);
			VisibleLinks.ClearItems();
			if(linksSource != null)
				AddBarVisibleLinks(VisibleLinks, linksSource);
		}
		protected virtual bool HandleMouseEvent(Point p) {
			return (mouseLastPoint != p);
		}
		public override void LayoutChanged() {
			if(LockLayout != 0) return;
			LockedUpdateVisibleLinks();
			UpdateViewInfo();
			base.LayoutChanged();
			Invalidate();
		}
		public void ForceMouseEnter() {
			this.mouseLastPoint = Point.Empty;
			OnMouseEnter(EventArgs.Empty);
		}
		bool IsPointDisabled(Point pt) { return pt == new Point(-1, -1); }
		bool IsSubControl { get { return this is CustomLinksControl; } }
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			if(!HandleMouseEvent(Cursor.Position)) return;
			mouseLastPoint = Cursor.Position;
			if(Manager.SelectionInfo.KeyboardHighlightedLink != null) return;
			BarItemLink link = GetLinkByPoint(Cursor.Position);
			if(!ShouldProcessHighlightedLink(link)) return;
			Manager.SelectionInfo.HighlightedLink = GetLinkByPoint(Cursor.Position);
			Manager.SelectionInfo.UpdatePopupHighlighting();
		}
		protected override void OnMouseLeave(EventArgs e) {
			if(Manager.IsCustomizing) return;
			if(GetVisibleLinks().Contains(Manager.SelectionInfo.HighlightedLink))
				Manager.SelectionInfo.HighlightedLink = null;
			Manager.SelectionInfo.UpdatePopupHighlighting();
			base.OnMouseLeave(e);
		}
		protected virtual bool ShouldHideKeyTips(BarItemLink link) {
			if(link is BarSubItemLink) return true;
			BarButtonItemLink buttonLink = link as BarButtonItemLink;
			if(buttonLink != null && buttonLink.Item != null && buttonLink.Item.IsDropDownButtonStyle) return true;
			return false;
		}
		protected virtual void HideKeyTips() {
			RibbonBarManager ribbonManager = Manager as RibbonBarManager;
			if(ribbonManager == null || ribbonManager.Ribbon == null) return;
			ribbonManager.Ribbon.DeactivateKeyTips();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(!HandleMouseEvent(Cursor.Position)) {
				base.OnMouseMove(e);
				return;
			}
			CheckDirty();
			OnMouseMoveCore(e);
			base.OnMouseMove(e);
		}
		protected virtual void OnMouseMoveCore(MouseEventArgs e) {
			BarItemLink link = GetLinkByPoint(Cursor.Position);
			Point pt = MouseLastPoint;
			mouseLastPoint = Cursor.Position;
			if(!IsPointDisabled(pt)) {
				if(Manager.IsCustomizing && Manager.SelectionInfo.CustomizeSelectedLink != null) {
					if(Manager.Helper.CustomizationManager.CheckAndCustomizationMouseMove(this, e, mouseDownPoint, GetLinkByPoint(mouseLastPoint))) return;
				}
				if(ShouldProcessHighlightedLink(link)) {
					if(ShouldHideKeyTips(link)) HideKeyTips();
					Manager.SelectionInfo.HighlightedLink = link;
					if(Manager.SelectionInfo.HighlightedLink != null)
						Manager.SelectionInfo.HighlightedLink.OnLinkActionCore(BarLinkAction.MouseMove, e);
				}
			}
			CheckUpdateHyperlink(link, pt);
		}
		Cursor SavedCursor { get; set; }
		private void CheckUpdateHyperlink(BarItemLink link, Point pt) {
			if(Manager.IsCustomizing)
				return;
			if(link != null && link.LinkViewInfo.HtmlStringCaptionInfo != null) {
				StringBlock block = link.LinkViewInfo.HtmlStringCaptionInfo.GetLinkByPoint(PointToClient(pt));
				if(block != null) {
					if(SavedCursor == null)
						SavedCursor = Cursor;
					Cursor = Cursors.Hand;
				}
				else {
					Cursor = SavedCursor;
					SavedCursor = null;
				}
			}
		}
		protected virtual bool ShouldProcessHighlightedLink(BarItemLink link) {
			RibbonBarManager ribbonManager = Manager as RibbonBarManager;
			if(ribbonManager == null || ribbonManager.Ribbon == null || ribbonManager.Ribbon.CustomizationPopupMenu == null || !ribbonManager.Ribbon.CustomizationPopupMenu.Visible) return true;
			if(ribbonManager.Ribbon.CustomizationPopupMenu.SubControl.VisibleLinks.Contains(link)) return true;
			return false;
		}
		protected virtual void OnDoubleClickLink(DXMouseEventArgs e, BarItemLink link) {
			Rectangle rect = Rectangle.Empty;
			if(link.RibbonItemInfo == null && link.LinkViewInfo != null) rect = link.LinkViewInfo.Rects[BarLinkParts.OpenArrow];
			if(rect.Contains(e.Location)) return;
			Manager.SelectionInfo.DoubleClickLink(link);
		}
		protected virtual void OnLeftMouseDown(DXMouseEventArgs e, BarItemLink link) {
			if(e.Clicks == 1 && (Control.ModifierKeys & Keys.Alt) != 0) {
				Capture = true;
				Manager.Helper.CustomizationManager.CheckAndStartHotCustomization(this, e, link);
			}
			else {
				if(link != null) {
					if(e.Clicks == 1) {
						Capture = link.IsNeedMouseCapture;
						Manager.SelectionInfo.PressLink(link);
					}
					if(e.Clicks == 2)
						OnDoubleClickLink(e, link);
					if(e.Clicks == 1) {
						if(Manager.SelectionInfo.CanResizeCustomizeSelectedLink(mouseDownPoint, link) && !Manager.IsLinkSizing) {
							Manager.Helper.DragManager.StartLinkSizing(this, link, mouseDownPoint);
						}
					}
					e.Handled = true;
					return;
				}
			}
		}
		protected virtual void OnRightMouseDown(DXMouseEventArgs e, BarItemLink link) {
			if(Manager.IsCustomizing) {
				if(link != null) {
					if(Manager.SelectionInfo.CustomizeSelectedLink != link)
						Manager.SelectionInfo.CustomizeSelectedLink = link;
					Manager.Helper.CustomizationManager.ShowItemDesigner();
					e.Handled = true;
					return;
				}
			}
			else {
				if(link != null && !link.Item.AllowRightClickInMenu && link.IsLinkInMenu)
					return;
				if(IsAllowShowMenusOnRightClick)
					Manager.ShowToolBarsPopup(link);
				else if(link != null && (link.Holder is PopupMenu || link.Holder is BarCustomContainerItem)) {
					Capture = link.IsNeedMouseCapture;
					Manager.SelectionInfo.PressLink(link);
					e.Handled = true;
					return;
				}
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			if(ee.Handled) {
				base.OnMouseDown(ee);
				return;
			}
			CheckDirty();
			this.mouseDownPoint = PointToScreen(e.Location);
			OnMouseDownCore(ee);
			base.OnMouseDown(ee);
		}
		protected virtual void OnMouseDownCore(DXMouseEventArgs ee) {
			BarItemLink link = GetLinkByPoint(MouseDownPoint);
			if(ee.Button == MouseButtons.Left) {
				OnLeftMouseDown(ee, link);
			}
			if(ee.Button == MouseButtons.Right && ee.Clicks == 1) {
				OnRightMouseDown(ee, link);
			}
		}
		protected virtual bool IsAllowShowMenusOnRightClick { get { return true; } } 
		StringBlock GetHyperlink(BarItemLink link, MouseEventArgs e) {
			if(!(link is BarStaticItemLink)) {
				return null;
			}
			return null;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(Destroying || IsDisposed) return;
			BarItemLink link = GetLinkByPoint(PointToScreen(e.Location));
			if((e.Button & MouseButtons.Left) != 0) {
				if(Manager.IsCustomizing) {
					if(Manager.IsLinkSizing) {
						Manager.Helper.DragManager.StopLinkSizing(e.Button == MouseButtons.Left);
					}
					if(link == null || !link.AlwaysWorking)
						return;
				}
				Capture = false;
				CheckNavigateHyperlink(link, e.Location);
				Manager.SelectionInfo.UnPressLink(link);
			}
			else if((e.Button & MouseButtons.Right) != 0) {
				if(!IsAllowShowMenusOnRightClick && link != null && (!link.IsLinkInMenu || link.Item.AllowRightClickInMenu)) {
					Capture = false;
					Manager.SelectionInfo.UnPressLink(link);
				}
			}
			base.OnMouseUp(e);
		}
		protected virtual void CheckNavigateHyperlink(BarItemLink link, Point point) {
			if(link == null || link.LinkViewInfo.HtmlStringCaptionInfo == null)
				return;
			StringBlock hlink = link.LinkViewInfo.HtmlStringCaptionInfo.GetLinkByPoint(point);
			if(hlink != null)
				link.Item.OnHyperLinkClick(link, hlink);
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			Manager.Helper.DragManager.ItemOnQueryContinueDrag(e, this);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			Manager.Helper.DragManager.ItemOnGiveFeedback(e, this);
		}
		protected override void OnDragEnter(DragEventArgs e) {
			Manager.Helper.DragManager.FireDoDragging = false;
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
		}
		protected override void OnDragLeave(EventArgs e) {
			Manager.Helper.DragManager.FireDoDragging = true;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			Manager.Helper.DragManager.StopDragging(this, e.Effect, false);
		}
		protected override void OnDragOver(DragEventArgs e) {
			if(!Manager.Helper.DragManager.CanAcceptDragObject(e.Data))
				e.Effect = DragDropEffects.None;
			else
				e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
			Manager.Helper.DragManager.DoDragging(this, new MouseEventArgs(Control.MouseButtons, 0, Control.MousePosition.X, Control.MousePosition.Y, 0));
			if(Manager.Helper.DragManager.UseDefaultCursors) {
				if(Manager.Helper.DragManager.HitInfo != null && Manager.Helper.DragManager.HitInfo.Cursor == BarManager.NoDropCursor)
					e.Effect = DragDropEffects.None;
			}
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl { get { return this; } }
		protected virtual ContainerKeyTipManager CreateKeyTipManager() {
			return new ContainerKeyTipManager(null, this, VisibleLinks);
		}
		public virtual ContainerKeyTipManager KeyTipManager {
			get {
				if(keyTipManager == null) keyTipManager = CreateKeyTipManager();
				return keyTipManager;
			}
		}
	}
	[ToolboxItem(false)]
	public class QuickCustomizationBarControl : CustomPopupBarControl, IFormContainedControl {
		BarQBarCustomizationItemLink containerLink;
		internal QuickCustomizationBarControl(BarManager barManager, BarQBarCustomizationItemLink containerLink)
			: base(barManager, null) {
			this.containerLink = containerLink;
		}
		protected override LinksNavigation CreateLinksNavigator() {
			return new QuickCustomizingNavigation(this);
		}
		public virtual BarQBarCustomizationItemLink ContainerLink { get { return containerLink; } }
		Size IFormContainedControl.CalcSize(int width, int maxHeight) {
			return CalcSize(CalcMinSize().Width);
		}
		void IFormContainedControl.SetParentForm(ControlForm form) { }
		public override bool IsSubMenu { get { return false; } }
		public override bool IsMultiLine { get { return true; } }
		protected internal override void UpdateVisibleLinks() {
			base.UpdateVisibleLinks();
			UpdateContainerLinkVisibleLinks();
		}
		protected virtual void UpdateContainerLinkVisibleLinks() {
			if(ContainerLink == null) return;
			ContainerLink.Item.OnGetItemData();
			Manager.Helper.RecentHelper.ClearInternalGroups(ContainerLink.VisibleLinks);
			ContainerLink.ReallyVisibleLinks = Manager.Helper.RecentHelper.BuildVisibleLinksList(ContainerLink.VisibleLinks);
			if(Manager.CanShowNonRecentItems) {
				AddVisibleLinks(VisibleLinks, ContainerLink.VisibleLinks);
			}
			else {
				AddVisibleLinks(VisibleLinks, ContainerLink.ReallyVisibleLinks);
				Manager.Helper.RecentHelper.CreateInternalGroups(ContainerLink.VisibleLinks, ContainerLink.ReallyVisibleLinks);
			}
			SetLinksOwner(this);
		}
		public override bool CanCloseByTimer { get { return true; } }
		public override BarItemLink OwnerLink { get { return ContainerLink; } }
		public override Rectangle PopupOwnerRectangle { get { return ContainerLink.SourceRectangle; } }
		public override bool CanOpenAsChild(IPopup popup) {
			CustomLinksControl sub = popup as CustomLinksControl;
			if(VisibleLinks.Contains(popup.OwnerLink)) return true;
			return false;
		}
	}
}
