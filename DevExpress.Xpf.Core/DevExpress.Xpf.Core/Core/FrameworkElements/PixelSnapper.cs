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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
#if SL
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
#else
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.ComponentModel;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Xpf.Core.Internal {
	public class PixelSnapperBase : Decorator {
		UIElement topElement;
		public PixelSnapperBase() {
			Unloaded += OnPixelSnapperUnloaded;
#if SL
			LayoutUpdated += OnLayoutUpdated;
#endif
		}
		protected UIElement TopElement {
			get {
				if(topElement == null)
					SetTopElement((UIElement)LayoutHelper.GetTopLevelVisual(this));
				return topElement;
			}
		}
		protected virtual void SetTopElement(UIElement topElement) {
			this.topElement = topElement;
		}
		protected virtual void ResetTopElement() {
			topElement = null;
		}
		void OnPixelSnapperUnloaded(object sender, RoutedEventArgs e) {
			ResetTopElement();
		}
		protected virtual Point GetCorrectedRenderOffset() {
			try {
				return RenderOffsetHelper.GetCorrectedRenderOffset(this, TopElement);
			}
			catch {
				ResetTopElement();
				return RenderOffsetHelper.GetCorrectedRenderOffset(this, TopElement);
			}
		}
		double delta = 0.001;
		protected override Size ArrangeOverride(Size arrangeSize) {
			Size sz = base.ArrangeOverride(new Size(Math.Floor(arrangeSize.Width), Math.Floor(arrangeSize.Height)));
			sz.Width += delta;
			delta = delta == 0.001 ? 0.002 : 0.001;
			return sz;
		}
		protected virtual bool IsRotatedLeft(Matrix mat) {
			return mat.M12 == -1 && mat.M21 == 1;
		}
		protected virtual bool IsRotatedRight(Matrix mat) {
			return mat.M12 == 1 && mat.M21 == -1;
		}
		protected virtual Point ApplyLeftRotation(Point pt) {
			return new Point(-pt.Y, pt.X);
		}
		protected virtual Point ApplyRightRotation(Point pt) {
			return new Point(pt.Y, -pt.X);
		}
		bool IsIdentityTransform(Transform transform) {
#if SL
			MatrixTransform matrixTransform = transform as MatrixTransform;
			return matrixTransform != null && matrixTransform.Matrix == Matrix.Identity;
#else
			return transform == Transform.Identity;
#endif
		}
		Point prevOffset = new Point(0, 0);
		protected virtual void UpdateRenderOffset() {
			MatrixTransform mt = RenderTransform as MatrixTransform;
#if SL
			if(mt == null) {
#else
			if(mt == null || mt.IsFrozen) {
#endif
				mt = new MatrixTransform();
				RenderTransform = mt;
			}
			if(!IsIdentityTransform(RenderTransform))
				mt.Matrix = new Matrix(mt.Matrix.M11, mt.Matrix.M12, mt.Matrix.M21, mt.Matrix.M22, mt.Matrix.OffsetX - prevOffset.X, mt.Matrix.OffsetY - prevOffset.Y);
			Point pt = GetCorrectedRenderOffset();
			if(Double.IsNaN(pt.X) || Double.IsNaN(pt.Y))
				return;
			if(pt.X != 0 || pt.Y != 0) {
				MatrixTransform gmt = this.TransformToVisual(TopElement) as MatrixTransform;
				if(IsRotatedLeft(gmt.Matrix)) {
					pt = ApplyLeftRotation(pt);
				} else if(IsRotatedRight(gmt.Matrix)) {
					pt = ApplyRightRotation(pt);
				}
				if(IsIdentityTransform(RenderTransform))
					RenderTransform = new MatrixTransform() { Matrix = new Matrix(1, 0, 0, 1, pt.X, pt.Y) };
				else {
					mt = RenderTransform as MatrixTransform;
					if(mt != null)
						mt.Matrix = new Matrix(mt.Matrix.M11, mt.Matrix.M12, mt.Matrix.M21, mt.Matrix.M22, mt.Matrix.OffsetX + pt.X, mt.Matrix.OffsetY + pt.Y);
					else {
						TransformGroup gr = RenderTransform as TransformGroup;
						if(gr != null && gr.Value.IsIdentity) {
							RenderTransform = new MatrixTransform() { Matrix = new Matrix(1, 0, 0, 1, pt.X, pt.Y) };
						}
					}
				}
			}
			prevOffset = pt;
		}
#if SL
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateRenderOffset();
		}
#else
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			UpdateRenderOffset();
			base.OnRenderSizeChanged(sizeInfo);
		}
#endif
	}
}
namespace DevExpress.Xpf.Core {
#if SL
	public class PixelSnapper : Decorator { }
#else
	public class PixelSnapper : DevExpress.Xpf.Core.Internal.PixelSnapperBase {
		public PixelSnapper() {
			IsVisibleChanged += OnIsVisibleChanged;
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ResetTopElement();
		}
	}
#endif
	public enum SnapperType { Floor, Ceil, Around }
	public class MeasurePixelSnapper : Decorator {
		#region static
		public static readonly DependencyProperty SnapperTypeProperty;
		static MeasurePixelSnapper() {
			SnapperTypeProperty = DependencyPropertyManager.Register("SnapperType", typeof(SnapperType), typeof(MeasurePixelSnapper), new FrameworkPropertyMetadata(SnapperType.Ceil, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
		#endregion
		public SnapperType SnapperType {
			get { return (SnapperType)GetValue(SnapperTypeProperty); }
			set { SetValue(SnapperTypeProperty, value); }
		}
		protected override Size MeasureOverride(Size constraint) {
			return MeasurePixelSnapperHelper.MeasureOverride(base.MeasureOverride(constraint), SnapperType);
		}
	}
#if !SL
	[Browsable(false)]
	public class MeasurePixelSnapperContentPresenter : ContentPresenter {
		protected internal MeasurePixelSnapperContentControl Snapper { protected get; set; }
		static MeasurePixelSnapperContentPresenter() {
			DataContextProperty.OverrideMetadata(typeof(MeasurePixelSnapperContentPresenter), new FrameworkPropertyMetadata(null, (d,e)=>((MeasurePixelSnapperContentPresenter)d).OnDataContextChanged(e), (d,e)=>((MeasurePixelSnapperContentPresenter)d).CoerceDataContext(e)));
		}
		protected virtual object CoerceDataContext(object e) {
			if (Snapper == null || Snapper.ContentTemplate==null)
				return e;
			return Snapper.DataContext;
		}
		protected virtual void OnDataContextChanged(DependencyPropertyChangedEventArgs e) {
		}
	}
#endif
	public class MeasurePixelSnapperContentControl : ContentControl {
		#region static
		public static readonly DependencyProperty SnapperTypeProperty;
		public static readonly DependencyProperty TranslateContentOnRenderProperty;
		static MeasurePixelSnapperContentControl() {
			SnapperTypeProperty = DependencyPropertyManager.Register("SnapperType", typeof(SnapperType), typeof(MeasurePixelSnapperContentControl), new FrameworkPropertyMetadata(SnapperType.Ceil, FrameworkPropertyMetadataOptions.AffectsMeasure));
			TranslateContentOnRenderProperty = DependencyPropertyManager.Register("TranslateContentOnRender", typeof(bool), typeof(MeasurePixelSnapperContentControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
		}
#if !SL
		const string DefaultTemplateXAML =
	 @"<ControlTemplate TargetType='local:MeasurePixelSnapperContentControl' " +
		 "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
		 "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
		 "xmlns:local='clr-namespace:DevExpress.Xpf.Core;assembly=DevExpress.Xpf.Core" + AssemblyInfo.VSuffix + "'>" +
			"<local:MeasurePixelSnapperContentPresenter "
				+ "Content='{TemplateBinding Content}' "
				+ "ContentTemplate='{TemplateBinding ContentTemplate}' "
				+ "Cursor='{TemplateBinding Cursor}' "
				+ "Margin='{TemplateBinding Padding}' "
				+ "HorizontalAlignment='{TemplateBinding HorizontalContentAlignment}' "
				+ "VerticalAlignment='{TemplateBinding VerticalContentAlignment}'/>" +
	 "</ControlTemplate>";
		static ControlTemplate defaultTemplate;
		static ControlTemplate DefaultTemplate {
			get {
				if (defaultTemplate == null)
					defaultTemplate = (ControlTemplate)XamlReader.Parse(DefaultTemplateXAML);
				return defaultTemplate;
			}
		}
#endif
		#endregion        
		public MeasurePixelSnapperContentControl() {
			DataContextChanged += new System.Windows.DependencyPropertyChangedEventHandler(OnDataContextChanged);
#if !SL
			this.Template = DefaultTemplate;
#endif
		}
		void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateChildDataContext();	  
		}
#if !SL
		protected override void OnContentChanged(object oldContent, object newContent) {
			FrameworkElement nFe = newContent as FrameworkElement;
			FrameworkContentElement nFce = newContent as FrameworkContentElement;
			if(nFe != null && nFe.Parent != null || nFce != null && nFce.Parent != null) {
				base.RemoveLogicalChild(oldContent);
				return;
			}
			base.OnContentChanged(oldContent, newContent);
		}
#endif
		public SnapperType SnapperType {
			get { return (SnapperType)GetValue(SnapperTypeProperty); }
			set { SetValue(SnapperTypeProperty, value); }
		}
		public bool TranslateContentOnRender {
			get { return (bool)GetValue(TranslateContentOnRenderProperty); }
			set { SetValue(TranslateContentOnRenderProperty, value); }
		}		
		protected override Size MeasureOverride(Size constraint) {   
			UpdateChildDataContext();
			return MeasurePixelSnapperHelper.MeasureOverride(base.MeasureOverride(constraint), SnapperType);
		}
		FrameworkElement topLevelVisual = null;
		FrameworkElement TopLevelVisual {
			get {
				if(topLevelVisual == null
#if !SL
					|| !topLevelVisual.IsAncestorOf(this)
#endif
					)
					topLevelVisual = LayoutHelper.GetTopLevelVisual(this) as FrameworkElement;
				return topLevelVisual;
			}
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			UpdateChildDataContext();
			return base.ArrangeOverride(arrangeBounds);
		}
		DependencyObject GetChild() {
			var child = System.Windows.Media.VisualTreeHelper.GetChildrenCount(this) > 0 ? System.Windows.Media.VisualTreeHelper.GetChild(this, 0) : null;
#if !SL
			var presenter = child as MeasurePixelSnapperContentPresenter;
			if (presenter != null) {
				presenter.Snapper = this;
			}
#endif
			return child;
		}
		public override void OnApplyTemplate() {
			UpdateChildDataContext();
			base.OnApplyTemplate();
		}		
		void UpdateChildDataContext() {
			ContentPresenter child = GetChild() as ContentPresenter;
			if(child != null && ContentTemplate != null)
				child.DataContext = DataContext;
		}
	}
	public static class MeasurePixelSnapperHelper {
		public static Size MeasureOverride(Size result, SnapperType snapper) {
			if(snapper == SnapperType.Ceil)
				return new Size(Math.Ceiling(result.Width), Math.Ceiling(result.Height));
			else if(snapper == SnapperType.Floor)
				return new Size(Math.Floor(result.Width), Math.Floor(result.Height));
			return new Size(Math.Round(result.Width), Math.Round(result.Height));
		}
	}
	[ContentProperty("Image")]
	public class ImagePixelSnapper : Decorator {
		public static readonly DependencyProperty ImageProperty =
			DependencyPropertyManager.Register("Image", typeof(Image), typeof(ImagePixelSnapper),
			new PropertyMetadata((d, e) => ((ImagePixelSnapper)d).OnImageChanged()));
		public Image Image {
			get { return (Image)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
#if !SILVERLIGHT
		public ImagePixelSnapper() {
			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
			IsXPMode = GetIsXPMode();
		}
		protected bool IsXPMode { get; private set; }
		PixelSnapper snapper = null;
		protected PixelSnapper Snapper {
			get {
				if(snapper == null) {
					snapper = new PixelSnapper();
					Child = snapper;
				}
				return snapper;
			}
		}
		bool GetIsXPMode() {
			bool isWin32NT = Environment.OSVersion.Platform == PlatformID.Win32NT;
			bool isMajor5 = Environment.OSVersion.Version.Major == 5;
			bool isMinor1 = Environment.OSVersion.Version.Minor == 1;
			return isWin32NT && isMajor5 && isMinor1;
		}
#endif
		protected virtual void OnImageChanged() {
			SetContent(Image);
		}
		protected virtual void SetContent(object content) {
#if !SILVERLIGHT
			if(!IsXPMode) Child = Image;
			else Snapper.Child = Image;
#else
			Child = Image;
#endif
		}
	}
#if !SL
	public class DXImage : Image {
		[ThreadStatic]
		static DispatcherTimer updateDXImageOffsetTimer;
		static DispatcherTimer UpdateDXImageOffsetTimer {			
			get { return updateDXImageOffsetTimer ?? (updateDXImageOffsetTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(500) }).Do(x=>x.Start()); }
		}
		public static bool GetLockUpdates(DependencyObject obj) {
			return (bool)obj.GetValue(LockUpdatesProperty);
		}
		public static void SetLockUpdates(DependencyObject obj, bool value) {
			obj.SetValue(LockUpdatesProperty, value);
		}
		public static readonly DependencyProperty LockUpdatesProperty =
			DependencyPropertyManager.RegisterAttached("LockUpdates", typeof(bool), typeof(DXImage), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
		public static BitmapScalingMode ImageQuality = BitmapScalingMode.Unspecified;
		readonly DevExpress.Data.Utils.WeakEventHandler<DXImage, EventArgs, EventHandler> weakUpdateHandler;
		public DXImage() {
			SnapsToDevicePixels = false;
			UseLayoutRounding = true;
			VisualBitmapScalingMode = ImageQuality;
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
			Unloaded += new RoutedEventHandler(DXImage_Unloaded);
			Loaded += new RoutedEventHandler(DXImage_Loaded);
			IgnoreUseLayoutRoundingCheck = false;
			weakUpdateHandler = new Data.Utils.WeakEventHandler<DXImage, EventArgs, EventHandler>(this, 
				(image, timer, args) => image.OnTimerTick(), 
				(wHandler, sender) => ((DispatcherTimer)sender).Tick -= wHandler.Handler, 
				wHandler => wHandler.OnEvent);
		}		
		public bool IgnoreUseLayoutRoundingCheck { get; set; }
		Size SourceSize {
			get {
				if(Source == null) return new Size();
				if(Source is System.Windows.Media.Imaging.BitmapSource) {
					var bitmapSource = (System.Windows.Media.Imaging.BitmapSource)Source;
					return new Size(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
				}
				return new Size(Source.Width, Source.Height);
			}
		}
		Point currentOffset;
		protected override void OnRender(DrawingContext dc) {
			if(this.Source != null) {
				dc.DrawImage(this.Source, new Rect(currentOffset, RenderSize));
			} else base.OnRender(dc);
		}
		bool updateRequested = false;
		void OnLayoutUpdated(object sender, EventArgs e) {			
			if(Source == null || ActualHeight == 0 || ActualWidth == 0 || GetLockUpdates(this)) return;
			updateRequested = true;			
		}
		void OnTimerTick() {
			if (!updateRequested)
				return;
			updateRequested = false;
			if (Source == null || ActualHeight == 0 || ActualWidth == 0 || GetLockUpdates(this)) return;
			Point offset = GetOffset();
			if (!LayoutDoubleHelper.AreClose(offset, currentOffset)) {
				currentOffset = offset;
				InvalidateVisual();
			}
		}
		bool lockUpdates = false;
		PresentationSource presentationSource;
		void DXImage_Unloaded(object sender, RoutedEventArgs e) {
			lockUpdates = true;
			PresentationSource.RemoveSourceChangedHandler(this, OnPresentationSourceChanged);
			presentationSource = null;
			UpdateDXImageOffsetTimer.Tick -= weakUpdateHandler.Handler;
		}
		void DXImage_Loaded(object sender, RoutedEventArgs e) {
			PresentationSource.AddSourceChangedHandler(this, OnPresentationSourceChanged);
			presentationSource = PresentationSource.FromVisual(this);
			lockUpdates = false;
			UpdateDXImageOffsetTimer.Tick += weakUpdateHandler.Handler;
		}
		private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e) {
			presentationSource = e.NewSource;
		}
		Point GetOffset() {
			Point offset = new Point();
			if(lockUpdates) return offset;
			if(presentationSource != null) {
				Visual rootVisual = presentationSource.RootVisual;
				if(!IgnoreUseLayoutRoundingCheck)
					if(rootVisual is FrameworkElement && ((FrameworkElement)rootVisual).UseLayoutRounding)
						return offset;
				offset = this.TransformToAncestor(rootVisual).Transform(offset);
				offset = presentationSource.CompositionTarget.TransformToDevice.Transform(offset);
				offset.X = Math.Round(offset.X);
				offset.Y = Math.Round(offset.Y);
				offset = presentationSource.CompositionTarget.TransformFromDevice.Transform(offset);
				offset = rootVisual.TransformToDescendant(this).Return(x => x.Transform(offset), () => offset); 
			}
			return offset;
		}
	}
#endif
	public class LayoutDoubleHelper {
		const double epsilon = 1.53E-06;
#if !SL
		static double standardDpi = 96d;
		static double currentDpi = new System.Windows.Forms.TextBox().CreateGraphics().DpiX;
		static double sc = standardDpi / currentDpi;
		static double cs = currentDpi / standardDpi;
		public static Size CeilScaledSize(Size sz) {
			Size result = new Size();
			result.Height = CeilScaledValue(result.Width);
			result.Width = CeilScaledValue(result.Height);
			return result;
		}
		public static Double CeilScaledValue(Double d) {
			return Math.Ceiling(Math.Ceiling(d) * (sc)) * (cs);
		}
#endif
		public static bool AreClose(double value1, double value2) {
			if(value1 == value2) {
				return true;
			}
			return (value1 - value2 < epsilon) && (value1 - value2 > epsilon);
		}
		public static bool AreClose(Point point1, Point point2) {
			return (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));
		}
		public static bool AreClose(Rect rect1, Rect rect2) {
			if(rect1.IsEmpty) {
				return rect2.IsEmpty;
			}
			return (((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width));
		}
		public static bool AreClose(Size size1, Size size2) {
			return (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));
		}
#if !SL
		public static bool AreClose(Vector vector1, Vector vector2) {
			return (AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y));
		}
#endif
	}
}
