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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Localization;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraPrinting.Shape.Native {
	#region IShapeDrawingInfo
	public interface IShapeDrawingInfo {
		float LineWidth { get; }
		DashStyle LineStyle { get; }
		int Angle { get; }
		bool Stretch { get; }
		Color FillColor { get; }
		Color ForeColor { get; }
	}
	#endregion
	#region ShapeHelper
	public static class ShapeHelper {
		public const int TopAngle = 0;
		public const int LeftAngle = 90;
		public const int RightAngle = LeftAngle * 3;
		public const int BottomAngle = LeftAngle * 2;
		public const double WholeCircleInRad = Math.PI * 2;
		public const int WholeCircleInDeg = 360;
		public const int HalfOfCircleInDeg = WholeCircleInDeg / 2;
		public static float RadToDeg(float angleInRad) {
			return ((float)HalfOfCircleInDeg / (float)Math.PI) * angleInRad;
		}
		public static float DegToRad(float angleInDeg) {
			return ((float)Math.PI / (float)HalfOfCircleInDeg) * angleInDeg;
		}
		public static PointF[] CreateRectanglePoints(RectangleF bounds) {
			return new PointF[] { 
										new PointF(bounds.Right, bounds.Top), 
										new PointF(bounds.Right, bounds.Bottom), 
										new PointF(bounds.Left, bounds.Bottom),
										bounds.Location 
									};
		}
		public static ShapeLineCommandCollection CreateConsecutiveLinesFromPoints(PointF[] points) {
			ConsecutiveLinesCreator consecutiveLinesCreator = new ConsecutiveLinesCreator();
			ClosedListIterator.Iterate(points, consecutiveLinesCreator);
			return consecutiveLinesCreator.Lines;
		}
		public static int ValidateRestrictedValue(int value, int minValue, int maxValue, string message) {
			if(value < minValue || maxValue < value) {
				throw new ArgumentOutOfRangeException(message);
			}
			return value;
		}
		public static float ValidateRestrictedValue(float value, float minValue, float maxValue, string message) {
			if(value < minValue || maxValue < value) {
				throw new ArgumentOutOfRangeException(message);
			}
			return value;
		}
		public static int ValidateAngle(int value) {
			int angle = value % ShapeHelper.WholeCircleInDeg;
			if(angle < 0)
				angle += ShapeHelper.WholeCircleInDeg;
			return angle;
		}
		public static int ValidatePercentageValue(int value, string message) {
			return ValidateRestrictedValue(value, 0, 100, message);
		}
		public static float ValidatePercentageValue(float value, string message) {
			return ValidateRestrictedValue(value, 0, 100, message);
		}
		public static ShapeCommandCollection CreateBraceCommands(RectangleF bounds, int tailLength, int tipLength, int fillet) {
			float braceLength = bounds.Height;
			float halfBraceLength = braceLength / 2;
			float tailLengthF = braceLength * (tailLength / 100f);
			float tipLengthF = braceLength * (tipLength / 100f);
			PointF braceCenter = RectHelper.CenterOf(bounds);
			float clawOrdinateTop = braceCenter.Y - halfBraceLength;
			ShapeLineCommand clawTop = new ShapeLineCommand(new PointF(braceCenter.X + tipLengthF, clawOrdinateTop), new PointF(braceCenter.X, clawOrdinateTop));
			ShapeLineCommand sectionTopTop = new ShapeLineCommand(clawTop.EndPoint, ConsecutiveLineCommandsRounder.DividePoints(clawTop.EndPoint, braceCenter, 0.5f));
			ShapeLineCommand sectionTopBottom = new ShapeLineCommand(sectionTopTop.EndPoint, braceCenter);
			ShapeLineCommand taleTop = new ShapeLineCommand(sectionTopBottom.EndPoint, new PointF(braceCenter.X - tailLengthF, braceCenter.Y));
			ShapeBezierCommand clawFilletTop = ConsecutiveLineCommandsRounder.CreateShortestLineFilletCommand(clawTop, sectionTopTop, 2 * fillet);
			ShapeBezierCommand taleFilletTop = ConsecutiveLineCommandsRounder.CreateShortestLineFilletCommand(sectionTopBottom, taleTop, 2 * fillet);
			LineCommandsRounder.RoundLineCommands(sectionTopBottom, taleTop, taleFilletTop);
			LineCommandsRounder.RoundLineCommands(clawTop, sectionTopTop, clawFilletTop);
			float clawOrdinateBottom = braceCenter.Y + halfBraceLength;
			ShapeLineCommand taleBottom = new ShapeLineCommand(new PointF(braceCenter.X - tailLengthF, braceCenter.Y), braceCenter);
			ShapeLineCommand sectionBottomTop = new ShapeLineCommand(taleBottom.EndPoint, ConsecutiveLineCommandsRounder.DividePoints(taleBottom.EndPoint, new PointF(braceCenter.X, clawOrdinateBottom), 0.5f));
			ShapeLineCommand sectionBottomBottom = new ShapeLineCommand(sectionBottomTop.EndPoint, new PointF(braceCenter.X, clawOrdinateBottom));
			ShapeLineCommand clawBottom = new ShapeLineCommand(sectionBottomBottom.EndPoint, new PointF(braceCenter.X + tipLengthF, clawOrdinateBottom));
			ShapeBezierCommand taleFilletBottom = ConsecutiveLineCommandsRounder.CreateShortestLineFilletCommand(taleBottom, sectionBottomTop, 2 * fillet);
			ShapeBezierCommand clawFilletBottom = ConsecutiveLineCommandsRounder.CreateShortestLineFilletCommand(sectionBottomBottom, clawBottom, 2 * fillet);
			LineCommandsRounder.RoundLineCommands(taleBottom, sectionBottomTop, taleFilletBottom);
			LineCommandsRounder.RoundLineCommands(sectionBottomBottom, clawBottom, clawFilletBottom);
			ShapePointsCommandCollection commands = new ShapePointsCommandCollection();
			commands.Add(clawTop);
			commands.Add(clawFilletTop);
			commands.Add(sectionTopTop);
			commands.Add(sectionTopBottom);
			commands.Add(taleFilletTop);
			commands.Add(taleTop);
			commands.Add(taleBottom);
			commands.Add(taleFilletBottom);
			commands.Add(sectionBottomTop);
			commands.Add(sectionBottomBottom);
			commands.Add(clawFilletBottom);
			commands.Add(clawBottom);
			ShapePathCommandCollection pathCommands = new ShapePathCommandCollection();
			ShapeDrawPathCommand drawPathCommand = new ShapeDrawPathCommand(commands);
			System.Diagnostics.Debug.Assert(!drawPathCommand.IsClosed, "");
			pathCommands.Add(drawPathCommand);
			return pathCommands;
		}
		public static float PercentsToRatio(int percents) {
			return PercentsToRatio((float)percents);
		}
		public static float PercentsToRatio(float percents) {
			return percents / 100.0f;
		}
		public static ShapeCommandCollection CreateCommands(ShapeBase shape, RectangleF bounds, int angle) {
			return shape.CreateCommands(bounds, angle);
		}
		public static PointF[] CreatePoints(ClosedShapeBase shape, RectangleF bounds, int angle) {
			return shape.CreatePoints(bounds, angle);
		}
		public static bool SupportsFillColor(ShapeBase shape) {
			return shape.SupportsFillColor;
		}
		public static string GetDisplayName(PreviewStringId shapeStringId) {
			return PreviewLocalizer.GetString(shapeStringId);
		}
		public static string GetInvariantName(PreviewStringId shapeStringId) {
			return PreviewLocalizer.Default.GetLocalizedString(shapeStringId);
		}
		public static void DrawShapeContent(ShapeBase shape, IGraphics gr, RectangleF clientBounds, IShapeDrawingInfo info, GdiHashtable gdi) {
			shape.DrawContent(gr, clientBounds, info, gdi);
		}
	}
	#endregion
	#region ClosedListVisitors
	public static class ClosedListIterator {
		public static void Iterate(IList list, IClosedListVisitor visitor) {
			int count = list.Count;
			for(int i = 0; i < count; i++) {
				visitor.VisitElement(list[(i - 1 + count) % count], list[i], i);
			}
		}
	}
	public interface IClosedListVisitor {
		void VisitElement(object previous, object current, int currentObjectIndex);
	}
	public class FilletCommandsCreator : IClosedListVisitor {
		ShapeBezierCommandCollection filletCommands = new ShapeBezierCommandCollection();
		ILinesAdjuster adjuster;
		float filletInPercents;
		public ShapeBezierCommandCollection FilletCommands {
			get { return filletCommands; }
		}
		public FilletCommandsCreator(float filletInPercents, ILinesAdjuster adjuster) {
			this.adjuster = adjuster;
			this.filletInPercents = filletInPercents;
		}
		void IClosedListVisitor.VisitElement(object previous, object current, int currentObjectIndex) {
			filletCommands.Add(ConsecutiveLineCommandsRounder.CreateFilletCommand((ShapeLineCommand)previous, (ShapeLineCommand)current, filletInPercents, adjuster));
		}
	}
	public class LineCommandsRounder : IClosedListVisitor {
		#region static
		public static void RoundLineCommands(ShapeLineCommand line1, ShapeLineCommand line2, ShapeBezierCommand filletCommand) {
			line1.EndPoint = filletCommand.StartPoint;
			line2.StartPoint = filletCommand.EndPoint;
		}
		#endregion
		ShapeBezierCommandCollection filletCommands;
		public LineCommandsRounder(ShapeBezierCommandCollection filletCommands) {
			this.filletCommands = filletCommands;
		}
		void IClosedListVisitor.VisitElement(object previous, object current, int currentObjectIndex) {
			RoundLineCommands((ShapeLineCommand)previous, (ShapeLineCommand)current, (ShapeBezierCommand)filletCommands[currentObjectIndex]);
		}
	}
	public class ConsecutiveLinesCreator : IClosedListVisitor {
		ShapeLineCommandCollection lines = new ShapeLineCommandCollection();
		public ShapeLineCommandCollection Lines {
			get { return lines; }
		}
		void IClosedListVisitor.VisitElement(object previous, object current, int currentObjectIndex) {
			lines.AddLine((PointF)previous, (PointF)current);
		}
	}
	#endregion
	#region LinesAdjusters
	public interface ILinesAdjuster {
		ShapeLineCommandCollection AdjustLines(ShapeLineCommand line1, ShapeLineCommand line2);
	}
	public class UniformLinesAdjuster : ILinesAdjuster {
		public static readonly UniformLinesAdjuster Instance = new UniformLinesAdjuster();
		protected UniformLinesAdjuster() {
		}
		public ShapeLineCommandCollection AdjustLines(ShapeLineCommand line1, ShapeLineCommand line2) {
			ShapeLineCommandCollection commands = new ShapeLineCommandCollection();
			FillCommands(commands, line1, line2);
			return commands;
		}
		protected virtual void FillCommands(ShapeLineCommandCollection commands, ShapeLineCommand line1, ShapeLineCommand line2) {
			commands.Add(line1);
			commands.Add(line2);
		}
	}
	public class ShortestLineLinesAdjuster : UniformLinesAdjuster {
		#region static
		static void FillCommandsWithRatios(ShapeLineCommandCollection commands, ShapeLineCommand line1, ShapeLineCommand line2, float ratio1, float ratio2) {
			PointF adjustPoint1 = ConsecutiveLineCommandsRounder.DividePoints(line1.StartPoint, line1.EndPoint, ratio1);
			commands.AddLine(adjustPoint1, line1.EndPoint);
			PointF adjustPoint2 = ConsecutiveLineCommandsRounder.DividePoints(line2.EndPoint, line2.StartPoint, ratio2);
			commands.AddLine(line2.StartPoint, adjustPoint2);
		}
		#endregion
		public new static readonly ShortestLineLinesAdjuster Instance = new ShortestLineLinesAdjuster();
		protected ShortestLineLinesAdjuster()
			: base() {
		}
		protected override void FillCommands(ShapeLineCommandCollection commands, ShapeLineCommand line1, ShapeLineCommand line2) {
			float lengthRatio = (line1.Length == 0 || line2.Length == 0) ? 1 : line1.Length / line2.Length;
			if(line1.Length < line2.Length)
				FillCommandsWithRatios(commands, line1, line2, 1, lengthRatio);
			else
				FillCommandsWithRatios(commands, line1, line2, 1 / lengthRatio, 1);
		}
	}
	#endregion
	#region LineCommandsRounder
	public static class ConsecutiveLineCommandsRounder {
		const float FilletCorrectFactor = 0.45f;
		#region tests
		public static ShapeBezierCommand CreateUniformlyFilletCommand(ShapeLineCommand line1, ShapeLineCommand line2, float filletInPercents) {
			return CreateFilletCommand(line1, line2, filletInPercents, UniformLinesAdjuster.Instance);
		}
		public static ShapeBezierCommand CreateShortestLineFilletCommand(ShapeLineCommand line1, ShapeLineCommand line2, float filletInPercents) {
			return CreateFilletCommand(line1, line2, filletInPercents, ShortestLineLinesAdjuster.Instance);
		}
		#endregion
		public static ShapeBezierCommand CreateFilletCommand(ShapeLineCommand line1, ShapeLineCommand line2, float filletInPercents, ILinesAdjuster adjuster) {
			System.Diagnostics.Debug.Assert(line1.EndPoint == line2.StartPoint);
			float fillet = ShapeHelper.PercentsToRatio(filletInPercents);
			ShapeLineCommandCollection commands = adjuster.AdjustLines(line1, line2);
			ShapeLineCommand adjustedLine1 = (ShapeLineCommand)commands[0];
			ShapeLineCommand adjustedLine2 = (ShapeLineCommand)commands[1];
			return new ShapeBezierCommand(
				CalcBasePoint(adjustedLine1.StartPoint, adjustedLine1.EndPoint, fillet),
				CalcControlPoint(adjustedLine1.StartPoint, adjustedLine1.EndPoint, fillet),
				CalcControlPoint(adjustedLine2.EndPoint, adjustedLine2.StartPoint, fillet),
				CalcBasePoint(adjustedLine2.EndPoint, adjustedLine2.StartPoint, fillet)
				);
		}
		public static PointF DividePoints(PointF start, PointF end, float divideFactorFromEnd) {
			return new PointF(DivideValues(start.X, end.X, divideFactorFromEnd), DivideValues(start.Y, end.Y, divideFactorFromEnd));
		}
		public static ShapePointsCommandCollection CreateRoundedConsecutiveLinesCommands(ShapeCommandCollection lineCommands, float filletInPercents, ILinesAdjuster adjuster) {
			FilletCommandsCreator filletCommandsCreator = new FilletCommandsCreator(filletInPercents, adjuster);
			ClosedListIterator.Iterate(lineCommands, filletCommandsCreator);
			LineCommandsRounder lineCommandsRounder = new LineCommandsRounder(filletCommandsCreator.FilletCommands);
			ClosedListIterator.Iterate(lineCommands, lineCommandsRounder);
			int count = lineCommands.Count;
			ShapePointsCommandCollection result = new ShapePointsCommandCollection();
			for(int i = 0; i < count; i++) {
				result.Add(filletCommandsCreator.FilletCommands[i]);
				result.Add(lineCommands[i]);
			}
			return result;
		}
		static PointF CalcBasePoint(PointF start, PointF end, float fillet) {
			PointF center = DividePoints(start, end, 0.5f);
			return DividePoints(center, end, fillet);
		}
		static PointF CalcControlPoint(PointF start, PointF end, float fillet) {
			return CalcBasePoint(start, end, fillet * FilletCorrectFactor);
		}
		static float DivideValues(float start, float end, float divideFactorFromEnd) {
			return (divideFactorFromEnd) * start + (1 - divideFactorFromEnd) * end;
		}
	}
	#endregion
	#region ShapeFillStyles
	#endregion
}
