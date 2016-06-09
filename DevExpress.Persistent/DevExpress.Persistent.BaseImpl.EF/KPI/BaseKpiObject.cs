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
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
namespace DevExpress.Persistent.BaseImpl.EF.Kpi {
	public abstract class BaseKpiObject {
		private Boolean isDefaultPropertyMemberInfoInitialized = false;
		private IMemberInfo defaultPropertyMemberInfo;
		public BaseKpiObject()
			: base() {
		}
		[Browsable(false)]
		public Int32 ID { get; set; }
		public override String ToString() {
			String result = "";
			if(!isDefaultPropertyMemberInfoInitialized) {
				String defaultPropertyName = "";
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(GetType());
				XafDefaultPropertyAttribute xafDefaultPropertyAttribute = typeInfo.FindAttribute<XafDefaultPropertyAttribute>();
				if(xafDefaultPropertyAttribute != null) {
					defaultPropertyName = xafDefaultPropertyAttribute.Name;
				}
				else {
					DefaultPropertyAttribute defaultPropertyAttribute = typeInfo.FindAttribute<DefaultPropertyAttribute>();
					if(defaultPropertyAttribute != null) {
						defaultPropertyName = defaultPropertyAttribute.Name;
					}
				}
				if(!String.IsNullOrWhiteSpace(defaultPropertyName)) {
					defaultPropertyMemberInfo = typeInfo.FindMember(defaultPropertyName);
				}
				isDefaultPropertyMemberInfoInitialized = true;
			}
			if(defaultPropertyMemberInfo != null) {
				Object obj = defaultPropertyMemberInfo.GetValue(this);
				if(obj != null) {
					result = obj.ToString();
				}
			}
			else {
				result = base.ToString();
			}
			return result;
		}
	}
}
