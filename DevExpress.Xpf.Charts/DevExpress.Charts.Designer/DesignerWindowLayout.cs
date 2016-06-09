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

using System.Windows;
using DevExpress.Utils.Serializing;
namespace DevExpress.Charts.Designer.Native {
	public class DesignerWindowLayout {
		const string DefaultKey = "DesignerWindowLayout";
		const string DXChartsRegistryPath = "Software\\Developer Express\\DXCharts\\";
		bool maximized = false;
		bool defaultMaximized = false;
		Size size = Size.Empty;
		Size defaultSize = Size.Empty;
		bool SizeChanged { get { return size != defaultSize; } }
		bool MaximizedChanged { get { return maximized != defaultMaximized; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public bool Maximized {
			get { return maximized; }
			set { maximized = value; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		public Size Size {
			get { return size; }
			set { size = value; }
		}
		public DesignerWindowLayout() {
			LoadLayoutFromRegistry();
		}
		void SaveLayout(string registryNode) {
			new RegistryXtraSerializer().SerializeObject(this, registryNode, DefaultKey);
		}
		void LoadLayout(string registryNode) {
			new RegistryXtraSerializer().DeserializeObject(this, registryNode, DefaultKey);
		}
		void LoadLayoutFromRegistry() {
			try {
				LoadLayout(DXChartsRegistryPath);
				defaultSize = size;
				defaultMaximized = maximized;
			}
			catch {
			}
		}
		public void SaveLayoutToRegistry() {
			if (SizeChanged || MaximizedChanged)
				SaveLayout(DXChartsRegistryPath);
		}
	}
}
