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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCustomCompositeFontEncoding : PdfCompositeFontEncoding {
		const string dictionaryType = "CMap";
		const string nameDictionaryKey = "CMapName";
		const string cidSystemInfoDictionaryKey = "CIDSystemInfo";
		const string wModeDictionaryKey = "WMode";
		const string useCMapDictionaryKey = "UseCMap";
		readonly string name;
		readonly PdfCIDSystemInfo cidSysteminfo;
		readonly PdfCompositeFontEncoding baseEncoding;
		readonly PdfCharacterMapping characterMapping;
		readonly bool isVertical;
		public string Name { get { return name; } }
		public PdfCIDSystemInfo CIDSystemInfo { get { return cidSysteminfo; } }
		public PdfCompositeFontEncoding BaseEncoding { get { return baseEncoding; } }
		public PdfCharacterMapping CharacterMapping { get { return characterMapping; } }
		public override bool IsVertical { get { return isVertical; } }
		internal PdfCustomCompositeFontEncoding(PdfReaderStream stream) {
			PdfReaderDictionary dictionary = stream.Dictionary;
			name = dictionary.GetName(nameDictionaryKey);
			PdfReaderDictionary cidDictionary = dictionary.GetDictionary(cidSystemInfoDictionaryKey);
			if (cidDictionary != null)
				cidSysteminfo = new PdfCIDSystemInfo(cidDictionary);
			int? wMode = dictionary.GetInteger(wModeDictionaryKey);
			if (wMode.HasValue)
				switch (wMode.Value) {
					case 0:
						isVertical = false;
						break;
					case 1:
						isVertical = true;
						break;
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						break;
				}
			object useCMap;
			if (dictionary.TryGetValue(useCMapDictionaryKey, out useCMap))
				baseEncoding = PdfCompositeFontEncoding.Create(useCMap);
			characterMapping = PdfCMapStreamParser.Parse(stream.GetData(true));
		}
		protected internal override PdfStringData GetStringData(byte[] bytes, double[] glyphOffsets) {
			return characterMapping.GetStringData(bytes, glyphOffsets);
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return objects.AddObject(this);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, dictionaryType);
			dictionary.AddName(nameDictionaryKey, name);
			dictionary.Add(cidSystemInfoDictionaryKey, cidSysteminfo);
			if (isVertical)
				dictionary.Add(wModeDictionaryKey, 1);
			dictionary.Add(useCMapDictionaryKey, baseEncoding, null);
			return new PdfCompressedStream(dictionary, characterMapping.Data);
		}
	}
}
