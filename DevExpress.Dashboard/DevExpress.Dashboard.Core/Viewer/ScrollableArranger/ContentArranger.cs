#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class ContentArranger {
		const int DefaultLineCount = 1;
		const int DefaultItemProportions = 1;
		ICollection items;
		Size itemProportionsValue = new Size(1, 1);
		Size itemMargin = Size.Empty;
		Size itemMinSize = Size.Empty;
		Size size = Size.Empty;
		ContentArrangementMode mode = ContentArrangementMode.Auto;
		ContentArrangementOptions options = ContentArrangementOptions.Default;
		ArrangementInfo arrangementInfo;
		int contentLineCount = DefaultLineCount;
		decimal itemProportions = 1;
		decimal borderProportion = 0;
		event EventHandler changed;
		int ItemsInLineCount {
			get { return Convert.ToInt32(Math.Ceiling((decimal)items.Count / contentLineCount)); }
		}
		public int ContentLineCount {
			get { return contentLineCount; }
			set {
				if(value != contentLineCount) {
					if(value <= 0)
						value = DefaultLineCount;
					contentLineCount = value;
					RaiseChanged();
				}
			}
		}
		public ContentArrangementMode ContentArrangementMode {
			get { return mode; }
			set {
				if(value != mode) {
					mode = value;
					RaiseChanged();
				}
			}
		}
		public ContentArrangementOptions ContentArrangementOptions {
			get { return options; }
			set {
				if(value != options) {
					options = value;
					RaiseChanged();
				}
			}
		}
		public Size ItemMargin {
			get { return itemMargin; }
			set {
				if(value != itemMargin) {
					itemMargin = value;
					RaiseChanged();
				}
			}
		}
		public Size ItemMinSize {
			get { return itemMinSize; }
			set {
				if(value != itemMinSize) {
					itemMinSize = value;
					RaiseChanged();
				}
			}
		}
		public Size ItemProportions {
			get { return itemProportionsValue; }
			set {
				if(value != itemProportionsValue) {
					itemProportionsValue = value;
					InitializeItemProportions();
					RaiseChanged();
				}
			}
		}
		public event EventHandler Changed {
			add { changed = (EventHandler)Delegate.Combine(changed, value); }
			remove { changed = (EventHandler)Delegate.Remove(changed, value); }
		}
		public ContentArranger(ICollection items)
			: this(items, new Size(1, 1), Size.Empty, 0, 0) {
		}
		public ContentArranger(ICollection items, Size itemProportions, Size itemMargin, int itemMinWidth, decimal borderProportion) {
			Initialize(items);
			Initialize(itemProportions, itemMargin, itemMinWidth, false, borderProportion);
		}
		public void Initialize(ICollection items) {
			this.items = items;
		}
		public void Initialize(Size proportions, Size margin, int itemMinWidth, decimal borderProportion) {
			Initialize(proportions, margin, itemMinWidth, false, borderProportion);
		}
		public void Initialize(ICollection items, Size proportions, Size margin, int itemMinWidth, decimal borderProportion) {
			Initialize(items);
			Initialize(proportions, margin, itemMinWidth, borderProportion);
		}
		void Initialize(Size proportions, Size margin, int itemMinWidth, bool changed, decimal borderProportion) {
			this.itemMargin = margin;
			this.itemProportionsValue = proportions;
			this.borderProportion = borderProportion;
			InitializeItemProportions();
			this.itemMinSize = new Size(itemMinWidth, (int)(itemMinWidth * itemProportions));
			if(changed)
				RaiseChanged();
		}
		public Size CalculateSize(Size newSize) {
			this.size = newSize;
			Size borderSize = GetBorder(newSize);
			borderSize = new Size(2 * borderSize.Width, 2 * borderSize.Height);
			this.arrangementInfo = CalculateArrangementInfo(newSize.Width - borderSize.Width, newSize.Height - borderSize.Height);
			return this.arrangementInfo.GetAreaSize() + borderSize;
		}
		public int GetHeightByWidth(int width) {
			int itemHeight = Convert.ToInt32(Math.Ceiling((decimal)width * itemProportions / ItemsInLineCount));
			return itemHeight * contentLineCount;
		}
		public int GetWidthByHeight(int height) {
			int itemWidth = Convert.ToInt32(Math.Ceiling((decimal)height / (ItemsInLineCount * itemProportions)));
			return itemWidth * contentLineCount;
		}
		public Size ApplySizeToItems() {
			if(arrangementInfo == null) return new Size(0, 0);
			Size borderSize = GetBorder(size);
			int layoutWidth = size.Width - 2 * borderSize.Width;
			int layoutHeight = size.Height - 2 * borderSize.Height;
			int itemWidth = arrangementInfo.GetWidth(true, layoutWidth);
			int itemHeight = arrangementInfo.GetHeight(true, layoutHeight);
			int index = 0;
			foreach(ISizable sizableItem in items) {
				if(sizableItem != null) {
					sizableItem.Top = arrangementInfo.GetTop(index, layoutHeight) + borderSize.Height;
					sizableItem.Left = arrangementInfo.GetLeft(index, layoutWidth) + borderSize.Width;
					sizableItem.Width = itemWidth;
					sizableItem.Height = itemHeight;
					index++;
				}
			}
			return new Size(arrangementInfo.GetWidth(false, layoutWidth), arrangementInfo.GetHeight(false, layoutHeight));
		}
		ArrangementInfo CalculateArrangementInfo(int width, int height) {
			switch(ContentArrangementMode) {
				case ContentArrangementMode.FixedColumnCount:
					return CreateArrangementInfo(width, ContentLineCount, itemMinSize.Width, itemProportions, PositioningDirection.Horizontal);
				case ContentArrangementMode.FixedRowCount:
					return CreateArrangementInfo(height, ContentLineCount, itemMinSize.Height, 1 / itemProportions, PositioningDirection.Vertical);
				case ContentArrangementMode.Auto:
					if(height < itemMinSize.Height && width / itemMinSize.Width >= items.Count) { 
						return new ArrangementInfo(items.Count, items.Count, itemMinSize.Width, itemMinSize.Height, itemMargin, PositioningDirection.Horizontal, options);
					}
					ArrangementInfo horzInfo = CreateArrangementInfo(width, width / itemMinSize.Width, itemMinSize.Width, itemProportions, PositioningDirection.Horizontal);
					for(int i = horzInfo.ItemsOnRowCount - 1; i >= 1; i--) {
						ArrangementInfo newHorzInfo = CreateArrangementInfo(width, i, itemMinSize.Width, itemProportions, PositioningDirection.Horizontal);
						if(height >= newHorzInfo.ItemsOnColumnCount * newHorzInfo.GetHeight())
							horzInfo = newHorzInfo;
						else
							break;
					}
					ArrangementInfo nextHorzInfo = CreateArrangementInfo(width, horzInfo.ItemsOnRowCount - 1, itemMinSize.Width, itemProportions, PositioningDirection.Horizontal);
					ArrangementInfo vertInfo = CreateArrangementInfo(height, nextHorzInfo.ItemsOnColumnCount, itemMinSize.Height, 1 / itemProportions, PositioningDirection.Vertical);
					int itemHeight = vertInfo.GetHeight();
					int itemWidth = vertInfo.GetWidth();
					int countOnWidth = nextHorzInfo.ItemsOnRowCount;
					if(horzInfo.GetHeight() < itemHeight && width >= countOnWidth * itemWidth)
						horzInfo = new ArrangementInfo(items.Count, countOnWidth, itemWidth, itemHeight, itemMargin, PositioningDirection.Horizontal, options);
					if(height < horzInfo.ItemsOnColumnCount * horzInfo.GetHeight()) {
						vertInfo = CreateArrangementInfo(height, horzInfo.ItemsOnColumnCount, itemMinSize.Height, 1 / itemProportions, PositioningDirection.Vertical);
						itemHeight = vertInfo.GetHeight();
						itemWidth = vertInfo.GetWidth();
						countOnWidth = vertInfo.ItemsOnColumnCount;
						if(height >= vertInfo.ItemsOnRowCount * itemHeight && width >= countOnWidth * itemWidth)
							horzInfo = new ArrangementInfo(items.Count, Math.Min(width / itemWidth, items.Count), itemWidth, itemHeight, itemMargin, PositioningDirection.Horizontal, options);
					}
					return horzInfo;
				default:
					throw new Exception("Undefined arrangement mode");
			}
		}
		ArrangementInfo CreateArrangementInfo(int width, int lineCount, int itemMinWidth, decimal proportions, PositioningDirection direction) {
			if(lineCount < 1)
				lineCount = 1;
			if(items.Count < lineCount)
				lineCount = items.Count;
			int itemWidth = width / lineCount;
			int itemHeight = Convert.ToInt32(itemWidth * proportions);
			if(itemWidth < itemMinWidth) {
				itemWidth = itemMinWidth;
				itemHeight = Convert.ToInt32(itemWidth * proportions);
			}
			return new ArrangementInfo(items.Count, lineCount, itemWidth, itemHeight, itemMargin, direction, options);
		}
		int CalculateBorder(int size) {
			return (int)Math.Ceiling(size * borderProportion);
		}
		Size GetBorder(Size size) {
			if((options & Viewer.ContentArrangementOptions.IgnoreMargins) == Viewer.ContentArrangementOptions.IgnoreMargins)
				return new Size(-itemMargin.Width, -itemMargin.Height);
			return new Size(CalculateBorder(size.Width), CalculateBorder(size.Height)); 
		}
		void InitializeItemProportions() {
			if(itemProportionsValue.Height > 0 && itemProportionsValue.Width > 0)
				itemProportions = (decimal)itemProportionsValue.Height / (decimal)itemProportionsValue.Width;
			else
				itemProportions = DefaultItemProportions;
		}
		void RaiseChanged() {
			if(changed != null)
				changed(this, EventArgs.Empty);
		}
	}
}
