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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Utils.Commands;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Collections;
using DevExpress.XtraEditors;
using System.Diagnostics;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowTabsFormCommand
	public class ShowTabsFormCommand : ChangeParagraphFormattingCommandBase<TabFormattingInfo> {
		IFormOwner tabFormOwner;
		public ShowTabsFormCommand(IRichEditControl control)
			: this(control, null) {
		}
		public ShowTabsFormCommand(IRichEditControl control, IFormOwner tabFormOwner)
			: base(control) {
			this.tabFormOwner = tabFormOwner;
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTabsFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowTabsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTabsFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowTabsFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowTabsFormCommandTabFormOwner")]
#endif
public IFormOwner TabFormOwner { get { return tabFormOwner; } set { tabFormOwner = value; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				TabsFormCommandUIState commandState = (TabsFormCommandUIState)state;
				ShowTabsForm(commandState);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		void ShowTabsForm(TabsFormCommandUIState commandState) {
			Control.ShowTabsForm(commandState.Value, commandState.DefaultTabWidth, ShowTabsFormCallback, commandState);
		}
		protected internal virtual void ShowTabsFormCallback(TabFormattingInfo tabInfo, int defaultTabWidth, object callbackData) {
			TabsFormCommandUIState commandUIState = (TabsFormCommandUIState)callbackData;
			commandUIState.Value = tabInfo;
			commandUIState.DefaultTabWidth = defaultTabWidth;
			base.ForceExecute(commandUIState);
		}
		protected internal override ParagraphPropertyModifier<TabFormattingInfo> CreateModifier(ICommandUIState state) {
			TabsFormCommandUIState commandState = (TabsFormCommandUIState)state;
			return new TabFormattingInfoModifier(commandState.Value, commandState.DefaultTabWidth);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.ParagraphTabs);
			TabsFormCommandUIState commandState = state as TabsFormCommandUIState;
			if (commandState == null)
				return;
			commandState.Value = GetCurrentPropertyValue();
			commandState.DefaultTabWidth = DocumentModel.DocumentProperties.DefaultTabWidth;
		}
		TabFormattingInfo GetCurrentPropertyValue() {
			MergedParagraphPropertyModifier<TabFormattingInfo> modifier = (MergedParagraphPropertyModifier<TabFormattingInfo>)CreateModifier(CreateDefaultCommandUIState());
			List<SelectionItem> items = DocumentModel.Selection.Items;
			int count = items.Count;
			TabFormattingInfo result = null;
			Debug.Assert(count > 0);
			for (int i = 0; i < count; i++) {
				SelectionItem item = items[i];
				DocumentModelPosition start = CalculateStartPosition(item, false);
				DocumentModelPosition end = CalculateEndPosition(item, false);				
				TabFormattingInfo properties = ActivePieceTable.ObtainMergedParagraphsPropertyValue(start.LogPosition, Math.Max(1, end.LogPosition - start.LogPosition), modifier);
				if (result != null)
					modifier.Merge(result, properties);
				else
					result = properties;
			}
			return result;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			TabsFormCommandUIState result = new TabsFormCommandUIState();
			result.TabsFormOwner = TabFormOwner;
			return result;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region TabsFormCommandUIState
	public class TabsFormCommandUIState : DefaultValueBasedCommandUIState<TabFormattingInfo> {
		int defaultTabWidth;
		IFormOwner tabsFormOwner;
		public int DefaultTabWidth { get { return defaultTabWidth; } set { defaultTabWidth = value; } }
		public IFormOwner TabsFormOwner { get { return tabsFormOwner; } set { tabsFormOwner = value; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	public interface IFormOwner {
		DocumentModelUnitConverter UnitConverter { get; }
		void Hide();
		void Show();
		void Close();
	}
}
