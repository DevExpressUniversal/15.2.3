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
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public class BorderShadingFormController : FormController {
		SelectedCellsCollection selectedCells;
		DocumentModel documentModel;
		IRichEditControl richEditControl;
		int gridWidth;
		public BorderShadingFormController (IRichEditControl iRichEditControl, DocumentModel documentModel, SelectedCellsCollection selectedCells) {
			this.selectedCells = selectedCells;
			this.documentModel = documentModel;
			this.richEditControl = iRichEditControl;
			SelectedCellsIntervalInRow firstInterval = selectedCells[0];
			SelectedCellsIntervalInRow lastInterval = selectedCells[selectedCells.RowsCount-1];
			BorderLineUp = firstInterval.StartCell.GetActualTopCellBorder().Info;
			BorderLineDown = lastInterval.StartCell.GetActualBottomCellBorder().Info;
			BorderLineHorizontalIn = firstInterval.StartCell.GetActualBottomCellBorder().Info;
			BorderLineLeft = firstInterval.StartCell.GetActualLeftCellBorder().Info;
			BorderLineRight = firstInterval.EndCell.GetActualRightCellBorder().Info;
			BorderLineVerticalIn = firstInterval.StartCell.GetActualRightCellBorder().Info;
			for (int j = 0; j < selectedCells.RowsCount; j++) {
				if ((selectedCells[j].EndCellIndex - selectedCells[j].StartCellIndex+1) >= 2) {
					BorderLineVerticalInVisible = true;
					break;
				}
			}
			if (selectedCells.RowsCount >= 2)
				BorderLineHorizontalInVisible = true;
			else
				BorderLineHorizontalInVisible = false;
		   FillColor = firstInterval.StartCell.BackgroundColor;
			for (int j = 0; j < selectedCells.RowsCount; j++) {
				for (int i = selectedCells[j].StartCellIndex; i <= selectedCells[j].EndCellIndex; i++) {
					if (selectedCells[j].Row.Cells[i].BackgroundColor != FillColor) {
						FillColor = null;
						break;
					}
				}
			}
			for (int i = firstInterval.StartCellIndex; i <= firstInterval.EndCellIndex; i++) {
				if (!firstInterval.Row.Cells[i].GetActualTopCellBorder().Info.Equals(BorderLineUp)) {
					BorderLineUp = null;
					break;
				}
			}
			for (int i = lastInterval.StartCellIndex; i <= lastInterval.EndCellIndex; i++) {
				if (!lastInterval.Row.Cells[i].GetActualBottomCellBorder().Info.Equals(BorderLineDown)) {
					BorderLineDown = null;
					break;
				}
			}
			for (int j = 0; j < selectedCells.RowsCount-1; j++) {
				for (int i = selectedCells[j].StartCellIndex; i <= selectedCells[j].EndCellIndex; i++) {
					if ((!selectedCells[j].Row.Cells[i].GetActualBottomCellBorder().Info.Equals(BorderLineHorizontalIn))) {
						BorderLineHorizontalIn = null;
						break;
					}
				}
			}
			for (int i = 0; i < selectedCells.RowsCount; i++) {
				if (!selectedCells[i].StartCell.GetActualLeftCellBorder().Info.Equals(BorderLineLeft)) {
					BorderLineLeft = null;
					break;
				}
			}
			for (int i = 0; i < selectedCells.RowsCount; i++) {
				if (!selectedCells[i].EndCell.GetActualRightCellBorder().Info.Equals(BorderLineRight)) {
					BorderLineRight = null;
					break;
				}
			}
			for (int j = 0; j < selectedCells.RowsCount; j++) {
				for (int i = selectedCells[j].StartCellIndex; i < selectedCells[j].EndCellIndex; i++) {
					if ((!selectedCells[j].Row.Cells[i].GetActualRightCellBorder().Info.Equals(BorderLineVerticalIn))) {
						BorderLineVerticalIn = null;
						break;
					}
				}
			}
			gridWidth = documentModel.UnitConverter.TwipsToModelUnits(15);
			SetModeButton = GetModeState();
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public IRichEditControl RichEditControl { get { return richEditControl; }  }
		public SetModeButtons SetModeButton { get; set; }
		public BorderInfo BorderLineUp { get; set; }
		public BorderInfo BorderLineHorizontalIn { get; set; }
		public BorderInfo BorderLineDown { get; set; }
		public BorderInfo BorderLineLeft { get; set; }
		public BorderInfo BorderLineRight { get; set; }
		public BorderInfo BorderLineVerticalIn { get; set; }
		public bool BorderLineHorizontalInVisible { get; set; }
		public bool BorderLineVerticalInVisible { get; set; }			   
		public Color? FillColor { get; set; } 
		public override void ApplyChanges() {
			documentModel.BeginUpdate();
			if (BorderLineUp != null) 
				SetBorders(new ToggleTableCellsTopBorderCommand(richEditControl), BorderLineUp);
			if (BorderLineDown != null)				
				SetBorders(new ToggleTableCellsBottomBorderCommand(richEditControl), BorderLineDown);
			if (BorderLineHorizontalIn != null) 
				SetBorders(new ToggleTableCellsInsideHorizontalBorderCommand(richEditControl), BorderLineHorizontalIn);  
			if (BorderLineLeft != null)
				SetBorders(new ToggleTableCellsLeftBorderCommand(richEditControl), BorderLineLeft);
			if (BorderLineRight != null)
				SetBorders(new ToggleTableCellsRightBorderCommand(richEditControl), BorderLineRight);
			if (BorderLineVerticalIn != null)
				SetBorders(new ToggleTableCellsInsideVerticalBorderCommand(richEditControl), BorderLineVerticalIn);
			if (FillColor != null)
				SetFillColor();
			documentModel.EndUpdate();
		}
	   void SetFillColor() {
		   for (int j = 0; j < selectedCells.RowsCount; j++) {
			   for (int i = selectedCells[j].StartCellIndex; i <= selectedCells[j].EndCellIndex; i++) {
				   selectedCells[j].Row.Cells[i].BackgroundColor = FillColor.Value;				   
			   }
		   }  
		}
		void SetBorders(ToggleTableCellsBordersCommandBase command, BorderInfo border) {
			command.NewBorder = border;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			state.Enabled = true;
			command.ForceExecute(state);
		}
		public BorderInfo GetInitialBorder() {
			BorderInfo[] borders = { BorderLineUp, BorderLineRight, BorderLineDown, BorderLineLeft, BorderLineVerticalIn, BorderLineHorizontalIn };
			for (int i = 0; i < borders.Length; i++) {
				if (borders[i] != null && borders[i].Style != BorderLineStyle.None)
					return borders[i];
			}
			return DocumentModel.TableBorderInfoRepository.CurrentItem;
		}
		public Color GetActiveColor() {
			if (FillColor == null)
				return DXColor.Empty;
			else
				return FillColor.Value;
		}
		public enum SetModeButtons { None, Box, All, Grid, Custom }
		SetModeButtons GetModeState() {
			if (IsModeStateNone()) 
				return SetModeButtons.None;
			if (IsModeStateAll()) 
				return SetModeButtons.All;
			if (IsModeStateBox()) 
				return SetModeButtons.Box;
			if (IsModeStateGrid()) 
				return SetModeButtons.Grid;		   
			return SetModeButtons.Custom; 
		}
		bool IsModeStateNone() {			
			BorderInfo[] borders = { BorderLineUp, BorderLineRight, BorderLineDown, BorderLineLeft, BorderLineVerticalIn, BorderLineHorizontalIn };
			for (int i = 0; i < borders.Length; i++) {
				if (borders[i] != null && borders[i].Style != BorderLineStyle.None)
					return false;
			}
			return true;
		}
		bool AreEqual(BorderInfo borderFirst, BorderInfo borderSecond) {
			if ((borderFirst.Style == borderSecond.Style) && (borderFirst.Color == borderSecond.Color) && (borderFirst.Width == borderSecond.Width))
				return true;
			else 
				return false;
		}
		bool IsModeStateAll() {
			BorderInfo[] borders = { BorderLineUp, BorderLineRight, BorderLineDown, BorderLineLeft, BorderLineVerticalIn, BorderLineHorizontalIn };
			if ((borders[0] == null) || (borders[0].Style == BorderLineStyle.None))
			   return false;
			if ((!BorderLineVerticalInVisible) && (!BorderLineHorizontalInVisible))
				return false;
			for (int i = 1; i < borders.Length; i++) {
				if ((borders[i] == null) || !AreEqual(borders[0], borders[i]))
					return false;
			   }			  
			return true;
		}
		bool IsModeStateBox() {
			BorderInfo[] borders = { BorderLineUp, BorderLineRight, BorderLineDown, BorderLineLeft };
			BorderInfo[] bordersIn = { BorderLineVerticalIn, BorderLineHorizontalIn };
			if ((borders[0] == null) || (borders[0].Style == BorderLineStyle.None)) 
				return false;
			for (int i = 1; i < borders.Length; i++) {
				if ((borders[i] == null) || !AreEqual(borders[0], borders[i]))
					return false;
			}
			if ((!BorderLineVerticalInVisible) && (!BorderLineHorizontalInVisible))
				return true;
			for (int i = 0; i < bordersIn.Length; i++){
				if ((bordersIn[i] == null) || (bordersIn[i].Style != BorderLineStyle.None))
					return false;
			}
			return true;
		}
		bool IsModeStateGrid() {
			BorderInfo[] borders = { BorderLineUp, BorderLineRight, BorderLineDown, BorderLineLeft };
			BorderInfo[] bordersIn = { BorderLineVerticalIn, BorderLineHorizontalIn };
			if ((borders[0] == null) || (borders[0].Style == BorderLineStyle.None)) 
				return false;
			if ((!BorderLineVerticalInVisible) && (!BorderLineHorizontalInVisible))
				return false;
			for (int i = 1; i < borders.Length; i++) {
				if ((borders[i] == null) || !AreEqual(borders[0], borders[i]))
					return false;
			} 
			foreach(BorderInfo border in bordersIn){
				if ((border == null) || (border.Style != BorderLineStyle.Single) || (border.Color != borders[0].Color) || (border.Width != gridWidth))
				   return false;
			}
			return true;
		}	   
	}
}
