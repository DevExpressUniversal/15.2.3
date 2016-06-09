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
using System.Drawing;
using System.Reflection;
using DevExpress.Data.Utils;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Localization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.DashboardWin.ServiceModel;
using System.Xml.Linq;
namespace DevExpress.DashboardWin.Commands {
	public abstract class DashboardDataSourceCommand : DashboardCommand {
		protected DataSourceInfo DataSourceInfo { get { return Control.SelectedDataSourceInfo; } }
		protected IDashboardDataSource DataSource { get { return Control.SelectedDataSource; } }
		protected string DataMember { get { return Control.SelectedDataMember; } }
		protected Dashboard Dashboard { get { return Control.Dashboard; } }
		protected DashboardDataSourceCommand(DashboardDesigner designer)
			: base(designer) {
		}
	}
	public abstract class DashboardEditDataSourceCommand : DashboardDataSourceCommand {
		protected DashboardEditDataSourceCommand(DashboardDesigner designer)
			: base(designer) {
		}
		XElement previousDataSourceState;
		protected XElement PreviousDataSourceState { get { return previousDataSourceState; } }
		protected abstract EditDataSourceHistoryItemBase CreateHistoryItem(IDashboardDataSource dataSource);
		protected virtual void SavePreviousDataSourceState() {
			previousDataSourceState = DataSource.SaveToXml();
		}
		protected override void ExecuteInternal(ICommandUIState state) {
			IDesignerUpdateService designerUpdateService = ((IServiceProvider)Control).GetService<IDesignerUpdateService>();
			Dashboard.BeginUpdate();
			try {
				if(designerUpdateService != null)
					designerUpdateService.SuspendUpdate();
				try {
					SavePreviousDataSourceState();
					bool result = RunAction();
					if(result) {
						EditDataSourceHistoryItemBase historyItem = CreateHistoryItem(DataSource);
						Control.History.RedoAndAdd(historyItem);
					}
				} finally {
					if(designerUpdateService != null)
						designerUpdateService.ResumeUpdate();
				}
			} finally {
				Dashboard.EndUpdate();
			}
		}
		protected virtual bool RunAction() { return false; }
	}
}
