#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.DataAccess.Native;
using DevExpress.Utils.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardWin.Forms.Export.Groups {
	public abstract class OptionsGroup : IDisposable, IOptionsEditor {
		Locker locker = new Locker(); 
		bool enabled = true;
		protected bool Enabled { get { return enabled; } }
		public void Init(ExtendedReportOptions opts) {
			Set(opts);
			SubscribeEvents();
			UpdateState(this);
		}
		public void Set(ExtendedReportOptions opts) {
			foreach(KeyValuePair<string, OptionsGroup> pair in GetOptionsGroups()) {
				pair.Value.Set(opts);
			}
			foreach(KeyValuePair<string, LabeledEditor> pair in GetLabeledEditors()) {
				pair.Value.Set(opts);
			}
		}
		public void Apply(ExtendedReportOptions opts) {
			foreach(KeyValuePair<string, OptionsGroup> pair in GetOptionsGroups()) {
				pair.Value.Apply(opts);
			}
			foreach(KeyValuePair<string, LabeledEditor> pair in GetLabeledEditors()) {
				pair.Value.Apply(opts);
			}
		}
		public IEnumerable<LabelAndEditor> GetControls() {
			List<LabelAndEditor> labelAndEditors = new List<LabelAndEditor>();
			Dictionary<string, LabeledEditor> labeledEditors = GetLabeledEditors();
			Dictionary<string, OptionsGroup> optionsGroups = GetOptionsGroups();
			foreach(string name in GetOptionsOrder()) {
				if(labeledEditors.ContainsKey(name))
					labelAndEditors.AddRange(labeledEditors[name].GetControls());
				else if(optionsGroups.ContainsKey(name))
					labelAndEditors.AddRange(optionsGroups[name].GetControls());
			}
			return labelAndEditors;
		}
		public void SetEnabled(bool enabled) {
			this.enabled = enabled;
		}
		public void SubscribeEvents() {
			foreach(KeyValuePair<string, OptionsGroup> pair in GetOptionsGroups()) {
				pair.Value.SubscribeEvents();
				pair.Value.ValueChanged += (s, e) => {
					if(!locker.IsLocked)
						UpdateStateCore(e.Initiator);
					var initiator = this;
					OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = initiator });
				};
			}
			foreach(KeyValuePair<string, LabeledEditor> pair in GetLabeledEditors()) {
				pair.Value.ValueChanged += (s, e) => {
					if(!locker.IsLocked)
						UpdateStateCore(e.Initiator);
					var initiator = this;
					OnValueChanged(new ExportDocumenInfoEventArgs { Initiator = initiator });
				};
			}
		}
		public event EventHandler<ExportDocumenInfoEventArgs> ValueChanged;
		void OnValueChanged(ExportDocumenInfoEventArgs e) {
			if(ValueChanged != null)
				ValueChanged(this, e);
		}
		protected abstract Dictionary<string, LabeledEditor> GetLabeledEditors();
		protected abstract Dictionary<string, OptionsGroup> GetOptionsGroups();
		protected abstract List<string> GetOptionsOrder();
		protected virtual void SetVisibility(ExtendedReportOptions opts) { }
		void DisableGroup() {
			foreach(KeyValuePair<string, OptionsGroup> pair in GetOptionsGroups()) {
				pair.Value.DisableGroup();
			}
			foreach(KeyValuePair<string, LabeledEditor> pair in GetLabeledEditors()) {
				pair.Value.SetEnabled(false);
			}
		}
		void UpdateStateCore(IOptionsEditor initiator) {
			ExtendedReportOptions opts = ExtendedReportOptions.Empty;
			Apply(opts);
			initiator.Apply(opts);
			if(Enabled)
				SetVisibility(opts);
			else
				DisableGroup();
			locker.Lock();
			Set(opts);
			locker.Unlock();
		}
		void UpdateState(OptionsGroup initiator) {
			foreach(KeyValuePair<string, OptionsGroup> pair in GetOptionsGroups()) {
				pair.Value.UpdateState(initiator);
			}
			UpdateStateCore(initiator);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				foreach(KeyValuePair<string, OptionsGroup> pair in GetOptionsGroups()) {
					pair.Value.Dispose(disposing);
				}
				foreach(KeyValuePair<string, LabeledEditor> pair in GetLabeledEditors()) {
					pair.Value.Dispose();
				}
			}
		}
	}
	public class TitleOptionsGroup : OptionsGroup {
		readonly ShowTitleEditor showTitleEditor;
		readonly TitleEditor titleEditor;
		public TitleOptionsGroup() {
			showTitleEditor = new ShowTitleEditor();
			titleEditor = new TitleEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("showTitleEditor", showTitleEditor);
			labeledEditors.Add("titleEditor", titleEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "showTitleEditor", "titleEditor" };
		}
		protected override void SetVisibility(ExtendedReportOptions opts) {
			titleEditor.SetEnabled(opts.DocumentContentOptions.ShowTitle);
		}
	}
	public class DocumentContentOptionsGroup : OptionsGroup {
		readonly TitleOptionsGroup titleOptionsGroup;
		readonly PDFFilterStatePresentationEditor filterStatePresentation;
		public DocumentContentOptionsGroup() {
			titleOptionsGroup = new TitleOptionsGroup();
			filterStatePresentation = new PDFFilterStatePresentationEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("titleOptionsGroup", titleOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("filterStatePresentation", filterStatePresentation);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "titleOptionsGroup", "filterStatePresentation" };
		}
	}
	public class PaperOptionsGroup : OptionsGroup {
		readonly SimplyPageLayoutEditor pageLayoutEditor;
		readonly PaperKindEditor paperKindEditor;
		readonly DocumentContentOptionsGroup captionOptionsGroup;
		public PaperOptionsGroup() {
			pageLayoutEditor = new SimplyPageLayoutEditor();
			paperKindEditor = new PaperKindEditor();
			captionOptionsGroup = new DocumentContentOptionsGroup();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("pageLayoutEditor", pageLayoutEditor);
			labeledEditors.Add("paperKindEditor", paperKindEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "pageLayoutEditor", "paperKindEditor", "captionOptionsGroup" };
		}
	}
	public class ScalingOptionsGroup : OptionsGroup {
		readonly ScaleModeEditor scaleModeEditor;
		readonly ScaleFactorEditor scaleFactorEditor;
		readonly AutoFitPageCountEditor autoFitPageCountEditor;
		public ScalingOptionsGroup() {
			scaleModeEditor = new ScaleModeEditor();
			scaleFactorEditor = new ScaleFactorEditor();
			autoFitPageCountEditor = new AutoFitPageCountEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("scaleModeEditor", scaleModeEditor);
			labeledEditors.Add("scaleFactorEditor", scaleFactorEditor);
			labeledEditors.Add("autoFitPageCountEditor", autoFitPageCountEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "scaleModeEditor", "scaleFactorEditor", "autoFitPageCountEditor" };
		}
		protected override void SetVisibility(ExtendedReportOptions opts) {
			scaleModeEditor.SetEnabled(true);
			switch(opts.ScalingOptions.ScaleMode) {
				case ExtendedScaleMode.None:
					scaleFactorEditor.SetEnabled(false);
					autoFitPageCountEditor.SetEnabled(false);
					break;
				case ExtendedScaleMode.UseScaleFactor:
					scaleFactorEditor.SetEnabled(true);
					autoFitPageCountEditor.SetEnabled(false);
					break;
				case ExtendedScaleMode.AutoFitToPageWidth:
					scaleFactorEditor.SetEnabled(false);
					autoFitPageCountEditor.SetEnabled(true);
					break;
			}
			if(opts.AutoPageOptions.AutoFitToPageSize && opts.AutoPageOptions.AutoRotate) {
				scaleFactorEditor.SetEnabled(false);
				autoFitPageCountEditor.SetEnabled(false);
			}
		}
	}
	public class SimplyDocumentOptionsWithScaleModeGroup : OptionsGroup {
		readonly SimplyPageLayoutEditor pageLayoutEditor;
		readonly PaperKindEditor paperKindEditor;
		readonly TitleOptionsGroup captionOptionsGroup;
		readonly ScalingOptionsGroup scaleModeOptionsGroup;
		public SimplyDocumentOptionsWithScaleModeGroup() {
			pageLayoutEditor = new SimplyPageLayoutEditor();
			paperKindEditor = new PaperKindEditor();
			captionOptionsGroup = new TitleOptionsGroup();
			scaleModeOptionsGroup = new ScalingOptionsGroup();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionsGroup);
			groups.Add("scaleModeOptionsGroup", scaleModeOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("pageLayoutEditor", pageLayoutEditor);
			labeledEditors.Add("paperKindEditor", paperKindEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "pageLayoutEditor", "paperKindEditor", "captionOptionsGroup", "scaleModeOptionsGroup" };
		}
	}
	public class DashboardOptionsGroup : OptionsGroup {
		readonly DashboardAutoPageLayoutEditor autoPageLayoutEditor;
		readonly PaperKindEditor paperKindEditor;
		readonly DocumentContentOptionsGroup titleOptionsGroup;
		readonly ScalingOptionsGroup scaleModeOptionsGroup;
		public DashboardOptionsGroup() {
			autoPageLayoutEditor = new DashboardAutoPageLayoutEditor();
			paperKindEditor = new PaperKindEditor();
			titleOptionsGroup = new DocumentContentOptionsGroup();
			scaleModeOptionsGroup = new ScalingOptionsGroup();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("titleOptionsGroup", titleOptionsGroup);
			groups.Add("scaleModeOptionsGroup", scaleModeOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("autoPageLayoutEditor", autoPageLayoutEditor);
			labeledEditors.Add("paperKindEditor", paperKindEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "autoPageLayoutEditor", "paperKindEditor", "titleOptionsGroup", "scaleModeOptionsGroup" };
		}
		protected override void SetVisibility(ExtendedReportOptions opts) {
			scaleModeOptionsGroup.SetEnabled(!(opts.AutoPageOptions.AutoFitToPageSize && opts.AutoPageOptions.AutoRotate));
		}
	}
	public class GridOptionsGroup : OptionsGroup {
		readonly PaperOptionsGroup documentOptionsGroup;
		readonly PrintHeadersOnEveryPageEditor printHeadersOnEveryPage;
		readonly FitToPageWidthEditor fitToPageWidhEditor;
		readonly ScalingOptionsGroup scaleModeOptionsGroup;
		public GridOptionsGroup() {
			documentOptionsGroup = new PaperOptionsGroup();
			printHeadersOnEveryPage = new PrintHeadersOnEveryPageEditor();
			fitToPageWidhEditor = new FitToPageWidthEditor();
			scaleModeOptionsGroup = new ScalingOptionsGroup();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("documentOptionsGroup", documentOptionsGroup);
			groups.Add("scaleModeOptionsGroup", scaleModeOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("printHeadersOnEveryPage", printHeadersOnEveryPage);
			labeledEditors.Add("fitToPageWidth", fitToPageWidhEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "documentOptionsGroup", "printHeadersOnEveryPage", "fitToPageWidth", "scaleModeOptionsGroup" };
		}
		protected override void SetVisibility(ExtendedReportOptions opts) {
			scaleModeOptionsGroup.SetEnabled(opts.ItemContentOptions.SizeMode != ItemSizeMode.FitWidth);
		}
	}
	public class PivotOptionsGroup : OptionsGroup {
		readonly PaperOptionsGroup documentOptionsGroup;
		readonly PrintHeadersOnEveryPageEditor printHeadersOnEveryPage;
		readonly ScalingOptionsGroup scaleModeOptionsGroup;
		public PivotOptionsGroup() {
			documentOptionsGroup = new PaperOptionsGroup();
			printHeadersOnEveryPage = new PrintHeadersOnEveryPageEditor();
			scaleModeOptionsGroup = new ScalingOptionsGroup();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("documentOptionsGroup", documentOptionsGroup);
			groups.Add("scaleModeOptionsGroup", scaleModeOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("printHeadersOnEveryPage", printHeadersOnEveryPage);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "documentOptionsGroup", "printHeadersOnEveryPage", "scaleModeOptionsGroup" };
		}
	}
	public class ChartOptionsGroup : OptionsGroup {
		readonly ItemAutoPageLayoutEditor autoPageLayoutEditor;
		readonly PaperKindEditor paperKindEditor;
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly SizeModeThreeItemsEditor chartSizeModeEditor;
		public ChartOptionsGroup() {
			autoPageLayoutEditor = new ItemAutoPageLayoutEditor();
			paperKindEditor = new PaperKindEditor();
			captionOptionGroup = new DocumentContentOptionsGroup();
			chartSizeModeEditor = new SizeModeThreeItemsEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("autoPageLayoutEditor", autoPageLayoutEditor);
			labeledEditors.Add("paperKindEditor", paperKindEditor);
			labeledEditors.Add("chartSizeModeEditor", chartSizeModeEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "autoPageLayoutEditor", "paperKindEditor", "captionOptionsGroup", "chartSizeModeEditor" };
		}
	}
	public class MapOptionsGroup : OptionsGroup {
		readonly ItemAutoPageLayoutEditor autoPageLayoutEditor;
		readonly PaperKindEditor paperKindEditor;
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly SizeModeTwoItemsEditor mapSizeModeEditor;
		public MapOptionsGroup() {
			autoPageLayoutEditor = new ItemAutoPageLayoutEditor();
			paperKindEditor = new PaperKindEditor();
			captionOptionGroup = new DocumentContentOptionsGroup();
			mapSizeModeEditor = new SizeModeTwoItemsEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("autoPageLayoutEditor", autoPageLayoutEditor);
			labeledEditors.Add("paperKindEditor", paperKindEditor);
			labeledEditors.Add("mapSizeModeEditor", mapSizeModeEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "autoPageLayoutEditor", "paperKindEditor", "captionOptionsGroup", "mapSizeModeEditor" };
		}
	}
	public class KPIOptionsGroup : OptionsGroup {
		readonly PaperOptionsGroup documentOptionsGroup;
		readonly AutoArrangeContentEditor autoArrangeContentEditor;
		readonly ScalingOptionsGroup scaleModeOptionsGroup;
		public KPIOptionsGroup() {
			documentOptionsGroup = new PaperOptionsGroup();
			autoArrangeContentEditor = new AutoArrangeContentEditor();
			scaleModeOptionsGroup = new ScalingOptionsGroup();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("documentOptionsGroup", documentOptionsGroup);
			groups.Add("scaleModeOptionsGroup", scaleModeOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("autoArrangeContentEditor", autoArrangeContentEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "documentOptionsGroup", "autoArrangeContentEditor", "scaleModeOptionsGroup" };
		}
		protected override void SetVisibility(ExtendedReportOptions opts) {
			scaleModeOptionsGroup.SetEnabled(!opts.ItemContentOptions.ArrangerOptions.AutoArrangeContent);
		}
	}
	public class ImageOptionsGroup : OptionsGroup {
		readonly TitleOptionsGroup titleOptionsGroup;
		readonly ImageFilterStatePresentationEditor imageFilterStatePresentation;
		readonly ImageFormatEditor imageFormatEditor;
		readonly ResolutionEditor resolutionEditor;
		public ImageOptionsGroup() {
			titleOptionsGroup = new TitleOptionsGroup();
			imageFilterStatePresentation = new ImageFilterStatePresentationEditor();
			imageFormatEditor = new ImageFormatEditor();
			resolutionEditor = new ResolutionEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("titleOptionsGroup", titleOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("imageFilterStatePresentation", imageFilterStatePresentation);
			labeledEditors.Add("imageFormatEditor", imageFormatEditor);
			labeledEditors.Add("resolutionEditor", resolutionEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "titleOptionsGroup", "imageFilterStatePresentation", "imageFormatEditor", "resolutionEditor" };
		}
	}
	public class DataOptionsGroup : OptionsGroup {
		readonly DataFormatEditor dataFormatEditor = new DataFormatEditor();
		readonly CsvValueSeparatorEditor csvValueSeparatorEditor = new CsvValueSeparatorEditor();
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			return new Dictionary<string, OptionsGroup>();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> editors = new Dictionary<string, LabeledEditor>();
			editors.Add("dataFormatEditor", dataFormatEditor);
			editors.Add("csvValueSeparatorEditor", csvValueSeparatorEditor);
			return editors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string>() { "dataFormatEditor", "csvValueSeparatorEditor" };
		}
		protected override void SetVisibility(ExtendedReportOptions opts) {
			csvValueSeparatorEditor.SetEnabled(opts.FormatOptions.ExcelOptions.Format == DashboardCommon.ExcelFormat.Csv);
		}
	}
	public class SimplyImageOptionsGroup : OptionsGroup {
		readonly TitleOptionsGroup titleOptionsGroup;
		readonly ImageFormatEditor imageFormatEditor;
		readonly ResolutionEditor resolutionEditor;
		public SimplyImageOptionsGroup() {
			titleOptionsGroup = new TitleOptionsGroup();
			imageFormatEditor = new ImageFormatEditor();
			resolutionEditor = new ResolutionEditor();
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("titleOptionsGroup", titleOptionsGroup);
			return groups;
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("imageFormatEditor", imageFormatEditor);
			labeledEditors.Add("resolutionEditor", resolutionEditor);
			return labeledEditors;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "titleOptionsGroup", "imageFormatEditor", "resolutionEditor" };
		}
	}
	public class DashboardPrintPreviewOptionsGroup : OptionsGroup {
		readonly DocumentContentOptionsGroup titleOptionsGroup;
		public DashboardPrintPreviewOptionsGroup() {
			titleOptionsGroup = new DocumentContentOptionsGroup();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			return labeledEditors;
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("titleOptionsGroup", titleOptionsGroup);
			return groups;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "titleOptionsGroup" };
		}
	}
	public class GridPintPreviewOptionsGroup : OptionsGroup {
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly PrintHeadersOnEveryPageEditor printHeadersOnEveryPage;
		readonly FitToPageWidthEditor fitToPageWidhEditor;
		public GridPintPreviewOptionsGroup() {
			captionOptionGroup = new DocumentContentOptionsGroup();
			printHeadersOnEveryPage = new PrintHeadersOnEveryPageEditor();
			fitToPageWidhEditor = new FitToPageWidthEditor();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("printHeadersOnEveryPage",printHeadersOnEveryPage);
			labeledEditors.Add("fitToPageWidth", fitToPageWidhEditor);
			return labeledEditors;
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "captionOptionsGroup", "printHeadersOnEveryPage", "fitToPageWidth" };
		}
	}
	public class PivotPintPreviewOptionsGroup : OptionsGroup {
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly PrintHeadersOnEveryPageEditor printHeadersOnEveryPage;
		public PivotPintPreviewOptionsGroup() {
			captionOptionGroup = new DocumentContentOptionsGroup();
			printHeadersOnEveryPage = new PrintHeadersOnEveryPageEditor();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("printHeadersOnEveryPage", printHeadersOnEveryPage);
			return labeledEditors;
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "captionOptionsGroup", "printHeadersOnEveryPage" };
		}
	}
	public class ChartPintPreviewOptionsGroup : OptionsGroup {
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly SizeModeThreeItemsEditor chartSizeModeEditor;
		public ChartPintPreviewOptionsGroup() {
			captionOptionGroup = new DocumentContentOptionsGroup();
			chartSizeModeEditor = new SizeModeThreeItemsEditor();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("chartSizeModeEditor", chartSizeModeEditor);
			return labeledEditors;
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "captionOptionsGroup", "chartSizeModeEditor" };
		}
	}
	public class MapPintPreviewOptionsGroup : OptionsGroup {
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly SizeModeTwoItemsEditor mapSizeModeEditor;
		public MapPintPreviewOptionsGroup() {
			captionOptionGroup = new DocumentContentOptionsGroup();
			mapSizeModeEditor = new SizeModeTwoItemsEditor();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("mapSizeModeEditor", mapSizeModeEditor);
			return labeledEditors;
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "captionOptionsGroup", "mapSizeModeEditor" };
		}
	}
	public class KPIPintPreviewOptionsGroup : OptionsGroup {
		readonly DocumentContentOptionsGroup captionOptionGroup;
		readonly AutoArrangeContentEditor autoArrangeContentEditor;
		public KPIPintPreviewOptionsGroup() {
			captionOptionGroup = new DocumentContentOptionsGroup();
			autoArrangeContentEditor = new AutoArrangeContentEditor();
		}
		protected override Dictionary<string, LabeledEditor> GetLabeledEditors() {
			Dictionary<string, LabeledEditor> labeledEditors = new Dictionary<string, LabeledEditor>();
			labeledEditors.Add("autoArrangeContentEditor", autoArrangeContentEditor);
			return labeledEditors;
		}
		protected override Dictionary<string, OptionsGroup> GetOptionsGroups() {
			Dictionary<string, OptionsGroup> groups = new Dictionary<string, OptionsGroup>();
			groups.Add("captionOptionsGroup", captionOptionGroup);
			return groups;
		}
		protected override List<string> GetOptionsOrder() {
			return new List<string> { "captionOptionsGroup", "autoArrangeContentEditor" };
		}
	}
}
