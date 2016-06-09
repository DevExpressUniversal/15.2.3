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
using Microsoft.Windows.Design.Model;
using Platform::System.Windows.Data;
using System;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Printing.Design.Bars;
using DevExpress.Xpf.Ribbon.Design;
using Platform::DevExpress.Xpf.Ribbon;
using Microsoft.Windows.Design.Metadata;
using Platform::System.Windows.Controls;
using Platform::System.Windows;
using Platform::DevExpress.Xpf.Printing;
namespace DevExpress.Xpf.Printing.Design.LayoutCreators {
	public class RibbonCommandCreator : CommandRibbonCreator, IStatusBarCommandCreator {
		public ModelItem DocumentViewer { get; set; }
		readonly BarItemResorces resourceCollection = new BarItemResorces();
		public override bool GenerateCommandParameter {
			get { return false; }
		}
		public override System.Type CommandsType { get { return null; } } 
		public override System.Type StringIdConverter { get { return null; } } 
		public override System.Collections.Generic.List<Microsoft.Windows.Design.Model.ModelItem> CreateBarItems(Microsoft.Windows.Design.Model.ModelItemCollection target, Microsoft.Windows.Design.Model.ModelItem masterControl, DevExpress.Xpf.Core.Design.BarInfoItems barInfoItems) {
			this.DocumentViewer = masterControl;
			return base.CreateBarItems(target, masterControl, barInfoItems);
		}
		protected override ModelItem AppendBarItem(ModelItemCollection target, string commandName, ModelItem masterControl, DevExpress.Xpf.Core.Design.BarItemInfo info) {
			var modelItem = base.AppendBarItem(target, commandName, masterControl, info);
			SetBarItemProperties(modelItem, commandName);
			return modelItem;
		}
		protected override void BindBarManagerToMasterControl(ModelItem barManager, ModelItem masterControl) {
		}
		void SetBarItemProperties(ModelItem modelItem, string commandName) {
			var resources = resourceCollection[commandName];
			if(resources == null)
				return;
			if(!String.IsNullOrEmpty(resources.Command)) {
				var commandBinding = ModelFactory.CreateItem(modelItem.Context, typeof(Binding));
				commandBinding.Properties["Path"].SetValue(resources.Command);
				commandBinding.Properties["ElementName"].SetValue(DocumentViewer.Name);
				commandBinding.Properties["FallbackValue"].SetValue(ResourceManager.CreateStaticResourceBinding(DocumentViewer, ResourceKeys.DisabledCommand));
				modelItem.Properties["Command"].SetValue(commandBinding);
			} else {
				modelItem.Properties["Command"].ClearValue();
			}
			if(!string.IsNullOrEmpty(resources.CommandParameter))
				modelItem.Properties["CommandParameter"].SetValue(resources.CommandParameter);
			if(!string.IsNullOrEmpty(resources.Glyph)) {
				var glyphBinding = ModelFactory.CreateItem(BarManager.Context, typeof(PrintingResourceImageExtension));
				glyphBinding.Properties["ResourceName"].SetValue(resources.Glyph);
				modelItem.Properties["Glyph"].SetValue(glyphBinding);
			}
			if(!string.IsNullOrEmpty(resources.LargeGlyph)) {
				var glyphBinding = ModelFactory.CreateItem(BarManager.Context, typeof(PrintingResourceImageExtension));
				glyphBinding.Properties["ResourceName"].SetValue(resources.LargeGlyph);
				modelItem.Properties["LargeGlyph"].SetValue(glyphBinding);
			}
			if(!string.IsNullOrEmpty(resources.Content)) {
				PrintingStringId result;
				if(Enum.TryParse<PrintingStringId>(resources.Content, out result)) {
					var contentBinding = ModelFactory.CreateItem(BarManager.Context, typeof(PrintingStringIdExtension));
					contentBinding.Properties["StringId"].SetValue(resources.Content);
					modelItem.Properties["Content"].SetValue(contentBinding);
				} else {
					modelItem.Properties["Content"].SetValue(resources.Content);
				}
			}
			if(!string.IsNullOrEmpty(resources.Hint)) {
				PrintingStringId result;
				if(Enum.TryParse<PrintingStringId>(resources.Hint, out result)) {
					var contentBinding = ModelFactory.CreateItem(BarManager.Context, typeof(PrintingStringIdExtension));
					contentBinding.Properties["StringId"].SetValue(resources.Hint);
					modelItem.Properties["Hint"].SetValue(contentBinding);
				} else {
					modelItem.Properties["Hint"].SetValue(resources.Hint);
				}
			}
			if(!string.IsNullOrEmpty(resources.IsCheckedString)) {
				var binding = ModelFactory.CreateItem(BarManager.Context, typeof(Binding));
				binding.Properties["Path"].SetValue(resources.IsCheckedString);
				binding.Properties["Mode"].SetValue(BindingMode.OneWay);
				binding.Properties["ElementName"].SetValue(DocumentViewer.Name);
				modelItem.Properties["IsChecked"].SetValue(binding);
			}
			if(!string.IsNullOrEmpty(resources.IsVisibleString)) {
				var binding = ModelFactory.CreateItem(BarManager.Context, typeof(Binding));
				binding.Properties["Path"].SetValue(resources.IsVisibleString);
				binding.Properties["ElementName"].SetValue(DocumentViewer.Name);
				binding.Properties["FallbackValue"].SetValue(false);
				modelItem.Properties["IsVisible"].SetValue(binding);
			}
			if(!string.IsNullOrEmpty(resources.IsEnabled)) {
				var binding = ModelFactory.CreateItem(BarManager.Context, typeof(Binding));
				binding.Properties["Path"].SetValue(resources.IsEnabled);
				binding.Properties["ElementName"].SetValue(DocumentViewer.Name);
				binding.Properties["FallbackValue"].SetValue(false);
				modelItem.Properties["IsEnabled"].SetValue(binding);
			}
			if(!resources.ShowScreenTip) {
				var binding = ModelFactory.CreateItem(BarManager.Context, typeof(Binding));
				modelItem.Properties["ShowScreenTip"].SetValue(resources.ShowScreenTip);
			}
			if(!string.IsNullOrEmpty(resources.EditValue)) {
				var binding = ModelFactory.CreateItem(BarManager.Context, typeof(Binding));
				binding.Properties["Path"].SetValue(resources.EditValue);
				binding.Properties["ElementName"].SetValue(DocumentViewer.Name);
				if(resources.EditValueOneWay)
					binding.Properties["Mode"].SetValue(BindingMode.OneWay);
				modelItem.Properties["EditValue"].SetValue(binding);
			}
		}
		protected override ModelItem CreateStaticResource(Microsoft.Windows.Design.EditingContext context, object instanceDictionaryKey) {
			return null;
		}
		public virtual ModelItem CreateStatusBarControl() {
			var rootContainer = BarManager.Properties["Child"].Value;
			ModelItem statusBarControl = FindOrCreateObjectByType(rootContainer.Properties["Children"].Collection, typeof(RibbonStatusBarControl), 2);
			PropertyIdentifier propertyIdentifier = new PropertyIdentifier(typeof(Grid), "Row");
			if(!statusBarControl.Properties[propertyIdentifier].IsSet) {
				statusBarControl.Properties[propertyIdentifier].SetValue(2);
			}
			return statusBarControl;
		}
		protected override ModelItem CreateRibbonControl() {
			ModelItem grid = CreateGrid(BarManager);
			ModelItemCollection children = grid.Properties["Children"].Collection;
			ModelItem ribbonControl = FindOrCreateObjectByType(children, typeof(RibbonControl), 0);
			PropertyIdentifier propertyIdentifier = new PropertyIdentifier(typeof(Grid), "Row");
			if(!ribbonControl.Properties[propertyIdentifier].IsSet) {
				ribbonControl.Properties[propertyIdentifier].SetValue(0);
			}
			return ribbonControl;
		}
		protected virtual ModelItem CreateGrid(ModelItem barManager) {
			ModelProperty childProperty = barManager.Properties["Child"];
			if(childProperty.IsSet) {
				ModelItem child = childProperty.Value;
				if(child != DocumentViewer)
					return child;
				ModelItem grid = ModelFactory.CreateItem(barManager.Context, typeof(Grid));
				CreateGridRows(grid.Properties["RowDefinitions"].Collection);
				barManager.Properties["Child"].SetValue(grid);
				ModelParent.Parent(barManager.Context, grid, child);
				PropertyIdentifier propertyIdentifier = new PropertyIdentifier(typeof(Grid), "Row");
				if(!child.Properties[propertyIdentifier].IsSet) {
					child.Properties[propertyIdentifier].SetValue(1);
				}
				return grid;
			} else {
				ModelItem grid = ModelFactory.CreateItem(barManager.Context, typeof(Grid));
				ModelParent.Parent(barManager.Context, barManager, grid);
				return grid;
			}
		}
		void CreateGridRows(ModelItemCollection gridRows) {
			for(int i = 0; i < 3; i++) {
				ModelItem row = ModelFactory.CreateItem(BarManager.Context, typeof(RowDefinition));
				row.Properties["Height"].SetValue(i == 1 ? new GridLength(1, GridUnitType.Star) : new GridLength(1, GridUnitType.Auto));
				gridRows.Insert(i, row);
			}
		}
		public virtual void CreateStatusBarItems(ModelItem modelItemInstance, StatusBarInfo statusBarInfo) {
			DocumentViewer = modelItemInstance;
			var statusBarModelItem = CreateStatusBarControl();
			var leftItems = statusBarInfo.LeftItems;
			var rightItems = statusBarInfo.RightItems;
			CreateAndAppendStatusBarItems(statusBarModelItem.Properties["LeftItemLinks"].Collection, leftItems);
			CreateAndAppendStatusBarItems(statusBarModelItem.Properties["RightItemLinks"].Collection, rightItems);
		}
		public virtual void CreateAndAppendStatusBarItems(ModelItemCollection linksCollection, BarInfoItems items) {
			List<ModelItem> modelItems = CreateBarItems(BarManager.Properties["Items"].Collection, DocumentViewer, items);
			List<ModelItem> itemLinks = CreateItemLinks(modelItems, items);
			itemLinks.ForEach(x => linksCollection.Add(x));
		}
		protected override object PrepareStringIdConverter() {
			return null;
		}
		public override object EnsureResourceDictionaryEntryExists(ModelItem barManager, Type entryType, string entryResourceKey) {
			return "commands";
		}
		public override Microsoft.Windows.Design.Model.ModelItem CreateBindingToPropertyOfResourceEntryInstance(Microsoft.Windows.Design.EditingContext context, object instanceDictionaryKey, string value) {
			var resources = resourceCollection[value];
			if((string)instanceDictionaryKey == "commands" && resources != null) {
				if(!string.IsNullOrEmpty(resources.Command)) {
					value = resources.Command;
				}
			}
			ModelItem result = ModelFactory.CreateItem(context, typeof(Binding));
			result.Properties["Path"].SetValue(value);
			result.Properties["ElementName"].SetValue(DocumentViewer.Name);
			return result;
		}
		public override ModelItem CreateObjectByNameAndCaption(ModelItem barManager, string name, string captionStringId, System.Type objectType, string captionPropertyName) {
			ModelItem bar = ModelFactory.CreateItem(barManager.Context, objectType);
			bar.Name = name;
			object captionBinding = CreateBindingToCaptionProperty(barManager.Context, captionStringId);
			bar.Properties[captionPropertyName].SetValue(captionBinding);
			return bar;
		}
		public virtual ModelItem CreateBindingToCaptionProperty(Microsoft.Windows.Design.EditingContext context, string captionStringId) {
			var result = ModelFactory.CreateItem(context, typeof(PrintingStringIdExtension));
			result.Properties["StringId"].SetValue(Enum.Parse(typeof(PrintingStringId), captionStringId));
			return result;
		}
	}
}
