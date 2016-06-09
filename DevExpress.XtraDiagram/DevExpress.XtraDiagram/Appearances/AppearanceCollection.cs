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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraDiagram.Appearance {
	public class DiagramAppearanceCollection : BaseAppearanceCollection {
		DiagramAppearanceObject hRuler;
		DiagramAppearanceObject vRuler;
		DiagramAppearanceObject shape;
		DiagramAppearanceObject connector;
		public DiagramAppearanceCollection() {
			this.shape = CreateDiagramAppearance("Shape");
			this.connector = CreateDiagramAppearance("Connector");
			this.hRuler = CreateDiagramAppearance("HRuler");
			this.vRuler = CreateDiagramAppearance("VRuler");
		}
		#region Shape
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramAppearanceObject Shape { get { return shape; } }
		bool ShouldSerializeShape() { return Shape.ShouldSerialize(); }
		void ResetShape() { Shape.Reset(); }
		#endregion
		#region Connector
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramAppearanceObject Connector { get { return connector; } }
		bool ShouldSerializeConnector() { return Connector.ShouldSerialize(); }
		void ResetConnector() { Connector.Reset(); }
		#endregion
		#region HRuler
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramAppearanceObject HRuler { get { return hRuler; } }
		bool ShouldSerializeHRuler() { return HRuler.ShouldSerialize(); }
		void ResetHRuler() { HRuler.Reset(); }
		#endregion
		#region VRuler
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DiagramAppearanceObject VRuler { get { return vRuler; } }
		bool ShouldSerializeVRuler() { return VRuler.ShouldSerialize(); }
		void ResetVRuler() { VRuler.Reset(); }
		#endregion
		public override string ToString() {
			return string.Empty;
		}
		protected virtual DiagramAppearanceObject CreateDiagramAppearance(string name) {
			return CreateAppearance(name) as DiagramAppearanceObject;
		}
		protected override AppearanceObject CreateAppearanceInstance(AppearanceObject parent, string name) {
			return new DiagramAppearanceObject(this, parent, name);
		}
		protected override AppearanceObject CreateNullAppearance() { return null; }
	}
}
