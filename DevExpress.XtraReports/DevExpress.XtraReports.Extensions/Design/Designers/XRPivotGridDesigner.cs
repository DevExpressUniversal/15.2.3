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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Localization;
using System.Drawing;
using DevExpress.XtraPrinting;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Serializing;
using System.IO;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraReports.Design.Frames;
using DevExpress.XtraPrinting.Native;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Paint;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraReports.UI.PivotGrid;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraPrinting.Design;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.Serialization;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Data;
using DevExpress.XtraReports.Native.CalculatedFields;
using DevExpress.XtraReports.Wizards.Builder;
namespace DevExpress.XtraReports.Design {
	[MouseTarget(typeof(PivotGridMouseTarget))]
	public class XRPivotGridDesigner : XRControlDesigner {
		#region inner classes
		class FakeSite : ISite, ILookAndFeelService {
			IComponent comp;
			DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel;
			public FakeSite(IComponent comp, DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
				this.comp = comp;
				this.lookAndFeel = lookAndFeel;
			}
			IComponent ISite.Component {
				get { return comp; }
			}
			IContainer ISite.Container {
				get { return null; }
			}
			bool ISite.DesignMode {
				get { return false; }
			}
			string ISite.Name {
				get { return string.Empty; }
				set { }
			}
			object IServiceProvider.GetService(Type t) {
				if(t == typeof(ILookAndFeelService))
					return this;
				return null;
			}
			void ILookAndFeelService.InitializeRootLookAndFeel(LookAndFeel.UserLookAndFeel lookAndFeel) {
			}
			LookAndFeel.UserLookAndFeel ILookAndFeelService.LookAndFeel {
				get { return lookAndFeel; }
			}
		}
		class FakePivotGridControl : PivotGridControl, IServiceProvider {
			Dictionary<string, bool> optionsView = new Dictionary<string, bool>();
			PivotGridAppearancesPrint paintAppearancePrint;
			IServiceProvider servProvider;
			Action<DataControllerBase> callback;
			public override PivotGridAppearancesPrint PaintAppearancePrint {
				get { return paintAppearancePrint; }
			}
			public FakePivotGridControl(IServiceProvider servProvider, Action<DataControllerBase> callback) {
				this.servProvider = servProvider;
				this.callback = callback;
			}
			object IServiceProvider.GetService(Type seviceType) {
				return servProvider != null ? servProvider.GetService(seviceType) : null;
			}
			public void InitializeProperties(XRPivotGridAppearances srcAppearance) {
				SynchAppearance(Appearance.Cell, srcAppearance.Cell);
				SynchAppearance(Appearance.CustomTotalCell, srcAppearance.CustomTotalCell);
				SynchAppearance(Appearance.FieldHeader, srcAppearance.FieldHeader);
				SynchAppearance(Appearance.FieldValue, srcAppearance.FieldValue);
				SynchAppearance(Appearance.FieldValueGrandTotal, srcAppearance.FieldValueGrandTotal);
				SynchAppearance(Appearance.FieldValueTotal, srcAppearance.FieldValueTotal);
				SynchAppearance(Appearance.GrandTotalCell, srcAppearance.GrandTotalCell);
				SynchAppearance(Appearance.Lines, srcAppearance.Lines);
				SynchAppearance(Appearance.TotalCell, srcAppearance.TotalCell);
				AppearancePrint.Assign(Appearance);
				AppearancePrint.Changed += new EventHandler(appearancePrint_Changed);
				paintAppearancePrint = new XRDesignAppearances(Data);
				paintAppearancePrint.Assign(Appearance);
				optionsView["ShowColumnHeaders"] = OptionsView.ShowColumnHeaders;
				optionsView["ShowRowHeaders"] = OptionsView.ShowRowHeaders;
				optionsView["ShowFilterHeaders"] = OptionsView.ShowFilterHeaders;
				optionsView["ShowDataHeaders"] = OptionsView.ShowDataHeaders;
				SetOptionsView(true, true, true, true);
			}
			void SetOptionsView(bool showColumnHeaders, bool showRowHeaders, bool showFilterHeaders, bool showDataHeaders) {
				OptionsView.ShowColumnHeaders = showColumnHeaders;
				OptionsView.ShowRowHeaders = showRowHeaders;
				OptionsView.ShowFilterHeaders = showFilterHeaders;
				OptionsView.ShowDataHeaders = showDataHeaders;
			}
			public void RestoreOptionsView() {
				SetOptionsView(optionsView["ShowColumnHeaders"], optionsView["ShowRowHeaders"], optionsView["ShowFilterHeaders"], optionsView["ShowDataHeaders"]);
			}
			static void SynchAppearance(AppearanceObject destAppearance, PrintAppearanceObject sourceAppearance) {
				if(destAppearance == null)
					return;
				destAppearance.Options.UseBackColor = sourceAppearance.IsSetBackColor;
				destAppearance.Options.UseBorderColor = sourceAppearance.IsSetBorderColor;
				destAppearance.Options.UseFont = sourceAppearance.IsSetFont;
				destAppearance.Options.UseForeColor = sourceAppearance.IsSetForeColor;
				XRAppearanceObject xrAppearanceObject = (XRAppearanceObject)sourceAppearance;
				if(xrAppearanceObject.IsSetTextHorizontalAlignment || xrAppearanceObject.IsSetTextVerticalAlignment)
					destAppearance.Options.UseTextOptions = true;
			}
			protected override PivotGridViewInfoData CreateData() {
				XRPivotGridViewInfoData value = new XRPivotGridViewInfoData(this);
				value.ListDataSourceCreated += OnListDataSourceCreated;
				return value;
			}
			void OnListDataSourceCreated(object sender, EventArgs e) {
				if(sender is PivotGridNativeDataSource)
					callback(((PivotGridNativeDataSource)sender).DataController);
			}
			void appearancePrint_Changed(object sender, EventArgs e) {
				CombineAppearances(paintAppearancePrint, AppearancePrint);
			}
			static void CombineAppearances(PivotGridAppearancesPrint dest, PivotGridAppearancesPrint src) {
				if(dest != null && src != null)
					dest.Combine(src, src.GetAppearanceDefaultInfo());
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					if(AppearancePrint != null)
						AppearancePrint.Changed -= new EventHandler(appearancePrint_Changed);
					if(paintAppearancePrint != null) {
						paintAppearancePrint.Dispose();
						paintAppearancePrint = null;
					}
				}
				base.Dispose(disposing);
			}
			protected override void DisposeDataCore() {
				this.fData = null;
			}
		}
		class XRPivotGridViewInfoData : PivotGridViewInfoData {
			public event EventHandler ListDataSourceCreated;
			public XRPivotGridViewInfoData(IViewInfoControl control)
				: base(control) {
			}
			protected override PivotGridFieldCollectionBase CreateFieldCollection() {
				return new XRPivotGridFieldCollection(this);
			}
			protected override PivotGridAppearancesPrint CreatePivotGridAppearancesPrint() {
				return new XRDesignAppearances(this);
			}
			protected override IPivotListDataSource CreateListDataSource() {
				IPivotListDataSource value = base.CreateListDataSource();
				if(ListDataSourceCreated != null)
					ListDataSourceCreated(value, EventArgs.Empty);
				return value;
			}
		}
		class XRPivotGridFieldCollection : PivotGridFieldCollection {
			public XRPivotGridFieldCollection(PivotGridData data)
				: base(data) {
			}
			protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
				PivotGridField field = new XRDesignPivotGridField(fieldName, area);
				if(string.IsNullOrEmpty(fieldName)) {
					XRNameCreationService nameCreationService = new XRNameCreationService(null);
					string name = nameCreationService.CreateNameByType(CollectComponentNames(Data.PivotGrid as IServiceProvider), typeof(XRPivotGridField));
					field.SetNameCore(name);
				} else
					field.SetNameCore(GenerateName(fieldName));
				return field;
			}
			string[] CollectComponentNames(IServiceProvider servProvider) {
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if(servProvider != null) {
					IDesignerHost host = servProvider.GetService<IDesignerHost>();
					if(host != null && host.Container != null) {
						foreach(IComponent component in host.Container.Components)
							dictionary[component.Site.Name] = component.Site.Name;
					}
				}
				foreach(PivotGridField field in this)
					dictionary[field.Name] = field.Name;
				string[] names = new string[dictionary.Count];
				dictionary.Values.CopyTo(names, 0);
				return names;
			}
		}
		class XRDesignPivotGridField : PivotGridField {
			public XRDesignPivotGridField(string fieldName, PivotArea area)
				: base(fieldName, area) {
			}
			protected override PivotGridFieldAppearances CreateAppearance() {
				return new XRDesignPivotGridFieldAppearances(this);
			}
		}
		class XRDesignPivotGridFieldAppearances : PivotGridFieldAppearances {
			const string GrandTotalCellAppearanceName = "GrandTotalCell";
			const string CustomTotalCellAppearanceName = "CustomTotalCell";
			const string TotalCellAppearanceName = "TotalCell";
			const string FieldValueGrandTotalAppearanceName = "FieldValueGrandTotal";
			const string FieldValueTotalAppearanceName = "FieldValueTotal";
			const string FieldValueAppearanceName = "FieldValue";
			const string FieldHeaderAppearanceName = "FieldHeader";
			[DisplayName(TotalCellAppearanceName)]
			public override AppearanceObject CellTotal { get { return base.CellTotal; } }
			[DisplayName(GrandTotalCellAppearanceName)]
			public override AppearanceObject CellGrandTotal { get { return base.CellGrandTotal; } }
			[DisplayName(FieldValueGrandTotalAppearanceName)]
			public override AppearanceObject ValueGrandTotal { get { return base.ValueGrandTotal; } }
			[DisplayName(FieldValueTotalAppearanceName)]
			public override AppearanceObject ValueTotal { get { return base.ValueTotal; } }
			[DisplayName(FieldValueAppearanceName)]
			public override AppearanceObject Value { get { return base.Value; } }
			[DisplayName(FieldHeaderAppearanceName)]
			public override AppearanceObject Header { get { return base.Header; } }
			bool ShouldSerializeCustomTotalCell() { return CustomTotalCell.ShouldSerialize(); }
			void ResetCustomTotalCell() { CustomTotalCell.Reset(); }
			public virtual AppearanceObject CustomTotalCell { get { return customTotalCell; } }
			AppearanceObject customTotalCell;
			public XRDesignPivotGridFieldAppearances(PivotGridField field)
				: base(field) {
				customTotalCell = CreateAppearance(CustomTotalCellAppearanceName);
			}
			protected override AppearanceObject CreateAppearanceInstance(AppearanceObject parent, string name) {
				return new XRDesignAppearanceObject(this, parent, name);
			}
			protected override void CreateAppearances() {
				base.CreateAppearances();
				RenameAppearance(CellGrandTotal, GrandTotalCellAppearanceName);
				RenameAppearance(CellTotal, TotalCellAppearanceName);
				RenameAppearance(ValueGrandTotal, FieldValueGrandTotalAppearanceName);
				RenameAppearance(ValueTotal, FieldValueTotalAppearanceName);
				RenameAppearance(Value, FieldValueAppearanceName);
				RenameAppearance(Header, FieldHeaderAppearanceName);
			}
			void RenameAppearance(AppearanceObject appearance, string name) {
				object key = GetAppearanceName(appearance);
				if(key != null)
					Names.Remove(key);
				Names[name] = appearance;
			}
			object GetAppearanceName(AppearanceObject appearance) {
				foreach(DictionaryEntry item in Names)
					if(ReferenceEquals(item.Value, appearance))
						return item.Key;
				return null;
			}
			public override string ToString() {
				return string.Empty;
			}
		}
		class XRDesignAppearances : PivotGridAppearancesPrint {
			public XRDesignAppearances(PivotGridViewInfoData data)
				: base(data) { }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.Cell"),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject Cell { get { return (XRDesignAppearanceObject)base.Cell; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldHeader"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject FieldHeader { get { return (XRDesignAppearanceObject)base.FieldHeader; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.TotalCell"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject TotalCell { get { return (XRDesignAppearanceObject)base.TotalCell; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.GrandTotalCell"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject GrandTotalCell { get { return (XRDesignAppearanceObject)base.GrandTotalCell; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.CustomTotalCell"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject CustomTotalCell { get { return (XRDesignAppearanceObject)base.CustomTotalCell; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValue"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject FieldValue { get { return (XRDesignAppearanceObject)base.FieldValue; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValueTotal"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject FieldValueTotal { get { return (XRDesignAppearanceObject)base.FieldValueTotal; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.FieldValueGrandTotal"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject FieldValueGrandTotal { get { return (XRDesignAppearanceObject)base.FieldValueGrandTotal; } }
			[
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PivotGrid.XRPivotGridAppearances.Lines"),
		   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
			public new XRDesignAppearanceObject Lines { get { return (XRDesignAppearanceObject)base.Lines; } }
			[
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public new XRDesignAppearanceObject FilterSeparator { get { return (XRDesignAppearanceObject)base.FilterSeparator; } }
			[
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public new XRDesignAppearanceObject HeaderGroupLine { get { return (XRDesignAppearanceObject)base.HeaderGroupLine; } }
			protected override AppearanceObject CreateAppearance(string name, AppearanceObject parent) {
				ValidateAppearanceName(name);
				AppearanceObject appearance = new XRDesignAppearanceObject(this, parent, name);
				InitAppearance(appearance, name);
				return appearance;
			}
		}
		class XRDesignAppearanceObject : AppearanceObject {
			#region hidden properties
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			]
			public override int FontSizeDelta {
				get { return base.FontSizeDelta; }
				set { base.FontSizeDelta = value; }
			}
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			]
			public override FontStyle FontStyleDelta {
				get { return base.FontStyleDelta; }
				set { base.FontStyleDelta = value; }
			}
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			]
			public override LinearGradientMode GradientMode {
				get { return base.GradientMode; }
				set { base.GradientMode = value; }
			}
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			]
			public override TextOptions TextOptions {
				get { return base.TextOptions; }
			}
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			]
			public override AppearanceOptions Options { get { return base.Options; } }
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			]
			public override Image Image {
				get { return base.Image; }
				set { base.Image = value; }
			}
			[
			Browsable(false),
			EditorBrowsable(EditorBrowsableState.Never),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
			XtraSerializableProperty(XtraSerializationVisibility.Hidden),
			]
			public override Color BackColor2 {
				get { return base.BackColor2; }
				set { base.BackColor2 = value; }
			}
			#endregion
			[
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRAppearanceObject.WordWrap"),
			]
			public bool WordWrap {
				get { return TextOptions.WordWrap == Utils.WordWrap.Wrap; }
				set { TextOptions.WordWrap = value ? Utils.WordWrap.Wrap : Utils.WordWrap.NoWrap; }
			}
			void ResetWordWrap() { WordWrap = false; }
			bool ShouldSerializeWordWrap() { return WordWrap != false; }
			[
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DefaultValue(HorzAlignment.Default),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRAppearanceObject.TextHorizontalAlignment"),
			]
			public HorzAlignment TextHorizontalAlignment {
				get { return TextOptions.HAlignment; }
				set { TextOptions.HAlignment = value; }
			}
			[
			Browsable(true),
			EditorBrowsable(EditorBrowsableState.Always),
			DefaultValue(VertAlignment.Default),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
			DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRAppearanceObject.TextVerticalAlignment"),
			]
			public VertAlignment TextVerticalAlignment {
				get { return TextOptions.VAlignment; }
				set { TextOptions.VAlignment = value; }
			}
			public XRDesignAppearanceObject(IAppearanceOwner owner, AppearanceObject parentAppearance, string name)
				: base(owner, parentAppearance, name) {
			}
			public override string ToString() {
				return string.Empty;
			}
		}
		#endregion
		List<ComponentViewInfo> drawInfos = new List<ComponentViewInfo>();
		internal List<ComponentViewInfo> DrawInfos {
			get { return drawInfos; }
		}
		public override ICollection AssociatedComponents {
			get {
				List<IComponent> comps = new List<IComponent>();
				foreach(IComponent c in PivotGrid.Fields) {
					if(c != null && c.Site != null) {
						comps.Add(c);
					}
				}
				return comps;
			}
		}
		XRPivotGrid PivotGrid {
			get { return (XRPivotGrid)Component; }
		}
		public XRPivotGridDesigner()
			: base() {
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Verbs.Add(CreateXRDesignerVerb(DesignSR.Verb_RunDesigner, ReportCommand.VerbPivotGridDesigner, true, false));
			XRControl.BrickCreated += new BrickEventHandlerBase(xrControl_BrickCreated);
			DesignerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(DesignerHost_TransactionClosed);
		}
		void DesignerHost_TransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			EnsureRefreshData(PivotGridData);
		}
		void EnsureRefreshData(PivotGridData data) {
			if(data != null && !data.IsDataBound)
				data.ReloadData();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				XRControl.BrickCreated -= new BrickEventHandlerBase(xrControl_BrickCreated);
				DesignerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(DesignerHost_TransactionClosed);
			}
			base.Dispose(disposing);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new XRPivotGridDesignerActionList1(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(new XRPivotGridDesignerActionList2(this));
		}
		public void RunDesigner() {
			RunDesignerCore(form => {
				DialogRunner.ShowDialog(form, DesignerHost);
			});
		}
#if DEBUGTEST
		public void RunDesignerTest() {
			RunDesignerCore(form => form.Show());
		}
		public void RunDesignerTest(Action<PivotGridEditorForm> action) {
			RunDesignerCore(form => {
				form.Show();
				action(form);
			});
		}
#endif
		PivotGridData PivotGridData {
			get { return ((IPivotGridDataContainer)PivotGrid).Data; }
		}
		void RunDesignerCore(Action<PivotGridEditorForm> action) {
			DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel = LookAndFeelProviderHelper.GetLookAndFeel(this.DesignerHost);
			DataControllerBase dataController = null;
			using(PivotGridEditorForm editor = new PivotGridEditorForm(lookAndFeel)) {
				editor.LookAndFeel.UseDefaultLookAndFeel = true;
				using(FakePivotGridControl fakePivotGrid = new FakePivotGridControl(PivotGrid.Site, item => {
					if(dataController != null)
						dataController.BeforePopulateColumns -= OnBeforePopulateColumns;
					dataController = item;
					dataController.BeforePopulateColumns += OnBeforePopulateColumns;
				})) {
					XmlXtraSerializer serializer = ((IPivotGridViewInfoDataOwner)fakePivotGrid).DataViewInfo.CreateXmlXtraSerializer();
					string appName = PivotGridData.AppName;
					using(Stream stream = new MemoryStream()) {
						serializer.SerializeObject(PivotGrid, stream, appName);
						fakePivotGrid.BeginInit();
						fakePivotGrid.Site = new FakeSite(fakePivotGrid, lookAndFeel);
						stream.Position = 0;
						serializer.DeserializeObject(fakePivotGrid, stream, appName);
						fakePivotGrid.InitializeProperties(PivotGrid.Appearance);
						fakePivotGrid.DataSource = PivotGridData.ListSource;
						fakePivotGrid.OLAPConnectionString = PivotGridData.OLAPConnectionString;
						fakePivotGrid.EndInit();
						editor.InitEditingObject(fakePivotGrid);
						editor.LookAndFeel.SetSkinStyle(lookAndFeel.ActiveSkinName);
						action(editor);
						fakePivotGrid.Site = null;
						fakePivotGrid.RestoreOptionsView();
						using(Stream stream2 = new MemoryStream()) {
							((IPivotGridViewInfoDataOwner)fakePivotGrid).DataViewInfo.CreateXmlXtraSerializer().SerializeObject(fakePivotGrid, stream2, appName);
							stream2.Position = 0;
							PivotGrid.RestoreLayoutFromStream(stream2, null);
						}
						AddFieldsToContainer(PivotGridData.Fields, DesignerHost.Container);
					}
				}
			}
			if(dataController != null)
				dataController.BeforePopulateColumns -= OnBeforePopulateColumns;
			IBandViewInfoService serv = DesignerHost.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
			if(serv != null)
				serv.Invalidate();
		}
		void OnBeforePopulateColumns(object sender, EventArgs e) {
			DataControllerBase dataController = (DataControllerBase)sender;
			IList<PropertyDescriptor> properties = RootReport.GetAppropriatedCalculatedFields(PivotGrid.GetEffectiveDataSource(), PivotGrid.GetEffectiveDataMember());
			if(properties != null) {
				foreach(PropertyDescriptor property in properties)
					dataController.Columns.Add(property);
			}
		}
		void AddFieldsToContainer(PivotGridFieldCollectionBase fields, IContainer container) {
			INameCreationService serv = GetService(typeof(INameCreationService)) as INameCreationService;
			foreach(PivotGridFieldBase field in fields)
				AddFieldToContainer(field, container, serv);
		}
		static void AddFieldToContainer(PivotGridFieldBase field, IContainer container, INameCreationService serv) {
			if(!string.IsNullOrEmpty(field.Name) && serv.IsValidName(field.Name))
				container.Add(field, field.Name);
			else
				container.Add(field);
		}
		public void AddFieldToArea(PivotArea pivotArea) {
			INameCreationService serv = GetService(typeof(INameCreationService)) as INameCreationService;
			string description = string.Format(DesignSR.Trans_Add, typeof(DevExpress.XtraPivotGrid.PivotGridField).Name);
			DesignerTransaction transaction = this.DesignerHost.CreateTransaction(description);
			try {
				this.RaiseComponentChanging(Component, XRComponentPropertyNames.Fields);
				PivotGridFieldBase field = PivotGrid.Fields.Add(string.Empty, pivotArea);
				AddFieldToContainer(field, DesignerHost.Container, (INameCreationService)GetService(typeof(INameCreationService)));
				this.RaiseComponentChanged(Component);
			} finally {
				transaction.Commit();
			}
		}
		public override void OnComponentChanged(ComponentChangedEventArgs e) {
			base.OnComponentChanged(e);
			if(e.Member != null && e.Member.Name == XRComponentPropertyNames.DataSource) {
				ValidateDataAdapter();
				ReportDesigner.SetDataMember(PivotGrid, e.NewValue);
			} else if(e.Member != null && e.Member.Name == XRComponentPropertyNames.DataMember) {
				ValidateDataAdapter();
			}
		}
		protected virtual void ValidateDataAdapter() {
			PivotGrid.DataAdapter = DataAdapterHelper.ValidateDataAdapter(fDesignerHost, PivotGrid.DataSource, PivotGrid.DataMember);
		}
		protected override string[] GetFilteredProperties() {
			List<string> names = new List<string>(XRComponentDesigner.stylePropertyNames);
			names.AddRange(XRComponentDesigner.filterPropertyNames);
			return names.ToArray();
		}
		void xrControl_BrickCreated(object sender, BrickEventArgsBase e) {
			UpdateDrawInfos(e.Brick as PanelBrick);
		}
		void UpdateDrawInfos(PanelBrick brick) {
			drawInfos.Clear();
			if(brick == null)
				return;
			PointF pt = ScreenPointToClient(System.Windows.Forms.Cursor.Position);
			foreach(Brick item in brick.Bricks) {
				DesignPivotBrick designBrick = item as DesignPivotBrick;
				if(designBrick == null)
					continue;
				if(designBrick.Value is Component) {
					RectangleF itemRect = this.zoomService.ToScaledPixels(item.Rect, GraphicsDpi.Document);
					ComponentViewInfo viewInfo = new ComponentViewInfo(itemRect, (Component)item.Value);
					if(GetSelectedComponents().Contains(viewInfo.Component))
						designBrick.MarkAsSelected();
					if(itemRect.Contains(pt)) {
						designBrick.MarkAsHot();
						viewInfo.Hot = true;
					}
					drawInfos.Add(viewInfo);
				}
			}
		}
		internal PointF ScreenPointToClient(Point pt) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(XRControl, fDesignerHost);
			RectangleF bounds = adapter.GetScreenBounds();
			return new PointF(pt.X - bounds.X, pt.Y - bounds.Y);
		}
	}
	class ComponentViewInfo {
		public bool Hot;
		public RectangleF RectanglePx;
		public Component Component;
		public ComponentViewInfo(RectangleF rect, Component component) {
			this.RectanglePx = rect;
			this.Component = component;
		}
	}
}
