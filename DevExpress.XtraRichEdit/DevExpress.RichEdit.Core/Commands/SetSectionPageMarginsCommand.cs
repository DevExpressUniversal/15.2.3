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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
namespace DevExpress.XtraRichEdit.Commands {
	#region SetPredefinedSectionPageMarginsCommand (abstract class)
	public abstract class SetPredefinedSectionPageMarginsCommand : ToggleChangeSectionFormattingCommandBase<PageMargins> {
		readonly PageMargins predefinedValue;
		protected SetPredefinedSectionPageMarginsCommand(IRichEditControl control)
			: base(control) {
			this.predefinedValue = CreatePredefinedValue();
		}
		#region Properties
		protected internal PageMargins PredefinedValue { get { return predefinedValue; } }
		public override string MenuCaption {
			get {
				DocumentUnit unit = DocumentServer.UIUnit;
				string format = XtraRichEditLocalizer.GetString(MenuCaptionStringId);
				UIUnitConverter unitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
				return String.Format(format,
					unitConverter.ToUIUnit(PredefinedValue.Left, unit).ToString(),
					unitConverter.ToUIUnit(PredefinedValue.Top, unit).ToString(),
					unitConverter.ToUIUnit(PredefinedValue.Right, unit).ToString(),
					unitConverter.ToUIUnit(PredefinedValue.Bottom, unit).ToString());
			}
		}
		#endregion
		protected internal override SectionPropertyModifier<PageMargins> CreateModifier(ICommandUIState state) {
			return new SectionPageMarginsModifier(PredefinedValue);
		}
		protected internal override bool IsCheckedValue(PageMargins value) {
			return value.Equals(PredefinedValue);
		}
		protected internal abstract PageMargins CreatePredefinedValue();
	}
	#endregion
	#region SetNormalSectionPageMarginsCommand
	public class SetNormalSectionPageMarginsCommand : SetPredefinedSectionPageMarginsCommand {
		public SetNormalSectionPageMarginsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNormalSectionPageMarginsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetNormalSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNormalSectionPageMarginsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetNormalSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNormalSectionPageMarginsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetNormalSectionPageMarginsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNormalSectionPageMarginsCommandImageName")]
#endif
		public override string ImageName { get { return "PageMarginsNormal"; } }
		#endregion
		protected internal override PageMargins CreatePredefinedValue() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			PageMargins result = new PageMargins();
			result.Left = unitConverter.HundredthsOfMillimeterToModelUnits(3000);
			result.Top = unitConverter.HundredthsOfMillimeterToModelUnits(2000);
			result.Bottom = unitConverter.HundredthsOfMillimeterToModelUnits(2000);
			result.Right = unitConverter.HundredthsOfMillimeterToModelUnits(1500);
			return result;
		}
	}
	#endregion
	#region SetNarrowSectionPageMarginsCommand
	public class SetNarrowSectionPageMarginsCommand : SetPredefinedSectionPageMarginsCommand {
		public SetNarrowSectionPageMarginsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNarrowSectionPageMarginsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetNarrowSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNarrowSectionPageMarginsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetNarrowSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNarrowSectionPageMarginsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetNarrowSectionPageMarginsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetNarrowSectionPageMarginsCommandImageName")]
#endif
		public override string ImageName { get { return "PageMarginsNarrow"; } }
		#endregion
		protected internal override PageMargins CreatePredefinedValue() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			PageMargins result = new PageMargins();
			result.Left = unitConverter.DocumentsToModelUnits(150);
			result.Top = unitConverter.DocumentsToModelUnits(150);
			result.Bottom = unitConverter.DocumentsToModelUnits(150);
			result.Right = unitConverter.DocumentsToModelUnits(150);
			return result;
		}
	}
	#endregion
	#region SetModerateSectionPageMarginsCommand
	public class SetModerateSectionPageMarginsCommand : SetPredefinedSectionPageMarginsCommand {
		public SetModerateSectionPageMarginsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetModerateSectionPageMarginsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetModerateSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetModerateSectionPageMarginsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetModerateSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetModerateSectionPageMarginsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetModerateSectionPageMarginsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetModerateSectionPageMarginsCommandImageName")]
#endif
		public override string ImageName { get { return "PageMarginsModerate"; } }
		#endregion
		protected internal override PageMargins CreatePredefinedValue() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			PageMargins result = new PageMargins();
			result.Left = unitConverter.DocumentsToModelUnits(225);
			result.Top = unitConverter.DocumentsToModelUnits(300);
			result.Bottom = unitConverter.DocumentsToModelUnits(300);
			result.Right = unitConverter.DocumentsToModelUnits(225);
			return result;
		}
	}
	#endregion
	#region SetWideSectionPageMarginsCommand
	public class SetWideSectionPageMarginsCommand : SetPredefinedSectionPageMarginsCommand {
		public SetWideSectionPageMarginsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetWideSectionPageMarginsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetWideSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetWideSectionPageMarginsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetWideSectionPageMargins; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetWideSectionPageMarginsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetWideSectionPageMarginsDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetWideSectionPageMarginsCommandImageName")]
#endif
		public override string ImageName { get { return "PageMarginsWide"; } }
		#endregion
		protected internal override PageMargins CreatePredefinedValue() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			PageMargins result = new PageMargins();
			result.Left = unitConverter.DocumentsToModelUnits(600);
			result.Top = unitConverter.DocumentsToModelUnits(300);
			result.Bottom = unitConverter.DocumentsToModelUnits(300);
			result.Right = unitConverter.DocumentsToModelUnits(600);
			return result;
		}
	}
	#endregion
}
