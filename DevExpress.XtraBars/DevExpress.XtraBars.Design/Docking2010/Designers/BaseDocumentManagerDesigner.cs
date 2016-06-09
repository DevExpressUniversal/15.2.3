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

using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Design {
	public enum DesignerGroupType { Regular, Main, Appearance, Layout }
	public class DocumentManagerDesignerGroupCollection : DesignerGroupCollection {
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
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		protected override void OnRemoveComplete(int index, object item) {
			DesignerGroup group = item as DesignerGroup;
			if(group != null) group.Dispose();
		}
	}
	public class BaseDocumentManagerDesigner : BaseDesigner {
		static ImageCollection largeImages;
		static ImageCollection smallImages;
		static ImageCollection groupImages;
		static BaseDocumentManagerDesigner() {
			largeImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Docking2010.Images.icons32x32.png",
				typeof(BaseDocumentManagerDesigner).Assembly, new Size(32, 32));
			smallImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Docking2010.Images.icons16x16.png",
				typeof(BaseDocumentManagerDesigner).Assembly, new Size(16, 16));
			groupImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraBars.Design.Docking2010.Images.navBarGroupIcons16x16.png",
				typeof(BaseDocumentManagerDesigner).Assembly, new Size(16, 16));
		}
		protected override object LargeImageList { get { return largeImages; } }
		protected override object SmallImageList { get { return smallImages; } }
		protected override object GroupImageList { get { return groupImages; } }
		public new DocumentManagerDesignerGroupCollection Groups { 
			get { return base.Groups as DocumentManagerDesignerGroupCollection; } 
		}
		protected override DesignerGroupCollection CreateGroupCollection() {
			return new DocumentManagerDesignerGroupCollection();
		}
		protected virtual DesignerGroup CreateDefaultMainGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, "Main", "Main DocumentManager settings (adjust the view and other properties).", GetDefaultGroupImage(0), true);
			CreateViewsFrame(group);
			CreateDocumentsFrame(group);
			return group;
		}
		protected virtual DesignerGroup CreateDefaultLayoutGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Layout, "Layout", "DocumentManager layout settings (adjust the view layout).", GetDefaultGroupImage(2), true);
			CreateLayoutFrame(group);
			return group;
		}
		protected virtual DesignerGroup CreateDefaultStyleGroup() {
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Appearance, "Appearance", "Adjust the appearance of the current view.", GetDefaultGroupImage(3), true);
			CreateAppearancesFrame(group);
			return group;
		}
		protected virtual void CreateViewsFrame(DesignerGroup group) {
			group.Add("Views", "Adjust the settings of the views and add new views.", typeof(Frames.MainFrame), GetDefaultLargeImage(0), GetDefaultSmallImage(0), true);
		}
		protected virtual void CreateLayoutFrame(DesignerGroup group) {
			group.Add("Layout", "Customize the current view's layout.", typeof(Frames.LayoutFrame), GetDefaultLargeImage(4), GetDefaultSmallImage(4), null);
		}
		protected virtual void CreateAppearancesFrame(DesignerGroup group) {
			group.Add("Appearances", "Manage the appearances for the current view.", typeof(Frames.AppearancesFrame), GetDefaultLargeImage(1), GetDefaultSmallImage(1), false);
		}
		protected virtual void CreateDocumentsFrame(DesignerGroup group) {
			group.Add("Documents", "Adjust the settings of the documents and add new documents.", typeof(Frames.DocumentsFrame), GetDefaultLargeImage(2), GetDefaultSmallImage(2), null);
		}
	}
	public class EmptyViewDesigner : BaseDocumentManagerDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			CreateDefaultMainGroup();
			CreateDefaultLayoutGroup();
			CreateDefaultStyleGroup();
		}
	}
	public class DefaultDesigner : BaseDocumentManagerDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			DesignerGroup group = Groups.AddOrClear(DesignerGroupType.Main, "Main", "Main DocumentManager settings (adjust the view and other properties).", GetDefaultGroupImage(0), true);
			  group.Add("Views", "Adjust the settings of the views and add new views.", typeof(Frames.MainFrame), GetDefaultLargeImage(0), GetDefaultSmallImage(0), true);
		}
	}
}
