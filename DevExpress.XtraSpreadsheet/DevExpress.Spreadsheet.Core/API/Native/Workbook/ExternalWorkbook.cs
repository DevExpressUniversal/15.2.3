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
using DevExpress.Utils;
using ModelDefinedName = DevExpress.XtraSpreadsheet.Model.DefinedName;
using ModelDefinedNameBase = DevExpress.XtraSpreadsheet.Model.DefinedNameBase;
namespace DevExpress.Spreadsheet {
	#region ExternalWorkbookCollection
	public interface ExternalWorkbookCollection : ISimpleCollection<ExternalWorkbook>, ICollection<ExternalWorkbook> {
		new int Count { get; }
		int IndexOf(ExternalWorkbook item);
		void Replace(int itemIndex, ExternalWorkbook newitem);
	}
	#endregion
	public interface ISupportsContentChanged {
		event EventHandler ContentChanged;
	}
	public interface ExternalWorkbook : ISupportsContentChanged {
		string Path { get; }
		System.Collections.Generic.IEnumerable<ExternalWorksheet> Worksheets { get; }
		System.Collections.Generic.IEnumerable<ExternalDefinedName> DefinedNames { get; }
		event EventHandler SchemaChanged;
	}
	public interface ExternalWorksheet {
		string Name { get; }
		CellValue GetCellValue(int columnIndex, int rowIndex);
		System.Collections.Generic.IEnumerable<ExternalDefinedName> DefinedNames { get; }
	}
	public interface ExternalDefinedName {
		string Name { get; }
		string RefersTo { get; }
	}
}
#region ExternalWorkbookCollection Implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using ModelExternalLink = DevExpress.XtraSpreadsheet.Model.External.ExternalLink;
	using ModelExternalWorkbook = DevExpress.XtraSpreadsheet.Model.External.ExternalWorkbook;
	using ModelExternalWorksheet = DevExpress.XtraSpreadsheet.Model.External.ExternalWorksheet;
	using ModelExternalDefinedName = DevExpress.XtraSpreadsheet.Model.External.ExternalDefinedName;
	using ModelExternalLinkCollection = DevExpress.XtraSpreadsheet.Model.External.ExternalLinkCollection;
	using ModelDocumentModel = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using Model = DevExpress.XtraSpreadsheet.Model;
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Collections;
	#region NativeExternalWorkbookCollection
	partial class NativeExternalWorkbookCollection : ExternalWorkbookCollection {
		#region Fields
		NativeWorkbook nativeWorkbook;
		readonly List<ExternalWorkbook> innerList;
		#endregion
		internal NativeExternalWorkbookCollection(NativeWorkbook nativeWorkbook) {
			this.nativeWorkbook = nativeWorkbook;
			this.innerList = new List<ExternalWorkbook>();
		}
		public void Initialize() {
			this.innerList.Clear();
			ModelDocumentModel documentModel = nativeWorkbook.DocumentModel;
			foreach (ModelExternalLink externalLink in documentModel.ExternalLinks) {
				RegisterModelExternalLink(externalLink);
			}
		}
		void RegisterModelExternalLink(ModelExternalLink externalLink) {
			if (externalLink.IsExternalWorkbook) {
				NativeExternalWorkbook nativeBook = new NativeExternalWorkbook(externalLink.Workbook);
				nativeBook.Initialize();
				innerList.Add(nativeBook);
			}
		}
		void UnRegisterModelExternalLink(ModelExternalLink externalLink) {
			if (externalLink.IsExternalWorkbook) {
				foreach (ExternalWorkbook externalWorkbook in this.innerList)
					if (StringExtensions.CompareInvariantCultureIgnoreCase(externalWorkbook.Path, externalLink.Workbook.FilePath) == 0) {
						this.innerList.Remove(externalWorkbook);
						break;
					}
			}
		}
		ModelExternalWorksheet CreateExternalWorksheet(ExternalWorksheet worksheet, ModelExternalWorkbook modelExternalWorkbook) {
			ModelExternalWorksheet result = new ModelExternalWorksheet(modelExternalWorkbook, worksheet.Name, worksheet);
			IEnumerable<ExternalDefinedName> definedNames = worksheet.DefinedNames;
			if (definedNames != null)
				foreach (ExternalDefinedName definedName in definedNames) {
					string name = definedName.Name;
					modelExternalWorkbook.CheckDefinedName(name, -1);
					if (result.DefinedNames.Contains(name))
						SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ErrorDefinedNameAlreadyExists);
					result.DefinedNames.Add(new ModelExternalDefinedName(name, modelExternalWorkbook, definedName.RefersTo, -1));
				}
			return result;
		}
		#region ICollection<ExternalWorkbook> Members
		public void Add(ExternalWorkbook item) {
			int modelIndex = nativeWorkbook.ModelWorkbook.ExternalLinks.Count;
			AddCore(item, innerList.Count, modelIndex, false);
		}
		ModelExternalLink PrepareModelExternalLink(ExternalWorkbook item, bool allowSameLinkName) {
			if (item == null)
				Exceptions.ThrowArgumentNullException("item");
			ModelDocumentModel modelWorkbook = nativeWorkbook.ModelWorkbook;
			if (string.IsNullOrEmpty(item.Path))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ExternalLinkInvalidPath);
			if (!allowSameLinkName && (innerList.Contains(item) || modelWorkbook.ExternalLinks.IndexOf(item.Path) >= 0))
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ExternalLinkAlreadyExists);
			ModelExternalLink modelLink = new ModelExternalLink(modelWorkbook);
			ModelExternalWorkbook modelExternalWorkbook = modelLink.Workbook;
			modelExternalWorkbook.FilePath = item.Path;
			IEnumerable<ExternalWorksheet> worksheets = item.Worksheets;
			if (worksheets == null)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ExternalWorkbookWithoutSheets);
			foreach (ExternalWorksheet worksheet in worksheets)
				modelExternalWorkbook.Sheets.Add(CreateExternalWorksheet(worksheet, modelExternalWorkbook));
			if (modelExternalWorkbook.Sheets.Count < 1)
				SpreadsheetExceptions.ThrowInvalidOperationException(XtraSpreadsheetStringId.Msg_ExternalWorkbookWithoutSheets);
			IEnumerable<ExternalDefinedName> definedNames = item.DefinedNames;
			if (definedNames != null)
				foreach (ExternalDefinedName definedName in definedNames) {
					string name = definedName.Name;
					modelExternalWorkbook.CheckDefinedName(name, -1);
					modelExternalWorkbook.DefinedNames.Add(new ModelExternalDefinedName(name, modelExternalWorkbook, definedName.RefersTo, -1));
				}
			return modelLink;
		}
		protected internal void AddCore(ExternalWorkbook item, int position, int modelPosition, bool modifyReferences) {
			ModelExternalLink modelLink = PrepareModelExternalLink(item, false);
			RegisterNewItem(item, modelLink, position, modelPosition, modifyReferences);
		}
		void RegisterNewItem(ExternalWorkbook item, ModelExternalLink modelLink, int position, int modelPosition, bool modifyReferences) {
			nativeWorkbook.ModelWorkbook.ExternalLinks.AddCore(modelLink, modelPosition, modifyReferences);
			item.ContentChanged += item_ContentChanged;
			item.SchemaChanged += item_SchemaChanged;
			innerList.Insert(position, item);
		}
		void item_ContentChanged(object sender, EventArgs e) {
			nativeWorkbook.DocumentModel.CalculationChain.ForceCalculate();
			nativeWorkbook.DocumentModel.InnerApplyChanges(Model.DocumentModelChangeActions.RaiseUpdateUI);
		}
		void item_SchemaChanged(object sender, EventArgs e) {
			ExternalWorkbook item = sender as ExternalWorkbook;
			if (item == null)
				return;
			RecreateItem(item);
			nativeWorkbook.DocumentModel.CalculationChain.ForceCalculate();
			nativeWorkbook.DocumentModel.InnerApplyChanges(Model.DocumentModelChangeActions.RaiseUpdateUI);
		}
		public int Count { get { return innerList.Count; } }
		public bool IsReadOnly { get { return false; } }
		public void Clear() {
			foreach (ExternalWorkbook item in innerList) {
				item.ContentChanged -= item_ContentChanged;
				item.SchemaChanged -= item_SchemaChanged;
			}
			Model.ExternalWorkbookCollectionClearCommand command = new Model.ExternalWorkbookCollectionClearCommand(nativeWorkbook.ModelWorkbook, ApiErrorHandler.Instance);
			command.Execute();
		}
		public bool Contains(ExternalWorkbook item) {
			return innerList.Contains(item);
		}
		public void CopyTo(ExternalWorkbook[] array, int arrayIndex) {
			Array.Copy(innerList.ToArray(), 0, array, arrayIndex, innerList.Count);
		}
		public bool Remove(ExternalWorkbook item) {
			return RemoveCore(item, true);
		}
		bool RemoveCore(ExternalWorkbook item, bool modifyReferences) {
			ModelExternalLink modelLink = GetModelExternalLinkByItem(item);
			if (modelLink != null) {
				Model.ExternalWorkbookRemoveCommand command = new Model.ExternalWorkbookRemoveCommand(modelLink, nativeWorkbook.ModelWorkbook, ApiErrorHandler.Instance, modifyReferences);
				command.Execute();
				item.ContentChanged -= item_ContentChanged;
				item.SchemaChanged -= item_SchemaChanged;
			}
			return true;
		}
		ModelExternalLink GetModelExternalLinkByItem(ExternalWorkbook item) {
			if (!ContainsByPath(item))
				return null;
			return this.nativeWorkbook.ModelWorkbook.ExternalLinks[item.Path];
		}
		int GetModelExternalLinkIndexByItem(ExternalWorkbook item) {
			if (!ContainsByPath(item))
				return -1;
			ModelExternalLinkCollection modelCollection = this.nativeWorkbook.ModelWorkbook.ExternalLinks;
			return modelCollection.IndexOf(item.Path);
		}
		#endregion
		void RecreateItem(ExternalWorkbook item) {
			int index = LookUpItemByName(item);
			int modelPosition = GetModelExternalLinkIndexByItem(item);
			if (RemoveCore(item, false))
				AddCore(item, index, modelPosition, false);
		}
		bool ContainsByPath(ExternalWorkbook item) {
			return LookUpItemByName(item) >= 0;
		}
		int LookUpItemByName(ExternalWorkbook item) {
			string itemPath = item.Path;
			for (int i = 0; i < innerList.Count; i++)
				if (StringExtensions.CompareInvariantCultureIgnoreCase(innerList[i].Path, itemPath) == 0)
					return i;
			return -1;
		}
		public void Replace(int itemIndex, ExternalWorkbook newitem) {
			nativeWorkbook.ModelWorkbook.History.DisableHistory();
			try {
				ModelExternalLink modelLink = PrepareModelExternalLink(newitem, true);
				ExternalWorkbook oldItem = this.innerList[itemIndex];
				int modelPosition = GetModelExternalLinkIndexByItem(oldItem);
				nativeWorkbook.ModelWorkbook.BeginUpdate();
				try {
					if (RemoveCore(oldItem, false))
						RegisterNewItem(newitem, modelLink, itemIndex, modelPosition, false);
				}
				finally {
					nativeWorkbook.ModelWorkbook.EndUpdate();
				}
			}
			finally {
				nativeWorkbook.ModelWorkbook.History.EnableHistory();
			}
		}
		#region IEnumerable<ExternalWorkbook> Members
		public IEnumerator<ExternalWorkbook> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		#region ISimpleCollection<ExternalWorkbook> Members
		public ExternalWorkbook this[int index] { get { return innerList[index]; } }
		#endregion
		#region ExternalWorkbookCollection Members
		public int IndexOf(ExternalWorkbook item) {
			return innerList.IndexOf(item);
		}
		#endregion
		#region ICollection Members
		bool ICollection.IsSynchronized { get { return ((ICollection)this.innerList).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((ICollection)this.innerList).SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		#endregion
		#region Invalidate
		protected internal virtual void Invalidate() {
			foreach (ExternalWorkbook workbook in innerList) {
				NativeExternalWorkbook externalWorkbook = workbook as NativeExternalWorkbook;
				if (externalWorkbook != null)
					externalWorkbook.Invalidate();
			}
			innerList.Clear();
		}
		#endregion
		#region Event handlers
		public void OnExternalLinks_LinkAdded(object sender, DevExpress.XtraSpreadsheet.Model.ExternalLinksCollectionChangedEventArgs args) {
			RegisterModelExternalLink(args.NewExternalLink);
		}
		public void OnExternalLinks_LinkRemoved(object sender, DevExpress.XtraSpreadsheet.Model.ExternalLinksCollectionChangedEventArgs args) {
			UnRegisterModelExternalLink(args.NewExternalLink);
		}
		public void OnExternalLinks_Cleared(object sender, EventArgs args) {
			ClearCore();
		}
		#endregion
		void ClearCore() {
			innerList.Clear();
		}
	}
	#endregion
	#region NativeExternalWorkbook
	partial class NativeExternalWorkbook : ExternalWorkbook {
		#region Fields
		readonly ModelExternalWorkbook modelExternalWorkbook;
		List<NativeExternalWorksheet> worksheets;
		List<NativeExternalDefinedName> definedNames;
		#endregion
		internal NativeExternalWorkbook(ModelExternalWorkbook modelExternalWorkbook) {
			this.modelExternalWorkbook = modelExternalWorkbook;
			this.CreateApiObjects();
		}
		protected internal virtual void CreateApiObjects() {
			this.worksheets = new List<NativeExternalWorksheet>();
			this.definedNames = new List<NativeExternalDefinedName>();
		}
		protected internal void Initialize() {
			worksheets.Clear();
			foreach (ModelExternalWorksheet sheet in modelExternalWorkbook.Sheets) {
				NativeExternalWorksheet nativeSheet = new NativeExternalWorksheet(sheet);
				nativeSheet.Initialize();
				worksheets.Add(nativeSheet);
			}
			definedNames.Clear();
			foreach (ModelDefinedNameBase definedName in modelExternalWorkbook.DefinedNames)
				definedNames.Add(new NativeExternalDefinedName((ModelExternalDefinedName)definedName));
		}
		#region ExternalWorkbook Members
		public string Path { get { return modelExternalWorkbook.FilePath; } }
		public List<NativeExternalWorksheet> Worksheets { get { return worksheets; } }
		public List<NativeExternalDefinedName> DefinedNames { get { return definedNames; } }
		IEnumerable<ExternalWorksheet> ExternalWorkbook.Worksheets { get { return worksheets; } }
		IEnumerable<ExternalDefinedName> ExternalWorkbook.DefinedNames { get { return definedNames; } }
		public event EventHandler ContentChanged { add { } remove { } }
		public event EventHandler SchemaChanged { add { } remove { } }
		#endregion
		#region Invalidate
		protected internal virtual void Invalidate() {
			foreach (NativeExternalWorksheet worksheet in worksheets) {
				worksheet.Invalidate();
			}
			foreach (NativeExternalDefinedName definedName in definedNames) {
				definedName.Invalidate();
			}
			worksheets.Clear();
			definedNames.Clear();
		}
		#endregion
	}
	#endregion
	#region NativeExternalWorksheet
	partial class NativeExternalWorksheet : ExternalWorksheet {
		#region Fields
		readonly ModelExternalWorksheet modelExternalWorksheet;
		List<NativeExternalDefinedName> definedNames;
		#endregion
		internal NativeExternalWorksheet(ModelExternalWorksheet modelExternalWorksheet) {
			this.modelExternalWorksheet = modelExternalWorksheet;
			this.CreateApiObjects();
		}
		protected internal virtual void CreateApiObjects() {
			this.definedNames = new List<NativeExternalDefinedName>();
		}
		protected internal virtual void Initialize() {
			definedNames.Clear();
			foreach (ModelExternalDefinedName definedName in modelExternalWorksheet.DefinedNames)
				definedNames.Add(new NativeExternalDefinedName(definedName));
		}
		#region ExternalWorksheet Members
		public string Name { get { return modelExternalWorksheet.Name; } }
		public CellValue GetCellValue(int columnIndex, int rowIndex) {
			Model.WorkbookDataContext context = modelExternalWorksheet.Workbook.DataContext;
			return new CellValue(modelExternalWorksheet.GetCalculatedCellValue(columnIndex, rowIndex), context);
		}
		public List<NativeExternalDefinedName> DefinedNames { get { return definedNames; } }
		IEnumerable<ExternalDefinedName> ExternalWorksheet.DefinedNames { get { return definedNames; } }
		#endregion
		#region Invalidate
		protected internal virtual void Invalidate() {
			foreach (NativeExternalDefinedName definedName in definedNames) {
				definedName.Invalidate();
			}
			definedNames.Clear();
		}
		#endregion
	}
	#endregion
	#region NativeExternalDefinedName
	partial class NativeExternalDefinedName : ExternalDefinedName {
		#region Fields
		readonly ModelExternalDefinedName modelExternalDefinedName;
		#endregion
		internal NativeExternalDefinedName(ModelExternalDefinedName modelExternalDefinedName) {
			this.modelExternalDefinedName = modelExternalDefinedName;
		}
		#region ExternalDefinedName Members
		public string Name { get { return modelExternalDefinedName.Name; } }
		public string RefersTo { get { return "=" + modelExternalDefinedName.GetReference(0, 0); } }
		#endregion
		#region Invalidate
		protected internal virtual void Invalidate() {
		}
		#endregion
	}
	#endregion
}
#endregion
