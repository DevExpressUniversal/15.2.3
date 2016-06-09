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
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
namespace DevExpress.XtraPrinting.Native {
	public interface ISubreportDocumentBand {
		ILayoutData CreateLayoutData(LayoutDataContext layoutContext, RectangleF bounds); 
		RectangleF ReportRect { get; set; }
	}
	public class DocumentBandContainer : DocumentBand {
		bool containsDetailBands = true;
		public override object GroupKey {
			get;
			set;
		}
		public DocumentBandContainer(bool containsDetailBands) : this() {
			this.containsDetailBands = containsDetailBands;
		}
		public DocumentBandContainer()
			: base(DocumentBandKind.Storage | DocumentBandKind.Detail, 0) {
		}
		DocumentBandContainer(DocumentBandContainer source, int rowIndex)
			: base(source, rowIndex) {
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new DocumentBandContainer(this, rowIndex);
		}
		public override MultiColumn MultiColumn {
			get {
				MultiColumn mc = GetDataSourceRoot().MultiColumn;
				if(mc != null && mc.Order == ColumnLayout.AcrossThenDown)
					return mc;
				return null;
			}
		}
		public override bool ContainsDetailBands() {
			return containsDetailBands;
		}
	}
	public class DetailDocumentBand : DocumentBand {
		public DetailDocumentBand()
			: base(DocumentBandKind.Detail) {
		}
		public DetailDocumentBand(int rowIndex)
			: base(DocumentBandKind.Detail, rowIndex) {
		}
		DetailDocumentBand(DetailDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
		}
		public override bool IsDetailBand {
			get { return true; }
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new DetailDocumentBand(this, rowIndex);
		}
	}
	public class MarginDocumentBand : DocumentBand {
		float height;
		public override float TotalHeight { get { return height; } }
		public override float SelfHeight { get { return Bricks.Count > 0 ? height : 0; } }
		protected internal override float Bottom { get { return Bricks.Count > 0 ? Top + height : 0; } }
		public MarginDocumentBand(DocumentBandKind kind, float height)
			: base(kind) {
			if(!this.IsKindOf(DocumentBandKind.TopMargin, DocumentBandKind.BottomMargin))
				throw new ArgumentException("kind");			
			this.height = height;
			RepeatEveryPage = true;
		}
		protected MarginDocumentBand(MarginDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
			this.height = source.height;
		}
		internal void SetHeight(float height) {
			this.height = height;
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new MarginDocumentBand(this, rowIndex);
		}
	}
}
