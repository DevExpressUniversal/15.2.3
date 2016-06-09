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

using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon.Native {
	public class ScatterAxisXMeasureHolder : DataItemHolder<Measure> {
		readonly ScatterChartDashboardItem scatterDashboardItem;
		protected internal override Measure DataItem { get { return scatterDashboardItem.AxisXMeasure; } set { scatterDashboardItem.AxisXMeasure = value; } }
		protected internal override string Caption { get { return DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisXMeasure); } }
		public ScatterAxisXMeasureHolder(ScatterChartDashboardItem scatterDashboardItem) {
			this.scatterDashboardItem = scatterDashboardItem;
		}
	}
	public class ScatterAxisYMeasureHolder : DataItemHolder<Measure> {
		readonly ScatterChartDashboardItem scatterDashboardItem;
		protected internal override Measure DataItem { get { return scatterDashboardItem.AxisYMeasure; } set { scatterDashboardItem.AxisYMeasure = value; } }
		protected internal override string Caption { get { return DashboardLocalizer.GetString(DashboardStringId.DescriptionItemAxisYMeasure); } }
		public ScatterAxisYMeasureHolder(ScatterChartDashboardItem scatterDashboardItem) {
			this.scatterDashboardItem = scatterDashboardItem;
		}
	}
	public class ChartScatterWeightHolder : DataItemHolder<Measure> {
		readonly ScatterChartDashboardItem scatterDashboardItem;
		protected internal override Measure DataItem { get { return scatterDashboardItem.Weight; } set { scatterDashboardItem.Weight = value; } }
		protected internal override string Caption { get { return DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasureWeight); } }
		public ChartScatterWeightHolder(ScatterChartDashboardItem scatterDashboardItem) {
			this.scatterDashboardItem = scatterDashboardItem;
		}
	}
}
