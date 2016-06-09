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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using System.Collections.Generic;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardWin.Native {
	public partial class ChartSeriesOptionsForm : DashboardForm {
		readonly ChartSelectorContext context;
		readonly ChartChangeSeriesOptionsCommandBase cancelCommand;
		public ChartSeriesOptionsForm() {
			InitializeComponent();
			tabbedControlGroup.SelectedTabPage = lcgSeriesType;
			tabbedControlGroup.SelectedTabPageIndex = 0;
		}
		public ChartSeriesOptionsForm(ChartSelectorContext context, IEnumerable<SeriesViewGroup> seriesViewGroups)
			: this() {
			this.context = context;
			cancelCommand = new CancelChartViewSelectorCommand(context);
			seriesGallery.Initialize(new ChartChangeSeriesOptionCommandFactory(context), seriesViewGroups);
			seriesGallery.GalleryControl.Controller = barAndDockingController1;
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			cePlotOnSecondaryAxis.Checked = context.Series.PlotOnSecondaryAxis;
			ceIgnoreEmptyPoints.Checked = context.Series.IgnoreEmptyPoints;
			ceShowPointMarkers.Checked = context.Series.ShowPointMarkers;
			ceShowPointLabels.Checked = context.Series.PointLabelOptions.ShowPointLabels;
			ceShowForZeroValues.Checked = context.Series.PointLabelOptions.ShowForZeroValues;
			cbOverlappingMode.SelectedIndex = (int)context.Series.PointLabelOptions.OverlappingMode;
			cbContentType.SelectedIndex = (int)context.Series.PointLabelOptions.Content;
			cbOrientation.SelectedIndex = (int)context.Series.PointLabelOptions.Orientation;
			cbPosition.SelectedIndex = (int)context.Series.PointLabelOptions.Position;
			UpdateEnable(GetActualCommand());
			seriesGallery.SelectedItemChanged += ExecuteCommand;
			cePlotOnSecondaryAxis.CheckedChanged += ExecuteCommand;
			ceIgnoreEmptyPoints.CheckedChanged += ExecuteCommand;
			ceShowPointMarkers.CheckedChanged += ExecuteCommand;
			ceShowPointLabels.CheckedChanged += ExecuteCommand;
			ceShowForZeroValues.CheckedChanged +=ExecuteCommand;
			cbOverlappingMode.SelectedIndexChanged += ExecuteCommand;
			cbContentType.SelectedIndexChanged += ExecuteCommand;
			cbOrientation.SelectedIndexChanged += ExecuteCommand;
			cbPosition.SelectedIndexChanged += ExecuteCommand;
		}
		~ChartSeriesOptionsForm() {
			seriesGallery.SelectedItemChanged -= ExecuteCommand;
			cePlotOnSecondaryAxis.CheckedChanged -= ExecuteCommand;
			ceIgnoreEmptyPoints.CheckedChanged -= ExecuteCommand;
			ceShowPointMarkers.CheckedChanged -= ExecuteCommand;
			ceShowPointLabels.CheckedChanged -= ExecuteCommand;
			ceShowForZeroValues.CheckedChanged -= ExecuteCommand;
			cbOverlappingMode.SelectedIndexChanged -= ExecuteCommand;
			cbContentType.SelectedIndexChanged -= ExecuteCommand;
			cbOrientation.SelectedIndexChanged -= ExecuteCommand;
			cbPosition.SelectedIndexChanged -= ExecuteCommand;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			const int minSizeHeight = 370;
			Size initialSize = ClientSize;
			Size size = seriesGallery.CalcBestSize();
			size.Width += initialSize.Width + seriesGallery.Left - seriesGallery.Right;
			size.Height += initialSize.Height + seriesGallery.Top - seriesGallery.Bottom;
			int minSizeWidth = lcgRoot.MinSize.Width + lcgRoot.Padding.Width;
			ClientSize = new Size(Math.Max(size.Width, minSizeWidth), Math.Max(size.Height, minSizeHeight));
			MinimumSize = new Size(minSizeWidth, minSizeHeight);
		}
		void ExecuteCommand(object sender, EventArgs e) {
			ChartChangeSeriesOptionCommand command = GetActualCommand();
			if(command != null) {
				command.Options.PlotOnSecondaryAxis = cePlotOnSecondaryAxis.Checked;
				command.Options.IgnoreEmptyPoints = ceIgnoreEmptyPoints.Checked;
				command.Options.ShowPointMarkers = ceShowPointMarkers.Checked;
				command.Options.PointLabelOptions.Assign(new PointLabelOptions() {
					ShowPointLabels = ceShowPointLabels.Checked,
					ShowForZeroValues = ceShowForZeroValues.Checked,
					OverlappingMode = (PointLabelOverlappingMode)cbOverlappingMode.SelectedIndex,
					Content = (PointLabelContentType)cbContentType.SelectedIndex,
					Orientation = (PointLabelOrientation)cbOrientation.SelectedIndex,
					Position = (PointLabelPosition)cbPosition.SelectedIndex
				});
				command.Execute();
				UpdateEnable(command);
			}
		}
		ChartChangeSeriesOptionCommand GetActualCommand() { 
			IList<GalleryItem> chechedItems = seriesGallery.GetCheckedItems();
			if(chechedItems.Count > 0) {
				return chechedItems[0].Tag as ChartChangeSeriesOptionCommand;
			}
			return null;
		}
		void UpdateEnable(ChartChangeSeriesOptionCommand command) {
			ceShowPointMarkers.Enabled = command.ShowPointMarkersEnable();
			ceIgnoreEmptyPoints.Enabled = command.IgnoreEmptyPointsEnable();
			cbOverlappingMode.Enabled = ceShowPointLabels.Checked;
			cbContentType.Enabled = ceShowPointLabels.Checked;
			cbOrientation.Enabled = ceShowPointLabels.Checked;
			ceShowForZeroValues.Enabled = ceShowPointLabels.Checked;
			cbPosition.Enabled = ceShowPointLabels.Checked && command.PointLabelPositionEnable();
			bool barOptionVisible = command.BarPointLabelOptionsEnable();
			bool bubbleOptionVisible = command.BubblePointLabelOptionsEnable();
			lciPosition.Visibility = barOptionVisible || bubbleOptionVisible ? LayoutVisibility.Always : LayoutVisibility.Never;
			separatorPointLabelOptions.Visibility = barOptionVisible || bubbleOptionVisible ? LayoutVisibility.Always : LayoutVisibility.Never;
			lciShowForZeroValues.Visibility = barOptionVisible ? LayoutVisibility.Always : LayoutVisibility.Never;
			lblBarOptions.Visibility = barOptionVisible ? LayoutVisibility.Always : LayoutVisibility.Never;
			lblBubbleOptions.Visibility = bubbleOptionVisible ? LayoutVisibility.Always : LayoutVisibility.Never;
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if(DialogResult == DialogResult.Cancel) {
				if(cancelCommand != null)
					cancelCommand.Execute();
			}
			else if(context != null)
				context.ApplyHistoryItem();
		}
	}
}
