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
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using System.Windows;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.Printing.PreviewControl.Rendering {
	public class DocumentPreviewRenderer : DocumentViewerRenderer {
		INativeRendererImpl nativeRenderer;
		new DocumentPresenterControl Presenter { get { return base.Presenter as DocumentPresenterControl; } }
		public DocumentPreviewRenderer(DocumentPresenterControl presenter)
			: base(presenter) {
				UpdateInnerRenderer();
		}
		public void UpdateInnerRenderer() {
			nativeRenderer = new DirectNativeRendererImpl();
		}
		public override bool RenderToGraphics(Graphics graphics, Editors.INativeImageRenderer renderer, Rect invalidateRect, System.Windows.Size totalSize) {
			DocumentViewModel model = (DocumentViewModel)Presenter.Document;
			if(Presenter.BehaviorProvider == null)
				return false;
			if(model.Return(x=> x.Pages.Count == 0, ()=> true)) {
				SetRenderMask(new DrawingBrush(new GeometryDrawing()));
				return true;
			}
			var renderedContent = new RenderedContent() {
				RenderedPages = Presenter.GetDrawingContent(),
				SelectionService = Presenter.SelectionService
			};
			SetRenderMask(Presenter.GenerateRenderMask(renderedContent.RenderedPages));
			return nativeRenderer.RenderToGraphics(graphics, renderedContent, Presenter.BehaviorProvider.ZoomFactor, Presenter.BehaviorProvider.RotateAngle);
		}
	}
}
