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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if SL
using DevExpress.Xpf.Collections;
#else
#endif
namespace DevExpress.XtraPrinting 
{
#if DEBUGTEST && !SL
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Count = {Items.Count}}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DebuggerHelpers.BrickListDebuggerTypeProxy))]
#endif
	public class BrickList : Collection<Brick>, IDisposable, IListWrapper<Brick>
	{
		RectangleF bounds = RectangleF.Empty;
		public RectangleF Bounds { 
			get { 
				if(bounds == RectangleF.Empty) 
					bounds = CalcBounds();
				return bounds;
			} 
		}
		public PointF Location { get { return Bounds.Location; } 
		}
		public SizeF Size { get { return Bounds.Size; } 
		}
		public float Height { get { return Bounds.Height; } 
		}
		public float Width { get { return Bounds.Width; } 
		}
		public float Left { get { return Bounds.Left; } 
		}
		public float Top { get { return Bounds.Top; } 
		}
		public float Right { get { return Bounds.Right; } 
		}
		public float Bottom { get { return Bounds.Bottom; } 
		}
		public BrickList() {
		}
		public BrickList(IList<Brick> list)
			: base(list) {
		}
		protected virtual RectangleF CalcBounds() {
			RectangleF r = Count > 0 ? this[0].InitialRect :
				RectangleF.Empty;
			for(int i = 1; i < Count; i++)
				r = RectangleF.Union(r, this[i].InitialRect);
			return r;
		}
		void IDisposable.Dispose() {
			for(int i = 0; i < Count; i++)
				this[i].Dispose();
			Clear();
		}
		public void InvalidateBounds() {
			bounds = RectangleF.Empty;
		}
		void IListWrapper<Brick>.Insert(Brick brick, int index) {
			throw new NotSupportedException();
		}
		void IListWrapper<Brick>.Add(Brick brick) {
			Add(brick);
		}
		public virtual void AddRange(IEnumerable<Brick> bricks) {
			foreach(Brick brick in bricks)
				Add(brick);
		}
		protected override void InsertItem(int index, Brick value) {
			base.InsertItem(index, value);
			InvalidateBounds();
		}
		protected override void RemoveItem(int index) {
			base.RemoveItem(index);
			InvalidateBounds();
		}
		protected override void ClearItems() {
			base.ClearItems();
			InvalidateBounds();
		}
		internal int InsertAfter(Brick brick, Brick previous) {
			int index = previous == null ? 0 : 
				IndexOf(previous) + 1;
			Insert(index, brick);
			return index;
		}
		internal void Offset(float yOffset) {
			PointF offset = new PointF(0.0f, yOffset);
			foreach (Brick brick in this)
				brick.Offset(offset);
		}
	}
}
