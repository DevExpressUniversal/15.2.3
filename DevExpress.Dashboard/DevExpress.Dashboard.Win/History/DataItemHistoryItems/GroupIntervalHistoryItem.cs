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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class GroupIntervalHistoryItem : DataItemHistoryItem {
		readonly TextGroupInterval? textGroupInterval;
		readonly bool? isDiscreteScale;
		readonly DateTimeGroupInterval? dateTimeGroupInterval;
		TextGroupInterval previousTextGroupInterval;
		bool previousIsDiscreteScale;
		DateTimeGroupInterval previousDateTimeGroupInterval;
		string previousFilterString;
		public override string Caption {
			get {
				return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemDimensionGroupInterval), DataItem.DisplayName);
			}
		}
		GroupIntervalHistoryItem(DataDashboardItem dataDashboardItem, Dimension dimension, TextGroupInterval? textGroupInterval, bool? isDiscreteScale, DateTimeGroupInterval? dateTimeGroupInterval)
			: base(dataDashboardItem, dimension) {
			this.textGroupInterval = textGroupInterval;
			this.isDiscreteScale = isDiscreteScale;
			this.dateTimeGroupInterval = dateTimeGroupInterval;
		}
		public GroupIntervalHistoryItem(DataDashboardItem dataDashboardItem, Dimension dimension, TextGroupInterval textGroupInterval)
			: this(dataDashboardItem, dimension, textGroupInterval, null, null) {
		}
		public GroupIntervalHistoryItem(DataDashboardItem dataDashboardItem, Dimension dimension, bool isDiscreteScale)
			: this(dataDashboardItem, dimension, null, isDiscreteScale, null) {
		}
		public GroupIntervalHistoryItem(DataDashboardItem dataDashboardItem, Dimension dimension, DateTimeGroupInterval dateTimeGroupInterval)
			: this(dataDashboardItem, dimension, null, null, dateTimeGroupInterval) {
		}
		protected override void PerformUndo() {
			Dimension dimension = (Dimension)DataItem;
			dimension.TextGroupInterval = previousTextGroupInterval;
			dimension.IsDiscreteNumericScale = previousIsDiscreteScale;
			dimension.DateTimeGroupInterval = previousDateTimeGroupInterval;
			DashboardItem.FilterString = previousFilterString;
		}
		protected override void PerformRedo() {
			previousFilterString = DashboardItem.FilterString;
			Dimension dimension = (Dimension)DataItem;
			previousTextGroupInterval = dimension.TextGroupInterval;
			previousIsDiscreteScale = dimension.IsDiscreteNumericScale;
			previousDateTimeGroupInterval = dimension.DateTimeGroupInterval;
			dimension.TextGroupInterval = textGroupInterval ?? DimensionDefinition.DefaultTextGroupInterval;
			dimension.IsDiscreteNumericScale = isDiscreteScale ?? Dimension.DefaultIsDiscreteNumericScale;
			dimension.DateTimeGroupInterval = dateTimeGroupInterval ?? DimensionDefinition.DefaultDateTimeGroupInterval; 
		}
	}
}
