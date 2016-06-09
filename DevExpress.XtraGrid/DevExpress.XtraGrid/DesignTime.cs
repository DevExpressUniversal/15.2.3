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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid;
namespace DevExpress.XtraGrid.Design { 
	public enum DesignerGroupType { Regular, Main, Repository, Appearance, Printing, Layout }
	public class GridDesignerGroupCollection : DesignerGroupCollection {
		public DesignerGroup AddOrClear(DesignerGroupType gtype, string caption, string description, Image image) {
			return AddOrClear(gtype, caption, description, image, false);
		}
		public DesignerGroup AddOrClear(DesignerGroupType gtype, string caption, string description, Image image, bool defaultExpanded) {
			DesignerGroup group = null;
			if(gtype != DesignerGroupType.Regular) {
				group = GetGroupByType(gtype);
				if(group != null) group.Clear();
			}
			if(group == null) group = Add(caption, description, image, defaultExpanded);
			group.Tag = gtype;
			return group;
		}
		public DesignerGroup GetGroupByType(DesignerGroupType gtype) {
			foreach(DesignerGroup group in this) {
				if(group.Tag == null) continue;
				if((DesignerGroupType)group.Tag == gtype) return group;
			}
			return null;
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n --) RemoveAt(n);
		}
		protected override void OnRemoveComplete(int index, object item) {
			DesignerGroup group = item as DesignerGroup;
			if(group != null) group.Dispose();
		}
	}
	public class BaseGridDesigner : BaseDesigner {
		[ThreadStatic]
		static ImageCollection largeImages = null;
		[ThreadStatic]
		static ImageCollection smallImages = null;
		[ThreadStatic]
		static ImageCollection groupImages = null;
		static ImageCollection LargeImages { 
			get { 
				if(largeImages == null) 
					largeImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.icons32x32.png", typeof(BaseGridDesigner).Assembly, new Size(32, 32));
				return largeImages; 
			} 
		}
		static ImageCollection SmallImages { 
			get { 
				if(smallImages == null) 
					smallImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.icons16x16.png", typeof(BaseGridDesigner).Assembly, new Size(16, 16));
				return smallImages; 
			} 
		}
		static ImageCollection GroupImages {
			get {
				if(groupImages == null)
					groupImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Images.navBarGroupIcons16x16.png", typeof(BaseGridDesigner).Assembly, new Size(16, 16));
				return groupImages;
			}
		}
		public new GridDesignerGroupCollection Groups { get { return base.Groups as GridDesignerGroupCollection; } }
		protected override object LargeImageList { get { return LargeImages; } }
		protected override object SmallImageList { get { return SmallImages; } }
		protected override object GroupImageList { get { return GroupImages; } }
		protected override DesignerGroupCollection CreateGroupCollection() {
			return new GridDesignerGroupCollection();
		}
		protected virtual DesignerGroup CreateDefaultMainGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, Properties.Resources.GroupMainCaption, Properties.Resources.GroupMainDescription, GetDefaultGroupImage(0), true);
			group.Add(Properties.Resources.ItemViewsCaption, Properties.Resources.ItemViewsDescription, "DevExpress.XtraGrid.Frames.LevelStyle", GetDefaultLargeImage(0), GetDefaultSmallImage(0), true);
			group.Add(Properties.Resources.ItemFeatureBrowserCaption, Properties.Resources.ItemFeatureBrowserDescription, "DevExpress.XtraGrid.FeatureBrowser.FeatureBrowserGridMainFrame", GetDefaultLargeImage(3), GetDefaultSmallImage(3));
			group.Add(Properties.Resources.ItemLayoutCaption, Properties.Resources.ItemLayoutDescription, "DevExpress.XtraGrid.Frames.Layouts", GetDefaultLargeImage(4), GetDefaultSmallImage(4), null, false);
			return group;
		}
		protected virtual DesignerGroup CreateDefaultRepositoryGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Repository, Properties.Resources.GroupRepositoryCaption, Properties.Resources.GroupRepositoryDescription, GetDefaultGroupImage(2), true);
			group.Add(Properties.Resources.ItemViewRepositoryCaption, Properties.Resources.ItemViewRepositoryDescription, "DevExpress.XtraGrid.Frames.ViewsEditor", GetDefaultLargeImage(6), GetDefaultSmallImage(6), null, false);
			group.Add(Properties.Resources.ItemEditorRepositoryCaption, Properties.Resources.ItemEditorRepositoryDescription, "DevExpress.XtraGrid.Frames.PersistentRepositoryGridEditor", GetDefaultLargeImage(7), GetDefaultSmallImage(7), true, false);
			return group;
		}
		protected virtual DesignerGroup CreateDefaultStyleGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Appearance, Properties.Resources.GroupAppearanceCaption, Properties.Resources.GroupAppearanceDescription, GetDefaultGroupImage(1), true);
			group.Add(Properties.Resources.ItemAppearancesCaption, Properties.Resources.ItemAppearancesDescription, "DevExpress.XtraGrid.Frames.AppearancesDesigner", GetDefaultLargeImage(8), GetDefaultSmallImage(8), null, false);
			group.Add(Properties.Resources.ItemFormatConditionsCaption, Properties.Resources.ItemFormatConditionsDescription, "DevExpress.XtraGrid.Frames.StyleFormatConditionFrame", GetDefaultLargeImage(9), GetDefaultSmallImage(9), null, false);
			group.Add(Properties.Resources.ItemStyleSchemesCaption, Properties.Resources.ItemStyleSchemesDescription, "DevExpress.XtraGrid.Frames.SchemeDesigner", GetDefaultLargeImage(10), GetDefaultSmallImage(10), null, false);
			return group;
		}
		protected virtual DesignerGroup CreateDefaultPrintingGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Printing, Properties.Resources.GroupPrintingCaption, Properties.Resources.GroupPrintingDescription, GetDefaultGroupImage(3));
			group.Add(Properties.Resources.ItemPrintAppearancesCaption, Properties.Resources.ItemPrintAppearancesDescription, "DevExpress.XtraGrid.Frames.PrintAppearancesDesigner", GetDefaultLargeImage(12), GetDefaultSmallImage(12), null, false);
			return group;
		}
	}
	public class GridViewDesigner : BaseGridDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			DesignerGroup group = CreateDefaultMainGroup();
			group.Insert(1, Properties.Resources.ItemColumnsCaption, Properties.Resources.ItemColumnsDescription, "DevExpress.XtraGrid.Frames.ColumnDesigner", GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			group.Add(Properties.Resources.ItemGroupSummaryCaption, Properties.Resources.ItemGroupSummaryDescription, "DevExpress.XtraGrid.Frames.GroupSummary", GetDefaultLargeImage(5), GetDefaultSmallImage(5), null, false);
			group.Add("EditForm Designer", "EditForm Description", "DevExpress.XtraGrid.Frames.EditFormDesigner", GetDefaultLargeImage(13), GetDefaultSmallImage(13), null, false);
			CreateDefaultStyleGroup();
			CreateDefaultRepositoryGroup();
			group = CreateDefaultPrintingGroup();
			group.Add(Properties.Resources.ItemPrintingSettingsCaption, Properties.Resources.ItemPrintingSettingsDescription, "DevExpress.XtraGrid.Frames.GridViewPrinting", GetDefaultLargeImage(11), GetDefaultSmallImage(11), null, false);
		}
	}
	public class BandedGridViewDesigner : GridViewDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			DesignerGroup group = Groups.GetGroupByType(DesignerGroupType.Main);
			if(group != null) {
				group.Insert(2, Properties.Resources.ItemBandsCaption, Properties.Resources.ItemBandsDescription, "DevExpress.XtraGrid.Frames.BandDesigner", GetDefaultLargeImage(2), GetDefaultSmallImage(2), null);
			}
			group = Groups.GetGroupByType(DesignerGroupType.Printing);
			if(group != null) {
				group.Clear();
				Groups.RemoveAt(Groups.IndexOf(group));
			}
			group = CreateDefaultPrintingGroup();
			group.Add(Properties.Resources.ItemPrintingSettingsCaption, Properties.Resources.ItemPrintingSettingsDescription, "DevExpress.XtraGrid.Frames.BandedViewPrinting", GetDefaultLargeImage(11), GetDefaultSmallImage(11), null, false);
		}
	}
	public class EmptyViewDesigner : BaseGridDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			CreateDefaultMainGroup();
			CreateDefaultStyleGroup();
		}
	}
	public class ControlGridDesigner : BaseGridDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, Properties.Resources.GroupMainCaption, Properties.Resources.GroupMainDescription, GetDefaultGroupImage(0), true);
			group.Add(Properties.Resources.ItemViewsCaption, Properties.Resources.ItemViewsDescription, "DevExpress.XtraGrid.Frames.LevelStyle", GetDefaultLargeImage(0), GetDefaultSmallImage(0), true);
		}
	}
	public class CardViewDesigner : BaseGridDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			DesignerGroup group = CreateDefaultMainGroup();
			group.Insert(1, Properties.Resources.ItemColumnsCaption, Properties.Resources.ItemColumnsCardDescription, "DevExpress.XtraGrid.Frames.ColumnDesigner", GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			CreateDefaultStyleGroup();
			CreateDefaultRepositoryGroup();
			group = CreateDefaultPrintingGroup();
			group.Add(Properties.Resources.ItemPrintingSettingsCaption, Properties.Resources.ItemPrintingSettingsDescription, "DevExpress.XtraGrid.Frames.CardViewPrinting", GetDefaultLargeImage(11), GetDefaultSmallImage(11));
		}
	}
	public class WinExplorerViewDesigner : BaseGridDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			DesignerGroup group = CreateDefaultMainGroup();
			CreateDefaultStyleGroup();
			CreateDefaultRepositoryGroup();
			group = CreateDefaultPrintingGroup();
			group.Add(Properties.Resources.ItemPrintingSettingsCaption, Properties.Resources.ItemPrintingSettingsDescription, "DevExpress.XtraGrid.Frames.GridViewPrinting", GetDefaultLargeImage(11), GetDefaultSmallImage(11), null, false);
		}
		protected override DesignerGroup CreateDefaultMainGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, Properties.Resources.GroupMainCaption, Properties.Resources.GroupMainDescription, GetDefaultGroupImage(0), true);
			group.Add(Properties.Resources.WinExplorerViewColumnsCaption, Properties.Resources.ItemColumnsDescription, "DevExpress.XtraGrid.Design.ListView.ListViewColumnDesigner", GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			group.Add(Properties.Resources.ItemViewsCaption, Properties.Resources.ItemViewsDescription, "DevExpress.XtraGrid.Frames.LevelStyle", GetDefaultLargeImage(0), GetDefaultSmallImage(0), true);
			group.Add(Properties.Resources.ItemFeatureBrowserCaption, Properties.Resources.ItemFeatureBrowserDescription, "DevExpress.XtraGrid.FeatureBrowser.FeatureBrowserGridMainFrame", GetDefaultLargeImage(3), GetDefaultSmallImage(3));
			group.Add(Properties.Resources.WinExplorerViewLayoutCaption, Properties.Resources.WinExplorerViewLayoutDescription, "DevExpress.XtraGrid.Design.ListView.ListViewLayout", GetDefaultLargeImage(4), GetDefaultSmallImage(4), null, false);
			return group;
		}
	}
}
