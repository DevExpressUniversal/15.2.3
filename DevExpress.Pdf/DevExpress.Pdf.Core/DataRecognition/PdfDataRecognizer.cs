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
namespace DevExpress.Pdf.Native {
	public class PdfDataRecognizer : PdfCommandInterpreter {
		public static PdfPageData Recognize(PdfPage page, bool recognizeAnnotationsData) {
			using (PdfDataRecognizer recognizer = new PdfDataRecognizer(page)) {
				recognizer.SaveGraphicsState();
				recognizer.Execute();
				recognizer.RestoreGraphicsState();
				if (recognizeAnnotationsData)
					foreach (PdfAnnotation annot in page.Annotations)
						recognizer.DrawAnnotation(annot, PdfAnnotationAppearanceState.Normal, null);
				return new PdfPageData(recognizer.pageTextData.Parse(), recognizer.pageImageData);
			}
		}
		readonly PdfTextParser pageTextData;
		readonly List<PdfPageImageData> pageImageData;
		PdfDataRecognizer(PdfPage page)
			: base(page, 0) {
			pageTextData = new PdfTextParser(page.CropBox);
			pageImageData = new List<PdfPageImageData>();
			double userUnit = page.UserUnit;
			PdfRectangle cropBox = page.CropBox;
			GraphicsState.TransformationMatrix = PdfTransformationMatrix.Scale(PdfTransformationMatrix.Translate(new PdfTransformationMatrix(), -cropBox.Left, -cropBox.Bottom), userUnit, userUnit);
		}
		protected override void DrawString(PdfStringData stringData, PdfPoint location, double[] glyphOffsets) {
			PdfTextState textState = GraphicsState.TextState;
			pageTextData.AddBlock(stringData, textState.Font, textState.FontSize, FontSizeFactor, textState.CharacterSpacing, TextWidthFactor, TextHeightFactor, glyphOffsets, location, TextAngle);
		}
		protected override void UpdateGraphicsState(PdfGraphicsStateChange change) {
		}
		protected override bool ShouldDrawAnnotation(PdfAnnotation annotation) {
			return !annotation.Flags.HasFlag(PdfAnnotationFlags.NoView) && base.ShouldDrawAnnotation(annotation);
		}
		public override void DrawImage(PdfImage image) {
			pageImageData.Insert(0, new PdfPageImageData(image, GraphicsState.TransformationMatrix));
		}
		public override void DrawShading(PdfShading shading) {
		}
		public override void StrokePaths() {
		}
		public override void FillPaths(bool useNonzeroWindingRule) {
		}
		public override void ClipPaths(bool useNonzeroWindingRule) {
		}
		public override void TransformPaths() {
		}
		public override void TransformAndStrokePaths() {
		}
		public override void CreateNewPaths() {
		}
		public override void AppendRectangle(double x, double y, double width, double height) {
		}
		public override void ClosePath() {
		}
		public override void BeginPath(PdfPoint startPoint) {
		}
		public override void AppendPathLineSegment(PdfPoint endPoint) {
		}
		public override void AppendPathBezierSegment(PdfPoint controlPoint1, PdfPoint controlPoint2, PdfPoint endPoint) {
		}
		public override void AppendPathBezierSegment(PdfPoint controlPoint2, PdfPoint endPoint) {
		}
		public override void SetColorForNonStrokingOperations(PdfColor color) {
		}
		public override void SetColorForStrokingOperations(PdfColor color) {
		}
	}
}
