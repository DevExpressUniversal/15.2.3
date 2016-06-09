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

using DevExpress.Utils.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotGridGestureScroller : OfficeScroller {
		PivotGridViewInfoBase view;
		Point offsetAccumulator = Point.Empty;
		public PivotGridGestureScroller(PivotGridViewInfoBase view) {
			this.view = view;
			LastLeftTopCoord = new Point(-1, -1);
		}
		protected PivotGridViewInfoBase View { get { return view; } }
		protected override void OnHScroll(int delta) { View.LeftTopCoord = new Point(View.LeftTopCoord.X + delta, View.LeftTopCoord.Y); }
		protected override void OnVScroll(int delta) { View.LeftTopCoord = new Point(View.LeftTopCoord.X, View.LeftTopCoord.Y + delta); }
		protected override bool AllowVScroll { get { return true; } }
		protected override bool AllowHScroll { get { return true; } }
		Point LeftTopCoord {
			get { return ViewInfo.LeftTopCoord; }
			set { ViewInfo.LeftTopCoord = value; }
		}
		Point LastLeftTopCoord { get; set; }
		int FirstVisibleCellWidth { get; set; }
		int FirstVisibleCellHeight { get; set; }
		int FirstInvisibleCellWidth { get; set; }
		int FirstInvisibleCellHeight { get; set; }
		PivotCellsViewInfoBase ViewInfo { get { return view.CellsArea; } }
		bool IsLeftBoundReached { get { return LeftTopCoord.X == 0; } }
		bool IsRightBoundReached { get { return LeftTopCoord.X == View.MaximumLeftTopCoord.X; } }
		bool IsTopBoundReached { get { return LeftTopCoord.Y == 0; } }
		bool IsBottomBoundReached { get { return LeftTopCoord.Y == View.MaximumLeftTopCoord.Y; } }
		int GetCellWidth(int columnIndex) {
			return ViewInfo.GetCellWidth(ViewInfo.GetColumnValue(columnIndex));
		}
		int GetCellHeight(int rowIndex) {
			return ViewInfo.GetCellHeight(ViewInfo.GetRowValue(rowIndex));
		}
		void RecalculateCellBounds() {
			FirstVisibleCellWidth = GetCellWidth(LeftTopCoord.X);
			FirstVisibleCellHeight = GetCellHeight(LeftTopCoord.Y);
			FirstInvisibleCellWidth = GetCellWidth(LeftTopCoord.X - 1);
			FirstInvisibleCellHeight = GetCellHeight(LeftTopCoord.Y - 1);
		}
		void EnsureCellBounds() {
			if(LastLeftTopCoord == LeftTopCoord) return;
			RecalculateCellBounds();
			LastLeftTopCoord = LeftTopCoord;
		}
		public void ResetOffsetAccumulator() {
			offsetAccumulator = Point.Empty;
		}
		void ApplyScrollOffset(int offsetX, int offsetY) {
			LeftTopCoord = new Point(LeftTopCoord.X + offsetX, LeftTopCoord.Y + offsetY);
		}
		void SetOverPan(ref Point overPan) {
			if((IsLeftBoundReached && offsetAccumulator.X < 0) || (IsRightBoundReached && offsetAccumulator.X > 0))
				overPan.X = -offsetAccumulator.X;
			if((IsTopBoundReached && offsetAccumulator.Y < 0) || (IsBottomBoundReached && offsetAccumulator.Y > 0))
				overPan.Y = -offsetAccumulator.Y;
		}
		public void GestureScroll(Point delta, ref Point overPan) {
			EnsureCellBounds();
			offsetAccumulator.X -= delta.X;
			offsetAccumulator.Y -= delta.Y;
			while(!IsRightBoundReached && offsetAccumulator.X >= FirstVisibleCellWidth) {
				ApplyScrollOffset(1, 0);
				offsetAccumulator.X -= WindowsFormsSettings.IsAllowPixelScrolling ? 1 : FirstVisibleCellWidth;
				EnsureCellBounds();
			}
			while(!IsLeftBoundReached && -offsetAccumulator.X >= FirstInvisibleCellWidth) {
				ApplyScrollOffset(-1, 0);
				offsetAccumulator.X += WindowsFormsSettings.IsAllowPixelScrolling ? 1 : FirstInvisibleCellWidth;
				EnsureCellBounds();
			}
			while(!IsBottomBoundReached && offsetAccumulator.Y >= FirstVisibleCellHeight) {
				ApplyScrollOffset(0, 1);
				offsetAccumulator.Y -= WindowsFormsSettings.IsAllowPixelScrolling ? 1 : FirstVisibleCellHeight;
				EnsureCellBounds();
			}
			while(!IsTopBoundReached && -offsetAccumulator.Y >= FirstInvisibleCellHeight) {
				ApplyScrollOffset(0, -1);
				offsetAccumulator.Y += WindowsFormsSettings.IsAllowPixelScrolling ? 1 : FirstInvisibleCellHeight;
				EnsureCellBounds();
			}
			SetOverPan(ref overPan);
		}
	}
}
