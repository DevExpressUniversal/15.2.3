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

using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class AddSeriesUserControl : GalleryWithFilterUserControl {
		public AddSeriesUserControl(SortedSet<ViewType> availableViews) {		  
			UpdateItemsState(availableViews);
		}
		void UpdateItemsState(SortedSet<ViewType> availableVies) {
			foreach (GalleryItemGroup group in gallery.Gallery.Groups)
				foreach (GalleryItem item in group.Items)
					item.Enabled = availableVies.Contains((ViewType)item.Tag);
		}
		protected override void FillGallery(GalleryControl gallery) {
			ViewTypesGalleryItemsBuilder.FillGalleryControl(gallery);
		}
	}
	public static class ViewTypesGalleryItemsBuilder {
		public static void FillGalleryControl(GalleryControl galleryControl) {
			Dictionary<string, List<ViewType>> galleryGroups = new Dictionary<string, List<ViewType>>();
			galleryGroups.Add("Bar Series", new List<ViewType>() { 
				ViewType.Bar,
				ViewType.StackedBar,
				ViewType.FullStackedBar,
				ViewType.SideBySideStackedBar,
				ViewType.SideBySideFullStackedBar
			});
			galleryGroups.Add("Bar Series 3D", new List<ViewType>() {
				ViewType.Bar3D,
				ViewType.StackedBar3D,
				ViewType.FullStackedBar3D,
				ViewType.ManhattanBar,
				ViewType.SideBySideStackedBar3D,
				ViewType.SideBySideFullStackedBar3D
			});
			galleryGroups.Add("Point And Bubble", new List<ViewType>() {
				ViewType.Point,
				ViewType.Bubble
			});
			galleryGroups.Add("Line Series", new List<ViewType>() {
				ViewType.Line,
				ViewType.StackedLine,
				ViewType.FullStackedLine,
				ViewType.StepLine,
				ViewType.Spline,
				ViewType.ScatterLine,
				ViewType.SwiftPlot
			});
			galleryGroups.Add("Line Series 3D", new List<ViewType>() {
				ViewType.Line3D,
				ViewType.StackedLine3D,
				ViewType.FullStackedLine3D,
				ViewType.StepLine3D,
				ViewType.Spline3D
			});
			galleryGroups.Add("Area Series", new List<ViewType>() {
				ViewType.Area,
				ViewType.StackedArea,
				ViewType.FullStackedArea,
				ViewType.StepArea,
				ViewType.SplineArea,
				ViewType.StackedSplineArea,
				ViewType.FullStackedSplineArea
			});
			galleryGroups.Add("Area Series 3D", new List<ViewType>() {
				ViewType.Area3D,
				ViewType.StackedArea3D,
				ViewType.FullStackedArea3D,
				ViewType.StepArea3D,
				ViewType.SplineArea3D,
				ViewType.StackedSplineArea3D,
				ViewType.FullStackedSplineArea3D
			});
			galleryGroups.Add("Range Series", new List<ViewType>() {
				ViewType.RangeBar,
				ViewType.SideBySideRangeBar,
				ViewType.RangeArea,
				ViewType.RangeArea3D
			});
			galleryGroups.Add("Pie Series", new List<ViewType>() {
				ViewType.Pie,
				ViewType.Doughnut,
				ViewType.NestedDoughnut,
				ViewType.Pie3D,
				ViewType.Doughnut3D
			});
			galleryGroups.Add("Funnel Series", new List<ViewType>() {
				ViewType.Funnel,
				ViewType.Funnel3D
			});
			galleryGroups.Add("Radar Series", new List<ViewType>() {
				ViewType.RadarPoint,
				ViewType.RadarLine,
				ViewType.ScatterRadarLine,
				ViewType.RadarArea
			});
			galleryGroups.Add("Polar Series", new List<ViewType>() {
				ViewType.PolarPoint,
				ViewType.PolarLine,
				ViewType.ScatterPolarLine,
				ViewType.PolarArea
			});
			galleryGroups.Add("Financial Series", new List<ViewType>() {
				ViewType.Stock,
				ViewType.CandleStick
			});
			galleryGroups.Add("Gantt Series", new List<ViewType>() {
				ViewType.Gantt,
				ViewType.SideBySideGantt
			});
			foreach (KeyValuePair<string, List<ViewType>> pair in galleryGroups) {
				GalleryItemGroup galleryGroup = new GalleryItemGroup();
				galleryGroup.Caption = pair.Key;
				foreach (ViewType viewType in pair.Value) {
					string caption = SeriesViewFactory.GetStringID(viewType);
					GalleryItem galleryItem = new GalleryItem(ImageResourcesUtils.GetImageFromResources(viewType, false), null, caption, "", 0, 0, viewType, caption);
					galleryGroup.Items.Add(galleryItem);
				}
				galleryControl.Gallery.Groups.Add(galleryGroup);
			}
		}
	}
}
