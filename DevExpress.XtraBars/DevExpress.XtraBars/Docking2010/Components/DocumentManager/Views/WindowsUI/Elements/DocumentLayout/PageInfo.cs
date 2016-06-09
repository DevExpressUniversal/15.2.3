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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IPageInfo :
		IDocumentContainerInfo {
	}
	class PageInfo : BaseContentContainerInfo, IPageInfo {
		IDocumentInfo documentInfoCore;
		public PageInfo(WindowsUIView view, Page page)
			: base(view, page) {
			documentInfoCore = CreateDocumentInfo(page.Document);
		}
		public override System.Type GetUIElementKey() {
			return typeof(IPageInfo);
		}
		public IDocumentInfo DocumentInfo {
			get { return documentInfoCore; }
		}
		protected IDocumentInfo CreateDocumentInfo(Document document) {
			return new DocumentInfo(Owner, document);
		}
		protected new PageInfoPainter Painter {
			get { return base.Painter as PageInfoPainter; }
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			Rectangle pageContent = CalcContentWithMargins(content);
			DocumentInfo.Calc(g, pageContent);
		}
		protected override void OnShown() {
			if(Owner.Manager != null && DocumentInfo != null)
				Owner.Manager.InvokePatchActiveChildren();
		}
	}
	class PageInfoPainter : ContentContainerInfoPainter {
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) {
		}
	}
	class PageInfoSkinPainter : PageInfoPainter {
		ISkinProvider providerCore;
		public PageInfoSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		public override Padding GetContentMargins() {
			SkinElement page = GetPageElement();
			if(page != null) {
				var edges = page.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual SkinElement GetPageElement() {
			return GetSkin()[MetroUISkins.SkinPage];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore);
		}
	}
}
