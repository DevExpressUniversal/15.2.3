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
using System.Text;
using System.Linq;
using System.Windows;
using System.Security;
using System.Collections;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Windows.Interop;
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using System.Windows.Input;
using DevExpress.Xpf.Office.Internal;
using System.Reflection;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.UI.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.Xpf.Scheduler.Native {
	public class AppointmentImagesNames {
		public const string ResourcePath = "DevExpress.Xpf.Scheduler.Images";
		public const string Reminder = "Reminder";
		public const string ChangedRecurrence = "ChangedRecurrence";
		public const string Recurrence = "Recurrence";
		public const string AppointmentStartContinueArrow = "AppointmentStartContinueArrow";
		public const string AppointmentEndContinueArrow = "AppointmentEndContinueArrow";
	}
	public static class XpfSchedulerUtils {
		public static readonly Point ZeroPoint = new Point(0, 0);
		public static bool IsTodayDate(TimeZoneHelper timeZoneEngine, DateTime date) {
			DateTime clientNow = timeZoneEngine.ToClientTime(DateTime.Now, TimeZoneInfo.Local.Id);
			return clientNow.Date == date;
		}
		public static bool IsZeroRect(Rect rect) {
			return rect.X == 0 && rect.Y == 0 && rect.Width == 0 && rect.Height == 0;
		}
		public static Size ValidateInfinitySize(IEnumerable items, Size availableSize) {
			if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
				return CalculateAvailableSize(items, availableSize);
			return availableSize;
		}
		static Size CalculateAvailableSize(IEnumerable items, Size availableSize) {
			Size result = new Size();
			foreach (UIElement elem in items) {
				elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				result.Width = Math.Max(result.Width, elem.DesiredSize.Width);
				result.Height = Math.Max(result.Height, elem.DesiredSize.Height);
			}
			if (!double.IsInfinity(availableSize.Width))
				result.Width = availableSize.Width;
			if (!double.IsInfinity(availableSize.Height))
				result.Height = availableSize.Height;
			return result;
		}
	}
	public static class SchedulerSizeHelper {
		public static Size Union(Size size1, Size size2, Orientation orientation) {
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(orientation);
			double primarySize = sizeHelper.GetDefineSize(size1) + sizeHelper.GetDefineSize(size2);
			double secondarySize = Math.Max(sizeHelper.GetSecondarySize(size1), sizeHelper.GetSecondarySize(size2));
			return sizeHelper.CreateSize(primarySize, secondarySize);
		}
		public static double[] GetPrimarySizes(Rect[] rects, Orientation orientation) {
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(orientation);
			double[] result = new double[rects.Length];
			for (int i = 0; i < rects.Length; i++)
				result[i] = sizeHelper.GetDefineSize(rects[i].Size());
			return result;
		}
		public static double GetPrimarySize(Rect rect, Orientation orientation) {
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(orientation);
			return sizeHelper.GetDefineSize(rect.Size());
		}
		public static bool AreClose(Size size1, Size size2) {
			return DoubleUtil.AreClose(size1.Width, size2.Width) && DoubleUtil.AreClose(size1.Height, size2.Height);
		}
		public static double CalclActualLength(double availableLength, double desiredLength) {
			return availableLength < desiredLength ? availableLength : desiredLength;
		}
	}
	public static class SchedulerRectHelper {
		public static Rect Offset(Rect rect, Point offset) {
			rect.X += offset.X;
			rect.Y += offset.Y;
			return rect;
		}
	}
	public class InnerBindingHelper {
		internal static Binding CreateBindingCore(object source, string propPath, BindingMode mode) {
			Binding binding = new Binding(propPath);
			binding.Source = source;
			binding.Mode = mode;
			return binding;
		}
		internal static void SetBinding(DependencyObject target, object source, DependencyProperty targetDependencyProperty, string sourcePropertyName) {
			Binding binding = new Binding();
			binding.Source = source;
			binding.Mode = BindingMode.OneWay;
			binding.Path = new PropertyPath(sourcePropertyName);
			BindingOperations.SetBinding(target, targetDependencyProperty, binding);
		}
		internal static void ClearBinding(DependencyObject target, DependencyProperty targetDependencyProperty) {
			target.ClearValue(targetDependencyProperty);
		}
		public static Binding CreateTwoWayPropertyBinding(object source, string propPath) {
			return CreateBindingCore(source, propPath, BindingMode.TwoWay);
		}
		public static Binding CreateOneWayPropertyBinding(object source) {
			return CreateOneWayPropertyBinding(source, null);
		}
		public static Binding CreateOneWayPropertyBinding(object source, string propPath) {
			return CreateBindingCore(source, propPath, BindingMode.OneWay);
		}
		public static void SetupDataContextBinding(FrameworkElement owner, FrameworkElement innerObject) {
			if (innerObject != null) {
#if SL
				innerObject.SetValue(FrameworkElement.DataContextProperty, null);
#else
				BindingOperations.ClearBinding(innerObject, FrameworkElement.DataContextProperty);
#endif
			}
			if (owner != null && innerObject != null)
				innerObject.SetBinding(FrameworkElement.DataContextProperty, CreateOneWayPropertyBinding(owner, "DataContext"));
		}
	}
	public static class BrushHelper {
		public static Brush CreateSolidColorBrush(Color color) {
			return new SolidColorBrush(color);
		}
#if SL
#else
		public static Brush CreateHatchBrush(Color color, double thickness) {
			Rect viewportBounds = new Rect(0, 0, 16, 16);
			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(CreateLineGeomerty(0, 6, 6, 0));
			geometryGroup.Children.Add(CreateLineGeomerty(0, 14, 14, 0));
			geometryGroup.Children.Add(CreateLineGeomerty(4, 18, 18, 4));
			geometryGroup.Children.Add(CreateLineGeomerty(12, 18, 18, 12));
			Path path = new Path();
			path.StrokeThickness = thickness;
			path.Stroke = CreateSolidColorBrush(color);
			path.Data = geometryGroup;
			Border border = new Border();
			border.Background = Brushes.White;
			border.Child = path;
			VisualBrush brush = new VisualBrush();
			brush.Visual = border;
			brush.Stretch = Stretch.None;
			brush.ViewportUnits = BrushMappingMode.Absolute;
			brush.Viewport = viewportBounds;
			brush.TileMode = TileMode.Tile;
			return brush;
		}
		public static Brush CreateHatchBrushPercent75(Color color, double thickness) {
			Rect viewportBounds = new Rect(0, 0, 8, 8);
			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(CreateLineGeomerty(1, 1, 2, 1));
			geometryGroup.Children.Add(CreateLineGeomerty(3, 3, 4, 3));
			geometryGroup.Children.Add(CreateLineGeomerty(1, 5, 2, 5));
			geometryGroup.Children.Add(CreateLineGeomerty(5, 1, 6, 1));
			geometryGroup.Children.Add(CreateLineGeomerty(5, 5, 6, 5));
			geometryGroup.Children.Add(CreateLineGeomerty(3, 7, 4, 7));
			geometryGroup.Children.Add(CreateLineGeomerty(7, 3, 8, 3));
			geometryGroup.Children.Add(CreateLineGeomerty(7, 3, 8, 3));
			geometryGroup.Children.Add(CreateLineGeomerty(7, 7, 8, 7));
			Path path = new Path();
			path.StrokeThickness = thickness;
			path.Stroke = CreateSolidColorBrush(color);
			path.Data = geometryGroup;
			Border border = new Border();
			border.Background = Brushes.White;
			border.SnapsToDevicePixels = true;
			border.UseLayoutRounding = false;
			border.SetSize(viewportBounds.Size);
			border.Child = path;
			VisualBrush brush = new VisualBrush();
			brush.Visual = border;
			brush.Stretch = Stretch.None;
			brush.ViewportUnits = BrushMappingMode.Absolute;
			brush.Viewport = viewportBounds;
			brush.TileMode = TileMode.Tile;
			return brush;
		}
		static LineGeometry CreateLineGeomerty(double x1, double y1, double x2, double y2) {
			return new LineGeometry(new Point(x1, y1), new Point(x2, y2));
		}
#endif
	}
#if !SL
	public static class XpfSchedulerImageHelper {
		public static BitmapSource CreateBitmapSource(Brush fillBrush, Brush borderBrush, int width, int height) {
			DrawingVisual visual = new DrawingVisual();
			DrawingContext context = visual.RenderOpen();
			try {
				context.DrawRectangle(fillBrush, new Pen(borderBrush, 1), new Rect(0, 0, width, height));
			} finally {
				context.Close();
			}
			RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
			bmp.Render(visual);
			return bmp;
		}
		[SecuritySafeCritical]
		public static System.Drawing.Bitmap ConvertToGdiBitmap(BitmapSource source) {
			BmpBitmapEncoder encoder = new BmpBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(source));
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
				encoder.Save(ms);
				return (System.Drawing.Bitmap)System.Drawing.Image.FromStream(ms);
			}
		}
		[SecuritySafeCritical]
		public static BitmapSource ConvertToBitmapSource(System.Drawing.Bitmap bitmap) {
			return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
		}
	}
