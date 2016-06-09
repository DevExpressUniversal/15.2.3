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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region MailMergeModeCommand
	public abstract class MailMergeModeCommand :MailMergeCommand {
		protected MailMergeModeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected abstract string ModeValue { get; }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdateFromUI();
			try {
				SetBookDefinedNameValue(MailMergeDefinedNames.MailMergeMode, ModeValue);
				Control.InnerControl.RaiseUpdateUI();
				Control.InnerControl.Owner.Redraw();
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			DefinedNameCollection definedNames = DocumentModel.DefinedNames;
			if (definedNames.Contains(MailMergeDefinedNames.MailMergeMode)) {
				DefinedNameBase definedName = DocumentModel.DefinedNames[MailMergeDefinedNames.MailMergeMode];
				VariantValue value = definedName.EvaluateToSimpleValue(DocumentModel.DataContext);
				value = value.ToText(DocumentModel.DataContext);
				if (value.IsText)
					state.Checked = string.Format("\"{0}\"", value.GetTextValue(DocumentModel.SharedStringTable)) == ModeValue;
			}
			else
				state.Checked = false;
		}
	}
	#endregion
}
