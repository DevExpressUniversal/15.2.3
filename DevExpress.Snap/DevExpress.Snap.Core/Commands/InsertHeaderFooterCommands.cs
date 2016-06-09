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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.Snap.Core.Commands {
	public abstract class InsertTemplateCommandBase : EditListCommandBase {
		protected InsertTemplateCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected abstract string TemplateSwitch { get; }
		protected override bool IsChecked() {
			return false;
		}
		protected virtual bool IsTemplateExists() {
			if (EditedFieldInfo == null)
				return false;
			InstructionController controller = EditedFieldInfo.CreateFieldInstructionController();
			return controller != null && controller.Instructions.GetBool(TemplateSwitch);
		}
		protected override void ExcecuteCoreInternal() {
			ITemplateModifier modifier = CreateTemplateModifier();
			TemplateModifierExecutor modifierExecutor = new TemplateModifierExecutor(EditedFieldInfo, TemplateSwitch);
			modifierExecutor.ModifyTemplate(modifier);			
		}
		protected abstract ITemplateModifier CreateTemplateModifier();
	}
	[CommandLocalization(Localization.SnapStringId.ListHeaderCommand_MenuCaption, Localization.SnapStringId.ListHeaderCommand_Description)]
	public class ListHeaderCommand : DropDownCommandBase {
		public ListHeaderCommand(IRichEditControl control)
			: base(control) {
		}		
		public override string ImageName { get { return "Header"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.ListHeader; } }
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] { SnapCommandId.InsertListHeader, SnapCommandId.RemoveListHeader };
		}
	}
	[CommandLocalization(Localization.SnapStringId.ListFooterCommand_MenuCaption, Localization.SnapStringId.ListFooterCommand_Description)]
	public class ListFooterCommand : DropDownCommandBase {
		public ListFooterCommand(IRichEditControl control)
			: base(control) {
		}		
		public override string ImageName { get { return "Footer"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.ListFooter; } }
		public override RichEditCommandId[] GetChildCommandIds() {
			return new RichEditCommandId[] { SnapCommandId.InsertListFooter, SnapCommandId.RemoveListFooter };
		}
	}
	public abstract class InsertListHeaderFooterCommandBase : InsertTemplateCommandBase {
		protected InsertListHeaderFooterCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected int FieldLevel { get { return EditedFieldInfo.Field.GetLevel(); } }
		protected override bool TryToKeepFieldSelection { get { return true; } }
		protected override bool IsEnabled() {
			return base.IsEnabled() && !IsTemplateExists();
		}
		protected override ITemplateModifier CreateTemplateModifier() {
			return CreateInsertHeaderFooterInternalCommand();
		}
		protected abstract ITemplateModifier CreateInsertHeaderFooterInternalCommand();
	}
	public abstract class RemoveListHeaderFooterCommandBase : InsertTemplateCommandBase {
		protected RemoveListHeaderFooterCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected int FieldLevel { get { return EditedFieldInfo.Field.GetLevel(); } }
		protected override bool IsEnabled() {
			return base.IsEnabled() && IsTemplateExists();
		}
		protected override ITemplateModifier CreateTemplateModifier() {
			return new InternalRemoveHeaderFooterCommand();
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertListHeaderCommand_MenuCaption, Localization.SnapStringId.InsertListHeaderCommand_Description)]
	public class InsertListHeaderCommand : InsertListHeaderFooterCommandBase {
		public InsertListHeaderCommand(IRichEditControl control)
			: base(control) {
		}
		protected override string TemplateSwitch { get { return SNListField.ListHeaderTemplateSwitch; } }
		public override string ImageName { get { return "InsertHeader"; } }
		protected override ITemplateModifier CreateInsertHeaderFooterInternalCommand() {
			return new InsertHeaderCommandInternal(DocumentModel, FieldLevel);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertListFooterCommand_MenuCaption, Localization.SnapStringId.InsertListFooterCommand_Description)]
	public class InsertListFooterCommand : InsertListHeaderFooterCommandBase {
		public InsertListFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected override string TemplateSwitch { get { return SNListField.ListFooterTemplateSwitch; } }
		public override string ImageName { get { return "InsertFooter"; } }
		protected override ITemplateModifier CreateInsertHeaderFooterInternalCommand() {
			return new InsertFooterCommandInternal(DocumentModel, FieldLevel);
		}
	}
	[CommandLocalization(Localization.SnapStringId.RemoveListHeaderCommand_MenuCaption, Localization.SnapStringId.RemoveListHeaderCommand_Description)]
	public class RemoveListHeaderCommand : RemoveListHeaderFooterCommandBase {
		public RemoveListHeaderCommand(IRichEditControl control)
			: base(control) {
		}
		protected override string TemplateSwitch { get { return SNListField.ListHeaderTemplateSwitch; } }
		public override string ImageName { get { return "RemoveHeader"; } }
	}
	[CommandLocalization(Localization.SnapStringId.RemoveListFooterCommand_MenuCaption, Localization.SnapStringId.RemoveListFooterCommand_Description)]
	public class RemoveListFooterCommand : RemoveListHeaderFooterCommandBase {
		public RemoveListFooterCommand(IRichEditControl control)
			: base(control) {
		}
		protected override string TemplateSwitch { get { return SNListField.ListFooterTemplateSwitch; } }
		public override string ImageName { get { return "RemoveFooter"; } }
	}
}
