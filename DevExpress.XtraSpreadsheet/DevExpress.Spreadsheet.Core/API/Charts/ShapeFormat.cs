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
using System.ComponentModel;
using DevExpress.Office;
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public interface ShapeFormat {
		ShapeFill Fill { get; }
		ShapeOutline Outline { get; }
		void ResetToMatchStyle();
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	partial class NativeShapeFormat : NativeObjectBase, ShapeFormat {
		#region Fields
		readonly Model.ShapeProperties modelShapeProperties;
		readonly NativeWorkbook nativeWorkbook;
		NativeShapeFill nativeFill;
		NativeShapeOutline nativeOutline;
		#endregion
		public NativeShapeFormat(Model.ShapeProperties modelShapeProperties, NativeWorkbook nativeWorkbook) {
			this.modelShapeProperties = modelShapeProperties;
			this.nativeWorkbook = nativeWorkbook;
		}
		protected NativeWorkbook NativeWorkbook { get { return nativeWorkbook; } }
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (nativeFill != null)
				nativeFill.IsValid = value;
			if (nativeOutline != null)
				nativeOutline.IsValid = value;
		}
		#region ShapeFormat Members
		public ShapeFill Fill {
			get {
				CheckValid();
				if (nativeFill == null)
					nativeFill = new NativeShapeFill(modelShapeProperties, nativeWorkbook);
				return nativeFill;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				if (nativeOutline == null)
					nativeOutline = new NativeShapeOutline(modelShapeProperties.Outline, nativeWorkbook);
				return nativeOutline;
			}
		}
		public virtual void ResetToMatchStyle() {
			CheckValid();
			modelShapeProperties.ResetToStyle();
		}
		#endregion
	}
}
