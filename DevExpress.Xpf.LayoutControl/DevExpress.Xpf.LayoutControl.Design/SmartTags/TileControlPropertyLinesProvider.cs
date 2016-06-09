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
using DependencyPropertyHelper = DevExpress.Xpf.Core.Design.DependencyPropertyHelper;
namespace DevExpress.Xpf.LayoutControl.Design {
	sealed class TileLayoutControlPropertyLinesProvider : PropertyLinesProviderBase {
		public TileLayoutControlPropertyLinesProvider() : base(typeof(TileLayoutControl)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateTileCommandProvider(viewModel, TileSize.ExtraSmall)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateTileCommandProvider(viewModel, TileSize.Small)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateTileCommandProvider(viewModel, TileSize.Large)));
			lines.Add(() => ActionPropertyLineViewModel.CreateLine(new CreateTileCommandProvider(viewModel, TileSize.ExtraLarge)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => TileLayoutControl.TileClickCommandProperty)));
			return lines;
		}
	}
	class CreateTileCommandProvider : CreateItemCommandProviderBase {
		public TileSize TileSize { get; private set; }
		public CreateTileCommandProvider(IPropertyLineContext context, TileSize tileSize)
			: base(context) {
			TileSize = tileSize;
		}
		protected override string GetCommandText() {
			return string.Format("Add {0} Tile", GetString(TileSize));
		}
		string GetString(TileSize tileSize) {
			switch(tileSize) {
				case TileSize.ExtraLarge:
					return "Extra Large";
				case TileSize.ExtraSmall:
					return "Extra Small";
				default:
					return tileSize.ToString();
			}
		}
		protected override ModelItem CreateItem(EditingContext context) {
			ModelItem tile = ModelFactory.CreateItem(context, typeof(Tile), CreateOptions.InitializeDefaults);
			tile.Properties["Size"].SetValue(TileSize);
			return tile;
		}
	}
	sealed class TilePropertyLinesProvider : PropertyLinesProviderBase {
		public TilePropertyLinesProvider()
			: base(typeof(Tile)) { }
		protected override SmartTagLineViewModelFactoryList GetPropertiesImpl(FrameworkElementSmartTagPropertiesViewModel viewModel) {
			SmartTagLineViewModelFactoryList lines = new SmartTagLineViewModelFactoryList();
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.AnimateContentChangeProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, "GroupHeader", typeof(TileLayoutControl)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.HeaderProperty)));
			lines.Add(() => new CommandPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.CommandProperty)));
			lines.Add(() => new ObjectPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.CommandParameterProperty)));
			lines.Add(() => new BooleanPropertyLineViewModel(viewModel, "IsFlowBreak", typeof(TileLayoutControl)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.SizeProperty), typeof(TileSize)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.HorizontalHeaderAlignmentProperty), typeof(HorizontalAlignment)));
			lines.Add(() => new EnumPropertyLineViewModel(viewModel, DependencyPropertyHelper.GetPropertyName(() => Tile.VerticalHeaderAlignmentProperty), typeof(VerticalAlignment)));
			return lines;
		}
	}
}
