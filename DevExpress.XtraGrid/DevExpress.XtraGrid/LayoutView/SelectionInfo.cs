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
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
namespace DevExpress.XtraGrid.Views.Layout.ViewInfo {
	public class LayoutViewSelectionInfo : BaseSelectionInfo {
		public LayoutViewSelectionInfo(LayoutView gv) : base(gv) { }
		protected override int NoneHitTest { get { return (int)LayoutViewHitTest.None; } }
		protected override int DefaultState { get { return (int)LayoutViewState.Normal; } }
		public new LayoutView View { get { return base.View as LayoutView; } }
		public new LayoutViewHitInfo PressedInfo {
			get {
				LayoutViewHitInfo res = base.PressedInfo as LayoutViewHitInfo;
				if(res == null) res = LayoutViewHitInfo.EmptyInfo;
				return res;
			}
		}
		protected override bool CanMousePressObject(BaseHitInfo hitInfo) {
			LayoutViewHitInfo hi = hitInfo as LayoutViewHitInfo;
			if(hi.InCardsArea) {
				return !View.PanModeActive && !View.IsCustomizationMode;
			}
			return base.CanMousePressObject(hitInfo);
		}
		public new LayoutViewHitInfo HotTrackedInfo {
			get {
				LayoutViewHitInfo res = base.HotTrackedInfo as LayoutViewHitInfo;
				if(res == null) res = LayoutViewHitInfo.EmptyInfo;
				return res;
			}
		}
		protected override int GetRowHandle(BaseHitInfo hitInfo) {
			if(hitInfo == null) return base.GetRowHandle(hitInfo);
			return ((LayoutViewHitInfo)hitInfo).RowHandle;
		}
		public virtual bool IsHotTrack(LayoutViewHitTest test) {
			return IsHotTrack((int)test);
		}
		protected override void CreateStates() {
			fValidHotTracks = new int[fCvalidHotTracks.Length];
			fValidPressedHitTests = new int[fCvalidPressedHitTests.Length];
			fViewPressedStates = new int[fCLayoutViewStates.Length];
			Array.Copy(fCvalidPressedHitTests, fValidPressedHitTests, fCvalidPressedHitTests.Length);
			Array.Copy(fCvalidHotTracks, fValidHotTracks, fCvalidHotTracks.Length);
			Array.Copy(fCLayoutViewStates, fViewPressedStates, fCLayoutViewStates.Length);
		}
		static LayoutViewHitTest[] fCvalidHotTracks = { LayoutViewHitTest.SingleModeButton,
														LayoutViewHitTest.RowModeButton,
														LayoutViewHitTest.ColumnModeButton,
														LayoutViewHitTest.MultiRowModeButton,
														LayoutViewHitTest.MultiColumnModeButton,
														LayoutViewHitTest.CarouselModeButton,
														LayoutViewHitTest.PanButton,
														LayoutViewHitTest.CustomizeButton,
														LayoutViewHitTest.Field,
														LayoutViewHitTest.FieldCaption,
														LayoutViewHitTest.FieldValue,
														LayoutViewHitTest.LayoutItem,
														LayoutViewHitTest.FieldPopupActionArea,
														LayoutViewHitTest.FieldSortButton,
														LayoutViewHitTest.FieldFilterButton,
														LayoutViewHitTest.CardExpandButton,
														LayoutViewHitTest.FilterPanelCloseButton, 
														LayoutViewHitTest.FilterPanelActiveButton, 
														LayoutViewHitTest.FilterPanelText, 
														LayoutViewHitTest.FilterPanelMRUButton, 
														LayoutViewHitTest.FilterPanelCustomizeButton,
														LayoutViewHitTest.CloseZoomButton};
		static LayoutViewHitTest[] fCvalidPressedHitTests = {
														LayoutViewHitTest.SingleModeButton,
														LayoutViewHitTest.RowModeButton,
														LayoutViewHitTest.ColumnModeButton,
														LayoutViewHitTest.MultiRowModeButton,
														LayoutViewHitTest.MultiColumnModeButton,
														LayoutViewHitTest.CarouselModeButton,
														LayoutViewHitTest.PanButton,
														LayoutViewHitTest.CustomizeButton,
														LayoutViewHitTest.LayoutItem,
														LayoutViewHitTest.FieldSortButton,
														LayoutViewHitTest.FieldFilterButton,
														LayoutViewHitTest.CardExpandButton,
														LayoutViewHitTest.FilterPanelCloseButton, 
														LayoutViewHitTest.FilterPanelActiveButton, 
														LayoutViewHitTest.FilterPanelText, 
														LayoutViewHitTest.FilterPanelMRUButton,		
														LayoutViewHitTest.FilterPanelCustomizeButton,
														LayoutViewHitTest.CloseZoomButton};
		static LayoutViewState[] fCLayoutViewStates = { 
														LayoutViewState.SingleCardModeButtonPressed,			
														LayoutViewState.RowModeButtonPressed,			
														LayoutViewState.ColumnModeButtonPressed,	
														LayoutViewState.MultiRowModeButtonPressed,
														LayoutViewState.MultiColumnModeButtonPressed,
														LayoutViewState.CarouselModeButtonPressed,
														LayoutViewState.PanButtonPressed,	
														LayoutViewState.CustomizeButtonPressed,
														LayoutViewState.LayoutItemPressed,
														LayoutViewState.FieldPopupActionSortPressed,
														LayoutViewState.FieldPopupActionFilterPressed,
														LayoutViewState.CardExpandButtonPressed,
														LayoutViewState.FilterPanelCloseButtonPressed, 
														LayoutViewState.FilterPanelActiveButtonPressed,
														LayoutViewState.FilterPanelTextPressed, 
														LayoutViewState.FilterPanelMRUButtonPressed, 
														LayoutViewState.FilterPanelCustomizeButtonPressed,
														LayoutViewState.CloseZoomButtonPressed};
		protected override void DoSetState(int newState) {
			View.SetState((LayoutViewState)newState);
		}
		protected override int GetState() { 
			return (int)View.State; 
		}
		protected override bool IsHotEquals(BaseHitInfo h1, BaseHitInfo h2) {
			return CheckEquals(h1 as LayoutViewHitInfo, h2 as LayoutViewHitInfo);
		}
		protected override bool IsPressedEquals(BaseHitInfo h1, BaseHitInfo h2) {
			return CheckEquals(h1 as LayoutViewHitInfo, h2 as LayoutViewHitInfo);
		}
		protected virtual bool CheckEquals(LayoutViewHitInfo gh1, LayoutViewHitInfo gh2) {
			return (gh2.HitTest == gh1.HitTest && gh1.RowHandle == gh2.RowHandle && gh1.Column == gh2.Column && GetTab(gh1) == GetTab(gh2));
		}
		protected virtual int GetTab(LayoutViewHitInfo hi) {
			if(hi.LayoutItemHitInfo != null && hi.LayoutItemHitInfo.IsTabbedGroup) 
				return hi.LayoutItemHitInfo.TabPageIndex;
			return -1;
		}
	}
}
