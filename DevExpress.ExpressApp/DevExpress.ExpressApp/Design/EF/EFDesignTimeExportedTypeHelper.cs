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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Design {
	public class EFDesignTimeExportedTypeHelper : IExportedTypeHelper {
		protected ITypesInfo typesInfo;
		public void Init(ITypesInfo typesInfo) {
			this.typesInfo = typesInfo;
		}
		public virtual Boolean IsExportedType(Type type) {
			if(type != null) {
				foreach(ITypeInfo contextTypeInfo in FindContextTypesInfo()) {
					foreach(IMemberInfo objectContextMemberInfo in contextTypeInfo.OwnMembers) {
						Type memberType = objectContextMemberInfo.MemberType;
						if(memberType.IsGenericType && EFDesignTimeTypeInfoHelper.IsEntitySetType(memberType.GetGenericTypeDefinition())
								&& (memberType.GetGenericArguments()[0].IsAssignableFrom(type))) {
							return true;
						}
					}
				}
			}
			return false;
		}
		private IEnumerable<ITypeInfo> FindContextTypesInfo() {
			List<ITypeInfo> contextTypesInfo = new List<ITypeInfo>();
			contextTypesInfo.AddRange(GetTypeDescendants(EFDesignTimeTypeInfoHelper.ObjectContextTypeName));
			contextTypesInfo.AddRange(GetTypeDescendants(EFDesignTimeTypeInfoHelper.DbContextTypeName));
			return contextTypesInfo;
		}
		private IEnumerable<ITypeInfo> GetTypeDescendants(string typeFullName) {
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(typeFullName);
			return typeInfo != null ? typeInfo.Descendants : new ITypeInfo[] { };
		}
	}
}
