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
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Utils;
namespace DevExpress.Office.Model {
	#region IPatternLinePainter<T>
	public interface IPatternLinePainter<T> where T : struct {
	}
	#endregion
	#region PatternLine<T, TPainter> (abstract class)
	public abstract class PatternLine<T> where T : struct {
		public abstract T Id { get; }
		public abstract Rectangle CalcLineBounds(Rectangle r, int thickness);
		public abstract void Draw(IPatternLinePainter<T> painter, RectangleF bounds, Color color);
		public virtual float CalcLinePenVerticalOffset(RectangleF lineBounds) {
			return lineBounds.Height / 2;
		}
	}
	#endregion
	#region PatternLineRepository<T> (abstract class)
	public abstract class PatternLineRepository<T, TItem, TCollection>
		where T : struct
		where TItem : PatternLine<T>
		where TCollection : List<TItem>, new() {
		readonly TCollection collection = new TCollection();
		protected PatternLineRepository() {
			PopulateRepository();
		}
		public TCollection Items { get { return collection; } }
		protected internal abstract void PopulateRepository();
		public bool RegisterPatternLine(TItem line) {
			Guard.ArgumentNotNull(line, "line");
			TItem existingPatternLine = GetPatternLineByType(line.Id);
			if (existingPatternLine != null)
				return false;
			collection.Add(line);
			return true;
		}
		public bool UnregisterPatternLine(TItem line) {
			Guard.ArgumentNotNull(line, "line");
			return UnregisterPatternLine(line.Id);
		}
		public bool UnregisterPatternLine(T type) {
			TItem line = GetPatternLineByType(type);
			if (line == null)
				return false;
			collection.Remove(line);
			return true;
		}
		internal TItem GetPatternLineByTypeInternal(T type) {
			TItem line = GetPatternLineByType(type);
			return line != null ? line : collection[0];
		}
		public TItem GetPatternLineByType(T line) {
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				if (collection[i].Id.Equals(line))
					return collection[i];
			}
			return null;
		}
	}
	#endregion
}
