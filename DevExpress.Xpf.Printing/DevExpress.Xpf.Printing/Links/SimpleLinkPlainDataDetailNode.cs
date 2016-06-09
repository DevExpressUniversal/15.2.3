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
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Printing {
	public partial class SimpleLink : TemplatedLink {
		class PlainDataDetailNode : IVisualDetailNode {
			readonly PlainDataRootNode root;
			public PlainDataDetailNode(PlainDataRootNode root, int index) {
				this.root = root;
				Index = index;
			}
			#region IVisualDetailNode Members
			public RowViewInfo GetDetail(bool allowContentReuse) {
				return root.GetDetail(Index);
			}
			#endregion
			#region IDataNode Members
			public bool CanGetChild(int index) {
				return false;
			}
			public IDataNode GetChild(int index) {
				throw new NotSupportedException();
			}
			public int Index { get; private set; }
			public bool IsDetailContainer {
				get { return false; }
			}
			public IDataNode Parent {
				get { return root; }
			}
			public bool PageBreakBefore { get { return false; } }
			public bool PageBreakAfter { get { return false; } }
			#endregion
		}
	}
}
