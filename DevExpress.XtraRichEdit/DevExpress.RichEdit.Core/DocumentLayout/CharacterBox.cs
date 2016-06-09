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
using System.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	#region CharacterBox
	public class CharacterBox : MultiPositionBox {
		Rectangle tightBounds;
		#region Properties
		public override bool IsVisible { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel DetailsLevel { get { return DocumentLayoutDetailsLevel.Character; } }
		protected internal override HitTestAccuracy HitTestAccuracy { get { return HitTestAccuracy.ExactCharacter; } }
		public Rectangle TightBounds { get { return tightBounds; } set { tightBounds = value; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		#endregion
		public override Box CreateBox() {
			return new CharacterBox();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateCharacterBoxHitTestManager(this);
		}
		public override void MoveVertically(int deltaY) {
			base.MoveVertically(deltaY);
			tightBounds.Y += deltaY;
		}
	}
	#endregion
	#region CharacterBoxCollection
	public class CharacterBoxCollection : BoxCollectionBase<CharacterBox> {
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, CharacterBox item) {
			calculator.Result.Character = item;
			calculator.Result.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Character);
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
			calculator.Result.Character = null;
		}
	}
	#endregion
	#region DetailRow
	public class DetailRow {
		CharacterBoxCollection characters;
		public DetailRow() {
			this.characters = new CharacterBoxCollection();
		}
		public CharacterBoxCollection Characters { get { return characters; } }
	}
	#endregion
}
