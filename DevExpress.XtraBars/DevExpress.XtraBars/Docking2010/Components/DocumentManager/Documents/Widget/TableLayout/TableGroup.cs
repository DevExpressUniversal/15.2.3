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

using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class TableGroup : BaseComponent, IDocumentGroup {
		TableDocumentCollection itemsCore;
		RowDefinitionCollection rowsCore;
		ColumnDefinitionCollection columnsCore;
		TableGroupInfo infoCore;
		WidgetView viewCore;
		protected internal const string Name = "TableLayoutGroup";
		public TableGroup(WidgetView view)
			: base(null) {
			rowsCore = CreateRowsCollection();
			columnsCore = CreateColumnsCollection();
			Rows.CollectionChanged += OnRowsCollectionChanged;
			Columns.CollectionChanged += OnColumnsCollectionChanged;
			viewCore = view;
			infoCore = CreateInfo();
			itemsCore = CreateTableDocumentCollection();
			Items.CollectionChanged += OnItemsCollectionChanged;
		}
		protected virtual TableGroupInfo CreateInfo() {
			return new TableGroupInfo(WidgetView, this);
		}
		public WidgetView WidgetView {
			get { return viewCore; }
		}
		public TableGroupInfo Info {
			get {
				if(infoCore == null)
					infoCore = CreateInfo();
				return infoCore;
			}
		}
		void OnRowsCollectionChanged(CollectionChangedEventArgs<RowDefinition> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				ea.Element.LengthChanged += ElementLengthChanged;
				Info.Register(ea.Element);
				if(viewCore != null)
					viewCore.AddToContainer(ea.Element);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				ea.Element.LengthChanged -= ElementLengthChanged;
				Info.Unregister(ea.Element);
				if(viewCore != null)
					viewCore.RemoveFromContainer(ea.Element);
			}
			SetLayoutModified();
			UpdateSizes();
			LayoutChanged();
		}
		private void UpdateSizes() {
			if(WidgetView.Manager != null && !WidgetView.IsUpdateLocked) {
				var widgetHost = WidgetView.Manager.GetOwnerControl() as WidgetsHost;
				if(widgetHost != null)
					widgetHost.UpdateLayoutWithAnimation();
			}
		}
		void ElementLengthChanged(object sender, System.EventArgs e) {
			LayoutChanged();
		}
		void OnColumnsCollectionChanged(CollectionChangedEventArgs<ColumnDefinition> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				Info.Register(ea.Element);
				ea.Element.LengthChanged += ElementLengthChanged;
				if(viewCore != null)
					viewCore.AddToContainer(ea.Element);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				Info.Unregister(ea.Element);
				ea.Element.LengthChanged -= ElementLengthChanged;
				if(viewCore != null)
					viewCore.RemoveFromContainer(ea.Element);
			}
			SetLayoutModified();
			UpdateSizes();
			LayoutChanged();
		}
		void SetLayoutModified() {
			if(WidgetView != null) {
				WidgetView.SetLayoutModified();
			}
		}
		protected override void OnCreate() {
			base.OnCreate();
		}
		protected override void OnDispose() {
			if(Items != null)
				Items.CollectionChanged -= OnItemsCollectionChanged;
			Ref.Dispose(ref rowsCore);
			Ref.Dispose(ref columnsCore);
			base.OnDispose();
		}
		void OnItemsCollectionChanged(CollectionChangedEventArgs<Document> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				Info.Register(ea.Element);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				Info.Unregister(ea.Element);
				if(WidgetView != null && WidgetView.Manager != null)
					WidgetView.Manager.Invalidate();
			}
			LayoutChanged();
		}
		protected internal void Register(RowDefinition row) {
			if(Info != null)
				Info.Register(row);
		}
		protected internal void Register(ColumnDefinition column) {
			if(Info != null)
				Info.Register(column);
		}
		protected internal void Unregister(RowDefinition row) {
			if(Info != null)
				Info.Unregister(row);
		}
		protected internal void Unregister(ColumnDefinition column) {
			if(Info != null)
				Info.Unregister(column);
		}
		protected override void OnLayoutChanged() {
			base.OnLayoutChanged();
			if(viewCore.Manager != null && viewCore.Manager.GetOwnerControl() is WidgetsHost) {
				(viewCore.Manager.GetOwnerControl() as WidgetsHost).LayoutChanged();
			}
		}
		protected virtual ColumnDefinitionCollection CreateColumnsCollection() {
			return new ColumnDefinitionCollection();
		}
		protected virtual RowDefinitionCollection CreateRowsCollection() {
			return new RowDefinitionCollection();
		}
		protected virtual TableDocumentCollection CreateTableDocumentCollection() {
			return new TableDocumentCollection(this);
		}
		WidgetViewController Controller {
			get { return WidgetView.Controller as WidgetViewController; }
		}
		protected internal void Add(BaseDocument document) {
			if(Controller != null)
				Controller.Dock(document as Document, this);
		}
		public RowDefinitionCollection Rows { get { return rowsCore; } }
		public ColumnDefinitionCollection Columns { get { return columnsCore; } }
		public TableDocumentCollection Items {
			get { return itemsCore; }
		}
		internal Document GetDocumentWithSpan(int columnIndex, int rowIndex) {
			foreach(var item in Items) {
				if(item.ColumnIndex == columnIndex && item.RowIndex == rowIndex && item.IsVisible)
					return item;
				bool column = false;
				bool row = false;
				if(item.ColumnIndex <= columnIndex && item.ColumnIndex + (item.ColumnSpan > 0 ? item.ColumnSpan - 1 : 0) >= columnIndex) {
					column = true;
				}
				if(item.RowIndex <= rowIndex && item.RowIndex + (item.RowSpan > 0 ? item.RowSpan - 1 : 0) >= rowIndex) {
					row = true;
				}
				if(column & row && item.IsVisible)
					return item;
			}
			return null;
		}
		internal Document GetDocument(int columnIndex, int rowIndex) {
			foreach(var item in Items) {
				if(item.ColumnIndex == columnIndex && item.RowIndex == rowIndex && item.IsVisible)
					return item;
			}
			return null;
		}
		internal IEnumerable<Document> GetDocumentsByColumn(int columnIndex) {
			foreach(var item in Items) {
				if(item.ColumnIndex == columnIndex)
					yield return item;
			}
		}
		internal IEnumerable<Document> GetDocumentsByRow(int rowIndex) {
			foreach(var item in Items) {
				if(item.RowIndex == rowIndex)
					yield return item;
			}
		}
		protected internal bool CheckLeft(Document document) {
			if(document.ColumnIndex == 0) return false;
			for(int i = document.RowIndex; i < document.RowIndex + document.RowSpan; i++) {
				if(GetDocumentWithSpan(document.ColumnIndex - 1, i) != null)
					return false;
			}
			return true;
		}
		protected internal bool CheckRight(Document document) {
			if(document.ColumnIndex + document.ColumnSpan - 1 == Columns.Count - 1) return false;
			for(int i = document.RowIndex; i < document.RowIndex + document.RowSpan; i++) {
				if(GetDocumentWithSpan(document.ColumnIndex + document.ColumnSpan, i) != null) {
					return false;
				}
			}
			return true;
		}
		protected internal bool ValidateDocumentPosition(Document checkedDocument, Document currentDocument) {
			for(int i = checkedDocument.RowIndex; i < checkedDocument.RowIndex + checkedDocument.RowSpan; i++) {
				for(int j = checkedDocument.ColumnIndex; j < checkedDocument.ColumnIndex + checkedDocument.ColumnSpan; j++) {
					var document = GetDocumentWithSpan(j, i);
					if(document != null && checkedDocument != document && !(document is EmptyDocument) && document != currentDocument)
						return true;
				}
			}
			return false;
		}
		protected internal bool CheckTop(Document document) {
			if(document.RowIndex == 0) return false;
			for(int i = document.ColumnIndex; i < document.ColumnIndex + document.ColumnSpan; i++) {
				if(GetDocumentWithSpan(i, document.RowIndex - 1) != null) {
					return false;
				}
			}
			return true;
		}
		protected internal bool CheckBottom(Document document) {
			if(document.RowIndex + document.RowSpan - 1 == Rows.Count - 1) return false;
			for(int i = document.ColumnIndex; i < document.ColumnIndex + document.ColumnSpan; i++) {
				if(GetDocumentWithSpan(i, document.RowIndex + document.RowSpan) != null) {
					return false;
				}
			}
			return true;
		}
		protected internal bool CheckBottomRight(Document document) {
			if(!CheckBottom(document) || !CheckRight(document)) return false;
			if(GetDocumentWithSpan(document.ColumnIndex + document.ColumnSpan, document.RowIndex + document.RowSpan) != null) return false;
			return true;
		}
		protected internal bool CheckBottomLeft(Document document) {
			if(!CheckBottom(document) || !CheckLeft(document)) return false;
			if(GetDocumentWithSpan(document.ColumnIndex - 1, document.RowIndex + document.RowSpan) != null) return false;
			return true;
		}
		protected internal bool CheckTopRight(Document document) {
			if(!CheckTop(document) || !CheckRight(document)) return false;
			if(GetDocumentWithSpan(document.ColumnIndex + document.ColumnSpan, document.RowIndex - 1) != null) return false;
			return true;
		}
		protected internal bool CheckTopLeft(Document document) {
			if(!CheckTop(document) || !CheckLeft(document)) return false;
			if(GetDocumentWithSpan(document.ColumnIndex - 1, document.RowIndex - 1) != null) return false;
			return true;
		}
		#region IDocumentGroup Members
		IBaseElementInfo IDocumentGroup.Info {
			get { return Info as IBaseElementInfo; }
		}
		BaseMutableList<Document> IDocumentGroup.Items {
			get { return Items; }
		}
		#endregion
	}
	public class TableDocumentCollection : BaseDocumentCollection<Document, TableGroup> {
		public TableDocumentCollection(TableGroup owner)
			: base(owner) {
		}
		protected override void NotifyOwnerOnInsert(int index) { }
	}
}
