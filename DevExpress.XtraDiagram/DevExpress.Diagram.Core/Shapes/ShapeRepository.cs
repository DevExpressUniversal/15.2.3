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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Shapes.Native;
using System.Windows;
using DevExpress.Diagram.Core.Shapes;
using DevExpress.Utils;
using DevExpress.Internal;
namespace DevExpress.Diagram.Core {
	public static class DiagramToolboxRegistrator {
		#region DEBUGTEST
#if DEBUGTEST
		public static int GetShapeTemplateInvokeCount { get; set; }
		public static void LoadStencilForTests(string stencilId) {
			LoadStencil(stencilId);
		}
#endif
		#endregion
		static readonly Dictionary<string, DiagramStencil> stencilsCore;
		public static IEnumerable<DiagramStencil> Stencils { get { return stencilsCore.Values; } }
		static DiagramToolboxRegistrator() {
			stencilsCore = new Dictionary<string, DiagramStencil>();
			LoadStencil("BasicShapes");
			LoadStencil("BasicFlowchartShapes");
			LoadStencil("SDLDiagramShapes");
			LoadStencil("ArrowShapes");
			LoadStencil("SoftwareIcons");
			LoadStencil("DecorativeShapes");
		}
		public static void RegisterStencil(DiagramStencil stencil) {
			if(stencil != null)
				stencilsCore[stencil.Id] = stencil;
		}
		public static void UnregisterStencil(DiagramStencil stencil) {
			if(stencil != null)
				stencilsCore.Remove(stencil.Id);
		}
		public static DiagramStencil GetStencil(string stencilId) {
			DiagramStencil stencil = null;
			if(stencilId != null && stencilsCore.TryGetValue(stencilId, out stencil))
				return stencil;
			return null;
		}
		public static DiagramStencil GetStencilByShape(ShapeDescription shape) {
			return stencilsCore.Values
				.FirstOrDefault(stencil => stencil.ContainsShape(shape));
		}
		public static void RegisterShapes(string stencilId, Func<string> getStencilName, ResourceDictionary dictionary, Func<string, string> getShapeName) {
			DiagramStencil stencil = new DiagramStencil(stencilId, getStencilName);
			DiagramToolboxRegistrator.RegisterStencil(stencil);
			var keys = dictionary.Keys.Cast<ShapeKey>().OrderBy(key => key.Id).ToList();
			foreach(ShapeKey shapeKey in keys) {
				string shapeId = shapeKey.ResourceKey;
				ShapeDescription shapeDescription = null;
				if(ShapeRegistratorHelper.IsTemplateShape(stencilId, shapeId))
					shapeDescription = ShapeDescription.CreateTemplateShape(shapeId, () => getShapeName(shapeId), () => GetShapeTemplate(dictionary, shapeKey));
				else
					shapeDescription = ShapeRegistratorHelper.CreateShape(stencilId, shapeId);
				stencil.RegisterShape(shapeDescription);
			}
		}
		static ShapeTemplate GetShapeTemplate(ResourceDictionary dictionary, ShapeKey shapeKey) {
#if DEBUGTEST
			GetShapeTemplateInvokeCount++;
#endif
			return (ShapeTemplate)dictionary[shapeKey];
		}
		static void LoadStencil(string stencilId) {
			ImageSourceHelper.RegisterPackScheme();
			var dictionary = new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(ShapeTemplate).Assembly, string.Format("Shapes/Resources/{0}.xaml", stencilId)) };
			Func<string> getStencilName = () => DiagramControlLocalizer.GetString(ShapeRegistratorHelper.GetCategoryStringId(stencilId));
			Func<string, string> getShapeName = shapeId => DiagramControlLocalizer.GetString(ShapeRegistratorHelper.GetShapeStringId(stencilId, shapeId));
			RegisterShapes(stencilId, getStencilName, dictionary, getShapeName);
		}
		internal static void LoadArrows() {
			ImageSourceHelper.RegisterPackScheme();
			var dictionary = new ResourceDictionary { Source = AssemblyHelper.GetResourceUri(typeof(ShapeTemplate).Assembly, "Shapes/Resources/Arrows.xaml") };
			Func<string, string> getArrowName = arrowId => DiagramControlLocalizer.GetString(ShapeRegistratorHelper.GetArrowStringId(arrowId));
			var keys = dictionary.Keys.Cast<ShapeKey>().OrderBy(key => key.Id).ToList();
			foreach(ShapeKey arrowKey in keys) {
				string arrowId = arrowKey.ResourceKey;
				ArrowDescription arrow = ArrowDescription.Create(arrowId, () => getArrowName(arrowId), (ArrowTemplate)dictionary[arrowKey]);
				ArrowDescriptions.RegisterArrow(arrowId, arrow);
			}
		}
	}
	public class DiagramStencil {
		readonly string idCore;
		readonly Func<string> getName;
		readonly Dictionary<string, ItemTool> toolsCore;
		public string Id { get { return idCore; } }
		public string Name { get { return getName(); } }
		public IEnumerable<ShapeDescription> Shapes { get { return toolsCore.Values.OfType<ShapeTool>().Select(x => x.Shape); } }
		public IEnumerable<ItemTool> Tools { get { return toolsCore.Values; } }
		public DiagramStencil(string id, Func<string> getName) {
			this.idCore = id;
			this.toolsCore = new Dictionary<string, ItemTool>();
			this.getName = getName;
		}
		public void RegisterShape(ShapeDescription shape) {
			if(shape != null)
				RegisterTool(new ShapeTool(shape));
		}
		public void UnregisterShape(ShapeDescription shape) {
			if(shape != null)
				UnregisterTool(shape.Id);
		}
		public ShapeDescription GetShape(string shapeId) {
			return (GetTool(shapeId) as ShapeTool).With(x => x.Shape);
		}
		public bool ContainsShape(string shapeId) { return GetShape(shapeId) != null; }
		public bool ContainsShape(ShapeDescription shape) { return GetShape(shape.Id) == shape; }
		public void RegisterTool(ItemTool tool) {
			if(tool != null)
				toolsCore[tool.ToolId] = tool;
		}
		public void UnregisterTool(ItemTool tool) {
			if(tool != null)
				UnregisterTool(tool.ToolId);
		}
		public void UnregisterTool(string toolId) {
			toolsCore.Remove(toolId);
		}
		public ItemTool GetTool(string toolId) {
			ItemTool tool = null;
			if(toolId != null && toolsCore.TryGetValue(toolId, out tool))
				return tool;
			return null;
		}
	}
}
