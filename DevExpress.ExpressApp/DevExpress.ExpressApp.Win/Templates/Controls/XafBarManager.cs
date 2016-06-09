#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Styles;
using DevExpress.XtraBars.ViewInfo;
namespace DevExpress.ExpressApp.Win.Templates.Controls {
	[Designer("DevExpress.ExpressApp.Win.Design.XafBarManagerDesigner, DevExpress.ExpressApp.Win.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[ToolboxItem(false)] 
	[DesignerCategory("Component")]
	public class XafBarManager : BarManager, IModelSynchronizable {
		private BarMerger merger;
		private IModelTemplateXtraBarsCustomization model;
		internal XtraBarsModelSynchronizer modelSynchronizer;
		private static bool useBarsModel = false;
		public XafBarManager()
			: base() {
			Init();
		}
		public XafBarManager(IContainer container)
			: base(container) {
			Init();
		}
		private static int mainMenuItemImageIndex;
		private static int actionContainerMenuBarItemImageIndex;
		private static int actionContainerBarItemImageIndex;
		private static int xafBarLinkContainerItemImageIndex;
		protected override void FillAdditionalBarItemInfoCollection(BarItemInfoCollection coll) {
			coll.Add(new BarItemInfo("ActionContainerBarItem", "XAF Action Container", XafBarManager.ActionContainerBarItemImageIndex, typeof(ActionContainerBarItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new DevExpress.XtraBars.Painters.BarCustomContainerLinkPainter(this.PaintStyle), true, false));
			coll.Add(new BarItemInfo("ActionContainerMenuBarItem", "XAF Menu Action Container", XafBarManager.ActionContainerMenuBarItemImageIndex, typeof(ActionContainerMenuBarItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new DevExpress.XtraBars.Painters.BarCustomContainerLinkPainter(this.PaintStyle), true, false));
			coll.Add(new BarItemInfo("XafBarLinkContainerItem", "XAF LinkContainer", XafBarManager.XafBarLinkContainerItemImageIndex, typeof(XafBarLinkContainerItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new DevExpress.XtraBars.Painters.BarCustomContainerLinkPainter(this.PaintStyle), true, false));
			coll.Add(new BarItemInfo("MainMenuItem", "XAF Menu", XafBarManager.MainMenuItemImageIndex, typeof(MainMenuItem), typeof(BarLinkContainerExItemLink), typeof(BarLinkContainerLinkViewInfo), new DevExpress.XtraBars.Painters.BarCustomContainerLinkPainter(this.PaintStyle), true, false));
		}
		public override object Images {
			get { return base.Images; }
			set { base.Images = value; }
		}
		private int AddImage(string imageName) {
			int result = 0;
			Image image = ImageLoader.Instance.GetImageInfo(imageName).Image;
			if(!BarItemsImages.Images.Contains(image)) {
				BarItemsImages.AddImage(image, imageName);
				result = BarItemsImages.Images.Count - 1;
			}
			else {
				result = BarItemsImages.Images.IndexOf(image);
			}
			return result;
		}
		private void Init() {
			if(!ImageLoader.IsInitialized) {
				ImageLoader.Init(new AssemblyResourceImageSource(GetType().Assembly.FullName));
			}
			if(merger == null) {
				merger = new BarMerger();
			}
			mainMenuItemImageIndex = AddImage("MainMenuItem");
			actionContainerMenuBarItemImageIndex = AddImage("ActionContainerMenuBarItem");
			actionContainerBarItemImageIndex = AddImage("ActionContainerBarItem");
			xafBarLinkContainerItemImageIndex = AddImage("XafBarLinkContainerItem");			
		}
		private void DisposeModelSynchronizer() {
			if(modelSynchronizer != null) {
				modelSynchronizer.Dispose();
				modelSynchronizer = null;
			}
		}
		protected override void RaiseMerge(BarManagerMergeEventArgs e) {
			base.RaiseMerge(e);
			merger.MergeBars(this, e.ChildManager);
		}
		protected override void RaiseUnMerge(BarManagerMergeEventArgs e) {
			base.RaiseUnMerge(e);
			merger.UnMergeBars(this, e.ChildManager);
		}
		protected override bool AllowHotCustomization { get { return false; } }
		[Browsable(false)]
		public static int MainMenuItemImageIndex {
			get { return mainMenuItemImageIndex; }
		}
		[Browsable(false)]
		public static int ActionContainerMenuBarItemImageIndex {
			get { return actionContainerMenuBarItemImageIndex; }
		}
		[Browsable(false)]
		public static int ActionContainerBarItemImageIndex {
			get { return actionContainerBarItemImageIndex; }
		}
		[Browsable(false)]
		public static int XafBarLinkContainerItemImageIndex {
			get { return xafBarLinkContainerItemImageIndex; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IModelTemplateXtraBarsCustomization Model {
			get { return model; }
			set {
				model = value;
				DisposeModelSynchronizer();
				if(model != null) {
					modelSynchronizer = new XtraBarsModelSynchronizer(this, model);
				}
			}
		}		
		#region IModelSynchronizable Members
		public void ApplyModel() {			
			if(modelSynchronizer != null && useBarsModel) {
				modelSynchronizer.ApplyModel();
			}
		}
		public void SynchronizeModel() {
			if(modelSynchronizer != null && useBarsModel) {
				modelSynchronizer.SynchronizeModel();
			}
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeModelSynchronizer();
			}
			base.Dispose(disposing);
		}
		public static bool UseBarsModel {
			get { return useBarsModel; }
			set { useBarsModel = value; }
		}
	}
}
