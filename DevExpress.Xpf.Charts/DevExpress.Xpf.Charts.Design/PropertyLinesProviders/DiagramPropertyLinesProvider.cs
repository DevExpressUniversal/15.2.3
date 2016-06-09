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
using System.Reflection;
using DevExpress.Charts.Designer;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Charts.Design {
	public class DiagramActionListPropertyLineContext : ActionListPropertyLineContext {
		ModelItem SelectedModelItem { get { return XpfModelItem.ToModelItem(Context.ModelItem); } }
		Diagram Diagram { get { return SelectedModelItem.View.PlatformObject as Diagram; } }
		public DiagramActionListPropertyLineContext(IPropertyLineContext context)
			: base(context) {
		}
		Uri GetImageUri(string imageUri) {
			Assembly designerAssembly = Assembly.GetAssembly(typeof(ChartDesigner));
			string assemblyName = designerAssembly.GetName().Name;
			return new Uri(string.Format("pack://application:,,,/{0};component/Images/Series/{1}_16.png", assemblyName, imageUri));
		}
		IEnumerable<Type> GetSeriesTypes() {
			IEnumerable<Type> seriesList = new Type[0];
			Diagram diagram = Diagram;
			if(diagram != null) {
				if(diagram is XYDiagram2D)
					seriesList = RegisterHelper.XYDiagram2DSeriesTypes;
				if(diagram is SimpleDiagram2D)
					seriesList = new Type[] { typeof(PieSeries2D) };
				if(diagram is RadarDiagram2D)
					seriesList = RegisterHelper.RadarDiagram2DSeriesTypes;
				if(diagram is PolarDiagram2D)
					seriesList = RegisterHelper.PolarDiagram2DSeriesTypes;
				if(diagram is XYDiagram3D)
					seriesList = RegisterHelper.XYDiagram3DSeriesTypes;
				if(diagram is SimpleDiagram3D)
					seriesList = new Type[] { typeof(PieSeries3D) };
			}
			return seriesList;
		}
		protected override void InitializeItems() {
			ObservableCollection<MenuItemInfo> items = new ObservableCollection<MenuItemInfo>();
			foreach(Type type in GetSeriesTypes()) {
				items.Add(new MenuItemInfo() { Command = SetSelectedItemCommand, Caption = type.Name, Tag = type, ImageSource = GetImageUri(type.Name) });
			}
			SelectedItem = items.Count > 0 ? items[0] : null;
			Items = items;
		}
		protected override void OnSelectedItemExecute(MenuItemInfo param) {
			Diagram diagram = Diagram;
			Series series = Activator.CreateInstance((Type)SelectedItem.Tag) as Series;
			if(series != null)
				SelectedModelItem.Properties["Series"].Collection.Add(series);
		}
	}
	public sealed class DiagramPropertyLinesProvider : PropertyLinesProviderBase {
		public DiagramPropertyLinesProvider() : base(typeof(Diagram)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new ActionListPropertyLineViewModel(new DiagramActionListPropertyLineContext(viewModel)) { Text = "Add Series" });
			return lines;
		}
	}
}
