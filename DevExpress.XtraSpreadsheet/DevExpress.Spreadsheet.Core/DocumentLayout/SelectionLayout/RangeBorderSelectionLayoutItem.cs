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

using System.Drawing;
using System.IO;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region RangeBorderSelectionLayoutItem
	public class RangeBorderSelectionLayoutItem : RangeSelectionLayoutItemBase {
		public const int HotZoneOffset = 2;
		const int resizeHotZoneOffsetInPixels = 1;
		readonly HotZoneCollection hotZones = new HotZoneCollection();
		Rectangle clipBound;
		public RangeBorderSelectionLayoutItem(SelectionLayout layout, CellPosition topLeft, CellPosition bottomRight)
			: base(layout, topLeft, bottomRight) {
		}
		#region Properties
		public HotZoneCollection HotZones { get { return hotZones; } }
		public Rectangle ClipBound { get { return clipBound; } }
		public bool IsLeftSideVisible { get; set; }
		public bool IsRightSideVisible { get; set; }
		public bool IsTopSideVisible { get; set; }
		public bool IsBottomSideVisible { get; set; }
		#endregion
		public override void Update(Page page) {
			base.Update(page);
			UpdateSideVisibility(page);
			HotZones.Clear();
			AddHotZones();
		}
		void UpdateSideVisibility(Page page) {
			IsLeftSideVisible = !page.IsBoundsNotIntersectsWithVisibleBounds(page.GridColumns, TopLeft.Column, TopLeft.Column);
			IsRightSideVisible = !page.IsBoundsNotIntersectsWithVisibleBounds(page.GridColumns, BottomRight.Column, BottomRight.Column);
			IsTopSideVisible = !page.IsBoundsNotIntersectsWithVisibleBounds(page.GridRows, TopLeft.Row, TopLeft.Row);
			IsBottomSideVisible = !page.IsBoundsNotIntersectsWithVisibleBounds(page.GridRows, BottomRight.Row, BottomRight.Row);
		}
		public override void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
		}
		public void AddHotZones() {
			if (Layout.IsMultiSelection || !Layout.DocumentModel.BehaviorOptions.DragAllowed)
				return;
			DocumentLayoutUnitConverter converter = Layout.LayoutUnitConverter;
			int offsetInLayouts = converter.PixelsToLayoutUnits(HotZoneOffset, DocumentModel.Dpi);
			int resizeOffsetInLayouts = converter.PixelsToLayoutUnits(resizeHotZoneOffsetInPixels, DocumentModel.Dpi);
			int onePixelInLayouts = converter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			Rectangle bounds = this.Bounds;
			int left = bounds.Left - onePixelInLayouts;
			int right = bounds.Right - onePixelInLayouts;
			int top = bounds.Top - onePixelInLayouts;
			int bottom = bounds.Bottom - onePixelInLayouts;
			Rectangle resizeBounds = Rectangle.FromLTRB(right - offsetInLayouts, bottom - offsetInLayouts, right + offsetInLayouts, bottom + offsetInLayouts);
			resizeBounds = Rectangle.Inflate(resizeBounds, resizeOffsetInLayouts, resizeOffsetInLayouts);
			InnerSpreadsheetControl innerControl = Layout.View.Control.InnerControl;
			if (ShouldAddResizeHotZone()) {
				this.clipBound = resizeBounds;
				AddHotZone(new RangeResizeHotZone(innerControl), resizeBounds); 
			}
			else {
				this.clipBound = Rectangle.Empty;
				resizeOffsetInLayouts = -(2 * offsetInLayouts + resizeOffsetInLayouts);
			}
			if (IsTopSideVisible)
				AddHotZone(new RangeDragHotZone(innerControl), Rectangle.FromLTRB(left + offsetInLayouts, top - offsetInLayouts, right - offsetInLayouts, top + offsetInLayouts)); 
			if (IsBottomSideVisible)
				AddHotZone(new RangeDragHotZone(innerControl), Rectangle.FromLTRB(left + offsetInLayouts, bottom - offsetInLayouts, right - offsetInLayouts - resizeOffsetInLayouts, bottom + offsetInLayouts)); 
			if (IsLeftSideVisible)
				AddHotZone(new RangeDragHotZone(innerControl), Rectangle.FromLTRB(left - offsetInLayouts, top - offsetInLayouts, left + offsetInLayouts, bottom + offsetInLayouts)); 
			if (IsRightSideVisible)
				AddHotZone(new RangeDragHotZone(innerControl), Rectangle.FromLTRB(right - offsetInLayouts, top - offsetInLayouts, right + offsetInLayouts, bottom - offsetInLayouts - resizeOffsetInLayouts)); 
		}
		bool ShouldAddResizeHotZone() {
			SpreadsheetBehaviorOptions options = Layout.DocumentModel.BehaviorOptions;
			return options.FillHandle.Enabled && options.Selection.AllowExtendSelection && IsRightSideVisible && IsBottomSideVisible;
		}
		protected internal virtual void AddHotZone(HotZone hotZone, Rectangle bounds) {
			hotZone.Bounds = bounds;
			HotZones.Add(hotZone);
		}
	}
	#endregion
}
