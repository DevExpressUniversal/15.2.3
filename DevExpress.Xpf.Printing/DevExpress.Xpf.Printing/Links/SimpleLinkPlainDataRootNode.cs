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
using System.Windows;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing {
	public partial class SimpleLink : TemplatedLink {
		class PlainDataRootNode : IRootDataNode {
			readonly int detailCount;
			readonly Dictionary<int, Pair<DataTemplate, object>> details;
			public SimpleLink Link { get; private set; }
			public RowViewInfo GetDetail(int index) {
				return Link.DetailTemplate != null ? new RowViewInfo(Link.DetailTemplate, details[index].Second) : null;
			}
			public PlainDataRootNode(SimpleLink link, int detailCount) {
				Link = link;
				this.detailCount = detailCount;
				details = new Dictionary<int, Pair<DataTemplate, object>>(detailCount);
			}
			#region IDataNode Members
			public bool CanGetChild(int index) {
				CreateAreaEventArgs args = new CreateAreaEventArgs(index);
				if(index >= detailCount)
					return false;
				Link.RaiseCreateDetail(args);
				details[index] = new Pair<DataTemplate, object>(Link.DetailTemplate, args.Data);
				return true;
			}
			public IDataNode GetChild(int index) {
				return new PlainDataDetailNode(this, index);
			}
			public int Index {
				get { throw new NotSupportedException(); }
			}
			public bool IsDetailContainer {
				get { return true; }
			}
			public IDataNode Parent {
				get { return null; }
			}
			public bool PageBreakBefore { get { return false; } }
			public bool PageBreakAfter { get { return false; } }
			#endregion
			#region IRootDataNode Members
			public int GetTotalDetailCount() {
				return detailCount;
			}
			#endregion
		}
	}
}
