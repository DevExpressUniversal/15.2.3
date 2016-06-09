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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Platform::System.Windows.Controls;
using Microsoft.Windows.Design.Metadata;
using Platform::DevExpress.Xpf.Core;
#if SILVERLIGHT
using Platform::System.Windows;
#else
using System.Windows;
#endif
namespace DevExpress.Xpf.Core.Design {
	internal class DXTabItemParentAdapter : ParentAdapter {
		public override void Parent(ModelItem parent, ModelItem child) {
			if(parent == null) throw new ArgumentNullException("parent");
			if(child == null) throw new ArgumentNullException("child");
			using(ModelEditingScope scope = parent.BeginEdit()) {
				parent.Content.SetValue(child);
				scope.Complete();
			}
		}
		public override ModelItem RedirectParent(ModelItem parent, Type childType) {
			if(parent == null) throw new ArgumentNullException("parent");
			if(childType == null) throw new ArgumentNullException("childType");
			if(parent.IsItemOfType(typeof(DXTabItem)) && parent.Parent != null && (!parent.Parent.IsItemOfType(typeof(DXTabControl)))) {
				return parent;
			}
			if(ModelFactory.ResolveType(parent.Context, new TypeIdentifier(typeof(DXTabItem).FullName)).IsAssignableFrom(childType)) {
				return parent;
			}
			if(parent.Content != null && parent.Content.IsSet) {
				return parent.Content.Value;
			}
			return base.RedirectParent(parent, childType);
		}
		public override void RemoveParent(ModelItem currentParent, ModelItem newParent, ModelItem child) {
			if(currentParent == null) throw new ArgumentNullException("currentParent");
			if(newParent == null) throw new ArgumentNullException("newParent");
			if(child == null) throw new ArgumentNullException("child");
			currentParent.Content.ClearValue();
		}
	}
}
