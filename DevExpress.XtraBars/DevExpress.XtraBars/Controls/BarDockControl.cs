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
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Utils;
using System.Windows.Forms.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.Data;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Helpers.Docking {
	public class DockRowCollection : CollectionBase {
		public DockRow this[int index] { get { return (DockRow)List[index]; } }
		public int Add(DockRow row) { return List.Add(row); }
		public void Insert(int index, DockRow row) { List.Insert(index, row); }
		public int IndexOf(DockRow row) { return List.IndexOf(row); }
		public void Remove(DockRow row) { List.Remove(row); }
		public int GetObjectCount() {
			int res = 0;
			foreach (DockRow row in this) {
				res += row.Count;
			}
			return res;
		}
		public DockRow RowByDockable(IDockableObject dockObject) {
			foreach (DockRow row in this) {
				if (row.Contains(dockObject)) return row;
			}
			return null;
		}
		public DockRowObject FindObject(IDockableObject dockObject) {
			DockRow row = RowByDockable(dockObject);
			if (row == null) return null;
			return row[dockObject];
		}
	}
	public class DockRowObject {
		public static Size ZeroSize = new Size(-10000, -10000);
		IDockableObject dockable;
		Rectangle bounds;
		Size minSize = ZeroSize;
		public DockRowObject() : this(null, Rectangle.Empty) { }
		public DockRowObject(IDockableObject dockable) : this(dockable, Rectangle.Empty) { }
		public DockRowObject(IDockableObject dockable, Rectangle bounds) {
			this.dockable = dockable;
			this.bounds = bounds;
		}
		public void ResetBounds() {
			this.bounds = Rectangle.Empty;
			this.minSize = ZeroSize;
		}
		public IDockableObject Dockable { get { return dockable; } set { dockable = value; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Size MinSize {
			get {
				if (minSize == ZeroSize) minSize = Dockable.CalcMinSize();
				return minSize;
			}
		}
	}
	public class DockRow : CollectionBase {
		BarDockControl dockControl;
		Rectangle rowRect;
		public DockRow(BarDockControl dockControl) {
			this.dockControl = dockControl;
			this.rowRect = Rectangle.Empty;
		}
		public DockRowObject this[int index] { get { return (DockRowObject)List[index]; } }
		public IDockableObject GetDockable(int index) { return this[index].Dockable; }
		public DockRowObject this[IDockableObject dockableObject] {
			get {
				for (int n = 0; n < Count; n++) {
					DockRowObject obj = this[n];
					if (obj.Dockable == dockableObject) return obj;
				}
				return null;
			}
		}
		public int CalcRowMaxSize(bool isVertical) {
			int res = 0;
			for (int n = 0; n < Count; n++) {
				Rectangle bounds = this[n].Bounds;
				if (isVertical)
					res = Math.Max(bounds.Width, res);
				else
					res = Math.Max(bounds.Height, res);
			}
			return res;
		}
		public virtual void UpdateRowSize(bool isVertical, int height) {
			for (int n = 0; n < this.Count; n++) {
				Rectangle r = this[n].Bounds;
				if (isVertical)
					r.Width = height;
				else
					r.Height = height;
				this[n].Bounds = r;
			}
		}
		public void ResetObjectBounds() {
			foreach (DockRowObject obj in this) obj.ResetBounds();
		}
		public Rectangle GetDockableRectangle(IDockableObject dockObject) { return dockObject.Bounds; }
		public void SetDockableRectangle(IDockableObject dockObject, Rectangle newRect) { dockObject.Bounds = newRect; }
		public Rectangle RowRect { get { return rowRect; } set { rowRect = value; } }
		public BarDockControl DockControl { get { return dockControl; } }
		public bool CanAddDockable(IDockableObject dockObject) {
			if (Count == 0) return true;
			if (GetDockable(0).UseWholeRow) return false;
			if (Contains(dockObject)) return false;
			return true;
		}
		public int Add(IDockableObject dockObject) {
			if (Contains(dockObject)) return IndexOf(dockObject);
			int index = List.Add(new DockRowObject(dockObject));
			return index;
		}
		public void Remove(IDockableObject dockObject) {
			int index = IndexOf(dockObject);
			if (index > -1) List.RemoveAt(index);
		}
		public void Insert(int index, IDockableObject dockObject) {
			if (Contains(dockObject)) return;
			List.Insert(index, new DockRowObject(dockObject));
		}
		public bool Contains(IDockableObject dockableObject) { return this[dockableObject] != null; }
		public int IndexOf(IDockableObject dockableObject) { return IndexOf(this[dockableObject]); }
		public int IndexOf(DockRowObject rowObject) { return List.IndexOf(rowObject); }
	}
	[TypeConverter(typeof(ExpandableObjectConverter)),
	ListBindable(false)]
	public class BarDockControls : CollectionBase {
		BarManager manager;
		public BarDockControls(BarManager manager) { this.manager = manager; }
		public BarDockControl this[int index] { get { return (BarDockControl)List[index]; } }
		public BarDockControl this[BarDockStyle dockStyle] {
			get {
				foreach (BarDockControl ctrl in this) {
					if (ctrl.DockStyle == dockStyle) return ctrl;
				}
				return null;
			}
		}
		public int Add(BarDockControl dockControl) {
			if (Contains(dockControl)) return IndexOf(dockControl);
			return List.Add(dockControl);
		}
		public int IndexOf(BarDockControl dockControl) { return List.IndexOf(dockControl); }
		public void Remove(BarDockControl dockControl) {
			if (List.Contains(dockControl)) List.Remove(dockControl);
		}
		public virtual bool Contains(BarDockControl ctrl) { return List.Contains(ctrl); }
		public BarManager Manager { get { return manager; } }
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			StandaloneBarDockControl control = value as StandaloneBarDockControl;
			if(control != null) {
				control.SetManagerCore(Manager);
				if(!Manager.IsLoading && !Manager.IsDesignMode)
					control.Init();
			}
		}
	}
}
namespace DevExpress.XtraBars {
	[TypeConverter("DevExpress.XtraBars.TypeConverters.StandaloneBarDockControlTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign),
	 Designer("DevExpress.XtraBars.Design.StandaloneBarDockControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	 DXToolboxItem(true), DesignTimeVisible(true),
	 Description("Allows bars to be docked at any position of a form."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "StandaloneBarDockControl")
	]
	public class StandaloneBarDockControl : BarDockControl, IXtraResizableControl {
		bool vertical = false;
		bool allowTransparency = false;
		public StandaloneBarDockControl()
			: base() {
			this.SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
		}
		protected override bool IsAutoSizeControl {
			get { return false; }
		}
		protected internal override bool AllowTransparencyCore {
			get {
				return AllowTransparency;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowTransparency {
			get { return allowTransparency; }
			set {
				if(AllowTransparency == value)
					return;
				allowTransparency = value;
				if(IsHandleCreated)
					LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlLockLayout")]
#endif
public override int LockLayout { get { return base.LockLayout; } set { base.LockLayout = value; } }
		public virtual void InitializeDesignTime(BarManager manager, BarDockStyle dockStyle) { InitDockControl(manager, dockStyle); }
		protected internal override void InitDockControl(BarManager manager, BarDockStyle dockStyle) {
			this.dockStyle = dockStyle;
			SetManagerCore(manager);
			Init();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			UpdateControls();
		}
		void UpdateControls() {
			if(DesignMode && Parent != null && Visible && Manager != null) {
				SkipRaiseSizeableChanged = true;
				try {
					Control[] controls = new Control[Controls.Count];
					for(int i = 0; i < Controls.Count; i++) {
						controls[i] = Controls[i];
					}
					Controls.Clear();
					Rows.Clear();
					BeginUpdate();
					for(int i = 0; i < controls.Length; i++) {
						DockedBarControl dbc = controls[i] as DockedBarControl;
						if(dbc != null) {
							dbc.Bar.BarControl = new DockedBarControl(Manager, dbc.Bar);
							AddDockable(dbc.Bar, dbc.Bar.DockInfo.DockRow, dbc.Bar.DockInfo.DockCol, true);
						}
					}
				}
				finally {
					EndUpdate();
					DoLayout(true);
					SkipRaiseSizeableChanged = false;
				}
			}
		}
		protected override bool CalcPerformLayoutOnDockChanged(Rectangle prevBounds) {
			return true;
		}
		protected override void MakeControlVisible() { }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlIsVertical"),
#endif
 EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(false)]
		public override bool IsVertical {
			get { return vertical; }
			set {
				if(IsVertical == value) return;
				vertical = value;
				OnIsVerticalChanged();
			}
		}
		protected virtual void ClearDockableSizeHash() {
			foreach(DockRow row in Rows) {
				for(int i = 0; i < row.Count; i++) {
					CustomControl c = row.GetDockable(i).BarControl;
					if(c != null)
						c.ClearHash();
				}
			}
		}
		protected virtual void OnIsVerticalChanged() {
			ClearDockableSizeHash();
				OnBarDockChanged();
			}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlDockStyle")]
#endif
public override BarDockStyle DockStyle { get { return BarDockStyle.Standalone; } }
		protected override void PaintBase(PaintEventArgs e) {
			base.PaintBase(e);
			using (GraphicsCache cache = new GraphicsCache(e)) {
				ControlPaint.DrawButton(e.Graphics, ClientRectangle, ButtonState.Flat);
				Rectangle r = Rectangle.Inflate(ClientRectangle, -2, -1);
				if (Manager == null && DesignMode) {
					AppearanceObject app = new AppearanceObject(new AppearanceDefault(Color.Red, Color.Transparent, HorzAlignment.Center, VertAlignment.Center));
					app.Font = cache.GetFont(app.Font, FontStyle.Bold);
					app.TextOptions.WordWrap = WordWrap.Wrap;
					app.DrawString(cache, "StandaloneBarDockControl.Manager property is not set. Add a BarManager onto the form.", r);
				}
			}
		}
		protected override void UpdateSize(Size newSize) {
			if (!GetAutoSizeState() || skipUpdateControlSize) return;
			if (newSize == Size.Empty) newSize = Size;
			if(IsHorizontalDocked)
				newSize.Width = Size.Width;
			if(IsVerticalDocked)
				newSize.Height = Size.Height;
			base.UpdateSize(newSize);
		}
		protected internal bool GetAutoSizeState() {
			return AutoSize || (AutoSizeInLayoutControl && IsInLayoutControl);
		}
		protected override void OnAutoSizeChanged(EventArgs e) {
			base.OnAutoSizeChanged(e);
			if (Manager == null) return;
			if (AutoSize) UpdateSize(CalcBestSize());
		}
		bool forceMaxClientSize = false;
		bool forceBoundsCoreSize = false;
		protected internal override int GetMaxClientWidth() {
			if(forceMaxClientSize) return 10000;
			if(forceBoundsCoreSize) {
				return IsVertical ? boundsCoreHeight : boundsCoreWidth;
			}
			if (GetAutoSizeState() && Dock == System.Windows.Forms.DockStyle.None) return 10000;
			if (IsVertical) return ClientRectangle.Height;
			return ClientRectangle.Width;
		}
		protected override Size GetEmptyDockSize() {
			return ClientRectangle.Size;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && Manager != null) {
				if (Manager.IsDesignMode)
					Manager.DockControls.Remove(this);
			}
			base.Dispose(disposing);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlHasBars")]
#endif
public virtual bool HasBars {
			get {
				if (Manager == null) return false;
				foreach (Bar bar in Manager.Bars) {
					if (bar.DockStyle == BarDockStyle.Standalone && bar.DockInfo.DockControl == this) return true;
				}
				return false;
			}
		}
		protected virtual int GetMaxWidth() {
			int maxWidth = IsVertical ? Size.Height : Size.Width;
			for (int n = 0; n < Rows.Count; n++) {
				for (int r = 0; r < Rows[n].Count; r++) {
					if (!IsVertical && Rows[n][r].Bounds.Width < 5000) maxWidth = Math.Max(maxWidth, Rows[n][r].Bounds.Width);
					else if (Rows[n][r].Bounds.Height < 5000) maxWidth = Math.Max(maxWidth, Rows[n][r].Bounds.Height);
				}
			}
			return maxWidth;
		}
		protected override void UpdateWholeRowBarWidth(int startN, int endN, int incN) {
			if (!GetAutoSizeState()) return;
			int maxWidth = GetMaxWidth();
			for (int n = startN; (incN == 1 ? n < endN : n >= endN); n += incN) {
				for (int r = 0; r < Rows[n].Count; r++) {
					if (Rows[n][r].Dockable.UseWholeRow) {
						if (!IsVertical) Rows[n][r].Bounds = new Rectangle(Rows[n][r].Bounds.X, Rows[n][r].Bounds.Y, maxWidth, Rows[n][r].Bounds.Height);
						else Rows[n][r].Bounds = new Rectangle(Rows[n][r].Bounds.X, Rows[n][r].Bounds.Y, Rows[n][r].Bounds.Width, maxWidth);
					}
				}
			}
		}
		protected internal override Rectangle CalcDockingRectangle() {
			if (!Visible) return Rectangle.Empty;
			if (Parent == null) return Bounds;
			Rectangle dockingRect = Parent.RectangleToScreen(Bounds);
			if (IsVertical) dockingRect.Inflate(7, 0);
			else dockingRect.Inflate(0, 7);
			return dockingRect;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlName"),
#endif
Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new string Name { get { return base.Name; } set { base.Name = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlLocation"),
#endif
Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Point Location { get { return base.Location; } set { base.Location = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlSize"),
#endif
Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Size Size { get { return base.Size; } set { base.Size = value; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlAutoSize"),
#endif
Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always)]
		public new bool AutoSize { get { return base.AutoSize; } set { base.AutoSize = value; } }
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always)]
		public new System.Windows.Forms.DockStyle Dock { get { return base.Dock; } set { base.Dock = value; } }
		protected override Size GetControlSize() {
			if(GetAutoSizeState())
				return CalcBestSize();
			return Size;
		}
		protected internal virtual Size CalcBestSize() {
			int maxValue = 0;
			int sizeVal = 0;
			if(Rows.Count == 0) {
				if(IsHorizontalDocked)
					return new Size(Size.Width, 0);
				else if(IsVerticalDocked)
					return new Size(0, Size.Height);
				return Size;
			}
			foreach (DockRow row in Rows) {
				if (!IsVertical) {
					sizeVal += row.RowRect.Height + ConstBarDockedRowIndent;
					if (row.Count != 0) maxValue = Math.Max(row[row.Count - 1].Bounds.Right, maxValue);
				} else {
					sizeVal += row.RowRect.Width + ConstBarDockedRowIndent;
					if(row.Count != 0) maxValue = Math.Max(row[row.Count - 1].Bounds.Bottom, maxValue);
				}
			}
			sizeVal -= ConstBarDockedRowIndent;
			if (!IsVertical) return new Size(maxValue, sizeVal);
			return new Size(sizeVal, maxValue);
		}
		bool isInLayoutInitialized = false;
		bool isInLayout = false;
		protected virtual bool IsInLayoutControl {
			get {
				if (!isInLayoutInitialized) isInLayout = Parent != null && Parent.GetType().ToString().EndsWith("LayoutControl");
				return isInLayout;
			}
		}
		protected void RefreshDockControls() {
			if (Manager == null) return;
			Point loc;
			Size sz;
			foreach (DockRow row in Rows) {
				foreach (DockRowObject obj in row) {
					Bar bar = obj.Dockable as Bar;
					if (bar == null || bar.BarControl == null) continue;
					loc = bar.BarControl.Location;
					sz = bar.BarControl.Size;
					Controls.Remove(bar.BarControl);
					bar.BarControl.Dispose();
					bar.BarControl = new DockedBarControl(Manager, bar);
					bar.BarControl.Location = loc;
					bar.BarControl.Size = sz;
					bar.BarControl.Visible = true;
					Controls.Add(bar.BarControl);
				}
			}
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			RefreshDockControls();
			isInLayoutInitialized = false;
		}
		bool skipUpdateControlSize = false;
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if (!DesignMode && Manager == null) return;
			skipUpdateControlSize = Dock == System.Windows.Forms.DockStyle.None;
			UpdateDockSize();
			skipUpdateControlSize = false;
		}
		int boundsCoreWidth, boundsCoreHeight;
		protected virtual void ForceDoDockLayout(int width, int height) {
			boundsCoreWidth = width;
			boundsCoreHeight = height;
			forceBoundsCoreSize = true;
			try {
				DoLayout(false);
			}
			finally {
				forceBoundsCoreSize = false;
			}
		}
		protected virtual void ForceDoAutoSizeLayout() {
			forceMaxClientSize = true;
			try {
				DoLayout(false);
			}
			finally {
				forceMaxClientSize = false;
			}
		}
		protected bool IsSideDocked {
			get {
				if((Dock == System.Windows.Forms.DockStyle.Top || Dock == System.Windows.Forms.DockStyle.Bottom) && !IsVertical)
					return true;
				if((Dock == System.Windows.Forms.DockStyle.Left || Dock == System.Windows.Forms.DockStyle.Right) && IsVertical)
					return true;
				return false;
			}
		}
		protected bool IsHorizontalDocked {
			get {
				return (Dock == System.Windows.Forms.DockStyle.Top || Dock == System.Windows.Forms.DockStyle.Bottom) && !IsVertical;
			}
		}
		protected bool IsVerticalDocked {
			get {
				return (Dock == System.Windows.Forms.DockStyle.Left || Dock == System.Windows.Forms.DockStyle.Right) && IsVertical;
			}
		}
		protected void ForceDoLayout(int width, int height) {
			if(width == 0 && IsHorizontalDocked && Parent != null) {
				width = Parent.Width;
			}
			if(height == 0 && IsVerticalDocked && Parent != null) {
				height = Parent.Height;
			}
			if(IsSideDocked)
				ForceDoDockLayout(width, height);
			else
				ForceDoAutoSizeLayout();
		}
		public override Size GetPreferredSize(Size proposedSize) {
			if((IsInLayoutControl || GetAutoSizeState())) {
				if(Manager == null)
					return Size;
				ForceDoLayout(proposedSize.Width, proposedSize.Height);
				Size sz = CalcBestSize();
				DoLayout(false);
				if(proposedSize.Width == int.MaxValue && proposedSize.Height == int.MaxValue)
					return sz;
				if(!sz.IsEmpty && Dock != System.Windows.Forms.DockStyle.Fill) {
					proposedSize.Height = IsHorizontalDocked || Rows.Count == 0? sz.Height : proposedSize.Height;
					proposedSize.Width = IsVerticalDocked || Rows.Count == 0? sz.Width : proposedSize.Width;
				}
			}
			return proposedSize;
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			int originalW = width, originalH = height;
			if ((IsInLayoutControl || GetAutoSizeState()) && Manager != null) {
				ForceDoLayout(width, height);
				Size sz = CalcBestSize();
				DoLayout(false);
				skipUpdateControlSize = false;
				if (!sz.IsEmpty && Dock != System.Windows.Forms.DockStyle.Fill) {
					height = IsHorizontalDocked && Rows.Count > 0 ? sz.Height : height;
					width = IsVerticalDocked && Rows.Count > 0 ? sz.Width : width;
				}
				if (IsInLayoutControl) {
					bool hasNonFitBar = HasNonFitBar();
					if(!AutoSizeInLayoutControl || !hasNonFitBar) {
						width = originalW;
						if(!AutoSizeInLayoutControl)
							height = originalH;
						if (isLayoutChangingBounds) {
							base.SetBoundsCore(x, y, width, height, specified);
							return;
						}
						else {
							isLayoutChangingBounds = true;
							RaiseSizeableChanged();
							isLayoutChangingBounds = false;
							return;
						}
					}
				}
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		bool isLayoutChangingBounds = false;
		#region IXtraResizableControl
		bool autoSizeInLayoutControl = true;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("StandaloneBarDockControlAutoSizeInLayoutControl"),
#endif
RefreshProperties(RefreshProperties.All), DefaultValue(true)]
		public virtual bool AutoSizeInLayoutControl {
			get { return autoSizeInLayoutControl; }
			set {
				autoSizeInLayoutControl = value;
				if(AutoSizeInLayoutControl) UpdateSize(CalcBestSize());
				RaiseSizeableChanged();
			}
		}
		bool HasNonFitBar() {
			foreach(DockRow row in Rows) {
				foreach(DockRowObject obj in row) {
					if(!obj.Dockable.UseWholeRow)
						return true;
				}
			}
			return false;
		}
		Size GetSizeInLayoutControl(int minWidth) {
			Size res = Size.Empty;
			if(AutoSizeInLayoutControl) {
				Size bestSize = CalcBestSize();
				res.Width = HasNonFitBar() ? bestSize.Width : minWidth;
				res.Height = bestSize.Height;
			}
			return res;
		}
		bool IXtraResizableControl.IsCaptionVisible { get { return false; } }
		Size IXtraResizableControl.MinSize {
			get {
				return GetSizeInLayoutControl(1);
			}
		}
		Size IXtraResizableControl.MaxSize {
			get {
				return GetSizeInLayoutControl(0);
			}
		}
		private static readonly object sizeableChanged = new object();
		event EventHandler IXtraResizableControl.Changed {
			add { Events.AddHandler(sizeableChanged, value); }
			remove { Events.RemoveHandler(sizeableChanged, value); }
		}
		protected override void DoLayout() {
			if(Manager == null) return;
			base.DoLayout();
			if(CanRaiseSizeableChanged()) RaiseSizeableChanged();
		}
		protected bool CanRaiseSizeableChanged() {
			return !(DesignMode && isCheckDirty);
		}
		bool SkipRaiseSizeableChanged { get; set; }
		protected virtual void RaiseSizeableChanged() {
			if (SkipRaiseSizeableChanged || Manager == null || Manager.IsLoading || Manager.lockFireChanged != 0 || (Manager.IsDesignMode && !Manager.Helper.LoadHelper.Loaded)) return;
			EventHandler changed = (EventHandler)Events[sizeableChanged];
			if (changed == null) return;
			changed(this, EventArgs.Empty);
		}
		#endregion
	}
	[TypeConverter("DevExpress.XtraBars.TypeConverters.BarDockControlTypeConverter, " + AssemblyInfo.SRAssemblyBarsDesign),
	Designer("DevExpress.XtraBars.Design.BarDockControlDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxItem(false), DesignTimeVisible(false)]
	public class BarDockControl : CustomControl {
		DockRowCollection rows;
		internal BarDockStyle dockStyle;
		bool allowDispose = false;
		int lockSize = 0;
		int lockUpdate;
		AppearanceObject appearance;
		public BarDockControl()
			: base(null) {
			this.appearance = new AppearanceObject();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.lockUpdate = 0;
			this.TabStop = false;
			this.rows = new DockRowCollection();
			this.dockStyle = BarDockStyle.None;
			this.SetStyle(ControlStyles.Selectable, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor, false);
			if(IsAutoSizeControl)
				this.AutoSize = true;
			CausesValidation = false;
		}
		protected virtual bool IsAutoSizeControl { get { return true; } }
		protected internal virtual bool AllowTransparencyCore { get { return false; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowDrop {
			get { return base.AllowDrop; }
			set { base.AllowDrop = value; }
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraBars.Accessible.DockControlAccessible(this);
		}
#if DXWhidbey
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarDockControlSite")]
#endif
		public override ISite Site {
			get {
				if(Manager != null && Manager.Helper != null) {
					if(Manager.Helper.DockingHelper != null && Manager.Helper.DockingHelper.InUpdateDocking) return null;
				}
				return base.Site;
			}
			set { base.Site = value; }
		}
#endif
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarDockControlViewInfo")]
#endif
		public virtual new DockControlViewInfo ViewInfo { get { return base.ViewInfo as DockControlViewInfo; } }
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void CancelUpdate() {
			if (lockUpdate == 0) return;
			--lockUpdate;
		}
		public virtual void EndUpdate() {
			if (lockUpdate == 0) return;
			if (--lockUpdate == 0) {
				OnBarDockChanged();
			}
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			base.SetBoundsCore(x, y, width, height, specified);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockControlBackColor"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get { return ViewInfo != null ? ViewInfo.Appearance.Normal.BackColor : base.BackColor; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ee);
			if (ee.Handled) return;
			if (e.Clicks == 1 && e.Button == MouseButtons.Left) {
				if (Manager != null && Manager.IsDesignMode)
					Manager.Helper.CustomizationManager.SelectObject(this);
			}
		}
		internal override void SetManagerCore(BarManager manager) {
			base.SetManagerCore(manager);
			if (manager != null && manager.IsDesignMode) {
				AllowDrop = true;
			}
		}
		bool initializing = false;
		protected virtual bool AllowOverrideSite { get { return !initializing; } }
		protected override void OnControlAdded(ControlEventArgs e) { }
		protected override void OnControlRemoved(ControlEventArgs e) { }
		protected internal DockRowCollection Rows { get { return rows; } }
		protected internal virtual void InitDockControl(BarManager manager, BarDockStyle dockStyle) {
			if (this.dockStyle == dockStyle) return; 
			System.Windows.Forms.DockStyle[] styles =
				new System.Windows.Forms.DockStyle[] {
														 System.Windows.Forms.DockStyle.Left, 
														 System.Windows.Forms.DockStyle.Top, 
														 System.Windows.Forms.DockStyle.Right, 
														 System.Windows.Forms.DockStyle.Bottom,
														 System.Windows.Forms.DockStyle.None};
			this.initializing = true;
			try {
			this.dockStyle = dockStyle;
			SetManagerCore(manager);
			Init();
			if (dockStyle != BarDockStyle.None) {
					if(Dock != styles[Convert.ToInt16(dockStyle) - 1])
				this.Dock = styles[Convert.ToInt16(dockStyle) - 1];
					if(!Visible)
				this.Visible = true;
			}
		}
			finally {
				this.initializing = false;
			}
		}
		void ResetAppearance() { Appearance.Reset(); Appearance.Options.Reset(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockControlAppearance"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance { get { return appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Name { get { return base.Name; } set { base.Name = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Point Location { get { return base.Location; } set { base.Location = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new Size Size { get { return base.Size; } set { base.Size = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new DockStyle Dock { get { return base.Dock; } set { base.Dock = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int TabIndex { get { return base.TabIndex; } set { base.TabIndex = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int LockLayout { get { return base.LockLayout; } set { base.LockLayout = value; } }
		bool ShouldSerializeControls() { return false; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Control.ControlCollection Controls { get { return base.Controls; } }
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			if (Manager != null && Manager.AllowCustomization)
				Manager.Customize();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = new DXMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			base.OnMouseUp(ee);
			if(ee.Handled)
				return;
			if (e.Button == MouseButtons.Right && Manager != null) {
				Manager.ShowToolBarsPopup();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockControlDockStyle"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual BarDockStyle DockStyle { get { return dockStyle; } }
		protected internal void AddDockable(IDockableObject dockObject, int rowIndex) {
			AddDockable(dockObject, rowIndex, -1);
		}
		protected internal void AddDockable(IDockableObject dockObject, int rowIndex, int colIndex) {
			AddDockable(dockObject, rowIndex, colIndex, true);
		}
		protected virtual void OnAddDockable(IDockableObject dockObject) {
			dockObject.BarControl.Visible = false;
			dockObject.BarControl.Parent = this;
			Controls.Add(dockObject.BarControl);
		}
		protected internal void AddDockable(IDockableObject dockObject, int rowIndex, int colIndex, bool updateIndexes) {
			if (Rows.RowByDockable(dockObject) != null) return;
			OnAddDockable(dockObject);
			if (rowIndex < 0 || rowIndex > Rows.Count) rowIndex = Rows.Count;
			if (dockObject.UseWholeRow) colIndex = -1;
			while (rowIndex < Rows.Count && colIndex != -1) {
				if (!Rows[rowIndex].CanAddDockable(dockObject)) {
					rowIndex++;
					continue;
				}
				break;
			}
			CalcMinMaxDockableRow(dockObject, ref rowIndex, ref colIndex);
			if (rowIndex >= Rows.Count || colIndex == -1) {
				DockRow row = new DockRow(this);
				colIndex = row.Add(dockObject);
				Rows.Insert(rowIndex, row);
				rowIndex = Rows.IndexOf(row);
			} else {
				DockRow row = Rows[rowIndex];
				if (colIndex < 0 || colIndex >= row.Count) {
					colIndex = row.Count;
				}
				row.Insert(colIndex, dockObject);
			}
			if (updateIndexes)
				UpdateDockIndexes(dockObject);
			dockObject.DockInfo.UpdateDockPosition(rowIndex, colIndex);
			if (IsEmpty(dockObject)) return;
			OnBarDockChanged();
		}
		bool IsEmpty(DockRow row) {
			if (row.Count == 0) return true;
			if (row.Count > 1) return false;
			for (int n = 0; n < row.Count; n++) {
				if (!IsEmpty(row.GetDockable(n))) return false;
			}
			return true;
		}
		protected virtual bool IsEmpty(IDockableObject dockable) {
			if (dockable == null || dockable.BarControl == null) return true;
			if (dockable.CalcSize(1000).IsEmpty) return true;
			return false;
		}
		internal void UpdateDockIndexes(IDockableObject ignoreObject) {
			for (int r = 0; r < Rows.Count; r++) {
				DockRow row = Rows[r] as DockRow;
				for (int c = 0; c < row.Count; c++) {
					IDockableObject dockObject = row.GetDockable(c);
					if (ignoreObject == dockObject) continue;
					dockObject.DockInfo.UpdateDockPosition(r, c);
				}
			}
		}
		protected internal int GetDockableDockRow(IDockableObject dockObject) {
			DockRow row = Rows.RowByDockable(dockObject);
			if (row == null) return -1;
			return Rows.IndexOf(row);
		}
		protected internal int GetDockableOnRowCount(IDockableObject dockObject) {
			DockRow row = Rows.RowByDockable(dockObject);
			if (row == null) return 0;
			return row.Count;
		}
		protected internal int GetDockableDockCol(IDockableObject dockObject) {
			DockRow row = Rows.RowByDockable(dockObject);
			if (row == null) return -1;
			return row.IndexOf(dockObject);
		}
		protected bool IsDestroying { get { return Manager == null || Manager.IsDestroying; } }
		protected internal void RemoveDockable(IDockableObject dockObject) {
			if (IsDestroying) return;
			Form form = FindForm();
			RemoveDockable(dockObject, Manager != null ? Manager.IsDocking : true);
			DevExpress.XtraEditors.Container.ContainerHelper.ClearUnvalidatedControl(dockObject.BarControl, form);
			DevExpress.XtraEditors.Container.ContainerHelper.ClearUnvalidatedControl(dockObject.BarControl, form);
		}
		protected virtual void OnRemoveDockable(IDockableObject dockObject) { }
		protected internal void RemoveDockable(IDockableObject dockObject, bool updateIndexes) {
			if (IsDestroying) return;
			DockRow row = Rows.RowByDockable(dockObject);
			OnRemoveDockable(dockObject);
			if(row != null) {
				row.Remove(dockObject);
				if(row.Count == 0) {
					if(updateIndexes) {
						dockObject.DockInfo.UpdateDockPosition(Rows.IndexOf(row), dockObject.DockInfo.DockCol); 
					}
					Rows.Remove(row);
				}
			}
			if (updateIndexes) UpdateDockIndexes(null);
			if(CanUpdateFocusControl(dockObject))
				Select();
			Controls.Remove(dockObject.BarControl);
			OnBarDockChanged();
		}
		bool CanUpdateFocusControl(IDockableObject dockObject) {
			return Manager != null && !Manager.Helper.DockingHelper.InUpdateDocking && dockObject != null && dockObject.BarControl != null && dockObject.IsVisible;
		}
		protected void LockSize() { this.lockSize++; }
		protected void UnlockSize() { this.lockSize--; }
		protected bool IsSizeUpdating { get { return lockSize != 0; } }
		protected virtual void OnAppearanceChanged(object source, EventArgs e) {
			if (Manager == null || Manager.IsLoading) return;
			LayoutChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowDispose { get { return allowDispose; } set { allowDispose = value; } }
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (Manager != null) Manager.DockControls.Remove(this);
				SetManagerCore(null);
				this.appearance.Changed -= new EventHandler(OnAppearanceChanged);
			}
			base.Dispose(disposing);
		}
		protected override void OnResize(EventArgs e) {
			UpdateViewInfo();
			Invalidate();
		}
		protected virtual int CalcMinDockableRow() {
			if (DockStyle != BarDockStyle.Bottom) return 0;
			for (int n = Rows.Count - 1; n >= 0; n--) {
				DockRow row = Rows[n] as DockRow;
				for (int obj = 0; obj < row.Count; obj++) {
					Bar stBar = row.GetDockable(obj) as Bar;
					if (stBar != null && stBar.IsStatusBar) return n + 1;
				}
			}
			return 0;
		}
		protected internal void CalcMinMaxDockableRow(IDockableObject dockObject, ref int rowIndex, ref int colIndex) {
			int minRow = CalcMinDockableRow(), maxRow = Rows.Count + 1;
			if (rowIndex != -1) {
				if (rowIndex < minRow) {
					rowIndex = minRow;
					colIndex = -1;
				}
				if (rowIndex > maxRow) {
					rowIndex = maxRow;
					colIndex = -1;
				}
			}
		}
		protected Size GetParentClientSize() {
			if(Parent == null)
				return Size.Empty;
			return Parent.DisplayRectangle.Size; 
		}
		protected virtual int ConstBarDockedRowBarIndent { get { return Manager.DrawParameters.Constants.BarDockedRowBarIndent; } }
		protected virtual int ConstBarDockedRowIndent { get { return Manager.DrawParameters.Constants.BarDockedRowIndent; } }
		protected virtual int ConstTopDockIndent { get { return Manager.DrawParameters.Constants.TopDockIndent; } }
		protected internal virtual int GetMaxClientWidth() {
			int res = 0;
			if (!IsVertical)
				res = GetParentClientSize().Width;
			else {
				res = GetParentClientSize().Height;
				if (Manager.DockControls[BarDockStyle.Top] != null)
					res -= Manager.DockControls[BarDockStyle.Top].ClientSize.Height;
				if (Manager.DockControls[BarDockStyle.Bottom] != null)
					res -= Manager.DockControls[BarDockStyle.Bottom].ClientSize.Height;
			}
			return res;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarDockControlIsVertical"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override bool IsVertical { get { return DockStyle == BarDockStyle.Right || DockStyle == BarDockStyle.Left; } set { } }
		protected virtual int CalcMaxRestWidth(DockRow row, int barIndex, int restWidth) {
			int res = restWidth;
			for (int n = barIndex + 1; n < row.Count; n++) {
				Size size = row[n].MinSize;
				res -= (GetWidth(size) + GetObjIndent(row, n - 1));
			}
			if (res < 0) res = 0;
			return res;
		}
		protected virtual ArrayList CalcBarMaxOffsets(DockRow row, int restWidth) {
			ArrayList list = new ArrayList();
			int rw = restWidth, n;
			int startOffset = 0;
			for (n = 0; n < row.Count; n++) {
				list.Add(startOffset);
				int maxBarWidth = CalcMaxRestWidth(row, n, rw);
				Size size = row.GetDockable(n).CalcSize(maxBarWidth);
				int ds = GetWidth(size) + GetObjIndent(row, n);
				rw -= ds;
				startOffset += ds;
			}
			if (rw < 0) rw = 0;
			for (n = 0; n < row.Count; n++) {
				list[n] = (int)list[n] + rw;
			}
			return list;
		}
		int CalcBarValidOffset(DockRow row, IDockableObject dockObject, ArrayList maxOffsets) {
			int maxOffset = (int)maxOffsets[row.IndexOf(dockObject)];
			int offset = dockObject.DockInfo.Offset;
			if (offset > maxOffset) return maxOffset;
			return offset;
		}
		bool AllowLastIndent() {
			return Manager != null && (DevExpress.Utils.Mdi.MdiClientSubclasser.GetMdiClient(Manager.Form as Form) != null);
		}
		int GetFar(Point point) { return IsVertical ? point.X : point.Y; }
		int GetNear(Point point) { return IsVertical ? point.Y : point.X; }
		int GetWidth(Size size) { return IsVertical ? size.Height : size.Width; }
		int GetWidth(Rectangle rect) { return GetWidth(rect.Size); }
		Size SetHeight(Size size, int height) { return IsVertical ? new Size(height, size.Height) : new Size(size.Width, height); }
		int GetHeight(Size size) { return IsVertical ? size.Width : size.Height; }
		int GetHeight(Rectangle rect) { return GetHeight(rect.Size); }
		Size SetSize(Size size, int width) { return IsVertical ? new Size(size.Width, width) : new Size(width, size.Height); }
		Point SetNear(Point point, int pos) { return IsVertical ? new Point(point.X, pos) : new Point(pos, point.Y); }
		Point SetFar(Point point, int pos) { return IsVertical ? new Point(pos, point.Y) : new Point(point.X, pos); }
		protected internal virtual bool IsRightToLeft { get { return !IsVertical && Manager != null && Manager.IsRightToLeft; } }
		protected virtual void MakeLayout(DockRow row, ref Point lastLocation, int rowIndent) {
			int maxWidth = GetMaxClientWidth(), indent;
			int restWidth = maxWidth;
			row.ResetObjectBounds();
			Rectangle barRect = Rectangle.Empty;
			row.RowRect = new Rectangle(lastLocation, Size.Empty);
			int deltaDirection = IsRightToLeft ? -1 : 1;
			ArrayList maxOffsets = CalcBarMaxOffsets(row, restWidth);
			bool ignoreOffset = false;
			for (int b = 0; b < row.Count; b++) {
				DockRowObject rowObj = row[b];
				bool isLast = b == row.Count - 1;
				int maxBarWidth = rowObj.Dockable.UseWholeRow ? restWidth : CalcMaxRestWidth(row, b, restWidth);
				int barOffset = CalcBarValidOffset(row, rowObj.Dockable, maxOffsets);
				if (ignoreOffset) barOffset = GetNear(lastLocation);
				else {
					if(IsRightToLeft) {
						barOffset = maxWidth - barOffset;
					}
				}
				lastLocation = SetNear(lastLocation, IsRightToLeft ?  Math.Min(barOffset, GetNear(lastLocation)) : Math.Max(barOffset, GetNear(lastLocation)));
				barRect.Location = lastLocation;
				barRect.Size = rowObj.Dockable.CalcSize(maxBarWidth);
				int calculatedWidth = GetWidth(barRect);
				barRect.Size = SetSize(barRect.Size, Math.Min(GetWidth(barRect), maxBarWidth));
				if (rowObj.Dockable.UseWholeRow) barRect.Size = SetSize(barRect.Size, restWidth);
				if(IsRightToLeft) {
					barRect.Location = SetNear(lastLocation, GetNear(lastLocation) - GetWidth(barRect));
				}
				indent = isLast && !AllowLastIndent() ? 0 : ConstBarDockedRowBarIndent;
				restWidth -= (GetWidth(barRect) + indent);
				lastLocation = SetNear(lastLocation, GetNear(lastLocation) + (GetWidth(barRect) + indent) * deltaDirection);
				rowObj.Bounds = barRect;
				ignoreOffset = GetWidth(barRect) < calculatedWidth;
			}
			int rowHeight = row.CalcRowMaxSize(IsVertical);
			row.UpdateRowSize(IsVertical, rowHeight);
			Size size = Size.Empty;
			size = SetSize(new Size(rowHeight, rowHeight), maxWidth);
			if(IsRightToLeft) {
				Rectangle rowRect = row.RowRect;
				rowRect.X = 0;
				row.RowRect = rowRect;
			}
			lastLocation = SetNear(lastLocation, IsRightToLeft ? maxWidth : 0);
			lastLocation = SetFar(lastLocation, GetFar(lastLocation) + rowHeight + rowIndent);
			row.RowRect = new Rectangle(row.RowRect.Location, size);
		}
		protected virtual void DoLayout() {
			DoLayout(true);
		}
		public override Size GetPreferredSize(Size proposedSize) {
			Size res = DoLayout(false);
			return res;
		}
		protected virtual Size DoLayout(bool updateSize) {
			Point lastLocation = Point.Empty;
			int startN, endN, incN;
			startN = 0; incN = 1;
			endN = Rows.Count;
			if (DockStyle == BarDockStyle.Left || DockStyle == BarDockStyle.Top || DockStyle == BarDockStyle.Standalone) {
				startN = 0; incN = 1;
				endN = Rows.Count;
			} else {
				startN = Rows.Count - 1;
				endN = 0; incN = -1;
			}
			if(IsRightToLeft) {
				lastLocation.X = Width; 
			}
			for (int n = startN; (incN == 1 ? n < endN : n >= endN); n += incN) {
				MakeLayout(Rows[n], ref lastLocation, GetIndent(endN, incN, n));
			}
			UpdateWholeRowBarWidth(startN, endN, incN);
			return UpdateControlBounds(startN, endN, incN, updateSize);
		}
		protected virtual void UpdateWholeRowBarWidth(int startN, int endN, int incN) { }
		protected virtual Size GetControlSize() { return Size; }
		Size UpdateControlBounds(int startN, int endN, int incN, bool updateSize) {
			Size prevSize = GetControlSize();
			int size = 0;
			LockSize();
			try {
				for (int n = startN; (incN == 1 ? n < endN : n >= endN); n += incN) {
					for (int r = 0; r < Rows[n].Count; r++) {
						if(updateSize) {
						Rows[n][r].Dockable.Bounds = Rows[n][r].Bounds;
						Rows[n][r].Dockable.SetVisible(true);
					}
					}
					size += GetHeight(Rows[n].RowRect) + GetIndent(endN, incN, n);
				}
				Size newSize = SetHeight(prevSize, size);
				if (DockStyle == BarDockStyle.Bottom) {
					if (newSize.Height < prevSize.Height) Top += prevSize.Height - newSize.Height;
				}
				if (DockStyle == BarDockStyle.Right) {
					if (newSize.Width < prevSize.Width) Left += prevSize.Width - newSize.Width;
				}
				if(updateSize)
					UpdateSize(newSize);
				return newSize;
			} finally {
				UnlockSize();
			}
		}
		protected virtual void UpdateSize(Size newSize) {
			Size = newSize;
		}
		int GetObjIndent(DockRow row, int position) {
			return (position == row.Count - 1 ? 0 : ConstBarDockedRowBarIndent);
		}
		int GetIndent(int endN, int incN, int n) {
			int rowIndent = ConstBarDockedRowIndent;
			if (Rows.Count == 1 && DockStyle != BarDockStyle.Top && !AllowLastIndent()) rowIndent = 0;
			if ((incN > 0 && n == endN - 1) || (incN < 0 && n == 0)) {
				if (DockStyle == BarDockStyle.Top && !AllowLastIndent()) rowIndent = ConstTopDockIndent;
				else {
					if (!AllowLastIndent())
						rowIndent = 0;
				}
			}
			return rowIndent;
		}
		internal void UpdateDockingCore() {
			SendToBack();
			UpdateDockSize();
		}
		public override void UpdateScheme() {
			base.UpdateScheme();
			OnBarDockChanged();
		}
		protected virtual bool IsLockUpdate { get { return lockUpdate != 0 || Manager == null || Manager.IsLoading; } }
		protected internal virtual void OnDockableChanged(IDockableObject dockable) {
			SetDirty();
			Invalidate();
			if (IsHandleCreated)
				BeginInvoke(new MethodInvoker(CheckDirty));
		}
		protected bool isCheckDirty = false;
		protected internal override void CheckDirty() {
			isCheckDirty = true;
			base.CheckDirty();
			if(dirty) {
				UpdateDockSize();
			}
			isCheckDirty = false;
		}
		bool dirty = false;
		protected void SetDirty() { this.dirty = true; }
		protected virtual bool CalcPerformLayoutOnDockChanged(Rectangle prevBounds) { return Bounds != prevBounds; }
		protected internal void OnBarDockChanged() {
			if (IsLockUpdate) return;
			Rectangle prevBounds = Bounds;
			try {
				if (Manager != null && !Manager.IsDesignMode && Manager.IsFormShown) Manager.Form.SuspendLayout();
				if (Parent !=null && Parent.Site != null && !Parent.Site.DesignMode) Parent.SuspendLayout();
				this.SuspendLayout();
				UpdateDockSize();
			} finally {
				bool force = CalcPerformLayoutOnDockChanged(prevBounds);
				this.ResumeLayout(force);
				if (Parent != null && Parent.Site != null && !Parent.Site.DesignMode) Parent.ResumeLayout(force);
				if(Manager != null && !Manager.IsDesignMode && Manager.IsFormShown) Manager.Form.ResumeLayout(force);
				Invalidate();
			}
		}
		protected virtual Size GetEmptyDockSize() { 
			if(Dock == System.Windows.Forms.DockStyle.Top || Dock == System.Windows.Forms.DockStyle.Bottom)
				return new Size(Width, 0);
			if(Dock == System.Windows.Forms.DockStyle.Left || Dock == System.Windows.Forms.DockStyle.Right)
				return new Size(0, Height);
			return Size; 
		}
		protected virtual void MakeControlVisible() {
			Visible = true;
		}
		internal void UpdateDockSize() {
			if (IsSizeUpdating) return;
			this.dirty = false;
			LockSize();
			UpdateVisualEffects(DevExpress.Utils.VisualEffects.UpdateAction.BeginUpdate);
			try {
				if (Rows.GetObjectCount() > 0) {
					DoLayout();
					MakeControlVisible();
				} else
					Size = GetEmptyDockSize();
			} finally {
				UnlockSize();
				UpdateVisualEffects(DevExpress.Utils.VisualEffects.UpdateAction.EndUpdate);
			}
		}
		protected internal virtual Rectangle CalcDockingRectangle() {
			Rectangle dockingRect = Bounds;
			if(Parent == null)
				return Bounds;
			dockingRect.Location = Parent.PointToScreen(dockingRect.Location);
			if (dockingRect.Size.IsEmpty || Controls.Count == 0) {
				if (IsVertical)
					dockingRect.Size = new Size(0, GetMaxClientWidth());
				else
					dockingRect.Size = new Size(GetMaxClientWidth(), 0);
			}
			if(WindowsFormsSettings.GetIsRightToLeftLayout(this))
				dockingRect.X -= dockingRect.Width;
			if (IsVertical)
				dockingRect.Inflate(7, 0);
			else
				dockingRect.Inflate(0, 7);
			return dockingRect;
		}
	}
}
