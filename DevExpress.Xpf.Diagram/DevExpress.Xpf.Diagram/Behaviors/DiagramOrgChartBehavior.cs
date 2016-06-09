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

using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Layout;
namespace DevExpress.Xpf.Diagram {
	using DevExpress.Mvvm.Native;
	using DevExpress.Mvvm.UI.Interactivity;
	using DevExpress.Mvvm.UI.Native;
	using System;
	using System.Runtime.CompilerServices;
	using System.Windows;
	using System.Windows.Controls;
	public partial class DiagramOrgChartBehavior : Behavior<DiagramControl>, IOrgChartBehavior {
		static readonly ItemTool DefaultTool = new ShapeTool(BasicShapes.Rectangle);
		static DiagramOrgChartBehavior() {
			DependencyPropertyRegistrator<DiagramOrgChartBehavior>.New()
				.Register(x => x.ItemTemplate, out ItemTemplateProperty, default(DataTemplate))
				.Register(x => x.ItemContainerStyle, out ItemContainerStyleProperty, default(Style))
				.Register(x => x.ItemTemplateSelector, out ItemTemplateSelectorProperty, default(DataTemplateSelector))
				.Register(x => x.Margin, out MarginProperty, default(double))
				;
		}
		readonly OrgChartController controller;
		public DiagramOrgChartBehavior() {
			controller = new OrgChartController(this);
		}
		IDiagramControl Diagram { get { return AssociatedObject; } }
		protected override void OnAttached() {
			base.OnAttached();
			if(AssociatedObject.IsLoaded)
				controller.OnAttached();
			else
				AssociatedObject.Loaded += AssociatedObject_Loaded;
		}
		void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			controller.OnAttached();
		}
		protected override void OnDetaching() {
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			controller.OnDetaching();
			base.OnDetaching();
		}
		ConditionalWeakTable<DataTemplate, Type> templateToTypesMap = new ConditionalWeakTable<DataTemplate, Type>();
		DiagramContentItem CreateContentItem(object dataItem) {
			return new DiagramContentItem() {
				Content = dataItem,
				ContentTemplate = ItemTemplate,
				ContentTemplateSelector = ItemTemplateSelector,
				Style = ItemContainerStyle,
			};
		}
		object TryCreateSelfHostedItem(DataTemplate template, object dataItem) {
			return template.LoadContent()
				.Do(content => (content as DiagramItem).Do(item => item.DataContext = dataItem));
		}
		DataTemplate ChooseTemplate(object dataItem) {
			return ItemTemplateSelector.With(x => x.SelectTemplate(dataItem, AssociatedObject)) ?? ItemTemplate;
		}
		IDiagramItem IOrgChartBehavior.CreateItem(object dataItem) {
			var template = ChooseTemplate(dataItem);
			if(template == null) {
				var item = (IDiagramShape)DefaultTool.CreateItem(Diagram);
				item.Size = DefaultTool.DefaultItemSize;
				item.Content = dataItem.ToString();
				return item;
			}
			Type templateContentType;
			if(templateToTypesMap.TryGetValue(template, out templateContentType)) {
				if(typeof(DiagramItem).IsAssignableFrom(templateContentType)) {
					return (DiagramItem)TryCreateSelfHostedItem(template, dataItem);
				} else {
					return CreateContentItem(dataItem);
				}
			} else {
				var content = TryCreateSelfHostedItem(template, dataItem);
				templateToTypesMap.Add(template, content.GetType());
				return (content as DiagramItem) ?? CreateContentItem(dataItem);
			}
		}
		IDiagramControl IOrgChartBehavior.Diagram {
			get { return AssociatedObject; }
		}
	}
}
