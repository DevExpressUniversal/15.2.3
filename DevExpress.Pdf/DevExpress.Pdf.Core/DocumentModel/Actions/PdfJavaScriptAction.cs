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
using System.Text;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfJavaScriptAction : PdfAction {
		internal const string Name = "JavaScript";
		const string jsDictionaryKey = "JS";
		readonly string javaScript;
		readonly bool storeAsStream;
		public string JavaScript { get { return javaScript; } }
		protected override string ActionType { get { return Name; } }
		internal PdfJavaScriptAction(PdfReaderDictionary dictionary) : base(dictionary) {
			object js;
			if (dictionary.TryGetValue(jsDictionaryKey, out js)) {
				js = dictionary.Objects.TryResolve(js);
				byte[] bytes;
				PdfReaderStream strm = js as PdfReaderStream;
				if (strm == null) {
					bytes = js as byte[];
					if (bytes == null)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else {
					byte[] rawData = strm.RawData;
					bytes = (rawData.Length == 2 && rawData[0] == PdfDocumentReader.CarriageReturn && rawData[1] == PdfDocumentReader.LineFeed) ? new byte[0] : strm.GetData(true);
					storeAsStream = true;
				}
				javaScript = PdfDocumentReader.ConvertToString(bytes);
			}
			else
				javaScript = String.Empty;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			if (storeAsStream) {
				int length = javaScript.Length;
				byte[] data = new byte[length * 2 + 2];
				data[0] = 254;
				data[1] = 255;
				Encoding.BigEndianUnicode.GetBytes(javaScript, 0, length, data, 2);
				dictionary.Add(jsDictionaryKey, objects.AddStream(data));
			}
			else
				dictionary.AddASCIIString(jsDictionaryKey, javaScript);
			return dictionary;
		}
	}
}
