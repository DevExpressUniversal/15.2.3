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
using System.Drawing;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardCommon.Viewer {
	enum PositioningDirection { Vertical, Horizontal }
	class ArrangementInfo {
		internal static Exception CreateDirectionException() {
			return new Exception("Undefined direction");
		}
		readonly int itemsOnRowCount;
		readonly int totalItemCount;
		readonly int itemWidth;
		readonly int itemHeight;
		readonly Size itemMargin;
		readonly PositioningDirection direction;
		readonly ContentArrangementOptions options;
		public int ItemsOnRowCount { get { return itemsOnRowCount; } }
		public int ItemsOnColumnCount {
			get { return (int)Math.Ceiling((decimal)totalItemCount / (decimal)itemsOnRowCount); }
		}
		public ArrangementInfo(int totalItemCount, int itemsOnRowCount, int itemWidth, int itemHeight, Size itemMargin,
				PositioningDirection direction, ContentArrangementOptions options) {
			if(itemsOnRowCount <= 0)
				throw new ArgumentException("ItemsOnRowCount cannot be zero or less");
			this.totalItemCount = totalItemCount;
			this.itemsOnRowCount = itemsOnRowCount;
			this.itemWidth = itemWidth;
			this.itemHeight = itemHeight;
			this.direction = direction;
			this.itemMargin = itemMargin;
			this.options = options;
		}
		public int GetTop(int itemIndex, int layoutHeight) {
			int top = itemMargin.Height;
			int externalOffset = GetExternalItemOffset(layoutHeight, direction == PositioningDirection.Horizontal);
			switch(direction) {
				case PositioningDirection.Horizontal:
					return top + (itemIndex / ItemsOnRowCount) * itemHeight + externalOffset;
				case PositioningDirection.Vertical:
					return top + (itemIndex % ItemsOnRowCount) * itemWidth + externalOffset;
				default:
					throw CreateDirectionException();
			}
		}
		public int GetLeft(int itemIndex, int layoutWidth) {
			int left = itemMargin.Width;
			int externalOffset = GetExternalItemOffset(layoutWidth, direction == PositioningDirection.Vertical);
			switch(direction) {
				case PositioningDirection.Horizontal:
					return left + (itemIndex % ItemsOnRowCount) * itemWidth + externalOffset;
				case PositioningDirection.Vertical:
					return left + (itemIndex / ItemsOnRowCount) * itemHeight + externalOffset;
				default:
					throw CreateDirectionException();
			}
		}
		int GetExternalItemOffset(int layoutLength, bool isColumns) {
			return (options & ContentArrangementOptions.AlignCenter) == ContentArrangementOptions.AlignCenter ? GetAreaOffset(layoutLength, isColumns) / 2 : 0;
		}
		int GetAreaOffset(int layoutLength, bool isColumns) {
			if(layoutLength > 0) {
				int alignOffset = isColumns ? ItemsOnColumnCount * itemHeight : ItemsOnRowCount * itemWidth;
				alignOffset = layoutLength - alignOffset;
				return alignOffset > 0 ? alignOffset : 0;
			} else
				return 0;
		}
		public int GetWidth() {
			return GetWidth(false, 0);
		}
		public int GetHeight() {
			return GetHeight(false, 0);
		}
		public int GetWidth(bool useMargin, int layoutWidth) {
			int margin = useMargin ? 2 * itemMargin.Width : 0;
			switch(direction) {
				case PositioningDirection.Horizontal:
					return itemWidth - margin;
				case PositioningDirection.Vertical:
					return itemHeight - margin;
				default:
					throw CreateDirectionException();
			}
		}
		public int GetHeight(bool useMargin, int layoutHeight) {
			int margin = useMargin ? 2 * itemMargin.Height : 0;
			switch(direction) {
				case PositioningDirection.Horizontal:
					return itemHeight - margin;
				case PositioningDirection.Vertical:
					return itemWidth - margin;
				default:
					throw CreateDirectionException();
			}
		}
		public Size GetAreaSize() {
			int rowsLength = itemWidth * itemsOnRowCount;
			int columnsLength = ItemsOnColumnCount * itemHeight;
			switch(direction) {
				case PositioningDirection.Horizontal:
					return new Size(rowsLength, columnsLength);
				case PositioningDirection.Vertical:
					return new Size(columnsLength, rowsLength);
				default:
					return Size.Empty;
			}
		}
	}
}
