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
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.DemoData.Model {
	public class ExampleModule : SimpleModule {
		public ExampleModule(Demo demo,
							string name,
							string displayName,
							string group,
							string type,
							string uri,
							string description,
							KnownDXVersion addedIn,
							KnownDXVersion updatedIn = KnownDXVersion.Before142,
							int featuredPriority = int.MaxValue,
							int newUpdatedPriority = int.MaxValue,
							bool isFeatured = false)
			: base(demo, name, displayName, group, type, description, addedIn, updatedIn,featuredPriority,newUpdatedPriority,isFeatured) {
				Uri = uri;
		}
		public string Uri { get; private set; }
	}
	public class SimpleModule : Module, IExecutable {
		string[] associatedFiles;
		KnownDXVersion addedIn;
		KnownDXVersion updatedIn;
		public SimpleModule(Demo demo,
							string name,
							string displayName,
							string group,
							string type,
							string description,
							KnownDXVersion addedIn,
							KnownDXVersion updatedIn = KnownDXVersion.Before142,
							int featuredPriority = int.MaxValue,
							int newUpdatedPriority = int.MaxValue,
							bool isFeatured = false,
							string[] associatedFiles = null,
							bool isCodeExample = false,
							bool showFromDemoChooser = true)
			: base(demo, name, displayName, group, type, description, addedIn, updatedIn) {
			FeaturedPriority = featuredPriority;
			NewUpdatedPriority = newUpdatedPriority;
			IsFeatured = isFeatured;
			this.associatedFiles = associatedFiles;
			IsCodeExample = isCodeExample;
			this.addedIn = addedIn;
			this.updatedIn = updatedIn;
			ShowFromDemoChooser = showFromDemoChooser;
		}
		public int FeaturedPriority { get; private set; }
		public int NewUpdatedPriority { get; private set; }
		public bool IsFeatured { get; private set; }
		public string[] AssociatedFiles { get { return associatedFiles ?? GetAssociatedFiles(); } }
		public bool IsCodeExample { get; private set; }
		public bool ShowFromDemoChooser { get; private set; }
		public DemoImage Icon {
			get {
				return new DemoImage(string.Format("{0}/{1}/{2}/{3}.Icon.png", Demo.Product.Platform.Name, Demo.Product.Name, Demo.Name, Name));
			}
		}
		string[] GetAssociatedFiles() {
			if (Name == "About")
				return new string[0];
			return new[] {
				GetAssociatedFile(Demo.CsSolutionPath, ".cs"),
				GetAssociatedFile(Demo.VbSolutionPath, ".vb")
			};
		}
		string GetAssociatedFile(string demoPath, string extension) {
			var dir = Path.GetDirectoryName(demoPath) + @"\Modules";
			int namePos = Type.LastIndexOf('.');
			int namespacePos = Type.LastIndexOf(".Demos") + ".Demos".Length;
			var moduleName = Type.Substring(namePos + 1);
			var namespaceStr = Type.Substring(namespacePos, namePos + 1 - namespacePos);
			namespaceStr = namespaceStr.Replace('.', '\\');
			return dir + namespaceStr + moduleName + extension;
		}
		public SimpleModule WithDemo(Demo demo) {
			return new SimpleModule(demo, Name, DisplayName, Group, Type, Description,
									addedIn, updatedIn, FeaturedPriority, NewUpdatedPriority, IsFeatured,
									AssociatedFiles, IsCodeExample);
		}
		public virtual string[] Arguments {
			get {
				return new[] {
					string.Format("/start:{0}", Type),
					"/fullwindow"
				};
			}
		}
		public string DoEventMessage { get { return Demo.DoEventMessage; } }
		public string LaunchPath { get { return Demo.LaunchPath; } }
		public Platform Platform { get { return Demo.Platform; } }
		public Requirement[] Requirements { get { return Demo.Requirements; } }
	}
	public class SimpleDemo : Demo {
		Func<Demo, List<Module>> createModules;
		KnownDXVersion addedIn;
		KnownDXVersion updatedIn;
		public SimpleDemo(Product product,
						  Func<Demo, List<Module>> createModules,
						  string name,
						  string displayName,
						  string csSolutionPath,
						  string vbSolutionPath,
						  string launchPath,
						  string argument = "-demochooser",
						  KnownDXVersion addedIn = KnownDXVersion.Before142,
						  KnownDXVersion updatedIn = KnownDXVersion.Before142,
						  Requirement[] requirements = null)
			: base(product, createModules, name, displayName, csSolutionPath, vbSolutionPath, addedIn, updatedIn, requirements) {
			this.launchPath = launchPath;
			this.argument = argument;
			this.createModules = createModules;
			this.addedIn = addedIn;
			this.updatedIn = updatedIn;
		}
		string launchPath;
		public override string LaunchPath { get { return launchPath; } }
		public DemoImage Screenshot { get { return new DemoImage(string.Format("{0}/{1}/{2}.Medium.png", Product.Platform.Name, Product.Name, Name)); } }
		string argument;
		public override string[] Arguments { get { return new[] { argument }; } }
		public SimpleDemo WithSolutionPaths(string csPath, string vbPath) {
			return new SimpleDemo(Product, createModules, Name, DisplayName, csPath, vbPath,
								  LaunchPath, argument, addedIn, updatedIn, Requirements);
		}
	}
}
