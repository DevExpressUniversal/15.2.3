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
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
#if SL
using DevExpress.Xpf.Utils.Themes;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Editors.WPFCompatibility;
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
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public class GradientMultiSlider : Control {
		#region static
		public static readonly DependencyProperty BrushProperty;
		internal static readonly DependencyPropertyKey ThumbsPropertyKey;
		public static readonly DependencyProperty ThumbsProperty;
		public static readonly DependencyProperty SelectedThumbProperty;
		public static readonly DependencyProperty SelectedThumbColorProperty;
		public static readonly DependencyProperty BrushTypeProperty;
		static GradientMultiSlider() {
			Type ownerType = typeof(GradientMultiSlider);
			BrushProperty = DependencyPropertyManager.Register("Brush", typeof(GradientBrush), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, (obj, args) => ((GradientMultiSlider)obj).OnBrushChanged((GradientBrush)args.NewValue)));
			ThumbsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Thumbs", typeof(ObservableCollection<GradientMultiSliderThumb>), ownerType, new PropertyMetadata(null));
			ThumbsProperty = ThumbsPropertyKey.DependencyProperty;
			SelectedThumbProperty = DependencyPropertyManager.Register("SelectedThumb", typeof(GradientMultiSliderThumb), ownerType,
				new PropertyMetadata(null, (obj, args) => ((GradientMultiSlider)obj).OnSelectedThumbChanged((GradientMultiSliderThumb)args.NewValue)));
			SelectedThumbColorProperty = DependencyPropertyManager.Register("SelectedThumbColor", typeof(Color), ownerType,
				new PropertyMetadata(Colors.Black, (obj, args) => ((GradientMultiSlider)obj).OnSelectedThumbColorChanged((Color)args.NewValue)));
			BrushTypeProperty = DependencyPropertyManager.Register("BrushType", typeof(GradientBrushType), ownerType,
				new PropertyMetadata(GradientBrushType.Linear, (obj, args) => ((GradientMultiSlider)obj).OnBrushTypeChanged((GradientBrushType)args.NewValue)));
		}
		#endregion
		public GradientBrush Brush {
			get { return (GradientBrush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		public ObservableCollection<GradientMultiSliderThumb> Thumbs {
			get { return (ObservableCollection<GradientMultiSliderThumb>)GetValue(ThumbsProperty); }
			internal set { SetValue(ThumbsPropertyKey, value); }
		}
		public GradientMultiSliderThumb SelectedThumb {
			get { return (GradientMultiSliderThumb)GetValue(SelectedThumbProperty); }
			set { SetValue(SelectedThumbProperty, value); }
		}
		public Color SelectedThumbColor {
			get { return (Color)GetValue(SelectedThumbColorProperty); }
			set { SetValue(SelectedThumbColorProperty, value); }
		}
		public GradientBrushType BrushType {
			get { return (GradientBrushType)GetValue(BrushTypeProperty); }
			set { SetValue(BrushTypeProperty, value); }
		}
		public ICommand FlipThumbsCommand { get; private set; }
		internal Rectangle GradientRectangle { get; set; }
		protected virtual void OnBrushChanged(GradientBrush newValue) {
			if (newValue == null)
				return;
			thumbsLocker.DoLockedActionIfNotLocked(() => {
				UpdateThumbs();
				SelectThumb(Thumbs.First());
			});
		}
		protected virtual void OnSelectedThumbChanged(GradientMultiSliderThumb newValue) {
			selectedThumbLocker.DoLockedActionIfNotLocked(() => SelectedThumbColor = newValue.Color);
			SelectThumb(newValue);
		}
		protected virtual void OnSelectedThumbColorChanged(Color newValue) {
			selectedThumbLocker.DoLockedActionIfNotLocked(() => { SelectedThumb.Color = newValue; });
			UpdateBrush(false);
		}
		protected virtual void OnBrushTypeChanged(GradientBrushType newValue) {
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
		}
		readonly Locker selectedThumbLocker, thumbsLocker;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeEvents();
			GradientRectangle = (Rectangle)GetTemplateChild("PART_GradientRect");
			SubscribeEvents();
		}
		public GradientMultiSlider() {
			this.SetDefaultStyleKey(typeof(GradientMultiSlider));
			Loaded += OnLoaded;
			selectedThumbLocker = new Locker();
			thumbsLocker = new Locker();
			FlipThumbsCommand = DelegateCommandFactory.Create<object>(obj => FlipThumbs(), false);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if (Brush != null)
				return;
			AddThumb(0, Colors.Black);
			AddThumb(1, Colors.White);
			SelectThumb(Thumbs.FirstOrDefault());
		}
		void UnsubscribeEvents() {
			GradientRectangle.Do(x => x.PreviewMouseLeftButtonDown -= OnGradientLeftButtonDown);
		}
		void SubscribeEvents() {
			GradientRectangle.Do(x => x.PreviewMouseLeftButtonDown += OnGradientLeftButtonDown);
		}
		void FlipThumbs() {
			foreach (var thumb in Thumbs) {
				thumb.Offset = Math.Abs(1 - thumb.Offset);
			}
		}
		void OnGradientLeftButtonDown(object sender, MouseButtonEventArgs e) {
			var thumbOffset = e.GetPosition((Rectangle)sender).X;
			var relativeOffset = thumbOffset / GradientRectangle.ActualWidth;
			SelectThumb(AddThumb(relativeOffset, GetColorAtOffset(thumbOffset)));
		}
		internal Color GetColorAtOffset(double thumbOffset) {
			return Brush.GetColorAtPoint(GradientRectangle.ActualWidth, GradientRectangle.ActualHeight, new Point(thumbOffset, GradientRectangle.ActualHeight / 2d));
		}
		internal GradientMultiSliderThumb AddThumb(double offset, Color color) {
			if (Thumbs == null)
				Thumbs = new ObservableCollection<GradientMultiSliderThumb>();
			var newThumb = new GradientMultiSliderThumb() { OwnerSlider = this, Offset = offset, Color = color };
			SubscribeThumbEvents(newThumb);
			Thumbs.Add(newThumb);
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
			return newThumb;
		}
		internal void AddThumb(GradientMultiSliderThumb thumb) {
			Thumbs.Add(thumb);
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
		}
		internal void RemoveThumb(GradientMultiSliderThumb thumb) {
			thumb.ThumbColorChanged -= OnThumbColorChanged;
			thumb.ThumbPositionChanged -= OnThumbPositionChanged;
			if (thumb.IsSelected)
				SelectThumb(Thumbs.FirstOrDefault());
			Thumbs.Remove(thumb);
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
		}
		internal void SelectThumb(GradientMultiSliderThumb thumb) {
			Thumbs.ForEach(x => x.IsSelected = false);
			thumb.IsSelected = true;
			SelectedThumb = thumb;
		}
		void OnThumbColorChanged(object sender, RoutedEventArgs e) {
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
		}
		void OnThumbPositionChanged(object sender, RoutedEventArgs e) {
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
		}
		void SubscribeThumbEvents(GradientMultiSliderThumb thumb) {
			thumb.ThumbPositionChanged += OnThumbPositionChanged;
			thumb.ThumbColorChanged += OnThumbColorChanged;
		}
		void UnsubscribeThumbEvents(GradientMultiSliderThumb thumb) {
			thumb.ThumbPositionChanged -= OnThumbPositionChanged;
			thumb.ThumbColorChanged -= OnThumbColorChanged;
		}
		internal void UpdateBrush(bool updateThumbs) {
			if (!updateThumbs)
				thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
			else
				UpdateBrush();
		}
		void UpdateBrush() {
			if (!IsInitialized)
				return;
			var gradientStops = new GradientStopCollection();
			foreach (GradientMultiSliderThumb thumb in Thumbs) {
				if (thumb.IgnoreThumb)
					continue;
				var gradientStop = new GradientStop(thumb.Color, thumb.Offset);
				gradientStops.Add(gradientStop);
			}
			UpdateBrush(gradientStops);
		}
		void UpdateBrush(GradientStopCollection gradientStops) {
			Brush = CloneBrush(Brush, BrushType, gradientStops);
		}
		GradientBrush CloneBrush(GradientBrush brush, GradientBrushType brushType, GradientStopCollection gradientStops) {
			if (brush == null || !IsValidBrush(brush, brushType)) {
				switch (BrushType) {
					case GradientBrushType.Linear:
						brush = new LinearGradientBrush(gradientStops, new Point(0d, 0.5), new Point(1d, 0.5));
						break;
					case GradientBrushType.Radial:
						brush = new RadialGradientBrush(gradientStops);
						break;
				}
			}
			var result = brush.With(x => x.Clone());
			result.Do(x => x.GradientStops = gradientStops);
			return result;
		}
		bool IsValidBrush(GradientBrush brush, GradientBrushType brushType) {
			return (brush is LinearGradientBrush && brushType == GradientBrushType.Linear) || (brush is RadialGradientBrush && brushType == GradientBrushType.Radial);
		}
		internal void UpdateThumbs() {
			if (Thumbs != null)
				Thumbs.ForEach(UnsubscribeThumbEvents);
			Thumbs = new ObservableCollection<GradientMultiSliderThumb>();
			foreach (var gradientStop in Brush.GradientStops) {
				AddThumb(gradientStop.Offset, gradientStop.Color);
			}
			thumbsLocker.DoLockedActionIfNotLocked(UpdateBrush);
		}
	}
	public static class LinearGradientBrushHelper {
		public static Color GetColorAtPoint(this GradientBrush brush, double width, double height, Point thePoint) {
			double y1 = 0.5 * height;
			Point p1 = new Point(0d, y1);
			double x2 = width;
			double y2 = 0.5 * height;
			Point p2 = new Point(x2, y2);
			Point p3;
			if (y1.AreClose(y2)) {
				p3 = new Point(thePoint.X, y1);
			}
			else if (x2.AreClose(0d)) {
				p3 = new Point(0d, thePoint.Y);
			}
			else {
				double m = (y2 - y1) / x2;
				double m2 = -1 / m;
				double b = y1;
				double c = thePoint.Y - m2 * thePoint.X;
				double x4 = (c - b) / (m - m2);
				double y4 = m * x4 + b;
				p3 = new Point(x4, y4);
			}
			double d4 = Dist(p3, p1, p2);
			double d2 = Dist(p2, p1, p2);
			double x = d4 / d2;
			double max = brush.GradientStops.Max(n => n.Offset);
			if (x > max) {
				x = max;
			}
			double min = brush.GradientStops.Min(n => n.Offset);
			if (x < min) {
				x = min;
			}
			GradientStop gs0 = brush.GradientStops.Where(n => n.Offset <= x).OrderBy(n => n.Offset).Last();
			GradientStop gs1 = brush.GradientStops.Where(n => n.Offset >= x).OrderBy(n => n.Offset).First();
			float y = 0f;
			if (!gs0.Offset.AreClose(gs1.Offset)) {
				y = (float)((x - gs0.Offset) / (gs1.Offset - gs0.Offset));
			}
			Color cx;
			if (brush.ColorInterpolationMode == ColorInterpolationMode.ScRgbLinearInterpolation) {
				float aVal = (gs1.Color.ScA - gs0.Color.ScA) * y + gs0.Color.ScA;
				float rVal = (gs1.Color.ScR - gs0.Color.ScR) * y + gs0.Color.ScR;
				float gVal = (gs1.Color.ScG - gs0.Color.ScG) * y + gs0.Color.ScG;
				float bVal = (gs1.Color.ScB - gs0.Color.ScB) * y + gs0.Color.ScB;
				cx = Color.FromScRgb(aVal, rVal, gVal, bVal);
			}
			else {
				byte aVal = (byte)((gs1.Color.A - gs0.Color.A) * y + gs0.Color.A);
				byte rVal = (byte)((gs1.Color.R - gs0.Color.R) * y + gs0.Color.R);
				byte gVal = (byte)((gs1.Color.G - gs0.Color.G) * y + gs0.Color.G);
				byte bVal = (byte)((gs1.Color.B - gs0.Color.B) * y + gs0.Color.B);
				cx = Color.FromArgb(aVal, rVal, gVal, bVal);
			}
			return cx;
		}
		static double Dist(Point px, Point po, Point pf) {
			double d = Math.Sqrt((px.Y - po.Y) * (px.Y - po.Y) + (px.X - po.X) * (px.X - po.X));
			if ((px.Y.LessThan(po.Y) && pf.Y.GreaterThan(po.Y)) || (px.Y.GreaterThan(po.Y) && pf.Y.LessThan(po.Y)) ||
				(px.Y.AreClose(po.Y) && px.X.LessThan(po.X) && pf.X.GreaterThan(po.X)) ||
				(px.Y.AreClose(po.Y) && px.X.GreaterThan(po.X) && pf.X.LessThan(po.X))) {
				d = -d;
			}
			return d;
		}
	}
}
