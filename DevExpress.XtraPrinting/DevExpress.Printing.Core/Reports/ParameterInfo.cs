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
using DevExpress.Utils;
#if !SILVERLIGHT
using System.Windows.Forms;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraReports.Parameters {
	public class ParameterInfo {
		readonly Parameter parameter;
		readonly Function<Control, Parameter> createEditor;
		Control editor;
		public ParameterInfo(Parameter parameter, Function<Control, Parameter> createEditor) {
			this.parameter = parameter;
			this.createEditor = createEditor;
		}
		public ParameterInfo(Parameter parameter, Control editor)
			: this(parameter, _ => null) {
			this.editor = editor;
		}
		public Parameter Parameter {
			get { return parameter; }
		}
		public Control Editor {
			get {
				return GetEditor(true);
			}
			set {
				if(editor == value)
					return;
				using(editor as IDisposable) {
				}
				editor = value;
			}
		}
		public Control GetEditor(bool forceCreate) {
			if(forceCreate && editor == null && createEditor != null)
				editor = createEditor(parameter);
			return editor;
		} 
	}
}
namespace DevExpress.XtraReports.Parameters.Native {
	public static class ParameterInfoFactory {
		public static ParameterInfo CreateWithoutEditor(Parameter parameter) {
			return new ParameterInfo(parameter, (Control)null);
		}
	}
}
