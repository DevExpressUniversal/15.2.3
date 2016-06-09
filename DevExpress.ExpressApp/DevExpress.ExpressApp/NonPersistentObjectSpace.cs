#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public class NonPersistentObjectSpace : BaseObjectSpace {
		private HashSet<INotifyPropertyChanged> objects;
		private HashSet<Object> newObjects;
		private HashSet<Object> deletedObjects;
		private HashSet<Object> modifiedObjects;
		protected override IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			IList result = null;
			if(ObjectsGetting != null) {
				ObjectsGettingEventArgs eventArgs = new ObjectsGettingEventArgs(objectType, criteria, sorting, inTransaction);
				ObjectsGetting(this, eventArgs);
#pragma warning disable 0618
				if(eventArgs.Handled) {
#pragma warning restore 0618
					result = eventArgs.Objects;
				}
			}
			if(result == null) {
				result = new BindingList<Object>();
			}
			foreach(Object obj in result) {
				if(obj is INotifyPropertyChanged) {
					((INotifyPropertyChanged)obj).PropertyChanged -= Object_PropertyChanged;
					((INotifyPropertyChanged)obj).PropertyChanged += Object_PropertyChanged;
					objects.Add((INotifyPropertyChanged)obj);
				}
				if((obj is IObjectSpaceLink) && (((IObjectSpaceLink)obj).ObjectSpace == null)) {
					((IObjectSpaceLink)obj).ObjectSpace = this;
				}
			}
			return result;
		}
		protected override IList<T> CreateCollection<T>(CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			IList<T> result = null;
			IList collection = CreateCollection(typeof(T), criteria, sorting, inTransaction);
			if(collection is IList<T>) {
				result = (IList<T>)collection;
			}
			else if(collection != null) {
				result = collection.OfType<T>().ToList<T>();
			}
			else {
				result = new BindingList<T>();
			}
			return result;
		}
		protected override Object CreateObjectCore(Type type) {
			Object obj = TypeHelper.CreateInstance(type);
			if(obj is INotifyPropertyChanged) {
				((INotifyPropertyChanged)obj).PropertyChanged -= Object_PropertyChanged;
				((INotifyPropertyChanged)obj).PropertyChanged += Object_PropertyChanged;
				objects.Add((INotifyPropertyChanged)obj);
			}
			newObjects.Add(obj);
			modifiedObjects.Add(obj);
			SetIsModified(true);
			return obj;
		}
		protected override void DeleteCore(IList objects) {
			if(objects.Count > 0) {
				OnObjectDeleting(objects);
			}
			foreach(Object obj in objects) {
				deletedObjects.Add(obj);
				modifiedObjects.Add(obj);
			}
			if(objects.Count > 0) {
				OnObjectDeleted(objects);
			}
		}
		protected override void DoCommit() {
			base.DoCommit();
			newObjects.Clear();
			deletedObjects.Clear();
			modifiedObjects.Clear();
		}
		protected override void Reload() {
			isReloading = true;
			SetIsModified(false);
			newObjects.Clear();
			deletedObjects.Clear();
			modifiedObjects.Clear();
			foreach(INotifyPropertyChanged obj in objects) {
				obj.PropertyChanged -= Object_PropertyChanged;
			}
			objects.Clear();
			isReloading = false;
			OnReloaded();
		}
		protected override void SetModified(Object obj, ObjectChangedEventArgs args) {
			if(obj != null) {
				if(IsKnownType(obj.GetType())) {
					modifiedObjects.Add(obj);
					OnObjectChanged(args);
					SetIsModified(true);
				}
				else {
					OnObjectChanged(args);
				}
			}
			else {
				SetIsModified(true);
			}
		}
		public NonPersistentObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore)
			: base(typesInfo, entityStore) {
			objects = new HashSet<INotifyPropertyChanged>();
			newObjects = new HashSet<Object>();
			deletedObjects = new HashSet<Object>();
			modifiedObjects = new HashSet<Object>();
		}
		public NonPersistentObjectSpace(ITypesInfo typesInfo)
			: this(typesInfo, null) {
		}
		public override void Dispose() {
			base.Dispose();
			ObjectsGetting = null;
			if(objects != null) {
				foreach(INotifyPropertyChanged obj in objects) {
					obj.PropertyChanged -= Object_PropertyChanged;
				}
				objects.Clear();
				objects = null;
			}
			if(newObjects != null) {
				newObjects.Clear();
				newObjects = null;
			}
			if(deletedObjects != null) {
				deletedObjects.Clear();
				deletedObjects = null;
			}
			if(modifiedObjects != null) {
				modifiedObjects.Clear();
				modifiedObjects = null;
			}
			isDisposed = true;
		}
		public override IList ModifiedObjects {
			get { return modifiedObjects.ToList(); }
		}
		public event EventHandler<ObjectsGettingEventArgs> ObjectsGetting;
		public event EventHandler<ObjectGettingEventArgs> ObjectGetting;
		public override Boolean Contains(Object obj) {
			return true;
		}
		public override IObjectSpace CreateNestedObjectSpace() {
			return new NonPersistentObjectSpace(typesInfo);
		}
		public override Object GetObject(Object objectFromDifferentObjectSpace) {
			Object result = objectFromDifferentObjectSpace;
			if(ObjectGetting != null) {
				ObjectGettingEventArgs eventArgs = new ObjectGettingEventArgs(objectFromDifferentObjectSpace, objectFromDifferentObjectSpace);
				ObjectGetting(this, eventArgs);
				result = eventArgs.TargetObject;
			}
			if(result is INotifyPropertyChanged) {
				((INotifyPropertyChanged)result).PropertyChanged -= Object_PropertyChanged;
				((INotifyPropertyChanged)result).PropertyChanged += Object_PropertyChanged;
				objects.Add((INotifyPropertyChanged)result);
			}
			if((result is IObjectSpaceLink) && (((IObjectSpaceLink)result).ObjectSpace == null)) {
				((IObjectSpaceLink)result).ObjectSpace = this;
			}
			return result;
		}
		public override Int32 GetObjectsCount(Type objectType, CriteriaOperator criteria) {
			Int32 result = 0;
			return result;
		}
		public override IQueryable<T> GetObjectsQuery<T>(Boolean inTransaction = false) {
			return CreateCollection<T>(null, null, false).AsQueryable<T>();
		}
		public override Boolean IsCollectionLoaded(Object collection) {
			return true;
		}
		public override Boolean IsDeletedObject(Object obj) {
			return deletedObjects.Contains(obj);
		}
		public override Boolean IsNewObject(Object obj) {
			return newObjects.Contains(obj);
		}
	}
}
