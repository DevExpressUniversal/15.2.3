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

using DevExpress.Data.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.PropertyGrid.Internal;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
namespace DevExpress.Xpf.PropertyGrid {
	public enum CellEditorPresenterPathMode {
		Absolute,
		Relative
	}
	public class CellEditorPresenter : Panel, INavigationSupport {
		public static readonly DependencyProperty IsSelectedProperty;		
		public static readonly DependencyProperty PathModeProperty;
		public static readonly DependencyProperty PathProperty;				
		Locker initializationLocker = new Locker();
		RowDataBase rowData;
		RowControlBase rowControl;
		PropertyGridView ownerView;
		private RowDataBase actualRowData;
		RequestPresenterWeakEventHandler requestPresenterHandler;
		FieldInvalidatedEventHandler fieldInvalidatedHandler;
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public RowDataBase RowData {
			get { return rowData; }
			set {
				if (rowData == value)
					return;
				rowData = value;
				OnRowDataChanged();
			}
		}
		public RowControlBase RowControl {
			get { return rowControl; }
			set {
				if (rowControl == value)
					return;
				var oldValue = rowControl;
				rowControl = value;
				OnRowControlChanged(oldValue, value);
			}
		}
		public PropertyGridView OwnerView {
			get { return ownerView; }
			set {
				if (ownerView == value)
					return;
				ownerView = value;
				OnOwnerViewChanged();
			}
		}
		public RowDataBase ActualRowData {
			get { return actualRowData; }
			set {
				if (value == actualRowData) return;
				actualRowData = value;
				OnActualRowDataChanged();
			}
		}		
		public string Path {
			get { return (string)GetValue(PathProperty); }
			set { SetValue(PathProperty, value); }
		}
		public CellEditorPresenterPathMode PathMode {
			get { return (CellEditorPresenterPathMode)GetValue(PathModeProperty); }
			set { SetValue(PathModeProperty, value); }
		}
		protected internal CellEditor CellEditor { get; private set; }				
		static CellEditorPresenter() {
			PropertyGridHelper.RowControlProperty.OverrideMetadata(typeof(CellEditorPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => (d as CellEditorPresenter).Do(x => x.RowControl = e.NewValue as RowControlBase)));
			PropertyGridHelper.RowDataProperty.OverrideMetadata(typeof(CellEditorPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => (d as CellEditorPresenter).Do(x => x.RowData = e.NewValue as RowDataBase)));
			PropertyGridHelper.ViewProperty.OverrideMetadata(typeof(CellEditorPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => (d as CellEditorPresenter).Do(x => x.OwnerView = e.NewValue as PropertyGridView)));
			NavigationManager.NavigationModeProperty.OverrideMetadata(typeof(CellEditorPresenter), new FrameworkPropertyMetadata(NavigationMode.Auto));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(CellEditorPresenter), new FrameworkPropertyMetadata(false, (d, e) => ((CellEditorPresenter)d).OnIsSelectedChanged((bool)e.OldValue)));			
			PathModeProperty = DependencyPropertyManager.Register("PathMode", typeof(CellEditorPresenterPathMode), typeof(CellEditorPresenter), new FrameworkPropertyMetadata(CellEditorPresenterPathMode.Relative, (d, e) => ((CellEditorPresenter)d).OnPathModeChanged((CellEditorPresenterPathMode)e.OldValue)));
			PathProperty = DependencyPropertyManager.Register("Path", typeof(string), typeof(CellEditorPresenter), new FrameworkPropertyMetadata(null, (d, e) => ((CellEditorPresenter)d).OnPathChanged((string)e.OldValue)));
		}
		public CellEditorPresenter() {
			requestPresenterHandler = new RequestPresenterWeakEventHandler(this, (d, o, e) => e.Register((CellEditorPresenter)d));
			fieldInvalidatedHandler = new FieldInvalidatedEventHandler((o, e) => ((CellEditorPresenter)this).OnFieldInvalidated(e.FullPath));
			PropertyGridHelper.SetEditorPresenter(this, this);
		}
		protected virtual void OnFieldInvalidated(string fieldName) {
			if (string.IsNullOrEmpty(Path))
				return;
			if (String.IsNullOrEmpty(fieldName) || fieldName == RowData.With(x=>x.FullPath)) {
				UpdateActualRowData();
			} else if (PathMode == CellEditorPresenterPathMode.Absolute && fieldName == Path) {
				UpdateActualRowData();
			} else if (String.Format("{0}.{1}", (RowData.With(x => x.FullPath) ?? ""), Path) == fieldName)
				UpdateActualRowData();
		}
		protected virtual void OnRowDataChanged() {
			UpdateActualRowData();
		}		
		protected virtual void OnOwnerViewChanged() {
			InternalUpdateCellEditor();
		}
		protected virtual void OnRowControlChanged(RowControlBase oldValue, RowControlBase newValue) {
			if (oldValue != null) {
				oldValue.RequestPresenter -= requestPresenterHandler.Handler;
				oldValue.With(x=>x.OwnerView).With(x=>x.PropertyGrid).Do(x=>x.RowDataGenerator.FieldInvalidated -= fieldInvalidatedHandler);
			}
			if (newValue != null) {
				newValue.RequestPresenter += requestPresenterHandler.Handler;
				newValue.With(x => x.OwnerView).With(x => x.PropertyGrid).Do(x => x.RowDataGenerator.FieldInvalidated += fieldInvalidatedHandler);
			}				
			InternalUpdateCellEditor();
		}
		protected virtual void OnActualRowDataChanged() {
			InternalUpdateCellEditor();
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) {
			if (IsSelected) {
				RowControl.EnsureSelection(this);
			}			
			InternalUpdateCellEditor();
		}		
		protected virtual void OnPathModeChanged(CellEditorPresenterPathMode oldValue) {
			UpdateActualRowData();
		}
		protected virtual void OnPathChanged(string oldValue) {
			UpdateActualRowData();
		}
		public override void BeginInit() {
			initializationLocker.Lock();
			base.BeginInit();
		}
		public override void EndInit() {
			base.EndInit();
			initializationLocker.Unlock();
			InternalUpdateCellEditor();
		}
		void UpdateActualRowData() {
			if (RowData == null) {
				ActualRowData = null;
				return;
			}
			if (String.IsNullOrEmpty(Path)) {
				ActualRowData = RowData;
				return;
			}
			RowDataGenerator generator = RowData.RowDataGenerator;
			DataViewBase dataView = generator.DataView;
			RowHandle targetHandle = null;
			if(PathMode == CellEditorPresenterPathMode.Relative){
				targetHandle = dataView.GetHandleByFieldName(dataView.GetFieldNameByHandle(RowData.Handle)+"."+Path);
			}
			if (PathMode == CellEditorPresenterPathMode.Absolute) {
				targetHandle = dataView.GetHandleByFieldName(Path);
			}
			ActualRowData = targetHandle.IsInvalid ? null : generator.ForceGetRowDataForHandle(targetHandle);
		}
		void InternalUpdateCellEditor() {
			initializationLocker.DoLockedActionIfNotLocked(() => {
				UpdateCellEditor();
			});
		}
		protected virtual void ClearCellEditor(PropertyGrid.CellEditor editor) {
			if (editor == null)
				return;
			editor.IsEditorFocused = false;
			editor.RowControl = null;
			editor.OwnerView = null;
			editor.RowData = null;			
			InternalChildren.Remove(editor);
		}
		protected virtual CellEditor CreateCellEditor() {
			return new CellEditor();			
		}
		protected virtual void UpdateCellEditor() {
			if (Equals(null, ActualRowData) || Equals(null, RowControl) || Equals(null, OwnerView)) {
				ClearCellEditor(CellEditor);
				CellEditor = null;
				return;
			}
			if (CellEditor == null) {
				CellEditor = CreateCellEditor();
				InternalChildren.Add(CellEditor);
			}			
			CellEditor.RowData = ActualRowData;
			CellEditor.RowControl = RowControl;
			CellEditor.OwnerView = OwnerView;
			CellEditor.IsEditorFocused = IsSelected;
		}
		protected internal void ShowEditor() {
			CellEditor.ShowEditor();
			OwnerView.ImmediateActionsManager.EnqueueAction(new SelectAllAction(CellEditor));
		}
		protected override Size MeasureOverride(Size availableSize) {
			if (CellEditor != null) {
				CellEditor.Measure(availableSize);
				return CellEditor.DesiredSize;
			}
			return new Size();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if (CellEditor != null) {
				CellEditor.Arrange(new Rect(new Point(0,0), finalSize));
				return finalSize;
			}
			return new Size();
		}
		#region INavigationSupport Members
		bool INavigationSupport.ProcessNavigation(NavigationDirection direction) {
			return false;
		}
		IEnumerable<FrameworkElement> INavigationSupport.GetChildren() {
			return new FrameworkElement[] { };
		}
		FrameworkElement INavigationSupport.GetParent() {
			return RowControl;
		}
		bool INavigationSupport.GetSkipNavigation() {
			return !((INavigationSupport)RowControl).GetSkipNavigation();
		}
		bool INavigationSupport.GetUseLinearNavigation() {
			return false;
		}
		#endregion        
	}
	internal class RequestPresenterEventArgs : EventArgs {
		List<CellEditorPresenter> presenters = new List<CellEditorPresenter>();
		public void Register(CellEditorPresenter presenter) {
			presenters.Add(presenter);
		}
		public IEnumerable<CellEditorPresenter> GetPresenters() {
			return presenters.ToArray();
		}
	}
	internal delegate void RequestPresenterEventHandler(object source, RequestPresenterEventArgs args);
	internal class RequestPresenterWeakEventHandler : WeakEventHandler<CellEditorPresenter, RequestPresenterEventArgs, RequestPresenterEventHandler> {
		static Action<WeakEventHandler<CellEditorPresenter, RequestPresenterEventArgs, RequestPresenterEventHandler>, object> onDetachAction = (handler, rowControl) => ((RowControlBase)rowControl).RequestPresenter -= handler.Handler;
		static Func<WeakEventHandler<CellEditorPresenter, RequestPresenterEventArgs, RequestPresenterEventHandler>, RequestPresenterEventHandler> createHandlerFunction = h => h.OnEvent;
		public RequestPresenterWeakEventHandler(CellEditorPresenter owner, Action<CellEditorPresenter, object, RequestPresenterEventArgs> onEventAction)
			: base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
}
