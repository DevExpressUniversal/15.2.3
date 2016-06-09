#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardWin.Native {
	public interface IDragAreaPainterBase {
		ObjectPainter ObjectPainter { get; }
		int HorizontalMargins { get; }
	}
	public interface IDragAreaPainter : IDragAreaPainterBase {
		int SectionIndent { get; }
		Color SectionHeaderColor { get; }
	}
	public interface IDragGroupPainter : IDragAreaPainterBase {
		int GroupIndent { get; }
		int ItemIndent { get; }
		int ButtonIndent { get; }
		Color DropIndicatorColor { get; }
	}
	public interface IDragItemOptionsButtonPainter : IDragAreaPainterBase {
		Rectangle GetActualBounds(Rectangle bounds);
	}
	public abstract class DragAreaPainter : StyleObjectPainter, IDragAreaPainterBase {
		public ObjectPainter ObjectPainter { get { return this; } }
		public abstract int HorizontalMargins { get; }
		protected DragAreaPainter() {
		}
	}
	public abstract class DragAreaPainters {
		public IDragAreaPainter AreaPainter { get; protected set; }
		public IDragGroupPainter GroupPainter { get; protected set; }
		public StyleObjectPainter GroupSelectorPainter { get; protected set; }
		public StyleObjectPainter DragItemPainter { get; protected set; }
		public IDragItemOptionsButtonPainter DragItemOptionsButtonPainter { get; protected set; }
		public StyleObjectPainter SplitterPainter { get; protected set; }
		protected DragAreaPainters() {
		}
	}
}
