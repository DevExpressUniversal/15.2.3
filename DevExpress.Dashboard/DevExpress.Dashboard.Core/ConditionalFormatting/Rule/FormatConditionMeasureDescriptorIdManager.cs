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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	static class FormatConditionMeasureDescriptorIdManager {
		const string MeasureStringFormat = "{0}_{1}";
		const string AxisStringFormat = "{0}_{1}_{2}";
		const string FormatConditionPrefix = "FormatConditionMeasure";
		const string NormalizedValuePrefix = "NormalizedValueMeasure";
		const string ZeroPositionPrefix = "ZeroPositionMeasure";
		const string Axis1Prefix = "Axis1";
		const string Axis2Prefix = "Axis2";
		public static string GetFormatConditionMeasureDescriptorId(string name) {
			return String.Format(MeasureStringFormat, FormatConditionPrefix, name);
		}
		public static string GetNormalizedValueMeasureDescriptorId(IFormatRuleLevel level) {
			return GetMeasureDescriptor(level, String.Format(MeasureStringFormat, NormalizedValuePrefix, level.ItemApplyTo.ActualId));
		}
		public static string GetZeroPositionMeasureDescriptorId(IFormatRuleLevel level) {
			return GetMeasureDescriptor(level, String.Format(MeasureStringFormat, ZeroPositionPrefix, level.ItemApplyTo.ActualId));
		}
		static string GetMeasureDescriptor(IFormatRuleLevel level, string measureId) {
			if(level.Axis1Item != null)
				measureId = String.Format(AxisStringFormat, measureId, Axis1Prefix, level.Axis1Item.ActualId);
			if(level.Axis2Item != null)
				measureId = String.Format(AxisStringFormat, measureId, Axis2Prefix, level.Axis2Item.ActualId);
			return measureId;
		}
	}
}
