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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Controls;
using System.Drawing.Design;
namespace DevExpress.Utils.Menu {
	public interface IDXDropDownControl {
		bool Visible { get; }
		void Show(IDXMenuManager manager, Control control, Point pos);
		void Hide();
	}
	public interface IDXDropDownControlEx : IDXDropDownControl {
		void Show(Control control, Point pos);
		event EventHandler Popup;
		event EventHandler CloseUp;
	}
	public interface IDXManagerPopupMenu { 
	}
	public interface IDesignTimeTopForm {
	}
	public interface IDXMenuManager {
		void ShowPopupMenu(DXPopupMenu menu, Control control, Point pos);
		IDXMenuManager Clone(Form newForm);
		void DisposeManager();
	}
	public interface IDXMenuShowTracker {
		void StartTracking();
		bool Track();
		void StopTracking();
	}
	public interface IDXMenuShowContext : IDisposable {
		bool Shown { get; }
	}
	public interface IDXDropDownMenuManager : IDXMenuManager {
		object ShowDropDownMenu(DXPopupMenu menu, Control control, Point pos);
	}
	public interface IDXPopupMenuForm { 
	}
	public interface IDXMenuManagerProvider {
		IDXMenuManager MenuManager { get; }
	}
	public class MenuManagerHelper {
		#region Find MenuManager
		static System.Collections.Generic.IList<Func<Control, IDXMenuManager>> findRoutines;
		public static void RegisterFindRoutine(Func<Control, IDXMenuManager> routine) {
			if(findRoutines == null)
				findRoutines = new System.Collections.Generic.List<Func<Control, IDXMenuManager>>();
			findRoutines.Add(routine);
		}
		public static IDXMenuManager FindMenuManager(Control container) {
			if(findRoutines == null) return null;
			for(int i = 0; i < findRoutines.Count; i++) {
				IDXMenuManager containerManager = findRoutines[i](container);
				if(containerManager != null)
					return containerManager;
			}
			return null;
		}
		#endregion Find MenuManager
		public static StandardMenuManager Standard { get { return StandardMenuManager.Default; } }
		public static StandardExMenuManager StandardEx { get { return StandardExMenuManager.Default; } }
		public static Office2003MenuManager Office2003 { get { return Office2003MenuManager.Default; } }
		public static IDXMenuManager GetMenuManager(UserLookAndFeel lookAndFeel) {
			return StandardEx;
		}
		static DefaultBoolean useParentMenuManagerCore = DefaultBoolean.Default;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static DefaultBoolean UseParentMenuManager {
			get { return useParentMenuManagerCore; }
			set { useParentMenuManagerCore = value; } 
		}
		protected static bool CanUseParentMenuManager(Control control) {
			if(UseParentMenuManager == DefaultBoolean.Default) 
				return (control.ContextMenuStrip == null) && (control.ContextMenu == null);
			return UseParentMenuManager != DefaultBoolean.False;
		}
		public static IDXMenuManager GetMenuManager(UserLookAndFeel lookAndFeel, Control container) {
			while(container != null) {
				IDXMenuManagerProvider provider = container as IDXMenuManagerProvider;
				if(provider != null && provider.MenuManager != null)
					return GetMenuManager(lookAndFeel, provider.MenuManager, container);
				IDXMenuManager containerManager = FindMenuManager(container);
				if(containerManager != null)
					return containerManager;
				container = container.Parent;
			}
			return GetMenuManager(lookAndFeel);
		}
		public static IDXMenuManager GetMenuManager(UserLookAndFeel lookAndFeel, IDXMenuManager menuManager, Control control) {
			if(menuManager == null || (control != null && control.Site != null && control.Site.DesignMode)) {
				if(control != null && CanUseParentMenuManager(control)) 
					return GetMenuManager(lookAndFeel, control.Parent);
				return GetMenuManager(lookAndFeel);
			}
			return menuManager;
		}
		public static void ShowMenu(DXPopupMenu menu, UserLookAndFeel lookAndFeel, IDXMenuManager menuManager, Control control, Point pos) {
			if(control != null && control.Site != null && control.Site.DesignMode)
				GetMenuManager(lookAndFeel).ShowPopupMenu(menu, control, pos);
			else
				GetMenuManager(lookAndFeel, menuManager, control).ShowPopupMenu(menu, control, pos);
		}
	}
	public class DXMenuShowTracker : IDXMenuShowTracker {
		protected DXMenuShowTracker() { }
		protected void TrackShowMenu() {
			if(tracking > 0) shown++;
		}
		#region IDXMenuManagerTracker Members
		int tracking, shown;
		void IDXMenuShowTracker.StartTracking() {
			if(0 == tracking++)
				shown = 0;
		}
		bool IDXMenuShowTracker.Track() {
			return (tracking > 0) && (shown > 0);
		}
		void IDXMenuShowTracker.StopTracking() {
			if(--tracking == 0)
				shown = 0;
		}
		#endregion
		public static IDXMenuShowContext Track(IDXMenuManager manager) {
			return new DXMenuShowContext(manager);
		}
		class DXMenuShowContext : IDXMenuShowContext {
			IDXMenuShowTracker tracker;
			public DXMenuShowContext(IDXMenuManager manager) {
				this.tracker = manager as IDXMenuShowTracker;
				if(tracker != null) tracker.StartTracking();
			}
			void IDisposable.Dispose() {
				if(tracker != null) tracker.StopTracking();
			}
			bool IDXMenuShowContext.Shown {
				get { return (tracker != null) && tracker.Track(); }
			}
		}
	}
	public class StandardMenuManager : DXMenuShowTracker, IDXMenuManager {
		static StandardMenuManager _default = null;
		public static StandardMenuManager Default {
			get {
				if(_default == null) _default = new StandardMenuManager();
				return _default;
			}
		}
		public void ShowPopupMenu(DXPopupMenu menu, Control control, Point pos) {
			if(menu == null) return;
			DXPopupStandardMenu dxMenu = new DXPopupStandardMenu(menu);
			TrackShowMenu();
			dxMenu.Show(control, pos);
		}
		IDXMenuManager IDXMenuManager.Clone(Form newForm) { return this; }
		void IDXMenuManager.DisposeManager() { }
	}
	public class StandardExMenuManager : DXMenuShowTracker, IDXMenuManager {
		static StandardExMenuManager _default = null;
		public static StandardExMenuManager Default {
			get {
				if(_default == null) _default = new StandardExMenuManager();
				return _default;
			}
		}
		public void ShowPopupMenu(DXPopupMenu menu, Control control, Point pos) {
			if(menu == null) return;
			DXPopupStandardOwnerDrawMenu dxmenu = new DXPopupStandardOwnerDrawMenu(new DXMenuItemPainter(), menu);
			TrackShowMenu();
			dxmenu.Show(control, pos);
		}
		IDXMenuManager IDXMenuManager.Clone(Form newForm) { return this; }
		void IDXMenuManager.DisposeManager() { }
	}
	public class SkinMenuManager : DXMenuShowTracker, IDXMenuManager {
		UserLookAndFeel lookAndFeel;
		public SkinMenuManager(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public void ShowPopupMenu(DXPopupMenu menu, Control control, Point pos) {
			if(menu == null) return;
			DXPopupStandardOwnerDrawMenu dxmenu = new DXPopupStandardOwnerDrawMenu(new DXSkinMenuItemPainter(lookAndFeel), menu);
			TrackShowMenu();
			dxmenu.Show(control, pos);
		}
		IDXMenuManager IDXMenuManager.Clone(Form newForm) { return this; }
		void IDXMenuManager.DisposeManager() { }
	}
	public class Office2003MenuManager : DXMenuShowTracker, IDXMenuManager {
		static Office2003MenuManager _default = null;
		public static Office2003MenuManager Default {
			get {
				if(_default == null) _default = new Office2003MenuManager();
				return _default;
			}
		}
		public void ShowPopupMenu(DXPopupMenu menu, Control control, Point pos) {
			if(menu == null) return;
			DXPopupStandardOwnerDrawMenu dxmenu = new DXPopupStandardOwnerDrawMenu(new DXOffice2003MenuItemPainter(), menu);
			TrackShowMenu();
			dxmenu.Show(control, pos);
		}
		IDXMenuManager IDXMenuManager.Clone(Form newForm) { return this; }
		void IDXMenuManager.DisposeManager() { }
	}
	#region Menu
	public delegate void DXMenuWndProcHandler(object sender, ref Message m);
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IDXMenuSupport {
		event DXMenuWndProcHandler WndProc;
		DXPopupMenu Menu { get; }
		void ShowMenu(Point pos);
	}
	public class DXPopupStandardMenu : IDisposable {
		Timer destroyTimer = null;
		bool allowDestroy = false, allowAutoDestroy = true;
		Hashtable standardMenu2Menu = null;
		ContextMenu standardMenu = null;
		DXPopupMenu menu;
		const int maxLevelCount = 5;
		public DXPopupStandardMenu(DXPopupMenu menu) {
			this.menu = menu;
		}
		public bool AllowAutoDestroy { get { return allowAutoDestroy; } set { allowAutoDestroy = value; } }
		public virtual void Dispose() {
			DestroyStandardMenu();
			Menu.GenerateCloseUpEvent();
			this.menu = null;
		}
		protected virtual void DestroyStandardMenu() {
			if(this.destroyTimer != null) this.destroyTimer.Dispose();
			if(StandardMenu != null) StandardMenu.Dispose();
			if(StandardMenu2Menu != null) StandardMenu2Menu.Clear();
			this.destroyTimer = null;
			this.standardMenu = null;
		}
		protected Hashtable StandardMenu2Menu { get { return standardMenu2Menu; } }
		protected internal ContextMenu StandardMenu { get { return standardMenu; } }
		public virtual DXPopupMenu Menu { get { return menu; } }
		public virtual void Show(Control control, Point pos) {
			if(control != null && !control.Visible) return;
			Menu.GenerateBeforePopupEvent();
			CreateMenu();
			OnBeforeShow();
			if(StandardMenu != null) {
				this.allowDestroy = false;
				IDXMenuSupport menuSupport = control as IDXMenuSupport;
				try {
					if(menuSupport != null) menuSupport.WndProc += new DXMenuWndProcHandler(OnWndProc);
					StandardMenu.Show(control, pos, Menu.GetIsRightToLeft() ? LeftRightAlignment.Left : LeftRightAlignment.Right);
				}
				finally {
					if(menuSupport != null) menuSupport.WndProc -= new DXMenuWndProcHandler(OnWndProc);
					this.allowDestroy = true;
					StartTimer();
				}
			}
		}
		const int WM_MENUCHAR = 0x0120;
		protected void OnWndProc(object sender, ref Message m) {
			if(StandardMenu != null && m.Msg == WM_MENUCHAR) {
				MethodInfo mi = typeof(System.Windows.Forms.Menu).GetMethod("WmMenuChar", BindingFlags.NonPublic | BindingFlags.Instance);
				object[] args = new object[] { m };
				if(mi != null) mi.Invoke(StandardMenu, args);
				m = (Message)args[0];
			}
		}
		protected virtual void OnBeforeShow() {
		}
		protected virtual void CreateMenu() {
			DestroyStandardMenu();
			if(StandardMenu2Menu == null) this.standardMenu2Menu = new Hashtable();
			MenuItem[] items = CreateMenuItems(Menu.Items, 0);
			if(items == null || items.Length == 0) return;
			this.standardMenu = new ContextMenu(items);
		}
		protected virtual MenuItem[] CreateMenuItems(DXMenuItemCollection collection, int level) {
			if(collection.Count == 0) return null;
			ArrayList list = new ArrayList();
			for(int n = 0; n < collection.Count; n++) {
				DXMenuItem item = collection[n];
				if(!item.Visible) continue;
				MenuItem stItem = CreateMenuItem(item, level);
				if(stItem != null) {
					if(item.BeginGroup) list.Add(CreateMenuItem(new DXMenuItem("-"), level));
					list.Add(stItem);
				}
			}
			return list.ToArray(typeof(MenuItem)) as MenuItem[];
			}
		protected virtual MenuItem CreateMenuItem(DXMenuItem item, int level) {
			MenuItem res = new MenuItem(item.Caption, null, item.Shortcut);
			res.Enabled = item.Enabled;
			SubscribeEvents(res);
			StandardMenu2Menu[res] = item;
			UpdateMenuItem(res, item, level);
			return res;
		}
		protected virtual void SubscribeEvents(MenuItem stItem) {
			stItem.Click += new EventHandler(OnStandardMenuClick);
		}
		protected virtual void UpdateFromStandardMenuItem(MenuItem stItem, DXMenuItem item) {
			DXMenuCheckItem checkItem = item as DXMenuCheckItem;
			if(checkItem != null) {
				checkItem.Checked = stItem.Checked;
				return;
			}
		}
		protected virtual void UpdateMenuItem(MenuItem stItem, DXMenuItem item, int level) {
			DXSubMenuItem subItem = item as DXSubMenuItem;
			if(subItem != null) {
				if(maxLevelCount == 0 || level < maxLevelCount) {
					stItem.MenuItems.Clear();
					subItem.GenerateBeforePopupEvent();
					MenuItem[] subItems = CreateMenuItems(subItem.Items, level + 1);
					if(subItems != null && subItems.Length > 0)
						stItem.MenuItems.AddRange(subItems);
				}
				return;
			} 
			DXMenuCheckItem checkItem = item as DXMenuCheckItem;
			if(checkItem != null) {
				stItem.Checked = checkItem.Checked;
				return;
			}
		}
		protected virtual DXMenuItem GetItemFromStandardItem(MenuItem item) {
			if(StandardMenu2Menu == null) return null;
			return StandardMenu2Menu[item] as DXMenuItem;
		}
		bool inClick = false; 
		protected virtual void OnStandardMenuClick(object sender, EventArgs e) {
			try {
				this.inClick = true;
				MenuItem stItem = sender as MenuItem;
				if(stItem != null) {
					DXMenuItem item = GetItemFromStandardItem(stItem);
					if(item != null) item.GenerateClickEvent();
				}
			}
			finally {
				this.inClick = false;
				CheckAutoDestroy();
			}
		}
		void StartTimer() {
			if(this.destroyTimer == null) {
				this.destroyTimer = new Timer();
				this.destroyTimer.Tick += new EventHandler(OnDestroyTimer);
				this.destroyTimer.Interval = 1000; 
				this.destroyTimer.Start();
			}
		}
		void OnDestroyTimer(object sender, EventArgs e) {
			if(this.destroyTimer != null) this.destroyTimer.Stop();
			CheckAutoDestroy();
		}
		void CheckAutoDestroy() {
			if(this.allowDestroy && !this.inClick) {
				DestroyStandardMenu();
				if(AllowAutoDestroy) Dispose();
			}
		}
	}
	public class DXPopupStandardOwnerDrawMenu : DXPopupStandardMenu {
		DXCustomMenuItemPainter menuPainter;
		public DXPopupStandardOwnerDrawMenu(DXCustomMenuItemPainter menuPainter, DXPopupMenu menu) : base(menu) {
			this.menuPainter = menuPainter;
		}
		public override void Dispose() {
			if(MenuPainter != null) MenuPainter.Dispose();
			this.menuPainter = null;
			base.Dispose();
		}
		protected DXCustomMenuItemPainter MenuPainter { get { return menuPainter; } }
		protected override void SubscribeEvents(MenuItem stItem) {
			base.SubscribeEvents(stItem);
			stItem.OwnerDraw = true;
			stItem.MeasureItem += new MeasureItemEventHandler(OnMenuMeasureItem);
			stItem.DrawItem += new DrawItemEventHandler(OnMenuDrawItem);
		}
		protected virtual void OnMenuMeasureItem(object sender, MeasureItemEventArgs e) {
			DXMenuItem item = GetItemFromStandardItem(sender as MenuItem);
			if(item == null) return;
			Size imageSize = item.Collection == null || item.Collection.Owner == null ? MaxImageSize : item.Collection.Owner.MaxImageSize;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				if(item.Caption == "-")
					MenuPainter.DoMeasureSeparator(item, new DXMeasureItemEventArgs(cache, e, imageSize));
				else 
					MenuPainter.DoMeasureItem(item, new DXMeasureItemEventArgs(cache, e, imageSize));
			}
		}
		protected virtual void OnMenuDrawItem(object sender, DrawItemEventArgs e) {
			MenuItem stdItem = sender as MenuItem;
			DXMenuItem item = GetItemFromStandardItem(stdItem);
			if(item == null)
				return;
			Size maxImageSize = item.Collection == null ? MaxImageSize : item.Collection.Owner.MaxImageSize;
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				if(item.Caption == "-")
					MenuPainter.DoDrawSeparator(new DXDrawItemEventArgs(cache, e, maxImageSize) { Menu = Menu });
				else
					MenuPainter.DoDrawItem(item, new DXDrawItemEventArgs(cache, e, maxImageSize) { Menu = Menu });
			}
		}
		Size imageSize = Size.Empty;
		protected override void OnBeforeShow() {
			this.imageSize = Size.Empty;
			for(int n = 0; n < this.Menu.Items.Count; n++) {
				DXMenuItem item = Menu.Items[n];
				if(item.GetImage() != null) {
					imageSize.Width = Math.Max(item.GetImage().Width, imageSize.Width);
					imageSize.Height = Math.Max(item.GetImage().Height, imageSize.Height);
				}
			}
		}
		protected Size MaxImageSize { get { return imageSize; } }
	}
	public class DXMeasureItemEventArgs : EventArgs {
		MeasureItemEventArgs args;
		GraphicsCache cache;
		Size maxImageSize;
		public DXMeasureItemEventArgs(GraphicsCache cache, MeasureItemEventArgs args, Size maxImageSize) {
			this.args = args;
			this.cache = cache;
			this.maxImageSize = maxImageSize;
		}
		public Size MaxImageSize { get { return maxImageSize; } }
		public MeasureItemEventArgs Args { get { return args; } }
		public GraphicsCache Cache { get { return cache; } }
		public int ItemHeight { get { return Args.ItemHeight; } set { Args.ItemHeight = value; } }
		public int ItemWidth { get { return Args.ItemWidth; } set { Args.ItemWidth = value; } }
	}
	public class DXDrawItemEventArgs : EventArgs {
		DrawItemEventArgs args;
		GraphicsCache cache;
		Size maxImageSize;
		public DXDrawItemEventArgs(GraphicsCache cache, DrawItemEventArgs args, Size maxImageSize) {
			this.args = args;
			this.cache = cache;
			this.maxImageSize = maxImageSize;
		}
		public Size MaxImageSize { get { return maxImageSize; } }
		public DrawItemEventArgs Args { get { return args; } }
		public GraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return Args.Graphics; } }
		public Rectangle Bounds { get { return Args.Bounds; } }
		public DrawItemState State { get { return Args.State; } }
		public Color ForeColor { get { return Args.ForeColor; } }
		public Font Font { get { return Args.Font; } }
		public DXPopupMenu Menu { get; internal set; }
	}
	public class DXCustomMenuItemPainter : IDisposable {
		public virtual void DoMeasureItem(DXMenuItem item, DXMeasureItemEventArgs e) { }
		public virtual void DoDrawItem(DXMenuItem item, DXDrawItemEventArgs e) { }
		public virtual void DoDrawSeparator(DXDrawItemEventArgs e) { }
		public virtual void Dispose() { }
		public virtual void DoMeasureSeparator(DXMenuItem item, DXMeasureItemEventArgs dXMeasureItemEventArgs) { }
	}
	public class DXMenuItemPainter : DXCustomMenuItemPainter {
		protected const int MinGlyphWidth = 12; 
		public DXMenuItemPainter() {
		}
		protected virtual int ArrowWidth { 
			get { return (int)(22 * DpiProvider.Default.DpiScaleFactor); } 
		}
		public override void Dispose() {
			base.Dispose();
		}
		static Image GetCheckBitmap() {
			Size menuCheckSize = SystemInformation.MenuCheckSize;
			Rectangle glyphRect = new Rectangle(Point.Empty, menuCheckSize);
			Bitmap bitmap = new Bitmap(menuCheckSize.Width, menuCheckSize.Height);
			using(Graphics g = Graphics.FromImage(bitmap)) {
				g.Clear(Color.Transparent);
				ControlPaint.DrawMenuGlyph(g, glyphRect, MenuGlyph.Checkmark);
				bitmap.MakeTransparent();
			}
			return bitmap;
		}
		protected virtual Size GetGlyphSize(Size maxImageSize) {
			Size minSize = SystemInformation.MenuCheckSize;
			return new Size(Math.Max(minSize.Width, maxImageSize.Width), Math.Max(minSize.Height, maxImageSize.Height));
		}
		protected virtual int GetSideStripTextIndent() { return 2; }
		protected virtual int GetSideStripSize(Size maxImageSize) {
			int delta = 2;
			if(maxImageSize.Width > 0) delta = 8;
			return GetGlyphSize(maxImageSize).Width + delta;
		}
		protected Font MenuFont { get { return AppearanceObject.DefaultMenuFont; } }
		public override void DoMeasureItem(DXMenuItem item, DXMeasureItemEventArgs e) {
			Size size = CalcItemSize(e.Cache, item, e);
			e.ItemHeight = size.Height;
			e.ItemWidth = size.Width;
		}
		public override void DoMeasureSeparator(DXMenuItem item, DXMeasureItemEventArgs e) {
			Size size = CalcSeparatorSize(e.Cache, item, e);
			e.ItemHeight = size.Height;
			e.ItemWidth = size.Width;
		}
		protected virtual Size CalcTextSize(GraphicsCache cache, DXMenuItem item) {
			AppearanceObject app = item.GetPaintAppearance(this, ObjectState.Normal);
			string text = item.Caption == "" ? "Wg" : item.Caption;
			Size size = app.CalcTextSize(cache, text, 0).ToSize();
			size.Width++; size.Height++;
			return size;
		}
		protected virtual Size CalcSeparatorSize(GraphicsCache cache, DXMenuItem item, DXMeasureItemEventArgs e) {
			return new Size(10, 6);
		}
		protected virtual Size CalcItemSize(GraphicsCache cache, DXMenuItem item, DXMeasureItemEventArgs e) {
			if(item is DXMenuHeaderItem)
				return CalcHeaderItemSize(cache, item, e);
			else
				return CalcButtonItemSize(cache, item, e);
		}
		private Size CalcHeaderItemSize(GraphicsCache cache, DXMenuItem item, DXMeasureItemEventArgs e) {
			Size textSize = CalcTextSize(cache, item), res;
			res = textSize;
			res.Width += GetSideStripSize(e.MaxImageSize) + ArrowWidth + GetSideStripTextIndent();
			res.Height += HeaderItemTextIndent * 2;
			return res;
		}
		protected int HeaderItemTextIndent { get { return 3; } }
		private Size CalcButtonItemSize(GraphicsCache cache, DXMenuItem item, DXMeasureItemEventArgs e) {
			Size textSize = CalcTextSize(cache, item), res;
			res = textSize;
			res.Height = Math.Max(textSize.Height, GetGlyphSize(e.MaxImageSize).Height + 2) + 4;
			res.Width += GetSideStripSize(e.MaxImageSize) + ArrowWidth + GetSideStripTextIndent();
			return res;
		}
		public class DrawInfo {
			public Rectangle Bounds, SideStripBounds, TextBounds;
			public DrawInfo() {
				this.Bounds = this.SideStripBounds = this.TextBounds = Rectangle.Empty;
			}
		}
		protected virtual DrawInfo CalcDrawInfo(DXMenuItem item, DXDrawItemEventArgs e) {
			DrawInfo info = new DrawInfo();
			Rectangle bounds = e.Bounds;
			info.Bounds = info.SideStripBounds = bounds;
			info.SideStripBounds.Width = GetSideStripSize(e.MaxImageSize);
			if(e.Menu.GetIsRightToLeft())
				info.SideStripBounds = new Rectangle(bounds.Right - info.SideStripBounds.Width, bounds.Y, info.SideStripBounds.Width, bounds.Height);
			bounds.Inflate(-1, -1);
			info.TextBounds = CalcTextBounds(item, info, e, bounds);
			return info;
		}
		private Rectangle CalcTextBounds(DXMenuItem item, DrawInfo info, DXDrawItemEventArgs e, Rectangle bounds) {
			if(item is DXMenuHeaderItem)
				return CalcHeaderTextBounds(item, info, e, bounds);
			return CalcButtonTextBounds(item, info, e, bounds);
		}
		private Rectangle CalcHeaderTextBounds(DXMenuItem item, DrawInfo info, DXDrawItemEventArgs e, Rectangle bounds) {
			Rectangle res = bounds;
			res.Inflate(-HeaderItemTextIndent, 0);
			return res;
		}
		private Rectangle CalcButtonTextBounds(DXMenuItem item, DrawInfo info, DXDrawItemEventArgs e, Rectangle bounds) {
			Rectangle res = bounds;
			if(e.Menu.GetIsRightToLeft()) {
				res.X = info.Bounds.X;
				res.Width = info.SideStripBounds.Left - info.Bounds.X;
			}
			else {
				res.X = info.SideStripBounds.Right + GetSideStripTextIndent();
				res.Width = bounds.Right - info.TextBounds.X;
			}
			return res;
		}
		internal bool NeedHideCursor(Rectangle bounds) {
			if(Cursor.Current == null) return false;
			return true;
		}
		public override void DoDrawItem(DXMenuItem item, DXDrawItemEventArgs e) {
			bool needHide = NeedHideCursor(e.Bounds);
			if(needHide) Cursor.Hide();
			DrawInfo info = CalcDrawInfo(item, e);
			DrawItemCore(item, e, info);
			e.Graphics.ExcludeClip(e.Bounds);
			if(needHide) Cursor.Show();
		}
		public override void DoDrawSeparator(DXDrawItemEventArgs e) {
			Point pt1 = new Point(e.Bounds.X, e.Bounds.Y + e.Bounds.Height / 2 - 1);
			Point pt2 = new Point(e.Bounds.Right, pt1.Y);
			e.Graphics.FillRectangle(GetMenuBackColor(), e.Bounds);
			e.Graphics.DrawLine(GetSeparatorColor(), pt1, pt2);
			pt1.Y++; pt2.Y++;
			e.Graphics.DrawLine(GetSeparatorColor2(), pt1, pt2);
		}
		protected virtual Pen GetSeparatorColor2() {
			return SystemPens.ControlLightLight;
		}
		protected virtual Pen GetSeparatorColor() {
			return SystemPens.ControlDark;
		}
		protected virtual Brush GetMenuBackColor() {
			return SystemBrushes.Menu;
		}
		protected virtual ObjectState GetItemState(DXMenuItem item, DXDrawItemEventArgs e) {
			if(item is DXMenuHeaderItem)
				return ObjectState.Normal;
			if((e.State & DrawItemState.Disabled) != 0)
				return ObjectState.Disabled;
			if((e.State & DrawItemState.Selected) != 0)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		protected virtual void DrawItemCore(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			DrawItemBackground(item, e, info);
			DrawItemText(item, e, info);
			DrawImage(item, e, info, false);
		}
		private void DrawItemText(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			AppearanceObject obj = item.GetPaintAppearance(this, GetItemState(item, e));
			obj.DrawString(e.Cache, item.Caption, info.TextBounds);
		}
		private void DrawItemBackground(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			if(item is DXMenuHeaderItem)
				DrawHeaderItemBackground(item, e, info);
			else
				DrawButtonItemBackground(item, e, info);
		}
		private void DrawButtonItemBackground(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			AppearanceObject obj = item.GetPaintAppearance(this, GetItemState(item, e));
			obj.FillRectangle(e.Cache, info.Bounds);
		}
		private void DrawHeaderItemBackground(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			e.Cache.FillRectangle(SystemBrushes.ControlDark, info.Bounds);
		}
		protected virtual void DrawImage(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info, bool selected) {
			Rectangle r = info.SideStripBounds;
			Size glyphSize = GetGlyphSize(e.MaxImageSize);
			Rectangle gl = Rectangle.Empty;
			gl.X = r.X + (r.Width - glyphSize.Width) / 2;
			gl.Y = r.Y + (r.Height - glyphSize.Height) / 2;
			gl.Size = glyphSize;
			DXMenuCheckItem checkItem = item as DXMenuCheckItem;
			if(checkItem != null && checkItem.Checked) {
				Rectangle check = RectangleHelper.GetCenterBounds(gl, glyphSize);
				check = Rectangle.Inflate(check, 2, 2);
				if(item.Image != null) {
					e.Cache.FillRectangle(SystemBrushes.ControlLightLight, check);
					ControlPaint.DrawBorder3D(e.Graphics, check, Border3DStyle.Flat);
				}
				else {
					check = RectangleHelper.GetCenterBounds(gl, SystemInformation.MenuCheckSize);
					using(Image image = GetCheckBitmap()) {
						e.Cache.Paint.DrawImage(e.Graphics, image, gl, new Rectangle(Point.Empty, image.Size), item.Enabled);
					}
				}
			}
			DrawImage(item, e, gl, item.Enabled);
		}
		void DrawImage(DXMenuItem item, DXDrawItemEventArgs e, Rectangle gl, bool enabled) {
			if(item.GetImage() == null) return;
			Rectangle destRect = RectangleHelper.GetCenterBounds(gl, item.GetImage().Size);
			Rectangle srcRect = new Rectangle(Point.Empty, item.GetImage().Size);
			bool enabledImage = !enabled && item.ImageDisabled != null;
			e.Cache.Paint.DrawImage(e.Graphics, item.GetImage(), destRect, srcRect, enabledImage ? true : enabled);
		}
		protected internal virtual AppearanceDefault GetAppearanceDefault(ObjectState itemState) {
			AppearanceDefault def = new AppearanceDefault();
			def.BackColor = SystemColors.Menu;
			def.Font = AppearanceObject.DefaultMenuFont;
			if(itemState == ObjectState.Disabled) {
				def.ForeColor = SystemColors.GrayText;
			}
			if(itemState == ObjectState.Normal) {
				def.ForeColor = SystemColors.MenuText;
			}
			if(itemState == ObjectState.Hot ||
				itemState == ObjectState.Pressed) {
					def.BackColor = SystemColors.MenuHighlight;
					def.ForeColor = SystemColors.HighlightText;
			}
			return def;
		}
	}
	public class DXSkinMenuItemPainter : DXMenuItemPainter {
		UserLookAndFeel lookAndFeel;
		public DXSkinMenuItemPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		protected Color GetSystemColor(Color color) {
			return Color.FromArgb(LookAndFeelHelper.GetSystemColor(lookAndFeel, color).ToArgb());
		}
		public override void DoDrawSeparator(DXDrawItemEventArgs e) {
			e.Graphics.FillRectangle(new SolidBrush(GetSystemColor(SystemColors.Menu)), e.Bounds);
			SkinElementInfo info = new SkinElementInfo(BarSkins.GetSkin(lookAndFeel)[BarSkins.SkinPopupMenuSeparator], e.Bounds);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		public override void DoMeasureSeparator(DXMenuItem item, DXMeasureItemEventArgs e) {
			SkinElementInfo info = new SkinElementInfo(BarSkins.GetSkin(lookAndFeel)[BarSkins.SkinPopupMenuSeparator], Rectangle.Empty);
			Rectangle rect = ObjectPainter.CalcObjectMinBounds(e.Cache.Graphics, SkinElementPainter.Default, info);
			e.ItemHeight = rect.Height;
		}
		protected internal override AppearanceDefault GetAppearanceDefault(ObjectState itemState) {
			AppearanceDefault res = base.GetAppearanceDefault(itemState);
			res.BackColor = GetSystemColor(SystemColors.Menu);
			res.ForeColor = GetSystemColor(SystemColors.MenuText);
			if(itemState == ObjectState.Disabled) {
				res.ForeColor = GetSystemColor(SystemColors.GrayText);
			}
			if(itemState == ObjectState.Hot ||
				itemState == ObjectState.Pressed) { 
				res.BackColor = GetSystemColor(SystemColors.MenuHighlight);
				res.ForeColor = GetSystemColor(SystemColors.HighlightText);
			}
			return res;
		}
	}
	public class DXOffice2003MenuItemPainter : DXMenuItemPainter {
		protected internal override AppearanceDefault GetAppearanceDefault(ObjectState itemState) {
			AppearanceDefault res = base.GetAppearanceDefault(itemState);
			res.BackColor = SystemColors.Menu;
			if(itemState != ObjectState.Disabled) {
				res.ForeColor = Office2003Colors.Default[Office2003Color.Text];
			}
			else {
				res.ForeColor = Office2003Colors.Default[Office2003Color.TextDisabled];
			}
			return res;
		}
		protected virtual Brush GetImageBackBrush(DXMenuItem item, GraphicsCache cache, Rectangle bounds, bool selected) {
			DXMenuCheckItem checkItem = item as DXMenuCheckItem;
			Color c1 = Office2003Colors.Default[Office2003Color.Button1Hot];
			if(checkItem == null || !checkItem.Checked) return null;
			if(selected) c1 = Office2003Colors.Default[Office2003Color.Button2Pressed];
			return cache.GetSolidBrush(c1);
		}
		public override void DoDrawItem(DXMenuItem item, DXDrawItemEventArgs e) {
			bool needHide = NeedHideCursor(e.Bounds);
			if(needHide) Cursor.Hide();
			DrawInfo info = CalcDrawInfo(item, e);
			if((e.State & DrawItemState.Selected) != 0)
				DrawHotItem(item, e, info);
			else
				DrawNormalItem(item, e, info);
			e.Graphics.ExcludeClip(e.Bounds);
			if(needHide) Cursor.Show();
		}
		protected virtual void DrawNormalItem(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			AppearanceObject obj = item.GetPaintAppearance(this, GetItemState(item, e));
			obj.FillRectangle(e.Cache, info.Bounds);
			Rectangle r = info.SideStripBounds;
			Color c1 = Office2003Colors.Default[Office2003Color.Button1], c2 = Office2003Colors.Default[Office2003Color.Button2];
			e.Cache.FillRectangle(e.Cache.GetGradientBrush(r, c1, c2, LinearGradientMode.Horizontal), r);
			obj.DrawString(e.Cache, item.Caption, info.TextBounds);
			DrawImage(item, e, info, false);
		}
		protected virtual void DrawHotItem(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info) {
			AppearanceObject obj = item.GetPaintAppearance(this, GetItemState(item, e));
			obj.FillRectangle(e.Cache, info.Bounds);
			Rectangle r = info.Bounds;
			e.Cache.DrawRectangle(e.Cache.GetPen(Office2003Colors.Default[Office2003Color.TabPageBorderColor]), r);
			r.Inflate(-1, -1);
			Color c1 = Office2003Colors.Default[Office2003Color.Button1Hot], c2 = Office2003Colors.Default[Office2003Color.Button2Hot];
			e.Cache.FillRectangle(e.Cache.GetGradientBrush(r, c1, c2, LinearGradientMode.Vertical), r);
			obj.DrawString(e.Cache, item.Caption, info.TextBounds);
			DrawImage(item, e, info, true);
		}
		protected virtual Pen GetImageBorder(DXMenuItem item, GraphicsCache cache, bool selected) {
			DXMenuCheckItem checkItem = item as DXMenuCheckItem;
			if(checkItem != null && checkItem.Checked) {
				return cache.GetPen(Office2003Colors.Default[Office2003Color.Border]);
			}
			return null;
		}
		protected override Size GetGlyphSize(Size maxImageSize) {
			return new Size(16, 16); 
		}
		protected override int GetSideStripTextIndent() { return 4; }
		protected override int GetSideStripSize(Size maxImageSize) {
			return GetGlyphSize(maxImageSize).Width + 8;
		}
		protected override void DrawImage(DXMenuItem item, DXDrawItemEventArgs e, DrawInfo info, bool selected) {
			Rectangle r = info.SideStripBounds;
			int glyphSize = GetGlyphSize(e.MaxImageSize).Width;
			r.X ++;
			r.Width = glyphSize + 4;
			r.Inflate(0, -1);
			Pen border = GetImageBorder(item, e.Cache, selected);
			if(border != null) e.Cache.DrawRectangle(border, r);
			r.Inflate(-1, -1);
			Brush back = GetImageBackBrush(item, e.Cache, r, selected);
			if(back != null) e.Cache.FillRectangle(back, r);
			if(item.GetImage() != null) {
				Rectangle gl = Rectangle.Empty;
				gl.X = r.X + (r.Width - glyphSize) / 2;
				gl.Y = r.Y + (r.Height - glyphSize) / 2;
				gl.Size = new Size(glyphSize, glyphSize);
				e.Cache.Paint.DrawImage(e.Graphics, item.GetImage(), gl, 
					new Rectangle(Point.Empty, item.GetImage().Size), 
					!item.Enabled && item.ImageDisabled != null ? true : item.Enabled);
			}
		}
	}
	public interface IDXMenuItemCollectionOwner {
		Size MaxImageSize { get; }
	}
	public class DXMenuItemCollection : CollectionBase {
		int lockUpdate = 0;
		public IDXMenuItemCollectionOwner Owner { get; private set; }
		public DXMenuItemCollection(IDXMenuItemCollectionOwner owner) {
			Owner = owner;
		}
		public event CollectionChangeEventHandler CollectionChanged;
		public void Insert(int index, DXMenuItem item) { 
			List.Insert(index, item);
		}
		public int Add(DXMenuItem item) { return List.Add(item); }
		public void Remove(DXMenuItem item) {
			if (List.Contains(item))
				List.Remove(item);
		}
		public DXMenuItem this[int index] { get { return List[index] as DXMenuItem; } }
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			DXMenuItem menuItem = item as DXMenuItem;
			menuItem.SetCollection(this);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, menuItem));
		}
		public void BeginUpdate() { lockUpdate ++; }
		public void EndUpdate() {
			if(--lockUpdate == 0) OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public int VisibleItemCount {
			get {
				int count = 0;
				for(int i = 0; i < Count; i++)
					if(this[i].Visible) count++;
				return count;
			}
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			DXMenuItem menuItem = item as DXMenuItem;
			menuItem.SetCollection(null);
			menuItem.Dispose();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, menuItem));
		}
		protected override void OnClear() {
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					List.RemoveAt(n);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnMenuItemChanged(DXMenuItem item) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, item));
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		public bool IsRightToLeft { get; set; }
	}
	public interface IOptionsMultiColumnOwner { 
		void OnChanged();
	}
	public class OptionsMultiColumn : BaseOptions {
		public OptionsMultiColumn(IOptionsMultiColumnOwner owner) {
			Owner = owner;
		}
		protected IOptionsMultiColumnOwner Owner { get; private set; }
		int columnCount = 0;
		[DefaultValue(0)]
		public int ColumnCount {
			get { return columnCount; }
			set {
				if(ColumnCount == value)
					return;
				int prev = ColumnCount;
				columnCount = value;
				OnChanged("ColumnCount", prev, ColumnCount);
			}
		}
		int maxItemWidth = 0;
		[DefaultValue(0)]
		public int MaxItemWidth {
			get { return maxItemWidth; }
			set {
				if(MaxItemWidth == value)
					return;
				int prev = MaxItemWidth;
				maxItemWidth = value;
				OnChanged("MaxItemWidth", prev, MaxItemWidth);
			}
		}
		ItemHorizontalAlignment imageHorizontalAlignment = ItemHorizontalAlignment.Default;
		[DefaultValue(ItemHorizontalAlignment.Default)]
		public ItemHorizontalAlignment ImageHorizontalAlignment {
			get { return imageHorizontalAlignment; }
			set {
				if(ImageHorizontalAlignment == value)
					return;
				ItemHorizontalAlignment prev = ImageHorizontalAlignment;
				imageHorizontalAlignment = value;
				OnChanged("ImageHorizontalAlignment", prev, ImageHorizontalAlignment);
			}
		}
		ItemVerticalAlignment imageVerticalAlignment = ItemVerticalAlignment.Default;
		[DefaultValue(ItemVerticalAlignment.Default)]
		public ItemVerticalAlignment ImageVerticalAlignment {
			get { return imageVerticalAlignment; }
			set {
				if(ImageVerticalAlignment == value)
					return;
				ItemVerticalAlignment prev = ImageVerticalAlignment;
				imageVerticalAlignment = value;
				OnChanged("ImageVerticalAlignment", prev, ImageVerticalAlignment);
			}
		}
		DefaultBoolean useMaxItemWidth = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean UseMaxItemWidth {
			get { return useMaxItemWidth; }
			set {
				if(UseMaxItemWidth == value)
					return;
				DefaultBoolean prev = UseMaxItemWidth;
				useMaxItemWidth = value;
				OnChanged("UseMaxItemWidth", prev, UseMaxItemWidth);
			}
		}
		int itemIndent = -1;
		[DefaultValue(-1)]
		public int ItemIndent {
			get { return itemIndent; }
			set {
				if(ItemIndent == value)
					return;
				int prev = ItemIndent;
				itemIndent = value;
				OnChanged("GalleryItemIndent", prev, ItemIndent);
			}
		}
		DefaultBoolean showItemText = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowItemText {
			get { return showItemText; }
			set {
				if(ShowItemText == value)
					return;
				DefaultBoolean prev = ShowItemText;
				showItemText = value;
				OnChanged("ShowItemText", prev, ShowItemText);
			}
		}
		DefaultBoolean largeImages = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean LargeImages {
			get { return largeImages; }
			set {
				if(LargeImages == value)
					return;
				DefaultBoolean prev = LargeImages;
				largeImages = value;
				OnChanged("LargeImages", prev, LargeImages);
			}
		}
		protected override void OnChanged(BaseOptionChangedEventArgs e) {
			base.OnChanged(e);
			if(Owner != null)
				Owner.OnChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeCore() { return ShouldSerialize(); }
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			OptionsMultiColumn gallery = options as OptionsMultiColumn;
			if(gallery != null) {
				this.columnCount = gallery.ColumnCount;
				this.itemIndent = gallery.ItemIndent;
				this.largeImages = gallery.LargeImages;
				this.maxItemWidth = gallery.MaxItemWidth;
				this.showItemText = gallery.ShowItemText;
				this.useMaxItemWidth = gallery.UseMaxItemWidth;
				this.imageHorizontalAlignment = gallery.ImageHorizontalAlignment;
				this.imageVerticalAlignment = gallery.ImageVerticalAlignment;
			}
		}
	}
	public enum MenuViewType { Menu, Toolbar, RibbonMiniToolbar }
	public class DXPopupMenu : DXSubMenuItem, IDXDropDownControl, IOptionsMultiColumnOwner {
		private static readonly object closeUp = new object();
		private static readonly object popupHide = new object();
		EventHandlerList events;
		object ownerPopup = null;
		ContentAlignment alignment;
		MenuViewType menuViewType;
		public DXPopupMenu(EventHandler beforePopup) : base("", beforePopup) {
			this.alignment = ContentAlignment.BottomRight;
			this.menuViewType = MenuViewType.Menu;
		}
		public DXPopupMenu() : base("") {
			this.alignment = ContentAlignment.BottomRight;
			this.menuViewType = MenuViewType.Menu;
		}
		public override bool IsRightToLeft {
			get { return Items != null ? Items.IsRightToLeft : false; }
			set { Items.IsRightToLeft = value; }
		}
		public override bool GetIsRightToLeft() {
			return IsRightToLeft;
		}
		OptionsMultiColumn optionsMultiColumn;
		void ResetOptionsMultiColumn() { OptionsMultiColumn.Reset(); }
		bool ShouldSerializeOptionsMultiColumn() { return OptionsMultiColumn.ShouldSerializeCore(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsMultiColumn OptionsMultiColumn {
			get { 
				if(optionsMultiColumn == null)
					optionsMultiColumn = new OptionsMultiColumn(this);
				return optionsMultiColumn;
			}
		}
		DefaultBoolean multiColumn = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean MultiColumn {
			get { return multiColumn; }
			set {
				if(MultiColumn == value)
					return;
				multiColumn = value;
			}
		}
		void ResetAlignment() { Alignment = ContentAlignment.BottomRight; }
		bool ShouldSerializeAlignment() { return Alignment != ContentAlignment.BottomRight; }
		public ContentAlignment Alignment {
			get { return alignment; }
			set { alignment = value; }
		}
		[DefaultValue(MenuViewType.Menu)]
		public MenuViewType MenuViewType {
			get { return menuViewType; }
			set { menuViewType = value; }
		}
		bool IDXDropDownControl.Visible { get { return this.Visible; } }
		void IDXDropDownControl.Show(IDXMenuManager manager, Control control, Point pos) {
			if (manager == null) return;
			manager.ShowPopupMenu(this, control, pos);
		}
		void IDXDropDownControl.Hide() {
			this.Visible = false;
		}
		public virtual void GenerateCloseUpEvent() {
			RaiseCloseUp();
		}
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		protected virtual void RaiseCloseUp() {
			EventHandler handler = Events[closeUp] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		protected virtual void RaisePopupHide() {
			EventHandler handler = Events[popupHide] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public event EventHandler CloseUp {
			add { Events.AddHandler(closeUp, value); }
			remove { Events.RemoveHandler(closeUp, value); }
		}
		public object OwnerPopup {
			get { return ownerPopup; }
			set { ownerPopup = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler PopupHide {
			add { Events.AddHandler(popupHide, value); }
			remove { Events.RemoveHandler(popupHide, value); }
		}
		public void HidePopup() {
			RaisePopupHide();
		}
		void IOptionsMultiColumnOwner.OnChanged() {
		}
	}
	public class DXMenuHeaderItem : DXMenuItem, IOptionsMultiColumnOwner {
		OptionsMultiColumn optionsMultiColumn;
		void ResetOptionsMultiColumn() { OptionsMultiColumn.Reset(); }
		bool ShouldSerializeOptionsMultiColumn() { return OptionsMultiColumn.ShouldSerializeCore(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public OptionsMultiColumn OptionsMultiColumn {
			get {
				if(optionsMultiColumn == null)
					optionsMultiColumn = new OptionsMultiColumn(this);
				return optionsMultiColumn;
			}
		}
		DefaultBoolean multiColumn = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean MultiColumn {
			get { return multiColumn; }
			set {
				if(MultiColumn == value)
					return;
				multiColumn = value;
			}
		}
		void IOptionsMultiColumnOwner.OnChanged() {
		}
	}
	public class DXMenuItem : Utils.MVVM.ISupportCommandBinding, IDisposable {
		DXMenuItemCollection collection;
		bool enabled = true, beginGroup = false, visible = true;
		bool closeMenuOnClick = true;
		DXMenuItemPriority priority;
		string caption;
		Image image = null, imageDisabled = null, largeImage, largeImageDisabled;
		Shortcut shortcut = Shortcut.None;
		object tag = null;
		public event EventHandler Click;
		SuperToolTip superTip;
		AppearanceObject appearance;
		AppearanceObject appearanceHover;
		AppearanceObject appearanceDisabled;
		AppearanceObject paintAppearance;
		public DXMenuItem() : this("") { }
		public DXMenuItem(string caption) : this(caption, null) { }
		public DXMenuItem(string caption, DXMenuItemPriority priority) : this(caption, null, priority) { }
		public DXMenuItem(string caption, EventHandler click, DXMenuItemPriority priority) : this(caption, click, null, null, null, null, priority) { }
		public DXMenuItem(string caption, EventHandler click) : this(caption, click, null) { }
		public DXMenuItem(string caption, EventHandler click, Image image) : this(caption, click, image, null) { }
		public DXMenuItem(string caption, EventHandler click, Image image, Image imageDisabled) : this(caption, click, image, imageDisabled, null, null) { }
		public DXMenuItem(string caption, EventHandler click, Image image, Image imageDisabled, Image largeImage, Image largeImageDisabled)
			: this(caption, click, image, imageDisabled, largeImage, largeImageDisabled, DXMenuItemPriority.Normal) {
		}
		public DXMenuItem(string caption, EventHandler click, Image image, Image imageDisabled, Image largeImage, Image largeImageDisabled, DXMenuItemPriority priority) {
			this.Click = click;
			this.caption = caption;
			this.image = image;
			this.priority = priority;
			this.imageDisabled = imageDisabled;
			this.largeImage = largeImage;
			this.largeImageDisabled = largeImageDisabled;
			this.appearance = CreateAppearance();
			this.appearance.Changed += OnAppearanceChanged;
			this.appearanceHover = CreateAppearance();
			this.appearanceHover.Changed += OnAppearanceChanged;
			this.appearanceDisabled = CreateAppearance();
			this.appearanceDisabled.Changed += OnAppearanceChanged;
			this.ItemState = ObjectState.Normal;
		}
		~DXMenuItem() { Dispose(false); }
		void OnAppearanceChanged(object sender, EventArgs e) {
		}
		bool isRightToLeft = false;
		public virtual bool IsRightToLeft {
			get { return isRightToLeft; }
			set { isRightToLeft = value; }
		}
		public virtual bool GetIsRightToLeft() {
			if(IsRightToLeft) return true;
			if(Collection != null) return Collection.IsRightToLeft;
			return IsRightToLeft;
		}
		protected virtual AppearanceObject CreateAppearance() {
			return new AppearanceObject();
		}
		public virtual void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				this.Click = null;
			}
		}
		protected internal ObjectState ItemState { get; set; }
		protected internal virtual AppearanceObject GetPaintAppearance(DXMenuItemPainter painter, ObjectState itemState) {
			if(this.paintAppearance == null || itemState != ItemState) {
				ItemState = itemState;
				this.paintAppearance = UpdatePaintAppearance(painter, itemState);
			}
			return this.paintAppearance;
		}
		protected virtual AppearanceObject UpdatePaintAppearance(DXMenuItemPainter painter, ObjectState itemState) {
			AppearanceObject res = new AppearanceObject();
			AppearanceObject item = GetItemAppearance(itemState);
			AppearanceHelper.Combine(res, new AppearanceObject[] { item }, painter.GetAppearanceDefault(itemState));
			if(res.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				res.TextOptions.HotkeyPrefix = HKeyPrefix.Show;
			res.TextOptions.RightToLeft = GetIsRightToLeft();
			return res;
		}
		protected virtual AppearanceObject GetItemAppearance(ObjectState itemState) {
			if(itemState == ObjectState.Disabled)
				return AppearanceDisabled;
			if(itemState == ObjectState.Pressed ||
				itemState == ObjectState.Hot)
				return AppearanceHovered;
			return Appearance;
		}
		[Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(UITypeEditor))]
		public SuperToolTip SuperTip {
			get { return superTip; }
			set { superTip = value; }
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance {
			get { return appearance; }
		}
		bool ShouldSerializeAppearanceDisabled() { return AppearanceDisabled.ShouldSerialize(); }
		void ResetAppearanceDisabled() { AppearanceDisabled.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceDisabled {
			get { return appearanceDisabled; }
		}
		bool ShouldSerializeAppearanceHover() { return AppearanceHovered.ShouldSerialize(); }
		void ResetAppearanceHover() { AppearanceHovered.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceHovered {
			get { return appearanceHover; }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemCollection")]
#endif
		public DXMenuItemCollection Collection { get { return collection; } }
		protected internal void SetCollection(DXMenuItemCollection collection) {
			this.collection = collection;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DXMenuItemShowHotKeyPrefix"),
#endif
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use Appearance.TextOptions.HotkeyPrefix instead of ShowHotKeyPrefix")]
		public bool ShowHotKeyPrefix {
			get { return Appearance.TextOptions.HotkeyPrefix == HKeyPrefix.Default; }
			set {
				if(ShowHotKeyPrefix == value) return;
				Appearance.TextOptions.HotkeyPrefix = value ? HKeyPrefix.Default : HKeyPrefix.None;
				OnChanged();
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemEnabled")]
#endif
		public bool Enabled {
			get { return enabled; }
			set {
				if(Enabled == value) return;
				enabled = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemVisible")]
#endif
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemTag")]
#endif
		public object Tag {
			get { return tag; }
			set {
				if(Tag == value) return;
				tag = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemBeginGroup")]
#endif
		public bool BeginGroup {
			get { return beginGroup; }
			set {
				if(BeginGroup == value) return;
				beginGroup = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemCaption")]
#endif
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DXMenuItemImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DXMenuItemImageDisabled"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image ImageDisabled {
			get { return imageDisabled; }
			set {
				if(ImageDisabled == value) return;
				imageDisabled = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DXMenuItemLargeImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image LargeImage {
			get { return largeImage; }
			set {
				if(LargeImage == value) return;
				largeImage = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("DXMenuItemLargeImageDisabled"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image LargeImageDisabled {
			get { return largeImageDisabled; }
			set {
				if(LargeImageDisabled == value) return;
				largeImageDisabled = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuItemShortcut")]
#endif
		public Shortcut Shortcut {
			get { return shortcut; }
			set {
				if(Shortcut == value) return;
				shortcut = value;
				OnChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CloseMenuOnClick {
			get { return closeMenuOnClick; }
			set {
				if(CloseMenuOnClick == value) return;
				closeMenuOnClick = value;
				OnChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual DXMenuItemPriority Priority { get { return priority; } set { this.priority = value; } }
		protected virtual void OnClick() {
			if(Click != null) Click(this, EventArgs.Empty);
		}
		public void GenerateClickEvent() {
			OnClick();
		}
		protected virtual void OnChanged() { 
			if(Collection != null) Collection.OnMenuItemChanged(this);
		}
		public Image GetImage() {
			if(Enabled) return Image;
			else {
				if(ImageDisabled != null) return ImageDisabled;
				return Image;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowGlyphSkinning { get; set; }
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(menuItem, execute) => menuItem.Click += (s, e) => execute(),
				(menuItem, canExecute) => menuItem.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(menuItem, execute) => menuItem.Click += (s, e) => execute(),
				(menuItem, canExecute) => menuItem.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(menuItem, execute) => menuItem.Click += (s, e) => execute(),
				(menuItem, canExecute) => menuItem.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
	public class DXButtonGroupItem : DXSubMenuItem {
		public DXButtonGroupItem() : base() { }
	}
	public class DXSubMenuItem : DXMenuItem, IDXMenuItemCollectionOwner {
		DXMenuItemCollection items;
		public event EventHandler BeforePopup;
		public DXSubMenuItem() : this("") { }
		public DXSubMenuItem(string caption) : this(caption, null) { }
		public DXSubMenuItem(string caption, EventHandler beforePopup) : base(caption) {
			this.items = new DXMenuItemCollection(this);
			this.items.CollectionChanged += new CollectionChangeEventHandler(OnItemsChanged);
			this.BeforePopup = beforePopup;
			MaxImageSize = new Size(16, 16);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Items != null) {
					this.items.CollectionChanged -= new CollectionChangeEventHandler(OnItemsChanged);
					this.items.Clear();
					this.items = null;
				}
				BeforePopup = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void OnItemsChanged(object sender, CollectionChangeEventArgs e) {
			UpdateMaxImageSize();
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXSubMenuItemItems")]
#endif
		public virtual DXMenuItemCollection Items { get { return items; } }
		protected virtual void OnBeforePopup() {
			if(BeforePopup != null) BeforePopup(this, EventArgs.Empty);
			UpdateMaxImageSize();
		}
		private void UpdateMaxImageSize() {
			int width = 16, height = 16;
			foreach(DXMenuItem item in Items) { 
				if(item.Image == null)
					continue;
				width = Math.Max(width, item.Image.Width);
				height = Math.Max(height, item.Image.Height);
			}
			MaxImageSize = new Size(width, height);
		}
		Size IDXMenuItemCollectionOwner.MaxImageSize { get { return MaxImageSize; } }
		protected internal Size MaxImageSize {
			get;
			private set;
		}
		public void GenerateBeforePopupEvent() {
			OnBeforePopup();
		}
	}
	public class DXMenuCheckItem : DXMenuItem {
		bool check;
		public event EventHandler CheckedChanged;
		public DXMenuCheckItem() : this("") { }
		public DXMenuCheckItem(string caption) : this(caption, false) { }
		public DXMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged) : this(caption, check) {
			this.Image = image;
			this.CheckedChanged = checkedChanged;
		}
		public DXMenuCheckItem(string caption, bool check) : base(caption) {
			this.check = check;
		}
		public override void Dispose() {
			CheckedChanged = null;
			base.Dispose();
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("DXMenuCheckItemChecked")]
#endif
		public bool Checked {
			get { return check; }
			set {
				if(Checked == value) return;
				check = value;
				OnCheckedChanged();
			}
		}
		protected override void OnClick() {
			Checked = !Checked;
			base.OnClick();
		}
		protected virtual void OnCheckedChanged() {
			OnChanged();
			if(CheckedChanged != null) CheckedChanged(this, EventArgs.Empty);
		}
	}
  #endregion Menu
	#region CommandMenuItem<T>
	public class CommandMenuItem<T> : DXMenuItem, IDXMenuItem<T> where T : struct {
		T id;
		public event EventHandler Update;
		public CommandMenuItem() : this(String.Empty) { }
		public CommandMenuItem(string caption) : this(caption, null) { }
		public CommandMenuItem(string caption, EventHandler click) : this(caption, click, null) { }
		public CommandMenuItem(string caption, EventHandler click, Image image) : this(caption, click, image, null) { }
		public CommandMenuItem(string caption, EventHandler click, Image image, EventHandler update)
			: base(caption, click, image) {
			this.Update = update;
		}
		public T Id { get { return id; } set { id = value; } }
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					this.Update = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public sealed override void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~CommandMenuItem() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void RaiseUpdate() {
			if (Update != null)
				Update(this, EventArgs.Empty);
		}
		public void GenerateUpdateEvent() {
			RaiseUpdate();
		}
	}
	#endregion
	#region CommandMenuCheckItem<T>
	public class CommandMenuCheckItem<T> : DXMenuCheckItem, IDXMenuCheckItem<T> where T : struct {
		T id;
		bool updating;
		public event EventHandler Update;
		public CommandMenuCheckItem() : this(String.Empty) { }
		public CommandMenuCheckItem(string caption) : this(caption, false) { }
		public CommandMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged) : this(caption, check, image, checkedChanged, null) { }
		public CommandMenuCheckItem(string caption, bool check) : this(caption, check, null) { }
		public CommandMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged, EventHandler update)
			: base(caption, check, image, checkedChanged) {
			this.Update = update;
		}
		public CommandMenuCheckItem(string caption, bool check, EventHandler update)
			: base(caption, check) {
			this.Update = update;
		}
		public T Id { get { return id; } set { id = value; } }
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					this.Update = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public sealed override void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~CommandMenuCheckItem() {
			Dispose(false);
		}
		#endregion
		protected internal virtual void RaiseUpdate() {
			if (Update != null) {
				updating = true;
				try {
					Update(this, EventArgs.Empty);
				}
				finally {
					updating = false;
				}
			}
		}
		public void GenerateUpdateEvent() {
			RaiseUpdate();
		}
		protected override void OnCheckedChanged() {
			if (!updating)
				base.OnCheckedChanged();
		}
	}
	#endregion
	#region CommandPopupMenu<T>
	public class CommandPopupMenu<T> : DXPopupMenu, IDXPopupMenu<T> where T : struct {
		T id;
		public CommandPopupMenu(EventHandler beforePopup)
			: base(beforePopup) {
		}
		public CommandPopupMenu()
			: base() {
		}
		public T Id { get { return id; } set { id = value; } }
		int IDXPopupMenu<T>.ItemsCount { get { return this.Items.Count; } }
		void IDXPopupMenu<T>.AddItem(IDXMenuItemBase<T> item) {
			DXMenuItem dxItem = item as DXMenuItem;
			if (dxItem != null)
				this.Items.Add(dxItem);
		}
		protected override void OnBeforePopup() {
			base.OnBeforePopup();
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				CommandMenuItem<T> item = Items[i] as CommandMenuItem<T>;
				if (item != null)
					item.GenerateUpdateEvent();
				else {
					CommandMenuCheckItem<T> checkItem = Items[i] as CommandMenuCheckItem<T>;
					if (checkItem != null)
						checkItem.GenerateUpdateEvent();
				}
			}
			RemoveBeginGroupFromFirstVisibleItem();
		}
		void RemoveBeginGroupFromFirstVisibleItem() {
			int count = Items.Count;
			for (int i = 0; i < count; i++) {
				DXMenuItem item = Items[i] as DXMenuItem;
				if (item != null && item.Visible) {
					if (item.BeginGroup)
						item.BeginGroup = false;
					break;
				}
			}
		}
		public void RemoveMenuItem(T id) {
			RemoveMenuItem(id, true);
		}
		public void EnableMenuItem(T id) {
			EnableMenuItem(id, true);
		}
		public void EnableMenuItem(T id, bool recursive) {
			DXMenuItem item = GetDXMenuItemById(this, id, recursive);
			if (item != null)
				item.Enabled = true;
		}
		public void DisableMenuItem(T id) {
			DisableMenuItem(id, true);
		}
		public void DisableMenuItem(T id, bool recursive) {
			DXMenuItem item = GetDXMenuItemById(this, id, recursive);
			if (item != null)
				item.Enabled = false;
		}
		public void RemoveMenuItem(T id, bool recursive) {
			DXPopupMenu parentMenu = this;
			DXMenuItem item = GetDXMenuItemInternal(this, id, recursive, ref parentMenu);
			if (item != null && parentMenu != null)
				parentMenu.Items.Remove(item);
		}
		public static DXMenuItem GetDXMenuItemById(DXPopupMenu menu, T id, bool recursive) {
			DXPopupMenu parentMenu = menu;
			return GetDXMenuItemInternal(menu, id, recursive, ref parentMenu);
		}
		internal static DXMenuItem GetDXMenuItemInternal(DXPopupMenu menu, T id, bool recursive, ref DXPopupMenu parentMenu) {
			int count = menu.Items.Count;
			for (int i = 0; i < count; i++) {
				DXMenuItem dxItem = menu.Items[i];
				CommandMenuItem<T> item = dxItem as CommandMenuItem<T>;
				if (item != null && Object.Equals(item.Id, id))
					return item;
				CommandMenuCheckItem<T> checkItem = dxItem as CommandMenuCheckItem<T>;
				if (checkItem != null && Object.Equals(checkItem.Id, id))
					return checkItem;
				if (recursive) {
					DXPopupMenu dxSubMenu = dxItem as DXPopupMenu;
					if (dxSubMenu != null) {
						CommandPopupMenu<T> subMenu = dxSubMenu as CommandPopupMenu<T>;
						if (subMenu != null && Object.Equals(subMenu.Id, id))
							return subMenu;
						dxItem = GetDXMenuItemInternal(dxSubMenu, id, true, ref parentMenu);
						if (dxItem != null) {
							parentMenu = dxSubMenu;
							return dxItem;
						}
					}
				}
			}
			return null;
		}
		protected internal static void MoveMenuItem(DXSubMenuItem menu, int from, int to) {
			IList items = menu.Items;
			int count = items.Count;
			if (from < 0 || from >= count)
				return;
			if (to < 0)
				to = 0;
			if (to >= count)
				to = count - 1;
			if (from == to)
				return;
			int delta = Math.Sign(to - from);
			object item = items[from];
			for (int i = from; i != to; i += delta)
				items[i] = items[i + delta];
			items[to] = item;
		}
		#region MoveMenuItem etc
		public void MoveMenuItem(DXMenuItem item, int to) {
			int index = IndexOf(Items, item);
			if (index >= 0)
				MoveMenuItem(this, index, to);
		}
		public void MoveMenuItem(T id, int to) {
			CommandMenuItem<T> item = GetDXMenuItemById(this, id, false) as CommandMenuItem<T>;
			if (item != null)
				MoveMenuItem(item, to);
		}
		public void MoveMenuCheckItem(T id, int to) {
			CommandMenuCheckItem<T> item = GetDXMenuItemById(this, id, false) as CommandMenuCheckItem<T>;
			if (item != null)
				MoveMenuItem(item, to);
		}
		public void MoveSubMenuItem(T id, int to) {
			CommandPopupMenu<T> item = GetDXMenuItemById(this, id, false) as CommandPopupMenu<T>;
			if (item != null)
				MoveMenuItem(item, to);
		}
		static int IndexOf(DXMenuItemCollection items, DXMenuItem item) {
			int count = items.Count;
			for (int i = 0; i < count; i++)
				if (items[i] == item)
					return i;
			return -1;
		}
		#endregion
	}
	#endregion
	#region DXMenuItemUIState
	public class DXMenuItemUIState : ICommandUIState {
		DXMenuItem item;
		public DXMenuItemUIState(DXMenuItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visible; } set { item.Visible = value; } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
	}
	#endregion
	#region DXMenuCheckItemUIState
	public class DXMenuCheckItemUIState : ICommandUIState {
		DXMenuCheckItem item;
		public DXMenuCheckItemUIState(DXMenuCheckItem item) {
			Guard.ArgumentNotNull(item, "item");
			this.item = item;
		}
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visible; } set { item.Visible = value; } }
		public virtual bool Checked { get { return item.Checked; } set { item.Checked = value; } }
		public virtual object EditValue { get { return null; } set { } }
	}
	#endregion
	#region DXMenuItemCommandAdapter<T> (abstract class)
	public abstract class DXMenuItemCommandAdapter<T> : IDXMenuItemCommandAdapter<T> where T : struct {
		Command command;
		public DXMenuItemCommandAdapter(Command command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		public Command Command { get { return command; } }
		public virtual void OnClick(object sender, EventArgs e) {
			DXMenuItem item = (DXMenuItem)sender;
			if (item.Visible && item.Enabled) {
				command.Execute();
			}
		}
		public virtual void OnUpdate(object sender, EventArgs e) {
			DXMenuItem item = (DXMenuItem)sender;
			if (item.Visible && item.Enabled) { 
				ICommandUIState state = command.CreateDefaultCommandUIState();
				command.UpdateUIState(state);
				DXMenuItemUIState menuItemState = new DXMenuItemUIState(item);
				menuItemState.Visible = state.Visible;
				menuItemState.Enabled = state.Enabled;
				menuItemState.Checked = state.Checked;
			}
		}
		public abstract IDXMenuItem<T> CreateMenuItem(DXMenuItemPriority priority);
	}
	#endregion
	#region DXMenuCheckItemCommandAdapter<T> (abstract class)
	public abstract class DXMenuCheckItemCommandAdapter<T> : IDXMenuCheckItemCommandAdapter<T> where T : struct {
		Command command;
		protected DXMenuCheckItemCommandAdapter(Command command) {
			Guard.ArgumentNotNull(command, "command");
			this.command = command;
		}
		public Command Command { get { return command; } }
		public virtual void OnCheckedChanged(object sender, EventArgs e) {
			DXMenuCheckItem item = (DXMenuCheckItem)sender;
			if (item.Visible && item.Enabled) {
				DXMenuCheckItemUIState commandState = new DXMenuCheckItemUIState(item);
				command.ForceExecute(commandState);
			}
		}
		public virtual void OnUpdate(object sender, EventArgs e) {
			DXMenuCheckItem item = (DXMenuCheckItem)sender;
			if (item.Visible && item.Enabled) { 
				DXMenuCheckItemUIState commandState = new DXMenuCheckItemUIState(item);
				command.UpdateUIState(commandState);
			}
		}
		public abstract IDXMenuCheckItem<T> CreateMenuItem(string groupId);
	}
	#endregion
}
