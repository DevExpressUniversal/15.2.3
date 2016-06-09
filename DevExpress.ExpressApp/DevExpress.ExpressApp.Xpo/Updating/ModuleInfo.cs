#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.Xpo.Updating {
	[MemberDesignTimeVisibility(false)]
	public class ModuleInfo : XPBaseObject, IModuleInfo {
		private int id;
		private string name;
		private string version;
		private bool isMain;
		private string assemblyFileName;
		public ModuleInfo(Session session) : base(session) { }
		public override void AfterConstruction() {
			base.AfterConstruction();
			name = "";
			version = "";
		}
		[Key(AutoGenerate = true)]
		public int ID {
			get { return id; }
			set { SetPropertyValue("ID", ref id, value); }
		}
		public string Version {
			get { return version; }
			set { SetPropertyValue("Version", ref version, value); }
		}
		public string Name {
			get { return name; }
			set { SetPropertyValue("Name", ref name, value); }
		}
		public string AssemblyFileName {
			get { return assemblyFileName; }
			set { SetPropertyValue("AssemblyFileName", ref assemblyFileName, value); }
		}
		public bool IsMain {
			get { return isMain; }
			set { SetPropertyValue("IsMain", ref isMain, value); }
		}
		public override string ToString() {
			return !string.IsNullOrEmpty(Name) ? Name : base.ToString();
		}
	}
}
