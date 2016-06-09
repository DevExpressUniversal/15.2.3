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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraBars.Utils {
	public class BarManagerOptionsLayout : OptionsLayoutBase {
		public BarManagerOptionsLayout() : base() {
			AllowAddNewItems = true;
			AllowRemoveOldItems = false;
		}
		[DefaultValue(true)]
		public bool AllowAddNewItems { get; set; }
		[DefaultValue(false)]
		public bool AllowRemoveOldItems { get; set; }
		internal bool ShouldSerializeCore() { return ShouldSerialize(); }
	}
	public class BarUtilites {
		public static bool IsRadialMenuManager(BarManager manager) {
			foreach(BarLinksHolder holder in manager.ItemHolders) {
				if(typeof(RadialMenu).IsAssignableFrom(holder.GetType()))
					return true;
			}
			return false;
		}
		public static bool IsBelongsToRadialMenuManager(BarItemAppearance itemAppearance) {
			BarItemAppearances appearances = itemAppearance.OwnerCore as BarItemAppearances;
			if(appearances == null)
				return false;
			BarItem item = appearances.Owner as BarItem;
			if(item == null || item.Manager == null)
				return false;
			return IsRadialMenuManager(item.Manager);
		}
		public static Bitmap RenderToBitmap(Control control, int offset) {
			Bitmap bitmap = RenderToBitmap(control);
			if(offset == 0 || (bitmap.Height - offset < 1))
				return bitmap;
			Bitmap clippedBitmap = new Bitmap(bitmap.Width, bitmap.Height - offset);
			using(Graphics graphics = Graphics.FromImage(clippedBitmap)) {
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.DrawImage(bitmap, new Rectangle(Point.Empty, clippedBitmap.Size), new Rectangle(0, offset, clippedBitmap.Width, clippedBitmap.Height), GraphicsUnit.Pixel);
				graphics.Save();
			}
			return clippedBitmap;
		}
		public static Bitmap RenderToBitmap(Control control) {
			Bitmap bitmap = new Bitmap(control.Width, control.Height);
			control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, control.Size));
			return bitmap;
		}
		public static Bitmap Merge(Bitmap top, Bitmap bottom) {
			Bitmap bitmap = new Bitmap(top.Width, top.Height + bottom.Height);
			using(Graphics graphics = Graphics.FromImage(bitmap)) {
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.DrawImage(top, new Rectangle(0, 0, bitmap.Width, top.Height));
				graphics.DrawImage(bottom, new Rectangle(0, top.Height, bitmap.Width, bottom.Height));
				graphics.Save();
			}
			return bitmap;
		}
		public static BarItemInfo[] GetItemInfoList(BarManager manager) {
			ArrayList list = new ArrayList();
			for(int n = 0; n < manager.PaintStyle.ItemInfoCollection.Count; n++) {
				BarItemInfo info = manager.PaintStyle.ItemInfoCollection[n];
				if(info.SupportRibbon) list.Add(info);
			}
			return list.ToArray(typeof(BarItemInfo)) as BarItemInfo[];
		}
		public static Rectangle GetFormRestoreBounds(Form form) {
			Rectangle res = new Rectangle();
			BarNativeMethods.WINDOWPLACEMENT wp = new BarNativeMethods.WINDOWPLACEMENT();
			wp.length = Marshal.SizeOf(typeof(BarNativeMethods.WINDOWPLACEMENT));
			BarNativeMethods.GetWindowPlacement(form.Handle, ref wp);
			res.Width = wp.rcNormalPosition_right - wp.rcNormalPosition_left;
			res.Height = wp.rcNormalPosition_bottom - wp.rcNormalPosition_top;
			res.X = wp.rcNormalPosition_left;
			res.Y = wp.rcNormalPosition_top;
			return res;
		}
		public static Color GetAppMenuLabelForeColor(ISkinProvider provider) {
			Color cr = RibbonSkins.GetSkin(provider)[RibbonSkins.SkinAppMenuPane].Properties.GetColor("LabelForeColor", Color.Empty);
			if(cr.IsSystemColor) cr = RibbonSkins.GetSkin(provider).GetSystemColor(cr);
			return cr;
		}
		static float screenDPI = 0;
		public static float GetScreenDPI() {
			if(screenDPI != 0) return screenDPI;
			GraphicsInfo gInfo = new GraphicsInfo();
			try {
				gInfo.AddGraphics(null);
				screenDPI = gInfo.Graphics.DpiX;
			}
			finally {
				gInfo.ReleaseGraphics();
			}
			return screenDPI;
		}
		static int maxLengthZBuffer = 10000;
		public static int[][] CreateZBuffer(Rectangle ownerRect) {
			int[][] buffer = new int[2][];
			int length = Math.Min(Math.Max(ownerRect.Width, 0), maxLengthZBuffer);
			buffer[0] = new int[length];
			buffer[1] = new int[length];
			for(int i = 0; i < length; i++) { buffer[0][i] = ownerRect.X + i; }
			return buffer;
		}
		public static List<Rectangle> GetEmptyAreas(int[][] buffer, int length) {
			length = Math.Min(Math.Max(length, 0), maxLengthZBuffer);
			List<Rectangle> rects = new List<Rectangle>();
			if(length == 0) return rects;
			int tempX, tempWidth;
			tempWidth = 0;
			tempX = -1;
			int min = buffer[1].Min();
			for(int i = 0; i < length; i++) {
				if(buffer[1][i] == min) {
					if(tempX == -1) tempX = i;
					tempWidth++;
					continue;
				}
				if(tempX == -1) continue;
				rects.Add(new Rectangle(buffer[0][tempX], 0, tempWidth, 0));
				tempWidth = 0; tempX = -1;
			}
			if(tempX > -1)
				rects.Add(new Rectangle(buffer[0][tempX], 0, tempWidth, 0));
			return rects;
		}
		public static Rectangle ConvertBoundsToRTL(Rectangle bounds, Rectangle ownerBounds) {
			int x = ownerBounds.Right + ownerBounds.X - bounds.Right;
			return new Rectangle(x, bounds.Y, bounds.Width, bounds.Height);
		}
	}
	public static class ReflectionUtils {
		public static void ForEachRuntimeSerializableProperty<T>(Action<PropertyInfo> visitor) {
			foreach(PropertyInfo prop in GetPublicProperties<T>()) {
				if(IsRuntimeSerializable(prop)) visitor(prop);
			}
		}
		static bool IsRuntimeSerializable(PropertyInfo prop) {
			if(SkipProperty(prop) || IsReadOnlyBuiltInType(prop)) return false;
			Type propType = prop.PropertyType;
			if(propType.IsDescendantOf<Component>() || propType.IsDescendantOf<Image>()) {
				return false;
			}
			if(propType.IsPureObject() || propType.IsAccessibilityObject()) return false;
			if(IsNonSerializableCollection(prop)) return false;
			if(IsPropertyHidden(prop) || CheckDesignerSerializationVisibility(prop, DesignerSerializationVisibility.Hidden)) return false;
			return true;
		}
		public static bool IsDescendantOf<T>(this Type dataType) {
			return typeof(T).IsAssignableFrom(dataType);
		}
		static bool SkipProperty(PropertyInfo prop) {
			return Attribute.IsDefined(prop, typeof(SkipRuntimeSerialization));
		}
		static bool IsReadOnlyBuiltInType(PropertyInfo prop) {
			if(prop.GetSetMethod() != null) return false;
			return Array.Exists(builtInTypes, t => t.Equals(prop.PropertyType));
		}
		static Type[] builtInTypes = new Type[] { typeof(bool), typeof(byte), typeof(char), typeof(double), typeof(float), typeof(int), typeof(long), typeof(short), typeof(string) };
		static bool IsNonSerializableCollection(PropertyInfo prop) {
			if(prop.PropertyType.IsDescendantOf<ICollection>()) {
				return !CheckDesignerSerializationVisibility(prop, DesignerSerializationVisibility.Content);
			}
			return false;
		}
		static bool CheckDesignerSerializationVisibility(PropertyInfo prop, DesignerSerializationVisibility visibility) {
			DesignerSerializationVisibilityAttribute attr = Attribute.GetCustomAttribute(prop, typeof(DesignerSerializationVisibilityAttribute)) as DesignerSerializationVisibilityAttribute;
			return attr != null && attr.Visibility == visibility;
		}
		static bool IsPureObject(this Type dataType) {
			return dataType.Equals(typeof(object));
		}
		static bool IsAccessibilityObject(this Type dataType) {
			return dataType.IsDescendantOf<BaseAccessible>();
		}
		public static IEnumerable<PropertyInfo> GetPublicProperties<T>() {
			foreach(PropertyInfo property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
				yield return property;
			}
		}
		static bool IsPropertyHidden(PropertyInfo prop) {
			EditorBrowsableAttribute editorBrowsable = Attribute.GetCustomAttribute(prop, typeof(EditorBrowsableAttribute)) as EditorBrowsableAttribute;
			if(editorBrowsable != null && editorBrowsable.State == EditorBrowsableState.Never) {
				BrowsableAttribute bb = Attribute.GetCustomAttribute(prop, typeof(BrowsableAttribute)) as BrowsableAttribute;
				if(bb != null && !bb.Browsable) return true;
			}
			return false;
		}
	}
	public class RibbonFormStateObject {
		FormWindowState formState;
		FormBorderStyle formBorderStyle;
		bool expandCollapseItemEnabled;
		bool hasPanel;
		public RibbonFormStateObject(FormWindowState formState, FormBorderStyle formBorderStyle, bool expandCollapseItemEnabled, bool hasPanel) {
			this.formState = formState;
			this.formBorderStyle = formBorderStyle;
			this.expandCollapseItemEnabled = expandCollapseItemEnabled;
			this.hasPanel = hasPanel;
		}
		public virtual bool IsNull { get { return false; } }
		public FormWindowState FormState { get { return formState; } }
		public FormBorderStyle FormBorderStyle { get { return formBorderStyle; } }
		public bool ExpandCollapseItemEnabled { get { return expandCollapseItemEnabled; } }
		public bool HasPanel { get { return hasPanel; } }
	}
	public class NullRibbonFormStateObject : RibbonFormStateObject {
		public NullRibbonFormStateObject()
			: base(FormWindowState.Normal, FormBorderStyle.None, false, true) {
		}
		public override bool IsNull { get { return true; } }
	}
	public class RibbonFormHelper {
		public static RibbonFormStateObject CreateFormStateObject(Form form) {
			RibbonForm ribbonForm = form as RibbonForm;
			if(ribbonForm == null) return new NullRibbonFormStateObject();
			return DoCreateFormStateObject(ribbonForm);
		}
		static RibbonFormStateObject DoCreateFormStateObject(RibbonForm ribbonForm) {
			bool expandCollapseItemEnabled = false;
			bool hasPanel = true;
			if(ribbonForm.Ribbon != null) {
				expandCollapseItemEnabled = ribbonForm.Ribbon.ExpandCollapseItem.Enabled;
				hasPanel = (ribbonForm.Ribbon.ViewInfo.PanelHeight > 0);
			}
			return new RibbonFormStateObject(ribbonForm.WindowState, ribbonForm.FormBorderStyle, expandCollapseItemEnabled, hasPanel);
		}
		public static void ApplyFormStateObject(Form form, RibbonFormStateObject obj) {
			if(obj.IsNull) return;
			DoApplyFormStateObject((RibbonForm)form, obj);
		}
		static void DoApplyFormStateObject(RibbonForm ribbonForm, RibbonFormStateObject obj) {
			ribbonForm.FormBorderStyle = obj.FormBorderStyle;
			if(ribbonForm.Ribbon != null) {
				ribbonForm.Ribbon.ExpandCollapseItem.Enabled = obj.ExpandCollapseItemEnabled;
			}
		}
	}
	public class MouseClickInfo {
		Rectangle clickRect;
		public MouseClickInfo() {
			Reset();
		}
		public void MouseDown() {
			Point loc = Control.MousePosition;
			Size sz = SystemInformation.DoubleClickSize;
			clickRect = new Rectangle(loc.X - sz.Width / 2, loc.Y - sz.Height / 2, sz.Width, sz.Height);
		}
		public bool IsClick() {
			return clickRect.Contains(Control.MousePosition);
		}
		public bool IsReady { get { return !Equals(Empty); } }
		public void Reset() {
			this.clickRect = Rectangle.Empty;
		}
		static MouseClickInfo Empty = new MouseClickInfo();
		public override bool Equals(object obj) {
			MouseClickInfo sample = obj as MouseClickInfo;
			if(sample == null)
				return false;
			return clickRect.Equals(sample.clickRect);
		}
		public override int GetHashCode() {
			return clickRect.GetHashCode();
		}
	}
	public interface IBarCommandSupports {
		BarCommandContextBase CommandContext { get; }
	}
	public abstract class BarCommandContextBase {
		IComponent component;
		public BarCommandContextBase(IComponent component) {
			this.component = component;
		}
		public T AddBarItem<T>(string category, string caption, Action<T> initializer = null) where T : BarItem {
			T barItem = null;
			OnBegin();
			try {
				barItem = DesignerHost.CreateComponent(typeof(T)) as T;
				SetOwner(barItem, CheckCategory(category));
				barItem.Caption = caption;
				if(initializer != null) {
					initializer(barItem);
				}
				if(IsDesignTime) UpdateDesigner(barItem);
			}
			finally {
				OnEnd();
			}
			return barItem;
		}
		protected virtual string CheckCategory(string category) {
			return category ?? string.Empty;
		}
		protected virtual void OnBegin() {
			if(Component is ISupportInitialize) ((ISupportInitialize)Component).BeginInit();
		}
		protected virtual void OnEnd() {
			if(Component is ISupportInitialize) ((ISupportInitialize)Component).EndInit();
		}
		protected abstract void SetOwner(BarItem barItem, string category);
		protected bool IsDesignTime { get { return Component.Site != null && Component.Site.DesignMode; } }
		protected IDesignerHost DesignerHost {
			get { return IsDesignTime ? Component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost : null; }
		}
		protected virtual void UpdateDesigner(BarItem barItem) {
			IComponentChangeService svc = Component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(svc != null) {
				svc.OnComponentChanging(Component, null);
				svc.OnComponentChanged(Component, null, null, null);
			}
		}
		protected IComponent Component { get { return component; } }
	}
	public interface IDockableObject {
		Point FloatMousePosition { get; set; }
		Point FloatLocation { get; set; }
		Size FloatSize { get; }
		Size DockedSize { get; }
		bool IsDragging { get; set; }
		bool IsVisible { get; set; }
		CustomControl BarControl { get; set; }
		BarDockControl DockControl { get; }
		BarDockStyle DockStyle { get; set; }
		bool CanDock(BarDockControl dockControl);
		bool UseWholeRow { get; }
		void VisibleChanged();
		BarDockInfo DockInfo { get; set; }
		void ApplyDocking(BarDockInfo dockInfo);
		Size CalcSize(int width);
		Size CalcMinSize();
		Rectangle Bounds { get; set; }
		void SetVisible(bool visible);
	}
	public class BarLinksHolderSerializer {
		public static BarItemLink CreateItemLink(BarManager manager, XtraItemEventArgs e, BarLinksHolder holder) {
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo pi = e.Item.ChildProperties["ItemId"];
			if(pi == null) return null;
			BarItem item = manager.Items.FindById(Convert.ToInt32(pi.Value));
			if(item == null) return null;
			return holder.AddItem(item);
		}
	}
	public class DXToolbar : DXPopupXtraMenu {
		Bar bar;
		public DXToolbar(BarManager manager, DXPopupMenu menu) : base(manager, menu) {
			this.bar = null;
		}
		public Bar Bar { get { return bar; } }
		protected override void OnPopupHide(object sender, EventArgs e) {
			base.OnPopupHide(sender, e);
			if(Bar == null)
				return;
			Bar.Visible = false;
		}
		protected override void DestroyBarsMenu() {
			if(Bar != null) {
				Manager.Bars.Remove(Bar);
				Bar.Dispose();
			}
			this.bar = null;
			base.DestroyBarsMenu();
		}
		protected Bar FindToolbarByTag(DXPopupMenu Menu) {
			foreach(Bar b in Manager.Bars) { 
				DXToolbar tb = b.Tag as DXToolbar;
				if(tb != null && tb.Menu == Menu)
					return b;
			}
			return null;
		}
		public override void Show(Control control, Point pos) {
			DestroyBarsMenu();
			if(Bar == null) {
				this.bar = FindToolbarByTag(Menu);
				if(Bar == null)
					this.bar = new Bar();
				DestroyPreviousItems();
				Bar.Tag = this;
			}
			Bar.OptionsBar.AllowCaptionWhenFloating = false;
			Bar.Visible = false;
			Manager.Bars.Add(Bar);
			InitializeBar(control, pos);
		}
		protected virtual void DestroyPreviousItems() {
			DXToolbar tb = Bar.Tag as DXToolbar;
			if(tb != null)
				tb.DestroyMenuItems();
		}
		protected virtual void InitializeBar(Control control, Point pos) {
			Bar.DockInfo.DockStyle = BarDockStyle.None;
			Bar.FloatLocation = control.PointToScreen(new Point(pos.X + 5, pos.Y - 40));
			InitializeBarItemLinks();
			Bar.Visible = true;
		}
		protected virtual void InitializeBarItemLinks() {
			DestroyMenuItems();
			if(StandardMenu2Menu == null)
				this.fStandardMenu2Menu = new Hashtable();
			BarItem[] items = CreateMenuItems(Menu.Items);
			if(items == null || items.Length == 0) return;
			Bar.ItemLinks.AddRange(items);
			UpdateBeginGroup(Menu.Items, Bar.ItemLinks);
		}
	}
	public class DXRibbonMiniToolbar : DXPopupXtraMenu {
		RibbonMiniToolbar toolbar;
		RibbonControl ribbon;
		public DXRibbonMiniToolbar(RibbonControl ribbon, DXPopupMenu menu) : base(ribbon.Manager, menu) {
			this.toolbar = null;
			this.ribbon = ribbon;
		}
		public RibbonMiniToolbar Toolbar { get { return toolbar; } }
		public RibbonControl Ribbon { get { return ribbon; } }
		protected override void OnPopupHide(object sender, EventArgs e) {
			if(Toolbar == null)
				return;
			Toolbar.Enabled = false;
		}
		protected override void DestroyBarsMenu() {
			base.DestroyBarsMenu();
			if(Toolbar != null)
				Toolbar.Dispose();
			this.toolbar = null;
		}
		protected virtual RibbonMiniToolbar FindToolbarByTag(object tag) {
			foreach(RibbonMiniToolbar toolbar in Ribbon.MiniToolbars) {
				DXRibbonMiniToolbar miniToolbar = toolbar.Tag as DXRibbonMiniToolbar;
				if(miniToolbar != null && miniToolbar.Menu == Menu)
					return toolbar;
			}
			return null;
		}
		public override void Show(Control control, Point pos) {
			DestroyBarsMenu();
			if(Toolbar == null) {
				this.toolbar = FindToolbarByTag(Menu);
				if(Toolbar == null)
					this.toolbar = new RibbonMiniToolbar();
				DestroyPreviousMenuItems();
				Toolbar.Tag = this;
				Ribbon.MiniToolbars.Add(Toolbar);
			}
			InitializeToolbar(control, pos);
			MouseButtons buttons = Control.MouseButtons;
			if((buttons & MouseButtons.Right) != 0) Manager.IgnoreMouseUp++;
			if((buttons & MouseButtons.Left) != 0) Manager.IgnoreLeftMouseUp++;
			Toolbar.UpdateVisibility(control == null? pos: control.PointToScreen(pos));
		}
		protected virtual void DestroyPreviousMenuItems() {
			DXRibbonMiniToolbar mt = Toolbar.Tag as DXRibbonMiniToolbar;
			if(mt == null)
				return;
			mt.DestroyMenuItems();
		}
		protected virtual void InitializeToolbar(Control control, Point pos) {
			Toolbar.ParentControl = control;
			Toolbar.ShowPoint = pos;
			Toolbar.Alignment = Menu.Alignment;
			InitializeToolbarItemLinks();
		}
		protected virtual void InitializeToolbarItemLinks() {
			DestroyMenuItems();
			if(StandardMenu2Menu == null) 
				this.fStandardMenu2Menu = new Hashtable();
			BarItem[] items = CreateMenuItems(Menu.Items);
			if(items == null || items.Length == 0) return;
			Toolbar.ItemLinks.AddRange(items);
			Toolbar.Enabled = true;
			UpdateBeginGroup(Menu.Items, Toolbar.ItemLinks);
		}
		protected override BarItem CreateButtonGroupItem(DXMenuItem item) {
			BarButtonGroup group = new BarButtonGroup(Ribbon.Manager);
			DXButtonGroupItem dxgroup = (DXButtonGroupItem)item;
			group.ItemLinks.AddRange(CreateMenuItems(dxgroup.Items));
			return group;
		}
	}
	public class DXPopupXtraMenu : IDisposable {
		bool allowAutoDestroy = true;
		protected Hashtable fStandardMenu2Menu = null;
		PopupMenuBase barsMenu;
		BarManager manager;
		DXPopupMenu menu;
		ArrayList menuItems = null;
		public DXPopupXtraMenu(BarManager manager, DXPopupMenu menu) {
			this.manager = manager;
			this.menu = menu;
			this.menu.PopupHide += new EventHandler(OnPopupHide);
		}
		protected virtual void OnPopupHide(object sender, EventArgs e) {
			if(BarsMenu == null)
				return;
			BarsMenu.HidePopup();
		}
		public BarManager Manager { get { return manager; } }
		public bool AllowAutoDestroy { get { return allowAutoDestroy; } set { allowAutoDestroy = value; } }
		bool disposing;
		public virtual void Dispose() {
			if(this.disposing)
				return;
			try {
				this.disposing = true;
				if(this.menu == null)
					return;
				DestroyBarsMenu();
				menu.GenerateCloseUpEvent();
				this.manager = null;
				this.menu = null;
			}
			finally { this.disposing = false; }
		}
		protected virtual void DestroyBarsMenu() {
			DestroyMenuItems();
			if(BarsMenu != null) BarsMenu.Dispose();
			if(StandardMenu2Menu != null) StandardMenu2Menu.Clear();
			this.barsMenu = null;
		}
		protected virtual void DestroyMenuItems() {
			if(this.menuItems == null) return;
			foreach(BarItem item in this.menuItems) {
				item.Dispose();
			}
			this.menuItems.Clear();
			this.menuItems = null;
		}
		protected Hashtable StandardMenu2Menu { get { return fStandardMenu2Menu; } }
		protected internal PopupMenuBase BarsMenu { get { return barsMenu; } }
		public virtual DXPopupMenu Menu { get { return menu; } }
		public virtual void Show(Control control, Point pos) {
			Menu.GenerateBeforePopupEvent();
			CreateMenu();
			if(BarsMenu != null) {
				if(control != null && control.IsHandleCreated) pos = control.PointToScreen(pos);
				BarsMenu.ShowPopup(Manager, pos, Menu.OwnerPopup as PopupControl);
			}
		}
		void OnMenuCloseUp(object sender, EventArgs e) {
			DestroyBarsMenu();
			if(AllowAutoDestroy) Dispose();
		}
		protected virtual void CreateMenu() {
			DestroyBarsMenu();
			if(Menu.Items.Count == 0) return;
			if(StandardMenu2Menu == null) this.fStandardMenu2Menu = new Hashtable();
			this.barsMenu = CreatePopupMenu();
			BarItem[] items = CreateMenuItems(Menu.Items);
			if(items == null || items.Length == 0) {
				DestroyBarsMenu();
				return;
			}
			this.barsMenu.AddItems(items);
			UpdateBeginGroup(Menu.Items, this.barsMenu.ItemLinks);
		}
		protected virtual PopupMenuBase CreatePopupMenu() {
			if(WindowsFormsSettings.PopupMenuStyle == PopupMenuStyle.RadialMenu) return CreateRadialMenuCore(Manager);
			return CreatePopupMenuCore(Manager);
		}
		protected virtual PopupMenuBase CreatePopupMenuCore(BarManager manager) {
			PopupMenu menu = new PopupMenu(manager);
			menu.MenuCreator = Menu;
			menu.MenuCaption = Menu.Caption;
			menu.AllowRibbonQATMenu = false;
			menu.CloseUp += new EventHandler(OnMenuCloseUp);
			menu.OptionsMultiColumn.Assign(Menu.OptionsMultiColumn);
			menu.MultiColumn = Menu.MultiColumn;
			return menu;
		}
		protected virtual PopupMenuBase CreateRadialMenuCore(BarManager manager) {
			RadialMenu radialMenu = new RadialMenu(manager);
			radialMenu.AutoExpand = true;
			return radialMenu;
		}
		int SubMenuLevel { get; set; }
		protected virtual BarItem[] CreateMenuItems(DXMenuItemCollection collection) {
			if(SubMenuLevel == 6)
				return new BarItem[] { };
			SubMenuLevel++;
			try {
				if(collection.Count == 0) return null;
				int visibleItemCount = collection.VisibleItemCount;
				ArrayList list = new ArrayList();
				for(int n = 0; n < collection.Count; n++) {
					DXMenuItem item = collection[n];
					if(!IsMenuItemAcceptable(item, visibleItemCount)) continue;
					BarItem bItem = CreateMenuItem(item);
					if(bItem != null) {
						if(this.menuItems == null) this.menuItems = new ArrayList();
						if(bItem is BarButtonItem) {
							BarButtonItem button = (BarButtonItem)bItem;
							button.CloseRadialMenuOnItemClick = true;
						}
						this.menuItems.Add(bItem);
						list.Add(bItem);
					}
				}
				return list.ToArray(typeof(BarItem)) as BarItem[];
			}
			finally {
				SubMenuLevel--;
			}
		}
		protected virtual bool IsMenuItemAcceptable(DXMenuItem menuItem, int count) {
			if(!menuItem.Visible) return false;
			if(BarsMenu is ISupportMenuItemFilter) {
				ISupportMenuItemFilter filter = (ISupportMenuItemFilter)BarsMenu;
				if(!filter.IsItemAcceptable(menuItem, count)) return false;
			}
			return true;
		}
		protected virtual BarItem CreateButtonGroupItem(DXMenuItem item) {
			BarLinkContainerExItem group = new BarLinkContainerExItem();
			Manager.Items.Add(group);
			DXButtonGroupItem dxgroup = (DXButtonGroupItem)item;
			group.ItemLinks.AddRange(CreateMenuItems(dxgroup.Items));
			return group;
		}
		protected virtual BarItem CreateMenuItem(DXMenuItem item) {
			BarItem res = null;
			if(item is DXButtonGroupItem) return CreateButtonGroupItem(item);
			if(item is DXEditMenuItem) res = CreateEditMenuItem(item);
			if(item is DXSubMenuItem) res = CreateSubItem(item);
			if(item is DXMenuCheckItem) res = CreateCheckItem(item);
			if(item is DXMenuHeaderItem) res = CreateHeaderItem(item);
			if(res == null) res = CreateButtonItem(item);
			SubscribeEvents(res);
			StandardMenu2Menu[res] = item;
			return res;
		}
		protected virtual BarItem CreateHeaderItem(DXMenuItem item) {
			DXMenuHeaderItem header = (DXMenuHeaderItem)item;
			BarHeaderItem barItem = new BarHeaderItem(Manager, true);
			barItem.MultiColumn = header.MultiColumn;
			AssignProperties(header, barItem);
			barItem.OptionsMultiColumn.Assign(header.OptionsMultiColumn);
			return barItem;
		}
		protected virtual BarItem CreateEditMenuItem(DXMenuItem item) {
			DXEditMenuItem editItem = (DXEditMenuItem)item;
			BarEditItem res = new BarEditItem(Manager, editItem.Edit);
			res.EditValue = editItem.EditValue;
			if(editItem.Width != -1)
				res.Width = editItem.Width;
			res.EditHeight = editItem.Height;
			AssignProperties(editItem, res);
			return res;
		}
		protected virtual BarItem CreateSubItem(DXMenuItem item) {
			DXSubMenuItem subItem = (DXSubMenuItem)item;
			BarSubItem bItem = new BarSubItem(Manager, true);
			AssignProperties(item, bItem);
			subItem.GenerateBeforePopupEvent();
			BarItem[] subItems = CreateMenuItems(subItem.Items);
			if(subItems != null && subItems.Length > 0) {
				bItem.AddItems(subItems);
				UpdateBeginGroup(subItem.Items, bItem.ItemLinks);
			}
			return bItem;
		}
		protected virtual BarButtonItem CreateButtonItem(DXMenuItem item) {
			BarButtonItem bItem = new BarButtonItem(Manager, true);
			bItem.CloseSubMenuOnClick = item.CloseMenuOnClick;
			AssignProperties(item, bItem);
			return bItem;
		}
		protected virtual BarItem CreateCheckItem(DXMenuItem item) {
			DXMenuCheckItem checkItem = (DXMenuCheckItem)item;
			BarButtonItem bItem = CreateButtonItem(item);
			bItem.CloseSubMenuOnClick = item.CloseMenuOnClick;
			bItem.ButtonStyle = BarButtonStyle.Check;
			bItem.Down = checkItem.Checked;
			return bItem;
		}
		void AssignProperties(DXMenuItem item, BarItem bItem) {
			bItem.Enabled = item.Enabled;
			bItem.Caption = item.Caption;
			bItem.AllowGlyphSkinning = item.AllowGlyphSkinning ? DefaultBoolean.True : DefaultBoolean.Default;
			bItem.Glyph = item.Image;
			bItem.GlyphDisabled = item.ImageDisabled;
			bItem.LargeGlyph = item.LargeImage;
			bItem.LargeGlyphDisabled = item.LargeImageDisabled;
			bItem.SuperTip = item.SuperTip;
			bItem.Appearance.Assign(item.Appearance);
			bItem.ItemInMenuAppearance.Normal.Assign(item.Appearance);
			bItem.ItemInMenuAppearance.Hovered.Assign(item.AppearanceHovered);
			bItem.ItemInMenuAppearance.Pressed.Assign(item.AppearanceHovered);
			bItem.ItemInMenuAppearance.Disabled.Assign(item.AppearanceDisabled);
			if(item.Shortcut != Shortcut.None) bItem.ItemShortcut = new BarShortcut(item.Shortcut);
			bItem.Tag = item.Tag;
		}
		protected virtual void UpdateBeginGroup(DXMenuItemCollection items, BarItemLinkCollection links) {
			foreach(DXMenuItem item in items) {
				if(item.BeginGroup) {
					for(int n = 0; n < links.Count; n++) {
						BarItemLink link = links[n];
						if(object.Equals(StandardMenu2Menu[link.Item], item)) {
							link.BeginGroup = true;
							break;
						}
					}
				}
			}
		}
		protected virtual void SubscribeEvents(BarItem bItem) {
			bItem.ItemClick += new ItemClickEventHandler(OnBarMenuClick);
			BarEditItem editItem = bItem as BarEditItem;
			if(editItem != null)
				editItem.EditValueChanged += new EventHandler(OnBarEditValueChanged);
		}
		void OnBarEditValueChanged(object sender, EventArgs e) {
			BarEditItem bItem = sender as BarEditItem;
			if(bItem == null) return;
			DXEditMenuItem item = GetItemFromStandardItem(bItem) as DXEditMenuItem;
			if(item == null) return;
			item.EditValue = bItem.EditValue;
		}
		protected virtual void UpdateFromMenuItem(BarItem bItem, DXMenuItem item) {
			DXMenuCheckItem checkItem = item as DXMenuCheckItem;
			if(checkItem != null) {
				BarButtonItem buttonItem = (BarButtonItem)bItem;
				checkItem.Checked = buttonItem.Down;
				return;
			}
		}
		protected virtual DXMenuItem GetItemFromStandardItem(BarItem item) {
			if(StandardMenu2Menu == null) return null;
			return StandardMenu2Menu[item] as DXMenuItem;
		}
		protected virtual void OnBarMenuClick(object sender, ItemClickEventArgs e) {
			BarItem bItem = e.Item;
			if(bItem == null) return;
			DXMenuItem item = GetItemFromStandardItem(bItem);
			if(item == null) return;
			item.GenerateClickEvent();
		}
	}
	public class FocusHelper {
		static IntPtr focusedComponent;
		public static IntPtr GetFocus() {
			return BarNativeMethods.GetFocus();
		}
		public static void SetFocus(HandleRef hWnd) {
			BarNativeMethods.SetFocus(hWnd);
		}
		public static void SaveFocus() {
			focusedComponent = BarNativeMethods.GetFocus();
		}
		public static void RestoreFocus() {
			BarNativeMethods.SetFocus(new HandleRef(Control.FromHandle(focusedComponent), focusedComponent));
		}
	}
	public class CachedColoredImage : IDisposable {
		Image image;
		Color color;
		public CachedColoredImage(Image image) {
			this.image = DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, DefaultColor);
			this.color = DefaultColor;
		}
		public Image GetImage(Color cr) {
			if(this.color != cr) {
				Image prev = this.image;
				this.image = DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, cr);
				this.color = cr;
				if(prev != null) prev.Dispose();
			}
			return image;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(image != null) image.Dispose();
				image = null;
			}
		}
		public static readonly Color DefaultColor = Color.Black;
	}
}
namespace DevExpress.XtraBars.Controls {
	public interface IDisposableEx : IDisposable {
		bool Destroying { get; }
	}
	public class MouseInfoArgs : MouseEventArgs {
		bool mouseUp;
		Point screenPoint;
		public MouseInfoArgs(MouseEventArgs e, bool isMouseUp)
			: base(e.Button, e.Clicks, e.X, e.Y, e.Delta) {
			this.mouseUp = isMouseUp;
			this.screenPoint = new Point(e.X, e.Y);
		}
		public virtual bool MouseUp { get { return mouseUp; } }
		public virtual bool MouseDown { get { return !MouseUp; } }
		public virtual Point ScreenPoint { get { return screenPoint; } }
	}
	[Flags]
	public enum BarMenuCloseType {
		None = 0,
		All = 1,
		Standard = 2,
		AllExceptMinimized = 4,
		AllExceptMiniToolbars = 8,
		AllExceptMiniToolbarsAndDXToolbars = 16,
		KeepPopupContainer = 32
	}
	public interface IBarObject {
		bool IsBarObject { get; }
		BarManager Manager { get; }
		BarMenuCloseType ShouldCloseMenuOnClick(MouseInfoArgs e, Control child);
		bool ShouldCloseOnOuterClick(Control control, MouseInfoArgs e);
		bool ShouldCloseOnLostFocus(Control newFocus);
	}
	public interface IBarObjectContainer {
		bool ContainsObject { get; }
		IBarObject BarObject { get; }
	}
	public interface ICustomBarControl {
		void ProcessKeyDown(KeyEventArgs e);
		bool ProcessKeyPress(KeyPressEventArgs e);
		bool IsNeededKey(KeyEventArgs e);
		bool IsInterceptKey(KeyEventArgs e);
		void ProcessKeyUp(KeyEventArgs e);
		Control Control { get; }
	}
}
namespace DevExpress.XtraBars.Registration {
	public static class BarEditorsRepositoryItemRegistrator {
		public static void Register() {
			ColorPickEditRepositoryItemRegistrator.Register();
			RepositoryItemPopupGalleryEdit.RegisterItem();
		}
	}
}
