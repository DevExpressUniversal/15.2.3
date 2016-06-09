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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public interface IObjectSpace : IDisposable {
		String Database { get; }
		Boolean IsCommitting { get; }
		Boolean IsConnected { get; }
		Boolean IsDeleting { get; }
		Boolean IsDisposed { get; }
		Boolean IsModified { get; }
		Boolean IsReloading { get; }
		IList ModifiedObjects { get; }
		Object Owner { get; set; }
		ITypesInfo TypesInfo { get; }
		void ApplyCriteria(Object collection, CriteriaOperator criteria);
		void ApplyFilter(Object collection, CriteriaOperator filter);
		Boolean CanApplyCriteria(Type collectionType);
		Boolean CanApplyFilter(Object collection);
		Boolean CanInstantiate(Type type);
		void CommitChanges();
		Boolean Contains(Object obj);
		IList CreateCollection(Type objectType);
		IList CreateCollection(Type objectType, CriteriaOperator criteria);
		IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting);
		IList CreateDataView(Type objectType, String expressions, CriteriaOperator criteria, IList<SortProperty> sorting);
		IList CreateDataView(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting);
		IObjectSpace CreateNestedObjectSpace();
		Object CreateObject(Type type);
		ObjectType CreateObject<ObjectType>();
		IDisposable CreateParseCriteriaScope();
		Object CreateServerCollection(Type objectType, CriteriaOperator criteria);
		void Delete(Object obj);
		void Delete(IList objects);
		void EnableObjectDeletionOnRemove(Object collection, Boolean enable);
		Object Evaluate(Type objectType, CriteriaOperator expression, CriteriaOperator criteria);
		Object FindObject(Type objectType, CriteriaOperator criteria);
		ObjectType FindObject<ObjectType>(CriteriaOperator criteria);
		Object FindObject(Type objectType, CriteriaOperator criteria, Boolean inTransaction);
		ObjectType FindObject<ObjectType>(CriteriaOperator criteria, Boolean inTransaction);
		CriteriaOperator GetAssociatedCollectionCriteria(Object obj, IMemberInfo memberInfo);
		CriteriaOperator GetCriteria(Object collection);
		IList<SortProperty> GetCollectionSorting(Object collection);
		String GetDisplayableProperties(Object collection);
		EvaluatorContextDescriptor GetEvaluatorContextDescriptor(Type type);
		ExpressionEvaluator GetExpressionEvaluator(Type type, CriteriaOperator criteria);
		ExpressionEvaluator GetExpressionEvaluator(EvaluatorContextDescriptor evaluatorContextDescriptor, CriteriaOperator criteria);
		CriteriaOperator GetFilter(Object collection);
		String GetKeyPropertyName(Type type);
		Type GetKeyPropertyType(Type type);
		Object GetKeyValue(Object obj);
		String GetKeyValueAsString(Object obj);
		Object GetObject(Object obj);
		ObjectType GetObject<ObjectType>(ObjectType obj);
		Object GetObjectByHandle(String handle);
		Object GetObjectByKey(Type type, Object key);
		ObjectType GetObjectByKey<ObjectType>(Object key);
		String GetObjectHandle(Object obj);
		Object GetObjectKey(Type objectType, String objectKeyString);
		IList GetObjects(Type type);
		IList<T> GetObjects<T>();
		IList GetObjects(Type type, CriteriaOperator criteria);
		IList<T> GetObjects<T>(CriteriaOperator criteria);
		IList GetObjects(Type type, CriteriaOperator criteria, Boolean inTransaction);
		IList<T> GetObjects<T>(CriteriaOperator criteria, Boolean inTransaction);
		Int32 GetObjectsCount(Type objectType, CriteriaOperator criteria);
		IQueryable<T> GetObjectsQuery<T>(Boolean inTransaction = false);
		ICollection GetObjectsToDelete(Boolean includeParent);
		ICollection GetObjectsToSave(Boolean includeParent);
		Type GetObjectType(Object obj);
		Int32 GetTopReturnedObjectsCount(Object collection);
		Boolean IsCollectionLoaded(Object collection);
		Boolean IsDeletedObject(Object obj);
		Boolean IsDeletionDeferredType(Type type);
		Boolean IsDisposedObject(Object obj);
		Boolean IsKnownType(Type type);
		Boolean IsNewObject(Object obj);
		Boolean IsObjectDeletionOnRemoveEnabled(Object collection);
		Boolean? IsObjectFitForCriteria(Type objectType, Object obj, CriteriaOperator criteria);
		Boolean? IsObjectFitForCriteria(Object obj, CriteriaOperator criteria);
		Boolean IsObjectToDelete(Object obj);
		Boolean IsObjectToSave(Object obj);
		CriteriaOperator ParseCriteria(String criteria);
		void SetModified(Object obj, IMemberInfo memberInfo);
		void SetModified(Object obj);
		Boolean Refresh();
		void ReloadCollection(Object collection);
		Object ReloadObject(Object obj);
		void RemoveFromModifiedObjects(Object obj);
		Boolean Rollback();
		void SetCollectionSorting(Object collection, IList<SortProperty> sorting);
		void SetDisplayableProperties(Object collection, String displayableProperties);
		void SetTopReturnedObjectsCount(Object collection, Int32 topReturnedObjects);
		event EventHandler<CancelEventArgs> Committing;
		event EventHandler Committed;
		event EventHandler<ConfirmationEventArgs> ConfirmationRequired;
		event EventHandler Connected;
		event EventHandler<HandledEventArgs> CustomCommitChanges;
		event EventHandler<CustomDeleteObjectsEventArgs> CustomDeleteObjects;
		event EventHandler<HandledEventArgs> CustomRefresh;
		event EventHandler<HandledEventArgs> CustomRollBack;
		event EventHandler Disposed;
		event EventHandler ModifiedChanged;
		event EventHandler<ObjectChangedEventArgs> ObjectChanged;
		event EventHandler<ObjectsManipulatingEventArgs> ObjectDeleting;
		event EventHandler<ObjectsManipulatingEventArgs> ObjectDeleted;
		event EventHandler<ObjectManipulatingEventArgs> ObjectEndEdit;
		event EventHandler<ObjectManipulatingEventArgs> ObjectReloaded;
		event EventHandler<ObjectManipulatingEventArgs> ObjectSaving;
		event EventHandler<ObjectManipulatingEventArgs> ObjectSaved;
		event EventHandler<CancelEventArgs> Refreshing;
		event EventHandler Reloaded;
		event EventHandler<CancelEventArgs> RollingBack;
	}
	public interface INestedObjectSpace : IObjectSpace {
		IObjectSpace ParentObjectSpace { get; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IAsyncServerDataSourceCreator {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		Object Create(Type objectType, CriteriaOperator criteria);
	}
	public interface IObjectSpaceLink {
		IObjectSpace ObjectSpace { get; set; }
	}
	public interface IObjectSpaceLinks {
		IList<IObjectSpace> ObjectSpaces { get; }
	}
}
