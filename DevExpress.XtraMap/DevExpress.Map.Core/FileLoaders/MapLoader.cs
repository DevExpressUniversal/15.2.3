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
using System.Drawing;
using DevExpress.Utils;
using System.Diagnostics.CodeAnalysis;
namespace DevExpress.Map.Native {
	public class IncorrectUriException : Exception {
		readonly Uri uri;
		public Uri Uri { get { return uri; } }
		public IncorrectUriException(Uri uri) {
			this.uri = uri;
		}
	}
	public class InconsistentDbfException : Exception {
	}
	public class ImageTextInfoCore {
		public string Text { get; set; }
		public Uri ImageUri { get; set; }
		public double ImageScale { get; set; }
		public double TextScale { get; set; }
		public DevExpress.Map.Kml.Model.ColorABGR TextColor { get; set; }
		public IImageTransform ImageTransform { get; set; }
	}
	public abstract class MapLoaderFactoryCore<TItem> {
		[SuppressMessage("Microsoft.Design", "CA1000:Do not declare static members on generic types")]
		public const double DefaultDotSize = -1.0d;
		public abstract TItem CreateDot(CoordPoint point);
		public abstract TItem CreateDot(CoordPoint point, double size);
		public abstract TItem CreateImageAndText(CoordPoint point, ImageTextInfoCore info);
		public abstract TItem CreateLine(CoordPoint point1, CoordPoint point2);
		public abstract TItem CreatePolyline();
		public abstract TItem CreatePolygon();
		public abstract TItem CreatePath();
		public abstract TItem CreateRectangle(CoordPoint location, double width, double height);
		public abstract TItem CreateEllipse(CoordPoint location, double width, double height);
	}
	public class ItemsLoadedEventArgs<TItem> : EventArgs {
		readonly List<TItem> items;
		public List<TItem> Items { get { return items; } }
		public ItemsLoadedEventArgs(List<TItem> items) {
			this.items = items;
		}
	}
	public class BoundsCalculatedEventArgs : EventArgs {
		readonly CoordBounds bounds;
		public CoordBounds Bounds { get { return bounds; } }
		public BoundsCalculatedEventArgs(CoordBounds bounds) {
			this.bounds = bounds;
		}
	}
	public abstract class ResourceLoaderBehaviorBase {
		public abstract Stream OpenResource(Uri resourceUri);
	}
	public abstract class MapLoaderCoreBase<TItem> {
		MapLoaderFactoryCore<TItem> factory;
		CoordObjectFactory coordFactory;
		CoordSystemCore coordinateSystem;
		CoordVector minPoint;
		CoordVector maxPoint;
		protected MapLoaderFactoryCore<TItem> Factory { get { return factory; } }
		protected CoordObjectFactory CoordFactory { get { return coordFactory; } }
		protected virtual CoordBounds BoundingRect { get { return new CoordBounds(minPoint.X, minPoint.Y, maxPoint.X, maxPoint.Y); } }
		public CoordSystemCore CoordinateSystem {
			get { return coordinateSystem; }
			set {
				Guard.ArgumentNotNull(value, "CoordinateSystem");
				coordinateSystem = value;
			}
		}
		public abstract List<TItem> Items { get; }
		public event EventHandler<ItemsLoadedEventArgs<TItem>> ItemsLoaded;
		public event EventHandler<BoundsCalculatedEventArgs> BoundsCalculated;
		protected MapLoaderCoreBase(MapLoaderFactoryCore<TItem> factory) {
			Guard.ArgumentNotNull(factory, "factory");
			this.factory = factory;
		}
		protected internal void SetCoordObjectFactory(CoordObjectFactory factory) {
			Guard.ArgumentNotNull(factory, "factory");
			this.coordFactory = factory;
		}
		protected void RaiseItemsLoaded(List<TItem> items) {
			if (ItemsLoaded != null)
				ItemsLoaded(this, new ItemsLoadedEventArgs<TItem>(items));
		}
		protected void RaiseBoundsCalculated(CoordBounds bounds) {
			if (BoundsCalculated != null)
				BoundsCalculated(this, new BoundsCalculatedEventArgs(bounds));
		}
		protected CoordPoint CreateConvertedPoint(double x, double y) {
			CoordPoint point = CoordinateSystem.CreatePoint(x, y);
			UpdateMinMaxPoint(point);
			return point;
		}
		void UpdateMinMaxPoint(CoordPoint convertedPoint) {
			this.minPoint.X = Math.Min(this.minPoint.X, convertedPoint.XCoord);
			this.minPoint.Y = Math.Min(this.minPoint.Y, convertedPoint.YCoord);
			this.maxPoint.X = Math.Max(this.maxPoint.X, convertedPoint.XCoord);
			this.maxPoint.Y = Math.Max(this.maxPoint.Y, convertedPoint.YCoord);
		}
	}
	public abstract class MapLoaderCore<TItem> : MapLoaderCoreBase<TItem> {
		protected string WorkingFolder { get; set; }
		protected MapLoaderCore(MapLoaderFactoryCore<TItem> factory) : base(factory) {
		}
		protected abstract ResourceLoaderBehaviorBase GetResourceLoaderBehavior(Uri resourceUri);
		protected internal abstract void LoadFromResource(Uri uri, ResourceLoaderBehaviorBase resourceLoaderBehavior);
		protected internal abstract void LoadFromWeb(Uri uri);
		protected internal abstract void LoadFromFile(Uri uri);
		public void Load(Uri uri, bool designMode) {
			WorkingFolder = null;
			string uriString = uri.ToString().ToLower();
			ResourceLoaderBehaviorBase resourceLoaderBehavior = GetResourceLoaderBehavior(uri);
			if (resourceLoaderBehavior != null)
				LoadFromResource(uri, resourceLoaderBehavior);
			else if (uriString.Contains(@"file://"))
				LoadFromFile(uri);
			else if (!designMode)
				LoadFromWeb(uri);
		}
	}
	public abstract class MapFormatLoaderCore<TItem> : MapLoaderCore<TItem> {
		protected MapFormatLoaderCore(MapLoaderFactoryCore<TItem> factory)
			: base(factory) {
		}
		void OnWebFileLoaded(object sender, MapWebLoaderEventArgs e) {
			if (e.Error == null && !e.Cancelled) {
				if (e.Stream == null)
					throw new IncorrectUriException(e.UserState as Uri);
				Load(e.Stream, true);
			}
		}
		protected abstract void LoadStream(Stream stream);
		protected bool CanLoad(Stream stream) {
			return stream != null && !Object.Equals(stream, Stream.Null) && (!stream.CanSeek || stream.Length > 0);
		}
		protected internal override void LoadFromResource(Uri uri, ResourceLoaderBehaviorBase resourceLoaderBehavior) {
			Stream stream = resourceLoaderBehavior.OpenResource(uri);
			if (stream == null)
				throw new IncorrectUriException(uri);
			Load(stream, true);
		}
		protected internal override void LoadFromWeb(Uri uri) {
			MapWebLoader webLoader = new MapWebLoader();
			webLoader.LoadComlete += OnWebFileLoaded;
			webLoader.ReadAsync(uri, uri);
		}
		protected internal override void LoadFromFile(Uri uri) {
			Stream stream = null;
			try {
				FileInfo fileInfo = new FileInfo(uri.LocalPath);
				WorkingFolder = fileInfo.DirectoryName;
				stream = fileInfo.OpenRead();
			} catch { ; }
			if (stream == null)
				throw new IncorrectUriException(uri);
			Load(stream, true);
		}
		protected virtual void Load(Stream stream, bool isDisposable) {
			if (CanLoad(stream))
				LoadStream(stream);
			if (isDisposable)
				stream.Dispose();
			RaiseItemsLoaded(Items);
		}
		public void LoadFormStream(Stream stream) {
			Load(stream, false);
		}
	}
}
