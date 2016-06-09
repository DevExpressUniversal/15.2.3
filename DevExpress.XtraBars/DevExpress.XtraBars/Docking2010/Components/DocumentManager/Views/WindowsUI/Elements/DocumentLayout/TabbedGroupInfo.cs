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
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ITabbedGroupInfo :
		IDocumentSelectorInfo<TabbedGroup> {
		bool IsTabHeaders { get; }
	}
	class TabbedGroupInfo : DocumentSelectorInfo<TabbedGroup>, ITabbedGroupInfo {
		public TabbedGroupInfo(WindowsUIView view, TabbedGroup group)
			: base(view, group) {
		}
		protected override ButtonsPanel CreateButtonsPanel() {
			return new TabbedGroupButtonsPanel(this);
		}
		protected override ObjectPainter GetButtonsPanelPainter() {
			return IsTabHeaders ?
				((WindowsUIViewPainter)Owner.Painter).GetTabbedGroupButtonsPanelTabPainter() :
				((WindowsUIViewPainter)Owner.Painter).GetTabbedGroupButtonsPanelTilePainter();
		}
		protected override bool AllowShowDocumentSelectorButton { get { return IsTabHeaders; } }
		public override Type GetUIElementKey() {
			return typeof(ITabbedGroupInfo);
		}
		protected override bool GetShowPageHeadersCore() {
			return true;
		}
		protected override bool GetButtonsPanelWrap() {
			return !IsTabHeaders;
		}
		public bool IsTabHeaders {
			get { return Group.Properties.ActualHeaderStyle != HeaderStyle.Tile; }
		}
		protected override bool GetIsButtonsPanelInverseOrder() {
			return false;
		}
		protected override System.Windows.Forms.Orientation GetButtonsPanelOrientation() {
			return IsTabHeaders ? System.Windows.Forms.Orientation.Vertical : System.Windows.Forms.Orientation.Horizontal;
		}
		protected override ContentAlignment GetButtonsPanelContentAlignment() {
			return ContentAlignment.TopLeft;
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			Size headersSize = ButtonsPanel.ViewInfo.CalcMinSize(g);
			Size headersAreaSize = Painter.GetHeadersSizeByContentSize(headersSize);
			Rectangle headersArea = Painter.GetHeadersBoundsByContentRectangle(
					new Rectangle(0, 0,
							Math.Min(content.Width - (headersAreaSize.Width - headersSize.Width), headersSize.Width),
							Math.Min(content.Height - (headersAreaSize.Height - headersSize.Height), headersSize.Height)
						));
			Rectangle headers = new Rectangle(
				content.Left - headersArea.Left,
				content.Top - headersArea.Top,
				headersSize.Width,
				content.Height - (headersAreaSize.Height - headersSize.Height));
			CalcButtonsPanel(g, headers);
			Rectangle pageContent = new Rectangle(
				content.Left + headersArea.Width, content.Top, content.Width - headersArea.Width, content.Height);
			base.CalcContent(g, pageContent);
		}
		protected override Rectangle CalcDocumentSelectorButtonCore(Graphics g, Size buttonsPanelMinSize, Rectangle headersRect) {
			if(buttonsPanelMinSize.IsEmpty || headersRect.IsEmpty) return Rectangle.Empty;
			Size minSize = DocumentSelectorButton.Info.CalcMinSize(g, DocumentSelectorButton.Painter);
			Point location = new Point(ButtonsPanel.Bounds.X, ButtonsPanel.Bounds.Bottom);
			Size size = new Size(buttonsPanelMinSize.Width, minSize.Height);
			return new Rectangle(location, size);
		}
		protected override Point CalcDocumentSelectorFlyoutPanelLocation(Size documentSelectorMenuSize) {
			return new Point(DocumentSelectorButton.Info.Bounds.Right, DocumentSelectorButton.Info.Bounds.Bottom - documentSelectorMenuSize.Height);
		}
		protected override IContentContainerHeaderInfo CreateHeaderInfo() {
			return new TabbedGroupHeaderInfo(Owner, this);
		}
		class TabbedGroupHeaderInfo : DocumentSelectorHeaderInfo {
			public TabbedGroupHeaderInfo(WindowsUIView view, ITabbedGroupInfo containerInfo)
				: base(view, containerInfo) {
			}
			public new ITabbedGroupInfo ContainerInfo {
				get { return base.ContainerInfo as ITabbedGroupInfo; }
			}
		}
	}
	class TabbedGroupInfoPainter : DocumentSelectorInfoPainter<TabbedGroup> {
		public override int ButtonsInterval {
			get { return 0; }
		}
	}
	class TabbedGroupInfoSkinPainter : DocumentSelectorInfoSkinPainter<TabbedGroup> {
		public TabbedGroupInfoSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override int ButtonsInterval {
			get {
				object interval = GetButtonsIntervalProperty();
				if(interval != null)
					return (int)interval;
				return 0;
			}
		}
		public override Rectangle GetHeadersBoundsByContentRectangle(Rectangle headers) {
			return Rectangle.Inflate(headers, 20, 0);
		}
		protected override object GetButtonsIntervalProperty() {
			return GetSkin().Properties[MetroUISkins.TabbedGroupButtonsInterval];
		}
		protected override SkinElement GetDocumentSelectorElement() {
			return GetSkin()[MetroUISkins.SkinTabbedGroup];
		}
	}
	class TabbedGroupHeaderInfoPainter : ContentContainerHeaderInfoPainter { }
	class TabbedGroupHeaderInfoSkinPainter : ContentContainerHeaderInfoSkinPainter {
		public TabbedGroupHeaderInfoSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetItemHeader() {
			return GetSkin()[MetroUISkins.SkinTabbedGroupItemHeader];
		}
	}
}
