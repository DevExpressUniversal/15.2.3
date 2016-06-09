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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using System;
namespace DevExpress.DashboardWin.Native {
	public class OptionsButton : IDisposable {
		readonly DragGroup group;
		Image glyph;
		DragAreaButtonState state = DragAreaButtonState.Normal;
		Rectangle bounds;
		public Rectangle Bounds { get { return bounds; } }
		public Image Glyph { get { return glyph; } set { glyph = value; } }
		public OptionsButton(DragGroup group, Image glyph) {
			this.group = group;
			this.glyph = glyph;
		}
		public int Arrange(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			DragAreaAppearances appearances = drawingContext.Appearances;
			DragAreaPainters painters = drawingContext.Painters;
			StyleObjectInfoArgs args = new StyleObjectInfoArgs(cache, group.Bounds, appearances.GroupAppearance);
			Rectangle clientRectangle = painters.GroupPainter.ObjectPainter.GetObjectClientRectangle(args);
			int size = drawingContext.DragItemHeight;
			bounds = new Rectangle(clientRectangle.Right - size, clientRectangle.Y, size, clientRectangle.Height);
			args = new StyleObjectInfoArgs(cache, bounds, appearances.DragAreaButtonAppearance);
			return drawingContext.SectionWidth - painters.GroupSelectorPainter.CalcBoundsByClientRectangle(args).Width;
		}
		public void PaintGlyph(DragAreaDrawingContext drawingContext, GraphicsCache cache) {
			Image coloredGlyph = drawingContext.Appearances.GetColoredDragAreaButtonGlyph(glyph, state);
			ObjectPainter.DrawObject(cache, drawingContext.Painters.GroupSelectorPainter,
				new DragAreaButtonInfoArgs(cache, bounds, drawingContext.Appearances.DragAreaButtonAppearance, state, coloredGlyph));
		}
		public void SetOptionsButtonState(DragAreaButtonState state) {
			this.state = state;
		}
		public void ShowDialog(DragAreaControl dragArea) {
			if(state == DragAreaButtonState.Disabled) 
				return;
			XtraForm form = group.Section.CreateOptionsForm(dragArea.Designer, group);
			if (form != null)
				using (form) {
					group.Section.Area.ParentControl.LockSelection();
					group.Select();
					SetOptionsButtonState(DragAreaButtonState.Normal);
					dragArea.Invalidate();
					form.StartPosition = FormStartPosition.CenterParent;
					form.LookAndFeel.ParentLookAndFeel = dragArea.LookAndFeel;
					try {
						form.ShowDialog(dragArea.Designer.FindForm());
					}
					finally {
						group.Cleanup();
						group.Section.Area.ParentControl.UnlockSelection();
						dragArea.Area.Invalidate();
					}
				}
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				glyph.Dispose();
		}
	}
	public class DragAreaButtonInfoArgs : ElementWithButtonInfoArgs {
		readonly Image glyph;
		public Image Glyph { get { return glyph; } }
		public DragAreaButtonInfoArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance, DragAreaButtonState optionsButtonState, Image glyph) : base(cache, bounds, appearance, optionsButtonState) {
			this.glyph = glyph;
		}
	}
}
