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
using System.Linq;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class ReferenceEditController {
		#region TextChanged
		TextChangedEventHandler onTextChanged;
		public event TextChangedEventHandler TextChanged { add { onTextChanged += value; } remove { onTextChanged -= value; } }
		protected internal virtual void RaiseTextChanged(string text) {
			if (onTextChanged != null) {
				TextChangedEventArgs args = new TextChangedEventArgs(text);
				onTextChanged(this, args);
			}
		}
		#endregion
		#region SelectionChanged
		ReferenceEditSelectionChangedEventHandler onSelectionChanged;
		public event ReferenceEditSelectionChangedEventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged(IList<CellRange> ranges) {
			if (onSelectionChanged != null) {
				ReferenceEditSelectionChangedEventArgs args = new ReferenceEditSelectionChangedEventArgs(ranges);
				onSelectionChanged(this, args);
			}
		}
		#endregion
		#region CollapsedChanged
		EventHandler onCollapsedChanged;
		public event EventHandler CollapsedChanged { add { onCollapsedChanged += value; } remove { onCollapsedChanged -= value; } }
		protected internal virtual void RaiseCollapsedChanged() {
			if (onCollapsedChanged != null)
				onCollapsedChanged(this, EventArgs.Empty);
		}
		#endregion
	}
	#region ReferenceEditSelectionChangedEventHandler
	public delegate void ReferenceEditSelectionChangedEventHandler(object sender, ReferenceEditSelectionChangedEventArgs e);
	public class ReferenceEditSelectionChangedEventArgs : EventArgs {
		readonly IList<CellRange> ranges;
		public ReferenceEditSelectionChangedEventArgs(IList<CellRange> ranges) {
			this.ranges = ranges;
		}
		public IList<CellRange> ReferenceEditSelection { get { return ranges; } }
	}
	#endregion
}
