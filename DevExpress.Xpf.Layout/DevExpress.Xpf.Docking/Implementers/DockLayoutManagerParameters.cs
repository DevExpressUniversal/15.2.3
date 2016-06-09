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

using System.Windows;
using DevExpress.Xpf.Docking.Base;
using SC = System.Collections;
namespace DevExpress.Xpf.Docking {
	public static class DockLayoutManagerParameters {
		static SC.Hashtable valueCache;
		static DockLayoutManagerParameters() {
			valueCache = new SC.Hashtable();
			valueCache[DockLayoutManagerParameter.DockingItemIntervalHorz] = 4.0;
			valueCache[DockLayoutManagerParameter.DockingItemIntervalVert] = 4.0;
			valueCache[DockLayoutManagerParameter.DockingRootMargin] = new Thickness(2.0);
			valueCache[DockLayoutManagerParameter.LayoutItemIntervalHorz] = 16.0;
			valueCache[DockLayoutManagerParameter.LayoutItemIntervalVert] = 4.0;
			valueCache[DockLayoutManagerParameter.LayoutGroupIntervalHorz] = 12.0;
			valueCache[DockLayoutManagerParameter.LayoutGroupIntervalVert] = 12.0;
			valueCache[DockLayoutManagerParameter.LayoutRootMargin] = new Thickness(12.0);
			valueCache[DockLayoutManagerParameter.CaptionToControlDistanceLeft] = 6.0;
			valueCache[DockLayoutManagerParameter.CaptionToControlDistanceTop] = 4.0;
			valueCache[DockLayoutManagerParameter.CaptionToControlDistanceRight] = 6.0;
			valueCache[DockLayoutManagerParameter.CaptionToControlDistanceBottom] = 4.0;
			valueCache[DockLayoutManagerParameter.LayoutPanelCaptionFormat] = DockingLocalizer.GetString(DockingStringId.LayoutPanelCaptionFormat);
			valueCache[DockLayoutManagerParameter.LayoutGroupCaptionFormat] = DockingLocalizer.GetString(DockingStringId.LayoutGroupCaptionFormat);
			valueCache[DockLayoutManagerParameter.LayoutControlItemCaptionFormat] = DockingLocalizer.GetString(DockingStringId.LayoutControlItemCaptionFormat);
			valueCache[DockLayoutManagerParameter.TabCaptionFormat] = DockingLocalizer.GetString(DockingStringId.TabCaptionFormat);
			valueCache[DockLayoutManagerParameter.WindowTitleFormat] = DockingLocalizer.GetString(DockingStringId.WindowTitleFormat);
			valueCache[DockLayoutManagerParameter.AutoHidePanelsFitToContainer] = true;
		}
		public static double DockingItemIntervalHorz {
			get { return (double)valueCache[DockLayoutManagerParameter.DockingItemIntervalHorz]; }
			set { valueCache[DockLayoutManagerParameter.DockingItemIntervalHorz] = value; }
		}
		public static double DockingItemIntervalVert {
			get { return (double)valueCache[DockLayoutManagerParameter.DockingItemIntervalVert]; }
			set { valueCache[DockLayoutManagerParameter.DockingItemIntervalVert] = value; }
		}
		public static Thickness DockingRootMargin {
			get { return (Thickness)valueCache[DockLayoutManagerParameter.DockingRootMargin]; }
			set { valueCache[DockLayoutManagerParameter.DockingRootMargin] = value; }
		}
		public static double LayoutItemIntervalHorz {
			get { return (double)valueCache[DockLayoutManagerParameter.LayoutItemIntervalHorz]; }
			set { valueCache[DockLayoutManagerParameter.LayoutItemIntervalHorz] = value; }
		}
		public static double LayoutItemIntervalVert {
			get { return (double)valueCache[DockLayoutManagerParameter.LayoutItemIntervalVert]; }
			set { valueCache[DockLayoutManagerParameter.LayoutItemIntervalVert] = value; }
		}
		public static double LayoutGroupIntervalHorz {
			get { return (double)valueCache[DockLayoutManagerParameter.LayoutGroupIntervalHorz]; }
			set { valueCache[DockLayoutManagerParameter.LayoutGroupIntervalHorz] = value; }
		}
		public static double LayoutGroupIntervalVert {
			get { return (double)valueCache[DockLayoutManagerParameter.LayoutGroupIntervalVert]; }
			set { valueCache[DockLayoutManagerParameter.LayoutGroupIntervalVert] = value; }
		}
		public static Thickness LayoutRootMargin {
			get { return (Thickness)valueCache[DockLayoutManagerParameter.LayoutRootMargin]; }
			set { valueCache[DockLayoutManagerParameter.LayoutRootMargin] = value; }
		}
		public static double CaptionToControlDistanceLeft {
			get { return (double)valueCache[DockLayoutManagerParameter.CaptionToControlDistanceLeft]; }
			set { valueCache[DockLayoutManagerParameter.CaptionToControlDistanceLeft] = value; }
		}
		public static double CaptionToControlDistanceTop {
			get { return (double)valueCache[DockLayoutManagerParameter.CaptionToControlDistanceTop]; }
			set { valueCache[DockLayoutManagerParameter.CaptionToControlDistanceTop] = value; }
		}
		public static double CaptionToControlDistanceRight {
			get { return (double)valueCache[DockLayoutManagerParameter.CaptionToControlDistanceRight]; }
			set { valueCache[DockLayoutManagerParameter.CaptionToControlDistanceRight] = value; }
		}
		public static double CaptionToControlDistanceBottom {
			get { return (double)valueCache[DockLayoutManagerParameter.CaptionToControlDistanceBottom]; }
			set { valueCache[DockLayoutManagerParameter.CaptionToControlDistanceBottom] = value; }
		}
		public static string LayoutPanelCaptionFormat {
			get { return (string)valueCache[DockLayoutManagerParameter.LayoutPanelCaptionFormat]; }
			set { valueCache[DockLayoutManagerParameter.LayoutPanelCaptionFormat] = value; }
		}
		public static string LayoutGroupCaptionFormat {
			get { return (string)valueCache[DockLayoutManagerParameter.LayoutGroupCaptionFormat]; }
			set { valueCache[DockLayoutManagerParameter.LayoutGroupCaptionFormat] = value; }
		}
		public static string LayoutControlItemCaptionFormat {
			get { return (string)valueCache[DockLayoutManagerParameter.LayoutControlItemCaptionFormat]; }
			set { valueCache[DockLayoutManagerParameter.LayoutControlItemCaptionFormat] = value; }
		}
		public static string TabCaptionFormat {
			get { return (string)valueCache[DockLayoutManagerParameter.TabCaptionFormat]; }
			set { valueCache[DockLayoutManagerParameter.TabCaptionFormat] = value; }
		}
		public static string WindowTitleFormat {
			get { return (string)valueCache[DockLayoutManagerParameter.WindowTitleFormat]; }
			set { valueCache[DockLayoutManagerParameter.WindowTitleFormat] = value; }
		}
		public static bool AutoHidePanelsFitToContainer {
			get { return (bool)valueCache[DockLayoutManagerParameter.AutoHidePanelsFitToContainer]; }
			set { valueCache[DockLayoutManagerParameter.AutoHidePanelsFitToContainer] = value; }
		}
	}
}
