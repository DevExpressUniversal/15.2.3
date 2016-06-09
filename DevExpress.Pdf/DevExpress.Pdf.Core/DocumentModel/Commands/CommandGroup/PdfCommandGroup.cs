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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public abstract class PdfCommandGroup : PdfCommand {
		readonly List<PdfCommand> children = new List<PdfCommand>();
		public IList<PdfCommand> Children { get { return children; } }
		protected internal virtual bool ShouldIgnoreUnknownCommands { get { return false; } }
		protected abstract string Suffix { get; }
		protected PdfCommandGroup() { 
		}
		protected internal override void Write(PdfResources resources, PdfDocumentWritableStream writer) {
			foreach (object prefix in GetPrefix(resources)) {
				writer.WriteSpace();
				writer.WriteObject(prefix, PdfObject.DirectObjectNumber);
			}
			foreach (PdfCommand cmd in children)
				cmd.Write(resources, writer);
			writer.WriteSpace();
			writer.WriteObject(new PdfToken(Suffix), PdfObject.DirectObjectNumber);
		}
		protected internal override void Execute(PdfCommandInterpreter interpreter) {
			foreach (PdfCommand command in children)
				try {
					command.Execute(interpreter);
				}
				catch { 
				}
		}
		protected abstract IEnumerable<object> GetPrefix(PdfResources resources);
	}
}
