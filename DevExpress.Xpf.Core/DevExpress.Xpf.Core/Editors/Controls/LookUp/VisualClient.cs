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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
namespace DevExpress.Xpf.Editors.Native {
	public abstract class VisualClientOwner : DependencyObject {
		Locker OpenPopupLocker { get; set; }
		public bool IsPopupOpened { get { return OpenPopupLocker.IsLocked; } }
		public bool PostPopupValue { get; protected set; }
		FrameworkElement innerEditor;
		public FrameworkElement InnerEditor {
			get {
				if (LookUpEditHelper.HasPopupContent(Editor))
					return innerEditor ?? (innerEditor = FindEditor());
				return null;
			}
		}
		protected virtual bool IsLoaded { get { return IsPopupOpened && InnerEditor != null; } }
		public bool IsKeyboardFocusWithin { get { return InnerEditor != null && InnerEditor.GetIsKeyboardFocusWithin(); } }
		protected PopupBaseEdit Editor { get; private set; }
		protected VisualClientOwner(PopupBaseEdit editor) {
			OpenPopupLocker = new Locker();
			Editor = editor;
		}
		protected abstract FrameworkElement FindEditor();
		protected abstract void SetupEditor();
		public virtual void PopupOpened() {
			SubscribeEvents();
		}
		public virtual void PopupClosed() {
			UnsubscribeEvents();
			OpenPopupLocker.Unlock();
		}
		public virtual void PopupDestroyed() {
			ClearInnerEditor();
		}
		protected virtual void ClearInnerEditor() {
			innerEditor = null;
		}
		public abstract void SyncProperties(bool syncDataSource);
		public virtual void SyncValues(bool resetTotal = false) {
		}
		public bool ProcessKeyDown(KeyEventArgs e) {
			if (e.Handled)
				return true;
			bool shouldProcess = ProcessKeyDownInternal(e);
			PostPopupValue = e.Handled;
			return shouldProcess;
		}
		public abstract bool ProcessKeyDownInternal(KeyEventArgs e);
		public virtual void BeforePopupOpened() {
			OpenPopupLocker.Lock();
			ClearInnerEditor();
			if (InnerEditor != null)
				SetupEditor();
		}
		public abstract object GetSelectedItem();
		public abstract IEnumerable GetSelectedItems();
		public virtual void PopupContentLoaded() {
			if (IsLoaded)
				SetupEditor();
		}
		public virtual bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return key == Key.Enter;
		}
		public virtual bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return true;
		}
		public virtual void InnerEditorMouseMove(object sender, MouseEventArgs e) {
			PostPopupValue = true;
		}
		protected virtual void SubscribeEvents() {
			if (InnerEditor == null)
				return;
			InnerEditor.MouseMove += InnerEditorMouseMove;
		}
		protected virtual void UnsubscribeEvents() {
			if (InnerEditor == null)
				return;
			InnerEditor.MouseMove -= InnerEditorMouseMove;
		}
		public virtual void PreviewTextInput(TextCompositionEventArgs e) {
		}
	}
	public class ListBoxVisualClientOwner : VisualClientOwner {
		LookUpEditBasePropertyProvider PropertyProvider { get { return (LookUpEditBasePropertyProvider)Editor.PropertyProvider; } }
		bool IsServerMode { get { return PropertyProvider.IsServerMode; } }
		protected override bool IsLoaded { get { return base.IsLoaded && ListBox != null; } }
		PopupListBox ListBox { get { return InnerEditor as PopupListBox; } }
		new ComboBoxEdit Editor { get { return base.Editor as ComboBoxEdit; } }
		public ListBoxVisualClientOwner(PopupBaseEdit editor) : base(editor) {
		}
		public override void SyncProperties(bool syncDataSource) {
			if (!IsLoaded)
				return;
			ListBox.SyncWithOwnerEdit(syncDataSource);
		}
		public override void SyncValues(bool resetTotals = false) {
			if (!IsLoaded)
				return;
			ListBox.SyncValuesWithOwnerEdit(resetTotals);
		}
		public override bool ProcessKeyDownInternal(KeyEventArgs e) {
			if (!IsLoaded)
				return true;
			ListBox.ProcessDownKey(e);
			return true;
		}
		public override object GetSelectedItem() {
			if (!IsLoaded)
				return null;
			return GetSelectedRowKey(ListBox.SelectedItem);
		}
		public override IEnumerable GetSelectedItems() {
			if (!IsLoaded || ListBox.SelectedItems == null)
				return new List<object>();
			return ListBox.SelectedItems.Cast<object>().Select(GetSelectedRowKey);
		}
		protected virtual object GetSelectedRowKey(object item) {
			if (!IsServerMode)
				return item;
			DataProxy proxy = (DataProxy)item;
			return proxy.With(x => x.f_RowKey);
		}
		public override void PopupOpened() {
			base.PopupOpened();
			if (ListBox == null)
				return;
			ListBox.InvalidateMeasure();
		}
		protected override FrameworkElement FindEditor() {
			if (LookUpEditHelper.GetPopupContentOwner(Editor).Child == null)
				return null;
			return LayoutHelper.FindElementByName(LookUpEditHelper.GetPopupContentOwner(Editor).Child, "PART_Content");
		}
		protected override void SetupEditor() {
			if (!IsLoaded)
				return;
			ListBox.SetupEditor();
		}
	}
}
