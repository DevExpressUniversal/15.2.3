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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Utils;
using ModelField = DevExpress.XtraRichEdit.Model.Field;
using ModelHyperlinkInfo = DevExpress.XtraRichEdit.Model.HyperlinkInfo;
using ModelDocumentIntervalEventHandler = DevExpress.XtraRichEdit.Model.DocumentIntervalEventHandler;
using ModelDocumentIntervalEventArgs = DevExpress.XtraRichEdit.Model.DocumentIntervalEventArgs;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model.History;
namespace DevExpress.XtraRichEdit.API.Native {
	#region Hyperlink
	[ComVisible(true)]
	public interface Hyperlink {
		string Target { get; set; }
		string NavigateUri { get; set; }
		string Anchor { get; set; }
		string ToolTip { get; set; }
		DocumentRange Range { get; }
		bool Visited { get; set; }
	}
	#endregion
	#region ReadOnlyHyperlinkCollection
	[ComVisible(true)]
	public interface ReadOnlyHyperlinkCollection : ISimpleCollection<Hyperlink> {
		ReadOnlyHyperlinkCollection Get(DocumentRange range);
	}
	#endregion
	#region HyperlinkCollection
	[ComVisible(true)]
	public interface HyperlinkCollection : ReadOnlyHyperlinkCollection {
		Hyperlink Create(DocumentPosition start, int length);
		Hyperlink Create(DocumentRange range);
		void Remove(Hyperlink hyperlink);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using DevExpress.Office.Utils;
	#region NativeHyperlink
	public class NativeHyperlink : Hyperlink {
		bool isValid;
		readonly NativeSubDocument document;
		readonly ModelPieceTable pieceTable;
		readonly ModelField field;
		readonly NativeField nativeField;
		public NativeHyperlink(NativeSubDocument document, ModelPieceTable pieceTable, ModelField field) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(field, "field");
			this.document = document;
			this.pieceTable = pieceTable;
			this.field = field;
			this.nativeField = FindNativeField(field);
			this.isValid = true;
		}
		public NativeSubDocument Document { get { return document; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		protected internal ModelHyperlinkInfo HyperlinkInfo { get { return pieceTable.HyperlinkInfos[field.Index]; } }
		protected internal ModelField Field { get { return field; } }
		#region Hyperlink Members
		public string Target {
			get {
				CheckValid();
				return HyperlinkInfo.Target;
			}
			set {
				CheckValid();
				ChangeHyperlinkTarget(value);
			}
		}
		void ChangeHyperlinkTarget(string value) {
			ModelHyperlinkInfo newInfo = HyperlinkInfo.Clone();
			newInfo.Target = value;
			UpdateHyperlink(newInfo);
		}
		public string NavigateUri {
			get {
				CheckValid();
				return HyperlinkInfo.NavigateUri;
			}
			set {
				CheckValid();
				ChangeHyperlinkNavigateUri(value);
			}
		}
		void ChangeHyperlinkNavigateUri(string value) {
			ModelHyperlinkInfo newInfo = HyperlinkInfo.Clone();
			newInfo.NavigateUri = value;
			UpdateHyperlink(newInfo);
		}
		public string Anchor {
			get {
				CheckValid();
				return HyperlinkInfo.Anchor;
			}
			set {
				CheckValid();
				ChangeHyperlinkAnchor(value);
			}
		}
		void ChangeHyperlinkAnchor(string value) {
			ModelHyperlinkInfo newInfo = HyperlinkInfo.Clone();
			newInfo.Anchor = value;
			UpdateHyperlink(newInfo);
		}
		public string ToolTip {
			get {
				CheckValid();
				return HyperlinkInfo.ToolTip;
			}
			set {
				CheckValid();
				ChangeHyperlinkToolTip(value);
			}
		}
		void ChangeHyperlinkToolTip(string value) {
			ModelHyperlinkInfo newInfo = HyperlinkInfo.Clone();
			newInfo.ToolTip = value;
			UpdateHyperlink(newInfo);
		}
		public DocumentRange Range {
			get {
				CheckValid();
				return nativeField.ResultRange;
			}
		}
		public bool Visited {
			get {
				CheckValid();
				return HyperlinkInfo.Visited;
			}
			set {
				CheckValid();
				HyperlinkInfo.Visited = value;
			}
		}
		#endregion
		void UpdateHyperlink(ModelHyperlinkInfo newInfo) {
			Document.PieceTable.ModifyHyperlinkCode(field, newInfo);
		}
		protected NativeField FindNativeField(ModelField field) {
			NativeFieldCollection nativeFields = (NativeFieldCollection)Document.Fields;
			int count = nativeFields.Count;
			for (int i = 0; i < count; i++) {
				NativeField nativeField = nativeFields[i];
				if (nativeField.Field == field)
					return nativeField;
			}
			return new NativeField(document, field);
		}
		protected void CheckValid() {
			if (!IsValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedHyperlinkError);
		}
	}
	#endregion
	#region NativeHyperlinkCollection
	public class NativeHyperlinkCollection : List<NativeHyperlink>, HyperlinkCollection {
		readonly NativeSubDocument document;
		public NativeHyperlinkCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		protected internal NativeHyperlink Find(int fieldIndex) {
			for (int i = 0; i < Count; i++) {
				if (this[i].Field.Index == fieldIndex)
					return this[i];
			}
			return null;
		}
		#region ISimpleCollection<Hyperlink> Members
		Hyperlink ISimpleCollection<Hyperlink>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<Hyperlink> Members
		IEnumerator<Hyperlink> IEnumerable<Hyperlink>.GetEnumerator() {
			return new EnumeratorAdapter<Hyperlink, NativeHyperlink>(this.GetEnumerator());
		}
		#endregion
		public Hyperlink Create(DocumentPosition start, int length) {
			if (document != null) {
				document.CheckValid();
				Guard.ArgumentNotNull(start, "start");
				Guard.ArgumentNonNegative(length, "length");
				document.CheckDocumentPosition(start);
				int prevCount = this.Count;
				PieceTable.CreateHyperlink(start.LogPosition, length, new ModelHyperlinkInfo());
				if (this.Count > prevCount)
					return this[prevCount];
			}
			return null;
		}
		public Hyperlink Create(DocumentRange range) {
			return Create(range.Start, range.Length);
		}
		void HyperlinkCollection.Remove(Hyperlink hyperlink) {
			if (document != null) {
				document.CheckValid();
				Guard.ArgumentNotNull(hyperlink, "hyperlink");
				PieceTable.DeleteHyperlink(((NativeHyperlink)hyperlink).Field);
			}
		}
		public ReadOnlyHyperlinkCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelRunIndex firstRunIndex = nativeRange.NormalizedStart.Position.RunIndex;
			ModelRunIndex lastRunIndex = nativeRange.NormalizedEnd.Position.RunIndex;
			NativeHyperlinkCollection result = new NativeHyperlinkCollection(document);
			int count = PieceTable.HyperlinkInfos.Count;
			for (int i = 0; i < count; i++) {
				ModelRunIndex firstIndex = this[i].Field.FirstRunIndex;
				ModelRunIndex lastIndex = this[i].Field.LastRunIndex;
				if ((firstIndex >= firstRunIndex) && (lastIndex <= lastRunIndex)) {
					NativeHyperlink hyperlink = new NativeHyperlink(document, PieceTable, PieceTable.Fields[i]);
					result.Add(hyperlink);
				}
			}
			return result;
		}
	}
	#endregion
}
