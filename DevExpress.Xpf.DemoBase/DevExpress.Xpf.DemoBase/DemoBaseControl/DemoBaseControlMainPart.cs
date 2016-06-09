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
using DevExpress.DemoData.Helpers;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.DemoBase.Internal {
	sealed class DemoBaseControlMainPart : DemoBaseControlPart {
		WeakEventHandler<ModuleAppearEventArgs> onPagesContainerModuleAppear;
		WeakEventHandler<EventArgs> onPagesContainerBackButtonClick;
		WeakEventHandler<EventArgs> onPagesContainerBuyNowClick;
		public DemoBaseControlMainPart(DemoBaseControl demoBaseControl, StartupBase startup)
			: base(demoBaseControl) {
			onPagesContainerModuleAppear = new WeakEventHandler<ModuleAppearEventArgs>(OnPagesContainerModuleAppear);
			onPagesContainerBackButtonClick = new WeakEventHandler<EventArgs>(OnPagesContainerBackButtonClick);
			onPagesContainerBuyNowClick = new WeakEventHandler<EventArgs>(OnPagesContainerBuyNowClick);
			PagesContainer = CreatePagesContainer(startup);
			Initialized();
		}
		public DemoBaseControlPagesContainer PagesContainer {
			get { return GetProperty(() => PagesContainer); }
			set { SetProperty(() => PagesContainer, value); }
		}
		public event EventHandler<ModuleAppearEventArgs> ModuleAppear;
		public event EventHandler BackButtonClick;
		public event EventHandler BuyNowButtonClick;
		void OnPagesContainerModuleAppear(object sender, ModuleAppearEventArgs e) {
			if(ModuleAppear != null)
				ModuleAppear(this, e);
		}
		void OnPagesContainerBackButtonClick(object sender, EventArgs e) {
			if(BackButtonClick != null)
				BackButtonClick(this, EventArgs.Empty);
		}
		void OnPagesContainerBuyNowClick(object sender, EventArgs e) {
			if(BuyNowButtonClick != null)
				BuyNowButtonClick(this, EventArgs.Empty);
		}
		DemoBaseControlPagesContainer CreatePagesContainer(StartupBase startup) {
			DemoBaseControlPagesContainer container = new DemoBaseControlPagesContainer(DemoBaseControl, startup);
			container.ModuleAppear += onPagesContainerModuleAppear.Handler;
			container.BuyButtonClick += onPagesContainerBuyNowClick.Handler;
			return container;
		}
	}
}
