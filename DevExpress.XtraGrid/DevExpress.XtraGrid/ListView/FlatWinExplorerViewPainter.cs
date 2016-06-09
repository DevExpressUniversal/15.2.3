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

using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.Views.WinExplorer.Drawing {
	public class FlatWinExplorerViewPainter : WindowsClassicWinExplorerViewPainter {
		ObjectPainter openCloseButtonPainter;
		public FlatWinExplorerViewPainter(BaseView view) : base(view) {
			this.openCloseButtonPainter = CreateOpenCloseButtonPainter();
		}
		protected virtual ObjectPainter CreateOpenCloseButtonPainter() { return new ExplorerBarOpenCloseButtonObjectPainter(); }
		protected internal override void DrawGroupCaptionButton(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			ExplorerBarOpenCloseButtonInfoArgs oa = new ExplorerBarOpenCloseButtonInfoArgs(e.Cache, groupInfo.CaptionButtonBounds, e.ViewInfo.PaintAppearance.GetAppearance("ItemHovered"), ((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo), groupInfo.ViewInfo.WinExplorerView.GetRowExpanded(groupInfo.Row.RowHandle));
			oa.BackAppearance.BackColor = SystemColors.Window;
			oa.BackAppearance.ForeColor = SystemColors.WindowText;
			openCloseButtonPainter.DrawObject(oa);
		}
	}
}
