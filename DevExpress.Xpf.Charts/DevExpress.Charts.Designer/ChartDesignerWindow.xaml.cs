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

using DevExpress.Charts.Native;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Charts.Designer.Native {
	public partial class ChartDesignerWindow : DXRibbonWindow {
		readonly CommandManager commandManager = new CommandManager();
		readonly WpfChartModel chartModel;
		readonly Theme defaulTheme = Theme.MetropolisLight;
		readonly DesignerWindowLayout layout;
		public static RoutedCommand DeleteElementChartCommand = new RoutedCommand();
		ChartDesigner designer;
		DesignerViewModel designerViewModel;
		bool showSaveDialog = true;
		public string CancelCaption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.CancelButtonCaption); }
		}
		public string SaveAndExitCaption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SaveAndExitButtonCaption); }
		}
		public string UndoCaption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Undo); }
		}
		public string RedoCaption {
			get { return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.Redo); }
		}
		public ChartDesignerWindow(ChartDesigner designer, bool designTime, Theme forceTheme) {
			this.designer = designer;
			InitializeComponent();
			layout = new DesignerWindowLayout();
			InitializeFormByLayout();
			chartModel = new WpfChartModel(designer.DesignerChart, commandManager);
			chartModel.RecursivelyUpdateChildren();
			designerViewModel = new DesignerViewModel(chartModel);
			DataContext = designerViewModel;
			DesignerProperties.SetIsInDesignMode(gridSeriesData, false);
			DesignerProperties.SetIsInDesignMode(chartStructureTree, false);
			if (forceTheme == null) {
				if (designTime) {
					try {
						ThemeManager.SetTheme(this, defaulTheme);
					}
					catch (Exception e) {
						ChartDebug.WriteWarning("Unable to set the MetropolisLight theme. Exception occured: " + e.Message);
					}
				}
				else {
					try {
						Theme theme = ThemeManager.GetTheme(designer.UserChart);
						ThemeManager.SetTheme(this, theme);
					}
					catch (Exception) { }
				}
			}
			else
				ThemeManager.SetTheme(this, forceTheme);
		}
		void InitializeFormByLayout() {
			if (layout.Size != Size.Empty) {
				Width = layout.Size.Width;
				Height = layout.Size.Height;
			}
			WindowState = layout.Maximized ? WindowState.Maximized : WindowState.Normal;
		}
		void OnDXRibbonWindowLoaded(object sender, RoutedEventArgs e) {
			this.dockLayoutManager.DockController.Hide(this.layoutPanelForChartTree);
			this.dockLayoutManager.DockController.Hide(this.layoutPanelForSeriesData);
		}
		void CommandDeleteElement_Executed(object sender, ExecutedRoutedEventArgs e) {
			chartModel.DeleteSelectedObject();
		}
		void ChartDesignerWindowClosing(object sender, CancelEventArgs e) {
			if (commandManager.Commands.Count != 0 && showSaveDialog) {
				string caption = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ChartDesignerWindowTitle);
				string message = ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.SaveAndExitWindowText);
				MessageBoxResult result = DXMessageBox.Show(this, message, caption, MessageBoxButton.YesNoCancel);
				if (result == MessageBoxResult.Yes)
					if (designer.IsDesignTime)
						commandManager.ApplyAllCommands(designer);
					else
						commandManager.ApplyAllCommands(designer.UserChart);
				e.Cancel = result == MessageBoxResult.Cancel;
			}
			if (!e.Cancel) {
				layout.Size = this.RestoreBounds.Size;
				designerViewModel.CleanUp();
			}
		}
		void btnUndo_ItemClick(object sender, ItemClickEventArgs e) {
			commandManager.Undo();
		}
		void btnRedo_ItemClick(object sender, ItemClickEventArgs e) {
			commandManager.Redo();
		}
		void btnCancel_Click(object sender, RoutedEventArgs e) {
			commandManager.Commands.Clear();
			Close();
		}
		void btnSaveAndExit_Click(object sender, RoutedEventArgs e) {
			if (designer.IsDesignTime)
				commandManager.ApplyAllCommands(designer);
			else
				commandManager.ApplyAllCommands(designer.UserChart);
			commandManager.Commands.Clear();
			Close();
		}
		void PatternEditorButtonClick(object sender, RoutedEventArgs e) {
			PatternEditor editor = new PatternEditor(defaulTheme,
				propertyGrid.SelectedPropertyValue.ToString(),
				PatternEditorUtils.GetAvailablePlaceholders(chartModel.SelectedModel.ChartElement, propertyGrid.SelectedPropertyPath),
				PatternEditorUtils.CreatePatternValuesSource(chartModel.SelectedModel.ChartElement));
			editor.ShowDialog();
			if (editor.DialogResult.HasValue && editor.DialogResult.Value)
				propertyGrid.SetRowValueByRowPath(propertyGrid.SelectedPropertyPath, editor.Pattern);
		}
		internal void CloseWithoutDialog() {
			showSaveDialog = false;
			Close();
			showSaveDialog = true;
		}
		internal void SaveLayoutToRegistry() {
			layout.Maximized = WindowState == WindowState.Maximized;
			layout.SaveLayoutToRegistry();
		}
	}
}
