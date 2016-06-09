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

using System.Drawing;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	namespace Tabbed {
		class DocumentGroupInfoElementHitInfo : LayoutElementHitInfo {
			public DocumentGroupInfoElementHitInfo(Point hitPoint, DocumentGroupInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InReorderingBounds {
				get { return InHeader || InControlBox; }
			}
			public override bool InHandlerBounds {
				get { return InHeader || InControlBox; }
			}
		}
		class DocumentInfoElementHitInfo : LayoutElementHitInfo {
			public DocumentInfoElementHitInfo(Point hitPoint, DocumentInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InReorderingBounds {
				get { return InBounds; }
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
#if DEBUGTEST
		class ResizeAssistentElementHitInfo : LayoutElementHitInfo {
			public ResizeAssistentElementHitInfo(Point point, BaseLayoutElement element): base(point, element) {
			}
			public override bool InHandlerBounds {
				get {
					return true;
				}
			}
		}
#endif
		class SplitterInfoElementHitInfo : LayoutElementHitInfo {
			public SplitterInfoElementHitInfo(Point hitPoint, SplitterInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InResizeBounds {
				get { return InBounds; }
			}
			protected override LayoutElementHitTest[] GetValidHotTests() {
				return new LayoutElementHitTest[] { LayoutElementHitTest.Bounds };
			}
			protected override LayoutElementHitTest[] GetValidPressedTests() {
				return new LayoutElementHitTest[] { LayoutElementHitTest.Bounds };
			}
		}
	}
}
