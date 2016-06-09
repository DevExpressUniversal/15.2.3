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

using System.Text;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP {
	public abstract class SortByTempMember : QueryTempMember {
		static string GetName(OLAPCubeColumn column) {
			return OLAPDataSourceQueryBase.MeasuresStringName +
				".[XtraPivotGrid " + column.GetHashCode().ToString() + " Sort]";
		}
		OLAPCubeColumn column;
		protected SortByTempMember(OLAPCubeColumn column)
			: base(true, GetName(column), "") {
			this.column = column;
			MDX = GetSortBy(column);
		}
		public OLAPCubeColumn Column { get { return column; } }
		protected abstract string GetSortBy(OLAPCubeColumn column);
	}
	public class SortByTempMember2005 : SortByTempMember {
		public SortByTempMember2005(OLAPCubeColumn column)
			: base(column) {
		}
		protected override string GetSortBy(OLAPCubeColumn column) {
			if(column.SortBySummary == null) {
				StringBuilder result = new StringBuilder();
				result.Append(column.Metadata.Hierarchy).Append(".currentmember.");
				switch(column.SortMode) {
					case PivotSortMode.ID:
						result.Append("properties('id', typed)");
						break;
					case PivotSortMode.Key:
						result.Append("properties(\"key\", typed)");
						break;
					case PivotSortMode.DisplayText:
						result.Append("member_caption");
						break;
					case PivotSortMode.Default:
					case PivotSortMode.DimensionAttribute: {
							string sp = column.ActualSortProperty;
							if(!string.IsNullOrEmpty(sp))
								result.Append("properties(\"").Append(sp).Append("\", typed)");
							else
								result.Append("member_value");
						}
						break;
					case PivotSortMode.Custom:
					case PivotSortMode.Value:
						result.Append("member_value");
						break;
				}
				return result.ToString();
			} else
				return column.GetSortBySummaryMDX();
		}
	}
	public class SortByTempMember2000 : SortByTempMember {
		public SortByTempMember2000(OLAPCubeColumn column)
			: base(column) {
		}
		protected override string GetSortBy(OLAPCubeColumn column) {
			if(column.SortBySummary == null) {
				StringBuilder result = new StringBuilder();
				if(column.SortMode == PivotSortMode.ID)
					result.Append("strtovalue(");
				result.Append(column.Hierarchy).Append(".currentmember.properties(\"");
				switch(column.SortMode) {
					case PivotSortMode.ID:
						result.Append("id");
						break;
					case PivotSortMode.Key:
						result.Append("key");
						break;
					case PivotSortMode.DimensionAttribute:
					case PivotSortMode.DisplayText:
					case PivotSortMode.Custom:
					case PivotSortMode.Default:
					case PivotSortMode.Value:
						result.Append("caption");
						break;
				}
				result.Append("\")");
				if(column.SortMode == PivotSortMode.ID)
					result.Append(")");
				return result.ToString();
			} else
				return column.GetSortBySummaryMDX();
		}
	}
}
