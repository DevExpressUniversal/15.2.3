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

using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.ChangeEditorRowLimitCommand_MenuCaption, Localization.SnapStringId.ChangeEditorRowLimitCommand_Description)]
	public class ChangeEditorRowLimitCommand : EditListCommandBase {
		EditorRowLimitCommandValue newValue;
		public ChangeEditorRowLimitCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return SnapCommandId.ChangeEditorRowLimit; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<EditorRowLimitCommandValue> valueBasedState = state as IValueBasedCommandUIState<EditorRowLimitCommandValue>;
			if (valueBasedState != null) {
				valueBasedState.Value = GetCurrentPropertyValue();
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			this.newValue = ((IValueBasedCommandUIState<EditorRowLimitCommandValue>)state).Value;
			base.ForceExecute(state);
		}
		protected internal override void UpdateFieldCode(XtraRichEdit.Fields.InstructionController controller) {
			if (newValue == null || newValue.Value < 0)
				return;
			controller.SetSwitch(SNListField.EditorRowLimitSwitch, newValue.Value.ToString(CultureInfo.InvariantCulture));			
		}
		EditorRowLimitCommandValue GetCurrentPropertyValue() {
			if (EditedFieldInfo == null)
				return null;
			return new EditorRowLimitCommandValue(EditedFieldInfo.ParsedInfo.EditorRowLimit);
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			DefaultValueBasedCommandUIState<EditorRowLimitCommandValue> result = new DefaultValueBasedCommandUIState<EditorRowLimitCommandValue>();
			return result;
		}
	}
	public class EditorRowLimitCommandValue {
		public EditorRowLimitCommandValue() {
		}
		public EditorRowLimitCommandValue(int value) {
			this.Value = value;
		}
		public int Value { get; set; }
		public override string ToString() {
			if (Value == 0)
				return SnapLocalizer.GetString(SnapStringId.EditorRowLimitShowAll);
			else
				return Value.ToString();
		}
		public override bool Equals(object obj) {
			EditorRowLimitCommandValue other = obj as EditorRowLimitCommandValue;
			if (other != null)
				return other.Value == Value;
			else
				return false;
		}
		public override int GetHashCode() {
			return Value;
		}
	}
}
