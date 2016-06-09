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
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Utils;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Persistent;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.MessageFilter;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.InternalItems;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Native {
	public class BarManagerHelper {
		public static void Activate(BarManager manager) { manager.Active = true; }
		public static void Deactivate(BarManager manager) { manager.Active = false; }
	}
}
namespace DevExpress.XtraBars.Utils {
	public class BarManagerHelpers : IDisposable {
		BarManager manager;
		BarCustomizationManager customizationManager;
		BarMdiHelper mdiHelper;
		BarLinkDragManager dragManager;
		DevExpress.Utils.Win.IPopupServiceControl barPopupController;
		BarManagerRecentHelper recentHelper;
		BarManagerDockingHelper dockingHelper;
		BarManagerLoadHelper loadHelper;
		public BarManagerHelpers(BarManager manager) {
			this.manager = manager;
			this.loadHelper = new BarManagerLoadHelper(Manager);
			this.mdiHelper = new BarMdiHelper(Manager);
			this.dockingHelper = new BarManagerDockingHelper(Manager);
			this.recentHelper = new BarManagerRecentHelper(Manager);
			this.dragManager = CreateLinkDragManager();
			this.customizationManager = CreateCustomizationManager();
			this.barPopupController = new BarPopupController(Manager);
		}
		public virtual void Dispose() {
			if(CustomizationManager != null) CustomizationManager.Dispose();
			if(DragManager != null) DragManager.Dispose();
			if(MdiHelper != null) MdiHelper.Dispose();
			this.mdiHelper = null;
			this.dragManager = null;
			this.customizationManager = null;
		}
		protected virtual BarCustomizationManager CreateCustomizationManager() { return new BarCustomizationManager(Manager); }
		protected virtual BarLinkDragManager CreateLinkDragManager() { return new BarLinkDragManager(Manager); }
		public virtual BarManagerLoadHelper LoadHelper { get { return loadHelper; } }
		public virtual BarMdiHelper MdiHelper { get { return mdiHelper; } }
		public virtual BarManager Manager { get { return manager; } }
		public virtual BarManagerDockingHelper DockingHelper { get { return dockingHelper; } }
		public virtual BarManagerRecentHelper RecentHelper { get { return recentHelper; } }
		public virtual BarCustomizationManager CustomizationManager { get { return customizationManager; } }
		public virtual BarLinkDragManager DragManager { get { return dragManager; } }
		public virtual BarManagerPaintStyle PaintStyle { get { return Manager.PaintStyle; } }
		public BarLinkPainter GetLinkPainter(Type linkType) {
			BarItemInfo itemInfo = PaintStyle.ItemInfoCollection[linkType];
			if(itemInfo == null) return null;
			return itemInfo.LinkPainter;
		}
		public CustomPainter GetControlPainter(Type barControlType) {
			BarControlInfo controlInfo = PaintStyle.BarInfoCollection.GetInfoByItem(barControlType);
			if(controlInfo == null) return null;
			return controlInfo.Painter;
		}
		public virtual BarItemLink CreateLink(BarItemLinkReadOnlyCollection links, BarItem item, object linkedObject) {
			BarItemInfo itemInfo = PaintStyle.ItemInfoCollection.GetInfoByItem(item.GetType());
			if(itemInfo == null) throw new Exception(item.GetType() + " itemInfo not found");
			return itemInfo.CreateLink(links, item, linkedObject);
		}
		public virtual BarLinkViewInfo CreateLinkViewInfo(BarItemLink link) {
			if(link == null) throw new ArgumentException("BarItemLink link");
			BarItemInfo itemInfo = PaintStyle.ItemInfoCollection[link.GetType()];
			if(itemInfo == null) throw new Exception(link.GetType() + " viewInfo not found");
			return itemInfo.CreateViewInfo(link);
		}
		public virtual CustomViewInfo CreateControlViewInfo(Control control) {
			if(control == null) throw new ArgumentException("Control control");
			BarControlInfo controlInfo = PaintStyle.BarInfoCollection.GetInfoByItem(control.GetType());
			if(controlInfo == null) throw new Exception(control.GetType() + " viewInfo not found");
			return controlInfo.CreateViewInfo(Manager, control);
		}
		public virtual DevExpress.XtraEditors.Repository.RepositoryItem CreateLinkEditor(string name) {
			return Manager.RepositoryItems.Add(name);
		}
		public DevExpress.XtraEditors.BaseEdit GetLinkEditor(DevExpress.XtraEditors.Repository.RepositoryItem item) {
			return Manager.EditorHelper.GetEditor(item);
		}
		public virtual DevExpress.XtraEditors.Drawing.BaseEditPainter GetLinkEditorPainter(DevExpress.XtraEditors.Repository.RepositoryItem item) {
			return Manager.EditorHelper.GetPainter(item);
		}
	}
	public class BarAnimatedItemsHelper {
		public static EditorAnimationInfo GetEditorAnimation(ISupportXtraAnimation ctrl, BarEditItemLink link) {
			return XtraAnimator.Current.Get(ctrl, link) as EditorAnimationInfo;
		}
		public static void UpdateAnimatedViewInfo(EditorAnimationInfo info, BaseEditViewInfo vi) {
			IAnimatedItem item = vi as IAnimatedItem;
			if(item == null) return;
			info.ViewInfo = vi;
			item.OnStart();
		}
		public static void UpdateAnimatedItem(ISupportXtraAnimation ctrl, CustomAnimationInvoker method, RibbonEditItemViewInfo vi) {
			if (vi == null) return;
			IAnimatedItem item = vi.EditViewInfo as IAnimatedItem;
			if(item == null || !item.IsAnimated) return;
			BarEditItemLink link = vi.Item as BarEditItemLink;
			EditorAnimationInfo info = GetEditorAnimation(ctrl, link);
			if(info != null) info.ViewInfo = vi.EditViewInfo;
			AddAnimatedItem(ctrl, method, vi, link);
		}
		public static void AddAnimatedItem(ISupportXtraAnimation ctrl, CustomAnimationInvoker method, BaseEditViewInfo vi, BarEditItemLink link) {
			IAnimatedItem item = vi as IAnimatedItem;
			if(item == null || item.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(link, vi, ctrl, vi as IAnimatedItem, method);
			item.OnStart();
		}
		public static void AddAnimatedItem(ISupportXtraAnimation ctrl, CustomAnimationInvoker method, BarEditLinkViewInfo vi, BarEditItemLink link) {
			AddAnimatedItem(ctrl, method, vi.EditViewInfo, link);
		}
		public static void AddAnimatedItem(ISupportXtraAnimation ctrl, CustomAnimationInvoker method, RibbonEditItemViewInfo vi, BarEditItemLink link) {
			AddAnimatedItem(ctrl, method, vi.EditViewInfo, link);
		}
		public static void Animate(Control ownerControl, BarManager manager, BaseAnimationInfo info, CustomAnimationInvoker method) {
			EditorAnimationInfo editorInfo = info as EditorAnimationInfo;
			if(editorInfo == null) return;
			IAnimatedItem animItem = editorInfo.ViewInfo as IAnimatedItem;
			if(animItem == null) return;
			if(manager == null || manager.SelectionInfo == null) {
				XtraAnimator.RemoveObject(ownerControl as ISupportXtraAnimation, info.AnimationId);
				animItem.OnStop();
				return;
			}
			if(ShouldAnimate(editorInfo, animItem, manager)) {
				animItem.UpdateAnimation(info);
				ownerControl.Invalidate(animItem.AnimationBounds);
			}
		}
		static bool ShouldAnimate(EditorAnimationInfo info, IAnimatedItem animItem, BarManager manager) {
			if(manager.SelectionInfo.ActiveEditor == null) return true;
			BarItemLink link = info.Link as BarItemLink;
			if(link == null) return true;
			if(link.BarControl != null) {
				if(link.BarControl != manager.SelectionInfo.ActiveBarControl) return true;
				return !manager.SelectionInfo.ActiveEditor.Bounds.IntersectsWith(animItem.AnimationBounds);
			}
			if(manager.SelectionInfo.ActiveEditor.Parent is RibbonStatusBar) {
				if(!(link.Links is RibbonStatusBarItemLinkCollection)) return true;
			}
			if(link.Links is RibbonStatusBarItemLinkCollection) return true;
			return !manager.SelectionInfo.ActiveEditor.Bounds.IntersectsWith(animItem.AnimationBounds);
		}
		static void StartAnimation(ISupportXtraAnimation ctrl, EditorAnimationInfo info, CustomAnimationInvoker method) {
			XtraAnimator.Current.AddEditorAnimation(info.Link, info.ViewInfo, ctrl, info.ViewInfo as IAnimatedItem, method);
		}
		static void StopAnimation(ISupportXtraAnimation ctrl, EditorAnimationInfo info) {
			XtraAnimator.RemoveObject(ctrl, info.Link);
		}
	}
	public class BarManagerLoadHelper {
		BarManager manager;
		bool loaded;
		public BarManagerLoadHelper(BarManager manager) {
			this.loaded = false;
			this.manager = manager;
		}
		public bool Loaded { 
			get { return loaded; } 
			set { loaded = value; }
		}
		public BarManager Manager { get { return manager; } }
		public virtual void Load() {
			if(Loaded) {
				Manager.LoadPostponedRepositories();
				return;
			}
			Manager.lockFireChanged ++;
			try {
				Manager.Helper.DockingHelper.CheckDockControlsAdded();
				LoadRepositories();
				LoadCore();
				if(Manager.AutoSaveInRegistry && !Manager.IsDesignMode) {
					if(Manager.DockManager != null) 
						Manager.DockManager.ResumeOnLoad();
					Manager.RestoreFromRegistry();
				}
				else 
					if(Manager.DockManager != null ) Manager.DockManager.ResumeOnLoad();
			}
			finally {
				Manager.lockFireChanged --;
			}
		}
		protected virtual void LoadRepositories() {
			Manager.LoadRepositories();
		}
		void UpdateDockScheme() {
			foreach(BarDockControl dock in Manager.DockControls) {
				dock.BeginUpdate();
				try {
					dock.UpdateScheme();
				} finally {
					dock.CancelUpdate();
				}
			}
		}
		protected virtual void LoadCore() {
			try {
				List<BarItem> items = CloneBarItemCollection(Manager.Items);
				foreach(BarItem item in items) {
					item.AfterLoad();
				}
				foreach(Bar bar in Manager.Bars) {
					if(bar.Text == "") bar.Text = bar.BarName;
					if(bar.postMainMenu) Manager.MainMenu = bar;
					if(bar.postStatusBar) Manager.StatusBar = bar;
					bar.SetCurrentStateAsDefault();
				}
				foreach(BarLinksHolder holder in Manager.ItemHolders) {
					if(holder is PopupMenuBase) {
						(holder as PopupMenuBase).CreateLinksFromLinksInfo();
					}
				}
				Manager.Categories.OnLoaded();
				UpdateDockScheme();
				Manager.ForceBaseOnLoaded();
			}
			finally {
				Manager.Helper.RecentHelper.ClearRecentItems();
				Manager.Helper.RecentHelper.UpdateRecentItems();
				this.loaded = true;
			}
			try {
				Manager.Helper.DockingHelper.InUpdateDocking = true;
				Manager.Helper.DockingHelper.UpdateBarDocking();
			}
			finally {
				Manager.Helper.DockingHelper.InUpdateDocking = false;
			}
			Manager.Helper.DockingHelper.CheckUpdateDockingOrder();
		}
		protected internal List<BarItem> CloneBarItemCollection(BarItems items) {
			List<BarItem> list = new List<BarItem>();
			foreach(BarItem item in items) {
				list.Add(item);
			}
			return list;
		}
	}
	public class BarManagerRecentHelper {
		BarManager manager;
		public BarManagerRecentHelper(BarManager manager) {
			this.manager = manager;
		}
		public BarManager Manager { get { return manager; } }
		class MyRecentLinkComparer : IComparer {
			IList visibleList;
			bool useVisibleIndex;
			internal MyRecentLinkComparer(IList visibleList, bool useVisibleIndex) {
				this.visibleList = visibleList;
				this.useVisibleIndex = useVisibleIndex;
			}
			int GetCompareValue(BarItemLink link) {
				if(useVisibleIndex) {
					return visibleList.IndexOf(link);
				}
				return link.RecentIndex;
			}
			int IComparer.Compare(object a, object b) {
				int v1 = GetCompareValue(a as BarItemLink), 
					v2 = GetCompareValue(b as BarItemLink);
				if(v1 == v2) return 0;
				if(v1 < 0) return 1;
				if(v2 < 0) return 0;
				return v1.CompareTo(v2);
			}
		}
		public virtual void ClearInternalGroups(ICollection visibleLinks) {
			foreach(BarItemLink link in visibleLinks) link.internalBeginGroup = false;
		}
		public virtual void CreateInternalGroups(IList visibleLinks, IList reallyVisibleLinks) {
			int vIndex = 0;
			foreach(BarItemLink link in visibleLinks) {
				bool beginGroup = link.BeginGroup;
				if(reallyVisibleLinks.Contains(link)) {
					beginGroup = false;
					vIndex = reallyVisibleLinks.IndexOf(link) + 1;
					continue;
				}
				if(vIndex >= reallyVisibleLinks.Count) break;
				if(beginGroup)
					(reallyVisibleLinks[vIndex++] as BarItemLink).internalBeginGroup = true;
				beginGroup = false;
			}
		}
		public ArrayList BuildRecentList(IList visibleLinks) {
			ArrayList res = new ArrayList();
			res.AddRange(visibleLinks);
			res.Sort(new MyRecentLinkComparer(visibleLinks, false));
			return res;
		}
		public ArrayList BuildVisibleLinksList(IList visibleLinks) {
			ArrayList res = new ArrayList(), recentList = BuildRecentList(visibleLinks);
			int recentCount = GetMostRecentItemCount(visibleLinks, recentList);
			if(recentCount == -1) {
				res.AddRange(visibleLinks);
				return res;
			}
			for(int n = 0; n < recentCount; n++) {
				res.Add(recentList[n]);
			}
			res.Sort(new MyRecentLinkComparer(visibleLinks, true));
			return res;
		}
		protected int GetMostRecentItemCount(IList visibleLinks, IList recentList) { 
			int min, max, level, n;
			min = int.MaxValue;
			max = 0;
			for(n = 0; n < visibleLinks.Count; n++) {
				BarItemLink link = visibleLinks[n] as BarItemLink;
				if(link.ClickCount < min) min = link.ClickCount;
				if(link.ClickCount > max) max = link.ClickCount;
			}
			level = min + ((max - min) * (100 - Manager.GetController().PropertiesBar.MostRecentItemsPercent)) / 100;
			int maxValidRecentIndex = -1;
			for(n = recentList.Count - 1; n >= 0; n--) {
				BarItemLink link = recentList[n] as BarItemLink;
				if(link.ClickCount >= level) {
					maxValidRecentIndex = n;
					break;
				}
			}
			maxValidRecentIndex ++;
			if(maxValidRecentIndex == recentList.Count) return -1;
			if(maxValidRecentIndex == 0) return 1;
			return maxValidRecentIndex;
		}
		public virtual void ClearRecentItems() {
			foreach(BarItem item in Manager.Items) {
				BarSubItem subItem = item as BarSubItem;
				if(subItem == null) continue;
				subItem.ItemLinks.ResetRecentIndexes();
				subItem.ItemLinks.ClearRecentIndexes();
			}
		}
		public virtual void UpdateRecentItems() {
			foreach(BarItem item in Manager.Items) {
				BarSubItem subItem = item as BarSubItem;
				if(subItem == null) continue;
				int n = 0;
				for(n = subItem.ItemLinks.Count - 1; n >= 0; n--) {
					subItem.ItemLinks.UpdateMostRecentlyUsed(subItem.ItemLinks[n], false);
				}
				for(n = 0; n < subItem.ItemLinks.Count; n++) {
					for(int k = 0; k < subItem.ItemLinks.Count; k++) {
						BarItemLink link = subItem.ItemLinks[k];
						if(link.RecentIndex == n)
							subItem.ItemLinks.UpdateRecentIndex(link, n, false, false);
					}
				}
				subItem.ItemLinks.UpdateRecentIndexes();
			}
		}
	}
	public class BarManagerDockingHelper {
		BarManager manager;
		bool inUpdateDocking;
		public BarManagerDockingHelper(BarManager manager) {
			this.manager = manager;
			this.inUpdateDocking = false;
		}
		public bool InUpdateDocking { 
			get { return inUpdateDocking; } set { inUpdateDocking = value; }
		}
		internal static void ResumeComponentNotifications(bool isNotificationsEnabled, Control form) {
			if(form == null) return;
			ResumeComponentNotifications(isNotificationsEnabled, form.Site);
		}
		internal static void ResumeComponentNotifications(bool isNotificationsEnabled, ISite site) {
			bool current = EnableComponentNotifications(isNotificationsEnabled, site);
			if(current && !isNotificationsEnabled) ResumeComponentNotifications(true, site);
		}
		internal static void ResumeComponentNotifications(bool isNotificationsEnabled, BarManager manager) {
			if (manager.Site != null)
				ResumeComponentNotifications(isNotificationsEnabled, manager.Site);
			else
				ResumeComponentNotifications(isNotificationsEnabled, manager.Form);
		}
		public static bool EnableComponentNotifications(bool newValue, Control form) {
			if(form == null) return false;
			return EnableComponentNotifications(newValue, form.Site);
		}
		public static bool EnableComponentNotifications(bool newValue, BarManager manager) {
			if(manager.Site != null) return EnableComponentNotifications(newValue, manager.Site);
			return EnableComponentNotifications(newValue, manager.Form);
		}
		public static bool EnableComponentNotifications(bool newValue, ISite site) {
			if(site == null) return false;
			IDesignerHost host = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return false;
			IDesignerLoaderService srv = host.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
			if(srv == null) return false;
			MethodInfo mi = srv.GetType().GetMethod("EnableComponentNotification", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null) return (bool)mi.Invoke(srv, new object[] { newValue });
			return false;
		}
		public BarManager Manager { get { return manager; } }
		internal class DockableObjectSorter : IComparer {
			BarManager manager;
			public DockableObjectSorter(BarManager manager) {
				this.manager = manager;
			}
			int IComparer.Compare(object a, object b) {
				IDockableObject b1 = a as IDockableObject;
				IDockableObject b2 = b as IDockableObject;
				if(b1.DockInfo.DockRow == b2.DockInfo.DockRow) {
					if(b1.DockInfo.DockCol == b2.DockInfo.DockCol) {
						int bar1OrigPos = GetBarOriginalPos(b1);
						int bar2OrigPos = GetBarOriginalPos(b2);
						if(bar1OrigPos == -1 || bar2OrigPos == -1) return 0;
						return -1 * bar1OrigPos.CompareTo(bar2OrigPos);
					}
					if(b1.DockInfo.DockCol == -1) return 1;
					if(b2.DockInfo.DockCol == -1) return -1;
					return b1.DockInfo.DockCol.CompareTo(b2.DockInfo.DockCol);
				}
				if(b1.DockInfo.DockRow == -1) return 1;
				if(b2.DockInfo.DockRow == -1) return -1;
				return b1.DockInfo.DockRow.CompareTo(b2.DockInfo.DockRow);
			}
			int GetBarOriginalPos(IDockableObject obj) {
				if(manager == null) return -1;
				return obj is Bar ? manager.Bars.IndexOf(obj as Bar) : -1;
			}
		}
		protected internal virtual void UpdateDockableObjects(ArrayList list) {
			Hashtable hash = new Hashtable();
			foreach(IDockableObject dock in list) {
				if(dock.DockInfo.DockRow < 0 || !dock.IsVisible) continue;
				BarDockStyle dockStyle = dock.DockStyle;
				if(hash.ContainsKey(dockStyle)) {
					hash[dockStyle] = Math.Min((int)hash[dockStyle], dock.DockInfo.DockRow);
				} else {
					hash[dockStyle] = dock.DockInfo.DockRow;
				}
			}
			foreach(IDockableObject dock in list) {
				if(dock.DockInfo.DockRow < 0 || !dock.IsVisible) continue;
				BarDockStyle dockStyle = dock.DockStyle;
				if(!hash.ContainsKey(dockStyle)) continue;
				int minRow = (int)hash[dockStyle];
				dock.DockInfo.UpdateDockPosition(dock.DockInfo.DockRow - minRow, dock.DockInfo.DockCol);
			}
		}
		bool suppressApplyDockRowCol = false;
		protected internal bool SuppressApplyDockRowCol { get { return suppressApplyDockRowCol; } set { suppressApplyDockRowCol = value; } }
		public virtual void UpdateBarDocking() {
			if(!Manager.DockingEnabled) return;
			bool prev = EnableComponentNotifications(false, Manager);
			try {
				if(Manager.Form == null && Manager.DockControls.Count > 0) Manager.Form = Manager.DockControls[0].Parent;
				if(Manager.Form != null && !Manager.IsDesignMode) Manager.Form.SuspendLayout();
				try {
					foreach(IDockableObject dock in Manager.Bars) {
						if(dock.DockControl != null) {
							dock.DockControl.RemoveDockable(dock, false);
						}
					}
					SuppressApplyDockRowCol = true;
					if(!Manager.AllowMoveBarOnToolbar) ResetBarOffsets();
					SuppressApplyDockRowCol = false;
					ArrayList list = GetDockableObjects();
					UpdateDockableObjects(list);
					foreach(IDockableObject dockObject in list) {
						dockObject.ApplyDocking(dockObject.DockInfo);
					}
				}
				finally {
					if(Manager.Form != null && !Manager.IsDesignMode) Manager.Form.ResumeLayout();
				}
			}
			finally {
				ResumeComponentNotifications(prev, Manager);
			}
		}
		protected internal ArrayList GetDockableObjects() {
			ArrayList list = new ArrayList();
			list.AddRange(Manager.Bars);
			list.Sort(new DockableObjectSorter(Manager));
			return list;
		}
		protected virtual void InitializeDesignTimeStandaloneControls() {
			if(!Manager.IsDesignMode) return;
			foreach(object comp in Manager.Container.Components) {
				StandaloneBarDockControl control = comp as StandaloneBarDockControl;
				if(control == null || (control.Manager != null && control.Manager != Manager)) continue;
				if(!Manager.DockControls.Contains(control)) Manager.DockControls.Add(control);
				control.InitDockControl(Manager, BarDockStyle.Standalone);
			}
		}
		public virtual void CreateDefaultDockControls() {
			if(!Manager.DockingEnabled) return;
			if(Manager.Form != null) {
				BarDockStyle[] ds = new BarDockStyle[] { BarDockStyle.Top, BarDockStyle.Bottom, BarDockStyle.Left, BarDockStyle.Right};
				while(Manager.DockControls.Count < 4) {
					int n = Manager.DockControls.Count;
					BarDockControl dk = new BarDockControl();
					if(Manager.IsDesignMode && Manager.Form.Container != null) {
						try {
							Manager.Form.Container.Add(dk, "barDockControl" + ds[n].ToString());
						}
						catch {
							Manager.Form.Container.Add(dk);
						}
					}
					Manager.DockControls.Add(dk);
				}
				try {
					Manager.initCount ++;
					for(int n = 0; n < Manager.DockControls.Count; n++) {
						BarDockControl dk = Manager.DockControls[n] as BarDockControl;
						if(n < 4) dk.InitDockControl(Manager, ds[n]);
						else dk.InitDockControl(Manager, BarDockStyle.Standalone);
					}
					InitializeDesignTimeStandaloneControls();
				}
				finally {
					Manager.initCount --;
				}
				CheckUpdateDockingOrder();
			}
		}
		public void CheckDockControlsAdded() {
			if(!Manager.DockingEnabled) return;
			if(Manager.Form == null)
				return;
			bool changed = false;
			for(int n = 0; n < Manager.DockControls.Count; n++) {
				BarDockControl dk = Manager.DockControls[n] as BarDockControl;
				if(n < 4) {
					if(!Manager.Form.Contains(dk)) {
						Manager.Form.Controls.Add(dk);
						changed = true;
					}
				}
			}
			if(changed) CheckUpdateDockingOrder();
		}
		void UpdateDockingOrderCore() {
			if(inUpdateDocking) return;
			inUpdateDocking = true;
			try {
				if(Manager.DockControls.Count >= 4) {
					if(Manager.DockControls[BarDockStyle.Left] != null)
						Manager.DockControls[BarDockStyle.Left].UpdateDockingCore();
					if(Manager.DockControls[BarDockStyle.Right] != null)
						Manager.DockControls[BarDockStyle.Right].UpdateDockingCore();
					if(Manager.DockControls[BarDockStyle.Bottom] != null)
						Manager.DockControls[BarDockStyle.Bottom].UpdateDockingCore();
					if(Manager.DockControls[BarDockStyle.Top] != null)
						Manager.DockControls[BarDockStyle.Top].UpdateDockingCore();
				}
			}
			finally {
				inUpdateDocking = false;
			}
		}
		void UpdateDockingSize() {
			if(inUpdateDocking) return;
			bool prev = EnableComponentNotifications(false, Manager);
			if(Manager.Form != null) Manager.Form.SuspendLayout();
			try {
				inUpdateDocking = true;
				try {
					if(Manager.DockControls.Count >= 4) {
						if(Manager.DockControls[BarDockStyle.Left] != null)
							Manager.DockControls[BarDockStyle.Left].UpdateDockSize();
						if(Manager.DockControls[BarDockStyle.Right] != null)
							Manager.DockControls[BarDockStyle.Right].UpdateDockSize();
						if(Manager.DockControls[BarDockStyle.Bottom] != null)
							Manager.DockControls[BarDockStyle.Bottom].UpdateDockSize();
						if(Manager.DockControls[BarDockStyle.Top] != null)
							Manager.DockControls[BarDockStyle.Top].UpdateDockSize();
					}
				}
				finally {
					inUpdateDocking = false;
				}
				if(Manager.Form != null) Manager.Form.ResumeLayout();
			}
			finally {
				ResumeComponentNotifications(prev, Manager);
			}
		}
		internal void CheckForceUpdateDockControls() {
			if(IsNeedUpdateDockingOrder())
				UpdateDockingOrderCore();
			else
				UpdateDockingSize();
		}
		internal void CheckUpdateDockingOrder() {
			if(IsNeedUpdateDockingOrder()) UpdateDockingOrderCore();
		}
		internal void ForceUpdateDockingOrder() {
			UpdateDockingOrderCore();
		}
		internal bool IsNeedUpdateDockingOrder() {
			if(Manager.DockControls.Count < 4 || Manager.DockControls[BarDockStyle.Top] == null) return false;
			Control parent = Manager.DockControls[BarDockStyle.Top].Parent;
			if(parent == null) return false;
			int i1 = parent.Controls.IndexOf(Manager.DockControls[BarDockStyle.Top]);
			int i2 = parent.Controls.IndexOf(Manager.DockControls[BarDockStyle.Bottom]);
			int i3 = parent.Controls.IndexOf(Manager.DockControls[BarDockStyle.Left]);
			int i4 = parent.Controls.IndexOf(Manager.DockControls[BarDockStyle.Right]);
			int count = parent.Controls.Count - 1;
#if DXWhidbey
			if(i1 != count || i2 != count - 1 || i4 != count - 2 || i3 != count - 3) return true;
#else
			if(i4 != count || i3 != count - 1 || i2 != count - 2 || i1 != count - 3) return true;
#endif
			return false;
		}
		protected virtual bool ShouldRemove(BarDockControl dock) {
			return !(dock is StandaloneBarDockControl);
		}
		public virtual void RemoveDefaultDockControls() {
			for(int n = Manager.DockControls.Count - 1; n >= 0; n--) {
				BarDockControl dock = Manager.DockControls[n];
				if(!ShouldRemove(dock)) {
					dock.InitDockControl(null, BarDockStyle.Standalone);
					continue;
				}
				dock.AllowDispose = true;
				if(Manager.Form != null && Manager.Form.Container != null)
					Manager.Form.Container.Remove(dock);
				if(dock.IsHandleCreated)
					dock.BeginInvoke(new MethodInvoker(dock.Dispose));
				else 
				dock.Dispose();
			}
		}
		protected virtual void ResetBarOffsets() {
			foreach(Bar bar in Manager.Bars) {
				bar.Offset = 0;
			}
		}
	}
	public class BarMdiHelper : IDisposable {
		BarManager manager;
		BarSystemMenuItem systemMenuItem;
		BarMdiButtonItem[] systemButtonItems;
		Form prevActiveMdiChild = null;
		bool isMdiMaximized = false, isShowSystemMenu = false;
		Form mdiDocumentForm = null;
		public BarMdiHelper(BarManager manager) {
			this.manager = manager;
		}
		public Form PrevActiveMdiChild { get { return prevActiveMdiChild; } }
		public virtual void Init() {
			this.systemMenuItem = new BarSystemMenuItem(Manager);
			BarMdiButtonItem.SystemItemType[] itemTypes = new BarMdiButtonItem.SystemItemType[] {
																										  BarMdiButtonItem.SystemItemType.Minimize, BarMdiButtonItem.SystemItemType.Restore,
																										  BarMdiButtonItem.SystemItemType.Close};
			this.systemButtonItems = new BarMdiButtonItem[3];
			for(int n = 0; n < 3; n++) {
				systemButtonItems[n] = new BarMdiButtonItem(Manager, itemTypes[n]);
			}
		}
		public virtual BarManager Manager { get { return manager; } }
		public virtual Bar MainMenu { get { return Manager.MainMenu; } }
		public virtual void Dispose() {
		}
		public virtual bool IsShowSystemMenu { get { return isShowSystemMenu; } 
		}
		protected internal virtual void AddSystemLinks(Bar bar, BarItemLinkReadOnlyCollection links) {
			if(systemMenuItem.GetChildSystemMenu(mdiDocumentForm) == IntPtr.Zero) return;
			systemMenuItem.MdiChild = mdiDocumentForm;
			BarItemLink link = systemMenuItem.CreateLink(links, bar);
			if(links.Count > 0)
				links.InsertItem(0, link);
			else
				links.AddItem(link);
			AddSystemButtons(mdiDocumentForm, bar, links);
		}
		protected virtual void CreateMdiChildSystemMenu(Form childForm) {
			isShowSystemMenu = true;
			if(!DevExpress.Utils.Mdi.MdiClientSubclasser.IsShowSystemMenu(manager.GetForm())) {
				isShowSystemMenu = false;
			}
			mdiDocumentForm = childForm;
			if(MainMenu == null) return;
			MainMenu.OnBarChanged();
		}
		protected virtual void DestroyMdiChildSystemMenu() {
			this.mdiDocumentForm = null;
			this.isShowSystemMenu = false;
			if(MainMenu == null) return;
			this.systemMenuItem.MdiChild = null;
			MainMenu.OnBarChanged();
		}
		protected virtual void RemoveSystemButtons() {
			if(MainMenu == null) return;
			while(MainMenu.ItemLinks.Count > 0) {
				BarMdiButtonItemLink link = MainMenu.ItemLinks[MainMenu.ItemLinks.Count - 1] as BarMdiButtonItemLink;
				if(link != null) {
					(link.Item as BarMdiButtonItem).ChildForm = null;
					MainMenu.RemoveLink(link);
				} else 
					break;
			}
		}
		string[] images = new String[] {"MinImage", "RestoreImage", "CloseImage"};
		public virtual void UpdateSystemButtons() {
			if(systemButtonItems == null || systemButtonItems.Length < 3) return;
			for(int n = 0; n < 3; n++) {
				Manager.PaintStyle.DrawParameters.UpdateMdiGlyphs(systemButtonItems[n], images[n]);
			}
		}
		protected internal BarMdiButtonItem[] SystemButtonItems { get { return systemButtonItems; } }
		protected virtual void AddSystemButtons(Form childForm, Bar bar, BarItemLinkReadOnlyCollection links) {
			for(int n = 0; n < 3; n++) {
				systemButtonItems[n].ChildForm = childForm;
				Manager.PaintStyle.DrawParameters.UpdateMdiGlyphs(systemButtonItems[n], images[n]);
				bool enabled = true;
				switch(n) {
					case 0: enabled = (childForm.ControlBox ? childForm.MinimizeBox : false); break;
					case 1: enabled = (childForm.ControlBox ? childForm.MaximizeBox : false); break;
					case 2: enabled = childForm.ControlBox; break;
				}
				systemButtonItems[n].Enabled = enabled;
			}
			links.AddItem(systemButtonItems[0].CreateLink(links, bar));
			links.AddItem(systemButtonItems[1].CreateLink(links, bar));
			links.AddItem(systemButtonItems[2].CreateLink(links, bar));
		}
		BarManager mergedManager = null;
		bool prevMainMenuVisible = false;
		protected internal BarManager MergedManager { get { return mergedManager; } }
		public virtual void MergeManager(BarManager manager) {
			if(manager == null) return;
			if(MergedManager == manager) return;
			manager.MergedOwner = Manager;
			if(Manager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.Never) {
				UnMergeManager();
				return;
			}
			Manager.BeginUpdate();
			try {
				if(MergedManager != null) {
					UnMergeManager();
				}
				if(MainMenu != null && manager.MainMenu != null) {
					this.prevMainMenuVisible = manager.MainMenu.Visible;
					if(prevMainMenuVisible || Manager.HideBarsWhenMerging) {
						if(Manager.AllowMergeInvisibleLinks)
							MainMenu.ItemLinks.Merge(manager.MainMenu.ItemLinks);
						else
						MainMenu.ItemLinks.Merge(manager.MainMenu.VisibleLinks);
						manager.UpdateMainMenuVisibility();
					}
				}
				Manager.RaiseMerge(new BarManagerMergeEventArgs(manager));
			}
			finally {
				this.mergedManager = manager;
				Manager.EndUpdate();
			}
		}
		public virtual void UnMergeManager() {
			if(MergedManager == null) return;
			MergedManager.MergedOwner = null;
			Manager.SelectionInfo.OnCloseAll(BarMenuCloseType.All);
			Manager.BeginUpdate();
			try {
				for(int n = Manager.Bars.Count - 1; n >= 0; n--) {
					Bar bar = Manager.Bars[n];
					bar.UnMerge();
				}
				for(int n = Manager.Items.Count - 1; n >= 0; n--) {
					BarLinksHolder holder = Manager.Items[n] as BarLinksHolder;
					if(holder != null) holder.ItemLinks.UnMerge();
				}
				MergedManager.UpdateMainMenuVisibility();
				Manager.RaiseUnMerge(new BarManagerMergeEventArgs(MergedManager));
			}
			finally {
				this.mergedManager = null;
				Manager.EndUpdate();
			}
		}
		internal bool IsMaximized(Form frm) {
			if(frm == null) return false;
			if(!frm.IsHandleCreated) {
				return frm.WindowState == FormWindowState.Maximized;
			}
			return BarNativeMethods.IsZoomed(frm.Handle);
		}
		Form GetActiveMdiChild(Form activeForm) {
			Form activeMdiChild = Manager.ActiveMdiChild;
			Form frm = Manager.Form as Form;
			if(activeForm != null) activeMdiChild = activeForm;
			if(activeMdiChild == null && frm.MdiChildren.Length > 0) {
				foreach(Form f in frm.MdiChildren) {
					if(f.Visible && IsMaximized(f)) {
						activeMdiChild = f;
						break;
					}
				}
			}
			return activeMdiChild;
		}
		const int STATE_CREATINGHANDLE = 0x00040000;
		bool IsControlCreated(Control ctrl) {
			if(ctrl == null || !ctrl.IsHandleCreated) return false;
			MethodInfo mi = typeof(Control).GetMethod("GetState", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic);
			if(mi != null) {
				return !(bool)mi.Invoke(ctrl, new object[] { STATE_CREATINGHANDLE });
			}
			return true;
		}
		int lockCheckMdi = 0;
		public void DoCheckMdi(Form activeForm) { DoCheckMdi(activeForm, false); }
		public virtual void DoCheckMdi(Form activeForm, bool force) {
			if(this.lockCheckMdi != 0 || Manager.IsDesignMode) return;
			if(!(Manager.Form is Form)) return;
			Form activeMdiChild = GetActiveMdiChild(activeForm);
			bool prevIsMdiMaximized = this.isMdiMaximized;
			this.isMdiMaximized = activeMdiChild != null && IsMaximized(activeMdiChild);
			if(this.prevActiveMdiChild == activeMdiChild && this.isMdiMaximized == prevIsMdiMaximized) return;
			if(!force && activeMdiChild != null && !IsControlCreated(activeMdiChild)) return;
			if(systemMenuItem == null)
				Init();
			try {
				this.lockCheckMdi++;
				Manager.BeginUpdate();
				if(this.prevActiveMdiChild != activeMdiChild) {
					DestroyMdiChildSystemMenu();
					if(this.isMdiMaximized) CreateMdiChildSystemMenu(activeMdiChild);
					if(Manager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.Never ||
							(Manager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.OnlyWhenChildMaximized && !this.isMdiMaximized)) {
						UnMergeManager();
						return;
					}
					var documentManager = Docking2010.DocumentManager.FromControl(manager.GetForm());
					if(documentManager == null || !documentManager.CanMergeOnDocumentActivate()) {
						if(activeMdiChild == null) { 
							UnMergeManager();
							return;
						}
						BarManager mergeMan = BarManager.FindManager(activeMdiChild);
						if(mergeMan == null)
							UnMergeManager();
						else
							MergeManager(mergeMan);
					}
				}
				else {
					if(this.isMdiMaximized != prevIsMdiMaximized) {
						if(isMdiMaximized) {
							CreateMdiChildSystemMenu(activeMdiChild);
						}
						else {
							DestroyMdiChildSystemMenu();
						}
						if(Manager.MdiMenuMergeStyle == BarMdiMenuMergeStyle.OnlyWhenChildMaximized) {
							if(this.isMdiMaximized)
								MergeManager(BarManager.FindManager(activeMdiChild));
							else
								UnMergeManager();
						}
					}
				}
			}
			finally {
				this.lockCheckMdi--;
				this.prevActiveMdiChild = activeMdiChild;
				Manager.EndUpdate();
			}
		}
	}
}
