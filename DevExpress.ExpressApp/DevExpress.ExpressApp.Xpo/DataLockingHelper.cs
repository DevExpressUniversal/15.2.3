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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.Xpo {
	public static class DataLockingHelper {
		public static DataLockingInfo GetDataLockingInfo(Session session) {
			Guard.ArgumentNotNull(session, "session");
			if(session.LockingOption == LockingOption.None) {
				return DataLockingInfo.Empty;
			}
			if(!session.TrackPropertiesModifications) {
				throw new ArgumentException("!session.TrackPropertiesModifications", "session"); 
			}
			List<ObjectLockingInfo> objectLockingInfoList = new List<ObjectLockingInfo>();
			IDictionary<XPClassInfo, List<Object>> objectToSaveKeysByClassInfo = GetObjectToSaveKeys(session);
			foreach(KeyValuePair<XPClassInfo, List<Object>> item in objectToSaveKeysByClassInfo) {
				XPClassInfo classInfo = item.Key;
				List<Object> objectToSaveKeys = item.Value;
				objectLockingInfoList.AddRange(CollectObjectLockingInfo(session, classInfo, objectToSaveKeys));
			}
			return new DataLockingInfo(objectLockingInfoList.ToArray());
		}
		private static IDictionary<XPClassInfo, List<Object>> GetObjectToSaveKeys(Session session) {
			IDictionary<XPClassInfo, List<Object>> objectToSaveKeysByClassInfo = new Dictionary<XPClassInfo, List<Object>>();
			ICollection objectsToSave = session.GetObjectsToSave();
			foreach(Object objectToSave in objectsToSave) {
				if(session.IsNewObject(objectToSave)) {
					continue;
				}
				XPClassInfo classInfo = session.GetClassInfo(objectToSave);
				List<Object> objectToSaveKeys;
				if(!objectToSaveKeysByClassInfo.TryGetValue(classInfo, out objectToSaveKeys)) {
					objectToSaveKeys = new List<Object>();
					objectToSaveKeysByClassInfo.Add(classInfo, objectToSaveKeys);
				}
				Object key = session.GetKeyValue(objectToSave);
				objectToSaveKeys.Add(key);
			}
			return objectToSaveKeysByClassInfo;
		}
		private static IEnumerable<ObjectLockingInfo> CollectObjectLockingInfo(Session session, XPClassInfo classInfo, List<Object> objectToSaveKeys) {
			List<ObjectLockingInfo> result = new List<ObjectLockingInfo>();
			IEnumerable<ViewRecord> view = GetDataFromDataLayer(session, classInfo, objectToSaveKeys);
			foreach(ViewRecord record in view) {
				Object objectToSave = session.GetLoadedObjectByKey(classInfo, record[classInfo.KeyProperty.Name]);
				Boolean isLocked;
				if(classInfo.OptimisticLockingBehavior == OptimisticLockingBehavior.ConsiderOptimisticLockingField) {
					Object valueInSession = classInfo.OptimisticLockFieldInDataLayer.GetValue(objectToSave);
					Object valueInDataLayer = record[classInfo.OptimisticLockFieldName];
					isLocked = !AreEqual(valueInDataLayer, valueInSession);
				}
				else {
					isLocked = IsLocked(session, classInfo, objectToSave, record);
				}
				if(isLocked) {
					Boolean canMerge = CanMerge(session, classInfo, objectToSave, record);
					result.Add(new ObjectLockingInfo(objectToSave, canMerge));
				}
			}
			return result;
		}
		private static IEnumerable<ViewRecord> GetDataFromDataLayer(Session session, XPClassInfo classInfo, List<Object> objectToSaveKeys) {
			const int MaxObjectsInView = 1000;
			CriteriaOperatorCollection properties = new CriteriaOperatorCollection();
			foreach(XPMemberInfo persistentProperty in classInfo.PersistentProperties) {
				properties.Add(new OperandProperty(persistentProperty.Name));
			}
			int currentIndex = 0;
			while(currentIndex < objectToSaveKeys.Count) {
				int restCount = objectToSaveKeys.Count - currentIndex;
				if(restCount > MaxObjectsInView) {
					restCount = MaxObjectsInView;
				}
				IEnumerable<Object> keysPortion = objectToSaveKeys.GetRange(currentIndex, restCount);
				currentIndex = currentIndex + restCount;
				CriteriaOperator criteria = new InOperator(classInfo.KeyProperty.Name, keysPortion);
				XPView view = new XPView(session, classInfo, properties, criteria);
				Boolean inTransactionMode = Session.InTransactionMode;
				try {
					Session.InTransactionMode = false;
					int count = view.Count; 
				}
				finally {
					Session.InTransactionMode = inTransactionMode;
				}
				foreach(ViewRecord record in view) {
					yield return record;
				}
			}
		}
		private static Boolean IsLocked(Session session, XPClassInfo classInfo, Object objectToSave, ViewRecord record) {
			foreach(XPMemberInfo persistentProperty in classInfo.PersistentProperties) {
				if(IsMemberLoaded(objectToSave, persistentProperty) && HasServerSideModifications(session, objectToSave, record, persistentProperty)) {
					return true;
				}
			}
			return false;
		}
		private static Boolean CanMerge(Session session, XPClassInfo classInfo, Object objectToSave, ViewRecord record) {
			foreach(XPMemberInfo persistentProperty in classInfo.PersistentProperties) {
				if(IsMemberLoaded(objectToSave, persistentProperty) && HasClientSideModifications(objectToSave, persistentProperty) && HasServerSideModifications(session, objectToSave, record, persistentProperty)) {
					return false;
				}
			}
			return true;
		}
		private static Boolean IsMemberLoaded(Object obj, XPMemberInfo memberInfo) {
			Boolean result;
			if(memberInfo.IsDelayed) {
				XPDelayedProperty delayedPropertyContainer = GetDelayedPropertyContainer(obj, memberInfo);
				result = delayedPropertyContainer.IsLoaded;
			}
			else {
				result = true;
			}
			return result;
		}
		private static XPDelayedProperty GetDelayedPropertyContainer(Object obj, XPMemberInfo memberInfo) {
			XPDelayedProperty result;
			DelayedAttribute delayedAttribute = (DelayedAttribute)memberInfo.GetAttributeInfo(typeof(DelayedAttribute));
			if(String.IsNullOrEmpty(delayedAttribute.FieldName) && obj is IXPCustomPropertyStore) {
				result = (XPDelayedProperty)((IXPCustomPropertyStore)obj).GetCustomPropertyValue(memberInfo);
			}
			else {
				XPMemberInfo fi = memberInfo.Owner.GetMember(delayedAttribute.FieldName);
				result = (XPDelayedProperty)fi.GetValue(obj);
			}
			return result;
		}
		private static Boolean HasClientSideModifications(Object objectToSave, XPMemberInfo persistentProperty) {
			return persistentProperty.GetModified(objectToSave);
		}
		private static Boolean HasServerSideModifications(Session session, Object objectToSave, ViewRecord record, XPMemberInfo persistentProperty) {
			Boolean hasClientSideModifications = persistentProperty.GetModified(objectToSave);
			Object valueOnLoad = hasClientSideModifications ? persistentProperty.GetOldValue(objectToSave) : persistentProperty.GetValue(objectToSave);
			Object storedValueOnLoad = GetStoredValue(session, persistentProperty, valueOnLoad);
			Object valueInDataLayer = record[persistentProperty.Name];
			return !AreEqual(valueInDataLayer, storedValueOnLoad);
		}
		private static Object GetStoredValue(Session session, XPMemberInfo memberInfo, Object memberValue) {
			if(memberValue == null) {
				return null;
			}
			if(memberInfo.IsKey) {
				return memberInfo.ExpandId(memberValue);
			}
			if(memberInfo.ReferenceType != null && session.Dictionary.QueryClassInfo(memberValue) != null) {
				return memberInfo.ReferenceType.GetId(memberValue);
			}
			if(memberInfo.Converter is UtcDateTimeConverter) {
				DateTime dateTime = (DateTime)memberValue;
				if(dateTime.Kind != DateTimeKind.Local) {
					return dateTime.ToLocalTime();
				}
			}
			return memberValue;
		}
		private static Boolean AreEqual(Object valueInDataLayer, Object valueInSession) {
			if(valueInDataLayer == null && valueInSession != null) {
				if(valueInSession.GetType().IsValueType) {
					return valueInSession.Equals(GetDefaultValue(valueInSession.GetType()));
				}
				else {
					return false;
				}
			}
			if(valueInDataLayer is DateTime && valueInSession is DateTime) {
				return AreEqual((DateTime)valueInDataLayer, (DateTime)valueInSession);
			}
			if(valueInDataLayer is Byte[] && valueInSession is Byte[]) {
				return AreEqual((Byte[])valueInDataLayer, (Byte[])valueInSession);
			}
			return EvalHelpers.CompareObjects(valueInDataLayer, valueInSession, true, true, null) == 0;
		}
		private static Object GetDefaultValue(Type type) {
			return Activator.CreateInstance(type);
		}
		private static Boolean AreEqual(DateTime dateTimeA, DateTime dateTimeB) {
			if(dateTimeA == dateTimeB) {
				return true;
			}
			TimeSpan difference = dateTimeA - dateTimeB;
			return Math.Abs(difference.TotalMilliseconds) < 10;
		}
		private static Boolean AreEqual(Byte[] arrayA, Byte[] arrayB) {
			if(arrayA == arrayB) {
				return true;
			}
			if(arrayA.Length != arrayB.Length) {
				return false;
			}
			for(int i = 0; i < arrayA.Length; i++) {
				if(arrayA[i] != arrayB[i]) {
					return false;
				}
			}
			return true;
		}
		public static void MergeData(Session session, DataLockingInfo dataLockingInfo) {
			Guard.ArgumentNotNull(session, "session");
			Guard.ArgumentNotNull(dataLockingInfo, "dataLockingInfo");
			if(!dataLockingInfo.CanMerge) {
				throw new ArgumentException("!dataLockingInfo.CanMerge", "dataLockingInfo"); 
			}
			ReloadLockedObjects(session, dataLockingInfo, OptimisticLockingReadBehavior.MergeCollisionThrowException);
		}
		public static void RefreshData(Session session, DataLockingInfo dataLockingInfo) {
			Guard.ArgumentNotNull(session, "session");
			Guard.ArgumentNotNull(dataLockingInfo, "dataLockingInfo");
			ReloadLockedObjects(session, dataLockingInfo, OptimisticLockingReadBehavior.ReloadObject);
		}
		private static void ReloadLockedObjects(Session session, DataLockingInfo dataLockingInfo, OptimisticLockingReadBehavior optimisticLockingReadBehavior) {
			if(dataLockingInfo.IsLocked) {
				Boolean inTransactionMode = Session.InTransactionMode;
				OptimisticLockingReadBehavior behavior = session.OptimisticLockingReadBehavior;
				try {
					Session.InTransactionMode = false;
					session.OptimisticLockingReadBehavior = optimisticLockingReadBehavior;
					foreach(ObjectLockingInfo info in dataLockingInfo.ObjectLockingInfo) {
						session.Reload(info.LockedObject);
					}
				}
				finally {
					Session.InTransactionMode = inTransactionMode;
					session.OptimisticLockingReadBehavior = behavior;
				}
			}
		}
	}
}
