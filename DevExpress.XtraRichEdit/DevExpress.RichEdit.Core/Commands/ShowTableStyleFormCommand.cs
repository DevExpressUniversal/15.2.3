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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowEditStyleFormCommand
	public class ShowTableStyleFormCommand : RichEditMenuItemSimpleCommand {
		#region Fields
		TableStyle style;
		#endregion
		public ShowTableStyleFormCommand(IRichEditControl control)
			: this(control, null) {
		}
		public ShowTableStyleFormCommand(IRichEditControl control, TableStyle style)
			: base(control) {
			this.style = style;
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableStyleFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowEditStyleForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableStyleFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowEditStyleForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableStyleFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowEditStyleFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableStyleFormCommandImageName")]
#endif
		public override string ImageName { get { return "ModifyStyle"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableStyleFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTableStyleFormCommandStyle")]
#endif
		public TableStyle Style { get { return style; } set { style = value; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (Style != null)
					ShowTableStyleForm(Style);
				else
					FindStyleAndShowForm();
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		void FindStyleAndShowForm() {
			DocumentModel model = Control.InnerControl.DocumentModel;
			RunIndex startIndex = model.Selection.Interval.NormalizedStart.RunIndex;
			RunIndex endIndex = model.Selection.Interval.NormalizedEnd.RunIndex;
			TextRunBase firstRun = model.ActivePieceTable.Runs[startIndex];
			ParagraphIndex index = firstRun.Paragraph.Index;
			TableCell sourceCell = firstRun.PieceTable.Paragraphs[index].GetCell();
			if (sourceCell == null)
				return;
			Table targetTable = sourceCell.Table;
			bool onlyOneTableStyle = true;
			for (RunIndex i = startIndex; i <= endIndex; i++) {
				TextRunBase run = model.ActivePieceTable.Runs[i];
				if (run.PieceTable.Paragraphs[index].GetCell().Table.TableStyle != targetTable.TableStyle) {
					onlyOneTableStyle = false;
					continue;
				}
			}
			if (onlyOneTableStyle & sourceCell != null)
				ShowTableStyleForm(targetTable.TableStyle);
		}
		internal virtual void ShowTableStyleForm(TableStyle style) {
			DocumentModel.BeginUpdate();
			Control.ShowTableStyleForm(style);
			DocumentModel.EndUpdate();
		}
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.TableStyle);
		}
	}
	#endregion
}
