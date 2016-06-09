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
	public class PdfBead : PdfObject {
		const string dictionaryTypeName = "Bead";
		const string threadDictionaryKey = "T";
		const string nextDictionaryKey = "N";
		const string previousDictionaryKey = "V";
		const string pageDictionaryKey = "P";
		const string locationDictionaryKey = "R";
		readonly int nextNumber;
		readonly int prevNumber;
		readonly PdfArticleThread thread;
		readonly PdfPage page;
		readonly PdfRectangle location;
		PdfBead next;
		PdfBead previous;
		internal int NextNumber { get { return nextNumber; } }
		internal int PrevNumber { get { return prevNumber; } }
		public PdfArticleThread Thread { get { return thread; } }
		public PdfPage Page { get { return page; } }
		public PdfRectangle Location { get { return location; } }
		public PdfBead Next { 
			get { return next; } 
			internal set { next = value; }
		}
		public PdfBead Previous { 
			get { return previous; } 
			internal set { previous = value; }
		}
		internal PdfBead(PdfArticleThread thread, PdfReaderDictionary dictionary) : base(dictionary.Number) {
			if (dictionary == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			this.thread = thread;
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			PdfObjectReference threadReference = dictionary.GetObjectReference(threadDictionaryKey);
			PdfObjectReference nextReference = dictionary.GetObjectReference(nextDictionaryKey);
			PdfObjectReference previousReference = dictionary.GetObjectReference(previousDictionaryKey);
			PdfObjectReference pageReference = dictionary.GetObjectReference(pageDictionaryKey);
			location = dictionary.GetRectangle(locationDictionaryKey);
			if ((type != null && type != dictionaryTypeName) || 
				(threadReference != null && threadReference.Number != thread.ObjectNumber) || nextReference == null || previousReference == null || location == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			nextNumber = nextReference.Number;
			prevNumber = previousReference.Number;
			if (pageReference != null)
				page = dictionary.Objects.GetPage(pageReference.Number);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(PdfDictionary.DictionaryTypeKey, new PdfName(dictionaryTypeName));
			dictionary.Add(nextDictionaryKey, next);
			dictionary.Add(previousDictionaryKey, previous);
			dictionary.Add(pageDictionaryKey, page);
			dictionary.Add(locationDictionaryKey, location);
			dictionary.Add(threadDictionaryKey, thread);
			return dictionary;
		}
	}
}
