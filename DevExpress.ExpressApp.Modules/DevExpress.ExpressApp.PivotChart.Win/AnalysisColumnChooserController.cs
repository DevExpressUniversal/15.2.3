#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.XtraPivotGrid;
using DevExpress.ExpressApp.Utils;
using System.Windows.Forms;
using DevExpress.Data.PivotGrid;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.PivotChart.Win {
	public interface IAnalysisEditorWin {
		AnalysisControlWin Control { get; }
		bool IsDataSourceReady { get; }
		object PropertyValue { get; }
		event EventHandler<EventArgs> IsDataSourceReadyChanged;
		event EventHandler<EventArgs> ControlCreated;
	}
	public partial class AnalysisColumnChooserController : ColumnChooserControllerBase {
		private IAnalysisEditorWin analysisEditor;
		private PivotGridControl pivotGrid;
		private void analysisEditor_ControlCreated(object sender, EventArgs e) {
			AttachToPivotGrid(((IAnalysisEditorWin)sender).Control.PivotGrid);
		}
		private void PivotGrid_ShowCustomizationForm(object sender, EventArgs e) {
			InsertButtons();
			UpdateRemoveButtonState();
			UpdateAddButtonState();
			pivotGrid.CustomizationForm.ActiveListBox.Items.ListChanged += new ListChangedEventHandler(ActiveListBoxItems_ListChanged);
			analysisEditor.IsDataSourceReadyChanged += new EventHandler<EventArgs>(analysisEditor_IsDataSourceReadyChanged);
		}
		private void analysisEditor_IsDataSourceReadyChanged(object sender, EventArgs e) {
			UpdateAddButtonState();
		}
		private void UpdateAddButtonState() {
			AddButton.Enabled = analysisEditor.IsDataSourceReady;
		}
		private void ActiveListBoxItems_ListChanged(object sender, ListChangedEventArgs e) {
			UpdateRemoveButtonState();
		}
		private void PivotGrid_HideCustomizationForm(object sender, EventArgs e) {
			DeleteButtons();
			analysisEditor.IsDataSourceReadyChanged -= new EventHandler<EventArgs>(analysisEditor_IsDataSourceReadyChanged);
		}
		private void UpdateRemoveButtonState() {
			RemoveButton.Enabled = pivotGrid.CustomizationForm.ActiveListBox.Items.Count > 0;
		}
#if DebugTest
		protected virtual void AddPropertiesNotVisibleInAnalysis(List<string> listToAddIn, string typeName) {
#else
		private void AddPropertiesNotVisibleInAnalysis(List<string> listToAddIn, string typeName) {
#endif
			IModelApplication modelApplication = Application.Model;
			IModelClass modelClass = typeName != null ? modelApplication.BOModel[typeName] : null;
			if(modelClass != null) {
				foreach(IModelMember modelMember in modelClass.AllMembers) {
					if(modelMember != null && modelMember is IModelMemberPivotChartVisibility && !((IModelMemberPivotChartVisibility)modelMember).IsVisibleInAnalysis) {
						listToAddIn.Add(modelMember.Name);
					}
				}
				if(modelClass.BaseClass != null) {
					AddPropertiesNotVisibleInAnalysis(listToAddIn, modelClass.BaseClass.TypeInfo.FullName);
				}
			}
		}
		private void AttachToPivotEditor(DetailView view) {
			IList<IAnalysisEditorWin> analysisEditors = view.GetItems<IAnalysisEditorWin>();
			if(analysisEditors.Count > 0) {
				analysisEditor = analysisEditors[0];
				if(analysisEditor.Control == null) {
					analysisEditor.ControlCreated += new EventHandler<EventArgs>(analysisEditor_ControlCreated);
				}
				else {
					AttachToPivotGrid(analysisEditor.Control.PivotGrid);
				}
			}
		}
		protected virtual void AttachToPivotGrid(PivotGridControl pivotGrid) {
			Guard.ArgumentNotNull(pivotGrid, "pivotGrid");
			if(this.pivotGrid != pivotGrid) {
				DetachFromPivotGrid();
				this.pivotGrid = pivotGrid;
				pivotGrid.ShowCustomizationForm += new EventHandler(PivotGrid_ShowCustomizationForm);
				pivotGrid.HideCustomizationForm += new EventHandler(PivotGrid_HideCustomizationForm);
			}
		}
		private void DetachFromPivotGrid() {
			if(pivotGrid != null) {
				pivotGrid.ShowCustomizationForm -= new EventHandler(PivotGrid_ShowCustomizationForm);
				pivotGrid.HideCustomizationForm -= new EventHandler(PivotGrid_HideCustomizationForm);
				if(!pivotGrid.IsDisposed) {
					if(pivotGrid.CustomizationForm != null && pivotGrid.CustomizationForm.Visible) {
						pivotGrid.CustomizationForm.ActiveListBox.Items.ListChanged -= new ListChangedEventHandler(ActiveListBoxItems_ListChanged);
					}
				}
				pivotGrid = null;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			AttachToPivotEditor((DetailView)View);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			DetachFromPivotGrid();
			if(analysisEditor != null) {
				analysisEditor.ControlCreated -= new EventHandler<EventArgs>(analysisEditor_ControlCreated);
				analysisEditor = null;
			}
		}		
		protected override Control ActiveListBox {
			get { return pivotGrid.CustomizationForm.ActiveListBox; }
		}
		protected override ITypeInfo DisplayedTypeInfo {
			get { return XafTypesInfo.Instance.FindTypeInfo(((IAnalysisInfo)analysisEditor.PropertyValue).DataType); }
		}
		protected override List<string> GetUsedProperties() {
			List<string> result = new List<string>();
			Type dataType = ((IAnalysisInfo)View.CurrentObject).DataType;
			AddPropertiesNotVisibleInAnalysis(result, dataType.FullName);
			foreach(PivotGridField field in pivotGrid.Fields) {
				if(!result.Contains(field.FieldName)) {
					result.Add(field.FieldName);
				}
			}
			return result;
		}
#if DebugTest
		public void AddColumnToTests(string propertyName) {
			AddColumn(propertyName);
		}
#endif
		protected override void AddColumn(string propertyName) {
			if(analysisEditor == null || analysisEditor.Control == null) {
				return;
			}
			IMemberInfo memberInfo = DisplayedTypeInfo.FindMember(propertyName);
			PivotGridFieldBuilder fieldBuilder = analysisEditor.Control.FieldBuilder;
			PivotGridField pivotGridField = analysisEditor.Control.CreatePivotGridField(CaptionHelper.GetFullMemberCaption(DisplayedTypeInfo, memberInfo.Name), fieldBuilder.GetBindingName(memberInfo), fieldBuilder.GetSummaryType(memberInfo));
			pivotGridField.Visible = false;
			if(!GetUsedProperties().Contains(pivotGridField.FieldName)) {
				analysisEditor.Control.PivotGrid.Fields.Add(pivotGridField);
				fieldBuilder.SetupPivotGridField(memberInfo);
				IAnalysisInfo analysisInfo = (IAnalysisInfo)analysisEditor.PropertyValue;
				if(!analysisInfo.DimensionProperties.Contains(propertyName)) {
					analysisInfo.DimensionProperties.Add(propertyName);
				}
			}
		}
		protected override void RemoveSelectedColumn() {
			DevExpress.XtraPivotGrid.Customization.PivotCustomizationTreeBox treeBox = (DevExpress.XtraPivotGrid.Customization.PivotCustomizationTreeBox)ActiveListBox;
			PivotGridField pivotGridField = ((IPivotGridViewInfoDataOwner)pivotGrid).DataViewInfo.GetField(treeBox.SelectedItem.Field);
			IAnalysisInfo analysisInfo = (IAnalysisInfo)analysisEditor.PropertyValue;
			pivotGrid.Fields.Remove(pivotGridField);
			if(analysisInfo.DimensionProperties.Contains(pivotGridField.FieldName)) {
				analysisInfo.DimensionProperties.Remove(pivotGridField.FieldName);
			}			
		}
		public AnalysisColumnChooserController() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
