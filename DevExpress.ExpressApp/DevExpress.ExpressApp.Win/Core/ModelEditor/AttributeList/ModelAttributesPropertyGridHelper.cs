#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Events;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public interface IModelAttributesPropertyEditorControl {
		void BeginDataUpdate();
		void EndDataUpdate();
		void SetImages(ImageCollection imageCollection);
		void SetErrorMessages(ErrorMessages errorMessages);
		void RefreshValues();
		void BeginSelectedObjectsUpdate();
		void EndSelectedObjectsUpdate();
		ModelTreeListNode SelectedObject { get; set; }
		ModelTreeListNode[] SelectedObjects { get; set; }
		String FocusedPropertyName { get; set; }
		bool EditorErrorState { get; }
		event EventHandler<CustomFillRepositoryItemEventArgs> CustomFillRepositoryItem;
		event CustomPropertyDescriptorsEventHandler CustomPropertyDescriptors;
		event FocusedRowChangedEventHandler FocusedRowChanged;
		event EventHandler<PropertyChangingEventArgs> PropertyChanging;
		event EventHandler<PropertyChangingEventArgs> PropertyChanged;
		event EventHandler<CalculateCellPropertyEventArgs> CalculateCellProperty;
		event EventHandler<CalculateItemImageIndexEventArgs> CalculateItemImageIndex;
		event EventHandler<ModelPropertyEditorActionExecuteEventArgs> ModelPropertyEditorActionExecute;
		event EventHandler<CustomHandleExceptionEventArgs> HandleException;
		event EventHandler DataSourceChanged;
	}
	public class ModelAttributesPropertyEditorController {
		SimpleAction resetAction = null;
		SimpleAction navigateToNodeAction = null;
		SimpleAction navigateToCalculatedPropertyAction = null;
		protected IModelAttributesPropertyEditorControl propertyEditor;
		private FastModelEditorHelper modelEditorHelper;
		public ModelAttributesPropertyEditorController() { }
		public ModelAttributesPropertyEditorController(IModelAttributesPropertyEditorControl propertyEditor, FastModelEditorHelper modelEditorHelper)
			: base() {
			this.modelEditorHelper = modelEditorHelper;
			InitializeComponent();
			SetControl(propertyEditor);
		}
		protected FastModelEditorHelper fastModelEditorHelper {
			get {
				return modelEditorHelper;
			}
		}
		private void InitializeComponent() {
			this.resetAction = new SimpleAction();
			this.resetAction.Caption = "ResetProperty";
			this.resetAction.Category = "ObjectsCreation";
			this.resetAction.Id = "ResetProperty";
			this.resetAction.Execute += new SimpleActionExecuteEventHandler(ResetAction_Execute);
			this.navigateToNodeAction = new SimpleAction();
			this.navigateToNodeAction.Caption = "NavigateToNode";
			this.navigateToNodeAction.Category = "ObjectsCreation";
			this.navigateToNodeAction.Id = "NavigateToNode";
			this.navigateToNodeAction.Execute += new SimpleActionExecuteEventHandler(NavigateToNodeAction_Execute);
			this.navigateToCalculatedPropertyAction = new SimpleAction();
			this.navigateToCalculatedPropertyAction.Caption = "NavigateToCalculatedProperty";
			this.navigateToCalculatedPropertyAction.Category = "ObjectsCreation";
			this.navigateToCalculatedPropertyAction.Id = "NavigateToCalculatedProperty";
			this.navigateToCalculatedPropertyAction.Execute += new SimpleActionExecuteEventHandler(NavigateToCalculatedPropertyAction_Execute);
		}
		protected virtual void NavigateToCalculatedPropertyAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			throw new NotImplementedException();
		}
		protected virtual void NavigateToNodeAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			throw new NotImplementedException();
		}
		protected virtual void ResetAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			throw new NotImplementedException();
		}
		protected virtual void SubscribeEvents() {
			if(propertyEditor != null) {
				propertyEditor.ModelPropertyEditorActionExecute += new EventHandler<ModelPropertyEditorActionExecuteEventArgs>(propertyEditor_ModelPropertyEditorActionExecute);
			}
		}
		protected virtual void UnSubscribeEvents() {
			if(propertyEditor != null) {
				propertyEditor.ModelPropertyEditorActionExecute -= new EventHandler<ModelPropertyEditorActionExecuteEventArgs>(propertyEditor_ModelPropertyEditorActionExecute);
			}
		}
		private void propertyEditor_ModelPropertyEditorActionExecute(object sender, ModelPropertyEditorActionExecuteEventArgs e) {
			switch(e.Action) {
				case ModelPropertyEditorAction.ResetProperty:
					ResetAction.DoExecute();
					break;
				case ModelPropertyEditorAction.NavigateToNode:
					NavigateToNodeAction.DoExecute();
					break;
				case ModelPropertyEditorAction.NavigateToCalculatedProperty:
					NavigateToCalculatedPropertyAction.DoExecute();
					break;
			}
		}
		protected virtual ImageCollection GetImages() {
			DevExpress.Utils.ImageCollection imageCollection = new DevExpress.Utils.ImageCollection();
			imageCollection.AddImage(ImageLoader.Instance.GetImageInfo("BO_Localization").Image);
			imageCollection.AddImage(ImageLoader.Instance.GetImageInfo("ModelEditor_KeyProperty").Image);
			imageCollection.AddImage(ImageLoader.Instance.GetImageInfo("ModelEditor_RequiredProperty").Image);
			return imageCollection;
		}
		protected FastModelEditorHelper ModelEditorHelper {
			get {
				return modelEditorHelper;
			}
		}
		public void SetControl(IModelAttributesPropertyEditorControl propertyEditor) {
			this.propertyEditor = propertyEditor;
			UnSubscribeEvents();
			this.propertyEditor = propertyEditor;
			SubscribeEvents();
			if(propertyEditor != null) {
				this.propertyEditor.SetImages(GetImages());
			}
		}
		public IModelAttributesPropertyEditorControl PropertyEditor {
			get { return propertyEditor; }
		}
		public SimpleAction ResetAction {
			get {
				return resetAction;
			}
		}
		public SimpleAction NavigateToNodeAction {
			get {
				return navigateToNodeAction;
			}
		}
		public SimpleAction NavigateToCalculatedPropertyAction {
			get {
				return navigateToCalculatedPropertyAction;
			}
		}
		public void Dispose() {
			UnSubscribeEvents();
			if(resetAction != null) {
				this.resetAction.Execute -= new SimpleActionExecuteEventHandler(ResetAction_Execute);
				resetAction.Dispose();
				resetAction = null;
			}
			if(navigateToNodeAction != null) {
				this.navigateToNodeAction.Execute -= new SimpleActionExecuteEventHandler(NavigateToNodeAction_Execute);
				navigateToNodeAction.Dispose();
				navigateToNodeAction = null;
			}
			if(navigateToCalculatedPropertyAction != null) {
				this.navigateToCalculatedPropertyAction.Execute -= new SimpleActionExecuteEventHandler(NavigateToCalculatedPropertyAction_Execute);
				navigateToCalculatedPropertyAction.Dispose();
				navigateToCalculatedPropertyAction = null;
			}
			propertyEditor = null;
		}
		#region Obsolete 15.1
		[Obsolete("Use the SetControl method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetContorl(IModelAttributesPropertyEditorControl propertyEditor) {
			SetControl(propertyEditor);
		}
		#endregion
	}
	[Flags]
	public enum ModelPropertyEditorCellProperty { Default = 1, Localization = 2, CalculateProperty = 4, Required = 8, RefProperty = 16, ModifiedProperty = 32, ReadOnly = 64 }
	public class ModelAttributesPropertyEditorControllerWin : ModelAttributesPropertyEditorController {
		Dictionary<object, PropertyDescriptorCollection> customPropertyDescriptorsCache = new Dictionary<object, PropertyDescriptorCollection>();
		private Dictionary<string, bool> hideProperty = new Dictionary<string, bool>();
		private Dictionary<string, bool?> propertyVisibleInVirtualTree = new Dictionary<string, bool?>();
		public ModelAttributesPropertyEditorControllerWin() : this(null) { }
		public ModelAttributesPropertyEditorControllerWin(FastModelEditorHelper modelEditorHelper) : this(null, modelEditorHelper) { }
		public ModelAttributesPropertyEditorControllerWin(IModelAttributesPropertyEditorControl propertyEditor, FastModelEditorHelper modelEditorHelper)
			: base(propertyEditor, modelEditorHelper) {
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			if(propertyEditor != null) {
				propertyEditor.CalculateCellProperty += new EventHandler<CalculateCellPropertyEventArgs>(propertyEditor_CalculateCellProperty);
				propertyEditor.PropertyChanging += new EventHandler<PropertyChangingEventArgs>(propertyEditor_EditValueChanging);
				propertyEditor.PropertyChanged += new EventHandler<PropertyChangingEventArgs>(propertyEditor_PropertyChanged);
				propertyEditor.CustomPropertyDescriptors += new CustomPropertyDescriptorsEventHandler(propertyEditor_CustomPropertyDescriptors);
				propertyEditor.CustomFillRepositoryItem += new EventHandler<CustomFillRepositoryItemEventArgs>(propertyEditor_CustomFillRepositoryItem);
				propertyEditor.CalculateItemImageIndex += new EventHandler<CalculateItemImageIndexEventArgs>(propertyEditor_CalculateItemImageIndex);
				propertyEditor.DataSourceChanged += new EventHandler(propertyEditor_DataSourceChanged);
			}
		}
		protected override void UnSubscribeEvents() {
			base.UnSubscribeEvents();
			if(propertyEditor != null) {
				propertyEditor.CalculateCellProperty -= new EventHandler<CalculateCellPropertyEventArgs>(propertyEditor_CalculateCellProperty);
				propertyEditor.PropertyChanging -= new EventHandler<PropertyChangingEventArgs>(propertyEditor_EditValueChanging);
				propertyEditor.PropertyChanged -= new EventHandler<PropertyChangingEventArgs>(propertyEditor_PropertyChanged);
				propertyEditor.CustomPropertyDescriptors -= new CustomPropertyDescriptorsEventHandler(propertyEditor_CustomPropertyDescriptors);
				propertyEditor.CustomFillRepositoryItem -= new EventHandler<CustomFillRepositoryItemEventArgs>(propertyEditor_CustomFillRepositoryItem);
				propertyEditor.CalculateItemImageIndex -= new EventHandler<CalculateItemImageIndexEventArgs>(propertyEditor_CalculateItemImageIndex);
				propertyEditor.DataSourceChanged -= new EventHandler(propertyEditor_DataSourceChanged);
			}
		}
		void propertyEditor_CalculateItemImageIndex(object sender, CalculateItemImageIndexEventArgs e) {
			ModelValueInfo valueInfo = e.Node.NodeInfo.GetValueInfo(e.PropertyName);
			if(valueInfo != null && valueInfo.IsLocalizable) {
				e.ImageIndex = 0;
			}
			else {
				if(IsKeyProperty(e.Node, e.PropertyName)) {
					e.ImageIndex = 1;
				}
				else {
					if(IsRequired(e.Node, e.PropertyName)) {
						e.ImageIndex = 2;
					}
				}
			}
		}
		void propertyEditor_CustomFillRepositoryItem(object sender, CustomFillRepositoryItemEventArgs e) {
			e.RepositoryItem = FillRepositoryItem(e.RepositoryItem, EditedModelNode, e.PropertyName);
		}
		void propertyEditor_CustomPropertyDescriptors(object sender, CustomPropertyDescriptorsEventArgs e) {
			PropertyDescriptorCollection result;
			if(!customPropertyDescriptorsCache.TryGetValue(e.Source, out result)) {
				result = CollectCustomPropertyDescriptors(e.Source, e.Context.PropertyDescriptor, e.Properties);
				customPropertyDescriptorsCache[e.Source] = result;
			}
			e.Properties = result;
		}
		private PropertyDescriptorCollection CollectCustomPropertyDescriptors(object source, PropertyDescriptor propertyDescriptor, PropertyDescriptorCollection properties) {
			List<PropertyDescriptor> visibleItems = new List<PropertyDescriptor>();
			string prefix = propertyDescriptor != null ? propertyDescriptor.Name : null;
			ModelNode node = source as ModelNode;
			for(int counter = 0; counter < properties.Count; counter++) {
				string propertyName = prefix == null
					? properties[counter].Name
					: prefix + "." + properties[counter].Name;
				if(node == null || IsPropertyVisible(node, propertyName)) {
					visibleItems.Add(properties[counter]);
				}
			}
			return new PropertyDescriptorCollection(visibleItems.ToArray(), true);
		}
		void propertyEditor_EditValueChanging(object sender, PropertyChangingEventArgs e) {
			e.Cancel = !CanModified(e.PropertyName);
		}
		void propertyEditor_PropertyChanged(object sender, PropertyChangingEventArgs e) {
			if(PropertyChanged != null) {
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(e.PropertyName);
				PropertyChanged(this, args);
			}
			ClearCustomPropertyDescriptorsCache();
		}
		void propertyEditor_CalculateCellProperty(object sender, CalculateCellPropertyEventArgs e) {
			e.CellStyle = GetCellProperty(e.SelectedObjects, e.PropertyName);
		}
		private ModelNode EditedModelNode {
			get {
				ModelTreeListNode selectedNode = propertyEditor.SelectedObject;
				if(selectedNode != null) {
					return selectedNode.ModelNode;
				}
				else {
					return null;
				}
			}
		}
		private ModelNode RefValue(ModelNode sourceNode, string propertyName, out string propertyToSelect) {
			propertyToSelect = string.Empty;
			if(sourceNode == null || string.IsNullOrEmpty(propertyName)) {
				return null;
			}
			string sourceNodePropertyName = null;
			ModelValueCalculatorAttribute attribute = fastModelEditorHelper.GetPropertyAttribute<ModelValueCalculatorAttribute>(sourceNode, propertyName);
			ModelNode result = null;
			if(attribute != null) {
				if(!string.IsNullOrEmpty(attribute.LinkValue)) {
					propertyToSelect = attribute.LinkValue;
					result = sourceNode;
				}
				if(!string.IsNullOrEmpty(attribute.NodeName)) {
					sourceNodePropertyName = attribute.NodeName;
				}
				if(!string.IsNullOrEmpty(attribute.PropertyName)) {
					propertyToSelect = attribute.PropertyName;
				}
			}
			else {
				sourceNodePropertyName = sourceNode.NodeInfo.GetSourceNodePath(propertyName);
				if(!string.IsNullOrEmpty(sourceNodePropertyName)) {
					propertyToSelect = propertyName;
				}
				else {
					sourceNodePropertyName = propertyName;
				}
			}
			if(result == null && !string.IsNullOrEmpty(propertyToSelect)) {
				ITypeInfo nodeInfo = XafTypesInfo.Instance.FindTypeInfo(sourceNode.GetType());
				if(nodeInfo != null) {
					IMemberInfo memberInfo = nodeInfo.FindMember(sourceNodePropertyName);
					if(memberInfo != null) {
						result = memberInfo.GetValue(sourceNode) as ModelNode;
					}
				}
			}
			return result;
		}
		private RepositoryItem CreatePrefferedLanguageRepositoryItem(RepositoryItem repositoryItem, ModelNode modelNode, string propertyName) {
			if(propertyName == "PreferredLanguage") {
				RepositoryItemComboBox comboBox = new RepositoryItemComboBox();
				foreach(object modelClass in AspectNames(modelNode.Application)) {
					comboBox.Items.Add(modelClass);
				}
				return comboBox;
			}
			else {
				return repositoryItem;
			}
		}
		private RepositoryItem CreatePropertyNameRepositoryItem(RepositoryItem repositoryItem, ModelNode modelNode, string propertyName) {
			if(propertyName == "PropertyName") {
				IModelObjectView viewInfo = null;
				if(modelNode is IModelMemberViewItem) {
					viewInfo = ((IModelMemberViewItem)modelNode).ParentView as IModelObjectView;
				}
				else if((modelNode is IModelSortProperty) && (modelNode.Parent is IModelSorting)) {
					viewInfo = ((IModelSortProperty)modelNode).Parent.Parent as IModelObjectView;
				}
				if(viewInfo != null && viewInfo.ModelClass != null) {
					RepositoryFieldPicker picker = new RepositoryFieldPicker();
					repositoryItem = picker;
					picker.ClassType = viewInfo.ModelClass.TypeInfo.Type;
				}
			}
			if(propertyName == "LookupProperty" && modelNode is IModelMemberViewItem) {
				IModelObjectView viewInfo = ((IModelMemberViewItem)modelNode).ParentView as IModelObjectView;
				if(viewInfo != null && viewInfo.ModelClass != null) {
					IMemberInfo lokupProperty = viewInfo.ModelClass.TypeInfo.FindMember(((IModelMemberViewItem)modelNode).PropertyName);
					if(lokupProperty != null) {
						RepositoryFieldPicker picker = new RepositoryFieldPicker();
						repositoryItem = picker;
						picker.ClassType = lokupProperty.MemberTypeInfo.Type;
					}
				}
			}
			if(propertyName == "TargetPropertyName") {
				Type targetType = modelNode.GetValue<Type>("TargetType");
				if(targetType != null) {
					RepositoryFieldPicker picker = new RepositoryFieldPicker();
					repositoryItem = picker;
					picker.ClassType = targetType;
				}
			}
			return repositoryItem;
		}
		private RepositoryItemComboBox CreateDataSourcePropertyRepositoryItem(IEnumerable valuesList, ModelNode modelNode, string propertyName, Type elementType) {
			RepositoryItemComboBox comboBox = new RepositoryItemModelNodesLookup();
			Type comboBoxItemsType = null;
			DataSourceCriteriaAttribute dataSourceCriteariaAttribute = fastModelEditorHelper.GetPropertyAttribute<DataSourceCriteriaAttribute>(modelNode, propertyName);
			ExpressionEvaluator expressionEvaluator = null;
			if(dataSourceCriteariaAttribute != null && elementType != null) {
				CriteriaWrapper criteriaWrapper = new CriteriaWrapper(dataSourceCriteariaAttribute.Value.ToString(), modelNode);
				criteriaWrapper.UpdateParametersValues(modelNode);
				expressionEvaluator = GetExpressionEvaluator(elementType, criteriaWrapper.CriteriaOperator);
			}
			Boolean isRequired = fastModelEditorHelper.IsRequired(modelNode, propertyName);
			if(!isRequired) {
				comboBox.Items.Add(CaptionHelper.NoneValue);
			}
			IMemberInfo currentPropertyMemberInfo = XafTypesInfo.Instance.FindTypeInfo(modelNode.GetType()).FindMember(propertyName);
			foreach(object modelClass in (IEnumerable)valuesList) {
				if(currentPropertyMemberInfo.MemberType.IsAssignableFrom(modelClass.GetType())) {
					if(expressionEvaluator == null || expressionEvaluator.Fit(modelClass)) {
						comboBox.Items.Add(modelClass);
						comboBoxItemsType = modelClass.GetType();
					}
				}
			}
			if((modelNode is IModelClass || modelNode is IModelListView) && comboBoxItemsType != null && typeof(IModelView).IsAssignableFrom(comboBoxItemsType)) {
				return GetSortedComboBox(comboBox, modelNode);
			}
			return comboBox;
		}
		private RepositoryItemComboBox GetSortedComboBox(RepositoryItemComboBox comboBox, ModelNode modelNode) {
			RepositoryItemComboBox result = new RepositoryItemModelNodesLookup();
			if(comboBox.Items[0] as string == CaptionHelper.NoneValue) {
				result.Items.Add(CaptionHelper.NoneValue);
				comboBox.Items.RemoveAt(0);
			}
			IModelClass currentClass = (modelNode is IModelClass) ? (IModelClass)modelNode : ((IModelListView)modelNode).ModelClass;
			foreach(object item in ViewNamesCalculator.SortByInheritanceHierarchy(comboBox.Items.OfType<IModelView>().ToList(), currentClass)) {
				result.Items.Add(item);
			}
			return result;
		}
		private bool GetReadOnlyForAllowEditAttribute(ModelNode node, string propertyName) {
			IMemberInfo memberInfo = null;
			if(propertyName == "AllowEdit") {
				if(node is IModelMember) {
					memberInfo = ((IModelMember)node).MemberInfo;
				}
				else if(node is IModelMemberViewItem && ((IModelMemberViewItem)node).ModelMember != null) {
					memberInfo = ((IModelMemberViewItem)node).ModelMember.MemberInfo;
				}
			}
			return memberInfo != null && memberInfo.IsReadOnly && SimpleTypes.IsSimpleType(memberInfo.MemberType);
		}
		private void SetCellProperty(ModelPropertyEditorCellProperty property, ref ModelPropertyEditorCellProperty result) {
			if(result == ModelPropertyEditorCellProperty.Default) {
				result = property;
			}
			else {
				result |= property;
			}
		}
		private void propertyEditor_DataSourceChanged(object sender, EventArgs e) {
			ClearCustomPropertyDescriptorsCache();
		}
		private void ClearCustomPropertyDescriptorsCache() {
			customPropertyDescriptorsCache = new Dictionary<object, PropertyDescriptorCollection>();
		}
		private bool CalculatePropertyVisible(ModelNode node, string propertyName) {
			bool result = false;
			if(propertyName == ModelValueNames.Index) {
				ModelNode parent = node.Parent;
				result = parent != null && !(parent is ModelApplicationBase);
			}
			else {
				List<string> propertyPath = new List<string>(propertyName.Split('.'));
				if(propertyPath.Count > 1) {
					object value = node.GetValue(propertyPath[0]);
					ModelNode nextNode = value as ModelNode;
					if(nextNode != null) {
						propertyPath.Remove(propertyPath[0]);
						result = IsPropertyVisible(nextNode, string.Join(".", propertyPath.ToArray()));
					}
					else {
						result = true;
					}
				}
				else {
					ModelValueInfo modelValueInfo = node.GetValueInfo(propertyName);
					if(modelValueInfo != null) {
						BrowsableAttribute browsableAttribute = fastModelEditorHelper.GetPropertyAttribute<BrowsableAttribute>(node, propertyName);
						result = browsableAttribute != null ? browsableAttribute.Browsable : fastModelEditorHelper.IsPropertyModelBrowsableVisible(node, propertyName);
					}
					else {
						result = node.KeyValueName == propertyName;
					}
				}
			}
			if(CalculateHideProperties(node, propertyName)) {
				result = false;
			}
			result = CalculatePropertyVisibleInVirtualTree(node, propertyName, result);
			return result;
		}
		private bool CalculateHideProperties(ModelNode node, string propertyName) {
			bool result;
			string key = node.GetType().FullName + " " + propertyName;
			if(!hideProperty.TryGetValue(key, out result)) {
				ModelValueInfo modelValueInfo = node.GetValueInfo(propertyName);
				result = false;
				if(modelValueInfo != null) {
					ModelHidePropertiesAttribute[] browsableAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelHidePropertiesAttribute>(node.GetType(), true);
					if(browsableAttributes != null) {
						foreach(ModelHidePropertiesAttribute attr in browsableAttributes) {
							if(attr.HideProperties != null) {
								foreach(string displayPropertyName in attr.HideProperties) {
									if(displayPropertyName == propertyName) {
										result = true;
										break;
									}
								}
							}
							if(result) {
								break;
							}
						}
					}
				}
				hideProperty[key] = result;
			}
			return result;
		}
		private bool CalculatePropertyVisibleInVirtualTree(ModelNode node, string propertyName, bool defaultValue) {
			if(ModelTreeNodeIsVitrual(node)) {
				bool? result;
				string key = node.GetType().FullName + " " + propertyName;
				if(!propertyVisibleInVirtualTree.TryGetValue(key, out result)) {
					ModelValueInfo modelValueInfo = node.GetValueInfo(propertyName);
					if(modelValueInfo != null) {
						ModelVirtualTreeDisplayPropertiesAttribute[] browsableAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeDisplayPropertiesAttribute>(node.GetType(), true);
						if(browsableAttributes != null && browsableAttributes.Length > 0) {
							foreach(ModelVirtualTreeDisplayPropertiesAttribute attr in browsableAttributes) {
								if(attr.All) {
									result = true;
									break;
								}
								if(attr.DisplayProperties != null) {
									foreach(string displayPropertyName in attr.DisplayProperties) {
										if(displayPropertyName == propertyName) {
											result = true;
											break;
										}
									}
								}
								if(result.HasValue && result.Value) {
									break;
								}
							}
							if(result == null) {
								result = false;
							}
						}
					}
					propertyVisibleInVirtualTree[key] = result;
					if(result == null) {
						result = defaultValue;
					}
				}
				return result.HasValue ? result.Value : defaultValue;
			}
			return defaultValue;
		}
		private bool ModelTreeNodeIsVitrual(ModelNode node) {
			if(propertyEditor != null) {
				ModelTreeListNode[] selectedNodes = propertyEditor.SelectedObjects;
				if(selectedNodes != null) {
					foreach(ModelTreeListNode treeNode in selectedNodes) {
						if(treeNode.ModelNode == node) {
							return treeNode.VirtualTreeNode;
						}
					}
				}
			}
			return false;
		}
		protected override void NavigateToNodeAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(IsRefProperty(EditedModelNode, propertyEditor.FocusedPropertyName, propertyEditor.SelectedObjects)) {
				ModelNode nodeToNavigate = RefNode(EditedModelNode, propertyEditor.FocusedPropertyName);
				if(nodeToNavigate != null) {
					NavigateToRefProperty(this, new NavigateToEventArgs(nodeToNavigate, ""));
				}
			}
		}
		protected override void NavigateToCalculatedPropertyAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			if(IsCalculatedProperty(EditedModelNode, propertyEditor.FocusedPropertyName)) {
				HeaderCellClick(EditedModelNode, propertyEditor.FocusedPropertyName);
			}
		}
		protected override void ResetAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
			foreach(ModelTreeListNode selectedObject in propertyEditor.SelectedObjects) {
				ModelNode targetNode = selectedObject.ModelNode;
				if(targetNode != null) {
					if(!IsCellDefaultValue(targetNode, propertyEditor.FocusedPropertyName)) {
						ResetProperty(targetNode, propertyEditor.FocusedPropertyName);
					}
				}
			}
		}
		private ModelPropertyEditorCellProperty GetCellProperty(object[] selectedObjects, string propertyName) {
			ModelPropertyEditorCellProperty result = ModelPropertyEditorCellProperty.Default;
			if(selectedObjects == null || selectedObjects.Length == 0 || string.IsNullOrEmpty(propertyName)) {
				return result;
			}
			ModelNode modelNode = (ModelNode)selectedObjects[0];
			if(IsCalculatedProperty(modelNode, propertyName)) {
				if(IsRequired(modelNode, propertyName) || IsKeyProperty(modelNode, propertyName)) {
					SetCellProperty(ModelPropertyEditorCellProperty.CalculateProperty | ModelPropertyEditorCellProperty.Required, ref result);
				}
				SetCellProperty(ModelPropertyEditorCellProperty.CalculateProperty, ref result);
			}
			if(IsRequired(modelNode, propertyName) || IsKeyProperty(modelNode, propertyName)) {
				SetCellProperty(ModelPropertyEditorCellProperty.Required, ref result);
			}
			if(IsRefProperty(modelNode, propertyName, selectedObjects)) {
				SetCellProperty(ModelPropertyEditorCellProperty.RefProperty, ref result);
			}
			if(!IsCellDefaultValue(modelNode, propertyName)) {
				SetCellProperty(ModelPropertyEditorCellProperty.ModifiedProperty, ref result);
			}
			if(IsReadOnly(modelNode, propertyName)) {
				SetCellProperty(ModelPropertyEditorCellProperty.ReadOnly, ref result);
			}
			return result;
		}
		private void HeaderCellClick(ModelNode modelNode, string propertyName) {
			if(NavigateToRefProperty != null) {
				string propertyToSelect;
				ModelNode nodeToNavigate = RefValue(modelNode, propertyName, out propertyToSelect);
				if(nodeToNavigate != null) {
					NavigateToRefProperty(this, new NavigateToEventArgs(nodeToNavigate, propertyToSelect));
				}
			}
		}
		protected virtual void ResetProperty(ModelNode editedModelNode, string propertyName) {
			if(editedModelNode == null || string.IsNullOrEmpty(propertyName)) {
				return;
			}
			if(!IsCellDefaultValue(editedModelNode, propertyName) && CanModified(propertyName)) {
				editedModelNode.ClearValue(propertyName);
			}
		}
		private bool IsCellDefaultValue(ModelNode editedModelNode, string propertyName) {
			if(editedModelNode == null || string.IsNullOrEmpty(propertyName)) {
				return true;
			}
			else {
				return !editedModelNode.IsValueModified(propertyName);
			}
		}
		private ModelNode CalculateDataSourceNode(ModelNode modelNode, string persistentPath, out string dataSourcePropertyName) {
			int index = persistentPath.LastIndexOf('.');
			dataSourcePropertyName = persistentPath;
			if(index >= 0) {
				string pathToDataSourceNode = persistentPath.Remove(index);
				dataSourcePropertyName = persistentPath.Substring(index + 1);
				return ModelNodePersistentPathHelper.FindValueByPath(modelNode, pathToDataSourceNode) as ModelNode;
			}
			return modelNode;
		}
		public bool IsPropertyVisible(ModelNode node, string propertyName) {
			if(node == null) {
				return false;
			}
			if(string.IsNullOrEmpty(propertyName)) {
				return true;
			}
			return CalculatePropertyVisible(node, propertyName);
		}
		public bool IsCalculatedProperty(ModelNode node, string propertyName) {
			string propertyToSelect;
			return RefValue(node, propertyName, out propertyToSelect) != null;
		}
		public bool IsRefProperty(ModelNode node, string propertyName, object[] selectedObjects) {
			if(selectedObjects.Length > 1) {
				List<string> sorsesId = new List<string>();
				foreach(Object obj in selectedObjects) {
					ModelNode sourceNode = RefNode((ModelNode)obj, propertyName);
					if(sourceNode == null) {
						return false;
					}
					if(sorsesId.Count == 0) {
						sorsesId.Add(sourceNode.Id);
					}
					else {
						if(!sorsesId.Contains(sourceNode.Id)) {
							return false;
						}
					}
				}
				return true;
			}
			else {
				return RefNode(node, propertyName) != null;
			}
		}
		public ModelNode RefNode(ModelNode node, string propertyName) {
			if(node != null && !string.IsNullOrEmpty(propertyName)) {
				return node.GetValue(propertyName) as ModelNode;
			}
			return null;
		}
		public bool IsReadOnly(ModelNode node, string propertyName) {
			bool result = false;
			if(node != null) {
				if(node.KeyValueName == propertyName || propertyName == ModelValueNames.Id) {
					result = !node.IsNewValueModified;
				}
				else {
					result = GetReadOnlyForAllowEditAttribute(node, propertyName);
				}
				result |= fastModelEditorHelper.IsReadOnly(node, propertyName);
			}
			return result;
		}
		public bool IsKeyProperty(ModelNode modelNode, string propertyName) {
			if(modelNode == null || string.IsNullOrEmpty(propertyName)) {
				return false;
			}
			return modelNode.KeyValueName == propertyName;
		}
		public Boolean IsRequired(ModelNode modelNode, String propertyName) {
			if(modelNode == null || String.IsNullOrEmpty(propertyName)) {
				return false;
			}
			return fastModelEditorHelper.IsRequired(modelNode, propertyName);
		}
		public bool CanModified() {
			return CanModified("");
		}
		public bool CanModified(string propertyName) {
			if(PropertyChanging != null) {
				PropertyChangingEventArgs args = new PropertyChangingEventArgs(propertyName);
				PropertyChanging(this, args);
				return !args.Cancel;
			}
			return true;
		}
		public IEnumerable CreateSortedValuesList(IEnumerable valuesList) {
			if(valuesList is ModelNode) {
				List<ModelNode> sortedValueList = new List<ModelNode>();
				foreach(object value in valuesList) {
					sortedValueList.Add((ModelNode)value);
				}
				sortedValueList.Sort(new ModelTreeListNodeComparer(fastModelEditorHelper, false));
				return sortedValueList;
			}
			else {
				return valuesList;
			}
		}
		public RepositoryItem FillRepositoryItem(RepositoryItem repositoryItem, ModelNode modelNode, string propertyName) {
			if(modelNode != null && !string.IsNullOrEmpty(propertyName)) {
				ModelValueInfo valueInfo = modelNode.NodeInfo.GetValueInfo(propertyName);
				repositoryItem = CreatePrefferedLanguageRepositoryItem(repositoryItem, modelNode, propertyName);
				repositoryItem = CreatePropertyNameRepositoryItem(repositoryItem, modelNode, propertyName);
				if(valueInfo != null) {
					if(!string.IsNullOrEmpty(valueInfo.PersistentPath)) {
						IEnumerable valuesList = null;
						Type elementType = null;
						if(valueInfo.PersistentPath == "this") {
							valuesList = modelNode as IEnumerable;
						}
						else {
							string dataSourcePropertyName;
							ModelNode dataSourceNode = CalculateDataSourceNode(modelNode, valueInfo.PersistentPath, out dataSourcePropertyName);
							if(dataSourceNode != null) {
								ITypeInfo nodeTypeInfo = XafTypesInfo.Instance.FindTypeInfo(dataSourceNode.GetType());
								IMemberInfo memberInfo = nodeTypeInfo.FindMember(dataSourcePropertyName);
								if(memberInfo != null && memberInfo.IsList) {
									elementType = memberInfo.ListElementType;
									valuesList = memberInfo.GetValue(dataSourceNode) as IEnumerable;
								}
							}
						}
						if(valuesList != null) {
							IEnumerable sortedValuesList = CreateSortedValuesList(valuesList);
							repositoryItem = CreateDataSourcePropertyRepositoryItem(sortedValuesList, modelNode, propertyName, elementType);
						}
					}
					else {
						if(typeof(IModelNode).IsAssignableFrom(valueInfo.PropertyType)) {
							object refNodeValue = modelNode.GetValue(propertyName);
							if(refNodeValue != null) {
								RepositoryItemModelNodesLookup comboBox = new RepositoryItemModelNodesLookup();
								comboBox.Items.Add(refNodeValue);
								repositoryItem = comboBox;
							}
						}
					}
				}
			}
			return repositoryItem;
		}
		public List<string> AspectNames(IModelApplication modelApplication) {
			List<string> result = new List<string>(((ModelApplicationBase)modelApplication).GetAspectNames());
			result.Sort();
			result.Insert(0, CaptionHelper.DefaultLanguage);
			result.Insert(1, CaptionHelper.UserLanguage);
			return result;
		}
		public ExpressionEvaluator GetExpressionEvaluator(Type type, CriteriaOperator criteria) {
			EvaluatorContextDescriptorDefault evaluatorContextDescriptor = new EvaluatorContextDescriptorDefault(type);
			return new ExpressionEvaluator(evaluatorContextDescriptor, criteria, false, null);
		}
		public event EventHandler<PropertyChangingEventArgs> PropertyChanging;
		public event EventHandler<PropertyChangingEventArgs> PropertyChanged;
		public event EventHandler<NavigateToEventArgs> NavigateToRefProperty;
