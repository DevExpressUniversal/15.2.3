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
using System.Collections.Generic;
using DevExpress.Compatibility.System;
namespace DevExpress.Charts.Native {
	public interface IDiagram {
		GRealRect2D ChartBounds { get; }		
	}
	public interface IXYDiagram : IDiagram {
		IList<IPane> Panes { get; }
		IAxisData AxisX { get; }
		IAxisData AxisY { get; }
		IEnumerable<IAxisData> SecondaryAxesX { get; }
		IEnumerable<IAxisData> SecondaryAxesY { get; }
		bool ScrollingEnabled { get; }
		bool Rotated { get; }
		ICrosshairOptions CrosshairOptions { get; }
		IList<IPane> GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync);
		InternalCoordinates MapPointToInternal(IPane pane, GRealPoint2D point);
		GRealPoint2D MapInternalToPoint(IPane pane, IAxisData axisX, IAxisData axisY, double argument, double value);
		List<IPaneAxesContainer> GetPaneAxesContainers(IList<RefinedSeries> activeSeries);		
		void UpdateCrosshairData(IList<RefinedSeries> seriesCollection);
		void UpdateAutoMeasureUnits();
		int GetAxisXLength(IAxisData axis);
	}
	public interface IIndicatorCalculator : IXYDiagram {
		void CalculateIndicators(IEnumerable<IRefinedSeries> activeSeries);
	}
	public interface ISwiftPlotDiagram : IXYDiagram {
	}
	public interface IPane {
		int PaneIndex { get; }
		GRealRect2D? MappingBounds { get; }
	}
	public interface IScaleMap {
		Transformation Transformation { get; }
		double NativeToInternal(object value);
		double NativeToRefined(object value);
		object InternalToNative(double value);
		double InternalToRefined(double value);
		double RefinedToInternal(double value);
		bool IsCompatible(object value);
	}
	public interface IAxisElementContainer {
		IEnumerable<IScaleBreak> ScaleBreaks { get; }
		IEnumerable<IConstantLine> ConstantLines { get; }
		IEnumerable<IStrip> Strips { get; }
		IEnumerable<ICustomAxisLabel> CustomLabels { get; }
	}
	public interface IAutoScaleBreaksContainer {
		bool Enabled { get; }
		void UpdateScaleBreaks(IList<IRefinedSeries> refinedSeries);
	}
	public interface IResolveLabelsOverlappingAxis {
		AxisLabelResolveOverlappingCache OverlappingCache { get; set; }
	}
	public interface ICrosshairAxis : IPatternHolder {
		string LabelPattern { get; }
		bool LabelVisible { get; }
	}
	public interface IAxisRange : ICloneable {
		object MinValue { get; }
		object MaxValue { get; }
		double MinValueInternal { get; }
		double MaxValueInternal { get; }
		void Assign(IAxisRange source);
		void Assign(IAxisRangeData source);
		void UpdateRange(object min, object max, double internalMin, double internalMax);
	}
	public enum CompatibleViewType {
		XYView,
		SimpleView,
		RadarView,
		PolarView,
		GanttView,
		SwiftPlotView
	}
	public interface IGeometryHolder {
		GeometryStripCreator CreateStripCreator();
	}
	public interface ICrosshairOptions {
		bool ShowOnlyInFocusedPane { get; }
		bool ShowArgumentLine { get; }
		bool ShowValueLine { get; }
		bool ShowGroupHeaders { get; }
		bool ShowTail { get; }
		string GroupHeaderPattern { get; }
		CrosshairSnapModeCore SnapMode { get; }
		CrosshairLabelModeCore LabelMode { get; }
		ICrosshairFreePosition LabelPosition { get; }
	}
	public enum DockCornerCore {
		TopRight,
		TopLeft,
		BottomRight,
		BottomLeft,
	}
	public interface ICrosshairFreePosition {
		bool IsMousePosition { get; }
		GRealRect2D DockBounds { get; }
		DockCornerCore DockCorner { get; }
		GRealPoint2D Offset { get; }
	}
}
