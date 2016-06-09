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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter : IDrawingBulletVisitor {
		#region DrawingTextAutoNumberSchemeTypeTable
		internal static Dictionary<DrawingTextAutoNumberSchemeType, string> DrawingTextAutoNumberSchemeTypeTable = GetDrawingTextAutoNumberSchemeTypeTable();
		static Dictionary<DrawingTextAutoNumberSchemeType, string> GetDrawingTextAutoNumberSchemeTypeTable() {
			Dictionary<DrawingTextAutoNumberSchemeType, string> result = new Dictionary<DrawingTextAutoNumberSchemeType, string>();
			result.Add(DrawingTextAutoNumberSchemeType.AlphaLcParenBoth, "alphaLcParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.AlphaLcParenR, "alphaLcParenR");
			result.Add(DrawingTextAutoNumberSchemeType.AlphaLcPeriod, "alphaLcPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.AlphaUcParenBoth, "alphaUcParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.AlphaUcParenR, "alphaUcParenR");
			result.Add(DrawingTextAutoNumberSchemeType.AlphaUcPeriod, "alphaUcPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.Arabic1Minus, "arabic1Minus");
			result.Add(DrawingTextAutoNumberSchemeType.Arabic2Minus, "arabic2Minus");
			result.Add(DrawingTextAutoNumberSchemeType.ArabicDbPeriod, "arabicDbPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.ArabicDbPlain, "arabicDbPlain");
			result.Add(DrawingTextAutoNumberSchemeType.ArabicParenBoth, "arabicParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.ArabicParenR, "arabicParenR");
			result.Add(DrawingTextAutoNumberSchemeType.ArabicPeriod, "arabicPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.ArabicPlain, "arabicPlain");
			result.Add(DrawingTextAutoNumberSchemeType.CircleNumDbPlain, "circleNumDbPlain");
			result.Add(DrawingTextAutoNumberSchemeType.CircleNumWdBlackPlain, "circleNumWdBlackPlain");
			result.Add(DrawingTextAutoNumberSchemeType.CircleNumWdWhitePlain, "circleNumWdWhitePlain");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1ChsPeriod, "ea1ChsPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1ChsPlain, "ea1ChsPlain");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1ChtPeriod, "ea1ChtPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1ChtPlain, "ea1ChtPlain");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1JpnChsDbPeriod, "ea1JpnChsDbPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1JpnKorPeriod, "ea1JpnKorPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.Ea1JpnKorPlain, "ea1JpnKorPlain");
			result.Add(DrawingTextAutoNumberSchemeType.Hebrew2Minus, "hebrew2Minus");
			result.Add(DrawingTextAutoNumberSchemeType.HindiAlpha1Period, "hindiAlpha1Period");
			result.Add(DrawingTextAutoNumberSchemeType.HindiAlphaPeriod, "hindiAlphaPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.HindiNumParenR, "hindiNumParenR");
			result.Add(DrawingTextAutoNumberSchemeType.HindiNumPeriod, "hindiNumPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.RomanLcParenBoth, "romanLcParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.RomanLcParenR, "romanLcParenR");
			result.Add(DrawingTextAutoNumberSchemeType.RomanLcPeriod, "romanLcPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.RomanUcParenBoth, "romanUcParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.RomanUcParenR, "romanUcParenR");
			result.Add(DrawingTextAutoNumberSchemeType.RomanUcPeriod, "romanUcPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.ThaiAlphaParenBoth, "thaiAlphaParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.ThaiAlphaParenR, "thaiAlphaParenR");
			result.Add(DrawingTextAutoNumberSchemeType.ThaiAlphaPeriod, "thaiAlphaPeriod");
			result.Add(DrawingTextAutoNumberSchemeType.ThaiNumParenBoth, "thaiNumParenBoth");
			result.Add(DrawingTextAutoNumberSchemeType.ThaiNumParenR, "thaiNumParenR");
			result.Add(DrawingTextAutoNumberSchemeType.ThaiNumPeriod, "thaiNumPeriod");
			return result;
		}
		#endregion
		protected internal virtual void GenerateDrawingTextBulletContent(IDrawingTextBullets bullets) {
			Guard.ArgumentNotNull(bullets, "IDrawingTextBullets");
			bullets.Color.Visit(this);
			bullets.Size.Visit(this);
			bullets.Typeface.Visit(this);
			bullets.Common.Visit(this);
		}
		#region IDrawingBulletVisitor Members
		void IDrawingBulletVisitor.Visit(DrawingBulletAutoNumbered bullet) {
			WriteStartElement("buAutoNum", OpenXmlExporter.DrawingMLNamespace);
			try {
				WriteStringValue("type", DrawingTextAutoNumberSchemeTypeTable[bullet.SchemeType]);
				WriteIntValue("startAt", bullet.StartAt, DrawingBulletAutoNumbered.DefaultStartAtValue);
			}
			finally {
				WriteEndElement();
			}
		}
		void IDrawingBulletVisitor.Visit(DrawingBulletCharacter bullet) {
			if (String.IsNullOrEmpty(bullet.Character))
				return;
			WriteStartElement("buChar", DrawingMLNamespace);
			try {
				WriteStringValue("char", bullet.Character);
			}
			finally {
				WriteEndElement();
			}
		}
		void IDrawingBulletVisitor.Visit(DrawingBlip bullet) {
			if (IsDefaultDrawingBlip(bullet))
				return;
			WriteStartElement("buBlip", DrawingMLNamespace);
			try {
				GenerateDrawingBlipContent(bullet);
			}
			finally {
				WriteEndElement();
			}
		}
		void IDrawingBulletVisitor.Visit(DrawingColor bullet) {
			if (bullet.IsEmpty)
				return;
			WriteStartElement("buClr", DrawingMLNamespace);
			try {
				GenerateDrawingColorContent(bullet);
			}
			finally {
				WriteEndElement();
			}
		}
		void IDrawingBulletVisitor.Visit(DrawingTextFont bullet) {
			GenerateDrawingTextFontContent(bullet, "buFont");
		}
		void IDrawingBulletVisitor.Visit(DrawingBulletSizePercentage bullet) {
			GenerateDrawingBulletSizeContent(bullet.Value, "buSzPct");
		}
		void IDrawingBulletVisitor.Visit(DrawingBulletSizePoints bullet) {
			GenerateDrawingBulletSizeContent(bullet.Value, "buSzPts");
		}
		void IDrawingBulletVisitor.VisitNoBullets() {
			GenerateDrawingBulletTag("buNone");
		}
		void IDrawingBulletVisitor.VisitBulletColorFollowText() {
			GenerateDrawingBulletTag("buClrTx");
		}
		void IDrawingBulletVisitor.VisitBulletTypefaceFollowText() {
			GenerateDrawingBulletTag("buFontTx");
		}
		void IDrawingBulletVisitor.VisitBulletSizeFollowText() {
			GenerateDrawingBulletTag("buSzTx");
		}
		void GenerateDrawingBulletTag(string tagName) {
			WriteStartElement(tagName, DrawingMLNamespace);
			WriteEndElement();
		}
		void GenerateDrawingBulletSizeContent(int value, string tagName) {
			WriteStartElement(tagName, DrawingMLNamespace);
			try {
				WriteIntValue("val", value);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
