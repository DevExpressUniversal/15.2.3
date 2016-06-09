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
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraPrinting {
	public class TableRow : IXtraSupportDeserializeCollectionItem {
		private TableCellList bricks;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TableRowBricks"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public BrickList Bricks { get { return bricks; } }
		public TableRow() {
			bricks = new TableCellList();
		}
		internal SizeF CalcSize() {
			bricks.InvalidateBounds();
			return bricks.Bounds.Size;
		}
		internal void Align(BrickAlignment alignment, float yOffset, float width) {
			float widthDifference = width - CalcSize().Width;
			switch(alignment) {
				case BrickAlignment.Far:
					AlignFromPoint(yOffset, widthDifference);
					break;
				case BrickAlignment.Center:
					AlignFromPoint(yOffset, widthDifference / 2);
					break;
				default:
					AlignFromPoint(yOffset, 0);
					break;
			}
		}
		void AlignFromPoint(float yOffset, float initialXOffset) {
			float xOffset = initialXOffset;
			foreach(Brick brick in bricks) {
				brick.Location = new PointF(xOffset, yOffset);
				xOffset += brick.Width;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				Bricks.Add((Brick)e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				return BrickFactory.CreateBrick(e);
			return null;
		}
	}
}
