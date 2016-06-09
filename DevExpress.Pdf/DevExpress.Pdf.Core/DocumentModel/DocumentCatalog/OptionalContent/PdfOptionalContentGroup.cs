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
	public class PdfOptionalContentGroup : PdfOptionalContent {
		internal const string Type = "OCG";
		const string nameDictionaryKey = "Name";
		const string intentDictionaryKey = "Intent";
		const string usageDictionaryKey = "Usage";
		readonly string name;
		readonly PdfOptionalContentIntent intent;
		readonly PdfOptionalContentUsage usage;
		public string Name { get { return name; } }
		public PdfOptionalContentIntent Intent { get { return intent; } }
		public PdfOptionalContentUsage Usage { get { return usage; } }
		internal PdfOptionalContentGroup(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			name = dictionary.GetString(nameDictionaryKey);
			if (name == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			intent = dictionary.GetOptionalContentIntent(intentDictionaryKey);
			PdfReaderDictionary usageDictionary = dictionary.GetDictionary(usageDictionaryKey);
			if (usageDictionary != null)
				usage = new PdfOptionalContentUsage(usageDictionary);
		}
		protected internal override object Write(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, Type);
			dictionary.Add(nameDictionaryKey, name);
			dictionary.AddIntent(intentDictionaryKey, intent);
			dictionary.Add(usageDictionaryKey, usage);
			return dictionary;
		}
	}
}
