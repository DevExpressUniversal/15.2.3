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
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.Persistent.AuditTrail {
	public class AuditDataItemComparer : IComparer<AuditDataItem> {
		public int Compare(AuditDataItem x, AuditDataItem y) {
			int compareResult = x.ModifiedOn.CompareTo(y.ModifiedOn);
			if(compareResult == 0) {
				compareResult = x.OperationType.CompareTo(y.OperationType);
			}
			return compareResult;
		}
	}
	public class AuditDataItem {
		private object auditObject;
		public object AuditObject {
			get { return auditObject; }
			set { auditObject = value; }
		}
		private XPMemberInfo memberInfo;
		public XPMemberInfo MemberInfo {
			get { return memberInfo; }
			set { memberInfo = value; }
		}
		private object oldValue;
		public object OldValue {
			get { return oldValue; }
			set { oldValue = value; }
		}
		private object newValue;
		public object NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
		private AuditOperationType operationType;
		public AuditOperationType OperationType {
			get { return operationType; }
			set { operationType = value; }
		}
		private DateTime modifiedOn;
		public DateTime ModifiedOn {
			get { return modifiedOn; }
		}
		public AuditDataItem(object auditObject, XPMemberInfo memberInfo, object oldValue, object newValue, AuditOperationType operationType) {
			this.auditObject = auditObject;
			this.memberInfo = memberInfo;
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.operationType = operationType;
			this.modifiedOn = DateTime.Now;
		}
		public override string ToString() {
			string result = "Object: ";
			if(auditObject != null) {
				result += auditObject.GetType().Name;
			}
			else {
				result += "null";
			}
			result += " " + operationType.ToString();
			return result;
		}
	}
}
