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
using System.Text;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata.Helpers;
using System.Collections;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.DB;
using System.Data;
using DevExpress.Xpo.Exceptions;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.ComponentModel;
namespace DevExpress.Xpo {
	public class SessionObjectLayer:
#if !SL
		IObjectLayerForTests
#else 
		IObjectLayer
#endif
		, IObjectLayerEx
		, IObjectLayerOnSession
		, ICommandChannel
	{
		public readonly bool ThroughCommitMode;
		public readonly Session ParentSession;
		readonly ICommandChannel nestedCommandChannel;
		readonly SecurityContext securityContextMain;		
		public SessionObjectLayer(Session parentSession)
			: this(parentSession, false) {
		}
		public SessionObjectLayer(Session parentSession, bool throughCommitMode)
			: this(parentSession, throughCommitMode, null, null, null) {
		}
		public SessionObjectLayer(Session parentSession, bool throughCommitMode, IGenericSecurityRule genericSecurityRule, ISecurityRuleProvider securityDictionary, object securityCustomContext) {
			this.ParentSession = parentSession;
			this.nestedCommandChannel = ParentSession as ICommandChannel;
			this.ThroughCommitMode = throughCommitMode;
			if(securityDictionary == null) return;
			this.securityContextMain = new SecurityContext(this, genericSecurityRule, securityDictionary, securityCustomContext);
		}
		SecurityContext GetSecurityContext(Session nestedSession) {
			return securityContextMain == null ? null : securityContextMain.Clone(nestedSession);
		}
		public void ClearDatabase() {
			ParentSession.ClearDatabase();
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types) {
			return ParentSession.UpdateSchema(dontCreateIfFirstTableNotExist, types);
		}
		static object nestedParentMapKeyObject = new object();
		internal NestedParentMap GetNestedParentMap(Session session) {
			object nestedParentMap = null;
			if(!((IWideDataStorage)session).TryGetWideDataItem(nestedParentMapKeyObject, out nestedParentMap)) {
#if !SL
				session.IdentityMapBehavior = ParentSession.IdentityMapBehavior;
				if(session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak) {
					nestedParentMap = new WeakNestedParentMap();
				} else
#endif
					nestedParentMap = new StrongNestedParentMap();
				((IWideDataStorage)session).SetWideDataItem(nestedParentMapKeyObject, nestedParentMap);
			}
			return (NestedParentMap)nestedParentMap;
		}
		public ICollection[] LoadObjects(Session session, ObjectsQuery[] queries) {
			if(queries == null || queries.Length == 0) return new object[0][];
			SecurityContext securityContext = GetSecurityContext(session);
			ObjectsQuery[] parentQueries = GetParentQueries(session, queries, securityContext);
			ICollection[] parentObjects = ParentSession.GetObjects(parentQueries);
			return new NestedLoader(session, ParentSession, GetNestedParentMap(session), securityContext).GetNestedObjects(parentObjects, GetForceList(queries));
		}
		class ParentAsyncResultInternal {
			Exception ex;
			object obj;
			public Exception Ex { get { return ex; } set { ex = value; } }
			public object Obj { get { return obj; } set { obj = value; } }
		}
		public object LoadObjectsAsync(Session session, ObjectsQuery[] queries, AsyncLoadObjectsCallback callback) {
			if(SynchronizationContext.Current == null) throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null) throw new ArgumentNullException();
			bool[] force = GetForceList(queries);
			SynchronizationContext syncContext = SynchronizationContext.Current;
			SecurityContext securityContext = GetSecurityContext(session);
			ManualResetEvent parentSessionLoadComplete = new ManualResetEvent(false);
			ParentAsyncResultInternal parentSessionLoadObjectsResult = new ParentAsyncResultInternal();
			ObjectsQuery[] parentQueries = GetParentQueries(session, queries, securityContext);
			AsyncRequest nestedRequest = ParentSession.GetObjectsAsync(parentQueries, new AsyncLoadObjectsCallback(delegate(ICollection[] collection, Exception ex) {
				parentSessionLoadObjectsResult.Obj = collection;
				parentSessionLoadObjectsResult.Ex = ex;
				parentSessionLoadComplete.Set();
			})) as AsyncRequest;			
			AsyncRequest mainRequest = new AsyncRequest(syncContext, new AsyncRequestExec(delegate(AsyncRequest ar){
				ar.RemoveNestedRequest(nestedRequest);
				parentSessionLoadComplete.WaitOne();
				ICollection[] parentCollection = (ICollection[])parentSessionLoadObjectsResult.Obj;
				Exception parentEx = parentSessionLoadObjectsResult.Ex;
				try {
					session.AsyncExecuteQueue.Invoke(ar.SyncContext, new SendOrPostCallback(delegate(object obj) {
						try {
							if(parentEx != null)
								callback(null, parentEx);
							else
								callback(new NestedLoader(session, ParentSession, GetNestedParentMap(session), securityContext).GetNestedObjects(parentCollection, force), null);
						}
						catch(Exception ex) {
							try {
								callback(null, ex);
							}
							catch(Exception) { }
						}
					}), null, true);
				}
				catch(Exception ex) {
					try {
						session.AsyncExecuteQueue.Invoke(ar.SyncContext, new SendOrPostCallback(delegate(object obj) {
							try {
								callback(null, ex);
							}
							catch(Exception) { }
						}), null, true);
					}
					catch(Exception) { }
				}
			}));
			mainRequest.AddNestedRequest(nestedRequest);
			return mainRequest.Start(session.AsyncExecuteQueue);
		}
		bool[] GetForceList(ObjectsQuery[] queries) {
			bool[] force = new bool[queries.Length];
			for(int i = 0; i < force.Length; i++) {
				force[i] = queries[i].Force;
			}
			return force;
		}
		ObjectsQuery[] GetParentQueries(Session session, ObjectsQuery[] queries, SecurityContext securityContext) {
			ObjectsQuery[] parentQueries = new ObjectsQuery[queries.Length];
			for(int i = 0; i < queries.Length; i++) {
				CriteriaOperator criteria = queries[i].Criteria;
				SortingCollection sorting = queries[i].Sorting;
				if(securityContext != null) {
					CriteriaOperator patchCriteria;
					ISecurityRule rule = securityContext.SecurityRuleProvider.GetRule(queries[i].ClassInfo);
					if(rule != null) {
						if(!SecurityContext.IsSystemClass(queries[i].ClassInfo) && rule.GetSelectFilterCriteria(securityContext, queries[i].ClassInfo, out patchCriteria))
							criteria = GroupOperator.And(SecurityCriteriaPatcher.Patch(queries[i].ClassInfo, securityContext, criteria), securityContext.ExpandToLogical(queries[i].ClassInfo, patchCriteria));
						else
							criteria = SecurityCriteriaPatcher.Patch(queries[i].ClassInfo, securityContext, criteria);
					}
					if(sorting != null) {
						SortingCollection patchedSorting = new SortingCollection();
						for(int s = 0; s < sorting.Count; s++) {
							patchedSorting.Add(new SortProperty(SecurityCriteriaPatcher.Patch(queries[i].ClassInfo, securityContext, sorting[s].Property), sorting[s].Direction));
						}
						sorting = patchedSorting;
					}
				}
				parentQueries[i] = new ObjectsQuery(queries[i].ClassInfo, GetNestedCriteria(session, criteria, securityContext),
					sorting, queries[i].SkipSelectedRecords, queries[i].TopSelectedRecords, Generators.CollectionCriteriaPatcher.CloneToAnotherSession(queries[i].CollectionCriteriaPatcher, ParentSession), queries[i].Force);
			}
			return parentQueries;
		}
		public List<object[]> SelectData(Session session, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria) {
			SecurityContext securityContext = GetSecurityContext(session);
			ObjectsQuery parentQuery = GetParentQueries(session, new ObjectsQuery[] { query }, securityContext)[0];
			return ParentSession.SelectData(parentQuery.ClassInfo, 
				SecurityPatchProperties(parentQuery.ClassInfo, properties, securityContext),
				parentQuery.Criteria,
				SecurityPatchProperties(parentQuery.ClassInfo, groupProperties, securityContext),
				securityContext == null ? groupCriteria : SecurityCriteriaPatcher.Patch(parentQuery.ClassInfo, securityContext, groupCriteria),
				parentQuery.CollectionCriteriaPatcher.SelectDeleted, parentQuery.SkipSelectedRecords, parentQuery.TopSelectedRecords, parentQuery.Sorting);
		}
		CriteriaOperatorCollection SecurityPatchProperties(XPClassInfo classInfo, CriteriaOperatorCollection properties, SecurityContext securityContext) {
			if(securityContext == null || properties == null || properties.Count == 0) return properties;
			CriteriaOperatorCollection patchedCollection = new CriteriaOperatorCollection();
			bool patched = false;
			for(int i = 0; i < properties.Count; i++) {
				CriteriaOperator patchedCriteria = SecurityCriteriaPatcher.Patch(classInfo, securityContext, properties[i]);
				if(!ReferenceEquals(patchedCriteria, properties[i])) patched = true;
				patchedCollection.Add(patchedCriteria);
			}
			if(patched) return patchedCollection;
			return properties;
		}
		public object SelectDataAsync(Session session, ObjectsQuery query, CriteriaOperatorCollection properties, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, AsyncSelectDataCallback callback) {
			if(SynchronizationContext.Current == null) throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null) throw new ArgumentNullException();
			SecurityContext securityContext = GetSecurityContext(session);
			SynchronizationContext syncContext = SynchronizationContext.Current;
			ManualResetEvent parentSessionSelectDataEvent = new ManualResetEvent(false);
			ParentAsyncResultInternal parentSessionSelectDataResult = new ParentAsyncResultInternal();
			ObjectsQuery parentQuery = GetParentQueries(session, new ObjectsQuery[] { query }, securityContext)[0];
			AsyncRequest nestedRequest = ParentSession.SelectDataAsync(parentQuery.ClassInfo,
				SecurityPatchProperties(parentQuery.ClassInfo, properties, securityContext),
				parentQuery.Criteria,
				SecurityPatchProperties(parentQuery.ClassInfo, groupProperties, securityContext),
				securityContext == null ? groupCriteria : SecurityCriteriaPatcher.Patch(parentQuery.ClassInfo, securityContext, groupCriteria),
				parentQuery.CollectionCriteriaPatcher.SelectDeleted, parentQuery.SkipSelectedRecords, parentQuery.TopSelectedRecords, parentQuery.Sorting,
					new AsyncSelectDataCallback(delegate(List<object[]> result, Exception ex) {
				parentSessionSelectDataResult.Obj = result;
				parentSessionSelectDataResult.Ex = ex;
				parentSessionSelectDataEvent.Set();
			})) as AsyncRequest;
			AsyncRequest mainRequest = new AsyncRequest(syncContext, new AsyncRequestExec(delegate(AsyncRequest request) {
				request.RemoveNestedRequest(nestedRequest);
				parentSessionSelectDataEvent.WaitOne();
				try {
					session.AsyncExecuteQueue.Invoke(request.SyncContext, new SendOrPostCallback(delegate(object obj) {
						try {
							callback((List<object[]>)parentSessionSelectDataResult.Obj, parentSessionSelectDataResult.Ex);
						}
						catch(Exception ex) {
							try {
								callback(null, ex);
							}
							catch(Exception) { }
						}
					}), null, true);
				}
				catch(Exception ex) {
					try {
						session.AsyncExecuteQueue.Invoke(request.SyncContext, new SendOrPostCallback(delegate(object obj) {
							try {
								callback(null, ex);
							}
							catch(Exception) { }
						}), null, true);
					}
					catch(Exception) { }
				}
			}));
			mainRequest.AddNestedRequest(nestedRequest);
			return mainRequest.Start(session.AsyncExecuteQueue);
		}
		class AsyncCommitContext {
			public readonly Session Session;
			public readonly ICollection ReadyListForDelete;
			public readonly ICollection CompleteListForSave;
			public readonly ICollection FullListForDelete;
			public readonly NestedParentMap Map;
			public readonly List<object> ParentsToSave;
			public CommitChangesMode Mode;
			public ObjectSet LockedParentsObjects;
			public readonly SecurityContext SecurityContext;
			ObjectDictionary<XPMemberInfo[]> membersNotToSaveDictionary;
			public bool IsMembersNotToSaveDictionaryExists {
				get { return membersNotToSaveDictionary != null; }
			}
			public ObjectDictionary<XPMemberInfo[]> MembersNotToSaveDictionary {
				get {
					if(membersNotToSaveDictionary == null) {
						membersNotToSaveDictionary = new ObjectDictionary<XPMemberInfo[]>();
					}
					return membersNotToSaveDictionary;
				}
			}
			public AsyncCommitContext(Session session, ICollection fullListForDelete, ICollection readyListForDelete, ICollection completeListForSave, NestedParentMap map, SecurityContext securityContext) {
				this.Session = session;
				this.ReadyListForDelete = readyListForDelete;
				this.CompleteListForSave = completeListForSave;
				this.FullListForDelete = fullListForDelete;
				this.Map = map;
				this.ParentsToSave = new List<object>();
				this.LockedParentsObjects = new ObjectSet();
				this.SecurityContext = securityContext;
			}
		}
		ICollection FilterListForDelete(Session session, NestedParentMap map, ICollection fullListForDelete) {
			if(fullListForDelete == null) return null;
			if(fullListForDelete.Count == 0) return fullListForDelete;
			List<object> result = new List<object>(fullListForDelete.Count / 2);
			foreach(object objToDelete in fullListForDelete) {
				if(session.IsNewObject(objToDelete)) {
					object parent = NestedWorksHelper.GetParentObject(session, ParentSession, map, objToDelete);
					if(parent == null) continue;
				}
				result.Add(objToDelete);
			}
			return result;
		}
		public void CommitChanges(Session session, ICollection fullListForDelete, ICollection completeListForSave) {
			NestedParentMap map = GetNestedParentMap(session);
			ICollection readyListForDelete = FilterListForDelete(session, map, fullListForDelete);
			AsyncCommitContext context = new AsyncCommitContext(session, fullListForDelete, readyListForDelete, completeListForSave, map, GetSecurityContext(session));
			BeginCommitChanges(context);
			EndCommitChanges(context);
			if(ThroughCommitMode && (context.Mode == CommitChangesMode.NotInTransactionUnitOfWork || context.Mode == CommitChangesMode.InTransaction)) {
				if(ParentSession.TrackingChanges) {
					ParentSession.FlushChanges();
				}
				ThroughCommitExec(context);
			}
		}
		public object CommitChangesAsync(Session session, ICollection fullListForDelete, ICollection completeListForSave, AsyncCommitCallback callback) {
			if(SynchronizationContext.Current == null) throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null) throw new ArgumentNullException();
			SynchronizationContext syncContext = SynchronizationContext.Current;
			NestedParentMap map = GetNestedParentMap(session);
			ICollection readyListForDelete = FilterListForDelete(session, map, fullListForDelete);
			AsyncCommitContext context = new AsyncCommitContext(session, fullListForDelete, readyListForDelete, completeListForSave, map, GetSecurityContext(session));
			BeginCommitChanges(context);
			return new AsyncRequest(syncContext, new AsyncRequestExec(delegate(AsyncRequest request) {
				try {
					session.AsyncExecuteQueue.Invoke(request.SyncContext, new SendOrPostCallback(delegate(object obj) {
						try {
							EndCommitChanges(context);
							if(ThroughCommitMode && (context.Mode == CommitChangesMode.NotInTransactionUnitOfWork || context.Mode == CommitChangesMode.InTransaction)) {
								if(ParentSession.TrackingChanges) {
									request.AddNestedRequest(ParentSession.CommitTransactionAsync(new AsyncCommitCallback(delegate(Exception parentEx) {
										try {
											ThroughCommitExec(context);
											callback(null);
										} catch(Exception ex) {
											try {
												callback(ex);
											} catch(Exception) { }
										}
									})) as AsyncRequest);
									return;
								}
								ThroughCommitExec(context);
							}
							callback(null);
						}
						catch(Exception ex) {
							try {
								callback(ex);
							}
							catch(Exception) { }
						}			  
					}), null, true);
				}
				catch(Exception ex) {
					try {
						session.AsyncExecuteQueue.Invoke(request.SyncContext, new SendOrPostCallback(delegate(object obj) {
							try {
								callback(ex);
							}
							catch(Exception) { }
						}), request, true);
					}
					catch(Exception) { }
				}
			})).Start(session.AsyncExecuteQueue);
		}
		void ThroughCommitExec(AsyncCommitContext context) {
			SessionStateStack.Enter(context.Session, SessionState.ApplyIdentities);
			try {
				foreach(object objToSave in context.CompleteListForSave) {
					XPClassInfo ci = context.Session.GetClassInfo(objToSave);
					object parentObj = NestedWorksHelper.GetParentObject(context.Session, ParentSession, context.Map, objToSave);
					if(parentObj == null) continue;
					if(context.Session.IsNewObject(objToSave)) {
						object key = ci.KeyProperty.GetValue(parentObj);
						ci.KeyProperty.SetValue(objToSave, key);
						SessionIdentityMap.RegisterObject(context.Session, objToSave, ci.KeyProperty.ExpandId(key)); 
					}
					XPMemberInfo olf = ci.OptimisticLockField;
					if(olf == null) continue;
					olf.SetValue(objToSave, olf.GetValue(parentObj));
				}
			} finally {
				SessionStateStack.Leave(context.Session, SessionState.ApplyIdentities);
			}
		}
		void BeginCommitChanges(AsyncCommitContext context) {
			if(context.Session.LockingOption != LockingOption.None) {
				NestedWorksHelper.ValidateVersions(context.Session, ParentSession, GetNestedParentMap(context.Session), context.LockedParentsObjects, context.ReadyListForDelete, context.Session.LockingOption, true);
				NestedWorksHelper.ValidateVersions(context.Session, ParentSession, GetNestedParentMap(context.Session), context.LockedParentsObjects, context.CompleteListForSave, context.Session.LockingOption, false);
			}
			if(context.SecurityContext != null) {
				ValidateObjectsOnCommit(context);
			}
			if(!ParentSession.TrackingChanges) {
				if(ParentSession.IsUnitOfWork)
					context.Mode = CommitChangesMode.NotInTransactionUnitOfWork;
				else
					context.Mode = CommitChangesMode.NotInTransactionSession;
				ParentSession.BeginTrackingChanges();
			}
			else
				context.Mode = CommitChangesMode.InTransaction;
			SessionStateStack.Enter(context.Session, SessionState.CommitChangesToDataLayerInner);
			SessionStateStack.Enter(ParentSession, SessionState.ReceivingObjectsFromNestedUow);
		}
		void EndCommitChanges(AsyncCommitContext context) {
			switch(context.Mode) {
				case CommitChangesMode.InTransaction:
				case CommitChangesMode.NotInTransactionUnitOfWork:
					Commit(context);
					break;
				case CommitChangesMode.NotInTransactionSession: {
						try {
							Commit(context);
							ParentSession.FlushChanges();
						}
						catch(Exception e) {
							if(ParentSession.TrackingChanges) {
								try {
									ParentSession.DropChanges();
								}
								catch(Exception e2) {
									throw new ExceptionBundleException(e, e2);
								}
							}
							throw;
						}
					}
					break;
			}
			GC.KeepAlive(context.LockedParentsObjects);
		}
		void Commit(AsyncCommitContext context) {
			try {
				foreach (object objToDel in context.ReadyListForDelete) {
					NestedWorksHelper.CommitDeletedObject(context.Session, ParentSession, context.Map, objToDel);
				}
				foreach (object objToSave in context.CompleteListForSave) {
					XPMemberInfo[] membersNotToSave = null;
					if(context.SecurityContext != null && context.IsMembersNotToSaveDictionaryExists)
						context.MembersNotToSaveDictionary.TryGetValue(objToSave, out membersNotToSave);
					context.ParentsToSave.Add(NestedWorksHelper.CommitObject(context.Session, ParentSession, context.Map, objToSave, membersNotToSave));
				}
				ParentSession.Save(context.ParentsToSave);
			} finally {
				SessionStateStack.Leave(ParentSession, SessionState.ReceivingObjectsFromNestedUow);
				SessionStateStack.Leave(context.Session, SessionState.CommitChangesToDataLayerInner);
			}
			foreach(object obj in context.FullListForDelete) {
				if(!context.Session.IsNewObject(obj))
					SessionIdentityMap.UnregisterObject(context.Session, obj);
				context.Map.KickOut(obj);
				IXPInvalidateableObject spoilableObject = obj as IXPInvalidateableObject;
				if(spoilableObject != null)
					spoilableObject.Invalidate();
			}
			ParentSession.TriggerObjectsLoaded(context.ParentsToSave);
		}
		void ValidateObjectsOnCommit(AsyncCommitContext context) {
			SecurityContext securityContext = context.SecurityContext;
			SecurityContextValidateItem[] objectsForSave = new SecurityContextValidateItem[context.CompleteListForSave.Count];
			SecurityContextValidateItem[] objectsForDelete = new SecurityContextValidateItem[context.ReadyListForDelete.Count];
			int counter = 0;
			foreach(object objToSave in context.CompleteListForSave) {
				objectsForSave[counter++] = new SecurityContextValidateItem(context.Session.GetClassInfo(objToSave), objToSave,
					NestedWorksHelper.GetParentObject(context.Session, ParentSession, context.Map, objToSave));
			}
			counter = 0;
			foreach(object objToDelete in context.ReadyListForDelete) {
				objectsForDelete[counter++] = new SecurityContextValidateItem(context.Session.GetClassInfo(objToDelete), objToDelete,
					NestedWorksHelper.GetParentObject(context.Session, ParentSession, context.Map, objToDelete));
			}
			if(securityContext.GenericSecurityRule != null
				&& !securityContext.GenericSecurityRule.ValidateObjectsOnCommit(securityContext, objectsForSave, objectsForDelete))
					throw new ObjectLayerSecurityException();
			for(int i = 0; i < objectsForSave.Length; i++) {
				SecurityContextValidateItem saveItem = objectsForSave[i];
				XPClassInfo ci = saveItem.ClassInfo;
				ISecurityRule rule = securityContext.SecurityRuleProvider.GetRule(ci);
				if(rule != null) {
					if(!rule.ValidateObjectOnSave(securityContext, ci, saveItem.TheObject, saveItem.RealObject))
						throw new ObjectLayerSecurityException(ci.FullName, false);
					List<XPMemberInfo> membersNotToSave = null;
					foreach(XPMemberInfo mi in ci.PersistentProperties) {
						if(mi.IsDelayed && !XPDelayedProperty.GetDelayedPropertyContainer(saveItem.TheObject, mi).IsLoaded)
							continue;
						if(mi.IsReadOnly)
							continue;
						object value = mi.GetValue(saveItem.TheObject);
						if(SecurityContext.IsSystemProperty(mi)) {
							continue;
						}
						object oldValue = null;
						object realOldValue = null;
						if(saveItem.RealObject != null) {
							realOldValue = mi.GetValue(saveItem.RealObject);
							CriteriaOperator memberExpression;
							if(rule.GetSelectMemberExpression(securityContext, ci, mi, out memberExpression)) {
								oldValue = securityContext.Evaluate(AnalyzeCriteriaCreator.GetUpClass(ci, mi.Owner), memberExpression, saveItem.RealObject);
							} else {
								oldValue = realOldValue;
							}
						}
						if(mi.ReferenceType != null && oldValue != null) {
							ISecurityRule referenceRule = securityContext.SecurityRuleProvider.GetRule(mi.ReferenceType);
							if(referenceRule != null) {
								oldValue = referenceRule.ValidateObjectOnSelect(securityContext, mi.ReferenceType, oldValue) ? oldValue : null;
							}
						}
						switch(rule.ValidateMemberOnSave(securityContext, mi, saveItem.TheObject, saveItem.RealObject, value, oldValue, realOldValue)) {
							case ValidateMemberOnSaveResult.DoRaiseException:
								throw new ObjectLayerSecurityException(saveItem.ClassInfo.FullName, mi.Name);
							case ValidateMemberOnSaveResult.DoNotSaveMember:
								if(membersNotToSave == null) membersNotToSave = new List<XPMemberInfo>();
								membersNotToSave.Add(mi);
								break;
						}
					}
					if(membersNotToSave != null && membersNotToSave.Count > 0) {
						context.MembersNotToSaveDictionary.Add(saveItem.TheObject, membersNotToSave.ToArray());
					}
				}
			}
			for(int i = 0; i < objectsForDelete.Length; i++) {
				SecurityContextValidateItem deleteItem = objectsForDelete[i];
				ISecurityRule rule = securityContext.SecurityRuleProvider.GetRule(deleteItem.ClassInfo);
				if(rule != null && !rule.ValidateObjectOnDelete(securityContext, deleteItem.ClassInfo, deleteItem.TheObject, deleteItem.RealObject)) {
					throw new ObjectLayerSecurityException(deleteItem.ClassInfo.FullName, true);
				}
			}
		}
		public void CreateObjectType(XPObjectType type) {
			ParentSession.ObjectLayer.CreateObjectType(type);
		}
		public ICollection[] GetObjectsByKey(Session session, ObjectsByKeyQuery[] queries) {
			if(queries == null || queries.Length == 0) return new object[0][];
			ICollection[] parentObjects = ParentSession.GetObjectsByKey(queries, false);
			bool[] force = new bool[parentObjects.Length];
			for(int i = 0; i < force.Length; i++) force[i] = true;
			return new NestedLoader(session, ParentSession, GetNestedParentMap(session), GetSecurityContext(session)).GetNestedObjects(parentObjects, force);
		}
		public object[] LoadDelayedProperties(Session session, object theObject, MemberPathCollection props) {
			object[] result = new object[props.Count];
			SecurityContext securityContext = GetSecurityContext(session);
			ISecurityRule rule = null;
			XPClassInfo ci = null;
			if(securityContext != null && props.Count > 0) {
				ci = session.GetClassInfo(theObject);
				rule = securityContext.SecurityRuleProvider.GetRule(ci);
			}
			bool isSystemClass = SecurityContext.IsSystemClass(ci);
			object parentObject = NestedWorksHelper.GetParentObject(session, ParentSession, GetNestedParentMap(session), theObject);
			for(int i = 0; i < props.Count; ++i) {
				XPMemberInfo mi = props[i][0];
				if(rule != null && !isSystemClass && !SecurityContext.IsSystemProperty(mi)) {
					CriteriaOperator expression;
					if(securityContext.GetSelectMemberExpression(rule, ci, mi, out expression)) {
						result[i] = securityContext.Evaluate(AnalyzeCriteriaCreator.GetUpClass(ci, mi.Owner), expression, parentObject);
						continue;
					}
				}
				result[i] = mi.GetValue(parentObject);
			}
			return result;
		}
		public ObjectDictionary<object> LoadDelayedProperties(Session session, IList objects, XPMemberInfo property) {
			ObjectDictionary<object> result = new ObjectDictionary<object>(objects.Count);
			SecurityContext securityContext = GetSecurityContext(session);
			foreach(object obj in objects) {
				object parentObj = NestedWorksHelper.GetParentObject(session, ParentSession, GetNestedParentMap(session), obj);
				if(securityContext != null) {
					XPClassInfo ci = session.GetClassInfo(obj);
					ISecurityRule rule = securityContext.SecurityRuleProvider.GetRule(ci);
					if(rule != null && !SecurityContext.IsSystemClass(ci) && !SecurityContext.IsSystemProperty(property)) {
						CriteriaOperator expression;
						if(securityContext.GetSelectMemberExpression(rule, ci, property, out expression)) {
							result.Add(obj, securityContext.Evaluate(property.Owner, expression, parentObj));
							continue;
						}
					}
				}
				result.Add(obj, property.GetValue(parentObj));
			}
			return result;
		}
		public PurgeResult Purge() {
			return ParentSession.PurgeDeletedObjects();
		}
		public void SetObjectLayerWideObjectTypes(Dictionary<XPClassInfo, XPObjectType> loadedTypes) {
			ParentSession.ObjectLayer.SetObjectLayerWideObjectTypes(loadedTypes);
		}
		public Dictionary<XPClassInfo, XPObjectType> GetObjectLayerWideObjectTypes() {
			return ParentSession.ObjectLayer.GetObjectLayerWideObjectTypes();
		}
		public void RegisterStaticTypes(params XPClassInfo[] types) {
			ParentSession.ObjectLayer.RegisterStaticTypes(types);
		}
		public bool IsStaticType(XPClassInfo type) {
			return ParentSession.ObjectLayer.IsStaticType(type);
		}
		public IObjectMap GetStaticCache(XPClassInfo info) {
			return ParentSession.ObjectLayer.GetStaticCache(info);
		}
