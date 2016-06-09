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

using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Core;
using System;
#if !SL
#else
#endif
namespace DevExpress.Xpf.Editors.Popups {
	enum PopupContentContainerVisualStates {
		TopToBottomDirection,
		BottomToTopDirection
	}
	[TemplateVisualState(Name = "TopToBottomDirection", GroupName = "DropDownDirectionStates")]
	[TemplateVisualState(Name = "BottomToTopDirection", GroupName = "DropDownDirectionStates")]
	public partial class PopupContentContainer : ContentControl {
		public static readonly DependencyProperty DropOppositeProperty = DependencyProperty.Register("DropOpposite", typeof(bool), typeof(PopupContentContainer),
			new PropertyMetadata((d, e) => ((PopupContentContainer)d).DropOppositeChanged()));
		public PopupContentContainer() { }
		public PopupContentContainer(EditorPopupBase popup) {
			SetBinding(DropOppositeProperty, new Binding() {
				Path = new PropertyPath("DropOpposite"),
				Source = ((PopupBaseEditPropertyProvider)ActualPropertyProvider.GetProperties(BaseEdit.GetOwnerEdit(popup))).PopupViewModel
			});
		}
		public bool DropOpposite {
			get { return (bool)GetValue(DropOppositeProperty); }
			set { SetValue(DropOppositeProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState(false);
		}
		protected virtual void DropOppositeChanged() {
			UpdateVisualState(true);
		}
		internal protected virtual void UpdateVisualState(bool useTransitions) {
			string stateName = DropOpposite ? PopupContentContainerVisualStates.BottomToTopDirection.ToString() : PopupContentContainerVisualStates.TopToBottomDirection.ToString();
			VisualStateManager.GoToState(this, stateName, useTransitions);
		}
	}
}
