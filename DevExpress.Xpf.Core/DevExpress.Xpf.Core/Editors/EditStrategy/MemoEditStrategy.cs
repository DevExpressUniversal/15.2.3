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
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Services;
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors.EditStrategy {
	public class MemoEditStrategy : PopupBaseEditStrategy {
		public MemoEditStrategy(MemoEdit editor)
			: base(editor) {
		}
		protected new MemoEdit Editor { get { return base.Editor as MemoEdit; } }
		public override void EditValueChanged(object oldValue, object newValue) {
			base.EditValueChanged(oldValue, newValue);
			SyncMemoWithEditor(Editor.Memo);
		}
		public void SyncWithMemo() {
			if (Editor.Memo != null)
				ValueContainer.SetEditValue(Editor.Memo.EditValue, UpdateEditorSource.TextInput);
		}
		public void SyncMemoWithEditor(TextEdit memo) {
			if (memo != null) {
				bool isUndoEnabledBackup = memo.EditBox.IsUndoEnabled;
				memo.EditBox.IsUndoEnabled = false;
				memo.EditValue = EditValue;
				memo.EditBox.IsUndoEnabled = isUndoEnabledBackup;
				memo.EditBox.CaretIndex = 0;
			}
		}
		bool isMemoTextModified = false;
		protected bool IsMemoTextModified {
			get { return isMemoTextModified; }
			set {
				isMemoTextModified = value;
				Editor.UpdateOkButtonIsEnabled(IsMemoTextModified);
			}
		}
		void Memo_EditValueChanged(object sender, EditValueChangedEventArgs e) {
			if (!IsMemoTextModified)
				IsMemoTextModified = true;
		}
		public void OnPopupOpened() {
			IsMemoTextModified = false;
			if (Editor.Memo == null)
				return;
			Editor.Memo.EditValueChanged += Memo_EditValueChanged;
		}
		protected internal override object ConvertEditValueForFormatDisplayText(object convertedValue) {
			object value = base.ConvertEditValueForFormatDisplayText(convertedValue);
			return TextBlockService.GetFirstLineFromText(Convert.ToString(value));
		}
	}
}
