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

using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Security.Strategy {
	[System.ComponentModel.DisplayName("Base Role")]
	[MapInheritance(MapInheritanceType.ParentTable)] 
	public class SecuritySystemRoleBase : SecuritySystemTypePermissionsObjectOwner, ISecurityRole, ISecuritySystemRole {
		private const bool AutoAssociationPermissionsDefaultValue = true;
		private string name;
		protected override IEnumerable<IOperationPermission> GetPermissionsCore() {
			List<IOperationPermission> result = new List<IOperationPermission>();
			result.AddRange(base.GetPermissionsCore());
			if(IsAdministrative) {
				result.Add(new IsAdministratorPermission());
			}
			if(CanEditModel) {
				result.Add(new ModelOperationPermission());
			}
			return result;
		}
		static SecuritySystemRoleBase() {
			AutoAssociationPermissions = AutoAssociationPermissionsDefaultValue;
		}
		public SecuritySystemRoleBase(Session session)
			: base(session) {
		}
		[RuleRequiredField("SecuritySystemRoleBase_Name_RuleRequiredField", DefaultContexts.Save)]
		[RuleUniqueValue("SecuritySystemRoleBase_Name_RuleUniqueValue", DefaultContexts.Save, "The role with the entered Name was already registered within the system")]
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public bool IsAdministrative {
			get { return GetPropertyValue<bool>("IsAdministrative"); }
			set { SetPropertyValue<bool>("IsAdministrative", value); }
		}
		public bool CanEditModel {
			get { return GetPropertyValue<bool>("CanEditModel"); }
			set { SetPropertyValue<bool>("CanEditModel", value); }
		}
		private TypeAssociationPermissionsOwnerHelper typeAssociationPermissionsOwnerHelper = new TypeAssociationPermissionsOwnerHelper();
		[Browsable(false)]
		public TypeAssociationPermissionsOwnerHelper TypeAssociationPermissionsOwnerHelper {
			get { return typeAssociationPermissionsOwnerHelper; }
		}
		private void UnsubscribeFromEvents() {
			TypePermissions.CollectionChanged -= TypePermissions_CollectionChanged;
		}
		void TypePermissions_CollectionChanged(object sender, XPCollectionChangedEventArgs e) {
			SecuritySystemTypePermissionObject securitySystemTypePermissionsObjectBase = e.ChangedObject as SecuritySystemTypePermissionObject;
			if(securitySystemTypePermissionsObjectBase != null) {
				if(e.CollectionChangedType == XPCollectionChangedType.BeforeRemove) {
					if(!Session.IsObjectsLoading) {
						typeAssociationPermissionsOwnerHelper.FindAndDeleteAssociationTypePermission(this, securitySystemTypePermissionsObjectBase);
					}
				}
			}
		}
		private void SubscribeToEvents() {
			TypePermissions.CollectionChanged += TypePermissions_CollectionChanged;
		}
		[DefaultValue(AutoAssociationPermissionsDefaultValue)]
		public static bool AutoAssociationPermissions { get; set; }
	}
}
