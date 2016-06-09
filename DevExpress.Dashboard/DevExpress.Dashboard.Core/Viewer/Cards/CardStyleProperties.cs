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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class CardStyleProperties : IDeltaColors {
		int MarginX { get { return DefaultCardMeasurements.DefaultMarginX; } }
		int MarginY { get { return DefaultCardMeasurements.DefaultMarginY; } }
		public Color ActualValueColor { get; set; }
		public Color SubTextColor { get; set; }
		public Color Neutral { get; set; }
		public Color Bad { get; set; }
		public Color Warning { get; set; }
		public Color Good { get; set; }
		public Size Margin { get; set; }
		public Size Proportions { get; set; }
		public float MainTitleHeight { get; set; }
		public float SubTitleHeight { get; set; }
		public float SubValue1Height { get; set; }
		public float SubValue2Height { get; set; }
		public float MainValueHeight { get; set; }
		public CardStyleProperties() {
			ActualValueColor = DefaultDeltaColors.DefaultActualValueColor;
			SubTextColor = SystemColors.GrayText;
			Neutral = DefaultDeltaColors.DefaultDeltaNeutralResultColor;
			Bad = DefaultDeltaColors.DefaultDeltaBadResultColor;
			Good = DefaultDeltaColors.DefaultDeltaGoodResultColor;
			Warning = DefaultDeltaColors.DefaultDeltaWarningResultColor;
			Margin = new Size(MarginX, MarginY);
			Proportions = DefaultCardMeasurements.DefaultDashboardCardProportions;
			MainTitleHeight = 0.3F;
			SubTitleHeight = 0.21F;
			SubValue1Height = 0.21F;
			SubValue2Height = 0.21F;
			MainValueHeight = 0.45F;
		}
		public void UpdateCardProportions(CardDashboardItemViewModel viewModel) {
			if(viewModel != null && viewModel.Cards.Count > 0) {
				CardViewModel card = viewModel.Cards[0];
				bool isSparklineMode = viewModel.HasSeriesDimensions ? viewModel.IsSparklineMode && card.ShowSparkline : viewModel.IsSparklineMode && viewModel.HasAtLeastOneSparkline;
				if(isSparklineMode) {
					if(Proportions == DefaultCardMeasurements.DefaultDashboardCardProportions)
						Proportions = DefaultCardMeasurements.DefaultDashboardCardSparklineProportions;
				}
				else {
					Proportions = DefaultCardMeasurements.DefaultDashboardCardProportions;
				}
			}
		}
		public Color GetActualValueColor(AppearanceObject appearanceObject, ObjectState objectState) {
			return ActualValueColor;
		}
	}
	public static class DefaultDeltaColors {
		public readonly static Color DefaultDeltaNeutralResultColor = SystemColors.GrayText;
		public readonly static Color DefaultDeltaBadResultColor = Color.FromArgb(214, 5, 5);
		public readonly static Color DefaultDeltaGoodResultColor = Color.FromArgb(63, 136, 48);
		public readonly static Color DefaultDeltaWarningResultColor = Color.FromArgb(228, 124, 2);
		public readonly static Color DefaultActualValueColor = SystemColors.WindowText;
		public readonly static Color DefaultBarColor = Color.FromArgb(27, 73, 165);
	}
	public static class DefaultCardMeasurements {
		public readonly static int DefaultMarginX = 3;
		public readonly static int DefaultMarginY = 3;
		public readonly static int DefaultDashboardCardSparklineTopIndent = 6;
		public readonly static Padding DefaultContentPadding = new Padding(16, 6, 16, 16);
		public readonly static Padding DefaultDashboardItemMargin = new Padding(1, 1, 1, 1);
		public readonly static Size DefaultDashboardCardProportions = new Size(2, 1);
		public readonly static Size DefaultDashboardCardSparklineProportions = new Size(200, 125);
	}
}
