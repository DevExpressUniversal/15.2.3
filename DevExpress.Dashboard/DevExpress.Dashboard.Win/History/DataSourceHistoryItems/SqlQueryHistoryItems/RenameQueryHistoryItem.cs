﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Xml.Linq;
namespace DevExpress.DashboardWin.Native {
	public class RenameQueryHistoryItem : EditDataSourceHistoryItemBase {
		readonly string previousQueryName = string.Empty;
		readonly string nextQueryName = string.Empty;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemRenameQuery; } }
		protected override string NextSelectedDataMember { get { return nextQueryName; } }
		public override string Caption { get { return string.Format(DashboardWinLocalizer.GetString(CaptionId), previousQueryName); } }
		public RenameQueryHistoryItem(IDashboardDataSource dataSource, XElement previousState, string oldQueryName, string newQueryName)
			: base(dataSource, previousState) {
				this.previousQueryName = oldQueryName;
				this.nextQueryName = newQueryName;
		}
		protected override void RedoInternal(DashboardDesigner designer) {
			base.RedoInternal(designer);
			DashboardDataSourceUpdater.RenameDataMember(designer.Dashboard, DataSource, previousQueryName, nextQueryName);
		}
		protected override void UndoInternal(DashboardDesigner designer) {
			base.UndoInternal(designer);
			DashboardDataSourceUpdater.RenameDataMember(designer.Dashboard, DataSource, nextQueryName, previousQueryName);
		}
	}
}
