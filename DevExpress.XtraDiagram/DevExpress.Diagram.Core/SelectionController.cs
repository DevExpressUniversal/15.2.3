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
using System;
using System.Collections.ObjectModel;
using DevExpress.Internal;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.Diagram.Core.TypeConverters;
namespace DevExpress.Diagram.Core {
	[TypeConverter(typeof(CustomObjectTypeConverter))]
	public interface ISelectionLayer {
		ISelectionLayerHandler CreateSelectionHandler(IDiagramControl diagram);
	}
	public interface ISelectionLayerHandler {
		void UpdateSelectionAdorners();
		void RecreateSelectionAdorners();
		void Clear();
		bool CanChangeZOrder { get; }
	}
	public enum ItemChangedKind {
		Bounds,
		Added,
		Removed,
		ZOrderChanged,
		Interaction,
	}
	#region SelectionController
	public class SelectionController {
		readonly IDiagramControl diagram;
		readonly ISelectionLayer defaultSelectionLayer;
		List<IDiagramItem> selectedItemsCore = new List<IDiagramItem>();
		public ReadOnlyCollection<IDiagramItem> SelectedItems { get; private set; }
		public IDiagramItem PrimarySelection { get { return SelectedItems.FirstOrDefault(); } }
		ISelectionLayer layer;
		Lazy<ISelectionLayerHandler> handler;
#if DEBUGTEST
		public ISelectionLayerHandler HandlerForTests { get { return handler.Value; } }
#endif
		bool AllowEmptySelection { get { return diagram.AllowEmptySelection; } }
		public SelectionController(IDiagramControl diagram, ISelectionLayer defaultSelectionLayer) {
			this.diagram = diagram;
			this.defaultSelectionLayer = defaultSelectionLayer;
			SelectedItems = selectedItemsCore.AsReadOnly();
			UpdateSelectionLayer(defaultSelectionLayer);
		}
		public bool CanChangeZOrder { get { return SelectedItems.Any() && handler.Value.CanChangeZOrder; } }
		public bool IsInSelection(IDiagramItem item) {
			return SelectedItems.Contains(item);
		}
		public void SelectItem(IDiagramItem item, bool addToSelection = false) {
			SelectItems(item.Yield(), addToSelection);
		}
		public void SelectItems(IEnumerable<IDiagramItem> items, bool addToSelection = false) {
			foreach(var item in items) {
				Guard.ArgumentNotNull(item, "item");
				if(item.GetRootDiagram() != diagram)
					throw new InvalidOperationException();
			}
			items = items.Where(x => x.Controller.ActualCanSelect);
			if(!addToSelection) {
				if(!items.Any()) {
					ClearSelection();
					return;
				}
				if(selectedItemsCore.SequenceEqual(items))
					return;
				ClearSelectionCore();
			}
			UpdateSelectionLayer(items.FirstOrDefault().With(x => x.SelectionLayer) ?? defaultSelectionLayer);
			var itemsToSelect = items.Where(x => !x.IsSelected);
			if(!itemsToSelect.Any()) {
				FallbackSelection();
				return;
			}
			selectedItemsCore.AddRange(itemsToSelect);
			foreach(var item in selectedItemsCore.Where(x => x.SelectionLayer != layer).ToArray()) {
				item.IsSelected = false;
				selectedItemsCore.Remove(item);
			}
			selectedItemsCore.ForEach(x => x.IsSelected = true);
			RecreateAndUpdateSelectionAdorners();
			OnSelectionChanged();
		}
		bool FallbackSelection() {
			if(!AllowEmptySelection) {
				SelectItem(PrimarySelection.With(x => x.Owner()) ?? diagram.RootItem());
				return true;
			}
			return false;
		}
		public void UnselectItem(IDiagramItem item, bool deletingItem) {
			if(!AllowEmptySelection && SelectedItems.Count <= 1) {
				if(deletingItem)
					ClearSelection();
				return;
			}
			item.IsSelected = false;
			if(!selectedItemsCore.Remove(item)) {
				return;
			}
			RecreateAndUpdateSelectionAdorners();
			OnSelectionChanged();
		}
		public void ClearSelection() {
			if(!selectedItemsCore.Any())
				return;
			if(FallbackSelection()) {
				return;
			}
			ClearSelectionCore();
			RecreateAndUpdateSelectionAdorners();
			OnSelectionChanged();
		}
		public void ClearSelectionOrMakePrimarySelection(IDiagramItem item) {
			if(AllowEmptySelection) {
				ClearSelection();
			} else if(item != null) {
				if(selectedItemsCore.Count > 1) {
					SelectItems(item.Yield().Concat(SelectedItems.Except(item.Yield())).ToArray());
				}
			}
		}
		void ClearSelectionCore() {
			selectedItemsCore.ForEach(x => x.IsSelected = false);
			selectedItemsCore.Clear();
		}
		void OnSelectionChanged() {
			diagram.Controller.OnSelectionChanged();
		}
		public void ItemChanged(IDiagramItem item, ItemChangedKind kind) {
			if(!SelectedItems.Contains(item))
				return;
			switch(kind) {
				case ItemChangedKind.Bounds:
					UpdateSelectionAdorners();
					break;
				case ItemChangedKind.Removed:
					UnselectItem(item, deletingItem: true);
					break;
			}
		}
		void UpdateSelectionLayer(ISelectionLayer newLayer) {
			if(layer == newLayer)
				return;
			layer = newLayer;
			if(handler != null && handler.IsValueCreated)
				handler.Value.Clear();
			handler = new Lazy<ISelectionLayerHandler>(() => layer.CreateSelectionHandler(diagram));
		}
		public void UpdateSelectionAdorners() {
			handler.Value.UpdateSelectionAdorners();
		}
		public void RecreateAndUpdateSelectionAdorners() {
			handler.Value.RecreateSelectionAdorners();
			handler.Value.UpdateSelectionAdorners();
		}
		public void ValidateSelection() {
			if(!AllowEmptySelection && !SelectedItems.Any())
				SelectItem(diagram.RootItem());
		}
	}
	#endregion
}
