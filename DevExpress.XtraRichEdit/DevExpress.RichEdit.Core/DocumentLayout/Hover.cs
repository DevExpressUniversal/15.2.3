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
using System.Linq;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	public interface IHoverLayoutItem {
		DocumentLogPosition LogStart { get; }
		DocumentLogPosition LogEnd { get; }
		PieceTable PieceTable { get; }
		Point MousePosition { get; set; }
	}
	public interface IHoverLayoutItem<TBox> : IHoverLayoutItem where TBox : Box {
		TBox Box { get; }
	}
	public interface IHoverPainter : IDisposable {
		void Draw();
	}
	public interface IHoverPainter<TBox> : IHoverPainter where TBox : Box { }
	public abstract class HoverLayoutBase<TBox> : IHoverLayoutItem<TBox> where TBox : Box {
		#region Fields
		readonly RichEditView view;
		readonly TBox box;
		readonly DocumentLogPosition logStart;
		readonly DocumentLogPosition logEnd;
		readonly PieceTable pieceTable;
		Point mousePosition;
		#endregion
		protected HoverLayoutBase(RichEditView view, TBox box, DocumentLogPosition start, PieceTable pieceTable, Point mousePosition) {
			Guard.ArgumentNotNull(view, "view");
			Guard.ArgumentNotNull(box, "box");
			this.view = view;
			this.box = box;
			this.logStart = start;
			this.logEnd = start + 1;
			this.pieceTable = pieceTable;
			this.mousePosition = mousePosition;
		}
		#region Properties
		public DocumentLogPosition LogStart { get { return logStart; } }
		public DocumentLogPosition LogEnd { get { return logEnd; } }
		public TBox Box { get { return box; } }
		public RichEditView View { get { return view; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public Point MousePosition {
			get { return mousePosition; }
			set {
				if(mousePosition == value) return;
				mousePosition = value;
				OnMousePositionChanged();
			}
		}
		#endregion
		protected virtual void OnMousePositionChanged() { }
	}
	public abstract class HoverPainterBase<TBox> : IHoverPainter<TBox> where TBox : Box {
		readonly IHoverLayoutItem<TBox> layoutItem;
		readonly Painter painter;
		protected HoverPainterBase(IHoverLayoutItem<TBox> layoutItem, Painter painter) {
			this.layoutItem = layoutItem;
			this.painter = painter;
		}
		protected IHoverLayoutItem<TBox> LayoutItem { get { return layoutItem; } }
		protected Painter Painter { get { return painter; } }
		public abstract void Draw();
		public virtual void Dispose() { }
	}
	public class HoverInfo {
		Box box;
		bool isInsideBox;
		public HoverInfo(RichEditHitTestResult hitTestResult) {
			this.box = hitTestResult == null ? null : hitTestResult.Box;
			this.isInsideBox = hitTestResult != null && hitTestResult.Box != null && hitTestResult.Box.ActualSizeBounds.Contains(hitTestResult.LogicalPoint);
		}
		public static HoverInfo Empty { get { return new HoverInfo(null); } }
		public Box Box { get { return box; } private set { box = value; } }
		public bool IsInsideBox { get { return isInsideBox; } private set { isInsideBox = value; } }
		public bool UpdateHover(RichEditHitTestResult hitTestResult) {
			HoverInfo hoverInfo = new HoverInfo(hitTestResult);
			if(this.Equals(hoverInfo))
				return false;
			CopyFrom(hoverInfo);
			return true;
		}
		void CopyFrom(HoverInfo hoverInfo) {
			Box = hoverInfo.Box;
			IsInsideBox = hoverInfo.IsInsideBox;
		}
		public override bool Equals(object obj) {
			HoverInfo hoverInfo = obj as HoverInfo;
			return hoverInfo == null ? false : Box == hoverInfo.Box && IsInsideBox == hoverInfo.IsInsideBox;
		}
		public override int GetHashCode() {
			return Box == null ? 0 : Box.GetHashCode() ^ IsInsideBox.GetHashCode();
		}
	}
	public class HoverLayoutItems : ObjectFactoryBase<Box, IHoverLayoutItem> {
		protected override Type[] GetConstructorParameters<TKey, T>() {
			return new Type[] { typeof(RichEditView), typeof(TKey), typeof(DocumentLogPosition), typeof(PieceTable), typeof(Point) };
		}
		public IHoverLayoutItem Get(RichEditView view, Box box, DocumentLogPosition start, PieceTable pieceTable, Point mousePosition) {
			return base.Get(box.GetType(), view, box, start, pieceTable, mousePosition);
		}
	}
}
