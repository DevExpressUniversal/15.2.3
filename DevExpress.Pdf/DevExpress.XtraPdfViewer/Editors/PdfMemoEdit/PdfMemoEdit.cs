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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfMemoEdit : MemoEdit {
		static PdfMemoEdit() {
			PdfRepositoryItemMemoEdit.Register();
		}
		readonly PdfEditorController controller;
		public PdfEditorController Controller { get { return controller; } }
		public override string EditorTypeName { get { return PdfRepositoryItemMemoEdit.EditorName; } }
		public PdfMemoEdit(PdfEditorController controller) {
			this.controller = controller;
		}
		protected override void OnTextChanged(EventArgs e) {
			base.OnTextChanged(e);
			MemoEditViewInfo vi = GetViewInfo() as MemoEditViewInfo;
			Rectangle rect = Rectangle.Empty;
			using (Graphics g = CreateGraphics())
			using (GraphicsCache cache = new GraphicsCache(g)) {
				IHeightAdaptable heightAdaptable = vi as IHeightAdaptable;
				if (heightAdaptable != null) {
					int h = heightAdaptable.CalcHeight(cache, vi.MaskBoxRect.Width);
					ObjectInfoArgs args = new ObjectInfoArgs();
					args.Bounds = new Rectangle(0, 0, vi.ClientRect.Width, h);
					rect = vi.BorderPainter.CalcBoundsByClientRectangle(args);
				}
			}
			Properties.ScrollBars = rect.Height > Height ? ScrollBars.Vertical : ScrollBars.None;
			ScrollToCaret();
		}
	}
}
