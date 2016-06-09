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
	public class PdfShadingPattern : PdfPattern {
		const string shadingDictionaryKey = "Shading";
		const string graphicsStateDictionaryKey = "ExtGState";
		readonly PdfShading shading;
		readonly PdfGraphicsStateParameters graphicsState;
		public PdfShading Shading { get { return shading; } }
		public PdfGraphicsStateParameters GraphicsState { get { return graphicsState; } }
		protected override int PatternType { get { return 2; } }
		internal PdfShadingPattern(PdfReaderDictionary dictionary) : base(dictionary) {
			object value;
			if (!dictionary.TryGetValue(shadingDictionaryKey, out value))
				PdfDocumentReader.ThrowIncorrectDataException();
			shading = dictionary.Objects.GetShading(value);
			graphicsState = dictionary.GetGraphicsStateParameters(graphicsStateDictionaryKey);
		}
		internal PdfShadingPattern(PdfShading shading, PdfTransformationMatrix matrix) : base(matrix) {
			this.shading = shading;
		}
		protected override PdfWriterDictionary GetDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.GetDictionary(objects);
			dictionary.Add(graphicsStateDictionaryKey, graphicsState);
			dictionary.Add(shadingDictionaryKey, shading);
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return GetDictionary(objects);
		}
	}
}
