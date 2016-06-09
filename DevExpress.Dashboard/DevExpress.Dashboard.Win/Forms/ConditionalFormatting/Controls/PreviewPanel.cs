#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class PreviewPanel : PictureEdit {
		PreviewPanelPainter painter;
		protected override string EmptyImageText { get { return ""; } }
		protected override BaseControlPainter Painter { get { return this.painter ?? base.Painter; } }
		public PreviewPanel() {
		}
		internal void Initialize(IStyleContainerProvider owner, UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			Properties.ContextMenuStrip = new ContextMenuStrip();
			this.painter = new PreviewPanelPainter(owner);
			Refresh();
		}
	}
	class PreviewPanelPainter : PictureEditPainter {
		readonly IStyleContainerProvider owner;
		public PreviewPanelPainter()
			: this(null) {
		}
		public PreviewPanelPainter(IStyleContainerProvider owner) {
			this.owner = owner;
		}
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
			if(owner != null) {
				if(owner.Style.IsEmpty) {
					using(StringFormat format = new StringFormat(StringFormatFlags.NoWrap)) {
						format.LineAlignment = StringAlignment.Center;
						format.Alignment = StringAlignment.Center;
						format.Trimming = StringTrimming.Word;
						info.ViewInfo.PaintAppearance.DrawString(info.Cache, CaptionManager.AutomaticCaption, info.Bounds, format);
					}
				} else {
					StyleSettingsContainerPainter.Draw(owner.Style, info, info.Bounds, "Preview", true);
				}
			}
		}
	}
}
