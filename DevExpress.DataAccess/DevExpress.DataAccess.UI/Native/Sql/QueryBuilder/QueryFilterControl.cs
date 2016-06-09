#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public partial class QueryFilterControl : FilterControl {
		readonly IParameterService parameterService;
		readonly IServiceProvider propertyGridServices;
		readonly Predicate<string> containsName;
		readonly PrefixNameGenerator nameGenerator;
		readonly string bindToString;
		readonly string createQueryParameterString;
		readonly string boundToString;
		public QueryFilterControl() {
			InitializeComponent();
		}
		public QueryFilterControl(IParameterService parameterService, IServiceProvider propertyGridServices) : this() {
			this.propertyGridServices = propertyGridServices;
			this.parameterService = parameterService;
			this.containsName = s => QueryParameters.Any(p => string.Equals(s, p.Name, StringComparison.Ordinal));
			this.nameGenerator = new PrefixNameGenerator(DataAccessUILocalizer.GetString(DataAccessUIStringId.DefaultNameParameter), 1);
			bindToString = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryFilter_BindTo);
			createQueryParameterString = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryFilter_CreateQueryParameter);
			boundToString = DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryFilter_BoundTo);
		}
		IEnumerable<QueryParameter> QueryParameters {
			get {
				return ParametersOwner.Parameters.OfType<QueryParameter>();
			}
		}
		FilterColumn FilterColumn {
			get {
				ClauseNode node = FocusInfo.Node as ClauseNode;
				if(node == null) return null;
				return node.Property as FilterColumn;
			}
		}
		protected override WinFilterTreeNodeModel CreateModel() {
			return new QueryFilterTreeNodeModel(this);
		}
		protected override void ShowAdditionalOperandParameterMenu() {
			FilterColumn column = FilterColumn;
			DXPopupMenu menu = new DXPopupMenu { IsRightToLeft = ViewInfo.RightToLeft };
			foreach(QueryParameter parameter in GetParametersByType(column.ColumnType)) {
				string externalParameterName;
				DXMenuItem menuItem = new DXMenuItem(parameter.Name, OnParameterClick);
				if(parameter.IsBoundToExternalParameter(out externalParameterName)) {
					SuperToolTip toolTip = new SuperToolTip();
					toolTip.Items.Add(string.Format(boundToString, externalParameterName));
					menuItem.SuperTip = toolTip;
				}
				menu.Items.Add(menuItem);
			}
			menu.Items.Add(new DXMenuItem(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryFilter_AddQueryParameter), (s, e) => { CreateQueryParameter(); }));
			if(parameterService != null ) {
				DXSubMenuItem subMenuItem = new DXSubMenuItem(bindToString) { BeginGroup = true };
				foreach(IParameter parameter in parameterService.Parameters) {
					DXMenuItem dxMenuItem = new DXMenuItem(parameter.Name, (s, e) => { BindToExternalParameter(((DXMenuItem)s).Tag as IParameter); });
					dxMenuItem.Tag = parameter;
					subMenuItem.Items.Add(dxMenuItem);
				}
				subMenuItem.Items.Add(new DXMenuItem(parameterService.AddParameterString, (s, e) => { CreateExternalParameter(); }));
				menu.Items.Add(subMenuItem);
			}
			ShowMenu(menu, FilterControlMenuType.AdditionalOperandParameter);
		}
		IEnumerable<QueryParameter> GetParametersByType(Type type) {
			foreach(QueryParameter queryParameter in QueryParameters) {
				if(queryParameter.Type == type)
					yield return queryParameter;
				if(queryParameter.Type == typeof(Expression)) {
					Expression expression = queryParameter.Value as Expression;
					if(expression != null && expression.ResultType == type)
						yield return queryParameter;
				}
			}
		}
		void CreateQueryParameter() {
			IParameter newParameter = new QueryParameter(nameGenerator.GenerateName(containsName), FilterColumn.ColumnType, null);
			if(EditParameter(newParameter, createQueryParameterString))
				AddParameter(newParameter);
		}
		void CreateExternalParameter() {
			IParameter newParameter = parameterService.CreateParameter(FilterColumn.ColumnType);
			if(EditParameter(newParameter, parameterService.CreateParameterString)) {
				parameterService.AddParameter(newParameter);
				AddParameter(newParameter.ToExternalQueryParameter());
			}
		}
		void BindToExternalParameter(IParameter parameter) {
			if(parameter != null) {
				AddParameter(parameter.ToExternalQueryParameter());	
			}
		}
		bool EditParameter(IParameter parameter, string formCaption) {
			IServiceContainer serviceContainer = new ServiceContainer(propertyGridServices);
			if(parameterService != null)
				serviceContainer.AddService(typeof(IParameterService), parameterService);
			using(PropertyGridForm form = new PropertyGridForm(propertyGridServices) {
				SelectedObject = parameter,
				ServiceProvider = serviceContainer,
				Text = formCaption
			}) {
				return form.ShowDialog() == DialogResult.OK;
			}
		}
		void AddParameter(IParameter parameter) {
			this.ParametersOwner.AddParameter(parameter.Name, parameter.Type);
			QueryParameter queryParameter = QueryParameters.First(p => p.Name == parameter.Name);
			queryParameter.Value = parameter.Value;
			FocusInfo.ChangeElement(parameter.Name);
		}
	}
	public class QueryFilterTreeNodeModel : WinFilterTreeNodeModel {
		public QueryFilterTreeNodeModel(QueryFilterControl queryFilterControl)
			: base(queryFilterControl) {
		}
		public override string GetLocalizedStringForFilterEmptyParameter() {
			return DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryFilter_SelectParameter);
		}
	}
	public static class ParameterHelper {
		public static bool IsBoundToExternalParameter(this QueryParameter parameter, out string externalParameterName) {
			externalParameterName = string.Empty;
			if(parameter.Type == typeof(Expression)) {
				Regex regex = new Regex(@"\AParameters\.(?<name>.+)\z", RegexOptions.None);
				Expression expression = parameter.Value as Expression;
				if(expression == null) {
					return false;
				}
				string substring = expression.ExpressionString.Substring(1, expression.ExpressionString.Length - 2);
				Match match = regex.Match(substring);
				if(match.Success) {
					externalParameterName = match.Groups["name"].Value;
					return true;
				}
			}
			return false;
		}
		public static IParameter ToExternalQueryParameter(this IParameter parameter) {
			Expression expression = new Expression(string.Format("[Parameters.{0}]", parameter.Name), parameter.Type);
			return new QueryParameter(parameter.Name, typeof(Expression), expression);
		}
	}
}
