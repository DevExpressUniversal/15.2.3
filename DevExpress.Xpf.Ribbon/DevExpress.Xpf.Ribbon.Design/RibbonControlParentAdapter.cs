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
using DevExpress.Xpf.Core.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Ribbon;
namespace DevExpress.Xpf.Ribbon.Design {
	class RibbonControlParentAdapter : ParentAdapter {
		public override void Parent(ModelItem newParent, ModelItem child) {
			using(ModelEditingScope scope = newParent.BeginEdit("Initialize BackstageViewControl")) {
				child.ResetLayout();
				newParent.Properties["RibbonStyle"].SetValue(RibbonStyle.Office2010);
				newParent.Properties["ApplicationMenu"].SetValue(child);
				scope.Complete();
			}
		}
		public override bool CanParent(ModelItem parent, Type childType) {
			return parent.IsItemOfType(typeof(RibbonControl)) && childType == typeof(BackstageViewControl);
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			currentParent.Properties["ApplicationMenu"].ClearValue();
			Parent(newParent, child);
		}
	}
}
