﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfFitBBoxVerticallyDestination : PdfDestination {
		internal const string Name = "FitBV";
		readonly double? left;
		public double? Left { get { return left; } }
		public PdfFitBBoxVerticallyDestination(PdfPage page, double? left) : base(page) {
			this.left = left;
		}
		internal PdfFitBBoxVerticallyDestination(PdfDocumentCatalog documentCatalog, object pageObject, double? left) : base(documentCatalog, pageObject) {
			this.left = left;
		}
		PdfFitBBoxVerticallyDestination(PdfFitBBoxVerticallyDestination destination)
			: base(destination) {
			left = destination.left;
		}
		protected internal override PdfDestination CreateDuplicate() {
			return new PdfFitBBoxVerticallyDestination(this);
		}
		protected internal override PdfTarget CreateTarget(IList<PdfPage> pages) {
			return new PdfTarget(PdfTargetMode.FitBBoxVertically, CalculatePageIndex(pages), left, null);
		}
		protected override void AddWriteableParameters(IList<object> parameters) {
			parameters.Add(new PdfName(Name));
			AddParameter(parameters, left);
		}
	}
}
