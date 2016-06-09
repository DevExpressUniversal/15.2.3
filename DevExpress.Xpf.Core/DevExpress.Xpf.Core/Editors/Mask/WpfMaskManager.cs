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

using System.Windows;
using DevExpress.Data.Mask;
using System.Globalization;
using System;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.EditStrategy;
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public class WpfMaskManager {
		Locker SetInitialEditValueLocker { get; set; }
		readonly IMaskManagerProvider maskManagerProvider;
		MaskManager MaskManager { get; set; }
		public int DisplayCursorPosition { get { return MaskManager != null ? MaskManager.DisplayCursorPosition : 0; } }
		public int DisplaySelectionLength { get { return MaskManager != null ? MaskManager.DisplaySelectionLength : 0; } }
		public int DisplaySelectionStart { get { return MaskManager != null ? MaskManager.DisplaySelectionStart : 0; } }
		public bool CanUndo { get { return MaskManager.CanUndo; } }
		public bool IsFinal { get { return MaskManager.IsFinal; } }
		public bool IsMatch { get { return MaskManager.IsMatch; } }
		public WpfMaskManager(IMaskManagerProvider maskManagerProvider) {
			this.maskManagerProvider = maskManagerProvider;
			SetInitialEditValueLocker = new Locker();
			Initialize();
		}
		public void Initialize() {
			UnsubscribeEvents();
			MaskManager = maskManagerProvider.CreateNew();
			SubscribeEvents();
		}
		void SubscribeEvents() {
			if(MaskManager == null)
				return;
			MaskManager.LocalEditAction += MaskManagerLocalEditAction;
			MaskManager.EditTextChanged += MaskManagerEditTextChanged;
		}
		void UnsubscribeEvents() {
			if(MaskManager == null)
				return;
			MaskManager.LocalEditAction -= MaskManagerLocalEditAction;
			MaskManager.EditTextChanged -= MaskManagerEditTextChanged;
		}
		void MaskManagerEditTextChanged(object sender, System.EventArgs e) {
			SetInitialEditValueLocker.DoIfNotLocked(() => maskManagerProvider.UpdateRequired());
		}
		void MaskManagerLocalEditAction(object sender, System.ComponentModel.CancelEventArgs e) {
			maskManagerProvider.LocalEditActionPerformed();
		}
		public void SetInitialEditValue(object initialEditValue) {
			if(MaskManager == null)
				return;
			SetInitialEditValueLocker.DoLockedAction(() => MaskManager.SetInitialEditValue(initialEditValue));
		}
		public bool FlushPendingEditActions() {
			return MaskManager.FlushPendingEditActions();
		}
		public string DisplayText {
			get {
				if(MaskManager != null)
					return MaskManager.DisplayText;
				return string.Empty;
			}
		}
		public object GetCurrentEditValue() {
			return MaskManager != null ? MaskManager.GetCurrentEditValue() : null;
		}
		public bool Insert(string insertion) {
			return MaskManager.Insert(insertion);
		}
		public bool CursorRight(bool forceSelection) {
			return MaskManager.CursorRight(forceSelection, false);
		}
		public bool CursorLeft(bool forceSelection) {
			return MaskManager.CursorLeft(forceSelection, false);
		}
		public bool CheckCursorLeft() {
			return MaskManager.CursorLeft(false, true);
		}
		public bool CheckCursorRight() {
			return MaskManager.CursorRight(false, true);
		}
		public bool Backspace() {
			return MaskManager.Backspace();
		}
		public bool Delete() {
			return MaskManager.Delete();
		}
		public bool CursorHome(bool forceSelection) {
			return MaskManager.CursorHome(forceSelection);
		}
		public bool CursorEnd(bool forceSelection) {
			return MaskManager.CursorEnd(forceSelection);
		}
		public void SelectAll() {
			MaskManager.SelectAll();
		}
		public bool Undo() {
			return MaskManager.Undo();
		}
		public bool SpinUp() {
			return MaskManager.SpinUp();
		}
		public bool SpinDown() {
			return MaskManager.SpinDown(); 
		}
		public bool CursorToDisplayPosition(int newPosition, bool forceSelection) {
			return MaskManager != null ? MaskManager.CursorToDisplayPosition(newPosition, forceSelection) : false;
		}
	}
}
