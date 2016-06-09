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

using DevExpress.XtraRichEdit;
using DevExpress.Utils.Commands;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.MailMergeCurrentRecord_MenuCaption, Localization.SnapStringId.MailMergeCurrentRecord_Description)]
	public class ChangeMailMergeCurrentRecordCommand : SnapMailMergeCommandBase {
		public ChangeMailMergeCurrentRecordCommand(IRichEditControl control) : base(control) {  }
		public override XtraRichEdit.Commands.RichEditCommandId Id { get { return SnapCommandId.MailMergeCurrentRecord; } }
		protected override bool ShouldApplyCommandsRestriction { get { return false; } }
		protected override bool IfCheckedCore(SnapMailMergeVisualOptions options) { return false; }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<MailMergeCurrentRecordCommandValue> valueBasedState = state as IValueBasedCommandUIState<MailMergeCurrentRecordCommandValue>;
			if(valueBasedState != null)
				valueBasedState.Value = GetCurrentPropertyValue();
		}
		public override void ForceExecute(ICommandUIState state) {
			MailMergeCurrentRecordCommandValue newValue = ((IValueBasedCommandUIState<MailMergeCurrentRecordCommandValue>)state).Value;
			if(object.ReferenceEquals(newValue, null) || (newValue.Value < 1)) {
				UpdateUIStateCore(state);
				return;
			}
			DocumentModel.SnapMailMergeVisualOptions.CurrentRecordIndex = newValue.Value - 1;
		}
		MailMergeCurrentRecordCommandValue GetCurrentPropertyValue() {
			ISnapControl snapControl = Control as ISnapControl;
			if(object.ReferenceEquals(snapControl, null))
				return null;
			SnapMailMergeVisualOptions mailMergeOptions = DocumentModel.SnapMailMergeVisualOptions;
			if(object.ReferenceEquals(mailMergeOptions.DataSourceName, null))
				return new MailMergeCurrentRecordCommandValue(0, null);
			return new MailMergeCurrentRecordCommandValue(mailMergeOptions.CurrentRecordIndex + 1, mailMergeOptions);
		}
	}
	public class MailMergeCurrentRecordCommandValue {
		SnapMailMergeVisualOptions options;
		public MailMergeCurrentRecordCommandValue() { }
		public MailMergeCurrentRecordCommandValue(int value, SnapMailMergeVisualOptions options) {
			Value = value;
			this.options = options;
		}
		public int Value { get; set; }
		public int MaxValue { get { return options.RecordCount; } }
		public override string ToString() {
			return Value.ToString();
		}
		public override bool Equals(object obj) {
			MailMergeCurrentRecordCommandValue other = obj as MailMergeCurrentRecordCommandValue;
			if(other == null)
				return false;
			return this.Value == other.Value;
		}
		public override int GetHashCode() {
			return Value;
		}
		public static MailMergeCurrentRecordCommandValue operator ++(MailMergeCurrentRecordCommandValue obj) {
			if(obj.MaxValue < 1)
				obj.Value = 0;
			else if(obj.Value < 1)
				obj.Value = 1;
			else if(obj.Value >= obj.MaxValue)
				obj.Value = obj.MaxValue;
			else
				obj.Value++;
			return obj;
		}
		public static MailMergeCurrentRecordCommandValue operator --(MailMergeCurrentRecordCommandValue obj) {
			if(obj.MaxValue < 1)
				obj.Value = 0;
			else if(obj.Value <= 1)
				obj.Value = 1;
			else if(obj.Value > obj.MaxValue)
				obj.Value = obj.MaxValue;
			else
				obj.Value--;
			return obj;
		}
		public static MailMergeCurrentRecordCommandValue operator +(MailMergeCurrentRecordCommandValue obj, int n) {
			int val = obj.Value + n;
			if(val < 1)
				val = 1;
			if(val > obj.MaxValue)
				val = obj.MaxValue;
			return new MailMergeCurrentRecordCommandValue(val, obj.options);
		}
		public static MailMergeCurrentRecordCommandValue operator -(MailMergeCurrentRecordCommandValue obj, int n) {
			return obj + (-n);
		}
	}
}
