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
namespace DevExpress.XtraSpreadsheet {
	public partial class ReferenceEditControl {
		#region ActivatedChanged
		EventHandler onActivatedChanged;
		public event EventHandler ActivatedChanged { add { onActivatedChanged += value; } remove { onActivatedChanged -= value; } }
		protected internal virtual void RaiseActivatedChanged() {
			if (onActivatedChanged != null)
				onActivatedChanged(this, EventArgs.Empty);
		}
		#endregion
		#region CollapsedChanged
		CollapsedChangedEventHandler onCollapsedChanged;
		public event CollapsedChangedEventHandler CollapsedChanged { add { onCollapsedChanged += value; } remove { onCollapsedChanged -= value; } }
		protected internal virtual void RaiseCollapsedChanged() {
			if (onCollapsedChanged != null) {
				CollapsedChangedEventArgs args = new CollapsedChangedEventArgs(Controller.IsCollapsed);
				onCollapsedChanged(this, args);
			}
		}
		#endregion
		#region CollapseButtonClick
		EventHandler onCollapseButtonClick;
		public event EventHandler CollapseButtonClick { add { onCollapseButtonClick += value; } remove { onCollapseButtonClick -= value; } }
		protected internal virtual void RaiseCollapseButtonClick() {
			if (onCollapseButtonClick != null)
				onCollapseButtonClick(this, EventArgs.Empty);
		}
		#endregion
	}
	#region CollapsedChangedEventHandler
	public delegate void CollapsedChangedEventHandler(object sender, CollapsedChangedEventArgs e);
	public class CollapsedChangedEventArgs : EventArgs {
		readonly bool isCollapsed;
		public CollapsedChangedEventArgs(bool isCollapsed) {
			this.isCollapsed = isCollapsed;
		}
		public bool Collapsed { get { return isCollapsed; } }
	}
	#endregion
}
