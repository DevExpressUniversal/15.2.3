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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using DevExpress.Design.SmartTags;
#if SILVERLIGHT
using AssemblyInfo = Platform::AssemblyInfo;
using PivotGridControl = Platform.DevExpress.Xpf.PivotGrid.PivotGridControl;
using PivotGridField = Platform.DevExpress.Xpf.PivotGrid.PivotGridField;
using IPivotOLAPDataSource = Platform.DevExpress.XtraPivotGrid.Data.IPivotOLAPDataSource;
using PivotGridWpfData = Platform.DevExpress.Xpf.PivotGrid.Internal.PivotGridWpfData;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
using FieldArea = Platform.DevExpress.Xpf.PivotGrid.FieldArea;
using FieldUnboundColumnType = Platform.DevExpress.Xpf.PivotGrid.FieldUnboundColumnType;
using FieldSummaryType = Platform.DevExpress.Xpf.PivotGrid.FieldSummaryType;
using FieldSummaryDisplayType = Platform.DevExpress.Xpf.PivotGrid.FieldSummaryDisplayType;
using PivotGridData = Platform.DevExpress.XtraPivotGrid.Data.PivotGridData;
using PivotGridDataAsync = Platform.DevExpress.XtraPivotGrid.Data.PivotGridDataAsync;
using PivotArea = Platform.DevExpress.XtraPivotGrid.PivotArea;
using PivotGridEmptyEventsImplementorBase = Platform.DevExpress.PivotGrid.Events.PivotGridEmptyEventsImplementorBase;
using FieldGroupInterval = Platform.DevExpress.Xpf.PivotGrid.FieldGroupInterval;
using System.Windows;
using System.Windows.Input;
#else
using DevExpress.Data;
using System.Windows.Input;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid;
using DevExpress.PivotGrid.Events;
using DevExpress.Data.Browsing.Design;
#endif
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Xpf.PivotGrid.Design {
#if SILVERLIGHT
	class PivotTreeViewPropertyLineViewModel : TreeViewPropertyLineViewModel {
		public PivotTreeViewPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type propertyType, IPropertyLinePlatformInfo info)
			: base(context, propertyName, propertyType, null, info) { }
		class PivotTreeViewItemsViewModel : TreeViewItemsViewModel {
			public PivotTreeViewItemsViewModel(PivotTreeViewPropertyLineViewModel treeViewPropertyLineViewModel) : base(treeViewPropertyLineViewModel) { }
			protected override IEnumerable<Core.Design.AssignDataContextDialog.ITreeViewItemViewModel> GetItems() {
				return base.GetItems();
			}
			public override void Update() { }
		}
		protected override TreeViewItemsViewModel CreateTreeViewItemsViewModel() {
			return new PivotTreeViewItemsViewModel(this);
		}
	}
