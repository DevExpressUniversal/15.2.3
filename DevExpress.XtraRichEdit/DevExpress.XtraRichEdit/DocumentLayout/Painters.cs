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
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	public class RichEditPaintersBase<TLayoutItem, TPainter> {
		readonly Dictionary<Type, TPainter> painters = new Dictionary<Type, TPainter>();
		TPainter defaultPainter;
		protected Dictionary<Type, TPainter> Painters { get { return painters; } }
		protected TPainter DefaultPainter { get { return defaultPainter; } }
		public void AddDefault(TPainter painter) {
			this.defaultPainter = painter;
		}
		public void Add<T>(TPainter painter) where T : TLayoutItem {
			Painters[typeof(T)] = painter;
		}
		public void RemoveDefault() {
			this.defaultPainter = default(TPainter);
		}
		public void Remove<T>() where T : TLayoutItem {
			Painters.Remove(typeof(T));
		}
		public void Clear() {
			this.defaultPainter = default(TPainter);
			Painters.Clear();
		}
		public TPainter Get(Type type) {
			TPainter painter;
			return Painters.TryGetValue(type, out painter) ? painter : DefaultPainter;
		}
		public TPainter Get<T>() where T : TLayoutItem {
			return Get(typeof(T));
		}
	}
	public class RichEditSelectionPainters : RichEditPaintersBase<ISelectionLayoutItem, RichEditSelectionPainter> { }
	public class RichEditHoverPainters : ObjectFactoryBase<IHoverLayoutItem, IHoverPainter> {
		protected override Type[] GetConstructorParameters<TKey, T>() {
			return new Type[] { typeof(TKey), typeof(Painter) };
		}
		public IHoverPainter Get(IHoverLayoutItem layoutItem, Painter painter) {
			return base.Get(layoutItem.GetType(), layoutItem, painter);
		}
	}
}
