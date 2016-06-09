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
using System.Text;
using System.Windows.Markup;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Platform::System.Windows;
using Platform::System.Windows.Data;
using Platform::DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Core.Design {
	#region CommandBarCreator (abstract class)
	public abstract class CommandBarCreator {
		readonly Dictionary<ModelItem, object> modelItemValues = new Dictionary<ModelItem, object>();
		ModelItem root;
		object commandsKey;
		ModelItem barManager;
		public abstract System.Type CommandsType { get; }
		public abstract System.Type StringIdConverter { get; }
		public virtual bool CanGenerateButtonGroups { get { return false; } }
		public virtual bool IsRibbonBarCreator { get { return CanGenerateButtonGroups; } }
		public virtual bool GenerateCommandParameter { get { return true; } }
		public object CommandsKey { get { return commandsKey; } }
		public ModelItem BarManager { get { return barManager; } }
		public ModelItem Root { get { return root; } }
		protected internal object StringIdConverterKey;
		public void CreateBars(ModelItem primarySelectionParent, ModelItem primarySelection, BarInfo[] barInfos) {
			this.root = primarySelection.Root;
			try {
				modelItemValues.Clear();
				using (ModelEditingScope scope = primarySelection.Root.BeginEdit()) {
					try {
						ModelItem masterControl = primarySelection;
						ModelItem masterControlParent = primarySelectionParent; 
						CreatorHelper.EnsureControlHasName(masterControl, root);
						this.barManager = PrepareBarManager(masterControlParent);
						if (this.barManager == null)
							return;
						this.commandsKey = PrepareCommands();
						if (commandsKey == null)
							return;
						StringIdConverterKey = PrepareStringIdConverter();
						BindBarManagerToMasterControl(barManager, masterControl);
						AppendBarItems(barManager.Properties["Items"].Collection, masterControl, barInfos);
					}
					finally {
						scope.Complete();
					}
				}
				modelItemValues.Clear();
			}
			finally {
				this.root = null;
			}
		}
		public void CreateBars(ModelItem primarySelection, BarInfo[] barInfos) {
			CreateBars(primarySelection, primarySelection, barInfos);
		}
		protected internal virtual ModelItem PrepareBarManager(ModelItem masterControl) {
			ModelItem barManager = FindBarManager(masterControl);
			if (barManager == null) {
				barManager = CreateBarManager(masterControl.Context, CreateOptions.None);
				barManager.Properties["ToolbarGlyphSize"].SetValue(GlyphSize.Small);
				CreatorHelper.EnsureControlHasName(barManager, root);
				WrapMasterControlWithBarManager(masterControl, barManager);
			}
			else
				CreatorHelper.EnsureControlHasName(barManager, root);
			return barManager;
		}
		protected internal virtual void WrapMasterControlWithBarManager(ModelItem masterControl, ModelItem barManager) {
#if !SL
			CreatorHelper.ReplaceMasterControl(masterControl, barManager);
			barManager.Properties["Child"].SetValue(masterControl);
#else
			ModelItem oldParent = masterControl.Parent;
			ModelParent.Parent(barManager.Context, barManager, masterControl);
			ModelParent.Parent(oldParent.Context, oldParent, barManager);
#endif
		}
		protected internal virtual void BindBarManagerToMasterControl(ModelItem barManager, ModelItem masterControl) {
			CreatorHelper.BindItemToControl(barManager, masterControl, "BarManager");
		}
		protected internal virtual ModelItem FindBarManager(ModelItem from) {
			return CreatorHelper.FindItem<BarManager>(from);
		}
		protected internal virtual object PrepareCommands() {
			return EnsureResourceDictionaryEntryExists(barManager, CommandsType, "commands");
		}
		protected internal virtual object PrepareStringIdConverter() {
			return EnsureResourceDictionaryEntryExists(barManager, StringIdConverter, "stringIdConverter");
		}
		public virtual object EnsureResourceDictionaryEntryExists(ModelItem barManager, System.Type entryType, string entryResourceKey) {
			ModelProperty resources = ObtainRootResources(barManager);
			if (resources == null)
				return null;
			ModelItemDictionary dictionary = resources.Dictionary;
			foreach (object k in dictionary.Keys) {
				string key = string.Empty;
				ModelItem keyItem = k as ModelItem;
				if (key != null)
					key = keyItem.GetCurrentValue() as string;
				else
					key = k as string;
				if (!string.IsNullOrEmpty(key)) {
					ModelItem item = dictionary[key] as ModelItem;
					if (item != null && entryType.IsAssignableFrom(item.ItemType))
						return key;
				}
			}
			ModelItem commands = ModelFactory.CreateItem(barManager.Context, entryType);
			string dictionaryKey = entryResourceKey;
			try {
				dictionary.Add(dictionaryKey, commands);
			}
			catch {
			}
			return dictionaryKey;
		}
		protected internal virtual ModelProperty ObtainRootResources(ModelItem barManager) {
			ModelItem root = barManager.Root;
			if (root == null)
				return null;
			if (!typeof(FrameworkElement).IsAssignableFrom(root.ItemType))
				return null;
			ModelProperty resources = root.Properties["Resources"];
			if (!resources.IsSet)
				resources.SetValue(ModelFactory.CreateItem(root.Context, typeof(ResourceDictionary)));
			return resources;
		}
		protected internal virtual ModelItem CreateBarManager(EditingContext context, CreateOptions options) {
			return ModelFactory.CreateItem(context, typeof(BarManager), options);
		}
		public static string ValidateName(string prefix, string name) {
			name = name.Replace(" ", "");
			name = name.Replace("&", "");
			name = name.Replace(",", "");
			name = name.Replace(".", "");
			name = name.Replace("\"", "");
			name = name.Replace("<", "");
			name = name.Replace(">", "");
			return prefix + name;
		}
		protected internal string CreateValidName(string prefix, string name) {
			return ValidateName(prefix, name);
		}
		protected internal virtual void AppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfo[] barInfos) {
			int count = barInfos.Length;
			for (int i = 0; i < count; i++)
				AppendBarItems(target, masterControl, barInfos[i]);
		}
		protected internal virtual void AppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfo barInfo) {
			BarInfoItems barInfoItems = GetBarInfoItems(barInfo);
			List<ModelItem> items = CreateBarItems(target, masterControl, barInfoItems);
			List<ModelItem> itemLinks = CreateItemLinks(items, barInfoItems);
			ModelItem bar = CreateBar(barInfo, masterControl);
			AppendItemLinks(bar, itemLinks);
		}
		public virtual List<ModelItem> CreateBarItems(ModelItemCollection target, ModelItem masterControl, BarInfoItems barInfoItems) {
			List<ModelItem> items = new List<ModelItem>();
			CreateAndAppendBarItems(target, masterControl, barInfoItems, items);
			return items;
		}
		protected internal virtual void CreateAndAppendBarItems(ModelItemCollection target, ModelItem masterControl, BarInfoItems barInfoItems, List<ModelItem> items) {
			int count = barInfoItems.Commands.Length;
			for (int i = 0; i < count; i++)
				CreateAndAppendBarItem(target, masterControl, barInfoItems.Infos[i], barInfoItems.Commands[i], items);
		}
		protected internal virtual void CreateAndAppendBarItem(ModelItemCollection target, ModelItem masterControl, BarItemInfo barItemInfo, string command, List<ModelItem> items) {
			BarButtonGroupItemInfo buttonGroupItem = barItemInfo as BarButtonGroupItemInfo;
			if (buttonGroupItem != null && !CanGenerateButtonGroups) {
				items.AddRange(CreateBarItems(target, masterControl, buttonGroupItem.Items));
				return;
			}
			ModelItem barItem = AppendBarItem(target, command, masterControl, barItemInfo);
			if (barItem != null) {
				items.Add(barItem);
				barItemInfo.CreateChildItems(this, barItem, masterControl);
			}
		}
		protected internal virtual List<ModelItem> CreateItemLinks(List<ModelItem> items, BarInfoItems barInfoItems) {
			List<ModelItem> itemLinks = new List<ModelItem>();
			int count = items.Count;
			for (int i = 0; i < count; i++)
				AppendBarItemLink(itemLinks, CreateBarItemLink(items[i]), i, barInfoItems);
			return itemLinks;
		}
		protected internal virtual void AppendBarItemLink(List<ModelItem> itemLinks, ModelItem link, int index, BarInfoItems barInfoItems) {
			itemLinks.Add(link);
		}
		protected internal virtual ModelItem CreateBarItemLink(ModelItem barItem) {
			BarItem item;
			try {
				item = barItem.GetCurrentValue() as BarItem;
			}
			catch {
				item = barItem.GetCurrentValue() as BarItem;
			}
			if (item == null) {
				object value;
				if (modelItemValues.TryGetValue(barItem, out value))
					item = value as BarItem;
			}
			ModelItem link = ModelFactory.CreateItem(barItem.Context, item.CreateLink().GetType());
			link.Properties["BarItemName"].SetValue(barItem.Name);
			return link;
		}
		protected internal virtual ModelItem CreateBar(BarInfo barInfo, ModelItem masterControl) {
			return CreateBar(CreateValidName("bar", barInfo.GroupName), barInfo.GroupCaptionStringId);
		}
		protected internal virtual ModelItem CreateBar(string name, string captionStringId) {
			ModelItem bar = CreateBarCore(name, captionStringId);
			if (!bar.Properties["DockInfo"].IsSet) {
				ModelItem barDockInfo = ModelFactory.CreateItem(barManager.Context, typeof(BarDockInfo));
				barDockInfo.Properties["ContainerType"].SetValue(BarContainerType.Top);
				bar.Properties["DockInfo"].SetValue(barDockInfo);
			}
			return bar;
		}
		protected internal virtual ModelItem FindOrCreateObjectByType(ModelItemCollection items, System.Type objectType) {
			return FindOrCreateObjectByType(items, objectType, int.MaxValue);
		}
		protected internal virtual ModelItem FindOrCreateObjectByType(ModelItemCollection items, System.Type objectType, int insertionIndex) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ModelItem item = items[i];
				if (objectType.IsAssignableFrom(item.ItemType))
					return item;
			}
			ModelItem result = ModelFactory.CreateItem(items.Context, objectType);
			if (insertionIndex >= items.Count)
				items.Add(result);
			else
				items.Insert(insertionIndex, result);
			return result;
		}
		protected internal virtual ModelItem FindOrCreateObjectByNameAndCaption(ModelItem barManager, ModelItemCollection items, string name, string captionStringId, System.Type objectType, string captionPropertyName) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ModelItem item = items[i];
				if (item.Name == name)
					return item;
			}
			ModelItem bar = CreateObjectByNameAndCaption(barManager, name, captionStringId, objectType, captionPropertyName);
			items.Add(bar);
			return bar;
		}
		public virtual ModelItem CreateObjectByNameAndCaption(ModelItem barManager, string name, string captionStringId, System.Type objectType, string captionPropertyName) {
			ModelItem bar = ModelFactory.CreateItem(barManager.Context, objectType);
			bar.Name = name;
			object captionBinding = CreateBindingToCaptionPropertyOfResourceEntryInstance(barManager.Context, StringIdConverterKey, captionStringId);
			bar.Properties[captionPropertyName].SetValue(captionBinding);
			return bar;
		}
		public virtual object CreateBindingToCaptionPropertyOfResourceEntryInstance(EditingContext context, object instanceDictionaryKey, string converterParameter) {
			ModelItem sourceStaticResource = CreateStaticResource(context, instanceDictionaryKey);
			ModelItem converterStaticResource = CreateStaticResource(context, instanceDictionaryKey);
			ModelItem result = ModelFactory.CreateItem(context, typeof(Binding));
			result.Properties["Source"].SetValue(sourceStaticResource);
			result.Properties["ConverterParameter"].SetValue(converterParameter);
			result.Properties["Converter"].SetValue(converterStaticResource);
			result.Properties["Mode"].SetValue(BindingMode.OneTime);
			return result;
		}
		protected internal virtual ModelItem CreateBarCore(string name, string captionStringId) {
			ModelItemCollection items = barManager.Properties["Bars"].Collection;
			return FindOrCreateObjectByNameAndCaption(barManager, items, name, captionStringId, typeof(Bar), "Caption");
		}
		protected internal virtual void AppendItemLinks(ModelItem bar, List<ModelItem> barItemsLinks) {
			ModelItemCollection links = bar.Properties["ItemLinks"].Collection;
			int count = barItemsLinks.Count;
			for (int i = 0; i < count; i++)
				links.Add(barItemsLinks[i]);
		}
		protected internal virtual ModelItem AppendBarItem(ModelItemCollection target, string commandName, ModelItem masterControl, BarItemInfo info) {
			string barName = CreateValidName(String.Empty, info.GenerateBarItemName(commandName));
			int count = target.Count;
			for (int i = 0; i < count; i++) {
				ModelItem item = target[i];
				if (item.Name == barName) {
					if (IsModelItemMatchBarInfo(item, info, commandName)) {
						return item;
					}
					return null;
				}
			}
			ModelItem barItem = info.CreateItem(barManager.Context);
			modelItemValues.Add(barItem, barItem.GetCurrentValue());
			barItem.Name = barName;
			info.SetupItem(this, barManager, barItem, masterControl);
			if (!string.IsNullOrEmpty(commandName) && !(info is BarButtonGroupItemInfo) && info.SupportsCommandBinding) {
				ModelItem commandBinding = CreateBindingToPropertyOfResourceEntryInstance(barManager.Context, commandsKey, commandName);
				barItem.Properties["Command"].SetValue(commandBinding);
				if (GenerateCommandParameter) {
					ModelItem commandParameterBinding = ModelFactory.CreateItem(barManager.Context, typeof(Binding));
					commandParameterBinding.Properties["ElementName"].SetValue(masterControl.Name);
					barItem.Properties["CommandParameter"].SetValue(commandParameterBinding);
				}
			}
			target.Add(barItem);
			return barItem;
		}
		public bool IsModelItemMatchBarInfo(ModelItem item, BarItemInfo info, string commandName) {
			try {
				if (item.ItemType == null)
					return false;
				if (item.ItemType.Name != info.XamlItemTag)
					return false;
				ModelProperty commandProperty = item.Properties["Command"];
				if (commandProperty == null || !commandProperty.IsSet)
					return false;
				ModelItem bindingItem = commandProperty.Value;
				if (bindingItem == null)
					return false;
				if (bindingItem.ItemType == null || bindingItem.ItemType.Name != "Binding")
					return false;
				ModelProperty pathProperty = bindingItem.Properties["Path"];
				if (pathProperty == null || !pathProperty.IsSet)
					return false;
				return true;
			}
			catch {
				return false;
			}
		}
		public virtual ModelItem CreateBindingToPropertyOfResourceEntryInstance(EditingContext context, object instanceDictionaryKey, string propertyName) {
			ModelItem staticResource = CreateStaticResource(context, instanceDictionaryKey);
			ModelItem result = ModelFactory.CreateItem(context, typeof(Binding));
			result.Properties["Path"].SetValue(propertyName);
			result.Properties["Mode"].SetValue(BindingMode.OneTime);
			result.Properties["Source"].SetValue(staticResource);
			return result;
		}
		protected internal virtual ModelItem CreateStaticResource(EditingContext context, object instanceDictionaryKey) {
#if SILVERLIGHT
			ModelItem staticResource = ModelFactory.CreateItem(context, new TypeIdentifier("MS.Internal.Metadata.ExposedTypes.Presentation.StaticResourceExtension")); 
#else
			ModelItem staticResource = ModelFactory.CreateItem(context, typeof(Platform::System.Windows.StaticResourceExtension)); 
#endif
			if (staticResource == null)
				staticResource = ModelFactory.CreateItem(context, typeof(System.Windows.StaticResourceExtension)); 
			staticResource.Properties["ResourceKey"].SetValue(instanceDictionaryKey);
			return staticResource;
		}
		protected internal virtual BarInfoItems GetBarInfoItems(BarInfo barInfo) {
			return barInfo.Items;
		}
	}
	#endregion
	#region CommandBarXamlCreator (abstract class)
	public abstract class CommandBarXamlCreator {
		class TagInfo {
			string tagName;
			int childCount;
			public string TagName { get { return tagName; } set { tagName = value; } }
			public int ChildCount { get { return childCount; } set { childCount = value; } }
		}
		readonly Dictionary<string, bool> registeredBarItems = new Dictionary<string, bool>();
		readonly Stack<TagInfo> tagStack = new Stack<TagInfo>();
		string barManagerName = "barManager1";
		string masterControlName = String.Empty;
		public string BarManagerName { get { return barManagerName; } set { barManagerName = value; } }
		public string MasterControlName { get { return masterControlName; } set { masterControlName = value; } }
		public virtual bool CanGenerateButtonGroups { get { return false; } }
		public virtual bool IsRibbonBarCreator { get { return CanGenerateButtonGroups; } }
		public void CreateBarsXaml(BarInfo[] barInfos, StringBuilder writer) {
			registeredBarItems.Clear();
			WriteStartElement(writer, "BarManager", "dxb");
			try {
				GenerateBarManagerContent(barInfos, writer);
			}
			finally {
				WriteEndElement(writer);
			}
		}
		void GenerateBarManagerContent(BarInfo[] barInfos, StringBuilder writer) {
			WriteAttributeString(writer, "Name", barManagerName);
			WriteAttributeString(writer, "ToolbarGlyphSize", "Small");
			GenerateBarItems(barInfos, writer);
			GenerateBarItemLinks(barInfos, writer);
			GenerateMasterControl(writer);
		}
		protected abstract void GenerateMasterControl(StringBuilder writer);
		protected void GenerateBarItems(BarInfo[] barInfos, StringBuilder writer) {
			WriteStartPropertyElement(writer, "BarManager.Items", "dxb");
			try {
				GenerateBarItemsCore(barInfos, writer);
			}
			finally {
				WriteEndPropertyElement(writer);
			}
		}
		protected internal virtual void GenerateBarItemLinks(BarInfo[] barInfos, StringBuilder writer) {
			WriteStartPropertyElement(writer, "BarManager.Bars", "dxb");
			try {
				GenerateBarItemLinksCore(barInfos, writer);
			}
			finally {
				WriteEndPropertyElement(writer);
			}
		}
		protected void GenerateBarItemLinksCore(BarInfo[] barInfos, StringBuilder writer) {
			int count = barInfos.Length;
			for (int i = 0; i < count; i++)
				GenerateBar(barInfos[i], writer);
		}
		protected void GenerateBarItemsCore(BarInfo[] barInfos, StringBuilder writer) {
			int count = barInfos.Length;
			for (int i = 0; i < count; i++)
				GenerateBarItems(barInfos[i], writer);
		}
		protected virtual void GenerateBarItems(BarInfo barInfo, StringBuilder writer) {
			BarInfoItems items = GetBarInfoItems(barInfo);
			GenerateBarItems(items, writer);
		}
		void GenerateBarItems(BarInfoItems barInfoItems, StringBuilder writer) {
			BarItemInfo[] infos = barInfoItems.Infos;
			string[] commands = barInfoItems.Commands;
			int count = infos.Length;
			for (int i = 0; i < count; i++)
				GenerateBarItem(infos[i], commands[i], writer);
		}
		protected virtual void GenerateBarItem(BarItemInfo info, string command, StringBuilder writer) {
			bool registered;
			string barItemName = info.GenerateBarItemName(command);
			if (registeredBarItems.TryGetValue(barItemName, out registered))
				return;
			else
				registeredBarItems.Add(barItemName, true);
			BarButtonGroupItemInfo buttonGroupItem = info as BarButtonGroupItemInfo;
			if (buttonGroupItem != null && !CanGenerateButtonGroups) {
				GenerateBarItems(buttonGroupItem.Items, writer);
				return;
			}
			BarSubItemInfo subItemInfo = info as BarSubItemInfo;
			WriteStartElement(writer, info.XamlItemTag, info.XamlPrefix);
			try {
				if (!String.IsNullOrEmpty(command)) {
					if (!(info is BarButtonGroupItemInfo)) {
						string commandValue = String.Format("{{Binding Path={0}, Mode=OneTime, Source={{StaticResource commands}} }}", command);
						WriteAttributeString(writer, "Command", commandValue);
					}
					WriteAttributeString(writer, "Name", barItemName);
				}
				info.SetupItem(this, writer, command, MasterControlName);
				if (subItemInfo != null && subItemInfo.Items.Commands.Length > 0)
					GenerateBarSubItemLinks(subItemInfo, command, writer);
			}
			finally {
				WriteEndElement(writer);
			}
			if (subItemInfo != null)
				GenerateBarItems(subItemInfo.Items, writer);
		}
		void GenerateBarSubItemLinks(BarSubItemInfo info, string command, StringBuilder writer) {
			WriteStartPropertyElement(writer, info.XamlItemTag + ".ItemLinks", info.XamlPrefix);
			try {
				GenerateBarItemLinks(info.Items, writer);
			}
			finally {
				WriteEndPropertyElement(writer);
			}
		}
		protected internal virtual void GenerateBar(BarInfo barInfo, StringBuilder writer) {
			WriteStartElement(writer, "Bar", "dxb");
			try {
				WriteStartPropertyElement(writer, "Bar.DockInfo", "dxb");
				try {
					WriteStartElement(writer, "BarDockInfo", "dxb");
					try {
						WriteAttributeString(writer, "ContainerType", "Top");
					}
					finally {
						WriteEndElement(writer);
					}
				}
				finally {
					WriteEndPropertyElement(writer);
				}
				WriteStartPropertyElement(writer, "Bar.ItemLinks", "dxb");
				try {
					GenerateBarItemLinks(barInfo.Items, writer);
				}
				finally {
					WriteEndPropertyElement(writer);
				}
			}
			finally {
				WriteEndElement(writer);
			}
		}
		protected void GenerateBarItemLinks(BarInfoItems barInfoItems, StringBuilder writer) {
			BarItemInfo[] infos = barInfoItems.Infos;
			string[] commands = barInfoItems.Commands;
			int count = infos.Length;
			for (int i = 0; i < count; i++)
				GenerateBarItemLink(infos[i], commands[i], writer, barInfoItems, i);
		}
		protected virtual void GenerateBarItemLink(BarItemInfo info, string command, StringBuilder writer, BarInfoItems barInfoItems, int index) {
			BarButtonGroupItemInfo buttonGroupItem = info as BarButtonGroupItemInfo;
			if (buttonGroupItem != null && !CanGenerateButtonGroups) {
				GenerateBarItemLinks(buttonGroupItem.Items, writer);
				return;
			}
			WriteStartElement(writer, info.XamlItemLinkTag, info.XamlLinkPrefix);
			try {
				WriteAttributeString(writer, "BarItemName", info.GenerateBarItemName(command));
				GenerateBarItemLinkCore(info, command, writer, barInfoItems, index);
			}
			finally {
				WriteEndElement(writer);
			}
		}
		protected virtual void GenerateBarItemLinkCore(BarItemInfo info, string command, StringBuilder writer, BarInfoItems barInfoItems, int index) {
		}
		protected internal virtual BarInfoItems GetBarInfoItems(BarInfo barInfo) {
			return barInfo.Items;
		}
		#region XmlWriter-like methods
		public void WriteStartElement(StringBuilder writer, string localName) {
			WriteStartElement(writer, localName, String.Empty);
		}
		public void WriteStartElement(StringBuilder writer, string localName, string prefix) {
			string text;
			if (String.IsNullOrEmpty(prefix))
				text = localName;
			else
				text = prefix + ":" + localName;
			if (tagStack.Count > 0) {
				TagInfo parentTagInfo = tagStack.Peek();
				if (parentTagInfo.ChildCount == 0)
					writer.Append('>');
				parentTagInfo.ChildCount++;
				writer.Append("\r\n");
			}
			TagInfo tagInfo = new TagInfo();
			tagInfo.TagName = text;
			tagStack.Push(tagInfo);
			writer.Append('<');
			writer.Append(text);
		}
		public void WriteStartPropertyElement(StringBuilder writer, string localName) {
			WriteStartElement(writer, localName);
		}
		public void WriteStartPropertyElement(StringBuilder writer, string localName, string prefix) {
			WriteStartElement(writer, localName, prefix);
		}
		public void WriteEndElement(StringBuilder writer) {
			WriteEndElementCore(writer, true);
		}
		public void WriteEndPropertyElement(StringBuilder writer) {
			WriteEndElementCore(writer, false);
		}
		void WriteEndElementCore(StringBuilder writer, bool allowEmptyTag) {
			TagInfo tagInfo = tagStack.Pop();
			if (tagInfo.ChildCount == 0) {
				if (allowEmptyTag) {
				writer.Append('/');
				writer.Append('>');
				}
				else {
					writer.Append('>');
					writer.Append('<');
				writer.Append('/');
					writer.Append(tagInfo.TagName);
				writer.Append('>');
				}
			}
			else {
				writer.Append('<');
				writer.Append('/');
				writer.Append(tagInfo.TagName);
				writer.Append('>');
			}
		}
		public void WriteAttributeString(StringBuilder writer, string localName, string value) {
			WriteAttributeString(writer, String.Empty, localName, value);
		}
		public void WriteAttributeString(StringBuilder writer, string prefix, string localName, string value) {
			writer.Append(' ');
			if (!String.IsNullOrEmpty(prefix)) {
				writer.Append(prefix);
				writer.Append(':');
			}
			writer.Append(localName);
			writer.Append('=');
			writer.Append('"');
			writer.Append(value);
			writer.Append('"');
		}
		#endregion
	}
	#endregion
	public class CreatorHelper {
		public static void EnsureControlHasName(ModelItem item, ModelItem root) {
			if (string.IsNullOrEmpty(item.Name)) {
				ModelItem fakeControl = ModelFactory.CreateItem(item.Context, item.ItemType, CreateOptions.InitializeDefaults);
				item.Name = fakeControl.Name;
			}
			if (string.IsNullOrEmpty(item.Name)) {
				try {
					System.Reflection.MethodInfo method = typeof(ModelFactory).GetMethod("AssignUniqueName");
					if (method != null)
						method.Invoke(null, new object[] { item.Context, root != null ? root : item, item });
				}
				catch {
				}
			}
			if (string.IsNullOrEmpty(item.Name)) {
				item.Name = item.ItemType.Name + "1";
			}
		}
		public static void ReplaceMasterControlViaProperty(ModelItem masterControl, string propertyName, ModelItem item) {
			ModelItem oldParent = masterControl.Parent;
			if (oldParent == null)
				return;
			ModelProperty property = oldParent.Properties.Find(propertyName);
			if (property != null && property.IsSet) {
				if (property.IsCollection) {
					ModelItemCollection children = property.Collection;
					if (children != null) {
						int index = children.IndexOf(masterControl);
						if (index >= 0)
							children.RemoveAt(index);
						children.Add(item);
					}
				}
				else if (!property.IsReadOnly)
					property.SetValue(item);
			}
		}
		public static void ReplaceMasterControl(ModelItem masterControl, ModelItem item) {
			ModelItem oldParent = masterControl.Parent;
			object parentInstance = oldParent.GetCurrentValue();
			if (parentInstance != null) {
				object[] attributes = parentInstance.GetType().GetCustomAttributes(typeof(ContentPropertyAttribute), true);
				if (attributes.Length > 0) {
					ContentPropertyAttribute attribute = attributes[0] as ContentPropertyAttribute;
					if (attribute != null) {
						ReplaceMasterControlViaProperty(masterControl, attribute.Name, item);
					}
				}
			}
		}
		public static ModelItem FindItem<ItemType>(ModelItem from) {
			ModelItem item = from;
			while (item != null) {
				if (item.ItemType == typeof(ItemType))
					break;
				item = item.Parent;
			}
			return item;
		}
		public static void BindItemToControl(ModelItem item, ModelItem control, string nameProperty) {
			ModelProperty itemProperty = control.Properties[nameProperty];
			if (!itemProperty.IsSet) {
				ModelItem bindingModelItem = ModelFactory.CreateItem(item.Context, typeof(Binding));
				bindingModelItem.Properties["ElementName"].SetValue(item.Name);
				bindingModelItem.Properties["Mode"].SetValue(BindingMode.OneTime);
				itemProperty.SetValue(bindingModelItem);
			}
		}
		public static void BindItemToControlSL(ModelItem item, ModelItem control, string nameProperty) {
			ModelProperty dockManagerProperty = control.Properties[nameProperty];
			if (!dockManagerProperty.IsSet) {
				ModelItem bindingModelItem = ModelFactory.CreateItem(item.Context, typeof(Binding));
				bindingModelItem.Properties["Mode"].SetValue(BindingMode.OneTime);
				RelativeSource source = new RelativeSource(RelativeSourceMode.FindAncestor);
				source.AncestorType = item.ItemType;
				source.AncestorLevel = 1;
				bindingModelItem.Properties["RelativeSource"].SetValue(source);
				dockManagerProperty.SetValue(bindingModelItem);
			}
		}
	}
}
