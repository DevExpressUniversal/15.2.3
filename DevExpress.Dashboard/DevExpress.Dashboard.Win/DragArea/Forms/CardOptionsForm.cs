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
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	public partial class CardOptionsForm : DashboardForm {
		readonly DashboardDesigner designer;
		readonly Card card;
		readonly CardDashboardItem dashboardItem;
		public CardOptionsForm() {
			InitializeComponent();
		}
		public CardOptionsForm(DashboardDesigner designer, CardDashboardItem dashboardItem, Card card)
			: this() {
			this.designer = designer;
			this.card = card;
			this.dashboardItem = dashboardItem;
			deltaOptionsControl.PrepareOptions(card.DeltaOptions);
			deltaOptionsControl.Enabled = card.ActualValue != null && card.TargetValue != null;
			sparklineOptionsControl.Enabled = ceShowSparkline.Enabled = card.SparklineValue != null;
			ceShowSparkline.Checked = card.ShowSparkline;
			sparklineOptionsControl.PrepareOptions(card.SparklineOptions);
		}
		void ShowSparklineCheckedChanged(object sender, EventArgs e) {
			sparklineOptionsControl.Enabled = ceShowSparkline.Checked;
		}
		void ButtonOKClick(object sender, EventArgs e) {
			CardOptionsHistoryItem historyItem = new CardOptionsHistoryItem(dashboardItem, card, ceShowSparkline.Checked, deltaOptionsControl.DeltaOptions, sparklineOptionsControl.SparklineOptions);
			historyItem.Redo(designer);
			designer.History.Add(historyItem);
		}
	}
}
