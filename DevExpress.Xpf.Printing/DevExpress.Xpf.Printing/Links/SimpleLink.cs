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
using DevExpress.Utils;
using DevExpress.XtraPrinting.DataNodes;
#if SL
using DevExpress.Xpf.Printing.Native;
#endif
namespace DevExpress.Xpf.Printing {
	public partial class SimpleLink : TemplatedLink {
		DataTemplate detailTemplate;
		int detailCount;
		public event EventHandler<CreateAreaEventArgs> CreateDetail;
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("SimpleLinkDetailTemplate")]
#endif
		public DataTemplate DetailTemplate {
			get { return detailTemplate; }
			set {
				Guard.ArgumentNotNull(value, "value");
				detailTemplate = value;
			}
		}
#if !SL
	[DevExpressXpfPrintingLocalizedDescription("SimpleLinkDetailCount")]
#endif
		public int DetailCount {
			get { return detailCount; }
			set {
				if(value < 0)
					throw new ArgumentOutOfRangeException("value");
				detailCount = value;
			}
		}
		public SimpleLink(DataTemplate detailTemplate, int detailCount, string documentName)
			: base(documentName) {
			DetailTemplate = detailTemplate;
			DetailCount = detailCount;
		}
		public SimpleLink(DataTemplate detail, int detailCount)
			: this(detail, detailCount, string.Empty) {
		}
		public SimpleLink(string documentName)
			: base(documentName) {
		}
		public SimpleLink()
			: this(string.Empty) {
		}
		protected override IRootDataNode CreateRootNode() {
			return new PlainDataRootNode(this, DetailCount);
		}
		void RaiseCreateDetail(CreateAreaEventArgs e) {
			if(CreateDetail != null) {
				CreateDetail(this, e);
			}
		}
	}
}
