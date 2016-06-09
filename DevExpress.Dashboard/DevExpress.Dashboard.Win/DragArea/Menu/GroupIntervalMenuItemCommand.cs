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
namespace DevExpress.DashboardWin.Native {
	public class TextGroupIntervalMenuItemCommand : DimensionMenuItemCommand {
		readonly TextGroupInterval groupInterval;
		public override bool Checked { get { return groupInterval == Dimension.TextGroupInterval; } }
		public override string Caption { get { return GroupIntervalCaptionProvider.GetTextGroupIntervalCaption(groupInterval); } }
		public TextGroupIntervalMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, TextGroupInterval groupInterval)
			: base(designer, dashboardItem, dimension) {
			this.groupInterval = groupInterval;
		}
		public override void Execute() {
			if (!Checked) {
				DashboardDesigner designer = Designer;
				GroupIntervalHistoryItem historyItem = new GroupIntervalHistoryItem(DashboardItem, Dimension, groupInterval);
				historyItem.Redo(designer);
				designer.History.Add(historyItem);
			}
		}
	}
	public class NumericGroupIntervalMenuItemCommand : DimensionMenuItemCommand {
		readonly bool isDiscreteScale;
		public override bool Checked { get { return isDiscreteScale == Dimension.IsDiscreteNumericScale; } }
		public override string Caption { get { return GroupIntervalCaptionProvider.GetNumericGroupIntervalCaption(isDiscreteScale); } }
		public NumericGroupIntervalMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, bool isDiscreteNumber)
			: base(designer, dashboardItem, dimension) {
			this.isDiscreteScale = isDiscreteNumber;
		}
		public override void Execute() {
			if (!Checked) {
				DashboardDesigner designer = Designer;
				GroupIntervalHistoryItem historyItem = new GroupIntervalHistoryItem(DashboardItem, Dimension, isDiscreteScale);
				historyItem.Redo(designer);
				designer.History.Add(historyItem);
			}
		}
	}
	public class DateTimeGroupIntervalMenuItemCommand : DimensionMenuItemCommand {
		readonly DateTimeGroupInterval groupInterval;
		public override bool Checked { get { return groupInterval == Dimension.DateTimeGroupInterval; } }
		public override string Caption { get { return GroupIntervalCaptionProvider.GetDateTimeGroupIntervalCaption(groupInterval); } }
		public DateTimeGroupIntervalMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, DateTimeGroupInterval groupInterval)
			: base(designer, dashboardItem, dimension) {
			this.groupInterval = groupInterval;
		}
		public override void Execute() {
			if  (!Checked) {
				DashboardDesigner designer = Designer;
				GroupIntervalHistoryItem historyItem = new GroupIntervalHistoryItem(DashboardItem, Dimension, groupInterval);
				designer.History.RedoAndAdd(historyItem);
			}
		}
	}
}
