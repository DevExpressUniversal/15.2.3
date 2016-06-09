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
using DevExpress.Map;
using DevExpress.Map.Native;
using System.ComponentModel;
namespace DevExpress.XtraMap {
	public class LinearRangeDistribution : RangeDistributionBase {
		static readonly LinearRangeDistribution defaultDistribution = new LinearRangeDistribution();
#if !SL
	[DevExpressXtraMapLocalizedDescription("LinearRangeDistributionDefault")]
#endif
		public static LinearRangeDistribution Default { get { return defaultDistribution; }  }
		public override double ConvertRangeValue(double min, double max, double value) {
			return RangeDistributionHelper.CalcLinearRangeValue(min, max, value);
		}
		public override string ToString() {
			return "(LinearRangeDistribution)";
		}
	}
	public abstract class EquationRangeDistribution : RangeDistributionBase {
		double factor;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("EquationRangeDistributionFactor"),
#endif
		DefaultValue(.0)]
		public double Factor {
			get { return factor; }
			set { factor = value; }
		}
	}
	public class ExponentialRangeDistribution : EquationRangeDistribution {
		public override double ConvertRangeValue(double min, double max, double value) {
			return RangeDistributionHelper.CalcExponentialRangeValue(min, max, value, Factor);
		}
		public override string ToString() {
			return "(ExponentialRangeDistribution)";
		}
	}
	public class LogarithmicRangeDistribution : EquationRangeDistribution {
		public override double ConvertRangeValue(double min, double max, double value) {
			return RangeDistributionHelper.CalcLogarithmicRangeValue(min, max, value, Factor);
		}
		public override string ToString() {
			return "(LogarithmicRangeDistribution)";
		}
	}
}
