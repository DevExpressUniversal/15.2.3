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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class FibonacciIndicator : FinancialIndicator, ISupportIndicatorLabel {
		public static readonly DependencyProperty ShowLevel23_6Property = DependencyPropertyManager.Register("ShowLevel23_6",
			typeof(bool), typeof(FibonacciIndicator), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty ShowLevel76_4Property = DependencyPropertyManager.Register("ShowLevel76_4",
			typeof(bool), typeof(FibonacciIndicator), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty LabelProperty = DependencyPropertyManager.Register("Label",
			typeof(IndicatorLabel), typeof(FibonacciIndicator), new PropertyMetadata(null, LabelChanged));
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowLevel23_6 {
			get { return (bool)GetValue(ShowLevel23_6Property); }
			set { SetValue(ShowLevel23_6Property, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowLevel76_4 {
			get { return (bool)GetValue(ShowLevel76_4Property); }
			set { SetValue(ShowLevel76_4Property, value); }
		}
		public FibonacciIndicator() {
			DefaultStyleKey = typeof(FibonacciIndicator);
		}
		#region ISupportIndicatorLabel implementation
		[Category(Categories.Elements)]
		public IndicatorLabel Label {
			get { return (IndicatorLabel)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}
		#endregion
		protected virtual IList<double> GetFibonacciLevels() {
			List<double> levels = new List<double>();
			if (ShowLevel23_6)
				levels.Add(0.236);
			levels.Add(0.382);
			levels.Add(0.5);
			levels.Add(0.618);
			if (ShowLevel76_4)
				levels.Add(0.764);
			return levels;
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			FibonacciIndicator fibonacciIndicator = indicator as FibonacciIndicator;
			if (fibonacciIndicator != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, fibonacciIndicator, ShowLevel23_6Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, fibonacciIndicator, ShowLevel76_4Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, fibonacciIndicator, LabelProperty);
			}
		}
		static void ActualLabelOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.ChangeOwnerAndUpdate(d, e, ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateXYDiagram2DItems);
		}
		static void LabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FibonacciIndicator indicator = d as FibonacciIndicator;
			if (indicator != null) {
				IndicatorLabel label = e.NewValue as IndicatorLabel;
				if (label != null) {
					label.ChangeOwner(((IOwnedElement)label).Owner, indicator);
					indicator.Item.Label = label;
				}
			}
			ChartElementHelper.Update(d, e);
		}
	}
}
