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
	#region RangeMailMergeBorderSelectionLayoutItem
	public class RangeMailMergeBorderSelectionLayoutItem : RangeSelectionLayoutItemBase {
		public const int HotZoneOffset = 2;
		const int ResizeHotZoneOffsetInPixels = 1;
		readonly HotZoneCollection hotZones = new HotZoneCollection();
		Rectangle clipBound;
		string mailMergeDefinedName;
		public RangeMailMergeBorderSelectionLayoutItem(SelectionLayout layout, CellPosition topLeft, CellPosition bottomRight,
													   string mailMergeDefinedName)
			: base(layout, topLeft, bottomRight) {
			this.mailMergeDefinedName = mailMergeDefinedName;
		}
		public HotZoneCollection HotZones { get { return hotZones; } }
		public Rectangle ClipBound { get { return clipBound; } }
		public override void Update(Page page) {
			base.Update(page);
			HotZones.Clear();
			AddHotZones();
		}
		public override void Draw(ISelectionPainter selectionPainter) {
		}
		public void AddHotZones() {
			Rectangle bounds = this.Bounds;
			InnerSpreadsheetControl innerControl = Layout.View.Control.InnerControl;
			DocumentLayoutUnitConverter converter = Layout.LayoutUnitConverter;
			int offsetInLayouts = converter.PixelsToLayoutUnits(HotZoneOffset, DocumentModel.Dpi);
			int resizeOffsetInLayouts = converter.PixelsToLayoutUnits(ResizeHotZoneOffsetInPixels, DocumentModel.Dpi);
			int onePixelInLayouts = converter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			int left = bounds.Left - onePixelInLayouts;
			int right = bounds.Right - onePixelInLayouts;
			int top = bounds.Top - onePixelInLayouts;
			int bottom = bounds.Bottom - onePixelInLayouts;
			Rectangle resizeBounds = Rectangle.FromLTRB(right - offsetInLayouts, bottom - offsetInLayouts, right + offsetInLayouts, bottom + offsetInLayouts);
			resizeBounds = Rectangle.Inflate(resizeBounds, resizeOffsetInLayouts, resizeOffsetInLayouts);
			this.clipBound = resizeBounds;
			AddHotZone(new MailMergeRangeResizeHotZone(innerControl, true, true, mailMergeDefinedName), Rectangle.FromLTRB(left - offsetInLayouts, top - offsetInLayouts, left + offsetInLayouts, top + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeResizeHotZone(innerControl, true, false, mailMergeDefinedName), Rectangle.FromLTRB(right - offsetInLayouts, top - offsetInLayouts, right + offsetInLayouts, top + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeResizeHotZone(innerControl, false, false, mailMergeDefinedName), Rectangle.FromLTRB(right - offsetInLayouts, bottom - offsetInLayouts, right + offsetInLayouts, bottom + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeResizeHotZone(innerControl, false, true, mailMergeDefinedName), Rectangle.FromLTRB(left - offsetInLayouts, bottom - offsetInLayouts, left + offsetInLayouts, bottom + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeMoveHotZone(innerControl, mailMergeDefinedName), Rectangle.FromLTRB(left + offsetInLayouts, top - offsetInLayouts, right - offsetInLayouts, top + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeMoveHotZone(innerControl, mailMergeDefinedName), Rectangle.FromLTRB(left + offsetInLayouts, bottom - offsetInLayouts, right - offsetInLayouts - resizeOffsetInLayouts, bottom + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeMoveHotZone(innerControl, mailMergeDefinedName), Rectangle.FromLTRB(left - offsetInLayouts, top - offsetInLayouts, left + offsetInLayouts, bottom + offsetInLayouts)); 
			AddHotZone(new MailMergeRangeMoveHotZone(innerControl, mailMergeDefinedName), Rectangle.FromLTRB(right - offsetInLayouts, top - offsetInLayouts, right + offsetInLayouts, bottom - offsetInLayouts - resizeOffsetInLayouts)); 
		}
		protected internal virtual void AddHotZone(HotZone hotZone, Rectangle bounds) {
			hotZone.Bounds = bounds;
			HotZones.Add(hotZone);
		}
	}
	#endregion
}
