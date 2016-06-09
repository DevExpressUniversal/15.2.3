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
	public class LineStyle : ChartDependencyObject {
		public static readonly DependencyProperty ThicknessProperty = DependencyPropertyManager.Register("Thickness",
			typeof(int), typeof(LineStyle), new PropertyMetadata(1, NotifyPropertyChanged), new ValidateValueCallback(ValidateThickness));
		public static readonly DependencyProperty DashCapProperty = DependencyPropertyManager.Register("DashCap",
			typeof(PenLineCap), typeof(LineStyle), new PropertyMetadata(PenLineCap.Square, NotifyPropertyChanged));
		public static readonly DependencyProperty LineJoinProperty = DependencyPropertyManager.Register("LineJoin",
			typeof(PenLineJoin), typeof(LineStyle), new PropertyMetadata(PenLineJoin.Miter, NotifyPropertyChanged));
		public static readonly DependencyProperty MiterLimitProperty = DependencyPropertyManager.Register("MiterLimit",
			typeof(double), typeof(LineStyle), new PropertyMetadata(10.0, NotifyPropertyChanged));
		public static readonly DependencyProperty DashStyleProperty = DependencyPropertyManager.Register("DashStyle",
			typeof(DashStyle), typeof(LineStyle), new PropertyMetadata(DashStyles.Solid, NotifyPropertyChanged));
		static bool ValidateThickness(object value) {
			return (int)value > 0;
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineStyleThickness"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public int Thickness {
			get { return (int)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineStyleDashCap"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public PenLineCap DashCap {
			get { return (PenLineCap)GetValue(DashCapProperty); }
			set { SetValue(DashCapProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineStyleLineJoin"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public PenLineJoin LineJoin {
			get { return (PenLineJoin)GetValue(LineJoinProperty); }
			set { SetValue(LineJoinProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineStyleMiterLimit"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public double MiterLimit {
			get { return (double)GetValue(MiterLimitProperty); }
			set { SetValue(MiterLimitProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LineStyleDashStyle"),
#endif
		Category(Categories.Common)
		]
		public DashStyle DashStyle {
			get { return (DashStyle)GetValue(DashStyleProperty); }
			set { SetValue(DashStyleProperty, value); }
		}		
		public LineStyle() {
		}
		public LineStyle(int thickness) {
			Thickness = thickness;
		}
		protected override ChartDependencyObject CreateObject() {
			return new LineStyle();
		}
		internal Pen CreatePen(Brush brush) {
			Pen pen = new Pen(brush, Thickness);
			pen.DashCap = DashCap;
			pen.DashStyle = DashStyle;
			pen.LineJoin = LineJoin;
			pen.MiterLimit = MiterLimit;
			return pen;
		}
	}   
}
