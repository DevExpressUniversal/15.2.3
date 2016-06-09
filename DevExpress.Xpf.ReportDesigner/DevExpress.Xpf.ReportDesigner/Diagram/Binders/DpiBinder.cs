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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Reports.UserDesigner.Layout.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram.Binders {
	public static class DpiBinder {
		public static void BindScalar(DiagramItemLayout layout, DependencyProperty dpiProperty, DependencyProperty floatProperty, DependencyProperty doubleProperty) {
			Bind(new ScalarConverter(), layout, dpiProperty, floatProperty, doubleProperty);
		}
		public static void BindPoint(DiagramItemLayout layout, DependencyProperty dpiProperty, DependencyProperty floatProperty, DependencyProperty doubleProperty) {
			Bind(new PointConverter(), layout, dpiProperty, floatProperty, doubleProperty);
		}
		public static void BindSize(DiagramItemLayout layout, DependencyProperty dpiProperty, DependencyProperty floatProperty, DependencyProperty doubleProperty) {
			Bind(new SizeConverter(), layout, dpiProperty, floatProperty, doubleProperty);
		}
		static void Bind(IMultiValueConverter converter, DiagramItemLayout layout, DependencyProperty dpiProperty, DependencyProperty floatProperty, DependencyProperty doubleProperty) {
			var binding = new MultiBinding() { Converter = converter, Mode = BindingMode.TwoWay };
			binding.Bindings.Add(new Binding() { Path = new PropertyPath(dpiProperty), Source = layout, Mode = BindingMode.TwoWay });
			binding.Bindings.Add(new Binding() { Path = new PropertyPath(floatProperty), Source = layout, Mode = BindingMode.TwoWay });
			BindingOperations.SetBinding(layout, doubleProperty, binding);
		}
		class ScalarConverter : IMultiValueConverter {
			float dpi;
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
				dpi = (float)values[0];
				return BoundsConverter.ToDouble((float)values[1], dpi);
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
				return new object[] { dpi, BoundsConverter.ToFloat((double)value, dpi) };
			}
		}
		class PointConverter : IMultiValueConverter {
			float dpi;
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
				dpi = (float)values[0];
				return BoundsConverter.ToPoint((System.Drawing.PointF)values[1], dpi);
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
				return new object[] { dpi, BoundsConverter.ToPointF((Point)value, dpi) };
			}
		}
		class SizeConverter : IMultiValueConverter {
			float dpi;
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
				dpi = (float)values[0];
				return BoundsConverter.ToSize((System.Drawing.SizeF)values[1], dpi);
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
				return new object[] { dpi, BoundsConverter.ToSizeF((Size)value, dpi) };
			}
		}
	}
}