#endif
	sealed class FieldPropertyLinesProvider : PropertyLinesProviderBase {
		public FieldPropertyLinesProvider() : base(typeof(PivotGridField)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			IModelProperty areaProperty = viewModel.RuntimeSelectedItem.Properties[PivotGridDesignTimeHelper.AreaPropertyName];
			IModelProperty fieldNameProperty = viewModel.RuntimeSelectedItem.Properties[PivotGridDesignTimeHelper.FieldNamePropertyName];
			string fieldName = fieldNameProperty.ComputedValue as string;
			IModelProperty olapConnectionStringProperty = GetPivotGridOlapConnectionStringProperty(viewModel);
			string value = olapConnectionStringProperty.IsSet ? olapConnectionStringProperty.ComputedValue as string : null;
			if(value == null && olapConnectionStringProperty.IsSet && olapConnectionStringProperty.Value != null) {
				value = olapConnectionStringProperty.Value.GetCurrentValue() as string;
			}
			PivotGridControl pivot = (PivotGridControl)viewModel.RuntimeSelectedItem.Parent.GetCurrentValue();
			PivotGridWpfData instantData = PivotGridControl.GetData(pivot);
			bool? showDataOnly = areaProperty.IsSet ? ((FieldArea)areaProperty.ComputedValue) == FieldArea.DataArea : (bool?)null;
			SmartTagLineViewModelBase model;
			if(value != null) {
#if SILVERLIGHT
				TreeViewPropertyLineViewModel fieldNameModel = new PivotTreeViewPropertyLineViewModel(viewModel, PivotGridDesignTimeHelper.FieldNamePropertyName, typeof(string), ((IPropertyLineContext)viewModel).PlatformInfoFactory.ForStandardProperty("string"));
				model = fieldNameModel;
				OlapFieldsPopulator.UpdateFields(fieldName, value, instantData, showDataOnly, fieldNameModel);
				fieldNameModel.CustomSelectCommand = () => {
					var item = fieldNameModel.TreeViewItemsViewModel.SelectedItem;
					PivotGridDesignTimeHelper.SetField(fieldNameModel.SelectedItem, item.SelectValue, item.DisplayText);
					return true;
				};
#else
				model = new PivotGridFieldPropertyLineViewModel(instantData, fieldName, value, showDataOnly, viewModel, PivotGridDesignTimeHelper.FieldNamePropertyName);
#endif
			} else {
				string[] props = instantData.ListDataSource.GetFieldList();
				if(props == null || props.Length == 0)
					model = new ObjectPropertyLineViewModel(viewModel, PivotGridDesignTimeHelper.FieldNamePropertyName);
				else {
					List<object> nodes = new List<object>();
					model = new ItemListPropertyLineViewModel(viewModel, PivotGridDesignTimeHelper.FieldNamePropertyName, typeof(string), ObjectInstanceSource.FromList(props));
				}
			}
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, PivotGridDesignTimeHelper.AreaPropertyName, typeof(FieldArea)));
			lines.Add(() => model);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, PivotGridDesignTimeHelper.CaptionPropertyName));
			EnumDepandantPropertyLineViewModel.EnumDepandantPropertyLineViewModelDeleagate action = (p, e) => {
				if(p == PivotGridDesignTimeHelper.AreaPropertyName)
					e.IsVisible = areaProperty.IsSet && ((FieldArea)areaProperty.ComputedValue) == FieldArea.DataArea;
			};
			if(!olapConnectionStringProperty.IsSet) {
				Type dataType = viewModel.RuntimeSelectedItem.Properties["DataType"].ComputedValue as Type;
				lines.Add(() => new EnumDepandantPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PivotGridField.GroupIntervalProperty), typeof(FieldGroupInterval),
					(p, e) => {
						if(p == PivotGridDesignTimeHelper.FieldNamePropertyName)
							e.IsVisible = (e.SelectedItem.Properties["DataType"].ComputedValue as Type) == typeof(DateTime);
					}) {
						IsVisible = dataType == typeof(DateTime)
					}
				);
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PivotGridField.UnboundTypeProperty), typeof(FieldUnboundColumnType)));
				lines.Add(() => new ObjectDependantPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PivotGridField.UnboundExpressionProperty),
					(p, e) => {
						if(p == DependencyPropertyHelper.GetPropertyName(() => PivotGridField.UnboundTypeProperty))
							e.IsVisible = !object.Equals(e.SelectedItem.Properties[DependencyPropertyHelper.GetPropertyName(() => PivotGridField.UnboundTypeProperty)].ComputedValue, FieldUnboundColumnType.Bound);
					}) {
						IsVisible = !object.Equals(viewModel.RuntimeSelectedItem.Properties[DependencyPropertyHelper.GetPropertyName(() => PivotGridField.UnboundTypeProperty)].ComputedValue, FieldUnboundColumnType.Bound)
#if !SILVERLIGHT
,
						Command = new ShowUnboundEditorCommand(viewModel)
#endif
					}
				);
				lines.Add(() => new EnumDepandantPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PivotGridField.SummaryTypeProperty), typeof(FieldSummaryType), action) {
					IsVisible = areaProperty.IsSet && ((FieldArea)areaProperty.ComputedValue) == FieldArea.DataArea
				});
				lines.Add(() => new EnumDepandantPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => PivotGridField.SummaryDisplayTypeProperty), typeof(FieldSummaryDisplayType), action));
			}
			return lines;
		}
		internal static IModelProperty GetPivotGridOlapConnectionStringProperty(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			IModelProperty olapConnectionStringProperty = viewModel.RuntimeSelectedItem.Parent.Properties[DependencyPropertyHelper.GetPropertyName(() => PivotGridControl.OlapConnectionStringProperty)];
			return olapConnectionStringProperty;
		}
	}
	class ObjectDependantPropertyLineViewModel : ObjectPropertyLineViewModel {
		EnumDepandantPropertyLineViewModel.EnumDepandantPropertyLineViewModelDeleagate action;
		public ObjectDependantPropertyLineViewModel(IPropertyLineContext context, string propertyName, EnumDepandantPropertyLineViewModel.EnumDepandantPropertyLineViewModelDeleagate action, IEnumerable<object> itemsSource = null) :
			base(context, propertyName) {
			ItemsSource = itemsSource;
			this.action = action;
		}
		public override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			if(action != null)
				action(propertyName, this);
		}
	}
	class EnumDepandantPropertyLineViewModel : EnumPropertyLineViewModel {
		public delegate void EnumDepandantPropertyLineViewModelDeleagate(string propertyName, SmartTagLineViewModelBase model);
		EnumDepandantPropertyLineViewModelDeleagate action;
		public EnumDepandantPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type enumType, EnumDepandantPropertyLineViewModelDeleagate action, ProvideEnumValuesCallback provideValuesCallback = null) :
			base(context, propertyName, enumType, null, provideValuesCallback) {
			this.action = action;
		}
		public override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			if(action != null)
				action(propertyName, this);
		}
	}
	class ShowUnboundEditorCommand : ICommand {
		FrameworkElementSmartTagPropertiesViewModel viewModel;
		EventHandler OnCanExecuteChanged = null;
		public ShowUnboundEditorCommand(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			this.viewModel = viewModel;
			viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(viewModel_PropertyChanged);
		}
		void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == DependencyPropertyHelper.GetPropertyName(() => PivotGridField.UnboundTypeProperty) && OnCanExecuteChanged != null)
				OnCanExecuteChanged.Invoke(null, null);
		}
		bool ICommand.CanExecute(object parameter) {
			PivotGridField field = viewModel.RuntimeSelectedItem.GetCurrentValue() as PivotGridField;
			PivotGridControl pivot = viewModel.RuntimeSelectedItem.Parent.GetCurrentValue() as PivotGridControl;
			if(field == null || pivot == null)
				return false;
			return !FieldPropertyLinesProvider.GetPivotGridOlapConnectionStringProperty(viewModel).IsSet;
		}
		event EventHandler ICommand.CanExecuteChanged {
			add { OnCanExecuteChanged += value; }
			remove { OnCanExecuteChanged -= value; }
		}
		void ICommand.Execute(object parameter) {
			PivotGridField field = viewModel.RuntimeSelectedItem.GetCurrentValue() as PivotGridField;
			PivotGridControl pivot = viewModel.RuntimeSelectedItem.Parent.GetCurrentValue() as PivotGridControl;
			if(field == null || pivot == null)
				return;
			pivot.ShowUnboundExpressionEditor(field);
		}
	}
	class PivotGridEventsImplementor : PivotGridEmptyEventsImplementorBase {
		public PivotGridEventsImplementor() { }
		public override bool QueryException(Exception ex) {
			return true;
		}
	}
}
