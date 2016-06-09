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

namespace DevExpress.DashboardCommon.ViewModel {
	public class CardViewModel : KpiElementViewModel {
		public bool IgnoreSubValue1DeltaColor { get; set; }
		public bool IgnoreSubValue2DeltaColor { get; set; }
		public SparklineOptionsViewModel SparklineOptions { get; set; }
		public bool ShowSparkline { get; set; }
		public CardViewModel()
			: base() {
		}
		public CardViewModel(Card card)
			: base(card) {
			DeltaOptions deltaOptions = card.DeltaOptions;
			DeltaValueType valueType = deltaOptions.SubValue1DisplayType;
			IgnoreSubValue1DeltaColor = valueType == DeltaValueType.ActualValue;
			valueType = deltaOptions.SubValue2DisplayType;
			IgnoreSubValue2DeltaColor = valueType == DeltaValueType.ActualValue;
			SparklineOptions = new SparklineOptionsViewModel(card.SparklineOptions);
			ShowSparkline = card.ShowSparkline;
		}
	}
}
