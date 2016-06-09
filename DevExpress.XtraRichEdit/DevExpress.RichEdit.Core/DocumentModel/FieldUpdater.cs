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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Fields;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	public class FieldUpdater {
		readonly PieceTable pieceTable;
		public FieldUpdater(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		protected DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		protected PieceTable PieceTable { get { return pieceTable; } }
		protected FieldCollection Fields { get { return pieceTable.Fields; } }
		#region Update fields
		public void UpdateFields(UpdateFieldOperationType updateType) {
			UpdateFields(Fields, null, updateType);
			UpdateFieldsInSubDocuments(RunIndex.Zero, new RunIndex(pieceTable.Runs.Count - 1), updateType);
		}
		public void UpdateFields(IList<Field> fields, Field parent, UpdateFieldOperationType updateType) {
			UpdateFields(GetFieldsToUpdate(fields, parent), updateType);
		}
		public void UpdateFields(IList<Field> fieldsToUpdate, UpdateFieldOperationType updateType) {
			DocumentModel.BeginUpdate();
			try {
				PrepareFieldsCore(fieldsToUpdate, updateType);
				UpdateFieldsCore(fieldsToUpdate, updateType);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void UpdateFieldsAsync(IList<Field> fieldsToUpdate, UpdateFieldOperationType updateType) {
			DocumentModel.BeginUpdate();
			try {
				PrepareFieldsCore(fieldsToUpdate, updateType);
				UpdateTaskDispatcher dispatcher = new UpdateTaskDispatcher(PieceTable);
				dispatcher.UpdateFields(fieldsToUpdate, DocumentModel.GetMailMergeDataMode(), updateType);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void UpdateFieldsCore(IList<Field> fieldsToUpdate, UpdateFieldOperationType updateType) {
			int count = fieldsToUpdate.Count;
			for(int i = 0; i < count; i++) {
				UpdateFieldRecursive(fieldsToUpdate[i], updateType);
			}
		}
		public List<Field> GetFieldsToUpdate(IList<Field> fields, Field parent) {
			List<Field> result = new List<Field>(fields.Count);
			int count = fields.Count;
			for(int i = 0; i < count; i++)
				if(fields[i].Parent == parent)
					result.Add(fields[i]);
			return result;
		}
		public void UpdateFieldAndNestedFields(Field field) {
			DocumentModel.BeginUpdate();
			try {
				PrepareFieldAndNestedFields(field, UpdateFieldOperationType.Normal);
				UpdateFieldRecursive(field, UpdateFieldOperationType.Normal);				
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
#endregion
#region Prepare
		public void PrepareFieldsCore(IList<Field> fieldsToUpdate, UpdateFieldOperationType updateType) {
			int count = fieldsToUpdate.Count;
			for(int i = 0; i < count; i++) {
				PrepareFieldAndNestedFields(fieldsToUpdate[i], updateType);
			}
		}
		public virtual void PrepareFieldUpdate(Field field, UpdateFieldOperationType updateType) {
			IFieldCalculatorService fieldCalculatorService = DocumentModel.GetService<IFieldCalculatorService>();
			if(fieldCalculatorService != null)
				fieldCalculatorService.PrepareField(PieceTable, field, updateType);
		}
		void PrepareFieldAndNestedFields(Field field, UpdateFieldOperationType updateType) {
			PrepareNestedFieldsUpdate(field, updateType);
			PrepareFieldUpdate(field, updateType);
		}
		void PrepareNestedFieldsUpdate(Field field, UpdateFieldOperationType updateType) {
			FieldCollection fields = Fields;
			FieldRunIndexComparable comparator = new FieldRunIndexComparable(field.FirstRunIndex);
			int index = Algorithms.BinarySearch(fields, comparator);
			if(index < 0) {
				index = ~index;
				if(index >= fields.Count) {
					Exceptions.ThrowInternalException();
				}
			}
			Field currentField = fields[index];
			while(currentField != field) {
				PrepareFieldUpdate(currentField, updateType);
				index++;
				currentField = fields[index];
			}
		}
#endregion
#region Update field
		public UpdateFieldOperationResult UpdateField(Field field, MailMergeDataMode mailMergeDataMode) {
			return UpdateField(field, mailMergeDataMode, UpdateFieldOperationType.Normal);
		}
		UpdateFieldOperationResult UpdateField(Field field, MailMergeDataMode mailMergeDataMode, UpdateFieldOperationType updateType) {
			if(field.DisableUpdate)
				return UpdateFieldOperationResult.DisabledUpdate;
			DocumentModel.BeginUpdate();
			try {
				UpdateFieldOperation operation = new UpdateFieldOperation(PieceTable, mailMergeDataMode);
				return operation.Execute(field, updateType);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void UpdateFieldRecursive(Field field, UpdateFieldOperationType updateType) {
			Debug.Assert(field != null);
			if(field.DisableUpdate)
				return;
			MailMergeDataMode mode = GetMailMergeDataMode();
			UpdateInnerCodeFields(field, updateType);
			UpdateFieldOperationResult result = UpdateField(field, mode, updateType);
			if(result.UpdateFieldResult != UpdateFieldResult.FieldUpdatedAndCodeDeleted) {
				if (!result.SuppressUpdateInnerCodeFields) {
					UpdateInnerResultFields(field, updateType);
					UpdateFieldsInSubDocuments(field.Result.Start, field.Result.End - 1, updateType);
				}
			}
		}
		protected virtual MailMergeDataMode GetMailMergeDataMode() {
			return DocumentModel.GetMailMergeDataMode();
		}
		void UpdateInnerCodeFields(Field parentField, UpdateFieldOperationType updateType) {
			Debug.Assert(parentField != null);
			List<Field> innerCodeFields = GetInnerCodeFields(Fields, parentField);
			int count = innerCodeFields.Count;
			for(int i = 0; i < count; i++)
				UpdateFieldRecursive(innerCodeFields[i], updateType);
			UpdateFieldsInSubDocuments(parentField.Code.Start + 1, parentField.Code.End - 1, updateType);
		}
		void UpdateFieldsInSubDocuments(RunIndex start, RunIndex end, UpdateFieldOperationType updateType) {
			if (!DocumentModel.FieldOptions.UpdateFieldsInTextBoxes)
				return;
			int nestedLevel = 0;
			TextRunCollection runs = PieceTable.Runs;
			for (RunIndex runIndex = start; runIndex <= end; runIndex++) {
				TextRunBase run = PieceTable.Runs[runIndex];
				FieldCodeStartRun startRun = run as FieldCodeStartRun;
				if (startRun != null) {
					nestedLevel++;
					continue;
				}
				FieldCodeEndRun endRun = run as FieldCodeEndRun;
				if (endRun != null) {
					nestedLevel--;
					continue;
				}
				if (nestedLevel != 0)
					continue;
				FloatingObjectAnchorRun anchorRun = run as FloatingObjectAnchorRun;
				if (anchorRun == null)
					continue;
				TextBoxFloatingObjectContent textBox = anchorRun.Content as TextBoxFloatingObjectContent;
				if (textBox == null)
					continue;
				FieldUpdater updater = new FieldUpdater(textBox.TextBox.PieceTable);
				updater.UpdateFields(updateType);
			}
		}
		void UpdateInnerResultFields(Field parentField, UpdateFieldOperationType updateType) {
			List<Field> innerResultFields = GetInnerResultFields(Fields, parentField);
			int count = innerResultFields.Count;
			for(int i = 0; i < count; i++) {
				FieldCalculatorService service = new FieldCalculatorService();
				CalculatedFieldBase fieldBase = service.ParseField(PieceTable, innerResultFields[i]);
				if(fieldBase is PageRefField)
					continue;
				UpdateFieldRecursive(innerResultFields[i], updateType);
			}
		}
		public virtual List<Field> GetInnerCodeFields(IList<Field> fields, Field parentField) {
			Debug.Assert(parentField != null);
			int fieldsCount = fields.Count;
			List<Field> result = new List<Field>();
			for(int i = 0; i < fieldsCount; i++)
				if(fields[i].Parent == parentField && fields[i].Result.End < parentField.Code.End)
					result.Add(fields[i]);
			return result;
		}
		public List<Field> GetInnerResultFields(IList<Field> fields, Field parentField) {
			Debug.Assert(parentField != null);
			int fieldsCount = fields.Count;
			List<Field> result = new List<Field>();
			for(int i = 0; i < fieldsCount; i++)
				if(fields[i].Parent == parentField && fields[i].Code.Start >= parentField.Result.Start)
					result.Add(fields[i]);
			return result;
		}
#endregion
	}
	public class MailMergeFieldUpdater : FieldUpdater {
		public MailMergeFieldUpdater(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override MailMergeDataMode GetMailMergeDataMode() {
			return MailMergeDataMode.FinalMerging;
		}
	}
	public class UpdateTaskDispatcher {
		readonly PieceTable pieceTable;
		readonly UpdateTaskPool taskPool;
		UpdateTaskDispatcherTree tree;
		MailMergeDataMode mailMergeDataMode;
		UpdateFieldOperationType updateType;
		public UpdateTaskDispatcher(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.taskPool = new UpdateTaskPool(pieceTable);
			this.tree = new UpdateTaskDispatcherTree();
			this.taskPool.FieldCalculated += OnFieldCalculated;
		}
		PieceTable PieceTable { get { return pieceTable; } }
		DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public void UpdateFields(IList<Field> fieldsToUpdate, MailMergeDataMode mailMergeDataMode, UpdateFieldOperationType updateType) {
			Guard.ArgumentNotNull(fieldsToUpdate, "fieldsToUpdate");
			this.mailMergeDataMode = mailMergeDataMode;
			this.updateType = updateType;
			if(fieldsToUpdate.Count == 0)
				return;
			foreach(Field field in fieldsToUpdate)
				ExpandCode(tree.AddChild(field));
			tree.ForEach(UpdateLeaves);
		}
		void UpdateLeaves(UpdateTaskDispatcherTreeNode node) {
			if(node.IsRoot || !node.IsLeaf)
				return;
			FieldCalculatorService calculator = pieceTable.CreateFieldCalculatorService();
			CalculatedFieldBase parsedField = calculator.ParseField(pieceTable, node.Field);
			this.taskPool.BeginTask(new UpdateTask(parsedField, node, this.mailMergeDataMode, this.updateType));				
		}
		void ExpandCode(UpdateTaskDispatcherTreeNode root) {
			List<Field> children = PieceTable.FieldUpdater.GetInnerCodeFields(pieceTable.Fields, root.Field);
			foreach(Field child in children) {
				ExpandCode(root.AddChild(child));
			}
		}
		void ExpandResult(UpdateTaskDispatcherTreeNode root) {
			List<Field> children = PieceTable.FieldUpdater.GetInnerResultFields(pieceTable.Fields, root.Field);
			foreach(Field child in children) {
				ExpandCode(root.AddChild(child));
			}
		}
		void OnFieldCalculated(object sender, FieldCalculatedEventArgs e) {		  
			UpdateFieldOperation operation = new UpdateFieldOperation(PieceTable, this.mailMergeDataMode);
			DocumentModel.BeginUpdate();
			UpdateTaskDispatcherTreeNode parentNode = null;
			UpdateTaskDispatcherTreeNode node = e.UpdateTask.Node;
			try {
				UpdateFieldOperationResult insertResult = operation.InsertResult(node.Field, e.Result, this.updateType);				
				parentNode = node.Parent;
				if(!insertResult.SuppressUpdateInnerCodeFields) {
					switch(insertResult.UpdateFieldResult) {
						case UpdateFieldResult.FieldUpdated:
							ExpandResult(node);
							node.MoveChildrenUp();
							break;
						case UpdateFieldResult.FieldUpdatedAndCodeDeleted:
							throw new NotImplementedException();
					}
				}				
			}
			finally {
				node.Remove();
				DocumentModel.EndUpdate();
			}
			if(parentNode != null)
				parentNode.ForEach(UpdateLeaves);
		}
	}
	public class UpdateTaskDispatcherTreeNode {
		Field field;
		UpdateTaskDispatcherTreeNode parent;
		List<UpdateTaskDispatcherTreeNode> children;
		public UpdateTaskDispatcherTreeNode(Field field) {
			this.field = field;
			this.parent = null;
			this.children = new List<UpdateTaskDispatcherTreeNode>(0);
		}
		public Field Field { get { return field; } }
		public UpdateTaskDispatcherTreeNode Parent { get { return parent; } }
		public bool IsLeaf { get { return children.Count == 0; } }
		public virtual bool IsRoot { get { return false; } } 
		public UpdateTaskDispatcherTreeNode AddChild(Field field) {
			UpdateTaskDispatcherTreeNode child = new UpdateTaskDispatcherTreeNode(field);
			child.parent = this;
			this.children.Add(child);
			return child;
		}
		public void Remove() {
			if(IsRoot || !IsLeaf)
				Exceptions.ThrowInvalidOperationException(String.Empty);
			this.parent.children.Remove(this);
		}
		public void ForEach(Action<UpdateTaskDispatcherTreeNode> action) {
			action.Invoke(this);
			foreach(UpdateTaskDispatcherTreeNode child in children) {
				child.ForEach(action);
			}
		}
		public void MoveChildrenUp() {
			foreach(UpdateTaskDispatcherTreeNode child in this.children) {
				child.parent = this.parent;
				this.parent.children.Add(child);
			}
			this.children.Clear();
		}
	}
	class UpdateTaskDispatcherTree : UpdateTaskDispatcherTreeNode {
		public UpdateTaskDispatcherTree() : base(null) { }
		public override bool IsRoot { get { return true; } }
	}
	class UpdateTaskPool {
		readonly PieceTable pieceTable;
		readonly IThreadSyncService threadSyncService;
		public UpdateTaskPool(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.threadSyncService = this.pieceTable.DocumentModel.GetService<IThreadSyncService>();
			Debug.Assert(this.threadSyncService != null);
		}
		public void BeginTask(UpdateTask task) {
			Debug.Assert(task.FieldInfo.AllowAsyncUpdate);
			ThreadPool.QueueUserWorkItem(ProcessTask, task);
		}
		public event FieldCalculatedEventHandler FieldCalculated;
		void RaiseFieldCalculated(UpdateTask task, CalculateFieldResult result) {
			if(FieldCalculated != null) {
				Action action = () => { FieldCalculated(this, new FieldCalculatedEventArgs(task, result)); };
				threadSyncService.EnqueueInvokeInUIThread(action);
			}
		}
		void ProcessTask(object state) {
			UpdateTask task = (UpdateTask)state;
			CalculateFieldResult result = this.pieceTable.CalculateFieldResult(task.ModelField, task.MailMergeDataMode, task.UpdateType);
			RaiseFieldCalculated(task, result);
		}
	}
	public class FieldCalculatedEventArgs : EventArgs {
		readonly UpdateTask updateTask;
		readonly CalculateFieldResult result;
		public FieldCalculatedEventArgs(UpdateTask updateTask, CalculateFieldResult result) {
			this.updateTask = updateTask;
			this.result = result;
		}
		public UpdateTask UpdateTask { get { return updateTask; } }
		public CalculateFieldResult Result { get { return result; } }
	}
	public delegate void FieldCalculatedEventHandler(object sender, FieldCalculatedEventArgs e);
	public class UpdateTask {
		readonly CalculatedFieldBase fieldInfo;
		readonly UpdateTaskDispatcherTreeNode node;
		readonly MailMergeDataMode mailMergeDataMode;
		readonly UpdateFieldOperationType updateType;
		public UpdateTask(CalculatedFieldBase fieldInfo, UpdateTaskDispatcherTreeNode node, MailMergeDataMode mailMergeDataMode, UpdateFieldOperationType updateType) {
			Guard.ArgumentNotNull(fieldInfo, "fieldInfo");
			Guard.ArgumentNotNull(node, "node");
			this.fieldInfo = fieldInfo;
			this.node = node;
			this.mailMergeDataMode = mailMergeDataMode;
			this.updateType = updateType;
		}
		public CalculatedFieldBase FieldInfo { get { return fieldInfo; } }
		public Field ModelField { get { return node.Field; } }
		public UpdateTaskDispatcherTreeNode Node { get { return node; } }
		public MailMergeDataMode MailMergeDataMode { get { return mailMergeDataMode; } }
		public UpdateFieldOperationType UpdateType { get { return updateType; } }
	}
}
