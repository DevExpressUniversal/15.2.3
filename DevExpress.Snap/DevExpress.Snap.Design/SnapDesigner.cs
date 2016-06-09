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
using System.Windows.Forms;
using DevExpress.Snap;
using DevExpress.Snap.Extensions;
using DevExpress.Snap.Extensions.UI;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraCharts.UI;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.UI;
using DevExpress.XtraReports.Design;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Extensions.Native;
using DevExpress.Snap.Core.Services;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Design {
	public class SnapControlDesigner : XtraRichEditDesigner {
		public SnapControlDesigner() {
			RegisterRepositoryItems();
		}
		static void RegisterRepositoryItems() {
			RepositoryItemMailMergeCurrentRecordEdit.RegisterMailMergeCurrentRecordEdit(); 
		}
		public SnapControl SnapControl {
			get { return (SnapControl)Component; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			if (designerHost == null)
				return;
			AddServices(designerHost);
			if (DesignHelpers.FindComponent(designerHost.Container, typeof(SnapDockManager)) == null)
				DesignHelpers.CreateComponent(designerHost, typeof(SnapDockManager));
			else if(DesignHelpers.FindComponent(designerHost.Container, typeof(SnapDocumentManager)) == null)
				DesignHelpers.CreateComponent(designerHost, typeof(SnapDocumentManager));
			foreach (IComponent component in designerHost.Container.Components) {
				SnapDockManager snapDockManager = component as SnapDockManager;
				if (snapDockManager != null && snapDockManager.SnapControl == null)
					snapDockManager.SnapControl = Component as SnapControl;
				SnapDocumentManager snapDocumentManager = component as SnapDocumentManager;
				if (snapDocumentManager != null && snapDocumentManager.ClientControl == null)
					snapDocumentManager.ClientControl = Component as SnapControl;
				FieldListDockPanel fieldListDockPanel = component as FieldListDockPanel;
				if (fieldListDockPanel != null && Object.ReferenceEquals(fieldListDockPanel.SnapControl, SnapControl))
					SnapControl.Dock = DockStyle.Fill;
				else {
					ReportExplorerDockPanel reportExplorerDockPanel = component as ReportExplorerDockPanel;
					if (reportExplorerDockPanel != null && Object.ReferenceEquals(reportExplorerDockPanel.SnapControl, SnapControl))
						SnapControl.Dock = DockStyle.Fill;
				}
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IDesignerHost designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			AddServices(designerHost);
		}
		protected internal virtual void AddServices(IDesignerHost designerHost) {
			AddServiceIfNotExists(designerHost, typeof(ILookAndFeelService), new LookAndFeelService());
			AddServiceIfNotExists(designerHost, typeof(IDataSourceCollectionProvider), new SnapMailMergeDataSourceCollectionProvider(((ISnapControlOptions)SnapControl.Options).SnapMailMergeVisualOptions));
			AddServiceIfNotExists(designerHost, typeof(IDataContextService), SnapControl.DocumentModel.GetService<IDataContextService>());
		}
		void AddServiceIfNotExists(IDesignerHost host, Type serviceType, object serviceInstance) {
			if (host == null)
				return;
			if (host.GetService(serviceType) != null)
				return;
			host.AddService(serviceType, serviceInstance);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(SnapControl.About));
		}
		protected override void OnAboutClick(object sender, EventArgs e) {
			SnapControl.About();
		}
		protected override DesignerActionList CreateRichEditActionList() {
			return new SnapBarsActionList(this);
		}
		protected override DesignerActionList CreateRichEditAllBarsActionList() {
			return new SnapAllBarsActionList(this);
		}
		protected override DesignerActionList CreateRichEditCommentsActionList() {
			return null;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (DesignerHost == null)
					return;
				SnapDockManager dockManager = DesignHelpers.FindComponent(DesignerHost.Container, typeof(SnapDockManager)) as SnapDockManager;
				if (dockManager != null && Object.ReferenceEquals(dockManager.SnapControl, SnapControl))
					dockManager.SnapControl = null;
				SnapDocumentManager dockumentManager = DesignHelpers.FindComponent(DesignerHost.Container, typeof(SnapDocumentManager)) as SnapDocumentManager;
				if(dockumentManager != null && Object.ReferenceEquals(dockumentManager.ClientControl, SnapControl)) {
					DesignerHost.DestroyComponent(dockumentManager);
				}
			}
			base.Dispose(disposing);
		}
	}
	public abstract class SnapControlBarCreatorBase : ControlCommandBarCreator {
		readonly SnapControl snapControl;
		protected SnapControlBarCreatorBase(SnapControl snapControl) {
			this.snapControl = snapControl;
		}
		protected SnapControl Control { get { return snapControl; } }
	}
	public class SnapBarsActionList : RichEditBarsActionList {
		SnapControl SnapControl { get { return ((SnapControlDesigner)this.Designer).SnapControl; } }
		public SnapBarsActionList(SnapControlDesigner designer) : base(designer) {
		}
		protected override void PopulateSortedActionItems(DesignerActionItemCollection items, bool isRibbon) {
			string suffix = GetActionSuffix(isRibbon);
			items.Add(new DesignerActionMethodItem(this, "CreateAppearanceBars", String.Format(" Create Appearance {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateSNMergeFieldBars", String.Format(" Create Field {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateGroupBars", String.Format(" Create Group {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateSNListBars", String.Format(" Create List {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateChartBars", String.Format(" Create Chart {0}", suffix), "Toolbar"));
			items.Add(new DesignerActionMethodItem(this, "CreateMailMergeBars", String.Format(" Create Mail Merge {0}", suffix), "Toolbar"));
			base.PopulateSortedActionItems(items, isRibbon);
			const int mailingsIndex = 10;
#if DEBUG || DEBUGTEST
			System.Diagnostics.Debug.Assert(items[mailingsIndex].DisplayName.StartsWith(" Create Mail Merge "));
#endif
			items.RemoveAt(mailingsIndex);
		}
		protected void CreateAppearanceBars() {
			ThemesBarCreator themesBarCreator = new ThemesBarCreator(SnapControl);
			AddNewBars(new ControlCommandBarCreator[] { themesBarCreator }, "Appearance", BarInsertMode.Add);
		}
		protected void CreateSNMergeFieldBars() {
			DataShapingBarCreator dataShapingBarCreator = new DataShapingBarCreator(SnapControl);
			SNMergeFieldPropertiesBarCreator propertiesBarCreator = new SNMergeFieldPropertiesBarCreator(SnapControl);
			AddNewBars(new ControlCommandBarCreator[] { dataShapingBarCreator, propertiesBarCreator }, "Fields Tools", BarInsertMode.Add);
		}
		protected void CreateGroupBars() {
			GroupingBarCreator groupingBarCreator = new GroupingBarCreator();
			AddNewBars(new ControlCommandBarCreator[] { groupingBarCreator }, "Group Tools", BarInsertMode.Add);
		}
		protected void CreateSNListBars() {
			ListHeaderAndFooterBarCreator insertHeaderFooterBarCreator = new ListHeaderAndFooterBarCreator();
			ListCommandsBarCreator propertiesBarCreator = new ListCommandsBarCreator(SnapControl);
			SNListEditorRowLimitBarCreator editorRowLimitBarCreator = new SNListEditorRowLimitBarCreator(SnapControl);
			AddNewBars(new ControlCommandBarCreator[] { insertHeaderFooterBarCreator, propertiesBarCreator, editorRowLimitBarCreator }, "List Tools", BarInsertMode.Add);
		}
		protected void CreateChartBars() {
			ControlCommandBarCreator typesCreator = new ChartTypesBarCreator();
			ControlCommandBarCreator appearanceCreator = new ChartAppearanceBarCreator();
			ControlCommandBarCreator wizardCreator = new SNChartWizardBarCreator();
			AddNewBars(new ControlCommandBarCreator[] { typesCreator, appearanceCreator, wizardCreator }, "Chart Tools", BarInsertMode.Add);
		}
		protected void CreateMailMergeBars() {
			ControlCommandBarCreator mailMergeCreator = new MailMergeBarCreator();
			ControlCommandBarCreator currentRecordCreator = new MailMergeCurrentRecordBarCreator();
			ControlCommandBarCreator finishAndMergeCreator = new FinishAndMergeBarCreator();
			AddNewBars(new ControlCommandBarCreator[] { mailMergeCreator, currentRecordCreator, finishAndMergeCreator }, "Mail Merge Tools", BarInsertMode.Add);
		}
		protected override List<ControlCommandBarCreator> GetInsertBarCreatorsCore() {
			List<ControlCommandBarCreator> result = base.GetInsertBarCreatorsCore();
			result.Insert(3, new ToolboxBarCreator());
			return result;
		}
		protected override List<ControlCommandBarCreator> GetDocumentViewsBarCreatorsCore() {
			List<ControlCommandBarCreator> result = base.GetDocumentViewsBarCreatorsCore();
			result.Add(new ViewBarCreator());
			result.Add(new SnapMailMergeBarCreator());
			return result;
		}
		protected override RichEditDesignTimeBarsGenerator CreateGenerator() {
			if (SnapControl == null)
				return null;
			return new SnapDesignTimeBarsGenerator(Designer.DesignerHost, SnapControl);
		}
		protected override ControlCommandBarCreator CreateRichEditMailMergeBarCreatorInstance() {
			return new SnapMailMergeBarCreator();
		}
		protected override ControlCommandBarCreator CreateRichEditFileBarCreatorInstance() {
			return new SnapFileBarCreator();
		}
		protected override ControlCommandBarCreator[] GetFileBarCreators() {
			List<ControlCommandBarCreator> result = new List<ControlCommandBarCreator>();
			result.AddRange(base.GetFileBarCreators());
			result.Add(new SnapFileDataBarCreator());
			return result.ToArray();
		}
		protected override ControlCommandBarCreator[] GetDocumentReviewBarCreators() {
			return new ControlCommandBarCreator[] { new RichEditDocumentProofingBarCreator() };
		}
		protected override ControlCommandBarCreator CreateTableCellStylesBarCreatorInstance() {
			return new SnapTableStylesBarCreator();
		}
	}
	public class SnapAllBarsActionList : SnapBarsActionList {
		public SnapAllBarsActionList(SnapControlDesigner designer)
			: base(designer) {
		}
		protected override void PopulateCreateBarsItems(DesignerActionItemCollection items) {
		}
		protected override void PopulateSortedActionItems(DesignerActionItemCollection items, bool isRibbon) {
			string name = String.Format(" Create All {0}", isRibbon ? "Tabs" : "Bars");
			items.Add(new DesignerActionMethodItem(this, "CreateAllBars", name, "Toolbar"));
		}
		[RefreshProperties(RefreshProperties.All)]
		public void CreateAllBars() {
			using (DesignerTransaction transaction = Designer.DesignerHost.CreateTransaction("Create RichEdit Bars")) {
				try {
					CreateFileBar();
					CreateHomeBar();
					CreateInsertBar();
					CreatePageLayoutBar();					
					CreateDocumentViewsBar();
					CreateHeaderFooterBar();
					CreateTableBar();
					CreateDocumentReviewBar();
					CreateDocumentReferencesBar();
					CreateFloatingObjectBar();
					CreateAppearanceBars();
					CreateSNMergeFieldBars();
					CreateGroupBars();
					CreateSNListBars();
					CreateChartBars();
					CreateMailMergeBars();
					transaction.Commit();
				}
				catch {
					transaction.Cancel();
				}
			}
		}
	}
	public class SnapDesignTimeBarsGenerator : RichEditDesignTimeBarsGenerator {
		public SnapDesignTimeBarsGenerator(IDesignerHost host, IComponent component)
			: base(host, component) {
		}
		protected override ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> CreateBarController() {
			return new SnapBarController();
		}
		protected override void EnsureReferences(IDesignerHost designerHost) {
			base.EnsureReferences(designerHost);
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblySnapExtensions);
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyBars);
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyChartsCore);
			ReferencesHelper.EnsureReferences(designerHost, AssemblyInfo.SRAssemblyChartsUI);
		}
	}
}
