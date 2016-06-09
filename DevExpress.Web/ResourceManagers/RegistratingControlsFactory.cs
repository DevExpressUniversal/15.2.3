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
using System.Reflection;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	internal static class RegistrationControlsFactory {
		public static IList<ASPxWebControl> GetControlsForStyleSheetRegistration(Suite suite) {
			return GetControls(suite, false);
		}
		public static IList<ASPxWebControl> GetControlsForScriptRegistration(Suite suite) {
			return GetControls(suite, true);
		}
		public static IList<ASPxWebControl> GetControlsForScriptRegistration(ControlType controlType) {
			var controls = new List<ASPxWebControl>();
			switch(controlType) {
				case ControlType.ASPxBinaryImage:
					ASPxBinaryImage binaryImage = new ASPxBinaryImage();
					binaryImage.Properties.EnableClientSideAPI = true;
					controls.Add(binaryImage);
					break;
				case ControlType.ASPxButtonEdit:
					ASPxButtonEdit buttonEdit = new ASPxButtonEdit();
					buttonEdit.Buttons.Add("dummy_button");
					buttonEdit.MaskSettings.Mask = "dummy_mask";
					buttonEdit.DisplayFormatString = "dummy_format";
					controls.Add(buttonEdit);
					break;
				case ControlType.ASPxColorEdit:
					ASPxColorEdit colorEdit = new ASPxColorEdit();
					colorEdit.DisplayFormatString = "dummy_format";
					controls.Add(colorEdit);
					break;
				case ControlType.ASPxComboBox:
					ASPxComboBox comboBox = new ASPxComboBox();
					comboBox.DisplayFormatString = "dummy_format";
					controls.Add(comboBox);
					break;
				case ControlType.ASPxDataView:
					ASPxDataView dataView = new ASPxDataView();
					dataView.AlwaysShowPager = true;
					controls.Add(dataView);
					break;
				case ControlType.ASPxDateEdit:
					ASPxDateEdit dateEdit = new ASPxDateEdit();
					dateEdit.UseMaskBehavior = true;
					dateEdit.DisplayFormatString = "dummy_format";
					dateEdit.TimeSectionProperties.Visible = true;
					controls.Add(dateEdit);
					break;
				case ControlType.ASPxDocking:
					controls.AddRange(new ASPxWebControl[] { new ASPxDockPanel(), new ASPxDockZone(), new ASPxDockManager() });
					break;
				case ControlType.ASPxDropDownEdit:
					ASPxDropDownEdit dropDownEdit = new ASPxDropDownEdit();
					dropDownEdit.DisplayFormatString = "dummy_format";
					controls.Add(dropDownEdit);
					break;
				case ControlType.ASPxFileManager:
					controls.AddRange(new ASPxWebControl[] { new ASPxFileManager(), new ASPxTextBox() });
					break;
				case ControlType.ASPxHyperLink:
					ASPxHyperLink hyperLink = new ASPxHyperLink();
					hyperLink.Properties.EnableClientSideAPI = true;
					controls.Add(hyperLink);
					break;
				case ControlType.ASPxImage:
					ASPxImage image = new ASPxImage();
					image.Properties.EnableClientSideAPI = true;
					controls.Add(image);
					break;
				case ControlType.ASPxLabel:
					ASPxLabel label = new ASPxLabel();
					label.AssociatedControlID = "dummy_id";
					label.Properties.EnableClientSideAPI = true;
					controls.Add(label);
					break;
				case ControlType.ASPxMemo:
					ASPxMemo memo = new ASPxMemo();
					memo.DisplayFormatString = "dummy_format";
					controls.Add(memo);
					break;
				case ControlType.ASPxMenu:
					ASPxMenu menu = new ASPxMenu();
					menu.Items.Add("dummy_item").Items.Add("dummy_item");
					menu.EnableSubMenuScrolling = true;
					controls.Add(menu);
					break;
				case ControlType.ASPxNavBar:
					ASPxNavBar navBar = new ASPxNavBar();
					navBar.Groups.Add("dummy_group").Items.Add("dummy_item");
					navBar.SetInitialized(true);
					controls.Add(navBar);
					break;
				case ControlType.ASPxPopupMenu:
					ASPxPopupMenu popupMenu = new ASPxPopupMenu();
					popupMenu.Items.Add("dummy_item").Items.Add("dummy_item");
					controls.Add(popupMenu);
					break;
				case ControlType.ASPxRoundPanel:
					ASPxRoundPanel roundPanel = new ASPxRoundPanel();
					roundPanel.EnableClientSideAPI = true;
					controls.Add(roundPanel);
					break;
				case ControlType.ASPxSpinEdit:
					ASPxSpinEdit spinEdit = new ASPxSpinEdit();
					spinEdit.DisplayFormatString = "dummy_format";
					controls.Add(spinEdit);
					break;
				case ControlType.ASPxSplitter:
					ASPxSplitter splitter = new ASPxSplitter();
					splitter.Panes.Add("dummy_pane");
					controls.Add(splitter);
					break;
				case ControlType.ASPxTabControl:
					ASPxTabControl tabControl = new ASPxTabControl();
					tabControl.Tabs.Add("dummy_tab");
					controls.Add(tabControl);
					ASPxPageControl pageControl = new ASPxPageControl();
					pageControl.TabPages.Add("dummy_tabpage");
					controls.Add(pageControl);
					break;
				case ControlType.ASPxTextBox:
					ASPxTextBox textBox = new ASPxTextBox();
					textBox.MaskSettings.Mask = "dummy_mask";
					textBox.DisplayFormatString = "dummy_format";
					controls.Add(textBox);
					break;
				case ControlType.ASPxTimeEdit:
					ASPxTimeEdit timeEdit = new ASPxTimeEdit();
					timeEdit.DisplayFormatString = "dummy_format";
					controls.Add(timeEdit);
					break;
				case ControlType.ASPxTreeView:
					ASPxTreeView treeView = new ASPxTreeView();
					treeView.Nodes.Add("dummy_node");
					controls.Add(treeView);
					break;
				case ControlType.ASPxUploadControl:
					ASPxUploadControl uploadControl = new ASPxUploadControl();
					uploadControl.ShowAddRemoveButtons = true;
					uploadControl.ShowProgressPanel = true;
					uploadControl.ShowUploadButton = true;
					controls.Add(uploadControl);
					break;
				default:
					controls.Add(CreateControl(controlType));
					break;
			}
			return controls;
		}
		static IList<ASPxWebControl> GetControls(Suite suite, bool scriptRegistration) {
			var controls = new List<ASPxWebControl>();
			switch(suite) {
				case Suite.NavigationAndLayout:
					AddNavigationControls(ref controls, scriptRegistration);
					break;
				case Suite.Editors:
					AddEditorsControls(ref controls, scriptRegistration);
					break;
				case Suite.Gauges:
					AddGaugeControl(ref controls);
					break;
				case Suite.Grid:
					AddGridViewControl(ref controls);
					AddNestedEditingControls(ref controls, scriptRegistration);
					break;
				case Suite.HtmlEditor:
					AddHtmlEditorControl(ref controls);
					AddSpellCheckerControl(ref controls);
					AddNestedEditingControls(ref controls, scriptRegistration);
					break;
				case Suite.PivotGrid:
					AddPivotGridControl(ref controls);
					AddNestedEditingControls(ref controls, scriptRegistration);
					break;
				case Suite.Scheduling:
					AddSchedulingControls(ref controls);
					break;
				case Suite.SpellChecker:
					AddSpellCheckerControl(ref controls);
					break;
				case Suite.Spreadsheet:
					AddSpreadsheetControl(ref controls);
					break;
				case Suite.RichEdit:
					AddRichEditControl(ref controls);
					break;
				case Suite.TreeList:
					AddTreeListControl(ref controls);
					AddNestedEditingControls(ref controls, scriptRegistration);
					break;
				case Suite.Charting:
					AddChartingControl(ref controls);
					break;
				case Suite.Reporting:
					AddReportingControls(ref controls);
					break;
				case Suite.All:
					AddAllControls(ref controls, scriptRegistration);
					break;
				default:
					break;
			}
			return controls;
		}
		static void AddAllControls(ref List<ASPxWebControl> controls, bool scriptRegistration) {
			AddNavigationControls(ref controls, scriptRegistration);
			AddEditorsControls(ref controls, scriptRegistration);
			AddGaugeControl(ref controls);
			AddGridViewControl(ref controls);
			AddHtmlEditorControl(ref controls);
			AddPivotGridControl(ref controls);
			AddSchedulingControls(ref controls);
			AddSpellCheckerControl(ref controls);
			AddSpreadsheetControl(ref controls);
			AddRichEditControl(ref controls);
			AddTreeListControl(ref controls);
			AddChartingControl(ref controls);
			AddReportingControls(ref controls);
		}
		static void AddNavigationControls(ref List<ASPxWebControl> controls, bool scriptRegistration) {
			if(scriptRegistration) {
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCallback));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCallbackPanel));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCloudControl));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxDataView));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxDocking));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxFileManager));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxFormLayout));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxGlobalEvents));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxHiddenField));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxImageGallery));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxImageSlider));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxImageZoom));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxImageZoomNavigator));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxLoadingPanel));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxMenu));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxNavBar));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxNewsControl));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxPager));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxPanel));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxPopupControl));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxPopupMenu));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxRatingControl));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxRibbon));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxRoundPanel));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxSplitter));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTabControl));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTimer));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTitleIndex));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTreeView));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxUploadControl));
			}
			else
				controls.Add(new ASPxMenu());
		}
		static void AddEditorsControls(ref List<ASPxWebControl> controls, bool scriptRegistration) {
			if(scriptRegistration) {
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxBinaryImage));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxButton));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxButtonEdit));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCalendar));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCaptcha));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCheckBox));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxCheckBoxList));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxColorEdit));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxComboBox));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxDateEdit));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxDropDownEdit));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxHyperLink));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxImage));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxLabel));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxListBox));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxMemo));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxProgressBar));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxRadioButton));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxRadioButtonList));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxSpinEdit));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTextBox));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTimeEdit));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTokenBox));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTrackBar));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxValidationSummary));
			}
			else
				controls.Add(new ASPxButtonEdit());
		}
		static void AddGaugeControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblyASPxGauges, "DevExpress.Web.ASPxGauges.ASPxGaugeControl"));
		}
		static void AddGridViewControl(ref List<ASPxWebControl> controls) {
			ASPxGridView grid = new ASPxGridView();
			grid.Columns.Add(new DevExpress.Web.GridViewDataColumn());
			grid.Settings.ShowFilterRow = true;
			grid.Settings.ShowFilterRowMenu = true;
			grid.ShowFilterControl();
			grid.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.Control;
			grid.Settings.HorizontalScrollBarMode = ScrollBarMode.Visible;
			grid.Settings.VerticalScrollBarMode = ScrollBarMode.Visible;
			grid.SettingsPager.AlwaysShowPager = true;
			controls.Add(grid);
		}
		static void AddHtmlEditorControl(ref List<ASPxWebControl> controls) {
			ASPxWebControl htmlEditor = CreateControl(AssemblyInfo.SRAssemblyHtmlEditorWeb, "DevExpress.Web.ASPxHtmlEditor.ASPxHtmlEditor");
			ReflectionUtils.InvokeInstanceMethod(htmlEditor, "CreateDefaultToolbars", false);
			controls.Add(htmlEditor);
		}
		static void AddPivotGridControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblyASPxPivotGrid, "DevExpress.Web.ASPxPivotGrid.ASPxPivotGrid"));
		}
		static void AddSchedulingControls(ref List<ASPxWebControl> controls) {
			controls.AddRange(new[] {
				CreateControl(AssemblyInfo.SRAssemblySchedulerWeb, "DevExpress.Web.ASPxScheduler.ASPxScheduler"),
				CreateControl(AssemblyInfo.SRAssemblySchedulerWeb, "DevExpress.Web.ASPxScheduler.Controls.AppointmentRecurrenceForm"),
				CreateControl(AssemblyInfo.SRAssemblySchedulerWeb, "DevExpress.Web.ASPxScheduler.Controls.ASPxSchedulerStatusInfo")
			});
		}
		static void AddSpellCheckerControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblySpellCheckerWeb, "DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker"));
		}
		static void AddSpreadsheetControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblyWebSpreadsheet, "DevExpress.Web.ASPxSpreadsheet.ASPxSpreadsheet"));
		}
		static void AddRichEditControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblyWebRichEdit, "DevExpress.Web.ASPxRichEdit.ASPxRichEdit"));
		}
		static void AddTreeListControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblyTreeListWeb, "DevExpress.Web.ASPxTreeList.ASPxTreeList"));
		}
		static void AddChartingControl(ref List<ASPxWebControl> controls) {
			controls.Add(CreateControl(AssemblyInfo.SRAssemblyChartsWeb, "DevExpress.XtraCharts.Web.WebChartControl"));
		}
		static void AddReportingControls(ref List<ASPxWebControl> controls) {
			controls.AddRange(new[] {
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ReportToolbar"),
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ReportViewer"),
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ReportParametersPanel"),
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ReportDocumentMap"),
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ASPxDocumentViewer"),
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ASPxReportDesigner"),
				CreateControl(AssemblyInfo.SRAssemblyReportsWeb, "DevExpress.XtraReports.Web.ASPxWebDocumentViewer")
			});
		}
		static void AddNestedEditingControls(ref List<ASPxWebControl> controls, bool scriptRegistration) {
			if(scriptRegistration) {
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxTextBox));
				controls.AddRange(GetControlsForScriptRegistration(ControlType.ASPxPopupControl));
			}
			else {
				AddNavigationControls(ref controls, scriptRegistration);
				AddEditorsControls(ref controls, scriptRegistration);
			}
		}
		static ASPxWebControl CreateControl(ControlType controlType) {
			Type[] types = Assembly.GetCallingAssembly().GetTypes();
			foreach(Type type in types) {
				if(type.Name == controlType.ToString())
					return CreateControl(type);
			}
			return null;
		}
		static ASPxWebControl CreateControl(string assemblyName, string controlName) {
			Assembly assembly = Assembly.Load(assemblyName + AssemblyInfo.FullAssemblyVersionExtension);
			return CreateControl(assembly.GetType(controlName));
		}
		static ASPxWebControl CreateControl(Type type) {
			return (ASPxWebControl)Activator.CreateInstance(type);
		}
	}
}
