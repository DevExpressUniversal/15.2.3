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
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Internal;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Utils.Serializing;
using System.IO;
using DevExpress.Diagram.Core.TypeConverters;
using System.Globalization;
namespace DevExpress.Diagram.Core {
	public interface IDiagramRoot : IDiagramContainer {
		IDiagramControl Diagram { get; }
	}
	public interface IDiagramItem {
		IList<IDiagramItem> NestedItems { get; }
		DiagramItemController Controller { get; }
		bool IsSelected { get; set; }
		[XtraSerializableProperty]
		bool IsTabStop { get; set; }
		[XtraSerializableProperty]
		Point Position { get; set; }
		[XtraSerializableProperty]
		Size Size { get; set; }
		[XtraSerializableProperty]
		double Weight { get; set; }
		Size MinSize { get; }
		Size ActualSize { get; }
		[XtraSerializableProperty]
		Thickness Padding { get; set; }
		[XtraSerializableProperty]
		Sides Anchors { get; set; }
		[XtraSerializableProperty]
		bool CanMove { get; set; }
		[XtraSerializableProperty]
		bool CanDelete { get; set; }
		[XtraSerializableProperty]
		bool CanCopy { get; set; }
		[XtraSerializableProperty]
		bool CanResize { get; set; }
		[XtraSerializableProperty]
		bool CanSnapToThisItem { get; set; }
		[XtraSerializableProperty]
		bool CanSnapToOtherItems { get; set; }
		[XtraSerializableProperty]
		bool CanSelect { get; set; }
		[XtraSerializableProperty]
		TextAlignment TextAlignment { get; set; }
		[XtraSerializableProperty]
		VerticalAlignment VerticalContentAlignment { get; set; }
		[XtraSerializableProperty]
		FontFamily FontFamily { get; set; }
		[XtraSerializableProperty]
		FontStretch FontStretch { get; set; }
		[XtraSerializableProperty]
		FontWeight FontWeight { get; set; }
		[XtraSerializableProperty]
		FontStyle FontStyle { get; set; }
		[XtraSerializableProperty]
		double FontSize { get; set; }
		[XtraSerializableProperty]
		TextDecorationCollection TextDecorations { get; set; }
		[XtraSerializableProperty]
		Brush Background { get; set; }
		[XtraSerializableProperty]
		Brush Foreground { get; set; }
		[XtraSerializableProperty]
		double StrokeThickness { get; set; }
		[XtraSerializableProperty]
		Brush Stroke { get; set; }
		[XtraSerializableProperty]
		DoubleCollection StrokeDashArray { get; set; }
		[XtraSerializableProperty]
		ISelectionLayer SelectionLayer { get; set; }
		[XtraSerializableProperty]
		DiagramItemStyleId ThemeStyleId { get; set; }
		[XtraSerializableProperty]
		object CustomStyleId { get; set; }
		[XtraSerializableProperty]
		DiagramThemeColorId? ForegroundId { get; set; }
		[XtraSerializableProperty]
		DiagramThemeColorId? BackgroundId { get; set; }
		[XtraSerializableProperty]
		DiagramThemeColorId? StrokeId { get; set; }
		[XtraSerializableProperty]
		double Angle { get; set; }
		[XtraSerializableProperty]
		bool CanRotate { get; set; }
		IEnumerable<PropertyDescriptor> GetEditableProperties();
		void SetCustomStyles(IDiagramItemStyle[] styles);
		PropertyDescriptor GetRealProperty(PropertyDescriptor pd);
	}
	public interface IDiagramShape : IDiagramItem {
		[XtraSerializableProperty]
		ShapeDescription Shape { get; set; }
		[XtraSerializableProperty]
		string Content { get; set; }
		[XtraSerializableProperty]
		DoubleCollection Parameters { get; set; }
	}
	public interface IDiagramContainer : IDiagramItem {
		[XtraSerializableProperty]
		bool IsSnapScope { get; set; }
		[XtraSerializableProperty]
		AdjustBoundaryBehavior AdjustBoundsBehavior { get; set; }
	}
	public static class IDiagramItemExtensions {
		public static void NotifyChanged(this IDiagramItem item, ItemChangedKind kind) {
			item.GetRootDiagram().Do(x => x.Controller.ItemChanged(item, kind));
		}
		public static int GetIndexInOwnerCollection(this IDiagramItem item) {
			return item.OwnerCollection().IndexOf(item);
		}
		public static IDiagramItem Owner(this IDiagramItem item) {
			return item.Controller.Owner;
		}
		public static IList<IDiagramItem> OwnerCollection(this IDiagramItem item) {
			return item.Owner().NestedItems;
		}
		public static Rect GetDiagramRect(this IDiagramItem diagramItem, Rect itemRect) {
			return diagramItem.GetDiagramRect(new Rect_Angle(itemRect, 0)).Rect;
		}
		internal static Point_Angle GetDiagramPoint(this IDiagramItem diagramItem, Point_Angle point) {
			return diagramItem.GetParentsIncludingSelf().Aggregate(point,
				(center, item) => new Point_Angle(center.Point.OffsetPoint(new Point(item.Padding.Left, item.Padding.Top)).Rotate(item.Angle, item.GetRotationCenter()).OffsetPoint(item.Position), center.Angle + item.Angle));
		}
		public static Rect_Angle GetDiagramRect(this IDiagramItem diagramItem, Rect_Angle itemRect) {
			var actualDiagramRectCenter = diagramItem.GetDiagramPoint(new Point_Angle(itemRect.Rect.GetCenter(), itemRect.Angle));
			return new Rect_Angle(actualDiagramRectCenter.Point.GetCenteredRect(itemRect.Rect.Size), actualDiagramRectCenter.Angle);
		}
		public static Point DiagramPosition(this IDiagramItem item) {
			return item.ActualDiagramBounds().Location;
		}
		public static Point ToDiagramPoint(this IDiagramItem item, Point localPoint) {
			return localPoint.OffsetPoint(item.DiagramPosition());
		}
		public static Point GetItemRelativePosition(this IDiagramItem diagramItem, Point diagramPoint) {
			return diagramItem.GetParentsIncludingSelf().Reverse().Aggregate(diagramPoint,
				(point, item) => point.OffsetPoint(item.Position.InvertPoint()).Rotate(-item.Angle, item.GetRotationCenter()).OffsetPoint(new Point(-item.Padding.Left, -item.Padding.Top)));
		}
		public static Rect GetItemRelativeRect(this IDiagramItem item, Rect rect) {
			Point localCenter = item.GetItemRelativePosition(rect.GetCenter());
			return localCenter.GetCenteredRect(rect.Size);
		}
		public static bool IsChild(this IDiagramItem child, IDiagramItem parent) {
			return child.GetParents().Contains(parent);
		}
		public static IEnumerable<IDiagramItem> GetParentsOnly(this IEnumerable<IDiagramItem> items) {
			return items.Where(child => !items.Any(x => child.IsChild(x)));
		}
		public static IDiagramContainer FindContainerItemAtPoint(this IDiagramContainer root, Point point) {
			return (IDiagramContainer)root.FindItemAtPoint(point, x => x is IDiagramContainer);
		}
		public static IDiagramItem FindItemAtPoint(this IDiagramContainer root, Point point, Predicate<IDiagramItem> condition) {
			IDiagramItem result = root;
			var margin = root.GetRootDiagram().GlueToItemDistance;
			root.IterateChildren(item => {
				var rotatedPoint = point.Rotate(-item.Angle, item.GetDiagramRotationCenter());
				if(condition(item) && item.ActualDiagramBounds().InflateRect(margin).Contains(rotatedPoint)) {
					result = item;
				}
				return false;
			});
			return result;
		}
		public static void IterateChildren(this IDiagramItem owner, Func<IDiagramItem, bool> func) {
			owner.NestedItems.Flatten(x => func(x) ? Enumerable.Empty<IDiagramItem>() : x.NestedItems).ForEach(x => { });
		}
		public static void IterateSelfAndChildren(this IDiagramItem owner, Action<IDiagramItem> action) {
			action(owner);
			owner.IterateChildren(x => { action(x); return false; });
		}
		public static void IterateChildren(this IDiagramItem owner, Action<IDiagramItem> action) {
			owner.IterateChildren(x => { action(x); return false; });
		}
		public static IEnumerable<IDiagramItem> GetSelfAndChildren(this IDiagramItem item) {
			return item.Yield().Flatten(x => x.NestedItems);
		}
		public static IEnumerable<IDiagramItem> GetChildren(this IDiagramItem item) {
			return item.GetSelfAndChildren().Skip(1);
		}
		#region finder
		public static DiagramItemFinder GetFinder(this IDiagramItem item) {
			return new DiagramItemFinder(item);
		}
		public static IEnumerable<IDiagramItem> GetParentsIncludingSelf(this IDiagramItem owner) {
			return LinqExtensions.Unfold(owner, x => x.Owner(), x => x == null);
		}
		public static IEnumerable<IDiagramItem> GetParents(this IDiagramItem owner) {
			return owner.GetParentsIncludingSelf().Skip(1);
		}
		public static int GetLevel(this IDiagramItem item) {
			return item.GetParents().Count();
		}
		public static IDiagramControl GetRootDiagram(this IDiagramItem item) {
			return (item.GetParentsIncludingSelf().Last() as IDiagramRoot).With(x => x.Diagram);
		}
		public static bool IsInDiagram(this IDiagramItem item) {
			return item.GetRootDiagram() != null;
		}
		#endregion
		public static void UpdateSize(this IDiagramRoot rootItem, IDiagramControl diagram) {
			rootItem.Size = diagram.PageSize;
		}
		public static void NotifyInteractionChanged(this IDiagramItem item) {
			item.NotifyChanged(ItemChangedKind.Interaction);
		}
		public static void SetBounds(this IDiagramItem item, Rect bounds) {
			item.Controller.Bounds = bounds;
		}
		public static Rect ActualDiagramBounds(this IDiagramItem item) {
			return item.RotatedDiagramBounds().Rect;
		}
		public static Rect ActualBounds(this IDiagramItem item) {
			return new Rect(item.Position, item.ActualSize);
		}
		public static Rect ClientBounds(this IDiagramItem item) {
			return item.ActualBounds().InflateRect(item.Padding.Invert());
		}
		public static Rect ClientDiagramBounds(this IDiagramItem item) {
			return item.RotatedClientDiagramBounds().Rect;
		}
		public static Rect_Angle RotatedClientDiagramBounds(this IDiagramItem item) {
			return item.Owner().GetDiagramRect(new Rect_Angle(item.ClientBounds(), item.Angle));
		}
		public static double CoerceWeight(this IDiagramItem item, double weight) {
			return Math.Max(weight, 0);
		}
		public static double CoerceAngle(this IDiagramItem item, double angle) {
			return MathHelper.NormalizeRotationAngle(angle);
		}
		public static DiagramRootController Controller(this IDiagramRoot item) {
			return (DiagramRootController)item.Controller;
		}
		#region Themes
		public static DiagramItemStyleId GetActualStyleId(this IDiagramItem item) {
			if(item.ThemeStyleId == DiagramItemStyleId.DefaultStyleId)
				return item.Controller.GetDefaultStyleId();
			return item.ThemeStyleId;
		}
		public static DiagramItemStyle GetStyle(this IDiagramItem item, IDiagramControl diagram, DiagramItemStyleId styleId) {
			var defaultThemeStyle = diagram.Theme.With(theme => theme.GetDiagramItemStyle(styleId));
			return item.Controller.CorrectThemeStyle(defaultThemeStyle, diagram.Theme.With(x => x.ColorPalette));
		}
		#endregion
		#region Rotation
		public static Point[] GetActualConnectionPoints(this IDiagramItem item) {
			return item.Controller.GetConnectionPoints().Select(p => p.Rotate(item.Angle, item.GetRotationCenter())).ToArray();
		}
		public static Point[] GetDiagramConnectionPoints(this IDiagramItem item) {
			return item.GetActualConnectionPoints().Select(point => point.OffsetPoint(item.DiagramPosition())).ToArray();
		}
		public static Point GetDiagramRotationCenter(this IDiagramItem item) {
			return item.DiagramPosition().OffsetPoint(item.GetRotationCenter());
		}
		public static Point GetRotationCenter(this IDiagramItem item) {
			return new Point(item.ActualSize.Width / 2, item.ActualSize.Height / 2);
		}
		public static Rect_Angle RotatedBounds(this IDiagramItem item) {
			return new Rect_Angle(item.ActualBounds(), item.Angle);
		}
		public static Rect_Angle RotatedDiagramBounds(this IDiagramItem item) {
			return item.Owner().GetDiagramRect(item.RotatedBounds());
		}
		public static void UpdateRouteForAttachedConnectors(this IDiagramItem item, Transaction transaction) {
			var diagram = item.GetRootDiagram();
			if(diagram == null)
				return;
			diagram.Controller.Connectors.UpdateRouteForAttachedConnectors(transaction, item);
		}
		#endregion
	}
	[TypeConverter(typeof(DiagramItemFinderPathConverter))]
	public sealed class DiagramItemFinderPath : EquatableObject<DiagramItemFinderPath>, IComparable<DiagramItemFinderPath> {
		public class DiagramItemFinderPathConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if(destinationType == null) {
					throw new ArgumentNullException("destinationType");
				}
				if((destinationType == typeof(string)) && (value is DiagramItemFinderPath)) {
					var itemFinderPath = (DiagramItemFinderPath)value;
					return string.Join(GetListSeparator().ToString(), itemFinderPath.path.Select(x => x.ToString()));
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				string val = value as string;
				if(!string.IsNullOrEmpty(val)) {
					var path = val.Split(GetListSeparator()).Select(x => int.Parse(x));
					return new DiagramItemFinderPath(path.ToArray());
				}
				return base.ConvertFrom(context, culture, value);
			}
			protected char GetListSeparator() {
				return CultureInfo.InvariantCulture.TextInfo.ListSeparator.First();
			}
		}
		public static DiagramItemFinderPath Create(IDiagramItem item, IList<IDiagramItem> roots) {
			var rootToIndexMap = roots
				.Select((root, index) => new { root, index })
				.ToDictionary(x => x.root, x => (int?)x.index);
			var current = item;
			var parentsPath = item.GetParentsIncludingSelf()
				.Reverse()
				.SkipWhile(x => rootToIndexMap.GetValueOrDefault(x) == null)
				.ToArray();
			if(!parentsPath.Any())
				return new DiagramItemFinderPath(EmptyArray<int>.Instance);
			var rootIndex = rootToIndexMap.GetValueOrDefault(parentsPath.First()).Value;
			var path = parentsPath
				.Skip(1)
				.Select(x => x.GetIndexInOwnerCollection());
			return new DiagramItemFinderPath(rootIndex.Yield().Concat(path).ToArray());
		}
		readonly int[] path;
		public bool IsEmpty { get { return path.Length == 0; } }
		public DiagramItemFinderPath(int[] path) {
			this.path = path;
		}
		public IDiagramItem FindItem(IDiagramControl diagram, IList<IDiagramItem> roots) {
			IDiagramItem root = diagram.RootItem();
			if(path.Any())
				root = roots[path.First()];
			return path.Skip(1).Aggregate<int, IDiagramItem>(root, (x, index) => {
				if(x == null || index >= x.NestedItems.Count) {
					return null;
				}
				return x.NestedItems[index];
			});
		}
		protected override bool Equals(DiagramItemFinderPath other) {
			return path.SequenceEqual(other.path);
		}
		static readonly IComparer<int> intComparer = Comparer<int>.Default;
		int IComparable<DiagramItemFinderPath>.CompareTo(DiagramItemFinderPath other) {
			for(int i = 0; i < Math.Min(path.Length, other.path.Length); i++) {
				var compare = intComparer.Compare(path[i], other.path[i]);
				if(compare != 0)
					return compare;
			}
			return intComparer.Compare(path.Length, other.path.Length);
		}
	}
	public sealed class DiagramItemFinder : EquatableObject<DiagramItemFinder>, IItemFinder<IDiagramItem> {
		internal readonly DiagramItemFinderPath Path;
		IDiagramControl diagram;
		public DiagramItemFinder(IDiagramItem item) {
			if(!item.IsInDiagram())
				throw new InvalidOperationException();
			this.diagram = item.GetRootDiagram();
			this.Path = DiagramItemFinderPath.Create(item, diagram.Items());
		}
		internal void SetDiagram(IDiagramControl diagram) {
			this.diagram = diagram;
		}
		public IDiagramItem FindItem() {
			return Path.FindItem(diagram, diagram.Items());
		}
		protected override bool Equals(DiagramItemFinder other) {
			return diagram == other.diagram && Path.Equals(other.Path);
		}
	}
	public static class IDiagramShapeExtensions {
		public static DiagramShapeController Controller(this IDiagramShape shape) {
			return (DiagramShapeController)shape.Controller;
		}
		public static DoubleCollection GetParameters(this IDiagramShape shape) {
			return new DoubleCollection(shape.Shape.ParameterCollection.Parameters.Select(param => shape.GetParameterValue(param)));
		}
		internal static int GetParameterIndex(this IDiagramShape shape, ParameterDescription parameter) {
			return Array.IndexOf(shape.Shape.ParameterCollection.Parameters, parameter);
		}
		internal static double GetParameterValue(this IDiagramShape shape, ParameterDescription parameter) {
			int parameterIndex = shape.GetParameterIndex(parameter);
			if(shape.Parameters != null)
				return shape.Parameters.ElementAt(parameterIndex);
			return parameter.DefaultValue;
		}
		internal static Point GetParameterPoint(this IDiagramShape shape, ParameterDescription parameter) {
			return parameter.GetPoint(shape.Size, shape.GetParameters().ToArray(), shape.GetParameterValue(parameter));
		}
		public static Point GetActualParameterPoint(this IDiagramShape shape, ParameterDescription parameter) {
			return shape.GetParameterPoint(parameter).OffsetPoint(shape.Position).Rotate(shape.Angle, shape.GetDiagramRotationCenter());
		}
	}
}