#if !SL
#pragma warning disable 618
		public event SchemaInitEventHandler SchemaInit {
			add { ParentSession.SchemaInit += value; }
			remove { ParentSession.SchemaInit -= value; }
		}
#pragma warning restore 618
#if !SL
	[DevExpressXpoLocalizedDescription("SessionObjectLayerConnection")]
#endif
public IDbConnection Connection {
			get { return null; }
		}
#if DEBUGTEST
		public IDbCommand CreateCommand() {
			throw new NotSupportedException();
		}
#endif
#endif
#if !SL
	[DevExpressXpoLocalizedDescription("SessionObjectLayerAutoCreateOption")]
#endif
public AutoCreateOption AutoCreateOption {
			get { return ParentSession.AutoCreateOption; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionObjectLayerObjectLayer")]
#endif
public IObjectLayer ObjectLayer {
			get { return this; }
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionObjectLayerDictionary")]
#endif
public XPDictionary Dictionary {
			get { return ParentSession.Dictionary; }
		}
		CriteriaOperator GetNestedCriteria(Session session, CriteriaOperator criteria, SecurityContext securityContext) {
			return ParentCriteriaGenerator.GetNestedCriteria(session, ParentSession, GetNestedParentMap(session), securityContext, criteria);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionObjectLayerDataLayer")]
#endif
public IDataLayer DataLayer {
			get { return ParentSession.DataLayer; }
		}
		public bool IsParentObjectToSave(Session session, object theObject) {
			if(theObject == null) return false;
			object parent = NestedWorksHelper.GetParentObject(session, ParentSession, GetNestedParentMap(session), theObject);
			if(parent == null)
				return false;
			return ParentSession.IsObjectToSave(parent, true);
		}
		public bool IsParentObjectToDelete(Session session, object theObject) {
			if(theObject == null) return false;
			object parent = NestedWorksHelper.GetParentObject(session, ParentSession, GetNestedParentMap(session), theObject);
			if(parent == null)
				return false;
			return ParentSession.IsObjectToDelete(parent, true);
		}
		public ICollection GetParentObjectsToSave(Session session) {
			ICollection parentResult = ParentSession.GetObjectsToSave(true);
			object[] result = new object[parentResult.Count];
			int index = 0;
			SecurityContext securityContext = GetSecurityContext(session);
			foreach(object obj in parentResult) {
				result[index++] = NestedWorksHelper.GetNestedObject(session, ParentSession, GetNestedParentMap(session), obj, securityContext);
			}
			return result;
		}
		public ICollection GetParentObjectsToDelete(Session session) {
			ICollection parentResult = ParentSession.GetObjectsToDelete(true);
			object[] result = new object[parentResult.Count];
			int index = 0;
			SecurityContext securityContext = GetSecurityContext(session);
			foreach(object obj in parentResult) {
				result[index++] = NestedWorksHelper.GetNestedObject(session, ParentSession, GetNestedParentMap(session), obj, securityContext);
			}
			return result;
		}
		public ICollection GetParentTouchedClassInfos(Session session) {
			return ParentSession.GetTouchedClassInfosIncludeParent();
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionObjectLayerCanLoadCollectionObjects")]
#endif
public bool CanLoadCollectionObjects {
			get {
#if SL
				return false;
#else
				return true;
#endif
			}
		}
		public object[] LoadCollectionObjects(Session session, XPMemberInfo refProperty, object owner) {
#if SL
			throw new NotSupportedException();
#else
			object parent = NestedWorksHelper.GetParentObject(session, ParentSession, GetNestedParentMap(session), owner);
			if(parent == null) {
				return new object[0];
			} else {
				XPBaseCollection nestedCollection = (XPBaseCollection)refProperty.GetValue(owner);
				XPBaseCollection parentCollection = (XPBaseCollection)refProperty.GetValue(parent);
				if(parentCollection.IsLoaded && (nestedCollection.IsLoaded || nestedCollection.ClearCount > 0))
					parentCollection.Reload();
				parentCollection.Load();
				object[] parentContent = ListHelper.FromCollection(parentCollection.Helper.IntObjList).ToArray();
				return ListHelper.FromCollection(new NestedLoader(session, ParentSession, GetNestedParentMap(session), GetSecurityContext(session)).GetNestedObjects(new ICollection[] { parentContent })[0]).ToArray();
			}
#endif
		}
		protected virtual bool AllowICommandChannelDoWithSecurityContext { get { return false; } }
		object ICommandChannel.Do(string command, object args) {
			if(securityContextMain != null && !AllowICommandChannelDoWithSecurityContext)
				throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Security_ICommandChannel_TransferringRequestsIsProhibited));
			if(nestedCommandChannel == null) {
				if(ParentSession == null) {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, ParentSession.GetType().FullName));
				}
			}
			return nestedCommandChannel.Do(command, args);
		}
	}
}
