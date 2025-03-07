﻿namespace DeansOffice.Sets
{
    public class Node<T>
    {
        public T Value;
        public int Key;
        public Node<T> Next;
        public ILock Lock = new TTASLock();
        public bool Marked = false;

        public Node(int key)
        {
            Key = key;
        }

        public Node(T value)
        {
            Value = value;
            Key = Value.GetHashCode();
        }

        public override int GetHashCode()
        {
            return Key;
        }
    }
}
