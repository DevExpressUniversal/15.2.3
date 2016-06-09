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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
namespace DevExpress.Persistent.AuditTrail {
	public class QueryCurrentUserNameEventArgs : EventArgs {
		private string currentUserName = "Unknown";
		public string CurrentUserName {
			get { return currentUserName; }
			set { currentUserName = value; }
		}
	}
	public class ConvertObjectToStringEventArgs : EventArgs {
		private object value;
		private string defaultStringRepresentation;
		private string stringRepresentation;
		public ConvertObjectToStringEventArgs(object value, string defaultStringRepresentation) {
			this.value = value;
			this.stringRepresentation = defaultStringRepresentation;
			this.defaultStringRepresentation = defaultStringRepresentation;
		}
		public string StringRepresentation {
			get {
				return stringRepresentation;
			}
			set {
				stringRepresentation = value;
			}
		}
		public object Value {
			get {
				return value;
			}
		}
		public string DefaultStringRepresentation {
			get {
				return defaultStringRepresentation;
			}
		}
	}
	public class CustomFindWeakReferenceEventArgs : HandledEventArgs {
		private object auditedObject;
		private object weakReference;
		public CustomFindWeakReferenceEventArgs(object auditedObject) : base() {
			this.auditedObject = auditedObject;
		}
		public object AuditedObject {
			get { return auditedObject; }
		}
		public object WeakReference {
			get { return weakReference; }
			set { weakReference = value; }
		}
	}
	public delegate void QueryCurrentUserNameEventHandler(object sender, QueryCurrentUserNameEventArgs e);
	public abstract class BaseAuditDataStore {
		private string blobDataString = "Blob data";
		private string nullValueString = "N/A";
		public abstract void Save(Session session, List<AuditDataItem> itemsToSave, IAuditTimestampStrategy timestampStrategy, string currentUserName);
		protected abstract string GetDefaultStringRepresentation(object value);
		protected virtual string GetStringRepresentation(object value) {
			if(ConvertObjectToString != null) {
				ConvertObjectToStringEventArgs eventArgs = new ConvertObjectToStringEventArgs(value, GetDefaultStringRepresentation(value));
				ConvertObjectToString(this, eventArgs);
				return eventArgs.StringRepresentation;
			}
			return GetDefaultStringRepresentation(value);
		}
		protected virtual void OnCustomFindWeakReference(CustomFindWeakReferenceEventArgs args) {
			if(CustomFindWeakReference != null) {
				CustomFindWeakReference(this, args);
			}
		}
		public event EventHandler<ConvertObjectToStringEventArgs> ConvertObjectToString;
		public string BlobDataString {
			get { return blobDataString; }
			set { blobDataString = value; }
		}
		public string NullValueString {
			get { return nullValueString; }
			set { nullValueString = value; }
		}
		public event EventHandler<CustomFindWeakReferenceEventArgs> CustomFindWeakReference;
	}
	public class AuditDataStore<AuditDataStoreItemType, AuditedObjectWeakReferenceType> : BaseAuditDataStore 
		where AuditDataStoreItemType : IAuditDataItemPersistent<AuditedObjectWeakReferenceType> 
		where AuditedObjectWeakReferenceType : BaseAuditedObjectWeakReference {
		public const int MaxLengthOfValue = 1024;
		private object ConvertToXPWeakReference(Session session, object obj) {
			object result = obj;
			IXPSimpleObject iXPSimpleObject = obj as IXPSimpleObject;
			if(iXPSimpleObject != null) {
				result = new XPWeakReference(session, session.GetObjectByKey(obj.GetType(), session.GetKeyValue(obj)));
				((XPWeakReference)result).Save();
			}
			return result;
		}
		public AuditDataStore() {
		}
		public override void Save(Session session, List<AuditDataItem> itemsToSave, IAuditTimestampStrategy timestampStrategy, string currentUserName) {
			UnitOfWork unitOfWork = session.DataLayer != null ? new UnitOfWork(session.DataLayer) : new UnitOfWork(session.ObjectLayer);
			Dictionary<object, AuditedObjectWeakReferenceType> AuditedObjectWeakReferenceCache = new Dictionary<object, AuditedObjectWeakReferenceType>();
			timestampStrategy.OnBeginSaveTransaction(unitOfWork);
			List<IAuditDataItemPersistent<AuditedObjectWeakReferenceType>> storeItemsToSave = new List<IAuditDataItemPersistent<AuditedObjectWeakReferenceType>>(itemsToSave.Count);
			List<AuditDataItem> sortedItemsToSave = new List<AuditDataItem>(itemsToSave);
			sortedItemsToSave.Sort(new AuditDataItemComparer());
			int correctionValue = 0;
			foreach(AuditDataItem item in sortedItemsToSave) {
				IAuditDataItemPersistent<AuditedObjectWeakReferenceType> auditDataStoreItem = CreateAuditDataStoreItem(AuditedObjectWeakReferenceCache, unitOfWork, session, item, currentUserName);
				auditDataStoreItem.ModifiedOn = timestampStrategy.GetTimestamp(item).AddMilliseconds((correctionValue++)/100000.0);
				storeItemsToSave.Add(auditDataStoreItem);
			}
			storeItemsToSave.Sort(new AuditDataStoreItemComparer<AuditedObjectWeakReferenceType>());
			for(int i = 0; i < storeItemsToSave.Count; i++) {
				unitOfWork.Save(storeItemsToSave[i]);
			}
			unitOfWork.CommitChanges();
		}
		protected IAuditDataItemPersistent<AuditedObjectWeakReferenceType> CreateAuditDataStoreItem(Dictionary<object, AuditedObjectWeakReferenceType> AuditedObjectWeakReferenceHash, Session session, Session auditDataItemSession, AuditDataItem auditDataItem, string currentUser) {
			IAuditDataItemPersistent<AuditedObjectWeakReferenceType> auditDataStoreItem = ReflectionHelper.CreateObject<AuditDataStoreItemType>(session); 
			List<string> stringsOfDescription = new List<string>();
			stringsOfDescription.Add(GetStringRepresentation(auditDataItem.OperationType));
			stringsOfDescription.Add(GetStringRepresentation(auditDataItem.AuditObject));
			stringsOfDescription.Add(auditDataItem.MemberInfo == null ? "N/A" : auditDataItem.MemberInfo.Name);
			object oldValue = ConvertToXPWeakReference(session, auditDataItem.OldValue);
			string stringOldValue = GetStringRepresentation(auditDataItem.OldValue);
			stringsOfDescription.Add(stringOldValue);
			object newValue = ConvertToXPWeakReference(session, auditDataItem.NewValue);
			string stringNewValue = GetStringRepresentation(auditDataItem.NewValue);
			stringsOfDescription.Add(stringNewValue);
			string descriptionString = string.Join("; ", stringsOfDescription.ToArray());
			int maxDescriptionLength = MaxLengthOfValue << 1;
			if(descriptionString.Length > maxDescriptionLength) {
				descriptionString = descriptionString.Substring(0, maxDescriptionLength);
			}
			auditDataStoreItem.Description = descriptionString;
			bool isDisposed = (auditDataItem.AuditObject is IXPInvalidateableObject) && ((IXPInvalidateableObject)auditDataItem.AuditObject).IsInvalidated;
			if(!isDisposed && auditDataItemSession.IsNewObject(auditDataItem.AuditObject)) {
				auditDataItemSession.Reload(auditDataItem.AuditObject);
				if(auditDataItemSession.IsNewObject(auditDataItem.AuditObject)) {
					throw new Exception("It's impossible to store audit information until object is saved");
				}
			}
			AuditedObjectWeakReferenceType auditObjectWR;
			if(!AuditedObjectWeakReferenceHash.TryGetValue(auditDataItem.AuditObject, out auditObjectWR)) {
				Object currentSessionAuditObject = session.GetObjectByKey(auditDataItem.AuditObject.GetType(), auditDataItemSession.GetKeyValue(auditDataItem.AuditObject));
				if(currentSessionAuditObject != null) { 
					CustomFindWeakReferenceEventArgs args = new CustomFindWeakReferenceEventArgs(currentSessionAuditObject);
					OnCustomFindWeakReference(args);
					if(args.Handled) {
						auditObjectWR = args.WeakReference as AuditedObjectWeakReferenceType;
					}
					else {
						auditObjectWR = session.FindObject<AuditedObjectWeakReferenceType>(
							new GroupOperator(
								new BinaryOperator("TargetType", session.GetObjectType(currentSessionAuditObject)),
								new BinaryOperator("TargetKey", XPWeakReference.KeyToString(session.GetKeyValue(currentSessionAuditObject)))
							));
					}
					if(auditObjectWR == null) {
						auditObjectWR = ReflectionHelper.CreateObject<AuditedObjectWeakReferenceType>(session, currentSessionAuditObject);
						auditObjectWR.DisplayName = GetStringRepresentation(currentSessionAuditObject);
					}
					AuditedObjectWeakReferenceHash[auditDataItem.AuditObject] = auditObjectWR;
				}
			}
			auditDataStoreItem.AuditedObject = auditObjectWR;
			auditDataStoreItem.ModifiedOn = auditDataItem.ModifiedOn;
			auditDataStoreItem.OldObject = oldValue as XPWeakReference;
			auditDataStoreItem.NewObject = newValue as XPWeakReference;
			auditDataStoreItem.OldValue = stringOldValue;
			auditDataStoreItem.NewValue = stringNewValue;
			auditDataStoreItem.UserName = currentUser;
			auditDataStoreItem.OperationType = GetStringRepresentation(auditDataItem.OperationType);
			auditDataStoreItem.PropertyName = auditDataItem.MemberInfo == null ? "" : auditDataItem.MemberInfo.Name;
			return auditDataStoreItem;
		}
		protected override string GetDefaultStringRepresentation(object value) {
			if(value == null) {
				return NullValueString;
			}
			if(value is XPWeakReference) {
				if(!(value as XPWeakReference).IsAlive) {
					return NullValueString;
				}
				return (value as XPWeakReference).Target.ToString();
			}
			ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(value.GetType());
			IMemberInfo defaultMember = (ti != null) ? ti.DefaultMember : null;
			string result;
			if(defaultMember != null) {
				object memberValue = defaultMember.GetValue(value);
				if(memberValue == null) {
					result = NullValueString;
				}
				else {
					result = memberValue.ToString();
				}
			}
			else {
				result = value.ToString();
			}
			if(result.Length > MaxLengthOfValue) {
				result = BlobDataString;
			}
			return result;
		}
		#region Obsolete 8.2
		[Obsolete("Use GetDefaultStringRepresentation instead", true)]
		protected virtual string GetStringValue(object value) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
