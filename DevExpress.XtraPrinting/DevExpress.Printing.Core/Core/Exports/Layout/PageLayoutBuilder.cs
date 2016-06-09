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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Export {
	public abstract class PageLayoutBuilder : ILayoutBuilder, IDisposable {
		XtraPrinting.Page page;
		protected LayoutControlCollection layoutControls;
		protected ExportContext exportContext;
		protected XtraPrinting.Page Page { get { return page; } }
		protected virtual PointF InitialLayoutLocation { get { return PointF.Empty; } }
		internal ExportContext ExportContext { get { return exportContext; } }
		protected PageLayoutBuilder(XtraPrinting.Page page, ExportContext exportContext) {
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(null == exportContext.DrawingPage);
#endif
			exportContext.SetDrawingPage(page);
			this.page = page;
			this.exportContext = exportContext;
		}
		public LayoutControlCollection BuildLayoutControls() {
			layoutControls = new LayoutControlCollection();
			RectangleF rect = new RectangleF(InitialLayoutLocation, page.PageSize);
			PageExporter exporter = BrickBaseExporter.GetExporter(exportContext.PrintingSystem, page) as PageExporter;
			exporter.ProcessLayout(this, rect.Location, rect);
			return layoutControls;
		}
		internal void AddData(BrickViewData[] data) {
			foreach(BrickViewData item in data) {
				layoutControls.Add(LayoutControl.Validate(item));
			}
		}
		internal virtual RectangleF ValidateLayoutRect(Brick brick, RectangleF rect) {
			return rect;
		}
		internal abstract BrickViewData[] GetData(Brick brick, RectangleF bounds, RectangleF clipRect);
		internal abstract RectangleF GetCorrectClipRect(RectangleF clipRect);
		#region IDisposable Members
		public void Dispose() {
			exportContext.ResetDrawingPage();
		}
		#endregion
	}
}
