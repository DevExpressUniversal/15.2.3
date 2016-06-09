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
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Import.OpenXml;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingBulletDestinationBase (abstract class)
	public abstract class DrawingBulletDestinationBase : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DrawingTextParagraphProperties properties;
		protected DrawingBulletDestinationBase(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer) {
			this.properties = properties;
		}
		protected DrawingTextParagraphProperties Properties { get { return properties; } }
	}
	#endregion
	#region DrawingBulletAutoNumberedDestination
	public class DrawingBulletAutoNumberedDestination : DrawingBulletDestinationBase {
		public DrawingBulletAutoNumberedDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int startAt = Importer.GetIntegerValue(reader, "startAt", 1);
			DrawingTextAutoNumberSchemeType? type = Importer.GetWpEnumOnOffNullValue(reader, "type", OpenXmlExporter.DrawingTextAutoNumberSchemeTypeTable);
			if (!type.HasValue)
				Importer.ThrowInvalidFile();
			DrawingValueChecker.CheckTextBulletStartAtNumValue(startAt);
			Properties.Bullets.Common = new DrawingBulletAutoNumbered(type.Value, (short)startAt);
		}
	}
	#endregion
	#region DrawingBulletCharacterDestination
	public class DrawingBulletCharacterDestination : DrawingBulletDestinationBase {
		public DrawingBulletCharacterDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string character = Importer.ReadAttribute(reader, "char");
			if (character == null)
				Importer.ThrowInvalidFile();
			Properties.Bullets.Common = new DrawingBulletCharacter(character);
		}
	}
	#endregion
	#region DrawingBulletPictureDestination
	public class DrawingBulletPictureDestination : DrawingTextParagraphPropertiesDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("blip", OnBlip);
			return result;
		}
		static DrawingBulletPictureDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingBulletPictureDestination)importer.PeekDestination();
		}
		static Destination OnBlip(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingBlip blip = new DrawingBlip(importer.ActualDocumentModel);
			GetThis(importer).Properties.Bullets.Common = blip;
			return new DrawingBlipDestination(importer, blip);
		}
		#endregion
		public DrawingBulletPictureDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region DrawingBulletSizeDestinationBase (abstract class)
	public abstract class DrawingBulletSizeDestinationBase : DrawingBulletDestinationBase {
		protected DrawingBulletSizeDestinationBase(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Properties.Bullets.Size = CreateBullet(Importer.GetIntegerValue(reader, "val", Int32.MinValue));
		}
		protected abstract IDrawingBullet CreateBullet(int value);
	}
	#endregion
	#region DrawingBulletSizePercentageDestination
	public class DrawingBulletSizePercentageDestination : DrawingBulletSizeDestinationBase {
		public DrawingBulletSizePercentageDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		protected override IDrawingBullet CreateBullet(int value) {
			DrawingValueChecker.CheckTextBulletSizePercentValue(value);
			return new DrawingBulletSizePercentage(value);
		}
	}
	#endregion
	#region DrawingBulletSizePointsDestination
	public class DrawingBulletSizePointsDestination : DrawingBulletSizeDestinationBase {
		public DrawingBulletSizePointsDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraphProperties properties)
			: base(importer, properties) {
		}
		protected override IDrawingBullet CreateBullet(int value) {
			DrawingValueChecker.CheckTextBulletSizePointsValue(value);
			return new DrawingBulletSizePoints(value);
		}
	}
	#endregion
}
