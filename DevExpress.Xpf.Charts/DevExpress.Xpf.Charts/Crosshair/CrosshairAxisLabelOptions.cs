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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class CrosshairAxisLabelOptions : ChartDependencyObject {
		readonly Brush crosshairLabelDefaultBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0x39, 0xCD));
		public static readonly DependencyProperty PatternProperty = DependencyPropertyManager.Register("Pattern",
			typeof(string), typeof(CrosshairAxisLabelOptions), new PropertyMetadata(""));
		public static readonly DependencyProperty VisibilityProperty = DependencyPropertyManager.Register("Visibility",
			typeof(bool?), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty BackgroundProperty = DependencyPropertyManager.Register("Background",
			typeof(Brush), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty ForegroundProperty = DependencyPropertyManager.Register("Foreground",
			typeof(Brush), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty FontWeightProperty = DependencyPropertyManager.Register("FontWeight",
			typeof(FontWeight), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty FontStyleProperty = DependencyPropertyManager.Register("FontStyle",
			typeof(FontStyle), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty FontStretchProperty = DependencyPropertyManager.Register("FontStretch",
			typeof(FontStretch), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty FontSizeProperty = DependencyPropertyManager.Register("FontSize",
			typeof(double), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		public static readonly DependencyProperty FontFamilyProperty = DependencyPropertyManager.Register("FontFamily",
			typeof(FontFamily), typeof(CrosshairAxisLabelOptions), new PropertyMetadata());
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public string Pattern {
			get { return (string)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}
		[
		Category(Categories.Behavior),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public bool? Visibility {
			get { return (bool?)GetValue(VisibilityProperty); }
			set { SetValue(VisibilityProperty, value); }
		}
		[
		Category(Categories.Brushes),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set {
				SetValue(BackgroundProperty, value);
			}
		}
		[
		Category(Categories.Brushes),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set {
				SetValue(ForegroundProperty, value);
			}
		}
		[
		Category(Categories.Text),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public FontFamily FontFamily {
			get { return (FontFamily)GetValue(FontFamilyProperty); }
			set {
				SetValue(FontFamilyProperty, value);
			}
		}
		[
		Category(Categories.Text),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public double FontSize {
			get { return (double)GetValue(FontSizeProperty); }
			set {
				SetValue(FontSizeProperty, value);
			}
		}
		[
		Category(Categories.Text),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public FontStretch FontStretch {
			get { return (FontStretch)GetValue(FontStretchProperty); }
			set {
				SetValue(FontStretchProperty, value);
			}
		}
		[
		Category(Categories.Text),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public FontStyle FontStyle {
			get { return (FontStyle)GetValue(FontStyleProperty); }
			set {
				SetValue(FontStyleProperty, value);
			}
		}
		[
		Category(Categories.Text),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public FontWeight FontWeight {
			get { return (FontWeight)GetValue(FontWeightProperty); }
			set {
				SetValue(FontWeightProperty, value);
			}
		}
		Axis2D owner;
		internal Axis2D Owner {
			get { return owner; }
			set { owner = value; }
		}
		internal Brush ActualBackground { 
			get {
				if (Background != null)
					return Background;
				else {
					Brush crosshairLineBrush = null;
					if (Owner != null)
						crosshairLineBrush = Owner.ActualCrosshairLineBrush;
					return crosshairLineBrush != null ? crosshairLineBrush : crosshairLabelDefaultBackground;
				}
			} 
		}
		internal Brush ActualForeground { get { return Foreground == null ? Brushes.White : Foreground; } }
		internal FontFamily ActualFontFamily { get { return FontFamily == null ? Owner.ActualLabel.FontFamily : FontFamily; } }
		internal double ActualFontSize { get { return FontSize == 0 ? Owner.ActualLabel.FontSize : FontSize; } }
		internal FontStretch ActualFontStretch { get { return FontStretch == FontStretches.Normal ? Owner.ActualLabel.FontStretch : FontStretch; } }
		internal FontStyle ActualFontStyle { get { return FontStyle == FontStyles.Normal ? Owner.ActualLabel.FontStyle : FontStyle; } }
		internal FontWeight ActualFontWeight { get { return FontWeight == FontWeights.Normal ? Owner.ActualLabel.FontWeight : FontWeight; } }
		protected override ChartDependencyObject CreateObject() {
			return new CrosshairAxisLabelOptions();
		}
	}
}
