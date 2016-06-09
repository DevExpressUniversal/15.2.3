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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraBars.Objects;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
namespace DevExpress.XtraBars.ViewInfo {
	public class LocationInfoSimple : LocationInfo {
		public LocationInfoSimple(Point location, Point altLocation) : base(location, altLocation, true, false) { }
		protected override int CalcX() {
			Rectangle rect = WorkingArea;
			Rectangle loc = new Rectangle(Location, WindowSize);
			if(rect.Contains(loc) || (loc.X >= rect.X && loc.Right <= rect.Right)) return loc.X;
			if(loc.Right > rect.Right) loc.X = rect.Right - loc.Width;
			loc.X = Math.Max(loc.X, rect.X);
			return loc.X;
		}
	}
	public class LocationInfo {
		Size windowSize;
		bool verticalOpen, onePointLocation, openXBack;
		bool useSystemMenuAlignment = false;
		Point location, altLocation;
		Rectangle workingArea = new Rectangle(-10000, -10000, 0, 0);
		Rectangle forceFormBounds = Rectangle.Empty;
		public LocationInfo(Point location, Point altLocation, bool verticalOpen, bool openXBack, bool onePointLocation) : 
			this(location, altLocation, verticalOpen, openXBack) {
			this.onePointLocation = onePointLocation;
		}
		public LocationInfo(Point location, Point altLocation, bool verticalOpen, bool openXBack, bool onePointLocation, bool useSystemMenuAlignment) :
			this(location, altLocation, verticalOpen, openXBack) {
			this.onePointLocation = onePointLocation;
			this.useSystemMenuAlignment = useSystemMenuAlignment;
		}
		public LocationInfo(Point location, Point altLocation, bool verticalOpen, bool openXBack) {
			this.windowSize = Size.Empty;
			this.onePointLocation = false;
			this.location = location;
			this.altLocation = altLocation;
			this.verticalOpen = verticalOpen;
			this.openXBack = openXBack;
		}
		public LocationInfo(Point onePoint) : this(onePoint, onePoint, false, false) {
			this.onePointLocation = true;
			this.location = onePoint;
		}
		protected Point OriginalLocation {
			get { return location; }
		}
		public Point Location { 
			get { 
				Point res = location;
				if(OpenXBack) {
					res.X -= WindowSize.Width;
				}
				return  res; 
			} 
		}
		public Point AltLocation { 
			get {
				if(OnePointLocation) return Location;
				Point res = altLocation;
				if(OpenXBack) {
					res.X -= WindowSize.Width;
				}
				return  res; 
			}
		}
		public Rectangle ForceFormBounds {
			get { return forceFormBounds; }
			set { forceFormBounds = value; }
		}
		public bool VerticalOpen { get { return verticalOpen; } }
		public bool OpenXBack { get { return openXBack; } }
		public Size WindowSize { get { return windowSize; }  set { windowSize = value; } }
		public virtual Point ShowPoint {
			get {
				if(ForceFormBounds.IsEmpty) return new Point(CalcX(), CalcY());
				return ForceFormBounds.Location;
			}
		}
		protected virtual bool OnePointLocation { get { return onePointLocation; } }
		protected virtual Rectangle WorkingArea {
			get {
				if(workingArea.X != -10000) return workingArea;
				Rectangle rect = ControlUtils.UseVirtualScreenForDropDown ? SystemInformation.VirtualScreen : SystemInformation.WorkingArea;
				if(SystemInformation.MonitorCount > 1) {
					Point pt = OriginalLocation;
					if(OpenXBack) {
						pt.X--;
						Screen ptScreen = Screen.FromPoint(pt);
						pt.X = Math.Max(pt.X, ControlUtils.UseVirtualScreenForDropDown ? ptScreen.Bounds.X : ptScreen.WorkingArea.X);
					}
					Screen scrBottom = Screen.FromPoint(pt), scrTop;
					if(Location == AltLocation)
						scrTop = scrBottom;
					else
						scrTop = Screen.FromPoint(AltLocation);
					if(scrBottom.Equals(scrTop)) rect = ControlUtils.UseVirtualScreenForDropDown ? scrTop.Bounds : scrTop.WorkingArea;
					else {
						rect = ControlUtils.UseVirtualScreenForDropDown ? scrBottom.Bounds : scrBottom.WorkingArea;
					}
				}
				workingArea = rect;
				return workingArea;
			}
		}
		protected virtual int CalcY() {
			int y = Location.Y;
			Rectangle rect = WorkingArea;
			int bottom = y + WindowSize.Height;
			int top = y - WindowSize.Height;
			int maxBottomOutsize = bottom - rect.Bottom;
			int maxTopOutsize = rect.Top - top;
			if(maxBottomOutsize > 0 && maxBottomOutsize > maxTopOutsize) {
				if(!OnePointLocation && VerticalOpen) y = AltLocation.Y - WindowSize.Height;
				else
					y = rect.Bottom - WindowSize.Height;
			} else {
				if(maxBottomOutsize > 0 && OnePointLocation) y = rect.Bottom - WindowSize.Height;
			}
			if(y < rect.Y) y = rect.Y;
			return y;
		}
		protected virtual int CalcX() {
			int x = Location.X;
			Rectangle rect = WorkingArea;
			int maxRightOutsize = MaxRightOutsize;
			int maxLeftOutsize = MaxLeftOutsize;
			if(maxRightOutsize > 0 && maxRightOutsize > maxLeftOutsize) {
				if(!OnePointLocation && !VerticalOpen) x = AltLocation.X - WindowSize.Width;
				else x = rect.Right - WindowSize.Width;
			}
			if(x < rect.Left) {
				if(!OnePointLocation && !VerticalOpen) x = AltLocation.X;
				else
					x = rect.Left;
			}
			return x;
		}
		public virtual int CalcMaxHeight() {
			Rectangle workArea = WorkingArea;
			int y = CalcY();
			if(!OnePointLocation && VerticalOpen && y < AltLocation.Y) {
				return AltLocation.Y - y;
			}
			return workArea.Bottom - y;
		}
		protected int MaxLeftOutsize { get { return WorkingArea.Left - (Location.X - WindowSize.Width); } }
		protected int MaxRightOutsize { get { return Location.X + WindowSize.Width - WorkingArea.Right; } }
	}
	public enum MarkArrowDirection { Left, Right, Down, Up };
	public enum LinkDropTargetEnum { None, Before, After };
	public enum LinkViewInfoRange { Current, Prev, Next };
	public enum LinkHitTest { None, LeftEdge, RightEdge, Body};
	public class LinkHitInfo {
		public Point Position;
		public LinkHitTest HitTest;
		public LinkHitInfo() {
			Clear();
		}
		public virtual void Clear() {
			HitTest = LinkHitTest.None;
			Position = BarManager.zeroPoint;
		}
	}
	public class BarLinkGetValueEventArgs : EventArgs {
		object val; 
		public BarLinkGetValueEventArgs(BarLinkViewInfo linkInfo, object val) {
			this.val = val;
			LinkInfo = linkInfo;
		}
		public BarLinkViewInfo LinkInfo { get; private set; }
		public object Value {
			get { return val; }
			set { val = value; }
		}
	}
	public delegate void BarLinkGetValueEventHandler(object sender, BarLinkGetValueEventArgs e);
	#region class CustomViewInfo
	public abstract class CustomViewInfo {
		protected bool ready;
		internal BarDrawParameters drawParameters;
		CustomViewInfo parentViewInfo;
		public object SourceObject;
		GraphicsInfo gInfo;
		BarManager manager;
		internal CustomViewInfo(BarManager manager, BarDrawParameters parameters) {
			this.manager = manager;
			this.drawParameters = parameters;
			this.ParentViewInfo = null;
			this.ready = false;
			this.gInfo = new GraphicsInfo();
		}
		public virtual CustomViewInfo ParentViewInfo { get { return parentViewInfo; } set { parentViewInfo = value; } }
		public GraphicsInfo GInfo { get { return gInfo; } }
		public BarManager Manager { get { return manager; } }
		public virtual BarDrawParameters DrawParameters { get { return drawParameters; } }
		public BarManagerPaintStyle PaintStyle { get { return DrawParameters.PaintStyle; } }
		public void UpdateInfo() {
			Update();
		}
		protected virtual void Update() {
		}
		public virtual void Clear() {
			ready = false; 
			SourceObject = null;
		}
		public virtual bool CheckReady(object sourceObject) {
			if(IsReady) return true;
			CalcViewInfo(null, sourceObject, Rectangle.Empty);
			return IsReady;
		}
		public virtual void CalcViewInfo(Graphics g, object sourceObject, Rectangle r) {
			Update();
			this.SourceObject = sourceObject;
			this.ready = true;
		}
		public virtual void ClearReady() { ready = false; }
		public virtual bool IsReady { get { return ready; } }
	}
	#endregion class CustomViewInfo
	public class ControlFormViewInfo : CustomViewInfo {
		ControlForm controlForm;
		public Rectangle WindowRect, CaptionRect, ControlRect, ContentRect;
		public string CaptionText;
		CustomPainter painter;
		public ControlFormViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters) {
			Clear();
			this.controlForm = controlForm;
			this.painter = ControlForm.Manager.Helper.GetControlPainter(ControlForm.GetType());
		}
		public virtual CustomPainter Painter { get { return painter; } }
		public virtual TitleBarEl CreateTitleBarInstance() {
			return new TitleBarEl(ControlForm.Manager);
		}
		public ControlForm ControlForm { get { return controlForm; } }
		public virtual bool IsShowCaption { get { return false; } }
		public virtual bool IsShowShadow { get { return DrawParameters.Constants.BarControlFormHasShadow; } }
		public virtual Brush FormBackBrush { get { return SystemBrushes.Window; } }
		public override void Clear() {
			ContentRect = WindowRect = CaptionRect = ControlRect = Rectangle.Empty;
			CaptionText = "";
			SourceObject = null;
			base.Clear();
		}
		public virtual Rectangle OwnerRectangle {
			get {
				IPopup popup = ControlForm.ContainedControl as IPopup;
				if(popup != null) return popup.PopupOwnerRectangle;
				return Rectangle.Empty;
			}
		}
		protected virtual int CalcContentIndent(BarIndent indent) {
			return 0;
		}
		protected virtual Size CalcBorderSize() { 
			return new Size(DrawParameters.Constants.BarControlFormBorderIndent, DrawParameters.Constants.BarControlFormBorderIndent);
		}
		public virtual Size CalcMinSize() {
			Size res = new Size(CalcBorderSize().Width * 2, CalcBorderSize().Height * 2);
			if(IsShowCaption) {
				res.Height += ControlForm.TitleBar.CalcHeight(null, 0);
			}
			res.Width += CalcContentIndent(BarIndent.Right) + CalcContentIndent(BarIndent.Left);
			res.Height += CalcContentIndent(BarIndent.Top) + CalcContentIndent(BarIndent.Bottom);
			return res;
		}
		protected virtual string GetCaptionText() { return "";}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle r) {
			Clear();
			SourceObject = sourceObject;
			Rectangle R;
			WindowRect = r;
			if(r.IsEmpty) return;
			R = WindowRect;
			R.Inflate(-CalcBorderSize().Width, -CalcBorderSize().Height);
			ContentRect = R;
			if(IsShowCaption) {
				CaptionRect = R;
				CaptionRect.Height = ControlForm.TitleBar.CalcHeight(g, 0);
				ControlForm.TitleBar.BeginUpdate();
				ControlForm.TitleBar.Caption = GetCaptionText();
				ControlForm.TitleBar.EndUpdate();
				ControlForm.TitleBar.ElementRectangle = CaptionRect;
				ContentRect.Y = CaptionRect.Bottom;
			}
			ContentRect.Height = R.Bottom - ContentRect.Top;
			r = ContentRect;
			r.X += CalcContentIndent(BarIndent.Left); r.Width -= (CalcContentIndent(BarIndent.Left) + CalcContentIndent(BarIndent.Right));
			r.Y += CalcContentIndent(BarIndent.Top); r.Height -= (CalcContentIndent(BarIndent.Top) + CalcContentIndent(BarIndent.Bottom));
			ControlRect = r;
			ready = true;
		}
	}
	public class  PopupContainerFormViewInfo : ControlFormViewInfo {
		public PopupContainerFormViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters, controlForm) {
		}
	}
	public class FloatingBarControlFormViewInfo : ControlFormViewInfo {
		public FloatingBarControlFormViewInfo(BarManager manager, BarDrawParameters parameters, ControlForm controlForm) : base(manager, parameters, controlForm) {
		}
		public override bool IsShowCaption { get { return Bar.OptionsBar.AllowCaptionWhenFloating; } }
		public virtual Bar Bar { 
			get {
				if(BarControl != null) return BarControl.Bar;
				return null;
			}
		}
		public virtual FloatingBarControl BarControl { 
			get {
				if(ControlForm.ContainedControl == null) return null;
				return ControlForm.ContainedControl as FloatingBarControl;
			}
		}
		public override TitleBarEl CreateTitleBarInstance() { return new FloatingBarTitleBarEl(ControlForm.Manager); }
		protected override Size CalcBorderSize() { return new Size(3, 3); }
		protected override string GetCaptionText() { return Bar != null ? Bar.Text : ""; }
	}
	public enum RectInfoType { None, SubMenuSeparator, HorzSeparator, VertSeparator, BackGround};
	public class RectInfo {
		public Rectangle Rect;
		public Brush Brush;
		public RectInfoType Type;
		public RectInfo(Brush brush, Rectangle rect, RectInfoType type) {
			Brush = brush;
			Rect = rect;
			Type = type;
		}
		public RectInfo() : this(null, Rectangle.Empty, RectInfoType.None) { }
	}
}
