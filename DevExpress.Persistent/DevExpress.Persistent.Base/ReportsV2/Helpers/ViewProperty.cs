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
using System.Drawing.Design;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Serializing;
namespace DevExpress.Persistent.Base.ReportsV2 {
	public class ViewProperty : IDataColumnInfoProvider, IXtraSerializableLayoutEx {
		private string displayName;
		private string expression;
		private DataSourceBase owner;
		public ViewProperty() {
			displayName = string.Empty;
			expression = string.Empty;
		}
		public ViewProperty(string name, string property) {
			this.displayName = name;
			this.expression = property;
		}
		public ViewProperty(string name, CriteriaOperator property) {
			this.displayName = name;
			if(!CriteriaOperator.Equals(null, property)) {
				this.expression = property.ToString();
			}
		}
		[XtraSerializableProperty(-1)]
		public string DisplayName {
			get { return displayName; }
			set { displayName = value; }
		}
		[Editor("DevExpress.XtraEditors.Design.ExpressionEditorBase, " + AssemblyInfo.SRAssemblyEditors, typeof(UITypeEditor))]
		[XtraSerializableProperty(-1)]
		public String Expression {
			get { return expression; }
			set { expression = value; }
		}
		internal void SetOwner(DataSourceBase owner) {
			this.owner = owner;
		}
		IDataColumnInfo IDataColumnInfoProvider.GetInfo(object arguments) {
			return new ViewPropertyInfoWrapper(this, owner);
		}
		internal class ViewPropertyMemberInfoWrapper : IDataColumnInfo, IDataColumnInfoProvider {
			readonly IMemberInfo memberInfo;
			readonly List<IDataColumnInfo> columns = new List<IDataColumnInfo>(0);
			public ViewPropertyMemberInfoWrapper(IMemberInfo memberInfo) {
				this.memberInfo = memberInfo;
			}
			public string Caption {
				get {
					return CaptionHelper.GetMemberCaption(memberInfo);
				}
			}
			public List<IDataColumnInfo> Columns { get { return columns; } }
			public DataControllerBase Controller { get { return null; } }
			public string FieldName { get { return memberInfo.Name; } }
			public Type FieldType { get { return memberInfo.MemberType; } }
			public string Name { get { return memberInfo.Name; } }
			public string UnboundExpression { get { return memberInfo.Name; } }
			IDataColumnInfo IDataColumnInfoProvider.GetInfo(object arguments) {
				throw new NotImplementedException();
			}
		}
		internal class ViewPropertyInfoWrapper : IDataColumnInfo {
			readonly ViewProperty column;
			readonly IServiceProvider serviceProvider;
			public ViewPropertyInfoWrapper(ViewProperty column, IServiceProvider serviceProvider) {
				this.column = column;
				this.serviceProvider = serviceProvider;
			}
			string IDataColumnInfo.Caption { get { return column.DisplayName; } }
			List<IDataColumnInfo> IDataColumnInfo.Columns {
				get {
					List<IDataColumnInfo> res = new List<IDataColumnInfo>();
					if(column.owner == null) return res;
					ITypesInfo typesInfoService = serviceProvider.GetService(typeof(ITypesInfo)) as ITypesInfo;
					ITypeInfo typeInfo = typesInfoService.FindTypeInfo(column.owner.DataType);
					if(typeInfo == null) return res;
					foreach(IMemberInfo member in typeInfo.Members) {
						if(member.IsVisible && member.IsPublic && member.IsPersistent) {
							res.Add(new ViewPropertyMemberInfoWrapper(member));
						}
					}
					return res;
				}
			}
			DataControllerBase IDataColumnInfo.Controller { get { return null; } }
			string IDataColumnInfo.FieldName { get { return ReferenceEquals(column.Expression, null) ? "" : column.Expression; } }
			Type IDataColumnInfo.FieldType {
				get {
					return typeof(string);
				}
			}
			string IDataColumnInfo.Name { get { return column.DisplayName; } }
			string IDataColumnInfo.UnboundExpression { get { return ReferenceEquals(column.Expression, null) ? "" : column.Expression; } }
		}
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(DevExpress.Utils.OptionsLayoutBase options, string propertyName, int id) {
			return propertyName != "Ref";
		}
		void IXtraSerializableLayoutEx.ResetProperties(DevExpress.Utils.OptionsLayoutBase options) { }
		#endregion
	}
}
