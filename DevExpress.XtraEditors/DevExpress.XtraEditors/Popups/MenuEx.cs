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

using DevExpress.XtraEditors.Repository;
using System;
using System.Drawing;
using System.ComponentModel;
namespace DevExpress.Utils.Menu {
	public class DXEditMenuItem : DXMenuItem {
		private static readonly object editValueChanged = new object();
		object editValue;
		RepositoryItem edit;
		EventHandlerList events;
		int width = -1;
		int height = -1;
		public DXEditMenuItem() : base() { }
		public DXEditMenuItem(string caption) : base(caption) { }
		public DXEditMenuItem(string caption, RepositoryItem edit) : this(caption) {
			this.edit = edit;
		}
		public DXEditMenuItem(string caption, RepositoryItem edit, Image image) : base(caption, null, image) {
			this.edit = edit;
		}
		public DXEditMenuItem(string caption, RepositoryItem edit, Image image, Image disabled) : base(caption, null, image, disabled) {
			this.edit = edit;
		}
		public DXEditMenuItem(string caption, RepositoryItem edit, EventHandler editValueChanged, Image image, Image disabled) : base(caption, null, image, disabled) {
			this.edit = edit;
			EditValueChanged += editValueChanged;
		}
		public DXEditMenuItem(string caption, RepositoryItem edit, EventHandler editValueChanged, Image image, Image disabled, int width, int height)
			: base(caption, null, image, disabled) {
			this.edit = edit;
			this.width = width;
			this.height = height;
			EditValueChanged += editValueChanged;
		}
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		public event EventHandler EditValueChanged {
			add { Events.AddHandler(editValueChanged, value); }
			remove { Events.RemoveHandler(editValueChanged, value); }
		}
		void RaiseEditValueChanged() {
			EventHandler handler = Events[editValueChanged] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public object EditValue {
			get { return editValue; }
			set {
				if(EditValue == value)
					return;
				editValue = value;
				OnEditValueChanged();
			}
		}
		protected virtual void OnEditValueChanged() {
			RaiseEditValueChanged();
		}
		public RepositoryItem Edit {
			get { return edit; }
			set { edit = value; }
		}
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public int Height {
			get { return height; }
			set { height = value; }
		}
	}
}
