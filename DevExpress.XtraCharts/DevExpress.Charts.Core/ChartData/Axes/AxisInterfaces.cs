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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
namespace DevExpress.Charts.Native {
	public interface IAxisTitle {
		object Content { get; }
	}
	public enum RangeCorrectionMode {
		Auto,
		Values,
		InternalValues,
	}
	public enum SideMarginMode {
		Auto,
		Disable,
		Enable,
		UserDisable,
		UserEnable,
	}
	public interface IAxisRangeData : IMinMaxValues {
		bool Auto { get; }
		object MinValue { get; set; }
		object MaxValue { get; set; }
		double SideMarginsMin { get; set; }
		double SideMarginsMax { get; set; }
		bool AutoCorrectMin { get; set; }
		bool AutoCorrectMax { get; set; }
		double RefinedMin { get; set; }
		double RefinedMax { get; set; }
		RangeCorrectionMode CorrectionMode { get; set; }
		bool AlwaysShowZeroLevel { get; set; }
		SideMarginMode AutoSideMargins { get; set; }
		double SideMarginsValue { get; set; }
		void Reset(bool needUpdate);
		bool Contains(double value);
		void UpdateRange(object min, object max, double internalMin, double internalMax);
		void ApplyState(RangeSnapshot rangeSnapshot);
	}
	public interface IVisualAxisRangeData : IAxisRangeData {
		bool SynchronizeWithWholeRange { get; set; }
		void ScrollOrZoomRange(object min, object max, double internalMin, double internalMax);
		void ScrollOrZoomRange(object min, object max, double internalMin, double internalMax, bool notifyUpdate);
	}
	public interface IWholeAxisRangeData : IAxisRangeData {
	}
	public interface IAxisLabelFormatterCore {
		string GetAxisLabelText(object axisValue);
	}
	public interface IAxisData : IAxisElementContainer {
		bool IsArgumentAxis { get; }
		bool IsValueAxis { get; } 
		bool ShowLabels { get; }
		bool ShowMajorTickmarks { get; }
		bool ShowMinorTickmarks { get; }
		bool ShowMajorGridlines { get; }
		bool ShowMinorGridlines { get; }
		bool Interlaced { get; }
		bool IsVertical { get; }
		bool FixedRange { get; }
		bool Reverse { get; }
		bool IsRadarAxis { get; }
		bool IsDisposed { get; }
		IAxisLabel Label { get; }
		int GridSpacingFactor { get; }
		int MinorCount { get; }
		bool CanShowCustomWithAutoLabels { get; }
		AxisScaleTypeMap AxisScaleTypeMap { get; set; }
		IAxisRange Range { get; }
		IAxisRange ScrollingRange { get; }
		IVisualAxisRangeData VisualRange { get; }
		IWholeAxisRangeData WholeRange { get; }
		IAxisTitle Title { get; }
		INumericScaleOptions NumericScaleOptions { get; }
		IDateTimeScaleOptions DateTimeScaleOptions { get; }
		IScaleOptionsBase QualitativeScaleOptions { get; }
		IComparer QualitativeScaleComparer { get; }
		IAxisGridMapping GridMapping { get; }
		IAxisLabelFormatterCore LabelFormatter { get; set; }
		AxisVisibilityInPanes AxisVisibilityInPanes { get; }
		void Deserialize();
		void UpdateUserValues(); 
		void UpdateAutoMeasureUnit();
		RangeValue IncreaseRange(RangeValue range, bool applySideMargins);
		IList<IMinMaxValues> CalculateRangeLimitsList(double min, double max);	   
		GRealRect2D GetLabelBounds(IPane pane);
	}
	public interface IPaneAxesContainer {
		IAxisData PrimaryAxisX { get; }
		IAxisData PrimaryAxisY { get; }
		IList<IAxisData> SecondaryAxesX { get; }
		IList<IAxisData> SecondaryAxesY { get; }
	}
	public interface IAxisGridMapping {
		Transformation Transformation { get; }
		double Offset { get; }
		bool IsCompatible(object value);
		double InternalToAligned(double internalValue);
		double AlignedToInternal(double alignedValue);
		object InternalToNative(double alignedValue);
		double NativeToInternal(object value);
	}
}
