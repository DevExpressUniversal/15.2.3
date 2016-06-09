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

using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Printing.ExportHelpers {
	internal class PictureExporter<TCol, TRow> : ExportHelperBase<TCol, TRow> 
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public PictureExporter(ExporterInfo<TCol, TRow> exportInfo)  : base(exportInfo){
		}
		delegate void FuncType (int column, int row, int width, int height, IXlPicture picture);
		public void SetPicture(Image picture, int column, int row, int width, int height, XlAnchorType type) {
			FuncType func = null;
			switch(type) {
				case XlAnchorType.OneCell: func = SetOneCellAnchor; break;
				case XlAnchorType.TwoCell: func = SetTwoCellAnchor; break;
				case XlAnchorType.Absolute: func = FitToCell; break;
			}
			ExportImage(picture, column, row, width, height, func);
		}
		void ExportImage(Image exportedImage, int column, int row, int width, int height, FuncType setAction) {
			if(exportedImage != null) {
				IXlPicture picture = ExportInfo.Exporter.BeginPicture();
				Image finalImage = exportedImage;
				picture.Image = finalImage;
				if(setAction != null) setAction(column,row,width,height,picture);
				ExportInfo.Exporter.EndPicture();
			}
		}
		static void SetOneCellAnchor(int column, int row, int width, int height, IXlPicture picture){
			picture.SetOneCellAnchor(new XlAnchorPoint(column, row), width, height);
		}
		static void SetTwoCellAnchor(int column, int row, int width, int height, IXlPicture picture){
			picture.SetTwoCellAnchor(new XlAnchorPoint(column, row), new XlAnchorPoint(width, height), XlAnchorType.TwoCell);
		}
		static void FitToCell(int column, int row, int width, int height, IXlPicture picture){
			picture.FitToCell(new XlCellPosition(column, row), width, height, true);
		}
	}
}
