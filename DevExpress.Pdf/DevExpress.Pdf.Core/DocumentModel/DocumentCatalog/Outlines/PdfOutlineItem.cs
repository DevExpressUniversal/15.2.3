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
	public abstract class PdfOutlineItem : PdfObject {
		const string firstDictionaryKey = "First";
		const string lastDictionaryKey = "Last";
		protected const string CountDictionaryKey = "Count";
		bool closed;
		PdfOutline first;
		PdfOutline last;
		int count;
		public bool Closed {
			get { return closed; }
			set { closed = value; }
		}
		public PdfOutline First {
			get { return first; }
			internal set { first = value; }
		}
		public PdfOutline Last {
			get { return last; }
			internal set { last = value; }
		}
		public int Count { get { return count; } }
		protected PdfOutlineItem() { 
		}
		protected PdfOutlineItem(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			PdfReaderDictionary outlineDictionary = dictionary.GetDictionary(firstDictionaryKey);
			if (outlineDictionary != null) {
				first = new PdfOutline(this, null, outlineDictionary);
				last = first;
				for (PdfReaderDictionary nextDictionary = outlineDictionary.GetDictionary(PdfOutline.NextDictionaryKey); nextDictionary != null; nextDictionary = nextDictionary.GetDictionary(PdfOutline.NextDictionaryKey)) {
					PdfOutline next = new PdfOutline(this, last, nextDictionary);
					last.Next = next;
					last = next;
				}
				if (dictionary.GetDictionary(lastDictionaryKey) == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			count = dictionary.GetInteger(CountDictionaryKey) ?? 0;
			if (first == null) {
				if (count != 0)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				closed = count <= 0;
				UpdateCount();
			}
		}
		internal int UpdateCount() {
			count = 0;
			for (PdfOutline outline = first; outline != null; outline = outline.Next) {
				count++;
				if (!outline.Closed)
					count += outline.UpdateCount();
			}
			return count;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(firstDictionaryKey, first);
			dictionary.Add(lastDictionaryKey, last);
			return dictionary;
		}
	}
}
