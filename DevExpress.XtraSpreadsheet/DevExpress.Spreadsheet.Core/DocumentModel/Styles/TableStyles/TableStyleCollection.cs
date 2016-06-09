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
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Model {
	public class TableStyleCollection : ICloneable<TableStyleCollection>, ISupportsCopyFrom<TableStyleCollection>, IBatchUpdateable, IBatchUpdateHandler, IEnumerable<TableStyle> {
		#region Fields
		readonly DocumentModel documentModel;
		readonly Dictionary<string, TableStyle> innerCollection;
		BatchUpdateHelper batchUpdateHelper;
		bool deferredRaiseChanged;
		string cachedDefaultTableStyleName;
		string cachedDefaultPivotStyleName;
		bool hasDefaultTableStyleName;
		bool hasDefaultPivotStyleName;
		#endregion
		public TableStyleCollection(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			innerCollection = new Dictionary<string, TableStyle>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			string defaultName = TableStyleName.DefaultStyleName.Name;
			cachedDefaultTableStyleName = defaultName;
			cachedDefaultPivotStyleName = defaultName;
			AddDefaultStyle();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public TableStyle this[string name] { get { return innerCollection[name]; } }
		public TableStyle DefaultTableStyle { get { return innerCollection[cachedDefaultTableStyleName]; } }
		public TableStyle DefaultPivotStyle { get { return innerCollection[cachedDefaultPivotStyleName]; } }
		public string CachedDefaultTableStyleName { get { return cachedDefaultTableStyleName; } }
		public string CachedDefaultPivotStyleName { get { return cachedDefaultPivotStyleName; } }
		public int Count { get { return innerCollection.Count; } }
		public bool HasDefaultTableStyleName { get { return hasDefaultTableStyleName; } }
		public bool HasDefaultPivotStyleName { get { return hasDefaultPivotStyleName; } }
		#endregion
		protected internal void AddDefaultStyle() {
			if (!innerCollection.ContainsKey(TableStyleName.DefaultStyleName.Name))
				AddCore(new TableStyle(documentModel, TableStyleName.DefaultStyleName, TableStyleElementIndexTableType.General));
		}
		#region HistoryOperations
		#region Add
		public void Add(TableStyle style) {
			if (style != null && !ContainsStyle(style))
				ApplyHistoryItem(new TableStyleCollectionAddHistoryItem(this, style));
		}
		protected internal void AddCore(TableStyle style) {
			style.IsRegistered = true;
			RaiseStyleAdded(style);
			innerCollection.Add(style.Name.Name, style);
			OnCollectionChanged();
		}
		#endregion
		#region Remove
		public void Remove(string name) {
			if (ContainsStyleName(name))
				ApplyHistoryItem(new TableStyleCollectionRemoveHistoryItem(this, innerCollection[name]));
		}
		protected internal void RemoveCore(TableStyle style) {
			style.IsRegistered = false;
			RaiseStyleRemoved(style);
			innerCollection.Remove(style.Name.Name);
			OnCollectionChanged();
		}
		#endregion
		#region Clear
		public void ClearAll() {
			if (Count != 0)
				ApplyHistoryItem(new TableStyleCollectionClearAllHistoryItem(this));
		}
		public void ClearPart(TableStyleElementIndexTableType tableType) {
			if (Count == 0)
				return;
			documentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					ApplyHistoryItem(new TableStyleCollectionClearPartHistoryItem(this, tableType));
				} finally {
					EndUpdate();
				}
			} finally {
				documentModel.EndUpdate();
			}
		}
		protected internal void ClearAllCore() {
			RaiseCollectionCleared(TableStyleElementIndexTableType.General);
			innerCollection.Clear();
			OnCollectionChanged();
		}
		protected internal void ClearPartStylesCore(TableStyleElementIndexTableType tableType) {
			RaiseCollectionCleared(tableType);
			foreach (TableStyle style in innerCollection.Values) {
				if (style.TableType == tableType) {
					style.IsRegistered = false;
					innerCollection.Remove(style.Name.Name);
				}
			}
			OnCollectionChanged();
		}
		#endregion
		void OnCollectionChanged() {
			if (IsUpdateLocked)
				this.deferredRaiseChanged = true;
			else
				RaiseCollectionChanged();
		}
		#region SetDefaultTableStyleName
		public void SetDefaultTableStyleName(string name) {
			if (!ShouldSetDefaultTableStyleName(name))
				return;
			documentModel.BeginUpdate();
			try {
				ApplyHistoryItem(new TableStyleCollectionSetDefaultTableStyleHistoryItem(this, cachedDefaultTableStyleName, name));
				ApplyHistoryItem(new TableStyleCollectionHasDefaultTableStyleNameHistoryItem(this, hasDefaultTableStyleName, true));
			} finally {
				documentModel.EndUpdate();
			}
		}
		bool ShouldSetDefaultTableStyleName(string name) {
			return !String.IsNullOrEmpty(name) && !TableStyleName.CompareStrings(name, cachedDefaultTableStyleName) && ContainsStyleName(name);
		}
		protected internal void SetDefaultTableStyleNameCore(string name) {
			cachedDefaultTableStyleName = name;
		}
		protected internal void SetHasDefaultTableStyleNameCore(bool value) {
			hasDefaultTableStyleName = value;
		}
		#endregion
		#region SetDefaultPivotStyleName
		public void SetDefaultPivotStyleName(string name) {
			if (!ShouldSetDefaultPivotStyleName(name))
				return;
			documentModel.BeginUpdate();
			try {
				ApplyHistoryItem(new TableStyleCollectionSetDefaultPivotStyleHistoryItem(this, cachedDefaultPivotStyleName, name));
				ApplyHistoryItem(new TableStyleCollectionHasDefaultPivotStyleNameHistoryItem(this, hasDefaultPivotStyleName, true));
			} finally {
				documentModel.EndUpdate();
			}
		}
		bool ShouldSetDefaultPivotStyleName(string name) {
			return !String.IsNullOrEmpty(name) && !TableStyleName.CompareStrings(name, cachedDefaultPivotStyleName) && ContainsStyleName(name);
		}
		protected internal void SetDefaultPivotStyleNameCore(string name) {
			cachedDefaultPivotStyleName = name;
		}
		protected internal void SetHasDefaultPivotStyleNameCore(bool value) {
			hasDefaultPivotStyleName = value;
		}
		#endregion
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#endregion
		#region StyleAdded event
		TableStyleCollectionChangedEventHandler onStyleAdded;
		public event TableStyleCollectionChangedEventHandler StyleAdded { add { onStyleAdded += value; } remove { onStyleAdded -= value; } }
		protected internal virtual void RaiseStyleAdded(TableStyle newStyle) {
			if (onStyleAdded != null) {
				TableStyleCollectionChangedEventArgs args = new TableStyleCollectionChangedEventArgs(newStyle);
				onStyleAdded(this, args);
			}
		}
		#endregion
		#region StyleRemoved event
		TableStyleCollectionChangedEventHandler onStyleRemoved;
		internal event TableStyleCollectionChangedEventHandler StyleRemoved { add { onStyleRemoved += value; } remove { onStyleRemoved -= value; } }
		protected internal virtual void RaiseStyleRemoved(TableStyle style) {
			if (onStyleRemoved != null) {
				TableStyleCollectionChangedEventArgs args = new TableStyleCollectionChangedEventArgs(style);
				onStyleRemoved(this, args);
			}
		}
		#endregion
		#region Clear Event
		TableStyleCollectionClearedEventHandler onCollectionClear;
		internal event TableStyleCollectionClearedEventHandler CollectionCleared { add { onCollectionClear += value; } remove { onCollectionClear -= value; } }
		protected internal virtual void RaiseCollectionCleared(TableStyleElementIndexTableType tableType) {
			if (onCollectionClear != null) {
				TableStyleCollectionClearedEventArgs args = new TableStyleCollectionClearedEventArgs(tableType);
				onCollectionClear(this, args);
			}
		}
		#endregion
		#region CollectionChanged
		EventHandler onCollectionChanged;
		public event EventHandler CollectionChanged { add { onCollectionChanged += value; } remove { onCollectionChanged -= value; } }
		protected internal virtual void RaiseCollectionChanged() {
			if (onCollectionChanged != null)
				onCollectionChanged(this, EventArgs.Empty);
		}
		#endregion
		public void CopyFrom(DocumentModel targetDocumentModel, TableStyleCollection sourceStyles) {
			this.ClearAllCore();
			foreach (TableStyle sourceStyle in sourceStyles) {
				TableStyle targetStyle = sourceStyle.CloneTo(targetDocumentModel);
				AddCore(targetStyle);
			}
		}
		public bool ContainsStyle(TableStyle style) {
			return ContainsStyleName(style.Name.Name);
		}
		public bool ContainsStyleName(string name) {
			return innerCollection.ContainsKey(name);
		}
		public IList<string> GetExistingCustomTableStyleNames() {
			return GetExistingStyleNames(CheckCustomTableStyle);
		}
		public IList<string> GetExistingCustomPivotStyleNames() {
			return GetExistingStyleNames(CheckCustomPivotStyle);
		}
		public IList<string> GetExistingNonHiddenCustomTableStyleNames() {
			return GetExistingStyleNames(CheckNonHiddenCustomTableStyle);
		}
		public IList<string> GetExistingNonHiddenCustomPivotStyleNames() {
			return GetExistingStyleNames(CheckNonHiddenCustomPivotStyle);
		}
		delegate bool HasStyle(TableStyle style);
		IList<string> GetExistingStyleNames(HasStyle hasStyle) {
			List<string> result = new List<string>();
			foreach (TableStyle style in this)
				if (hasStyle(style))
					result.Add(style.Name.Name);
			return result;
		}
		bool CheckCustomTableStyle(TableStyle style) {
			return !style.IsPredefined && style.IsTableStyle;
		}
		bool CheckCustomPivotStyle(TableStyle style) {
			return !style.IsPredefined && style.IsPivotStyle;
		}
		bool CheckNonHiddenCustomTableStyle(TableStyle style) {
			return !style.Name.IsDefault && !style.IsPredefined && !style.IsHidden && (style.IsTableStyle || style.IsGeneralStyle);
		}
		bool CheckNonHiddenCustomPivotStyle(TableStyle style) {
			return !style.Name.IsDefault && !style.IsPredefined && !style.IsHidden && (style.IsPivotStyle || style.IsGeneralStyle);
		}
		#region ISupportsCopyFrom<TableStyleCollection> Members
		public void CopyFrom(TableStyleCollection value) {
			cachedDefaultTableStyleName = value.cachedDefaultTableStyleName;
			cachedDefaultPivotStyleName = value.cachedDefaultPivotStyleName;
			hasDefaultTableStyleName = value.hasDefaultTableStyleName;
			hasDefaultPivotStyleName = value.hasDefaultPivotStyleName;
			innerCollection.Clear();
			foreach (TableStyle style in value.innerCollection.Values)
				innerCollection.Add(style.Name.Name, style);
		}
		#endregion
		#region ICloneable<TableStyleCollection> Members
		public TableStyleCollection Clone() {
			TableStyleCollection result = new TableStyleCollection(documentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			deferredRaiseChanged = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (deferredRaiseChanged)
				RaiseCollectionChanged();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		#region IEnumerable<TStyle> menbers
		public IEnumerator<TableStyle> GetEnumerator() {
			return innerCollection.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerCollection.Values.GetEnumerator();
		}
		#endregion
		public void ForEach(Action<TableStyle> action) {
			Guard.ArgumentNotNull(action, "action");
			foreach (string index in innerCollection.Keys)
				action(innerCollection[index]);
		}
		public string GetDefaultStyleName(bool forTables) {
			return forTables ? cachedDefaultTableStyleName : cachedDefaultPivotStyleName;
		}
	}
} 
