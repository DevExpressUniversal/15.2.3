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
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region ColorTableDestination
	public class ColorTableDestination : DestinationBase {
		public static Color AutoColor = DXColor.Empty;
		public static Color[] DefaultMSWordColor = {	 AutoColor,
														 Color.FromArgb(255, 0, 0, 0),
														 Color.FromArgb(255, 0, 0, 255),
														 Color.FromArgb(255, 0, 255, 255),
														 Color.FromArgb(255, 0, 255, 0),
														 Color.FromArgb(255, 255, 0, 255),
														 Color.FromArgb(255, 255, 0, 0),
														 Color.FromArgb(255, 255, 255, 0),
														 Color.FromArgb(255, 255, 255, 255),
														 Color.FromArgb(255, 0, 0, 128),
														 Color.FromArgb(255, 0, 128, 128),
														 Color.FromArgb(255, 0, 128, 0),
														 Color.FromArgb(255, 128, 0, 128),
														 Color.FromArgb(255, 128, 0, 0),
														 Color.FromArgb(255, 128, 128, 0),
														 Color.FromArgb(255, 128, 128, 128),
														 Color.FromArgb(255, 192, 192, 192)
													 };
		int r, g, b;
		bool wasColor;
		public ColorTableDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected override ControlCharTranslatorTable ControlCharHT { get { return null; } }
		protected override KeywordTranslatorTable KeywordHT { get { return null; } }
		void Reset() {
			r = g = b = 0;
			wasColor = false;
		}
		protected override bool ProcessKeywordCore(string keyword, int parameterValue, bool hasParameter) {
			if (hasParameter == false)
				parameterValue = 0;
			switch (keyword) {
				case "bin":
					return base.ProcessKeywordCore(keyword, parameterValue, hasParameter);
				case "red":
					r = parameterValue;
					wasColor = true;
					break;
				case "green":
					g = parameterValue;
					wasColor = true;
					break;
				case "blue":
					b = parameterValue;
					wasColor = true;
					break;
				default:
					return false;
			}
			return true;
		}
		protected override DestinationBase CreateClone() {
			return new ColorTableDestination(Importer);
		}
		bool IsColorValid() {
			return r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255;
		}
		protected override void ProcessCharCore(char ch) {
			if (ch == ';') {
				if (wasColor) {
					if (IsColorValid())
						Importer.DocumentProperties.Colors.Add(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
					else
						RtfImporter.ThrowInvalidRtfFile();
				}
				else {
					RtfColorCollection colors = Importer.DocumentProperties.Colors;
					int newColorIndex = colors.Count;
					Color color = DXColor.Empty;
					if (newColorIndex < DefaultMSWordColor.Length)
						color = DefaultMSWordColor[newColorIndex];
					Importer.DocumentProperties.Colors.Add(color);
				}
				Reset();
			}
		}
	}
	#endregion
}
