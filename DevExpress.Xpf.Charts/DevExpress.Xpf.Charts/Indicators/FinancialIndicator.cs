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
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public abstract class FinancialIndicator : Indicator {
		public static readonly DependencyProperty Argument1Property = DependencyPropertyManager.Register("Argument1", typeof(object), typeof(FinancialIndicator), new PropertyMetadata(null, ArgumentPropertyChanged));
		public static readonly DependencyProperty Argument2Property = DependencyPropertyManager.Register("Argument2", typeof(object), typeof(FinancialIndicator), new PropertyMetadata(null, ArgumentPropertyChanged));
		public static readonly DependencyProperty ValueLevel1Property = DependencyPropertyManager.Register("ValueLevel1", typeof(ValueLevel), typeof(FinancialIndicator), new PropertyMetadata(ValueLevel.Value, ChartElementHelper.Update));
		public static readonly DependencyProperty ValueLevel2Property = DependencyPropertyManager.Register("ValueLevel2", typeof(ValueLevel), typeof(FinancialIndicator), new PropertyMetadata(ValueLevel.Value, ChartElementHelper.Update));
		static void ArgumentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e);
		}
		[
		Category(Categories.Behavior),
		TypeConverter(typeof(ValueTypeConverter)),
		XtraSerializableProperty
		]
		public object Argument1 {
			get { return GetValue(Argument1Property); }
			set { SetValue(Argument1Property, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ValueLevel ValueLevel1 {
			get { return (ValueLevel)GetValue(ValueLevel1Property); }
			set { SetValue(ValueLevel1Property, value); }
		}
		[
		Category(Categories.Behavior),
		TypeConverter(typeof(ValueTypeConverter)),
		XtraSerializableProperty
		]
		public object Argument2 {
			get { return GetValue(Argument2Property); }
			set { SetValue(Argument2Property, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ValueLevel ValueLevel2 {
			get { return (ValueLevel)GetValue(ValueLevel2Property); }
			set { SetValue(ValueLevel2Property, value); }
		}
		protected override void Assign(Indicator indicator) {
			base.Assign(indicator);
			FinancialIndicator financialIndicator = indicator as FinancialIndicator;
			if (financialIndicator != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, financialIndicator, Argument1Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, financialIndicator, Argument2Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, financialIndicator, ValueLevel1Property);
				CopyPropertyValueHelper.CopyPropertyValue(this, financialIndicator, ValueLevel2Property);
			}
		}
	}
}
