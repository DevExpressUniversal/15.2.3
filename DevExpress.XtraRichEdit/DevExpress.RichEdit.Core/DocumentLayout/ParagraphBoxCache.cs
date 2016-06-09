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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region ParagraphBoxCollection
	public class ParagraphBoxCollection {
		#region Fields
		readonly BoxCollection boxCollection;
		RunIndex paragraphStartRunIndex;
		NumberingListBox numberingListBox;
		bool containsLayoutDependentBox;
		bool containsFloatingObjectAnchorBox;
		#endregion
		public ParagraphBoxCollection() {
			this.boxCollection = new BoxCollection();
			InvalidateBoxes();
		}
		#region Properties
		public Box this[int index] { get { return boxCollection[index]; } }
		internal BoxCollection InnerCollection { get { return boxCollection; } }
		public NumberingListBox NumberingListBox { get { return numberingListBox; } set { numberingListBox = value; } }
		public RunIndex ParagraphStartRunIndex {
			get { return paragraphStartRunIndex; }
			set {
				if (paragraphStartRunIndex == value)
					return;
				if (IsValid)
					OffsetRunIndices(value - paragraphStartRunIndex);
				paragraphStartRunIndex = value;
			}
		}
		public bool IsValid { get { return paragraphStartRunIndex >= RunIndex.Zero && !containsLayoutDependentBox; } }
		public int Count { get { return boxCollection.Count; } }
		public bool ContainsFloatingObjectAnchorBox { get { return containsFloatingObjectAnchorBox; } }
		#endregion
		protected virtual void OffsetRunIndices(int delta) {
			int count = boxCollection.Count;
			for (int i = 0; i < count; i++) {
				boxCollection[i].OffsetRunIndices(delta);
			}
			if (numberingListBox != null)
				numberingListBox.OffsetRunIndices(delta);
		}
		public virtual void InvalidateBoxes() {
			paragraphStartRunIndex = new RunIndex(-1);
			numberingListBox = null;
		}
		public void Add(Box box) {
			Guard.ArgumentNotNull(box, "box");
			boxCollection.Add(box);
			if (box is LayoutDependentTextBox)
				containsLayoutDependentBox = true;
			if (box is FloatingObjectAnchorBox)
				containsFloatingObjectAnchorBox = true;
		}
		public Box GetBox(BoxInfo boxInfo) {
			Guard.ArgumentNotNull(boxInfo, "boxInfo");
			int boxCount = boxCollection.Count;
			for (int i = 0; i < boxCount; i++) {
				if (boxCollection[i].StartPos.AreEqual(boxInfo.StartPos) && boxCollection[i].EndPos.AreEqual(boxInfo.EndPos))
					return boxCollection[i];
			}
			return null;
		}
		public void Clear() {
			boxCollection.Clear();
			paragraphStartRunIndex = new RunIndex(-1);
			this.numberingListBox = null;
			containsLayoutDependentBox = false;
			containsFloatingObjectAnchorBox = false;
		}
	}
	#endregion
}
