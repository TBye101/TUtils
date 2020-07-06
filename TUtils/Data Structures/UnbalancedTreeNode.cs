using System;
using System.Collections.Generic;

namespace TUtils.Data_Structures
{
        /// <summary>
        /// A unspecialized unbalanced tree node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class UnbalancedTreeNode<T> : IEquatable<UnbalancedTreeNode<T>>
            where T : IEquatable<T>
        {
            public T Value { get; set; }
            public UnbalancedTreeNode<T> Parent { get; set; }
            private readonly List<UnbalancedTreeNode<T>> Children;

            public UnbalancedTreeNode(T value, UnbalancedTreeNode<T> parent)
            {
                this.Value = value;
                this.Children = new List<UnbalancedTreeNode<T>>();
                this.Parent = parent;
            }

            public void AddChild(UnbalancedTreeNode<T> child)
            {
                this.Children.Add(child);
            }

            public void RemoveChild(UnbalancedTreeNode<T> child)
            {
                this.Children.Remove(child);
            }

            public IReadOnlyList<UnbalancedTreeNode<T>> GetChildren()
            {
                return this.Children;
            }

            public bool IsRoot()
            {
                return this.Parent == null;
            }

            public bool HasChildren()
            {
                return this.Children.Count > 0;
            }

            public bool Equals(UnbalancedTreeNode<T> other)
            {
                return this.Value.Equals(other.Value);
            }
        }
}