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

using DevExpress.Xpf.Bars;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Native;
using System;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageHeaderLinksControl : RibbonStatusBarPartControlBase, IMergingSupport {
		#region static
		static RibbonPageHeaderLinksControl() { }
		#endregion
		public RibbonPageHeaderLinksControl(ILogicalChildrenContainer owner) : base(owner) {
			DefaultStyleKey = typeof(RibbonPageHeaderLinksControl);
			ContainerType = LinkContainerType.RibbonPageHeader;
		}
		public override void OnApplyTemplate() {
			ForceCalcMaxGlyphSize();
			base.OnApplyTemplate();
		}
		public RibbonControl Ribbon { get; internal set; }
		protected internal virtual bool HasVisibleItems() {
			foreach(BarItemLinkInfo linkInfo in Items) {
				if(linkInfo.LinkBase.ActualIsVisible)
					return true;
			}
			return false;
		}
		public override LinksHolderType HolderType { get { return LinksHolderType.RibbonPageHeader; } }
		internal void SetExpandMode(BarPopupExpandMode expandMode) {
			foreach(var item in Items) {
				var linkInfo = ItemContainerGenerator.ContainerFromItem(item) as BarItemLinkInfo;
				if(linkInfo == null)
					continue;
				linkInfo.BarPopupExpandMode = expandMode;
			}
		}
		#region IMergingSupport Members
		bool IMergingSupport.CanMerge(IMergingSupport second) { return second.GetType().Name.Equals("MDIMenuBar"); }
		bool IMergingSupport.IsMerged { get { return ((ILinksHolder)this).MergedParent != null; } }
		bool IMergingSupport.IsAutomaticallyMerged { get; set; }
		bool IMergingSupport.IsMergedParent(IMergingSupport second) { return ((ILinksHolder)this).MergedParent == second; }
		void IMergingSupport.Merge(IMergingSupport second) { ((ILinksHolder)this).Merge((ILinksHolder)second); }
		object IMergingSupport.MergingKey { get { return typeof(Bar); } }
		void IMergingSupport.Unmerge(IMergingSupport second) { ((ILinksHolder)this).UnMerge((ILinksHolder)second); }
		#endregion        
		#region IMultipleElementRegistratorSupport Members
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if (Equals(typeof(IMergingSupport), registratorKey))
				return MergingPropertiesHelper.MainMenuID;
			throw new ArgumentException();
		}
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(IMergingSupport) }; }
		}
		#endregion   
	}
}
