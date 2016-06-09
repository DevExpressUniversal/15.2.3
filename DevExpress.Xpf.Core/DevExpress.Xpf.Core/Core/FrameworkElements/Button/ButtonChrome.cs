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

using DevExpress.Xpf.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Core.Native {
	public class ButtonChrome : Chrome {
		static ButtonChrome() { }
		DropDownButtonBase owner;
		public RenderButtonBorderContext ContentPart { get; private set; }
		public RenderButtonBorderContext ArrowPart { get; private set; }
		public RenderButtonBorderContext ContentAndArrowPart { get; private set; }
		public DropDownButtonBase Owner {
			get { return owner; }
			set {
				owner = value;
				OnOwnerChanged();
			}
		}
		protected virtual void OnOwnerChanged() {
			UnsubscribeEvents(Owner);
			SubscribeEvents(Owner);
		}
		protected virtual void UnsubscribeEvents (DropDownButtonBase owner) {
			if (owner != null) {
				owner.MouseMove -= OnOwnerMouseMove;
				owner.MouseLeave -= OnOwnerMouseLeave;
				owner.RemoveHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnOwnerMouseLeftButtonDown));
				owner.RemoveHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnOwnerMouseLeftButtonUp));
			}
		}
	   protected virtual void SubscribeEvents(DropDownButtonBase owner) {
			if (owner != null) {
				owner.MouseMove += OnOwnerMouseMove;
				owner.MouseLeave += OnOwnerMouseLeave;
				owner.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnOwnerMouseLeftButtonDown), true);
				owner.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnOwnerMouseLeftButtonUp), true);
			}
		}
		protected override FrameworkRenderElementContext InitializeContext() {
			try { return base.InitializeContext(); } finally { OnApplyRenderTemplate(); }
		}  
		protected override void ReleaseContext(FrameworkRenderElementContext context) {
			base.ReleaseContext(context);
			OnApplyRenderTemplate();
		}
		protected virtual void OnApplyRenderTemplate() {
			ContentPart = (RenderButtonBorderContext)Namescope.GetElement("GlyphAndContentBorder");
			ArrowPart = (RenderButtonBorderContext)Namescope.GetElement("ArrowBorder");
			ContentAndArrowPart = (RenderButtonBorderContext)Namescope.GetElement("GlyphContentAndArrowBorder");
		}
		public void UpdateStates() {
			if (ContentPart == null || ArrowPart == null || ContentAndArrowPart == null)
				return;
			if (Owner.IsPopupOpen) {
				if (Owner.ActAsDropDown)
					ContentAndArrowPart.MouseState = "Pressed";
				else
					ArrowPart.MouseState = "Pressed";
				return;
			}
			if (!Owner.IsMouseOver) {
				Owner.IsMouseOverArrow = false;
				if (Owner.IsChecked != true)
					ContentPart.MouseState = "Normal";
				ArrowPart.MouseState = "Normal";
				ContentAndArrowPart.MouseState = "Normal";
				return;
			}
			if (Owner.ActAsDropDown) {
				if (!ContentAndArrowPart.IsMouseOverCore) {
					ContentAndArrowPart.MouseState = "Normal";
					return;
				}
				if (Mouse.LeftButton == MouseButtonState.Pressed) {
					if (!Owner.IsPopupClosing)
						Owner.IsPopupOpen = true;
					ContentAndArrowPart.MouseState = "Pressed";
				} else
					ContentAndArrowPart.MouseState = "MouseOver";
			} else {
				if (Owner.IsChecked == true) {
					ContentPart.MouseState = "Pressed";
				} else {
					if (ContentPart.IsMouseOverCore && !ArrowPart.IsMouseOverCore) {
						if (Mouse.LeftButton == MouseButtonState.Pressed)
							ContentPart.MouseState = "Pressed";
						else
							ContentPart.MouseState = "MouseOver";
					} else
						ContentPart.MouseState = "Normal";				   
				}
				if (!ArrowPart.IsMouseOverCore) {
					Owner.IsMouseOverArrow = false;
					ArrowPart.MouseState = "Normal";					
					return;
				}
				Owner.IsMouseOverArrow = true;
				if (Mouse.LeftButton == MouseButtonState.Pressed) {
					if (!Owner.IsPopupClosing)
						Owner.IsPopupOpen = true;
					ArrowPart.MouseState = "Pressed";
				} else
					ArrowPart.MouseState = "MouseOver";				
			}
		}
		protected virtual void OnOwnerMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			UpdateStates();
		}
		protected virtual void OnOwnerMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			UpdateStates();
		}
		protected virtual void OnOwnerMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			UpdateStates();
		}
		protected virtual void OnOwnerMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
			UpdateStates();
		}	
	}
	public class RenderButtonBorder : RenderControl {
		Dock? placement;
		public Dock? Placement {
			get { return placement; }
			set { SetProperty(ref placement, value); }
		}
		protected override FrameworkRenderElementContext CreateContextInstance() {
			return new RenderButtonBorderContext(this);
		}
		protected override void InitializeContext(FrameworkRenderElementContext context) {
			base.InitializeContext(context);
			((RenderButtonBorderContext)context).Placement = Placement;
		}
	}
	public class RenderButtonBorderContext : RenderControlContext {
		Dock? placement;		
		string mouseState;
		public RenderButtonBorderContext(RenderButtonBorder factory) : base(factory) { }
		public Dock? Placement {
			get { return placement; }
			set { SetProperty(ref placement, value, FREInvalidateOptions.None, OnPlacementChanged); }
		}
		public string MouseState {
			get { return mouseState; }
			set { SetProperty(ref mouseState, value, FREInvalidateOptions.None, OnMouseStateChanged); }
		}
		protected virtual void OnMouseStateChanged() {
			GoToState(mouseState != null ? mouseState : "Normal");
		}
		protected virtual void OnPlacementChanged() {
			GoToState(placement.HasValue ? placement.ToString() : "Default");
		}
		public override void UpdateStates() {
			base.UpdateStates();
			OnPlacementChanged();
			OnMouseStateChanged();
		}
	}
	public class RenderDropDownArrowTemplateSelector : RenderTemplateSelector {
		public override RenderTemplate SelectTemplate(FrameworkElement chrome, object content) {
			var resourceProvider = ThemeHelper.GetResourceProvider(chrome);		   
			return resourceProvider.GetButtonInfoDropDownGlyphKindTemplate(chrome);			
		}
	}
}
