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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Internal {
	public partial class InnerRichEditControl {
		#region Events
		#region ReadOnlyChanged
		EventHandler onReadOnlyChanged;
		public event EventHandler ReadOnlyChanged { add { onReadOnlyChanged += value; } remove { onReadOnlyChanged -= value; } }
		protected internal virtual void RaiseReadOnlyChanged() {
			if (onReadOnlyChanged != null)
				onReadOnlyChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region OvertypeChanged
		EventHandler onOvertypeChanged;
		public event EventHandler OvertypeChanged { add { onOvertypeChanged += value; } remove { onOvertypeChanged -= value; } }
		protected internal virtual void RaiseOvertypeChanged() {
			if (onOvertypeChanged != null)
				onOvertypeChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region SearchComplete
		SearchCompleteEventHandler onSearchComplete;
		internal event SearchCompleteEventHandler SearchComplete { add { onSearchComplete += value; } remove { onSearchComplete -= value; } }
		protected internal void RaiseSearchComplete(SearchCompleteEventArgs e) {
			if (onSearchComplete != null)
				onSearchComplete(Owner, e);
		}
		#endregion
		#region ZoomChanged
		EventHandler onZoomChanged;
		public event EventHandler ZoomChanged { add { onZoomChanged += value; } remove { onZoomChanged -= value; } }
		protected internal virtual void RaiseZoomChanged() {
			if (onZoomChanged != null)
				onZoomChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region HyperlinkClick
		HyperlinkClickEventHandler onHyperlinkClick;
		public event HyperlinkClickEventHandler HyperlinkClick { add { onHyperlinkClick += value; } remove { onHyperlinkClick -= value; } }
		protected internal virtual void RaiseHyperlinkClick(HyperlinkClickEventArgs args) {
			if (onHyperlinkClick != null)
				onHyperlinkClick(Owner, args);
		}
		#endregion
		#region LayoutUnitChanged
		public event EventHandler LayoutUnitChanged;
		protected internal virtual void RaiseLayoutUnitChanged() {
			if (LayoutUnitChanged != null)
				LayoutUnitChanged(this, EventArgs.Empty);
		}
		#endregion
		#region VisiblePagesChanged
		EventHandler onVisiblePagesChanged;
		public event EventHandler VisiblePagesChanged { add { onVisiblePagesChanged += value; } remove { onVisiblePagesChanged -= value; } }
		protected internal virtual void RaiseVisiblePagesChanged() {
			if (onVisiblePagesChanged != null)
				onVisiblePagesChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
	}
}
