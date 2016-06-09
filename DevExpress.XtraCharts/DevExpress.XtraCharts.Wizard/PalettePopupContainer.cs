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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	internal class PalettePopupContainerPainter : ButtonEditPainter {
		PaletteRepository repository;
		public PalettePopupContainerPainter(PaletteRepository repository) {
			this.repository = repository;
		}
		protected override void DrawText(ControlGraphicsInfoArgs info) {
			base.DrawText(info);
			TextEditViewInfo vi = info.ViewInfo as TextEditViewInfo;
			if (repository == null || !vi.AllowDrawText)
				return;
			SizeF size = info.Cache.Graphics.MeasureString(vi.DisplayText, vi.PaintAppearance.Font);
			int width = MathUtils.Ceiling((double)size.Width);
			Rectangle rect = vi.MaskBoxRect;
			if (rect.Width <= width)
				return;
			rect.Width -= width;
			rect.X += width;
			using (Image image = PaletteUtils.CreateEditorImage(repository[vi.DisplayText])) {
				if (image.Height < rect.Height)
					rect.Y += (rect.Height - image.Height) / 2;
				info.Cache.Graphics.DrawImageUnscaled(image, rect.X, rect.Y);
			}
		}
	}
	internal partial class PalettePopupContainerEdit : DevExpress.XtraEditors.PopupContainerEdit {
		PaletteRepository repository;
		BaseControlPainter painter;
		protected override BaseControlPainter Painter { 
			get { 
				if (painter == null) 
					painter = new PalettePopupContainerPainter(repository);
				return painter; 
			}
		}
		public PalettePopupContainerEdit() {
			InitializeComponent();
		}
		public void SetPaletteRepository(PaletteRepository repository) {
			this.repository = repository;
		}
	}
}
