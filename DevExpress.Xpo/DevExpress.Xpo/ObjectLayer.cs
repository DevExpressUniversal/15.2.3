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
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Generators;
using DevExpress.Xpo.Metadata.Helpers;
using System.Threading;
using System.Data;
using System.Resources;
using System.Reflection;
using System.ComponentModel;
namespace DevExpress.Xpo.Helpers {
	public interface IObjectLayerProvider : DevExpress.Xpo.Metadata.Helpers.IXPDictionaryProvider {
		IObjectLayer ObjectLayer { get; }
	}
	public interface IObjectLayerForTests : IObjectLayer {
		void ClearDatabase();
	}
}
namespace DevExpress.Xpo {
	public interface IObjectLayer: IObjectLayerProvider  {
		ICollection[] LoadObjects(Session session, ObjectsQuery[] queries);
		object LoadObjectsAsync(Session session, ObjectsQuery[] queries, AsyncLoadObjectsCallback callback);
		List<object[]> SelectData(Session session, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria);
		object SelectDataAsync(Session session, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, AsyncSelectDataCallback callback);
		ICollection[] GetObjectsByKey(Session session, ObjectsByKeyQuery[] queries);
		void CommitChanges(Session session, ICollection fullListForDelete, ICollection completeListForSave);
		object CommitChangesAsync(Session session, ICollection fullListForDelete, ICollection completeListForSave, AsyncCommitCallback callback);
		PurgeResult Purge();
		void SetObjectLayerWideObjectTypes(Dictionary<XPClassInfo, XPObjectType> loadedTypes);
		void CreateObjectType(XPObjectType objectType);
		Dictionary<XPClassInfo, XPObjectType> GetObjectLayerWideObjectTypes();
		void RegisterStaticTypes(params XPClassInfo[] types);
		bool IsStaticType(XPClassInfo type);
		IObjectMap GetStaticCache(XPClassInfo info);
		bool CanLoadCollectionObjects { get; }
		object[] LoadCollectionObjects(Session session, XPMemberInfo refProperty, object ownerObject);
	}
	public interface IObjectLayerEx: IDataLayerProvider {
		UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types);
#if !SL
		event SchemaInitEventHandler SchemaInit;
		IDbConnection Connection { get;}
#if DEBUGTEST
		IDbCommand CreateCommand();
#endif
#endif
		object[] LoadDelayedProperties(Session session, object theObject, MemberPathCollection props);
		ObjectDictionary<object> LoadDelayedProperties(Session session, IList objects, XPMemberInfo property);
		AutoCreateOption AutoCreateOption { get; }
	}
	public interface IObjectLayerOnSession {
		bool IsParentObjectToSave(Session session, object theObject);
		bool IsParentObjectToDelete(Session session, object theObject);
		ICollection GetParentObjectsToSave(Session session);
		ICollection GetParentObjectsToDelete(Session session);
		ICollection GetParentTouchedClassInfos(Session session);
	}
	public class ObjectsByKeyQuery {
		XPClassInfo classInfo;
		ICollection idCollection;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsByKeyQueryClassInfo")]
#endif
public XPClassInfo ClassInfo { get { return classInfo; } }
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsByKeyQueryIdCollection")]
#endif
public ICollection IdCollection { get { return idCollection; } }
		public ObjectsByKeyQuery(XPClassInfo classInfo, ICollection idCollection) {
			this.classInfo = classInfo;
			this.idCollection = idCollection;
		}
	}
	public enum CommitChangesMode {
		None = 0,
		InTransaction,
		NotInTransactionUnitOfWork,
		NotInTransactionSession
	}
	public delegate void AsyncSelectDataCallback(List<object[]> result, Exception ex);
	public delegate void AsyncCommitCallback(Exception ex);
	public class SimpleObjectLayer : 
#if !SL
		  IObjectLayerForTests
#else
		  IObjectLayer
