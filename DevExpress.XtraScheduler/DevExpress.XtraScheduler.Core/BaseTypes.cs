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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraScheduler {
	public class CustomFieldCollection : IBatchUpdateable, IEnumerable {
		readonly NotificationCollection<CustomField> fields;
		public CustomFieldCollection() {
			this.fields = new NotificationCollection<CustomField>();
		}
		#region Properties
		protected internal NotificationCollection<CustomField> Fields { get { return fields; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("CustomFieldCollectionCount")]
#endif
public int Count { get { return this.Fields.Count; } }
		public object this[int index] {
			get {
				CustomField field = GetFieldByIndex(index);
				return field.Value;
			}
			set {
				CustomField field = GetFieldByIndex(index);
				AssignCustomFieldValue(field, value);
			}
		}
		public object this[string name] {
			get {
				CustomField field = GetFieldByName(name);
				if (field == null)
					return null;
				return field.Value;
			}
			set {
				CustomField field = GetFieldByName(name);
				if (field == null)
					return;
				AssignCustomFieldValue(field, value);
			}
		}
		#endregion
		#region IBatchUpdateable Members
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return ((IBatchUpdateable)Fields).BatchUpdateHelper; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("CustomFieldCollectionIsUpdateLocked")]
#endif
public bool IsUpdateLocked { get { return Fields.IsUpdateLocked; } }
		public void BeginUpdate() {
			Fields.BeginUpdate();
		}
		public void CancelUpdate() {
			Fields.CancelUpdate();
		}
		public void EndUpdate() {
			Fields.EndUpdate();
		}
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return Fields.GetEnumerator();
		}
		#endregion
		internal int Add(CustomField field) {
			return Fields.Add(field);
		}
		void AssignCustomFieldValue(CustomField field, object value) {
			if (Object.Equals(field.Value, value))
				return;
			CustomField oldValue = new CustomField(field.Name, field.Value);
			field.Value = value;
			CollectionChangingEventArgs<CustomField> args = new CollectionChangingEventArgs<CustomField>(CollectionChangedAction.Changed, field);
			args.PropertyName = field.Name;
			args.NewValue = field;
			args.OldValue = oldValue;
			DevExpress.Internal.DXNotificationCollectionAccessor.OnCollectionChanging(Fields, args);
			if (args.Cancel)
				field.Value = oldValue.Value;
			else
				DevExpress.Internal.DXNotificationCollectionAccessor.OnCollectionChanged(Fields, new CollectionChangedEventArgs<CustomField>(CollectionChangedAction.Changed, field));
		}
		internal CustomField GetFieldByName(string name) {
			int count = Count;
			for (int i = 0; i < count; i++) {
				CustomField field = Fields[i];
				if (field.Name == name)
					return field;
			}
			return null;
		}
		internal CustomField GetFieldByIndex(int index) {
			return Fields[index];
		}
		internal void CloneTo(CustomFieldCollection target) {
			target.Clear();
			int count = Count;
			for (int i = 0; i < count; i++) {
				CustomField source = Fields[i];
				CustomField field = new CustomField();
				field.Name = source.Name;
				field.Value = source.Value;
				target.Add(field);
			}
		}
		public void Clear() {
			Fields.Clear();
		}
		public void RemoveAt(int index) {
			Fields.RemoveAt(index);
		}
	}
	public interface IPersistentObject : IIdProvider, IBatchUpdateable, IDisposable {
		object Id { get; }
		object RowHandle { get; set; }
		bool IsDisposed { get; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		CustomFieldCollection CustomFields { get; }
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new bool IsUpdateLocked { get; }
		void Assign(IPersistentObject source);
		void Delete();
		object GetSourceObject(ISchedulerStorageBase storage);
		object GetRow(ISchedulerStorageBase storage);
		object GetValue(ISchedulerStorageBase storage, string columnName);
		void SetValue(ISchedulerStorageBase storage, string columnName, object val);
	}	
	#region PersistentObject
#pragma warning disable 618 // Obsolete
	public abstract class PersistentObject : IPersistentObject, IInternalPersistentObject, IBatchUpdateHandler, IXmlPersistable {
#pragma warning restore 618 // Obsolete
		#region Fields
		object id;
		object rowHandle = -1;
		BatchUpdateHelper batchUpdateHelper;
		CustomFieldCollection customFields;
		bool isDisposed;
		bool onContentChangingWasRaised;
		bool changeCancelled;
		#endregion
		#region Properties
		public virtual object Id {
			get { return id; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("You should use the 'CustomFields' instead.", true)]
		public object Tag { get { return null; } set { } }
		public object RowHandle { get { return rowHandle; } set { rowHandle = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CustomFieldCollection CustomFields { 
			get {
				if (IsDisposed)
					Exceptions.ThrowObjectDisposedException("PersistentObject", "CustomFields");
				return customFields; 
			} 
		}
		protected internal CustomFieldCollection InternalCustomFields { get { return customFields; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed { get { return isDisposed; } }
		internal bool OnContentChangingWasRaised { get { return onContentChangingWasRaised; } }
		public virtual bool DeferChangingToEndUpdate { get { return true; } }
		bool IInternalPersistentObject.ChangeCancelled { get { return changeCancelled; } set { changeCancelled = value; } }
		#endregion
		#region Events
		#region StateChanged
		PersistentObjectStateChangedEventHandler onStateChanged;
		event PersistentObjectStateChangedEventHandler IInternalPersistentObject.StateChanged { add { onStateChanged += value; } remove { onStateChanged -= value; } }
		void IInternalPersistentObject.RaiseStateChanged(IPersistentObject obj, PersistentObjectState state) {
			RaiseStateChanged(obj, state);
		}
		void IInternalPersistentObject.RaiseStateChanged(PersistentObjectState state) {
			RaiseStateChanged(state);
		}
		protected virtual void RaiseStateChanged(PersistentObjectState state) {
			if (onStateChanged != null) {
				PersistentObjectStateChangedEventArgs e = new PersistentObjectStateChangedEventArgs(this, state);
				onStateChanged(this, e);
			}
		}
		protected virtual void RaiseStateChanged(IPersistentObject obj, PersistentObjectState state) {
			if (onStateChanged != null) {
				PersistentObjectStateChangedEventArgs e = new PersistentObjectStateChangedEventArgs(obj, state);
				onStateChanged(this, e);
			}
		}
		#endregion
		#region StateChanging
		PersistentObjectStateChangingEventHandler onStateChanging;
		event PersistentObjectStateChangingEventHandler IInternalPersistentObject.StateChanging { add { onStateChanging += value; } remove { onStateChanging -= value; } }
		bool IInternalPersistentObject.RaiseStateChanging(IPersistentObject obj, PersistentObjectState state, string propertyName, object oldValue, object newValue) {
			return RaiseStateChanging(obj, state, propertyName, oldValue, newValue);
		}
		bool IInternalPersistentObject.RaiseStateChanging(PersistentObjectState state, string propertyName, object oldValue, object newValue) {
			return RaiseStateChanging(this, state, propertyName, oldValue, newValue);
		}
		protected bool RaiseStateChanging(PersistentObjectState state, string propertyName, object oldValue, object newValue) {
			return RaiseStateChanging(this, state, propertyName, oldValue, newValue);
		}
		public bool RaiseDeleting() {
			return RaiseStateChanging(this, PersistentObjectState.Deleted, String.Empty, null, null);
		}
		public bool RaiseChildDeleting(IPersistentObject obj) {
			return RaiseStateChanging(obj, PersistentObjectState.ChildDeleted, String.Empty, null, null);
		}
		protected virtual bool RaiseStateChanging(IPersistentObject obj, PersistentObjectState state, string propertyName, object oldValue, object newValue) {
			if (onStateChanging != null) {
				PersistentObjectStateChangingEventArgs e = new PersistentObjectStateChangingEventArgs(obj, state);
				e.PropertyName = propertyName;
				e.OldValue = oldValue;
				e.NewValue = newValue;
				onStateChanging(this, e);
				return !e.Cancel;
			}
			else
				return true;
		}
		#endregion
		#endregion
		protected PersistentObject() {
			InitBatchUpdateHelper();
			this.customFields = new CustomFieldCollection();
			this.customFields.Fields.CollectionChanging += new CollectionChangingEventHandler<CustomField>(OnCustomFieldsCollectionChanging);
			this.customFields.Fields.CollectionChanged += new CollectionChangedEventHandler<CustomField>(OnCustomFieldsCollectionChanged);
		}
		protected virtual void InitBatchUpdateHelper() {
			this.batchUpdateHelper = CreateBatchUpdateHelper();
		}
		protected virtual BatchUpdateHelper CreateBatchUpdateHelper() {
			return new BatchUpdateHelper(this);
		}
		protected virtual bool ShouldRecreateBatchUpdateHelper() {
			return false;
		}
		protected internal void RecreateBatchUpdateHelper() {
			batchUpdateHelper = CreateBatchUpdateHelper();
		}
		protected virtual void SaveObjectStateToBatchUpdateHelper() {
		}
		protected virtual bool CanReleaseBatchUpdateHelper() { return false; }
		protected virtual void ReleaseBatchUpdateHelper() {
			this.batchUpdateHelper = null;
		}
		void IIdProvider.SetId(object id) {
			this.id = id;
		}
		internal void SetId(object id) {
			((IIdProvider)this).SetId(id);
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.customFields != null) {
					this.customFields.Fields.CollectionChanged -= new CollectionChangedEventHandler<CustomField>(OnCustomFieldsCollectionChanged);
					this.customFields.Fields.CollectionChanging -= new CollectionChangingEventHandler<CustomField>(OnCustomFieldsCollectionChanging);
				}
				this.customFields = null;
				this.batchUpdateHelper = null;
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if (ShouldRecreateBatchUpdateHelper()) {
				RecreateBatchUpdateHelper();
				SaveObjectStateToBatchUpdateHelper();
			}
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
		public bool IsUpdateLocked { get { return batchUpdateHelper != null ? batchUpdateHelper.IsUpdateLocked : false; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdate();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
			OnBeginUpdate();
		}
		void IBatchUpdateHandler.OnEndUpdate() {
			OnEndUpdate();
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdate();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
			OnCancelUpdate();
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdate();
		}
		protected virtual void OnFirstBeginUpdate() {
			onContentChangingWasRaised = false;
			changeCancelled = false;
		}
		protected virtual void OnBeginUpdate() {
			customFields.BeginUpdate();
		}
		protected virtual void OnEndUpdate() {
			customFields.EndUpdate();
		}
		protected virtual void OnLastEndUpdate() {
			if ( ((IInternalPersistentObject)this).DeferChangingToEndUpdate ) {
				if (onContentChangingWasRaised) {
					if (OnContentChanging(String.Empty, String.Empty, String.Empty))
						OnContentChanged();
					else
						((IInternalPersistentObject)this).RollbackObjectState(this);
				}
			}
			else {
				if (onContentChangingWasRaised && !changeCancelled)
					OnContentChanged();
			}
			if (CanReleaseBatchUpdateHelper())
				ReleaseBatchUpdateHelper();
		}
		protected virtual void OnCancelUpdate() {
			customFields.CancelUpdate();
		}
		protected virtual void OnLastCancelUpdate() {
			if (CanReleaseBatchUpdateHelper())
				ReleaseBatchUpdateHelper();
		}
		#endregion
		void OnCustomFieldsCollectionChanging(object sender, CollectionChangingEventArgs<CustomField> e) {
			object oldValue = e.OldValue != null ? e.OldValue.Value : null;
			object newValue = e.NewValue != null ? e.NewValue.Value : null;
			e.Cancel = !OnContentChanging(e.PropertyName, oldValue, newValue);
		}
		void OnCustomFieldsCollectionChanged(object sender, CollectionChangedEventArgs<CustomField> e) {
			OnContentChanged();
		}
		public virtual void OnContentChanged() {
			if (!IsUpdateLocked)
				RaiseStateChanged(PersistentObjectState.Changed);
		}
		public virtual bool OnContentChanging(string propertyName, object oldValue, object newValue) {
			if ( IsUpdateLocked ) {
				if ( ((IInternalPersistentObject)this).DeferChangingToEndUpdate ) {
					onContentChangingWasRaised = true;
					return true;
				}
				else {
					if ( !onContentChangingWasRaised ) {
						if ( !((IInternalPersistentObject)this).RaiseStateChanging(PersistentObjectState.Changed, propertyName, oldValue, newValue) )
							changeCancelled = true;
						onContentChangingWasRaised = true;
					}
					return !changeCancelled;
				}
			}
			return ((IInternalPersistentObject)this).RaiseStateChanging(PersistentObjectState.Changed, propertyName, oldValue, newValue);
		}
		public virtual void RollbackObjectState(IPersistentObject objectToRollback) {
			((IInternalPersistentObject)this).RaiseStateChanged(objectToRollback, PersistentObjectState.RollbackState);
		}
		public virtual bool CanDelete() {
			return ((IInternalPersistentObject)this).RaiseDeleting();
		}
		public virtual void DeleteCore() {
			RaiseStateChanged(PersistentObjectState.Deleted);
		}
		public virtual void Delete() {
			if ( ((IInternalPersistentObject)this).CanDelete() )
				DeleteCore();
		}
		public virtual object GetSourceObject(ISchedulerStorageBase storage) {
			return GetRow(storage);
		}
		public abstract object GetRow(ISchedulerStorageBase storage);
		public abstract object GetValue(ISchedulerStorageBase storage, string columnName);
		public abstract void SetValue(ISchedulerStorageBase storage, string columnName, object val);
		protected internal abstract XmlPersistenceHelper CreateXmlPersistenceHelper();
		[Obsolete("You should use the DevExpress.XtraScheduler.Xml.XmlPersistenceHelper.ToXml method instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ToXml() {
			return CreateXmlPersistenceHelper().ToXml();
		}
		[Obsolete("You should use the DevExpress.XtraScheduler.Xml.XmlPersistenceHelper.FromXml method instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void FromXml(string val) {
			CreateXmlPersistenceHelper().FromXml(val);
		}
		public virtual void Assign(IPersistentObject source) {
			if (source != null)
				source.CustomFields.CloneTo(CustomFields);
		}	   
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	#region PersistentObjectState
	public enum PersistentObjectState {
		ChildCreated,
		Changed,
		Deleted,
		ChildDeleted,
		RollbackState
	};
	#endregion
	#region CustomField
	public class CustomField {
		object val;
		string name = String.Empty;
		protected internal CustomField() {
		}
		public CustomField(string name, object val) {
			this.name = name;
			this.val = val;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("CustomFieldValue")]
#endif
		public object Value { get { return val; } set { val = value; } }
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("CustomFieldName")]
#endif
		public string Name { get { return name; } set { name = value; } }
	}
	#endregion    
}
namespace DevExpress.XtraScheduler.Internal {
	public interface IInternalPersistentObject {
		bool ChangeCancelled { get; set; }
		bool DeferChangingToEndUpdate { get; }
		bool CanDelete();
		void DeleteCore();
		bool OnContentChanging(string propertyName, object oldValue, object newValue);
		void OnContentChanged();
		bool RaiseChildDeleting(IPersistentObject obj);
		bool RaiseDeleting();
		bool RaiseStateChanging(PersistentObjectState state, string propertyName, object oldValue, object newValue);
		bool RaiseStateChanging(IPersistentObject obj, PersistentObjectState state, string propertyName, object oldValue, object newValue);
		void RaiseStateChanged(IPersistentObject obj, PersistentObjectState state);
		void RaiseStateChanged(PersistentObjectState state);
		void RollbackObjectState(IPersistentObject objectToRollback);
		event PersistentObjectStateChangedEventHandler StateChanged;
		event PersistentObjectStateChangingEventHandler StateChanging;
	}
}
