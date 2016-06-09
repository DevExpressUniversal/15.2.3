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
	#region ShowLineNumberingFormCommand
	public class ShowLineNumberingFormCommand : ChangeSectionFormattingCommandBase<LineNumberingInfo> {
		public ShowLineNumberingFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineNumberingFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowLineNumberingForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineNumberingFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowLineNumberingForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLineNumberingFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowLineNumberingFormDescription; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				IValueBasedCommandUIState<LineNumberingInfo> valueBasedState = state as IValueBasedCommandUIState<LineNumberingInfo>;
				ShowLineNumberingForm(valueBasedState.Value, ShowLineNumberingFormCallback, state);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowLineNumberingFormCallback(LineNumberingInfo properties, object callbackData) {
			IValueBasedCommandUIState<LineNumberingInfo> valueBasedState = callbackData as IValueBasedCommandUIState<LineNumberingInfo>;
			valueBasedState.Value = properties;
			base.ForceExecute(valueBasedState);
		}
		internal virtual void ShowLineNumberingForm(LineNumberingInfo properties, ShowLineNumberingFormCallback callback, object callbackData) {
			Control.ShowLineNumberingForm(properties, callback, callbackData);
		}
		protected internal override SectionPropertyModifier<LineNumberingInfo> CreateModifier(ICommandUIState state) {
			IValueBasedCommandUIState<LineNumberingInfo> valueBasedState = state as IValueBasedCommandUIState<LineNumberingInfo>;
			return new SectionLineNumberingModifier(valueBasedState.Value);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<LineNumberingInfo> valueBasedState = state as IValueBasedCommandUIState<LineNumberingInfo>;
			if (valueBasedState == null)
				return;
			valueBasedState.Value = GetCurrentPropertyValue();
		}
		LineNumberingInfo GetCurrentPropertyValue() {
			MergedSectionPropertyModifier<LineNumberingInfo> modifier = (MergedSectionPropertyModifier<LineNumberingInfo>)CreateModifier(CreateDefaultCommandUIState());
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			LineNumberingInfo result = null;
			Debug.Assert(count > 0);
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, false);
				DocumentModelPosition end = CalculateEndPosition(item, false);
				LineNumberingInfo properties = DocumentModel.ObtainMergedSectionsPropertyValue(start.LogPosition, Math.Max(1, end.LogPosition - start.LogPosition), modifier);
				if (result != null)
					modifier.Merge(result, properties);
				else
					result = properties;
			}
			return result;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<LineNumberingInfo> result = new DefaultValueBasedCommandUIState<LineNumberingInfo>();
			return result;
		}
	}
	#endregion
}
