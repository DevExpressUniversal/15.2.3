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
using System.Text;
using System.Drawing;
using System.Collections;
namespace DevExpress.XtraPrinting.Native {
	public interface IPageContentAlgorithm {
		bool Process(PSPage page, DocumentBand documentBand, RectangleF bounds, PointF offset);
		RectangleF Bounds { get; }
		IList AddedPageBricks { get; }
	}
	public interface IPageContentService {
		void SetAlgorithm(DocumentBand docBand, IPageContentAlgorithm algorithm);
		IPageContentAlgorithm GetAlgorithm(DocumentBand docBand);
	}
	class PageContentService : IPageContentService, IDisposable {
		Dictionary<DocumentBand, IPageContentAlgorithm> dictiaonary = new Dictionary<DocumentBand, IPageContentAlgorithm>();
		public void SetAlgorithm(DocumentBand docBand, IPageContentAlgorithm algorithm) {
			dictiaonary[docBand] = algorithm;
		}
		public IPageContentAlgorithm GetAlgorithm(DocumentBand docBand) {
			IPageContentAlgorithm value;
			return dictiaonary.TryGetValue(docBand, out value) ? value : null;
		}
		void IDisposable.Dispose() {
			dictiaonary.Clear();
		}
	}
}