#else
	public static class XpfSchedulerImageHelper {
		public static BitmapSource CreateBitmapSource(Brush fillBrush, Brush borderBrush, int width, int height) {
			Rectangle rectangle = new Rectangle();
			rectangle.Width = width;
			rectangle.Height = height;
			rectangle.Fill = fillBrush;
			rectangle.Stroke = borderBrush;
			WriteableBitmap wb = new WriteableBitmap(width, height);
			wb.Render(rectangle, new TranslateTransform());
			wb.Invalidate();
			return wb;
		}
		public static BitmapSource ConvertToBitmapSource(Image bitmap) {
			return bitmap.Source as BitmapSource;
		}
	}
#endif
	public static class ScrollViewerHelper {
		public static TimeOfDayInterval CalcTimeOffset(ScrollViewer viewer, TimeOfDayInterval what) {
			double totalHeight = viewer.ExtentHeight;
			double visibleEnd = viewer.VerticalOffset + viewer.ViewportHeight;
			if (totalHeight <= visibleEnd && viewer.VerticalOffset == 0)
				return what.Clone();
			double visibleStart = viewer.VerticalOffset;
			long totalTicks = what.End.Ticks - what.Start.Ticks;
			long oneTick = totalTicks / (long)totalHeight;
			return new TimeOfDayInterval(new TimeSpan(oneTick * (int)visibleStart + what.Start.Ticks), new TimeSpan(oneTick * (int)visibleEnd + what.Start.Ticks));
		}
	}
	public class SchedulerRectUtils {
		public static double CalcDateY(TimeSpan time, TimeOfDayInterval interval, Size size) {
			if (!interval.Contains(time))
				return -1;
			double offsetRatio = (time - interval.Start).Ticks / (double)interval.Duration.Ticks;
			double offset = size.Height * offsetRatio;
			return Math.Round(offset);
		}
	}
	internal static class SchedulerLayoutHelper {
		public static void InvalidateParentLayoutCache(FrameworkElement element) {
			int actualColumn = Grid.GetColumn(element);
			Grid.SetColumn(element, actualColumn + 1);
			Grid.SetColumn(element, actualColumn);
		}
	}
	public static class SchedulerExtensions {
		public static bool IsNeverMeasured(this UIElement element) {
			return element.DesiredSize == new Size(0, 0);
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Internal {
	#region LoadedUnloadedSubscriber
	public delegate void SubscribtionEventDelegate(FrameworkElement element);
	public class LoadedUnloadedSubscriber : FrameworkElement {
		readonly SubscribtionEventDelegate onSubscribe;
		readonly SubscribtionEventDelegate onUnSubscribe;
		readonly FrameworkElement element;
		public LoadedUnloadedSubscriber(FrameworkElement element, SubscribtionEventDelegate subscribeEventsDelegate, SubscribtionEventDelegate unsubscribeEventsDelegate) {
			this.element = element;
			element.Loaded -= new RoutedEventHandler(OnLoaded);
			element.Loaded += new RoutedEventHandler(OnLoaded);
			this.onSubscribe = subscribeEventsDelegate;
			this.onUnSubscribe = unsubscribeEventsDelegate;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			FrameworkElement element = (FrameworkElement)sender;
			element.Loaded -= new RoutedEventHandler(OnLoaded);
			element.Loaded -= new RoutedEventHandler(OnLoaded);
			element.Unloaded -= new RoutedEventHandler(OnUnloaded);
			element.Unloaded += new RoutedEventHandler(OnUnloaded);
			onSubscribe(element);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			FrameworkElement element = (FrameworkElement)sender;
			element.Unloaded -= new RoutedEventHandler(OnUnloaded);
			element.Loaded -= new RoutedEventHandler(OnLoaded); 
			element.Loaded += new RoutedEventHandler(OnLoaded);
			onUnSubscribe(element);
		}
	}
	#endregion
	#region IAppointmentsInfoChangedListener<T>
	public interface IAppointmentsInfoChangedListener<T> {
		void OnChanged(List<T> oldInfos, List<T> newInfos);
	}
	#endregion
	#region AppointmentsInfoChangedNotifier<T>
	public class AppointmentsInfoChangedNotifier<T> {
		readonly List<WeakReference> listeners = new List<WeakReference>();
		public void RegisterListener(IAppointmentsInfoChangedListener<T> listener) {
			if (!Contains(listener))
				listeners.Add(new WeakReference(listener));
		}
		public void RegisterListeners(DependencyObject child) {
			while (child != null) {
				DependencyObject parent = VisualTreeHelper.GetParent(child);
				IAppointmentsInfoChangedListener<T> listener = parent as IAppointmentsInfoChangedListener<T>;
				if (listener != null)
					RegisterListener(listener);
				child = parent;
			}
		}
		public void UnregisterListener(IAppointmentsInfoChangedListener<T> listener) {
			int index = IndexOf(listener);
			if (index >= 0)
				listeners.RemoveAt(index);
		}
		public void UnregisterListeners() {
			listeners.Clear();
		}
		public bool Contains(IAppointmentsInfoChangedListener<T> listener) {
			return IndexOf(listener) >= 0;
		}
		int IndexOf(IAppointmentsInfoChangedListener<T> listener) {
			for (int i = 0; i < listeners.Count; i++) {
				if (Object.ReferenceEquals(listener, GetListener(listeners[i])))
					return i;
			}
			return -1;
		}
		public void NotifyAppointmentsChanged(List<T> oldInfos, List<T> newInfos) {
			for (int i = listeners.Count - 1; i >= 0; i--) {
				IAppointmentsInfoChangedListener<T> listener = GetListener(listeners[i]);
				if (listener != null)
					listener.OnChanged(oldInfos, newInfos);
				else
					listeners.RemoveAt(i);
			}
		}
		IAppointmentsInfoChangedListener<T> GetListener(WeakReference reference) {
			IAppointmentsInfoChangedListener<T> listener = reference.Target as IAppointmentsInfoChangedListener<T>;
			return listener != null ? listener : default(IAppointmentsInfoChangedListener<T>);
		}
	}
	#endregion
	public static class WeakReferenceRepository {
		readonly static List<WeakReference> innerList = new List<WeakReference>();
		public static void AddObject(object listener) {
			innerList.Add(new WeakReference(listener));
		}
		public static void RemoveObject(object listener) {
			for (int i = innerList.Count - 1; i >= 0; i--) {
				if (Object.ReferenceEquals(innerList[i].Target, listener))
					innerList.RemoveAt(i);
			}
		}
		public static void DoAction<T>(Action<T> action) {
			for (int i = innerList.Count - 1; i >= 0; i--) {
				object target = innerList[i].Target;
				if (target != null) {
					if (target is T)
						action((T)target);
				} else
					innerList.RemoveAt(i);
			}
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Xpf.Scheduler {
	public class FrameworkElementInfo {
		FrameworkElement element;
		public FrameworkElementInfo(FrameworkElement element) {
			this.element = element;
			SubscribeEvents(this.element);
		}
		public FrameworkElement Element { get { return element; } }
		void OnUnloaded(object sender, RoutedEventArgs e) {
			FrameworkElement element = (FrameworkElement)sender;
			LogShort("Unloaded", element);
			UnsubscribeEvents(element);
			this.element = null;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			LogShort("Loaded", (FrameworkElement)sender);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			LogFull(element as FrameworkElement);
		}
		private void SubscribeEvents(FrameworkElement fe) {
			fe.LayoutUpdated += new EventHandler(OnLayoutUpdated);
			fe.Loaded += new RoutedEventHandler(OnLoaded);
			fe.Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		void UnsubscribeEvents(FrameworkElement fe) {
			fe.LayoutUpdated -= new EventHandler(OnLayoutUpdated);
			fe.Loaded -= new RoutedEventHandler(OnLoaded);
			fe.Unloaded -= new RoutedEventHandler(OnUnloaded);
		}
		void LogShort(string action, FrameworkElement fe) {
			System.Diagnostics.Debug.WriteLine(String.Format("{0} - {1}", action, GetElementId(fe)));
		}
		string GetElementId(FrameworkElement fe) {
			StringBuilder sb = new StringBuilder();
			sb.Append(this.element.GetType().Name);
			if (!String.IsNullOrEmpty(this.element.Name))
				sb.AppendFormat(" ({0})", this.element.Name);
			return sb.ToString();
		}
		void LogFull(FrameworkElement element) {
			StringBuilder sb = new StringBuilder();
			sb.Append(this.element.GetType().Name);
			if (!String.IsNullOrEmpty(this.element.Name))
				sb.AppendFormat(" ({0})", this.element.Name);
			sb.Append(" - ");
			sb.AppendFormat("margin: ({0})", this.element.Margin);
			TextBlock tb = element as TextBlock;
			if (tb != null)
				sb.AppendFormat(", padding: {0}", tb.Padding);
			System.Diagnostics.Debug.WriteLine(sb.ToString());
		}
	}
	public class DebuggerHelper : DependencyObject {
		static List<FrameworkElementInfo> Elements = new List<FrameworkElementInfo>();
		#region DebugPadding
		public static readonly DependencyProperty DebugPaddingProperty = DependencyProperty.RegisterAttached("DebugPadding", typeof(bool), typeof(DebuggerHelper), new PropertyMetadata(false, new PropertyChangedCallback(DebugPaddingPropertyChanged)));
		static void DebugPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = d as TextBlock;
			if (element == null)
				return;
			element.Unloaded += new RoutedEventHandler(element_Unloaded);
			Elements.Add(new FrameworkElementInfo(element));
		}
		static void element_Unloaded(object sender, RoutedEventArgs e) {
			FrameworkElementInfo info = Elements.Find(item => (FrameworkElement)sender == item.Element);
			Elements.Remove(info);
		}
		public static void SetDebugPadding(DependencyObject d, bool value) {
			d.SetValue(DebugPaddingProperty, value);
		}
		public static bool GetDebugPadding(DependencyObject d) {
			return (bool)d.GetValue(DebugPaddingProperty);
		}
		#endregion
		public static void WriteTotalChildrenCount(string elementName, UIElement element) {
			int childrenCount = 0;
			DevExpress.Xpf.Core.Native.LayoutHelper.ForEachElement(element as FrameworkElement, fe => childrenCount++);
			System.Diagnostics.Debug.WriteLine(String.Format("Total {0} children count: {1}", elementName, childrenCount));
		}
	}
	public class DebugConverterExtension : MarkupExtension, IValueConverter {
		static DebugConverterExtension instance;
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (instance == null)
				instance = new DebugConverterExtension();
			return instance;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
	public class BindingDebugConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		#endregion
	}
}
#endif
namespace DevExpress.Xpf.Scheduler.Internal {
	public static class VisualElementHelper {
		public static string GetTypeName(DependencyObject current) {
			if (current == null)
				return "none";
			return current.GetType().Name;
		}
		public static object GetElementName(DependencyObject child) {
			if (child == null)
				return String.Empty;
			FrameworkElement fe = child as FrameworkElement;
			if (fe == null)
				return String.Empty;
			return fe.Name;
		}
		public static string GetStringValue(object newValue) {
			if (newValue == null)
				return "null";
#if DEBUGTEST
			SharedGroupStateCollection sharedGroupStateCollection = newValue as SharedGroupStateCollection;
			if (sharedGroupStateCollection != null) {
				return String.Format("{{{0}}}", sharedGroupStateCollection.GetStringValue());
			}
			return newValue.ToString();
		}
#else
			return String.Empty;
		}
#endif
	}
#if !SL
	[SecuritySafeCritical]
	public class DpiIndependentUtils {
		static bool isTransformToDeviceInitialized;
		static bool isTransformFromDeviceInitialized;
		static Matrix transformToDevice;
		static Matrix transformFromDevice;
		static Matrix TransformToDevice {
			get {
				if (!isTransformToDeviceInitialized) {
					isTransformToDeviceInitialized = true;
					using (var windowSource = new HwndSource(new HwndSourceParameters()))
						transformToDevice = windowSource.CompositionTarget.TransformToDevice;
				}
				return transformToDevice;
			}
		}
		static Matrix TransformFromDevice {
			get {
				if (!isTransformFromDeviceInitialized) {
					isTransformFromDeviceInitialized = true;
					using (var windowSource = new HwndSource(new HwndSourceParameters()))
						transformFromDevice = windowSource.CompositionTarget.TransformFromDevice;
				}
				return transformFromDevice;
			}
		}
		public static Rect ToPixel(Rect sourceRect) {
			Point start = TransformToDevice.Transform(sourceRect.Location);
			Size size = (Size)TransformToDevice.Transform((Vector)sourceRect.Size);
			return new Rect(start, size);
		}
		public static Size ToPixel(Size size) {
			return (Size)TransformToDevice.Transform((Vector)size);
		}
		public static Thickness ToPixel(Thickness thickness) {
			return thickness;
		}
		public static Rect ToLogical(Rect rect) {
			Point start = TransformFromDevice.Transform(rect.Location);
			Size size = (Size)TransformFromDevice.Transform((Vector)rect.Size);
			return new Rect(start, size);
		}
	}
#endif
	class SelectionAppointmentsSynchronizer {
		SchedulerControl Scheduler { get; set; }
		internal bool innerCollectionLocker = false;
		internal int observableCollectionLocker = 0;
		public SelectionAppointmentsSynchronizer(SchedulerControl control) {
			Scheduler = control;
		}
		public void SubscribeOnInnerCollectionChange() {
			if (this.innerCollectionLocker)
				return;
			Scheduler.SelectedAppointments.CollectionChanged += new CollectionChangedEventHandler<Appointment>(InnerCollectionChanged);
			this.innerCollectionLocker = true;
		}
		public void UnsubscribeOnInnerCollectionChange() {
			this.innerCollectionLocker = false;
			Scheduler.SelectedAppointments.CollectionChanged -= new CollectionChangedEventHandler<Appointment>(InnerCollectionChanged);
		}
		public void SubscribeOnObservableCollectionChanged(ObservableCollection<Appointment> collection) {
			if (observableCollectionLocker > 0)
				observableCollectionLocker--;
			if (observableCollectionLocker == 0)
				collection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ObservableCollectionChanged);
		}
		public void UnsubscribeOnObservableCollectionChanged(ObservableCollection<Appointment> collection) {
			observableCollectionLocker++;
			collection.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ObservableCollectionChanged);
		}
		void ObservableCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UnsubscribeOnInnerCollectionChange();
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add) {
				Scheduler.SelectedAppointments.Add(e.NewItems[0] as Appointment);
			} else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove) {
				Scheduler.SelectedAppointments.Remove(e.OldItems[0] as Appointment);
			} else {
				SyhcnronizeInnerSelectedAppointments();
			}
			SubscribeOnInnerCollectionChange();
		}
		void InnerCollectionChanged(object sender, CollectionChangedEventArgs<Appointment> e) {
			UnsubscribeOnObservableCollectionChanged(Scheduler.SelectedAppointmentsBindable);
			if (e.Action == CollectionChangedAction.Add) {
				Scheduler.SelectedAppointmentsBindable.Add(e.Element);
			} else if (e.Action == CollectionChangedAction.Remove) {
				Scheduler.SelectedAppointmentsBindable.Remove(e.Element);
			} else if (e.Action == CollectionChangedAction.Clear) {
				Scheduler.SelectedAppointmentsBindable.Clear();
			} else {
				SyhcnronizeSelectedAppointmentsBindable();
			}
			SubscribeOnObservableCollectionChanged(Scheduler.SelectedAppointmentsBindable);
		}
		public void SyhcnronizeSelectedAppointmentsBindable() {
			if (Scheduler == null)
				return;
			ILockable lockableCollection = Scheduler.SelectedAppointmentsBindable as ILockable;
			if (lockableCollection != null)
				lockableCollection.BeginUpdate();
			Scheduler.SelectedAppointmentsBindable.Clear();
			foreach (Appointment item in Scheduler.SelectedAppointments) {
				Scheduler.SelectedAppointmentsBindable.Add(item);
			}
			if (lockableCollection != null)
				lockableCollection.EndUpdate();
		}
		public void SyhcnronizeInnerSelectedAppointments() {
			if (Scheduler == null)
				return;
			Scheduler.SelectedAppointments.BeginUpdate();
			Scheduler.SelectedAppointments.Clear();
			foreach (Appointment item in Scheduler.SelectedAppointmentsBindable) {
				Scheduler.SelectedAppointments.Add(item);
			}
			Scheduler.SelectedAppointments.EndUpdate();
		}
	}
	public class MouseWheelScrollHelper {
		int distance = 0;
		DateTime lastEvent = DateTime.MinValue;
		const int SkipInterval = 400;
		const int PixelLineHeight = 120;
		IMouseWheelScrollClient client;
		public MouseWheelScrollHelper(IMouseWheelScrollClient client) {
			this.client = client;
		}
		public void OnMouseWheel(MouseWheelEventArgs e) {
#if !SL
			MouseWheelEventArgsEx dxmea = e as MouseWheelEventArgsEx;
			if (DateTime.Now.Subtract(lastEvent).TotalMilliseconds > SkipInterval) {
				ResetDistance();
			}
			this.lastEvent = DateTime.Now;
			int delta = (dxmea == null) ? e.Delta : dxmea.DeltaX;
#else
			int delta = e.Delta;
#endif
			if (delta % 120 == 0 && distance == 0) {
				OnScrollLine(e, delta / 120, true);
				return;
			}
			distance += delta;
			int lineCount = distance / PixelLineHeight;
			distance = distance % PixelLineHeight;
			if (lineCount == 0)
				return;
			OnScrollLine(e, lineCount, false);
		}
		void ResetDistance() {
			this.distance = 0;
		}
		void OnScrollLine(MouseWheelEventArgs e, int linesCount, bool allowSystemLinesCount) {
			if (client == null)
				return;
#if !SL
			if (e is MouseWheelEventArgsEx)
				client.OnMouseWheel(new MouseWheelEventArgsEx(e.MouseDevice, e.Timestamp, linesCount, 0));
			else
				client.OnMouseWheel(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, linesCount));
#else
			client.OnMouseWheel(e);
#endif
		}
		int GetDirection(MouseWheelEventArgs e) {
			return e.Delta > 0 ? -1 : 1;
		}
	}
	public interface ISupportCheckIntegrity {
		void CheckIntegrity();
	}
	public static class SchedulerAssert {
		[Conditional("Debug")]
		public static void CheckIntegrity(object target) {
			ISupportCheckIntegrity checker = target as ISupportCheckIntegrity;
			if (checker != null)
				checker.CheckIntegrity();
			IList itemList = target as IList;
			if (itemList != null)
				CheckItemsIntegrity(itemList);
		}
		[Conditional("Debug")]
		static void CheckItemsIntegrity(IList itemList) {
			int count = itemList.Count;
			for (int i = 0; i < count; i++)
				CheckIntegrity(itemList[i]);
		}
	}
	public class WpfColorToStringSerializer {
		private static readonly Dictionary<string, Color> KnownColors;
		static WpfColorToStringSerializer() {
			PropertyInfo[] knownColorProperties = typeof(Colors).GetProperties();
			KnownColors = new Dictionary<string, Color>(knownColorProperties.Length);
			FillKnownValues(knownColorProperties);
		}
		private static void FillKnownValues(PropertyInfo[] knownColorProperties) {
			foreach (PropertyInfo p in knownColorProperties) {
				Color colorValue = (Color)p.GetValue(null, null);
				KnownColors[p.Name] = colorValue;
			}
		}
		public static Color StringToColor(string colorName) {
			if (string.IsNullOrEmpty(colorName))
				return ColorExtension.FromArgb(0);
			if (colorName.StartsWith("0x")) {
				string colorHexValue = colorName.Substring(2, 8);
				int argbColor = int.Parse(colorHexValue, NumberStyles.HexNumber);
				return ColorExtension.FromArgb(argbColor);
			}
			Color result;
			if (KnownColors.TryGetValue(colorName, out result))
				return result;
			System.Drawing.Color winColor = WinColorToStringSerializer.StringToColor(colorName);
			return ColorExtension.FromArgb(winColor.ToArgb());
		}
		public static string ColorToString(Color color) {
			return String.Format("0x{0:X8}", color.ToArgb());
		}
	}
	public static class WpfSchedulerImageHelper {
		private static readonly BytesToImageSourceConverter converter;
		static WpfSchedulerImageHelper() {
			converter = new BytesToImageSourceConverter();
		}
		public static ImageSource CreateImageFromBytes(byte[] bytes) {
			return (ImageSource)converter.Convert(bytes, typeof(BitmapImage), null, CultureInfo.InvariantCulture);
		}
		public static byte[] GetImageBytes(ImageSource image) {
			return ImageLoader2.ImageToByteArray(image);
		}
	}
	[SecuritySafeCritical]
	public static class BrushToImageBytesConverter {
		public static byte[] Convert(Brush brush, Size imageSize) {
			GeometryDrawing drawing = new GeometryDrawing(brush, null, new RectangleGeometry(new Rect(0.0, 0.0, imageSize.Width, imageSize.Height)));
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen()) {
				drawingContext.PushTransform(new TranslateTransform(-drawing.Bounds.X, -drawing.Bounds.Y));
				drawingContext.DrawDrawing(drawing);
			}
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)imageSize.Width, (int)imageSize.Height, 96, 96, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(drawingVisual);
			BitmapFrame frame = BitmapFrame.Create(renderTargetBitmap);
			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(frame);
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
				encoder.Save(ms);
				return ms.ToArray();
			}
		}
	}
}
namespace DevExpress.XtraScheduler.Internal.Diagnostics {
	#region Logger
#if DEBUGTEST
	public static class DebugConfig {
		static DebugConfig() {
			SchedulerLogger.UsageMessageType |= XpfLoggerTraceLevel.Warning;
		}
		internal static void Init() {
		}
	}
#endif
	public class XpfLoggerTraceLevel : LoggerTraceLevel {
		public const int PixelSnappedUniformGrid = 0x8;
		public const int HightLimitControl = 0x10;
		public const int AppoinmentPanel = 0x20;
		public const int SharedSizePanel = 0x80;
		public const int GanttViewGroupByResourceLayoutPanel = 0x100;
		public const int PixelSnappedSharedSizePanel = 0x200;
		public const int WeekPanel = 0x400;
		public const int ItemsBasedComponent = 0x800;
		public const int TimelineViewPanelGroupByNone = 0x1000;
		public const int VisualTimeScaleHeader = 0x2000;
		public const int VisualComponent = 0x4000;
		public const int AppointmentPanel = 0x8000;
		public const int RangeControl = 0x10000;
		public const int SchedulerControl = 0x20000;
		public const int Panel = PixelSnappedUniformGrid | HightLimitControl | AppoinmentPanel | SharedSizePanel | GanttViewGroupByResourceLayoutPanel | PixelSnappedSharedSizePanel | WeekPanel | ItemsBasedComponent | TimelineViewPanelGroupByNone | AppointmentPanel;
	}
	#endregion
}
