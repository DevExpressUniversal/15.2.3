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
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.XtraSpellChecker;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region UnderlineAnchorType
	public enum UnderlineAnchorType {
		Left,
		Right,
	}
	#endregion
	#region UnderlineAnchor
	public class UnderlineAnchor {
		Box anchorBox;
		UnderlineAnchorType anchorType = UnderlineAnchorType.Left;
		public Box AnchorBox { get { return anchorBox; } set { anchorBox = value; } }
		public UnderlineAnchorType AnchorType { get { return anchorType; } set { anchorType = value; } }
	}
	#endregion
	#region UnderlineBox
	public class UnderlineBox {
		#region Fields
		int startAnchorIndex;
		int boxCount;
		int underlineThickness;
		int underlinePosition;
		Rectangle underlineBounds;
		Rectangle clipBounds;
		#endregion
		public UnderlineBox()
			: this(0, 0) {
		}
		public UnderlineBox(int startAnchorIndex)
			: this(startAnchorIndex, 0) {
		}
		public UnderlineBox(int startAnchorIndex, int boxCount) {
			this.startAnchorIndex = startAnchorIndex;
			this.boxCount = boxCount;
		}
		#region Properties
		public int StartAnchorIndex { get { return startAnchorIndex; } set { startAnchorIndex = value; } }
		public int EndAnchorIndex { get { return startAnchorIndex + boxCount; } }
		public int BoxCount { get { return boxCount; } set { boxCount = value; } }
		public int UnderlineThickness { get { return underlineThickness; } set { underlineThickness = value; } }
		public int UnderlinePosition { get { return underlinePosition; } set { underlinePosition = value; } }
		public Rectangle UnderlineBounds { get { return underlineBounds; } set { underlineBounds = value; } }
		public Rectangle ClipBounds { get { return clipBounds; } set { clipBounds = value; } }
		#endregion
		public virtual void MoveVertically(int deltaY) {
			underlineBounds.Y += deltaY;
			clipBounds.Y += deltaY;
		}
	}
	#endregion
	#region UnderlineBoxCollectionBase<T> (abstract class)
	public abstract class UnderlineBoxCollectionBase<T> : List<T> where T : UnderlineBox {
		public void MoveVertically(int deltaY) {
			int count = Count;
			for (int i = 0; i < count; i++)
				this[i].MoveVertically(deltaY);
		}
	}
	#endregion
	#region UnderlineBoxCollection
	public class UnderlineBoxCollection : UnderlineBoxCollectionBase<UnderlineBox> {
	}
	#endregion
	#region ErrorBox
	public class ErrorBox : UnderlineBox {
		#region Fields
		int firstBoxOffset;
		int lastBoxOffset;
		FormatterPosition startPos;
		FormatterPosition endPos;
		SpellingError errorType;
		#endregion
		public ErrorBox()
			: base() {
			this.errorType = SpellingError.Misspelling;
		}
		public ErrorBox(int startAnchorIndex)
			: base(startAnchorIndex) {
		}
		public ErrorBox(int startAnchorIndex, int boxCount)
			: base(startAnchorIndex, boxCount) {
		}
		public ErrorBox(int startAnchorIndex, int boxCount, int firstBoxOffset, int lastBoxOffset)
			: base(startAnchorIndex, boxCount) {
			this.firstBoxOffset = firstBoxOffset;
			this.lastBoxOffset = lastBoxOffset;
		}
		#region Properties
		public int FirstBoxOffset { get { return firstBoxOffset; } set { firstBoxOffset = value; } }
		public int LastBoxOffset { get { return lastBoxOffset; } set { lastBoxOffset = value; } }
		public FormatterPosition StartPos { get { return startPos; } set { startPos = value; } }
		public FormatterPosition EndPos { get { return endPos; } set { endPos = value; } }
		internal SpellingError ErrorType { get { return errorType; } set { errorType = value; } }
		#endregion
		public virtual void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportErrorBox(this);
		}
	}
	#endregion
	#region ErrorBoxCollection
	public class ErrorBoxCollection : UnderlineBoxCollectionBase<ErrorBox> {
	}
	#endregion
}
