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
using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public abstract class RangeDistributionBase : MapDependencyObject, IRangeDistribution {
		public abstract double ConvertRangeValue(double min, double max, double value);
	}
	public class LinearRangeDistribution : RangeDistributionBase {
		static readonly LinearRangeDistribution defaultDistribution = new LinearRangeDistribution();
		public static LinearRangeDistribution Default { get { return defaultDistribution; } }
		protected override MapDependencyObject CreateObject() {
			return new LinearRangeDistribution();
		}
		public override double ConvertRangeValue(double min, double max, double value) {
			return RangeDistributionHelper.CalcLinearRangeValue(min, max, value);
		}
	}
	public abstract class EquationRangeDistribution : RangeDistributionBase {
		public static readonly DependencyProperty FactorProperty = DependencyPropertyManager.Register("Factor",
			typeof(double), typeof(EquationRangeDistribution), new PropertyMetadata(1.0, NotifyPropertyChanged));
		[Category(Categories.Appearance)]
		public double Factor {
			get { return (double)GetValue(FactorProperty); }
			set { SetValue(FactorProperty, value); }
		}
	}
	public class ExponentialRangeDistribution : EquationRangeDistribution {
		protected override MapDependencyObject CreateObject() {
			return new ExponentialRangeDistribution();
		}
		public override double ConvertRangeValue(double min, double max, double value) {
			return RangeDistributionHelper.CalcExponentialRangeValue(min, max, value, Factor);
		}
	}
	public class LogarithmicRangeDistribution : EquationRangeDistribution {
		protected override MapDependencyObject CreateObject() {
			return new LogarithmicRangeDistribution();
		}
		public override double ConvertRangeValue(double min, double max, double value) {
			return RangeDistributionHelper.CalcLogarithmicRangeValue(min, max, value, Factor);
		}
	}
}
