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
	public abstract class PdfFunction : PdfObject {
		protected const string FunctionTypeDictionaryKey = "FunctionType";
		internal static PdfFunction Parse(PdfObjectCollection objects, object value, bool expectDefault) {
			value = objects.TryResolve(value);
			PdfName name = value as PdfName;
			if (name == null)
				return PdfCustomFunction.PerformParse(value);
			switch (name.Name) {
				case PdfPredefinedFunction.DefaultFunctionName:
					if (expectDefault)
						return PdfPredefinedFunction.Default;
					break;
				case PdfPredefinedFunction.IdentityFunctionName:
					return PdfPredefinedFunction.Identity;
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		protected PdfFunction() : base(PdfObject.DirectObjectNumber) { 
		}
		protected PdfFunction(int objectNumber) : base(objectNumber) {
		}
		protected internal abstract bool IsSame(PdfFunction function);
		protected internal abstract double[] Transform(double[] arguments);
		protected internal abstract object Write(PdfObjectCollection objects);
	}
}
