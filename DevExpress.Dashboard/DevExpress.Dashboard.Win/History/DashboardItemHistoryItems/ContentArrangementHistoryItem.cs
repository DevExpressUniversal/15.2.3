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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ContentArrangementHistoryItem : DashboardItemHistoryItem<DashboardItem> {
		readonly IElementContainer elementContainer;
		readonly ContentArrangementMode arrangementModePrev;
		readonly ContentArrangementMode arrangementModeNext;
		readonly int lineCountPrev;
		readonly int lineCountNext;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemContentArrangement; } }
		public ContentArrangementHistoryItem(DashboardItem dashboardItem, ContentArrangementMode arrangementMode, int lineCount) : base(dashboardItem) {
			elementContainer = dashboardItem.ElementContainer;
			arrangementModePrev = elementContainer.ContentArrangementMode;
			arrangementModeNext = arrangementMode;
			lineCountPrev = elementContainer.ContentLineCount;
			lineCountNext = lineCount;
		}
		protected override void PerformUndo() {
			if (elementContainer != null) {
				elementContainer.ContentArrangementMode = arrangementModePrev;
				elementContainer.ContentLineCount = lineCountPrev;
			}
		}
		protected override void PerformRedo() {
			if (elementContainer != null) {
				elementContainer.ContentArrangementMode = arrangementModeNext;
				elementContainer.ContentLineCount = lineCountNext;
			}
		}
	}
}
