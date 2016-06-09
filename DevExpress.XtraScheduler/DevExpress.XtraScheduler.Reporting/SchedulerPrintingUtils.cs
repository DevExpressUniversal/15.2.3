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
using DevExpress.XtraScheduler.Reporting.Native;
using System.Collections;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraScheduler.Reporting.Native {   
	public static class DesignUtils {
		public static PropertyInfo GetAllowedProperty(Type type, string propertyName) {
			return type.GetProperty(propertyName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
		}
		public static bool IsPropertyAllowed(Type type, string propertyName) {
			return GetAllowedProperty(type, propertyName) != null;
		}
		public static void ArrangeControlRight(ReportViewControlBase masterControl, ReportViewControlBase control) {
			control.LocationF = new PointF(masterControl.RightF, masterControl.TopF);
			control.HeightF = masterControl.HeightF;
		}
		public static void ArrangeControlBottom(ReportViewControlBase masterControl, ReportViewControlBase control) {
			control.LocationF = new PointF(masterControl.LeftF, masterControl.BottomF);
			control.WidthF = masterControl.WidthF;
		}
		public static void ArrangeControlTop(ReportViewControlBase masterControl, ReportViewControlBase control) {
			control.LocationF = new PointF(masterControl.LeftF, masterControl.TopF - control.HeightF);
			control.WidthF = masterControl.WidthF;
		}
		public static void ArrangeControlLeft(ReportViewControlBase masterControl, ReportRelatedControlBase control) {
			control.LocationF = new PointF(masterControl.LeftF - control.WidthF, masterControl.TopF);
			control.HeightF = masterControl.HeightF;
		}
	}
	public static class SchedulerPrintingUtils {
		public static List<object> FindObjectsByType(object sourceReport, FieldInfo[] fieldInfos, Type type) {
			List<object> result = new List<object>();
			for (int i = 0; i < fieldInfos.Length; i++) {
				Type fType = fieldInfos[i].FieldType;
				if (type.IsAssignableFrom(fType))
					result.Add(fieldInfos[i].GetValue(sourceReport));
			}
			return result;
		}
		public static int[] SplitVisibleColumns(int totalValuesCount, int columnCount, ColumnArrangementMode columnMode) {
			if (columnMode == ColumnArrangementMode.Ascending)
				return SplitEquallyAscending(totalValuesCount, columnCount);
			else
				return SplitEquallyDescending(totalValuesCount, columnCount);
		}
		internal static int[] SplitEquallyAscending(int totalValuesCount, int columnCount) {
			if (columnCount <= 0)
				return new int[0];
			if (columnCount == 1)
				return new int[1] { totalValuesCount };
			int[] result = new int[columnCount];
			int valuesPerColumn = totalValuesCount / columnCount;
			int remainder = totalValuesCount % columnCount;
			for (int i = columnCount - 1; i >= 0; i--, remainder--) {
				int values = valuesPerColumn + ((remainder > 0) ? 1 : 0);
				result[i] = values;
			}
			return result;
		}
		internal static int[] SplitEquallyDescending(int totalValuesCount, int columnCount) {
			if (columnCount <= 0)
				return new int[0];
			if (columnCount == 1)
				return new int[1] { totalValuesCount };
			int[] result = new int[columnCount];
			int valuesPerColumn = totalValuesCount / columnCount;
			int remainder = totalValuesCount % columnCount;
			for (int i = 0; i < columnCount; i++, remainder--) {
				int values = valuesPerColumn + ((remainder > 0) ? 1 : 0);
				result[i] = values;
			}
			return result;
		}
		public static int CalculateTotalValues(int[] values, int startIndex, int limitIndex) {
			int count = values.Length;
			if (startIndex < 0 || startIndex >= count)
				return 0;
			if (limitIndex < 0 || limitIndex >= count)
				return 0;
			int result = 0;
			int i = startIndex;
			while (true) {
				if (i >= limitIndex)
					break;
				result += values[i];
				i++;
			}
			return result;
		}
	}
	#region ReportViewControlComparer
	public class ReportViewControlComparer : IComparer<XRControl>, IComparer<ReportViewControlBase> {
		#region IComparer<XRControl> Members
		public int Compare(XRControl x, XRControl y) {
			return CompareCore(x as ReportRelatedControlBase, y as ReportViewControlBase);
		}
		#endregion
		#region IComparer<ReportViewControlBase> Members
		public int Compare(ReportViewControlBase x, ReportViewControlBase y) {
			return CompareCore(x as ReportRelatedControlBase, y);
		}
		#endregion
		int CompareCore(ReportRelatedControlBase slave, ReportViewControlBase master) {
			if (slave != null && master != null) {
				if (slave.LayoutOptionsHorizontal.MasterControl == master || slave.LayoutOptionsVertical.MasterControl == master)
					return 1;
			}
			return 0;
		}
	}
	#endregion
}
