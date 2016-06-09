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
using DevExpress.Utils.Frames;
using System.Drawing;
using DevExpress.LookAndFeel;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Skins;
using System.Collections;
using System.Reflection;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils;
using DevExpress.XtraEditors.ButtonsPanelControl;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraEditors.Designer.Utils {
	class ReflectorHelper {
		public static Type PropertyGridViewType(Control control) { return control.GetType(); }
		public static FieldInfo FieldInfo(Type type, string name, BindingFlags flag) {
			return type.GetField(name, flag);
		}
		public static MethodInfo MethodInfo(Type type, string name, BindingFlags flag) {
			return type.GetMethod(name, flag);
		}
		public static PropertyInfo PropertyInfo(Type type, string name, BindingFlags flag) {
			return type.GetProperty(name, flag);
		}
	}
	interface IPaintInterface {
		Color BackColor { get; }
		Color ForeColor { get; }
		Color LineColor { get; }
		Color ViewColor { get; }
		Color SelectColor { get; }
		Color SelectForeColor { get; }
	}
	interface IDXPropertyTabEx {
		string Filter { get; set; }
	}
	public class DXPropertiesTabEx : System.Windows.Forms.PropertyGridInternal.PropertiesTab, IDXPropertyTabEx {
		Bitmap bmpCore;
		string filterCore;
		string IDXPropertyTabEx.Filter {
			get { return filterCore; }
			set { filterCore = value; }
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes) {
			GridItem gridItem = context as GridItem;
			var res = base.GetProperties(context, component, attributes);
			return PropertiesDescriptorSearchHelper.GetProperties(gridItem, component, filterCore, res);
		}
		public override Bitmap Bitmap {
			get {
				if(bmpCore == null) {
					Type tabType = typeof(System.Windows.Forms.PropertyGridInternal.PropertiesTab);
					string resource = tabType.Name + ".bmp";
					try {
						this.bmpCore = new Bitmap(tabType, resource);
					}
					catch(Exception) {
					}
				}
				return bmpCore;
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(bmpCore != null)
				bmpCore.Dispose();
			bmpCore = null;
		}
	}
	public class DXEventsTabEx : System.Windows.Forms.Design.EventsTab, IDXPropertyTabEx {
		Bitmap bmpCore;
		string filterCore;
		string IDXPropertyTabEx.Filter {
			get { return filterCore; }
			set { filterCore = value; }
		}
		public DXEventsTabEx(IServiceProvider provider) : base(provider) { }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes) {
			GridItem gridItem = context as GridItem;
			var res = base.GetProperties(context, component, attributes);
			return PropertiesDescriptorSearchHelper.GetProperties(gridItem, component, filterCore, res);
		}
		public override Bitmap Bitmap {
			get {
				if(bmpCore == null) {
					Type tabType = typeof(System.Windows.Forms.Design.EventsTab);
					string resource = tabType.Name + ".bmp";
					try {
						this.bmpCore = new Bitmap(tabType, resource);
					}
					catch(Exception) {
					}
				}
				return bmpCore;
			}
		}
		public override void Dispose() {
			base.Dispose();
			if(bmpCore != null)
				bmpCore.Dispose();
			bmpCore = null;
		}
	}
	[ToolboxItem(false)]
	public class DXPropertyGridEx : PropertyGridEx, ISupportLookAndFeel, IPropertyGridSearchClient {
		const int toolBarHeigth = 32;
		FilterButtonPanel toolBarButtonPanel;
		UserLookAndFeel lookAndFeel;
		BasePainter painter;
		PropertyGridViewPainter gridPainter;
		Color gridLineColorCore = Color.White, selectColorCore = SystemColors.Highlight, selectForeColorCore = SystemColors.HighlightText;
		object[] selectedObjectsCore;
		public new object SelectedObject {
			get {
				if(this.SelectedObjects == null || this.SelectedObjects.Length == 0) return null;
				return this.SelectedObjects[0];
			}
			set {
				if(this.SelectedObject == value) return;
				if(value == null) this.SelectedObjects = new object[0];
				else this.SelectedObjects = new object[] { value };
			}
		}
		public new object[] SelectedObjects {
			get { return selectedObjectsCore; }
			set {
				if(SelectedObjects == value) return;
				selectedObjectsCore = value;
				OnSelectedChanged(value);
			}
		}
		protected override Type DefaultTabType { get { return typeof(DXPropertiesTabEx); } }
		protected virtual void OnSelectedChanged(object[] filterObjects) {
			base.SelectedObjects = filterObjects;
		}
		public bool ShowSearchPanel {
			get { return toolBarButtonPanel.SearchControlVisible; }
			set { toolBarButtonPanel.SearchControlVisible = value; }
		}
		public DXPropertyGridEx()
			: base() {
			InizializeComponent();
			this.painter = CreatePainter();
			AssignHandle();
			GridView.SizeChanged += GridViewSizeChanged;
			ToolStrip.SizeChanged += SetSizeToolStrip;
		}
		protected override void Dispose(bool disposing) {
			GridView.SizeChanged -= GridViewSizeChanged;
			ToolStrip.SizeChanged -= SetSizeToolStrip;
			lookAndFeel.StyleChanged -= OnLookAndFeelChanged;
			if(gridPainter != null)
				gridPainter.Dispose();
			this.gridPainter = null;
			if(toolBarButtonPanel != null)
				toolBarButtonPanel.Dispose();
			this.toolBarButtonPanel = null;
			base.Dispose(disposing);
		}
		void SetSizeToolStrip(object sender, EventArgs e) {
			ToolStrip.Size = new Size(this.Width, toolBarHeigth);
			this.toolBarButtonPanel.Size = ToolStrip.Size;
		}
		Rectangle CalcViewBounds() {
			int y = ToolBarButtonPanel.Visible ? ToolBarButtonPanel.Bounds.Bottom - 1 : 0;
			int doccomentHeight = Doccomment.Visible ? Doccomment.Height : 0;
			int toolBarButtonPanelHeight = ToolBarButtonPanel.Visible ? ToolBarButtonPanel.Height - 1 : 0;
			int height = Height - doccomentHeight - toolBarButtonPanelHeight;
			return new Rectangle(GridView.Bounds.X, y, GridView.Width, height);
		}
		void GridViewSizeChanged(object sender, EventArgs e) {
			GridView.Bounds = CalcViewBounds();
		}
		void AssignHandle() {
			this.gridPainter = new PropertyGridViewPainter(GridView);
			this.gridPainter.AssignHandle(GridView.Handle);
			DoccommentPainter doccomentPainter = new DoccommentPainter(Doccomment);
			doccomentPainter.AssignHandle(Doccomment.Handle);
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			painter = CreatePainter();
		}
		protected BasePainter CreatePainter() {
			switch(LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Office2003: return new Office2003Painter(this);
				case ActiveLookAndFeelStyle.Skin: return new SkinPainter(this, LookAndFeel.ActiveLookAndFeel);
			}
			return new BasePainter(this);
		}
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == 0x000B && toolBarButtonPanel != null) {
				toolBarButtonPanel.BringToFront();
				painter.DrawStyle();
			}
			base.WndProc(ref msg);
		}
		protected override void OnResize(EventArgs e) {
			gridPainter.LockScrollValueChanged();
			base.OnResize(e);
			CalcViewBounds();
			gridPainter.UnlockScrollValueChanged();
		}
		void InizializeComponent() {
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
			this.toolBarButtonPanel = new FilterButtonPanel();
			this.toolBarButtonPanel.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
			new ButtonControl("Button", ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.PBCatego.png", typeof(DXPropertyGridEx).Assembly), -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, null, false, -1, true, null, true, true, true, null, ViewSortButtons[0], 1, false),
			new ButtonControl("Button",  ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.PBAlpha.png", typeof(DXPropertyGridEx).Assembly), -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, null, false, -1, true, null, true, false, true, null, ViewSortButtons[1], 1, false),
			new ButtonControl("Button", ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.PBProp.png", typeof(DXPropertyGridEx).Assembly), -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, null, false, -1, true, null, true, false, false, null, null, 0, false),
			new ButtonControl("Button", ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.PBEvent.png", typeof(DXPropertyGridEx).Assembly), -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.CheckButton, null, false, -1, true, null, true, false, false, null, null, 0, false)});
			this.toolBarButtonPanel.ContentAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolBarButtonPanel.Padding = new Padding(4, 4, 4, 4);
			this.toolBarButtonPanel.ButtonInterval = 2;
			this.toolBarButtonPanel.Location = Point.Empty;
			this.toolBarButtonPanel.Name = "toolBarButtonPanel";
			this.toolBarButtonPanel.Size = this.ToolStrip.Size;
			this.toolBarButtonPanel.TabIndex = 5;
			this.toolBarButtonPanel.Text = "toolBarButtonPanel";
			this.toolBarButtonPanel.ButtonChecked += new BaseButtonEventHandler(toolBarButtonPanel_ButtonChecked);
			this.toolBarButtonPanel.Client = this;
			this.Controls.Add(toolBarButtonPanel);
		}
		protected override void OnPropertySortChanged(EventArgs e) {
			if(ViewSortButtons.Length > 0) {
				for(int i = 0; i < 2; i++) {
					(toolBarButtonPanel.Buttons[i] as ISupportGroupUpdate).LockCheckEvent();
					toolBarButtonPanel.Buttons[i].Properties.Checked = (toolBarButtonPanel.Buttons[i].Properties.Tag as ToolStripButton).Checked;
					(toolBarButtonPanel.Buttons[i] as ISupportGroupUpdate).UnlockCheckEvent();
				}
			}
		}
		void toolBarButtonPanel_ButtonChecked(object sender, BaseButtonEventArgs e) {
			PerformButtonClick(e.Button);
		}
		void PerformButtonClick(IBaseButton button) {
			ToolStripButton toolStripButton = button.Properties.Tag as ToolStripButton;
			if(toolStripButton != null)
				toolStripButton.PerformClick();
		}
		public void ShowTabEvents(bool show) {
			base.ShowEvents(show);
			if(ViewTabButtons.Length > 0) {
				for(int i = 0; i < ViewTabButtons.Length; i++) {
					ToolStripButton button = ViewTabButtons[i];
					ChangeShowTab(toolBarButtonPanel.Buttons[i + 2], button, i == 0);
				}
			}
		}
		public void MoveSplitterTo(int xpos) {
			if(GridView == null) return;
			MethodInfo mi = GridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);
			if(mi != null)
				mi.Invoke(GridView, new object[] { xpos });
		}
		void ChangeShowTab(IBaseButton button, ToolStripButton stripButton, bool checkedCore) {
			button.Properties.Tag = stripButton;
			stripButton.Tag = button;
			button.Properties.Visible = stripButton.Visible;
			button.Properties.Checked = checkedCore;
		}
		protected internal Color GridLineColor { get { return gridLineColorCore; } set { gridLineColorCore = value; } }
		protected internal Color GridSelectColor { get { return selectColorCore; } set { selectColorCore = value; } }
		protected internal Color GridSelectForeColor { get { return selectForeColorCore; } set { selectForeColorCore = value; } }
		internal FilterButtonPanel ToolBarButtonPanel { get { return toolBarButtonPanel; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		[ Category("Appearance"), DefaultValue(true)]
		public bool AllowDrawBorder {
			get { return !(Parent is DevExpress.XtraTab.XtraTabPage); }
		}
		public override bool ToolbarVisible {
			get { return base.ToolbarVisible; }
			set {
				toolBarButtonPanel.Visible = value;
				base.ToolbarVisible = value;
			}
		}
		internal new Control GridView { get { return base.GridView as Control; } }
		protected internal Control Doccomment {
			get {
				return ReflectorHelper.FieldInfo(typeof(PropertyGrid), "doccomment", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as Control;
			}
		}
		ToolStrip ToolStrip {
			get {
				return ReflectorHelper.FieldInfo(typeof(PropertyGrid), "toolStrip",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as ToolStrip;
			}
		}
		ToolStripButton[] ViewSortButtons {
			get {
				return ReflectorHelper.FieldInfo(typeof(PropertyGrid), "viewSortButtons",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as ToolStripButton[];
			}
		}
		new ToolStripButton[] ViewTabButtons {
			get {
				return ReflectorHelper.FieldInfo(typeof(PropertyGrid), "viewTabButtons",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as ToolStripButton[];
			}
		}
		#region Hide property PropertyGrid
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color CategoryForeColor { get { return base.CategoryForeColor; } set { base.CategoryForeColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color CommandsActiveLinkColor { get { return base.CommandsActiveLinkColor; } set { base.CommandsActiveLinkColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color CommandsBackColor { get { return base.CommandsBackColor; } set { base.CommandsBackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color CommandsDisabledLinkColor { get { return base.CommandsDisabledLinkColor; } set { base.CommandsDisabledLinkColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color CommandsForeColor { get { return base.CommandsForeColor; } set { base.CommandsForeColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color CommandsLinkColor { get { return base.CommandsLinkColor; } set { base.CommandsLinkColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color HelpBackColor { get { return base.HelpBackColor; } set { base.HelpBackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color HelpForeColor { get { return base.HelpForeColor; } set { base.HelpForeColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color LineColor { get { return base.LineColor; } set { base.LineColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ViewBackColor { get { return base.ViewBackColor; } set { base.ViewBackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color ViewForeColor { get { return base.ViewForeColor; } set { base.ViewForeColor = value; } }
		#endregion
		#region ISearchControlClient Members
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			FilterObjectsInfo info = searchInfo as FilterObjectsInfo;
			if(info == null) return;
			string filter = info.SearchText;
			if(PropertyTabs == null) return;
			int refresh = 0;
			foreach(System.Windows.Forms.Design.PropertyTab tab in PropertyTabs) {
				IDXPropertyTabEx tabEx = tab as IDXPropertyTabEx;
				if(tabEx == null) continue;
				if(tabEx.Filter == filter) continue;
				tabEx.Filter = filter;
				refresh++;
			}
			if(refresh > 0)
				Refresh();
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new FilterObjectProvider(this);
		}
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) { }
		bool ISearchControlClient.IsAttachedToSearchControl {
			get { return toolBarButtonPanel is FilterButtonPanel; }
		}
		#endregion
	}
	class DoccommentPainter : NativeWindow {
		Control control;
		DXPropertyGridEx Parent { get { return control.Parent as DXPropertyGridEx; } }
		public DoccommentPainter(Control control)
			: base() {
			this.control = control;
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if(m.Msg == 0x000F) {
				control.Bounds = new Rectangle(0, Parent.Height - Heigth - 1, Parent.Size.Width, Heigth);
				using(Graphics g = Graphics.FromHwnd(control.Handle))
					OnPaint(g);
			}
		}
		void OnPaint(Graphics g) {
			Color borderColor = Parent.AllowDrawBorder ? Parent.GridLineColor : Parent.ViewBackColor;
			g.DrawRectangle(new Pen(borderColor, 1), new Rectangle(new Point(0, 0), new Size(control.Width - 1, control.Height - 1)));
			g.DrawLine(new Pen(Parent.GridLineColor, 1), new Point(0, 0), new Point(control.Width - 1, 0));
		}
		int Heigth {
			get { return Parent.Height - Parent.GridView.Height - (Parent.ToolBarButtonPanel.Visible ? Parent.ToolBarButtonPanel.Height - 1 : 0); }
		}
	}
	enum DrawingOptions {
		PRF_CHECKVISIBLE = 1,
		PRF_NONCLIENT = 2,
		PRF_CLIENT = 4,
		PRF_ERASEBKGND = 8,
		PRF_CHILDREN = 16,
		PRF_OWNED = 32
	}
	class PropertyGridViewPainter : NativeWindow {
		Control control;
		VScrollBar scrollBarCore;
		DXPropertyGridEx parent;
		public PropertyGridViewPainter(Control control)
			: base() {
			this.control = control;
			this.parent = control.Parent as DXPropertyGridEx;
			parent.LookAndFeel.StyleChanged += new EventHandler(lookAndFeelStyleChanged);
			this.rootScrollBar = GetScrollBar();
			this.rootScrollBar.SizeChanged += new EventHandler(rootScrollBarSizeChanged);
			this.rootScrollBar.ValueChanged += new EventHandler(rootScrollBarValueChanged);
			this.rootScrollBar.VisibleChanged += new EventHandler(rootScrollBarVisibleChanged);
			this.scrollBarCore = new VScrollBar();
			this.scrollBarCore.ValueChanged += new EventHandler(scrollBarCoreValueChanged);
			this.control.Controls.Remove(this.rootScrollBar);
			this.control.Controls.Add(this.scrollBarCore);
		}
		internal void Dispose() {
			if(parent != null) {
				parent.LookAndFeel.StyleChanged -= new EventHandler(lookAndFeelStyleChanged);
				parent = null;
			}
			if(rootScrollBar != null) {
				this.rootScrollBar.SizeChanged -= new EventHandler(rootScrollBarSizeChanged);
				this.rootScrollBar.ValueChanged -= new EventHandler(rootScrollBarValueChanged);
				this.rootScrollBar.VisibleChanged -= new EventHandler(rootScrollBarVisibleChanged);
			}
			this.rootScrollBar = null;
			this.control = null;
			this.parent = null;
		}
		DXPropertyGridEx Parent { get { return parent; } }
		public void LockScrollValueChanged() { lockValueChanged++; }
		public void UnlockScrollValueChanged() {
			rootScrollBar.Value = scrollBarCore.Value;
			lockValueChanged--;
			if(lockValueChanged == 0)
				Refresh();
		}
		void ScrollBarRefresh() {
			scrollBarCore.Bounds = rootScrollBar.Bounds;
			scrollBarCore.Dock = rootScrollBar.Dock;
			scrollBarCore.Minimum = rootScrollBar.Minimum;
			scrollBarCore.SmallChange = rootScrollBar.SmallChange;
			scrollBarCore.Maximum = rootScrollBar.Maximum;
			scrollBarCore.LargeChange = rootScrollBar.LargeChange;
		}
		public void Refresh() {
			ScrollBarRefresh();
			if(lockValueChanged == 0)
				scrollBarCore.Value = rootScrollBar.Value;
			MethodInfo info = control.GetType().GetMethod("OnResize", BindingFlags.NonPublic | BindingFlags.Instance);
			if(info != null)
				info.Invoke(control, new object[] { EventArgs.Empty });
		}
		void rootScrollBarVisibleChanged(object sender, EventArgs e) {
			scrollBarCore.Visible = rootScrollBar.Visible;
		}
		int lockValueChanged = 0;
		void rootScrollBarValueChanged(object sender, EventArgs e) {
			if(lockValueChanged == 0)
				scrollBarCore.Value = rootScrollBar.Value;
		}
		void lookAndFeelStyleChanged(object sender, EventArgs e) {
			this.scrollBarCore.LookAndFeel.UseDefaultLookAndFeel = Parent.LookAndFeel.UseDefaultLookAndFeel;
			this.scrollBarCore.LookAndFeel.UseWindowsXPTheme = Parent.LookAndFeel.UseWindowsXPTheme;
			this.scrollBarCore.LookAndFeel.Style = Parent.LookAndFeel.Style;
			this.scrollBarCore.LookAndFeel.SetSkinStyle(Parent.LookAndFeel.ActiveSkinName);
		}
		void rootScrollBarSizeChanged(object sender, EventArgs e) {
			ScrollBarRefresh();
		}
		void scrollBarCoreValueChanged(object sender, EventArgs e) {
			if(rootScrollBar.Value != scrollBarCore.Value)
				OnScroll();
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_PAINT) {
				DoPaint();
				m.Result = IntPtr.Zero;
				return;
			}
			base.WndProc(ref m);
		}
		void DoPaint() {
			NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
			BufferedGraphicsContext contextGraphics = BufferedGraphicsManager.Current;
			contextGraphics.MaximumBuffer = new Size(control.Width + 1, control.Height + 1);
			try {
				IntPtr dc = NativeMethods.BeginPaint(Handle, ref ps);
				using(BufferedGraphics bufferedGraphics = contextGraphics.Allocate(dc, new Rectangle(0, 0, control.Width, control.Height))) {
					OnPrint(bufferedGraphics.Graphics);
					OnPaint(bufferedGraphics.Graphics);
					bufferedGraphics.Render();
				}
				rootScrollBarSizeChanged(rootScrollBar, new EventArgs());
			}
			finally {
				NativeMethods.EndPaint(Handle, ref ps);
			}
		}
		void OnPrint(Graphics g) {
			IntPtr hdc = g.GetHdc();
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(control.Handle, MSG.WM_PRINT, hdc, new IntPtr((int)(DrawingOptions.PRF_CHILDREN | DrawingOptions.PRF_CLIENT | DrawingOptions.PRF_NONCLIENT | DrawingOptions.PRF_OWNED)));
			g.ReleaseHdc(hdc);
		}
		void OnPaint(Graphics g) {
			try {
				DrawBorderGrid(g);
				if(TotalProps > 0) {
					int level = 0;
					foreach(var cell in Entry) {
						GridItem item = cell as GridItem;
						PaintCategory(g, item, ref level);
						if(Parent.PropertySort == PropertySort.Categorized || Parent.PropertySort == PropertySort.CategorizedAlphabetical)
							DrawLineBetweenCategory(g, item, level);
					}
					if(scrollBarCore.Visible)
						g.FillRectangle(new SolidBrush(Parent.GridLineColor), control.Width - scrollBarCore.Width - 2, 0, 1, control.Height);
				}
			}
			catch {
			}
		}
		void PaintCategory(Graphics graphics, GridItem gridItem, ref int level) {
			if(Parent.PropertySort == PropertySort.Alphabetical || Parent.PropertySort == PropertySort.NoSort)
				DrawGridItem(graphics, gridItem, level);
			level = DrawExpandedItem(graphics, gridItem, level, false);
		}
		void DrawBorderGrid(Graphics graphics) {
			Color backColor = Parent.AllowDrawBorder ? Parent.GridLineColor : Parent.ViewBackColor;
			graphics.DrawRectangle(new Pen(backColor), new Rectangle(new Point(0, 0), new Size(control.Size.Width - 1, control.Height - 1)));
			graphics.DrawLine(new Pen(Parent.GridLineColor), new Point(0, 0), new Point(control.Width - 1, 0));
		}
		void DrawLineBetweenCategory(Graphics graphics, GridItem item, int level) {
			if(item.Expanded) graphics.FillRectangle(new SolidBrush(Parent.ViewBackColor), 1, 1 - ScrollOffset + LevelOffset(level), GetOutlineIconSize() + 3, 1);
			else graphics.FillRectangle(new SolidBrush(Parent.ViewBackColor), 1, 1 - ScrollOffset + LevelOffset(level), control.Width - 2, 1);
		}
		void DrawGridItem(Graphics graphics, GridItem item, int level) {
			graphics.DrawRectangle(new Pen(Parent.GridLineColor, 1), CalcRectangleLabelCell(item, level));
			if(item == Parent.SelectedGridItem) DrawSelectItem(graphics, item, level);
			Rectangle cellBounds = CalcRectangleValueCell(item, level);
			graphics.DrawRectangle(new Pen(Parent.GridLineColor, 1), CalcRectangleValueCell(item, level));
			if(!Parent.AllowDrawBorder)
				graphics.DrawLine(new Pen(Parent.ViewBackColor), new Point(cellBounds.Right, cellBounds.Top), new Point(cellBounds.Right, cellBounds.Bottom));
		}
		void DrawSelectItem(Graphics graphics, GridItem item, int level) {
			graphics.FillRectangle(new SolidBrush(Parent.GridSelectColor), CalcRectangleSelectCell(item, level));
			TextRenderer.DrawText(graphics, Parent.SelectedGridItem.Label, Parent.Font, CalcRectangleStringCell(item, level), Parent.GridSelectForeColor, (TextFormatFlags.PreserveGraphicsTranslateTransform | TextFormatFlags.PreserveGraphicsClipping));
		}
		int DrawExpandedItem(Graphics graphics, GridItem item, int level, bool selected) {
			if(item.GridItems.Count > 0) {
				Rectangle rect = CalcRectangleImage(item, level);
				PaintOutline(graphics, rect, GetImageCategory(item), selected);
			}
			level += 1;
			if(item.Expanded)
				level = PaintSubCategory(graphics, item, level);
			return level;
		}
		Rectangle CalcRectangleImage(GridItem gridItem, int level) {
			Rectangle outlineRect = OutlineRect(gridItem);
			return InflateRectangle(outlineRect, 1, -ScrollOffset + LevelOffset(level) + 2, 0, 0);
		}
		Rectangle CalcRectangleLabelCell(GridItem gridItem, int level) {
			return CalcRectangleCell(gridItem, level, GetOutlineIconSize() + 5, InternalLabelWidth);
		}
		Rectangle CalcRectangleSelectCell(GridItem gridItem, int level) {
			Rectangle rect = CalcRectangleLabelCell(gridItem, level);
			return InflateRectangle(rect, 1, 1, -1, -1);
		}
		Rectangle CalcRectangleStringCell(GridItem gridItem, int level) {
			Rectangle rect = CalcRectangleLabelCell(gridItem, level);
			return InflateRectangle(rect, OutlineRect(gridItem).X + 1, 2, -1 - OutlineRect(gridItem).X, -1);
		}
		Rectangle CalcRectangleCell(GridItem gridItem, int level, int x, int width) {
			return new Rectangle(x - 1, 1 - ScrollOffset + LevelOffset(level), width - x, RowHeight + 1);
		}
		Rectangle CalcRectangleValueCell(GridItem gridItem, int level) {
			return CalcRectangleCell(gridItem, level, InternalLabelWidth, this.Size.Width);
		}
		Rectangle InflateRectangle(Rectangle rect, int offsetX, int offsetY, int offsetWidth, int offsetHeigth) {
			return new Rectangle(rect.X + offsetX, rect.Y + offsetY, rect.Width + offsetWidth, rect.Height + offsetHeigth);
		}
		bool CanDraw(Rectangle rect) {
			Size ourSize = this.GetOurSize();
			Point ptOurLocation = PtOurLocation;
			Rectangle clipRectangle = new Rectangle(ptOurLocation, ourSize);
			return clipRectangle.Contains(rect.Location);
		}
		int PaintSubCategory(Graphics graphics, GridItem gridItem, int level) {
			GridItemCollection items = Item(gridItem);
			foreach(GridItem item in items) {
				DrawGridItem(graphics, item, level);
				level = DrawExpandedItem(graphics, item, level, item == Parent.SelectedGridItem && OutlineRect(item).X >= GetOutlineIconSize());
			}
			return level;
		}
		public virtual void PaintOutline(Graphics graphics, Rectangle r, Image image, bool selected) {
			if(CanDraw(r)) {
				SolidBrush brush = new SolidBrush(selected ? Parent.GridSelectColor : Parent.ViewBackColor);
				graphics.FillRectangle(brush, r);
				using(Image coloredImage = DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, Parent.ForeColor))
					graphics.DrawImage(coloredImage, r);
			}
		}
		int ScrollOffset { get { return (RowHeight + 1) * rootScrollBar.Value; } }
		int LevelOffset(int level) { return (RowHeight + 1) * level; }
		Image GetImageCategory(GridItem item) {
			return item.Expanded ? ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.Check.png", typeof(PropertyGridViewPainter).Assembly) : ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Designer.Utils.Resource.Uncheck.png", typeof(PropertyGridViewPainter).Assembly);
		}
		#region PropertyGridView
		IEnumerable Entry {
			get {
				return ReflectorHelper.FieldInfo(control.GetType(), "topLevelGridEntries",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(control) as IEnumerable;
			}
		}
		Rectangle OutlineRect(GridItem item) { return (Rectangle)item.GetType().GetProperty("OutlineRect").GetValue(item, null); }
		GridItemCollection Item(object gridItem) {
			return gridItem.GetType().GetProperty("GridItems").GetValue(gridItem, null) as GridItemCollection;
		}
		Size Size {
			get { return (Size)control.GetType().GetProperty("Size").GetValue(control, null); }
		}
		Size ClientSize {
			get { return (Size)control.GetType().GetProperty("ClientSize").GetValue(control, null); }
		}
		protected int RowHeight {
			get {
				return (int)ReflectorHelper.FieldInfo(control.GetType(), "cachedRowHeight",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(control);
			}
		}
		int TotalProps {
			get {
				return (int)ReflectorHelper.FieldInfo(control.GetType(), "totalProps",
				  BindingFlags.NonPublic | BindingFlags.Instance).GetValue(control);
			}
		}
		Point PtOurLocation {
			get {
				return (Point)ReflectorHelper.FieldInfo(control.GetType(), "ptOurLocation",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(control);
			}
		}
		ScrollBar rootScrollBar;
		ScrollBar GetScrollBar() {
			return (ScrollBar)ReflectorHelper.PropertyInfo(control.GetType(), "ScrollBar",
				BindingFlags.NonPublic | BindingFlags.Instance).GetValue(control, null);
		}
		int InternalLabelWidth {
			get {
				return (int)ReflectorHelper.FieldInfo(control.GetType(), "labelWidth",
					BindingFlags.NonPublic | BindingFlags.Instance).GetValue(control);
			}
		}
		Size GetOurSize() {
			MethodInfo method = ReflectorHelper.MethodInfo(control.GetType(), "GetOurSize", BindingFlags.NonPublic | BindingFlags.Instance);
			return (Size)method.Invoke(control, null);
		}
		protected virtual bool GetScrollbarHidden() {
			return ((this.rootScrollBar == null) || !this.rootScrollBar.Visible);
		}
		public virtual int GetOutlineIconSize() {
			MethodInfo method = ReflectorHelper.MethodInfo(control.GetType(), "GetOutlineIconSize", BindingFlags.Public | BindingFlags.Instance);
			return (int)method.Invoke(control, null);
		}
		void OnScroll() {
			MethodInfo method = ReflectorHelper.MethodInfo(control.GetType(), "OnScroll", BindingFlags.NonPublic | BindingFlags.Instance);
			method.Invoke(control, new object[] { rootScrollBar, new ScrollEventArgs(ScrollEventType.ThumbPosition, scrollBarCore.Value) });
		}
		#endregion
	}
	public class BasePainter : IPaintInterface {
		DXPropertyGridEx propertyGrid;
		public BasePainter(DXPropertyGridEx control) {
			this.propertyGrid = control;
		}
		public void DrawStyle() {
			propertyGrid.BackColor = BackColor;
			propertyGrid.CategoryForeColor = propertyGrid.ViewForeColor = propertyGrid.Doccomment.ForeColor = propertyGrid.ForeColor = ForeColor;
			propertyGrid.Doccomment.BackColor = propertyGrid.LineColor = propertyGrid.ViewBackColor = ViewColor;
			propertyGrid.GridLineColor = LineColor;
			propertyGrid.GridSelectColor = SelectColor;
			propertyGrid.GridSelectForeColor = SelectForeColor;
		}
		#region IPaintInterface Members
		public virtual Color BackColor {
			get { return SystemColors.Control; }
		}
		public virtual Color ForeColor {
			get { return SystemColors.WindowText; }
		}
		public virtual Color LineColor {
			get { return SystemColors.ControlDark; }
		}
		public virtual Color ViewColor {
			get { return SystemColors.Window; }
		}
		public virtual Color SelectColor {
			get { return SystemColors.Highlight; }
		}
		public virtual Color SelectForeColor {
			get { return SystemColors.HighlightText; }
		}
		#endregion
	}
	public class SkinPainter : BasePainter {
		ISkinProvider provider;
		Skin skin;
		public SkinPainter(DXPropertyGridEx control, ISkinProvider lookAndFeel)
			: base(control) {
			this.provider = lookAndFeel;
			this.skin = VGridSkins.GetSkin(provider);
		}
		SkinElement GetCategoryColor() {
			return skin[VGridSkins.SkinCategory];
		}
		SkinElement GetRowColor() {
			return skin[VGridSkins.SkinRow];
		}
		SkinElement GetLineColor() {
			return skin[VGridSkins.SkinGridLine];
		}
		SkinElement GetSelectColor() {
			return skin[VGridSkins.SkinBandBorder];
		}
		#region IPaintInterface Members
		public override Color BackColor {
			get { return GetCategoryColor().Color.GetBackColor(); }
		}
		public override Color ForeColor {
			get { return LookAndFeelHelper.GetSystemColorEx(provider, SystemColors.WindowText); }
		}
		public override Color LineColor {
			get { return GetLineColor().Color.GetBackColor(); }
		}
		public override Color ViewColor {
			get { return CommonSkins.GetSkin(provider).TranslateColor(SystemColors.Window); }
		}
		public override Color SelectColor {
			get { return CommonSkins.GetSkin(provider).TranslateColor(SystemColors.Highlight); }
		}
		public override Color SelectForeColor {
			get { return CommonSkins.GetSkin(provider).TranslateColor(SystemColors.HighlightText); }
		}
		#endregion
	}
	public class Office2003Painter : BasePainter {
		public Office2003Painter(DXPropertyGridEx control) : base(control) { }
		AppearanceDefault Get(Office2003GridAppearance app) {
			return Office2003Colors.Default[app].Clone() as AppearanceDefault;
		}
		#region IPaintInterface Members
		public override Color BackColor {
			get { return Get(Office2003GridAppearance.GroupRow).BackColor; }
		}
		public override Color ForeColor {
			get { return SystemColors.WindowText; }
		}
		public override Color LineColor {
			get { return Get(Office2003GridAppearance.GridLine).BackColor; }
		}
		public override Color ViewColor {
			get { return SystemColors.Window; }
		}
		public override Color SelectColor {
			get { return SystemColors.Highlight; }
		}
		public override Color SelectForeColor {
			get { return SystemColors.HighlightText; }
		}
		#endregion
	}
}
