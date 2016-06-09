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
using System.Drawing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.XamlExport {
	class LineBrickXamlExporter : VisualBrickXamlExporter {
		protected override void WriteBrickToXamlCore(XamlWriter writer, VisualBrick brick, XamlExportContext exportContext) {
			LineBrick lineBrick = brick as LineBrick;
			if(lineBrick == null)
				throw new ArgumentException("brick");
			writer.WriteStartElement(XamlTag.Line);
			writer.WriteAttribute(XamlAttribute.Style, string.Format(staticResourceFormat, exportContext.ResourceMap.LineStylesDictionary[lineBrick]));
			RectangleF lineRect = GetLineClientRectInPixels(lineBrick);
			writer.WriteAttribute(XamlAttribute.X1, lineBrick.GetPoint1(lineRect).X);
			writer.WriteAttribute(XamlAttribute.Y1, lineBrick.GetPoint1(lineRect).Y);
			writer.WriteAttribute(XamlAttribute.X2, lineBrick.GetPoint2(lineRect).X);
			writer.WriteAttribute(XamlAttribute.Y2, lineBrick.GetPoint2(lineRect).Y);
			writer.WriteAttribute(XamlAttribute.StrokeDashArray, VisualBrick.GetDashPattern(lineBrick.LineStyle));
			writer.WriteEndElement();
		}
		static RectangleF GetLineClientRectInPixels(LineBrick lineBrick) {
			RectangleF lineRect = new RectangleF(PointF.Empty, lineBrick.Size).DocToDip();
			lineRect.Width = lineRect.Width - GetBorderWidth(BorderSide.Left, lineBrick) - GetBorderWidth(BorderSide.Right, lineBrick);
			lineRect.Height = lineRect.Height - GetBorderWidth(BorderSide.Top, lineBrick) - GetBorderWidth(BorderSide.Bottom, lineBrick);
			return lineRect;
		}
		protected override XamlBrickExportMode GetBrickExportMode() {
			return XamlBrickExportMode.Content;
		}
	}
}
