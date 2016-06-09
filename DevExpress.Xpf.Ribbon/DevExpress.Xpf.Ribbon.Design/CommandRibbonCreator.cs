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

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Windows.Media.Media3D;
using System.Windows;
using Microsoft.Windows.Design.Model;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Design;
using DevExpress.Xpf.Ribbon;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Ribbon.Design {
	#region CommandRibbonCreator (abstract class)
	public abstract class CommandRibbonCreator : CommandBarCreator {
		BarInfo currentBarInfo;
		public override bool CanGenerateButtonGroups { get { return true; } }
		protected override ModelItem CreateBar(BarInfo barInfo, ModelItem masterControl) {
			ModelItem pageGroup = CreateRibbonPageGroup(barInfo);
			if (pageGroup != null) {
				if (!string.IsNullOrEmpty(barInfo.RibbonPageGroupButtonCommand)) {
					ModelItem commandBinding = CreateBindingToPropertyOfResourceEntryInstance(BarManager.Context, CommandsKey, barInfo.RibbonPageGroupButtonCommand);
					pageGroup.Properties["CaptionButtonCommand"].SetValue(commandBinding);
					if (GenerateCommandParameter) {
						ModelItem commandParameterBinding = ModelFactory.CreateItem(BarManager.Context, typeof(Binding));
						commandParameterBinding.Properties["ElementName"].SetValue(masterControl.Name);
						pageGroup.Properties["CaptionButtonCommandParameter"].SetValue(commandParameterBinding);
					}
				}
				else
					pageGroup.Properties["ShowCaptionButton"].SetValue(false);
			}
			return pageGroup;
		}
		protected internal virtual ModelItem CreateRibbonPageGroup(BarInfo barInfo) {
			ModelItem ribbonPage = CreateRibbonPage(barInfo);
			ModelItemCollection groups = ribbonPage.Properties["Groups"].Collection;
			return FindOrCreateObjectByNameAndCaption(BarManager, groups, CreateValidName("grp", barInfo.RibbonCategoryName + barInfo.PageName + barInfo.GroupName), barInfo.GroupCaptionStringId, typeof(RibbonPageGroup), "Caption");
		}
		protected internal virtual ModelItem CreateRibbonPage(BarInfo barInfo) {
			ModelItem ribbonPageCategory = CreateRibbonPageCategory(barInfo);
			ModelItemCollection pages = ribbonPageCategory.Properties["Pages"].Collection;
			return FindOrCreateObjectByNameAndCaption(BarManager, pages, CreateValidName("page", barInfo.RibbonCategoryName + barInfo.PageName), barInfo.PageCaptionStringId, typeof(RibbonPage), "Caption");
		}
		protected internal virtual ModelItem CreateRibbonPageCategory(BarInfo barInfo) {
			ModelItem ribbonControl = CreateRibbonControl();
			ModelItemCollection items = ribbonControl.Properties["Categories"].Collection;
			if (!string.IsNullOrEmpty(barInfo.RibbonCategoryName)) {
				ModelItem category = FindOrCreateObjectByNameAndCaption(BarManager, items, CreateValidName("cat", barInfo.RibbonCategoryName), barInfo.RibbonCategoryStringId, typeof(RibbonPageCategory), "Caption");
				if (category == null)
					return category;
				if (!string.IsNullOrEmpty(barInfo.CategoryCommand)) {
					PropertyIdentifier propertyIdentifier = new PropertyIdentifier(typeof(DevExpress.Xpf.Office.UI.AttachedCommand), "Command");
					if (!ribbonControl.Properties[propertyIdentifier].IsSet) {
						ModelItem commandBinding = CreateBindingToPropertyOfResourceEntryInstance(BarManager.Context, CommandsKey, barInfo.CategoryCommand);
						category.Properties[propertyIdentifier].SetValue(commandBinding);
					}
				}
				return category;
			}
			else
				return FindOrCreateObjectByType(items, typeof(RibbonDefaultPageCategory));
		}
		protected internal virtual ModelItem CreateRibbonControl() {
			ModelItem dockPanel = CreateDockPanel(BarManager);
			ModelItemCollection children = dockPanel.Properties["Children"].Collection;
			ModelItem ribbonControl = FindOrCreateObjectByType(children, typeof(RibbonControl), 0);
			PropertyIdentifier propertyIdentifier = new PropertyIdentifier(typeof(System.Windows.Controls.DockPanel), "Dock");
			if (!ribbonControl.Properties[propertyIdentifier].IsSet) {
				ribbonControl.Properties[propertyIdentifier].SetValue(System.Windows.Controls.Dock.Top);
			}
			return ribbonControl;
		}
		protected internal virtual ModelItem CreateDockPanel(ModelItem barManager) {
			ModelProperty childProperty = barManager.Properties["Child"];
			if (childProperty.IsSet) {
				ModelItem child = childProperty.Value;
				if (typeof(DockPanel).IsAssignableFrom(child.ItemType))
					return child;
				ModelItem dockPanel = ModelFactory.CreateItem(barManager.Context, typeof(DockPanel));
				barManager.Properties["Child"].SetValue(dockPanel);
				ModelParent.Parent(barManager.Context, dockPanel, child);
				return dockPanel;
			}
			else {
				ModelItem dockPanel = ModelFactory.CreateItem(barManager.Context, typeof(DockPanel));
				ModelParent.Parent(barManager.Context, barManager, dockPanel);
				return dockPanel;
			}
		}
		protected override void CreateAndAppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfoItems barInfoItems, List<ModelItem> items) {
			if (barInfoItems.RibbonCustomButtonOrder == null)
				base.CreateAndAppendBarItems(target, masterControl, barInfoItems, items);
			else {
				int[] indices = barInfoItems.RibbonCustomButtonOrder;
				int count = indices.Length;
				for (int i = 0; i < count; i++) {
					int index = indices[i];
					CreateAndAppendBarItem(target, masterControl, barInfoItems.Infos[index], barInfoItems.Commands[index], items);
				}
			}
		}
		protected override void AppendBarItemLink(List<ModelItem> itemLinks, ModelItem link, int index, BarInfoItems barInfoItems) {
			if (barInfoItems.RibbonCustomButtonOrder != null && ShouldSetSmallWithoutTextRibbonStyle(barInfoItems.RibbonItemIndicesWithDefaultRibbonStyle, index))
				link.Properties["RibbonStyle"].SetValue(RibbonItemStyles.SmallWithoutText);
			if (barInfoItems.RibbonItemIndicesWithHiddenSeparatorBefore != null && barInfoItems.RibbonItemIndicesWithHiddenSeparatorBefore.Contains(index)) {
				ModelItem separatorLink = ModelFactory.CreateItem(link.Context, typeof(BarItemLinkSeparator));
				separatorLink.Properties["IsVisible"].SetValue(false);
				itemLinks.Add(separatorLink);
			}
			base.AppendBarItemLink(itemLinks, link, index, barInfoItems);
		}
		protected internal virtual bool ShouldSetSmallWithoutTextRibbonStyle(IList<int> ribbonItemIndicesWithDefaultRibbonStyle, int index) {
			return ribbonItemIndicesWithDefaultRibbonStyle == null || !ribbonItemIndicesWithDefaultRibbonStyle.Contains(index);
		}
		protected override void AppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfo barInfo) {
			this.currentBarInfo = barInfo;
			base.AppendBarItems(target, masterControl, barInfo);
			this.currentBarInfo = null;
		}
		protected override ModelItem AppendBarItem(ModelItemCollection target, string commandName, ModelItem masterControl, BarItemInfo info) {
			if (currentBarInfo != null && !String.IsNullOrEmpty(commandName) && commandName == currentBarInfo.RibbonPageGroupButtonCommand)
				return null;
			return base.AppendBarItem(target, commandName, masterControl, info);
		}
	}
	#endregion
	#region CommandRibbonXamlCreator
	public abstract class CommandRibbonXamlCreator : CommandBarXamlCreator {
		string ribbonControlName = "ribbonControl1";
		BarInfo currentBarInfo;
		public string RibbonControlName { get { return ribbonControlName; } set { ribbonControlName = value; } }
		public override bool CanGenerateButtonGroups { get { return true; } }
		protected override void GenerateBarItemLinks(BarInfo[] barInfos, StringBuilder writer) {
			WriteStartElement(writer, "DXDockPanel", "dx");
			try {
				GenerateRibbonControl(barInfos, writer);
			}
			finally {
				WriteEndElement(writer);
			}
		}
		protected override void GenerateBarItems(BarInfo barInfo, StringBuilder writer) {
			this.currentBarInfo = barInfo;
			base.GenerateBarItems(barInfo, writer);
			this.currentBarInfo = null;
		}
		protected override void GenerateBarItem(BarItemInfo info, string command, StringBuilder writer) {
			if (currentBarInfo != null && !String.IsNullOrEmpty(command) && command == currentBarInfo.RibbonPageGroupButtonCommand)
				return;
			base.GenerateBarItem(info, command, writer);
		}
		protected override void GenerateBarItemLink(BarItemInfo info, string command, StringBuilder writer, BarInfoItems barInfoItems, int index) {
			if (currentBarInfo != null && !String.IsNullOrEmpty(command) && command == currentBarInfo.RibbonPageGroupButtonCommand)
				return;
			if (barInfoItems.RibbonItemIndicesWithHiddenSeparatorBefore != null && barInfoItems.RibbonItemIndicesWithHiddenSeparatorBefore.Contains(index)) {
				WriteStartElement(writer, "BarItemLinkSeparator", "dxb");
				try {
					WriteAttributeString(writer, "IsVisible", "False");
				}
				finally {
					WriteEndElement(writer);
				}
			}
			base.GenerateBarItemLink(info, command, writer, barInfoItems, index);
		}
		protected override void GenerateBarItemLinkCore(BarItemInfo info, string command, StringBuilder writer, BarInfoItems barInfoItems, int index) {
			if (barInfoItems.RibbonCustomButtonOrder != null && ShouldSetSmallWithoutTextRibbonStyle(barInfoItems.RibbonItemIndicesWithDefaultRibbonStyle, index))
				WriteAttributeString(writer, "RibbonStyle", "SmallWithoutText");
		}
		protected internal virtual bool ShouldSetSmallWithoutTextRibbonStyle(IList<int> ribbonItemIndicesWithDefaultRibbonStyle, int index) {
			return ribbonItemIndicesWithDefaultRibbonStyle == null || !ribbonItemIndicesWithDefaultRibbonStyle.Contains(index);
		}
		protected virtual void GenerateRibbonControl(BarInfo[] barInfos, StringBuilder writer) {
			WriteStartElement(writer, "RibbonControl", "dxr");
			try {
				WriteAttributeString(writer, "dx", "DXDockPanel.Dock", "Top");
				WriteAttributeString(writer, "x", "Name", RibbonControlName);
				Dictionary<string, List<BarInfo>> categories = CreateCategories(barInfos);
				GenerateCategories(categories, writer);
			}
			finally {
				WriteEndElement(writer);
			}
		}
		Dictionary<string, List<BarInfo>> CreateCategories(BarInfo[] barInfos) {
			Dictionary<string, List<BarInfo>> result = new Dictionary<string, List<BarInfo>>();
			int count = barInfos.Length;
			for (int i = 0; i < count; i++) {
				string category = barInfos[i].RibbonCategoryStringId;
				List<BarInfo> barInfoList;
				if (!result.TryGetValue(category, out barInfoList)) {
					barInfoList = new List<BarInfo>();
					result.Add(category, barInfoList);
				}
				barInfoList.Add(barInfos[i]);
			}
			return result;
		}
		Dictionary<string, List<BarInfo>> CreatePages(BarInfo[] barInfos) {
			Dictionary<string, List<BarInfo>> result = new Dictionary<string, List<BarInfo>>();
			int count = barInfos.Length;
			for (int i = 0; i < count; i++) {
				string page = barInfos[i].PageCaptionStringId;
				List<BarInfo> barInfoList;
				if (!result.TryGetValue(page, out barInfoList)) {
					barInfoList = new List<BarInfo>();
					result.Add(page, barInfoList);
				}
				barInfoList.Add(barInfos[i]);
			}
			return result;
		}
		void GenerateCategories(Dictionary<string, List<BarInfo>> categories, StringBuilder writer) {
			foreach (string categoryStringId in categories.Keys)
				GenerateCategory(categoryStringId, categories[categoryStringId].ToArray(), writer);
		}
		void GenerateCategory(string categoryStringId, BarInfo[] barInfos, StringBuilder writer) {
			if (barInfos.Length <= 0)
				return;
			if (String.IsNullOrEmpty(categoryStringId))
				WriteStartElement(writer, "RibbonDefaultPageCategory", "dxr");
			else {
				WriteStartElement(writer, "RibbonPageCategory", "dxr");
				WriteAttributeString(writer, "Caption", String.Format("{{Binding Source={{StaticResource stringIdConverter}}, ConverterParameter={0}, Converter={{StaticResource stringIdConverter}}, Mode=OneTime}}", categoryStringId));
				WriteAttributeString(writer, "Name", CommandBarCreator.ValidateName("cat", barInfos[0].RibbonCategoryName));
				if (!string.IsNullOrEmpty(barInfos[0].CategoryCommand))
					WriteAttributeString(writer, "dxo", "AttachedCommand.Command", String.Format("{{Binding {0}, Mode=OneTime, Source={{StaticResource commands}} }}", barInfos[0].CategoryCommand));
				else
					WriteAttributeString(writer, "IsVisible", "False");
			}
			try {
				Dictionary<string, List<BarInfo>> pages = CreatePages(barInfos);
				GeneratePages(pages, writer);
			}
			finally {
				WriteEndElement(writer);
			}
		}
		void GeneratePages(Dictionary<string, List<BarInfo>> pages, StringBuilder writer) {
			foreach (string pageStringId in pages.Keys)
				GeneratePage(pageStringId, pages[pageStringId].ToArray(), writer);
		}
		void GeneratePage(string pageStringId, BarInfo[] barInfos, StringBuilder writer) {
			if (barInfos.Length <= 0)
				return;
			WriteStartElement(writer, "RibbonPage", "dxr");
			try {
				WriteAttributeString(writer, "Caption", String.Format("{{Binding Source={{StaticResource stringIdConverter}}, ConverterParameter={0}, Converter={{StaticResource stringIdConverter}}, Mode=OneTime}}", pageStringId));
				WriteAttributeString(writer, "Name", CommandBarCreator.ValidateName("page", barInfos[0].RibbonCategoryName + barInfos[0].PageName));
				GenerateBarItemLinksCore(barInfos, writer);
			}
			finally {
				WriteEndElement(writer);
			}
		}
		protected override void GenerateBar(BarInfo barInfo, StringBuilder writer) {
			this.currentBarInfo = barInfo;
			WriteStartElement(writer, "RibbonPageGroup", "dxr");
			try {
				WriteAttributeString(writer, "Caption", String.Format("{{Binding Source={{StaticResource stringIdConverter}}, ConverterParameter={0}, Converter={{StaticResource stringIdConverter}}, Mode=OneTime}}", barInfo.GroupCaptionStringId));
				WriteAttributeString(writer, "Name", CommandBarCreator.ValidateName("grp", barInfo.RibbonCategoryName + barInfo.PageName + barInfo.GroupName));
				if (String.IsNullOrEmpty(barInfo.RibbonPageGroupButtonCommand))
					WriteAttributeString(writer, "ShowCaptionButton", "False");
				else
					WriteAttributeString(writer, "CaptionButtonCommand", String.Format("{{Binding Path={0}, Mode=OneTime, Source={{StaticResource commands}}}}", barInfo.RibbonPageGroupButtonCommand));
				GenerateBarItemLinks(GetBarInfoItems(barInfo), writer);
			}
			finally {
				WriteEndElement(writer);
			}
			this.currentBarInfo = null;
		}
	}
	#endregion
	public class BarDropDownGalleryItemInfo : BarItemInfo {
		readonly BarInfoItems items;
		public BarDropDownGalleryItemInfo(BarInfoItems items) : this(items, 1) {
		}
		public BarDropDownGalleryItemInfo(BarInfoItems items, int columnCount) {
			this.items = items;
			this.ColumnCount = columnCount;
		}
		public bool IsItemCaptionVisible { get; set; }
		public bool IsItemDescriptionVisible { get; set; }
		public int ColumnCount { get; set; }
		public override string XamlPrefix { get { return XmlNamespaceConstants.BarsPrefix; } }
		public override string XamlItemTag { get { return typeof(BarSplitButtonItem).Name; } }
		public override string XamlItemLinkTag { get { return typeof(BarItemLink).Name; } }
		public override ModelItem CreateItem(EditingContext context) {
			return ModelFactory.CreateItem(context, typeof(BarSplitButtonItem));
		}
		public override void SetupItem(CommandBarCreator creator, ModelItem barManager, ModelItem item, ModelItem masterControl) {
			item.Properties["ActAsDropDown"].SetValue(true);
			ModelItem popupMenu = ModelFactory.CreateItem(item.Context, typeof(GalleryDropDownPopupMenu));
			SetupPopupMenu(creator, popupMenu, masterControl);
			item.Properties["PopupControl"].SetValue(popupMenu);
		}
		void SetupPopupMenu(CommandBarCreator creator, ModelItem item, ModelItem masterControl) {
			ModelItem gallery = ModelFactory.CreateItem(item.Context, typeof(Gallery));
			SetupGallery(creator, gallery, masterControl);
			item.Properties["Gallery"].SetValue(gallery);
		}
		void SetupGallery(CommandBarCreator creator, ModelItem item, ModelItem masterControl) {
			item.Properties["ColCount"].SetValue(ColumnCount);
			if (!IsItemCaptionVisible)
				item.Properties["IsItemCaptionVisible"].SetValue(IsItemCaptionVisible);
			if (!IsItemDescriptionVisible)
				item.Properties["IsItemDescriptionVisible"].SetValue(IsItemDescriptionVisible);
			item.Properties["ItemDescriptionHorizontalAlignment"].SetValue(HorizontalAlignment.Left);
			ModelItemCollection target = item.Properties["Groups"].Collection;
			creator.CreateBarItems(target, masterControl, this.items);
		}
		public override void SetupItem(CommandBarXamlCreator creator, StringBuilder writer, string command, string masterControlName) {
		}
	}
}
