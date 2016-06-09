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
using System.Text;
using DevExpress.XtraRichEdit.Utils;
using ModelField = DevExpress.XtraRichEdit.Model.Field;
using DocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
using ModelFieldCollection = DevExpress.XtraRichEdit.Model.FieldCollection;
using DocumentModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.API.Native {
	[ComVisible(true)]
	public interface Field {
		DocumentRange Range { get; }
		DocumentRange CodeRange { get; }
		DocumentRange ResultRange { get; }
		bool ShowCodes { get; set; }
		bool Locked { get; set; }
		void Update();
		Field Parent { get; }
	}
	#region ReadOnlyFieldCollection
	[ComVisible(true)]
	public interface ReadOnlyFieldCollection : ISimpleCollection<Field> {
		ReadOnlyFieldCollection Get(DocumentRange range);
	}
	#endregion
	[ComVisible(true)]
	public interface FieldCollection : ReadOnlyFieldCollection {
		void Update();
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		Field Add(DocumentRange range);
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		Field Add(DocumentPosition start, string code);
		Field Create(DocumentRange range);
		Field Create(DocumentPosition start, string code);
	}
	#region MergeFieldName
	[ComVisible(true)]
	public class MergeFieldName : IComparable<MergeFieldName> {
		#region Fields
		string name;
		string displayName;
		#endregion
		public MergeFieldName() {
			this.name = String.Empty;
			this.displayName = String.Empty;
		}
		public MergeFieldName(string name) {
			this.name = name;
			this.displayName = name;
		}
		public MergeFieldName(string name, string displayName) {
			this.name = name;
			this.displayName = displayName;
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MergeFieldNameName")]
#endif
		public string Name { get { return name; } set { name = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MergeFieldNameDisplayName")]
#endif
		public string DisplayName { get { return displayName; } set { displayName = value; } }
		#endregion
		public override string ToString() {
			return DisplayName;
		}
		public int CompareTo(MergeFieldName other) {
			return String.Compare(DisplayName, other.DisplayName);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelField = DevExpress.XtraRichEdit.Model.Field;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using DevExpress.Office.Utils;
	public class NativeField : Field {
		readonly NativeSubDocument document;
		readonly ModelField field;
		bool isValid;
		internal NativeField(NativeSubDocument document, ModelField field) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(field, "field");
			this.document = document;
			this.field = field;
			this.isValid = true;
		}
		#region Field Members
		public DocumentRange Range {
			get {
				CheckValid();
				return GetRange();
			}
		}
		public DocumentRange CodeRange {
			get {
				CheckValid();
				return GetCodeRange();
			}
		}
		public DocumentRange ResultRange {
			get {
				CheckValid();
				return GetResultRange();
			}
		}
		public bool ShowCodes {
			get {
				CheckValid();
				return field.IsCodeView;
			}
			set {
				CheckValid();
				if(ShowCodes != value)
					document.PieceTable.ToggleFieldCodesFromCommandOrApi(field);
			}
		}
		public bool Locked {
			get {
				CheckValid();
				return field.Locked;
			}
			set {
				CheckValid();
				if (Locked != value)
					document.PieceTable.ToggleFieldLocked(field);
			}
		}
		public void Update() {
			CheckValid();
			new DevExpress.XtraRichEdit.Model.FieldUpdater(document.PieceTable).UpdateFieldAndNestedFields(field);
		}
		public Field Parent {
			get {
				if (field.Parent != null)
					return new NativeField(document, field.Parent);
				else
					return null;
			}
		}
		#endregion
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		protected internal ModelField Field { get { return field; } }
		private DocumentRange GetRange() {
			DevExpress.XtraRichEdit.Model.PieceTable pieceTable = document.PieceTable;
			DocumentModelPosition start = DocumentModelPosition.FromRunStart(pieceTable, field.FirstRunIndex);
			DocumentModelPosition end = DocumentModelPosition.FromRunStart(pieceTable, field.LastRunIndex + 1);
			return new NativeDocumentRange(document, start, end);
		}
		private DocumentRange GetCodeRange() {
			DevExpress.XtraRichEdit.Model.PieceTable pieceTable = document.PieceTable;
			DocumentModelPosition start = DocumentModelPosition.FromRunStart(pieceTable, field.Code.Start + 1);
			DocumentModelPosition end = DocumentModelPosition.FromRunStart(pieceTable, field.Code.End);
			return new NativeDocumentRange(document, start, end);
		}
		private DocumentRange GetResultRange() {
			DevExpress.XtraRichEdit.Model.PieceTable pieceTable = document.PieceTable;
			DocumentModelPosition start = DocumentModelPosition.FromRunStart(pieceTable, field.Result.Start);
			DocumentModelPosition end = DocumentModelPosition.FromRunStart(pieceTable, field.Result.End);
			return new NativeDocumentRange(document, start, end);
		}
		void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedFieldError);
		}
	}
	public class NativeFieldCollection : List<NativeField>, FieldCollection {
		readonly NativeSubDocument document;
		internal NativeFieldCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		#region ISimpleCollection<Field> Members
		Field ISimpleCollection<Field>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region Fields Members
		public void Update() {
			document.BeginUpdate();
			try {
				DocumentModel documentModel = document.DocumentModel;
				documentModel.BeginUpdate();
				try {
					DevExpress.XtraRichEdit.Model.PieceTable pieceTable = document.PieceTable;
					var updater = new DevExpress.XtraRichEdit.Model.FieldUpdater(pieceTable);
					updater.UpdateFields(DevExpress.XtraRichEdit.Model.UpdateFieldOperationType.Normal);
				}
				finally {
					documentModel.EndUpdate();
				}
			}
			finally {
				document.EndUpdate();
			}
		}
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		public Field Add(DocumentRange codeRange) {
			ModelField field = document.PieceTable.CreateField(codeRange.Start.LogPosition, codeRange.Length);
			return this[field.Index];
		}
		[Obsolete("This method has become obsolete. Use the 'Create' method instead.")]
		public Field Add(DocumentPosition start, string code) {
			document.BeginUpdate();
			try {
				DocumentRange range = document.InsertText(start, code);
				return Add(range);
			}
			finally {
				document.EndUpdate();
			}
		}
		public Field Create(DocumentRange codeRange) {
			ModelField field = document.PieceTable.CreateField(codeRange.Start.LogPosition, codeRange.Length);
			return this[field.Index];
		}
		public Field Create(DocumentPosition start, string code) {
			document.BeginUpdate();
			try {
				DocumentRange range = document.InsertText(start, code);
				return Create(range);
			}
			finally {
				document.EndUpdate();
			}
		}
		public ReadOnlyFieldCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelRunIndex firstRunIndex = nativeRange.NormalizedStart.Position.RunIndex;
			ModelRunIndex lastRunIndex = nativeRange.NormalizedEnd.Position.RunIndex;
			NativeFieldCollection result = new NativeFieldCollection(document);
			int count = PieceTable.Fields.Count;
			for (int i = 0; i < count; i++) {
				ModelRunIndex firstIndex = PieceTable.Fields[i].FirstRunIndex;
				ModelRunIndex lastIndex = PieceTable.Fields[i].LastRunIndex;
				if ((firstIndex >= firstRunIndex) && (lastIndex <= lastRunIndex)) {
					NativeField field = new NativeField(document, PieceTable.Fields[i]);
					result.Add(field);
				}
			}
			return result;
		}
		#endregion
		#region IEnumerable<Field> Members
		IEnumerator<Field> IEnumerable<Field>.GetEnumerator() {
			return new EnumeratorAdapter<Field, NativeField>(this.GetEnumerator());
		}
		#endregion
	}
}
