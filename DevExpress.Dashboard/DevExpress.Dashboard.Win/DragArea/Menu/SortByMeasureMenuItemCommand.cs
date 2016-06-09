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
	public class SortByMeasureMenuItemCommand : DimensionMenuItemCommand, IComparable<SortByMeasureMenuItemCommand> {
		readonly Measure measure;
		public override bool CanExecute { get { return base.CanExecute && !Dimension.TopNOptions.Enabled; } }
		public override string Caption { get { return measure.DisplayName; } }
		public override bool Checked {
			get {
				Measure sortByMeasure = Dimension.ActualSortByMeasure;
				return sortByMeasure != null && sortByMeasure.EqualsByDefinition(measure);
			}
		}
		public SortByMeasureMenuItemCommand(DashboardDesigner designer, DataDashboardItem dashboardItem, Dimension dimension, Measure measure)
			: base(designer, dashboardItem, dimension) {
			this.measure = measure;
		}
		public override void Execute() {
			Dimension dimension = Dimension;
			DimensionSortOrder newSortOrder = dimension.SortOrder == DimensionSortOrder.None ? DimensionSortOrder.Ascending : dimension.SortOrder;
			if(newSortOrder != dimension.SortOrder || measure != dimension.SortByMeasure) {
				SortingHistoryItem historyItem = new SortingHistoryItem(DashboardItem, dimension, newSortOrder, measure, dimension.SortMode);
				Designer.History.RedoAndAdd(historyItem);
			}
		}
		int IComparable<SortByMeasureMenuItemCommand>.CompareTo(SortByMeasureMenuItemCommand command) {
			return Caption.CompareTo(command.Caption);
		}
	}
}
