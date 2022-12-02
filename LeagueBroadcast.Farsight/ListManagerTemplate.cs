using System;
using System.Collections;
using System.Collections.Generic;

namespace LeagueBroadcast.Farsight
{

    //Taken from Jiingz on unknowncheats.me
    public class ListManagerTemplate : IEnumerable<GameObject>
    {
        private int offset;

        public ListManagerTemplate(int offset)
        {
            this.offset = offset;
        }

        /// <summary>
        /// Returns the ManagerTemplate Unit Pointer which contains all Units if accessed.
        /// </summary>
        /// <returns>Address of the unit pointer</returns>
        private int GetUnitPointer()
        {
            int template = MemoryUtils.ReadMemory<int>(MemoryUtils.m_baseAddress + offset);

            int unitVector = MemoryUtils.ReadMemory<int>(MemoryUtils.m_baseAddress + template + 0x4);

            return unitVector;
        }

        /// <summary>
        /// Returns the size of the ManagerTemplate
        /// </summary>
        /// <returns>Size of the ManagerTemplate</returns>
        private int GetSize()
        {
            int template = MemoryUtils.ReadMemory<int>(MemoryUtils.m_baseAddress + offset);

            return MemoryUtils.ReadMemory<int>(MemoryUtils.m_baseAddress + template + 0x8);
        }


        public IEnumerator<GameObject> GetEnumerator()
        {
            GameObject unit = new GameObject();
            for (int i = 0; i < GetSize(); i++)
            {
                int unitPointer = MemoryUtils.ReadMemory<int>(MemoryUtils.m_baseAddress + GetUnitPointer());

                //Read memory region to gameobject
                //MemoryUtils.ReadMemory<GameObject>(Constants.handle, (IntPtr)(unitPointer + i * 0x4), ref unit);

                yield return unit;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
