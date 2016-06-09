#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using DevExpress.DataAccess.Native.Data;
namespace DevExpress.DashboardCommon.Native {
	public class DecimalMinMaxCalculator { 
		readonly ReadOnlyPropertyDescriptor propertyDescriptor;
		readonly IList dataEngine;
		decimal absMinValue;
		decimal absMaxValue;
		decimal absRange;
		decimal? zeroPosition;
		bool alwaysShowZeroLevel;
		public decimal AbsoluteMinValue { 
			get {
				return absMinValue;
			}
		}
		public decimal AbsoluteMaxValue {
			get {
				return absMaxValue;
			}
		}
		public decimal AbsoluteRange {
			get {
				return absRange;
			}
		}
		public decimal AbsoluteZeroPosition {
			get {
				if(!zeroPosition.HasValue)
					CalculateAbsZeroPosition();
				return zeroPosition.Value;
			}
		}
		public DecimalMinMaxCalculator(ReadOnlyPropertyDescriptor propertyDescriptor, IList dataEngine) {
			this.propertyDescriptor = propertyDescriptor;
			this.dataEngine = dataEngine;
			zeroPosition = null;
			CalculateAbsMinMaxValue();
			CalculateAbsRange();
		}
		public DecimalMinMaxCalculator(ReadOnlyPropertyDescriptor propertyDescriptor, IList dataEngine, bool alwaysShowZeroLevel)
			: this(propertyDescriptor, dataEngine) {
			this.alwaysShowZeroLevel = alwaysShowZeroLevel;
		}
		public decimal NormalizeValue(decimal value) {
			decimal range = AbsoluteRange;
			if(range == decimal.Zero)
				range = decimal.One;
			decimal normalizedValue = value / range;
			if(!alwaysShowZeroLevel && Math.Sign(AbsoluteMinValue) == Math.Sign(AbsoluteMaxValue)) {
				const decimal startPercent = 0.15m;
				decimal min = AbsoluteMinValue;
				decimal max = AbsoluteMaxValue;
				decimal minAbs = Math.Abs(min);
				decimal maxAbs = Math.Abs(max);
				decimal minimum = Math.Min(minAbs, maxAbs);
				decimal maximum = Math.Max(minAbs, maxAbs);
				decimal delta = maximum - minimum;
				if(delta != decimal.Zero && (minimum / maximum) > startPercent) {
					decimal ratio = (1 - startPercent) / delta;
					normalizedValue = Math.Sign(value) * (startPercent + ratio * (Math.Abs(value) - minimum));
				}
			}
			return normalizedValue;
		}
		void CalculateAbsMinMaxValue() {
			decimal min;
			decimal max;
			absMinValue = decimal.MaxValue;
			absMaxValue = decimal.MinValue;
			for(int i = 0; i < dataEngine.Count; i++) {
				object value = propertyDescriptor.GetValue(dataEngine[i]);
				try {
					min = max = Convert.ToDecimal(value);
				}
				catch {
					min = 0;
					max = 0;
				}
				if(min < absMinValue)
					absMinValue = min;
				if(max > absMaxValue)
					absMaxValue = max;
			}
		}
		void CalculateAbsRange() {
			decimal min = AbsoluteMinValue;
			decimal minAbs = Math.Abs(min);
			decimal max = AbsoluteMaxValue;
			decimal maxAbs = Math.Abs(max);
			if(min < 0)
				if(max < 0)
					absRange = minAbs;
				else
					absRange = minAbs + maxAbs;
			else
				absRange = maxAbs;
		}
		void CalculateAbsZeroPosition() {
			decimal min = AbsoluteMinValue;
			decimal minAbs = Math.Abs(min);
			decimal max = AbsoluteMaxValue;
			if(min < 0)
				if(max < 0)
					zeroPosition = decimal.One;
				else{
					decimal range = AbsoluteRange;
					zeroPosition = range != decimal.Zero ? minAbs / range : decimal.Zero;
				}
			else
				zeroPosition = decimal.Zero;
		}
	}
}
