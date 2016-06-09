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
using System.Linq.Expressions;
using DevExpress.Xpf.Core;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Drawing.Printing;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class PageSetupViewModel : INotifyPropertyChanged {
		#region Inner Classes
		public class Margins {
			public float Left { get; private set; }
			public float Top { get; private set; }
			public float Right { get; private set; }
			public float Bottom { get; private set; }
			public Margins(float left, float top, float right, float bottom) {
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
		}
		#endregion
		#region Fields and Properties
		readonly IEnumerable<PaperKind> paperKinds;
		readonly Margins minMargins;
		PaperKind paperKind;
		float leftMargin;
		float topMargin;
		float rightMargin;
		float bottomMargin;
		bool landscape;
		public PaperKind PaperKind {
			get { return paperKind; }
			set {
				paperKind = value;
				RaisePropertyChanged(() => PaperKind);
			}
		}
		public IEnumerable<PaperKind> PaperKinds {
			get { return paperKinds; }
		}
		public bool Landscape {
			get { return landscape; }
			set {
				landscape = value;
				RaisePropertyChanged(() => Landscape);
			}
		}
		public float LeftMargin {
			get { return InchesToMarginsUnit(leftMargin); }
			set {
				var leftMarginTmp = MarginsUnitToInches(value);
				if(leftMarginTmp >= minMargins.Left && leftMargin + leftMarginTmp < PageSizeInInches.Width) {
					leftMargin = leftMarginTmp;
					RaisePropertyChanged(() => LeftMargin);
				}
			}
		}
		public float TopMargin {
			get { return InchesToMarginsUnit(topMargin); }
			set {
				var topMarginTmp = MarginsUnitToInches(value);
				if(topMarginTmp >= minMargins.Top && leftMargin + topMarginTmp < PageSizeInInches.Height) {
					topMargin = topMarginTmp;
					RaisePropertyChanged(() => TopMargin);
				}
			}
		}
		public float RightMargin {
			get { return InchesToMarginsUnit(rightMargin); }
			set {
				var rightMarginTmp = MarginsUnitToInches(value);
				if(rightMarginTmp >= minMargins.Right && leftMargin + rightMarginTmp < PageSizeInInches.Width) {
					rightMargin = rightMarginTmp;
					RaisePropertyChanged(() => RightMargin);
				}
			}
		}
		public float BottomMargin {
			get { return InchesToMarginsUnit(bottomMargin); }
			set {
				var bottomMarginTmp = MarginsUnitToInches(value);
				if(bottomMarginTmp >= minMargins.Bottom && topMargin + bottomMarginTmp < PageSizeInInches.Height) {
					bottomMargin = bottomMarginTmp;
					RaisePropertyChanged(() => BottomMargin);
				}
			}
		}
		public string MarginsDisplayedText {
			get {
				string measuringUnit = GetUsersIsMetric() ? PrintingLocalizer.GetString(PrintingStringId.PageSetupMillimeters) : PrintingLocalizer.GetString(PrintingStringId.PageSetupInches);
				return string.Format(PrintingLocalizer.GetString(PrintingStringId.PageSetupMarginsCaptionFormat), measuringUnit);
			}
		}
		System.Drawing.Size PageSizeInInches {
			get { return PageSizeInfo.GetPageSize(PaperKind, System.Drawing.GraphicsUnit.Inch); }
		}
		float MarginsUnit {
			get { return GetUsersIsMetric() ? GraphicsDpi.Millimeter : GraphicsDpi.Inch; }
		}
		#endregion
		#region ctor
		public PageSetupViewModel() {
			minMargins = new Margins(0, 0, 0, 0);
			paperKinds = GetPaperKinds();
			paperKind = PaperKind.Letter;
		}
		#endregion
		#region Methods
		public Margins GetMargins(float dpi) {
			return new Margins(
				GraphicsUnitConverter.Convert(leftMargin, GraphicsDpi.Inch, dpi),
				GraphicsUnitConverter.Convert(topMargin, GraphicsDpi.Inch, dpi),
				GraphicsUnitConverter.Convert(rightMargin, GraphicsDpi.Inch, dpi),
				GraphicsUnitConverter.Convert(bottomMargin, GraphicsDpi.Inch, dpi)
			);
		}
		public void SetMargins(float left, float top, float right, float bottom, float dpi) {
			LeftMargin = GraphicsUnitConverter.Convert(left, dpi, MarginsUnit);
			TopMargin = GraphicsUnitConverter.Convert(top, dpi, MarginsUnit);
			RightMargin = GraphicsUnitConverter.Convert(right, dpi, MarginsUnit);
			BottomMargin = GraphicsUnitConverter.Convert(bottom, dpi, MarginsUnit);
		}
		internal virtual bool GetUsersIsMetric() {
			return System.Globalization.RegionInfo.CurrentRegion.IsMetric;
		}
		static IEnumerable<PaperKind> GetPaperKinds() {
#if SL
			return PageSizeInfo.AvailablePaperKinds;
#else
			foreach(PaperKind kind in Enum.GetValues(typeof(PaperKind)))
				yield return kind;
#endif
		}
		float InchesToMarginsUnit(float value) {
			return GraphicsUnitConverter.Convert(value, GraphicsDpi.Inch, MarginsUnit);
		}
		float MarginsUnitToInches(float value) {
			return GraphicsUnitConverter.Convert(value, MarginsUnit, GraphicsDpi.Inch);
		}
		#endregion
		#region INotifyPropertyChanged members
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged<T>(Expression<Func<T>> property) {
			PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
		}
		#endregion
	}
}
