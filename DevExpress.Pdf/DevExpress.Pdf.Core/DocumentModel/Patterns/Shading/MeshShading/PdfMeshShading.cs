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
	public abstract class PdfMeshShading : PdfShading {
		const string bitsPerCoordinateDictionaryKey = "BitsPerCoordinate";
		const string bitsPerComponentDictionaryKey = "BitsPerComponent";
		const string bitsPerFlagDictionaryKey = "BitsPerFlag";
		const string decodeDictionaryKey = "Decode";
		static readonly List<int> validBitsPerCoordinate = new List<int> { 1, 2, 4, 8, 12, 16, 24, 32 };
		static readonly List<int> validBitsPerComponent = new List<int> { 1, 2, 4, 8, 12, 16 };
		static readonly List<int> validBitsPerFlag = new List<int> { 2, 4, 8 };
		readonly int bitsPerCoordinate;
		readonly int bitsPerComponent;
		readonly int bitsPerFlag;
		readonly PdfDecodeRange decodeX;
		readonly PdfDecodeRange decodeY;
		readonly PdfDecodeRange[] decodeC;
		readonly byte[] data;
		protected int BitsPerCoordinate { get { return bitsPerCoordinate; } }
		protected int BitsPerComponent { get { return bitsPerComponent; } }
		protected int BitsPerFlag { get { return bitsPerFlag; } }
		protected PdfDecodeRange DecodeX { get { return decodeX; } }
		protected PdfDecodeRange DecodeY { get { return decodeY; } }
		protected PdfDecodeRange[] DecodeC { get { return decodeC; } }
		protected byte[] Data { get { return data; } }
		protected override bool IsFunctionRequired { get { return false; } }
		protected virtual bool HasBitsPerFlag { get { return false; } }
		protected PdfMeshShading(PdfReaderStream stream) : base(stream.Dictionary) {
			if (HasBitsPerFlag) { 
				int? valueBitsPerFlag = stream.Dictionary.GetInteger(bitsPerFlagDictionaryKey);
				if (!valueBitsPerFlag.HasValue)
					PdfDocumentReader.ThrowIncorrectDataException();
				bitsPerFlag = valueBitsPerFlag.Value;
				if (!validBitsPerFlag.Contains(bitsPerFlag))
					PdfDocumentReader.ThrowIncorrectDataException();				
			}
			int decodeCArraySize = Function == null ? ColorSpace.ComponentsCount : 1;
			PdfReaderDictionary dictionary = stream.Dictionary;
			int? valueBitsPerCoordinate = dictionary.GetInteger(bitsPerCoordinateDictionaryKey);
			int? valueBitsPerComponent = dictionary.GetInteger(bitsPerComponentDictionaryKey);
			IList<object> decodeValues = dictionary.GetArray(decodeDictionaryKey);
			if (!valueBitsPerCoordinate.HasValue || !valueBitsPerComponent.HasValue || decodeValues == null || decodeValues.Count != decodeCArraySize * 2 + 4)
				PdfDocumentReader.ThrowIncorrectDataException();
			bitsPerCoordinate = valueBitsPerCoordinate.Value;
			bitsPerComponent = valueBitsPerComponent.Value;
			if (!validBitsPerCoordinate.Contains(bitsPerCoordinate) || !validBitsPerComponent.Contains(bitsPerComponent))
				PdfDocumentReader.ThrowIncorrectDataException();
			decodeX = new PdfDecodeRange(PdfDocumentReader.ConvertToDouble(decodeValues[0]), PdfDocumentReader.ConvertToDouble(decodeValues[1]), bitsPerCoordinate);
			decodeY = new PdfDecodeRange(PdfDocumentReader.ConvertToDouble(decodeValues[2]), PdfDocumentReader.ConvertToDouble(decodeValues[3]), bitsPerCoordinate);
			decodeC = new PdfDecodeRange[decodeCArraySize];
			for (int i = 0, index = 4; i < decodeCArraySize; i++)
				decodeC[i] = new PdfDecodeRange(PdfDocumentReader.ConvertToDouble(decodeValues[index++]), PdfDocumentReader.ConvertToDouble(decodeValues[index++]), bitsPerComponent);
			data = stream.GetData(true);
		}
		protected PdfMeshShading(int bitsPerFlag, int bitsPerComponent, int bitsPerCoordinate, PdfDecodeRange decodeX, PdfDecodeRange decodeY, PdfDecodeRange[] decodeC, PdfObjectList<PdfCustomFunction> functions)
			: base(functions) {
			this.bitsPerFlag = bitsPerFlag;
			this.bitsPerCoordinate= bitsPerCoordinate;
			this.bitsPerComponent = bitsPerComponent;
			this.decodeX = decodeX;
			this.decodeY = decodeY;
			this.decodeC = decodeC;
		}
		protected PdfIntegerStreamReader CreateIntegerStreamReader() { 
			return new PdfIntegerStreamReader(bitsPerFlag, bitsPerCoordinate, bitsPerComponent, decodeX, decodeY, decodeC, data);
		}
		protected override PdfDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(bitsPerCoordinateDictionaryKey, bitsPerCoordinate);
			dictionary.Add(bitsPerComponentDictionaryKey, bitsPerComponent);
			if (HasBitsPerFlag)
				dictionary.Add(bitsPerFlagDictionaryKey, bitsPerFlag);
			int decodeCArraySize = decodeC.Length;
			object[] decodeArray = new object[decodeCArraySize * 2 + 4];
			decodeArray[0] = decodeX.Min;
			decodeArray[1] = decodeX.Max;
			decodeArray[2] = decodeY.Min;
			decodeArray[3] = decodeY.Max;
			for (int i = 0, index = 4; i < decodeCArraySize; i++) {
				PdfDecodeRange decodeRange = decodeC[i];
				decodeArray[index++] = decodeRange.Min;
				decodeArray[index++] = decodeRange.Max;
			}
			dictionary.Add(decodeDictionaryKey, decodeArray);
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return new PdfCompressedStream(CreateDictionary(objects), data != null ? data : GetData());
		}
		protected virtual byte[] GetData(){
			return null;
		}
	}
}
