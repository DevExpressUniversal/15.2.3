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

namespace DevExpress.Charts.Native {
	public abstract class AxisGridMapping : IAxisGridMapping {
		readonly AxisScaleTypeMap map;
		readonly double offset;
		protected AxisScaleTypeMap Map {
			get {
				return map;
			}
		}
		public Transformation Transformation {
			get {
				return map.Transformation;
			}
		}
		public double Offset {
			get {
				return offset;
			}
		}
		public AxisGridMapping(AxisScaleTypeMap map, double offset) {
			this.map = map;
			this.offset = offset;
		}
		public bool IsCompatible(object value) {
			return map.IsCompatible(value);
		}
		public double NativeToInternal(object value) {
			return map.NativeToInternal(value);
		}
		public abstract double InternalToAligned(double internalValue);
		public abstract object InternalToNative(double alignedValue);
		public abstract double AlignedToInternal(double alignedValue);
	}
	public class AxisDateTimeGridMapping : AxisGridMapping {
		readonly DateTimeGridAlignmentNative alignment;
		protected new AxisDateTimeMap Map {
			get {
				return (AxisDateTimeMap)base.Map;
			}
		}
		public AxisDateTimeGridMapping(AxisDateTimeMap map, DateTimeGridAlignmentNative alignment, double offset) : base(map, offset) {
			this.alignment = alignment;
		}
		public override double InternalToAligned(double internalValue) {
			if (internalValue < 0)
				return -Map.RefinedToInternal(Map.NativeToRefined(Map.InternalToNative(-internalValue)), (DateTimeMeasureUnitNative)alignment);
			return Map.RefinedToInternal(Map.NativeToRefined(Map.InternalToNative(internalValue)), (DateTimeMeasureUnitNative)alignment);
		}
		public override object InternalToNative(double alignedValue) {
			return Map.InternalToNative(alignedValue);
		}
		public override double AlignedToInternal(double alignedValue) {
			if (alignedValue < 0)
				return -Map.NativeToInternal(Map.InternalToNative(-alignedValue, (DateTimeMeasureUnitNative)alignment));
			return Map.NativeToInternal(Map.InternalToNative(alignedValue, (DateTimeMeasureUnitNative)alignment));
		}
	}
	public class AxisNumericGridMapping : AxisGridMapping {
		readonly double alignment;
		AxisNumericalMap NumericMap { get { return (AxisNumericalMap)Map; } }
		public AxisNumericGridMapping(AxisNumericalMap map, double alignment, double offset) : base(map, offset) {
			this.alignment = alignment;
		}
		public override double InternalToAligned(double internalValue) {
			if (NumericMap.MeasureUnit.HasValue)
				return internalValue / (alignment / NumericMap.MeasureUnit.Value);
			return internalValue / alignment;
		}
		public override object InternalToNative(double alignedValue) {
			return Map.InternalToNative(alignedValue);
		}
		public override double AlignedToInternal(double alignedValue) {
			if (NumericMap.MeasureUnit.HasValue)
				return alignedValue * (alignment / NumericMap.MeasureUnit.Value);
			return alignedValue * alignment;
		}
	}
	public class AxisQualitativeGridMapping : AxisGridMapping {
		public AxisQualitativeGridMapping(AxisQualitativeMap map, double offset) : base(map, offset) { }
		public override double InternalToAligned(double internalValue) {
			return internalValue;
		}
		public override object InternalToNative(double alignedValue) {
			string native = (string)Map.InternalToNative(alignedValue);
			if (string.IsNullOrEmpty(native))
				return null;
			return native;
		}
		public override double AlignedToInternal(double alignedValue) {
			return alignedValue;
		}
	}
}
