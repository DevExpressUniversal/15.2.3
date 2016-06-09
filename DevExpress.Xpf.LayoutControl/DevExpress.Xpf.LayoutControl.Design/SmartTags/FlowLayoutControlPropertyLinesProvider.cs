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
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.LayoutControl;
using Platform::System.Windows;
using Platform::System.Windows.Controls;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
using GroupBox = Platform::DevExpress.Xpf.LayoutControl.GroupBox;
namespace DevExpress.Xpf.LayoutControl.Design {
	sealed class FlowLayoutControlPropertyLinesProvider : PropertyLinesProviderBase {
		public FlowLayoutControlPropertyLinesProvider()
			: base(typeof(FlowLayoutControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			if(!typeof(TileLayoutControl).IsAssignableFrom(viewModel.RuntimeSelectedItem.ItemType))
				lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateNewGroupBoxCommandAction(viewModel)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => FlowLayoutControl.MaximizedElementPositionProperty), typeof(MaximizedElementPosition)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => FlowLayoutControl.OrientationProperty), typeof(Orientation)));
			return lines;
		}
	}
	class CreateNewGroupBoxCommandAction : CreateItemCommandProviderBase {
		public CreateNewGroupBoxCommandAction(IPropertyLineContext context)
			: base(context) { }
		protected override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(GroupBox), CreateOptions.InitializeDefaults);
		}
		protected override string GetCommandText() {
			return "Add a new item";
		}
	}
	sealed class GroupBoxPropertyLinesProvider : PropertyLinesProviderBase {
		public GroupBoxPropertyLinesProvider()
			: base(typeof(GroupBox)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			bool isInDockLayoutControl = BarManagerDesignTimeHelper.FindParentByType<DockLayoutControl>(XpfModelItem.ToModelItem(viewModel.RuntimeSelectedItem)) != null;
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => GroupBox.HeaderProperty)));
			if(isInDockLayoutControl) {
				lines.Add(() => new EnumPropertyLineViewModel(viewModel, "Dock", typeof(Platform::DevExpress.Xpf.LayoutControl.Dock), typeof(DockLayoutControl)));
			}
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => GroupBox.MaximizeElementVisibilityProperty), typeof(Visibility)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => GroupBox.MinimizeElementVisibilityProperty), typeof(Visibility)));
			return lines;
		}
	}
}
