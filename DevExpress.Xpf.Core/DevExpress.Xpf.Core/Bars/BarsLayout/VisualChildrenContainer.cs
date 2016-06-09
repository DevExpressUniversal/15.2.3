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
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Bars.Internal {
	public class VisualChildrenContainer : IVisualChildrenContainer {
		IVisualOwner VisualOwner { get; set; }
		ILogicalOwner LogicalOwner { get; set; }
		List<Visual> Children { get; set; }
		public VisualChildrenContainer(IVisualOwner visualOwner, ILogicalOwner logicalOwner = null) {
			VisualOwner = visualOwner;
			LogicalOwner = logicalOwner;
			Children = new List<Visual>();
		}
		public IEnumerator GetEnumerator() {
			return Children.GetEnumerator();
		}
		public int VisualChildrenCount { get { return Children.Count; } }
		public Visual GetVisualChild(int index) {
			return Children[index];
		}
		public void AddChild(Visual visual) {
			VisualOwner.AddChild(visual);
			LogicalOwner.Do(x => x.AddChild(visual));
			Children.Add(visual);
		}
		public void RemoveChild(Visual visual) {
			RemoveChildInternal(visual);
			Children.Remove(visual);
		}
		void RemoveChildInternal(Visual visual) {
			VisualOwner.RemoveChild(visual);
			LogicalOwner.Do(x => x.RemoveChild(visual));
		}
		public void Clear() {
			foreach (var child in Children)
				RemoveChildInternal(child);
			Children.Clear();
		}
	}
}