#endif
		  , IObjectLayerEx, ICommandChannel
		{
		readonly IDataLayer dataLayer;
		readonly ICommandChannel nestedCommandChannel;
		public SimpleObjectLayer(IDataLayer dataLayer){
			this.dataLayer = dataLayer;
			this.nestedCommandChannel = dataLayer as ICommandChannel;
		}
		public static SimpleObjectLayer FromDataLayer(IDataLayer dataLayer){
			return dataLayer == null ? null : new SimpleObjectLayer(dataLayer);
		}
		public ICollection[] LoadObjects(Session session, ObjectsQuery[] queries) {
			ICollection[] rv = new ObjectCollectionLoader(session, dataLayer).LoadObjects(queries);
			SessionIdentityMap.Extract(session).Compact();
			return rv;
		}
		public object LoadObjectsAsync(Session session, ObjectsQuery[] queries, AsyncLoadObjectsCallback callback) {
			if(SynchronizationContext.Current == null) throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null) throw new ArgumentNullException();
			return new ObjectCollectionLoader(session, dataLayer).LoadObjectsAsync(queries, callback, SynchronizationContext.Current);
		}
		public List<object[]> SelectData(Session session, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			return SelectDataInternal(dataLayer, query, properties, groupProperties, groupCriteria);
		}
		internal static List<object[]> SelectDataInternal(IDataLayer dataLayer, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			QueryData queryData = new QueryData(query.ClassInfo, properties);
			SelectStatement root = ClientSelectSqlGenerator.GenerateSelect(query.ClassInfo, query.Criteria,
				properties, query.Sorting, groupProperties, groupCriteria, query.CollectionCriteriaPatcher, query.SkipSelectedRecords, query.TopSelectedRecords);
			SelectStatementResult result = dataLayer.SelectData(new SelectStatement[] { root }).ResultSet[0];
			queryData.SetData(result);
			List<object[]> res = new List<object[]>(queryData.Count);
			bool reuseArray = result.Rows.Length > 0 && properties.Count == result.Rows[0].Values.Length;
			int pos = 0;
			while(queryData.MoveNext()) {
				object[] row = reuseArray ? result.Rows[pos].Values : new object[properties.Count];
				for(int i = 0; i < properties.Count; i++)
					row[i] = queryData.GetAccessor(i).Value;
				res.Add(row);
				pos++;
			}
			return res;
		}
		public object SelectDataAsync(Session session, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, AsyncSelectDataCallback callback) {
			if(SynchronizationContext.Current == null) throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null) throw new ArgumentNullException();
			SynchronizationContext syncContext = SynchronizationContext.Current;
			return new AsyncRequest(syncContext, new AsyncRequestExec(delegate(AsyncRequest request) {
				List<object[]> result = null;
				try {
					if(query == null) {
						result = new List<object[]>(0);
					} else {
						result = SelectData(session, query, properties, groupProperties, groupCriteria);
					}
					session.AsyncExecuteQueue.Invoke(syncContext, new SendOrPostCallback(delegate(object o) {
						try {
							callback(result, null);
						} catch(Exception) { }
					}), null, true);
				} catch(Exception ex) {
					try {
						session.AsyncExecuteQueue.Invoke(syncContext, new SendOrPostCallback(delegate(object o) {
							try {
								callback(null, ex);
							} catch(Exception) { }
						}), null, true);
					} catch(Exception) { }
				}
			})).Start(session.AsyncExecuteQueue);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleObjectLayerDictionary")]
