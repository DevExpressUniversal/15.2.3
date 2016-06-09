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
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using System.Drawing;
using DevExpress.Snap.Core.UI.Templates;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.Utils;
using DevExpress.Snap.Core.UI.Template;
using DevExpress.Office.Drawing;
using DevExpress.Snap.Localization;
namespace DevExpress.Snap.Native.PageDecorators {
	public class TemplateDecoratorPagePainter {
		readonly SnapControl control;
		readonly DocumentLayout documentLayout;
		readonly PageViewInfo pageViewInfo;
		readonly ILayoutToPhysicalBoundsConverter converter;
		public TemplateDecoratorPagePainter(SnapControl control, DocumentLayout documentLayout, PageViewInfo pageViewInfo) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(pageViewInfo, "pageViewInfo");
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			this.control = control;
			this.documentLayout = documentLayout;
			this.pageViewInfo = pageViewInfo;
			this.converter = new LayoutToPhysicalBoundsConverter(control, pageViewInfo);
		}
		protected ILayoutToPhysicalBoundsConverter Converter { get { return converter; } }
		protected SnapControl Control { get { return control; } }
		protected PageViewInfo PageViewInfo { get { return pageViewInfo; } }
		protected DocumentLayout DocumentLayout { get { return documentLayout; } }
		public void DrawPageDecorators(Painter painter) {
			GdiPlusPainter gdiPlusPainter = painter as GdiPlusPainter;
			if (gdiPlusPainter == null)
				return;
			DrawPageDecoratorsCore(gdiPlusPainter);
			DrawActiveUIElements(gdiPlusPainter);
		}
		void DrawPageDecoratorsCore(GdiPlusPainter painter) {
			SnapSelectionInfo selectionInfo = Control.InnerControl.GetSelectionInfo();
			TemplateDecoratorInfoCalculator calculator = new TemplateDecoratorInfoCalculator();
			ITemplateDecoratorInfo[] decorators = calculator.GetDecorators(selectionInfo);
			if (decorators == null)
				return;
			DrawDecoratorItems(painter, decorators);
		}
		private void DrawActiveUIElements(GdiPlusPainter painter) {
			Page page = PageViewInfo.Page;
			foreach (PageArea area in page.Areas)
				foreach (Column column in area.Columns)
					DrawActiveUIElementsForColumn(painter, page, area, column);
		}
		void DrawDecoratorItems(GdiPlusPainter painter, ITemplateDecoratorInfo[] decorators) {
			Guard.ArgumentNotNull(decorators, "decorators");
			if (decorators.Length == 0)
				return;
			DrawDecoratorBorders(painter, decorators);
		}
		void DrawDecoratorBorders(GdiPlusPainter painter, ITemplateDecoratorInfo[] decorators) {
			if (decorators.Length == 0)
				return;
			Page page = PageViewInfo.Page;
			bool wholeList = IsWholeListOrGroup(decorators);
			ITemplateDecoratorInfo info = decorators[0];
			TemplateController templateController = new TemplateController(info.PieceTable);
			SnapTemplateDecoratorViewInfoCalculator calculator = new SnapTemplateDecoratorViewInfoCalculator(info.PieceTable, documentLayout, templateController.GetActualResultInterval(info.Interval));
			List<TemplateDecoratorItem> items = calculator.Calculate(page);
			TemplateDecoratorItemPainter itemPainter = new TemplateDecoratorItemPainter(painter, Converter);
			foreach (TemplateDecoratorItem item in items) {
				if (!wholeList)
					itemPainter.DrawWithCaption(item, TemplateDecoratorItemLookAndFeelProperties.InnermostItem, TemplateDecoratorItemLookAndFeelProperties.TemplateTypeFont, GetLocalizedDecoratorTypeString(info.DecoratorType));
				else {
					TemplateDecoratorItemBorder itemBorder = TemplateDecoratorItemLookAndFeelProperties.OuterItems[Math.Min(TemplateDecoratorItemLookAndFeelProperties.OuterItems.Length - 1, 0)];
					itemPainter.Draw(item, itemBorder);
				}
			}
		}
		string GetLocalizedDecoratorTypeString(TemplateDecoratorType decoratorType) {
			string stringId = "SnapStringId.TemplateDecoratorType_" + decoratorType;
			System.Resources.ResourceManager resourceManager = new System.Resources.ResourceManager("DevExpress.Snap.LocalizationRes", typeof(SnapResLocalizer).Assembly);
			return resourceManager.GetString(stringId);
		}
		bool IsWholeListOrGroup(ITemplateDecoratorInfo[] decorators) {
			if (decorators.Length < 1)
				return false;
			return decorators[0].DecoratorType == TemplateDecoratorType.WholeList || decorators[0].DecoratorType == TemplateDecoratorType.WholeGroup;
		}
		List<TemplateDecoratorItem> GetPageDecoratorItems(ITemplateDecoratorInfo decoratorInfo, Page page, SnapPieceTable pieceTable) {
			if (decoratorInfo == null)
				return new List<TemplateDecoratorItem>();
			TemplateController templateController = new TemplateController(pieceTable);
			SnapTemplateDecoratorViewInfoCalculator innerInfoCalculator = new SnapTemplateDecoratorViewInfoCalculator(pieceTable, DocumentLayout, templateController.GetActualResultInterval(decoratorInfo.Interval));
			List<TemplateDecoratorItem> innerItems = innerInfoCalculator.Calculate(page);
			return innerItems;
		}
		void Draw(ILayoutUIElement layoutElement, GdiPlusPainter painter, Page page, PageArea area, Column column) {
			ILayoutUIElementViewInfo viewInfo = layoutElement.CalcViewInfoForColumn(page, area, column, DocumentLayout);
			if (viewInfo != null) {
				viewInfo.Draw(painter, Converter);
				Control.InnerControl.ActiveUIElementViewInfos.Add(viewInfo);
			}
		}
		void DrawActiveUIElementsForColumn(GdiPlusPainter painter, Page page, PageArea area, Column column) {
			Control.InnerControl.ActiveUIElements.ForEach(e => { Draw(e, painter, page, area, column); });
			Control.InnerControl.HotTrackUIElements.ForEach(e => { Draw(e, painter, page, area, column); });
		}
	}
}
