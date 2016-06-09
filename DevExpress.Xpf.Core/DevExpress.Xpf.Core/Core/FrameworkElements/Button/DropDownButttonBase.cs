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
using DevExpress.Xpf.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Themes;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars;
using System.Collections;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Core {
	[ContentProperty("PopupContent")]
	public abstract class DropDownButtonBase : SimpleButton {
		static readonly Action<ButtonBase, bool> setIsPressed;		
		public static readonly DependencyProperty ArrowGlyphProperty;
		public static readonly DependencyProperty GlyphAlignmentProperty;
		public static readonly DependencyProperty ArrowAlignmentProperty;
		public static readonly DependencyProperty ContentAndGlyphPaddingProperty;
		public static readonly DependencyProperty ArrowPaddingProperty;
		public static readonly DependencyProperty IsMouseOverArrowProperty;		
		public static readonly DependencyProperty PopupContentProperty;
		public static readonly DependencyProperty PopupContentTemplateProperty;
		public static readonly DependencyProperty PopupContentTemplateSelectorProperty;
		public static readonly DependencyProperty IsPopupOpenProperty;
		public static readonly DependencyProperty PopupAutoWidthProperty;
		public static readonly DependencyProperty PopupWidthProperty;
		public static readonly DependencyProperty PopupMinWidthProperty;
		public static readonly DependencyProperty PopupMaxWidthProperty;
		public static readonly DependencyProperty PopupHeightProperty;
		public static readonly DependencyProperty PopupMinHeightProperty;
		public static readonly DependencyProperty PopupMaxHeightProperty;
		public static readonly DependencyProperty PopupDropAlignmentProperty;
		BarPopupBase actualPopup;
		readonly Locker clickLocker;
		readonly Locker popupClosingLocker;		
		protected internal bool IsPopupClosing { get { return popupClosingLocker.IsLocked; } }
		protected internal ButtonChrome ButtonChrome { get; private set; }
		protected internal abstract bool ActAsDropDown { get; }		
		static DropDownButtonBase() {
			ArrowGlyphProperty = DependencyProperty.Register("ArrowGlyph", typeof(ImageSource), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null));
			GlyphAlignmentProperty = DependencyProperty.Register("GlyphAlignment", typeof(Dock), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null));
			ArrowAlignmentProperty = DependencyProperty.Register("ArrowAlignment", typeof(Dock), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null));
			ContentAndGlyphPaddingProperty = DependencyProperty.Register("ContentAndGlyphPadding", typeof(Dock), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null));
			ArrowPaddingProperty = DependencyProperty.Register("ArrowPadding", typeof(Dock), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null));
			IsMouseOverArrowProperty = DependencyProperty.Register("IsMouseOverArrow", typeof(bool), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null));			
			PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(Object), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((DropDownButtonBase)d).UpdatePopup())));
			PopupContentTemplateProperty = DependencyProperty.Register("PopupContentTemplate", typeof(DataTemplate), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((DropDownButtonBase)d).UpdatePopup())));
			PopupContentTemplateSelectorProperty = DependencyProperty.Register("PopupContentTemplateSelector", typeof(DataTemplateSelector), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((DropDownButtonBase)d).UpdatePopup())));
			IsPopupOpenProperty = DependencyPropertyManager.Register("IsPopupOpen", typeof(bool), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((DropDownButtonBase)d).OnIsPopupOpenChanged((bool)e.OldValue, (bool)e.NewValue)), new CoerceValueCallback(CoerceIsPopupOpen)));
			PopupAutoWidthProperty = DependencyProperty.Register("PopupAutoWidth", typeof(bool), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(true));
			PopupWidthProperty = DependencyProperty.Register("PopupWidth", typeof(double), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(double.NaN, (d, e) => ((DropDownButtonBase)d).OnPopupPropertiesChanged((double)e.OldValue, (double)e.NewValue)));
			PopupMinWidthProperty = DependencyProperty.Register("PopupMinWidth", typeof(double), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(0.0, (d, e) => ((DropDownButtonBase)d).OnPopupPropertiesChanged((double)e.OldValue, (double)e.NewValue)));
			PopupMaxWidthProperty = DependencyProperty.Register("PopupMaxWidth", typeof(double), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(double.PositiveInfinity, (d, e) => ((DropDownButtonBase)d).OnPopupPropertiesChanged((double)e.OldValue, (double)e.NewValue)));
			PopupHeightProperty = DependencyProperty.Register("PopupHeight", typeof(double), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(double.NaN, (d, e) => ((DropDownButtonBase)d).OnPopupPropertiesChanged((double)e.OldValue, (double)e.NewValue)));
			PopupMinHeightProperty = DependencyProperty.Register("PopupMinHeight", typeof(double), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(0.0, (d, e) => ((DropDownButtonBase)d).OnPopupPropertiesChanged((double)e.OldValue, (double)e.NewValue)));
			PopupMaxHeightProperty = DependencyProperty.Register("PopupMaxHeight", typeof(double), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(double.PositiveInfinity, (d, e) => ((DropDownButtonBase)d).OnPopupPropertiesChanged((double)e.OldValue, (double)e.NewValue)));
			PopupDropAlignmentProperty = DependencyProperty.Register("PopupDropAlignment", typeof(PlacementMode), typeof(DropDownButtonBase), new FrameworkPropertyMetadata(PlacementMode.Bottom));			
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButtonBase), new FrameworkPropertyMetadata(typeof(DropDownButtonBase)));
			setIsPressed = ReflectionHelper.CreateInstanceMethodHandler<ButtonBase, Action<ButtonBase, bool>>(null, "SetIsPressed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		}
		static object CoerceIsPopupOpen(DependencyObject d, object baseValue) {
			DropDownButtonBase button = (DropDownButtonBase)d;
			bool current = (bool)baseValue;
			if (button.ActualPopup == null)
				current = false;
			return current;
		}
		static object IsPopupClosingAndLock(DependencyObject d, object value) {
			bool current = (bool)value;
			DropDownButtonBase control = (DropDownButtonBase)d;
			if (control.popupClosingLocker.IsLocked)
				return false;
			return current;
		}
		public DropDownButtonBase() {
			clickLocker = new Locker();
			popupClosingLocker = new Locker();
			IsPopupOpen = false;
		}
		public ImageSource ArrowGlyph {
			get { return (ImageSource)GetValue(ArrowGlyphProperty); }
			set { SetValue(ArrowGlyphProperty, value); }
		}
		public Dock GlyphAlignment {
			get { return (Dock)GetValue(GlyphAlignmentProperty); }
			set { SetValue(GlyphAlignmentProperty, value); }
		}
		public Dock ArrowAlignment {
			get { return (Dock)GetValue(ArrowAlignmentProperty); }
			set { SetValue(ArrowAlignmentProperty, value); }
		}
		public Dock ContentAndGlyphPadding {
			get { return (Dock)GetValue(ContentAndGlyphPaddingProperty); }
			set { SetValue(ContentAndGlyphPaddingProperty, value); }
		}
		public Dock ArrowPadding {
			get { return (Dock)GetValue(ArrowPaddingProperty); }
			set { SetValue(ArrowPaddingProperty, value); }
		}
		public bool IsMouseOverArrow {
			get { return (bool)GetValue(IsMouseOverArrowProperty); }
			set { SetValue(IsMouseOverArrowProperty, value); }
		}
		public Object PopupContent {
			get { return (Object)GetValue(PopupContentProperty); }
			set { SetValue(PopupContentProperty, value); }
		}
		public bool PopupAutoWidth {
			get { return (bool)GetValue(PopupAutoWidthProperty); }
			set { SetValue(PopupAutoWidthProperty, value); }
		}
		public double PopupWidth {
			get { return (double)GetValue(PopupWidthProperty); }
			set { SetValue(PopupWidthProperty, value); }
		}
		public double PopupMinWidth {
			get { return (double)GetValue(PopupMinWidthProperty); }
			set { SetValue(PopupMinWidthProperty, value); }
		}
		public double PopupMaxWidth {
			get { return (double)GetValue(PopupMaxWidthProperty); }
			set { SetValue(PopupMaxWidthProperty, value); }
		}
		public double PopupHeight {
			get { return (double)GetValue(PopupHeightProperty); }
			set { SetValue(PopupHeightProperty, value); }
		}
		public double PopupMinHeight {
			get { return (double)GetValue(PopupMinHeightProperty); }
			set { SetValue(PopupMinHeightProperty, value); }
		}
		public double PopupMaxHeight {
			get { return (double)GetValue(PopupMaxHeightProperty); }
			set { SetValue(PopupMaxHeightProperty, value); }
		}
		public PlacementMode PopupDropAlignment {
			get { return (PlacementMode)GetValue(PopupDropAlignmentProperty); }
			set { SetValue(PopupDropAlignmentProperty, value); }
		}
		public DataTemplate PopupContentTemplate {
			get { return (DataTemplate)GetValue(PopupContentTemplateProperty); }
			set { SetValue(PopupContentTemplateProperty, value); }
		}
		public DataTemplateSelector PopupContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PopupContentTemplateSelectorProperty); }
			set { SetValue(PopupContentTemplateSelectorProperty, value); }
		}
		protected internal BarPopupBase ActualPopup {
			get { return actualPopup; }
			set {
				if (actualPopup == value)
					return;
				var oldValue = actualPopup;
				actualPopup = value;
				OnActualPopupChanged(oldValue);
			}
		}
		public bool IsPopupOpen {
			get { return (bool)GetValue(IsPopupOpenProperty); }
			set { SetValue(IsPopupOpenProperty, value); }
		}
		protected override IEnumerator LogicalChildren {
			get { return new MergedEnumerator(base.LogicalChildren, new SingleObjectEnumerator(ActualPopup)); }
		}		
		void ActualPopupClosing(object sender, EventArgs e) {
			popupClosingLocker.Lock();
			this.IsPopupOpen = false;
			ButtonChrome.UpdateStates();
			popupClosingLocker.Unlock();
		}
		public override void OnApplyTemplate() {
			ButtonChrome = (ButtonChrome)GetTemplateChild("PART_Owner");
			ButtonChrome.Owner = this;
		}
		protected virtual void OnActualPopupChanged(BarPopupBase oldValue) {
			if (oldValue != null) {
				oldValue.Closed -= new EventHandler(ActualPopupClosing);
				RemoveLogicalChild(oldValue);
			}							
			if (ActualPopup != null) {
				ActualPopup.Closed += new EventHandler(ActualPopupClosing);
				AddLogicalChild(ActualPopup);
				ActualPopup.PlacementTarget = this;
				ActualPopup.Placement = PopupDropAlignment;
				UpdateActualPopupSize();
			}
		}		
		protected virtual void UpdatePopup() {
			if (PopupContent != null) {
				if (PopupContent is BarPopupBase) {
					ActualPopup = (BarPopupBase)Activator.CreateInstance(PopupContent.GetType());
					ActualPopup = (BarPopupBase)PopupContent;
				} else {
					ActualPopup = new PopupControlContainer();
					var content = new ContentControl();
					content.Content = PopupContent;
					content.ContentTemplate = PopupContentTemplate;
					((PopupControlContainer)ActualPopup).Content = content;
				}
			} else {
				ActualPopup = null;
			}
		}
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			UpdateActualPopupSize();
		}
		protected virtual void UpdateActualPopupSize() {
			if (ActualPopup == null)
				return;
			if (PopupAutoWidth) {
				var ui = ActualPopup.Child as PopupBorderControl;
				if (ui != null) {
					ui.ClearValue(PopupBorderControl.ContentMaxWidthProperty);
					ui.ClearValue(PopupBorderControl.ContentWidthProperty);
					(ui as PopupBorderControl).ContentMinWidth = ActualWidth;
				} else {
					ActualPopup.ClearValue(MaxWidthProperty);
					ActualPopup.ClearValue(WidthProperty);
					ActualPopup.MinWidth = ActualWidth;
				}					
			} else
				UpdatePopupWidth();			
			UpdatePopupHeight();
		}
		void UpdatePopupWidth() {
			var ui = ActualPopup.Child;
			if (ui is PopupBorderControl) {
				(ui as PopupBorderControl).ContentWidth = PopupWidth;
				(ui as PopupBorderControl).ContentMinWidth = PopupMinWidth;
				(ui as PopupBorderControl).ContentMaxWidth = PopupMaxWidth;
				return;
			}
			ActualPopup.Width = PopupWidth;
			ActualPopup.MinWidth = PopupMinWidth;
			ActualPopup.MaxWidth = PopupMaxWidth;
		}
		void UpdatePopupHeight() {
			var ui = ActualPopup.Child;
			if (ui is PopupBorderControl) {
				(ui as PopupBorderControl).ContentHeight = PopupHeight;
				(ui as PopupBorderControl).ContentMinHeight = PopupMinHeight;
				(ui as PopupBorderControl).ContentMaxHeight = PopupMaxHeight;
				return;
			}
			ActualPopup.Height = PopupHeight;
			ActualPopup.MinHeight = PopupMinHeight;
			ActualPopup.MaxHeight = PopupMaxHeight;
		}
		protected virtual void OnIsPopupOpenChanged(bool oldValue, bool newValue) {
			popupClosingLocker.DoIfNotLocked(new Action(() => ShowPopup(newValue)));		  
		}
		void ShowPopup(bool newValue) {
			if (newValue) {
				ButtonChrome.UpdateStates();
				var disp = NavigationTree.DisableMouseEventsProcessing();
				if (ActualPopup != null)
					ActualPopup.IsOpen = true;						   
				if (disp != null)
					Dispatcher.BeginInvoke(new Action(() => disp.Dispose()));
			}
		}
		protected virtual void OnPopupPropertiesChanged(double oldValue, double newValue) {
			UpdateActualPopupSize();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			if (ClickMode != ClickMode.Hover) {
				e.Handled = true;
				Focus();
				if (e.ButtonState == MouseButtonState.Pressed) {
					Mouse.Capture(this, CaptureMode.SubTree);
					if (IsMouseCaptured) {
						if (e.ButtonState == MouseButtonState.Pressed) {
							if (!IsPressed) 
								setIsPressed(this, true);
							if ((ButtonKind == ButtonKind.Repeat)&&(!ActAsDropDown)) {
								if (IsPressed) {
									StartTimer();
								}
							}
						} else 
							ReleaseMouseCapture();
					}
				}
				if (ClickMode == ClickMode.Press) {
					bool exceptionThrown = true;
					try {
						using (GetMouseClickLocker()) {
							OnClick();
						}						
						exceptionThrown = false;
					} finally {
						if (exceptionThrown) {
							setIsPressed(this, false);
							ReleaseMouseCapture();
						}
					}
				}
			}
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			using (GetMouseClickLocker()) {
				base.OnMouseEnter(e);
			}
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			using (GetMouseClickLocker()) {
				base.OnMouseLeftButtonUp(e);
				if (ButtonKind == ButtonKind.Repeat)
					StopTimer();
			}			
		}
		IDisposable GetMouseClickLocker() {
			if (ButtonChrome.ArrowPart.IsMouseOverCore)
				return clickLocker.Lock();
			return clickLocker;
		}
		protected override void OnClick() {
			clickLocker.DoIfNotLocked(base.OnClick);
		}
	}
	public class TemplateToImageSourceExtension : MarkupExtension {
		public ButtonsThemeKeys ResourceKey { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			var resourceKey = new ButtonsThemeKeyExtension() { ResourceKey = ResourceKey };
			var targetObject = (serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget).With(x => x.TargetObject);
			DataTemplate resource = null;
			if (targetObject != null) {
				var targetFE = targetObject as FrameworkElement;
				if (targetFE != null)
					resource = targetFE.TryFindResource(resourceKey) as DataTemplate;
				if (resource == null) {
					var targetFCE = targetObject as FrameworkContentElement;
					if (targetFCE != null)
						resource = targetFCE.TryFindResource(resourceKey) as DataTemplate;
				}
			}
			if (resource == null) {
				resource = Application.Current.With(x => x.TryFindResource(resourceKey)) as DataTemplate;
			}			
			if (resource == null)
				return null;
			ContentControl contentControl = new ContentControl();
			contentControl.Content = "";
			contentControl.ContentTemplate= resource;
			contentControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			contentControl.Arrange(new Rect(new Point(0,0), contentControl.DesiredSize));
			contentControl.UpdateLayout();
			VisualBrush brush = new VisualBrush(contentControl) { Stretch = Stretch.None, ViewboxUnits = BrushMappingMode.Absolute, ViewportUnits = BrushMappingMode.Absolute };
			brush.Viewbox = new Rect(contentControl.RenderSize);
			brush.Viewport = new Rect(contentControl.RenderSize);
			RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)(contentControl.RenderSize.Width*ScreenHelper.ScaleX), (int)(contentControl.RenderSize.Height*ScreenHelper.ScaleX), 
				ScreenHelper.CurrentDpi, ScreenHelper.CurrentDpi, PixelFormats.Pbgra32);
			targetBitmap.Render(contentControl);			
			return targetBitmap;
		}
	}
}
