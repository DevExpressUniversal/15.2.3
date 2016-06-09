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
using System.Windows;
using DevExpress.Design.SmartTags;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
namespace DevExpress.Xpf.Core.Design.Services.PropertyLinesProviders {
	public sealed class EventToCommandPropertyLinesProvider : PropertyLinesProviderBase {
		ObjectPropertyLineViewModel EventNamePropertyLine { get; set; }
		public EventToCommandPropertyLinesProvider() : base(typeof(EventToCommand)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			viewModel.PropertyChanged += OnPropertyChanged;
			EventNamePropertyLine = new ObjectPropertyLineViewModel(viewModel, EventToCommand.EventNameProperty.Name);
			lines.Add(() => EventNamePropertyLine);
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, EventToCommand.SourceNameProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, EventToCommand.SourceObjectProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, EventToCommand.AllowChangingEventOwnerIsEnabledProperty.Name));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, EventToCommand.CommandProperty.Name));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, EventToCommand.CommandParameterProperty.Name));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, EventToCommand.DispatcherPriorityProperty.Name, typeof(System.Windows.Threading.DispatcherPriority)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, EventToCommand.MarkRoutedEventsAsHandledProperty.Name));
			lines.Add(() => new ItemListPropertyLineViewModel(viewModel, EventToCommand.EventArgsConverterProperty.Name, typeof(IEventArgsConverter), DXTypeInfoInstanceSource.FromTypeList(new Type[] { typeof(EventArgsToDataCellConverter), typeof(EventArgsToDataRowConverter), typeof(ItemsControlMouseEventArgsConverter) })));
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, EventToCommand.PassEventArgsToCommandProperty.Name));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, EventToCommand.ProcessEventsFromDisabledEventOwnerProperty.Name));
			lines.Add(() => new NullableBooleanPropertyLineViewModel(viewModel, EventToCommand.UseDispatcherProperty.Name));
			EventNamePropertyLine.ItemsSource = GetSourceObjectBySourceName(EventNamePropertyLine.SelectedItem);
			return lines;
		}
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == EventToCommand.SourceNameProperty.Name) {
				EventNamePropertyLine.ItemsSource = GetSourceObjectBySourceName(EventNamePropertyLine.SelectedItem);
			}
		}
		IEnumerable<object> GetSourceObjectBySourceName(IModelItem selectedItem) {
			var modelItem = XpfModelItem.ToModelItem(selectedItem);
			if(selectedItem == null || modelItem == null)
				return null;
			var service = modelItem.Context.Services.GetService<ModelService>();
			ModelItem sourceObject = null;
			string sourceName = selectedItem.Properties["SourceName"].ComputedValue as string;
			sourceObject = string.IsNullOrEmpty(sourceName) ? XpfModelItem.ToModelItem(selectedItem.Parent) : service.FromName(modelItem.Root, sourceName);
			return sourceObject == null ? null : sourceObject.Events.Select((ev) => ev.Name);
		}
	}
}
