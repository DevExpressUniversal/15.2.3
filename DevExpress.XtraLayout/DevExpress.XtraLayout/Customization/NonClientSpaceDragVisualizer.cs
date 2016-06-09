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
using System.Text;
using DevExpress.XtraLayout.Handlers;
using DevExpress.Utils.DragDrop;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraLayout.Customization {
	public class NonClientSpaceDragVisualizer {
		[ThreadStatic]
		static NonClientSpaceDragVisualizer _default;
		[ThreadStatic]
		static int refCount = 0;
		bool fEnabledCore = false;
		readonly Size DefaultSize = new Size((int)(80 * Skins.DpiProvider.Default.DpiScaleFactor), (int)(24 * Skins.DpiProvider.Default.DpiScaleFactor));
		public static NonClientSpaceDragVisualizer QueryReference() {
			AddRef();
			return Default;
		}
		public static int AddRef() {
			return ++refCount;
		}
		public static void Release(ref NonClientSpaceDragVisualizer reference) {
			reference = null;
			if(--refCount == 0) {
				if(_default != null) {
					_default.Dispose();
					_default = null;
				}
			}
		}
		static NonClientSpaceDragVisualizer Default {
			get {
				if(_default == null) _default = CreateDefaultInstance();
				return _default;
			}
		}
		protected static NonClientSpaceDragVisualizer CreateDefaultInstance() {
			return new NonClientSpaceDragVisualizer();
		}
		protected NonClientSpaceDragVisualizer() {
			SubscribeDraggingEvents();
		}
		protected void Dispose() {
			UnSubscribeDraggingEvents();
			if(DragDropItemCursor.DefaultExists) {
				DragCursor.Cursor = Cursors.Arrow;
				DragCursor.Visible = false;
			}
		}
		void SubscribeDraggingEvents() {
			DragDropDispatcherFactory.Default.NonClientDragEnter += new OnNonClientSpaceDragEnter(Default_NonClientDragEnter);
			DragDropDispatcherFactory.Default.NonClientDragging += new OnNonClientSpaceDragging(Default_NonClientDragging);
			DragDropDispatcherFactory.Default.NonClientDragLeave += new OnNonClientSpaceDragEnter(Default_NonClientDragLeave);
			DragDropDispatcherFactory.Default.NonClientDrop += new OnNonClientSpaceDrop(Default_NonClientDrop);
		}
		void UnSubscribeDraggingEvents() {
			DragDropDispatcherFactory.Default.NonClientDragEnter -= new OnNonClientSpaceDragEnter(Default_NonClientDragEnter);
			DragDropDispatcherFactory.Default.NonClientDragging -= new OnNonClientSpaceDragging(Default_NonClientDragging);
			DragDropDispatcherFactory.Default.NonClientDragLeave -= new OnNonClientSpaceDragEnter(Default_NonClientDragLeave);
			DragDropDispatcherFactory.Default.NonClientDrop -= new OnNonClientSpaceDrop(Default_NonClientDrop);
		}
		public bool Enable {
			get { return fEnabledCore; }
			set { fEnabledCore = value; }
		}
		protected internal virtual DragDropItemCursor DragCursor {
			get { return DragDropItemCursor.Default; }
		}
		bool AllowProcessEvent {
			get { return Enable; }
		}
		void CustomizeDragCursor() {
			if(DragCursor.Cursor != Cursors.No) DragCursor.Cursor = Cursors.No;
			DragCursor.DragItem = DragDropDispatcherFactory.Default.DragItem;
			UpdateDragCursorSize();
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(DragCursor.Handle, DevExpress.Utils.Drawing.Helpers.MSG.WM_SETCURSOR, DragCursor.Handle, (IntPtr)0x1);
		}
		protected virtual void UpdateDragCursorSize() {
			if(DragCursor.Size != DefaultSize) DragCursor.Size = DefaultSize;
		}
		void Default_NonClientDrop(Point pt) {
			if(!AllowProcessEvent) return;
			if(DragCursor.Visible) DragCursor.Visible = false;
		}
		void Default_NonClientDragging(Point pt) {
			if(!AllowProcessEvent) return;
			MakeDragCursorVisibleAndCustomize();
			UpdateDragCursorLocation(pt);
		}
		protected virtual void UpdateDragCursorLocation(Point pt) {
			DragCursor.Location = pt - new Size(DefaultSize.Width / 2, DefaultSize.Height / 2);
		}
		void MakeDragCursorVisibleAndCustomize() {
			if(!DragCursor.Visible && !RDPHelper.IsRemoteSession && DragCursor.FrameOwner != null) DragCursor.Visible = true;
			CustomizeDragCursor();
		}
		void Default_NonClientDragEnter() {
			if(!AllowProcessEvent) return;
			MakeDragCursorVisibleAndCustomize();
			DragCursor.Update();
		}
		void Default_NonClientDragLeave() {
			if(!AllowProcessEvent) return;
			DragCursor.Cursor = Cursors.Arrow;
			if(DragCursor.Visible) DragCursor.Visible = false;
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(DragCursor.Handle, DevExpress.Utils.Drawing.Helpers.MSG.WM_SETCURSOR, DragCursor.Handle, (IntPtr)0x1);
		}
	}
}
