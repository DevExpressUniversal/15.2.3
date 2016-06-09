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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
namespace DevExpress.Xpf.Editors {
	public interface ISelectorEditPropertyProvider {
		SelectionViewModel SelectionViewModel { get; }
	}
	public class ListBoxEditBasePropertyProvider : ActualPropertyProvider, ISelectorEditPropertyProvider {
		public static readonly DependencyProperty DisplayMemberPathProperty;
		public static readonly DependencyProperty SelectionViewModelProperty;
		public static readonly DependencyProperty ShowWaitIndicatorProperty;
		static ListBoxEditBasePropertyProvider() {
			Type ownerType = typeof(ListBoxEditBasePropertyProvider);
			SelectionViewModelProperty = DependencyPropertyManager.Register("SelectionViewModel", typeof(SelectionViewModel), ownerType,
				new FrameworkPropertyMetadata(null));
			DisplayMemberPathProperty = DependencyPropertyManager.Register("DisplayMemberPath", typeof(string), ownerType);
			ShowWaitIndicatorProperty = DependencyPropertyManager.Register("ShowWaitIndicator", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
		}
		public bool ShowWaitIndicator {
			get { return (bool)GetValue(ShowWaitIndicatorProperty); }
			set { SetValue(ShowWaitIndicatorProperty, value); }
		}
		public SelectionEventMode SelectionEventMode { get { return StyleSettings.GetSelectionEventMode((ISelectorEdit)Editor); } }
		public string DisplayMemberPath {
			get { return (string)GetValue(DisplayMemberPathProperty); }
			set { SetValue(DisplayMemberPathProperty, value); }
		}
		public SelectionViewModel SelectionViewModel {
			get { return (SelectionViewModel)GetValue(SelectionViewModelProperty); }
			set { SetValue(SelectionViewModelProperty, value); }
		}
		new ListBoxEditStyleSettings StyleSettings { get { return (ListBoxEditStyleSettings)base.StyleSettings; } }
		new ListBoxEdit Editor { get { return (ListBoxEdit)base.Editor; } }
		public ListBoxEditBasePropertyProvider(BaseEdit editor)
			: base(editor) {
			SelectionViewModel = new SelectionViewModel(() => Editor.ListBoxCore);
		}
		public bool ShowCustomItems() {
			return StyleSettings.ShowCustomItem(Editor);
		}
	}
	public class LookUpEditBasePropertyProvider : PopupBaseEditPropertyProvider, ISelectorEditPropertyProvider {
		static int currentHandle;
		static readonly object locker = new object();
		public object IncrementalFilteringHandle { get; private set; }
		public object TokenEditorDataViewHandle { get; set; }
		public static readonly DependencyProperty SelectionViewModelProperty;
		public static readonly DependencyProperty ShowWaitIndicatorProperty;
		static LookUpEditBasePropertyProvider() {
			Type ownerType = typeof(LookUpEditBasePropertyProvider);
			SelectionViewModelProperty = DependencyPropertyManager.Register("SelectionViewModel", typeof(SelectionViewModel), ownerType,
				new FrameworkPropertyMetadata(null));
			ShowWaitIndicatorProperty = DependencyPropertyManager.Register("ShowWaitIndicator", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
		}
		public bool ShowWaitIndicator {
			get { return (bool)GetValue(ShowWaitIndicatorProperty); }
			set { SetValue(ShowWaitIndicatorProperty, value); }
		}
		public SelectionViewModel SelectionViewModel {
			get { return (SelectionViewModel)GetValue(SelectionViewModelProperty); }
			set { SetValue(SelectionViewModelProperty, value); }
		}
		public bool ApplyItemTemplateToSelectedItem { get; private set; }
		public SelectionMode SelectionMode { get { return StyleSettings.GetSelectionMode(Editor); } }
		public FindMode FindMode { get { return Editor.FindMode ?? StyleSettings.GetFindMode(Editor); } }
		public FilterCondition FilterCondition { get { return Editor.FilterCondition ?? StyleSettings.FilterCondition; } }
		public FilterByColumnsMode FilterByColumnsMode { get { return StyleSettings.FilterByColumnsMode; } }
		public bool EnableTokenWrapping { get { return StyleSettings.GetEnableTokenWrapping(); } }
		public bool IsSingleSelection { get { return SelectionMode == SelectionMode.Single; } }
		new LookUpEditBase Editor { get { return (LookUpEditBase)base.Editor; } }
		new BaseLookUpStyleSettings StyleSettings { get { return (BaseLookUpStyleSettings)base.StyleSettings; } }
		public bool IsServerMode { get { return Editor.ItemsProvider.IsAsyncServerMode || Editor.ItemsProvider.IsSyncServerMode; } }
		public bool IncrementalFiltering { get { return Editor.IncrementalFiltering ?? StyleSettings.GetIncrementalFiltering(); } }
		public LookUpEditBasePropertyProvider(TextEdit editor)
			: base(editor) {
			SelectionViewModel = new SelectionViewModel(() => (ISelectorEditInnerListBox)LookUpEditHelper.GetVisualClient(Editor).InnerEditor);
			IncrementalFilteringHandle = GenerateHandle();
		}
		public object GenerateHandle() {
			lock (locker) {
				return currentHandle++;
			}
		}
		public void SetApplyItemTemplateToSelectedItem(bool value) {
			ApplyItemTemplateToSelectedItem = value;
		}
		public override EditorPlacement GetFindButtonPlacement() {
			return Editor.FindButtonPlacement ?? StyleSettings.GetFindButtonPlacement(Editor);
		}
		public override EditorPlacement GetAddNewButtonPlacement() {
			return Editor.AddNewButtonPlacement ?? StyleSettings.GetAddNewButtonPlacement(Editor);
		}
		public override EditorPlacement GetNullValueButtonPlacement() {
			return Editor.NullValueButtonPlacement ?? StyleSettings.GetNullValueButtonPlacement(Editor);
		}
		protected internal virtual IEnumerable<string> GetHighlightedColumns() {
			return new List<string> { Editor.DisplayMember };
		}
		public override bool CalcSuppressFeatures() {
			return !ApplyItemTemplateToSelectedItem && base.CalcSuppressFeatures();
		}
		public virtual bool ShowCustomItems() {
			return StyleSettings.ShowCustomItem(Editor);
		}
	}
}
