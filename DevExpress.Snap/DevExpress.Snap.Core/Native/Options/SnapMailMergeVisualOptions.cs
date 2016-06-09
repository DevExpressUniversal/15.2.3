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
using System.Drawing.Design;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Native.Options {
	[TypeConverter("DevExpress.Snap.Core.Native.Options.NativeSnapMailMergeOptionsTypeConverter," + AssemblyInfo.SRAssemblySnapCore)]
	public class NativeSnapMailMergeVisualOptions : RichEditNotificationOptions, SnapMailMergeVisualOptions {
		int currentRecordIndex = 0;
		string dataSourceName;
		string dataMember;
		string filterString = string.Empty;
		int recordCount = 0;
		readonly NativeMailMergeSorting sorting;
		readonly GroupFieldInfoCollection sortingBindings;
		readonly SnapDocumentModel documentModel;
		DataSourceInfo contextInfo;
		public NativeSnapMailMergeVisualOptions(SnapDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.sortingBindings = new GroupFieldInfoCollection(this);
			this.sorting = new NativeMailMergeSorting(this.sortingBindings);
			this.documentModel.DataSourceDispatcher.CollectionChanged += DataSourceDispatcherCollectionChanged;
		}
		void DataSourceDispatcherCollectionChanged(object sender, EventArgs e) {
			if(object.ReferenceEquals(dataSourceName, null))
				return;
			SetRootDataContext(false);
		}
		#region Properties
		#region DataSource
		[
		RefreshProperties(RefreshProperties.All),
#if !SL
 AttributeProvider(typeof(IListSource)),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(null)]
		public object DataSource {
			get {
				if (dataSourceName == null)
					return null;
				return DataSourceDispatcher.GetDataSource(dataSourceName);
			}
			set {
				if (Object.ReferenceEquals(DataSourceDispatcher.GetDataSource(dataSourceName), value))
					return;
				object oldDataSource = DataSource;
				DataSourceInfo info = DataSourceDispatcher.GetInfo(value);
				if (object.ReferenceEquals(info, null)) {
					info = DocumentModel.RegisterDataSource(value);
					dataSourceName = info.DataSourceName;
				}
				else
					dataSourceName = info.DataSourceName;
				SetRootDataContext(info, false);
				OnChanged("DataSource", oldDataSource, value);
				documentModel.RaiseSnapMailMergeDataSourceChanged();
			}
		}
		#endregion
		#region DataSourceName
		[Browsable(false)]
		public string DataSourceName {
			get { return dataSourceName; }
			set {
				if (string.Compare(dataSourceName, value) == 0)
					return;
				string oldDataSourceName = dataSourceName;
				dataSourceName = value;
				SetRootDataContext(false);
				OnChanged("DataSourceName", oldDataSourceName, value);
				documentModel.RaiseSnapMailMergeDataSourceChanged();
			}
		}
		#endregion
		#region IsMailMergeEnabled
		public bool IsMailMergeEnabled { get { return DataSource != null || DataSourceName != null; } }
		#endregion
		#region DataMember
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		RefreshProperties(RefreshProperties.All),
#if !SL
 Editor(ControlConstants.DataMemberEditor, typeof(UITypeEditor)),
#endif
 DefaultValue("")]
		public string DataMember {
			get { return dataMember; }
			set {
				if (string.Compare(dataMember, value) == 0)
					return;
				string oldDataMember = dataMember;
				dataMember = value ?? string.Empty;
				SetRootDataContext(true);
				OnChanged("DataMember", oldDataMember, value);
				documentModel.RaiseSnapMailMergeDataSourceChanged();
			}
		}
		bool ShouldSerializeDataMember() { return !string.IsNullOrEmpty(dataMember); }
		#endregion
		#region CurrentRecordIndex
		[
		DefaultValue(0)]
		public int CurrentRecordIndex {
			get { return currentRecordIndex; }
			set {
				if (currentRecordIndex == value)
					return;
				int oldValue = currentRecordIndex;
				DocumentModel.RaiseSnapMailMergeActiveRecordChanging();
				currentRecordIndex = value;
				SetRootDataContext(true);
				DocumentModel.RaiseSnapMailMergeActiveRecordChanged();
				OnChanged("CurrentRecordIndex", oldValue, value);
			}
		}
		#endregion
		#region Sorting
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
		public SnapListSorting Sorting { get { return sorting; } }
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Never),
		ResDisplayName(typeof(CoreResFinder), SnapResLocalizer.DefaultResourceFile, "SnapStringId.MailMergeSorting_MenuCaption"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GroupFieldInfoCollection SortingBindings { get { return sortingBindings; } }
		bool ShouldSerializeSortingBindings() { return sortingBindings.Count > 0; }
		#endregion
		#region FilterString
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.Snap.Extensions.Native.FilterStringEditor," + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor)),
		DefaultValue("")]
		public string FilterString {
			get { return filterString; }
			set {
				value = value ?? string.Empty;
				if (string.Compare(filterString, value) == 0)
					return;
				string oldFileterString = filterString;
				filterString = value;
				SetRootDataContext(true);
				OnChanged("FilterString", oldFileterString, value);
			}
		}
		bool ShouldSerializeFilterString() { return !string.IsNullOrEmpty(filterString); }
		#endregion
		public int RecordCount { get { return recordCount; } }
		protected internal SnapDocumentModel DocumentModel { get { return documentModel; } }
		protected internal DataSourceInfoCollection DataSources { get { return documentModel.DataSources; } }
		protected internal IDataSourceDispatcher DataSourceDispatcher { get { return documentModel.DataSourceDispatcher; } }
		#endregion
		protected internal void SetRootDataContext(bool force) {
			DataSourceInfo info;
			if(string.Equals(this.dataSourceName, DataSourceDispatcher.DefaultDataSourceName))
				info = DataSourceDispatcher.DefaultDataSourceInfo;
			else
				info = DataSourceDispatcher.GetInfo(this.dataSourceName);
			SetRootDataContext(info, force);
		}
		protected internal void SetRootDataContext(DataSourceInfo info, bool force) {
			if (IsLockUpdate)
				return;
			if(!force && InfosAreEqual(info, this.contextInfo))
				return;
			contextInfo = info;
			DocumentModel.BeginUpdate();
			DocumentModel.SetRootDataContext(info);
			DocumentModel.UpdateFields(UpdateFieldOperationType.Normal);
			DocumentModel.EndUpdate();
			RefreshRecordCount();
		}
		static bool InfosAreEqual(DataSourceInfo a, DataSourceInfo b) {
			if(object.ReferenceEquals(a, b))
				return true;
			if(object.ReferenceEquals(b, null) || object.ReferenceEquals(a, null))
				return false;
			if(!object.ReferenceEquals(a.DataSource, b.DataSource))
				return false;
			if(!string.Equals(a.DataSourceName, b.DataSourceName))
				return false;
			int n = a.CalculatedFields.Count;
			if(b.CalculatedFields.Count != n)
				return false;
			for(int i = 0; i < n; i++)
				if(!object.ReferenceEquals(a.CalculatedFields[i], b.CalculatedFields[i]))
					return false;
			return true;
		}
		public override void EndUpdate() {
			base.EndUpdate();
			if (DocumentModel != null)
				SetRootDataContext(true);
		}
		public virtual void CopyFrom(SnapMailMergeVisualOptions value) {
			BeginUpdate();
			CopyFromCore(value);
			EndUpdate();
		}
		protected virtual void CopyFromCore(SnapMailMergeVisualOptions value) {
			this.dataMember = value.DataMember;
			this.dataSourceName = value.DataSourceName;
			this.sorting.AddRange(value.Sorting);
			this.filterString = value.FilterString;
			this.currentRecordIndex = value.CurrentRecordIndex;
		}
		protected internal override void ResetCore() {
			ResetMailMerge();
		}
		public void ResetMailMerge() {
			this.dataMember = string.Empty;
			this.dataSourceName = null;
			this.currentRecordIndex = 0;
			this.filterString = string.Empty;
			if (this.sorting != null)
				this.sorting.Clear();
			if (DocumentModel != null)
				DocumentModel.RaiseSnapMailMergeDataSourceChanged();
		}
		internal void OnSortingChanged(GroupFieldInfoCollection oldValue) {
			OnChanged("SortingBindings", oldValue, SortingBindings);
		}
		public int RefreshRecordCount() {
			this.recordCount = 0;
			if (object.ReferenceEquals(DataSource, null))
				return 0;
			if(DataSource is System.Data.DataSet && string.IsNullOrEmpty(DataMember.Trim()))
				return 0;
			return RefreshRecordsCountCore();
		}
		int RefreshRecordsCountCore() {
			IFieldDataAccessService fieldDataAccessService = DocumentModel.GetService<IFieldDataAccessService>();
			IFieldContextService fieldContextService = fieldDataAccessService.FieldContextService;
			IFieldPathService fieldPathService = fieldDataAccessService.FieldPathService;
			ICalculationContext calculationContext = fieldContextService.BeginCalculation(DataSourceDispatcher);
			string path = FieldPathService.EncodePath(DataMember);
			FieldPathDataMemberInfo dataMemberInfo = fieldPathService.FromString(path).DataMemberInfo;
			dataMemberInfo.AddFilter(FilterString);
			IFieldContext dataContext = new RootFieldContext(DataSourceDispatcher, DataSourceName);
			using (IDataEnumerator dataEnumerator = calculationContext.GetChildDataEnumerator(dataContext, dataMemberInfo)) {
				if (!object.ReferenceEquals(dataEnumerator, null)) 
					while (dataEnumerator.MoveNext())
						this.recordCount++;
			}
			fieldContextService.EndCalculation(calculationContext);
			return this.recordCount;
		}
	}
	public class NativeSnapMailMergeOptionsTypeConverter : TypeConverter {
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value, attributes);
			if (!Object.ReferenceEquals(((IDataDispatcherOptions)value).DataSource, null))
				return properties;
			PropertyDescriptorCollection result = new PropertyDescriptorCollection(null);
			foreach (PropertyDescriptor item in properties)
				if (item.Name != "SortingBindings" && item.Name != "Filters")
					result.Add(item);
			return result;
		}
	}
	[Editor("DevExpress.Snap.Extensions.Native.SortingCollectionEditor," + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
	public class GroupFieldInfoCollection : IList<GroupFieldInfo>, ICloneable<GroupFieldInfoCollection>, IList {
		readonly List<GroupFieldInfo> innerList;
		readonly NativeSnapMailMergeVisualOptions owner;
		public GroupFieldInfoCollection(NativeSnapMailMergeVisualOptions owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.innerList = new List<GroupFieldInfo>();
		}
		protected void Update() {
			owner.SetRootDataContext(true);
		}
		#region IList<GroupFieldInfo> members
		public int IndexOf(GroupFieldInfo item) {
			return this.innerList.IndexOf(item);
		}
		public void Insert(int index, GroupFieldInfo item) {
			GroupFieldInfoCollection oldValue = Clone();
			innerList.Insert(index, item);
			Update();
			owner.OnSortingChanged(oldValue);
		}
		public void RemoveAt(int index) {
			GroupFieldInfoCollection oldValue = Clone();
			innerList.RemoveAt(index);
			Update();
			owner.OnSortingChanged(oldValue);
		}
		public GroupFieldInfo this[int index] {
			get { return innerList[index]; }
			set {
				if (innerList[index] == value)
					return;
				GroupFieldInfoCollection oldValue = Clone();
				innerList[index] = value;
				Update();
				owner.OnSortingChanged(oldValue);
			}
		}
		public void Add(GroupFieldInfo item) {
			GroupFieldInfoCollection oldValue = Clone();
			innerList.Add(item);
			Update();
			owner.OnSortingChanged(oldValue);
		}
		public void Clear() {
			if (Count == 0)
				return;
			GroupFieldInfoCollection oldValue = Clone();
			innerList.Clear();
			Update();
			owner.OnSortingChanged(oldValue);
		}
		public bool Contains(GroupFieldInfo item) {
			return innerList.Contains(item);
		}
		public void CopyTo(GroupFieldInfo[] array, int arrayIndex) {
			innerList.CopyTo(array, arrayIndex);
		}
		public int Count { get { return innerList.Count; } }
		bool ICollection<GroupFieldInfo>.IsReadOnly { get { return false; } }
		public bool Remove(GroupFieldInfo item) {
			GroupFieldInfoCollection oldValue = Clone();
			bool removed = innerList.Remove(item);
			if (removed) {
				Update();
				owner.OnSortingChanged(oldValue);
			}
			return removed;
		}
		public IEnumerator<GroupFieldInfo> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region IList members
		int IList.Add(object value) {
			Add((GroupFieldInfo)value);
			return innerList.Count - 1;
		}
		bool IList.Contains(object value) {
			return Contains((GroupFieldInfo)value);
		}
		int IList.IndexOf(object value) {
			return IndexOf((GroupFieldInfo)value);
		}
		void IList.Insert(int index, object value) {
			Insert(index, (GroupFieldInfo)value);
		}
		bool IList.IsFixedSize { get { return false; } }
		void IList.Remove(object value) {
			Remove((GroupFieldInfo)value);
		}
		object IList.this[int index] {
			get { return this[index]; }
			set { this[index] = (GroupFieldInfo)value; }
		}
		void ICollection.CopyTo(Array array, int index) {
			CopyTo((GroupFieldInfo[])array, index);
		}
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return this; } }
		bool IList.IsReadOnly { get { return false; } }
		#endregion
		#region ICloneable<GroupFieldInfoCollection> members
		public GroupFieldInfoCollection Clone() {
			GroupFieldInfoCollection result = new GroupFieldInfoCollection(this.owner);
			result.innerList.AddRange(this);
			return result;
		}
		#endregion
		public void AddRange(IEnumerable<GroupFieldInfo> collection) {
			GroupFieldInfoCollection oldValue = Clone();
			innerList.AddRange(collection);
			if (Count == oldValue.Count)
				return;
			Update();
			owner.OnSortingChanged(oldValue);
		}
	}
}
