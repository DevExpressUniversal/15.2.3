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
using System.Windows.Data;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum CandleStickElements {
		TopStick,
		BottomStick,
		Candle,
		InvertedCandle
	}
	public class CandleStick2DModelPanel : Panel, IFinishInvalidation {
		public static readonly DependencyProperty ElementsProperty = DependencyPropertyManager.RegisterAttached("Elements",
			typeof(CandleStickElements), typeof(CandleStick2DModelPanel));
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty PointLayoutProperty = DependencyPropertyManager.Register("PointLayout",
			typeof(SeriesPointLayout), typeof(CandleStick2DModelPanel), new PropertyMetadata(null, PointLayoutPropertyChanged));
		public CandleStick2DModelPanel() {
			Binding binding = new Binding() {
				Path = new PropertyPath("Layout")
			};
			SetBinding(PointLayoutProperty, binding);
		}
		public static void SetElements(UIElement element, CandleStickElements value) {
			element.SetValue(ElementsProperty, value);
		}
		[NonCategorized]
		public static CandleStickElements GetElements(UIElement element) {
			return (CandleStickElements)element.GetValue(ElementsProperty);
		}
		SeriesPointLayout PointLayout {
			get { return (SeriesPointLayout)GetValue(PointLayoutProperty); }
		}
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement child in Children) {
				if (PointLayout != null) {
					CandleStickElement candleStickElement = CandleStickElement.CreateInstance(GetElements(child), PointLayout as CandleStickSeries2DPointLayout);
					Rect elementRect = candleStickElement.CalculateArrangeRect(availableSize);
					child.Measure(new Size(elementRect.Width, elementRect.Height));
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
					CandleStickElement candleStickElement = CandleStickElement.CreateInstance(GetElements(child), PointLayout as CandleStickSeries2DPointLayout);
					Rect elementRect = candleStickElement.CalculateArrangeRect(finalSize);
					child.Arrange(elementRect);
				}
				else
					child.Arrange(RectExtensions.Zero);
			}
			return finalSize;
		}
		static void PointLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
			CandleStick2DModelPanel panel = (CandleStick2DModelPanel)d;
			foreach (UIElement child in panel.Children) {
				CandleStickElement candleStickElement = CandleStickElement.CreateInstance(GetElements(child), (CandleStickSeries2DPointLayout)e.NewValue);
				child.Visibility = candleStickElement.IsVisible ? Visibility.Visible : Visibility.Collapsed;
			}
		}
	}
}
