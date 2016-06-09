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
	public class PdfTransparencyGroup : PdfObject {
		const string dictionaryType = "Group";
		const string dictionarySubtype = "Transparency";
		const string subtypeDictionaryKey = "S";
		const string colorSpaceDictionaryKey = "CS";
		const string isolatedDictionaryKey = "I";
		const string knockoutDictionaryKey = "K";
		readonly PdfColorSpace colorSpace;
		readonly bool isolated;
		readonly bool knockout;
		public PdfColorSpace ColorSpace { get { return colorSpace; } }
		public bool Isolated { get { return isolated; } }
		public bool Knockout { get { return knockout; } }
		internal PdfTransparencyGroup(PdfReaderDictionary dictionary) : base (dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string subtype = dictionary.GetName(subtypeDictionaryKey);
			if ((type != null && type != dictionaryType) || (!String.IsNullOrEmpty(subtype) && subtype != dictionarySubtype))
				PdfDocumentReader.ThrowIncorrectDataException();
			object value;
			if (dictionary.TryGetValue(colorSpaceDictionaryKey, out value))
				colorSpace = PdfColorSpace.Parse(dictionary.Objects, value);
			isolated = dictionary.GetBoolean(isolatedDictionaryKey) ?? false;
			knockout = dictionary.GetBoolean(knockoutDictionaryKey) ?? false;
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryType));
			dictionary.Add(subtypeDictionaryKey, new PdfName(dictionarySubtype));
			if (colorSpace != null)
				dictionary.Add(colorSpaceDictionaryKey, colorSpace.Write(collection));
			dictionary.Add(isolatedDictionaryKey, isolated, false);
			dictionary.Add(knockoutDictionaryKey, knockout, false);
			return dictionary;
		}
	}
}
