#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheGroupValuesBy (enum)
	public enum PivotCacheGroupValuesBy {
		Days,
		Hours,
		Minutes,
		Months,
		Quarters,
		NumericRanges,
		Seconds,
		Years
	}
	#endregion
	#region PivotCacheFieldGroup
	public class PivotCacheFieldGroup {
		PivotCacheDiscreteGroupingProperties discreteGroupingProperties;
		PivotCacheRangeGroupingProperties rangeGroupingProperties;
		PivotCacheGroupItems groupItems;
		int fieldBase;
		int parent;
		public PivotCacheFieldGroup(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			discreteGroupingProperties = new PivotCacheDiscreteGroupingProperties(documentModel);
			rangeGroupingProperties = new PivotCacheRangeGroupingProperties(documentModel);
			groupItems = new PivotCacheGroupItems(documentModel);
			fieldBase = -1;
			parent = -1;
		}
		public PivotCacheDiscreteGroupingProperties DiscreteGroupingProperties { get { return discreteGroupingProperties; } }
		public PivotCacheRangeGroupingProperties RangeGroupingProperties { get { return rangeGroupingProperties; } }
		public PivotCacheGroupItems GroupItems { get { return groupItems; } }
		public int FieldBase { get { return fieldBase; } set { fieldBase = value; } }
		public int Parent { get { return parent; } set { parent = value; } }
		public bool Initialized {
			get {
				return GroupItems.Count != 0 || DiscreteGroupingProperties.Count != 0 ||
					RangeGroupingProperties.HasGroup || Parent != -1;
			}
		}
		public void CopyFrom(PivotCacheFieldGroup source) {
			discreteGroupingProperties.CopyFrom(source.discreteGroupingProperties);
			rangeGroupingProperties.CopyFrom(source.rangeGroupingProperties);
			this.groupItems.InnerList.Capacity = source.groupItems.Count;
			foreach (var sourceGroupItem in source.groupItems) {
				var targetGroupItem = sourceGroupItem.Clone();
				this.groupItems.AddWithoutHistoryAndNotifications(targetGroupItem);
			}
			this.fieldBase = source.fieldBase;
			this.parent = source.parent;
		}
	}
	#endregion
	#region PivotCacheRangeGroupingProperties
	public class PivotCacheRangeGroupingProperties {
		readonly DocumentModel documentModel;
		bool autoStart;
		bool autoEnd;
		PivotCacheGroupValuesBy groupBy;
		double groupInterval;
		double startNum;
		double endNum;
		DateTime startDate;
		DateTime endDate;
		public PivotCacheRangeGroupingProperties(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			autoStart = true;
			autoEnd = true;
			groupBy = PivotCacheGroupValuesBy.NumericRanges;
			groupInterval = 1;
			startNum = double.MaxValue;
			endNum = double.MinValue;
			startDate = DateTime.MaxValue;
			endDate = DateTime.MinValue;
		}
		public bool HasGroup { 
			get {
				if (autoStart && autoEnd && groupInterval == 1 && groupBy == PivotCacheGroupValuesBy.NumericRanges)
					if (startNum == double.MaxValue && endNum == double.MinValue)
						if (startDate == DateTime.MaxValue && endDate == DateTime.MinValue)
							return false;
				return true;
			} 
		}
		#region AutoEnd
		public bool AutoEnd { get { return autoEnd; } set { HistoryHelper.SetValue(documentModel, autoEnd, value, SetAutoEndCore); } }
		protected internal void SetAutoEndCore(bool value) {
			autoEnd = value;
		}
		#endregion
		#region AutoStart
		public bool AutoStart { get { return autoStart; } set { HistoryHelper.SetValue(documentModel, autoStart, value, SetAutoStartCore); } }
		protected internal void SetAutoStartCore(bool value) {
			autoStart = value;
		}
		#endregion
		#region GroupBy
		public PivotCacheGroupValuesBy GroupBy { get { return groupBy; } set { HistoryHelper.SetValue(documentModel, groupBy, value, SetGroupByCore); } }
		protected internal void SetGroupByCore(PivotCacheGroupValuesBy value) {
			groupBy = value;
		}
		#endregion
		#region GroupInterval
		public double GroupInterval { get { return groupInterval; } set { HistoryHelper.SetValue(documentModel, groupInterval, value, SetGroupIntervalCore); } }
		protected internal void SetGroupIntervalCore(double value) {
			groupInterval = value;
		}
		#endregion
		#region StartNum
		public double StartNum { get { return startNum; } set { HistoryHelper.SetValue(documentModel, startNum, value, SetStartNumCore); } }
		protected internal void SetStartNumCore(double value) {
			startNum = value;
		}
		#endregion
		#region EndNum
		public double EndNum { get { return endNum; } set { HistoryHelper.SetValue(documentModel, endNum, value, SetEndNumCore); ; } }
		protected internal void SetEndNumCore(double value) {
			endNum = value;
		}
		#endregion
		#region StartDate
		public DateTime StartDate { get { return startDate; } set { HistoryHelper.SetValue(documentModel, startDate, value, SetStartDateCore); ; } }
		protected internal void SetStartDateCore(DateTime value) {
			startDate = value;
		}
		#endregion
		#region EndDate
		public DateTime EndDate { get { return endDate; } set { HistoryHelper.SetValue(documentModel, endDate, value, SetEndDateCore); ; } }
		protected internal void SetEndDateCore(DateTime value) {
			endDate = value;
		}
		public void CopyFrom(PivotCacheRangeGroupingProperties source) {
			this.autoStart = source.autoStart;
			this.autoEnd = source.autoEnd;
			this.groupBy = source.groupBy;
			this.groupInterval = source.groupInterval;
			this.startNum = source.startNum;
			this.endNum = source.endNum;
			this.startDate = source.startDate;
			this.endDate = source.endDate;
		}
		#endregion
	}
	#endregion
	#region PivotCacheDiscreteGroupingProperties
	public class PivotCacheDiscreteGroupingProperties : UndoableCollection<IPivotCacheRecordValue> { 
		public PivotCacheDiscreteGroupingProperties(IDocumentModelPart documentModel)
			: base(documentModel) {
		}
		public void CopyFrom(PivotCacheDiscreteGroupingProperties source) {
			this.Capacity = source.Count;
			Debug.Assert(this.Count == 0);
			foreach (IPivotCacheRecordValue sourceItem in source) {
				IPivotCacheRecordValue targetItem = sourceItem.Clone();
				this.AddWithoutHistoryAndNotifications(targetItem);
			}
		}
	}
	#endregion
	#region PivotCacheGroupItems
	public class PivotCacheGroupItems : UndoableCollection<IPivotCacheRecordValue> {
		public PivotCacheGroupItems(IDocumentModelPart documentModel)
			: base(documentModel) {
		}
	}
	#endregion
}
