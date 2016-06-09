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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.ViewInfo;
namespace DevExpress.ExpressApp.PivotGrid.Win {
	[ToolboxItem(false)]
	public class XafPivotGridControl : PivotGridControl {
		public XafPivotGridControl() : base() { }
		public XafPivotGridControl(PivotGridViewInfoData viewInfoData) : base(viewInfoData) { }
		protected override PivotGridViewInfoData CreateData() {
			return new XafPivotGridViewInfoData(this);
		}
	}
	public class XafPivotGridViewInfoData : PivotGridViewInfoData, IDataObjectTypeProvider {
		private Type dataObjectType;
		private string GetLocalizedFieldCaption(string fieldName) {
			if(dataObjectType != null) {
				return CaptionHelper.GetFullMemberCaption(XafTypesInfo.Instance.FindTypeInfo(dataObjectType), fieldName);
			}
			return fieldName;
		}
		private bool IsVisibleFieldName(string fieldName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(dataObjectType);
			if(typeInfo != null) {
				IMemberInfo memberInfo = typeInfo.FindMember(fieldName);
				if(memberInfo != null) {
					if(memberInfo.Owner.KeyMember == memberInfo) {
						return true;
					}
					return memberInfo.IsPublic && memberInfo.IsVisible && !memberInfo.IsList &&
						(memberInfo.MemberTypeInfo.IsDomainComponent || memberInfo.Owner.IsDomainComponent);
				}
				else {
					return false;
				}
			}
			else {
				return !string.IsNullOrEmpty(fieldName) && (fieldName.IndexOf("!") < 0)
						&& (fieldName.ToLower() != "this") && !fieldName.ToLower().EndsWith(".this");
			}
		}
		public XafPivotGridViewInfoData() : base() { }
		public XafPivotGridViewInfoData(IViewInfoControl control) : base(control) { }
		protected override void RetrieveFieldCore(PivotArea area, string fieldName, string caption, string displayFolder, bool visible) {
			if(IsVisibleFieldName(fieldName)) {
				base.RetrieveFieldCore(area, fieldName, caption, displayFolder, visible);
				if(dataObjectType != null) {
					Fields[fieldName].Caption = GetLocalizedFieldCaption(fieldName);
				}
			}
		}
		public override string[] GetFieldList() {
			List<string> result = new List<string>();
			foreach(string fieldName in base.GetFieldList()) {
				if(IsVisibleFieldName(fieldName)) {
					result.Add(fieldName);
				}
			}
			return result.ToArray();
		}
		#region IDataObjectTypeProvider Members
		public Type DataObjectType {
			get { return dataObjectType; }
			set { dataObjectType = value; }
		}
		#endregion
#if DebugTest
		public bool DebugTest_IsVisibleFieldName(string fieldName) {
			return IsVisibleFieldName(fieldName);
		}
#endif
	}
}
