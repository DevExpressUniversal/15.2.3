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

#if METASHARP
using MetaSharp;
using System.Reflection;
using MetaSharp.Native;
namespace DevExpress.Xpf.Diagram {
	using DevExpress.Mvvm;
	using DevExpress.Diagram.Core;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Input;
	using System.Text;
	static class DiagramCommandsGenerator {
		public static string GenerateCommands(MetaContext context) {
			var fields = typeof(DiagramCommandsBase)
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(x => x.Name.EndsWith("Command") && x.FieldType.Name.StartsWith(typeof(DiagramCommand).Name))
				.Select(x => new {
					Name = x.Name,
					ParemeterType = (x.FieldType.IsGenericType ? x.FieldType.GetGenericArguments().Single().Name : null)
				})
				.ToArray();
			var properties = fields
				.Select(x => "public ICommand" + x.ParemeterType.With(paramName => "<" + paramName + ">") + " " + x.Name.ReplaceEnd("Command", string.Empty) +" { get; private set; }")
				.ConcatStringsWithNewLines()
				.AddTabs(1);
			var assignments = fields
				.Select(x => x.Name.ReplaceEnd("Command", string.Empty) + " = CreateCommand(" + x.Name + ");")
				.ConcatStringsWithNewLines()
				.AddTabs(2);
			string template =
@"partial class DiagramCommands {{
{0}
    public DiagramCommands(DiagramControl diagram) 
        : base(diagram) {{
{1}
    }}
}}";
			return context.WrapMembers(string.Format(template, properties, assignments));
		}
	}
}
#endif
