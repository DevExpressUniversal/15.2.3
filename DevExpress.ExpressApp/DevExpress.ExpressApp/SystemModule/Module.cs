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
using System.Drawing;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule.ModelXmlConverters;
using DevExpress.ExpressApp.SystemModule.Notifications;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Includes Controllers that represent basic features for XAF applications.")]
	[ToolboxBitmap(typeof(XafApplication), "Resources.Toolbox_Module_System.ico")]
	public sealed class SystemModule : ModuleBase, IModelXmlConverter {
		private static object lockObj;
		public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
			base.CustomizeTypesInfo(typesInfo);
			CurrentUserIdOperator.Register();
			IsAssignableFromViewModelClassCriteriaOperator.Register();
			lock(lockObj) {
				foreach(string parameterName in ParametersFactory.GetRegisteredParameterNames()) {
					IParameter parameter = ParametersFactory.FindParameter(parameterName);
					if(parameter.IsReadOnly && (CriteriaOperator.GetCustomFunction(parameter.Name) == null)) {
						CriteriaOperator.RegisterCustomFunction(new ReadOnlyParameterCustomFunctionOperator(parameter));
					}
				}
			}
		}
		protected internal override IEnumerable<Type> GetRegularTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(DevExpress.ExpressApp.Editors.IModelRegisteredPropertyEditor));
			result.Add(typeof(DevExpress.ExpressApp.Editors.IModelRegisteredPropertyEditors));
			result.Add(typeof(DevExpress.ExpressApp.Editors.IModelRegisteredViewItem));
			result.Add(typeof(DevExpress.ExpressApp.Editors.IModelRegisteredViewItems));
			result.Add(typeof(DevExpress.ExpressApp.Model.Core.ModelApplicationBase));
			result.Add(typeof(DevExpress.ExpressApp.Model.Core.ModelNode));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.EditorFactoryLogics));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelActionContainerViewItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelActionLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelActionsLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelBandDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelBandedLayoutItemDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelBandsLayoutDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelBOModelClassMembersLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelBOModelLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelClassLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelClassReportsVisibilityLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelColumnDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelColumnsDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelControllerLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelDashboardViewItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelDashboardViewLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelDetailViewLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelListViewLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelMemberLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelMemberViewItemDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelModelViewsEditorTypeLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelObjectViewLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelOptionsLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelViewControllerLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelViewDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelViewsDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.DomainLogics.ModelWindowControllerLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelAction));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelActionContainerViewItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelActionDesign));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelActions));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelApplication));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelAssemblyResourceImageSource));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBand));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBandedColumn));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBandedLayoutItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBandsLayout));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBaseChoiceActionItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBOModel));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelBOModelClassMembers));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelChoiceActionItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelChoiceActionItems));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelClass));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelClassInterfaces));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelColumn));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelColumns));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelColumnSummary));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelColumnSummaryItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelCompositeView));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelControlDetailItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelController));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelControllerActions));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelControllers));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelDashboardView));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelDashboardViewItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelDetailView));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelDisableReasons));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelFileImageSource));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelFormState));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelImageSource));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelImageSources));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelInterfaceLink));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutElement));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutElementWithCaption));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutElementWithCaptionOptions));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutGroup));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutManagerOptions));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLayoutViewItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelListView));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelListViewSplitLayout));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLocalization));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLocalizationGroup));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLocalizationItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelLocalizationItemBase));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelMember));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelMemberViewItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelNode));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelObjectView));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelOptions));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelPropertyEditor));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelReason));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelSchemaModule));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelSchemaModules));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelSorting));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelSortProperty));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelSplitLayout));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelStaticImage));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelStaticText));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelTabbedGroup));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelTemplate));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelTemplateCustomization));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelTemplates));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelView));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelViewController));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelViewItem));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelViewItems));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelViewLayout));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelViewLayoutElement));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelViews));
			result.Add(typeof(DevExpress.ExpressApp.Model.IModelWindowController));
			result.Add(typeof(DevExpress.ExpressApp.Model.ModelLayoutElementWithCaptionOptionsLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.ModelLayoutGroupLogic));
			result.Add(typeof(DevExpress.ExpressApp.Model.ModelLayoutViewItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.DefaultListViewNewItemRowPositionDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelActionContainer));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelActionLink));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelActionLinkLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelActionToContainerMapping));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelCreatableItem));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelCreatableItems));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelHiddenActions));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelListViewFilterItem));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelListViewFilters));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelNavigationItem));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelNavigationItems));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelOptionsDashboard));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelOptionsDashboards));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.IModelRootNavigationItems));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelActionContainerLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelChoiceActionItemChildItemsDisplayStyleDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelClassNavigationItemDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelClassShowAutoFilterRowDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelCreatableItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelDetailViewDefaultFocusedItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelListViewFilterItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelListViewFiltersLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelNavigationItemDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelNavigationItemsDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ModelPropertyEditorLinkViewDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.NewItemRowPositionDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.SystemModule.ShowAutoFilterRowDomainLogic));
			return result;
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			return new ModuleTypeList();
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] {
				typeof(DiagnosticInfoObject),
				typeof(AboutInfo),
				typeof(CriteriaProvider),
				typeof(DashboardViewItemDescriptor),
				typeof(DashboardOrganizer),
				typeof(ViewDashboardOrganizationItem),
				typeof(StaticTextDashboardOrganizationItem),
				typeof(StaticImageDashboardOrganizationItem),
				typeof(ActionContainerDashboardOrganizationItem),
				typeof(DashboardCreationInfo),
				typeof(ModelDifferenceCopyParameters)
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(LogonController),
				typeof(ObjectViewController),
				typeof(ViewController),
				typeof(WindowController),
				typeof(ActionsCriteriaViewController),
				typeof(ActionsInfoController),
				typeof(AutoFilterRowListViewController),
				typeof(CheckDeletedObjectController),
				typeof(DashboardCreationWizardController),
				typeof(DashboardCustomizationController),
				typeof(DashboardDeactivateItemsActionsController),				
				typeof(DashboardOrganizerItemsCollectionsController),
				typeof(DeleteObjectsViewController),
				typeof(DependentEditorController),
				typeof(ModificationsController),
				typeof(DetailViewEditorActionController),
				typeof(DetailViewLinkController),
				typeof(DiagnosticInfoController),
				typeof(DialogController),
				typeof(ActionControlsSiteController),
				typeof(FillActionContainersController),
				typeof(FilterController),
				typeof(FindLookupDialogController),
				typeof(FindLookupNewObjectDialogController),
				typeof(FocusDefaultDetailViewItemController),
				typeof(HideActionsViewController),
				typeof(LinkDialogController),
				typeof(LinkNewObjectController),
				typeof(LinkToListViewController),
				typeof(LinkUnlinkController),
				typeof(ListEditorNewObjectController),
				typeof(ListEditorPreviewRowViewController),
				typeof(ListViewProcessCurrentObjectController),
				typeof(LogoffController),
				typeof(NewItemRowListViewController),
				typeof(NewObjectViewController),
				typeof(ObjectMethodActionsViewController),
				typeof(RecordsNavigationController),
				typeof(RefreshController),
				typeof(ShowNavigationItemController),
				typeof(ViewDashboardOrganizationItemController),
				typeof(ViewInfoController),
				typeof(ViewNavigationController),
				typeof(WindowTemplateController),
				typeof(ListEditorSecurityController),
				typeof(ModelDifferenceViewController),
				typeof(ModelDifferenceAspectViewController),
				typeof(ListViewControllerBase),
				typeof(PostponeTimeListViewController),
				typeof(ResetViewSettingsController)
			};
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(PreviewControlLocalizer));
			return result;
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelClass, IModelClassDesignable>();
			extenders.Add<IModelColumn, IModelBandedColumn>();
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new ObjectMethodActionsActionsNodeUpdater());
		}
		public override void AddModelNodeValidators(IModelNodeValidatorRegistrator validatorRegistrator) {
			base.AddModelNodeValidators(validatorRegistrator);
			validatorRegistrator.AddValidator<IModelNode>(new RequiredReferenceModelValuesValidator());
			validatorRegistrator.AddValidator<IModelMemberViewItem>(new ModelMemberViewItemValidator());
			validatorRegistrator.AddValidator<IModelDashboardViewItem>(new ModelDashboardViewItemValidator());
			((ModelApplicationCreator)validatorRegistrator).GetNodeInfo(typeof(IModelViewLayout)).OnUpdateNodes = new Action<IList<ModelNode>>(
				delegate(IList<ModelNode> nodes) {
					ConvertLayoutDiffsController.ConvertLayoutDiffs(nodes);
				});
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		static SystemModule() {
			lockObj = new object();
			SystemExceptionResourceLocalizer.Register(typeof(SystemExceptionResourceLocalizer));
			UserVisibleExceptionResourceLocalizer.Register(typeof(UserVisibleExceptionResourceLocalizer));
			ParametersFactory.RegisterParameter(new DevExpress.ExpressApp.Filtering.CurrentUserIdParameter());
		}
		public SystemModule() {
		}
		protected internal override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration(EditorAliases.GridListEditor, typeof(object), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelDashboardViewItem), typeof(DashboardViewItem), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelControlDetailItem), typeof(DevExpress.ExpressApp.Layout.ControlViewItem), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelStaticText))));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelStaticImage))));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelActionContainerViewItem))));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.ListPropertyEditor, typeof(IEnumerable), typeof(ListPropertyEditor), true,
				ListPropertyEditor.IsMemberListPropertyEditorCompatible)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.ListPropertyEditor, typeof(IEnumerable<>), typeof(ListPropertyEditor), true,
				ListPropertyEditor.IsMemberListPropertyEditorCompatible)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.DefaultPropertyEditor, typeof(object), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ProtectedContentPropertyEditor, typeof(object), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.DetailPropertyEditor, typeof(object), false, typeof(DetailPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ObjectPropertyEditor, typeof(object), delegate(IModelMember modelMember) {
				return (modelMember.MemberInfo != null) && modelMember.MemberInfo.MemberTypeInfo.IsDomainComponent && !modelMember.MemberInfo.IsList && modelMember.MemberInfo.IsAggregated;
			})));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.LookupPropertyEditor, typeof(object), delegate(IModelMember modelMember) {
				return (modelMember.MemberInfo != null) && modelMember.MemberInfo.MemberTypeInfo.IsDomainComponent && !modelMember.MemberInfo.IsList && !modelMember.MemberInfo.IsAggregated;
			})));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ByteArrayPropertyEditor, typeof(Byte[]), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.StringPropertyEditor, typeof(String), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.RichTextPropertyEditor, typeof(String), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.HtmlPropertyEditor, typeof(String), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.TimeSpanPropertyEditor, typeof(TimeSpan), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.EnumPropertyEditor, typeof(Enum), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int16), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int32), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt16), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt32), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int64), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt64), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.BooleanPropertyEditor, typeof(Boolean), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.FloatPropertyEditor, typeof(Single), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.TypePropertyEditor, typeof(Type), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.VisibleInReportsTypePropertyEditor, typeof(Type), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.DateTimePropertyEditor, typeof(DateTime), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.DecimalPropertyEditor, typeof(Decimal), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.PropertiesCollectionEditor, typeof(ITypedList),
				delegate(IModelMember member) {
					if(member.MemberInfo == null) {
						return false;
					}
					else {
						foreach(ITypeInfo typeInfo in member.MemberInfo.MemberTypeInfo.ImplementedInterfaces) {
							if(typeof(IList).IsAssignableFrom(typeInfo.Type)) {
								return false;
							}
						}
					}
					return true;
				})));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ImagePropertyEditor, typeof(Image), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ImagePropertyEditor, typeof(Byte[]), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.BytePropertyEditor, typeof(Byte), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ColorPropertyEditor, typeof(Color), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.DoublePropertyEditor, typeof(Double), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.CriteriaPropertyEditor, typeof(String), false,
				IsCriteriaProperty)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.PopupCriteriaPropertyEditor, typeof(String), false,
				IsCriteriaProperty)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ExtendedCriteriaPropertyEditor, typeof(String), false,
				IsCriteriaProperty)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.CheckedListBoxEditor, typeof(String), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.ImagePropertyEditor, typeof(object), delegate(IModelMember modelMember) {
				return (modelMember.MemberInfo != null) && modelMember.MemberInfo.MemberTypeInfo.FindAttribute<MediaDataObjectAttribute>() != null;
			})));
		}
		private static bool IsCriteriaProperty(IModelMember modelMember) {
			return
				(modelMember.MemberInfo != null)
				&&
				(
#pragma warning disable 0618
					(modelMember.MemberInfo.FindAttribute<CriteriaObjectTypeMemberAttribute>() != null)
#pragma warning restore 0618
					||
					(modelMember.MemberInfo.FindAttribute<CriteriaOptionsAttribute>() != null)
				);
		}
		#region IModelXmlConverter Members
		private ModelClassIdConverter modelClassIdConverter = new ModelClassIdConverter();
		private ModelViewIdConverter modelViewIdConverter = new ModelViewIdConverter();
		private ModelNavigationItemIdConverter modelNavigationItemIdConverter = new ModelNavigationItemIdConverter();
		public void ConvertXml(ConvertXmlParameters convertXmlArgs) {
			IModelApplicationServices services = convertXmlArgs.Node.Root as IModelApplicationServices;
			if((convertXmlArgs.Node is IModelApplication) && (convertXmlArgs.XmlNodeName == "Options")
				||
				(convertXmlArgs.Node is IModelViews) && (convertXmlArgs.XmlNodeName == "ListView")) {
				if(convertXmlArgs.Values.ContainsKey("UseServerMode")) {
					String value = convertXmlArgs.Values["UseServerMode"];
					if(value.ToLower() == "true") {
						convertXmlArgs.Values["DataAccessMode"] = "Server";
					}
					else {
						convertXmlArgs.Values["DataAccessMode"] = "Client";
					}
					convertXmlArgs.Values.Remove("UseServerMode");
				}
			}
			if((services == null) || ((services != null) && (services.GetModuleVersion(Name) == null || services.GetModuleVersion(Name) < new Version(13, 1)))) {
				if(typeof(IModelMemberViewItem).IsAssignableFrom(convertXmlArgs.NodeType)) {
					if(convertXmlArgs.Values.Count > 0 && !convertXmlArgs.ContainsKey(ModelValueNames.Id) &&
						convertXmlArgs.Values.ContainsKey("PropertyName")) {
						convertXmlArgs.Values.Add(ModelValueNames.Id, convertXmlArgs.Values["PropertyName"]);
					}
				}
				if(typeof(IModelLayoutItem).IsAssignableFrom(convertXmlArgs.NodeType)) {
					if(convertXmlArgs.Values.Count > 0) {
						ConvertSize("MaxSize", convertXmlArgs);
						ConvertSize("MinSize", convertXmlArgs);
					}
				}
				if(typeof(IModelLayoutViewItem).IsAssignableFrom(convertXmlArgs.NodeType)) {
					if(convertXmlArgs.Values.Count > 0 && convertXmlArgs.ContainsKey(ModelValueNames.Id) &&
						!convertXmlArgs.Values.ContainsKey("ViewItem")) {
						convertXmlArgs.Values.Add("ViewItem", convertXmlArgs.Values[convertXmlArgs.GetRealKey(ModelValueNames.Id)]);
					}
				}
				if(convertXmlArgs.Node is IModelClass) {
					if(convertXmlArgs.XmlNodeName == "Member") {
						IModelBOModelClassMembers members = ((IModelClass)convertXmlArgs.Node).OwnMembers;
						if(members == null) {
							members = ((IModelClass)convertXmlArgs.Node).AddNode<IModelBOModelClassMembers>("OwnMembers");
						}
						convertXmlArgs.SubNode = members;
					}
				}
				if(convertXmlArgs.Node is IModelRootNavigationItems && convertXmlArgs.XmlNodeName == "Item") {
					IModelNavigationItems items = ((IModelRootNavigationItems)convertXmlArgs.Node).Items;
					if(items == null) {
						items = convertXmlArgs.Node.AddNode<IModelNavigationItems>("Items");
					}
					convertXmlArgs.SubNode = items;
					convertXmlArgs.NodeType = typeof(IModelNavigationItem);
				}
				if(convertXmlArgs.Node is IModelNavigationItem && convertXmlArgs.XmlNodeName == "Item") {
					IModelNavigationItems items = ((IModelNavigationItem)convertXmlArgs.Node).Items;
					if(items == null) {
						items = convertXmlArgs.Node.AddNode<IModelNavigationItems>("Items");
					}
					convertXmlArgs.SubNode = items;
					convertXmlArgs.NodeType = typeof(IModelNavigationItem);
				}
				if(convertXmlArgs.XmlNodeName == "NavigationItems") {
					if(convertXmlArgs.Values.ContainsKey("Current") && !convertXmlArgs.Values.ContainsKey("StartupNavigationItem")) {
						convertXmlArgs.Values.Add("StartupNavigationItem", convertXmlArgs.Values["Current"]);
					}
				}
				if(convertXmlArgs.XmlNodeName == "ActionRef") {
					convertXmlArgs.NodeType = typeof(IModelActionLink);
				}
				if(convertXmlArgs.XmlNodeName == "ColumnInfo") {
					convertXmlArgs.NodeType = typeof(IModelColumn);
				}
				if(convertXmlArgs.XmlNodeName == "DetailView") {
					IDictionary<string, string> values = convertXmlArgs.Values;
					if(values.ContainsKey("DefaultItem") && !values.ContainsKey("DefaultFocusedItem")) {
						values["DefaultFocusedItem"] = values["DefaultItem"];
					}
				}
				if(convertXmlArgs.Node is IModelApplication && convertXmlArgs.XmlNodeName == "DetailViewItems") {
					convertXmlArgs.NodeType = typeof(IModelRegisteredViewItems);
				}
				if(convertXmlArgs.Node is IModelRegisteredViewItems && convertXmlArgs.XmlNodeName == "DetailViewItem") {
					convertXmlArgs.NodeType = typeof(IModelRegisteredViewItem);
				}
				if((convertXmlArgs.Node is IModelControllers) && (convertXmlArgs.XmlNodeName == "ViewController")) {
					if (convertXmlArgs.Values.ContainsKey("Name")) {
						String value = convertXmlArgs.Values["Name"];
						if(value.Contains("DetailViewController")) {
							value = value.Replace("DevExpress.ExpressApp.SystemModule.DetailViewController", "DevExpress.ExpressApp.SystemModule.ModificationsController");
							value = value.Replace("DevExpress.ExpressApp.Win.SystemModule.WinDetailViewController", "DevExpress.ExpressApp.Win.SystemModule.WinModificationsController");
							value = value.Replace("DevExpress.ExpressApp.Web.SystemModule.WebDetailViewController", "DevExpress.ExpressApp.Web.SystemModule.WebModificationsController");
							convertXmlArgs.Values["Name"] = value;
						}
					}
				}
			}
			if(services != null) {
				Version modelModuleVersion = services.GetModuleVersion(Name);
				if((modelModuleVersion == null) || (modelModuleVersion < new Version(10, 2))) {
					if(typeof(IModelLayoutViewItem).IsAssignableFrom(convertXmlArgs.NodeType)
						&& convertXmlArgs.Values.ContainsKey("ShowCaption")
						&& (convertXmlArgs.Values["ShowCaption"] == "True")) {
						convertXmlArgs.Values.Remove("ShowCaption");
					}
				}
				modelClassIdConverter.Convert(convertXmlArgs);
				modelViewIdConverter.Convert(convertXmlArgs);
				modelNavigationItemIdConverter.Convert(convertXmlArgs);
			}
		}
		private static void ConvertSize(string propertyName, ConvertXmlParameters convertXmlArgs) {
			IDictionary<string, string> values = convertXmlArgs.Values;
			if(!values.ContainsKey(propertyName)) {
				bool hasValue = false;
				int width = 0, height = 0;
				string widthAttributeName = propertyName + "Width";
				if(values.ContainsKey(widthAttributeName)) {
					hasValue = true;
					width = int.Parse(values[widthAttributeName]);
					values.Remove(widthAttributeName);
				}
				string heightAttributeName = propertyName + "Height";
				if(values.ContainsKey(heightAttributeName)) {
					hasValue = true;
					height = int.Parse(values[heightAttributeName]);
					values.Remove(heightAttributeName);
				}
				if(hasValue) {
					convertXmlArgs.Values.Add(propertyName, new SizeConverter().ConvertToInvariantString(new Size(width, height)));
				}
			}
		}
		#endregion
