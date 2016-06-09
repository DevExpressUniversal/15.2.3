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

using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Design {
	public class WindowsUIViewDesigner : BaseDocumentManagerDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			CreateDefaultMainGroup();
			CreateDefaultElementsGroup();
			CreateDefaultLayoutGroup();
			CreateDefaultStyleGroup();
		}
		protected override DesignerGroup CreateDefaultMainGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, "Main", "Main DocumentManager settings (adjust the view and other properties).", GetDefaultGroupImage(0), true);
			CreateViewsFrame(group);
			return group;
		}
		protected override DesignerGroup CreateDefaultLayoutGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Layout, "Layout", "WindowsUIView layout settings.", GetDefaultGroupImage(2), true);
			CreateNavigationTreeFrame(group);
			CreateNavigationLayout(group);
			return group;
		}
		protected virtual DesignerGroup CreateDefaultElementsGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Regular, "Elements", "Adjust the settings of the WindowsUIView elements or create new elements.", GetDefaultGroupImage(1), true);
			CreateDocumentsFrame(group);
			CreateTilesFrame(group);
			CreateContentContainerFrame(group);
			return group;
		}
		protected virtual void CreateNavigationTreeFrame(DesignerGroup group) {
			group.Add("Navigation Tree", "Adjust the structure of Application Navigation Tree and assign tiles and documents with content containers.",
				typeof(Frames.NavigationTreeFrame), GetDefaultLargeImage(6), GetDefaultSmallImage(6), null);
		}
		protected virtual void CreateNavigationLayout(DesignerGroup group) {
			group.Add("Layout", "Customize the current view's layout.",
				typeof(Frames.WindowsUIViewLayoutFrame), GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
		}
		protected virtual void CreateTilesFrame(DesignerGroup group) {
			group.Add("Tiles", "Adjust the settings of the tiles and create tiles for all documents.", 
				typeof(Frames.TileFrame), GetDefaultLargeImage(3), GetDefaultSmallImage(3), null);
		}
		protected virtual void CreateContentContainerFrame(DesignerGroup group) {
			group.Add("Content Containers", "Adjust the settings of the content containers or create new content containers.",
				typeof(Frames.ContentContainersFrame), GetDefaultLargeImage(5), GetDefaultSmallImage(5), null);
		}
	}
}
