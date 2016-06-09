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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Docking.Platform;
using DevExpress.Xpf.Docking.VisualElements;
using LK = DevExpress.Xpf.Docking.Platform.FloatingWindowLock.LockerKey;
namespace DevExpress.Xpf.Docking {
	public class MDIController : IMDIController, ILockOwner {
		bool isDisposingCore = false;
		DockLayoutManager containerCore;
		ActiveItemHelper ActivationHelper;
		DocumentPanel activeItemCore;
		BarManager mdiMenuManagerCore;
		MDIMenuBar mdiMenuBarCore;
		public MDIController(DockLayoutManager container) {
			containerCore = container;
			ActivationHelper = new ActiveItemHelper(this);
		}
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				this.isDisposingCore = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected void OnDisposing() {
			Ref.Dispose(ref ActivationHelper);
			activeItemCore = null;
			containerCore = null;
			mdiMenuManagerCore = null;
			mdiMenuBarCore = null;
		}
		public DockLayoutManager Container {
			get { return containerCore; }
		}
		void LockActivate() {
			lockActivateCounter++;
		}
		void UnlockActivate() {
			lockActivateCounter--;
		}
		int lockActivateCounter = 0;
		public void Activate(BaseLayoutItem item) {
			Activate(item, true);
		}
		public void Activate(BaseLayoutItem item, bool focus) {
			if(lockActivateCounter > 0 || !(item is DocumentPanel) || !item.AllowActivate || item.IsClosed) return;
			lockActivateCounter++;
			try {
				if(RaiseItemCancelEvent(item, DockLayoutManager.MDIItemActivatingEvent)) {
					item.InvokeCancelActivation(ActiveItem);
					return;
				}
				ActivationHelper.ActivateItem(item, focus);
			}
			finally { lockActivateCounter--; }
		}
		public BaseLayoutItem ActiveItem {
			get { return activeItemCore; }
			set {
				if(ActiveItem == value) return;
				SetActiveItemCore(value as DocumentPanel);
			}
		}
		void SetActiveItemCore(DocumentPanel value) {
			SetActive(false);
			DocumentPanel oldItem = activeItemCore;
			this.activeItemCore = value;
			SetActive(true);
			Container.isMDIItemActivation++;
			Container.ActiveMDIItem = ActiveItem;
			Container.isMDIItemActivation--;
			RaiseActiveItemChanged(value, oldItem);
		}
		void SetActive(bool value) {
			if(ActiveItem != null) {
				ActiveItem.SetActive(value);
				if(value && lockActivateCounter == 0) 
					ActivationHelper.SelectInGroup(ActiveItem);
			}
		}
		void RaiseActiveItemChanged(DocumentPanel item, DocumentPanel oldItem) {
			Container.RaiseEvent(
					 new MDIItemActivatedEventArgs(item, oldItem) { Source = Container }
				 );
		}
		static void ApplyState(DocumentGroup documentGroup, DocumentPanel document, MDIState state) {
			if(documentGroup != null && (documentGroup.IsMaximized || state != MDIState.Normal)) {
				foreach(BaseLayoutItem item in documentGroup.Items) {
					DocumentPanel doc = item as DocumentPanel;
					if(state == MDIState.Maximized)
						doc.IsMinimizedBeforeMaximize = doc.IsMinimized;
					MDIState mdiState = state;
					if(state == MDIState.Normal) {
						if(doc.IsMaximized && doc.IsMinimizedBeforeMaximize)
							mdiState = MDIState.Minimized;
					}
					MDIStateHelper.SetMDIState(item, mdiState);
				}
			}
			else MDIStateHelper.SetMDIState(document, state);
		}
		void UpdateMenuBar() {
		}
		public bool Maximize(BaseLayoutItem item) {
			if(item == null || item.IsClosed || !item.AllowMaximize || !item.IsMaximizable) return false;
			FloatGroup fRoot = item.GetRoot() as FloatGroup;
			fRoot
				.With(x => x.FloatingWindowLock)
				.Do(x => x.Lock(LK.WindowState));
			try {
				if(item is DocumentPanel) return Maximize((DocumentPanel)item);
				BaseLayoutItem itemToMaximize = fRoot ?? item;
				itemToMaximize.SetIsMaximized(true);
				Container.InvalidateView(item.GetRoot());
				return true;
			}
			finally {
				fRoot
					.With(x => x.FloatingWindowLock)
					.Do(x => x.Unlock(LK.WindowState));
			}
		}
		public bool Maximize(DocumentPanel document) {
			if(document == null || document.IsClosed || !document.AllowMaximize) return false;
			FloatGroup fRoot = document.GetRoot() as FloatGroup;
			FloatingWindowLock state = fRoot != null ? fRoot.FloatingWindowLock : null;
			state.Do(x => x.Lock(LK.WindowState));
			try {
				DocumentGroup documentGroup = document.Parent as DocumentGroup;
				if(documentGroup != null && documentGroup.IsTabbed)
					return false;
				FloatGroup fGroup = document.Parent as FloatGroup;
				if(fGroup != null || documentGroup != null) {
					ApplyState(documentGroup, document, MDIState.Maximized);
					if(fGroup != null) {
						fGroup.SetIsMaximized(true);
						Container.InvalidateView(fGroup);
					}
				}
				UpdateMenuBar();
				return true;
			}
			finally {
				state.Do(x => x.Unlock(LK.WindowState));
			}
		}
		internal int lockMinimizeBoundsCalculation = 0;
		public bool Minimize(DocumentPanel document) {
			if(document == null || document.IsClosed || !document.AllowMinimize) return false;
			bool isMaximized = MDIStateHelper.GetMDIState(document) == MDIState.Maximized;
			if(isMaximized && document.IsMinimizedBeforeMaximize) return Restore(document);
			DocumentGroup documentGroup = document.Parent as DocumentGroup;
			if(documentGroup == null || documentGroup.IsTabbed)
				return false;
			Size mdiArea = Size.Empty;
			if(lockMinimizeBoundsCalculation == 0 || !document.MinimizeLocation.HasValue) {
				UpdateLayout(documentGroup);
				mdiArea = GetMDIAreaSize(documentGroup);
			}
			MDIStateHelper.SetMDIState(document, MDIState.Minimized);
			if(lockMinimizeBoundsCalculation == 0) {
				UpdateLayout(document);
				Action action = new Action(() => MinimizeCore(document, documentGroup, mdiArea));
				if(isMaximized) 
					InvokeHelper.BeginInvoke(document, action, InvokeHelper.Priority.Render);
				else action.Invoke();
				RestoreOtherDocuments(documentGroup, document);
			}
			return true;
		}
		void MinimizeCore(DocumentPanel document, DocumentGroup documentGroup, Size mdiArea) {
			Point mdiLocaction = CalcMinimizeLocation(documentGroup, document, mdiArea);
			DocumentPanel.SetMDILocation(document, mdiLocaction);
		}
		void RestoreOtherDocuments(DocumentGroup dGroup, DocumentPanel document) {
			BaseLayoutItem[] items = dGroup.GetItems();
			for(int i = 0; i < items.Length; i++) {
				DocumentPanel doc = items[i] as DocumentPanel;
				if(doc == null || doc == document) continue;
				if(doc.IsMaximized)
					Restore(doc);
			}
		}
		Point CalcMinimizeLocation(DocumentGroup dGroup, DocumentPanel document, Size mdiArea) {
			if(document.MinimizeLocation.HasValue)
				return document.MinimizeLocation.Value;
			double x = 0;
			double y = mdiArea.Height - document.MDIDocumentSize.Height;
			BaseLayoutItem[] items = dGroup.GetItems();
			for(int i = 0; i < items.Length; i++) {
				DocumentPanel doc = items[i] as DocumentPanel;
				if(doc == null || !doc.IsMinimized) continue;
				if(doc == document || doc.MinimizeLocation.HasValue) continue;
				x += doc.MDIDocumentSize.Width;
			}
			return new Point(x, y);
		}
		public bool Restore(BaseLayoutItem item) {
			if(item == null || item.IsClosed) return false;
			FloatGroup fRoot = item.GetRoot() as FloatGroup;
			fRoot
				.With(x => x.FloatingWindowLock)
				.Do(x => x.Lock(LK.WindowState));
			try {
				if(item is DocumentPanel) return Restore((DocumentPanel)item);
				BaseLayoutItem itemToRestore = fRoot ?? item;
				itemToRestore.SetIsMaximized(false);
				Container.InvalidateView(item.GetRoot());
				return true;
			}
			finally {
				fRoot
					.With(x => x.FloatingWindowLock)
					.Do(x => x.Unlock(LK.WindowState));
			}
		}
		public bool Restore(DocumentPanel document) {
			if(document == null || document.IsClosed) return false;
			FloatGroup fRoot = document.GetRoot() as FloatGroup;
			FloatingWindowLock state = fRoot != null ? fRoot.FloatingWindowLock : null;
			state.Do(x => x.Lock(LK.WindowState));
			try {
				DocumentGroup documentGroup = document.Parent as DocumentGroup;
				if(documentGroup != null && documentGroup.IsTabbed)
					return false;
				FloatGroup fGroup = document.Parent as FloatGroup;
				if(fGroup != null || documentGroup != null) {
					ApplyState(documentGroup, document, MDIState.Normal);
					if(fGroup != null) {
						fGroup.SetIsMaximized(false);
						Container.InvalidateView(fGroup);
					}
					ActivationHelper.RestoreKeyboardFocus(document);
				}
				return true;
			}
			finally {
				state.Do(x => x.Unlock(LK.WindowState));
			}
		}
		public bool TileHorizontal(BaseLayoutItem item) {
			if(item == null || item.IsClosed) return false;
			DocumentGroup dGroup = GetDocumentGroup(item);
			if(dGroup != null && !dGroup.IsUngroupped) {
				BaseLayoutItem[] items = dGroup.GetItems();
				Size mdiArea = GetMDIAreaSize(dGroup);
				Rect[] rects = TileHelper.GetTiles(mdiArea, items.Length, false);
				for(int i = 0; i < items.Length; i++) {
					DocumentPanel document = items[i] as DocumentPanel;
					if(document == null) continue;
					MDIState state = MDIStateHelper.GetMDIState(document);
					if(state != MDIState.Normal)
						Restore(document);
					DocumentPanel.SetMDILocation(document, rects[i].Location());
					DocumentPanel.SetMDISize(document, rects[i].Size());
				}
				UpdateLayout(dGroup);
				return true;
			}
			return false;
		}
		public bool TileVertical(BaseLayoutItem item) {
			if(item == null || item.IsClosed) return false;
			DocumentGroup dGroup = GetDocumentGroup(item);
			if(dGroup != null && !dGroup.IsUngroupped) {
				BaseLayoutItem[] items = dGroup.GetItems();
				Size mdiArea = GetMDIAreaSize(dGroup);
				Rect[] rects = TileHelper.GetTiles(mdiArea, items.Length, true);
				for(int i = 0; i < items.Length; i++) {
					DocumentPanel document = items[i] as DocumentPanel;
					if(document == null) continue;
					MDIState state = MDIStateHelper.GetMDIState(document);
					if(state != MDIState.Normal)
						Restore(document);
					DocumentPanel.SetMDILocation(document, rects[i].Location());
					DocumentPanel.SetMDISize(document, rects[i].Size());
				}
				UpdateLayout(dGroup);
				return true;
			}
			return false;
		}
		public bool ArrangeIcons(BaseLayoutItem item) {
			if(item == null || item.IsClosed) return false;
			DocumentGroup dGroup = GetDocumentGroup(item);
			if(dGroup != null && !dGroup.IsUngroupped) {
				lockMinimizeBoundsCalculation++;
				BaseLayoutItem[] items = dGroup.GetItems();
				for(int i = 0; i < items.Length; i++) {
					DocumentPanel document = items[i] as DocumentPanel;
					if(document == null) continue;
					if(!document.IsMinimized)
						Minimize(document);
				}
				UpdateLayout(dGroup);
				double x = 0; double y = 0;
				Size mdiArea = GetMDIAreaSize(dGroup);
				double bottom = mdiArea.Height;
				for(int i = 0; i < items.Length; i++) {
					DocumentPanel document = items[i] as DocumentPanel;
					if(document == null) continue;
					y = bottom - document.MDIDocumentSize.Height;
					DocumentPanel.SetMDILocation(document, new Point(x, y));
					x += document.MDIDocumentSize.Width;
				}
				lockMinimizeBoundsCalculation--;
				return true;
			}
			return false;
		}
		static void UpdateLayout(DocumentGroup dGroup) {
			dGroup.UpdateLayout();
		}
		static void UpdateLayout(DocumentPanel document) {
			document.UpdateLayout();
		}
		public bool Cascade(BaseLayoutItem item) {
			if(item == null || item.IsClosed) return false;
			DocumentGroup dGroup = GetDocumentGroup(item);
			if(dGroup != null && !dGroup.IsUngroupped) {
				UpdateLayout(dGroup);
				BaseLayoutItem[] items = CascadeHelper.GetItemsToCascade(dGroup, ActiveItem == dGroup.SelectedItem);
				items = LayoutItemsHelper.SortByZIndex(items);
				double windowCaptionHeight = GetMDIDocumentHeaderHeight(dGroup);
				Size mdiArea = GetMDIAreaSize(dGroup);
				var bounds = CascadeHelper.GetBounds(mdiArea, items.Length, windowCaptionHeight);
				for(int i = 0; i < items.Length; i++) {
					DocumentPanel document = items[i] as DocumentPanel;
					if(document == null) continue;
					MDIState state = MDIStateHelper.GetMDIState(document);
					if(state != MDIState.Normal)
						Restore(document);
					DocumentPanel.SetMDISize(document, bounds[i].Size());
					DocumentPanel.SetMDILocation(document, bounds[i].Location());
				}
				return true;
			}
			return false;
		}
		public bool ChangeMDIStyle(BaseLayoutItem item) {
			if(item == null || item.IsClosed) return false;
			DocumentGroup dGroup = GetDocumentGroup(item);
			if(dGroup != null && !dGroup.IsUngroupped) {
				dGroup.ChangeMDIStyle();
				UpdateLayout(dGroup);
				return true;
			}
			return false;
		}
		DocumentGroup GetDocumentGroup(BaseLayoutItem item) {
			DocumentPanel dPanel = item as DocumentPanel;
			DocumentGroup dGroup = item as DocumentGroup;
			if(dGroup == null && dPanel != null) {
				dGroup = dPanel.Parent as DocumentGroup;
			}
			return dGroup;
		}
		double GetMDIDocumentHeaderHeight(DocumentGroup dGroup) {
			double value = dGroup.MDIDocumentHeaderHeight;
			return double.IsNaN(value) ? 24 : value;
		}
		Size GetMDIAreaSize(DocumentGroup dGroup) {
			return dGroup.MDIAreaSize;
		}
		bool RaiseItemCancelEvent(BaseLayoutItem item, RoutedEvent routedEvent) {
			return Container.RaiseItemCancelEvent(item, routedEvent);
		}
		public T CreateCommand<T>(BaseLayoutItem[] items) where T : MDIControllerCommand, new() {
			return new T() { Controller = this, Items = items };
		}
		public MDIMenuBar MDIMenuBar {
			get {
				if(mdiMenuBarCore == null) {
					if(!IsDisposing) {
						mdiMenuManagerCore = new BarManager() {
							Name = UniqueNameHelper.GetMDIBarManagerName(),
							KeyGestureWorkingMode = Xpf.Bars.KeyGestureWorkingMode.AllKeyGesture,
							CreateStandardLayout = false
						};
						DevExpress.Xpf.Core.Serialization.DXSerializer.SetEnabled(mdiMenuManagerCore, false);
						mdiMenuBarCore = new Docking.MDIMenuBar(Container, mdiMenuManagerCore);
						DockLayoutManager.AddToVisualTree(Container, mdiMenuManagerCore);
						DockLayoutManager.AddLogicalChild(Container, mdiMenuManagerCore);
					}
				}
				return mdiMenuBarCore;
			}
		}
		#region ILockOwner Members
		void ILockOwner.Lock() {
			LockActivate();
		}
		void ILockOwner.Unlock() {
			UnlockActivate();
		}
		#endregion
		internal static class TileHelper {
			public static Rect[] GetTiles(Size size, int count, bool horz) {
				Rect[] rects = new Rect[count];
				if(size.Width * size.Height <= 0) return rects;
				if(count < 4) {
					double x = 0; double y = 0;
					double l = (horz ? size.Width : size.Height) / (double)count;
					for(int i = 0; i < count; i++) {
						rects[i] = new Rect(x, y, horz ? l : size.Width, horz ? size.Height : l);
						if(horz) x += l;
						else y += l;
					}
					return rects;
				}
				else {
					int sqrt = (int)(Math.Sqrt((double)count) + 0.5);
					int n1 = (int)(Math.Sqrt((double)count * size.Width / size.Height) + 0.5);
					int n2 = (int)((double)count / (double)n1 + 0.5);
					if(sqrt * sqrt == count) n1 = n2 = sqrt;
					double x = 0; double y = 0;
					double w = size.Width / (double)n1;
					double h = size.Height / (double)n2;
					int n = horz ? n1 : n2;
					for(int i = 0; i < count; i++) {
						rects[i] = new Rect(x, y, w, h);
						if(horz) {
							x += w;
							if((i + 1) % n1 == 0) {
								x = 0; y += h;
							}
							if(n2 > 1 && (i + 1) / n1 + 1 == n2) {
								w = size.Width / (double)(count - n1 * (n2 - 1));
							}
						}
						else {
							y += h;
							if((i + 1) % n2 == 0) {
								y = 0; x += w;
							}
							if(n1 > 1 && (i + 1) / n2 + 1 == n1) {
								h = size.Height / (double)(count - n2 * (n1 - 1));
							}
						}
					}
				}
				return rects;
			}
		}
		internal static class CascadeHelper {
			static readonly double minWidth = 200;
			static readonly double minHeight = 100;
			public static Rect[] GetBounds(Size size, int count, double offset) {
				Rect[] bounds = new Rect[count];
				double bestWidth = Math.Max(size.Width - count * offset, minWidth);
				double bestHeight = Math.Max(size.Height - count * offset, minHeight);
				double bestCountW = Math.Round((size.Width - bestWidth) / offset);
				double bestCountH = Math.Round((size.Height - bestHeight) / offset);
				int itemsInRow = (int)Math.Max(1, Math.Min(bestCountW, bestCountH));
				int rows = (int)Math.Round((double)count / itemsInRow);
				int k = 0;
				for(int i = 0; i < rows; i++) {
					for(int j = 0; j < itemsInRow; j++) {
						bounds[k] = new Rect(new Point(offset * j, offset * j), new Size(bestWidth, bestHeight));
						if(++k == bounds.Length) break;
					}
				}
				return bounds;
			}
			public static BaseLayoutItem[] GetItemsToCascade(DocumentGroup group, bool f = false) {
				if(group == null) return new BaseLayoutItem[] { };
				BaseLayoutItem[] items = group.GetItems();
				int size = items.Length - 1;
				if(f && group.SelectedItem != null) {
					int index = group.Items.IndexOf(group.SelectedItem);
					if(index < size) {
						Array.Copy(items, index + 1, items, index, size - index);
						items[size] = group.SelectedItem;
					}
				}
				return items;
			}
		}
	}
	#region Commands
	public abstract class MDIControllerCommand : ICommand {
		internal MDIControllerCommand() { }
		protected abstract void ExecuteCore(IMDIController controller, BaseLayoutItem[] items);
		protected abstract bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items);
		protected internal IMDIController Controller { get; set; }
		protected internal BaseLayoutItem[] Items { get; set; }
		#region ICommand
		event EventHandler CanExecuteChangedCore;
		event EventHandler ICommand.CanExecuteChanged {
			add { CanExecuteChangedCore += value; }
			remove { CanExecuteChangedCore -= value; }
		}
		protected void RaiseCanExecuteChanged() {
			if(CanExecuteChangedCore != null)
				CanExecuteChangedCore(this, EventArgs.Empty);
		}
		bool ICommand.CanExecute(object parameter) {
			if(Items == null) return false;
			return CanExecuteCore(Controller, Items);
		}
		void ICommand.Execute(object parameter) {
			ExecuteCore(Controller, Items);
		}
		#endregion ICommand
		#region static
		static MDIControllerCommand() {
			Maximize = new MDIControllerCommandLink("Maximize", new MaximizeDocumentCommand());
			Minimize = new MDIControllerCommandLink("Minimize", new MinimizeDocumentCommand());
			Restore = new MDIControllerCommandLink("Restore", new RestoreDocumentCommand());
			TileVertical = new MDIControllerCommandLink("TileVertical", new TileVerticalCommand());
			TileHorizontal = new MDIControllerCommandLink("TileHorizontal", new TileHorizontalCommand());
			Cascade = new MDIControllerCommandLink("Cascade", new CascadeCommand());
			ArrangeIcons = new MDIControllerCommandLink("ArrangeIcons", new ArrangeIconsCommand());
			ChangeMDIStyle = new MDIControllerCommandLink("ChangeMDIStyle", new ChangeMDIStyleCommand());
		}
		public static RoutedCommand Maximize { get; private set; }
		public static RoutedCommand Minimize { get; private set; }
		public static RoutedCommand Restore { get; private set; }
		public static RoutedCommand TileVertical { get; private set; }
		public static RoutedCommand TileHorizontal { get; private set; }
		public static RoutedCommand Cascade { get; private set; }
		public static RoutedCommand ArrangeIcons { get; private set; }
		public static RoutedCommand ChangeMDIStyle { get; private set; }
		internal static void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
			IMDIController controller = MDIControllerHelper.GetMDIController(sender);
			BaseLayoutItem[] items = e.Parameter as BaseLayoutItem[];
			if(e.Parameter is BaseLayoutItem)
				items = new BaseLayoutItem[] { (BaseLayoutItem)e.Parameter };
			MDIControllerCommand command = ((MDIControllerCommandLink)e.Command).Command;
			e.CanExecute = (controller != null && items != null) && command.CanExecuteCore(controller, items);
		}
		internal static void Executed(object sender, ExecutedRoutedEventArgs e) {
			IMDIController controller = MDIControllerHelper.GetMDIController(sender);
			BaseLayoutItem[] items = e.Parameter as BaseLayoutItem[];
			if(e.Parameter is BaseLayoutItem)
				items = new BaseLayoutItem[] { (BaseLayoutItem)e.Parameter };
			if(controller != null && items != null) {
				MDIControllerCommand command = ((MDIControllerCommandLink)e.Command).Command;
				command.ExecuteCore(controller, items);
			}
		}
		#endregion static
		class MDIControllerCommandLink : RoutedCommand {
			public MDIControllerCommandLink(string name, MDIControllerCommand command) :
				base(name, typeof(MDIControllerCommand)) {
				Command = command;
			}
			public MDIControllerCommand Command { get; private set; }
		}
	}
	public class MaximizeDocumentCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.Maximize(item as DocumentPanel);
		}
	}
	public class MinimizeDocumentCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			if(items.Length == 1) return items[0].AllowMinimize;
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.Minimize(item as DocumentPanel);
		}
	}
	public class RestoreDocumentCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			if(items.Length == 1) return items[0].AllowRestore;
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.Restore(item as DocumentPanel);
		}
	}
	public class TileVerticalCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.TileVertical(item);
		}
	}
	public class TileHorizontalCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.TileHorizontal(item);
		}
	}
	public class CascadeCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.Cascade(item);
		}
	}
	public class ArrangeIconsCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.ArrangeIcons(item);
		}
	}
	public class ChangeMDIStyleCommand : MDIControllerCommand {
		protected override bool CanExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			return items.Length > 0;
		}
		protected override void ExecuteCore(IMDIController controller, BaseLayoutItem[] items) {
			foreach(BaseLayoutItem item in items)
				controller.ChangeMDIStyle(item);
		}
	}
	#endregion Commands    
	public static class MDIControllerHelper {
		internal static void Restore(BaseLayoutItem item) {
			if(!(item is DocumentPanel)) return;
			DockLayoutManager container = item.FindDockLayoutManager() ?? DockLayoutManager.FindManager(item);
			if(container != null && !container.IsDisposing) {
				container.MDIController.Restore((DocumentPanel)item);
			}
		}
		public static T CreateCommand<T>(DockLayoutManager container, BaseLayoutItem[] items) where T : MDIControllerCommand, new() {
			IMDIController controller = GetMDIController(container);
			return (controller != null) ? controller.CreateCommand<T>(items) : null;
		}
		static IMDIController GetMDIController(DockLayoutManager container) {
			return (container != null) ? container.MDIController : null;
		}
		static BarManager GetBarManager(DockLayoutManager container) {
			return (container != null && !container.IsDisposing) ?
				container.CustomizationController.BarManager : null;
		}
		public static IMDIController GetMDIController(object obj) {
			DockLayoutManager container = obj as DockLayoutManager;
			if(container == null) {
				DependencyObject dObj = obj as DependencyObject;
				container = (dObj != null) ? DockLayoutManager.GetDockLayoutManager(dObj) : null;
			}
			return GetMDIController(container);
		}
		public static void MergeMDITitles(DependencyObject dObj) {
			if(dObj == null) return;
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(dObj);
			if(manager != null && manager.ShowMaximizedDocumentCaptionInWindowTitle) {
				BaseLayoutItem item = DockLayoutManager.GetLayoutItem(dObj);
				if(item != null) manager.SetMDIChildrenTitle(item.Caption as string);
			}
		}
		public static void UnMergeMDITitles(DependencyObject dObj) {
			if(dObj == null) return;
			DockLayoutManager manager = DockLayoutManager.GetDockLayoutManager(dObj);
			if(manager != null) manager.ResetMDIChildrenTitle();
		}   
		internal static void DoMerge(BaseDocument document) {
			DoMergeOrUnMerge(document, true);
		}
		internal static void DoUnMerge(BaseDocument document) {
			DoMergeOrUnMerge(document, false);
		}
		static void DoMergeOrUnMerge(BaseDocument document, bool merge) {
			var currentValue = GetElementMergingBehavior(document);
			if (!merge && currentValue != ElementMergingBehavior.InternalWithExternal)
				return;
			var dlm = document.With(x => x.DocumentPanel).With(x => x.Manager);
			if(dlm != null) {				
				var parent = BarManager.GetBarManager(dlm);
				var child = FindChildBarManager(document);
				if(child == null || (merge ? RaiseMerge(dlm, parent, child) : RaiseUnMerge(dlm, parent, child))) {
					SetElementMergingBehavior(document, merge ? ElementMergingBehavior.InternalWithExternal : ElementMergingBehavior.InternalWithInternal);
				}
			}
		}
		static ElementMergingBehavior GetElementMergingBehavior(BaseDocument document) {
			return document.With(x => x.LayoutItem).Return(x => MergingProperties.GetElementMergingBehavior(x), () => ElementMergingBehavior.Undefined);
		}
		static void SetElementMergingBehavior(BaseDocument document, ElementMergingBehavior value) {
			var lI = document.With(x => x.LayoutItem);
			if (lI == null)
				return;
			MergingProperties.SetElementMergingBehavior(lI, value);
		}
		public static bool MergeMDIMenuItems(DependencyObject dObj) {
			var mmb = dObj.With(DockLayoutManager.GetDockLayoutManager).With(GetMDIController).With(x => x.MDIMenuBar);
			if (mmb == null)
				return false;
			MergingProperties.SetElementMergingBehavior(mmb, ElementMergingBehavior.All);
			return ((IMergingSupport)mmb).IsMerged;
		}
		public static void UnMergeMDIMenuItems(DependencyObject dObj) {
			var mmb = dObj.With(DockLayoutManager.GetDockLayoutManager).With(GetMDIController).With(x => x.MDIMenuBar);
			if (mmb == null)
				return;
			MergingProperties.SetElementMergingBehavior(mmb, ElementMergingBehavior.None);
		}
	  static bool RaiseMerge(DockLayoutManager manager, BarManager parent, BarManager child) {
			BarMergeEventArgs barMergeEventArgs = new BarMergeEventArgs(parent, child) { RoutedEvent = DockLayoutManager.MergeEvent };
			manager.RaiseEvent(barMergeEventArgs);
			return !barMergeEventArgs.Cancel;
		}
		static bool RaiseUnMerge(DockLayoutManager manager, BarManager parent, BarManager child) {
			BarMergeEventArgs barMergeEventArgs = new BarMergeEventArgs(parent, child) { RoutedEvent = DockLayoutManager.UnMergeEvent };
			manager.RaiseEvent(barMergeEventArgs);
			return !barMergeEventArgs.Cancel;
		}
		public static BarManager FindChildBarManager(DependencyObject element) {
			if(element == null) return null;
			BarManager result = null;
			using(var e = LayoutItemsHelper.GetEnumerator(element, null)) {
				while(e.MoveNext()) {
					if(e.Current is BarManager) {
						return e.Current as BarManager;
					}
					if(e.Current is IUIElement && e.Current != element) break;
				}
			}
			return result;
		}
		static internal MDIMergeStyle GetActualMDIMergeStyle(DockLayoutManager manager, BaseLayoutItem item) {
			MDIMergeStyle mergeStyle = DocumentPanel.GetMDIMergeStyle(item);
			return mergeStyle != MDIMergeStyle.Default || manager == null ? mergeStyle : manager.MDIMergeStyle;
		}
	}
	public class MDIMenuBar : Bar, IDockLayoutManagerListener {
		public enum ItemType { Minimize, Restore, Close };
		public const string MDIMenuBarCategory = "MDIButtons";
		public const string MDIMenuBarItemPrefix = "__MDIMenuBarItem__";		
		public MDIMenuBar(DockLayoutManager manager, BarManager mdiBarManager) {
			DockLayoutManager.SetDockLayoutManager(this, manager);
			MergingProperties.SetElementMergingBehavior(this, ElementMergingBehavior.None);
			Name = UniqueNameHelper.GetMDIBarName();
			IsMainMenu = true;
			var category = new BarManagerCategory() { Name = MDIMenuBarCategory };
			var barItemMinimize = CreateBarButtonItem(ItemType.Minimize, manager);
			var barItemRestore = CreateBarButtonItem(ItemType.Restore, manager);
			var barItemClose = CreateBarButtonItem(ItemType.Close, manager);
			mdiBarManager.Categories.Add(category);
			mdiBarManager.Items.Add(barItemMinimize);
			mdiBarManager.Items.Add(barItemRestore);
			mdiBarManager.Items.Add(barItemClose);
			ItemLinks.Add(barItemMinimize);
			ItemLinks.Add(barItemRestore);
			ItemLinks.Add(barItemClose);
			mdiBarManager.Bars.Add(this);
		}
		BarItem CreateBarButtonItem(ItemType type, DockLayoutManager manager) {
			MDIButtonSettings settings = MDIButtonSettings.GetSettings(type);
			BarMDIButtonItem barItem = new BarMDIButtonItem()
			{
				Name = GetBarItemName(type),
				CategoryName = MDIMenuBarCategory,
				Content = settings.Content,
				Hint = settings.Hint,
				KeyGesture = settings.KeyGesture,
				Command = settings.Command
			};
			BindingHelper.SetBinding(barItem, BarItem.CommandParameterProperty, manager, DockLayoutManager.ActiveMDIItemProperty);
			BindingHelper.SetBinding(barItem, BarItem.IsVisibleProperty, manager, settings.IsVisibleBindingPath);
			BindingHelper.SetBinding(barItem, BarItem.CommandTargetProperty, this, DockLayoutManager.DockLayoutManagerProperty);
			return barItem;
		}
		public BarMDIButtonItem GetBarItem(ItemType type) {
			return (Manager != null) ? (BarMDIButtonItem)Manager.Items[GetBarItemName(type)] : null;
		}
		public void UpdateMDIButtonBorderStyle(DockLayoutManager manager, Style style) {
			foreach(BarItemLinkBase link in ItemLinks) {
				if(style == null) link.ClearValue(BarItemLinkBase.CustomResourcesProperty);
				else {
					ResourceDictionary dictionary = new ResourceDictionary();
					dictionary.Add(GetBorderStyleKey(manager), style);
					dictionary.Add(GetBorderStyleKey(manager, true), style);
					link.CustomResources = dictionary;
				}
			}
		}
		static Bars.Themes.BarItemThemeKeyExtension GetBorderStyleKey(DockLayoutManager manager, bool forRibbonControl = false) {
			return new Bars.Themes.BarItemThemeKeyExtension()
			{
				ThemeName = DockLayoutManagerExtension.GetThemeName(manager),
				ResourceKey = forRibbonControl ? Bars.Themes.BarItemThemeKeys.BorderStyleInRibbonPageHeader : Bars.Themes.BarItemThemeKeys.BorderStyleInMainMenu
			};
		}
		static string GetBarItemName(ItemType type) {
			return MDIMenuBarItemPrefix + type.ToString();
		}
		public class MDIButtonSettings {
			public ICommand Command { get; private set; }
			public object Content { get; private set; }
			public string Hint { get; private set; }
			public KeyGesture KeyGesture { get; private set; }
			public string IsVisibleBindingPath { get; private set; }
			protected MDIButtonSettings() { }
			class MinimizeMDIButtonSettings : MDIButtonSettings {
				internal MinimizeMDIButtonSettings() {
					Command = MDIControllerCommand.Minimize;
					Hint = DockingLocalizer.Active.GetLocalizedString(DockingStringId.ControlButtonMinimize);
					IsVisibleBindingPath = "ActiveMDIItem.ShowMinimizeButton";
				}
			}
			class RestoreMDIButtonSettings : MDIButtonSettings {
				internal RestoreMDIButtonSettings() {
					Command = MDIControllerCommand.Restore;
					Hint = DockingLocalizer.Active.GetLocalizedString(DockingStringId.ControlButtonRestore);
					IsVisibleBindingPath = "ActiveMDIItem.ShowRestoreButton";
				}
			}
			class CloseMDIButtonSettings : MDIButtonSettings {
				internal CloseMDIButtonSettings() {
					Command = DockControllerCommand.Close;
					Hint = DockingLocalizer.Active.GetLocalizedString(DockingStringId.ControlButtonClose);
					KeyGesture = new KeyGesture(Key.F4, ModifierKeys.Control);
					IsVisibleBindingPath = "ActiveMDIItem.ShowCloseButton";
				}
			}
			public static MDIButtonSettings GetSettings(ItemType type) {
				switch(type) {
					case ItemType.Minimize: return new MinimizeMDIButtonSettings();
					case ItemType.Restore: return new RestoreMDIButtonSettings();
					case ItemType.Close: return new CloseMDIButtonSettings();
				}
				throw new NotSupportedException(type.ToString());
			}
		}
		#region IDockLayoutManagerListener Members
		void IDockLayoutManagerListener.Subscribe(DockLayoutManager manager) {
		}
		void IDockLayoutManagerListener.Unsubscribe(DockLayoutManager manager) {
		}
		#endregion
	}
	public class BarMDIButtonItem : BarButtonItem {
		public BarMDIButtonItem() {
			IsPrivate = true;
		}
		protected override bool CanKeyboardSelect { get { return false; } }
		public override BarItemLink CreateLink(bool isPrivate) {
			return new BarMDIButtonItemLink(this);
		}
		protected override Type GetLinkType() {
			return typeof(BarMDIButtonItemLink);
		}
	}
	public class BarMDIButtonItemLink : BarButtonItemLink {
		public BarMDIButtonItemLink() {
			MergeType = BarItemMergeType.Replace;
			MergeOrder = int.MaxValue;
			Alignment = BarItemAlignment.Far;
		}
		internal BarMDIButtonItemLink(BarMDIButtonItem item)
			: this() {
			Item = item;
		}		
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BindingHelper.SetBinding(this, CustomResourcesProperty, link, "CustomResources");
			BindingHelper.SetBinding(this, IsEnabledProperty, link, "IsEnabled");
		}
		protected override bool AllowShowCustomizationMenu { get { return false; } set { } }
		public override bool IsPrivate { get { return true; } protected set { } }
		protected override bool IsPrivateLinkInCustomizationMode { get { return true; } set { } }
	}
	static class MergingHelper {
		static object syncObj = new object();
		static IList<WeakReference> clients = new List<WeakReference>();
		public static void AddMergingClient(IMergingClient client) {
			lock(syncObj) {
				object[] targets = Purge();
				if(Array.IndexOf(targets, client) == -1) {
					if(clients.Count == 0) {
					}
					clients.Add(new WeakReference(client));
				}
			}
		}
		public static void RemoveClient(IMergingClient client) {
			lock(syncObj) {
				WeakReference[] references = new WeakReference[clients.Count];
				clients.CopyTo(references, 0);
				for(int i = 0; i < references.Length; i++) {
					WeakReference wRef = references[i];
					object target = wRef.Target;
					if(target == null || object.ReferenceEquals(target, client))
						clients.Remove(wRef);
				}
				if(clients.Count == 0) {
				}
			}
		}
		public static void DoMerging() {
			lock(syncObj) {
				object[] targets = Purge();
				for(int i = 0; i < targets.Length; i++) {
					IMergingClient target = targets[i] as IMergingClient;
					if(target != null) {
						target.Merge();
						RemoveClient(target);
					}
				}
			}
		}
		static object[] Purge() {
			List<object> targets = new List<object>();
			WeakReference[] references = new WeakReference[clients.Count];
			clients.CopyTo(references, 0);
			for(int i = 0; i < references.Length; i++) {
				WeakReference wRef = references[i];
				object target = wRef.Target;
				if(target == null)
					clients.Remove(wRef);
				else targets.Add(target);
			}
			return targets.ToArray();
		}
	}
}
