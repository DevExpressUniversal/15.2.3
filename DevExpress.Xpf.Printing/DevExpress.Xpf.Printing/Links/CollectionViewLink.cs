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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Printing.DataNodes;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.Printing {
	public class CollectionViewLink : TemplatedLink {
		GroupInfoCollection groupInfos;
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("CollectionViewLinkGroupInfos")]
#endif
		public GroupInfoCollection GroupInfos {
			get {
				if(groupInfos == null)
					groupInfos = new GroupInfoCollection();
				return groupInfos;
			}
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("CollectionViewLinkCollectionView")]
#endif
		public ICollectionView CollectionView { get; set; }
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("CollectionViewLinkDetailTemplate")]
#endif
		public DataTemplate DetailTemplate { get; set; }
		public CollectionViewLink()
			: base(string.Empty) {
		}
		protected override void BuildCore() {
			if(CollectionView == null)
				throw new InvalidOperationException("CollectionView is null");
			base.BuildCore();
		}
		protected override IRootDataNode CreateRootNode() {
			return new VisualRootNode(CollectionView, DetailTemplate, GroupInfos);
		}
	}
}