#if DebugTest
		public void DebugTest_RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			RegisterEditorDescriptors(editorDescriptors);
		}
#endif
	}
	sealed class RequiredReferenceModelValuesValidator : IModelNodeValidator<IModelNode> {
		private Dictionary<string, bool> requiredAttributeHash = new Dictionary<string, bool>();
		internal RequiredReferenceModelValuesValidator() { }
		private IEnumerable<ModelValueInfo> GetRequiredPersistentValues(ModelNodeInfo nodeInfo) {
			foreach(ModelValueInfo valueInfo in nodeInfo.ValuesInfo) {
				Type type = nodeInfo.GeneratedClass;
				string propertyHashKey = type.FullName + "@" + valueInfo.Name;
				bool isRequired = false;
				if(!requiredAttributeHash.TryGetValue(propertyHashKey, out isRequired)) {
					isRequired = ModelEditorHelper.GetPropertyAttribute<RequiredAttribute>(type, valueInfo.Name) != null;
					requiredAttributeHash[propertyHashKey] = isRequired;
				};
				if(!string.IsNullOrEmpty(valueInfo.PersistentPath) && valueInfo.CanPersistentPathBeUsed && valueInfo.TypeConverter == null && isRequired) {
					yield return valueInfo;
				}
			}
		}
		public bool IsValid(IModelNode modelNode, IModelApplication application) {
			ModelNode node = (ModelNode)modelNode;
			if(node.NodeInfo == null)
				return true;
			foreach(ModelValueInfo valueInfo in GetRequiredPersistentValues(node.NodeInfo)) {
				object value = node.GetValue(valueInfo.Name);
				if(value == null) {
					return false;
				}
			}
			return true;
		}
	}
	sealed class ModelMemberViewItemValidator : IModelNodeValidator<IModelMemberViewItem> {
		internal ModelMemberViewItemValidator() { }
		public bool IsValid(IModelMemberViewItem modelMemberViewItem, IModelApplication application) {
			string propertyName = modelMemberViewItem.PropertyName;
			if(!string.IsNullOrEmpty(propertyName)) {
				IModelObjectView parentView = modelMemberViewItem.ParentView as IModelObjectView;
				if(parentView != null) {
					IModelClass modelClass = parentView.ModelClass;
					return modelClass != null && modelClass.FindMember(propertyName) != null;
				}
			}
			return false;
		}
	}
	sealed class ModelDashboardViewItemValidator : IModelNodeValidator<IModelDashboardViewItem> {
		internal ModelDashboardViewItemValidator() { }
		public bool IsValid(IModelDashboardViewItem modelDashboardViewItem, IModelApplication application) {
			IModelView view = modelDashboardViewItem.View;
			return view == null || !(view is IModelDashboardView);
		}
	}
	public static class CustomFunctionOperatorHelper {
		public static void Register(ICustomFunctionOperator customFunctionOperator) {
			ICustomFunctionOperator registeredItem = CriteriaOperator.GetCustomFunction(customFunctionOperator.Name);
			if(registeredItem != null && registeredItem != customFunctionOperator) {
				throw new InvalidOperationException();
			}
			if(registeredItem == null) {
				CriteriaOperator.RegisterCustomFunction(customFunctionOperator);
			}
		}
	}
	public class ReadOnlyParameterCustomFunctionOperator : ParameterCustomFunctionOperator {
		public ReadOnlyParameterCustomFunctionOperator(IParameter parameter) : base(parameter) {
			if(!parameter.IsReadOnly) {
				throw new ArgumentException();
			}
		}
	}
	public class ParameterCustomFunctionOperator : ICustomFunctionOperatorConvertibleToExpression {
		private IParameter parameter;
		public ParameterCustomFunctionOperator(IParameter parameter) {
			this.parameter = parameter;
		}
		public object Evaluate(params object[] operands) {
			return parameter.CurrentValue;
		}
		public string Name {
			get { return parameter.Name; }
		}
		public Type ResultType(params Type[] operands) {
			return parameter.Type;
		}
		Expression ICustomFunctionOperatorConvertibleToExpression.Convert(ICriteriaToExpressionConverter converter, params Expression[] operands) {
			return Expression.Constant(parameter.CurrentValue);
		}
	}
	public class CurrentUserIdOperator : ICustomFunctionOperatorConvertibleToExpression {
		public const string OperatorName = "CurrentUserId";
		private static readonly CurrentUserIdOperator instance = new CurrentUserIdOperator();
		public static void Register() {
			CustomFunctionOperatorHelper.Register(instance);
		}
		public object Evaluate(params object[] operands) {
			return SecuritySystem.CurrentUserId;
		}
		public string Name {
			get { return OperatorName; }
		}
		public Type ResultType(params Type[] operands) {
			return typeof(object);
		}
		Expression ICustomFunctionOperatorConvertibleToExpression.Convert(ICriteriaToExpressionConverter converter, params Expression[] operands) {
			return Expression.Constant(SecuritySystem.CurrentUserId);
		}
	}
}
