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
using System.Text;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.Native {
	public static class DashboardSpecialValuesInternal {
		public static object ToSpecialUniqueValue(object value) {
			if (Helper.IsNullValue(value))
				return DashboardSpecialValues.NullValue;
			if (Helper.IsPivotGridOthersValue(value))
				return DashboardSpecialValues.OthersValue;
			if(Helper.IsPivotErrorValue(value))
				return DashboardSpecialValues.ErrorValue;
			return value;
		}
		public static string ToSpecialUniqueName(object value, string uniqueName) {
			if(Helper.IsNullValue(value))
				return DashboardSpecialValues.NullValue;
			if(Helper.IsPivotGridOthersValue(value))
				return DashboardSpecialValues.OthersValue;
			if(Helper.IsPivotErrorValue(value))
				return DashboardSpecialValues.ErrorValue;
			return uniqueName;
		}
		public static object ToSpecialValue(object value, object defaultValue) {
			if(Helper.IsNullValue(value))
				return null;
			if(Helper.IsPivotGridOthersValue(value))
				return DashboardSpecialValues.OthersValue;
			if(Helper.IsPivotErrorValue(value))
				return DashboardSpecialValues.ErrorValue;
			return defaultValue;
		}
		public static string ToSpecialDisplayText(object value, string defaultDisplayText) {
			if(Helper.IsNullValue(value))
				return DashboardLocalizer.GetString(DashboardStringId.DashboardNullValue);
			if(Helper.IsPivotGridOthersValue(value))
				return DashboardLocalizer.GetString(DashboardStringId.TopNOthersValue);
			if(Helper.IsPivotErrorValue(value))
				return DashboardLocalizer.GetString(DashboardStringId.DashboardErrorValue);
			return defaultDisplayText;
		}
		public static bool TryGetDisplayText(object value, out string displayText) {
			displayText = null;
			if(DashboardSpecialValues.IsNullValue(value) || DashboardSpecialValues.IsOlapNullValue(value)) {
				displayText = DashboardLocalizer.GetString(DashboardStringId.DashboardNullValue);
				return true;
			} else if(DashboardSpecialValues.IsOthersValue(value)) {
				displayText = DashboardLocalizer.GetString(DashboardStringId.TopNOthersValue);
				return true;
			} else if(DashboardSpecialValues.IsErrorValue(value)) {
				displayText = DashboardLocalizer.GetString(DashboardStringId.DashboardErrorValue);
				return true;
			} else 
				return false;
		}
		public static object FromSpecialValue(object value) {
			if (DashboardSpecialValues.IsNullValue(value))
				return null;
			if (DashboardSpecialValues.IsOthersValue(value))
				return PivotGridNativeDataSource.OthersValue;
			return value;
		}
		public static object GetFieldValueItemValue(PivotGridData pivotData, PivotFieldValueItem item) {
			if (item == null)
				return DashboardSpecialValues.NullValue;
			if (pivotData.IsOLAP)
				return item.DisplayText ?? DashboardSpecialValues.NullValue.ToString();
			else {
				object value = item.Value;
				return ToSpecialUniqueValue(value);
			}
		}
		public static bool IsDashboardSpecialValue(object value) {
			return DashboardSpecialValues.IsNullValue(value) || 
				DashboardSpecialValues.IsOlapNullValue(value) || 
				DashboardSpecialValues.IsOthersValue(value) ||
				DashboardSpecialValues.IsErrorValue(value);
		}
	}
	public class DashboardValueComparer : ValueComparer {
		public DashboardValueComparer()
			: base() {
		}
		public override int Compare(object x, object y) {
			if(DashboardSpecialValues.IsNullValue(x) || DashboardSpecialValues.IsOlapNullValue(x)) x = null;
			if(DashboardSpecialValues.IsNullValue(y) || DashboardSpecialValues.IsOlapNullValue(y)) y = null;
			if(DashboardSpecialValuesInternal.IsDashboardSpecialValue(x) || DashboardSpecialValuesInternal.IsDashboardSpecialValue(y)) {
				x = GetWeight(x);
				y = GetWeight(y);
			}
			return base.Compare(x, y);
		}
		int GetWeight(object value) {
			if(DashboardSpecialValues.IsErrorValue(value))
				return 1;
			else if(DashboardSpecialValues.IsOthersValue(value))
				return 2;
			else
				return 0;
		}
	}
}
