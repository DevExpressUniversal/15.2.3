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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using DevExpress.Office.Forms;
namespace DevExpress.XtraRichEdit.Forms {
	public class RichEditInsertSymbolViewModel : InsertSymbolViewModel {
		readonly IRichEditControl control;
		readonly FormControllerParameters parameters;
		private class InsertSymbolControllerParameters : FormControllerParameters {
			internal InsertSymbolControllerParameters(IRichEditControl control)
				: base(control) {
			}
		}
		public RichEditInsertSymbolViewModel(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.parameters = new InsertSymbolControllerParameters(control);
		}
		public IRichEditControl Control { get { return control; } }
		public FormControllerParameters Parameters { get { return parameters; } }
		public override void ApplyChanges() {
			IInsertSymbolCommand command = Control.CreateCommand(RichEditCommandId.InsertSymbol) as IInsertSymbolCommand;
			if (command.CanExecute()) {
				DefaultValueBasedCommandUIState<SymbolProperties> state = new DefaultValueBasedCommandUIState<SymbolProperties>();
				state.Value = new SymbolProperties(this.UnicodeChar, this.FontName);
				command.ForceExecute(state);
			}
		}
	}
}
