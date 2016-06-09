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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class PercentOptions : ChartDependencyObject {
		public static readonly DependencyProperty ValueAsPercentProperty = DependencyPropertyManager.Register("ValueAsPercent",
			typeof(bool), typeof(PercentOptions), new FrameworkPropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty PercentageAccuracyProperty = DependencyPropertyManager.Register("PercentageAccuracy",
			typeof(int), typeof(PercentOptions), new FrameworkPropertyMetadata(2, NotifyPropertyChanged), ValidatePercentageAccuracy);
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PercentOptionsValueAsPercent"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public bool ValueAsPercent {
			get { return (bool)GetValue(ValueAsPercentProperty); }
			set { SetValue(ValueAsPercentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PercentOptionsPercentageAccuracy"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int PercentageAccuracy {
			get { return (int)GetValue(PercentageAccuracyProperty); }
			set { SetValue(PercentageAccuracyProperty, value); }
		}
		static bool ValidatePercentageAccuracy(object value) {
			return (int)value > 0;
		}
		static bool ValidateValuePercentPrecision(object value) {
			return (int)value > 0;
		}
		internal double GetFullStackedPercentageValue(ISeries series, RefinedPoint point) {
			IFullStackedPoint fullStacked = (IFullStackedPoint)point;
			return Math.Round(fullStacked.NormalizedValue, PercentageAccuracy);
		}
		internal void Assign(PercentOptions percentOptions) {
			if (percentOptions != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, percentOptions, ValueAsPercentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, percentOptions, PercentageAccuracyProperty);
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new PercentOptions();
		}
	}
}
