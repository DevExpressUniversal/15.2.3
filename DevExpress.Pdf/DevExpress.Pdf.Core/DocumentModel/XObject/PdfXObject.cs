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
	public abstract class PdfXObject : PdfObject {
		const string dictionaryType = "XObject";
		const string structParentDictionaryKey = "StructParent";
		const string openPrepressInterfaceDictionaryKey = "OPI";
		internal static PdfXObject Parse(PdfReaderStream stream, PdfResources parentResources, string defaultSubtype) {
			PdfReaderDictionary dictionary = stream.Dictionary;
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			string subType = dictionary.GetName(PdfDictionary.DictionarySubtypeKey) ?? defaultSubtype;
			if ((type != null && type != dictionaryType && type != "Xobject") || String.IsNullOrEmpty(subType))
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (subType) {
				case PdfImage.Type:
					return new PdfImage(stream);
				case PdfForm.Type:
					return PdfForm.Create(stream, parentResources);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		readonly PdfMetadata metadata;
		readonly int? structParent;
		readonly PdfOpenPrepressInterface openPrepressInterface;
		readonly PdfOptionalContent optionalContent;
		public PdfMetadata Metadata { get { return metadata; } }
		public int? StructParent { get { return structParent; } }
		public PdfOpenPrepressInterface OpenPrepressInterface { get { return openPrepressInterface; } }
		public PdfOptionalContent OptionalContent { get { return optionalContent; } }
		protected PdfXObject() : base(PdfObject.DirectObjectNumber) { 
		}
		protected PdfXObject(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			metadata = dictionary.GetMetadata();
			structParent = dictionary.GetInteger(structParentDictionaryKey);
			openPrepressInterface = PdfOpenPrepressInterface.Create(dictionary.GetDictionary(openPrepressInterfaceDictionaryKey));
			optionalContent = dictionary.GetOptionalContent();
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateStream(objects);
		}
		protected virtual PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, dictionaryType);
			dictionary.Add(PdfMetadata.Name, metadata);
			dictionary.AddNullable(structParentDictionaryKey, structParent);
			if (openPrepressInterface != null)
				dictionary.Add(openPrepressInterfaceDictionaryKey, openPrepressInterface.ToWritableObject(objects));
			dictionary.Add(PdfOptionalContent.DictionaryKey, optionalContent);
			return dictionary;
		}
		protected abstract PdfStream CreateStream(PdfObjectCollection objects);
	}
}
