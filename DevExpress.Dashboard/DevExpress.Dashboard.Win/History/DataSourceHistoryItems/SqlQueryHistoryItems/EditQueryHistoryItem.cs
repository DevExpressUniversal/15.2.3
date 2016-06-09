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
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
namespace DevExpress.DashboardWin.Native {
	public class EditQueryHistoryItem : EditQueryHistoryItemBase {
		readonly Dictionary<string, string> dataMemberRenamingMap;
		readonly string queryName;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemEditQuery; } }
		protected override string NextSelectedDataMember { get { return queryName; } }
		public override string Caption { get { return String.Format(DashboardWinLocalizer.GetString(CaptionId), queryName); } }
		public EditQueryHistoryItem(IDashboardDataSource dataSource, XElement previousState, DashboardParameterCollection parameters, ParameterChangesCollection parametersChanged, string queryName, Dictionary<string, string> dataMemberRenamingMap) :
			base(dataSource, previousState, parameters, parametersChanged) {
				this.queryName = queryName;
				this.dataMemberRenamingMap = dataMemberRenamingMap;
		}
		protected override void RedoInternal(DashboardDesigner designer) {
			base.RedoInternal(designer);
			DashboardDataSourceUpdater.RenameColumns(designer.Dashboard, DataSource, queryName, dataMemberRenamingMap);
		}
		protected override void UndoInternal(DashboardDesigner designer) {
			base.UndoInternal(designer);
			DashboardDataSourceUpdater.RenameColumns(designer.Dashboard, DataSource, queryName, dataMemberRenamingMap.ToDictionary(kp => kp.Value, kp => kp.Key));
		}
	}
}
