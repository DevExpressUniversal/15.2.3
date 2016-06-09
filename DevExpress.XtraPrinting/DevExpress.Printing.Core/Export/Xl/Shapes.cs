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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Implementation {
	#region XlDrawingObjectBase
	public abstract class XlDrawingObjectBase {
		public string Name { get; set; }
		public XlAnchorType AnchorType { get; set; }
		public XlAnchorType AnchorBehavior { get; set; }
		public XlAnchorPoint TopLeft { get; set; }
		public XlAnchorPoint BottomRight { get; set; }
		internal bool IsSingleCellAnchor() {
			return BottomRight.ColumnOffsetInPixels == 0 && BottomRight.RowOffsetInPixels == 0 && BottomRight.Equals(TopLeft);
		}
	}
	#endregion
	#region XlDrawingObject
	public abstract class XlDrawingObject : XlDrawingObjectBase {
		public void SetAbsoluteAnchor(int x, int y, int width, int height) {
			AnchorType = XlAnchorType.Absolute;
			TopLeft = new XlAnchorPoint(0, 0, x, y);
			BottomRight = new XlAnchorPoint(0, 0, x + width, y + height);
		}
		public void SetOneCellAnchor(XlAnchorPoint topLeft, int width, int height) {
			AnchorType = XlAnchorType.OneCell;
			TopLeft = topLeft;
			BottomRight = new XlAnchorPoint(topLeft.Column, topLeft.Row, topLeft.ColumnOffsetInPixels + width, topLeft.RowOffsetInPixels + height);
		}
		public void SetTwoCellAnchor(XlAnchorPoint topLeft, XlAnchorPoint bottomRight, XlAnchorType anchorBehavior) {
			AnchorType = XlAnchorType.TwoCell;
			AnchorBehavior = anchorBehavior;
			TopLeft = topLeft;
			BottomRight = bottomRight;
		}
	}
	#endregion
	#region XlGeometryPreset
	public enum XlGeometryPreset {
		None,
		Line,
		LineInv,
		Triangle,
		RtTriangle,
		Rect,
		Diamond,
		Parallelogram,
		Trapezoid,
		NonIsoscelesTrapezoid,
		Pentagon,
		Hexagon,
		Heptagon,
		Octagon,
		Decagon,
		Dodecagon,
		Star4,
		Star5,
		Star6,
		Star7,
		Star8,
		Star10,
		Star12,
		Star16,
		Star24,
		Star32,
		RoundRect,
		Round1Rect,
		Round2SameRect,
		Round2DiagRect,
		SnipRoundRect,
		Snip1Rect,
		Snip2SameRect,
		Snip2DiagRect,
		Plaque,
		Ellipse,
		Teardrop,
		HomePlate,
		Chevron,
		PieWedge,
		Pie,
		BlockArc,
		Donut,
		NoSmoking,
		RightArrow,
		LeftArrow,
		UpArrow,
		DownArrow,
		StripedRightArrow,
		NotchedRightArrow,
		BentUpArrow,
		LeftRightArrow,
		UpDownArrow,
		LeftUpArrow,
		LeftRightUpArrow,
		QuadArrow,
		LeftArrowCallout,
		RightArrowCallout,
		UpArrowCallout,
		DownArrowCallout,
		LeftRightArrowCallout,
		UpDownArrowCallout,
		QuadArrowCallout,
		BentArrow,
		UturnArrow,
		CircularArrow,
		LeftCircularArrow,
		LeftRightCircularArrow,
		CurvedRightArrow,
		CurvedLeftArrow,
		CurvedUpArrow,
		CurvedDownArrow,
		SwooshArrow,
		Cube,
		Can,
		LightningBolt,
		Heart,
		Sun,
		Moon,
		SmileyFace,
		IrregularSeal1,
		IrregularSeal2,
		FoldedCorner,
		Bevel,
		Frame,
		HalfFrame,
		Corner,
		DiagStripe,
		Chord,
		Arc,
		LeftBracket,
		RightBracket,
		LeftBrace,
		RightBrace,
		BracketPair,
		BracePair,
		StraightConnector1,
		BentConnector2,
		BentConnector3,
		BentConnector4,
		BentConnector5,
		CurvedConnector2,
		CurvedConnector3,
		CurvedConnector4,
		CurvedConnector5,
		Callout1,
		Callout2,
		Callout3,
		AccentCallout1,
		AccentCallout2,
		AccentCallout3,
		BorderCallout1,
		BorderCallout2,
		BorderCallout3,
		AccentBorderCallout1,
		AccentBorderCallout2,
		AccentBorderCallout3,
		WedgeRectCallout,
		WedgeRoundRectCallout,
		WedgeEllipseCallout,
		CloudCallout,
		Cloud,
		Ribbon,
		Ribbon2,
		EllipseRibbon,
		EllipseRibbon2,
		LeftRightRibbon,
		VerticalScroll,
		HorizontalScroll,
		Wave,
		DoubleWave,
		Plus,
		FlowChartProcess,
		FlowChartDecision,
		FlowChartInputOutput,
		FlowChartPredefinedProcess,
		FlowChartInternalStorage,
		FlowChartDocument,
		FlowChartMultidocument,
		FlowChartTerminator,
		FlowChartPreparation,
		FlowChartManualInput,
		FlowChartManualOperation,
		FlowChartConnector,
		FlowChartPunchedCard,
		FlowChartPunchedTape,
		FlowChartSummingJunction,
		FlowChartOr,
		FlowChartCollate,
		FlowChartSort,
		FlowChartExtract,
		FlowChartMerge,
		FlowChartOfflineStorage,
		FlowChartOnlineStorage,
		FlowChartMagneticTape,
		FlowChartMagneticDisk,
		FlowChartMagneticDrum,
		FlowChartDisplay,
		FlowChartDelay,
		FlowChartAlternateProcess,
		FlowChartOffpageConnector,
		ActionButtonBlank,
		ActionButtonHome,
		ActionButtonHelp,
		ActionButtonInformation,
		ActionButtonForwardNext,
		ActionButtonBackPrevious,
		ActionButtonEnd,
		ActionButtonBeginning,
		ActionButtonReturn,
		ActionButtonDocument,
		ActionButtonSound,
		ActionButtonMovie,
		Gear6,
		Gear9,
		Funnel,
		MathPlus,
		MathMinus,
		MathMultiply,
		MathDivide,
		MathEqual,
		MathNotEqual,
		CornerTabs,
		SquareTabs,
		PlaqueTabs,
		ChartX,
		ChartStar,
		ChartPlus,
	}
	#endregion
	#region XlOutlineDashing
	public enum XlOutlineDashing {
		Solid = 0,
		SystemDash = 1,
		SystemDot = 2,
		SystemDashDot = 3,
		SystemDashDotDot = 4,
		Dot = 5,
		Dash = 6,
		LongDash = 7,
		DashDot = 8,
		LongDashDot = 9,
		LongDashDotDot = 10,
	}
	#endregion
	#region XlOutline
	public class XlOutline {
		const double minWidth = 0.0;
		const double maxWidth = 1584.0;
		XlColor color = XlColor.Empty;
		double width = 0.75;
		public XlColor Color {
			get { return color; }
			set { color = value ?? XlColor.Empty; }
		}
		public XlOutlineDashing Dashing { get; set; }
		public double Width {
			get { return width; }
			set {
				if(value < minWidth)
					value = minWidth;
				if(value > maxWidth)
					value = maxWidth;
				width = value;
			}
		}
	}
	#endregion
	#region XlGraphicFrame
	public class XlGraphicFrame {
		const double minRotation = -3600.0;
		const double maxRotation = 3600.0;
		double rotation = 0.0;
		public bool FlipHorizontal { get; set; }
		public bool FlipVertical { get; set; }
		public double Rotation {
			get { return rotation; }
			set {
				if(value < minRotation || value > maxRotation)
					throw new ArgumentOutOfRangeException(string.Format("Rotation angle out of range {0}...{1}", minRotation, maxRotation));
				rotation = value;
			}
		}
		internal bool IsDefault() {
			return rotation == 0.0 && !FlipHorizontal && !FlipVertical;
		}
	}
	#endregion
	#region XlShape
	public class XlShape : XlDrawingObject {
		readonly XlOutline outline = new XlOutline();
		readonly XlGraphicFrame frame = new XlGraphicFrame();
		protected XlShape(XlGeometryPreset geometryPreset) {
			GeometryPreset = geometryPreset;
		}
		protected internal int Id { get; set; }
		public XlGeometryPreset GeometryPreset { get; private set; }
		public XlOutline Outline { get { return outline; } }
		public XlGraphicFrame Frame { get { return frame; } }
		public static XlShape Line(XlColor color, XlOutlineDashing dashing = XlOutlineDashing.Solid) {
			if(color.IsAutoOrEmpty)
				throw new ArgumentException("Line color is auto or empty!");
			XlShape result = new XlShape(XlGeometryPreset.Line);
			result.Outline.Color = color;
			result.Outline.Dashing = dashing;
			return result;
		}
	}
	#endregion
	#region IXlShapeContainer
	public interface IXlShapeContainer {
		IList<XlShape> Shapes { get; }
	}
	#endregion
}
