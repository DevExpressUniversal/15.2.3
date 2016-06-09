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
using DevExpress.Snap.Core.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit.UI;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Commands;
namespace DevExpress.Snap.Extensions.UI {
	public class MailMergeCurrentRecordItem : RichEditCommandBarEditItem<MailMergeCurrentRecordCommandValue> {
		public MailMergeCurrentRecordItem() {  }
		public MailMergeCurrentRecordItem(BarManager manager) : base(manager) { }
		public MailMergeCurrentRecordItem(string caption) : base(caption) { }
		public MailMergeCurrentRecordItem(BarManager manager, string caption) : base(manager, caption) { }
		protected override XtraRichEdit.Commands.RichEditCommandId CommandId { get { return SnapCommandId.MailMergeCurrentRecord; } }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SnapControl Control { get { return (SnapControl)base.Control; } set { Control = value; } }
		protected override ICommandUIState CreateCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<MailMergeCurrentRecordCommandValue> value = new DefaultValueBasedCommandUIState<MailMergeCurrentRecordCommandValue>();
			value.Value = ParseEditValue(EditValue);
			return value;
		}
		protected override RepositoryItem CreateEdit() {
			RepositoryItem edit = new RepositoryItemMailMergeCurrentRecordEdit();
			return edit;
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			RepositoryItemMailMergeCurrentRecordEdit edit = Edit as RepositoryItemMailMergeCurrentRecordEdit;
			if(!object.ReferenceEquals(edit, null))
				edit.ButtonPressed += edit_ButtonPressed;				
		}
		void edit_ButtonPressed(object sender, XtraEditors.Controls.ButtonPressedEventArgs e) {
			MailMergeCurrentRecordEdit edit = (MailMergeCurrentRecordEdit)sender;
			EditValue = edit.ProcessButtonClick(e.Button);
			InvokeCommand();
		}
		MailMergeCurrentRecordCommandValue ParseEditValue(object editValue) {
			try {
				MailMergeCurrentRecordCommandValue value = editValue as MailMergeCurrentRecordCommandValue;
				if(value != null)
					return value;
				return new MailMergeCurrentRecordCommandValue(Convert.ToInt32(editValue), ((DevExpress.Snap.Core.Options.ISnapControlOptions)Control.InnerControl.Options).SnapMailMergeVisualOptions);
			}
			catch {
				return null;
			}
		}
	}
}