#endif
public XPDictionary Dictionary {
			get { return dataLayer.Dictionary; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleObjectLayerObjectLayer")]
#endif
public IObjectLayer ObjectLayer {
			get { return this; }
		}
#if !SL
		public event SchemaInitEventHandler SchemaInit {
			add { DataLayer.SchemaInit += value; }
			remove { DataLayer.SchemaInit -= value; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleObjectLayerConnection")]
#endif
public IDbConnection Connection {
			get { return DataLayer.Connection; }
		}
#endif
		public ICollection[] GetObjectsByKey(Session session, ObjectsByKeyQuery[] queries) {
			if(queries == null || queries.Length == 0)return new object[0][];
			List<ObjectsQuery> allQueries = new List<ObjectsQuery>();
			List<int> groupsCount = new List<int>();
			foreach(ObjectsByKeyQuery getQuery in queries) {
				List<ObjectsQuery> queriesByKeys = GetQueriesByKeys(session, getQuery);
				allQueries.AddRange(queriesByKeys);
				groupsCount.Add(queriesByKeys.Count);
			}
			ICollection[] allResults = LoadObjects(session, allQueries.ToArray());
			if(groupsCount.Count == 1) return new ICollection[] { GetObjectProcessGroup(allResults) };
			List<ICollection> result = new List<ICollection>();
			List<ICollection> currentGroupList = new List<ICollection>();
			int currentGroupIndex = 0;
			foreach(ICollection currentResultCollection in allResults) {
				currentGroupList.Add(currentResultCollection);
				if(currentGroupList.Count >= groupsCount[currentGroupIndex]) {
					result.Add(GetObjectProcessGroup(currentGroupList.ToArray()));
					currentGroupList.Clear();
					currentGroupIndex++;
					if(currentGroupIndex >= groupsCount.Count) break;
				}
			}
			return result.ToArray();
		}
		static ICollection GetObjectProcessGroup(ICollection[] results) {
			if(results.Length == 1) return results[0];
			int fullCount = 0;
			for(int i = 0; i < results.Length; i++) {
				fullCount += results[i].Count;
			}
			object[] result = new object[fullCount];
			int pos = 0;
			for(int i = 0; i < results.Length; i++) {
				foreach(object obj in results[i]) {
					result[pos++] = obj;
				}
			}
			return result;
		}
		static List<ObjectsQuery> GetQueriesByKeys(Session session, ObjectsByKeyQuery getQuery) {
			List<ObjectsQuery> queries = new List<ObjectsQuery>();
			OperandProperty keyProperty = new OperandProperty(getQuery.ClassInfo.KeyProperty.Name);
			int length = getQuery.IdCollection.Count;
			if(length == 0) return queries;
			if(length == 1) {
				IEnumerator enm = getQuery.IdCollection.GetEnumerator();
				object oneId = enm.MoveNext() ? enm.Current : null;
				queries.Add(new ObjectsQuery(getQuery.ClassInfo, new BinaryOperator(keyProperty, new OperandValue(oneId), BinaryOperatorType.Equal), null, 0, 0, new CollectionCriteriaPatcher(true, session.TypesManager), true));
				return queries;
			}
			List<CriteriaOperator> idList = new List<CriteriaOperator>();
			int pos = 0;
			int currentSize = 0;
			bool useGetTerminalInSize = getQuery.IdCollection.Count > XpoDefault.MaxInSize;
			foreach(object id in getQuery.IdCollection) {
				if(idList.Count == 0) {
					if(useGetTerminalInSize) {
						currentSize = XpoDefault.GetTerminalInSize(length - pos);
					} else {
						currentSize = length - pos;
					}
				}
				idList.Add(new OperandValue(id));
				pos++;
				currentSize--;
				if(currentSize == 0) {
					queries.Add(new ObjectsQuery(getQuery.ClassInfo, new InOperator(keyProperty, idList), null, 0, 0, new CollectionCriteriaPatcher(true, session.TypesManager), true));
					idList.Clear();
				}
			}
			return queries;
		}
		class CommitChangesContext {
			public ProcessingSave ProcessingSave;
			public BatchWideDataHolder4Modification BatchWideData;
			public ModificationStatement[] Statements;
			public ModificationResult Result;
			public Exception Ex;
			public Session Session;
			public CommitChangesContext(Session session, BatchWideDataHolder4Modification batchWideData) {
				this.Session = session;
				this.BatchWideData = batchWideData;
			}
		}
		class AsyncCommitRequest : AsyncRequest {
			CommitChangesContext commitContext;
			AsyncCommitCallback callback;
			public CommitChangesContext CommitContext { get { return commitContext; } }
			public AsyncCommitCallback Callback { get { return callback; } }
			public AsyncCommitRequest(CommitChangesContext commitContext, AsyncCommitCallback callback, SynchronizationContext syncContext, AsyncRequestExec exec)
				: base(syncContext, exec) {
				this.commitContext = commitContext;
				this.callback = callback;
			}
		}
		ICollection FilterListForDelete(Session session, ICollection fullListForDelete) {
			if(fullListForDelete == null) return null;
			if(fullListForDelete.Count == 0) return fullListForDelete;
			List<object> result = new List<object>(fullListForDelete.Count / 2);
			foreach(object objToDelete in fullListForDelete) {
				if(!session.IsNewObject(objToDelete)) {
					result.Add(objToDelete);
				}
			}
			return result;
		}
		public void CommitChanges(Session session, ICollection fullListForDelete, ICollection completeListForSave) {
			ICollection readyListForDelete = FilterListForDelete(session, fullListForDelete);
			BatchWideDataHolder4Modification batchWideData = new BatchWideDataHolder4Modification(this);
			batchWideData.RegisterDeletedObjects(fullListForDelete);
			CommitChangesContext context = new CommitChangesContext(session, batchWideData);
			BeginCommitChangesInsideTransaction(context, readyListForDelete, completeListForSave);
			if(context.Statements.Length != 0) {
				SessionStateStack.Enter(session, SessionState.CommitChangesToDataLayerInner);
				try {
					context.Result = DataLayer.ModifyData(context.Statements);
				}
				finally {
					SessionStateStack.Leave(session, SessionState.CommitChangesToDataLayerInner);
				}
			}
			EndCommitChangesInsideTransaction(context);
		}
		public object CommitChangesAsync(Session session, ICollection fullListForDelete, ICollection completeListForSave, AsyncCommitCallback callback) {
			if(SynchronizationContext.Current == null) throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null) throw new ArgumentNullException();
			BatchWideDataHolder4Modification batchWideData = new BatchWideDataHolder4Modification(this);
			ICollection readyListForDelete = FilterListForDelete(session, fullListForDelete);
			batchWideData.RegisterDeletedObjects(fullListForDelete);
			CommitChangesContext context = new CommitChangesContext(session, batchWideData);
			BeginCommitChangesInsideTransaction(context, readyListForDelete, completeListForSave);
			SessionStateStack.Enter(session, SessionState.CommitChangesToDataLayerInner);
			return new AsyncCommitRequest(context, callback, SynchronizationContext.Current, new AsyncRequestExec(CommitTransactionAsyncExec)).Start(session.AsyncExecuteQueue);
		}
		void CommitTransactionAsyncExec(AsyncRequest ar) {
			AsyncCommitRequest request = ar as AsyncCommitRequest;
			if(request == null) return;
			CommitChangesContext context = request.CommitContext;
			try {
				if(context.Statements.Length != 0) {
					context.Result = DataLayer.ModifyData(context.Statements);
				}
				context.Session.AsyncExecuteQueue.Invoke(request.SyncContext, new SendOrPostCallback(delegate(object obj) {
					try {
						SessionStateStack.Leave(request.CommitContext.Session, SessionState.CommitChangesToDataLayerInner);
						SessionStateStack.Enter(request.CommitContext.Session, SessionState.CommitTransactionNonReenterant);
						try {
							EndCommitChangesInsideTransaction(request.CommitContext);
						} finally {
							SessionStateStack.Leave(request.CommitContext.Session, SessionState.CommitTransactionNonReenterant);
						}
						request.Callback(request.CommitContext.Ex);
					} catch(Exception ex) {
						try {
							request.Callback(ex);
						} catch(Exception) { }
					}
				}), null, true);
			}
			catch(Exception ex) {
				try {
					context.Ex = ex;
					context.Session.AsyncExecuteQueue.Invoke(request.SyncContext, new SendOrPostCallback(delegate(object obj) {
						try {
							SessionStateStack.Leave(request.CommitContext.Session, SessionState.CommitChangesToDataLayerInner);
							request.Callback(request.CommitContext.Ex);
						}
						catch(Exception) { }
					}), null, true);
				}
				catch(Exception) { }
			}
		}
		void BeginCommitChangesInsideTransaction(CommitChangesContext context, ICollection fullListForDelete, ICollection completeListForSave) {
			List<ModificationStatement> deleteFirstList, deleteLastList;
			context.ProcessingSave = new ProcessingSave(context.Session, context.BatchWideData);
			ProcessDeletedObjects(context, fullListForDelete, completeListForSave, out deleteFirstList, out deleteLastList);
			List<ModificationStatement> insertupdateList = context.ProcessingSave.Process();
			List<ModificationStatement> statements = new List<ModificationStatement>();
			statements.AddRange(deleteFirstList);
			statements.AddRange(insertupdateList);
			statements.AddRange(deleteLastList);
			if(statements.Count != 0) {
				if(context.Session.LockingOption == LockingOption.None) {
					foreach(ModificationStatement statement in statements)
						statement.RecordsAffected = 0;
				}
			}
			context.Statements = statements.ToArray();
		}
		void EndCommitChangesInsideTransaction(CommitChangesContext context) {
			context.ProcessingSave.ProcessResults(context.Result);
		}
		public void CreateObjectType(XPObjectType objectType) {
			using(StrongSession s = new StrongSession(this)) {
				s.Save(objectType);
			}
		}
		void CollectClassInfosFormObjects(Session session, Dictionary<XPClassInfo, XPClassInfo> collector, IEnumerable objects) {
			foreach(object obj in objects) {
				session.ThrowIfObjectFromDifferentSession(obj);
				XPClassInfo ci = session.GetClassInfo(obj);
				collector[ci] = ci;
			}
		}
		Dictionary<XPClassInfo, XPClassInfo> CollectClassInfosFormObjects(Session session, IEnumerable objects) {
			Dictionary<XPClassInfo, XPClassInfo> result = new Dictionary<XPClassInfo, XPClassInfo>();
			CollectClassInfosFormObjects(session, result, objects);
			return result;
		}
		void ProcessDeletedObjects(CommitChangesContext context, ICollection fullListForDelete, ICollection completeListForSave, out List<ModificationStatement> beforeInserts, out List<ModificationStatement> afterUpdates) {
			Session session = context.Session;
			Dictionary<XPClassInfo, XPClassInfo> deleteBeforeClassInfos = CollectClassInfosFormObjects(session, fullListForDelete);
			Dictionary<XPClassInfo, XPClassInfo> badClassInfos = CollectClassInfosFormObjects(session, completeListForSave);
			for(; ; ) {
				bool changes = false;
				foreach(XPClassInfo badCi in ListHelper.FromCollection(badClassInfos.Keys)) {
					foreach(XPMemberInfo mi in badCi.ObjectProperties) {
						foreach(XPClassInfo goodCi in ListHelper.FromCollection(deleteBeforeClassInfos.Keys)) {
							if(goodCi.IsAssignableTo(mi.ReferenceType)) {
								deleteBeforeClassInfos.Remove(goodCi);
								badClassInfos[goodCi] = goodCi;
								changes = true;
							}
						}
					}
				}
				if(!changes)
					break;
			}
			List<object> objectsDeletedFirst = new List<object>();
			List<object> objectsDeletedLast = new List<object>();
			foreach(object obj in fullListForDelete) {
				if(deleteBeforeClassInfos.ContainsKey(session.GetClassInfo(obj)))
					objectsDeletedFirst.Add(obj);
				else
					objectsDeletedLast.Add(obj);
			}
			beforeInserts = DeleteQueryGenerator.GenerateDelete(Dictionary, objectsDeletedFirst, session.LockingOption, context.BatchWideData);
			afterUpdates = DeleteQueryGenerator.GenerateDelete(Dictionary, objectsDeletedLast, session.LockingOption, context.BatchWideData);
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types) {
			return DataLayer.UpdateSchema(dontCreateIfFirstTableNotExist, types);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleObjectLayerAutoCreateOption")]
#endif
public AutoCreateOption AutoCreateOption {
			get { return DataLayer.AutoCreateOption; }
		}
		public object[] LoadDelayedProperties(Session session, object theObject, MemberPathCollection props) {
			SelectStatement root = ClientSelectSqlGenerator.GenerateSelect(session.GetClassInfo(theObject),
				new OperandProperty(session.GetClassInfo(theObject).KeyProperty.Name) == new OperandValue(theObject),
				props,
				null, null, null, (CollectionCriteriaPatcher)null, 0, 1);
			object[] dbResult = DataLayer.SelectData(root).ResultSet[0].Rows[0].Values;
			object[] result = new object[props.Count];
			for(int i = 0; i < props.Count; ++i) {
				result[i] = QueryData.CreateDataConverter(props[i][0]).ConvertFromStorageType(dbResult[i]);
			}
			return result;
		}
		public ObjectDictionary<object> LoadDelayedProperties(Session session, IList objects, XPMemberInfo property) {
			XPClassInfo selectClass = property.Owner;
			if(!property.Owner.IsPersistent) {
				selectClass = property.GetMappingClass(session.GetClassInfo(objects[0]));
			}
			List<SelectStatement> selects = new List<SelectStatement>();
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			props.Add(new OperandProperty(selectClass.KeyProperty.Name));
			props.Add(new OperandProperty(property.Name));
			for(int i = 0; i < objects.Count; ) {
				int inSize = XpoDefault.GetTerminalInSize(objects.Count - i);
				selects.Add(ClientSelectSqlGenerator.GenerateSelect(selectClass,
				new InOperator(selectClass.KeyProperty.Name, DevExpress.Xpo.Generators.GetRangeHelper.GetRange(objects, i, inSize)),
				props,
				null, null, null, (CollectionCriteriaPatcher)null, 0, 0));
				i += inSize;
			}
			SelectedData selected = DataLayer.SelectData(selects.ToArray());
			ValueConverter keyConverter = QueryData.CreateDataConverter(selectClass.KeyProperty);
			ValueConverter valConverter = QueryData.CreateDataConverter(property);
			ObjectDictionary<object> result = new ObjectDictionary<object>(objects.Count);
			foreach(SelectStatementResult rowSet in selected.ResultSet) {
				foreach(SelectStatementResultRow row in rowSet.Rows) {
					object key = keyConverter.ConvertFromStorageType(row.Values[0]);
					object obj = session.GetObjectByKey(selectClass, key);
					if(obj == null)
						continue;
					object val = valConverter.ConvertFromStorageType(row.Values[1]);
					result.Add(obj, val);
				}
			}
			return result;
		}
		public void SetObjectLayerWideObjectTypes(Dictionary<XPClassInfo, XPObjectType> loadedTypes) {
			BaseDataLayer.SetDataLayerWideObjectTypes(DataLayer, loadedTypes);
		}
		public Dictionary<XPClassInfo, XPObjectType> GetObjectLayerWideObjectTypes() {
			return BaseDataLayer.GetDataLayerWideObjectTypes(DataLayer);
		}
		public void RegisterStaticTypes(params XPClassInfo[] types) {
			BaseDataLayer.RegisterStaticTypes(DataLayer, types);
		}
		public bool IsStaticType(XPClassInfo type) {
			return BaseDataLayer.IsStaticType(DataLayer, type);
		}
		public IObjectMap GetStaticCache(XPClassInfo info) {
			return BaseDataLayer.GetStaticCache(DataLayer, info);
		}
#if !SL && DEBUGTEST
		public IDbCommand CreateCommand() {
			return DataLayer.CreateCommand();
		}
#endif
		public void ClearDatabase() {
			((IDataLayerForTests)DataLayer).ClearDatabase();			
		}
		public PurgeResult Purge() {
			return Purger.Purge(this, DataLayer);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleObjectLayerDataLayer")]
#endif
public IDataLayer DataLayer {
			get { return dataLayer; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SimpleObjectLayerCanLoadCollectionObjects")]
#endif
public bool CanLoadCollectionObjects {
			get { return false; }
		}
		public object[] LoadCollectionObjects(Session session, XPMemberInfo refProperty, object ownerObject) {
			throw new NotSupportedException();
		}
		object ICommandChannel.Do(string command, object args) {
			if(nestedCommandChannel == null) {
				if(dataLayer == null) {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, dataLayer.GetType().FullName));
				}
			}
			return nestedCommandChannel.Do(command, args);
		}
	}
}
