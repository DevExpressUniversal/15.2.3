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
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using System.Linq;
using DevExpress.Xpf.Docking.Base;
using System.Windows.Controls;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Design {
	class DockLayoutManagerDesignFeatureConnector : FeatureConnector<DockLayoutManagerAdornerProvider> {
		public DockLayoutManagerDesignFeatureConnector(FeatureManager manager)
			: base(manager) {
			Context.Services.Publish<DockLayoutManagerDesignService>(new DockLayoutManagerDesignService(Context));
			Context.Services.Publish<LockService>(new LockService());
		}
	}
	class LockService {
		int lockCounter;
		public void Lock() {
			if(IsLocked) return;
			lockCounter++;
		}
		public void UnLock() {
			lockCounter--;
		}
		public bool IsLocked { get { return lockCounter > 0; } }
	}
	class DockLayoutManagerDesignService {
		static string DefaultLayoutControlItemCaption { get { return DockingLocalizer.GetString(DockingStringId.DTLayoutControlItemCaption); } }
		static string DefaultLayoutPanelCaption { get { return DockingLocalizer.GetString(DockingStringId.DTLayoutPanelCaption); } }
		static string DefaultDocumentPanelCaption { get { return DockingLocalizer.GetString(DockingStringId.DTDocumentPanelCaption); } }
		public DockLayoutManagerDesignService(EditingContext context) {
			Context = context;
			PropertyStoreHelper = new PropertyStoreHelper();
		}
		ModelItem CreateItemCore(Type type) {
			var item = ModelFactory.CreateItem(Context, type, CreateOptions.InitializeDefaults, null);
			item.ResetLayout();
			return item;
		}
		public ModelItem CreateItem(LayoutTypes type) {
			switch(type) {
				case LayoutTypes.Group: {
						ModelItem group = CreateItemCore(typeof(LayoutGroup));
						return group;
					}
				case LayoutTypes.DocumentGroup: {
						ModelItem group = CreateItemCore(typeof(DocumentGroup));
						ModelItem panel = CreateItem(LayoutTypes.DocumentPanel);
						group.Properties["Items"].Collection.Add(panel);
						return group;
					}
				case LayoutTypes.DocumentPanel: {
						ModelItem panel = CreateItemCore(typeof(DocumentPanel));
						panel.Properties["Caption"].SetValue(DefaultDocumentPanelCaption);
						panel.Properties["Content"].SetValue(ModelFactory.CreateItem(Context, typeof(Grid)));
						return panel;
					}
				case LayoutTypes.LayoutPanel: {
						ModelItem panel = CreateItemCore(typeof(LayoutPanel));
						panel.Properties["Caption"].SetValue(DefaultLayoutPanelCaption);
						return panel;
					}
				case LayoutTypes.Label:
					return CreateItemCore(typeof(LabelItem));
				case LayoutTypes.EmptySpace:
					return CreateItemCore(typeof(EmptySpaceItem));
				case LayoutTypes.Separator:
					return CreateItemCore(typeof(SeparatorItem));
				case LayoutTypes.Splitter:
					return CreateItemCore(typeof(LayoutSplitter));
				case LayoutTypes.ControlItem: {
						ModelItem item = CreateItemCore(typeof(LayoutControlItem));
						ModelItem content = ModelFactory.CreateItem(Context, typeof(TextBox), null);
						item.Properties["Caption"].SetValue(DefaultLayoutControlItemCaption);
						item.Content.SetValue(content);
						return item;
					}
				case LayoutTypes.GroupBox: {
						ModelItem groupBox = CreateItem(LayoutTypes.Group);
						groupBox.Properties["GroupBorderStyle"].SetValue(GroupBorderStyle.GroupBox);
						groupBox.Properties["ShowCaption"].SetValue(true);
						groupBox.Properties["Caption"].SetValue("GroupBox");
						ModelItem controlItem = CreateItem(LayoutTypes.ControlItem);
						groupBox.ItemsProperty().Add(controlItem);
						return groupBox;
					}
				case LayoutTypes.LayoutTabbedGroup: {
						ModelItem tabGroup = CreateItem(LayoutTypes.Group);
						tabGroup.Properties["GroupBorderStyle"].SetValue(GroupBorderStyle.Tabbed);
						ModelItem tab = CreateItem(LayoutTypes.Group);
						tab.Properties["Caption"].SetValue("Tab");
						tabGroup.ItemsProperty().Add(tab);
						ModelItem controlItem = CreateItem(LayoutTypes.ControlItem);
						tab.ItemsProperty().Add(controlItem);
						return tabGroup;
					}
				case LayoutTypes.LayoutTab: {
						ModelItem tab = CreateItem(LayoutTypes.Group);
						tab.Properties["Caption"].SetValue("Tab");
						ModelItem controlItem = CreateItem(LayoutTypes.ControlItem);
						tab.ItemsProperty().Add(controlItem);
						return tab;
					}
				default:
					return null;
			}
		}
		public void AddItem(ModelItem item, ModelItem content) {
			bool isDocument = item.Is<DocumentGroup>();
			Type targetType = isDocument ? typeof(DocumentPanel) : typeof(LayoutPanel);
			ModelItem newPanel = ModelFactory.CreateItem(Context, targetType);
			newPanel.Properties["Caption"].SetValue(isDocument ? DefaultDocumentPanelCaption : DefaultLayoutPanelCaption);
			SetContent(newPanel, content);
			item.ItemsProperty().Add(newPanel);
		}
		void AddToSide(ModelItem newChild, ModelItem targetModel, DockTypeValue dockTypeValue) {
			Orientation targetOrientation = dockTypeValue.DockType.ToOrientation();
			var insertType = dockTypeValue.DockType.ToInsertType();
			ModelItemCollection targetCollection = targetModel.Properties["Items"].Collection;
			var parentOrientation = (Orientation)targetModel.Properties["Orientation"].ComputedValue;
			if(parentOrientation == targetOrientation) {
				int index = insertType == InsertType.Before ? 0 : targetModel.Properties["Items"].Collection.Count;
				targetCollection.Insert(index, newChild);
			}
			else {
				ModelItem newParent = ModelFactory.CreateItem(Context, typeof(LayoutGroup), null);
				ModelItem[] items = targetCollection.ToArray();
				targetCollection.Clear();
				newParent.ItemsProperty().AddRange(items);
				targetModel.Properties["Orientation"].SetValue(targetOrientation);
				newParent.Properties["Orientation"].SetValue(parentOrientation);
				targetCollection.Add(newParent);
				targetCollection.Insert(insertType == InsertType.Before ? 0 : 1, newChild);
			}
		}
		void AddToCenter(ModelItem newChild, ModelItem item, BaseLayoutItem target, DockTypeValue dockTypeValue) {
			Orientation targetOrientation = dockTypeValue.DockType.ToOrientation();
			var insertType = dockTypeValue.DockType.ToInsertType();
			if(target.IsTabPage) {
				target = target.Parent;
				item = item.Parent;
			}
			ModelItem modelTarget = ModelServiceHelper.FindModelItem(Context, target.Parent);
			ModelItemCollection targetCollection = modelTarget.Properties["Items"].Collection;
			var parentOrientation = (Orientation)modelTarget.Properties["Orientation"].ComputedValue;
			int index = targetCollection.IndexOf(item);
			if(parentOrientation == targetOrientation) {
				index += insertType == InsertType.Before ? 0 : 1;
				targetCollection.Insert(index, newChild);
			}
			else {
				ModelItem newParent = ModelFactory.CreateItem(Context, typeof(LayoutGroup), null);
				newParent.Properties["Orientation"].SetValue(targetOrientation);
				targetCollection.Insert(index, newParent);
				targetCollection.Remove(item);
				newParent.ItemsProperty().Add(item);
				newParent.ItemsProperty().Insert(insertType == InsertType.Before ? 0 : 1, newChild);
			}
		}
		void Fill(ModelItem newChild, ModelItem item, ModelItem dockManagerModel, BaseLayoutItem target) {
			if(target.IsTabPage) {
				target = target.Parent;
			}
			if(target is LayoutGroup) {
				ModelItem modelTarget = item == dockManagerModel ? dockManagerModel.Content.Value : ModelServiceHelper.FindModelItem(Context, target);
				ModelItemCollection targetCollection = modelTarget.Properties["Items"].Collection;
				targetCollection.Add(newChild);
			}
			else {
				ModelItem modelTarget = ModelServiceHelper.FindModelItem(Context, target.Parent);
				ModelItemCollection targetCollection = modelTarget.Properties["Items"].Collection;
				int index = targetCollection.IndexOf(item);
				ModelItem newParent = ModelFactory.CreateItem(Context, typeof(TabbedGroup), null);
				targetCollection.Insert(index, newParent);
				targetCollection.Remove(item);
				newParent.ItemsProperty().Add(item);
				newParent.ItemsProperty().Add(newChild);
			}
		}
		LayoutTypes GetActualLayoutType(ModelItem item, LayoutTypes type, DockType dockType) {
			if(type == LayoutTypes.DocumentPanel) {
				return (dockType == DockType.Fill && (item.Is<DocumentPanel>() || item.Is<DocumentGroup>() || item.Parent.Is<DocumentGroup>())) ?
					LayoutTypes.DocumentPanel : LayoutTypes.DocumentGroup;
			}
			return type;
		}
		public void AddItem(ModelItem item, DockTypeValue dockTypeValue, LayoutTypes type) {
			if(dockTypeValue == null || dockTypeValue.DockType == DockType.None) return;
			ModelItem newChild = CreateItem(GetActualLayoutType(item, type, dockTypeValue.DockType));
			DockLayoutManager manager = GetManager(item);
			ModelItem Root = item.GetDockLayoutManager();
			if(manager == null) return;
			manager.EnsureLayoutRoot();
			if(dockTypeValue.IsCenter) {
				BaseLayoutItem target = (!dockTypeValue.IsCenter || item.Is<DockLayoutManager>()) ? manager.LayoutRoot : item.As<BaseLayoutItem>();
				if(dockTypeValue.DockType != DockType.Fill)
					AddToCenter(newChild, item, target, dockTypeValue);
				else
					Fill(newChild, item, Root, target);
			}
			else
				AddToSide(newChild, Root.Content.Value, dockTypeValue);
		}
		DockLayoutManager GetManager(ModelItem item) {
			ModelItem manager = item.GetDockLayoutManager();
			return manager != null ? manager.As<DockLayoutManager>() : null;
		}
		public void RemoveItem(ModelItem model) {
			Type type = model.ItemType;
			ModelItem manager = model.GetDockLayoutManager();
			ModelItemCollection parentCollection;
			if(type == typeof(FloatGroup))
				parentCollection = manager.Properties["FloatGroups"].Collection;
			else
				if(type == typeof(AutoHideGroup))
					parentCollection = manager.Properties["AutoHideGroups"].Collection;
				else
					parentCollection = model.Parent.Properties["Items"].Collection;
			parentCollection.Remove(model);
		}
		public void RemoveAll(ModelItem item) {
			item.ItemsProperty().Clear();
		}
		public void HidePanel(ModelItem item) {
			ModelItem manager = item.GetDockLayoutManager();
			using(ModelEditingScope scope = item.BeginEdit(DockingLocalizer.GetString(DockingStringId.DockingOperations))) {
				RemoveItem(item);
				manager.Properties["ClosedPanels"].Collection.Add(item);
				scope.Complete();
			}
		}
		public void HideLayoutItem(ModelItem item) {
			ModelItem manager = item.GetDockLayoutManager();
			using(ModelEditingScope scope = item.BeginEdit(DockingLocalizer.GetString(DockingStringId.DockingOperations))) {
				RemoveItem(item);
				manager.Properties["HiddenItems"].Collection.Add(item);
				scope.Complete();
			}
		}
		public void AutoHideItem(ModelItem item, Dock dock) {
			DockLayoutManager manager = GetManager(item);
			if(manager == null) return;
			BaseLayoutItem target = item.As<BaseLayoutItem>();
			manager.DockController.Hide(target, dock);
		}
		public void FloatItem(ModelItem item) {
			ModelItem manager = item.GetDockLayoutManager();
			using(ModelEditingScope scope = item.BeginEdit(DockingLocalizer.GetString(DockingStringId.DockingOperations))) {
				item.Parent.Properties["Items"].Collection.Remove(item);
				ModelItem floatGroup = ModelFactory.CreateItem(Context, typeof(FloatGroup));
				floatGroup.Properties["Items"].Collection.Add(item);
				manager.Properties["FloatGroups"].Collection.Add(floatGroup);
				scope.Complete();
			}
		}
		public void Group(ModelItem item) {
			DockLayoutManager manager = GetManager(item);
			if(manager == null) return;
			BaseLayoutItem target = item.As<BaseLayoutItem>();
			manager.LayoutController.Group(new BaseLayoutItem[] { target });
		}
		public void RestoreLayout(ModelItem root, DefaultLayoutType defaultLayout) {
			if(MessageBox.Show(
				DockingLocalizer.GetString(DockingStringId.DTLoadLayoutWarning),
				DockingLocalizer.GetString(DockingStringId.DTLoadLayoutWarningCaption),
				MessageBoxButton.YesNo) == MessageBoxResult.No) {
				return;
			}
			ModelItem manager = root.GetDockLayoutManager();
			if(!manager.Content.IsSet)
				manager.Content.SetValue(ModelFactory.CreateItem(Context, typeof(LayoutGroup)));
			ModelItem layoutRoot = manager.Content.Value;
			DefaultLayoutFactory factory = new DefaultLayoutFactory(Context);
			using(ModelEditingScope scope = manager.BeginEdit(DockingLocalizer.GetString(DockingStringId.DockingOperations))) {
				layoutRoot.ItemsProperty().Clear();
				switch(defaultLayout) {
					case DefaultLayoutType.Simple:
						factory.RestoreSimpleLayout(layoutRoot);
						break;
					case DefaultLayoutType.MDI:
						factory.RestoreMDILayout(layoutRoot);
						break;
					case DefaultLayoutType.IDE:
						factory.RestoreIDELayout(layoutRoot);
						break;
				}
				Context.Items.SetValue(new Microsoft.Windows.Design.Interaction.Selection(root));
				scope.Complete();
			}
		}
		#region DefaultLayoutFactory
		class DefaultLayoutFactory {
			public EditingContext Context { get; private set; }
			public DefaultLayoutFactory(EditingContext context) {
				Context = context;
			}
			void ResetLayout(ModelItem item) {
				item.Properties["HorizontalAlignment"].ClearValue();
				item.Properties["VerticalAlignment"].ClearValue();
			}
			ModelItem CreateItem(Type type, object caption) {
				ModelItem item = ModelFactory.CreateItem(Context, type, CreateOptions.InitializeDefaults, null);
				ResetLayout(item);
				item.Properties["Caption"].SetValue(caption);
				return item;
			}
			public void RestoreMDILayout(ModelItem root) {
				ModelItem group = ModelFactory.CreateItem(Context, typeof(DocumentGroup));
				group.Properties["MDIStyle"].SetValue(MDIStyle.MDI);
				root.Properties["Items"].Collection.Add(group);
				var propertySize = new Microsoft.Windows.Design.Metadata.PropertyIdentifier(typeof(DocumentPanel), "MDISize");
				var propertyLocation = new Microsoft.Windows.Design.Metadata.PropertyIdentifier(typeof(DocumentPanel), "MDILocation");
				ModelItem panel1 = ModelFactory.CreateItem(Context, typeof(DocumentPanel), CreateOptions.InitializeDefaults, null);
				panel1.Properties["Caption"].SetValue("Document1");
				panel1.Properties[propertySize].SetValue(new Size(200, 200));
				ResetLayout(panel1);
				group.Properties["Items"].Collection.Add(panel1);
				ModelItem panel2 = ModelFactory.CreateItem(Context, typeof(DocumentPanel), CreateOptions.InitializeDefaults, null);
				panel2.Properties[propertySize].SetValue(new Size(200, 200));
				panel2.Properties[propertyLocation].SetValue(new Point(30, 30));
				panel2.Properties["Caption"].SetValue("Document2");
				ResetLayout(panel2);
				group.Properties["Items"].Collection.Add(panel2);
			}
			public void RestoreIDELayout(ModelItem root) {
				ModelItem toolbox = CreateItem(typeof(LayoutPanel), "Toolbox");
				ModelItem group1 = ModelFactory.CreateItem(Context, typeof(LayoutGroup));
				group1.Properties["Orientation"].SetValue(Orientation.Vertical);
				ModelItem group2 = ModelFactory.CreateItem(Context, typeof(LayoutGroup));
				ModelItem tab1 = ModelFactory.CreateItem(Context, typeof(TabbedGroup));
				ModelItem tab2 = ModelFactory.CreateItem(Context, typeof(TabbedGroup));
				ModelItem docGroup = ModelFactory.CreateItem(Context, typeof(DocumentGroup));
				root.ItemsProperty().AddRange(new ModelItem[] { group1, tab1 });
				group1.ItemsProperty().AddRange(new ModelItem[] { group2, tab2 });
				group2.ItemsProperty().AddRange(new ModelItem[] { toolbox, docGroup });
				ModelItem document1 = CreateItem(typeof(DocumentPanel), "Document1");
				docGroup.Properties["Items"].Collection.Add(document1);
				ModelItem document2 = CreateItem(typeof(DocumentPanel), "Document2");
				docGroup.Properties["Items"].Collection.Add(document2);
				ModelItem errorList = CreateItem(typeof(LayoutPanel), "Error List");
				tab1.Properties["Items"].Collection.Add(errorList);
				ModelItem output = CreateItem(typeof(LayoutPanel), "Output");
				tab1.Properties["Items"].Collection.Add(output);
				ModelItem findResults = CreateItem(typeof(LayoutPanel), "Find Results");
				tab1.Properties["Items"].Collection.Add(findResults);
				ModelItem solutionExplorer = CreateItem(typeof(LayoutPanel), "Solution Explorer");
				tab2.Properties["Items"].Collection.Add(solutionExplorer);
				ModelItem properties = CreateItem(typeof(LayoutPanel), "Properties");
				tab2.Properties["Items"].Collection.Add(properties);
			}
			public void RestoreSimpleLayout(ModelItem root) {
				ModelItem group = ModelFactory.CreateItem(Context, typeof(LayoutGroup));
				group.Properties["Orientation"].SetValue(Orientation.Vertical);
				root.Properties["Items"].Collection.Add(group);
				ModelItem panel1 = CreateItem(typeof(LayoutPanel), "Panel1");
				group.Properties["Items"].Collection.Add(panel1);
				ModelItem panel2 = CreateItem(typeof(LayoutPanel), "Panel2");
				group.Properties["Items"].Collection.Add(panel2);
				ModelItem panel3 = CreateItem(typeof(LayoutPanel), "Panel3");
				root.Properties["Items"].Collection.Add(panel3);
			}
		}
		#endregion
		public void SaveLayout(DockLayoutManager manager) {
			var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
			PrepareFileDialog(saveFileDialog);
			bool? result = saveFileDialog.ShowDialog();
			if(result != null && result.Value) {
				manager.SaveLayoutToXml(saveFileDialog.FileName);
			}
		}
		public void RestoreLayout(ModelItem modelItem) {
			var openFileDialog = new Microsoft.Win32.OpenFileDialog();
			PrepareFileDialog(openFileDialog);
			bool? result = openFileDialog.ShowDialog();
			if(result != null && result.Value) {
				SelectionOperations.Select(Context, modelItem);
				modelItem.Content.ClearValue();
				DockLayoutManager manager = modelItem.View.PlatformObject as DockLayoutManager;
				manager.EnsureLayoutRoot();
				manager.RestoreLayoutFromXml(openFileDialog.FileName);
			}
		}
		void PrepareFileDialog(Microsoft.Win32.FileDialog dialog) {
			dialog.FileName = "Layout";
			dialog.DefaultExt = ".xml";
			dialog.Filter = "*.xml|*.xml";
		}
		bool? RequestItemCreation() {
			if(!RestoreBoolProperty(StoredProperty.CreateItemAskNextTime)) return RestoreBoolProperty(StoredProperty.CreateItemAction);
			CreateLayoutDialog dialog = new CreateLayoutDialog();
			dialog.ShowDialog();
			if(dialog.Result != null) {
				StoreProperty(StoredProperty.CreateItemAskNextTime, dialog.ShouldAskNextTime);
				StoreProperty(StoredProperty.CreateItemAction, dialog.Result);
			}
			return dialog.Result;
		}
		public void SetPanelContent(ModelItem target, ModelItem content) {
			if(typeof(LayoutPanel).IsAssignableFrom(target.ItemType)) {
				LayoutPanel targetPanel = target.As<LayoutPanel>();
				if(targetPanel.Content != null) {
					bool? messageBoxResult = RequestItemCreation();
					switch(messageBoxResult) {
						case true:
							if(targetPanel.Layout != null) {
								ModelItem layout = target.Properties["Content"].Value;
								AddLayoutControlItem(layout, content);
							}
							else
								SetPanelLayout(target, targetPanel, content);
							return;
						case false:
							break;
						default:
							return;
					}
				}
				SetPanelContent(target, targetPanel, content);
			}
		}
		void CreatePanelContent(ModelItem target, BaseLayoutItem targetPanel, object content) {
			target.Content.SetValue(content);
		}
		void SetPanelLayout(ModelItem target, BaseLayoutItem targetPanel, ModelItem content) {
			ModelItem groupModel = ModelFactory.CreateItem(Context, typeof(LayoutGroup));
			groupModel.Properties["Orientation"].SetValue(Orientation.Vertical);
			ModelItem modelItem1 = ModelFactory.CreateItem(Context, typeof(LayoutControlItem));
			modelItem1.Properties["Caption"].SetValue(DefaultLayoutControlItemCaption);
			ModelItem modelItem2 = ModelFactory.CreateItem(Context, typeof(LayoutControlItem));
			modelItem2.Properties["Caption"].SetValue(DefaultLayoutControlItemCaption);
			ModelItem oldContent = target.Properties["Content"].Value;
			modelItem1.Content.SetValue(oldContent);
			modelItem2.Content.SetValue(content);
			groupModel.ItemsProperty().Add(modelItem1);
			groupModel.ItemsProperty().Add(modelItem2);
			CreatePanelContent(target, targetPanel, groupModel);
		}
		void SetPanelContent(ModelItem target, BaseLayoutItem targetPanel, ModelItem content) {
			if(target.Is<LayoutPanel>()) {
				CreatePanelContent(target, targetPanel, content);
			}
		}
		public void AddLayoutControlItem(ModelItem parent, ModelItem child) {
			ModelItem model = CreateItem(LayoutTypes.ControlItem);
			model.Content.SetValue(child);
			parent.ItemsProperty().Add(model);
		}
		public void CreateNewPanelForChild(ModelItem newParent, ModelItem child) {
			ModelItem rootItem = newParent.Properties["LayoutRoot"].Value;
			ModelItem newPanel = rootItem.ItemsProperty().Add(new LayoutPanel() { Caption = DefaultLayoutPanelCaption });
			SetContent(newPanel, child);
		}
		public void SetContent(ModelItem target, ModelItem content) {
			content.ResetLayout();
			if(typeof(LayoutPanel).IsAssignableFrom(target.ItemType))
				target.Properties["Content"].SetValue(content);
			if(typeof(LayoutControlItem).IsAssignableFrom(target.ItemType))
				target.Content.SetValue(content);
		}
		public void ResetLayout(ModelItem modelItem) {
			BaseLayoutItem item = modelItem.As<BaseLayoutItem>();
			switch(item.ItemType) {
				case LayoutItemType.Panel:
				case LayoutItemType.Document:
					ResetLayoutPanel(modelItem);
					break;
				case LayoutItemType.Group:
					ReseLayoutGroup(modelItem);
					break;
				case LayoutItemType.ControlItem:
					ResetLayoutControlItem(modelItem);
					break;
				case LayoutItemType.FixedItem:
					ResetFixedItem(modelItem);
					break;
			}
		}
		void ResetLayoutPanel(ModelItem item) {
			item.Properties["Content"].ClearValue();
		}
		void ReseLayoutGroup(ModelItem item) {
			item.Properties["GroupBorderStyle"].ClearValue();
			item.Properties["Orientation"].ClearValue();
		}
		void ResetLayoutControlItem(ModelItem item) {
			item.Properties["CaptionHorizontalAlignment"].ClearValue();
			item.Properties["CaptionVerticalAlignment"].ClearValue();
			item.Properties["ControlHorizontalAlignment"].ClearValue();
			item.Properties["ControlVerticalAlignment"].ClearValue();
			item.Properties["CaptionLocation"].ClearValue();
		}
		void ResetFixedItem(ModelItem item) {
			item.Properties["ContentHorizontalAlignment"].ClearValue();
			item.Properties["ContentVerticalAlignment"].ClearValue();
		}
		public EditingContext Context { get; private set; }
		PropertyStoreHelper PropertyStoreHelper { get; set; }
		public void StoreProperty(StoredProperty property, object value) {
			PropertyStoreHelper.StoreProperty(property, value);
		}
		public void Store() {
			PropertyStoreHelper.Store();
		}
		public int RestoreIntProperty(StoredProperty property) {
			return PropertyStoreHelper.RestoreIntProperty(property);
		}
		public bool RestoreBoolProperty(StoredProperty property) {
			return PropertyStoreHelper.RestoreBoolProperty(property);
		}
		public void ResetProperties() {
			PropertyStoreHelper.ResetProperties();
			RaiseCustomizationReseted();
		}
		public event EventHandler CustomizationReseted;
		protected void RaiseCustomizationReseted() {
			if(CustomizationReseted != null)
				CustomizationReseted(this, RoutedEventArgs.Empty);
		}
	}
	class PropertyStoreHelper {
		public const string SettingsPath = "Software\\Developer Express\\Designer\\DxDocking\\";
		PropertyStore PropertyStore { get; set; }
		Dictionary<StoredProperty, object> StoredValues = new Dictionary<StoredProperty, object>();
		Dictionary<StoredProperty, object> DefaultValues = new Dictionary<StoredProperty, object>();
		public PropertyStoreHelper() {
			PropertyStore = new PropertyStore(SettingsPath);
			InitDefaultValues();
			RestoreProperties();
		}
		void InitDefaultValues() {
			DefaultValues.Add(StoredProperty.CustomizationPanelLeft, 0);
			DefaultValues.Add(StoredProperty.CustomizationPanelTop, 0);
			DefaultValues.Add(StoredProperty.CustomizationPanelIsExpanded, true);
			DefaultValues.Add(StoredProperty.CustomizationPanelIsLayoutTreeExpanded, true);
			DefaultValues.Add(StoredProperty.CreateItemAskNextTime, true);
			DefaultValues.Add(StoredProperty.CreateItemAction, false);
		}
		void RestoreProperties() {
			PropertyStore.Restore();
			RestoreInt(StoredProperty.CustomizationPanelLeft);
			RestoreInt(StoredProperty.CustomizationPanelTop);
			RestoreBool(StoredProperty.CustomizationPanelIsExpanded);
			RestoreBool(StoredProperty.CustomizationPanelIsLayoutTreeExpanded);
			RestoreBool(StoredProperty.CreateItemAskNextTime);
			RestoreBool(StoredProperty.CreateItemAction);
		}
		void RestoreInt(StoredProperty property) {
			StoredValues[property] = PropertyStore.RestoreIntProperty(property.ToString(), (int)DefaultValues[property]);
		}
		void RestoreBool(StoredProperty property) {
			StoredValues[property] = PropertyStore.RestoreBoolProperty(property.ToString(), (bool)DefaultValues[property]);
		}
		public void StoreProperty(StoredProperty property, object value) {
			StoredValues[property] = value;
		}
		public void Store() {
			foreach(KeyValuePair<StoredProperty, object> pair in StoredValues) {
				PropertyStore.AddProperty(pair.Key.ToString(), pair.Value);
			}
			PropertyStore.Store();
		}
		public int RestoreIntProperty(StoredProperty property) {
			object value;
			bool result = StoredValues.TryGetValue(property, out value);
			return (int)(result ? value : DefaultValues[property]);
		}
		public bool RestoreBoolProperty(StoredProperty property) {
			object value;
			bool result = StoredValues.TryGetValue(property, out value);
			return (bool)(result ? value : DefaultValues[property]);
		}
		public void ResetProperties() {
			foreach(KeyValuePair<StoredProperty, object> pair in DefaultValues) {
				StoredValues[pair.Key] = pair.Value;
			}
			Store();
		}
	}
	enum StoredProperty {
		CustomizationPanelLeft,
		CustomizationPanelTop,
		CustomizationPanelIsExpanded,
		CustomizationPanelIsLayoutTreeExpanded,
		CreateItemAskNextTime,
		CreateItemAction
	}
}
