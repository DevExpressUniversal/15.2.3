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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.UI {
	#region ChangeAppearanceGalleryItem
	public class ChangeAppearanceGalleryItem : ChartCommandGalleryItem {
		public ChangeAppearanceGalleryItem()
			: base() {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ChangeAppearance; } }
		ChartAppearanceInfo ActualAppearanceInfo {
			get {
				Chart chart = Control.Chart;
				return new ChartAppearanceInfo(chart.AppearanceRepository[chart.AppearanceName], chart.PaletteBaseColorNumber);
			}
		}
		ICommandUIState CreateAppearanceCommandUIState(Command command) {
			DefaultValueBasedCommandUIState<ChartAppearanceInfo> state = new DefaultValueBasedCommandUIState<ChartAppearanceInfo>();
			state.Value = Tag as ChartAppearanceInfo;
			return state;
		}
		protected override void InvokeCommand() {
			Command command = CreateCommand();
			if (command != null) {
				ICommandUIState state = CreateAppearanceCommandUIState(command);
				if (command.CanExecute())
					command.ForceExecute(state);
			}
		}
		protected override SuperToolTip GetSuperTip() {
			return null;
		}
		internal void UnsubscribeEvents() {
			if (Control != null)
				UnsubscribeControlEvents();
		}
	}
	#endregion
	#region ChangeAppearanceGalleryBaseBarManagerItem
	public class ChangeAppearanceGalleryBaseBarManagerItem : ChartCommandDropDownGalleryBarItem {
		static readonly int galleryColumnCount = 7;
		static readonly int galleryRowCount = 4;
		protected override ChartCommandId CommandId { get { return ChartCommandId.ChangeAppearancePlaceHolder; } }
		protected override Size GalleryImageSize { get { return new Size(80, 50); } }
		protected override int GalleryColumnCount { get { return galleryColumnCount; } }
		protected override int GalleryRowCount { get { return galleryRowCount; } }
		protected override bool ShowGroupCaption { get { return false; } }
		protected override bool ShowItemText { get { return false; } }
		public ChangeAppearanceGalleryBaseBarManagerItem()
			: base() {
		}
		public ChangeAppearanceGalleryBaseBarManagerItem(BarManager manager)
			: base(manager) {
		}
		public ChangeAppearanceGalleryBaseBarManagerItem(string caption)
			: base(caption) {
		}
		public ChangeAppearanceGalleryBaseBarManagerItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		void PopulateGalleryItems() {
			if (DesignMode || Control == null)
				return;
			InDropDownGallery gallery = GalleryDropDown.Gallery;
			gallery.BeginUpdate();
			if (gallery.Groups.Count == 0)
				gallery.Groups.Add(new GalleryItemGroup());
			GalleryItemCollection galleryItems = gallery.Groups[0].Items;
			foreach (GalleryItem item in galleryItems) {
				ChangeAppearanceGalleryItem changePaletteGalleryItem = item as ChangeAppearanceGalleryItem;
				changePaletteGalleryItem.UnsubscribeEvents();
				changePaletteGalleryItem.Dispose();
			}
			galleryItems.Clear();
			Chart chart = Control.Chart;
			Palette currentPalette = chart.PaletteRepository[Control.Chart.PaletteName];
			ChartAppearanceInfo currentApperarnceInfo = new ChartAppearanceInfo(chart.AppearanceRepository[chart.AppearanceName], chart.PaletteBaseColorNumber);
			ViewType viewType = chart.Series.Count == 0 ? ViewType.Bar : SeriesViewFactory.GetViewType(chart.Series[0].View);
			foreach(ChartAppearance appearance in chart.AppearanceRepository) {
				if (!currentPalette.Predefined || appearance == chart.AppearanceRepository[chart.AppearanceName] || String.IsNullOrEmpty(appearance.PaletteName) || appearance.PaletteName == currentPalette.Name)
					for (int i = 0; i <= currentPalette.Count; i++) {
						ChangeAppearanceGalleryItem galleryItem = new ChangeAppearanceGalleryItem();
						galleryItem.Image = AppearanceImageHelper.CreateImage(viewType, appearance, currentPalette, i);
						ChartAppearanceInfo apperarnceInfo = new ChartAppearanceInfo(appearance, i);
						galleryItem.Tag = apperarnceInfo;
						galleryItem.Control = Control;
						galleryItems.Add(galleryItem);
						if (apperarnceInfo.Equals(currentApperarnceInfo)) {
							galleryItem.Checked = true;
							gallery.MakeVisible(galleryItem);
						}
					}
			}
			gallery.EndUpdate();
		}
		protected override void OnControlChanged() {
			base.OnControlChanged();
			PopulateGalleryItems();
		}
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			PopulateGalleryItems();
		}
	}
	#endregion
}
