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
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public class ToolTipBlankSeparatorItem : ToolTipItem {
		internal int Height {
			get { return Appearance.Font.Height; }
		}
		internal ToolTipBlankSeparatorItem(int height) {
			TrySetHeight(height);
		}
		void TrySetHeight(int height) {
			height = height <= 0 ? 1 : height;
			if(Appearance.Font.Height == height)
				return;
			Appearance.Font = new Font(Appearance.Font.FontFamily, height, GraphicsUnit.Pixel);
		}
	}
	public class DashboardTitleFilterToolTip {
		internal const int MinFilterValues = 4;
		internal const int MaxFilterListValues = 100;
		const string defaultText = "Title";
		const int separatorItemSize = 1;
		static ToolTipItem CreateToolTipItem(bool isTitle, string text) {
			ToolTipItem item;
			if(isTitle)
				item = new ToolTipTitleItem();
			else
				item = new ToolTipItem();
			item.Text = text;
			return item;
		}
		static ToolTipBlankSeparatorItem CreateToolTipSeparatorItem() {
			return new ToolTipBlankSeparatorItem(separatorItemSize);
		}
		readonly ToolTipController toolTipController;
		readonly SuperToolTip tip;
		IList<DimensionFilterValues> masterFilterValues;
		int maxFilterListValues;
		internal ToolTipController ToolTipController { get { return toolTipController; } }
		internal SuperToolTip SuperTip { get { return tip; } }
		internal IList<DimensionFilterValues> MasterFilterValues {
			get { return masterFilterValues; }
			set {
				masterFilterValues = value;
				Refresh();
			}
		}
		public event EventHandler<RequestScreenHeightEventArgs> RequestScreenHeight;
		internal DashboardTitleFilterToolTip() {
			toolTipController = new ToolTipController();
			tip = new SuperToolTip();
			tip.DistanceBetweenItems = 4;
			toolTipController.AutoPopDelay = 100000;
			toolTipController.InitialDelay = 0;
			toolTipController.BeforeShow += new ToolTipControllerBeforeShowEventHandler(OnToolTipControllerBeforeShow);
		}
		void OnToolTipControllerBeforeShow(object sender, ToolTipControllerShowEventArgs e) {
			if(MasterFilterValues == null) {
				tip.Items.Clear();
				return;
			}
			int newMaxFilterListValues = CalcMaxFilterListValues(Screen.FromControl(e.SelectedControl).Bounds.Height);
			if(newMaxFilterListValues == maxFilterListValues)
				return;
			maxFilterListValues = newMaxFilterListValues;
			RefreshContent();
		}
		void Refresh() {
			RequestScreenHeightEventArgs eventArgs = new RequestScreenHeightEventArgs();
			if(RequestScreenHeight != null)
				RequestScreenHeight(this, eventArgs);
			maxFilterListValues = CalcMaxFilterListValues(eventArgs.ScreenHeight);
			RefreshContent();
		}
		void RefreshContent() {
			tip.Items.Clear();
			if(masterFilterValues == null)
				return;
			int maxFilterValues = CalcMaxFilterValues(maxFilterListValues);
			CreateTip(maxFilterValues);
		}
		void CreateTip(int maxFilterValues) {
			for(int i = 0; i < masterFilterValues.Count; i++) {
				DimensionFilterValues dimension = masterFilterValues[i];
				if(i > 0)
					tip.Items.Add(CreateToolTipSeparatorItem());
				tip.Items.Add(CreateToolTipItem(true, dimension.Name));
				for(int j = 0; j < maxFilterValues; j++) {
					if(j >= dimension.Values.Count) {
						if(dimension.Truncated)
							tip.Items.Add(CreateToolTipItem(false, "..."));
						break;
					}
					if(j + 1 == maxFilterValues && (j + 1 < dimension.Values.Count || dimension.Truncated)) {
						tip.Items.Add(CreateToolTipItem(false, "..."));
						break;
					}
					FormattableValue value = dimension.Values[j];
					string text = value == null ? string.Empty : DashboardTitlePresenter.FormatFilterValue(value);
					tip.Items.Add(CreateToolTipItem(false, text));
				}
			}
		}
		int CalcMaxFilterValues(int maxFilterListValues) {
			maxFilterListValues = Math.Min(maxFilterListValues, MaxFilterListValues);
			int maxExistingFilterValues = 0;
			foreach(DimensionFilterValues dimensionFilterValues in masterFilterValues) {
				maxExistingFilterValues = Math.Max(
					maxExistingFilterValues,
					dimensionFilterValues.Values.Count + (dimensionFilterValues.Truncated ? 1 : 0)
				);
			}
			int maxFilterValuesStart = Math.Max(Math.Min(maxFilterListValues, maxExistingFilterValues), MinFilterValues);
			int maxFilterValues;
			int curFilterListValues;
			for(maxFilterValues = maxFilterValuesStart; maxFilterValues >= MinFilterValues; maxFilterValues--) {
				if(maxFilterValues == MinFilterValues)
					break;
				curFilterListValues = 0;
				foreach(DimensionFilterValues dimensionFilterValues in masterFilterValues) {
					curFilterListValues += Math.Min(dimensionFilterValues.Values.Count, maxFilterValues) + 1;
					if(curFilterListValues > maxFilterListValues)
						break;
				}
				if(curFilterListValues <= maxFilterListValues)
					break;
			}
			return maxFilterValues;
		}
		int CalcMaxFilterListValues(int screenHeight) {
			if(screenHeight == 0) return 0;
			int textHeight = CreateToolTipItem(false, defaultText).Appearance.Font.Height;
			int separatorsHeight = masterFilterValues == null ? 0 : masterFilterValues.Count * (CreateToolTipSeparatorItem().Height + tip.DistanceBetweenItems);
			return (int)((screenHeight - tip.Padding.Top - tip.Padding.Bottom - separatorsHeight) / (textHeight + tip.DistanceBetweenItems));
		}
	}
	public class RequestScreenHeightEventArgs : EventArgs {
		public int ScreenHeight { get; set; }
		public RequestScreenHeightEventArgs() {
			ScreenHeight = 0;
		}
	}
}
