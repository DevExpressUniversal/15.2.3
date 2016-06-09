﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraPdfViewer.Native {
	public class PdfListBoxEditViewInfo : BaseListBoxViewInfo {
		PdfEditorBorderPainter borderPainter;
		PdfListBoxEdit EditorOwner { get { return (PdfListBoxEdit)OwnerControl; } }
		public override BorderPainter BorderPainter {
			get {
				if (borderPainter == null)
					borderPainter = new PdfEditorBorderPainter(EditorOwner.Controller);
				return borderPainter;
			}
		}
		protected override void CalcContentRect(System.Drawing.Rectangle bounds) {
			base.CalcContentRect(borderPainter.GetScaledContentRect(bounds));
		}
		protected override BaseListBoxItemPainter CreateItemPainter() {
			return new PdfListBoxItemPainter(EditorOwner);
		}
		protected override Color GetItemForeColor(ItemInfo itemInfo) {
			if (itemInfo.State.HasFlag(DrawItemState.Selected))
				return EditorOwner.Controller.Settings.SelectionForeColor;
			return base.GetItemForeColor(itemInfo);
		}
		public PdfListBoxEditViewInfo(PdfListBoxEdit owner)
			: base(owner) {
		}
	}
}
