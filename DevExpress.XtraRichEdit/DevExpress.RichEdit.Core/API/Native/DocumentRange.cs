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
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.API.Native {
	#region DocumentRange
	[ComVisible(true)]
	public interface DocumentRange {
		DocumentPosition Start { get; }
		DocumentPosition End { get; }
		int Length { get; }
		bool Contains(DocumentPosition pos);
		SubDocument BeginUpdateDocument();
		void EndUpdateDocument(SubDocument document);
	}
	#endregion
	#region FixedRange
	public class FixedRange {
		readonly int start;
		readonly int end;
		readonly int length;
		public FixedRange(int start, int length) {
			Guard.ArgumentNonNegative(start, "start");
			Guard.ArgumentNonNegative(length, "length");
			this.start = start;
			this.length = length;
			this.end = start + length - 1;
		}
		public int Start { get { return start; } }
		public int Length { get { return length; } }
		private int End { get { return end; } }
		public bool Contains(FixedRange range) {
			return Start <= range.Start && End >= range.End;
		}
		public bool Contains(int position) {
			return Start <= position && End >= position;
		}
		public bool Intersect(FixedRange range) {
			return (range.Start <= Start && range.End >= Start) || (range.Start <= End && range.End >= End) || Contains(range);
		}
		public override bool Equals(object obj) {
			FixedRange range = obj as FixedRange;
			if (range == null)
				return false;
			return Start == range.start && Length == range.length;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override string ToString() {
			return String.Format("{0}, {1}", Start, Length);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelParagraphIndex = DevExpress.XtraRichEdit.Model.ParagraphIndex;
	using ModelParagraphCollection = DevExpress.Office.Utils.List<DevExpress.XtraRichEdit.Model.Paragraph, DevExpress.XtraRichEdit.Model.ParagraphIndex>;
	#region NativeDocumentRange
	public class NativeDocumentRange : DocumentRange {
		readonly NativeDocumentPosition start;
		readonly NativeDocumentPosition end;
		internal NativeDocumentRange(NativeSubDocument document, ModelPosition start, ModelPosition end)
			: this(new NativeDocumentPosition(document, start.Clone()), new NativeDocumentPosition(document, end.Clone())) {
		}
		internal NativeDocumentRange(NativeDocumentPosition start, NativeDocumentPosition end) {
			this.start = start;
			this.end = end;
		}
		public NativeDocumentPosition Start { get { return start; } }
		public NativeDocumentPosition End { get { return end; } }
		public NativeDocumentPosition NormalizedStart {
			get {
				if (start.LogPosition <= end.LogPosition)
					return start;
				else
					return end;
			}
		}
		public NativeDocumentPosition NormalizedEnd {
			get {
				if (start.LogPosition <= end.LogPosition)
					return end;
				else
					return start;
			}
		}
		public int Length { get { return Math.Abs(end.LogPosition - start.LogPosition); } }
		public bool Contains(DocumentPosition pos) {
			if (start.LogPosition <= end.LogPosition)
				return start.LogPosition <= pos.LogPosition && pos.LogPosition < end.LogPosition;
			else
				return end.LogPosition <= pos.LogPosition && pos.LogPosition < start.LogPosition;
		}
		#region DocumentRange Members
		DocumentPosition DocumentRange.Start { get { return start; } }
		DocumentPosition DocumentRange.End { get { return end; } }
		#endregion
		public SubDocument BeginUpdateDocument() {
			start.Document.ReferenceCount++;
			return start.Document;
		}
		public void EndUpdateDocument(SubDocument document) {
			NativeSubDocument nativeDocument = (NativeSubDocument)document;
			nativeDocument.ReferenceCount--;
		}
	}
	#endregion
}
