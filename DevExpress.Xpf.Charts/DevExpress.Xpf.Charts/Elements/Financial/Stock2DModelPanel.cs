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
using System.Windows.Controls;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum StockElements {
		OpenLine,
		CloseLine,
		CenterLine
	}
	public class Stock2DModelPanel : Panel, IFinishInvalidation {
		public static readonly DependencyProperty ElementsProperty = DependencyPropertyManager.RegisterAttached("Elements", typeof(StockElements), typeof(Stock2DModelPanel));
		public static void SetElements(UIElement element, StockElements value) {
			element.SetValue(ElementsProperty, value);
		}
		[NonCategorized]
		public static StockElements GetElements(UIElement element) {
			return (StockElements)element.GetValue(ElementsProperty);
		}
		StockSeries2DPointLayout PointLayout {
			get {
				SeriesPointPresentationData presentationData = DataContext as SeriesPointPresentationData;
				return presentationData != null ? presentationData.Layout as StockSeries2DPointLayout : null;
			}
		}
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement child in Children) {
				if (PointLayout != null) {
					StockElement stockElement = StockElement.CreateInstance(GetElements(child), PointLayout);
					child.Measure(stockElement.CalculateMeasureSize(availableSize));
				}
				else
					child.Measure(new Size(0, 0));
			}
			Size constraint = new Size();
			constraint.Width = double.IsInfinity(availableSize.Width) ? 0 : availableSize.Width;
			constraint.Height = double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height;
			return constraint;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				if (PointLayout != null) {
					StockElement stockElement = StockElement.CreateInstance(GetElements(child), PointLayout);
					child.Visibility = stockElement.IsVisible ? Visibility.Visible : Visibility.Collapsed;
					child.Arrange(stockElement.CalculateArrangeRect(child, finalSize));
				}
				else
					child.Arrange(RectExtensions.Zero);
			}
			return finalSize;
		}
	}
}
