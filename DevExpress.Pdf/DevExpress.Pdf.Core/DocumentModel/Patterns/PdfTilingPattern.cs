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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfTilingType {
		[PdfFieldValue(1)]
		ConstantSpacing,
		[PdfFieldValue(2)]
		NoDistortion,
		[PdfFieldValue(3)]
		FasterTiling
	}
	public class PdfTilingPattern : PdfPattern {
		const string paintTypeDictionaryKey = "PaintType";
		const string tilingTypeDictionaryKey = "TilingType";
		const string boundingBoxDictionaryKey = "BBox";
		const string xStepDictionaryKey = "XStep";
		const string yStepDictionaryKey = "YStep";
		const string resourcesDictionaryKey = "Resources";
		const int coloredPaintType = 1;
		const int uncoloredPaintType = 2;
		readonly bool colored;
		readonly PdfTilingType tilingType;
		readonly PdfRectangle boundingBox;
		readonly double xStep;
		readonly double yStep;
		readonly PdfCommandList commands;
		readonly PdfResources resources;
		public bool Colored { get { return colored; } }
		public PdfTilingType TilingType { get { return tilingType; } }
		public PdfRectangle BoundingBox { get { return boundingBox; } }
		public double XStep { get { return xStep; } }
		public double YStep { get { return yStep; } }
		public PdfCommandList Commands { get { return commands; } }
		internal PdfResources Resources { get { return resources; } }
		protected override int PatternType { get { return 1; } }
		internal PdfTilingPattern(PdfReaderStream stream) : base(stream.Dictionary) {
			PdfReaderDictionary dictionary = stream.Dictionary;
			int? paintType = dictionary.GetInteger(paintTypeDictionaryKey);
			tilingType = PdfEnumToValueConverter.Parse<PdfTilingType>(dictionary.GetInteger(tilingTypeDictionaryKey));
			boundingBox = dictionary.GetRectangle(boundingBoxDictionaryKey);
			double? xStepValue = dictionary.GetNumber(xStepDictionaryKey);
			double? yStepValue = dictionary.GetNumber(yStepDictionaryKey);
			resources = dictionary.GetResources(resourcesDictionaryKey, null, true);
			if (!paintType.HasValue || boundingBox == null || !xStepValue.HasValue || !yStepValue.HasValue || resources == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (paintType.Value) {
				case coloredPaintType:
					colored = true;
					break;
				case uncoloredPaintType:
					colored = false;
					break;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			xStep = xStepValue.Value;
			yStep = yStepValue.Value;
			if (xStep == 0 || yStep == 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			commands = PdfContentStreamParser.GetContent(resources, stream.GetData(true));
		}
		internal PdfTilingPattern(PdfTransformationMatrix matrix, PdfRectangle boundingBox, double xStep, double yStep, PdfDocumentCatalog documentCatalog) : base(matrix) {
			this.boundingBox = boundingBox;
			this.xStep = xStep;
			this.yStep = yStep;
			colored = true;
			tilingType = PdfTilingType.NoDistortion;
			resources = new PdfResources(documentCatalog, null, null, true);
			commands = new PdfCommandList();
		}
		internal PdfTransformationMatrix GetTransformationMatrix(int width, int height) {
			double factorX = width / boundingBox.Width;
			double factorY = height / boundingBox.Height;
			return new PdfTransformationMatrix(factorX, 0, 0, factorY, -boundingBox.Left * factorX, -boundingBox.Bottom * factorY);
		}
		protected override PdfWriterDictionary GetDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.GetDictionary(objects);
			dictionary.Add(paintTypeDictionaryKey, colored ? coloredPaintType : uncoloredPaintType);
			dictionary.Add(tilingTypeDictionaryKey, PdfEnumToValueConverter.Convert<PdfTilingType>(tilingType));
			dictionary.Add(boundingBoxDictionaryKey, boundingBox.ToWritableObject());
			dictionary.Add(xStepDictionaryKey, xStep);
			dictionary.Add(yStepDictionaryKey, yStep);
			dictionary.Add(resourcesDictionaryKey, resources);
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return commands.ToStream(resources, GetDictionary(objects));
		}
	}
}
