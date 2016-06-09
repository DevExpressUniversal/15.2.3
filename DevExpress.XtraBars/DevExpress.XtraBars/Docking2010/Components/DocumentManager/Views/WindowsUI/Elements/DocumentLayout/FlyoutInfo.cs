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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Customization;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IFlyoutInfo :
		IDocumentContainerInfo {
	}
	internal class FlyoutInfo : BaseContentContainerInfo, IFlyoutInfo {
		IDocumentInfo documentInfoCore;
		public IDocumentInfo DocumentInfo {
			get { return documentInfoCore; }
		}
		protected new FlyoutInfoPainter Painter {
			get { return base.Painter as FlyoutInfoPainter; }
		}
		public FlyoutInfo(WindowsUIView view, Flyout flyout)
			: base(view, flyout) {
			documentInfoCore = CreateDocumentInfo(flyout.Document);
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			DocumentInfo.Calc(g, content);
		}
		protected IDocumentInfo CreateDocumentInfo(Document document) {
			return new FlyoutDocumentInfo(Owner, document, Container as IFlyoutContainer); ;
		}
		protected override IContentContainerHeaderInfo CreateHeaderInfo() {
			return new FlyoutInfo.FlyoutHeaderInfo(Owner, this); 
		}
		public override Type GetUIElementKey() {
			return typeof(IFlyoutInfo);
		}
		protected override void OnShown() {
			if((Owner.Manager != null) && (DocumentInfo != null)) {
				Owner.Manager.InvokePatchActiveChildren();
			}
		}
		class FlyoutHeaderInfo : ContentContainerHeaderInfo, IContentContainerHeaderInfo, IBaseElementInfo, IDisposable, IUIElement, IStringImageProvider, IInteractiveElementInfo {
			public new IFlyoutInfo ContainerInfo {
				get { return ContainerInfo as IFlyoutInfo; }
			}
			public FlyoutHeaderInfo(WindowsUIView view, IFlyoutInfo containerInfo)
				: base(view, containerInfo) {
			}
			protected override void CalcElements(Point offset, Size textSize, Size buttonSize, Size contentSize, int textOffset) { }
			protected override Size CalcMinContentSize(GraphicsCache cache, Size textSize, Size buttonSize, int interval) {
				return Size.Empty;
			}
			protected override void CalcTitleBounds(GraphicsCache cache, Rectangle headerRect, Size textSize, Size buttonSize) { }
			Size DevExpress.XtraBars.Docking2010.Views.WindowsUI.IContentContainerHeaderInfo.CalcMinSize(Graphics g, Rectangle bounds) {
				return Size.Empty;
			}
		}
	}
	internal class FlyoutInfoPainter : ContentContainerInfoPainter {
		public FlyoutInfoPainter() { }
		public override void DrawCaption(ObjectInfoArgs e, string caption, Font font, Brush brush, Rectangle bounds, StringFormat format) { }
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) { }
		protected override void DrawHeader(GraphicsCache cache, IContentContainerHeaderInfo info) { }
	}
	internal class FlyoutInfoSkinPainter : FlyoutInfoPainter {
		ISkinProvider providerCore;
		public FlyoutInfoSkinPainter(ISkinProvider provider) {
			providerCore = provider;
		}
		public override Padding GetContentMargins() {
			SkinElement page = GetPageElement();
			if(page != null) {
				SkinPaddingEdges edges = page.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual SkinElement GetPageElement() {
			return GetSkin()[MetroUISkins.SkinPage];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore); ;
		}
	}
	internal class FlyoutDocumentInfo : DocumentInfo, IDocumentInfo {
		private IFlyoutContainer flyoutCore;
		public FlyoutDocumentInfo(WindowsUIView owner, Document document)
			: base(owner, document) {
		}
		public FlyoutDocumentInfo(WindowsUIView owner, Document document, IFlyoutContainer flyout)
			: base(owner, document) {
			flyoutCore = flyout;
		}
		void IDocumentInfo.PatchChild(Rectangle view, bool setActive) {
			if(base.Document == null) return;
			Control child = base.Owner.Manager.GetChild(base.Document);
			if(child != null) {
				ContentAlignment flyoutAlignment = ContentAlignment.MiddleCenter;
				if(flyoutCore != null) {
					if(flyoutCore.FlyoutProperties.ActualStyle == FlyoutStyle.Popup)
						flyoutAlignment = flyoutCore.FlyoutProperties.ActualAlignment;
				}
				if(setActive) {
					Size childSize = base.Document.InitializedControlSize;
					BaseView.PatchMaximized(child);
					child.BringToFront();
					child.Size = childSize;
					child.Bounds = DevExpress.Utils.PlacementHelper.Arrange(childSize, base.Client, flyoutAlignment);
				}
				else {
					child.Bounds = new Rectangle(view.X - base.Client.Width, view.Y - base.Client.Height, 100, 100);
				}
			}
		}
	}
}
