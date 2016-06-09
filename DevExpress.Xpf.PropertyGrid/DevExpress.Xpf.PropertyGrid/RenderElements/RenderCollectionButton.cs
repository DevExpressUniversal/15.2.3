﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PropertyGrid {
	public class RenderCollectionButton : RenderButton {
		protected override FrameworkRenderElementContext CreateContextInstance() { return new RenderCollectionButtonContext(this); }
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
			((RenderCollectionButtonContext)context).Attach();
		}
	}
	public class RenderCollectionButtonContext : RenderButtonContext {
		RowControl rowControl;
		protected RowControl RowControl { get { return rowControl; } }
		CollectionButtonKind kind;
		public RenderCollectionButtonContext(RenderCollectionButton button) : base(button) {
			RenderTemplateSelector = null;
			Click += OnClick;
		}
		protected virtual void OnClick(IFrameworkRenderElementContext sender, RenderEventArgsBase args) {
			if (RowControl != null)
				RowControl.OnCollectionButtonClick(this);
		}
		public virtual void Attach() {
			rowControl = ElementHost.Parent as RowControl ?? ElementHost.Parent.With(PropertyGridHelper.GetRowControl) as RowControl;
			if (rowControl != null) {
				rowControl.RowDataChanged += OnRowDataChanged;
				OnRowDataChanged(RowControl, EventArgs.Empty);
			}			
		}
		protected virtual void OnRowDataChanged(object sender, EventArgs e) {
			if (RowControl == null || RowControl.RowData == null) {
				Kind = CollectionButtonKind.Remove;
				return;
			}
			Kind = RowControl.RowData.IsCollectionRow ? CollectionButtonKind.Add : CollectionButtonKind.Remove;
		}
		public override void Release() {
			if (rowControl != null) {
				rowControl.RowDataChanged -= OnRowDataChanged;
				rowControl = null;
				OnRowDataChanged(RowControl, EventArgs.Empty);
			}
			base.Release();
		}
		public CollectionButtonKind Kind {
			get { return kind; }
			set { SetProperty(ref kind, value, FREInvalidateOptions.None, OnKindChanged); }
		}
		public override void UpdateStates() {
			base.UpdateStates();
			UpdateKindState();
		}
		void OnKindChanged() { UpdateKindState(); }
		void UpdateKindState() { GoToState(Kind.ToString()); }
	}
}
