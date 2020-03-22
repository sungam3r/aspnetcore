// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Http.HPack;

namespace Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack
{
    [DebuggerDisplay("Name = {Name} Value = {Value}")]
    internal class HPackHeaderEntry
    {
        // Header name and value
        public string Name;
        public string Value;

        // Chained list of headers in the same bucket
        public HPackHeaderEntry Next;
        public int Hash;

        // Compute dynamic table index
        public int Index;

        // Doubly linked list
        public HPackHeaderEntry Before;
        public HPackHeaderEntry After;

        /// <summary>
        /// Initialize header values. An entry will be reinitialized when reused.
        /// </summary>
        public void Initialize(int hash, string name, string value, int index, HPackHeaderEntry next)
        {
            Debug.Assert(name != null);
            Debug.Assert(value != null);

            Name = name;
            Value = value;
            Index = index;
            Hash = hash;
            Next = next;
        }

        public uint CalculateSize()
        {
            return (uint)HeaderField.GetLength(Name.Length, Value.Length);
        }

        /// <summary>
        /// Remove entry from the linked list and reset header values.
        /// </summary>
        public void Remove()
        {
            Before.After = After;
            After.Before = Before;
            Before = null;
            After = null;
            Next = null;
            Hash = 0;
            Name = null;
            Value = null;
        }

        /// <summary>
        /// Add before an entry in the linked list.
        /// </summary>
        public void AddBefore(HPackHeaderEntry existingEntry)
        {
            After = existingEntry;
            Before = existingEntry.Before;
            Before.After = this;
            After.Before = this;
        }
    }
}
