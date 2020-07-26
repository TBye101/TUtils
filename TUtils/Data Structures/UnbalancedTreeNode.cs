using System;
using System.Collections.Generic;
using TUtils.IO;

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
                child.Parent = this;
                this.Children.Add(child);
            }

            public void RemoveChild(UnbalancedTreeNode<T> child)
            {
                this.Children.Remove(child);
                child.Parent = null;
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

            /// <summary>
            /// Prints this tree to the specified log file.
            /// </summary>
            /// <param name="indent"></param>
            public void LogTree(Log log, string indent = "")
            {
                this.PrintNode(this, true, log, indent);
            }

            //Adapted from: https://stackoverflow.com/a/8567550
            protected void PrintNode(UnbalancedTreeNode<T> other, bool last, Log log, string indent = "")
            {
                log.Write(LogLevel.Debug, indent + "+- " + string.Format("[{0}]", other.Value));
                indent += last ? "   " : "|  ";

                for (int i = 0; i < other.Children.Count; i++)
                {
                    PrintNode(other.Children[i], i == other.Children.Count - 1, log, indent);
                }
            }
        }
}