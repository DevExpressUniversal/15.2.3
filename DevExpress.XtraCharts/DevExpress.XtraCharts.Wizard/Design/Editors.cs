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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Wizard.ChartAxesControls;
using DevExpress.Charts.Native;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.Utils.Editors;
using DevExpress.XtraCharts.Designer.Native;
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Design {
	public abstract class ChartEditorBase : UITypeEditor {
		object value;
		object instance;
		protected object Value {
			get { return value; }
			set { this.value = value; }
		}
		protected object Instance { get { return instance; } }
		protected virtual bool ShouldCreateTransaction { get { return true; } }
		protected virtual string TransactionName { get { return String.Empty; } }
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.value = value;
			instance = context.Instance;
			if (provider != null) {
				IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
				if (edSvc != null) {
					Form form = null;
					try {
						if ((this as SummaryFunctionEditor) != null)
							((SummaryFunctionEditor)this).Initialize(provider);
						form = CreateForm();
						if (form != null) {
							DesignerTransaction transaction = null;
							if (ShouldCreateTransaction) {
								IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
								if (designerHost != null && !designerHost.InTransaction) {
									transaction = designerHost.CreateTransaction(TransactionName);
									if (transaction != null)
										Changing();
								}
							}
							try {
								AfterShowDialog(form, edSvc.ShowDialog(form));
							}
							catch {
								if (transaction != null) {
									transaction.Cancel();
									transaction = null;
								}
							}
							if (transaction != null) {
								Changed();
								transaction.Commit();
							}
						}
					}
					finally {
						if (form != null)
							form.Dispose();
						RestoreSelection();
					}
				}
			}
			return this.value;
		}
		protected abstract Form CreateForm();
		protected virtual void Changing() {
		}
		protected virtual void Changed() {
		}
		protected virtual void AfterShowDialog(Form form, DialogResult dialogResult) {
		}
		protected virtual void RestoreSelection() {
		}
		protected virtual object GetChartElementInstance(object designerInstance) {
			object instance = designerInstance;
			if (instance is FakeComponent)
				instance = ((FakeComponent)instance).Object;
			return instance;
		}
	}
	public class KeyCollectionEditor : DevExpress.Utils.UI.CollectionEditor {
		IServiceProvider provider;
		Type lastType = typeof(string);
		Type[] types;
		IWindowsFormsEditorService edSvc = null;
		public KeyCollectionEditor(Type type)
			: base(type) {
			types = new Type[] { typeof(string), typeof(Int32), typeof(double), typeof(DateTime) };
		}
		protected override Type[] CreateNewItemTypes() {
			return types;
		}
		protected override Type CreateCollectionItemType() {
			return typeof(string);
		}
		protected override object CreateInstance(Type itemType) {
			object objValue = null;
			if (provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc != null) {
					try {
						ObjectEditor editor = new ObjectEditor("");
						if (edSvc.ShowDialog(editor) == DialogResult.OK) {
							objValue = editor.EditValue;
						}
					}
					catch { }
				}
			}
			return objValue;
		}
		protected override IList GetObjectsFromInstance(object instance) {
			IList list = base.GetObjectsFromInstance(instance);
			if (list == null || list.Count == 0 || list[0] == null)
				return null;
			return list;
		}
		protected override Utils.UI.CollectionEditorFormBase CreateCollectionForm(IServiceProvider serviceProvider) {
			provider = serviceProvider;
			return base.CreateCollectionForm(serviceProvider);
		}
	}
	public class PaletteTypeEditor : UITypeEditor {
		const int ColorCount = 6;
		static Chart GetChart(ITypeDescriptorContext context) {
			if (context == null)
				return null;
			IChartContainer container = context.Instance as IChartContainer;
			return container == null ? null : container.Chart;
		}
		static PaletteRepository GetPaletteRepository(ITypeDescriptorContext context) {
			Chart chart = GetChart(context);
			if (chart == null)
				return null;
			return context.PropertyDescriptor.Name.StartsWith("Indicators") ? chart.IndicatorsPaletteRepository : chart.PaletteRepository;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			PaletteRepository paletteRepository = GetPaletteRepository(e.Context);
			if (paletteRepository != null) {
				string paletteName = e.Value as string;
				if (!String.IsNullOrEmpty(paletteName)) {
					Graphics gr = e.Graphics;
					Rectangle bounds = e.Bounds;
					Palette palette = paletteRepository[paletteName];
					int count = palette.Count < ColorCount ? palette.Count : ColorCount;
					int x = bounds.X;
					for (int i = 0; i < count; i++) {
						int width = (int)Math.Round((double)(bounds.Right - x) / (count - i));
						using (Brush brush = new SolidBrush(palette[i].Color))
							gr.FillRectangle(brush, new Rectangle(x, bounds.Y, width, bounds.Height));
						x += width;
					}
					return;
				}
			}
			base.PaintValue(e);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (provider == null)
				return val;
			PaletteRepository paletteRepository = GetPaletteRepository(context);
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (paletteRepository == null || edSvc == null)
				return val;
			using (PaletteEditControl ctrl = new PaletteEditControl(edSvc)) {
				ctrl.PaletteRepository = paletteRepository;
				Chart chart = GetChart(context);
				ctrl.SelectedPalette = context.PropertyDescriptor.Name.StartsWith("Indicators") ? chart.IndicatorsPalette : chart.Palette;
				edSvc.DropDownControl(ctrl);
				Palette currentPalette = ctrl.SelectedPalette;
				return currentPalette == null ? val : currentPalette.Name;
			}
		}
	}
	public class ColorizerPaletteTypeEditor : UITypeEditor {
		const int ColorCount = 6;
		static PaletteRepository GetPaletteRepository(ITypeDescriptorContext context) {
			if (context == null)
				return null;
			IPaletteRepositoryProvider provider = context.Instance as IPaletteRepositoryProvider;
			return provider.GetPaletteRepository();
		}
		static ChartPaletteColorizerBase GetColorizer(ITypeDescriptorContext context) {
			ChartPaletteColorizerBase colorizer = context.Instance as ChartPaletteColorizerBase;
			if (colorizer != null)
				return colorizer;
			PaletteChartColorizerBaseModel colorizerModel = context.Instance as PaletteChartColorizerBaseModel;
			if (colorizerModel != null)
				return colorizerModel.Colorizer as ChartPaletteColorizerBase;
			return null;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			PaletteRepository paletteRepository = GetPaletteRepository(e.Context);
			if (paletteRepository != null) {
				string paletteName = e.Value as string;
				if (!String.IsNullOrEmpty(paletteName)) {
					Graphics gr = e.Graphics;
					Rectangle bounds = e.Bounds;
					Palette palette = paletteRepository[paletteName];
					int count = palette.Count < ColorCount ? palette.Count : ColorCount;
					int x = bounds.X;
					for (int i = 0; i < count; i++) {
						int width = (int)Math.Round((double)(bounds.Right - x) / (count - i));
						using (Brush brush = new SolidBrush(palette[i].Color))
							gr.FillRectangle(brush, new Rectangle(x, bounds.Y, width, bounds.Height));
						x += width;
					}
					return;
				}
			}
			base.PaintValue(e);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (provider == null)
				return val;
			PaletteRepository paletteRepository = GetPaletteRepository(context);
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (paletteRepository == null || edSvc == null)
				return val;
			using (PaletteEditControl ctrl = new PaletteEditControl(edSvc)) {
				ctrl.PaletteRepository = paletteRepository;
				ChartPaletteColorizerBase colorizer = GetColorizer(context);
				if (colorizer == null)
					return val;
				ctrl.SelectedPalette = colorizer.Palette;
				edSvc.DropDownControl(ctrl);
				Palette currentPalette = ctrl.SelectedPalette;
				return currentPalette == null ? val : currentPalette.Name;
			}
		}
	}
	public class IndicatorsEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnIndicatorsChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new IndicatorsCollectionForm((IndicatorCollection)collection);
		}
	}
	public class AxisVisibilityInPanesEditor : ChartEditorBase {
		Axis2D axis;
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnAxisVisibilityChanged); } }
		protected override Form CreateForm() {
			object instance = Instance;
			axis = instance as Axis2D;
			if (axis == null) {
				FakeComponent fakeComponent = instance as FakeComponent;
				if (fakeComponent == null)
					return null;
				axis = fakeComponent.Object as Axis2D;
				if (axis == null)
					return null;
			}
			Chart chart = ((IOwnedElement)axis).ChartContainer.Chart;
			AxisVisibilityInPanesForm form = new AxisVisibilityInPanesForm((IDictionary)Value, chart);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void Changing() {
			((IChartElementWizardAccess)axis).RaiseControlChanging();
		}
		protected override void Changed() {
			((IChartElementWizardAccess)axis).RaiseControlChanged();
		}
	}
	public class SeriesPointFilterCollectionEditor : ChartEditorBase {
		PieSeriesViewBase view;
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnExplodedPointsFilters); } }
		protected override Form CreateForm() {
			SeriesPointFilterCollection collection = (SeriesPointFilterCollection)Value;
			object instance = Instance;
			view = instance as PieSeriesViewBase;
			if (view == null) {
				FakeComponent fakeComponent = instance as FakeComponent;
				if (fakeComponent != null)
					view = fakeComponent.Object as PieSeriesViewBase;
				if (view == null)
					return null;
			}
			SeriesPointFilterCollectionForm form = new SeriesPointFilterCollectionForm(collection);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)((IOwnedElement)collection).ChartContainer.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void Changing() {
			((IChartElementWizardAccess)view).RaiseControlChanging();
		}
		protected override void Changed() {
			((IChartElementWizardAccess)view).RaiseControlChanged();
		}
	}
	public class AnchorPointSeriesPointEditor : ChartEditorBase {
		SeriesPoint point;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			point = Value as SeriesPoint;
			if (point == null) {
				AnchorPointSeriesPointItem item = Value as AnchorPointSeriesPointItem;
				if (item == null)
					return null;
				point = item.Point;
				if (point == null)
					return null;
			}
			Chart chart = ((IOwnedElement)point).ChartContainer.Chart;
			if (chart == null)
				return null;
			SeriesPointListForm form = new SeriesPointListForm(chart.Series);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.EditValue = point;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK) {
				SeriesPoint newPoint = ((SeriesPointListForm)form).EditValue;
				if (newPoint != null)
					Value = newPoint;
			}
		}
	}
	class WorkdaysEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (provider == null || !(val is Weekday))
				return val;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (edSvc == null)
				return val;
			using (WorkdaysControl ctrl = new WorkdaysControl((Weekday)val)) {
				edSvc.DropDownControl(ctrl);
				return ctrl.Workdays;
			}
		}
	}
	public class HolidayCollectionEditor : ChartEditorBase {
		string transactionName;
		protected override string TransactionName { get { return transactionName; } }
		protected override Form CreateForm() {
			WorkdaysOptions options = Instance as WorkdaysOptions;
			KnownDateCollection collection = Value as KnownDateCollection;
			if (options == null || collection == null)
				return null;
			bool isHolidays = Object.ReferenceEquals(options.Holidays, collection);
			transactionName = ChartLocalizer.GetString(isHolidays ? ChartStringId.TrnHolidaysChanged : ChartStringId.TrnExactWorkdaysChanged);
			return new HolidaysCollectionForm(collection, ((IOwnedElement)options).ChartContainer, isHolidays);
		}
	}
	public class DataFilterCollectionEditor : ChartEditorBase {
		SeriesBase series;
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnDataFiltersChanged); } }
		protected override Form CreateForm() {
			DataFilterCollection collection = (DataFilterCollection)Value;
			object instance = Instance;
			series = instance as SeriesBase;
			if (series == null) {
				FakeComponent fakeComponent = instance as FakeComponent;
				if (fakeComponent != null)
					series = fakeComponent.Object as SeriesBase;
				if (series == null)
					return null;
			}
			DataFilterCollectionForm form = new DataFilterCollectionForm(collection);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)((IOwnedElement)collection).ChartContainer.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void Changing() {
			((IChartElementWizardAccess)series).RaiseControlChanging();
		}
		protected override void Changed() {
			((IChartElementWizardAccess)series).RaiseControlChanged();
		}
	}
	public class ExplodedPointsEditor : ChartEditorBase {
		PieSeriesViewBase view;
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnExplodedPoints); } }
		protected override Form CreateForm() {
			ExplodedSeriesPointCollection collection = (ExplodedSeriesPointCollection)Value;
			Chart chart = ((IOwnedElement)collection).ChartContainer.Chart;
			object instance = Instance;
			view = instance as PieSeriesViewBase;
			if (view == null) {
				FakeComponent fakeComponent = instance as FakeComponent;
				if (fakeComponent != null)
					view = fakeComponent.Object as PieSeriesViewBase;
				if (view == null)
					return null;
			}
			ExplodedPointsListForm form = new ExplodedPointsListForm(((IOwnedElement)view).Owner as Series, view.ExplodedPoints);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			return form;
		}
		protected override void Changed() {
			((IChartElementWizardAccess)view).RaiseControlChanged();
		}
	}
	public abstract class ChartCollectionEditorBase : ChartEditorBase {
		Chart chart;
		protected Chart Chart { get { return chart; } }
		protected override Form CreateForm() {
			ChartCollectionBase collection = (ChartCollectionBase)Value;
			chart = ((IOwnedElement)collection).ChartContainer.Chart;
			CollectionEditorForm form = CreateForm(collection);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.Initialize(chart);
			return form;
		}
		protected override void Changing() {
			((IChartElementWizardAccess)chart).RaiseControlChanging();
		}
		protected override void Changed() {
			((IChartElementWizardAccess)chart).RaiseControlChanged();
		}
		protected override void RestoreSelection() {
			ChartDesignHelper.SelectObject(chart, ((IOwnedElement)Value).Owner);
		}
		protected abstract CollectionEditorForm CreateForm(ChartCollectionBase collection);
	}
	public class StripEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnStripsChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new StripCollectionEditorForm((StripCollection)collection);
		}
	}
	public class CustomAxisLabelEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnCustomAxisLabelChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new CustomAxisLabelCollectionEditorForm((CustomAxisLabelCollection)collection);
		}
	}
	public class SecondaryAxisXCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxesXChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new SecondaryAxisXCollectionEditorForm(Chart.Container.ServiceProvider, (SecondaryAxisXCollection)collection);
		}
	}
	public class SecondaryAxisYCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxesXChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new SecondaryAxisYCollectionEditorForm(Chart.Container.ServiceProvider, (SecondaryAxisYCollection)collection);
		}
	}
	public class SwiftPlotDiagramSecondaryAxisXCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxesXChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new SwiftPlotDiagramSecondaryAxisXCollectionEditorForm(Chart.Container.ServiceProvider,
				(SwiftPlotDiagramSecondaryAxisXCollection)collection);
		}
	}
	public class SwiftPlotDiagramSecondaryAxisYCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSecondaryAxesXChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new SwiftPlotDiagramSecondaryAxisYCollectionEditorForm(Chart.Container.ServiceProvider,
				(SwiftPlotDiagramSecondaryAxisYCollection)collection);
		}
	}
	public class XYDiagramPaneCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnXYDiagramPanesChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new XYDiagramPaneCollectionEditorForm((XYDiagramPaneCollection)collection);
		}
	}
	public class ScaleBreakCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnScaleBreaksChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new ScaleBreakCollectionEditorForm((ScaleBreakCollection)collection);
		}
		protected override void RestoreSelection() {
			ChartDesignHelper.SelectObject(Chart, ((IOwnedElement)Value).Owner);
		}
	}
	public class SeriesViewEditor : ChartEditorBase {
		SeriesViewBase view;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			view = Value as SeriesViewBase;
			if (view == null)
				return null;
			ViewTypesForm form = new ViewTypesForm();
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)((IOwnedElement)view).ChartContainer.RenderProvider.LookAndFeel;
			form.EditValue = StringResourcesUtils.GetStringId(view);
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK) {
				SeriesViewBase newView = SeriesViewFactory.CreateInstance(((ViewTypesForm)form).EditValue);
				CommonUtils.CopySettings(newView, view);
				Value = newView;
			}
		}
	}
	public class StringCollectionEditor : ChartEditorBase {
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			StringCollectionEditorForm form = new StringCollectionEditorForm((string[])Value);
			ISupportLookAndFeel supportLookAndFeel = Form.ActiveForm as ISupportLookAndFeel;
			if (supportLookAndFeel != null)
				form.LookAndFeel.ParentLookAndFeel = supportLookAndFeel.LookAndFeel;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK)
				Value = ((StringCollectionEditorForm)form).Lines;
		}
	}
	public class SummaryFunctionEditor : ChartEditorBase {
		IServiceProvider serviceProvider;
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSummaryFunctionChanged); } }
		protected override Form CreateForm() {
			SeriesBase series = GetChartElementInstance(Instance) as SeriesBase;
			if (series == null)
				return null;
			SummaryFunctionEditorForm form = new SummaryFunctionEditorForm();
			IChartContainer chartContainer = CommonUtils.FindChartContainer(series);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chartContainer.RenderProvider.LookAndFeel;
			form.Initialize(series, chartContainer.Chart, chartContainer.IsEndUserDesigner, serviceProvider);
			return form;
		}
		public void Initialize(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
	}
	public class DataMemberEditor : UITypeEditor {
		protected object RunPicker(IServiceProvider provider, IChartContainer container, DataContext dataContext, object dataSource, string chartDataMember, string dataMember, ScaleType? filterScaleType) {
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (edSvc == null)
				return dataMember;
			using (DataMemberPickerContainer picker = new DataMemberPickerContainer(edSvc)) {
				ScaleType[] filterTypes = filterScaleType.HasValue ? new ScaleType[] { filterScaleType.Value } : new ScaleType[] { };
				picker.Initialize(dataContext, dataSource, chartDataMember, dataMember, container.ServiceProvider, filterTypes);
				edSvc.DropDownControl(picker);
				return picker.DataMember == null ? dataMember : picker.DataMember;
			}
		}
		protected virtual object GetChartElementInstance(object designerInstance) {
			object instance = designerInstance;
			if (instance is FakeComponent)
				instance = ((FakeComponent)instance).Object;
			return instance;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (provider != null) {
				object instance = context.Instance;
				if (instance != null) {
					instance = GetChartElementInstance(instance);
					DataFilter dataFilter = instance as DataFilter;
					object obj = dataFilter != null ? ((IOwnedElement)dataFilter).Owner : instance;
					object dataSource = null;
					ScaleType? scaleType = null;
					string dataMember = String.Empty;
					SeriesBase series = obj as SeriesBase;
					IChartContainer container = null;
					if (series == null) {
						ValueDataMemberCollection dataMembers = obj as ValueDataMemberCollection;
						if (dataMembers == null) {
							container = obj as IChartContainer;
							if (container != null) {
								DataContainer dataContainer = container.Chart.DataContainer;
								dataSource = dataContainer.ActualDataSource;
								dataMember = dataContainer.DataMember;
							}
						}
						else {
							series = ((IOwnedElement)dataMembers).Owner as SeriesBase;
							dataSource = SeriesDataBindingUtils.GetDataSource(series);
							dataMember = SeriesDataBindingUtils.GetDataMember(series);
							scaleType = series.ValueScaleType;
							container = CommonUtils.FindChartContainer(series);
						}
					}
					else {
						dataSource = SeriesDataBindingUtils.GetDataSource(series);
						dataMember = SeriesDataBindingUtils.GetDataMember(series);
						container = CommonUtils.FindChartContainer(series);
					}
					if (dataSource != null) {
						DataContext dataContext = container != null && container.DataProvider != null ? container.DataProvider.DataContext : null;
						value = RunPicker(provider, container, dataContext, dataSource, dataMember, (string)value, scaleType);
					}
				}
			}
			return value;
		}
	}
	public class AnnotationAnchorPointEditor : ChartEditorBase {
		AnnotationAnchorPoint anchorPoint;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			anchorPoint = Value as AnnotationAnchorPoint;
			Annotation annotation = ((IOwnedElement)anchorPoint).Owner as Annotation;
			if (anchorPoint == null || annotation == null)
				return null;
			Chart chart = ((IOwnedElement)annotation).Owner as Chart;
			if (chart == null)
				return null;
			AnnotationAnchoringTypesForm form = new AnnotationAnchoringTypesForm(annotation);
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.EditValue = anchorPoint;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK) {
				AnnotationAnchorPoint newAnchorPoint = ((AnnotationAnchoringTypesForm)form).EditValue;
				if (newAnchorPoint != null) {
					newAnchorPoint.Assign(anchorPoint);
					Value = newAnchorPoint;
				}
			}
		}
	}
	public class AnnotationShapePositionEditor : ChartEditorBase {
		AnnotationShapePosition position;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			position = Value as AnnotationShapePosition;
			Annotation annotation = ((IOwnedElement)position).Owner as Annotation;
			if (position == null || annotation == null)
				return null;
			Chart chart = ((IOwnedElement)annotation).Owner as Chart;
			if (chart == null)
				return null;
			AnnotationShapePositionTypesForm form = new AnnotationShapePositionTypesForm();
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.EditValue = position;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK) {
				AnnotationShapePosition newPosition = ((AnnotationShapePositionTypesForm)form).EditValue;
				if (newPosition != null) {
					newPosition.Assign(position);
					Value = newPosition;
				}
			}
		}
	}
	public class SeriesCollectionEditor : ChartEditorBase {
		Chart chart;
		IHitTest hitTest;
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSeriesChanged); } }
		protected override Form CreateForm() {
			chart = null;
			hitTest = null;
			Series seriesToEditPoints;
			SeriesCollection coll = Value as SeriesCollection;
			if (coll == null) {
				SeriesPointCollection pointCollection = Value as SeriesPointCollection;
				if (pointCollection == null)
					return null;
				seriesToEditPoints = pointCollection.Owner;
				IChartContainer container = CommonUtils.FindChartContainer(seriesToEditPoints);
				chart = container.Chart;
			}
			else {
				seriesToEditPoints = null;
				var dataContainer = ((IOwnedElement)coll).Owner as DataContainer;
				chart = ((IOwnedElement)dataContainer).Owner as Chart;
			}
			hitTest = chart.HitTestController.Selected;
			SeriesCollectionForm form = new SeriesCollectionForm(chart);
			if (seriesToEditPoints != null)
				form.EditPoints(seriesToEditPoints);
			return form;
		}
		protected override void Changing() {
			((IChartElementWizardAccess)chart).RaiseControlChanging();
		}
		protected override void Changed() {
			((IChartElementWizardAccess)chart).RaiseControlChanged();
		}
		protected override void RestoreSelection() {
			ChartDesignHelper.SelectObject(chart, hitTest);
		}
	}
	public class AnnotationCollectionEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnAnnotationsChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new AnnotationCollectionEditorForm((IAnnotationCollection)collection);
		}
	}
	public class ChartTitleEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnChartTitlesChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new ChartTitleEditorForm((ChartTitleCollection)collection);
		}
		protected override void RestoreSelection() {
			ChartDesignHelper.SelectObject(Chart, Chart);
		}
	}
	public class ConstantLineEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnConstantLinesChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new ConstantLineCollectionEditorForm((ConstantLineCollection)collection);
		}
	}
	public class CrosshairLabelPositionEditor : ChartEditorBase {
		CrosshairLabelPosition position;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			position = Value as CrosshairLabelPosition;
			if (position == null)
				return null;
			Chart chart = ((IOwnedElement)position).Owner.Owner as Chart;
			if (chart == null)
				return null;
			CrosshairLabelPositionTypesForm form = new CrosshairLabelPositionTypesForm();
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.EditValue = position;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK) {
				CrosshairLabelPosition newPosition = ((CrosshairLabelPositionTypesForm)form).EditValue;
				if (newPosition != null) {
					newPosition.Assign(position);
					Value = newPosition;
				}
			}
		}
	}
	public class SeriesTitleEditor : ChartCollectionEditorBase {
		protected override string TransactionName { get { return ChartLocalizer.GetString(ChartStringId.TrnSeriesTitleChanged); } }
		protected override CollectionEditorForm CreateForm(ChartCollectionBase collection) {
			return new SeriesTitleEditorForm((SeriesTitleCollection)collection);
		}
		protected override void RestoreSelection() {
			ChartDesignHelper.SelectObject(Chart, ((IOwnedElement)Value).Owner.Owner);
		}
	}
	public abstract class AxisTypeEditor : UITypeEditor {
		protected abstract XYDiagram2D GetXYDiagram(ITypeDescriptorContext context);
		protected abstract SeriesViewAxisEditControl CreateEditControl(IWindowsFormsEditorService edSvc, XYDiagram2D diagram, object val);
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if (provider == null)
				return val;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			XYDiagram2D diagram = GetXYDiagram(context);
			if (edSvc == null || diagram == null)
				return val;
			SeriesViewAxisEditControl ctrl = CreateEditControl(edSvc, diagram, val);
			if (ctrl == null)
				return val;
			using (ctrl) {
				edSvc.DropDownControl(ctrl);
				return ctrl.CurrentAxis == null ? val : ctrl.CurrentAxis;
			}
		}
	}
	public class SeriesViewAxisXTypeEditor : SeriesViewAxisTypeEditor {
		protected override SeriesViewAxisEditControl CreateEditControl(IWindowsFormsEditorService edSvc, XYDiagram2D diagram, object val) {
			Axis2D currentAxis = val as Axis2D;
			return currentAxis == null ? null : new SeriesViewAxisXEditControl(edSvc, diagram, currentAxis);
		}
	}
	public class SeriesViewAxisYTypeEditor : SeriesViewAxisTypeEditor {
		protected override SeriesViewAxisEditControl CreateEditControl(IWindowsFormsEditorService edSvc, XYDiagram2D diagram, object val) {
			Axis2D currentAxis = val as Axis2D;
			return currentAxis == null ? null : new SeriesViewAxisYEditControl(edSvc, diagram, currentAxis);
		}
	}
	public abstract class SeriesViewAxisTypeEditor : AxisTypeEditor {
		protected override XYDiagram2D GetXYDiagram(ITypeDescriptorContext context) {
			ChartElement holder = context.Instance as ChartElement;
			FakeComponent fakeComponent = context.Instance as FakeComponent;
			if (fakeComponent != null)
				holder = fakeComponent.Object as ChartElement;
			return holder == null ? null : CommonUtils.GetXYDiagram2D(holder);
		}
	}
	public class PaneTypeEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			ChartElement holder = context.Instance as ChartElement;
			FakeComponent fakeComponent = context.Instance as FakeComponent;
			if (fakeComponent != null)
				holder = fakeComponent.Object as ChartElement;
			XYDiagram2D diagram = holder == null ? null : CommonUtils.GetXYDiagram2D(holder);
			XYDiagramPaneBase currentPane = val as XYDiagramPaneBase;
			if (provider == null || diagram == null || currentPane == null)
				return val;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (edSvc == null)
				return val;
			using (SeriesViewPaneEditControl ctrl = new SeriesViewPaneEditControl(edSvc, diagram, currentPane)) {
				edSvc.DropDownControl(ctrl);
				return ctrl.CurrentPane == null ? val : ctrl.CurrentPane;
			}
		}
	}
	public class ToolTipPositionEditor : ChartEditorBase {
		ToolTipPosition position;
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected override Form CreateForm() {
			position = Value as ToolTipPosition;
			if (position == null)
				return null;
			Chart chart = ((IOwnedElement)position).ChartContainer.Chart;
			if (chart == null)
				return null;
			ToolTipPositionTypesForm form = new ToolTipPositionTypesForm();
			form.LookAndFeel.ParentLookAndFeel = (UserLookAndFeel)chart.Container.RenderProvider.LookAndFeel;
			form.EditValue = position;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK) {
				ToolTipPosition newPosition = ((ToolTipPositionTypesForm)form).EditValue;
				if (newPosition != null) {
					newPosition.Assign(position);
					Value = newPosition;
				}
			}
		}
	}
	public class SeriesGroupTypeEditor : UITypeEditor {
		protected virtual object GetChartElementInstance(object designerInstance) {
			object instance = designerInstance;
			if (instance is FakeComponent)
				instance = ((FakeComponent)instance).Object;
			return instance;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			SeriesViewBase view = GetChartElementInstance(context.Instance) as SeriesViewBase;
			ISupportSeriesGroups groupView = view as ISupportSeriesGroups;
			if (provider == null || view == null || groupView == null)
				return val;
			IWindowsFormsEditorService edSvc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			Series series = ((IOwnedElement)view).Owner as Series;
			if (edSvc == null || series == null)
				return val;
			SeriesGroupWrappers wrappers = SeriesGroupsHelper.CreateSeriesGroupWrappers(series);
			using (SeriesGroupSelectControl ctrl = new SeriesGroupSelectControl(edSvc, wrappers, groupView.SeriesGroup)) {
				edSvc.DropDownControl(ctrl);
				return ctrl.CurrentGroup == null ? val : ctrl.CurrentGroup;
			}
		}
	}
	public class ValueDataMemberCollectionEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.None;
		}
	}
	public class PatternEditor : ChartEditorBase {
		protected override bool ShouldCreateTransaction { get { return false; } }
		protected virtual string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is SeriesBase) {
				SeriesBase series = instance as SeriesBase;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForPoint(series);
			}
			return placeholders;
		}
		protected override Form CreateForm() {
			object instance = GetChartElementInstance(Instance);
			PatternEditorForm form = new PatternEditorForm((string)Value, GetAvailablePlaceholders(instance), PatternEditorUtils.CreatePatternValuesSource(instance));
			ISupportLookAndFeel supportLookAndFeel = Form.ActiveForm as ISupportLookAndFeel;
			if (supportLookAndFeel != null)
				form.LookAndFeel.ParentLookAndFeel = supportLookAndFeel.LookAndFeel;
			return form;
		}
		protected override void AfterShowDialog(Form form, DialogResult dialogResult) {
			if (dialogResult == DialogResult.OK)
				Value = ((PatternEditorForm)form).Pattern;
		}
	}
	public class ToolTipPointPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is SeriesBase) {
				SeriesBase series = instance as SeriesBase;
				placeholders = PatternEditorUtils.GetAvailableToolTipPlaceholdersForPoint(series);
			}
			return placeholders;
		}
	}
	public class ToolTipSeriesPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is SeriesBase) {
				SeriesBase series = instance as SeriesBase;
				placeholders = PatternEditorUtils.GetAvailableToolTipPlaceholdersForSeries(series);
			}
			return placeholders;
		}
	}
	public class CrosshairAxisLabelPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is CrosshairAxisLabelOptions) {
				CrosshairAxisLabelOptions labelOptions = instance as CrosshairAxisLabelOptions;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForCrosshairAxisLabel(labelOptions);
			}
			return placeholders;
		}
	}
	public class GroupHeaderPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is CrosshairOptions) {
				CrosshairOptions crosshairOptions = instance as CrosshairOptions;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForCrosshairGroupHeader(crosshairOptions);
			}
			return placeholders;
		}
	}
	public class SeriesLabelTextPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is SeriesLabelBase) {
				SeriesLabelBase label = instance as SeriesLabelBase;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForLabel(label);
			}
			return placeholders;
		}
	}
	public class AxisLabelPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is AxisLabel) {
				AxisLabel axisLabel = instance as AxisLabel;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForAxisLabel(axisLabel);
			}
			return placeholders;
		}
	}
	public class RangeColorizerLegendItemPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is RangeColorizer) {
				RangeColorizer colorizer = instance as RangeColorizer;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForRangeColorizerLegendItem(colorizer);
			}
			return placeholders;
		}
	}
	public class KeyColorColorizerLegendItemPatternEditor : PatternEditor {
		protected override string[] GetAvailablePlaceholders(object instance) {
			string[] placeholders = new string[0];
			if (instance is KeyColorColorizer) {
				KeyColorColorizer colorizer = instance as KeyColorColorizer;
				placeholders = PatternEditorUtils.GetAvailablePlaceholdersForRangeColorizerLegendItem(colorizer);
			}
			return placeholders;
		}
	}
	public class ColorizerPickerEditor : GenericTypePickerEditor<ChartColorizerBase> {
		protected override GenericTypePickerControl<ChartColorizerBase> CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new ColorizerPickerControl(this, value);
		}
	}
	public class ColorizerPickerControl : GenericTypePickerControl<ChartColorizerBase> {
		public ColorizerPickerControl(GenericTypePickerEditor<ChartColorizerBase> editor, object editValue)
			: base(editor, editValue) {
		}
	}
	public abstract class GenericTypePickerEditor<T> : UITypeEditor where T : class {
		IWindowsFormsEditorService edSvc;
		protected abstract GenericTypePickerControl<T> CreateObjectPickerControl(ITypeDescriptorContext context, object value);
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if (!CanEditValue(context, provider))
				return value;
			using (GenericTypePickerControl<T> objectPickerControl = CreateObjectPickerControl(context, value)) {
				objectPickerControl.Initialize(context);
				edSvc.DropDownControl(objectPickerControl);
				object newValue = objectPickerControl.EditValue;
				if (newValue == null)
					value = newValue;
				else {
					Type type = newValue as Type;
					if (type != null && (value == null || type != value.GetType()))
						value = CreateInstanceByType(type, context);
				}
			}
			return value;
		}
		protected virtual bool CanEditValue(ITypeDescriptorContext context, IServiceProvider provider) {
			if (context == null || context.Instance == null || provider == null)
				return false;
			edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc == null)
				return false;
			return true;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.DropDown;
			}
			return base.GetEditStyle(context);
		}
		internal void CloseDropDown() {
			edSvc.CloseDropDown();
		}
		public virtual object CreateInstanceByType(Type type, ITypeDescriptorContext context) {
			return Activator.CreateInstance(type);
		}
	}
	[ToolboxItem(false)]
	public abstract class GenericTypePickerControl<T> : UserControl where T : class {
		public const string NoneString = "(none)";
		readonly GenericTypePickerEditor<T> editor;
		DevExpress.XtraEditors.ListBoxControl listBox;
		object editValue;
		object initialValue;
		Type[] supportedTypes;
		protected internal virtual bool SupportNoneItem { get { return true; } }
		public object EditValue { get { return editValue; } }
		public GenericTypePickerEditor<T> Editor { get { return editor; } }
		protected GenericTypePickerControl(GenericTypePickerEditor<T> editor, object editValue) {
			this.editor = editor;
			this.initialValue = editValue;
			this.editValue = editValue;
			BorderStyle = BorderStyle.None;
			listBox = new DevExpress.XtraEditors.ListBoxControl();
			listBox.Dock = DockStyle.Fill;
			listBox.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			listBox.MouseUp += new MouseEventHandler(lbMouseUp);
			listBox.SelectedIndexChanged += new EventHandler(lbSelectedIndexChanged);
			Controls.Add(listBox);
		}
		protected virtual Type[] GetSupportedTypes(ITypeDescriptorContext context) {
			return ReflectionHelper.GetTypeDescendants(ReflectionHelper.XtraChartAssembly, typeof(T), new List<Type>());
		}
		public virtual void Initialize(ITypeDescriptorContext context) {
			listBox.BeginUpdate();
			if (SupportNoneItem)
				listBox.Items.Add(NoneString);
			supportedTypes = GetSupportedTypes(context);
			foreach (Type type in supportedTypes){
				string name;
				if (type.IsSubclassOf(typeof(DesignerChartElementModelBase)) && type.Name.EndsWith("Model"))
					name = type.Name.Substring(0, type.Name.Length - "Model".Length);
				else
					name = type.Name;
				listBox.Items.Add(name);
			}
			listBox.EndUpdate();
			if (initialValue != null) {
				listBox.SelectedValue = initialValue.GetType().Name;
			}
			else {
				if (SupportNoneItem) listBox.SelectedIndex = 0;
			}
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			this.listBox.Focus();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if (keyData == Keys.Enter) {
				editor.CloseDropDown();
				return true;
			}
			if (keyData == Keys.Escape) {
				editValue = initialValue;
				editor.CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		void lbSelectedIndexChanged(object sender, EventArgs e) {
			int index = SupportNoneItem ? listBox.SelectedIndex - 1 : listBox.SelectedIndex;
			editValue = index >= 0 ? supportedTypes[index] : null;
		}
		void lbMouseUp(object sender, MouseEventArgs e) {
			editor.CloseDropDown();
		}
	}
	public abstract class GradientModeEditorBase<TGradientMode> : UITypeEditor {
		protected virtual object GetChartElementInstance(object designerInstance) {
			return designerInstance;
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			if (e.Value != null) {
				IGradientFillOptions<TGradientMode> options = GetChartElementInstance(e.Context.Instance) as IGradientFillOptions<TGradientMode>;
				if (options != null) {
					IGradientPainter painter = options.GetPainter((TGradientMode)e.Value, null);
					if (painter != null) {
						painter.FillPolygon(e.Graphics, new RectanglePolygon(e.Bounds), Color.Black, Color.White);
						return;
					}
				}
			}
			base.PaintValue(e);
		}
	}
	public class RectangleGradientModeEditor : GradientModeEditorBase<RectangleGradientMode> {
	}
	public class PolygonGradientModeEditor : GradientModeEditorBase<PolygonGradientMode> {
	}
	public class HatchStyleTypeEditor : UITypeEditor {
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override void PaintValue(PaintValueEventArgs e) {
			if (e.Value == null || !(e.Value is HatchStyle))
				base.PaintValue(e);
			else
				using (Brush brush = new HatchBrush((HatchStyle)e.Value, Color.Black, Color.White))
					e.Graphics.FillRectangle(brush, e.Bounds);
		}
	}
}
