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

using System.Windows;
using System.Collections.Generic;
using System;
using System.Linq;
namespace DevExpress.Xpf.Layout.Core {
	public interface ITabHeaderInfo {
		object TabHeader { get; }
		bool IsSelected { get; }
		Size DesiredSize { get; }
		Size CaptionImage { get; }
		Size CaptionText { get; }
		Size ControlBox { get; }
		double CaptionImageToCaptionDistance { get; }
		double CaptionToControlBoxDistance { get; }
		TabHeaderPinLocation PinLocation { get; }
		bool IsPinned { get; }
		Rect Rect { get; set; }
		bool IsVisible { get; set; }
		int ZIndex { get; set; }
		bool ShowCaption { get; set; }
		bool ShowCaptionImage { get; set; }
	}
	public interface ITabHeaderLayoutOptions {
		Size Size { get; }
		bool IsHorizontal { get; }
		bool IsAutoFill { get; }
		bool SelectedRowFirst { get; }
		int ScrollIndex { get; }
	}
	public interface ITabHeaderLayoutResult {
		Size Size { get; }
		bool HasScroll { get; }
		Rect[] Headers { get; }
		IScrollResult ScrollResult { get; }
		bool IsEmpty { get; }
	}
	public interface IScrollResult {
		int Index { get; }
		int MaxIndex { get; }
		bool CanScrollPrev { get; }
		bool CanScrollNext { get; }
		double ScrollOffset { get; }
	}
	public interface ITabHeaderLayoutCalculator {
		ITabHeaderLayoutResult Calc(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options);
	}
	public enum TabHeaderLayoutType {
		Default, Trim, Scroll, MultiLine
	}
	public enum TabHeaderPinLocation {
		Default, Near, Far
	}
	static class ITabHeaderInfoExtensions {
		public static bool IsRightAligned(this ITabHeaderInfo info) {
			return info.IsPinned && info.PinLocation == TabHeaderPinLocation.Far;
		}
	}
	public abstract class BaseLayoutCalculator : ITabHeaderLayoutCalculator {
		public ITabHeaderLayoutResult Calc(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			PrepareHeaders(headers, options);
			return CalcCore(headers, options);
		}
		protected List<ITabHeaderInfo> noneAlignedItems = new List<ITabHeaderInfo>();
		protected List<ITabHeaderInfo> rightAlignedItems = new List<ITabHeaderInfo>();
		protected void PrepareHeaders(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				info.IsVisible = true;
				info.ShowCaption = true;
				info.ShowCaptionImage = true;
				info.ZIndex = 0;
				if(info.IsPinned) {
					if(info.IsRightAligned()) {
						rightAlignedItems.Add(info);
					}
				}
				else noneAlignedItems.Add(info);
				OnPrepareHeader(info, options);
			}
		}
		protected abstract ITabHeaderLayoutResult CalcCore(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options);
		protected virtual void OnPrepareHeader(ITabHeaderInfo info, ITabHeaderLayoutOptions options) { }
		protected virtual ITabHeaderLayoutResult CalcRow(Rect row, bool horz, ITabHeaderInfo[] headers) {
			double offset = 0; double dim = 0;
			double rightOffset = TabHeaderHelper.GetLength(horz, row);
			for(int i = headers.Length - 1; i >= 0; i--) {
				ITabHeaderInfo info = headers[i];
				if(!info.IsRightAligned()) continue;
				Size size = TabHeaderHelper.GetSize(info, horz);
				rightOffset -= TabHeaderHelper.GetLength(horz, size);
				info.Rect = TabHeaderHelper.Arrange(row, horz, rightOffset, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				if(info.IsRightAligned()) continue;
				Size size = TabHeaderHelper.GetSize(info, horz);
				info.Rect = TabHeaderHelper.Arrange(row, horz, offset, size);
				offset += TabHeaderHelper.GetLength(horz, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			if(offset == 0) return Result.Empty;
			if(rightAlignedItems.Count > 0) offset = Math.Max(offset, CheckInfinity(TabHeaderHelper.GetLength(horz, row)));
			Size result = TabHeaderHelper.GetSize(horz, offset, dim);
			Rect[] rects = JustifyRowHeaders(headers, horz, result);
			return new Result(result, (headers.Length > 0 && MathHelper.IsZero(offset)) || (offset > TabHeaderHelper.GetLength(horz, row)), rects);
		}
		protected ITabHeaderLayoutResult CalcScaledCaptions(Rect header, bool horz, ITabHeaderInfo[] headers, double scaleFactor) {
			double summaryRounding = 0;
			double offset = 0; double dim = 0;
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				Size size = TabHeaderHelper.GetScaledSize(info, horz, scaleFactor, ref summaryRounding);
				info.Rect = TabHeaderHelper.Arrange(header, horz, offset, size);
				offset += TabHeaderHelper.GetLength(horz, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			Size result = TabHeaderHelper.GetSize(horz, offset, dim);
			Rect[] rects = JustifyRowHeaders(headers, horz, result);
			return new Result(result, rects);
		}
		protected Rect[] JustifyRowHeaders(ITabHeaderInfo[] headers, bool horz, Size size) {
			Rect[] rects = new Rect[headers.Length];
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				if(!info.IsVisible) continue;
				rects[i] = info.Rect = new Rect(
						CheckInfinity(info.Rect.Left), CheckInfinity(info.Rect.Top),
						horz ? info.Rect.Width : size.Width, horz ? size.Height : info.Rect.Height
					);
			}
			return rects;
		}
		protected static double CheckInfinity(double pos) {
			return double.IsInfinity(pos) ? 0 : pos;
		}
		protected double CalcScaleFactor(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			double onlyCaptionImageAndControlBox = 0;
			double onlyCaptions = 0;
			double captionSpaces = 0;
			bool horz = options.IsHorizontal;
			double pinned = 0;
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				if(info.IsPinned) {
					pinned += TabHeaderHelper.GetLength(horz, info.DesiredSize);
					continue;
				}
				double length = TabHeaderHelper.GetLength(horz, info.DesiredSize);
				double caption = TabHeaderHelper.GetLength(horz, info.CaptionText);
				double captionWithSpace = caption + info.CaptionToControlBoxDistance;
				double captionImageWithSpace = TabHeaderHelper.GetLength(horz, info.CaptionImage) + info.CaptionImageToCaptionDistance;
				onlyCaptions += caption;
				captionSpaces += info.CaptionToControlBoxDistance;
				onlyCaptionImageAndControlBox += (length - captionWithSpace);
			}
			double result = (TabHeaderHelper.GetLength(horz, options.Size) - pinned - onlyCaptionImageAndControlBox - captionSpaces) / onlyCaptions;
			return double.IsPositiveInfinity(result) || double.IsNaN(result) ? 1 : result;
		}
		protected class ScrollResult : IScrollResult {
			public ScrollResult(int index, int maxIndex, double offset) {
				Index = index;
				MaxIndex = maxIndex;
				CanScrollNext = index < maxIndex;
				CanScrollPrev = index > 0;
				ScrollOffset = offset;
			}
			public int Index { get; private set; }
			public int MaxIndex { get; private set; }
			public bool CanScrollPrev { get; private set; }
			public bool CanScrollNext { get; private set; }
			public double ScrollOffset { get; private set; }
		}
		protected class Result : ITabHeaderLayoutResult {
			static Result() {
				Empty = new Result(Size.Empty, false, new Rect[0]);
			}
			public Result(Size result, Rect[] headers)
				: this(result, false, headers) {
			}
			public Result(Size result, bool hasScroll, Rect[] headers) {
				Size = result;
				HasScroll = hasScroll;
				Headers = headers;
				ScrollResult = null;
			}
			public Result(ITabHeaderLayoutResult result, IScrollResult scrollResult) {
				Size = result.Size;
				HasScroll = result.HasScroll;
				Headers = result.Headers;
				ScrollResult = scrollResult;
			}
			public Size Size { get; private set; }
			public bool HasScroll { get; private set; }
			public Rect[] Headers { get; private set; }
			public IScrollResult ScrollResult { get; private set; }
			public static Result Empty { get; private set; }
			public bool IsEmpty { get { return object.Equals(this, Empty); } }
		}
	}
	public class TrimLayoutCalculator : BaseLayoutCalculator {
		protected ITabHeaderLayoutResult CalcOnlyControlBoxes(Rect header, bool horz, ITabHeaderInfo[] headers, double onlyControlBox, double available) {
			double selected = available - onlyControlBox;
			double offset = 0; double dim = 0;
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				if(!info.IsPinned) {
					info.ShowCaption = false;
					info.ShowCaptionImage = false;
				}
				Size size = TabHeaderHelper.GetSize(info, horz);
				if(info.IsSelected && !info.IsPinned) {
					info.ShowCaption = true;
					info.ShowCaptionImage = true;
					if(horz) size.Width += selected;
					else size.Height += selected;
				}
				info.Rect = TabHeaderHelper.Arrange(header, horz, offset, size);
				offset += TabHeaderHelper.GetLength(horz, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			Size result = TabHeaderHelper.GetSize(horz, offset, dim);
			Rect[] rects = JustifyRowHeaders(headers, horz, result);
			return new Result(result, rects);
		}
		protected ITabHeaderLayoutResult CalcOnlyVisibleElement(Rect header, bool horz, ITabHeaderInfo[] headers, double available) {
			double offset = 0; double dim = 0;
			double remainingSpace = available;
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				if(!info.IsPinned) continue;
				Size size = TabHeaderHelper.GetSize(info, horz);
				if(info.IsPinned && info.PinLocation == TabHeaderPinLocation.Far) {
					remainingSpace -= TabHeaderHelper.GetLength(horz, size);
					info.Rect = TabHeaderHelper.Arrange(header, horz, remainingSpace, size);
				}
				else {
					info.Rect = TabHeaderHelper.Arrange(header, horz, offset, size);
					offset += TabHeaderHelper.GetLength(horz, size);
				}
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				if(info.IsPinned && info.PinLocation == TabHeaderPinLocation.Far) continue;
				if(info.IsPinned) continue;
				info.ShowCaption = false;
				info.ShowCaptionImage = false;
				Size size = TabHeaderHelper.GetSize(info, horz);
				if(offset + TabHeaderHelper.GetLength(horz, size) > remainingSpace) {
					if(offset + TabHeaderHelper.GetLength(horz, info.ControlBox) > remainingSpace) {
						info.IsVisible = false;
						info.Rect = Rect.Empty;
						continue;
					}
					else {
						size = info.ControlBox;
					}
				}
				info.Rect = TabHeaderHelper.Arrange(header, horz, offset, size);
				offset += TabHeaderHelper.GetLength(horz, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			if(rightAlignedItems.Count > 0) offset = Math.Max(offset, CheckInfinity(available));
			Size result = TabHeaderHelper.GetSize(horz, offset, dim);
			Rect[] rects = JustifyRowHeaders(headers, horz, result);
			return new Result(result, rects);
		}
		double allElements = 0;
		double onlyCaptionImageAndControlBox = 0;
		double onlyControlBox = 0;
		double onlyCaptions = 0;
		double captionSpaces = 0;
		double pinnedLengths = 0;
		protected override void OnPrepareHeader(ITabHeaderInfo info, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			Size header = options.Size;
			double length = TabHeaderHelper.GetLength(horz, info.DesiredSize);
			double caption = TabHeaderHelper.GetLength(horz, info.CaptionText);
			double captionWithSpace = caption + info.CaptionToControlBoxDistance;
			double captionImageWithSpace = TabHeaderHelper.GetLength(horz, info.CaptionImage) + info.CaptionImageToCaptionDistance;
			double controlBox = TabHeaderHelper.GetLength(horz, info.ControlBox);
			info.ZIndex = info.IsPinned ? 2 : info.IsSelected ? 1 : 0;
			if(info.IsPinned) {
				pinnedLengths += length;
				return;
			}
			allElements += length;
			onlyCaptions += caption;
			captionSpaces += info.CaptionToControlBoxDistance;
			onlyCaptionImageAndControlBox += (length - captionWithSpace);
			onlyControlBox += (length - captionWithSpace - captionImageWithSpace);
		}
		protected override ITabHeaderLayoutResult CalcCore(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			Size header = options.Size;
			Rect headerRect = new Rect(0, 0, header.Width, header.Height);
			double available1 = TabHeaderHelper.GetLength(horz, header);
			double available = TabHeaderHelper.GetLength(horz, header) - pinnedLengths;
			if(allElements < available) {
				if(options.IsAutoFill) {
					double factor = CalcScaleFactor(headers, options);
					if(factor > 1.0)
						return CalcScaledCaptions(headerRect, options.IsHorizontal, headers, factor);
				}
				return CalcRow(headerRect, options.IsHorizontal, headers);
			}
			if(onlyCaptionImageAndControlBox < available) {
				double factor = (available - onlyCaptionImageAndControlBox - captionSpaces) / onlyCaptions;
				return CalcScaledCaptions(headerRect, horz, headers, factor);
			}
			if(onlyControlBox < available) {
				return CalcOnlyControlBoxes(headerRect, horz, headers, onlyControlBox, available);
			}
			return CalcOnlyVisibleElement(headerRect, horz, headers, available1);
		}
	}
	public class ScrollLayoutCalculator : BaseLayoutCalculator {
		double pinnedLengths = 0;
		protected override void OnPrepareHeader(ITabHeaderInfo info, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			Size header = options.Size;
			double length = TabHeaderHelper.GetLength(horz, info.DesiredSize);
			info.ZIndex = info.IsPinned ? 2 : info.IsSelected ? 1 : 0;
			if(info.IsPinned) {
				pinnedLengths += length;
				return;
			}
		}
		protected override ITabHeaderLayoutResult CalcRow(Rect row, bool horz, ITabHeaderInfo[] headers) {
			double offset = 0; double dim = 0;
			double rightOffset = TabHeaderHelper.GetLength(horz, row);
			for(int i = headers.Length - 1; i >= 0; i--) {
				ITabHeaderInfo info = headers[i];
				if(!info.IsRightAligned()) continue;
				Size size = TabHeaderHelper.GetSize(info, horz);
				rightOffset -= TabHeaderHelper.GetLength(horz, size);
				info.Rect = TabHeaderHelper.Arrange(row, horz, rightOffset, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				Size size = TabHeaderHelper.GetSize(info, horz);
				if(!info.IsRightAligned()) {
					info.Rect = TabHeaderHelper.Arrange(row, horz, offset, size);
				}
				offset += TabHeaderHelper.GetLength(horz, size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(!horz, size));
			}
			if(offset == 0) return Result.Empty;
			double scrollOffset = offset - pinnedLengths;
			if(rightAlignedItems.Count > 0) offset = Math.Max(offset, CheckInfinity(TabHeaderHelper.GetLength(horz, row)));
			Size result = TabHeaderHelper.GetSize(horz, offset, dim);
			Rect[] rects = JustifyRowHeaders(headers, horz, result);
			return new Result(result, (headers.Length > 0 && MathHelper.IsZero(scrollOffset) && noneAlignedItems.Count > 0) || (scrollOffset > TabHeaderHelper.GetLength(horz, row) - pinnedLengths), rects);
		}
		protected IScrollResult ScrollRowHeaders(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options, ITabHeaderLayoutResult result) {
			bool horz = options.IsHorizontal;
			double scrollLength = TabHeaderHelper.GetLength(horz, options.Size) - pinnedLengths;
			double length = TabHeaderHelper.GetLength(horz, result.Size) - pinnedLengths;
			double[] near = new double[headers.Length];
			int maxIndex = 0;
			double prev = 0;
			for(int i = 0; i < headers.Length; i++) {
				ITabHeaderInfo info = headers[i];
				near[i] = (i > 0) ? near[i - 1] + prev : 0;
				prev = TabHeaderHelper.GetLength(horz, info.Rect);
				if(length - near[i] < scrollLength) {
					maxIndex = i;
					break;
				}
			}
			int index = Math.Max(0, Math.Min(maxIndex, options.ScrollIndex));
			double offset = (index == maxIndex) ? scrollLength - length : -near[index];
			for(int i = 0; i < headers.Length; i++) {
				Rect h = headers[i].Rect;
				if(horz) {
					if(!headers[i].IsPinned) {
						result.Headers[i].X += offset;
						h.X += offset;
					}
				}
				else {
					if(!headers[i].IsPinned) {
						result.Headers[i].Y += offset;
						h.Y += offset;
					}
				}
				headers[i].Rect = h;
			}
			double scrollOffset = TabHeaderHelper.GetLength(horz, headers[index].Rect);
			return new ScrollResult(index, maxIndex, scrollOffset);
		}
		protected override ITabHeaderLayoutResult CalcCore(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			Rect headerRect = new Rect(0, 0, options.Size.Width, options.Size.Height);
			if(options.IsAutoFill) {
				double factor = CalcScaleFactor(headers, options);
				if(factor > 1.0)
					return CalcScaledCaptions(headerRect, options.IsHorizontal, headers, factor);
			}
			ITabHeaderLayoutResult rowResult = CalcRow(headerRect, options.IsHorizontal, headers);
			if(rowResult.HasScroll) {
				rowResult = new Result(rowResult, ScrollRowHeaders(headers, options, rowResult));
			}
			return rowResult;
		}
	}
	public class MultiLineLayoutCalculator : BaseLayoutCalculator {
		double allElements = 0;
		protected override void OnPrepareHeader(ITabHeaderInfo info, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			Size header = options.Size;
			double length = TabHeaderHelper.GetLength(horz, info.DesiredSize);
			allElements += length;
		}
		protected override ITabHeaderLayoutResult CalcCore(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			List<Row> rows;
			if(allElements > TabHeaderHelper.GetLength(horz, options.Size)) {
				List<ITabHeaderInfo> pinned = new List<ITabHeaderInfo>();
				List<ITabHeaderInfo> nonPinned = new List<ITabHeaderInfo>();
				foreach(var header in headers) {
					if(header.IsPinned) pinned.Add(header);
					else nonPinned.Add(header);
				}
				if(options.SelectedRowFirst) {
					rows = GetRows(nonPinned.ToArray(), options);
					rows = rows.Concat(GetRows(pinned.ToArray(), options)).ToList();
				}
				else {
					rows = GetRows(pinned.ToArray(), options);
					rows = rows.Concat(GetRows(nonPinned.ToArray(), options)).ToList();
				}
			}
			else {
				rows = GetRows(headers, options);
			}
			double offset = 0; double dim = 0;
			List<Rect> rects = new List<Rect>();
			foreach(Row row in rows) {
				Rect rowRect = new Rect(horz ? 0 : offset, horz ? offset : 0, row.Size.Width, row.Size.Height);
				ITabHeaderLayoutResult rowResult;
				if(options.IsAutoFill || rows.Count > 1) {
					double factor = CalcScaleFactor(row.Headers, options);
					rowResult = CalcScaledCaptions(rowRect, horz, row.Headers, factor);
				}
				else rowResult = CalcRow(new Rect(options.Size), horz, row.Headers);
				offset += rowResult.Size.IsEmpty ? 0 : TabHeaderHelper.GetLength(!horz, rowResult.Size);
				dim = Math.Max(dim, TabHeaderHelper.GetLength(horz, rowResult.Size));
				rects.AddRange(rowResult.Headers);
			}
			return new Result(TabHeaderHelper.GetSize(horz, dim, offset), rects.ToArray());
		}
		List<Row> GetRows(ITabHeaderInfo[] headers, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			Size header = options.Size;
			int i = 0; double rowLength = 0; double rowDim = 0;
			List<Row> rows = new List<Row>();
			List<ITabHeaderInfo> currentRow = new List<ITabHeaderInfo>();
			while(i < headers.Length) {
				ITabHeaderInfo info = headers[i++];
				Size size = TabHeaderHelper.GetSize(info, horz);
				double len = TabHeaderHelper.GetLength(horz, size);
				double dim = TabHeaderHelper.GetLength(!horz, size);
				if(rowLength + len > TabHeaderHelper.GetLength(horz, header)) {
					rows.Add(new Row(currentRow.ToArray(), TabHeaderHelper.GetSize(horz, rowLength, rowDim)));
					currentRow.Clear();
					rowLength = 0;
					rowDim = 0;
				}
				currentRow.Add(info);
				rowLength += len;
				rowDim = Math.Max(rowDim, dim);
			}
			if(currentRow.Count > 0) {
				rows.Add(new Row(currentRow.ToArray(), TabHeaderHelper.GetSize(horz, rowLength, rowDim)));
				if(rows.Count > 1) {
					CheckBalance(rows[rows.Count - 2], rows[rows.Count - 1], options);
				}
			}
			if(rows.Count > 0) {
				Row selectedRow = GetSelected(rows);
				rows.Remove(selectedRow);
				if(options.SelectedRowFirst)
					rows.Insert(0, selectedRow);
				else
					rows.Add(selectedRow);
				UpdateZIndex(rows, options.SelectedRowFirst);
			}
			return rows;
		}
		void CheckBalance(Row r1, Row r2, ITabHeaderLayoutOptions options) {
			bool horz = options.IsHorizontal;
			double total = r1.GetLengtn(horz) + r2.GetLengtn(horz);
			r1.Balance(r2, total * 0.5, horz);
		}
		void UpdateZIndex(List<Row> rows, bool first) {
			foreach(Row r in rows) {
				for(int i = 0; i < r.Headers.Length; i++) {
					r.Headers[i].ZIndex = first ? rows.Count - rows.IndexOf(r) : rows.IndexOf(r);
				}
			}
		}
		Row GetSelected(List<Row> rows) {
			foreach(Row r in rows) {
				for(int i = 0; i < r.Headers.Length; i++) {
					if(r.Headers[i].IsSelected) return r;
				}
			}
			return rows[0];
		}
		public class Row {
			public Row(ITabHeaderInfo[] headers, Size size) {
				Headers = headers;
				Size = size;
			}
			public ITabHeaderInfo[] Headers { get; private set; }
			public Size Size { get; private set; }
			public double GetLengtn(bool horz) {
				double result = 0;
				for(int i = 0; i < Headers.Length; i++) {
					result += TabHeaderHelper.GetLength(Headers[i], horz);
				}
				return result;
			}
			public void Balance(Row target, double threshold, bool horz) {
				double result = 0;
				List<ITabHeaderInfo> sourceHeaders = new List<ITabHeaderInfo>(Headers);
				List<ITabHeaderInfo> targetHeaders = new List<ITabHeaderInfo>(target.Headers);
				List<ITabHeaderInfo> movedHeaders = new List<ITabHeaderInfo>();
				for(int i = 0; i < Headers.Length; i++) {
					ITabHeaderInfo info = Headers[i];
					double l = TabHeaderHelper.GetLength(info, horz);
					if(result + 0.5 * l > threshold) {
						sourceHeaders.Remove(info);
						movedHeaders.Insert(0, info);
					}
					result += l;
				}
				for(int i = 0; i < movedHeaders.Count; i++) {
					targetHeaders.Insert(0, movedHeaders[i]);
				}
				Headers = sourceHeaders.ToArray();
				target.Headers = targetHeaders.ToArray();
			}
		}
	}
	public static class HeaderLayoutCalculatorFactory {
		public static ITabHeaderLayoutCalculator GetCalculator(TabHeaderLayoutType type) {
			switch(type) {
				case TabHeaderLayoutType.Trim:
					return new TrimLayoutCalculator();
				case TabHeaderLayoutType.Scroll:
					return new ScrollLayoutCalculator();
				case TabHeaderLayoutType.MultiLine:
					return new MultiLineLayoutCalculator();
			}
			return new ScrollLayoutCalculator();
		}
	}
}
