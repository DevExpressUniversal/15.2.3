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
using System.Collections.Generic;
namespace DevExpress.Web.Internal {
	public class ModelItem {
		public static readonly ModelItem
			Summary = new ModelItem(),
			First = new ModelItem(),
			Prev = new ModelItem(),
			Ellipsis = new ModelItem(),
			Next = new ModelItem(),
			Last = new ModelItem(),
			All = new ModelItem(),
			PageSize = new ModelItem();
	}
	public class ModelNumericItem : ModelItem {
		int value;
		public ModelNumericItem(int value) {
			this.value = value;
		}
		public int Value { get { return value; } }
	}
	public class ModelSection {
		ASPxPagerBase pager;
		public ModelSection(ASPxPagerBase pager) {
			this.pager = pager;
		}
		protected ASPxPagerBase Pager { get { return pager; } }
		protected virtual bool SupportsSummary { get { return true; } }
		protected virtual bool SupportsFirstButton { get { return true; } }
		protected virtual bool SupportsPrevButton { get { return true; } }
		protected virtual bool SupportsNumericButtons { get { return true; } }
		protected virtual bool SupportsNextButton { get { return true; } }
		protected virtual bool SupportsLastButton { get { return true; } }
		protected virtual bool SupportsAllButton { get { return true; } }
		protected virtual bool SupportsPageSize { get { return true; } }
		public bool ShowSummary { get { return SupportsSummary && IsButtonVisible(Pager.Summary); } }
		public bool ShowFirstButton { get { return SupportsFirstButton && IsButtonVisible(Pager.FirstPageButton); } }
		public bool ShowPrevButton { get { return SupportsPrevButton && IsButtonVisible(Pager.PrevPageButton); } }
		public bool ShowNumericButtons { get { return SupportsNumericButtons && Pager.ShowNumericButtons; } }
		public bool ShowLastButton { get { return SupportsLastButton && IsButtonVisible(Pager.LastPageButton); } }
		public bool ShowNextButton { get { return SupportsNextButton && IsButtonVisible(Pager.NextPageButton); } }
		public bool ShowAllButton { get { return SupportsAllButton && IsButtonVisible(Pager.AllButton); } }
		public bool ShowPageSize { get { return SupportsPageSize && IsButtonVisible(Pager.PageSizeItemSettings); } }
		protected bool IsButtonVisible(PagerButtonProperties button) {
			if(!button.Visible)
				return false;
			return Pager.ShowDisabledButtons || !button.IsDisabled(Pager.PageIndex, Pager.PageCount);
		}
		protected bool IsPageSizeVisible {
			get { return IsButtonVisible(Pager.PageSizeItemSettings); }
		}
		protected bool IsPageSizeVisibleLeft {
			get { return IsPageSizeVisible && Pager.PageSizeItemSettings.Position == PagerPageSizePosition.Left; }
		}
		protected bool IsPageSizeVisibleRight {
			get { return IsPageSizeVisible && Pager.PageSizeItemSettings.Position == PagerPageSizePosition.Right; }
		}
		public bool Visible {
			get {
				return ShowSummary || ShowFirstButton || ShowPrevButton || ShowNumericButtons
					|| ShowLastButton || ShowNextButton || ShowAllButton || ShowPageSize;
			}
		}
	}
	public class ModelLeftSection : ModelSection {
		public ModelLeftSection(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override bool SupportsSummary {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight : Pager.Summary.Position == PagerButtonPosition.Left; }
		}
		protected override bool SupportsFirstButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight
					: Pager.Summary.Position == PagerButtonPosition.Right || Pager.Summary.Position == PagerButtonPosition.Inside; }
		}
		protected override bool SupportsPrevButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight
					: Pager.Summary.Position == PagerButtonPosition.Right || Pager.Summary.Position == PagerButtonPosition.Inside; }
		}
		protected override bool SupportsNumericButtons {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight : Pager.Summary.Position == PagerButtonPosition.Right; }
		}
		protected override bool SupportsNextButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight : Pager.Summary.Position == PagerButtonPosition.Right; }
		}
		protected override bool SupportsLastButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight : Pager.Summary.Position == PagerButtonPosition.Right; }
		}
		protected override bool SupportsAllButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight : Pager.Summary.Position == PagerButtonPosition.Right; }
		}
		protected override bool SupportsPageSize {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft : Pager.Summary.Position == PagerButtonPosition.Left; }
		}
	}
	public class ModelMiddleSection : ModelSection {
		public ModelMiddleSection(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override bool SupportsSummary {
			get { return Pager.Summary.Position == PagerButtonPosition.Inside && !IsPageSizeVisible; }
		}
		protected override bool SupportsFirstButton {
			get { return false; }
		}
		protected override bool SupportsPrevButton {
			get { return false; }
		}
		protected override bool SupportsNumericButtons {
			get { return Pager.Summary.Position == PagerButtonPosition.Inside && !IsPageSizeVisible; }
		}
		protected override bool SupportsNextButton {
			get { return false; }
		}
		protected override bool SupportsLastButton {
			get { return false; }
		}
		protected override bool SupportsAllButton {
			get { return false; }
		}
		protected override bool SupportsPageSize {
			get { return false; }
		}
	}
	public class ModelRightSection : ModelSection {
		public ModelRightSection(ASPxPagerBase pager)
			: base(pager) {
		}
		protected override bool SupportsSummary {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft : Pager.Summary.Position == PagerButtonPosition.Right; }
		}
		protected override bool SupportsFirstButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft : Pager.Summary.Position == PagerButtonPosition.Left; }
		}
		protected override bool SupportsPrevButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft : Pager.Summary.Position == PagerButtonPosition.Left; }
		}
		protected override bool SupportsNumericButtons {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft : Pager.Summary.Position == PagerButtonPosition.Left; }
		}
		protected override bool SupportsNextButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft
					: Pager.Summary.Position == PagerButtonPosition.Left || Pager.Summary.Position == PagerButtonPosition.Inside; }
		}
		protected override bool SupportsLastButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft
					: Pager.Summary.Position == PagerButtonPosition.Left || Pager.Summary.Position == PagerButtonPosition.Inside; }
		}
		protected override bool SupportsAllButton {
			get { return IsPageSizeVisible ? IsPageSizeVisibleLeft
					: Pager.Summary.Position == PagerButtonPosition.Left || Pager.Summary.Position == PagerButtonPosition.Inside; }
		}
		protected override bool SupportsPageSize {
			get { return IsPageSizeVisible ? IsPageSizeVisibleRight : Pager.Summary.Position == PagerButtonPosition.Right; }
		}
	}
	public class ModelBuilder {
		ASPxPagerBase pager;
		ModelSection section;
		List<ModelItem> result;
		public ModelBuilder(ASPxPagerBase pager, ModelSection section) {
			this.pager = pager;
			this.section = section;
			this.result = new List<ModelItem>();
		}
		public List<ModelItem> Result { get { return result; } }
		protected ASPxPagerBase Pager { get { return pager; } }
		protected ModelSection Section { get { return section; } }
		public void Build() {
			Result.Clear();
			if(Section.ShowPageSize && Pager.PageSizeItemSettings.Position == PagerPageSizePosition.Left)
				Result.Add(ModelItem.PageSize);
			if(Section.ShowSummary && Pager.Summary.Position == PagerButtonPosition.Left)
				Result.Add(ModelItem.Summary);
			if(Section.ShowFirstButton)
				Result.Add(ModelItem.First);
			if(Section.ShowPrevButton)
				Result.Add(ModelItem.Prev);
			if(Section.ShowSummary && Pager.Summary.Position == PagerButtonPosition.Inside)
				Result.Add(ModelItem.Summary);
			if(Section.ShowNumericButtons) {
				if(Pager.PageCount <= Pager.NumericButtonCount) {
					for(int i = 0; i < Pager.PageCount; i++)
						AddNumber(i);
				} else {
					switch(Pager.EllipsisMode) {
						case PagerEllipsisMode.None:
							AddNumbersWithOutsideEllipsis(false);
							break;
						case PagerEllipsisMode.OutsideNumeric:
							AddNumbersWithOutsideEllipsis(true);
							break;
						case PagerEllipsisMode.InsideNumeric:
							AddNumbersWithInsideEllipsis();
							break;
					}
				}
			}
			if(Section.ShowNextButton)
				Result.Add(ModelItem.Next);
			if(Section.ShowLastButton)
				Result.Add(ModelItem.Last);
			if(Section.ShowAllButton)
				Result.Add(ModelItem.All);
			if(Section.ShowSummary && Pager.Summary.Position == PagerButtonPosition.Right)
				Result.Add(ModelItem.Summary);
			if(Section.ShowPageSize && Pager.PageSizeItemSettings.Position == PagerPageSizePosition.Right)
				Result.Add(ModelItem.PageSize);
		}
		protected void AddNumber(int value) {
			Result.Add(new ModelNumericItem(value));
		}
		protected void AddNumbersWithOutsideEllipsis(bool ellipsis) {
			int pageIndex = Pager.GetNumericButtonsCurrentPageIndex();
			int startIndex = pageIndex - Pager.NumericButtonCount / 2 + (1 - Pager.NumericButtonCount % 2);
			int endIndex = startIndex + Pager.NumericButtonCount;
			if(startIndex < 0) {
				startIndex = 0;
				endIndex = startIndex + Pager.NumericButtonCount;
			}
			if(endIndex > Pager.PageCount) {
				endIndex = Pager.PageCount;
				startIndex = endIndex - Pager.NumericButtonCount;
			}
			if(ellipsis && startIndex > 0)
				Result.Add(ModelItem.Ellipsis);
			for(int i = startIndex; i < endIndex; i++)
				AddNumber(i);
			if(ellipsis && endIndex < Pager.PageCount)
				Result.Add(ModelItem.Ellipsis);
		}
		protected void AddNumbersWithInsideEllipsis() {
			if(Pager.NumericButtonCount < 3) {
				AddNumbersWithOutsideEllipsis(true);
			} else {
				int pageIndex = Pager.GetNumericButtonsCurrentPageIndex();
				int sectionSize = Pager.NumericButtonCount / 3;
				int middleSectionSize = Pager.NumericButtonCount - sectionSize - sectionSize;
				int leftSectionStart = 0;
				int leftSectionEnd = leftSectionStart + sectionSize;
				int middleSectionStart = pageIndex - middleSectionSize / 2 + (1 - middleSectionSize % 2);
				int middleSectionEnd = middleSectionStart + middleSectionSize;
				int rightSectionStart = Pager.PageCount - sectionSize;
				int rightSectionEnd = rightSectionStart + sectionSize;
				bool hasLeftSection = true;
				bool hasRightSection = true;
				if(leftSectionEnd + 1 > middleSectionStart) {
					hasLeftSection = false;
					middleSectionStart = leftSectionStart;
					middleSectionEnd = middleSectionStart + sectionSize + middleSectionSize;
				}
				if(rightSectionStart - 1 < middleSectionEnd) {
					hasRightSection = false;
					middleSectionEnd = rightSectionEnd;
					middleSectionStart = middleSectionEnd - sectionSize - middleSectionSize;
				}
				if(hasLeftSection) {
					for(int i = leftSectionStart; i < leftSectionEnd; i++)
						AddNumber(i);
					Result.Add(ModelItem.Ellipsis);
				}
				for(int i = middleSectionStart; i < middleSectionEnd; i++)
					Result.Add(new ModelNumericItem(i));
				if(hasRightSection) {
					Result.Add(ModelItem.Ellipsis);
					for(int i = rightSectionStart; i < rightSectionEnd; i++)
						AddNumber(i);
				}
			}
		}
	}
}
