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
using System;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Core.Design {
	class BarManagerParentAdapter : ParentAdapter {
		public override void Parent(ModelItem newParent, ModelItem child) {
			if(child.IsItemOfType(typeof(BarItem)))
				BarManagerDesignTimeHelper.AddBarItem(newParent, child);
			else if(child.IsItemOfType(typeof(Bar)))
				BarManagerDesignTimeHelper.AddBar(newParent, child);
			else
				newParent.Content.SetValue(child);
		}
		public override ModelItem RedirectParent(ModelItem parent, Type childType) {
			if(parent.Content.Value == null || childType.IsSubclassOf(typeof(BarItem)) || childType.IsSubclassOf(typeof(Bar)))
				return parent;
			return parent.Content.Value;
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			TargetMethod method = null;
			if(child.IsItemOfType(typeof(BarItem))) {
				method = BarManagerDesignTimeHelper.RemoveBarItem;
			} else if(child.IsItemOfType(typeof(Bar))) {
				method = BarManagerDesignTimeHelper.RemoveBar;
			} else currentParent.Content.SetValue(null);
			if(method == null) return;
			using(ModelEditingScope scope = child.Parent.BeginEdit(string.Format("Remove {0}", child.ItemType.Name))) {
				method(child);
				scope.Complete();
			}
		}
		delegate void TargetMethod(ModelItem item);
		public override bool CanParent(ModelItem parent, Type childType) {
			return base.CanParent(parent, childType) || childType.IsSubclassOf(typeof(BarItem)) || childType.IsSubclassOf(typeof(Bar));
		}
	}
}
