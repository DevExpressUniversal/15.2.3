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

using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
namespace DevExpress.DashboardCommon {
	public class FormatConditionAverage : FormatConditionStyleBase {
		const string XmlAverageType = "AverageType";
		const DashboardFormatConditionAboveBelowType DefaultAverageType = DashboardFormatConditionAboveBelowType.Above;
		DashboardFormatConditionAboveBelowType averageType = DefaultAverageType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionAverageAverageType"),
#endif
		DefaultValue(DefaultAverageType)
		]
		public DashboardFormatConditionAboveBelowType AverageType {
			get { return averageType; }
			set {
				if(AverageType != value) {
					averageType = value;
					OnChanged();
				}
			}
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionAverage();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlAverageType, AverageType, DefaultAverageType);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.LoadEnum<DashboardFormatConditionAboveBelowType>(element, XmlAverageType, x => averageType = x);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionAverage;
			if(source != null) {
				AverageType = source.AverageType;
			}
		}
		protected override IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			return new SummaryItemTypeEx[]{ SummaryItemTypeEx.Average };
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			object val = valueProvider.GetValue(this);
			if(!CheckValue(val)) return false;
			object numValue = ValueManager.ConvertToNumber(val);
			if(numValue == null) return false;
			object average = ValueManager.ConvertToNumber(GetAggregationValue());
			if(average != null) {
				int res = ValueManager.CompareValues(numValue, average, true);
				switch(AverageType) {
					case DashboardFormatConditionAboveBelowType.Above:
						return res > 0;
					case DashboardFormatConditionAboveBelowType.Below:
						return res < 0;
					case DashboardFormatConditionAboveBelowType.AboveOrEqual:
						return res >= 0;
					case DashboardFormatConditionAboveBelowType.BelowOrEqual:
						return res <= 0;
				}
			}
			return false; 
		}
	}
}
