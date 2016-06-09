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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeTableStyleCommand
	public class ChangeTableStyleCommand : ChangeFormattingCommand {
		public ChangeTableStyleCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ChangeTableStyleCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ChangeTableStyle; } }
		public override void UpdateUIState(DevExpress.Utils.Commands.ICommandUIState state) {
			if (DocumentModel.Selection.IsWholeSelectionInOneTable()) {
				state.Enabled = true;
				base.UpdateUIState(state);
			}
			else {
				state.Enabled = false;
				UpdateUIStateViaService(state);
			}
		}
		protected override IXtraRichEditFormatting GetInputPositionPropertyValue(DocumentModelPosition pos) {
			TextRunBase run = ActivePieceTable.Runs[pos.RunIndex];
			TableCell cell = run.Paragraph.GetCell();
			if (cell != null) {
			Table table = cell.Table;
			string styleName = table.TableStyle.StyleName;
			if (styleName != TableStyleCollection.DefaultTableStyleName)
				return new TableStyleFormatting(table.TableStyle.Id);
			}
			TableStyle defaultStyle = InnerControl.DocumentModel.TableStyles.DefaultItem;
			return new TableStyleFormatting(defaultStyle.Id);
		}
	}
	#endregion
}
