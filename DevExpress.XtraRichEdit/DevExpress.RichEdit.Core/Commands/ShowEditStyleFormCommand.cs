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
	public class ShowEditStyleFormCommand : RichEditMenuItemSimpleCommand {
		bool UseParagraphStyle;
		public ShowEditStyleFormCommand(IRichEditControl control)
			: base(control) {
			UseParagraphStyle = false;
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowEditStyleFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowEditStyleForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowEditStyleFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowEditStyleForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowEditStyleFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowEditStyleFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowEditStyleFormCommandImageName")]
#endif
		public override string ImageName { get { return "ModifyStyle"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowEditStyleFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
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
			CharacterStyle characterSourceStyle = firstRun.CharacterStyle;
			ParagraphStyle paragraphSourceStyle = firstRun.Paragraph.ParagraphStyle;
			bool OnlyOneParagraphStyle = true;
			for (RunIndex i = startIndex; i <= endIndex; i++) {
				TextRunBase run = model.ActivePieceTable.Runs[i];
				if (run.Paragraph.ParagraphStyle != paragraphSourceStyle) {
					OnlyOneParagraphStyle = false;
					continue;
				}
				if (run.CharacterStyle != characterSourceStyle) {
					UseParagraphStyle = true;
					continue;
				}
				if (run.CharacterStyleIndex == 0)
					UseParagraphStyle = true;
			}
			if (OnlyOneParagraphStyle) {
				ParagraphIndex index = firstRun.Paragraph.Index;
				if (UseParagraphStyle)
					ShowEditStyleForm(paragraphSourceStyle, index, ShowEditStyleFormCallback);
				else
					ShowEditStyleForm(characterSourceStyle, index, ShowEditStyleFormCallback);
			}
		}
		void ModifyParagraphStyle(ParagraphStyle paragraphSourceStyle, ParagraphStyle paragraphStyle) {
			paragraphSourceStyle.BeginUpdate();
			try {
				paragraphSourceStyle.CopyProperties(paragraphStyle);
				paragraphSourceStyle.StyleName = paragraphStyle.StyleName;
				paragraphSourceStyle.Tabs.SetTabs(paragraphStyle.Tabs.GetTabs());
				paragraphSourceStyle.Parent = paragraphStyle.Parent;
				paragraphSourceStyle.NextParagraphStyle = paragraphStyle.NextParagraphStyle;
			}
			finally {
				paragraphSourceStyle.EndUpdate();
			}
		}
		void ModifyCharacterStyle(CharacterStyle characterSourceStyle, CharacterStyle characterStyle) {
			characterSourceStyle.BeginUpdate();
			try {
				characterSourceStyle.CopyProperties(characterStyle);
				characterSourceStyle.StyleName = characterStyle.StyleName;
				characterSourceStyle.Parent = characterStyle.Parent;
			}
			finally {
				characterSourceStyle.EndUpdate();
			}
		}
		protected internal virtual void ShowEditStyleFormCallback(IStyle sourceStyle, IStyle targetStyle) {
			DocumentModel.BeginUpdate();
			try {
				ParagraphStyle paragraphStyleTo = sourceStyle as ParagraphStyle;
				ParagraphStyle paragraphStyleFrom = targetStyle as ParagraphStyle;
				if (paragraphStyleTo != null && paragraphStyleFrom != null) {
					ModifyParagraphStyle(paragraphStyleTo, paragraphStyleFrom);
					return;
				}
				CharacterStyle characterStyleTo = sourceStyle as CharacterStyle;
				CharacterStyle characterStyleFrom = targetStyle as CharacterStyle;
				if (characterStyleTo != null && characterStyleFrom != null) {
					ModifyCharacterStyle(characterStyleTo, characterStyleFrom);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		internal virtual void ShowEditStyleForm(ParagraphStyle sourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			Control.ShowEditStyleForm(sourceStyle, index, callback);
		}
		internal virtual void ShowEditStyleForm(CharacterStyle sourceStyle, ParagraphIndex index, ShowEditStyleFormCallback callback) {
			Control.ShowEditStyleForm(sourceStyle, index, callback);
		}
		protected internal override void ExecuteCore() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.CharacterStyle);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphStyle);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
