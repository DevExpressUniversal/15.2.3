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

using System.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfThumbnailCommandInterpreter : PdfRenderingCommandInterpreter {
		public static Bitmap CreateThumbnailImage(PdfImage image) {
			using (PdfThumbnailCommandInterpreter interpreter = new PdfThumbnailCommandInterpreter()) {
				interpreter.DrawImage(image);
				return interpreter.bitmap;
			}
		}
		Bitmap bitmap;
		PdfThumbnailCommandInterpreter() {
		}
		public override void DrawImage(PdfImage image) {
			PdfImageData imageData = image.GetActualData();
			bitmap = new Bitmap(imageData.Width, imageData.Height);
			PerformRendering(image, imageData, thumbnail => {
				using (Graphics gr = Graphics.FromImage(bitmap))
					gr.DrawImageUnscaled(thumbnail, Point.Empty);
			});
		}
		public override void BeginText() {
		}
		public override void EndText() {
		}
		public override void SetTextMatrix(PdfTransformationMatrix matrix) {
		}
		public override void StrokePaths() {
		}
		public override void FillPaths(bool useNonzeroWindingRule) {
		}
		public override void ClipPaths(bool useNonzeroWindingRule) {
		}
		public override void DrawShading(PdfShading shading) { 
		}
		protected override void DrawString(PdfStringData data, PdfPoint location, double[] glyphOffsets) {
		}
		protected override void UpdateGraphicsState(PdfGraphicsStateChange change) {
		}
	}
}
