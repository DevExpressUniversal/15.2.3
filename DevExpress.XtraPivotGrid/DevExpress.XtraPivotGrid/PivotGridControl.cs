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
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Data;
using DevExpress.Data.ChartDataSources;
using DevExpress.Data.Linq;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.Export;
using DevExpress.LookAndFeel;
using DevExpress.PivotGrid.Printing;
using DevExpress.PivotGrid.Export;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.PivotGrid.ServerMode.Queryable;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Printing;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPivotGrid {
	[
	DXToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData),
	Designer("DevExpress.XtraPivotGrid.Design.PivotGridControlDesigner, " + AssemblyInfo.SRAssemblyPivotGridDesign, typeof(IDesigner)),
	ToolboxBitmap(typeof(DevExpress.XtraPivotGrid.PivotGridControl), DevExpress.Utils.ControlConstants.BitmapPath + "PivotGridControl.bmp"),
	Description("Represents data from an underlying data source in a cross-tabulated form; allows data from an OLAP server to be displayed. Supports summaries to be calculated manually and automatically using one of multiple summary functions.")
]
#if DXWhidbey
	[Docking(DockingBehavior.Ask)]
#endif
	[DevExpress.Utils.Design.DataAccess.OLAPDataAccessMetadata("All", SupportedProcessingModes = "Pivot")]
	public class PivotGridControl : DevExpress.XtraPivotGrid.ViewInfo.BaseViewInfoControl, IComponentLoading,
		ISupportLookAndFeel, IPrintable, IToolTipControlClient, IPivotGridEventsImplementor, ICustomizationFormOwner,
		IPivotGridDataOwner, DevExpress.Utils.Menu.IDXManagerPopupMenu, IBindingList, ITypedList, IPivotGrid, IPivotGridViewInfoDataOwner,
		IPivotGridPrinterOwner, IDataContainerBase, IMouseWheelSupport, ISupportXtraSerializer, IThreadSafeAccessible,
		IXtraSerializable, IXtraSerializableLayout, IXtraSerializableLayoutEx, IXtraSupportDeserializeCollectionItem, IXtraSupportDeserializeCollection, IPivotGestureClient, IMouseWheelScrollClient
	{
		protected const string fieldsPropertyName = "Fields", groupsPropertyName = "Groups", formatConditionsPropertyName = "FormatConditions", formatRulesPropertyName = "FormatRules";
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3, LayoutIdOptionsView = 4, LayoutIdColumns = 5, LayoutIdFormatRules = 6;
		class ChartDataEnumerator : IEnumerator {
			PivotGridControl pivotGrid;
			PivotGridControl PivotGrid { get { return pivotGrid; } }
			ICollection Collection { get { return (ICollection)PivotGrid; } }
			IList List { get { return (IList)PivotGrid; } }
			int index;
			int Index { get { return index; } set { index = value; } }
			public ChartDataEnumerator(PivotGridControl pivotGrid) {
				this.pivotGrid = pivotGrid;
			}
			#region IEnumerator Members
			object IEnumerator.Current { get { return List[Index]; } }
			bool IEnumerator.MoveNext() {
				if(!PivotGrid.Data.ChartDataSource.IsChartDataValid) throw new Exception("Invalid enumerator");
				if(Collection.Count == 0 || Index == Collection.Count - 1) return false;
				Index++;
				return true;
			}
			void IEnumerator.Reset() { Index = 0; }
			#endregion
		}
		protected PivotGridViewInfoData fData;
		object dataSource;
		string dataMember;
		string olapConnectionString;
		UserLookAndFeel lookAndFeel;
		internal PivotGridPrinter printer;
		ToolTipController toolTipController;
		PivotGestureHelper pivotGestureHelper;
		bool isEditorReady;
		UserAction userAction;
		MouseWheelScrollHelper mouseHelper;
		static readonly object dataSourceChanged = new object();
		static readonly object beginRefresh = new object();
		static readonly object endRefresh = new object();
		static readonly object gridLayout = new object();
		static readonly object fieldAreaChanging = new object();
		static readonly object customFilterPopupItems = new object();
		static readonly object customFieldValueCells = new object();
		static readonly object customUnboundFieldData = new object();
		static readonly object customSummary = new object();
		static readonly object customFieldSort = new object();
		static readonly object customServerModeSort = new object();
		static readonly object showingCustomizationForm = new object();
		static readonly object showCustomizationForm = new object();
		static readonly object hideCustomizationForm = new object();
		static readonly object layoutUpgrade = new object();
		static readonly object beforeLoadLayout = new object();
		static readonly object groupFilterChanged = new object();
		static readonly object fieldFilterChanged = new object();
		static readonly object fieldFilterChanging = new object();
		static readonly object fieldAreaChanged = new object();
		static readonly object fieldExpandedInFieldGroupChanged = new object();
		static readonly object fieldWidthChanged = new object();
		static readonly object fieldUnboundExpressionChanged = new object();
		static readonly object fieldAreaIndexChanged = new object();
		static readonly object fieldVisibleChanged = new object();
		static readonly object fieldPropertyChanged = new object();
		static readonly object fieldValueDisplayText = new object();
		static readonly object customGroupInterval = new object();
		static readonly object fieldValueImageIndex = new object();
		static readonly object customCellDisplayText = new object();
		static readonly object customCellValue = new object();
		static readonly object cellDoubleClick = new object();
		static readonly object cellClick = new object();
		static readonly object cellSelectionChanged = new object();
		static readonly object focusedCellChanged = new object();
		static readonly object leftTopCellChanged = new object();
		static readonly object customDrawCell = new object();
		static readonly object customAppearance = new object();
		static readonly object customDrawFieldValue = new object();
		static readonly object customDrawFieldHeader = new object();
		static readonly object customDrawFieldHeaderArea = new object();
		static readonly object customDrawEmptyArea = new object();
		static readonly object customRowHeight = new object();
		static readonly object customColumnWidth = new object();
		static readonly object menuItemClick = new object();
		static readonly object popupMenuShowing = new object();
		static readonly object fieldValueCollapsed = new object();
		static readonly object fieldValueExpanded = new object();
		static readonly object fieldValueCollapsing = new object();
		static readonly object fieldValueExpanding = new object();
		static readonly object fieldValueNotExpanded = new object();
		static readonly object fieldTooltipShowing = new object();
		static readonly object prefilterCriteriaChanged = new object();
		static readonly object olapQueryTimeout = new object();
		static readonly object olapException = new object();
		static readonly object queryException = new object();
		static readonly object editValueChanged = new object();
		static readonly object validatingEditor = new object();
		static readonly object invalidValueException = new object();
		static readonly object customEditValue = new object();
		static readonly object showingEditor = new object();
		static readonly object shownEditor = new object();
		static readonly object hiddenEditor = new object();
		static readonly object customCellEdit = new object();
		static readonly object customCellEditForEditing = new object();
		static readonly object customChartDataSourceData = new object();
		static readonly object customChartDataSourceRows = new object();
		static readonly object customExportHeader = new object();
		static readonly object customExportFieldValue = new object();
		static readonly object customExportCell = new object();
		static readonly object exportStarted = new object();
		static readonly object exportFinished = new object();
		static readonly object asyncOperationStarting = new object();
		static readonly object asyncOperationCompleted = new object();
		static readonly object userActionChanged = new object();
		static ImageCollection customizationTreeNodeImages;
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomizationTreeNodeImages")]
#endif
		public static ImageCollection CustomizationTreeNodeImages {
			get {
				if(customizationTreeNodeImages == null)
					customizationTreeNodeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraPivotGrid.Images.FieldsTreeHierarchy.png",
						typeof(PivotGridControl).Assembly, new Size(16, 16));
				return customizationTreeNodeImages;
			}
			set {
				customizationTreeNodeImages = value;
			}
		}
		static PivotEndUpdateMode endUpdateMode = PivotEndUpdateMode.Invalidate;
		[
		DefaultValue(PivotEndUpdateMode.Invalidate),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlEndUpdateMode")
#else
	Description("")
#endif
		]
		public static PivotEndUpdateMode EndUpdateMode {
			get { return endUpdateMode; }
			set { endUpdateMode = value; }
		}
		public static void About() {
			DevExpress.Utils.About.AboutHelper.Show(DevExpress.Utils.About.ProductKind.DXperienceWin, new DevExpress.Utils.About.ProductStringInfoWin(DevExpress.Utils.About.ProductInfoHelper.WinPivotGrid));
		}
		public PivotGridControl()
			: base() {
			InitializeCore(null);
		}
		protected PivotGridControl(PivotGridViewInfoData viewInfoData)
			: base() {
			InitializeCore(viewInfoData);
		}
		void InitializeCore(PivotGridViewInfoData viewInfoData) {
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.Selectable, true);
			if(viewInfoData == null) {
				fData = CreateData();
				OptionsView.ShowAllTotals();
			}
			else
				fData = viewInfoData;
			this.dataSource = null;
			this.dataMember = string.Empty;
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(OnLookAndFeel_StyleChanged);
			fData.ControlLookAndFeel = lookAndFeel;
			HScrollBar.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			VScrollBar.LookAndFeel.ParentLookAndFeel = this.lookAndFeel;
			this.printer = CreatePrinter();
			AddToDefaultTooltipController();
			this.toolTipController = null;
			this.editorContainer = new PivotGridEditorContainer();
			this.editorHelper = EditorContainer.EditorHelper;
			EditorHelper.Pivot = this;
			this.isEditorReady = true;
			this.pivotGestureHelper = new PivotGestureHelper(this);
			this.mouseHelper = new MouseWheelScrollHelper(this);
			Data.ChartDataSource.ListChanged += OnChartDataSourceListChanged;
			this.RightToLeftChanged += (s, e) => {
				Data.LookAndFeelChanged();
				ViewInfo.ResetPaintBounds();
				ViewInfo.CellsArea.CalculateCellsViewInfo();
			};
#if DEBUGTEST
			DevExpress.XtraPivotGrid.Tests.SafeFixture.AddDisposable(this);
