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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public abstract class PdfSetCMYKColorSpaceCommand : PdfCommand {
		static bool ValidateColorComponent(double value) {
			return value >= 0 && value <= 1;
		}
		readonly double c;
		readonly double m;
		readonly double y;
		readonly double k;
		public double C { get { return c; } }
		public double M { get { return m; } }
		public double Y { get { return y; } }
		public double K { get { return k; } }
		protected PdfSetCMYKColorSpaceCommand(double c, double m, double y, double k) {
			if (!ValidateColorComponent(c))
				throw new ArgumentOutOfRangeException("c", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
			if (!ValidateColorComponent(m))
				throw new ArgumentOutOfRangeException("m", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
			if (!ValidateColorComponent(y))
				throw new ArgumentOutOfRangeException("y", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
			if (!ValidateColorComponent(k))
				throw new ArgumentOutOfRangeException("k", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
			this.c = c;
			this.m = m;
			this.y = y;
			this.k = k;
		}
		protected PdfSetCMYKColorSpaceCommand(PdfOperands operands) {
			c = operands.PopDouble();
			m = operands.PopDouble();
			y = operands.PopDouble();
			k = operands.PopDouble();
			if (!ValidateColorComponent(c) || !ValidateColorComponent(m) || !ValidateColorComponent(y) || !ValidateColorComponent(k))
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteDouble(c);
			writer.WriteSpace();
			writer.WriteDouble(m);
			writer.WriteSpace();
			writer.WriteDouble(y);
			writer.WriteSpace();
			writer.WriteDouble(k);
			writer.WriteSpace();
		}
	}
}
