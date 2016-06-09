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
using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPdfViewer.Native {
	class PdfDocumentViewerViewManager : ContinuousViewManager {			
		PdfDocumentViewer documentViewer;
		internal int ClientHeight { get { return Height; } }
		internal int ClientWidth { get { return Width; } }
		public PdfDocumentViewerViewManager(PdfDocumentViewer documentViewer, ViewControl viewControl) : base (documentViewer, viewControl) {
			this.documentViewer = documentViewer;
		}
		public override void OffsetVertScroll(float dy) {
			base.OffsetVertScroll(dy);
			documentViewer.Viewer.HandleScrolling();
		}
		public override void OffsetHorzScroll(float dx) {
			base.OffsetHorzScroll(dx);
			documentViewer.Viewer.HandleScrolling();
		}
		protected override float GetMinLeftMargin(float zoom) {
			PdfViewer viewer = documentViewer.Viewer;
			return viewer.ContentMarginMode == PdfContentMarginMode.Static ? PSUnitConverter.PixelToDoc(Math.Max(viewer.ContentMinMargin, Edges.Left), zoom) : base.GetMinLeftMargin(zoom);
		}
		protected override float GetMinRightMargin(float zoom) {
			PdfViewer viewer = documentViewer.Viewer;
			return viewer.ContentMarginMode == PdfContentMarginMode.Static ? PSUnitConverter.PixelToDoc(Math.Max(viewer.ContentMinMargin, Edges.Right), zoom) : base.GetMinRightMargin(zoom);
		}
		protected override float GetMinTopMargin(float zoom) {
			PdfViewer viewer = documentViewer.Viewer;
			return viewer.ContentMarginMode == PdfContentMarginMode.Static ? PSUnitConverter.PixelToDoc(Math.Max(viewer.ContentMinMargin, Edges.Top), zoom) : base.GetMinTopMargin(zoom);
		}
		protected override float GetMinBottomMargin(float zoom) {
			PdfViewer viewer = documentViewer.Viewer;
			return viewer.ContentMarginMode == PdfContentMarginMode.Static ? PSUnitConverter.PixelToDoc(Math.Max(viewer.ContentMinMargin, Edges.Bottom), zoom) : base.GetMinBottomMargin(zoom);
		}
		protected override void SetContinuousScroll(float val) {
			base.SetContinuousScroll(val);
			pc.ViewControl.Invalidate();
		}
	}
}
