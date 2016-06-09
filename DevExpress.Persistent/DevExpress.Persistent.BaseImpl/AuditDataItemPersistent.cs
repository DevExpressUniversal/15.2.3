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
using DevExpress.Data.Filtering;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[ImageName("BO_Audit_ChangeHistory")]
	public class AuditDataItemPersistent : BaseObject, IAuditDataItemPersistent<AuditedObjectWeakReference> {
		private DateTime modifiedOn;
		private string operationType;
		private XPWeakReference newObject;
		private XPWeakReference oldObject;
		private AuditedObjectWeakReference auditedObject;
		private string userName;
		private string propertyName;
		[Indexed]
		public string UserName {
			get { return userName; }
			set { SetPropertyValue("UserName", ref userName, value); }
		}
		[Indexed]
		public DateTime ModifiedOn {
			get { return modifiedOn; }
			set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
		}
		[Indexed]
		public string OperationType {
			get { return operationType; }
			set { SetPropertyValue("OperationType", ref operationType, value); }
		}
		[Size(2048), Delayed, MemberDesignTimeVisibility(true)]
		public string Description {
			get { return GetDelayedPropertyValue<string>("Description"); }
			set { SetDelayedPropertyValue<string>("Description", value); }
		}
		[Association("AuditedObjectWeakReference-AuditDataItemPersistent"), MemberDesignTimeVisibility(false)]
		public AuditedObjectWeakReference AuditedObject {
			get { return auditedObject; }
			set { SetPropertyValue("AuditedObject", ref auditedObject, value); }
		}
		[Aggregated, MemberDesignTimeVisibility(false)]
		public XPWeakReference OldObject {
			get { return oldObject; }
			set { SetPropertyValue("OldObject", ref oldObject, value); }
		}
		[Aggregated, MemberDesignTimeVisibility(false)]
		public XPWeakReference NewObject {
			get { return newObject; }
			set { SetPropertyValue("NewObject", ref newObject, value); }
		}
		[Delayed, Size(1024)]
		public string OldValue {
			get { return GetDelayedPropertyValue<string>("OldValue"); }
			set { SetDelayedPropertyValue<string>("OldValue", value); }
		}
		[Delayed, Size(1024)]
		public string NewValue {
			get { return GetDelayedPropertyValue<string>("NewValue"); }
			set { SetDelayedPropertyValue<string>("NewValue", value); }
		}
		public string PropertyName {
			get { return propertyName; }
			set { SetPropertyValue("PropertyName", ref propertyName, value); }
		}
		public AuditDataItemPersistent(Session session, string userName, DateTime modifiedOn, string description)
			: base(session) {
			this.userName = userName;
			this.modifiedOn = modifiedOn;
			this.Description = description;
		}
		public AuditDataItemPersistent(Session session) : base(session) { }
	}
	public class AuditedObjectWeakReference : BaseAuditedObjectWeakReference {
		public AuditedObjectWeakReference(Session session) : base(session) { }
		public AuditedObjectWeakReference(Session session, object target) : base(session, target) { }
		[Association("AuditedObjectWeakReference-AuditDataItemPersistent")]
		public XPCollection<AuditDataItemPersistent> AuditDataItems {
			get { return GetCollection<AuditDataItemPersistent>("AuditDataItems"); }
		}
		static public XPCollection<AuditDataItemPersistent> GetAuditTrail(Session session, object obj) {
			AuditedObjectWeakReference auditObjectWR = session.FindObject<AuditedObjectWeakReference>(
				new GroupOperator(
					new BinaryOperator("TargetType", session.GetObjectType(obj)),
					new BinaryOperator("TargetKey", AuditedObjectWeakReference.KeyToString(session.GetKeyValue(obj)))
				));
			if(auditObjectWR != null) {
				auditObjectWR.AuditDataItems.BindingBehavior = CollectionBindingBehavior.AllowNone;
				return auditObjectWR.AuditDataItems;
			}
			else
				return null;
		}
	}
}
