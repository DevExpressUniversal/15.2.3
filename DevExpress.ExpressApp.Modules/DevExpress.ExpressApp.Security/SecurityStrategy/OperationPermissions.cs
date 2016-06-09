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
using DevExpress.ExpressApp.Utils;
using System.Diagnostics;
using DevExpress.Data.Filtering;
namespace DevExpress.ExpressApp.Security {
	public class IsAdministratorPermission : IOperationPermission {
		public string Operation {
			get { return ""; }
		}
	}
	public abstract class OperationPermissionBase : IOperationPermission {
		public OperationPermissionBase(string operation) {
			Guard.ArgumentNotNullOrEmpty(operation, "operation");
			if(!GetSupportedOperations().Contains(operation)) {
				Guard.CreateArgumentOutOfRangeException(operation, "operation");
			}
			this.Operation = operation;
		}
		public string Operation { get; private set; }
		public abstract IList<string> GetSupportedOperations();
	}
	[DebuggerDisplay("TypeOperationPermission, Type = {ObjectType.FullName}, {Operation}")]
	public class TypeOperationPermission : OperationPermissionBase, ITypeOperationPermission {
		public TypeOperationPermission(Type objectType, string operation)
			: base(operation) {
			Guard.ArgumentNotNull(objectType, "objectType");
			this.ObjectType = objectType;
		}
		public override IList<string> GetSupportedOperations() {
			return new string[] { SecurityOperations.Read, SecurityOperations.Write, SecurityOperations.Create, SecurityOperations.Delete, SecurityOperations.Navigate };
		}
		public Type ObjectType { get; private set; }
	}
	[DebuggerDisplay("NavigateOperationPermission, TargetID = {TargetID}, {Operation}")]
	public class NavigateOperationPermission : IOperationPermission {
		public NavigateOperationPermission(string targetID) {
			Guard.ArgumentNotNullOrEmpty(targetID, "targetID");
			TargetID = targetID;
		}
		public string TargetID { get; set; }
		public string Operation {
			get { return "Navigate"; }
		}
	}
	[DebuggerDisplay("MemberOperationPermission, Type = {ObjectType.FullName}, Member = {MemberName}, {Operation}")]
	public class MemberOperationPermission : OperationPermissionBase, ITypeOperationPermission {
		public MemberOperationPermission(Type objectType, string memberName, string operation)
			: base(operation) {
			Guard.ArgumentNotNull(objectType, "objectType");
			Guard.ArgumentNotNullOrEmpty(memberName, "memberName");
			this.ObjectType = objectType;
			this.MemberName = memberName;
		}
		public override IList<string> GetSupportedOperations() {
			return new string[] { SecurityOperations.Read, SecurityOperations.Write };
		}
		public Type ObjectType { get; private set; }
		public string MemberName { get; private set; }
	}
	[DebuggerDisplay("MemberCriteriaOperationPermission, Type = {ObjectType.FullName}, Member = {MemberName}, {Operation}, Criteria = {Criteria}")]
	public class MemberCriteriaOperationPermission : OperationPermissionBase, ITypeOperationPermission {
		public MemberCriteriaOperationPermission(Type objectType, string memberName, string criteria, string operation)
			: base(operation) {
			Guard.ArgumentNotNull(criteria, "criteria");
			Guard.ArgumentNotNull(objectType, "objectType");
			Guard.ArgumentNotNullOrEmpty(memberName, "memberName");
			this.ObjectType = objectType;
			this.MemberName = memberName;
			this.Criteria = criteria;
		}
		public override IList<string> GetSupportedOperations() {
			return new string[] { SecurityOperations.Read, SecurityOperations.Write };
		}
		public Type ObjectType { get; private set; }
		public string MemberName { get; private set; }
		public string Criteria { get; private set; }
	}
	public class ModelOperationPermission : OperationPermissionBase {
		public ModelOperationPermission()
			: base(SecurityOperations.Write) {
		}
		public override IList<string> GetSupportedOperations() {
			return new string[] { SecurityOperations.Write };
		}
	}
	[DebuggerDisplay("ObjectOperationPermission, Type = {ObjectType.FullName}, Criteria = {Criteria}")]
	public class ObjectOperationPermission : OperationPermissionBase, ITypeOperationPermission {
		private Type objectType;
		private string criteria;
		public ObjectOperationPermission(Type objectType, string criteria, string operation)
			: base(operation) {
			Guard.ArgumentNotNull(objectType, "objectType");
			this.objectType = objectType;
			this.criteria = criteria;
		}
		public override IList<string> GetSupportedOperations() {
			return new string[] { SecurityOperations.Read, SecurityOperations.Write, SecurityOperations.Delete, SecurityOperations.Navigate };
		}
		public Type ObjectType {
			get { return objectType; }
		}
		public string Criteria {
			get {
				return criteria;
			}
		}
	}
}
