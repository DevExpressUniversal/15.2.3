#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public class PdfGraphicsState {
		PdfTransformationMatrix transformationMatrix = new PdfTransformationMatrix();
		PdfGraphicsPath clippingPath;
		PdfColorSpace strokingColorSpace = new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
		PdfColorSpace nonStrokingColorSpace = new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.Gray);
		PdfColor strokingColor = new PdfColor(0.0);
		PdfColor nonStrokingColor = new PdfColor(0.0);
		PdfTextState textState = new PdfTextState();
		double lineWidth = 1.0;
		PdfLineCapStyle lineCap = PdfLineCapStyle.Butt;
		PdfLineJoinStyle lineJoin = PdfLineJoinStyle.Miter;
		double miterLimit = 10.0;
		PdfLineStyle lineStyle = PdfLineStyle.CreateSolid();
		PdfRenderingIntent renderingIntent = PdfRenderingIntent.RelativeColorimetric;
		bool strokeAdjustment = false;
		PdfBlendMode blendMode = PdfBlendMode.Normal;
		PdfSoftMask softMask = PdfEmptySoftMask.Instance;
		double strokingAlphaConstant = 1.0;
		double nonStrokingAlphaConstant = 1.0;
		bool alphaSource = false;
		bool strokingOverprint = false;
		bool nonStrokingOverprint = false;
		PdfOverprintMode overprintMode = PdfOverprintMode.Erase;
		PdfFunction blackGenerationFunction = PdfPredefinedFunction.Default;
		PdfFunction undercolorRemovalFunction = PdfPredefinedFunction.Default;
		PdfFunction[] transferFunction = new PdfFunction[] { PdfPredefinedFunction.Default };
		PdfHalftone halftone = PdfDefaultHalftone.Instance;
		double flatnessTolerance = 1.0;
		double smoothnessTolerance = 0.0;
		bool textKnockout = true;
		public PdfTransformationMatrix TransformationMatrix {
			get { return transformationMatrix; } 
			internal set { transformationMatrix = value; } 
		}
		public PdfGraphicsPath ClippingPath { get { return clippingPath; } }
		public PdfColorSpace StrokingColorSpace { 
			get { return strokingColorSpace; } 
			internal set { strokingColorSpace = value; }
		}
		public PdfColorSpace NonStrokingColorSpace { 
			get { return nonStrokingColorSpace; } 
			internal set { nonStrokingColorSpace = value; }
		}
		public PdfColor StrokingColor { 
			get { return strokingColor; } 
			internal set { strokingColor = value; }
		}
		public PdfColor NonStrokingColor { 
			get { return nonStrokingColor; } 
			internal set { nonStrokingColor = value; }
		}
		public PdfTextState TextState { get { return textState; } }
		public double LineWidth { 
			get { return lineWidth; }
			internal set { lineWidth = value; }
		}
		public PdfLineCapStyle LineCap { 
			get { return lineCap; } 
			internal set { lineCap = value; }
		}
		public PdfLineJoinStyle LineJoin { 
			get { return lineJoin; } 
			set { lineJoin = value; }
		}
		public double MiterLimit { 
			get { return miterLimit; } 
			internal set { miterLimit = value; }
		}
		public PdfLineStyle LineStyle { 
			get { return lineStyle; } 
			internal set { lineStyle = value; }
		}
		public PdfRenderingIntent RenderingIntent { 
			get { return renderingIntent; } 
			internal set { renderingIntent = value; }
		}
		public bool StrokeAdjustment { get { return strokeAdjustment; } }
		public PdfBlendMode BlendMode { get { return blendMode; } }
		public PdfSoftMask SoftMask { get { return softMask; } }
		public double StrokingAlphaConstant { get { return strokingAlphaConstant; } }
		public double NonStrokingAlphaConstant { get { return nonStrokingAlphaConstant; } }
		public bool AlphaSource { get { return alphaSource; } }
		public bool StrokingOverprint { get { return strokingOverprint; } }
		public bool NonStrokingOverprint { get { return nonStrokingOverprint; } }
		public PdfOverprintMode OverprintMode { get { return overprintMode; } }
		public PdfFunction BlackGenerationFunction { get { return blackGenerationFunction; } }
		public PdfFunction UndercolorRemovalFunction { get { return undercolorRemovalFunction; } }
		public PdfFunction[] TransferFunction { get { return transferFunction; } }
		public PdfHalftone Halftone { get { return halftone; } }
		public double FlatnessTolerance { 
			get { return flatnessTolerance; } 
			internal set { flatnessTolerance = value; }
		}
		public double SmoothnessTolerance { get { return smoothnessTolerance; } }
		public bool TextKnockout { get { return textKnockout; } }
		public PdfGraphicsState Clone() {
			PdfGraphicsState copy = new PdfGraphicsState();
			copy.transformationMatrix = transformationMatrix;
			copy.clippingPath = clippingPath;
			copy.strokingColorSpace = strokingColorSpace;
			copy.nonStrokingColorSpace = nonStrokingColorSpace;
			copy.strokingColor = strokingColor;
			copy.nonStrokingColor = nonStrokingColor;
			copy.textState = textState.Clone();
			copy.lineWidth = lineWidth;
			copy.lineCap = lineCap;
			copy.lineJoin = lineJoin;
			copy.miterLimit = miterLimit;
			copy.lineStyle = lineStyle;
			copy.renderingIntent = renderingIntent;
			copy.strokeAdjustment = strokeAdjustment;
			copy.blendMode = blendMode;
			copy.softMask = softMask;
			copy.strokingAlphaConstant = strokingAlphaConstant;
			copy.nonStrokingAlphaConstant = nonStrokingAlphaConstant;
			copy.alphaSource = alphaSource;
			copy.strokingOverprint = strokingOverprint;
			copy.nonStrokingOverprint = nonStrokingOverprint;
			copy.overprintMode = overprintMode;
			copy.blackGenerationFunction = blackGenerationFunction;
			copy.undercolorRemovalFunction = undercolorRemovalFunction;
			copy.transferFunction = transferFunction;
			copy.halftone = halftone;
			copy.flatnessTolerance = flatnessTolerance;
			copy.smoothnessTolerance = smoothnessTolerance;
			copy.textKnockout = textKnockout;
			return copy;
		}
		internal PdfGraphicsStateChange ApplyParameters(PdfGraphicsStateParameters parameters) {
			PdfGraphicsStateChange change = ApplyParameter(ref lineWidth, parameters.LineWidth, PdfGraphicsStateChange.Pen) | ApplyParameter(ref lineCap, parameters.LineCap, PdfGraphicsStateChange.Pen) |
				ApplyParameter(ref lineJoin, parameters.LineJoin, PdfGraphicsStateChange.Pen) | ApplyParameter(ref miterLimit, parameters.MiterLimit, PdfGraphicsStateChange.Pen) |
				ApplyParameter(ref renderingIntent, parameters.RenderingIntent, PdfGraphicsStateChange.RenderingIntent) | 
				ApplyParameter(ref strokingOverprint, parameters.StrokingOverprint, PdfGraphicsStateChange.Overprint) |
				ApplyParameter(ref nonStrokingOverprint, parameters.NonStrokingOverprint, PdfGraphicsStateChange.Overprint) | 
				ApplyParameter(ref overprintMode, parameters.OverprintMode, PdfGraphicsStateChange.Overprint) |
				ApplyParameter(ref flatnessTolerance, parameters.FlatnessTolerance, PdfGraphicsStateChange.FlatnessTolerance) |
				ApplyParameter(ref smoothnessTolerance, parameters.SmoothnessTolerance, PdfGraphicsStateChange.SmoothnessTolerance) |
				ApplyParameter(ref strokeAdjustment, parameters.StrokeAdjustment, PdfGraphicsStateChange.StrokeAdjustment) | ApplyParameter(ref blendMode, parameters.BlendMode, PdfGraphicsStateChange.BlendMode) |
				ApplyParameter(ref strokingAlphaConstant, parameters.StrokingAlphaConstant, PdfGraphicsStateChange.Alpha) | 
				ApplyParameter(ref nonStrokingAlphaConstant, parameters.NonStrokingAlphaConstant, PdfGraphicsStateChange.Alpha) | 
				ApplyParameter(ref alphaSource, parameters.AlphaSource, PdfGraphicsStateChange.Alpha) | ApplyParameter(ref textKnockout, parameters.TextKnockout, PdfGraphicsStateChange.TextKnockout);
			PdfLineStyle newLineStyle = parameters.LineStyle;
			if (newLineStyle != null && (lineStyle == null || !lineStyle.IsSame(newLineStyle))) {
				lineStyle = newLineStyle;
				change |= PdfGraphicsStateChange.Pen;
			}
			PdfFont newFont = parameters.Font;
			if (newFont != null && !Object.ReferenceEquals(newFont, textState.Font)) {
				textState.Font = newFont;
				change |= PdfGraphicsStateChange.Font;
			}
			double? parametersFontSize = parameters.FontSize;
			if (parametersFontSize.HasValue) {
				double newFontSize = parametersFontSize.Value;
				if (newFontSize != textState.FontSize) {
					textState.FontSize = newFontSize;
					change |= PdfGraphicsStateChange.Font;
				}
			}
			PdfFunction newBlackGenerationFunction = parameters.BlackGenerationFunction;
			if (newBlackGenerationFunction != null && !newBlackGenerationFunction.IsSame(blackGenerationFunction)) {
				blackGenerationFunction = newBlackGenerationFunction;
				change |= PdfGraphicsStateChange.BlackGenerationFunction;
			}
			PdfFunction newUndercolorRemovalFunction = parameters.UndercolorRemovalFunction;
			if (newUndercolorRemovalFunction != null && !newUndercolorRemovalFunction.IsSame(undercolorRemovalFunction)) {
				undercolorRemovalFunction = newUndercolorRemovalFunction;
				change |= PdfGraphicsStateChange.UndercolorRemovalFunction;
			}
			PdfFunction[] newTransferFunction = parameters.TransferFunction;
			if (newTransferFunction != null) {
				int length = transferFunction.Length;
				bool isSame = newTransferFunction.Length != length;
				if (isSame)
					for (int i = 0; i < length; i++)
						if (!newTransferFunction[i].IsSame(transferFunction[i])) {
							isSame = false;
							break;
						}
				if (!isSame) {
					transferFunction = newTransferFunction;
					change |= PdfGraphicsStateChange.TransferFunction;
				}
			}
			PdfHalftone newHalftone = parameters.Halftone;
			if (newHalftone != null && !newHalftone.IsSame(halftone)) {
				halftone = newHalftone;
				change |= PdfGraphicsStateChange.Halftone;
			}
			PdfSoftMask newSoftMask = parameters.SoftMask;
			if (newSoftMask != null && !newSoftMask.IsSame(softMask)) {
				softMask = newSoftMask;
				change |= PdfGraphicsStateChange.SoftMask;
			}
			return change;
		}
		PdfGraphicsStateChange ApplyParameter<T>(ref T parameter, T? value, PdfGraphicsStateChange change) where T : struct {
			if (value.HasValue) {
				T newValue = value.Value;
				if (!newValue.Equals(parameter)) {
					parameter = newValue;
					return change;
				}
			}
			return PdfGraphicsStateChange.None;
		}
	}
}
