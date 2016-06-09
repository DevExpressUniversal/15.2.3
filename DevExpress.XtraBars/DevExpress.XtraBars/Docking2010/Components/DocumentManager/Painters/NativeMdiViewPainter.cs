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
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Views.NativeMdi {
	public class NativeMdiViewPainter : BaseViewPainter {
		public NativeMdiViewPainter(NativeMdiView view)
			: base(view) {
		}
		public NativeMdiViewInfo Info {
			get { return View.ViewInfo as NativeMdiViewInfo; }
		}
		protected override void DrawCore(GraphicsCache bufferedCache, Rectangle clip) {
			DrawBackground(bufferedCache, clip);
		}
	}
	public class NativeMdiViewSkinPainter : NativeMdiViewPainter {
		Skin skin;
		public NativeMdiViewSkinPainter(NativeMdiView view)
			: base(view) {
			skin = DockingSkins.GetSkin(View.ElementsLookAndFeel);
		}
		protected override void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			SkinElement element = skin[DockingSkins.SkinNativeMdiViewBackground];
			if(element != null) {
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, new SkinElementInfo(element, bounds));
				DrawBackgroundImage(cache.Graphics, View.BackgroundImage, bounds);
			}
			else base.DrawBackgroundCore(cache, bounds);
		}
		protected internal override ObjectPainter GetDocumentSelectorHeaderPainter() {
			return new Customization.DocumentSelectorHeaderSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorFooterPainter() {
			return new Customization.DocumentSelectorFooterSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorItemsListPainter() {
			return new Customization.DocumentSelectorItemsListSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorPreviewPainter() {
			return new Customization.DocumentSelectorPreviewSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorBackgroundPainter() {
			return new Customization.DocumentSelectorBackgroundSkinPainter(View.ElementsLookAndFeel);
		}
	}
}
