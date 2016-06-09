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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraDiagram.Options {
	public abstract class DiagramOptionsBase : BaseOptions {
		DiagramOptionsChangingEventHandler changingCore;
		public DiagramOptionsBase() {
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
		}
		protected void OnChanging(string name, object value) {
			OnChanging(new DiagramOptionsChangingEventArgs(name, value));
		}
		protected virtual void OnChanging(DiagramOptionsChangingEventArgs e) {
			if(IsLockUpdate)
				return;
			RaiseOnChanging(e);
		}
		protected virtual void RaiseOnChanging(DiagramOptionsChangingEventArgs e) {
			if(changingCore != null) changingCore(this, e);
		}
		internal event DiagramOptionsChangingEventHandler Changing {
			add { this.changingCore += value; }
			remove { this.changingCore -= value; }
		}
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected internal new bool ShouldSerialize(IComponent owner) { return base.ShouldSerialize(owner); }
	}
	public class DiagramOptionsChangingEventArgs : EventArgs {
		readonly string name;
		readonly object value;
		public DiagramOptionsChangingEventArgs(string name, object value) {
			this.name = name;
			this.value = value;
		}
		public string Name { get { return name; } }
		public object Value { get { return value; } }
	}
	public delegate void DiagramOptionsChangingEventHandler(object sender, DiagramOptionsChangingEventArgs e);
}
