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
using System.IO;
using System.Collections;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public abstract class PdfAction : PdfDocumentDictionaryObject {
		public abstract string Subtype { get; }
		protected PdfAction(bool compressed)
			: base(compressed) {
		}
		public override void FillUp() {
			Dictionary.Add("Type", "Action");
			Dictionary.Add("S", Subtype);
		}
	}
	public class PdfURIAction : PdfAction {
		string uri;
		public override string Subtype { get { return "URI"; } }
		public string URI { get { return uri; } set { uri = value; } } 
		public PdfURIAction(string uri, bool compressed) : base(compressed) {
			this.uri = uri;
		}
		public override void FillUp() {
			base.FillUp();
			if(uri != null)	Dictionary.Add("URI", new PdfLiteralString(uri));
		}
	}
	public class PdfGoToAction : PdfAction {
		PdfDestination dest;
		public override string Subtype { get { return "GoTo"; } }
		public PdfGoToAction(PdfDestination dest, bool compressed) : base(compressed) {
			this.dest =dest;
		}
		public override void FillUp() {
			base.FillUp();
			if(dest != null) Dictionary.Add("D", dest);
		}
	}
}
