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
using System.Windows.Input;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public enum XYDiagram2DScrollOrientation {
		AxisXScroll = ScrollingOrientation.AxisXScroll,
		AxisYScroll = ScrollingOrientation.AxisYScroll,
		BothAxesScroll = ScrollingOrientation.BothAxesScroll
	}
	public enum XYDiagram2DScrollEventType {
		LargeDecrement = NavigationType.LargeDecrement,
		LargeIncrement = NavigationType.LargeIncrement,
		SmallDecrement = NavigationType.SmallDecrement,
		SmallIncrement = NavigationType.SmallIncrement,
		ThumbPosition = NavigationType.ThumbPosition,
		LeftButtonMouseDrag = NavigationType.LeftButtonMouseDrag,
		MiddleButtonMouseDrag = NavigationType.MiddleButtonMouseDrag,
		ArrowKeys = NavigationType.ArrowKeys,
		Command = NavigationType.Command
	}
	public enum XYDiagram2DZoomEventType {
		SetRange = NavigationType.SetRange,
		ZoomIn = NavigationType.ZoomIn,
		ZoomOut = NavigationType.ZoomOut,
		ZoomUndo = NavigationType.ZoomUndo,
		Command = NavigationType.Command
	}
	public sealed class AxisRangePositions {
		readonly NavigationType navigationType;
		readonly double position1;
		readonly double position2;
		internal NavigationType NavigationType { get { return navigationType; } }
		public double Position1 { get { return position1; } }
		public double Position2 { get { return position2; } }
		internal AxisRangePositions(NavigationType navigationType, double position1, double position2) {
			this.navigationType = navigationType;
			this.position1 = position1;
			this.position2 = position2;
		}
		public AxisRangePositions(double position1, double position2) : this(NavigationType.Command, position1, position2) {
		}
	}
	public struct RangeInfo {
		readonly double min;
		readonly double max;
		readonly object minValue;
		readonly object maxValue;
		public double Min { get { return min; } }
		public double Max { get { return max; } }
		public object MinValue { get { return minValue; } }
		public object MaxValue { get { return maxValue; } }
		internal RangeInfo(AxisRangeInfo info) {
			min = info.Min;
			max = info.Max;
			minValue = info.MinValue;
			maxValue = info.MaxValue;
		}
	}
	public class XYDiagram2DScrollEventArgs : RoutedEventArgs {
		readonly XYDiagram2DScrollOrientation scrollOrientation;
		readonly XYDiagram2DScrollEventType type;
		readonly Pane pane;
		readonly AxisX2D axisX;
		readonly AxisY2D axisY;
		RangeInfo oldXRange;
		RangeInfo oldYRange;
		RangeInfo newXRange;
		RangeInfo newYRange;
		public XYDiagram2DScrollOrientation ScrollOrientation { get { return scrollOrientation; } }
		public XYDiagram2DScrollEventType Type { get { return type; } }
		public Pane Pane { get { return pane; } }
		public AxisX2D AxisX { get { return axisX; } }
		public AxisY2D AxisY { get { return axisY; } }
		[Obsolete("This property is obsolete now. Use OldXRange instead."),]
		public AxisRange OldAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use OldYRange instead."),]
		public AxisRange OldAxisYRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewXRange instead."),]
		public AxisRange NewAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewYRange instead."),]
		public AxisRange NewAxisYRange { get { return null; } }
		public RangeInfo OldXRange { get { return oldXRange; } }
		public RangeInfo OldYRange { get { return oldYRange; } }
		public RangeInfo NewXRange { get { return newXRange; } }
		public RangeInfo NewYRange { get { return newYRange; } }
		internal XYDiagram2DScrollEventArgs(XYDiagram2DScrollOrientation scrollOrientation, XYDiagram2DScrollEventType type, Pane pane, AxisX2D axisX, AxisY2D axisY, 
			AxisRangeInfo oldAxisXRange, AxisRangeInfo oldAxisYRange, AxisRangeInfo newAxisXRange, AxisRangeInfo newAxisYRange) : base(XYDiagram2D.ScrollEvent) {
			this.scrollOrientation = scrollOrientation;
			this.type = type;
			this.pane = pane;
			this.axisX = axisX;
			this.axisY = axisY;
			this.oldXRange = new RangeInfo(oldAxisXRange);
			this.oldYRange = new RangeInfo(oldAxisYRange);
			this.newXRange = new RangeInfo(newAxisXRange);
			this.newYRange = new RangeInfo(newAxisYRange);
		}
	}
	public class XYDiagram2DZoomEventArgs : RoutedEventArgs {
		readonly XYDiagram2DZoomEventType type;
		readonly Pane pane;
		readonly AxisX2D axisX;
		readonly AxisY2D axisY;
		RangeInfo oldXRange;
		RangeInfo oldYRange;
		RangeInfo newXRange;
		RangeInfo newYRange;
		public XYDiagram2DZoomEventType Type { get { return type; } }
		public Pane Pane { get { return pane; } }
		public AxisX2D AxisX { get { return axisX; } }
		public AxisY2D AxisY { get { return axisY; } }
		[Obsolete("This property is obsolete now. Use OldXRange instead."),]
		public AxisRange OldAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use OldYRange instead."),]
		public AxisRange OldAxisYRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewXRange instead."),]
		public AxisRange NewAxisXRange { get { return null; } }
		[Obsolete("This property is obsolete now. Use NewYRange instead."),]
		public AxisRange NewAxisYRange { get { return null; } }
		public RangeInfo OldXRange { get { return oldXRange; } }
		public RangeInfo OldYRange { get { return oldYRange; } }
		public RangeInfo NewXRange { get { return newXRange; } }
		public RangeInfo NewYRange { get { return newYRange; } }
		internal XYDiagram2DZoomEventArgs(XYDiagram2DZoomEventType type, Pane pane, AxisX2D axisX, AxisY2D axisY, 
			AxisRangeInfo oldAxisXRange, AxisRangeInfo oldAxisYRange, AxisRangeInfo newAxisXRange, AxisRangeInfo newAxisYRange): base(XYDiagram2D.ZoomEvent) {
			this.type = type;
			this.pane = pane;
			this.axisX = axisX;
			this.axisY = axisY;
			this.oldXRange = new RangeInfo(oldAxisXRange);
			this.oldYRange = new RangeInfo(oldAxisYRange);
			this.newXRange = new RangeInfo(newAxisXRange);
			this.newYRange = new RangeInfo(newAxisYRange);
		}
	}
	public delegate void XYDiagram2DScrollEventHandler(object sender, XYDiagram2DScrollEventArgs e);
	public delegate void XYDiagram2DZoomEventHandler(object sender, XYDiagram2DZoomEventArgs e);
	public static class XYDiagram2DCommands {
		static readonly RoutedCommand scrollHorizontally = new RoutedCommand("ScrollHorizontally", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand scrollVertically = new RoutedCommand("ScrollVertically", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand scrollAxisXTo = new RoutedCommand("ScrollAxisXTo", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand scrollAxisYTo = new RoutedCommand("ScrollAxisYTo", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand setAxisXRange = new RoutedCommand("SetAxisXRange", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand setAxisYRange = new RoutedCommand("SetAxisYRange", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand setAxisXZoomRatio = new RoutedCommand("SetAxisXZoomRatio", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand setAxisYZoomRatio = new RoutedCommand("SetAxisYZoomRatio", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand zoomIntoRectangle = new RoutedCommand("ZoomIntoRectangle", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand zoomIn = new RoutedCommand("ZoomIn", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand zoomOut = new RoutedCommand("ZoomOut", typeof(XYDiagram2DCommands));
		static readonly RoutedCommand undoZoom = new RoutedCommand("UndoZoom", typeof(XYDiagram2DCommands));
		public static RoutedCommand ScrollHorizontally { get { return scrollHorizontally; } }
		public static RoutedCommand ScrollVertically { get { return scrollVertically; } }
		public static RoutedCommand ScrollAxisXTo { get { return scrollAxisXTo; } }
		public static RoutedCommand ScrollAxisYTo { get { return scrollAxisYTo; } }
		public static RoutedCommand SetAxisXRange { get { return setAxisXRange; } }
		public static RoutedCommand SetAxisYRange { get { return setAxisYRange; } }
		public static RoutedCommand SetAxisXZoomRatio { get { return setAxisXZoomRatio; } }
		public static RoutedCommand SetAxisYZoomRatio { get { return setAxisYZoomRatio; } }
		public static RoutedCommand ZoomIntoRectangle { get { return zoomIntoRectangle; } }
		public static RoutedCommand ZoomIn { get { return zoomIn; } }
		public static RoutedCommand ZoomOut { get { return zoomOut; } }
		public static RoutedCommand UndoZoom { get { return undoZoom; } }
	}
}
