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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class PopupTreeListEditVisualClient : VisualClientOwner {
		bool ShouldSyncProperties { get { return IsLoaded && Editor.EditMode != EditMode.InplaceInactive; } }
		internal GridControl Grid { get { return InnerEditor as GridControl; } }
		TreeListView View { get { return (TreeListView)GridControl.GetActiveView(Grid); } }
		PopupTreeListEditStyleSettings StyleSettings { get { return (PopupTreeListEditStyleSettings)ActualPropertyProvider.GetProperties(Editor).StyleSettings; } }
		new PopupTreeListEdit Editor { get { return (PopupTreeListEdit)base.Editor; } }
		[ThreadStatic]
		static ReflectionHelper reflectionHelper;
		static ReflectionHelper ReflectionHelper {
			get {
				return reflectionHelper ?? (reflectionHelper = new ReflectionHelper());
			}
		}
		public PopupTreeListEditVisualClient(PopupTreeListEdit editor) : base(editor) {
		}
		protected override FrameworkElement FindEditor() {
			if(LookUpEditHelper.GetPopupContentOwner(Editor).Child == null)
				return null;
			return LayoutHelper.FindElementByName(LookUpEditHelper.GetPopupContentOwner(Editor).Child, "PART_GridControl");
		}
		object GetSelectedValue() {
			var editSettings = (PopupTreeListEditSettings)((IBaseEdit)Editor).Settings;
			return editSettings.ItemsProvider.GetValue(GetSelectedItem());
		}
		public override object GetSelectedItem() {
			return Grid.CurrentItem;
		}
		public override IEnumerable GetSelectedItems() {
			throw new NotSupportedException();
		}
		public override bool ProcessKeyDownInternal(KeyEventArgs e) {
			if(e.Handled)
				return true;
			if(IsPopupInAStateThatAllowsClosingOrCancelling() && e.Key == Key.Escape) {
				Editor.CancelPopup();
				e.Handled = true;
				return true;
			}
			if(IsPopupInAStateThatAllowsClosingOrCancelling() && e.Key == Key.Enter) {
				if(CanAcceptValue(GetSelectedValue())) {
					Editor.ClosePopup();
					e.Handled = true;
					return true;
				}
			}
			return false;
		}
		bool IsPopupInAStateThatAllowsClosingOrCancelling() {
			return Editor.IsPopupOpen && View != null && !View.IsContextMenuOpened && !View.IsColumnFilterOpened;
		}
		protected override void SubscribeEvents() {
			if(Grid == null)
				return;
			Grid.MouseLeftButtonUp += Grid_MouseLeftButtonUp;
			base.SubscribeEvents();
		}
		protected override void UnsubscribeEvents() {
			if(Grid == null)
				return;
			Grid.MouseLeftButtonUp -= Grid_MouseLeftButtonUp;
			base.UnsubscribeEvents();
		}
		void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			if(e.LeftButton != MouseButtonState.Released)
				return;
			Debug.Assert(StyleSettings.GetPopupFooterButtons(Editor) != PopupFooterButtons.OkCancel);
			if(CanAcceptValue(GetSelectedValue())) {
				Editor.ClosePopup();
			}
		}
		bool CanAcceptValue(object value) {
			if(Editor.PopupSelectionValidator == null)
				return true;
			return Editor.PopupSelectionValidator.CanSelectValue(value);
		}
		protected override void SetupEditor() {
			if(Grid == null)
				return;
			UpdateViewProperty(TreeListView.ChildNodesPathProperty, Editor.ChildNodesPath);
			Grid.Columns.Add(new GridColumn() { FieldName = Editor.PopupDisplayMember, CellTemplate = Editor.TreeListCellTemplate });
			SyncProperties(true);
		}
		protected void UpdateViewProperty(DependencyProperty property, object value) {
			UpdateProperty(View, property, value);
		}
		void UpdateProperty(DependencyObject obj, DependencyProperty property, object value) {
			ValueSource source = System.Windows.DependencyPropertyHelper.GetValueSource(obj, property);
			if(source.BaseValueSource == BaseValueSource.Unknown || source.BaseValueSource == BaseValueSource.Default) {
				obj.SetValue(property, value);
			}
		}
		public override void SyncProperties(bool syncDataSource) {
			if(!ShouldSyncProperties)
				return;
			if(syncDataSource)
				SetupDataSource();
			SyncValues();
			Dispatcher.BeginInvoke(new Action(() => { if(IsLoaded) Grid.InvalidateMeasure(); }));
			SyncSelectedItems(syncDataSource);
		}
		void SetupDataSource() {
			object dataSource = Editor.ItemsSource;
			if(Grid.ItemsSource != dataSource) {
				var valueSource = System.Windows.DependencyPropertyHelper.GetValueSource(Grid, DataControlBase.ItemsSourceProperty).BaseValueSource;
				if(valueSource == BaseValueSource.Default || valueSource == BaseValueSource.Local) {
					Grid.ItemsSource = dataSource;
				}
			}
		}
		void SyncSelectedItems(bool updateTotals) {
			HierarchicalDataLocator locator = new HierarchicalDataLocator(View.Nodes, GetTreeListNodeValueMemberPath(), "Nodes", "IsExpanded", Editor.HierarchicalPathProvider);
			TreeListNode node = (TreeListNode)locator.FindItemByValue(Editor.EditValue);
			if(node != null) {
				View.FocusedRowHandle = node.RowHandle;
			}
		}
		string GetTreeListNodeValueMemberPath() {
			return string.IsNullOrEmpty(Editor.ValueMember) ? "Content" : "Content." + Editor.ValueMember;
		}
	}
}
