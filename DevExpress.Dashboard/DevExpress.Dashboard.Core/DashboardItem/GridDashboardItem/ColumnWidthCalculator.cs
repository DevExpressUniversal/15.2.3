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
namespace DevExpress.DashboardCommon.Native {
	public static class ColumnWidthCalculator {
		public static ColumnsWidthOptionsInfo CalcActualWidth(ColumnsWidthOptionsInfo info, int maxVisibleWidth) {
			ColumnsWidthOptionsInfo optionsInfo = info.Clone();
			if(optionsInfo.ColumnsInfo.Count != 0 && maxVisibleWidth != 0) {
				if(optionsInfo.ColumnWidthMode == GridColumnWidthMode.AutoFitToContents)
					foreach(ColumnWidthOptionsInfo col in optionsInfo.ColumnsInfo)
						col.ActualWidth = (int)col.InitialWidth;
				int actualWidthSum = ActualWidthSum(optionsInfo.ColumnsInfo);
				if((actualWidthSum != maxVisibleWidth) && (optionsInfo.ColumnWidthMode != GridColumnWidthMode.AutoFitToContents || (optionsInfo.ColumnWidthMode == GridColumnWidthMode.AutoFitToContents && actualWidthSum < maxVisibleWidth)))
					CalcWidthCore(optionsInfo, maxVisibleWidth);
			}
			return optionsInfo;
		}
		static void CalcWidthCore(ColumnsWidthOptionsInfo info, int maxVisibleWidth) {
			foreach(ColumnWidthOptionsInfo col in info.ColumnsInfo)
				if(info.ColumnWidthMode != DashboardCommon.GridColumnWidthMode.Manual || col.IsFixedWidth)
					col.ActualWidth = (int)col.InitialWidth;
				else
					col.ActualWidth = 0;
			double actualWidthSumOfColumnsWithFixedWidthState = GetActualWidthSumOfColumnsWithFixedWidthState(info);
			double initialWidthSumOfColumnsWithFixedWidthState = GetInitialWidthSumOfColumnsWithFixedWidthState(info);
			bool initialFixedWidthState = actualWidthSumOfColumnsWithFixedWidthState != 0 && actualWidthSumOfColumnsWithFixedWidthState < initialWidthSumOfColumnsWithFixedWidthState ? true : false;
			CalcWidthsForColumnsWithCurrentFixedWidthState(info, maxVisibleWidth, initialFixedWidthState);
			if(ActualWidthSum(info.ColumnsInfo) != maxVisibleWidth)
				CalcWidthsForColumnsWithCurrentFixedWidthState(info, maxVisibleWidth, !initialFixedWidthState);
		}
		static double GetActualWidthSumOfColumnsWithFixedWidthState(ColumnsWidthOptionsInfo info) {
			double widthSum = 0;
			foreach(ColumnWidthOptionsInfo columnInfo in info.ColumnsInfo)
				if(info.ColumnWidthMode == GridColumnWidthMode.Manual && columnInfo.IsFixedWidth)
					widthSum += columnInfo.ActualWidth;
			return widthSum;
		}
		static double GetInitialWidthSumOfColumnsWithFixedWidthState(ColumnsWidthOptionsInfo info) {
			double widthSum = 0;
			foreach(ColumnWidthOptionsInfo columnInfo in info.ColumnsInfo)
				if(info.ColumnWidthMode == GridColumnWidthMode.Manual && columnInfo.IsFixedWidth)
					widthSum += columnInfo.InitialWidth;
			return widthSum;
		}
		static int ActualWidthSum(List<ColumnWidthOptionsInfo> info) {
			int widthSum = 0;
			foreach(ColumnWidthOptionsInfo columnInfo in info)
				widthSum += columnInfo.ActualWidth;
			return widthSum;
		}
		static double AllWidthSum(List<ColumnWidthOptionsInfo> info) {
			double widthSum = 0;
			foreach(ColumnWidthOptionsInfo columnInfo in info)
				widthSum += columnInfo.GetNonEmptyWidth();
			return widthSum;
		}
		static void CalcWidthsForColumnsWithCurrentFixedWidthState(ColumnsWidthOptionsInfo info, int maxVisibleWidth, bool currentFixedWidthState) {
			double widthSumOfColumnsWithFixedWidthStateNotEqualCurrent = GetWidthSumOfColumnsWithFixedWidthStateNotEqualCurrent(info, currentFixedWidthState);
			double widthSumOfColumnsWithCurrentFixedWidthState = AllWidthSum(info.ColumnsInfo) - widthSumOfColumnsWithFixedWidthStateNotEqualCurrent;
			double newWidthSumOfColumnsWithCurrentFixedWidthState = Math.Abs(maxVisibleWidth - widthSumOfColumnsWithFixedWidthStateNotEqualCurrent);
			foreach(ColumnWidthOptionsInfo columnInfo in info.ColumnsInfo) {
				if(info.ColumnWidthMode != GridColumnWidthMode.Manual || columnInfo.IsFixedWidth == currentFixedWidthState) {
					int correctedWidth = (int)Math.Round(newWidthSumOfColumnsWithCurrentFixedWidthState * (columnInfo.GetNonEmptyWidth()) / widthSumOfColumnsWithCurrentFixedWidthState);
					int finalWidth = correctedWidth >= columnInfo.MinWidth ? correctedWidth : columnInfo.MinWidth;
					if(columnInfo.ActualWidth != finalWidth) {
						columnInfo.ActualWidth = finalWidth;
					}
				}
			}
			ReverseOrderedLinearCorrection(info, maxVisibleWidth, currentFixedWidthState);
		}
		static void ReverseOrderedLinearCorrection(ColumnsWidthOptionsInfo info, int maxVisibleWidth, bool currentFixedWidthState) {
			int actualWidthSum = ActualWidthSum(info.ColumnsInfo);
			for(int i = info.ColumnsInfo.Count - 1; i >= 0; i--) {
				int remainder = actualWidthSum - maxVisibleWidth;
				if(remainder == 0)
					return;
				ColumnWidthOptionsInfo columnInfo = info.ColumnsInfo[i];
				if(info.ColumnWidthMode != GridColumnWidthMode.Manual || columnInfo.IsFixedWidth == currentFixedWidthState) {
					columnInfo.ActualWidth -= remainder;
					actualWidthSum -= remainder;
					if(columnInfo.ActualWidth < columnInfo.MinWidth) {
						actualWidthSum += Math.Abs(columnInfo.ActualWidth - columnInfo.MinWidth);
						columnInfo.ActualWidth = columnInfo.MinWidth;
					}
				}
			}
		}
		static double GetWidthSumOfColumnsWithFixedWidthStateNotEqualCurrent(ColumnsWidthOptionsInfo info, bool currentFixedWidthState) {
			double sumNonSpecificWidth = 0;
			if(info.ColumnWidthMode == GridColumnWidthMode.Manual)
				foreach(ColumnWidthOptionsInfo columnInfo in info.ColumnsInfo)
					if(columnInfo.IsFixedWidth != currentFixedWidthState)
						sumNonSpecificWidth += columnInfo.GetNonEmptyWidth();
			return sumNonSpecificWidth;
		}
	}
}
