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

using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.Native;
namespace DevExpress.DashboardWin.Native {
	public class HistoryItemDataSourceUpdater {
		class ParametersCriteriaExtractor : CriteriaPatcher {
			public static List<string> ProcessCriteria(CriteriaOperator criteria) {
				ParametersCriteriaExtractor patcher = new ParametersCriteriaExtractor();
				patcher.Process(criteria);
				patcher.list.Sort();
				for(int i = patcher.list.Count - 1; i > 0; i--)
					if(patcher.list[i] == patcher.list[i - 1])
						patcher.list.RemoveAt(i);
				return patcher.list;
			}
			readonly List<string> list = new List<string>();
			public override CriteriaOperator Visit(OperandValue theOperand) {
				OperandParameter operandParameter = theOperand as OperandParameter;
				if(!ReferenceEquals(operandParameter, null)) {
					list.Add(operandParameter.ParameterName);
				}
				return base.Visit(theOperand);
			}
		}
		Dashboard dashboard;
		ParameterChangesCollection parameterChanges;
		public HistoryItemDataSourceUpdater(Dashboard dashboard, ParameterChangesCollection parameterChanges) {
			this.dashboard = dashboard;
			this.parameterChanges = parameterChanges;
		}
		public void Update(IEnumerable<IParameter> parameters) {
			UpdateInternal(null, parameters);
		}
		public void Update(IEnumerable<IDashboardDataSource> forcedDataSources) {
			UpdateInternal(forcedDataSources, null);
		}
		void UpdateInternal(IEnumerable<IDashboardDataSource> forcedDataSources, IEnumerable<IParameter> parameters) {
			List<DataDashboardItem> itemsToRefresh = new List<DataDashboardItem>();
			List<IDashboardDataSource> dataSourceToUpdate = new List<IDashboardDataSource>();
			List<string> affectedParameters = GetAffectedParameters();
			Dictionary<IDashboardDataSource, List<CalculatedField>> affectedCalculatedFields = GetAffectedCalculatedFields(dashboard, affectedParameters);
			foreach(DashboardItem item in dashboard.Items) {
				DataDashboardItem dataItem = item as DataDashboardItem;
				if(dataItem == null || dataItem.DataSource == null)
					continue;
				List<CalculatedField> affectedFields = null;
				affectedCalculatedFields.TryGetValue(dataItem.DataSource, out affectedFields);
				if(NeedRefreshItem(dataItem, affectedParameters, affectedFields))
					itemsToRefresh.Add(dataItem);
			}
			foreach(IDashboardDataSource dataSource in dashboard.DataSources)
				if(NeedUpdateDataSource(dataSource, affectedParameters))
					dataSourceToUpdate.Add(dataSource);
			if(forcedDataSources != null)
				dataSourceToUpdate.AddRange(forcedDataSources.Where(d => !dataSourceToUpdate.Contains(d)));
			for(int i = itemsToRefresh.Count - 1; i >= 0; i--)
				if(dataSourceToUpdate.Contains(itemsToRefresh[i].DataSource))
					itemsToRefresh.RemoveAt(i);
			List<CalculatedField> changed = null;
			foreach(IDashboardDataSource dataSource in dashboard.DataSources)
				if(!dataSourceToUpdate.Contains(dataSource) && affectedCalculatedFields.TryGetValue(dataSource, out changed)) {
					if(parameters == null)
						parameters = dashboard.Parameters;
					dataSource.SetParameters(parameters);
					IEnumerable<string> dataMembers = changed.Select(cf => cf.DataMember).Distinct();
					foreach(string dataMember in dataMembers)
						dataSource.OnCalculatedFieldParametersChanged(changed.Where(cf => cf.DataMember == dataMember).ToList(), dataMember);
				}
			dashboard.BeginUpdate();
			if (dataSourceToUpdate.Count > 0)
				foreach (IDashboardDataSource dataSource in dataSourceToUpdate)
					dashboard.FillDataSource(dataSource);
			dashboard.EndUpdate();
		}
		protected virtual bool NeedUpdateDataSource(IDashboardDataSource dataSource, List<string> affectedParameters) {
			if(dataSource.Filter != null && NeedUpdateCriteriaValue(CriteriaOperator.Parse(dataSource.Filter), affectedParameters))
				return true;
#pragma warning disable 612, 618
			SqlDataProvider dataProvider = dataSource.DataProvider as SqlDataProvider;
			if(affectedParameters.Count > 0 && dataProvider != null && dataProvider.IsCustomSql)
				return true;
#pragma warning restore 612, 618
			return false;
		}
		List<string> GetAffectedParameters() {
			List<string> affected = new List<string>();
			affected.AddRange(parameterChanges.ParametersAdded.Select(p => p.Name));
			affected.AddRange(parameterChanges.ParametersRemoved.Select(p => p.Name));
			affected.AddRange(parameterChanges.ParametersChanged.Select(p => p.Name));
			return affected;
		}
		Dictionary<IDashboardDataSource, List<CalculatedField>> GetAffectedCalculatedFields(Dashboard dashboard, List<string> affectedParameters) {
			Dictionary<IDashboardDataSource, List<CalculatedField>> dic = new Dictionary<IDashboardDataSource, List<CalculatedField>>();
			affectedParameters = affectedParameters.ConvertAll((p) => string.Format(CalculatedFieldsController.ParameterExpressionFormatString, p));
			foreach(IDashboardDataSource dataSource in dashboard.DataSources) {
				if(dataSource.CalculatedFields != null) {
					List<CalculatedField> affected = dataSource.CalculatedFields.Where(
													  (f) => affectedParameters.Any((p) => f.Expression != null && f.Expression.Contains(p)))
																															.ToList();
					if(affected.Count != 0)
						dic.Add(dataSource, affected);
				}
			}
			return dic;
		}
		bool NeedRefreshItem(DataDashboardItem item, List<string> affectedParameters, List<CalculatedField> affectedCalculatedFields) {
			return !ReferenceEquals(item.ActualFilterCriteria, null) && NeedUpdateCriteriaValue(item.ActualFilterCriteria, affectedParameters) ||
				 affectedCalculatedFields != null && item.GetDataMembers().Any((m) => affectedCalculatedFields.Any((f) => f.Name == m));
		}
		bool NeedUpdateCriteriaValue(CriteriaOperator criteria, List<string> affectedParameters) {
			foreach(string parameter in ParametersCriteriaExtractor.ProcessCriteria(criteria))
				if(affectedParameters.Contains(parameter))
					return true;
			return false;
		}
	}	
}
