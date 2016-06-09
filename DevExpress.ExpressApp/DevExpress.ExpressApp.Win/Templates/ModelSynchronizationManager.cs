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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;
using System.Drawing;
namespace DevExpress.ExpressApp.Win.Templates {
	[ToolboxItem(false)] 
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[Designer("DevExpress.ExpressApp.Design.ModelSynchronizationManagerDesigner, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.ComponentModel.Design.IDesigner))]
	[DesignerCategory("Component")]
	public partial class ModelSynchronizationManager : Component, IModelSynchronizable {
		private List<IModelSynchronizable> modelSynchronizableComponents;
		public ModelSynchronizationManager() {
			modelSynchronizableComponents = new List<IModelSynchronizable>();
			InitializeComponent();
		}
		public ModelSynchronizationManager(IContainer container)
			: this() {
			container.Add(this);
		}
		public  void ApplyModel() {
			foreach(IModelSynchronizable synchronizer in modelSynchronizableComponents) {
				synchronizer.ApplyModel();
			}
		}
		public  void SynchronizeModel() {
			foreach(IModelSynchronizable synchronizer in modelSynchronizableComponents) {
				synchronizer.SynchronizeModel();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.ExpressApp.Design.ComponentsCollectionEditor, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		[ReadOnly(false), TypeConverter(typeof(CollectionConverter))]
		public List<IModelSynchronizable> ModelSynchronizableComponents {
			get {
				return modelSynchronizableComponents;
			}
			set { modelSynchronizableComponents = value; }
		}
	}
}
