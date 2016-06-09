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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Ribbon {
	public class DXRibbonWindowTitlePanel : Panel {
		#region static
		public static readonly DependencyProperty MinCaptionWidthBlockProperty =
			DependencyPropertyManager.Register("MinCaptionWidthBlock", typeof(TextBlock), typeof(DXRibbonWindowTitlePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnMinCaptionWidthBlockPropertyChanged)));
		public static readonly DependencyProperty CaptionContentControlProperty =
			DependencyPropertyManager.Register("CaptionContentControl", typeof(ContentControl), typeof(DXRibbonWindowTitlePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCaptionContentControlPropertyChanged)));
		public static readonly DependencyProperty RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(typeof(DXRibbonWindowTitlePanel), new PropertyMetadata(OnRibbonStylePropertyChanged));
		public static readonly DependencyProperty QuickAccessToolbarContainerProperty =
			DependencyPropertyManager.Register("QuickAccessToolbarContainer", typeof(ContentControl), typeof(DXRibbonWindowTitlePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnQuickAccessToolbarContainerPropertyChanged)));
		public static readonly DependencyProperty RibbonWindowProperty =
			DependencyPropertyManager.Register("RibbonWindow", typeof(DXRibbonWindow), typeof(DXRibbonWindowTitlePanel), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnRibbonWindowPropertyChanged)));
		protected static void OnQuickAccessToolbarContainerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowTitlePanel)d).OnQuickAccessToolbarContainerChanged((ContentControl)e.OldValue);
		}
		protected static void OnRibbonWindowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowTitlePanel)d).OnRibbonWindowChanged((DXRibbonWindow)e.OldValue);
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowTitlePanel)d).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		protected static void OnCaptionContentControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowTitlePanel)d).OnCaptionContentControlChanged((ContentControl)e.OldValue);
		}
		protected static void OnMinCaptionWidthBlockPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DXRibbonWindowTitlePanel)d).OnMinCaptionWidthBlockChanged((TextBlock)e.OldValue);
		}
		#endregion
		#region props
		public ContentControl QuickAccessToolbarContainer {
			get { return (ContentControl)GetValue(QuickAccessToolbarContainerProperty); }
			set { SetValue(QuickAccessToolbarContainerProperty, value); }
		}
		public DXRibbonWindow RibbonWindow {
			get { return (DXRibbonWindow)GetValue(RibbonWindowProperty); }
			set { SetValue(RibbonWindowProperty, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public ContentControl CaptionContentControl {
			get { return (ContentControl)GetValue(CaptionContentControlProperty); }
			set { SetValue(CaptionContentControlProperty, value); }
		}
		public TextBlock MinCaptionWidthBlock {
			get { return (TextBlock)GetValue(MinCaptionWidthBlockProperty); }
			set { SetValue(MinCaptionWidthBlockProperty, value); }
		}
		public double CategoriesPaneLeftOffset {
			get {
				if(CategoriesPane == null)
					return 0d;
				return CategoriesPane.TranslatePoint(new Point(), this).X;
			}
		}
		public double ControlBoxWidth {
			get { return controlBoxWidth; }
			set {
				if (controlBoxWidth.Equals(value))
					return;
				double oldValue = controlBoxWidth;
				controlBoxWidth = value;
				OnControlBoxWidthChanged(oldValue);
			}
		}
		public Point UnavailableArea {
			get { return unavailableArea; }
			set {
				if (Point.Equals(value, unavailableArea))
					return;
				Point oldValue = unavailableArea;
				unavailableArea = value;
				OnUnavailableAreaChanged(oldValue);
			}
		}
		RibbonControl Ribbon { get { return TemplatedParent as RibbonControl; } }
		RibbonPageCategoriesPane CategoriesPane { get { return Ribbon.With(ribbon => ribbon.CategoriesPane); } }
		#endregion
		public DXRibbonWindowTitlePanel() {
			unavailableArea = new Point(-1d, -1d);
			LayoutUpdated += OnLayoutUpdated;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(CaptionContentControl == null || MinCaptionWidthBlock == null)
				return base.MeasureOverride(availableSize);
			Size desiredSize = new Size();
			UpdateUnavailableArea();
			foreach(UIElement child in Children) {
				child.Measure(SizeHelper.Infinite);
				if(child == MinCaptionWidthBlock)
					continue;
				desiredSize.Width += child.DesiredSize.Width;
				desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
			}
			double unavailbaleWidth = UnavailableArea.Y - UnavailableArea.X;
			double qatWidth = 0d;
			if(QuickAccessToolbarContainer != null && RibbonStyle != RibbonStyle.TabletOffice) {
				qatWidth = Math.Max(0d, QuickAccessToolbarContainer.TranslatePoint(new Point(QuickAccessToolbarContainer.DesiredSize.Width, 0), this).X);
				unavailbaleWidth += qatWidth;
			}
			if(Math.Ceiling(desiredSize.Width + unavailbaleWidth) > availableSize.Width) {
				double startPoint = 0d;
				double leftAvailable = Math.Max(0d, UnavailableArea.X - startPoint - qatWidth);
				double rightAvailable = Math.Max(0d, availableSize.Width - UnavailableArea.Y);
				if(UnavailableArea.Y - UnavailableArea.X == 0)
					leftAvailable = Math.Max(0d, availableSize.Width - unavailbaleWidth);
				double minAvailableWidth = MinCaptionWidthBlock.DesiredSize.Width;
				desiredSize.Width -= CaptionContentControl.DesiredSize.Width;
				minAvailableWidth = Math.Max(leftAvailable, rightAvailable);
				if(minAvailableWidth < MinCaptionWidthBlock.DesiredSize.Width)
					CaptionContentControl.Measure(Size.Empty);
				else
					CaptionContentControl.Measure(new Size(minAvailableWidth, availableSize.Height));
				desiredSize.Width += CaptionContentControl.DesiredSize.Width;
			}
			return desiredSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (CaptionContentControl == null || MinCaptionWidthBlock == null)
				return base.ArrangeOverride(finalSize);
			UpdateUnavailableArea();
			Point leftTop = new Point();
			if (QuickAccessToolbarContainer != null && RibbonStyle != RibbonStyle.TabletOffice) {
				leftTop.X += QuickAccessToolbarContainer.TranslatePoint(new Point(QuickAccessToolbarContainer.ActualWidth, 0), this).X;
			}
			double leftAvailable = Math.Max(0d, UnavailableArea.X - leftTop.X);
			double rightAvailable = Math.Max(0d, finalSize.Width - UnavailableArea.Y);
			if (UnavailableArea.Y == UnavailableArea.X) {
				leftAvailable = 0d;
				rightAvailable = finalSize.Width - leftTop.X;
			}
			if (rightAvailable > leftAvailable) {
				var root = TemplatedParent as UIElement;
				double left = TranslatePoint(new Point(), root).X;
				double windowCenter = (left + finalSize.Width + ControlBoxWidth - CaptionContentControl.DesiredSize.Width) / 2 - left;
				if (UnavailableArea.X != UnavailableArea.Y)
					leftTop.X = UnavailableArea.Y;
				leftTop.X = Math.Max(windowCenter, leftTop.X);
			} else {
				leftTop.X += Math.Max(0d, (leftAvailable - CaptionContentControl.DesiredSize.Width) / 2);
			}
			leftTop.X = Math.Min(leftTop.X, finalSize.Width - CaptionContentControl.DesiredSize.Width / 2);
			CaptionContentControl.Arrange(new Rect(leftTop, new Size(CaptionContentControl.DesiredSize.Width, finalSize.Height)));
			return finalSize;
		}
		protected virtual void OnLayoutUpdated(object sender, EventArgs e) {
			UpdateUnavailableArea();
			UpdateControlBoxWidth();
			Margin = new Thickness(0, 0, ControlBoxWidth, 0);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateRibbonWindow();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			RibbonWindow = null;
		}
		protected virtual void OnUnavailableAreaChanged(Point oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnControlBoxWidthChanged(double oldValue) {
			InvalidateMeasure();
		}
		protected virtual void UpdateControlBoxWidth() {
			if(RibbonWindow == null) {
				ControlBoxWidth = 0d;
				return;
			}
			if(RibbonWindow.IsAeroMode) {
				ControlBoxWidth = RibbonWindow.GetControlBoxRect().Width;
			} else {
				var container = RibbonWindow.GetControlBoxContainer();
				if(container == null)
					ControlBoxWidth = 0d;
				else {
					var ribbon = Ribbon ?? RibbonWindow.Ribbon;
					Point position = container.TranslatePoint(new Point(), RibbonWindow);
					ControlBoxWidth = ribbon.FlowDirection == FlowDirection.RightToLeft ? position.X : RibbonWindow.ActualWidth - position.X;
				}
			}
		}
		protected virtual void UpdateRibbonWindow() {
			RibbonWindow = Window.GetWindow(this) as DXRibbonWindow;
		}
		protected virtual void OnMinCaptionWidthBlockChanged(TextBlock oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnCaptionContentControlChanged(ContentControl oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnRibbonWindowChanged(DXRibbonWindow oldValue) {
			UpdateControlBoxWidth();
		}
		protected virtual void OnQuickAccessToolbarContainerChanged(ContentControl oldValue) {
			InvalidateMeasure();
		}
		void OnRibbonHeaderItemsPanelStartWidthChanged(object sender, ValueChangedEventArgs<double> e) {
			UpdateUnavailableArea();
		}
		void OnRibbonHeaderItemsPanelEndWidthChanged(object sender, ValueChangedEventArgs<double> e) {
			UpdateUnavailableArea();
		}
		void UpdateUnavailableArea(double start, double end) {
			double coerce = start == -1 && end == -1 ? 0d : CategoriesPaneLeftOffset;
			UnavailableArea = new Point(start + coerce, end + coerce);
		}
		void UpdateUnavailableArea() {
			double start = -1d;
			double end = -1d;
			if(CategoriesPane != null) {
				if (CategoriesPane.Opacity > 0) {
					start = CategoriesPane.GetContextualCategoriesLeftOffset();
					end = start + CategoriesPane.GetHeadersWidth();
				} else if (Ribbon.ApplicationMenu is BackstageViewControl) {
					start = 0d;
					end = Ribbon.TranslatePoint(new Point(((BackstageViewControl)Ribbon.ApplicationMenu).GetLeftPartActualWidth(), 0d), this).X;
				}
			}
			UpdateUnavailableArea(start, end);
		}
		double controlBoxWidth;
		Point unavailableArea;
	}
	public class TitleToMinTextWidthConverter : System.Windows.Markup.MarkupExtension, System.Windows.Data.IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			string sValue = value as string;
			if (sValue == null) return value;
			int length = sValue.Length;
			if (length == 0) return sValue;
			return (length <= 3 ? sValue : sValue.Remove(3)) + "...";
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
}
