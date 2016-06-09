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

using EnvDTE;
using VSLangProj;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel.Design;
using System;
using System.Reflection;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting.Design {
	public class BarsReferencesHelper {
		public static void AddBarsReferences(ComponentDesigner designer) {
			AddReference(designer.Component.Site, typeof(IXtraSerializable).Assembly);
			AddReference(designer.Component.Site, typeof(SimpleButton).Assembly);
			AddReference(designer.Component.Site, typeof(BarManager).Assembly);
		}
		static void AddReference(IServiceProvider provider, Assembly assembly) {
			try {
				ProjectItem projectItem = provider.GetService(typeof(ProjectItem)) as ProjectItem;
				Project project = projectItem.ContainingProject;
				VSProject vsProject = project.Object as VSProject;
				vsProject.References.Add(assembly.GetName().Name);
			} catch { }
		}
	}
}
