#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Data.Filtering;
namespace DevExpress.DashboardWin.Native {
	class CalculatedFieldExpressionEditorLogic : ExpressionEditorLogicEx, IActualParametersProvider {
		CalculatedFieldExpressionEditorForm form;
		internal IDataColumnInfo ColumnInfo { get { return contextInstance as IDataColumnInfo; } }
		internal CalculatedField Field { get { return contextInstance as CalculatedField; } }
		public CalculatedFieldExpressionEditorLogic(IExpressionEditor editor, IDataColumnInfo contextInstance, CalculatedFieldExpressionEditorForm form)
			: base(editor, contextInstance) {
			this.form = form;
		}
		protected override void FillFieldsTable(Dictionary<string, string> itemsTable) {
			form.ShowParametersEditor();
		}
		protected override void RefreshInputParameters() {
			form.HideParametersEditor();
			base.RefreshInputParameters();
			editor.SetDescription(string.Empty);
		}
		protected override IList<FunctionEditorCategory> GetFunctionsTypeNames() {
			return new List<FunctionEditorCategory> {
				FunctionEditorCategory.All,
				FunctionEditorCategory.DateTime,
				FunctionEditorCategory.Logical,
				FunctionEditorCategory.Math,
				FunctionEditorCategory.String
			};
		}
		protected override void FillParametersTable(Dictionary<string, string> itemsTable) {
			foreach(DashboardParameter parameter in form.Parameters) {
				string name = parameter.Name;
				if(string.IsNullOrEmpty(name))
					continue;
				name = string.Format("[{0}]", name);
				if(!itemsTable.ContainsKey(name))
					itemsTable.Add(name, editor.GetResourceString("Parameters Description Prefix") + parameter.Type.Name);
			}
		}
		protected override ItemClickHelper GetItemClickHelper(string selectedItemText, IExpressionEditor editor) {
			if(selectedItemText == form.GetCalculatedFieldResourceString("AggregateFunctions.Text"))
				return new AggregatesClickHelper(form);
			return base.GetItemClickHelper(selectedItemText, editor);
		}
		class AggregatesClickHelper : ItemClickHelper {
			CalculatedFieldExpressionEditorForm form;
			public AggregatesClickHelper(CalculatedFieldExpressionEditorForm form)
				: base(form) {
				this.form = form;
			}
			protected override void FillItemsTable() {
				AddItemTable("Avg()", editor.GetResourceString("AvgAggregate.Description"), 0);
				AddItemTable("Count()", editor.GetResourceString("CountAggregate.Description"), 0);
				AddItemTable("CountDistinct()", form.GetCalculatedFieldResourceString("CountDistinctAggregate.Description"), 0);
				AddItemTable("Max()", editor.GetResourceString("MinAggregate.Description"), 0);
				AddItemTable("Min()", editor.GetResourceString("MaxAggregate.Description"), 0);
				AddItemTable("Sum()", editor.GetResourceString("SumAggregate.Description"), 0);
			}
		}
		protected override void ValidateExpressionEx(string expression) {
			expression = ConvertToFields(expression);
			IDashboardDataSource dataSource = form.DataSourceInfo != null ? form.DataSourceInfo.DataSource : null;
			string dataMember = form.DataSourceInfo != null ? form.DataSourceInfo.DataMember : null;
			if(dataSource != null) {
				dataSource.GetCalculatedFieldDesiredType(expression, dataMember, Field == null ? null : Field.Name, false, this);
				CriteriaOperator criteria = dataSource.GetExpandedCalculatedFieldExpressionOperator(expression, ColumnInfo.Name, true, dataMember, this, false);
				if(!ReferenceEquals(criteria, null)) {
					if(DevExpress.PivotGrid.CriteriaVisitors.HasAggregateCriteriaChecker.Check(criteria)) {
						Dashboard dashboard = form.Dashboard;
						if(dashboard != null && dashboard.Items != null && ColumnInfo != null) {
							foreach(DashboardItem item in dashboard.Items) {
								DataDashboardItem dataDashboardItem = item as DataDashboardItem;
								if(dataDashboardItem != null) {
									Dimension dimension = dataDashboardItem.GetDimension(ColumnInfo.Name);
									if(dimension != default(Dimension)) {
										throw new CalculatedFieldExpressionEditorException(string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageSummaryCalculatedFieldInDimension), ColumnInfo.Name, dimension.DisplayName));
									}
								}
							}
						}
					}
				}
			}
		}
		protected override void ShowError(Exception exception) {
			CalculatedFieldExpressionEditorException cfe = exception as CalculatedFieldExpressionEditorException;
			if(cfe != null)
				editor.ShowError(cfe.Message);
			else
				base.ShowError(exception);
		}
#if DEBUGTEST
		internal void ValidateExpressionExAccess(string expression) {
			ValidateExpressionEx(expression);
		}
#endif
		protected override object[] GetListOfInputTypesObjects() {
			return new object[] {
				editor.GetResourceString("Functions.Text"),
				form.GetCalculatedFieldResourceString("AggregateFunctions.Text"),
				editor.GetResourceString("Operators.Text"),
				editor.GetResourceString("Fields.Text"),
				editor.GetResourceString("Constants.Text"),
				editor.GetResourceString("Parameters.Text")
			};
		}
		IEnumerable<IParameter> IActualParametersProvider.GetActualParameters() {
			throw new InvalidOperationException();
		}
		IEnumerable<IParameter> IActualParametersProvider.GetParameters() {
			return form.Parameters;
		}
	}
}
