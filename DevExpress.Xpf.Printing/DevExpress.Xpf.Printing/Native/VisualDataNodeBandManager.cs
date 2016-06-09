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
using System.Windows;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
namespace DevExpress.Xpf.Printing.Native {
	class VisualDataNodeBandManager : DataNodeBandManager {
		readonly DocumentBandInitializer bandInitializer;
		public readonly int totalDetailCount;
		public VisualDataNodeBandManager(IRootDataNode rootNode, DocumentBandInitializer bandInitializer)
			: base(rootNode) {
			Guard.ArgumentNotNull(bandInitializer, "bandInitializer");
			this.bandInitializer = bandInitializer;
			totalDetailCount = rootNode.GetTotalDetailCount();
		}
		protected override DocumentBand CreateDetailBand(IDataNode node) {
			DetailDocumentBand docBand = new DetailDocumentBand();
			bandInitializer.Initialize(docBand, ((IVisualDetailNode)node).GetDetail, node.Index);
			docBand.Scale(rootBand.PrintingSystem.Document.ScaleFactor, null);
			if(totalDetailCount != -1) {
				rootBand.PrintingSystem.ProgressReflector.RangeValue++;
			}
			return docBand;
		}
		protected override DocumentBand CreateGroupHeaderBand(IGroupNode node) {
			DocumentBand docBand = new DocumentBand(DocumentBandKind.Header);
			bandInitializer.Initialize(docBand, ((IVisualGroupNode)node).GetHeader, node.Index);
			docBand.Scale(rootBand.PrintingSystem.Document.ScaleFactor, null);
			return docBand;
		}
		protected override DocumentBand CreateGroupFooterBand(IGroupNode node) {
			DocumentBand docBandFooter = new DocumentBand(DocumentBandKind.Footer);
			bandInitializer.Initialize(docBandFooter, ((IVisualGroupNode)node).GetFooter, node.Index);
			if(!(node is IVisualGroupNodeFixedFooter)) {
				docBandFooter.Scale(rootBand.PrintingSystem.Document.ScaleFactor, null);
				return docBandFooter;
			}
			DocumentBand footerDocBandRoot = new DocumentBand(DocumentBandKind.Footer);
			footerDocBandRoot.AddBand(docBandFooter);
			DocumentBand docBandFooterFixed = new DocumentBand(DocumentBandKind.Footer);
			bandInitializer.Initialize(docBandFooterFixed, ((IVisualGroupNodeFixedFooter)node).GetFixedFooter, node.Index);
			footerDocBandRoot.AddBand(docBandFooterFixed);
			footerDocBandRoot.Scale(rootBand.PrintingSystem.Document.ScaleFactor, null);
			return footerDocBandRoot;
		}
		public override void Initialize(RootDocumentBand rootBand) {
			if(rootBand == null)
				throw new ArgumentNullException("rootBand");
			base.Initialize(rootBand);
			if(totalDetailCount != -1) {
				rootBand.PrintingSystem.ProgressReflector.InitializeRange(totalDetailCount);
			}
		}
		protected override void OnEndOfData() {
			base.OnEndOfData();
			rootBand.PrintingSystem.ProgressReflector.MaximizeRange();
		}
	}
}
