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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.Data {
	public class SparklineTooltipValues {
		public string Start { get; set; }
		public string End { get; set; }
		public string Min { get; set; }
		public string Max { get; set; }
	}
	public class CardData: KpiElementData {
		readonly decimal subValue1;
		readonly decimal subValue2;
		readonly string subValue1Text;
		readonly string subValue2Text;
		readonly List<double> sparklineValues;
		public decimal SubValue1 { get { return subValue1; } }
		public decimal SubValue2 { get { return subValue2; } }
		public string SubValue1Text { get { return subValue1Text; } }
		public string SubValue2Text { get { return subValue2Text; } }
		public List<double> SparklineValues { get { return sparklineValues; } }
		public SparklineTooltipValues SparklineTooltipValues { get; private set; }
		public CardData(string title, SelectionData selectionData, DeltaValues deltaValues, List<double> sparklineValues)
			: base(title, selectionData, deltaValues) {
			subValue1 = deltaValues.SubValue1;
			subValue2 = deltaValues.SubValue2;
			subValue1Text = deltaValues.SubValue1Text;
			subValue2Text = deltaValues.SubValue2Text;
			this.sparklineValues = sparklineValues;
		}
		public CardData(string title, SelectionData selectionData, DeltaValues deltaValues, List<double> sparklineValues, SparklineTooltipValues sparklineTooltipValues)
			: this(title, selectionData, deltaValues, sparklineValues) {
				this.SparklineTooltipValues = sparklineTooltipValues;
		}
	}
}
