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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.Mvvm;
#if SL
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
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
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public enum ColorPickerColorMode {
		RGB,
		CMYK,
		HLS,
		HSB
	}
	public class ColorPicker : Control, IColorEdit {
		const double ZThumbHeight = 10;
		const double XYThumbHeight = 21;
		const double XYThumbWidth = 21;
		const double MinWidthWithEditors = 297; 
		static double DpiScaleX { get { return ScreenHelper.ScaleX; } }
		static double DpiScaleY { get { return DpiScaleX; } }  
		public static readonly DependencyProperty EditModeProperty;
		public static readonly DependencyProperty ColorProperty;
		public static readonly DependencyProperty DefaultColorProperty;
		public static readonly DependencyProperty ShowDefaultColorProperty;
		public static readonly DependencyProperty ShowEditorsProperty;
		public static readonly DependencyProperty ColorModeProperty;
		static readonly DependencyPropertyKey ColorViewModelPropertyKey;
		public static readonly DependencyProperty ColorViewModelProperty;
		static readonly DependencyPropertyKey ActualShowEditorsPropertyKey;
		public static readonly DependencyProperty ActualShowEditorsProperty;
		static readonly DependencyPropertyKey HSBColorPropertyKey;
		public static readonly DependencyProperty HSBColorProperty;
		static readonly DependencyPropertyKey ActualZThumbOffsetPropertyKey;
		public static readonly DependencyProperty ActualZThumbOffsetProperty;
		static readonly DependencyPropertyKey ActualXYThumbXOffsetPropertyKey;
		public static readonly DependencyProperty ActualXYThumbXOffsetProperty;
		static readonly DependencyPropertyKey ActualXYThumbYOffsetPropertyKey;
		public static readonly DependencyProperty ActualXYThumbYOffsetProperty;
		public static readonly RoutedEvent ColorChangedEvent;
		static ColorPicker() {
			Type ownerType = typeof(ColorPicker);
			EditModeProperty = DependencyPropertyManager.Register("EditMode", typeof(EditMode), ownerType,
				new PropertyMetadata(EditMode.Standalone, (obj, args) => ((ColorPicker)obj).OnEditModeChanged((EditMode)args.NewValue)));
			ColorProperty = DependencyPropertyManager.Register("Color", typeof(Color), ownerType,
				new FrameworkPropertyMetadata(Text2ColorHelper.DefaultColor, FrameworkPropertyMetadataOptions.None, (obj, args) => ((ColorPicker)obj).OnColorChangedInternal((Color)args.OldValue, (Color)args.NewValue)));
			DefaultColorProperty = DependencyPropertyManager.Register("DefaultColor", typeof(Color), ownerType,
				new FrameworkPropertyMetadata(Text2ColorHelper.DefaultColor));
			ShowDefaultColorProperty = DependencyPropertyManager.Register("ShowDefaultColor", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			ShowEditorsProperty = DependencyPropertyManager.Register("ShowEditors", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (obj, args) => ((ColorPicker)obj).OnShowEditorsChangedInternal((bool)args.OldValue, (bool)args.NewValue)));
			ColorModeProperty = DependencyPropertyManager.Register("ColorMode", typeof(ColorPickerColorMode), ownerType,
				new FrameworkPropertyMetadata(ColorPickerColorMode.RGB, FrameworkPropertyMetadataOptions.None, (obj, args) => ((ColorPicker)obj).OnColorModeChangedInternal((ColorPickerColorMode)args.OldValue, (ColorPickerColorMode)args.NewValue)));
			ColorViewModelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ColorViewModel", typeof(ColorBase), ownerType,
				new FrameworkPropertyMetadata(null));
			ColorViewModelProperty = ColorViewModelPropertyKey.DependencyProperty;
			ActualShowEditorsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowEditors", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, (obj, args) => ((ColorPicker)obj).OnActualShowEditorsChangedInternal((bool)args.OldValue, (bool)args.NewValue)));
			ActualShowEditorsProperty = ActualShowEditorsPropertyKey.DependencyProperty;
			HSBColorPropertyKey = DependencyPropertyManager.RegisterReadOnly("HSBColor", typeof(HSBColor), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (obj, args) => ((ColorPicker)obj).OnHSBColorChangedInternal((HSBColor)args.OldValue, (HSBColor)args.NewValue)));
			HSBColorProperty = HSBColorPropertyKey.DependencyProperty;
			ActualZThumbOffsetPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualZThumbOffset", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d));
			ActualZThumbOffsetProperty = ActualZThumbOffsetPropertyKey.DependencyProperty;
			ActualXYThumbXOffsetPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualXYThumbXOffset", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d));
			ActualXYThumbXOffsetProperty = ActualXYThumbXOffsetPropertyKey.DependencyProperty;
			ActualXYThumbYOffsetPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualXYThumbYOffset", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d));
			ActualXYThumbYOffsetProperty = ActualXYThumbYOffsetPropertyKey.DependencyProperty;
			ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChangedEvent", RoutingStrategy.Direct, typeof(ColorChangedEventArgs), ownerType);
		}
		void OnColorModeChangedInternal(ColorPickerColorMode oldValue, ColorPickerColorMode newValue) {
			OnColorModeChanged(oldValue, newValue);
		}
		void OnEditModeChanged(EditMode newValue) {
			ColorViewModel.Do(x => x.EditMode = newValue);
		}
		void OnShowEditorsChangedInternal(bool oldValue, bool newValue) {
			UpdateActualShowEditors();
			OnShowEditorsChanged(oldValue, newValue);
		}
		void OnActualShowEditorsChangedInternal(bool oldValue, bool newValue) {
			if (newValue)
				InitTooltips();
			OnActualShowEditorsChanged(oldValue, newValue);
		}
		void OnHSBColorChangedInternal(HSBColor oldValue, HSBColor newValue) {
			updateColorLocker.DoLockedActionIfNotLocked(UpdateColor);
			UpdateViewModelColor();
			updateThumbsLocker.DoIfNotLocked(UpdateThumbs);
		}
		void OnColorChangedInternal(Color oldValue, Color newValue) {
			updateColorLocker.DoLockedActionIfNotLocked(UpdateHSBColor);
			OnColorChanged(oldValue, newValue);
			RaiseColorChanged(newValue);
		}
		public EditMode EditMode {
			get { return (EditMode)GetValue(EditModeProperty); }
			set { SetValue(EditModeProperty, value); }
		}
		public Color Color {
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public Color DefaultColor {
			get { return (Color)GetValue(DefaultColorProperty); }
			set { SetValue(DefaultColorProperty, value); }
		}
		public bool ShowDefaultColor {
			get { return (bool)GetValue(ShowDefaultColorProperty); }
			set { SetValue(ShowDefaultColorProperty, value); }
		}
		public HSBColor HSBColor {
			get { return (HSBColor)GetValue(HSBColorProperty); }
			private set { this.SetValue(HSBColorPropertyKey, value); }
		}
		public double ActualZThumbOffset {
			get { return (double)GetValue(ActualZThumbOffsetProperty); }
			internal set { this.SetValue(ActualZThumbOffsetPropertyKey, value); }
		}
		public double ActualXYThumbXOffset {
			get { return (double)GetValue(ActualXYThumbXOffsetProperty); }
			internal set { this.SetValue(ActualXYThumbXOffsetPropertyKey, value); }
		}
		public double ActualXYThumbYOffset {
			get { return (double)GetValue(ActualXYThumbYOffsetProperty); }
			internal set { this.SetValue(ActualXYThumbYOffsetPropertyKey, value); }
		}
		public bool ShowEditors {
			get { return (bool)GetValue(ShowEditorsProperty); }
			set { SetValue(ShowEditorsProperty, value); }
		}
		public bool ActualShowEditors {
			get { return (bool)GetValue(ActualShowEditorsProperty); }
			private set { this.SetValue(ActualShowEditorsPropertyKey, value); }
		}
		public ColorPickerColorMode ColorMode {
			get { return (ColorPickerColorMode)GetValue(ColorModeProperty); }
			set { SetValue(ColorModeProperty, value); }
		}
		public ColorBase ColorViewModel {
			get { return (ColorBase)GetValue(ColorViewModelProperty); }
			private set { this.SetValue(ColorViewModelPropertyKey, value); }
		}
		public event RoutedEventHandler ColorChanged {
			add { AddHandler(ColorChangedEvent, value); }
			remove { RemoveHandler(ColorChangedEvent, value); }
		}
		public ICommand SetDefaultColorCommand { get; private set; }
		Canvas zColorArea;
		Canvas colorArea;
		DataContentPresenter editorsContentPresenter;
		TextEdit resultColorTextEdit;
		readonly Locker updateColorLocker = new Locker();
		readonly Locker updateThumbsLocker = new Locker();
		bool IsXYThumbMoving { get; set; }
		bool IsZThumbMoving { get; set; }
#if !SL
		Window transparentWindow;
		public ICommand EnablePippetModeCommand { get; private set; }
#endif
		object IColorEdit.EditValue { get { return Color; } set { } }
		PaletteCollection IColorEdit.Palettes { get { return null; } set { } }
		CircularList<Color> IColorEdit.RecentColors { get { return null; } }
		public ColorPicker() {
			this.SetDefaultStyleKey(typeof(ColorPicker));
			SizeChanged += OnSizeChanged;
			Loaded += OnLoaded;
#if !SL
			EnablePippetModeCommand = new DelegateCommand(EnablePippetMode);
#endif
			SetDefaultColorCommand = new DelegateCommand(SetDefaultColor);
			ColorViewModel = new RGBColor(Color);
			ColorViewModel.Do(x => x.ColorChanged += ColorViewModelColorChanged);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			UpdateHSBColor();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnsubscribeEvents();
			zColorArea = (Canvas)GetTemplateChild("PART_ZColorArea");
			colorArea = (Canvas)GetTemplateChild("PART_ColorArea");
			resultColorTextEdit = (TextEdit)GetTemplateChild("PART_ResultColor");
			editorsContentPresenter = (DataContentPresenter)GetTemplateChild("PART_EditorsContentPresenter");
			SubscribeEvents();
		}
		protected void RaiseColorChanged(Color color) {
			RaiseEvent(new ColorChangedEventArgs(color));
		}
		protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs args) {
			InitTooltips();
		}
		protected virtual void OnColorChanged(Color oldValue, Color newValue) {
		}
		protected virtual void OnShowEditorsChanged(bool oldValue, bool newValue) {
		}
		protected virtual void OnActualShowEditorsChanged(bool oldValue, bool newValue) {
		}
		protected virtual void OnColorModeChanged(ColorPickerColorMode oldValue, ColorPickerColorMode newValue) {
			ColorViewModel.Do(x => x.ColorChanged -= ColorViewModelColorChanged);
			switch (newValue) {
				case ColorPickerColorMode.RGB:
					ColorViewModel = new RGBColor(Color) { EditMode = EditMode };
					break;
				case ColorPickerColorMode.HSB:
					ColorViewModel = new HSBColor(Color) { EditMode = EditMode };
					break;
				case ColorPickerColorMode.HLS:
					ColorViewModel = new HLSColor(Color) { EditMode = EditMode };
					break;
				case ColorPickerColorMode.CMYK:
					ColorViewModel = new CMYKColor(Color) { EditMode = EditMode };
					break;
			}
			ColorViewModel.Do(x => x.ColorChanged += ColorViewModelColorChanged);
		}
		protected virtual void UnsubscribeEvents() {
			colorArea.Do(x => x.SizeChanged -= OnColorAreaSizeChanged);
			colorArea.Do(x => x.MouseLeftButtonDown -= OnColorAreaLeftMouseButtonDown);
			colorArea.Do(x => x.MouseLeftButtonUp -= OnColorAreaLeftMouseButtonUp);
			colorArea.Do(x => x.MouseMove -= OnColorAreaMouseMove);
			zColorArea.Do(x => x.SizeChanged -= OnZColorAreaSizeChanged);
			zColorArea.Do(x => x.MouseLeftButtonDown -= OnZColorAreaLeftMouseButtonDown);
			zColorArea.Do(x => x.MouseLeftButtonUp -= OnZColorAreaLeftMouseButtonUp);
			zColorArea.Do(x => x.MouseMove -= OnZColorAreaMouseMove);
		}
		protected virtual void SubscribeEvents() {
			colorArea.Do(x => x.SizeChanged += OnColorAreaSizeChanged);
			colorArea.Do(x => x.MouseLeftButtonDown += OnColorAreaLeftMouseButtonDown);
			colorArea.Do(x => x.MouseLeftButtonUp += OnColorAreaLeftMouseButtonUp);
			colorArea.Do(x => x.MouseMove += OnColorAreaMouseMove);
			zColorArea.Do(x => x.SizeChanged += OnZColorAreaSizeChanged);
			zColorArea.Do(x => x.MouseLeftButtonDown += OnZColorAreaLeftMouseButtonDown);
			zColorArea.Do(x => x.MouseLeftButtonUp += OnZColorAreaLeftMouseButtonUp);
			zColorArea.Do(x => x.MouseMove += OnZColorAreaMouseMove);
		}
		protected internal virtual void OnApplyTemplateInternal() {
			SetupFocusedEditor();
			InitTooltips();
		}
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
			SetupFocusedEditor();
		}
		public bool NeedsKey(Key key, ModifierKeys modifiers) {
			if (!ActualShowEditors)
				return true;
			if (key == Key.Tab)
				return !(ModifierKeysHelper.IsShiftPressed(modifiers) ? IsFirstEditor() : IsLastEditor());
			return true;
		}
		void IColorEdit.AddCustomColor(Color color) {
		}
		void SetDefaultColor() {
			Color = DefaultColor;
		}
		void ColorViewModelColorChanged(object sender, ColorViewModelValueChangedEventArgs e) {
			if(HSBColor == null || HSBColor.Color != e.Color)
				HSBColor = new HSBColor(e.Color);
		}
		bool IsFirstEditor() {
			IList<SpinEdit> editors = GetEditors();
			return editors.First().IsKeyboardFocusWithin;
		}
		bool IsLastEditor() {
			return resultColorTextEdit.IsKeyboardFocusWithin;
		}
		void FocusFirstEditor() {
			if (!ActualShowEditors)
				return;
			var editors = GetEditors();
			if (editors.Count != 0)
				editors.First().Focus();
		}
		IList<SpinEdit> GetEditors() {
			var editors = new List<SpinEdit>();
			if (ActualShowEditors) {
				LayoutHelper.FindElement(editorsContentPresenter, element => {
					if (element is SpinEdit)
						editors.Add((SpinEdit)element);
					return false;
				});
			}
			return editors;
		}
		void SetupFocusedEditor() {
#if !SL
			if (IsKeyboardFocused)
#else
			if (IsFocused)
#endif
				FocusFirstEditor();
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateActualShowEditors();
		}
		void InitTooltips() {
#if !SL
			if (!IsLoaded || EditMode == EditMode.InplaceInactive)
				return;
			IList<TextBlock> editors = new List<TextBlock>();
			LayoutHelper.ForEachElement(editorsContentPresenter, element => {
				if (element is TextBlock)
					editors.Add((TextBlock)element);
			});
			if (editors.Count == 0)
				return;
			switch (ColorMode) {
				case ColorPickerColorMode.RGB:
					editors[0].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerRed) });
					editors[1].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerGreen) });
					editors[2].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerBlue) });
					break;
				case ColorPickerColorMode.HSB:
					editors[0].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerHue) });
					editors[1].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerSaturation) });
					editors[2].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerBrightness) });
					break;
				case ColorPickerColorMode.HLS:
					editors[0].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerHue) });
					editors[1].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerLightness) });
					editors[2].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerSaturation) });
					break;
				case ColorPickerColorMode.CMYK:
					editors[0].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerCyan) });
					editors[1].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerMagenta) });
					editors[2].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerYellow) });
					editors[3].SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerKeyColor) });
					break;
			}
			editors.Last().SetToolTip(new ToolTip() { Content = EditorLocalizer.GetString(EditorStringId.ColorPickerAlpha) });
