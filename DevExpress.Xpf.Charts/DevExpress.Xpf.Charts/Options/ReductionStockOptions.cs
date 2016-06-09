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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum StockLevel {
		LowValue,
		HighValue,
		OpenValue,
		CloseValue
	}
	public class ReductionStockOptions : ChartDependencyObject {
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(SolidColorBrush), typeof(ReductionStockOptions), new PropertyMetadata(defaultBrush, NotifyPropertyChanged));
		public static readonly DependencyProperty LevelProperty = DependencyPropertyManager.Register("Level",
			typeof(StockLevel), typeof(ReductionStockOptions), new PropertyMetadata(StockLevel.CloseValue, NotifyPropertyChanged));
		public static readonly DependencyProperty EnabledProperty = DependencyPropertyManager.Register("Enabled",
			typeof(bool), typeof(ReductionStockOptions), new PropertyMetadata(true, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ReductionStockOptionsBrush"),
#endif
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public SolidColorBrush Brush {
			get { return (SolidColorBrush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ReductionStockOptionsLevel"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public StockLevel Level {
			get { return (StockLevel)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ReductionStockOptionsEnabled"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Enabled {
			get { return (bool)GetValue(EnabledProperty); }
			set { SetValue(EnabledProperty, value); }
		}
		static readonly SolidColorBrush defaultBrush = Brushes.Red;
		internal SolidColorBrush ActualBrush { get { return Brush != null ? Brush : defaultBrush; } }
		internal void Assign(ReductionStockOptions reductionOptions) {
			if (reductionOptions != null) {
				if (CopyPropertyValueHelper.IsValueSet(reductionOptions, BrushProperty))
					if (CopyPropertyValueHelper.VerifyValues(this, reductionOptions, BrushProperty))
						Brush = reductionOptions.Brush.CloneCurrentValue();
				CopyPropertyValueHelper.CopyPropertyValue(this, reductionOptions, LevelProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, reductionOptions, EnabledProperty);
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new ReductionStockOptions();
		}
	}
}
