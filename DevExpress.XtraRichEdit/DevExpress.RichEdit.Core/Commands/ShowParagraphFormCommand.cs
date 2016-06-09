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
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Collections;
using System.Diagnostics;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowParagraphFormCommand
	public class ShowParagraphFormCommand : ChangeParagraphFormattingCommandBase<MergedParagraphProperties> {
		public ShowParagraphFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowParagraphFormCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ShowParagraphForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowParagraphFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowParagraphForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowParagraphFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowParagraphFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowParagraphFormCommandImageName")]
#endif
		public override string ImageName { get { return "Paragraph"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowParagraphFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<MergedParagraphProperties> valueBasedState = state as IValueBasedCommandUIState<MergedParagraphProperties>;
				ShowParagraphForm(valueBasedState.Value, ShowParagraphFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowParagraphFormCallback(MergedParagraphProperties properties, object callbackData) {
			IValueBasedCommandUIState<MergedParagraphProperties> valueBasedState = callbackData as IValueBasedCommandUIState<MergedParagraphProperties>;
			valueBasedState.Value = properties;
			base.ForceExecute(valueBasedState);
		}
		internal virtual void ShowParagraphForm(MergedParagraphProperties paragraphProperties, ShowParagraphFormCallback callback, object callbackData) {
			Control.ShowParagraphForm(paragraphProperties, callback, callbackData);
		}
		protected internal override ParagraphPropertyModifier<MergedParagraphProperties> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<MergedParagraphProperties> valueBasedState = state as IValueBasedCommandUIState<MergedParagraphProperties>;
			return new ParagraphPropertiesModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<MergedParagraphProperties> valueBasedState = state as IValueBasedCommandUIState<MergedParagraphProperties>;
			if (valueBasedState == null)
				return;
			valueBasedState.Value = GetCurrentPropertyValue();
		}
		MergedParagraphProperties GetCurrentPropertyValue() {
			MergedParagraphPropertyModifier<MergedParagraphProperties> modifier = (MergedParagraphPropertyModifier<MergedParagraphProperties>)CreateModifier(CreateDefaultCommandUIState());
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			MergedParagraphProperties result = null;
			Debug.Assert(count > 0);
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, false);
				DocumentModelPosition end = CalculateEndPosition(item, false);
				MergedParagraphProperties properties = ActivePieceTable.ObtainMergedParagraphsPropertyValue(start.LogPosition, Math.Max(1, end.LogPosition - start.LogPosition), modifier);;
				if (result != null)
					modifier.Merge(result, properties);
				else
					result = properties;
			}
			return result;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<MergedParagraphProperties> result = new DefaultValueBasedCommandUIState<MergedParagraphProperties>();
			return result;
		}
	}
	#endregion
}
