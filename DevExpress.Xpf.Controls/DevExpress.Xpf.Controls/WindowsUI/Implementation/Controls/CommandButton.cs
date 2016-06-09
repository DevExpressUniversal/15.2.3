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
using System.Windows.Controls;
using DevExpress.Xpf.WindowsUI.Base;
using System.Windows.Media;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.WindowsUI {
#if !SILVERLIGHT
#endif
	public class CommandButton : Button {
		#region static
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty AllowGlyphThemingProperty;
		static readonly DependencyPropertyKey IsGlyphVisiblePropertyKey;
		public static readonly DependencyProperty IsGlyphVisibleProperty;
		public static readonly DependencyProperty IsEllipseEnabledProperty;
		public static readonly DependencyProperty EllipseDiameterProperty;
		public static readonly DependencyProperty GlyphWidthProperty;
		public static readonly DependencyProperty GlyphHeightProperty;
		public static readonly DependencyProperty GlyphStretchProperty;
		public static readonly DependencyProperty StrokeThicknessProperty;
		static CommandButton() {
			var dProp = new DependencyPropertyRegistrator<CommandButton>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Glyph", ref GlyphProperty, (ImageSource)null,
				(d, e) => ((CommandButton)d).OnGlyphChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue));
			dProp.Register("AllowGlyphTheming", ref AllowGlyphThemingProperty, (bool)false);
			dProp.RegisterReadonly("IsGlyphVisible", ref IsGlyphVisiblePropertyKey, ref IsGlyphVisibleProperty, false);
			dProp.Register("IsEllipseEnabled", ref IsEllipseEnabledProperty, true, FrameworkPropertyMetadataOptions.AffectsMeasure);
			dProp.Register("EllipseDiameter", ref EllipseDiameterProperty, 42d);
			dProp.Register("GlyphWidth", ref GlyphWidthProperty, double.NaN);
			dProp.Register("GlyphHeight", ref GlyphHeightProperty, double.NaN);
			dProp.Register("GlyphStretch", ref GlyphStretchProperty, Stretch.Uniform);
			dProp.Register("StrokeThickness", ref StrokeThicknessProperty, 3d);
		}
		#endregion
		public CommandButton() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(CommandButton);
#endif
		}
		internal void OnClickInternal() {
			OnClick();
		}
		protected virtual void OnGlyphChanged(ImageSource oldValue, ImageSource newValue) {
			this.SetValue(IsGlyphVisiblePropertyKey, Glyph != null);
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		public bool AllowGlyphTheming {
			get { return (bool)GetValue(AllowGlyphThemingProperty); }
			set { SetValue(AllowGlyphThemingProperty, value); }
		}
		public bool IsGlyphVisible {
			get { return (bool)GetValue(IsGlyphVisibleProperty); }
		}
		public bool IsEllipseEnabled {
			get { return (bool)GetValue(IsEllipseEnabledProperty); }
			set { SetValue(IsEllipseEnabledProperty, value); }
		}
		public double EllipseDiameter {
			get { return (double)GetValue(EllipseDiameterProperty); }
			set { SetValue(EllipseDiameterProperty, value); }
		}
		public double GlyphWidth {
			get { return (double)GetValue(GlyphWidthProperty); }
			set { SetValue(GlyphWidthProperty, value); }
		}
		public double GlyphHeight {
			get { return (double)GetValue(GlyphHeightProperty); }
			set { SetValue(GlyphHeightProperty, value); }
		}
		public Stretch GlyphStretch {
			get { return (Stretch)GetValue(GlyphStretchProperty); }
			set { SetValue(GlyphStretchProperty, value); }
		}
		public double StrokeThickness {
			get { return (double)GetValue(StrokeThicknessProperty); }
			set { SetValue(StrokeThicknessProperty, value); }
		}
	}
}
