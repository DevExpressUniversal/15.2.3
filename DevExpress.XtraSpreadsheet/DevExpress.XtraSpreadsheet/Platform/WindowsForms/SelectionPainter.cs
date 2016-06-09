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
using System.Drawing;
using System.IO;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.Office.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Drawing;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Utils;
using System.Reflection;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetSelectionPainter
	public abstract class SpreadsheetSelectionPainter : OfficeSelectionPainter, ISelectionPainter, IHotZoneVisitor {
		#region Fields
		protected static readonly Color selectionColor = Color.FromArgb(128, 125, 190, 255);
		protected static readonly Color mailMergeRangeBorderColor = Color.FromArgb(255, 74, 203, 252);
		protected static readonly Color selectionBorderColor = Color.Black;
		bool isMultiSelection;
		#endregion
		protected SpreadsheetSelectionPainter(GraphicsCache cache)
			: base(cache) {
			ResetAutoFilterImages();
		}
		#region Properties
		public new GraphicsCache Cache { get { return (GraphicsCache)base.Cache; } }
		protected bool IsMultiSelection { get { return isMultiSelection; } }
		public override SmoothingMode FillSmoothingMode { get { return SmoothingMode.AntiAlias; } }
		public override PixelOffsetMode FillPixelOffsetMode { get { return PixelOffsetMode.HighSpeed; } }
		#endregion
		delegate void DrawSelectionLayoutItem(RangeSelectionLayoutItemBase item);
		public void Draw(PageSelectionLayoutItem item) {
			this.isMultiSelection = item.Layout.IsMultiSelection;
			DrawWithExcludedBounds(item.Bounds, item, DrawPageSelectionLayoutItem);
			item.BorderItem.Draw(this);
		}
		void DrawPageSelectionLayoutItem(RangeSelectionLayoutItemBase item) {
			List<RangeSelectionLayoutItem> innerItems = ((PageSelectionLayoutItem)item).InnerItems;
			int count = innerItems.Count;
			for (int i = 0; i < count; i++) {
				innerItems[i].Draw(this);
			}
		}
		public void Draw(RangeSelectionLayoutItem item) {
			DrawWithExcludedBounds(item.ClipBounds, item, DrawRangeSelectionLayoutItem);
		}
		void DrawRangeSelectionLayoutItem(RangeSelectionLayoutItemBase item) {
			FillRectangle(item.Bounds);
		}
		public void Draw(RangeMailMergeLayoutItem item) {
			Cache.DrawRectangle(Cache.GetPen(mailMergeRangeBorderColor, GetConvertedMailMergeRangeWidth()), item.Bounds);
			Cache.FillRectangle(mailMergeRangeBorderColor, item.TextBounds);
			StringFormat format = StringFormat.GenericTypographic;
			format.LineAlignment = StringAlignment.Center;
			format.Alignment = StringAlignment.Near;
			Cache.DrawString(item.Text, item.FontInfo.Font, Brushes.White, item.TextBounds, format);
		}
		public void Draw(PrintRangeSelectionLayoutSubItem item) {
			DrawPrintRangeRectangle(item.Bounds);
		}
		protected abstract int GetConvertedMailMergeRangeWidth();
		void DrawWithExcludedBounds(Rectangle excludedBound, RangeSelectionLayoutItemBase item, DrawSelectionLayoutItem action) {
			if (excludedBound == Rectangle.Empty)
				action(item);
			else {
				List<Rectangle> excludedBounds = new List<Rectangle>();
				excludedBounds.Add(excludedBound);
				DrawWithExcludedBounds(excludedBounds, item, action);
			}
		}
		void DrawWithExcludedBounds(List<Rectangle> excludedBounds, RangeSelectionLayoutItemBase item, DrawSelectionLayoutItem action) {
			GraphicsClipState clipState = Cache.ClipInfo.SaveClip();
			try {
				ExcludeRanges(excludedBounds);
				action(item);
			}
			finally {
				Cache.ClipInfo.RestoreClipRelease(clipState);
			}
		}
		public void Draw(RangeBorderSelectionLayoutItem item) {
			if (IsMultiSelection) {
				DrawRectangle(item.Bounds);
			}
			else {
				DrawWithExcludedBounds(item.ClipBound, item, DrawRangeBorderSelectionLayoutItem);
				DrawHotZones(item.HotZones);
			}
		}
		void DrawRangeBorderSelectionLayoutItem(RangeSelectionLayoutItemBase item) {
			DrawRectangle(item.Bounds);
		}
		public void Draw(RangeBorderDashSelectionLayoutItem item) {
			DrawDashRectangle(item.Bounds);
		}
		public void Draw(PageDashSelectionLayoutItem item) {
			for (int i = 0; i < item.BorderItems.Count; i++)
				item.BorderItems[i].Draw(this);
		}
		public void Draw(CutCopyRangeDashBorderLayoutItem item) {
			DrawDashRectangle(item.Bounds);
		}
		public void Draw(PictureSelectionLayoutItem item) {
			Rectangle bounds = item.Bounds;
			Point center = RectangleUtils.CenterPoint(bounds);
			bool transformApplied = TryPushRotationTransform(center, item.DocumentModel.GetPictureRotationAngleInDegrees(item.PictureIndex));
			try {
				DrawRectangle(bounds);
				DrawHotZones(item.HotZones);
			}
			finally {
				if (transformApplied)
					PopTransform();
			}
		}
		void ExcludeRanges(List<Rectangle> clipBounds) {
			int count = clipBounds.Count;
			for (int i = 0; i < count; i++) {
				Rectangle bounds = clipBounds[i];
				if (bounds != Rectangle.Empty)
					Graphics.ExcludeClip(bounds);
			}
		}
		protected internal void DrawHotZones(HotZoneCollection hotZones) {
			hotZones.ForEach(DrawHotZone);
		}
		protected internal void DrawHotZone(HotZone hotZone) {
			hotZone.Visit(this);
		}
		protected internal virtual Color GetSelectionBorderColor() {
			return selectionBorderColor;
		}
		protected internal virtual Color GetPrintRangeBorderColor() {
			return DXColor.Gray;
		}
		#region IHotZoneVisitor Members
		public void Visit(DrawingObjectRotationHotZone hotZone) {
			DrawLine(RectangleUtils.CenterPoint(hotZone.Bounds), hotZone.LineEnd);
			DrawEllipticHotZone(hotZone.Bounds, HotZoneRotationGradientColor);
		}
		public void Visit(IResizeHotZone hotZone) {
			Rectangle bounds = hotZone.Bounds;
			if (hotZone.Type == ResizeHotZoneType.Ellipse)
				DrawEllipticHotZone(bounds, HotZoneGradientColor);
			else {
				System.Diagnostics.Debug.Assert(hotZone.Type == ResizeHotZoneType.Rectangle);
				DrawRectangularHotZone(bounds, HotZoneGradientColor);
			}
		}
		public void Visit(RangeDragHotZone hotZone) {
		}
		public void Visit(RangeResizeHotZone hotZone) {
			DrawRangeResizeHotZone(hotZone.Bounds);
		}
		public void Visit(FormulaRangeDragHotZone hotZone) {
		}
		public void Visit(FormulaRangeResizeHotZone hotZone) {
		}
		public void Visit(IFilterHotZone hotZone) {
			DrawColumnAutoFilter(hotZone);
		}
		void IHotZoneVisitor.Visit(CommentDragHotZone hotZone) {
		}
		void IHotZoneVisitor.Visit(DataValidationHotZone hotZone) {
			DrawDataValidationHotZone(hotZone);
		}
		void IHotZoneVisitor.Visit(PivotTableExpandCollapseHotZone hotZone) {
			DrawPivotTableExpandCollapseHotZone(hotZone.Bounds, hotZone.IsCollapsed);
		}
		#endregion
		string[] autoFilterImageNames = new string[] { "DropDown", "Filtered", "Ascending", "Descending", "FilteredAndAscending", "FilteredAndDescending" };
		Image[] autoFilterImages;
		void ResetAutoFilterImages() {
			this.autoFilterImages = new Image[autoFilterImageNames.Length];
		}
		Image GetAutoFilterImage(int index) {
			index = Math.Min(Math.Max(0, index), autoFilterImageNames.Length - 1);
			Image result = autoFilterImages[index];
			if (result != null)
				return result;
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraSpreadsheet.Images.AutoFilter." + autoFilterImageNames[index] + ".png");
			if (stream == null)
				return null;
			result = DevExpress.Data.Utils.ImageTool.ImageFromStream(stream);
			result = ObtainActualAutoFilterGlyph(result);
			autoFilterImages[index] = result;
			return result;
		}
		int CalculateAutoFilterImageIndex(AutoFilterColumnHotZone hotZone) {
			SortCondition sortCondition = hotZone.SortCondition;
			if (hotZone.FilterColumn.IsNonDefault) {
				if (sortCondition == null)
					return 1;
				return sortCondition.Descending ? 5 : 4;
			}
			else {
				if (sortCondition == null)
					return 0;
				return sortCondition.Descending ? 3 : 2;
			}
		}
		int CalculatePivotAutoFilterImageIndex(PivotTableFilterHotZone hotZone) {
			if (hotZone.IsFilterApplied) {
				if (hotZone.IsSortApplied)
					return hotZone.SortTypeDescending ? 5 : 4;
				return 1;
			}
			else
				if (hotZone.IsSortApplied)
					return hotZone.SortTypeDescending ? 3 : 2;
				return 0;
		}
		protected virtual void DrawColumnAutoFilter(IFilterHotZone hotZone) {
			int imageIndex = 0;
			if (hotZone is AutoFilterColumnHotZone)
				imageIndex = CalculateAutoFilterImageIndex(hotZone as AutoFilterColumnHotZone);
			else
				imageIndex = CalculatePivotAutoFilterImageIndex(hotZone as PivotTableFilterHotZone);
			DrawAutoFilterImage(hotZone.BoundsHotZone, imageIndex);
		}
		protected virtual void DrawDataValidationHotZone(DataValidationHotZone hotZone) {
			DrawAutoFilterImage(hotZone.Bounds, 0);
		}
		void DrawAutoFilterImage(Rectangle bounds, int imageIndex) {
			DrawColumnAutoFilterBackground(bounds);
			Image image = GetAutoFilterImage(imageIndex);
			if (image == null)
				return;
			Point location = bounds.Location;
			location.X += (bounds.Width - image.Width) / 2;
			location.Y += (bounds.Height - image.Height) / 2;
			Cache.Graphics.DrawImageUnscaled(image, location);
		}
		protected abstract void DrawDashRectangle(Rectangle bounds);
		protected abstract void DrawPrintRangeRectangle(Rectangle bounds);
		protected abstract void DrawRangeResizeHotZone(Rectangle bounds);
		protected abstract void DrawColumnAutoFilterBackground(Rectangle bounds);
		protected abstract void DrawPivotTableExpandCollapseHotZone(Rectangle bounds, bool isCollapsed);
		protected virtual Image ObtainActualAutoFilterGlyph(Image image) {
			return image;
		}
	}
	#endregion
}
