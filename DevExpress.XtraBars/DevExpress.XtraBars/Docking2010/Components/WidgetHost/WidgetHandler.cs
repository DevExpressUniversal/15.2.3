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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Mdi;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IWidgetsHostHandler {
		bool DragMode { get; }
		bool SizeMode { get; }
		void ProcessMove(Document document);
		void ProcessMoving(WidgetContainer container);
		void ProcessEnterSizeMove(WidgetContainer container);
		void ProcessExitSizeMove(WidgetContainer container);
		void ProcessWindowPosChanging(WidgetContainer container, IntPtr lParam);
		void ProcessWindowPosChanged(WidgetContainer container, IntPtr lParam);
		bool ProcessSysCommand(Document document, WidgetContainer widgetContainer, ref Message m);
		void UpdateDragService();
	}
	public enum WidgetsHostHandlerState { Normal, DragMode, SizeMode }
	public class WidgetsHostHandler : IWidgetsHostHandler {
		WidgetsHost hostCore;
		IWidgetsHostDragService dragServiceCore;
		IWidgetsHostSizeServise sizeServiceCore;
		public WidgetsHostHandler(WidgetsHost host) {
			hostCore = host;
			sizeServiceCore = CreateSizeService();
			dragServiceCore = CreateDragService();
		}
		IWidgetsHostSizeServise CreateSizeService() {
			return new TableLayoutSizeService(Host);
		}
		IWidgetsHostDragService CreateDragService() {
			if(View.LayoutMode == LayoutMode.StackLayout)
				return new StackLayoutDragService(Host);
			if(View.LayoutMode == LayoutMode.FlowLayout)
				return new FlowLayoutDragService(Host);
			return new TableLayoutDragService(Host);
		}
		public void UpdateDragService() {
			dragServiceCore = CreateDragService();
		}
		public WidgetsHost Host { get { return hostCore; } }
		public DocumentManager Manager { get { return (DocumentManager)hostCore.Owner; } }
		public WidgetsHostHandlerState State { get; internal set; }
		public WidgetView View { get { return Manager.View as WidgetView; } }
		public Document DragItem { get; internal set; }
		public IWidgetsHostDragService DragService {
			get { return dragServiceCore; }
		}
		public IWidgetsHostSizeServise SizeService {
			get { return sizeServiceCore; }
		}
		public virtual bool DragMode {
			get { return GetState(WidgetsHostHandlerState.DragMode); }
			set { SetState(WidgetsHostHandlerState.DragMode, value); }
		}
		public virtual bool SizeMode {
			get { return GetState(WidgetsHostHandlerState.SizeMode); }
			set { SetState(WidgetsHostHandlerState.SizeMode, value); }
		}
		bool GetState(WidgetsHostHandlerState state) {
			return (State & state) != 0;
		}
		void SetState(WidgetsHostHandlerState state, bool value) {
			if(value)
				State |= state;
			else
				State &= ~state;
		}
		public void ProcessWindowPosChanging(WidgetContainer container, IntPtr lParam) {
			WinAPI.WINDOWPOS pos = (WinAPI.WINDOWPOS)BarNativeMethods.PtrToStructure(lParam, typeof(WinAPI.WINDOWPOS));
			Rectangle position = pos.ToRectangle();
			BoundsAnimationInfo info = XtraAnimator.Current.Get(Host, container) as BoundsAnimationInfo;
			if(container.Document == null || container.Document.Manager == null) return;
			if(container.Document.IsMaximized &&
				(Host.ClientSize.Width != position.Width || Host.ClientSize.Height != position.Height) &&
				(info != null && (info.CurrentBounds.Width != position.Width))) {
				position.X = 0;
				position.Y = 0;
				position.Width = Host.ClientSize.Width;
				position.Height = Host.ClientSize.Height;
				pos.RestoreFromRectangle(position);
				BarNativeMethods.StructureToPtr(pos, lParam, false);
				return;
			}
			if((info != null && (info.CurrentBounds.Width != position.Width || info.CurrentBounds.Height != position.Height)) && View.AllowDocumentStateChangeAnimation == DefaultBoolean.True) {
				position.X = info.CurrentBounds.X;
				position.Y = info.CurrentBounds.Y;
				position.Width = info.CurrentBounds.Width;
				position.Height = info.CurrentBounds.Height;
				pos.RestoreFromRectangle(position);
				BarNativeMethods.StructureToPtr(pos, lParam, false);
				return;
			}
			if(container.Info == null) return;
			Size minSize = container.Info.GetNCMinSize();
			if(position.Height <= minSize.Height) {
				position.Height = minSize.Height;
			}
			if(position.Width <= minSize.Width) {
				position.Width = minSize.Width;
			}
			if(View.LayoutMode == LayoutMode.TableLayout && SizeMode) {
				SizeService.OnSizing(container, ref position);
				Host.Invalidate();
			}
			pos.RestoreFromRectangle(position);
			BarNativeMethods.StructureToPtr(pos, lParam, false);
		}
		public void ProcessWindowPosChanged(WidgetContainer container, IntPtr lParam) {
			if(lParam == IntPtr.Zero || container.Document == null) return;
			WinAPI.WINDOWPOS pos = (WinAPI.WINDOWPOS)BarNativeMethods.PtrToStructure(lParam, typeof(WinAPI.WINDOWPOS));
			if(!SizeMode) return;
			container.Document.SetBoundsCore(new Rectangle(pos.x, pos.y, pos.cx, pos.cy));
			View.ViewInfo.SetDirty();
			using(Image image = new Bitmap(1, 1)) {
				using(Graphics g = Graphics.FromImage(image)) {
					View.ViewInfo.Calc(g, Host.ClientRectangle);
				}
			}
			UpdateSizes(container.Document as Document);
		}
		void UpdateSizes(Document document) {
			if(View.LayoutMode == LayoutMode.TableLayout) return;
			if(View.LayoutMode == LayoutMode.FlowLayout) {
				foreach(var item in View.FlowLayoutGroup.Items) {
					if(!item.IsVisible) continue;
					item.Control.Parent.Bounds = item.Info.Bounds;
				}
			}
			if(document.Parent == null) return;
			foreach(var item in document.Parent.Items) {
				if(!item.IsVisible) continue;
				item.Control.Parent.Bounds = item.Info.Bounds;
			}
		}
		bool RequestToEnterDragMode;
		public bool ProcessSysCommand(Document document, WidgetContainer widgetContainer, ref Message m) {
			if((WinAPIHelper.GetInt(m.WParam) & 0xFFF0) == DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_CLOSE) {
				if(Host.MaximizedContainer == widgetContainer)
					UpdateMaximizedContainer(null);
				if(Manager.View.Controller.Close(document))
					m.Result = IntPtr.Zero;
				return false;
			}
			if((WinAPIHelper.GetInt(m.WParam) & 0xFFF0) == DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MOVE) {
				RequestToEnterDragMode = true;
			}
			if((WinAPIHelper.GetInt(m.WParam) & 0xFFF0) == DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_SIZE) {
				SizeMode = true;
				Host.LockUpdateLayout();
			}
			switch(Docking2010.WinAPIHelper.GetInt(m.WParam)) {
				case 0xF032:
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MAXIMIZE:
					if(!document.CanMaximize()) return true;
					UpdateMaximizedContainer(widgetContainer);
					widgetContainer.SetMaximized(true);
					break;
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_MINIMIZE:
					Host.HorizontalScroll.Enabled = true;
					Host.VerticalScroll.Enabled = true;
					break;
				case 0xF122:
				case DevExpress.Utils.Drawing.Helpers.NativeMethods.SC.SC_RESTORE:
					if(Host.MaximizedContainer != null && Host.MaximizedContainer == widgetContainer)
						UpdateMaximizedContainer(null);
					widgetContainer.SetMaximized(false);
					break;
			}
			return true;
		}
		void UpdateMaximizedContainer(WidgetContainer widgetContainer) {
			Host.HorizontalScroll.Enabled = widgetContainer == null;
			Host.VerticalScroll.Enabled = widgetContainer == null;
			Host.MaximizedContainer = widgetContainer;
		}
		public void OnEndDragging() {
			DragMode = false;
			DragItem = null;
		}
		public void ProcessMove(Document document) {
			if(!DragMode) return;
			if(DragItem != document) return;
			if(document.IsMaximized) {
				UpdateMaximizedContainer(null);
				(document.Control.Parent as WidgetContainer).SetMaximized(false, false);
			}
			Point p = Host.PointToClient(Cursor.Position);
			MouseEventArgs args = new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 0, p.X, p.Y, 0);
			View.RaiseDragging(document, args);
			DragService.OnDragging(document, args);
		}
		public void ProcessMoving(WidgetContainer container) { }
		public void ProcessEnterSizeMove(WidgetContainer container) {
			Host.LockUpdateLayout();
			if(RequestToEnterDragMode && !SizeMode) {
				DragMode = true;
				View.RaiseBeginDragging(container.Document as Document);
				DragItem = container.Document as Document;
				DragService.OnBegin(container.Document as Document);
				container.BringToFront();
				RequestToEnterDragMode = false;
			}
			else SizeMode = true;
		}
		public void ProcessExitSizeMove(WidgetContainer container) {
			if(DragMode) {
				OnEndDragging();
				DragService.OnDrop(container.Document);
				View.RaiseEndDragging(container.Document);
			}
			if(SizeMode) {
				if(View.LayoutMode == LayoutMode.TableLayout) {
					SizeService.OnEndSizing(container.Document);
					Host.UpdateLayout();
				}
				SizeMode = false;
				Host.UnlockUpdateLayout();
			}
		}
	}
}
