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
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class DocumentSelectorScrollItemInfo : DocumentSelectorItemInfo {
		public DocumentSelectorScrollItemInfo()
			: base(string.Empty, null) {
			Index = -1;
		}
		public override void Calc(GraphicsCache cache, DocumentSelectorItemPainter painter, Point offset) {
			PaintAppearance.Assign(painter.DefaultAppearance);
			Rectangle content = painter.CalcObjectMinBounds(this);
			Bounds = new Rectangle(offset, new Size(painter.MaxItemWidth, content.Height));
		}
	}
	public class DocumentSelectorScrollUpItemInfo : DocumentSelectorScrollItemInfo { }
	public class DocumentSelectorScrollDownItemInfo : DocumentSelectorScrollItemInfo { }
	public class DocumentSelectorScrollItemPainter : DocumentSelectorItemPainter {
		protected override void DrawContent(GraphicsCache cache, DocumentSelectorItemInfo info) {
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(0, 0, 0, 10);
		}
		protected override void DrawBackground(GraphicsCache cache, DocumentSelectorItemInfo info) {
			base.DrawBackground(cache, info);
		}
	}
	public class DocumentSelectorScrollItemSkinPainter : DocumentSelectorScrollItemPainter {
		ISkinProvider providerCore;
		public DocumentSelectorScrollItemSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected Skin GetSkin() {
			return NavBarSkins.GetSkin(providerCore);
		}
		protected SkinElement GetBackground(bool up) {
			return GetSkin()[up ? NavBarSkins.SkinScrollUp : NavBarSkins.SkinScrollDown];
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElement element = GetBackground(e is DocumentSelectorScrollUpItemInfo);
			SkinElementInfo elementInfo = new SkinElementInfo(element);
			return SkinElementPainter.Default.CalcObjectMinBounds(elementInfo);
		}
		protected override void DrawBackground(GraphicsCache cache, DocumentSelectorItemInfo info) {
			SkinElement element = GetBackground(info is DocumentSelectorScrollUpItemInfo);
			SkinElementInfo elementInfo = new SkinElementInfo(element, info.Bounds);
			elementInfo.State = info.State;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement element = GetBackground(e is DocumentSelectorScrollUpItemInfo);
			SkinElementInfo elementInfo = new SkinElementInfo(element, client);
			return SkinElementPainter.Default.CalcBoundsByClientRectangle(elementInfo, client);
		}
	}
}
