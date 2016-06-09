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

using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheDimensionCollection
	public class PivotCacheDimensionCollection : List<PivotCacheDimension> {
	}
	#endregion
	#region PivotCacheDimension
	public class PivotCacheDimension {
		bool measure;
		string caption;
		string name;
		string uniqueName;
		public bool Measure { get { return measure; } set { measure = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string Name { get { return name; } set { name = value; } }
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public void CopyFrom(PivotCacheDimension sourceItem) {
			this.measure = sourceItem.measure;
			this.caption = sourceItem.caption;
			this.name = sourceItem.name;
			this.uniqueName = sourceItem.uniqueName;
		}
	}
	#endregion
	#region PivotCacheKpiCollection
	public class PivotCacheKpiCollection : List<PivotCacheKpi> {
	}
	#endregion
	#region PivotCacheKpi
	public class PivotCacheKpi {
		string uniqueName;
		string caption;
		string displayFolder;
		string measureGroupName;
		string parent;
		string valueUniqueName;
		string goalUniqueName;
		string statusUniqueName;
		string trendUniqueName;
		string weightUniqueName;
		string timeMemberUniqueName;
		public string UniqueName { get { return uniqueName; } set { uniqueName = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public string DisplayFolder { get { return displayFolder; } set { displayFolder = value; } }
		public string MeasureGroupName { get { return measureGroupName; } set { measureGroupName = value; } }
		public string Parent { get { return parent; } set { parent = value; } }
		public string ValueUniqueName { get { return valueUniqueName; } set { valueUniqueName = value; } }
		public string GoalUniqueName { get { return goalUniqueName; } set { goalUniqueName = value; } }
		public string StatusUniqueName { get { return statusUniqueName; } set { statusUniqueName = value; } }
		public string TrendUniqueName { get { return trendUniqueName; } set { trendUniqueName = value; } }
		public string WeightUniqueName { get { return weightUniqueName; } set { weightUniqueName = value; } }
		public string TimeMemberUniqueName { get { return timeMemberUniqueName; } set { timeMemberUniqueName = value; } }
		public void CopyFrom(PivotCacheKpi source) {
			this.uniqueName = source.uniqueName;
			this.caption = source.caption;
			this.displayFolder = source.displayFolder;
			this.measureGroupName = source.measureGroupName;
			this.parent = source.parent;
			this.valueUniqueName = source.valueUniqueName;
			this.goalUniqueName = source.goalUniqueName;
			this.statusUniqueName = source.statusUniqueName;
			this.trendUniqueName = source.trendUniqueName;
			this.weightUniqueName = source.weightUniqueName;
			this.timeMemberUniqueName = source.timeMemberUniqueName;
		}
	}
	#endregion
	#region PivotCacheDimensionMapCollection
	public class PivotCacheDimensionMapCollection : List<PivotCacheDimensionMap> {
	}
	#endregion
	#region PivotCacheDimensionMap
	public class PivotCacheDimensionMap {
		int dimension = int.MinValue;
		int measureGroup = int.MinValue;
		public int Dimension { get { return dimension; } set { dimension = value; } }
		public int MeasureGroup { get { return measureGroup; } set { measureGroup = value; } }
		public void CopyFrom(PivotCacheDimensionMap source) {
			this.dimension = source.dimension;
			this.measureGroup = source.measureGroup;
		}
	}
	#endregion
	#region PivotCacheMeasureGroupCollection
	public class PivotCacheMeasureGroupCollection : List<PivotCacheMeasureGroup> {
	}
	#endregion
	#region PivotCacheMeasureGroup
	public class PivotCacheMeasureGroup {
		string caption;
		string name;
		public string Caption { get { return caption; } set { caption = value; } }
		public string Name { get { return name; } set { name = value; } }
		public void CopyFrom(PivotCacheMeasureGroup sourceItem) {
			this.caption = sourceItem.Caption;
			this.name = sourceItem.Name;
		}
	}
	#endregion
}
