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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region HeaderFooterLayout
	public class HeaderFooterLayout {
		#region Fields
		readonly HeaderFooterOptions options;
		readonly HeaderFooterLayoutItem firstHeader;
		readonly HeaderFooterLayoutItem firstFooter;
		readonly HeaderFooterLayoutItem evenHeader;
		readonly HeaderFooterLayoutItem evenFooter;
		readonly HeaderFooterLayoutItem oddHeader;
		readonly HeaderFooterLayoutItem oddFooter;
		Rectangle totalBounds;
		#endregion
		public HeaderFooterLayout(Rectangle bounds, HeaderFooterOptions options, IHeaderFooterFormatTagProvider formatTagProvider) {
			Guard.ArgumentNotNull(options, "options");
			this.totalBounds = bounds;
			this.options = options;
			this.firstHeader = new HeaderFooterLayoutItem(options.FirstHeader, formatTagProvider);
			this.firstFooter = new HeaderFooterLayoutItem(options.FirstFooter, formatTagProvider);
			this.evenHeader = new HeaderFooterLayoutItem(options.EvenHeader, formatTagProvider);
			this.evenFooter = new HeaderFooterLayoutItem(options.EvenFooter, formatTagProvider);
			this.oddHeader = new HeaderFooterLayoutItem(options.OddHeader, formatTagProvider);
			this.oddFooter = new HeaderFooterLayoutItem(options.OddFooter, formatTagProvider);
		}
		#region Properties
		public Rectangle TotalBounds { get { return totalBounds; } }
		public HeaderFooterLayoutItem FirstHeader { get { return firstHeader; } }
		public HeaderFooterLayoutItem FirstFooter { get { return firstFooter; } }
		public HeaderFooterLayoutItem EvenHeader { get { return evenHeader; } }
		public HeaderFooterLayoutItem EvenFooter { get { return evenFooter; } }
		public HeaderFooterLayoutItem OddHeader { get { return oddHeader; } }
		public HeaderFooterLayoutItem OddFooter { get { return oddFooter; } }
		#endregion
		public HeaderFooterLayoutItem GetActualHeader(int pageIndex) {
			return GetActualItem(pageIndex, firstHeader, evenHeader, oddHeader);
		}
		public HeaderFooterLayoutItem GetActualFooter(int pageIndex) {
			return GetActualItem(pageIndex, firstFooter, evenFooter, oddFooter);
		}
		HeaderFooterLayoutItem GetActualItem(int pageIndex, HeaderFooterLayoutItem first, HeaderFooterLayoutItem even, HeaderFooterLayoutItem odd) {
			return GetActualItemCore(pageIndex, first, even, odd);
		}
		HeaderFooterLayoutItem GetActualItemCore(int pageIndex, HeaderFooterLayoutItem first, HeaderFooterLayoutItem even, HeaderFooterLayoutItem odd) {
			if (pageIndex == 0 && options.DifferentFirst)
				return first;
			else if (options.DifferentOddEven)
				return pageIndex % 2 == 0 ? odd : even;
			return odd;
		}
		public void UpdateTotalPageTag(int totalPages) {
			firstHeader.UpdateTotalPageTag(totalPages);
			evenHeader.UpdateTotalPageTag(totalPages);
			oddHeader.UpdateTotalPageTag(totalPages);
			firstFooter.UpdateTotalPageTag(totalPages);
			evenFooter.UpdateTotalPageTag(totalPages);
			oddFooter.UpdateTotalPageTag(totalPages);
		}
	}
	#endregion
	#region HeaderFooterLayoutItem
	public class HeaderFooterLayoutItem {
		#region Fields
		List<HeaderFooterTextBox> leftTextBoxes;
		List<HeaderFooterTextBox> centerTextBoxes;
		List<HeaderFooterTextBox> rightTextBoxes;
		#endregion
		public HeaderFooterLayoutItem(string content, IHeaderFooterFormatTagProvider formatTagProvider) {
			HeaderFooterBuilder builder = new HeaderFooterBuilder(content, true, formatTagProvider);
			if (!String.IsNullOrEmpty(builder.Left)) {
				this.leftTextBoxes = new List<HeaderFooterTextBox>();
				this.leftTextBoxes.Add(new HeaderFooterTextBox(Rectangle.Empty, builder.Left));
			}
			if (!String.IsNullOrEmpty(builder.Center)) {
				this.centerTextBoxes = new List<HeaderFooterTextBox>();
				this.centerTextBoxes.Add(new HeaderFooterTextBox(Rectangle.Empty, builder.Center));
			}
			if (!String.IsNullOrEmpty(builder.Right)) {
				this.rightTextBoxes = new List<HeaderFooterTextBox>();
				this.rightTextBoxes.Add(new HeaderFooterTextBox(Rectangle.Empty, builder.Right));
			}
		}
		#region Properties
		public List<HeaderFooterTextBox> Left { get { return leftTextBoxes; } }
		public List<HeaderFooterTextBox> Center { get { return centerTextBoxes; } }
		public List<HeaderFooterTextBox> Right { get { return rightTextBoxes; } }
		#endregion
		public string GetLeftPlainText() {
			return GetPlainText(Left);
		}
		public string GetFormattedLeftPlainText(int currentPageIndex) {
			return GetPlainText(Left, currentPageIndex);
		}
		public string GetCenterPlainText() {
			return GetPlainText(Center);
		}
		public string GetFormattedCenterPlainText(int currentPageIndex) {
			return GetPlainText(Center, currentPageIndex);
		}
		public string GetRightPlainText() {
			return GetPlainText(Right);
		}
		public string GetFormattedRightPlainText(int currentPageIndex) {
			return GetPlainText(Right, currentPageIndex);
		}
		string GetPlainText(List<HeaderFooterTextBox> boxes) {
			if (boxes == null || boxes.Count == 0)
				return String.Empty;
			StringBuilder builder = new StringBuilder();
			foreach (HeaderFooterTextBox box in boxes)
				builder.Append(box.Text);
			return builder.ToString();
		}
		string GetPlainText(List<HeaderFooterTextBox> boxes, int currentPageIndex) {
			string text = GetPlainText(boxes);
			return text.Replace("&P", currentPageIndex.ToString());
		}
		public void UpdateTotalPageTag(int totalPagesCount) {
			UpdateTotalPageTagCore(Left, totalPagesCount);
			UpdateTotalPageTagCore(Center, totalPagesCount);
			UpdateTotalPageTagCore(Right, totalPagesCount);
		}
		void UpdateTotalPageTagCore(List<HeaderFooterTextBox> boxes, int totalPagesCount) {
			if (boxes == null)
				return;
			foreach (HeaderFooterTextBox box in boxes)
				box.UpdateTotalPageTag(totalPagesCount);
		}
	}
	#endregion
	#region HeaderFooterTextBox
	public class HeaderFooterTextBox {
		#region Fields
		Rectangle bounds;
		string text;
		#endregion
		public HeaderFooterTextBox(Rectangle bounds, string text) {
			Guard.ArgumentIsNotNullOrEmpty(text, "text");
			this.bounds = bounds;
			this.text = text;
		}
		#region Properties
		public Rectangle Bounds { get { return bounds; } }
		public string Text { get { return text; } }
		#endregion
		protected internal void UpdateTotalPageTag(int totalPagesCount) {
			this.text = text.Replace("&N", totalPagesCount.ToString());
		}
	}
	#endregion
}
