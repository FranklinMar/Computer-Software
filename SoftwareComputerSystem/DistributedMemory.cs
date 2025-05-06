using ParallelTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoftwareComputerSystem.DistributedMemory.MemoryAllocationEvent;
using static SoftwareComputerSystem.MemoryBlock;

namespace SoftwareComputerSystem
{
    public class DistributedMemory
    {
        public class MemoryAllocationEvent
        {
            public enum MemoryEventType
            {
                Allocation, Release
            }
            public int TickStart { get; set; }
            public int TickEnd { get; set; }
            public MemoryEventType EventType { get; set; }
            public int BlockAddress { get; set; }
            public Tree Node { get; set; }
            public override string ToString()
            {
                string Type = Node is TreeValue ? "value" : "operation";
                return $"Cycle {TickStart} - {TickEnd}: {EventType} of block {BlockAddress} for {Type} '{Node.Value}' of Node ID: {Node.GetHashCode():X8}";
            }
        }
        private Dictionary<int, MemoryBlock> MemoryBlocks = new Dictionary<int, MemoryBlock>();
        private int NextFreeBlock = 0;
        public int MemoryAccessTime { get; private set; }
        public int BlocksCount { get; private set; }
        public List<MemoryAllocationEvent> AllocationHistory { get; private set; } = new List<MemoryAllocationEvent>();
        public MemoryBlock this[int blockAddress]
        {
            get
            {
                if (MemoryBlocks.TryGetValue(blockAddress, out MemoryBlock block))
                {
                    return block;
                }
                return null;
            }
        }

        public DistributedMemory(int totalBlocks = 16, int memoryAccessTime = 1)
        {
            BlocksCount = totalBlocks;
            MemoryAccessTime = memoryAccessTime;
            for (int i = 0; i < BlocksCount; i++)
            {
                MemoryBlocks[i] = new MemoryBlock(i);
            }
        }

        public int AllocateMemory(Tree node, int CurrentTick/*, out int BlockAddress*/)
        {
            int BlockAddress = -1;
            //BlockAddress = -1;
            for (int i = 0; i < BlocksCount; i++)
            {
                int Address = (NextFreeBlock + i) % BlocksCount;
                if (!MemoryBlocks[Address].IsOccupied)
                {
                    BlockAddress = Address;
                    NextFreeBlock = (Address + 1) % BlocksCount;
                    break;
                }
            }

            if (BlockAddress == -1)
            {
                throw new OutOfMemoryException("Not enough memory blocks for operation");
            }
            /*if (BlockAddress == -1)
            {
                return false;
            }*/

            int allocationCompleteTick = CurrentTick + MemoryAccessTime;
            MemoryBlocks[BlockAddress].Allocate(node, allocationCompleteTick);
            AllocationHistory.Add(new MemoryAllocationEvent
            {
                TickStart = CurrentTick,
                TickEnd = allocationCompleteTick,
                EventType = MemoryEventType.Allocation,
                BlockAddress = BlockAddress,
                Node = node
            });
            return BlockAddress;
            //return allocationCompleteTick;
            //return true;
        }
        public int ReleaseMemory(int blockAddress, int CurrentTick)
        {
            if (MemoryBlocks.TryGetValue(blockAddress, out MemoryBlock block) && block.IsOccupied)
            {
                int releaseCompleteTick = CurrentTick + MemoryAccessTime;

                AllocationHistory.Add(new MemoryAllocationEvent
                {
                    TickStart = CurrentTick,
                    TickEnd = releaseCompleteTick,
                    EventType = MemoryEventType.Release,
                    BlockAddress = blockAddress,
                    Node = block.Node
                });
                block.Release(releaseCompleteTick);
                return releaseCompleteTick;
            }
            return CurrentTick;
        }

        public override string ToString()
        {
            StringBuilder SB = new StringBuilder();
            SB.Append('(');
            for (int i = 1; i <= MemoryBlocks.Count; i++) {
                MemoryBlock Block = MemoryBlocks[i - 1];
                SB.Append(Block);
                if (i != MemoryBlocks.Count)
                {
                    SB.Append(',');
                }
                if (i % 4 == 0)
                {
                    SB.Append('\n');
                }
            }
            SB.Append(')');
            return SB.ToString();
        }
    }
}
