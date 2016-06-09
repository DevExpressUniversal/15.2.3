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

using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class ValueLevelItem {
		readonly ValueLevel valueLevel;
		readonly string text;
		public ValueLevel ValueLevel { get { return valueLevel; } }
		public ValueLevelItem(ValueLevel valueLevel) {
			this.valueLevel = valueLevel;
			switch (valueLevel) {
				case ValueLevel.Value:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelValue);
					break;
				case ValueLevel.Value_1:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelValue_1);
					break;
				case ValueLevel.Value_2:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelValue_2);
					break;
				case ValueLevel.Low:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelLow);
					break;
				case ValueLevel.High:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelHigh);
					break;
				case ValueLevel.Open:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelOpen);
					break;
				case ValueLevel.Close:
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelClose);
					break;
				default:
					ChartDebug.Fail("Unknown value level.");
					text = ChartLocalizer.GetString(ChartStringId.WizValueLevelValue);
					break;
			}
		}
		public override string ToString() {
			return text;
		}
		public override bool Equals(object obj) {
			if (!(obj is ValueLevelItem))
				return false;
			ValueLevelItem item = (ValueLevelItem)obj;
			return valueLevel == item.valueLevel;
		}
		public override int GetHashCode() {
			return valueLevel.GetHashCode();
		}
	}
}
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(ValueLevelTypeConterter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)
	]
	public enum ValueLevel { 
		Value = ValueLevelInternal.Value,
		Value_1 = ValueLevelInternal.Value_1,
		Value_2 = ValueLevelInternal.Value_2,
		Low = ValueLevelInternal.Low,
		High = ValueLevelInternal.High,
		Open = ValueLevelInternal.Open,
		Close = ValueLevelInternal.Close
	}
}
