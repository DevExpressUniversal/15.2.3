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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfPaintXObjectCommand : PdfCommand {
		internal const string Name = "Do";
		readonly string name;
		readonly PdfResources resources;
		PdfXObject xObject;
		public PdfXObject XObject {
			get {
				if (xObject == null)
					xObject = resources.GetXObject(name);
				return xObject;
			}
		}
		internal PdfPaintXObjectCommand(string name, PdfResources resources) {
			this.name = name;
			this.resources = resources;
		}
		internal PdfPaintXObjectCommand(PdfResources resources, PdfOperands operands) {
			xObject = resources.GetXObject(operands.PopName());
			if (xObject == null)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteName(string.IsNullOrEmpty(name) ? resources.FindXObjectName(xObject) : new PdfName(name));
			writer.WriteSpace();
			writer.WriteString(Name);
		}
		protected internal override void Execute(PdfCommandInterpreter interpreter) {
			PdfImage image = XObject as PdfImage;
			if (image == null) {
				PdfForm form = XObject as PdfForm;
				if (form != null)
					interpreter.DrawForm(form);
			}
			else
				interpreter.DrawImage(image);
		}
	}
}
