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
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
using DevExpress.Utils;
namespace DevExpress.Office.Drawing {
	#region IDrawingTextRun
	public interface IDrawingTextRun {
		string Text { get; }
		ISupportsInvalidate Parent { get; set; }
		IDrawingTextRun CloneTo(IDocumentModel documentModel);
		void Visit(IDrawingTextRunVisitor visitor);
		DrawingTextCharacterProperties RunProperties { get; }
	}
	#endregion
	#region IDrawingTextRunVisitor
	public interface IDrawingTextRunVisitor {
		void Visit(DrawingTextRun item);
		void Visit(DrawingTextField item);
		void Visit(DrawingTextLineBreak item);
	}
	#endregion
	#region DrawingTextRunBase
	public abstract class DrawingTextRunBase {
		#region Fields
		readonly IDocumentModel documentModel;
		readonly InvalidateProxy innerParent;
		readonly DrawingTextCharacterProperties runProperties;
		#endregion
		protected DrawingTextRunBase(IDocumentModel documentModel) {
			this.innerParent = new InvalidateProxy();
			this.documentModel = documentModel;
			this.runProperties = new DrawingTextCharacterProperties(documentModel) { Parent = this.innerParent };
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return documentModel; } }
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		protected ISupportsInvalidate InnerParent { get { return innerParent; } }
		public DrawingTextCharacterProperties RunProperties { get { return runProperties; } }
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextRunBase other = obj as DrawingTextRunBase;
			return other != null && runProperties.Equals(other.runProperties);
		}
		public override int GetHashCode() {
			return runProperties.GetHashCode();
		}
		#endregion
		protected virtual void CopyFrom(DrawingTextRunBase value) {
			Guard.ArgumentNotNull(value, "value");
			this.runProperties.CopyFrom(value.runProperties);
		}
		protected void InvalidateParent() {
			this.innerParent.Invalidate();
		}
	}
	#endregion
	#region DrawingTextRunStringBase
	public abstract class DrawingTextRunStringBase : DrawingTextRunBase {
		#region Fields
		string text = string.Empty;
		#endregion
		protected DrawingTextRunStringBase(IDocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		#region Text
		public string Text {
			get { return text; }
			set {
				if (string.IsNullOrEmpty(value))
					value = string.Empty;
				if (text == value)
					return;
				SetText(value);
			}
		}
		void SetText(string value) {
			DrawingTextRunTextPropertyChangedHistoryItem historyItem = new DrawingTextRunTextPropertyChangedHistoryItem(DocumentModel.MainPart, this, text, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetTextCore(string value) {
			this.text = value;
			InvalidateParent();
		}
		#endregion
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextRunStringBase other = obj as DrawingTextRunStringBase;
			return other != null && string.Equals(text, other.text) && base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ text.GetHashCode();
		}
		#endregion
		protected void CopyFrom(DrawingTextRunStringBase value) {
			base.CopyFrom(value);
			Text = value.Text;
		}
	}
	#endregion
	#region DrawingTextRun
	public class DrawingTextRun : DrawingTextRunStringBase, IDrawingTextRun, ISupportsCopyFrom<DrawingTextRun> {
		public DrawingTextRun(IDocumentModel documentModel)
			: base(documentModel) {
		}
		public DrawingTextRun(IDocumentModel documentModel, string text)
			: base(documentModel) {
			if (!string.IsNullOrEmpty(text))
				SetTextCore(text);
		}
		#region IDrawingTextRun Members
		public IDrawingTextRun CloneTo(IDocumentModel documentModel) {
			DrawingTextRun result = new DrawingTextRun(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingTextRunVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextRun> Members
		public void CopyFrom(DrawingTextRun value) {
			base.CopyFrom(value);
		}
		#endregion
	}
	#endregion
	#region DrawingTextField
	public class DrawingTextField : DrawingTextRunStringBase, IDrawingTextRun, ISupportsCopyFrom<DrawingTextField> {
		#region Fields
		Guid fieldId;
		string fieldType;
		DrawingTextParagraphProperties paragraphProperties;
		#endregion
		public DrawingTextField(IDocumentModel documentModel)
			: base(documentModel) {
			this.fieldId = Guid.Empty;
			this.fieldType = string.Empty;
			this.paragraphProperties = new DrawingTextParagraphProperties(documentModel) { Parent = InnerParent };
		}
		#region Properties
		#region FieldId
		public Guid FieldId {
			get { return fieldId; }
			set {
				if (fieldId.Equals(value))
					return;
				SetFieldId(value);
			}
		}
		void SetFieldId(Guid value) {
			DrawingTextFieldIdPropertyChangedHistoryItem historyItem = new DrawingTextFieldIdPropertyChangedHistoryItem(DocumentModel.MainPart, this, fieldId, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetFieldIdCore(Guid value) {
			this.fieldId = value;
		}
		#endregion
		#region FieldType
		public string FieldType {
			get { return fieldType; }
			set {
				if (value == null)
					value = string.Empty;
				if (fieldType.Equals(value))
					return;
				SetFieldType(value);
			}
		}
		void SetFieldType(string value) {
			DrawingTextFieldTypePropertyChangedHistoryItem historyItem = new DrawingTextFieldTypePropertyChangedHistoryItem(DocumentModel.MainPart, this, fieldType, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetFieldTypeCore(string value) {
			this.fieldType = value;
			InvalidateParent();
		}
		#endregion
		public DrawingTextParagraphProperties ParagraphProperties { get { return paragraphProperties; } }
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextField other = obj as DrawingTextField;
			if (other == null)
				return false;
			return base.Equals(other) &&
				this.fieldId.Equals(other.fieldId) &&
				this.fieldType.Equals(other.fieldType) &&
				this.paragraphProperties.Equals(other.paragraphProperties);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ fieldId.GetHashCode() ^ fieldType.GetHashCode() ^ paragraphProperties.GetHashCode();
		}
		#endregion
		#region IDrawingTextRun Members
		public IDrawingTextRun CloneTo(IDocumentModel documentModel) {
			DrawingTextField result = new DrawingTextField(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingTextRunVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextField> Members
		public void CopyFrom(DrawingTextField value) {
			base.CopyFrom(value);
			FieldId = value.FieldId;
			FieldType = value.FieldType;
			this.paragraphProperties.CopyFrom(value.paragraphProperties);
		}
		#endregion
	}
	#endregion
	#region DrawingTextLineBreak
	public class DrawingTextLineBreak : DrawingTextRunBase, IDrawingTextRun, ISupportsCopyFrom<DrawingTextLineBreak> {
		public DrawingTextLineBreak(IDocumentModel documentModel)
			: base(documentModel) {
		}
		#region IDrawingTextRun Members
		public string Text { get { return Environment.NewLine; }}
		public IDrawingTextRun CloneTo(IDocumentModel documentModel) {
			DrawingTextLineBreak result = new DrawingTextLineBreak(documentModel);
			result.CopyFrom(this);
			return result;
		}
		public void Visit(IDrawingTextRunVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextLineBreak> Members
		public void CopyFrom(DrawingTextLineBreak value) {
			base.CopyFrom(value);
		}
		#endregion
	}
	#endregion
	#region DrawingTextRunCollection
	public class DrawingTextRunCollection : UndoableClonableCollection<IDrawingTextRun> {
		readonly InvalidateProxy innerParent = new InvalidateProxy();
		public DrawingTextRunCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		protected internal ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public override UndoableClonableCollection<IDrawingTextRun> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new DrawingTextRunCollection(documentModelPart.DocumentModel);
		}
		public override IDrawingTextRun GetCloneItem(IDrawingTextRun item, IDocumentModelPart documentModelPart) {
			return item.CloneTo(documentModelPart.DocumentModel);
		}
		public override int AddCore(IDrawingTextRun item) {
			int result = base.AddCore(item);
			item.Parent = this.innerParent;
			this.innerParent.Invalidate();
			return result;
		}
		protected internal override void InsertCore(int index, IDrawingTextRun item) {
			base.InsertCore(index, item);
			item.Parent = this.innerParent;
			this.innerParent.Invalidate();
		}
		public override void RemoveAtCore(int index) {
			IDrawingTextRun item = this[index];
			base.RemoveAtCore(index);
			item.Parent = null;
			this.innerParent.Invalidate();
		}
		public override void ClearCore() {
			if (Count == 0)
				return;
			foreach (IDrawingTextRun item in this)
				item.Parent = null;
			base.ClearCore();
			this.innerParent.Invalidate();
		}
		public override void AddRangeCore(IEnumerable<IDrawingTextRun> collection) {
			foreach (IDrawingTextRun item in collection)
				item.Parent = this.innerParent;
			base.AddRangeCore(collection);
			this.innerParent.Invalidate();
		}
	}
	#endregion
}
