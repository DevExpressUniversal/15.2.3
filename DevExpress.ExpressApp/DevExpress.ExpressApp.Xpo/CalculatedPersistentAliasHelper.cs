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
using System.Reflection;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Xpo {
	public static class CalculatedPersistentAliasHelper {
		public static void CustomizeTypesInfo(ITypesInfo typesInfo) {
			foreach(ITypeInfo typeInfo in typesInfo.PersistentTypes) {
				foreach(CalculatedPersistentAliasAttribute calculatedPersistentAliasAttribute in typeInfo.FindAttributes<CalculatedPersistentAliasAttribute>()) {
					PropertyInfo calculatedAliasProperty = typeInfo.Type.GetProperty(calculatedPersistentAliasAttribute.CalculatedAliasPropertyName, BindingFlags.Static | BindingFlags.Public);
					if(calculatedAliasProperty != null) {
						String expression = (String)calculatedAliasProperty.GetValue(null, null);
						if(!String.IsNullOrEmpty(expression)) {
							IMemberInfo memberInfo = typeInfo.FindMember(calculatedPersistentAliasAttribute.PropertyName);
							if(memberInfo == null) {
								throw new InvalidOperationException(String.Format("An invalid property name is passed to the CalculatedPersistentAlias attribute. The '{0}' type does not expose the '{1}' property.", typeInfo.FullName, calculatedPersistentAliasAttribute.PropertyName));
							}
							PersistentAliasAttribute attribute = memberInfo.FindAttribute<PersistentAliasAttribute>();
							if(attribute == null) {
								memberInfo.AddAttribute(new PersistentAliasAttribute(expression));
								typesInfo.RefreshInfo(typeInfo);
							}
						}
					}
					else {
						Tracing.Tracer.LogWarning("The public static property {0}.{1} specified in the CalculatedPersistenAlias doesn't exist", typeInfo.FullName, calculatedPersistentAliasAttribute.CalculatedAliasPropertyName);
					}
				}
			}
		}
	}
}
