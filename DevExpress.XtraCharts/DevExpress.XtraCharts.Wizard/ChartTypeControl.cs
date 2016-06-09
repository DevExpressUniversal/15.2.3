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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartTypeControl : InternalWizardControlBase {
		#region Nested types
		enum SeriesViewCategoryID {
			All, 					
			Bars,
			PointsAndLines,
			Pie,
			Funnel,
			Areas,
			RangeViews, 
			Radar,
			Advanced, 	
			Custom
		}
		class SeriesViewDescription {
			readonly ViewType viewType;
			readonly int defaultSeriesCount;
			readonly Type type;
			readonly Image image;
			public ViewType ViewType { get { return viewType; } }
			public int DefaultSeriesCount { get { return defaultSeriesCount; } }
			public Type Type { get { return type; } }
			public Image Image { get { return image; } }
			public SeriesViewDescription(ViewType viewType, int defaultSeriesCount) {
				this.viewType = viewType;
				this.defaultSeriesCount = defaultSeriesCount;
				type = SeriesViewFactory.GetType(viewType);
				image = ImageCache[viewType];
			}
		}
		class SeriesViewCategory : IEnumerable {
			readonly SeriesViewCategoryID categoryID;
			readonly List<SeriesViewDescription> descriptions;
			public SeriesViewCategoryID CategoryID { get { return categoryID; } }
			public int Count { get { return descriptions.Count; } }
			public SeriesViewDescription this[int index] { get { return descriptions[index]; } }
			public virtual bool Used { get { return descriptions.Count != 0; } }
			public SeriesViewCategory(SeriesViewCategoryID categoryID, FilterSeriesTypesCollection filterCollection) {
				this.categoryID = categoryID;
				descriptions = SeriesViewCategoryRepository.CreateDescriptions(categoryID);
				FilterDescriptions(filterCollection);
			}
			protected SeriesViewCategory(SeriesViewCategoryID categoryID) : this(categoryID, null) {
			}
			void FilterDescriptions(FilterSeriesTypesCollection filterCollection) {
				if (filterCollection != null)
					for (int i = descriptions.Count - 1; i >= 0; i--) 
						if (filterCollection.Contains(descriptions[i].ViewType))
							descriptions.RemoveAt(i);
			}
			public IEnumerator GetEnumerator() {
				return descriptions.GetEnumerator();
			}
		}
		class CustomSeriesViewCategory : SeriesViewCategory {
			bool used;
			public override bool Used {	get { return used; } }
			public CustomSeriesViewCategory(bool used) : base(SeriesViewCategoryID.Custom) {
				this.used = used;
			}
		}
		class SeriesViewCategoryRepository {
			static List<SeriesViewDescription> CreateBarsDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.Bar, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedBar, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedBar, 2));
				list.Add(new SeriesViewDescription(ViewType.SideBySideStackedBar, 4));
				list.Add(new SeriesViewDescription(ViewType.SideBySideFullStackedBar, 4));
				list.Add(new SeriesViewDescription(ViewType.Bar3D, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedBar3D, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedBar3D, 2));
				list.Add(new SeriesViewDescription(ViewType.SideBySideStackedBar3D, 4));
				list.Add(new SeriesViewDescription(ViewType.SideBySideFullStackedBar3D, 4));
				list.Add(new SeriesViewDescription(ViewType.ManhattanBar, 2));
				return list;
			}
			static List<SeriesViewDescription> CreateRangeViewsDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.RangeBar, 2));
				list.Add(new SeriesViewDescription(ViewType.SideBySideRangeBar, 2));
				list.Add(new SeriesViewDescription(ViewType.RangeArea, 2));
				list.Add(new SeriesViewDescription(ViewType.RangeArea3D, 2));
				return list;
			}
			static List<SeriesViewDescription> CreatePointsAndLinesDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.Point, 2));
				list.Add(new SeriesViewDescription(ViewType.Bubble, 2));
				list.Add(new SeriesViewDescription(ViewType.Line, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedLine, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedLine, 3));
				list.Add(new SeriesViewDescription(ViewType.StepLine, 2));
				list.Add(new SeriesViewDescription(ViewType.Spline, 2));
				list.Add(new SeriesViewDescription(ViewType.ScatterLine, 1));
				list.Add(new SeriesViewDescription(ViewType.SwiftPlot, 2));
				list.Add(new SeriesViewDescription(ViewType.Line3D, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedLine3D, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedLine3D, 3));
				list.Add(new SeriesViewDescription(ViewType.StepLine3D, 2));
				list.Add(new SeriesViewDescription(ViewType.Spline3D, 2));
				return list;
			}
			static List<SeriesViewDescription> CreateAreasDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.Area, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedArea, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedArea, 3));
				list.Add(new SeriesViewDescription(ViewType.StepArea, 2));
				list.Add(new SeriesViewDescription(ViewType.SplineArea, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedSplineArea, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedSplineArea, 2));
				list.Add(new SeriesViewDescription(ViewType.Area3D, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedArea3D, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedArea3D, 2));
				list.Add(new SeriesViewDescription(ViewType.StepArea3D, 2));
				list.Add(new SeriesViewDescription(ViewType.SplineArea3D, 2));
				list.Add(new SeriesViewDescription(ViewType.StackedSplineArea3D, 2));
				list.Add(new SeriesViewDescription(ViewType.FullStackedSplineArea3D, 2));
				return list;
			}
			static List<SeriesViewDescription> CreatePieDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.Pie, 1));
				list.Add(new SeriesViewDescription(ViewType.Doughnut, 1));
				list.Add(new SeriesViewDescription(ViewType.NestedDoughnut, 2));
				list.Add(new SeriesViewDescription(ViewType.Pie3D, 1));
				list.Add(new SeriesViewDescription(ViewType.Doughnut3D, 1));
				return list;
			}
			static List<SeriesViewDescription> CreateFunnelDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.Funnel, 1));
				list.Add(new SeriesViewDescription(ViewType.Funnel3D, 1));
				return list;
			}
			static List<SeriesViewDescription> CreateAdvancedDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.Stock, 1));
				list.Add(new SeriesViewDescription(ViewType.CandleStick, 1));
				list.Add(new SeriesViewDescription(ViewType.Gantt, 2));
				list.Add(new SeriesViewDescription(ViewType.SideBySideGantt, 2));
				return list;
			}
			static List<SeriesViewDescription> CreateRadarAndPolarDescription() {
				Assembly asm = Assembly.GetAssembly(typeof(Chart));
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				list.Add(new SeriesViewDescription(ViewType.RadarPoint, 2));
				list.Add(new SeriesViewDescription(ViewType.RadarLine, 2));
				list.Add(new SeriesViewDescription(ViewType.RadarArea, 2));
				list.Add(new SeriesViewDescription(ViewType.ScatterRadarLine, 2));
				list.Add(new SeriesViewDescription(ViewType.PolarPoint, 2));
				list.Add(new SeriesViewDescription(ViewType.PolarLine, 2));
				list.Add(new SeriesViewDescription(ViewType.PolarArea, 2));
				list.Add(new SeriesViewDescription(ViewType.ScatterPolarLine, 2));
				return list;
			}
			static List<SeriesViewDescription> CreateAllDescriptions() {
				List<SeriesViewDescription> list = new List<SeriesViewDescription>();
				foreach (SeriesViewCategoryID id in Enum.GetValues(typeof(SeriesViewCategoryID)))
					if (id != SeriesViewCategoryID.Custom && id != SeriesViewCategoryID.All)
						list.AddRange(CreateDescriptions(id));
				return list;
			}
			public static List<SeriesViewDescription> CreateDescriptions(SeriesViewCategoryID category) {
				switch (category) {
					case SeriesViewCategoryID.Bars:
						return CreateBarsDescription();
					case SeriesViewCategoryID.PointsAndLines:
						return CreatePointsAndLinesDescription();
					case SeriesViewCategoryID.Pie:
						return CreatePieDescription();
					case SeriesViewCategoryID.Funnel:
						return CreateFunnelDescription();
					case SeriesViewCategoryID.Areas:
						return CreateAreasDescription();
					case SeriesViewCategoryID.Radar:
						return CreateRadarAndPolarDescription();
					case SeriesViewCategoryID.RangeViews:
						return CreateRangeViewsDescription();
					case SeriesViewCategoryID.Advanced:
						return CreateAdvancedDescription();
					case SeriesViewCategoryID.All:
						return CreateAllDescriptions();
					default:
						return new List<SeriesViewDescription>();
				}
			}
			readonly bool useCustom;
			readonly List<SeriesViewCategory> seriesOrder = new List<SeriesViewCategory>();
			public bool UseCustom { get { return useCustom; } }
			public IList<SeriesViewCategory> CategoryOrder { get { return seriesOrder; } }
			public SeriesViewCategory this[int index] {
				get {
					int current = -1;
					for (int i = 0; i < seriesOrder.Count; i++) {
						SeriesViewCategory category = seriesOrder[i];
						if (category.Used)
							current++;
						if (current == index)
							return category;
					}
					return null;
				}
			}
			public SeriesViewCategoryRepository(bool useCustom, FilterSeriesTypesCollection filterCollection) {
				this.useCustom = useCustom;
				seriesOrder.Add(new CustomSeriesViewCategory(this.useCustom));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.All, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.Bars, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.PointsAndLines, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.Pie, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.Funnel, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.Areas, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.RangeViews, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.Radar, filterCollection));
				seriesOrder.Add(new SeriesViewCategory(SeriesViewCategoryID.Advanced, filterCollection));
			}
		}
		class TypePageImageRepository {
			readonly Dictionary<ViewType, Image> imageCache = new Dictionary<ViewType, Image>();
			public Image this[ViewType viewType] { get { return imageCache[viewType]; } }
			public TypePageImageRepository() {
				InitializeImageCache();
			}
			void InitializeImageCache() {
				foreach (ViewType viewType in SeriesViewFactory.ViewTypes)
					imageCache[viewType] = ImageResourcesUtils.GetImageFromResources(SeriesViewFactory.CreateInstance(viewType),SeriesViewImageType.WizardImage);
				using (Brush brush = new SolidBrush(Color.Black))
					using (StringFormat format = new StringFormat(StringFormatFlags.NoClip)) {
						format.Alignment = StringAlignment.Center;
						PutCaptionOnImages(brush, format);
					}
			}
			void PutCaptionOnImages(Brush brush, StringFormat format) {
				foreach (KeyValuePair<ViewType, Image> pair in imageCache)
					PutCaptionOnImage(pair.Value, SeriesViewFactory.GetStringID(pair.Key), brush, format);
			}
			void PutCaptionOnImage(Image image, string caption, Brush brush, StringFormat format) {
				using (Graphics gr = Graphics.FromImage(image)) {
					GraphicsUnit unit = GraphicsUnit.Pixel;
					RectangleF imageBounds = image.GetBounds(ref unit);
					Font font = GetCaptionFont(caption, imageBounds.Width * 0.9f, gr);
					if (font != null) {
						using (font) {
							RectangleF textRect = new RectangleF();
							textRect.Location = new PointF(imageBounds.Width * 0.05f, imageBounds.Height * 0.845f);
							textRect.Size = new SizeF(imageBounds.Width * 0.9f, imageBounds.Height * 0.1f);
							gr.SetClip(textRect);
							gr.DrawString(caption, font, brush, textRect, format);
						}
					}
				}
			}
			Font GetCaptionFont(string caption, float maxWidth, Graphics gr) {
				float emSize = 11;
				while (emSize > 7) {
					Font font = CreateFont(emSize--);
					SizeF size = gr.MeasureString(caption, font);
					if (size.Width < maxWidth)
						return font;
					font.Dispose();
				}
				return CreateFont(emSize);
			}
			Font CreateFont(float emSize) {
				return new Font("Tahoma", emSize);
			}
		}
		#endregion
		static TypePageImageRepository imageCache = new TypePageImageRepository();
		static TypePageImageRepository ImageCache { get { return imageCache; } }
		public static string Title { get { return "Type"; } }
		FilterSeriesTypesCollection filterCollection;
		WizardFormBase form;
		Chart chart;
		Chart originalChart;
		ChartDesignControl designControl;
		SeriesViewCategoryRepository categoryRepository;
		SeriesViewCategory currentCategory;
		Locker locker = new Locker();
		DataContainer DataContainer { get { return chart == null ? null : chart.DataContainer; } }
		SeriesViewBase BaseView {
			get { return (BindingHelper.HasBoundSeries(chart) || chart.Series.Count == 0) ? DataContainer.SeriesTemplate.View : chart.Series[0].View; }
		}
		bool CurrentTypeChanged { get { return !categoryRepository.UseCustom || cbCategory.SelectedIndex > 0; } }
		SeriesViewCategory CurrentCategory { get { return currentCategory; } }
		public ChartTypeControl() {
			InitializeComponent();
			images.OnApplyChanges += new EventHandler(imageListControl_DoubleClick);
		}
		public override void InitializeChart(WizardFormBase form) {
			this.form = form;
			filterCollection = form.FilterSeriesCollection;
			chart = form.Chart;
			originalChart = form.OriginalChart;
			designControl = form.DesignControl;
			designControl.TabIndex = images.TabIndex;
			designControl.Dock = DockStyle.Fill;
			pnlType.Controls.Add(designControl);
			categoryRepository = new SeriesViewCategoryRepository(!TestChartIdentity(), filterCollection);
			FilterComboBox();
			SelectCategory();
		}
		public override bool ValidateContent() {
			try {
				if (CurrentTypeChanged && CurrentCategory.Count > 0) {
					Type viewType = CurrentCategory[images.SelectedIndex].Type;
					using (SeriesViewBase view = (SeriesViewBase)Activator.CreateInstance(viewType)) {
						foreach (Series series in chart.Series)
							SeriesScaleTypeUtils.CheckScaleTypes(series, view);
						SeriesScaleTypeUtils.CheckScaleTypes(DataContainer.SeriesTemplate, view); 
						foreach (Series series in chart.Series)
							SeriesScaleTypeUtils.UpdateScaleTypes(series, view);
						SeriesScaleTypeUtils.UpdateScaleTypes(DataContainer.SeriesTemplate, view);
					}
				}
			}
			catch (Exception ex) {
				originalChart.Container.ShowErrorMessage(ex.Message, ChartLocalizer.GetString(ChartStringId.WizErrorMessageTitle));
				return false;
			}
			return base.ValidateContent();
		}
		public override void CompleteChanges() {
			if (CurrentTypeChanged && CurrentCategory.Count > 0) {
				SeriesViewDescription descr = CurrentCategory[images.SelectedIndex];
				if (chart.Series.Count == 0 && !BindingHelper.HasBoundSeries(chart))
					for (int i = 0; i < descr.DefaultSeriesCount; i++)
						chart.Series.Add(CreateNewSeries(i));
				Type viewType = descr.Type;
				for (int i = 0; i < chart.Series.Count; i++) {
					Series series = chart.Series[i];
					if (!series.IsAutoCreated && series.View.GetType() != viewType)
						series.ChangeView(descr.ViewType);
				}
				if(DataContainer.SeriesTemplate.View.GetType() != viewType)
					DataContainer.SeriesTemplate.ChangeView(descr.ViewType);
			}
		}
		public override void Release() {
			base.Release();
			pnlType.Controls.Remove(designControl);
		}
		void InitializeDefaultStackedGroup(ISupportSeriesGroups view, int seriesIndex) {
			if (view != null) {
				if (seriesIndex < 2)
					view.SeriesGroup = "Group 1";
				else
					view.SeriesGroup = "Group 2";
			}
		}
		Series CreateNewSeries(int seriesIndex) {
			SeriesViewDescription descr = (SeriesViewDescription)CurrentCategory[images.SelectedIndex];
			Series series = new Series(chart.Series.GenerateName(), descr.ViewType);
			ChartDesignHelper.InitializeDefaultGanttScaleType(series);
			InitializeDefaultStackedGroup(series.View as ISupportSeriesGroups, seriesIndex);
			return series;
		}
		bool TestChartIdentity() {
			if (chart.Series.Count == 0)
				return true;
			Type firstSeriesViewType = (BindingHelper.HasBoundSeries(chart) ? DataContainer.SeriesTemplate.View : chart.Series[0].View).GetType();
			for (int i = 0; i < chart.Series.Count; i++)
				if (!chart.Series[i].View.GetType().Equals(firstSeriesViewType))
					return false;
			return true;
		}
		bool SelectImageIfPossible(SeriesViewBase view) {
			for (int i = 0; i < CurrentCategory.Count; i++) {
				SeriesViewDescription description = (SeriesViewDescription)CurrentCategory[i];
				if (description.Type.Equals(view.GetType())) {
					images.SelectedIndex = i;
					return true;
				}
			}
			return false;
		}
		void SelectCategory() {
			locker.Lock();
			try {
				cbCategory.SelectedIndex = 0;
				currentCategory = categoryRepository[0];
				UpdateControls();
				if (images.ItemCount > 0 && !categoryRepository.UseCustom && !SelectImageIfPossible(BaseView))
					images.SelectedIndex = 0;
			}
			finally {
				locker.Unlock();
			}
		}
		void UpdateControls() {
			if (CurrentCategory.CategoryID == SeriesViewCategoryID.Custom) {
				images.Visible = false;
				designControl.Visible = true;
			}
			else {
				imageList.Images.Clear();
				images.Clear();
				if (CurrentCategory.Count > 0) {
					foreach (SeriesViewDescription description in CurrentCategory)
						imageList.Images.Add(description.Image);
					images.Initialize(imageList);
					images.SelectedIndex = 0;
				}
				images.Visible = true;
				designControl.Visible = false;
			}
		}
		void FilterComboBox() {
			IList<SeriesViewCategory> order = this.categoryRepository.CategoryOrder;
			for (int i = order.Count - 1; i >= 0; i--) 
				if (!order[i].Used)
					cbCategory.Properties.Items.RemoveAt(i);
		}
		void cbCategory_SelectedIndexChanged(object sender, EventArgs e) {
			if (!locker.IsLocked) {
				currentCategory = categoryRepository[cbCategory.SelectedIndex];
				UpdateControls();
				if (CurrentTypeChanged)
					SelectImageIfPossible(BaseView);
			}
		}
		void imageListControl_DoubleClick(object sender, EventArgs e) {
			if (images.SelectedIndex != -1)
				form.SelectNextPage();
		}
		internal void FocusImagesControl() {
			images.Focus();
		}
	}
}
