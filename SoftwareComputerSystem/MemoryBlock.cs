using ParallelTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareComputerSystem
{
    public class MemoryBlock
    {
        public int Address { get; }
        public Tree Node { get; private set; }
        public bool IsOccupied => Node != null;
        public int CycleOccupied { get; private set; } = -1;
        public int CycleReleased { get; private set; } = -1;

        public MemoryBlock(int address)
        {
            Address = address;
        }

        public void Allocate(Tree node, int CurrectTick)
        {
            Node = node;
            CycleOccupied = CurrectTick;
        }

        public void Release(int CurrentTick)
        {
            Node = null;
            CycleReleased = CurrentTick;
        }
        public override string ToString()
        {
            if (!IsOccupied)
            {
                return $"Block[{Address:8}]: {new string('-', 8)}";
            }
            return $"Block[{Address:8}]: {Node.GetHashCode():X8}";
        }
    }
}
