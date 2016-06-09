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
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class PreviousElementSelector {
		static readonly Dictionary<Type, Type> ownerTypes = new Dictionary<Type, Type>() {
			{ typeof(Series), typeof(Chart) },
			{ typeof(Annotation), typeof(Chart) },
			{ typeof(ChartTitle), typeof(Chart) },
			{ typeof(AxisBase), typeof(Diagram) },
			{ typeof(XYDiagramPane), typeof(Diagram) },
			{ typeof(ConstantLine), typeof(AxisBase) },
			{ typeof(ScaleBreak), typeof(AxisBase) },
			{ typeof(Strip), typeof(AxisBase) },
			{ typeof(Indicator), typeof(Series) },
			{ typeof(SeriesTitle), typeof(Series) }
		};
		readonly ChartCollectionBase collection;
		public PreviousElementSelector(ChartCollectionBase collection) {
			this.collection = collection;
		}
		bool IsLastIndex(int index) {
			return index == collection.Count - 1;
		}
		object GetPreviuosCollectionElement(int sourceIndex) {
			if (collection.Count == 1)
				return null;
			int index = IsLastIndex(sourceIndex) ? sourceIndex - 1 : sourceIndex + 1;
			return collection.GetElementByIndex(index);
		}
		object GetParentElement(object sourceElement, Type parentType) {
			if (sourceElement != null && parentType.IsAssignableFrom(sourceElement.GetType()))
				return sourceElement;
			IOwnedElement ownedElement = sourceElement as IOwnedElement;
			return ownedElement != null ? GetParentElement(ownedElement.Owner, parentType) : null;
		}
		Type GetParentType(Type type) {
			Type parentType = null;
			foreach (Type itemType in ownerTypes.Keys) {
				if (itemType.IsAssignableFrom(type)) {
					parentType = ownerTypes[itemType];
					break;
				}
			}
			return parentType;
		}
		public object GetObjectToSelect(object sourceElement, int sourceElementIndex) {
			object previuosCollectionElement = GetPreviuosCollectionElement(sourceElementIndex);
			if (previuosCollectionElement != null)
				return previuosCollectionElement;
			else {
				Type parentType = GetParentType(sourceElement.GetType());
				return parentType != null ? GetParentElement(sourceElement, parentType) : null;
			}
		}
	}
	public abstract class DeleteCommandBase<TChartElement> : ChartCommandBase where TChartElement : ChartElement {
		PreviousElementSelector previousElementSelector;
		PreviousElementSelector PreviousElementSelector {
			get {
				if (previousElementSelector == null)
					previousElementSelector = new PreviousElementSelector(ChartCollection);
				return previousElementSelector;
			}
		}
		protected internal override bool CanDisposeOldValue { get { return true; } }
		protected abstract ChartCollectionBase ChartCollection { get; }
		protected DeleteCommandBase(CommandManager commandManager)
			: base(commandManager) {
		}
		protected abstract void InsertIntoCollection(int index, TChartElement chartElement);
		protected virtual void RemoveFromCollection(TChartElement chartElement) {
			ChartCollection.Remove(chartElement);
		}
		protected override object CreatePropertiesCache(ChartElement chartElement) {
			return CreateCollectionPropertiesCache((TChartElement)chartElement);
		}
		protected virtual object CreateCollectionPropertiesCache(TChartElement chartElement) {
			return null;
		}
		protected override void RestoreProperties(ChartElement chartElement, object properties) {
			RestoreCollectionProperties((TChartElement)chartElement, properties);
		}
		protected virtual void RestoreCollectionProperties(TChartElement chartElement, object properties) {
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			TChartElement chartElement = parameter as TChartElement;
			if (chartElement != null) {
				int index = ChartCollection.IndexOf(chartElement);
				object properties = CreateCollectionPropertiesCache(chartElement);
				object objectToSelect = PreviousElementSelector.GetObjectToSelect(chartElement, index);
				RemoveFromCollection(chartElement);
				HistoryItem hItem = new HistoryItem(this, chartElement, null, index) { ObjectToSelect = objectToSelect };
				hItem.TargetObject = properties;
				return hItem;
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			TChartElement chartElement = (TChartElement)historyItem.OldValue;
			int index = (int)historyItem.Parameter;
			InsertIntoCollection(index, chartElement);
			RestoreCollectionProperties(chartElement, historyItem.TargetObject);
			historyItem.ObjectToSelect = chartElement;
		}
		public override void RedoInternal(HistoryItem historyItem) {
			TChartElement chartElement = (TChartElement)historyItem.OldValue;
			historyItem.ObjectToSelect = PreviousElementSelector.GetObjectToSelect(chartElement, (int)historyItem.Parameter);
			ChartCollection.Remove(chartElement);
		}
	}
	public abstract class AddCommandBase<TChartElement> : ChartCommandBase where TChartElement : ChartElement {
		PreviousElementSelector previousElementSelector;
		PreviousElementSelector PreviousElementSelector {
			get {
				if (previousElementSelector == null)
					previousElementSelector = new PreviousElementSelector(ChartCollection);
				return previousElementSelector;
			}
		}
		protected internal override bool CanDisposeNewValue { get { return true; } }
		protected abstract ChartCollectionBase ChartCollection { get; }
		protected AddCommandBase(CommandManager commandManager)
			: base(commandManager) {
		}
		protected virtual void GenerateName(TChartElement chartElement) {
			ChartElementNamed namedElement = chartElement as ChartElementNamed;
			if (namedElement != null) {
				string name = ((ChartElementNamedCollection)ChartCollection).GenerateName();
				namedElement.Name = name;
			}
		}
		protected abstract void AddToCollection(TChartElement chartElement);
		protected abstract TChartElement CreateChartElement(object parameter);
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			TChartElement chartElement = CreateChartElement(parameter);
			if (chartElement != null) {
				GenerateName(chartElement);
				AddToCollection(chartElement);
				return new HistoryItem(this, null, chartElement, null) { ObjectToSelect = chartElement };
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			TChartElement chartElement = (TChartElement)historyItem.NewValue;
			historyItem.ObjectToSelect = PreviousElementSelector.GetObjectToSelect(chartElement, ChartCollection.Count - 1);
			ChartCollection.Remove(chartElement);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			TChartElement chartElement = (TChartElement)historyItem.NewValue;
			AddToCollection(chartElement);
			historyItem.ObjectToSelect = chartElement;
		}
	}
	public class SwapCommandParameter {
		readonly int oldIndex;
		readonly int newIndex;
		public int OldIndex { get { return oldIndex; } }
		public int NewIndex { get { return newIndex; } }
		public SwapCommandParameter(int oldIndex, int newIndex) {
			this.oldIndex = oldIndex;
			this.newIndex = newIndex;
		}
	}
	public class SwapCommand : ChartCommandBase {
		readonly ChartCollectionBase collection;
		public SwapCommand(CommandManager commandManager, ChartCollectionBase collection)
			: base(commandManager) {
			this.collection = collection;
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			SwapCommandParameter swapParameter = parameter as SwapCommandParameter;
			if (swapParameter != null) {
				collection.Swap(swapParameter.OldIndex, swapParameter.NewIndex);
				return new HistoryItem(this, null, null, swapParameter);
			}
			return null;
		}
		public override void UndoInternal(HistoryItem historyItem) {
			SwapCommandParameter swapParameter = (SwapCommandParameter)historyItem.Parameter;
			collection.Swap(swapParameter.NewIndex, swapParameter.OldIndex);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			SwapCommandParameter swapParameter = (SwapCommandParameter)historyItem.Parameter;
			collection.Swap(swapParameter.OldIndex, swapParameter.NewIndex);
		}
	}
	public abstract class ClearCommandBase<TChartElement> : ChartCommandBase where TChartElement : ChartElement {
		protected internal override bool CanDisposeOldValue { get { return true; } }
		protected abstract ChartCollectionBase ChartCollection { get; }
		public ClearCommandBase(CommandManager commandManager)
			: base(commandManager) {
		}
		List<TChartElement> MakeCollectionSnapshot(ChartCollectionBase collection) {
			List<TChartElement> collectionSnapshot = new List<TChartElement>();
			foreach (TChartElement chartElement in collection) {
				collectionSnapshot.Add(chartElement);
			}
			return collectionSnapshot;
		}
		void RestoreCollectionFromSnapshot(List<TChartElement> collectionSnapshot) {
			foreach (TChartElement chartElement in collectionSnapshot) {
				AddToCollection(chartElement);
			}
		}
		protected abstract void AddToCollection(TChartElement chartElement);
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			List<TChartElement> collectionSnapshot = MakeCollectionSnapshot(ChartCollection);
			ChartCollection.Clear();
			return new HistoryItem(this, collectionSnapshot, null, null);
		}
		public override void UndoInternal(HistoryItem historyItem) {
			RestoreCollectionFromSnapshot((List<TChartElement>)historyItem.OldValue);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			ChartCollection.Clear();
		}
	}
}
