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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
namespace DevExpress.DashboardWin.Native {
	public class DashboardParametersHistoryItem : IHistoryItem {
		DashboardParameterCollection parameters;
		ParameterChangesCollection parameterChanges;
		public virtual string Caption {
			get { return DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemEditDashboardParameters); }
		}
		public bool IsEmpty {
			get { return !parameterChanges.Where(p => !(new DashboardParametersEqualityComparer().Equals(p.OldParameter, p.NewParameter))).Any(); }
		}
		public DashboardParametersHistoryItem(DashboardParameterCollection parameters, ParameterChangesCollection parametersChanged) {
			this.parameters = parameters;
			this.parameterChanges = parametersChanged;
		}
		public void Undo(DashboardDesigner designer) {
			PerformOperation(designer, false);
			PushUpdateNotifications(designer);
		}
		public virtual void Redo(DashboardDesigner designer) {
			PerformOperation(designer, true);
			PushUpdateNotifications(designer);
		}
		protected virtual void PerformOperation(DashboardDesigner designer, bool redo) {
			ApplyParametersState(designer, redo);
			PrefixNameGenerator nameGenerator = new PrefixNameGenerator("rename");
			Dictionary<string, string> renamingMapA = new Dictionary<string, string>();
			Dictionary<string, string> renamingMapB = new Dictionary<string, string>();
			foreach(ParameterChangeInfo change in parameterChanges.ParametersRenamed) {
				string tempParameterName = nameGenerator.GenerateName(name => parameterChanges.ParametersRenamed.Any(p => p.OldParameter.Name == name || p.NewParameter.Name == name));
				if(redo) {
					renamingMapA.Add(change.OldParameter.Name, tempParameterName);
					renamingMapB.Add(tempParameterName, change.NewParameter.Name);
				}
				else {
					renamingMapA.Add(change.NewParameter.Name, tempParameterName);
					renamingMapB.Add(tempParameterName, change.OldParameter.Name);
				}
			}
			RenameParametersInDataDashboardItems(designer.Dashboard.Items, renamingMapA);
			RenameParametersInDataSources(designer.Dashboard.DataSources, renamingMapA);
			RenameParametersInCalculatedFields(designer.Dashboard.DataSources, renamingMapA);
			RenameParametersInDataDashboardItems(designer.Dashboard.Items, renamingMapB);
			RenameParametersInDataSources(designer.Dashboard.DataSources, renamingMapB);
			RenameParametersInCalculatedFields(designer.Dashboard.DataSources, renamingMapB);
		}
		internal void ApplyParametersState(DashboardDesigner designer, bool redo) {
			parameters.BeginUpdate();
			parameters.Clear();
			if(redo)
				parameters.AddRange(parameterChanges.Where(p => p.NewParameter != null).Select(p => p.NewParameter));
			else
				parameters.AddRange(parameterChanges.Where(p => p.OldParameter != null).Select(p => p.OldParameter));
			parameters.EndUpdate();
			foreach(IDashboardDataSource dataSource in designer.Dashboard.DataSources) {
				dataSource.SetParameters(parameters);
			}
		}
		protected virtual IEnumerable<IDashboardDataSource> GetForcedDataSources(IEnumerable<IDashboardDataSource> dataSources) {
			return dataSources;
		}
		void PushUpdateNotifications(DashboardDesigner designer) {		   
			new HistoryItemDataSourceUpdater(designer.Dashboard, parameterChanges).Update(GetForcedDataSources(designer.Dashboard.DataSources));
		}
		void RenameParametersInDataDashboardItems(IEnumerable<DashboardItem> items, Dictionary<string, string> renamingMap) {
			foreach(DashboardItem item in items) {
				DataDashboardItem dataDashboardItem = item as DataDashboardItem;
				if(dataDashboardItem == null)
					continue;
				dataDashboardItem.FilterString = NamesCriteriaPatcher.Process(dataDashboardItem.FilterString, renamingMap, false);
			}
		}
		void RenameParametersInDataSources(IEnumerable<IDashboardDataSource> dataSources, Dictionary<string, string> renamingMap) {
			foreach(IDashboardDataSource dataSource in dataSources) {
				IParametersRenamer renamer = dataSource as IParametersRenamer;
				if(renamer!=null)
					renamer.RenameParameters(renamingMap);
			}
		}
		void RenameParametersInCalculatedFields(IEnumerable<IDashboardDataSource> dataSources, Dictionary<string, string> renamingMap) {
			foreach(IDashboardDataSource dataSource in dataSources)
				foreach(KeyValuePair<string, string> change in renamingMap) {
					if(dataSource.CalculatedFields != null) {
						string oldParameterName = string.Format(CalculatedFieldsController.ParameterExpressionFormatString, change.Key);
						string newParameterName = string.Format(CalculatedFieldsController.ParameterExpressionFormatString, change.Value);
						foreach(CalculatedField field in dataSource.CalculatedFields)
							if(field.Expression != null)
								if(field.Expression.Contains(oldParameterName))
									field.Expression = field.Expression.Replace(oldParameterName, newParameterName);
					}
				}
		}
	}
}
