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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class ColorLegend : ItemsLayerLegend {
		internal static readonly DependencyPropertyKey ColorLegendItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ColorLegendItems",
			typeof(ObservableCollection<ColorLegendItem>), typeof(ColorLegend), new PropertyMetadata(null));
		public static readonly DependencyProperty CustomItemsProperty = DependencyPropertyManager.Register("CustomItems",
			typeof(ColorLegendItemCollection), typeof(ColorLegend), new PropertyMetadata(CustomItemsPropertyChanged));
		[Category(Categories.Data)]
		public ColorLegendItemCollection CustomItems {
			get { return (ColorLegendItemCollection)GetValue(CustomItemsProperty); }
			set { SetValue(CustomItemsProperty, value); }
		}
		protected ColorLegend() {
			this.SetValue(CustomItemsProperty, new ColorLegendItemCollection());
		}
		protected internal override IEnumerable<MapLegendItemBase> CustomItemsInternal { get { return CustomItems; } }
	}
	public class ColorListLegend : ColorLegend {
		protected internal override bool ReverseItems { get { return true; } }
		public ColorListLegend() {
			DefaultStyleKey = typeof(ColorListLegend);
		}
	}
	public class ColorScaleLegend : ColorLegend {
		public ColorScaleLegend() {
			DefaultStyleKey = typeof(ColorScaleLegend);
		}
	}
	public class ColorScalePanel : Panel {
		double maxHeight = 0;
		double maxWidth = 32;
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement child in Children) {
				child.Measure(availableSize);
				maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
				maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
			}
			return new Size(maxWidth * Children.Count, maxHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (Children.Count > 0) {
				double itemWidth = finalSize.Width / Children.Count;
				double actualWidth = Math.Max(itemWidth, maxWidth);
				for (int i = 0; i < Children.Count; i++)
					Children[i].Arrange(new Rect(actualWidth * i, 0, actualWidth, Children[i].DesiredSize.Height));
			}
			return finalSize;
		}
	}
	public class TextToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(Visibility))
				if (value is string)
					return String.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
