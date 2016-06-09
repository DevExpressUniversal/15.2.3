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

using DevExpress.Utils;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfLZWDecodeFilter : PdfFlateLZWDecodeFilter {
		internal const string Name = "LZWDecode";
		internal const string ShortName = "LZW";
		const int defaultEarlyChange = 1;
		const string earlyChangeDictionaryKey = "EarlyChange";
		readonly bool earlyChange = true;
		public bool EarlyChange { get { return earlyChange; } }
		protected internal override string FilterName { get { return Name; } }
		internal PdfLZWDecodeFilter(PdfReaderDictionary parameters) : base(parameters) {
			if (parameters != null) {
				int? earlyChangeValue = parameters.GetInteger(earlyChangeDictionaryKey);
				if (earlyChangeValue.HasValue)
					switch (earlyChangeValue.Value) {
						case 0:
							earlyChange = false;
							break;
						case 1:
							break;
						default:
							PdfDocumentReader.ThrowIncorrectDataException();
							break;
					}
			}
		}
		protected internal override PdfDictionary Write(PdfObjectCollection objects) {
			PdfDictionary dictionary = base.Write(objects);
			if (!earlyChange) {
				if (dictionary == null)
					dictionary = new PdfDictionary();
				dictionary.Add(earlyChangeDictionaryKey, 0);
			}
			return dictionary;
		}
		protected override byte[] PerformDecode(byte[] data) {
			return LZWDecoder.Decode(data, 9);
		}
	}
}
