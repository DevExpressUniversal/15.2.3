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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking {
	public enum VisualizerFormType { Top, Bottom, Left, Right, Center, CenterNoTabs}
	public enum VisualizerState { AllVisible, CenterVisibleWithTabs, CenterVisibleWithoutTabs, AllButCenter, AllHidden}
	public enum VisualizerRole { PanelVisualizer, RootLayoutVisualizer}
	public enum VisualizerHitInfoType { Top, Bottom, Left, Right, CenterCenter, CenterTop, CenterBottom, CenterLeft, CenterRight, Nothing}
	public enum TabsPosition { Top = 1, Bottom, Left, Right }
	public class HotTrackFrame : VisualizerForm {
		SolidBrush hotBrush = new SolidBrush(Color.FromArgb(79, 122, 208));
		protected TabsPosition tabsPosition = TabsPosition.Right;
		bool isTabbed = false;
		public void SetIsTabbed(bool isTabbed) {
			this.isTabbed = isTabbed;
		}
		public void SetTabsPosition(TabsPosition pos) {
			tabsPosition = pos;
		}
		public HotTrackFrame(int fadeSpeed, int framesCount)
			: base(VisualizerFormType.Left, fadeSpeed, framesCount, false) {
			Opacity = 0.39;
			TransparencyKey = Color.Magenta;
			SetStyle(ControlStyles.AllPaintingInWmPaint, false);
			SetStyle(ControlStyles.UserPaint, true);
		}
		protected override void Init() {
			winRegion = null;
			base.Init();
		}
		Rectangle headerRectCore;
		internal Rectangle HeaderRect {
			get { return headerRectCore; }
			set {
				if(HeaderRect != value) {
					headerRectCore = value;
					Invalidate();
					Update();
				}
			} 
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(isTabbed) {
				Rectangle tabRect = ClientRectangle;
				tabRect.Inflate(-4, -4);
				tabRect.Y += 20;
				tabRect.Height -= 20;
				Rectangle clientAreaRect = tabRect;
				Size tabSizeH = new Size(50, 20);
				Size tabSizeV = new Size(20, 50);
				Size headerRectSize = Size.Empty;
				Point headerRectLocation = Point.Empty;
				if(!HeaderRect.IsEmpty) {
					headerRectSize = HeaderRect.Size;
					headerRectLocation = HeaderRect.Location;
				}
				switch(tabsPosition) {
					case TabsPosition.Left:
						tabRect.Width = tabSizeV.Width;
						tabRect.Height = tabSizeV.Height + ((headerRectSize.Height > 0) ? headerRectSize.Height - tabSizeV.Height : 0);
						tabRect.Y = headerRectLocation.Y > 0 ? headerRectLocation.Y : tabRect.Y;
						clientAreaRect.X = tabRect.Right;
						clientAreaRect.Width -= tabRect.Width;
						break;
					case TabsPosition.Right:
						tabRect.Width = tabSizeV.Width;
						tabRect.Height = tabSizeV.Height + ((headerRectSize.Height > 0) ? headerRectSize.Height - tabSizeV.Height : 0);
						tabRect.Y = headerRectLocation.Y > 0 ? headerRectLocation.Y : tabRect.Y;
						clientAreaRect.Width -= tabRect.Width;
						tabRect.X = clientAreaRect.Right;
						break;
					case TabsPosition.Top:
						tabRect.Width = tabSizeH.Width + ((headerRectSize.Width > 0) ? headerRectSize.Width - tabSizeH.Width : 0);
						tabRect.Height = tabSizeH.Height;
						tabRect.X = headerRectLocation.X > 0 ? headerRectLocation.X : tabRect.X;
						clientAreaRect.Y = tabRect.Bottom;
						clientAreaRect.Height -= tabRect.Height;
						break;
					case TabsPosition.Bottom:
						tabRect.Width = tabSizeH.Width + ((headerRectSize.Width > 0) ? headerRectSize.Width - tabSizeH.Width : 0);
						tabRect.Height = tabSizeH.Height;
						tabRect.X = headerRectLocation.X > 0 ? headerRectLocation.X : tabRect.X;
						clientAreaRect.Height -= tabRect.Height;
						tabRect.Y = clientAreaRect.Bottom;
						break;
				}
				FillTabRect(e, tabRect, clientAreaRect);
			}
			else FillDockRect(e, ClientRectangle);
		}
		protected virtual void FillTabRect(PaintEventArgs e, Rectangle header, Rectangle content) {
			e.Graphics.FillRectangle(hotBrush, header);
			e.Graphics.FillRectangle(hotBrush, content);
		}
		protected virtual void FillDockRect(PaintEventArgs e, Rectangle bounds) {
			e.Graphics.FillRectangle(hotBrush, bounds);
		}
	}
	public class VisualizerForm : DevExpress.Utils.DragDrop.BaseDragHelperForm {
		VisualizerFormType formTypeCore;
		VisualizerVisibilityArgs visibilityArgs;
		[ThreadStatic]
		static Bitmap topBmp, bottomBmp, leftBmp, rightBmp, centerBmp,centerNoTabsBmp;
		protected Region winRegion = null;
		bool allowInit = false;
		static VisualizerForm() {}
		protected Bitmap TopBmp {
			get {
				if(topBmp == null) topBmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.up.png", typeof(VisualizerForm).Assembly);
				return topBmp;
			}
		}
		protected Bitmap BottomBmp {
			get {
				if (bottomBmp == null) bottomBmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.down.png", typeof(VisualizerForm).Assembly);
				return bottomBmp;
			}
		}
		protected Bitmap LeftBmp {
			get {
				if (leftBmp == null) leftBmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.left.png", typeof(VisualizerForm).Assembly);
				return leftBmp;
			}
		}
		protected Bitmap RightBmp {
			get {
				if (rightBmp == null) rightBmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.right.png", typeof(VisualizerForm).Assembly);
				return rightBmp;
			}
		}
		protected Bitmap CenterBmp {
			get {
				if (centerBmp == null) centerBmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.center.png", typeof(VisualizerForm).Assembly);
				return centerBmp;
			}
		}
		protected Bitmap CenterNoTabsBmp {
			get {
				if (centerNoTabsBmp == null) centerNoTabsBmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.center-without-tab.png", typeof(VisualizerForm).Assembly);
				return centerNoTabsBmp;
			}
		}
		protected override void Init() {
			if(!allowInit) return;
			base.Init();
			if(winRegion != null) this.Region = winRegion;
		}
		public VisualizerForm(VisualizerFormType formType, int fadeSpeed, int framesCount, bool needCalculateRegion)
			: base(fadeSpeed, framesCount) {
			allowInit = false;
			formTypeCore = formType;
			if(needCalculateRegion) 
				winRegion = DevExpress.Utils.Win.BitmapToRegion.Convert(ActualBitmap, Color.Magenta);
			else winRegion = null;
			allowInit = true;
			Init();
		}
		public VisualizerFormType FormType {
			get { return formTypeCore; }
			set { formTypeCore = value; Invalidate(); Update(); }
		}
		protected virtual Bitmap ActualBitmap {
			get {
				switch(formTypeCore) {
					case VisualizerFormType.Bottom:
						return BottomBmp;
					case VisualizerFormType.Center:
						return CenterBmp;
					case VisualizerFormType.Left:
						return LeftBmp;
					case VisualizerFormType.Right:
						return RightBmp;
					case VisualizerFormType.Top:
						return TopBmp;
					case VisualizerFormType.CenterNoTabs:
						return CenterNoTabsBmp;
				}
				return null;
			}
		}
		public VisualizerVisibilityArgs VisualizerVisibilityState {
			get { return visibilityArgs; }
			set { visibilityArgs = value; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			e.Graphics.DrawImage(ActualBitmap, Point.Empty);
		}
	}
	public class WindowInfo {
		Rectangle bounds;
		bool visible;
		VisualizerFormType type;
		VisualizerVisibilityArgs visibilityArgs;
		public WindowInfo(Rectangle bounds, bool visible, VisualizerFormType type) {
			Bounds = bounds;
			Visible = visible;
			this.type = type;
		}
		public WindowInfo(VisualizerFormType type) {
			this.type = type;
		}
		public Rectangle Bounds {
			get {
				return bounds;
			}
			set {
				bounds = value;
			}
		}
		public VisualizerFormType Type {
			get {
				return type;
			}
		}
		public bool Visible {
			get {
				return visible;
			}
			set {
				visible = value;
			}
		}
		public VisualizerVisibilityArgs VisibilityArgs {
			get { return visibilityArgs; }
			set { visibilityArgs = value; }
		}
	}
	public class VS2005StyleDockingVisualizerViewInfo {
		Hashtable windows = new Hashtable();
		VS2005StyleDockingVisualizer owner;
		public VS2005StyleDockingVisualizerViewInfo(VS2005StyleDockingVisualizer owner) {
			this.owner = owner;
			CreateWindows();
		}
		protected virtual void CreateWindows() {
			windows.Add(VisualizerFormType.Bottom, new WindowInfo(VisualizerFormType.Bottom));
			windows.Add(VisualizerFormType.Center, new WindowInfo(VisualizerFormType.Center));
			windows.Add(VisualizerFormType.CenterNoTabs, new WindowInfo(VisualizerFormType.CenterNoTabs));
			windows.Add(VisualizerFormType.Left, new WindowInfo(VisualizerFormType.Left));
			windows.Add(VisualizerFormType.Right, new WindowInfo(VisualizerFormType.Right));
			windows.Add(VisualizerFormType.Top, new WindowInfo(VisualizerFormType.Top));
		}
		public WindowInfo this[VisualizerFormType type] {
			get { return (WindowInfo)windows[type]; }
		}
		protected Rectangle GetWorkingArea(Rectangle rect) {
			Rectangle workingArea = rect;
			workingArea.X += owner.Padding.X;
			workingArea.Y += owner.Padding.Y;
			workingArea.Width -= (owner.Padding.X + owner.Padding.Width);
			workingArea.Height -= (owner.Padding.Y + +owner.Padding.Height);
			return workingArea;
		}
		protected Point GetCenter(Rectangle rect) {
			return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
		}
		protected virtual void CalculateRectangles() {
			Rectangle workingArea = GetWorkingArea(owner.Bounds);
			Point centerWorkingArea = GetCenter(workingArea);
			this[VisualizerFormType.Bottom].Bounds =
				new Rectangle(new Point(centerWorkingArea.X - owner[VisualizerFormType.Bottom].Width / 2,
				workingArea.Bottom - owner[VisualizerFormType.Bottom].Height), owner[VisualizerFormType.Bottom]);
			this[VisualizerFormType.Top].Bounds =
				new Rectangle(new Point(centerWorkingArea.X - owner[VisualizerFormType.Top].Width / 2,
				workingArea.Top), owner[VisualizerFormType.Top]);
			this[VisualizerFormType.Left].Bounds =
				new Rectangle(new Point(workingArea.Left,
				centerWorkingArea.Y - owner[VisualizerFormType.Left].Height / 2), owner[VisualizerFormType.Left]);
			this[VisualizerFormType.Right].Bounds =
				new Rectangle(new Point(workingArea.Right - owner[VisualizerFormType.Right].Width,
				centerWorkingArea.Y - owner[VisualizerFormType.Right].Height / 2), owner[VisualizerFormType.Right]);
			if(owner.Role == VisualizerRole.RootLayoutVisualizer) {
				workingArea = GetWorkingArea(owner.RestBounds);
				centerWorkingArea = GetCenter(workingArea);
			}
			this[VisualizerFormType.Center].Bounds = this[VisualizerFormType.CenterNoTabs].Bounds =
				new Rectangle(new Point(centerWorkingArea.X - owner[VisualizerFormType.Center].Width / 2, centerWorkingArea.Y - owner[VisualizerFormType.Center].Height / 2), owner[VisualizerFormType.Center]);
		}
		protected void SetAllVisible(bool visible) {
			this[VisualizerFormType.Bottom].Visible = visible;
			this[VisualizerFormType.Center].Visible = visible;
			this[VisualizerFormType.CenterNoTabs].Visible = visible;
			this[VisualizerFormType.Left].Visible = visible;
			this[VisualizerFormType.Right].Visible = visible;
			this[VisualizerFormType.Top].Visible = visible;
		}
		protected virtual void CalculateVisibility() {
			switch(owner.State) {
				case VisualizerState.AllVisible:
					SetAllVisible(true);
					if(owner.Role == VisualizerRole.PanelVisualizer)
						this[VisualizerFormType.CenterNoTabs].Visible = false;
					else
						this[VisualizerFormType.Center].Visible = false;
					break;
				case VisualizerState.AllButCenter:
					SetAllVisible(true);
					this[VisualizerFormType.Center].Visible = false;
					this[VisualizerFormType.CenterNoTabs].Visible = false;
					break;
				case VisualizerState.CenterVisibleWithoutTabs:
					SetAllVisible(false);
					this[VisualizerFormType.CenterNoTabs].Visible = true;
					break;
				case VisualizerState.CenterVisibleWithTabs:
					SetAllVisible(false);
					this[VisualizerFormType.Center].Visible = true;
					break;
				case VisualizerState.AllHidden:
					SetAllVisible(false);
					break;
			}
			if(owner.StateArgs != null) {
				if(owner.State == VisualizerState.AllButCenter || owner.State == VisualizerState.AllVisible) {
					this[VisualizerFormType.Top].Visible &= owner.StateArgs.Top;
					this[VisualizerFormType.Bottom].Visible &= owner.StateArgs.Bottom;
					this[VisualizerFormType.Left].Visible &= owner.StateArgs.Left;
					this[VisualizerFormType.Right].Visible &= owner.StateArgs.Right;
				}
				if(owner.State == VisualizerState.CenterVisibleWithTabs) {
					this[VisualizerFormType.Top].Visible &= owner.StateArgs.Top;
					this[VisualizerFormType.Bottom].Visible &= owner.StateArgs.Bottom;
					this[VisualizerFormType.Left].Visible &= owner.StateArgs.Left;
					this[VisualizerFormType.Right].Visible &= owner.StateArgs.Right;
					this[VisualizerFormType.Center].Visible &= owner.StateArgs.Tabbed;
					if(!this[VisualizerFormType.Center].Visible)
						this[VisualizerFormType.CenterNoTabs].Visible = true;
				}
				((WindowInfo)windows[VisualizerFormType.Center]).VisibilityArgs = owner.StateArgs;
				((WindowInfo)windows[VisualizerFormType.CenterNoTabs]).VisibilityArgs = owner.StateArgs;
			}
		}
		public void Calculate() {
			CalculateRectangles();
			CalculateVisibility();
		}
		protected virtual VisualizerHitInfoType CalcCenterHitInfo(Point point, bool withTabs, WindowInfo winInfo) {
			Rectangle centerRectBounds = this[withTabs ? VisualizerFormType.Center : VisualizerFormType.CenterNoTabs].Bounds;
			Point p1 = new Point(centerRectBounds.X, centerRectBounds.Y + this[VisualizerFormType.Top].Bounds.Height);
			Point p2 = new Point(centerRectBounds.X + this[VisualizerFormType.Left].Bounds.Width, centerRectBounds.Y);
			Rectangle topRect = new Rectangle(p2, this[VisualizerFormType.Top].Bounds.Size);
			Rectangle centerRect = new Rectangle(new Point(topRect.X, topRect.Bottom), new Size(topRect.Width, this[VisualizerFormType.Left].Bounds.Height));
			Rectangle botomRect = new Rectangle(new Point(centerRect.X, centerRect.Bottom), this[VisualizerFormType.Bottom].Bounds.Size);
			Rectangle leftRect = new Rectangle(p1, this[VisualizerFormType.Left].Bounds.Size);
			Rectangle rightRect = new Rectangle(new Point(centerRect.Right, centerRect.Y), this[VisualizerFormType.Right].Bounds.Size);
			if(winInfo == null || winInfo.VisibilityArgs == null) return VisualizerHitInfoType.Nothing;
			if(topRect.Contains(point) && winInfo.VisibilityArgs.Top) return VisualizerHitInfoType.CenterTop;
			if(centerRect.Contains(point) && winInfo.VisibilityArgs.Tabbed) return VisualizerHitInfoType.CenterCenter;
			if(leftRect.Contains(point) && winInfo.VisibilityArgs.Left) return VisualizerHitInfoType.CenterLeft;
			if(rightRect.Contains(point) && winInfo.VisibilityArgs.Right) return VisualizerHitInfoType.CenterRight;
			if(botomRect.Contains(point) && winInfo.VisibilityArgs.Bottom) return VisualizerHitInfoType.CenterBottom;
			return VisualizerHitInfoType.Nothing;
		}
		public VisualizerHitInfoType CalcHitInfo(Point point) {
			if(this[VisualizerFormType.Bottom].Bounds.Contains(point) && this[VisualizerFormType.Bottom].Visible) return VisualizerHitInfoType.Bottom;
			if(this[VisualizerFormType.Top].Bounds.Contains(point) && this[VisualizerFormType.Top].Visible) return VisualizerHitInfoType.Top;
			if(this[VisualizerFormType.Left].Bounds.Contains(point) && this[VisualizerFormType.Left].Visible) return VisualizerHitInfoType.Left;
			if(this[VisualizerFormType.Right].Bounds.Contains(point) && this[VisualizerFormType.Right].Visible) return VisualizerHitInfoType.Right;
			if(this[VisualizerFormType.CenterNoTabs].Bounds.Contains(point) && this[VisualizerFormType.CenterNoTabs].Visible) return CalcCenterHitInfo(point, false, (WindowInfo)windows[VisualizerFormType.CenterNoTabs]);
			if(this[VisualizerFormType.Center].Bounds.Contains(point) && this[VisualizerFormType.Center].Visible) return CalcCenterHitInfo(point, false, (WindowInfo)windows[VisualizerFormType.Center]);
			return VisualizerHitInfoType.Nothing;
		}
	}
	public class VisualizerVisibilityArgs {
		bool top, bottom, left, right, tabbed;
		public bool Top { get { return top; } set { top = value; } }
		public bool Bottom { get { return bottom; } set { bottom = value; } }
		public bool Left { get { return left; } set { left = value; } }
		public bool Right { get { return right; } set { right = value; } }
		public bool Tabbed { get { return tabbed; } set { tabbed = value; } }
	}
	public class VS2005StyleDockingVisualizer : IDisposable{
		protected Hashtable frames = new Hashtable();
		protected HotTrackFrame hotTrackWindow;
		VisualizerState state;
		VisualizerVisibilityArgs stateArgs;
		protected VS2005StyleDockingVisualizerViewInfo viewInfoCore;
		Rectangle bounds, padding = new Rectangle(20,20,20,20);
		Rectangle restBoundsCore = Rectangle.Empty;
		VisualizerRole roleCore;
		protected int fadeSpeed;
		protected int fadeFramesCount;
		protected Hashtable defaultWindowSizes = new Hashtable();
		public VS2005StyleDockingVisualizer(VisualizerRole role, int fadeSpeed, int framesCount) {
			this.fadeSpeed = fadeSpeed;
			this.fadeFramesCount = framesCount;
			roleCore = role;
			CreateWindows();
			SetDefaultWindowSizes();
			viewInfoCore = CreateViewInfo();
			state = VisualizerState.AllHidden;
		}
		protected virtual void SetDefaultWindowSizes() {
			Size defaultH = new Size(29, 31);
			Size defaultW = new Size(31, 29);
			defaultWindowSizes.Add(VisualizerFormType.Bottom, defaultH);
			defaultWindowSizes.Add(VisualizerFormType.Center, new Size(89, 89));
			defaultWindowSizes.Add(VisualizerFormType.CenterNoTabs, new Size(89, 89));
			defaultWindowSizes.Add(VisualizerFormType.Left, defaultW);
			defaultWindowSizes.Add(VisualizerFormType.Right, defaultW);
			defaultWindowSizes.Add(VisualizerFormType.Top, defaultH);
		}
		protected virtual void CreateWindows() {
			frames.Add(VisualizerFormType.Bottom, new VisualizerForm(VisualizerFormType.Bottom, fadeSpeed, fadeFramesCount, false));
			frames.Add(VisualizerFormType.Center, new VisualizerForm(VisualizerFormType.Center, fadeSpeed, fadeFramesCount, true));
			frames.Add(VisualizerFormType.CenterNoTabs, new VisualizerForm(VisualizerFormType.CenterNoTabs, fadeFramesCount, fadeSpeed, true));
			frames.Add(VisualizerFormType.Left, new VisualizerForm(VisualizerFormType.Left, fadeSpeed, fadeFramesCount, false));
			frames.Add(VisualizerFormType.Right, new VisualizerForm(VisualizerFormType.Right, fadeSpeed, fadeFramesCount, false));
			frames.Add(VisualizerFormType.Top, new VisualizerForm(VisualizerFormType.Top, fadeSpeed, fadeFramesCount, false));
			hotTrackWindow = new HotTrackFrame(fadeSpeed, fadeFramesCount);
		}
		public VisualizerHitInfoType CalcHitInfo(Point p) {
			return CalcHitInfoCore(p);
		}
		protected virtual VisualizerHitInfoType CalcHitInfoCore(Point p) {
			return viewInfoCore.CalcHitInfo(p);
		}
		protected virtual VS2005StyleDockingVisualizerViewInfo CreateViewInfo() {
			return new VS2005StyleDockingVisualizerViewInfo(this);
		}
		public VisualizerRole Role {
			get { return roleCore; }
		}
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if(bounds != value) {
					bounds = value;
					Update();
				}
			}
		}
		public VS2005StyleDockingVisualizerViewInfo ViewInfo { 
			get { return viewInfoCore; } 
		}
		public Rectangle RestBounds {
			get { return restBoundsCore; }
			set {
				if(restBoundsCore != value) {
					restBoundsCore = value;
					Update();
				}
			}
		}
		public Size this[VisualizerFormType type] {
			get { return (Size)defaultWindowSizes[type]; }
		}
		public VisualizerState State {
			get { return state; }
		}
		public VisualizerVisibilityArgs StateArgs {
			get { return stateArgs; }
		}
		public virtual Rectangle Padding {
			get { return padding; }
		}
		public VisualizerForm GetFrame(VisualizerFormType type) {
			return frames[type] as VisualizerForm;
		}
		protected void SetBoundsVisibility(VisualizerForm frame, WindowInfo info ) {
			frame.SetBounds(info.Bounds.X, info.Bounds.Y, info.Bounds.Width, info.Bounds.Height);
			frame.Visible = info.Visible;
			if(info.VisibilityArgs != null) frame.VisualizerVisibilityState = info.VisibilityArgs;
			frame.Refresh();
		}
		protected virtual void UpdateWindows() {
			SetBoundsVisibility(GetFrame(VisualizerFormType.Bottom), this.viewInfoCore[VisualizerFormType.Bottom]);
			SetBoundsVisibility(GetFrame(VisualizerFormType.Center), this.viewInfoCore[VisualizerFormType.Center]);
			SetBoundsVisibility(GetFrame(VisualizerFormType.CenterNoTabs), this.viewInfoCore[VisualizerFormType.CenterNoTabs]);
			SetBoundsVisibility(GetFrame(VisualizerFormType.Left), this.viewInfoCore[VisualizerFormType.Left]);
			SetBoundsVisibility(GetFrame(VisualizerFormType.Right), this.viewInfoCore[VisualizerFormType.Right]);
			SetBoundsVisibility(GetFrame(VisualizerFormType.Top), this.viewInfoCore[VisualizerFormType.Top]);
		}
		protected void UpdateState(VisualizerState newState, VisualizerVisibilityArgs args) {
			if(State != newState) {
				state = newState;
				stateArgs = args;
				Update();
			}
		}
		public void ShowCenter(VisualizerVisibilityArgs args) {
			UpdateState(VisualizerState.CenterVisibleWithTabs, args);
		}
		public void ShowAllButCenter(VisualizerVisibilityArgs args) {
			UpdateState(VisualizerState.AllButCenter, args);
		}
		public void ShowAll(VisualizerVisibilityArgs args) {
			UpdateState(VisualizerState.AllVisible, args);
		}
		public void Hide() {
			UpdateState(VisualizerState.AllHidden, new VisualizerVisibilityArgs());
			if(hotTrackWindow != null)
				hotTrackWindow.Visible = false;
		}
		bool isDisposingCore;
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		public void Dispose() {
			if(!isDisposingCore) {
				isDisposingCore = true;
				OnDispose();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDispose() {
			if(hotTrackWindow != null)
				hotTrackWindow.Dispose();
			foreach(VisualizerForm form in frames.Values) {
				form.Dispose();
			}
		}
		public virtual bool UpdateHotTracked(Point point, bool processOnlyCenter, TabsPosition pos) {
			return UpdateHotTracked(point, processOnlyCenter, pos, Rectangle.Empty);
		}
		public virtual bool UpdateHotTracked(Point point, bool processOnlyCenter, TabsPosition pos, Rectangle headerRect) {
			viewInfoCore.Calculate();
			VisualizerHitInfoType hitInfo = CalcHitInfo(point);
			Rectangle hotTrackedWindowBounds;
			if(Role == VisualizerRole.RootLayoutVisualizer && (
				hitInfo == VisualizerHitInfoType.CenterBottom ||
				hitInfo == VisualizerHitInfoType.CenterLeft ||
				hitInfo == VisualizerHitInfoType.CenterRight ||
				hitInfo == VisualizerHitInfoType.CenterTop))
				hotTrackedWindowBounds = RestBounds;
			else
				hotTrackedWindowBounds = Bounds;
			if(Role == VisualizerRole.RootLayoutVisualizer && hitInfo == VisualizerHitInfoType.CenterCenter)
				hotTrackedWindowBounds = Rectangle.Empty;
			if(processOnlyCenter) {
				if(hitInfo == VisualizerHitInfoType.Bottom ||
				hitInfo == VisualizerHitInfoType.Top ||
				hitInfo == VisualizerHitInfoType.Left ||
				hitInfo == VisualizerHitInfoType.Right)
					hitInfo = VisualizerHitInfoType.Nothing;
			}
			hotTrackWindow.SetIsTabbed(false);
			switch(hitInfo) {
				case VisualizerHitInfoType.Nothing:
					if(!headerRect.IsEmpty) {
						hotTrackWindow.SetIsTabbed(true);
						hotTrackWindow.SetTabsPosition(pos);
						hotTrackWindow.HeaderRect = headerRect;
					}
					else {
						hotTrackWindow.HeaderRect = Rectangle.Empty;
						hotTrackedWindowBounds = Rectangle.Empty;
					}
					break;
				case VisualizerHitInfoType.CenterCenter:
					hotTrackWindow.SetIsTabbed(true);
					hotTrackWindow.SetTabsPosition(pos);
					break;
				case VisualizerHitInfoType.Bottom:
				case VisualizerHitInfoType.CenterBottom:
					hotTrackedWindowBounds.Y += hotTrackedWindowBounds.Height / 3 * 2;
					hotTrackedWindowBounds.Height = hotTrackedWindowBounds.Height / 3;
					break;
				case VisualizerHitInfoType.Top:
				case VisualizerHitInfoType.CenterTop:
					hotTrackedWindowBounds.Height = hotTrackedWindowBounds.Height / 3;
					break;
				case VisualizerHitInfoType.Left:
				case VisualizerHitInfoType.CenterLeft:
					hotTrackedWindowBounds.Width = hotTrackedWindowBounds.Width / 3;
					break;
				case VisualizerHitInfoType.Right:
				case VisualizerHitInfoType.CenterRight:
					hotTrackedWindowBounds.X += hotTrackedWindowBounds.Width / 3 * 2;
					hotTrackedWindowBounds.Width = hotTrackedWindowBounds.Width / 3;
					break;
			}
			bool result = false;
			if(hotTrackedWindowBounds == Rectangle.Empty) {
				hotTrackWindow.Visible = false;
			}
			else {
				hotTrackWindow.Bounds = hotTrackedWindowBounds;
				hotTrackWindow.Visible = true;
				result = true;
			}
			return result;
		}
		public void Update() {
			viewInfoCore.Calculate();
			UpdateWindows();
		}
	}
}
