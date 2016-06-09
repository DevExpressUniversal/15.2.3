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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	using IndexesList = List<Tuple<int, DashboardItemFormatRule>>;
	public class ClearDataItemFormatRuleHistoryItem : DashboardItemHistoryItem<DataDashboardItem> {
		readonly IndexesList clearedFormatRules = new IndexesList();
		readonly DataItem dataItem;
		public override string Caption { get { return string.Format(DashboardWinLocalizer.GetString(CaptionId), dataItem.DisplayName, DashboardItem.Name); } }
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemFormatRuleClear; } }
		public ClearDataItemFormatRuleHistoryItem(DataDashboardItem dataDashboardItem, DataItem dataItem)
			: base(dataDashboardItem) {
			this.dataItem = dataItem;
		}
		protected override void PerformUndo() {
			DashboardItem.FormatRulesInternal.BeginUpdate();
			for(int index = clearedFormatRules.Count - 1; index >= 0; index--) {
				Tuple<int, DashboardItemFormatRule> tuple = clearedFormatRules[index];
				DashboardItem.FormatRulesInternal.Insert(tuple.Item1, tuple.Item2);
			}
			DashboardItem.FormatRulesInternal.EndUpdate();
			clearedFormatRules.Clear();
		}
		protected override void PerformRedo() {
			DashboardItem.FormatRulesInternal.BeginUpdate();
			for(int index = DashboardItem.FormatRulesInternal.Count - 1; index >= 0; index--) {
				DashboardItemFormatRule rule = DashboardItem.FormatRulesInternal[index];
				if(rule.LevelCore.Item == dataItem) {
					clearedFormatRules.Add(new Tuple<int, DashboardItemFormatRule>(index, rule));
					DashboardItem.FormatRulesInternal.RemoveAt(index);
				}
			}
			DashboardItem.FormatRulesInternal.EndUpdate();
		}
	}
}
