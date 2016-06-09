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
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Printing.Design.Bars;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Printing;
namespace DevExpress.Xpf.Printing.Design.LayoutCreators {
	class BarsCommandCreator : CommandBarCreator, IStatusBarCommandCreator {
		public ModelItem DocumentViewer { get; set; }
		readonly BarItemResorces resourceCollection = new BarItemResorces();
		public override bool GenerateCommandParameter {
			get { return false; }
		}
		public override Type CommandsType { get { return null; } }
		public override Type StringIdConverter { get { return null; } }
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
		protected override void BindBarManagerToMasterControl(ModelItem barManager, ModelItem masterControl) {
		}
		protected override ModelItem AppendBarItem(ModelItemCollection target, string commandName, ModelItem masterControl, DevExpress.Xpf.Core.Design.BarItemInfo info) {
			var modelItem = base.AppendBarItem(target, commandName, masterControl, info);
			SetBarItemProperties(modelItem, commandName);
			return modelItem;
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
		protected override void AppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfo barInfo) {
			DocumentViewer = masterControl;
			BarManager.Properties["ToolbarGlyphSize"].SetValue(GlyphSize.Default);
			ModelItem bar = CreateBar("barPreview", PrintingStringId.ToolBarCaption.ToString());
			bar.Properties["UseWholeRow"].SetValue(true);
			BarInfoItems barInfoItems = GetBarInfoItems(barInfo);
			if(bar.Properties["ItemLinks"].Collection.Count != 0)
				AppendSeparator(bar.Properties["ItemLinks"].Collection);
			List<ModelItem> items = CreateBarItems(target, masterControl, barInfoItems);
			List<ModelItem> itemLinks = CreateItemLinks(items, barInfoItems);
			AppendItemLinks(bar, itemLinks);
		}
		void AppendSeparator(ModelItemCollection items) {
			ModelItem separator = ModelFactory.CreateItem(DocumentViewer.Context, typeof(BarItemLinkSeparator));
			items.Add(separator);
		}
		public void CreateStatusBarItems(ModelItem modelItemInstance, StatusBarInfo statusBarInfo) {
			ModelItem bar = CreateBar("statusBar", PrintingStringId.StatusBarCaption.ToString());
			bar.Properties["UseWholeRow"].SetValue(true);
			bar.Properties["DockInfo"].SetValue(ModelFactory.CreateItem(bar.Context, new BarDockInfo() { ContainerType = BarContainerType.Bottom }));
			bar.Properties["IsStatusBar"].SetValue(true);
			var leftItems = statusBarInfo.LeftItems;
			var rightItems = statusBarInfo.RightItems;
			CreateAndAppendStatusBarItems(bar.Properties["ItemLinks"].Collection, leftItems, true);
			CreateAndAppendStatusBarItems(bar.Properties["ItemLinks"].Collection, rightItems, false);
		}
		public virtual void CreateAndAppendStatusBarItems(ModelItemCollection linksCollection, BarInfoItems items, bool isLeftItems) {
			List<ModelItem> modelItems = CreateBarItems(BarManager.Properties["Items"].Collection, DocumentViewer, items);
			List<ModelItem> itemLinks = CreateItemLinks(modelItems, items);
			itemLinks.ForEach(x => linksCollection.Add(x));
		}
	}
}
