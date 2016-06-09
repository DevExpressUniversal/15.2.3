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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Data.Filtering;
#if !SL
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Collections.Specialized;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core;
using System.Windows.Media;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextCompositionEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLTextCompositionEventArgs;
using System.Collections.Specialized;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.Editors.Services {
	public abstract class TextInputServiceBase : BaseEditBaseService {
		new TextEditPropertyProvider PropertyProvider { get { return (TextEditPropertyProvider)base.PropertyProvider; } }
		new TextEditBase OwnerEdit { get { return (TextEditBase)base.OwnerEdit; } }
		TextInputSettingsBase TextInputSettings { get { return PropertyProvider.TextInputSettings; } }
		protected EditBoxWrapper EditBox { get { return OwnerEdit.EditBox; } }
		public virtual int SelectionStart { get { return TextInputSettings.SelectionStart; } }
		public virtual int SelectionLength { get { return TextInputSettings.SelectionLength; } }
		public virtual bool ShouldUseFormatting { get { return TextInputSettings.ShouldUseFormatting; } }
		public virtual bool AcceptsReturn { get { return TextInputSettings.GetAcceptsReturn(); } }
		protected TextInputServiceBase(TextEditBase editor)
			: base(editor) {
		}
		public virtual void PreviewTextInput(TextCompositionEventArgs e) {
			TextInputSettings.ProcessPreviewTextInput(e);
		}
		public virtual void PreviewKeyDown(KeyEventArgs e) {
			TextInputSettings.ProcessPreviewKeyDown(e);
		}
		protected virtual bool GetShouldUseFormatting() {
			return false;
		}
		public virtual string FormatDisplayText(object editValue, bool applyFormatting) {
			return TextInputSettings.FormatDisplayText(editValue, applyFormatting);
		}
		public virtual void PerformNullInput() {
			TextInputSettings.PerformNullInput();
		}
		public virtual void GotFocus() {
			TextInputSettings.PerformGotFocus();
		}
		public virtual void LostFocus() {
			TextInputSettings.PerformLostFocus();
		}
		public virtual void FlushPendingEditActions(UpdateEditorSource updateSource) {
			TextInputSettings.FlushPendingEditActions(updateSource);
		}
		public virtual void Initialize() {
		}
		public virtual void SyncWithEditor() {
			TextInputSettings.ProcessSyncWithEditor();
		}
		public virtual bool SpinUp() {
			return TextInputSettings.SpinUp();
		}
		public virtual bool SpinDown() {
			return TextInputSettings.SpinDown();
		}
		public virtual void Delete() {
			TextInputSettings.Delete();
		}
		public virtual void SelectAll() {
			Select(0, EditBox.Text.Length);
		}
		public virtual void Select(int selectionStart, int selectionLength) {
			TextInputSettings.Select(selectionStart, selectionLength);
		}
		public virtual void SetInitialEditValue(object editValue) {
			TextInputSettings.SetInitialEditValue(editValue);
		}
		public virtual bool CanUndo() {
			return TextInputSettings.CanUndo();
		}
		public virtual void Undo() {
			TextInputSettings.Undo();
		}
		public virtual void Cut() {
			TextInputSettings.Cut();
		}
		public virtual bool CanCut() {
			return TextInputSettings.CanCut();
		}
		public virtual bool CanPaste() {
			return TextInputSettings.CanPaste();
		}
		public void Paste() {
			TextInputSettings.Paste();
		}
		public virtual void Copy() {
			TextInputSettings.Copy();
		}
		public virtual bool CanCopy() {
			return TextInputSettings.CanCopy();
		}
		public virtual bool CanDelete() {
			return TextInputSettings.CanDelete();
		}
		public virtual bool CanSelectAll() {
			return TextInputSettings.CanSelectAll();
		}
		public virtual void InsertText(string value) {
			TextInputSettings.InsertText(value);
		}
		public virtual bool NeedsKey(Key key, ModifierKeys modifier) {
			return TextInputSettings.NeedsKey(key, modifier);
		}
		public virtual bool IsValueValid(object value) {
			return TextInputSettings.IsValueValid(value);
		}
		public virtual void PreviewMouseDown(MouseEventArgs e) {
			TextInputSettings.ProcessPreviewMouseDown(e);
		}
		public virtual void PreviewMouseUp(MouseEventArgs e) {
			TextInputSettings.ProcessPreviewMouseUp(e);
		}
		public virtual void PreviewMouseWheel(MouseWheelEventArgs e) {
			TextInputSettings.ProcessPreviewMouseWheel(e);
		}
		public virtual void Reset() {
			TextInputSettings.Reset();
		}
		public virtual void UpdateIme() {
			TextInputSettings.UpdateIme();
		}
	}
}
