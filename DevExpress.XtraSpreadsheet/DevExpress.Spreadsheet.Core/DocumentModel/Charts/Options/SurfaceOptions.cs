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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SurfaceOptions
	public class SurfaceOptions : ICloneable<SurfaceOptions>, ISupportsCopyFrom<SurfaceOptions> {
		#region Fields
		readonly IChart parent;
		ShapeProperties shapeProperties;
		PictureOptions pictureOptions;
		int thickness;
		#endregion
		public SurfaceOptions(IChart parent) {
			this.parent = parent;
			this.pictureOptions = new PictureOptions(parent);
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.thickness = 0;
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public PictureOptions PictureOptions { get { return pictureOptions; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public int Thickness {
			get { return thickness; }
			set {
				if(thickness == value)
					return;
				ValueChecker.CheckValue(value, 0, 100, "Thickness");
				SetThickness(value);
			}
		}
		#endregion
		void SetThickness(int value) {
			SurfaceOptionsThicknessPropertyChangedHistoryItem historyItem = new SurfaceOptionsThicknessPropertyChangedHistoryItem(DocumentModel, this, thickness, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetThicknessCore(int value) {
			this.thickness = value;
			Parent.Invalidate();
		}
		#region ICloneable<SurfaceOptions> Members
		public SurfaceOptions Clone() {
			SurfaceOptions result = new SurfaceOptions(this.parent);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<SurfaceOptions> Members
		public void CopyFrom(SurfaceOptions value) {
			Guard.ArgumentNotNull(value, "value");
			this.thickness = value.thickness;
			this.shapeProperties.CopyFrom(value.shapeProperties);
			this.pictureOptions.CopyFrom(value.pictureOptions);
		}
		#endregion
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
		}
	}
	#endregion
}