#if DebugTest
		public ModelPropertyEditorCellProperty DebugTest_GetCellProperty(ModelNode selectedObject, string propertyName) {
			return GetCellProperty(new object[] { selectedObject }, propertyName);
		}
		public ModelPropertyEditorCellProperty DebugTest_GetCellProperty(object[] selectedObjects, string propertyName) {
			return GetCellProperty(selectedObjects, propertyName);
		}
		public bool DebugTest_IsCellDefaultValue(ModelNode editedModelNode, string propertyName) {
			return IsCellDefaultValue(editedModelNode, propertyName);
		}
		public ModelNode DebugTest_CalculateDataSourceNode(ModelNode modelNode, string persistentPath, out string dataSourcePropertyName) {
			return CalculateDataSourceNode(modelNode, persistentPath, out dataSourcePropertyName);
		}
		public ModelNode DebugTest_RefValue(ModelNode sourceNode, string propertyName, out string propertyToSelect) {
			return RefValue(sourceNode, propertyName, out propertyToSelect);
		}
		public PropertyDescriptorCollection DebugTest_CollectCustomPropertyDescriptors(object source, PropertyDescriptor propertyDescriptor, PropertyDescriptorCollection properties) {
			return CollectCustomPropertyDescriptors(source, propertyDescriptor, properties);
		}
