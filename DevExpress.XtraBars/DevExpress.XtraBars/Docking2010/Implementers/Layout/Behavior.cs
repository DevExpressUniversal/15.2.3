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
	class DockLayoutElementBehavior : ILayoutElementBehavior {
		public virtual bool AllowDragging {
			get { return false; }
		}
		public virtual bool CanDrag(OperationType operation) {
			return false;
		}
	}
	class DocumentManagerElementBehavior : DockLayoutElementBehavior {
		DocumentManagerElement elementCore;
		protected DocumentManagerElement Element {
			get { return elementCore; }
		}
		public DocumentManagerElementBehavior(DocumentManagerElement element) {
			elementCore = element;
		}
	}
	class BaseViewElementBehavior : DockLayoutElementBehavior {
		BaseViewElement elementCore;
		protected BaseViewElement Element {
			get { return elementCore; }
		}
		public BaseViewElementBehavior(BaseViewElement element) {
			elementCore = element;
		}
	}
	class FloatDocumentElementBehavior : DockLayoutElementBehavior {
		FloatDocumentInfoElement elementCore;
		protected FloatDocumentInfoElement Element {
			get { return elementCore; }
		}
		public FloatDocumentElementBehavior(FloatDocumentInfoElement element) {
			elementCore = element;
		}
		public override bool AllowDragging {
			get { return true; }
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return true;
			}
			return false;
		}
	}
	class FloatPanelElementBehavior : DockLayoutElementBehavior {
		FloatPanelInfoElement elementCore;
		protected FloatPanelInfoElement Element {
			get { return elementCore; }
		}
		public FloatPanelElementBehavior(FloatPanelInfoElement element) {
			elementCore = element;
		}
		public override bool AllowDragging {
			get { return true; }
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return true;
			}
			return false;
		}
	}
	class DocumentsHostWindowElementBehavior : DockLayoutElementBehavior {
		DocumentsHostWindowElement elementCore;
		protected DocumentsHostWindowElement Element {
			get { return elementCore; }
		}
		public DocumentsHostWindowElementBehavior(DocumentsHostWindowElement element) {
			elementCore = element;
		}
		public override bool AllowDragging {
			get { return true; }
		}
		public override bool CanDrag(OperationType operation) {
			switch(operation) {
				case OperationType.FloatingMoving:
					return true;
			}
			return false;
		}
	}
}
