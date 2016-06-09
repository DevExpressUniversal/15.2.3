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

using DevExpress.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core.Layout {
	public enum UseConnectionsMode {
		OutgoingAndIncoming,
		OutgoingOnly,
	}
	public static class GraphOperations {
#if DEBUGTEST
		internal static ISet<T>[] SplitToConnectedComponentsForTests<T>(IEnumerable<Connection<T>> connections) {
			return SplitToConnectedComponentsCore(connections, GetItemToLinksMap(connections));
		}
		internal static Func<T, IEnumerable<T>> FindWeightedForTests<T>(T root, IEnumerable<Connection<T>> connections, UseConnectionsMode useConnectionsMode = UseConnectionsMode.OutgoingAndIncoming) {
			var itemToLinksMap = GetItemToLinksMap(connections);
			var map = FindWeightedCore(root, x => itemToLinksMap(x).GetLinks(useConnectionsMode));
			return x => map[x];
		}
#endif
		public static TreeAndItems<T>[] SplitToForest<T>(IEnumerable<Connection<T>> connections) {
			var itemToLinksMap = GetItemToLinksMap(connections);
			var components = SplitToConnectedComponentsCore(connections, itemToLinksMap);
			return components.Select(component => {
				var root = ChooseRoot(component, itemToLinksMap);
				var wieghtedTree = FindWeightedCore(root, x => itemToLinksMap(x).Outgoing);
				Func<T, IEnumerable<T>> getChildren = x => wieghtedTree[x];
				var itemsNotInTree = component.Except(root.Yield().Flatten(getChildren)).ToArray();
				return new TreeAndItems<T>(new Tree<T>(root, getChildren), itemsNotInTree);
			}).ToArray();
		}
		static T ChooseRoot<T>(IEnumerable<T> items, Func<T, Links<T>> itemToLinksMap) {
			return items
				.MaxBy(x => {
					var links = itemToLinksMap(x);
					return links.Incoming.Length == 0 
						? (long)int.MaxValue + links.Outgoing.Length
						: links.Outgoing.Length - links.Incoming.Length;
				});
		}
		static ISet<T>[] SplitToConnectedComponentsCore<T>(IEnumerable<Connection<T>> connections, Func<T, Links<T>> itemToLinksMap) {
			var allItems = Enumerable.Concat(
				connections.Select(x => x.From),
				connections.Select(x => x.To)
			)
			.Where(x => x != null);
			var allItemsSet = new HashSet<T>(allItems);
			var components = new List<ISet<T>>();
			while(allItemsSet.Any()) {
				var map = FindWeightedCore(allItemsSet.First(), x => itemToLinksMap(x).GetLinks());
				allItemsSet.ExceptWith(map.Keys);
				components.Add(new HashSet<T>(map.Keys));
			}
			return components.ToArray();
		}
		static IDictionary<T, IList<T>> FindWeightedCore<T>(T root, Func<T, IEnumerable<Link<T>>> getLinks) {
			var connectedItems = new Dictionary<T, IList<T>>();
			var adjacentConnections = new SortedDictionary<double, Queue<Connection<T>>>();
			Action<T> addLinks = item => {
				if(connectedItems.ContainsKey(item))
					return;
				getLinks(item)
					.Where(x => !connectedItems.ContainsKey(x.To))
					.Select(x => new Connection<T>(item, x))
					.ForEach(x => adjacentConnections.GetOrAdd(x.Weight, () => new Queue<Connection<T>>()).Enqueue(x));
				connectedItems.GetOrAdd(item, () => new List<T>());
			};
			addLinks(root);
			while(adjacentConnections.Any()) {
				var minKey = adjacentConnections.Keys.First();
				var minWeightConnections = adjacentConnections[minKey];
				var minConnection = minWeightConnections.Dequeue();
				if(!minWeightConnections.Any())
					adjacentConnections.Remove(minKey);
				if(!connectedItems.ContainsKey(minConnection.To))
					connectedItems[minConnection.From].Add(minConnection.To);
				addLinks(minConnection.To);
			}
			return connectedItems;
		}
		static Func<T, Links<T>> GetItemToLinksMap<T>(IEnumerable<Connection<T>> connections) {
			var fullConnections = connections.Where(x => x.From != null && x.To != null && !Equals(x.From, x.To));
			var outgoing = fullConnections
				.GroupBy(x => x.From)
				.ToDictionary(x => x.Key, x => x.Select(connection => new Link<T>(connection.To, connection.Weight)).ToArray());
			var incoming = fullConnections
				.GroupBy(x => x.To)
				.ToDictionary(x => x.Key, x => x.Select(connection => new Link<T>(connection.From, connection.Weight)).ToArray());
			var emptyArray = EmptyArray<Link<T>>.Instance;
			return x  => new Links<T>(
				outgoing.GetValueOrDefault(x, emptyArray),
				incoming.GetValueOrDefault(x, emptyArray)
			);
		}
	}
	public struct Connection<T> {
		public static bool operator ==(Connection<T> left, Connection<T> right) {
			return Equals(left.From, right.From) && left.Link == right.Link;
		}
		public static bool operator !=(Connection<T> left, Connection<T> right) {
			return !(left == right);
		}
		public readonly Link<T> Link;
		public readonly T From;
		public double Weight { get { return Link.Weight; } }
		public T To { get { return Link.To; } }
		public Connection(T from, Link<T> link) {
			From = from;
			Link = link;
		}
		public Connection(T from, T to, double weight = 1) 
			: this(from, new Link<T>(to, weight)) {
		}
		public override bool Equals(object obj) {
			if(!(obj is Connection<T>))
				return false;
			return this == (Connection<T>)obj;
		}
		public override int GetHashCode() {
			return From.GetHashCode() ^ Link.GetHashCode();
		}
		public override string ToString() {
			return string.Format("{0} - {1}, {2}", From, To, Weight);
		}
		public Connection<T> SetWeight(double value) {
			return new Connection<T>(From, To, value);
		}
	}
	public struct Link<T> {
		public static bool operator ==(Link<T> left, Link<T> right) {
			return Equals(left.To, right.To) && Equals(left.Weight, right.Weight);
		}
		public static bool operator !=(Link<T> left, Link<T> right) {
			return !(left == right);
		}
		public readonly T To;
		public readonly double Weight;
		public Link(T to, double weight) {
			To = to;
			Weight = weight;
		}
		public override bool Equals(object obj) {
			if(!(obj is Link<T>))
				return false;
			return this == (Link<T>)obj;
		}
		public override int GetHashCode() {
			return To.GetHashCode() ^ Weight.GetHashCode();
		}
		public override string ToString() {
			return string.Format("{0}, {1}", To, Weight);
		}
		public Link<T> SetWeight(double value) {
			return new Link<T>(To, value);
		}
	}
	public struct Links<T> {
		public readonly Link<T>[] Outgoing, Incoming;
		public Links(Link<T>[] outgoing, Link<T>[] incoming) {
			Outgoing = outgoing;
			Incoming = incoming;
		}
		public IEnumerable<Link<T>> GetLinks(UseConnectionsMode useConnectionsMode = UseConnectionsMode.OutgoingAndIncoming) {
			return Enumerable.Concat(
				Outgoing,
				useConnectionsMode == UseConnectionsMode.OutgoingAndIncoming ? Incoming : EmptyArray<Link<T>>.Instance
			);
		}
	}
	public struct Tree<T> {
		public readonly T Root;
		public readonly Func<T, IEnumerable<T>> GetChildren;
		public Tree(T root, Func<T, IEnumerable<T>> getChildren) {
			Root = root;
			GetChildren = getChildren;
		}
	}
	public struct TreeAndItems<T> {
		public readonly Tree<T> Tree;
		public readonly T[] Items;
		public TreeAndItems(Tree<T> tree, T[] items) {
			Tree = tree;
			Items = items;
		}
	}
}
