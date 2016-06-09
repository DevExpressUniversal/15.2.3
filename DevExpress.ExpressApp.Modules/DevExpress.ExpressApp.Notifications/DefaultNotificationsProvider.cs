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

using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.ExpressApp.Notifications {
	public class DefaultNotificationsProvider : INotificationsProvider, IDisposable {
		private XafApplication application;
		protected IDictionary<Type, IObjectSpace> notificationObjectSpaces;
		private ITypeInfo GetNotificationSourceObjectTypeInfo(INotificationItem item) {
			Type notificationSourceWrapperType = item.NotificationSource.GetType(); 
			return XafTypesInfo.Instance.FindTypeInfo(notificationSourceWrapperType);
		}
		private CriteriaOperator GetCustomNotificationsCollectionCriteria(Type type) {
			CustomizeCollectionCriteriaEventArgs args = new CustomizeCollectionCriteriaEventArgs(type);
			if(CustomizeNotificationCollectionCriteria != null) {
				CustomizeNotificationCollectionCriteria(this, args);
			}
			return args.Criteria;
		}
		protected virtual HashSet<ITypeInfo> CollectNotificationObjectTypes() {
			HashSet<ITypeInfo> result = new HashSet<ITypeInfo>();
			foreach(ITypeInfo typeInfo in XafTypesInfo.Instance.FindTypeInfo(typeof(ISupportNotifications)).Implementors) {
				if(typeInfo.IsInterface || !((TypesInfo)XafTypesInfo.Instance).IsRegisteredEntity(typeInfo.Type)) {
					continue;
				}
				if(!typeInfo.HasDescendants) {
					result.Add(typeInfo);
				}
			}
			return result;
		}
		protected virtual IList<ISupportNotifications> CreateNotificationObjectsCollection(CriteriaOperator defaultCriteria) {
			IList allNotificationObjects = new ArrayList();
			IList<ISupportNotifications> notificationObjects = new List<ISupportNotifications>();
			foreach(ITypeInfo typeInfo in NotificationTypesInfo) {
				if(!typeInfo.Implements<ISupportNotifications>()) {
					continue;
				}
				IObjectSpace objectSpace;
				if(notificationObjectSpaces.TryGetValue(typeInfo.Type, out objectSpace)) {
					objectSpace.Refresh();
				}
				else {
					objectSpace = application.CreateObjectSpace(typeInfo.Type);
					notificationObjectSpaces.Add(typeInfo.Type, objectSpace);
				}
				CriteriaOperator criteria = CriteriaOperator.And(defaultCriteria, GetCustomNotificationsCollectionCriteria(typeInfo.Type));
				IList currentTypeCollection = objectSpace.CreateCollection(typeInfo.Type, criteria);
				if(currentTypeCollection != null) {
					allNotificationObjects = CollectionsHelper.MergeCollections(currentTypeCollection, allNotificationObjects);
				}
			}
			foreach(ISupportNotifications notificationObject in allNotificationObjects) {
				notificationObjects.Add(notificationObject);
			}
			return notificationObjects;
		}
		protected virtual void Dismiss(INotificationItem item) {
			item.NotificationSource.AlarmTime = null;
			item.NotificationSource.IsPostponed = false;
		}
		protected virtual void Postpone(INotificationItem itemToPostpone, TimeSpan postponeTime) {
			itemToPostpone.NotificationSource.AlarmTime = DateTime.Now + postponeTime;
			itemToPostpone.NotificationSource.IsPostponed = true;
		}
		public DefaultNotificationsProvider(XafApplication application) {
			Guard.ArgumentNotNull(application, "application");
			notificationObjectSpaces = new Dictionary<Type, IObjectSpace>();
			this.application = application;
			NotificationTypesInfo = CollectNotificationObjectTypes();
		}
		public void Dispose() {
			if(NotificationTypesInfo != null) {
				NotificationTypesInfo.Clear();
				NotificationTypesInfo = null;
			}
			if(notificationObjectSpaces != null) {
				foreach(IObjectSpace objectSpace in notificationObjectSpaces.Values) {
					objectSpace.Dispose();
				}
				notificationObjectSpaces.Clear();
				notificationObjectSpaces = null;
			}
			CustomizeNotificationCollectionCriteria = null;
		}
		public IList<INotificationItem> GetNotificationItems() {
			IList<INotificationItem> result = new List<INotificationItem>();
			CriteriaOperator activeCriteria = CriteriaOperator.And(!new NullOperator("AlarmTime"), new BinaryOperator("AlarmTime", DateTime.Now, BinaryOperatorType.LessOrEqual));
			CriteriaOperator criteria = CriteriaOperator.Or(new BinaryOperator("IsPostponed", true), activeCriteria);
			IList<ISupportNotifications> notificationObjects = CreateNotificationObjectsCollection(criteria);
			foreach(ISupportNotifications notificationObject in notificationObjects) {
				result.Add(new CommonNotificationItem(notificationObject));
			}
			return result;
		}
		public int GetActiveNotificationsCount() {
			int result = 0;
			foreach(ITypeInfo typeInfo in NotificationTypesInfo) {
				if(typeInfo.Implements<ISupportNotifications>()) {
					using(IObjectSpace objectSpace = application.CreateObjectSpace()) {
						CriteriaOperator customCriteria = GetCustomNotificationsCollectionCriteria(typeInfo.Type);
						CriteriaOperator criteria = CriteriaOperator.And(!new NullOperator("AlarmTime"), new BinaryOperator("AlarmTime", DateTime.Now, BinaryOperatorType.LessOrEqual));
						result += objectSpace.GetObjectsCount(typeInfo.Type, CriteriaOperator.And(criteria, customCriteria));
					}
				}
			}
			return result;
		}
		public int GetPostponedNotificationsCount() {
			int result = 0;
			CriteriaOperator notActiveCriteria = CriteriaOperator.Or(new NullOperator("AlarmTime"), new BinaryOperator("AlarmTime", DateTime.Now, BinaryOperatorType.Greater));
			CriteriaOperator criteria = CriteriaOperator.And(new BinaryOperator("IsPostponed", true), notActiveCriteria);
			if(NotificationTypesInfo != null) {
				foreach(ITypeInfo typeInfo in NotificationTypesInfo) {
					if(typeInfo.Implements<ISupportNotifications>()) {
						using(IObjectSpace objectSpace = application.CreateObjectSpace()) {
							CriteriaOperator resultCriteria = CriteriaOperator.And(criteria, GetCustomNotificationsCollectionCriteria(typeInfo.Type));
							result += objectSpace.GetObjectsCount(typeInfo.Type, resultCriteria);
						}
					}
				}
			}
			return result;
		}
		public void Dismiss(IEnumerable<INotificationItem> notificationItems) {
			IList<IObjectSpace> objectSpacesToCommit = new List<IObjectSpace>();
			foreach(INotificationItem notificationItem in notificationItems) {
				ITypeInfo typeInfo = GetNotificationSourceObjectTypeInfo(notificationItem);
				if(NotificationTypesInfo.Contains(typeInfo) && typeInfo.Implements<ISupportNotifications>()) {
					Dismiss(notificationItem);
					if(notificationObjectSpaces.Keys.Contains(GetNotificationSourceObjectTypeInfo(notificationItem).Type)) {
						objectSpacesToCommit.Add(notificationObjectSpaces[GetNotificationSourceObjectTypeInfo(notificationItem).Type]);
					}
				}
			}
			foreach(IObjectSpace objectspace in objectSpacesToCommit) {
				objectspace.CommitChanges();
			}
		}
		public void Postpone(IEnumerable<INotificationItem> notificationItems, TimeSpan postponeTime) {
			IList<IObjectSpace> objectSpacesToCommit = new List<IObjectSpace>();
			foreach(INotificationItem notificationItem in notificationItems) {
				ITypeInfo typeInfo = GetNotificationSourceObjectTypeInfo(notificationItem);
				if(NotificationTypesInfo.Contains(typeInfo) && typeInfo.Implements<ISupportNotifications>()) {
					Postpone(notificationItem, postponeTime);
					if(notificationObjectSpaces.Keys.Contains(GetNotificationSourceObjectTypeInfo(notificationItem).Type)) {
						objectSpacesToCommit.Add(notificationObjectSpaces[GetNotificationSourceObjectTypeInfo(notificationItem).Type]);
					}
				}
			}
			foreach(IObjectSpace objectspace in objectSpacesToCommit) {
				objectspace.CommitChanges();
			}
		}
		public HashSet<ITypeInfo> NotificationTypesInfo {
			get;
			private set;
		}
		public event EventHandler<CustomizeCollectionCriteriaEventArgs> CustomizeNotificationCollectionCriteria;
	}
	[DomainComponent]
	public class CommonNotificationItem : INotificationItem {
		ISupportNotifications notificationObject;
		public CommonNotificationItem(ISupportNotifications notificationObject) {
			Guard.ArgumentNotNull(notificationObject, "notificationObject");
			this.notificationObject = notificationObject;
		}
		public ISupportNotifications NotificationSource { get { return notificationObject; } }
	}
}
