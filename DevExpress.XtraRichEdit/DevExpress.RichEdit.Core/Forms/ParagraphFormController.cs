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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Forms {
	#region ParagraphFormControllerParameters
	public class ParagraphFormControllerParameters : FormControllerParameters {
		readonly MergedParagraphProperties paragraphProperties;
		readonly DocumentModelUnitConverter unitConverter;
		internal ParagraphFormControllerParameters(IRichEditControl control, MergedParagraphProperties paragraphProperties, DocumentModelUnitConverter unitConverter)
			: base(control) {
			Guard.ArgumentNotNull(paragraphProperties, "paragraphProperties");
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.paragraphProperties = paragraphProperties;
			this.unitConverter = unitConverter;
		}
		internal MergedParagraphProperties ParagraphProperties { get { return paragraphProperties; } }
		internal DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
	}
	#endregion
	#region ParagraphFormController
	public class ParagraphFormController : FormController {
		#region Fields
		readonly MergedParagraphProperties sourceProperties;
		ParagraphAlignment? alignment;
		int? firstLineIndent;
		ParagraphFirstLineIndent? firstLineIndentType;
		int? leftIndent;
		int? rightIndent;
		int? spacingAfter;
		int? spacingBefore;
		float? lineSpacing;
		ParagraphLineSpacing? lineSpacingType;
		bool? suppressHyphenation;
		bool? suppressLineNumbers;
		int? outlineLevel;
		readonly DocumentModelUnitConverter unitConverter;
		readonly List<string> outlineLevelItems;
		bool? keepLinesTogether;
		bool? pageBreakBefore;
		bool? contextualSpacing;
		bool canEditTabs;
		#endregion
		public ParagraphFormController(ParagraphFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceProperties = controllerParameters.ParagraphProperties;
			this.unitConverter = controllerParameters.UnitConverter;
			this.outlineLevelItems = new List<string>();
			if (controllerParameters.Control.InnerControl != null) {
				DocumentCapabilitiesOptions capabilityOptions = controllerParameters.Control.InnerControl.DocumentModel.DocumentCapabilities;
				this.canEditTabs = capabilityOptions.ParagraphFormattingAllowed && capabilityOptions.ParagraphTabsAllowed;
			}
			else
				this.canEditTabs = true;
			InitializeController();
		}
		#region Properties
		public DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		protected internal MergedParagraphProperties SourceProperties { get { return sourceProperties; } }
		protected ParagraphFormattingOptions SourceOptions { get { return SourceProperties.Options; } }
		protected ParagraphFormattingInfo SourceInfo { get { return SourceProperties.Info; } }
		public ParagraphAlignment? Alignment { get { return alignment; } set { alignment = value; } }
		public int? FirstLineIndent { get { return firstLineIndent; } set { firstLineIndent = value; } }
		public ParagraphFirstLineIndent? FirstLineIndentType { get { return firstLineIndentType; } set { firstLineIndentType = value; } }
		public int? LeftIndent { get { return leftIndent; } set { leftIndent = value; } }
		public int? RightIndent { get { return rightIndent; } set { rightIndent = value; } }
		public int? SpacingAfter { get { return spacingAfter; } set { spacingAfter = value; } }
		public int? SpacingBefore { get { return spacingBefore; } set { spacingBefore = value; } }
		public float? LineSpacing { get { return lineSpacing; } set { lineSpacing = value; } }
		public ParagraphLineSpacing? LineSpacingType { get { return lineSpacingType; } set { lineSpacingType = value; } }
		public bool? SuppressHyphenation { get { return suppressHyphenation; } set { suppressHyphenation = value; } }
		public bool? SuppressLineNumbers { get { return suppressLineNumbers; } set { suppressLineNumbers = value; } }
		public int? OutlineLevel { get { return outlineLevel; } set { outlineLevel = value; } }
		public IList<string> OutlineLevelItems { get { return outlineLevelItems; } }
		public bool? KeepLinesTogether { get { return keepLinesTogether; } set { keepLinesTogether = value; } }
		public bool? PageBreakBefore { get { return pageBreakBefore; } set { pageBreakBefore = value; } }
		public bool? ContextualSpacing { get { return contextualSpacing; } set { contextualSpacing = value; } }
		public bool CanEditTabs { get { return canEditTabs; } }
		#endregion
		Nullable<T> ConvertToNullable<T>(bool use, T value) where T : struct {
			if (use)
				return value;
			return null;
		}
		protected internal virtual void InitializeController() {
			LeftIndent = ConvertToNullable(SourceOptions.UseLeftIndent, SourceInfo.LeftIndent);
			RightIndent = ConvertToNullable(SourceOptions.UseRightIndent, SourceInfo.RightIndent);
			FirstLineIndent = ConvertToNullable(SourceOptions.UseFirstLineIndent, SourceInfo.FirstLineIndent);
			FirstLineIndentType = ConvertToNullable(SourceOptions.UseFirstLineIndent, SourceInfo.FirstLineIndentType);
			Alignment = ConvertToNullable(SourceOptions.UseAlignment, SourceInfo.Alignment);
			SpacingAfter = ConvertToNullable(SourceOptions.UseSpacingAfter, SourceInfo.SpacingAfter);
			SpacingBefore = ConvertToNullable(SourceOptions.UseSpacingBefore, SourceInfo.SpacingBefore);
			LineSpacing = ConvertToNullable(SourceOptions.UseLineSpacing, SourceInfo.LineSpacing);
			LineSpacingType = ConvertToNullable(SourceOptions.UseLineSpacing, SourceInfo.LineSpacingType);
			SuppressHyphenation = ConvertToNullable(SourceOptions.UseSuppressHyphenation, SourceInfo.SuppressHyphenation);
			SuppressLineNumbers = ConvertToNullable(SourceOptions.UseSuppressLineNumbers, SourceInfo.SuppressLineNumbers);
			OutlineLevel = ConvertToNullable(SourceOptions.UseOutlineLevel, SourceInfo.OutlineLevel);
			KeepLinesTogether = ConvertToNullable(SourceOptions.UseKeepLinesTogether, SourceInfo.KeepLinesTogether);
			PageBreakBefore = ConvertToNullable(SourceOptions.UsePageBreakBefore, SourceInfo.PageBreakBefore);
			ContextualSpacing = ConvertToNullable(SourceOptions.UseContextualSpacing, SourceInfo.ContextualSpacing);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevelBody);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel1);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel2);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel3);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel4);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel5);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel6);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel7);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel8);
			AddOutlineLevelItem(XtraRichEditStringId.Caption_OutlineLevel9);		   
		}
		protected internal virtual void AddOutlineLevelItem(XtraRichEditStringId stringId) {
			outlineLevelItems.Add(XtraRichEditLocalizer.GetString(stringId));
		}
		public override void ApplyChanges() {
			ApplyLeftIndent();
			ApplyRightIndent();
			ApplyFirstLineIndent();
			ApplyAlignment();
			ApplySpacingAfter();
			ApplySpacingBefore();
			ApplyLineSpacing();
			ApplySuppressHyphenation();
			ApplySuppressLineNumbers();
			ApplyOutlineLevel();
			ApplyKeepLinesTogether();
			ApplyPageBreakBefore();
			ApplyContextualSpacing();
		}
		protected internal virtual void ApplyRightIndent() {
			SourceOptions.UseRightIndent = (RightIndent != null) && (SourceInfo.RightIndent != RightIndent || !SourceOptions.UseRightIndent);
			if (SourceOptions.UseRightIndent)
				SourceInfo.RightIndent = RightIndent.Value;
		}
		protected internal virtual void ApplyLeftIndent() {
			SourceOptions.UseLeftIndent = (LeftIndent != null) && (SourceInfo.LeftIndent != LeftIndent || !SourceOptions.UseLeftIndent);
			if (SourceOptions.UseLeftIndent)
				SourceInfo.LeftIndent = LeftIndent.Value;
		}
		protected internal virtual void ApplyFirstLineIndent() {
			SourceOptions.UseFirstLineIndent = (FirstLineIndentType != null) && (FirstLineIndent != null || firstLineIndentType == ParagraphFirstLineIndent.None) && ((SourceInfo.FirstLineIndent != FirstLineIndent) || (SourceInfo.FirstLineIndentType != FirstLineIndentType) || !SourceOptions.UseLeftIndent);
			if (SourceOptions.UseFirstLineIndent) {
				if (FirstLineIndent != null)
					SourceInfo.FirstLineIndent = FirstLineIndent.Value;
				SourceInfo.FirstLineIndentType = FirstLineIndentType.Value;
			}
		}
		protected internal virtual void ApplyAlignment() {
			SourceOptions.UseAlignment = (Alignment != null) && (SourceInfo.Alignment != Alignment || !SourceOptions.UseAlignment);
			if (SourceOptions.UseAlignment)
				SourceInfo.Alignment = Alignment.Value;
		}
		protected internal virtual void ApplySpacingAfter() {
			SourceOptions.UseSpacingAfter = (SpacingAfter != null) && (SourceInfo.SpacingAfter != SpacingAfter || !SourceOptions.UseSpacingAfter);
			if (SourceOptions.UseSpacingAfter)
				SourceInfo.SpacingAfter = SpacingAfter.Value;
		}
		protected internal virtual void ApplySpacingBefore() {
			SourceOptions.UseSpacingBefore = (SpacingBefore != null) && (SourceInfo.SpacingBefore != SpacingBefore || !SourceOptions.UseSpacingBefore);
			if (SourceOptions.UseSpacingBefore)
				SourceInfo.SpacingBefore = SpacingBefore.Value;
		}
		protected internal virtual void ApplyContextualSpacing() {
			SourceOptions.UseContextualSpacing = (ContextualSpacing != null) && (SourceInfo.ContextualSpacing != ContextualSpacing || !SourceOptions.UseContextualSpacing);
			if (SourceOptions.UseContextualSpacing)
				SourceInfo.ContextualSpacing = ContextualSpacing.Value;
		}
		protected internal virtual void ApplyLineSpacing() {
			SourceOptions.UseLineSpacing = (LineSpacingType != null) && ((SourceInfo.LineSpacing != LineSpacing) || (SourceInfo.LineSpacingType != LineSpacingType) || !SourceOptions.UseLeftIndent);
			if (SourceOptions.UseLineSpacing) {
				if (LineSpacing != null)
						SourceInfo.LineSpacing = LineSpacing.Value;
				SourceInfo.LineSpacingType = LineSpacingType.Value;
			}
		}
		protected internal virtual void ApplySuppressHyphenation() {
			SourceOptions.UseSuppressHyphenation = (SuppressHyphenation != null) && (SourceInfo.SuppressHyphenation != SuppressHyphenation || !SourceOptions.UseSuppressHyphenation);
			if (SourceOptions.UseSuppressHyphenation)
				SourceInfo.SuppressHyphenation = SuppressHyphenation.Value;
		}
		protected internal virtual void ApplySuppressLineNumbers() {
			SourceOptions.UseSuppressLineNumbers = (SuppressLineNumbers != null) && (SourceInfo.SuppressLineNumbers != SuppressLineNumbers || !SourceOptions.UseSuppressLineNumbers);
			if (SourceOptions.UseSuppressLineNumbers)
				SourceInfo.SuppressLineNumbers = SuppressLineNumbers.Value;
		}
		protected internal virtual void ApplyOutlineLevel() {
			SourceOptions.UseOutlineLevel = (OutlineLevel != null) && (SourceInfo.OutlineLevel != OutlineLevel || !SourceOptions.UseOutlineLevel);
			if (SourceOptions.UseOutlineLevel)
				SourceInfo.OutlineLevel = OutlineLevel.Value;
		}
		protected internal virtual void ApplyKeepLinesTogether() {
			SourceOptions.UseKeepLinesTogether = (KeepLinesTogether != null) && (SourceInfo.KeepLinesTogether != KeepLinesTogether || !SourceOptions.UseKeepLinesTogether);
			if (SourceOptions.UseKeepLinesTogether)
				SourceInfo.KeepLinesTogether = KeepLinesTogether.Value;
		}
		protected internal virtual void ApplyPageBreakBefore() {
			SourceOptions.UsePageBreakBefore = (PageBreakBefore != null) && (SourceInfo.PageBreakBefore != PageBreakBefore || !SourceOptions.UsePageBreakBefore);
			if (SourceOptions.UsePageBreakBefore)
				SourceInfo.PageBreakBefore = PageBreakBefore.Value;
		}
	}
	#endregion
}
