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

using System.Windows;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public class Format : Freezable {
		public Format() {
			FormatMask = ConditionalFormatMask.None;
			TextDecorations = new TextDecorationCollection();
		}
		[XtraSerializableProperty]
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Format), CreateMetadata(ConditionalFormatMask.Background));
		[XtraSerializableProperty]
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(Format), CreateMetadata(ConditionalFormatMask.Foreground));
		[XtraSerializableProperty]
		public double FontSize {
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}
		public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register("FontSize", typeof(double), typeof(Format), CreateMetadata(ConditionalFormatMask.FontSize));
		[XtraSerializableProperty]
		public FontStyle FontStyle {
			get { return (FontStyle)GetValue(FontStyleProperty); }
			set { SetValue(FontStyleProperty, value); }
		}
		public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register("FontStyle", typeof(FontStyle), typeof(Format), CreateMetadata(ConditionalFormatMask.FontStyle));
		[XtraSerializableProperty]
		public FontFamily FontFamily {
			get { return (FontFamily)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}
		public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(Format), CreateMetadata(ConditionalFormatMask.FontFamily));
		[XtraSerializableProperty]
		public FontStretch FontStretch {
			get { return (FontStretch)GetValue(FontStretchProperty); }
			set { SetValue(FontStretchProperty, value); }
		}
		public static readonly DependencyProperty FontStretchProperty = DependencyProperty.Register("FontStretch", typeof(FontStretch), typeof(Format), CreateMetadata(ConditionalFormatMask.FontStretch));
		[XtraSerializableProperty]
		public FontWeight FontWeight {
			get { return (FontWeight)GetValue(FontWeightProperty); }
			set { SetValue(FontWeightProperty, value); }
		}
		public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(Format), CreateMetadata(ConditionalFormatMask.FontWeight));
		[XtraSerializableProperty]
		public TextDecorationCollection TextDecorations {
			get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
			set { SetValue(TextDecorationsProperty, value); }
		}
		public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(Format), CreateMetadata(ConditionalFormatMask.TextDecorations));
		[XtraSerializableProperty]
		public ImageSource Icon {
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(Format), CreateMetadata(ConditionalFormatMask.DataBarOrIcon));
		[XtraSerializableProperty]
		public VerticalAlignment IconVerticalAlignment {
			get { return (VerticalAlignment)GetValue(IconVerticalAlignmentProperty); }
			set { SetValue(IconVerticalAlignmentProperty, value); }
		}
		public static readonly DependencyProperty IconVerticalAlignmentProperty =
			DependencyProperty.Register("IconVerticalAlignment", typeof(VerticalAlignment), typeof(Format), new PropertyMetadata(VerticalAlignment.Center));
		static PropertyMetadata CreateMetadata(ConditionalFormatMask flag) {
			return new PropertyMetadata((d, e) => ((Format)d).UpdateFormatMask(e, flag));
		}
		protected override Freezable CreateInstanceCore() {
			return new Format();
		}
		internal ConditionalFormatMask FormatMask { get; private set; }
		void UpdateFormatMask(DependencyPropertyChangedEventArgs e, ConditionalFormatMask flag) {
			if(IsMaskedPropertyAssigned(e.Property))
				FormatMask |= flag;
			else
				FormatMask &= ~flag;
		}
		bool IsMaskedPropertyAssigned(DependencyProperty property) {
			if(property == TextDecorationsProperty)
				return TextDecorations != null && TextDecorations.Count > 0;
			return this.IsPropertyAssigned(property);
		}
	}
}