#endif
		}
		void UpdateActualShowEditors() {
			ActualShowEditors = ActualWidth >= MinWidthWithEditors && ShowEditors;
		}
		void OnColorAreaLeftMouseButtonDown(object sender, MouseButtonEventArgs e) {
			IsXYThumbMoving = true;
			colorArea.CaptureMouse();
			MoveXYThumb(e);
		}
		void OnColorAreaLeftMouseButtonUp(object sender, MouseButtonEventArgs e) {
			IsXYThumbMoving = false;
			colorArea.ReleaseMouseCapture();
		}
		void OnColorAreaMouseMove(object sender, MouseEventArgs e) {
			if (IsXYThumbMoving)
				MoveXYThumb(e);
		}
		void OnZColorAreaLeftMouseButtonDown(object sender, MouseButtonEventArgs e) {
			IsZThumbMoving = true;
			zColorArea.CaptureMouse();
			MoveZThumb(e);
		}
		void OnZColorAreaLeftMouseButtonUp(object sender, MouseButtonEventArgs e) {
			IsZThumbMoving = false;
			zColorArea.ReleaseMouseCapture();
		}
		void OnZColorAreaMouseMove(object sender, MouseEventArgs e) {
			if (IsZThumbMoving)
				MoveZThumb(e);
		}
		void MoveZThumb(MouseEventArgs e) {
			Point position = e.GetPosition(zColorArea);
			double y = Math.Max(Math.Min(position.Y, zColorArea.ActualHeight), 0d);
			ActualZThumbOffset = y - ZThumbHeight / 2d;
			updateThumbsLocker.DoLockedAction(UpdateColorFromThumbs);
		}
		void MoveXYThumb(MouseEventArgs e) {
			Point position = e.GetPosition(colorArea);
			double x = Math.Max(Math.Min(position.X, colorArea.ActualWidth), 0d);
			double y = Math.Max(Math.Min(position.Y, colorArea.ActualHeight), 0d);
			ActualXYThumbXOffset = x - XYThumbWidth / 2d;
			ActualXYThumbYOffset = y - XYThumbHeight / 2d;
			updateThumbsLocker.DoLockedAction(UpdateColorFromThumbs);
		}
		void OnColorAreaSizeChanged(object sender, SizeChangedEventArgs e) {
			updateThumbsLocker.DoLockedActionIfNotLocked(UpdateThumbs);
		}
		void OnZColorAreaSizeChanged(object sender, SizeChangedEventArgs e) {
			updateThumbsLocker.DoLockedActionIfNotLocked(UpdateThumbs);
		}
		void UpdateHSBColor() {
			HSBColor = new HSBColor(Color);
		}
		void UpdateColor() {
			Color = HSBColor.Color;
		}
		void UpdateViewModelColor() {
			ColorViewModel.Do(x => x.Color = HSBColor.Color);
		}
		void UpdateThumbs() {
			if (HSBColor == null || colorArea == null ||  zColorArea == null)
				return;
			if (colorArea.ActualWidth.IsZero() || colorArea.ActualHeight.IsZero() || zColorArea.ActualHeight.IsZero())
				return;
			double colorAreaWidth = colorArea.ActualWidth;
			double colorAreaHeight = colorArea.ActualHeight;
			double zColorAreaHeight = zColorArea.ActualHeight;
			ActualXYThumbXOffset = (HSBColor.S / 100d) * colorAreaWidth - XYThumbWidth / 2d;
			ActualXYThumbYOffset = (Math.Abs(100 - HSBColor.B) / 100d) * colorAreaHeight - XYThumbHeight / 2d;
			ActualZThumbOffset = (HSBColor.H / 360d) * zColorAreaHeight - ZThumbHeight / 2d;
		}
		internal void UpdateColorFromThumbs() {
			if (colorArea == null || colorArea.ActualWidth.IsZero() || colorArea.ActualHeight.IsZero() || zColorArea == null || zColorArea.ActualHeight.IsZero())
				return;
			double colorAreaWidth = colorArea.ActualWidth;
			double colorAreaHeight = colorArea.ActualHeight;
			double zColorAreaHeight = zColorArea.ActualHeight;
			int s = Convert.ToInt32(((XYThumbWidth / 2d + ActualXYThumbXOffset) / colorAreaWidth) * 100d);
			int b = Math.Abs(100 - Convert.ToInt32(((XYThumbHeight / 2d + ActualXYThumbYOffset) / colorAreaHeight) * 100d));
			int h = Convert.ToInt32(((ZThumbHeight / 2d + ActualZThumbOffset) / zColorAreaHeight) * 360d);
			HSBColor = new HSBColor(h, s, b, HSBColor.A);
		}
