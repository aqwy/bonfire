using UnityEngine;
using System.Collections.Generic;

namespace Destructible2D
{
	public class D2dLinkedList<T>
		where T : class
	{
		public class Node
		{
			public Node Prev;
			public Node Next;
			public int  Index;
			public T    Value;
		}
		
		public int Count;
		
		public List<Node> Nodes = new List<Node>();
		
		public Stack<int> Pool = new Stack<int>();
		
		public Node First;
		
		public Node Last;
		
		public void Clear()
		{
			Count = 0;
			First = null;
			Last  = null;
			
			Pool.Clear();
			
			for (var i = Nodes.Count - 1; i >= 0; i--)
			{
				Pool.Push(i);
			}
		}
		
		public Node AddFirst(T newValue)
		{
			var node = GetNode(newValue);
			
			if (First != null)
			{
				node.Next = First;
				
				First.Prev = node;
				
				First = node;
			}
			else
			{
				First = node;
				Last  = node;
			}
			
			return node;
		}
		
		public Node AddLast(T newValue)
		{
			var node = GetNode(newValue);
			
			if (Last != null)
			{
				node.Prev = Last;
				
				Last.Next = node;
				
				Last = node;
			}
			else
			{
				First = node;
				Last  = node;
			}
			
			return node;
		}
		
		public void Remove(Node n)
		{
			if (n == First)
			{
				First = n.Next;
			}
			
			if (n == Last)
			{
				Last = n.Prev;
			}
			
			if (n.Prev != null)
			{
				n.Prev.Next = n.Next;
			}
			
			if (n.Next != null)
			{
				n.Next.Prev = n.Prev;
			}
			
			Count -= 1;
		}
		
		private Node GetNode(T newValue)
		{
			Count += 1;
			
			var node = default(Node);
			
			if (Pool.Count > 0)
			{
				node = Nodes[Pool.Pop()];
			}
			else
			{
				node = new Node();
				
				node.Index = Nodes.Count;
				
				Nodes.Add(node);
			}
			
			node.Prev  = null;
			node.Next  = null;
			node.Value = newValue;
			
			return node;
		}
	}
}