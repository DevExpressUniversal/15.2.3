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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeHeadingLevelCommandBase (abstract class)
	public abstract class ChangeHeadingLevelCommandBase : ChangeParagraphStyleCommandBase {
		protected ChangeHeadingLevelCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ChangeParagraphProperty(Paragraph paragraph) {
			int level = CalculateHeadingLevel(paragraph);
			if (level >= 0)
				base.ChangeParagraphProperty(paragraph); 
			else {
				level = CalculateOutlineLevel(paragraph);
				if (level >= 0 && level <= 9)
					paragraph.ParagraphProperties.OutlineLevel = level;
			}
		}
		protected internal override int CalculateParagraphStyleIndex(Paragraph paragraph) {
			int level = CalculateHeadingLevel(paragraph);
			if (level < 0)
				return paragraph.ParagraphStyleIndex; 
			if (level == 0)
				return DocumentModel.ParagraphStyles.DefaultItemIndex;
			return DocumentModel.GetHeadingParagraphStyle(level);
		}
		protected internal abstract int CalculateHeadingLevel(Paragraph paragraph);
		protected internal abstract int CalculateOutlineLevel(Paragraph paragraph);
	}
	#endregion
	#region IncrementParagraphOutlineLevelCommand
	public class IncrementParagraphOutlineLevelCommand : ChangeHeadingLevelCommandBase {
		public IncrementParagraphOutlineLevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementParagraphOutlineLevelCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementParagraphOutlineLevel; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementParagraphOutlineLevelCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementParagraphOutlineLevelDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("IncrementParagraphOutlineLevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.IncrementParagraphOutlineLevel; } }
		#endregion
		protected internal override int CalculateHeadingLevel(Paragraph paragraph) {
			int level = paragraph.ParagraphProperties.OutlineLevel;
			if (level != 0)
				return -1; 
			level = paragraph.ParagraphStyle.ParagraphProperties.OutlineLevel;
			if (level == 0)
				return 1;
			return Math.Min(9, Math.Max(level + 1, 1));
		}
		protected internal override int CalculateOutlineLevel(Paragraph paragraph) {
			int level = paragraph.ParagraphProperties.OutlineLevel;
			return Math.Min(9, Math.Max(level + 1, 0));
		}
	}
	#endregion
	#region DecrementParagraphOutlineLevelCommand
	public class DecrementParagraphOutlineLevelCommand : ChangeHeadingLevelCommandBase {
		public DecrementParagraphOutlineLevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementParagraphOutlineLevelCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementParagraphOutlineLevel; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementParagraphOutlineLevelCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DecrementParagraphOutlineLevelDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DecrementParagraphOutlineLevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.DecrementParagraphOutlineLevel; } }
		#endregion
		protected internal override int CalculateHeadingLevel(Paragraph paragraph) {
			int level = paragraph.ParagraphProperties.OutlineLevel;
			if (level != 0)
				return -1; 
			level = paragraph.ParagraphStyle.ParagraphProperties.OutlineLevel;
			if (level == 0)
				return 1;
			return Math.Min(9, Math.Max(level - 1, 1));
		}
		protected internal override int CalculateOutlineLevel(Paragraph paragraph) {
			int level = paragraph.ParagraphProperties.OutlineLevel;
			return Math.Min(9, Math.Max(level - 1, 0));
		}
	}
	#endregion
	#region SetParagraphHeadingLevelCommandBase (abstract class)
	public abstract class SetParagraphHeadingLevelCommandBase : ChangeHeadingLevelCommandBase {
		protected SetParagraphHeadingLevelCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetParagraphHeadingLevel; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetParagraphHeadingLevelDescription; } }
		public override string MenuCaption { get { return String.Format(XtraRichEditLocalizer.GetString(MenuCaptionStringId), Level); } }
		public override string Description { get { return String.Format(XtraRichEditLocalizer.GetString(DescriptionStringId), Level); } }
		protected internal abstract int Level { get; }
		#endregion
		protected internal override int CalculateHeadingLevel(Paragraph paragraph) {
			return Level;
		}
		protected internal override int CalculateOutlineLevel(Paragraph paragraph) {
			return Level;
		}
	}
	#endregion
	#region SetParagraphHeading1LevelCommand
	public class SetParagraphHeading1LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading1LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading1LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading1Level; } }
		protected internal override int Level { get { return 1; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading2LevelCommand
	public class SetParagraphHeading2LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading2LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading2LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading2Level; } }
		protected internal override int Level { get { return 2; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading3LevelCommand
	public class SetParagraphHeading3LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading3LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading3LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading3Level; } }
		protected internal override int Level { get { return 3; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading4LevelCommand
	public class SetParagraphHeading4LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading4LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading4LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading4Level; } }
		protected internal override int Level { get { return 4; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading5LevelCommand
	public class SetParagraphHeading5LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading5LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading5LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading5Level; } }
		protected internal override int Level { get { return 5; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading6LevelCommand
	public class SetParagraphHeading6LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading6LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading6LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading6Level; } }
		protected internal override int Level { get { return 6; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading7LevelCommand
	public class SetParagraphHeading7LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading7LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading7LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading7Level; } }
		protected internal override int Level { get { return 7; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading8LevelCommand
	public class SetParagraphHeading8LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading8LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading8LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading8Level; } }
		protected internal override int Level { get { return 8; } }
		#endregion
	}
	#endregion
	#region SetParagraphHeading9LevelCommand
	public class SetParagraphHeading9LevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphHeading9LevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphHeading9LevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphHeading9Level; } }
		protected internal override int Level { get { return 9; } }
		#endregion
	}
	#endregion
	#region SetParagraphBodyTextLevelCommand
	public class SetParagraphBodyTextLevelCommand : SetParagraphHeadingLevelCommandBase {
		public SetParagraphBodyTextLevelCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphBodyTextLevelCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetParagraphBodyTextLevel; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphBodyTextLevelCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetParagraphBodyTextLevelDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphBodyTextLevelCommandMenuCaption")]
#endif
		public override string MenuCaption { get { return XtraRichEditLocalizer.GetString(MenuCaptionStringId); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphBodyTextLevelCommandDescription")]
#endif
		public override string Description { get { return XtraRichEditLocalizer.GetString(DescriptionStringId); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetParagraphBodyTextLevelCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetParagraphBodyTextLevel; } }
		protected internal override int Level { get { return 0; } }
		#endregion
	}
	#endregion
	#region AddParagraphsToTableOfContentsCommand
	public class AddParagraphsToTableOfContentsCommand : RichEditMenuItemSimpleCommand {
		public AddParagraphsToTableOfContentsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_AddParagraphsToTableOfContents; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_AddParagraphsToTableOfContentsDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.AddParagraphsToTableOfContents; } }
		public override string ImageName { get { return "AddParagraphToTableOfContents"; } }
		#endregion
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
			else {
				state.Enabled = IsContentEditable;
				state.Visible = true;
				ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphStyle, state.Enabled);
				ApplyDocumentProtectionToSelectedParagraphs(state);
			}
		}
	}
	#endregion
}
