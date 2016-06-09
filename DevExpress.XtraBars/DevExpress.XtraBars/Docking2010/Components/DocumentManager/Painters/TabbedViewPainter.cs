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
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public class TabbedViewPainter : BaseViewPainter {
		public TabbedViewPainter(TabbedView view)
			: base(view) {
		}
		public TabbedViewInfo Info {
			get { return View.ViewInfo as TabbedViewInfo; }
		}
		protected internal override int GetRootMargin() {
			return 1;
		}
		protected override void DrawCore(GraphicsCache bufferedCache, Rectangle clip) {
			DrawBackground(bufferedCache, clip);
			DrawGroups(bufferedCache);
			DrawSplitters(bufferedCache);
		}
		protected virtual void DrawGroups(GraphicsCache cache) {
			foreach(IDocumentGroupInfo groupInfo in Info.GetGroupInfos()) {
				groupInfo.Draw(cache);
			}
		}
		protected virtual void DrawSplitters(GraphicsCache cache) {
			foreach(ISplitterInfo splitter in Info.GetSplitterInfos()) {
				splitter.Draw(cache);
			}
		}
	}
	public class TabbedViewSkinPainter : TabbedViewPainter {
		Skin skin;
		public TabbedViewSkinPainter(TabbedView view)
			: base(view) {
			skin = DockingSkins.GetSkin(View.ElementsLookAndFeel);
		}
		protected internal override int GetRootMargin() {
			return skin.Properties.GetInteger(DockingSkins.DocumentGroupRootMargin);
		}
		protected internal override Color GetBackColor(Color parentBackColor) {
			Color skinColor = skin.Properties.GetColor(DockingSkins.DocumentGroupBackColor);
			return skinColor.IsEmpty ? parentBackColor : skinColor;
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
		public override ObjectPainter GetWaitScreenPainter() {
			return new Customization.WaitScreenSkinPainter(View.ElementsLookAndFeel);
		}
	}
}
