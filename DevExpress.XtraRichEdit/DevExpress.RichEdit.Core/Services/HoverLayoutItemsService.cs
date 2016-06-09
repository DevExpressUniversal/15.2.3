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

using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Services {
	public interface IHoverLayoutItemsService {
		void Add<TBox, THoverLayout>() where TBox : Box where THoverLayout : IHoverLayoutItem;
		void AddIfNotPresent<TBox, THoverLayout>() where TBox : Box where THoverLayout : IHoverLayoutItem;
		void Remove<TBox>() where TBox : Box;
		void Clear();
		IHoverLayoutItem Get(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable, Point logicalPoint);
	}
}
namespace DevExpress.XtraRichEdit.Services.Implementation {
	public class HoverLayoutItemsService : IHoverLayoutItemsService {
		readonly HoverLayoutItems hoverLayoutItems = new HoverLayoutItems();
		public void Add<TBox, THoverLayout>()
		where TBox : Box
		where THoverLayout : IHoverLayoutItem {
			hoverLayoutItems.Add<TBox, THoverLayout>();
		}
		public void AddIfNotPresent<TBox, THoverLayout>()
		where TBox : Box
		where THoverLayout : IHoverLayoutItem {
			if(!hoverLayoutItems.Contains<TBox>())
				hoverLayoutItems.Add<TBox, THoverLayout>();
		}
		public void Remove<TBox>()
		where TBox : Box {
			hoverLayoutItems.Remove<TBox>();
		}
		public void Clear() {
			hoverLayoutItems.Clear();
		}
		public IHoverLayoutItem Get(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable, Point logicalPoint) {
			return hoverLayoutItems.Get(view, box, start, pieceTable, logicalPoint);
		}
	}
}
