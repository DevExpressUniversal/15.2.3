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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public sealed class PdfPredefinedFunction : PdfFunction {
		internal const string DefaultFunctionName = "Default";
		internal const string IdentityFunctionName = "Identity";
		static readonly PdfPredefinedFunction defaultFunction = new PdfPredefinedFunction();
		static readonly PdfPredefinedFunction identityFunction = new PdfPredefinedFunction();
		public static PdfFunction Default { get { return defaultFunction; } }
		public static PdfFunction Identity { get { return identityFunction; } }
		protected internal override bool IsSame(PdfFunction function) {
			return Object.ReferenceEquals(this, function);
		}
		protected internal override double[] Transform(double[] arguments) {
			return arguments;
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return new PdfName(Object.ReferenceEquals(this, defaultFunction) ? DefaultFunctionName : IdentityFunctionName);
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return Write(objects);
		}
	}
}
