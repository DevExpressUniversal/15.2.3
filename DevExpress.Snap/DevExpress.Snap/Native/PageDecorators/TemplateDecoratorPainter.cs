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
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Drawing;
namespace DevExpress.Snap.Native.PageDecorators {
	public class TemplateDecoratorPainter : IDecoratorPainter {
		#region Fields
		readonly RichEditView view;
		#endregion
		public TemplateDecoratorPainter(RichEditView view) {
			this.view = view;
		}
		#region Properties
		protected RichEditView View { get { return view; } }
		protected SnapControl Control { get { return (SnapControl)View.Control; } }
		#endregion
		public void DrawDecorators(Painter painter, PageViewInfoCollection viewInfos) {
			if (!Control.InnerControl.DrawTemplateDecorators && !Control.InnerControl.ForceDrawTemplateDecorators)
				return;
			int count = viewInfos.Count;
			RichEditViewPageViewInfoCollection richEditViewInfos = viewInfos as RichEditViewPageViewInfoCollection;
			if (richEditViewInfos == null)
				return;
			Control.InnerControl.ActiveUIElementViewInfos.Clear();
			DocumentLayout documentLayout = richEditViewInfos.DocumentLayout;
			for (int i = 0; i < count; i++) {
				TemplateDecoratorPagePainter pagePainter = CreateDecoratorPagePainter(documentLayout, viewInfos[i]);
				pagePainter.DrawPageDecorators(painter);
			}
		}
		protected virtual TemplateDecoratorPagePainter CreateDecoratorPagePainter(DocumentLayout documentLayout, PageViewInfo pageViewInfo) {
			return new TemplateDecoratorPagePainter(Control, documentLayout, pageViewInfo);
		}
	}
}
