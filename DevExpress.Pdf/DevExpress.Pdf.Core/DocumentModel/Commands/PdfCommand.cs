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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public abstract class PdfCommand {
		internal static PdfCommand Create(PdfResources resources, string name, PdfOperands operands, bool supportType3FontCommands, bool shouldIgnoreUnknownCommands) {
			bool skipValidation = false;
			try {
				switch (name) {
					case PdfSaveGraphicsStateCommand.Name:
						return new PdfSaveGraphicsStateCommand();
					case PdfRestoreGraphicsStateCommand.Name:
						return new PdfRestoreGraphicsStateCommand();
					case PdfModifyTransformationMatrixCommand.Name:
						return new PdfModifyTransformationMatrixCommand(new PdfTransformationMatrix(operands));
					case PdfSetLineWidthCommand.Name:
						return new PdfSetLineWidthCommand(operands);
					case PdfSetLineCapStyleCommand.Name:
						return new PdfSetLineCapStyleCommand(operands);
					case PdfSetLineJoinStyleCommand.Name:
						return new PdfSetLineJoinStyleCommand(operands);
					case PdfSetMiterLimitCommand.Name:
						return new PdfSetMiterLimitCommand(operands);
					case PdfSetLineStyleCommand.Name:
						return new PdfSetLineStyleCommand(operands);
					case PdfSetRenderingIntentCommand.Name:
						return new PdfSetRenderingIntentCommand(operands);
					case PdfSetFlatnessToleranceCommand.Name:
						return new PdfSetFlatnessToleranceCommand(operands);
					case PdfSetGraphicsStateParametersCommand.Name:
						return new PdfSetGraphicsStateParametersCommand(resources, operands);
					case PdfBeginPathCommand.Name:
						return new PdfBeginPathCommand(operands);
					case PdfAppendLineSegmentCommand.Name:
						return new PdfAppendLineSegmentCommand(operands);
					case PdfAppendBezierCurveCommand.Name:
						return new PdfAppendBezierCurveCommand(operands);
					case PdfAppendBezierCurveWithPreviousControlPointCommand.Name:
						return new PdfAppendBezierCurveWithPreviousControlPointCommand(operands);
					case PdfAppendBezierCurveWithNextControlPointCommand.Name:
						return new PdfAppendBezierCurveWithNextControlPointCommand(operands);
					case PdfClosePathCommand.Name:
						return new PdfClosePathCommand();
					case PdfAppendRectangleCommand.Name:
						return new PdfAppendRectangleCommand(operands);
					case PdfStrokePathCommand.Name:
						return new PdfStrokePathCommand();
					case PdfCloseAndStrokePathCommand.Name:
						return new PdfCloseAndStrokePathCommand();
					case PdfFillPathUsingNonzeroWindingNumberRuleCommand.Name:
					case "F":
						return new PdfFillPathUsingNonzeroWindingNumberRuleCommand();
					case PdfFillPathUsingEvenOddRuleCommand.Name:
						return new PdfFillPathUsingEvenOddRuleCommand();
					case PdfFillAndStrokePathUsingNonzeroWindingNumberRuleCommand.Name:
						return new PdfFillAndStrokePathUsingNonzeroWindingNumberRuleCommand();
					case PdfFillAndStrokePathUsingEvenOddRuleCommand.Name:
						return new PdfFillAndStrokePathUsingEvenOddRuleCommand();
					case PdfCloseFillAndStrokePathUsingNonzeroWindingNumberRuleCommand.Name:
						return new PdfCloseFillAndStrokePathUsingNonzeroWindingNumberRuleCommand();
					case PdfCloseFillAndStrokePathUsingEvenOddRuleCommand.Name:
						return new PdfCloseFillAndStrokePathUsingEvenOddRuleCommand();
					case PdfEndPathWithoutFillingAndStrokingCommand.Name:
						return new PdfEndPathWithoutFillingAndStrokingCommand();
					case PdfModifyClippingPathUsingNonzeroWindingNumberRuleCommand.Name:
						return new PdfModifyClippingPathUsingNonzeroWindingNumberRuleCommand();
					case PdfModifyClippingPathUsingEvenOddRuleCommand.Name:
						return new PdfModifyClippingPathUsingEvenOddRuleCommand();
					case PdfBeginTextCommand.Name:
						return new PdfBeginTextCommand();
					case PdfEndTextCommand.Name:
						return new PdfEndTextCommand();
					case PdfSetCharacterSpacingCommand.Name:
						return new PdfSetCharacterSpacingCommand(operands);
					case PdfSetWordSpacingCommand.Name:
						return new PdfSetWordSpacingCommand(operands);
					case PdfSetTextHorizontalScalingCommand.Name:
					case "Th":
						return new PdfSetTextHorizontalScalingCommand(operands);
					case PdfSetTextLeadingCommand.Name:
						return new PdfSetTextLeadingCommand(operands);
					case PdfSetTextFontCommand.Name:
						return new PdfSetTextFontCommand(resources, operands);
					case PdfSetTextRenderingModeCommand.Name:
						return new PdfSetTextRenderingModeCommand(operands);
					case PdfSetTextRiseCommand.Name:
						return new PdfSetTextRiseCommand(operands);
					case PdfStartTextLineWithOffsetsCommand.Name:
						return new PdfStartTextLineWithOffsetsCommand(operands);
					case PdfStartTextLineWithOffsetsAndLeadingCommand.Name:
						return new PdfStartTextLineWithOffsetsAndLeadingCommand(operands);
					case PdfStartTextLineCommand.Name:
						return new PdfStartTextLineCommand();
					case PdfSetTextMatrixCommand.Name:
						return new PdfSetTextMatrixCommand(new PdfTransformationMatrix(operands));
					case PdfShowTextCommand.Name:
						return new PdfShowTextCommand(operands);
					case PdfShowTextOnNextLineCommand.Name:
						return new PdfShowTextOnNextLineCommand(operands);
					case PdfShowTextWithGlyphPositioningCommand.Name:
						return new PdfShowTextWithGlyphPositioningCommand(operands);
					case PdfSetColorSpaceForStrokingOperationsCommand.Name:
						return new PdfSetColorSpaceForStrokingOperationsCommand(resources, operands);
					case PdfSetColorSpaceForNonStrokingOperationsCommand.Name:
						return new PdfSetColorSpaceForNonStrokingOperationsCommand(resources, operands);
					case PdfSetColorForStrokingOperationsCommand.Name:
						return new PdfSetColorForStrokingOperationsCommand(operands);
					case PdfSetColorForNonStrokingOperationsCommand.Name:
						return new PdfSetColorForNonStrokingOperationsCommand(operands);
					case PdfSetColorAdvancedForStrokingOperationsCommand.Name:
						return new PdfSetColorAdvancedForStrokingOperationsCommand(resources, operands);
					case PdfSetColorAdvancedForNonStrokingOperationsCommand.Name:
						return new PdfSetColorAdvancedForNonStrokingOperationsCommand(resources, operands);
					case PdfSetGrayColorSpaceForStrokingOperationsCommand.Name:
						return new PdfSetGrayColorSpaceForStrokingOperationsCommand(operands);
					case PdfSetGrayColorSpaceForNonStrokingOperationsCommand.Name:
						return new PdfSetGrayColorSpaceForNonStrokingOperationsCommand(operands);
					case PdfSetRGBColorSpaceForStrokingOperationsCommand.Name:
						return new PdfSetRGBColorSpaceForStrokingOperationsCommand(operands);
					case PdfSetRGBColorSpaceForNonStrokingOperationsCommand.Name:
						return new PdfSetRGBColorSpaceForNonStrokingOperationsCommand(operands);
					case PdfSetCMYKColorSpaceForStrokingOperationsCommand.Name:
						return new PdfSetCMYKColorSpaceForStrokingOperationsCommand(operands);
					case PdfSetCMYKColorSpaceForNonStrokingOperationsCommand.Name:
						return new PdfSetCMYKColorSpaceForNonStrokingOperationsCommand(operands);
					case PdfPaintShadingPatternCommand.Name:
						return new PdfPaintShadingPatternCommand(resources, operands);
					case PdfPaintXObjectCommand.Name:
						return new PdfPaintXObjectCommand(resources, operands);
					case PdfDesignateMarkedContentPointCommand.Name:
						return new PdfDesignateMarkedContentPointCommand(operands);
					case PdfDesignateMarkerContentPointWithParametersCommand.Name:
						return new PdfDesignateMarkerContentPointWithParametersCommand(resources, operands);
					case PdfMarkedContentCommand.EndToken:
						return new PdfUnknownCommand(name, operands);
					default:
						if (supportType3FontCommands)
							switch (name) {
								case PdfSetCharWidthCommand.Name:
									return new PdfSetCharWidthCommand(operands);
								case PdfSetCacheDeviceCommand.Name:
									return new PdfSetCacheDeviceCommand(operands);
							}
						if (!shouldIgnoreUnknownCommands)
							PdfDocumentReader.ThrowIncorrectDataException();
						return new PdfUnknownCommand(name, operands);
				}
			}
			catch {
				skipValidation = true;
				throw;
			}
			finally {
				if (!skipValidation)
					operands.VerifyCount();
			}
		}
		protected internal virtual void Execute(PdfCommandInterpreter interpreter) {
		}
		protected internal abstract void Write(PdfResources resources, PdfDocumentWritableStream writer);
	}
}
