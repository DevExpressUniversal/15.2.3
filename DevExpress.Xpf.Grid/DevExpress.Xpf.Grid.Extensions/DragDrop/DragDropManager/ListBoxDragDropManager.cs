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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.DragDrop;
using System.ComponentModel;
using System;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Grid {
	[TargetType(typeof(ListBoxEdit))]
	public class ListBoxDragDropManager : DragDropManagerBase {
		ListBoxDropEventHandler DropEventHandler;
		ListBoxDroppedEventHandler DroppedEventHandler;
		ListBoxDragOverEventHandler DragOverEventHandler;
		ListBoxStartDragEventHandler StartDragEventHandler;
		#region inner classes
		public class ListBoxDragSource : SupportDragDropBase {
			protected override FrameworkElement Owner { get { return listBox; } }
			readonly ListBoxEdit listBox;
			public ListBoxDragSource(ListBoxDragDropManager dragDropManager, ListBoxEdit listBox)
				: base(dragDropManager) {
				this.listBox = listBox;
			}
			protected override FrameworkElement SourceElementCore {
				get { return listBox; }
			}
		}
		#endregion
		public ListBoxDragDropManager() { }
		public ListBoxEdit ListBox { get { return (ListBoxEdit)AssociatedObject; } }
		#region Events
		[Category("Events")]
		public event ListBoxDragOverEventHandler DragOver {
			add { DragOverEventHandler += value; }
			remove { DragOverEventHandler -= value; }
		}
		[Category("Events")]
		public event ListBoxDropEventHandler Drop {
			add { DropEventHandler += value; }
			remove { DropEventHandler -= value; }
		}
		[Category("Events")]
		public event ListBoxDroppedEventHandler Dropped {
			add { DroppedEventHandler += value; }
			remove { DroppedEventHandler -= value; }
		}
		[Category("Events")]
		public event ListBoxStartDragEventHandler StartDrag {
			add { StartDragEventHandler += value; }
			remove { StartDragEventHandler -= value; }
		}
		#endregion
		protected internal override IList ItemsSource { get { return ListBox.ItemsSource as IList; } }
		protected internal override void OnDrop(DragDropManagerBase sourceManager, UIElement source, Point pt) {
			if(DropEventIsLocked) return;
			ListBoxDropEventArgs e = RaiseDropEvent(sourceManager);
			if(!e.Handled) {
				if(sourceManager.DraggingRows != null && sourceManager.DraggingRows.Count > 0 && AllowDrop && !ReferenceEquals(this, sourceManager)) {
					foreach(object obj in sourceManager.DraggingRows) {
						object rawObject = sourceManager.GetObject(obj);
						sourceManager.GetSource(obj).Remove(rawObject);
						if(ItemsSource == null)
							throw new InvalidOperationException("The ListBoxEdit should be bound to an items source. Set the ItemsSource property.");
						ItemsSource.Add(rawObject);
					}
				}
			}
			RaiseDroppedEvent(sourceManager, e.DraggedRows);
			base.OnDrop(sourceManager, source, pt);
		}
		protected virtual void RaiseDroppedEvent(DragDropManagerBase sourceManager, IList draggedRows) {
			if(DroppedEventHandler != null) {
				ListBoxDroppedEventArgs e = new ListBoxDroppedEventArgs(ListBox, this, sourceManager, draggedRows);
				DroppedEventHandler(this, e);
			}
		}
		protected virtual ListBoxDropEventArgs RaiseDropEvent(DragDropManagerBase sourceManager) {
			ListBoxDropEventArgs e = new ListBoxDropEventArgs(ListBox, this, sourceManager, sourceManager.DraggingRows) {
				Handled = false,
			};
			if(DropEventHandler != null)
				DropEventHandler(this, e);
			return e;
		}
		protected internal override IList CalcDraggingRows(IndependentMouseEventArgs e) {
			ListBoxItem currentItem = FindListBoxItem(e.GetPosition(ListBox));
			if(mouseDownItem == null || (currentItem != null && mouseDownItem != currentItem))
				return null;
			List<object> list = new List<object>();
			IList<object> selectedItems = SelectedItemsCache;
			if(selectedItems.Contains(mouseDownItem.Content))
				list.AddRange(selectedItems);
			else
				list.Add(mouseDownItem.Content);
			return list;
		}
		ListBoxItem FindListBoxItem(Point p) {
			HitTestResult hitTestResult = VisualTreeHelper.HitTest(ListBox, p);
			return hitTestResult != null ? LayoutHelper.FindParentObject<ListBoxItem>(hitTestResult.VisualHit) : null;
		}
		[Browsable(false)]
		public override void OnDragOver(DragDropManagerBase sourceManager, UIElement source, Point pt) {
			base.OnDragOver(sourceManager, source, pt);
			ListBoxDragOverEventArgs e = RaiseDragOverEvent(sourceManager, pt, DropTargetType.None);
			sourceManager.SetDragInfoVisibility(e.ShowDragInfo);
			sourceManager.SetDropMarkerVisibility(e.ShowDropMarker);
			DropEventIsLocked = e.Handled ? !e.AllowDrop : !AllowDrop || sourceManager.DraggingRows == null || sourceManager.DraggingRows.Count < 1 || ReferenceEquals(this, sourceManager);
			if(!DropEventIsLocked) {
				sourceManager.SetDropTargetType(DropTargetType.DataArea);
				sourceManager.ShowDropMarker(ListBox, TableDragIndicatorPosition.None);
			}
		}
		protected virtual ListBoxDragOverEventArgs RaiseDragOverEvent(DragDropManagerBase sourceManager, Point pt, DropTargetType dropTargetType) {
			ListBoxDragOverEventArgs e = new ListBoxDragOverEventArgs(ListBox, this, sourceManager, sourceManager.DraggingRows) {
				Handled = false,
			};
			if(DragOverEventHandler != null)
				DragOverEventHandler(this, e);
			return e;
		}
		protected override void OnAttached() {
			base.OnAttached();
			if(ListBox != null) {
				DragDropManagerBase.SetDragDropManager(ListBox, this);
				DragDropHelper = new RowDragDropElementHelper(new ListBoxDragSource(this, ListBox));
				DragManager.SetDropTargetFactory(ListBox, new DragDropManagerDropTargetFactory());
#if !SL
				ListBox.PreviewMouseDown += new MouseButtonEventHandler(OnListBoxPreviewMouseDown);
#endif
			}
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			if(ListBox != null)
				ListBox.PreviewMouseDown -= OnListBoxPreviewMouseDown;
			SelectedItemsCache = null;
			mouseDownItem = null;
		}
		IList<object> SelectedItemsCache;
		ListBoxItem mouseDownItem;
		void OnListBoxPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			mouseDownItem = FindListBoxItem(e.GetPosition(ListBox));
			SelectedItemsCache = ListBox.SelectedItems.ToList<object>();
		}
		protected internal override bool CustomAllowDrag(IndependentMouseEventArgs e) {
			DraggingRows = CalcDraggingRows(e);
			StartDragEventArgs startDragArgs = RaiseStartDragEvent(e);
			return startDragArgs.CanDrag;
		}
		protected internal override StartDragEventArgs RaiseStartDragEvent(IndependentMouseEventArgs e) {
			ListBoxStartDragEventArgs startDragArgs = new ListBoxStartDragEventArgs(ListBox, this) {
				CanDrag = true,
			};
			if(StartDragEventHandler != null)
				StartDragEventHandler(this, startDragArgs);
			return startDragArgs;
		}
		protected override bool NeedAdornerLogicalOwner { get { return false; } }
		protected override bool CanShowDropMarker() {
			return !ListBox.RenderSize.IsZero();
		}
		protected internal override bool CanStartDrag(MouseButtonEventArgs e) {
			return true;
		}
		protected internal override void OnDragLeave() {
			base.OnDragLeave();
			SelectedItemsCache = null;
			mouseDownItem = null;
		}
	}
}
