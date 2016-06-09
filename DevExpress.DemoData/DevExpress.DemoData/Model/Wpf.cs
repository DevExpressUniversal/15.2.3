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
	public enum WpfModuleLinkType {
		Video,
		Blogs,
		KB,
		Documentation,
		Demos
	}
	public class WpfModuleLink {
		public WpfModuleLink(string title, WpfModuleLinkType type, string url) {
			Title = title;
			Type = type;
			Url = url;
		}
		public string Title { get; set; }
		public WpfModuleLinkType Type { get; set; }
		public string Url { get; set; }
	}
	public class WpfModule : Module {
		public WpfModule(Demo demo,
						 string name,
						 string displayName,
						 string group,
						 string type,
						 string shortDescription,
						 string description,
						 bool allowTouchThemes = true,
						 bool allowDarkThemes = true,
						 bool allowSwitchingThemes = true,
						 bool allowRtl = true,
						 bool isMvvm = false,
						 bool isFeatured = false,
						 KnownDXVersion addedIn = KnownDXVersion.Before142,
						 KnownDXVersion updatedIn = KnownDXVersion.Before142,
						 WpfModuleLink[] links = null)
			: base(demo, name, displayName, group, type, description, addedIn, updatedIn)
		{
			ShortDescription = shortDescription;
			IsMvvm = isMvvm;
			AllowTouchThemes = allowTouchThemes;
			AllowDarkThemes = allowDarkThemes;
			AllowSwitchingThemes = allowSwitchingThemes;
			AllowRtl = allowRtl;
			Links = links ?? new WpfModuleLink[0];
			IsFeatured = isFeatured;
		}
		public string ShortDescription { get; private set; }
		public bool IsMvvm { get; private set; }
		public bool AllowTouchThemes { get; private set; }
		public bool AllowDarkThemes { get; private set; }
		public bool AllowSwitchingThemes { get; private set; }
		public bool AllowRtl { get; private set; }
		public WpfModuleLink[] Links { get; private set; }
		public bool IsFeatured { get; private set; }
	}
	public class WpfDemo : Demo {
		public WpfDemo(Product product,
					   Func<Demo, List<Module>> createModules,
					   string name,
					   string displayName,
					   string assemblyName,
					   string csSolutionPath,
					   string vbSolutionPath,
					   string windowCaption,
					   string launchPath,
					   KnownDXVersion addedIn = KnownDXVersion.Before142,
					   Requirement[] requirements = null)
			: base(product, createModules, name, displayName, csSolutionPath, vbSolutionPath, addedIn, KnownDXVersion.Before142, requirements)
		{
			WindowCaption = windowCaption;
			AssemblyName = assemblyName;
			this.launchPath = launchPath;
		}
		public string WindowCaption { get; private set; }
		public string AssemblyName { get; private set; }
		string launchPath;
		public override string LaunchPath { get { return launchPath; } }
		public override string[] Arguments { get { return new string[0]; } }
	}
}
