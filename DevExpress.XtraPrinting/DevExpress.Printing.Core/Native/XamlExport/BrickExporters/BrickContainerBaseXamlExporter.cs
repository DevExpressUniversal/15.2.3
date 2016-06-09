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
	class BrickContainerBaseXamlExporter : BrickXamlExporterBase {
		public override void WriteBrickToXaml(XamlWriter writer, XtraPrinting.BrickBase brick, XamlExportContext exportContext, RectangleF clipRect, Action<XamlWriter> declareNamespaces, Action<XamlWriter, object> writeCustomProperties) {
			writer.WriteStartElement(XamlTag.Canvas);
			BrickContainerBase brickContainer = (BrickContainerBase)brick;
			if(brickContainer.Brick != null) {
				float width = brickContainer.Brick.Width.DocToDip();
				float height = brickContainer.Brick.Height.DocToDip();
				writer.WriteAttribute(XamlAttribute.Width, width);
				writer.WriteAttribute(XamlAttribute.Height, height);
			}
			writer.WriteAttribute(XamlAttribute.CanvasLeft, (brick.X + brick.InnerBrickListOffset.X).DocToDip());
			writer.WriteAttribute(XamlAttribute.CanvasTop, (brick.Y + brick.InnerBrickListOffset.Y).DocToDip());
			if(brickContainer.Brick != null)
				writer.WriteAttribute(XamlAttribute.Tag, DocumentMapTreeViewNodeHelper.GetTagByIndices(exportContext.Page.GetIndicesByBrick(brickContainer.Brick), exportContext.PageNumber - 1));
		}
		public override void WriteEndTags(XamlWriter writer) {
			writer.WriteEndElement(); 
		}
		protected override XamlBrickExportMode GetBrickExportMode() {
			return XamlBrickExportMode.ChildElements;
		}
	}
}
