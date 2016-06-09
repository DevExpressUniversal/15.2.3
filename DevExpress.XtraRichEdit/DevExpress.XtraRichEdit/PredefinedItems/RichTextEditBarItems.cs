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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditCommandBarButtonItem (abstract class)
	public abstract class RichEditCommandBarButtonItem : ControlCommandBarButtonItem<RichEditControl, RichEditCommandId> {
		protected RichEditCommandBarButtonItem()
			: base() {
		}
		protected RichEditCommandBarButtonItem(string caption)
			: base(caption) {
		}
		protected RichEditCommandBarButtonItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected RichEditCommandBarButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
	}
	#endregion
	#region RichEditCommandBarCheckItem (abstract class)
	public abstract class RichEditCommandBarCheckItem : ControlCommandBarCheckItem<RichEditControl, RichEditCommandId> {
		protected RichEditCommandBarCheckItem()
			: base() {
		}
		protected RichEditCommandBarCheckItem(string caption)
			: base(caption) {
		}
		protected RichEditCommandBarCheckItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected RichEditCommandBarCheckItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
	}
	#endregion
	#region RichEditCommandBarEditItem (abstract class)
	public abstract class RichEditCommandBarEditItem<T> : ControlCommandBarEditItem<RichEditControl, RichEditCommandId, T> {
		protected RichEditCommandBarEditItem()
			: base() {
		}
		protected RichEditCommandBarEditItem(string caption)
			: base(caption) {
		}
		protected RichEditCommandBarEditItem(BarManager manager)
			: base(manager) {
		}
		protected RichEditCommandBarEditItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
	}
	#endregion
	#region RichEditCommandBarSubItem (abstract class)
	public abstract class RichEditCommandBarSubItem : ControlCommandBarSubItem<RichEditControl, RichEditCommandId> {
		protected RichEditCommandBarSubItem()
			: base() {
		}
		protected RichEditCommandBarSubItem(string caption)
			: base(caption) {
		}
		protected RichEditCommandBarSubItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected RichEditCommandBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
	}
	#endregion
	#region RichEditCommandGalleryBarItem (abstract class)
	public abstract class RichEditCommandGalleryBarItem : ControlCommandGalleryBarItem<RichEditControl, RichEditCommandId> {
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditControl RichEditControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
	}
	#endregion
}
