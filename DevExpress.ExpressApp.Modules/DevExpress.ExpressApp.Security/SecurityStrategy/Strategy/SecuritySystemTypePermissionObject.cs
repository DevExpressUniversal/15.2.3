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

using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Security.Strategy {
	[System.ComponentModel.DisplayName("Type Operation Permissions")]
	[ImageName("BO_Security_Permission_Type")]
	[MapInheritance(MapInheritanceType.ParentTable)]
	public class SecuritySystemTypePermissionObject : SecuritySystemTypePermissionsObjectBase {
		private Session session;
		public SecuritySystemTypePermissionObject(Session session)
			: base(session) {
			this.session = session;
		}
		[Association]
		[VisibleInListView(false), VisibleInDetailView(false)]
		public SecuritySystemTypePermissionsObjectOwner Owner {
			get { return GetPropertyValue<SecuritySystemTypePermissionsObjectOwner>("Owner"); }
			set { SetPropertyValue<SecuritySystemTypePermissionsObjectOwner>("Owner", value); }
		}
		protected override void OnPropertyChangedPermissions(string operation, bool value) {			
					SecuritySystemRoleBase securitySystemRoleBase = Owner as SecuritySystemRoleBase;
					if(securitySystemRoleBase != null && SecuritySystemRoleBase.AutoAssociationPermissions) {
						securitySystemRoleBase.TypeAssociationPermissionsOwnerHelper.FindAndSetAssociationTypePermission(this, operation, value);		   
			}
		}
		public void SetAssociationObjectPermissions(bool value, string securityOperations, string criteria) {
		}
	}
}
