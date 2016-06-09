#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web {
	public class CustomizeParameterEditorsEventArgs : EventArgs {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const bool ShouldSetParameterValueDefault = true;
		ASPxEditBase editor;
		public ASPxEditBase Editor {
			get { return editor; }
			set {
				Guard.ArgumentNotNull(value, "Editor");
				editor = value;
			}
		}
		public Parameter Parameter { get; private set; }
		public XtraReport Report { get; private set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSetParameterValue { get; set; }
		public CustomizeParameterEditorsEventArgs(Parameter parameter, ASPxEditBase editor, XtraReport report)
			: this(parameter, editor, report, ShouldSetParameterValueDefault) {
		}
		internal CustomizeParameterEditorsEventArgs(Parameter parameter, ASPxEditBase editor, XtraReport report, bool shouldSetParameterValue) {
			Parameter = parameter;
			this.editor = editor;
			Report = report;
			ShouldSetParameterValue = shouldSetParameterValue;
		}
	}
}
