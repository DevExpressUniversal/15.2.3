﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	class BarParentAdapter : ParentAdapter {
		public override void Parent(ModelItem newParent, ModelItem child) {
			ModelItem barManager = BarManagerDesignTimeHelper.FindBarManagerInParent(newParent);
			if(barManager == null) return;
			ModelItem childLink = null;
			if(child.IsItemOfType(typeof(BarItem))) {
				childLink = BarManagerDesignTimeHelper.CreateBarItemLink(child);
				BarManagerDesignTimeHelper.AddBarItem(barManager, child);
			} else if(child.IsItemOfType(typeof(BarItemLink)))
				childLink = child;
			BarManagerDesignTimeHelper.AddBarItemLink(newParent.Properties["ItemLinks"], childLink);
			SelectionHelper.SetSelection(newParent.Context, childLink);
			barManager.View.UpdateLayout();
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			BarManagerDesignTimeHelper.RemoveBarItemLink(child);
		}
		public override bool CanParent(ModelItem parent, Type childType) {
			return childType.IsSubclassOf(typeof(BarItem)) || childType.IsSubclassOf(typeof(BarItemLinkBase));
		}
	}
}
