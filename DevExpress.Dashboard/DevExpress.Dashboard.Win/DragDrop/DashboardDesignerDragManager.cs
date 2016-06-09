#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils.DragDrop;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Controls;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.DragDrop {
	public interface IDragSource {
		bool AllowNullDrop { get; }
		IHistoryItem PerformDrag(IDragObject dragObject, bool isSameDragGroup);
		void Cancel();
	}
	public interface IDropAction {
		void ShowIndicator(bool visible);
		IHistoryItem PerformDrop();
	}
	public interface IDropDestination {
		Rectangle ScreenBounds { get; }
		IDropAction GetDropAction(IDragObject dragObject, Point screenPt);
	}
	public class DashboardDesignerDragManager : IDisposable {
		readonly DragWindow dragWindow = new DragWindow();
		readonly List<IDropDestination> dropTargets = new List<IDropDestination>();
		DashboardDesigner dashboardDesigner;
		IDropDestination activeDropTarget;
		IDropAction activeDropAction;
		IDragSource dragSource;
		IDragObject dragObject;
		bool isDisposed;
		protected IDragSource DragSource { get { return dragSource; } }
		internal DragWindow DragWindow { get { return dragWindow; } }
		protected IDragObject DragObject { get { return dragObject; } }
		protected IDropAction ActiveDropAction {
			get { return activeDropAction; }
			set {
				if(!object.Equals(value, activeDropAction)) {
					if(activeDropAction != null)
						activeDropAction.ShowIndicator(false);
					activeDropAction = value;
					if(activeDropAction != null)
						activeDropAction.ShowIndicator(true);
				}
			}
		}
		public DashboardDesignerDragManager(DashboardDesigner dashboardDesigner) {
			this.dashboardDesigner = dashboardDesigner;
			dragWindow.DragStarted += OnDragStarted;
			dragWindow.DragEnded += OnDragEnded;
			Reset();
		}
		void Reset() {
			activeDropTarget = null;
			activeDropAction = new SelfDropAction(null);
			dragSource = null;
			dragObject = null;
		}
		void OnDragStarted(object sender, EventArgs e) {
			dragWindow.LocationChanged += OnLocationChanged;
		}
		void OnDragEnded(object sender, DragCompleteEventArgs e) {
			dragWindow.LocationChanged -= OnLocationChanged;
			if(!e.IsDragCancelled) {
				IHistoryItem historyItem;
				if(activeDropAction != null) {
					activeDropAction.ShowIndicator(false);
					historyItem = activeDropAction.PerformDrop();
				}
				else
					historyItem = dragSource.PerformDrag(dragObject, false);
				if(historyItem != null) {
					bool success = true;
					try {
						historyItem.Redo(dashboardDesigner);						
					}
					catch (InvalidOperationException) { 
						success = false;
					}   
					if(success)
						dashboardDesigner.History.Add(historyItem);
				}
			}
			Reset();
		}
		void OnLocationChanged(object sender, EventArgs e) {
			UpdateDropAction();
		}
		internal void UpdateDropAction() {
			Point screenPt = dragWindow.DragLocation;
			activeDropTarget = dropTargets.Find(dropTarget => dropTarget.ScreenBounds.Contains(screenPt));
			ActiveDropAction = activeDropTarget != null ? activeDropTarget.GetDropAction(dragObject, screenPt) : null;
			UpdateCursor();
		}
		void UpdateCursor() {
			if(activeDropAction != null)
				Cursor.Current = dragObject.IsGroup ? Cursors.SizeAll : Cursors.Default;
			else
				Cursor.Current = dragSource.AllowNullDrop ? DragManager.DragRemoveCursor : Cursors.No;
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if (dragWindow != null) {
					dragWindow.Dispose();
					dragWindow.DragStarted -= OnDragStarted;
					dragWindow.DragEnded -= OnDragEnded;
				}
				dashboardDesigner = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		public void RegisterDropTarget(IDropDestination dropTarget) {
			dropTargets.Add(dropTarget);
		}
		public void StartDrag(IDragObject dragObject, Bitmap dragBitmap, Point startScreenPt, Size offset) {
			if(isDisposed)
				throw new Exception("DragManager is already disposed");
			if(dragWindow.IsDragging)
				throw new Exception("Dragging is still active. You should stop previous dragging operation first.");
			this.dragSource = dragObject.DragSource;
			this.dragObject = dragObject;
			activeDropAction = new SelfDropAction(dragObject);
			dragWindow.StartDrag(startScreenPt, dragBitmap, offset);
		}
	}
}
