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
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram.Binders {
	public static class AbsoluteBinder {
		public static void BindX(DiagramItemLayout layout, DependencyProperty localProperty, DependencyProperty absoluteProperty) {
			Bind(DiagramItemLayout.ParentLeftAbsoluteProperty, layout, localProperty, absoluteProperty);
		}
		public static void BindY(DiagramItemLayout layout, DependencyProperty localProperty, DependencyProperty absoluteProperty) {
			Bind(DiagramItemLayout.ParentTopAbsoluteProperty, layout, localProperty, absoluteProperty);
		}
		static void Bind(DependencyProperty parentProperty, DiagramItemLayout layout, DependencyProperty localProperty, DependencyProperty absoluteProperty) {
			var binding = new MultiBinding() { Converter = new Converter(), Mode = BindingMode.TwoWay };
			binding.Bindings.Add(new Binding() { Path = new PropertyPath(parentProperty), Source = layout, Mode = BindingMode.TwoWay });
			binding.Bindings.Add(new Binding() { Path = new PropertyPath(localProperty), Source = layout, Mode = BindingMode.TwoWay });
			BindingOperations.SetBinding(layout, absoluteProperty, binding);
		}
		class Converter : IMultiValueConverter {
			double parentAbsolute;
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
				parentAbsolute = (double)values[0];
				return parentAbsolute + (double)values[1];
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
				return new object[] { parentAbsolute, (double)value - parentAbsolute };
			}
		}
	}
}
