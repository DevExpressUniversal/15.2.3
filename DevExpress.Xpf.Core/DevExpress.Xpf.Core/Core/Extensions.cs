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

using System.Windows.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
#if !DXWINDOW
using DevExpress.Xpf.Core.Native;
#endif
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core.Native {
#endif
#if !DXWINDOW
	public static class WinFormsCompatibilityExtensions {
		public static System.Drawing.Size ToWinFormsSize(this System.Windows.Size size) {
			return new System.Drawing.Size((int)size.Width, (int)size.Height);
		}
		public static System.Windows.Size FromWinForms(this System.Drawing.Size size) {
			return new System.Windows.Size(size.Width, size.Height);
		}
		public static System.Windows.Point FromWinForms(this System.Drawing.Point point) {
			return new System.Windows.Point(point.X, point.Y);
		}
		public static System.Drawing.Rectangle ToWinFormsRectangle(this Rect rect) {
			return new System.Drawing.Rectangle(rect.Location.ToWinFormsPoint(), rect.Size.ToWinFormsSize());
		}
		public static Rect FromWinForms(this System.Drawing.Rectangle rect) {
			return new System.Windows.Rect(rect.Location.FromWinForms(), rect.Size.FromWinForms());
		}
		public static System.Drawing.Point ToWinFormsPoint(this System.Windows.Point point) {
			return new System.Drawing.Point((int)point.X, (int)point.Y);
		}
		public static System.Drawing.Color ToWinFormsColor(this System.Windows.Media.Color color) {
			return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
		}
	}
	public static class DependencyObjectExtensions {
		public static readonly DependencyProperty DataContextProperty =
			DependencyProperty.RegisterAttached("DataContext", typeof(object), typeof(DependencyObjectExtensions), null);
		public static object GetDataContext(DependencyObject obj) {
			return obj.GetValue(DataContextProperty);
		}
		public static void SetDataContext(DependencyObject obj, object value) {
			obj.SetValue(DataContextProperty, value);
		}
		public static bool HasDefaultValue(this DependencyObject o, DependencyProperty property) {
			object value = o.GetValue(property);
			object defaultValue = property.GetMetadata(o.GetType()).DefaultValue;
			return object.Equals(value, defaultValue) || defaultValue == DependencyProperty.UnsetValue && value == null;
		}
		public static bool IsPropertyAssigned(this DependencyObject o, DependencyProperty property) {
			return o.ReadLocalValue(property) != DependencyProperty.UnsetValue;
		}
		public static bool IsPropertySet(this DependencyObject o, DependencyProperty property) {
			BaseValueSource source = System.Windows.DependencyPropertyHelper.GetValueSource(o, property).BaseValueSource;
			return source == BaseValueSource.Style || source == BaseValueSource.StyleTrigger || source == BaseValueSource.ImplicitStyleReference ||
				source == BaseValueSource.TemplateTrigger || source == BaseValueSource.ParentTemplate || source == BaseValueSource.ParentTemplateTrigger ||
				source == BaseValueSource.Local;
		}
		public static void ReadPropertyFromXML(this DependencyObject o, XmlReader xml, DependencyProperty property, string propertyName, Type propertyType) {
			object localValue = o.ReadLocalValue(property);
			if (localValue is BindingExpression) {
				BindingMode mode = ((BindingExpression)localValue).ParentBinding.Mode;
				if (mode == BindingMode.Default) {
					var metadata = property.GetMetadata(o) as FrameworkPropertyMetadata;
					mode = metadata != null && metadata.BindsTwoWayByDefault ? BindingMode.TwoWay : BindingMode.OneWay;
				}
				if (!(mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource))
					return;
			}
			if (xml[propertyName] != null) {
				string s = xml[propertyName];
				object value;
				if (propertyType.IsEnum)
					value = Enum.Parse(propertyType, s, true);
				else
					if (typeof(FrameworkElement).IsAssignableFrom(propertyType))
					value = o is FrameworkElement ? ((FrameworkElement)o).FindName(s) : null;
				else {
					value = null;
					var typeConverterAttributes = propertyType.GetCustomAttributes(typeof(TypeConverterAttribute), true);
					if (typeConverterAttributes.Length > 0) {
						var converterType = Type.GetType(((TypeConverterAttribute)typeConverterAttributes[0]).ConverterTypeName);
						if (converterType != null) {
							var converter = (TypeConverter)converterType.GetConstructor(Type.EmptyTypes).Invoke(null);
							if (converter.CanConvertFrom(typeof(string)))
								if (propertyType == typeof(Thickness))
									value = converter.ConvertFromInvariantString(s);
								else
									value = converter.ConvertFromString(s);
						}
					}
					if (value == null) {
						value = ((IConvertible)s).ToType(propertyType, CultureInfo.InvariantCulture);
						if (value is string && localValue != null && localValue != DependencyProperty.UnsetValue &&
							!(localValue is string) && !(localValue is BindingExpression))
							return;
					}
				}
				o.SetValue(property, value);
			}
			else
				o.ClearValue(property);
		}
		public static void WritePropertyToXML(this DependencyObject o, XmlWriter xml, DependencyProperty property, string propertyName) {
			if (!o.IsPropertyAssigned(property))
				return;
			object value = o.GetValue(property);
			string s;
			if (value == null)
				s = null;
			else
				if (value is double)
				s = ((double)value).ToString(CultureInfo.InvariantCulture);
			else
					if (value is FrameworkElement)
				s = ((FrameworkElement)value).Name;
			else
				s = value.ToString();
			xml.WriteAttributeString(propertyName, s);
		}
		public static object GetCoerceOldValue(this DependencyObject o, DependencyProperty dp) {
			return o.GetValue(dp);
		}
		public static void SetBinding(this FrameworkElement o, DependencyObject bindingSource, string bindingPath, DependencyProperty property) {
			Binding b = new Binding(bindingPath);
			b.Source = bindingSource;
			o.SetBinding(property, b);
		}
		public static void SetCurrentValueIfDefault(this DependencyObject o, DependencyProperty property, object value) {
			if (!o.IsPropertyAssigned(property))
				o.SetCurrentValue(property, value);
		}
		public static void SetValueIfDefault(this DependencyObject o, DependencyProperty property, object value) {
			if (!o.IsPropertyAssigned(property))
				o.SetValue(property, value);
		}
		public static void SetValueIfNotDefault(this DependencyObject o, DependencyProperty property, object value) {
			object defaultValue = property.GetMetadata(o.GetType()).DefaultValue;
			if (value == defaultValue || value == null && defaultValue == DependencyProperty.UnsetValue) {
				if (o.IsPropertyAssigned(property))
					o.ClearValue(property);
			}
			else
				o.SetValue(property, value);
		}
		public static object StorePropertyValue(this DependencyObject o, DependencyProperty property) {
			return new DependencyPropertyValueInfo(o, property);
		}
		public static object StoreAndAssignPropertyValue(this DependencyObject o, DependencyProperty property) {
			var result = o.StorePropertyValue(property);
			o.SetValue(property, o.GetValue(property));
			return result;
		}
		public static void RestorePropertyValue(this DependencyObject o, DependencyProperty property, object storedInfo) {
			((DependencyPropertyValueInfo)storedInfo).RestorePropertyValue(o, property);
		}
		public static UIElement GetElementByName(this DependencyObject owner, string elementName) {
			if (VisualTreeHelper.GetChildrenCount(owner) == 0)
				return null;
			FrameworkElement element = (FrameworkElement)VisualTreeHelper.GetChild(owner, 0);
			return element.FindName(elementName) as UIElement;
		}
		public static T FindElementByTypeInParents<T>(this DependencyObject element, DependencyObject stopElement) where T : FrameworkElement {
			if (element == null || element == stopElement)
				return null;
			if (element is T)
				return element as T;
			return VisualTreeHelper.GetParent(element).FindElementByTypeInParents<T>(stopElement);
		}
		public static bool FindIsInParents(this DependencyObject child, DependencyObject parent) {
			if (child == null)
				return false;
			if (parent == null)
				return false;
			if (VisualTreeHelper.GetParent(child) == null)
				return false;
			if (child == parent)
				return true;
			return FindIsInParents(VisualTreeHelper.GetParent(child), parent);
		}
		public static bool IsInDesignTool(this DependencyObject o) {
			return DesignerProperties.GetIsInDesignMode(o);
		}
		private struct DependencyPropertyValueInfo {
			private object _LocalValue;
			private object _Value;
			public DependencyPropertyValueInfo(DependencyObject o, DependencyProperty property) {
				_LocalValue = o.ReadLocalValue(property);
				_Value = o.GetValue(property);
			}
			public override bool Equals(object obj) {
				if (obj is DependencyPropertyValueInfo) {
					var info = (DependencyPropertyValueInfo)obj;
					return IsAssigned == info.IsAssigned &&
						(_Value == null && info._Value == null || _Value != null && _Value.Equals(info._Value));
				}
				else
					return base.Equals(obj);
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
			public void RestorePropertyValue(DependencyObject o, DependencyProperty property) {
				if (IsAssigned)
					if (_LocalValue is BindingExpressionBase)
						((FrameworkElement)o).SetBinding(property, ((BindingExpressionBase)_LocalValue).ParentBindingBase);
					else
						o.SetValue(property, _LocalValue);
				else {
					o.ClearValue(property);
					o.InvalidateProperty(property);
				}
			}
			private bool IsAssigned { get { return _LocalValue != DependencyProperty.UnsetValue; } }
		}
	}
	public static partial class UIElementExtensions {
		public static Rect InvisibleBounds = new Rect(-1000000, 0, 0, 0);
		public static bool GetVisible(this UIElement element) {
			return element.Visibility == Visibility.Visible;
		}
		public static void SetVisible(this UIElement element, bool visible) {
			Visibility newVisibiliity = visible ? Visibility.Visible : Visibility.Collapsed;
			if (newVisibiliity != element.Visibility) {
				element.Visibility = newVisibiliity;
				INotifyVisibilityChanged notifyVisibilityChanged = element as INotifyVisibilityChanged;
				if (notifyVisibilityChanged != null)
					notifyVisibilityChanged.OnVisibilityChanged();
			}
		}
		public static Size GetDesiredSize(this UIElement element) {
			return element.DesiredSize;
		}
		public static double GetRoundedSize(double size) {
			return Math.Ceiling(size);
		}
		public static Size GetRoundedSize(Size size) {
			return new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
		}
		public static FrameworkElement GetRootVisual(this UIElement element) {
			if (!BrowserInteropHelper.IsBrowserHosted) {
				PresentationSource presentationSource = PresentationSource.FromVisual(element);
				return presentationSource != null ? presentationSource.RootVisual as FrameworkElement : null;
			}
			else
				return (FrameworkElement)Application.Current.MainWindow.Content;
		}
		public static Point MapPoint(this UIElement element, Point p, UIElement destination) {
			if (destination != null)
				return element.TranslatePoint(p, destination);
			else
				if (!BrowserInteropHelper.IsBrowserHosted)
				return element.PointToScreen(p);
			else
				return element.TranslatePoint(p, element.GetRootVisual());
		}
		public static Point MapPointFromScreen(this UIElement element, Point p) {
			if (!BrowserInteropHelper.IsBrowserHosted)
				return element.PointFromScreen(p);
			else
				return element.GetRootVisual().MapPoint(p, element);
		}
		public static Rect MapRect(this UIElement element, Rect rect, UIElement destination) {
			return new Rect(element.MapPoint(rect.TopLeft(), destination), element.MapPoint(rect.BottomRight(), destination));
		}
		public static Rect MapRectFromScreen(this UIElement element, Rect rect) {
			return new Rect(element.MapPointFromScreen(rect.TopLeft()), element.MapPointFromScreen(rect.BottomRight()));
		}
		public static bool HasDefaultRenderTransform(this UIElement element) {
			return element.RenderTransform == null ||
				element.RenderTransform is MatrixTransform && ((MatrixTransform)element.RenderTransform).Matrix.IsIdentity;
		}
		public static bool Focus(this UIElement element) {
			if (element is Control)
				return (element as Control).Focus();
			return false;
		}
		public static void InvalidateParentsOfModifiedChildren(this UIElement element) {
			bool needsRemeasuring = false;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++) {
				var child = VisualTreeHelper.GetChild(element, i) as UIElement;
				if (child != null) {
					InvalidateParentsOfModifiedChildren(child);
					if (!child.IsMeasureValid)
						needsRemeasuring = true;
				}
			}
			if (needsRemeasuring)
				element.InvalidateMeasure();
		}
	}
#endif
	public static class PointExtensions {
		public static double Distance(this Point point, Point otherPoint) {
			return ((Vector)point - (Vector)otherPoint).Length;
		}
		static FrameworkElement GetRootVisual(DependencyObject obj) {
			return LayoutHelper.GetTopLevelVisual(obj);
		}
		public static Point ToRootVisualSafe(this Point pt, object from) {
			try {
				return ToRootVisual(pt, from);
			}
			catch {
				return pt;
			}
		}
		public static Point ToRootVisual(this Point pt, object from) {
			UIElement fromElement = from as UIElement;
			if (fromElement == null)
				return pt;
			GeneralTransform gt = fromElement.TransformToVisual(GetRootVisual(fromElement));
			return gt.Transform(pt);
		}
		public static Point ToLocalSafe(this Point pt, object from) {
			try {
				return ToLocal(pt, from);
			}
			catch {
				return pt;
			}
		}
		public static Point ToLocal(this Point pt, object from) {
			UIElement fromElement = from as UIElement;
			if (fromElement == null)
				return pt;
			GeneralTransform gt = GetRootVisual(fromElement).TransformToVisual(fromElement);
			return gt.Transform(pt);
		}
	}
#if !DXWINDOW
	public static class PointHelper {
		public static bool IsEmpty(Point p) {
			return p == Empty;
		}
		public static Point Abs(Point p) {
			return new Point(Math.Abs(p.X), Math.Abs(p.Y));
		}
		public static Point Add(Point p1, Point p2) {
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}
		public static Point Average(Point p1, Point p2) {
			return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
		}
		public static Point Min(Point p1, Point p2) {
			return new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
		}
		public static Point Max(Point p1, Point p2) {
			return new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
		}
		public static Point Multiply(Point p, double by) {
			return new Point(p.X * by, p.Y * by);
		}
		public static Point Multiply(Point p1, Point p2) {
			return new Point(p1.X * p2.X, p1.Y * p2.Y);
		}
		public static void Offset(ref Point p, double x, double y) {
			p.X += x;
			p.Y += y;
		}
		public static Point Sign(Point p) {
			return new Point(Math.Sign(p.X), Math.Sign(p.Y));
		}
		public static Point Subtract(Point p1, Point p2) {
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}
		public static Point Empty { get { return Rect.Empty.TopLeft(); } }
	}
#endif
	public static class RectHelper {
		public static Rect New(Size size) {
			return new Rect(0, 0, size.Width, size.Height);
		}
		public static Point TopLeft(this Rect rect) {
			return new Point(rect.Left, rect.Top);
		}
		public static Point TopRight(this Rect rect) {
			return new Point(rect.Right, rect.Top);
		}
		public static Point BottomRight(this Rect rect) {
			return new Point(rect.Right, rect.Bottom);
		}
		public static Point BottomLeft(this Rect rect) {
			return new Point(rect.Left, rect.Bottom);
		}
		public static Point Location(this Rect rect) {
			return rect.TopLeft();
		}
		public static Size Size(this Rect rect) {
			if (rect.IsEmpty)
				return System.Windows.Size.Empty;
			else
				return new Size(rect.Width, rect.Height);
		}
		public static void SetLocation(ref Rect rect, Point location) {
			rect.X = location.X;
			rect.Y = location.Y;
		}
		public static void SetSize(ref Rect rect, Size size) {
			rect.Width = size.Width;
			rect.Height = size.Height;
		}
		public static Thickness Padding(Rect outsideRect, Rect insideRect) {
			Thickness result = new Thickness(0);
			result.Left = insideRect.Left - outsideRect.Left;
			result.Top = insideRect.Top - outsideRect.Top;
			result.Right = outsideRect.Right - insideRect.Right;
			result.Bottom = outsideRect.Bottom - insideRect.Bottom;
			return result;
		}
		public static void IncLeft(ref Rect rect, double value) {
			SetLeft(ref rect, rect.Left + value);
		}
		public static void IncTop(ref Rect rect, double value) {
			SetTop(ref rect, rect.Top + value);
		}
		public static void SetLeft(ref Rect rect, double left) {
			if (!double.IsInfinity(rect.Width))
				rect.Width = Math.Max(0, rect.Width - (left - rect.Left));
			rect.X = left;
		}
		public static void SetRight(ref Rect rect, double right) {
			rect.Width = Math.Max(0, right - rect.Left);
		}
		public static void SetTop(ref Rect rect, double top) {
			if (!double.IsInfinity(rect.Height))
				rect.Height = Math.Max(0, rect.Height - (top - rect.Top));
			rect.Y = top;
		}
		public static void SetBottom(ref Rect rect, double bottom) {
			rect.Height = Math.Max(0, bottom - rect.Top);
		}
		public static void Inflate(ref Rect rect, double x, double y) {
			rect.X -= x;
			rect.Y -= y;
			double width = rect.Width + 2 * x;
			double height = rect.Height + 2 * y;
			if (width < 0 || height < 0)
				rect = Rect.Empty;
			else {
				rect.Width = width;
				rect.Height = height;
			}
		}
		public static void Inflate(ref Rect rect, Side side, double value) {
			switch (side) {
				case Side.Left:
					IncLeft(ref rect, -value);
					return;
				case Side.Top:
					IncTop(ref rect, -value);
					return;
				case Side.Right:
					rect.Width += value;
					return;
				case Side.Bottom:
					rect.Height += value;
					return;
			}
		}
		public static void Offset(ref Rect rect, double x, double y) {
			rect.Offset(x, y);
		}
		public static void AlignHorizontally(ref Rect rect, Rect area, HorizontalAlignment alignment) {
			switch (alignment) {
				case HorizontalAlignment.Left:
					rect.X = area.Left;
					break;
				case HorizontalAlignment.Center:
					rect.X = ((area.Left + area.Right - rect.Width) / 2).Round();
					break;
				case HorizontalAlignment.Right:
					rect.X = Math.Floor(area.Right - rect.Width);
					break;
				case HorizontalAlignment.Stretch:
					rect.X = area.Left;
					rect.Width = area.Width;
					break;
			}
		}
		public static void AlignVertically(ref Rect rect, Rect area, VerticalAlignment alignment) {
			switch (alignment) {
				case VerticalAlignment.Top:
					rect.Y = area.Top;
					break;
				case VerticalAlignment.Center:
					rect.Y = ((area.Top + area.Bottom - rect.Height) / 2).Round();
					break;
				case VerticalAlignment.Bottom:
					rect.Y = Math.Floor(area.Bottom - rect.Height);
					break;
				case VerticalAlignment.Stretch:
					rect.Y = area.Top;
					rect.Height = area.Height;
					break;
			}
		}
		public static void Deflate(ref Rect rect, Thickness padding) {
			rect.X += padding.Left;
			rect.Y += padding.Top;
			rect.Width = Math.Max(0, rect.Width - (padding.Left + padding.Right));
			rect.Height = Math.Max(0, rect.Height - (padding.Top + padding.Bottom));
		}
		public static void Inflate(ref Rect rect, Thickness padding) {
			rect.X -= padding.Left;
			rect.Y -= padding.Top;
			rect.Width += padding.Left + padding.Right;
			rect.Height += padding.Top + padding.Bottom;
		}
#if !DXWINDOW
		public static void SnapToDevicePixels(ref Rect rect) {
			rect.Width = UIElementExtensions.GetRoundedSize(rect.Width);
			rect.Height = UIElementExtensions.GetRoundedSize(rect.Height);
		}
#endif
		public static double GetSideOffset(this Rect rect, Side side) {
			switch (side) {
				case Side.Left:
					return rect.Left;
				case Side.Top:
					return rect.Top;
				case Side.Right:
					return rect.Right;
				case Side.Bottom:
					return rect.Bottom;
				default:
					return double.NaN;
			}
		}
		public static void Union(this Rect rect1, Rect rect2) {
			rect1.Union(rect2);
		}
	}
#if !DXWINDOW
	public static class SizeHelper {
		public static bool IsZero(this Size size) {
			return size.Width == 0 && size.Height == 0;
		}
		public static Point ToPoint(this Size size) {
			return new Point(size.Width, size.Height);
		}
		public static Rect ToRect(this Size size) {
			return new Rect(0, 0, size.Width, size.Height);
		}
		public static void Deflate(ref Size size, Thickness padding) {
			size.Width = Math.Max(0, size.Width - (padding.Left + padding.Right));
			size.Height = Math.Max(0, size.Height - (padding.Top + padding.Bottom));
		}
		public static void Inflate(ref Size size, Thickness padding) {
			size.Width = Math.Max(0, size.Width + padding.Left + padding.Right);
			size.Height = Math.Max(0, size.Height + padding.Top + padding.Bottom);
		}
		public static void UpdateMinSize(ref Size minSize, Size size) {
			UpdateMinSize(ref minSize, size, true, true);
		}
		public static void UpdateMinSize(ref Size minSize, Size size, bool updateWidth, bool updateHeight) {
			if (updateWidth)
				minSize.Width = Math.Min(minSize.Width, size.Width);
			if (updateHeight)
				minSize.Height = Math.Min(minSize.Height, size.Height);
		}
		public static void UpdateMaxSize(ref Size maxSize, Size size) {
			UpdateMaxSize(ref maxSize, size, true, true);
		}
		public static void UpdateMaxSize(ref Size maxSize, Size size, bool updateWidth, bool updateHeight) {
			if (updateWidth)
				maxSize.Width = Math.Max(maxSize.Width, size.Width);
			if (updateHeight)
				maxSize.Height = Math.Max(maxSize.Height, size.Height);
		}
		public static Size ToMeasureValid(Size size) {
			return ToMeasureValid(size, true, true);
		}
		public static Size ToMeasureValid(Size size, bool updateWidth, bool updateHeight) {
			return new Size(updateWidth ? ToMeasureValid(size.Width) : size.Width, updateHeight ? ToMeasureValid(size.Height) : size.Height);
		}
		public static bool IsMeasureValid(double length) {
			return !double.IsInfinity(length) && !double.IsNaN(length) && length >= 0;
		}
		static double ToMeasureValid(double length) {
			return IsMeasureValid(length) ? length : 0d;
		}
		public static Size ToInfinity(Size size, bool isInfinityWidth, bool isInfinityHeight) {
			return new Size(isInfinityWidth ? double.PositiveInfinity : size.Width, isInfinityHeight ? double.PositiveInfinity : size.Height);
		}
		public static Size Parse(string s) {
			var result = new Size();
			string[] numbers = s.Split(ThicknessHelper.NumericListSeparator);
			if (numbers != null && numbers.Length == 2) {
				result.Width = double.Parse(numbers[0], CultureInfo.InvariantCulture);
				result.Height = double.Parse(numbers[1], CultureInfo.InvariantCulture);
			}
			return result;
		}
		static SizeHelper() {
			infinite = new Size(double.PositiveInfinity, double.PositiveInfinity);
			zero = new Size(0, 0);
		}
		public static Size Infinite { get { return infinite; } }
		public static Size Zero { get { return zero; } }
		static Size infinite;
		static Size zero;
	}
	public static class ThicknessHelper {
		internal const char NumericListSeparator = ',';
		public static Thickness Multiply(this Thickness thickness, double value) {
			return new Thickness(thickness.Left * value, thickness.Top * value, thickness.Right * value, thickness.Bottom * value);
		}
		public static double GetValue(this Thickness thickness, Side side) {
			switch (side) {
				case Side.Left:
					return thickness.Left;
				case Side.Top:
					return thickness.Top;
				case Side.Right:
					return thickness.Right;
				case Side.Bottom:
					return thickness.Bottom;
				default:
					return double.NaN;
			}
		}
		public static void SetValue(ref Thickness thickness, Side side, double value) {
			switch (side) {
				case Side.Left:
					thickness.Left = value;
					break;
				case Side.Top:
					thickness.Top = value;
					break;
				case Side.Right:
					thickness.Right = value;
					break;
				case Side.Bottom:
					thickness.Bottom = value;
					break;
			}
		}
		public static Thickness ChangeValue(this Thickness thickness, Side side, double value) {
			Thickness result = thickness;
			SetValue(ref result, side, value);
			return result;
		}
		public static void Inc(ref Thickness value, Thickness by) {
			value.Left += by.Left;
			value.Top += by.Top;
			value.Right += by.Right;
			value.Bottom += by.Bottom;
		}
		public static Thickness Parse(string s) {
			var result = new Thickness();
			var numbers = s.Split(NumericListSeparator);
			if (numbers != null)
				switch (numbers.Length) {
					case 1:
						result.Left = result.Top = result.Right = result.Bottom = double.Parse(numbers[0], CultureInfo.InvariantCulture);
						break;
					case 2:
						result.Left = result.Right = double.Parse(numbers[0], CultureInfo.InvariantCulture);
						result.Top = result.Bottom = double.Parse(numbers[1], CultureInfo.InvariantCulture);
						break;
					case 4:
						result.Left = double.Parse(numbers[0], CultureInfo.InvariantCulture);
						result.Top = double.Parse(numbers[1], CultureInfo.InvariantCulture);
						result.Right = double.Parse(numbers[2], CultureInfo.InvariantCulture);
						result.Bottom = double.Parse(numbers[3], CultureInfo.InvariantCulture);
						break;
				}
			return result;
		}
		public static string ToString(Thickness thickness) {
			if (thickness.Left == thickness.Right && thickness.Top == thickness.Bottom)
				if (thickness.Left == thickness.Top)
					return thickness.Left.ToString(CultureInfo.InvariantCulture);
				else
					return thickness.Left.ToString(CultureInfo.InvariantCulture) + NumericListSeparator +
						thickness.Top.ToString(CultureInfo.InvariantCulture);
			else
				return thickness.Left.ToString(CultureInfo.InvariantCulture) + NumericListSeparator +
					thickness.Top.ToString(CultureInfo.InvariantCulture) + NumericListSeparator +
					thickness.Right.ToString(CultureInfo.InvariantCulture) + NumericListSeparator +
					thickness.Bottom.ToString(CultureInfo.InvariantCulture);
		}
	}
	public static class TranslateTransformExtension {
		public static Point GetTranslationPoint(this TranslateTransform transform) {
			return new Point(transform.X, transform.Y);
		}
	}
	public static class StreamExtension {
		public static byte[] GetDataFromStream(this Stream stream) {
			if (stream == null)
				return null;
			byte[] data = new byte[stream.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(data, 0, data.Length);
			return data;
		}
		public static string ReadToString(this Stream stream) {
			string res = "";
			StringBuilder s = new StringBuilder();
			if (stream == null)
				return null;
			try {
				int b;
				do {
					b = stream.ReadByte();
					if (b != -1)
						s.Append((Char)b);
				} while (b != -1);
			}
			finally {
				stream.Dispose();
			}
			res = s.ToString();
			return res;
		}
		public static string ReadString(this Stream stream) {
			if (stream == null)
				return string.Empty;
			long position = stream.Position;
			string str = string.Empty;
			try {
				stream.Position = 0;
				str = new StreamReader(stream).ReadToEnd();
			}
			finally {
				stream.Position = position;
			}
			return str;
		}
	}
	public static class ArrayExtension {
		public static byte[] Slice(this byte[] source, int sourceIndex, int length) {
			byte[] slice = new byte[length];
			Array.Copy(source, sourceIndex, slice, 0, length);
			return slice;
		}
	}
	public static class DictionaryExtension {
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> dictionary) {
			foreach (var item in dictionary) {
				source.Add(item.Key, item.Value);
			}
		}
	}
	public static class AssemblyExtension {
		public static Stream GetEmbeddedResource(this Assembly assembly, string resourceName) {
			byte[] res;
			if (!assembly.GetEmbeddedResource(resourceName, out res))
				return null;
			return new MemoryStream(res);
		}
		public static bool GetEmbeddedResource(this Assembly assembly, string resourceName, out byte[] resource) {
			resource = null;
			string[] manifestResourceNames = assembly.GetManifestResourceNames();
			string baseName = assembly.FullName;
			baseName = baseName.Substring(0, baseName.IndexOf(",")) + ".g";
			if (!manifestResourceNames.Contains(baseName + ".resources"))
				return false;
			ResourceManager manager = new ResourceManager(baseName, assembly);
			UnmanagedMemoryStream stream = manager.GetStream(resourceName);
			if (stream == null)
				return false;
			try {
				resource = new byte[stream.Length];
				stream.Read(resource, 0, (int)stream.Length);
			}
			finally {
				stream.Dispose();
			}
			return true;
		}
		public static ResourceDictionary GetResourceDictionary(this Assembly assembly, string resourceName) {
			Stream stream = assembly.GetEmbeddedResource(resourceName);
			return XamlReader.Load(stream) as ResourceDictionary;
		}
	}
#endif
	public static class DoubleExtensions {
		public static bool InRange(this double value, double min, double max) {
			return value >= min && value <= max;
		}
		public static double ToRange(this double value, double min, double max) {
			return Math.Min(Math.Max(value, min), max);
		}
		public static bool IsZero(this double value) {
			return Math.Abs(value) < 2.2204460492503131E-15;
		}
		public static double Round(this double value, bool toLower = false) {
			double r = value;
			if (toLower)
				r = value > 0 ? Math.Floor(value) : Math.Ceiling(value);
			else
				r = value > 0 ? Math.Ceiling(value) : Math.Floor(value);
			if (AreClose(Math.Abs(r - value), 0.5))
				return r;
			return Math.Round(value);
		}
		public static bool AreClose(this double value1, double value2) {
			if (value1 == value2) {
				return true;
			}
			double num2 = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
			double num = value1 - value2;
			return ((-num2 < num) && (num2 > num));
		}
		public static bool AreClose(Size size1, Size size2) {
			return (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));
		}
		public static bool AreClose(Point point1, Point point2) {
			return (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));
		}
		public static bool GreaterThan(this double value1, double value2) {
			return ((value1 > value2) && !AreClose(value1, value2));
		}
		public static bool LessThan(this double value1, double value2) {
			return ((value1 < value2) && !AreClose(value1, value2));
		}
		public static bool LessThanOrClose(this double value1, double value2) {
			if (value1 >= value2) {
				return AreClose(value1, value2);
			}
			return true;
		}
		public static bool GreaterThanOrClose(this double value1, double value2) {
			if (value1 <= value2) {
				return AreClose(value1, value2);
			}
			return true;
		}
	}
