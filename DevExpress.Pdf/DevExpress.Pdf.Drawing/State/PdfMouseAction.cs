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

namespace DevExpress.Pdf.Drawing {
	public struct PdfMouseAction {
		readonly PdfDocumentPosition documentPosition;
		readonly PdfMouseButton button;
		readonly PdfModifierKeys modifierKeys;
		readonly int clicks;
		readonly bool isOutsideOfView;
		public PdfDocumentPosition DocumentPosition { get { return documentPosition; } }
		public PdfMouseButton Button { get { return button; } }
		public PdfModifierKeys ModifierKeys { get { return modifierKeys; } }
		public int Clicks { get { return clicks; } }
		public bool IsOutsideOfView { get { return isOutsideOfView; } }
		public PdfMouseAction(PdfDocumentPosition documentPosition, PdfMouseButton button, PdfModifierKeys modifierKeys, int clicks, bool isOutsideOfView) {
			this.documentPosition = documentPosition;
			this.button = button;
			this.modifierKeys = modifierKeys;
			this.clicks = clicks;
			this.isOutsideOfView = isOutsideOfView;
		}
		public PdfMouseAction(PdfDocumentPosition documentPosition, PdfMouseButton button, PdfModifierKeys modifierKeys, int clicks) : this(documentPosition, button, modifierKeys, clicks, false) {
		}
	}
}
