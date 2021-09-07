using System;
using System.Collections.Generic;
using System.Text;

namespace titanfall2_rp
{
    internal class ConvarManager
    {
        private int m_pICVar = 0;
        private Dictionary<string, int> ConVars = new Dictionary<string, int>();
        private readonly Titanfall2Api _tf2Api;

        public ConvarManager(int pCvar, Titanfall2Api titanfall2Api)
        {
            this._tf2Api = titanfall2Api;
            m_pICVar = pCvar;

            int hashMapEntry;
            if (m_pICVar != 0)
            {
                //bucket table                                                 TODO MIGHT NEED TO USE 0x PREFIX!!!
                var shortCuts = _tf2Api._sharp!.Memory.Read<int>((IntPtr)(m_pICVar + 52)); //m_pCVarList
                hashMapEntry = _tf2Api._sharp!.Memory.Read<int>((IntPtr)shortCuts); //ptr to list

                //walk list
                while (hashMapEntry != 0)
                {
                    var pConVar = _tf2Api._sharp!.Memory.Read<int>((IntPtr)(hashMapEntry + 4)); //entry
                    var pConVarName = Encoding.Default.GetString(_tf2Api._sharp!.Memory.Read((IntPtr)_tf2Api._sharp!.Memory.Read<int>((IntPtr)(pConVar + 12)),255));

                    if (!ConVars.ContainsValue(pConVar))
                        ConVars.Add(pConVarName.ToLower(), pConVar);

                    hashMapEntry = _tf2Api._sharp!.Memory.Read<int>((IntPtr)(hashMapEntry + 4));
                }
            }
        }

        public ConVar FindFast(string str)
        {
            if (ConVars.ContainsKey(str.ToLower()))
                return new ConVar(ConVars[str.ToLower()],_tf2Api);

            return new ConVar(0,_tf2Api);
        }

        public int Find(string str)
        {
            int hashMapEntry;

            if (m_pICVar != 0)
            {
                //bucket table
                var shortCuts = _tf2Api._sharp!.Memory.Read<int>((IntPtr)(m_pICVar + 52)); //m_pCVarList
                hashMapEntry = _tf2Api._sharp!.Memory.Read<int>((IntPtr)shortCuts); //ptr to list

                //walk list
                while (hashMapEntry != 0)
                {
                    var pConVar = _tf2Api._sharp!.Memory.Read<int>((IntPtr)(hashMapEntry + 4)); //entry
                    var pConVarName = Encoding.Default.GetString(_tf2Api._sharp!.Memory.Read((IntPtr)_tf2Api._sharp!.Memory.Read<int>((IntPtr)(pConVar + 12)), 255));
                    if (pConVarName.ToLower() == str.ToLower())
                    {
                        //found the nigger
                        return pConVar;
                    }
                    hashMapEntry = _tf2Api._sharp!.Memory.Read<int>((IntPtr)(hashMapEntry + 4));
                }
            }

            return 0;
        }
    }
}