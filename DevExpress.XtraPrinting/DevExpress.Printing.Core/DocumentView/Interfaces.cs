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
using System.ComponentModel.Design;
using System.Collections.Generic;
using System;
using DevExpress.XtraPrinting;
#if SL
namespace DevExpress.DocumentView {
	public interface IPage {
	}
}
#else
namespace DevExpress.DocumentView {
	public interface IPageItem {
		int Index { get; }
		int OriginalIndex { get; }
		int PageCount { get; }
		int OriginalPageCount { get; }
	}
	public interface IPage {
		int Index { get; }
		SizeF PageSize { get; }
		RectangleF UsefulPageRectF { get; }
		RectangleF ApplyMargins(RectangleF pageRect);
		void Draw(Graphics gr, PointF position);
	}
}
#endif
namespace DevExpress.DocumentView {
	public interface IDocument : IServiceContainer {
		event EventHandler Disposed;
		event EventHandler DocumentChanged;
		event EventHandler BeforeBuildPages;
		event EventHandler AfterBuildPages;
		event ExceptionEventHandler CreateDocumentException;
		event EventHandler PageBackgrChanged;
		bool IsEmpty { get; }
		bool IsCreating { get; }
		IList<IPage> Pages { get; }
		IPageSettings PageSettings { get; }
		void BeforeDrawPages(object syncObj);
		void AfterDrawPages(object syncObj, int[] pageIndices);
	}
	public interface IPageSettings {
		int LeftMargin { get; set; }
		int RightMargin { get; set; }
		int TopMargin { get; set; }
		int BottomMargin { get; set; }
		SizeF PageSize { get; }
	}
}
