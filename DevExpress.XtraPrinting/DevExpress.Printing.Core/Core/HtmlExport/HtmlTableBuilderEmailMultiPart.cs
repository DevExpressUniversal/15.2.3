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

using System.Linq;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using HtmlCellLayout = DevExpress.XtraPrinting.Export.Web.HtmlBuilderBase.HtmlCellLayout;
namespace DevExpress.XtraPrinting.Export.Web {
	static class MailBuilderHelper {
		public static void SetNavigationUrlCore(VisualBrick brick, DXHtmlContainerControl fCurrentCell) {
			string url = brick.Url;
			if(!string.IsNullOrEmpty(url)) {
				DXHtmlAnchor a = new DXHtmlAnchor();
				if(brick.Target.Equals("_self"))
					url = "#" + url;
				a.HRef = url;
				var controlsList = fCurrentCell.Controls.Cast<DXWebControlBase>().ToList();
				foreach(DXWebControlBase control in controlsList)
					a.Controls.Add(control);
				fCurrentCell.Controls.Clear();
				fCurrentCell.Controls.Add(a);
			}
		}
		public static void FillCellStyle(BrickViewData data, DXHtmlContainerControl control, HtmlCellLayout areaLayout) {
			if(data.Style != null) {
				control.Style.Value += PSHtmlStyleRender.GetHtmlStyle(data.Style.Font, data.Style.ForeColor, data.Style.BackColor, data.Style.BorderColor,
						areaLayout.Borders, areaLayout.Padding, data.Style.BorderDashStyle);
			}
		}
	}
	class HtmlTableBuilderMail : HtmlTableBuilder {
		protected override void SetNavigationUrlCore(VisualBrick brick) {
			MailBuilderHelper.SetNavigationUrlCore(brick, fCurrentCell);
		}
		protected override void FillCellStyle(BrickViewData data, DXHtmlContainerControl control, HtmlCellLayout areaLayout) {
			MailBuilderHelper.FillCellStyle(data, control, areaLayout);
		}
	}
	class HtmlDivBuilderMail : HtmlDivBuilder {
		protected override void SetNavigationUrlCore(VisualBrick brick) {
			MailBuilderHelper.SetNavigationUrlCore(brick, fCurrentCell);
		}
		protected override void FillCellStyle(BrickViewData data, DXHtmlContainerControl control, HtmlCellLayout areaLayout) {
			MailBuilderHelper.FillCellStyle(data, control, areaLayout);
		}
	}
}
