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
	public class RangeFilterArgumentHolder : DataItemHolder<Dimension> {
		readonly RangeFilterDashboardItem rangeFilter;
		protected internal override Dimension DataItem { get { return rangeFilter.Argument; } set { rangeFilter.Argument = value; } }
		protected internal override string Caption { get { return DashboardLocalizer.GetString(DashboardStringId.DescriptionItemArgument); } }
		protected internal override DataFieldType[] CompatibleTypesRestriction { get { return new[] { DataFieldType.DateTime, DataFieldType.Integer, DataFieldType.Float, DataFieldType.Double, DataFieldType.Decimal }; } }
		public RangeFilterArgumentHolder(RangeFilterDashboardItem rangeFilter) {
			this.rangeFilter = rangeFilter;
		}
		protected override DataItem AdjustInternal(Dimension dataItem) {
			if(!dataItem.IsDiscreteNumericScale && !RangeFilterDashboardItem.IncompatibleDateTimeGroupIntervalsMapping.ContainsKey(dataItem.DateTimeGroupInterval) &&
				dataItem.SortOrder == DimensionSortOrder.Ascending && dataItem.SortByMeasure == null && !dataItem.TopNOptions.Enabled)
				return dataItem;
			DateTimeGroupInterval dateTimeGroupInterval;
			if(!RangeFilterDashboardItem.IncompatibleDateTimeGroupIntervalsMapping.TryGetValue(dataItem.DateTimeGroupInterval, out dateTimeGroupInterval))
				dateTimeGroupInterval = dataItem.DateTimeGroupInterval;
			return new Dimension(dataItem.ID, new DimensionDefinition(dataItem.DataMember, dateTimeGroupInterval, dataItem.TextGroupInterval));
		}
	}
}
