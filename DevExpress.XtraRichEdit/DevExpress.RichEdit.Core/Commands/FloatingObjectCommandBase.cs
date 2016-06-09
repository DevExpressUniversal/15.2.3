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
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region FloatingObjectCommandBase (abstract class)
	public abstract class FloatingObjectCommandBase : SelectionBasedPropertyChangeCommandBase {
		protected FloatingObjectCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && DocumentModel.Selection.IsFloatingObjectSelected();
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected DocumentModelChangeActions ChangeFloatingObjectProperty(FloatingObjectRunPropertyModifierBase modifier, DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			if (end.LogPosition - start.LogPosition != 1)
				return DocumentModelChangeActions.None;
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[start.RunIndex] as FloatingObjectAnchorRun;
			if (run == null)
				return DocumentModelChangeActions.None;
			modifier.ModifyFloatingObjectRun(run, start.RunIndex);
			return DocumentModelChangeActions.None;
		}
	}
	#endregion
	#region ChangeFloatingObjectPropertyCommandBase<T> (abstract class)
	public abstract class ChangeFloatingObjectPropertyCommandBase<T> : FloatingObjectCommandBase {
		protected ChangeFloatingObjectPropertyCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			FloatingObjectRunPropertyModifier<T> modifier = CreateModifier(state);
			return ChangeFloatingObjectProperty(modifier, start, end, state);
		}
		protected internal virtual bool GetCurrentPropertyValue(out T value) {
			FloatingObjectRunPropertyModifier<T> modifier = CreateModifier(CreateDefaultCommandUIState());
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
			if (run != null) {
				value = modifier.GetFloatingObjectValue(run);
				return true;
			}
			else {
				value = default(T);
				return false;
			}
		}
		protected internal abstract FloatingObjectRunPropertyModifier<T> CreateModifier(ICommandUIState state);
	}
	#endregion
	#region ChangeFloatingObjectTextWrapTypeCommandBase (abstract class)
	public abstract class ChangeFloatingObjectTextWrapTypeCommandBase : ChangeFloatingObjectPropertyCommandBase<FloatingObjectTextWrapType> {
		protected ChangeFloatingObjectTextWrapTypeCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract FloatingObjectTextWrapType TextWrapType { get; }
		protected internal override FloatingObjectRunPropertyModifier<FloatingObjectTextWrapType> CreateModifier(ICommandUIState state) {
			return new FloatingObjectRunTextWrapTypeModifier(TextWrapType);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = IsChecked(state);
		}
		protected virtual bool IsChecked(ICommandUIState state) {
			if (!state.Enabled)
				return false;
			FloatingObjectRunPropertyModifier<FloatingObjectTextWrapType> modifier = CreateModifier(state);
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
			if (run == null)
				return false;
			return (modifier.GetFloatingObjectValue(run) == TextWrapType);
		}
	}
	#endregion
	#region SetFloatingObjectSquareTextWrapTypeCommand
	public class SetFloatingObjectSquareTextWrapTypeCommand : ChangeFloatingObjectTextWrapTypeCommandBase {
		public SetFloatingObjectSquareTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectSquareTextWrapTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectSquareTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectSquareTextWrapTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectSquareTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectSquareTextWrapTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectSquareTextWrapTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectSquareTextWrapTypeCommandImageName")]
#endif
		public override string ImageName { get { return "TextWrapSquare"; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectBehindTextWrapTypeCommand
	public class SetFloatingObjectBehindTextWrapTypeCommand : ChangeFloatingObjectPropertyCommandBase<bool> {
		public SetFloatingObjectBehindTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBehindTextWrapTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectBehindTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBehindTextWrapTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBehindTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBehindTextWrapTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBehindTextWrapTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBehindTextWrapTypeCommandImageName")]
#endif
		public override string ImageName { get { return "TextWrapBehind"; } }
		protected internal bool IsBehindDoc { get { return true; } }
		#endregion
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			FloatingObjectRunPropertyModifier<bool> modifier = CreateModifier(state);
			return ChangeFloatingObjectProperty(modifier, start, end, state);
		}
		protected internal override FloatingObjectRunPropertyModifier<bool> CreateModifier(ICommandUIState state) {
			return new FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier(IsBehindDoc);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = IsChecked(state);
		}
		protected virtual bool IsChecked(ICommandUIState state) {
			if (!state.Enabled)
				return false;
			FloatingObjectRunPropertyModifier<bool> modifier = CreateModifier(state);
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
			if (run == null)
				return false;
			return (modifier.GetFloatingObjectValue(run) == true) && (run.FloatingObjectProperties.TextWrapType == FloatingObjectTextWrapType.None);
		}
	}
	#endregion
	#region SetFloatingObjectInFrontOfTextWrapTypeCommand
	public class SetFloatingObjectInFrontOfTextWrapTypeCommand : ChangeFloatingObjectPropertyCommandBase<bool> {
		public SetFloatingObjectInFrontOfTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectInFrontOfTextWrapTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectInFrontOfTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectInFrontOfTextWrapTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectInFrontOfTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectInFrontOfTextWrapTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectInFrontOfTextWrapTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectInFrontOfTextWrapTypeCommandImageName")]
#endif
		public override string ImageName { get { return "TextWrapInFrontOfText"; } }
		protected internal bool IsBehindDoc { get { return false; } }
		#endregion
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			FloatingObjectRunPropertyModifier<bool> modifier = CreateModifier(state);
			return ChangeFloatingObjectProperty(modifier, start, end, state);
		}
		protected internal override FloatingObjectRunPropertyModifier<bool> CreateModifier(ICommandUIState state) {
			return new FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier(IsBehindDoc);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = IsChecked(state);
		}
		protected virtual bool IsChecked(ICommandUIState state) {
			if (!state.Enabled)
				return false;
			FloatingObjectRunPropertyModifier<bool> modifier = CreateModifier(state);
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
			if (run == null)
				return false;
			return (modifier.GetFloatingObjectValue(run) == false) &&(run.FloatingObjectProperties.TextWrapType == FloatingObjectTextWrapType.None);
		}
	}
	#endregion
	#region SetFloatingObjectThroughTextWrapTypeCommand
	public class SetFloatingObjectThroughTextWrapTypeCommand : ChangeFloatingObjectTextWrapTypeCommandBase {
		public SetFloatingObjectThroughTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectThroughTextWrapTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectThroughTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectThroughTextWrapTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectThroughTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectThroughTextWrapTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectThroughTextWrapTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectThroughTextWrapTypeCommandImageName")]
#endif
		public override string ImageName { get { return "TextWrapThrough"; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Through; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectTightTextWrapTypeCommand
	public class SetFloatingObjectTightTextWrapTypeCommand : ChangeFloatingObjectTextWrapTypeCommandBase {
		public SetFloatingObjectTightTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTightTextWrapTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectTightTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTightTextWrapTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTightTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTightTextWrapTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTightTextWrapTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTightTextWrapTypeCommandImageName")]
#endif
		public override string ImageName { get { return "TextWrapTight"; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Tight; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectTopAndBottomTextWrapTypeCommand
	public class SetFloatingObjectTopAndBottomTextWrapTypeCommand : ChangeFloatingObjectTextWrapTypeCommandBase {
		public SetFloatingObjectTopAndBottomTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopAndBottomTextWrapTypeCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectTopAndBottomTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopAndBottomTextWrapTypeCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopAndBottomTextWrapType; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopAndBottomTextWrapTypeCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopAndBottomTextWrapTypeDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopAndBottomTextWrapTypeCommandImageName")]
#endif
		public override string ImageName { get { return "TextWrapTopAndBottom"; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.TopAndBottom; } }
		#endregion
	}
	#endregion
	#region ChangeFloatingObjectAlignmentCommand (abstract class)
	public abstract class ChangeFloatingObjectAlignmentCommandBase : FloatingObjectCommandBase {
		protected ChangeFloatingObjectAlignmentCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get; }
		protected internal abstract FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get; }
		protected internal abstract FloatingObjectHorizontalPositionType HorizontalPositionType { get; }
		protected internal abstract FloatingObjectVerticalPositionType VerticalPositionType { get; }
		protected internal abstract FloatingObjectTextWrapType TextWrapType { get; }
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			DocumentModelChangeActions result = DocumentModelChangeActions.None;
			result |= ChangeFloatingObjectProperty(new FloatingObjectRunHorizontalPositionAlignmentModifier(HorizontalPositionAlignment), start, end, state);
			result |= ChangeFloatingObjectProperty(new FloatingObjectRunVerticalPositionAlignmentModifier(VerticalPositionAlignment), start, end, state);
			result |= ChangeFloatingObjectProperty(new FloatingObjectRunHorizontalPositionTypeModifier(HorizontalPositionType), start, end, state);
			result |= ChangeFloatingObjectProperty(new FloatingObjectRunVerticalPositionTypeModifier(VerticalPositionType), start, end, state);
			return result;
		}
	}
	#endregion
	#region SetFloatingObjectTopLeftAlignmentCommand
	public class SetFloatingObjectTopLeftAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectTopLeftAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Left; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Top; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopLeftAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopLeftAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopLeftAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopLeftAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopLeftAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectTopLeftAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopLeftAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectTopLeft"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectTopCenterAlignmentCommand
	public class SetFloatingObjectTopCenterAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectTopCenterAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Center; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Top; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopCenterAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopCenterAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopCenterAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopCenterAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopCenterAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectTopCenterAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopCenterAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectTopCenter"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectTopRightAlignmentCommand
	public class SetFloatingObjectTopRightAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectTopRightAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Right; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Top; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopRightAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopRightAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopRightAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectTopRightAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopRightAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectTopRightAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectTopRightAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectTopRight"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectMiddleLeftAlignmentCommand
	public class SetFloatingObjectMiddleLeftAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectMiddleLeftAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Left; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Center; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleLeftAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleLeftAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleLeftAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleLeftAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleLeftAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectMiddleLeftAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleLeftAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectMiddleLeft"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectMiddleCenterAlignmentCommand
	public class SetFloatingObjectMiddleCenterAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectMiddleCenterAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Center; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Center; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleCenterAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleCenterAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleCenterAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleCenterAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleCenterAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectMiddleCenterAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleCenterAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectMiddleCenter"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectMiddleRightAlignmentCommand
	public class SetFloatingObjectMiddleRightAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectMiddleRightAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Right; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Center; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleRightAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleRightAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleRightAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectMiddleRightAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleRightAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectMiddleRightAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectMiddleRightAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectMiddleRight"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectBottomLeftAlignmentCommand
	public class SetFloatingObjectBottomLeftAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectBottomLeftAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Left; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Bottom; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomLeftAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomLeftAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomLeftAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomLeftAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomLeftAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectBottomLeftAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomLeftAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectBottomLeft"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectBottomCenterAlignmentCommand
	public class SetFloatingObjectBottomCenterAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectBottomCenterAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Center; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Bottom; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomCenterAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomCenterAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomCenterAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomCenterAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomCenterAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectBottomCenterAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomCenterAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectBottomCenter"; } }
		#endregion
	}
	#endregion
	#region SetFloatingObjectBottomRightAlignmentCommand
	public class SetFloatingObjectBottomRightAlignmentCommand : ChangeFloatingObjectAlignmentCommandBase {
		public SetFloatingObjectBottomRightAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return FloatingObjectHorizontalPositionAlignment.Right; } }
		protected internal override FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return FloatingObjectVerticalPositionAlignment.Bottom; } }
		protected internal override FloatingObjectHorizontalPositionType HorizontalPositionType { get { return FloatingObjectHorizontalPositionType.Margin; } }
		protected internal override FloatingObjectVerticalPositionType VerticalPositionType { get { return FloatingObjectVerticalPositionType.Margin; } }
		protected internal override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomRightAlignmentCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomRightAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomRightAlignmentCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetFloatingObjectBottomRightAlignmentDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomRightAlignmentCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetFloatingObjectBottomRightAlignment; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetFloatingObjectBottomRightAlignmentCommandImageName")]
