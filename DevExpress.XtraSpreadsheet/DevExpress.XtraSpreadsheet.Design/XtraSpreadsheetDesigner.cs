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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Design;
using DevExpress.XtraSpreadsheet.UI;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSpreadsheet.Design {
	#region XtraSpreadsheetDesigner
	public class XtraSpreadsheetDesigner : BaseControlDesigner, IServiceProvider {
		#region Fields
		IComponentChangeService changeService;
		IDesignerHost host;
		DesignerVerbCollection verbs;
		#endregion
		public XtraSpreadsheetDesigner() {
			XtraSpreadsheetRepositoryItemsRegistrator.Register();
		}
		#region Properties
		public SpreadsheetControl SpreadsheetControl { get { return Control as SpreadsheetControl; } }
		public IComponentChangeService ChangeService { get { return changeService; } }
		public IDesignerHost DesignerHost {
			get {
				if (host == null) {
					host = (IDesignerHost)GetService(typeof(IDesignerHost));
				}
				return host;
			}
		}
		protected override bool AllowHookDebugMode { get { return true; } }
		public override DesignerVerbCollection DXVerbs {
			get {
				if (this.verbs == null)
					CreateVerbs();
				return verbs;
			}
		}
		#endregion
		protected virtual void CreateVerbs() {
			this.verbs = new DesignerVerbCollection(new DesignerVerb[] { new DesignerVerb("About", new EventHandler(OnAboutClick)) });
		}
		protected virtual void OnAboutClick(object sender, EventArgs e) {
			SpreadsheetControl.About();
		}
		object IServiceProvider.GetService(Type type) {
			return base.GetService(type);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SpreadsheetAutomaticBindHelper.BindToSpellChecker(this);
			DesignerActionUIService service = EditorContextHelperEx.GetDesignerUIService(Component);
			if (service != null)
				service.ShowUI(Component);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Control control = component as Control;
			if (control != null)
				control.AllowDrop = false;
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += OnComponentAdded;
				changeService.ComponentRemoved += OnComponentRemoved;
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(CreateSpreadsheetAllBarsActionList());
			list.Add(CreateSpreadsheetActionList());
			list.Add(CreateSpreadsheetFormulaBarsActionList());
			list.Add(CreateSpreadsheetDockManagerActionList());
			base.RegisterActionLists(list);
		}
		protected virtual DesignerActionList CreateSpreadsheetDockManagerActionList() {
			return new SpreadsheetDocksActionList(this);
		}
		protected virtual DesignerActionList CreateSpreadsheetFormulaBarsActionList() {
			return new SpreadsheetFormulaBarsActionList(this);
		}
		protected virtual DesignerActionList CreateSpreadsheetAllBarsActionList() {
			return new SpreadsheetAllBarsActionList(this);
		}
		protected virtual DesignerActionList CreateSpreadsheetActionList() {
			return new SpreadsheetBarsActionList(this);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SpreadsheetControl.About));
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "SpellChecker", typeof(ISpellChecker));
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "MenuManager", typeof(IDXMenuManager));
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "FormulaBar", typeof(IFormulaBarControl));
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			SpreadsheetAutomaticBindHelper.BindToSpellChecker(this);
			AutomaticBindHelper.BindToComponent(this, "MenuManager", typeof(IDXMenuManager));
			AutomaticBindHelper.BindToComponent(this, "FormulaBar", typeof(IFormulaBarControl));
		}
		public void AddAllBars(bool clearExistingItemsBefore, BarInsertMode insertMode) {
			if (SpreadsheetDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				SpreadsheetAllBarsActionList actionList = new SpreadsheetAllBarsActionList(this);
				actionList.CreateAllBarsCore(clearExistingItemsBefore, insertMode);
			}
		}
	}
	#endregion
	#region SpreadsheetAutomaticBindHelper
	public static class SpreadsheetAutomaticBindHelper {
		public static bool BindToSpellChecker(ControlDesigner designer) {
			return AutomaticBindHelper.BindToComponent(designer, "SpellChecker", typeof(ISpellChecker));
		}
	}
	#endregion
	#region SpreadsheetBarsActionList
	public class SpreadsheetBarsActionList : DesignerActionList {
		readonly XtraSpreadsheetDesigner designer;
		public SpreadsheetBarsActionList(XtraSpreadsheetDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public XtraSpreadsheetDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (!SpreadsheetDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				PopulateCreateBarsItems(result);
				return result;
			}
			PopulateSortedActionItems(result);
			return result;
		}
		protected internal virtual void PopulateCreateBarsItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateRibbon", " Create Ribbon", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateBarManager", " Create BarManager", "Toolbar"));
		}
		protected internal virtual void PopulateSortedActionItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateFileBar", " Create File Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateHomeBar", " Create Home Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateInsertBar", " Create Insert Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreatePageLayoutBar", " Create Page Layout Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateFormulasBar", " Create Formulas Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateDataBar", " Create Data Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateReviewBar", " Create Review Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateViewBar", " Create View Bars", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateChartTools", " Create Chart Tools", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateTableTools", " Create Table Tools", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreatePictureTools", " Create Picture Tools", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateDrawingTools", " Create Drawing Tools", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreatePivotTableTools", " Create PivotTable Tools", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateMailMergeBar", " Create Mail Merge Bars", "Toolbar"));
		}
		#region CreateRibbon
		[RefreshProperties(RefreshProperties.All)]
		public void CreateRibbon() {
			SpreadsheetControl control = Designer.SpreadsheetControl;
			if (control == null)
				return;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create Ribbon")) {
				RibbonControl ribbon = (RibbonControl)Designer.DesignerHost.CreateComponent(typeof(RibbonControl));
				Designer.ChangeService.OnComponentChanging(form, null);
				form.Controls.Add(ribbon);
				RibbonForm ribbonForm = form as RibbonForm;
				if (ribbonForm != null)
					ribbonForm.Ribbon = ribbon;
				Designer.ChangeService.OnComponentChanging(form, null);
				Designer.ChangeService.OnComponentChanging(control, null);
				control.MenuManager = ribbon;
				Designer.ChangeService.OnComponentChanged(control, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		#endregion
		#region CreateBarManager
		[RefreshProperties(RefreshProperties.All)]
		public void CreateBarManager() {
			SpreadsheetControl control = Designer.SpreadsheetControl;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return;
			Control form = DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
			if (form == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create BarManager")) {
				Designer.ChangeService.OnComponentChanging(form, null);
				Designer.ChangeService.OnComponentChanged(form, null, null, null);
				BarManager barManager = (BarManager)Designer.DesignerHost.CreateComponent(typeof(BarManager));
				Designer.ChangeService.OnComponentChanging(container, null);
				container.Add(barManager);
				Designer.ChangeService.OnComponentChanged(container, null, null, null);
				Designer.ChangeService.OnComponentChanging(barManager, null);
				barManager.Form = form;
				Designer.ChangeService.OnComponentChanged(barManager, null, null, null);
				Designer.ChangeService.OnComponentChanging(control, null);
				control.MenuManager = barManager;
				Designer.ChangeService.OnComponentChanged(control, null, null, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		#endregion
		#region CreateFileBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFileBar() {
			AddNewBars(GetFileBarCreators(), "File", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetFileBarCreators() {
			ControlCommandBarCreator creator = new SpreadsheetFileBarCreator();
			ControlCommandBarCreator infoCreator = new SpreadsheetFileInfoBarCreator();
			return new ControlCommandBarCreator[] { creator, infoCreator };
		}
		#endregion
		#region CreateHomeBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateHomeBar() {
			AddNewBars(GetHomeBarCreators(), "Home", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetHomeBarCreators() {
			return GetHomeBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetHomeBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetClipboardBarCreator());
			result.Add(new SpreadsheetFontBarCreator());
			result.Add(new SpreadsheetAlignmentBarCreator());
			result.Add(new SpreadsheetNumberBarCreator());
			result.Add(new SpreadsheetStylesBarCreator());
			result.Add(new SpreadsheetCellsBarCreator());
			result.Add(new SpreadsheetEditingBarCreator());
			return result;
		}
		#endregion
		#region CreateInsertBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateInsertBar() {
			AddNewBars(GetInsertBarCreators(), "Insert", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetInsertBarCreators() {
			return GetInsertBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetInsertBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetTablesBarCreator());
			result.Add(new SpreadsheetIllustrationsBarCreator());
			result.Add(new SpreadsheetChartsBarCreator());
			result.Add(new SpreadsheetLinksBarCreator());
			result.Add(new SpreadsheetSymbolsBarCreator());
			return result;
		}
		#endregion
		#region CreatePageLayoutBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreatePageLayoutBar() {
			AddNewBars(GetPageLayoutBarCreators(), "PageLayout", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetPageLayoutBarCreators() {
			return GetPageLayoutBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetPageLayoutBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetPageSetupBarCreator());
			result.Add(new SpreadsheetPageSetupShowBarCreator());
			result.Add(new SpreadsheetPageSetupPrintBarCreator());
			result.Add(new SpreadsheetArrangeBarCreator());
			return result;
		}
		#endregion
		#region CreateReviewBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateReviewBar() {
			AddNewBars(GetReviewBarCreators(), "Review", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetReviewBarCreators() {
			return GetReviewBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetReviewBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetCommentsBarCreator());
			result.Add(new SpreadsheetChangesBarCreator());
			return result;
		}
		#endregion
		#region CreateViewBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateViewBar() {
			AddNewBars(GetViewBarCreators(), "View", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetViewBarCreators() {
			return GetViewBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetViewBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetShowBarCreator());
			result.Add(new SpreadsheetZoomBarCreator());
			result.Add(new SpreadsheetWindowBarCreator());
			return result;
		}
		#endregion
		#region CreateFormulasBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFormulasBar() {
			AddNewBars(GetFormulasBarCreators(), "Formulas", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetFormulasBarCreators() {
			ControlCommandBarCreator functionLibrary = new SpreadsheetFunctionLibraryBarCreator();
			ControlCommandBarCreator formulaDefinedNames = new SpreadsheetFormulaDefinedNamesBarCreator();
			ControlCommandBarCreator formulaAuditing = new SpreadsheetFormulaAuditingBarCreator();
			ControlCommandBarCreator formulaCalculation = new SpreadsheetFormulaCalculationBarCreator();
			return new ControlCommandBarCreator[] { functionLibrary, formulaDefinedNames, formulaAuditing, formulaCalculation };
		}
		#endregion
		#region CreateDataBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateDataBar() {
			AddNewBars(GetDataBarCreators(), "Data", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetDataBarCreators() {
			return GetDataBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetDataBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetSortAndFilterBarCreator());
			result.Add(new SpreadsheetDataToolsBarCreator());
			result.Add(new SpreadsheetOutlineBarCreator());
			return result;
		}
		#endregion
		#region CreateChartTools
		[RefreshProperties(RefreshProperties.All)]
		public void CreateChartTools() {
			AddNewBars(GetChartToolsBarCreators(), "ChartTools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetChartToolsBarCreators() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			PopulateChartDesignBarCreators(result);
			PopulateChartLayoutBarCreators(result);
			PopulateChartFormatBarCreators(result);
			return result.ToArray();
		}
		#endregion
		#region CreateChartDesignBar
		protected virtual void PopulateChartDesignBarCreators(List<ControlCommandBarCreator> creators) {
			creators.Add(new SpreadsheetChartsDesignTypeBarCreator());
			creators.Add(new SpreadsheetChartsDesignDataBarCreator());
			creators.Add(new SpreadsheetChartsDesignLayoutsBarCreator());
			creators.Add(new SpreadsheetChartsDesignStylesBarCreator());
		}
		#endregion
		#region CreateChartLayoutBars
		protected virtual void PopulateChartLayoutBarCreators(List<ControlCommandBarCreator> creators) {
			creators.Add(new SpreadsheetChartsLayoutLabelsBarCreator());
			creators.Add(new SpreadsheetChartsLayoutAxesBarCreator());
			creators.Add(new SpreadsheetChartsLayoutAnalysisBarCreator());
		}
		#endregion
		#region CreateChartFormatBars
		protected virtual void PopulateChartFormatBarCreators(List<ControlCommandBarCreator> creators) {
			creators.Add(new SpreadsheetChartsFormatArrangeBarCreator());
		}
		#endregion
		#region CreateTableTools
		[RefreshProperties(RefreshProperties.All)]
		public void CreateTableTools() {
			AddNewBars(GetTableToolsBarCreators(), "TableTools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetTableToolsBarCreators() {
			return GetTableToolsBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetTableToolsBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetTablePropertiesBarCreator());
			result.Add(new SpreadsheetTableToolsBarCreator());
			result.Add(new SpreadsheetTableStyleOptionsBarCreator());
			result.Add(new SpreadsheetTableStylesBarCreator());
			return result;
		}
		#endregion
		#region CreatePictureTools
		[RefreshProperties(RefreshProperties.All)]
		public void CreatePictureTools() {
			AddNewBars(GetPictureToolsBarCreators(), "PictureTools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetPictureToolsBarCreators() {
			return GetPictureToolsBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetPictureToolsBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetPictureFormatArrangeBarCreator());
			return result;
		}
		#endregion
		#region CreateDrawingTools
		[RefreshProperties(RefreshProperties.All)]
		public void CreateDrawingTools() {
			AddNewBars(GetDrawingToolsBarCreators(), "DrawingTools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetDrawingToolsBarCreators() {
			return GetDrawingToolsBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetDrawingToolsBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new SpreadsheetDrawingFormatArrangeBarCreator());
			return result;
		}
		#endregion
		#region CreatePivotTableTools
		[RefreshProperties(RefreshProperties.All)]
		public void CreatePivotTableTools() {
			AddNewBars(GetPivotTableToolsBarCreators(), "PivotTableTools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetPivotTableToolsBarCreators() {
			return GetPivotTableToolsBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetPivotTableToolsBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			PopulatePivotTableAnalyzeBarCreator(result);
			PopulatePivotTableDesignBarCreator(result);
			return result;
		}
		#endregion
		#region CreatePivotTableAnalyzeBar
		private void PopulatePivotTableAnalyzeBarCreator(List<ControlCommandBarCreator> creators) {
			creators.Add(new SpreadsheetPivotTableAnalyzePivotTableBarCreator());
			creators.Add(new SpreadsheetPivotTableAnalyzeAcitveFieldBarCreator());
			creators.Add(new SpreadsheetPivotTableAnalyzeDataBarCreator());
			creators.Add(new SpreadsheetPivotTableAnalyzeActionsBarCreator());
			creators.Add(new SpreadsheetPivotTableAnalyzeShowBarCreator());
		}
		#endregion
		#region CreatePivotTableDesignBar
		private void PopulatePivotTableDesignBarCreator(List<ControlCommandBarCreator> creators) {
			creators.Add(new SpreadsheetPivotTableDesignLayoutBarCreator());
			creators.Add(new SpreadsheetPivotTableDesignPivotTableStyleOptionsBarCreator());
			creators.Add(new SpreadsheetPivotTableDesignPivotTableStylesBarCreator());
		}
		#endregion
		#region CreateMailMergeBar
		[RefreshProperties(RefreshProperties.All)]
		public void CreateMailMergeBar() {
			AddNewBars(GetMailMergeBarCreators(), "MailMergeTools", BarInsertMode.Add);
		}
		protected ControlCommandBarCreator[] GetMailMergeBarCreators() {
			return GetMailMergeBarCreatorsCore().ToArray();
		}
		protected virtual List<ControlCommandBarCreator> GetMailMergeBarCreatorsCore() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.Add(new MailMergeDataBarCreator());
			result.Add(new MailMergeModeBarCreator());
			result.Add(new MailMergeExtendedBarCreator());
			result.Add(new MailMergeGroupingBarCreator());
			result.Add(new MailMergeFilteringBarCreator());
			result.Add(new MailMergeBindingBarCreator());
			return result;
		}
		#endregion
		protected void AddNewBars(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			SpreadsheetDesignTimeBarsGenerator generator = CreateGenerator();
			generator.AddNewBars(creators, barName, insertMode);
		}
		public void ClearExistingItems() {
			ControlCommandBarCreator fakeBarCreator = new SpreadsheetFileBarCreator();
			SpreadsheetDesignTimeBarsGenerator generator = CreateGenerator();
			generator.ClearExistingItems(fakeBarCreator);
		}
		protected virtual SpreadsheetDesignTimeBarsGenerator CreateGenerator() {
			return new SpreadsheetDesignTimeBarsGenerator(this.Designer.DesignerHost, Designer.Control);
		}
	}
	#endregion
	#region SpreadsheetAllBarsActionList
	public class SpreadsheetAllBarsActionList : SpreadsheetBarsActionList {
		public SpreadsheetAllBarsActionList(XtraSpreadsheetDesigner designer)
			: base(designer) {
		}
		protected internal override void PopulateCreateBarsItems(DesignerActionItemCollection items) {
		}
		protected internal override void PopulateSortedActionItems(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateAllBars", " Create All Bars", "Toolbar"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateAllBars() {
			CreateAllBarsCore(false, BarInsertMode.Add);
		}
		public void CreateAllBarsCore(bool clearExistingItemsBefore, BarInsertMode insertMode) {
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create Spreadsheet Bars")) {
				if (clearExistingItemsBefore)
					ClearExistingItems();
				List<ControlCommandBarCreator[]> creators = new List<ControlCommandBarCreator[]>();
				creators.Add(GetFileBarCreators());
				creators.Add(GetHomeBarCreators());
				creators.Add(GetInsertBarCreators());
				creators.Add(GetPageLayoutBarCreators());
				creators.Add(GetFormulasBarCreators());
				creators.Add(GetDataBarCreators());
				creators.Add(GetReviewBarCreators());
				creators.Add(GetViewBarCreators());
				creators.Add(GetChartToolsBarCreators());
				creators.Add(GetTableToolsBarCreators());
				creators.Add(GetPictureToolsBarCreators());
				creators.Add(GetDrawingToolsBarCreators());
				creators.Add(GetPivotTableToolsBarCreators());
				int first, last, step;
				if (insertMode == BarInsertMode.Add) {
					first = 0;
					last = creators.Count;
					step = 1;
				}
				else {
					first = creators.Count - 1;
					last = -1;
					step = -1;
				}
				for (int i = first; i != last; i += step)
					AddNewBars(creators[i], "Create Spreadsheet Bars", insertMode);
				transaction.Commit();
			}
		}
	}
	#endregion
	#region SpreadsheetFormulaBarsActionList
	public class SpreadsheetFormulaBarsActionList : DesignerActionList {
		readonly XtraSpreadsheetDesigner designer;
		public SpreadsheetFormulaBarsActionList(XtraSpreadsheetDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public XtraSpreadsheetDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			if (!SpreadsheetDesignTimeBarsGenerator.IsExistBarContainer(Component)) {
				PopulateCreateBarsItems(result);
				return result;
			}
			PopulateSortedActionItems(result);
			return result;
		}
		protected internal virtual void PopulateCreateBarsItems(DesignerActionItemCollection items) {
			AddFormulaBarItem(items);
		}
		protected internal virtual void PopulateSortedActionItems(DesignerActionItemCollection items) {
			AddFormulaBarItem(items);
		}
		void AddFormulaBarItem(DesignerActionItemCollection items) {
			if (Designer.SpreadsheetControl != null && Designer.SpreadsheetControl.GetService<IFormulaBarControl>() == null
													&& Designer.SpreadsheetControl.GetService<INameBoxControl>() == null)
				items.Add(new DesignerActionMethodItem(this, "CreateFormulaBar", " Create FormulaBar", "Toolbar"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFormulaBar() {
			SpreadsheetControl control = Designer.SpreadsheetControl;
			if (control == null)
				return;
			Control targetContainer = ObtainTargetContainer(control);
			if (targetContainer == null)
				return;
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create FormulaBar")) {
				SpreadsheetFormulaBarControl formulaBar = (SpreadsheetFormulaBarControl)Designer.DesignerHost.CreateComponent(typeof(SpreadsheetFormulaBarControl));
				SpreadsheetNameBoxControl nameBox = (SpreadsheetNameBoxControl)Designer.DesignerHost.CreateComponent(typeof(SpreadsheetNameBoxControl));
				SplitContainerControl splitContainer = (SplitContainerControl)Designer.DesignerHost.CreateComponent(typeof(SplitContainerControl));
				SplitterControl splitter = (SplitterControl)Designer.DesignerHost.CreateComponent(typeof(SplitterControl));
				ChangeForm(targetContainer, splitContainer, splitter);
				ChangeSplitter(splitter);
				ChangeNameBox(control, nameBox);
				ChangeSplitContainer(formulaBar, nameBox, splitContainer);
				ChangeFormulaBar(control, formulaBar);
				Designer.ChangeService.OnComponentChanging(control, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		Control ObtainTargetContainer(SpreadsheetControl control) {
			Control parent = control.Parent;
			if (parent != null)
				return parent;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return null;
			return DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
		}
		void ChangeFormulaBar(SpreadsheetControl control, SpreadsheetFormulaBarControl formulaBar) {
			Designer.ChangeService.OnComponentChanging(formulaBar, null);
			formulaBar.SpreadsheetControl = control;
			formulaBar.Dock = DockStyle.Fill;
			Designer.ChangeService.OnComponentChanged(formulaBar, null, null, null);
		}
		void ChangeNameBox(SpreadsheetControl control, SpreadsheetNameBoxControl nameBox) {
			Designer.ChangeService.OnComponentChanging(nameBox, null);
			nameBox.SpreadsheetControl = control;
			nameBox.Dock = DockStyle.Fill;
			nameBox.Size = new System.Drawing.Size(145, 20);
			nameBox.MinimumSize = new System.Drawing.Size(0, 20);
			Designer.ChangeService.OnComponentChanged(nameBox, null, null, null);
		}
		void ChangeForm(Control form, SplitContainerControl splitContainer, SplitterControl splitter) {
			Designer.ChangeService.OnComponentChanging(form, null);
			form.Controls.Add(splitter);
			form.Controls.Add(splitContainer);
			Designer.ChangeService.OnComponentChanged(form, null, null, null);
		}
		void ChangeSplitContainer(SpreadsheetFormulaBarControl formulaBar, SpreadsheetNameBoxControl nameBox, SplitContainerControl splitContainer) {
			Designer.ChangeService.OnComponentChanging(splitContainer, null);
			splitContainer.Width = nameBox.Width + formulaBar.Width;
			splitContainer.MinimumSize = new System.Drawing.Size(0, 20);
			splitContainer.Dock = DockStyle.Top;
			splitContainer.Height = nameBox.Height;
			splitContainer.SplitterPosition = nameBox.Width;
			splitContainer.Panel1.Controls.Add(nameBox);
			splitContainer.Panel2.Controls.Add(formulaBar);
			Designer.ChangeService.OnComponentChanged(splitContainer, null, null, null);
		}
		void ChangeSplitter(SplitterControl splitter) {
			Designer.ChangeService.OnComponentChanging(splitter, null);
			splitter.Dock = DockStyle.Top;
			splitter.MinSize = 20;
			Designer.ChangeService.OnComponentChanged(splitter, null, null, null);
		}
	}
	#endregion
	#region SpreadsheetDocksActionList
	public class SpreadsheetDocksActionList : DesignerActionList {
		readonly XtraSpreadsheetDesigner designer;
		public SpreadsheetDocksActionList(XtraSpreadsheetDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public XtraSpreadsheetDesigner Designer { get { return designer; } }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = new DesignerActionItemCollection();
			PopulateSortedActionItems(result);
			return result;
		}
		protected internal virtual void PopulateSortedActionItems(DesignerActionItemCollection items) {
			AddDockManagerItem(items);
		}
		void AddDockManagerItem(DesignerActionItemCollection items) {
			items.Add(new DesignerActionMethodItem(this, "CreateFieldListPanel", " Create Field List Panel", "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateParametersPanel", " Create Parameters Panel", "Toolbar"));
		}
		public object FindComponent(IContainer container, Type componentType) {
			return FindComponentInternal(container, delegate(Type testingObjectType) {
				return testingObjectType == componentType;
			});
		}
		public Component CreateComponent(IDesignerHost designerHost, Type componentType) {
			IToolboxUser toolboxUser = designerHost.GetDesigner(designerHost.RootComponent) as IToolboxUser;
			if (toolboxUser != null) {
				toolboxUser.ToolPicked(new ToolboxItem(componentType));
			}
			return FindComponent(designerHost.Container, componentType) as Component;
		}
		object FindComponentInternal(IContainer container, Predicate<Type> condition) {
			if (container == null) return null;
			foreach (object obj in container.Components) {
				if (condition(obj.GetType())) return obj;
			}
			return null;
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateFieldListPanel() {
			CreateSpreadsheetDockPanel<FieldListDockPanel>("Create Field List Dock Panel", dockManager => dockManager.CreateFieldListDockPanel());
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateParametersPanel() {
			CreateSpreadsheetDockPanel<MailMergeParametersDockPanel>("Create Parameters Dock Panel", dockManager => dockManager.CreateMailMergeParametersDockPanel());
		}
		void CreateSpreadsheetDockPanel<T>(string transactionName, Action<SpreadsheetDockManager> creator) {
			SpreadsheetControl control = Designer.SpreadsheetControl;
			if(control == null)
				return;
			Control targetContainer = ObtainTargetContainer(control);
			if(targetContainer == null)
				return;
			using(DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction(transactionName)) {
				IDesignerHost designerHost = (IDesignerHost) GetService(typeof(IDesignerHost));
				if(designerHost == null)
					return;
				SpreadsheetDockManager dockManager = GetDockManager();
				if(dockManager == null)
					return;
				if(!dockManager.Contains(typeof(T))) {
					dockManager.BeginUpdate();
					creator(dockManager);
					dockManager.EndUpdate();
				}
				Designer.ChangeService.OnComponentChanging(control, null);
				transaction.Commit();
				EditorContextHelperEx.RefreshSmartPanel(Component);
			}
		}
		SpreadsheetDockManager GetDockManager() {
			IDesignerHost designerHost = (IDesignerHost) GetService(typeof(IDesignerHost));
			SpreadsheetDockManager dockManager = (SpreadsheetDockManager) FindComponent(designerHost.Container, typeof(SpreadsheetDockManager));
			if(dockManager == null) {
				dockManager = (SpreadsheetDockManager) CreateComponent(designerHost, typeof(SpreadsheetDockManager));
			}
			SpreadsheetControl control = Designer.SpreadsheetControl;
			if(dockManager != null) {
				ChangeDockManager(control, dockManager);
			}
			return dockManager;
		}
		Control ObtainTargetContainer(SpreadsheetControl control) {
			Control parent = control.Parent;
			if (parent != null)
				return parent;
			IContainer container = Designer.DesignerHost.Container;
			if (container == null)
				return null;
			return DevExpress.XtraBars.Design.DesignHelpers.GetContainerControl(container);
		}
		void ChangeDockManager(SpreadsheetControl control, SpreadsheetDockManager dockManager) {
			Designer.ChangeService.OnComponentChanging(dockManager, null);
			dockManager.SpreadsheetControl = control;
			Designer.ChangeService.OnComponentChanged(dockManager, null, null, null);
		}
	}
	#endregion
	#region SpreadsheetDesignTimeBarsGenerator
	public class SpreadsheetDesignTimeBarsGenerator : DesignTimeBarsGenerator<SpreadsheetControl, SpreadsheetCommandId> {
		public SpreadsheetDesignTimeBarsGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override BarGenerationManagerFactory<SpreadsheetControl, SpreadsheetCommandId> CreateBarGenerationManagerFactory() {
			return new SpreadsheetBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> CreateBarController() {
			return new SpreadsheetBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblySpreadsheet);
		}
	}
	#endregion
	#region SpreadsheetDesignTimeStatusBarGenerator
	public class SpreadsheetDesignTimeStatusBarGenerator : DesignTimeBarsGenerator<SpreadsheetControl, SpreadsheetCommandId> {
		public SpreadsheetDesignTimeStatusBarGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override BarGenerationManagerFactory<SpreadsheetControl, SpreadsheetCommandId> CreateBarGenerationManagerFactory() {
			return new SpreadsheetStatusBarGenerationManagerFactory();
		}
		protected override ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> CreateBarController() {
			return new SpreadsheetBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblySpreadsheet);
		}
	}
	#endregion
	#region SpreadsheetBarGenerationManager
	public class SpreadsheetBarGenerationManager : BarGenerationManager<SpreadsheetControl, SpreadsheetCommandId> {
		public SpreadsheetBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region SpreadsheetRibbonGenerationManager
	public class SpreadsheetRibbonGenerationManager : RibbonGenerationManager<SpreadsheetControl, SpreadsheetCommandId> {
		public SpreadsheetRibbonGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
	}
	#endregion
	#region SpreadsheetBarGenerationManagerFactory
	public class SpreadsheetBarGenerationManagerFactory : BarGenerationManagerFactory<SpreadsheetControl, SpreadsheetCommandId> {
		protected override RibbonGenerationManager<SpreadsheetControl, SpreadsheetCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController) {
			return new SpreadsheetRibbonGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<SpreadsheetControl, SpreadsheetCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController) {
			return new SpreadsheetBarGenerationManager(creator, container, barController);
		}
	}
	#endregion
	#region SpreadsheetStatusBarGenerationManager
	public class SpreadsheetStatusBarGenerationManager : StatusBarGenerationManager<SpreadsheetControl, SpreadsheetCommandId> {
		public SpreadsheetStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController)
			: base(creator, container, barController) {
		}
	}
	#endregion
	#region SpreadsheetRibbonStatusBarGenerationManager
	public class SpreadsheetRibbonStatusBarGenerationManager : RibbonStatusBarGenerationManager<SpreadsheetControl, SpreadsheetCommandId> {
		public SpreadsheetRibbonStatusBarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController)
			: base(creator, container, barController) {
		}
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
	}
	#endregion
	#region SpreadsheetStatusBarGenerationManagerFactory
	public class SpreadsheetStatusBarGenerationManagerFactory : BarGenerationManagerFactory<SpreadsheetControl, SpreadsheetCommandId> {
		protected override RibbonGenerationManager<SpreadsheetControl, SpreadsheetCommandId> CreateRibbonGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController) {
			return new SpreadsheetRibbonStatusBarGenerationManager(creator, container, barController);
		}
		protected override BarGenerationManager<SpreadsheetControl, SpreadsheetCommandId> CreateBarGenerationManagerInstance(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> barController) {
			return new SpreadsheetStatusBarGenerationManager(creator, container, barController);
		}
	}
	#endregion
}
