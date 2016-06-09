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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region IPageAreaController
	public interface IPageAreaController {
		void Reset(Section section);
		void BeginSectionFormatting(Section section, int currentColumnsCount);
		void RestartFormattingFromTheStartOfSection(Section section, int currentAreaIndex);
		void RestartFormattingFromTheMiddleOfSection(Section section, int currentAreaIndex);
		void ClearInvalidatedContent(FormatterPosition pos);
		CompleteFormattingResult CompleteCurrentAreaFormatting();
		PageArea GetNextPageArea(bool keepFloatingObjects);
	}
	#endregion
	#region PageAreaController
	public class PageAreaController : IPageAreaController {
		#region Fields
		readonly PageController pageController;
		PageAreaControllerState state;
		#endregion
		public PageAreaController(PageController pageController) {
			Guard.ArgumentNotNull(pageController, "pageController");
			this.pageController = pageController;
			SwitchToState(CreateDefaultState(0));
		}
		#region Properties
		public PageController PageController { get { return pageController; } }
		public virtual PageAreaCollection Areas { get { return pageController.Pages.Last.Areas; } }
		public PageAreaControllerState State { get { return state; } }
		public virtual Rectangle CurrentAreaBounds { get { return State.CurrentAreaBounds; } }
		#endregion
		#region Events
		#endregion
		protected internal virtual PageAreaControllerState CreateDefaultState(int currentAreaIndex) {
			return new DefaultPageAreaControllerState(this, currentAreaIndex);
		}
		public virtual CompleteFormattingResult CompleteCurrentAreaFormatting() {
			return State.CompleteCurrentAreaFormatting();
		}
		public virtual PageArea GetNextPageArea(bool keepFloatingObjects) {
			return State.GetNextPageArea(keepFloatingObjects);
		}
		public virtual void Reset(Section section) {
			State.Reset(section);
		}
		public virtual void BeginSectionFormatting(Section section, int currentColumnsCount) {
			State.BeginSectionFormatting(section, currentColumnsCount);
		}
		public virtual void RestartFormattingFromTheStartOfSection(Section section, int currentAreaIndex) {
			State.RestartFormattingFromTheStartOfSection(section, currentAreaIndex);
		}
		public virtual void RestartFormattingFromTheMiddleOfSection(Section section, int currentAreaIndex) {
			State.RestartFormattingFromTheMiddleOfSection(section, currentAreaIndex);
		}
		protected internal virtual void RestartFormattingFromTheStartOfRowAtCurrentPage() {
			State.RestartFormattingFromTheStartOfRowAtCurrentPage();
		}
		public virtual void ClearInvalidatedContent(FormatterPosition pos) {
			State.ClearInvalidatedContent(pos);
		}
		public virtual void SwitchToState(PageAreaControllerState state) {
			this.state = state;
		}
		public virtual void RemoveLastPageArea() {
			this.Areas.RemoveAt(Areas.Count - 1);
			if (this.Areas.Count == 0)
				PageController.RemoveLastPage();
		}
	}
	#endregion
	#region PageAreaControllerState (abstract class)
	public abstract class PageAreaControllerState : IPageAreaController {
		#region Fields
		readonly PageAreaController owner;
		Rectangle currentAreaBounds;
		#endregion
		protected PageAreaControllerState(PageAreaController owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		#region Properties
		public PageAreaController Owner { get { return owner; } }
		public Rectangle CurrentAreaBounds { get { return currentAreaBounds; } }
		public PageController PageController { get { return Owner.PageController; } }
		public virtual PageAreaCollection Areas { get { return Owner.Areas; } }
		#endregion
		public abstract CompleteFormattingResult CompleteCurrentAreaFormatting();
		public abstract PageArea GetNextPageArea(bool keepFloatingObjects);
		public virtual void Reset(Section section) {
			BeginSectionFormatting(section, 0);
		}
		public virtual void BeginSectionFormatting(Section section, int currentColumnsCount) {
			ApplySectionStart(section, currentColumnsCount);
			CreateCurrentAreaBounds();
		}
		public virtual void RestartFormattingFromTheStartOfSection(Section section, int currentAreaIndex) {
			Debug.Assert(currentAreaIndex >= 0);
			CreateCurrentAreaBounds();
		}
		public virtual void RestartFormattingFromTheMiddleOfSection(Section section, int currentAreaIndex) {
			Debug.Assert(currentAreaIndex >= 0);
			CreateCurrentAreaBounds();
		}
		protected internal virtual void RestartFormattingFromTheStartOfRowAtCurrentPage() {
		}
		public void ClearInvalidatedContent(FormatterPosition pos) {
			PageAreaCollection areas = Areas;
			int areaIndex = areas.BinarySearchBoxIndex(pos);
			if (areaIndex < 0) {
				areaIndex = ~areaIndex;
			}
			if (areaIndex + 1 < areas.Count)
				areas.RemoveRange(areaIndex + 1, areas.Count - areaIndex - 1);
		}
		protected internal virtual PageArea GetNextPageAreaCore() {
			PageArea result = new PageArea(PageController.PieceTable.ContentType, PageController.CurrentSection);
			result.Bounds = CurrentAreaBounds;
			return result;
		}
		protected internal virtual void CreateCurrentAreaBounds() {
			this.currentAreaBounds = CreateCurrentAreaBoundsCore();
		}
		protected internal virtual void RestoreCurrentAreaBounds(Rectangle oldBounds) {
			this.currentAreaBounds = oldBounds;
		}
		protected internal abstract Rectangle CreateCurrentAreaBoundsCore();
		protected internal abstract void ApplySectionStart(Section section, int currentColumnsCount);
	}
	#endregion
	#region DefaultPageAreaControllerState
	public class DefaultPageAreaControllerState : PageAreaControllerState {
		int nextAreaIndex;
		public DefaultPageAreaControllerState(PageAreaController owner, int nextAreaIndex)
			: base(owner) {
			this.nextAreaIndex = nextAreaIndex;
		}
		public override CompleteFormattingResult CompleteCurrentAreaFormatting() {
			if (nextAreaIndex == 0)
				return PageController.CompleteCurrentPageFormatting();
			else {
				Rectangle bounds = CurrentAreaBounds;
				CreateCurrentAreaBounds();
				int newBoundsHeight = CurrentAreaBounds.Height;
				RestoreCurrentAreaBounds(bounds);
				if (newBoundsHeight <= 0) {
					CompleteFormattingResult result = PageController.CompleteCurrentPageFormatting();
					if (result != CompleteFormattingResult.Success)
						return result;
				}
			}
			return CompleteFormattingResult.Success;
		}
		public override PageArea GetNextPageArea(bool keepFloatingObjects) {
			if (nextAreaIndex == 0) {
				PageController.GetNextPage(keepFloatingObjects);
				CreateCurrentAreaBounds();
			}
			else {
				CreateCurrentAreaBounds();
				if (CurrentAreaBounds.Height <= 0) {
					nextAreaIndex = 0;
					PageController.GetNextPage(keepFloatingObjects);
					CreateCurrentAreaBounds();
				}
			}
			PageArea newPageArea = GetNextPageAreaCore();
			Areas.Add(newPageArea);
			return newPageArea;
		}
		public override void BeginSectionFormatting(Section section, int currentColumnsCount) {
			base.BeginSectionFormatting(section, currentColumnsCount);
		}
		public override void RestartFormattingFromTheStartOfSection(Section section, int currentAreaIndex) {
			Debug.Assert(currentAreaIndex >= 0);
			nextAreaIndex = currentAreaIndex;
			base.RestartFormattingFromTheStartOfSection(section, currentAreaIndex);
		}
		public override void RestartFormattingFromTheMiddleOfSection(Section section, int currentAreaIndex) {
			Debug.Assert(currentAreaIndex >= 0);
			nextAreaIndex = currentAreaIndex;
			base.RestartFormattingFromTheMiddleOfSection(section, currentAreaIndex);
		}
		protected internal override Rectangle CreateCurrentAreaBoundsCore() {
			if (nextAreaIndex == 0)
				return PageController.PageClientBounds;
			else {
				Debug.Assert(Areas.Count > 0);
				Rectangle result = PageController.PageClientBounds;
				int lastAreaBottom = Areas[nextAreaIndex].Bounds.Bottom;
				result.Height = result.Bottom - lastAreaBottom;
				result.Y = lastAreaBottom;
				return result;
			}
		}
		protected internal override void ApplySectionStart(Section section, int currentColumnsCount) {
			switch (section.GeneralSettings.StartType) {
				case SectionStartType.Continuous:
					ApplyContinuousSectionStart(section);
					break;
				case SectionStartType.Column:
					if (section.GetActualColumnsCount() != currentColumnsCount)
						nextAreaIndex = 0;
					break;
				case SectionStartType.EvenPage:
				case SectionStartType.OddPage:
				case SectionStartType.NextPage:
					nextAreaIndex = 0;
					break;
				default:
					break;
			}
		}
		protected internal virtual void ApplyContinuousSectionStart(Section section) {
			nextAreaIndex = 0;
		}
	}
	#endregion
}
