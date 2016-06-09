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

using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public class RenderCommandButton : RenderCheckBox {
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderCommandButtonContext(this);
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
			((RenderCommandButtonContext)context).Attach();
		}
	}
	public class RenderCommandButtonContext : RenderCheckBoxContext {
		RowControl rowControl;
		Bars.Native.CommandSourceHelper commandHelper;
		protected RowControl RowControl { get { return rowControl; } }
		public RenderCommandButtonContext(RenderCommandButton factory) : base(factory) {
			Click += OnClick;
		}
		protected virtual void OnClick(IFrameworkRenderElementContext sender, RenderEventArgsBase args) {
			if(commandHelper!=null && commandHelper.HasCommand) {
				commandHelper.Execute();
				return;
			}
			if (RowControl != null)
				RowControl.OnCommandButtonClick(this);
		}
		public virtual void Attach() {
			rowControl = ElementHost.Parent as RowControl ?? ElementHost.Parent.With(PropertyGridHelper.GetRowControl) as RowControl;
			if (rowControl != null) {
				rowControl.RowDataChanged += OnRowDataChanged;
				OnRowDataChanged(RowControl, EventArgs.Empty);
			}			
		}		
		public override void Release() {
			if (rowControl != null) {
				rowControl.RowDataChanged -= OnRowDataChanged;
				rowControl = null;
				OnRowDataChanged(RowControl, EventArgs.Empty);
			}			
			base.Release();
		}
		protected virtual void OnRowDataChanged(object sender, EventArgs e) {
			if (commandHelper != null)
				commandHelper.CanExecuteChanged -= OnCanExecuteChanged;
			commandHelper = RowControl.With(x => new Bars.Native.CommandSourceHelper(x));
			if (commandHelper != null)
				commandHelper.CanExecuteChanged += OnCanExecuteChanged;
			OnCanExecuteChanged(null, null);
		}
		protected virtual void OnCanExecuteChanged(object sender, EventArgs e) {
			IsEnabled = commandHelper == null ? true : commandHelper.CanExecute;
		}
	}
}
