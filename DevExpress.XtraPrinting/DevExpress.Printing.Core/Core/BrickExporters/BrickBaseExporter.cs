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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class BrickBaseExporter {
		internal static BrickBaseExporter GetExporter(IPrintingSystemContext context, object brick) {
			if(context != null && context.PrintingSystem != null)
				return context.PrintingSystem.ExportersFactory.GetExporter(brick);
			BrickBaseExporter exporter = ExportersFactory.CreateExporter(brick);
			exporter.SetBrick(brick);
			return exporter;
		}
		object brick;
		protected object Brick { get { return brick; } }
		protected BrickBase BrickBase { get { return (BrickBase)brick; } }
		public virtual void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
		}
		protected internal virtual void DrawWarningRect(IGraphics gr, RectangleF r, string message) {
			PSBrickPaint.DrawWarningRect(gr, r);
		}
		internal protected void SetBrick(object brick){
			this.brick = brick;
		}
		internal virtual void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
		}
		protected void ExecuteClippedAction(IGraphics gr, RectangleF clipBounds, Action0 action) {
			if(BrickBase.NoClip) {
				action();
				return;
			}
			RectangleF oldClip = gr.ClipBounds;
			try {
				gr.ClipBounds = clipBounds;
				action();
			} finally {
				gr.ClipBounds = oldClip;
			}
		}
	}
}
