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

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
#if SL
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Utils.Themes;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Shapes;
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public class GradientMultiSliderThumb : Control {
		#region static
		const double RemoveThumbHeight = 50d;
		protected static readonly DependencyPropertyKey ActualOffsetPropertyKey;
		public static readonly DependencyProperty ActualOffsetProperty;
		public static readonly DependencyProperty OffsetProperty;
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly RoutedEvent ThumbPositionChangedEvent;
		public static readonly RoutedEvent ThumbColorChangedEvent;
		static GradientMultiSliderThumb() {
			Type ownerType = typeof(GradientMultiSliderThumb);
			OffsetProperty = DependencyPropertyManager.Register("Offset", typeof(double), ownerType,
				new PropertyMetadata(0d, (obj, args) => ((GradientMultiSliderThumb)obj).OnOffsetChanged((double)args.NewValue)));
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), ownerType,
				new PropertyMetadata(Colors.Black, (obj, args) => ((GradientMultiSliderThumb)obj).OnColorChanged((Color)args.NewValue)));
			ActualOffsetPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualOffset", typeof(double), ownerType,
				new PropertyMetadata(0d, (obj, args) => ((GradientMultiSliderThumb)obj).OnActualOffsetChanged((double)args.NewValue)));
			ActualOffsetProperty = ActualOffsetPropertyKey.DependencyProperty;
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), ownerType, 
				new PropertyMetadata(false, (obj, args) => ((GradientMultiSliderThumb)obj).OnSelectedChanged((bool)args.NewValue)));
			ThumbPositionChangedEvent = EventManager.RegisterRoutedEvent("ThumbPositionChangedEvent", RoutingStrategy.Direct, typeof(RoutedEventArgs), ownerType);
			ThumbColorChangedEvent = EventManager.RegisterRoutedEvent("ThumbColorChangedEvent", RoutingStrategy.Direct, typeof(RoutedEventArgs), ownerType);
		}
		#endregion
		public double ActualOffset {
			get { return (double)GetValue(ActualOffsetProperty); }
			protected set { SetValue(ActualOffsetPropertyKey, value); }
		}
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public event RoutedEventHandler ThumbPositionChanged {
			add { AddHandler(ThumbPositionChangedEvent, value); }
			remove { RemoveHandler(ThumbPositionChangedEvent, value); }
		}
		public event RoutedEventHandler ThumbColorChanged {
			add { AddHandler(ThumbColorChangedEvent, value); }
			remove { RemoveHandler(ThumbColorChangedEvent, value); }
		}
		GradientMultiSlider ownerSlider;
		public GradientMultiSlider OwnerSlider {
			get { return ownerSlider; }
			set {
				ownerSlider.Do(x => x.SizeChanged -= OnOwnerSizeChanged);
				ownerSlider = value;
				ownerSlider.Do(x => x.SizeChanged += OnOwnerSizeChanged);
			}
		}
		public bool IgnoreThumb { get; set; }
		Locker OffsetLocker { get; set; }
		bool IsDragging { get; set; }
		double SliderWidth { get { return OwnerSlider.With(x => x.GradientRectangle).Return(x => x.ActualWidth, () => 0d); } }
		double ThumbWidth { get { return GetThumbWidth(); } }
		public GradientMultiSliderThumb() {
			this.SetDefaultStyleKey(typeof(GradientMultiSliderThumb));
			OffsetLocker = new Locker();
			Loaded += OnLoaded;
			IsDragging = false;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateActualOffset();
		}
		protected virtual void OnOffsetChanged(double newValue) {
			UpdateActualOffset();
			RaiseThumbPositionChangedEvent();
		}
		protected virtual void OnColorChanged(Color newValue) {
			RaiseThumbColorChangedEvent();
		}
		protected virtual void OnActualOffsetChanged(double newValue) {
			OffsetLocker.DoLockedActionIfNotLocked(() => Offset = CalcOffset(newValue));
		}
		protected virtual void OnOwnerSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateActualOffset();
		}
		protected virtual void OnSelectedChanged(bool newValue) {
			UpdateActualOffset();
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			if (OwnerSlider != null)
				OwnerSlider.SelectThumb(this);
			IsDragging = true;
			CaptureMouse();
		}
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonUp(e);
			IsDragging = false;
			ReleaseMouseCapture();
			if(IgnoreThumb)
				OwnerSlider.RemoveThumb(this);
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			base.OnPreviewMouseMove(e);
			if (!IsDragging)
				return;
			double yOffset = e.GetPosition(OwnerSlider.GradientRectangle).Y;
			if (DoubleExtensions.LessThanOrClose(yOffset, RemoveThumbHeight) && IgnoreThumb) {
				Opacity = 1;
				IgnoreThumb = false;
				OwnerSlider.UpdateBrush(false);
			}
			else if (DoubleExtensions.GreaterThan(yOffset, RemoveThumbHeight) && !IgnoreThumb && OwnerSlider.Thumbs.Count > 2) {
				Opacity = 0;
				IgnoreThumb = true;
				OwnerSlider.UpdateBrush(false);
			}
			double offset = e.GetPosition(OwnerSlider.GradientRectangle).X;
			offset = Math.Max(0, Math.Min(offset, SliderWidth));
			ActualOffset = offset - ThumbWidth / 2d;
		}
		void UpdateActualOffset() {
			OffsetLocker.DoLockedActionIfNotLocked(() => ActualOffset = CalcActualOffset(Offset));
		}
		double CalcActualOffset(double newValue) {
			return SliderWidth * newValue - ThumbWidth / 2d;
		}
		double CalcOffset(double newValue) {
			return !SliderWidth.AreClose(0d) ? (newValue + ThumbWidth / 2d) / SliderWidth : 0d;
		}
		double GetThumbWidth() {
			var canvas = LayoutHelper.FindElementByType(this, typeof(Canvas));
			if (canvas == null)
				return ActualWidth;
			return canvas.VisualChildren().OfType<UIElement>().Max(i => i.DesiredSize.Width);
		}
		void RaiseThumbPositionChangedEvent() {
			RaiseEvent(new RoutedEventArgs(ThumbPositionChangedEvent));
		}
		void RaiseThumbColorChangedEvent() {
			RaiseEvent(new RoutedEventArgs(ThumbColorChangedEvent));
		}
	}
}
