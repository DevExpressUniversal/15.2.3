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
using System.ComponentModel;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Native {
	#region AppointmentIdHashCollection
	public class AppointmentIdHashCollection : Dictionary<object, RelevantDependenciesHashCollection> {
	}
	#endregion
	#region RelevantDependencyHashCollection
	public class RelevantDependenciesHashCollection : Dictionary<object, AppointmentDependency> {
	}
	#endregion
	#region DependencyHashCollection
	public class DependencyHashCollection {
		#region Fields
		AppointmentIdHashCollection idHash;
		#endregion
		#region Properties
		public int Count { get { return idHash.Count; } }
		protected internal Dictionary<object, RelevantDependenciesHashCollection>.KeyCollection Keys { get { return idHash.Keys; } }
		protected internal RelevantDependenciesHashCollection this[object key] { get { return idHash[key]; } set { idHash[key] = value; } }
		#endregion
		public DependencyHashCollection() {
			this.idHash = new AppointmentIdHashCollection();
		}
		public void Clear() {
			foreach (RelevantDependenciesHashCollection item in this.idHash.Values) {
				item.Clear();
			}
			this.idHash.Clear();
		}
		public bool ContainsKey(object key) {
			return this.idHash.ContainsKey(key);
		}
		public void Add(object key, RelevantDependenciesHashCollection value) {
			this.idHash.Add(key, value);
		}
		public void Add(object key, object relevantId, AppointmentDependency value) {
			if (!this.idHash.ContainsKey(key)) {
				RelevantDependenciesHashCollection collection = new RelevantDependenciesHashCollection();
				collection.Add(relevantId, value);
				Add(key, collection);
			} else {
				RelevantDependenciesHashCollection collection = this.idHash[key];
				if (!collection.ContainsKey(relevantId))
					collection.Add(relevantId, value);
			}
		}
		public void Remove(object key) {
			this.idHash.Remove(key);
		}
		public void Remove(object key, object relevantId) {
			if (key != null && this.idHash.ContainsKey(key)) {
				RelevantDependenciesHashCollection collection = this.idHash[key];
				if (relevantId != null && collection.ContainsKey(relevantId)) {
					collection.Remove(relevantId);
					if (collection.Count == 0)
						Remove(key);
				}
			}
		}
		protected internal virtual AppointmentDependencyBaseCollection GetDependenciesById(object id) {
			AppointmentDependencyBaseCollection result = new AppointmentDependencyBaseCollection();
			if (!IsExists(id))
				return result;			
			result.AddRange(this.idHash[id].Values);
			return result;
		}
		protected internal virtual bool IsExists(object keyId, object relevantId) {
			return keyId != null && relevantId != null ? idHash.ContainsKey(keyId) && idHash[keyId].ContainsKey(relevantId) : false;
		}
		protected internal virtual bool IsExists(object id) {
			return id != null ? idHash.ContainsKey(id) : false;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler {
	#region AppointmentDependencyBaseCollection
	public class AppointmentDependencyBaseCollection : NotificationCollection<AppointmentDependency> {
		#region Fields
		DependencyHashCollection parentIdHash = new DependencyHashCollection();
		DependencyHashCollection dependentIdHash = new DependencyHashCollection();
		#endregion
		#region Properties
		internal DependencyHashCollection ParentIdHash { get { return parentIdHash; } }
		internal DependencyHashCollection DependentIdHash { get { return dependentIdHash; } }
		#endregion
		public AppointmentDependencyBaseCollection GetDependenciesByParentId(object parentId) {
			return ParentIdHash.GetDependenciesById(parentId);
		}
		public AppointmentDependencyBaseCollection GetDependenciesByDependentId(object dependentId) {
			return DependentIdHash.GetDependenciesById(dependentId);
		}
		protected internal AppointmentDependencyBaseCollection GetAllDependenciesById(object id) {
			AppointmentDependencyBaseCollection result = new AppointmentDependencyBaseCollection();
			result.AddRange(GetDependenciesByParentId(id));
			result.AddRange(GetDependenciesByDependentId(id));
			return result;
		}
		protected internal virtual bool Contains(object id1, object id2) {
			if (AppointmentDependencyInstance.Empty != GetDependencyByIds(id1, id2))
				return true;
			if (AppointmentDependencyInstance.Empty != GetDependencyByIds(id2, id1))
				return true;
			return false;
		}
		protected override void OnInsertComplete(int index, AppointmentDependency value) {
			base.OnInsertComplete(index, value);
			if (AppointmentDependencyInstance.IsNullOrEmptyAppointmentId(value))
				return;
			ParentIdHash.Add(value.ParentId, value.DependentId, value);
			DependentIdHash.Add(value.DependentId, value.ParentId, value);
		}
		protected override void OnRemoveComplete(int index, AppointmentDependency value) {
			base.OnRemoveComplete(index, value);
			ParentIdHash.Remove(value.ParentId, value.DependentId);
			DependentIdHash.Remove(value.DependentId, value.ParentId);
		}
		public AppointmentDependency GetDependencyByIds(object parentId, object dependentId) {
			ICollection<AppointmentDependency> byParentId = GetDependenciesByParentId(parentId);
			foreach (AppointmentDependency item in byParentId)
				if (Object.Equals(item.DependentId, dependentId))
					return item;
			return AppointmentDependencyInstance.Empty;
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ParentIdHash.Clear();
			DependentIdHash.Clear();
		}
		internal AppointmentDependencyBaseCollection Clone() {
			AppointmentDependencyBaseCollection result = new AppointmentDependencyBaseCollection();
			result.AddRange(this);
			return result;
		}
	}
	#endregion
	#region AppointmentDependencyCollection
	public class AppointmentDependencyCollection : AppointmentDependencyBaseCollection {
		ISchedulerStorageBase storage;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentDependencyCollection() {
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		public AppointmentDependencyCollection(ISchedulerStorageBase storage) {
			if (storage == null)
				Exceptions.ThrowArgumentException("storage", storage);
			this.storage = storage;
			this.UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
#if !SL
	[DevExpressXtraSchedulerCoreLocalizedDescription("AppointmentDependencyCollectionStorage")]
#endif
		public ISchedulerStorageBase Storage { get { return storage; } }
		public virtual void ReadXml(string fileName) {
			CreateXmlConverter().ReadXml(fileName);
		}
		public virtual void ReadXml(Stream stream) {
			CreateXmlConverter().ReadXml(stream);
		}
		public virtual void WriteXml(string fileName) {
			CreateXmlConverter().WriteXml(fileName);
		}
		public virtual void WriteXml(Stream stream) {
			CreateXmlConverter().WriteXml(stream);
		}
		protected internal StorageXmlConverter<AppointmentDependency> CreateXmlConverter() {
			IAppointmentDependencyStorage objectStorage = storage.AppointmentDependencies;
			return new StorageXmlConverter<AppointmentDependency>(objectStorage, new AppointmentDependencyCollectionXmlPersistenceHelper(objectStorage));
		}
		protected internal void SetStorageCore(ISchedulerStorageBase storage) {
			this.storage = storage;
		}
	}
	#endregion
}
