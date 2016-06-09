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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.XtraGrid.Views.BandedGrid.ViewInfo {
	public class BandedGridSelectionInfo : GridSelectionInfo { 
		public BandedGridSelectionInfo(BandedGridView view) : base(view) {
		}
		protected override void CreateStates() {
			fValidHotTracks = new int[fBvalidHotTracks.Length];
			Array.Copy(fBvalidHotTracks, fValidHotTracks, fBvalidHotTracks.Length);
			fValidPressedHitTests = new int[fBvalidPressedHitTests.Length];
			Array.Copy(fBvalidPressedHitTests, fValidPressedHitTests, fBvalidPressedHitTests.Length);
			fViewPressedStates = new int[fBBandedGridStates.Length];
			Array.Copy(fBBandedGridStates, fViewPressedStates, fBBandedGridStates.Length);
		}
		static BandedGridHitTest[] fBvalidHotTracks = 
				{	BandedGridHitTest.ColumnFilterButton, 
					BandedGridHitTest.FilterPanelActiveButton,
					BandedGridHitTest.FilterPanelText, 
					BandedGridHitTest.FilterPanelMRUButton, 
					BandedGridHitTest.FilterPanelCustomizeButton,
					BandedGridHitTest.RowIndicator, 
					BandedGridHitTest.FilterCloseButton, BandedGridHitTest.GroupPanelColumnFilterButton,
					BandedGridHitTest.RowCell, BandedGridHitTest.CellButton, BandedGridHitTest.Column, BandedGridHitTest.GroupPanelColumn,
					BandedGridHitTest.Footer, BandedGridHitTest.RowFooter, BandedGridHitTest.RowEdge,
					BandedGridHitTest.Band
				};
		static BandedGridHitTest[] fBvalidPressedHitTests = 
				{	BandedGridHitTest.Column, 
					BandedGridHitTest.FilterPanelActiveButton,
					BandedGridHitTest.FilterPanelText, 
					BandedGridHitTest.FilterPanelMRUButton, 
					BandedGridHitTest.FilterPanelCustomizeButton,
					BandedGridHitTest.ColumnFilterButton,	BandedGridHitTest.FilterCloseButton, 
					BandedGridHitTest.ColumnButton, BandedGridHitTest.GroupPanelColumn, 
					BandedGridHitTest.GroupPanelColumnFilterButton,
					BandedGridHitTest.Band
				};
		static BandedGridState[] fBBandedGridStates = 
				{ BandedGridState.ColumnDown, 
					BandedGridState.FilterPanelActiveButtonPressed,
					BandedGridState.FilterPanelTextPressed, 
					BandedGridState.FilterPanelMRUButtonPressed, 
					BandedGridState.FilterPanelCustomizeButtonPressed,
					BandedGridState.ColumnFilterDown, BandedGridState.FilterCloseButtonPressed,
					BandedGridState.ColumnButtonDown, BandedGridState.ColumnDown,
					BandedGridState.ColumnFilterDown,
					BandedGridState.BandDown
				};
		public GridBand PressedBand { 
			get { 
				return IsPressed((int)BandedGridHitTest.Band) ? PressedInfo.Band : null; 
			}
		}
		public GridBand HotTrackedBand {
			get {
				if(HotTrackedInfo.Band == null) return null;
				if(HotTrackedInfo.HitTest == BandedGridHitTest.Band && HotTrackedInfo.InBandPanel) return HotTrackedInfo.Band;
				return null;
			}
		}
		public new BandedGridHitInfo PressedInfo { 
			get { 
				BandedGridHitInfo res = base.PressedInfo as BandedGridHitInfo; 
				if(res == null) res = BandedGridHitInfo.EmptyBanded;
				return res;
			} 
		}
		public new BandedGridHitInfo HotTrackedInfo { 
			get { 
				BandedGridHitInfo res = base.HotTrackedInfo as BandedGridHitInfo; 
				if(res == null) res = BandedGridHitInfo.EmptyBanded;
				return res;
			} 
		}
		protected override bool CanMousePressObject(BaseHitInfo hitInfo) { 
			if(!base.CanMousePressObject(hitInfo)) return false;
			BandedGridHitInfo bi = hitInfo as BandedGridHitInfo;
			if(bi == null) return true;
			if(bi.Band != null && bi.HitTest == BandedGridHitTest.Band) {
				return (View.GridControl.IsDesignMode || bi.Band.OptionsBand.AllowPress);
			}
			return true; 
		}
		protected override bool IsHotEquals(BaseHitInfo h1, BaseHitInfo h2) {
			BandedGridHitInfo gh1 = h1 as BandedGridHitInfo, gh2 = h2 as BandedGridHitInfo;
			if(gh1.InBandPanel || gh2.InBandPanel) {
				if(gh1.InBandPanel == gh2.InBandPanel) return (gh1.Band == gh2.Band && gh1.HitTest == gh2.HitTest);
				return false;
			}
			return base.IsHotEquals(h1, h2);
		}
		protected override bool IsPressedEquals(BaseHitInfo h1, BaseHitInfo h2) {
			BandedGridHitInfo gh1 = h1 as BandedGridHitInfo, gh2 = h2 as BandedGridHitInfo;
			if(gh1.InBandPanel && gh2.InBandPanel) {
				return (gh1.Band == gh2.Band && gh1.HitTest == gh2.HitTest);
			}
			return base.IsPressedEquals(h1, h2);
		}
	}
}
