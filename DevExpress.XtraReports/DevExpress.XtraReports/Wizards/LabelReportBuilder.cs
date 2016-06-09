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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.Data.XtraReports.Labels;
namespace DevExpress.XtraReports.Wizards {
	public class LabelReportBuilder : ReportBuilderBase {
		LabelInfo labelInfo;
		PaperKindList paperKindList;
		public LabelReportBuilder(XtraReport report, IComponentFactory componentFactory, LabelInfo labelInfo, PaperKindList paperKindList)
			: base(report, componentFactory) {
			this.labelInfo = labelInfo;
			this.paperKindList = paperKindList;
		}
		protected override void ExecuteCore() {
			Report.PaperKind = paperKindList.PaperKind;
			Report.Landscape = paperKindList.Landscape;
			Report.RollPaper = paperKindList.IsRollPaper;
			if(Report.PaperKind == PaperKind.Custom)
				Report.PageSize = paperKindList.SizeInReportUnits;
			float labelDpi = GraphicsDpi.UnitToDpi(labelInfo.Unit);
			int top = (int)XRConvert.Convert(labelInfo.TopMargin, labelDpi, Report.Dpi);
			int left = (int)XRConvert.Convert(labelInfo.LeftMargin, labelDpi, Report.Dpi);
			Report.Margins = new Margins(left, 0, top, 0);
			float labelWidth = XRConvert.Convert(labelInfo.LabelWidth, labelDpi, Report.Dpi);
			float labelHeight = XRConvert.Convert(labelInfo.LabelHeight, labelDpi, Report.Dpi);
			DetailBand detail = GetBandByType(typeof(DetailBand)) as DetailBand;
			XRPanel panel = (XRPanel)CreateComponent(typeof(XRPanel));
			panel.CanGrow = false;
			detail.Controls.Add(panel);
			panel.WidthF = labelWidth;
			panel.HeightF = labelHeight;
			panel.Borders = BorderSide.All;
			detail.HeightF = XRConvert.Convert(labelInfo.VPitch, labelDpi, Report.Dpi);
			Report.ReportPrintOptions.DetailCountOnEmptyDataSource = labelInfo.CalculateLabelCount(paperKindList.Size, paperKindList.UnitDpi);
			if(labelInfo.MoreOneColumnOnPage(paperKindList.Size, paperKindList.UnitDpi)) {
				detail.MultiColumn.ColumnWidth = XRConvert.Convert(labelInfo.LabelWidth, labelDpi, Report.Dpi);
				detail.MultiColumn.ColumnSpacing = XRConvert.Convert(labelInfo.HPitch - labelInfo.LabelWidth, labelDpi, Report.Dpi);
				detail.MultiColumn.Layout = ColumnLayout.AcrossThenDown;
			}
		}
	}
	public class PaperKindList : ArrayList {
		int currentIndex;
		int currentID = 1;
		float reportDpi;
		PaperKind RealPaperKind { get { return this[currentIndex].PaperKind; } }
		#region static
		static Dictionary<PaperKind, PaperKind> rotatedPaperKindsHashTable;
		static PaperKindList() {
			GetRotatedPaperKinds();
		}
		static PaperKind UnRotatedPaperKind(PaperKind paperKind) {
			PaperKind resultPaperKind;
			if(rotatedPaperKindsHashTable.TryGetValue(paperKind, out resultPaperKind))
				return resultPaperKind;
			return paperKind;
		}
		static void GetRotatedPaperKinds() {
			rotatedPaperKindsHashTable = new Dictionary<PaperKind, PaperKind>();
			rotatedPaperKindsHashTable.Add(PaperKind.A3Rotated, PaperKind.A3);
			rotatedPaperKindsHashTable.Add(PaperKind.A4Rotated, PaperKind.A4);
			rotatedPaperKindsHashTable.Add(PaperKind.A5Rotated, PaperKind.A5);
			rotatedPaperKindsHashTable.Add(PaperKind.A6Rotated, PaperKind.A6);
			rotatedPaperKindsHashTable.Add(PaperKind.B5JisRotated, PaperKind.B5Transverse);
			rotatedPaperKindsHashTable.Add(PaperKind.B6JisRotated, PaperKind.B6Jis);
			rotatedPaperKindsHashTable.Add(PaperKind.JapaneseDoublePostcardRotated, PaperKind.JapaneseDoublePostcard);
			rotatedPaperKindsHashTable.Add(PaperKind.JapanesePostcardRotated, PaperKind.JapanesePostcard);
			rotatedPaperKindsHashTable.Add(PaperKind.LetterRotated, PaperKind.Letter);
			rotatedPaperKindsHashTable.Add(PaperKind.Prc16KRotated, PaperKind.Prc16K);
			rotatedPaperKindsHashTable.Add(PaperKind.Prc32KRotated, PaperKind.Prc32K);
			rotatedPaperKindsHashTable.Add(PaperKind.Prc32KBigRotated, PaperKind.Prc32KBig);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber1Rotated, PaperKind.PrcEnvelopeNumber1);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber10Rotated, PaperKind.PrcEnvelopeNumber10);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber2Rotated, PaperKind.PrcEnvelopeNumber2);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber3Rotated, PaperKind.PrcEnvelopeNumber3);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber4Rotated, PaperKind.PrcEnvelopeNumber4);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber5Rotated, PaperKind.PrcEnvelopeNumber5);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber6Rotated, PaperKind.PrcEnvelopeNumber6);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber7Rotated, PaperKind.PrcEnvelopeNumber7);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber8Rotated, PaperKind.PrcEnvelopeNumber8);
			rotatedPaperKindsHashTable.Add(PaperKind.PrcEnvelopeNumber9Rotated, PaperKind.PrcEnvelopeNumber9);
		}
		#endregion
		public new PaperKindItem this[int index] { get { return (PaperKindItem)base[index]; } }
		public int CurrentIndex {
			get { return currentIndex; }
			set {
				if(value < 0 || value >= Count) return;
				currentIndex = value;
			}
		}
		public int CurrentID {
			get { return currentID; }
			set {
				if(value <= 0) return;
				for(int i = 0; i < Count; i++)
					if(this[i].ID == value) {
						currentID = value;
						currentIndex = i;
						return;
					}
			}
		}
		public float ReportDpi { get { return reportDpi; } }
		public string PaperName { get { return this[currentIndex].Name; } }
		public string PaperSizeText { get { return this[currentIndex].SizeText; } }
		public SizeF Size { get { return this[currentIndex].Size; } }
		public Size SizeInReportUnits { get { return this[currentIndex].SizeInReportUnits; } }
		public float UnitDpi { get { return this[currentIndex].UnitDpi; } }
		public bool IsRollPaper { get { return this[currentIndex].IsRollPaper; } }
		public PaperKind PaperKind { get { return UnRotatedPaperKind(RealPaperKind); } }
		public bool Landscape { get { return RealPaperKind != this.PaperKind; } }
		public PaperKindList(float reportDpi) {
			this.reportDpi = reportDpi;
		}
		public void Add(PaperKindItem paperKind) {
			if(paperKind == null) return;
			paperKind.Owner = this;
			base.Add(paperKind);
		}
	}
	public class PaperKindItem {
		PaperKind paperKind;
		int id;
		string name;
		SizeF size;
		PaperKindList owner;
		float unitDpi;
		bool isRollPaper;
		internal PaperKindList Owner { get { return owner; } set { owner = value; } }
		internal int ID { get { return id; } }
		internal PaperKind PaperKind { get { return paperKind; } }
		public string Name { get { return name; } }
		public string SizeText {
			get {
				Size size = SizeInReportUnits;
				return "(" + Convert.ToString(size.Width) + " x " + Convert.ToString(size.Height) + ")";
			}
		}
		public float UnitDpi { get { return unitDpi; } }
		public bool IsRollPaper { get { return isRollPaper; } }
		public PaperKindItem Value { get { return this; } } 
		public SizeF Size { get { return size; } }
		public Size SizeInReportUnits { get { return System.Drawing.Size.Round(XRConvert.Convert(size, unitDpi, Owner.ReportDpi)); } }
		public PaperKindItem(string name, SizeF size, int id, PaperKind paperKind)
			: this(name, size, id, paperKind, GraphicsDpi.HundredthsOfAnInch, false) {
		}
		public PaperKindItem(string name, SizeF size, int id, PaperKind paperKind, float unitDpi, bool isRollPaper) {
			this.name = name;
			this.size = size;
			this.id = id;
			this.paperKind = paperKind;
			this.unitDpi = unitDpi;
			this.isRollPaper = isRollPaper;
		}
	}
	public class LabelInfo {
		#region Fields & Properties
		float labelWidth;
		float labelHeight;
		float hPitch;
		float vPitch;
		float topMargin;
		float leftMargin;
		GraphicsUnit unit;
		public float LabelWidth {
			get { return labelWidth; }
			set { labelWidth = value; }
		}
		public float LabelHeight {
			get { return labelHeight; }
			set { labelHeight = value; }
		}
		public float HPitch {
			get { return hPitch; }
			set { hPitch = value; }
		}
		public float VPitch {
			get { return vPitch; }
			set { vPitch = value; }
		}
		public float TopMargin {
			get { return topMargin; }
			set { topMargin = value; }
		}
		public float LeftMargin {
			get { return leftMargin; }
			set { leftMargin = value; }
		}
		public GraphicsUnit Unit {
			get { return unit; }
			set { unit = value; }
		}
		#endregion
		public void Assign(LabelInfo src) {
			if(src == null)
				return;
			this.labelWidth = src.labelWidth;
			this.labelHeight = src.labelHeight;
			this.hPitch = src.hPitch;
			this.vPitch = src.vPitch;
			this.topMargin = src.topMargin;
			this.leftMargin = src.leftMargin;
			this.unit = src.unit;
		}
		public int CalculateLabelCount(SizeF paperSize, float paperSizeDpi) {
			int labelsCountHorz = LabelWizardHelper.GetLabelsCount(HPitch, LabelWidth, LeftMargin, Unit, paperSize.Width, GraphicsDpi.DpiToUnit(paperSizeDpi));
			int labelsCountVert = LabelWizardHelper.GetLabelsCount(VPitch, LabelHeight, TopMargin, Unit, paperSize.Height, GraphicsDpi.DpiToUnit(paperSizeDpi));
			return labelsCountHorz * labelsCountVert;
		}
		public bool MoreOneColumnOnPage(SizeF paperSize, float paperSizeDpi) {
			int labelsCountHorz = LabelWizardHelper.GetLabelsCount(HPitch, LabelWidth, LeftMargin, Unit, paperSize.Width, GraphicsDpi.DpiToUnit(paperSizeDpi));
			return labelsCountHorz > 1;
		}
	}
}
