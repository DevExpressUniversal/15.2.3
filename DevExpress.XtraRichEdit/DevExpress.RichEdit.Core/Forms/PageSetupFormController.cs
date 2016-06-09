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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !DXPORTABLE
using System.Drawing.Printing;
using PaperKindType = System.Drawing.Printing.PaperKind;
#else
using DevExpress.Compatibility.System.Drawing.Printing;
using PaperKindType = DevExpress.Compatibility.System.Drawing.Printing.PaperKind;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	#region PageSetupFormInitialTabPage
	public enum PageSetupFormInitialTabPage {
		Default = 0,
		Margins = 0,
		Paper = 1,
		Layout = 2,
	}
	#endregion
	#region PageSetupFormControllerParameters
	public class PageSetupFormControllerParameters : FormControllerParameters {
		readonly PageSetupInfo pageSetupInfo;
		PageSetupFormInitialTabPage initialTabPage;
		public PageSetupFormControllerParameters(IRichEditControl control, PageSetupInfo pageSetupInfo)
			: base(control) {
			Guard.ArgumentNotNull(pageSetupInfo, "pageSetupInfo");
			this.pageSetupInfo = pageSetupInfo;
		}
		public PageSetupInfo PageSetupInfo { get { return pageSetupInfo; } }
		public PageSetupFormInitialTabPage InitialTabPage { get { return initialTabPage; } set { initialTabPage = value; } }
	}
	#endregion
	#region PageSetupInfo
	public class PageSetupInfo : ICloneable<PageSetupInfo>, ISupportsCopyFrom<PageSetupInfo> {
		SectionPropertiesApplyType applyType;
		SectionPropertiesApplyType availableApplyType;
		int? leftMargin;
		int? rightMargin;
		int? topMargin;
		int? bottomMargin;
		int? paperWidth;
		int? paperHeight;
		PaperKind? paperKind;
		bool? landscape;
		SectionStartType? sectionStartType;
		bool? differentFirstPage;
		bool? differentOddAndEvenPages;
		public PageSetupInfo() {
			this.AvailableApplyType = SectionPropertiesApplyType.CurrentSection | SectionPropertiesApplyType.SelectedSections | SectionPropertiesApplyType.WholeDocument;
		}
		public SectionPropertiesApplyType AvailableApplyType { get { return availableApplyType; } set { availableApplyType = value; } }
		public SectionPropertiesApplyType ApplyType { get { return applyType; } set { applyType = value; } }
		public int? LeftMargin { get { return leftMargin; } set { leftMargin = value; } }
		public int? RightMargin { get { return rightMargin; } set { rightMargin = value; } }
		public int? TopMargin { get { return topMargin; } set { topMargin = value; } }
		public int? BottomMargin { get { return bottomMargin; } set { bottomMargin = value; } }
		public int? PaperWidth { get { return paperWidth; } set { paperWidth = value; } }
		public int? PaperHeight { get { return paperHeight; } set { paperHeight = value; } }
		public PaperKind? PaperKind { get { return paperKind; } set { paperKind = value; } }
		public bool? Landscape { get { return landscape; } set { landscape = value; } }
		public SectionStartType? SectionStartType { get { return sectionStartType; } set { sectionStartType = value; } }
		public bool? DifferentFirstPage { get { return differentFirstPage; } set { differentFirstPage = value; } }
		public bool? DifferentOddAndEvenPages { get { return differentOddAndEvenPages; } set { differentOddAndEvenPages = value; } }
		#region ICloneable<PageSetupInfo> Members
		public PageSetupInfo Clone() {
			PageSetupInfo clone = new PageSetupInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<PageSetupInfo> Members
		public void CopyFrom(PageSetupInfo value) {
			this.ApplyType = value.ApplyType;
			this.AvailableApplyType = value.AvailableApplyType;
			this.LeftMargin = value.LeftMargin;
			this.RightMargin = value.RightMargin;
			this.TopMargin = value.TopMargin;
			this.BottomMargin = value.BottomMargin;
			this.PaperWidth = value.PaperWidth;
			this.PaperHeight = value.PaperHeight;
			this.PaperKind = value.PaperKind;
			this.Landscape = value.Landscape;
			this.SectionStartType = value.SectionStartType;
			this.DifferentFirstPage = value.DifferentFirstPage;
			this.DifferentOddAndEvenPages = value.DifferentOddAndEvenPages;
		}
		#endregion
	}
	#endregion
	#region PageSetupFormController
	public class PageSetupFormController : FormController {
		readonly IRichEditControl control;
		readonly PageSetupInfo sourcePageSetupInfo;
		readonly PageSetupInfo pageSetupInfo;
		DocumentModelUnitConverter valueUnitConverter;
		int customWidth;
		int customHeight;
		public PageSetupFormController(PageSetupFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.sourcePageSetupInfo = controllerParameters.PageSetupInfo;
			this.pageSetupInfo = sourcePageSetupInfo.Clone();
			DocumentModelUnitConverter unitConverter = control.InnerControl.DocumentModel.UnitConverter;
			this.customWidth = PaperWidth.HasValue ? PaperWidth.Value : unitConverter.TwipsToModelUnits(PaperSizeCalculator.CalculatePaperSize(PaperKindType.Letter).Width);
			this.customHeight = PaperHeight.HasValue ? PaperHeight.Value : unitConverter.TwipsToModelUnits(PaperSizeCalculator.CalculatePaperSize(PaperKindType.Letter).Height);
			this.valueUnitConverter = unitConverter; 
		}
		#region Properties
		public IRichEditControl Control { get { return control; } }
		public PageSetupInfo SourcePageSetupInfo { get { return sourcePageSetupInfo; } }
		public DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } set { valueUnitConverter = value; } }
		public SectionPropertiesApplyType AvailableApplyType { get { return pageSetupInfo.AvailableApplyType; } }
		public SectionPropertiesApplyType ApplyType { get { return pageSetupInfo.ApplyType; } set { pageSetupInfo.ApplyType = value; } }
		public int? LeftMargin { get { return pageSetupInfo.LeftMargin; } set { pageSetupInfo.LeftMargin = value; } }
		public int? RightMargin { get { return pageSetupInfo.RightMargin; } set { pageSetupInfo.RightMargin = value; } }
		public int? TopMargin { get { return pageSetupInfo.TopMargin; } set { pageSetupInfo.TopMargin = value; } }
		public int? BottomMargin { get { return pageSetupInfo.BottomMargin; } set { pageSetupInfo.BottomMargin = value; } }
		public int? PaperWidth { get { return pageSetupInfo.PaperWidth; } set { pageSetupInfo.PaperWidth = value; } }
		public int? PaperHeight { get { return pageSetupInfo.PaperHeight; } set { pageSetupInfo.PaperHeight = value; } }
		public bool? DifferentFirstPage { get { return pageSetupInfo.DifferentFirstPage; } set { pageSetupInfo.DifferentFirstPage = value; } }
		public bool? DifferentOddAndEvenPages { get { return pageSetupInfo.DifferentOddAndEvenPages; } set { pageSetupInfo.DifferentOddAndEvenPages = value; } }
		public SectionStartType? SectionStartType { get { return pageSetupInfo.SectionStartType; } set { pageSetupInfo.SectionStartType = value; } }
		public int CustomWidth { get { return customWidth; } set { customWidth = value; } }
		public int CustomHeight { get { return customHeight; } set { customHeight = value; } }
		public IList<PaperKind> FullPaperKindList { get { return ChangeSectionPaperKindCommand.FullPaperKindList; } }
		#region PaperKind
		public PaperKind? PaperKind {
			get { return pageSetupInfo.PaperKind; }
			set {
				if (PaperKind == value)
					return;
				pageSetupInfo.PaperKind = value;
				Size size;
				if (PaperKind.HasValue && PaperKind.Value != PaperKindType.Custom) {
					DocumentModelUnitConverter unitConverter = control.InnerControl.DocumentModel.UnitConverter;
					size = unitConverter.TwipsToModelUnits(PaperSizeCalculator.CalculatePaperSize(PaperKind.Value));
					if (Landscape.HasValue && Landscape.Value) {
						int temp = size.Width;
						size.Width = size.Height;
						size.Height = temp;
					}
				}
				else
					size = CalculateCustomSize();
				PaperWidth = size.Width;
				PaperHeight = size.Height;
			}
		}
		#endregion
		#region Landscape
		public bool? Landscape {
			get { return pageSetupInfo.Landscape; }
			set {
				if (Landscape == value)
					return;
				if (Landscape.HasValue) {
					pageSetupInfo.Landscape = value;
					ChangeOrientation();
				}
				else
					pageSetupInfo.Landscape = value;
			}
		}
		#endregion
		#endregion
		Size CalculateCustomSize() {
			if (Landscape.HasValue && Landscape.Value)
				return new Size(CustomHeight, CustomWidth);
			else
				return new Size(CustomWidth, CustomHeight);
		}
		protected internal virtual void ChangeOrientation() {
			if (!Landscape.HasValue)
				return;
			int customValue = CustomWidth;
			CustomWidth = CustomHeight;
			CustomHeight = customValue;
			int? value = PaperWidth;
			PaperWidth = PaperHeight;
			PaperHeight = value;
			int? left = LeftMargin;
			int? right = RightMargin;
			int? top = TopMargin;
			int? bottom = BottomMargin;
			if (Landscape.Value) {
				LeftMargin = top;
				RightMargin = bottom;
				TopMargin = right;
				BottomMargin = left;
			}
			else {
				LeftMargin = bottom;
				RightMargin = top;
				TopMargin = left;
				BottomMargin = right;
			}
		}
		public virtual void UpdatePaperKind() {
			if (!PaperWidth.HasValue)
				return;
			if (!PaperHeight.HasValue)
				return;
			DocumentModelUnitConverter unitConverter = control.InnerControl.DocumentModel.UnitConverter;
			Size paperSize;
			if (Landscape.HasValue && Landscape.Value)
				paperSize = new Size(PaperHeight.Value, PaperWidth.Value);
			else
				paperSize = new Size(PaperWidth.Value, PaperHeight.Value);
			Size size = unitConverter.ModelUnitsToTwips(paperSize);
			pageSetupInfo.PaperKind = PaperSizeCalculator.CalculatePaperKind(size, PaperKindType.Custom);
		}
		public override void ApplyChanges() {
			sourcePageSetupInfo.CopyFrom(pageSetupInfo);
		}
		public bool IsTopBottomMarginsValid() {
			int topMargin = TopMargin ?? 0;
			int bottomMargin = BottomMargin ?? 0;
			int paperHeight = PaperHeight ?? 0;
			return (topMargin + bottomMargin) < paperHeight;
		}
		public bool IsLeftRightMarginsValid() {
			int leftMargin = LeftMargin ?? 0;
			int rightMargin = RightMargin ?? 0;
			int paperWidth = PaperWidth ?? 0;
			return (leftMargin + rightMargin) < paperWidth;
		}
	}
	#endregion
	#region SectionPropertiesApplyType
	[Flags]
	public enum SectionPropertiesApplyType {
		WholeDocument = 0,
		CurrentSection = 1,
		SelectedSections = 2,
		ThisPointForward = 4
	}
	#endregion
	#region SectionPropertiesApplyTypeListBoxItem
	public class SectionPropertiesApplyTypeListBoxItem {
		static readonly Dictionary<SectionPropertiesApplyType, XtraRichEditStringId> stringIdTable = CreateStringIdTable();
		readonly SectionPropertiesApplyType applyType;
		public SectionPropertiesApplyTypeListBoxItem(SectionPropertiesApplyType applyType) {
			this.applyType = applyType;
		}
		public SectionPropertiesApplyType ApplyType { get { return applyType; } }
		XtraRichEditStringId StringId { get { return stringIdTable[ApplyType]; } }
		static Dictionary<SectionPropertiesApplyType, XtraRichEditStringId> CreateStringIdTable() {
			Dictionary<SectionPropertiesApplyType, XtraRichEditStringId> result = new Dictionary<SectionPropertiesApplyType, XtraRichEditStringId>();
			result.Add(SectionPropertiesApplyType.WholeDocument, XtraRichEditStringId.Caption_SectionPropertiesApplyToWholeDocument);
			result.Add(SectionPropertiesApplyType.CurrentSection, XtraRichEditStringId.Caption_SectionPropertiesApplyToCurrentSection);
			result.Add(SectionPropertiesApplyType.SelectedSections, XtraRichEditStringId.Caption_SectionPropertiesApplyToSelectedSections);
			result.Add(SectionPropertiesApplyType.ThisPointForward, XtraRichEditStringId.Caption_SectionPropertiesApplyThisPointForward);
			return result;
		}
		public override string ToString() {
			return XtraRichEditLocalizer.GetString(StringId);
		}
	}
	#endregion
	#region PageSetupSectionStartListBoxItem
	public class PageSetupSectionStartListBoxItem {
		static readonly Dictionary<SectionStartType, XtraRichEditStringId> stringIdTable = CreateStringIdTable();
		readonly SectionStartType sectionStartType;
		public PageSetupSectionStartListBoxItem(SectionStartType sectionStartType) {
			this.sectionStartType = sectionStartType;
		}
		public SectionStartType SectionStartType { get { return sectionStartType; } }
		XtraRichEditStringId StringId { get { return stringIdTable[SectionStartType]; } }
		static Dictionary<SectionStartType, XtraRichEditStringId> CreateStringIdTable() {
			Dictionary<SectionStartType, XtraRichEditStringId> result = new Dictionary<SectionStartType, XtraRichEditStringId>();
			result.Add(SectionStartType.Continuous, XtraRichEditStringId.Caption_PageSetupSectionStartContinuous);
			result.Add(SectionStartType.Column, XtraRichEditStringId.Caption_PageSetupSectionStartColumn);
			result.Add(SectionStartType.NextPage, XtraRichEditStringId.Caption_PageSetupSectionStartNextPage);
			result.Add(SectionStartType.OddPage, XtraRichEditStringId.Caption_PageSetupSectionStartOddPage);
			result.Add(SectionStartType.EvenPage, XtraRichEditStringId.Caption_PageSetupSectionStartEvenPage);
			return result;
		}
		public override string ToString() {
			return XtraRichEditLocalizer.GetString(StringId);
		}
	}
	#endregion
	#region PageSetupFormDefaults
	public static class PageSetupFormDefaults {
		public const int MinTopAndBottomMarginByDefault = -31680; 
		public const int MaxTopAndBottomMarginByDefault = 31680; 
		public const int MinLeftAndRightMarginByDefault = 0;
		public const int MaxLeftAndRightMarginByDefault = 31680; 
		public const int MinPaperWidthAndHeightByDefault = 144; 
		public const int MaxPaperWidthAndHeightByDefault = 31680; 
	}
	#endregion
}
