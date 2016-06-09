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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using DevExpress.Data.Linq;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid.Data;
#if SL
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class GridDataCellMadeVisibleEventManager : WeakEventManager {
		static GridDataCellMadeVisibleEventManager CurrentManager {
			get {
				Type managerType = typeof(GridDataCellMadeVisibleEventManager);
				GridDataCellMadeVisibleEventManager currentManager =
					(GridDataCellMadeVisibleEventManager)WeakEventManager.GetCurrentManager(managerType);
				if(currentManager == null) {
					currentManager = new GridDataCellMadeVisibleEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		GridDataCellMadeVisibleEventManager() { }
		public static void AddListener(PivotGridWpfData source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(PivotGridWpfData source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		protected override void StartListening(object source) {
			PivotGridWpfData data = (PivotGridWpfData)source;
			data.CellMadeVisible += OnCellMadeVisible;
		}
		protected override void StopListening(object source) {
			PivotGridWpfData data = (PivotGridWpfData)source;
			data.CellMadeVisible -= OnCellMadeVisible;
		}
		void OnCellMadeVisible(PivotGridWpfData data, CellMadeVisibleEventArgs e) {
			base.DeliverEvent(data, e);
		}
	}
	public class VisualItemsSelectionChangedEventManager : WeakEventManager {
		static VisualItemsSelectionChangedEventManager CurrentManager {
			get {
				Type managerType = typeof(VisualItemsSelectionChangedEventManager);
				VisualItemsSelectionChangedEventManager currentManager =
					(VisualItemsSelectionChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if(currentManager == null) {
					currentManager = new VisualItemsSelectionChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		VisualItemsSelectionChangedEventManager() { }
		public static void AddListener(PivotVisualItems source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(PivotVisualItems source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		protected override void StartListening(object source) {
			PivotVisualItems visualItems = (PivotVisualItems)source;
			visualItems.SelectionChanged += OnSelectionChanged;
		}
		protected override void StopListening(object source) {
			PivotVisualItems visualItems = (PivotVisualItems)source;
			visualItems.SelectionChanged -= OnSelectionChanged;
		}
		void OnSelectionChanged(object sender, EventArgs e) {
			base.DeliverEvent(sender, e);
		}
	}
	public class VisualItemsFocusedCellChangedEventManager : WeakEventManager {
		static VisualItemsFocusedCellChangedEventManager CurrentManager {
			get {
				Type managerType = typeof(VisualItemsFocusedCellChangedEventManager);
				VisualItemsFocusedCellChangedEventManager currentManager =
					(VisualItemsFocusedCellChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if(currentManager == null) {
					currentManager = new VisualItemsFocusedCellChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		VisualItemsFocusedCellChangedEventManager() { }
		public static void AddListener(PivotVisualItems source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(PivotVisualItems source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		protected override void StartListening(object source) {
			PivotVisualItems visualItems = (PivotVisualItems)source;
			visualItems.FocusedCellChanged += OnFocusedCellChanged;
		}
		protected override void StopListening(object source) {
			PivotVisualItems visualItems = (PivotVisualItems)source;
			visualItems.FocusedCellChanged -= OnFocusedCellChanged;
		}
		void OnFocusedCellChanged(object sender, FocusedCellChangedEventArgs e) {
			base.DeliverEvent(sender, e);
		}
	}
	public class PropertyChangedEventManager : WeakEventManager {
		static PropertyChangedEventManager CurrentManager {
			get {
				Type managerType = typeof(PropertyChangedEventManager);
				PropertyChangedEventManager currentManager =
					(PropertyChangedEventManager)WeakEventManager.GetCurrentManager(managerType);
				if(currentManager == null) {
					currentManager = new PropertyChangedEventManager();
					WeakEventManager.SetCurrentManager(managerType, currentManager);
				}
				return currentManager;
			}
		}
		PropertyChangedEventManager() { }
		public static void AddListener(PivotGridControl source, IWeakEventListener listener) {
			CurrentManager.ProtectedAddListener(source, listener);
		}
		public static void RemoveListener(PivotGridControl source, IWeakEventListener listener) {
			CurrentManager.ProtectedRemoveListener(source, listener);
		}
		protected override void StartListening(object source) {
			PivotGridControl FrameworkElement = (PivotGridControl)source;
			FrameworkElement.PropertyChanged += OnPropertyChanged;
		}		
		protected override void StopListening(object source) {
			PivotGridControl FrameworkElement = (PivotGridControl)source;
			FrameworkElement.PropertyChanged -= OnPropertyChanged;
		}
		void OnPropertyChanged(object sender, PivotPropertyChangedEventArgs e) {
			base.DeliverEvent(sender, e);
		}
	}
	public class PivotComplexListExtractionAlgorithm : 
#if !SL
		ComplexListExtractionAlgorithm
#else
		WrappedICollectionViewListExtractionAlgorithm
#endif
	{
	   readonly WeakReference pivot;
	   public PivotComplexListExtractionAlgorithm(PivotGridControl pivot) {
			this.pivot = new WeakReference(pivot);
		}
#if !SL
		protected override IList GetListFromICollectionView(ICollectionView collectionView) {
			return new PivotICollectionViewHelper(collectionView,
								CollectionView.NewItemPlaceholder,
								pivot.Target as PivotGridControl
			);
		}
#endif
	}
	public class PivotICollectionViewHelper : ICollectionViewHelper, IWeakEventListener  {
		readonly WeakReference pivot;
		public PivotICollectionViewHelper(ICollectionView collection, object newItemPlaceHolder, PivotGridControl pivot)
			: base(collection, newItemPlaceHolder) {
				this.pivot = new WeakReference(pivot);
		}
		public PivotICollectionViewHelper(Type elementType, ICollectionView collection, object newItemPlaceHolder, PivotGridControl pivot)
			: base(elementType, collection, newItemPlaceHolder) {
				this.pivot = new WeakReference(pivot);
		}
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			NotifyCollectionChangedEventArgs args = e as NotifyCollectionChangedEventArgs;
			if(managerType == typeof(CollectionChangedEventManager) && args.Action == NotifyCollectionChangedAction.Reset) {
				PivotGridControl pg = pivot.Target as PivotGridControl;
				if(pg != null) {
					if(pg.UseAsyncMode)
						pg.ReloadDataAsync();
					else
						pg.ReloadData();
				}
			}
			return ReceiveWeakEvent(managerType, sender, e);
		}
	}
}
