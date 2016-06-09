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

using DevExpress.Pdf.Interop;
namespace DevExpress.Pdf.Drawing {
	internal class PdfFontRegistrationData {
		readonly string name;
		readonly double widthFactor;
		readonly int weight;
		readonly bool italic;
		readonly FontPitchAndFamily pitchAndFamily;
		readonly bool useEmbeddedEncoding;
		readonly PdfFontRegistrator registrator;
		readonly bool isType3Font;
		public string Name { get { return name; } }
		public double WidthFactor { get { return widthFactor; } }
		public int Weight { get { return weight; } }
		public bool Italic { get { return italic; } }
		public FontPitchAndFamily PitchAndFamily { get { return pitchAndFamily; } }
		public bool UseEmbeddedEncoding { get { return useEmbeddedEncoding; } }
		public PdfFontRegistrator Registrator { get { return registrator; } }
		public bool IsType3Font { get { return isType3Font; } }
		public PdfFontRegistrationData(string name, double widthFactor, int weight, bool italic, FontPitchAndFamily pitchAndFamily, bool useEmbeddedEncoding, PdfFontRegistrator registrator, bool isType3Font) {
			this.name = name;
			this.widthFactor = widthFactor;
			this.weight = weight;
			this.italic = italic;
			this.pitchAndFamily = pitchAndFamily;
			this.useEmbeddedEncoding = useEmbeddedEncoding;
			this.registrator = registrator;
			this.isType3Font = isType3Font;
		}
		public PdfFontRegistrationData(string name, double widthFactor, int weight, bool italic, FontPitchAndFamily pitchAndFamily, bool useEmbeddedEncoding, PdfFontRegistrator registrator) 
			: this(name, widthFactor, weight, italic, pitchAndFamily, useEmbeddedEncoding, registrator, false) {
		}
	}
}
