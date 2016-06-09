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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Diagram.Core;
namespace DevExpress.Diagram.Core {
	public static class RulerRenderHelper {
		struct Tick {
			public readonly double Step;
			public readonly double Offset;
			public Tick(double step, double offset) {
				Step = step;
				Offset = offset;
			}
		}
		class GridRenderInfo {
			public int TotalTicks { get; private set; }
			public Tick SmallTick { get; private set; }
			public Tick MediumTick { get; private set; }
			public Tick LargeTick { get; private set; }
			static double[] tickWeights = new double[] { 0.325, 0.5, 1.0 };
			public GridRenderInfo(int totalTicks, Tick smallTick, Tick mediumTick, Tick largeTick) {
				TotalTicks = totalTicks;
				SmallTick = smallTick;
				MediumTick = mediumTick;
				LargeTick = largeTick;
			}
			public double GetPosition(int step) {
				return SmallTick.Offset + SmallTick.Step * step;
			}
			public double GetTickWeight(int step) {
				if(IsLargeTick(step)) {
					return GetLargeTickWeight();
				}
				if(IsMediumTick(step)) {
					return GetMediumTickWeight();
				}
				return GetSmallTickWeight();
			}
			public bool IsMediumTick(int step) {
				return (step - MediumTick.Offset) % MediumTick.Step == 0;
			}
			public bool IsLargeTick(int step) {
				return (step - LargeTick.Offset) % LargeTick.Step == 0;
			}
			internal static double GetLargeTickWeight() {
				return tickWeights[2];
			}
			internal static double GetMediumTickWeight() {
				return tickWeights[1];
			}
			internal static double GetSmallTickWeight() {
				return tickWeights[0];
			}
		}
		class RulerRenderInfo : GridRenderInfo {
			public int LabelOffset { get { return (int)LargeTick.Offset; } }
			public int LabelStep { get { return (int)LargeTick.Step; } }
			public double ZoomLevel { get; private set; }
			public RulerRenderInfo(GridRenderInfo gridRenderInfo, double zoomLevel)
				: base(gridRenderInfo.TotalTicks, gridRenderInfo.SmallTick, gridRenderInfo.MediumTick, gridRenderInfo.LargeTick) {
				ZoomLevel = zoomLevel;
			}
			public double GetLabel(int step, double offset) {
				if(!IsLabelStep(step))
					return double.NaN;
				double position = offset - SmallTick.Offset;
				return (step * SmallTick.Step - position) / ZoomLevel;
			}
			bool IsLabelStep(int step) {
				return (step - LabelOffset) % LabelStep == 0;
			}
		}
		public static void DrawGrid(MeasureUnit measureUnit, Size? gridSize, Rect drawingArea, double zoomLevel, bool justLarge, Action<AxisLine> drawLine) {
			DrawGridLines(measureUnit, gridSize, drawingArea, zoomLevel, justLarge, drawLine, Orientation.Horizontal);
			DrawGridLines(measureUnit, gridSize, drawingArea, zoomLevel, justLarge, drawLine, Orientation.Vertical);
		}
		public static void DrawRuler(MeasureUnit measureUnit, Orientation orientation, Size size, double offset, double zoomLevel, Action<double, double> drawTick, Action<double, double> drawLabel) {
			var rulerInfo = GenerateTicks(measureUnit, orientation, size, offset, zoomLevel);
			double position = 0d;
			for(int i = 0; i < rulerInfo.TotalTicks; i++) {
				position = rulerInfo.GetPosition(i);
				drawTick(position, rulerInfo.GetTickWeight(i));
				double label = rulerInfo.GetLabel(i, offset);
				if(!double.IsNaN(label)) {
					double labelValue = measureUnit.FromPixels(label / measureUnit.Multiplier);
					drawLabel(position, Math.Round(labelValue, 1));
				}
			}
		}
		public static Size GetGridSize(MeasureUnit unit, double zoomLevel) {
			var steps = GetSteps(unit, zoomLevel);
			double step = steps.MediumStep * steps.SmallStep;
			return new Size(step, step);
		}
		static bool CanDrawLine(bool justLarge, GridRenderInfo renderInfo, int i) {
			bool isLargeTick = renderInfo.IsLargeTick(i);
			return justLarge && isLargeTick || !justLarge && !isLargeTick && renderInfo.IsMediumTick(i);
		}
		static void DrawGridLines(MeasureUnit measureUnit, Size? gridSize, Rect drawingArea, double zoomLevel, bool justLarge, Action<AxisLine> drawLine, Orientation orientation) {
			double min = orientation.GetSide(drawingArea, Side.Near);
			double max = orientation.GetSide(drawingArea, Side.Far);
			double lineStart = Math.Max(0d, orientation.Rotate().GetSide(drawingArea, Side.Near));
			double lineEnd = lineStart + orientation.Rotate().GetSize(drawingArea);
			double gridSizeValue = gridSize.HasValue ? orientation.GetSize(gridSize.Value) : double.NaN;
			var renderInfo = GenerateGrid(measureUnit, min, 0d, max, zoomLevel, gridSizeValue);
			for(int i = 0; i < renderInfo.TotalTicks; i++) {
				if(!CanDrawLine(justLarge, renderInfo, i))
					continue;
				double linePosition = Math.Round(min + renderInfo.GetPosition(i));
				Point from = orientation.MakePoint(linePosition, lineStart);
				Point to = orientation.MakePoint(linePosition, lineEnd);
				drawLine(new AxisLine(from, lineEnd - lineStart, orientation.Rotate()));
			}
		}
		static GridRenderInfo GenerateGrid(MeasureUnit measureUnit, double min, double offset, double max, double zoomLevel, double gridSize = double.NaN) {
			TickStepsData steps = GetSteps(measureUnit, zoomLevel);
			double smallStep = double.IsNaN(gridSize) ? measureUnit.ToPixels(steps.SmallStep) : 1d;
			Tick smallTick = GetSmallTick(smallStep * zoomLevel, offset - min);
			double mediumStep = steps.MediumStep;
			double largeStep = steps.LargeStep;
			if(!double.IsNaN(gridSize)) {
				largeStep = gridSize;
				mediumStep = gridSize % 5 == 0 ? largeStep / 5 : largeStep / 2;
			}
			double originStep = Math.Round((offset - min - smallTick.Offset) / smallTick.Step);
			double mediumTickOffset = GetTickOffset(originStep, mediumStep);
			double largeTickOffset = GetTickOffset(originStep, largeStep);
			int totalTicks = (int)Math.Round((max - min + smallTick.Step - smallTick.Offset) / smallTick.Step);
			return new GridRenderInfo(totalTicks, smallTick, new Tick(mediumStep, mediumTickOffset), new Tick(largeStep, largeTickOffset));
		}
		static RulerRenderInfo GenerateTicks(MeasureUnit measureUnit, Orientation orientation, Size size, double offset, double zoomLevel) {
			var gridInfo = GenerateGrid(measureUnit, 0d, offset, orientation.GetSize(size), zoomLevel);
			return new RulerRenderInfo(gridInfo, zoomLevel);
		}
		static TickStepsData GetSteps(MeasureUnit measureUnit, double zoomLevel) {
			TickStepsData[] table = measureUnit.StepsData;
			TickStepsData steps = table.First();
			for(int i = 1; i < table.Length; i++) {
				if(table[i].ZoomLevel > zoomLevel)
					break;
				steps = table[i];
			}
			return steps;
		}
		static double GetTickOffset(double originStep, double tickStep) {
			double offset = originStep % tickStep;
			if(offset < 0)
				offset += tickStep;
			return offset;
		}
		static Tick GetSmallTick(double tickStep, double start) {
			double tickOffset = GetTickOffset(Math.Floor(start), tickStep);
			return new Tick(tickStep, tickOffset);
		}
	}
}
