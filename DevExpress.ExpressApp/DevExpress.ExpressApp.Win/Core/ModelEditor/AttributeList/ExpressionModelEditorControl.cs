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
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.XtraEditors.Design;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class ExpressionDataColumnInfo : IDataColumnInfo {
		private IModelMember memberModel;
		private List<IDataColumnInfo> columns;
		public ExpressionDataColumnInfo(IModelMember memberModel) {
			this.memberModel = memberModel;
			UnboundExpression = memberModel.Expression;
		}
		public String Caption {
			get {
				return String.IsNullOrWhiteSpace(memberModel.Caption) ? memberModel.Name : memberModel.Caption;
			}
		}
		public List<IDataColumnInfo> Columns {
			get {
				if(columns == null) {
					columns = new List<IDataColumnInfo>();
					if(memberModel.ModelClass != null) {
						foreach(IModelMember memberModel2 in memberModel.ModelClass.AllMembers) {
							if(memberModel2 != memberModel) {
								IMemberInfo memberInfo = memberModel.ModelClass.TypeInfo.FindMember(memberModel2.Name);
								if((memberInfo != null) && memberInfo.IsVisible && memberInfo.IsPublic) {
									columns.Add(new ExpressionDataColumnInfo(memberModel2));
								}
							}
						}
					}
				}
				return columns;
			}
		}
		public DataControllerBase Controller {
			get { return null; }
		}
		public String FieldName {
			get { return memberModel.Name; }
		}
		public Type FieldType {
			get { return memberModel.Type; }
		}
		public String Name {
			get { return memberModel.Name; }
		}
		public String UnboundExpression { get; set; }
	}
	[ToolboxItem(false)]
	public class ExpressionModelEditorControl : ExpressionEditorBase {
		protected override ExpressionEditorForm CreateForm(Object instance, IDesignerHost designerHost, object value) {
			ExpressionDataColumnInfo expressionDataColumnInfo = new ExpressionDataColumnInfo((IModelMember)instance);
			return new XafExpressionEditorForm(expressionDataColumnInfo, designerHost);
		}
		public ExpressionModelEditorControl()
			: base() {
		}
	}
}
