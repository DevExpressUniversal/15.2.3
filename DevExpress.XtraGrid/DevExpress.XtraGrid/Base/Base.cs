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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraGrid.Scrolling {
	public class BaseViewOfficeScroller : OfficeScroller {
		BaseView view;
		public BaseViewOfficeScroller(BaseView view) {
			this.view = view;
		}
		protected BaseView View { get { return view; } }
		protected override void OnStopScroller() { View.SetDefaultState(); }
		protected override void OnStartScroller() { View.SetScrollingState(); }
		protected override bool AllowHScroll { get { return false; } }
		protected override bool AllowVScroll { get { return false; } }
	}
	public class CardViewOfficeScroller : BaseViewOfficeScroller {
		public CardViewOfficeScroller(CardView view) : base(view) { }
		protected new CardView View { get { return base.View as CardView; } }
		protected override void OnHScroll(int delta) { View.TopLeftCardIndex += delta; }
		protected override bool AllowVScroll { get { return false; } }
		protected override bool AllowHScroll { get { return true; } }
	}
	public class GridViewOfficeScroller : BaseViewOfficeScroller {
		public GridViewOfficeScroller(GridView view) : base(view) { }
		protected new GridView View { get { return base.View as GridView; } }
		protected override void OnHScroll(int delta) { View.LeftCoord += 4 * delta; }
		protected override void OnVScroll(int delta) { View.TopRowIndex += delta; }
		protected override bool AllowVScroll { get { return true; } }
		protected override bool AllowHScroll { get { return !View.OptionsView.ColumnAutoWidth; } }
	}
}
