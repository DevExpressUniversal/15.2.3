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

using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Security.Strategy {
	[ImageName("BO_Role"), System.ComponentModel.DisplayName("Role")]
	[MapInheritance(MapInheritanceType.ParentTable)]
	public class SecuritySystemRole : SecuritySystemRoleBase {
		protected override IEnumerable<IOperationPermissionProvider> GetChildrenCore() {
			List<IOperationPermissionProvider> result = new List<IOperationPermissionProvider>();
			result.AddRange(base.GetChildrenCore());
			result.AddRange(new EnumerableConverter<IOperationPermissionProvider, SecuritySystemRole>(ChildRoles));
			return result;
		}
		public SecuritySystemRole(Session session)
			: base(session) {
		}
		[Association("Users-Roles")]
		public XPCollection<SecuritySystemUser> Users {
			get {
				return GetCollection<SecuritySystemUser>("Users");
			}
		}
		[Association("ParentRoles-ChildRoles")]
		public XPCollection<SecuritySystemRole> ChildRoles {
			get { return GetCollection<SecuritySystemRole>("ChildRoles"); }
		}
		[Association("ParentRoles-ChildRoles")]
		public XPCollection<SecuritySystemRole> ParentRoles {
			get { return GetCollection<SecuritySystemRole>("ParentRoles"); }
		}
	}
}
