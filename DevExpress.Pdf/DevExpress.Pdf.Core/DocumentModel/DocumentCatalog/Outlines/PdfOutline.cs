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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfOutline : PdfOutlineItem {
		internal const string NextDictionaryKey = "Next";
		const string titleDictionaryKey = "Title";
		const string prevDictionaryKey = "Prev";
		const string destinationDictionaryKey = "Dest";
		const string actionDictionaryKey = "A";
		const string colorDictionaryKey = "C";
		const string flagsDictionaryKey = "F";
		const int italicFlag = 1;
		const int boldFlag = 2;
		internal static PdfOutline CreateOutlineTree(PdfOutlineItem parent, IList<PdfBookmark> bookmarks) {
			int count = bookmarks.Count;
			if (count == 0)
				return null;
			PdfOutline first = new PdfOutline(parent, null, bookmarks[0]);
			PdfOutline last = first;
			for (int i = 1; i < count; i++) {
				PdfOutline next = new PdfOutline(parent, last, bookmarks[i]);
				last.Next = next;
				last = next;
			}
			parent.Last = last;
			return first;
		}
		readonly PdfDocumentCatalog documentCatalog;
		readonly string title;
		readonly PdfOutlineItem parent;
		readonly PdfDestinationObject destination;
		readonly PdfAction action;
		readonly PdfColor color;
		readonly bool isItalic;
		readonly bool isBold;
		PdfOutline prev;
		PdfOutline next;
		internal PdfDestinationObject DestinationObject { get { return destination; } }
		public string Title { get { return title; } }
		public PdfOutlineItem Parent { get { return parent; } }
		public PdfDestination Destination { get { return destination == null ? null : destination.GetDestination(documentCatalog, true); } }
		public PdfAction Action { get { return action; } }
		public PdfColor Color { get { return color; } }
		public bool IsItalic { get { return isItalic; } }
		public bool IsBold { get { return isBold; } }
		public PdfOutline Prev {
			get { return prev; }
			internal set { prev = value; }
		}
		public PdfOutline Next {
			get { return next; }
			internal set { next = value; }
		}
		PdfOutline(PdfOutlineItem parent, PdfOutline prev, PdfBookmark bookmark) {
			this.parent = parent;
			this.prev = prev;
			title = bookmark.Title;
			destination = bookmark.DestinationObject;
			action = bookmark.Action;
			isItalic = bookmark.IsItalic;
			isBold = bookmark.IsBold;
			PdfRGBColor textColor = bookmark.TextColor;
			color = new PdfColor(textColor.R, textColor.G, textColor.B);
			Closed = bookmark.IsInitiallyClosed;
			First = CreateOutlineTree(this, bookmark.Children);
			UpdateCount();
		}
		internal PdfOutline(PdfOutlineItem parent, PdfOutline prev, PdfReaderDictionary dictionary) : base(dictionary) {
			this.parent = parent;
			this.prev = prev;
			documentCatalog = dictionary.Objects.DocumentCatalog;
			ObjectNumber = dictionary.Number;
			title = dictionary.GetString(titleDictionaryKey) ?? String.Empty;
			destination = dictionary.GetDestination(destinationDictionaryKey);
			action = dictionary.GetAction(actionDictionaryKey);
			if (prev == null && dictionary.GetDictionary(prevDictionaryKey) != null)
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<double> components = dictionary.GetArray<double>(colorDictionaryKey, o => PdfDocumentReader.ConvertToDouble(o));
			if (components == null)
				color = new PdfColor(0, 0, 0);
			else {
				if (components.Count != 3)
					PdfDocumentReader.ThrowIncorrectDataException();
				color = new PdfColor(components[0], components[1], components[2]);
			}
			int flags = dictionary.GetInteger(flagsDictionaryKey) ?? 0;
			isItalic = (flags & italicFlag) > 0;
			isBold = (flags & boldFlag) > 0;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			object result = base.ToWritableObject(objects);
			PdfWriterDictionary dictionary = result as PdfWriterDictionary;
			if (dictionary != null) {
				dictionary.Add(titleDictionaryKey, title);
				dictionary.Add("Parent", parent);
				dictionary.Add(prevDictionaryKey, prev);
				dictionary.Add(NextDictionaryKey, next);
				dictionary.Add(CountDictionaryKey, Closed ? -Count : Count, 0);
				if (destination != null)
					dictionary.Add(destinationDictionaryKey, destination.ToWriteableObject(documentCatalog, objects, true));
				dictionary.Add(actionDictionaryKey, action);
				foreach (double component in color.Components)
					if (component != 0) {
						dictionary.Add(colorDictionaryKey, color);
						break;
					}
				dictionary.Add(flagsDictionaryKey, (isItalic ? italicFlag : 0) | (isBold ? boldFlag : 0), 0);
			}
			return result;
		}
	}
}
