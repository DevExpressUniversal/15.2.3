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
using System.Drawing.Printing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Reporting.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting {
	public enum PrintColorSchema { Default, FullColor, GrayScale, BlackAndWhite };
	#region XtraSchedulerReport
	[
	Designer("DevExpress.XtraScheduler.Reporting.Design.SchedulerReportDesigner_," + AssemblyInfo.SRAssemblySchedulerDesign, typeof(IRootDesigner)),
	XRDesigner("DevExpress.XtraScheduler.Reporting.Design.SchedulerReportDesigner," + AssemblyInfo.SRAssemblySchedulerReportingExtensions, typeof(IRootDesigner))]
	public class XtraSchedulerReport : CustomReportBase, ISchedulerReport {
		SchedulerPrintAdapter schedulerAdapter;
		SchedulerPrintAdapter innerSchedulerAdapter;
		PrintingSystem fakedPrintingSystem;
		ReportPrintManager printManager;
		ReportControlChangeManager changeManager;
		PrintColorSchema printColorSchema;
		ReportViewManager viewManager;
		bool icCloned = false;
		ResourceColorSchemasCache resourceColorSchemasCache;
		public const string DefaultReportTemplateExt = ".schrepx";
		static XtraSchedulerReport() {
			CompilerReferences.XtraReportsRefs.Add(GetCurrentAssemblyLocation());
			CompilerReferences.XtraReportsRefs.Add(GetAssemblyLocations(AssemblyInfo.SRAssemblyScheduler));
			CompilerReferences.XtraReportsRefs.Add(GetAssemblyLocations(AssemblyInfo.SRAssemblySchedulerCore));
			var location = GetAssemblyLocations(AssemblyInfo.SRAssemblyUtils);
			if (!CompilerReferences.XtraReportsRefs.Contains(location))
				CompilerReferences.XtraReportsRefs.Add(location);
			DevExpress.XtraReports.Configuration.Settings.Default.StorageOptions.Extension = DefaultReportTemplateExt;
		}
		internal static string GetAssemblyLocations(string name) {
			try {
				return Assembly.Load(name).Location;
			} catch {
				return string.Empty;
			}
		}
		internal static string GetCurrentAssemblyLocation() {
			try {
				return Assembly.GetExecutingAssembly().Location;
			} catch {
				return string.Empty;
			}
		}
		public XtraSchedulerReport() {
			this.printManager = new ReportPrintManager(this);
			this.changeManager = new ReportControlChangeManager(ReportControlChangeActions.None);
			this.viewManager = CreateViewManager();
			this.printColorSchema = PrintColorSchema.Default;
			this.resourceColorSchemasCache = new ResourceColorSchemasCache();
			PrintTool.MakeCommandResponsive(this.PrintingSystem);
			SchedulerReportHelper.SetPrintingSystemCommandsVisibility(this.PrintingSystem);
		}
		#region Properties
		[Category(SRCategoryNames.Scheduler), DefaultValue(null), 
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("XtraSchedulerReportSchedulerAdapter")
#else
	Description("")
#endif
]
		public SchedulerPrintAdapter SchedulerAdapter {
			get { return schedulerAdapter; }
			set {
				if (schedulerAdapter == value)
					return;
				UnsubscribeSchedulerAdapterEvents();
				schedulerAdapter = value;
				SubscribeSchedulerAdapterEvents();
				OnSchedulerAdapterChanged();
			}
		}
		[Category(SRCategoryNames.Appearance), DefaultValue(PrintColorSchemaOptions.DefaultPrintColorSchema),
#if !SL
	DevExpressXtraSchedulerReportingLocalizedDescription("XtraSchedulerReportPrintColorSchema")
#else
	Description("")
#endif
		]
		public PrintColorSchema PrintColorSchema {
			get { return printColorSchema; }
			set {
				if (printColorSchema == value)
					return;
				printColorSchema = value;
				OnPrintColorSchemaChanged();
			}
		}
		protected internal ReportViewManager ViewManager { get { return viewManager; } }
		internal ResourceColorSchemasCache ResourceColorSchemasCache { get { return resourceColorSchemasCache; } }
		[Category(SRCategoryNames.View), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), Browsable(false)]
		public ReportViewCollection Views { get { return ViewManager.Views; } }
		protected internal SchedulerPrintAdapter ActualSchedulerAdapter {
			get { return SchedulerAdapter != null ? SchedulerAdapter : InnerSchedulerAdapter; }
		}
		protected internal SchedulerPrintAdapter InnerSchedulerAdapter {
			get {
				if (innerSchedulerAdapter == null) {
					SchedulerPrintAdapter adapter = CreateInnerSchedulerAdapter();
					SetInnerSchedulerAdapter(adapter);
				}
				return innerSchedulerAdapter;
			}
		}
		internal PrintingSystem FakedPrintingSystem {
			get {
				if (fakedPrintingSystem == null) {
					fakedPrintingSystem = new PrintingSystem();
					fakedPrintingSystem.PageSettings.Assign(
						new Margins(0, 0, 0, 0),
						new Margins(0, 0, 0, 0),
						PaperKind.Custom,
						new System.Drawing.Size(int.MaxValue, int.MaxValue),
						false);
					fakedPrintingSystem.PageSettings.AssignDefaultPrinterSettings(new PrinterSettingsUsing(false, false, false));
				}
				return fakedPrintingSystem;
			}
		}
		protected internal ReportPrintManager PrintManager { get { return printManager; } }
		protected internal ReportControlChangeManager ChangeManager { get { return changeManager; } }
		protected internal virtual bool IsDesignMode { get { return DesignMode; } }
		internal bool IsCloned { get { return icCloned; } set { icCloned = value; } }
		#endregion
		#region Dispose
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (viewManager != null) {
						viewManager.Dispose();
						viewManager = null;
					}
					if (fakedPrintingSystem != null) {
						fakedPrintingSystem.Dispose();
						fakedPrintingSystem = null;
					}
					if (printManager != null)
						printManager = null;
					if (schedulerAdapter != null) {
						UnsubscribeSchedulerAdapterEvents();
						schedulerAdapter = null;
					}
					if (innerSchedulerAdapter != null) {
						UnsubscribeInnerSchedulerAdapterEvents();
						innerSchedulerAdapter.Dispose();
						innerSchedulerAdapter = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected virtual void SetInnerSchedulerAdapter(SchedulerPrintAdapter adapter) {
			if (innerSchedulerAdapter != null) {
				UnsubscribeInnerSchedulerAdapterEvents();
				innerSchedulerAdapter.Dispose();
			}
			this.innerSchedulerAdapter = adapter;
			SubscribeInnerSchedulerAdapterEvents();
		}
		protected internal virtual void SubscribeInnerSchedulerAdapterEvents() {
			if (innerSchedulerAdapter != null)
				innerSchedulerAdapter.Disposed += new EventHandler(OnInnerSchedulerAdapterDisposed);
		}
		protected internal virtual void UnsubscribeInnerSchedulerAdapterEvents() {
			if (innerSchedulerAdapter != null)
				innerSchedulerAdapter.Disposed -= new EventHandler(OnInnerSchedulerAdapterDisposed);
		}
		protected void OnInnerSchedulerAdapterDisposed(object sender, EventArgs e) {
			this.innerSchedulerAdapter = null;
		}
		protected virtual SchedulerPrintAdapter CreateInnerSchedulerAdapter() {
			XtraSchedulerDebug.Assert(SchedulerAdapter == null);
			return new InnerPrintAdapter();
		}
		protected virtual ReportViewManager CreateViewManager() {
			return new ReportViewManager(this);
		}
		#region SubscribeSchedulerAdapterEvents
		protected virtual void SubscribeSchedulerAdapterEvents() {
			if (SchedulerAdapter == null)
				return;
			SchedulerAdapter.SchedulerSourceChanged += new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.TimeIntervalChanged += new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.FirstDayOfWeekChanged += new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.ClientTimeZoneIdChanged += new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.ResourceColorsChanged += new EventHandler(OnResourceColorsChanged);
			SchedulerAdapter.Disposed += new EventHandler(OnSchedulerAdapterDisposed);
		}
		#endregion
		#region UnsubscribeSchedulerAdapterEvents
		protected virtual void UnsubscribeSchedulerAdapterEvents() {
			if (SchedulerAdapter == null)
				return;
			SchedulerAdapter.SchedulerSourceChanged -= new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.TimeIntervalChanged -= new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.FirstDayOfWeekChanged -= new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.ClientTimeZoneIdChanged -= new EventHandler(OnSchedulerAdapterPropertiesChanged);
			SchedulerAdapter.ResourceColorsChanged -= new EventHandler(OnResourceColorsChanged);
			SchedulerAdapter.Disposed -= new EventHandler(OnSchedulerAdapterDisposed);
		}
		#endregion
		protected internal virtual void OnSchedulerAdapterChanged() {
			UpdateResourceColorSchemasCache();
			ViewManager.UpdateSchedulerAdapterForAll();
		}
		protected void OnResourceColorsChanged(object sender, EventArgs e) {
			UpdateResourceColorSchemasCache();
		}
		protected void OnSchedulerAdapterPropertiesChanged(object sender, EventArgs e) {
			ViewManager.ApplyChangesForAll(ReportControlChangeType.SchedulerAdapterChanged);
		}
		protected void OnSchedulerAdapterDisposed(object sender, EventArgs e) {
			this.schedulerAdapter = null;
		}
		private void OnPrintColorSchemaChanged() {
		}
		#region Changes management
		protected internal virtual void ApplyChanges(IReportControlChangeTarget target, ReportControlChangeType change) {
			ReportControlChangeActions actions = ReportChangeActionsCalculator.CalculateChangeActions(change);
			ApplyChangesCore(target, change, actions);
		}
		protected internal virtual void ApplyChangesCore(IReportControlChangeTarget target, ReportControlChangeType change, ReportControlChangeActions actions) {
			List<ReportControlChangeType> changeTypes = new List<ReportControlChangeType>();
			changeTypes.Add(change);
			ApplyTargetChanges(target, changeTypes, actions);
		}
		protected internal virtual void ApplyTargetChanges(IReportControlChangeTarget target, List<ReportControlChangeType> changeTypes, ReportControlChangeActions actions) {
			ChangeManager.Actions = actions;
			ChangeManager.ApplyChanges(target);
		}
		#endregion
		protected override void SetProgressRanges(float progressRange) {
			PrintingSystem.ProgressReflector.SetProgressRanges(new float[0]);
			PrintingSystem.ProgressReflector.CanAutocreateRange = false;
		}
		protected override bool ShouldCreateDetail(int index) {
			if (DesignMode)
				return index == 0;
			if (ReportPrintOptions.DetailCount > 0 && index >= ReportPrintOptions.DetailCount)
				return false;
			return PrintManager.ShouldCreateDetail(index);
		}
		class BandPresenterWrapper : BandPresenter {
			BandPresenter source;
			public BandPresenterWrapper(XtraSchedulerReport report, BandPresenter source)
				: base(report) {
				this.source = source;
			}
			public override ArrayList GetPrintableControls(Band band) {
				ArrayList controls = source.GetPrintableControls(band);
				XRControl[] xrControls = (XRControl[])controls.ToArray(typeof(XRControl));
				List<XRControl> sourceControls = new List<XRControl>(xrControls);
				IList<XRControl> sortedControls = ((XtraSchedulerReport)report).SortControls(sourceControls);
				ArrayList result = new ArrayList();
				int count = sortedControls.Count;
				for (int i = 0; i < count; i++)
					result.Add(sortedControls[i]);
				return result;
			}
		}
		protected override BandPresenter CreateBandPresenter() {
			return new BandPresenterWrapper(this, base.CreateBandPresenter());
		}
		protected virtual IList<XRControl> SortControls(List<XRControl> sourceControls) {
			return Algorithms.TopologicalSort<XRControl>(sourceControls, new ReportViewControlComparer());
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			if (SchedulerAdapter != null) {
				components.Add(SchedulerAdapter);
			}
			components.AddRange(Views);
		}
		protected override void CopyFrom(XtraReport source, bool forceDataSource) {
			base.CopyFrom(source, forceDataSource);
			ViewManager.ClearViews();
			FieldInfo[] fieldInfos = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			List<object> components;
			components = SchedulerPrintingUtils.FindObjectsByType(source, fieldInfos, typeof(SchedulerPrintAdapter));
			if (components.Count >= 1) {
				SchedulerAdapter = (SchedulerPrintAdapter)components[0];
			}
			components = SchedulerPrintingUtils.FindObjectsByType(source, fieldInfos, typeof(ReportViewBase));
			for (int i = 0; i < components.Count; i++) {
				Views.Add((ReportViewBase)components[i]);
			}
		}
		protected override void CopyDataProperties(XRControl control) {
			base.CopyDataProperties(control);
			XtraSchedulerReport schedulerReport = control as XtraSchedulerReport;
			XtraSchedulerDebug.Assert(schedulerReport != null);
			if (schedulerReport != null) {
				PrintColorSchema = schedulerReport.PrintColorSchema;
				SchedulerAdapter = schedulerReport.SchedulerAdapter;
				IsCloned = true;
				ViewManager.UpdateAllowFakeDataForAll();
				ViewManager.UpdateSchedulerAdapterForAll();
			}
		}
		internal int GetPrintColumnIndex(ReportViewControlBase control) {
			return PrintManager.GetPrintColumnIndex(control);
		}
		protected override void OnReportInitialize() {
			base.OnReportInitialize();
			ViewManager.UpdateVisibleIntervalsForAll(); 
		}
		protected override void OnDesignerLoaded(DevExpress.XtraReports.UserDesigner.DesignerLoadedEventArgs e) {
			base.OnDesignerLoaded(e);
			ViewManager.UpdateVisibleIntervalsForAll();
		}
		internal void UpdateResourceColorSchemasCache() {
			ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> schemas = GetResourceColorSchemas();
			ResourceBaseCollection resources = GetResources();
			ResourceColorSchemasCache.Update(resources, schemas);
		}
		public SchedulerColorSchema GetResourceColorSchema(Resource resource) {
			return ResourceColorSchemasCache.GetSchema(resource);
		}
		internal ResourceBaseCollection GetResources() {
			ResourceBaseCollection result = ActualSchedulerAdapter.GetResources();
			if (result.Count == 0)
				result = ReportViewBase.CreateFakeResources();
			return result;
		}
		internal ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> GetResourceColorSchemas() {
			ISchedulerColorSchemaCollection<SchedulerColorSchemaBase> schemas = ActualSchedulerAdapter.GetResourceColorSchemas();
			return schemas;
		}
		public override void CreateDocument(bool buildPagesInBackground) {
			if (SchedulerAdapter == null) {
				base.CreateDocument(buildPagesInBackground);
				return;
			}
			SchedulerAdapter.RunCreatingDocument(new Action<bool>((backgroundBuild) => {
				base.CreateDocument(backgroundBuild);
				IStatusBrushAdapter statusAdapter = SchedulerAdapter as IStatusBrushAdapter;
				if (statusAdapter != null)
					statusAdapter.ClearCache();
			}), buildPagesInBackground);
		}
	}
	#endregion
}
