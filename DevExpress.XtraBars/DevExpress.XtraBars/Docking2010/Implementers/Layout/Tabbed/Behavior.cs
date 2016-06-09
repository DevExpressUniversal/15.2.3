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

using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	namespace Tabbed {
		class DocumentGroupInfoElementBehavior : DockLayoutElementBehavior {
			DocumentGroupInfoElement elementCore;
			protected DocumentGroupInfoElement Element {
				get { return elementCore; }
			}
			public DocumentGroupInfoElementBehavior(DocumentGroupInfoElement element) {
				elementCore = element;
			}
		}
		class DocumentInfoElementBehavior : DockLayoutElementBehavior {
			DocumentInfoElement elementCore;
			protected DocumentInfoElement Element {
				get { return elementCore; }
			}
			public DocumentInfoElementBehavior(DocumentInfoElement element) {
				elementCore = element;
			}
			public override bool AllowDragging {
				get { return Element.TabPage.PageEnabled; }
			}
			public override bool CanDrag(OperationType operation) {
				switch(operation) {
					case OperationType.NonClientDragging:
					case OperationType.Reordering:
						return Element.CanReordering;
					case OperationType.Floating:
						return Element.CanFloat;
					case OperationType.Docking:
						return !Element.CanFloat;
				}
				return false;
			}
		}
		class SplitterInfoElementBehavior : DockLayoutElementBehavior {
			SplitterInfoElement elementCore;
			protected SplitterInfoElement Element {
				get { return elementCore; }
			}
			public SplitterInfoElementBehavior(SplitterInfoElement element) {
				elementCore = element;
			}
			public override bool AllowDragging {
				get { return true; }
			}
			public override bool CanDrag(OperationType operation) {
				switch(operation) {
					case OperationType.Resizing:
						return true;
				}
				return false;
			}
		}
	}
}