#endif
	}
	public class PropertyChangingEventArgs : CancelEventArgs {
		private string propertyName;
		public PropertyChangingEventArgs(string propertyName) {
			this.propertyName = propertyName;
		}
		public string PropertyName {
			get { return propertyName; }
		}
	}
	public class NavigateToEventArgs : ModelNodeEventArgs {
		public NavigateToEventArgs(ModelNode node, string propertyName)
			: base(node, propertyName) {
		}
	}
	public class ModelNodeEventArgs : CancelEventArgs {
		private ModelNode node;
		private string propertyName;
		public ModelNodeEventArgs(ModelNode node, string propertyName) {
			this.node = node;
			this.propertyName = propertyName;
		}
		public ModelNode Node {
			get { return node; }
		}
		public string PropertyName {
			get { return propertyName; }
		}
	}
	public class CustomFillRepositoryItemEventArgs : ModelNodeEventArgs {
		private RepositoryItem repositoryItem;
		public CustomFillRepositoryItemEventArgs(ModelNode node, string propertyName, RepositoryItem repositoryItem)
			: base(node, propertyName) {
			this.repositoryItem = repositoryItem;
		}
		public RepositoryItem RepositoryItem {
			get { return repositoryItem; }
			set { repositoryItem = value; }
		}
	}
	public class CalculateCellPropertyEventArgs : EventArgs {
		private ModelPropertyEditorCellProperty cellStyle;
		private Object[] selectedObjects;
		private string propertyName;
		public CalculateCellPropertyEventArgs(Object[] selectedObjects, string propertyName) {
			this.selectedObjects = selectedObjects;
			this.propertyName = propertyName;
		}
		public Object[] SelectedObjects {
			get { return selectedObjects; }
		}
		public string PropertyName {
			get { return propertyName; }
		}
		public ModelPropertyEditorCellProperty CellStyle {
			get { return cellStyle; }
			set { cellStyle = value; }
		}
	}
	public enum ModelPropertyEditorAction { ResetProperty, NavigateToNode, NavigateToCalculatedProperty }
	public class ModelPropertyEditorActionExecuteEventArgs : EventArgs {
		ModelPropertyEditorAction action;
		public ModelPropertyEditorActionExecuteEventArgs(ModelPropertyEditorAction action) {
			this.action = action;
		}
		public ModelPropertyEditorAction Action {
			get {
				return action;
			}
		}
	}
	public class CalculateItemImageIndexEventArgs : ModelNodeEventArgs {
		private int imageIndex = -1;
		public CalculateItemImageIndexEventArgs(ModelNode node, string propertyName) : base(node, propertyName) { }
		public int ImageIndex {
			get { return imageIndex; }
			set { imageIndex = value; }
		}
	}
}
