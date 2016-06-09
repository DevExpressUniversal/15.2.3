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
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraDiagram.Bars;
namespace DevExpress.XtraDiagram {
	public partial class DiagramControl : ICommandAwareControl<DiagramCommandId> {
		protected internal void RaiseUpdateUI() {
			OnUpdateUI();
		}
		protected internal void RaiseBeforeDispose() {
			OnBeforeDispose();
		}
		protected virtual void OnUpdateUI() {
			if(this.updateUI != null) this.updateUI(this, EventArgs.Empty);
		}
		protected virtual void OnBeforeDispose() {
			if(this.beforeDispose != null) this.beforeDispose(this, EventArgs.Empty);
		}
		#region ICommandAwareControl
		EventHandler beforeDispose;
		event EventHandler ICommandAwareControl<DiagramCommandId>.BeforeDispose {
			add { this.beforeDispose += value; }
			remove { this.beforeDispose -= value; }
		}
		void ICommandAwareControl<DiagramCommandId>.CommitImeContent() {
		}
		Command ICommandAwareControl<DiagramCommandId>.CreateCommand(DiagramCommandId id) {
			return CreateCommand(id);
		}
		protected virtual Command CreateCommand(DiagramCommandId id) {
			return Commands.CreateCommand(id);
		}
		void ICommandAwareControl<DiagramCommandId>.Focus() { Focus(); }
		bool ICommandAwareControl<DiagramCommandId>.HandleException(Exception e) { return false; }
		EventHandler updateUI;
		event EventHandler ICommandAwareControl<DiagramCommandId>.UpdateUI {
			add { this.updateUI += value; }
			remove { this.updateUI -= value; }
		}
		CommandBasedKeyboardHandler<DiagramCommandId> ICommandAwareControl<DiagramCommandId>.KeyboardHandler { get { return null; } }
		object IServiceProvider.GetService(Type serviceType) { return null; }
		#endregion
	}
}
