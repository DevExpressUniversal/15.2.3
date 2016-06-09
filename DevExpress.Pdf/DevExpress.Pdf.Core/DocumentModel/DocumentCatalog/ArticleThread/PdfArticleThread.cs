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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfArticleThread : PdfObject {
		const string firstBeadDictionaryKey = "F";
		const string threadInfoDictionaryKey = "I";
		internal static IList<PdfArticleThread> Parse(PdfObjectCollection objects, IList<object> array) {
			if (array == null)
				return null;
			List<PdfArticleThread> threads = new List<PdfArticleThread>();
			foreach (object obj in array) {
				PdfObjectReference reference = obj as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				threads.Add(objects.GetArticleThread(reference));
			}
			return threads;
		}
		readonly PdfDocumentInfo threadInfo;
		PdfBead firstBead;
		public string Title {
			get { return threadInfo.Title; }
			set { threadInfo.Title = value; }
		}
		public string Author {
			get { return threadInfo.Author; }
			set { threadInfo.Author = value; }
		}
		public string Subject {
			get { return threadInfo.Subject; }
			set { threadInfo.Subject = value; }
		}
		public string Keywords {
			get { return threadInfo.Keywords; }
			set { threadInfo.Keywords = value; }
		}
		public string Creator {
			get { return threadInfo.Creator; }
			set { threadInfo.Creator = value; }
		}
		public string Producer {
			get { return threadInfo.Producer; }
			set { threadInfo.Producer = value; }
		}
		public DateTimeOffset? CreationDate {
			get { return threadInfo.CreationDate; }
			set { threadInfo.CreationDate = value; }
		}
		public DateTimeOffset? ModDate {
			get { return threadInfo.ModDate; }
			set { threadInfo.ModDate = value; }
		}
		public DefaultBoolean Trapped {
			get { return threadInfo.Trapped; }
			set { threadInfo.Trapped = value; }
		}
		public PdfBead FirstBead { 
			get { return firstBead; } 
			internal set { firstBead = value; }
		}
		internal PdfArticleThread(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			if (type != null && type != "Thread")
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfObjectCollection objects = dictionary.Objects;
			PdfObjectReference reference = dictionary.GetObjectReference(firstBeadDictionaryKey);
			if (reference != null) {
				firstBead = objects.GetBead(reference.Number, this);
				int firstBeadNumber = firstBead.ObjectNumber;
				int nextNumber = firstBead.NextNumber;
				PdfBead previous = firstBead;
				for (int prevNumber = firstBeadNumber; nextNumber != firstBeadNumber;) {
					PdfBead bead = objects.GetBead(nextNumber, this);
					if (bead.PrevNumber != prevNumber)
						PdfDocumentReader.ThrowIncorrectDataException();
					previous.Next = bead;
					bead.Previous = previous;
					previous = bead;
					prevNumber = nextNumber;
					nextNumber = bead.NextNumber;
				}
				previous.Next = firstBead;
				firstBead.Previous = previous;
			}
			PdfReaderDictionary informationDictionary = dictionary.GetDictionary(threadInfoDictionaryKey);
			threadInfo = informationDictionary == null ? new PdfDocumentInfo() : new PdfDocumentInfo(informationDictionary);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(collection);
			dictionary.Add(firstBeadDictionaryKey, firstBead);
			dictionary.AddIfPresent(threadInfoDictionaryKey, collection.AddObject(threadInfo));
			return dictionary;
		}
	}
}
