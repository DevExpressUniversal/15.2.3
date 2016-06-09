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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc.UI {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	public static class HtmlHelperExtension {
		const string HtmlHelperKey = "DXHtmlHelper";
		public static ExtensionsFactory<ModelType> DevExpress<ModelType>(this HtmlHelper<ModelType> helper) {
			Prepare(helper);
			return ExtensionsFactory<ModelType>.InstanceInternal;
		}
		public static ExtensionsFactory DevExpress(this HtmlHelper helper) {
			Prepare(helper);
			return ExtensionsFactory.InstanceInternal;
		}
		static void Prepare(this HtmlHelper helper) {
			HttpUtils.SetContextValue<HtmlHelper>(HtmlHelperKey, helper);
			MvcUrlResolutionService.Initialize();
		}
		internal static HtmlHelper HtmlHelper {
			get { return HttpUtils.GetContextValue<HtmlHelper>(HtmlHelperKey, null); }
		}
		internal static ViewContext ViewContext {
			get { return (HtmlHelper != null) ? HtmlHelper.ViewContext : null; }
		}
	}
	public enum ExtensionType {
		GridView, GridLookup, HtmlEditor,
		CallbackPanel, Panel, Menu, NavBar, Ribbon, PopupControl, RatingControl, RoundPanel, Splitter, PageControl, TabControl, TreeView, UploadControl,
		Button, BinaryImage, ButtonEdit, Calendar, Captcha, CheckBox, CheckBoxList, ColorEdit, ComboBox, DateEdit, DropDownEdit, TrackBar, TokenBox,
		HyperLink, Image, Label, ListBox, Memo, ProgressBar, RadioButton, RadioButtonList, SpinEdit, TextBox, TimeEdit, ValidationSummary,
		Chart,
		ReportDesigner, WebDocumentViewer, DocumentViewer, [EditorBrowsable(EditorBrowsableState.Never)]ReportViewer, [EditorBrowsable(EditorBrowsableState.Never)]ReportToolbar,
		PivotGrid, PivotCustomizationExtension, PopupMenu, LoadingPanel, Scheduler, TimeZoneEdit,
		AppointmentRecurrenceForm, SchedulerStatusInfo, DateNavigator, DockManager, DockPanel, DockZone, DataView, TreeList, FileManager,
		ImageSlider, ImageGallery, ImageZoom, ImageZoomNavigator, FormLayout,
		Icons16x16, Icons16x16gray, Icons16x16office2013, Icons32x32, Icons32x32gray, Icons32x32office2013, Icons16x16devav, Icons32x32devav,
		DashboardViewer, Spreadsheet, SpellChecker, RichEdit, CardView
	}
	public enum ExtensionSuite {
		All, NavigationAndLayout, Editors, GridView, HtmlEditor, Chart, Report, PivotGrid, Scheduler, TreeList, Icons, DashboardViewer, Spreadsheet, SpellChecker, RichEdit, CardView
	}
	public class ExtensionsFactory<ModelType> : ExtensionsFactory {
		static ExtensionsFactory<ModelType> instance = new ExtensionsFactory<ModelType>();
		internal static new ExtensionsFactory<ModelType> InstanceInternal {
			get { return instance; }
		}
		public FormLayoutExtension<ModelType> FormLayout(FormLayoutSettings<ModelType> settings) {
			return CreateExtension<FormLayoutExtension<ModelType>, FormLayoutSettings<ModelType>>(settings);
		}
		public FormLayoutExtension<ModelType> FormLayout(Action<FormLayoutSettings<ModelType>> method) {
			return CreateExtension<FormLayoutExtension<ModelType>, FormLayoutSettings<ModelType>>(method);
		}
		public LabelExtension LabelFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return LabelFor(expression, null);
		}
		public LabelExtension LabelFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<LabelSettings> method) {
			ModelMetadata metadata = GetModelMetadataByExpression(expression);
			string fieldName = ExpressionHelper.GetExpressionText(expression);
			LabelSettings settings = new LabelSettings() {
				Text = metadata.DisplayName ?? metadata.PropertyName,
				AssociatedControlName = HtmlHelperExtension.HtmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName),
				ToolTip = metadata.Description
			};
			if (method != null)
				method(settings);
			return CreateExtension<LabelExtension, LabelSettings>(settings);
		}
		public TextBoxExtension TextBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return TextBoxFor(expression, null);
		}
		public TextBoxExtension TextBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<TextBoxSettings> method) {
			return (TextBoxExtension)EditorInternalFor(expression, typeof(TextBoxExtension), method);
		}
		public BinaryImageEditExtension BinaryImageFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return BinaryImageFor(expression, null);
		}
		public BinaryImageEditExtension BinaryImageFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<BinaryImageEditSettings> method) {
			return (BinaryImageEditExtension)EditorInternalFor(expression, typeof(BinaryImageEditExtension), method);
		}
		public ButtonEditExtension ButtonEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return ButtonEditFor(expression, null);
		}
		public ButtonEditExtension ButtonEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<ButtonEditSettings> method) {
			return (ButtonEditExtension)EditorInternalFor(expression, typeof(ButtonEditExtension), method);
		}
		public CheckBoxExtension CheckBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return CheckBoxFor(expression, null);
		}
		public CheckBoxExtension CheckBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<CheckBoxSettings> method) {
			return (CheckBoxExtension)EditorInternalFor(expression, typeof(CheckBoxExtension), method);
		}
		public CheckBoxListExtension CheckBoxListFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return CheckBoxListFor(expression, null);
		}
		public CheckBoxListExtension CheckBoxListFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<CheckBoxListSettings> method) {
			Action<CheckBoxListSettings> assignAdditionalSettingsMethod = settings => {
				Type valueType = GetValueTypeByExpression(expression);
				if (valueType != null)
					settings.Properties.ValueType = valueType;
				if (method != null)
					method(settings);
			};
			return (CheckBoxListExtension)EditorInternalFor(expression, typeof(CheckBoxListExtension), assignAdditionalSettingsMethod);
		}
		public CalendarExtension CalendarFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return CalendarFor(expression, null);
		}
		public CalendarExtension CalendarFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<CalendarSettings> method) {
			return (CalendarExtension)EditorInternalFor(expression, typeof(CalendarExtension), method);
		}
		public ColorEditExtension ColorEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return ColorEditFor(expression, null);
		}
		public ColorEditExtension ColorEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<ColorEditSettings> method) {
			return (ColorEditExtension)EditorInternalFor(expression, typeof(ColorEditExtension), method);
		}
		public ComboBoxExtension ComboBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return ComboBoxFor(expression, null);
		}
		public ComboBoxExtension ComboBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<ComboBoxSettings> method) {
			Action<ComboBoxSettings> assignAdditionalSettingsMethod = settings => {
				Type valueType = GetValueTypeByExpression(expression);
				if (valueType != null)
					settings.Properties.ValueType = valueType;
				if (method != null)
					method(settings);
			};
			return (ComboBoxExtension)EditorInternalFor(expression, typeof(ComboBoxExtension), assignAdditionalSettingsMethod);
		}
		public TokenBoxExtension TokenBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return TokenBoxFor(expression, null);
		}
		public TokenBoxExtension TokenBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<TokenBoxSettings> method) {
			return (TokenBoxExtension)EditorInternalFor(expression, typeof(TokenBoxExtension), method);
		}
		public DateEditExtension DateEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return DateEditFor(expression, null);
		}
		public DateEditExtension DateEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<DateEditSettings> method) {
			return (DateEditExtension)EditorInternalFor(expression, typeof(DateEditExtension), method);
		}
		public DropDownEditExtension DropDownEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return DropDownEditFor(expression, null);
		}
		public DropDownEditExtension DropDownEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<DropDownEditSettings> method) {
			return (DropDownEditExtension)EditorInternalFor(expression, typeof(DropDownEditExtension), method);
		}
		public HyperLinkExtension HyperLinkFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return HyperLinkFor(expression, null);
		}
		public HyperLinkExtension HyperLinkFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<HyperLinkSettings> method) {
			return (HyperLinkExtension)EditorInternalFor(expression, typeof(HyperLinkExtension), method);
		}
		public ImageEditExtension ImageFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return ImageFor(expression, null);
		}
		public ImageEditExtension ImageFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<ImageEditSettings> method) {
			return (ImageEditExtension)EditorInternalFor(expression, typeof(ImageEditExtension), method);
		}
		public ListBoxExtension ListBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return ListBoxFor(expression, null);
		}
		public ListBoxExtension ListBoxFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<ListBoxSettings> method) {
			Action<ListBoxSettings> assignAdditionalSettingsMethod = settings => {
				Type valueType = GetValueTypeByExpression(expression);
				if (valueType != null)
					settings.Properties.ValueType = valueType;
				if (method != null)
					method(settings);
			};
			return (ListBoxExtension)EditorInternalFor(expression, typeof(ListBoxExtension), assignAdditionalSettingsMethod);
		}
		public MemoExtension MemoFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return MemoFor(expression, null);
		}
		public MemoExtension MemoFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MemoSettings> method) {
			return (MemoExtension)EditorInternalFor(expression, typeof(MemoExtension), method);
		}
		public ProgressBarExtension ProgressBarFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return ProgressBarFor(expression, null);
		}
		public ProgressBarExtension ProgressBarFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<DevExpress.Web.Mvc.ProgressBarSettings> method) {
			return (ProgressBarExtension)EditorInternalFor(expression, typeof(ProgressBarExtension), method);
		}
		public RadioButtonListExtension RadioButtonListFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return RadioButtonListFor(expression, null);
		}
		public RadioButtonListExtension RadioButtonListFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<RadioButtonListSettings> method) {
			Action<RadioButtonListSettings> assignAdditionalSettingsMethod = settings => {
				Type valueType = GetValueTypeByExpression(expression);
				if (valueType != null)
					settings.Properties.ValueType = valueType;
				if (method != null)
					method(settings);
			};
			return (RadioButtonListExtension)EditorInternalFor(expression, typeof(RadioButtonListExtension), assignAdditionalSettingsMethod);
		}
		public SpinEditExtension SpinEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return SpinEditFor(expression, null);
		}
		public SpinEditExtension SpinEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<SpinEditSettings> method) {
			return (SpinEditExtension)EditorInternalFor(expression, typeof(SpinEditExtension), method);
		}
		public TimeEditExtension TimeEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return TimeEditFor(expression, null);
		}
		public TimeEditExtension TimeEditFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<TimeEditSettings> method) {
			return (TimeEditExtension)EditorInternalFor(expression, typeof(TimeEditExtension), method);
		}
		public TrackBarExtension TrackBarFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return TrackBarFor(expression, null);
		}
		public TrackBarExtension TrackBarFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<TrackBarSettings> method) {
			Action<TrackBarSettings> assignAdditionalSettingsMethod = settings => {
				Type valueType = GetValueTypeByExpression(expression);
				if (valueType != null)
					settings.Properties.ValueType = valueType;
				if (method != null)
					method(settings);
			};
			return (TrackBarExtension)EditorInternalFor(expression, typeof(TrackBarExtension), assignAdditionalSettingsMethod);
		}
		public RadioButtonExtension RadioButtonFor<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return RadioButtonFor(expression, null);
		}
		public RadioButtonExtension RadioButtonFor<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<RadioButtonSettings> method) {
			return (RadioButtonExtension)EditorInternalFor(expression, typeof(RadioButtonExtension), method);
		}
		EditorExtension EditorInternalFor<ValueType, SettingsType>(Expression<Func<ModelType, ValueType>> expression, Type extensionType,
				Action<SettingsType> method) where SettingsType : EditorSettings {
			ModelMetadata metadata = GetModelMetadataByExpression(expression);
			SettingsType settings = Activator.CreateInstance<SettingsType>();
			settings.Name = ExtensionsHelper.GetFullHtmlFieldName(expression);
			ExtensionsHelper.ConfigureEditPropertiesByMetadata(settings.Properties, metadata);
			if(method != null)
				method(settings);
			EditorExtension extension = (EditorExtension)CreateExtension(extensionType, settings, HtmlHelperExtension.ViewContext, metadata);
			return extension.Bind(metadata.Model ?? GetModelStateValue(settings.Name, GetValueTypeByExpression(expression)));
		}
		Type GetValueTypeByExpression<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			ModelMetadata metadata = GetModelMetadataByExpression(expression);
			Type modelType = metadata != null && metadata.ModelType != null ? metadata.ModelType : null;
			return modelType != null ? Nullable.GetUnderlyingType(modelType) ?? modelType : null;
		}
		ModelMetadata GetModelMetadataByExpression<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			var typedHtmlHelper = HtmlHelperExtension.HtmlHelper as HtmlHelper<ModelType>;
			if(typedHtmlHelper != null)
				return ModelMetadata.FromLambdaExpression(expression, typedHtmlHelper.ViewData);
			return ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<ModelType>());
		}
		object GetModelStateValue(string key, Type destinationType) {
			ModelState state;
			if(HtmlHelperExtension.HtmlHelper.ViewData.ModelState.TryGetValue(key, out state) && state.Value != null)
				return state.Value.ConvertTo(destinationType, System.Globalization.CultureInfo.InvariantCulture);
			return null;
		}
	}
	public class ExtensionsFactory {
		static Dictionary<string, ExtensionBase> extensions = new Dictionary<string, ExtensionBase>();
		static ExtensionsFactory instance = new ExtensionsFactory();
		protected ExtensionsFactory() : base() {
		}
		protected internal static Dictionary<string, ExtensionBase> RenderedExtensions {
			get {
				if(HttpContext.Current == null)
					return extensions;
				return HttpUtils.GetContextObject<Dictionary<string, ExtensionBase>>("DXMvcExtensions");
			}
		}
		[Obsolete("Use the HtmlHelper.DevExpress() method instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public static ExtensionsFactory Instance {
			get { return InstanceInternal; }
		}
		[Obsolete("Use the DevExpressHelper.Theme property instead."),
		EditorBrowsable(EditorBrowsableState.Never)]
		public static string Theme {
			get { return DevExpressHelper.Theme; }
			set { DevExpressHelper.Theme = value; }
		}
		internal static ExtensionsFactory InstanceInternal {
			get { return instance; }
		}
		public SchedulerExtension Scheduler(SchedulerSettings settings) {
			return CreateExtension<SchedulerExtension, SchedulerSettings>(settings);
		}
		public SchedulerExtension Scheduler(Action<SchedulerSettings> method) {
			return CreateExtension<SchedulerExtension, SchedulerSettings>(method);
		}
		public TimeZoneEditExtension TimeZoneEdit(TimeZoneEditSettings settings) {
			return CreateExtension<TimeZoneEditExtension, TimeZoneEditSettings>(settings);
		}
		public TimeZoneEditExtension TimeZoneEdit(Action<TimeZoneEditSettings> method) {
			return CreateExtension<TimeZoneEditExtension, TimeZoneEditSettings>(method);
		}
		public AppointmentRecurrenceFormExtension AppointmentRecurrenceForm(AppointmentRecurrenceFormSettings settings) {
			return CreateExtension<AppointmentRecurrenceFormExtension, AppointmentRecurrenceFormSettings>(settings);
		}
		public AppointmentRecurrenceFormExtension AppointmentRecurrenceForm(Action<AppointmentRecurrenceFormSettings> method) {
			return CreateExtension<AppointmentRecurrenceFormExtension, AppointmentRecurrenceFormSettings>(method);
		}
		public SchedulerStatusInfoExtension SchedulerStatusInfo(SchedulerStatusInfoSettings settings) {
			return CreateExtension<SchedulerStatusInfoExtension, SchedulerStatusInfoSettings>(settings);
		}
		public SchedulerStatusInfoExtension SchedulerStatusInfo(Action<SchedulerStatusInfoSettings> method) {
			return CreateExtension<SchedulerStatusInfoExtension, SchedulerStatusInfoSettings>(method);
		}
		public DateNavigatorExtension DateNavigator(SchedulerSettings settings) {
			return CreateExtension<DateNavigatorExtension, SchedulerSettings>(settings);
		}
		public DateNavigatorExtension DateNavigator(Action<SchedulerSettings> method) {
			return CreateExtension<DateNavigatorExtension, SchedulerSettings>(method);
		}
		public ReportDesignerExtension ReportDesigner(ReportDesignerSettings settings) {
			return CreateExtension<ReportDesignerExtension, ReportDesignerSettings>(settings);
		}
		public ReportDesignerExtension ReportDesigner(Action<ReportDesignerSettings> method) {
			return CreateExtension<ReportDesignerExtension, ReportDesignerSettings>(method);
		}
		public WebDocumentViewerExtension WebDocumentViewer(WebDocumentViewerSettings settings) {
			return CreateExtension<WebDocumentViewerExtension, WebDocumentViewerSettings>(settings);
		}
		public WebDocumentViewerExtension WebDocumentViewer(Action<WebDocumentViewerSettings> method) {
			return CreateExtension<WebDocumentViewerExtension, WebDocumentViewerSettings>(method);
		}
		public DocumentViewerExtension DocumentViewer(DocumentViewerSettings settings) {
			return CreateExtension<DocumentViewerExtension, DocumentViewerSettings>(settings);
		}
		public DocumentViewerExtension DocumentViewer(Action<DocumentViewerSettings> method) {
			return CreateExtension<DocumentViewerExtension, DocumentViewerSettings>(method);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ReportToolbarExtension ReportToolbar(ReportToolbarSettings settings) {
			return CreateExtension<ReportToolbarExtension, ReportToolbarSettings>(settings);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ReportToolbarExtension ReportToolbar(Action<ReportToolbarSettings> method) {
			return CreateExtension<ReportToolbarExtension, ReportToolbarSettings>(method);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ReportViewerExtension ReportViewer(ReportViewerSettings settings) {
			return CreateExtension<ReportViewerExtension, ReportViewerSettings>(settings);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ReportViewerExtension ReportViewer(Action<ReportViewerSettings> method) {
			return CreateExtension<ReportViewerExtension, ReportViewerSettings>(method);
		}
		public ChartControlExtension Chart(ChartControlSettings settings) {
			return CreateExtension<ChartControlExtension, ChartControlSettings>(settings);
		}
		public ChartControlExtension Chart(Action<ChartControlSettings> method) {
			return CreateExtension<ChartControlExtension, ChartControlSettings>(method);
		}
		public PivotGridExtension PivotGrid(PivotGridSettings settings) {
			return CreateExtension<PivotGridExtension, PivotGridSettings>(settings);
		}
		public PivotGridExtension PivotGrid(Action<PivotGridSettings> method) {
			return CreateExtension<PivotGridExtension, PivotGridSettings>(method);
		}
		public PivotCustomizationExtension PivotCustomizationExtension(PivotGridSettings settings) {
			return CreateExtension<PivotCustomizationExtension, PivotGridSettings>(settings);
		}
		public PivotCustomizationExtension PivotCustomizationExtension(Action<PivotGridSettings> method) {
			return CreateExtension<PivotCustomizationExtension, PivotGridSettings>(method);
		}
		public GridViewExtension GridView(GridViewSettings settings) {
			return CreateExtension<GridViewExtension, GridViewSettings>(settings);
		}
		public GridViewExtension GridView(Action<GridViewSettings> method) {
			return CreateExtension<GridViewExtension, GridViewSettings>(method);
		}
		public GridViewExtension<RowType> GridView<RowType>(GridViewSettings<RowType> settings) where RowType: class {
			return CreateExtension<GridViewExtension<RowType>, GridViewSettings<RowType>>(settings);
		}
		public GridViewExtension<RowType> GridView<RowType>(Action<GridViewSettings<RowType>> method) where RowType: class {
			return CreateExtension<GridViewExtension<RowType>, GridViewSettings<RowType>>(method);
		}
		public GridLookupExtension GridLookup(GridLookupSettings settings) {
			return CreateExtension<GridLookupExtension, GridLookupSettings>(settings);
		}
		public GridLookupExtension GridLookup(Action<GridLookupSettings> method) {
			return CreateExtension<GridLookupExtension, GridLookupSettings>(method);
		}
		public CardViewExtension CardView(CardViewSettings settings) {
			return CreateExtension<CardViewExtension, CardViewSettings>(settings);
		}
		public CardViewExtension CardView(Action<CardViewSettings> method) {
			return CreateExtension<CardViewExtension, CardViewSettings>(method);
		}
		public CardViewExtension<CardType> CardView<CardType>(CardViewSettings<CardType> settings) where CardType : class {
			return CreateExtension<CardViewExtension<CardType>, CardViewSettings<CardType>>(settings);
		}
		public CardViewExtension<CardType> CardView<CardType>(Action<CardViewSettings<CardType>> method) where CardType : class {
			return CreateExtension<CardViewExtension<CardType>, CardViewSettings<CardType>>(method);
		}
		public HtmlEditorExtension HtmlEditor(HtmlEditorSettings settings) {
			return CreateExtension<HtmlEditorExtension, HtmlEditorSettings>(settings);
		}
		public HtmlEditorExtension HtmlEditor(Action<HtmlEditorSettings> method) {
			return CreateExtension<HtmlEditorExtension, HtmlEditorSettings>(method);
		}
		public SpellCheckerExtension SpellChecker(SpellCheckerSettings settings) {
			return CreateExtension<SpellCheckerExtension, SpellCheckerSettings>(settings);
		}
		public SpellCheckerExtension SpellChecker(Action<SpellCheckerSettings> method) {
			return CreateExtension<SpellCheckerExtension, SpellCheckerSettings>(method);
		}
		public TreeListExtension TreeList(TreeListSettings settings) {
			return CreateExtension<TreeListExtension, TreeListSettings>(settings);
		}
		public TreeListExtension TreeList(Action<TreeListSettings> method) {
			return CreateExtension<TreeListExtension, TreeListSettings>(method);
		}
		public TreeListExtension<RowType> TreeList<RowType>(TreeListSettings<RowType> settings) where RowType: class {
			return CreateExtension<TreeListExtension<RowType>, TreeListSettings<RowType>>(settings);
		}
		public TreeListExtension<RowType> TreeList<RowType>(Action<TreeListSettings<RowType>> method) where RowType: class {
			return CreateExtension<TreeListExtension<RowType>, TreeListSettings<RowType>>(method);
		}
		public SpreadsheetExtension Spreadsheet(SpreadsheetSettings settings) {
			return CreateExtension<SpreadsheetExtension, SpreadsheetSettings>(settings);
		}
		public SpreadsheetExtension Spreadsheet(Action<SpreadsheetSettings> method) {
			return CreateExtension<SpreadsheetExtension, SpreadsheetSettings>(method);
		}
		public RichEditExtension RichEdit(RichEditSettings settings) {
			return CreateExtension<RichEditExtension, RichEditSettings>(settings);
		}
		public RichEditExtension RichEdit(Action<RichEditSettings> method) {
			return CreateExtension<RichEditExtension, RichEditSettings>(method);
		}
		public CallbackPanelExtension CallbackPanel(CallbackPanelSettings settings) {
			return CreateExtension<CallbackPanelExtension, CallbackPanelSettings>(settings);
		}
		public CallbackPanelExtension CallbackPanel(Action<CallbackPanelSettings> method) {
			return CreateExtension<CallbackPanelExtension, CallbackPanelSettings>(method);
		}
		public PanelExtension Panel(PanelSettings settings) {
			return CreateExtension<PanelExtension, PanelSettings>(settings);
		}
		public PanelExtension Panel(Action<PanelSettings> method) {
			return CreateExtension<PanelExtension, PanelSettings>(method);
		}
		public DataViewExtension DataView(DataViewSettings settings) {
			return CreateExtension<DataViewExtension, DataViewSettings>(settings);
		}
		public DataViewExtension DataView(Action<DataViewSettings> method) {
			return CreateExtension<DataViewExtension, DataViewSettings>(method);
		}
		public DockManagerExtension DockManager(DockManagerSettings settings) {
			return CreateExtension<DockManagerExtension, DockManagerSettings>(settings);
		}
		public DockManagerExtension DockManager(Action<DockManagerSettings> method) {
			return CreateExtension<DockManagerExtension, DockManagerSettings>(method);
		}
		public DockPanelExtension DockPanel(DockPanelSettings settings) {
			return CreateExtension<DockPanelExtension, DockPanelSettings>(settings);
		}
		public DockPanelExtension DockPanel(Action<DockPanelSettings> method) {
			return CreateExtension<DockPanelExtension, DockPanelSettings>(method);
		}
		public DockZoneExtension DockZone(DockZoneSettings settings) {
			return CreateExtension<DockZoneExtension, DockZoneSettings>(settings);
		}
		public DockZoneExtension DockZone(Action<DockZoneSettings> method) {
			return CreateExtension<DockZoneExtension, DockZoneSettings>(method);
		}
		public FileManagerExtension FileManager(DevExpress.Web.Mvc.FileManagerSettings settings) {
			return CreateExtension<FileManagerExtension, DevExpress.Web.Mvc.FileManagerSettings>(settings);
		}
		public FileManagerExtension FileManager(Action<DevExpress.Web.Mvc.FileManagerSettings> method) {
			return CreateExtension<FileManagerExtension, DevExpress.Web.Mvc.FileManagerSettings>(method);
		}
		public ImageGalleryExtension ImageGallery(ImageGallerySettings settings) {
			return CreateExtension<ImageGalleryExtension, ImageGallerySettings>(settings);
		}
		public ImageGalleryExtension ImageGallery(Action<ImageGallerySettings> method) {
			return CreateExtension<ImageGalleryExtension, ImageGallerySettings>(method);
		}
		public ImageSliderExtension ImageSlider(ImageSliderSettings settings) {
			return CreateExtension<ImageSliderExtension, ImageSliderSettings>(settings);
		}
		public ImageSliderExtension ImageSlider(Action<ImageSliderSettings> method) {
			return CreateExtension<ImageSliderExtension, ImageSliderSettings>(method);
		}
		public ImageZoomExtension ImageZoom(ImageZoomSettings settings) {
			return CreateExtension<ImageZoomExtension, ImageZoomSettings>(settings);
		}
		public ImageZoomExtension ImageZoom(Action<ImageZoomSettings> method) {
			return CreateExtension<ImageZoomExtension, ImageZoomSettings>(method);
		}
		public ImageZoomNavigatorExtension ImageZoomNavigator(ImageZoomNavigatorSettings settings) {
			return CreateExtension<ImageZoomNavigatorExtension, ImageZoomNavigatorSettings>(settings);
		}
		public ImageZoomNavigatorExtension ImageZoomNavigator(Action<ImageZoomNavigatorSettings> method) {
			return CreateExtension<ImageZoomNavigatorExtension, ImageZoomNavigatorSettings>(method);
		}
		public LoadingPanelExtension LoadingPanel(LoadingPanelSettings settings) {
			return CreateExtension<LoadingPanelExtension, LoadingPanelSettings>(settings);
		}
		public LoadingPanelExtension LoadingPanel(Action<LoadingPanelSettings> method) {
			return CreateExtension<LoadingPanelExtension, LoadingPanelSettings>(method);
		}
		public MenuExtension Menu(MenuSettings settings) {
			return CreateExtension<MenuExtension, MenuSettings>(settings);
		}
		public MenuExtension Menu(Action<MenuSettings> method) {
			return CreateExtension<MenuExtension, MenuSettings>(method);
		}
		public PopupMenuExtension PopupMenu(PopupMenuSettings settings) {
			return CreateExtension<PopupMenuExtension, PopupMenuSettings>(settings);
		}
		public PopupMenuExtension PopupMenu(Action<PopupMenuSettings> method) {
			return CreateExtension<PopupMenuExtension, PopupMenuSettings>(method);
		}
		public NavBarExtension NavBar(NavBarSettings settings) {
			return CreateExtension<NavBarExtension, NavBarSettings>(settings);
		}
		public NavBarExtension NavBar(Action<NavBarSettings> method) {
			return CreateExtension<NavBarExtension, NavBarSettings>(method);
		}
		public RibbonExtension Ribbon(RibbonSettings settings) {
			return CreateExtension<RibbonExtension, RibbonSettings>(settings);
		}
		public RibbonExtension Ribbon(Action<RibbonSettings> method) {
			return CreateExtension<RibbonExtension, RibbonSettings>(method);
		}
		public PopupControlExtension PopupControl(PopupControlSettings settings) {
			return CreateExtension<PopupControlExtension, PopupControlSettings>(settings);
		}
		public PopupControlExtension PopupControl(Action<PopupControlSettings> method) {
			return CreateExtension<PopupControlExtension, PopupControlSettings>(method);
		}
		public RatingControlExtension RatingControl(RatingControlSettings settings) {
			return CreateExtension<RatingControlExtension, RatingControlSettings>(settings);
		}
		public RatingControlExtension RatingControl(Action<RatingControlSettings> method) {
			return CreateExtension<RatingControlExtension, RatingControlSettings>(method);
		}
		public RoundPanelExtension RoundPanel(RoundPanelSettings settings) {
			return CreateExtension<RoundPanelExtension, RoundPanelSettings>(settings);
		}
		public RoundPanelExtension RoundPanel(Action<RoundPanelSettings> method) {
			return CreateExtension<RoundPanelExtension, RoundPanelSettings>(method);
		}
		public SplitterExtension Splitter(SplitterSettings settings) {
			return CreateExtension<SplitterExtension, SplitterSettings>(settings);
		}
		public SplitterExtension Splitter(Action<SplitterSettings> method) {
			return CreateExtension<SplitterExtension, SplitterSettings>(method);
		}
		public TabControlExtension TabControl(TabControlSettings settings) {
			return CreateExtension<TabControlExtension, TabControlSettings>(settings);
		}
		public TabControlExtension TabControl(Action<TabControlSettings> method) {
			return CreateExtension<TabControlExtension, TabControlSettings>(method);
		}
		public PageControlExtension PageControl(PageControlSettings settings) {
			return CreateExtension<PageControlExtension, PageControlSettings>(settings);
		}
		public PageControlExtension PageControl(Action<PageControlSettings> method) {
			return CreateExtension<PageControlExtension, PageControlSettings>(method);
		}
		public TreeViewExtension TreeView(TreeViewSettings settings) {
			return CreateExtension<TreeViewExtension, TreeViewSettings>(settings);
		}
		public TreeViewExtension TreeView(Action<TreeViewSettings> method) {
			return CreateExtension<TreeViewExtension, TreeViewSettings>(method);
		}
		public UploadControlExtension UploadControl(UploadControlSettings settings) {
			return CreateExtension<UploadControlExtension, UploadControlSettings>(settings);
		}
		public UploadControlExtension UploadControl(Action<UploadControlSettings> method) {
			return CreateExtension<UploadControlExtension, UploadControlSettings>(method);
		}
		public ButtonExtension Button(ButtonSettings settings) {
			return CreateExtension<ButtonExtension, ButtonSettings>(settings);
		}
		public ButtonExtension Button(Action<ButtonSettings> method) {
			return CreateExtension<ButtonExtension, ButtonSettings>(method);
		}
		public BinaryImageEditExtension BinaryImage(BinaryImageEditSettings settings) {
			return CreateExtension<BinaryImageEditExtension, BinaryImageEditSettings>(settings);
		}
		public BinaryImageEditExtension BinaryImage(Action<BinaryImageEditSettings> method) {
			return CreateExtension<BinaryImageEditExtension, BinaryImageEditSettings>(method);
		}
		public ButtonEditExtension ButtonEdit(ButtonEditSettings settings) {
			return CreateExtension<ButtonEditExtension, ButtonEditSettings>(settings);
		}
		public ButtonEditExtension ButtonEdit(Action<ButtonEditSettings> method) {
			return CreateExtension<ButtonEditExtension, ButtonEditSettings>(method);
		}
		public CalendarExtension Calendar(CalendarSettings settings) {
			return CreateExtension<CalendarExtension, CalendarSettings>(settings);
		}
		public CalendarExtension Calendar(Action<CalendarSettings> method) {
			return CreateExtension<CalendarExtension, CalendarSettings>(method);
		}
		public CaptchaExtension Captcha(CaptchaSettings settings) {
			return CreateExtension<CaptchaExtension, CaptchaSettings>(settings);
		}
		public CaptchaExtension Captcha(Action<CaptchaSettings> method) {
			return CreateExtension<CaptchaExtension, CaptchaSettings>(method);
		}
		public CheckBoxExtension CheckBox(CheckBoxSettings settings) {
			return CreateExtension<CheckBoxExtension, CheckBoxSettings>(settings);
		}
		public CheckBoxExtension CheckBox(Action<CheckBoxSettings> method) {
			return CreateExtension<CheckBoxExtension, CheckBoxSettings>(method);
		}
		public CheckBoxListExtension CheckBoxList(CheckBoxListSettings settings) {
			return CreateExtension<CheckBoxListExtension, CheckBoxListSettings>(settings);
		}
		public CheckBoxListExtension CheckBoxList(Action<CheckBoxListSettings> method) {
			return CreateExtension<CheckBoxListExtension, CheckBoxListSettings>(method);
		}
		public ColorEditExtension ColorEdit(ColorEditSettings settings) {
			return CreateExtension<ColorEditExtension, ColorEditSettings>(settings);
		}
		public ColorEditExtension ColorEdit(Action<ColorEditSettings> method) {
			return CreateExtension<ColorEditExtension, ColorEditSettings>(method);
		}
		public ComboBoxExtension ComboBox(ComboBoxSettings settings) {
			return CreateExtension<ComboBoxExtension, ComboBoxSettings>(settings);
		}
		public ComboBoxExtension ComboBox(Action<ComboBoxSettings> method) {
			return CreateExtension<ComboBoxExtension, ComboBoxSettings>(method);
		}
		public DateEditExtension DateEdit(DateEditSettings settings) {
			return CreateExtension<DateEditExtension, DateEditSettings>(settings);
		}
		public DateEditExtension DateEdit(Action<DateEditSettings> method) {
			return CreateExtension<DateEditExtension, DateEditSettings>(method);
		}
		public DropDownEditExtension DropDownEdit(DropDownEditSettings settings) {
			return CreateExtension<DropDownEditExtension, DropDownEditSettings>(settings);
		}
		public DropDownEditExtension DropDownEdit(Action<DropDownEditSettings> method) {
			return CreateExtension<DropDownEditExtension, DropDownEditSettings>(method);
		}
		public TrackBarExtension TrackBar(TrackBarSettings settings) {
			return CreateExtension<TrackBarExtension, TrackBarSettings>(settings);
		}
		public TrackBarExtension TrackBar(Action<TrackBarSettings> method) {
			return CreateExtension<TrackBarExtension, TrackBarSettings>(method);
		}
		public HyperLinkExtension HyperLink(HyperLinkSettings settings) {
			return CreateExtension<HyperLinkExtension, HyperLinkSettings>(settings);
		}
		public HyperLinkExtension HyperLink(Action<HyperLinkSettings> method) {
			return CreateExtension<HyperLinkExtension, HyperLinkSettings>(method);
		}
		public ImageEditExtension Image(ImageEditSettings settings) {
			return CreateExtension<ImageEditExtension, ImageEditSettings>(settings);
		}
		public ImageEditExtension Image(Action<ImageEditSettings> method) {
			return CreateExtension<ImageEditExtension, ImageEditSettings>(method);
		}
		public LabelExtension Label(LabelSettings settings) {
			return CreateExtension<LabelExtension, LabelSettings>(settings);
		}
		public LabelExtension Label(Action<LabelSettings> method) {
			return CreateExtension<LabelExtension, LabelSettings>(method);
		}
		public ListBoxExtension ListBox(ListBoxSettings settings) {
			return CreateExtension<ListBoxExtension, ListBoxSettings>(settings);
		}
		public ListBoxExtension ListBox(Action<ListBoxSettings> method) {
			return CreateExtension<ListBoxExtension, ListBoxSettings>(method);
		}
		public MemoExtension Memo(MemoSettings settings) {
			return CreateExtension<MemoExtension, MemoSettings>(settings);
		}
		public MemoExtension Memo(Action<MemoSettings> method) {
			return CreateExtension<MemoExtension, MemoSettings>(method);
		}
		public ProgressBarExtension ProgressBar(DevExpress.Web.Mvc.ProgressBarSettings settings) {
			return CreateExtension<ProgressBarExtension, DevExpress.Web.Mvc.ProgressBarSettings>(settings);
		}
		public ProgressBarExtension ProgressBar(Action<DevExpress.Web.Mvc.ProgressBarSettings> method) {
			return CreateExtension<ProgressBarExtension, DevExpress.Web.Mvc.ProgressBarSettings>(method);
		}
		public RadioButtonExtension RadioButton(RadioButtonSettings settings) {
			return CreateExtension<RadioButtonExtension, RadioButtonSettings>(settings);
		}
		public RadioButtonExtension RadioButton(Action<RadioButtonSettings> method) {
			return CreateExtension<RadioButtonExtension, RadioButtonSettings>(method);
		}
		public RadioButtonListExtension RadioButtonList(RadioButtonListSettings settings) {
			return CreateExtension<RadioButtonListExtension, RadioButtonListSettings>(settings);
		}
		public RadioButtonListExtension RadioButtonList(Action<RadioButtonListSettings> method) {
			return CreateExtension<RadioButtonListExtension, RadioButtonListSettings>(method);
		}
		public SpinEditExtension SpinEdit(SpinEditSettings settings) {
			return CreateExtension<SpinEditExtension, SpinEditSettings>(settings);
		}
		public SpinEditExtension SpinEdit(Action<SpinEditSettings> method) {
			return CreateExtension<SpinEditExtension, SpinEditSettings>(method);
		}
		public TextBoxExtension TextBox(TextBoxSettings settings) {
			return CreateExtension<TextBoxExtension, TextBoxSettings>(settings);
		}
		public TextBoxExtension TextBox(Action<TextBoxSettings> method) {
			return CreateExtension<TextBoxExtension, TextBoxSettings>(method);
		}
		public TimeEditExtension TimeEdit(TimeEditSettings settings) {
			return CreateExtension<TimeEditExtension, TimeEditSettings>(settings);
		}
		public TimeEditExtension TimeEdit(Action<TimeEditSettings> method) {
			return CreateExtension<TimeEditExtension, TimeEditSettings>(method);
		}
		public TokenBoxExtension TokenBox(TokenBoxSettings settings) {
			return CreateExtension<TokenBoxExtension, TokenBoxSettings>(settings);
		}
		public TokenBoxExtension TokenBox(Action<TokenBoxSettings> method) {
			return CreateExtension<TokenBoxExtension, TokenBoxSettings>(method);
		}
		public ValidationSummaryExtension ValidationSummary() {
			return CreateExtension<ValidationSummaryExtension, ValidationSummarySettings>(new ValidationSummarySettings());
		}
		public ValidationSummaryExtension ValidationSummary(ValidationSummarySettings settings) {
			return CreateExtension<ValidationSummaryExtension, ValidationSummarySettings>(settings);
		}
		public ValidationSummaryExtension ValidationSummary(Action<ValidationSummarySettings> method) {
			return CreateExtension<ValidationSummaryExtension, ValidationSummarySettings>(method);
		}
		static Dictionary<ExtensionType, Dictionary<MenuIconSetType, string>> IconSetFactory = new Dictionary<ExtensionType, Dictionary<MenuIconSetType, string>> {
			{ 
				ExtensionType.HtmlEditor, new Dictionary<MenuIconSetType, string> {
					{ MenuIconSetType.Colored, HERibbonImages.RibbonHESpriteName },
					{ MenuIconSetType.ColoredLight, HERibbonImages.RibbonHESpriteName },
					{ MenuIconSetType.GrayScaled, HERibbonImages.RibbonHEGSpriteName },
					{ MenuIconSetType.GrayScaledWithWhiteHottrack, HERibbonImages.RibbonHEGWSpriteName }
				}
			},
			{ 
				ExtensionType.Spreadsheet, new Dictionary<MenuIconSetType, string> {
					{ MenuIconSetType.Colored, SpreadsheetRibbonImages.RibbonSSSpriteName },
					{ MenuIconSetType.ColoredLight, SpreadsheetRibbonImages.RibbonSSSpriteName },
					{ MenuIconSetType.GrayScaled, SpreadsheetRibbonImages.RibbonSSGSpriteName },
					{ MenuIconSetType.GrayScaledWithWhiteHottrack, SpreadsheetRibbonImages.RibbonSSGWSpriteName }
				}
			},
			{ 
				ExtensionType.RichEdit, new Dictionary<MenuIconSetType, string> {
					{ MenuIconSetType.Colored, RichEditRibbonImages.RibbonRESpriteName },
					{ MenuIconSetType.ColoredLight, RichEditRibbonImages.RibbonRESpriteName },
					{ MenuIconSetType.GrayScaled, RichEditRibbonImages.RibbonREGSpriteName },
					{ MenuIconSetType.GrayScaledWithWhiteHottrack, RichEditRibbonImages.RibbonREGWSpriteName }
				}
			},
			{ 
				ExtensionType.DocumentViewer, new Dictionary<MenuIconSetType, string> {
					{ MenuIconSetType.Colored, DocumentViewerRibbonImages.RibbonDVSpriteName },
					{ MenuIconSetType.ColoredLight, DocumentViewerRibbonImages.RibbonDVSpriteName },
					{ MenuIconSetType.GrayScaled, DocumentViewerRibbonImages.RibbonDVGSpriteName },
					{ MenuIconSetType.GrayScaledWithWhiteHottrack, DocumentViewerRibbonImages.RibbonDVGWSpriteName }
				}
			}
		};
		public void RenderStyleSheets(Page page, string clientUIControlColorScheme, params StyleSheet[] cssItems) {
			ASPxWebClientUIControl.GlobalColorScheme = clientUIControlColorScheme;
			RenderStyleSheets(page, cssItems);
		}
		public void RenderStyleSheets(Page page, params StyleSheet[] cssItems) {
			List<StyleSheet> cssItemsList = new List<StyleSheet>();
			foreach(StyleSheet cssItem in cssItems) {
				if(cssItem.ExtensionType.HasValue)
					cssItemsList.Add(cssItem);
				else if(cssItem.ExtensionSuite.HasValue) {
					List<ExtensionType> extensionTypes = ExtensionManager.GetExtensionTypes(cssItem.ExtensionSuite.Value);
					foreach(ExtensionType extensionType in extensionTypes)
						cssItemsList.Add(new StyleSheet {
							ExtensionType = extensionType,
							Theme = cssItem.Theme,
							SkinIDs = cssItem.SkinIDs,
							MenuIconSet = cssItem.MenuIconSet
						});
				}
				else
					throw new ArgumentException("You should specify either ExtensionType or ExtensionSuite for the StyleSheet object.");
			}
			MvcRenderMode savedRenderMode = MvcUtils.RenderMode;
			MvcUtils.RenderMode = MvcRenderMode.RenderResources;
			try {
				ResourceManager.ClearCssResources(); 
				foreach(StyleSheet cssItem in cssItemsList) {
					string[] skinIDs = (cssItem.SkinIDs != null && cssItem.SkinIDs.Length > 0) ? cssItem.SkinIDs : new string[] { "" };
					foreach(string skinID in skinIDs)
						ExtensionManager.RegisterStyleSheets(cssItem.ExtensionType.Value, cssItem.Theme, skinID);
					if(cssItem.MenuIconSet != MenuIconSetType.NotSet && IconSetFactory.ContainsKey(cssItem.ExtensionType.Value)){
						string iconSetResourcePath = string.Format("DevExpress.Web.Css.{0}.css", IconSetFactory[cssItem.ExtensionType.Value][cssItem.MenuIconSet]);
						ResourceManager.RegisterCssResource(null, typeof(ASPxWebControl), iconSetResourcePath);
					}
				}
				ResourceManager.RenderCssResources(page, Utils.CreateHtmlTextWriter(), false);
			}
			finally {
				MvcUtils.RenderMode = savedRenderMode;
			}
		}
		public MvcHtmlString GetStyleSheets(string clientUIControlColorScheme, params StyleSheet[] cssItems) {
			DevExpress.Web.ASPxWebClientUIControl.GlobalColorScheme = clientUIControlColorScheme;
			return GetStyleSheets(cssItems);
		}
		public MvcHtmlString GetStyleSheets(params StyleSheet[] cssItems) {
			return Utils.GetInnerWriterOutput(() => RenderStyleSheets(null, cssItems));
		}
		public void RenderScripts(Page page, params Script[] scriptItems) {
			List<Script> scriptItemsList = new List<Script>();
			foreach(Script scriptItem in scriptItems) {
				if(scriptItem.ExtensionType.HasValue)
					scriptItemsList.Add(scriptItem);
				else if(scriptItem.ExtensionSuite.HasValue) {
					List<ExtensionType> extensionTypes = ExtensionManager.GetExtensionTypes(scriptItem.ExtensionSuite.Value);
					foreach(ExtensionType extensionType in extensionTypes)
						scriptItemsList.Add(new Script {
							ExtensionType = extensionType
						});
				}
				else
					throw new ArgumentException("You should specify either ExtensionType or ExtensionSuite for the Script object.");
			}
			MvcRenderMode savedRenderMode = MvcUtils.RenderMode;
			MvcUtils.RenderMode = MvcRenderMode.RenderResources;
			try {
				if(MvcUtils.RenderScriptsCalled)
					ResourceManager.ClearScriptResources(); 
				foreach(Script scriptItem in scriptItemsList)
					ExtensionManager.RegisterScripts(scriptItem.ExtensionType.Value);
				ResourceManager.RenderScriptResources(page, Utils.CreateHtmlTextWriter());
				ResourceManager.RenderScriptBlocks(page, Utils.CreateHtmlTextWriter());
			}
			finally {
				MvcUtils.RenderMode = savedRenderMode;
			}
			MvcUtils.RenderScriptsCalled = true;
		}
		public MvcHtmlString GetScripts(params Script[] scriptItems) {
			return Utils.GetInnerWriterOutput(() => { RenderScripts(null, scriptItems); });
		}
		internal static T CreateExtension<T, T1>(T1 settings)
			where T : ExtensionBase
			where T1 : SettingsBase {
			return (T)CreateExtension(typeof(T), settings, HtmlHelperExtension.ViewContext);
		}
		internal static T CreateExtension<T, T1>(Action<T1> method)
			where T : ExtensionBase
			where T1 : SettingsBase, new() {
			ViewContext viewContext = HtmlHelperExtension.ViewContext; 
			T1 settings = new T1();
			method(settings);
			return (T)CreateExtension(typeof(T), settings, viewContext);
		}
		internal static ExtensionBase CreateExtension(Type type, SettingsBase settings, ViewContext viewContext) {
			return (ExtensionBase)Activator.CreateInstance(type, settings, viewContext);
		}
		internal static ExtensionBase CreateExtension(Type type, SettingsBase settings, ViewContext viewContext, ModelMetadata metadata) {
			return (ExtensionBase)Activator.CreateInstance(type, settings, viewContext, metadata);
		}
		internal static SettingsBase CreateSettings(Type type, string name) {
			SettingsBase settings = (SettingsBase)Activator.CreateInstance(type);
			settings.Name = name;
			return settings;
		}
	}
	[System.Diagnostics.DebuggerDisplay("ExtensionType={ExtensionType}")]
	public abstract class ResourceItem {
		public ExtensionType? ExtensionType { get; set; }
		public ExtensionSuite? ExtensionSuite { get; set; }
		public ResourceItem() {
			ExtensionType = null;
			ExtensionSuite = null;
		}
	}
	public class StyleSheet : ResourceItem {
		public StyleSheet() {
			Theme = string.Empty;
			SkinIDs = new string[0];
			MenuIconSet = MenuIconSetType.NotSet;
		}
		public string Theme { get; set; }
		public string[] SkinIDs { get; set; }
		public MenuIconSetType MenuIconSet { get; set; }
	}
	public class Script : ResourceItem { }
}
