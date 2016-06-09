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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Markup;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core.Native {
#endif
	public class ScreenHelper : DependencyObject {
		static readonly double standartDpi = 96d;
		static readonly double currentDpi = new System.Windows.Forms.TextBox().CreateGraphics().DpiX;
		static readonly double scale = standartDpi / currentDpi;
		static readonly double dpiThicknessCorrection;
		public static double CurrentDpi { get { return currentDpi; } }
		static ScreenHelper() {
			dpiThicknessCorrection = ScaleX.Round(false) / ScaleX;
		}
		static double GetScaledValue(double value) {
			return value * scale;
		}
		public static double DpiThicknessCorrection { get { return dpiThicknessCorrection;  } }
#if DEBUGTEST
		public static List<Rect> ScreenRects = new List<Rect>();
		public static Point? ScreenPoint;
#endif
		public static Point GetScaledPoint(Point point) {
			return new Point(GetScaledValue(point.X), GetScaledValue(point.Y));
		}
		protected static Size GetScaledSize(Size size) {
			return new Size(GetScaledValue(size.Width), GetScaledValue(size.Height));
		}
		public static Rect GetScaledRect(Rect rect) {
			return new Rect(GetScaledPoint(rect.TopLeft), GetScaledSize(rect.Size));
		}
		protected static Point PointToScreenCore(Point point, FrameworkElement baseElement) {
			if(BrowserInteropHelper.IsBrowserHosted) {
				return new Point();
			}
			return baseElement.PointToScreen(point);
		}
		public static Point GetScreenPoint(FrameworkElement edit, Point offset = new Point()) {
#if DEBUGTEST
			if(ScreenPoint.HasValue)
				return ScreenPoint.Value;
#endif
			Point point = new Point();
			if(IsAttachedToPresentationSource(edit)) {
				point = GetScaledPoint(PointToScreenCore(offset, edit));
			}
			return point;
		}
		public static bool IsAttachedToPresentationSource(FrameworkElement owner) {
			if(BrowserInteropHelper.IsBrowserHosted)
				return true;
			return owner != null && (PresentationSource.FromDependencyObject(owner) != null);
		}
		public static Rect GetScreenRect(IntPtr handle) {
			var screen = System.Windows.Forms.Screen.FromHandle(handle);
			return screen != null ? GetScreenWorkArea(screen) : new Rect();
		}
		public static Rect GetScreenRect(FrameworkElement edit) {
			return GetScreenRect(GetScreenPoint(edit));
		}
		public static Rect GetScreenRect(Point point) {
			Rect screenRect = new Rect();
			foreach(Rect screen in GetScreenRects()) {
				if(screen.Contains(point))
					screenRect = screen;
			}
			return screenRect;
		}
		public static Rect GetNearestScreenRect(Point point) {
			return point.GetNearestRect(GetScreenRects());
		}
		public static Rect GetScreenWorkArea(FrameworkElement element) {
			Screen screen = GetScreen(element);
			return screen != null ? GetScreenWorkArea(screen) : new Rect();
		}
		public static bool IsOnPrimaryScreen(Point point) {
			Screen screen = GetScreen(point);
			return screen != null ? screen.Primary : false;
		}
		public static bool ContainsPointsOnScreens(params Point[] points) {
			List<Point> listPoints = new List<Point>(points);
			foreach(Rect screen in GetScreenRects()) {
				for(int i = listPoints.Count - 1; i >= 0; i--) {
					if(screen.Contains(listPoints[i]))
						listPoints.RemoveAt(i);
				}
			}
			return (listPoints.Count == 0) ? true : false;
		}
		public static Point GetScreenLocation(Point localLocation, FrameworkElement owner) {
			Point ownerPoint = GetScreenPoint(owner);
			if(owner.FlowDirection == System.Windows.FlowDirection.RightToLeft) {
				return new Point(ownerPoint.X - localLocation.X, localLocation.Y + ownerPoint.Y);
			} else {
				return new Point(localLocation.X + ownerPoint.X, localLocation.Y + ownerPoint.Y);
			}
		}
		public static Point GetClientLocation(Point globalLocation, FrameworkElement owner) {
			Point ownerPoint = GetScreenPoint(owner);
			if(owner.FlowDirection == System.Windows.FlowDirection.RightToLeft) {
				return new Point(ownerPoint.X - globalLocation.X, globalLocation.Y - ownerPoint.Y);
			} else {
				return new Point(globalLocation.X - ownerPoint.X, globalLocation.Y - ownerPoint.Y);
			}
		}
		public static List<Rect> GetScreenRects() {
#if DEBUGTEST
			if(ScreenRects.Count != 0)
				return ScreenRects;
#endif
			List<Rect> listScreens = new List<Rect>();
			foreach(Screen screen in Screen.AllScreens) {
				listScreens.Add(GetScreenRect(screen));
			}
			return listScreens;
		}
		public static Rect GetScreenRectsUnion() {
			Rect resultRect = Rect.Empty;
			foreach(var rect in GetScreenRects()) {
				resultRect.Union(rect);
			}
			return resultRect;
		}
		static Screen GetScreen(FrameworkElement element) {
			Point p = GetScreenPoint(element);
			return GetScreen(p);
		}
		static Screen GetScreen(Point p) {
			foreach(Screen screen in Screen.AllScreens)
				if(GetScreenRect(screen).Contains(p))
					return screen;
			return null;
		}
		static Rect GetScreenRect(Screen screen) {
			return GetScaledRect(new Rect(
					new Point(screen.Bounds.Left, screen.Bounds.Top),
					new Size(screen.Bounds.Width, screen.Bounds.Height)
					));
		}
		static Rect GetScreenWorkArea(Screen screen) {
			return GetScaledRect(new Rect(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height));
		}
		public static Point UpdateContainerLocation(Rect containerRect) {
			if(ScreenHelper.ContainsPointsOnScreens(containerRect.Location,
																		new Point(containerRect.Location.X + containerRect.Width, containerRect.Location.Y),
																		new Point(containerRect.Location.X, containerRect.Location.Y + containerRect.Height),
																		new Point(containerRect.Location.X + containerRect.Width, containerRect.Location.Y + containerRect.Height))) {
				return containerRect.Location;
			}
			List<Rect> rectScreens = ScreenHelper.GetScreenRects();
			double moveXLeft = double.MinValue;
			double moveXRight = double.MinValue;
			double moveYBottom = double.MinValue;
			double moveYTop = double.MinValue;
			GetMoveToRect(containerRect.Location, rectScreens, ref moveXLeft, ref moveYTop);
			GetMoveToRect(new Point(containerRect.Location.X + containerRect.Width, containerRect.Location.Y), rectScreens, ref moveXRight, ref moveYTop);
			GetMoveToRect(new Point(containerRect.Location.X, containerRect.Location.Y + containerRect.Height), rectScreens, ref moveXLeft, ref moveYBottom);
			GetMoveToRect(new Point(containerRect.Location.X + containerRect.Width, containerRect.Location.Y + containerRect.Height), rectScreens, ref moveXRight, ref moveYBottom);
			double moveX = GetMaxValue(moveXLeft, moveXRight);
			double moveY = GetMaxValue(moveYTop, moveYBottom);
			return new Point((moveX != double.MinValue) ? (containerRect.Location.X - moveX) : containerRect.Location.X,
												(moveY != double.MinValue) ? (containerRect.Location.Y - moveY) : containerRect.Location.Y);
		}
		static double GetMaxValue(double firstValue, double secondValue) {
			if(firstValue == double.MinValue)
				return secondValue;
			if(secondValue == double.MinValue)
				return firstValue;
			if(Math.Abs(firstValue) > Math.Abs(secondValue))
				return firstValue;
			return secondValue;
		}
		static double GetMinValue(double firstValue, double secondValue) {
			if(Math.Abs(firstValue) < Math.Abs(secondValue))
				return firstValue;
			return secondValue;
		}
		static void GetMoveToRect(Point point, System.Collections.Generic.List<Rect> rectScreens, ref double minX, ref double minY) {
			bool isContains = false;
			foreach(Rect rect in rectScreens) {
				if(rect.Contains(point))
					isContains = true;
			}
			double localMinX = double.MaxValue;
			double localMinY = double.MaxValue;
			bool xIsInRect = false, yIsInRect = false;
			if(!isContains) {
				foreach(Rect rect in rectScreens) {
					if(point.X < rect.Left || point.X > rect.Right) {
						localMinX = GetMinValue(localMinX, GetMinValue((point.X - rect.Left), (point.X - rect.Right)));
					} else {
						xIsInRect = true;
					}
					if(point.Y < rect.Top || point.Y > rect.Bottom) {
						localMinY = GetMinValue(localMinY, GetMinValue((point.Y - rect.Top), (point.Y - rect.Bottom)));
					} else {
						yIsInRect = true;
					}
				}
			}
			if((localMinX != double.MaxValue) && !xIsInRect)
				minX = localMinX;
			if((localMinY != double.MaxValue) && ((xIsInRect && yIsInRect) || !yIsInRect))
				minY = localMinY;
		}
		public static double GetScreenPadding(DependencyObject obj) {
			return (double)obj.GetValue(ScreenPaddingProperty);
		}
		public static void SetScreenPadding(DependencyObject obj, double value) {
			obj.SetValue(ScreenPaddingProperty, value);
		}
		public static void OnScreenPaddingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			Border border = obj as Border;
			double realValue = (double)e.NewValue / ScaleX;
			border.Padding = new Thickness(realValue);
		}
		public static double GetScreenMargin(DependencyObject obj) {
			return (double)obj.GetValue(ScreenMarginProperty);
		}
		public static void SetScreenMargin(DependencyObject obj, double value) {
			obj.SetValue(ScreenMarginProperty, value);
		}
		public static void OnScreenMarginChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			Border border = obj as Border;
			double realValue = (double)e.NewValue / ScaleX;
			border.Margin = new Thickness(realValue);
		}
		static double scaleXCore = -1;
		public static double ScaleX {
			get {
				if(scaleXCore < 0) {
					PropertyInfo pi = typeof(System.Windows.FrameworkElement).GetProperty("DpiScaleX", BindingFlags.NonPublic | BindingFlags.Static);
					if(pi != null) {
						scaleXCore = (double)pi.GetValue(null, null);
					} else
						scaleXCore = 1.0;
				}
				return scaleXCore;
			}
#if DEBUGTEST
			set { scaleXCore = value; }
#endif
		}
		public static readonly DependencyProperty ScreenPaddingProperty =
		   DependencyProperty.RegisterAttached("ScreenPadding", typeof(double), typeof(ScreenHelper), new PropertyMetadata(0d, OnScreenPaddingChanged));
		public static readonly DependencyProperty ScreenMarginProperty =
			DependencyProperty.RegisterAttached("ScreenMargin", typeof(double), typeof(ScreenHelper), new PropertyMetadata(0d, OnScreenMarginChanged));
	}
	public class ThicknessExtension : MarkupExtension {
		public ThicknessExtension(Thickness origin) { OriginValue = origin; }
		public ThicknessExtension() { }
		public Thickness OriginValue { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var sx = 1d / ScreenHelper.ScaleX;
			return new Thickness(sx * OriginValue.Left, sx * OriginValue.Top, sx * OriginValue.Right, sx * OriginValue.Bottom);
		}
	}
}
