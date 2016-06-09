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
using System.Drawing;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Native {
	#region PersistentObjectStorageFilteredComponentImplementation<T>
	public abstract class PersistentObjectStorageFilteredComponentImplementation<T> : IFilteredComponent where T : IPersistentObject {
		IPersistentObjectStorage<T> objectStorage;
		protected PersistentObjectStorageFilteredComponentImplementation(IPersistentObjectStorage<T> objectStorage) {
			if (objectStorage == null)
				Exceptions.ThrowArgumentNullException("objectStorage");
			this.objectStorage = objectStorage;
		}
		public IPersistentObjectStorage<T> ObjectStorage { get { return objectStorage; } }
		internal ISchedulerStorageBase Storage { get { return ((IInternalPersistentObjectStorage<T>)objectStorage).Storage; } }
		#region IFilteredComponent Members
		#region Events
		public event EventHandler PropertiesChanged {
			add { ((IInternalPersistentObjectStorage<T>)ObjectStorage).MappingsChanged += value; }
			remove { ((IInternalPersistentObjectStorage<T>)ObjectStorage).MappingsChanged -= value; }
		}
		public event EventHandler RowFilterChanged {
			add { ((IInternalPersistentObjectStorage<T>)ObjectStorage).FilterChanged += value; }
			remove { ((IInternalPersistentObjectStorage<T>)ObjectStorage).FilterChanged -= value; }
		}
		#endregion
		public CriteriaOperator RowCriteria {
			get {
				return CriteriaOperator.Parse(ObjectStorage.Filter);
			}
			set {
				string newValue = String.Empty;
				if (!Object.ReferenceEquals(value, null))
					newValue = value.ToString();
				if (ObjectStorage.Filter == newValue)
					return;
				ObjectStorage.Filter = newValue;
			}
		}
		public virtual IBoundPropertyCollection CreateFilterColumnCollection() {
			FilterColumnCollection columns = new FilterColumnCollection();
			PopulateFilterColumns(columns);
			return columns;
		}
		protected internal virtual void PopulateFilterColumns(FilterColumnCollection columns) {
			MappingCollection mappings = ((IInternalPersistentObjectStorage<T>)objectStorage).ActualMappings;
			int count = mappings.Count;
			for (int i = 0; i < count; i++) {
				MappingBase mapping = mappings[i];
				if (CanCreateColumnMapping(mapping)) {
					FilterColumn filterColumn = CreateFilterColumn(mapping);
					bool canAddColumn = PrepareFilterColumn(filterColumn);
					if (canAddColumn)
						columns.Add(filterColumn);
				}
			}
		}
		#endregion
		protected internal abstract MappingFilterColumn CreateFilterColumn(MappingBase mapping);
		protected internal abstract bool CanCreateColumnMapping(MappingBase mapping);
		protected internal abstract bool PrepareFilterColumn(FilterColumn filterColumn);
	}
	#endregion
	#region AppointmentStorageFilteredComponentImplementation
	public class AppointmentStorageFilteredComponentImplementation : PersistentObjectStorageFilteredComponentImplementation<Appointment> {
		public AppointmentStorageFilteredComponentImplementation(AppointmentStorage storage)
			: base(storage) {
		}
		#region CreateFilterColumn
		protected internal override MappingFilterColumn CreateFilterColumn(MappingBase mapping) {
			return new AppointmentMappingFilterColumn(mapping, (AppointmentStorage)ObjectStorage);
		}
		#endregion
		#region CanCreateColumnMapping
		protected internal override bool CanCreateColumnMapping(MappingBase mapping) {
			if (mapping.Name == AppointmentSR.Type)
				return false;
			if (mapping.Name == AppointmentSR.RecurrenceInfo)
				return false;
			if (mapping.Name == AppointmentSR.ReminderInfo)
				return false;
			return true;
		}
		#endregion
		#region PrepareFilterColumn
		protected internal override bool PrepareFilterColumn(FilterColumn filterColumn) {
			bool canAddFilterColumn = !((IInternalSchedulerStorage)Storage).RaisePrepareAppointmentFilterColumn(filterColumn);
			return canAddFilterColumn;
		}
		#endregion
	}
	#endregion
	#region ResourceStorageFilteredComponentImplementation
	public class ResourceStorageFilteredComponentImplementation : PersistentObjectStorageFilteredComponentImplementation<Resource> {
		public ResourceStorageFilteredComponentImplementation(ResourceStorageBase storage)
			: base(storage) {
		}
		#region CreateFilterColumn
		protected internal override MappingFilterColumn CreateFilterColumn(MappingBase mapping) {
			return new ResourceMappingFilterColumn(mapping);
		}
		#endregion
		#region CanCreateColumnMapping
		protected internal override bool CanCreateColumnMapping(MappingBase mapping) {
			if (mapping.Name == ResourceSR.Image)
				return false;
			return true;
		}
		#endregion
		#region PrepareFilterColumn
		protected internal override bool PrepareFilterColumn(FilterColumn filterColumn) {
			bool canAddFilterColumn = !((IInternalSchedulerStorage)Storage).RaisePrepareResourceFilterColumn(filterColumn);
			return canAddFilterColumn;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.UI {
	#region MappingFilterColumn
	public abstract class MappingFilterColumn : FilterColumn {
		#region Fields
		MappingBase mapping;
		RepositoryItem resolvedEditor;
		string columnCaption;
		#endregion
		protected MappingFilterColumn(MappingBase mapping) {
			if (mapping == null)
				Exceptions.ThrowArgumentNullException("mapping");
			this.mapping = mapping;
			this.SetColumnCaption(mapping.Name);
		}
		#region Properties
		public override FilterColumnClauseClass ClauseClass {
			get {
				Type mappingType = mapping.Type;
				if (mappingType == typeof(String))
					return FilterColumnClauseClass.String;
				if (typeof(Array).IsAssignableFrom(mappingType))
					return FilterColumnClauseClass.Blob;
				if (typeof(Image).IsAssignableFrom(mappingType))
					return FilterColumnClauseClass.Blob;
				return FilterColumnClauseClass.Generic;
			}
		}
		public override RepositoryItem ColumnEditor {
			get {
				if (resolvedEditor == null)
					resolvedEditor = CreateRepository();
				return resolvedEditor;
			}
		}
		public override string ColumnCaption { get { return columnCaption; } }
		public override Type ColumnType { get { return mapping.Type; } }
		public override string FieldName { get { return mapping.Name; } }
		public override Image Image { get { return null; } }
		protected MappingBase Mapping { get { return mapping; } }
		#endregion
		#region CreateRepository
		protected internal virtual RepositoryItem CreateRepository() {
			if (ColumnType == typeof(Boolean))
				return new RepositoryItemCheckEdit();
			if (ColumnType == typeof(DateTime))
				return new RepositoryItemDateEdit();
			if (ColumnType == typeof(int))
				return new RepositoryItemSpinEdit();
			return new RepositoryItemTextEdit();
		}
		#endregion
		#region Dispose
		public override void Dispose() {
			base.Dispose();
			if (resolvedEditor != null) {
				resolvedEditor.Dispose();
				resolvedEditor = null;
			}
		}
		#endregion
		#region SetColumnCaption
		public override void SetColumnCaption(string caption) {
			base.SetColumnCaption(caption);
			this.columnCaption = caption;
		}
		#endregion
		#region SetColumnEditor
		public override void SetColumnEditor(RepositoryItem item) {
			base.SetColumnEditor(item);
			this.resolvedEditor = item;
		}
		#endregion
	}
	#endregion
	#region AppointmentMappingFilterColumn
	public class AppointmentMappingFilterColumn : MappingFilterColumn {
		AppointmentStorage storage;
		public AppointmentMappingFilterColumn(MappingBase mapping, AppointmentStorage storage)
			: base(mapping) {
			if (storage == null)
				Exceptions.ThrowArgumentNullException("storage");
			this.storage = storage;
		}
		public AppointmentStorage Storage { get { return storage; } }
		#region CreateRepository
		protected internal override RepositoryItem CreateRepository() {
			if (FieldName == AppointmentSR.Status)
				return CreateStatusMappingFilterColumn();
			if (FieldName == AppointmentSR.Label)
				return CreateLabelMappingFilterColumn();
			if (FieldName == AppointmentSR.ResourceId)
				return CreateResourceMappingFilterColumn();
			return base.CreateRepository();
		}
		#endregion
		#region CreateStatusMappingFilterColumn
		protected internal virtual RepositoryItem CreateStatusMappingFilterColumn() {
			RepositoryItemImageComboBox item = new RepositoryItemImageComboBox();
			RepositoryImageComboBoxFillHelper<IAppointmentStatus> fillHelper = new RepositoryImageComboBoxFillHelper<IAppointmentStatus>();
			fillHelper.FillComboBoxWithIndexValues(item, storage.Statuses);
			return item;
		}
		#endregion
		#region CreateLabelMappingFilterColumn
		protected internal virtual RepositoryItem CreateLabelMappingFilterColumn() {
			RepositoryItemImageComboBox item = new RepositoryItemImageComboBox();
			RepositoryImageComboBoxFillHelper<IAppointmentLabel> fillHelper = new RepositoryImageComboBoxFillHelper<IAppointmentLabel>();
			fillHelper.FillComboBoxWithIndexValues(item, storage.Labels);
			return item;
		}
		#endregion
		#region CreateResourceMappingFilterColumn
		protected internal virtual RepositoryItem CreateResourceMappingFilterColumn() {
			RepositoryItemImageComboBox item = new RepositoryItemImageComboBox();
			ResourceBaseCollection resources = ((IInternalSchedulerStorage)Storage.Storage).GetNonFilteredResourcesCore();
			int count = resources.Count;
			for (int i = 0; i < count; i++) {
				Resource resource = resources[i];
				item.Items.Add(new ImageComboBoxItem(resource.Caption, resource.Id, -1));
			}
			return item;
		}
		#endregion
	}
	#endregion
	#region ResourceMappingFilterColumn
	public class ResourceMappingFilterColumn : MappingFilterColumn {
		public ResourceMappingFilterColumn(MappingBase mapping)
			: base(mapping) {
		}
		#region CreateRepository
		protected internal override RepositoryItem CreateRepository() {
			if (FieldName == ResourceSR.Color)
				return CreateColorMappingFilterColumn();
			return base.CreateRepository();
		}
		#endregion
		#region CreateColorMappingFilterColumn
		protected internal virtual RepositoryItem CreateColorMappingFilterColumn() {
			RepositoryItemColorEdit item = new RepositoryItemColorEdit();
			item.StoreColorAsInteger = true;
			item.TextEditStyle = TextEditStyles.DisableTextEditor;
			return item;
		}
		#endregion
	}
	#endregion
}
