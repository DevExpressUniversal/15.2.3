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
using System.Drawing.Printing;
using System.Web.Mvc;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	public class MVCxGridViewSettings : ASPxGridViewSettings {
		public MVCxGridViewSettings()
			: base(null) {
		}
	}
	public class MVCxGridViewBehaviorSettings : ASPxGridViewBehaviorSettings {
		public MVCxGridViewBehaviorSettings()
			: base(null) {
		}
		protected internal MVCxGridViewBehaviorSettings(MVCxGridView gridView)
			: base(gridView) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ProcessFocusedRowChangedOnServer { get { return base.ProcessFocusedRowChangedOnServer; } set { base.ProcessFocusedRowChangedOnServer = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ProcessSelectionChangedOnServer { get { return base.ProcessSelectionChangedOnServer; } set { base.ProcessSelectionChangedOnServer = value; } }
		protected override bool IsRaiseOnFocusedRowChanged() {
			return Grid != null && MvcUtils.RenderMode != MvcRenderMode.None;
		}
	}
	public class MVCxGridViewTextSettings : ASPxGridViewTextSettings {
		public MVCxGridViewTextSettings()
			: base(null) {
		}
	}
	public class MVCxGridViewExportSettings {
		GridViewExportStyles styles;
		private static readonly object renderBrick = new object();
		GridViewExporterHeaderFooter pageHeader, pageFooter;
		public MVCxGridViewExportSettings() {
			this.styles = new GridViewExportStyles(null);
			this.pageHeader = new GridViewExporterHeaderFooter();
			this.pageFooter = new GridViewExporterHeaderFooter();
			MaxColumnWidth = ASPxGridViewExporter.DefaultMaxColumnWidth;
			ExportedRowType = GridViewExportedRowType.All;
			ExportSelectedRowsOnly = false;
			BottomMargin = -1;
			TopMargin = -1;
			LeftMargin = -1;
			RightMargin = -1;
			DetailVerticalOffset = 5;
			DetailHorizontalOffset = 5;
			PaperKind = PaperKind.Letter;
		}
		public ASPxGridViewExportRenderingEventHandler RenderBrick { get; set; }
		public GridViewGetExportDetailGridViewEventHandler GetExportDetailGridViews { get; set; }
		public EventHandler BeforeExport { get; set; }
		public string FileName { get; set; }
		public int MaxColumnWidth { get; set; }
		public bool PrintSelectCheckBox { get; set; }
		public bool PreserveGroupRowStates { get; set; }
		[Obsolete("Use the ExportSelectedRowsOnly property instead.")]
		public GridViewExportedRowType ExportedRowType { get; set; }
		public bool ExportSelectedRowsOnly { get; set; }
		public int BottomMargin { get; set; }
		public int TopMargin { get; set; }
		public int LeftMargin { get; set; }
		public int RightMargin { get; set; }
		public bool Landscape { get; set; }
		public GridViewExportStyles Styles { get { return styles; } }
		public GridViewExporterHeaderFooter PageHeader { get { return pageHeader; } }
		public GridViewExporterHeaderFooter PageFooter { get { return pageFooter; } }
		public string ReportHeader { get; set; }
		public string ReportFooter { get; set; }
		public int DetailVerticalOffset { get; set; }
		public int DetailHorizontalOffset { get; set; }
		public bool ExportEmptyDetailGrid { get; set; }
		public PaperKind PaperKind { get; set; }
		public string PaperName { get; set; }
	}
	public class MVCxGridViewDetailSettings : ASPxGridViewDetailSettings {
		public MVCxGridViewDetailSettings()
			: base(null) {
		}
		public string MasterGridName { get; set; }
		public override void Assign(DevExpress.Web.PropertiesBase source) {
			base.Assign(source);
			MVCxGridViewDetailSettings src = source as MVCxGridViewDetailSettings;
			if(src != null)
				MasterGridName = src.MasterGridName;
		}
	}
	public class MVCxGridViewEditingSettings : ASPxGridViewEditingSettings {
		public MVCxGridViewEditingSettings()
			: base(null) {
			ShowModelErrorsForEditors = true;
		}
		public object AddNewRowRouteValues { get; set; }
		public object UpdateRowRouteValues { get; set; }
		public object DeleteRowRouteValues { get; set; }
		public object BatchUpdateRouteValues { get; set; }
		public bool ShowModelErrorsForEditors { get; set; }
		protected internal new bool IsEditForm { get { return base.IsEditForm; } }
		protected internal new bool IsPopupEditForm { get { return base.IsPopupEditForm; } }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			MVCxGridViewEditingSettings src = source as MVCxGridViewEditingSettings;
			if(src != null) {
				AddNewRowRouteValues = src.AddNewRowRouteValues;
				UpdateRowRouteValues = src.UpdateRowRouteValues;
				DeleteRowRouteValues = src.DeleteRowRouteValues;
				BatchUpdateRouteValues = src.BatchUpdateRouteValues;
				ShowModelErrorsForEditors = src.ShowModelErrorsForEditors;
			}
		}
	}
	public class MVCxGridViewPopupControlSettings : ASPxGridViewPopupControlSettings {
		public MVCxGridViewPopupControlSettings()
			: base(null) {
		}
	}
	public class MVCxGridViewSearchPanelSettings : ASPxGridViewSearchPanelSettings {
		public MVCxGridViewSearchPanelSettings()
			: base(null) {
		}
		protected internal MVCxGridViewSearchPanelSettings(MVCxGridView gridView)
			: base(gridView) {
		}
		public string CustomEditorName { get { return base.CustomEditorID; } set { base.CustomEditorID = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string CustomEditorID { get { return base.CustomEditorID; } set { base.CustomEditorID = value; } }
	}
	public class MVCxGridViewBatchUpdateValues<T, S> where T : new() {
		bool editorErrorsLoaded;
		ModelStateDictionary modelState;
		Dictionary<T, ModelStateDictionary> editorErrors;
		public MVCxGridViewBatchUpdateValues() {
			Insert = new List<T>();
			Update = new List<T>();
			DeleteKeys = new List<S>();
			this.editorErrors = new Dictionary<T, ModelStateDictionary>();
		}
		protected ModelStateDictionary ModelState { get { return modelState; } set { modelState = value; } }
		public List<T> Insert { get; set; }
		public List<T> Update { get; set; }
		public List<S> DeleteKeys { get; set; }
		public Dictionary<T, ModelStateDictionary> EditorErrors {
			get {
				LoadEditorErrors();
				return editorErrors;
			}
		}
		public bool IsValid(T item) {
			return !EditorErrors.ContainsKey(item);
		}
		public void SetErrorText(T item, string errorText) {
			if(Insert.Contains(item))
				MVCxGridBatchEditHelperAdapter.SetInsertRowErrorText(ModelState, Insert.IndexOf(item), errorText);
			else if(Update.Contains(item))
				MVCxGridBatchEditHelperAdapter.SetUpdateRowErrorText(ModelState, Update.IndexOf(item), errorText);
		}
		public void SetErrorText(S key, string errorText) {
			if(!DeleteKeys.Contains(key)) return;
			MVCxGridBatchEditHelperAdapter.SetDeleteRowErrorText(ModelState, DeleteKeys.IndexOf(key), errorText);
		}
		protected virtual void LoadEditorErrors() {
			if(editorErrorsLoaded) return;
			editorErrorsLoaded = true;
			LoadEditorErrorsCore(Insert, MVCxGridBatchEditHelperAdapter.GetInsertEditorErrors(ModelState));
			LoadEditorErrorsCore(Update, MVCxGridBatchEditHelperAdapter.GetUpdateEditorErrors(ModelState));
		}
		protected virtual void LoadEditorErrorsCore(List<T> items, Dictionary<int, ModelStateDictionary> errorState) {
			foreach(var pair in errorState) {
				var index = pair.Key;
				if(items.Count <= index) continue;
				EditorErrors[items[index]] = pair.Value;
			}
		}
	}
	public class MVCxGridViewFormatConditionCollection : GridViewFormatConditionCollection {
		public GridViewFormatConditionHighlight AddHighlight() {
			return AddCore<GridViewFormatConditionHighlight>(null);
		}
		public GridViewFormatConditionHighlight AddHighlight(Action<GridViewFormatConditionHighlight> method) {
			return AddCore<GridViewFormatConditionHighlight>(method);
		}
		public GridViewFormatConditionHighlight AddHighlight(string fieldName, string expression, GridConditionHighlightFormat format) {
			var condition = new GridViewFormatConditionHighlight {
				FieldName = fieldName,
				Expression = expression,
				Format = format
			};
			Add(condition);
			return condition;
		}
		public GridViewFormatConditionTopBottom AddTopBottom(){
			return AddCore<GridViewFormatConditionTopBottom>(null);
		}
		public GridViewFormatConditionTopBottom AddTopBottom(Action<GridViewFormatConditionTopBottom> method) {
			return AddCore<GridViewFormatConditionTopBottom>(method);
		}
		public GridViewFormatConditionTopBottom AddTopBottom(string fieldName, GridTopBottomRule rule, GridConditionHighlightFormat format) {
			var condition = new GridViewFormatConditionTopBottom {
				FieldName = fieldName,
				Rule = rule,
				Format = format
			};
			Add(condition);
			return condition;
		}
		public GridViewFormatConditionColorScale AddColorScale() {
			return AddCore<GridViewFormatConditionColorScale>(null);
		}
		public GridViewFormatConditionColorScale AddColorScale(Action<GridViewFormatConditionColorScale> method) {
			return AddCore<GridViewFormatConditionColorScale>(method);
		}
		public GridViewFormatConditionColorScale AddColorScale(string fieldName, GridConditionColorScaleFormat format) {
			var condition = new GridViewFormatConditionColorScale {
				FieldName = fieldName,
				Format = format
			};
			Add(condition);
			return condition;
		}
		public GridViewFormatConditionIconSet AddIconSet() {
			return AddCore<GridViewFormatConditionIconSet>(null);
		}
		public GridViewFormatConditionIconSet AddIconSet(Action<GridViewFormatConditionIconSet> method) {
			return AddCore<GridViewFormatConditionIconSet>(method);
		}
		public GridViewFormatConditionIconSet AddIconSet(string fieldName, GridConditionIconSetFormat format) {
			var condition = new GridViewFormatConditionIconSet {
				FieldName = fieldName,
				Format = format
			};
			Add(condition);
			return condition;
		}
		T AddCore<T>(Action<T> method) where T: GridFormatConditionBase {
			var condition = Activator.CreateInstance<T>();
			if(method != null)
				method(condition);
			Add(condition);
			return condition;
		}
	}
}
