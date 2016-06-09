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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
namespace DevExpress.Tutorials {
	public class ModuleInfo {
		ModuleBase module;
		Type moduleType;
		int id;
		string description, whatsThisXMLFile, whatsThisCodeFile, aboutFile;
		public ModuleInfo(int id, Type moduleType, string description, string whatsThisXMLFile, string whatsThisCodeFile) : this(id, moduleType, description, whatsThisXMLFile, whatsThisCodeFile, "") {}
		public ModuleInfo(int id, Type moduleType, string description, string whatsThisXMLFile, string whatsThisCodeFile, string aboutFile) {
			if(!moduleType.IsSubclassOf(typeof(ModuleBase)))
				throw new ArgumentException("Module class should be derived from ModuleBase");
			this.description = description;
			this.aboutFile = aboutFile;
			this.whatsThisXMLFile = whatsThisXMLFile;
			this.whatsThisCodeFile = whatsThisCodeFile;
			this.moduleType = moduleType;
			this.module = null;
			this.id = id;
		}
		public ModuleBase Module { 
			get { 
				if(module == null)
					module = CreateModule(moduleType);
				return module; 
			} 
		}
		public int Id { get { return id; } }
		public void Hide() {
			if(module != null) 
				module.Visible = false;
		}
		private ModuleBase CreateModule(Type moduleType) {
			if(this.module == null) {
				ConstructorInfo constructorInfoObj = moduleType.GetConstructor(Type.EmptyTypes);
				if (constructorInfoObj == null) 
					throw new ApplicationException(moduleType.FullName + 
						" doesn't have public constructor with empty parameters");					
				ModuleBase ret = constructorInfoObj.Invoke(null) as ModuleBase;
				if(this.description != string.Empty) ret.TutorialInfo.Description = this.description;
				if(this.whatsThisXMLFile != string.Empty) ret.TutorialInfo.WhatsThisXMLFile = this.whatsThisXMLFile;
				if(this.whatsThisCodeFile != string.Empty) ret.TutorialInfo.WhatsThisCodeFile = this.whatsThisCodeFile;
				if(this.aboutFile != string.Empty) ret.TutorialInfo.AboutFile = this.aboutFile;
				return ret;
			}
			return module;
		}
	}
	[ListBindable(false)]
	public class ModuleInfoCollection : CollectionBase {
		static ModuleInfoCollection instance;
		ModuleBase currentModule;
		static ModuleInfoCollection() {
			instance = new ModuleInfoCollection();
		}
		ModuleInfoCollection() : base() {
			this.currentModule = null;
		}
		public static ModuleInfo ModuleInfoById(int id) {
			foreach(ModuleInfo info in instance) {
				if(info.Id == id)
					return info;
			}
			return null;
		}
		public ModuleInfo this[int index] { get { return List[index] as  ModuleInfo; } }
		public static void Add(int id, Type moduleType, string description, string whatsThisXMLFile, string whatsThisCodeFile) {
			Add(id, moduleType, description, whatsThisXMLFile, whatsThisCodeFile, "");
		}
		public static void Add(int id, Type moduleType, string description, string whatsThisXMLFile, string whatsThisCodeFile, string aboutFile) {
			ModuleInfo item = new ModuleInfo(id, moduleType, description, whatsThisXMLFile, whatsThisCodeFile, aboutFile);
			instance.Add(item);
		}
		public static ModuleInfoCollection Instance { get { return instance; } }
		public static void SetCurrentModule(ModuleBase module) {
			instance.currentModule = module;
		}
		public static void ShowModule(ModuleBase module, Control parent) {
			parent.Parent.SuspendLayout();
			if(module == instance.currentModule) return;
			if(instance.currentModule != null)
				instance.currentModule.Hide();
			if(module == null) return;
			module.Visible = false;
			module.Parent = parent;
			module.Dock = DockStyle.Fill;
			module.Visible = true;
			parent.Parent.ResumeLayout(true);
			SetCurrentModule(module);
		}
		public static ModuleBase CurrentModule { get { return instance.currentModule; } }
		void Add(ModuleInfo value) { 
			if(List.IndexOf(value) < 0)
				List.Add(value);
		}
	}
}
