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

#if no_compile
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using DevExpress.XtraLayout;
namespace DevExpress.XtraLayout.DesignTime {
	public class DesignerCustomizer : DesignCustomizeCore {
		IDesignerHost host;
		public DesignerCustomizer(IDesignerHost host, LayoutControl lc, ControlDesigner owner) {
			this.host = host;
			this.owner = owner;
			this.layoutControl = lc;
		}
		private LayoutControlDesigner Owner {
			get {
				return (LayoutControlDesigner)owner;
			}
		}
		DesignerTransaction movingTransaction;
		public void StartMovingItems() {
			movingTransaction = host.CreateTransaction("Sizing items");
			Owner.OnChanging(Owner, EventArgs.Empty);
		}
		public void EndMovingItems() {
			using(movingTransaction) {
				Owner.OnChanged(Owner, EventArgs.Empty);
				movingTransaction.Commit();
			}
		}
		DesignerTransaction draggingTransaction;
		public void StartDraggingItems() {
			draggingTransaction = host.CreateTransaction("Dragging item");
			Owner.OnChanging(Owner, EventArgs.Empty);
		}
		public void EndDraggingItems() {
			using(draggingTransaction) {
				Owner.OnChanged(Owner, EventArgs.Empty);
				draggingTransaction.Commit();
			}
		}
	}
}
#endif
