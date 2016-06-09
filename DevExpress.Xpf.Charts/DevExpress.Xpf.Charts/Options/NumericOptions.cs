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
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum NumericFormat {
		General = NumericOptionsFormat.General,
		Scientific = NumericOptionsFormat.Scientific,
		FixedPoint = NumericOptionsFormat.FixedPoint,
		Currency = NumericOptionsFormat.Currency,
		Percent = NumericOptionsFormat.Percent,
		Number = NumericOptionsFormat.Number
	}
	public class NumericOptions : ChartDependencyObject, INumericOptions {
		public static readonly DependencyProperty FormatProperty = DependencyPropertyManager.Register("Format",
			typeof(NumericFormat), typeof(NumericOptions), new PropertyMetadata(NumericFormat.General, NotifyPropertyChanged));
		public static readonly DependencyProperty PrecisionProperty = DependencyPropertyManager.Register("Precision",
			typeof(int), typeof(NumericOptions), new PropertyMetadata(2, NotifyPropertyChanged), ValidatePrecision);
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NumericOptionsFormat"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public NumericFormat Format {
			get { return (NumericFormat)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("NumericOptionsPrecision"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int Precision {
			get { return (int)GetValue(PrecisionProperty); }
			set { SetValue(PrecisionProperty, value); }
		}
		static bool ValidatePrecision(object value) {
			return (int)value >= 0;
		}
		#region INumericOptions implementation
		NumericOptionsFormat INumericOptions.Format { get { return (NumericOptionsFormat)Format; } }
		#endregion
		internal void Assign(NumericOptions numericOptions) {
			if (numericOptions != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, numericOptions, FormatProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, numericOptions, PrecisionProperty);
			}
		}
		protected override ChartDependencyObject CreateObject() {
			return new NumericOptions();
		}
	}
}
