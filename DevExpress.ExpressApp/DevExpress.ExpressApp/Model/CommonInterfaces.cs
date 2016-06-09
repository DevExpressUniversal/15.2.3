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
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Model {
	[ImageName("ModelEditor_Application")]
	[DisplayProperty("Title")]
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelApplication")]
#endif
	public interface IModelApplication : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationTitle"),
#endif
 Localizable(true)]
		string Title { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationCompany"),
#endif
 Localizable(true)]
		string Company { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationCopyright"),
#endif
 Localizable(true)]
		string Copyright { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationLogo")]
#endif
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		string Logo { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationDescription"),
#endif
 Localizable(true)]
		string Description { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationVersionFormat"),
#endif
 Localizable(true)]
		string VersionFormat { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationPreferredLanguage"),
#endif
 DefaultValue(CaptionHelper.UserLanguage)]
		string PreferredLanguage { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationProtectedContentText"),
#endif
 Localizable(true), DefaultValue("Protected Content")]
		string ProtectedContentText { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelApplicationAboutInfoString"),
#endif
 Localizable(true), DefaultValue("{0:ProductName}<br>{0:Version}<br>{0:Copyright}<br>{0:Description}")]
		string AboutInfoString { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationOptions")]
#endif
		IModelOptions Options { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationLocalization")]
#endif
		IModelLocalization Localization { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationBOModel")]
#endif
		IModelBOModel BOModel { get; }
		[Browsable(false)]
		IModelSchemaModules SchemaModules { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationImageSources")]
#endif
		IModelImageSources ImageSources { get; }
		[Browsable(false)]
		IModelTemplates Templates { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationActionDesign")]
#endif
		IModelActionDesign ActionDesign { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationViews")]
#endif
		IModelViews Views { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationViewItems")]
#endif
		IModelRegisteredViewItems ViewItems { get; }
	}
	[ImageName("ModelEditor_Settings")]
	[ModelNodesGenerator(typeof(ModelOptionsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelOptions")]
#endif
	public interface IModelOptions : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelOptionsLookupSmallCollectionItemCount"),
#endif
 Category("Behavior"), DefaultValue(25)]
		int LookupSmallCollectionItemCount { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelOptionsDataAccessMode"),
#endif
 Category("Behavior"), DefaultValue(CollectionSourceDataAccessMode.Client)]
		[RefreshProperties(RefreshProperties.All)]
		CollectionSourceDataAccessMode DataAccessMode { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelOptionsUseServerMode"),
#endif
 Category("Behavior"), DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		Boolean UseServerMode { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelOptionsLayoutManagerOptions")]
#endif
		IModelLayoutManagerOptions LayoutManagerOptions { get; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelLayoutManagerOptions")]
#endif
	public interface IModelLayoutManagerOptions : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutManagerOptionsCaptionColon"),
#endif
 Category("Layout")]
		[DefaultValue(DevExpress.ExpressApp.Layout.LayoutManager.DefaultCaptionColon), Localizable(true)]
		string CaptionColon { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutManagerOptionsEnableCaptionColon"),
#endif
 Category("Layout")]
		[DefaultValue(true)]
		bool EnableCaptionColon { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutManagerOptionsCaptionLocation"),
#endif
 Category("Layout")]
		[DefaultValue(Locations.Default)]
		Locations CaptionLocation { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutManagerOptionsCaptionHorizontalAlignment"),
#endif
 DefaultValue(HorzAlignment.Default)]
		[Category("Layout")]
		HorzAlignment CaptionHorizontalAlignment { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutManagerOptionsCaptionVerticalAlignment"),
#endif
 Category("Layout")]
		[DefaultValue(VertAlignment.Default)]
		VertAlignment CaptionVerticalAlignment { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLayoutManagerOptionsCaptionWordWrap"),
#endif
 Category("Layout")]
		[DefaultValue(WordWrap.Default)]
		WordWrap CaptionWordWrap { get; set; }
	}
	[ImageName("BO_Localization")]
	[ModelNodesGenerator(typeof(ModelLocalizationNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelLocalization")]
#endif
	public interface IModelLocalization : IModelNode, IModelList<IModelLocalizationGroup> {
	}
	[ModelAbstractClass]
	[KeyProperty("Name")]
	public interface IModelLocalizationItemBase : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLocalizationItemBaseName"),
#endif
 Required()]
		string Name { get; set; }
	}
	[ModelNodesGenerator(typeof(ModelLocalizationGroupGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelLocalizationGroup")]
#endif
	public interface IModelLocalizationGroup : IModelLocalizationItemBase, IModelList<IModelLocalizationItemBase> {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLocalizationGroupValue"),
#endif
 Category("Data")]
		[Localizable(true)]
		[ModelBrowsable(typeof(EnumTypeNameLocalizationOnlyCalculator))]
		string Value { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelLocalizationItem")]
#endif
	public interface IModelLocalizationItem : IModelLocalizationItemBase {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelLocalizationItemValue"),
#endif
 Category("Data")]
		[Localizable(true)]
		string Value { get; set; }
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ImageName("ModelEditor_Business_Object_Model")]
	[ModelNodesGenerator(typeof(ModelBOModelClassNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelBOModel")]
#endif
	public interface IModelBOModel : IModelNode, IModelList<IModelClass> {
		IModelClass GetClass(Type type);
	}
	[ModelNodesGenerator(typeof(ModelBOModelMemberNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelBOModelClassMembers")]
#endif
	[ModelReadOnly(typeof(ModelClassMembersReadOnlyCalculator))]
	public interface IModelBOModelClassMembers : IModelNode, IModelList<IModelMember> {
	}
	[DisplayProperty("ShortName")]
	[ImageName("ModelEditor_Class_Object")]
	[KeyProperty("Name")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelClass")]
#endif
	public interface IModelClass : IModelNode {
		IModelMember FindMember(string memberName);
		IModelMember FindOwnMember(string memberName);
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IModelList<IModelMember> AllMembers { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelClassOwnMembers")]
#endif
		IModelBOModelClassMembers OwnMembers { get; }
		[ModelBrowsable(typeof(InterfaceTypePropertyOnlyCalculator))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IModelClassInterfaces InterfaceLinks { get; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		ITypeInfo TypeInfo { get; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IEnumerable<Type> ListEditorsType { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassName"),
#endif
 Category("Data")]
		[Required]
		string Name { get; }
		[ReadOnly(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ShortName { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassBaseClass"),
#endif
 Category("Data")]
		IModelClass BaseClass { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassEditorType"),
#endif
 ModelPersistentName("EditorTypeName")]
		[TypeConverter(typeof(StringToTypeConverterBase))]
		[Category("Appearance")]
		[DataSourceProperty("ListEditorsType")]
		Type EditorType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultProperty"),
#endif
 Category("Data")]
		[ReadOnly(true)]
		string DefaultProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassFriendlyKeyProperty"),
#endif
 ModelValueCalculator("BaseClass", "FriendlyKeyProperty")]
		[Category("Data")]
		[ReadOnly(true)]
		string FriendlyKeyProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassObjectCaptionFormat"),
#endif
 Localizable(true)]
		[Category("Format")]
		[ModelValueCalculator("BaseClass", "ObjectCaptionFormat")]
		string ObjectCaptionFormat { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassKeyProperty"),
#endif
 Category("Data")]
		[ModelValueCalculator("BaseClass", "KeyProperty"), ReadOnly(true)]
		string KeyProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassImageName"),
#endif
 Category("Appearance")]
		[ModelValueCalculator("BaseClass", "ImageName")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		string ImageName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultListViewImage"),
#endif
 Category("Appearance")]
		[ModelValueCalculator("this", "ImageName")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		string DefaultListViewImage { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultDetailViewImage"),
#endif
 Category("Appearance")]
		[ModelValueCalculator("this", "ImageName")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		string DefaultDetailViewImage { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultListViewAllowEdit"),
#endif
 Category("Behavior")]
		[DefaultValue(false)]
		bool DefaultListViewAllowEdit { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultListViewMasterDetailMode"),
#endif
 DefaultValue(MasterDetailMode.ListViewOnly)]
		[Category("Behavior")]
		DevExpress.ExpressApp.MasterDetailMode DefaultListViewMasterDetailMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassIsCreatableItem"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		bool IsCreatableItem { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultLookupEditorMode"),
#endif
 Category("Behavior")]
		DevExpress.Persistent.Base.LookupEditorMode DefaultLookupEditorMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultListView"),
#endif
 DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[DataSourceCriteria(ModelClassLogic.ModelViewsByClassCriteria)]
		[Category("Appearance")]
		IModelListView DefaultListView { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultLookupListView"),
#endif
 DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[DataSourceCriteria(ModelClassLogic.ModelViewsByClassCriteria)]
		[Category("Appearance")]
		IModelListView DefaultLookupListView { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassDefaultDetailView"),
#endif
 DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[DataSourceCriteria(ModelClassLogic.ModelViewsByClassCriteria)]
		[Category("Appearance")]
		IModelDetailView DefaultDetailView { get; set; }
	}
	[ImageName("ModelEditor_Member")]
	[KeyProperty("Name")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelMember")]
#endif
	public interface IModelMember : IModelNode, IModelCommonMemberViewItem {
		[Browsable(false)]
		IModelClass ModelClass { get; }
		[Browsable(false)]
		IMemberInfo MemberInfo { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberName"),
#endif
 Required()]
		[Category("Data")]
		String Name { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberType"),
#endif
 Category("Data")]
		[ModelReadOnly(typeof(ModelMemberReadOnlyCalculator))]
		[TypeConverter(typeof(StringToTypeConverterEx))]
		[RefreshProperties(RefreshProperties.All)]
		[Required]
		Type Type { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberSize"),
#endif
 Category("Layout")]
		int Size { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberAllowAdd"),
#endif
 Category("Behavior")]
		[ReadOnly(true)]
		AllowAdd AllowAdd { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberIsCustom"),
#endif
 Category("Behavior")]
		[ModelBrowsable(typeof(ModelMemberVisibilityCalculator))]
		[ReadOnly(true)]
		Boolean IsCustom { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberIsCalculated"),
#endif
 Category("Behavior")]
		[ModelBrowsable(typeof(ModelMemberVisibilityCalculator))]
		[ModelReadOnly(typeof(ModelMemberReadOnlyCalculator))]
		[RefreshProperties(RefreshProperties.All)]
		Boolean IsCalculated { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberExpression"),
#endif
 Category("Data")]
		[ModelBrowsable(typeof(ModelMemberVisibilityCalculator))]
		[ModelReadOnly(typeof(ModelMemberReadOnlyCalculator))]
		[Required(typeof(ModelMemberRequiredCalculator))]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ExpressionModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(UITypeEditor))]
		String Expression { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberDataSourceProperty"),
#endif
 Category("Data")]
		String DataSourceProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberDataSourceCriteriaProperty"),
#endif
 Category("Data")]
		String DataSourceCriteriaProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberIsVisibleInDetailView"),
#endif
 DefaultValue(true)]
		[ReadOnly(true)]
		[Category("Behavior")]
		bool? IsVisibleInDetailView { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberIsVisibleInLookupListView"),
#endif
 DefaultValue(false)]
		[ReadOnly(true)]
		[Category("Behavior")]
		bool? IsVisibleInLookupListView { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberIsVisibleInListView"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		bool? IsVisibleInListView { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberListViewImageEditorMode"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Behavior")]
		[DefaultValue(ImageEditorMode.PictureEdit)]
		ImageEditorMode ListViewImageEditorMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberDetailViewImageEditorMode"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Behavior")]
		[DefaultValue(ImageEditorMode.PictureEdit)]
		ImageEditorMode DetailViewImageEditorMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberListViewImageEditorCustomHeight"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Layout")]
		int ListViewImageEditorCustomHeight { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberDetailViewImageEditorFixedHeight"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Layout")]
		int DetailViewImageEditorFixedHeight { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberDetailViewImageEditorFixedWidth"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Layout")]
		int DetailViewImageEditorFixedWidth { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberGroupInterval"),
#endif
 ModelBrowsable(typeof(DateTimePropertyOnlyCalculator))]
		[Category("Behavior")]
		GroupInterval GroupInterval { get; set; }
		[Category("Data"), Browsable(false)]
		bool AllowNull { get; }
	}
	public interface IModelInterfaceLink : IModelNode {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelInterfaceLinkInterface")]
#endif
		[DefaultValue(null)]
		IModelClass Interface { get; }
	}
	[ModelNodesGenerator(typeof(ModelClassInterfacesNodesGenerator))]
	public interface IModelClassInterfaces : IModelNode, IModelList<IModelInterfaceLink> { }
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelSchemaModules")]
#endif
	public interface IModelSchemaModules : IModelNode, IModelList<IModelSchemaModule> {
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[KeyProperty("Name")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelSchemaModule")]
#endif
	public interface IModelSchemaModule : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelSchemaModuleName"),
#endif
 Required()]
		[ReadOnly(true)]
		[Category("Data")]
		string Name { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelSchemaModuleVersion"),
#endif
 Category("Data")]
		[ReadOnly(true)]
		string Version { get; set; }
	}
	[Browsable(false)]
	[ModelNodesGenerator(typeof(TemplatesModelNodeGenerator))]
	public interface IModelTemplates : IModelNode, IModelList<IModelTemplate> {
	}
	[ModelPersistentName("Template")]
	public interface IModelTemplate : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelTemplateId"),
#endif
 Required()]
		string Id { get; set; }
	}
	public interface IModelTemplateCustomization : IModelNode {
		[Required()]
		string Id { get; set; }
	}
	public interface IModelFormState : IModelTemplateCustomization {
		string State { get; set; }
		string MaximizedOnScreen { get; set; }
		string X { get; set; }
		string Y { get; set; }
		string Width { get; set; }
		string Height { get; set; }
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ImageName("ModelEditor_Actions_ActionDesign")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelActionDesign")]
#endif
	public interface IModelActionDesign : IModelNode {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionDesignActions")]
#endif
		IModelActions Actions { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionDesignControllers")]
#endif
		IModelControllers Controllers { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionDesignDisableReasons")]
#endif
		IModelDisableReasons DisableReasons { get; }
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ImageName("ModelEditor_Actions")]
	[ModelNodesGenerator(typeof(ModelActionsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelActions")]
#endif
	public interface IModelActions : IModelNode, IModelList<IModelAction> {
	}
	[ImageName("ModelEditor_Actions")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelAction")]
#endif
	public interface IModelAction : IModelNode, IModelToolTip {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionShortCaption"),
#endif
 Localizable(true)]
		string ShortCaption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionNullValuePrompt"),
#endif
 Localizable(true)]
		string NullValuePrompt { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionImageName"),
#endif
 Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string ImageName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionSelectionDependencyType"),
#endif
 Category("Behavior")]
		DevExpress.ExpressApp.Actions.SelectionDependencyType SelectionDependencyType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionCategory"),
#endif
 Category("Appearance")]
		[ReadOnly(true)]
		string Category { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionConfirmationMessage"),
#endif
 Localizable(true)]
		string ConfirmationMessage { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionShortcut")]
#endif
		string Shortcut { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionId"),
#endif
 Required()]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionTargetViewType"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		ViewType TargetViewType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionTargetViewNesting"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		Nesting TargetViewNesting { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionTargetObjectType"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		string TargetObjectType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionTargetViewId"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		string TargetViewId { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionTargetObjectsCriteria"),
#endif
 Category("Behavior")]
		[CriteriaOptions("TargetObjectType")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string TargetObjectsCriteria { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionTargetObjectsCriteriaMode"),
#endif
 Category("Behavior")]
		DevExpress.ExpressApp.Actions.TargetObjectsCriteriaMode TargetObjectsCriteriaMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionPaintStyle"),
#endif
 DefaultValue(DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Default)]
		[Category("Appearance")]
		DevExpress.ExpressApp.Templates.ActionItemPaintStyle PaintStyle { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionQuickAccess"),
#endif
 Category("Behavior")]
		bool QuickAccess { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionShowItemsOnClick"),
#endif
 Category("Behavior")]
		bool ShowItemsOnClick { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionCaptionFormat"),
#endif
 DefaultValue("{0}")]
		string CaptionFormat { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionDefaultItemMode"),
#endif
 Category("Behavior")]
		DevExpress.ExpressApp.Actions.DefaultItemMode DefaultItemMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionImageMode"),
#endif
 Category("Appearance")]
		DevExpress.ExpressApp.Actions.ImageMode ImageMode { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionDisableReasons")]
#endif
		IModelDisableReasons DisableReasons { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelActionController"),
#endif
 ReadOnly(true)]
		[DataSourceProperty("Application.ActionDesign.Controllers")]
		[Category("Data")]
		IModelController Controller { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelActionChoiceActionItems")]
#endif
		[OmitDefaultGeneration]
		IModelChoiceActionItems ChoiceActionItems { get; }
	}
	[ModelNodesGenerator(typeof(ModelChoiceActionItemsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelChoiceActionItems")]
#endif
	public interface IModelChoiceActionItems : IModelNode, IModelList<IModelChoiceActionItem> {
	}
	[DisplayProperty("Caption")]
	public interface IModelBaseChoiceActionItem : IModelNode, IModelToolTip {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelBaseChoiceActionItemId"),
#endif
 Required()]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelBaseChoiceActionItemCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelBaseChoiceActionItemImageName"),
#endif
 Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string ImageName { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelBaseChoiceActionItemShortcut")]
#endif
		string Shortcut { get; set; }
	}
	public interface IModelChoiceActionItemChildItemsDisplayStyle {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelChoiceActionItemChildItemsDisplayStyleChildItemsDisplayStyle"),
#endif
 Category("Behavior")]
		DevExpress.ExpressApp.Templates.ActionContainers.ItemsDisplayStyle ChildItemsDisplayStyle { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelChoiceActionItem")]
#endif
	public interface IModelChoiceActionItem : IModelBaseChoiceActionItem {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelChoiceActionItemChoiceActionItems")]
#endif
		[OmitDefaultGeneration] 
		IModelChoiceActionItems ChoiceActionItems { get; }
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ImageName("ModelEditor_Controllers")]
	[ModelNodesGenerator(typeof(ModelControllersNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelControllers")]
#endif
	public interface IModelControllers : IModelNode, IModelList<IModelController> {
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[KeyProperty("Name")]
	public interface IModelController : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelControllerName"),
#endif
 Required()]
		[Category("Data")]
		string Name { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelControllerBaseController"),
#endif
 ReadOnly(true)]
		[DataSourceProperty("Application.ActionDesign.Controllers")]
		[Category("Data")]
		IModelController BaseController { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelControllerActions")]
#endif
		IModelControllerActions Actions { get; }
	}
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[ModelNodesGenerator(typeof(ModelControllerActionsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelControllerActions")]
#endif
	public interface IModelControllerActions : IModelNode, IModelList<IModelActionLink> { }
	[ModelReadOnly(typeof(ModelReadOnlyCalculator))]
	[KeyProperty("Name")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelViewController")]
#endif
	public interface IModelViewController : IModelController {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewControllerTargetViewType"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		DevExpress.ExpressApp.ViewType TargetViewType { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewControllerTargetViewNesting"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		DevExpress.ExpressApp.Nesting TargetViewNesting { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewControllerTargetObjectType"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		string TargetObjectType { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewControllerTargetViewId"),
#endif
 Category("Behavior")]
		[ReadOnly(true)]
		string TargetViewId { get; }
	}
	[ReadOnly(true)]
	[KeyProperty("Name")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelWindowController")]
#endif
	public interface IModelWindowController : IModelController {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelWindowControllerTargetWindowType"),
#endif
 Category("Behavior")]
		WindowType TargetWindowType { get; }
	}
	[ImageName("ModelEditor_Actions_DisableReasons")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelDisableReasons")]
#endif
	public interface IModelDisableReasons : IModelNode, IModelList<IModelReason> {
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelReason")]
#endif
	public interface IModelReason : IModelNode {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelReasonId"),
#endif
 Required()]
		string Id { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelReasonCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
	}
	[ImageName("ModelEditor_Views")]
	[ModelNodesGenerator(typeof(ModelViewsNodesGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ModelIModelViews")]
#endif
	public interface IModelViews : IModelNode, IModelList<IModelView> {
		[Browsable(false)]
		IEnumerable<Type> ListEditorsType { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelViewsDefaultListEditor"),
#endif
 TypeConverter(typeof(StringToTypeConverterBase))]
		[DataSourceProperty("ListEditorsType")]
		[Category("Appearance")]
		Type DefaultListEditor { get; set; }
		IModelDetailView GetDefaultDetailView<T>();
		IModelListView GetDefaultListView<T>();
	}
	public interface IModelToolTip {
		[Editor(DevExpress.ExpressApp.Utils.Constants.MultilineStringEditorType, typeof(System.Drawing.Design.UITypeEditor))]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelToolTipToolTip"),
#endif
 Localizable(true)]
		[DefaultValue("")]
		string ToolTip { get; set; }
	}
	public interface IModelToolTipOptions {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelToolTipOptionsToolTipTitle"),
#endif
 Localizable(true)]
		[DefaultValue("")]
		string ToolTipTitle { get; set; }
		[Category("Behavior")]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelToolTipOptionsToolTipIconType"),
#endif
 DefaultValue(ToolTipIconType.None)]
		ToolTipIconType ToolTipIconType { get; set; }
	}
	public interface IModelCommonMemberViewItem : IModelToolTip {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemEditMask"),
#endif
 Category("Format"), Localizable(true)]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.MaskModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string EditMask { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemEditMaskType"),
#endif
 ModelBrowsable(typeof(EditMaskTypeVisibilityCalculator))]
		[Category("Format")]
		DevExpress.ExpressApp.Editors.EditMaskType EditMaskType { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemIsPassword"),
#endif
 Category("Behavior")]
		bool IsPassword { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemDisplayFormat"),
#endif
 Category("Format"), Localizable(true)]
		string DisplayFormat { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemCaption"),
#endif
 Localizable(true)]
		string Caption { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemRowCount"),
#endif
 ModelBrowsable(typeof(StringPropertyOnlyCalculator))]
		[Category("Layout")]
		int RowCount { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemAllowEdit"),
#endif
 Category("Behavior")]
		bool AllowEdit { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemLookupProperty"),
#endif
 Category("Data")]
		string LookupProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemDataSourcePropertyIsNullMode"),
#endif
 Category("Data")]
		DevExpress.Persistent.Base.DataSourcePropertyIsNullMode DataSourcePropertyIsNullMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemDataSourcePropertyIsNullCriteria"),
#endif
 Category("Data")]
		[CriteriaOptions("MemberInfo,Type,ModelMember.Type")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string DataSourcePropertyIsNullCriteria { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemDataSourceCriteria"),
#endif
 Category("Data")]
		[CriteriaOptions("MemberInfo,Type,ModelMember.Type")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string DataSourceCriteria { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemCaptionForTrue"),
#endif
 Localizable(true)]
		[ModelBrowsable(typeof(BooleanPropertyOnlyCalculator))]
		[Category("Appearance")]
		string CaptionForTrue { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemCaptionForFalse"),
#endif
 Localizable(true)]
		[ModelBrowsable(typeof(BooleanPropertyOnlyCalculator))]
		[Category("Appearance")]
		string CaptionForFalse { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemImageForTrue"),
#endif
 ModelBrowsable(typeof(BooleanPropertyOnlyCalculator))]
		[Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string ImageForTrue { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemImageForFalse"),
#endif
 ModelBrowsable(typeof(BooleanPropertyOnlyCalculator))]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		[Category("Appearance")]
		string ImageForFalse { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemImageSizeMode"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Behavior")]
		[DefaultValue(ImageSizeMode.Zoom)]
		DevExpress.Persistent.Base.ImageSizeMode ImageSizeMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemPredefinedValues"),
#endif
 Localizable(true)]
		[ModelBrowsable(typeof(StringPropertyOnlyCalculator))]
		[Category("Data")]
		string PredefinedValues { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemLookupEditorMode"),
#endif
 Category("Behavior")]
		DevExpress.Persistent.Base.LookupEditorMode LookupEditorMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemImmediatePostData"),
#endif
 Category("Behavior")]
		bool ImmediatePostData { get; set; }
		[Browsable(false)]
		IEnumerable<Type> PropertyEditorTypes { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemPropertyEditorType"),
#endif
 DataSourceProperty("PropertyEditorTypes")]
		[TypeConverter(typeof(StringToTypeConverterBase))]
		[Category("Appearance")]
		[Required(typeof(ModelMemberRequiredCalculator))]
		[RefreshProperties(RefreshProperties.All)]
		Type PropertyEditorType { get; set; }
		[ModelBrowsable(typeof(LookupViewItemOnlyCalculator))]
		[Category("Behavior")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelCommonMemberViewItemAllowClear")]
#endif
		Boolean AllowClear { get; set; }
	}
	[ModelAbstractClass]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelLayoutElement : IModelNode {
		String Id { get; set; }
	}
	[ModelInterfaceImplementor(typeof(IModelCommonMemberViewItem), "ModelMember")]
	public interface IModelMemberViewItem : IModelNode, IModelCommonMemberViewItem, IModelLayoutElement {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemModelMember"),
#endif
 Category("Data")]
		IModelMember ModelMember { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemDataSourceProperty"),
#endif
 Category("Data")]
		String DataSourceProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemDataSourceCriteriaProperty"),
#endif
 Category("Data")]
		[CriteriaOptions("Type,ModelMember.Type")]
		String DataSourceCriteriaProperty { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemPropertyName"),
#endif
 Required()]
		[Category("Data")]
		String PropertyName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemMaxLength"),
#endif
 Category("Layout")]
		int MaxLength { get; set; }
		[Browsable(false)]
		IModelList<IModelView> Views { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemView"),
#endif
 DataSourceProperty("Views")]
		[Category("Appearance")]
		IModelView View { get; set; }
		[Browsable(false)]
		IModelView ParentView { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemImageEditorCustomHeight"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Layout")]
		int ImageEditorCustomHeight { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemImageEditorMode"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Appearance")]
		DevExpress.Persistent.Base.ImageEditorMode ImageEditorMode { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemImageEditorFixedWidth"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Layout")]
		int ImageEditorFixedWidth { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelMemberViewItemImageEditorFixedHeight"),
#endif
 ModelBrowsable(typeof(ImagePropertyEditorCalculator))]
		[Category("Layout")]
		int ImageEditorFixedHeight { get; set; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelClassDesignable {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool IsDesigned { get; set; }
	}
	public enum AllowAdd { Default, True, False }
	public enum GroupInterval { None, Day, Month, Year, Smart }
	public enum SummaryType { None, Sum, Min, Max, Count, Average, Custom }
}
