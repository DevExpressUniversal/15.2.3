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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System.IO;
using System.Windows.Resources;
using DevExpress.Xpf.Utils;
using System.Linq;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public class HideClosingItemsBehavior : Behavior<DockLayoutManager> {
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.DockItemClosing += OnDockItemClosing;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.DockItemClosing -= OnDockItemClosing;
		}
		void OnDockItemClosing(object sender, Docking.Base.ItemCancelEventArgs e) {
			e.Item.Visibility = Visibility.Collapsed;
			e.Cancel = true;
		}
	}
	public class CursorAttachedBehavior : Behavior<DXScrollViewer> {
		readonly TimeSpan UpdateCursorTimeSpan = TimeSpan.FromMilliseconds(1000);
		public static readonly DependencyProperty CursorModeProperty = DependencyPropertyManager.Register("CursorMode", typeof(CursorModeType), typeof(CursorAttachedBehavior),
			new PropertyMetadata(CursorModeType.SelectTool, (obj, args) => ((CursorAttachedBehavior)obj).OnCursorModeChanged((CursorModeType)args.NewValue)));
		public CursorModeType CursorMode {
			get { return (CursorModeType)GetValue(CursorModeProperty); }
			set { SetValue(CursorModeProperty, value); }
		}
		WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler> PreviewKeyDownHandler { get; set; }
		WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler> PreviewKeyUpHandler { get; set; }
		DispatcherTimer UpdateCursorTimer { get; set; }
		FrameworkElement CursorRoot { get; set; }
		DocumentPreviewControl PreviewRoot { get; set; }
		public CursorAttachedBehavior() {
			PreviewKeyDownHandler = new WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler>(this,
				(behavior, sender, args) => behavior.PreviewKeyDownHandlerInternal(args),
				(h, sender) => ((FrameworkElement)sender).PreviewKeyDown -= h.Handler,
				h => h.OnEvent);
			PreviewKeyUpHandler = new WeakEventHandler<CursorAttachedBehavior, KeyEventArgs, KeyEventHandler>(this,
				(behavior, sender, args) => behavior.PreviewKeyUpHandlerInternal(args),
				(h, sender) => ((FrameworkElement)sender).PreviewKeyUp -= h.Handler,
				h => h.OnEvent);
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateCursorTimer = new DispatcherTimer(UpdateCursorTimeSpan, DispatcherPriority.Normal, UpdateCursorTick, AssociatedObject.Dispatcher);
			UpdateCursorTimer.Stop();
			((PageSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
			((PageSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
			((PageSelector)AssociatedObject.TemplatedParent).PreviewMouseMove += OnPreviewMouseMove;
			((PageSelector)AssociatedObject.TemplatedParent).MouseEnter += OnMouseEnter;
			((PageSelector)AssociatedObject.TemplatedParent).MouseLeave += OnMouseLeave;
			CursorRoot = LayoutHelper.FindRoot(AssociatedObject) as FrameworkElement;
			CursorRoot.Do(x => x.PreviewKeyDown += PreviewKeyDownHandler.Handler);
			CursorRoot.Do(x => x.PreviewKeyUp += PreviewKeyUpHandler.Handler);
			PreviewRoot = LayoutHelper.FindParentObject<DocumentPreviewControl>(AssociatedObject);
		}
		void UpdateCursorTick(object sender, EventArgs e) {
			if(!ModifierKeysHelper.IsCtrlPressed(Keyboard.Modifiers)) {
				UpdateCursorTimer.Stop();
				SetDefaultCursor();
			}
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			((PageSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
			((PageSelector)AssociatedObject.TemplatedParent).PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
			((PageSelector)AssociatedObject.TemplatedParent).PreviewMouseMove -= OnPreviewMouseMove;
			((PageSelector)AssociatedObject.TemplatedParent).MouseEnter -= OnMouseEnter;
			((PageSelector)AssociatedObject.TemplatedParent).MouseLeave -= OnMouseLeave;
			CursorRoot.Do(x => x.PreviewKeyDown -= PreviewKeyDownHandler.Handler);
			CursorRoot.Do(x => x.PreviewKeyUp -= PreviewKeyUpHandler.Handler);
			UpdateCursorTimer.Stop();
		}
		protected virtual void OnCursorModeChanged(CursorModeType newValue) {
			SetDefaultCursor();
		}
		void PreviewKeyDownHandlerInternal(KeyEventArgs args) {
			SetDefaultCursor();
			UpdateCursorTimer.Stop();
			UpdateCursorTimer.Start();
		}
		void PreviewKeyUpHandlerInternal(KeyEventArgs args) {
			SetDefaultCursor();
			UpdateCursorTimer.Stop();
		}
		void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			SetDefaultCursor(e);
		}
		void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			SetDragCursor(e);
		}
		void OnPreviewMouseMove(object sender, MouseEventArgs e) {
			if(CursorMode == CursorModeType.SelectTool)
				SetSelectToolModeCursor(e.GetPosition(PreviewRoot));
		}
		Brick GetHitTestBrick() {
			var point = Mouse.GetPosition(PreviewRoot.DocumentPresenter);
			return PreviewRoot.DocumentPresenter.NavigationStrategy.GetBrick(point);
		}
		void SetSelectToolModeCursor(Point p) {
			if(PreviewRoot == null || !PreviewRoot.Document.Return(x => x.IsLoaded, () => false) || !PreviewRoot.Document.Pages.Any())
				return;
			var hittest = PreviewRoot.InputHitTest(p);
			if(hittest == AssociatedObject && Mouse.LeftButton == MouseButtonState.Pressed && PreviewRoot.DocumentPresenter.SelectionService.CanSelect) {
				CursorHelper.SetCursor(AssociatedObject, PreviewCursors.Cross);
				return;
			}
			var brick = GetHitTestBrick();
			if(brick != null && !string.IsNullOrEmpty(brick.Url)) {
				CursorHelper.SetCursor(AssociatedObject, Cursors.Hand);
				return;
			}
			CursorHelper.SetCursor(AssociatedObject, Cursors.Arrow);
		}
		void OnMouseLeave(object sender, MouseEventArgs e) {
			if(MouseHelper.IsMouseLeftButtonPressed(e))
				return;
			AssociatedObject.Cursor = Cursors.Arrow;
		}
		void OnMouseEnter(object sender, MouseEventArgs e) {
			if(MouseHelper.IsMouseLeftButtonPressed(e))
				return;
			SetDefaultCursor(e);
		}
		void SetDefaultCursor() {
			SetDefaultCursor(null);
		}
		void SetDefaultCursor(MouseEventArgs e) {
			switch(CursorMode) {
				case CursorModeType.HandTool:
					CursorHelper.SetCursor(AssociatedObject, PreviewCursors.Hand);
					break;
				case CursorModeType.SelectTool:
					var point = e != null ? e.GetPosition(PreviewRoot) : new Point(0, 0);
					SetSelectToolModeCursor(point);
					break;
			}
		}
		void SetDragCursor(MouseEventArgs e) {
			switch(CursorMode) {
				case CursorModeType.HandTool:
					CursorHelper.SetCursor(AssociatedObject, PreviewCursors.HandDrag);
					break;
				case CursorModeType.SelectTool:
					var point = e != null ? e.GetPosition(PreviewRoot) : new Point(0, 0);
					SetSelectToolModeCursor(point);
					break;
			}
		}
	}
	public enum PreviewCursors {
		Hand,
		HandDrag,
		Cross
	}
	public static class CursorHelper {
		static Cursor HandCursor { get; set; }
		static Cursor HandDragCursor { get; set; }
		static Cursor CrossCursor { get; set; }
		public static void SetCursor(FrameworkElement element, Cursor cursor) {
			element.Do(x => x.Cursor = cursor);
		}
		public static void SetCursor(FrameworkElement element, PreviewCursors cursor) {
			switch(cursor) {
				case PreviewCursors.Hand:
					SetCursor(element, HandCursor);
					break;
				case PreviewCursors.HandDrag:
					SetCursor(element, HandDragCursor);
					break;
				case PreviewCursors.Cross:
					SetCursor(element, CrossCursor);
					break;
				default:
					throw new ArgumentOutOfRangeException("cursor");
			}
		}
		static CursorHelper() {
			HandCursor = LoadCursor("Images/Cursors/CursorHand.cur", 11, 13);
			HandDragCursor = LoadCursor("Images/Cursors/CursorHandDrag.cur", 11, 13);
			CrossCursor = LoadCursor("Images/Cursors/CursorCross.cur", 11, 11);
		}
		static string GetUriString(string cursorFilePath, string rootNamespace) {
			return string.Format("pack://application:,,,/{0}{1};component/{2}", rootNamespace, AssemblyInfo.VSuffix, cursorFilePath);
		}
		static Stream UpdateCursorHotspot(Uri uri, byte hotspotx, byte hotspoty) {
			StreamResourceInfo sri = Application.GetResourceStream(uri);
			Stream s = sri.Stream;
			byte[] buffer = new byte[s.Length];
			s.Read(buffer, 0, (int)s.Length);
			MemoryStream ms = new MemoryStream();
			buffer[10] = hotspotx;
			buffer[12] = hotspoty;
			ms.Write(buffer, 0, (int)s.Length);
			ms.Position = 0;
			return ms;
		}
		static Cursor LoadCursor(string filename, byte hotspotx, byte hotspoty) {
			string uriString = GetUriString(filename, XmlNamespaceConstants.PrintingNamespace);
			Uri cursorFileUri = new Uri(uriString, UriKind.RelativeOrAbsolute);
			return new Cursor(UpdateCursorHotspot(cursorFileUri, hotspotx, hotspoty));
		}
	}
}
