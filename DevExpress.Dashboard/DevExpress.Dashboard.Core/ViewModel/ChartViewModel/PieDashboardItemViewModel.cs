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

using System.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewModel {
	public class PieDashboardItemViewModel : ChartDashboardItemBaseViewModel {
		public PieType PieType { get; set; }
		public PieLegendPosition LegendPosition { get; set; }
		public PieValueType LabelContentType { get; set; }
		public PieValueType TooltipContentType { get; set; }
		public bool ShowPieCaptions { get; set; }
		public bool ProvideValuesAsArguments { get; set; }
		public string[] ValueDataMembers { get; set; }
		public string[] ValueDisplayNames { get; set; }
		public string[] ColorDataMembers { get; set; }
		public NumericFormatViewModel PercentFormatViewModel { get; set; }
		public PieDashboardItemViewModel() : base(){
		}
		public PieDashboardItemViewModel(PieDashboardItem dashboardItem, PieInternal pie)
			: base(dashboardItem) {
			LabelContentType = dashboardItem.LabelContentType;
			TooltipContentType = dashboardItem.TooltipContentType;
			PieType = dashboardItem.PieType;
			LegendPosition = dashboardItem.LegendPosition;
			ShowPieCaptions = dashboardItem.ShowPieCaptions;
			ProvideValuesAsArguments = dashboardItem.ProvideValuesAsArguments;
			ValueDataMembers = pie.ValueDataMembers.ToArray();
			ValueDisplayNames = pie.ValueDisplayNames.ToArray();
			ColorDataMembers = dashboardItem.ColorMeasuresByHue ?
				dashboardItem.Values.Select(measure => ChartDashboardItemBase.CorrectColorMeasureId(DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionString(measure.GetMeasureDefinition()))).ToArray() :
				new string[] { ChartDashboardItemBase.ColorMeasure };
			PercentFormatViewModel = new NumericFormatViewModel(NumericFormatType.Percent, 2, DataItemNumericUnit.Ones, false, false, 0, null);
		}
	}
}
