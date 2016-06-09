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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DevExpress.Diagram.Core.Localization;
namespace DevExpress.Diagram.Core {
	public class DiagramToolboxControlViewModel : ObservableObject {
		public DiagramToolboxControlViewModel(IEnumerable<DiagramStencil> categories) {
			this.categories = (categories == null) ? Enumerable.Empty<DiagramStencil>() : categories;
			Stencils = new ObservableCollection<StencilInfo>();
			CheckedStencils = new ObservableCollection<StencilInfo>();
			ShapesItemCollection = new ObservableCollection<ShapesItem>();
			RemoveSelectedStencilCommand = new DelegateCommand(RemoveSelectedStencil, CanRemoveSelectedStencil);
			MoveUpSelectedStencilCommand = new DelegateCommand(MoveUpSelectedStencil, CanMoveUpSelectedStencil);
			MoveDownSelectedStencilCommand = new DelegateCommand(MoveDownSelectedStencil, CanMoveDownSelectedStencil);
			SelectShapePreviewModeCommand = new DelegateCommand<string>(SelectShapePreviewMode, (x) => true);
			foreach (var stensil in this.categories) {
				Stencils.Add(new StencilInfo(stensil.Name, stensil.Id, (s, isChecked) => CheckStencil(s, isChecked), (s, isSelected) => SelectStencil(s, isSelected), true));
			}
			UpdateCommandsState();
		}
		public ICommand RemoveSelectedStencilCommand { get; set; }
		public ICommand MoveUpSelectedStencilCommand { get; set; }
		public ICommand MoveDownSelectedStencilCommand { get; set; }
		public ICommand SelectShapePreviewModeCommand { get; set; }
		IEnumerable<DiagramStencil> categories;
		ObservableCollection<StencilInfo> stencils;
		ObservableCollection<StencilInfo> checkedStencils;
		StencilInfo selectedStencil;
		ObservableCollection<ShapesItem> shapesItemCollection;
		string searchText;
		ShapeToolboxPreviewMode shapePreviewMode;
		public ObservableCollection<StencilInfo> Stencils {
			get { return stencils; }
			set { SetPropertyValue("Stencils", ref stencils, value); }
		}
		public ObservableCollection<StencilInfo> CheckedStencils {
			get { return checkedStencils; }
			set { SetPropertyValue("CheckedStencils", ref checkedStencils, value); }
		}
		public StencilInfo SelectedStencil {
			get { return selectedStencil; }
			set {
				if (selectedStencil == value)
					return;
				selectedStencil = value;
				OnSelectedStencilChanged();
				RaisePropertyChangedEvent("SelectedStencil");
			}
		}
		public string SearchText {
			get { return searchText; }
			set {
				if (searchText == value)
					return;
				searchText = (string.IsNullOrEmpty(value)) ? null : value;
				SearchTextChanged();
				RaisePropertyChangedEvent("SearchText");
			}
		}
		public ObservableCollection<ShapesItem> ShapesItemCollection {
			get { return shapesItemCollection; }
			set { SetPropertyValue("ShapesItemCollection", ref shapesItemCollection, value); }
		}
		public ShapeToolboxPreviewMode ShapePreviewMode {
			get { return shapePreviewMode; }
			set { SetPropertyValue("ShapePreviewMode", ref shapePreviewMode, value); }
		}
		void SearchTextChanged() {
			if (!string.IsNullOrEmpty(searchText)) {
				ShapesItemCollection.Clear();
				foreach (var category in categories) {
					var shapes = category.Tools.Where(x => x.ToolName.ToLower().Contains(searchText.ToLower()));
					if (shapes.Count() > 0)
						ShapesItemCollection.Add(new ShapesItem(category.Name, shapes, (x) => ClearShapesItemSelection(x)));
				}
			} else
				LoadSelectedStencilShapes();
		}
		void CheckStencil(StencilInfo stencil, bool isChecked) {
			if (isChecked) {
				if (CheckedStencils.Count == 0) {
					CheckedStencils.Add(new StencilInfo(DiagramControlLocalizer.GetString(DiagramControlStringId.QuickShapes_Name), "QuickShapes", null, (s, ch) => SelectStencil(s, ch), false));
				}
				CheckedStencils.Add(stencil);
				SelectStencil(stencil);
			} else {
				int stencilIndex = CheckedStencils.IndexOf(stencil);
				CheckedStencils.Remove(stencil);
				if (CheckedStencils.Count == 1)
					CheckedStencils.Clear();
				if (CheckedStencils.Count > stencilIndex) {
					SelectStencil(CheckedStencils.ElementAt(stencilIndex));
				} else
					SelectStencil(CheckedStencils.LastOrDefault());
			}
		}
		public void RemoveSelectedStencil() {
			StencilInfo stencil = Stencils.SingleOrDefault(x => x.Id == SelectedStencil.Id);
			if (stencil != null)
				stencil.IsChecked = false;
			UpdateCommandsState();
		}
		public bool CanRemoveSelectedStencil() {
			return SelectedStencil != null && SelectedStencil != CheckedStencils.FirstOrDefault();
		}
		public void MoveUpSelectedStencil() {
			MoveSelectedStencil(-1);
			UpdateCommandsState();
		}
		public bool CanMoveUpSelectedStencil() {
			return SelectedStencil != null && SelectedStencil != CheckedStencils.ElementAt(1) && SelectedStencil != CheckedStencils.FirstOrDefault();
		}
		public void MoveDownSelectedStencil() {
			MoveSelectedStencil(1);
			UpdateCommandsState();
		}
		public bool CanMoveDownSelectedStencil() {
			return SelectedStencil != null && SelectedStencil != CheckedStencils.LastOrDefault() && SelectedStencil != CheckedStencils.FirstOrDefault();
		}
		public void SelectShapePreviewMode(string newMode) {
			ShapePreviewMode = (ShapeToolboxPreviewMode)Enum.Parse(typeof(ShapeToolboxPreviewMode), newMode);
		}
		void MoveSelectedStencil(int distance) {
			StencilInfo stencil = Stencils.SingleOrDefault(x => x.Id == SelectedStencil.Id);
			int newIndex = CheckedStencils.IndexOf(SelectedStencil) + distance;
			CheckedStencils.Remove(SelectedStencil);
			CheckedStencils.Insert(newIndex, stencil);
			SelectStencil(stencil, true);
		}
		public void OnSelectedStencilChanged() {
			if (SelectedStencil != null)
				SelectedStencil.IsSelected = true;
			UpdateCommandsState();
			LoadSelectedStencilShapes();
			SearchText = null;
		}
		public void CheckStencils(string[] stencilsId) {
			UncheckStencils();
			if (stencilsId == null || stencilsId.Length == 0) return;
			StencilInfo stencil = null;
			foreach (string id in stencilsId) {
				stencil = Stencils.SingleOrDefault(x => x.Id == id);
				if (stencil != null)
					stencil.IsChecked = true;
			}	
			stencil = CheckedStencils.SingleOrDefault(x => x.Id == stencilsId[0]);
			if (stencil != null) {
				SelectedStencil.IsSelected = false;
				SelectStencil(stencil);
			}
		}
		void UncheckStencils() {
			foreach(var stencil in Stencils)
				stencil.IsChecked = false;
		}
		public void SelectStencil(StencilInfo stencil, bool isSelected = true) {
			if (isSelected) {
				SelectedStencil = stencil;
				if (SelectedStencil != null)
					SelectedStencil.IsSelected = true;
			}
		}
		void LoadSelectedStencilShapes() {
			ShapesItemCollection.Clear();
			if (SelectedStencil == null) {
				return;
			} else if (SelectedStencil == CheckedStencils.FirstOrDefault()) {
				LoadQuickShapesCollection();
			} else {
				DiagramStencil category = categories.SingleOrDefault(x => x.Id == SelectedStencil.Id);
				ShapesItemCollection.Add(new ShapesItem(category.Name, category.Tools, null, false));
			}
		}
		void LoadQuickShapesCollection() {
			ShapesItemCollection.Clear();
			foreach (var stencil in CheckedStencils) {
				if (stencil != CheckedStencils.FirstOrDefault()) {
					var quickShapes = new ObservableCollection<ItemTool>();
					DiagramStencil category = categories.SingleOrDefault(x => x.Id == stencil.Id);
					foreach (var tool in category.Tools.Where(x => x.IsQuick))
						quickShapes.Add(tool);
					ShapesItemCollection.Add(new ShapesItem(stencil.Name, quickShapes, (x) => ClearShapesItemSelection(x), true));
				}
			}
		}
		void ClearShapesItemSelection(ShapesItem shapesItem) {
			foreach (var item in ShapesItemCollection)
				if (item != shapesItem)
					item.SelectedTool = null;
		}
		void UpdateCommandsState() {
			((DelegateCommand)RemoveSelectedStencilCommand).RaiseCanExecuteChanged();
			((DelegateCommand)MoveUpSelectedStencilCommand).RaiseCanExecuteChanged();
			((DelegateCommand)MoveDownSelectedStencilCommand).RaiseCanExecuteChanged();
		}
	}
	public class StencilInfo : ObservableObject {
		Action<StencilInfo, bool> checkedAction;
		Action<StencilInfo, bool> selectedAction;
		bool isChecked;
		bool isSelected;
		bool menuIsVisible;
		string id;
		string name;
		public bool IsChecked {
			get { return isChecked; }
			set {
				if (isChecked == value)
					return;
				isChecked = value;
				if (checkedAction != null)
					checkedAction(this, isChecked);
				RaisePropertyChangedEvent("IsChecked");
			}
		}
		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected == value)
					return;
				isSelected = value;
				if (selectedAction != null)
					selectedAction(this, isSelected);
				RaisePropertyChangedEvent("IsSelected");
			}
		}
		public bool MenuIsVisible {
			get { return menuIsVisible; }
			set { SetPropertyValue("MenuIsVisible", ref menuIsVisible, value); }
		}
		public string Id {
			get { return id; }
			set { SetPropertyValue("Id", ref id, value); }
		}
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public StencilInfo(string name, string id, Action<StencilInfo, bool> checkedAction, Action<StencilInfo, bool> selectedAction = null, bool menuIsVisible = true) {
			Name = name;
			Id = id;
			this.checkedAction = checkedAction;
			this.selectedAction = selectedAction;
			MenuIsVisible = menuIsVisible;
		}
	}
	public class ShapesItem : ObservableObject {
		string name;
		bool nameIsVisible;
		ObservableCollection<ItemTool> tools;
		ItemTool selectedTool;
		Action<ShapesItem> selectedAction;
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public bool NameIsVisible {
			get { return nameIsVisible; }
			set { SetPropertyValue("NameIsVisible", ref nameIsVisible, value); }
		}
		public ObservableCollection<ItemTool> Tools {
			get { return tools; }
			set { SetPropertyValue("Tools", ref tools, value); }
		}
		public ItemTool SelectedTool {
			get { return selectedTool; }
			set {
				if (selectedTool == value)
					return;
				if (selectedTool == null && value != null && selectedAction != null) {
					selectedAction(this);
				}
				selectedTool = value;
				RaisePropertyChangedEvent("SelectedTool");
			}
		}
		public ShapesItem(string itemName, IEnumerable<ItemTool> tools, Action<ShapesItem> action = null, bool nameIsVisible = true) {
			Name = itemName;
			Tools = new ObservableCollection<ItemTool>();
			if (tools != null)
				foreach (var item in tools)
					Tools.Add(item);
			if (Tools.Count == 0)
				NameIsVisible = false;
			else
				NameIsVisible = nameIsVisible;
			this.selectedAction = action;
		}
	}
	public class ObservableObject : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChangedEvent(string propertyName) {
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
		public void SetPropertyValue<T>(string propertyName, ref T propertyValueHolder, T newValue) {
			if (object.Equals(propertyValueHolder, newValue))
				return;
			propertyValueHolder = newValue;
			RaisePropertyChangedEvent(propertyName);
		}
	}
	internal class DelegateCommand<T> : ICommand {
		Func<T, bool> canExecuteMethod = null;
		Action<T> executeMethod = null;
		event EventHandler canExecuteChanged;
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) {
			this.executeMethod = executeMethod;
			this.canExecuteMethod = canExecuteMethod;
		}
		public event EventHandler CanExecuteChanged {
			add { canExecuteChanged += value; }
			remove { canExecuteChanged -= value; }
		}
		public void RaiseCanExecuteChanged() {
			if (canExecuteChanged != null)
				canExecuteChanged(this, EventArgs.Empty);
		}
		public bool CanExecute(T parameter) {
			if (canExecuteMethod == null) return true;
			return canExecuteMethod(parameter);
		}
		void ICommand.Execute(object parameter) {
			Execute((T)parameter);
		}
		public void Execute(T parameter) {
			if (!CanExecute(parameter))
				return;
			if (executeMethod == null) return;
			executeMethod(parameter);
		}
		bool ICommand.CanExecute(object parameter) {
			return CanExecute((T)parameter);
		}
	}
	internal class DelegateCommand : DelegateCommand<object> {
		public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
			: base(
				executeMethod != null ? (Action<object>)(o => executeMethod()) : null,
				canExecuteMethod != null ? (Func<object, bool>)(o => canExecuteMethod()) : null) {
		}
	}
	public enum ShapeToolboxPreviewMode {
		IconsAndNames,
		NamesUnderIcons,
		IconsOnly,
		NamesOnly
	}
}
