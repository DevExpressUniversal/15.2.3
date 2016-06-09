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
using System.Security;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[ImageName("BO_Security_Permission")]
	public class PersistentPermission : BaseObject, IPersistentPermission, IXpoCloneable {
		private RoleBase role;
		private PersistentPermissionImpl persistentPermissionImpl = new PersistentPermissionImpl();
#if MediumTrust
		[Persistent, Size(4000), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public string SerializedPermission {
			get { return persistentPermissionImpl.SerializedPermission; }
			set {
				string oldValue = persistentPermissionImpl.SerializedPermission;
				persistentPermissionImpl.SerializedPermission = value;
				OnChanged("SerializedPermission", oldValue, persistentPermissionImpl.SerializedPermission);
			}
		}
#else
		[Persistent, Size(4000), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
		protected string SerializedPermission {
			get {
				if(this.IsDeleted) {
					return "";
				}
				return persistentPermissionImpl.SerializedPermission;
			}
			set {
				string oldValue = persistentPermissionImpl.SerializedPermission;
				persistentPermissionImpl.SerializedPermission = value;
				OnChanged("SerializedPermission", oldValue, persistentPermissionImpl.SerializedPermission);
			}
		}
#endif
		public PersistentPermission(Session session) : base(session) { }
		public PersistentPermission(Session session, IPermission permission)
			: base(session) {
			this.persistentPermissionImpl.Permission = permission;
		}
		public override string ToString() {
			return persistentPermissionImpl.ToString();
		}
		[Association("Role-PersistentPermissions")]
		public RoleBase Role {
			get { return role; }
			set { SetPropertyValue("Role", ref role, value); }
		}
		IPermission IPersistentPermission.Permission {
			get { return persistentPermissionImpl.Permission; }
			set {
				IPermission oldValue = persistentPermissionImpl.Permission;
				persistentPermissionImpl.Permission = value;
				OnChanged("Permission", oldValue, persistentPermissionImpl.Permission);
			}
		}
		[NonPersistent]
		[ModelDefault("ReadOnly", "false")]
		[EditorAlias(EditorAliases.DetailPropertyEditor)]
		public IPermission Permission {
			get { return persistentPermissionImpl.Permission; }
		}
		IXPSimpleObject IXpoCloneable.CloneTo(Type targetType) {
			if(!typeof(PersistentPermission).IsAssignableFrom(targetType)) {
				return null;
			}
			PersistentPermission result = (PersistentPermission)TypeHelper.CreateInstance(GetType(), Session);
			result.Role = Role;
			result.SerializedPermission = SerializedPermission;
			return result;
		}
	}
}
