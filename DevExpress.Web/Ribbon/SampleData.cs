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
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class RibbonHierarchicalSampleData : HierarchicalSampleData {
		public RibbonHierarchicalSampleData(int depth, string path)
			: base(depth, path) {
		}
		protected override void CreateSampleNodes(int depth, string path) {
			if(depth == 0) { 
				for(var i = 1; i < 4; i++) {
					List.Add(new RibbonHierarchicalSampleDataNode(string.Format("Tab {0}", i), depth, path));
				}
			}
			else if(depth == 1) { 
				for(var i = 1; i < 4; i++) {
					List.Add(new RibbonHierarchicalSampleDataNode(string.Format("Group {0}", i), depth, path));
				}
			}
		}
	}
	public class RibbonHierarchicalSampleDataNode : HierarchicalSampleDataNode {
		public override string NavigateUrl {
			get { return (Depth > 1) ? base.NavigateUrl : ""; } 
		}
		public RibbonHierarchicalSampleDataNode(string text, int depth, string path)
			: base(text, depth, path) {
		}
		protected override IHierarchicalEnumerable CreateChildren(int depth, string path) {
			return new RibbonHierarchicalSampleData(depth, path);
		}
	}
}
