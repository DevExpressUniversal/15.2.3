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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils.Commands;
using System;
using System.Collections;
namespace DevExpress.DashboardWin.Commands {
	public class AddCalculatedFieldCommand : EditCalculatedFieldCommand {
		public override DashboardCommandId Id { get { return DashboardCommandId.AddCalculatedField; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandAddCalculatedFieldCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandAddCalculatedFieldDescription; } }
		public override string ImageName { get { return "AddCalculatedField"; } }
		public AddCalculatedFieldCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IDashboardDataSource dataSource = DataSource;
			state.Enabled = dataSource != null && dataSource.GetDataSourceInternal().Properties.CalculatedFieldsSupported;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			IDashboardDataSource dataSource = DataSource;
			DashboardParameterCollection parameters = Dashboard.Parameters;
			IServiceProvider serviceProvider = Control;
			if(dataSource != null) {
				CalculatedField newCalculatedField = new CalculatedField();
				newCalculatedField.DataType = CalculatedFieldType.Object;
				newCalculatedField.DataMember = DataMember;
				CalculatedFieldInfo calculatedFieldInfo = EditCalculatedField(newCalculatedField);
				if(calculatedFieldInfo != null) {
					newCalculatedField.Expression = calculatedFieldInfo.Expression;
					IDashboardDesignerHistoryService historyService = serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>();
					historyService.RedoAndAdd(new AddCalculatedFieldHistoryItem(newCalculatedField, dataSource));
					try {
						IDashboardParameterService service = serviceProvider.RequestServiceStrictly<IDashboardParameterService>();
						Type type = dataSource.GetCalculatedFieldDesiredType(newCalculatedField.Expression, DataMember, newCalculatedField.Name, true, service);
						if(type == null) {
							if(dataSource.IsSqlServerMode(DataMember))
								newCalculatedField.SetAutoType(typeof(string));
						} else {
							type = Nullable.GetUnderlyingType(type) ?? type;
							newCalculatedField.SetAutoType(type);
						}
					} catch { }
				}
			}
		}
	}
}
