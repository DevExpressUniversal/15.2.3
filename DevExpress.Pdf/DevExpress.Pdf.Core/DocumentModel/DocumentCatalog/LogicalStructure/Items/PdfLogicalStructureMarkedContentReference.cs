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
	public class PdfLogicalStructureMarkedContentReference : PdfLogicalStructureItem {
		internal const string Type = "MCR";
		const string pageDictionaryKey = "Pg";
		const string streamDictionaryKey = "Stm";
		const string streamOwnDictionaryKey = "StmOwn";
		const string mcidDictionaryKey = "MCID";
		readonly PdfPage page;
		readonly PdfForm form;
		readonly int mcid;
		public PdfPage Page { get { return page; } }
		public PdfForm Form { get { return form; } }
		public int Mcid { get { return mcid; } }
		protected internal override PdfPage ContainingPage { get { return page; } }
		internal PdfLogicalStructureMarkedContentReference(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			PdfObjectCollection objects = dictionary.Objects;
			PdfObjectReference pageReference = dictionary.GetObjectReference(pageDictionaryKey);
			if (pageReference != null) {
				page = objects.DocumentCatalog.FindPage(pageReference);
				if (page == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			PdfObjectReference streamReference = dictionary.GetObjectReference(streamDictionaryKey);
			if (streamReference != null) {
				form = objects.GetXObject(streamReference, null, PdfForm.Type) as PdfForm;
				if (form == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			int? mcidValue = dictionary.GetInteger(mcidDictionaryKey);
			if (!mcidValue.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			mcid = mcidValue.Value;
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.AddName(PdfDictionary.DictionaryTypeKey, Type);
			dictionary.Add(pageDictionaryKey, page);
			dictionary.Add(streamDictionaryKey, form);
			dictionary.Add(mcidDictionaryKey, mcid);
			return dictionary;
		}
	}
}
