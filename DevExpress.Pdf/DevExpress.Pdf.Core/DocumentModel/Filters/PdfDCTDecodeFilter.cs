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
	public class PdfDCTDecodeFilter : PdfFilter {
		internal const string Name = "DCTDecode";
		internal const string ShortName = "DCT";
		const string colorTransformDictionaryKey = "ColorTransform";
		readonly bool colorTransform;
		public bool ColorTransform { get { return colorTransform; } }
		protected internal override string FilterName { get { return Name; } }
		protected internal override PdfImageDataType ImageDataType { get { return PdfImageDataType.DCTDecode; } }
		internal PdfDCTDecodeFilter(PdfReaderDictionary parameters) {
			if (parameters != null) {
				int? colorTransformValue = parameters.GetInteger(colorTransformDictionaryKey);
				if (colorTransformValue.HasValue)
					switch (colorTransformValue.Value) {
						case 0:
							colorTransform = false;
							break;
						case 1:
							colorTransform = true;
							break;
						default:
							PdfDocumentReader.ThrowIncorrectDataException();
							break;
					}
			}
		}
		protected internal override PdfDictionary Write(PdfObjectCollection objects) {
			if (!colorTransform)
				return null;
			PdfDictionary dictionary = new PdfDictionary();
			dictionary.Add(colorTransformDictionaryKey, 1);
			return dictionary;
		}
		protected internal override byte[] Decode(byte[] data) {
			PdfDocumentReader.ThrowIncorrectDataException();
			return data;
		}
	}
}
