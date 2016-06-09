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
using System.Linq;
using DevExpress.Office;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Text;
using System.Diagnostics;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region Comment
	public interface Comment {
		CommentRunCollection Runs { get; }
		string Reference { get; }
		string Author { get; set; }
		string Text { get; set; }
		bool Visible { get; set; }
	}
	#endregion
	#region CommentCollection
	public interface CommentCollection : ISimpleCollection<Comment> {
		void RemoveAt(int index);
		Comment Add(Range range, string author);
		Comment Add(Range range, string author, string text);
		void Remove(Range range);
		int IndexOf(Comment comment);
		void Remove(Comment comment);
		void Clear();
		bool Contains(Comment comment);
		List<Comment> GetComments(Range range);
	}
	#endregion
	#region CommentRun
	public interface CommentRun {
		string Text { get; set; }
		SpreadsheetFont Font { get; }
	}
	#endregion
	#region CommentRunCollection
	public interface CommentRunCollection : ISimpleCollection<CommentRun> {
		void RemoveAt(int index);
		void Remove(CommentRun run);
		int IndexOf(CommentRun run);
		CommentRun Add(string text);
		void Clear();
		void Insert(int index, string text);
		bool Contains(CommentRun run);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	sealed partial class NativeComment : NativeObjectBase, Comment {
		readonly Model.Comment modelComment;
		readonly NativeCommentRunCollection runs;
		public NativeComment(Model.Comment modelComment) {
			Guard.ArgumentNotNull(modelComment, "modelComment");
			this.modelComment = modelComment;
			this.runs = new NativeCommentRunCollection(modelComment.Runs);
		}
		Model.Comment ModelComment { [DebuggerStepThrough] get { CheckValid(); return modelComment; } }
		Model.DocumentModel DocumentModel { [DebuggerStepThrough]  get { return ModelComment.Workbook; } }
		public CommentRunCollection Runs {[DebuggerStepThrough]  get { return runs; } }
		public string Text {
			get { return ModelComment.GetPlainText(); }
			set { ModelComment.SetPlainText(value); }
		}
		public string Author {
			get {
				return DocumentModel.CommentAuthors[ModelComment.AuthorId];
			}
			set {
				ModelComment.AuthorId = DocumentModel.CommentAuthors.AddIfNotPresent(value);
			}
		}
		public string Reference {
			get {
				Model.CellPosition modelPosition = ModelComment.Reference.AsAbsolute();
				return Model.CellReferenceParser.ToString(modelPosition);
			}
		}
		public bool Visible {
			get { return ModelComment.Visible; }
			set { ModelComment.Visible = value; }
		}
	}
	partial class NativeCommentCollection : NativeCollectionForUndoableCollectionBase<Comment, NativeComment, Model.Comment>, CommentCollection {
		public NativeCommentCollection(NativeWorksheet worksheet)
			: base(worksheet) {
		}
		protected override UndoableCollection<Model.Comment> GetModelCollection() {
			return ModelWorksheet.Comments;
		}
		protected override void ClearModelObjects() {
			ModelWorksheet.ClearComments();
		}
		protected override NativeComment CreateNativeObject(Model.Comment comment) {
			return new NativeComment(comment);
		}
		public override IEnumerable<Model.Comment> GetModelItemEnumerable() {
			return ModelWorksheet.Comments.InnerList;
		}
		public override int ModelCollectionCount {
			get { return ModelWorksheet.Comments.Count; }
		}
		protected override void RemoveModelObjectAt(int index) {
			ModelWorksheet.RemoveCommentAt(index);
		}
		protected override void InvalidateItem(NativeComment item) {
			item.IsValid = false;
		}
		public Comment Add(Range range, string author) {
			Model.CellPosition getTopLeft = Worksheet.GetTopLeft(range);
			ModelWorksheet.CreateComment(getTopLeft, author);
			return InnerList[Count - 1];
		}
		public Comment Add(Range range, string author, string text) {
			Comment comment = Add(range, author);
			comment.Runs.Add(text);
			return comment;
		}
		public void Remove(Range range) {
			ModelWorksheet.ClearComments(Worksheet.GetModelRange(range));
		}
		public List<Comment> GetComments(Range range) {
			Model.CellRangeBase modelRange = this.Worksheet.GetModelRange(range);
			List<Model.Comment> modelComments = new List<Model.Comment>();
			modelRange.ForEach(
				r => modelComments.AddRange(ModelWorksheet.GetComments(r))
			);
			List<Comment> result = modelComments.ConvertAll(modelComment => (Comment) CreateNativeObject(modelComment));
			return result;
		}
	}
	partial class NativeCommentRun : NativeObjectBase, CommentRun {
		readonly Model.CommentRun modelRun;
		public NativeCommentRun(Model.CommentRun modelRun) {
			Guard.ArgumentNotNull(modelRun, "modelRun");
			this.modelRun = modelRun;
		}
		protected internal Model.CommentRun ModelRun { get { CheckValid(); return modelRun; } }
		public string Text { get { return ModelRun.Text; } set { ModelRun.Text = value; } }
		public Spreadsheet.SpreadsheetFont Font {
			get {
				return new NativeFont(ModelRun, (Model.IRunFontInfo)ModelRun);
			}
		}
	}
	sealed partial class NativeCommentRunCollection :  CommentRunCollection {
		readonly Model.CommentRunCollection modelCollection;
		public NativeCommentRunCollection(Model.CommentRunCollection modelCollection) {
			this.modelCollection = modelCollection;
		}
		CommentRun ISimpleCollection<CommentRun>.this[int index] {
			get {
				return CreateNativeObject(modelCollection[index]);
			}
		}
		CommentRun CommentRunCollection.Add(string text) {
			Model.CommentRun newModelObject = new Model.CommentRun(modelCollection.Worksheet, text);
			modelCollection.Add(newModelObject);
			return CreateNativeObject(newModelObject);
		}
		void CommentRunCollection.Clear() {
			modelCollection.Clear();
		}
		bool CommentRunCollection.Contains(CommentRun run) {
			Model.CommentRun modelObject = GetModelObject(run, false);
			if (modelObject == null)
				return false;
			return modelCollection.Contains(modelObject);
		}
		int ICollection.Count {[DebuggerStepThrough] get { return modelCollection.Count; } }
		bool ICollection.IsSynchronized {[DebuggerStepThrough] get { return (modelCollection.InnerList as ICollection).IsSynchronized; } }
		object ICollection.SyncRoot {[DebuggerStepThrough] get { return (modelCollection.InnerList as ICollection).SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) {
			List<CommentRun> gatheredCollection = modelCollection.InnerList.ConvertAll(
				(model) => (CommentRun)CreateNativeObject(model));
			(gatheredCollection as ICollection).CopyTo(array, index);
		}
		public IEnumerator<CommentRun> GetEnumerator() {
			Func<Model.CommentRun, CommentRun> selector = (modelRun) => (CommentRun)new NativeCommentRun(modelRun);
			IEnumerable<CommentRun> enumerable = modelCollection.InnerList.Select<Model.CommentRun, CommentRun>(selector);
			return enumerable.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			Func<Model.CommentRun, CommentRun> selector = (modelRun) => (CommentRun)new NativeCommentRun(modelRun);
			IEnumerable<CommentRun> enumerable = modelCollection.InnerList.Select<Model.CommentRun, CommentRun>(selector);
			return enumerable.GetEnumerator();
		}
		int CommentRunCollection.IndexOf(CommentRun run) {
			Model.CommentRun modelObject = GetModelObject(run, false);
			if (modelObject == null)
				return -1;
			return modelCollection.IndexOf(modelObject);
		}
		void CommentRunCollection.Insert(int index, string text) {
			Model.CommentRun newObject = new Model.CommentRun(modelCollection.Worksheet, text);
			modelCollection.Insert(index, newObject);
		}
		CommentRun CreateNativeObject(Model.CommentRun modelObject) {
			return new NativeCommentRun(modelObject);
		}
		Model.CommentRun GetModelObject(CommentRun argument, bool throwException) {
			NativeCommentRun nativeArgument = argument as NativeCommentRun;
			if (nativeArgument == null)
				if (throwException)
					throw new ArgumentException("CommentRun");
				else
					return null;
			Model.CommentRun modelObject = nativeArgument.ModelRun;
			return modelObject;
		}
		void CommentRunCollection.Remove(CommentRun run) {
			Model.CommentRun modelObject = GetModelObject(run, true);
			modelCollection.Remove(modelObject);
		}
		void CommentRunCollection.RemoveAt(int index) {
			modelCollection.RemoveAt(index);
		}
	}
	partial class NativeCommentRunCollectionOLD : CommentRunCollection {
		readonly Model.Comment modelComment;
		readonly List<NativeCommentRun> innerList;
		public NativeCommentRunCollectionOLD(Model.Comment modelComment) {
			Guard.ArgumentNotNull(modelComment, "modelComment");
			this.modelComment = modelComment;
			this.innerList = new List<NativeCommentRun>();
		}
		protected Model.Worksheet ModelWorksheet { get { return modelComment.Worksheet; } }
		protected internal List<NativeCommentRun> InnerList {
			get {
				if (innerList.Count == 0 && modelComment.Runs.Count != 0)
					PopulateCommentRuns();
				return innerList;
			}
		}
		void PopulateCommentRuns() {
			innerList.Clear();
			ModelComment.Runs.ForEach(RegisterCommentRuns);
		}
		void RegisterCommentRuns(Model.CommentRun run) {
			innerList.Add(new NativeCommentRun(run));
		}
		protected Model.Comment ModelComment { get { return modelComment; } }
		public int Count { get { return ModelComment.Runs.Count; } }
		public CommentRun this[int index] { get { return InnerList[index]; } }
		protected internal void OnInsert(int index, Model.CommentRun run) {
			InnerList.Insert(index, new NativeCommentRun(run));
		}
		public void RemoveAt(int index) {
			ModelComment.Runs.RemoveAt(index);
		}
		protected internal void OnRemoveAt(int index) {
			InnerList[index].IsValid = false;
			InnerList.RemoveAt(index);
		}
		public void Remove(CommentRun run) {
			int index = InnerList.IndexOf((NativeCommentRun)run);
			ModelComment.Runs.RemoveAt(index);
		}
		public int IndexOf(CommentRun run) {
			return InnerList.IndexOf((NativeCommentRun)run);
		}
		public CommentRun Add(string text) {
			Model.CommentRun modelRun = new Model.CommentRun(ModelWorksheet);
			modelRun.Text = text;
			ModelComment.Runs.Add(modelRun);
			return InnerList[Count - 1];
		}
		protected internal void OnAdd(Model.CommentRun run) {
			InnerList.Add(new NativeCommentRun(run));
		}
		public void Clear() {
			ModelComment.Runs.Clear();
		}
		protected internal void OnClear() {
			foreach (NativeCommentRun run in InnerList)
				run.IsValid = false;
			InnerList.Clear();
		}
		public void Insert(int index, string text) {
			Model.CommentRun modelRun = new Model.CommentRun(ModelWorksheet);
			modelRun.Text = text;
			ModelComment.Runs.Insert(index, modelRun);
		}
		public bool Contains(CommentRun run) {
			return InnerList.Contains((NativeCommentRun)run);
		}
		public IEnumerator<CommentRun> GetEnumerator() {
			return new EnumeratorAdapter<CommentRun, NativeCommentRun>(innerList.GetEnumerator());
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		object ICollection.SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
	}
}
