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

using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.ViewInjection;
using DevExpress.Xpf.LayoutControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Mvvm.UI.ViewInjection {
	public class FlowLayoutControlWrapper : IItemsControlWrapper<FlowLayoutControl> {
		public FlowLayoutControl Target { get; set; }
		public object ItemsSource {
			get { return Target.ItemsSource; }
			set { Target.ItemsSource = (IEnumerable)value; }
		}
		public DataTemplate ItemTemplate {
			get { return Target.ItemTemplate; }
			set { Target.ItemTemplate = value; }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return Target.ItemTemplateSelector; }
			set { Target.ItemTemplateSelector = value; }
		}
	}
	public class TileLayoutControlWrapper : FlowLayoutControlWrapper, IItemsControlWrapper<TileLayoutControl> {
		public new TileLayoutControl Target {
			get { return (TileLayoutControl)base.Target; }
			set { base.Target = value; }
		}
	}
	public class TileLayoutControlStrategy : ItemsControlStrategy<TileLayoutControl, TileLayoutControlWrapper> {
		const string Exception2 = "ViewInjectionService cannot create view by name or type, because the target control has the ItemTemplate/ItemTemplateSelector property set.";
		DataTemplate tileItemTemplate;
		DataTemplate tileContentTemplate;
		DataTemplate GetTileItemTemplate() {
			if(tileContentTemplate == null) {
				var factory = new FrameworkElementFactory(typeof(ContentPresenter));
				factory.SetBinding(ContentPresenter.ContentProperty, new Binding());
				factory.SetValue(ContentPresenter.ContentTemplateSelectorProperty, ViewSelector);
				tileContentTemplate = new DataTemplate() { VisualTree = factory };
				tileContentTemplate.Seal();
			}
			if(tileItemTemplate == null) {
				var factory = new FrameworkElementFactory(typeof(Tile));
				factory.SetBinding(Tile.ContentProperty, new Binding());
				factory.SetValue(Tile.ContentTemplateProperty, tileContentTemplate);
				tileItemTemplate = new DataTemplate() { VisualTree = factory };
				tileItemTemplate.Seal();
			}
			return tileItemTemplate;
		}
		protected override void InitItemTemplate() {
			if(Wrapper.ItemTemplate == null && Wrapper.ItemTemplateSelector == null)
				Wrapper.ItemTemplate = GetTileItemTemplate();
		}
		protected override void CheckInjectionProcedure(object viewModel, string viewName, Type viewType) {
			if(Wrapper.ItemTemplate != GetTileItemTemplate() && (!string.IsNullOrEmpty(viewName) || viewType != null))
				throw new InvalidOperationException(Exception2);
		}
	}
}
