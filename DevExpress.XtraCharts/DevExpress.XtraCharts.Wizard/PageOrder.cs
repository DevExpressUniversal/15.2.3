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
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils.Controls;
using DevExpress.XtraCharts.Native;
using System.Windows.Forms;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraTab;
using DevExpress.Utils;
namespace DevExpress.XtraCharts.Wizard {
	public class WizardGroup : IEnumerable {
		internal static readonly string CustructionGroupName = ChartLocalizer.GetString(ChartStringId.WizConstructionGroupName);
		internal static readonly string PresentationGroupName = ChartLocalizer.GetString(ChartStringId.WizPresentationGroupName);
		bool released = false;
		string name;
		PageOrderAlgorithm orderAlg;
		OrderCollection<WizardPage> pageOrder = new OrderCollection<WizardPage>();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardGroupName")]
#endif
		public string Name { 
			get { return name; }
		}
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardGroupPages")]
#endif
		public WizardPage[] Pages { 
			get { return pageOrder.Order; } 
			set { pageOrder.Order = value; } 
		}
		internal int PageCount { 
			get { return pageOrder.Count; } 
		}
		internal WizardGroup(PageOrderAlgorithm orderAlg, string name) {
			this.orderAlg = orderAlg;
			this.name = name;
		}
		public WizardPage RegisterPage(WizardPageType pageType, string label) {
			return RegisterPage(pageType, label, string.Empty, string.Empty, null);
		}
		public WizardPage RegisterPage(WizardPageType pageType, string label, string header, string description, Image image) {
			Type type = PageOrderAlgorithm.DefaultPageMap[pageType];
			return RegisterPage(pageType, type, label, header, description, image);
		}
		public WizardPage RegisterPage(Type pageType, string label) {
			return RegisterPage(pageType, label, string.Empty, string.Empty, null);
		}
		public WizardPage RegisterPage(Type pageType, string label, string header, string description, Image image) {
			return RegisterPage(WizardPageType.UserDefined, pageType, label, header, description, image);
		}
		WizardPage RegisterPage(WizardPageType pageType, Type type, string label, string header, string description, Image image) {
			if (released)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgRegisterPageInUnregisterGroup));
			WizardPage page = WizardPage.CreateWizardPage(this, pageType, type, label, header, description, image);
			this.orderAlg.RegisterPageForUniqueChecking(page);
			this.pageOrder.Register(page);
			return page;
		}
		public void UnregisterPage(WizardPage page) {
			if (!this.pageOrder.Unregister(page))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnregisterPageError));
			this.orderAlg.FreeUniquePage(page);
		}
		public int GetPageIndex(WizardPage page) {
			for(int i = 0; i < Pages.Length; i++)
				if(Pages[i] == page)
					return i;
			return -1;
		}
		internal void Release() {
			this.orderAlg = null;
			this.released = true;
		}
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return this.pageOrder.GetEnumerator();
		}
		#endregion
	}
	public class WizardPage {
		internal static WizardPage CreateWizardPage(WizardGroup group, WizardPageType wizardPageType, Type type, string label, string header, string description, Image image) {
			switch (wizardPageType) {
				case WizardPageType.Axes:
					return new WizardAxisPage(group, WizardPageType.Axes,label, header, description, image);
				case WizardPageType.Panes:
					return new WizardPanePage(group, WizardPageType.Panes, label, header, description, image);
				case WizardPageType.Annotations:
					return new WizardAnnotationPage(group, WizardPageType.Annotations, label, header, description, image);
				case WizardPageType.Data:
					return new WizardDataPage(group, WizardPageType.Data, label, header, description, image);
				case WizardPageType.View:
					return new WizardSeriesViewPage(group, WizardPageType.View, label, header, description, image);
				case WizardPageType.SeriesLabels:
					return new WizardSeriesLabelsPage(group, WizardPageType.SeriesLabels, label, header, description, image);
				case WizardPageType.Diagram:
					return new WizardDiagramPage(group, WizardPageType.Diagram, label, header, description, image);
				case WizardPageType.Legend:
					return new WizardLegendPage(group, WizardPageType.Legend, label, header, description, image);
				case WizardPageType.Titles:
					return new WizardTitlePage(group, WizardPageType.Titles, label, header, description, image);
				case WizardPageType.Chart:
					return new WizardChartPage(group, WizardPageType.Chart, label, header, description, image);
				case WizardPageType.SeriesSettings:
					return new WizardSeriesPage(group, WizardPageType.SeriesSettings, label, header, description, image);
				case WizardPageType.UserDefined:
					return new UserDefinedWizardPage(group, type, label, header, description, image);
				default:
					return new WizardPage(group, wizardPageType, label, header, description, image);
			}
		}
		static Type[] competentTypes = new Type[] { typeof(InternalWizardControlBase), typeof(WizardControlBase) };
		static Image emptyImage;
		string defaultLabel = ChartLocalizer.GetString(ChartStringId.DefaultWizardPageLabel);
		WizardPageType wizardPageType = WizardPageType.UserDefined;
		protected Type type;
		string label;
		string header;
		string description;
		WizardGroup group;
		Image image;
		ILabelControl labelControl;
		IParentControl parentControl;
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageInitializePage")]
#endif
		public event InitializePageEventHandler InitializePage;
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageApplyChanges")]
#endif
		public event ApplyChangesEventHandler ApplyChanges;
		internal Type Type { get { return type; } }
		internal ILabelControl LabelControl { get { return labelControl; } set { labelControl = value; } }
		internal IParentControl ParentControl { get { return parentControl; } set { parentControl = value; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageWizardPageType")]
#endif
		public WizardPageType WizardPageType { get { return wizardPageType; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageLabel")]
#endif
		public string Label { get { return label; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageDescription")]
#endif
		public string Description { get { return description; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageGroup")]
#endif
		public WizardGroup Group { get { return group; } }
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPageImage")]
#endif
		public Image Image { get { return image; } }
		static WizardPage() {
			emptyImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraCharts.Design.Wizard.Images.empty.png", Assembly.GetAssembly(typeof(Chart)));
		}
		internal WizardPage(WizardGroup group, WizardPageType wizardPageType, string label, string header, string description, Image image) : this(group, PageOrderAlgorithm.DefaultPageMap[wizardPageType], label, header, description, image) {
			this.wizardPageType = wizardPageType;
		}
		internal WizardPage(WizardGroup group, Type type, string label, string header, string description, Image image) {
			CheckType(type);
			this.type = type;
			this.group = group;
			this.label = label != string.Empty ? label : defaultLabel;
			this.header = header;
			this.description = description;
			this.image = image != null ? image : emptyImage;
		}
		internal void OnInitializePage(InitializePageEventArgs args) {
			if (InitializePage != null)
				InitializePage(this, args);
		}
		internal void OnValidateHandler(ApplyChangesEventArgs args) {
			if (ApplyChanges != null)
				ApplyChanges(this, args);
		}
		internal void AddControl(InternalWizardControlBase control) {
			PrepareControlsSizeForDecreaseFlicking(control);
			AddNewControl(control);
			UpdateCaptions();
		}
		void CheckType(Type type) {
			if (type.IsAbstract)
				throw new ArgumentException(string.Format(ChartLocalizer.GetString(ChartStringId.MsgWizardAbstractPageType), type.ToString()));
			foreach (Type baseType in competentTypes)
				if (baseType.IsAssignableFrom(type))
					return;
			throw new ArgumentException(string.Format(ChartLocalizer.GetString(ChartStringId.MsgWizardIncorrectBasePageType), 
				type.ToString(), typeof(WizardControlBase).ToString()));
		}
		void FreeControls(Control newControl, Control parentControl) {
			foreach (Control releaseControl in parentControl.Controls) {
				if (releaseControl == newControl)
					continue;
				parentControl.Controls.Remove(releaseControl);
				releaseControl.Dispose();
			}
		}
		void AddNewControl(InternalWizardControlBase control) {
			Control parentControl = this.ParentControl.Control;
			parentControl.SuspendLayout();
			parentControl.Controls.Add(control);
			control.Dock = DockStyle.Fill;
			control.BringToFront();
			control.Focus();
			FreeControls(control, parentControl);
			parentControl.ResumeLayout();
		}
		void PrepareControlsSizeForDecreaseFlicking(InternalWizardControlBase control) {
			Control parentControl = this.parentControl.Control;
			if (parentControl.Tag == null || ((parentControl.Tag != null && (Size)parentControl.Tag == Size.Empty)))
				parentControl.Tag = parentControl.Size;
			Size size = (Size)parentControl.Tag;
			control.Size = size;
		}
		void UpdateCaptions() {
			LabelControl.Highlight();
			ParentControl.SetDescription(this.description);
			ParentControl.SetHeader(this.header);
		}
	}
	public class WizardDataPage : WizardPage {
		DataPageTabCollection filters = new DataPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardDataPageHiddenPageTabs")]
#endif
		public DataPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardDataPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class UserDefinedWizardPage : WizardPage {
		internal UserDefinedWizardPage(WizardGroup group, Type type, string label, string header, string description, Image image) : base (
			group, type, label, header, description, image) {
		}
		public Type UserControlType { get { return type; } }
	}
	public class WizardDiagramPage : WizardPage {
		DiagramPageTabCollection filters = new DiagramPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardDiagramPageHiddenPageTabs")]
#endif
		public DiagramPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardDiagramPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardAxisPage : WizardPage {
		AxisPageTabCollection filters = new AxisPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardAxisPageHiddenPageTabs")]
#endif
		public AxisPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardAxisPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardPanePage : WizardPage {
		PanePageTabCollection filters = new PanePageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardPanePageHiddenPageTabs")]
#endif
		public PanePageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardPanePage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardAnnotationPage : WizardPage {
		AnnotationPageTabCollection filters = new AnnotationPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardAnnotationPageHiddenPageTabs")]
#endif
		public AnnotationPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardAnnotationPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardSeriesViewPage : WizardPage {
		SeriesViewPageTabCollection filters = new SeriesViewPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardSeriesViewPageHiddenPageTabs")]
#endif
		public SeriesViewPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardSeriesViewPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardTitlePage : WizardPage {
		TitlePageTabCollection filters = new TitlePageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardTitlePageHiddenPageTabs")]
#endif
		public TitlePageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardTitlePage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardLegendPage : WizardPage {
		LegendPageTabCollection filters = new LegendPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardLegendPageHiddenPageTabs")]
#endif
		public LegendPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardLegendPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardSeriesLabelsPage : WizardPage {
		SeriesLabelsPageTabCollection filters = new SeriesLabelsPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardSeriesLabelsPageHiddenPageTabs")]
#endif
		public SeriesLabelsPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardSeriesLabelsPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardChartPage : WizardPage {
		ChartPageTabCollection filters = new ChartPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardChartPageHiddenPageTabs")]
#endif
		public ChartPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardChartPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	public class WizardSeriesPage : WizardPage {
		SeriesPageTabCollection filters = new SeriesPageTabCollection();
#if !SL
	[DevExpressXtraChartsWizardLocalizedDescription("WizardSeriesPageHiddenPageTabs")]
#endif
		public SeriesPageTabCollection HiddenPageTabs { get { return filters; } }
		internal WizardSeriesPage(WizardGroup group, WizardPageType pageType, string label, string header, string description, Image image) : base(group, pageType, label, header, description, image) {
		}
	}
	internal class PageOrderAlgorithm : IEnumerable {
		static IDictionary<WizardPageType, Type> defaultPageMap;
		static IDictionary<WizardPageType, Image> imageCache;
		public static IDictionary<WizardPageType, Type> DefaultPageMap { get { return defaultPageMap; } }
		static IDictionary<WizardPageType, Image> LoadImagesFromResources() {
			Dictionary<WizardPageType, Image> images = new Dictionary<WizardPageType, Image>();
			string basePath = ImageResourcesUtils.WizardImagePath;
			Assembly asm = Assembly.GetAssembly(typeof(Chart));
			images.Add(WizardPageType.ChartType, ResourceImageHelper.CreateImageFromResources(basePath + "type.png", asm));
			images.Add(WizardPageType.Appearance, ResourceImageHelper.CreateImageFromResources(basePath + "appearance.png", asm));
			images.Add(WizardPageType.SeriesSettings, ResourceImageHelper.CreateImageFromResources(basePath + "series.png", asm));
			images.Add(WizardPageType.Data, ResourceImageHelper.CreateImageFromResources(basePath + "data.png", asm));
			images.Add(WizardPageType.SeriesLabels, ResourceImageHelper.CreateImageFromResources(basePath + "labels.png", asm));
			images.Add(WizardPageType.Axes, ResourceImageHelper.CreateImageFromResources(basePath + "axes.png", asm));
			images.Add(WizardPageType.Legend, ResourceImageHelper.CreateImageFromResources(basePath + "legend.png", asm));
			images.Add(WizardPageType.Titles, ResourceImageHelper.CreateImageFromResources(basePath + "title.png", asm));
			images.Add(WizardPageType.View, ResourceImageHelper.CreateImageFromResources(basePath + "seriesview.png", asm));
			images.Add(WizardPageType.UserDefined, ResourceImageHelper.CreateImageFromResources(basePath + "empty.png", asm));
			images.Add(WizardPageType.Diagram, ResourceImageHelper.CreateImageFromResources(basePath + "diagram.png", asm));
			images.Add(WizardPageType.Panes, ResourceImageHelper.CreateImageFromResources(basePath + "panes.png", asm));
			images.Add(WizardPageType.Chart, ResourceImageHelper.CreateImageFromResources(basePath + "properties.png", asm));
			images.Add(WizardPageType.Annotations, ResourceImageHelper.CreateImageFromResources(basePath + "annotations.png", asm));
			return images;
		}
		public static PageOrderAlgorithm GenerateDefaultWizard() {
			PageOrderAlgorithm order = new PageOrderAlgorithm();
			WizardGroup group = order.RegisterGroup(WizardGroup.CustructionGroupName);
			group.RegisterPage(
				WizardPageType.ChartType, 
				ChartLocalizer.GetString(ChartStringId.WizChartTypePageName), 
				ChartLocalizer.GetString(ChartStringId.WizChartTypePageName),
				ChartLocalizer.GetString(ChartStringId.WizChartTypePageDescription), 
				imageCache[WizardPageType.ChartType]);
			group.RegisterPage(
				WizardPageType.Appearance, 
				ChartLocalizer.GetString(ChartStringId.WizAppearancePageName),
				ChartLocalizer.GetString(ChartStringId.WizAppearancePageName),
				ChartLocalizer.GetString(ChartStringId.WizAppearancePageDescription), 
				imageCache[WizardPageType.Appearance]);
			group.RegisterPage(
				WizardPageType.SeriesSettings,
				ChartLocalizer.GetString(ChartStringId.WizSeriesPageName),
				ChartLocalizer.GetString(ChartStringId.WizSeriesPageName),
				ChartLocalizer.GetString(ChartStringId.WizSeriesPageDescription), 
				imageCache[WizardPageType.SeriesSettings]);
			group.RegisterPage(
				WizardPageType.Data,
				ChartLocalizer.GetString(ChartStringId.WizDataPageName),
				ChartLocalizer.GetString(ChartStringId.WizDataPageName),
				ChartLocalizer.GetString(ChartStringId.WizDataPageDescription), 
				imageCache[WizardPageType.Data]);
			group = order.RegisterGroup(WizardGroup.PresentationGroupName);
			group.RegisterPage(
				WizardPageType.Chart,
				ChartLocalizer.GetString(ChartStringId.WizChartPageName),
				ChartLocalizer.GetString(ChartStringId.WizChartPageName),
				ChartLocalizer.GetString(ChartStringId.WizChartPageDescription),
				imageCache[WizardPageType.Chart]);
			group.RegisterPage(
				WizardPageType.Diagram,
				ChartLocalizer.GetString(ChartStringId.WizDiagramPageName),
				ChartLocalizer.GetString(ChartStringId.WizDiagramPageName),
				ChartLocalizer.GetString(ChartStringId.WizDiagramPageDescription), 
				imageCache[WizardPageType.Diagram]);
			group.RegisterPage(
				WizardPageType.Panes,
				ChartLocalizer.GetString(ChartStringId.WizPanesPageName),
				ChartLocalizer.GetString(ChartStringId.WizPanesPageName),
				ChartLocalizer.GetString(ChartStringId.WizPanesPageDescription),
				imageCache[WizardPageType.Panes]);
			group.RegisterPage(
				WizardPageType.Axes,
				ChartLocalizer.GetString(ChartStringId.WizAxesPageName),
				ChartLocalizer.GetString(ChartStringId.WizAxesPageName),
				ChartLocalizer.GetString(ChartStringId.WizAxesPageDescription), 
				imageCache[WizardPageType.Axes]);
			group.RegisterPage(
				WizardPageType.View, 
				ChartLocalizer.GetString(ChartStringId.WizSeriesViewPageName),  
				ChartLocalizer.GetString(ChartStringId.WizSeriesViewPageName), 
				ChartLocalizer.GetString(ChartStringId.WizSeriesViewPageDescription),
				imageCache[WizardPageType.View]);
			group.RegisterPage(
				WizardPageType.SeriesLabels,
				ChartLocalizer.GetString(ChartStringId.WizSeriesLabelsPageName),
				ChartLocalizer.GetString(ChartStringId.WizSeriesLabelsPageName),
				ChartLocalizer.GetString(ChartStringId.WizSeriesLabelsPageDescription), 
				imageCache[WizardPageType.SeriesLabels]);
			group.RegisterPage(
				WizardPageType.Titles,
				ChartLocalizer.GetString(ChartStringId.WizChartTitlesPageName),
				ChartLocalizer.GetString(ChartStringId.WizChartTitlesPageName),
				ChartLocalizer.GetString(ChartStringId.WizChartTitlesPageDescription), 
				imageCache[WizardPageType.Titles]);
			group.RegisterPage(
				WizardPageType.Legend,
				ChartLocalizer.GetString(ChartStringId.WizLegendPageName),
				ChartLocalizer.GetString(ChartStringId.WizLegendPageName),
				ChartLocalizer.GetString(ChartStringId.WizLegendPageDescription), 
				imageCache[WizardPageType.Legend]);
			group.RegisterPage(
				WizardPageType.Annotations,
				ChartLocalizer.GetString(ChartStringId.WizAnnotationsPageName),
				ChartLocalizer.GetString(ChartStringId.WizAnnotationsPageName),
				ChartLocalizer.GetString(ChartStringId.WizAnnotationsPageDescription),
				imageCache[WizardPageType.Annotations]);			
			return order;
		}
		static PageOrderAlgorithm() {
			defaultPageMap = new Dictionary<WizardPageType, Type>();
			defaultPageMap.Add(WizardPageType.Appearance, typeof(ChartAppearanceControl));
			defaultPageMap.Add(WizardPageType.Axes, typeof(ChartAxesControl));
			defaultPageMap.Add(WizardPageType.ChartType, typeof(ChartTypeControl));
			defaultPageMap.Add(WizardPageType.Data, typeof(NewChartDataControl));
			defaultPageMap.Add(WizardPageType.View, typeof(SeriesViewConfigControl));
			defaultPageMap.Add(WizardPageType.Legend, typeof(ChartLegendControl));
			defaultPageMap.Add(WizardPageType.SeriesLabels, typeof(SeriesLabelsControl));
			defaultPageMap.Add(WizardPageType.Titles, typeof(ChartTitlesControl));
			defaultPageMap.Add(WizardPageType.SeriesSettings, typeof(SeriesConfigControl));
			defaultPageMap.Add(WizardPageType.Diagram, typeof(ChartDiagramControl));
			defaultPageMap.Add(WizardPageType.Chart, typeof(ChartPropertiesControl));
			defaultPageMap.Add(WizardPageType.Panes, typeof(ChartPanesControl));
			defaultPageMap.Add(WizardPageType.Annotations, typeof(ChartAnnotationsControl));
			imageCache = LoadImagesFromResources();
		}
		OrderCollection<WizardGroup> groupOrder = new OrderCollection<WizardGroup>();
		IList<WizardPage> registeredPages = new List<WizardPage>();
		public int GroupCount { get { return groupOrder.Count; } }
		public IList<WizardPage> UniquePages { get { return registeredPages; } }
		public WizardGroup[] GroupOrder { get { return groupOrder.Order; } set { groupOrder.Order = value; } }
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this.groupOrder.GetEnumerator();
		}
		#endregion
		void FreeUniquePages(WizardGroup group) {
			foreach (WizardPage page in group)
				FreeUniquePage(page);
		}
		void CheckUniqueGroup(string testGroupName) {
			foreach (WizardGroup group in this.groupOrder) 
				if (group.Name == testGroupName)
					throw new ArgumentException(string.Format(
						ChartLocalizer.GetString(ChartStringId.MsgWizardNonUniqueGroupName)));
		}
		bool IsUniquePage(WizardPage testPage) {
			foreach (WizardPage page in registeredPages)
				if (page.Type == testPage.Type)
					return false;
			return true;
		}
		internal int IndexOfGroup(WizardGroup group) {
			for (int i = 0; i < GroupOrder.Length; i++)
				if (GroupOrder[i] == group)
					return i;
			return -1;
		}
		internal WizardGroup FindGroupByName(string groupName) {
			foreach(WizardGroup group in groupOrder.Order)
				if(group.Name == groupName)
					return group;
			return null;
		}
		internal WizardPage FindPageByPageType(WizardPageType pageType) {
			foreach(WizardPage page in registeredPages)
				if(page.WizardPageType == pageType)
					return page;
			return null;
		}
		public WizardGroup RegisterGroup(string groupName) {
			CheckUniqueGroup(groupName);
			WizardGroup group = new WizardGroup(this, groupName);
			this.groupOrder.Register(group);
			return group;
		}
		public void UnregisterGroup(WizardGroup group) {
			if (!this.groupOrder.Unregister(group))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnregisterGroupError));
			FreeUniquePages(group);
			group.Release();
		}
		public void RegisterPageForUniqueChecking(WizardPage page) {
			if (!IsUniquePage(page))
				throw new ArgumentException(string.Format(
					ChartLocalizer.GetString(ChartStringId.MsgWizardNonUniquePageType), page.Type.ToString()));
			this.registeredPages.Add(page);
		}
		public void FreeUniquePage(WizardPage page) {
			this.registeredPages.Remove(page);
		}
	}
	class OrderCollection<OrderClass> : IEnumerable {
		List<OrderClass> order = new List<OrderClass>();
		public OrderClass[] Order {
			get {
				return order.ToArray();
			}
			set {
				CheckOrder(value);
				order.Clear();
				order.AddRange(value);
			}
		}
		public int Count { get { return order.Count; } }
		public void Register(OrderClass item) {
			order.Add(item);
		}
		public bool Unregister(OrderClass item) {
			return order.Remove(item);
		}
		void CheckOrder(OrderClass[] testOrder) {
			if (testOrder.Length != this.order.Count)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgOrderArrayLengthMismatch));
			IList<OrderClass> uniquePages = new List<OrderClass>(testOrder.Length);
			foreach (OrderClass item in testOrder) {
				if (this.order.IndexOf(item) == -1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgOrderUnregisteredElementFound));
				else if (uniquePages.IndexOf(item) != -1)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgOrderRepeatedElementFound));
				uniquePages.Add(item);
			}
		}
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			return this.order.GetEnumerator();
		}
		#endregion
	}
}
