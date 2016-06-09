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
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Grid {
	public interface IGroupValueClient {
		void UpdateText();
	}
	public interface IGroupValuePresenter {
		GridGroupValueData ValueData { get; set; }
		bool? UseTemplate { get; }
		FrameworkElement Element { get; }
		DataTemplateSelector ContentTemplateSelector { get; set; }
	}
	public class GroupValuePresenter : Control, IGroupValuePresenter {
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(GroupValuePresenter), new PropertyMetadata(null));
		GroupValuePresenterController controller;
		static GroupValuePresenter() {
			Type ownerType = typeof(GroupValuePresenter);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			GridViewHitInfoBase.HitTestAcceptorProperty.OverrideMetadata(ownerType, new PropertyMetadata(new DevExpress.Xpf.Grid.HitTest.GroupValueTableViewHitTestAcceptor()));
		}
		public GroupValuePresenter() {
			controller = new GroupValuePresenterController(this);
		}
		#region IGroupValuePresenter Members
		GridGroupValueData IGroupValuePresenter.ValueData {
			get { return controller.ValueData; }
			set { controller.ValueData = value; }
		}
		bool? IGroupValuePresenter.UseTemplate { get { return false; } }
		FrameworkElement IGroupValuePresenter.Element { get { return this; } }
		DataTemplateSelector IGroupValuePresenter.ContentTemplateSelector {
			get { return null; }
			set { }
		}
		#endregion
	}
	class NullGroupValuePresenter : IGroupValuePresenter {
		GridGroupValueData IGroupValuePresenter.ValueData {
			get { return null; }
			set { }
		}
		bool? IGroupValuePresenter.UseTemplate { get { return null; } }
		FrameworkElement IGroupValuePresenter.Element { get { return null; } }
		DataTemplateSelector IGroupValuePresenter.ContentTemplateSelector {
			get { return null; }
			set { }
		}
	}
	abstract class GroupValuePresenterControllerBase<T> : IGroupValueClient where T : GridColumnData {
		T valueDataCore;
		internal protected T ValueData {
			get { return valueDataCore; }
			set {
				if(valueDataCore != value) {
					SetClient(null);
					valueDataCore = value;
					SetClient(this);
					UpdateText();
				}
			}
		}
		protected abstract void UpdateText();
		protected abstract void SetClient(IGroupValueClient client);
		#region IGroupValueClient Members
		void IGroupValueClient.UpdateText() {
			UpdateText();
		}
		#endregion
	}
	class GroupValuePresenterController : GroupValuePresenterControllerBase<GridGroupValueData> {
		readonly GroupValuePresenter presenter;
		public GroupValuePresenterController(GroupValuePresenter presenter) {
			this.presenter = presenter;
		}
		protected override void UpdateText() {
			presenter.Text = ValueData != null ? ValueData.DisplayText : null;
		}
		protected override void SetClient(IGroupValueClient client) {
			if(ValueData != null)
				ValueData.SetGroupValueClient(client);
		}
	}	
}
