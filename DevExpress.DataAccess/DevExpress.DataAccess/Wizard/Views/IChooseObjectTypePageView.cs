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
namespace DevExpress.DataAccess.Wizard.Views {
	public interface IChooseObjectTypePageView {
		TypeViewInfo SelectedItem { get; set; }
		bool ShowAll { get; }
		void Initialize(IEnumerable<TypeViewInfo> items, bool showAll);
		event EventHandler Changed;
	}
	public sealed class TypeViewInfo {
		public enum NodeType {
			Namespace = 0,
			Class = 1,
			Interface = 2,
			StaticClass = 3,
			AbstractClass = 4
		}
		public TypeViewInfo(bool highlighted, string ns, string typeName, NodeType classType) : this(highlighted, ns, typeName, classType, null) { }
		public TypeViewInfo(bool highlighted, string ns, string typeName, NodeType classType, IEnumerable<TypeViewInfo> nested) {
			TypeName = typeName;
			ClassType = classType;
			Namespace = ns;
			Highlighted = highlighted;
			if(nested != null) {
				Nested = nested.Select(item => {
					item.Parent = this;
					return item;
				}).ToArray();
			}
		}
		public bool Highlighted { get; private set; }
		public string Namespace { get; private set; }
		public string TypeName { get; private set; }
		public NodeType ClassType { get; private set; }
		public TypeViewInfo Parent { get; private set; }
		public TypeViewInfo[] Nested { get; private set; }
	}
}
