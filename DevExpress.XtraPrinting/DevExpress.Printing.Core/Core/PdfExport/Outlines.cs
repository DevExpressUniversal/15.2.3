#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.IO;
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfOutlineEntryCollection : CollectionBase {		
		public PdfOutlineEntry this[int index] { get { return (PdfOutlineEntry)List[index]; }
		} 
		public PdfOutlineEntryCollection() {
		}
		public int IndexOf(PdfOutlineEntry item) {
			return InnerList.IndexOf(item);
		}
		public int Add(PdfOutlineEntry item) {
			System.Diagnostics.Debug.Assert(item != null);
			return List.Contains(item) ? IndexOf(item) : List.Add(item);
		}
	}
	public class PdfOutlineItem : PdfDocumentDictionaryObject {		
		PdfOutlineItem parent;
		PdfOutlineEntryCollection entries = new PdfOutlineEntryCollection();
		public PdfOutlineItem Parent { get { return parent; }
		}
		public PdfOutlineEntryCollection Entries { get { return entries; }
		}
		public PdfOutlineEntry First { get { return entries.Count > 0 ? entries[0] : null; }
		} 
		public PdfOutlineEntry Last { get { return entries.Count > 0 ? entries[entries.Count - 1] : null; }
		}
		public PdfOutlineItem(PdfOutlineItem parent, bool compressed) : base(compressed) {
			this.parent = parent;
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			foreach(PdfOutlineEntry entry in entries)
				entry.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			for(int i = 0; i < Entries.Count; i++) 
				Entries[i].Write(writer);
		}
		public override void FillUp() {
			if(Entries.Count > 0) {	
				Dictionary.Add("Count", Entries.Count); 
				Dictionary.Add("First", First.InnerObject);
				Dictionary.Add("Last", Last.InnerObject);
			}
			foreach(PdfOutlineEntry entry in entries)
				entry.FillUp();
			base.FillUp();
		}
	}
	public class PdfOutlines : PdfOutlineItem {
		public bool Active { get { return Entries.Count > 0; } }
		public PdfOutlines(bool compressed) : base(null, compressed) {
		}
	}
	public class PdfOutlineEntry : PdfOutlineItem {
		string title = String.Empty;
		DestinationInfo destInfo;
		PdfDestination dest;
		public int DestPageIndex { get { return destInfo != null ? destInfo.DestPageIndex : -1; }
		}
		public float DestTop { get { return destInfo != null ? destInfo.DestTop : 0; }
		}
		public int Index { get { return Parent != null ? Parent.Entries.IndexOf(this) : -1; }
		}
		public PdfOutlineEntry Next { get { return GetOutlineEntry(Index + 1); }
		}
		public PdfOutlineEntry Prev { get { return GetOutlineEntry(Index - 1); }
		}
		public PdfOutlineEntry(PdfOutlineItem parent, string title, DestinationInfo destInfo, bool compressed) : base(parent, compressed) {
			this.destInfo = destInfo;
			this.title = title;
		}
		public void SetDestination(PdfDestination dest) {
			this.dest = dest;
		}
		public override void FillUp() {
			if(dest != null) {
				Dictionary.Add("Title", new PdfTextUnicode(title));
				Dictionary.Add("Dest", dest);
				if(Prev != null)
					Dictionary.Add("Prev", Prev.InnerObject);
				if(Next != null)
					Dictionary.Add("Next", Next.InnerObject);
				if(Parent != null)
					Dictionary.Add("Parent", Parent.InnerObject);
			}
			base.FillUp();
		}
		private PdfOutlineEntry GetOutlineEntry(int index) {
			if(Parent != null) {
				if(index >= 0 && index < Parent.Entries.Count) 
					return Parent.Entries[index];
			}
			return null;
		}
	}
}
