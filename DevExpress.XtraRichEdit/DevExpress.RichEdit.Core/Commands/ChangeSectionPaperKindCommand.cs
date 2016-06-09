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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Compatibility.System.Drawing.Printing;
#if SL
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils;
#else
using System.Drawing.Printing;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeSectionPaperKindCommand
	public class ChangeSectionPaperKindCommand : ToggleChangeSectionFormattingCommandBase<PaperKind> {
		static readonly List<PaperKind> defaultPaperKindList = CreateDefaultPaperKindList();
		static readonly List<PaperKind> fullPaperKindList = CreateFullPaperKindList();
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeSectionPaperKindCommandDefaultPaperKindList")]
#endif
		public static IList<PaperKind> DefaultPaperKindList { get { return defaultPaperKindList; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeSectionPaperKindCommandFullPaperKindList")]
#endif
		public static IList<PaperKind> FullPaperKindList { get { return fullPaperKindList; } }
		static List<PaperKind> CreateDefaultPaperKindList() {
			List<PaperKind> result = new List<PaperKind>();
			result.Add(PaperKind.Letter);
			result.Add(PaperKind.Legal);
			result.Add(PaperKind.Folio);
			result.Add(PaperKind.A4);
			result.Add(PaperKind.B5);
			result.Add(PaperKind.Executive);
			result.Add(PaperKind.A5);
			result.Add(PaperKind.A6);
			return result;
		}
		static List<PaperKind> CreateFullPaperKindList() {
			List<PaperKind> result = new List<PaperKind>();
#if SL
			foreach (PaperKind paperKind in EnumExtensions.GetValues(typeof(PaperKind)))
#else
			foreach (PaperKind paperKind in Enum.GetValues(typeof(PaperKind)))
#endif
				if (paperKind != PaperKind.Custom)
					result.Add(paperKind);
			return result;
		}
		PaperKind paperKind = PaperKind.Letter;
		public ChangeSectionPaperKindCommand(IRichEditControl control)
			: base(control) {
		}
		public ChangeSectionPaperKindCommand(IRichEditControl control, PaperKind paperKind)
			: base(control) {
			this.paperKind = paperKind;
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeSectionPaperKindCommandPaperKind")]
#endif
		public PaperKind PaperKind { get { return paperKind; } set { paperKind = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeSectionPaperKindCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKind; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeSectionPaperKindCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKindDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeSectionPaperKindCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeSectionPaperKind; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<PaperKind> valueBasedState = state as IValueBasedCommandUIState<PaperKind>;
			if (valueBasedState != null) {
				if (valueBasedState.Value != PaperKind.Custom)
					PaperKind = valueBasedState.Value;
			}
			base.ForceExecute(state);
		}
		protected internal override SectionPropertyModifier<PaperKind> CreateModifier(ICommandUIState state) {
			return new SectionPaperKindAndSizeModifier(PaperKind);
		}
		protected internal override bool IsCheckedValue(PaperKind value) {
			return value.Equals(PaperKind);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<PaperKind> result = new DefaultValueBasedCommandUIState<PaperKind>();
			result.Value = PaperKind;
			return result;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeSectionPaperKindPlaceholderCommand
	public class ChangeSectionPaperKindPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public ChangeSectionPaperKindPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKind; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeSectionPagePaperKindDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeSectionPaperKindPlaceholder; } }
		public override string ImageName { get { return "PaperSize"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Sections);
			ApplyDocumentProtectionToSelectedSections(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
}
