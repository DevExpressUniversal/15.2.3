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
using DevExpress.Office;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Layout {
	public interface ISpaceBox {
		int MinWidth { get; set; }
		Box Box { get; }
	}
	#region SpaceBoxa
	public class SpaceBoxa : CharacterMarkBox, ISpaceBox {
		int minWidth;
		public int MinWidth { get { return minWidth; } set { minWidth = value; } }
		public override bool IsVisible { get { return false; } }
		public override bool IsNotWhiteSpaceBox { get { return false; } }
		public override bool IsLineBreak { get { return false; } }
		protected internal override char MarkCharacter { get { return Characters.MiddleDot; } }
		Box ISpaceBox.Box { get { return this; } }
		public override Box CreateBox() {
			return new SpaceBoxa();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportSpaceBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateSpaceBoxHitTestManager(this);
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region SingleSpaceBox
	public class SingleSpaceBox : SingleCharacterMarkBox, ISpaceBox {
		int minWidth;
		public int MinWidth { get { return minWidth; } set { minWidth = value; } }
		public override bool IsVisible { get { return false; } }
		public override bool IsNotWhiteSpaceBox { get { return false; } }
		public override bool IsLineBreak { get { return false; } }
		protected internal override char MarkCharacter { get { return Characters.MiddleDot; } }
		Box ISpaceBox.Box { get { return this; } }
		public override Box CreateBox() {
			return new SingleSpaceBox();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportSpaceBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateSpaceBoxHitTestManager(this);
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
}
