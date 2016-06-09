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
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public class CalculatedFieldInfo {
		public string Expression { get; set; }
	}
	public class EditCalculatedFieldCommand : DashboardDataSourceCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.EditCalculatedField; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandAddCalculatedFieldCaption; } } 
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandAddCalculatedFieldDescription; } } 
		public EditCalculatedFieldCommand(DashboardDesigner control)
			: base(control) {
		}
		protected CalculatedFieldInfo EditCalculatedField(CalculatedField cField) {
			IServiceProvider serviceProvider = Control;
			if(ReferenceEquals(null, CriteriaOperator.GetCustomFunction(DashboardDistinctCountFunction.ExpressionEditorName)))
				CriteriaOperator.RegisterCustomFunction(new DashboardDistinctCountFunction(DashboardDistinctCountFunction.ExpressionEditorName));
			try {
				using(CalculatedFieldExpressionEditorForm form = new CalculatedFieldExpressionEditorForm(cField, DataSourceInfo, Dashboard.Parameters, serviceProvider)) {
					IDashboardGuiContextService guiService = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
					if (form.ShowDialog(guiService.Win32Window) == DialogResult.OK) {
						CalculatedFieldInfo calculatedFieldInfo = new CalculatedFieldInfo();
						try {
							CriteriaOperator val = CriteriaOperator.Parse(form.Expression);
							if(!ReferenceEquals(null, val))
								calculatedFieldInfo.Expression = CriteriaOperator.ToString(val.Accept(new CustomFunctionReplacer(DashboardDistinctCountFunction.ExpressionEditorName, DashboardDistinctCountFunction.FunctionName)));
							else
								calculatedFieldInfo.Expression = null;
						} catch {
							calculatedFieldInfo.Expression = null;
						}
						if(DataSource.IsSqlServerMode(cField.DataMember))
							DataSource.SetCalculatedFieldIsServerEvaluable(cField, true);
						return calculatedFieldInfo;
					}
				}
				return null;
			} finally {
				CriteriaOperator.UnregisterCustomFunction(CriteriaOperator.GetCustomFunction(DashboardDistinctCountFunction.ExpressionEditorName));
			}
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			EditCalculatedFieldCommandUIState editCalcFieldState = (EditCalculatedFieldCommandUIState)state;
			CalculatedField cField = editCalcFieldState.CalculatedField;
			IDashboardDataSource dataSource = editCalcFieldState.DataSource;
			CalculatedFieldInfo calculatedFieldInfo = EditCalculatedField(cField);
			if(calculatedFieldInfo != null) {
				IServiceProvider serviceProvider = Control;
				IDashboardDesignerHistoryService historyService = serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>();
				historyService.RedoAndAdd(new EditCalculatedFieldExpressionHistoryItem(cField, calculatedFieldInfo.Expression, dataSource));
			}
		}
	}
	public class EditCalculatedFieldCommandUIState : ICommandUIState {
		public CalculatedField CalculatedField { get; private set; }
		public IDashboardDataSource DataSource { get; private set; }
		public EditCalculatedFieldCommandUIState(CalculatedField calculatedField, IDashboardDataSource dataSource) {
			Guard.ArgumentNotNull(calculatedField, "calculatedField");
			Guard.ArgumentNotNull(dataSource, "dataSource");
			CalculatedField = calculatedField;
			DataSource = dataSource;
		}
		#region ICommandUIState members
		bool ICommandUIState.Checked { get { return false; } set { } }
		object ICommandUIState.EditValue { get { return null; } set { } }
		bool ICommandUIState.Enabled { get { return true; } set { } }
		bool ICommandUIState.Visible { get { return true; } set { } }
		#endregion
	}
}
