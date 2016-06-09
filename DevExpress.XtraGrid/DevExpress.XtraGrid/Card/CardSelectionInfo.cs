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
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Views.Card.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Views.Card.ViewInfo {
	public class CardSelectionInfo : BaseSelectionInfo { 
		public CardSelectionInfo(CardView gv) : base(gv) {
		}
		protected override int NoneHitTest { get { return (int)CardHitTest.None; } }
		protected override int DefaultState { get { return (int)CardState.Normal; } }
		public new CardView View { get { return base.View as CardView; } }
		public new CardHitInfo PressedInfo { 
			get { 
				CardHitInfo res = base.PressedInfo as CardHitInfo;
				if(res == null) res = CardHitInfo.EmptyInfo;
				return res;
			}
		}
		public new CardHitInfo HotTrackedInfo { 
			get { 
				CardHitInfo res = base.HotTrackedInfo as CardHitInfo; 
				if(res == null) res = CardHitInfo.EmptyInfo;
				return res;
			} 
		}
		protected override int GetRowHandle(BaseHitInfo hitInfo) {
			if(hitInfo == null) return base.GetRowHandle(hitInfo);
			return ((CardHitInfo)hitInfo).RowHandle;
		}
		public virtual bool IsHotTrack(CardHitTest test) { return IsHotTrack((int)test); }
		protected override void CreateStates() {
			fValidHotTracks = new int[fCvalidHotTracks.Length];
			Array.Copy(fCvalidHotTracks, fValidHotTracks, fCvalidHotTracks.Length);
			fValidPressedHitTests = new int[fCvalidPressedHitTests.Length];
			Array.Copy(fCvalidPressedHitTests, fValidPressedHitTests, fCvalidPressedHitTests.Length);
			fViewPressedStates = new int[fCcardStates.Length];
			Array.Copy(fCcardStates, fViewPressedStates, fCcardStates.Length);
		}
		static CardHitTest[] fCvalidHotTracks = {CardHitTest.FilterPanelCloseButton, 
													CardHitTest.FilterPanelActiveButton, 
													CardHitTest.FilterPanelText, 
													CardHitTest.FilterPanelMRUButton, 
													CardHitTest.FilterPanelCustomizeButton,
													CardHitTest.CloseZoomButton, CardHitTest.QuickCustomizeButton,  
													CardHitTest.CardExpandButton, CardHitTest.CardCaptionErrorIcon, CardHitTest.CardDownButton, CardHitTest.CardUpButton, CardHitTest.FieldValue};
		static CardHitTest[] fCvalidPressedHitTests = {CardHitTest.FilterPanelCloseButton, CardHitTest.FilterPanelActiveButton, 
														  CardHitTest.FilterPanelText, 
														  CardHitTest.FilterPanelMRUButton,		
														  CardHitTest.FilterPanelCustomizeButton,
														  CardHitTest.CloseZoomButton, 
														  CardHitTest.QuickCustomizeButton, 
														  CardHitTest.CardExpandButton, CardHitTest.CardDownButton, CardHitTest.CardUpButton};
		static CardState[] fCcardStates = { CardState.FilterPanelCloseButtonPressed, CardState.FilterPanelActiveButtonPressed,
											  CardState.FilterPanelTextPressed, 
											  CardState.FilterPanelMRUButtonPressed, 
											  CardState.FilterPanelCustomizeButtonPressed,
											  CardState.CloseZoomButtonPressed, 
											  CardState.QuickCustomizeButtonPressed, 
											  CardState.CardExpandButtonPressed, CardState.CardDownButtonPressed, 
											  CardState.CardUpButtonPressed};
		protected override void DoSetState(int newState) {
			View.SetState((CardState)newState);
		}
		protected override int GetState() { return (int)View.State; }
		protected override bool IsHotEquals(BaseHitInfo h1, BaseHitInfo h2) {
			CardHitInfo gh1 = h1 as CardHitInfo, gh2 = h2 as CardHitInfo;
			if(gh1.HitTest == gh2.HitTest && gh1.Column == gh2.Column && gh1.RowHandle == gh2.RowHandle) {
				return true;
			}
			return false;
		}
		protected override bool IsPressedEquals(BaseHitInfo h1, BaseHitInfo h2) {
			CardHitInfo gh1 = h1 as CardHitInfo, gh2 = h2 as CardHitInfo;
			if(gh2.HitTest == gh1.HitTest && gh1.RowHandle == gh2.RowHandle && gh1.Column == gh2.Column) return true;
			return false;
		}
	}
}
