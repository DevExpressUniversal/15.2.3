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
using System.Drawing;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Localization;
using DevExpress.ExpressApp.Win.Model;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.ExpressApp.Win.SystemModule {
	[DXToolboxItem(true)]
	[ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
	[Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms XAF applications.")]
	[ToolboxBitmap(typeof(WinApplication), "Resources.Toolbox_Module_System_Win.ico")]
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed class SystemWindowsFormsModule : ModuleBase, IModelXmlConverter, IModelNodeUpdater<IModelOptions>, IModelNodeUpdater<IModelLocalization> {
		static SystemWindowsFormsModule() {
			DevExpress.Data.Filtering.EnumProcessingHelper.RegisterEnum(typeof(DevExpress.ExpressApp.UIType));
			DevExpress.Data.Filtering.EnumProcessingHelper.RegisterEnum(typeof(DevExpress.XtraBars.Ribbon.RibbonFormStyle));
		}
		private void application_SetupComplete(object sender, EventArgs e) {
			((XafApplication)sender).SetupComplete -= new EventHandler<EventArgs>(application_SetupComplete);
			new DefaultSkinListGenerator(((XafApplication)sender).Model).SetPredefinedLookAndFeelStyle();
		}
		private PrintingSettingsStorage printingSettingsStorage = PrintingSettingsStorage.Application;
		protected override ModuleTypeList GetRequiredModuleTypesCore() {
			return new ModuleTypeList(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		}
		protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
			editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(EditorAliases.GridListEditor, typeof(object), typeof(GridListEditor), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelStaticText), typeof(StaticTextViewItem), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelStaticImage), typeof(StaticImageViewItem), true)));
			editorDescriptors.Add(new ViewItemDescriptor(new ViewItemRegistration(typeof(IModelActionContainerViewItem), typeof(WinActionContainerViewItem), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.StringPropertyEditor, typeof(string), typeof(StringPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.RichTextPropertyEditor, typeof(string), typeof(RichTextPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ProtectedContentPropertyEditor, typeof(Object), typeof(ProtectedContentPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ObjectPropertyEditor, typeof(object), typeof(ObjectPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.LookupPropertyEditor, typeof(object), typeof(LookupPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TimeSpanPropertyEditor, typeof(TimeSpan), typeof(TimeSpanPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.EnumPropertyEditor, typeof(Enum), typeof(EnumPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int16), typeof(IntegerPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int32), typeof(IntegerPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt16), typeof(IntegerPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt32), typeof(IntegerPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(Int64), typeof(LongPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.IntegerPropertyEditor, typeof(UInt64), typeof(LongPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.BooleanPropertyEditor, typeof(Boolean), typeof(BooleanPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.FloatPropertyEditor, typeof(float), typeof(FloatPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TypePropertyEditor, typeof(Type), typeof(TypePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.VisibleInReportsTypePropertyEditor, typeof(Type), typeof(VisibleInReportsTypePropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DateTimePropertyEditor, typeof(DateTime), typeof(DatePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DecimalPropertyEditor, typeof(Decimal), typeof(DecimalPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.PropertiesCollectionEditor, typeof(ITypedList), typeof(PropertiesCollectionEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DefaultPropertyEditor, typeof(object), typeof(DefaultPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ImagePropertyEditor, typeof(Image), typeof(ImagePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ImagePropertyEditor, typeof(Byte[]), typeof(ImagePropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.BytePropertyEditor, typeof(byte), typeof(BytePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ColorPropertyEditor, typeof(Color), typeof(ColorPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.DoublePropertyEditor, typeof(double), typeof(DoublePropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.CriteriaPropertyEditor, typeof(String), typeof(CriteriaPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.PopupCriteriaPropertyEditor, typeof(String), typeof(PopupCriteriaPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.PopupExpressionPropertyEditor, typeof(String), false, IsTypedElementProperty)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.PopupExpressionPropertyEditor, typeof(String), typeof(PopupExpressionPropertyEditor), false)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.ExtendedCriteriaPropertyEditor, typeof(String), false, typeof(CriteriaPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new AliasAndEditorTypeRegistration(EditorAliases.CheckedListBoxEditor, typeof(String), false, typeof(CheckedListBoxStringPropertyEditor), true)));
			editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.ImagePropertyEditor, typeof(Object), typeof(ImagePropertyEditor), true)));
		}
		private static bool IsTypedElementProperty(IModelMember modelMember) {
			return (modelMember.MemberInfo != null) && (modelMember.MemberInfo.FindAttribute<DevExpress.ExpressApp.Core.ElementTypePropertyAttribute>() != null);
		}
		protected override IEnumerable<Type> GetRegularTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(DevExpress.ExpressApp.Win.BaseDocumentSettingsDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.Model.IModelBandWin));
			result.Add(typeof(DevExpress.ExpressApp.Win.Model.IModelBandedColumnWin));
			result.Add(typeof(DevExpress.ExpressApp.Win.Model.IModelBandsLayoutWin));
			result.Add(typeof(DevExpress.ExpressApp.Win.ModelOptionsTabbedMdiLayoutDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.DefaultListViewShowFindPanelDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelEditorControlSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelLabel));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelModelEditorSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelMRUEditSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelOptionsPrintingSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelOptionsRibbon));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelOptionsWin));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelPrintingSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelSearchControllerSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelSearchControlSettings));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelSeparator));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelSplitter));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateBar));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateBarItem));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateBarItemBase));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateBarState));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateFormStates));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateNavBarCustomization));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateWin));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateXtraBarsCustomization));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelTemplateXtraBarsCustomizations));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelWinLayoutGroup));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelWinLayoutItem));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelWinLayoutManagerDetailViewOptions));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.IModelWinLayoutManagerOptions));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.ModelApplicationOptionsSkinLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.ModelOptionsWinForSkin));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.ModelPrintingSettingsDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.ModelWinLayoutGroupLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.ShowFindPanelDomainLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.WinLayoutItemLogic));
			result.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.WinLayoutManagerDetailViewOptionsLogic));
			return result;
		}
		protected override IEnumerable<Type> GetDeclaredExportedTypes() {
			return new Type[] { typeof(ProcessDataLockingInfoDialogObject) };
		}
		protected override IEnumerable<Type> GetDeclaredControllerTypes() {
			return new Type[] {
				typeof(DevExpress.ExpressApp.Win.EasyTest.LookupControlFinderController),
				typeof(DevExpress.ExpressApp.Win.EasyTest.WindowControlFinderController),
				typeof(AboutInfoController),
				typeof(AboutInfoFormController),
				typeof(ChooseSkinController),				
				typeof(CloseMdiChildWindowController),
				typeof(CloseWindowController),
				typeof(DashboardWinLayoutManagerController),
				typeof(WinFocusDashboardControlController),
				typeof(DockPanelsVisibilityController),
				typeof(EditModelController),
				typeof(ExitController),
				typeof(GridEditorColumnChooserController),
				typeof(GridListEditorController),
				typeof(GridListEditorPreviewRowController),
				typeof(HtmlFormattingController),
				typeof(ListViewFocusedElementToClipboardController),
				typeof(LockController),
				typeof(MdiTabImageController),
				typeof(FormIconController),
				typeof(NewItemRowDataSourcePropertyController),
				typeof(OpenObjectController),
				typeof(ToolbarVisibilityController),
				typeof(VersionsCompatibilityController),
				typeof(WaitCursorController),
				typeof(WinModificationsController),
				typeof(WinExportAnalysisController),
				typeof(WinExportController),
				typeof(WinFocusDefaultDetailViewItemController),
				typeof(WinFocusListEditorControlController),
				typeof(WinLayoutManagerController),
				typeof(WinWindowTemplateController),
				typeof(WinNewObjectViewController),
				typeof(WinShowStartupNavigationItemController),
				typeof(WinViewNavigationController),
				typeof(PrintingController),
				typeof(GridListEditorMemberLevelSecurityController),
				typeof(ProcessDataLockingInfoController),
				typeof(ProcessDataLockingInfoDialogController),
				typeof(FillCheckListBoxItemsViewController),
				typeof(FillCheckListBoxItemsDetailViewController),
				typeof(ContextMenuViewController),
				typeof(MainMdiWindowController),
				typeof(RibbonFillActionContainersController),
				typeof(WinModelDifferenceViewController)
			};
		}
		public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator) {
			updaterRegistrator.AddUpdater<IModelOptions>(this);
			updaterRegistrator.AddUpdater<IModelLocalization>(this);
			base.AddModelNodeUpdaters(updaterRegistrator);
		}
		public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			base.AddGeneratorUpdaters(updaters);
			updaters.Add(new FilterCaptionsLocalizationModelNodesGeneratorUpdater());
		}
		public override ICollection<Type> GetXafResourceLocalizerTypes() {
			ICollection<Type> result = new List<Type>();
			result.Add(typeof(GridControlLocalizer));
			result.Add(typeof(LayoutControlLocalizer));
			result.Add(typeof(NavBarControlLocalizer));
			result.Add(typeof(BarControlLocalizer));
			result.Add(typeof(DocumentManagerControlLocalizer));
			result.Add(typeof(DockManagerLocalizer));
			result.Add(typeof(RichEditControlLocalizer));
			result.Add(typeof(TreeListControlLocalizer));
			result.Add(typeof(VerticalGridControlLocalizer));
			result.Add(typeof(XtraEditorsLocalizer));
			result.Add(typeof(LargeStringEditFindFormLocalizer));
			result.Add(typeof(MainFormTemplateLocalizer));
			result.Add(typeof(DetailViewFormTemplateLocalizer));
			result.Add(typeof(MainFormV2TemplateLocalizer));
			result.Add(typeof(DetailFormV2TemplateLocalizer));
			result.Add(typeof(MainRibbonFormV2TemplateLocalizer));
			result.Add(typeof(DetailRibbonFormV2TemplateLocalizer));
			result.Add(typeof(NestedFrameTemplateLocalizer));
			result.Add(typeof(NestedFrameTemplateV2Localizer));
			result.Add(typeof(LookupControlTemplateLocalizer));
			result.Add(typeof(PopupFormTemplateLocalizer));
			return result;
		}
		public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			base.ExtendModelInterfaces(extenders);
			extenders.Add<IModelColumn, IModelColumnWin>();
			extenders.Add<IModelBand, IModelBandWin>();
			extenders.Add<IModelBandedColumn, IModelBandedColumnWin>();
			extenders.Add<IModelBandsLayout, IModelBandsLayoutWin>();
			extenders.Add<IModelApplication, IModelApplicationModelEditor>();
			extenders.Add<IModelTemplate, IModelTemplateWin>();
			extenders.Add<IModelOptions, IModelOptionsWin>();
			extenders.Add<IModelListView, IModelListViewShowFindPanel>();
			extenders.Add<IModelClass, IModelClassShowFindPanel>();
			extenders.Add<IModelOptions, IModelOptionsPrintingSettings>();
			extenders.Add<IModelOptions, IModelOptionsTabbedMdiLayout>();
			extenders.Add<IModelDetailView, IModelOptionsPrintingSettings>();
			extenders.Add<IModelListView, IModelOptionsPrintingSettings>();
			extenders.Add<IModelFormState, IBaseDocumentSettings>();
			extenders.Add<IModelCommonMemberViewItem, IModelToolTipOptions>();
			extenders.Add<IModelNavigationItem, IModelToolTipOptions>();
		}
		public override void Setup(XafApplication application) {
			base.Setup(application);
			application.SetupComplete += new EventHandler<EventArgs>(application_SetupComplete);
		}
		public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
			return ModuleUpdater.EmptyModuleUpdaters;
		}
		[DefaultValue(PrintingSettingsStorage.Application)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("SystemWindowsFormsModulePrintingSettingsStorage")]
