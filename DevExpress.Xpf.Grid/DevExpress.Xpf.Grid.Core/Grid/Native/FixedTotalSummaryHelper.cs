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

using DevExpress.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Grid.Native {
	public static class FixedTotalSummaryHelper {
		public static string GetFixedSummariesString(IList<GridTotalSummaryData> totalSummaryDataList) {
			if(totalSummaryDataList == null)
				return String.Empty;
			string result = String.Empty;
			foreach(GridTotalSummaryData data in totalSummaryDataList) {
				if(!String.IsNullOrEmpty(result))
					result += GridControlLocalizer.GetString(GridControlStringId.SummaryItemsSeparator);
				result += ColumnBase.GetSummaryText(data, true);
			}
			return result;
		}
		static void GenerateCountTotalSummary(SummaryItemBase item, Func<SummaryItemBase, object> getSummaryValue, IList<GridTotalSummaryData> destination) {
			object summary = getSummaryValue(item);
			destination.Add(new GridTotalSummaryData(item, summary, null));
		}
		public static void GenerateTotalSummaries(IList<SummaryItemBase> source, IColumnCollection ColumnsCore, Func<SummaryItemBase, object> getSummaryValue, IList<GridTotalSummaryData> destination) {
			destination.Clear();
			foreach(SummaryItemBase item in source) {
				if(String.IsNullOrEmpty(item.FieldName) && item.SummaryType == SummaryItemType.Count) {
					GenerateCountTotalSummary(item, getSummaryValue, destination);
					continue;
				}
				ColumnBase column = ColumnsCore[item.FieldName];
				object summary = getSummaryValue(item);
				destination.Add(new GridTotalSummaryData(item, summary, column));
			}
		}
	}
}
