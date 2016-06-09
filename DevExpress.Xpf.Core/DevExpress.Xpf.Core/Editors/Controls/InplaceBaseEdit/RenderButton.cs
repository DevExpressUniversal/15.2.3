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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using System;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Editors {
	public class RenderButton : RenderContentControl {
		ClickMode clickMode;
		public ClickMode ClickMode {
			get { return clickMode; }
			set { SetProperty(ref clickMode, value); }
		}
		public RenderButton() {
			RenderTemplateSelector = new RenderButtonTemplateSelector();
		}		
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderButtonContext(this);
		}
	}
	public class RenderButtonContext : RenderContentControlContext {			 
		bool? isChecked = false;
		ClickMode? clickMode;
		bool canRaiseClick;
		public ClickMode? ClickMode {
			get { return clickMode; }
			set { SetProperty(ref clickMode, value, FREInvalidateOptions.None); }
		}
		public bool? IsChecked {
			get { return isChecked; }
			set { SetProperty(ref isChecked, value, FREInvalidateOptions.None, UpdateCheckedState); }
		}
		ClickMode ActualClickMode { get { return ClickMode.HasValue ? ClickMode.Value : ((RenderButton)Factory).ClickMode; } }
		public event RenderEventHandler Click;
		public RenderButtonContext(RenderButton factory) : base(factory) { } 
		public override void UpdateStates() {
			base.UpdateStates();
			UpdateCheckedState();
			canRaiseClick &= IsMouseOver;
		}		
		protected virtual void UpdateCheckedState() {			
			string stateName = "Indeterminate";
			if (IsChecked.HasValue) {
				if (IsChecked.Value)
					stateName = "Checked";
				else
					stateName = "Unchecked";
			}
			GoToState(stateName);
		}
		protected override void OnMouseEnter(MouseRenderEventArgs args) {
			base.OnMouseEnter(args);
			if (ActualClickMode == System.Windows.Controls.ClickMode.Hover)
				RaiseClickEvent(args);
		}		
		protected override void OnPreviewMouseDown(MouseRenderEventArgs args) {
			base.OnPreviewMouseDown(args);
			if (ActualClickMode == System.Windows.Controls.ClickMode.Press)
				RaiseClickEvent(args);
		}
		protected override void OnMouseUp(MouseRenderEventArgs args) {
			base.OnMouseUp(args);
			if (ActualClickMode == System.Windows.Controls.ClickMode.Release)
				RaiseClickEvent(args);
		}
		void RaiseClickEvent(MouseRenderEventArgs args) {
			if (Click == null)
				return;
			Click(this, args);
		}
	}
	public class RenderButtonTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);
			return resourceProvider.GetRenderButtonTemplate(chrome);
		}
	}
}
