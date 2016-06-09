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

using System.ComponentModel;
using System.IO;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
namespace DevExpress.Xpf.Charts {
	public abstract class ModelBase : DependencyObject, IChartElement {
		#region IChartElement implementation
		IChartElement owner;
		IChartElement IOwnedElement.Owner { get { return owner; } set { SetOwner(value); } }
		void IChartElement.AddChild(object child) { }
		void IChartElement.RemoveChild(object child) { }
		bool IChartElement.Changed(ChartUpdate args) {
			Changed(args);
			return true;
		}
		ViewController INotificationOwner.Controller { get { return owner == null ? null : owner.Controller; } }
		protected virtual void Changed(ChartUpdate args) {
			if (owner != null)
				owner.Changed(args);
		}
		internal void SetOwner(IChartElement owner) {
			this.owner = owner;
		}
		#endregion
		const string modelRootPath = @"component/Models/";
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return this.GetType().Name; } }
		static DependencyObject LoadInternal(string source) {
			string path = string.IsNullOrEmpty(source) ? string.Empty : Path.Combine(modelRootPath, source);
			return XamlLoaderHelper<DependencyObject>.LoadFromResource(path);
		}
		static DependencyObject LoadExternal(string source) {
			string path = string.IsNullOrEmpty(source) ? string.Empty : ";component/" + source;
			return XamlLoaderHelper<DependencyObject>.LoadFromUserResource(path);
		}
		protected static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ModelBase model = d as ModelBase;
			if(model != null)
				model.ClearCache();
			ChartElementHelper.UpdateWithClearDiagramCache(d, e);
		} 
		protected DependencyObject LoadObject(string source, bool loadFromResources, bool internalModel) {
			if(!loadFromResources)
				return XamlLoaderHelper<DependencyObject>.LoadFromFile(source);
			if(internalModel) {
				DependencyObject obj = LoadInternal(source);
				if(obj != null)
					return obj;
			}
			return LoadExternal(source);
		}
		protected internal abstract void ClearCache();
	}
}
