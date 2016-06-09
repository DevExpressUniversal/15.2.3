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

using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public class CardOptionsHistoryItem : KpiDeltaOptionsHistoryItem<Card> {
		readonly Card card;
		readonly bool initialShowSparkline;
		readonly bool currentShowSparkline;
		readonly SparklineOptions initialSparklineOptions;
		readonly SparklineOptions currentSparklineOptions;
		protected override string PropertyName { get { return "Cards"; } }
		public CardOptionsHistoryItem(CardDashboardItem cardDashboardItem, Card card, bool showSparkline, DeltaOptions deltaOptions, SparklineOptions sparklineOptions)
			: base(cardDashboardItem, card, deltaOptions) {
			this.card = card;
			initialShowSparkline = card.ShowSparkline;
			currentShowSparkline = showSparkline;
			initialSparklineOptions = card.SparklineOptions.Clone();
			currentSparklineOptions = sparklineOptions.Clone();
		}
		protected override void PerformRedo() {
			base.PerformRedo();
			card.ShowSparkline = currentShowSparkline;
			card.SparklineOptions.Assign(currentSparklineOptions);
		}
		protected override void PerformUndo() {
			base.PerformUndo();
			card.ShowSparkline = initialShowSparkline;
			card.SparklineOptions.Assign(initialSparklineOptions);
		}
	}
}
