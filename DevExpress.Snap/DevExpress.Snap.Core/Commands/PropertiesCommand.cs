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
using System;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.PropertiesCommand_MenuCaption, Localization.SnapStringId.PropertiesCommand_Description)]
	public class PropertiesCommand : SnapMenuItemSimpleCommand { 
		public PropertiesCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEdit.Commands.RichEditCommandId Id { get { return SnapCommandId.Properties; } }
		public override string ImageName {
			get {
				return "Properties";
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandsRestriction(state, CharacterFormatting);
			if (!state.Enabled)
				return;
			ListFieldSelectionController controller = new ListFieldSelectionController(DocumentModel);
			SnapFieldInfo fieldInfo = controller.FindDataField();
			if (fieldInfo != null) {
				SNMergeFieldBase parsedInfo = fieldInfo.ParsedInfo as SNMergeFieldBase;
				state.Enabled = parsedInfo != null && parsedInfo.HasProperties;
			}
			else
				state.Enabled = false;
		}
		protected internal override void ExecuteCore() {
		}
		public override void Execute() {
		}
	}
}
