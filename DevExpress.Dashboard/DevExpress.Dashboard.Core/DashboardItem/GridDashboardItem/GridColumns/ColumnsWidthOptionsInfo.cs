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
using System.Linq;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Native {
	public class ColumnsWidthOptionsInfo {
		readonly List<ColumnWidthOptionsInfo> columnsWidthInfo = new List<ColumnWidthOptionsInfo>();
		public GridColumnWidthMode ColumnWidthMode { get; set; }
		public List<ColumnWidthOptionsInfo> ColumnsInfo { get { return columnsWidthInfo; } }
		public ColumnsWidthOptionsInfo Clone() {
			ColumnsWidthOptionsInfo clone = new ColumnsWidthOptionsInfo();
			clone.ColumnWidthMode = ColumnWidthMode;
			foreach(ColumnWidthOptionsInfo column in ColumnsInfo)
				clone.ColumnsInfo.Add(column.Clone());
			return clone;
		}
		public void CalcWeight() {
			double actualWidthSum = 0;
			double weightSum = 0;
			foreach(ColumnWidthOptionsInfo info in ColumnsInfo) {
				if(info.WidthType == GridColumnFixedWidthType.Weight) {
					actualWidthSum += info.ActualWidth;
					weightSum += info.Weight;
				}
			}
			foreach(ColumnWidthOptionsInfo info in ColumnsInfo)
				if(info.WidthType == GridColumnFixedWidthType.Weight) {
					info.Weight = info.ActualWidth * weightSum / actualWidthSum;
					info.InitialWidth = info.Weight;
				}
		}
	}
	public class ColumnWidthOptionsInfo {
		public int ActualIndex { get; set; }
		public int ActualWidth { get; set; }
		public int MinWidth { get; set; }
		public double InitialWidth { get; set; }
		public double Weight { get; set; }
		public GridColumnFixedWidthType WidthType { get; set; }
		public double FixedWidth { get; set; }
		public double DefaultBestCharacterCount { get; set; }
		public GridColumnDisplayMode DisplayMode { get; set; }
		public bool IsFixedWidth { get { return WidthType != GridColumnFixedWidthType.Weight; } }
		public double GetNonEmptyWidth() {
			return ActualWidth != 0 ? ActualWidth : InitialWidth;
		}
		public ColumnWidthOptionsInfo Clone() {
			ColumnWidthOptionsInfo clone = new ColumnWidthOptionsInfo();
			clone.ActualIndex = ActualIndex;
			clone.ActualWidth = ActualWidth;
			clone.InitialWidth = InitialWidth;
			clone.WidthType = WidthType;
			clone.MinWidth = MinWidth;
			clone.Weight = Weight;
			clone.FixedWidth = FixedWidth;
			clone.DefaultBestCharacterCount = DefaultBestCharacterCount;
			clone.DisplayMode = DisplayMode;
			return clone;
		}
	}
}
