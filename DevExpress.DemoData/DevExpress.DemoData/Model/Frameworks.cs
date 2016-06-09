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
using System.Linq;
using System.Text;
namespace DevExpress.DemoData.Model {
	public class FrameworkDemo : Demo {
		public FrameworkDemo(Product product,
							 Func<Demo, List<Module>> createModules,
							 string name,
							 string displayName,
							 string csSolutionPath,
							 string vbSolutionPath,
							 string shortDescription,
							 string group,
							 string winPath,
							 string webPath,
							 string buildItPath,
							 Requirement[] requirements = null)
			: base(product, createModules, name, displayName, csSolutionPath, vbSolutionPath, requirements: requirements)
		{
			ShortDescription = shortDescription;
			Group = group;
			WinPath = winPath;
			WebPath = webPath;
			BuildItPath = buildItPath;
		}
		public string ShortDescription { get; private set; }
		public DemoImage MediumImage { get { return new DemoImage(string.Format("{0}/{1}/{2}.Medium.png", Product.Platform.Name, Product.Name, Name)); } }
		public string Group { get; private set; }
		public string WinPath { get; private set; }
		public string WebPath { get; private set; }
		public string BuildItPath { get; private set; }
		public override string LaunchPath { get { return WinPath; } }
		public override string[] Arguments { get { return new string[0]; } }
	}
}
