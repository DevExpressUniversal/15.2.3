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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region DrawingTextParagraph
	public class DrawingTextParagraph : ICloneable<DrawingTextParagraph>, ISupportsCopyFrom<DrawingTextParagraph> {
		#region Fields
		const int ApplyParagraphPropertiesIndex = 0;
		const int ApplyEndRunPropertiesIndex = 1;
		readonly InvalidateProxy innerParent;
		readonly DrawingTextRunCollection runs;
		readonly DrawingTextParagraphProperties paragraphProperties;
		readonly DrawingTextCharacterProperties endRunProperties;
		bool[] options;
		#endregion
		public DrawingTextParagraph(IDocumentModel documentModel) {
			this.innerParent = new InvalidateProxy();
			this.runs = new DrawingTextRunCollection(documentModel) { Parent = this.innerParent };
			this.paragraphProperties = new DrawingTextParagraphProperties(documentModel) { Parent = this.innerParent };
			this.endRunProperties = new DrawingTextCharacterProperties(documentModel) { Parent = this.innerParent };
			this.options = new bool[2];
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return runs.DocumentModel; } }
		protected internal ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public DrawingTextRunCollection Runs { get { return runs; } }
		public DrawingTextParagraphProperties ParagraphProperties { get { return paragraphProperties; } }
		public DrawingTextCharacterProperties EndRunProperties { get { return endRunProperties; } }
		public bool ApplyParagraphProperties { get { return options[ApplyParagraphPropertiesIndex]; } set { SetOptions(ApplyParagraphPropertiesIndex, value); } }
		public bool ApplyEndRunProperties { get { return options[ApplyEndRunPropertiesIndex]; } set { SetOptions(ApplyEndRunPropertiesIndex, value); } }
		#endregion
		void SetOptions(int index, bool value) {
			if (options[index] == value)
				return;
			HistoryItem item = new DrawingTextParagraphOptionsChangedHistoryItem(this, index, options[index], value);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		internal void SetOptionsCore(int index, bool value) {
			options[index] = value;
			this.innerParent.Invalidate();
		}
		public string GetPlainText() {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.runs.Count; i++)
				sb.Append(this.runs[i].Text);
			return sb.ToString();
		}
		public void SetPlainText(string value) {
			this.runs.Clear();
			DrawingTextRun item = new DrawingTextRun(DocumentModel, value);
			this.runs.Add(item);
		}
		public DrawingTextParagraph CloneTo(IDocumentModel documentModel) {
			DrawingTextParagraph result = new DrawingTextParagraph(documentModel);
			result.CopyFrom(this);
			return result;
		}
		#region ICloneable<DrawingTextParagraph> Members
		public DrawingTextParagraph Clone() {
			DrawingTextParagraph result = new DrawingTextParagraph(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<TextParagraph> Members
		public void CopyFrom(DrawingTextParagraph value) {
			Guard.ArgumentNotNull(value, "value");
			this.runs.CopyFrom(value.runs);
			this.paragraphProperties.CopyFrom(value.paragraphProperties);
			this.endRunProperties.CopyFrom(value.endRunProperties);
			this.options[ApplyParagraphPropertiesIndex] = value.ApplyParagraphProperties;
			this.options[ApplyEndRunPropertiesIndex] = value.ApplyEndRunProperties;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextParagraph other = obj as DrawingTextParagraph;
			if (other == null)
				return false;
			if (!runs.Equals(other.runs))
				return false;
			if (!paragraphProperties.Equals(other.paragraphProperties))
				return false;
			if (!endRunProperties.Equals(other.endRunProperties))
				return false;
			return
				ApplyParagraphProperties == other.ApplyParagraphProperties &&
				ApplyEndRunProperties == other.ApplyEndRunProperties; 
		}
		public override int GetHashCode() {
			return 
				runs.GetHashCode() ^ paragraphProperties.GetHashCode() ^ endRunProperties.GetHashCode() ^ 
				ApplyParagraphProperties.GetHashCode() ^ ApplyEndRunProperties.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region DrawingTextParagraphCollection
	public class DrawingTextParagraphCollection : UndoableClonableCollection<DrawingTextParagraph> {
		readonly InvalidateProxy innerParent;
		public DrawingTextParagraphCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
			this.innerParent = new InvalidateProxy();
		}
		public ISupportsInvalidate Parent { get { return innerParent.Target; } set { innerParent.Target = value; } }
		public override UndoableClonableCollection<DrawingTextParagraph> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new DrawingTextParagraphCollection(documentModelPart.DocumentModel);
		}
		public override DrawingTextParagraph GetCloneItem(DrawingTextParagraph item, IDocumentModelPart documentModelPart) {
			return item.CloneTo(documentModelPart.DocumentModel);
		}
		public override int AddCore(DrawingTextParagraph item) {
			int result = base.AddCore(item);
			item.Parent = this.innerParent;
			this.innerParent.Invalidate();
			return result;
		}
		protected internal override void InsertCore(int index, DrawingTextParagraph item) {
			base.InsertCore(index, item);
			item.Parent = this.innerParent;
			this.innerParent.Invalidate();
		}
		public override void RemoveAtCore(int index) {
			DrawingTextParagraph item = this[index];
			item.Parent = null;
			base.RemoveAtCore(index);
			this.innerParent.Invalidate();
		}
		public override void ClearCore() {
			if (Count == 0)
				return;
			foreach (DrawingTextParagraph item in this)
				item.Parent = null;
			base.ClearCore();
			this.innerParent.Invalidate();
		}
		public override void AddRangeCore(IEnumerable<DrawingTextParagraph> collection) {
			foreach (DrawingTextParagraph item in this)
				item.Parent = this.innerParent;
			base.AddRangeCore(collection);
			this.innerParent.Invalidate();
		}
	}
	#endregion
}