#endif
		public PrintingSettingsStorage PrintingSettingsStorage {
			get { return printingSettingsStorage; }
			set { printingSettingsStorage = value; }
		}
		#region ISchemaUpdater Members
		#endregion
		#region IModelXmlConverter Members
		public void ConvertXml(ConvertXmlParameters parameters) {
			if(parameters.XmlNodeName == "Template") {
				if(!parameters.Values.ContainsKey(ModelValueNames.IsNewNode)) {
					parameters.Values.Add(ModelValueNames.IsNewNode, "True");
				}
			}
			if(parameters.XmlNodeName == "XtraBarsCustomization" && parameters.Node is IModelTemplateWin) {
				IModelTemplateXtraBarsCustomizations bars = ((IModelTemplateWin)parameters.Node).XtraBarsCustomizations;
				if(bars == null) {
					bars = parameters.Node.AddNode<IModelTemplateXtraBarsCustomizations>("XtraBarsCustomizations");
				}
				parameters.SubNode = bars;
				parameters.NodeType = typeof(IModelTemplateXtraBarsCustomization);
			}
			if(parameters.XmlNodeName == "FormState" &&
				parameters.Node != null && parameters.Node is IModelTemplateWin) {
				IModelTemplateFormStates formState = ((IModelTemplateWin)parameters.Node).FormStates;
				if(formState == null) {
					formState = parameters.Node.AddNode<IModelTemplateFormStates>("FormStates");
				}
				parameters.SubNode = formState;
			}
			if(parameters.XmlNodeName == "ListView") {
				string oldProperty1 = "ActiveFilterString";
				string oldProperty2 = "IsActiveFilterEnabled";
				string newProperty1 = "Filter";
				string newProperty2 = "FilterEnabled";
				if(parameters.ContainsKey(oldProperty1)) {
					parameters.Values[newProperty1] = parameters.Values[oldProperty1];
					parameters.Values.Remove(oldProperty1);
				}
				if(parameters.ContainsKey(oldProperty2)) {
					parameters.Values[newProperty2] = parameters.Values[oldProperty2];
					parameters.Values.Remove(oldProperty2);
				}
			}
		}
		#endregion
		#region IModelNodeUpdater<IModelOptions> Members
		public void UpdateNode(IModelOptions node, IModelApplication application) {
			if(node.HasValue("RibbonControlStyle")) {
				string value = node.GetValue<string>("RibbonControlStyle");
				node.ClearValue("RibbonControlStyle");
				((IModelOptionsWin)node).RibbonOptions.RibbonControlStyle = ((DevExpress.XtraBars.Ribbon.RibbonControlStyle)Enum.Parse(typeof(DevExpress.XtraBars.Ribbon.RibbonControlStyle), value));
			}
		}
		#endregion
		#region IModelNodeUpdater<IModelLocalization> Members
		public void UpdateNode(IModelLocalization node, IModelApplication application) {
			if(node != null) {
				IModelLocalizationGroup frameTemplatesLocalizationGroup = node["FrameTemplates"];
				if(frameTemplatesLocalizationGroup != null) {
					Dictionary<string, string> mainFormOldNodeNameToNewNodeNameMap = GetMainFormLocalizationConversionMap();
					UpdateLocalization(mainFormOldNodeNameToNewNodeNameMap, "MainForm", typeof(MainForm), frameTemplatesLocalizationGroup);
					Dictionary<string, string> detailViewFormOldNodeNameToNewNodeNameMap = GetDetailViewFormLocalizationConversionMap();
					UpdateLocalization(detailViewFormOldNodeNameToNewNodeNameMap, "DetailViewForm", typeof(DetailViewForm), frameTemplatesLocalizationGroup);
					Dictionary<string, string> lookupControlTemplateOldNodeNameToNewNodeNameMap = GetLookupControlTemplateLocalizationConversionMap();
					UpdateLocalization(lookupControlTemplateOldNodeNameToNewNodeNameMap, "LookupControlTemplate", typeof(LookupControlTemplate), frameTemplatesLocalizationGroup);
					Dictionary<string, string> nestedFrameTemplateOldNodeNameToNewNodeNameMap = GetNestedFrameTemplateLocalizationConversionMap();
					UpdateLocalization(nestedFrameTemplateOldNodeNameToNewNodeNameMap, "NestedFrameTemplate", typeof(NestedFrameTemplate), frameTemplatesLocalizationGroup);
					Dictionary<string, string> popupFormOldNodeNameToNewNodeNameMap = GetPopupFormLocalizationConversionMap();
					UpdateLocalization(popupFormOldNodeNameToNewNodeNameMap, "PopupForm", typeof(PopupForm), frameTemplatesLocalizationGroup);
				}
			}
		}
		private Dictionary<string, string> GetPopupFormLocalizationConversionMap() {
			Dictionary<string, string> oldNodeNameToNewNodeNameMap = new Dictionary<string, string>();
			oldNodeNameToNewNodeNameMap.Add("ObjectsCreation", "cObjectsCreation.Caption");
			oldNodeNameToNewNodeNameMap.Add("RecordEdit", "cRecordEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("Edit", "cEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("OpenObject", "cOpenObject.Caption");
			oldNodeNameToNewNodeNameMap.Add("View", "cView.Caption");
			oldNodeNameToNewNodeNameMap.Add("UndoRedo", "cUndoRedo.Caption");
			oldNodeNameToNewNodeNameMap.Add("Export", "cExport.Caption");
			oldNodeNameToNewNodeNameMap.Add("Print", "cPrint.Caption");
			return oldNodeNameToNewNodeNameMap;
		}
		private Dictionary<string, string> GetNestedFrameTemplateLocalizationConversionMap() {
			Dictionary<string, string> oldNodeNameToNewNodeNameMap = new Dictionary<string, string>();
			oldNodeNameToNewNodeNameMap.Add("ObjectsCreation", "cObjectsCreation.Caption");
			oldNodeNameToNewNodeNameMap.Add("RecordEdit", "cRecordEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("Save", "cSave.Caption");
			oldNodeNameToNewNodeNameMap.Add("Edit", "cEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("OpenObject", "cOpenObject.Caption");
			oldNodeNameToNewNodeNameMap.Add("Reports", "cReports.Caption");
			oldNodeNameToNewNodeNameMap.Add("Link", "cLink.Caption");
			oldNodeNameToNewNodeNameMap.Add("View", "cView.Caption");
			oldNodeNameToNewNodeNameMap.Add("OtherActions", "cDefault.Caption");
			oldNodeNameToNewNodeNameMap.Add("Filters", "cFilters.Caption");
			oldNodeNameToNewNodeNameMap.Add("Export", "cExport.Caption");
			oldNodeNameToNewNodeNameMap.Add("Diagnostic", "cDiagnostic.Caption");
			oldNodeNameToNewNodeNameMap.Add("Menu", "cMenu.Caption");
			oldNodeNameToNewNodeNameMap.Add("Toolbar", "standardToolBar.Text");
			oldNodeNameToNewNodeNameMap.Add("RecordsNavigation", "cRecordsNavigation.Caption");
			oldNodeNameToNewNodeNameMap.Add("Print", "cPrint.Caption");
			return oldNodeNameToNewNodeNameMap;
		}
		private Dictionary<string, string> GetLookupControlTemplateLocalizationConversionMap() {
			Dictionary<string, string> oldNodeNameToNewNodeNameMap = new Dictionary<string, string>();
			oldNodeNameToNewNodeNameMap.Add("AvailableRecords", "AvailableRecords.Text");
			oldNodeNameToNewNodeNameMap.Add("Type", "typeLabel.Text");
			oldNodeNameToNewNodeNameMap.Add("Find", "findLabel.Text");
			return oldNodeNameToNewNodeNameMap;
		}
		private Dictionary<string, string> GetDetailViewFormLocalizationConversionMap() {
			Dictionary<string, string> oldNodeNameToNewNodeNameMap = new Dictionary<string, string>();
			oldNodeNameToNewNodeNameMap.Add("MainMenuBar", "_mainMenuBar.Text");
			oldNodeNameToNewNodeNameMap.Add("FileSubMenu", "barSubItemFile.Caption");
			oldNodeNameToNewNodeNameMap.Add("ObjectsCreation", "cObjectsCreation.Caption");
			oldNodeNameToNewNodeNameMap.Add("Save", "cSave.Caption");
			oldNodeNameToNewNodeNameMap.Add("Edit", "cEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("OpenObject", "cOpenObject.Caption");
			oldNodeNameToNewNodeNameMap.Add("UndoRedo", "cUndoRedo.Caption");
			oldNodeNameToNewNodeNameMap.Add("Reports", "cReports.Caption");
			oldNodeNameToNewNodeNameMap.Add("Close", "cClose.Caption");
			oldNodeNameToNewNodeNameMap.Add("File", "cFile.Caption");
			oldNodeNameToNewNodeNameMap.Add("Print", "cPrint.Caption");
			oldNodeNameToNewNodeNameMap.Add("Export", "cExport.Caption");
			oldNodeNameToNewNodeNameMap.Add("EditSubMenu", "barSubItemEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("RecordEdit", "cRecordEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("ToolsSubMenu", "barSubItemTools.Caption");
			oldNodeNameToNewNodeNameMap.Add("Tools", "cTools.Caption");
			oldNodeNameToNewNodeNameMap.Add("Options", "cOptions.Caption");
			oldNodeNameToNewNodeNameMap.Add("Diagnostic", "cDiagnostic.Caption");
			oldNodeNameToNewNodeNameMap.Add("ViewSubMenu", "barSubItemView.Caption");
			oldNodeNameToNewNodeNameMap.Add("RecordsNavigation", "cRecordsNavigation.Caption");
			oldNodeNameToNewNodeNameMap.Add("View", "cView.Caption");
			oldNodeNameToNewNodeNameMap.Add("HelpSubMenu", "barSubItemHelp.Caption");
			oldNodeNameToNewNodeNameMap.Add("About", "cAbout.Caption");
			oldNodeNameToNewNodeNameMap.Add("MainToolbar", "standardToolBar.Text");
			oldNodeNameToNewNodeNameMap.Add("Search", "cSearch.Caption");
			oldNodeNameToNewNodeNameMap.Add("Filters", "cFilters.Caption");
			oldNodeNameToNewNodeNameMap.Add("FullTextSearch", "cFullTextSearch.Caption");
			oldNodeNameToNewNodeNameMap.Add("StatusBar", "_statusBar.Text");
			oldNodeNameToNewNodeNameMap.Add("Menu", "cMenu.Caption");
			return oldNodeNameToNewNodeNameMap;
		}
		private Dictionary<string, string> GetMainFormLocalizationConversionMap() {
			Dictionary<string, string> oldNodeNameToNewNodeNameMap = new Dictionary<string, string>();
			oldNodeNameToNewNodeNameMap.Add("MainMenu", "_mainMenuBar.Text");
			oldNodeNameToNewNodeNameMap.Add("FileSubMenu", "barSubItemFile.Caption");
			oldNodeNameToNewNodeNameMap.Add("ObjectsCreation", "cObjectsCreation.Caption");
			oldNodeNameToNewNodeNameMap.Add("Save", "cSave.Caption");
			oldNodeNameToNewNodeNameMap.Add("Edit", "cEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("OpenObject", "cOpenObject.Caption  ");
			oldNodeNameToNewNodeNameMap.Add("UndoRedo", "cUndoRedo.Caption");
			oldNodeNameToNewNodeNameMap.Add("Appearance", "cAppearance.Caption");
			oldNodeNameToNewNodeNameMap.Add("Reports", "cReports.Caption");
			oldNodeNameToNewNodeNameMap.Add("ViewsHistoryNavigation", "cViewsHistoryNavigation.Caption");
			oldNodeNameToNewNodeNameMap.Add("File", "cFile.Caption");
			oldNodeNameToNewNodeNameMap.Add("Print", "cPrint.Caption");
			oldNodeNameToNewNodeNameMap.Add("Export", "cExport.Caption");
			oldNodeNameToNewNodeNameMap.Add("Exit", "cExit.Caption");
			oldNodeNameToNewNodeNameMap.Add("EditSubMenu", "barSubItemEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("RecordEdit", "cRecordEdit.Caption");
			oldNodeNameToNewNodeNameMap.Add("ViewSubMenu", "barSubItemView.Caption");
			oldNodeNameToNewNodeNameMap.Add("NavigationBar", "");
			oldNodeNameToNewNodeNameMap.Add("NavigationPanel", "dockPanelNavigation.Text");
			oldNodeNameToNewNodeNameMap.Add("RecordsNavigation", "cRecordsNavigation.Caption");
			oldNodeNameToNewNodeNameMap.Add("View", "cView.Caption");
			oldNodeNameToNewNodeNameMap.Add("ToolsSubMenu", "barSubItemTools.Caption");
			oldNodeNameToNewNodeNameMap.Add("Tools", "cTools.Caption");
			oldNodeNameToNewNodeNameMap.Add("Diagnostic", "cDiagnostic.Caption");
			oldNodeNameToNewNodeNameMap.Add("Options", "cOptions.Caption");
			oldNodeNameToNewNodeNameMap.Add("Window", "Window.Caption");
			oldNodeNameToNewNodeNameMap.Add("HelpSubMenu", "barSubItemHelp.Caption");
			oldNodeNameToNewNodeNameMap.Add("About", "cAbout.Caption");
			oldNodeNameToNewNodeNameMap.Add("Panels", "cPanels.Caption");
			oldNodeNameToNewNodeNameMap.Add("Windows", "cWindows.Caption");
			oldNodeNameToNewNodeNameMap.Add("MainToolbar", "standardToolBar.Text");
			oldNodeNameToNewNodeNameMap.Add("Search", "cSearch.Caption");
			oldNodeNameToNewNodeNameMap.Add("FullTextSearch", "cFullTextSearch.Caption");
			oldNodeNameToNewNodeNameMap.Add("Filters", "cFilters.Caption");
			oldNodeNameToNewNodeNameMap.Add("StatusBar", "_statusBar.Text");
			oldNodeNameToNewNodeNameMap.Add("WindowList", "barMdiChildrenListItem.Caption");
			oldNodeNameToNewNodeNameMap.Add("CloseAllWindows", "");
			oldNodeNameToNewNodeNameMap.Add("Unspecified", "cDefault.Caption");
			oldNodeNameToNewNodeNameMap.Add("Menu", "cMenu.Caption");
			return oldNodeNameToNewNodeNameMap;
		}
		private void UpdateLocalization(Dictionary<string, string> oldNameToNewNameMap, string oldTemplateLocalizationNodeName, Type templateType, IModelLocalizationGroup frameTemplatesLocalizationGroup) {
			IModelLocalizationGroup oldTemplateLocalizationGroup = (IModelLocalizationGroup)frameTemplatesLocalizationGroup[oldTemplateLocalizationNodeName];
			if(oldTemplateLocalizationGroup != null) {
				IModelLocalizationGroup targetNode = frameTemplatesLocalizationGroup.AddNode<IModelLocalizationGroup>(templateType.FullName);
				foreach(IModelLocalizationItem item in oldTemplateLocalizationGroup) {
					if(oldNameToNewNameMap.ContainsKey(((ModelNode)item).Id)) {
						string newItemId = oldNameToNewNameMap[((ModelNode)item).Id];
						if(!string.IsNullOrEmpty(newItemId)) {
							IModelLocalizationItem addedItem = targetNode.AddNode<IModelLocalizationItem>(newItemId);
							addedItem.Name = newItemId;
							((ModelNode)addedItem).ApplyDiffValues((ModelNode)item);
						}
					}
				}
				oldTemplateLocalizationGroup.ClearNodes();
				oldTemplateLocalizationGroup.Remove();
			}
		}
		#endregion
	}
	public interface IModelColumnWin {
		[Browsable(false)]
		SummaryType GroupSummaryType { get; set; }
		[Browsable(false)]
		string GroupSummaryFieldName { get; set; }
		[Browsable(false)]
		DevExpress.Data.ColumnSortOrder GroupSummarySortOrder { get; set; }
	}
	public interface IModelApplicationModelEditor {
		[Browsable(false)]
		IModelModelEditorSettings ModelEditorSettings { get; }
	}
	[ModelPersistentName("ModelEditor")]
	[Browsable(false)]
	public interface IModelModelEditorSettings : IModelNode {
		IModelEditorControlSettings ModelEditorControl { get; }
		IModelSearchControlSettings SearchControl { get; }
		IModelSearchControllerSettings SearchController { get; }
		IModelMRUEditSettings MRUEdit { get; }
		String State { get; set; }
		String MaximizedOnScreen { get; set; }
		String X { get; set; }
		String Y { get; set; }
		String Width { get; set; }
		String Height { get; set; }
	}
	[Browsable(false)]
	public interface IModelEditorControlSettings : IModelNode {
		string SplitterPosition { get; set; }
		string PropertySplitterPosition { get; set; }
		string SearchControlVisible { get; set; }
		string FocusedObject { get; set; }
		string CurrentAspect { get; set; }
		string GroupSettings { get; set; }
	}
	[Browsable(false)]
	public interface IModelSearchControlSettings : IModelNode {
		string SearchSplitterPosition { get; set; }
		string SchemaVisible { get; set; }
	}
	[Browsable(false)]
	public interface IModelSearchControllerSettings : IModelNode {
		string SelectedNodes { get; set; }
	}
	[Browsable(false)]
	public interface IModelMRUEditSettings : IModelNode {
		string Items { get; set; }
	}
	[ModelPersistentName("XtraBarsCustomizations")]
	public interface IModelTemplateXtraBarsCustomizations : IModelNode, IModelList<IModelTemplateXtraBarsCustomization> {
	}
	[ModelPersistentName("XtraBarsCustomization")]
	public interface IModelTemplateXtraBarsCustomization : IModelTemplateCustomization, IModelList<IModelTemplateBar> {
		[DefaultValue(false)]
		bool LargeIcons { get; set; }
	}
	public interface IModelTemplateBarItemBase : IModelTemplateCustomization, IModelList<IModelTemplateBarItem> {
		bool IsNew { get; set; }
		bool IsDeleted { get; set; }
		[DefaultValue(-1)]
		int Width { get; set; }
		[DefaultValue(true)]
		bool IsVisible { get; set; }
	}
	[ModelPersistentName("Bar")]
	public interface IModelTemplateBar : IModelTemplateBarItemBase {
		[DefaultValue(-1)]
		int Offset { get; set; }
		[DefaultValue(BarDockStyle.Top)]
		BarDockStyle DockStyle { get; set; }
		[DefaultValue(-1)]
		int DockRow { get; set; }
		[DefaultValue(-1)]
		int DockCol { get; set; }
		[DefaultValue(0)]
		int FloatLocationX { get; set; }
		[DefaultValue(0)]
		int FloatLocationY { get; set; }
	}
	[ModelPersistentName("BarItem")]
	public interface IModelTemplateBarItem : IModelTemplateBarItemBase {
	}
	[ModelPersistentName("BarState")]
	public interface IModelTemplateBarState : IModelTemplateCustomization, IModelList<IModelTemplateXtraBarsCustomization> {
	}
	[ModelPersistentName("FormStates")]
	public interface IModelTemplateFormStates : IModelNode, IModelList<IModelFormState> {
	}
	[ModelPersistentName("NavBarCustomization")]
	public interface IModelTemplateNavBarCustomization : IModelTemplateCustomization {
		int Width { get; set; }
		[DefaultValue(-1)]
		int NavigationPaneMaxVisibleGroups { get; set; }
	}
	public interface IModelTemplateWin : IModelNode {
		string DockManagerSettings { get; set; }
		IModelTemplateXtraBarsCustomizations XtraBarsCustomizations { get; }
		IModelTemplateFormStates FormStates { get; }
		IModelTemplateNavBarCustomization NavBarCustomization { get; }
	}
	public interface IModelListViewShowFindPanel {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelListViewShowFindPanelShowFindPanel"),
#endif
 Category("Behavior")]
		[ModelValueCalculator("ModelClass", "DefaultListViewShowFindPanel")]
		bool ShowFindPanel { get; set; }
	}
	public interface IModelClassShowFindPanel {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelClassShowFindPanelDefaultListViewShowFindPanel"),
#endif
 Category("Behavior")]
		bool DefaultListViewShowFindPanel { get; set; }
	}
	[DomainLogic(typeof(IModelListViewShowFindPanel))]
	public static class ShowFindPanelDomainLogic {
		public static bool Get_ShowFindPanel(IModelListView modelListView) {
			bool showFindFilter = false;
			if(modelListView.ModelClass != null) {
				showFindFilter = ((IModelClassShowFindPanel)modelListView.ModelClass).DefaultListViewShowFindPanel;
			}
			return showFindFilter;
		}
	}
	[DomainLogic(typeof(IModelClassShowFindPanel))]
	public static class DefaultListViewShowFindPanelDomainLogic {
		public static bool Get_DefaultListViewShowFindPanel(IModelClass modelClass) {
			ListViewFindPanelAttribute attribute = modelClass.TypeInfo.FindAttribute<ListViewFindPanelAttribute>();
			if(attribute == null) {
				attribute = new ListViewFindPanelAttribute(false);
			}
			return attribute.ShowFindFilter;
		}
	}
	public class FilterCaptionsLocalizationModelNodesGeneratorUpdater : ModelNodesGeneratorUpdater<ModelLocalizationNodesGenerator> {
		public const string FilterCaptionsGroup = "OpenSaveDialogFilters";
		public override void UpdateNode(DevExpress.ExpressApp.Model.Core.ModelNode node) {
			IModelLocalization modelLocalization = (IModelLocalization)node;
			IModelLocalizationGroup filterCaptionsLocalizationGroup = modelLocalization[FilterCaptionsGroup];
			if(filterCaptionsLocalizationGroup == null) {
				filterCaptionsLocalizationGroup = modelLocalization.AddNode<IModelLocalizationGroup>(FilterCaptionsGroup);
			}
			List<SaveFileDialogFilterItem> filterItems = new List<SaveFileDialogFilterItem>(WinExportController.ExportTargetFilterItems.Values);
			filterItems.AddRange(WinExportController.ImageFilterItems);
			filterItems.Add(WinExportController.AllFilesFilterItem);
			foreach(SaveFileDialogFilterItem filterItem in filterItems) {
				IModelLocalizationItem item = (IModelLocalizationItem)filterCaptionsLocalizationGroup[filterItem.Id];
				if(item == null) {
					item = filterCaptionsLocalizationGroup.AddNode<IModelLocalizationItem>(filterItem.Id);
					item.Value = filterItem.Caption;
				}
			}
		}
	}
}
