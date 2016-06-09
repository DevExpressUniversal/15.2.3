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
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	public class HyperlinkDestination : DevExpress.XtraRichEdit.Import.OpenXml.HyperlinkDestination {
		static readonly HyperlinkAttributeHandlerTable attributeHandlerTable = CreateAttributeHandlerTable();
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("r", OnRun);
			result.Add("fldSimple", OnFieldSimple);
			result.Add("hlink", OnHyperlink);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			return result;
		}
		static HyperlinkAttributeHandlerTable CreateAttributeHandlerTable() {
			HyperlinkAttributeHandlerTable result = new HyperlinkAttributeHandlerTable();
			result.Add("dest", OnIdAttribute);
			result.Add("bookmark", OnAnchorAttribute);
			result.Add("target", OnTargetFrameAttribute);
			result.Add("screenTip", OnTooltipAttribute);
			result.Add("noHistory", OnHistoryAttribute);
			result.Add("arbLocation", OnDocLocationAttribute);
			return result;
		}
		public HyperlinkDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal override HyperlinkAttributeHandlerTable AttributeHandlerTable { get { return attributeHandlerTable; } }
		static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateRunDestination();
		}
		static Destination OnFieldSimple(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldSimpleDestination(importer);
		}
		static Destination OnHyperlink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		static void OnIdAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
			info.NavigateUri = value;
		}
	}
}
