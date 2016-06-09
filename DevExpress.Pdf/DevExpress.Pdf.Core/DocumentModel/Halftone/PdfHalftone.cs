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
	public abstract class PdfHalftone : PdfObject {
		protected const string HalftoneTypeDictionaryKey = "HalftoneType";
		protected const string HalftoneNameDictionaryKey = "HalftoneName";
		const string halftoneType = "Halftone";
		internal static PdfHalftone Parse(object value) {
			PdfName name = value as PdfName;
			if (name == null) {
				PdfReaderDictionary dictionary = value as PdfReaderDictionary;
				if (dictionary == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
				int? halftoneSubtype = dictionary.GetInteger(HalftoneTypeDictionaryKey);
				if ((type != null && type != halftoneType) || !halftoneSubtype.HasValue)
					PdfDocumentReader.ThrowIncorrectDataException();
				switch (halftoneSubtype.Value) {
					case PdfStandardHalftone.Number:
						return new PdfStandardHalftone(dictionary);
					case PdfSeparateHalftone.Number:
						return new PdfSeparateHalftone(dictionary);
					default:
						PdfDocumentReader.ThrowIncorrectDataException();
						return null;
				}
			}
			if (name.Name != PdfDefaultHalftone.Id)
				PdfDocumentReader.ThrowIncorrectDataException();
			return PdfDefaultHalftone.Instance;
		}
		readonly string name;
		public string Name { get { return name; } }
		protected PdfHalftone(string name) : base (PdfObject.DirectObjectNumber) {
			this.name = name;
		}
		protected PdfHalftone(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			name = dictionary.GetString(HalftoneNameDictionaryKey);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(halftoneType));
			dictionary.Add(HalftoneNameDictionaryKey, PdfDocumentStream.ConvertToArray(name), null);
			return dictionary;
		}
		protected internal virtual bool IsSame(PdfHalftone halftone) {
			return String.Equals(name, halftone.name);
		}
		protected internal virtual object CreateWriteableObject(PdfObjectCollection objects) {
			return objects.AddObject(this);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
	}
}
