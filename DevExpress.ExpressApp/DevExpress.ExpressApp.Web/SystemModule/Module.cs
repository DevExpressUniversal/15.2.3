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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Localization;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
namespace DevExpress.ExpressApp.Web.SystemModule {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET XAF applications.")]
	[ToolboxBitmap(typeof(WebApplication), "Resources.Toolbox_Module_System_Web.ico")]
	[ToolboxItemFilter("Xaf.Platform.Web")]
	public sealed class SystemAspNetModule : ModuleBase, IModelXmlConverter {
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] {
				typeof(LogonAttemptsAmountedToLimit),
				typeof(ParameterlessLogonFailedInfo)
			};
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(CallbackStartupScriptController),
				typeof(PreserveValidationErrorMessageAfterPostbackController),
				typeof(SessionDictionaryDifferenceStoreWindowController),
				typeof(WebListEditorSettingsStoreViewController),
				typeof(WebLogonController),
				typeof(DevExpress.ExpressApp.Web.TestScripts.TestScriptsController),
				typeof(ASPxGridListEditorPreviewRowViewController),
				typeof(WebListEditorProcessCallbackCompleteController),
				typeof(ChooseThemeController),
				typeof(CloseAfterSaveController),
				typeof(FocusController),
				typeof(ListEditorInplaceEditController),
				typeof(ListViewController),
				typeof(RedirectOnCallbackController),
				typeof(RedirectOnViewChangedController),
				typeof(RegisterThemeAssemblyController),
				typeof(WebDeleteObjectsViewController),
				typeof(WebDependentEditorController),
				typeof(WebModificationsController),
				typeof(WebExportAnalysisController),
				typeof(WebExportController),
				typeof(WebFocusDefaultDetailViewItemController),
				typeof(WebFocusPopupWindowController),
				typeof(WebIdAssignationController),
				typeof(WebLinkUnlinkController),
				typeof(WebListEditorRefreshController),
				typeof(WebNewObjectViewController),
				typeof(WebObjectMethodActionsViewController),
				typeof(WebRecordsNavigationController),				
				typeof(WebViewNavigationController),
				typeof(WebPropertyEditorImmediatePostDataController),
				typeof(UpdateListEditorSelectedObjectsController),
				typeof(ActionHandleExceptionController),
				typeof(ListEditorCustomRenderController),
				typeof(ComplexWebListEditorMemberLevelSecurityController),
				typeof(WebModelDifferenceViewController),
				typeof(ASPxGridListEditorMemberLevelSecurityController),
				typeof(WebShowStartupNavigationItemController),
				typeof(NewTemplateImagesReplacerController),
				typeof(FindPopupController), 
				typeof(ASPxCheckedComboBoxController),
				typeof(WebConfirmUnsavedChangesDetailViewController),
				typeof(ASPxGridListEditorConfirmUnsavedChangesController),
				typeof(ActionConfirmUnsavedChangesController),
				typeof(HeaderMenuController)
			};
		}
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			return new ModuleTypeList(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelCustomUserControlViewItemWeb), typeof(WebCustomUserControlViewItem), true)));
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(EditorAliases.GridListEditor, typeof(Object), typeof(ASPxGridListEditor), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelStaticText), typeof(DevExpress.ExpressApp.Web.Editors.StaticTextViewItem), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelStaticImage), typeof(DevExpress.ExpressApp.Web.Editors.StaticImageViewItem), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelActionContainerViewItem), typeof(DevExpress.ExpressApp.Web.Editors.WebActionContainerViewItem), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.StringPropertyEditor, typeof(String), typeof(ASPxStringPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TimeSpanPropertyEditor, typeof(TimeSpan), typeof(ASPxTimeSpanPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ImagePropertyEditor, typeof(Image), typeof(ASPxImagePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ImagePropertyEditor, typeof(Byte[]), typeof(ASPxImagePropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.EnumPropertyEditor, typeof(Enum), typeof(ASPxEnumPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ColorPropertyEditor, typeof(Color), typeof(ASPxColorPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int16), typeof(ASPxIntPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int32), typeof(ASPxIntPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt16), typeof(ASPxIntPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt32), typeof(ASPxIntPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int64), typeof(ASPxInt64PropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt64), typeof(ASPxInt64PropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.BytePropertyEditor, typeof(Byte), typeof(ASPxBytePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.BooleanPropertyEditor, typeof(Boolean), typeof(ASPxBooleanPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DecimalPropertyEditor, typeof(Decimal), typeof(ASPxDecimalPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TypePropertyEditor, typeof(Type), typeof(ASPxTypePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.VisibleInReportsTypePropertyEditor, typeof(Type), typeof(ASPxVisibleInReportsTypePropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.LookupPropertyEditor, typeof(object), typeof(ASPxLookupPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.FloatPropertyEditor, typeof(Single), typeof(ASPxFloatPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DefaultPropertyEditor, typeof(Object), typeof(ASPxDefaultPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DoublePropertyEditor, typeof(Double), typeof(ASPxDoublePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ObjectPropertyEditor, typeof(Object), typeof(ASPxObjectPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DetailPropertyEditor, typeof(Object), typeof(ASPxObjectPropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ByteArrayPropertyEditor, typeof(Byte[]), typeof(ASPxByteArrayPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DateTimePropertyEditor, typeof(DateTime), typeof(ASPxDateTimePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ProtectedContentPropertyEditor, typeof(Object), typeof(ASPxProtectedContentPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.CriteriaPropertyEditor, typeof(String), typeof(ASPxCriteriaPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.PopupCriteriaPropertyEditor, typeof(String), typeof(ASPxPopupCriteriaPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.CheckedListBoxEditor, typeof(String), false, typeof(ASPxCheckedListBoxStringPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ImagePropertyEditor, typeof(Object), typeof(ASPxImagePropertyEditor), true)));
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(ASPxGridViewControlLocalizer));
			result.Add(typeof(ASPxGridViewResourceLocalizer));
			result.Add(typeof(ASPxImagePropertyEditorLocalizer));
			result.Add(typeof(ASPxEditorsResourceLocalizer));
			result.Add(typeof(ASPxperienceResourceLocalizer));
			result.Add(typeof(ASPxCriteriaPropertyEditorLocalizer));
			return result;
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelListView, IModelListViewWeb>();
			extenders.Add<IModelListView, IModelOptionsWebProvider>();
			extenders.Add<IModelDetailView, IModelDetailViewWeb>();
			extenders.Add<IModelOptions, IModelOptionsWeb>();
			extenders.Add<IModelRootNavigationItems, IModelRootNavigationItemsWeb>();
			extenders.Add<IModelAction, IModelActionWeb>();
			extenders.Add<IModelAction, IModelPopupWindowShowActionWeb>();
			extenders.Add<IModelViewLayoutElement, IModelViewLayoutElementWeb>();
			extenders.Add<IModelMemberViewItem, IModelMemberViewItemWeb>();
			extenders.Add<IModelViewLayout, IModelViewLayoutWeb>();
			extenders.Add<IModelLayoutGroup, IModelLayoutGroupWeb>();
			extenders.Add<IModelColumn, IModelColumnWeb>();
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		#region IModelXmlConverter Members
		public void ConvertXml(ConvertXmlParameters parameters) {
			if(parameters.XmlNodeName == "ListView") {
				string oldProperty = "FilterExpression";
				string newProperty = "Filter";
				if(parameters.ContainsKey(oldProperty)) {
					parameters.Values[newProperty] = parameters.Values[oldProperty];
					parameters.Values.Remove(oldProperty);
				}
			}
		}
		#endregion
	}
	public enum InlineEditMode { EditForm, EditFormAndDisplayRow, Inline, PopupEditForm, Batch }
	public enum AdaptivityMode { Off, HideDataCells, HideDataCellsWindowLimit }
	public enum DetailRowMode { None, DetailView, DetailViewWithActions }
	public interface IModelDetailViewWeb {
		[ModelValueCalculator("Application.Options", typeof(IModelOptionsWeb), "CollectionsEditMode")]
		DevExpress.ExpressApp.Editors.ViewEditMode? CollectionsEditMode { get; set; }
	}
	public class ModelListViewGridListEditorVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			IModelListView modelListView = node as IModelListView;
			if(modelListView != null && modelListView.EditorType != null) {
				return typeof(ASPxGridListEditor).IsAssignableFrom(modelListView.EditorType);
			}
			else return false;
		}
	}
	public interface IModelColumnWeb {
		[ DefaultValue(0)]
		int AdaptivePriority { get; set; }
	}
	public interface IModelListViewWeb {
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelListViewWebShowSelectionColumn"),
#endif
 DefaultValue(true)]
		bool ShowSelectionColumn { get; set; }
		[Category("Behavior")]
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelListViewWebInlineEditMode"),
#endif
 DefaultValue(InlineEditMode.Inline)]
		InlineEditMode InlineEditMode { get; set; }
		[Browsable(false)]
		int PageIndex { get; set; }
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("IModelListViewWebPageSize")]
#endif
		[DefaultValue(PagerModelSynchronizer.DefaultPageSize), Category("Layout")]
		int PageSize { get; set; }
		[ModelBrowsable(typeof(ModelListViewGridListEditorVisibilityCalculator))]
		[DefaultValue(false)]
		bool EnableEndlessPaging { get; set; }
		[ModelBrowsable(typeof(ModelListViewGridListEditorVisibilityCalculator))]
		[DefaultValue(600)]
		int VerticalScrollableHeight { get; set; }
		[ModelBrowsable(typeof(ModelListViewGridListEditorVisibilityCalculator))]
		[DefaultValue(DetailRowMode.None)]
		[Category("Behavior")]
		DetailRowMode DetailRowMode { get; set; }
		[Category("Behavior")]
		[ ModelValueCalculator("this", "DetailView")]
		[DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[DataSourceCriteria(ModelObjectViewLogic.ModelViewsByClassCriteria)]
		[ModelBrowsable(typeof(ModelListViewGridListEditorVisibilityCalculator))]
		IModelDetailView DetailRowView { get; set; }
	}
	public interface IModelOptionsWebProvider {
		IModelOptionsWeb GetOptionsWeb();
	}
	[DomainLogic(typeof(IModelOptionsWebProvider))]
	public static class ModelOptionsWebProviderLogic {
		public static IModelOptionsWeb GetOptionsWeb(IModelOptionsWebProvider provider) {
			return (IModelOptionsWeb)((IModelNode)provider).Application.Options;
		}
	}
	public class ModelViewLayoutElementWebReadOnlyCalculator : IModelIsReadOnly {
		public Boolean IsReadOnly(IModelNode node, String propertyName) {
			if(propertyName == "IsCardGroup" && node is IModelViewLayoutElementWeb && !((IModelViewLayoutElementWeb)node).IsCardGroup) {
				return ModelViewLayoutElementWebLogic.GetParentState(node);
			}
			return false;
		}
		public Boolean IsReadOnly(IModelNode node, IModelNode childNode) {
			return false;
		}
	}
	public class ModelViewLayoutElementWebVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			if(propertyName == "IsCardGroup") {
				return node is IModelLayoutGroup || node is IModelTabbedGroup;
			}
			return true;
		}
	}
	public class ModelLayoutGroupWebVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			if(propertyName == "IsCollapsibleCardGroup") {
				IModelViewLayoutElementWeb group = node as IModelViewLayoutElementWeb;
				if(group != null) {
					return group.IsCardGroup;
				}
			}
			return true;
		}
	}
	[DomainLogic(typeof(IModelViewLayoutElementWeb))]
	public static class ModelViewLayoutElementWebLogic {
		public static bool Get_IsCardGroup(IModelViewLayoutElementWeb elementModel) {
			bool result = false;
			if(elementModel is IModelTabbedGroup || elementModel is IModelLayoutGroup) {
				if(!GetParentState((IModelNode)elementModel)) {
					bool showCaption = false;
					if(elementModel is IModelLayoutElementWithCaptionOptions) {
						bool? showCaptionCore = ((IModelLayoutElementWithCaptionOptions)elementModel).ShowCaption;
						showCaption = showCaptionCore.HasValue ? showCaptionCore.Value : false;
					}
					result |= showCaption;
					result |= elementModel is IModelTabbedGroup;
					if(!result && elementModel is IEnumerable) {
						foreach(object item in (IEnumerable)elementModel) {
							if(!(item is IModelLayoutGroup) && !(item is IModelTabbedGroup) && item is IModelViewLayoutElementWeb && !((IModelViewLayoutElementWeb)item).IsCardGroup) {
								result = true;
								break;
							}
						}
					}
				}
			}
			else {
				result = false;
			}
			return result;
		}
		public static bool GetParentState(IModelNode elementModel) {
			bool result = false;
			IModelNode parent = elementModel.Parent;
			while(parent != null && !result && parent is IModelViewLayoutElementWeb) {
				result = ((IModelViewLayoutElementWeb)parent).IsCardGroup;
				parent = parent.Parent;
			}
			return result;
		}
		public static bool Get_Adaptivity(IModelViewLayoutElement model) {
			IModelViewLayoutWeb layout = DevExpress.ExpressApp.Web.Layout.LayoutCSSCalculator.GetModelLayoutNode(model) as IModelViewLayoutWeb;
			if(layout != null) {
				return layout.Adaptivity;
			}
			return false;
		}
	}
	public interface IModelViewLayoutElementWeb {
		[Category("Behavior")]
		string CustomCSSClassName { get; set; }
		[ModelReadOnly(typeof(ModelViewLayoutElementWebReadOnlyCalculator))]
		[ModelBrowsable(typeof(ModelViewLayoutElementWebVisibilityCalculator))]
		[Category("Behavior")]
		bool IsCardGroup { get; set; }
		[Category("Behavior")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool Adaptivity { get; set; }
	}
	public interface IModelViewLayoutWeb {
		[Category("Behavior")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool Adaptivity { get; set; }
	}
	public interface IModelMemberViewItemWeb {
		[Category("Behavior")]
		[ModelValueCalculator("Application.Options", typeof(IModelOptionsWeb), "ConfirmUnsavedChanges")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		bool TrackPropertyChangesOnClient { get; set; }
	}
	public interface IModelLayoutGroupWeb {
		[Category("Behavior")]
		[DefaultValue(false)]
		[ModelBrowsable(typeof(ModelLayoutGroupWebVisibilityCalculator))]
		bool IsCollapsibleCardGroup { get; set; }
	}
	public interface IModelOptionsWeb {
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelOptionsWebListViewAllowSort"),
#endif
 DefaultValue(true)]
		bool ListViewAllowSort { get; set; }
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelOptionsWebListViewEnablePageSizeChooser"),
#endif
 DefaultValue(true)]
		bool ListViewEnablePageSizeChooser { get; set; }
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelOptionsWebListViewEnableColumnChooser"),
#endif
 DefaultValue(true)]
		bool ListViewEnableColumnChooser { get; set; }
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelOptionsWebCollectionsEditMode"),
#endif
 DefaultValue(ViewEditMode.View)]
		ViewEditMode CollectionsEditMode { get; set; }
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("IModelOptionsWebConfirmUnsavedChanges")]
#endif
		[Category("Behavior"), DefaultValue(true)]
		bool ConfirmUnsavedChanges { get; set; }
	}
	public interface IModelRootNavigationItemsWeb {
		[ DefaultValue(true)]
		[Category("Behavior")]
		bool ShowNavigationOnStart { get; set; }
	}
	public interface IModelActionWeb {
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelActionWebIsPostBackRequired"),
#endif
 DefaultValue(false)]
		[Category("Behavior")]
		bool IsPostBackRequired { get; set; }
		[
#if !SL
	DevExpressExpressAppWebLocalizedDescription("IModelActionWebAdaptivePriority"),
#endif
 DefaultValue(1000)]
		[Category("Behavior")]
		int AdaptivePriority { get; set; }
		[DefaultValue(false)]
		[Category("Behavior")]
#if !SL
	[DevExpressExpressAppWebLocalizedDescription("IModelActionWebConfirmUnsavedChanges")]
#endif
		bool ConfirmUnsavedChanges { get; set; }
	}
	public interface IModelPopupWindowShowActionWeb {
		[ DefaultValue(false)]
		[Category("Behavior")]
		bool UseFindTemplate { get; set; }
		[ DefaultValue(false)]
		[Category("Behavior")]
		bool ShowInFindPopup { get; set; }
	}
	[DomainLogic(typeof(IModelViewLayoutWeb))]
	public static class ModelViewLayoutWebLogic {
		public static bool Get_Adaptivity() {
			return WebApplicationStyleManager.IsNewStyle;
		}
	}
}
