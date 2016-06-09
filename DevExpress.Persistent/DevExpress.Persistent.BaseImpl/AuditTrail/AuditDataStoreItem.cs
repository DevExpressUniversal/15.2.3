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
using DevExpress.Xpo;
namespace DevExpress.Persistent.AuditTrail {
	public interface IBaseAuditDataItemPersistent {
	}
	public interface IAuditDataItemPersistent<AuditedObjectWeakReferenceType> : IBaseAuditDataItemPersistent {
		string UserName { get;set;}
		DateTime ModifiedOn { get;set;}
		string OperationType { get;set;}
		string Description { get;set;}
		AuditedObjectWeakReferenceType AuditedObject { get;set;}
		XPWeakReference OldObject { get;set;}
		XPWeakReference NewObject { get;set;}
		string OldValue { get;set;}
		string NewValue { get;set;}
		string PropertyName { get;set;}
	}
	public class AuditDataStoreItemComparer<AuditedObjectWeakReferenceType> : IComparer<IAuditDataItemPersistent<AuditedObjectWeakReferenceType>> {
		#region IComparer<IAuditDataItem> Members
		public int Compare(IAuditDataItemPersistent<AuditedObjectWeakReferenceType> x, IAuditDataItemPersistent<AuditedObjectWeakReferenceType> y) {
			int compareResult = x.ModifiedOn.CompareTo(y.ModifiedOn);
			if(compareResult == 0) {
				if(x.OperationType != null) {
					compareResult = x.OperationType.CompareTo(y.OperationType);
				}
				else {
					return -1;
				}
			}
			return compareResult;
		}
		#endregion
	}
	[DefaultProperty("DisplayName"), NonPersistent]
	public class BaseAuditedObjectWeakReference : XPWeakReference {
		private const int displayNameSize = 250;
		public BaseAuditedObjectWeakReference(Session session)
			: base(session) {
		}
		public BaseAuditedObjectWeakReference(Session session, object target)
			: base(session, target) {
			OnSetTarget();
		}
		private string displayName = "";
		[Size(displayNameSize)] 
		public string DisplayName {
			get { return displayName; }
			set {
				string val = value;
				if(!IsLoading && !string.IsNullOrEmpty(val) && val.Length > displayNameSize) {
					val = val.Substring(0, displayNameSize);
				}
				SetPropertyValue("DisplayName", ref displayName, val);
			}
		}
		protected override void OnChanged(string propertyName, object oldValue, object newValue) {
			base.OnChanged(propertyName, oldValue, newValue);
			if(propertyName == "Target") {
				OnSetTarget();
			}
		}
		private void OnSetTarget() {
			if(Target != null) {
				object keyValue = Session.GetKeyValue(Target);
				guidId = null;
				if(keyValue is Guid) {
					guidId = (Guid?)keyValue;
				}
				intId = null;
				if(keyValue is int) {
					intId = (int?)keyValue;
				}
			}
		}
#if MediumTrust
		private Guid? guidId;
		public Guid? GuidId {
			get { return guidId; }
			set { SetPropertyValue("GuidId", ref guidId, value); }
		}
		private int? intId;
		public int? IntId {
			get { return intId; }
			set { SetPropertyValue("IntId", ref intId, value); }
		}
#else
		[Persistent("GuidId")]
		private Guid? guidId;
		[PersistentAlias("guidId")]
		public Guid? GuidId {
			get { return guidId; }
		}
		[Persistent("IntId")]
		private int? intId;
		[PersistentAlias("intId")]
		public int? IntId {
			get { return intId; }
		}
#endif
	}
}
