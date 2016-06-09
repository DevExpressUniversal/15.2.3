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
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Editors {
	public interface IEditorsFactory {
		ListEditor CreateListEditor(IModelListView modelListView, XafApplication application, CollectionSourceBase collectionSource);
		ViewItem CreateDetailViewEditor(bool needProtectedContent, IModelViewItem modelViewItem, Type objectType, XafApplication application, IObjectSpace objectSpace);
		ViewItem CreateDetailViewEditor(bool needProtectedContent, IModelMemberViewItem modelViewItem, Type objectType, XafApplication application, IObjectSpace objectSpace);
		ViewItem CreatePropertyEditorByType(Type editorType, IModelMemberViewItem modelViewItem, Type objectType, XafApplication application, IObjectSpace objectSpace);
	}
	public class EditorsFactory : IEditorsFactory {
		public static string ProtectedContentDefaultText = "Protected Content";
		private static ClassEditorInfoCalculator listViewEditorCalculator = new ClassEditorInfoCalculator();
		private static MemberEditorInfoCalculator detailViewEditorCalculator = new MemberEditorInfoCalculator();
		private ViewItem CreateViewItem(Type viewItemType, String viewItemId, bool checkConstructor, Type objectType, IModelNode modelViewItem) {
			if(viewItemType == null) {
				throw new InvalidOperationException("Cannot create editor for item: " + viewItemId);
			}
			Object[] args = new Object[] { objectType, modelViewItem };
			if(checkConstructor && viewItemType.GetConstructor(new Type[] { typeof(Type), modelViewItem.GetType() }) == null) {
				args = new Object[] { modelViewItem, objectType };
			}
			return (ViewItem)TypeHelper.CreateInstance(viewItemType, args);
		}
		private void SetupViewItem(ViewItem viewItem, IModelNode model, XafApplication application, IObjectSpace objectSpace) {
			if(viewItem is IComplexViewItem) {
				((IComplexViewItem)viewItem).Setup(objectSpace, application);
			}
			if(viewItem is IProtectedContentEditor) {
				((IProtectedContentEditor)viewItem).ProtectedContentText = model.Application.ProtectedContentText;
			}
		}
		public ListEditor CreateListEditor(IModelListView modelListView, XafApplication application, CollectionSourceBase collectionSource) {
			Guard.ArgumentNotNull(modelListView, "modelListView");
			Type editorType = modelListView.EditorType;
			if (editorType == null) {
				editorType = listViewEditorCalculator.GetEditorType(modelListView.ModelClass);
			}
			if (editorType != null) {
				ListEditor result = (ListEditor)Activator.CreateInstance(editorType, modelListView);
				if (result is IComplexListEditor) {
					((IComplexListEditor)result).Setup(collectionSource, application);
				}
				if (result is IProtectedContentEditor) {
					((IProtectedContentEditor)result).ProtectedContentText = modelListView.Application.ProtectedContentText;
				}
				return result;
			}
			else {
				throw new Exception("Cannot create ListEditor for view: " + modelListView.Id);
			}
		}
		public ViewItem CreateDetailViewEditor(bool needProtectedContent, IModelViewItem modelViewItem, Type objectType, XafApplication application, IObjectSpace objectSpace) {
			ViewItem result = null;
			if(modelViewItem is IModelMemberViewItem) {
				result = CreateDetailViewEditor(needProtectedContent, (IModelMemberViewItem)modelViewItem, objectType, application, objectSpace);
			}
			else {
				Type editorType = detailViewEditorCalculator.GetEditorType(modelViewItem);
				result = CreateViewItem(editorType, modelViewItem.Id, true, objectType, modelViewItem);
				SetupViewItem(result, modelViewItem, application, objectSpace);
			}
			return result;
		}
		public ViewItem CreateDetailViewEditor(bool needProtectedContent, IModelMemberViewItem modelViewItem, Type objectType, XafApplication application, IObjectSpace objectSpace) {
			Guard.ArgumentNotNull(modelViewItem, "modelViewItem");
			Type editorType = null;
			if (needProtectedContent) {
				editorType = modelViewItem.Application.ViewItems.PropertyEditors.ProtectedContentPropertyEditor;
			}
			else {
				editorType = modelViewItem.PropertyEditorType;
			}
			return CreatePropertyEditorByType(editorType, modelViewItem, objectType, application, objectSpace);
		}
		public ViewItem CreatePropertyEditorByType(Type editorType, IModelMemberViewItem modelViewItem, Type objectType, XafApplication application, IObjectSpace objectSpace) {
			ViewItem result = CreateViewItem(editorType, modelViewItem.Id, false, objectType, modelViewItem);
			SetupViewItem(result, modelViewItem, application, objectSpace);
			return result;
		}
	}
	public abstract class EditorRegistration {
		public EditorRegistration(string aliasName, Type elementType) {
			Alias = aliasName;
			ElementType = elementType;
		}
		public string Alias { get; private set; }
		public Type ElementType { get; private set; }
	}
	public delegate bool IsMemberCompatibleHandler(IModelMember modelMember);
	public delegate bool IsClassCompatibleHandler(IModelClass modelClass);
	public class AliasRegistration : EditorRegistration, IAliasRegistration {
		bool isDefaultAlias;
		IsMemberCompatibleHandler memberCompatibleHandler;
		IsClassCompatibleHandler classCompatibleHandler;
		bool hasCustomizableDefaultEditorType = true;
		public AliasRegistration(string aliasName, Type elementType, bool isDefaultAlias)
			: base(aliasName, elementType) {
			this.isDefaultAlias = isDefaultAlias;
		}
		public AliasRegistration(string aliasName, Type elementType, bool hasCustomizableDefaultEditorType, IsMemberCompatibleHandler memberHandler)
			: this(aliasName, elementType, false) {
			this.hasCustomizableDefaultEditorType = hasCustomizableDefaultEditorType;
			this.memberCompatibleHandler = memberHandler;
		}
		public AliasRegistration(string aliasName, Type elementType, IsMemberCompatibleHandler memberHandler)
			: this(aliasName, elementType, true, memberHandler) {
		}
		public AliasRegistration(string aliasName, Type elementType, IsClassCompatibleHandler classHandler)
			: this(aliasName, elementType, false) {
			this.classCompatibleHandler = classHandler;
		}
		public bool IsDefaultAlias {
			get {
				return isDefaultAlias;
			}
		}
		public override string ToString() {
			return "ElementType: " + ElementType.ToString() + " IsDefaultAlias: " + IsDefaultAlias.ToString();
		}
		public bool? IsCompatible(IModelNode modelNode) {
			if (modelNode is IModelClass && classCompatibleHandler != null) {
				return classCompatibleHandler((IModelClass)modelNode);
			}
			else if (modelNode is IModelMember && memberCompatibleHandler != null) {
				return memberCompatibleHandler((IModelMember)modelNode);
			}
			return null;
		}
		public bool HasCompatibleDelegate {
			get {
				return (classCompatibleHandler != null) || memberCompatibleHandler != null;
			}
		}
		public bool HasCustomizableDefaultEditorType {
			get { return hasCustomizableDefaultEditorType; }
		}
	}
	public interface IEditorTypeRegistration {
		string Alias { get; }
		Type EditorType { get; }
		Type ElementType { get; }
		bool IsDefaultEditor { get; }
	}
	public class EditorTypeRegistration : EditorRegistration, IEditorTypeRegistration {
		bool isDefaultEditor;
		Type editorType;
		public EditorTypeRegistration(string aliasName, Type elementType, Type editorType, bool isDefaultEditor)
			: base(aliasName, elementType) {
			this.editorType = editorType;
			this.isDefaultEditor = isDefaultEditor;
		}
		public bool IsDefaultEditor {
			get {
				return isDefaultEditor;
			}
		}
		public Type EditorType {
			get {
				return editorType;
			}
		}
		public override string ToString() {
			return "EditorType: " + EditorType.ToString() + " IsDefaultEditor: " + IsDefaultEditor.ToString();
		}
	}
	public class AliasAndEditorTypeRegistration : EditorTypeRegistration, IAliasRegistration {
		AliasRegistration aliasRegistration;
		public AliasAndEditorTypeRegistration(string aliasName, Type elementType, bool isDefaultAlias, Type editorType, bool isDefaultEditor)
			: base(aliasName, elementType, editorType, isDefaultEditor) {
			aliasRegistration = new AliasRegistration(aliasName, elementType, isDefaultAlias);
		}
		public AliasAndEditorTypeRegistration(string aliasName, Type elementType, Type editorType, bool isDefaultEditor,
			IsMemberCompatibleHandler memberHandler)
			: base(aliasName, elementType, editorType, isDefaultEditor) {
			aliasRegistration = new AliasRegistration(aliasName, elementType, true, memberHandler);
		}
		public AliasAndEditorTypeRegistration(string aliasName, Type elementType, Type editorType, bool isDefaultEditor,
			IsClassCompatibleHandler classHandler)
			: base(aliasName, elementType, editorType, isDefaultEditor) {
			aliasRegistration = new AliasRegistration(aliasName, elementType, classHandler);
		}
		#region IAliasRegistration Members
		public bool IsDefaultAlias {
			get {
				return aliasRegistration.IsDefaultAlias;
			}
		}
		public bool HasCompatibleDelegate {
			get { return aliasRegistration.HasCompatibleDelegate; }
		}
		#endregion
		public override string ToString() {
			return " Alias:" + Alias + " Type:" + ElementType.ToString() + " EditorType: " + EditorType.ToString() + " IsDefaultEditor" + IsDefaultEditor;
		}
		public bool? IsCompatible(IModelNode modelNode) {
			return aliasRegistration.IsCompatible(modelNode);
		}
		public bool HasCustomizableDefaultEditorType {
			get { return aliasRegistration.HasCustomizableDefaultEditorType; }
		}
	}
	public abstract class EditorDescriptor {
		public EditorDescriptor(EditorRegistration registrationParams) {
			Guard.ArgumentNotNull(registrationParams, "registrationParams");
			RegistrationParams = registrationParams;
		}
		public EditorRegistration RegistrationParams { get; private set; }
		public override string ToString() {
			return RegistrationParams.ToString();
		}
	}
	public interface IAliasRegistration {
		string Alias { get; }
		bool IsDefaultAlias { get; }
		Type ElementType { get; }
		bool HasCompatibleDelegate { get; }
		bool HasCustomizableDefaultEditorType { get; }
		bool? IsCompatible(IModelNode modelNode);
	}
	public sealed class PropertyEditorDescriptor : EditorDescriptor {
		public PropertyEditorDescriptor(EditorRegistration registrationParam) : base(registrationParam) { }
	}
	public sealed class ListEditorDescriptor : EditorDescriptor {
		public ListEditorDescriptor(EditorRegistration registrationParam) : base(registrationParam) { }
	}
	public class ViewItemRegistration : EditorRegistration, IAliasRegistration, IEditorTypeRegistration {
		private Type detailViewItemType;
		private bool isDefaultEditor;
		public ViewItemRegistration(Type modelNodeType)
			: base(modelNodeType.Name.Replace("IModel", ""), modelNodeType) {
		}
		public ViewItemRegistration(Type modelNodeType, Type detailViewItemType, bool isDefault)
			: base(modelNodeType.Name.Replace("IModel", ""), modelNodeType) {
			this.detailViewItemType = detailViewItemType;
			this.isDefaultEditor = isDefault;
		}
		#region IAliasRegistration Members
		public bool IsDefaultAlias {
			get { return true; }
		}
		public bool? IsCompatible(IModelNode modelNode) {
			return true;
		}
		public bool HasCompatibleDelegate {
			get { return false; }
		}
		public bool HasCustomizableDefaultEditorType {
			get {
				return true;
			}
		}
		#endregion
		#region IEditorTypeRegistration Members
		public Type EditorType {
			get { return detailViewItemType; }
		}
		public bool IsDefaultEditor {
			get { return isDefaultEditor; }
		}
		#endregion
	}
	public class ViewItemDescriptor : EditorDescriptor {
		public ViewItemDescriptor(ViewItemRegistration registrationParam) : base(registrationParam) { }
		public new ViewItemRegistration RegistrationParams {
			get { return (ViewItemRegistration)base.RegistrationParams; }
		}
	}
	public class ModelRegisteredViewItemsGenerator : ModelNodesGeneratorBase {
		private void GenerateViewItemsRegistration(EditorDescriptors editorDescriptors, IModelRegisteredViewItems modelViewItems) {
			foreach (ViewItemDescriptor viewItem in editorDescriptors.ViewItems) {
				ViewItemRegistration viewItemRegistration = viewItem.RegistrationParams;
				if (viewItemRegistration.EditorType != null) {
					IModelRegisteredViewItem modelViewItem = modelViewItems[viewItemRegistration.Alias];
					if (modelViewItem == null) {
						modelViewItem = modelViewItems.AddNode<IModelRegisteredViewItem>(viewItemRegistration.Alias);
						modelViewItem.SetValue<List<Type>>("RegisteredTypes", new List<Type>());
					}
					modelViewItem.RegisteredTypes.Add(viewItemRegistration.EditorType);
					if (viewItemRegistration.IsDefaultEditor) {
						modelViewItem.DefaultItemType = viewItemRegistration.EditorType;
					}
				}
			}
		}
		private static IModelRegisteredPropertyEditor GetPropertyEditorRegistration(IModelRegisteredPropertyEditors modelPropertyEditors, IEditorTypeRegistration propertyEditor, string Id) {
			IModelRegisteredPropertyEditor modelPropertyEditor = modelPropertyEditors[Id];
			if (modelPropertyEditor == null) {
				modelPropertyEditor = modelPropertyEditors.AddNode<IModelRegisteredPropertyEditor>(Id);
				modelPropertyEditor.SetValue<List<Type>>("RegisteredTypes", new List<Type>());
				modelPropertyEditor.DefaultDisplayFormat = FormattingProvider.GetDisplayFormat(propertyEditor.ElementType);
				modelPropertyEditor.DefaultEditMask = FormattingProvider.GetEditMask(propertyEditor.ElementType);
				modelPropertyEditor.DefaultEditMaskType = FormattingProvider.GetEditMaskType(propertyEditor.ElementType);
			}
			return modelPropertyEditor;
		}
		internal static bool GetIsDefaultEditor(IAliasRegistration aliasRegistration) {
			if (aliasRegistration != null) {
				return aliasRegistration.ElementType == typeof(Object) && (aliasRegistration.IsDefaultAlias || !aliasRegistration.HasCompatibleDelegate);
			}
			return false;
		}
		private void GeneratePropertyEditorsRegistation(EditorDescriptors editorDescriptors, IModelRegisteredViewItems modelViewItems) {
			IModelRegisteredPropertyEditors modelPropertyEditors = modelViewItems.PropertyEditors;
			foreach (IEditorTypeRegistration propertyEditor in editorDescriptors.PropertyEditorRegistrations) {
				IAliasRegistration aliasRegistration = editorDescriptors.PropertyEditorRegistrations.GetAliasRegistration(propertyEditor);
				IModelRegisteredPropertyEditor modelPropertyEditor = null;
				bool isDefaultEditor = GetIsDefaultEditor(aliasRegistration);
				bool isProptectedContentEditor = isDefaultEditor && typeof(IProtectedContentEditor).IsAssignableFrom(propertyEditor.EditorType);
				if ((aliasRegistration.IsDefaultAlias)) {
					if (!isDefaultEditor) {
						modelPropertyEditor = GetPropertyEditorRegistration(modelPropertyEditors, propertyEditor, propertyEditor.ElementType.FullName);
						if (propertyEditor.IsDefaultEditor) {
							modelPropertyEditor.EditorType = propertyEditor.EditorType;
						}
					}
				}
				else {
					if (aliasRegistration.HasCompatibleDelegate && aliasRegistration.HasCustomizableDefaultEditorType) {
						modelPropertyEditor = GetPropertyEditorRegistration(modelPropertyEditors, propertyEditor, aliasRegistration.Alias);
						if (propertyEditor.IsDefaultEditor) {
							modelPropertyEditor.EditorType = propertyEditor.EditorType;
						}
					}
				}
				if (modelPropertyEditor != null && modelPropertyEditor.RegisteredTypes.IndexOf(propertyEditor.EditorType) == -1) {
					modelPropertyEditor.RegisteredTypes.Add(propertyEditor.EditorType);
				}
				if (isDefaultEditor) {
					if (propertyEditor.IsDefaultEditor && (aliasRegistration.IsDefaultAlias)) {
						modelPropertyEditors.DefaultItemType = propertyEditor.EditorType;
					}
					if (modelPropertyEditors.RegisteredTypes == null) {
						modelPropertyEditors.SetValue<List<Type>>("RegisteredTypes", new List<Type>());
					}
					modelPropertyEditors.RegisteredTypes.Add(propertyEditor.EditorType);
				}
				if (isProptectedContentEditor) {
					if (propertyEditor.IsDefaultEditor) {
						modelPropertyEditors.ProtectedContentPropertyEditor = propertyEditor.EditorType;
					}
					if (modelPropertyEditors.ProtectedContentRegisteredTypes == null) {
						modelPropertyEditors.SetValue<List<Type>>("ProtectedContentRegisteredTypes", new List<Type>());
					}
					modelPropertyEditors.ProtectedContentRegisteredTypes.Add(propertyEditor.EditorType);
				}
			}
			for (int i = modelPropertyEditors.Count - 1; i >= 0; i--) {
				IModelRegisteredPropertyEditor propertyEditorRegistation = modelPropertyEditors[i];
				if (propertyEditorRegistation.EditorType == null) {
					propertyEditorRegistation.Remove();
				}
			}
		}
		protected override void GenerateNodesCore(ModelNode node) {
			EditorDescriptors editorDescriptors = ((IModelSources)node.Application).EditorDescriptors;
			IModelRegisteredViewItems modelDetailViewItems = (IModelRegisteredViewItems)node;
			GenerateViewItemsRegistration(editorDescriptors, modelDetailViewItems);
			GeneratePropertyEditorsRegistation(editorDescriptors, modelDetailViewItems);
		}
	}
}
