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
using System.Windows.Controls;
namespace DevExpress.Xpf.Grid.Native {
	internal class CardViewRowNavigation : GridViewRowNavigationBase {
		protected CardView CardView { get { return (CardView)View; } }
		protected Orientation Orientation { get { return CardView.Orientation; } }
		public CardViewRowNavigation(CardView view) : base(view) { }
		public override void OnLeft(bool isCtrlPressed) {
			if(Orientation == Orientation.Vertical) {
				if(ShouldCollapseRow())
					CardView.CollapseFocusedRow();
				else
					CardView.MovePrevColumnCard();
			} else
				CardView.MovePrevRowCard();
		}
		public override void OnRight(bool isCtrlPressed) {
			if(Orientation == Orientation.Vertical) {
				if(ShouldExpandRow())
					CardView.ExpandFocusedRow();
				else
					CardView.MoveNextColumnCard();
			} else
				CardView.MoveNextRowCard();
		}
		public override void OnUp(bool isCtrlPressed) {
			if(Orientation == Orientation.Vertical)
				CardView.MovePrevRowCard();
			else {
				if(ShouldExpandRow())
					CardView.ExpandFocusedRow();
				else if(View.IsExpandableRowFocused())
					View.MoveNextRow();
				else
					CardView.MovePrevColumnCard();
			}
		}
		public override void OnDown() {
			if(Orientation == Orientation.Vertical)
				CardView.MoveNextRowCard();
			else {
				if(ShouldCollapseRow())
					CardView.CollapseFocusedRow();
				else if(View.IsExpandableRowFocused())
					CardView.MovePrevRow();
				else
					CardView.MoveNextColumnCard();
			}
		}
	}
}