#if !SL
		void EnablePippetMode() {
			Rect screenRectUnion = ScreenHelper.GetScreenRectsUnion();
			InitTransparentWindow((int)screenRectUnion.Width, (int)screenRectUnion.Height, (int)screenRectUnion.Left, (int)screenRectUnion.Top);
			UpdateScreenBitmap((int)(screenRectUnion.Width * DpiScaleX), (int)(screenRectUnion.Height * DpiScaleY), screenRectUnion.TopLeft);
			transparentWindow.Show();
			Mouse.Capture(transparentWindow);
		}
		void InitTransparentWindow(int width, int height, int left, int top) {
			if (transparentWindow == null)
				transparentWindow = new Window();
			transparentWindow.Width = width;
			transparentWindow.Height = height;
			transparentWindow.Background = new SolidColorBrush(Color.FromArgb(0x05, 0xff, 0xff, 0xff));
			transparentWindow.WindowStyle = WindowStyle.None;
			transparentWindow.AllowsTransparency = true;
			transparentWindow.Left = left;
			transparentWindow.Top = top;
			transparentWindow.Topmost = true;
			transparentWindow.ShowInTaskbar = false;
			transparentWindow.Focusable = false;
			transparentWindow.ShowActivated = false;
			transparentWindow.PreviewMouseDown += OnTransparentWindowMouseDown;
			transparentWindow.PreviewMouseMove += OnPreviewMouseMove;
		}
		void OnPreviewMouseMove(object sender, MouseEventArgs e) {
			Point point = e.GetPosition(this);
			Point screenPoint = PointToScreen(point);
			Color = GetColorFromPoint(NormalizePoint(screenPoint));
			e.Handled = true;
		}
		Point NormalizePoint(Point screenPoint) {
			Rect screenRectUnion = ScreenHelper.GetScreenRectsUnion();
			double x = screenPoint.X;
			double y = screenPoint.Y;
			if (screenRectUnion.Left.LessThan(0d))
				x += Math.Abs(screenRectUnion.Left);
			if (screenRectUnion.Top.LessThan(0d))
				y += Math.Abs(screenRectUnion.Top);
			return new Point(x, y);
		}
		void OnTransparentWindowMouseDown(object sender, MouseButtonEventArgs e) {
			transparentWindow.PreviewMouseDown -= OnTransparentWindowMouseDown;
			transparentWindow.PreviewMouseMove -= OnPreviewMouseMove;
			transparentWindow.Close();
			transparentWindow = null;
			Mouse.Capture(null);
			e.Handled = true;
		}
		System.Drawing.Bitmap screenBitmap;
		void UpdateScreenBitmap(int width, int height, Point topLeft) {
			screenBitmap = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(screenBitmap)) {
				gr.CopyFromScreen(new System.Drawing.Point((int)topLeft.X, (int)topLeft.Y), System.Drawing.Point.Empty, new System.Drawing.Size(width, height));
			}
		}
		Color GetColorFromPoint(Point point) {
			System.Drawing.Color result = screenBitmap.GetPixel((int)point.X, (int)point.Y);
			return Color.FromArgb(result.A, result.R, result.G, result.B);
		}
#endif
	}
	public class ColorChangedEventArgs : RoutedEventArgs {
		public Color Color { get; private set; }
		public ColorChangedEventArgs(Color color)
			: base(ColorPicker.ColorChangedEvent) {
			Color = color;
		}
	}
}
