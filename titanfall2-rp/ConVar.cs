using System;
using System.Text;
using Process.NET;

namespace titanfall2_rp
{
/*
    class ConVar
    {
    public:
	    char pad_0x0000[0x4]; //0x0000
	    ConVar* m_pNext; //0x0004 
	    __int32 m_bRegistered; //0x0008 change to bol
	    char* m_pszName; //0x000C 
	    char* m_pszDescription; //0x0010 
	    __int32 m_nFlags; //0x0014 
	    char pad_0x0018[0x4]; //0x0018
	    ConVar* m_pParent; //0x001C 
	    char* m_pszDefaultValue; //0x0020 
	    char* m_pszValue; //0x0024 
	    __int32 m_nSize; //0x0028 
	    float m_flValue; //0x002C 
	    __int32 m_nValue; //0x0030 
	    __int32 m_bHasMin; //0x0034  change to bool
	    float m_fMinVal; //0x0038 
	    __int32 m_bHasMax; //0x003C  change to bool
	    float m_fMaxVal; //0x0040 

    };//Size=0x0044
    */
    internal class ConVar
    {
        private static int _address;
        private readonly ProcessSharp _sharp;

        public ConVar(int ptr, ProcessSharp sharp)
        {
            this._sharp = sharp;
            _address = ptr;
        }

        public int Pointer
        {
            get
            {
                return _address;
            }
        }

        public ConVar GetNext()
        {
            return new ConVar(_sharp!.Memory.Read<int>((IntPtr)(_address + 0x4)),_sharp);
        }

        public bool IsRegistered
        {
            get
            {
                return _sharp.Memory.Read<bool>((IntPtr)(_address + 0x8));
            }
        }

        public string Name
        {
            get
            {
                return Encoding.Default.GetString(_sharp.Memory.Read((IntPtr)_sharp.Memory.Read<int>((IntPtr)(_address + 0xC)),255));
            }
        }

        public string Description
        {
            get
            {
                return Encoding.Default.GetString(_sharp.Memory.Read((IntPtr)_sharp.Memory.Read<int>((IntPtr)(_address + 0x10)),255));
            }
        }

        public int Flags
        {
            get
            {
                return _sharp.Memory.Read<int>((IntPtr)(_address + 0x14));
            }
        }

        public ConVar GetParent()
        {
            return new ConVar(_sharp.Memory.Read<int>((IntPtr)(_address + 0x1C)),_sharp);
        }
        public string DefaultValue
        {
            get
            {
                return Encoding.Default.GetString(_sharp.Memory.Read((IntPtr)_sharp.Memory.Read<int>((IntPtr)(_address + 0x20)), 255));
            }
        }

        public string GetString()
        {
            return Encoding.Default.GetString(_sharp.Memory.Read((IntPtr)_sharp.Memory.Read<int>((IntPtr)(_address + 0x24)),255));
        }
        public int GetSize()
        {
            return _sharp.Memory.Read<int>((IntPtr)(_address + 0x28));
        }
        public float GetFloat()
        {
            return _sharp.Memory.Read<float>((IntPtr)(_address + 0x2C));
        }

        public int GetInt()
        {
            return _sharp.Memory.Read<int>((IntPtr)(_address + 0x30));
        }

        public bool HasMin()
        {
            return _sharp.Memory.Read<bool>((IntPtr)(_address + 0x34));
        }

        public float GetMinValue()
        {
            return _sharp.Memory.Read<float>((IntPtr)(_address + 0x38));
        }
        public bool HasMax()
        {
            return _sharp.Memory.Read<bool>((IntPtr)(_address + 0x3C));
        }

        public float GetMaxValue()
        {
            return _sharp.Memory.Read<float>((IntPtr)(_address + 0x40));
        }

        public void SetValue(string val)
        {
            _sharp.Memory.Write((IntPtr)_sharp.Memory.Read<int>((IntPtr)(_address + 0x24)), Encoding.Default.GetBytes(val));
        }

        public void SetValue(float val)
        {
            _sharp.Memory.Write<float>((IntPtr)(_address + 0x2C), val);
        }
        public void SetValue(int val)
        {
            _sharp.Memory.Write<float>((IntPtr)(_address + 0x30), val);
        }
    }
}