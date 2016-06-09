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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Native;
namespace DevExpress.Diagram.Core {
	public static class DiagramLayoutActions {
		public static void SaveDocument(this IDiagramControl diagram, bool overwrite = false) {
			string fileName = (overwrite || string.IsNullOrEmpty(diagram.Controller.FileName)) ? diagram.ShowSaveFileDialog() : diagram.Controller.FileName;
			if (!string.IsNullOrEmpty(fileName)) {
				try {
					diagram.SaveDocument(fileName);
				} catch (Exception e) {
					diagram.ShowMessage(GetErrorMessage(DiagramControlLocalizer.GetString(DiagramControlStringId.DocumentLoadErrorMessage), e), errorMessage: true);
					return;
				}
				diagram.Controller.FileName = fileName;
			}
		}
		public static void LoadDocument(this IDiagramControl diagram, bool createNew = false) {
			string fileName = createNew ? string.Empty : diagram.ShowOpenFileDialog();
			if (!string.IsNullOrEmpty(fileName) || createNew) {
				try {
					if(createNew)
						diagram.NewDocument();
					else
						diagram.LoadDocument(fileName);
				} catch (Exception e) {
					diagram.ShowMessage(GetErrorMessage(DiagramControlLocalizer.GetString(DiagramControlStringId.DocumentLoadErrorMessage), e), errorMessage: true);
					return;
				}
				diagram.Controller.FileName = fileName;
			}
		}
		static string GetErrorMessage(string messageHeader, Exception e) {
			if (e == null) return string.Empty;
			if (e is TargetInvocationException && e.InnerException != null) {
				e = e.InnerException;
			}
			return string.Format("{0} - {1}", messageHeader, e.Message);
		}
	}
}
