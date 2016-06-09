#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfPanoseFamilyKind { Any = 0, NoFit = 1, LatinText = 2, LatinHandWritten = 3, LatinDecorative = 4, LatinSymbol = 5 }
	public enum PdfPanoseSerifStyle { Any = 0, NoFit = 1, Cove = 2, ObtuseCove = 3, SquareCove = 4, ObtuseSquareCove = 5, Square = 6, Thin = 7, Oval = 8, 
									  Exaggerated = 9, Triangle = 10, NormalSans = 11, ObtuseSans = 12, PerpendicularSans = 13, Flared = 14, Rounded = 15 }
	public enum PdfPanoseWeight { Any = 0, NoFit = 1, VeryLight = 2, Light = 3, Thin = 4, Book = 5, Medium = 6, Demi = 7, Bold = 8, Heavy = 9, Black = 10, ExtraBlack = 11 }
	public enum PdfPanoseProportion { Any = 0, NoFit = 1, OldStyle = 2, Modern = 3, EvenWidth = 4, Extended = 5, Condensed = 6, VeryExtended = 7, VeryCondensed = 8, Monospaced = 9 }
	public enum PdfPanoseContrast { Any = 0, NoFit = 1, None = 2, VeryLow = 3, Low = 4, MediumLow = 5, Medium = 6, MediumHigh = 7, High = 8, VeryHigh = 9 }
	public enum PdfPanoseStrokeVariation { Any = 0, NoFit = 1, NoVariation = 2, GradualDiagonal = 3, GradualTransitional = 4, GradualVertical = 5, 
										   GradualHorizontal = 6, RapidVertical = 7, RapidHorizontal = 8, InstantVertical = 9, InstantHorizontal = 10 }
	public enum PdfPanoseArmStyle { Any = 0, NoFit = 1, StraightArmsHorizontal = 2, StraightArmsWedge = 3, StraightArmsVertical = 4, StraightArmsSingleSerif = 5, StraightArmsDoubleSerif = 6, 
									NonStraightHorizontal = 7, NonStraightWedge = 8, NonStraightVertical = 9, NonStraightSingleSerif = 10, NonStraightDoubleSerif = 11 }
	public enum PdfPanoseLetterform { Any = 0, NoFit = 1, NormalContact = 2, NormalWeighted = 3, NormalBoxed = 4, NormalFlattened = 5, NormalRounded = 6, NormalOffCenter = 7, NormalSquare = 8, 
									  ObliqueContact = 9, ObliqueWeighted = 10, ObliqueBoxed = 11, ObliqueFlattened = 12, ObliqueRounded = 13, ObliqueOffCenter = 14, ObliqueSquare = 15 }
	public enum PdfPanoseMidline { Any = 0, NoFit = 1, StandardTrimmer = 2, StandardPointed = 3, StandardSerifed = 4, HighTrimmed = 5, HighPointed = 6, 
								   HighSerifed = 7, ConstantTrimmed = 8, ConstantPointed = 9, ConstantSerifed = 10, LowTrimmed = 11, LowPointed = 12, LowSerifed = 13 }
	public enum PdfPanoseXHeight { Any = 0, NoFit = 1, ConstantSmall = 2, ConstantStandard = 3, ConstantLarge = 4, DuckingSmall = 5, DuckingStandard = 6, DuckingLarge = 7 }
	public struct PdfPanose {
		const int length = 10;
		public PdfPanoseFamilyKind FamilyKind { get; set; }
		public PdfPanoseSerifStyle SerifStyle { get; set; }
		public PdfPanoseWeight Weight { get; set; }
		public PdfPanoseProportion Proportion { get; set; }
		public PdfPanoseContrast Contrast { get; set; }
		public PdfPanoseStrokeVariation StrokeVariation { get; set; }
		public PdfPanoseArmStyle ArmStyle { get; set; }
		public PdfPanoseLetterform LetterForm { get; set; }
		public PdfPanoseMidline Midline { get; set; }
		public PdfPanoseXHeight XHeight { get; set; }
		internal bool IsDefault { 
			get { 
				return FamilyKind == PdfPanoseFamilyKind.Any && SerifStyle == PdfPanoseSerifStyle.Any && Weight == PdfPanoseWeight.Any && 
					Proportion == PdfPanoseProportion.Any && Contrast == PdfPanoseContrast.Any && StrokeVariation == PdfPanoseStrokeVariation.Any && 
					ArmStyle == PdfPanoseArmStyle.Any && LetterForm == PdfPanoseLetterform.Any && Midline == PdfPanoseMidline.Any && XHeight == PdfPanoseXHeight.Any; 
			} 
		}
		internal PdfPanose(PdfBinaryStream stream) : this() {
			byte[] data = stream.ReadArray(10);
			FamilyKind = (PdfPanoseFamilyKind)data[0];
			SerifStyle = (PdfPanoseSerifStyle)data[1];
			Weight = (PdfPanoseWeight)data[2];
			Proportion = (PdfPanoseProportion)data[3];
			Contrast = (PdfPanoseContrast)data[4];
			StrokeVariation = (PdfPanoseStrokeVariation)data[5];
			ArmStyle = (PdfPanoseArmStyle)data[6];
			LetterForm = (PdfPanoseLetterform)data[7];
			Midline = (PdfPanoseMidline)data[8];
			XHeight = (PdfPanoseXHeight)data[9];
		}
		internal void Write(PdfBinaryStream stream) {
			byte[] data = new byte[] { (byte)FamilyKind, (byte)SerifStyle, (byte)Weight, (byte)Proportion, (byte)Contrast, (byte)StrokeVariation, (byte)ArmStyle, (byte)LetterForm, (byte)Midline, (byte)XHeight };
			stream.WriteArray(data);
		}
	}
}
