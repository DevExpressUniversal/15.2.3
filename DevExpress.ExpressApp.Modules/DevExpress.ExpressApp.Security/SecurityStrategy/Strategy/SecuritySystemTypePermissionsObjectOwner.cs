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

using DevExpress.ExpressApp.Security.Strategy.PermissionMatrix;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Security.Strategy {
	[System.ComponentModel.DisplayName("Type Operation Permissions Owner")]
	[Persistent("SecuritySystemRole")] 
	public class SecuritySystemTypePermissionsObjectOwner : XPCustomObject, IPermissionMatrixTypePermissionsOwner, IOperationPermissionProvider {
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
		[Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
		private Guid oid = Guid.Empty;
		[PersistentAlias("oid"), Browsable(false)]
		public Guid Oid {
			get { return oid; }
		}
#endif
		private TypePermissionMatrix matrix;
		protected virtual IEnumerable<IOperationPermissionProvider> GetChildrenCore() {
			return new IOperationPermissionProvider[0];
		}
		protected virtual IEnumerable<IOperationPermission> GetPermissionsCore() {
			List<IOperationPermission> result = new List<IOperationPermission>();
			foreach(SecuritySystemTypePermissionObject persistentPermission in TypePermissions) {
				result.AddRange(persistentPermission.GetPermissions());
			}
			return result;
		}
		protected override void OnLoading() {
			matrix = null;
			base.OnLoading();
		}
		public SecuritySystemTypePermissionsObjectOwner(Session session)
			: base(session) {
		}
		IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions() {
			return GetPermissionsCore();
		}
		IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren() {
			return GetChildrenCore();
		}
		[Association]
		[DevExpress.Xpo.Aggregated]
		public XPCollection<SecuritySystemTypePermissionObject> TypePermissions {
			get { return GetCollection<SecuritySystemTypePermissionObject>("TypePermissions"); }
		}
		[DevExpress.ExpressApp.DC.Aggregated, VisibleInDetailView(false)]
		[System.ComponentModel.DisplayName("Data Access Permissions")]
		public IList<TypePermissionMatrixItem> TypePermissionMatrix {
			get {
				if(matrix == null) {
					matrix = new TypePermissionMatrix(this, TypePermissions);
				}
				return matrix.TypePermissions;
			}
		}
		#region IPermissionMatrixTypePermissionsOwner Members
		SecuritySystemTypePermissionObject IPermissionMatrixTypePermissionsOwner.CreateItem(Type targetType) {
			SecuritySystemTypePermissionObject result = new SecuritySystemTypePermissionObject(Session);
			result.TargetType = targetType;
			TypePermissions.Add(result);
			return result;
		}
		#endregion
		#region Simplified access to the TypePermissionsObjectOwnerHelper class method. For more details about these methods see TypePermissionsObjectOwnerHelper class documentation.
		public SecuritySystemTypePermissionObject FindTypePermissionObject<T>() {
			return TypePermissionsObjectOwnerHelper.FindTypePermissionObject<T>(this);
		}
		public SecuritySystemTypePermissionObject FindTypePermissionObject(Type targetType) {
			return TypePermissionsObjectOwnerHelper.FindTypePermissionObject(this, targetType);
		}
		public void SetTypePermissions<T>(string securityOperations, SecuritySystemModifier modifier) {
			TypePermissionsObjectOwnerHelper.SetTypePermissions<T>(this, securityOperations, modifier);
		}
		public void SetTypePermissions(Type targetType, string securityOperations, SecuritySystemModifier modifier) {
			TypePermissionsObjectOwnerHelper.SetTypePermissions(this, targetType, securityOperations, modifier);
		}
		public void SetTypePermissionsRecursively<T>(string securityOperations, SecuritySystemModifier modifier) {
			TypePermissionsObjectOwnerHelper.SetTypePermissionsRecursively<T>(this, securityOperations, modifier);
		}
		public void SetTypePermissionsRecursively(Type targetType, string securityOperations, SecuritySystemModifier modifier) {
			TypePermissionsObjectOwnerHelper.SetTypePermissionsRecursively(this, targetType, securityOperations, modifier);
		}
		public SecuritySystemTypePermissionObject EnsureTypePermissionObject<T>() {
			return TypePermissionsObjectOwnerHelper.EnsureTypePermissionObject<T>(this);
		}
		public SecuritySystemTypePermissionObject EnsureTypePermissionObject(Type targetType) {
			return TypePermissionsObjectOwnerHelper.EnsureTypePermissionObject(this, targetType);
		}
		public void EnsureTypePermissions<T>(string securityOperations) {
			TypePermissionsObjectOwnerHelper.SetTypePermissions<T>(this, securityOperations, SecuritySystemModifier.Allow);
		}
		public void EnsureTypePermissions(Type targetType, string securityOperations) {
			TypePermissionsObjectOwnerHelper.SetTypePermissions(this, targetType, securityOperations, SecuritySystemModifier.Allow);
		}
		public SecuritySystemObjectPermissionsObject AddObjectAccessPermission<T>(string criteria, string operations) {
			return TypePermissionsObjectOwnerHelper.AddObjectAccessPermission<T>(this, criteria, operations);
		}
		public SecuritySystemObjectPermissionsObject AddObjectAccessPermission(Type targetType, string criteria, string operations) {
			return TypePermissionsObjectOwnerHelper.AddObjectAccessPermission(this, targetType, criteria, operations);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission<T>(string members, string operations) {
			return TypePermissionsObjectOwnerHelper.AddMemberAccessPermission<T>(this, members, operations);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission<T>(string members, string operations, string criteria) {
			return TypePermissionsObjectOwnerHelper.AddMemberAccessPermission<T>(this, members, operations, criteria);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission(Type targetType, string members, string operations) {
			return TypePermissionsObjectOwnerHelper.AddMemberAccessPermission(this, targetType, members, operations);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission(Type targetType, string members, string operations, string criteria) {
			return TypePermissionsObjectOwnerHelper.AddMemberAccessPermission(this, targetType, members, operations, criteria);
		}
		#endregion
	}
}
