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

using System.Collections;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Core.Native {
	public class RenderItemsControlContext : FrameworkRenderElementContext {
		IList itemsSource;
		RenderPanel itemPanelTemplate;
		RenderTemplate itemTemplate;
		protected override FrameworkRenderElementContext GetRenderChild(int index) {
			return ItemsHost;
		}
		protected override int RenderChildrenCount { get { return ItemsHost != null ? 1 : 0; } }		
		public IList ItemsSource {
			get { return itemsSource; }
			set { SetProperty(ref itemsSource, value, FREInvalidateOptions.UpdateSubTree); }
		}		
		public RenderPanel ItemPanelTemplate {
			get { return itemPanelTemplate; }
			set { SetProperty(ref itemPanelTemplate, value, FREInvalidateOptions.UpdateSubTree); }
		}		
		public RenderTemplate ItemTemplate {
			get { return itemTemplate; }
			set { SetProperty(ref itemTemplate, value, FREInvalidateOptions.UpdateSubTree); }
		}
		public RenderItemsControlContext(RenderItemsControl factory)
			: base(factory) {
		}
		public RenderPanelContext ItemsHost { get; private set; }
		public override void AddChild(FrameworkRenderElementContext child) {
			base.AddChild(child);
			RenderPanelContext pContext = (RenderPanelContext)child;
			ItemsHost.Do(RemoveChild);
			ItemsHost = pContext;
		}
	}
}
