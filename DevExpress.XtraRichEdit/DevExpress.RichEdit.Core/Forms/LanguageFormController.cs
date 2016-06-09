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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms {
	#region LanguageFormController
	public class LanguageFormController : FormController {
		DocumentModel documentModel;
		IRichEditControl richEditControl;
		bool? noProof;
		public LanguageFormController(IRichEditControl iRichEditControl, DocumentModel documentModel) {
			this.documentModel = documentModel;
			this.richEditControl = iRichEditControl;
			ChangeLanguageCommand command = new ChangeLanguageCommand(richEditControl);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			IValueBasedCommandUIState<LangInfo> valueBasedState = (IValueBasedCommandUIState<LangInfo>)state;
			Language = valueBasedState.Value;
			ChangeNoProofCommand commandNoProof = new ChangeNoProofCommand(richEditControl);
			ICommandUIState stateNoProof = commandNoProof.CreateDefaultCommandUIState();
			commandNoProof.UpdateUIState(stateNoProof);
			IValueBasedCommandUIState<bool?> valueBasedStateNoProof = (IValueBasedCommandUIState<bool?>)stateNoProof;
			NoProof = valueBasedStateNoProof.Value;
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public IRichEditControl RichEditControl { get { return richEditControl; } }
		public LangInfo Language { get; set; }
		public bool? NoProof { get { return noProof; } set { noProof = value; } }
		public override void ApplyChanges() {
			documentModel.BeginUpdate();
			if (Language.Latin != null){
				ChangeLanguageCommand command = new ChangeLanguageCommand(richEditControl);
				ICommandUIState state = command.CreateDefaultCommandUIState();
				state.Enabled = true;
				IValueBasedCommandUIState<LangInfo> valueBasedState = state as IValueBasedCommandUIState<LangInfo>;
				if (valueBasedState != null)
					valueBasedState.Value = Language;
				command.ForceExecute(state);
			}
			if (NoProof != null) {
				ChangeNoProofCommand commandNoProof = new ChangeNoProofCommand(richEditControl);
				ICommandUIState stateNoProof = commandNoProof.CreateDefaultCommandUIState();
				stateNoProof.Enabled = true;
				IValueBasedCommandUIState<bool?> valueBasedStateNoProof = stateNoProof as IValueBasedCommandUIState<bool?>;
				if (valueBasedStateNoProof != null)
					valueBasedStateNoProof.Value = NoProof;
				commandNoProof.ForceExecute(stateNoProof);
			}
			documentModel.EndUpdate();
		}
	}
	#endregion
}
