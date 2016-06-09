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

extern alias Platform;
using Microsoft.Windows.Design.Interaction;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Windows.Design.Policies;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design;
using DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Ribbon;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core.Native;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Core.Design.SmartTags;
namespace DevExpress.Xpf.Ribbon.Design {
	[UsesItemPolicy(typeof(SmartTagAdornerSelectionPolicy))]
	class RibbonControlSelectionAdornerProvider : SelectionAdornerProviderBase {
		public override SelectionBorder CreateSelectionBorder() {
			return new RibbonSelectionBorder();
		}
	}
	class RibbonSelectionBorder : SelectionBorder {
		protected override FrameworkElement GetSelectedElement() {
			if(PrimarySelection == null) return null;
			RibbonViewProvider viewProvider = new RibbonViewProvider();
			return viewProvider.ProvideView(PrimarySelection);
		}
		protected override void OnPrimarySelectionChanged(ModelItem oldSelection) {
			base.OnPrimarySelectionChanged(oldSelection);
			if(PrimarySelection != null && PrimarySelection.IsItemOfType(typeof(RibbonPageGroup))) {
				RibbonControlHelper.ShowRibbonPageGroupInScrollViewer((RibbonPageGroup)PrimarySelection.GetCurrentValue());
			} else if(PrimarySelection != null) {
				ModelItem pageGroup = GetPageGroup(PrimarySelection);
				if(pageGroup != null)
					RibbonControlHelper.ShowRibbonPageGroupInScrollViewer((RibbonPageGroup)pageGroup.GetCurrentValue());
			}
		}
		ModelItem GetPageGroup(ModelItem PrimarySelection) {
			if(PrimarySelection.IsItemOfType(typeof(BarItemLink)))
				return RibbonDesignTimeHelper.GetRibbonPageGroupByBarItemLink(PrimarySelection);
			else return RibbonDesignTimeHelper.FindParent<RibbonPageGroup>(PrimarySelection);
		}
	}
	[UsesItemPolicy(typeof(SelectionParentPolicy))]
	class RibbonControlTaskProvider : DeleteItemTaskProviderBase {
		protected override void OnDeleteKeyPressed(ModelItem selectedItem) {
			base.OnDeleteKeyPressed(selectedItem);
			RemoveItemMethod targetMethod = null;
			if(selectedItem.IsItemOfType(typeof(RibbonPage))) targetMethod = RibbonDesignTimeHelper.RemoveRibbonPage;
			else if(selectedItem.IsItemOfType(typeof(RibbonPageGroup))) targetMethod = RibbonDesignTimeHelper.RemoveRibbonPageGroup;
			else if(selectedItem.IsItemOfType(typeof(RibbonPageCategory))) targetMethod = RibbonDesignTimeHelper.RemoveRibbonPageCategory;
			if(targetMethod == null) return;
			using(ModelEditingScope scope = selectedItem.Parent.BeginEdit(string.Format("Remove {0}", selectedItem.ItemType.Name))) {
				targetMethod(selectedItem);
				scope.Complete();
			}
		}
		delegate void RemoveItemMethod(ModelItem item);
	}
	class RibbonViewProvider : IViewProvider {
		public FrameworkElement ProvideView(ModelItem item) {
			if(item.IsItemOfType(typeof(RibbonPage))) {
				ModelItem ribbon = RibbonDesignTimeHelper.FindParent<RibbonControl>(item);
				if(ribbon == null) return null;
				return (RibbonHeaderVisibility)ribbon.Properties["RibbonHeaderVisibility"].ComputedValue == RibbonHeaderVisibility.Visible ?
					(FrameworkElement)RibbonControlHelper.GetRibbonPageCaptionControl((RibbonPage)item.GetCurrentValue()) :
					RibbonControlHelper.GetRibbonSelectedPageControl((RibbonControl)ribbon.GetCurrentValue());
			} else if(item.IsItemOfType(typeof(RibbonPageGroup))) {
				return RibbonControlHelper.GetRibbonPageGroupControl((RibbonPageGroup)item.GetCurrentValue());
			} else if(item.IsItemOfType(typeof(RibbonPageCategory))) {
				return RibbonControlHelper.GetRibbonPageCategoryHeaderControl((RibbonPageCategory)item.GetCurrentValue());
			}
			return null;
		}
	}
}