#endif
		public override string ImageName { get { return "AlignFloatingObjectBottomRight"; } }
		#endregion
	}
	#endregion
	#region FloatingObjectBringForwardCommandBase (abstract class)
	public abstract class FloatingObjectBringForwardCommandBase : FloatingObjectCommandBase {
		readonly FloatingObjectProperties floatingObjectProperties;
		protected FloatingObjectBringForwardCommandBase(IRichEditControl control)
			: base(control) {
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
			if (run != null)
				floatingObjectProperties = run.FloatingObjectProperties;
		}
		protected FloatingObjectProperties FloatingObjectProperties { get { return floatingObjectProperties; } }
		protected internal int ZOrder { get { return floatingObjectProperties.ZOrder; } }
		protected void ChangeZOrder() {
			if (FloatingObjectProperties == null)
				return;
			List<IZOrderedObject> floatingObjectList = ActivePieceTable.GetFloatingObjectList();
			ChangeZOrderCore(floatingObjectList, floatingObjectList.IndexOf(FloatingObjectProperties));
		}
		abstract protected void ChangeZOrderCore(IList<IZOrderedObject> objects, int objectIndex);
	}
	#endregion
	#region FloatingObjectBringForwardCommand
	public class FloatingObjectBringForwardCommand : FloatingObjectBringForwardCommandBase {	   
		public FloatingObjectBringForwardCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringForwardCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringForward; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringForwardCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringForwardDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringForwardCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectBringForward; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringForwardCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectBringForward"; } }
		#endregion
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			ChangeZOrder();
			return DocumentModelChangeActions.None;
		}
		protected override void ChangeZOrderCore(IList<IZOrderedObject> objects, int objectIndex) {
			ZOrderManager manager = new ZOrderManager();
			manager.BringForward(objects, objectIndex);
		}
	}
	#endregion
	#region FloatingObjectBringToFrontCommand
	public class FloatingObjectBringToFrontCommand : FloatingObjectBringForwardCommandBase {
		public FloatingObjectBringToFrontCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringToFrontCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringToFront; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringToFrontCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringToFrontDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringToFrontCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectBringToFront; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringToFrontCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectBringToFront"; } }
		#endregion
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			ChangeZOrder();
			return DocumentModelChangeActions.None;
		}
		protected override void ChangeZOrderCore(IList<IZOrderedObject> objects, int objectIndex) {
			ZOrderManager manager = new ZOrderManager();
			manager.BringToFront(objects, objectIndex);
		}
	}
	#endregion
	#region FloatingObjectBringInFrontOfTextCommand
	public class FloatingObjectBringInFrontOfTextCommand : ChangeFloatingObjectPropertyCommandBase<bool> {
		public FloatingObjectBringInFrontOfTextCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal bool IsBehindDoc { get { return false; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringInFrontOfTextCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringInFrontOfText; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringInFrontOfTextCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringInFrontOfTextDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringInFrontOfTextCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectBringInFrontOfText; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectBringInFrontOfTextCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectBringInFrontOfText"; } }
		#endregion
		protected internal override FloatingObjectRunPropertyModifier<bool> CreateModifier(ICommandUIState state) {
			return new FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier(IsBehindDoc);
		}
	}
	#endregion
	#region FloatingObjectSendBackwardCommandBase (abstract class)
	public abstract class FloatingObjectSendBackwardCommandBase : FloatingObjectCommandBase {
		readonly FloatingObjectProperties floatingObjectProperties;
		protected FloatingObjectSendBackwardCommandBase(IRichEditControl control)
			: base(control) {
			FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
			if (run != null)
				floatingObjectProperties = run.FloatingObjectProperties;
		}
		protected FloatingObjectProperties FloatingObject { get { return floatingObjectProperties; } }
		protected internal int ZOrder { get { return floatingObjectProperties.ZOrder; } }
		protected void ChangeZOrder() {
			if (FloatingObject == null)
				return;
			List<IZOrderedObject> floatingObjectList = ActivePieceTable.GetFloatingObjectList();
			ChangeZOrderCore(floatingObjectList, floatingObjectList.IndexOf(FloatingObject));
		}
		abstract protected void ChangeZOrderCore(IList<IZOrderedObject> objects, int objectIndex);
	}
	#endregion
	#region FloatingObjectSendBackwardCommand
	public class FloatingObjectSendBackwardCommand : FloatingObjectSendBackwardCommandBase {
		public FloatingObjectSendBackwardCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBackwardCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendBackward; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBackwardCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendBackwardDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBackwardCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectSendBackward; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBackwardCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectSendBackward"; } }
		#endregion
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			ChangeZOrder();
			return DocumentModelChangeActions.None;
		}
		protected override void ChangeZOrderCore(IList<IZOrderedObject> objects, int objectIndex) {
			ZOrderManager manager = new ZOrderManager();
			manager.SendBackward(objects, objectIndex);
		}
	}
	#endregion
	#region FloatingObjectSendToBackCommand
	public class FloatingObjectSendToBackCommand : FloatingObjectSendBackwardCommandBase {
		public FloatingObjectSendToBackCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendToBackCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendToBack; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendToBackCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendToBackDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendToBackCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectSendToBack; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendToBackCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectSendToBack"; } }
		#endregion
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			ChangeZOrder();
			return DocumentModelChangeActions.None;
		}
		protected override void ChangeZOrderCore(IList<IZOrderedObject> objects, int objectIndex) {
			ZOrderManager manager = new ZOrderManager();
			manager.SendToBack(objects, objectIndex);
		}
	}
	#endregion
	#region FloatingObjectSendBehindTextCommand
	public class FloatingObjectSendBehindTextCommand : ChangeFloatingObjectPropertyCommandBase<bool> {
		public FloatingObjectSendBehindTextCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal bool IsBehindDoc { get { return true; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBehindTextCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendBehindText; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBehindTextCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendBehindTextDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBehindTextCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectSendBehindText; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FloatingObjectSendBehindTextCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectSendBehindText"; } }
		#endregion
		protected internal override FloatingObjectRunPropertyModifier<bool> CreateModifier(ICommandUIState state) {
			return new FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier(IsBehindDoc);
		}
	}
	#endregion
	#region FloatingObjectZOrderComparer
	public class FloatingObjectZOrderComparer : IComparer<IZOrderedObject> {
		public int Compare(IZOrderedObject first, IZOrderedObject second) {
			return Comparer<int>.Default.Compare(first.ZOrder, second.ZOrder);
		}
	}
	#endregion
	#region ChangeFloatingObjectFillColorCommand
	public class ChangeFloatingObjectFillColorCommand : ChangeFloatingObjectPropertyCommandBase<Color> {
		public ChangeFloatingObjectFillColorCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectFillColorCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFloatingObjectFillColor; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectFillColorCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectFillColor; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectFillColorCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectFillColorDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectFillColorCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectFillColor"; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<Color> result = new DefaultValueBasedCommandUIState<Color>();
			return result;
		}
		protected internal override FloatingObjectRunPropertyModifier<Color> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			return new FloatingObjectRunFillColorModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState != null) {
				Color value;
				GetCurrentPropertyValue(out value);
				valueBasedState.Value = value;
			}
		}
	}
	#endregion
	#region ChangeFloatingObjectOutlineColorCommand
	public class ChangeFloatingObjectOutlineColorCommand : ChangeFloatingObjectPropertyCommandBase<Color> {
		public ChangeFloatingObjectOutlineColorCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineColorCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFloatingObjectOutlineColor; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineColorCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineColor; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineColorCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineColorDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineColorCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectOutlineColor"; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<Color> result = new DefaultValueBasedCommandUIState<Color>();
			return result;
		}
		protected internal override FloatingObjectRunPropertyModifier<Color> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			return new FloatingObjectRunOutlineColorModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<Color> valueBasedState = state as IValueBasedCommandUIState<Color>;
			if (valueBasedState != null) {
				Color value;
				GetCurrentPropertyValue(out value);
				valueBasedState.Value = value;
			}
		}
	}
	#endregion
	#region ChangeFloatingObjectOutlineWidthCommand
	public class ChangeFloatingObjectOutlineWidthCommand : ChangeFloatingObjectPropertyCommandBase<int> {
		public ChangeFloatingObjectOutlineWidthCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineWidthCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFloatingObjectOutlineWeight; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineWidthCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineWidth; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeFloatingObjectOutlineWidthCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectOutlineWidthDescription; } }
		#endregion
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<int> result = new DefaultValueBasedCommandUIState<int>();
			return result;
		}
		protected internal override FloatingObjectRunPropertyModifier<int> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
			if (valueBasedState == null)
				Exceptions.ThrowInternalException();
			return new FloatingObjectRunOutlineWidthAndColorModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<int> valueBasedState = state as IValueBasedCommandUIState<int>;
			if (valueBasedState != null) {
				int value;
				GetCurrentPropertyValue(out value);
				valueBasedState.Value = value;
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region ChangeFloatingObjectAlignmentCommand
	public class ChangeFloatingObjectAlignmentCommand : RichEditMenuItemSimpleCommand {
		public ChangeFloatingObjectAlignmentCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectAlignment; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectAlignmentDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFloatingObjectAlignment; } }
		public override string ImageName { get { return "FloatingObjectAlignment"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable && DocumentModel.Selection.Length == 1 && ActivePieceTable.Runs[DocumentModel.Selection.Interval.Start.RunIndex] is FloatingObjectAnchorRun;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region ChangeFloatingObjectTextWrapTypeCommand
	public class ChangeFloatingObjectTextWrapTypeCommand : RichEditMenuItemSimpleCommand {
		public ChangeFloatingObjectTextWrapTypeCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectTextWrapType; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeFloatingObjectTextWrapTypeDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeFloatingObjectTextWrapType; } }
		public override string ImageName { get { return "FloatingObjectTextWrapType"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable && DocumentModel.Selection.Length == 1 && ActivePieceTable.Runs[DocumentModel.Selection.Interval.Start.RunIndex] is FloatingObjectAnchorRun;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region ChangeFloatingObjectTextWrapTypeMenuCommand
	public class ChangeFloatingObjectTextWrapTypeMenuCommand : ChangeFloatingObjectTextWrapTypeCommand {
		public ChangeFloatingObjectTextWrapTypeMenuCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
	#region FloatingObjectBringForwardPlaceholderCommand
	public class FloatingObjectBringForwardPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public FloatingObjectBringForwardPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringForwardPlaceholder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectBringForwardPlaceholderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectBringForwardPlaceholder; } }
		public override string ImageName { get { return "FloatingObjectBringForward"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable && DocumentModel.Selection.Length == 1 && ActivePieceTable.Runs[DocumentModel.Selection.Interval.Start.RunIndex] is FloatingObjectAnchorRun;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region FloatingObjectBringForwardMenuCommand
	public class FloatingObjectBringForwardMenuCommand : FloatingObjectBringForwardPlaceholderCommand {
		public FloatingObjectBringForwardMenuCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
	#region FloatingObjectSendBackwardPlaceholderCommand
	public class FloatingObjectSendBackwardPlaceholderCommand : RichEditMenuItemSimpleCommand, IPlaceholderCommand {
		public FloatingObjectSendBackwardPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendBackwardPlaceholder; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FloatingObjectSendBackwardPlaceholderDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.FloatingObjectSendBackwardPlaceholder; } }
		public override string ImageName { get { return "FloatingObjectSendBackward"; } }
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable && DocumentModel.Selection.Length == 1 && ActivePieceTable.Runs[DocumentModel.Selection.Interval.Start.RunIndex] is FloatingObjectAnchorRun;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	#endregion
	#region FloatingObjectSendBackwardMenuCommand
	public class FloatingObjectSendBackwardMenuCommand : FloatingObjectSendBackwardPlaceholderCommand {
		public FloatingObjectSendBackwardMenuCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Visible = state.Enabled;
		}
	}
	#endregion
}
