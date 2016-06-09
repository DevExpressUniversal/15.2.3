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
using System.Text;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public static class SeriesIncompatibilityHelper {
		const string delimiter = ",\n";
		const SeriesIncompatibilityType notDisplayedIncompatibilityType = SeriesIncompatibilityType.Invisible;
		static string SeriesIncompatibilityTypeToString(SeriesIncompatibilityType type) {
			switch (type) {
				case SeriesIncompatibilityType.ByViewType:
					return ChartLocalizer.GetString(ChartStringId.MsgIncompatibleByViewType);
				case SeriesIncompatibilityType.ByArgumentScaleType:
					return ChartLocalizer.GetString(ChartStringId.MsgIncompatibleByArgumentScaleType);
				case SeriesIncompatibilityType.ByValueScaleType:
					return ChartLocalizer.GetString(ChartStringId.MsgIncompatibleByValueScaleType);
				default:
					ChartDebug.Fail("Incorrect series incompatibility type");
					return String.Empty;
			}
		}
		static string ConstructMessage(SeriesIncompatibilityType type, ISeries series) {
			return String.Format(ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesMessage),
				SeriesIncompatibilityTypeToString(type),
				series == null ? ChartLocalizer.GetString(ChartStringId.AutocreatedSeriesName) : series.Name);
		}
		public static string ConstructMessage(SeriesIncompatibilityInfo incompatibilityInfo) {
			StringBuilder builder = new StringBuilder();
			foreach (KeyValuePair<SeriesIncompatibilityType, ISeries> pair in incompatibilityInfo) {
				if (notDisplayedIncompatibilityType == pair.Key)
					return null;
				string currentMessage = ConstructMessage(pair.Key, pair.Value);
				if (!String.IsNullOrEmpty(currentMessage)) {
					if (builder.Length != 0)
						builder.AppendLine(delimiter);
					builder.Append(currentMessage);
				}
			}
			if (builder.Length > 0)
				builder.Append(".");
			return builder.ToString();
		}
	}
}
