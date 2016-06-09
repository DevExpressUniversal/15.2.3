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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Layout;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region GoToPrevNextPageHeaderFooterCommand (abstract class)
	public abstract class GoToPrevNextPageHeaderFooterCommand : HeaderFooterRelatedMultiCommandBase {
		protected GoToPrevNextPageHeaderFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			PrevNextHeaderFooterInfoCalculatorBase calculateCommand = CreateCalculateNextHeaderFooterInfoCommand();
			calculateCommand.Execute();
			NextHeaderFooterInfo info = calculateCommand.NextHeaderFooterInfo;
			if (info == null || !info.CanGoOrCreate)
				return;
			if (info.CanGo)
				GoToExistingHeaderFooter(info.HeaderFooter, info.Page.PageIndex, info.Section);
			else {
				SectionHeaderFooterBase newHeaderFooter = calculateCommand.CreateNewHeaderFooter(info);
				if (newHeaderFooter != null)
					GoToExistingHeaderFooter(newHeaderFooter, info.Page.PageIndex, info.Section);
			}
		}
		protected internal abstract PrevNextHeaderFooterInfoCalculatorBase CreateCalculateNextHeaderFooterInfoCommand();
	}
	#endregion
	#region GoToNextPageHeaderFooterCommand
	public class GoToNextPageHeaderFooterCommand : GoToPrevNextPageHeaderFooterCommand {
		public GoToNextPageHeaderFooterCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToNextPageHeaderFooterCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToNextHeaderFooter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToNextPageHeaderFooterCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToNextHeaderFooterDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToNextPageHeaderFooterCommandImageName")]
#endif
		public override string ImageName { get { return "GoToNextHeaderFooter"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToNextPageHeaderFooterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.GoToNextHeaderFooter; } }
		protected internal override PrevNextHeaderFooterInfoCalculatorBase CreateCalculateNextHeaderFooterInfoCommand() {
			if (ActivePieceTable.ContentType is SectionHeader)
				return new NextHeaderInfoCalculator(Control);
			else
				return new NextFooterInfoCalculator(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region GoToPreviousPageHeaderFooterCommand
	public class GoToPreviousPageHeaderFooterCommand : GoToPrevNextPageHeaderFooterCommand {
		public GoToPreviousPageHeaderFooterCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPreviousPageHeaderFooterCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPreviousHeaderFooter; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPreviousPageHeaderFooterCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPreviousHeaderFooterDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPreviousPageHeaderFooterCommandImageName")]
#endif
		public override string ImageName { get { return "GoToPreviousHeaderFooter"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("GoToPreviousPageHeaderFooterCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.GoToPreviousHeaderFooter; } }
		protected internal override PrevNextHeaderFooterInfoCalculatorBase CreateCalculateNextHeaderFooterInfoCommand() {
			if (ActivePieceTable.ContentType is SectionHeader)
				return new PrevHeaderInfoCalculator(Control);
			else
				return new PrevFooterInfoCalculator(Control);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region NextHeaderFooterInfo
	public class NextHeaderFooterInfo {
		#region Fields
		readonly SectionHeaderFooterBase headerFooter;
		readonly Page page;
		readonly Section section;
		readonly HeaderFooterType type;
		#endregion
		protected NextHeaderFooterInfo(SectionHeaderFooterBase headerFooter, Page page, Section section, HeaderFooterType type) {
			this.headerFooter = headerFooter;
			this.page = page;
			this.section = section;
			this.type = type;
		}
		#region Properties
		public SectionHeaderFooterBase HeaderFooter { get { return headerFooter; } }
		public Page Page { get { return page; } }
		public Section Section { get { return section; } }
		public HeaderFooterType Type { get { return type; } }
		public bool CanGoOrCreate { get { return Page != null && Section != null; } }
		public bool CanGo { get { return HeaderFooter != null && Page != null && Section != null; } }
		#endregion
		public static NextHeaderFooterInfo CreateNoJump() {
			return new NextHeaderFooterInfo(null, null, null, HeaderFooterType.Odd);
		}
		public static NextHeaderFooterInfo CreateJumpToExisting(SectionHeaderFooterBase headerFooter, Page page, Section section) {
			return new NextHeaderFooterInfo(headerFooter, page, section, headerFooter.Type);
		}
		public static NextHeaderFooterInfo CreateJumpToNonExisting(Page page, Section section, HeaderFooterType type) {
			return new NextHeaderFooterInfo(null, page, section, type);
		}
	}
	#endregion
	#region PrevNextHeaderFooterInfoCalculatorBase (abstract class)
	public abstract class PrevNextHeaderFooterInfoCalculatorBase : RichEditCaretBasedCommand {
		protected PrevNextHeaderFooterInfoCalculatorBase(IRichEditControl control)
			: base(control) {
		}
		public abstract NextHeaderFooterInfo NextHeaderFooterInfo { get; }
		public abstract SectionHeaderFooterBase CreateNewHeaderFooter(NextHeaderFooterInfo info);
	}
	#endregion
	#region PrevNextHeaderFooterInfoCalculator<T> (abstract class)
	public abstract class PrevNextHeaderFooterInfoCalculator<T> : PrevNextHeaderFooterInfoCalculatorBase where T : SectionHeaderFooterBase {
		NextHeaderFooterInfo nextHeaderFooterInfo;
		protected PrevNextHeaderFooterInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override NextHeaderFooterInfo NextHeaderFooterInfo { get { return nextHeaderFooterInfo; } }
		#endregion
		protected internal override void ExecuteCore() {
			UpdateCaretPosition(DocumentLayoutDetailsLevel.PageArea);
			if (!CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.PageArea))
				return;
			this.nextHeaderFooterInfo = CalculateTargetHeaderFooterInfo((T)DocumentModel.ActivePieceTable.ContentType, CaretPosition.LayoutPosition.Page);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.HeadersFooters, state.Enabled);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsHeaderFooter && ActiveViewType == RichEditViewType.PrintLayout;
		}
		protected internal virtual NextHeaderFooterInfo CalculateTargetHeaderFooterInfo(T currentSectionHeaderFooter, Page currentPage) {
			List<HeaderFooterType> searchOrder = CreateSearchOrder(GetSection(currentPage));
			int currentIndex = searchOrder.IndexOf(currentSectionHeaderFooter.Type);
			if (ShouldGoToNextSection(currentIndex, searchOrder.Count))
				return CalculateNextSectionHeaderFooterInfo(currentSectionHeaderFooter, currentPage);
			else {
				HeaderFooterType type = GetNextHeaderFooterType(searchOrder, currentIndex);
				return CalculateNextPageHeaderFooterInfo(currentSectionHeaderFooter, currentPage, type);
			}
		}
		protected internal virtual List<HeaderFooterType> CreateSearchOrder(Section section) {
			List<HeaderFooterType> result = new List<HeaderFooterType>();
			if (section.GeneralSettings.DifferentFirstPage) {
				result.Add(HeaderFooterType.First);
				if (section.DocumentModel.DocumentProperties.DifferentOddAndEvenPages) {
					result.Add(HeaderFooterType.Even);
					result.Add(HeaderFooterType.Odd);
				}
				else
					result.Add(HeaderFooterType.Odd);
			}
			else {
				if (section.DocumentModel.DocumentProperties.DifferentOddAndEvenPages) {
					result.Add(HeaderFooterType.Odd);
					result.Add(HeaderFooterType.Even);
				}
				else
					result.Add(HeaderFooterType.Odd);
			}
			return result;
		}
		public NextHeaderFooterInfo CalculateNextPageHeaderFooterInfoForHeaderFooter(T expectedHeaderFooter, Page currentPage) {
			for (; ; ) {
				currentPage = ObtainNextPage(currentPage);
				if (currentPage == null)
					break;
				PageArea area = GetHeaderFooterPageArea(currentPage);
				if (area == null)
					continue;
				if (Object.ReferenceEquals(expectedHeaderFooter, area.PieceTable.ContentType))
					return NextHeaderFooterInfo.CreateJumpToExisting(expectedHeaderFooter, currentPage, GetSection(currentPage));
			}
			return NextHeaderFooterInfo.CreateNoJump();
		}
		protected internal virtual NextHeaderFooterInfo CalculateNextPageHeaderFooterInfo(T currentSectionHeaderFooter, Page currentPage, HeaderFooterType type) {
			Page nextPage = ObtainNextPage(currentPage);
			if (nextPage == null)
				return NextHeaderFooterInfo.CreateNoJump(); 
			Section section = GetSection(currentPage);
			if (!Object.ReferenceEquals(GetSection(nextPage), section))
				return CalculateNextSectionHeaderFooterInfo(currentSectionHeaderFooter, currentPage);
			PageArea nextHeaderFooterArea = GetHeaderFooterPageArea(nextPage);
			if (nextHeaderFooterArea == null)
				return NextHeaderFooterInfo.CreateJumpToNonExisting(nextPage, section, type); 
			else {
				for (; ; ) { 
					T headerFooter = (T)nextHeaderFooterArea.PieceTable.ContentType;
					if (headerFooter.Type == type)
						break;
					Page page = ObtainNextPage(nextPage);
					if (page == null || !Object.ReferenceEquals(GetSection(page), section)) {
						nextHeaderFooterArea = null;
						break;
					}
					nextPage = page;
					nextHeaderFooterArea = GetHeaderFooterPageArea(nextPage);
					if (nextHeaderFooterArea == null)
						return NextHeaderFooterInfo.CreateNoJump(); 
				}
				if (nextHeaderFooterArea != null)
					return NextHeaderFooterInfo.CreateJumpToExisting((T)nextHeaderFooterArea.PieceTable.ContentType, nextPage, GetSection(nextPage)); 
				else
					return NextHeaderFooterInfo.CreateJumpToNonExisting(nextPage, GetSection(nextPage), type); 
			}
		}
		protected internal virtual NextHeaderFooterInfo GetPageHeaderFooterInfo(Page nextPage) {
			Section section = GetSection(nextPage);
			PageArea nextHeaderFooterArea = GetHeaderFooterPageArea(nextPage);
			if (nextHeaderFooterArea != null)
				return NextHeaderFooterInfo.CreateJumpToExisting((T)nextHeaderFooterArea.PieceTable.ContentType, nextPage, section); 
			else {
				List<HeaderFooterType> searchOrder = CreateSearchOrder(section);
				HeaderFooterType type = CalculateSectionNewHeaderFooterType(nextPage, searchOrder);
				return NextHeaderFooterInfo.CreateJumpToNonExisting(nextPage, section, type); 
			}
		}
		public override SectionHeaderFooterBase CreateNewHeaderFooter(NextHeaderFooterInfo info) {
			InsertPageHeaderFooterCoreCommandBase command = CreateInsertObjectCommand(info.Type);
			SectionHeaderFooterBase result;
			DocumentModel.BeginUpdate();
			try {
				result = command.ModifyModelCore(GetSection(info.Page));
			}
			finally {
				DocumentModel.EndUpdate();
			}
			ActiveView.EnforceFormattingCompleteForVisibleArea();
			return result;
		}
		protected internal abstract bool ShouldGoToNextSection(int currentOrder, int orderCount);
		protected internal abstract Page ObtainNextPage(Page currentPage);
		protected internal abstract Section GetSection(Page page);
		protected internal abstract PageArea GetHeaderFooterPageArea(Page page);
		protected internal abstract NextHeaderFooterInfo CalculateNextSectionHeaderFooterInfo(T currentSectionHeaderFooter, Page currentPage);
		protected internal abstract InsertPageHeaderFooterCoreCommandBase CreateInsertObjectCommand(HeaderFooterType type);
		protected internal abstract HeaderFooterType CalculateSectionNewHeaderFooterType(Page page, List<HeaderFooterType> searchOrder);
		protected internal abstract HeaderFooterType GetNextHeaderFooterType(List<HeaderFooterType> searchOrder, int currentIndex);
	}
	#endregion
	#region PrevHeaderFooterInfoCalculator<T> (abstract class)
	public abstract class PrevHeaderFooterInfoCalculator<T> : PrevNextHeaderFooterInfoCalculator<T> where T : SectionHeaderFooterBase {
		protected PrevHeaderFooterInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ShouldGoToNextSection(int currentOrder, int orderCount) {
			return currentOrder <= 0;
		}
		protected internal override Page ObtainNextPage(Page currentPage) {
			if (currentPage.PageIndex <= 0)
				return null;
			else
				return ActiveView.DocumentLayout.Pages[currentPage.PageIndex - 1];
		}
		protected internal override NextHeaderFooterInfo CalculateNextSectionHeaderFooterInfo(T currentSectionHeaderFooter, Page currentPage) {
			Section section = GetSection(currentPage);
			SectionIndex sectionIndex = DocumentModel.Sections.IndexOf(section);
			if (sectionIndex <= new SectionIndex(0))
				return NextHeaderFooterInfo.CreateNoJump();  
			PageCollection pages = ActiveView.DocumentLayout.Pages;
			for (int i = currentPage.PageIndex - 1; i >= 0; i--) {
				Page nextPage = pages[i];
				if (!Object.ReferenceEquals(GetSection(nextPage), section))
					return GetPageHeaderFooterInfo(nextPage);
			}
			return NextHeaderFooterInfo.CreateNoJump();  
		}
		protected internal override HeaderFooterType GetNextHeaderFooterType(List<HeaderFooterType> searchOrder, int currentIndex) {
			return searchOrder[currentIndex - 1];
		}
		protected internal override HeaderFooterType CalculateSectionNewHeaderFooterType(Page page, List<HeaderFooterType> searchOrder) {
			Section section = GetSection(page);
			int pageCount = 1;
			PageCollection pages = ActiveView.DocumentLayout.Pages;
			for (int i = page.PageIndex - 1; i >= 0 && pageCount < searchOrder.Count; i--) {
				Page nextPage = pages[i];
				if (!Object.ReferenceEquals(GetSection(nextPage), section))
					break;
				else
					pageCount++;
			}
				return searchOrder[pageCount - 1];
		}
	}
	#endregion
	#region NextHeaderFooterInfoCalculator<T> (abstract class)
	public abstract class NextHeaderFooterInfoCalculator<T> : PrevNextHeaderFooterInfoCalculator<T> where T : SectionHeaderFooterBase {
		protected NextHeaderFooterInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ShouldGoToNextSection(int currentOrder, int orderCount) {
			return currentOrder + 1 >= orderCount;
		}
		protected internal override Page ObtainNextPage(Page currentPage) {
			int nextPageIndex = currentPage.PageIndex + 1;
			ActiveView.EnsureFormattingCompleteForPreferredPage(nextPageIndex);
			if (nextPageIndex >= ActiveView.DocumentLayout.Pages.Count)
				return null; 
			else
				return ActiveView.DocumentLayout.Pages[nextPageIndex];
		}
		protected internal override NextHeaderFooterInfo CalculateNextSectionHeaderFooterInfo(T currentSectionHeaderFooter, Page currentPage) {
			Section section = GetSection(currentPage);
			SectionIndex sectionIndex = DocumentModel.Sections.IndexOf(section);
			if (sectionIndex + 1 >= new SectionIndex(DocumentModel.Sections.Count))
				return NextHeaderFooterInfo.CreateNoJump();  
			for (; ; ) {
				Page nextPage = ObtainNextPage(currentPage);
				if (nextPage == null)
					return NextHeaderFooterInfo.CreateNoJump();  
				if (!Object.ReferenceEquals(GetSection(nextPage), section))
					return GetPageHeaderFooterInfo(nextPage);
				else
					currentPage = nextPage;
			}
		}
		protected internal override HeaderFooterType GetNextHeaderFooterType(List<HeaderFooterType> searchOrder, int currentIndex) {
			return searchOrder[currentIndex + 1];
		}
		protected internal override HeaderFooterType CalculateSectionNewHeaderFooterType(Page page, List<HeaderFooterType> searchOrder) {
			return searchOrder[0];
		}
	}
	#endregion
	#region PrevHeaderInfoCalculator
	public class PrevHeaderInfoCalculator : PrevHeaderFooterInfoCalculator<SectionHeader> {
		public PrevHeaderInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override PageArea GetHeaderFooterPageArea(Page page) {
			return page.Header;
		}
		protected internal override Section GetSection(Page page) {
			return page.Areas.First.Section;
		}
		protected internal override InsertPageHeaderFooterCoreCommandBase CreateInsertObjectCommand(HeaderFooterType type) {
			return new InsertPageHeaderCoreCommand(Control, type);
		}
	}
	#endregion
	#region NextHeaderInfoCalculator
	public class NextHeaderInfoCalculator : NextHeaderFooterInfoCalculator<SectionHeader> {
		public NextHeaderInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override PageArea GetHeaderFooterPageArea(Page page) {
			return page.Header;
		}
		protected internal override Section GetSection(Page page) {
			return page.Areas.First.Section;
		}
		protected internal override InsertPageHeaderFooterCoreCommandBase CreateInsertObjectCommand(HeaderFooterType type) {
			return new InsertPageHeaderCoreCommand(Control, type);
		}
	}
	#endregion
	#region PrevFooterInfoCalculator
	public class PrevFooterInfoCalculator : PrevHeaderFooterInfoCalculator<SectionFooter> {
		public PrevFooterInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override PageArea GetHeaderFooterPageArea(Page page) {
			return page.Footer;
		}
		protected internal override Section GetSection(Page page) {
			return page.Areas.Last.Section;
		}
		protected internal override InsertPageHeaderFooterCoreCommandBase CreateInsertObjectCommand(HeaderFooterType type) {
			return new InsertPageFooterCoreCommand(Control, type);
		}
	}
	#endregion
	#region NextFooterInfoCalculator
	public class NextFooterInfoCalculator : NextHeaderFooterInfoCalculator<SectionFooter> {
		public NextFooterInfoCalculator(IRichEditControl control)
			: base(control) {
		}
		protected internal override PageArea GetHeaderFooterPageArea(Page page) {
			return page.Footer;
		}
		protected internal override Section GetSection(Page page) {
			return page.Areas.Last.Section;
		}
		protected internal override InsertPageHeaderFooterCoreCommandBase CreateInsertObjectCommand(HeaderFooterType type) {
			return new InsertPageFooterCoreCommand(Control, type);
		}
	}
	#endregion
	#region MakeNearestHeaderFooterActiveCommand (abstract class)
	public abstract class MakeNearestHeaderFooterActiveCommand<T> : RichEditCaretBasedCommand where T : SectionHeaderFooterBase {
		readonly T targetTable;
		protected MakeNearestHeaderFooterActiveCommand(IRichEditControl control, T targetTable)
			: base(control) {
			this.targetTable = targetTable;
		}
		protected internal T TargetTable { get { return targetTable; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		protected internal override void ExecuteCore() {
			UpdateCaretPosition(DocumentLayoutDetailsLevel.PageArea);
			if (!CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.PageArea))
				return;
			ChangeActivePieceTable(CaretPosition.LayoutPosition);
		}
		protected internal virtual void ChangeActivePieceTable(DocumentLayoutPosition layoutPosition) {
			Page currentPage = layoutPosition.Page;
			PieceTable headerFooter = GetPageHeaderFooter(currentPage);
			if (headerFooter != null && targetTable == (SectionHeaderFooterBase)headerFooter.ContentType)
				MakeHeaderFooterActive(headerFooter, layoutPosition.PageArea.Section, currentPage.PageIndex);
			else if (!TryMakePrevHeaderFooterActive(currentPage))
				MakeNextHeaderFooterActive(currentPage);
		}
		protected internal virtual bool TryMakePrevHeaderFooterActive(Page currentPage) {
			PrevHeaderFooterInfoCalculator<T> calculator = CreatePrevHeaderFooterCalculator();
			NextHeaderFooterInfo info = calculator.CalculateNextPageHeaderFooterInfoForHeaderFooter(targetTable, currentPage);
			if (info != null && info.CanGo) {
				MakeHeaderFooterActive(info.HeaderFooter.PieceTable, info.Section, info.Page.PageIndex);
				return true;
			}
			return false;
		}
		protected internal virtual void MakeNextHeaderFooterActive(Page currentPage) {
			NextHeaderFooterInfoCalculator<T> calculator = CreateNextHeaderFooterCalculator();
			NextHeaderFooterInfo info = calculator.CalculateNextPageHeaderFooterInfoForHeaderFooter(targetTable, currentPage);
			if (info != null && info.CanGo)
				MakeHeaderFooterActive(info.HeaderFooter.PieceTable, info.Section, info.Page.PageIndex);
		}
		protected abstract PrevHeaderFooterInfoCalculator<T> CreatePrevHeaderFooterCalculator();
		protected abstract NextHeaderFooterInfoCalculator<T> CreateNextHeaderFooterCalculator();
		protected abstract PieceTable GetPageHeaderFooter(Page page);
		protected internal virtual void MakeHeaderFooterActive(PieceTable pieceTable, Section section, int pageIndex) {			
			IPieceTableProvider provider = new ExplicitPieceTableProvider(pieceTable, section, pageIndex);
			MakeHeaderFooterActiveCommand command = new MakeHeaderFooterActiveCommand(Control, provider);
			command.Execute();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.HeadersFooters, state.Enabled);
			if (state.Enabled)
				state.Enabled = ActiveViewType == RichEditViewType.PrintLayout;
		}
	}
	#endregion
	#region MakeNearestHeaderActiveCommand
	public class MakeNearestHeaderActiveCommand : MakeNearestHeaderFooterActiveCommand<SectionHeader> {
		public MakeNearestHeaderActiveCommand(IRichEditControl control, SectionHeader targetTable)
			: base(control, targetTable) {
		}
		protected override PrevHeaderFooterInfoCalculator<SectionHeader> CreatePrevHeaderFooterCalculator() {
			return new PrevHeaderInfoCalculator(Control);
		}
		protected override NextHeaderFooterInfoCalculator<SectionHeader> CreateNextHeaderFooterCalculator() {
			return new NextHeaderInfoCalculator(Control);
		}
		protected override PieceTable GetPageHeaderFooter(Page page) {
			return page.Header != null ? page.Header.PieceTable: null;
		}
	}
	#endregion
	#region MakeNearestFooterActiveCommand
	public class MakeNearestFooterActiveCommand : MakeNearestHeaderFooterActiveCommand<SectionFooter> {
		public MakeNearestFooterActiveCommand(IRichEditControl control, SectionFooter targetTable)
			: base(control, targetTable) {
		}
		protected override PrevHeaderFooterInfoCalculator<SectionFooter> CreatePrevHeaderFooterCalculator() {
			return new PrevFooterInfoCalculator(Control);
		}
		protected override NextHeaderFooterInfoCalculator<SectionFooter> CreateNextHeaderFooterCalculator() {
			return new NextFooterInfoCalculator(Control);
		}
		protected override PieceTable GetPageHeaderFooter(Page page) {
			return page.Footer != null ? page.Footer.PieceTable : null;
		}
	}
	#endregion
}
