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
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
	public class LinkListCheckItem : BarCheckItem {
		bool shouldCreateItemBindings = true;
		internal bool ShouldCreateItemBindings { get { return shouldCreateItemBindings; } private set { shouldCreateItemBindings = value; } }
		public LinkListCheckItem(BarItemLinkBase linkBase) {
			LinkBase = linkBase;
			if(Link != null && Link.Item != null)
				CreateBindings();
			IsThreeState = false;
			IsPrivate = true;
			CloseSubMenuOnClick = false;
		}
		private BarItemLinkBase linkBaseCore = null;
		public BarItemLinkBase LinkBase {
			get { return linkBaseCore; }
			set {
				if(linkBaseCore == value) return;
				BarItemLinkBase oldValue = linkBaseCore;
				linkBaseCore = value;
				OnLinkBaseChanged(oldValue);
			}
		}
		protected virtual void OnLinkBaseChanged(BarItemLinkBase oldValue) {
			CreateBindings();
		}
		public BarItemLink Link { get { return LinkBase as BarItemLink; } }
		protected internal virtual void CopyPropertiesFromItem() {
			if(Link == null || Link.Item == null)
				return;
			Content = Link.Item.GetCustomizationContent();
			if(Content == null)
				Content = Link.Item.GetContent();
			Glyph = Link.Item.GetCustomizationGlyph();
			if(Glyph == null)
				Glyph = Link.Item.GetGlyph();
			GlyphTemplate = Link.Item.GlyphTemplate;
		}
		protected internal virtual void CreateBindings() {
			if(!ShouldCreateItemBindings)
				return;
			CopyPropertiesFromItem();
			Binding visibilityBinding = new Binding("IsVisible") { Mode = BindingMode.TwoWay, Source = LinkBase };
			this.ClearValue(IsCheckedProperty);
			BindingOperations.SetBinding(this, IsCheckedProperty, visibilityBinding);
			ShouldCreateItemBindings = false;
		}
	}
}
