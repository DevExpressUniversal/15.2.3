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
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation.Internal {
	public class TileButtonControl : veContentContainer {
		#region static
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty HorizontalGlyphAlignmentProperty;
		public static readonly DependencyProperty VerticalGlyphAlignmentProperty;
		public static readonly DependencyProperty AllowGlyphThemingProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ActualGlyphColorProperty;
		static readonly DependencyPropertyKey ActualGlyphColorPropertyKey;
		public static readonly DependencyProperty GlyphColorProperty;
		static TileButtonControl() {
			Type ownerType = typeof(TileButtonControl);
			GlyphProperty = DependencyProperty.Register("Glyph", typeof(ImageSource), ownerType);
			HorizontalGlyphAlignmentProperty = DependencyProperty.Register("HorizontalGlyphAlignment", typeof(HorizontalAlignment), ownerType, new PropertyMetadata(HorizontalAlignment.Left));
			VerticalGlyphAlignmentProperty = DependencyProperty.Register("VerticalGlyphAlignment", typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Top));
			AllowGlyphThemingProperty = DependencyProperty.Register("AllowGlyphTheming", typeof(bool), ownerType);
			ActualGlyphColorPropertyKey = DependencyProperty.RegisterReadOnly("ActualGlyphColor", typeof(Color), ownerType, new PropertyMetadata(null));
			ActualGlyphColorProperty = ActualGlyphColorPropertyKey.DependencyProperty;
			GlyphColorProperty = DependencyProperty.Register("GlyphColor", typeof(Color), ownerType, new PropertyMetadata(Colors.Transparent, OnGlyphColorChanged));
		}
		private static void OnGlyphColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TileButtonControl)d).OnGlyphColorChanged((Color)e.OldValue, (Color)e.NewValue);
		}
		#endregion
		public TileButtonControl() {
			DefaultStyleKey = typeof(TileButtonControl);
			AttachPropertyListener("Foreground", ForegroundListener);
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public HorizontalAlignment HorizontalGlyphAlignment {
			get { return (HorizontalAlignment)GetValue(HorizontalGlyphAlignmentProperty); }
			set { SetValue(HorizontalGlyphAlignmentProperty, value); }
		}
		public VerticalAlignment VerticalGlyphAlignment {
			get { return (VerticalAlignment)GetValue(VerticalGlyphAlignmentProperty); }
			set { SetValue(VerticalGlyphAlignmentProperty, value); }
		}
		public bool AllowGlyphTheming {
			get { return (bool)GetValue(AllowGlyphThemingProperty); }
			set { SetValue(AllowGlyphThemingProperty, value); }
		}
		public Color GlyphColor {
			get { return (Color)GetValue(GlyphColorProperty); }
			set { SetValue(GlyphColorProperty, value); }
		}
		protected override void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) {
			base.OnPropertyChanged(propertyListener, oldValue, newValue);
			if(propertyListener == ForegroundListener) {
				UpdateActualGlyphColor();
			}
		}
		void OnGlyphColorChanged(Color color1, Color color2) {
			UpdateActualGlyphColor();
		}		
		void UpdateActualGlyphColor() {
			Color glyphColor = (Foreground as SolidColorBrush).Return(x => x.Color, () => GlyphColor);
			SetValue(ActualGlyphColorPropertyKey, glyphColor);
		}
	}
	public class TileArrowControl : ControlBase {
		#region static
		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(TileArrowControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsCheckedChanged)));
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ArrowDirectionProperty = DependencyProperty.Register("ArrowDirection", typeof(ButtonDirection), typeof(TileArrowControl), new UIPropertyMetadata(ButtonDirection.Down));
		private static void OnIsCheckedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			TileArrowControl tileArrowControl = o as TileArrowControl;
			if(tileArrowControl != null)
				tileArrowControl.OnIsCheckedChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		#endregion
		public TileArrowControl() {
			DefaultStyleKey = typeof(TileArrowControl);
		}
		protected virtual void OnIsCheckedChanged(bool oldValue, bool newValue) {
			Controller.UpdateState(false);
		}
		public new ClickableController Controller { get { return (ClickableController)base.Controller; } }
		protected override ControlControllerBase CreateController() {
			return new TileArrowControlController(this);
		}
		public event EventHandler Click {
			add { Controller.Click += value; }
			remove { Controller.Click -= value; }
		}
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			set { SetValue(IsCheckedProperty, value); }
		}
		internal ButtonDirection ArrowDirection {
			get { return (ButtonDirection)GetValue(ArrowDirectionProperty); }
			set { SetValue(ArrowDirectionProperty, value); }
		}
		class TileArrowControlController : ClickableController {
			public new TileArrowControl Control { get { return base.Control as TileArrowControl; } }
			public TileArrowControlController(IControl control)
				: base(control) {
				CaptureMouseOnDown = true;
			}
			public override void UpdateState(bool useTransitions) {
				base.UpdateState(useTransitions);
				VisualStateManager.GoToState(Control, "EmptyCheckedState", useTransitions);
				VisualStateManager.GoToState(Control, Control.IsChecked ? "Checked" : "Unchecked", useTransitions);
			}
		}
	}
}
