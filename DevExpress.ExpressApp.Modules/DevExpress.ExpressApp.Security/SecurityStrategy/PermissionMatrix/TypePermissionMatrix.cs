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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Security.Strategy.PermissionMatrix {
	public class TypePermissionMatrix {
		private IList<TypePermissionMatrixItem> typePermissionItems;
		[Obsolete("Use 'SecurityStrategy.GetSecuredTypes' instead.", true)]
		public static IEnumerable<Type> GetSecuredTypes() {
			return SecurityStrategy.GetSecuredTypes();
		}
		public TypePermissionMatrix(IPermissionMatrixTypePermissionsOwner owner, IList<SecuritySystemTypePermissionObject> sourcePermissions) {
			Dictionary<Type, SecuritySystemTypePermissionObject> permissions = new Dictionary<Type, SecuritySystemTypePermissionObject>();
			foreach(SecuritySystemTypePermissionObject item in sourcePermissions) {
				if(item.TargetType != null) { 
					permissions[item.TargetType] = item;
				}
			}
			typePermissionItems = new BindingList<TypePermissionMatrixItem>();
			foreach(Type type in SecurityStrategy.GetSecuredTypes()) {
				SecuritySystemTypePermissionObject typePermissionObject;
				if(permissions.TryGetValue(type, out typePermissionObject)) {
					typePermissionItems.Add(new TypePermissionMatrixItem(typePermissionObject));
				}
				else {
					typePermissionItems.Add(new TypePermissionMatrixItem(type, owner));
				}
			}
		}
		public IList<TypePermissionMatrixItem> TypePermissions {
			get {
				return typePermissionItems;
			}
		}
	}
}
