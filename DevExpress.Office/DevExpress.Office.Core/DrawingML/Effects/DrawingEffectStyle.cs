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

using DevExpress.Office;
using DevExpress.Office.DrawingML;
using DevExpress.Office.History;
using DevExpress.Utils;
namespace DevExpress.Office.Drawing {
	#region ContainerEffectType
	public enum ContainerEffectType { 
		DirectedAcyclicGraph, 
		List
	}
	#endregion
	#region DrawingEffectStyle
	public class DrawingEffectStyle : ISupportsCopyFrom<DrawingEffectStyle> {
		#region Fields
		readonly ContainerEffect containerEffect;
		readonly Scene3DProperties scene3DProperies;
		readonly Shape3DProperties shape3DProperties;
		#endregion
		public DrawingEffectStyle(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.containerEffect = new ContainerEffect(documentModel);
			this.scene3DProperies = new Scene3DProperties(documentModel);
			this.shape3DProperties = new Shape3DProperties(documentModel);
		}
		#region Properties
		public ContainerEffect ContainerEffect { get { return containerEffect; } }
		public Scene3DProperties Scene3DProperties { get { return scene3DProperies; } }
		public Shape3DProperties Shape3DProperties { get { return shape3DProperties; } }
		public bool IsDefault { get { return containerEffect.IsEmpty && scene3DProperies.IsDefault && shape3DProperties.IsDefault; } }
		#endregion
		public void ApplyEffects(IDrawingEffectVisitor visitor) {
			ContainerEffect.ApplyEffects(visitor); 
		}
		public DrawingEffectStyle CloneTo(IDocumentModel documentModel) {
			DrawingEffectStyle result = new DrawingEffectStyle(documentModel);
			result.CopyFrom(this);
			return result;
		}
		#region ISupportsCopyFrom<DrawingEffectStyle> Members
		public void CopyFrom(DrawingEffectStyle value) {
			containerEffect.CopyFrom(value.containerEffect);
			scene3DProperies.CopyFrom(value.scene3DProperies);
			shape3DProperties.CopyFrom(value.shape3DProperties);
		}
		#endregion
	}
	#endregion
}
