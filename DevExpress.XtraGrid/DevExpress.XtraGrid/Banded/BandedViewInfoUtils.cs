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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid.Drawing;
using DevExpress.XtraGrid.Dragging;
namespace DevExpress.XtraGrid.Views.BandedGrid.ViewInfo {
	public enum BandedGridHitTest { None, Column, ColumnEdge, ColumnButton, ColumnFilterButton, ColumnPanel,
		RowCell, RowIndicator, RowGroupButton, RowGroupCheckSelector, Row, RowPreview, RowDetail, RowDetailEdge, 
		RowDetailIndicator, EmptyRow, GroupPanel, GroupPanelColumn, GroupPanelColumnFilterButton, Footer, CellButton,
		CustomizationForm, FilterPanel, FilterCloseButton, RowFooter, RowEdge,
		FixedLeftDiv, FixedRightDiv, VScrollBar, HScrollBar, FilterPanelActiveButton,
		FilterPanelText, FilterPanelMRUButton, FilterPanelCustomizeButton, ViewCaption, MasterTabPageHeader, GroupRowCell,
		Band = 100, BandEdge = 101, BandPanel = 102, BandButton = 103
	};
	public class BandedGridHitInfo : GridHitInfo {
		static BandedGridHitInfo empty = null;
		internal static BandedGridHitInfo EmptyBanded {
			get { 
				if(empty == null) empty = new BandedGridHitInfo();
				return empty;
			}
		}
		const int BandedGridHitTestStart = 100;
		BandedGridHitTest _bandedHitTest;
		GridBand band;
		public BandedGridHitInfo() {
		}
		public override void Clear() {
			this.band = null;
			this._bandedHitTest = BandedGridHitTest.None;
			base.Clear();
		}
		public GridBand Band { get { return band; } set { band = value; } }
		public new BandedGridColumn Column { get { return base.Column as BandedGridColumn; } set { base.Column = value; } }
		public virtual bool InBandPanel {
			get {
				return IsValid && (HitTest == BandedGridHitTest.Band || 
					HitTest == BandedGridHitTest.BandEdge || 
					HitTest == BandedGridHitTest.BandButton || 
					HitTest == BandedGridHitTest.BandPanel);
			}
		}
		protected virtual GridHitTest BandedHitToGrid(BandedGridHitTest hit) {
			int v = (int)hit;
			if(v < BandedGridHitTestStart) return (GridHitTest)v;
			return GridHitTest.None;
		}
		protected internal override int HitTestInt { get { return (int)_bandedHitTest; } }
		public new BandedGridHitTest HitTest {
			get { return _bandedHitTest; }
			set {
				SetHitTest((int)value);
			}
		}
		protected override void SetHitTest(int val) {
			base.SetHitTest((int)BandedHitToGrid((BandedGridHitTest)val));
			this._bandedHitTest = (BandedGridHitTest)val;
		}
	}
}
