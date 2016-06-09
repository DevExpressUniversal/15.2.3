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
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class PdfHatchPatternConstructor {
		public static PdfHatchPatternConstructor Create(PdfHatchBrush hatchBrush) {
			switch (hatchBrush.HatchStyle) {
				case HatchStyle.BackwardDiagonal:
					return new PdfBackwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Cross:
					return new PdfCrossHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Horizontal:
					return new PdfHorizontalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.LargeCheckerBoard:
					return new PdfCheckerBoardHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.HorizontalBrick:
					return new PdfHorizontalBriksHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DarkHorizontal:
					return new PdfDarkHorizontalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.SmallGrid:
					return new PdfSmallGridHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.LightHorizontal:
					return new PdfLightHorizontalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.NarrowHorizontal:
					return new PdfNarrowHorizontalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DarkUpwardDiagonal:
					return new PdfDarkUpwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.LightUpwardDiagonal:
					return new PdfLightUpwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.WideUpwardDiagonal:
					return new PdfWideUpwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Vertical:
					return new PdfVerticalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DarkVertical:
					return new PdfDarkVerticalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.LightVertical:
					return new PdfLightVerticalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.NarrowVertical:
					return new PdfNarrowVerticalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DiagonalBrick:
					return new PdfDiagonalBricksHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.ForwardDiagonal:
					return new PdfForwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DarkDownwardDiagonal:
					return new PdfDarkDownwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.LightDownwardDiagonal:
					return new PdfLightDownwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.WideDownwardDiagonal:
					return new PdfWideDownwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.SolidDiamond:
					return new PdfSolidDiamondHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DiagonalCross:
					return new PdfDiagonalCrossHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DashedHorizontal:
					return new PdfDashedHorizontalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DashedVertical:
					return new PdfDashedVerticalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DashedDownwardDiagonal:
					return new PdfDashedDownwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DashedUpwardDiagonal:
					return new PdfDashedUpwardDiagonalHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.OutlinedDiamond:
					return new PdfOutlinedDiamondHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.ZigZag:
					return new PdfZigZagHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.SmallCheckerBoard:
					return new PdfSmallCheckerBoardHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent05:
					return new PdfPercent05HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent10:
					return new PdfPercent10HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent20:
					return new PdfPercent20HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent25:
					return new PdfPercent25HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent30:
					return new PdfPercent30HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent40:
					return new PdfPercent40HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent50:
					return new PdfPercent50HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent60:
					return new PdfPercent60HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent70:
					return new PdfPercent70HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent75:
					return new PdfPercent75HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent80:
					return new PdfPercent80HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Percent90:
					return new PdfPercent90HatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DottedGrid:
					return new PdfDottedGridHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.DottedDiamond:
					return new PdfDottedDiamondHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Plaid:
					return new PdfPlaidHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Divot:
					return new PdfDivotHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Sphere:
					return new PdfSphereHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Wave:
					return new PdfWaveHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Trellis:
					return new PdfTrellisHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.LargeConfetti:
					return new PdfLargeConfettiHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.SmallConfetti:
					return new PdfSmallConfettiHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Weave:
					return new PdfWeaveHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				case HatchStyle.Shingle:
					return new PdfShingleHatchPatternConstructor(hatchBrush.ForegroundColor, hatchBrush.BackgroundColor);
				default:
					return null;
			}
		}
		const double DefaultLineStep = 6;
		const double DefaultLineWidth = 0.7f;
		readonly Color foreColor;
		readonly Color backColor;
		readonly double lineWidth;
		readonly double lineStep;
		PdfCommandConstructor constructor;
		PdfTransformationMatrix transform;
		protected virtual double LineWidth { get { return lineWidth; } }
		protected virtual double LineStep { get { return lineStep; } }
		protected virtual PdfLineCapStyle LineCapStyle { get { return PdfLineCapStyle.ProjectingSquare; } }
		internal PdfCommandConstructor Constructor { get { return constructor; } }
		protected PdfHatchPatternConstructor(Color foreColor, Color backColor) {
			this.foreColor = foreColor;
			this.backColor = backColor;
			this.lineWidth = DefaultLineWidth;
			this.lineStep = DefaultLineStep;
			transform = new PdfTransformationMatrix();
		}
		protected void RotateTransform(float digree) {
			double angle = System.Math.PI * digree / 180;
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			transform = PdfTransformationMatrix.Multiply(transform, new PdfTransformationMatrix(cos, -sin, sin, cos, 0, 0));
		}
		protected void MultipleTransform(PdfTransformationMatrix transform) {
			this.transform = PdfTransformationMatrix.Multiply(this.transform, transform);
		}
		public PdfPattern CreatePattern(PdfRectangle bBox, PdfDocumentCatalog documentCatalog) {
			double tileSize = LineStep;
			double lineOffset = LineWidth / 2;
			PdfTilingPattern pattern = new PdfTilingPattern(PdfTransformationMatrix.Multiply(new PdfTransformationMatrix(1, 0, 0, -1, 0, bBox.Height), transform), 
				new PdfRectangle(-lineOffset, -lineOffset, tileSize + lineOffset, tileSize + lineOffset), tileSize, tileSize, documentCatalog);
			constructor = new PdfCommandConstructor(pattern.Commands, pattern.Resources);
			GetCommands();
			return pattern;
		}
		protected virtual void GetCommands() {
			constructor.SetColorForNonStrokingOperations(PdfGraphicsConverter.GdiToPdfColor(backColor));
			constructor.SetGraphicsStateParameters(new PdfGraphicsStateParameters() { NonStrokingAlphaConstant = backColor.A / 255f });
			constructor.FillRectangle(new PdfRectangle(new PdfPoint(0, 0), new PdfPoint(LineStep, LineStep)));
			constructor.SetColorForNonStrokingOperations(PdfGraphicsConverter.GdiToPdfColor(foreColor));
			constructor.SetColorForStrokingOperations(PdfRGBColor.Create(foreColor.R / 255f, foreColor.G / 255f, foreColor.B / 255f));
			constructor.SetGraphicsStateParameters(new PdfGraphicsStateParameters() {
				NonStrokingAlphaConstant = foreColor.A / 255.0,
				StrokingAlphaConstant = foreColor.A / 255.0
			});
			constructor.SetLineWidth(LineWidth);
			constructor.SetLineCapStyle(LineCapStyle);
		}
	}
}