#if !DXWINDOW
	public static class OrientationExtensions {
		public static Orientation OrthogonalValue(this Orientation value) {
			return value == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
		}
	}
	public static class BooleanExtensions {
		public static Visibility ToVisibility(this bool value) {
			return value ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
namespace DevExpress.Xpf.Editors.Helpers {
	public static class ObjectToTrackBarRangeConverter {
		public static TrackBarEditRange TryConvert(object value) {
			if (value is IConvertible) {
				double result = Convert.ToDouble(value);
				return new TrackBarEditRange() { SelectionStart = result, SelectionEnd = result };
			}
			TrackBarEditRange range = value as TrackBarEditRange;
			return range == null ? new TrackBarEditRange() : new TrackBarEditRange() { SelectionStart = range.SelectionStart, SelectionEnd = range.SelectionEnd };
		}
	}
	public static class ObjectToDoubleConverter {
		public static double TryConvertToDouble(this object value) {
			return TryConvert(value);
		}
		public static double TryConvert(object value) {
			double result;
			try {
				result = Convert.ToDouble(value);
			}
			catch {
				result = 0d;
			}
			return result;
		}
	}
	public static class ObjectToDateTimeConverter {
		public static DateTime TryConvertToDateTime(this object value) {
			return TryConvert(value);
		}
		public static DateTime TryConvert(object value) {
			DateTime result;
			try {
				result = Convert.ToDateTime(value);
			}
			catch {
				result = DateTime.Today;
			}
			return result;
		}
	}
	public static class ObjectToDecimalConverter {
		public static decimal TryConvertToDecimal(this object value) {
			return TryConvert(value);
		}
		public static decimal TryConvert(object value) {
			decimal result;
			try {
				result = Convert.ToDecimal(value);
			}
			catch {
				result = 0M;
			}
			return result;
		}
	}
	public static class SetCurrentValueHelper {
		public static void SetCurrentValue(this BaseEdit edit, DependencyProperty property, object value) {
			edit.SetCurrentValue(property, value);
		}
	}
	public static class LinqExtensions {
		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, IEnumerable<T> second) {
			foreach (T t in source) { yield return t; }
			foreach (T t in second) { yield return t; }
		}
		public static bool TrueForEach<T>(this IEnumerable<T> source, Func<T, bool> action) {
			return source.All(action);
		}
		public static bool IsSubsetOf<T>(this IEnumerable<T> a, IEnumerable<T> b) {
			return !a.Except(b).Any();
		}
	}
#endif
}
namespace DevExpress.Xpf.Core {
#if !DXWINDOW
	public static class FrameworkElementExtensions {
#if !DXWINDOW
		public static bool IsLoaded(this FrameworkElement element) {
			return LayoutHelper.IsElementLoaded(element);
		}
		public static bool IsReallyVisible(this FrameworkElement obj) {
			return ((UIElement)obj).GetVisible() && obj.Width != 0 && obj.Height != 0;
		}
		public static void InvalidateMeasureEx(this UIElement obj) {
			IFrameworkElement elem = obj as IFrameworkElement;
			if (elem == null)
				return;
#if DEBUGTEST
			Size res = elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			((FrameworkElement)elem).Width = res.Width;
			((FrameworkElement)elem).Height = res.Height;
#else
			elem.InvalidateMeasureEx();
#endif
		}
		public static void MeasureEx(this UIElement obj, Size availableSize) {
#if DEBUGTEST
			IFrameworkElement elem = obj as IFrameworkElement;
			if (elem == null) {
				if (((FrameworkElement)obj).Parent != null)
					((FrameworkElement)obj).Measure(availableSize);
				return;
			}
			Size res = elem.Measure(availableSize);
			((FrameworkElement)elem).Width = res.Width;
			((FrameworkElement)elem).Height = res.Height;
#else
			obj.Measure(availableSize);
#endif
		}
		public static double GetLeft(this FrameworkElement element) {
			return GetPosition(element).X;
		}
		public static double GetTop(this FrameworkElement element) {
			return GetPosition(element).Y;
		}
		public static void SetLeft(this FrameworkElement element, double value) {
			Canvas.SetLeft(element, value);
		}
		public static void SetTop(this FrameworkElement element, double value) {
			Canvas.SetTop(element, value);
		}
		public static Point GetPosition(this FrameworkElement element) {
			return element.GetPosition(element.GetParent() as FrameworkElement);
		}
		public static Point GetPosition(this FrameworkElement element, FrameworkElement relativeTo) {
			return element.GetBounds(relativeTo).Location();
		}
		public static Size GetSize(this FrameworkElement element) {
			return new Size(element.ActualWidth, element.ActualHeight);
		}
		public static Size GetVisualSize(this FrameworkElement element) {
			var slotSize = LayoutInformation.GetLayoutSlot(element).Size();
			SizeHelper.Deflate(ref slotSize, element.Margin);
			var result = element.GetSize();
			result.Width = Math.Min(result.Width, slotSize.Width);
			result.Height = Math.Min(result.Height, slotSize.Height);
			return result;
		}
		public static void SetSize(this FrameworkElement element, Size value) {
			element.Width = value.Width;
			element.Height = value.Height;
		}
		public static double GetRealWidth(this FrameworkElement element) {
			return Math.Max(element.MinWidth, Math.Min(element.Width, element.MaxWidth));
		}
		public static double GetRealHeight(this FrameworkElement element) {
			return Math.Max(element.MinHeight, Math.Min(element.Height, element.MaxHeight));
		}
		public static Size GetMinSize(this FrameworkElement element) {
			return new Size(element.MinWidth, element.MinHeight);
		}
		public static Size GetMaxSize(this FrameworkElement element) {
			return new Size(element.MaxWidth, element.MaxHeight);
		}
		public static Rect GetBounds(this FrameworkElement element) {
			return element.GetBounds(element.GetParent() as FrameworkElement);
		}
		public static Rect GetBounds(this FrameworkElement element, FrameworkElement relativeTo) {
			return element.MapRect(new Rect(new Point(0, 0), element.GetSize()), relativeTo);
		}
		public static Rect GetVisualBounds(this FrameworkElement element) {
			return element.GetVisualBounds(element.GetParent() as FrameworkElement, false);
		}
		public static Rect GetVisualBounds(this FrameworkElement element, FrameworkElement relativeTo) {
			return GetVisualBounds(element, relativeTo, true);
		}
		public static Rect GetVisualBounds(this FrameworkElement element, FrameworkElement relativeTo, bool checkParentBounds) {
			Rect result = element.MapRect(new Rect(new Point(0, 0), element.GetVisualSize()), relativeTo);
			if (checkParentBounds) {
				FrameworkElement parent = VisualTreeHelper.GetParent(element).FindElementByTypeInParents<FrameworkElement>(null);
				if (parent != null)
					result.Intersect(parent.GetVisualBounds(relativeTo));
			}
			return result;
		}
		public static void SetBounds(this FrameworkElement element, Rect bounds) {
			if (!bounds.IsEmpty) {
				element.SetLeft(bounds.Left);
				element.SetTop(bounds.Top);
			}
			element.Width = bounds.Width;
			element.Height = bounds.Height;
		}
		public static bool IsVisible(this FrameworkElement element) {
			return element.IsVisible;
		}
		public static int GetZIndex(this FrameworkElement element) {
			return Canvas.GetZIndex(element);
		}
		public static void SetZIndex(this FrameworkElement element, int value) {
			Canvas.SetZIndex(element, value);
		}
#endif
		public static FrameworkElement GetRootParent(this FrameworkElement element) {
			FrameworkElement result;
			DependencyObject parent = element;
			do {
				result = (FrameworkElement)parent;
				parent = result.GetVisualParent();
			}
			while (parent != null);
			return result;
		}
		public static DependencyObject GetTemplatedParent(this FrameworkElement element) {
			return element.TemplatedParent;
		}
		public static FrameworkElement GetVisualParent(this FrameworkElement element) {
			DependencyObject parent = VisualTreeHelper.GetParent(element);
			while (parent != null && !(parent is FrameworkElement)) {
				parent = VisualTreeHelper.GetParent(parent);
			}
			if (parent == null && element.Parent is Popup)
				parent = (FrameworkElement)element.Parent;
			return parent != null ? (FrameworkElement)parent : null;
		}
		public static bool IsInVisualTree(this FrameworkElement element) {
			if (IsFullyTrusted)
				return PresentationSource.FromVisual(element) != null;
			else
				return element.GetRootParent() == Application.Current.MainWindow;
		}
		static bool IsFullyTrusted {
			get {
#if DXWINDOW
				return true;
#else
				return AppDomain.CurrentDomain.IsFullyTrusted;
#endif
			}
		}
		public static void RemoveFromVisualTree(this FrameworkElement obj) {
			DependencyObject parent = obj.GetParent();
			if (parent == null)
				return;
			ContentPresenter presenterParent = parent as ContentPresenter;
			if (presenterParent != null) {
				RemoveFromVisualTree(presenterParent);
			}
			ContentControl controlParent = parent as ContentControl;
			if (controlParent != null) {
				controlParent.Content = null;
			}
			Panel panelParent = parent as Panel;
			if (panelParent != null) {
				panelParent.Children.Remove(obj);
			}
		}
		public static DependencyObject GetParent(this FrameworkElement element) {
			DependencyObject result = element.Parent;
			if (result == null)
				result = VisualTreeHelper.GetParent(element);
			return result;
		}
#if !DXWINDOW
		public static void SetParent(this FrameworkElement element, DependencyObject value) {
			if (element.Parent == value)
				return;
			DependencyObject parent = element.Parent;
			if (parent is Panel)
				((Panel)parent).Children.Remove(element);
			else if (parent is Border)
				((Border)parent).Child = null;
			if (value is Panel)
				((Panel)value).Children.Add(element);
			else if (value is Border)
				((Border)value).Child = element;
		}
		public static void ClipToBounds(this FrameworkElement element) {
			element.Clip = new RectangleGeometry { Rect = RectHelper.New(element.GetSize()) };
		}
		public static bool GetIsClipped(this FrameworkElement element) {
			return FrameworkElementHelper.GetIsClipped(element);
		}
		public static void SetIsClipped(this FrameworkElement element, bool value) {
			FrameworkElementHelper.SetIsClipped(element, value);
		}
		public static bool Contains(this FrameworkElement element, Point absolutePosition) {
			DependencyObject firstHit = null;
			VisualTreeHelper.HitTest(element,
				delegate (DependencyObject hitElement) {
					firstHit = hitElement;
					return HitTestFilterBehavior.Stop;
				},
				(hitTestResult) => HitTestResultBehavior.Stop,
				new PointHitTestParameters(element.MapPointFromScreen(absolutePosition)));
			return firstHit == element;
		}
		public static UIElement FindElement(this FrameworkElement element, Point absolutePosition, Func<UIElement, bool> condition) {
			UIElement result = null;
			VisualTreeHelper.HitTest(element,
				delegate (DependencyObject hitElement) {
					result = hitElement as UIElement;
					if (result == null || !condition(result))
						return HitTestFilterBehavior.Continue;
					else
						return HitTestFilterBehavior.Stop;
				},
				(hitTestResult) => HitTestResultBehavior.Continue,
				new PointHitTestParameters(element.MapPointFromScreen(absolutePosition)));
			return result;
		}
		public static object GetToolTip(this FrameworkElement element) {
			return FrameworkElementHelper.GetToolTip(element);
		}
		public static void SetToolTip(this FrameworkElement element, object toolTip) {
			FrameworkElementHelper.SetToolTip(element, toolTip);
		}
		public static void ApplyStyleValuesToPropertiesWithLocalValues(this FrameworkElement element) {
			if (element.Style == null)
				return;
			foreach (Setter setter in element.Style.Setters)
				if (element.IsPropertyAssigned(setter.Property))
					element.SetValue(setter.Property, setter.Value);
		}
#endif
	}
	public static class ContentPresenterExtensions {
		public static FrameworkElement GetUIElement(this ContentPresenter presenter) {
			Debug.Assert(VisualTreeHelper.GetChildrenCount(presenter) != 0);
			return VisualTreeHelper.GetChild(presenter, 0) as FrameworkElement;
		}
	}
	public static class BorderExtensions {
		public static readonly DependencyProperty ClipChildProperty =
			DependencyProperty.RegisterAttached("ClipChild", typeof(bool), typeof(BorderExtensions), new PropertyMetadata(OnClipChildChanged));
		public static bool GetClipChild(Border border) {
			return (bool)border.GetValue(ClipChildProperty);
		}
		public static void SetClipChild(Border border, bool value) {
			border.SetValue(ClipChildProperty, value);
		}
		public static void ClipChild(this Border border) {
			if (border.Child != null)
				border.Child.Clip = border.GetChildClip();
		}
		public static Geometry GetChildClip(this Border border) {
			if (border.Child == null || !border.IsInVisualTree())
				return null;
			var rect = RectHelper.New(border.GetSize());
			RectHelper.Deflate(ref rect, border.BorderThickness);
			if (border.Child.GetVisible())
				rect = border.MapRect(rect, (FrameworkElement)border.Child);
			else
				rect.X = rect.Y = 0;
			var cornerRadius = border.CornerRadius;
			var borderThickness = border.BorderThickness;
			var corner = new Size[]
			{
				new Size(Math.Max(0, cornerRadius.TopLeft - borderThickness.Left / 2), Math.Max(0, cornerRadius.TopLeft - borderThickness.Top / 2)),
				new Size(Math.Max(0, cornerRadius.TopRight - borderThickness.Right / 2), Math.Max(0, cornerRadius.TopRight - borderThickness.Top / 2)),
				new Size(Math.Max(0, cornerRadius.BottomRight - borderThickness.Right / 2), Math.Max(0, cornerRadius.BottomRight - borderThickness.Bottom / 2)),
				new Size(Math.Max(0, cornerRadius.BottomLeft - borderThickness.Left / 2), Math.Max(0, cornerRadius.BottomLeft - borderThickness.Bottom / 2))
			};
			var figure = new PathFigure { IsClosed = true };
			figure.StartPoint = new Point(rect.Left, rect.Top + corner[0].Height);
			figure.Segments.Add(new ArcSegment
			{
				Point = new Point(rect.Left + corner[0].Width, rect.Top),
				Size = corner[0],
				SweepDirection = SweepDirection.Clockwise
			});
			figure.Segments.Add(new LineSegment { Point = new Point(rect.Right - corner[1].Width, rect.Top) });
			figure.Segments.Add(new ArcSegment
			{
				Point = new Point(rect.Right, rect.Top + corner[1].Height),
				Size = corner[1],
				SweepDirection = SweepDirection.Clockwise
			});
			figure.Segments.Add(new LineSegment { Point = new Point(rect.Right, rect.Bottom - corner[2].Height) });
			figure.Segments.Add(new ArcSegment
			{
				Point = new Point(rect.Right - corner[2].Width, rect.Bottom),
				Size = corner[2],
				SweepDirection = SweepDirection.Clockwise
			});
			figure.Segments.Add(new LineSegment { Point = new Point(rect.Left + corner[3].Width, rect.Bottom) });
			figure.Segments.Add(new ArcSegment
			{
				Point = new Point(rect.Left, rect.Bottom - corner[3].Height),
				Size = corner[3],
				SweepDirection = SweepDirection.Clockwise
			});
			var result = new PathGeometry();
			result.Figures.Add(figure);
			return result;
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		private static DependencyProperty BorderThicknessListener =
			DependencyProperty.RegisterAttached("BorderThicknessListener", typeof(Thickness), typeof(BorderExtensions),
				new PropertyMetadata(OnBorderPropertyChanged));
		[IgnoreDependencyPropertiesConsistencyChecker]
		private static DependencyProperty CornerRadiusListener =
			DependencyProperty.RegisterAttached("CornerRadiusListener", typeof(CornerRadius), typeof(BorderExtensions),
				new PropertyMetadata(OnBorderPropertyChanged));
		private static void OnBorderPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			var border = (Border)o;
			if (border.IsInVisualTree()) {
				if (e.Property == CornerRadiusListener)
					border.ClipChild();
				else {
					if (border.Child != null) {
						var childSlot = new Rect(0, 0, border.ActualWidth, border.ActualHeight);
						RectHelper.Deflate(ref childSlot, border.BorderThickness);
						RectHelper.Deflate(ref childSlot, border.Padding);
						if (LayoutInformation.GetLayoutSlot((FrameworkElement)border.Child) == childSlot) {
							border.ClipChild();
							return;
						}
					}
					EventHandler onLayoutUpdated = null;
					onLayoutUpdated = delegate {
						border.LayoutUpdated -= onLayoutUpdated;
						border.ClipChild();
					};
					border.LayoutUpdated += onLayoutUpdated;
				}
			}
		}
		private static void OnBorderSizeChanged(object sender, SizeChangedEventArgs e) {
			((Border)sender).ClipChild();
		}
		private static void OnClipChildChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			var border = o as Border;
			if (border == null)
				return;
			if ((bool)e.NewValue) {
				border.SetBinding(BorderThicknessListener, new Binding("BorderThickness") { Source = border });
				border.SetBinding(CornerRadiusListener, new Binding("CornerRadius") { Source = border });
				border.SizeChanged += OnBorderSizeChanged;
			}
			else {
				border.ClearValue(BorderThicknessListener);
				border.ClearValue(CornerRadiusListener);
				border.SizeChanged -= OnBorderSizeChanged;
			}
		}
	}
	public static class PopupExtensions {
		public static void MakeVisible(this Popup popup) {
			popup.MakeVisible(null, null);
		}
		public static void MakeVisible(this Popup popup, Point? offset, Rect? clearZone) {
			var placementTarget = popup.GetParent() as UIElement;
			if (popup.PlacementTarget != null)
				placementTarget = popup.PlacementTarget;
			Point popupOffset = MakeVisible(placementTarget, offset ?? popup.GetOffset(), clearZone, popup.Child);
			popup.SetOffset(popupOffset);
		}
		public static Point MakeVisible(UIElement placementTarget, Point popupOffset, Rect? clearZone, UIElement popupChild) {
			if (popupChild.DesiredSize.IsZero())
				popupChild.Measure(SizeHelper.Infinite);
			Size popupSize = popupChild.DesiredSize;
			Rect workArea = GetScreenWorkArea(placementTarget, popupChild);
			if (placementTarget != null)
				workArea = placementTarget.MapRectFromScreen(workArea);
			bool isRTL =
				popupChild is FrameworkElement && ((FrameworkElement)popupChild).FlowDirection == FlowDirection.RightToLeft &&
				(placementTarget == null || placementTarget is FrameworkElement && ((FrameworkElement)placementTarget).FlowDirection == FlowDirection.LeftToRight);
			if (isRTL)
				popupOffset.X -= popupSize.Width;
			if (popupOffset.X + popupSize.Width > workArea.Right)
				if (clearZone != null && popupOffset.Y < clearZone.Value.Bottom && popupOffset.Y + popupSize.Height > clearZone.Value.Top)
					popupOffset.X -= 2 * (popupOffset.X - clearZone.Value.Right) + clearZone.Value.Width + popupSize.Width;
				else
					popupOffset.X = workArea.Right - popupSize.Width;
			popupOffset.X = Math.Max(workArea.Left, popupOffset.X);
			if (isRTL)
				popupOffset.X += popupSize.Width;
			if (popupOffset.Y + popupSize.Height > workArea.Bottom)
				if (clearZone != null && popupOffset.X < clearZone.Value.Right && popupOffset.X + popupSize.Width > clearZone.Value.Left)
					popupOffset.Y -= 2 * (popupOffset.Y - clearZone.Value.Bottom) + clearZone.Value.Height + popupSize.Height;
				else
					popupOffset.Y = workArea.Bottom - popupSize.Height;
			popupOffset.Y = Math.Max(workArea.Top, popupOffset.Y);
			return popupOffset;
		}
		public static Point GetOffset(this Popup popup) {
			return new Point(popup.HorizontalOffset, popup.VerticalOffset);
		}
		public static void SetOffset(this Popup popup, Point offset) {
			if (popup.HorizontalOffset != offset.X)
				popup.HorizontalOffset = offset.X;
			if (popup.VerticalOffset != offset.Y)
				popup.VerticalOffset = offset.Y;
		}
		public static void BringToFront(this Popup popup) {
			popup.IsOpen = false;
			popup.IsOpen = true;
		}
		private static Rect GetScreenWorkArea(UIElement placementTarget, UIElement popupChild) {
			if (!BrowserInteropHelper.IsBrowserHosted)
				if (placementTarget != null)
					return ScreenHelper.GetScreenWorkArea((FrameworkElement)placementTarget);
				else
					return SystemParameters.WorkArea;
			else
				return new Rect(popupChild.GetRootVisual().GetSize());
		}
	}
#endif
}
