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
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils.Commands;
namespace DevExpress.XtraScheduler.UI {
	#region ISchedulerBarItem interface
	public interface ISchedulerBarItem {
		SchedulerControl SchedulerControl { get; set; }
	}
	#endregion
	#region SchedulerCommandBarButtonItemBase
	public abstract class SchedulerCommandBarButtonItemBase : CommandBasedBarButtonItem, ISchedulerBarItem {
		#region Fields
		SchedulerControl schedulerControl;
		#endregion
		protected SchedulerCommandBarButtonItemBase()
			: base() {
		}
		protected SchedulerCommandBarButtonItemBase(string caption)
			: base(caption) {
		}
		protected SchedulerCommandBarButtonItemBase(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected SchedulerCommandBarButtonItemBase(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				if (Object.ReferenceEquals(schedulerControl, value))
					return;
				if (schedulerControl != null)
					UnsubscribeControlEvents();
				schedulerControl = value;
				if (schedulerControl != null)
					SubscribeControlEvents();
			}
		}
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (schedulerControl != null) {
						UnsubscribeControlEvents();
						schedulerControl = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeControlEvents() {
			schedulerControl.BeforeDispose += new EventHandler(OnBeforeDisposeControl);
		}
		protected internal virtual void UnsubscribeControlEvents() {
			schedulerControl.BeforeDispose -= new EventHandler(OnBeforeDisposeControl);
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.SchedulerControl = null;
		}
	}
	#endregion
	#region SchedulerCommandBarButtonItem (abstract class)
	public abstract class SchedulerCommandBarButtonItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId>, ISchedulerBarItem {
		protected SchedulerCommandBarButtonItem()
			: base() {
		}
		protected SchedulerCommandBarButtonItem(string caption)
			: base(caption) {
		}
		protected SchedulerCommandBarButtonItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected SchedulerCommandBarButtonItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl SchedulerControl {
			get { return base.Control; }
			set {
				base.Control = value;
			}
		}
		#endregion
	}
	#endregion
	#region SchedulerCommandBarCheckItem (abstract class)
	public abstract class SchedulerCommandBarCheckItem : CommandBasedBarCheckItem, ISchedulerBarItem {
		#region Fields
		SchedulerControl schedulerControl;
		#endregion
		protected SchedulerCommandBarCheckItem()
			: base() {
		}
		protected SchedulerCommandBarCheckItem(string caption)
			: base(caption) {
		}
		protected SchedulerCommandBarCheckItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected SchedulerCommandBarCheckItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				if (Object.ReferenceEquals(schedulerControl, value))
					return;
				if (schedulerControl != null)
					UnsubscribeControlEvents();
				schedulerControl = value;
				if (schedulerControl != null)
					SubscribeControlEvents();
			}
		}
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (schedulerControl != null) {
						UnsubscribeControlEvents();
						schedulerControl = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected internal virtual void SubscribeControlEvents() {
			schedulerControl.BeforeDispose += new EventHandler(OnBeforeDisposeControl);
		}
		protected internal virtual void UnsubscribeControlEvents() {
			schedulerControl.BeforeDispose -= new EventHandler(OnBeforeDisposeControl);
		}
		protected internal virtual void OnBeforeDisposeControl(object sender, EventArgs e) {
			this.SchedulerControl = null;
		}
	}
	#endregion
	#region SchedulerCommandBarSubItem (abstract class)
	public abstract class SchedulerCommandBarSubItem : ControlCommandBarSubItem<SchedulerControl, SchedulerCommandId> {
		protected SchedulerCommandBarSubItem()
			: base() {
		}
		protected SchedulerCommandBarSubItem(string caption)
			: base(caption) {
		}
		protected SchedulerCommandBarSubItem(BarManager manager)
			: base(manager, string.Empty) {
		}
		protected SchedulerCommandBarSubItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		#region Properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SchedulerControl SchedulerControl { get { return this.Control; } set { this.Control = value; } }
		#endregion
	}
	#endregion
	public abstract class SchedulerCommandBarEditItem<T> : ControlCommandBarEditItem<SchedulerControl, SchedulerCommandId, T> {
		protected SchedulerCommandBarEditItem() {
		}
		protected SchedulerCommandBarEditItem(string caption)
			: base(caption) {
		}
		protected SchedulerCommandBarEditItem(BarManager manager)
			: base(manager) {
		}
		protected SchedulerCommandBarEditItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		public virtual bool UseCommandCaption {get; set;}
		protected override string GetDefaultCaption() {
			if (!UseCommandCaption)
				return String.Empty;
			Command command = CreateCommand();
			return command != null ? command.MenuCaption : String.Empty;
		}
	}
	public abstract class SchedulerBarStaticItem : BarStaticItem {
		protected SchedulerBarStaticItem() {
			Caption = GetDefaultCaption();
		}
		#region Caption
		public override string Caption {
			get {
				return base.Caption;
			}
			set {
				base.Caption = value;
			}
		}
		protected override bool ShouldSerializeCaption() {
			return GetDefaultCaption() != Caption;
		}
		protected override void ResetCaption() {
			Caption = GetDefaultCaption();
		}
		#endregion
		#region Glyph
		public override System.Drawing.Image Glyph {
			get {
				return base.Glyph;
			}
			set {
				base.Glyph = value;
			}
		}
		#endregion
		#region LargeGlyph
		public override System.Drawing.Image LargeGlyph {
			get {
				return base.LargeGlyph;
			}
			set {
				base.LargeGlyph = value;
			}
		}
		#endregion
		public abstract SchedulerExtensionsStringId StringId { get; }
		protected virtual string GetDefaultCaption() {
			return SchedulerExtensionsLocalizer.GetString(StringId);
		}
	}
}