#endif
		}
		PivotGestureHelper PivotGestureHelper { get { return pivotGestureHelper; } }
		protected virtual void AddToDefaultTooltipController() {
			ToolTipController.DefaultController.AddClientControl(this);
		}
		protected virtual void RemoveFromDefaultTooltipController() {
			ToolTipController.DefaultController.RemoveClientControl(this);
		}
		protected override void Dispose(bool disposing) {
			if(IsDisposed) return;
			if(disposing) {
				DestroyCustomization();
				if(this.lookAndFeel != null) {
					this.lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeel_StyleChanged);
					this.lookAndFeel.Dispose();
					this.lookAndFeel = null;
				}
				if(printer != null) {
					this.printer.Dispose();
					this.printer = null;
				}
				DisposeEdit();
				DisposeData();
				ToolTipController = null;
				RemoveFromDefaultTooltipController();
			}
			base.Dispose(disposing);
		}
		protected void DisposeEdit() {
			HideEditor();
			if(this.editorContainer != null) {
				this.editorContainer.Dispose();
				this.editorContainer = null;
			}
		}
		protected virtual void DisposeData() {
			if(this.fData != null) {
				this.fData.ChartDataSource.ListChanged -= OnChartDataSourceListChanged;
				DisposeDataCore();
			}
		}
		protected virtual void DisposeDataCore() {
			this.fData.Dispose();
			this.fData = null;
		}
		protected virtual PivotGridViewInfoData CreateData() {
			return new PivotGridViewInfoData(this);
		}
		protected internal PivotGridViewInfoData CreateEmptyData() {
			return CreateData();
		}
		public override void BeginUpdate() {
			base.BeginUpdate();
			Data.BeginUpdate();
		}
		public new virtual void EndUpdate() {
			base.EndUpdate();
		}
		public bool IsUpdateLocked { get { return Data.IsLockUpdate; } }
		protected override void CancelUpdate() {
			base.CancelUpdate();
			Data.EndUpdate();
		}
		protected override Rectangle ScrollableRectangle { get { return ViewInfo.ScrollableBounds; } }
		public void LayoutChanged() {
			Data.LayoutChanged();
		}
		bool useDisabledStatePainterCore = true;
		[
		DefaultValue(true), 
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlUseDisabledStatePainter"),
#endif
		Category("Appearance")
		]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainterCore; }
			set {
				if(useDisabledStatePainterCore != value) {
					useDisabledStatePainterCore = value;
					Invalidate();
				}
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlDataSource"),
#endif
 Category("Data"),
#if DXWhidbey
 AttributeProvider(typeof(IListSource)),
#else
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
#endif
 DefaultValue(null), Localizable(true)
		]
		public object DataSource {
			get { return dataSource; }
			set {
				if(value == this) return;
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(!IsValidDataSource(value))
					return;
				EnforceValidDataMember(value);
				SetDataSource(value);
			}
		}
		protected void EnforceValidDataMember(object dataSource) {
			if(dataSource == null)
				this.dataMember = "";
			if(DataMember == "" || BindingContext == null || dataSource == null || (DesignMode && IsLoading) || IsLoading)
				return;
			try {
				BindingManagerBase bm = BindingContext[dataSource, DataMember];
			}
			catch {
				this.dataMember = "";
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlDataMember"),
#endif
 Category("Data"), Localizable(true), DefaultValue(""),
		Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string DataMember {
			get { return dataMember; }
			set {
				if(DataMember == value) return;
				dataMember = value;
				ActivateDataSource();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOLAPConnectionString"),
#endif
 Category("Data"), Localizable(true), DefaultValue(null),
		Editor("DevExpress.XtraPivotGrid.Design.OLAPConnectionEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string OLAPConnectionString {
			get { return olapConnectionString; }
			set {
				if(olapConnectionString != value) {
					olapConnectionString = value;
					isDataSourceActive = false;
					if(!String.IsNullOrEmpty(value)) DataSource = null;
					EnsureDataSourceIsActive();
				}
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOLAPDataProvider"),
#endif
 Category("Data"), Localizable(true), DefaultValue(OLAPDataProvider.Default),
		]
		public OLAPDataProvider OLAPDataProvider {
			get { return Data.OLAPDataProvider; }
			set { Data.OLAPDataProvider = value; }
		}
		[Browsable(false)]
		public bool IsOLAPDataSource { get { return !String.IsNullOrEmpty(OLAPConnectionString); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList ListSource { get { return Data.ListSource; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }
		[Category("Data"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridCells Cells {
			get {
				EnsureViewInfoIsCalculated();
				return Data.Cells;
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlBorderStyle"),
#endif
 Category("Appearance"), DefaultValue(BorderStyles.Default)]
		public BorderStyles BorderStyle { get { return Data.BorderStyle; } set { Data.BorderStyle = value; } }
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		void ResetLookAndFeel() { LookAndFeel.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public PivotGridAppearances Appearance { get { return Data.Appearance; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridAppearances PaintAppearance { get { return Data.PaintAppearance; } }
		void ResetAppearancePrint() { AppearancePrint.Reset(); }
		bool ShouldSerializeAppearancePrint() { return AppearancePrint.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlAppearancePrint"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public virtual PivotGridAppearancesPrint AppearancePrint { get { return Data.AppearancePrint; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual PivotGridAppearancesPrint PaintAppearancePrint { get { return Data.PaintAppearancePrint; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFields"),
#endif
 Browsable(true),
		Editor("DevExpress.XtraPivotGrid.Design.FieldsCollectionEditor, " + AssemblyInfo.SRAssemblyPivotGridDesign,
			typeof(System.Drawing.Design.UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdColumns)
		]
		public PivotGridFieldCollection Fields { get { return Data.Fields; } }
		public PivotGridField GetFieldAt(Point pt) {
			return Data.GetField(ViewInfo.GetFieldAt(pt)) as PivotGridField;
		}
		public List<PivotGridField> GetFieldsByArea(PivotArea area) {
			List<PivotGridFieldBase> baseList = Data.GetFieldsByArea(area, false);
			List<PivotGridField> res = new List<PivotGridField>(baseList.Count);
			for(int i = 0; i < baseList.Count; i++)
				res.Add((PivotGridField)baseList[i]);
			return res;
		}
		public PivotGridField GetFieldByArea(PivotArea area, int areaIndex) {
			return (PivotGridField)Data.GetFieldByArea(area, areaIndex);
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1000, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdFormatRules)
		]
		public virtual PivotGridFormatRuleCollection FormatRules { get { return Data.FormatRules; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFormatConditions"),
#endif
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 1000, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance),
		Browsable(false)]
		public PivotGridFormatConditionCollection FormatConditions { get { return Data.FormatConditions; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 100),
		XtraSerializablePropertyId(LayoutIdLayout)
		]
		public PivotGridGroupCollection Groups { get { return Data.Groups; } }
		bool ShouldSerializePrefilter() { return this.Prefilter.ShouldSerialize(); }
		void ResetPrefilter() { this.Prefilter.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlPrefilter"),
#endif
 Category("Data"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public Prefilter Prefilter { get { return Data.Prefilter; } }
		bool ShouldSerializeOptionsBehavior() { return OptionsBehavior.ShouldSerialize(); }
		void ResetOptionsBehavior() { OptionsBehavior.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsBehavior"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsBehavior OptionsBehavior { get { return Data.OptionsBehavior; } }
		bool ShouldSerializeOptionsFilterPopup() { return OptionsFilterPopup.ShouldSerialize(); }
		void ResetOptionsFilterPopup() { OptionsFilterPopup.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsFilterPopup"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsFilterPopup OptionsFilterPopup { get { return Data.OptionsFilterPopup; } }
		bool ShouldSerializeOptionsCustomization() { return OptionsCustomization.ShouldSerialize(); }
		void ResetOptionsCustomization() { OptionsCustomization.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsCustomization"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsCustomizationEx OptionsCustomization { get { return Data.OptionsCustomization; } }
		bool ShouldSerializeOptionsDataField() { return OptionsDataField.ShouldSerialize(); }
		void ResetOptionsDataField() { OptionsDataField.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsDataField"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsDataField OptionsDataField { get { return Data.OptionsDataField; } }
		bool ShouldSerializeOptionsData() { return OptionsData.ShouldSerialize(); }
		void ResetOptionsData() { OptionsData.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsData"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsData OptionsData { get { return Data.OptionsData; } }
		bool ShouldSerializeOptionsChartDataSource() { return OptionsChartDataSource.ShouldSerialize(); }
		void ResetOptionsChartDataSource() { OptionsChartDataSource.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsChartDataSource"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsChartDataSource OptionsChartDataSource { get { return Data.OptionsChartDataSource; } }
		PivotGridOptionsLayout optionsLayout;
		protected virtual PivotGridOptionsLayout CreateOptionsLayout() {
			return new PivotGridOptionsLayout();
		}
		bool ShouldSerializeOptionsLayout() { return OptionsLayout.ShouldSerialize(); }
		void ResetOptionsLayout() { OptionsLayout.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsLayout"),
#endif
 Category("Options"),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue, -1),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PivotGridOptionsLayout OptionsLayout {
			get {
				if(optionsLayout == null)
					optionsLayout = CreateOptionsLayout();
				return optionsLayout;
			}
		}
		bool ShouldSerializeOptionsHint() { return OptionsHint.ShouldSerialize(); }
		void ResetOptionsHint() { OptionsHint.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsHint"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsHint OptionsHint { get { return Data.OptionsHint; } }
		bool ShouldSerializeOptionsMenu() { return OptionsMenu.ShouldSerialize(); }
		void ResetOptionsMenu() { OptionsMenu.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsMenu"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsMenu OptionsMenu { get { return Data.OptionsMenu; } }
		bool ShouldSerializeOptionsSelection() { return OptionsSelection.ShouldSerialize(); }
		void ResetOptionsSelection() { OptionsSelection.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsSelection"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsSelection OptionsSelection { get { return Data.OptionsSelection; } }
		bool ShouldSerializeOptionsPrint() { return OptionsPrint.ShouldSerialize(); }
		void ResetOptionsPrint() { OptionsPrint.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsPrint"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public PivotGridOptionsPrint OptionsPrint { get { return Data.OptionsPrint; } }
		bool ShouldSerializeOptionsView() { return OptionsView.ShouldSerialize(); }
		void ResetOptionsView() { OptionsView.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsView"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public virtual PivotGridOptionsView OptionsView { get { return Data.OptionsView; } }
		bool ShouldSerializeOptionsOLAP() { return OptionsOLAP.ShouldSerialize(); }
		void ResetOptionsOLAP() { OptionsOLAP.Reset(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOptionsOLAP"),
#endif
 Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public virtual PivotGridOptionsOLAP OptionsOLAP { get { return Data.OptionsOLAP; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlMenuManager"),
#endif
 DefaultValue(null)]
		public IDXMenuManager MenuManager { get { return Data.MenuManager; } set { Data.MenuManager = value; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlDataSourceChanged"),
#endif
 Category("Data")]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(dataSourceChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlBeginRefresh"),
#endif
 Category("Data")]
		public event EventHandler BeginRefresh {
			add { this.Events.AddHandler(beginRefresh, value); }
			remove { this.Events.RemoveHandler(beginRefresh, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlEndRefresh"),
#endif
 Category("Data")]
		public event EventHandler EndRefresh {
			add { this.Events.AddHandler(endRefresh, value); }
			remove { this.Events.RemoveHandler(endRefresh, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlGridLayout"),
#endif
 Category("Layout")]
		public event EventHandler GridLayout {
			add { this.Events.AddHandler(gridLayout, value); }
			remove { this.Events.RemoveHandler(gridLayout, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldAreaChanging"),
#endif
 Category("Layout")]
		public event PivotAreaChangingEventHandler FieldAreaChanging {
			add { this.Events.AddHandler(fieldAreaChanging, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanging, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomFilterPopupItems"),
#endif
 Category("Data")]
		public event PivotCustomFilterPopupItemsEventHandler CustomFilterPopupItems {
			add { this.Events.AddHandler(customFilterPopupItems, value); }
			remove { this.Events.RemoveHandler(customFilterPopupItems, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomFieldValueCells"),
#endif
 Category("Data")]
		public event PivotCustomFieldValueCellsEventHandler CustomFieldValueCells {
			add { this.Events.AddHandler(customFieldValueCells, value); }
			remove { this.Events.RemoveHandler(customFieldValueCells, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomUnboundFieldData"),
#endif
 Category("Data")]
		public event CustomFieldDataEventHandler CustomUnboundFieldData {
			add { this.Events.AddHandler(customUnboundFieldData, value); }
			remove { this.Events.RemoveHandler(customUnboundFieldData, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomSummary"),
#endif
 Category("Data")]
		public event PivotGridCustomSummaryEventHandler CustomSummary {
			add { this.Events.AddHandler(customSummary, value); }
			remove { this.Events.RemoveHandler(customSummary, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomFieldSort"),
#endif
 Category("Data")]
		public event PivotGridCustomFieldSortEventHandler CustomFieldSort {
			add { this.Events.AddHandler(customFieldSort, value); }
			remove { this.Events.RemoveHandler(customFieldSort, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomServerModeSort"),
#endif
 Category("Data")]		
		public event EventHandler<CustomServerModeSortEventArgs> CustomServerModeSort {
			add { this.Events.AddHandler(customServerModeSort, value); }
			remove { this.Events.RemoveHandler(customServerModeSort, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlShowingCustomizationForm"),
#endif
 Category("Customization")]
		public event CustomizationFormShowingEventHandler ShowingCustomizationForm {
			add { this.Events.AddHandler(showingCustomizationForm, value); }
			remove { this.Events.RemoveHandler(showingCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlShowCustomizationForm"),
#endif
 Category("Customization")]
		public event EventHandler ShowCustomizationForm {
			add { this.Events.AddHandler(showCustomizationForm, value); }
			remove { this.Events.RemoveHandler(showCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlHideCustomizationForm"),
#endif
 Category("Customization")]
		public event EventHandler HideCustomizationForm {
			add { this.Events.AddHandler(hideCustomizationForm, value); }
			remove { this.Events.RemoveHandler(hideCustomizationForm, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlLayoutUpgrade"),
#endif
 Category("Customization")]
		public event LayoutUpgradeEventHandler LayoutUpgrade {
			add { this.Events.AddHandler(layoutUpgrade, value); }
			remove { this.Events.RemoveHandler(layoutUpgrade, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlBeforeLoadLayout"),
#endif
 Category("Customization")]
		public event LayoutAllowEventHandler BeforeLoadLayout {
			add { this.Events.AddHandler(beforeLoadLayout, value); }
			remove { this.Events.RemoveHandler(beforeLoadLayout, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlGroupFilterChanged"),
#endif
 Category("Data")]
		public event PivotGroupEventHandler GroupFilterChanged {
			add { this.Events.AddHandler(groupFilterChanged, value); }
			remove { this.Events.RemoveHandler(groupFilterChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldFilterChanged"),
#endif
 Category("Data")]
		public event PivotFieldEventHandler FieldFilterChanged {
			add { this.Events.AddHandler(fieldFilterChanged, value); }
			remove { this.Events.RemoveHandler(fieldFilterChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldFilterChanging"),
#endif
 Category("Data")]
		public event PivotFieldFilterChangingEventHandler FieldFilterChanging {
			add { this.Events.AddHandler(fieldFilterChanging, value); }
			remove { this.Events.RemoveHandler(fieldFilterChanging, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldAreaChanged"),
#endif
 Category("Layout")]
		public event PivotFieldEventHandler FieldAreaChanged {
			add { this.Events.AddHandler(fieldAreaChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldExpandedInFieldGroupChanged"),
#endif
 Category("Layout")]
		public event PivotFieldEventHandler FieldExpandedInFieldGroupChanged {
			add { this.Events.AddHandler(fieldExpandedInFieldGroupChanged, value); }
			remove { this.Events.RemoveHandler(fieldExpandedInFieldGroupChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldWidthChanged"),
#endif
 Category("Layout")]
		public event PivotFieldEventHandler FieldWidthChanged {
			add { this.Events.AddHandler(fieldWidthChanged, value); }
			remove { this.Events.RemoveHandler(fieldWidthChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldVisibleChanged"),
#endif
 Category("Layout")]
		public event PivotFieldEventHandler FieldVisibleChanged {
			add { this.Events.AddHandler(fieldVisibleChanged, value); }
			remove { this.Events.RemoveHandler(fieldVisibleChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldAreaIndexChanged"),
#endif
 Category("Layout")]
		public event PivotFieldEventHandler FieldAreaIndexChanged {
			add { this.Events.AddHandler(fieldAreaIndexChanged, value); }
			remove { this.Events.RemoveHandler(fieldAreaIndexChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldUnboundExpressionChanged"),
#endif
 Category("Layout")]
		public event PivotFieldEventHandler FieldUnboundExpressionChanged {
			add { this.Events.AddHandler(fieldUnboundExpressionChanged, value); }
			remove { this.Events.RemoveHandler(fieldUnboundExpressionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldPropertyChanged"),
#endif
 Category("Data")]
		public event PivotFieldPropertyChangedEventHandler FieldPropertyChanged {
			add { this.Events.AddHandler(fieldPropertyChanged, value); }
			remove { this.Events.RemoveHandler(fieldPropertyChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldValueDisplayText"),
#endif
 Category("Data")]
		public event PivotFieldDisplayTextEventHandler FieldValueDisplayText {
			add { this.Events.AddHandler(fieldValueDisplayText, value); }
			remove { this.Events.RemoveHandler(fieldValueDisplayText, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomGroupInterval"),
#endif
 Category("Data")]
		public event PivotCustomGroupIntervalEventHandler CustomGroupInterval {
			add { this.Events.AddHandler(customGroupInterval, value); }
			remove { this.Events.RemoveHandler(customGroupInterval, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomChartDataSourceData"),
#endif
 Category("Data")]
		public event PivotCustomChartDataSourceDataEventHandler CustomChartDataSourceData {
			add { this.Events.AddHandler(customChartDataSourceData, value); }
			remove { this.Events.RemoveHandler(customChartDataSourceData, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomChartDataSourceRows"),
#endif
 Category("Data")]
		public event PivotCustomChartDataSourceRowsEventHandler CustomChartDataSourceRows {
			add { this.Events.AddHandler(customChartDataSourceRows, value); }
			remove { this.Events.RemoveHandler(customChartDataSourceRows, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldValueImageIndex"),
#endif
 Category("Appearance")]
		public event PivotFieldImageIndexEventHandler FieldValueImageIndex {
			add { this.Events.AddHandler(fieldValueImageIndex, value); }
			remove { this.Events.RemoveHandler(fieldValueImageIndex, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomCellDisplayText"),
#endif
 Category("Data")]
		public event PivotCellDisplayTextEventHandler CustomCellDisplayText {
			add { this.Events.AddHandler(customCellDisplayText, value); }
			remove { this.Events.RemoveHandler(customCellDisplayText, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomCellValue"),
#endif
 Category("Data")]
		public event EventHandler<PivotCellValueEventArgs> CustomCellValue {
			add { this.Events.AddHandler(customCellValue, value); }
			remove { this.Events.RemoveHandler(customCellValue, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCellDoubleClick"),
#endif
 Category("Behavior")]
		public event PivotCellEventHandler CellDoubleClick {
			add { this.Events.AddHandler(cellDoubleClick, value); }
			remove { this.Events.RemoveHandler(cellDoubleClick, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCellClick"),
#endif
 Category("Behavior")]
		public event PivotCellEventHandler CellClick {
			add { this.Events.AddHandler(cellClick, value); }
			remove { this.Events.RemoveHandler(cellClick, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFocusedCellChanged"),
#endif
 Category("Behavior")]
		public event EventHandler FocusedCellChanged {
			add { this.Events.AddHandler(focusedCellChanged, value); }
			remove { this.Events.RemoveHandler(focusedCellChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlLeftTopCellChanged"),
#endif
 Category("Behavior")]
		public event EventHandler<PivotLeftTopCellChangedEventArgs> LeftTopCellChanged {
			add { this.Events.AddHandler(leftTopCellChanged, value); }
			remove { this.Events.RemoveHandler(leftTopCellChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCellSelectionChanged"),
#endif
 Category("Behavior")]
		public event EventHandler CellSelectionChanged {
			add { this.Events.AddHandler(cellSelectionChanged, value); }
			remove { this.Events.RemoveHandler(cellSelectionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlPopupMenuShowing"),
#endif
 Category("Behavior")]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { this.Events.AddHandler(popupMenuShowing, value); }
			remove { this.Events.RemoveHandler(popupMenuShowing, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlMenuItemClick"),
#endif
 Category("Behavior")]
		public event PivotGridMenuItemClickEventHandler MenuItemClick {
			add { this.Events.AddHandler(menuItemClick, value); }
			remove { this.Events.RemoveHandler(menuItemClick, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomDrawFieldHeaderArea"),
#endif
 Category("Appearance")]
		public event PivotCustomDrawHeaderAreaEventHandler CustomDrawFieldHeaderArea {
			add { this.Events.AddHandler(customDrawFieldHeaderArea, value); }
			remove { this.Events.RemoveHandler(customDrawFieldHeaderArea, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomDrawEmptyArea"),
#endif
 Category("Appearance")]
		public event PivotCustomDrawEventHandler CustomDrawEmptyArea {
			add { this.Events.AddHandler(customDrawEmptyArea, value); }
			remove { this.Events.RemoveHandler(customDrawEmptyArea, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomDrawCell"),
#endif
 Category("Appearance")]
		public event PivotCustomDrawCellEventHandler CustomDrawCell {
			add { this.Events.AddHandler(customDrawCell, value); }
			remove { this.Events.RemoveHandler(customDrawCell, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomAppearance"),
#endif
 Category("Appearance")]
		public event PivotCustomAppearanceEventHandler CustomAppearance {
			add { this.Events.AddHandler(customAppearance, value); }
			remove { this.Events.RemoveHandler(customAppearance, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomDrawFieldHeader"),
#endif
 Category("Appearance")]
		public event PivotCustomDrawFieldHeaderEventHandler CustomDrawFieldHeader {
			add { this.Events.AddHandler(customDrawFieldHeader, value); }
			remove { this.Events.RemoveHandler(customDrawFieldHeader, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomDrawFieldValue"),
#endif
 Category("Appearance")]
		public event PivotCustomDrawFieldValueEventHandler CustomDrawFieldValue {
			add { this.Events.AddHandler(customDrawFieldValue, value); }
			remove { this.Events.RemoveHandler(customDrawFieldValue, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomRowHeight"),
#endif
 Category("Appearance")]
		public event EventHandler<PivotCustomRowHeightEventArgs> CustomRowHeight {
			add { this.Events.AddHandler(customRowHeight, value); }
			remove { this.Events.RemoveHandler(customRowHeight, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomColumnWidth"),
#endif
 Category("Appearance")]
		public event EventHandler<PivotCustomColumnWidthEventArgs> CustomColumnWidth {
			add { this.Events.AddHandler(customColumnWidth, value); }
			remove { this.Events.RemoveHandler(customColumnWidth, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldValueCollapsed"),
#endif
 Category("Behavior")]
		public event PivotFieldValueEventHandler FieldValueCollapsed {
			add { this.Events.AddHandler(fieldValueCollapsed, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsed, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldValueExpanded"),
#endif
 Category("Behavior")]
		public event PivotFieldValueEventHandler FieldValueExpanded {
			add { this.Events.AddHandler(fieldValueExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanded, value); }
		}
		[ Category("Behavior")]
		public event PivotFieldValueEventHandler FieldValueNotExpanded {
			add { this.Events.AddHandler(fieldValueNotExpanded, value); }
			remove { this.Events.RemoveHandler(fieldValueNotExpanded, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldValueCollapsing"),
#endif
 Category("Behavior")]
		public event PivotFieldValueCancelEventHandler FieldValueCollapsing {
			add { this.Events.AddHandler(fieldValueCollapsing, value); }
			remove { this.Events.RemoveHandler(fieldValueCollapsing, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldValueExpanding"),
#endif
 Category("Behavior")]
		public event PivotFieldValueCancelEventHandler FieldValueExpanding {
			add { this.Events.AddHandler(fieldValueExpanding, value); }
			remove { this.Events.RemoveHandler(fieldValueExpanding, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlFieldTooltipShowing"),
#endif
 Category("Behavior")]
		public event PivotFieldTooltipShowingEventHandler FieldTooltipShowing {
			add { this.Events.AddHandler(fieldTooltipShowing, value); }
			remove { this.Events.RemoveHandler(fieldTooltipShowing, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlPrefilterCriteriaChanged"),
#endif
 Category("Data")]
		public event EventHandler PrefilterCriteriaChanged {
			add { this.Events.AddHandler(prefilterCriteriaChanged, value); }
			remove { this.Events.RemoveHandler(prefilterCriteriaChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOLAPQueryTimeout"),
#endif
 Category("Data")]
		public event EventHandler OLAPQueryTimeout {
			add { this.Events.AddHandler(olapQueryTimeout, value); }
			remove { this.Events.RemoveHandler(olapQueryTimeout, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlOLAPException"),
#endif
 Category("Data"), Browsable(false), Obsolete("The OLAPException event is obsolete now. Use the QueryException event instead.")]
		public event PivotOlapExceptionEventHandler OLAPException {
			add { this.Events.AddHandler(olapException, value); }
			remove { this.Events.RemoveHandler(olapException, value); }
		}
		[ Category("Data")]
		public event PivotQueryExceptionEventHandler QueryException {
			add { this.Events.AddHandler(queryException, value); }
			remove { this.Events.RemoveHandler(queryException, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlEditValueChanged"),
#endif
 Category("Editor")]
		public event EditValueChangedEventHandler EditValueChanged {
			add { this.Events.AddHandler(editValueChanged, value); }
			remove { this.Events.RemoveHandler(editValueChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlValidatingEditor"),
#endif
 Category("Editor")]
		public event BaseContainerValidateEditorEventHandler ValidatingEditor {
			add { this.Events.AddHandler(validatingEditor, value); }
			remove { this.Events.RemoveHandler(validatingEditor, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlInvalidValueException"),
#endif
 Category("Editor")]
		public event InvalidValueExceptionEventHandler InvalidValueException {
			add { this.Events.AddHandler(invalidValueException, value); }
			remove { this.Events.RemoveHandler(invalidValueException, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomEditValue"),
#endif
 Category("Editor")]
		public event CustomEditValueEventHandler CustomEditValue {
			add { this.Events.AddHandler(customEditValue, value); }
			remove { this.Events.RemoveHandler(customEditValue, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlShowingEditor"),
#endif
 Category("Editor")]
		public event EventHandler<CancelPivotCellEditEventArgs> ShowingEditor {
			add { this.Events.AddHandler(showingEditor, value); }
			remove { this.Events.RemoveHandler(showingEditor, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlShownEditor"),
#endif
 Category("Editor")]
		public event EventHandler<PivotCellEditEventArgs> ShownEditor {
			add { this.Events.AddHandler(shownEditor, value); }
			remove { this.Events.RemoveHandler(shownEditor, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlHiddenEditor"),
#endif
 Category("Editor")]
		public event EventHandler<PivotCellEditEventArgs> HiddenEditor {
			add { this.Events.AddHandler(hiddenEditor, value); }
			remove { this.Events.RemoveHandler(hiddenEditor, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomCellEdit"),
#endif
 Category("Editor")]
		public event EventHandler<PivotCustomCellEditEventArgs> CustomCellEdit {
			add { this.Events.AddHandler(customCellEdit, value); }
			remove { this.Events.RemoveHandler(customCellEdit, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomCellEditForEditing"),
#endif
 Category("Editor")]
		public event EventHandler<PivotCustomCellEditEventArgs> CustomCellEditForEditing {
			add { this.Events.AddHandler(customCellEditForEditing, value); }
			remove { this.Events.RemoveHandler(customCellEditForEditing, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomExportHeader"),
#endif
 Category("Export & Printing")]
		public event EventHandler<CustomExportHeaderEventArgs> CustomExportHeader {
			add { this.Events.AddHandler(customExportHeader, value); }
			remove { this.Events.RemoveHandler(customExportHeader, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomExportFieldValue"),
#endif
 Category("Export & Printing")]
		public event EventHandler<CustomExportFieldValueEventArgs> CustomExportFieldValue {
			add { this.Events.AddHandler(customExportFieldValue, value); }
			remove { this.Events.RemoveHandler(customExportFieldValue, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlCustomExportCell"),
#endif
 Category("Export & Printing")]
		public event EventHandler<CustomExportCellEventArgs> CustomExportCell {
			add { this.Events.AddHandler(customExportCell, value); }
			remove { this.Events.RemoveHandler(customExportCell, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlExportStarted"),
#endif
 Category("Export & Printing")]
		public event EventHandler ExportStarted {
			add { this.Events.AddHandler(exportStarted, value); }
			remove { this.Events.RemoveHandler(exportStarted, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlExportFinished"),
#endif
 Category("Export & Printing")]
		public event EventHandler ExportFinished {
			add { this.Events.AddHandler(exportFinished, value); }
			remove { this.Events.RemoveHandler(exportFinished, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlAsyncOperationStarting"),
#endif
 Category("Async")]
		public event EventHandler AsyncOperationStarting {
			add { this.Events.AddHandler(asyncOperationStarting, value); }
			remove { this.Events.RemoveHandler(asyncOperationStarting, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlAsyncOperationCompleted"),
#endif
 Category("Async")]
		public event EventHandler AsyncOperationCompleted {
			add { this.Events.AddHandler(asyncOperationCompleted, value); }
			remove { this.Events.RemoveHandler(asyncOperationCompleted, value); }
		}
		[ Category("Async")]
		public event PivotUserActionEventHandler UserActionChanged {
			add { this.Events.AddHandler(userActionChanged, value); }
			remove { this.Events.RemoveHandler(userActionChanged, value); }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlHeaderImages"),
#endif
 Category("Appearance"), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object HeaderImages { get { return Data.HeaderImages; } set { Data.HeaderImages = value; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlValueImages"),
#endif
 Category("Appearance"), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ValueImages { get { return Data.ValueImages; } set { Data.ValueImages = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Text { get { return base.Text; } set { base.Text = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[Browsable(false)]
		public UserAction UserAction {
			get { return userAction; }
			internal set {
				if(userAction == value)
					return;
				userAction = value;
				RaiseUserAction();
			}
		}
		bool IsQueryableDataSource {
			get {
				if(!(DataSource is ILinqServerModeFrontEndOwner))
					return false;
				IListSource listSource = DataSource as IListSource;
				if(listSource == null)
					return false;
				return listSource.GetList() is IBindingList;
			}
		}
		IQueryable Queryable {
			get {
				ILinqServerModeFrontEndOwner qdatasource = ((ILinqServerModeFrontEndOwner)DataSource);
				if(qdatasource.QueryableSource != null)
					return qdatasource.QueryableSource;
				if(DesignMode && qdatasource.ElementType != null)
					return ((IEnumerable)Activator.CreateInstance(typeof(List<>).MakeGenericType(qdatasource.ElementType))).AsQueryable();
				return null;
			}
		}
		IBindingList QueryableDataSourceBindingList { get { return (IBindingList)(((IListSource)DataSource).GetList()); } }
		public void BestFit(PivotGridField field) {
			if(ViewInfo != null)
				ViewInfo.BestFit(Data.GetFieldItem(field) as PivotFieldItem);
		}
		public void BestFit() {
			if(ViewInfo != null)
				ViewInfo.BestFit();
		}
		public void BestFitColumnArea() {
			if(ViewInfo != null)
				ViewInfo.BestFitColumnArea();
		}
		public void BestFitRowArea() {
			if(ViewInfo != null)
				ViewInfo.BestFitRowArea();
		}
		public void BestFitDataHeaders(bool considerRowArea) {
			if(ViewInfo != null)
				ViewInfo.BestFitDataHeaders(considerRowArea);
		}
		public PivotGridHitInfo CalcHitInfo(Point hitPoint) { return ViewInfo.CalcHitInfo(hitPoint); }
		public void RefreshData() {
			Data.ReloadData();
		}
		public void RetrieveFields() {
			Data.RetrieveFields();
		}
		public void RetrieveFields(PivotArea area, bool visible) {
			Data.RetrieveFields(area, visible);
		}
		public void FieldsCustomization(Point showPoint) {
			Data.FieldsCustomization(showPoint);
		}
		public void FieldsCustomization() {
			Data.FieldsCustomization();
		}
		public void FieldsCustomization(Control parentControl) {
			Data.FieldsCustomization(parentControl);
		}
		public void DestroyCustomization() {
			Data.DestroyCustomization();
		}
		public void ShowCustomization() {
			Data.FieldsCustomization();
		}
		public void HideCustomization() {
			DestroyCustomization();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle CustomizationFormBounds { get { return Data.CustomizationFormBounds; } set { Data.CustomizationFormBounds = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CustomizationForm CustomizationForm { get { return Data.CustomizationForm; } }
		[Browsable(false)]
		public bool CanResizeField(Point pt) {
			if(!ViewInfo.IsReady) return false;
			return ViewInfo.GetSizingField(pt.X, pt.Y) != null;
		}
		public void SaveLayoutToXml(string xmlFile) { SaveLayoutToXml(xmlFile, OptionsLayout); }
		public void RestoreLayoutFromXml(string xmlFile) { RestoreLayoutFromXml(xmlFile, OptionsLayout); }
		public void SaveLayoutToRegistry(string path) { SaveLayoutToRegistry(path, OptionsLayout); }
		public void RestoreLayoutFromRegistry(string path) { RestoreLayoutFromRegistry(path, OptionsLayout); }
		public void SaveLayoutToStream(Stream stream) { SaveLayoutToStream(stream, OptionsLayout); }
		public void RestoreLayoutFromStream(Stream stream) { RestoreLayoutFromStream(stream, OptionsLayout); }
		public void SaveLayoutToXml(string xmlFile, OptionsLayoutBase options) {
			SaveLayoutCore(Data.CreateXmlXtraSerializer(), xmlFile, options);
		}
		public void RestoreLayoutFromXml(string xmlFile, OptionsLayoutBase options) {
			RestoreLayoutCore(Data.CreateXmlXtraSerializer(), xmlFile, options);
		}
		public void SaveLayoutToRegistry(string path, OptionsLayoutBase options) {
			SaveLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public void RestoreLayoutFromRegistry(string path, OptionsLayoutBase options) {
			RestoreLayoutCore(new RegistryXtraSerializer(), path, options);
		}
		public void SaveLayoutToStream(Stream stream, OptionsLayoutBase options) {
			SaveLayoutCore(Data.CreateXmlXtraSerializer(), stream, options);
		}
		public void RestoreLayoutFromStream(Stream stream, OptionsLayoutBase options) {
			RestoreLayoutCore(Data.CreateXmlXtraSerializer(), stream, options);
		}
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			serializer.SerializeObject(this, path, Data.AppName, options);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path, OptionsLayoutBase options) {
			serializer.DeserializeObject(this, path, Data.AppName, options);
		}
		public void SaveCollapsedStateToStream(Stream stream) {
			Data.SaveCollapsedStateToStream(stream);
		}
		public void SaveCollapsedStateToFile(string path) {
			using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				SaveCollapsedStateToStream(stream);
			}
		}
		public void LoadCollapsedStateFromStream(Stream stream) {
			Data.LoadCollapsedStateFromStream(stream);
			LayoutChanged();
		}
		public void LoadCollapsedStateFromFile(string path) {
			FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			LoadCollapsedStateFromStream(stream);
			stream.Close();
		}
		public void SavePivotGridToFile(string path) {
			SavePivotGridToFile(path, false);
		}
		public void SavePivotGridToFile(string path, bool compress) {
			Data.SavePivotGridToFile(path, compress);
		}
		public void SavePivotGridToStream(Stream stream) {
			SavePivotGridToStream(stream, false);
		}
		public void SavePivotGridToStream(Stream stream, bool compress) {
			Data.SavePivotGridToStream(stream, compress);
		}
		public void CollapseAll() {
			Data.ChangeExpandedAll(false);
		}
		public void CollapseAllRows() {
			Data.ChangeExpandedAll(false, false);
		}
		public void CollapseAllColumns() {
			Data.ChangeExpandedAll(true, false);
		}
		public void ExpandAll() {
			Data.ChangeExpandedAll(true);
		}
		public void ExpandAllRows() {
			Data.ChangeExpandedAll(false, true);
		}
		public void ExpandAllColumns() {
			Data.ChangeExpandedAll(true, true);
		}
		public void CollapseValue(bool isColumn, object[] values) {
			Data.ChangeExpanded(isColumn, values, false);
		}
		public void ExpandValue(bool isColumn, object[] values) {
			Data.ChangeExpanded(isColumn, values, true);
		}
		public int GetColumnIndex(object[] values) {
			return Data.GetFieldValueIndex(true, values);
		}
		public int GetRowIndex(object[] values) {
			return Data.GetFieldValueIndex(false, values);
		}
		public int GetColumnIndex(object[] values, PivotGridField field) {
			return Data.GetFieldValueIndex(true, values, field);
		}
		public int GetRowIndex(object[] values, PivotGridField field) {
			return Data.GetFieldValueIndex(false, values, field);
		}
		public bool IsObjectCollapsed(PivotGridField field, int lastLevelIndex) {
			return VisualItems.IsObjectCollapsed(field, lastLevelIndex);
		}
		public bool IsObjectCollapsed(bool isColumn, int lastLevelIndex, int level) {
			return VisualItems.IsObjectCollapsed(isColumn, lastLevelIndex, level);
		}
		public object GetFieldValue(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetFieldValue(field, lastLevelIndex);
		}
		public PivotGridValueType GetFieldValueType(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetFieldValueType(field, lastLevelIndex);
		}
		public PivotGridValueType GetFieldValueType(bool isColumn, int lastLevelIndex) {
			return VisualItems.GetLastLevelItem(isColumn, lastLevelIndex).ValueType;
		}
		public IOLAPMember GetFieldValueOLAPMember(PivotGridField field, int lastLevelIndex) {
			return VisualItems.GetOLAPMember(field, lastLevelIndex);
		}
		public object GetCellValue(int columnIndex, int rowIndex) {
			return VisualItems.GetCellValue(columnIndex, rowIndex);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues) {
			if(Data.GetFieldCountByArea(PivotArea.DataArea) != 1)
				throw new Exception("This method can be used if there is just one field in the data area only.");
			PivotGridField dataField = GetFieldByArea(PivotArea.DataArea, 0);
			return GetCellValue(columnValues, rowValues, dataField);
		}
		public object GetCellValue(object[] columnValues, object[] rowValues, PivotGridField dataField) {
			return Data.GetCellValue(columnValues, rowValues, dataField);
		}
		public PivotSummaryDataSource CreateSummaryDataSource() {
			return Data.CreateSummaryDataSource();
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource() {
			return Data.GetDrillDownDataSource(-1, -1, 0);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return Data.CreateDrillDownDataSource(columnIndex, rowIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			return Data.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount);
		}		
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return Data.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return Data.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return Data.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return Data.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int columnIndex, int rowIndex, List<string> customColumns) {
			return Data.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, -1, customColumns);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateServerModeDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return Data.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		#region async
		public virtual bool IsAsyncInProgress { get { return Data.IsLocked; } }
		public void EndUpdateAsync() {
			EndUpdateAsync(DoEmptyComplete);
		}
		public void EndUpdateAsync(AsyncCompletedHandler asyncCompleted) {
			base.CancelUpdate();
			Data.EndUpdateAsync(true, asyncCompleted);
		}
		public void SetDataSourceAsync(object dataSource) {
			SetDataSourceAsync(dataSource, DoEmptyComplete);
		}
		public void SetDataSourceAsync(object dataSource, AsyncCompletedHandler asyncCompleted) {
			BeginUpdate();
			DataSource = dataSource;
			EndUpdateAsync(asyncCompleted);
		}
		public void SetOLAPConnectionStringAsync(string olapConnectionString) {
			SetOLAPConnectionStringAsync(olapConnectionString, DoEmptyComplete);
		}
		public void SetOLAPConnectionStringAsync(string olapConnectionString, AsyncCompletedHandler asyncCompleted) {
			BeginUpdate();
			try {
				OLAPConnectionString = olapConnectionString;
			}
			catch {
				CancelUpdate();
				throw;
			}
			EndUpdateAsync(asyncCompleted);
		}
		public void CollapseAllAsync() {
			CollapseAllAsync(DoEmptyComplete);
		}
		public void CollapseAllAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(false, true, asyncCompleted);
		}
		public void CollapseAllRowsAsync() {
			CollapseAllRowsAsync(DoEmptyComplete);
		}
		public void CollapseAllRowsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(false, false, true, asyncCompleted);
		}
		public void CollapseAllColumnsAsync() {
			CollapseAllColumnsAsync(DoEmptyComplete);
		}
		public void CollapseAllColumnsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(true, false, true, asyncCompleted);
		}
		public void ExpandAllAsync() {
			ExpandAllAsync(DoEmptyComplete);
		}
		public void ExpandAllAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(true, true, asyncCompleted);
		}
		public void ExpandAllRowsAsync() {
			ExpandAllRowsAsync(DoEmptyComplete);
		}
		public void ExpandAllRowsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(false, true, true, asyncCompleted);
		}
		public void ExpandAllColumnsAsync() {
			ExpandAllColumnsAsync(DoEmptyComplete);
		}
		public void ExpandAllColumnsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAllAsync(true, true, true, asyncCompleted);
		}
		public void CollapseValueAsync(bool isColumn, object[] values) {
			CollapseValueAsync(isColumn, values, DoEmptyComplete);
		}
		public void CollapseValueAsync(bool isColumn, object[] values, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAsync(isColumn, values, false, true, asyncCompleted);
		}
		public void ExpandValueAsync(bool isColumn, object[] values) {
			ExpandValueAsync(isColumn, values, DoEmptyComplete);
		}
		public void ExpandValueAsync(bool isColumn, object[] values, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAsync(isColumn, values, true, true, asyncCompleted);
		}
		public void ChangeFieldExpandedAsync(PivotGridFieldBase field, bool expand) {
			ChangeFieldExpandedAsync(field, expand, DoEmptyComplete);
		}
		public void ChangeFieldExpandedAsync(PivotGridFieldBase field, bool expand, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeFieldExpandedAsync(field, expand, true, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(AsyncCompletedHandler asyncCompleted) {
			Data.GetDrillDownDataSourceAsync(-1, -1, 0, true, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, AsyncCompletedHandler asyncCompleted) {
			Data.CreateDrillDownDataSourceAsync(columnIndex, rowIndex, true, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, AsyncCompletedHandler asyncCompleted) {
			Data.CreateDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, true, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex,
				List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, -1, customColumns, asyncCompleted);
		}
		public void CreateDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			 Data.CreateQueryModeDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, true, asyncCompleted);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateOLAPDrillDownDataSourceAsync(int columnIndex, int rowIndex,
				List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, -1, customColumns, asyncCompleted);
		}
		[Obsolete("The CreateOLAPDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateOLAPDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, asyncCompleted);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateServerModeDrillDownDataSourceAsync(int columnIndex, int rowIndex, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
			CreateDrillDownDataSourceAsync(columnIndex, rowIndex, -1, customColumns, asyncCompleted);
		}
		[Obsolete("The CreateServerModeDrillDownDataSource method is obsolete now. Use the CreateDrillDownDataSource method instead.")]
		public void CreateServerModeDrillDownDataSourceAsync(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, AsyncCompletedHandler asyncCompleted) {
		   CreateDrillDownDataSourceAsync(columnIndex, rowIndex, maxRowCount, customColumns, asyncCompleted);
		}
		public void RetrieveFieldsAsync() {
			RetrieveFieldsAsync(DoEmptyComplete);
		}
		public void RetrieveFieldsAsync(AsyncCompletedHandler asyncCompleted) {
			Data.RetrieveFieldsAsync(true, asyncCompleted);
		}
		public void RetrieveFieldsAsync(PivotArea area, bool visible) {
			RetrieveFieldsAsync(area, visible, DoEmptyComplete);
		}
		public void RetrieveFieldsAsync(PivotArea area, bool visible, AsyncCompletedHandler asyncCompleted) {
			Data.RetrieveFieldsAsync(area, visible, true, asyncCompleted);
		}
		public void RefreshDataAsync() {
			RefreshDataAsync(DoEmptyComplete);
		}
		public void RefreshDataAsync(AsyncCompletedHandler asyncCompleted) {
			Data.ReloadDataAsync(true, asyncCompleted);
		}
		public void SetFieldSortingAsync(PivotGridField field, PivotSortOrder sortOrder) {
			SetFieldSortingAsync(field, sortOrder, DoEmptyComplete);
		}
		public void SetFieldSortingAsync(PivotGridField field, PivotSortOrder sortOrder, AsyncCompletedHandler asyncCompleted) {
			Data.SetFieldSortingAsync(field, sortOrder, PivotSortMode.DisplayText, null, true, true, asyncCompleted);
		}
		public void ClearFieldSortingAsync(PivotGridField field) {
			ClearFieldSortingAsync(field, DoEmptyComplete);
		}
		public void ClearFieldSortingAsync(PivotGridField field, AsyncCompletedHandler asyncCompleted) {
			Data.ClearFieldSortingAsync(field, true, asyncCompleted);
		}
		public void ChangeFieldSortOrderAsync(PivotGridField field) {
			ChangeFieldSortOrderAsync(field, DoEmptyComplete);
		}
		public void ChangeFieldSortOrderAsync(PivotGridField field, AsyncCompletedHandler asyncCompleted) {
			Data.ChangeFieldSortOrderAsync(field, true, asyncCompleted);
		}
		void DoEmptyComplete(object operationResult) {
		}
		#endregion
		internal Rectangle GetRealSelection(Rectangle selection) {
			int x1 = selection.Left, y1 = selection.Top,
				x2 = selection.Right, y2 = selection.Bottom;
			if(x1 < 0) x1 = 0;
			if(y1 < 0) y1 = 0;
			if(x2 >= ViewInfo.CellsArea.ColumnCount) x2 = ViewInfo.CellsArea.ColumnCount - 1;
			if(y2 >= ViewInfo.CellsArea.RowCount) y2 = ViewInfo.CellsArea.RowCount - 1;
			int lastColLevel = Data.GetLevelCount(true) - 1,
				lastRowLevel = Data.GetLevelCount(false) - 1;
			PivotCellViewInfoBase cell1 = ViewInfo.CellsArea.CreateCellViewInfo(x1, y1),
				cell2 = ViewInfo.CellsArea.CreateCellViewInfo(x2, y2);
			if(cell2.ColumnField != null && cell2.ColumnField.AreaIndex != lastColLevel &&
				cell2.ColumnFieldIndex < ViewInfo.CellsArea.CreateCellViewInfo(x2 - 1, y2).ColumnFieldIndex) {
				int startAreaIndex = cell2.ColumnField.AreaIndex;
				cell2 = ViewInfo.CellsArea.CreateCellViewInfo(++x2, y2);
				while(cell2.ColumnField != null && cell2.ColumnField.AreaIndex < startAreaIndex &&
					x2 < ViewInfo.CellsArea.ColumnCount && cell2.RowFieldIndex >= 0) {
					cell2 = ViewInfo.CellsArea.CreateCellViewInfo(++x2, y2);
				}
			}
			if(cell2.RowField != null && cell2.RowField.AreaIndex != lastRowLevel &&
				cell2.RowFieldIndex < ViewInfo.CellsArea.CreateCellViewInfo(x2, y2 - 1).RowFieldIndex) {
				int startAreaIndex = cell2.RowField.AreaIndex;
				cell2 = ViewInfo.CellsArea.CreateCellViewInfo(x2, ++y2);
				while(cell2.RowField != null && cell2.RowField.AreaIndex < startAreaIndex &&
					y2 < ViewInfo.CellsArea.RowCount && cell2.RowFieldIndex >= 0) {
					cell2 = ViewInfo.CellsArea.CreateCellViewInfo(x2, ++y2);
				}
			}
			int realX1 = cell1.ColumnFieldIndex >= 0 ? cell1.ColumnFieldIndex : 0,
				realY1 = cell1.RowFieldIndex >= 0 ? cell1.RowFieldIndex : 0,
				realX2 = cell2.ColumnFieldIndex >= 0 ? cell2.ColumnFieldIndex : Data.GetCellCount(true),
				realY2 = cell2.RowFieldIndex >= 0 ? cell2.RowFieldIndex : Data.GetCellCount(false);
			return new Rectangle(realX1, realY1, realX2 - realX1, realY2 - realY1);
		}
		protected virtual void RaiseDataSourceChanged() {
			EventHandler handler = (EventHandler)this.Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseBeginRefresh() {
			EventHandler handler = (EventHandler)this.Events[beginRefresh];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseEndRefresh() {
			EventHandler handler = (EventHandler)this.Events[endRefresh];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseGridLayout() {
			EventHandler handler = (EventHandler)this.Events[gridLayout];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseAreaChanging(PivotGridField field, PivotArea newArea, int newAreaIndex) {
			PivotAreaChangingEventHandler handler = (PivotAreaChangingEventHandler)this.Events[fieldAreaChanging];
			if(handler != null) {
				PivotAreaChangingEventArgs e = EventArgsHelper.CreateAreaChangingEventArgs(field, newArea, newAreaIndex);
				handler(this, e);
				return e.Allow;
			}
			else
				return true;
		}
		protected virtual object RaiseCustomUnboundColumnData(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			CustomFieldDataEventHandler handler = (CustomFieldDataEventHandler)this.Events[customUnboundFieldData];
			if(handler != null) {
				CustomFieldDataEventArgs e = EventArgsHelper.CreateCustomFieldDataEventArgs(field, listSourceRowIndex, expValue);
				handler(this, e);
				return e.Value;
			}
			else
				return expValue;
		}
		protected virtual void RaiseCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			PivotGridCustomSummaryEventHandler handler = (PivotGridCustomSummaryEventHandler)this.Events[customSummary];
			if(handler != null) {
				PivotGridCustomSummaryEventArgs e = EventArgsHelper.CreateCustomSummaryEventArgs(field, customSummaryInfo);
				handler(this, e);
			}
		}
		PivotGridCustomFieldSortEventArgs fieldSortEventArgs = null;
		protected virtual int? RaiseCustomFieldSort(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			PivotGridCustomFieldSortEventHandler handler = (PivotGridCustomFieldSortEventHandler)this.Events[customFieldSort];
			if(handler != null) {
				if(fieldSortEventArgs != null && (fieldSortEventArgs.Field != field || fieldSortEventArgs.Data != Data)) {
					fieldSortEventArgs = null;
				}
				if(fieldSortEventArgs == null) {
					fieldSortEventArgs = EventArgsHelper.CreateCustomFieldSortEventArgs(field);
				}
				fieldSortEventArgs.SetArgs(listSourceRow1, listSourceRow2, value1, value2, sortOrder);
				handler(this, fieldSortEventArgs);
				return fieldSortEventArgs.GetSortResult();
			}
			else
				return null;
		}
		CustomServerModeSortEventArgs customServerModeSortEventArgs = null;
		protected virtual int? RaiseCustomServerModeSort(PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridFieldBase field, PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			EventHandler<CustomServerModeSortEventArgs> handler = (EventHandler<CustomServerModeSortEventArgs>)this.Events[customServerModeSort];
			if(handler != null) {
				if(customServerModeSortEventArgs != null && customServerModeSortEventArgs.Field != field)
					customServerModeSortEventArgs = null;
				if(customServerModeSortEventArgs == null)
					customServerModeSortEventArgs = EventArgsHelper.CreateCustomServerModeSortEventArgs((PivotGridField)field);
				customServerModeSortEventArgs.SetArgs(value0, value1, helper);
				handler(this, customServerModeSortEventArgs);
				return customServerModeSortEventArgs.Result;
			}
			else
				return null;
		}
		protected virtual void RaiseHideCustomizationForm() {
			EventHandler handler = (EventHandler)this.Events[hideCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseShowingCustomizationForm(Form customizationForm, ref Control parentControl) {
			CustomizationFormShowingEventHandler handler = (CustomizationFormShowingEventHandler)this.Events[showingCustomizationForm];
			if(handler != null) {
				CustomizationFormShowingEventArgs e = EventArgsHelper.CreateCustomizationFormShowingEventArgs(customizationForm, ref parentControl);
				handler(this, e);
				parentControl = e.ParentControl;
				return e.Cancel;
			}
			else
				return false;
		}
		protected virtual void RaiseShowCustomizationForm() {
			EventHandler handler = (EventHandler)this.Events[showCustomizationForm];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseLayoutUpgrade(LayoutUpgradeEventArgs e) {
			LayoutUpgradeEventHandler handler = (LayoutUpgradeEventHandler)this.Events[layoutUpgrade];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseBeforeLoadLayout(LayoutAllowEventArgs e) {
			LayoutAllowEventHandler handler = (LayoutAllowEventHandler)this.Events[beforeLoadLayout];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseGroupFilterChanged(PivotGridGroup group) {
			PivotGroupEventHandler handler = (PivotGroupEventHandler)this.Events[groupFilterChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateGroupEventArgs(group));
		}
		protected virtual void RaiseFieldFilterChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldFilterChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual bool RaiseFieldFilterChanging(PivotGridField field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			PivotFieldFilterChangingEventHandler handler = (PivotFieldFilterChangingEventHandler)this.Events[fieldFilterChanging];
			if(handler != null) {
				PivotFieldFilterChangingEventArgs e = EventArgsHelper.CreateFieldFilterChangingEventArgs(field, filterType, showBlanks, values);
				handler(this, e);
				return e.Cancel;
			}
			else
				return false;
		}
		protected virtual void RaiseFieldAreaChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual void RaiseFieldExpandedInFieldsGroupChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldExpandedInFieldGroupChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual void RaiseFieldWidthChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldWidthChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual void RaiseFieldUnboundExpressionChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldUnboundExpressionChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual void RaiseFieldAreaIndexChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldAreaIndexChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual void RaiseFieldVisibleChanged(PivotGridField field) {
			PivotFieldEventHandler handler = (PivotFieldEventHandler)this.Events[fieldVisibleChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldEventArgs(field));
		}
		protected virtual void RaiseFieldPropertyChanged(PivotGridField field, PivotFieldPropertyName propertyName) {
			PivotFieldPropertyChangedEventHandler handler = (PivotFieldPropertyChangedEventHandler)this.Events[fieldPropertyChanged];
			if(handler != null) handler(this, EventArgsHelper.CreateFieldPropertyChangedEventArgs(field, propertyName));
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridField field, object value) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = EventArgsHelper.CreateFieldDisplayTextEventArgs(field, value, field.GetValueText(value));
				handler(this, e);
				return e.DisplayText;
			}
			else
				return field.GetValueText(value);
		}
		protected virtual string RaiseFieldValueDisplayText(PivotGridField field, IOLAPMember member) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = EventArgsHelper.CreateFieldDisplayTextEventArgs(field, member.Value, field.GetValueText(member));
				handler(this, e);
				return e.DisplayText;
			}
			else
				return field.GetValueText(member);
		}
		protected virtual string RaiseFieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			PivotFieldDisplayTextEventHandler handler = (PivotFieldDisplayTextEventHandler)this.Events[fieldValueDisplayText];
			if(handler != null) {
				PivotFieldDisplayTextEventArgs e = EventArgsHelper.CreateFieldDisplayTextEventArgs(item, defaultText);
				handler(this, e);
				return e.DisplayText;
			}
			else
				return defaultText;
		}
		protected virtual object RaiseCustomGroupInterval(PivotGridField field, object value) {
			PivotCustomGroupIntervalEventHandler handler = (PivotCustomGroupIntervalEventHandler)this.Events[customGroupInterval];
			if(handler != null) {
				PivotCustomGroupIntervalEventArgs e = EventArgsHelper.CreateCustomGroupIntervalEventArgs(field, value);
				handler(this, e);
				return e.GroupValue;
			}
			else
				return value;
		}
		protected virtual object RaiseCustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			PivotCustomChartDataSourceDataEventHandler handler = (PivotCustomChartDataSourceDataEventHandler)this.Events[customChartDataSourceData];
			if(handler != null) {
				PivotCustomChartDataSourceDataEventArgs e = EventArgsHelper.CreateCustomChartDataSourceDataEventArgs(itemType, itemDataMember, fieldValueItem, cellItem, value);
				handler(this, e);
				return e.Value;
			}
			return value;
		}
		void RaiseCustomChartDataSourceRows(PivotWinChartDataSource ds, IList<PivotChartDataSourceRowBase> rows) {
			PivotCustomChartDataSourceRowsEventHandler handler = (PivotCustomChartDataSourceRowsEventHandler)this.Events[customChartDataSourceRows];
			if(handler != null) {
				PivotCustomChartDataSourceRowsEventArgs e = EventArgsHelper.CreateCustomChartDataSourceRowsEventArgs(ds, rows);
				handler(this, e);
			}
		}
		protected virtual bool RaiseCustomFilterPopupItems(PivotGridFilterItems items) {
			PivotCustomFilterPopupItemsEventHandler handler = (PivotCustomFilterPopupItemsEventHandler)this.Events[customFilterPopupItems];
			if(handler != null) {
				handler(this, EventArgsHelper.CreateCustomFilterPopupItemsEventArgs(items));
				return true;
			}
			else
				return false;
		}
		protected virtual bool RaiseCustomFieldValueCells(PivotVisualItemsBase items) {
			PivotCustomFieldValueCellsEventHandler handler = (PivotCustomFieldValueCellsEventHandler)this.Events[customFieldValueCells];
			if(handler != null) {
				PivotCustomFieldValueCellsEventArgs e = EventArgsHelper.CreateCustomFieldValueCellsEventArgs(items);
				handler(this, e);
				return e.IsUpdateRequired;
			}
			else
				return false;
		}
		protected virtual int RaiseFieldValueImageIndex(PivotFieldValueItem item) {
			PivotFieldImageIndexEventHandler handler = (PivotFieldImageIndexEventHandler)this.Events[fieldValueImageIndex];
			if(handler != null) {
				PivotFieldImageIndexEventArgs e = EventArgsHelper.CreateFieldImageIndexEventArgs(item);
				handler(this, e);
				return e.ImageIndex;
			}
			else
				return -1;
		}
		protected virtual string RaiseCellDisplayText(PivotGridCellItem cellItem) {
			PivotCellDisplayTextEventHandler handler = (PivotCellDisplayTextEventHandler)this.Events[customCellDisplayText];
			if(handler != null) {
				PivotCellDisplayTextEventArgs e = EventArgsHelper.CreateCellDisplayTextEventArgs(cellItem);
				handler(this, e);
				return e.DisplayText;
			}
			else
				return cellItem.Text;
		}
		protected virtual object RaiseCustomCellValue(PivotGridCellItem cellItem) {
			EventHandler<PivotCellValueEventArgs> handler = (EventHandler<PivotCellValueEventArgs>)this.Events[customCellValue];
			if(handler != null) {
				PivotCellValueEventArgs e = EventArgsHelper.CreateCellValueEventArgs(cellItem);
				handler(this, e);
				return e.Value;
			}
			else
				return cellItem.Value;
		}
		protected virtual void RaiseCellDoubleClick(PivotCellViewInfo cellViewInfo) {
			PivotCellEventHandler handler = (PivotCellEventHandler)this.Events[cellDoubleClick];
			if(handler != null) handler(this, EventArgsHelper.CreateCellEventArgs(cellViewInfo));
		}
		protected virtual void RaiseCellClick(PivotCellViewInfo cellViewInfo) {
			PivotCellEventHandler handler = (PivotCellEventHandler)this.Events[cellClick];
			if(handler != null) handler(this, EventArgsHelper.CreateCellEventArgs(cellViewInfo));
		}
		protected virtual void RaiseCellSelectionChanged() {
			EventHandler handler = (EventHandler)this.Events[cellSelectionChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseFocusedCellChanged() {
			EventHandler handler = (EventHandler)this.Events[focusedCellChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseLeftTopCellChanged(Point oldValue, Point newValue) {
			EventHandler<PivotLeftTopCellChangedEventArgs> handler =
				(EventHandler<PivotLeftTopCellChangedEventArgs>)this.Events[leftTopCellChanged];
			if(handler != null)
				handler(this, EventArgsHelper.CreateLeftTopCellChangedEventArgs(oldValue, newValue));
		}
		protected virtual bool RaiseCustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw) {
			PivotCustomDrawCellEventHandler handler = (PivotCustomDrawCellEventHandler)this.Events[customDrawCell];
			if(handler != null) {
				PivotCustomDrawCellEventArgs e = EventArgsHelper.CreateCustomDrawCellEventArgs(paintArgs, ref appearance, cellViewInfo, defaultDraw);
				handler(this, e);
				appearance = e.Appearance;
				return e.Handled;
			}
			else
				return false;
		}
		protected virtual void RaiseCustomAppearance(ref AppearanceObject appearance, PivotGridCellItem cellItem, Rectangle? bounds) {
			PivotCustomAppearanceEventHandler handler = (PivotCustomAppearanceEventHandler)this.Events[customAppearance];
			if(handler != null) {
				PivotCustomAppearanceEventArgs e = EventArgsHelper.CreateCustomAppearanceEventArgs(ref appearance, cellItem, bounds);
				handler(this, e);
				appearance = e.Appearance;
			}
		}
		protected virtual bool RaiseCustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			PivotCustomDrawFieldHeaderEventHandler handler = (PivotCustomDrawFieldHeaderEventHandler)this.Events[customDrawFieldHeader];
			if(handler != null) {
				PivotCustomDrawFieldHeaderEventArgs e = EventArgsHelper.CreateCustomDrawFieldHeaderEventArgs(headerViewInfo, paintArgs, painter, defaultDraw);
				handler(this, e);
				return e.Handled;
			}
			else
				return false;
		}
		protected virtual bool RaiseCustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			PivotCustomDrawFieldValueEventHandler handler = (PivotCustomDrawFieldValueEventHandler)this.Events[customDrawFieldValue];
			if(handler != null) {
				PivotCustomDrawFieldValueEventArgs e = EventArgsHelper.CreateCustomDrawFieldValueEventArgs(paintArgs, fieldCellViewInfo, info, painter, defaultDraw);
				handler(this, e);
				return e.Handled;
			}
			else
				return false;
		}
		protected virtual int RaiseCustomRowHeight(PivotFieldValueItem item, int height) {
			EventHandler<PivotCustomRowHeightEventArgs> handler = (EventHandler<PivotCustomRowHeightEventArgs>)this.Events[customRowHeight];
			if(handler != null) {
				PivotCustomRowHeightEventArgs e = EventArgsHelper.CreateCustomRowHeightEventArgs(item, height);
				handler(this, e);
				return e.RowHeight;
			}
			else
				return height;
		}
		protected virtual int RaiseCustomColumnWidth(PivotFieldValueItem item, int width) {
			EventHandler<PivotCustomColumnWidthEventArgs> handler = (EventHandler<PivotCustomColumnWidthEventArgs>)this.Events[customColumnWidth];
			if(handler != null) {
				PivotCustomColumnWidthEventArgs e = EventArgsHelper.CreateCustomColumnWidthEventArgs(item, width);
				handler(this, e);
				return e.ColumnWidth;
			}
			else
				return width;
		}
		protected virtual bool RaiseCustomDrawFieldHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			PivotCustomDrawHeaderAreaEventHandler handler = (PivotCustomDrawHeaderAreaEventHandler)this.Events[customDrawFieldHeaderArea];
			if(handler != null) {
				PivotCustomDrawHeaderAreaEventArgs e = EventArgsHelper.CreateCustomDrawHeaderAreaEventArgs(headersViewInfo, paintArgs, bounds, defaultDraw);
				handler(this, e);
				return e.Handled;
			}
			else
				return false;
		}
		protected virtual bool RaiseCustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			PivotCustomDrawEventHandler handler = (PivotCustomDrawEventHandler)this.Events[customDrawEmptyArea];
			if(handler != null) {
				PivotCustomDrawEventArgs e = EventArgsHelper.CreateCustomDrawEventArgs(appearanceOwner, paintArgs, bounds, defaultDraw);
				handler(this, e);
				return e.Handled;
			}
			else
				return false;
		}
		protected virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[popupMenuShowing];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseMenuItemClick(PivotGridMenuItemClickEventArgs e) {
			PivotGridMenuItemClickEventHandler handler = (PivotGridMenuItemClickEventHandler)this.Events[menuItemClick];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseFieldValueCollapsed(PivotFieldValueItem item) {
			PivotFieldValueEventHandler handler = (PivotFieldValueEventHandler)this.Events[fieldValueCollapsed];
			if(handler != null)
				handler(this, EventArgsHelper.CreateFieldValueEventArgs(item));
		}
		protected virtual void RaiseFieldValueExpanded(PivotFieldValueItem item) {
			PivotFieldValueEventHandler handler = (PivotFieldValueEventHandler)this.Events[fieldValueExpanded];
			if(handler != null)
				handler(this, EventArgsHelper.CreateFieldValueEventArgs(item));
		}
		protected virtual void RaiseFieldValueNotExpanded(PivotFieldValueItem item, PivotGridField field) {
			PivotFieldValueEventHandler handler = (PivotFieldValueEventHandler)this.Events[fieldValueNotExpanded];
			if(handler != null) {
				PivotFieldValueEventArgs e = item != null ? EventArgsHelper.CreateFieldValueEventArgs(item) :
					EventArgsHelper.CreateFieldValueEventArgs(field);
				handler(this, e);
			}
		}
		protected virtual bool RaiseFieldValueCollapsing(PivotFieldValueItem item) {
			PivotFieldValueCancelEventHandler handler = (PivotFieldValueCancelEventHandler)this.Events[fieldValueCollapsing];
			return RaiseFieldValueCollapsingExpandingCore(item, handler);
		}
		protected virtual bool RaiseFieldValueExpanding(PivotFieldValueItem item) {
			PivotFieldValueCancelEventHandler handler = (PivotFieldValueCancelEventHandler)this.Events[fieldValueExpanding];
			return RaiseFieldValueCollapsingExpandingCore(item, handler);
		}
		protected bool RaiseFieldValueCollapsingExpandingCore(PivotFieldValueItem item, PivotFieldValueCancelEventHandler handler) {
			if(handler != null) {
				PivotFieldValueCancelEventArgs e = EventArgsHelper.CreateFieldValueCancelEventArgs(item);
				handler(this, e);
				return !e.Cancel;
			}
			else
				return true;
		}
		protected virtual void RaiseFieldTooltipShowing(PivotFieldTooltipShowingEventArgs e) {
			PivotFieldTooltipShowingEventHandler handler = (PivotFieldTooltipShowingEventHandler)this.Events[fieldTooltipShowing];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaisePrefilterCriteriaChanged() {
			EventHandler handler = (EventHandler)this.Events[prefilterCriteriaChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseOLAPQueryTimeout() {
			EventHandler handler = (EventHandler)this.Events[olapQueryTimeout];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual bool RaiseQueryException(Exception ex) {
			PivotOlapExceptionEventArgs e = null;
			PivotOlapExceptionEventHandler handler = (PivotOlapExceptionEventHandler)this.Events[olapException];
			if(handler != null) {
				e = EventArgsHelper.CreateOlapExceptionEventArgs(ex);
				handler(this, e);
			}
			PivotQueryExceptionEventHandler handler2 = (PivotQueryExceptionEventHandler)this.Events[queryException];
			if(handler2 != null) {
				if(e == null)
					e = EventArgsHelper.CreateOlapExceptionEventArgs(ex);
				handler2(this, e);
			}
			return e == null ? false : e.Handled;
		}
		protected virtual void RaiseEditValueChanged(PivotCellViewInfo cellInfo, BaseEdit edit) {
			EditValueChangedEventHandler handler = (EditValueChangedEventHandler)this.Events[editValueChanged];
			if(handler != null) {
				EditValueChangedEventArgs e = EventArgsHelper.CreateEditValueChangedEventArgs(cellInfo, edit);
				handler(this, e);
			}
		}
		protected virtual void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			BaseContainerValidateEditorEventHandler handler = (BaseContainerValidateEditorEventHandler)this.Events[validatingEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
			InvalidValueExceptionEventHandler handler = (InvalidValueExceptionEventHandler)this.Events[invalidValueException];
			if(handler != null) handler(this, e);
		}
		protected virtual object RaiseCustomEditValue(object value, PivotGridCellItem cellItem) {
			CustomEditValueEventHandler handler = (CustomEditValueEventHandler)this.Events[customEditValue];
			if(handler != null) {
				CustomEditValueEventArgs e = EventArgsHelper.CreateCustomEditValueEventArgs(value, cellItem);
				handler(this, e);
				return e.Value;
			}
			else
				return value;
		}
		protected virtual void RaiseShowingEditor(CancelPivotCellEditEventArgs e) {
			EventHandler<CancelPivotCellEditEventArgs> handler = (EventHandler<CancelPivotCellEditEventArgs>)this.Events[showingEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseShownEditor(PivotCellEditEventArgs e) {
			EventHandler<PivotCellEditEventArgs> handler = (EventHandler<PivotCellEditEventArgs>)this.Events[shownEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseHiddenEditor(PivotCellEditEventArgs e) {
			EventHandler<PivotCellEditEventArgs> handler = (EventHandler<PivotCellEditEventArgs>)this.Events[hiddenEditor];
			if(handler != null) handler(this, e);
		}
		protected virtual RepositoryItem RaiseCustomCellEdit(PivotGridCellItem cellItem, RepositoryItem repositoryItem) {
			EventHandler<PivotCustomCellEditEventArgs> handler = (EventHandler<PivotCustomCellEditEventArgs>)this.Events[customCellEdit];
			if(handler != null) {
				PivotCustomCellEditEventArgs e = EventArgsHelper.CreateCustomCellEditEventArgs(cellItem, repositoryItem);
				handler(this, e);
				return e.RepositoryItem;
			}
			return repositoryItem;
		}
		protected virtual RepositoryItem RaiseCustomCellEditForEditing(PivotCellViewInfo cellViewInfo, RepositoryItem repositoryItem) {
			EventHandler<PivotCustomCellEditEventArgs> handler = (EventHandler<PivotCustomCellEditEventArgs>)this.Events[customCellEditForEditing];
			if(handler != null) {
				PivotCustomCellEditEventArgs e = EventArgsHelper.CreateCustomCellEditEventArgs(cellViewInfo, repositoryItem);
				handler(this, e);
				return e.RepositoryItem;
			}
			return repositoryItem;
		}
		protected virtual bool RaiseCustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearance, ref Rectangle rect) {
			EventHandler<CustomExportHeaderEventArgs> handler = (EventHandler<CustomExportHeaderEventArgs>)this.Events[customExportHeader];
			if(handler != null) {
				CustomExportHeaderEventArgs e = EventArgsHelper.CreateCustomExportHeaderEventArgs(brick, field, appearance, ref rect);
				handler(this, e);
				appearance = e.Appearance;
				brick = e.Brick;
				return e.ApplyAppearanceToBrickStyle;
			}
			else
				return false;
		}
		protected virtual bool RaiseCustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem fieldValueItem, IPivotPrintAppearance appearance, ref Rectangle rect) {
			EventHandler<CustomExportFieldValueEventArgs> handler = (EventHandler<CustomExportFieldValueEventArgs>)this.Events[customExportFieldValue];
			if(handler != null) {
				CustomExportFieldValueEventArgs e = EventArgsHelper.CreateCustomExportFieldValueEventArgs(brick, fieldValueItem, appearance, ref rect);
				handler(this, e);
				appearance = e.Appearance;
				brick = e.Brick;
				return e.ApplyAppearanceToBrickStyle;
			}
			else
				return false;
		}
		protected virtual bool RaiseCustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			EventHandler<CustomExportCellEventArgs> handler = (EventHandler<CustomExportCellEventArgs>)this.Events[customExportCell];
			if(handler != null) {
				CustomExportCellEventArgs e = EventArgsHelper.CreateCustomExportCellEventArgs(brick, cellItem, appearance, graphicsUnit, ref rect);
				handler(this, e);
				appearance = e.Appearance;
				brick = e.Brick;
				return e.ApplyAppearanceToBrickStyle;
			}
			else
				return false;
		}
		protected virtual void RaiseExportStarted() {
			EventHandler handler = (EventHandler)this.Events[exportStarted];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseExportFinished() {
			EventHandler handler = (EventHandler)this.Events[exportFinished];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseAsyncOperationStarting() {
			EventHandler handler = (EventHandler)this.Events[asyncOperationStarting];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseAsyncOperationCompleted() {
			EventHandler handler = (EventHandler)this.Events[asyncOperationCompleted];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected virtual void RaiseUserAction() {
			PivotUserActionEventHandler handler = (PivotUserActionEventHandler)this.Events[userActionChanged];
			if(handler != null)
				handler(this, new PivotUserActionEventArgs(UserAction));
		}
		bool IsValidDataSource(object dataSource) {
			if(dataSource == null) return true;
			if(dataSource is IPivotGridDataSource) return true;
			if(dataSource is IList) return true;
			if(dataSource is IListSource) return true;
			if(dataSource is DataSet) return true;
			if(dataSource is System.Data.DataView) {
				System.Data.DataView dv = dataSource as System.Data.DataView;
				if(dv.Table == null) return false;
				return true;
			}
			if(dataSource is System.Data.DataTable) return true;
			return false;
		}
		bool isDataSourceActive;
		protected bool IsDataSourceActive { get { return isDataSourceActive; } }
		void SetDataSource(object dataSource) {
			UnsubscribeDataSourceEvents();
			this.dataSource = dataSource;
			isDataSourceActive = false;
			if(dataSource != null)
				OLAPConnectionString = null;
			EnsureDataSourceIsActive();
		}
		void UnsubscribeDataSourceEvents() {
			if(IsQueryableDataSource) {
				IBindingList queryableDataSourceBindingList = QueryableDataSourceBindingList;
				queryableDataSourceBindingList.ListChanged -= OnQueryableChanged;
			}
		}
		void ActivateDataSource() {
			if(IsLoading || fData == null) return;
			isDataSourceActive = true;
			if(!string.IsNullOrEmpty(olapConnectionString)) {
				fData.OLAPConnectionString = olapConnectionString;
				return;
			}
			fData.OLAPConnectionString = null;
			IPivotGridDataSource pivotDataSource = DataSource as IPivotGridDataSource;
			if(pivotDataSource != null) {
				fData.PivotDataSource = pivotDataSource;
				return;
			}
			if(IsQueryableDataSource) {
				SetQueryablePivotDataSource();
				IBindingList queryableDataSourceBindingList = QueryableDataSourceBindingList;
				queryableDataSourceBindingList.ListChanged -= OnQueryableChanged;
				queryableDataSourceBindingList.ListChanged += OnQueryableChanged;
				return;
			}
			DataView dataView = DataSource as DataView;
			if(dataView != null && dataView.Table == null)
				return;
			fData.SetDataSource(BindingContext, DataSource, DataMember);
		}
		void OnQueryableChanged(object sender, EventArgs a) {
			SetQueryablePivotDataSource();
		}
		void OnQueryableDataSourceExceptionThrown(object sender, LinqServerModeExceptionThrownEventArgs a) {
			throw new Exception("Server Mode Data Source exception thrown", a.Exception);
		}
		void OnQueryableDataSourceInconsistencyDetected(object sender, LinqServerModeInconsistencyDetectedEventArgs a) {
			if(a.Handled)
				return;
			throw new Exception("Server Mode Data Source inconsistency detected");
		}
		void SetQueryablePivotDataSource() {
			fData.PivotDataSource = new ServerModeDataSource(new QueryableQueryExecutor(Queryable));
		}
		void EnsureDataSourceIsActive() {
			if(!IsDataSourceActive) ActivateDataSource();
		}
		public string[] GetFieldList() {
			return Data.GetFieldList();
		}
		public List<string> GetOLAPKPIList() {
			return Data.GetOLAPKPIList();
		}
		public PivotOLAPKPIMeasures GetOLAPKPIMeasures(string kpiName) {
			return Data.GetOLAPKPIMeasures(kpiName);
		}
		public PivotOLAPKPIValue GetOLAPKPIValue(string kpiName) {
			return Data.GetOLAPKPIValue(kpiName);
		}
		public PivotKPIGraphic GetOLAPKPIServerGraphic(string kpiName, PivotKPIType kpiType) {
			return Data.GetOLAPKPIServerGraphic(kpiName, kpiType);
		}
		public Bitmap GetKPIBitmap(PivotKPIGraphic graphic, int state) {
			return Data.GetKPIBitmap(graphic, state);
		}
		public OLAPDataSourceBase CreateOLAPDataSourceClone() {
			return Data.CreateOLAPDataSourceClone();
		}
		bool IComponentLoading.IsLoading { get { return IsLoading; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		protected virtual void OnLookAndFeel_StyleChanged(object sender, EventArgs e) {
			fData.LookAndFeelChanged();
		}
		protected internal PivotGridViewInfoData Data {
			get {
				EnsureDataSourceIsActive();
				return fData;
			}
		}
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return this.Data; }
		}
		PivotEventArgsHelper EventArgsHelper { get { return Data.EventArgsHelper; } }
		protected PivotGridViewInfo ViewInfo { get { return Data != null ? (PivotGridViewInfo)Data.ViewInfo : null; } }
		protected PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		public void EnsureViewInfoIsCalculated() {
			if(ViewInfo != null && !ViewInfo.IsReady)
				ViewInfo.EnsureIsCalculated();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point LeftTopCoord {
			get { return ViewInfo.LeftTopCoord; }
			set {
				InvalidateScrollBars();
				CloseEditor();
				ViewInfo.LeftTopCoord = value;
			}
		}
		protected override Color ScrollBarsGlyphColor { get { return LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control); } }
		protected override BaseViewInfo RootViewInfo { get { return Data != null ? Data.ViewInfo : null; } }
		protected override void UpdateEditor() {
			if(!HasActiveEditor || this.isEditorReady)
				return;
			UpdateEditorInfoArgs infoArgs = CreateUpdateEditorInfoArgs(GetCellInfo(FocusedCell));
			ActiveEditor.Bounds = infoArgs.Bounds;
			this.isEditorReady = true;
		}
		bool HasActiveEditor { get { return ActiveEditor != null; } }
		public virtual void ForceInitialize() {
			OnLoaded();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			EditorContainer.OnLoaded();
			ActivateDataSource();
			if(ListSource == null && string.IsNullOrEmpty(OLAPConnectionString))
				RefreshData();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(ViewInfo != null) ViewInfo.ClientSizeChanged();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			if(ViewInfo != null)
				ViewInfo.StopDragging();
			base.OnHandleDestroyed(e);
		}
		protected override void OnValidating(CancelEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				CloseEditor();
			}
			catch(HideException) {
				e.Cancel = true;
			}
			finally {
				EditorHelper.EndAllowHideException();
			}
			base.OnValidating(e);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if(Data != null)
				Data.InvalidateFocusedCell();
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if(Data != null)
				Data.InvalidateFocusedCell();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(IsControlDown && key == Keys.Tab) return base.ProcessDialogKey(Keys.Tab);
			if(key == Keys.Down || key == Keys.Up || key == Keys.Left || key == Keys.Right || key == Keys.Tab) return false;
			return base.ProcessDialogKey(keyData);
		}
		bool IsControlDown { get { return Control.ModifierKeys == Keys.Control; } }
		Point clickedCellCoord = SelectionVisualItems.EmptyCoord;
		protected Point ClickedCellCoord { get { return clickedCellCoord; } set { clickedCellCoord = value; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(HasActiveEditor && !ActiveEditor.Bounds.Contains(e.Location)) {
					CloseEditor();
				}
				Point clickedCell = ViewInfo.CellsArea.GetCellCoordAt(e.Location),
					focusedCell = FocusedCell;
				ClickedCellCoord = clickedCell;
				base.OnMouseDown(e);
				if(e.Button == MouseButtons.Left) {
					EditorShowMode showMode = OptionsBehavior.GetEditorShowMode();
					if(VisualItems.Selection.IsEmpty && (showMode == EditorShowMode.MouseDown ||
						(showMode == EditorShowMode.MouseDownFocused && clickedCell == focusedCell))) {
						ShowEditor(clickedCell);
						if(HasActiveEditor) {
							DevExpress.XtraPivotGrid.ViewInfo.PivotCellsViewInfo.DoubleClickChecker.Lock();
							ViewInfo.MouseUp(e);	
							DevExpress.XtraPivotGrid.ViewInfo.PivotCellsViewInfo.DoubleClickChecker.Unlock();
							ActiveEditor.SendMouse(ActiveEditor.PointToClient(Control.MousePosition), Control.MouseButtons);
						}
					}
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseUp(e);
				if(e.Button == MouseButtons.Left && !HasActiveEditor
						&& OptionsBehavior.GetEditorShowMode() == EditorShowMode.MouseUp
						&& ClickedCellCoord == ViewInfo.CellsArea.GetCellCoordAt(e.Location)) {
					ShowEditor(ClickedCellCoord);
				}
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseClick(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				if(OptionsBehavior.GetEditorShowMode() == EditorShowMode.Click
					&& ViewInfo.CellsArea.GetCellCoordAt(e.Location) == FocusedCell) {
					ShowEditor(FocusedCell);
				}
				base.OnMouseClick(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseDoubleClick(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseMove(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyDown(e);
				if(!e.Handled)
					ProcessKeyDown(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnKeyPress(e);
				if(!e.Handled)
					ProcessKeyPress(e);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		protected virtual void ProcessKeyDown(KeyEventArgs e) {
			if(ViewInfo == null || HasActiveEditor || !ViewInfo.IsReady) return;
			RepositoryItem repositoryItem = GetCellEditForEditing(GetCellInfo(FocusedCell));
			if(repositoryItem == null) return;
			if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2) {
				ShowEditor();
				return;
			}
			if(repositoryItem.IsActivateKey(e.KeyCode)) {
				ShowEditorByKey(e);
				return;
			}
		}
		static char[] keyPress = new char[] { (char)27, (char)9, (char)8, (char)3 };
		protected virtual void ProcessKeyPress(KeyPressEventArgs e) {
			if(ViewInfo == null || HasActiveEditor || !ViewInfo.IsReady) return;
			if(Array.IndexOf(keyPress, e.KeyChar) == -1) {
				if(e.KeyChar == ' ' && IsControlDown) return;
				if(e.KeyChar == 1 && IsControlDown) return;
				ShowEditorByKeyPress(e);
			}
		}
		protected virtual void ShowEditorByKey(KeyEventArgs e) {
			ShowEditor();
			if(HasActiveEditor && e.KeyCode != Keys.Enter) {
				ActiveEditor.SendKey(e);
			}
		}
		protected virtual void ShowEditorByKeyPress(KeyPressEventArgs e) {
			ShowEditor();
			if(HasActiveEditor && e.KeyChar != 13 && e.KeyChar != 9) {
				ActiveEditor.SendKey(lastKeyMessage, e);
			}
		}
		internal object lastKeyMessage = null;
		protected override void WndProc(ref Message m) {
			lastKeyMessage = DevExpress.XtraEditors.Senders.BaseSender.SaveMessage(ref m, lastKeyMessage);
			if(DevExpress.XtraEditors.Senders.BaseSender.RequireShowEditor(ref m)) ShowEditor();
			if(!PivotGestureHelper.WndProc(ref m))
				base.WndProc(ref m);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			mouseHelper.OnMouseWheel(e);
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ee) {
			OnMouseWheelCore(ee);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ee) {
			try {
				EditorHelper.BeginAllowHideException();
				base.OnMouseWheel(ee);
				DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
				if(!e.Handled && !XtraForm.ProcessSmartMouseWheel(this, ee))
					mouseHelper.OnMouseWheel(ee);
			}
			catch(HideException) { }
			finally {
				EditorHelper.EndAllowHideException();
				UpdateCursor();
			}
		}
		#region IMouseWheelScrollClient Members
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			OnMouseWheelEx(e);
		}
		int GetScrollLinesCount() {
			return SystemInformation.MouseWheelScrollLines >= 0 ? SystemInformation.MouseWheelScrollLines : 1;
		}
		void ChangeVScrollBarValue(int delta, int value) {
			int scrollValue = VScrollBar.Value + (delta > 0 ? -value : value);
			if(scrollValue < 0)
				scrollValue = 0;
			int maxValue = VScrollBar.Maximum - VScrollBar.LargeChange + 1;
			if(scrollValue > maxValue)
				scrollValue = maxValue;
			VScrollBar.Value = scrollValue;
		}
		protected virtual void OnMouseWheelEx(MouseWheelScrollClientArgs e) {
			if(!e.Horizontal) {
				if(WindowsFormsSettings.IsAllowPixelScrolling) {
					if(e.InPixels) {
						VScrollBar.Value += e.Distance;
					} else {
						ChangeVScrollBarValue(e.Delta, ViewInfo.FieldMeasures.DefaultFieldValueHeight);
					}
				} else {
					ChangeVScrollBarValue(e.Delta, GetScrollLinesCount());
				}
			}
		}
		bool IMouseWheelScrollClient.PixelModeHorz { get { return WindowsFormsSettings.IsAllowPixelScrolling; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return WindowsFormsSettings.IsAllowPixelScrolling; } }
		#endregion
		protected virtual void UpdateCursor() {
			if(ViewInfo != null && ViewInfo.State == PivotGridViewInfoState.FieldResizing)
				Cursor.Current = Cursors.SizeWE;
			else
				Cursor.Current = this.Cursor;
		}
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		protected override void OnSizeChanged(EventArgs e) {
			if(IsHandleCreated && ViewInfo != null)
				ViewInfo.ClientSizeChanged();
			ViewInfo.ResetPaintBounds();
			base.OnSizeChanged(e);
			this.isEditorReady = false;
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlIsDragging"),
#endif
 Browsable(false)]
		public bool IsDragging { get { return ViewInfo.IsDragging; } }
		protected override bool IsHScrollBarVisible { get { return ViewInfo.IsHScrollBarVisible; } }
		protected override bool IsVScrollBarVisible { get { return ViewInfo.IsVScrollBarVisible; } }
		protected override ScrollArgs HScrollBarInfo { get { return ViewInfo.HScrollBarInfo; } }
		protected override ScrollArgs VScrollBarInfo { get { return ViewInfo.VScrollBarInfo; } }
		protected override bool IsRightToLeft { get { return Data.IsRightToLeft; } }
		protected override void ScrollBarsValueChanged(Point newValue) {
			try {
				EditorHelper.BeginAllowHideException();
				LeftTopCoord = newValue;
			}
			catch(HideException) {
				UpdateScrollBars();
			}
			finally {
				EditorHelper.EndAllowHideException();
			}
		}
		protected override void InternalRefresh() {
			Data.InvalidateViewInfo();
			base.InternalRefresh();
		}
		protected virtual PivotGridPrinter CreatePrinter() {
			PivotGridPrinter printer = new PivotGridPrinter(this);
			return printer;
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridControlToolTipController"),
#endif
 DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null) {
					ToolTipController.DefaultController.RemoveClientControl(this);
					ToolTipController.AddClientControl(this);
				}
				else ToolTipController.DefaultController.AddClientControl(this);
			}
		}
		public virtual bool ShowUnboundExpressionEditor(PivotGridField field) {
			UserAction = UserAction.FieldUnboundExpression;
			try {
				using(ExpressionEditorForm form = new UnboundColumnExpressionEditorForm(field, null)) {
					form.SetMenuManager(MenuManager);
					form.StartPosition = FormStartPosition.CenterParent;
					if(form.ShowDialog(this) == DialogResult.OK) {
						field.UnboundExpression = form.Expression;
						return true;
					}
					return false;
				}
			}
			finally {
				UserAction = UserAction.None;
			}
		}
		#region IToolTipControlClient
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetToolTipObjectInfoCore(point);
		}
		protected virtual ToolTipControlInfo GetToolTipObjectInfoCore(Point point) {
			ToolTipControlInfo info = ViewInfo.GetToolTipObjectInfo(point);
			if(info != null) {
				PivotFieldTooltipShowingEventArgs e = EventArgsHelper.CreateFieldTooltipShowingEventArgs(point, info.Text);
				RaiseFieldTooltipShowing(e);
				if(e.ShowTooltip == false) return null;
				info.Text = e.Text;
			}
			return info;
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
		#endregion
		#region IPrintable
		bool IPrintable.CreatesIntersectedBricks {
			get { return false; }
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			if(printer != null) {
				printer.Release();
			}
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			printer.SetCommandsVisibility(ps);
			EnsureDataSourceIsActive();
			printer.Initialize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			printer.CreateArea(areaName, graph);
		}
		void IPrintable.AcceptChanges() {
			printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl { get { return printer.PropertyEditorControl; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsPrintingAvailable { get { return ComponentPrinterBase.IsPrintingAvailable(false); } }
		public void ShowPrintPreview() { printer.PerformPrintingAction(() => printer.ComponentPrinter.ShowPreview(LookAndFeel)); }
		public void ShowRibbonPrintPreview() { printer.PerformPrintingAction(() => printer.ComponentPrinter.ShowRibbonPreview(LookAndFeel)); }
		public void Print() { printer.PerformPrintingAction(delegate { printer.ComponentPrinter.Print(); }); }
		#endregion
		#region Export
		void Export(ExportTarget target, string filepath) {
			printer.PerformPrintingExportAction(delegate { printer.ComponentPrinter.Export(target, filepath); });
		}
		void Export(ExportTarget target, Stream stream) {
			printer.PerformPrintingExportAction(delegate { printer.ComponentPrinter.Export(target, stream); });
		}
		void Export(ExportTarget target, string filepath, ExportOptionsBase options) {
			if(ExportUtils.AllowNewExcelExportEx(options as IDataAwareExportOptions,target))
				using(PivotGridViewImplementer gridView = new PivotGridViewImplementer(target, options, OptionsPrint, Data, Text)) {
					gridView.Export(filepath);
				}
			else
				printer.PerformPrintingExportAction(delegate { printer.ComponentPrinter.Export(target, filepath, options); });
		}
		void Export(ExportTarget target, Stream stream, ExportOptionsBase options) {
			if(ExportUtils.AllowNewExcelExportEx(options as IDataAwareExportOptions,target))
				using(PivotGridViewImplementer gridView = new PivotGridViewImplementer(target, options, OptionsPrint, Data, Text)) {
					gridView.Export(stream);
				}
			else
				printer.PerformPrintingExportAction(delegate { printer.ComponentPrinter.Export(target, stream, options); });
		}
		public void ExportToXls(string filePath) {
			ExportToXls(filePath, TextExportMode.Value);
		}
		public void ExportToXls(string filePath, TextExportMode textExportMode) {
			ExportToXls(filePath, new XlsExportOptions(textExportMode));
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xls, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToXls(Stream stream) {
			ExportToXls(stream, TextExportMode.Value);
		}
		public void ExportToXls(Stream stream, TextExportMode textExportMode) {
			ExportToXls(stream, new XlsExportOptions(textExportMode));
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xls, stream, options);
			RaiseExportFinished();
		}
		public void ExportToXlsx(string filePath) {
			ExportToXlsx(filePath, TextExportMode.Value);
		}
		public void ExportToXlsx(string filePath, TextExportMode textExportMode) {
			ExportToXlsx(filePath, new XlsxExportOptions(textExportMode));
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xlsx, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToXlsx(Stream stream) {
			ExportToXlsx(stream, TextExportMode.Value);
		}
		public void ExportToXlsx(Stream stream, TextExportMode textExportMode) {
			ExportToXlsx(stream, new XlsxExportOptions(textExportMode));
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Xlsx, stream, options);
			RaiseExportFinished();
		}
		public void ExportToRtf(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Rtf, filePath);
			RaiseExportFinished();
		}
		public void ExportToRtf(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Rtf, stream);
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath);
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, string htmlCharSet) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath, new HtmlExportOptions(htmlCharSet));
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath, new HtmlExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Html, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Html, stream);
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Html, stream, new HtmlExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Html, stream, options);
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath);
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, string htmlCharSet) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath, new MhtExportOptions(htmlCharSet));
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath, new MhtExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream, string htmlCharSet, string title, bool compressed) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, stream, new MhtExportOptions(htmlCharSet, title, compressed));
			RaiseExportFinished();
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Mht, stream, options);
			RaiseExportFinished();
		}
		public void ExportToPdf(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, filePath);
			RaiseExportFinished();
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, stream);
			RaiseExportFinished();
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Pdf, stream, options);
			RaiseExportFinished();
		}
		public void ExportToImage(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Image, filePath);
			RaiseExportFinished();
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Image, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToImage(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Image, stream);
			RaiseExportFinished();
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Image, stream, options);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath, options);
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		public void ExportToText(string filePath, string separator, Encoding encoding) {
			RaiseExportStarted();
			Export(ExportTarget.Text, filePath, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream);
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, TextExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream, options);
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, string separator) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream, new TextExportOptions(separator));
			RaiseExportFinished();
		}
		public void ExportToText(Stream stream, string separator, Encoding encoding) {
			RaiseExportStarted();
			Export(ExportTarget.Text, stream, new TextExportOptions(separator, encoding));
			RaiseExportFinished();
		}
		public void ExportToCsv(Stream stream) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, stream);
			RaiseExportFinished();
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, stream, options);
			RaiseExportFinished();
		}
		public void ExportToCsv(string filePath) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, filePath);
			RaiseExportFinished();
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			RaiseExportStarted();
			Export(ExportTarget.Csv, filePath, options);
			RaiseExportFinished();
		}
		#endregion
		#region IPivotGridEventsImplementor
		void IPivotGridEventsImplementorBase.DataSourceChanged() {
			RaiseDataSourceChanged();
		}
		void IPivotGridEventsImplementorBase.BeginRefresh() {
			RaiseBeginRefresh();
		}
		void IPivotGridEventsImplementorBase.EndRefresh() {
			RaiseEndRefresh();
		}
		void IPivotGridEventsImplementorBase.LayoutChanged() {
			RaiseGridLayout();
		}
		bool IPivotGridEventsImplementorBase.FieldAreaChanging(PivotGridFieldBase field, PivotArea newArea, int newAreaIndex) {
			return RaiseAreaChanging((PivotGridField)field, newArea, newAreaIndex);
		}
		object IPivotGridEventsImplementorBase.GetUnboundValue(PivotGridFieldBase field, int listSourceRowIndex, object expValue) {
			return RaiseCustomUnboundColumnData(field, listSourceRowIndex, expValue);
		}
		void IPivotGridEventsImplementorBase.CalcCustomSummary(PivotGridFieldBase field, PivotCustomSummaryInfo customSummaryInfo) {
			if(IsDesignModeCore) return;
			RaiseCustomSummary(field, customSummaryInfo);
		}
		bool IPivotGridEventsImplementor.ShowingCustomizationForm(Form customizationForm, ref Control parentControl) {
			return RaiseShowingCustomizationForm(customizationForm, ref parentControl);
		}
		void IPivotGridEventsImplementor.ShowCustomizationForm() {
			RaiseShowCustomizationForm();
		}
		void IPivotGridEventsImplementor.HideCustomizationForm() {
			RaiseHideCustomizationForm();
		}
		void IPivotGridEventsImplementor.AsyncOperationStarting() {
			RaiseAsyncOperationStarting();
		}
		void IPivotGridEventsImplementor.AsyncOperationCompleted() {
			RaiseAsyncOperationCompleted();
		}
		void IPivotGridEventsImplementor.OnPopupMenuShowing(PopupMenuShowingEventArgs e) {
			RaisePopupMenuShowing(e);
		}
		void IPivotGridEventsImplementor.OnPopupMenuItemClick(PivotGridMenuItemClickEventArgs e) {
			RaiseMenuItemClick(e);
		}
		void IPivotGridEventsImplementorBase.GroupFilterChanged(PivotGridGroup group) {
			RaiseGroupFilterChanged((PivotGridGroup)group);
		}
		void IPivotGridEventsImplementorBase.FieldFilterChanged(PivotGridFieldBase field) {
			RaiseFieldFilterChanged((PivotGridField)field);
		}
		bool IPivotGridEventsImplementorBase.FieldFilterChanging(PivotGridFieldBase field, PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return RaiseFieldFilterChanging((PivotGridField)field, filterType, showBlanks, values);
		}
		void IPivotGridEventsImplementorBase.FieldAreaChanged(PivotGridFieldBase field) {
			RaiseFieldAreaChanged((PivotGridField)field);
		}
		void IPivotGridEventsImplementorBase.FieldExpandedInFieldsGroupChanged(PivotGridFieldBase field) {
			RaiseFieldExpandedInFieldsGroupChanged((PivotGridField)field);
		}
		void IPivotGridEventsImplementorBase.FieldWidthChanged(PivotGridFieldBase field) {
			RaiseFieldWidthChanged((PivotGridField)field);
		}
		void IPivotGridEventsImplementorBase.FieldUnboundExpressionChanged(PivotGridFieldBase field) {
			RaiseFieldUnboundExpressionChanged((PivotGridField)field);
		}
		void IPivotGridEventsImplementorBase.FieldAreaIndexChanged(PivotGridFieldBase field) {
			RaiseFieldAreaIndexChanged((PivotGridField)field);
		}
		void IPivotGridEventsImplementorBase.FieldVisibleChanged(PivotGridFieldBase field) {
			RaiseFieldVisibleChanged((PivotGridField)field);
		}
		void IPivotGridEventsImplementorBase.FieldPropertyChanged(PivotGridFieldBase field, PivotFieldPropertyName propertyName) {
			RaiseFieldPropertyChanged((PivotGridField)field, propertyName);
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotFieldValueItem item, string defaultText) {
			return RaiseFieldValueDisplayText(item, defaultText);
		}
		bool IPivotGridEventsImplementorBase.BeforeFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(item.IsCollapsed)
				return RaiseFieldValueExpanding(item);
			else
				return RaiseFieldValueCollapsing(item);
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeExpanded(PivotFieldValueItem item) {
			if(item.IsCollapsed)
				RaiseFieldValueExpanded(item);
			else
				RaiseFieldValueCollapsed(item);
		}
		void IPivotGridEventsImplementorBase.AfterFieldValueChangeNotExpanded(PivotFieldValueItem item, PivotGridFieldBase field) {
			RaiseFieldValueNotExpanded(item, (PivotGridField)field);
		}
		[Obsolete]
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, object value) {
#pragma warning disable 612
			return RaiseFieldValueDisplayText((PivotGridField)field, value);
#pragma warning restore 612
		}
		string IPivotGridEventsImplementorBase.FieldValueDisplayText(PivotGridFieldBase field, IOLAPMember value) {
			return RaiseFieldValueDisplayText((PivotGridField)field, value);
		}
		int IPivotGridEventsImplementor.GetCustomRowHeight(PivotFieldValueItem item, int height) {
			return RaiseCustomRowHeight(item, height);
		}
		int IPivotGridEventsImplementor.GetCustomColumnWidth(PivotFieldValueItem item, int width) {
			return RaiseCustomColumnWidth(item, width);
		}
		object IPivotGridEventsImplementorBase.CustomGroupInterval(PivotGridFieldBase field, object value) {
			return RaiseCustomGroupInterval((PivotGridField)field, value);
		}
		int IPivotGridEventsImplementor.FieldValueImageIndex(PivotFieldValueItem item) {
			return RaiseFieldValueImageIndex(item);
		}
		string IPivotGridEventsImplementorBase.CustomCellDisplayText(PivotGridCellItem cellItem) {
			return RaiseCellDisplayText(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomCellValue(PivotGridCellItem cellItem) {
			return RaiseCustomCellValue(cellItem);
		}
		int? IPivotGridEventsImplementorBase.GetCustomSortRows(int listSourceRow1, int listSourceRow2, object value1, object value2, PivotGridFieldBase field, PivotSortOrder sortOrder) {
			return RaiseCustomFieldSort(listSourceRow1, listSourceRow2, value1, value2, field, sortOrder);
		}
		int? IPivotGridEventsImplementorBase.QuerySorting(DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value0, DevExpress.PivotGrid.QueryMode.Sorting.IQueryMemberProvider value1, PivotGridFieldBase field, DevExpress.PivotGrid.QueryMode.Sorting.ICustomSortHelper helper) {
			return RaiseCustomServerModeSort(value0, value1, field, helper);
		}
		void IPivotGridEventsImplementor.CellDoubleClick(PivotCellViewInfo cellViewInfo) {
			RaiseCellDoubleClick(cellViewInfo);
		}
		void IPivotGridEventsImplementor.CellClick(PivotCellViewInfo cellViewInfo) {
			RaiseCellClick(cellViewInfo);
		}
		void IPivotGridEventsImplementor.CellSelectionChanged() {
			RaiseCellSelectionChanged();
		}
		void IPivotGridEventsImplementor.FocusedCellChanged() {
			RaiseFocusedCellChanged();
		}
		void IPivotGridEventsImplementor.LeftTopCellChanged(Point oldValue, Point newValue) {
			RaiseLeftTopCellChanged(oldValue, newValue);
		}
		bool IPivotGridEventsImplementor.CustomDrawHeaderArea(PivotHeadersViewInfoBase headersViewInfo, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			return RaiseCustomDrawFieldHeaderArea(headersViewInfo, paintArgs, bounds, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawEmptyArea(IPivotCustomDrawAppearanceOwner appearanceOwner, ViewInfoPaintArgs paintArgs, Rectangle bounds, MethodInvoker defaultDraw) {
			return RaiseCustomDrawEmptyArea(appearanceOwner, paintArgs, bounds, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawFieldHeader(PivotHeaderViewInfoBase headerViewInfo, ViewInfoPaintArgs paintArgs, HeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return RaiseCustomDrawFieldHeader(headerViewInfo, paintArgs, painter, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawFieldValue(ViewInfoPaintArgs paintArgs, PivotFieldsAreaCellViewInfo fieldCellViewInfo, PivotHeaderObjectInfoArgs info, PivotHeaderObjectPainter painter, MethodInvoker defaultDraw) {
			return RaiseCustomDrawFieldValue(paintArgs, fieldCellViewInfo, info, painter, defaultDraw);
		}
		bool IPivotGridEventsImplementor.CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject appearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw) {
			return RaiseCustomDrawCell(paintArgs, ref appearance, cellViewInfo, defaultDraw);
		}
		void IPivotGridEventsImplementor.CustomAppearance(ref AppearanceObject appearance, PivotGridCellItem cellItem, Rectangle? bounds) {
			RaiseCustomAppearance(ref appearance, cellItem, bounds);
		}
		void IPivotGridEventsImplementorBase.PrefilterCriteriaChanged() {
			RaisePrefilterCriteriaChanged();
		}
		void IPivotGridEventsImplementorBase.OLAPQueryTimeout() {
			RaiseOLAPQueryTimeout();
		}
		bool IPivotGridEventsImplementorBase.QueryException(Exception ex) {
			return RaiseQueryException(ex);
		}
		object IPivotGridEventsImplementor.CustomEditValue(object value, PivotGridCellItem cellItem) {
			return RaiseCustomEditValue(value, cellItem);
		}
		RepositoryItem IPivotGridEventsImplementor.GetCellEdit(PivotGridCellItem cellItem) {
			return GetCellEdit(cellItem);
		}
		object IPivotGridEventsImplementorBase.CustomChartDataSourceData(PivotChartItemType itemType, PivotChartItemDataMember itemDataMember, PivotFieldValueItem fieldValueItem, PivotGridCellItem cellItem, object value) {
			return RaiseCustomChartDataSourceData(itemType, itemDataMember, fieldValueItem, cellItem, value);
		}
		void IPivotGridEventsImplementorBase.CustomChartDataSourceRows(IList<PivotChartDataSourceRowBase> rows) {
			RaiseCustomChartDataSourceRows(ChartDataSource, rows);
		}
		bool IPivotGridEventsImplementorBase.CustomFilterPopupItems(PivotGridFilterItems items) {
			return RaiseCustomFilterPopupItems(items);
		}
		bool IPivotGridEventsImplementorBase.CustomFieldValueCells(PivotVisualItemsBase items) {
			return RaiseCustomFieldValueCells(items);
		}
		#endregion
		#region IPivotGridDataOwner
		void IPivotGridDataOwner.FireChanged(object[] changedObjects) {
			if(!DesignMode || IsLoading) return;
			if(changedObjects == null) {
				changedObjects = new object[] { this };
			}
			IComponentChangeService srv = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) {
				foreach(object obj in changedObjects) {
					Component component = obj as Component;
					if(component == null) continue;
					srv.OnComponentChanged(component, null, null, null);
				}
				srv.OnComponentChanged(this, null, null, null);
			}
		}
		#endregion
		#region ChartDataSource
		PivotWinChartDataSource ChartDataSource {
			get {
				if(IsDisposed)
					throw new ObjectDisposedException("PivotGridControl");
				return (PivotWinChartDataSource)(Data.ChartDataSource);
			}
		}
		internal virtual void EnsureChartData() {
			ChartDataSource.EnsureChartData();
		}
		internal virtual void RaiseListChanged() {
			RaiseListChangedProps();
			RaiseListChangedReset();
		}
		void RaiseListChangedProps() {
			if(IsDisposed || ChartDataSource.IsListChangedLocked)
				return;
			if(listChanged != null)
				listChanged(this, new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0));
		}
		void RaiseListChangedReset() {
			if(IsDisposed || ChartDataSource.IsListChangedLocked)
				return;
			if(listChanged != null)
				listChanged(this, new ListChangedEventArgs(ListChangedType.Reset, 0));
		}
		void OnChartDataSourceListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.PropertyDescriptorChanged)
				RaiseListChangedProps();
			else if(e.ListChangedType == ListChangedType.Reset)
				RaiseListChangedReset();
			else
				throw new Exception("Incorrect list changed event type!");
		}
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return ChartDataSource.GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return ChartDataSource != null ? this.Name : string.Empty;
		}
		#endregion
		protected IBindingList IBindingList {
			get { return (IBindingList)ChartDataSource; }
		}
		#region IBindingList Members
		void IBindingList.AddIndex(PropertyDescriptor property) { IBindingList.AddIndex(property); }
		object IBindingList.AddNew() { return IBindingList.AddNew(); }
		bool IBindingList.AllowEdit { get { return IBindingList.AllowEdit; } }
		bool IBindingList.AllowNew { get { return IBindingList.AllowNew; } }
		bool IBindingList.AllowRemove { get { return IBindingList.AllowRemove; } }
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			IBindingList.ApplySort(property, direction);
		}
		int IBindingList.Find(PropertyDescriptor property, object key) { return IBindingList.Find(property, key); }
		bool IBindingList.IsSorted { get { return IBindingList.IsSorted; } }
		ListChangedEventHandler listChanged;
		event ListChangedEventHandler IBindingList.ListChanged {
			add { this.listChanged += value; }
			remove { this.listChanged -= value; }
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) { IBindingList.RemoveIndex(property); }
		void IBindingList.RemoveSort() { IBindingList.RemoveSort(); }
		ListSortDirection IBindingList.SortDirection { get { return IBindingList.SortDirection; } }
		PropertyDescriptor IBindingList.SortProperty { get { return IBindingList.SortProperty; } }
		bool IBindingList.SupportsChangeNotification { get { return IBindingList.SupportsChangeNotification; } }
		bool IBindingList.SupportsSearching { get { return IBindingList.SupportsSearching; } }
		bool IBindingList.SupportsSorting { get { return IBindingList.SupportsSorting; } }
		#endregion
		#region IList Members
		int IList.Add(object value) { return ChartDataSource.Add(value); }
		void IList.Clear() { ChartDataSource.Clear(); }
		bool IList.Contains(object value) { return ChartDataSource.Contains(value); }
		int IList.IndexOf(object value) { return ChartDataSource.IndexOf(value); }
		void IList.Insert(int index, object value) { ChartDataSource.Insert(index, value); }
		bool IList.IsFixedSize { get { return ChartDataSource.IsFixedSize; } }
		bool IList.IsReadOnly { get { return ChartDataSource.IsReadOnly; } }
		void IList.Remove(object value) { ChartDataSource.Remove(value); }
		void IList.RemoveAt(int index) { ChartDataSource.RemoveAt(index); }
		object IList.this[int index] {
			get { return ChartDataSource[index]; }
			set { ChartDataSource[index] = value; }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) { ChartDataSource.CopyTo(array, index); }
		int ICollection.Count { get { return ChartDataSource.Count; } }
		bool ICollection.IsSynchronized { get { return ChartDataSource.IsSynchronized; } }
		object ICollection.SyncRoot { get { return ChartDataSource.SyncRoot; } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable)ChartDataSource).GetEnumerator();
		}
		#endregion
		#region IPivotGrid Members
		protected IChartDataSource IChartDataSource {
			get {
				return Data != null ? (IChartDataSource)(ChartDataSource) : null;
			}
		}
		IList<string> IPivotGrid.ArgumentColumnNames {
			get { return IPivot.ArgumentColumnNames; }
		}
		IList<string> IPivotGrid.ValueColumnNames {
			get { return IPivot.ValueColumnNames; }
		}
		protected IPivotGrid IPivot {
			get { return (IPivotGrid)ChartDataSource; }
		}
		event DataChangedEventHandler IChartDataSource.DataChanged {
			add {
				if(IChartDataSource != null)
					IChartDataSource.DataChanged += value;
			}
			remove {
				if(IChartDataSource != null)
					IChartDataSource.DataChanged -= value;
			}
		}
		bool IPivotGrid.RetrieveDataByColumns {
			get { return IPivot.RetrieveDataByColumns; }
			set { IPivot.RetrieveDataByColumns = value; }
		}
		bool IPivotGrid.SinglePageSupported { get { return IPivot.SinglePageSupported; } }
		bool IPivotGrid.SinglePageOnly { get { return IPivot.SinglePageOnly; } set { IPivot.SinglePageOnly = value; } }
		bool IPivotGrid.SelectionSupported { get { return IPivot.SelectionSupported; } }
		bool IPivotGrid.SelectionOnly {
			get { return IPivot.SelectionOnly; }
			set { IPivot.SelectionOnly = value; }
		}
		bool IPivotGrid.RetrieveColumnTotals {
			get { return IPivot.RetrieveColumnTotals; }
			set { IPivot.RetrieveColumnTotals = value; }
		}
		bool IPivotGrid.RetrieveColumnGrandTotals {
			get { return IPivot.RetrieveColumnGrandTotals; }
			set { IPivot.RetrieveColumnGrandTotals = value; }
		}
		bool IPivotGrid.RetrieveColumnCustomTotals {
			get { return IPivot.RetrieveColumnCustomTotals; }
			set { IPivot.RetrieveColumnCustomTotals = value; }
		}
		bool IPivotGrid.RetrieveRowTotals {
			get { return IPivot.RetrieveRowTotals; }
			set { IPivot.RetrieveRowTotals = value; }
		}
		bool IPivotGrid.RetrieveRowGrandTotals {
			get { return IPivot.RetrieveRowGrandTotals; }
			set { IPivot.RetrieveRowGrandTotals = value; }
		}
		bool IPivotGrid.RetrieveRowCustomTotals {
			get { return IPivot.RetrieveRowCustomTotals; }
			set { IPivot.RetrieveRowCustomTotals = value; }
		}
		bool IPivotGrid.RetrieveEmptyCells {
			get { return IPivot.RetrieveEmptyCells; }
			set { IPivot.RetrieveEmptyCells = value; }
		}
		bool IPivotGrid.RetrieveDateTimeValuesAsMiddleValues {
			get { return IPivot.RetrieveDateTimeValuesAsMiddleValues; }
			set { IPivot.RetrieveDateTimeValuesAsMiddleValues = value; }
		}
		int IPivotGrid.UpdateDelay {
			get { return IPivot.UpdateDelay; }
			set { IPivot.UpdateDelay = value; }
		}
		int IPivotGrid.MaxAllowedSeriesCount {
			get { return IPivot.MaxAllowedSeriesCount; }
			set { IPivot.MaxAllowedSeriesCount = value; }
		}
		int IPivotGrid.MaxAllowedPointCountInSeries {
			get { return IPivot.MaxAllowedPointCountInSeries; }
			set { IPivot.MaxAllowedPointCountInSeries = value; }
		}
		string IChartDataSource.ValueDataMember { get { return IPivot.ValueDataMember; } }
		string IChartDataSource.ArgumentDataMember { get { return IPivot.ArgumentDataMember; } }
		string IChartDataSource.SeriesDataMember { get { return IPivot.SeriesDataMember; } }
		DefaultBoolean IPivotGrid.RetrieveFieldValuesAsText {
			get { return IPivot.RetrieveFieldValuesAsText; }
			set { IPivot.RetrieveFieldValuesAsText = value; }
		}
		DateTimeMeasureUnitNative? IChartDataSource.DateTimeArgumentMeasureUnit { get { return IPivot.DateTimeArgumentMeasureUnit; } }
		IDictionary<DateTime, DateTimeMeasureUnitNative> IPivotGrid.DateTimeMeasureUnitByArgument { get { return IPivot.DateTimeMeasureUnitByArgument; } }
		int IPivotGrid.AvailableSeriesCount { get { return IPivot.AvailableSeriesCount; } }
		IDictionary<object, int> IPivotGrid.AvailablePointCountInSeries { get { return IPivot.AvailablePointCountInSeries; } }
		void IPivotGrid.LockListChanged() {
			IPivot.LockListChanged();
		}
		void IPivotGrid.UnlockListChanged() {
			IPivot.UnlockListChanged();
		}
		#endregion
		#endregion
		#region Editors
		PivotGridEditorContainer editorContainer;
		PivotGridEditorContainerHelper editorHelper;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal PivotGridEditorContainer EditorContainer { get { return editorContainer; } }
		protected internal PivotGridEditorContainerHelper EditorHelper { get { return editorHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RepositoryItemCollection RepositoryItems { get { return EditorHelper.RepositoryItems; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseEdit ActiveEditor { get { return EditorHelper == null ? null : EditorHelper.ActiveEditor; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object EditValue {
			get { return !HasActiveEditor ? null : ActiveEditor.EditValue; }
			set { if(HasActiveEditor) ActiveEditor.EditValue = value; }
		}
		public void ShowEditor() {
			ShowEditor(FocusedCell);
		}
		public void ShowEditor(Point location) {
			CloseEditor();
			PivotCellViewInfo cellInfo = GetCellInfo(location);
			if(cellInfo == null) return;
			BaseEdit edit = ActivateEditor(cellInfo);
			if(edit != null)
				RaiseShownEditor(EventArgsHelper.CreateCellEditEventArgs(cellInfo, edit));
		}
		bool CanShowEditor(PivotCellViewInfo cellInfo, RepositoryItem repositoryItem) {
			if(IsDesignModeCore)
				return false;
			CancelPivotCellEditEventArgs e = EventArgsHelper.CreateCancelPivotCellEditEventArgs(cellInfo, repositoryItem);
			RaiseShowingEditor(e);
			if(e.Cancel) return false;
			return true;
		}
		protected internal Point FocusedCell { get { return VisualItems.FocusedCell; } }
		protected internal BaseEdit ActivateEditor(Point cellCoord) {
			PivotCellViewInfo cellInfo = GetCellInfo(cellCoord);
			if(cellInfo != null)
				return ActivateEditor(cellInfo);
			return null;
		}
		protected virtual BaseEdit ActivateEditor(PivotCellViewInfo cellInfo) {
			RepositoryItem rItem = GetCellEditForEditing(cellInfo);
			if(rItem == null || !GetAllowEdit(cellInfo) || !CanShowEditor(cellInfo, rItem))
				return null;
			return UpdateEditor(rItem, CreateUpdateEditorInfoArgs(cellInfo));
		}
		protected virtual bool GetAllowEdit(PivotCellViewInfo cellInfo) {
			return cellInfo.DataField != null ? cellInfo.DataField.CanEdit : false;
		}
		protected virtual RepositoryItem GetCellEditForEditing(PivotCellViewInfo cellInfo) {
			return RaiseCustomCellEditForEditing(cellInfo, GetCellEdit(cellInfo));
		}
		protected virtual RepositoryItem GetCellEdit(PivotGridCellItem cellInfo) {
			RepositoryItem repositoryItem = cellInfo != null && cellInfo.DataField != null ? ((PivotFieldItem)cellInfo.DataField).FieldEdit : null;
			return RaiseCustomCellEdit(cellInfo, repositoryItem);
		}
		protected UpdateEditorInfoArgs CreateUpdateEditorInfoArgs(PivotCellViewInfo cellInfo) {
			Rectangle bounds = cellInfo.GetEditorBounds(new Point(1, 1), new Size(3, 3), true);
			return new UpdateEditorInfoArgs(Data.IsFieldReadOnly(Data.GetField(cellInfo.DataField) as PivotGridField), bounds, cellInfo.Appearance, cellInfo.Value, LookAndFeel);
		}
		protected BaseEdit UpdateEditor(RepositoryItem rItem, UpdateEditorInfoArgs updateArgs) {
			BaseEdit edit = EditorHelper.UpdateEditor(rItem, updateArgs);
			EditorHelper.ShowEditor(edit, this);
			return edit;
		}
		public void CloseEditor() {
			if(!HasActiveEditor) return;
			try {
				Focus();
			}
			catch { }
			PostEditor();
			HideEditor();
		}
		public void HideEditor() {
			if(!HasActiveEditor) return;
			BaseEdit edit = ActiveEditor;
			EditorHelper.HideEditorCore(this, true);
			RaiseHiddenEditor(EventArgsHelper.CreateCellEditEventArgs(GetCellInfo(FocusedCell), edit));
		}
		public bool PostEditor() {
			if(!HasActiveEditor || !ActiveEditor.IsModified) return false;
			if(!ValidateEditor()) return false;
			PivotCellViewInfo cellInfo = GetCellInfo(FocusedCell);
			if(ActiveEditor.IsModified && !object.Equals(cellInfo.Value, ActiveEditor.EditValue)) {
				OnEditValueChanged(cellInfo, ActiveEditor);
				try {
					if(BindingContext != null && dataSource != null) {
						CurrencyManager currencyManager = BindingContext[dataSource, dataMember] as CurrencyManager;
						if(currencyManager != null && currencyManager.Position >= 0)
							currencyManager.EndCurrentEdit();
					}
				} catch { }
				Data.DoRefresh();
			}
			return true;
		}
		public bool ValidateEditor() {
			if(!HasActiveEditor) return true;
			return EditorHelper.ValidateEditor(this);
		}
		void OnValidatingEditor(BaseContainerValidateEditorEventArgs e) {
			RaiseValidatingEditor(e);
		}
		protected virtual void OnEditValueChanged(PivotCellViewInfo cellInfo, BaseEdit edit) {
			RaiseEditValueChanged(cellInfo, edit);
		}
		protected internal PivotCellViewInfo GetCellInfo(Point coord) {
			return ViewInfo != null ? (PivotCellViewInfo)ViewInfo.CellsArea.CreateCellViewInfo(coord.X, coord.Y) : null;
		}
		protected internal class PivotGridEditorContainer : EditorContainer {
			protected override EditorContainerHelper CreateHelper() {
				return new PivotGridEditorContainerHelper(this);
			}
			new internal protected PivotGridEditorContainerHelper EditorHelper { get { return (PivotGridEditorContainerHelper)base.EditorHelper; } }
			protected override void RaiseEditorKeyDown(KeyEventArgs e) {
				base.RaiseEditorKeyDown(e);
				if(e.Handled) return;
				if(EditorHelper.ActiveEditor != null) {
					BaseEdit be = (BaseEdit)EditorHelper.ActiveEditor;
					if(be.IsNeededKey(e))
						return;
				}
				try {
					EditorHelper.BeginAllowHideException();
					ProcessKeyDown(e);
				}
				catch(HideException) {
					e.Handled = true;
				}
				finally {
					EditorHelper.EndAllowHideException();
				}
			}
			void ProcessKeyDown(KeyEventArgs e) {
				switch(e.KeyCode) {
					case Keys.Enter:
						EditorHelper.Pivot.CloseEditor();
						break;
					case Keys.Escape:
						EditorHelper.Pivot.HideEditor();
						break;
					case Keys.Tab:
						if(!e.Handled && !e.Control && !e.Shift) {
							EditorHelper.Pivot.CloseEditor();
							e.Handled = EditorHelper.Pivot.VisualItems.OnKeyDown((int)Keys.Tab, e.Control, e.Shift);
							EditorHelper.Pivot.ShowEditor();
						}
						break;
				}
			}
			internal new void OnLoaded() {
				base.OnLoaded();
			}
		}
		protected internal class PivotGridEditorContainerHelper : EditorContainerHelper {
			PivotGridControl pivot;
			public PivotGridEditorContainerHelper(EditorContainer owner)
				: base(owner) {
			}
			public override void Dispose() {
				base.Dispose();
				Pivot = null;
			}
			protected override void RaiseInvalidValueException(InvalidValueExceptionEventArgs e) {
				if(Pivot != null)
					Pivot.RaiseInvalidValueException(e);
			}
			protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs va) {
				if(Pivot != null)
					Pivot.RaiseValidatingEditor(va);
			}
			protected override bool IsLoading {
				get { return base.IsLoading || Pivot.IsLoading; }
			}
			internal PivotGridControl Pivot {
				get { return pivot; }
				set {
					pivot = value;
					((PersistentRepository)InternalRepository).SetParentComponent(Pivot);
				}
			}
			public override ToolTipController RealToolTipController {
				get {
					return (Pivot.ToolTipController == null) ? ToolTipController.DefaultController : Pivot.ToolTipController;
				}
			}
			protected override IDXMenuManager MenuManager {
				get { return Pivot.MenuManager; }
			}
			protected override void OnRepositoryItemChanged(RepositoryItem item) {
				base.OnRepositoryItemChanged(item);
				Pivot.Data.InvalidateViewInfo();
			}
		}
		#endregion Editors
		#region IPivotGridPrinterOwner Members
		bool IPivotGridPrinterOwner.CustomExportHeader(ref IVisualBrick brick, PivotFieldItemBase field, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return RaiseCustomExportHeader(ref brick, field, appearance, ref rect);
		}
		bool IPivotGridPrinterOwner.CustomExportFieldValue(ref IVisualBrick brick, PivotFieldValueItem fieldValueItem, IPivotPrintAppearance appearance, ref Rectangle rect) {
			return RaiseCustomExportFieldValue(ref brick, fieldValueItem, appearance, ref rect);
		}
		bool IPivotGridPrinterOwner.CustomExportCell(ref IVisualBrick brick, PivotGridCellItem cellItem, IPivotPrintAppearance appearance, GraphicsUnit graphicsUnit, ref Rectangle rect) {
			return RaiseCustomExportCell(ref brick, cellItem, appearance, graphicsUnit, ref rect);
		}
		#endregion
		#region Serialization
		PivotGridSerializationHelper serializationHelper;
		protected PivotGridSerializationHelper SerializationHelper {
			get {
				if(serializationHelper == null)
					serializationHelper = CreateSerializationHelper();
				return serializationHelper;
			}
		}
		protected virtual PivotGridSerializationHelper CreateSerializationHelper() {
			return new PivotGridSerializationHelper(Data);
		}
		#region IXtraSerializable
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			OnStartDeserializing(e);
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			OnEndDeserializing(restoredVersion);
		}
		void IXtraSerializable.OnStartSerializing() { }
		void IXtraSerializable.OnEndSerializing() { }
		protected virtual void OnStartDeserializing(LayoutAllowEventArgs e) {
			RaiseBeforeLoadLayout(e);
			if(!e.Allow) return;
			Data.SetIsDeserializing(true);
			Data.BeginUpdate();
		}
		protected virtual void OnEndDeserializing(string restoredVersion) {
			Data.OnDeserializationComplete();
			Data.SetIsDeserializing(false);
			try {
				if(restoredVersion != OptionsLayout.LayoutVersion)
					RaiseLayoutUpgrade(EventArgsHelper.CreateLayoutUpgradeEventArgs(restoredVersion));
			}
			finally {
				Data.EndUpdate();
			}
		}
		#endregion
		#region IXtraSerializableLayout Members
		string IXtraSerializableLayout.LayoutVersion {
			get { return OptionsLayout.LayoutVersion; }
		}
		#endregion
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			PivotGridOptionsLayout opts = options as PivotGridOptionsLayout;
			if(opts == null) return true;
			if(opts.StoreAllOptions || opts.Columns.StoreAllOptions) return true;
			switch(id) {
				case LayoutIdAppearance:
					return opts.StoreAppearance || opts.Columns.StoreAppearance;
				case LayoutIdData:
					return opts.StoreDataSettings;
				case LayoutIdOptionsView:
					return opts.StoreVisualOptions;
				case LayoutIdLayout:
					return opts.Columns.StoreLayout;
				case LayoutIdFormatRules:
					return opts.StoreFormatRules;
			}
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			PivotGridOptionsLayout opts = options as PivotGridOptionsLayout;
			if(opts == null || opts.StoreAppearance || opts.Columns.StoreAppearance || opts.StoreAllOptions) {
				Appearance.Reset();
				AppearancePrint.Reset();
			}
			if(opts == null || opts.StoreVisualOptions || opts.StoreAllOptions) {
				OptionsView.Reset();
			}
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsPrint) != 0)
				OptionsPrint.Reset();
			if(opts == null || (opts.ResetOptions & PivotGridResetOptions.OptionsDataField) != 0)
				OptionsDataField.Reset();
			if(opts == null) return;
			if((opts.ResetOptions & PivotGridResetOptions.OptionsFilterPopup) != 0)
				OptionsFilterPopup.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsHint) != 0)
				OptionsHint.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsMenu) != 0)
				OptionsMenu.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsBehavior) != 0)
				OptionsBehavior.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsChartDataSource) != 0)
				OptionsChartDataSource.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsCustomization) != 0)
				OptionsCustomization.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsData) != 0)
				OptionsData.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsOLAP) != 0)
				OptionsOLAP.Reset();
			if((opts.ResetOptions & PivotGridResetOptions.OptionsSelection) != 0)
				OptionsSelection.Reset();
			if(opts.StoreLayoutOptions && (opts.ResetOptions & PivotGridResetOptions.OptionsLayout) != 0)
				OptionsLayout.Reset();
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == fieldsPropertyName)
				return SerializationHelper.DeserializeField(e);
			if(propertyName == groupsPropertyName)
				return SerializationHelper.DeserializeGroup(e);
			if(propertyName == formatConditionsPropertyName)
				return SerializationHelper.DeserializeFormatConditionsItem(e);
			if(propertyName == formatRulesPropertyName)
				return SerializationHelper.DeserializeFormatRulesItem(e);
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) { }
		#endregion
		#region IXtraSupportDeserializeCollection Members
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) { }
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) { }
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			if(propertyName == fieldsPropertyName)
				return SerializationHelper.ClearFields(e);
			if(propertyName == groupsPropertyName)
				return SerializationHelper.ClearGroups(e);
			if(propertyName == formatConditionsPropertyName)
				return SerializationHelper.ClearFormatConditions(e);
			return false;
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraFindFieldsItem(XtraItemEventArgs e) {
			return SerializationHelper.FindField(e);
		}
		#endregion
		#region IThreadSafeAccessible Members
		IThreadSafeAccessible ThreadSafeAccess {
			get { return (IThreadSafeAccessible)Data.VisualItems; } 
		}
		IThreadSafeFieldCollection IThreadSafeAccessible.Fields { get { return ThreadSafeAccess.Fields; } }
		IThreadSafeGroupCollection IThreadSafeAccessible.Groups { get { return ThreadSafeAccess.Groups; } }
		string IThreadSafeAccessible.GetCellDisplayText(int columnIndex, int rowIndex) {
			return ThreadSafeAccess.GetCellDisplayText(columnIndex, rowIndex);
		}
		IThreadSafeField IThreadSafeAccessible.GetFieldByArea(PivotArea area, int index) {
			return ThreadSafeAccess.GetFieldByArea(area, index);
		}
		IThreadSafeField IThreadSafeAccessible.GetFieldByLevel(bool isColumn, int level) {
			return ThreadSafeAccess.GetFieldByLevel(isColumn, level);
		}
		int IThreadSafeAccessible.GetFieldCountByArea(PivotArea area) {
			return ThreadSafeAccess.GetFieldCountByArea(area);
		}
		string IThreadSafeAccessible.GetFieldValueDisplayText(IThreadSafeField field, int lastLevelIndex) {
			return ThreadSafeAccess.GetFieldValueDisplayText(field, lastLevelIndex);
		}
		List<IThreadSafeField> IThreadSafeAccessible.GetFieldsByArea(PivotArea area) {
			return ThreadSafeAccess.GetFieldsByArea(area);
		}
		int IThreadSafeAccessible.ColumnCount {
			get { return ThreadSafeAccess.ColumnCount; }
		}
		int IThreadSafeAccessible.RowCount {
			get { return ThreadSafeAccess.RowCount; }
		}
		bool IThreadSafeAccessible.IsAsyncInProgress { 
			get { return ThreadSafeAccess.IsAsyncInProgress; } 
		}
		#endregion
		#region IPivotGestureClient Implemented Members
		IntPtr IGestureClient.Handle { get { return Handle; } }
		IntPtr IGestureClient.OverPanWindowHandle { get { return PivotGestureHelper.FindOverpanWindow(this); } }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(ViewInfo.CellsArea.PaintBounds.Contains(point))
				return PivotGridViewInfo.AllowedCellAreaGestures;
			return GestureAllowArgs.None;
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
			if(ViewInfo.CellsArea.PaintBounds.Contains(info.Start.Point))
				ViewInfo.OnGestureBegin(info);
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(ViewInfo.CellsArea.PaintBounds.Contains(info.Start.Point)) {
				ViewInfo.OnGesturePan(info, delta, ref overPan);
				Refresh();
			}
		}
		void IPivotGestureClient.OnTwoFingerSelection(Point start, Point end) {
			if(ViewInfo.CellsArea.PaintBounds.Contains(start) && ViewInfo.CellsArea.PaintBounds.Contains(end))
				ViewInfo.OnGestureTwoFingerSelection(start, end);
		}
		Point IGestureClient.PointToClient(Point p) {
			return PointToClient(p);
		}
		#endregion
		#region IGestureClient Unneeded Members
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ConvertFormatConditionToFormatRules() {
			if(FormatConditions.Count == 0)
				return;
			BeginUpdate();
			try {
				FormatRules.BeginUpdate();
				foreach(PivotGridStyleFormatCondition condition in FormatConditions) {
					PivotGridFormatRule format = new PivotGridFormatRule();
					FormatRuleTotalTypeSettings sett = new FormatRuleTotalTypeSettings();
					sett.ApplyToCell = condition.ApplyToCell;
					sett.ApplyToCustomTotalCell = condition.ApplyToCustomTotalCell;
					sett.ApplyToGrandTotalCell = condition.ApplyToGrandTotalCell;
					sett.ApplyToTotalCell = condition.ApplyToTotalCell;
					format.Settings = sett;
					format.Enabled = condition.Enabled;
					format.Tag = condition.Tag;
					format.Name = condition.Name;
					if(condition.Field != null)
						format.Measure = (PivotGridField)condition.Field;
					if(condition.Condition == DevExpress.XtraGrid.FormatConditionEnum.Expression) {
						FormatConditionRuleExpression ruleExpression = new FormatConditionRuleExpression() { Expression = condition.Expression };
						ruleExpression.Appearance.Assign(condition.Appearance);
						format.Rule = ruleExpression;
					} else {
						FormatConditionRuleValue ruleValue = new FormatConditionRuleValue() { Condition = (FormatCondition)condition.Condition, Value1 = condition.Value1, Value2 = condition.Value2 };
						ruleValue.Appearance.Assign(condition.Appearance);
						format.Rule = ruleValue;
					}
					FormatRules.Add(format);
				}
				FormatConditions.Clear();
			} finally {
				FormatRules.EndUpdate();
				EndUpdate();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Data != null && RootViewInfo != null)
				VisualItems.EnsureIsCalculated();
			base.OnPaint(e);
			RaisePaintEvent(this, e);
		}
	}
	public interface IPivotGridViewInfoDataOwner {
		PivotGridViewInfoData DataViewInfo { get; }
	}
	public enum PivotEndUpdateMode { Refresh, Invalidate };
}
